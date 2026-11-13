using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

public static class CentroOesteRemodelagem
{
    private const string ScenePath = "Assets/Scenes/Cidade/CentroSantaRita.unity";
    private const string PavimentosPath = "Assets/CentroSantaRita/Pavimentos/";
    private const string Bairro = "Art/RuaNorte/Bairro/Detalhes/";
    private const string Modulos = "Art/RuaNorte/Entrada/Modulos/";
    private const string Vegetacao = "Art/RuaNorte/Vegetacao/";
    private const string GrupoRua = "Remodelagem Oeste · Rua e Detalhes";
    private const string GrupoBosque = "Bosques e matagal oeste";

    [MenuItem("Santa Rita/Centro/Remodelar Oeste e Acesso Sociedade")]
    public static void ExecutarPeloMenu()
    {
        string resultado = Run();
        Debug.Log("[CentroOesteRemodelagem] " + resultado);
    }

    public static string Run()
    {
        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || scene.name != "CentroSantaRita")
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        var root = GameObject.Find("Capitulo2_CentroSantaRita");
        if (root == null)
            throw new Exception("Objeto raiz 'Capitulo2_CentroSantaRita' não encontrado na cena.");

        var floor = Object.FindObjectsByType<Tilemap>(FindObjectsInactive.Include)
            .FirstOrDefault(m => m.name == "Pavimentos do Centro");
        if (floor == null)
            throw new Exception("Tilemap 'Pavimentos do Centro' não encontrado.");

        var mata = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include)
            .FirstOrDefault(t => t.name == "Matagal das ruas");
        if (mata == null)
            throw new Exception("Objeto 'Matagal das ruas' não encontrado.");

        int tilesAlterados = RemodelarPavimentos(floor);
        int arvoresCriadas = RemodelarArvores(root.transform, mata);
        int detalhesCriados = RemodelarRuaLuzesEDetalhes(root.transform);

        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene))
            throw new Exception("Falha ao salvar a cena CentroSantaRita.unity.");

        string qa = VerificarAcessos();
        return $"Remodelagem concluída: {tilesAlterados} tiles ajustados, {arvoresCriadas} árvores/arbustos, {detalhesCriados} postes/luzes/detalhes.\n{qa}";
    }

    private static int RemodelarPavimentos(Tilemap floor)
    {
        TileBase[] LoadSet(string nome, int count) =>
            Enumerable.Range(0, count)
                .Select(i => AssetDatabase.LoadAssetAtPath<TileBase>($"{PavimentosPath}{nome}_{i}.asset"))
                .ToArray();

        var grama = LoadSet("Grama", 4);
        var terra = LoadSet("Terra", 4);
        var pedra = LoadSet("Pedregulho", 4);
        var calcada = LoadSet("Calcada", 2);
        var asfalto = LoadSet("Asfalto", 4);

        if (grama.Concat(terra).Concat(pedra).Concat(calcada).Concat(asfalto).Any(t => t == null))
            throw new Exception("Um ou mais assets de pavimento estão ausentes em " + PavimentosPath);

        TileBase Pick(TileBase[] set, int x, int y) => set[(x * 73 ^ y * 193) & (set.Length - 1)];
        TileBase Pick2x2(TileBase[] set, int x, int y) => set[(x & 1) + (1 - (y & 1)) * 2];

        Undo.RegisterCompleteObjectUndo(floor, "Remodelar pavimentos oeste e acesso Sociedade");
        int changed = 0;

        void SetTile(int x, int y, TileBase tile)
        {
            var pos = new Vector3Int(x, y, 0);
            if (floor.GetTile(pos) == tile) return;
            floor.SetTile(pos, tile);
            changed++;
        }

        // 1. Limpar blocos quadrados antigos e o caminho diagonal anterior na área oeste/sul
        //    (preservando o asfalto principal x=29..33, y>=22 e a margem sul do rio y<0)
        for (int y = 0; y <= 55; y++)
        {
            for (int x = -2; x <= 36; x++)
            {
                if (x >= 29 && y >= 22) continue; // pista principal e lado leste preservados
                if (x == 28 && y >= 23) continue; // meio-fio oeste da via principal
                if (x >= 34 && y >= 22) continue; // praça/leste preservados

                var old = floor.GetTile(new Vector3Int(x, y, 0));
                if (old == null) continue;
                string n = old.name;
                if (n.StartsWith("Calcada") || n.StartsWith("Terra") || n.StartsWith("Pedregulho") || n.StartsWith("Asfalto"))
                    SetTile(x, y, Pick(grama, x, y));
            }
        }

        // 2. Transição natural no fim do asfalto principal (y=21..22, x=28..33)
        //    O asfalto termina suavemente em pedregulho/terra batida que segue direto para o sul
        for (int x = 29; x <= 32; x++)
        {
            SetTile(x, 21, Pick2x2(pedra, x, 21));
            SetTile(x, 20, Pick(terra, x, 20));
        }
        SetTile(28, 22, Pick(calcada, 28, 22));
        SetTile(28, 21, Pick2x2(pedra, 28, 21));
        SetTile(33, 21, Pick(terra, 33, 21));

        // 3. Rua de chão (Terra) em L conectando a estrada principal à última casa (Sociedade)
        //    Sem escadinha diagonal: trecho reto norte-sul alinhado com a pista + trecho reto leste-oeste
        //    Trecho norte-sul: x = 29..32, y = 2..19
        for (int y = 2; y <= 19; y++)
        {
            for (int x = 29; x <= 32; x++)
                SetTile(x, y, Pick(terra, x, y));
        }

        //    Trecho leste-oeste: x = 8..31, y = 2..4
        for (int y = 2; y <= 4; y++)
        {
            for (int x = 8; x <= 31; x++)
                SetTile(x, y, Pick(terra, x, y));
        }

        //    Suavização da curva interna (canto x=28, y=5) e canto externo (x=32, y=2)
        SetTile(28, 5, Pick(terra, 28, 5));
        SetTile(32, 2, Pick(grama, 32, 2));

        //    Soleira limpa em frente à Sociedade (x=3..7, y=3..4) com transição em terra
        for (int x = 3; x <= 7; x++)
        {
            for (int y = 3; y <= 4; y++)
                SetTile(x, y, Pick(calcada, x, y));
            SetTile(x, 2, Pick(terra, x, 2));
        }
        SetTile(2, 3, Pick(terra, 2, 3));
        SetTile(2, 4, Pick(terra, 2, 4));

        // 4. Acesso limpo e realista da Casa da Costureira (porta ~ 21.8, 27.5)
        for (int x = 19; x <= 27; x++)
        {
            SetTile(x, 27, Pick(terra, x, 27));
            SetTile(x, 28, x >= 20 && x <= 23 ? Pick(calcada, x, 28) : Pick(terra, x, 28));
        }
        SetTile(28, 27, Pick2x2(pedra, 28, 27));
        SetTile(28, 28, Pick2x2(pedra, 28, 28));

        // 5. Acesso limpo e realista da Oficina de Rádio (porta ~ 11.2, 33.2)
        for (int x = 9; x <= 27; x++)
        {
            SetTile(x, 32, Pick(terra, x, 32));
            SetTile(x, 33, x >= 9 && x <= 14 ? Pick(calcada, x, 33) : Pick(terra, x, 33));
        }
        SetTile(28, 32, Pick2x2(pedra, 28, 32));
        SetTile(28, 33, Pick2x2(pedra, 28, 33));

        // 6. Acesso limpo e realista dos Fundos do Mercadinho (porta ~ 20.2, 48.7)
        for (int x = 18; x <= 27; x++)
        {
            SetTile(x, 48, Pick(terra, x, 48));
            SetTile(x, 49, x >= 18 && x <= 22 ? Pick(calcada, x, 49) : Pick(terra, x, 49));
        }
        SetTile(28, 48, Pick2x2(pedra, 28, 48));
        SetTile(28, 49, Pick2x2(pedra, 28, 49));

        return changed;
    }

    private static int RemodelarArvores(Transform root, Transform mata)
    {
        var antigoBosque = mata.Find("Bosques junto às casas");
        if (antigoBosque != null)
        {
            Undo.RegisterFullObjectHierarchyUndo(antigoBosque.gameObject, "Substituir bosque antigo");
            Object.DestroyImmediate(antigoBosque.gameObject);
        }

        var existente = mata.Find(GrupoBosque);
        if (existente != null)
            Object.DestroyImmediate(existente.gameObject);

        // Desativa árvores/matagal pré-existentes cujo tronco caia sobre a nova rua de chão em L
        bool SobreRuaEmL(Vector3 p)
        {
            bool naVertical = p.x >= 27.6f && p.x <= 33.9f && p.y >= 1.2f && p.y <= 22.0f;
            bool naHorizontal = p.x >= 2.5f && p.x <= 33.9f && p.y >= 1.2f && p.y <= 5.6f;
            return naVertical || naHorizontal;
        }

        foreach (Transform child in mata)
        {
            if (child.name == GrupoBosque) continue;
            if (SobreRuaEmL(child.position) && child.gameObject.activeSelf)
            {
                Undo.RecordObject(child.gameObject, "Limpar árvore sobre a rua de chão");
                child.gameObject.SetActive(false);
            }
        }

        // Também desativa árvores soltas diretamente sob Capitulo2_CentroSantaRita que bloqueiam a rua em L
        foreach (Transform child in root)
        {
            if (!child.gameObject.activeSelf) continue;
            string n = child.name;
            if (!(n.Contains("Arvore") || n.Contains("Árvore") || n.Contains("Matagal") || n.Contains("Arbusto")))
                continue;
            if (SobreRuaEmL(child.position))
            {
                Undo.RecordObject(child.gameObject, "Limpar árvore solta sobre a rua de chão");
                child.gameObject.SetActive(false);
            }
        }

        // Também desativa árvores em "Rua vertical - arte" que estejam sobre a rua em L
        var ruaVerticalArte = GameObject.Find("Rua vertical - arte");
        if (ruaVerticalArte != null)
        {
            foreach (var sr in ruaVerticalArte.GetComponentsInChildren<SpriteRenderer>(false))
            {
                if (SobreRuaEmL(sr.transform.position))
                {
                    Undo.RecordObject(sr.gameObject, "Limpar árvore de Rua vertical - arte sobre a rua de chão");
                    sr.gameObject.SetActive(false);
                }
            }
        }

        var modelos = mata.Cast<Transform>()
            .Where(t => t.name.StartsWith("Árvore nova", StringComparison.Ordinal) && t.gameObject.activeInHierarchy)
            .ToArray();

        var grupo = new GameObject(GrupoBosque);
        Undo.RegisterCreatedObjectUndo(grupo, "Criar bosques e matagal oeste");
        grupo.transform.SetParent(mata, false);

        var worldLit = Resources.Load<Material>("Art/WorldLit");
        int count = 0;

        // Árvores principais aproximando o matagal das casas e preenchendo o miolo junto à rua de chão
        var arvores = new (float x, float y, string especie, float scale)[]
        {
            // Ao redor do Fundos_MesmoMercadinho (casa topo ~ 20.2, 49..53)
            (15.2f, 52.8f, "Camboata", 1.02f),
            (17.1f, 54.4f, "Araucaria", 1.05f),
            (20.0f, 55.0f, "Pitangueira", 0.96f),
            (23.3f, 54.1f, "Bracatinga", 1.00f),
            (25.6f, 52.4f, "CamboataJovem", 0.92f),
            (24.9f, 50.4f, "Pitangueira", 0.90f),
            (15.6f, 49.8f, "BracatingaJovem", 0.94f),
            (15.3f, 46.2f, "Camboata", 0.98f),
            (17.6f, 45.2f, "Pitangueira", 0.90f),
            (22.8f, 45.1f, "BracatingaJovem", 0.88f),
            (25.7f, 45.6f, "Camboata", 0.95f),

            // Conectando o matagal entre a casa do topo e a Oficina de Rádio (y=36..44)
            (14.5f, 42.6f, "Araucaria", 1.04f),
            (18.2f, 42.1f, "Camboata", 0.96f),
            (21.6f, 41.8f, "Pitangueira", 0.94f),
            (24.8f, 41.2f, "Bracatinga", 0.98f),
            (16.2f, 38.8f, "BracatingaJovem", 0.90f),
            (19.5f, 38.2f, "Camboata", 1.00f),
            (23.1f, 38.5f, "Pitangueira", 0.94f),
            (25.9f, 37.9f, "CamboataJovem", 0.88f),
            (15.4f, 35.8f, "Pitangueira", 0.88f),
            (18.8f, 35.4f, "BracatingaJovem", 0.90f),
            (22.4f, 35.3f, "Camboata", 0.95f),
            (25.3f, 35.1f, "Pitangueira", 0.86f),

            // Ao redor da Oficina de Rádio (~ 11.2, 34) e Casa da Costureira (~ 21.8, 28.5)
            (6.8f, 35.8f, "Camboata", 1.02f),
            (7.6f, 32.4f, "Pitangueira", 0.94f),
            (14.2f, 30.2f, "CamboataJovem", 0.92f),
            (16.8f, 30.1f, "Pitangueira", 0.95f),
            (18.5f, 29.8f, "BracatingaJovem", 0.86f),
            (24.9f, 30.2f, "Camboata", 0.96f),
            (26.1f, 31.4f, "BracatingaJovem", 0.85f),
            (15.2f, 25.0f, "Camboata", 0.98f),
            (17.9f, 24.7f, "Pitangueira", 0.92f),
            (24.7f, 24.8f, "Bracatinga", 0.95f),
            (26.4f, 25.3f, "CamboataJovem", 0.84f),

            // Miolo arborizado entre a Casa da Costureira e a nova rua de chão (x=10..27, y=7..22)
            (1.2f, 7.8f, "Araucaria", 1.02f),
            (4.8f, 9.4f, "Camboata", 0.98f),
            (8.4f, 8.9f, "Pitangueira", 0.92f),
            (1.1f, 4.2f, "BracatingaJovem", 0.88f),
            (11.8f, 8.2f, "Camboata", 0.98f),
            (14.6f, 11.2f, "Araucaria", 1.04f),
            (18.4f, 8.6f, "Pitangueira", 0.94f),
            (22.2f, 7.8f, "Bracatinga", 0.98f),
            (25.6f, 9.4f, "CamboataJovem", 0.90f),
            (13.2f, 15.8f, "Bracatinga", 1.00f),
            (17.6f, 14.5f, "Camboata", 0.96f),
            (21.4f, 13.8f, "Pitangueira", 0.92f),
            (25.8f, 14.2f, "BracatingaJovem", 0.88f),
            (16.4f, 19.6f, "CamboataJovem", 0.90f),
            (20.6f, 19.8f, "Araucaria", 1.02f),
            (24.8f, 19.5f, "Pitangueira", 0.92f),
            (26.4f, 22.0f, "CamboataJovem", 0.86f),

            // Margem leste da rua de chão (x=34..36, y=3..20), conectando ao matagal leste
            (35.0f, 19.2f, "Pitangueira", 0.92f),
            (34.8f, 15.2f, "Camboata", 0.96f),
            (35.2f, 11.4f, "BracatingaJovem", 0.90f),
            (34.6f, 7.2f, "Pitangueira", 0.94f),
            (34.4f, 3.6f, "CamboataJovem", 0.88f),
        };

        for (int i = 0; i < arvores.Length; i++)
        {
            var (x, y, especie, scale) = arvores[i];
            GameObject go;
            if (modelos.Length > 0 && i % 2 == 0)
            {
                var src = modelos[i % modelos.Length];
                go = Object.Instantiate(src.gameObject, grupo.transform);
                go.name = $"Árvore bosque {(i + 1):00}";
                go.transform.position = new Vector3(x, y, 0);
                go.transform.localScale *= scale;
                var sr = go.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.flipX = (i % 2 == 1);
                    sr.sortingOrder = 1000 - Mathf.RoundToInt(y * 10);
                }
            }
            else
            {
                var sp = Resources.Load<Sprite>(Vegetacao + especie);
                if (sp == null) continue;
                go = new GameObject($"Vegetação {especie} {(i + 1):00}", typeof(SpriteRenderer));
                go.transform.SetParent(grupo.transform, false);
                go.transform.position = new Vector3(x, y, 0);
                float targetW = especie == "Araucaria" ? 4.2f : (especie.Contains("Jovem") ? 2.1f : 3.0f);
                float s = (targetW * scale) / sp.bounds.size.x;
                go.transform.localScale = new Vector3(s, s, 1);
                var sr = go.GetComponent<SpriteRenderer>();
                sr.sprite = sp;
                sr.sharedMaterial = worldLit;
                sr.flipX = (i % 2 == 0);
                sr.sortingOrder = 1000 - Mathf.RoundToInt(y * 10);
            }
            Undo.RegisterCreatedObjectUndo(go, "Criar árvores do bosque");
            count++;
        }

        // Moitas de borda, samambaias e folhiço junto às casas e margens da rua de chão
        var rasteiras = new (string asset, float x, float y, float w, bool noPiso)[]
        {
            ("MoitaDeBorda", 17.0f, 50.6f, 1.35f, false),
            ("MoitaDeBorda", 23.8f, 50.4f, 1.30f, false),
            ("SamambaiaDaMata", 16.4f, 47.4f, 1.05f, false),
            ("MoitaDeBorda", 8.6f, 34.4f, 1.30f, false),
            ("SamambaiaDaMata", 14.3f, 34.2f, 1.10f, false),
            ("MoitaDeBorda", 19.0f, 28.4f, 1.25f, false),
            ("SamambaiaDaMata", 24.6f, 28.1f, 1.05f, false),
            ("MoitaDeBorda", 1.8f, 5.2f, 1.40f, false),
            ("MoitaDeBorda", 13.5f, 6.2f, 1.35f, false),
            ("SamambaiaDaMata", 21.0f, 6.0f, 1.15f, false),
            ("MoitaDeBorda", 27.6f, 9.8f, 1.30f, false),
            ("MoitaDeBorda", 27.6f, 17.2f, 1.30f, false),
            ("SamambaiaDaMata", 33.8f, 13.5f, 1.15f, false),
            ("Folhico", 16.2f, 48.2f, 1.15f, true),
            ("Folhico", 12.8f, 32.1f, 1.15f, true),
            ("Folhico", 22.8f, 26.6f, 1.10f, true),
            ("Folhico", 15.0f, 4.9f, 1.20f, true),
            ("Folhico", 28.8f, 12.0f, 1.20f, true),
            ("GalhoCaido", 26.8f, 6.8f, 1.10f, true),
            ("GalhoCaido", 33.4f, 8.4f, 1.05f, true),
        };

        for (int i = 0; i < rasteiras.Length; i++)
        {
            var (asset, x, y, w, noPiso) = rasteiras[i];
            var sp = Resources.Load<Sprite>(Vegetacao + asset);
            if (sp == null) continue;
            var go = new GameObject($"SubBosque {asset} {(i + 1):00}", typeof(SpriteRenderer));
            Undo.RegisterCreatedObjectUndo(go, "Criar sub-bosque");
            go.transform.SetParent(grupo.transform, false);
            go.transform.position = new Vector3(x, y, 0);
            float s = w / sp.bounds.size.x;
            go.transform.localScale = new Vector3(s, s, 1);
            var sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = sp;
            sr.sharedMaterial = worldLit;
            sr.flipX = (i % 2 == 1);
            sr.sortingOrder = noPiso ? -1685 : (1000 - Mathf.RoundToInt(y * 10));
            count++;
        }

        return count;
    }

    private static int RemodelarRuaLuzesEDetalhes(Transform root)
    {
        // Limpa composições antigas da trilha para evitar duplicação
        foreach (var antigoNome in new[] { "Detalhes do caminho da Sociedade", GrupoRua })
        {
            var antigo = root.Find(antigoNome);
            if (antigo != null)
            {
                Undo.RegisterFullObjectHierarchyUndo(antigo.gameObject, "Limpar composição anterior");
                Object.DestroyImmediate(antigo.gameObject);
            }
        }

        // Limpa postes oeste anteriores em "Postes dos acessos" e desativa postes antigos da Sociedade
        var postesAcessos = root.Find("Postes dos acessos");
        if (postesAcessos != null)
        {
            var remover = new List<GameObject>();
            foreach (Transform child in postesAcessos)
            {
                if (child.name.StartsWith("Poste oeste · ", StringComparison.Ordinal) ||
                    child.name.StartsWith("Luz · Poste oeste · ", StringComparison.Ordinal))
                {
                    remover.Add(child.gameObject);
                }
                else if (child.name == "Poste da trilha da Sociedade" ||
                         child.name == "Poste da rua da Sociedade" ||
                         child.name == "Luz · Poste da rua da Sociedade" ||
                         (child.name == "Luz quente do poste" && Vector2.Distance(child.position, new Vector2(13.4f, 7.1f)) < 3.5f))
                {
                    Undo.RecordObject(child.gameObject, "Desativar poste antigo da Sociedade");
                    child.gameObject.SetActive(false);
                }
            }
            foreach (var go in remover)
            {
                Undo.DestroyObjectImmediate(go);
            }
        }

        // Limpa decalques anteriores em "Pisos e margens das entradas"
        var pisosMargens = root.Find("Pisos e margens das entradas");
        if (pisosMargens != null)
        {
            var remover = new List<GameObject>();
            foreach (Transform child in pisosMargens)
            {
                if (child.name.StartsWith("Detalhe oeste · ", StringComparison.Ordinal))
                    remover.Add(child.gameObject);
            }
            foreach (var go in remover)
                Undo.DestroyObjectImmediate(go);
        }

        var grupo = new GameObject(GrupoRua);
        Undo.RegisterCreatedObjectUndo(grupo, "Criar iluminação e detalhes oeste");
        grupo.transform.SetParent(root, false);

        var worldLit = Resources.Load<Material>("Art/WorldLit");
        int count = 0;

        void Decal(string nome, string path, float x, float y, float width, float angle, float alpha, bool flip = false)
        {
            var sp = Resources.Load<Sprite>(path);
            if (sp == null) return;
            var go = new GameObject("Detalhe oeste · " + nome, typeof(SpriteRenderer));
            Undo.RegisterCreatedObjectUndo(go, "Adicionar detalhe de piso");
            go.transform.SetParent(pisosMargens != null ? pisosMargens : grupo.transform, false);
            go.transform.position = new Vector3(x, y, 0);
            go.transform.rotation = Quaternion.Euler(0, 0, angle);
            float s = width / sp.bounds.size.x;
            go.transform.localScale = new Vector3(s, s, 1);
            var sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = sp;
            sr.flipX = flip;
            sr.color = new Color(.74f, .73f, .70f, alpha);
            sr.sharedMaterial = worldLit;
            sr.sortingOrder = -1690;
            count++;
        }

        void Prop(string nome, string path, float x, float y, float width, bool flip = false)
        {
            var sp = Resources.Load<Sprite>(path);
            if (sp == null) return;
            var go = new GameObject(nome, typeof(SpriteRenderer));
            Undo.RegisterCreatedObjectUndo(go, "Adicionar detalhe urbano");
            go.transform.SetParent(grupo.transform, false);
            go.transform.position = new Vector3(x, y, 0);
            float s = width / sp.bounds.size.x;
            go.transform.localScale = new Vector3(s, s, 1);
            var sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = sp;
            sr.flipX = flip;
            sr.sharedMaterial = worldLit;
            sr.sortingOrder = 1000 - Mathf.RoundToInt(y * 10);
            count++;
        }

        void Poste(string nome, float x, float y, bool flip)
        {
            var sp = Resources.Load<Sprite>("Art/RuaNorte/Poste");
            if (sp == null) return;
            var parentPostes = postesAcessos != null ? postesAcessos : grupo.transform;
            var go = new GameObject("Poste oeste · " + nome, typeof(SpriteRenderer), typeof(BoxCollider2D));
            Undo.RegisterCreatedObjectUndo(go, "Instalar poste oeste");
            go.transform.SetParent(parentPostes, false);
            go.transform.position = new Vector3(x, y, 0);
            float s = 1.08f / sp.bounds.size.x;
            go.transform.localScale = new Vector3(s, s, 1);
            var sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = sp;
            sr.flipX = flip;
            sr.sharedMaterial = worldLit;
            sr.sortingOrder = 1000 - Mathf.RoundToInt(y * 10);

            var col = go.GetComponent<BoxCollider2D>();
            col.size = new Vector2(.22f / s, .22f / s);
            col.offset = new Vector2((flip ? .32f : -.32f) / s, .11f / s);

            var b = sr.bounds;
            float bx = Mathf.Lerp(b.min.x, b.max.x, flip ? 1f - 58f / 72f : 58f / 72f);
            float by = Mathf.Lerp(b.min.y, b.max.y, 200f / 216f);
            var lightGo = new GameObject("Luz · Poste oeste · " + nome, typeof(Light2D));
            Undo.RegisterCreatedObjectUndo(lightGo, "Instalar luz do poste");
            lightGo.transform.SetParent(parentPostes, false);
            lightGo.transform.position = new Vector3(bx, by - 1.65f, 0);
            var light = lightGo.GetComponent<Light2D>();
            light.lightType = Light2D.LightType.Point;
            light.color = new Color(1f, .68f, .40f);
            light.intensity = .72f;
            light.pointLightInnerRadius = .55f;
            light.pointLightOuterRadius = 4.6f;

            Decal("Folhas ao pé do poste", Modulos + "Folhas", x + (flip ? -.18f : .18f), y - .12f, .80f, 0, .45f, flip);
            count += 2;
        }

        // 1. Detalhamento da estrada de chão no padrão do Capítulo 1 (SantaRitaRuaNorte.Bairro):
        //    rastros na lama no eixo, cascalho alternado nos acostamentos, beira de barro, folhiço e poças
        int idxV = 0;
        for (float y = 3.8f; y <= 20.6f; y += 2.15f + .35f * (idxV % 2), idxV++)
        {
            float lado = idxV % 2 == 0 ? 1f : -1f;
            Decal("Cascalho na margem", Bairro + "CascalhoDaVila",
                30.9f + lado * 1.55f, y + .25f, .62f + .14f * (idxV % 4), 17f * (idxV % 3), .26f, lado < 0);
            if (idxV % 2 == 0)
                Decal("Rastro na rua de chão", Bairro + "RastrosNaLama",
                    30.85f + lado * .18f, y, .92f + .12f * (idxV % 3), 0f, .25f, lado < 0);
            if (idxV % 3 == 0)
                Decal("Beira de terra", Bairro + "BeiraDeBarro",
                    30.9f - lado * 2.05f, y + .35f, .72f + .10f * (idxV % 3), lado > 0 ? -90f : 90f, .27f, lado > 0);
            else
                Decal("Folhiço na margem", Vegetacao + "Folhico",
                    30.9f + lado * 2.35f, y - .15f, .76f + .08f * (idxV % 3), 0f, .31f, lado < 0);
        }

        int idxH = 0;
        for (float x = 7.8f; x <= 29.2f; x += 2.15f + .35f * (idxH % 2), idxH++)
        {
            float lado = idxH % 2 == 0 ? 1f : -1f;
            Decal("Cascalho na margem", Bairro + "CascalhoDaVila",
                x + .25f, 3.55f + lado * 1.25f, .62f + .14f * (idxH % 4), 90f + 17f * (idxH % 3), .26f, lado < 0);
            if (idxH % 2 == 0)
                Decal("Rastro na rua de chão", Bairro + "RastrosNaLama",
                    x, 3.55f + lado * .16f, .92f + .12f * (idxH % 3), 90f, .25f, lado < 0);
            if (idxH % 3 == 0)
                Decal("Beira de terra", Bairro + "BeiraDeBarro",
                    x + .35f, 3.55f - lado * 1.65f, .72f + .10f * (idxH % 3), lado > 0 ? 0f : 180f, .27f, lado > 0);
            else
                Decal("Folhiço na margem", Vegetacao + "Folhico",
                    x - .15f, 3.55f + lado * 1.85f, .76f + .08f * (idxH % 3), 90f, .31f, lado < 0);
        }

        // Transição asfalto -> rua de chão, poças na pista e capim nas bordas
        Decal("Cascalho de transição", Bairro + "CascalhoDaVila", 30.1f, 21.3f, 1.02f, 90f, .38f);
        Decal("Cascalho de transição", Bairro + "CascalhoDaVila", 32.0f, 21.4f, .96f, 82f, .34f, true);
        Decal("Margem da poça", Bairro + "BordaDePoca", 29.7f, 18.4f, .92f, 8f, .24f);
        Decal("Margem da poça", Bairro + "BordaDePoca", 31.4f, 11.4f, .88f, -12f, .24f, true);
        Decal("Margem da poça", Bairro + "BordaDePoca", 29.9f, 5.1f, .90f, 15f, .23f);
        Decal("Margem da poça", Bairro + "BordaDePoca", 22.8f, 3.2f, .88f, -6f, .24f);
        Decal("Margem da poça", Bairro + "BordaDePoca", 14.6f, 2.8f, .84f, 10f, .23f, true);
        Decal("Capim de margem", Modulos + "Capim", 28.6f, 16.5f, .85f, 0, .55f);
        Decal("Capim de margem", Modulos + "Capim", 33.4f, 9.2f, .85f, 0, .55f, true);
        Decal("Capim de margem", Modulos + "Capim", 18.2f, 5.2f, .85f, 0, .55f);
        Decal("Capim de margem", Modulos + "Capim", 18.8f, 47.4f, .80f, 0, .55f);
        Decal("Capim de margem", Modulos + "Capim", 14.8f, 31.6f, .80f, 0, .55f, true);

        // 2. Postes e iluminação ao longo da rua de chão em L e das casas
        Poste("Poste transição asfalto-chão", 28.3f, 21.6f, false);
        Poste("Poste trecho norte da rua de chão", 33.2f, 16.5f, true);
        Poste("Poste trecho sul da rua de chão", 33.2f, 10.5f, true);
        Poste("Poste da esquina da Sociedade", 33.0f, 4.6f, true);
        Poste("Poste trecho leste da rua da Sociedade", 25.2f, 5.1f, false);
        Poste("Poste meio da rua da Sociedade", 17.5f, 5.1f, false);
        Poste("Poste chegada da Sociedade", 9.6f, 5.1f, false);
        Poste("Poste da rua da Oficina", 15.8f, 31.8f, false);
        Poste("Poste da entrada do Mercadinho", 16.9f, 47.6f, false);

        // 3. Mobília e detalhes realistas das casas e da rua
        Prop("Caixa de correio · Costureira", Bairro + "CorreioDeMadeira", 24.3f, 28.4f, .52f);
        Prop("Cadeira junto à Costureira", "Art/RuaNorte/Cadeira", 19.6f, 28.6f, .58f);
        Prop("Poste de rede · Oficina", Modulos + "PosteRede", 8.4f, 34.4f, .95f);
        Prop("Lixeira · Mercadinho", Modulos + "Lixeira", 23.6f, 49.0f, .56f);
        Prop("Caixotes · Mercadinho", "Art/RuaNorte/Caixotes", 17.6f, 49.1f, .78f);
        Prop("Caixa de correio · Sociedade", Bairro + "CorreioDeMadeira", 8.4f, 5.2f, .54f, true);
        Prop("Banco junto à Sociedade", "Art/Cap2/Remodelagem/Centro/Banco", 1.9f, 4.4f, 1.15f);
        Prop("Balizador do entroncamento", "Art/Cap2/Remodelagem/Centro/BalizadorCentro", 28.6f, 20.6f, .44f);
        Prop("Balizador leste do entroncamento", "Art/Cap2/Remodelagem/Centro/BalizadorCentro", 33.2f, 21.4f, .44f);
        Prop("Balizador da esquina da Sociedade", "Art/Cap2/Remodelagem/Centro/BalizadorCentro", 28.5f, 5.2f, .44f);
        Prop("Pedras da margem", Modulos + "Pedras", 28.7f, 7.2f, .85f);
        Prop("Pedras da margem norte", Modulos + "Pedras", 28.7f, 14.5f, .82f, true);
        Prop("Pedras junto à esquina", Modulos + "Pedras", 32.8f, 2.2f, .82f, true);
        Prop("Drenagem · transição asfalto", Bairro + "BueiroDaVila", 28.55f, 21.25f, .76f);
        Prop("Drenagem · esquina Sociedade", Bairro + "BueiroDaVila", 32.85f, 5.35f, .72f, true);

        // 4. Vagalumes com halo suave e iluminação real são gerenciados pelo shader Nevoeiro e Capitulo2Exterior.
        return count;
    }

    private static string VerificarAcessos()
    {
        var player = Object.FindAnyObjectByType<PlayerController>();
        if (player == null) return "QA de rota: PlayerController não encontrado na cena.";
        var pc = player.GetComponent<BoxCollider2D>();
        if (pc == null) return "QA de rota: BoxCollider2D do jogador não encontrado.";

        Physics2D.SyncTransforms();
        var destinos = new[]
        {
            ("MercadinhoDeposito", new Vector2(28, 48)),
            ("OficinaRadio", new Vector2(28, 32.5f)),
            ("CasaCostureira", new Vector2(28, 27)),
            ("SociedadeBeiraRio", new Vector2(30.5f, 22.5f))
        };

        var resultados = new List<string>();
        foreach (var (cena, inicio) in destinos)
        {
            var door = Object.FindObjectsByType<Capitulo2Portal>(FindObjectsInactive.Include)
                .FirstOrDefault(d => d.cenaDestino == cena);
            if (door == null)
            {
                resultados.Add($"{cena}=SEM_PORTAL");
                continue;
            }

            Vector2 goal = (Vector2)door.transform.position + Vector2.down * .65f;
            const float step = .35f;
            float minx = Mathf.Min(goal.x, inicio.x) - 4f, maxx = Mathf.Max(goal.x, inicio.x) + 4f;
            float miny = Mathf.Min(goal.y, inicio.y) - 4f, maxy = Mathf.Max(goal.y, inicio.y) + 4f;
            int w = Mathf.CeilToInt((maxx - minx) / step) + 1;
            int h = Mathf.CeilToInt((maxy - miny) / step) + 1;

            Vector2 Point(int x, int y) => new(minx + x * step, miny + y * step);
            bool Free(Vector2 p) => !Physics2D.OverlapBoxAll(p + pc.offset, pc.size * .95f, 0)
                .Any(c => c.enabled && !c.isTrigger && c.gameObject != player.gameObject &&
                          !Physics2D.GetIgnoreLayerCollision(player.gameObject.layer, c.gameObject.layer));

            int sx = Mathf.RoundToInt((inicio.x - minx) / step);
            int sy = Mathf.RoundToInt((inicio.y - miny) / step);
            var q = new Queue<Vector2Int>();
            var seen = new bool[w, h];
            q.Enqueue(new Vector2Int(sx, sy));
            seen[sx, sy] = true;
            bool found = false;

            while (q.Count > 0)
            {
                var a = q.Dequeue();
                var p = Point(a.x, a.y);
                if (Vector2.Distance(p, goal) < .55f && Free(p))
                {
                    found = true;
                    break;
                }
                foreach (var dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                {
                    var n = a + dir;
                    if (n.x < 0 || n.y < 0 || n.x >= w || n.y >= h || seen[n.x, n.y]) continue;
                    seen[n.x, n.y] = true;
                    if (Free(Point(n.x, n.y))) q.Enqueue(n);
                }
            }

            resultados.Add($"{cena}={(found ? "PASS" : "FAIL")}");
        }

        return "QA rotas: " + string.Join(", ", resultados);
    }
}

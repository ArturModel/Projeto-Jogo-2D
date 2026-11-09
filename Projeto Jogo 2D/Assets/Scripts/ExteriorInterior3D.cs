using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal sealed class ExteriorInterior3D : MonoBehaviour
{
    internal enum Local { Posto, Mercadinho, Casa, Jardim, Bar, Sociedade = 6, Salao }
    private readonly List<Transform> sprites = new List<Transform>();
    private readonly List<UnityEngine.Object> recursos = new List<UnityEngine.Object>();
    private Camera vista;
    private Renderer nevoa;

    internal static void Montar(Transform pai, Camera vista, int layer, Vector3 origem, float yaw, Local local)
    {
        var objeto = new GameObject("Exterior 3D — " + local);
        objeto.transform.SetParent(pai, false);
        objeto.transform.localPosition = origem;
        objeto.transform.localRotation = Quaternion.Euler(0, yaw, 0);
        var exterior = objeto.AddComponent<ExteriorInterior3D>();
        exterior.vista = vista;
        vista.GetUniversalAdditionalCameraData().requiresDepthTexture = true;
        exterior.Construir(layer, local);
        exterior.LateUpdate();
    }

    // Each opening uses local horizontal coordinates and keeps the original wall thickness.
    internal static void Parede(float esquerda, float direita, float altura, Rect[] vaos,
        Action<float, float, float, float> trecho)
    {
        float inicio = esquerda;
        foreach (Rect vao in vaos)
        {
            trecho(inicio, vao.xMin, 0, altura);
            if (vao.yMin > 0f) trecho(vao.xMin, vao.xMax, 0, vao.yMin);
            if (vao.yMax < altura) trecho(vao.xMin, vao.xMax, vao.yMax, altura);
            inicio = vao.xMax;
        }
        trecho(inicio, direita, 0, altura);
    }

    private void Construir(int layer, Local local)
    {
        if (local == Local.Salao)
        {
            MontarSalao(layer);
            return;
        }

        bool terra = local == Local.Casa || local == Local.Jardim;
        bool jardim = local == Local.Jardim;
        float inicioPista = local == Local.Posto ? 3.05f : local == Local.Mercadinho ? 2.45f : 2.05f;
        float fimPista = jardim ? 3.7f : inicioPista + (terra ? 4.3f : 6.5f);
        var pavimento = Material("Pavimento molhado", terra ? "Art/Tiles/Terra" : "Art/Tiles/Asfalto",
            terra ? new Color(.62f,.50f,.36f) : new Color(.49f,.52f,.53f), new Vector2(25,5), .95f);
        var calcada = Material("Calçada", "Art/Tiles/Calcada", new Color(.57f,.55f,.48f), new Vector2(22,2), .6f);
        var relva = Material("Margem", "Art/Tiles/Grama", new Color(.34f,.42f,.28f), new Vector2(22,13));
        var pedra = Material("Guia", "Art/Tiles/Pedra", new Color(.46f,.44f,.36f), new Vector2(30,1), .4f);
        Piso(layer, "Pátio e passeio", 0, inicioPista, -.025f, calcada);
        Piso(layer, terra ? "Caminho de barro" : "Asfalto em perspectiva", inicioPista, fimPista, -.16f, pavimento);
        Piso(layer, "Calçada do outro lado", fimPista, fimPista+1.55f, -.035f, calcada);
        Piso(layer, "Chão da mata", fimPista+1.55f, 29f, -.06f, relva);
        foreach (float z in new[] { inicioPista, fimPista })
            Objeto(layer, PrimitiveType.Cube, "Meio-fio", new Vector3(0,-.098f,z), new Vector3(40,.15f,.18f), pedra);

        if (!terra && !jardim)
        {
            var tinta = Material("Pintura gasta", null, new Color(.52f,.43f,.22f), Vector2.one);
            for (int i = -4; i <= 4; i++)
            {
                if (local == Local.Mercadinho && i == 0) continue;
                Objeto(layer, PrimitiveType.Cube, "Faixa do eixo", new Vector3(i*4.2f,-.154f,(inicioPista+fimPista)*.5f),
                    new Vector3(2.35f,.012f,.09f), tinta);
            }
            if (local == Local.Mercadinho)
            {
                var faixa = Material("Faixa de pedestres", "Art/Tiles/Calcada", new Color(.83f,.79f,.64f), new Vector2(1,4));
                for (int i = -3; i <= 3; i++)
                    Objeto(layer, PrimitiveType.Cube, "Travessia da Encruzilhada",
                        new Vector3(i*.51f,-.151f,(inicioPista+fimPista)*.5f), new Vector3(.30f,.01f,5.3f), faixa);
            }
        }

        var rng = new System.Random(98 + (int)local * 37);
        string[] copas = { "Camboata", "Bracatinga", "Pitangueira", "Araucaria" };
        for (int fileira = 0; fileira < 2; fileira++)
        for (int i = -5; i <= 5; i++)
        {
            float x = i*3.55f + (float)rng.NextDouble() * 2f - 1f;
            float z = fimPista + 2.55f + fileira*4.4f + (float)rng.NextDouble()*2.1f;
            Billboard(layer, "Árvore " + local, "Art/RuaNorte/Vegetacao/" + copas[rng.Next(copas.Length)],
                new Vector3(x,-.035f,z), 3.7f + (float)rng.NextDouble()*2.8f, new Color(.84f,.91f,.75f));
            if (fileira == 0)
                Billboard(layer, "Submata", "Art/RuaNorte/Vegetacao/MoitaDeBorda",
                    new Vector3(x+.5f,-.04f,z-1.65f), .85f+(float)rng.NextDouble()*.60f, new Color(.82f,.88f,.69f));
        }

        float zPoste = jardim ? 4.35f : fimPista + .6f;
        float xPoste = local == Local.Mercadinho ? 3.4f : local == Local.Posto ? 4.65f : 4f;
        var a = new Vector3(-xPoste,3.82f,zPoste);
        var b = new Vector3(xPoste+1.35f,3.82f,zPoste+.28f);
        Poste(layer, a, jardim ? 5f : 10.5f);
        Poste(layer, b, jardim ? 4f : 9f);
        var luar = Luz(layer, "Luz fria difusa sobre a rua", new Vector3(0,5.4f,fimPista+1f), 2.1f, 19f);
        luar.color = new Color(.54f,.64f,.75f);
        if (!jardim)
        {
            var reflexo = Luz(layer, "Rebatimento da marquise no asfalto",
                new Vector3(.35f,2.35f,inicioPista-.42f), 1.55f, 6.5f);
            reflexo.color = new Color(1f,.79f,.58f);
        }
        MontarNevoa(layer, a, b, jardim);
        MontarChuva(layer, fimPista+5f);
    }

    private void MontarSalao(int layer)
    {
        var madeira = Material("Assoalho do salão", "Bocha/MadeiraCancha", new Color(.59f,.47f,.33f), new Vector2(9,7));
        var reboco = Material("Parede do salão", "Art/Tiles/Calcada", new Color(.53f,.49f,.36f), new Vector2(6,2));
        Piso(layer, "Salão além da porta", 0, 8, 0, madeira);
        Objeto(layer, PrimitiveType.Cube, "Fundo do salão", new Vector3(0,1.6f,7.6f), new Vector3(14,3.2f,.18f), reboco);
        Objeto(layer, PrimitiveType.Cube, "Banco do salão", new Vector3(1.2f,.44f,5.8f), new Vector3(3.4f,.22f,.60f), madeira);
        Luz(layer, "Luz do salão", new Vector3(0,2.8f,4f), 3.5f, 6.3f);
    }

    private void Poste(int layer, Vector3 lampada, float intensidade)
    {
        Billboard(layer, "Poste em pixel art", "Art/RuaNorte/Poste", new Vector3(lampada.x,-.04f,lampada.z),
            4.0f, new Color(.99f,.86f,.69f));
        var luz = Luz(layer, "Poste âmbar", lampada, intensidade, 10f);
        luz.type = LightType.Spot;
        luz.transform.localRotation = Quaternion.Euler(79f,180f,0);
        luz.spotAngle = 112f;
        luz.innerSpotAngle = 66f;
        var bulbo = Material("Bulbo âmbar", null, new Color(1,.72f,.36f), Vector2.one, unlit: true);
        Objeto(layer, PrimitiveType.Sphere, "Filamento do poste", lampada, new Vector3(.12f,.07f,.12f), bulbo);
        var rebatimento = Luz(layer, "Luz rebatida no pavimento", lampada + new Vector3(0,-1.8f,-2.5f), 1.0f, 6f);
        rebatimento.color = new Color(.70f,.63f,.49f);
    }

    private Light Luz(int layer, string nome, Vector3 pos, float intensidade, float alcance)
    {
        var luz = new GameObject(nome).AddComponent<Light>();
        luz.transform.SetParent(transform, false);
        luz.transform.localPosition = pos;
        luz.type = LightType.Point;
        luz.cullingMask = 1 << layer;
        luz.range = alcance;
        luz.intensity = intensidade;
        luz.color = new Color(1f,.73f,.44f);
        return luz;
    }

    private void MontarNevoa(int layer, Vector3 a, Vector3 b, bool jardim)
    {
        var shader = Resources.Load<Shader>("Art/Sinuca/NevoaExterior3D");
        if (shader == null || !shader.isSupported) return;
        var material = new Material(shader);
        material.SetVector("_PosteA", transform.TransformPoint(a));
        material.SetVector("_PosteB", transform.TransformPoint(b));
        material.SetFloat("_ForcaPostes", jardim ? .45f : 1f);
        recursos.Add(material);
        nevoa = Objeto(layer, PrimitiveType.Cube, "Volume de neblina", new Vector3(0,2.35f,13.6f),
            new Vector3(40,5.3f,26.7f), material).GetComponent<Renderer>();
    }

    private void MontarChuva(int layer, float fundo)
    {
        var shader = Resources.Load<Shader>("Art/Sinuca/ChuvaEntradaBar");
        if (shader == null || !shader.isSupported) return;
        var material = new Material(shader);
        material.SetFloat("_ChuvaContinua", 1f);
        material.SetVector("_Luz", new Vector4(-4f, 2.8f, fundo - 4f, 0f));
        recursos.Add(material);
        var random = new System.Random(324);
        const int total = 540;
        var vertices = new Vector3[total * 4];
        var uv = new Vector2[vertices.Length];
        var gotas = new List<Vector4>(vertices.Length);
        var indices = new int[total * 6];
        for (int i = 0; i < total; i++)
        {
            var pos = new Vector3((float)random.NextDouble()*26f - 13f,
                (float)random.NextDouble(), 1.35f + (float)random.NextDouble()*fundo);
            var gota = new Vector4(.88f + (float)random.NextDouble()*.60f,
                .11f + (float)random.NextDouble()*.15f, .006f, .28f);
            int v = i*4;
            for (int j = 0; j < 4; j++)
            {
                vertices[v+j] = pos;
                uv[v+j] = new Vector2(j%2 == 0 ? -1 : 1, j/2);
                gotas.Add(gota);
            }
            int t = i*6;
            indices[t]=v; indices[t+1]=v+2; indices[t+2]=v+1;
            indices[t+3]=v+1; indices[t+4]=v+2; indices[t+5]=v+3;
        }
        var mesh = new Mesh { name = "Chuva em profundidade", vertices = vertices, uv = uv, triangles = indices };
        mesh.SetUVs(1, gotas);
        mesh.bounds = new Bounds(new Vector3(0,1.8f,fundo*.5f+.65f), new Vector3(30,4.2f,fundo+2f));
        recursos.Add(mesh);
        var objeto = new GameObject("Chuva além da soleira", typeof(MeshFilter), typeof(MeshRenderer));
        objeto.layer = layer;
        objeto.transform.SetParent(transform,false);
        objeto.GetComponent<MeshFilter>().sharedMesh = mesh;
        objeto.GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    private void Piso(int layer, string nome, float perto, float longe, float altura, Material material)
    {
        Objeto(layer, PrimitiveType.Cube, nome, new Vector3(0,altura-.07f,(perto+longe)*.5f),
            new Vector3(40,.14f,longe-perto), material);
    }

    private Material Material(string nome, string textura, Color cor, Vector2 escala, float molhado = 0f, bool unlit = false)
    {
        var material = new Material(Resources.Load<Material>("Art/BarRef7/" + (unlit ? "BarUnlit" : "BarLit"))) { name = nome };
        if (!unlit) material.shader = Resources.Load<Shader>("Art/Sinuca/SalaLit");
        if (textura != null) material.SetTexture("_BaseMap", Resources.Load<Texture2D>(textura));
        material.SetTextureScale("_BaseMap", escala);
        material.SetColor("_BaseColor", cor);
        if (!unlit)
        {
            material.SetFloat("_WetThreshold", molhado);
            material.SetFloat("_Smoothness", molhado > 0f ? .63f : .1f);
        }
        recursos.Add(material);
        return material;
    }

    private void Billboard(int layer, string nome, string caminho, Vector3 pe, float altura, Color cor)
    {
        var imagem = Resources.Load<Texture2D>(caminho);
        if (imagem == null) return;
        var material = Material(nome, caminho, cor, Vector2.one);
        material.SetFloat("_AlphaClip", 1);
        material.SetFloat("_Cull", 0);
        material.SetFloat("_ActorLighting", .7f);
        material.renderQueue = 2450;
        var sprite = Objeto(layer, PrimitiveType.Quad, nome, pe + Vector3.up * (altura*.5f),
            new Vector3(altura * imagem.width / imagem.height, altura, 1), material);
        sprites.Add(sprite.transform);
    }

    private GameObject Objeto(int layer, PrimitiveType tipo, string nome, Vector3 pos, Vector3 escala, Material material)
    {
        var objeto = GameObject.CreatePrimitive(tipo);
        objeto.name = nome;
        objeto.layer = layer;
        objeto.transform.SetParent(transform,false);
        objeto.transform.localPosition = pos;
        objeto.transform.localScale = escala;
        var collider = objeto.GetComponent<Collider>();
        collider.enabled = false;
        Destroy(collider);
        var renderer = objeto.GetComponent<Renderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = ShadowCastingMode.Off;
        return objeto;
    }

    private void LateUpdate()
    {
        if (vista == null) return;
        if (nevoa != null) nevoa.enabled = !vista.orthographic && !MenuPausa.ReduzirEfeitosIntensos;
        foreach (Transform sprite in sprites)
        {
            Vector3 direcao = vista.orthographic ? vista.transform.forward : sprite.position - vista.transform.position;
            direcao.y = 0;
            if (direcao.sqrMagnitude > .0001f) sprite.rotation = Quaternion.LookRotation(direcao, Vector3.up);
        }
    }

    private void OnDestroy()
    {
        foreach (var recurso in recursos) Destroy(recurso);
    }
}

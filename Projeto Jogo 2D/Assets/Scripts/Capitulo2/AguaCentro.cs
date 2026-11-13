using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

internal sealed class AguaCentro : MonoBehaviour
{
    private const int ShorePpu = 16;
    private const float ShoreRange = 1.5f;
    private const int LimiteShaderLuzes = 88;

    private readonly List<Vector3Int> celulas = new();
    private readonly List<Object> recursos = new();
    private readonly List<(Vector3 relPos, Vector4 color, float reflectRadius)> luzesDaAgua = new();
    private readonly Vector4[] lampArray = new Vector4[LimiteShaderLuzes];
    private readonly Vector4[] colorArray = new Vector4[LimiteShaderLuzes];
    private readonly List<(SpriteRenderer sr, Transform tr, float phase)> juncosAnimados = new();
    private readonly List<(SpriteRenderer sr, float phase, float baseScale)> marolasAnimadas = new();

    private Tilemap agua;
    private Transform jogador;
    private Camera camPrincipal;
    private AudioSource fonte;
    private AudioClip clip;
    private Material material;
    private Texture2D shoreMap;
    private Vector2[] bordaAguaCentros = System.Array.Empty<Vector2>();
    private float halfCellX = 0.5f;
    private float halfCellY = 0.5f;
    private Vector2 origemAtual;
    private Vector3 ultimaPosCamara = new(float.NaN, 0f, 0f);
    private float ultimoOrthoSize = -1f;
    private Vector3 ultimaPosAudio = new(float.NaN, 0f, 0f);
    private float ultimaDistAgua = 7f;
    private float ultimoDxAgua;
    private float proximaBusca;
    private bool pausado;

    internal static AguaCentro Instalar(Tilemap original, Transform jogador)
    {
        if (original == null) return null;
        var existente = original.GetComponent<AguaCentro>();
        if (existente != null) return existente;

        var shader = Resources.Load<Shader>("Art/RuaNorte/AguaCorrente");
        if (shader == null || !shader.isSupported)
        {
            Debug.LogWarning("Água do Centro: shader indisponível.");
            return null;
        }

        var componente = original.gameObject.AddComponent<AguaCentro>();
        componente.Iniciar(original, jogador, shader);
        return componente;
    }

    private void Iniciar(Tilemap original, Transform alvo, Shader shader)
    {
        jogador = alvo;

        var obj = new GameObject("Água corrente do Centro", typeof(Tilemap), typeof(TilemapRenderer));
        obj.transform.SetParent(original.transform.parent, false);
        obj.transform.localPosition = original.transform.localPosition;
        obj.transform.localRotation = original.transform.localRotation;
        obj.transform.localScale = original.transform.localScale;

        agua = obj.GetComponent<Tilemap>();
        agua.tileAnchor = original.tileAnchor;
        agua.color = original.color;

        var renderer = obj.GetComponent<TilemapRenderer>();
        var anterior = original.GetComponent<TilemapRenderer>();
        renderer.sortingLayerID = anterior.sortingLayerID;
        int baseOrder = anterior.sortingOrder;
        renderer.sortingOrder = baseOrder + 2; // Acima de Pavimentos (-3000) para desenhar a margem orgânica
        anterior.sortingOrder = baseOrder + 4; // Mantém a ponte de Madeira (-2996) acima da água e da margem

        material = new Material(shader) { name = "Água corrente do Centro" };
        renderer.sharedMaterial = material;

        var pavimentos = EncontrarPavimentos(original);
        var aguaSet = new HashSet<Vector3Int>();
        var madeiraSet = new HashSet<Vector3Int>();
        TileBase aguaTile = null;
        var aguaMatrix = Matrix4x4.identity;
        var aguaColor = Color.white;

        foreach (var p in original.cellBounds.allPositionsWithin)
        {
            var tile = original.GetTile(p);
            if (tile == null) continue;
            if (tile.name == "Agua")
            {
                if (aguaTile == null)
                {
                    aguaTile = tile;
                    aguaMatrix = original.GetTransformMatrix(p);
                    aguaColor = original.GetColor(p);
                }
                agua.SetTile(p, tile);
                agua.SetTileFlags(p, TileFlags.None);
                agua.SetTransformMatrix(p, original.GetTransformMatrix(p));
                agua.SetColor(p, original.GetColor(p));
                celulas.Add(p);
                aguaSet.Add(p);
            }
            else if (tile.name == "Madeira")
            {
                madeiraSet.Add(p);
            }
        }

        foreach (var p in celulas) original.SetTile(p, null);

        // Preenche 1 anel visual nos tiles de terra adjacentes à água para permitir a transição curva sem quinas
        if (aguaTile != null && pavimentos != null)
        {
            var padding = new HashSet<Vector3Int>();
            foreach (var c in aguaSet)
            {
                for (int dy = -1; dy <= 1; dy++)
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;
                    var n = new Vector3Int(c.x + dx, c.y + dy, c.z);
                    if (aguaSet.Contains(n) || madeiraSet.Contains(n)) continue;
                    if (pavimentos.GetTile(n) != null) padding.Add(n);
                }
            }

            foreach (var n in padding)
            {
                agua.SetTile(n, aguaTile);
                agua.SetTileFlags(n, TileFlags.None);
                agua.SetTransformMatrix(n, aguaMatrix);
                agua.SetColor(n, aguaColor);
            }
        }

        ConstruirMapaDaMargem(original, pavimentos, aguaSet, madeiraSet);
        CriarDetalhesDaAgua(pavimentos, aguaSet, madeiraSet, anterior.sortingLayerID, baseOrder);

        // Pré-calcula apenas as células de borda da água para o áudio por proximidade sem iterar 807 células por frame
        var tamanhoCelula = agua.layoutGrid.cellSize;
        halfCellX = tamanhoCelula.x * 0.5f;
        halfCellY = tamanhoCelula.y * 0.5f;
        var bordas = new List<Vector2>(celulas.Count / 4);
        foreach (var c in celulas)
        {
            if (!aguaSet.Contains(c + Vector3Int.up) || !aguaSet.Contains(c + Vector3Int.down) ||
                !aguaSet.Contains(c + Vector3Int.left) || !aguaSet.Contains(c + Vector3Int.right))
            {
                Vector3 centro = agua.GetCellCenterWorld(c);
                bordas.Add(new Vector2(centro.x, centro.y));
            }
        }
        bordaAguaCentros = bordas.ToArray();

        fonte = gameObject.AddComponent<AudioSource>();
        clip = SantaRitaBarAudio.CriarTextura(1);
        fonte.clip = clip;
        fonte.loop = true;
        fonte.playOnAwake = false;
        fonte.volume = 0;

        Debug.Log($"Água do Centro: {celulas.Count} tiles unidos pela corrente com margem orgânica.", this);
    }

    internal void SincronizarLuzesDoCentro(IReadOnlyList<(Vector3 pos, float raio, Color cor)> luzes, Vector2 origem, Material piso)
    {
        if (material == null) return;

        origemAtual = origem;
        material.SetVector("_Origin", new Vector4(origem.x, origem.y, 0f, 0f));
        if (piso != null)
        {
            material.SetFloat("_TownGroundMist", piso.GetFloat("_TownGroundMist"));
            material.SetFloat("_WindStrength", piso.GetFloat("_WindStrength"));
        }

        luzesDaAgua.Clear();
        if (luzes != null)
        {
            // Filtra apenas luminárias cujo banho ou coluna de reflexo pode alcançar alguma borda do rio
            for (int i = 0; i < luzes.Count; i++)
            {
                var (pos, raio, cor) = luzes[i];
                float reflectR = Mathf.Max(raio * 1.55f, 13.5f);
                if (!LuminariaAlcancaraRio(pos, reflectR)) continue;

                luzesDaAgua.Add((
                    new Vector3(pos.x - origem.x, pos.y - origem.y, pos.z),
                    new Vector4(cor.r, cor.g, cor.b, raio),
                    reflectR));
            }
        }

        AtualizarLuzesVisiveisNaAgua(true);
    }

    private bool LuminariaAlcancaraRio(Vector3 pos, float reflectR)
    {
        if (bordaAguaCentros.Length == 0) return true;
        float reflectR2 = reflectR * reflectR;
        for (int i = 0; i < bordaAguaCentros.Length; i++)
        {
            Vector2 c = bordaAguaCentros[i];
            float dx = pos.x - c.x;
            float dy = pos.y - c.y;
            if (dx * dx + dy * dy <= reflectR2) return true;
            if (Mathf.Abs(dx) <= 1.2f && dy >= -0.5f && dy <= 11.0f) return true;
        }
        return false;
    }

    private void AtualizarLuzesVisiveisNaAgua(bool forcar)
    {
        if (material == null) return;
        if (camPrincipal == null || !camPrincipal.isActiveAndEnabled)
            camPrincipal = Camera.main;

        if (camPrincipal == null)
        {
            if (!forcar) return;
            int n = Mathf.Min(luzesDaAgua.Count, LimiteShaderLuzes);
            for (int i = 0; i < n; i++)
            {
                var item = luzesDaAgua[i];
                lampArray[i] = new Vector4(item.relPos.x, item.relPos.y, item.relPos.z, item.color.w);
                colorArray[i] = new Vector4(item.color.x, item.color.y, item.color.z, 1f);
            }
            for (int i = n; i < LimiteShaderLuzes; i++) { lampArray[i] = Vector4.zero; colorArray[i] = Vector4.zero; }
            material.SetInteger("_TownLampCount", n);
            material.SetVectorArray("_TownLamps", lampArray);
            material.SetVectorArray("_TownLampColors", colorArray);
            return;
        }

        Vector3 camPos = camPrincipal.transform.position;
        float ortho = camPrincipal.orthographic ? camPrincipal.orthographicSize : 8f;
        if (!forcar && (camPos - ultimaPosCamara).sqrMagnitude < 0.25f && Mathf.Abs(ortho - ultimoOrthoSize) < 0.05f)
            return;

        ultimaPosCamara = camPos;
        ultimoOrthoSize = ortho;

        float camRelX = camPos.x - origemAtual.x;
        float camRelY = camPos.y - origemAtual.y;
        float padY = ortho + 2.5f;
        float padX = ortho * camPrincipal.aspect + 2.5f;
        float minX = camRelX - padX;
        float maxX = camRelX + padX;
        float minY = camRelY - padY;
        float maxY = camRelY + padY;

        int count = 0;
        for (int i = 0; i < luzesDaAgua.Count && count < LimiteShaderLuzes; i++)
        {
            var (relPos, col, reflectR) = luzesDaAgua[i];
            if (relPos.x + reflectR < minX || relPos.x - reflectR > maxX) continue;
            if (relPos.y + reflectR < minY) continue;
            if (Mathf.Min(relPos.y - reflectR, relPos.y - 11.0f) > maxY) continue;

            lampArray[count] = new Vector4(relPos.x, relPos.y, relPos.z, col.w);
            colorArray[count] = new Vector4(col.x, col.y, col.z, 1f);
            count++;
        }

        for (int i = count; i < LimiteShaderLuzes; i++)
        {
            lampArray[i] = Vector4.zero;
            colorArray[i] = Vector4.zero;
        }

        material.SetInteger("_TownLampCount", count);
        material.SetVectorArray("_TownLamps", lampArray);
        material.SetVectorArray("_TownLampColors", colorArray);
    }

    internal void AtualizarLuzDinamica(Material piso, bool efeitosAtivos,
        Vector4 viewFocus, Vector4 playerLamp, Vector2 playerLampDir)
    {
        if (material == null || piso == null) return;
        material.SetVector("_PlayerLamp", playerLamp);
        material.SetVector("_PlayerLampDirection", playerLampDir);
        material.SetVector("_ViewFocus", viewFocus);
        material.SetFloat("_WindStrength", efeitosAtivos ? 1f : 0f);
        material.SetFloat("_TownGroundMist", efeitosAtivos ? piso.GetFloat("_TownGroundMist") : 0f);
    }

    private Tilemap EncontrarPavimentos(Tilemap original)
    {
        if (original.transform.parent != null)
        {
            var irmao = original.transform.parent.Find("Pavimentos do Centro");
            if (irmao != null && irmao.TryGetComponent(out Tilemap map)) return map;
        }
        foreach (var map in FindObjectsByType<Tilemap>())
            if (map.gameObject.scene == gameObject.scene && map.name == "Pavimentos do Centro")
                return map;
        return null;
    }

    private void ConstruirMapaDaMargem(Tilemap original, Tilemap pavimentos,
        HashSet<Vector3Int> aguaSet, HashSet<Vector3Int> madeiraSet)
    {
        if (celulas.Count == 0) return;

        int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
        foreach (var c in aguaSet)
        {
            if (c.x < minX) minX = c.x;
            if (c.y < minY) minY = c.y;
            if (c.x > maxX) maxX = c.x;
            if (c.y > maxY) maxY = c.y;
        }
        minX -= 2; minY -= 2; maxX += 2; maxY += 2;

        int cellsW = maxX - minX + 1;
        int cellsH = maxY - minY + 1;
        int w = cellsW * ShorePpu;
        int h = cellsH * ShorePpu;
        int total = w * h;

        // Classifica cada célula do retângulo: 0 = vazio, 1 = água, 2 = terra, 3 = ponte
        var tipo = new byte[cellsW * cellsH];
        for (int cy = minY; cy <= maxY; cy++)
        for (int cx = minX; cx <= maxX; cx++)
        {
            var c = new Vector3Int(cx, cy, 0);
            int idx = (cy - minY) * cellsW + (cx - minX);
            if (aguaSet.Contains(c)) tipo[idx] = 1;
            else if (madeiraSet.Contains(c)) tipo[idx] = 3;
            else if (pavimentos != null && pavimentos.GetTile(c) != null) tipo[idx] = 2;
        }

        // Calcula distância euclidiana em texels até Terra, até Água e até Ponte via 8SSEDT
        var dxLand = new short[total];
        var dyLand = new short[total];
        var dxWater = new short[total];
        var dyWater = new short[total];
        var dxBridge = new short[total];
        var dyBridge = new short[total];

        const short Inf = 16000;
        for (int y = 0; y < h; y++)
        {
            int cy = y / ShorePpu;
            for (int x = 0; x < w; x++)
            {
                int cx = x / ShorePpu;
                byte t = tipo[cy * cellsW + cx];
                int i = y * w + x;
                if (t == 2) { dxLand[i] = 0; dyLand[i] = 0; } else { dxLand[i] = Inf; dyLand[i] = Inf; }
                if (t == 1) { dxWater[i] = 0; dyWater[i] = 0; } else { dxWater[i] = Inf; dyWater[i] = Inf; }
                if (t == 3) { dxBridge[i] = 0; dyBridge[i] = 0; } else { dxBridge[i] = Inf; dyBridge[i] = Inf; }
            }
        }

        PropagarEdt(dxLand, dyLand, w, h);
        PropagarEdt(dxWater, dyWater, w, h);
        PropagarEdt(dxBridge, dyBridge, w, h);

        var sdArray = new float[total];
        var bridgeArray = new float[total];
        for (int i = 0; i < total; i++)
        {
            int cx = (i % w) / ShorePpu;
            int cy = (i / w) / ShorePpu;
            byte t = tipo[cy * cellsW + cx];
            float dLand = Mathf.Sqrt(dxLand[i] * dxLand[i] + dyLand[i] * dyLand[i]);
            float dWater = Mathf.Sqrt(dxWater[i] * dxWater[i] + dyWater[i] * dyWater[i]);
            float dBridge = Mathf.Sqrt(dxBridge[i] * dxBridge[i] + dyBridge[i] * dyBridge[i]);

            sdArray[i] = ((t == 2) ? dWater : -dLand) / ShorePpu;
            bridgeArray[i] = dBridge / ShorePpu;
        }

        // Suaviza o campo de distância em O(w*h) por soma acumulada (preservando as bordas da ponte)
        var sdBlurH = new float[total];
        var sdBlur = new float[total];
        const int blurRadius = 6;
        var prefix = new double[Mathf.Max(w, h) + 1];

        for (int y = 0; y < h; y++)
        {
            int row = y * w;
            prefix[0] = 0.0;
            for (int x = 0; x < w; x++) prefix[x + 1] = prefix[x] + sdArray[row + x];
            for (int x = 0; x < w; x++)
            {
                int x0 = x > blurRadius ? x - blurRadius : 0;
                int x1 = x + blurRadius < w ? x + blurRadius : w - 1;
                sdBlurH[row + x] = (float)((prefix[x1 + 1] - prefix[x0]) / (x1 - x0 + 1));
            }
        }
        for (int x = 0; x < w; x++)
        {
            prefix[0] = 0.0;
            for (int y = 0; y < h; y++) prefix[y + 1] = prefix[y] + sdBlurH[y * w + x];
            for (int y = 0; y < h; y++)
            {
                int y0 = y > blurRadius ? y - blurRadius : 0;
                int y1 = y + blurRadius < h ? y + blurRadius : h - 1;
                sdBlur[y * w + x] = (float)((prefix[y1 + 1] - prefix[y0]) / (y1 - y0 + 1));
            }
        }

        var pixels = new Color32[total];
        for (int i = 0; i < total; i++)
        {
            int cx = (i % w) / ShorePpu;
            int cy = (i / w) / ShorePpu;
            bool temPavimento = tipo[cy * cellsW + cx] == 2;

            float bridgeUnits = bridgeArray[i];
            float keepBlur = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.15f, 0.55f, bridgeUnits));
            float sdUnits = Mathf.Lerp(sdArray[i], sdBlur[i], keepBlur);

            byte r = (byte)Mathf.Clamp(Mathf.RoundToInt((0.5f + sdUnits / ShoreRange) * 255f), 0, 255);
            byte g = (byte)Mathf.Clamp(Mathf.RoundToInt((bridgeUnits / ShoreRange) * 255f), 0, 255);
            byte b = temPavimento ? (byte)255 : (byte)0;
            pixels[i] = new Color32(r, g, b, 255);
        }

        shoreMap = new Texture2D(w, h, TextureFormat.RGBA32, false, true)
        {
            name = "Mapa da margem do rio",
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        shoreMap.SetPixels32(pixels);
        shoreMap.Apply(false, true);
        recursos.Add(shoreMap);

        Vector3 origemMundo = original.CellToWorld(new Vector3Int(minX, minY, 0));
        Vector3 tamanhoCelula = original.layoutGrid.cellSize;
        material.SetTexture("_ShoreMap", shoreMap);
        material.SetVector("_ShoreRect", new Vector4(
            origemMundo.x,
            origemMundo.y,
            cellsW * tamanhoCelula.x,
            cellsH * tamanhoCelula.y));
    }

    private static void PropagarEdt(short[] dx, short[] dy, int w, int h)
    {
        static void Relax(short[] gx, short[] gy, int idx, int nIdx, int ox, int oy)
        {
            int nx = gx[nIdx] + ox;
            int ny = gy[nIdx] + oy;
            int cx = gx[idx];
            int cy = gy[idx];
            if (nx * nx + ny * ny < cx * cx + cy * cy)
            {
                gx[idx] = (short)nx;
                gy[idx] = (short)ny;
            }
        }

        for (int y = 0; y < h; y++)
        {
            int row = y * w;
            for (int x = 0; x < w; x++)
            {
                int i = row + x;
                if (x > 0) Relax(dx, dy, i, i - 1, -1, 0);
                if (y > 0)
                {
                    Relax(dx, dy, i, i - w, 0, -1);
                    if (x > 0) Relax(dx, dy, i, i - w - 1, -1, -1);
                    if (x < w - 1) Relax(dx, dy, i, i - w + 1, 1, -1);
                }
            }
            for (int x = w - 2; x >= 0; x--)
                Relax(dx, dy, row + x, row + x + 1, 1, 0);
        }

        for (int y = h - 1; y >= 0; y--)
        {
            int row = y * w;
            for (int x = w - 1; x >= 0; x--)
            {
                int i = row + x;
                if (x < w - 1) Relax(dx, dy, i, i + 1, 1, 0);
                if (y < h - 1)
                {
                    Relax(dx, dy, i, i + w, 0, 1);
                    if (x > 0) Relax(dx, dy, i, i + w - 1, -1, 1);
                    if (x < w - 1) Relax(dx, dy, i, i + w + 1, 1, 1);
                }
            }
            for (int x = 1; x < w; x++)
                Relax(dx, dy, row + x, row + x - 1, -1, 0);
        }
    }

    private void CriarDetalhesDaAgua(Tilemap pavimentos, HashSet<Vector3Int> aguaSet,
        HashSet<Vector3Int> madeiraSet, int sortingLayerID, int baseOrder)
    {
        var grupo = new GameObject("Detalhes da água do Centro");
        grupo.transform.SetParent(agua.transform, false);
        recursos.Add(grupo);

        var worldLit = Resources.Load<Material>("Art/WorldLit");
        var spriteCapim = Resources.Load<Sprite>("Art/RuaNorte/Entrada/Modulos/Capim");
        var spritePedras = Resources.Load<Sprite>("Art/RuaNorte/Entrada/Modulos/Pedras");

        var atlas = CriarAtlasDetalhesAgua(
            out var pedraRioA, out var pedraRioB,
            out var marolaBase,
            out var juncoA, out var juncoB,
            out var aguapeA, out var aguapeB);
        if (atlas != null) recursos.Add(atlas);

        int HashCell(int x, int y, int salt)
        {
            unchecked
            {
                int h = x * 374761393 + y * 668265263 + salt * 1442695041;
                h = (h ^ (h >> 13)) * 1274126177;
                return (h ^ (h >> 16)) & 0x7FFFFFFF;
            }
        }

        bool PertoDePonte(Vector3Int c)
        {
            if (c.x >= 42 && c.x <= 49 && c.y <= 1) return true;
            for (int dy = -1; dy <= 1; dy++)
            for (int dx = -1; dx <= 1; dx++)
                if (madeiraSet.Contains(new Vector3Int(c.x + dx, c.y + dy, c.z))) return true;
            return false;
        }

        SpriteRenderer Spawn(string nome, Sprite sp, Vector3 pos, int order, float scale, bool flip, Color cor)
        {
            if (sp == null) return null;
            var go = new GameObject(nome, typeof(SpriteRenderer));
            go.transform.SetParent(grupo.transform, false);
            go.transform.position = pos;
            go.transform.localScale = new Vector3(scale, scale, 1f);
            var sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = sp;
            sr.flipX = flip;
            sr.color = cor;
            sr.sortingLayerID = sortingLayerID;
            sr.sortingOrder = order;
            if (worldLit != null) sr.sharedMaterial = worldLit;
            return sr;
        }

        // 1. Pequenos detalhes ao longo da margem e dentro da água rasa
        foreach (var c in aguaSet)
        {
            if (PertoDePonte(c)) continue;
            bool terraAcima = pavimentos != null && pavimentos.GetTile(c + Vector3Int.up) != null && !aguaSet.Contains(c + Vector3Int.up);
            bool terraDireita = pavimentos != null && pavimentos.GetTile(c + Vector3Int.right) != null && !aguaSet.Contains(c + Vector3Int.right);
            int rnd = HashCell(c.x, c.y, 17);
            float jitterX = ((rnd % 100) / 100f - 0.5f) * 0.45f;
            float jitterY = (((rnd / 100) % 100) / 100f - 0.5f) * 0.25f;
            Vector3 centro = agua.GetCellCenterWorld(c);

            if (terraAcima || terraDireita)
            {
                // Beira de barro + pedras e capim de margem em pontos espaçados
                float bankX = centro.x + jitterX;
                float bankY = terraAcima ? centro.y + 0.42f + jitterY * 0.25f : centro.y + jitterY;
                if (terraDireita && !terraAcima) bankX = centro.x + 0.42f;

                int pick = rnd % 10;
                if (pick is 0 or 1 or 2 && spritePedras != null)
                {
                    float s = (0.54f + (rnd % 20) * 0.01f) / Mathf.Max(0.1f, spritePedras.bounds.size.x);
                    Spawn("Pedras da margem do rio", spritePedras, new Vector3(bankX, bankY - 0.04f, 0f),
                        baseOrder + 5, s, (rnd & 2) == 2, new Color(0.76f, 0.80f, 0.84f, 0.90f));
                }
                else if (pick is 3 or 4 or 5 && spriteCapim != null)
                {
                    float s = (0.62f + (rnd % 18) * 0.012f) / Mathf.Max(0.1f, spriteCapim.bounds.size.x);
                    int order = 1000 - Mathf.RoundToInt((bankY + 0.08f) * 10);
                    Spawn("Capim da margem do rio", spriteCapim, new Vector3(bankX, bankY + 0.04f, 0f),
                        order, s, (rnd & 4) == 4, new Color(0.68f, 0.78f, 0.72f, 0.94f));
                }
                else if (pick is 6 or 7)
                {
                    var sp = (rnd & 8) == 0 ? juncoA : juncoB;
                    float yPos = bankY - 0.16f;
                    var sr = Spawn("Junco da margem", sp, new Vector3(bankX, yPos, 0f),
                        1000 - Mathf.RoundToInt(yPos * 10), 0.92f, (rnd & 1) == 1, new Color(0.86f, 0.90f, 0.86f, 0.95f));
                    if (sr != null) juncosAnimados.Add((sr, sr.transform, (rnd % 628) * 0.01f));
                }
                continue;
            }

            // Água aberta próxima à margem (célula 1 ou 2 unidades dentro do rio)
            bool pertoDaMargem = aguaSet.Contains(c + Vector3Int.up) &&
                                 (pavimentos != null && pavimentos.GetTile(c + Vector3Int.up * 2) != null);
            bool aguaAoRedor = aguaSet.Contains(c + Vector3Int.left) && aguaSet.Contains(c + Vector3Int.right) &&
                               aguaSet.Contains(c + Vector3Int.down);
            if (!aguaAoRedor) continue;

            if (pertoDaMargem && rnd % 9 == 0)
            {
                // Pedrinhas emergindo da água com pequena marola animada
                Vector3 p = new(centro.x + jitterX, centro.y + jitterY, 0f);
                var marola = Spawn("Marola ao redor de pedra", marolaBase, p + new Vector3(0f, -0.04f, 0f),
                    baseOrder + 3, 0.95f, false, new Color(0.55f, 0.72f, 0.92f, 0.42f));
                if (marola != null) marolasAnimadas.Add((marola, (rnd % 628) * 0.01f, 0.95f));

                Spawn("Pedra no rio", (rnd & 1) == 0 ? pedraRioA : pedraRioB, p,
                    baseOrder + 5, 0.92f, (rnd & 2) == 2, new Color(0.82f, 0.86f, 0.92f, 0.96f));
            }
            else if (pertoDaMargem && rnd % 13 == 3)
            {
                // Pequenos aguapés / folhas flutuantes em remansos perto da margem
                Vector3 p = new(centro.x + jitterX, centro.y + jitterY, 0f);
                Spawn("Aguapé na água", (rnd & 1) == 0 ? aguapeA : aguapeB, p,
                    baseOrder + 5, 0.88f, (rnd & 4) == 4, new Color(0.75f, 0.84f, 0.78f, 0.92f));
            }
            else if (!pertoDaMargem && c.y >= -6 && rnd % 23 == 5)
            {
                // Raras pedras no meio do leito visível com marola suave
                Vector3 p = new(centro.x + jitterX, centro.y + jitterY, 0f);
                var marola = Spawn("Marola no leito do rio", marolaBase, p + new Vector3(0f, -0.04f, 0f),
                    baseOrder + 3, 1.05f, false, new Color(0.50f, 0.68f, 0.90f, 0.36f));
                if (marola != null) marolasAnimadas.Add((marola, (rnd % 628) * 0.01f, 1.05f));

                Spawn("Pedra submersa no rio", (rnd & 1) == 0 ? pedraRioA : pedraRioB, p,
                    baseOrder + 5, 0.86f, (rnd & 2) == 2, new Color(0.74f, 0.80f, 0.88f, 0.92f));
            }
        }

        // 2. Partículas sutis à deriva na correnteza (pequenas folhas e brilhos d'água)
        CriarParticulasCorrenteza(grupo.transform, sortingLayerID, baseOrder + 3);
    }

    private void CriarParticulasCorrenteza(Transform parent, int sortingLayerID, int order)
    {
        var go = new GameObject("Partículas da correnteza do rio", typeof(ParticleSystem));
        go.transform.SetParent(parent, false);
        go.transform.position = new Vector3(36f, -4.5f, 0f);

        var ps = go.GetComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main;
        main.duration = 8f;
        main.loop = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(4.5f, 7.5f);
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.11f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(0.18f, 0.28f, 0.42f, 0.48f),
            new Color(0.24f, 0.30f, 0.22f, 0.42f));
        main.maxParticles = 45;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 6.5f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(95f, 6.2f, 0.1f);

        var vel = ps.velocityOverLifetime;
        vel.enabled = true;
        vel.space = ParticleSystemSimulationSpace.World;
        vel.x = new ParticleSystem.MinMaxCurve(0.24f, 0.40f);
        vel.y = new ParticleSystem.MinMaxCurve(-0.04f, 0.04f);
        vel.z = new ParticleSystem.MinMaxCurve(0f, 0f);

        var colLife = ps.colorOverLifetime;
        colLife.enabled = true;
        var grad = new Gradient();
        grad.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.75f, 0.25f), new GradientAlphaKey(0.75f, 0.75f), new GradientAlphaKey(0f, 1f) });
        colLife.color = new ParticleSystem.MinMaxGradient(grad);

        var psr = go.GetComponent<ParticleSystemRenderer>();
        psr.sortingLayerID = sortingLayerID;
        psr.sortingOrder = order;
        var shader = Shader.Find("Sprites/Default");
        if (shader != null)
        {
            var mat = new Material(shader);
            recursos.Add(mat);
            psr.sharedMaterial = mat;
        }

        ps.Play();
    }

    private Texture2D CriarAtlasDetalhesAgua(
        out Sprite pedraRioA, out Sprite pedraRioB,
        out Sprite marolaBase,
        out Sprite juncoA, out Sprite juncoB,
        out Sprite aguapeA, out Sprite aguapeB)
    {
        const int Aw = 64, Ah = 64;
        var tex = new Texture2D(Aw, Ah, TextureFormat.RGBA32, false)
        {
            name = "AtlasDetalhesAguaCentro",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        var atlasPx = new Color32[Aw * Ah];

        Sprite Embutir(string nome, int ox, int oy, int w, int h, Color32[] src, Vector2 pivot)
        {
            for (int y = 0; y < h; y++)
            {
                int dstRow = (oy + y) * Aw + ox;
                int srcRow = y * w;
                for (int x = 0; x < w; x++) atlasPx[dstRow + x] = src[srcRow + x];
            }
            var sp = Sprite.Create(tex, new Rect(ox, oy, w, h), pivot, 32f, 0, SpriteMeshType.FullRect);
            sp.name = nome;
            recursos.Add(sp);
            return sp;
        }

        Color32[] GerarPedraRio(int variante)
        {
            const int w = 20, h = 14;
            var px = new Color32[w * h];
            Color32 contorno = new(24, 30, 38, 255);
            Color32 baseEscura = new(44, 52, 62, 255);
            Color32 meio = new(66, 76, 86, 255);
            Color32 brilho = new(96, 108, 120, 255);
            Color32 musgo = new(54, 72, 54, 255);

            void DesenharPedra(float cx, float cy, float rx, float ry)
            {
                for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    float dx = (x - cx) / rx;
                    float dy = (y - cy) / ry;
                    float d = dx * dx + dy * dy;
                    if (d > 1.0f) continue;
                    int i = y * w + x;
                    if (d > 0.72f) px[i] = contorno;
                    else if (dy < -0.15f) px[i] = baseEscura;
                    else if (dy > 0.25f && dx < 0.15f) px[i] = brilho;
                    else px[i] = ((x + y) % 5 == 0) ? musgo : meio;
                }
            }

            if (variante == 0) { DesenharPedra(8f, 6f, 5.2f, 3.8f); DesenharPedra(13.5f, 4.8f, 3.2f, 2.5f); }
            else { DesenharPedra(10.5f, 6.2f, 4.6f, 3.5f); DesenharPedra(5.5f, 4.5f, 2.8f, 2.2f); }
            return px;
        }

        Color32[] GerarMarolaAnel()
        {
            const int w = 26, h = 12;
            var px = new Color32[w * h];
            Color32 anel = new(170, 200, 230, 145);
            for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float dx = (x - (w - 1) * 0.5f) / (w * 0.45f);
                float dy = (y - (h - 1) * 0.5f) / (h * 0.42f);
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                if (d >= 0.76f && d <= 0.96f && (x + y) % 3 != 0) px[y * w + x] = anel;
            }
            return px;
        }

        Color32[] GerarJunco(int variante)
        {
            const int w = 18, h = 24;
            var px = new Color32[w * h];
            Color32 hasteEscura = new(44, 62, 42, 255);
            Color32 hasteClara = new(68, 92, 58, 255);
            Color32 taboa = new(78, 54, 38, 255);
            Color32 taboaLuz = new(102, 74, 52, 255);

            void Haste(int x0, int altura, int inclina, bool comTaboa)
            {
                for (int y = 1; y <= altura && y < h; y++)
                {
                    int x = Mathf.Clamp(x0 + Mathf.RoundToInt(inclina * (y / (float)altura)), 0, w - 1);
                    px[y * w + x] = (y > altura * 0.55f) ? hasteClara : hasteEscura;
                    if (comTaboa && y >= altura - 5 && y <= altura - 2)
                    {
                        px[y * w + x] = taboa;
                        if (x + 1 < w) px[y * w + x + 1] = taboaLuz;
                    }
                }
            }

            if (variante == 0) { Haste(5, 16, -2, false); Haste(8, 21, 0, true); Haste(11, 18, 2, false); Haste(13, 13, 3, false); }
            else { Haste(4, 14, -2, false); Haste(7, 19, -1, true); Haste(10, 22, 1, true); Haste(13, 15, 2, false); }
            return px;
        }

        Color32[] GerarAguape(int variante)
        {
            const int w = 16, h = 12;
            var px = new Color32[w * h];
            Color32 borda = new(34, 54, 38, 255);
            Color32 folha = new(54, 82, 54, 255);
            Color32 folhaLuz = new(76, 108, 72, 255);

            void Disco(float cx, float cy, float rx, float ry)
            {
                for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    float dx = (x - cx) / rx;
                    float dy = (y - cy) / ry;
                    if (dx > 0.2f && Mathf.Abs(dy) < 0.25f) continue;
                    float d = dx * dx + dy * dy;
                    if (d > 1.0f) continue;
                    px[y * w + x] = d > 0.72f ? borda : (dy > 0.1f ? folhaLuz : folha);
                }
            }

            Disco(6.5f, 6.2f, 4.2f, 3.0f);
            if (variante == 0) Disco(11.5f, 4.2f, 2.5f, 1.9f);
            else Disco(11.0f, 7.5f, 2.7f, 2.0f);
            return px;
        }

        pedraRioA = Embutir("PedraRio_0", 1, 1, 20, 14, GerarPedraRio(0), new Vector2(0.5f, 0.35f));
        pedraRioB = Embutir("PedraRio_1", 22, 1, 20, 14, GerarPedraRio(1), new Vector2(0.5f, 0.35f));
        aguapeA = Embutir("AguapeRio_0", 43, 1, 16, 12, GerarAguape(0), new Vector2(0.5f, 0.5f));
        marolaBase = Embutir("MarolaPedra", 1, 17, 26, 12, GerarMarolaAnel(), new Vector2(0.5f, 0.5f));
        aguapeB = Embutir("AguapeRio_1", 29, 17, 16, 12, GerarAguape(1), new Vector2(0.5f, 0.5f));
        juncoA = Embutir("JuncoRio_0", 1, 31, 18, 24, GerarJunco(0), new Vector2(0.5f, 0.1f));
        juncoB = Embutir("JuncoRio_1", 21, 31, 18, 24, GerarJunco(1), new Vector2(0.5f, 0.1f));

        tex.SetPixels32(atlasPx);
        tex.Apply(false, true);
        return tex;
    }

    private void Update()
    {
        bool pausa = Time.timeScale == 0 || MenuPausa.Aberto || !Application.isFocused;
        if (pausa != pausado)
        {
            if (fonte != null)
            {
                if (pausa) fonte.Pause();
                else fonte.UnPause();
            }
            pausado = pausa;
        }
        if (pausa) return;

        AtualizarLuzesVisiveisNaAgua(false);

        float t = Time.time;
        for (int i = 0; i < juncosAnimados.Count; i++)
        {
            var (sr, tr, phase) = juncosAnimados[i];
            if (sr == null || !sr.isVisible) continue;
            float sway = Mathf.Sin(t * 1.45f + phase) * 3.2f + Mathf.Sin(t * 2.6f + phase * 1.3f) * 1.1f;
            tr.localRotation = Quaternion.Euler(0f, 0f, sway);
        }

        for (int i = 0; i < marolasAnimadas.Count; i++)
        {
            var (sr, phase, baseScale) = marolasAnimadas[i];
            if (sr == null || !sr.isVisible) continue;
            float pulso = Mathf.Repeat(t * 0.52f + phase, 1f);
            float s = baseScale * (0.86f + pulso * 0.34f);
            sr.transform.localScale = new Vector3(s, s, 1f);
            var c = sr.color;
            c.a = (1f - pulso) * 0.44f;
            sr.color = c;
        }

        if (fonte == null) return;
        if (jogador == null && Time.unscaledTime >= proximaBusca)
        {
            proximaBusca = Time.unscaledTime + 2f;
            var player = FindAnyObjectByType<PlayerController>();
            if (player != null && player.gameObject.scene == gameObject.scene) jogador = player.transform;
        }

        float distancia = 7f, dx = 0f;
        if (jogador != null && bordaAguaCentros.Length > 0)
        {
            Vector3 pos = jogador.position;
            if ((pos - ultimaPosAudio).sqrMagnitude >= 0.0025f)
            {
                ultimaPosAudio = pos;
                float minD2 = 49f;
                float bestDx = 0f;
                for (int i = 0; i < bordaAguaCentros.Length; i++)
                {
                    Vector2 c = bordaAguaCentros[i];
                    float x = Mathf.Max(0f, Mathf.Abs(c.x - pos.x) - halfCellX);
                    float y = Mathf.Max(0f, Mathf.Abs(c.y - pos.y) - halfCellY);
                    float d2 = x * x + y * y;
                    if (d2 < minD2) { minD2 = d2; bestDx = c.x - pos.x; }
                }
                distancia = Mathf.Sqrt(minD2);
                ultimaDistAgua = distancia;
                ultimoDxAgua = bestDx;
                dx = bestDx;
            }
            else
            {
                distancia = ultimaDistAgua;
                dx = ultimoDxAgua;
            }
        }

        float sairCena = SceneFader.AtenuacaoAudioSaidaCena;
        float volume = .23f * (1f - Mathf.SmoothStep(0, 1, distancia / 7f)) * AudioSettingsController.VolumeEfeitos * (1f - sairCena);
        if (SinucaMinigameUI.IsShowing || BochaMinigameUI.IsShowing) volume = 0f;
        if (GameMessageUI.IsShowing) volume *= .4f;
        float vel = sairCena > .001f ? .8f : (SceneFader.EmTransicaoAudioPortal ? .35f : .15f);
        fonte.volume = Mathf.MoveTowards(fonte.volume, volume, Time.unscaledDeltaTime * vel);
        fonte.panStereo = Mathf.Clamp(dx / 7f, -.5f, .5f);
        if (volume > 0 && !fonte.isPlaying) fonte.Play();
        if (volume == 0 && fonte.volume == 0 && fonte.isPlaying) fonte.Stop();
    }

    private void OnDestroy()
    {
        if (clip != null) Destroy(clip);
        if (material != null) Destroy(material);
        if (agua != null) Destroy(agua.gameObject);
        foreach (var item in recursos)
            if (item != null) Destroy(item);
    }
}

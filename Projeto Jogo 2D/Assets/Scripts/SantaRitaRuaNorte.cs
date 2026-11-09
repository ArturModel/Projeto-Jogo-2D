using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

// Bancada visual da Rua Norte. As coordenadas e interações pertencem ao Capitulo1Director.
[DefaultExecutionOrder(150)]
public sealed partial class SantaRitaRuaNorte : MonoBehaviour
{
    private const string Art = "Art/RuaNorte/";
    private const int TileSize = 64;
    private const int CasterLimit = 32;
    private enum Ground { Grass, Road, Walk, Dirt, Verge, Curb, DirtRoad, Crack, Puddle, Stripe, Drain, SideVerge, DirtVerge }

    private enum Sombra { Projetada, Contato, Nenhuma, Veiculo, EdificioSuave }

    private readonly Dictionary<int, Tile> tiles = new();
    private readonly Vector4[] casterMin = new Vector4[CasterLimit];
    private readonly Vector4[] casterMax = new Vector4[CasterLimit];
    private MaterialPropertyBlock receiverProperties;
    private Material lighting;
    private Texture2D groundCells;
    private Texture2D entranceWater;
    private Sprite fogSprite;
    private SpriteRenderer backFog;
    private SpriteRenderer frontFog;
    private Material backFogMaterial;
    private Material frontFogMaterial;
    private SpriteRenderer player;
    private PlayerController playerController;
    private Material previousPlayerMaterial;
    private MaterialPropertyBlock previousPlayerProperties;
    private bool entrada;
    private bool posto;
    private bool encruzilhada;
    private bool bairro;
    private bool AvenidaMolhada => entrada || posto || encruzilhada;
    private static SantaRitaRuaNorte playerLightingOwner;
    private float caminhadaAteRaio;
    private float inicioRaio = float.NegativeInfinity;
    private float posicaoRaio;
    private float baseRaio;
    private float desenhoRaio;
    private AudioSource trovao;
    private float tocarTrovaoEm = float.PositiveInfinity;
    private bool trovaoPausado;
    private int casterCount;
    private Vector2 origin;
    private Vector2 size;

    public static bool Active => Capitulo1Director.ZonaAtual == Capitulo1Director.ZonaId.Zona5_RuaNorteBar;
    public static bool EntradaAtiva => entradaAtual != null &&
        Capitulo1Director.ZonaAtual == Capitulo1Director.ZonaId.Zona1_Rodovia;
    private static SantaRitaRuaNorte entradaAtual;
    private static Sprite[] objetosEntrada;

    public static Sprite ObjetoDaEntrada(string nome)
    {
        objetosEntrada ??= Resources.LoadAll<Sprite>(Art + "Entrada/ObjetosEntrada");
        return System.Array.Find(objetosEntrada, sprite => sprite.name == nome);
    }

    private bool ZonaAtiva => bairro
        ? Capitulo1Director.ZonaAtual == Capitulo1Director.ZonaId.Zona4_Bairro
        : encruzilhada
        ? Capitulo1Director.ZonaAtual == Capitulo1Director.ZonaId.Zona3_Encruzilhada
        : posto
        ? Capitulo1Director.ZonaAtual == Capitulo1Director.ZonaId.Zona2_Posto
        : entrada ? EntradaAtiva : Active;

    public static bool BuildGround(Transform zone, Vector2 min, int width, int height, bool entrada = false, bool posto = false, bool encruzilhada = false, bool bairro = false)
    {
        var atlas = Resources.Load<Texture2D>(Art + "Terreno");
        var shader = Resources.Load<Shader>(Art + "LuzVolumetrica");
        if (atlas == null || shader == null || !shader.isSupported) return false;

        var look = zone.gameObject.AddComponent<SantaRitaRuaNorte>();
        look.receiverProperties = new MaterialPropertyBlock();
        look.origin = min;
        look.size = new Vector2(width, height);
        look.entrada = entrada;
        look.posto = posto;
        look.encruzilhada = encruzilhada;
        look.bairro = bairro;
        if (entrada)
        {
            entradaAtual = look;
        }
        look.lighting = new Material(shader) { name = "Luz local de " + zone.name };
        if (look.AvenidaMolhada)
        {
            look.lighting.SetFloat("_DoorReflection", entrada ? 0 : 1);
            look.lighting.SetFloat("_Crosswalk", encruzilhada ? 1 : 0);
            look.lighting.SetFloat("_Entrance", 1);
        }
        look.lighting.SetFloat("_WetRoad", 1);
        if (bairro) look.lighting.SetFloat("_Bairro", 1);
        look.lighting.SetVector("_Origin", new Vector4(min.x, min.y, 0, 0));
        var surface = Resources.Load<Texture2D>(Art + "Superficie");
        if (surface != null && surface.width == atlas.width && surface.height == atlas.height)
        {
            look.lighting.SetTexture("_GroundSurface", surface);
            look.lighting.SetFloat("_GroundSurfaceEnabled", 1f);
            var relief = Resources.Load<Texture2D>(Art + "Relevo");
            if (relief != null && relief.width == atlas.width && relief.height == atlas.height)
            {
                look.lighting.SetTexture("_GroundRelief", relief);
                look.lighting.SetFloat("_GroundReliefStrength", 1f);
            }
        }
        look.BuildTilemap(atlas, width, height);
        return true;
    }

    public static void Finish(Transform world)
    {
        var look = world.Find("Zona5_RuaNorteBar")?.GetComponent<SantaRitaRuaNorte>();
        if (look != null) { look.Dress(); look.PrepararTemporal(); }
        var entrada = world.Find("Zona1_Rodovia")?.GetComponent<SantaRitaRuaNorte>();
        if (entrada != null) { entrada.DressEntrance(); entrada.PrepararTemporal(); }
        var posto = world.Find("Zona2_Posto")?.GetComponent<SantaRitaRuaNorte>();
        if (posto != null) { posto.DressPosto(); posto.PrepararTemporal(); }
        var encruzilhada = world.Find("Zona3_Encruzilhada")?.GetComponent<SantaRitaRuaNorte>();
        if (encruzilhada != null) { encruzilhada.DressEncruzilhada(); encruzilhada.PrepararTemporal(); }
        var bairro = world.Find("Zona4_Bairro")?.GetComponent<SantaRitaRuaNorte>();
        if (bairro != null) { bairro.DressBairro(); bairro.PrepararTemporal(); }
    }

    private void BuildTilemap(Texture2D atlas, int width, int height)
    {
        var grid = new GameObject("Grade " + name, typeof(Grid));
        grid.transform.SetParent(transform, false);
        grid.transform.position = origin;

        var floor = new GameObject("Piso_" + name, typeof(Tilemap), typeof(TilemapRenderer));
        floor.transform.SetParent(grid.transform, false);
        var map = floor.GetComponent<Tilemap>();
        var renderer = floor.GetComponent<TilemapRenderer>();
        renderer.sortingOrder = -20;
        renderer.sharedMaterial = lighting;
        var cells = new TileBase[width * height];
        var atlasCells = new Color32[cells.Length];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Ground kind;
                bool flipX, flipY;
                if (bairro) SelectBairroGround(x, y, out kind, out flipX, out flipY);
                else if (encruzilhada) SelectEncruzilhadaGround(x, y, out kind, out flipX, out flipY);
                else if (posto) SelectPostoGround(x, y, out kind, out flipX, out flipY);
                else if (entrada) SelectEntranceGround(x, y, out kind, out flipX, out flipY);
                else SelectGround(x, y, out kind, out flipX, out flipY);
                int cell = y * width + x;
                uint hash = (uint)cell * 2654435761u;
                int variant = (int)((hash ^ (hash >> 16)) & 3u);
                cells[cell] = GroundTile(atlas, kind, variant, flipX, flipY);
                atlasCells[cell] = new Color32((byte)variant, (byte)(atlas.height / TileSize - (int)kind - 1),
                    flipX ? (byte)255 : (byte)0, flipY ? (byte)255 : (byte)0);
            }
        }
        map.SetTilesBlock(new BoundsInt(0, 0, 0, width, height, 1), cells);

        // O shader usa a mesma seleção de tiles para ler os dois lados de cada junta.
        groundCells = new Texture2D(width, height, TextureFormat.RGBA32, false, true)
        {
            name = "Vizinhança do piso de " + name,
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        groundCells.SetPixels32(atlasCells);
        groundCells.Apply(false, true);
        lighting.SetTexture("_GroundCells", groundCells);
        BuildWaterMask(width);
    }

    private static void SelectGround(int x, int y, out Ground kind, out bool flipX, out bool flipY)
    {
        flipX = false;
        flipY = false;
        switch (y)
        {
            case 6: kind = x < 6 ? Ground.Grass : Ground.Verge; flipY = true; return;
            case 7: kind = x < 6 ? Ground.DirtVerge : Ground.Curb; flipY = true; return;
            case 12: kind = x < 6 ? Ground.DirtVerge : Ground.Curb; return;
            case 13: kind = x < 6 ? Ground.Grass : (x == 29 ? Ground.Drain : Ground.Walk); return;
            case 14:
                kind = x >= 11 && x <= 24 ? Ground.Walk :
                    (x == 10 || x == 25 ? Ground.SideVerge : Ground.Verge);
                flipX = x == 25;
                return;
            case 15:
                kind = x >= 11 && x <= 24 ? Ground.Verge : Ground.Grass;
                return;
        }

        if (y < 8 || y > 11) { kind = Ground.Grass; return; }
        if (x < 5) { kind = Ground.Dirt; return; }
        if (x == 5) { kind = Ground.DirtRoad; return; }
        if (y == 10 && x % 5 < 2) { kind = Ground.Stripe; return; }
        int wear = (x * 19 + y * 13 + x * y) % 17;
        kind = y != 10 && wear < 2 ? Ground.Puddle : (wear < 6 ? Ground.Crack : Ground.Road);
    }

    private void BuildWaterMask(int width)
    {
        // Posições locais à rodovia. Cada poça tem escala, perfil e brilho próprios.
        const int pixelsPerUnit = 32;
        int firstRow = bairro ? 0 : 8;
        int rows = bairro || encruzilhada ? (int)size.y - firstRow : 4;
        int textureWidth = width * pixelsPerUnit;
        int textureHeight = rows * pixelsPerUnit;
        var pixels = new Color32[textureWidth * textureHeight];
        var puddles = new (float x, float y, float rx, float ry, float tilt, float blue, float shine)[]
        {
            (1.40f, 9.55f, .43f, .22f, -.16f, .65f, .69f),
            (4.75f, 11.35f, .32f, .13f,  .18f, .20f, .63f),
            (6.10f, 8.65f, 1.02f, .38f, -.20f, .86f, .72f),
            (9.45f, 11.48f, .74f, .24f,  .14f, .24f, 1.00f),
            (11.65f, 9.52f, .48f, .30f, -.18f, .92f, .79f),
            (15.10f, 10.97f, 1.18f, .49f,  .10f, .58f, 1.00f),
            (19.50f, 8.84f, .62f, .18f,  .16f, .38f, .55f),
            (22.80f, 11.28f, .80f, .35f, -.24f, .80f, .87f),
            (26.60f, 9.32f, .34f, .23f,  .29f, .28f, .65f),
            (29.90f, 11.20f, 1.10f, .40f, -.14f, .49f, .91f),
            (33.55f, 8.90f, .52f, .20f,  .12f, .90f, .65f)
        };

        if (bairro) puddles = PocasDoBairro;
        if (encruzilhada)
        {
            puddles = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(puddles, new[]
            {
                (19.47f, 15.43f, .36f, .63f, -.08f, .45f, .86f),
                (21.17f, 17.36f, .37f, .58f, .04f, .54f, .81f),
                (19.64f, 20.18f, .30f, .77f, .06f, .38f, .75f),
                (20.80f, 22.48f, .41f, .72f, -.07f, .55f, .84f),
                (19.15f, 24.81f, .30f, .49f, .10f, .47f, .82f)
            }));
        }

        for (int i = 0; i < puddles.Length; i++)
        {
            var pool = puddles[i];
            float seed = 13.6f + i * 8.73f;
            int minX = Mathf.Max(0, Mathf.FloorToInt((pool.x - pool.rx * 1.55f) * pixelsPerUnit));
            int maxX = Mathf.Min(textureWidth, Mathf.CeilToInt((pool.x + pool.rx * 1.55f) * pixelsPerUnit));
            float marginY = pool.ry * 1.55f + Mathf.Abs(pool.tilt) * pool.rx * 1.55f;
            int minY = Mathf.Max(0, Mathf.FloorToInt((pool.y - firstRow - marginY) * pixelsPerUnit));
            int maxY = Mathf.Min(textureHeight, Mathf.CeilToInt((pool.y - firstRow + marginY) * pixelsPerUnit));
            byte blue = (byte)Mathf.RoundToInt(pool.blue * 255);
            byte shine = (byte)Mathf.RoundToInt(pool.shine * 255);
            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    float offsetX = (x + .5f) / pixelsPerUnit - pool.x;
                    float offsetY = firstRow + (y + .5f) / pixelsPerUnit - pool.y;
                    float u = offsetX / pool.rx;
                    float v = (offsetY - pool.tilt * offsetX) / pool.ry;
                    float warp = Mathf.PerlinNoise(u * 1.6f + seed, v * 1.5f + seed) - .5f;
                    float chips = Mathf.PerlinNoise(x * .15f + seed, y * .21f) - .5f;
                    float radius = u * u + v * v + warp * 1.15f + chips * .25f;
                    byte coverage = (byte)Mathf.RoundToInt((1 - Mathf.SmoothStep(0, 1, (radius - .73f) / .32f)) * 255);
                    if (coverage != 0) pixels[y * textureWidth + x] = new Color32(coverage, blue, shine, 255);
                }
            }
        }

        entranceWater = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false, true)
        {
            name = "Máscara das poças de " + name,
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        entranceWater.SetPixels32(pixels);
        entranceWater.Apply(false, true);
        lighting.SetTexture("_EntranceWater", entranceWater);
        lighting.SetVector("_EntranceWaterRect", new Vector4(0, firstRow, width, rows));
    }

    private static void SelectEntranceGround(int x, int y, out Ground kind, out bool flipX, out bool flipY)
    {
        flipX = false;
        flipY = y < 8;
        bool recuo = x >= 6 && x <= 12;
        switch (y)
        {
            case 5: kind = Ground.Verge; return;
            case 6: kind = Ground.Walk; return;
            case 7: kind = Ground.Curb; return;
            case 12: kind = recuo ? Ground.Road : Ground.Curb; return;
            case 13: kind = x == 26 ? Ground.Drain : Ground.Walk; return;
            case 14:
                kind = recuo ? Ground.Walk : (x == 5 || x == 13 ? Ground.SideVerge : Ground.Verge);
                flipX = x == 13;
                return;
            case 15: kind = recuo ? Ground.Verge : Ground.Grass; return;
        }

        if (y < 8 || y > 11) { kind = Ground.Grass; return; }
        if (y == 10 && x % 5 < 2) { kind = Ground.Stripe; return; }
        int wear = (x * 19 + y * 13 + x * y) % 17;
        kind = wear < 5 ? Ground.Crack : Ground.Road;
    }

    private static void SelectPostoGround(int x, int y, out Ground kind, out bool flipX, out bool flipY)
    {
        SelectEntranceGround(x, y, out kind, out flipX, out flipY);
        bool patio = (x >= 7 && x <= 16) || (x >= 24 && x <= 32);
        flipX = false;
        switch (y)
        {
            case 12: kind = Ground.Curb; break;
            case 13: kind = x == 15 || x == 30 ? Ground.Drain : Ground.Walk; break;
            case 14:
                bool edge = x == 6 || x == 17 || x == 23 || x == 33;
                kind = patio ? Ground.Walk : edge ? Ground.SideVerge : Ground.Verge;
                flipX = x == 17 || x == 33;
                break;
            case 15: kind = patio ? Ground.Verge : Ground.Grass; break;
        }
    }

    private static void SelectEncruzilhadaGround(int x, int y, out Ground kind, out bool flipX, out bool flipY)
    {
        SelectEntranceGround(x, y, out kind, out flipX, out flipY);
        bool eixoDaSubida = x >= 18 && x <= 22;
        bool patio = (x >= 6 && x <= 15) || (x >= 27 && x <= 34);

        if (eixoDaSubida && y >= 8 && y <= 13)
        {
            kind = Ground.Road;
        }
        else if (y >= 14 && x >= 16 && x <= 24)
        {
            // A borda contínua da trilha é composta no material sobre a grama.
            kind = Ground.Grass;
        }
        else if (y == 12)
        {
            kind = Ground.Curb;
        }
        else if (y == 13)
        {
            kind = x == 13 || x == 27 ? Ground.Drain : Ground.Walk;
        }
        else if (y == 14)
        {
            kind = patio ? Ground.Walk : Ground.Verge;
        }
        else if (y == 15 && patio)
        {
            kind = Ground.Verge;
        }
        else if (y == 10 && x >= 10 && x < 14)
        {
            kind = Ground.Road;
        }
    }

    private Tile GroundTile(Texture2D atlas, Ground kind, int variant, bool flipX, bool flipY)
    {
        int key = ((int)kind * 4 + variant) * 4 + (flipX ? 1 : 0) + (flipY ? 2 : 0);
        if (tiles.TryGetValue(key, out var tile)) return tile;
        tile = ScriptableObject.CreateInstance<Tile>();
        tile.name = kind.ToString();
        var rect = new Rect(variant * TileSize, atlas.height - ((int)kind + 1) * TileSize, TileSize, TileSize);
        tile.sprite = Sprite.Create(atlas, rect, new Vector2(.5f, .5f), TileSize, 0, SpriteMeshType.FullRect);
        tile.colliderType = Tile.ColliderType.None;
        tile.transform = Matrix4x4.Scale(new Vector3(flipX ? -1 : 1, flipY ? -1 : 1, 1));
        tiles.Add(key, tile);
        return tile;
    }

    private void Dress()
    {
        casterCount = 0;
        for (int i = 0; i < 5; i++)
        {
            DressTree("Arvore_Norte_Z5_" + i, i % 2 != 0 ? "Araucaria" : "Arvore", .84f + i * .045f);
            DressTree("Arvore_Sul_Z5_" + i, i % 2 == 0 ? "Pitangueira" : "Arvore", .78f + i * .033f);
        }
        DressTree("Araucaria_BarDoZe", "Araucaria", 1.05f);
        Plant("Bosque_Oeste", "Bracatinga", 502f, 18.6f, .92f);
        Plant("Bosque_Ombro", "Pitangueira", 512.35f, 17.05f, .83f);
        Plant("Bosque_Fundo", "Bracatinga", 520.55f, 19.2f, .96f);
        Plant("Bosque_Leste", "Pitangueira", 535.55f, 17.1f, .86f);
        Plant("Bosque_Sul", "Pitangueira", 507.9f, 3.55f, .78f);
        Plant("Bosque_Sudeste", "Bracatinga", 533.6f, 4.2f, .9f);

        // Sub-bosque baixo nos pés das árvores e nas bordas do calçamento.
        PlantUnderstory("Moita_Ombro", new Vector2(511.58f, 16.82f), .62f, false);
        PlantUnderstory("Moita_Oeste", new Vector2(510.32f, 16.55f), .49f, true);
        PlantUnderstory("Moita_Leste", new Vector2(524.15f, 16.22f), .73f, true);
        PlantUnderstory("Moita_Fundo", new Vector2(525.23f, 16.58f), .53f, false);

        DressProp("Predio_Fachada_BarDoZe", "Fachada", new Vector3(7.2f, 3.1f, 3.8f), 1f, .82f);
        DressProp("Caminhonete_Clube_Bar", "Picape", new Vector3(3.9f, 1.35f, 1.65f));
        DressProp("Mesa_Calcada_Bar", "Mesa", new Vector3(.86f, .66f, .76f));
        DressChair("Cadeira_Calcada_1", new Vector2(512.26f, 13.29f), false);
        DressChair("Cadeira_Calcada_2", new Vector2(513.53f, 13.40f), true);
        DressProp("Engradados_Bar_Ext", "Engradado", new Vector3(.65f, .45f, .51f));
        DressProp("Dumpster_Bar_Ext", "Contentor", new Vector3(1.24f, .7f, .91f));
        DressProp("Caixas_BarDoZe_Ext", "Caixotes", new Vector3(.64f, .55f, .93f));
        DressProp("Orelhao_BarDoZe", "Orelhao", new Vector3(.25f, .25f, 1.4f));
        DressProp("Portao_Centro", "Portao", new Vector3(4.0f, .42f, 1.8f));
        DressProp("Poste_Z5_1", "Poste", new Vector3(.17f, .2f, 3.15f));
        DressProp("Poste_Z5_2", "Poste", new Vector3(.17f, .2f, 3.15f));
        DressProp("Arbusto_RuaNorte", "Arbusto", new Vector3(.95f, .6f, .65f));
        var drain = transform.Find("Grelha_RuaNorte");
        if (drain != null) drain.gameObject.SetActive(false);

        foreach (var light in GetComponentsInChildren<Light2D>(true))
            light.enabled = false;

        lighting.SetVectorArray("_Lamps", new[]
        {
            Lamp("Poste_Z5_1", .66f, -.42f, 3.18f, 7.25f),
            Lamp("Predio_Fachada_BarDoZe", 0f, -.85f, 2.82f, 7.0f),
            Lamp("Poste_Z5_2", .66f, -.42f, 3.18f, 6.9f)
        });
        lighting.SetVectorArray("_LampColors", new[]
        {
            new Vector4(2.00f, 1.52f, .92f, 1),
            new Vector4(2.05f, 1.28f, .57f, 1),
            new Vector4(1.82f, 1.44f, .87f, 1)
        });
        UpdateCasters(false);
    }

    private void DressEntrance()
    {
        casterCount = 0;
        for (int i = 0; i < 6; i++)
        {
            DressTree("Arvore_Norte_Z1_" + i, i % 3 == 1 ? "Araucaria" : "Arvore", .81f + i * .03f);
            DressTree("Arvore_Sul_Z1_" + i, i % 2 == 0 ? "Pitangueira" : "Bracatinga", .76f + i * .027f);
        }
        Plant("Bosque_Entrada_Oeste", "Araucaria", 101.5f, 17.5f, .89f);
        Plant("Bosque_Entrada_Leste", "Araucaria", 126f, 17.2f, .92f);
        PlantUnderstory("Moita_Entrada_Oeste", new Vector2(104.4f, 15.2f), .64f, false);
        PlantUnderstory("Moita_Entrada_Placa", new Vector2(115.4f, 14.9f), .48f, true);
        PlantUnderstory("Moita_Entrada_Sul", new Vector2(106.5f, 5.35f), .69f, true);
        PlantUnderstory("Moita_Entrada_Acostamento", new Vector2(124.5f, 5.15f), .58f, false);
        DressProp("Carro_Marcus_Sprite", "Entrada/CarroMarcus", new Vector3(3.65f, .78f, 1.54f), sombra: Sombra.Veiculo);
        DressProp("Placa_SantaRita", "Entrada/PlacaSantaRita", new Vector3(2.6f, .34f, 2.45f));
        DressProp("Banco_Rodovia", "Entrada/BancoRodovia", new Vector3(1.65f, .48f, .86f), sombra: Sombra.Contato);
        DressProp("Placa_Horarios_Rodovia", "Entrada/QuadroParada", new Vector3(.8f, .2f, 1.82f), sombra: Sombra.Contato);
        DressProp("Orelhao_Rodovia", "Orelhao", new Vector3(.25f, .25f, 1.4f));
        DressProp("Mala_Abandonada_Rodovia", "Entrada/MalaRodovia", new Vector3(.82f, .36f, .76f), sombra: Sombra.Contato);
        DressProp("Lixeira_Rodovia", null, new Vector3(.23f, .21f, .75f), sombra: Sombra.Contato);
        DressProp("Seta_Para_Posto", null, new Vector3(.23f, .2f, 1.25f), sombra: Sombra.Contato);
        for (int x = 104; x <= 130; x += 6)
            DressProp("CercaSul_" + x, null, new Vector3(2.2f, .35f, .83f), sombra: Sombra.Contato);
        for (int i = 1; i <= 3; i++)
            DressProp("Poste_Z1_" + i, "Poste", new Vector3(.17f, .2f, 3.15f));

        foreach (var light in GetComponentsInChildren<Light2D>(true))
            light.enabled = false;

        var lamps = new[]
        {
            Lamp("Poste_Z1_1", .66f, -.42f, 3.18f, 8.2f),
            Lamp("Poste_Z1_2", .66f, -.42f, 3.18f, 8.2f),
            Lamp("Poste_Z1_3", .66f, -.42f, 3.18f, 8.6f)
        };
        lighting.SetVectorArray("_Lamps", lamps);
        lighting.SetVectorArray("_LampColors", new[]
        {
            new Vector4(1.89f, 1.48f, .97f, 1),
            new Vector4(2.10f, 1.64f, 1.01f, 1),
            new Vector4(1.70f, 1.40f, .92f, 1)
        });
        UpdateCasters(false);
    }

    private void DressPosto()
    {
        casterCount = 0;
        for (int i = 0; i < 6; i++)
        {
            DressTree("Arvore_Norte_Z2_" + i, i % 3 == 1 ? "Araucaria" : "Arvore", .82f + i * .028f);
            DressTree("Arvore_Sul_Z2_" + i, i % 2 == 0 ? "Pitangueira" : "Bracatinga", .80f + i * .026f);
        }
        PlantUnderstory("Moita_Posto_Oeste", new Vector2(205.9f, 15.3f), .62f, false);
        PlantUnderstory("Moita_Posto_Leste", new Vector2(219.2f, 15.9f), .49f, true);
        PlantUnderstory("Moita_Venda", new Vector2(233.4f, 15.2f), .71f, false);

        var facade = DressProp("Predio_Posto", "Posto/Fachada", new Vector3(7.2f, 2.1f, 3.8f), heightFactor: .82f);
        if (facade != null) facade.color = Color.white;
        DressProp("Predio_Venda_Antiga", "Posto/Venda", new Vector3(6.2f, 2.1f, 3.25f), heightFactor: .83f);
        DressProp("Picape_No_Posto", "Posto/Picape", new Vector3(3.65f, 1.10f, 1.65f), sombra: Sombra.Veiculo);
        DressProp("Sedan_Avenida_Posto", "Posto/Sedan", new Vector3(3.44f, .97f, 1.43f), sombra: Sombra.Veiculo);
        DressProp("Banco_Venda", "Entrada/BancoRodovia", new Vector3(1.65f, .48f, .86f), sombra: Sombra.Contato);
        DressProp("Caixotes_Venda", "Caixotes", new Vector3(1.2f, .70f, .93f), sombra: Sombra.Contato);
        for (int i = 1; i <= 4; i++)
            DressProp("Poste_Z2_" + i, "Poste", new Vector3(.17f, .2f, 3.15f));
        for (int i = 1; i <= 2; i++)
            DressProp("UtilityPole_Z2_" + i, null, new Vector3(.22f, .20f, 2.6f));

        foreach (var light in GetComponentsInChildren<Light2D>(true))
            light.enabled = false;

        lighting.SetVectorArray("_Lamps", new[]
        {
            Lamp("Poste_Z2_1", .66f, -.42f, 3.18f, 8.2f),
            Lamp("Predio_Posto", -.4f, -.45f, 2.75f, 7.4f),
            Lamp("Poste_Z2_2", .66f, -.42f, 3.18f, 8.3f)
        });
        lighting.SetVectorArray("_LampColors", new[]
        {
            new Vector4(1.89f, 1.48f, .97f, 1),
            new Vector4(2.15f, 1.34f, .64f, 1),
            new Vector4(1.85f, 1.46f, .94f, 1)
        });
        lighting.SetVector("_FourthLamp", Lamp("Poste_Z2_3", .66f, -.42f, 3.18f, 8.2f));
        lighting.SetVector("_FifthLamp", Lamp("Poste_Z2_4", .66f, -.42f, 3.18f, 8.2f));
        UpdateCasters(false);
    }

    private void DressEncruzilhada()
    {
        casterCount = 0;
        for (int i = 0; i < 5; i++)
            DressTree("Arvore_Norte_Z3_" + i, i % 2 == 0 ? "Arvore" : "Araucaria", .91f + i * .04f);
        for (int i = 0; i < 6; i++)
            DressTree("Arvore_Sul_Z3_" + i, i % 2 == 0 ? "Pitangueira" : "Bracatinga", .82f + i * .025f);

        Plant("Arvore_Subida_Oeste", "Bracatinga", 315.4f, 22.5f, .92f);
        Plant("Arvore_Subida_Leste", "Araucaria", 325.0f, 23.1f, .9f);
        PlantUnderstory("Moita_Mercadinho", new Vector2(315.8f, 16.5f), .68f, false);
        PlantUnderstory("Moita_Subida", new Vector2(324.7f, 19.8f), .83f, true);
        PlantUnderstory("Moita_Esquina", new Vector2(334.4f, 16.9f), .62f, false);
        PlantUnderstory("Moita_Sul", new Vector2(314.7f, 4.8f), .8f, true);

        DressProp("Predio_Mercadinho", "Mercadinho/Fachada", new Vector3(7.2f, 2.4f, 5.6f), heightFactor: .86f);
        DressProp("Casa_Esquina_Encruzilhada", "Mercadinho/Casa", new Vector3(5.15f, 1.75f, 2.65f), heightFactor: .83f, sombra: Sombra.EdificioSuave);
        DressProp("Picape_Encruzilhada_Sprite", "Mercadinho/Picape", new Vector3(3.7f, 1.1f, 1.55f), sombra: Sombra.Veiculo);
        DressProp("Carro_Esquina", "Mercadinho/Carro", new Vector3(3.25f, .97f, 1.4f), sombra: Sombra.Veiculo);
        DressProp("Banco_Encruzilhada", "Entrada/BancoRodovia", new Vector3(1.65f, .48f, .86f), sombra: Sombra.Contato);
        DressProp("Engradado_Merc_1", "Caixotes", new Vector3(.9f, .55f, .93f), sombra: Sombra.Contato);
        DressProp("Engradado_Merc_2", "Engradado", new Vector3(.65f, .45f, .51f), sombra: Sombra.Contato);
        DressProp("Lixeira_Mercadinho", "Contentor", new Vector3(1.24f, .7f, .91f));
        DressProp("Mural_Encruzilhada", null, new Vector3(.72f, .2f, 1.8f));
        DressProp("Cerca_Esquina", null, new Vector3(3.4f, .45f, 1.1f));
        DressProp("Placa_Subida_Bairro", null, new Vector3(.23f, .2f, 1.25f));
        for (int i = 1; i <= 4; i++)
            DressProp("Poste_Z3_" + i, "Poste", new Vector3(.17f, .2f, 3.15f));
        for (int i = 1; i <= 2; i++)
            DressProp("UtilityPole_Z3_" + i, null, new Vector3(.22f, .20f, 2.6f));

        foreach (var light in GetComponentsInChildren<Light2D>(true))
            light.enabled = false;

        lighting.SetVectorArray("_Lamps", new[]
        {
            Lamp("Poste_Z3_1", .66f, -.42f, 3.18f, 8.2f),
            Lamp("Predio_Mercadinho", .08f, -.45f, 2.75f, 7.5f),
            Lamp("Poste_Z3_2", .66f, -.42f, 3.18f, 8.2f)
        });
        lighting.SetVectorArray("_LampColors", new[]
        {
            new Vector4(1.89f, 1.48f, .97f, 1),
            new Vector4(2.15f, 1.34f, .64f, 1),
            new Vector4(1.85f, 1.46f, .94f, 1)
        });
        lighting.SetVector("_FourthLamp", Lamp("Poste_Z3_3", .66f, -.42f, 3.18f, 8.2f));
        lighting.SetVector("_FifthLamp", Lamp("Poste_Z3_4", .66f, -.42f, 3.18f, 8.2f));
        UpdateCasters(false);
    }

    private void DressChair(string name, Vector2 feet, bool faceLeft)
    {
        var chair = transform.Find(name);
        if (chair == null) return;
        chair.position = feet;
        var renderer = DressProp(name, "Cadeira", new Vector3(.45f, .38f, .78f));
        if (renderer == null) return;
        renderer.flipX = faceLeft;
        renderer.sortingOrder = 1000 - Mathf.RoundToInt(feet.y * 10);
        renderer.color = new Color(.88f, .94f, .96f);
    }

    private void PlantUnderstory(string name, Vector2 feet, float scale, bool flip)
    {
        var sprite = Resources.Load<Sprite>(Art + "Arbusto");
        if (sprite == null) return;
        var obj = transform.Find(name)?.gameObject ?? new GameObject(name, typeof(SpriteRenderer));
        obj.transform.SetParent(transform, false);
        obj.transform.position = feet;
        obj.transform.localScale = new Vector3(scale, scale, 1);
        var renderer = obj.GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.flipX = flip;
        renderer.color = new Color(.72f, .81f, .77f);
        renderer.sortingOrder = 1000 - Mathf.RoundToInt(feet.y * 10);
        renderer.sharedMaterial = lighting;
        SetReceiver(renderer, feet.y, .85f, -1);
    }

    private Vector4 Lamp(string prop, float xOffset, float yOffset, float height, float reach)
    {
        Vector2 at = (Vector2)transform.Find(prop).position - origin;
        return new Vector4(at.x + xOffset, at.y + yOffset, height, reach);
    }

    private void Plant(string name, string art, float x, float y, float scale)
    {
        var tree = transform.Find(name);
        if (tree == null)
        {
            var obj = new GameObject(name, typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(SantaRitaForeground));
            tree = obj.transform;
            tree.SetParent(transform, false);
            tree.position = new Vector3(x, y, 0);
            obj.GetComponent<SpriteRenderer>().sortingOrder = 1000 - Mathf.RoundToInt(y * 10);
            var collider = obj.GetComponent<BoxCollider2D>();
            collider.size = new Vector2(.62f, .43f) * scale;
            collider.offset = new Vector2(0, .23f) * scale;
        }
        DressTree(name, art, scale);
    }

    private void DressTree(string name, string art, float scale)
    {
        var renderer = DressProp(name, art, new Vector3(.53f, .63f, 2.85f) * scale, scale,
            sombra: AvenidaMolhada || bairro ? Sombra.Nenhuma : Sombra.Projetada);
        if (renderer == null) return;
        renderer.color = new Color(.87f, .94f, .93f);
        renderer.GetPropertyBlock(receiverProperties);
        receiverProperties.SetVector("_Wind", new Vector4(.092f * scale,
            renderer.bounds.size.y, renderer.transform.position.x * .79f, 0));
        renderer.SetPropertyBlock(receiverProperties);
    }

    private SpriteRenderer DressProp(string name, string art, Vector3 volume, float scale = 1f,
        float heightFactor = 1f, Sombra sombra = Sombra.Projetada)
    {
        var prop = transform.Find(name);
        if (prop == null) return null;
        var renderer = prop.GetComponent<SpriteRenderer>() ?? prop.GetComponentInChildren<SpriteRenderer>(true);
        if (renderer == null) return null;
        if (art != null)
        {
            var sprite = art.StartsWith("Entrada/", System.StringComparison.Ordinal)
                ? ObjetoDaEntrada(art.Substring("Entrada/".Length))
                : Resources.Load<Sprite>(Art + art);
            if (sprite == null) return null;
            renderer.sprite = sprite;
        }

        // A escala dos sprites novos é de 64 pixels por unidade. Compense os colisores existentes.
        if (art != null && renderer.transform == prop)
        {
            Vector3 ratio = prop.localScale / scale;
            foreach (var box in prop.GetComponents<BoxCollider2D>())
            {
                box.size = Vector2.Scale(box.size, ratio);
                box.offset = Vector2.Scale(box.offset, ratio);
            }
            prop.localScale = new Vector3(scale, scale, 1f);
        }

        Vector2 at = (Vector2)prop.position - origin;
        int id = -1;
        if (casterCount < CasterLimit - 1)
        {
            id = casterCount++;
            casterMin[id] = new Vector4(at.x - volume.x * .5f, at.y + .04f, 0, (float)sombra);
            casterMax[id] = new Vector4(at.x + volume.x * .5f, at.y + volume.y, volume.z, 0);
        }
        if (AvenidaMolhada) renderer.sortingOrder = 1000 - Mathf.RoundToInt(prop.position.y * 10);
        renderer.sharedMaterial = lighting;
        SetReceiver(renderer, prop.position.y, heightFactor, id);
        return renderer;
    }

    private void SetReceiver(SpriteRenderer renderer, float y, float heightFactor, int id)
    {
        renderer.GetPropertyBlock(receiverProperties);
        receiverProperties.SetVector("_Receiver", new Vector4(y, heightFactor, id + 1, 0));
        renderer.SetPropertyBlock(receiverProperties);
    }

    private void PrepararTemporal()
    {
        caminhadaAteRaio = Random.Range(12f, 22f);
        var thunderClip = Resources.Load<AudioClip>("Audio/TrovaoEntrada");
        if (thunderClip != null)
        {
            trovao = gameObject.AddComponent<AudioSource>();
            trovao.playOnAwake = false;
            trovao.clip = thunderClip;
            trovao.spatialBlend = 0;
        }

        DressFog();
        var lamps = lighting.GetVectorArray("_Lamps");
        if (backFogMaterial != null) backFogMaterial.SetVectorArray("_EntranceLamps", lamps);
        if (frontFogMaterial != null) frontFogMaterial.SetVectorArray("_EntranceLamps", lamps);
    }

    private void DressFog()
    {
        if (backFog == null)
        {
            var shader = Resources.Load<Shader>(Art + "Nevoeiro");
            if (shader == null || !shader.isSupported) return;
            var house = new Vector4(origin.x + size.x * .5f, 15.8f, 0f, 0f);
            if (!AvenidaMolhada)
            {
                var facade = transform.Find(bairro ? "Predio_CasaDoAvo_40" : "Predio_Fachada_BarDoZe").GetComponent<SpriteRenderer>();
                var baseAt = facade.transform.position;
                house = new Vector4(baseAt.x, baseAt.y, facade.bounds.extents.x, 3.1f);
            }
            var texture = Texture2D.whiteTexture;
            fogSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                Vector2.zero, texture.width, 0, SpriteMeshType.FullRect);
            backFog = CreateFog("Nevoeiro sobre o bosque", shader, house, false);
            backFogMaterial = backFog.sharedMaterial;
            frontFog = CreateFog(entrada ? "Névoa baixa entre os postes" : "Nevoeiro junto à soleira",
                shader, house, true);
            frontFogMaterial = frontFog.sharedMaterial;
        }
        backFog.sortingOrder = 2030;
        if (frontFog != null) frontFog.sortingOrder = 2031;
        backFog.sharedMaterial = backFogMaterial;
        if (frontFog != null) frontFog.sharedMaterial = frontFogMaterial;
    }

    private SpriteRenderer CreateFog(string name, Shader shader, Vector4 house, bool front)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(transform, false);
        obj.transform.position = origin;
        obj.transform.localScale = new Vector3(size.x, size.y, 1);
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = fogSprite;
        var material = new Material(shader);
        material.SetVector("_Ground", new Vector4(origin.x, origin.y, size.x, size.y));
        material.SetVector("_Bar", house);
        material.SetFloat("_Front", front ? 1 : 0);
        material.SetFloat("_Entrance", AvenidaMolhada ? 1 : 0);
        renderer.sharedMaterial = material;
        return renderer;
    }

    private void LateUpdate()
    {
        if (lighting == null || receiverProperties == null) return;
        bool effects = ZonaAtiva && !MenuPausa.ReduzirEfeitosIntensos;
        if (trovao != null) AtualizarTrovao(effects);
        if (backFog != null)
        {
            backFog.enabled = effects;
            if (frontFog != null) frontFog.enabled = effects;
        }
        lighting.SetFloat("_WindStrength", effects ? 1 : 0);
        if (!ZonaAtiva)
        {
            if (player != null) { RestorePlayer(); UpdateCasters(false); }
            lighting.SetVector("_ViewFocus", Vector4.zero);
            inicioRaio = float.NegativeInfinity;
            lighting.SetFloat("_Lightning", 0);
            lighting.SetVector("_MirrorBounds", Vector4.zero);
            return;
        }
        if (player == null)
        {
            var actor = FindAnyObjectByType<PlayerController>();
            if (actor == null || !actor.TryGetComponent(out player)) return;
            playerLightingOwner?.RestorePlayer();
            playerLightingOwner = this;
            playerController = actor;
            previousPlayerMaterial = player.sharedMaterial;
            previousPlayerProperties = new MaterialPropertyBlock();
            player.GetPropertyBlock(previousPlayerProperties);
            player.sharedMaterial = lighting;
        }
        Vector2 focus = (Vector2)player.transform.position - origin;
        lighting.SetVector("_ViewFocus", new Vector4(focus.x, focus.y + .7f, effects ? 1f : .35f, 0));
        SetReceiver(player, player.transform.position.y, .94f, casterCount);
        UpdateCasters(true);
        AtualizarClimaDaRua(effects);
        AtualizarReflexoDoMarcus();
        if (AvenidaMolhada || bairro)
        {
            if (playerController.ObterFeixeLanterna(out var lente, out var direcao))
            {
                Vector2 ponto = lente - origin;
                lighting.SetVector("_PlayerLamp", new Vector4(ponto.x, ponto.y, lente.y - player.transform.position.y, 6.2f));
                lighting.SetVector("_PlayerLampDirection", direcao);
            }
            else lighting.SetVector("_PlayerLamp", Vector4.zero);
        }
    }

    private void AtualizarClimaDaRua(bool effects)
    {
        if (!effects || !Application.isFocused || MenuPausa.Aberto || SceneFader.IsTransitioning)
            inicioRaio = float.NegativeInfinity;

        if (effects && playerController.CaminhandoParaEventoAmbiental)
        {
            caminhadaAteRaio -= Time.deltaTime;
            if (caminhadaAteRaio <= 0)
            {
                caminhadaAteRaio = Random.Range(25f, 48f);
                inicioRaio = Time.time;
                posicaoRaio = Mathf.Clamp(player.transform.position.x + Random.Range(-3f, 3f),
                    origin.x + 2, origin.x + size.x - 2);
                desenhoRaio = Random.value * 30f;
                baseRaio = bairro ? Mathf.Clamp(player.transform.position.y + 1.6f, 2.5f, size.y - 3f) : 14.7f;
                playerController.ReagirAoRaio();
                tocarTrovaoEm = Time.time + .20f;
            }
        }

        float elapsed = Time.time - inicioRaio;
        float flash = 1 - Mathf.SmoothStep(0, 1, elapsed / .48f);
        lighting.SetFloat("_Lightning", flash);
        var bolt = new Vector4(posicaoRaio, desenhoRaio, flash, baseRaio);
        if (backFogMaterial != null) backFogMaterial.SetVector("_Lightning", bolt);
        if (frontFogMaterial != null) frontFogMaterial.SetVector("_Lightning", bolt);
        float recoil = Mathf.Clamp01(elapsed / .24f);
        player.GetPropertyBlock(receiverProperties);
        receiverProperties.SetFloat("_Startle", Mathf.Round(Mathf.Sin(recoil * Mathf.PI) * 3) / 32f);
        player.SetPropertyBlock(receiverProperties);
    }

    private void AtualizarTrovao(bool effects)
    {
        if (!effects)
        {
            if (trovao.isPlaying || trovaoPausado) trovao.Stop();
            trovaoPausado = false;
            tocarTrovaoEm = float.PositiveInfinity;
            return;
        }

        if (!Application.isFocused || MenuPausa.Aberto || Time.timeScale == 0)
        {
            if (trovao.isPlaying)
            {
                trovao.Pause();
                trovaoPausado = true;
            }
            return;
        }

        // O trovão é mais forte que os efeitos de interface, mas usa o mesmo controle.
        trovao.volume = AudioSettingsController.VolumeEfeitos * 4.8f;
        if (trovaoPausado)
        {
            trovao.UnPause();
            trovaoPausado = false;
        }
        if (Time.time >= tocarTrovaoEm)
        {
            tocarTrovaoEm = float.PositiveInfinity;
            trovao.Play();
        }
    }

    private void AtualizarReflexoDoMarcus()
    {
        var frame = player.sprite;
        if (frame == null || frame.packed)
        {
            lighting.SetVector("_MirrorBounds", Vector4.zero);
            return;
        }

        var bounds = player.bounds;
        var uv = frame.rect;
        var atlas = frame.texture;
        bool mirrored = player.flipX ^ (player.transform.lossyScale.x < 0);
        lighting.SetTexture("_MarcusFrame", atlas);
        lighting.SetVector("_MirrorUV", new Vector4(
            (mirrored ? uv.xMax : uv.xMin) / atlas.width, uv.yMin / atlas.height,
            (mirrored ? -uv.width : uv.width) / atlas.width, uv.height / atlas.height));
        lighting.SetVector("_MirrorBounds", new Vector4(bounds.min.x - origin.x, bounds.min.y - origin.y,
            bounds.size.x, bounds.size.y));
        lighting.SetFloat("_MirrorFeet", player.transform.position.y - origin.y);
    }

    private void UpdateCasters(bool withPlayer)
    {
        if (withPlayer)
        {
            Vector2 at = (Vector2)player.transform.position - origin;
            casterMin[casterCount] = new Vector4(at.x - .22f, at.y - .10f, 0, 0);
            casterMax[casterCount] = new Vector4(at.x + .22f, at.y + .16f, 1.54f, 0);
        }
        lighting.SetInt("_CasterCount", casterCount + (withPlayer ? 1 : 0));
        lighting.SetVectorArray("_CasterMin", casterMin);
        lighting.SetVectorArray("_CasterMax", casterMax);
    }

    private void RestorePlayer()
    {
        if (playerLightingOwner == this) playerLightingOwner = null;
        if (player == null) return;
        if (player.sharedMaterial == lighting)
        {
            player.sharedMaterial = previousPlayerMaterial;
            player.SetPropertyBlock(previousPlayerProperties);
        }
        player = null;
    }

    private void OnDestroy()
    {
        if (entradaAtual == this) entradaAtual = null;
        RestorePlayer();
        foreach (var tile in tiles.Values)
        {
            Destroy(tile.sprite);
            Destroy(tile);
        }
        Destroy(groundCells);
        Destroy(entranceWater);
        Destroy(fogSprite);
        Destroy(backFogMaterial);
        Destroy(frontFogMaterial);
        Destroy(lighting);
    }
}

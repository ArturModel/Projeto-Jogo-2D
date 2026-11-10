using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

// Bancada visual da Rua Norte. As coordenadas e interações pertencem ao Capitulo1Director.
[DefaultExecutionOrder(150)]
public sealed partial class SantaRitaRuaNorte : MonoBehaviour
{
    private const string Art = "Art/RuaNorte/";
    private const int TileSize = 64;
    private const int CasterLimit = 48;
    private enum Ground { Grass, Road, Walk, Dirt, Verge, Curb, DirtRoad, Crack, Puddle, Stripe, Drain, SideVerge, DirtVerge }

    private enum Sombra { Projetada, Contato, Nenhuma, Veiculo, EdificioSuave, ProjetadaSuave, Recorte }

    private readonly Dictionary<int, Tile> tiles = new();
    private readonly Vector4[] casterMin = new Vector4[CasterLimit];
    private readonly Vector4[] casterMax = new Vector4[CasterLimit];
    private MaterialPropertyBlock receiverProperties;
    private Material lighting;
    private Texture2D groundCells;
    private Texture2D entranceWater;
    private const int WaterPixelsPerUnit = 32;
    private int firstWaterRow;
    private Sprite fogSprite;
    private SpriteRenderer backFog;
    private SpriteRenderer frontFog;
    private Material backFogMaterial;
    private Material frontFogMaterial;
    private readonly List<Vector4> bocaisDosPostes = new();
    private readonly List<(LensFlareComponentSRP flare, Vector4 fonte)> flaresDaRua = new();
    private GameObject flaresDosPostes;
    private LensFlareDataSRP perfilDosFlares;
    private SpriteRenderer player;
    private PlayerController playerController;
    private Material previousPlayerMaterial;
    private MaterialPropertyBlock previousPlayerProperties;
    private bool entrada;
    private bool exteriorComposto;
    private bool posto;
    private bool encruzilhada;
    private bool bairro;
    private bool RuaDoBar => !entrada && !posto && !encruzilhada && !bairro;
    private bool AvenidaMolhada => !bairro;
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
    public static bool EmZonaDaRuaNorte => playerLightingOwner != null || Active || EntradaAtiva;
    private static SantaRitaRuaNorte entradaAtual;

    internal static bool ControlaRenderDaRua(SpriteRenderer renderer)
    {
        var exterior = renderer.GetComponentInParent<SantaRitaRuaNorte>();
        return exterior != null && exterior.exteriorComposto;
    }
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
        if (look.RuaDoBar) look.lighting.SetFloat("_BarStreet", 1);
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
        if (look != null && !look.exteriorComposto)
        {
            look.DressBar();
            look.PrepararTemporal();
            look.exteriorComposto = true;
        }
        var entrada = world.Find("Zona1_Rodovia")?.GetComponent<SantaRitaRuaNorte>();
        if (entrada != null && !entrada.exteriorComposto)
        {
            entrada.DressEntrance();
            entrada.PrepararTemporal();
            entrada.exteriorComposto = true;
        }
        var posto = world.Find("Zona2_Posto")?.GetComponent<SantaRitaRuaNorte>();
        if (posto != null && !posto.exteriorComposto)
        {
            posto.DressPosto();
            posto.PrepararTemporal();
            posto.exteriorComposto = true;
        }
        var encruzilhada = world.Find("Zona3_Encruzilhada")?.GetComponent<SantaRitaRuaNorte>();
        if (encruzilhada != null && !encruzilhada.exteriorComposto)
        {
            encruzilhada.DressEncruzilhada();
            encruzilhada.PrepararTemporal();
            encruzilhada.exteriorComposto = true;
        }
        var bairro = world.Find("Zona4_Bairro")?.GetComponent<SantaRitaRuaNorte>();
        if (bairro != null && !bairro.exteriorComposto)
        {
            bairro.DressBairro();
            bairro.PrepararTemporal();
            bairro.exteriorComposto = true;
        }
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
                else SelectBarGround(x, y, out kind, out flipX, out flipY);
                int cell = y * width + x;
                int variant = GroundVariant(cell);
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

    private void BuildWaterMask(int width)
    {
        // Posições locais à rodovia. Cada poça tem escala, perfil e brilho próprios.
        const int pixelsPerUnit = WaterPixelsPerUnit;
        int firstRow = firstWaterRow = bairro ? 0 : 8;
        int rows = bairro || encruzilhada ? (int)size.y - firstRow : 5;
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

        if (entrada)
        {
            puddles = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(puddles, new[]
            {
                (6.61f, 11.83f, .52f, .68f, .19f, .34f, .92f),
                (16.49f, 11.94f, .62f, .72f, -.19f, .32f, .93f),
                (29.38f, 11.86f, .78f, .44f, .14f, .48f, .88f),
                (9.18f, 11.89f, 1.13f, .28f, .05f, .57f, .83f),
                (12.52f, 8.22f, .82f, .14f, .05f, .77f, .76f),
                (23.47f, 8.20f, .53f, .13f, -.07f, .59f, .72f)
            }));
        }
        if (RuaDoBar) puddles = PocasDoBar;
        if (bairro) puddles = PocasDoBairro;
        if (posto) puddles = PocasDoPosto;
        if (encruzilhada)
        {
            puddles = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(puddles, new[]
            {
                (7.85f, 11.82f, .86f, .46f, -.14f, .54f, .84f),
                (19.47f, 15.43f, .42f, .68f, -.08f, .52f, .82f),
                (21.17f, 17.36f, .44f, .62f, .04f, .58f, .79f),
                (19.64f, 20.18f, .36f, .78f, .06f, .48f, .76f),
                (20.80f, 22.48f, .46f, .74f, -.07f, .58f, .81f),
                (19.15f, 24.81f, .34f, .54f, .10f, .52f, .78f)
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
        entranceWater.Apply(false, false); // O contato do pé lê a mesma cobertura que o shader.
        lighting.SetTexture("_EntranceWater", entranceWater);
        lighting.SetVector("_EntranceWaterRect", new Vector4(0, firstRow, width, rows));
    }

    internal float CoberturaDePoca(Vector2 posicao)
    {
        if (entranceWater == null || !ZonaAtiva) return 0f;

        Vector2 local = posicao - origin - Vector2.up * firstWaterRow;
        int x = Mathf.FloorToInt(local.x * WaterPixelsPerUnit);
        int y = Mathf.FloorToInt(local.y * WaterPixelsPerUnit);
        if (x < 0 || y < 0 || x >= entranceWater.width || y >= entranceWater.height) return 0f;

        return entranceWater.GetPixel(x, y).r;
    }

    private static int GroundVariant(int cell)
    {
        uint hash = (uint)cell * 2654435761u;
        return (int)((hash ^ (hash >> 16)) & 3u);
    }

    // O prólogo consulta as mesmas células da Rodovia em perspectiva.
    internal static Texture2D CreateEntranceGroundLookup(Texture2D atlas)
    {
        const int width = 36, height = 20;
        var lookup = new Texture2D(width, height, TextureFormat.RGBA32, false, true)
        {
            name = "Piso da Rodovia no prólogo",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        var pixels = new Color32[width * height];
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            SelectEntranceGround(x, y, out var kind, out bool flipX, out bool flipY);
            int cell = y * width + x;
            pixels[cell] = new Color32((byte)GroundVariant(cell), (byte)(atlas.height / TileSize - (int)kind - 1),
                flipX ? (byte)255 : (byte)0, flipY ? (byte)255 : (byte)0);
        }
        lookup.SetPixels32(pixels);
        lookup.Apply(false, true);
        return lookup;
    }

    private static void SelectEntranceGround(int x, int y, out Ground kind, out bool flipX, out bool flipY)
    {
        SelectAvenueGround(x, y, out kind, out flipX, out flipY);
        if (kind == Ground.Crack && (x * 17 + y * 23) % 5 != 0) kind = Ground.Road;
        bool parada = x >= 18 && x <= 20;
        bool orelhao = x == 26 || x == 27;
        if (y == 13) kind = Ground.Walk;
        if (y == 14 && (parada || orelhao)) kind = Ground.Walk;
        if (y == 14 && (x == 17 || x == 21 || x == 25 || x == 28))
        {
            kind = Ground.SideVerge;
            flipX = x == 21 || x == 28;
        }
        if (y == 15 && (parada || orelhao)) kind = Ground.Verge;
    }

    private static void SelectAvenueGround(int x, int y, out Ground kind, out bool flipX, out bool flipY)
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
        SelectAvenueGround(x, y, out kind, out flipX, out flipY);
        bool patio = (x >= 7 && x <= 16) || (x >= 24 && x <= 32);
        flipX = false;
        switch (y)
        {
            case 12: kind = Ground.Curb; break;
            case 13: kind = Ground.Walk; break;
            case 14:
                bool edge = x == 6 || x == 17 || x == 23 || x == 33;
                kind = patio ? Ground.Walk : edge ? Ground.SideVerge : Ground.Verge;
                flipX = x == 17 || x == 33;
                break;
            case 15: kind = patio ? Ground.Verge : Ground.Grass; break;
        }
        if (kind == Ground.Crack && (x + 2 * y) % 4 != 0) kind = Ground.Road;
    }

    private static void SelectEncruzilhadaGround(int x, int y, out Ground kind, out bool flipX, out bool flipY)
    {
        SelectAvenueGround(x, y, out kind, out flipX, out flipY);
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
            kind = Ground.Walk;
        }
        else if (y == 14)
        {
            kind = patio ? Ground.Walk : Ground.Verge;
        }
        else if (y == 15 && patio)
        {
            kind = Ground.Verge;
        }
        else if (y == 10 && x >= 8 && x < 14)
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
        var carro = DressProp("Carro_Marcus_Sprite", "Entrada/CarroMarcus", new Vector3(3.65f, .78f, 1.54f), sombra: Sombra.Veiculo);
        DressProp("Placa_SantaRita", "Entrada/PlacaSantaRita", new Vector3(2.6f, .34f, 2.45f), sombra: Sombra.Recorte);
        DressProp("Banco_Rodovia", "Entrada/BancoRodovia", new Vector3(1.65f, .48f, .86f), sombra: Sombra.Recorte);
        DressProp("Placa_Horarios_Rodovia", "Entrada/QuadroParada", new Vector3(.8f, .2f, 1.82f), sombra: Sombra.Recorte);
        DressProp("Orelhao_Rodovia", "Orelhao", new Vector3(.25f, .25f, 1.4f));
        DressProp("Mala_Abandonada_Rodovia", "Entrada/MalaRodovia", new Vector3(.82f, .36f, .76f), sombra: Sombra.Contato);
        DressProp("Lixeira_Rodovia", null, new Vector3(.47f, .31f, .75f), sombra: Sombra.ProjetadaSuave);
        DressProp("Seta_Para_Posto", null, new Vector3(.23f, .2f, 1.25f), sombra: Sombra.Contato);
        for (int x = 104; x <= 130; x += 6)
            DressProp("CercaSul_" + x, null, new Vector3(2.2f, .35f, .83f), sombra: Sombra.Nenhuma);
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
        ComporChegada(carro);
        ComporBosqueDaRua();
        luzesDeRecorte = lamps;
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
        DressTree("Araucaria_Posto_Fundo", "Araucaria", 1.03f);
        PlantUnderstory("Moita_Posto_Oeste", new Vector2(205.9f, 15.3f), .62f, false);
        PlantUnderstory("Moita_Posto_Leste", new Vector2(219.2f, 15.9f), .49f, true);
        PlantUnderstory("Moita_Venda", new Vector2(233.4f, 15.2f), .71f, false);

        var facade = DressProp("Predio_Posto", "Posto/Fachada", new Vector3(7.2f, 2.1f, 3.8f), heightFactor: .82f);
        if (facade != null) facade.color = Color.white;
        DressProp("Predio_Venda_Antiga", "Posto/Venda", new Vector3(6.2f, 2.1f, 3.25f), heightFactor: .83f);
        var picape = DressProp("Picape_No_Posto", "Posto/Picape", new Vector3(3.65f, 1.10f, 1.65f), sombra: Sombra.Veiculo);
        DressProp("Sedan_Avenida_Posto", "Posto/Sedan", new Vector3(3.44f, .97f, 1.43f), sombra: Sombra.Veiculo);
        DressProp("Banco_Venda", "Entrada/BancoRodovia", new Vector3(1.65f, .48f, .86f), sombra: Sombra.Recorte);
        for (int i = 1; i <= 4; i++)
            DressProp("Poste_Z2_" + i, "Poste", new Vector3(.17f, .2f, 3.15f));

        foreach (var light in GetComponentsInChildren<Light2D>(true))
            light.enabled = false;

        var lamps = new[]
        {
            Lamp("Poste_Z2_1", .66f, -.42f, 3.18f, 8.2f),
            Lamp("Predio_Posto", -.4f, -.45f, 2.75f, 7.4f),
            Lamp("Poste_Z2_2", .66f, -.42f, 3.18f, 8.3f)
        };
        lighting.SetVectorArray("_Lamps", lamps);
        lighting.SetVectorArray("_LampColors", new[]
        {
            new Vector4(1.89f, 1.48f, .97f, 1),
            new Vector4(2.15f, 1.34f, .64f, 1),
            new Vector4(1.85f, 1.46f, .94f, 1)
        });
        lighting.SetVector("_FourthLamp", Lamp("Poste_Z2_3", .66f, -.42f, 3.18f, 8.2f));
        lighting.SetVector("_FifthLamp", Lamp("Poste_Z2_4", .66f, -.42f, 3.18f, 8.2f));
        ComporPosto(facade, picape);
        ComporBosqueDaRua();
        luzesDeRecorte = lamps;
        UpdateCasters(false);
    }

    private void DressEncruzilhada()
    {
        casterCount = 0;
        for (int i = 0; i < 5; i++)
            DressTree("Arvore_Norte_Z3_" + i, i % 2 == 0 ? "Arvore" : "Araucaria", .91f + i * .04f);
        for (int i = 0; i < 6; i++)
            DressTree("Arvore_Sul_Z3_" + i, i % 2 == 0 ? "Pitangueira" : "Bracatinga", .82f + i * .025f);

        DressTree("Araucaria_Encruzilhada", "Araucaria", 1.03f);
        Plant("Arvore_Subida_Oeste", "Bracatinga", 315.4f, 22.5f, .92f);
        Plant("Arvore_Subida_Leste", "Araucaria", 325.0f, 23.1f, .9f);
        PlantUnderstory("Moita_Mercadinho", new Vector2(315.8f, 16.5f), .68f, false);
        PlantUnderstory("Moita_Subida", new Vector2(324.7f, 19.8f), .83f, true);
        PlantUnderstory("Moita_Esquina", new Vector2(334.4f, 16.9f), .62f, false);
        PlantUnderstory("Moita_Sul", new Vector2(314.7f, 4.8f), .8f, true);

        var fachada = DressProp("Predio_Mercadinho", "Mercadinho/Fachada", new Vector3(7.2f, 2.4f, 5.6f), heightFactor: .86f);
        DressProp("Casa_Esquina_Encruzilhada", "Mercadinho/Casa", new Vector3(6.62f, 2.0f, 3.10f),
            scale: 1.30f, heightFactor: .83f, sombra: Sombra.EdificioSuave);
        DressProp("Picape_Encruzilhada_Sprite", "Mercadinho/Picape", new Vector3(3.7f, 1.1f, 1.55f), sombra: Sombra.Veiculo);
        var carro = DressProp("Carro_Esquina", "Mercadinho/Carro", new Vector3(3.25f, .97f, 1.4f), sombra: Sombra.Veiculo);
        DressProp("Banco_Encruzilhada", "Entrada/BancoRodovia", new Vector3(1.65f, .48f, .86f), sombra: Sombra.Recorte);
        if (DressProp("Lixeira_Mercadinho", "Mercadinho/LixeiraBaixa", new Vector3(.63f, .37f, .63f),
                sombra: Sombra.ProjetadaSuave) == null)
            DressProp("Lixeira_Mercadinho", "Contentor", new Vector3(1.24f, .7f, .91f));
        DressProp("Mural_Encruzilhada", null, new Vector3(.72f, .2f, 1.8f));
        DressProp("Aviso_Obra_Drenagem", null, new Vector3(.40f, .14f, .68f));
        DressProp("Placa_Subida_Bairro", null, new Vector3(.23f, .2f, 1.25f));
        for (int i = 1; i <= 3; i++)
            DressProp("Poste_Z3_" + i, "Poste", new Vector3(.17f, .2f, 3.15f));

        foreach (var light in GetComponentsInChildren<Light2D>(true))
            light.enabled = false;

        var lamps = new[]
        {
            Lamp("Poste_Z3_1", .66f, -.42f, 3.18f, 8.2f),
            Lamp("Predio_Mercadinho", .08f, -.45f, 2.75f, 7.5f),
            Lamp("Poste_Z3_2", .66f, -.42f, 3.18f, 8.2f)
        };
        lighting.SetVectorArray("_Lamps", lamps);
        lighting.SetVectorArray("_LampColors", new[]
        {
            new Vector4(1.89f, 1.48f, .97f, 1),
            new Vector4(2.15f, 1.34f, .64f, 1),
            new Vector4(1.85f, 1.46f, .94f, 1)
        });
        lighting.SetVector("_FourthLamp", Lamp("Poste_Z3_3", .66f, -.42f, 3.18f, 8.2f));
        ComporEncruzilhada(fachada, carro);
        ComporBosqueDaRua();
        luzesDeRecorte = lamps;
        UpdateCasters(false);
    }

    private void DressChair(string name, bool faceLeft)
    {
        var renderer = DressProp(name, "Cadeira", new Vector3(.45f, .38f, .78f));
        if (renderer == null) return;
        renderer.flipX = faceLeft;
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
        var objeto = transform.Find(prop);
        Vector2 at = (Vector2)objeto.position - origin;
        if (height > 3f && objeto.TryGetComponent<SpriteRenderer>(out var poste))
        {
            // Centro do vidro âmbar na arte de 72 × 216 px, medido a partir da base.
            var bounds = poste.bounds;
            float x = poste.flipX ? 1f - 58f / 72f : 58f / 72f;
            float bocalX = Mathf.Lerp(bounds.min.x, bounds.max.x, x) - origin.x;
            float bocalY = Mathf.Lerp(bounds.min.y, bounds.max.y, 200f / 216f) - origin.y;
            bocaisDosPostes.Add(new Vector4(bocalX, bocalY, at.y, 1f));
        }
        return new Vector4(at.x + xOffset, at.y + yOffset, height, reach);
    }

    private void Plant(string name, string art, float x, float y, float scale, bool espelhar = false, bool temTroncoSolido = true)
    {
        var tree = transform.Find(name);
        if (tree == null)
        {
            var obj = new GameObject(name, typeof(SpriteRenderer));
            tree = obj.transform;
            tree.SetParent(transform, false);
            tree.position = new Vector3(x, y, 0);
            obj.GetComponent<SpriteRenderer>().sortingOrder = 1000 - Mathf.RoundToInt(y * 10);
            if (temTroncoSolido)
            {
                var collider = obj.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(.62f, .43f) * scale;
                collider.offset = new Vector2(0, .23f) * scale;
                obj.AddComponent<SantaRitaForeground>();
            }
        }
        DressTree(name, art, scale, espelhar);
    }

    private void DressTree(string name, string art, float scale, bool espelhar = false)
    {
        string especie = art == "Arvore" ? "Camboata" : art;
        string caminho = "Vegetacao/" + especie;
        bool novaArvore = Resources.Load<Sprite>(Art + caminho) != null;
        var renderer = DressProp(name, novaArvore ? caminho : art, new Vector3(.53f, .63f, 2.85f) * scale, scale,
            sombra: novaArvore ? Sombra.Nenhuma : Sombra.Projetada);
        if (renderer == null) return;
        if (espelhar) renderer.flipX = true;
        renderer.color = new Color(.87f, .94f, .93f);
        if (novaArvore && AplicarMaterialDaMata(renderer, especie, .092f * scale)) return;
        renderer.GetPropertyBlock(receiverProperties);
        receiverProperties.SetVector("_Wind", new Vector4(.092f * scale, renderer.bounds.size.y,
            renderer.transform.position.x * .79f, 0));
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
            var sprite = art.StartsWith("Entrada/", System.StringComparison.Ordinal) && art.IndexOf('/', "Entrada/".Length) < 0
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
        int limite = AvenidaMolhada ? CasterLimit : 32;
        if (casterCount < limite - 1 && sombra != Sombra.Nenhuma)
        {
            id = casterCount++;
            casterMin[id] = new Vector4(at.x - volume.x * .5f, at.y + .04f, 0, (float)sombra);
            casterMax[id] = new Vector4(at.x + volume.x * .5f, at.y + volume.y, volume.z, 0);
        }
        renderer.sortingOrder = 1000 - Mathf.RoundToInt(prop.position.y * 10);
        renderer.sharedMaterial = lighting;
        SetReceiver(renderer, prop.position.y, heightFactor, id);
        AplicarRelevoDaRodovia(renderer);
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
        var fontes = new Vector4[bairro ? 9 : 5];
        for (int i = 0; i < Mathf.Min(bocaisDosPostes.Count, fontes.Length); i++)
            fontes[i] = bocaisDosPostes[i];
        if (frontFogMaterial != null)
        {
            frontFogMaterial.SetVectorArray("_EntranceLamps", fontes);
            frontFogMaterial.SetInteger("_EntranceLampCount", fontes.Length);
        }
        PrepararFlaresDosPostes(fontes);
        if (bairro || RuaDoBar) return;

        // Uma fonte de alcance curto junto à parada, à bomba ou à banca de frutas.
        Vector4 accent = entrada ? new Vector4(119.58f, 13.56f, 1.94f, 4.7f)
            : posto ? new Vector4(212.2f, 13.24f, 2.46f, 5.1f)
            : new Vector4(308.12f, 13.31f, 2.22f, 4.9f);
        accent.x -= origin.x;
        accent.y -= origin.y;
        lighting.SetVector("_AccentLamp", accent);
        lighting.SetVector("_AccentColor", entrada ? new Vector4(1.28f, 1.53f, 1.72f, 0)
            : posto ? new Vector4(2.00f, 1.47f, .89f, 0)
            : new Vector4(2.08f, 1.23f, .63f, 0));
    }

    private void DressFog()
    {
        if (backFog == null)
        {
            var shader = Resources.Load<Shader>(Art + "Nevoeiro");
            if (shader == null || !shader.isSupported) return;
            var texture = Texture2D.whiteTexture;
            fogSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                Vector2.zero, texture.width, 0, SpriteMeshType.FullRect);
            backFog = CreateFog("Relâmpago atrás da rua", shader, false);
            backFogMaterial = backFog.sharedMaterial;
            frontFog = CreateFog("Vagalumes e luz dispersa", shader, true);
            frontFogMaterial = frontFog.sharedMaterial;
            frontFogMaterial.SetVectorArray("_FireflyPatches", FaixasDeVagalumes());
        }
        backFog.sortingOrder = 2030;
        if (frontFog != null) frontFog.sortingOrder = 2031;
        backFog.sharedMaterial = backFogMaterial;
        if (frontFog != null) frontFog.sharedMaterial = frontFogMaterial;
    }

    private Vector4[] FaixasDeVagalumes()
    {
        // Retângulos em coordenadas locais, seguindo as bordas do bosque e as moitas.
        if (entrada) return new[] { new Vector4(2, 15, 16, 19), new Vector4(21, 15, 35, 19),
            new Vector4(1, 3, 18, 5.9f), new Vector4(20, 3, 35, 5.7f) };
        if (bairro) return new[] { new Vector4(2, 3.2f, 17, 7.4f), new Vector4(23, 3.4f, 40, 7.4f),
            new Vector4(21.8f, 16.2f, 33, 19.4f), new Vector4(27.3f, 28.6f, 33, 32) };
        if (RuaDoBar) return new[] { new Vector4(1, 15.1f, 10, 20), new Vector4(26, 15.5f, 37, 20.7f),
            new Vector4(2, 3, 19, 5.8f), new Vector4(22, 3, 36, 5.8f) };
        if (posto) return new[] { new Vector4(1, 15.5f, 8, 20.6f), new Vector4(16, 15.5f, 24, 21),
            new Vector4(27, 18, 37, 21), new Vector4(2, 3.2f, 36, 5.8f) };
        return new[] { new Vector4(1, 17.7f, 16, 24.8f), new Vector4(15.8f, 16, 19.6f, 25),
            new Vector4(22, 16.3f, 27, 25), new Vector4(2, 3, 38, 5.7f) };
    }

    private SpriteRenderer CreateFog(string name, Shader shader, bool front)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(transform, false);
        obj.transform.position = origin;
        obj.transform.localScale = new Vector3(size.x, size.y, 1);
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = fogSprite;
        var material = new Material(shader);
        material.SetVector("_Ground", new Vector4(origin.x, origin.y, size.x, size.y));
        material.SetFloat("_Front", front ? 1 : 0);
        material.SetFloat("_Entrance", 1);
        renderer.sharedMaterial = material;
        return renderer;
    }

    private void LateUpdate()
    {
        if (lighting == null || receiverProperties == null) return;
        bool effects = ZonaAtiva && !MenuPausa.ReduzirEfeitosIntensos;
        if (flaresDosPostes != null && flaresDosPostes.activeSelf != effects)
            flaresDosPostes.SetActive(effects);
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
            lighting.SetVector("_PlayerLamp", Vector4.zero);
            DesativarSombraDaLanterna();
            inicioRaio = float.NegativeInfinity;
            lighting.SetFloat("_Lightning", 0);
            lighting.SetVector("_MirrorBounds", Vector4.zero);
            return;
        }
        if (luzesDeRecorte != null) ProjetarSombrasDaRua();
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
        if (effects) AtualizarFlaresDosPostes(focus);
        lighting.SetVector("_ViewFocus", new Vector4(focus.x, focus.y + .7f, effects ? 1f : .35f, 0));
        SetReceiver(player, player.transform.position.y, .94f, casterCount);
        UpdateCasters(true);
        AtualizarClimaDaRua(effects);
        AtualizarReflexoDoMarcus();
        if (playerController.ObterFeixeLanterna(out var lente, out var direcao))
        {
            Vector2 ponto = lente - origin;
            float alturaLuz = Mathf.Max(1.05f, lente.y - player.transform.position.y + .42f);
            var lamp = new Vector4(ponto.x, ponto.y, alturaLuz, 6.2f);
            lighting.SetVector("_PlayerLamp", lamp);
            lighting.SetVector("_PlayerLampDirection", direcao);
            ProjetarSombraDaLanterna(lamp);
        }
        else
        {
            lighting.SetVector("_PlayerLamp", Vector4.zero);
            DesativarSombraDaLanterna();
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
        lighting.SetInt("_ArrivalPlayerCaster", withPlayer ? casterCount : -1);
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

    private void PrepararFlaresDosPostes(Vector4[] fontes)
    {
        perfilDosFlares = ScriptableObject.CreateInstance<LensFlareDataSRP>();
        perfilDosFlares.elements = new[]
        {
            new LensFlareDataElementSRP
            {
                tint = new Color(1f, .61f, .24f),
                sizeXY = new Vector2(1f, .12f),
                uniformScale = 1.06f,
                localIntensity = .23f,
                edgeOffset = .72f,
                fallOff = .79f
            },
            new LensFlareDataElementSRP
            {
                tint = new Color(1f, .70f, .32f),
                sizeXY = new Vector2(.14f, 1f),
                uniformScale = .44f,
                rotation = 22f,
                localIntensity = .12f,
                edgeOffset = .72f,
                fallOff = .79f
            },
            new LensFlareDataElementSRP
            {
                position = .48f,
                flareType = SRPLensFlareType.Polygon,
                sideCount = 6,
                sdfRoundness = .38f,
                tint = new Color(1f, .64f, .34f),
                uniformScale = .25f,
                localIntensity = .095f,
                edgeOffset = .58f,
                fallOff = .78f
            },
            new LensFlareDataElementSRP
            {
                position = .74f,
                flareType = SRPLensFlareType.Polygon,
                sideCount = 6,
                sdfRoundness = .5f,
                tint = new Color(1f, .56f, .26f),
                uniformScale = .39f,
                localIntensity = .065f,
                edgeOffset = .76f,
                fallOff = .78f
            }
        };

        flaresDosPostes = new GameObject("Reflexos na lente dos postes");
        flaresDosPostes.transform.SetParent(transform, false);
        flaresDosPostes.SetActive(false);
        foreach (var fonte in fontes)
        {
            if (fonte.w <= 0) continue;
            var emissor = new GameObject("Lens flare");
            emissor.transform.SetParent(flaresDosPostes.transform, false);
            emissor.transform.position = origin + new Vector2(fonte.x, fonte.y);
            var flare = emissor.AddComponent<LensFlareComponentSRP>();
            flare.lensFlareData = perfilDosFlares;
            flare.attenuationByLightShape = false;
            flare.scaleByDistanceCurve = AnimationCurve.Constant(0f, 1f, 1f);
            flare.radialScreenAttenuationCurve = AnimationCurve.EaseInOut(.5f, 1f, 1f, 0f);
            flare.intensity = 0f;
            flaresDaRua.Add((flare, fonte));
        }
    }

    private void AtualizarFlaresDosPostes(Vector2 posicao)
    {
        foreach (var (flare, fonte) in flaresDaRua)
        {
            float lateral = Mathf.Abs(posicao.x - fonte.x);
            float abaixo = fonte.z - posicao.y;
            float entrada = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(-.12f, .42f, abaixo));
            float saida = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(1.45f, 2.65f, abaixo));
            float largura = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(.30f, 1.55f, lateral));
            flare.intensity = entrada * saida * largura;
        }
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
        if (sombrasDosPostes != null)
        {
            sombrasDosPostes.Release();
            Destroy(sombrasDosPostes);
        }
        LiberarRecursosSombraLanterna();
        foreach (var sprite in spritesDaFiacao)
        {
            Destroy(sprite.texture);
            Destroy(sprite);
        }
        Destroy(fogSprite);
        Destroy(backFogMaterial);
        Destroy(frontFogMaterial);
        Destroy(perfilDosFlares);
        Destroy(spriteDasLuminariasDaSubida);
        Destroy(lighting);
    }
}

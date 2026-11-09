using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

// Bancada visual da Rua Norte. As coordenadas e interações pertencem ao Capitulo1Director.
[DefaultExecutionOrder(150)]
public sealed class SantaRitaRuaNorte : MonoBehaviour
{
    private const string Art = "Art/RuaNorte/";
    private const int TileSize = 64;
    private const int CasterLimit = 32;
    private enum Ground { Grass, Road, Walk, Dirt, Verge, Curb, DirtRoad, Crack, Puddle, Stripe, Drain, SideVerge, DirtVerge }

    private enum Sombra { Projetada, Contato, Nenhuma, Veiculo }

    private readonly Dictionary<int, Tile> tiles = new();
    private readonly Vector4[] casterMin = new Vector4[CasterLimit];
    private readonly Vector4[] casterMax = new Vector4[CasterLimit];
    private MaterialPropertyBlock receiverProperties;
    private Material lighting;
    private Texture2D groundCells;
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

    private bool ZonaAtiva => entrada ? EntradaAtiva : Active;

    public static bool BuildGround(Transform zone, Vector2 min, int width, int height, bool entrada = false)
    {
        var atlas = Resources.Load<Texture2D>(Art + "Terreno");
        var shader = Resources.Load<Shader>(Art + "LuzVolumetrica");
        if (atlas == null || shader == null || !shader.isSupported) return false;

        var look = zone.gameObject.AddComponent<SantaRitaRuaNorte>();
        look.receiverProperties = new MaterialPropertyBlock();
        look.origin = min;
        look.size = new Vector2(width, height);
        look.entrada = entrada;
        if (entrada) entradaAtual = look;
        look.lighting = new Material(shader) { name = entrada ? "Luz local da entrada" : "Luz local da Rua Norte" };
        if (entrada) look.lighting.SetFloat("_DoorReflection", 0);
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
        if (look != null) look.Dress();
        var entrada = world.Find("Zona1_Rodovia")?.GetComponent<SantaRitaRuaNorte>();
        if (entrada != null) entrada.DressEntrance();
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
                if (entrada) SelectEntranceGround(x, y, out kind, out flipX, out flipY);
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
        kind = y != 10 && wear < 2 ? Ground.Puddle : (wear < 5 ? Ground.Crack : Ground.Road);
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
        DressFog();
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

        lighting.SetVectorArray("_Lamps", new[]
        {
            Lamp("Poste_Z1_1", .66f, -.42f, 3.18f, 8.2f),
            Lamp("Poste_Z1_2", .66f, -.42f, 3.18f, 8.2f),
            Lamp("Poste_Z1_3", .66f, -.42f, 3.18f, 8.6f)
        });
        lighting.SetVectorArray("_LampColors", new[]
        {
            new Vector4(1.89f, 1.48f, .97f, 1),
            new Vector4(2.10f, 1.64f, 1.01f, 1),
            new Vector4(1.70f, 1.40f, .92f, 1)
        });
        UpdateCasters(false);
        DressFog();
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
            sombra: entrada ? Sombra.Nenhuma : Sombra.Projetada);
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
        if (entrada) renderer.sortingOrder = 1000 - Mathf.RoundToInt(prop.position.y * 10);
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

    private void DressFog()
    {
        if (backFog == null)
        {
            var shader = Resources.Load<Shader>(Art + "Nevoeiro");
            if (shader == null || !shader.isSupported) return;
            var house = new Vector4(origin.x + size.x * .5f, 15.8f, 0f, 0f);
            if (!entrada)
            {
                var facade = transform.Find("Predio_Fachada_BarDoZe").GetComponent<SpriteRenderer>();
                var baseAt = facade.transform.position;
                house = new Vector4(baseAt.x, baseAt.y, facade.bounds.extents.x, 3.1f);
            }
            var texture = Texture2D.whiteTexture;
            fogSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                Vector2.zero, texture.width, 0, SpriteMeshType.FullRect);
            backFog = CreateFog("Nevoeiro sobre o bosque", shader, house, false);
            backFogMaterial = backFog.sharedMaterial;
            if (!entrada)
            {
                frontFog = CreateFog("Nevoeiro junto à soleira", shader, house, true);
                frontFogMaterial = frontFog.sharedMaterial;
            }
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
        renderer.sharedMaterial = material;
        return renderer;
    }

    private void LateUpdate()
    {
        if (lighting == null || receiverProperties == null) return;
        bool effects = ZonaAtiva && !MenuPausa.ReduzirEfeitosIntensos;
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
            return;
        }
        if (player == null)
        {
            var actor = FindAnyObjectByType<PlayerController>();
            if (actor == null || !actor.TryGetComponent(out player)) return;
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
        if (entrada)
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
        Destroy(fogSprite);
        Destroy(backFogMaterial);
        Destroy(frontFogMaterial);
        Destroy(lighting);
    }
}

using UnityEngine;
using UnityEngine.Rendering.Universal;

// The lights are placed on the exploration plane. The visible fixtures sit higher in the sprite composition.
internal sealed class SantaRitaBarLighting : MonoBehaviour
{
    private static readonly Color Amber = new Color(1f, .78f, .53f);
    private static readonly Color Iron = new Color(.22f, .24f, .21f);
    private static readonly Color Wood = new Color(.36f, .23f, .16f);
    private static readonly Color Brass = new Color(.72f, .47f, .23f);
    private Texture2D pixel;
    private Sprite block, shadow;
    private Material lit;
    private PlayerController player;
    private SpriteRenderer playerShadow, playerContact;

    internal void Build(Rect floor, Material material)
    {
        lit = material;
        shadow = Resources.Load<Sprite>("Art/Acessos/SombraContato");
        pixel = new Texture2D(1, 1, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        pixel.SetPixel(0, 0, Color.white);
        pixel.Apply(false, true);
        block = Sprite.Create(pixel, new Rect(0, 0, 1, 1), Vector2.one * .5f, 32f);

        // Rails and dark edges frame the lambril without taking space out of the aisles.
        Strip("VigaDoFundo", new Vector2(floor.center.x, floor.yMax + 1.94f), new Vector2(floor.width + .65f, .19f), Wood);
        Strip("FrisoDoFundo", new Vector2(floor.center.x, floor.yMax + .94f), new Vector2(floor.width, .09f), Wood);
        Strip("RodapeDoFundo", new Vector2(floor.center.x, floor.yMax + .04f), new Vector2(floor.width, .12f), Iron);
        Strip("RodapeEsquerdo", new Vector2(floor.xMin + .04f, floor.center.y), new Vector2(.10f, floor.height), Wood);
        Strip("RodapeDireito", new Vector2(floor.xMax - .04f, floor.center.y), new Vector2(.10f, floor.height), Wood);
        Strip("OclusaoDoFundo", new Vector2(floor.center.x, floor.yMax - .11f), new Vector2(floor.width, .22f), new Color(.14f, .10f, .11f, .44f));
        foreach (float x in new[] { floor.xMin, floor.xMax })
            Strip("PilarDeCanto", new Vector2(x, floor.yMax + 1f), new Vector2(.22f, 2f), Wood);

        var table = SantaRitaBarLayout.TableCenter;
        var pendant = table + new Vector2(0, 2.35f);
        Strip("FioDoPendente", pendant + Vector2.up * .30f, new Vector2(.0625f, .53f), Iron);
        Strip("CupulaVerde", pendant, new Vector2(.72f, .16f), new Color(.17f, .31f, .19f));
        Strip("TopoDaCupula", pendant + Vector2.up * .12f, new Vector2(.44f, .12f), new Color(.24f, .38f, .21f));
        Strip("LampadaDaSinuca", pendant + Vector2.down * .11f, new Vector2(.44f, .07f), new Color(1f, .86f, .56f));
        Point("LuzDaSinuca", table + Vector2.up * .14f, 3.6f, 1.04f, Amber);

        var counter = new Vector2(6.35f, 11.51f);
        Strip("LuminariaDoBalcao", counter, new Vector2(1.35f, .15f), Iron);
        Strip("DifusorDoBalcao", counter + Vector2.down * .09f, new Vector2(1.14f, .06f), Brass);
        Point("LuzDoBalcao", counter + Vector2.down * 1.55f, 3.7f, .76f, Amber);
        Sconce(new Vector2(floor.xMin + .18f, 6.25f), 1f);
        Sconce(new Vector2(floor.xMax - .18f, 3.85f), -1f);
        Point("ReflexoFrioDaEntrada", new Vector2(9.9f, 1.65f), 4.5f, .24f, new Color(.54f, .66f, .83f));

        if (shadow == null) return;
        playerShadow = SpriteAt("SombraProjetadaMarcus", shadow, Vector2.zero, Vector2.one, -4, Color.white);
        playerContact = SpriteAt("SombraContatoMarcus", shadow, Vector2.zero, Vector2.one, -3, Color.white);
        playerShadow.enabled = playerContact.enabled = false;
    }

    internal void AddShadow(Transform subject, float width, float depth, float length, bool moving)
    {
        if (shadow == null) return;
        var cast = SpriteAt("SombraProjetada_" + subject.name, shadow, Vector2.zero, Vector2.one, -4, Color.white);
        var contact = SpriteAt("SombraContato_" + subject.name, shadow, Vector2.zero, Vector2.one, -3, Color.white);
        if (moving)
        {
            cast.transform.SetParent(subject, true);
            contact.transform.SetParent(subject, true);
        }
        PlaceShadow(cast, contact, subject.position, width, depth, length);
    }

    private void LateUpdate()
    {
        if (playerShadow == null) return;
        if (player == null) player = FindAnyObjectByType<PlayerController>();
        bool inside = player != null && player.isActiveAndEnabled &&
            SantaRitaBarLayout.ExplorationFloor.Contains((Vector2)player.transform.position - SantaRitaBarLayout.Origin);
        playerShadow.enabled = playerContact.enabled = inside;
        if (inside) PlaceShadow(playerShadow, playerContact, player.transform.position, .57f, .22f, .74f);
    }

    private static void PlaceShadow(SpriteRenderer cast, SpriteRenderer contact, Vector2 feet, float width, float depth, float length)
    {
        var source = SantaRitaBarLayout.Origin + SantaRitaBarLayout.TableCenter + new Vector2(-.32f, 1.25f);
        Vector2 away = feet - source;
        if (away.sqrMagnitude < .04f) away = Vector2.down;
        away.Normalize();
        cast.transform.position = feet + away * (length * .23f);
        cast.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(away.y, away.x) * Mathf.Rad2Deg);
        Resize(cast, new Vector2(length, width));
        cast.color = new Color(.27f, .23f, .26f, .42f);
        contact.transform.position = feet;
        Resize(contact, new Vector2(width * 1.17f, depth));
        contact.color = new Color(.19f, .16f, .16f, .62f);
    }

    private void Sconce(Vector2 at, float inward)
    {
        Strip("SuporteDaArandela", at, new Vector2(.22f, .41f), Iron);
        Strip("VidroDaArandela", at + Vector2.right * (.05f * inward), new Vector2(.14f, .25f), Brass);
        Strip("ChapeuDaArandela", at + Vector2.up * .19f, new Vector2(.31f, .07f), Iron);
        Point("LuzDeArandela", at + new Vector2(.58f * inward, -.42f), 3.1f, .57f, Amber);
    }

    private void Point(string label, Vector2 at, float radius, float intensity, Color color)
    {
        var go = new GameObject(label);
        go.transform.SetParent(transform, false);
        go.transform.position = SantaRitaBarLayout.Origin + at;
        var light = go.AddComponent<Light2D>();
        light.lightType = Light2D.LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.pointLightInnerRadius = radius * .16f;
        light.pointLightOuterRadius = radius;
        light.falloffIntensity = .65f;
    }

    private void Strip(string label, Vector2 at, Vector2 size, Color color) => SpriteAt(label, block, at, size, -1, color);

    private SpriteRenderer SpriteAt(string label, Sprite sprite, Vector2 at, Vector2 size, int order, Color color)
    {
        var go = new GameObject(label);
        go.transform.SetParent(transform, false);
        go.transform.position = SantaRitaBarLayout.Origin + at;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sharedMaterial = lit;
        sr.sortingOrder = order;
        sr.color = color;
        Resize(sr, size);
        return sr;
    }

    private static void Resize(SpriteRenderer sr, Vector2 size)
    {
        Vector3 native = sr.sprite.bounds.size;
        sr.transform.localScale = new Vector3(size.x / native.x, size.y / native.y, 1);
    }

    private void OnDestroy()
    {
        Destroy(block);
        Destroy(pixel);
    }
}

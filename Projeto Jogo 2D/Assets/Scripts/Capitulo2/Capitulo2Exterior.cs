using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

internal sealed class Capitulo2Exterior : MonoBehaviour
{
    private const string Art = "Art/Cap2/Remodelagem/";
    private static readonly HashSet<string> Fachadas = new()
    {
        "Igreja", "Prefeitura", "Rodoviaria", "Fundos_MesmoMercadinho", "OficinaRadio",
        "CasaCostura", "Sociedade", "Abrigo_Cemiterio"
    };
    private readonly List<SpriteRenderer> clima = new();
    private readonly List<UnityEngine.Object> recursos = new();
    private readonly List<Vector2> fontes = new();
    private Material lit, shadow;
    private string pasta;

    internal void Montar(bool cemiterio, Vector2 min, Vector2 max)
    {
        lit = Resources.Load<Material>("Art/WorldLit");
        if (lit == null) return;
        pasta = Art + (cemiterio ? "Cemiterio/" : "Centro/");
        var existentes = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sprite in existentes)
        {
            if (sprite.gameObject.scene != gameObject.scene || sprite.sprite == null) continue;
            if (sprite.name == "Poste") Trocar(sprite, "Art/RuaNorte/Poste", 1.12f);
            if (sprite.name.StartsWith("Arvore", StringComparison.Ordinal))
            {
                string[] especies = { "Pitangueira", "Camboata", "Bracatinga", "Araucaria" };
                Vector2 at = sprite.transform.position;
                int variation = Mathf.Abs(Mathf.RoundToInt(at.x * 19 + at.y * 37)) % especies.Length;
                if (Trocar(sprite, "Art/RuaNorte/Vegetacao/" + especies[variation], Mathf.Clamp(sprite.bounds.size.x, 2.7f, 5.3f)))
                    sprite.color = Color.Lerp(Color.white, new Color(.68f, .75f, .76f), .36f + variation * .11f);
            }
            if (!cemiterio && sprite.name.StartsWith("BancoPraca_", StringComparison.Ordinal))
                Trocar(sprite, pasta + "Banco", 2f);
            if (!cemiterio && sprite.name == "QuadroAvisosDaPraca")
                Trocar(sprite, pasta + "Quadro", 1.63f);
            if (cemiterio && sprite.name.StartsWith("Sepultura_", StringComparison.Ordinal))
            {
                Vector2 at = sprite.transform.position;
                int variation = Mathf.Abs(Mathf.RoundToInt(at.x + at.y * 2)) % 5;
                Trocar(sprite, pasta + (variation == 1 ? "Cruz" : "Lapide"), variation == 1 ? .68f : 1.17f);
            }
        }

        foreach (var sprite in existentes)
        {
            if (sprite == null || sprite.gameObject.scene != gameObject.scene || sprite.sprite == null) continue;
            if (sprite.name == "Poste") fontes.Add(new Vector2(sprite.bounds.max.x - .18f, sprite.bounds.max.y - .22f));
            if (Fachadas.Contains(sprite.name))
            {
                Vector2 at = sprite.transform.position;
                // A fonte fica acima da soleira; o gatilho e a fachada conservam seus IDs.
                var fixture = new GameObject("Luz de soleira · " + sprite.name, typeof(Light2D));
                fixture.transform.SetParent(transform, false);
                fixture.transform.position = at + new Vector2(0, .45f);
                var light = fixture.GetComponent<Light2D>();
                light.lightType = Light2D.LightType.Point;
                light.color = new Color(1, .72f, .43f);
                light.intensity = .59f;
                light.pointLightInnerRadius = .55f;
                light.pointLightOuterRadius = 4.2f;
            }
        }

        if (cemiterio)
        {
            Prop("Velas junto ao abrigo", "Velas", new Vector2(76.20f, 100.24f));
            Prop("Vaso no acesso ao jazigo", "Vaso", new Vector2(62.18f, 98.94f));
            Prop("Vaso junto à ala leste", "Vaso", new Vector2(74.85f, 89.12f));
            Prop("Mureta da ala oeste", "Pedra", new Vector2(58.6f, 85.46f));
            Prop("Mureta da ala leste", "Pedra", new Vector2(69.65f, 85.46f));
            Prop("Grade sobre o muro oeste", "Grade", new Vector2(59.7f, 84.08f));
            Prop("Grade sobre o muro leste", "Grade", new Vector2(68.4f, 84.08f));
            Submata(new Vector2(50.75f, 88.9f), new Vector2(50.1f, 95.6f), new Vector2(56.8f, 103.1f),
                new Vector2(71.7f, 103.8f), new Vector2(81.15f, 100.1f), new Vector2(81.1f, 91.2f));
        }
        else
        {
            foreach (float y in new[] { 43f, 50f, 57f })
            {
                Prop("Floreira da praça", "Floreira", new Vector2(35.7f, y - 1.45f));
                Prop("Floreira da praça", "Floreira", new Vector2(55.25f, y - 1.45f));
            }
            Prop("Canteiro do acesso à Prefeitura", "Canteiro", new Vector2(85.05f, 65.63f));
            Prop("Canteiro do acesso à Prefeitura", "Canteiro", new Vector2(90.92f, 65.63f));
            Prop("Canteiro do acesso à Rodoviária", "Canteiro", new Vector2(71.66f, 21.54f));
            foreach (var at in new[] { new Vector2(40.9f, 63.14f), new Vector2(48.6f, 63.14f),
                new Vector2(66.08f, 20.83f), new Vector2(69.99f, 20.83f) })
                Prop("Balizador da soleira", "Balizador", at);
            foreach (var at in new[] { new Vector2(40.90f, 60.55f), new Vector2(56.72f, 38.85f),
                new Vector2(71.78f, 30.76f), new Vector2(26.65f, 21.33f), new Vector2(28.2f, 40.91f) })
                Prop("Ralo de sarjeta", "Ralo", at, plano: true);
            Submata(new Vector2(22.6f, 80.45f), new Vector2(20.56f, 83.28f), new Vector2(30.05f, 85.2f),
                new Vector2(40.72f, 69.31f), new Vector2(51.27f, 67.38f), new Vector2(9.1f, 12.9f));
        }

        var projection = Resources.Load<Shader>("Art/Cap2/Remodelagem/PisoMolhadoC2");
        if (projection != null)
        {
            shadow = new Material(projection);
            recursos.Add(shadow);
            foreach (var sprite in existentes)
            {
                if (sprite == null || sprite.gameObject.scene != gameObject.scene || sprite.sprite == null) continue;
                if (Fachadas.Contains(sprite.name) || sprite.name.StartsWith("Sepultura_") ||
                    sprite.name.StartsWith("BancoPraca_") || sprite.name.StartsWith("Arvore") ||
                    sprite.name == "Poste") Projetar(sprite);
            }
            foreach (var p in fontes) Reflexo(p - new Vector2(.02f, 3.12f));
        }
        Nevoa(min, max);
    }

    private bool Trocar(SpriteRenderer target, string caminho, float largura)
    {
        var sprite = Resources.Load<Sprite>(caminho);
        if (sprite == null) return false;
        target.sprite = sprite;
        // Escala só o desenho. A árvore e o banco mantêm a pegada original.
        var box = target.GetComponent<BoxCollider2D>();
        Vector3 old = target.transform.localScale;
        var scale = largura / Mathf.Max(.01f, sprite.bounds.size.x);
        target.transform.localScale = new Vector3(scale, scale, old.z);
        if (box != null)
        {
            var ratio = new Vector2(old.x / scale, old.y / scale);
            box.size = Vector2.Scale(box.size, ratio);
            box.offset = Vector2.Scale(box.offset, ratio);
        }
        target.sharedMaterial = lit;
        return true;
    }

    private SpriteRenderer Prop(string nome, string arte, Vector2 at, bool plano = false)
    {
        var sprite = Resources.Load<Sprite>(arte.StartsWith("Art/", StringComparison.Ordinal) ? arte : pasta + arte);
        if (sprite == null) return null;
        var obj = new GameObject(nome, typeof(SpriteRenderer));
        obj.transform.SetParent(transform, false);
        obj.transform.position = at;
        var renderer = obj.GetComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sharedMaterial = lit;
        renderer.sortingOrder = plano ? -1700 : 1000 - Mathf.RoundToInt(at.y * 10);
        return renderer;
    }

    private void Submata(params Vector2[] positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            var bush = Prop("Submata de margem", "Art/RuaNorte/Vegetacao/" +
                (i % 2 == 0 ? "MoitaDeBorda" : "SamambaiaDaMata"), positions[i]);
            if (bush != null) bush.color = new Color(.76f, .82f, .80f);
        }
    }

    private void Projetar(SpriteRenderer sprite)
    {
        if (fontes.Count == 0 || !sprite.enabled) return;
        Vector2 at = sprite.transform.position;
        Vector2 direction = Vector2.zero;
        float nearest = 10f;
        foreach (var source in fontes)
        {
            Vector2 delta = at - (source - Vector2.up * 2.5f);
            float distance = delta.magnitude;
            if (distance >= nearest) continue;
            nearest = distance;
            direction = delta.normalized;
        }
        if (nearest >= 10f) return;
        var pts = sprite.sprite.vertices;
        var vertices = new Vector3[pts.Length];
        Vector3 scale = sprite.transform.lossyScale;
        float yMin = sprite.sprite.bounds.min.y;
        for (int i = 0; i < pts.Length; i++)
        {
            float y = (pts[i].y - yMin) * scale.y;
            vertices[i] = new Vector3(pts[i].x * scale.x * (sprite.flipX ? -1 : 1) + y * direction.x * .54f,
                y * (direction.y * .48f + .12f), 0);
        }
        var mesh = new Mesh { name = "Silhueta " + sprite.name, vertices = vertices, uv = sprite.sprite.uv };
        mesh.triangles = Array.ConvertAll(sprite.sprite.triangles, n => (int)n);
        recursos.Add(mesh);
        var obj = new GameObject("Sombra · " + sprite.name, typeof(MeshFilter), typeof(MeshRenderer));
        obj.transform.SetParent(transform, false);
        obj.transform.position = at;
        obj.GetComponent<MeshFilter>().sharedMesh = mesh;
        var renderer = obj.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = shadow;
        renderer.sortingOrder = -1600;
        var props = new MaterialPropertyBlock();
        props.SetTexture("_MainTex", sprite.sprite.texture);
        props.SetColor("_Tint", new Color(.030f, .043f, .051f, Mathf.Lerp(.26f, .04f, nearest / 10f)));
        renderer.SetPropertyBlock(props);
    }

    private void Reflexo(Vector2 at)
    {
        var obj = new GameObject("Brilho de piso molhado", typeof(SpriteRenderer));
        obj.transform.SetParent(transform, false);
        obj.transform.position = at;
        obj.transform.localScale = new Vector3(1.95f, 3.20f, 1);
        var renderer = obj.GetComponent<SpriteRenderer>();
        renderer.sprite = Quad();
        renderer.sortingOrder = -1580;
        renderer.sharedMaterial = shadow;
        var props = new MaterialPropertyBlock();
        props.SetFloat("_Wet", 1);
        props.SetColor("_Tint", new Color(.90f, .60f, .32f, .26f));
        renderer.SetPropertyBlock(props);
        clima.Add(renderer);
    }

    private Sprite quad;
    private Sprite Quad()
    {
        if (quad != null) return quad;
        var texture = Texture2D.whiteTexture;
        quad = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), texture.width);
        recursos.Add(quad);
        return quad;
    }

    private void Nevoa(Vector2 min, Vector2 max)
    {
        var shader = Resources.Load<Shader>("Art/RuaNorte/Nevoeiro");
        if (shader == null) return;
        Vector2 extent = max - min;
        for (int i = 0; i < 2; i++)
        {
            var obj = new GameObject(i == 0 ? "Névoa de fundo" : "Dispersão nos postes", typeof(SpriteRenderer));
            obj.transform.SetParent(transform, false);
            obj.transform.position = (min + max) * .5f;
            obj.transform.localScale = new Vector3(extent.x, extent.y, 1);
            var renderer = obj.GetComponent<SpriteRenderer>();
            renderer.sprite = Quad();
            renderer.sortingOrder = 2030 + i;
            var material = new Material(shader);
            recursos.Add(material);
            material.SetVector("_Ground", new Vector4(min.x, min.y, extent.x, extent.y));
            material.SetFloat("_Entrance", 1);
            material.SetFloat("_Front", i);
            material.SetFloat("_BeamStrength", .54f);
            var lamps = new Vector4[9];
            for (int j = 0; j < Mathf.Min(9, fontes.Count); j++)
            {
                Vector2 p = fontes[j] - min;
                lamps[j] = new Vector4(p.x, p.y, p.y - 2.5f, 1);
            }
            material.SetVectorArray("_EntranceLamps", lamps);
            material.SetInteger("_EntranceLampCount", Mathf.Min(9, fontes.Count));
            renderer.sharedMaterial = material;
            clima.Add(renderer);
        }
    }

    private void LateUpdate()
    {
        bool enabled = !MenuPausa.ReduzirEfeitosIntensos;
        foreach (var renderer in clima) renderer.enabled = enabled;
    }

    private void OnDestroy()
    {
        foreach (var item in recursos) Destroy(item);
    }
}


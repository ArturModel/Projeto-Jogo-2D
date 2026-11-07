using UnityEngine;
using UnityEngine.Rendering.Universal;

internal static class SantaRitaBochaAmbiente
{
    internal static readonly Vector3 PendenteDaEntrada = new Vector3(9.4f, 13.1f, 0);
    private static readonly Color LuzQuente = new Color(1f, .79f, .54f);

    internal static void Aplicar()
    {
        var sala = GameObject.Find("SalaCanchaFechada");
        if (sala == null || sala.transform.Find("AmbienteCancha") != null) return;

        var ambiente = new GameObject("AmbienteCancha").transform;
        ambiente.SetParent(sala.transform, false);
        var lit = Resources.Load<Material>("Art/WorldLit");
        var global = GameObject.Find("Global Light 2D");
        if (global != null && global.TryGetComponent<Light2D>(out var luzGlobal))
        {
            luzGlobal.intensity = .41f;
            luzGlobal.color = new Color(.79f, .86f, .96f);
        }

        // Lower the old even fill over the track. Leave the hall and stairs on their own circuit.
        foreach (var luz in Object.FindObjectsByType<Light2D>())
            if (luz.name == "LuminariaDeUso" && luz.transform.position.y > 12f)
                luz.intensity *= .25f;

        // One sequence of pendants along the play lane, with cooler light in the waiting area.
        foreach (float x in new[] { PendenteDaEntrada.x, 16.5f, 23.9f })
        {
            SpriteNoMundo(ambiente, "PendenteDaCancha", "Bocha/PendenteSociedade", new Vector2(x, PendenteDaEntrada.y + .45f), 1.50f, 862, lit);
            Ponto(ambiente, "LuzDoPendente", new Vector2(x, 12.1f), 4.45f, x == PendenteDaEntrada.x ? .91f : .79f, LuzQuente);
        }

        SpriteNoMundo(ambiente, "PlacarDeGiz", "Bocha/PlacarSociedade", new Vector2(6.3f, 16.88f), 1.67f, 847, lit);
        SpriteNoMundo(ambiente, "MuralDeAvisos", "Bocha/MuralSociedade", new Vector2(17.7f, 17.27f), 2.18f, 846, lit);
        Ponto(ambiente, "LuzDoMaterial", new Vector2(4.2f, 14.8f), 3.2f, .63f, LuzQuente);
        Ponto(ambiente, "LuzDoMural", new Vector2(17.6f, 16.8f), 2.9f, .43f, LuzQuente);
        Ponto(ambiente, "ReflexoDasJanelas", new Vector2(26.6f, 16.7f), 4.3f, .32f, new Color(.52f, .70f, 1f));

        MontarSalaDeEspera(ambiente, lit);
        var sombra = Resources.Load<Sprite>("Art/Acessos/SombraContato");
        if (sombra == null) return;
        foreach (var sr in Object.FindObjectsByType<SpriteRenderer>())
        {
            if (sr.name != "BancoCanchaSul" && sr.name != "BancoCanchaNorte" && sr.name != "BancoDeEspera") continue;
            var pos = sr.transform.position;
            Sombra(ambiente, sombra, pos + new Vector3(.19f, -.24f), new Vector2(sr.bounds.size.x * .94f, .54f), .42f, lit);
            Sombra(ambiente, sombra, pos + Vector3.down * .08f, new Vector2(sr.bounds.size.x * .87f, .26f), .65f, lit);
        }
    }

    private static void MontarSalaDeEspera(Transform ambiente, Material lit)
    {
        // Take the visitors' desk out of the open floor and keep the notebook and interaction aligned.
        Vector3 mesaOffset = new Vector3(1.8f, 1.74f);
        foreach (string nome in new[] { "MesaDeVisitas", "CadernoDeVisitas", "Examinar_visitas_sociedade" })
            Mover(nome, mesaOffset);
        Vector3 armarioOffset = new Vector3(.4f, 1.9f);
        Mover("ArmarioDoMaterial", armarioOffset);

        foreach (float x in new[] { 11.8f, 20.4f })
        {
            const float y = 5.2f;
            var banco = SpriteNoMundo(ambiente, "BancoDeEspera", "Art/Props/BancoCancha", new Vector2(x, y), 2.8f, 948, lit);
            if (banco == null) continue;
            var col = banco.gameObject.AddComponent<BoxCollider2D>();
            col.size = new Vector2(2.45f, .34f) / banco.transform.localScale.x;
            col.offset = Vector2.up * (.20f / banco.transform.localScale.y);
        }
        SpriteNoMundo(ambiente, "MuralDoSalao", "Bocha/MuralSociedade", new Vector2(15.8f, 6.96f), 2.05f, 941, lit);
        SpriteNoMundo(ambiente, "FlamulaDoClube", "Bocha/FlamulaSociedade", new Vector2(20.4f, 6.91f), .98f, 941, lit);
        SpriteNoMundo(ambiente, "CaixaDoMaterial", "Art/Props/CaixasManutencao", new Vector2(26.1f, 5.15f), 1.03f, 949, lit);
        Ponto(ambiente, "LuzDaEspera", new Vector2(12f, 5.2f), 4.1f, .58f, LuzQuente);
        Ponto(ambiente, "LuzDoCaderno", new Vector2(15.7f, 5.45f), 3.8f, .70f, LuzQuente);
        Ponto(ambiente, "LuzDoArmario", new Vector2(23.3f, 5.4f), 4.25f, .46f, new Color(1f, .82f, .60f));
    }

    private static void Mover(string nome, Vector3 deslocamento)
    {
        var objeto = GameObject.Find(nome);
        if (objeto == null) return;
        objeto.transform.position += deslocamento;
        if (objeto.TryGetComponent<SpriteRenderer>(out var sprite))
            sprite.sortingOrder -= Mathf.RoundToInt(deslocamento.y * 10f);
    }

    private static SpriteRenderer SpriteNoMundo(Transform raiz, string nome, string recurso, Vector2 pos, float largura, int ordem, Material lit)
    {
        var sprite = Resources.Load<Sprite>(recurso);
        if (sprite == null) return null;
        var sr = CriarSprite(raiz, nome, pos, sprite, ordem, lit);
        sr.transform.localScale = Vector3.one * (largura / sprite.bounds.size.x);
        return sr;
    }

    private static void Sombra(Transform raiz, Sprite sprite, Vector2 pos, Vector2 tamanho, float alpha, Material lit)
    {
        var sr = CriarSprite(raiz, "SombraDoBanco", pos, sprite, -2085, lit);
        sr.transform.localScale = new Vector3(tamanho.x / sprite.bounds.size.x, tamanho.y / sprite.bounds.size.y, 1);
        sr.color = new Color(.21f, .18f, .16f, alpha);
    }

    private static SpriteRenderer CriarSprite(Transform raiz, string nome, Vector2 pos, Sprite sprite, int ordem, Material lit)
    {
        var go = new GameObject(nome);
        go.transform.SetParent(raiz, false);
        go.transform.position = pos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sharedMaterial = lit;
        sr.sortingOrder = ordem;
        return sr;
    }

    private static void Ponto(Transform raiz, string nome, Vector2 pos, float alcance, float intensidade, Color cor)
    {
        var go = new GameObject(nome);
        go.transform.SetParent(raiz, false);
        go.transform.position = pos;
        var luz = go.AddComponent<Light2D>();
        luz.lightType = Light2D.LightType.Point;
        luz.pointLightInnerRadius = alcance * .18f;
        luz.pointLightOuterRadius = alcance;
        luz.intensity = intensidade;
        luz.falloffIntensity = .65f;
        luz.color = cor;
    }
}

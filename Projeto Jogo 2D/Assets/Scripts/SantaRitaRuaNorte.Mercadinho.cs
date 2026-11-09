using UnityEngine;

public sealed partial class SantaRitaRuaNorte
{
    private const string ObjetosDoMercadinho = Art + "Mercadinho/";
    private const string ObjetosDaSubida = ObjetosDoMercadinho + "Subida/";
    private const string ObjetosDoBloqueio = ObjetosDoMercadinho + "Bloqueio/";

    private Sprite spriteDasLuminariasDaSubida;

    private void ComporEncruzilhada(SpriteRenderer fachada, SpriteRenderer carro)
    {
        lighting.SetFloat("_Arrival", 1);
        ConfigurarReflexoDoCarro(carro);
        ComporFrenteDoMercadinho(fachada);
        ComporSubidaDeTerra();
        ComporPasseioDaEncruzilhada();
        ComporBloqueioDaAvenida();
    }

    private void ComporFrenteDoMercadinho(SpriteRenderer fachada)
    {
        var banca = ModuloDaRodovia("Banca de frutas", "BancaDeFrutas", new Vector2(307.65f, 13.64f),
            volume: new Vector3(1.20f, .39f, .87f), pasta: ObjetosDoMercadinho);
        if (banca != null)
            LimiteDaRodovia(banca.transform, new Vector2(0, .19f), new Vector2(1.20f, .36f));
        ModuloDaRodovia("Cavalete do mercadinho", "CavaleteDoMercadinho", new Vector2(312.90f, 13.45f),
            volume: new Vector3(.47f, .29f, .86f), pasta: ObjetosDoMercadinho);
        ModuloDaRodovia("Jardineira do mercadinho", "JardineiraDaLoja", new Vector2(314.27f, 13.73f),
            volume: new Vector3(.92f, .32f, .70f), pasta: ObjetosDoMercadinho);
        ModuloDaRodovia("Canaleta da soleira", "CanaletaDaLoja", new Vector2(311f, 12.99f),
            plano: true, pasta: ObjetosDoMercadinho);
        if (fachada != null)
        {
            FixarDetalheDaRodovia(fachada, "Medidor lateral do mercadinho", "MedidorDaLoja",
                new Vector2(2.94f, 1.36f), ObjetosDoMercadinho);
            FixarDetalheDaRodovia(fachada, "Cano lateral do mercadinho", "CanoDeChuva", new Vector2(-2.98f, .10f));
        }
    }

    private void ComporSubidaDeTerra()
    {
        ModuloDaRodovia("Bueiro no pé da subida", "BueiroDaSubida", new Vector2(323.02f, 15.15f),
            volume: new Vector3(.94f, .38f, .60f), pasta: ObjetosDaSubida);
        InstalarBalizasDeTerra(new Vector2(318.14f, 17.12f), new Vector2(322.33f, 18.65f),
            new Vector2(317.88f, 21.58f), new Vector2(321.52f, 23.36f));

        DetalharBarro("Cascalho molhado", "CascalhoDaSubida", new (float x, float y, float escala)[]
        {
            (318.77f, 13.81f, .86f), (321.94f, 14.05f, .75f), (317.93f, 15.82f, 1.02f),
            (322.51f, 16.65f, .79f), (320.25f, 18.34f, .76f), (318.10f, 19.54f, .90f),
            (320.61f, 21.53f, .72f), (318.12f, 22.71f, .88f), (319.70f, 24.04f, .73f)
        }, .48f);
        DetalharBarro("Sulco de pneu", "SulcoDaSubida", new (float x, float y, float escala)[]
        {
            (319.73f, 14.26f, .88f), (321.81f, 15.74f, .85f), (319.60f, 17.74f, .76f),
            (320.88f, 19.23f, .91f), (318.87f, 21.35f, .90f), (320.39f, 22.34f, .77f)
        }, .44f);
        DetalharBarro("Água nas marcas de roda", "AguaDaSubida", new (float x, float y, float escala)[]
        {
            (319.47f, 15.22f, .92f), (321.49f, 17.34f, 1.02f),
            (319.64f, 19.97f, .82f), (321.22f, 22.87f, .72f)
        }, .25f);
        int i = 0;
        foreach (var (x, y, escala) in new (float x, float y, float escala)[]
        {
            (317.52f, 15.24f, .86f), (322.68f, 15.86f, 1.04f), (318.12f, 18.65f, .87f),
            (322.04f, 20.20f, .96f), (317.58f, 22.36f, .81f), (321.79f, 24.01f, .92f)
        })
            ModuloDaRodovia("Capim entre pedras da subida", "CapimDaSubida", new Vector2(x, y), escala,
                flip: i++ % 2 == 0, tinta: new Color(.80f, .85f, .84f), pasta: ObjetosDaSubida);

        EspalharVegetacao("Samambaia", new (float x, float y, float escala)[]
        {
            (316.71f, 15.53f, .77f), (324.06f, 15.95f, .94f), (317.08f, 19.17f, .97f),
            (323.08f, 20.62f, .84f), (316.76f, 23.40f, .89f), (322.81f, 24.53f, .92f)
        });
    }

    private void InstalarBalizasDeTerra(params Vector2[] pontos)
    {
        var arte = Resources.Load<Sprite>("Bocha/LuminariaCancha");
        if (arte != null && spriteDasLuminariasDaSubida == null)
        {
            // Reutiliza a caixa metálica com vidro, sem o suporte suspenso.
            spriteDasLuminariasDaSubida = Sprite.Create(arte.texture,
                new Rect(8, 9, 22, 13), new Vector2(.5f, 0f), 88f);
        }

        var luzes = new Vector4[pontos.Length];
        for (int i = 0; i < pontos.Length; i++)
        {
            Vector2 pe = pontos[i];
            var baliza = ModuloDaRodovia("Baliza de madeira da subida", "BalizaDaSubida", pe,
                volume: new Vector3(.18f, .21f, .64f), pasta: ObjetosDaSubida);
            if (baliza == null || spriteDasLuminariasDaSubida == null) continue;

            var luminaria = new GameObject("Luminária da baliza", typeof(SpriteRenderer));
            luminaria.transform.SetParent(baliza.transform, false);
            luminaria.transform.position = new Vector3(pe.x, baliza.bounds.max.y - .02f, 0);
            var renderer = luminaria.GetComponent<SpriteRenderer>();
            renderer.sprite = spriteDasLuminariasDaSubida;
            renderer.sortingOrder = baliza.sortingOrder + 1;
            renderer.sharedMaterial = lighting;
            renderer.GetPropertyBlock(receiverProperties);
            receiverProperties.SetVector("_Receiver", new Vector4(0, 0, 0, 2));
            renderer.SetPropertyBlock(receiverProperties);

            Vector2 centro = pe - origin;
            float altura = renderer.bounds.center.y - pe.y;
            luzes[i] = new Vector4(centro.x, centro.y, altura, 2.05f);
        }
        lighting.SetVectorArray("_MarkerLamps", luzes);
    }

    private void DetalharBarro(string nome, string sprite, (float x, float y, float escala)[] pontos, float opacidade)
    {
        for (int i = 0; i < pontos.Length; i++)
        {
            var (x, y, escala) = pontos[i];
            ModuloDaRodovia(nome, sprite, new Vector2(x, y), escala, flip: i % 2 == 0,
                plano: true, tinta: new Color(.91f, .91f, .89f, opacidade), pasta: ObjetosDaSubida);
        }
    }

    private void ComporPasseioDaEncruzilhada()
    {
        int i = 0;
        foreach (var pe in new[] { new Vector2(302.45f, 14.19f), new Vector2(304.83f, 14.19f),
                     new Vector2(325.36f, 14.79f) })
        {
            var muro = ModuloDaRodovia("Muro baixo da encruzilhada", "Muro", pe, flip: i++ % 2 == 0,
                volume: new Vector3(2.39f, .33f, .92f));
            if (muro != null) LimiteDaRodovia(muro.transform, new Vector2(0, .22f), new Vector2(2.39f, .30f));
        }
        EspalharVegetacao("Sebe", new (float x, float y, float escala)[]
        {
            (302.76f, 14.97f, .92f), (305.48f, 14.88f, 1.03f), (325.01f, 15.55f, .94f),
            (334.40f, 16.04f, 1.12f), (336.45f, 17.01f, .89f), (312.75f, 5.05f, .94f),
            (324.14f, 5.03f, 1.12f), (331.04f, 4.92f, .99f)
        });
        MontarDefensas(new[] { 303.46f, 306.04f, 315.03f, 317.61f, 323.06f, 333.20f }, 5.53f);
        SpriteRenderer anterior = null;
        foreach (float x in new[] { 302.84f, 314.02f, 325.38f, 335.06f })
        {
            var poste = ModuloDaRodovia("Poste do passeio sul", "PosteRede", new Vector2(x, 5.26f),
                tinta: new Color(.73f, .76f, .78f), volume: new Vector3(.19f, .20f, 2.83f));
            if (poste == null) continue;
            if (anterior != null) CriarVaoDeFiacao(anterior, poste);
            anterior = poste;
        }
        foreach (float x in new[] { 303.85f, 307.55f, 314.49f, 324.26f, 328.75f, 333.10f })
            ModuloDaRodovia("Boca de lobo da encruzilhada", "Grelha", new Vector2(x, 12.54f), plano: true);
        foreach (float x in new[] { 305.25f, 308.62f, 315.22f, 320.47f, 328.64f, 333.69f })
            ModuloDaRodovia("Dreno do passeio sul", "Grelha", new Vector2(x, 7.20f), .88f, plano: true);
        EspalharDecalques("Folhas", new (float x, float y, float escala)[]
        {
            (303.99f, 12.43f, .64f), (314.41f, 12.40f, .63f), (328.64f, 12.41f, .66f)
        }, .45f);
    }

    internal static void DelimitarObraDaEncruzilhada(Transform zona, Vector2 origem, float altura)
    {
        var limite = new GameObject("Limite físico da árvore e da margem", typeof(EdgeCollider2D));
        limite.transform.SetParent(zona, false);
        limite.transform.position = origem;
        var borda = limite.GetComponent<EdgeCollider2D>();
        // A linha acompanha a parte sólida do tronco e termina nos maciços dos dois lados.
        borda.points = new[]
        {
            new Vector2(34.52f, 0), new Vector2(34.52f, 4.72f),
            new Vector2(34.39f, 6.37f), new Vector2(34.72f, 7.42f),
            new Vector2(35.03f, 8.14f), new Vector2(35.28f, 8.90f),
            new Vector2(35.75f, 9.95f), new Vector2(35.98f, 11.06f),
            new Vector2(36.25f, 11.79f), new Vector2(36.56f, 12.51f),
            new Vector2(36.74f, 13.40f), new Vector2(36.22f, 14.99f),
            new Vector2(35.58f, 17.03f), new Vector2(35.58f, altura)
        };
        borda.edgeRadius = .18f;
    }

    private void ComporBloqueioDaAvenida()
    {
        ModuloDaRodovia("Árvore caída atravessando a avenida", "TroncoDoBloqueio", new Vector2(335.92f, 6.33f),
            plano: true, tinta: new Color(.92f, .88f, .83f), pasta: ObjetosDoBloqueio);
        ModuloDaRodovia("Raiz exposta na margem norte", "RaizDoBloqueio", new Vector2(337.18f, 13.00f),
            .82f, volume: new Vector3(1.04f, .53f, .77f), pasta: ObjetosDoBloqueio);

        int i = 0;
        foreach (var pe in new[] { new Vector2(334.47f, 6.06f), new Vector2(335.31f, 5.19f),
                     new Vector2(336.34f, 14.18f), new Vector2(336.04f, 15.35f) })
            ModuloDaRodovia("Pedras e terra na margem", "PedrasDoBloqueio", pe, .88f + .08f * (i % 3),
                flip: i++ % 2 == 0, volume: new Vector3(1.53f, .36f, .47f), pasta: ObjetosDoBloqueio);
        foreach (var pe in new[] { new Vector2(334.31f, 8.43f), new Vector2(336.36f, 9.79f),
                     new Vector2(335.63f, 11.25f), new Vector2(337.53f, 12.18f) })
            ModuloDaRodovia("Ramos partidos na pista", "RamosQuebrados", pe, .74f + .10f * (i % 3),
                flip: i++ % 2 == 0, plano: true, tinta: new Color(.86f, .84f, .80f, .83f), pasta: ObjetosDoBloqueio);

        EspalharDecalques("Terra", new (float x, float y, float escala)[]
        {
            (334.70f, 6.41f, .92f), (335.58f, 9.65f, .80f),
            (336.89f, 13.04f, 1.12f), (336.16f, 14.53f, .92f)
        }, .49f);
        EspalharDecalques("Pedras", new (float x, float y, float escala)[]
        {
            (334.63f, 7.06f, .68f), (335.84f, 9.04f, .63f),
            (336.10f, 12.16f, .76f), (336.46f, 13.58f, .88f)
        }, .70f);
        EspalharVegetacao("Samambaia", new (float x, float y, float escala)[]
        {
            (334.08f, 5.69f, .86f), (335.61f, 6.13f, .77f),
            (336.58f, 13.79f, .81f), (335.72f, 15.05f, .83f)
        });
    }
}

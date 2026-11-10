using UnityEngine;

public sealed partial class SantaRitaRuaNorte
{
    private const string ObjetosDoPosto = Art + "Posto/";

    private static readonly (float x, float y, float rx, float ry, float tilt, float blue, float shine)[] PocasDoPosto =
    {
        (3.42f, 9.09f, .86f, .29f, -.16f, .70f, .71f),
        (5.12f, 11.84f, .44f, .52f, .10f, .56f, .85f),
        (8.13f, 11.72f, 1.05f, .29f, -.13f, .62f, .75f),
        (11.50f, 11.18f, .86f, .35f, .08f, .54f, .88f),
        (12.23f, 9.62f, .53f, .21f, -.19f, .64f, .79f),
        (15.30f, 11.87f, .72f, .29f, .12f, .58f, .84f),
        (18.09f, 11.45f, .64f, .38f, -.21f, .56f, .86f),
        (20.84f, 10.45f, 1.04f, .34f, .12f, .65f, .75f),
        (24.57f, 11.81f, .62f, .44f, .15f, .55f, .86f),
        (27.64f, 9.34f, .68f, .23f, -.17f, .63f, .73f),
        (29.45f, 11.49f, 1.09f, .36f, -.07f, .65f, .84f),
        (34.74f, 11.78f, .58f, .44f, -.08f, .54f, .85f)
    };

    private void ComporPosto(SpriteRenderer fachada, SpriteRenderer picape)
    {
        lighting.SetFloat("_Arrival", 1);
        ConfigurarReflexoDoCarro(picape);
        ComporPatioDoPosto(fachada);
        ComporTaludeDoPosto();
        ComporAcostamentoDoPosto();
    }

    private void ComporPatioDoPosto(SpriteRenderer fachada)
    {
        var interacao = transform.Find("Bomba_Posto_Interacao");
        if (interacao != null)
        {
            var bomba = ModuloDaRodovia("Bomba analógica do posto", "BombaAnalogica", interacao.position,
                volume: new Vector3(.64f, .37f, 1.47f), pasta: ObjetosDoPosto);
            if (bomba != null)
            {
                LimiteDaRodovia(bomba.transform, new Vector2(-.07f, .14f), new Vector2(.51f, .31f));
                interacao.GetComponent<InteracaoCapitulo1>().VincularIndicador(bomba);
                interacao.position += Vector3.down * .65f;
            }
        }
        if (fachada != null)
        {
            FixarDetalheDaRodovia(fachada, "Extintor da coluna", "ExtintorPosto",
                new Vector2(3.46f, .65f), ObjetosDoPosto);
            FixarDetalheDaRodovia(fachada, "Cano lateral do posto", "CanoDeChuva",
                new Vector2(-3.45f, .14f));
        }
        foreach (Vector2 pe in new[] { new Vector2(209.72f, 14.08f), new Vector2(230.22f, 14.22f) })
            ModuloDaRodovia("Jardineira da calçada", "JardineiraPosto", pe,
                volume: new Vector3(1.30f, .31f, .76f), pasta: ObjetosDoPosto);
        ModuloDaRodovia("Tambor na lateral do posto", "TamborDeOleo", new Vector2(216.72f, 13.98f),
            volume: new Vector3(.66f, .46f, 1.03f), pasta: ObjetosDoPosto);
        ModuloDaRodovia("Canaleta do pátio", "CanaletaPosto", new Vector2(212.20f, 12.86f),
            plano: true, pasta: ObjetosDoPosto);

        foreach (float x in new[] { 203.92f, 209.22f, 216.26f, 221.62f, 226.74f, 232.41f, 236.86f })
            ModuloDaRodovia("Boca de lobo da avenida", "Grelha", new Vector2(x, 12.53f), plano: true);
        EspalharDecalques("Terra", new (float x, float y, float escala)[]
        {
            (209.72f, 14.00f, .86f), (212.20f, 13.18f, .78f), (216.72f, 13.90f, .82f),
            (227.60f, 13.55f, .88f), (230.22f, 14.14f, .85f), (205.10f, 12.68f, .72f),
            (218.10f, 12.68f, .74f), (224.55f, 12.68f, .72f), (234.70f, 12.68f, .74f)
        }, .34f);
        EspalharDecalques("Pedras", new (float x, float y, float escala)[]
        {
            (209.18f, 13.96f, .58f), (216.38f, 13.88f, .62f), (222.15f, 14.48f, .64f),
            (227.15f, 13.48f, .56f), (230.78f, 14.12f, .58f), (235.12f, 14.16f, .62f)
        }, .54f);
        EspalharDecalques("Folhas", new (float x, float y, float escala)[]
        {
            (203.64f, 12.42f, .61f), (209.54f, 13.92f, .58f), (216.47f, 12.38f, .64f),
            (221.48f, 12.42f, .60f), (226.60f, 12.41f, .62f), (230.05f, 14.08f, .57f),
            (234.64f, 7.20f, .69f), (212.15f, 7.22f, .62f)
        }, .48f);
        EspalharVegetacao("Capim", new (float x, float y, float escala)[]
        {
            (205.28f, 12.74f, .54f), (209.10f, 14.04f, .58f), (216.98f, 13.94f, .56f),
            (218.26f, 12.74f, .54f), (224.72f, 12.74f, .55f), (229.62f, 14.18f, .58f),
            (234.88f, 12.74f, .54f)
        });
    }

    private void ComporTaludeDoPosto()
    {
        var trechos = new (float x, float y)[]
        {
            (202.65f, 14.18f), (205.03f, 14.18f),
            (218.71f, 14.67f), (221.09f, 14.67f), (223.47f, 14.67f),
            (234.78f, 14.28f), (237.16f, 14.28f)
        };
        for (int i = 0; i < trechos.Length; i++)
        {
            var (x, y) = trechos[i];
            var muro = ModuloDaRodovia("Muro baixo do talude", "Muro", new Vector2(x, y), flip: false,
                volume: new Vector3(2.39f, .33f, .92f));
            if (muro == null) continue;
            LimiteDaRodovia(muro.transform, new Vector2(0, .22f), new Vector2(2.40f, .30f));
            if (i == 0 || i == 1 || i == 3 || i == 5)
                FixarDetalheDaRodovia(muro, "Dreno do talude", "DrenoDeMuro",
                    new Vector2(i % 2 == 0 ? -.44f : .52f, .08f));
        }
        foreach (var pe in new[] {
            new Vector2(201.48f, 14.12f), new Vector2(203.84f, 14.12f), new Vector2(206.19f, 14.12f),
            new Vector2(217.52f, 14.60f), new Vector2(219.90f, 14.60f), new Vector2(222.28f, 14.60f), new Vector2(224.67f, 14.60f),
            new Vector2(233.61f, 14.20f), new Vector2(235.97f, 14.20f), new Vector2(238.35f, 14.20f) })
            ModuloDaRodovia("Arremate do talude", "Pilar", pe, volume: new Vector3(.39f, .32f, 1.00f));

        EspalharVegetacao("Sebe", new (float x, float y, float escala)[]
        {
            (202.33f, 14.98f, .92f), (204.12f, 15.04f, .88f), (205.78f, 14.98f, .95f),
            (218.35f, 15.36f, .94f), (220.42f, 15.42f, 1.04f), (222.84f, 15.40f, .99f),
            (234.48f, 15.02f, .96f), (236.68f, 15.00f, 1.06f)
        });
        EspalharVegetacao("Samambaia", new (float x, float y, float escala)[]
        {
            (201.87f, 14.06f, .73f), (203.95f, 14.10f, .68f), (206.49f, 14.24f, .62f),
            (217.85f, 14.54f, .72f), (219.74f, 14.56f, .82f), (222.72f, 14.53f, .68f),
            (224.42f, 14.55f, .70f), (234.46f, 14.22f, .74f), (236.60f, 14.21f, .65f)
        });
        EspalharVegetacao("HeraDoMuro", new (float x, float y, float escala)[]
        {
            (202.95f, 14.14f, .94f), (204.85f, 14.13f, .98f), (219.12f, 14.62f, .95f),
            (221.45f, 14.62f, 1f), (223.68f, 14.61f, .92f), (235.16f, 14.25f, .96f),
            (237.32f, 14.24f, .92f)
        });
    }

    private void ComporAcostamentoDoPosto()
    {
        MontarDefensas(new[] { 203.14f, 205.72f, 210.62f, 213.20f, 218.36f, 220.94f, 228.46f, 231.04f, 235.54f }, 5.54f);
        SpriteRenderer anterior = null;
        foreach (float x in new[] { 202.56f, 213.56f, 224.22f, 235.58f })
        {
            var poste = ModuloDaRodovia("Poste do acostamento", "PosteRede", new Vector2(x, 5.26f),
                tinta: new Color(.73f, .77f, .79f), volume: new Vector3(.19f, .2f, 2.83f));
            if (poste == null) continue;
            if (anterior != null) CriarVaoDeFiacao(anterior, poste);
            anterior = poste;
        }
        foreach (float x in new[] { 205.08f, 212.22f, 220.01f, 227.96f, 234.74f })
            ModuloDaRodovia("Dreno do passeio sul", "Grelha", new Vector2(x, 7.20f), .87f, plano: true);
        EspalharVegetacao("Capim", new (float x, float y, float escala)[]
        {
            (204.56f, 5.73f, .65f), (208.55f, 5.74f, .72f), (212.79f, 5.60f, .61f),
            (216.62f, 5.70f, .57f), (221.78f, 5.57f, .73f), (226.18f, 5.66f, .71f),
            (229.96f, 5.59f, .64f), (235.09f, 5.62f, .72f)
        });
        EspalharVegetacao("Sebe", new (float x, float y, float escala)[]
        {
            (205.04f, 4.96f, .99f), (211.28f, 5.07f, .88f), (216.02f, 4.85f, 1.11f),
            (222.44f, 4.97f, .95f), (230.50f, 4.96f, 1.05f), (235.08f, 4.82f, .95f)
        });
    }
}

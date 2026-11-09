using UnityEngine;

public sealed partial class SantaRitaRuaNorte
{
    private const string PastaDaMata = Art + "Vegetacao/";

    private static readonly (float x, float y, string especie, float escala)[] BosqueDaRodovia =
    {
        (1.65f, 14.98f, "Pitangueira", .87f), (5.20f, 15.44f, "Bracatinga", .92f),
        (11.90f, 16.05f, "Pitangueira", .90f), (16.92f, 15.34f, "Camboata", .82f),
        (22.91f, 15.40f, "Pitangueira", .93f), (27.85f, 15.25f, "Bracatinga", .91f),
        (34.59f, 15.35f, "Camboata", .93f), (3.44f, 18.29f, "Araucaria", 1.16f),
        (14.63f, 18.16f, "Camboata", 1.12f), (25.55f, 18.41f, "Araucaria", 1.13f),
        (9.49f, 3.66f, "Pitangueira", .95f), (29.68f, 3.53f, "Camboata", .93f)
    };

    private static readonly (float x, float y, string especie, float escala)[] BosqueDoPosto =
    {
        (2.49f, 14.93f, "Pitangueira", .85f), (5.84f, 15.57f, "Bracatinga", .91f),
        (16.72f, 16.14f, "Pitangueira", .78f), (19.55f, 15.56f, "Camboata", .85f),
        (22.92f, 15.82f, "Bracatinga", .89f), (34.04f, 15.48f, "Pitangueira", .85f),
        (37.18f, 15.12f, "Camboata", .99f), (4.20f, 18.21f, "Camboata", 1.02f),
        (15.52f, 19.04f, "Araucaria", 1.14f), (21.95f, 18.29f, "Araucaria", 1.05f),
        (35.67f, 18.55f, "Bracatinga", 1.13f), (8.48f, 3.67f, "Pitangueira", .90f),
        (20.15f, 3.53f, "Camboata", .96f), (32.87f, 3.58f, "Bracatinga", .92f)
    };

    private static readonly (float x, float y, string especie, float escala)[] BosqueDaEncruzilhada =
    {
        (2.90f, 14.98f, "Pitangueira", .89f), (6.39f, 16.04f, "Camboata", .84f),
        (15.76f, 16.38f, "Bracatinga", .84f), (15.89f, 19.55f, "Camboata", .98f),
        (24.10f, 16.80f, "Pitangueira", .88f), (24.37f, 21.29f, "Bracatinga", 1.04f),
        (16.62f, 23.08f, "Pitangueira", .81f), (23.53f, 24.70f, "Camboata", .94f),
        (34.08f, 16.76f, "Camboata", 1.02f), (37.34f, 17.17f, "Bracatinga", 1.01f),
        (38.64f, 15.58f, "Pitangueira", .83f), (3.91f, 20.04f, "Araucaria", 1.15f),
        (29.04f, 19.73f, "Araucaria", 1.13f), (35.98f, 20.24f, "Araucaria", 1.08f),
        (7.11f, 3.72f, "Camboata", .91f), (18.25f, 3.53f, "Pitangueira", .97f),
        (30.53f, 3.66f, "Bracatinga", .92f), (35.02f, 5.13f, "Pitangueira", .86f)
    };

    // Os vazios no passeio sul correspondem à mala e à boca da travessia.
    private static readonly float[] MataSulDaRodovia = { 2.05f, 6.24f, 10.91f, 15.18f, 19.18f, 25.35f, 29.44f, 33.78f };
    private static readonly float[] MataSulDoPosto = { 2.26f, 6.52f, 11.01f, 15.52f, 20.35f, 25.41f, 30.05f, 34.71f, 37.22f };
    private static readonly float[] MataSulDaEncruzilhada = { 2.30f, 6.61f, 14.21f, 18.56f, 23.54f, 28.18f, 32.39f, 36.07f, 38.53f };

    private static readonly (float x, float y, string especie, float escala)[] FundoDaRodovia =
    {
        (1.32f, 19.28f, "Camboata", 1.12f), (6.87f, 18.47f, "Bracatinga", 1.07f),
        (9.96f, 19.13f, "Araucaria", 1.08f), (13.13f, 19.63f, "Bracatinga", 1.05f),
        (17.94f, 18.84f, "Camboata", 1.10f), (20.81f, 19.47f, "Araucaria", 1.07f),
        (23.45f, 18.45f, "Bracatinga", 1.04f), (28.51f, 19.50f, "Camboata", 1.11f),
        (32.09f, 18.71f, "Araucaria", 1.17f), (34.81f, 19.32f, "Bracatinga", 1.06f)
    };

    private static readonly (float x, float y, string especie, float escala)[] FundoDoPosto =
    {
        (2.26f, 20.26f, "Camboata", 1.13f), (5.46f, 21.28f, "Araucaria", 1.17f),
        (8.05f, 19.54f, "Bracatinga", 1.06f), (11.77f, 20.39f, "Camboata", 1.04f),
        (14.33f, 21.51f, "Araucaria", 1.19f), (18.61f, 20.58f, "Bracatinga", 1.05f),
        (21.21f, 21.30f, "Camboata", 1.10f), (25.24f, 19.61f, "Araucaria", 1.14f),
        (28.13f, 20.46f, "Bracatinga", 1.03f), (31.50f, 21.26f, "Camboata", 1.14f),
        (33.32f, 19.84f, "Pitangueira", 1.06f), (36.43f, 20.90f, "Araucaria", 1.19f)
    };

    private static readonly (float x, float y, string especie, float escala)[] FundoDaEncruzilhada =
    {
        (2.14f, 22.31f, "Bracatinga", 1.09f), (5.28f, 24.83f, "Araucaria", 1.20f),
        (7.59f, 20.26f, "Camboata", 1.04f), (8.38f, 23.54f, "Bracatinga", 1.06f),
        (10.48f, 25.43f, "Camboata", 1.14f), (11.14f, 21.66f, "Pitangueira", 1.04f),
        (13.88f, 20.73f, "Bracatinga", 1.09f), (13.04f, 23.42f, "Araucaria", 1.17f),
        (15.05f, 25.26f, "Camboata", 1.12f), (16.63f, 21.43f, "CamboataJovem", 1.04f),
        (24.02f, 22.73f, "CamboataJovem", 1.11f), (24.54f, 25.31f, "Araucaria", 1.18f),
        (26.58f, 20.05f, "Camboata", 1.07f), (27.25f, 23.23f, "Bracatinga", 1.13f),
        (29.95f, 25.36f, "Camboata", 1.16f), (31.03f, 21.72f, "Pitangueira", 1.05f),
        (33.81f, 24.32f, "Araucaria", 1.18f), (37.92f, 23.06f, "Bracatinga", 1.08f)
    };

    // Fileiras desencontradas atrás das construções e nas margens da subida.
    private static readonly (float x, float y, string especie, float escala)[] MatagalDaEncruzilhada =
    {
        (302.2f, 17.43f, "Pitangueira", 0.99f), (305f, 17.69f, "Camboata", 1.08f),
        (308f, 17.17f, "Bracatinga", 1.08f), (310.8f, 17.56f, "Camboata", 1.14f),
        (313.5f, 17.27f, "Pitangueira", 1.06f), (315.8f, 17.43f, "CamboataJovem", 0.94f),
        (324.2f, 17.69f, "CamboataJovem", 1.03f), (326.5f, 17.17f, "Camboata", 1.03f),
        (329.2f, 17.56f, "Bracatinga", 1.13f), (332f, 17.27f, "Pitangueira", 1.04f),
        (334.8f, 17.43f, "Camboata", 1.09f), (337.8f, 17.69f, "Bracatinga", 1.06f),
        (340f, 17.17f, "Pitangueira", 1.12f), (300.5f, 19.27f, "Camboata", 1.07f),
        (303.1f, 19.66f, "Bracatinga", 1.13f), (306.1f, 19.37f, "Pitangueira", 1.13f),
        (309f, 19.53f, "Camboata", 1.16f), (311.9f, 19.79f, "Bracatinga", 1.07f),
        (314.7f, 19.27f, "Camboata", 1.02f), (316.7f, 19.66f, "BracatingaJovem", 1.02f),
        (323.9f, 19.37f, "BracatingaJovem", 1.02f), (325.9f, 19.53f, "Bracatinga", 1.14f),
        (328.8f, 19.79f, "Camboata", 1.08f), (331.6f, 19.27f, "Pitangueira", 1.12f),
        (334.2f, 19.66f, "Bracatinga", 1.14f), (337f, 19.37f, "Camboata", 1.12f),
        (339.7f, 19.53f, "Bracatinga", 1.05f), (301.5f, 21.52f, "Bracatinga", 1.17f),
        (304.4f, 21.68f, "Camboata", 1.11f), (307.2f, 21.94f, "Bracatinga", 1.1f),
        (310.2f, 21.42f, "Pitangueira", 1.1f), (313.1f, 21.81f, "Camboata", 1.13f),
        (315.3f, 21.52f, "Pitangueira", 1.01f), (316.8f, 21.68f, "CamboataJovem", 1.04f),
        (323.3f, 21.94f, "CamboataJovem", 1.03f), (325.4f, 21.42f, "Camboata", 1.08f),
        (328f, 21.81f, "Pitangueira", 1.12f), (331f, 21.52f, "Bracatinga", 1.12f),
        (333.7f, 21.68f, "Camboata", 1.13f), (336.8f, 21.94f, "Pitangueira", 1.09f),
        (339.3f, 21.42f, "Camboata", 1.19f), (300.5f, 24.04f, "Camboata", 1.14f),
        (303.5f, 23.52f, "Pitangueira", 1.14f), (306.2f, 23.91f, "Camboata", 1.2f),
        (309.1f, 23.62f, "Bracatinga", 1.12f), (312f, 23.78f, "Camboata", 1.15f),
        (314.9f, 24.04f, "Bracatinga", 1.03f), (316.5f, 23.52f, "BracatingaJovem", 1.03f),
        (323.4f, 23.91f, "BracatingaJovem", 1.06f), (325.6f, 23.62f, "Pitangueira", 1.08f),
        (328.5f, 23.78f, "Bracatinga", 1.13f), (331.3f, 24.04f, "Camboata", 1.18f),
        (334.1f, 23.52f, "Bracatinga", 1.16f), (337f, 23.91f, "Camboata", 1.16f),
        (339.8f, 23.62f, "Pitangueira", 1.14f), (302f, 25.41f, "Bracatinga", 1.2f),
        (305f, 25.12f, "Camboata", 1.2f), (308f, 25.28f, "Pitangueira", 1.15f),
        (311f, 25.54f, "Bracatinga", 1.18f), (314f, 25.02f, "Camboata", 1.19f),
        (316.3f, 25.41f, "CamboataJovem", 1.1f), (323f, 25.12f, "CamboataJovem", 1.11f),
        (325.1f, 25.28f, "Bracatinga", 1.14f), (328f, 25.54f, "Camboata", 1.18f),
        (331f, 25.02f, "Bracatinga", 1.17f), (334f, 25.41f, "Pitangueira", 1.16f),
        (337f, 25.12f, "Bracatinga", 1.15f), (340f, 25.28f, "Camboata", 1.2f),
        (316.1f, 16.2f, "BracatingaJovem", 1.02f), (324.3f, 16.4f, "CamboataJovem", 1.05f),
        (317.02f, 18.2f, "CamboataJovem", 0.88f), (323.7f, 18.15f, "BracatingaJovem", 0.95f),
        (316.9f, 20.6f, "CamboataJovem", 0.9f), (323.2f, 22.7f, "CamboataJovem", 0.92f),
    };

    private void FecharCopasDaEncruzilhada()
    {
        // Raízes fora do corredor de terra. O preenchimento é só visual.
        for (int i = 0; i < MatagalDaEncruzilhada.Length; i++)
        {
            var (x, y, especie, escala) = MatagalDaEncruzilhada[i];
            Plant("Copa do matagal " + i, especie, x, y, escala, i % 2 == 0, temTroncoSolido: false);
        }

        InserirSubmata("MoitaDeBorda", new Vector2(315.75f, 15.25f), 1.02f, true);
        InserirSubmata("MoitaDeBorda", new Vector2(316.20f, 16.19f), 1.06f, false);
        InserirSubmata("MoitaDeBorda", new Vector2(324.35f, 15.40f), 1.08f, true);
        InserirSubmata("MoitaDeBorda", new Vector2(324.75f, 16.46f), 1.12f, false);
    }

    private void ComporBosqueDaRua()
    {
        var plantio = entrada ? BosqueDaRodovia : posto ? BosqueDoPosto : BosqueDaEncruzilhada;
        for (int i = 0; i < plantio.Length; i++)
        {
            var (x, y, especie, escala) = plantio[i];
            float worldX = origin.x + x;
            float worldY = origin.y + y;
            bool espelhar = i % 3 == 1;
            Plant(especie + " do bosque " + i, especie, worldX, worldY, escala, espelhar);

            if (y < 14.6f || y > 19.7f) continue;
            float lado = espelhar ? -1f : 1f;
            var raiz = new Vector2(worldX, worldY);
            InserirSubmata("MoitaDeBorda", raiz + new Vector2(lado * .57f, -.13f),
                .67f + .08f * (i % 4), espelhar);
            InserirSubmata("SamambaiaDaMata", raiz + new Vector2(-lado * .65f, -.34f),
                .60f + .06f * (i % 3), !espelhar);
            if (i % 3 == 0)
                Plant("Árvore jovem do bosque " + i, "CamboataJovem", worldX + lado * 1.16f,
                    worldY + .61f, .77f + .08f * (i % 2), !espelhar);
        }
        ComporFundoDaMata(entrada ? FundoDaRodovia : posto ? FundoDoPosto : FundoDaEncruzilhada);
        if (encruzilhada) FecharCopasDaEncruzilhada();
        else if (entrada || posto) FecharMatasDaAvenida();
        ComporMataSul(entrada ? MataSulDaRodovia : posto ? MataSulDoPosto : MataSulDaEncruzilhada);
    }

    private void FecharMatasDaAvenida()
    {
        // As duas fileiras de baixo terminam antes do passeio sul (y = 6).
        PlantarFileiraDaAvenida("fundo sul", .55f, .35f, 2.65f, baixa: false, fase: posto ? 2 : 0);
        PlantarFileiraDaAvenida("borda sul", 2.88f, 1.38f, 1.95f, baixa: true, fase: posto ? 1 : 3);

        // No norte, as árvores começam atrás da parada e das fachadas.
        PlantarFileiraDaAvenida("borda norte", posto ? 16.88f : 16.38f, .62f, 2.65f,
            baixa: false, fase: posto ? 3 : 1);
        PlantarFileiraDaAvenida("fundo norte", posto ? 19.72f : 18.60f, 1.77f, 2.65f,
            baixa: false, fase: posto ? 0 : 2);
    }

    private void PlantarFileiraDaAvenida(string faixa, float y, float inicio, float passo, bool baixa, int fase)
    {
        string[] especies = baixa
            ? new[] { "CamboataJovem", "BracatingaJovem" }
            : new[] { "Camboata", "Pitangueira", "Bracatinga", "Camboata", "Bracatinga" };
        float[] desviosX = { -.15f, .24f, .05f, -.23f, .18f };
        float[] desviosY = { .12f, -.25f, .34f, -.08f, .22f };

        int i = 0;
        for (float x = inicio; x < size.x; x += passo, i++)
        {
            int variante = i + fase;
            float escala = (baixa ? .96f : 1.08f) + .04f * (variante % 4);
            var raiz = origin + new Vector2(x + desviosX[variante % desviosX.Length],
                y + desviosY[variante % desviosY.Length]);
            Plant("Copa da avenida · " + faixa + " " + i, especies[variante % especies.Length],
                raiz.x, raiz.y, escala, variante % 2 == 0, temTroncoSolido: false);
        }
    }

    private void ComporFundoDaMata((float x, float y, string especie, float escala)[] árvores)
    {
        for (int i = 0; i < árvores.Length; i++)
        {
            var (x, y, especie, escala) = árvores[i];
            var raiz = origin + new Vector2(x, y);
            bool espelhar = i % 2 == 0;
            Plant("Copa de fundo " + i, especie, raiz.x, raiz.y, escala, espelhar);
            if (i % 3 == 0)
                InserirSubmata("MoitaDeBorda", raiz + new Vector2(espelhar ? .61f : -.62f, -.30f),
                    .91f + .06f * (i % 4), !espelhar);
        }
    }

    private void ComporMataSul(float[] bolsões)
    {
        for (int i = 0; i < bolsões.Length; i++)
        {
            float x = origin.x + bolsões[i];
            float lado = i % 2 == 0 ? 1 : -1;
            float variação = (i % 3) * .19f;

            Plant("Copa jovem da margem sul " + i, "CamboataJovem", x - lado * .95f,
                2.58f + variação, .91f + variação * .30f, lado < 0);
            Plant("Copa leve da margem sul " + i, "BracatingaJovem", x + lado * .81f,
                3.54f - variação, .83f + variação * .32f, lado > 0);
            InserirSubmata("MoitaDeBorda", new Vector2(x + lado * .29f, 4.71f - variação),
                .98f + variação * .28f, lado > 0);
            InserirSubmata("SamambaiaDaMata", new Vector2(x - lado * .66f, 4.98f + variação * .31f),
                .72f + variação * .27f, lado < 0);
            InserirSubmata("MoitaDeBorda", new Vector2(x + lado * 1.23f, 2.02f + variação),
                .76f + variação * .24f, lado < 0);

            ModuloDaRodovia("Folhiço sob as copas", "Folhico", new Vector2(x - lado * .30f, 4.24f - variação),
                .99f, flip: lado < 0, plano: true, tinta: new Color(.81f, .85f, .83f, .64f), pasta: PastaDaMata);
            ModuloDaRodovia("Folhiço no interior da mata", "Folhico", new Vector2(x + lado * .41f, 2.11f + variação),
                .91f, flip: lado > 0, plano: true, tinta: new Color(.81f, .85f, .83f, .57f), pasta: PastaDaMata);
            if (i % 2 == 0)
                ModuloDaRodovia("Galho úmido caído", "GalhoCaido", new Vector2(x + lado * .91f, 3.85f + variação),
                    .87f, flip: lado < 0, plano: true, tinta: new Color(.83f, .84f, .79f, .86f), pasta: PastaDaMata);
        }
    }

    private void InserirSubmata(string arte, Vector2 pe, float escala, bool espelhar)
    {
        var planta = ModuloDaRodovia("Submata · " + arte, arte, pe, escala, flip: espelhar,
            tinta: new Color(.87f, .94f, .93f), pasta: PastaDaMata);
        if (planta != null) AplicarMaterialDaMata(planta, arte, .031f * escala);
    }

    private bool AplicarMaterialDaMata(SpriteRenderer renderer, string especie, float vento)
    {
        var relevo = Resources.Load<Texture2D>(PastaDaMata + "Relevos/" + especie);
        if (relevo == null) return false;

        // Compensação em luz linear a partir da luminosidade mediana das copas de origem.
        var paleta = especie switch
        {
            "Araucaria" => new Vector4(1.04f, 1f, 1f, .93f),
            "Bracatinga" => new Vector4(.46f, .47f, .42f, .79f),
            "Camboata" => new Vector4(.70f, .73f, .78f, .90f),
            "CamboataJovem" => new Vector4(1.17f, 1.14f, 1.25f, .90f),
            "BracatingaJovem" => new Vector4(.75f, .72f, .87f, .91f),
            "MoitaDeBorda" => new Vector4(1.04f, 1.06f, 1.18f, .88f),
            "SamambaiaDaMata" => new Vector4(.71f, .77f, .94f, .90f),
            _ => new Vector4(.87f, .91f, 1f, .93f)
        };
        renderer.GetPropertyBlock(receiverProperties);
        receiverProperties.SetTexture("_PropSurface", relevo);
        receiverProperties.SetFloat("_PropSurfaceEnabled", 1);
        receiverProperties.SetVector("_CanopyTone", paleta);
        receiverProperties.SetVector("_Wind", new Vector4(vento, renderer.bounds.size.y,
            renderer.transform.position.x * .79f, 1));
        renderer.SetPropertyBlock(receiverProperties);
        return true;
    }
}

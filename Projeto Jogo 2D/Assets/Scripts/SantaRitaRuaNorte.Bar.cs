using UnityEngine;
using UnityEngine.Rendering.Universal;

public sealed partial class SantaRitaRuaNorte
{
    private const string DetalhesDoBar = Art + "Bar/Detalhes/";

    private static readonly (float x, float y, float rx, float ry, float tilt, float blue, float shine)[] PocasDoBar =
    {
        (4.35f, 9.5f, .51f, .24f, .11f, .53f, .76f),
        (7.04f, 11.76f, .55f, .39f, .18f, .42f, .96f),
        (10.45f, 8.64f, .86f, .21f, -.16f, .75f, .77f),
        (12.81f, 11.48f, .66f, .43f, -.12f, .43f, .98f),
        (15.55f, 9.4f, .60f, .30f, .14f, .86f, .80f),
        (18.16f, 11.26f, 1.02f, .38f, -.10f, .49f, .98f),
        (20.29f, 8.45f, .65f, .19f, .07f, .63f, .75f),
        (22.55f, 11.27f, .72f, .42f, .15f, .40f, .97f),
        (26.84f, 9.43f, 1.04f, .41f, -.12f, .78f, .84f),
        (29.98f, 11.54f, .66f, .43f, .15f, .46f, .97f),
        (33.43f, 11.83f, .62f, .49f, .12f, .72f, .84f)
    };

    private static void SelectBarGround(int x, int y, out Ground kind, out bool flipX, out bool flipY)
    {
        bool chegada = x < 6;
        bool recuo = x >= 7 && x <= 11;
        bool patio = x >= 11 && x <= 27;
        bool abrigo = x >= 22 && x <= 25;
        flipX = false;
        flipY = y < 8;
        switch (y)
        {
            case 5: kind = x >= 6 && x <= 35 ? Ground.Verge : Ground.Grass; return;
            case 6: kind = x >= 6 && x <= 35 ? Ground.Walk : Ground.Grass; return;
            case 7: kind = chegada ? Ground.DirtVerge : x <= 35 ? Ground.Curb : Ground.Grass; return;
            case 12: kind = chegada ? Ground.DirtVerge : recuo ? Ground.Road : x <= 30 ? Ground.Curb : Ground.Grass; return;
            case 13: kind = chegada || x > 30 ? Ground.Grass : Ground.Walk; return;
            case 14:
                kind = patio || x >= 36 ? Ground.Walk : x >= 6 && x <= 30 ? Ground.Verge : Ground.Grass;
                return;
            case 15:
                kind = abrigo ? Ground.Walk : patio || x >= 36 ? Ground.Verge : Ground.Grass;
                return;
        }
        if (y < 8 || y > 11 || x > 30) { kind = Ground.Grass; return; }
        if (x < 5) { kind = Ground.Dirt; return; }
        if (x == 5) { kind = Ground.DirtRoad; return; }
        if (y == 10 && x >= 7 && x <= 27 && x % 5 < 2) { kind = Ground.Stripe; return; }
        kind = (x * 19 + y * 13 + x * y) % 19 < 3 ? Ground.Crack : Ground.Road;
    }

    private void DressBar()
    {
        casterCount = 0;
        DressProp("Predio_Fachada_BarDoZe", "Fachada", new Vector3(7.2f, 2.4f, 3.65f), heightFactor: .82f, sombra: Sombra.EdificioSuave);
        DressProp("Caminhonete_Clube_Bar", "Picape", new Vector3(3.85f, 1.18f, 1.65f), sombra: Sombra.Veiculo);
        DressProp("Mesa_Calcada_Bar", "Mesa", new Vector3(.86f, .63f, .76f), sombra: Sombra.Recorte);
        DressChair("Cadeira_Calcada_1", false);
        DressChair("Cadeira_Calcada_2", true);
        if (DressProp("Engradados_Bar_Ext", "Bar/Detalhes/EngradadosRetornaveis", new Vector3(1.18f, .48f, .64f), sombra: Sombra.Recorte) == null)
            DressProp("Engradados_Bar_Ext", "Engradado", new Vector3(.65f, .45f, .51f));
        DressProp("Dumpster_Bar_Ext", "Contentor", new Vector3(1.24f, .70f, .91f), sombra: Sombra.ProjetadaSuave);
        DressProp("Orelhao_BarDoZe", "Orelhao", new Vector3(.25f, .25f, 1.40f), sombra: Sombra.Recorte);
        if (DressProp("Portao_Centro", "Bar/Detalhes/PortaoDoCentro", new Vector3(4.45f, .25f, 1.72f), sombra: Sombra.Recorte) == null)
            DressProp("Portao_Centro", "Portao", new Vector3(4.0f, .36f, 1.8f));
        for (int i = 1; i <= 4; i++)
            DressProp("Poste_Z5_" + i, "Poste", new Vector3(.16f, .18f, 2.64f), sombra: Sombra.ProjetadaSuave);

        ComporCalcadasDoBar();
        ComporMataDoBar();

        foreach (var light in GetComponentsInChildren<Light2D>(true))
            light.enabled = false;

        var lamps = new[]
        {
            Lamp("Poste_Z5_3", .66f, -.42f, 3.18f, 8.35f),
            Lamp("Predio_Fachada_BarDoZe", 0f, -.85f, 2.82f, 5.8f),
            Lamp("Poste_Z5_2", .66f, -.42f, 3.18f, 8.35f)
        };
        lighting.SetVectorArray("_Lamps", lamps);
        lighting.SetVectorArray("_LampColors", new[]
        {
            new Vector4(1.99f, 1.54f, .99f, 1),
            new Vector4(1.58f, 1.13f, .69f, 1),
            new Vector4(1.95f, 1.47f, .92f, 1)
        });
        lighting.SetVector("_FourthLamp", Lamp("Poste_Z5_1", .66f, -.42f, 3.18f, 8.4f));
        lighting.SetVector("_FifthLamp", Lamp("Poste_Z5_4", .66f, -.42f, 3.18f, 8.2f));
        luzesDeRecorte = lamps;
        UpdateCasters(false);
    }

    private void ComporCalcadasDoBar()
    {
        // Mesas à esquerda da entrada, retornáveis sob o abrigo e coleta afastada do balcão.
        ObjetoDoBar("Cavalete do bar", "CavaleteBar", new Vector2(515.52f, 13.93f), new Vector3(.62f, .36f, .74f));
        ObjetoDoBar("Jardineira do passeio", "JardineiraBar", new Vector2(511.09f, 15.02f), new Vector3(.98f, .51f, .91f));
        ObjetoDoBar("Caixa térmica do bar", "CaixaTermica", new Vector2(522.91f, 14.82f), new Vector3(.95f, .60f, 1.02f));
        ObjetoDoBar("Barril do bar", "Barril", new Vector2(510.03f, 14.69f), new Vector3(.65f, .51f, .82f));

        var fundo = ModuloDaRodovia("Mureta do abrigo de serviço", "Muro", new Vector2(523.76f, 15.70f),
            1.33f, volume: new Vector3(3.22f, .34f, .90f));
        if (fundo != null) LimiteDaRodovia(fundo.transform, new Vector2(0, .13f), new Vector2(2.36f, .32f));
        foreach (float x in new[] { 522.22f, 525.30f })
        {
            var pilar = ModuloDaRodovia("Pilar de madeira do abrigo", "PilarMadeira", new Vector2(x, 14.63f),
                .71f, volume: new Vector3(.25f, .24f, 1.56f), pasta: "Art/Props/");
            if (pilar != null) LimiteDaRodovia(pilar.transform, new Vector2(0, .09f), new Vector2(.30f, .26f));
        }
        var cobertura = ModuloDaRodovia("Cobertura do abrigo de serviço", "CoberturaZinco", new Vector2(523.76f, 15.91f),
            1.26f, volume: new Vector3(3.27f, .68f, 1.92f), alturaDoVolume: 1.48f, pasta: DetalhesDoBar);
        if (cobertura != null) cobertura.sortingOrder = 855;

        // O portão fecha a boca de uma subida, com os dois vãos laterais entregues à alvenaria.
        var acesso = (Vector2)transform.Find("Portao_Centro").position;
        for (int lado = -1; lado <= 1; lado += 2)
        {
            var muro = ModuloDaRodovia("Ala de pedra do portão", "Muro", acesso + new Vector2(lado * 3.38f, .04f),
                .94f, flip: lado > 0, volume: new Vector3(2.35f, .34f, .74f));
            if (muro != null) LimiteDaRodovia(muro.transform, new Vector2(0, .18f), new Vector2(2.44f, .40f));
        }

        foreach (float x in new[] { 506.12f, 511.71f, 518.88f, 525.11f, 529.0f })
            ModuloDaRodovia("Boca de lobo · Rua Norte", "Grelha", new Vector2(x, 12.51f), .84f, plano: true);
        foreach (float x in new[] { 508.4f, 516.76f, 524.28f, 531.26f })
            ModuloDaRodovia("Drenagem sul · Rua Norte", "Grelha", new Vector2(x, 7.17f), .88f, plano: true);

        foreach (float x in new[] { 503.61f, 506.45f, 509.29f })
            ModuloDaRodovia("Arrimo da margem norte", "Muro", new Vector2(x, 15.40f),
                1.14f, volume: new Vector3(2.72f, .29f, .84f));
        InserirSubmata("MoitaDeBorda", new Vector2(527.12f, 15.35f), .92f, true);
        InserirSubmata("SamambaiaDaMata", new Vector2(528.42f, 14.97f), .80f, false);
        foreach (float x in new[] { 508.46f, 511.22f, 521.85f, 524.58f, 527.30f })
            ModuloDaRodovia("Arrimo da margem sul", "Muro", new Vector2(x, 5.56f),
                1.10f, flip: x > 520f, volume: new Vector3(2.65f, .34f, .81f));
        for (int i = 0; i < 13; i++)
        {
            float x = 502.55f + i * 2.68f;
            ModuloDaRodovia("Folhiço no passeio", "Folhico", new Vector2(x, 6.18f + .09f * (i % 3)),
                .62f + .10f * (i % 4), flip: i % 2 == 0, plano: true,
                tinta: new Color(.83f, .83f, .81f, .52f), pasta: PastaDaMata);
        }
    }

    private void ObjetoDoBar(string nome, string arte, Vector2 pe, Vector3 volume)
    {
        var objeto = ModuloDaRodovia(nome, arte, pe, volume: volume, pasta: DetalhesDoBar);
        if (objeto != null) LimiteDaRodovia(objeto.transform, Vector2.up * volume.y * .48f, new Vector2(volume.x, volume.y));
    }

    private void ComporMataDoBar()
    {
        string[] especies = { "Camboata", "Pitangueira", "Bracatinga", "Araucaria", "CamboataJovem", "BracatingaJovem" };
        var sorte = new System.Random(3515);
        for (int faixa = 0; faixa < 5; faixa++)
        {
            bool norte = faixa >= 2;
            bool borda = faixa == 1 || faixa == 2;
            int i = 0;
            for (float x = 501.10f + (faixa % 2) * 1.46f; x < 537.8f; x += 2.61f, i++)
            {
                float px = x + (float)sorte.NextDouble() * .75f;
                if (norte && px > 530.25f && px < 535.65f) continue;
                float y = faixa switch { 0 => 1.15f, 1 => 4.22f, 2 => 15.96f, 3 => 18.65f, _ => 21.14f };
                if (faixa == 2 && px > 510.7f && px < 529.1f)
                    y += px > 513.8f && px < 525.2f ? 1.32f : .61f;
                float py = y + (float)sorte.NextDouble() * .58f;
                string especie = especies[sorte.Next(especies.Length)];
                float escala = .88f + (float)sorte.NextDouble() * .27f;
                bool solido = borda && i % 2 == 0;
                string nome = $"Mata da Rua Norte {faixa}-{i}";
                Plant(nome, especie, px, py, escala, i % 2 == 1, temTroncoSolido: solido);
                if (borda)
                {
                    InserirSubmata("MoitaDeBorda", new Vector2(px - .63f, py - .32f), .78f, i % 2 == 1);
                    InserirSubmata("SamambaiaDaMata", new Vector2(px + .82f, py + .10f), .70f, i % 2 == 0);
                    if (!norte && !solido)
                        transform.Find(nome).gameObject.AddComponent<SantaRitaForeground>();
                }
            }
        }
    }
}


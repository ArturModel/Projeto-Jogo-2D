using UnityEngine;
using UnityEngine.Rendering.Universal;

public sealed partial class SantaRitaRuaNorte
{
    // Mapa em coordenadas locais. A mesma malha guia o chão, a mata e os detalhes.
    private static readonly (Vector2 inicio, Vector2 fim, float margem)[] RuasDoBairro =
    {
        (new(20, -1), new(20, 4.6f), 1.65f),
        (new(20, 4.6f), new(23.4f, 8.2f), 1.80f),
        (new(23.4f, 8.2f), new(25.8f, 12.8f), 1.85f),
        (new(25.8f, 12.8f), new(24.8f, 16.5f), 1.85f),
        (new(24.8f, 16.5f), new(23.5f, 21.25f), 1.90f),
        (new(23.5f, 21.25f), new(29.9f, 21.0f), 1.90f),
        (new(29.9f, 21.0f), new(34.7f, 24.35f), 1.85f),
        (new(34.7f, 24.35f), new(35.7f, 29.1f), 1.85f),
        (new(35.7f, 29.1f), new(35.5f, 35), 1.75f),
        (new(23.4f, 8.2f), new(16.0f, 8.45f), 1.44f),
        (new(16.0f, 8.45f), new(9.5f, 10.25f), 1.40f),
        (new(25.8f, 12.8f), new(31.65f, 13.55f), 1.34f),
        (new(23.5f, 21.25f), new(14.15f, 21.52f), 1.39f),
        (new(34.95f, 25.55f), new(39.25f, 25.45f), 1.27f),
        (new(14.65f, 8.85f), new(14.7f, 12.65f), 1.13f)
    };

    // Pátios curtos, orientados para a rua e livres de troncos.
    private static readonly Rect[] PatiosDoBairro =
    {
        Rect.MinMaxRect(7.5f, 10.5f, 13.2f, 13.0f),
        Rect.MinMaxRect(29.0f, 13.6f, 34.2f, 16.1f),
        Rect.MinMaxRect(12.1f, 21.6f, 17.3f, 24.0f),
        Rect.MinMaxRect(19.8f, 22.0f, 27.1f, 24.55f),
        Rect.MinMaxRect(37.3f, 25.6f, 42.1f, 27.6f)
    };

    private static readonly (float x, float y, float rx, float ry, float tilt, float blue, float shine)[] PocasDoBairro =
    {
        (19.47f, 3.4f, .30f, .72f, -.18f, .45f, .90f),
        (21.1f, 6.0f, .37f, .63f, .23f, .63f, .81f),
        (13.5f, 8.76f, .81f, .30f, .12f, .52f, .85f),
        (18.70f, 8.95f, .66f, .27f, -.06f, .66f, .74f),
        (24.65f, 10.6f, .36f, .68f, .20f, .45f, .90f),
        (28.65f, 12.98f, .85f, .29f, .08f, .49f, .92f),
        (25.99f, 15.18f, .32f, .75f, -.18f, .56f, .85f),
        (23.65f, 18.42f, .38f, .71f, -.16f, .53f, .86f),
        (15.95f, 20.94f, .73f, .25f, .07f, .63f, .83f),
        (22.24f, 21.3f, .84f, .31f, -.14f, .54f, .82f),
        (28.1f, 20.37f, .90f, .31f, -.07f, .52f, .96f),
        (32.4f, 22.14f, .69f, .31f, .24f, .54f, .84f),
        (35.25f, 26.52f, .37f, .82f, .09f, .39f, .88f),
        (36.15f, 29.6f, .43f, .75f, -.16f, .54f, .86f),
        (34.92f, 31.91f, .28f, .71f, .03f, .56f, .88f)
    };

    private static float DistanciaDaRuaDoBairro(Vector2 posicao)
    {
        float menor = float.PositiveInfinity;
        foreach (var (inicio, fim, margem) in RuasDoBairro)
        {
            Vector2 trecho = fim - inicio;
            float t = Mathf.Clamp01(Vector2.Dot(posicao - inicio, trecho) / trecho.sqrMagnitude);
            menor = Mathf.Min(menor, Vector2.Distance(posicao, inicio + t * trecho) - margem);
        }
        return menor;
    }

    internal static bool PatioDoBairro(Vector2 posicao)
    {
        foreach (var patio in PatiosDoBairro)
            if (patio.Contains(posicao)) return true;
        return false;
    }

    internal static bool RuaDeTerraDoBairro(Vector2 posicao) => DistanciaDaRuaDoBairro(posicao) < 0;

    private static void SelectBairroGround(int x, int y, out Ground kind, out bool flipX, out bool flipY)
    {
        flipX = false;
        flipY = false;
        var centro = new Vector2(x + .5f, y + .5f);
        kind = PatioDoBairro(centro) ? Ground.Walk : Ground.Grass;
    }

    private void ConfigurarTrilhasDoBairro()
    {
        var trechos = new Vector4[RuasDoBairro.Length];
        var larguras = new Vector4[RuasDoBairro.Length];
        for (int i = 0; i < RuasDoBairro.Length; i++)
        {
            var (inicio, fim, margem) = RuasDoBairro[i];
            trechos[i] = new Vector4(inicio.x, inicio.y, fim.x, fim.y);
            larguras[i] = new Vector4(margem, 0, 0, 0);
        }
        lighting.SetVectorArray("_BairroTrechos", trechos);
        lighting.SetVectorArray("_BairroMargens", larguras);
    }

    private void DressBairro()
    {
        casterCount = 0;
        DressProp("Predio_CasaDoAvo_40", "Bairro/Avo", new Vector3(6.35f, 2.0f, 3.0f), heightFactor: .86f, sombra: Sombra.EdificioSuave);
        DressProp("Bairro_Casa_14", "Bairro/Lurdes", new Vector3(5.20f, 1.75f, 2.65f), heightFactor: .85f, sombra: Sombra.EdificioSuave);
        DressProp("Bairro_Casa_22", "Bairro/Casa22", new Vector3(5.25f, 1.75f, 2.65f), heightFactor: .85f, sombra: Sombra.EdificioSuave);
        DressProp("Bairro_Casa_32", "Bairro/Casa32", new Vector3(5.25f, 1.75f, 2.65f), heightFactor: .85f, sombra: Sombra.EdificioSuave);
        DressProp("Bairro_Casa_48", "Bairro/Casa48", new Vector3(5.15f, 1.75f, 2.65f), heightFactor: .85f, sombra: Sombra.EdificioSuave);
        DressProp("Van_Garagem_Casa14", "Bairro/Van", new Vector3(3.3f, .95f, 1.74f), sombra: Sombra.Veiculo);
        DressProp("Caixotes_Casa22", "Caixotes", new Vector3(.96f, .64f, .89f), sombra: Sombra.Contato);
        DressProp("CaixaCorreio_40_Sprite", "Bairro/Detalhes/CorreioDeMadeira", new Vector3(.56f, .42f, 1.13f), sombra: Sombra.Contato);
        DressProp("Seta_Subida_BarZe", null, new Vector3(.34f, .26f, 1.36f), sombra: Sombra.Contato);
        DressProp("NPC_SeuRamiro_Bairro", null, new Vector3(.42f, .35f, 1.49f), sombra: Sombra.Contato);
        foreach (string fence in new[] { "Cerca_Casa14", "Cerca_Casa22", "Cerca_Casa32", "Cerca_Avo_Esq", "Cerca_Avo_Dir" })
            DressProp(fence, "Entrada/Modulos/Muro", new Vector3(3.25f, .39f, .83f), scale: 1.28f, sombra: Sombra.EdificioSuave);
        for (int i = 1; i <= 5; i++)
            DressProp("Poste_Z4_" + i, "Poste", new Vector3(.12f, .15f, 2.6f), sombra: Sombra.ProjetadaSuave);

        ConfigurarTrilhasDoBairro();
        ComporMataDoBairro();
        ComporDetalhesDoBairro();
        InstalarBalizasDeTerra(new Vector2(418.22f, 3.2f), new Vector2(423.21f, 12.8f),
            new Vector2(427.9f, 20.06f), new Vector2(437.7f, 31.1f));

        foreach (var light in GetComponentsInChildren<Light2D>(true))
            light.enabled = false;

        lighting.SetVectorArray("_Lamps", new[]
        {
            Lamp("Poste_Z4_1", .66f, -.42f, 3.18f, 8.5f),
            Lamp("Poste_Z4_3", .66f, -.42f, 3.18f, 8.8f),
            Lamp("Poste_Z4_4", .66f, -.42f, 3.18f, 8.7f)
        });
        lighting.SetVectorArray("_LampColors", new[]
        {
            new Vector4(1.95f, 1.50f, .94f, 1),
            new Vector4(1.99f, 1.53f, .97f, 1),
            new Vector4(1.94f, 1.43f, .89f, 1)
        });
        lighting.SetVector("_FourthLamp", Lamp("Poste_Z4_2", .66f, -.42f, 3.18f, 8.15f));
        lighting.SetVector("_FifthLamp", Lamp("Poste_Z4_5", .66f, -.42f, 3.18f, 7.95f));
        InstalarPostesDoBairro();
        lighting.SetFloat("_DoorReflection", 0);
        UpdateCasters(false);
    }

    private void InstalarPostesDoBairro()
    {
        var pontos = new[]
        {
            new Vector2(408.65f, 10.95f), new Vector2(429.30f, 14.5f),
            new Vector2(412.78f, 22.53f), new Vector2(437.95f, 26.18f)
        };
        var luzes = new Vector4[pontos.Length];
        for (int i = 0; i < pontos.Length; i++)
        {
            string nome = "Poste complementar do Bairro " + (i + 1);
            var poste = ModuloDaRodovia(nome, "Poste", pontos[i],
                volume: new Vector3(.12f, .15f, 2.6f), pasta: Art);
            if (poste != null) luzes[i] = Lamp(nome, .66f, -.42f, 3.18f, 7.9f);
        }
        lighting.SetVectorArray("_BairroPostes", luzes);
    }

    private const string DetalhesDoBairro = Art + "Bairro/Detalhes/";

    private void ComporDetalhesDoBairro()
    {
        for (int rua = 0; rua < RuasDoBairro.Length; rua++)
        {
            var (inicio, fim, margem) = RuasDoBairro[rua];
            Vector2 eixo = (fim - inicio).normalized;
            Vector2 acostamento = new Vector2(-eixo.y, eixo.x);
            float angulo = Vector2.SignedAngle(Vector2.up, eixo);
            int i = 0;
            for (float d = 1.1f; d < Vector2.Distance(inicio, fim) - .8f; d += 2.05f + .42f * (i % 3), i++)
            {
                Vector2 centro = origin + inicio + eixo * d;
                float lado = (i + rua) % 2 == 0 ? 1 : -1;
                DecalqueDoBairro("CascalhoDaVila", centro + acostamento * lado * (margem - .34f),
                    .53f + .14f * (i % 4), angulo + 17f * (i % 3), .24f, lado < 0);
                if ((i + rua) % 4 == 0 && !PatioDoBairro(centro - acostamento * lado * margem))
                    DecalqueDoBairro("BeiraDeBarro", centro - acostamento * lado * (margem + .16f) + eixo * .35f,
                        .63f + .09f * (i % 3), angulo + (lado > 0 ? -90f : 90f), .25f, lado > 0);
                if ((i + rua) % 3 == 0)
                    DecalqueDoBairro("RastrosNaLama", centro + eixo * .30f, .83f + .10f * (i % 3),
                        angulo, .24f, lado < 0);
                else
                    DecalqueDoBairro("Folhico", centro + acostamento * lado * (margem + .36f),
                        .72f + .07f * (i % 3), angulo, .31f, lado < 0, PastaDaMata);
            }
        }

        var pontoDaPista = transform.Find("Pista_Rastros_Bairro");
        if (pontoDaPista != null)
        {
            var rastro = DecalqueDoBairro("RastrosNaLama", pontoDaPista.position, 1.38f, -79f, .62f, false);
            pontoDaPista.GetComponent<InteracaoCapitulo1>().VincularIndicador(rastro);
        }

        foreach (var (x, y, rx, ry, tilt, _, _) in PocasDoBairro)
            DecalqueDoBairro("BordaDePoca", origin + new Vector2(x, y), Mathf.Max(rx, ry),
                rx > ry ? -6f + tilt * 24f : 88f + tilt * 24f, .23f, tilt < 0);

        foreach (var (x, y) in new (float x, float y)[] { (410.35f, 10.55f), (431.65f, 13.68f), (414.15f, 21.95f),
                     (423.5f, 22.3f), (439.55f, 25.77f) })
            DecalqueDoBairro("SoleiraDePedra", new Vector2(x, y), .76f, 0, .54f, x < 420f);

        ModuloDaRodovia("Drenagem do Bairro · entrada", "BueiroDaVila", new Vector2(422.45f, 4.21f),
            .78f, volume: new Vector3(.65f, .31f, .32f), pasta: DetalhesDoBairro);
        ModuloDaRodovia("Drenagem do Bairro · curva", "BueiroDaVila", new Vector2(432.55f, 23.82f),
            .74f, flip: true, volume: new Vector3(.61f, .28f, .28f), pasta: DetalhesDoBairro);

        foreach (var (jardim, x, y) in new (string, float, float)[] { ("Jardim_Casa14", 411.66f, 12.25f),
                     ("Jardim_Casa22", 432.92f, 15.73f), ("Jardim_Casa32", 412.95f, 23.39f),
                     ("Jardim_Avo_40", 425.41f, 23.86f) })
        {
            var vasos = ModuloDaRodovia("Vasos na varanda", "VasosDaVaranda", new Vector2(x, y),
                .87f, flip: x > 420f, volume: new Vector3(.45f, .30f, .43f), pasta: DetalhesDoBairro);
            if (vasos != null) transform.Find(jardim)?.gameObject.SetActive(false);
        }
    }

    private SpriteRenderer DecalqueDoBairro(string arte, Vector2 pos, float escala, float angulo, float opacidade,
        bool flip, string pasta = DetalhesDoBairro)
    {
        var decalque = ModuloDaRodovia("Piso do Bairro · " + arte, arte, pos, escala, flip,
            plano: true, tinta: new Color(.86f, .86f, .83f, opacidade), pasta: pasta);
        if (decalque != null) decalque.transform.localRotation = Quaternion.Euler(0, 0, angulo);
        return decalque;
    }

    private void ComporMataDoBairro()
    {
        string[] especies = { "Camboata", "Bracatinga", "Pitangueira", "Camboata", "Araucaria" };
        var sorteio = new System.Random(317);
        for (int linha = 0; linha < 17; linha++)
        for (int coluna = 0; coluna < 22; coluna++)
        {
            var raiz = new Vector2(.6f + coluna * 2.04f + (linha % 2) * .82f,
                .5f + linha * 2.02f);
            raiz += new Vector2((float)sorteio.NextDouble() - .5f, (float)sorteio.NextDouble() - .5f) * .92f;
            float margem = DistanciaDaRuaDoBairro(raiz);
            if (margem < .86f || DentroDeLoteDoBairro(raiz)) continue;

            bool espelhar = sorteio.Next(2) == 0;
            string especie = margem < 1.5f
                ? (espelhar ? "CamboataJovem" : "BracatingaJovem")
                : especies[sorteio.Next(especies.Length)];
            Vector2 pe = origin + raiz;
            Plant("Mata do Bairro " + linha + " " + coluna, especie, pe.x, pe.y,
                .91f + (float)sorteio.NextDouble() * .23f, espelhar, temTroncoSolido: false);
            InserirSubmata(espelhar ? "MoitaDeBorda" : "SamambaiaDaMata",
                pe + new Vector2(espelhar ? .44f : -.48f, -.34f), .86f, !espelhar);
        }

        foreach (var pe in new[] { new Vector2(405.8f, 12.2f), new Vector2(417.7f, 23.68f),
                     new Vector2(428.54f, 23.85f), new Vector2(434.73f, 15.1f), new Vector2(442f, 26.55f) })
            InserirSubmata("MoitaDeBorda", pe, .93f, pe.x > 425);
    }

    private static bool DentroDeLoteDoBairro(Vector2 posicao)
    {
        foreach (var patio in PatiosDoBairro)
        {
            var lote = Rect.MinMaxRect(patio.xMin - .64f, patio.yMin - .46f,
                patio.xMax + .64f, patio.yMax + 3.9f);
            if (lote.Contains(posicao)) return true;
        }
        return posicao.x >= 13.1f && posicao.x <= 17.1f && posicao.y >= 10f && posicao.y <= 14.5f;
    }
}

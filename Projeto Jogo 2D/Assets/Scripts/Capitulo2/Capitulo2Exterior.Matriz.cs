using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

internal sealed partial class Capitulo2Exterior
{
    private const string Matriz = Art + "Matriz/";
    private const float CentroEixoX = 46.0f;
    private const float QuadroX = 38.85f;
    private const float OrelhaoX = 53.15f;
    private const float MobiliaDoAdroY = 60.40f;
    private const int LimiteLuzesCentro = 32;
    private const int LimiteOclusoresCentro = 96;
    private const float DensidadeNevoaCentro = .30f;
    private int indiceLuzEntradaMatriz = -1;
    private static readonly Color Ambar = new(1f, .74f, .47f);
    private readonly List<(Vector3 pos, float raio, Color cor)> luzesVolumetricasPraca = new();
    private readonly List<Vector4> oclusoresMin = new();
    private readonly List<Vector4> oclusoresMax = new();
    private readonly Vector4[] bufferMin = new Vector4[LimiteOclusoresCentro];
    private readonly Vector4[] bufferMax = new Vector4[LimiteOclusoresCentro];
    private readonly List<(SpriteRenderer sprite, Bounds bounds, int oclusor, float inclinacao, float profundidade, float densidade)>
        silhuetasDoCentro = new();
    private const int TamanhoRecorte = 18;
    private Material projetorDaPraca;
    private Mesh malhaDeRecorte;
    private RenderTexture recortesDosPostes;
    private RenderTexture recorteDaLanterna;
    private MaterialPropertyBlock propriedadesDoRecorte;
    private SpriteRenderer nevoaSobreMarcus;
    private MaterialPropertyBlock propriedadesNevoaMarcus;
    private readonly Dictionary<Material, Material> materiaisDaNevoaCentro = new();
    private MaterialPropertyBlock propriedadesNevoaCenario;
    private float ultimaDensidadeNevoaCenario = -1f;
    private Vector4 ultimoRecorteDaLanterna;
    private int indiceOclusorMarcus = -1;

    private static bool NaPraca(Vector2 p) =>
        p.x >= 34f && p.x <= 58f && p.y >= 39f && p.y <= 70f;

    private static bool NoCanteiroDaPraca(int x, int y) =>
        ((x >= 37 && x < 41) || (x >= 51 && x < 55)) &&
        ((y >= 43 && y < 47) || (y >= 54 && y < 58));

    private void ComporPraca(SpriteRenderer[] existentes)
    {
        int bancoIdx = 0;
        Vector2[] posicoesBancos =
        {
            new(41.45f, 50.45f),
            new(50.55f, 50.45f),
            new(41.45f, 61.10f),
            new(50.55f, 61.10f)
        };

        // Reposiciona gatilhos, portais e NPCs da praça em simetria ao redor de x = 46.0f,
        // mantendo todos os IDs e fluxos narrativos intactos.
        foreach (var tr in FindObjectsByType<Transform>())
        {
            if (tr == null || tr.gameObject.scene != gameObject.scene) continue;
            switch (tr.name)
            {
                case "Porta_Porta_IgrejaMatriz":
                    tr.position = new Vector3(CentroEixoX, 63.35f, tr.position.z);
                    break;
                case "Spawn_volta_IgrejaMatriz":
                    tr.position = new Vector3(CentroEixoX, 61.45f, tr.position.z);
                    break;
                case "Examinar_avisos_praca":
                    tr.position = new Vector3(QuadroX, MobiliaDoAdroY - .40f, tr.position.z);
                    break;
                case "Examinar_orientacao":
                    tr.position = new Vector3(43.20f, 42.10f, tr.position.z);
                    break;
                case "NPC_chico":
                    tr.position = new Vector3(42.55f, 42.35f, tr.position.z);
                    break;
                case "MoradorNoBanco":
                    tr.position = new Vector3(41.10f, 50.52f, tr.position.z);
                    break;
                case "VizinhaNaPraca":
                    tr.position = new Vector3(41.80f, 50.52f, tr.position.z);
                    break;
                case "Examinar_moradores_praca":
                    tr.position = new Vector3(41.45f, 50.10f, tr.position.z);
                    break;
                case "AO_Igreja":
                case "AO_FontePublica":
                case "AO_QuadroAvisosDaPraca":
                case "AO_orelhao_centro_matriz":
                    tr.gameObject.SetActive(false);
                    break;
            }
        }

        foreach (var sprite in existentes)
        {
            if (sprite == null || sprite.gameObject.scene != gameObject.scene || sprite.sprite == null) continue;
            Vector2 p = sprite.transform.position;
            if (!NaPraca(p)) continue;

            string nome = sprite.name;
            if (nome.StartsWith("BancoPraca_", StringComparison.Ordinal) ||
                nome.StartsWith("AO_BancoPraca_", StringComparison.Ordinal) ||
                nome.StartsWith("AO_", StringComparison.Ordinal) ||
                nome is "Flores" or "FloresDoCanteiro" or "JardimCuidado")
            {
                sprite.gameObject.SetActive(false);
                continue;
            }

            if (nome == "Igreja")
            {
                // Desativa o ShadowCaster2D legado (ShadowFootprint) que projetava uma faixa
                // retangular escura sobre a metade inferior da porta da Igreja.
                foreach (var caster in sprite.GetComponentsInChildren<ShadowCaster2D>(true))
                    caster.enabled = false;
                foreach (Transform child in sprite.transform)
                    if (child.name is "ShadowFootprint" or "AO_Igreja")
                        child.gameObject.SetActive(false);

                sprite.transform.position = new Vector3(CentroEixoX, 64.55f, sprite.transform.position.z);
                Trocar(sprite, Matriz + "Fachada", 8.35f);
                sprite.sortingOrder = 1000 - Mathf.RoundToInt(64.55f * 10);
                var box = sprite.GetComponent<BoxCollider2D>();
                if (box != null)
                {
                    float s = sprite.transform.localScale.x;
                    box.size = new Vector2(7.70f / s, 3.60f / s);
                    box.offset = new Vector2(0f, 1.15f / s);
                }
                ConfigurarSpriteVolumetricoPraca(sprite, 63.52f, .86f, 2.85f, .96f, .42f, modoOclusor: 0);
                continue;
            }

            if (nome == "FontePublica")
            {
                foreach (var caster in sprite.GetComponentsInChildren<ShadowCaster2D>(true))
                    caster.enabled = false;
                sprite.transform.position = new Vector3(CentroEixoX, 50.45f, sprite.transform.position.z);
                sprite.sortingOrder = 1000 - Mathf.RoundToInt(50.45f * 10);
                float baseFonte = sprite.bounds.min.y + 1f / 32f;
                ConfigurarSpriteVolumetricoPraca(sprite, baseFonte, .78f, 1.85f, 2.25f, .92f, modoOclusor: 3);
                RegistrarLuzVolumetricaPraca("Reflexo frio da água da fonte",
                    new Vector3(CentroEixoX, baseFonte + .42f, 1.10f), 2.8f, .12f,
                    new Color(.56f, .72f, .88f), new Color(.20f, .29f, .40f));
                FontePracaAnimada.Instalar(sprite, controladorMarcus != null ? controladorMarcus.transform : null);
                continue;
            }

            if (nome == "BancoPublico")
            {
                Vector2 destino = bancoIdx < posicoesBancos.Length
                    ? posicoesBancos[bancoIdx++]
                    : new Vector2(p.x < CentroEixoX ? 41.45f : 50.55f, p.y);
                sprite.transform.position = new Vector3(destino.x, destino.y, sprite.transform.position.z);
                Trocar(sprite, Matriz + "BancoFerro", 2.15f);
                sprite.sortingOrder = 1000 - Mathf.RoundToInt(destino.y * 10);
                ConfigurarSpriteVolumetricoPraca(sprite, destino.y - .32f, .72f, .92f, .62f, .36f, modoOclusor: 5);
                continue;
            }

            if (nome == "QuadroAvisosDaPraca")
            {
                sprite.transform.position = new Vector3(QuadroX, MobiliaDoAdroY, sprite.transform.position.z);
                Trocar(sprite, pasta + "Quadro", 1.63f);
                sprite.sortingOrder = 1000 - Mathf.RoundToInt(MobiliaDoAdroY * 10);
                ConfigurarSpriteVolumetricoPraca(sprite, MobiliaDoAdroY - .60f, .82f, 1.55f, .52f, .34f, modoOclusor: 5);
                continue;
            }

            if (nome == "orelhao_centro_matriz")
            {
                sprite.transform.position = new Vector3(OrelhaoX, MobiliaDoAdroY, sprite.transform.position.z);
                sprite.sortingOrder = 1000 - Mathf.RoundToInt(MobiliaDoAdroY * 10);
                ConfigurarSpriteVolumetricoPraca(sprite, MobiliaDoAdroY - .70f, .88f, 2.05f, .58f, .44f, modoOclusor: 5);
                continue;
            }

            if (nome == "LixeiraDaPraca")
            {
                sprite.transform.position = new Vector3(49.45f, 42.35f, sprite.transform.position.z);
                Trocar(sprite, "Art/RuaNorte/Entrada/Modulos/Lixeira", .72f);
                sprite.sortingOrder = 1000 - Mathf.RoundToInt(42.35f * 10);
                ConfigurarSpriteVolumetricoPraca(sprite, 42.02f, .76f, .88f, .42f, .38f, modoOclusor: 5);
                continue;
            }

            if (nome.StartsWith("Arvore", StringComparison.Ordinal) ||
                nome.StartsWith("Pinheiro", StringComparison.Ordinal) ||
                nome.StartsWith("Arbusto", StringComparison.Ordinal) ||
                nome.StartsWith("Moita", StringComparison.Ordinal) ||
                nome is "Camboata" or "Pitangueira" or "Bracatinga" or "Araucaria" or "CamboataJovem" or "BracatingaJovem")
            {
                // Desativa as árvores antigas espalhadas dentro da praça para não tapar a visão da Matriz e do adro.
                sprite.gameObject.SetActive(false);
                continue;
            }

            if (nome is "NPC_chico" or "MoradorNoBanco" or "VizinhaNaPraca")
            {
                Vector2 posNpc = sprite.transform.position;
                sprite.sortingOrder = 1000 - Mathf.RoundToInt(posNpc.y * 10);
                ConfigurarSpriteVolumetricoPraca(sprite, posNpc.y - .45f, .84f, 1.55f, .44f, .34f, modoOclusor: 5, projetarSilhueta: false);
            }
        }

        foreach (var light in FindObjectsByType<Light2D>())
        {
            if (light.gameObject.scene == gameObject.scene && NaPraca(light.transform.position) &&
                (light.name is "LuzQuente" or "LuminariaDeUso"))
                light.enabled = false;
        }

        // Mantém o limiar livre para a porta continuar totalmente visível e acessível.
        var votivos = PecaPraca("Votivos sob a janela", "Votivos", CentroEixoX + 2.32f, 64.38f, .74f);
        if (votivos != null)
        {
            votivos.sortingOrder = 1000 - Mathf.RoundToInt(64.55f * 10) + 2;
            ConfigurarSpriteVolumetricoPraca(votivos, 63.55f, .82f, 1.25f, 0f, 0f, modoOclusor: -1);
        }

        foreach (float x in new[] { CentroEixoX - 3.95f, CentroEixoX + 3.95f })
        {
            var vaso = PecaPraca("Floreira junto à Matriz", "VasoFolhagem", x, 63.48f, 1.32f);
            OcuparBasePraca(vaso, new Vector2(1.05f, .42f));
            ConfigurarSpriteVolumetricoPraca(vaso, 63.10f, .78f, 1.18f, .92f, .44f, modoOclusor: 5);
            RegistrarLuzVolumetricaPraca("Luz rasante da Matriz", new Vector3(x, 63.15f, 1.65f), 4.8f, .48f,
                new Color(1f, .81f, .60f), new Color(1.28f, .99f, .70f));
        }
        indiceLuzEntradaMatriz = luzesVolumetricasPraca.Count;
        RegistrarLuzVolumetricaPraca("Luz da entrada da Matriz", new Vector3(CentroEixoX, 63.05f, 2.15f), 8.8f, 1.20f,
            Ambar, new Color(3.00f, 2.08f, 1.30f));
        RegistrarLuzVolumetricaPraca("Luz difusa da fachada da Matriz", new Vector3(CentroEixoX, 66.25f, 1.55f),
            5.6f, .42f, new Color(1f, .84f, .64f), new Color(1.42f, 1.08f, .76f));
        RegistrarLuzVolumetricaPraca("Contraluz fria do bosque da Matriz", new Vector3(CentroEixoX, 71.8f, 2.6f),
            9.4f, .25f, new Color(.48f, .60f, .84f), new Color(.58f, .73f, 1.04f));

        // Luzes do quadro de avisos e do orelhão (com balizadores de proteção idênticos ao Cap. 1).
        RegistrarLuzVolumetricaPraca("Luz do quadro de avisos", new Vector3(QuadroX, MobiliaDoAdroY - .40f, 1.95f), 4.2f, .52f,
            new Color(1f, .78f, .52f), new Color(1.42f, 1.06f, .68f));
        RegistrarLuzVolumetricaPraca("Luz do orelhão da Matriz", new Vector3(OrelhaoX, MobiliaDoAdroY - .40f, 2.15f), 4.6f, .62f,
            new Color(.96f, .82f, .58f), new Color(1.52f, 1.18f, .78f));
        foreach (float dx in new[] { -.58f, .58f })
        {
            var balizaOrelhao = PecaPraca("Balizador do orelhão", "Art/RuaNorte/Entrada/Modulos/Balizadores",
                OrelhaoX + dx, MobiliaDoAdroY - .53f, .34f);
            if (balizaOrelhao != null)
            {
                ConfigurarSpriteVolumetricoPraca(balizaOrelhao, MobiliaDoAdroY - .73f, .75f, .68f, .28f, .24f, modoOclusor: 5);
            }
        }

        // Muretas de pedra e pilares na divisa norte da praça (y = 63.95f), ladeando a Igreja como no Cap. 1.
        ComporMuretasDaMatriz();

        // Quatro canteiros simétricos com bordas de granito, árvores do Cap. 1, arbustos, samambaias e folhiço.
        ComporCanteirosEMatagalDaPraca();

        // Balizadores de ferro fora do caminho central, alinhados às bordas do eixo de tijolinho e do adro.
        foreach (float y in new[] { 42.60f, 46.60f, 54.40f, 58.60f })
        {
            BalizadorPraca(42.82f, y);
            BalizadorPraca(49.18f, y);
        }
        BalizadorPraca(40.15f, 61.60f);
        BalizadorPraca(51.85f, 61.60f);

        // Quatro postes altos simétricos nas alamedas laterais da praça.
        PostePraca(35.55f, 44.60f, true, "Poste sudoeste da Matriz");
        PostePraca(56.45f, 44.60f, false, "Poste sudeste da Matriz");
        PostePraca(35.55f, 57.60f, true, "Poste noroeste da Matriz");
        PostePraca(56.45f, 57.60f, false, "Poste nordeste da Matriz");

        // Drenagem pluvial (grelhas e ralos), folhas acumuladas, manchas de terra úmida e pedregulhos soltos.
        ComporDrenagemEDetalhesDoPiso();
    }

    private void ComporMuretasDaMatriz()
    {
        const string modulos = "Art/RuaNorte/Entrada/Modulos/";
        float yMuro = 63.88f;

        // Ala oeste (x = 35.2 a 41.7) e ala leste (x = 50.3 a 56.8), simétricas em relação a x = 46.0f.
        (float xPilarIni, float xMuro1, float xMuro2, float xPilarFim, float xDreno)[] alas =
        {
            (35.25f, 36.95f, 39.85f, 41.55f, 37.35f),
            (56.75f, 55.05f, 52.15f, 50.45f, 54.65f)
        };

        foreach (var ala in alas)
        {
            foreach (float xMuro in new[] { ala.xMuro1, ala.xMuro2 })
            {
                var muro = PecaPraca("Mureta de pedra da Matriz", "MuretaPedra", xMuro, yMuro, 2.88f);
                if (muro != null)
                {
                    OcuparBasePraca(muro, new Vector2(2.72f, .48f));
                    ConfigurarSpriteVolumetricoPraca(muro, yMuro - .38f, .76f, 1.18f, 2.68f, .46f, modoOclusor: 5);
                }
                var hera = PecaPraca("Hera sobre a mureta", modulos + "HeraDoMuro", xMuro + .18f, yMuro + .14f, 1.35f);
                if (hera != null)
                {
                    hera.sortingOrder = (muro != null ? muro.sortingOrder : 360) + 1;
                    ConfigurarSpriteVolumetricoPraca(hera, yMuro - .28f, .76f, 1.05f, 0f, 0f, modoOclusor: -1);
                }
            }

            foreach (float xPilar in new[] { ala.xPilarIni, ala.xPilarFim })
            {
                var pilar = PecaPraca("Pilar de pedra da Matriz", modulos + "Pilar", xPilar, yMuro + .04f, .82f);
                if (pilar != null)
                {
                    pilar.sortingOrder = 1000 - Mathf.RoundToInt((yMuro - .08f) * 10);
                    OcuparBasePraca(pilar, new Vector2(.68f, .52f));
                    ConfigurarSpriteVolumetricoPraca(pilar, yMuro - .40f, .82f, 1.42f, .68f, .50f, modoOclusor: 5);
                }
            }

            var dreno = PecaPraca("Dreno de pedra da mureta", modulos + "DrenoDeMuro", ala.xDreno, yMuro - .22f, .62f, noPiso: true);
            if (dreno != null)
            {
                dreno.sortingOrder = 1000 - Mathf.RoundToInt((yMuro - .04f) * 10);
                ConfigurarSpriteVolumetricoPraca(dreno, yMuro - .38f, 0f, .25f, 0f, 0f, modoOclusor: -1);
            }
        }

        // Muretas baixas nas quinas sul da praça (x = 35.6..36.8 e x = 55.2..56.4 em y = 40.55f),
        // emoldurando a entrada da praça sem bloquear os passeios nem o eixo central.
        foreach (float xQuina in new[] { 36.25f, 55.75f })
        {
            var muretaSul = PecaPraca("Mureta de entrada da praça", modulos + "Muro", xQuina, 40.62f, 1.95f);
            if (muretaSul != null)
            {
                OcuparBasePraca(muretaSul, new Vector2(1.75f, .38f));
                ConfigurarSpriteVolumetricoPraca(muretaSul, 40.32f, .72f, .88f, 1.72f, .38f, modoOclusor: 5);
            }
            float dirPilar = xQuina < CentroEixoX ? 1.02f : -1.02f;
            var pilarSul = PecaPraca("Pilar de entrada da praça", modulos + "Pilar", xQuina + dirPilar, 40.65f, .68f);
            if (pilarSul != null)
            {
                pilarSul.sortingOrder = 1000 - Mathf.RoundToInt(40.55f * 10);
                OcuparBasePraca(pilarSul, new Vector2(.56f, .42f));
                ConfigurarSpriteVolumetricoPraca(pilarSul, 40.30f, .78f, 1.12f, .54f, .42f, modoOclusor: 5);
            }
        }
    }

    private void ComporCanteirosEMatagalDaPraca()
    {
        const string veg = "Art/RuaNorte/Vegetacao/";
        const string centroProps = Art + "Centro/";

        // Quatro canteiros simétricos: vegetação baixa (floreiras, moitas e samambaias) para manter
        // toda a visão da Praça da Matriz, bancos, orelhão, quadro e Igreja 100% limpa e desobstruída.
        (float cx, float cy, bool oeste, bool norte)[] canteiros =
        {
            (39.0f, 45.0f, true, false),
            (53.0f, 45.0f, false, false),
            (39.0f, 56.0f, true, true),
            (53.0f, 56.0f, false, true)
        };

        foreach (var (cx, cy, oeste, norte) in canteiros)
        {
            var curvo = PecaPraca("Canteiro curvo de granito", "CanteiroCurvo", cx, cy - 1.45f, 2.65f);
            if (curvo != null)
            {
                OcuparBasePraca(curvo, new Vector2(2.30f, .44f));
                ConfigurarSpriteVolumetricoPraca(curvo, cy - 1.75f, .74f, .86f, 2.25f, .44f, modoOclusor: 5);
            }

            var vasoNorte = PecaPraca("Floreira norte do canteiro", centroProps + (norte ? "Canteiro" : "Floreira"),
                cx + (oeste ? -.75f : .75f), cy + 1.25f, norte ? 1.32f : .88f);
            if (vasoNorte != null)
            {
                OcuparBasePraca(vasoNorte, new Vector2(norte ? 1.08f : .68f, .36f));
                ConfigurarSpriteVolumetricoPraca(vasoNorte, cy + 1.02f, .74f, .82f, .90f, .38f, modoOclusor: 5);
            }

            var moitaExterna = PecaPraca("Moita externa do canteiro", veg + "MoitaDeBorda",
                cx + (oeste ? -.88f : .88f), cy + .32f, 1.18f);
            if (moitaExterna != null)
            {
                ConfigurarVegetacaoVolumetricaPraca(moitaExterna, "MoitaDeBorda",
                    new Vector2(cx + (oeste ? -.88f : .88f), cy + .32f), .62f);
            }

            var moita = PecaPraca("Moita baixa do canteiro", veg + "MoitaDeBorda",
                cx + (oeste ? .62f : -.62f), cy + .25f, 1.15f);
            if (moita != null)
            {
                ConfigurarVegetacaoVolumetricaPraca(moita, "MoitaDeBorda", new Vector2(cx, cy + .25f), .62f);
            }

            var samambaia = PecaPraca("Samambaia da mata no canteiro", veg + "SamambaiaDaMata",
                cx + (oeste ? -.82f : .82f), cy - .55f, 1.04f);
            if (samambaia != null)
            {
                ConfigurarVegetacaoVolumetricaPraca(samambaia, "SamambaiaDaMata", new Vector2(cx, cy - .55f), .58f);
            }

            var samambaia2 = PecaPraca("Samambaia interna do canteiro", veg + "SamambaiaDaMata",
                cx + (oeste ? .55f : -.55f), cy - .78f, .92f);
            if (samambaia2 != null)
            {
                ConfigurarVegetacaoVolumetricaPraca(samambaia2, "SamambaiaDaMata", new Vector2(cx, cy - .78f), .55f);
            }

            var folhico = PecaPraca("Folhiço do canteiro", veg + "Folhico", cx, cy - .15f, 1.48f, noPiso: true);
            if (folhico != null)
                ConfigurarSpriteVolumetricoPraca(folhico, cy - .15f, 0f, .15f, 0f, 0f, modoOclusor: -1);
        }

        // Copas em camadas atrás da fachada, com os troncos afastados da entrada e das alamedas.
        (string especie, float x, float y, float largura)[] bosqueNorte =
        {
            ("CamboataJovem", 36.2f, 66.5f, 2.15f),
            ("Pitangueira", 38.6f, 67.0f, 2.25f),
            ("Pitangueira", 53.4f, 67.0f, 2.25f),
            ("BracatingaJovem", 55.8f, 66.5f, 2.15f),
            ("Pitangueira", 40.7f, 65.85f, 3.25f),
            ("CamboataJovem", 42.6f, 67.55f, 2.75f),
            ("Bracatinga", 50.2f, 66.1f, 3.5f),
            ("Camboata", 52.3f, 67.9f, 3.15f),
            ("Pitangueira", 55.5f, 68.65f, 2.7f),
            ("Camboata", 36.9f, 69.2f, 3.1f),
            ("Bracatinga", 40.5f, 68.7f, 3.6f),
            ("Pitangueira", 43.1f, 70.4f, 3.1f),
            ("Camboata", 46.2f, 70.0f, 3.7f),
            ("Araucaria", 48.8f, 72.0f, 2.7f),
            ("Bracatinga", 51.1f, 69.0f, 3.5f),
            ("Camboata", 54.6f, 69.7f, 3.3f)
        };
        PlantarArvoresNoGramado("Árvore de fundo da Matriz", bosqueNorte);

        // Segunda camada de copas no fundo: reforça o enquadramento da fachada sem pôr troncos
        // na alameda central. A variação de espécies mantém a silhueta da mata irregular.
        (string especie, float x, float y, float largura)[] bosqueDistante =
        {
            ("BracatingaJovem", 37.2f, 73.1f, 2.45f),
            ("Pitangueira", 40.8f, 74.0f, 2.85f),
            ("CamboataJovem", 44.0f, 73.6f, 2.55f),
            ("Bracatinga", 48.2f, 74.2f, 3.05f),
            ("Pitangueira", 51.8f, 73.4f, 2.75f)
        };
        PlantarArvoresNoGramado("Árvore distante atrás da Matriz", bosqueDistante);

        // Matagal rasteiro (MoitaDeBorda e SamambaiaDaMata) rente à mureta norte e nas quinas sul.
        (string arte, float x, float y, float largura)[] matagalMureta =
        {
            ("MoitaDeBorda", 36.2f, 64.42f, 1.32f),
            ("SamambaiaDaMata", 38.2f, 64.35f, 1.12f),
            ("MoitaDeBorda", 40.3f, 64.40f, 1.22f),
            ("MoitaDeBorda", 51.7f, 64.40f, 1.22f),
            ("SamambaiaDaMata", 53.8f, 64.35f, 1.12f),
            ("MoitaDeBorda", 55.8f, 64.42f, 1.32f),
            ("SamambaiaDaMata", 36.2f, 63.40f, .88f),
            ("SamambaiaDaMata", 40.7f, 63.40f, .84f),
            ("SamambaiaDaMata", 51.3f, 63.40f, .84f),
            ("SamambaiaDaMata", 55.8f, 63.40f, .88f),
            ("SamambaiaDaMata", 36.25f, 40.22f, .86f),
            ("SamambaiaDaMata", 55.75f, 40.22f, .86f)
        };
        foreach (var (arte, x, y, largura) in matagalMureta)
        {
            var m = PecaPraca("Matagal de borda da Matriz", veg + arte, x, y, largura);
            if (m != null)
                ConfigurarVegetacaoVolumetricaPraca(m, arte, new Vector2(x, y), .65f);
        }
    }

    private void ComporDrenagemEDetalhesDoPiso()
    {
        const string modulos = "Art/RuaNorte/Entrada/Modulos/";

        // Grelhas e ralos de sarjeta nos encontros entre o passeio, o asfalto e o adro.
        (string arte, float x, float y, float largura)[] drenagem =
        {
            (modulos + "Grelha", 34.82f, 41.20f, .64f),
            (modulos + "Grelha", 57.18f, 41.20f, .64f),
            (modulos + "Grelha", 34.82f, 52.80f, .64f),
            (modulos + "Grelha", 57.18f, 52.80f, .64f),
            (Art + "Centro/Ralo", 42.65f, 40.25f, .58f),
            (Art + "Centro/Ralo", 49.35f, 40.25f, .58f),
            (Art + "Centro/Ralo", 39.85f, 62.85f, .58f),
            (Art + "Centro/Ralo", 52.15f, 62.85f, .58f)
        };
        foreach (var (arte, x, y, largura) in drenagem)
        {
            var ralo = PecaPraca("Drenagem da praça", arte, x, y, largura, noPiso: true);
            if (ralo != null)
                ConfigurarSpriteVolumetricoPraca(ralo, y, 0f, .1f, 0f, 0f, modoOclusor: -1);
        }

        // Folhas caídas, manchas de terra úmida e pedrinhas ao pé dos bancos, muretas e meios-fios.
        (string arte, float x, float y, float largura)[] detalhesPiso =
        {
            (modulos + "Folhas", 36.8f, 63.35f, .88f),
            (modulos + "Folhas", 40.4f, 63.32f, .82f),
            (modulos + "Folhas", 51.6f, 63.32f, .82f),
            (modulos + "Folhas", 55.2f, 63.35f, .88f),
            (modulos + "Folhas", 41.45f, 49.95f, .78f),
            (modulos + "Folhas", 50.55f, 49.95f, .78f),
            (modulos + "Folhas", 37.2f, 43.2f, .84f),
            (modulos + "Folhas", 54.8f, 43.2f, .84f),
            (modulos + "Folhas", 37.2f, 57.8f, .84f),
            (modulos + "Folhas", 54.8f, 57.8f, .84f),
            (modulos + "Pedras", 35.8f, 50.2f, .72f),
            (modulos + "Pedras", 56.2f, 50.2f, .72f),
            (modulos + "Pedras", 42.3f, 59.1f, .68f),
            (modulos + "Pedras", 49.7f, 59.1f, .68f),
            (modulos + "Terra", 38.4f, 63.38f, 1.12f),
            (modulos + "Terra", 53.6f, 63.38f, 1.12f)
        };
        foreach (var (arte, x, y, largura) in detalhesPiso)
        {
            var det = PecaPraca("Detalhe de piso da Matriz", arte, x, y, largura, noPiso: true);
            if (det != null)
                ConfigurarSpriteVolumetricoPraca(det, y, 0f, .1f, 0f, 0f, modoOclusor: -1);
        }
    }

    private SpriteRenderer PecaPraca(string nome, string arte, float x, float y, float largura,
        bool noPiso = false)
    {
        var r = Prop(nome, arte.StartsWith("Art/", StringComparison.Ordinal) ? arte : Matriz + arte,
            new Vector2(x, y), noPiso);
        if (r == null) return null;
        r.transform.localScale = Vector3.one * (largura / r.sprite.bounds.size.x);
        return r;
    }

    private void PostePraca(float x, float y, bool ladoDireito, string nome = "Poste da alameda")
    {
        if (GramaNoCentro(new Vector2(x, y))) return;
        var r = PecaPraca(nome, "Art/RuaNorte/Poste", x, y, 1.08f);
        if (r == null) return;
        r.flipX = !ladoDireito;
        OcuparBasePraca(r, new Vector2(.28f, .28f));
        Vector2 lamp = SantaRitaRuaNorte.BocalDoPoste(r);
        fontes.Add(lamp);
        float baseY = r.bounds.min.y;
        float alturaDoBocal = lamp.y - baseY;
        ConfigurarSpriteVolumetricoPraca(r, baseY, 1f, alturaDoBocal, .24f, .24f, modoOclusor: 5,
            superficie: Resources.Load<Texture2D>("Art/RuaNorte/Entrada/Relevos/Poste"));
        luzesVolumetricasPraca.Add((new Vector3(lamp.x, baseY, alturaDoBocal), 8.6f,
            new Color(1.92f, 1.48f, .96f)));
        LuzPraca("Luz · " + nome, lamp - Vector2.up * 1.65f, 4.9f, .72f, Ambar);
    }

    private void BalizadorPraca(float x, float y)
    {
        if (GramaNoCentro(new Vector2(x, y))) return;
        var r = PecaPraca("Balizador de ferro da praça", "BalizadorFerro", x, y, .40f);
        if (r == null) return;
        OcuparBasePraca(r, new Vector2(.26f, .26f));
        var pe = new Vector2(x, y);
        ConfigurarSpriteVolumetricoPraca(r, y - .18f, .82f, .86f, .24f, .24f, modoOclusor: 5);
        luzesVolumetricasPraca.Add((new Vector3(x, y - .10f, .82f), 3.35f, new Color(1.58f, 1.14f, .68f)));
        LuzPraca("Luz baixa do passeio", pe + Vector2.up * .48f, 2.45f, .44f, Ambar);
    }

    private void RegistrarLuzVolumetricaPraca(string nome, Vector3 mundoXYZ, float raio, float potencia2D,
        Color cor2D, Color corShader)
    {
        luzesVolumetricasPraca.Add((mundoXYZ, raio, corShader));
        LuzPraca(nome, new Vector2(mundoXYZ.x, mundoXYZ.y + 1.15f), raio * .78f, potencia2D, cor2D);
    }

    private void ConfigurarSpriteVolumetricoPraca(SpriteRenderer r, float baseY, float inclinacao,
        float altura3D, float larguraOclusor, float profOclusor, int modoOclusor, Texture2D superficie = null, bool projetarSilhueta = true)
    {
        if (r == null || materialDoPiso == null) return;
        r.sharedMaterial = materialDoPiso;
        int ownerId = 0;
        if (modoOclusor >= 0 && oclusoresMin.Count < LimiteOclusoresCentro - 1)
        {
            Vector2 centro = r.bounds.center;
            Vector2 min = new(centro.x - larguraOclusor * .5f - origemDoPiso.x, baseY - profOclusor * .5f - origemDoPiso.y);
            Vector2 max = new(centro.x + larguraOclusor * .5f - origemDoPiso.x, baseY + profOclusor * .5f - origemDoPiso.y);
            oclusoresMin.Add(new Vector4(min.x, min.y, 0f, modoOclusor));
            oclusoresMax.Add(new Vector4(max.x, max.y, altura3D, 0f));
            ownerId = oclusoresMin.Count;
        }
        var bloco = new MaterialPropertyBlock();
        r.GetPropertyBlock(bloco);
        if (r.sprite != null && r.sprite.texture != null)
            bloco.SetTexture("_MainTex", r.sprite.texture);
        bloco.SetVector("_Receiver", new Vector4(baseY, inclinacao, ownerId, 0f));
        bloco.SetVector("_Wind", Vector4.zero);
        bloco.SetVector("_CanopyTone", Vector4.one);
        if (superficie != null) bloco.SetTexture("_PropSurface", superficie);
        bloco.SetFloat("_PropSurfaceEnabled", superficie != null ? 1f : 0f);
        r.SetPropertyBlock(bloco);
        if (inclinacao > 0f && projetarSilhueta)
            RegistrarSilhuetaDoCentro(r, inclinacao, Mathf.Clamp(profOclusor * .4f, .08f, .42f), 1f, ownerId - 1);
    }

    private void ConfigurarVegetacaoVolumetricaPraca(SpriteRenderer r, string especie, Vector2 pos, float escala)
    {
        if (r == null || materialDoPiso == null || r.sprite == null) return;
        r.sharedMaterial = materialDoPiso;
        Bounds b = r.bounds;
        float baseY = b.min.y + b.size.y * .10f;
        int ownerId = 0;
        if (oclusoresMin.Count < LimiteOclusoresCentro - 1)
        {
            bool arvore = especie is "Camboata" or "Pitangueira" or "Bracatinga" or "Araucaria" or "CamboataJovem" or "BracatingaJovem";
            float larg = b.size.x * (arvore ? .58f : .56f);
            float prof = arvore ? .85f : .38f;
            float alt = arvore ? 3.25f * escala : 1.35f * escala;
            float modo = 4f;
            oclusoresMin.Add(new Vector4(b.center.x - larg * .5f - origemDoPiso.x, baseY - prof * .5f - origemDoPiso.y, 0f, modo));
            oclusoresMax.Add(new Vector4(b.center.x + larg * .5f - origemDoPiso.x, baseY + prof * .5f - origemDoPiso.y, alt, 0f));
            ownerId = oclusoresMin.Count;
        }
        var superficie = Resources.Load<Texture2D>("Art/RuaNorte/Vegetacao/" + especie + "_Superficie");
        var bloco = new MaterialPropertyBlock();
        r.GetPropertyBlock(bloco);
        if (r.sprite.texture != null)
            bloco.SetTexture("_MainTex", r.sprite.texture);
        bloco.SetVector("_Receiver", new Vector4(baseY, .85f, ownerId, 0f));
        float ampVento = especie is "Capim" or "Samambaia" or "SamambaiaDaMata" ? .045f : .028f;
        bloco.SetVector("_Wind", new Vector4(ampVento, Mathf.Max(.45f, b.size.y * .85f), pos.x * .7f + pos.y * 1.1f, 1f));
        bloco.SetVector("_CanopyTone", new Vector4(.96f, 1.02f, .94f, .86f));
        if (superficie != null)
        {
            bloco.SetTexture("_PropSurface", superficie);
            bloco.SetFloat("_PropSurfaceEnabled", 1f);
        }
        else
        {
            bloco.SetFloat("_PropSurfaceEnabled", 0f);
        }
        r.SetPropertyBlock(bloco);
        RegistrarSilhuetaDoCentro(r, .85f, .17f, .66f, ownerId - 1);
    }

    private void RegistrarSilhuetaDoCentro(SpriteRenderer sprite, float inclinacao, float profundidade,
        float densidade, int oclusor = -1)
    {
        if (sprite.sprite == null || sprite.sprite.packed) return;
        silhuetasDoCentro.Add((sprite, sprite.bounds, oclusor, inclinacao, profundidade, densidade));
    }

    private void IntegrarObjetosDasRuas(SpriteRenderer[] existentes)
    {
        if (materialDoPiso == null) return;
        foreach (var sprite in existentes)
        {
            if (sprite == null || sprite.gameObject.scene != gameObject.scene || !sprite.gameObject.activeInHierarchy) continue;
            string nome = sprite.name;
            if (nome.StartsWith("AO_", StringComparison.Ordinal))
            {
                sprite.gameObject.SetActive(false);
                continue;
            }
            if (!sprite.enabled || sprite.sprite == null || sprite.sharedMaterial == materialDoPiso) continue;

            bool vegetacao = nome.StartsWith("Arvore", StringComparison.Ordinal) ||
                nome is "SubBosque" or "Flores" or "FloresDoCanteiro" or "JardimCuidado";
            bool fachada = Fachadas.Contains(nome);
            bool mobiliario = nome.StartsWith("Banco", StringComparison.Ordinal) ||
                nome.StartsWith("Placa_", StringComparison.Ordinal) || nome.StartsWith("orelhao_", StringComparison.Ordinal) ||
                nome is "Cerca" or "MuroDoCemiterio" or "Portao_Cemiterio" or "Grade_Portaria_Futura" or
                    "Caixotes_Patio" or "Lixeira_Patio" or "LixeiraDaPraca";
            if (!vegetacao && !fachada && !mobiliario) continue;

            if (fachada)
            {
                // A fachada conserva o material e a luz de soleira. O piso recebe seu contorno.
                foreach (var caster in sprite.GetComponentsInChildren<ShadowCaster2D>())
                    caster.enabled = false;
                RegistrarSilhuetaDoCentro(sprite, .85f, .24f, 1f);
                ConfigurarNevoaDoCenario(sprite);
            }
            else if (vegetacao)
            {
                RegistrarSilhuetaDoCentro(sprite, .85f, .17f, .66f);
                ConfigurarNevoaDoCenario(sprite);
            }
            else
            {
                ConfigurarSpriteVolumetricoPraca(sprite, sprite.bounds.min.y, .85f,
                    0f, 0f, .36f, modoOclusor: -1);
            }
        }
    }

    private void ConfigurarNevoaDoCenario(SpriteRenderer sprite)
    {
        var original = sprite.sharedMaterial;
        if (original == null) return;
        bool litPadrao = original == lit;
        var basePbr = litPadrao ? Resources.Load<Material>("Art/WorldLit") : original;
        if (basePbr == null || !basePbr.HasProperty("_CentroFogStrength")) return;
        if (!materiaisDaNevoaCentro.TryGetValue(basePbr, out var material))
        {
            material = new Material(basePbr) { name = basePbr.name + " · névoa do Centro" };
            material.SetFloat("_CentroFogStrength", DensidadeNevoaCentro);
            materiaisDaNevoaCentro.Add(basePbr, material);
            recursos.Add(material);
        }
        sprite.sharedMaterial = material;
        propriedadesNevoaCenario ??= new MaterialPropertyBlock();
        sprite.GetPropertyBlock(propriedadesNevoaCenario);
        propriedadesNevoaCenario.SetVector("_CentroFogReceiver",
            new Vector4(sprite.bounds.min.y, .85f, 0, litPadrao ? 1f : 0f));
        sprite.SetPropertyBlock(propriedadesNevoaCenario);
    }

    private static void OcuparBasePraca(SpriteRenderer r, Vector2 tamanho)
    {
        if (r == null) return;
        var collider = r.gameObject.AddComponent<BoxCollider2D>();
        collider.size = tamanho / r.transform.localScale.x;
        collider.offset = Vector2.up * (.11f / r.transform.localScale.x);
    }

    private void LuzPraca(string nome, Vector2 p, float raio, float potencia, Color cor)
    {
        var obj = new GameObject(nome, typeof(Light2D));
        obj.transform.SetParent(transform, false);
        obj.transform.position = p;
        var light = obj.GetComponent<Light2D>();
        light.lightType = Light2D.LightType.Point;
        light.color = cor;
        light.intensity = potencia;
        light.pointLightInnerRadius = raio * .16f;
        light.pointLightOuterRadius = raio;
        light.falloffIntensity = .64f;
    }

    // As linhas da Rua Norte ficam intactas. Os dois pisos da Matriz são
    // acrescentados ao topo do atlas, sem alterar as coordenadas do shader.
    private enum PisoCentro { Especial = -1, Grama, Asfalto, Calcada, Terra, Fissura = 7, Pedregulho, Adro }
    private static readonly Vector3Int[] LadosDoPiso =
        { Vector3Int.left, Vector3Int.right, Vector3Int.down, Vector3Int.up };
    private Material materialDoPiso;
    private Vector2 origemDoPiso;
    private Tilemap pisoDoCentro;

    private static bool PistaVertical(int x, int y) =>
        (x >= 29 && x <= 33 && y >= 22 && y <= 81) ||
        (x >= 58 && x <= 62 && y >= 20 && y <= 79);

    private static bool PistaHorizontal(int x, int y) =>
        y >= 35 && y <= 39 && x >= 29 && x <= 79;

    private static bool PistaPrincipal(int x, int y) =>
        PistaVertical(x, y) || PistaHorizontal(x, y);

    private static bool CaminhoDeTerra(int x, int y)
    {
        var p = new Vector2(x + .5f, y + .5f);
        return Trecho(p, new Vector2(31, 27), new Vector2(20, 19), 1.95f) ||
            Trecho(p, new Vector2(20, 19), new Vector2(5, 13), 2.05f) ||
            Trecho(p, new Vector2(31, 22), new Vector2(46, 2), 1.55f);
    }

    private static bool Trecho(Vector2 p, Vector2 a, Vector2 b, float raio)
    {
        Vector2 delta = b - a;
        float t = Mathf.Clamp01(Vector2.Dot(p - a, delta) / delta.sqrMagnitude);
        return (p - (a + t * delta)).sqrMagnitude <= raio * raio;
    }

    private static PisoCentro ClassificarPiso(Vector3Int c, string nome)
    {
        int x = c.x, y = c.y;
        if (nome is "Agua" or "Madeira") return PisoCentro.Especial;
        if (PistaPrincipal(x, y))
            return ((x * 17 ^ y * 23) & 15) == 0 ? PisoCentro.Fissura : PisoCentro.Asfalto;

        // O tapete central de tijolinho (eixoDaMatriz) avança até y = 40 (meio-fio da rua principal sul),
        // conectando a faixa de pedestres diretamente ao largo da fonte e ao adro da Igreja em x = 46.0f.
        bool eixoDaMatriz = x >= 43 && x < 49 && y >= 40 && y < 64;
        bool largoDaFonte = x >= 42 && x < 50 && y >= 47 && y < 54;
        bool adro = x >= 40 && x < 52 && y >= 59 && y < 64;
        bool faixaDasMuretas = y == 63 && x >= 34 && x < 58;
        if (eixoDaMatriz || largoDaFonte || adro || faixaDasMuretas) return PisoCentro.Adro;

        // A boca da subida cruza o passeio. O shader dissolve as bordas no asfalto.
        if (y == 82 && x >= 29 && x <= 32) return PisoCentro.Terra;

        foreach (var lado in LadosDoPiso)
            if (PistaPrincipal(x + lado.x, y + lado.y)) return PisoCentro.Calcada;

        // Quatro canteiros ajardinados simétricos no interior da praça e bosque ao norte da mureta (y >= 64).
        if (NoCanteiroDaPraca(x, y)) return PisoCentro.Grama;
        if (x >= 35 && x < 57 && y >= 64 && y < 69) return PisoCentro.Grama;

        // Todas as alamedas internas da praça (x = 35..56, y = 41..63) ao redor dos canteiros e do adro são Pedregulho.
        if (x >= 35 && x < 57 && y >= 41 && y < 64) return PisoCentro.Pedregulho;
        if (nome == "Pedra") return PisoCentro.Pedregulho;

        if (CaminhoDeTerra(x, y)) return PisoCentro.Terra;
        if (nome.StartsWith("Asfalto", StringComparison.Ordinal)) return PisoCentro.Asfalto;
        if (nome == "Calcada") return PisoCentro.Calcada;
        if (nome.StartsWith("Terra", StringComparison.Ordinal)) return PisoCentro.Terra;
        if (nome.StartsWith("Grama", StringComparison.Ordinal)) return PisoCentro.Grama;
        return PisoCentro.Especial;
    }

    private void PavimentarCentro(SpriteRenderer[] existentes)
    {
        var original = Array.Find(FindObjectsByType<Tilemap>(),
            m => m.gameObject.scene == gameObject.scene && m.name == "Terreno");
        if (original == null) return;
        const string c1 = "Art/RuaNorte/";
        var atlas = Resources.Load<Texture2D>(Matriz + "PisosCentroTerreno");
        var superficie = Resources.Load<Texture2D>(Matriz + "PisosCentroSuperficie");
        var relevo = Resources.Load<Texture2D>(Matriz + "PisosCentroRelevo");
        var shader = Resources.Load<Shader>(c1 + "LuzVolumetrica");
        if (atlas == null || atlas.width != 256 || atlas.height != 960 ||
            superficie == null || superficie.width != atlas.width || superficie.height != atlas.height ||
            relevo == null || relevo.width != atlas.width || relevo.height != atlas.height ||
            shader == null || !shader.isSupported)
        {
            Debug.LogError("Pavimentos do Centro: faltam o atlas, o shader ou as superfícies da Rua Norte.");
            return;
        }

        materialDoPiso = new Material(shader) { name = "Piso urbano · Luz da Rua Norte", mainTexture = atlas };
        recursos.Add(materialDoPiso);
        materialDoPiso.SetFloat("_Town", 1);
        materialDoPiso.SetFloat("_TownGroundMist", DensidadeNevoaCentro);
        materialDoPiso.SetFloat("_WetRoad", 1);
        materialDoPiso.SetFloat("_GroundSurfaceEnabled", 1);
        materialDoPiso.SetFloat("_GroundReliefStrength", 1);
        materialDoPiso.SetFloat("_ContactStrength", 0f);
        materialDoPiso.SetFloat("_WindStrength", .65f);
        materialDoPiso.SetTexture("_GroundSurface", superficie);
        materialDoPiso.SetTexture("_GroundRelief", relevo);

        var limites = original.cellBounds;
        int largura = limites.size.x, altura = limites.size.y;
        origemDoPiso = original.CellToWorld(limites.min);
        materialDoPiso.SetVector("_Origin", origemDoPiso);
        var atlasTiles = TilesDaRuaNorte(atlas);
        var pavimentos = new TileBase[largura * altura];
        var outros = new TileBase[pavimentos.Length];
        var vizinhanca = new Color32[pavimentos.Length];
        Array.Fill(vizinhanca, new Color32(255, 0, 0, 0));
        var preservar = new List<Vector3Int>();
        var baseRenderer = original.GetComponent<TilemapRenderer>();
        pisoDoCentro = CriarMapaVisual(original, "Pavimentos do Centro", materialDoPiso);
        var especiais = CriarMapaVisual(original, "Água e madeira do Centro", lit);

        foreach (var c in limites.allPositionsWithin)
        {
            var tile = original.GetTile(c);
            if (tile == null) continue;
            int x = c.x, y = c.y;
            int index = (y - limites.yMin) * largura + x - limites.xMin;
            int variante = (x * 73 ^ y * 193) & 3;
            var piso = ClassificarPiso(c, tile.name);
            if (piso == PisoCentro.Especial)
            {
                outros[index] = tile;
                preservar.Add(c);
                continue;
            }

            if (piso is PisoCentro.Adro or PisoCentro.Pedregulho)
                variante = (x & 1) + (1 - (y & 1)) * 2;
            else if (piso == PisoCentro.Calcada)
                variante = (x + y) & 1; // Usa apenas as lajes íntegras (0 e 1), sem os buracos de terra da rodovia.

            int linha = LinhaDoAtlas(piso);
            pavimentos[index] = atlasTiles[(int)piso, variante];
            vizinhanca[index] = new Color32((byte)variante, (byte)linha, 0, 0);
            bool naVertical = PistaVertical(x, y);
            bool naHorizontal = PistaHorizontal(x, y);
            if (naVertical != naHorizontal &&
                (naVertical ? (x == 60 || x == 31 && y < 78) && (y & 3) == 1 :
                    y == 37 && (x & 3) == 1 && (x < 42 || x >= 50)))
                FaixaPraca(x, y, naVertical, baseRenderer.sortingLayerID, baseRenderer.sortingOrder + 1);
        }

        // Faixa de travessia de pedestres na rua principal sul (x = 43..48, centro x = 46.0f),
        // conectando o asfalto diretamente ao eixo de tijolinho da Igreja Matriz.
        FaixaTravessiaMatriz(baseRenderer.sortingLayerID, baseRenderer.sortingOrder + 1);

        pisoDoCentro.SetTilesBlock(limites, pavimentos);
        especiais.SetTilesBlock(limites, outros);
        foreach (var c in preservar)
        {
            especiais.SetTileFlags(c, TileFlags.None);
            especiais.SetTransformMatrix(c, original.GetTransformMatrix(c));
            especiais.SetColor(c, original.GetColor(c));
        }
        var lookup = new Texture2D(largura, altura, TextureFormat.RGBA32, false, true)
        {
            name = "Vizinhança dos pavimentos do Centro",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        lookup.SetPixels32(vizinhanca);
        lookup.Apply(false, true);
        recursos.Add(lookup);
        materialDoPiso.SetTexture("_GroundCells", lookup);
        EspalharPedregulhoDaMatriz();

        baseRenderer.enabled = false;
        foreach (var legado in FindObjectsByType<TilemapRenderer>())
            if (legado.gameObject.scene == gameObject.scene && legado.name == "DetalhesTerreno")
                legado.enabled = false;
        foreach (var sprite in existentes)
        {
            if (sprite.gameObject.scene != gameObject.scene) continue;
            if (sprite.name is "CaminhosBeiraRio" or "CaminhoRuaNorte" or "FaixaPedestre")
                sprite.enabled = false;
            if (sprite.name == "Poste") sprite.gameObject.SetActive(false);
            var pe = sprite.transform.position;
            if (GramaNoCentro(pe) && sprite.name is ("BancoMargem" or "AO_BancoMargem" or "JardimCuidado" or "Flores" or "FloresDoCanteiro"))
                sprite.gameObject.SetActive(false);
            var c = pisoDoCentro.WorldToCell(pe);
            if (!PistaPrincipal(c.x, c.y)) continue;
            if (sprite.name.StartsWith("Placa_RUA NORTE", StringComparison.Ordinal) ||
                sprite.name.StartsWith("Placa_BEIRA-RIO", StringComparison.Ordinal))
                sprite.transform.position = new Vector3(28.45f, pe.y, pe.z);
            if (sprite.name == "Flores")
                sprite.transform.position = new Vector3(63.65f, pe.y, pe.z);
        }
    }

    private void FaixaTravessiaMatriz(int layer, int order)
    {
        for (int i = -3; i <= 3; i++)
        {
            float x = CentroEixoX + i * .74f;
            var go = new GameObject("Faixa de travessia da Matriz", typeof(SpriteRenderer));
            go.transform.SetParent(transform, false);
            go.transform.position = new Vector2(x, 38.85f);
            go.transform.localScale = new Vector3(.38f, 1.72f, 1f);
            var r = go.GetComponent<SpriteRenderer>();
            r.sprite = Quad();
            r.sharedMaterial = lit;
            r.color = new Color(.56f, .53f, .44f, .42f);
            r.sortingLayerID = layer;
            r.sortingOrder = order;
        }
    }

    private void EspalharPedregulhoDaMatriz()
    {
        const int pixelsPorUnidade = 32;
        var area = new BoundsInt(34, 39, 0, 24, 30, 1);
        int largura = area.size.x * pixelsPorUnidade;
        int altura = area.size.y * pixelsPorUnidade;
        var pixels = new byte[largura * altura];

        foreach (var c in area.allPositionsWithin)
        {
            string piso = pisoDoCentro.GetTile(c)?.name;
            float alcance = piso switch
            {
                "Grama" => .32f,
                "Calcada" => .20f,
                "Adro" => .18f,
                _ => 0
            };
            if (alcance == 0) continue;

            foreach (var lado in LadosDoPiso)
            {
                if (pisoDoCentro.GetTile(c + lado)?.name != "Pedregulho") continue;
                for (int py = 0; py < pixelsPorUnidade; py++)
                for (int px = 0; px < pixelsPorUnidade; px++)
                {
                    float u = (px + .5f) / pixelsPorUnidade;
                    float v = (py + .5f) / pixelsPorUnidade;
                    float d = lado.x != 0 ? (lado.x > 0 ? 1 - u : u) : (lado.y > 0 ? 1 - v : v);
                    // Preserva a faixa da fiada de pedra (d < .095f) intacta e espalha grãos suaves logo além dela.
                    if (d < .095f || d > alcance) continue;
                    float x = c.x + u;
                    float y = c.y + v;
                    float saliencia = Mathf.PerlinNoise(x * 1.85f + 13.7f, y * 1.85f + 42.2f);
                    float detalhe = Mathf.PerlinNoise(x * 9.1f + 31.1f, y * 9.1f + 17.9f);
                    float lingua = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.38f, .76f, saliencia));
                    float borda = .095f + (alcance - .095f) * (.45f * lingua + .22f * detalhe);
                    bool cascalhoSolto = d < alcance * (.52f + .38f * lingua) &&
                        detalhe > .76f + .10f * (d / alcance);
                    if (d >= borda && !cascalhoSolto) continue;
                    int ix = (c.x - area.xMin) * pixelsPorUnidade + px;
                    int iy = (c.y - area.yMin) * pixelsPorUnidade + py;
                    pixels[iy * largura + ix] = 220;
                }
            }
        }

        var recorte = new Texture2D(largura, altura, TextureFormat.R8, false, true)
        {
            name = "Contorno do pedregulho da Matriz",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        recorte.SetPixelData(pixels, 0);
        recorte.Apply(false, true);
        recursos.Add(recorte);
        Vector2 inicio = (Vector2)pisoDoCentro.CellToWorld(area.min) - origemDoPiso;
        materialDoPiso.SetTexture("_MatrizBorder", recorte);
        materialDoPiso.SetVector("_MatrizBorderRect", new Vector4(inicio.x, inicio.y, area.size.x, area.size.y));
    }

    private Tilemap CriarMapaVisual(Tilemap original, string nome, Material material)
    {
        var obj = new GameObject(nome, typeof(Tilemap), typeof(TilemapRenderer));
        obj.transform.SetParent(original.transform.parent, false);
        obj.transform.localPosition = original.transform.localPosition;
        var map = obj.GetComponent<Tilemap>();
        map.tileAnchor = original.tileAnchor;
        var renderer = obj.GetComponent<TilemapRenderer>();
        var baseRenderer = original.GetComponent<TilemapRenderer>();
        renderer.sharedMaterial = material;
        renderer.sortingLayerID = baseRenderer.sortingLayerID;
        renderer.sortingOrder = baseRenderer.sortingOrder;
        return map;
    }

    private Tile[,] TilesDaRuaNorte(Texture2D atlas)
    {
        var tiles = new Tile[atlas.height / 64, 4];
        foreach (var piso in new[] { PisoCentro.Grama, PisoCentro.Asfalto, PisoCentro.Calcada,
                     PisoCentro.Terra, PisoCentro.Fissura, PisoCentro.Pedregulho, PisoCentro.Adro })
        for (int variante = 0; variante < 4; variante++)
        {
            int linha = LinhaDoAtlas(piso);
            var sprite = Sprite.Create(atlas, new Rect(variante * 64, linha * 64, 64, 64),
                new Vector2(.5f, .5f), 64, 0, SpriteMeshType.FullRect);
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.name = piso.ToString();
            tile.sprite = sprite;
            tile.colliderType = Tile.ColliderType.None;
            tiles[(int)piso, variante] = tile;
            recursos.Add(sprite);
            recursos.Add(tile);
        }
        return tiles;
    }

    private static int LinhaDoAtlas(PisoCentro piso) => piso switch
    {
        PisoCentro.Pedregulho => 13,
        PisoCentro.Adro => 14,
        _ => 12 - (int)piso
    };

    private void PrepararPocasDoCentro(int largura, int altura)
    {
        var pocas = new List<(float x, float y, float rx, float ry, float tilt, float blue, float shine)>
        {
            (32.5f, 24.8f, .36f, .52f, -.12f, .68f, .78f),
            (29.7f, 29.4f, .40f, .76f, .04f, .61f, .76f),
            (33.2f, 34.6f, .52f, .86f, .03f, .64f, .85f),
            (29.8f, 42.7f, .38f, .66f, -.12f, .73f, .76f),
            (32.6f, 46.4f, .39f, .62f, .18f, .71f, .78f),
            (32.7f, 50.2f, .58f, 1.02f, .13f, .52f, .93f),
            (29.8f, 58.1f, .46f, .84f, .11f, .61f, .73f),
            (32.3f, 63.7f, .53f, 1.02f, -.18f, .68f, .82f),
            (33.2f, 69.1f, .44f, .68f, .04f, .49f, .87f),
            (30.0f, 75.6f, .48f, .70f, .05f, .63f, .73f),
            (32.6f, 79.4f, .43f, .65f, -.14f, .67f, .76f),
            (41.2f, 38.9f, .98f, .35f, -.12f, .42f, .88f),
            (50.8f, 38.9f, .98f, .35f, .12f, .42f, .88f),
            (46.0f, 36.2f, .86f, .38f, .02f, .54f, .92f),
            // Poças espelhadas no adro e passeios da Praça da Matriz (refletem a Igreja, postes e Marcus).
            (46.0f, 61.25f, 1.04f, .65f, .03f, .52f, .94f),
            (45.82f, 59.45f, .83f, .43f, -.07f, .54f, .90f),
            (42.2f, 61.9f, .78f, .48f, -.08f, .48f, .90f),
            (49.8f, 61.9f, .78f, .48f, .08f, .48f, .90f),
            (46.0f, 55.2f, .92f, .56f, -.04f, .50f, .91f),
            (46.0f, 45.2f, .88f, .54f, .05f, .50f, .91f),
            (36.2f, 50.4f, .56f, .74f, .09f, .46f, .86f),
            (55.8f, 50.4f, .56f, .74f, -.09f, .46f, .86f),
            (66.3f, 38.9f, 1.05f, .41f, .06f, .43f, .89f),
            (73.4f, 35.6f, .83f, .44f, -.16f, .55f, .83f),
            (62.4f, 26.0f, .44f, .86f, -.08f, .51f, .92f),
            (59.2f, 31.2f, .52f, .74f, .13f, .67f, .80f),
            (58.7f, 46.9f, .48f, .78f, .15f, .60f, .88f),
            (61.9f, 42.4f, .33f, .53f, -.12f, .70f, .78f),
            (62.2f, 55.3f, .63f, .85f, -.14f, .57f, .75f),
            (59.1f, 60.9f, .54f, .93f, .21f, .69f, .83f),
            (58.6f, 65.7f, .42f, .72f, -.14f, .52f, .88f),
            (62.3f, 71.4f, .50f, .89f, -.05f, .52f, .81f),
            (59.2f, 76.6f, .42f, .68f, .16f, .68f, .76f),
            (24.5f, 22.4f, .77f, .32f, .24f, .34f, .76f),
            (13.5f, 16.3f, .58f, .29f, .18f, .35f, .65f),
            (27.5f, 83.65f, .67f, .30f, -.18f, .39f, .66f),
            (30.4f, 81.85f, .45f, .32f, -.36f, .47f, .70f)
        };
        for (int i = 0; i < pocas.Count; i++)
        {
            var poca = pocas[i];
            poca.x -= origemDoPiso.x;
            poca.y -= origemDoPiso.y;
            pocas[i] = poca;
        }
        var mask = SantaRitaRuaNorte.CriarMascaraDePocas("Poças das ruas do Centro", largura, 0, altura,
            32, pocas.ToArray(), legivel: false);
        recursos.Add(mask);
        materialDoPiso.SetTexture("_EntranceWater", mask);
        materialDoPiso.SetVector("_EntranceWaterRect", new Vector4(0, 0, largura, altura));
    }

    private void IluminarPisoDoCentro()
    {
        if (materialDoPiso == null) return;
        var celulas = materialDoPiso.GetTexture("_GroundCells");
        PrepararPocasDoCentro(celulas.width, celulas.height);

        // Cada luminária afeta o piso e os sprites no mesmo sistema de coordenadas.
        var postes = new Vector4[LimiteLuzesCentro];
        var cores = new Vector4[LimiteLuzesCentro];
        int quantidade = 0;

        foreach (var (pos, raio, cor) in luzesVolumetricasPraca)
        {
            if (quantidade >= postes.Length) break;
            postes[quantidade] = new Vector4(pos.x - origemDoPiso.x, pos.y - origemDoPiso.y, pos.z, raio);
            cores[quantidade] = new Vector4(cor.r, cor.g, cor.b, 1f);
            quantidade++;
        }

        materialDoPiso.SetInteger("_TownLampCount", quantidade);
        materialDoPiso.SetFloat("_TownChurchLamp", indiceLuzEntradaMatriz);
        materialDoPiso.SetVectorArray("_TownLamps", postes);
        materialDoPiso.SetVectorArray("_TownLampColors", cores);

        // Reserva o oclusor dinâmico do Marcus e vincula seu SpriteRenderer ao shader volumétrico na praça.
        indiceOclusorMarcus = oclusoresMin.Count;
        oclusoresMin.Add(Vector4.zero);
        oclusoresMax.Add(Vector4.zero);
        for (int i = 0; i < oclusoresMin.Count; i++)
        {
            bufferMin[i] = oclusoresMin[i];
            bufferMax[i] = oclusoresMax[i];
        }
        materialDoPiso.SetInteger("_CasterCount", oclusoresMin.Count);
        materialDoPiso.SetInteger("_ArrivalPlayerCaster", indiceOclusorMarcus);
        materialDoPiso.SetVectorArray("_CasterMin", bufferMin);
        materialDoPiso.SetVectorArray("_CasterMax", bufferMax);

        PrepararRecortesDaPraca(postes, quantidade);
    }

    private void PrepararRecortesDaPraca(Vector4[] luzes, int quantidade)
    {
        if (quantidade == 0 || !SystemInfo.supports2DArrayTextures ||
            !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8)) return;
        var shader = Resources.Load<Shader>("Art/RuaNorte/Entrada/ProjetarRecorte");
        if (shader == null || !shader.isSupported) return;

        recortesDosPostes = CriarTexturaDeRecorte("Silhuetas dos postes da Matriz", quantidade);
        if (recortesDosPostes == null) return;
        projetorDaPraca = new Material(shader);
        malhaDeRecorte = SantaRitaRuaNorte.CriarMalhaDeProjecao();
        propriedadesDoRecorte = new MaterialPropertyBlock();
        recursos.Add(projetorDaPraca);
        recursos.Add(malhaDeRecorte);
        propriedadesDoRecorte.SetVector("_ShadowArea", new Vector4(TamanhoRecorte, TamanhoRecorte, 0, 0));
        propriedadesDoRecorte.SetVector("_ShadowChannel", new Vector4(1, 0, 0, 0));

        using (var pass = new CommandBuffer { name = "Projetar silhuetas da Matriz por luminária" })
        {
            for (int i = 0; i < quantidade; i++)
            {
                pass.SetRenderTarget(recortesDosPostes, 0, CubemapFace.Unknown, i);
                DesenharRecortesDaPraca(pass, recortesDosPostes, luzes[i]);
            }
            Graphics.ExecuteCommandBuffer(pass);
        }

        // O piso já recebe a silhueta desses objetos. Os volumes continuam iluminando os sprites entre si.
        foreach (var (_, _, oclusor, _, _, _) in silhuetasDoCentro)
        {
            if (oclusor < 0) continue;
            var max = oclusoresMax[oclusor];
            max.w = 1f;
            oclusoresMax[oclusor] = max;
            bufferMax[oclusor] = max;
        }
        // Com o recorte disponível, o piso só precisa testar os volumes restantes e Marcus.
        var oclusoresRestantes = new float[LimiteOclusoresCentro];
        int quantidadeRestante = 0;
        for (int i = 0; i < oclusoresMax.Count; i++)
        {
            if (oclusoresMax[i].w <= .5f) oclusoresRestantes[quantidadeRestante++] = i;
        }
        materialDoPiso.SetFloatArray("_TownProjectedCasterIds", oclusoresRestantes);
        materialDoPiso.SetInteger("_TownProjectedCasterCount", quantidadeRestante);
        materialDoPiso.SetVectorArray("_CasterMax", bufferMax);
        materialDoPiso.SetFloat("_TownShadowSize", TamanhoRecorte);
        materialDoPiso.SetVector("_ArrivalShadowSize", Vector2.one * TamanhoRecorte);
        materialDoPiso.SetTexture("_TownShadows", recortesDosPostes);
        materialDoPiso.SetFloat("_TownShadowEnabled", 1f);
    }

    private RenderTexture CriarTexturaDeRecorte(string nome, int fatias = 0)
    {
        const int ppu = 32;
        var textura = new RenderTexture(TamanhoRecorte * ppu, TamanhoRecorte * ppu,
            0, RenderTextureFormat.R8, RenderTextureReadWrite.Linear)
        {
            name = nome,
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            dimension = fatias > 0 ? TextureDimension.Tex2DArray : TextureDimension.Tex2D,
            volumeDepth = Mathf.Max(1, fatias)
        };
        if (!textura.Create())
        {
            Destroy(textura);
            return null;
        }
        recursos.Add(textura);
        return textura;
    }

    private void DesenharRecortesDaPraca(CommandBuffer pass, RenderTexture alvo, Vector4 luz)
    {
        pass.SetViewport(new Rect(0, 0, alvo.width, alvo.height));
        pass.ClearRenderTarget(false, true, Color.clear);
        float metade = TamanhoRecorte * .5f;
        Vector2 origem = origemDoPiso + new Vector2(luz.x - metade, luz.y - metade);
        Vector2 centro = origem + Vector2.one * metade;
        float alcance2 = luz.w * luz.w;
        luz.x = metade;
        luz.y = metade;
        propriedadesDoRecorte.SetVector("_ShadowLamp", luz);
        foreach (var (sprite, bounds, _, inclinacao, profundidade, densidade) in silhuetasDoCentro)
        {
            // Vegetação e mobiliário têm posição fixa. Use a base armazenada para descartar os que não recebem a luz.
            float dx = Mathf.Clamp(centro.x, bounds.min.x, bounds.max.x) - centro.x;
            float dy = bounds.min.y - centro.y;
            if (dx * dx + dy * dy > alcance2 || sprite == null || !sprite.enabled ||
                !sprite.gameObject.activeInHierarchy || sprite.sprite == null || sprite.sprite.packed) continue;
            SantaRitaRuaNorte.DesenharSilhuetaDaLuz(pass, malhaDeRecorte, projetorDaPraca, propriedadesDoRecorte,
                sprite, origem, luz, bounds.min.y, inclinacao, profundidade, densidade);
        }
    }

    private void ProjetarRecorteDaLanterna(Vector4 luz)
    {
        if (projetorDaPraca == null) return;
        if (recorteDaLanterna == null)
            recorteDaLanterna = CriarTexturaDeRecorte("Silhuetas da lanterna na Matriz");
        if (recorteDaLanterna == null) return;

        // Mesma altura efetiva da lanterna no projetor da Rua Norte.
        luz.z = Mathf.Max(2.45f, luz.z);
        luz.x = Mathf.Round(luz.x * 32f) / 32f;
        luz.y = Mathf.Round(luz.y * 32f) / 32f;
        if (luz == ultimoRecorteDaLanterna && recorteDaLanterna.IsCreated())
        {
            materialDoPiso.SetFloat("_PlayerShadowEnabled", 1f);
            return;
        }
        using (var pass = new CommandBuffer { name = "Projetar silhuetas da lanterna na Matriz" })
        {
            pass.SetRenderTarget(recorteDaLanterna);
            DesenharRecortesDaPraca(pass, recorteDaLanterna, luz);
            Graphics.ExecuteCommandBuffer(pass);
        }
        materialDoPiso.SetVector("_ArrivalShadowOrigin",
            new Vector2(luz.x, luz.y) - Vector2.one * (TamanhoRecorte * .5f));
        materialDoPiso.SetTexture("_PlayerShadows", recorteDaLanterna);
        materialDoPiso.SetFloat("_PlayerShadowEnabled", 1f);
        ultimoRecorteDaLanterna = luz;
    }

    private void AtualizarNevoaSobreMarcus(bool efeitosAtivos, float pesY)
    {
        if (nevoaSobreMarcus == null && efeitosAtivos)
        {
            const string nome = "Névoa no corpo de Marcus";
            var existente = marcus.transform.Find(nome);
            var obj = existente != null ? existente.gameObject : new GameObject(nome, typeof(SpriteRenderer));
            if (existente == null) obj.transform.SetParent(marcus.transform, false);
            nevoaSobreMarcus = obj.GetComponent<SpriteRenderer>();
            nevoaSobreMarcus.sharedMaterial = materialDoPiso;
            recursos.Add(obj);
        }
        if (nevoaSobreMarcus == null) return;
        nevoaSobreMarcus.enabled = efeitosAtivos && marcus.enabled && marcus.sprite != null;
        if (!nevoaSobreMarcus.enabled) return;

        nevoaSobreMarcus.sprite = marcus.sprite;
        nevoaSobreMarcus.flipX = marcus.flipX;
        nevoaSobreMarcus.flipY = marcus.flipY;
        nevoaSobreMarcus.color = new Color(1, 1, 1, marcus.color.a);
        nevoaSobreMarcus.sortingLayerID = marcus.sortingLayerID;
        // Depois do corpo e do recorte de luz, acompanhando a ordenação do próprio ator.
        nevoaSobreMarcus.sortingOrder = marcus.sortingOrder + 2;
        propriedadesNevoaMarcus ??= new MaterialPropertyBlock();
        propriedadesNevoaMarcus.SetTexture("_MainTex", marcus.sprite.texture);
        propriedadesNevoaMarcus.SetVector("_Receiver", new Vector4(pesY, .86f, indiceOclusorMarcus + 1, 1f));
        nevoaSobreMarcus.SetPropertyBlock(propriedadesNevoaMarcus);
    }

    private void AtualizarLuzesDinamicasDaPraca(bool efeitosAtivos)
    {
        if (materialDoPiso == null) return;
        float densidade = efeitosAtivos ? materialDoPiso.GetFloat("_TownGroundMist") : 0f;
        if (!Mathf.Approximately(densidade, ultimaDensidadeNevoaCenario))
        {
            foreach (var material in materiaisDaNevoaCentro.Values)
                material.SetFloat("_CentroFogStrength", densidade);
            ultimaDensidadeNevoaCenario = densidade;
        }

        if (marcus != null)
        {
            Bounds bm = marcus.bounds;
            var colliderPes = marcus.GetComponent<Collider2D>();
            // O limite do sprite oscila entre quadros da animação; o collider mantém a
            // referência da sombra projetada presa ao chão durante caminhada e repouso.
            float pesY = colliderPes != null ? colliderPes.bounds.min.y + .05f : marcus.transform.position.y;
            AtualizarNevoaSobreMarcus(efeitosAtivos, pesY);
            Vector2 centroMarcus = new(bm.center.x, pesY);
            bool marcusNaPraca = NaPraca(centroMarcus);

            // Foco da câmera/olhar na praça e atualização do volume 3D do Marcus para sombras projetadas no piso.
            Vector2 focoRelativo = centroMarcus - origemDoPiso;
            materialDoPiso.SetVector("_ViewFocus", new Vector4(focoRelativo.x, focoRelativo.y, marcusNaPraca ? .85f : .35f, 0f));

            if (indiceOclusorMarcus >= 0)
            {
                bufferMin[indiceOclusorMarcus] = new Vector4(bm.min.x - origemDoPiso.x, pesY - .12f - origemDoPiso.y, 0f, 0f);
                bufferMax[indiceOclusorMarcus] = new Vector4(bm.max.x - origemDoPiso.x, pesY + .12f - origemDoPiso.y,
                    Mathf.Max(.9f, bm.size.y * .94f), 0f);
                materialDoPiso.SetVectorArray("_CasterMin", bufferMin);
                materialDoPiso.SetVectorArray("_CasterMax", bufferMax);
            }
        }

        if (efeitosAtivos && controladorMarcus != null &&
            controladorMarcus.ObterFeixeLanterna(out Vector2 origemLanterna, out Vector2 direcaoLanterna))
        {
            const float alcanceLanterna = 6.2f;
            Vector2 rel = origemLanterna - origemDoPiso;
            float alturaLuz = marcus != null ? Mathf.Max(1.05f, origemLanterna.y - marcus.transform.position.y + .42f) : 1.05f;
            var luz = new Vector4(rel.x, rel.y, alturaLuz, alcanceLanterna);
            materialDoPiso.SetVector("_PlayerLamp", luz);
            materialDoPiso.SetVector("_PlayerLampDirection", direcaoLanterna);
            ProjetarRecorteDaLanterna(luz);
        }
        else
        {
            materialDoPiso.SetVector("_PlayerLamp", Vector4.zero);
            materialDoPiso.SetFloat("_PlayerShadowEnabled", 0f);
        }
    }

    private void ComporMargensDoCentro()
    {
        var terreno = pisoDoCentro;
        if (terreno == null) return;
        AnimarCopasDasRuas();
        const string modulos = "Art/RuaNorte/Entrada/Modulos/";
        (float x, float y, bool direita)[] postes =
        {
            (28.40f, 43.7f, true), (34.45f, 51.2f, false), (28.45f, 72.5f, true),
            (57.55f, 51.2f, true), (63.50f, 57.2f, false), (57.45f, 68.1f, true),
            (63.50f, 74.2f, false), (63.55f, 26.6f, false), (68.65f, 40.55f, true),
            (75.45f, 34.45f, false), (13.25f, 11.0f, false), (18.40f, 46.75f, true)
        };
        foreach (var (x, y, direita) in postes)
        {
            Vector2 pe = new(x, y);
            string piso = terreno.GetTile(terreno.WorldToCell(pe))?.name;
            if (piso != "Calcada" || Physics2D.OverlapCircle(pe, .20f) != null) continue;
            PostePraca(x, y, direita, "Poste do passeio");
            PecaPraca("Folhas ao pé do poste", modulos + "Folhas", x + .16f, y - .12f, .83f, noPiso: true);
            PecaPraca("Ralo junto ao passeio", Art + "Centro/Ralo", x + (direita ? .28f : -.28f),
                y - .48f, .56f, noPiso: true);
        }

        // Pequenos acúmulos junto ao meio-fio, entre os trechos de drenagem.
        foreach (var (x, y, largura) in new (float, float, float)[]
        {
            (28.85f, 53.8f, .46f), (34.13f, 61.2f, .55f), (28.82f, 76.7f, .61f),
            (34.14f, 79.5f, .44f), (63.18f, 29.6f, .54f), (57.83f, 48.6f, .49f),
            (63.19f, 61.8f, .58f), (57.81f, 76.9f, .42f)
        })
        {
            if (terreno.GetTile(terreno.WorldToCell(new Vector2(x, y)))?.name != "Calcada") continue;
            var folhas = PecaPraca("Folhiço levado à sarjeta", modulos + "Folhas", x, y, largura, noPiso: true);
            if (folhas == null) continue;
            folhas.flipX = x > 33 && x < 58;
            folhas.transform.localScale = Vector3.Scale(folhas.transform.localScale, new Vector3(.72f, 1.28f, 1));
        }

        MobiliaDosAcessos(terreno);

        // Grupos alternam árvores jovens e copas maiores, atrás do limite dos passeios.
        PlantarArvoresNoGramado("Árvore do matagal da via", new (string, float, float, float)[]
        {
            ("CamboataJovem", 26.8f, 45.1f, 1.8f), ("Pitangueira", 24.9f, 46.4f, 3.0f),
            ("BracatingaJovem", 26.5f, 49.4f, 2.2f), ("Camboata", 25.5f, 51.4f, 3.1f),
            ("BracatingaJovem", 26.9f, 68.1f, 2.0f), ("Bracatinga", 24.6f, 69.9f, 3.6f),
            ("Pitangueira", 26.3f, 72.6f, 2.8f), ("Camboata", 25.0f, 74.2f, 3.0f),
            ("CamboataJovem", 36.1f, 73.1f, 2.0f), ("Bracatinga", 37.5f, 75.3f, 3.1f),
            ("Pitangueira", 36.0f, 77.4f, 2.7f),
            ("CamboataJovem", 65.1f, 24.1f, 2.0f), ("Bracatinga", 66.9f, 25.8f, 3.2f),
            ("Pitangueira", 65.7f, 29.0f, 2.8f),
            ("BracatingaJovem", 65.1f, 51.0f, 1.9f), ("Camboata", 67.1f, 52.4f, 3.4f),
            ("Pitangueira", 65.6f, 54.6f, 2.9f), ("Bracatinga", 67.2f, 56.1f, 3.2f),
            ("CamboataJovem", 65.1f, 65.1f, 2.0f), ("Camboata", 67.0f, 66.8f, 3.2f),
            ("BracatingaJovem", 65.6f, 69.0f, 2.2f),
            ("BracatingaJovem", 55.6f, 72.8f, 2.2f), ("Pitangueira", 54.2f, 74.4f, 3.1f),
            ("Camboata", 55.9f, 76.3f, 3.3f)
        });
    }

    private void PlantarArvoresNoGramado(string nome, (string especie, float x, float y, float largura)[] arvores)
    {
        const string vegetacao = "Art/RuaNorte/Vegetacao/";
        var bloco = new MaterialPropertyBlock();
        foreach (var (especie, x, y, largura) in arvores)
        {
            Vector2 pe = new(x, y);
            if (!GramaNoCentro(pe) || Physics2D.OverlapCircle(pe, .25f) != null) continue;
            var arvore = PecaPraca(nome, vegetacao + especie, x, y, largura);
            if (arvore == null) continue;
            ConfigurarVegetacaoVolumetricaPraca(arvore, especie, pe, largura / 3f);
            var relevo = Resources.Load<Texture2D>(vegetacao + "Relevos/" + especie);
            if (relevo != null)
                SantaRitaRuaNorte.ConfigurarCopa(arvore, especie, relevo, .06f, bloco);
        }
    }

    private void AnimarCopasDasRuas()
    {
        var rua = Array.Find(gameObject.scene.GetRootGameObjects(), obj => obj.name == "Rua vertical - arte");
        if (rua == null) return;

        var relevos = new Dictionary<string, Texture2D>();
        var bloco = new MaterialPropertyBlock();
        foreach (var copa in rua.GetComponentsInChildren<SpriteRenderer>())
        {
            string especie = copa.name;
            if (copa.sprite == null) continue;
            if (especie.StartsWith("Arrimo", StringComparison.Ordinal) || especie == "BalizadorCentro")
            {
                RegistrarSilhuetaDoCentro(copa, .85f, .20f, 1f);
                ConfigurarNevoaDoCenario(copa);
            }
            else if (especie is "PedrasCentro" or "GrelhaCentro" or "TampaCentro")
                ConfigurarNevoaDoCenario(copa);
            if (especie is not ("Camboata" or "Pitangueira" or "Bracatinga" or "Araucaria" or
                "CamboataJovem" or "BracatingaJovem")) continue;
            RegistrarSilhuetaDoCentro(copa, .85f, .17f, .66f);

            if (!relevos.TryGetValue(especie, out var relevo))
            {
                relevo = Resources.Load<Texture2D>("Art/RuaNorte/Vegetacao/Relevos/" + especie);
                relevos.Add(especie, relevo);
            }
            if (relevo == null) continue;

            var pe = copa.transform.position;
            copa.sharedMaterial = materialDoPiso;
            copa.GetPropertyBlock(bloco);
            bloco.SetTexture("_MainTex", copa.sprite.texture);
            bloco.SetVector("_Receiver", new Vector4(pe.y, .85f, 0, 0));
            copa.SetPropertyBlock(bloco);
            SantaRitaRuaNorte.ConfigurarCopa(copa, especie, relevo, .092f * copa.transform.lossyScale.x, bloco);
        }
    }

    private bool GramaNoCentro(Vector2 pe) =>
        pisoDoCentro != null && pisoDoCentro.GetTile(pisoDoCentro.WorldToCell(pe))?.name == "Grama";

    private void MobiliaDosAcessos(Tilemap terreno)
    {
        const string props = Art + "Centro/";
        // Removidos os dois canteiros avulsos em (37.8, 62.0) e (52.1, 62.0) que ficavam
        // soltos no meio das alamedas de pedregulho da Matriz.
        (string sprite, string nome, float x, float y, float largura)[] pontos =
        {
            ("Banco", "Banco do pátio da Prefeitura", 83.3f, 65.8f, 1.65f),
            ("Floreira", "Floreira da Prefeitura", 90.3f, 64.7f, .92f),
            ("Banco", "Banco de espera da Rodoviária", 70.7f, 19.85f, 1.60f),
            ("Floreira", "Floreira da espera da Rodoviária", 72.9f, 21.1f, .88f),
            ("Floreira", "Floreira da costureira", 23.5f, 26.1f, .83f),
            ("Banco", "Banco junto à oficina", 14.3f, 31.3f, 1.52f),
            ("Balizador", "Balizador do acesso ao Beira-Rio", 10.2f, 10.85f, .39f),
            ("Balizador", "Balizador da subida norte", 26.1f, 80.2f, .39f)
        };
        foreach (var (sprite, nome, x, y, largura) in pontos)
        {
            Vector2 pe = new(x, y);
            string piso = terreno.GetTile(terreno.WorldToCell(pe))?.name;
            if (piso is not ("Calcada" or "Pedregulho") ||
                Physics2D.OverlapBox(pe, new Vector2(largura, .42f), 0) != null) continue;
            var peca = PecaPraca(nome, props + sprite, x, y, largura);
            OcuparBasePraca(peca, new Vector2(largura * .83f, .40f));
            if (peca != null)
                ConfigurarSpriteVolumetricoPraca(peca, peca.bounds.min.y, .85f, 0f, 0f, .40f, modoOclusor: -1);
        }
    }

    private void FaixaPraca(int x, int y, bool vertical, int layer, int order)
    {
        var go = new GameObject("Pintura gasta da pista", typeof(SpriteRenderer));
        go.transform.SetParent(transform, false);
        go.transform.position = new Vector2(x + .5f, y + .5f);
        go.transform.localScale = vertical ? new Vector3(.063f, .72f, 1) : new Vector3(.72f, .063f, 1);
        var r = go.GetComponent<SpriteRenderer>();
        r.sprite = Quad();
        r.sharedMaterial = lit;
        r.color = new Color(.53f, .48f, .31f, .45f);
        r.sortingLayerID = layer;
        r.sortingOrder = order;
    }
}

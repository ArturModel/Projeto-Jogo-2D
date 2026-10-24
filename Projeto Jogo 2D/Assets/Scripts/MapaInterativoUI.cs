using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Mapa Interativo de Santa Rita (Tecla M) + Diário de Investigação e Inventário de Fichas.
/// Usa a arte oficial do mapa de Santa Rita (inspirada na estrutura de Zelda: Ocarina of Time com
/// entrada ao sul/oeste, chefes nos cantos e Clube de Santa Rita no topo oposto) combinada com
/// radar dinâmico estilo GTA San Andreas: blips nítidos, rota pontilhada GPS, waypoint pulsante,
/// seleção interativa de pontos e posição de Marcus sincronizada com as ruas reais da cidade.
/// </summary>
public class MapaInterativoUI : MonoBehaviour
{
    public static MapaInterativoUI Instance { get; private set; }
    public static bool IsShowing => Instance != null && Instance.painelRaiz != null && Instance.painelRaiz.activeSelf;

    private GameObject painelRaiz;
    private GameObject hudHintGo;
    private TextMeshProUGUI hudHintText;

    private RectTransform mapAreaRect;
    private RectTransform mapViewportRect;
    private readonly Vector2 mapBaseSize = new Vector2(412f, 309f);
    private float mapZoom = 1f;
    private bool arrastandoMapa;
    private Vector2 posInicialClique;
    private bool arrastouNoCliqueAtual;
    private bool ocultarBlipsNoMapa;
    private TextMeshProUGUI textoZoomIndicador;
    private Image mapBackgroundImage;
    private RectTransform playerMarkerRect;
    private Image playerMarkerImage;
    private RectTransform objectivePulseRect;
    private Image objectivePulseImage;
    private readonly List<RectTransform> routeDots = new List<RectTransform>();

    private TextMeshProUGUI textoLocalAtual;
    private TextMeshProUGUI textoObjetivoAtual;
    private TextMeshProUGUI textoDiarioConteudo;
    private TextMeshProUGUI textoStatusSave;
    private ScrollRect diarioScroll;

    private int abaAtiva = 0; // 0 = Radar, 1 = Diário, 2 = Fichas, 3 = Legenda legível
    private readonly Button[] botoesAba = new Button[4];
    private string pontoSelecionadoId = "";

    private struct PontoMapa
    {
        public string id;
        public string nome;
        public string descricao;
        public Vector2 uv;
        public Color cor;
        public string tipo;
    }

    private readonly List<PontoMapa> pontos = new List<PontoMapa>();
    private readonly Dictionary<string, Image> iconesPontos = new Dictionary<string, Image>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetStatic()
    {
        Instance = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoBootstrap()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        VerificarCenaECriar(SceneManager.GetActiveScene().name);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        VerificarCenaECriar(scene.name);
    }

    private static void VerificarCenaECriar(string sceneName)
    {
        bool cenaExploracao = sceneName == "EntradaDaCidade" || sceneName == "CasaDoAvo" || sceneName == "BarDoZe" || Capitulo2Director.CenaDoCapitulo2(sceneName);
        if (!cenaExploracao)
        {
            if (Instance != null && Instance.hudHintGo != null)
                Instance.hudHintGo.SetActive(false);
            FecharSeAberto();
            return;
        }

        EnsureInstance();
        if (Instance.hudHintGo != null)
            Instance.hudHintGo.SetActive(true);
    }

    public static MapaInterativoUI EnsureInstance()
    {
        if (Instance != null) return Instance;
        var go = new GameObject("MapaInterativoUI");
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(go);
        }
        Instance = go.AddComponent<MapaInterativoUI>();
        Instance.InicializarPontos();
        Instance.ConstruirUI();
        return Instance;
    }

    public static void FecharSeAberto()
    {
        if (Instance != null && Instance.painelRaiz != null && Instance.painelRaiz.activeSelf)
            Instance.Fechar();
    }

    private void InicializarPontos()
    {
        pontos.Clear();

        // Coordenadas UV calibradas exatamente sobre a arte oficial de Santa Rita (MapaSantaRitaArte.jpg)
        // e sincronizadas com os prédios reais da cena EntradaDaCidade:
        pontos.Add(new PontoMapa
        {
            id = "entrada",
            nome = "ENTRADA / CARRO DE MARCUS",
            descricao = "Rodovia de chegada a Santa Rita. O carro de Marcus ficou estacionado antes da subida.",
            uv = SantaRitaGeography.UV("entrada"),
            cor = new Color(0.45f, 0.82f, 1.00f),
            tipo = "inicio"
        });
        pontos.Add(new PontoMapa
        {
            id = "posto",
            nome = "POSTO DA ENTRADA",
            descricao = "Posto logo na chegada. O frentista conhece a rotina dos moradores e dá referências das ruas.",
            uv = SantaRitaGeography.UV("posto"),
            cor = new Color(0.98f, 0.82f, 0.32f),
            tipo = "posto"
        });
        pontos.Add(new PontoMapa
        {
            id = "orelhao_sul",
            nome = "ORELHÃO (AVENIDA / VENDA)",
            descricao = "Telefone público com linha interna ativa. Único meio de salvar o progresso manualmente.",
            uv = SantaRitaGeography.UV("orelhao_sul"),
            cor = new Color(0.30f, 0.95f, 0.52f),
            tipo = "orelhao"
        });
        pontos.Add(new PontoMapa
        {
            id = "mercadinho",
            nome = "MERCADINHO SANTA RITA",
            descricao = "Venda da encruzilhada. A atendente viu Adílio comprar café e velas hoje cedo.",
            uv = SantaRitaGeography.UV("mercadinho"),
            cor = new Color(0.98f, 0.64f, 0.24f),
            tipo = "mercado"
        });
        pontos.Add(new PontoMapa
        {
            id = "casa_avo",
            nome = "CASA DE ADÍLIO (BAIRRO, Nº 40)",
            descricao = "Casa do avô de Marcus no Bairro das Casas. Endereço do avô no Bairro das Casas.",
            uv = SantaRitaGeography.UV("casa_avo"),
            cor = new Color(1.00f, 0.88f, 0.34f),
            tipo = "casa"
        });
        pontos.Add(new PontoMapa
        {
            id = "bar_ze",
            nome = "BAR DO ZÉ [1º CHEFE — SINUCA]",
            descricao = "Bar na Rua Norte. Zé guarda o portão do Centro e exige duelo Melhor de 3 ('A Bola Marcada').",
            uv = SantaRitaGeography.UV("bar_ze"),
            cor = new Color(0.98f, 0.32f, 0.28f),
            tipo = "boss1"
        });
        pontos.Add(new PontoMapa
        {
            id = "orelhao_bar",
            nome = "ORELHÃO (RUA NORTE / BAR DO ZÉ)",
            descricao = "Telefone público ao lado do Bar do Zé. Recomendado salvar antes da partida de sinuca.",
            uv = SantaRitaGeography.UV("orelhao_bar"),
            cor = new Color(0.30f, 0.95f, 0.52f),
            tipo = "orelhao"
        });
        pontos.Add(new PontoMapa
        {
            id = "centro",
            nome = "CENTRO DE SANTA RITA",
            descricao = "Coração da cidade. Bloqueado pelo portão da Rua Norte até derrotar Zé e obter a 1ª Ficha do Clube.",
            uv = SantaRitaGeography.UV("centro"),
            cor = new Color(0.88f, 0.84f, 0.74f),
            tipo = "centro"
        });
        pontos.Add(new PontoMapa
        {
            id = "canto_se",
            nome = "CANCHA DE BOCHA [CANTO SUDOESTE]",
            descricao = "Reduto tradicionalista no canto sul. Um dos chefes guardiões das Fichas do Clube.",
            uv = SantaRitaGeography.UV("canto_se"),
            cor = new Color(0.75f, 0.58f, 0.42f),
            tipo = "boss_canto"
        });
        pontos.Add(new PontoMapa
        {
            id = "capela",
            nome = "CAPELA ANTIGA [EXTREMO SUL]",
            descricao = "Capela isolada ao sul da mata. Guarda registros paroquiais da noite de 1998.",
            uv = SantaRitaGeography.UV("capela"),
            cor = new Color(0.75f, 0.58f, 0.42f),
            tipo = "boss_canto"
        });
        pontos.Add(new PontoMapa
        {
            id = "truco",
            nome = "GALPÃO DO TRUCO [CANTO SUDESTE]",
            descricao = "Galpão nos fundos da cidade onde corre o jogo pesado de Truco.",
            uv = SantaRitaGeography.UV("truco"),
            cor = new Color(0.75f, 0.58f, 0.42f),
            tipo = "boss_canto"
        });
        pontos.Add(new PontoMapa
        {
            id = "cemiterio",
            nome = "CEMITÉRIO MUNICIPAL [NORTE]",
            descricao = "Cemitério silencioso na encosta norte, entre o Bar do Zé e o Clube.",
            uv = SantaRitaGeography.UV("cemiterio"),
            cor = new Color(0.68f, 0.66f, 0.72f),
            tipo = "boss_canto"
        });
        pontos.Add(new PontoMapa
        {
            id = "prefeitura",
            nome = "PREFEITURA / ARQUIVO",
            descricao = "Passagem institucional entre o Centro e a colina do Clube de Santa Rita.",
            uv = SantaRitaGeography.UV("prefeitura"),
            cor = new Color(0.78f, 0.72f, 0.58f),
            tipo = "boss_canto"
        });
        pontos.Add(new PontoMapa
        {
            id = "clube",
            nome = "CLUBE DE SANTA RITA [CHEFE FINAL]",
            descricao = "No extremo oposto à entrada da cidade. Onde a cúpula se reúne e a verdade sobre as 03:17 é guardada.",
            uv = SantaRitaGeography.UV("clube"),
            cor = new Color(0.95f, 0.18f, 0.22f),
            tipo = "clube"
        });
        pontos.Add(new PontoMapa { id = "igreja", nome = "IGREJA DA MATRIZ", descricao = "Nave e testemunho do padre; acima da praça.", uv = SantaRitaGeography.UV("igreja"), cor = new Color(.9f,.83f,.6f), tipo = "centro" });
        pontos.Add(new PontoMapa { id = "rodoviaria", nome = "RODOVIÁRIA", descricao = "Espera, guichê e telefone do plantão; inferior/leste do Centro.", uv = SantaRitaGeography.UV("rodoviaria"), cor = new Color(.6f,.8f,.75f), tipo = "centro" });
        pontos.Add(new PontoMapa { id = "orelhao_centro", nome = "ORELHÃO DA PRAÇA", descricao = "Save exclusivamente manual, junto aos bancos.", uv = SantaRitaGeography.UV("orelhao_centro"), cor = new Color(.3f,.95f,.52f), tipo = "orelhao" });
        pontos.Add(new PontoMapa { id = "orelhao_bocha", nome = "ORELHÃO DA BEIRA-RIO", descricao = "Disponível antes e depois de Tonão; salvar não consome fichas.", uv = SantaRitaGeography.UV("orelhao_bocha"), cor = new Color(.3f,.95f,.52f), tipo = "orelhao" });
    }

    private void ConstruirUI()
    {
        Canvas canvas = UIFactory.CreateCanvas(transform, "MapaInterativoCanvas", 8500);
        canvas.gameObject.AddComponent<GraphicRaycaster>();

        // Hint discreto no canto superior direito durante a exploração
        hudHintGo = new GameObject("HudHintMapa", typeof(RectTransform), typeof(Image));
        hudHintGo.transform.SetParent(canvas.transform, false);
        var hhRect = hudHintGo.GetComponent<RectTransform>();
        hhRect.anchorMin = new Vector2(1f, 1f);
        hhRect.anchorMax = new Vector2(1f, 1f);
        hhRect.pivot = new Vector2(1f, 1f);
        hhRect.anchoredPosition = new Vector2(-10f, -10f);
        hhRect.sizeDelta = new Vector2(154f, 22f);
        hudHintGo.GetComponent<Image>().color = new Color(0.06f, 0.06f, 0.08f, 0.84f);

        hudHintText = CriarTexto("HintTxt", hudHintGo.transform, "[M] Mapa / Diário", 14, new Color(0.95f, 0.85f, 0.52f, 0.98f), TextAlignmentOptions.Center);
        UIFactory.Stretch(hudHintText.rectTransform);

        // Painel principal do Mapa
        painelRaiz = new GameObject("PainelMapaRaiz", typeof(RectTransform), typeof(Image));
        painelRaiz.transform.SetParent(canvas.transform, false);
        UIFactory.Stretch(painelRaiz.GetComponent<RectTransform>());
        painelRaiz.GetComponent<Image>().color = new Color(0.02f, 0.03f, 0.04f, 0.92f);

        var moldura = new GameObject("MolduraMapa", typeof(RectTransform), typeof(Image));
        moldura.transform.SetParent(painelRaiz.transform, false);
        var mRect = moldura.GetComponent<RectTransform>();
        Posicionar(mRect, Vector2.zero, new Vector2(624f, 348f));
        moldura.GetComponent<Image>().color = new Color(0.13f, 0.11f, 0.09f, 0.99f);

        var titulo = CriarTexto("TituloMapa", moldura.transform, "MAPA TÁTICO — SANTA RITA (2004)", 17, new Color(0.96f, 0.86f, 0.54f), TextAlignmentOptions.Left);
        Posicionar(titulo.rectTransform, new Vector2(-172f, 160f), new Vector2(260f, 20f));

        var subFechar = CriarTexto("SubFechar", moldura.transform, "[Roda / + -] Zoom  •  [Segurar Mouse Esq. e Arrastar / WASD] Mover  •  [M / ESC] Fechar", 10.5f, new Color(0.84f, 0.80f, 0.70f), TextAlignmentOptions.Right);
        Posicionar(subFechar.rectTransform, new Vector2(118f, 160f), new Vector2(368f, 20f));

        // Esquerda: Área Gráfica do Mapa 2D em proporção 4:3 fiel à arte original (412 x 309)
        var viewport = new GameObject("MapViewport", typeof(RectTransform), typeof(Image), typeof(RectMask2D));
        viewport.transform.SetParent(moldura.transform, false);
        mapViewportRect = viewport.GetComponent<RectTransform>();
        Posicionar(mapViewportRect, new Vector2(-98f, -6f), mapBaseSize);
        viewport.GetComponent<Image>().color = new Color(0.06f, 0.08f, 0.07f, 1f);

        var mapContainer = new GameObject("MapContainer", typeof(RectTransform));
        mapContainer.transform.SetParent(viewport.transform, false);
        mapAreaRect = mapContainer.GetComponent<RectTransform>();
        Posicionar(mapAreaRect, Vector2.zero, mapBaseSize);

        var mapArtGo = new GameObject("MapArt", typeof(RectTransform), typeof(Image));
        mapArtGo.transform.SetParent(mapContainer.transform, false);
        UIFactory.Stretch(mapArtGo.GetComponent<RectTransform>());
        mapBackgroundImage = mapArtGo.GetComponent<Image>();
        mapBackgroundImage.sprite = CarregarOuGerarArteMapa();
        mapBackgroundImage.preserveAspect = false;
        // Bíblia R1 corrige os letreiros da imagem histórica. Texto TMP mantém
        // nitidez no zoom e o arquivo da arte permanece intacto.
        RotuloCanonico("PREFEITURA", new Vector2(.705f, .704f));
        RotuloCanonico("ESCOLA", new Vector2(.622f, .610f));

        // Trilha pontilhada estilo GPS (do jogador até o objetivo atual)
        Sprite dotSprite = GerarIconePontoRota();
        for (int i = 0; i < 14; i++)
        {
            var dotGo = new GameObject("RouteDot_" + i, typeof(RectTransform), typeof(Image));
            dotGo.transform.SetParent(mapAreaRect, false);
            var dr = dotGo.GetComponent<RectTransform>();
            dr.sizeDelta = new Vector2(4f, 4f);
            var dImg = dotGo.GetComponent<Image>();
            dImg.sprite = dotSprite;
            dImg.color = new Color(1f, 0.88f, 0.25f, 0.85f);
            dImg.raycastTarget = false;
            routeDots.Add(dr);
        }

        // Blips interativos estilo GTA San Andreas (sem poluir o texto já desenhado na arte do mapa)
        for (int i = 0; i < pontos.Count; i++)
            CriarMarcadorPonto(mapAreaRect, pontos[i]);

        // Waypoint Pulsante de Objetivo (estilo GTA San Andreas)
        var objGo = new GameObject("WaypointObjetivo", typeof(RectTransform), typeof(Image));
        objGo.transform.SetParent(mapAreaRect, false);
        objectivePulseRect = objGo.GetComponent<RectTransform>();
        objectivePulseRect.sizeDelta = new Vector2(22f, 22f);
        objectivePulseImage = objGo.GetComponent<Image>();
        objectivePulseImage.sprite = GerarIconeAlvo();
        objectivePulseImage.color = new Color(1f, 0.90f, 0.18f, 0.98f);
        objectivePulseImage.raycastTarget = false;

        // Marcador da Posição Atual de Marcus (tempo real)
        var playerGo = new GameObject("MarcadorMarcus", typeof(RectTransform), typeof(Image));
        playerGo.transform.SetParent(mapAreaRect, false);
        playerMarkerRect = playerGo.GetComponent<RectTransform>();
        playerMarkerRect.sizeDelta = new Vector2(16f, 16f);
        playerMarkerImage = playerGo.GetComponent<Image>();
        playerMarkerImage.sprite = GerarIconeJogador();
        playerMarkerImage.color = new Color(0.20f, 0.96f, 1f, 1f);
        playerMarkerImage.raycastTarget = false;

        var playerLbl = CriarTexto("LblVoce", playerMarkerRect, "VOCÊ", 10, new Color(0.45f, 0.98f, 1f, 1f), TextAlignmentOptions.Center);
        Posicionar(playerLbl.rectTransform, new Vector2(0f, -11f), new Vector2(48f, 12f));
        playerLbl.raycastTarget = false;

        // Barra flutuante de controles de Zoom/Arraste na base do Viewport do Mapa
        var barraZoom = new GameObject("BarraControlesMapa", typeof(RectTransform), typeof(Image));
        barraZoom.transform.SetParent(viewport.transform, false);
        var bzRect = barraZoom.GetComponent<RectTransform>();
        Posicionar(bzRect, new Vector2(0f, -141f), new Vector2(404f, 22f));
        barraZoom.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.06f, 0.88f);

        CriarBotaoControleMapa(barraZoom.transform, "Zoom +", new Vector2(-172f, 0f), new Vector2(52f, 18f), () => AplicarZoomIncremental(1.3f, Vector2.zero));
        CriarBotaoControleMapa(barraZoom.transform, "Zoom -", new Vector2(-114f, 0f), new Vector2(52f, 18f), () => AplicarZoomIncremental(1f / 1.3f, Vector2.zero));
        CriarBotaoControleMapa(barraZoom.transform, "1x Tudo", new Vector2(-56f, 0f), new Vector2(54f, 18f), ResetarZoomEPan);
        CriarBotaoControleMapa(barraZoom.transform, "Focar Você", new Vector2(10f, 0f), new Vector2(70f, 18f), CentralizarNoJogador);
        CriarBotaoControleMapa(barraZoom.transform, "Ícones On/Off", new Vector2(88f, 0f), new Vector2(78f, 18f), AlternarVisibilidadeBlips);

        textoZoomIndicador = CriarTexto("TxtZoom", barraZoom.transform, "1.0x | Arraste c/ mouse", 10f, new Color(0.92f, 0.84f, 0.56f), TextAlignmentOptions.Right);
        Posicionar(textoZoomIndicador.rectTransform, new Vector2(164f, 0f), new Vector2(72f, 18f));

        // Direita: Painel Lateral (Radar / Diário / Fichas)
        var sidePanel = new GameObject("SidePanel", typeof(RectTransform), typeof(Image));
        sidePanel.transform.SetParent(moldura.transform, false);
        var spRect = sidePanel.GetComponent<RectTransform>();
        Posicionar(spRect, new Vector2(208f, -6f), new Vector2(188f, 309f));
        sidePanel.GetComponent<Image>().color = new Color(0.09f, 0.08f, 0.07f, 0.98f);

        botoesAba[0] = CriarBotaoAba(sidePanel.transform, "RADAR", new Vector2(-66f, 138f), () => SelecionarAba(0));
        botoesAba[1] = CriarBotaoAba(sidePanel.transform, "DIÁRIO", new Vector2(-22f, 138f), () => SelecionarAba(1));
        botoesAba[2] = CriarBotaoAba(sidePanel.transform, "FICHAS", new Vector2(22f, 138f), () => SelecionarAba(2));
        botoesAba[3] = CriarBotaoAba(sidePanel.transform, "LEGENDA", new Vector2(66f, 138f), () => SelecionarAba(3));

        textoLocalAtual = CriarTexto("TextoLocal", sidePanel.transform, "", 12, new Color(0.55f, 0.88f, 0.98f), TextAlignmentOptions.TopLeft);
        Posicionar(textoLocalAtual.rectTransform, new Vector2(0f, 108f), new Vector2(174f, 20f));

        textoObjetivoAtual = CriarTexto("TextoObjetivo", sidePanel.transform, "", 12, new Color(0.98f, 0.84f, 0.38f), TextAlignmentOptions.TopLeft);
        Posicionar(textoObjetivoAtual.rectTransform, new Vector2(0f, 73f), new Vector2(174f, 50f));
        textoObjetivoAtual.textWrappingMode = TextWrappingModes.Normal;

        textoDiarioConteudo = CriarTexto("TextoConteudo", sidePanel.transform, "", 11.5f, new Color(0.90f, 0.87f, 0.80f), TextAlignmentOptions.TopLeft);
        Posicionar(textoDiarioConteudo.rectTransform, new Vector2(0f, -28f), new Vector2(174f, 164f));
        textoDiarioConteudo.textWrappingMode = TextWrappingModes.Normal;
        var leituraGo = new GameObject("LeituraRolavel", typeof(RectTransform), typeof(Image), typeof(RectMask2D), typeof(ScrollRect));
        leituraGo.transform.SetParent(sidePanel.transform, false);
        var leituraRect = leituraGo.GetComponent<RectTransform>();
        Posicionar(leituraRect, new Vector2(0f, -37f), new Vector2(174f, 154f));
        leituraGo.GetComponent<Image>().color = new Color(.09f,.08f,.07f,.98f);
        textoDiarioConteudo.transform.SetParent(leituraGo.transform, false);
        var leituraTexto = textoDiarioConteudo.rectTransform;
        leituraTexto.anchorMin = new Vector2(0,1); leituraTexto.anchorMax = new Vector2(1,1);
        leituraTexto.pivot = new Vector2(.5f,1); leituraTexto.anchoredPosition = Vector2.zero; leituraTexto.sizeDelta = Vector2.zero;
        var tamanhoTexto = textoDiarioConteudo.gameObject.AddComponent<ContentSizeFitter>();
        tamanhoTexto.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        diarioScroll = leituraGo.GetComponent<ScrollRect>(); diarioScroll.viewport = leituraRect; diarioScroll.content = leituraTexto;
        diarioScroll.horizontal = false; diarioScroll.movementType = ScrollRect.MovementType.Clamped; diarioScroll.scrollSensitivity = 24;

        textoStatusSave = CriarTexto("TextoStatusSave", sidePanel.transform, "", 11f, new Color(0.72f, 0.82f, 0.68f), TextAlignmentOptions.BottomLeft);
        Posicionar(textoStatusSave.rectTransform, new Vector2(0f, -136f), new Vector2(174f, 28f));
        textoStatusSave.textWrappingMode = TextWrappingModes.Normal;

        painelRaiz.SetActive(false);
    }

    private Button CriarBotaoControleMapa(Transform pai, string rotulo, Vector2 pos, Vector2 tam, UnityEngine.Events.UnityAction acao)
    {
        var go = new GameObject("BtnMapa_" + rotulo, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(pai, false);
        Posicionar(go.GetComponent<RectTransform>(), pos, tam);
        var img = go.GetComponent<Image>();
        img.color = new Color(0.22f, 0.19f, 0.15f, 0.98f);

        var txt = CriarTexto("Lbl", go.transform, rotulo, 9.5f, new Color(0.96f, 0.90f, 0.72f), TextAlignmentOptions.Center);
        UIFactory.Stretch(txt.rectTransform);

        var btn = go.GetComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(acao);
        return btn;
    }

    private void RotuloCanonico(string texto, Vector2 uv)
    {
        var go = new GameObject("Rotulo_" + texto, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(mapAreaRect, false);
        var rect = go.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = uv;
        rect.sizeDelta = new Vector2(37, 8); rect.anchoredPosition = Vector2.zero;
        go.GetComponent<Image>().color = new Color(.04f, .055f, .045f, 1);
        go.GetComponent<Image>().raycastTarget = false;
        var label = CriarTexto("Nome", go.transform, texto, 5.5f, new Color(.88f, .86f, .75f), TextAlignmentOptions.Center);
        UIFactory.Stretch(label.rectTransform); label.raycastTarget = false;
    }

    private void CriarMarcadorPonto(RectTransform mapRect, PontoMapa p)
    {
        var go = new GameObject("Ponto_" + p.id, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(mapRect, false);
        var r = go.GetComponent<RectTransform>();
        r.anchorMin = p.uv;
        r.anchorMax = p.uv;
        r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = Vector2.zero;
        r.sizeDelta = (p.tipo == "clube" || p.tipo == "boss1") ? new Vector2(15f, 15f) : new Vector2(12f, 12f);

        var img = go.GetComponent<Image>();
        img.sprite = GerarIconeBlip(p.tipo, p.cor);
        img.color = Color.white;
        iconesPontos[p.id] = img;

        string idCapturado = p.id;
        var btn = go.GetComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(() =>
        {
            if (arrastouNoCliqueAtual) return;
            pontoSelecionadoId = idCapturado;
            SelecionarAba(0);
        });
    }

    private Button CriarBotaoAba(Transform pai, string rotulo, Vector2 pos, UnityEngine.Events.UnityAction acao)
    {
        var go = new GameObject("Aba_" + rotulo, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(pai, false);
        Posicionar(go.GetComponent<RectTransform>(), pos, new Vector2(42f, 20f));
        var img = go.GetComponent<Image>();
        img.color = new Color(0.20f, 0.18f, 0.15f, 1f);

        var txt = CriarTexto("Lbl", go.transform, rotulo, 9.5f, new Color(0.94f, 0.90f, 0.80f), TextAlignmentOptions.Center);
        UIFactory.Stretch(txt.rectTransform);
        txt.raycastTarget = true;

        var btn = go.GetComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(acao);
        return btn;
    }

    void Update()
    {
        if (Keyboard.current == null) return;
        if (MenuPausa.Aberto) return;

        string sceneName = SceneManager.GetActiveScene().name;
        bool cenaValida = sceneName == "EntradaDaCidade" || sceneName == "CasaDoAvo" || sceneName == "BarDoZe" || Capitulo2Director.CenaDoCapitulo2(sceneName);
        if (!cenaValida) return;
        if (hudHintText != null)
            hudHintText.text = GameState.Tem(Flags.MapaDoCarroObtido) ? "[M] Mapa / Diário" : "[E] Pegar Mapa no Carro";

        if (Keyboard.current.mKey.wasPressedThisFrame || Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (GameMessageUI.IsShowing || SinucaMinigameUI.IsShowing || BochaMinigameUI.IsShowing) return;
            if (!GameState.Tem(Flags.MapaDoCarroObtido))
            {
                Legenda.Dizer("Meu mapa e minha lanterna estão no carro marrom aqui atrás. Preciso examiná-lo com [E].", 3.5f);
                return;
            }

            if (IsShowing)
                Fechar();
            else
                Abrir();
        }

        if (IsShowing)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Fechar();
                return;
            }
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SelecionarAba(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SelecionarAba(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) SelecionarAba(2);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) SelecionarAba(3);

            AtualizarPosicaoMarcadoresDinamicos();
            AtualizarZoomMapa();
        }
    }

    private void AlternarVisibilidadeBlips()
    {
        ocultarBlipsNoMapa = !ocultarBlipsNoMapa;
        foreach (var kv in iconesPontos)
        {
            if (kv.Value != null)
                kv.Value.gameObject.SetActive(!ocultarBlipsNoMapa);
        }
        for (int i = 0; i < routeDots.Count; i++)
        {
            if (routeDots[i] != null)
                routeDots[i].gameObject.SetActive(!ocultarBlipsNoMapa);
        }
        if (objectivePulseRect != null)
            objectivePulseRect.gameObject.SetActive(!ocultarBlipsNoMapa);
    }

    private void ResetarZoomEPan()
    {
        mapZoom = 1f;
        if (mapAreaRect != null)
        {
            mapAreaRect.localScale = Vector3.one;
            mapAreaRect.sizeDelta = mapBaseSize;
            mapAreaRect.anchoredPosition = Vector2.zero;
        }
        AtualizarIndicadorZoom();
    }

    private void CentralizarNoJogador()
    {
        if (mapAreaRect == null || playerMarkerRect == null) return;
        if (mapZoom < 2.2f)
        {
            mapZoom = 2.4f;
            mapAreaRect.localScale = Vector3.one;
            mapAreaRect.sizeDelta = mapBaseSize * mapZoom;
        }
        Vector2 uv = playerMarkerRect.anchorMin;
        Vector2 offsetDoCentro = new Vector2((0.5f - uv.x) * mapBaseSize.x * mapZoom, (0.5f - uv.y) * mapBaseSize.y * mapZoom);
        mapAreaRect.anchoredPosition = offsetDoCentro;
        LimitarPanDoMapa();
        AtualizarIndicadorZoom();
    }

    private void AplicarZoomIncremental(float fator, Vector2 pontoFocalLocal)
    {
        if (mapAreaRect == null) return;
        float novoZoom = Mathf.Clamp(mapZoom * fator, 1f, 6f);
        if (!Mathf.Approximately(novoZoom, mapZoom))
        {
            float razao = novoZoom / mapZoom;
            mapAreaRect.anchoredPosition = (mapAreaRect.anchoredPosition - pontoFocalLocal) * razao + pontoFocalLocal;
            mapZoom = novoZoom;
            mapAreaRect.localScale = Vector3.one;
            mapAreaRect.sizeDelta = mapBaseSize * mapZoom;
            LimitarPanDoMapa();
            AtualizarIndicadorZoom();
        }
    }

    private void LimitarPanDoMapa()
    {
        if (mapAreaRect == null) return;
        Vector2 limite = mapBaseSize * Mathf.Max(0f, mapZoom - 1f) * 0.5f;
        mapAreaRect.anchoredPosition = new Vector2(
            Mathf.Clamp(mapAreaRect.anchoredPosition.x, -limite.x, limite.x),
            Mathf.Clamp(mapAreaRect.anchoredPosition.y, -limite.y, limite.y));
    }

    private void AtualizarIndicadorZoom()
    {
        if (textoZoomIndicador != null)
            textoZoomIndicador.text = $"{mapZoom:0.0}x";
    }

    private void AtualizarZoomMapa()
    {
        if (mapViewportRect == null || mapAreaRect == null) return;

        // Teclado: + / - para zoom, WASD / Setas para mover o mapa, C para centralizar, Z para resetar
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.equalsKey.wasPressedThisFrame || kb.numpadPlusKey.wasPressedThisFrame)
                AplicarZoomIncremental(1.25f, Vector2.zero);
            if (kb.minusKey.wasPressedThisFrame || kb.numpadMinusKey.wasPressedThisFrame)
                AplicarZoomIncremental(1f / 1.25f, Vector2.zero);
            if (kb.zKey.wasPressedThisFrame)
                ResetarZoomEPan();
            if (kb.cKey.wasPressedThisFrame)
                CentralizarNoJogador();
            if (kb.iKey.wasPressedThisFrame)
                AlternarVisibilidadeBlips();

            Vector2 dirTeclado = Vector2.zero;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) dirTeclado.y -= 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) dirTeclado.y += 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) dirTeclado.x += 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) dirTeclado.x -= 1f;

            if (dirTeclado.sqrMagnitude > 0.01f)
            {
                if (mapZoom <= 1.01f)
                {
                    // Se o jogador tentar mover com WASD em 1x, aplica um leve zoom automático para permitir explorar
                    AplicarZoomIncremental(1.8f, Vector2.zero);
                }
                mapAreaRect.anchoredPosition += dirTeclado.normalized * (240f * Time.unscaledDeltaTime);
                LimitarPanDoMapa();
            }
        }

        if (Mouse.current == null) return;
        Vector2 mouse = Mouse.current.position.ReadValue();
        bool dentroDoMapa = RectTransformUtility.RectangleContainsScreenPoint(mapViewportRect, mouse, null);

        // Zoom na roda do mouse focado no cursor
        float scrollY = Mouse.current.scroll.ReadValue().y;
        if (dentroDoMapa && Mathf.Abs(scrollY) > 0.01f)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(mapViewportRect, mouse, null, out Vector2 local);
            AplicarZoomIncremental(scrollY > 0f ? 1.22f : (1f / 1.22f), local);
        }

        // Arrastar segurando o botão ESQUERDO, direito ou do meio do mouse
        bool botaoPressionado = Mouse.current.leftButton.isPressed ||
                                Mouse.current.rightButton.isPressed ||
                                Mouse.current.middleButton.isPressed;

        bool botaoIniciou = Mouse.current.leftButton.wasPressedThisFrame ||
                            Mouse.current.rightButton.wasPressedThisFrame ||
                            Mouse.current.middleButton.wasPressedThisFrame;

        if (botaoIniciou && dentroDoMapa)
        {
            arrastandoMapa = true;
            posInicialClique = mouse;
            arrastouNoCliqueAtual = false;
        }

        if (!botaoPressionado)
        {
            arrastandoMapa = false;
        }

        if (arrastandoMapa && botaoPressionado)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            if (delta.sqrMagnitude > 0.001f)
            {
                if (Vector2.Distance(mouse, posInicialClique) > 5f)
                {
                    arrastouNoCliqueAtual = true;
                    // Se o jogador tentar arrastar o mapa com o mouse ainda em 1.0x, dá um zoom suave automático para ele conseguir arrastar!
                    if (mapZoom <= 1.02f)
                    {
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(mapViewportRect, mouse, null, out Vector2 localDrag);
                        AplicarZoomIncremental(2.0f, localDrag);
                    }
                }

                Canvas canvas = mapViewportRect.GetComponentInParent<Canvas>();
                float escala = (canvas != null && canvas.scaleFactor > 0.01f) ? canvas.scaleFactor : 1f;
                mapAreaRect.anchoredPosition += delta / escala;
                LimitarPanDoMapa();
            }
        }
    }

    public void Abrir(int abaInicial = -1)
    {
        if (!GameState.Tem(Flags.MapaDoCarroObtido))
        {
            Legenda.Dizer("Meu mapa está no carro marrom logo atrás. Preciso examiná-lo com [E].", 3.5f);
            return;
        }
        if (abaInicial >= 0)
        {
            abaAtiva = Mathf.Clamp(abaInicial, 0, 3);
        }
        painelRaiz.SetActive(true);
        if (mapAreaRect != null)
        {
            mapAreaRect.localScale = Vector3.one;
            mapAreaRect.sizeDelta = mapBaseSize * mapZoom;
            LimitarPanDoMapa();
        }
        AtualizarIndicadorZoom();
        SelecionarAba(abaAtiva);
        AtualizarPosicaoMarcadoresDinamicos();
    }

    public void Fechar()
    {
        arrastandoMapa = false;
        if (painelRaiz != null)
            painelRaiz.SetActive(false);
    }

    private void SelecionarAba(int indice)
    {
        if (diarioScroll != null) diarioScroll.verticalNormalizedPosition = 1;
        abaAtiva = Mathf.Clamp(indice, 0, 3);
        for (int i = 0; i < botoesAba.Length; i++)
        {
            if (botoesAba[i] != null)
            {
                var img = botoesAba[i].GetComponent<Image>();
                img.color = (i == abaAtiva)
                    ? new Color(0.72f, 0.25f, 0.18f, 1f)
                    : new Color(0.18f, 0.16f, 0.14f, 1f);
            }
        }
        AtualizarPainelLateral();
    }

    /// <summary>
    /// Converte a coordenada do mundo nas 5 Zonas Externas e 4 Interiores do Capítulo 1
    /// para a coordenada UV exata nas estradas da arte oficial de Santa Rita (MapaSantaRitaArte.jpg).
    /// </summary>
    private Vector2 ConverterMundoParaUV(Vector3 pos, out string nomeLocal) =>
        SantaRitaGeography.ConverterCapitulo1(pos, out nomeLocal);

    private void AtualizarPosicaoMarcadoresDinamicos()
    {
        string scene = SceneManager.GetActiveScene().name;
        Vector2 playerUV = new Vector2(0.110f, 0.640f);
        string nomeLocal = "Entrada de Santa Rita";

        if (Capitulo2Director.Instance != null)
        {
            var playerC2 = FindAnyObjectByType<PlayerController>();
            Capitulo2Director.Instance.PosicaoNoMapa(playerC2 != null ? playerC2.transform.position : Vector3.zero, out playerUV, out nomeLocal);
        }
        else if (scene == "CasaDoAvo")
        {
            playerUV = SantaRitaGeography.UV("casa_avo");
            nomeLocal = "Interior — Casa de Adílio (Nº 40)";
        }
        else if (scene == "BarDoZe")
        {
            playerUV = SantaRitaGeography.UV("bar_ze");
            nomeLocal = "Interior — Bar do Zé (Mesa de Sinuca)";
        }
        else
        {
            var player = FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                playerUV = ConverterMundoParaUV(player.transform.position, out nomeLocal);
            }
        }

        if (playerMarkerRect != null)
        {
            playerMarkerRect.anchorMin = playerUV;
            playerMarkerRect.anchorMax = playerUV;
            playerMarkerRect.anchoredPosition = Vector2.zero;
            float blink = 0.75f + 0.25f * Mathf.Sin(Time.unscaledTime * 6f);
            playerMarkerImage.color = new Color(0.25f, 0.96f, 1f, blink);
        }

        Vector2 targetUV = ObterUVObjetivoAtual();
        if (objectivePulseRect != null)
        {
            objectivePulseRect.anchorMin = targetUV;
            objectivePulseRect.anchorMax = targetUV;
            objectivePulseRect.anchoredPosition = Vector2.zero;
            float scale = 1f + 0.25f * Mathf.Sin(Time.unscaledTime * 5.5f);
            objectivePulseRect.localScale = new Vector3(scale, scale, 1f);
        }

        // Atualiza a trilha pontilhada estilo GPS entre Marcus e o objetivo ativo
        for (int i = 0; i < routeDots.Count; i++)
        {
            float t = (i + 1f) / (routeDots.Count + 1f);
            Vector2 posUV = Vector2.Lerp(playerUV, targetUV, t);
            var dr = routeDots[i];
            dr.anchorMin = posUV;
            dr.anchorMax = posUV;
            dr.anchoredPosition = Vector2.zero;
            float wave = 0.45f + 0.55f * Mathf.Sin(Time.unscaledTime * 7f - i * 0.55f);
            dr.GetComponent<Image>().color = new Color(1f, 0.88f, 0.22f, wave);
        }

        string objStr = GameState.ObterObjetivo();
        if (string.IsNullOrEmpty(objStr))
            objStr = "Investigar a casa do avô Adílio no Bairro das Casas, nº 40.";

        if (textoLocalAtual != null)
            textoLocalAtual.text = "LOCAL: " + nomeLocal.ToUpperInvariant();
        if (textoObjetivoAtual != null)
            textoObjetivoAtual.text = "OBJETIVO: " + objStr;
    }

    private Vector2 ObterUVObjetivoAtual() => SantaRitaGeography.ObjetivoAtual();

    private void AtualizarPainelLateral()
    {
        if (textoStatusSave != null)
        {
            textoStatusSave.text = GameState.TemProgressoNaoSalvo
                ? "Save: Progresso em RAM (use um Orelhão [TEL])"
                : (CheckpointSystem.HasCheckpoint ? "Save: Gravado no último Orelhão [TEL]" : "Save: Nenhum Orelhão usado ainda");
        }

        textoDiarioConteudo.fontSize = abaAtiva == 3 ? 10.5f : 11.5f;
        if (abaAtiva == 0)
        {
            string infoSelecionado = "";
            if (!string.IsNullOrEmpty(pontoSelecionadoId))
            {
                for (int i = 0; i < pontos.Count; i++)
                {
                    if (pontos[i].id == pontoSelecionadoId)
                    {
                        infoSelecionado = $"<b><color=#FFD848>> {pontos[i].nome}</color></b>\n{pontos[i].descricao}\n\n";
                        break;
                    }
                }
            }

            string statusCentro = GameState.Tem(Flags.ZeDerrotadoNaSinuca)
                ? "<color=#58E878>Portão do Centro: DESTRANCADO</color>"
                : "<color=#F06858>Portão do Centro: TRANCADO por Zé</color>";

            textoDiarioConteudo.text =
                infoSelecionado +
                "<b>LEGENDA DO RADAR:</b>\n" +
                "<color=#48F0FF>[VOCÊ] MARCUS</color> — Posição atual\n" +
                "<color=#FFE030>[ALVO] ROTA GPS</color> — Objetivo ativo\n" +
                "<color=#48F080>[TEL] ORELHÃO</color> — Save Manual\n" +
                "<color=#FFE058>[40] CASA DE ADÍLIO</color> — Bairro, nº 40\n" +
                "<color=#F85848>[8] BAR DO ZÉ</color> — 1º Chefe (Sinuca)\n" +
                "<color=#F83038>[*] CLUBE</color> — Topo oposto (Chefe Final)\n\n" +
                statusCentro;
        }
        else if (abaAtiva == 1)
        {
            var entradas = GameState.ObterEntradasDiario();
            if (entradas.Count == 0)
            {
                textoDiarioConteudo.text =
                    "<b>FATOS OBSERVADOS:</b>\n" +
                    "- Outubro de 2004: Adílio parou de atender o telefone hoje cedo.\n" +
                    "- Parei o carro na entrada de Santa Rita.\n\n" +
                    "<b>PISTAS & HIPÓTESES:</b>\n" +
                    "- Converse no Posto e examine a Casa de Adílio (nº 40).";
                return;
            }

            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < entradas.Count; i++)
            {
                var e = entradas[i];
                if (e.categoria == "item") continue;
                string catUpper = (e.categoria ?? "").ToUpperInvariant();
                string tagCor = catUpper.Contains("FATO") ? "#88C8F0" : (catUpper.Contains("HIP") ? "#E8B068" : "#98E898");
                sb.Append("<color=").Append(tagCor).Append(">[").Append(catUpper).Append("] ").Append(e.titulo).Append(":</color>\n");
                sb.Append(e.descricao).Append("\n\n");
            }
            textoDiarioConteudo.text = sb.ToString();
        }
        else if (abaAtiva == 2)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("<b>FICHAS DO CLUBE & PROVAS:</b>\n\n");

            if (GameState.Tem(Flags.RecebeuFichaClubeZe))
            {
                sb.Append("<color=#FFD848>[*] FICHA DO CLUBE #01 (ZÉ)</color>\n");
                sb.Append("ID: ficha_clube_ze_01\n");
                sb.Append("Ficha metálica cunhada pelo Clube de Santa Rita, conquistada de Zé na Melhor de 3 da Sinuca. Abre acesso ao Centro.\n\n");
            }
            else
            {
                sb.Append("<color=#888278>[ ] Ficha #01 — Bar do Zé (Pendente)</color>\n");
                sb.Append("<color=#888278>[ ] Ficha #03 — Galpão do Truco</color>\n\n");
            }

            if (GameState.Tem(Capitulo2Director.Ficha))
            {
                sb.Append("<color=#FFD848>[*] FICHA #02 — TONÃO / BEIRA-RIO</color>\nMetal gasto, recebido na Bocha. Marca a origem do relato de Tonão; não é chave mágica.\n\n");
            }
            else sb.Append("<color=#888278>[ ] Ficha #02 — Sociedade Beira-Rio</color>\n\n");
            if (GameState.Tem("c2_adilio_recado"))
                sb.Append("Via de Tonão: Adílio queria perguntar a Amaro quem escolheu as frases. Uma intenção, não um avistamento posterior.\n\n");
            if (GameState.Tem(Flags.Pista_NotaFiscal2004))
            {
                sb.Append("<color=#98E898>- Cupom do Mercadinho (2004)</color>\n");
                sb.Append("Emitido hoje cedo; traz anotação de Adílio sobre o Bar do Zé.\n");
            }

            textoDiarioConteudo.text = sb.ToString();
        }
        else
        {
            // Texto independente da arte original: continua nítido em qualquer zoom.
            textoDiarioConteudo.text =
                "<b>LEGENDA DO MAPA</b>\n" +
                "[o] Jogador (você)\n" +
                "[+] Personagem / NPC\n" +
                "[#] Casa ou prédio\n" +
                "[$] Comércio / serviço\n" +
                "[*] Local importante\n" +
                "=== Caminho principal\n" +
                "--- Caminho secundário\n" +
                "[=] Ponte\n" +
                "~~~ Água / rio\n" +
                "^^^ Mata / vegetação\n" +
                "||| Cerca ou muro\n\n" +
                "Roda/+/-: zoom.\nSegure o mouse e arraste: mover.";
        }
    }

    private Sprite CarregarOuGerarArteMapa()
    {
        string pathDisk = System.IO.Path.Combine(Application.dataPath, "Resources/Cap1/MapaSantaRitaArte_HD.png");
        if (System.IO.File.Exists(pathDisk))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(pathDisk);
            Texture2D texDisk = new Texture2D(2, 2, TextureFormat.RGBA32, true);
            if (ImageConversion.LoadImage(texDisk, bytes))
            {
                texDisk.filterMode = FilterMode.Trilinear;
                texDisk.anisoLevel = 8;
                texDisk.wrapMode = TextureWrapMode.Clamp;
                return Sprite.Create(texDisk, new Rect(0, 0, texDisk.width, texDisk.height), new Vector2(0.5f, 0.5f), 100f);
            }
        }

        Sprite sprArte = Resources.Load<Sprite>("Cap1/MapaSantaRitaArte_HD");
        if (sprArte != null) return sprArte;

        Texture2D texArte = Resources.Load<Texture2D>("Cap1/MapaSantaRitaArte_HD");
        if (texArte != null)
        {
            return Sprite.Create(texArte, new Rect(0, 0, texArte.width, texArte.height), new Vector2(0.5f, 0.5f), 100f);
        }

        // Fallback caso o Resource ainda não tenha sido importado
        const int w = 256;
        const int h = 192;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        Color mata = new Color32(28, 52, 28, 255);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, y, mata);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 32f);
    }

    private Sprite GerarIconeBlip(string tipo, Color corBase)
    {
        const int s = 14;
        var tex = new Texture2D(s, s, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        Color bg = new Color(0.06f, 0.06f, 0.08f, 0.95f);

        for (int y = 0; y < s; y++)
        {
            for (int x = 0; x < s; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(6.5f, 6.5f));
                if (d > 6.6f)
                    tex.SetPixel(x, y, Color.clear);
                else if (d > 5.1f)
                    tex.SetPixel(x, y, bg);
                else
                    tex.SetPixel(x, y, corBase);
            }
        }

        // Glifo central em pixel art para leitura imediata estilo GTA
        Color fg = (tipo == "boss1" || tipo == "clube") ? Color.white : new Color(0.08f, 0.08f, 0.10f, 1f);
        if (tipo == "orelhao")
        {
            // Gancho de telefone
            tex.SetPixel(5, 9, fg); tex.SetPixel(6, 9, fg); tex.SetPixel(5, 8, fg);
            tex.SetPixel(5, 7, fg); tex.SetPixel(5, 6, fg); tex.SetPixel(5, 5, fg);
            tex.SetPixel(6, 4, fg); tex.SetPixel(7, 4, fg); tex.SetPixel(8, 5, fg);
        }
        else if (tipo == "boss1")
        {
            // Bola 8 / Sinuca
            for (int y = 4; y <= 9; y++)
                for (int x = 4; x <= 9; x++)
                    if (Vector2.Distance(new Vector2(x, y), new Vector2(6.5f, 6.5f)) <= 2.4f)
                        tex.SetPixel(x, y, new Color(0.08f, 0.08f, 0.10f, 1f));
            tex.SetPixel(6, 6, Color.white);
            tex.SetPixel(7, 7, Color.white);
        }
        else if (tipo == "clube")
        {
            // Coroa / Estrela do Chefe Final
            for (int i = 4; i <= 9; i++) { tex.SetPixel(i, 6, fg); tex.SetPixel(6, i, fg); }
            tex.SetPixel(5, 5, fg); tex.SetPixel(8, 8, fg); tex.SetPixel(5, 8, fg); tex.SetPixel(8, 5, fg);
        }
        else if (tipo == "casa")
        {
            // Telhado de casa
            tex.SetPixel(6, 9, fg); tex.SetPixel(7, 9, fg);
            for (int x = 4; x <= 9; x++) tex.SetPixel(x, 8, fg);
            for (int y = 4; y <= 7; y++) { tex.SetPixel(5, y, fg); tex.SetPixel(8, y, fg); }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, s, s), new Vector2(0.5f, 0.5f), 32f);
    }

    private Sprite GerarIconePontoRota()
    {
        var tex = new Texture2D(6, 6, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        for (int y = 0; y < 6; y++)
            for (int x = 0; x < 6; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(2.5f, 2.5f));
                tex.SetPixel(x, y, d <= 2.6f ? (d > 1.7f ? Color.black : Color.white) : Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 6, 6), new Vector2(0.5f, 0.5f), 32f);
    }

    private Sprite GerarIconeJogador()
    {
        var tex = new Texture2D(14, 14, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        for (int y = 0; y < 14; y++)
            for (int x = 0; x < 14; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(6.5f, 6.5f));
                if (d > 6.2f) tex.SetPixel(x, y, Color.clear);
                else if (d > 4.6f) tex.SetPixel(x, y, Color.black);
                else if (d > 2.2f) tex.SetPixel(x, y, Color.white);
                else tex.SetPixel(x, y, new Color(0.05f, 0.25f, 0.35f, 1f));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 14, 14), new Vector2(0.5f, 0.5f), 32f);
    }

    private Sprite GerarIconeAlvo()
    {
        var tex = new Texture2D(20, 20, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        for (int y = 0; y < 20; y++)
            for (int x = 0; x < 20; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(9.5f, 9.5f));
                bool outerRing = d >= 7.0f && d <= 9.2f;
                bool border = (d >= 6.2f && d < 7.0f) || (d > 9.2f && d <= 9.9f);
                tex.SetPixel(x, y, outerRing ? Color.white : (border ? new Color(0, 0, 0, 0.85f) : Color.clear));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 20, 20), new Vector2(0.5f, 0.5f), 32f);
    }

    private TextMeshProUGUI CriarTexto(string nome, Transform pai, string conteudo, float tamanho, Color cor, TextAlignmentOptions alinhamento)
    {
        TextMeshProUGUI t = UIFactory.CreateLabel(pai, nome, tamanho);
        t.text = conteudo;
        t.color = cor;
        t.alignment = alinhamento;
        return t;
    }

    private void Posicionar(RectTransform r, Vector2 pos, Vector2 tam)
    {
        r.anchorMin = new Vector2(0.5f, 0.5f);
        r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = pos;
        r.sizeDelta = tam;
    }
}

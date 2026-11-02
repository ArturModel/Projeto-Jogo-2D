using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Sinuquinha no Bar do Zé: quinze bolas numeradas, grupos e preta por último.
/// A adjudicação é isolada em SinuquinhaRules; física/renderização não concedem pontos.
/// </summary>
public class SinucaMinigameUI : MonoBehaviour
{
    public static SinucaMinigameUI Instance
    {
        get
        {
            EnsureInstance();
            return instance;
        }
    }

    private static SinucaMinigameUI instance;
    public static bool IsShowing => instance != null && instance.raiz != null && instance.raiz.activeSelf;

    public const float TableWidth = 420f;
    public const float TableHeight = 210f;
    public const float BallRadius = 5.4f;
    public const float PocketRadius = 5.9f;
    public const float FixedStep = 1f / 120f;
    private const float CushionRestitution = 0.80f;
    private const float BallRestitution = 0.94f;
    private const float BallFriction = 88f;
    private const float StopSpeedSq = 3.2f;
    private const float MaxShotPower = 485f;
    private const float MinShotPower = 95f;

    private enum EstadoPartida
    {
        Inativo,
        TutorialRegras,
        PosicionandoBranca,
        MirandoJogador,
        CarregandoForca,
        TurnoZePensando,
        BolasEmMovimento,
        ResolvendoTacada,
        FimDeMao,
        FimDeSerieDerrota,
        FimDeSerieVitoria
    }

    private enum DispositivoCarga
    {
        Nenhum = 0,
        MouseEsquerdo = 1,
        TeclaEspaco = 2
    }

    private class BolaMesa
    {
        public int numero;
        public Vector2 pos;
        public Vector2 vel;
        public bool ativa;
        public bool caiuNestaTacada;
        public bool caindoNaCacapa;
        public float progressoQueda;
        public Vector2 cacapaAlvo;
        public Quaternion rotacao = Quaternion.identity;
        public float spinLateral;
        public float spinLongitudinal; // > 0 topspin (Avançar), < 0 backspin (Recuar)
        public RectTransform rect;
        public Image img;
        public TextMeshProUGUI label;
        public GameObject marcaVisual;
    }

    private struct BicoCacapa
    {
        public Vector2 pos;
        public float raio;
    }

    private GameObject raiz;
    private RectTransform mesaRect;
    private SinucaTableView tableView3D;
    private RectTransform linhaMiraRect;
    private RectTransform tacoRect;

    private TextMeshProUGUI txtPlacarSerie;
    private TextMeshProUGUI txtPlacarMao;
    private TextMeshProUGUI txtTurnoStatus;
    private TextMeshProUGUI txtRegraBolaMarcada;
    private TextMeshProUGUI txtPercepcaoSubtitulo;
    private TextMeshProUGUI txtControlesRodape;
    private Image barraForcaFill;
    private Image overlayPercepcao;

    // Botões de Câmera, Prática e Efeito Opcional na Branca
    private Button btnVistaCamera;
    private TextMeshProUGUI txtBtnVistaCamera;
    private Button btnToggleEfeito;
    private TextMeshProUGUI txtBtnToggleEfeito;
    private GameObject painelEfeitoRecolhivel;
    private Button btnEfeitoNormal;
    private Button btnEfeitoAvancar;
    private Button btnEfeitoRecuar;
    private TextMeshProUGUI txtTendenciaEfeito;
    private Button btnAcaoPraticaOuDuelo;
    private TextMeshProUGUI txtBtnAcaoPraticaOuDuelo;

    private GameObject painelModal;
    private TextMeshProUGUI txtModalTitulo;
    private TextMeshProUGUI txtModalCorpo;
    private Button btnModalPrimario;
    private TextMeshProUGUI txtBtnPrimario;
    private Button btnModalSecundario;
    private TextMeshProUGUI txtBtnSecundario;

    private readonly List<BolaMesa> bolas = new List<BolaMesa>();
    private readonly List<SinucaVisualBall> bufferVisuais = new List<SinucaVisualBall>(16);
    private readonly Vector2[] cacapas = new Vector2[6];
    private readonly List<BicoCacapa> bicosCacapas = new List<BicoCacapa>(12);

    private EstadoPartida estado = EstadoPartida.Inativo;
    public static int MaoEmCurso => IsShowing ? (instance.modoPratica ? 1 : instance.maoAtual) : 0;

    private bool modoPratica = false;
    private int maoAtual = 1;
    private int maosVencidasMarcus = 0;
    private int maosVencidasZe = 0;

    private int pontosMarcus = 0;
    private int pontosZe = 0;
    private bool turnoDeMarcus = true;
    private bool brancaNaMao = false;

    private int numeroBolaMarcada = -1;
    private readonly Vector2[] inicioTacadaPos=new Vector2[16];
    private readonly Quaternion[] inicioTacadaRot=new Quaternion[16];
    private readonly bool[] inicioTacadaAtiva=new bool[16];
    private readonly Image[] encaçapadasIcones=new Image[16];
    private TextMeshProUGUI txtEncaçapadasMarcus,txtEncaçapadasZe;
    private bool demonstracaoMao2Feita = false;
    private int contadorTurnosMao = 0;

    private float anguloMiraRad = 0f;
    private float forcaNormalizada = 0f;
    private bool carregandoDirectionUp = true;
    private DispositivoCarga dispositivoCargaAtivo = DispositivoCarga.Nenhum;
    private int frameConfirmacaoBranca = -1;
    private SinucaCueSpin efeitoEscolhido = SinucaCueSpin.Normal;
    private bool painelEfeitoAberto = false;
    private float acumuladorFisica = 0f;
    private int ultimaLarguraTela;
    private int ultimaAlturaTela;

    private bool houvePrimeiroContatoBolaObjeto = false;
    private bool houveToqueTabelaOuCacapa = false;
    private bool brancaCaiuNestaTacada = false;

    private readonly List<int> bolasObjetoCaidasNaTacada = new List<int>();

    // Terror psicológico contextual do Bar do Zé e síntese de áudio por evento
    private float multiplicadorLuzAmbiente = 1f;
    private float timerOscilacaoLuz = 0f;
    private bool zePassandoGizNoTurno = true;
    private bool miraAmeacaBolaMarcada = false;
    private float ultimoSomGiz = -10f;
    private static AudioClip clipTacada;
    private static AudioClip clipColisaoBola;
    private static AudioClip clipTabela;
    private static AudioClip clipCacapa;
    private static AudioClip clipGiz;

    // Métricas diagnósticas para QA (S01..S27)
    public static double UltimaDecisaoZeMs { get; private set; }
    public static int UltimosCandidatosZe { get; private set; }
    public static int EventosAudioDisparados { get; private set; }
    public bool EmModoPratica => modoPratica;
    public SinucaCameraView CameraAtual => tableView3D != null ? tableView3D.CameraModo : SinucaCameraView.Mesa3D;
    public SinucaCueSpin EfeitoAtual => efeitoEscolhido;

    private Vector2 contatoTaco;
    private SinucaSpinSelector spinSelector;
    private GameObject painelAlvo;
    private TextMeshProUGUI txtAlvo;
    private Button[] escolhasAlvo=new Button[16];
    private SinuquinhaRules regrasBrasileiras;
    private int primeiroContatoNumero = -1, bolaCantada, cantadaManual;
    private static readonly Vector2[] marcasBrasileiras=CriarTriangulo();
    private static Vector2[] CriarTriangulo(){var p=new Vector2[16];int[] order={1,9,2,3,8,10,11,4,12,5,6,13,7,14,15};int i=0;for(int row=0;row<5;row++)for(int col=0;col<=row;col++)p[order[i++]]=new Vector2(65+row*BallRadius*1.75f,(col-row*.5f)*BallRadius*2.02f);return p;}
    private bool gruposPorNumero;

    private Action onVictoryCallback;
    private Action onDefeatCallback;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetStatic()
    {
        instance = null;
        UltimaDecisaoZeMs = 0;
        UltimosCandidatosZe = 0;
        EventosAudioDisparados = 0;
    }

    public static void IniciarConfronto(Action aoVencerSerie)
    {
        EnsureInstance();
        instance.ComecarNovaSerie(aoVencerSerie);
    }

    public void IniciarDuelo(Action aoVencer, Action aoPerder = null)
    {
        if (raiz == null)
        {
            InicializarCacapas();
            ConstruirUI();
        }
        onDefeatCallback = aoPerder;
        ComecarNovaSerie(aoVencer);
    }

    public void PrepararSala(){var view=GetComponentInChildren<SinucaTableView>(true);if(view!=null)view.Preaquecer();}
    public void IniciarPraticaOpcional()
    {
        if (raiz == null)
        {
            InicializarCacapas();
            ConstruirUI();
        }
        ComecarModoPratica();
    }

    public void Fechar()
    {
        StopAllCoroutines();
        CancelarCargaAtiva();
        estado = EstadoPartida.Inativo;
        modoPratica = false;
        if (Application.isPlaying) SantaRitaBarAudio.DefinirContexto(false, 0);
        if (raiz != null) raiz.SetActive(false);
    }

    public void CancelarCargaAoSuspender()
    {
        if (estado == EstadoPartida.CarregandoForca)
        {
            CancelarCargaAtiva();
            if (txtTurnoStatus != null)
                txtTurnoStatus.text = "Tacada cancelada. Ajuste a mira e segure para carregar.";
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && IsShowing)
        {
            CancelarCargaAoSuspender();
        }
    }

    private static void EnsureInstance()
    {
        if (instance != null) return;
        instance = FindAnyObjectByType<SinucaMinigameUI>();
        if (instance != null) return;

        GameObject go = new GameObject("SinucaMinigameUI");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<SinucaMinigameUI>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        InicializarCacapas();
        ConstruirUI();
        raiz.SetActive(false);
    }

    private void InicializarCacapas()
    {
        float hx = TableWidth * 0.5f - 4f;  // 206
        float hy = TableHeight * 0.5f - 4f; // 101
        cacapas[0] = new Vector2(-hx, hy);
        cacapas[1] = new Vector2(0f, hy + 2f);
        cacapas[2] = new Vector2(hx, hy);
        cacapas[3] = new Vector2(-hx, -hy);
        cacapas[4] = new Vector2(0f, -hy - 2f);
        cacapas[5] = new Vector2(hx, -hy);

        bicosCacapas.Clear();
        float w = TableWidth * 0.5f;  // 210
        float h = TableHeight * 0.5f; // 105
        float jawR = 1.8f;
        float cOffset = 10.5f;
        float sOffset = 9.0f;

        // Bicos das 4 caçapas de canto (2 por canto)
        foreach (float sx in new[] { -1f, 1f })
        {
            foreach (float sy in new[] { -1f, 1f })
            {
                bicosCacapas.Add(new BicoCacapa { pos = new Vector2(sx * (w - cOffset), sy * h), raio = jawR });
                bicosCacapas.Add(new BicoCacapa { pos = new Vector2(sx * w, sy * (h - cOffset)), raio = jawR });
            }
        }
        // Bicos das 2 caçapas laterais (meio norte e meio sul)
        foreach (float sy in new[] { -1f, 1f })
        {
            bicosCacapas.Add(new BicoCacapa { pos = new Vector2(-sOffset, sy * h), raio = jawR });
            bicosCacapas.Add(new BicoCacapa { pos = new Vector2(sOffset, sy * h), raio = jawR });
        }
    }

    private void ConstruirUI()
    {
        Canvas canvas = UIFactory.CreateCanvas(transform, "SinucaCanvas", 6500);
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler != null) scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        TMP_FontAsset font = UIFactory.Font;

        raiz = new GameObject("RaizSinuca", typeof(RectTransform));
        raiz.transform.SetParent(canvas.transform, false);
        RectTransform raizRect = raiz.GetComponent<RectTransform>();
        UIFactory.Stretch(raizRect);

        // A planta projetada ocupa toda a tela; impedir duplicação do mundo 2D por transparência.
        GameObject bgObj = new GameObject("FundoEscuroBar", typeof(RectTransform), typeof(Image));
        bgObj.transform.SetParent(raiz.transform, false);
        UIFactory.Stretch(bgObj.GetComponent<RectTransform>());
        bgObj.GetComponent<Image>().color = new Color32(12, 10, 8, 255);

        // Overlay de percepção sutil (subtítulo atmosférico + leve vinheta da 3ª mão)
        GameObject percObj = new GameObject("OverlayPercepcao", typeof(RectTransform), typeof(Image));
        percObj.transform.SetParent(raiz.transform, false);
        UIFactory.Stretch(percObj.GetComponent<RectTransform>());
        overlayPercepcao = percObj.GetComponent<Image>();
        overlayPercepcao.color = new Color(0.18f, 0.04f, 0.04f, 0f);
        overlayPercepcao.raycastTarget = false;

        // Cabeçalho superior (Placar da Série, Placar da Mão, Regra da Bola Marcada, Status do Turno)
        GameObject header = new GameObject("HeaderHUD", typeof(RectTransform), typeof(Image));
        header.transform.SetParent(raiz.transform, false);
        RectTransform headerRect = header.GetComponent<RectTransform>();
        Posicionar(headerRect, new Vector2(0f, 156f), new Vector2(624f, 42f));
        header.GetComponent<Image>().color = new Color32(20, 17, 14, 240);
        Outline headerOut = header.AddComponent<Outline>();
        headerOut.effectColor = new Color32(98, 72, 42, 175);

        txtPlacarSerie = CriarTexto("TxtPlacarSerie", header.transform,
            "SÉRIE MELHOR DE 3 — MARCUS [0] x [0] ZÉ", 10.6f, new Color32(238, 214, 158, 255), TextAlignmentOptions.TopLeft);
        Posicionar(txtPlacarSerie.rectTransform, new Vector2(-122f, 9f), new Vector2(356f, 18f));

        txtPlacarMao = CriarTexto("TxtPlacarMao", header.transform,
            "MÃO 1: MARCUS 0/4  |  ZÉ 0/4", 10.2f, new Color32(215, 224, 215, 255), TextAlignmentOptions.TopLeft);
        Posicionar(txtPlacarMao.rectTransform, new Vector2(-122f, -9f), new Vector2(356f, 18f));

        txtRegraBolaMarcada = CriarTexto("TxtRegraBolaMarcada", header.transform,
            "SINUQUINHA · 15 BOLAS · PRETA 8 POR ÚLTIMO", 9.4f, new Color32(255, 192, 118, 255), TextAlignmentOptions.Left);
        Posicionar(txtRegraBolaMarcada.rectTransform, new Vector2(-122f, -11f), new Vector2(356f, 16f));

        txtTurnoStatus = CriarTexto("TxtTurnoStatus", header.transform,
            "Sua vez, Marcus.", 9.6f, new Color32(168, 232, 184, 255), TextAlignmentOptions.Left);
        Posicionar(txtTurnoStatus.rectTransform, new Vector2(-122f, -25f), new Vector2(356f, 16f));

        // Botões superiores à direita: VISTA (3D / Superior), PRÁTICA / DUELO e REGRAS
        btnVistaCamera = CriarBotaoHUD(header.transform, "BtnVistaCamera", "VISTA: MESA 3D (V)",
            new Vector2(126f, 10f), new Vector2(114f, 18f), new Color32(38, 56, 46, 255), out txtBtnVistaCamera);
        btnVistaCamera.onClick.AddListener(AlternarVistaCamera);

        btnAcaoPraticaOuDuelo = CriarBotaoHUD(header.transform, "BtnAcaoPraticaOuDuelo", "PRÁTICA LIVRE",
            new Vector2(126f, -10f), new Vector2(114f, 18f), new Color32(54, 42, 28, 255), out txtBtnAcaoPraticaOuDuelo);
        btnAcaoPraticaOuDuelo.onClick.AddListener(AoClicarBotaoPraticaOuDuelo);

        Button btnRegras = CriarBotaoHUD(header.transform, "BtnVerRegras", "REGRAS",
            new Vector2(244f, 10f), new Vector2(104f, 18f), new Color32(52, 38, 24, 255), out _);
        btnRegras.onClick.AddListener(AbrirModalRegrasDuranteJogo);

        var targetButton=CriarBotaoHUD(header.transform,"EscolherPrimeiroToque","ALVO: AUTO",new Vector2(244,-10),new Vector2(104,18),new Color32(47,55,44,255),out txtAlvo);
        targetButton.onClick.AddListener(AbrirEscolhaAlvo);

        // Viewport 3D da Mesa de Sinuca (SinucaTableView : MaskableGraphic)
        GameObject viewportObj = new GameObject("MesaSinuca3DViewport", typeof(RectTransform), typeof(CanvasRenderer), typeof(SinucaTableView));
        viewportObj.transform.SetParent(raiz.transform, false);
        mesaRect = viewportObj.GetComponent<RectTransform>();
        Posicionar(mesaRect, new Vector2(0f, -8f), new Vector2(640f, 280f));

        viewportObj.transform.SetSiblingIndex(2);
        txtRegraBolaMarcada.transform.SetParent(raiz.transform,false);Posicionar(txtRegraBolaMarcada.rectTransform,new Vector2(0,124),new Vector2(610,15));
        txtTurnoStatus.transform.SetParent(raiz.transform,false);Posicionar(txtTurnoStatus.rectTransform,new Vector2(0,109),new Vector2(610,15));
        tableView3D = viewportObj.GetComponent<SinucaTableView>();
        tableView3D.color = Color.white;
        tableView3D.InicializarOverlays(font);
        tableView3D.AoPressionarMesa = OnMesaPointerDown;
        tableView3D.AoMoverMesa = OnMesaPointerMove;
        tableView3D.AoAjustarMira=delta=>{if(estado==EstadoPartida.MirandoJogador&&!MenuPausa.Aberto){anguloMiraRad+=delta;AtualizarVisualTacoEMira();}};
        tableView3D.AoSoltarMesa = OnMesaPointerUp;
        tableView3D.AoScrollZoom = OnMesaScrollZoom;

        // Container oculto de compatibilidade legado (mantém rects/labels/bolas caso inspecionados por reflexão)
        GameObject containerLegado = new GameObject("ContainerLegadoCompat", typeof(RectTransform));
        containerLegado.transform.SetParent(mesaRect, false);
        containerLegado.SetActive(false);

        GameObject miraObj = new GameObject("LinhaMira", typeof(RectTransform), typeof(Image));
        miraObj.transform.SetParent(containerLegado.transform, false);
        linhaMiraRect = miraObj.GetComponent<RectTransform>();

        GameObject tacoObj = new GameObject("TacoSinuca", typeof(RectTransform), typeof(Image));
        tacoObj.transform.SetParent(containerLegado.transform, false);
        tacoRect = tacoObj.GetComponent<RectTransform>();

        CriarBolasNaMesa(containerLegado.transform, font);

        // Painel recolhível de Efeito Opcional na Branca (Normal / Avançar / Recuar)
        // Efeito somente no ponto de contato da branca.

        var spinObject=new GameObject("PontoDeContatoBranca",typeof(RectTransform),typeof(CanvasRenderer),typeof(SinucaSpinSelector));
        spinObject.transform.SetParent(raiz.transform,false);
        Posicionar(spinObject.GetComponent<RectTransform>(),new Vector2(285,-112),new Vector2(42,42));
        spinSelector=spinObject.GetComponent<SinucaSpinSelector>();
        spinSelector.CanSelect=()=>estado==EstadoPartida.MirandoJogador&&!MenuPausa.Aberto;
        spinSelector.Changed=p=>{if(estado!=EstadoPartida.MirandoJogador)return;contatoTaco=p;
            efeitoEscolhido=p.y>.15f?SinucaCueSpin.Avancar:p.y<-.15f?SinucaCueSpin.Recuar:SinucaCueSpin.Normal;
            txtTendenciaEfeito.text="Contato: "+(p.x<-.15f?"esquerda / ":p.x>.15f?"direita / ":"")+(p.y<-.15f?"recuo":p.y>.15f?"avanço":"centro");};

        var contactLabel=CriarTexto("LegendaContato",raiz.transform,"TOQUE NA BRANCA",7,new Color32(224,211,165,255),TextAlignmentOptions.Center);
        Posicionar(contactLabel.rectTransform,new Vector2(285,-85),new Vector2(75,10));
        var cameraLabel=CriarTexto("LegendaCamera",raiz.transform,"A / D · CONTORNAR    ← / → · MIRAR",8,new Color32(224,211,165,255),TextAlignmentOptions.Left);
        Posicionar(cameraLabel.rectTransform,new Vector2(-164,-116),new Vector2(280,16));
        txtEncaçapadasMarcus=CriarTexto("EncaçapadasMarcus",raiz.transform,"MARCUS · ENCAÇAPADAS",8.5f,new Color32(234,224,195,255),TextAlignmentOptions.Left);
        txtEncaçapadasZe=CriarTexto("EncaçapadasZe",raiz.transform,"ZÉ · ENCAÇAPADAS",8.5f,new Color32(234,224,195,255),TextAlignmentOptions.Left);
        Posicionar(txtEncaçapadasMarcus.rectTransform,new Vector2(-225,91),new Vector2(160,12));
        Posicionar(txtEncaçapadasZe.rectTransform,new Vector2(115,91),new Vector2(160,12));
        for(int n=1;n<16;n++)if(n!=8){var chip=new GameObject("Encaçapada_"+n,typeof(RectTransform),typeof(Image));chip.transform.SetParent(raiz.transform,false);var im=chip.GetComponent<Image>();im.sprite=CriarSpriteCirculo(40,SinucaTableView.CorDaBola(n),new Color(.84f,.78f,.58f));im.raycastTarget=false;encaçapadasIcones[n]=im;var number=CriarTexto("Numero",chip.transform,n.ToString(),8.5f,(n==1||n==5||n==9||n==13)?new Color(.06f,.06f,.04f):Color.white,TextAlignmentOptions.Center);UIFactory.Stretch(number.rectTransform);chip.SetActive(false);}
        var guideLabel=CriarTexto("LegendaDaMira",raiz.transform,"CLARA: BRANCA   ·   DOURADA: COLORIDA",7.5f,new Color32(239,214,146,255),TextAlignmentOptions.Left);
        Posicionar(guideLabel.rectTransform,new Vector2(-164,-103),new Vector2(280,12));
        ConstruirEscolhaAlvo();
        // Subtítulo de percepção psicológica (discreto acima do rodapé)
        txtPercepcaoSubtitulo = CriarTexto("TxtPercepcaoSubtitulo", raiz.transform,
            "", 9.8f, new Color32(215, 198, 172, 225), TextAlignmentOptions.Center);
        Posicionar(txtPercepcaoSubtitulo.rectTransform, new Vector2(-30f, -89f), new Vector2(510f, 14f));

        // Rodapé: Barra de Força + Resumo dos Controles Simples
        GameObject footer = new GameObject("FooterHUD", typeof(RectTransform), typeof(Image));
        footer.transform.SetParent(raiz.transform, false);
        RectTransform footerRect = footer.GetComponent<RectTransform>();
        Posicionar(footerRect, new Vector2(0f, -154f), new Vector2(624f, 44f));
        footer.GetComponent<Image>().color = new Color32(20, 17, 14, 238);

        TextMeshProUGUI lblForca = CriarTexto("LblForca", footer.transform,
            "FORÇA DA TACADA:", 9.8f, new Color32(228, 214, 184, 255), TextAlignmentOptions.Left);
        Posicionar(lblForca.rectTransform, new Vector2(-242f, 9f), new Vector2(124f, 16f));

        GameObject barraBg = new GameObject("BarraForcaBg", typeof(RectTransform), typeof(Image));
        barraBg.transform.SetParent(footer.transform, false);
        Posicionar(barraBg.GetComponent<RectTransform>(), new Vector2(-88f, 9f), new Vector2(180f, 12f));
        barraBg.GetComponent<Image>().color = new Color32(42, 38, 34, 255);

        GameObject barraFill = new GameObject("BarraForcaFill", typeof(RectTransform), typeof(Image));
        barraFill.transform.SetParent(barraBg.transform, false);
        RectTransform fillRect = barraFill.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(0f, 1f);
        fillRect.offsetMin = new Vector2(1f, 1f);
        fillRect.offsetMax = new Vector2(-1f, -1f);
        barraForcaFill = barraFill.GetComponent<Image>();
        barraForcaFill.color = new Color32(235, 168, 52, 255);
        barraForcaFill.fillAmount = 0f;

        txtTendenciaEfeito = CriarTexto("TxtTendenciaEfeito", footer.transform,
            "Tacada Normal: a branca segue o rebote natural após o contato.", 9.4f, new Color32(186, 214, 198, 255), TextAlignmentOptions.Left);
        Posicionar(txtTendenciaEfeito.rectTransform, new Vector2(158f, 9f), new Vector2(292f, 16f));

        txtControlesRodape = CriarTexto("TxtControlesRodape", footer.transform,
            "Aponte: mirar  |  Segure e solte: tacar  |  Roda: zoom  |  V: vistas  |  A/D: contornar  |  ←/→: mira  |  Dir./C: cancelar  |  ESC: pausa",
            8.9f, new Color32(178, 174, 164, 255), TextAlignmentOptions.Center);
        Posicionar(txtControlesRodape.rectTransform, new Vector2(0f, -10f), new Vector2(612f, 16f));

        ConstruirModal(raiz.transform);
    }


    private void ConstruirEscolhaAlvo()
    {
        painelAlvo=new GameObject("PrimeiroToque",typeof(RectTransform),typeof(Image));painelAlvo.transform.SetParent(raiz.transform,false);
        Posicionar(painelAlvo.GetComponent<RectTransform>(),new Vector2(183,77),new Vector2(238,92));painelAlvo.GetComponent<Image>().color=new Color32(28,31,25,250);
        var title=CriarTexto("ExplicacaoAlvo",painelAlvo.transform,"QUAL BOLA VOCÊ PRETENDE TOCAR PRIMEIRO?\nAUTO acompanha sua mira. Escolha uma bola para anunciar uma jogada de tabela.",8,new Color32(232,219,183,255),TextAlignmentOptions.Center);Posicionar(title.rectTransform,new Vector2(0,21),new Vector2(222,39));
        for(int n=0;n<=15;n++)
        {
            int target=n;var button=CriarBotaoHUD(painelAlvo.transform,"Alvo"+n,n==0?"AUTO":n.ToString(),new Vector2(-98+(n%8)*28,n<8?-8:-36),new Vector2(n==0?33:24,24),new Color32(43,47,37,255),out var label);
            if(n>0){button.GetComponent<Image>().sprite=CriarSpriteCirculo(40,SinucaTableView.CorDaBola(n),new Color(.72f,.65f,.44f));label.color=(n==8||n==2||n==4||n==7||n==10||n==12||n==15)?Color.white:new Color(.06f,.06f,.045f);}
            escolhasAlvo[n]=button;button.onClick.AddListener(()=>{cantadaManual=target;txtAlvo.text=target==0?"ALVO: AUTO":"PRIMEIRO TOQUE: "+target;painelAlvo.SetActive(false);txtTurnoStatus.text=target==0?"Mire a bola pretendida; o alvo é identificado pela linha.":"Primeiro toque anunciado: bola "+target+". Acerte essa bola antes das demais.";});
        }
        painelAlvo.SetActive(false);
    }
    private void AbrirEscolhaAlvo()
    {
        if(estado!=EstadoPartida.MirandoJogador)return;CancelarCargaAoSuspender();
        for(int n=1;n<=15;n++)escolhasAlvo[n].interactable=modoPratica?bolas[n].ativa:regrasBrasileiras!=null&&regrasBrasileiras.CanNominate(n);
        painelAlvo.SetActive(!painelAlvo.activeSelf);
    }

    private Button CriarBotaoHUD(Transform pai, string nome, string rotulo, Vector2 pos, Vector2 tam, Color32 corFundo, out TextMeshProUGUI txtOut)
    {
        GameObject obj = new GameObject(nome, typeof(RectTransform), typeof(Image), typeof(Button));
        obj.transform.SetParent(pai, false);
        Posicionar(obj.GetComponent<RectTransform>(), pos, tam);
        obj.GetComponent<Image>().color = corFundo;
        Outline o = obj.AddComponent<Outline>();
        o.effectColor = new Color32(146, 114, 66, 185);

        txtOut = CriarTexto("Txt_" + nome, obj.transform, rotulo, 9.2f, new Color32(242, 224, 182, 255), TextAlignmentOptions.Center);
        UIFactory.Stretch(txtOut.rectTransform);
        return obj.GetComponent<Button>();
    }

    private void ConstruirPainelEfeitoOpcional()
    {
        painelEfeitoRecolhivel = new GameObject("PainelEfeitoOpcional", typeof(RectTransform), typeof(Image));
        painelEfeitoRecolhivel.transform.SetParent(raiz.transform, false);
        RectTransform rt = painelEfeitoRecolhivel.GetComponent<RectTransform>();
        Posicionar(rt, new Vector2(188f, 88f), new Vector2(236f, 32f));
        painelEfeitoRecolhivel.GetComponent<Image>().color = new Color32(22, 24, 28, 244);
        Outline o = painelEfeitoRecolhivel.AddComponent<Outline>();
        o.effectColor = new Color32(118, 146, 168, 190);

        btnEfeitoNormal = CriarBotaoHUD(painelEfeitoRecolhivel.transform, "BtnEfeitoNormal", "1 · Normal",
            new Vector2(-74f, 0f), new Vector2(70f, 22f), new Color32(48, 72, 54, 255), out _);
        btnEfeitoNormal.onClick.AddListener(() => SelecionarEfeitoBranca(SinucaCueSpin.Normal));

        btnEfeitoAvancar = CriarBotaoHUD(painelEfeitoRecolhivel.transform, "BtnEfeitoAvancar", "2 · Avançar",
            new Vector2(0f, 0f), new Vector2(72f, 22f), new Color32(36, 44, 52, 255), out _);
        btnEfeitoAvancar.onClick.AddListener(() => SelecionarEfeitoBranca(SinucaCueSpin.Avancar));

        btnEfeitoRecuar = CriarBotaoHUD(painelEfeitoRecolhivel.transform, "BtnEfeitoRecuar", "3 · Recuar",
            new Vector2(74f, 0f), new Vector2(70f, 22f), new Color32(36, 44, 52, 255), out _);
        btnEfeitoRecuar.onClick.AddListener(() => SelecionarEfeitoBranca(SinucaCueSpin.Recuar));

        painelEfeitoRecolhivel.SetActive(false);
    }

    private void CriarBolasNaMesa(Transform pai, TMP_FontAsset font)
    {
        bolas.Clear();
        Sprite bolaSprite = CriarSpriteCirculo(32, Color.white, new Color32(20, 20, 20, 180));

        for (int i = 0; i <= 15; i++)
        {
            GameObject bObj = new GameObject($"Bola_{i}", typeof(RectTransform), typeof(Image));
            bObj.transform.SetParent(pai, false);
            RectTransform bRect = bObj.GetComponent<RectTransform>();
            Posicionar(bRect, Vector2.zero, new Vector2(BallRadius * 2f, BallRadius * 2f));

            Image bImg = bObj.GetComponent<Image>();
            bImg.sprite = bolaSprite;
            bImg.color = SinucaTableView.CorDaBola(i);
            bImg.raycastTarget = false;

            TextMeshProUGUI lbl = null;
            if (i > 0)
            {
                lbl = CriarTexto($"Num_{i}", bObj.transform, i.ToString(), 9.5f, Color.white, TextAlignmentOptions.Center);
                UIFactory.Stretch(lbl.rectTransform);
                lbl.fontStyle = FontStyles.Bold;
                lbl.raycastTarget = false;
            }

            GameObject marcaObj = new GameObject("MarcaGizX", typeof(RectTransform), typeof(Image));
            marcaObj.transform.SetParent(bObj.transform, false);
            RectTransform mRect = marcaObj.GetComponent<RectTransform>();
            Posicionar(mRect, Vector2.zero, new Vector2(BallRadius * 2.7f, BallRadius * 2.7f));
            Image mImg = marcaObj.GetComponent<Image>();
            mImg.sprite = CriarSpriteCirculo(36, new Color(0f, 0f, 0f, 0f), new Color32(255, 70, 70, 255));
            mImg.raycastTarget = false;

            TextMeshProUGUI xTxt = CriarTexto("TxtX", marcaObj.transform, "X", 11f, new Color32(255, 220, 120, 255), TextAlignmentOptions.Center);
            Posicionar(xTxt.rectTransform, new Vector2(0f, 10f), new Vector2(24f, 14f));
            xTxt.fontStyle = FontStyles.Bold;
            xTxt.raycastTarget = false;
            marcaObj.SetActive(false);

            bolas.Add(new BolaMesa
            {
                numero = i,
                pos = Vector2.zero,
                vel = Vector2.zero,
                ativa = true,
                caiuNestaTacada = false,
                caindoNaCacapa = false,
                progressoQueda = 0f,
                rotacao = Quaternion.identity,
                spinLongitudinal = 0f,
                rect = bRect,
                img = bImg,
                label = lbl,
                marcaVisual = marcaObj
            });
        }
    }

    private void ConstruirModal(Transform pai)
    {
        painelModal = new GameObject("PainelModalSinuca", typeof(RectTransform), typeof(Image));
        painelModal.transform.SetParent(pai, false);
        RectTransform modalRect = painelModal.GetComponent<RectTransform>();
        Posicionar(modalRect, Vector2.zero, new Vector2(492f, 254f));
        painelModal.GetComponent<Image>().color = new Color32(16, 15, 13, 248);
        Outline o = painelModal.AddComponent<Outline>();
        o.effectColor = new Color32(185, 142, 72, 230);
        o.effectDistance = new Vector2(1.5f, -1.5f);

        txtModalTitulo = CriarTexto("TxtModalTitulo", painelModal.transform,
            "SINUCA — BAR DO ZÉ", 14.5f, new Color32(242, 204, 122, 255), TextAlignmentOptions.Center);
        Posicionar(txtModalTitulo.rectTransform, new Vector2(0f, 98f), new Vector2(452f, 26f));
        txtModalTitulo.fontStyle = FontStyles.Bold;

        txtModalCorpo = CriarTexto("TxtModalCorpo", painelModal.transform,
            "", 10.6f, new Color32(226, 222, 212, 255), TextAlignmentOptions.TopLeft);
        Posicionar(txtModalCorpo.rectTransform, new Vector2(0f, 6f), new Vector2(446f, 148f));
        txtModalCorpo.textWrappingMode = TextWrappingModes.Normal;

        GameObject btn1Obj = new GameObject("BtnModalPrimario", typeof(RectTransform), typeof(Image), typeof(Button));
        btn1Obj.transform.SetParent(painelModal.transform, false);
        Posicionar(btn1Obj.GetComponent<RectTransform>(), new Vector2(-95f, -96f), new Vector2(178f, 32f));
        btn1Obj.GetComponent<Image>().color = new Color32(42, 92, 58, 255);
        btnModalPrimario = btn1Obj.GetComponent<Button>();
        txtBtnPrimario = CriarTexto("TxtBtn1", btn1Obj.transform, "INICIAR PARTIDA", 11f, Color.white, TextAlignmentOptions.Center);
        UIFactory.Stretch(txtBtnPrimario.rectTransform);
        txtBtnPrimario.fontStyle = FontStyles.Bold;

        GameObject btn2Obj = new GameObject("BtnModalSecundario", typeof(RectTransform), typeof(Image), typeof(Button));
        btn2Obj.transform.SetParent(painelModal.transform, false);
        Posicionar(btn2Obj.GetComponent<RectTransform>(), new Vector2(95f, -96f), new Vector2(178f, 32f));
        btn2Obj.GetComponent<Image>().color = new Color32(78, 52, 34, 255);
        btnModalSecundario = btn2Obj.GetComponent<Button>();
        txtBtnSecundario = CriarTexto("TxtBtn2", btn2Obj.transform, "PRÁTICA CURTA", 11f, Color.white, TextAlignmentOptions.Center);
        UIFactory.Stretch(txtBtnSecundario.rectTransform);
        txtBtnSecundario.fontStyle = FontStyles.Bold;

        painelModal.SetActive(false);
    }

    private void ComecarNovaSerie(Action aoVencerSerie)
    {
        StopAllCoroutines();
        CancelarCargaAtiva();
        onVictoryCallback = aoVencerSerie;
        raiz.SetActive(true);

        modoPratica = false;
        maoAtual = 1;
        maosVencidasMarcus = 0;
        maosVencidasZe = 0;
        demonstracaoMao2Feita = false;
        SelecionarEfeitoBranca(SinucaCueSpin.Normal);
        painelEfeitoAberto = false;
        if (painelEfeitoRecolhivel != null) painelEfeitoRecolhivel.SetActive(false);

        ultimaLarguraTela = Screen.width;
        ultimaAlturaTela = Screen.height;

        ConfigurarMao(1);
        MostrarModal("ZÉ · COMO VAMOS JOGAR?","Quinze bolas no triângulo. Você prefere identificar os grupos pelas cores ou pelos números? A preta 8 é sempre a última. A branca já está posicionada.","LISAS / LISTRADAS",()=>{gruposPorNumero=false;painelModal.SetActive(false);IniciarTurnoAtual();},"1–7 / 9–15",()=>{gruposPorNumero=true;painelModal.SetActive(false);IniciarTurnoAtual();});
    }

    private void ComecarModoPratica()
    {
        StopAllCoroutines();
        CancelarCargaAtiva();
        raiz.SetActive(true);
        if (painelModal != null) painelModal.SetActive(false);

        modoPratica = true;
        maoAtual = 1;
        pontosMarcus = 0;
        pontosZe = 0;
        turnoDeMarcus = true;
        brancaNaMao = false;
        numeroBolaMarcada = -1;
        contadorTurnosMao = 0;
        SelecionarEfeitoBranca(SinucaCueSpin.Normal);

        if (Application.isPlaying) SantaRitaBarAudio.DefinirContexto(true, 1);

        // Na prática curta colocamos a branca e 3 bolas fáceis para experimentar mira, vista 3D/Superior e efeito sem pressão
        for (int i = 0; i <= 15; i++)
        {
            bolas[i].vel = Vector2.zero;
            bolas[i].caiuNestaTacada = false;
            bolas[i].caindoNaCacapa = false;
            bolas[i].progressoQueda = 0f;
            bolas[i].rotacao = Quaternion.Euler(i * 35f, i * 50f, 0f);
            bolas[i].spinLongitudinal = 0f;
            bolas[i].spinLateral = 0f;
            if (bolas[i].marcaVisual != null) bolas[i].marcaVisual.SetActive(false);
        }

        bolas[0].ativa = true;
        bolas[0].pos = new Vector2(-110f, 0f);

        bolas[1].ativa = true;
        bolas[1].pos = new Vector2(20f, 0f);
        bolas[2].ativa = true;
        bolas[2].pos = new Vector2(95f, 38f);
        bolas[3].ativa = true;
        bolas[3].pos = new Vector2(95f, -38f);
        for (int i = 4; i <= 15; i++)
        {
            bolas[i].ativa = false;
            bolas[i].pos = new Vector2(92f + i * 10f, 0f);
        }

        anguloMiraRad = 0f;
        forcaNormalizada = 0f;
        estado = EstadoPartida.MirandoJogador;
        if (txtPercepcaoSubtitulo != null)
            txtPercepcaoSubtitulo.text = "";

        AtualizarTextosHUD();
        SincronizarTransformBolas();
        AtualizarVisualTacoEMira();
    }

    private void AoClicarBotaoPraticaOuDuelo()
    {
        CancelarCargaAtiva();
        if (modoPratica)
        {
            // Se está na prática, iniciar o duelo real melhor de 3 contra Zé
            ComecarNovaSerie(onVictoryCallback);
            if (painelModal != null) painelModal.SetActive(false);
            IniciarTurnoAtual();
        }
        else
        {
            ComecarModoPratica();
        }
    }

    private void AlternarVistaCamera()
    {
        CancelarCargaAoSuspender();
        if (tableView3D != null)
        {
            tableView3D.AlternarCamera();
            AtualizarBotaoVistaCamera();
            AtualizarVisualTacoEMira();
        }
    }

    private void AtualizarBotaoVistaCamera()
    {
        if (txtBtnVistaCamera == null || tableView3D == null) return;
        txtBtnVistaCamera.text = tableView3D.CameraModo == SinucaCameraView.Mesa3D
            ? "VISTA: DIAGONAL (V)"
            : tableView3D.CameraModo == SinucaCameraView.LinhaTaco ? "VISTA: TACO (V)" : "VISTA: SUPERIOR (V)";
    }

    private void AlternarPainelEfeito()
    {
        CancelarCargaAoSuspender();
        painelEfeitoAberto = !painelEfeitoAberto;
        if (painelEfeitoRecolhivel != null)
            painelEfeitoRecolhivel.SetActive(painelEfeitoAberto);
    }

    public void SelecionarEfeitoBranca(SinucaCueSpin novoEfeito)
    {
        contatoTaco = new Vector2(0,novoEfeito==SinucaCueSpin.Avancar?.65f:novoEfeito==SinucaCueSpin.Recuar?-.65f:0);
        if(spinSelector!=null)spinSelector.SetContact(contatoTaco);
        if (efeitoEscolhido != novoEfeito)
            CancelarCargaAoSuspender();

        efeitoEscolhido = novoEfeito;

        if (txtBtnToggleEfeito != null)
        {
            string nome = novoEfeito == SinucaCueSpin.Avancar ? "AVANÇAR" :
                          novoEfeito == SinucaCueSpin.Recuar ? "RECUAR" : "NORMAL";
            txtBtnToggleEfeito.text = $"EFEITO: {nome}";
        }

        if (txtTendenciaEfeito != null)
        {
            switch (novoEfeito)
            {
                case SinucaCueSpin.Avancar:
                    txtTendenciaEfeito.text = "Avançar (opcional): a branca tende a acompanhar para frente após o contato.";
                    txtTendenciaEfeito.color = new Color32(156, 214, 248, 255);
                    break;
                case SinucaCueSpin.Recuar:
                    txtTendenciaEfeito.text = "Recuar (opcional): a branca tende a frear ou puxar para trás após o contato.";
                    txtTendenciaEfeito.color = new Color32(248, 198, 142, 255);
                    break;
                default:
                    txtTendenciaEfeito.text = "Tacada Normal: a branca segue o rebote natural após o contato.";
                    txtTendenciaEfeito.color = new Color32(186, 214, 198, 255);
                    break;
            }
        }

        AtualizarDestaqueBotoesEfeito();
        AtualizarVisualTacoEMira();
    }

    private void AtualizarDestaqueBotoesEfeito()
    {
        if (btnEfeitoNormal != null)
            btnEfeitoNormal.GetComponent<Image>().color = efeitoEscolhido == SinucaCueSpin.Normal
                ? new Color32(48, 86, 58, 255) : new Color32(34, 38, 44, 255);
        if (btnEfeitoAvancar != null)
            btnEfeitoAvancar.GetComponent<Image>().color = efeitoEscolhido == SinucaCueSpin.Avancar
                ? new Color32(42, 74, 98, 255) : new Color32(34, 38, 44, 255);
        if (btnEfeitoRecuar != null)
            btnEfeitoRecuar.GetComponent<Image>().color = efeitoEscolhido == SinucaCueSpin.Recuar
                ? new Color32(94, 64, 38, 255) : new Color32(34, 38, 44, 255);
    }

    private void AbrirTutorialInicial()
    {
        estado = EstadoPartida.TutorialRegras;
        MostrarModal(
            "SINUCA NO BAR DO ZÉ — MELHOR DE 3 MÃOS",
            "Quinze bolas: lisas 1–7 e listradas 9–15. A primeira conversão válida define os grupos.\n" +
            "Converta todas as bolas do seu grupo. Só então jogue na preta 8.\n" +
            "Falta: bola na mão ao adversário. Limpe seu grupo e só então converta a preta 8; preta antecipada perde a mão.\n" +
            "A branca já começa pronta. Reposicionamento só depois de falta. Da 2ª mão em diante, não encaçape a bola com X: a tacada será anulada.\n" +
            "A/D: contornar. Setas ou mouse: mirar. Segure/solte para tacar. V: vistas. Da 2ª mão em diante, cair a marcada anula a tacada e devolve a formação. Duas vitórias vencem a série.",
            "DUELAR COM ZÉ",
            () =>
            {
                painelModal.SetActive(false);
                IniciarTurnoAtual();
            },
            "PRÁTICA CURTA",
            () =>
            {
                painelModal.SetActive(false);
                ComecarModoPratica();
            }
        );
    }

    private void AbrirModalRegrasDuranteJogo()
    {
        if (estado == EstadoPartida.BolasEmMovimento || estado == EstadoPartida.ResolvendoTacada) return;
        CancelarCargaAoSuspender();
        EstadoPartida estadoAnterior = estado;
        MostrarModal(
            "REGRAS DA MESA — BAR DO ZÉ",
            "Quinze bolas: lisas 1–7 e listradas 9–15. A primeira conversão válida define os grupos.\n" +
            "Converta todas as bolas do seu grupo. Só então jogue na preta 8.\n" +
            "Falta: bola na mão ao adversário. Limpe seu grupo e só então converta a preta 8; preta antecipada perde a mão.\n" +
            "A branca já começa pronta. Reposicionamento só depois de falta. Da 2ª mão em diante, não encaçape a bola com X: a tacada será anulada.\n" +
            "A/D: contornar. Setas ou mouse: mirar. Segure/solte para tacar. V: vistas. Da 2ª mão em diante, cair a marcada anula a tacada e devolve a formação. Duas vitórias vencem a série.",
            "VOLTAR À MESA",
            () =>
            {
                painelModal.SetActive(false);
                if (estado == EstadoPartida.TutorialRegras)
                    IniciarTurnoAtual();
                else
                    estado = estadoAnterior;
            },
            modoPratica ? "IR PARA O DUELO" : "PRÁTICA CURTA",
            () =>
            {
                painelModal.SetActive(false);
                AoClicarBotaoPraticaOuDuelo();
            }
        );
    }

    private void ConfigurarMao(int numeroMao)
    {
        modoPratica = false;
        maoAtual = numeroMao;
        pontosMarcus = 0;
        pontosZe = 0;
        regrasBrasileiras = new SinuquinhaRules(numeroMao % 2 == 1);
        turnoDeMarcus = (numeroMao % 2 == 1); // Alternating opening player.
        brancaNaMao = false;
        numeroBolaMarcada = -1;
        contadorTurnosMao = 0;
        acumuladorFisica = 0f;
        CancelarCargaAtiva();
        SelecionarEfeitoBranca(SinucaCueSpin.Normal);

        if (Application.isPlaying) SantaRitaBarAudio.DefinirContexto(true, numeroMao);

        // Posicionar Branca na cabeceira esquerda
        bolas[0].ativa = true;
        bolas[0].caiuNestaTacada = false;
        bolas[0].caindoNaCacapa = false;
        bolas[0].progressoQueda = 0f;
        bolas[0].pos = new Vector2(-110f, 0f);
        bolas[0].vel = Vector2.zero;
        bolas[0].rotacao = Quaternion.identity;
        bolas[0].spinLongitudinal = 0f;

        // Individual home marks of the Brazilian game.
        for (int i = 1; i <= 15; i++)
        {
            bolas[i].ativa = true;
            bolas[i].caiuNestaTacada = false;
            bolas[i].caindoNaCacapa = false;
            bolas[i].progressoQueda = 0f;
            bolas[i].pos = marcasBrasileiras[i];
            bolas[i].vel = Vector2.zero;
            bolas[i].rotacao = Quaternion.Euler(i * 28f, i * 44f, 0f);
            bolas[i].spinLongitudinal = 0f;
            bolas[i].spinLateral = 0f;
            if (bolas[i].marcaVisual != null) bolas[i].marcaVisual.SetActive(false);
        }

        anguloMiraRad = 0f;
        forcaNormalizada = 0f;
        AtualizarBarraForcaVisual(0f);

        AplicarPercepcaoPorMao();
        SincronizarTransformBolas();
        AtualizarTextosHUD();
        AtualizarVisualTacoEMira();
    }

    private void AplicarPercepcaoPorMao()
    {
        bool reduzir = MenuPausa.ReduzirEfeitosIntensos;
        multiplicadorLuzAmbiente = 1f;

        if (modoPratica || maoAtual == 1)
        {
            overlayPercepcao.color = new Color(0f, 0f, 0f, 0f);
            txtPercepcaoSubtitulo.text = modoPratica
                ? ""
                : "O rádio de pilha chiando baixo no balcão. Zé observa sua postura no taco.";
        }
        else if (maoAtual == 2)
        {
            overlayPercepcao.color = reduzir ? new Color(0f, 0f, 0f, 0f) : new Color(0.06f, 0.08f, 0.12f, 0.08f);
            txtPercepcaoSubtitulo.text = "[O murmúrio do bar cessa. Ouve-se apenas a goteira no balde e o giz azul riscando a bola marcada.]";
        }
        else
        {
            overlayPercepcao.color = reduzir ? new Color(0f, 0f, 0f, 0f) : new Color(0.14f, 0.05f, 0.04f, 0.12f);
            txtPercepcaoSubtitulo.text = "[03:17 no relógio parado atrás do balcão. A chuva bate forte na porta trancada dos fundos.]";
        }
    }

    private void IniciarTurnoAtual()
    {
        cantadaManual=0;
        if(txtAlvo!=null)txtAlvo.text="ALVO: AUTO";
        if(painelAlvo!=null)painelAlvo.SetActive(false);
        if (modoPratica)
        {
            turnoDeMarcus = true;
            if (!bolas[0].ativa)
            {
                bolas[0].ativa = true;
                bolas[0].pos = EncontrarPosicaoLivreDeterministica(new Vector2(-110f, 0f), 0);
            }
            // Se todas as bolas numeradas da prática caíram, repõe automaticamente as 3 bolas de treino
            int rest = 0;
            for (int i = 1; i <= 15; i++) if (bolas[i].ativa) rest++;
            if (rest == 0)
            {
                bolas[1].ativa = true; bolas[1].pos = EncontrarPosicaoLivreDeterministica(new Vector2(20f, 0f), 1);
                bolas[2].ativa = true; bolas[2].pos = EncontrarPosicaoLivreDeterministica(new Vector2(95f, 38f), 2);
                bolas[3].ativa = true; bolas[3].pos = EncontrarPosicaoLivreDeterministica(new Vector2(95f, -38f), 3);
                txtTurnoStatus.text = "Prática: Bolas de treino repostas na mesa.";
            }
            estado = brancaNaMao ? EstadoPartida.PosicionandoBranca : EstadoPartida.MirandoJogador;
            SincronizarTransformBolas();
            AtualizarTextosHUD();
            AtualizarVisualTacoEMira();
            return;
        }

        contadorTurnosMao++;
        AtualizarRegraBolaMarcadaAntesDoTurno();

        // Demonstração breve na abertura da 2ª mão quando a Bola Marcada entra em vigor
        if (maoAtual == 2 && !demonstracaoMao2Feita && numeroBolaMarcada != -1)
        {
            demonstracaoMao2Feita = true;
            TocarSomGiz();
            MostrarModal(
                "2ª MÃO — \"ESSA FICA NA MESA\"",
                "Zé apoia o polegar na borda da caçapa e risca um <b>[X]</b> de giz sobre a <b>Bola " + numeroBolaMarcada + "</b>:\n\n" +
                "<i>\"Aqui em Santa Rita a gente aprende cedo que tem coisa que não se mexe, Marcus. Essa bola marcada fica na mesa. " +
                "Pode até resvalar nela, mas se ela cair na caçapa, a tacada inteira não vale nada.\"</i>\n\n" +
                "(A bola marcada muda a cada turno enquanto houver duas ou mais bolas disponíveis do seu grupo.)",
                "ENTENDIDO",
                () =>
                {
                    painelModal.SetActive(false);
                    ContinuarInicioTurno();
                },
                null, null
            );
            return;
        }

        // Pequena oscilação de luz na troca de turnos da 2ª e 3ª mão (nunca durante a mira/carga!)
        if (!MenuPausa.ReduzirEfeitosIntensos && maoAtual >= 2)
        {
            timerOscilacaoLuz = 0.32f;
        }

        ContinuarInicioTurno();
    }

    private void ContinuarInicioTurno()
    {
        CancelarCargaAtiva();
        // Cada tacada volta ao padrão Normal para nunca exigir gerenciamento extra (o jogador pode trocar se quiser)
        SelecionarEfeitoBranca(SinucaCueSpin.Normal);

        SincronizarTransformBolas();
        AtualizarTextosHUD();

        if (!bolas[0].ativa)
        {
            bolas[0].ativa = true;
            bolas[0].caindoNaCacapa = false;
            bolas[0].progressoQueda = 0f;
            bolas[0].pos = EncontrarPosicaoLivreDeterministica(new Vector2(-110f, 0f), 0);
            bolas[0].vel = Vector2.zero;
            SincronizarTransformBolas();
        }

        if (turnoDeMarcus)
        {
            if (brancaNaMao)
            {
                estado = EstadoPartida.PosicionandoBranca;
                txtTurnoStatus.text = "SUA VEZ (BOLA NA MÃO): Mova com Mouse ou setas e confirme com Clique Esq. ou E.";
            }
            else
            {
                estado = EstadoPartida.MirandoJogador;
                ApontarMiraParaBolaMaisProxima();
                txtTurnoStatus.text = "SUA VEZ, MARCUS: Mire com Mouse ou ←/→ e segure Clique Esq. ou Espaço para tacar.";
            }
            AtualizarVisualTacoEMira();
        }
        else
        {
            estado = EstadoPartida.TurnoZePensando;
            txtTurnoStatus.text = brancaNaMao
                ? "VEZ DE ZÉ (Bola na Mão): Zé posiciona a branca..."
                : "VEZ DE ZÉ: Zé analisa os ângulos da mesa...";
            AtualizarVisualTacoEMira();
            StartCoroutine(ExecutarTurnoZeCoroutine());
        }
    }

    private void AtualizarRegraBolaMarcadaAntesDoTurno()
    {
        numeroBolaMarcada=-1;if(modoPratica||maoAtual<2||regrasBrasileiras==null)return;
        var candidates=new List<int>();for(int n=1;n<16;n++)if(n!=8&&bolas[n].ativa&&regrasBrasileiras.CanNominate(n))candidates.Add(n);
        if(candidates.Count>=2)numeroBolaMarcada=candidates[(contadorTurnosMao+maoAtual)%candidates.Count];
    }

    private void ApontarMiraParaBolaMaisProxima()
    {
        float menorDist = float.MaxValue;
        Vector2 alvo = new Vector2(92f, 0f);

        for (int i = 1; i <= 15; i++)
        {
            if (!bolas[i].ativa || i == numeroBolaMarcada || (!modoPratica && regrasBrasileiras!=null && !regrasBrasileiras.CanNominate(i))) continue;
            float d = Vector2.Distance(bolas[0].pos, bolas[i].pos);
            if (d < menorDist)
            {
                menorDist = d;
                alvo = bolas[i].pos;
            }
        }

        Vector2 dir = (alvo - bolas[0].pos).normalized;
        if (dir.sqrMagnitude > 0.001f)
            anguloMiraRad = Mathf.Atan2(dir.y, dir.x);
    }

    private void Update()
    {
        if (!IsShowing || estado == EstadoPartida.Inativo) return;

        // Proteção contra mudança de resolução durante carga
        if (Screen.width != ultimaLarguraTela || Screen.height != ultimaAlturaTela)
        {
            ultimaLarguraTela = Screen.width;
            ultimaAlturaTela = Screen.height;
            CancelarCargaAoSuspender();
        }

        if (MenuPausa.Aberto)
        {
            CancelarCargaAoSuspender();
            return;
        }

        if (painelModal != null && painelModal.activeSelf)
        {
            CancelarCargaAoSuspender();
            return;
        }

        var kb = Keyboard.current;
        if(painelAlvo!=null&&painelAlvo.activeSelf)
        {
            if(kb!=null&&kb.escapeKey.wasPressedThisFrame)painelAlvo.SetActive(false);
            return;
        }

        if(kb!=null && (kb.aKey.isPressed||kb.dKey.isPressed))
        {
            CancelarCargaAoSuspender();
            float orbit=(kb.aKey.isPressed?1:0)-(kb.dKey.isPressed?1:0);
            tableView3D.Orbit(orbit*(kb.shiftKey.isPressed?25f:70f)*Time.unscaledDeltaTime);
        }
        // Atalho V para alternar entre Mesa 3D e Vista Superior (cancela carga com segurança)
        if (kb != null && kb.vKey.wasPressedThisFrame)
        {
            AlternarVistaCamera();
        }

        // Atualizar oscilação sutil de luz apenas entre turnos (jamais durante mira/carga)
        if (timerOscilacaoLuz > 0f && estado != EstadoPartida.MirandoJogador && estado != EstadoPartida.CarregandoForca)
        {
            timerOscilacaoLuz -= Time.deltaTime;
            multiplicadorLuzAmbiente = 1f - Mathf.Sin(timerOscilacaoLuz * 28f) * 0.08f;
        }
        else
        {
            multiplicadorLuzAmbiente = 1f;
        }

        switch (estado)
        {
            case EstadoPartida.PosicionandoBranca:
                AtualizarPosicionamentoBranca();
                break;

            case EstadoPartida.MirandoJogador:
                AtualizarMiraJogador();
                break;

            case EstadoPartida.CarregandoForca:
                AtualizarCargaForca();
                break;

            case EstadoPartida.BolasEmMovimento:
                AvancarSimulacaoPassoFixo(Time.deltaTime);
                break;
        }
    }

    private void AvancarSimulacaoPassoFixo(float deltaTime)
    {
        if (deltaTime <= 0f || float.IsNaN(deltaTime)) return;
        acumuladorFisica += Mathf.Min(deltaTime, 0.1f) + 0.000001f;
        int safety = 0;
        while (acumuladorFisica >= FixedStep && estado == EstadoPartida.BolasEmMovimento && safety++ < 24)
        {
            acumuladorFisica -= FixedStep;
            SimularFisicaMesa(FixedStep);
        }
    }

    // =========================================================================
    // Entrada do Jogador (Mouse 3D + Teclado A/D + Espaço + Proteções S02..S07)
    // =========================================================================

    private void OnMesaPointerDown(Vector2 tablePos, PointerEventData.InputButton button)
    {
        if (MenuPausa.Aberto || (painelModal != null && painelModal.activeSelf)) return;

        if (button == PointerEventData.InputButton.Right)
        {
            if (estado == EstadoPartida.CarregandoForca)
            {
                CancelarCargaAtiva();
                txtTurnoStatus.text = "Tacada cancelada. Ajuste a mira e segure para carregar.";
            }
            return;
        }

        if (button != PointerEventData.InputButton.Left) return;

        if (estado == EstadoPartida.PosicionandoBranca)
        {
            PosicionarBrancaEm(tablePos);
            if (PosicaoValidaSemColisao(bolas[0].pos, 0))
            {
                ConfirmarBrancaNaMao();
            }
            else
            {
                txtTurnoStatus.text = "Posição inválida! Afaste a branca das outras bolas e das caçapas.";
            }
            return;
        }

        if (estado == EstadoPartida.MirandoJogador && Time.frameCount != frameConfirmacaoBranca)
        {
            AtualizarAnguloMiraParaPonto(tablePos);
            IniciarCargaPorDispositivo(DispositivoCarga.MouseEsquerdo);
        }
    }

    private void OnMesaPointerMove(Vector2 tablePos)
    {
        if (MenuPausa.Aberto || (painelModal != null && painelModal.activeSelf)) return;

        if (estado == EstadoPartida.PosicionandoBranca)
        {
            PosicionarBrancaEm(tablePos);
            return;
        }

        // Durante a carga, a mira permanece travada (mover o mouse NÃO altera direção nem força)
        if (estado == EstadoPartida.MirandoJogador)
        {
            AtualizarAnguloMiraParaPonto(tablePos);
            AtualizarVisualTacoEMira();
        }
    }

    private void OnMesaPointerUp(Vector2 tablePos, bool dentroDoViewport, PointerEventData.InputButton button)
    {
        if (estado != EstadoPartida.CarregandoForca) return;
        if (dispositivoCargaAtivo != DispositivoCarga.MouseEsquerdo || button != PointerEventData.InputButton.Left)
            return;

        if (!dentroDoViewport || MenuPausa.Aberto || (painelModal != null && painelModal.activeSelf))
        {
            CancelarCargaAtiva();
            txtTurnoStatus.text = "Tacada cancelada fora da mesa.";
            return;
        }

        DispararTacadaCarregada();
    }

    private void OnMesaScrollZoom(float novoZoom)
    {
        AtualizarVisualTacoEMira();
    }

    private void PosicionarBrancaEm(Vector2 tablePos)
    {
        float limX = TableWidth * 0.5f - BallRadius - 6f;
        float limY = TableHeight * 0.5f - BallRadius - 6f;
        bolas[0].pos = new Vector2(
            Mathf.Clamp(tablePos.x, -limX, limX),
            Mathf.Clamp(tablePos.y, -limY, limY));
        SincronizarTransformBolas();
        AtualizarVisualTacoEMira();
    }

    private void ConfirmarBrancaNaMao()
    {
        brancaNaMao = false;
        frameConfirmacaoBranca = Time.frameCount; // Impede que o mesmo clique/tecla inicie carga no mesmo frame (S06)
        estado = EstadoPartida.MirandoJogador;
        ApontarMiraParaBolaMaisProxima();
        tableView3D.RecentrarCamera();
        txtTurnoStatus.text = "Branca posicionada. Mire e segure Clique Esq. ou Espaço para tacar.";
        AtualizarVisualTacoEMira();
    }

    private void AtualizarAnguloMiraParaPonto(Vector2 tablePos)
    {
        Vector2 delta = tablePos - bolas[0].pos;
        if (delta.sqrMagnitude > 4f)
        {
            anguloMiraRad = Mathf.Atan2(delta.y, delta.x);
        }
    }

    private void AtualizarPosicionamentoBranca()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        Vector2 move = Vector2.zero;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) move.y += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) move.y -= 1f;
        if (kb.leftArrowKey.isPressed) move.x -= 1f;
        if (kb.rightArrowKey.isPressed) move.x += 1f;

        if (move.sqrMagnitude > 0.01f)
        {
            var camera=tableView3D.Room.camera;var r=camera.transform.right;var f=camera.transform.up;Vector2 screenRight=new Vector2(r.x,r.z).normalized,screenUp=new Vector2(f.x,f.z).normalized;
            PosicionarBrancaEm(bolas[0].pos + (screenRight*move.x+screenUp*move.y).normalized * 135f * Time.deltaTime);
        }

        if (kb.eKey.wasPressedThisFrame || kb.enterKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame)
        {
            if (PosicaoValidaSemColisao(bolas[0].pos, 0))
            {
                ConfirmarBrancaNaMao();
            }
            else
            {
                txtTurnoStatus.text = "Posição inválida! Afaste a branca das outras bolas e das caçapas.";
            }
        }
    }

    private void AtualizarMiraJogador()
    {
        var kb = Keyboard.current;
        if (kb != null)
        {
            float rotSpeed = 1.65f;
            if (kb.shiftKey.isPressed) rotSpeed = 0.55f;

            if (kb.leftArrowKey.isPressed)
                anguloMiraRad -= tableView3D.AngularScreenSign(bolas[0].pos,anguloMiraRad)*rotSpeed*Time.deltaTime;
            if (kb.rightArrowKey.isPressed)
                anguloMiraRad += tableView3D.AngularScreenSign(bolas[0].pos,anguloMiraRad)*rotSpeed*Time.deltaTime;

            if (Time.frameCount != frameConfirmacaoBranca && kb.spaceKey.wasPressedThisFrame)
            {
                IniciarCargaPorDispositivo(DispositivoCarga.TeclaEspaco);
                return;
            }
        }

        // Terror psicológico sutil do Bar do Zé: quando Zé está passando giz no taco reserva,
        // se Marcus aponta diretamente para a Bola Marcada [X], o som de giz de Zé para subitamente.
        if (!modoPratica && maoAtual >= 2 && !miraAmeacaBolaMarcada && Time.unscaledTime - ultimoSomGiz > 4.8f)
        {
            ultimoSomGiz = Time.unscaledTime;
            TocarSomGiz(0.08f);
        }

        AtualizarVisualTacoEMira();
    }

    private void IniciarCargaPorDispositivo(DispositivoCarga disp)
    {
        dispositivoCargaAtivo = disp;
        estado = EstadoPartida.CarregandoForca;
        forcaNormalizada = 0.08f;
        carregandoDirectionUp = true;
        AtualizarBarraForcaVisual(forcaNormalizada);
        txtTurnoStatus.text = "Carregando força... Solte para tacar (Dir. ou C cancela).";
        AtualizarVisualTacoEMira();
    }

    private void CancelarCargaAtiva()
    {
        dispositivoCargaAtivo = DispositivoCarga.Nenhum;
        forcaNormalizada = 0f;
        AtualizarBarraForcaVisual(0f);
        if (estado == EstadoPartida.CarregandoForca)
        {
            estado = EstadoPartida.MirandoJogador;
        }
        AtualizarVisualTacoEMira();
    }

    private void AtualizarBarraForcaVisual(float valor01)
    {
        float v = Mathf.Clamp01(valor01);
        if (barraForcaFill != null)
        {
            barraForcaFill.fillAmount = v;
            RectTransform rt = barraForcaFill.rectTransform;
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(v, 1f);
            rt.offsetMin = new Vector2(1f, 1f);
            rt.offsetMax = new Vector2(v > 0.01f ? -1f : 0f, -1f);
            barraForcaFill.color = Color.Lerp(new Color32(215, 168, 52, 255), new Color32(245, 98, 38, 255), v);
        }
    }

    public static float OscilarCarga(float atual, float dt, ref bool subindo)
    {
        const float chargeSpeed = 1.05f;
        atual += (subindo ? 1f : -1f) * chargeSpeed * dt;
        if (atual >= 1f)
        {
            atual = 1f;
            subindo = false;
        }
        else if (atual <= 0.08f)
        {
            atual = 0.08f;
            subindo = true;
        }
        return atual;
    }

    private void AtualizarCargaForca()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;

        // Cancelar carga sem consumir tacada (Botão Direito, C, Backspace)
        bool cancelarTeclado = kb != null && (kb.cKey.wasPressedThisFrame || kb.backspaceKey.wasPressedThisFrame);
        bool cancelarMouse = mouse != null && mouse.rightButton.wasPressedThisFrame;
        if (cancelarTeclado || cancelarMouse)
        {
            CancelarCargaAtiva();
            txtTurnoStatus.text = "Tacada cancelada. Ajuste a mira e segure para carregar.";
            return;
        }

        // A carga avança mesmo com o mouse totalmente parado
        forcaNormalizada = OscilarCarga(forcaNormalizada, Time.deltaTime, ref carregandoDirectionUp);
        AtualizarBarraForcaVisual(forcaNormalizada);

        AtualizarVisualTacoEMira();

        // Somente a liberação do MESMO dispositivo que iniciou a carga dispara a tacada (S02/S04)
        if (dispositivoCargaAtivo == DispositivoCarga.TeclaEspaco && kb != null && kb.spaceKey.wasReleasedThisFrame)
        {
            DispararTacadaCarregada();
        }
    }

    private void DispararTacadaCarregada()
    {
        float forcaFinal = Mathf.Clamp01(forcaNormalizada);
        dispositivoCargaAtivo = DispositivoCarga.Nenhum;
        forcaNormalizada = 0f;
        AtualizarBarraForcaVisual(0f);

        Vector2 dir = new Vector2(Mathf.Cos(anguloMiraRad), Mathf.Sin(anguloMiraRad));
        float potencia = Mathf.Lerp(MinShotPower, MaxShotPower, forcaFinal);
        ExecutarTacadaComEfeito(dir, potencia, efeitoEscolhido);
    }

    // =========================================================================
    // IA de Zé (Avaliação Física no Mesmo Kernel + Consciência da Bola Marcada)
    // =========================================================================

    private IEnumerator ExecutarTurnoZeCoroutine()
    {
        yield return new WaitForSeconds(0.65f);
        while (MenuPausa.Aberto || (painelModal != null && painelModal.activeSelf))
            yield return null;

        if (brancaNaMao)
        {
            bolas[0].pos = EscolherPosicaoBrancaZe();
            brancaNaMao = false;
            SincronizarTransformBolas();
            AtualizarVisualTacoEMira();
            yield return new WaitForSeconds(0.45f);
            while (MenuPausa.Aberto || (painelModal != null && painelModal.activeSelf))
                yield return null;
        }

        EscolherTacadaZe(out Vector2 direcaoZe, out float potenciaZe);
        anguloMiraRad = Mathf.Atan2(direcaoZe.y, direcaoZe.x);
        AtualizarVisualTacoEMira();
        yield return new WaitForSeconds(0.32f);
        while (MenuPausa.Aberto || (painelModal != null && painelModal.activeSelf) || estado == EstadoPartida.TutorialRegras)
            yield return null;
        if (estado != EstadoPartida.TurnoZePensando) yield break;

        ExecutarTacadaComEfeito(direcaoZe, potenciaZe, SinucaCueSpin.Normal);
    }

    private Vector2 EscolherPosicaoBrancaZe()
    {
        int on=regrasBrasileiras!=null?regrasBrasileiras.OnBall:1;
        Vector2 best=EncontrarPosicaoLivreDeterministica(new Vector2(-135,0),0);float score=float.NegativeInfinity;
        for(float x=-150;x<=-110;x+=2)for(float y=-44;y<=44;y+=2)
        {
            Vector2 p=new Vector2(x,y);if(!PosicaoValidaSemColisao(p,0))continue;
            bool clear=on>0&&CaminhoLivreEntre(p,bolas[on].pos,0,on);
            float value=(clear?1000:0)-Vector2.Distance(p,bolas[Mathf.Max(1,on)].pos);
            if(value>score){score=value;best=p;}
        }
        return best;
    }

    private void EscolherTacadaZe(out Vector2 direcaoEscolhida, out float potenciaEscolhida)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        UltimosCandidatosZe = 0;

        Vector2 cuePos = bolas[0].pos;
        // Quebrar a formação de quinze bolas com força; as conversões normais permanecem fora.
        if(!modoPratica&&regrasBrasileiras!=null&&regrasBrasileiras.Opening)
        {
            Vector2 delta=bolas[regrasBrasileiras.OnBall].pos-cuePos;
            direcaoEscolhida=delta.normalized;
            potenciaEscolhida=MaxShotPower*.88f;
            UltimosCandidatosZe=1;UltimaDecisaoZeMs=sw.Elapsed.TotalMilliseconds;return;
        }
        float melhorScore = float.MinValue;
        direcaoEscolhida = Vector2.right;
        potenciaEscolhida = 250f;

        // 1. Avaliar tacadas diretas de encaçapamento para todas as bolas legais
        for (int i = 1; i <= 15; i++)
        {
            if (!bolas[i].ativa || i == numeroBolaMarcada || (!modoPratica && regrasBrasileiras!=null && !regrasBrasileiras.CanNominate(i))) continue;

            for (int p = 0; p < cacapas.Length; p++)
            {
                Vector2 pocketPos = cacapas[p];
                Vector2 ballToPocket = (pocketPos - bolas[i].pos);
                float distBallPocket = ballToPocket.magnitude;
                if (distBallPocket < 1f) continue;
                Vector2 dirBallPocket = ballToPocket / distBallPocket;

                // Ponto de impacto na bola-alvo (ghost ball)
                Vector2 ghostPos = bolas[i].pos - dirBallPocket * (BallRadius * 1.98f);
                Vector2 cueToGhost = ghostPos - cuePos;
                float distCueGhost = cueToGhost.magnitude;
                if (distCueGhost < BallRadius * 1.5f) continue;

                Vector2 dirCueGhost = cueToGhost / distCueGhost;
                float cosCut = Vector2.Dot(dirCueGhost, dirBallPocket);
                if (cosCut <= 0.15f) continue; // Ângulo de corte impossível

                if (!CaminhoLivreEntre(cuePos, ghostPos, 0, i)) continue;
                if (!CaminhoLivreEntre(bolas[i].pos, pocketPos, i, -1)) continue;

                float potBase = Mathf.Clamp(165f + (distCueGhost + distBallPocket) * 0.58f / Mathf.Max(0.35f, cosCut), 155f, 415f);
                AvaliarCandidatoZe(dirCueGhost, potBase, ref melhorScore, ref direcaoEscolhida, ref potenciaEscolhida);
            }
        }

        // 2. Candidatos de segurança / centro das bolas legais caso nenhuma caçapa limpa exista
        for (int i = 1; i <= 15; i++)
        {
            if (!bolas[i].ativa || i == numeroBolaMarcada || (!modoPratica && regrasBrasileiras!=null && !regrasBrasileiras.CanNominate(i))) continue;
            Vector2 dirCentro = (bolas[i].pos - cuePos).normalized;
            AvaliarCandidatoZe(dirCentro, 245f, ref melhorScore, ref direcaoEscolhida, ref potenciaEscolhida);
            Vector2 perp = new Vector2(-dirCentro.y, dirCentro.x);
            AvaliarCandidatoZe(((bolas[i].pos + perp * (BallRadius * 0.85f)) - cuePos).normalized, 265f,
                ref melhorScore, ref direcaoEscolhida, ref potenciaEscolhida);
        }

        // Pequena variação humana explícita antes da tacada (mantém a partida equilibrada e vencível com Normal)
        int seed = maoAtual * 131 + contadorTurnosMao * 37 + pontosZe * 17 + pontosMarcus * 7;
        float jitterGraus = (((seed % 19) - 9) / 9f) * 1.35f;
        direcaoEscolhida = RotateVector2(direcaoEscolhida, jitterGraus * Mathf.Deg2Rad).normalized;

        UltimaDecisaoZeMs = sw.Elapsed.TotalMilliseconds;
    }

    private void AvaliarCandidatoZe(Vector2 dir, float potencia, ref float melhorScore, ref Vector2 melhorDir, ref float melhorPot)
    {
        UltimosCandidatosZe++;
        SimularPrevisaoRapida(dir, potencia,
            out int bolasLegaisCaidas,
            out bool caiuBranca,
            out bool caiuMarcada,
            out bool tocouObjeto,
            out bool tocouTabelaOuCacapa,
            out float menorDistCacapa);

        float score = 0f;
        if (caiuBranca || caiuMarcada || !tocouObjeto || !tocouTabelaOuCacapa)
        {
            score -= 500f;
        }
        else
        {
            score += bolasLegaisCaidas * 140f;
            score += Mathf.Clamp(120f - menorDistCacapa * 0.45f, 0f, 80f);
        }

        if (score > melhorScore)
        {
            melhorScore = score;
            melhorDir = dir;
            melhorPot = potencia;
        }
    }

    private void SimularPrevisaoRapida(
        Vector2 dir,
        float potencia,
        out int bolasLegaisCaidas,
        out bool caiuBranca,
        out bool caiuMarcada,
        out bool tocouObjeto,
        out bool tocouTabelaOuCacapa,
        out float menorDistCacapa)
    {
        Vector2[] p = new Vector2[16];
        Vector2[] v = new Vector2[16];
        bool[] a = new bool[16];
        for (int i = 0; i < 16; i++)
        {
            p[i] = bolas[i].pos;
            v[i] = Vector2.zero;
            a[i] = bolas[i].ativa;
        }

        v[0] = dir.normalized * potencia;
        bolasLegaisCaidas = 0;
        caiuBranca = false;
        caiuMarcada = false;
        tocouObjeto = false;
        tocouTabelaOuCacapa = false;
        menorDistCacapa = 999f;

        float limX = TableWidth * 0.5f - BallRadius;
        float limY = TableHeight * 0.5f - BallRadius;
        const float dt = 1f / 90f;

        for (int step = 0; step < 320; step++)
        {
            bool movendo = false;
            for (int i = 0; i < 16; i++)
            {
                if (!a[i]) continue;
                float spdSq = v[i].sqrMagnitude;
                if (spdSq <= StopSpeedSq)
                {
                    v[i] = Vector2.zero;
                    continue;
                }
                movendo = true;
                p[i] += v[i] * dt;
                float spd = Mathf.Sqrt(spdSq);
                v[i] = v[i] / spd * Mathf.Max(0f, spd - BallFriction * dt);

                for (int c = 0; c < cacapas.Length; c++)
                {
                    float dCac = Vector2.Distance(p[i], cacapas[c]);
                    if (i > 0 && (modoPratica || regrasBrasileiras == null || regrasBrasileiras.CanNominate(i)) && dCac < menorDistCacapa)
                        menorDistCacapa = dCac;

                    if (dCac <= PocketRadius)
                    {
                        a[i] = false;
                        v[i] = Vector2.zero;
                        tocouTabelaOuCacapa = true;
                        if (i == 0) caiuBranca = true;
                        else if (i == numeroBolaMarcada) caiuMarcada = true;
                        else if(i==8&&!modoPratica&&regrasBrasileiras!=null&&!regrasBrasileiras.CanNominate(8)) caiuMarcada=true;
                        else if(modoPratica||regrasBrasileiras==null||regrasBrasileiras.CanNominate(i)) bolasLegaisCaidas++;
                        break;
                    }
                }
                if (!a[i]) continue;

                if (p[i].x < -limX && !PertoDeCacapa(p[i], 20f)) { p[i].x = -limX; v[i].x = Mathf.Abs(v[i].x) * CushionRestitution; if (tocouObjeto) tocouTabelaOuCacapa = true; }
                else if (p[i].x > limX && !PertoDeCacapa(p[i], 20f)) { p[i].x = limX; v[i].x = -Mathf.Abs(v[i].x) * CushionRestitution; if (tocouObjeto) tocouTabelaOuCacapa = true; }

                if (p[i].y < -limY && !PertoDeCacapa(p[i], 20f)) { p[i].y = -limY; v[i].y = Mathf.Abs(v[i].y) * CushionRestitution; if (tocouObjeto) tocouTabelaOuCacapa = true; }
                else if (p[i].y > limY && !PertoDeCacapa(p[i], 20f)) { p[i].y = limY; v[i].y = -Mathf.Abs(v[i].y) * CushionRestitution; if (tocouObjeto) tocouTabelaOuCacapa = true; }
            }

            float minDist = BallRadius * 2f;
            for (int i = 0; i < 16; i++)
            {
                if (!a[i]) continue;
                for (int j = i + 1; j < 16; j++)
                {
                    if (!a[j]) continue;
                    Vector2 delta = p[j] - p[i];
                    float dist = delta.magnitude;
                    if (dist < minDist && dist > 0.0001f)
                    {
                        if (i == 0 || j == 0){if(!tocouObjeto&&!modoPratica&&regrasBrasileiras!=null&&!regrasBrasileiras.CanNominate(i==0?j:i))caiuMarcada=true;tocouObjeto=true;}
                        Vector2 n = delta / dist;
                        float overlap = minDist - dist;
                        p[i] -= n * (overlap * 0.5f);
                        p[j] += n * (overlap * 0.5f);
                        float vn = Vector2.Dot(v[i] - v[j], n);
                        if (vn > 0f)
                        {
                            Vector2 imp = n * (vn * BallRestitution);
                            v[i] -= imp;
                            v[j] += imp;
                        }
                    }
                }
            }

            if (!movendo) break;
        }
    }

    private bool CaminhoLivreEntre(Vector2 a, Vector2 b, int ignorar1, int ignorar2)
    {
        Vector2 ab = b - a;
        float len = ab.magnitude;
        if (len < 0.01f) return true;
        Vector2 dir = ab / len;

        for (int i = 0; i < bolas.Count; i++)
        {
            if (i == ignorar1 || i == ignorar2 || !bolas[i].ativa) continue;
            Vector2 ap = bolas[i].pos - a;
            float proj = Vector2.Dot(ap, dir);
            if (proj <= 0f || proj >= len) continue;
            Vector2 closest = a + dir * proj;
            if (Vector2.Distance(bolas[i].pos, closest) < BallRadius * 1.95f)
                return false;
        }
        return true;
    }

    private static Vector2 RotateVector2(Vector2 v, float rad)
    {
        float c = Mathf.Cos(rad);
        float s = Mathf.Sin(rad);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }

    // =========================================================================
    // Execução da Tacada e Simulação Física Passo Fixo (Swept + Efeito + Bicos)
    // =========================================================================

    private void ExecutarTacada(Vector2 direcao, float potencia)
    {
        ExecutarTacadaComEfeito(direcao, potencia, SinucaCueSpin.Normal);
    }

    private void ExecutarTacadaComEfeito(Vector2 direcao, float potencia, SinucaCueSpin efeito)
    {
        CancelarCargaAtiva();
        for(int n=0;n<bolas.Count;n++){inicioTacadaPos[n]=bolas[n].pos;inicioTacadaRot[n]=bolas[n].rotacao;inicioTacadaAtiva[n]=bolas[n].ativa;}
        estado = EstadoPartida.BolasEmMovimento;
        acumuladorFisica = 0f;

        houvePrimeiroContatoBolaObjeto = false;
        primeiroContatoNumero = -1;
        var cantada = CalcularGuiaDeMira(bolas[0].pos, Mathf.Atan2(direcao.y,direcao.x), efeito);
        bolaCantada = turnoDeMarcus && cantadaManual>0 ? cantadaManual : cantada.atingeBola ? cantada.numeroBolaAlvo : regrasBrasileiras != null ? regrasBrasileiras.OnBall : 1;
        houveToqueTabelaOuCacapa = false;
        brancaCaiuNestaTacada = false;

        bolasObjetoCaidasNaTacada.Clear();

        for (int i = 0; i < bolas.Count; i++)
        {
            bolas[i].caiuNestaTacada = false;
            bolas[i].caindoNaCacapa = false;
            bolas[i].progressoQueda = 0f;
            bolas[i].spinLongitudinal = 0f;
            bolas[i].spinLateral = 0f;
        }

        Vector2 dirNorm = direcao.sqrMagnitude > 0.0001f ? direcao.normalized : Vector2.right;
        float potClamped = Mathf.Clamp(potencia, 20f, MaxShotPower * 1.15f);
        bolas[0].vel = dirNorm * potClamped * Mathf.Sqrt(1f-.35f*contatoTaco.sqrMagnitude);
        bolas[0].spinLateral=turnoDeMarcus ? contatoTaco.x*potClamped*.3f : 0;

        // Configurar giro longitudinal na bola branca conforme modo escolhido (Normal / Avançar / Recuar)
        bolas[0].spinLongitudinal = efeito == SinucaCueSpin.Avancar ? potClamped * 0.58f :
                                    efeito == SinucaCueSpin.Recuar ? -potClamped * 0.64f : 0f;

        if(turnoDeMarcus && Mathf.Abs(contatoTaco.y)>.01f)bolas[0].spinLongitudinal=contatoTaco.y*potClamped*.4f;
        TocarSomTacada(Mathf.InverseLerp(MinShotPower, MaxShotPower, potClamped));
        AtualizarVisualTacoEMira();
        txtTurnoStatus.text = turnoDeMarcus ? "Marcus executou a tacada..." : "Zé executou a tacada...";
    }

    private void SimularFisicaMesa(float dt)
    {
        if (dt <= 0f || float.IsNaN(dt)) return;

        // Sub-stepping interno para evitar qualquer tunneling mesmo se dt > FixedStep
        int subSteps = Mathf.Clamp(Mathf.CeilToInt(dt / (1f / 240f)), 1, 6);
        float subDt = dt / subSteps;

        float limX = TableWidth * 0.5f - BallRadius;
        float limY = TableHeight * 0.5f - BallRadius;

        for (int s = 0; s < subSteps; s++)
        {
            // 1. Integrar movimento, rolagem 3D e verificar entrada nas caçapas
            for (int i = 0; i < bolas.Count; i++)
            {
                BolaMesa b = bolas[i];

                if (b.caindoNaCacapa)
                {
                    b.progressoQueda = Mathf.MoveTowards(b.progressoQueda, 1f, subDt * 1.6f);
                    b.pos = Vector2.Lerp(b.pos, b.cacapaAlvo, subDt * 12f);
                    continue;
                }

                if (!b.ativa) continue;

                float speedSq = b.vel.sqrMagnitude;
                if (speedSq <= StopSpeedSq && Mathf.Abs(b.spinLongitudinal) < 4f)
                {
                    b.vel = Vector2.zero;
                    b.spinLongitudinal = 0f;
                    continue;
                }

                Vector2 deslocamento = b.vel * subDt;
                b.pos += deslocamento;

                // Atualizar rotação 3D da esfera pelo deslocamento no pano + giro longitudinal
                float dist = deslocamento.magnitude;
                if (dist > 0.0001f)
                {
                    Vector2 dirMov = deslocamento / dist;
                    Vector3 eixoRolagem = new Vector3(dirMov.y, 0f, -dirMov.x);
                    float anguloGraus = (dist / BallRadius) * Mathf.Rad2Deg;
                    b.rotacao = Quaternion.AngleAxis(anguloGraus, eixoRolagem) * b.rotacao;
                }

                // Atrito do pano + decaimento gradual do spin longitudinal
                float speed = Mathf.Sqrt(speedSq);
                if (speed > 0.001f)
                {
                    float decel = BallFriction * subDt;
                    float newSpeed = Mathf.Max(0f, speed - decel);
                    b.vel = (b.vel / speed) * newSpeed;
                }
                b.spinLongitudinal = Mathf.MoveTowards(b.spinLongitudinal, 0f, BallFriction * 0.35f * subDt);
                b.spinLateral=Mathf.MoveTowards(b.spinLateral,0,BallFriction*.18f*subDt);

                // Verificar captura pelas 6 caçapas
                for (int p = 0; p < cacapas.Length; p++)
                {
                    float distPocket = Vector2.Distance(b.pos, cacapas[p]);
                    // Funil suave da boca da caçapa (somente dentro da garganta real)
                    if (distPocket < PocketRadius * 1.18f && distPocket > PocketRadius)
                    {
                        Vector2 pull = (cacapas[p] - b.pos).normalized;
                        b.vel += pull * (42f * subDt);
                    }

                    if (distPocket <= PocketRadius)
                    {
                        RegistrarQuedaNaCacapa(i, cacapas[p]);
                        break;
                    }
                }

                if (!b.ativa) continue;

                // Colisão com os bicos (jaws) das caçapas quando a bola bate na quina da abertura
                for (int j = 0; j < bicosCacapas.Count; j++)
                {
                    BicoCacapa jaw = bicosCacapas[j];
                    Vector2 deltaJaw = b.pos - jaw.pos;
                    float distJaw = deltaJaw.magnitude;
                    float minJawDist = BallRadius + jaw.raio;
                    if (distJaw < minJawDist && distJaw > 0.0001f)
                    {
                        // Se já está praticamente dentro do centro da caçapa, deixa cair
                        if (PertoDeCacapa(b.pos, PocketRadius * 0.96f)) continue;

                        Vector2 nJaw = deltaJaw / distJaw;
                        b.pos = jaw.pos + nJaw * minJawDist;
                        float vnJaw = Vector2.Dot(b.vel, nJaw);
                        if (vnJaw < 0f)
                        {
                            b.vel -= (1f + CushionRestitution * 0.92f) * vnJaw * nJaw;
                            RegistrarTabela();
                            TocarSomTabela(Mathf.Abs(vnJaw) / MaxShotPower);
                        }
                    }
                }

                // Rebote nas tabelas retas (fora da garganta das caçapas)
                bool naBocaCacapa = PertoDeCacapa(b.pos, PocketRadius * 1.12f);
                if (!naBocaCacapa)
                {
                    if (b.pos.x < -limX)
                    {
                        b.pos.x = -limX;
                        if (b.vel.x < 0f)
                        {
                            b.vel.x = -b.vel.x * CushionRestitution;
                            b.vel.y+=b.spinLateral*.18f*(b.pos.x<0?1:-1);b.spinLateral*=.55f;
                            RegistrarTabela();
                            TocarSomTabela(Mathf.Abs(b.vel.x) / MaxShotPower);
                        }
                    }
                    else if (b.pos.x > limX)
                    {
                        b.pos.x = limX;
                        if (b.vel.x > 0f)
                        {
                            b.vel.x = -b.vel.x * CushionRestitution;
                            b.vel.y+=b.spinLateral*.18f*(b.pos.x<0?1:-1);b.spinLateral*=.55f;
                            RegistrarTabela();
                            TocarSomTabela(Mathf.Abs(b.vel.x) / MaxShotPower);
                        }
                    }

                    if (b.pos.y < -limY)
                    {
                        b.pos.y = -limY;
                        if (b.vel.y < 0f)
                        {
                            b.vel.y = -b.vel.y * CushionRestitution;
                            b.vel.x+=b.spinLateral*.18f*(b.pos.y<0?-1:1);b.spinLateral*=.55f;
                            RegistrarTabela();
                            TocarSomTabela(Mathf.Abs(b.vel.y) / MaxShotPower);
                        }
                    }
                    else if (b.pos.y > limY)
                    {
                        b.pos.y = limY;
                        if (b.vel.y > 0f)
                        {
                            b.vel.y = -b.vel.y * CushionRestitution;
                            b.vel.x+=b.spinLateral*.18f*(b.pos.y<0?-1:1);b.spinLateral*=.55f;
                            RegistrarTabela();
                            TocarSomTabela(Mathf.Abs(b.vel.y) / MaxShotPower);
                        }
                    }
                }
            }

            // 2. Colisões elásticas esfera-esfera (com transferência de impulso + efeito longitudinal da branca)
            float minDist = BallRadius * 2f;
            for (int i = 0; i < bolas.Count; i++)
            {
                if (!bolas[i].ativa) continue;
                for (int j = i + 1; j < bolas.Count; j++)
                {
                    if (!bolas[j].ativa) continue;

                    Vector2 delta = bolas[j].pos - bolas[i].pos;
                    float dist = delta.magnitude;
                    if (dist < minDist && dist > 0.0001f)
                    {
                        if (i == 0 || j == 0)
                        {
                            if (!houvePrimeiroContatoBolaObjeto) primeiroContatoNumero = i == 0 ? bolas[j].numero : bolas[i].numero;
                            houvePrimeiroContatoBolaObjeto = true;
                        }

                        Vector2 normal = delta / dist;
                        float overlap = minDist - dist;
                        bolas[i].pos -= normal * (overlap * 0.5f);
                        bolas[j].pos += normal * (overlap * 0.5f);

                        Vector2 relVel = bolas[i].vel - bolas[j].vel;
                        float velAlongNormal = Vector2.Dot(relVel, normal);
                        if (velAlongNormal > 0f)
                        {
                            Vector2 impulse = normal * (velAlongNormal * BallRestitution);
                            bolas[i].vel -= impulse;
                            bolas[j].vel += impulse;

                            // Aplicar efeito longitudinal físico da bola branca (Avançar / Recuar) no contato
                            if (i == 0 && Mathf.Abs(bolas[0].spinLongitudinal) > 1f)
                            {
                                Vector2 dirSpin = normal * bolas[0].spinLongitudinal * 0.72f;
                                bolas[0].vel += dirSpin;
                                bolas[0].spinLongitudinal *= 0.25f;
                            }

                            TocarSomColisaoBola(velAlongNormal / MaxShotPower);
                        }
                    }
                }
            }
        }

        SincronizarTransformBolas();
        AtualizarVisualTacoEMira();

        // 3. Verificar repouso completo de todas as bolas
        bool algumaEmMovimento = false;
        for (int i = 0; i < bolas.Count; i++)
        {
            if (bolas[i].ativa && bolas[i].vel.sqrMagnitude > StopSpeedSq)
            {
                algumaEmMovimento = true;
                break;
            }
            if (bolas[i].caindoNaCacapa && bolas[i].progressoQueda < 0.99f)
            {
                algumaEmMovimento = true;
                break;
            }
        }

        if (!algumaEmMovimento)
        {
            for (int i = 0; i < bolas.Count; i++)
            {
                bolas[i].vel = Vector2.zero;
                bolas[i].spinLongitudinal = 0f;
            bolas[i].spinLateral = 0f;
                bolas[i].caindoNaCacapa = false;
            }
            ResolverFimDaTacada();
        }
    }

    private bool PertoDeCacapa(Vector2 pos, float raioBusca)
    {
        for (int p = 0; p < cacapas.Length; p++)
        {
            if (Vector2.Distance(pos, cacapas[p]) <= raioBusca)
                return true;
        }
        return false;
    }

    private void RegistrarQuedaNaCacapa(int indiceBola, Vector2 posCacapa)
    {
        BolaMesa b = bolas[indiceBola];
        if (!b.ativa) return; // Proteção contra contagem dupla na mesma caçapa (S12)

        b.ativa = false;
        b.vel = Vector2.zero;
        b.spinLongitudinal = 0f;
        b.caiuNestaTacada = true;
        b.caindoNaCacapa = Application.isPlaying;
        b.progressoQueda = 0f;
        b.cacapaAlvo = posCacapa;
        houveToqueTabelaOuCacapa = true;

        TocarSomCacapa();

        if (indiceBola == 0)
        {
            brancaCaiuNestaTacada = true;
        }
        else
        {
            if (!bolasObjetoCaidasNaTacada.Contains(indiceBola))
                bolasObjetoCaidasNaTacada.Add(indiceBola);
        }
    }

    // =========================================================================
    // Resolução da Tacada, Faltas, Reposição e Contabilidade das 15 Bolas
    // =========================================================================

    private void ResolverFimDaTacada()
    {
        estado = EstadoPartida.ResolvendoTacada;

        for (int i = 0; i < bolas.Count; i++)
            bolas[i].caindoNaCacapa = false;

        if (modoPratica)
        {
            ResolverFimDaTacadaPratica();
            return;
        }

        var convertidas = new List<int>();
        foreach (int index in bolasObjetoCaidasNaTacada) convertidas.Add(bolas[index].numero);
        if(regrasBrasileiras==null)regrasBrasileiras=new SinuquinhaRules(turnoDeMarcus);
        var resultado = regrasBrasileiras.Resolve(bolaCantada,primeiroContatoNumero,convertidas,brancaCaiuNestaTacada,numeroBolaMarcada,!houveToqueTabelaOuCacapa);
        if(resultado.revertShot)for(int n=0;n<bolas.Count;n++){bolas[n].pos=inicioTacadaPos[n];bolas[n].rotacao=inicioTacadaRot[n];bolas[n].ativa=inicioTacadaAtiva[n];bolas[n].vel=Vector2.zero;bolas[n].caindoNaCacapa=false;bolas[n].progressoQueda=0;}
        // Highest ball first has precedence when home spots are occupied.
        resultado.respot.Sort((x,y)=>y.CompareTo(x));
        if(resultado.replayOpening)foreach(int n in resultado.respot)bolas[n].ativa=false;
        foreach(int n in resultado.respot)
        {
            bolas[n].ativa=true;bolas[n].vel=Vector2.zero;bolas[n].caiuNestaTacada=false;
            bolas[n].pos=ReporBrasileira(n);
        }
        bolasObjetoCaidasNaTacada.Clear();
        pontosMarcus=regrasBrasileiras.MarcusPoints;pontosZe=regrasBrasileiras.ZePoints;
        turnoDeMarcus=regrasBrasileiras.MarcusTurn;brancaNaMao=resultado.cueInHand;
        if(brancaNaMao){bolas[0].ativa=true;bolas[0].vel=Vector2.zero;bolas[0].pos=EncontrarPosicaoLivreDeterministica(new Vector2(-135,0),0);}
        SincronizarTransformBolas();AtualizarVisualTacoEMira();AtualizarTextosHUD();
        if(regrasBrasileiras.Finished){ConcluirMao(regrasBrasileiras.Winner==0);return;}
        txtTurnoStatus.text=resultado.replayOpening?"Saída repetida, sem penalidade. Bola da vez: "+regrasBrasileiras.OnBall:
            resultado.foul?resultado.reason+". Bola na mão para o adversário.":resultado.credited>0?"Bola do seu grupo convertida. Continua na mesa.":"Sem conversão. Passa a vez.";
        if(regrasBrasileiras.MayRefuse && turnoDeMarcus)
        {
            MostrarModal("ESCOLHA DA JOGADA",txtTurnoStatus.text+" Você pode jogar ou devolver a ação ao Zé.","JOGAR",()=>{painelModal.SetActive(false);AgendarProximoTurno(.3f);},
                "DEVOLVER",()=>{painelModal.SetActive(false);regrasBrasileiras.Refuse();turnoDeMarcus=regrasBrasileiras.MarcusTurn;AgendarProximoTurno(.3f);});
        }
        else AgendarProximoTurno(1f);
    }

    private void ResolverFimDaTacadaPratica()
    {
        if (brancaCaiuNestaTacada || !bolas[0].ativa)
        {
            bolas[0].ativa = true;
            bolas[0].vel = Vector2.zero;
            bolas[0].pos = EncontrarPosicaoLivreDeterministica(new Vector2(-110f, 0f), 0);
            brancaNaMao = true;
            txtTurnoStatus.text = "Prática: Branca caiu na caçapa. Bola na mão para reposicionar.";
        }
        else if (bolasObjetoCaidasNaTacada.Count > 0)
        {
            txtTurnoStatus.text = $"Prática: Boa tacada! ({bolasObjetoCaidasNaTacada.Count} bola(s) encaçapada(s)).";
            brancaNaMao = false;
        }
        else
        {
            txtTurnoStatus.text = "Prática: Mire e segure para testar outra tacada, ou clique em INICIAR DUELO.";
            brancaNaMao = false;
        }

        bolasObjetoCaidasNaTacada.Clear();
        SincronizarTransformBolas();
        AtualizarTextosHUD();
        AtualizarVisualTacoEMira();
        IniciarTurnoAtual();
    }

    private void RegistrarTabela()
    {
        if (houvePrimeiroContatoBolaObjeto) houveToqueTabelaOuCacapa = true;
    }

    private void AgendarProximoTurno(float segundos)
    {
        if (Application.isPlaying) StartCoroutine(AguardarEIniciarProximoTurno(segundos));
    }

    public bool ValidarInvarianteSeteBolas()
    {
        if (modoPratica) return true;
        int emJogo = 0;
        for (int i = 1; i <= 15; i++)
            if (bolas[i].ativa) emJogo++;

        if(regrasBrasileiras==null)return true;
        for(int i=1;i<=15;i++)if(bolas[i].ativa!=regrasBrasileiras.Present[i])return false;
        return pontosMarcus==regrasBrasileiras.MarcusPoints&&pontosZe==regrasBrasileiras.ZePoints;
    }

    private IEnumerator AguardarEIniciarProximoTurno(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        while (MenuPausa.Aberto || (painelModal != null && painelModal.activeSelf))
            yield return null;
        if (estado != EstadoPartida.ResolvendoTacada) yield break;
        IniciarTurnoAtual();
    }

    private void ConcluirMao(bool marcusVenceu)
    {
        estado = EstadoPartida.FimDeMao;
        if (marcusVenceu) maosVencidasMarcus++;
        else maosVencidasZe++;

        AtualizarTextosHUD();

        if (maosVencidasMarcus >= 2)
        {
            ConcluirSerieVitoria();
            return;
        }

        if (maosVencidasZe >= 2)
        {
            ConcluirSerieDerrota();
            return;
        }

        int proximaMao = maoAtual + 1;
        string titulo = marcusVenceu
            ? ("FIM DA " + maoAtual + "ª MÃO — VITÓRIA DE MARCUS")
            : ("FIM DA " + maoAtual + "ª MÃO — VITÓRIA DE ZÉ");

        string falaZe = (proximaMao == 2)
            ? "Zé passa o pano na beirada da mesa:\n<i>\"Você ainda lembra como segura o taco. Vamos ver na segunda mão, quando nem toda bola puder cair.\"</i>"
            : "O placar empata em 1 a 1. A chuva engrossa no telhado de zinco:\n<i>\"Última mão, Marcus. Quem vencer esta partida leva a conversa até o fim.\"</i>";

        MostrarModal(
            titulo,
            "Placar da Série: <b>MARCUS " + maosVencidasMarcus + " x " + maosVencidasZe + " ZÉ</b>\n\n" + falaZe,
            "IR PARA A " + proximaMao + "ª MÃO",
            () =>
            {
                painelModal.SetActive(false);
                ConfigurarMao(proximaMao);
                IniciarTurnoAtual();
            },
            null, null
        );
    }

    private void ConcluirSerieVitoria()
    {
        estado = EstadoPartida.FimDeSerieVitoria;
        if (Application.isPlaying) SantaRitaBarAudio.DefinirContexto(false, 0);
        MostrarModal(
            "VITÓRIA NA SÉRIE (" + maosVencidasMarcus + " A " + maosVencidasZe + ")",
            "Zé encosta o taco no suporte da parede e fica em silêncio por alguns segundos, ouvindo a chuva nos fundos do bar.\n\n" +
            "Ele tira do bolso do colete uma <b>Ficha Metálica do Clube</b> e a coloca sobre o feltro verde.",
            "OUVIR ZÉ",
            () =>
            {
                painelModal.SetActive(false);
                raiz.SetActive(false);
                estado = EstadoPartida.Inativo;
                var cb = onVictoryCallback;
                onVictoryCallback = null;
                cb?.Invoke();
            },
            null, null
        );
    }

    private void ConcluirSerieDerrota()
    {
        estado = EstadoPartida.FimDeSerieDerrota;
        if (Application.isPlaying) SantaRitaBarAudio.DefinirContexto(false, 0);
        bool temSaveManual = CheckpointSystem.HasCheckpoint;

        if (temSaveManual)
        {
            MostrarModal(
                "FIM DA SÉRIE (" + maosVencidasMarcus + " A " + maosVencidasZe + ") — ZÉ ENCERRA A CONVERSA",
                "Zé apaga a lâmpada sobre a mesa de sinuca:\n" +
                "<i>\"Você está com a cabeça em 1998, Marcus. Volte quando tiver firmeza para ouvir o que aconteceu hoje.\"</i>\n\n" +
                "Escolha qual <b>registro manual</b> deseja carregar, ou volte ao Menu Principal.",
                "ESCOLHER SAVE",
                () =>
                {
                    painelModal.SetActive(false);
                    PayphoneCheckpoint.AbrirSelecaoCarregar(ConcluirSerieDerrota, () =>
                    { raiz.SetActive(false); estado = EstadoPartida.Inativo; });
                },
                "MENU PRINCIPAL",
                () =>
                {
                    painelModal.SetActive(false);
                    raiz.SetActive(false);
                    estado = EstadoPartida.Inativo;
                    onDefeatCallback?.Invoke();
                    SceneFader.Instance.LoadScene("MainMenu");
                }
            );
        }
        else
        {
            MostrarModal(
                "FIM DA SÉRIE (" + maosVencidasMarcus + " A " + maosVencidasZe + ") — SEM SAVE MANUAL",
                "Zé apaga a lâmpada sobre a mesa de sinuca e fecha a conversa.\n\n" +
                "<b>Nenhum Orelhão foi utilizado para salvar manualmente nesta sessão.</b>\n" +
                "Escolha reiniciar a jornada ou retornar ao Menu Principal.",
                "NOVO JOGO",
                () =>
                {
                    painelModal.SetActive(false);
                    raiz.SetActive(false);
                    estado = EstadoPartida.Inativo;
                    CheckpointSystem.Clear();
                    GameState.Limpar();
                    SceneFader.Instance.LoadScene("Prologo");
                },
                "MENU PRINCIPAL",
                () =>
                {
                    painelModal.SetActive(false);
                    raiz.SetActive(false);
                    estado = EstadoPartida.Inativo;
                    onDefeatCallback?.Invoke();
                    SceneFader.Instance.LoadScene("MainMenu");
                }
            );
        }
    }

    private Vector2 ReporBrasileira(int number)
    {
        if(PosicaoValidaSemColisao(marcasBrasileiras[number],number))return marcasBrasileiras[number];
        for(int higher=15;higher>number;higher--)
            if(PosicaoValidaSemColisao(marcasBrasileiras[higher],number))return marcasBrasileiras[higher];
        return EncontrarPosicaoLivreDeterministica(new Vector2(-155,0),number);
    }

    private Vector2 EncontrarPosicaoLivreDeterministica(Vector2 preferida, int ignorarIndice)
    {
        float limX = TableWidth * 0.5f - BallRadius - 8f;
        float limY = TableHeight * 0.5f - BallRadius - 8f;

        preferida.x = Mathf.Clamp(preferida.x, -limX, limX);
        preferida.y = Mathf.Clamp(preferida.y, -limY, limY);

        if (PosicaoValidaSemColisao(preferida, ignorarIndice))
            return preferida;

        for (int passo = 1; passo <= 18; passo++)
        {
            float raio = passo * (BallRadius * 2.1f);
            for (int angIdx = 0; angIdx < 12; angIdx++)
            {
                float ang = angIdx * (Mathf.PI * 2f / 12f);
                Vector2 cand = preferida + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * raio;
                cand.x = Mathf.Clamp(cand.x, -limX, limX);
                cand.y = Mathf.Clamp(cand.y, -limY, limY);
                if (PosicaoValidaSemColisao(cand, ignorarIndice))
                    return cand;
            }
        }

        return Vector2.zero;
    }

    private bool PosicaoValidaSemColisao(Vector2 pos, int ignorarIndice)
    {
        for (int p = 0; p < cacapas.Length; p++)
        {
            if (Vector2.Distance(pos, cacapas[p]) < PocketRadius + BallRadius * 0.8f)
                return false;
        }

        for (int i = 0; i < bolas.Count; i++)
        {
            if (i == ignorarIndice || !bolas[i].ativa) continue;
            if (Vector2.Distance(pos, bolas[i].pos) < BallRadius * 2.08f)
                return false;
        }

        return true;
    }

    // =========================================================================
    // Guia de Mira Discreta (Mesmo Modelo Geométrico da Simulação)
    // =========================================================================

    public SinucaAimGuideData CalcularGuiaDeMira(Vector2 origemBranca, float anguloRad, SinucaCueSpin efeito)
    {
        SinucaAimGuideData guia = new SinucaAimGuideData
        {
            ativa = true,
            origem = origemBranca,
            pontoContato = origemBranca,
            atingeBola = false,
            numeroBolaAlvo = -1,
            alvoEhMarcada = false,
            posBolaAlvo = Vector2.zero,
            direcaoBolaAlvo = Vector2.zero,
            direcaoBrancaPosContato = Vector2.zero
        };

        Vector2 dir = new Vector2(Mathf.Cos(anguloRad), Mathf.Sin(anguloRad)).normalized;
        float menorT = float.MaxValue;
        int bolaAtingida = -1;

        // 1. Interseção contínua (swept circle de raio 2 * BallRadius) contra cada bola ativa
        float rSum = BallRadius * 2f;
        float rSumSq = rSum * rSum;

        for (int i = 1; i < bolas.Count; i++)
        {
            if (!bolas[i].ativa) continue;
            Vector2 toBall = bolas[i].pos - origemBranca;
            float proj = Vector2.Dot(toBall, dir);
            if (proj <= 0f) continue;

            float perpSq = toBall.sqrMagnitude - proj * proj;
            if (perpSq >= rSumSq) continue;

            float thc = Mathf.Sqrt(rSumSq - perpSq);
            float tHit = proj - thc;
            if (tHit >= 0f && tHit < menorT)
            {
                menorT = tHit;
                bolaAtingida = i;
            }
        }

        // 2. Interseção contra os limites das tabelas (±limX, ±limY)
        float limX = TableWidth * 0.5f - BallRadius;
        float limY = TableHeight * 0.5f - BallRadius;
        float tTabela = float.MaxValue;

        if (Mathf.Abs(dir.x) > 0.0001f)
        {
            float tx = ((dir.x > 0f ? limX : -limX) - origemBranca.x) / dir.x;
            if (tx > 0f && tx < tTabela) tTabela = tx;
        }
        if (Mathf.Abs(dir.y) > 0.0001f)
        {
            float ty = ((dir.y > 0f ? limY : -limY) - origemBranca.y) / dir.y;
            if (ty > 0f && ty < tTabela) tTabela = ty;
        }

        if (bolaAtingida != -1 && menorT <= tTabela)
        {
            Vector2 ghostCenter = origemBranca + dir * menorT;
            Vector2 posAlvo = bolas[bolaAtingida].pos;
            Vector2 normalImpacto = (posAlvo - ghostCenter).normalized;

            guia.pontoContato = ghostCenter;
            guia.atingeBola = true;
            guia.numeroBolaAlvo = bolaAtingida;
            guia.alvoEhMarcada = (bolaAtingida == numeroBolaMarcada);
            guia.posBolaAlvo = posAlvo;
            guia.direcaoBolaAlvo = normalImpacto;

            // Tendência curta da branca após o impacto (tangente + influência longitudinal do efeito opcional)
            Vector2 tangente = dir - normalImpacto * Vector2.Dot(dir, normalImpacto);
            Vector2 biasSpin = efeito == SinucaCueSpin.Avancar ? normalImpacto * 0.65f :
                               efeito == SinucaCueSpin.Recuar ? -normalImpacto * 0.72f : Vector2.zero;
            Vector2 tendencia = tangente + biasSpin;
            guia.direcaoBrancaPosContato = tendencia.sqrMagnitude > 0.001f ? tendencia.normalized : dir;
        }
        else
        {
            float distFinal = Mathf.Min(tTabela, 500f);
            guia.pontoContato = origemBranca + dir * distFinal;
        }

        return guia;
    }

    private void SincronizarTransformBolas()
    {
        for (int i = 0; i < bolas.Count; i++)
        {
            var b = bolas[i];
            if (b.rect != null)
            {
                b.rect.gameObject.SetActive(b.ativa);
                b.rect.anchoredPosition = b.pos;
            }
        }
    }

    private void AtualizarVisualTacoEMira()
    {
        bool mostrarMira = (estado == EstadoPartida.MirandoJogador || estado == EstadoPartida.CarregandoForca) && bolas.Count > 0 && bolas[0].ativa;
        if (linhaMiraRect != null) linhaMiraRect.gameObject.SetActive(mostrarMira);
        if (tacoRect != null) tacoRect.gameObject.SetActive(mostrarMira);

        if (mostrarMira && linhaMiraRect != null && tacoRect != null)
        {
            float graus = anguloMiraRad * Mathf.Rad2Deg;
            linhaMiraRect.anchoredPosition = bolas[0].pos;
            linhaMiraRect.localEulerAngles = new Vector3(0f, 0f, graus);

            Vector2 dir = new Vector2(Mathf.Cos(anguloMiraRad), Mathf.Sin(anguloMiraRad));
            float recuo = 12f + forcaNormalizada * 26f;
            tacoRect.anchoredPosition = bolas[0].pos - dir * recuo;
            tacoRect.localEulerAngles = new Vector3(0f, 0f, graus);
        }

        SincronizarMesa3D();
    }

    private void SincronizarMesa3D()
    {
        if (tableView3D == null || bolas.Count == 0) return;

        bufferVisuais.Clear();
        for (int i = 0; i < bolas.Count; i++)
        {
            BolaMesa b = bolas[i];
            bufferVisuais.Add(new SinucaVisualBall
            {
                numero = b.numero,
                pos = b.pos,
                vel = b.vel,
                ativa = b.ativa,
                caindo = b.caindoNaCacapa,
                quedaProgresso = b.progressoQueda,
                cacapaAlvo = b.cacapaAlvo,
                rotacao = b.rotacao,
                marcada = (i > 0 && i == numeroBolaMarcada)
            });
        }

        bool mostrarMira = (estado == EstadoPartida.MirandoJogador || estado == EstadoPartida.CarregandoForca) && bolas[0].ativa;
        SinucaAimGuideData guia = mostrarMira
            ? CalcularGuiaDeMira(bolas[0].pos, anguloMiraRad, efeitoEscolhido)
            : new SinucaAimGuideData { ativa = false };

        miraAmeacaBolaMarcada = mostrarMira && guia.atingeBola && guia.alvoEhMarcada;

        // Encenação psicológica contextual: se Marcus aponta para a Bola Marcada [X], Zé para o gesto de giz e observa em silêncio
        if (mostrarMira && !modoPratica && maoAtual >= 2 && txtPercepcaoSubtitulo != null)
        {
            if (miraAmeacaBolaMarcada)
                txtPercepcaoSubtitulo.text = $"[Zé para de passar giz no taco. \"A bola {numeroBolaMarcada} tem marca, Marcus. Essa fica na mesa.\"]";
            else if (maoAtual == 2)
                txtPercepcaoSubtitulo.text = "[O murmúrio do bar cessa. Ouve-se apenas a goteira no balde e o giz azul riscando o taco.]";
            else if (maoAtual == 3)
                txtPercepcaoSubtitulo.text = "[03:17 no relógio parado atrás do balcão. A chuva bate forte na porta trancada dos fundos.]";
        }

        bool brancaValida = estado != EstadoPartida.PosicionandoBranca || PosicaoValidaSemColisao(bolas[0].pos, 0);

        tableView3D.AtualizarEstadoRender(
            bufferVisuais,
            cacapas,
            guia,
            mostrarMira,
            bolas[0].pos,
            anguloMiraRad,
            forcaNormalizada,
            efeitoEscolhido,
            estado == EstadoPartida.PosicionandoBranca,
            brancaValida,
            modoPratica ? 1 : maoAtual,
            numeroBolaMarcada,
            MenuPausa.ReduzirEfeitosIntensos,
            multiplicadorLuzAmbiente,
            zePassandoGizNoTurno,
            miraAmeacaBolaMarcada);
    }

    private void AtualizarTextosHUD()
    {
        if (txtPlacarSerie != null)
        {
            txtPlacarSerie.text = modoPratica
                ? "PRÁTICA LIVRE — BAR DO ZÉ"
                : $"SÉRIE MELHOR DE 3 — MARCUS [{maosVencidasMarcus}] x [{maosVencidasZe}] ZÉ  (Mão {maoAtual}/3)";
        }

        if (txtPlacarMao != null)
        {
            if (modoPratica)
            {
                int rest = 0;
                for (int i = 1; i <= 15; i++) if (bolas[i].ativa) rest++;
                txtPlacarMao.text = $"TREINO LIVRE: {rest} bola(s) na mesa  |  Teste mira, força e vista (V)";
            }
            else
            {
                txtPlacarMao.text = $"MARCUS {pontosMarcus}  |  ZÉ {pontosZe}  |  PRETA 8 POR ÚLTIMO";
            }
        }

        if (txtRegraBolaMarcada != null)
        {
            if (modoPratica)
                txtRegraBolaMarcada.text = "Prática Livre: Quando estiver pronto, clique em INICIAR DUELO para enfrentar Zé.";
            else if (numeroBolaMarcada > 0 && bolas[numeroBolaMarcada].ativa)
                txtRegraBolaMarcada.text = $"REGRA ATIVA — \"ESSA FICA NA MESA\": Bola [{numeroBolaMarcada}X] NÃO pode ser encaçapada!";
            else if (maoAtual >= 2)
                txtRegraBolaMarcada.text = "REGRA \"ESSA FICA NA MESA\": Desativada (menos de 2 bolas restantes).";
            else
                txtRegraBolaMarcada.text = "SINUQUINHA · 15 BOLAS · PRETA 8 POR ÚLTIMO";
        }

        if(!modoPratica&&regrasBrasileiras!=null&&txtRegraBolaMarcada!=null)
        {
            string sequence=regrasBrasileiras.Opening?"SAÍDA · branca pronta para tacar":regrasBrasileiras.MarcusGroup==0?"MESA ABERTA · grupos definidos na primeira conversão válida":(gruposPorNumero?"SEU GRUPO: "+(regrasBrasileiras.MarcusGroup==1?"1–7":"9–15"):"SEU GRUPO: "+(regrasBrasileiras.MarcusGroup==1?"LISAS":"LISTRADAS"))+" · PRETA 8 POR ÚLTIMO";
            txtRegraBolaMarcada.text=sequence+(numeroBolaMarcada>0?" • MARCADA: "+numeroBolaMarcada:"");
        }

        if (txtBtnAcaoPraticaOuDuelo != null)
        {
            txtBtnAcaoPraticaOuDuelo.text = modoPratica ? "INICIAR DUELO" : "PRÁTICA LIVRE";
        }

        if(txtEncaçapadasMarcus!=null&&regrasBrasileiras!=null)
        {
            txtEncaçapadasMarcus.text="MARCUS · ENCAÇAPADAS"+(regrasBrasileiras.MarcusGroup==0?" —":"");
            txtEncaçapadasZe.text="ZÉ · ENCAÇAPADAS"+(regrasBrasileiras.MarcusGroup==0?" —":"");
            int left=0,right=0;for(int n=1;n<16;n++)if(n!=8){bool show=!modoPratica&&!regrasBrasileiras.Present[n]&&regrasBrasileiras.MarcusGroup!=0;encaçapadasIcones[n].gameObject.SetActive(show);if(show){bool marcus=SinuquinhaRules.Group(n)==regrasBrasileiras.MarcusGroup;int j=marcus?left++:right++;Posicionar(encaçapadasIcones[n].rectTransform,new Vector2((marcus?-298:42)+j*16,78),new Vector2(13,13));}}
        }
        AtualizarBotaoVistaCamera();
    }

    // =========================================================================
    // Síntese e Disparo de Áudio por Evento Real (Tacada, Colisão, Tabela, Caçapa, Giz)
    // =========================================================================

    private static void GarantirClipsAudio()
    {
        if (clipTacada != null) return;
        const int hz = 22050;

        // 1. Impacto seco da sola do taco na bola branca
        {
            var data = new float[(int)(hz * 0.09f)];
            var rng = new System.Random(1998);
            for (int i = 0; i < data.Length; i++)
            {
                float t = i / (float)hz;
                float env = Mathf.Exp(-t * 68f);
                float tone = Mathf.Sin(2f * Mathf.PI * 520f * t) * 0.55f + Mathf.Sin(2f * Mathf.PI * 1180f * t) * 0.25f;
                float noise = ((float)rng.NextDouble() * 2f - 1f) * 0.35f;
                data[i] = Mathf.Clamp((tone + noise) * env, -0.9f, 0.9f);
            }
            clipTacada = AudioClip.Create("SR_Sinuca_Tacada", data.Length, 1, hz, false);
            clipTacada.SetData(data, 0);
        }

        // 2. Estalo cristalino bola-bola (resina fenólica)
        {
            var data = new float[(int)(hz * 0.075f)];
            for (int i = 0; i < data.Length; i++)
            {
                float t = i / (float)hz;
                float env = Mathf.Exp(-t * 85f);
                float click = Mathf.Sin(2f * Mathf.PI * 1680f * t) * 0.62f + Mathf.Sin(2f * Mathf.PI * 2840f * t) * 0.32f;
                data[i] = Mathf.Clamp(click * env, -0.9f, 0.9f);
            }
            clipColisaoBola = AudioClip.Create("SR_Sinuca_ColisaoBola", data.Length, 1, hz, false);
            clipColisaoBola.SetData(data, 0);
        }

        // 3. Batida abafada na tabela de borracha/madeira
        {
            var data = new float[(int)(hz * 0.11f)];
            var rng = new System.Random(2004);
            for (int i = 0; i < data.Length; i++)
            {
                float t = i / (float)hz;
                float env = Mathf.Exp(-t * 42f);
                float thud = Mathf.Sin(2f * Mathf.PI * 145f * t) * 0.72f + ((float)rng.NextDouble() * 2f - 1f) * 0.18f;
                data[i] = Mathf.Clamp(thud * env, -0.85f, 0.85f);
            }
            clipTabela = AudioClip.Create("SR_Sinuca_Tabela", data.Length, 1, hz, false);
            clipTabela.SetData(data, 0);
        }

        // 4. Queda da bola no cesto de couro da caçapa
        {
            var data = new float[(int)(hz * 0.22f)];
            var rng = new System.Random(317);
            for (int i = 0; i < data.Length; i++)
            {
                float t = i / (float)hz;
                float env = t < 0.06f ? Mathf.Exp(-t * 38f) : Mathf.Exp(-(t - 0.06f) * 26f) * 0.75f;
                float drop = Mathf.Sin(2f * Mathf.PI * (t < 0.06f ? 260f : 185f) * t) * 0.65f + ((float)rng.NextDouble() * 2f - 1f) * 0.22f;
                data[i] = Mathf.Clamp(drop * env, -0.85f, 0.85f);
            }
            clipCacapa = AudioClip.Create("SR_Sinuca_Cacapa", data.Length, 1, hz, false);
            clipCacapa.SetData(data, 0);
        }

        // 5. Fricção curta do giz azul na ponteira do taco
        {
            var data = new float[(int)(hz * 0.16f)];
            var rng = new System.Random(1990);
            float filt = 0f;
            for (int i = 0; i < data.Length; i++)
            {
                float t = i / (float)hz;
                float n = (float)rng.NextDouble() * 2f - 1f;
                filt = Mathf.Lerp(filt, n, 0.32f);
                float env = Mathf.Sin(Mathf.Clamp01(t / 0.16f) * Mathf.PI);
                data[i] = Mathf.Clamp((n - filt) * 0.45f * env, -0.6f, 0.6f);
            }
            clipGiz = AudioClip.Create("SR_Sinuca_Giz", data.Length, 1, hz, false);
            clipGiz.SetData(data, 0);
        }
    }

    private void TocarClipSintetico(AudioClip clip, float volumeBase, float pitch = 1f)
    {
        EventosAudioDisparados++;
        if (!Application.isPlaying || Time.timeScale == 0f || MenuPausa.Aberto) return;
        float volPref = PlayerPrefs.GetFloat("audio_efeitos", 1f);
        if (volPref <= 0.001f || clip == null) return;

        AudioSource src = GetComponent<AudioSource>();
        if (src == null)
        {
            src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 0f;
        }
        src.pitch = Mathf.Clamp(pitch, 0.85f, 1.18f);
        src.PlayOneShot(clip, Mathf.Clamp01(volumeBase * volPref));
    }

    private void TocarSomTacada(float intensidade01)
    {
        GarantirClipsAudio();
        float v = Mathf.Lerp(0.18f, 0.42f, Mathf.Clamp01(intensidade01));
        if (Application.isPlaying) SantaRitaSound.Play("sinuca", v * 0.65f, Mathf.Lerp(0.94f, 1.06f, intensidade01), 0.03f);
        TocarClipSintetico(clipTacada, v, Mathf.Lerp(0.95f, 1.08f, intensidade01));
    }

    private void TocarSomColisaoBola(float intensidade01)
    {
        GarantirClipsAudio();
        float v = Mathf.Lerp(0.12f, 0.44f, Mathf.Clamp01(intensidade01));
        if (Application.isPlaying) SantaRitaSound.Play("sinuca", v * 0.85f, Mathf.Lerp(0.96f, 1.12f, intensidade01), 0.025f);
        TocarClipSintetico(clipColisaoBola, v, Mathf.Lerp(0.96f, 1.12f, intensidade01));
    }

    private void TocarSomTabela(float intensidade01)
    {
        GarantirClipsAudio();
        float v = Mathf.Lerp(0.10f, 0.30f, Mathf.Clamp01(intensidade01));
        TocarClipSintetico(clipTabela, v, Mathf.Lerp(0.92f, 1.04f, intensidade01));
    }

    private void TocarSomCacapa()
    {
        GarantirClipsAudio();
        TocarClipSintetico(clipCacapa, 0.34f, 0.98f);
    }

    private void TocarSomGiz(float volume = 0.16f)
    {
        GarantirClipsAudio();
        TocarClipSintetico(clipGiz, volume, 1.0f);
    }

    private void MostrarModal(string titulo, string corpo, string txtPrimario, UnityEngine.Events.UnityAction acaoPrimaria,
        string txtSecundario, UnityEngine.Events.UnityAction acaoSecundaria)
    {
        painelModal.SetActive(true);
        txtModalTitulo.text = titulo;
        txtModalCorpo.text = corpo;

        btnModalPrimario.onClick.RemoveAllListeners();
        txtBtnPrimario.text = txtPrimario;
        btnModalPrimario.onClick.AddListener(acaoPrimaria);

        if (!string.IsNullOrEmpty(txtSecundario) && acaoSecundaria != null)
        {
            btnModalSecundario.gameObject.SetActive(true);
            btnModalPrimario.GetComponent<RectTransform>().anchoredPosition = new Vector2(-95f, -96f);
            txtBtnSecundario.text = txtSecundario;
            btnModalSecundario.onClick.RemoveAllListeners();
            btnModalSecundario.onClick.AddListener(acaoSecundaria);
        }
        else
        {
            btnModalSecundario.gameObject.SetActive(false);
            btnModalPrimario.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -96f);
        }
    }

    private Sprite CriarSpriteCirculo(int size, Color fill, Color border)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        float r = size * 0.5f - 1f;
        Vector2 c = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), c);
                if (d > r) tex.SetPixel(x, y, Color.clear);
                else if (d > r - 2.1f && border.a > 0.01f) tex.SetPixel(x, y, border);
                else tex.SetPixel(x, y, fill);
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f);
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

#if UNITY_EDITOR || DEBUG
    public bool SimularTesteDeInvariantes(out string relatorio)
    {
        // Public compatibility seam: the current house pool rules supersede seven-ball scoring.
        var r=new SinuquinhaRules(true);
        r.Resolve(1,1,new List<int>{1},false,-1);
        if(r.MarcusGroup!=1||!r.MarcusTurn||r.Present[1]){relatorio="Grupo/continuidade inválido";return false;}
        var foul=r.Resolve(9,9,new List<int>(),false,-1);
        if(!foul.foul||!foul.cueInHand||r.MarcusTurn){relatorio="Falta/branca na mão inválida";return false;}
        relatorio="Sinuquinha: quinze bolas, grupo definido por conversão e falta com branca na mão. Física/câmeras na suíte Sinuquinha15.";
        return true;
    }

#endif
}

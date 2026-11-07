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
public partial class SinucaMinigameUI : MonoBehaviour
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
    // Match the 16 cm rendered mouth; capture radius is clearance for the ball centre.
    public const float PocketOpeningRadius = .080f / (2.84f / TableWidth);
    public const float PocketRadius = PocketOpeningRadius - BallRadius;
    public const float FixedStep = 1f / 120f;
    private const float CushionRestitution = 0.74f;
    private const float BallRestitution = 0.88f;
    // Equal solid spheres: I = 2/5 mr². Values use the rendered table's metre scale.
    private const float BallMassKg = 0.170f;
    private const float MetresPerPhysicsUnit = 2.84f / TableWidth;
    private const float SlidingFriction = 0.18f * 9.81f / MetresPerPhysicsUnit;
    private const float BallFriction = 0.026f * 9.81f / MetresPerPhysicsUnit;
    private const float StopSpeedSq = 0.16f;
    private const float MaxShotPower = 0.60f / (BallMassKg * MetresPerPhysicsUnit);
    private const float MinShotPower = 0.018f / (BallMassKg * MetresPerPhysicsUnit);

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
        public float spinLongitudinal; // Initial cue spin, expressed as surface speed.
        public Vector2 rolagem; // rω projected into the cloth plane; preserved through ball impacts.
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
    private TextMeshProUGUI txtFalaZe;
    private CanvasGroup legendaZe;
    private AudioSource vozFalaZe;
    private AudioClip[] silabasZe;
    private readonly SinucaZeComments comentariosZe = new SinucaZeComments();
    private string falaZePendente;
    private bool falaZePendenteUrgente;
    private float tempoLetraZe, tempoLeituraZe, intervaloSilabaZe, intervaloFalaZe;
    private int caractereZe, silabaZe;
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

    private GameObject painelOpcoes, grupoForca, escudoModal;
    private Button btnOpcoes, btnAjuda, btnCancelarTacada;
    private TextMeshProUGUI txtForcaPercentual;
    private int paginaAjuda;
    private bool ajudaInicialPendente;
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
    private float alvoMiraMouseRad, ultimoAnguloMouseRad, offsetMiraMouseRad;
    private bool miraMouseAtiva, referenciaMiraMouse;
    private float forcaNormalizada = 0f;
    private Vector2 inicioPuxadaUI, direcaoPuxadaUI;
    private bool puxadaValida;
    private readonly AudioSource[] vozesSinuca = new AudioSource[6];
    private readonly Dictionary<AudioClip, float> ultimoImpacto = new Dictionary<AudioClip, float>();
    private int proximaVoz;
    private bool audioSuspenso;
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

    // Terror psicológico contextual do Bar do Zé e áudio por evento
    private float multiplicadorLuzAmbiente = 1f;
    private float timerOscilacaoLuz = 0f;
    private bool zePassandoGizNoTurno = true;
    private bool miraAmeacaBolaMarcada = false;
    private float ultimoSomGiz = -10f;
    private static AudioClip clipTacada;
    private static AudioClip[] clipsColisaoBola;
    private int proximaColisaoBola;
    private float ultimoSomColisaoBola = -10f;
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
        ReiniciarPercepcoesSinuca();
        StopAllCoroutines();
        if(tableView3D!=null)tableView3D.Room?.CancelOpponentShot();
        LimparFalaZe();
        RestaurarCursor();
        CancelarCargaAtiva();
        estado = EstadoPartida.Inativo;
        modoPratica = false;
        if (Application.isPlaying) SantaRitaBarAudio.DefinirContexto(false, 0);
        foreach (var voz in vozesSinuca) if (voz != null) voz.Stop();
        if (raiz != null) raiz.SetActive(false);
    }

    public void CancelarCargaAoSuspender()
    {
        EncerrarAjusteEfeito();
        miraMouseAtiva = referenciaMiraMouse = false;
        if (tableView3D != null) tableView3D.ReiniciarReferenciaPonteiro();
        if (estado == EstadoPartida.CarregandoForca)
        {
            CancelarCargaAtiva();
            if (txtTurnoStatus != null)
                txtTurnoStatus.text = "Tacada cancelada. Mire, clique e puxe para trás. Solte para tacar.";
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && IsShowing)
        {
            interfaceLivre=true;Cursor.lockState=CursorLockMode.None;Cursor.visible=true;
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
        GarantirColisoesGravadas();
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
        // Rounded tips sit outside the drawn opening, rather than narrowing its centre.
        float expandedMouth = PocketOpeningRadius + jawR;
        float cOffset = 4f + Mathf.Sqrt(expandedMouth * expandedMouth - 4f * 4f);
        float sOffset = Mathf.Sqrt(expandedMouth * expandedMouth - 2f * 2f);

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

    private float levantarAposTacada = -1f;
    private float eixoMiraApoiada;
    private bool interfaceLivre, cursorGuardado;
    private CursorLockMode cursorAnterior;
    private bool cursorAnteriorVisivel;
    private Vector2 puxadaAcumulada;
    private int framePuxada=-1, frameInicioCarga=-1, frameCaptura=-1;
    private TextMeshProUGUI reticuloFPS;

    private void OnDisable(){EncerrarAjusteEfeito();if(tableView3D!=null)tableView3D.Room?.CancelOpponentShot();RestaurarCursor();LimparFalaZe();ReiniciarPercepcoesSinuca();}
    private void OnDestroy()
    {
        RestaurarCursor();LimparFalaZe();
        DestruirAudioPercepcao();
        if(silabasZe!=null)foreach(var clip in silabasZe)if(clip!=null)Destroy(clip);
    }

    private void RestaurarCursor()
    {
        if(!cursorGuardado)return;
        Cursor.lockState=cursorAnterior;Cursor.visible=cursorAnteriorVisivel;cursorGuardado=false;
    }
    private void AtualizarCapturaCursor()
    {
        if(!cursorGuardado){cursorAnterior=Cursor.lockState;cursorAnteriorVisivel=Cursor.visible;cursorGuardado=true;}
        bool capturar=!interfaceLivre && Application.isFocused && !EntradaMesaBloqueada(true);
        var mode=capturar?CursorLockMode.Locked:CursorLockMode.None;
        if(Cursor.lockState!=mode){Cursor.lockState=mode;if(capturar)frameCaptura=Time.frameCount;}
        Cursor.visible=!capturar;
        if(reticuloFPS!=null)reticuloFPS.gameObject.SetActive(capturar && EmPe);
    }
    private Vector2 CentroDaMesaNaTela()
        => RectTransformUtility.WorldToScreenPoint(null,mesaRect.TransformPoint(mesaRect.rect.center));

    private void LerMouseCapturado()
    {
        var mouse=Mouse.current;if(Time.frameCount==frameCaptura)return;if(mouse==null || Cursor.lockState!=CursorLockMode.Locked || EntradaMesaBloqueada())return;
        if(mouse.rightButton.wasPressedThisFrame && !EmPe)
        {
            OnMesaPointerDown(Vector2.zero,PointerEventData.InputButton.Right);
            return;
        }
        if(ajustandoEfeito)
        {
            if(!mouse.rightButton.isPressed)EncerrarAjusteEfeito();
            else ArrastarEfeito(mouse.delta.ReadValue());
            return;
        }
        if(mouse.leftButton.wasPressedThisFrame)
        {
            if(EmPe)
            {
                if(estado==EstadoPartida.MirandoJogador && tableView3D.Room.RayHitsWhite(new Vector2(.5f,.5f),bolas[0].pos))OnMesaPointerDown(bolas[0].pos,PointerEventData.InputButton.Left);
                else if(estado==EstadoPartida.PosicionandoBranca && tableView3D.ScreenToTablePoint(CentroDaMesaNaTela(),null,out var p))OnMesaPointerDown(p,PointerEventData.InputButton.Left);
            }
            else OnMesaPointerDown(Vector2.zero,PointerEventData.InputButton.Left);
        }
        else if(!EmPe && estado==EstadoPartida.MirandoJogador)
            tableView3D.AoAjustarMira?.Invoke(mouse.delta.ReadValue().x / Mathf.Max(.01f,mesaRect.GetComponentInParent<Canvas>().scaleFactor)*.003f);
        if(mouse.leftButton.wasReleasedThisFrame)OnMesaPointerUp(Vector2.zero,true,PointerEventData.InputButton.Left);
    }

    private RectTransform headerMesa, footerMesa;
    private bool EmPe => tableView3D != null && tableView3D.CameraModo == SinucaCameraView.Caminhando3D;

    private void ConstruirUI()
    {
        Canvas canvas = UIFactory.CreateCanvas(transform, "SinucaCanvas", 6500);
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        canvas.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        raiz = new GameObject("RaizSinuca", typeof(RectTransform));
        raiz.transform.SetParent(canvas.transform, false);
        UIFactory.Stretch(raiz.GetComponent<RectTransform>());

        var fundo = CriarPainelHUD(raiz.transform, "FundoEscuroBar", Vector2.zero, new Vector2(640,360));
        fundo.GetComponent<Image>().color = new Color32(12,10,8,255);
        UIFactory.Stretch(fundo.GetComponent<RectTransform>());

        var viewport = new GameObject("MesaSinuca3DViewport", typeof(RectTransform), typeof(CanvasRenderer), typeof(SinucaTableView));
        viewport.transform.SetParent(raiz.transform, false);
        mesaRect = viewport.GetComponent<RectTransform>();
        UIFactory.Stretch(mesaRect);
        tableView3D = viewport.GetComponent<SinucaTableView>();
        tableView3D.color = Color.white;
        tableView3D.AoPressionarMesa = OnMesaPointerDown;
        tableView3D.AoMoverMesa = OnMesaPointerMove;
        tableView3D.AoAjustarMira = delta => { if (estado == EstadoPartida.MirandoJogador && !ajustandoEfeito && !interfaceLivre && !EntradaMesaBloqueada()) { if(!tableView3D.Room.SettledAtCue)return; miraMouseAtiva=referenciaMiraMouse=false; anguloMiraRad += delta*(Keyboard.current!=null&&Keyboard.current.shiftKey.isPressed?.15f:1f); LimitarMiraApoiada(); AtualizarVisualTacoEMira(); } };
        tableView3D.AoSoltarMesa = OnMesaPointerUp;
        tableView3D.AoScrollZoom = OnMesaScrollZoom;

        var legado = new GameObject("ContainerLegadoCompat", typeof(RectTransform));
        legado.transform.SetParent(mesaRect,false);
        legado.SetActive(false);
        var mira = new GameObject("LinhaMira", typeof(RectTransform), typeof(Image));
        mira.transform.SetParent(legado.transform,false); linhaMiraRect = mira.GetComponent<RectTransform>();
        var taco = new GameObject("TacoSinuca", typeof(RectTransform), typeof(Image));
        taco.transform.SetParent(legado.transform,false); tacoRect = taco.GetComponent<RectTransform>();
        CriarBolasNaMesa(legado.transform, UIFactory.Font);

        var percepcao = CriarPainelHUD(raiz.transform,"OverlayPercepcao",Vector2.zero,new Vector2(640,360));
        UIFactory.Stretch(percepcao.GetComponent<RectTransform>());
        overlayPercepcao = percepcao.GetComponent<Image>();
        overlayPercepcao.color = Color.clear; overlayPercepcao.raycastTarget = false;

        var header = CriarPainelHUD(raiz.transform,"HeaderHUD",new Vector2(0,165),new Vector2(624,26));
        txtPlacarSerie = CriarTexto("TxtPlacarSerie",header.transform,"MARCUS 0  :  0 ZÉ",10,new Color32(242,220,170,255),TextAlignmentOptions.Left);
        Posicionar(txtPlacarSerie.rectTransform,new Vector2(-193,0),new Vector2(214,18));
        txtRegraBolaMarcada = CriarTexto("TxtRegraBolaMarcada",header.transform,"Mesa aberta · preta 8 por último",8.2f,new Color32(207,207,188,255),TextAlignmentOptions.Left);
        Posicionar(txtRegraBolaMarcada.rectTransform,new Vector2(-16,0),new Vector2(132,18));
        txtRegraBolaMarcada.overflowMode = TextOverflowModes.Ellipsis;
        txtPlacarMao = CriarTexto("TxtPlacarMao",legado.transform,"",10,Color.white,TextAlignmentOptions.Left);
        btnVistaCamera = CriarBotaoHUD(header.transform,"BtnVistaCamera","Sala",new Vector2(91,0),new Vector2(62,20),new Color32(44,64,54,255),out txtBtnVistaCamera);
        btnVistaCamera.onClick.AddListener(AlternarVistaCamera);
        btnOpcoes = CriarBotaoHUD(header.transform,"BtnOpcoesMesa","Opções",new Vector2(157,0),new Vector2(60,20),new Color32(43,45,41,255),out _);
        btnOpcoes.onClick.AddListener(AlternarOpcoesMesa);
        btnAjuda = CriarBotaoHUD(header.transform,"BtnVerRegras","Ajuda",new Vector2(221,0),new Vector2(60,20),new Color32(63,51,33,255),out _);
        btnAjuda.onClick.AddListener(AbrirModalRegrasDuranteJogo);
        var btnPausa=CriarBotaoHUD(header.transform,"BtnPausaSinuca","Pausa",new Vector2(279,0),new Vector2(48,20),new Color32(35,39,32,85),out _);
        btnPausa.GetComponent<Outline>().effectColor=new Color32(133,121,91,55);btnPausa.GetComponentInChildren<TextMeshProUGUI>().fontSize=9;
        btnPausa.onClick.AddListener(()=>{interfaceLivre=true;var pausa=FindAnyObjectByType<MenuPausa>();if(pausa==null)pausa=gameObject.AddComponent<MenuPausa>();pausa.Abrir();AtualizarCapturaCursor();});

        var footer = CriarPainelHUD(raiz.transform,"FooterHUD",new Vector2(0,-164),new Vector2(624,28));
        txtTurnoStatus = CriarTexto("TxtTurnoStatus",footer.transform,"Aproxime-se da branca.",9,new Color32(224,234,219,255),TextAlignmentOptions.Left);
        Posicionar(txtTurnoStatus.rectTransform,new Vector2(0,7),new Vector2(596,12));
        txtTurnoStatus.overflowMode = TextOverflowModes.Ellipsis;
        txtControlesRodape = CriarTexto("TxtControlesRodape",footer.transform,"WASD: caminhar · Mouse: olhar · Esc: cursor",8.3f,new Color32(173,184,172,255),TextAlignmentOptions.Left);
        Posicionar(txtControlesRodape.rectTransform,new Vector2(0,-6),new Vector2(596,12));
        grupoForca = new GameObject("ForcaContextual",typeof(RectTransform));
        grupoForca.transform.SetParent(footer.transform,false);
        Posicionar(grupoForca.GetComponent<RectTransform>(),new Vector2(104,0),new Vector2(124,24));
        txtForcaPercentual = CriarTexto("ForcaPercentual",grupoForca.transform,"Força 0%",9,new Color32(235,204,139,255),TextAlignmentOptions.Center);
        Posicionar(txtForcaPercentual.rectTransform,new Vector2(0,7),new Vector2(124,12));
        var barra = CriarPainelHUD(grupoForca.transform,"BarraForcaBg",new Vector2(0,-5),new Vector2(124,7));
        barra.GetComponent<Image>().color = new Color32(55,55,43,255);
        var fill = new GameObject("BarraForcaFill",typeof(RectTransform),typeof(Image));
        fill.transform.SetParent(barra.transform,false); barraForcaFill = fill.GetComponent<Image>();
        AtualizarBarraForcaVisual(0);
        btnCancelarTacada = CriarBotaoHUD(footer.transform,"BtnCancelarTacada","Cancelar",new Vector2(248,0),new Vector2(96,20),new Color32(74,43,34,255),out _);
        btnCancelarTacada.onClick.AddListener(()=>{CancelarCargaAtiva();txtTurnoStatus.text="Tacada cancelada. Ajuste a mira para tentar de novo.";});
        grupoForca.SetActive(false); btnCancelarTacada.gameObject.SetActive(false);

        headerMesa=header.GetComponent<RectTransform>();footerMesa=footer.GetComponent<RectTransform>();
        headerMesa.anchorMin=headerMesa.anchorMax=new Vector2(.5f,1);headerMesa.anchoredPosition=new Vector2(0,-15);
        footerMesa.anchorMin=footerMesa.anchorMax=new Vector2(.5f,0);footerMesa.anchoredPosition=new Vector2(0,16);
        header.GetComponent<Image>().color=new Color32(15,17,14,150);
        footer.GetComponent<Image>().color=new Color32(15,17,14,145);
        foreach(var panel in new[]{header,footer}) { var outline=panel.GetComponent<Outline>();if(outline!=null)outline.enabled=false; }
        foreach(var button in new[]{btnVistaCamera,btnOpcoes,btnAjuda,btnCancelarTacada})
        {button.GetComponent<Image>().color=new Color32(35,39,32,85);button.GetComponent<Outline>().effectColor=new Color32(133,121,91,55);button.GetComponentInChildren<TextMeshProUGUI>().fontSize=9;}
        reticuloFPS=CriarTexto("ReticuloFPS",raiz.transform,"·",12,new Color32(228,221,192,120),TextAlignmentOptions.Center);
        Posicionar(reticuloFPS.rectTransform,Vector2.zero,new Vector2(12,12));reticuloFPS.raycastTarget=false;
        // Atmosphere shares the same context line; no floating instructional labels on the table.
        txtPercepcaoSubtitulo = CriarTexto("TxtPercepcaoSubtitulo",legado.transform,"",9,Color.white,TextAlignmentOptions.Left);
        ConstruirLegendaZe();
        ConstruirOpcoesMesa();
        ConstruirAjusteEfeito();
        ConstruirEscolhaAlvo();
        ConstruirModal(raiz.transform);
        AtualizarPainelContextual();
    }

    private GameObject CriarPainelHUD(Transform pai,string nome,Vector2 pos,Vector2 tamanho)
    {
        var obj = new GameObject(nome,typeof(RectTransform),typeof(Image));
        obj.transform.SetParent(pai,false);
        Posicionar(obj.GetComponent<RectTransform>(),pos,tamanho);
        obj.GetComponent<Image>().color = new Color32(22,24,21,246);
        return obj;
    }

    private void ConstruirOpcoesMesa()
    {
        painelOpcoes = CriarPainelHUD(raiz.transform,"OpcoesMesa",Vector2.zero,new Vector2(640,360));
        UIFactory.Stretch(painelOpcoes.GetComponent<RectTransform>());
        painelOpcoes.GetComponent<Image>().color = new Color(0,0,0,.5f);
        painelOpcoes.AddComponent<Button>().onClick.AddListener(()=>painelOpcoes.SetActive(false));
        var card = CriarPainelHUD(painelOpcoes.transform,"AjustesMesa",new Vector2(132,0),new Vector2(344,294));
        card.AddComponent<Button>().navigation = new Navigation { mode = Navigation.Mode.None };
        var titulo = CriarTexto("TituloOpcoes",card.transform,"OPÇÕES DA MESA",13,new Color32(242,220,170,255),TextAlignmentOptions.Left);
        Posicionar(titulo.rectTransform,new Vector2(-38,122),new Vector2(236,22));
        var fechar = CriarBotaoHUD(card.transform,"FecharOpcoes","Fechar",new Vector2(122,122),new Vector2(72,25),new Color32(48,51,43,255),out _);
        fechar.onClick.AddListener(()=>painelOpcoes.SetActive(false));
        var instrucao = CriarTexto("AjudaContato",card.transform,"EFEITO OPCIONAL · toque no ponto da branca",9,new Color32(173,184,172,255),TextAlignmentOptions.Left);
        Posicionar(instrucao.rectTransform,new Vector2(0,96),new Vector2(312,16));
        var spin = new GameObject("PontoDeContatoBranca",typeof(RectTransform),typeof(CanvasRenderer),typeof(SinucaSpinSelector));
        spin.transform.SetParent(card.transform,false);
        Posicionar(spin.GetComponent<RectTransform>(),new Vector2(-130,57),new Vector2(44,44));
        spinSelector=spin.GetComponent<SinucaSpinSelector>();
        spinSelector.CanSelect=()=>estado==EstadoPartida.MirandoJogador&&!MenuPausa.Aberto;
        txtTendenciaEfeito=CriarTexto("TxtTendenciaEfeito",card.transform,"Toque no centro: tacada sem efeito.",10,new Color32(210,219,203,255),TextAlignmentOptions.Left);
        Posicionar(txtTendenciaEfeito.rectTransform,new Vector2(26,67),new Vector2(240,32));
        spinSelector.Changed=AplicarContatoEfeito;
        var reset=CriarBotaoHUD(card.transform,"SemEfeito","Centralizar toque",new Vector2(28,34),new Vector2(160,23),new Color32(44,64,54,255),out _);
        reset.onClick.AddListener(()=>SelecionarEfeitoBranca(SinucaCueSpin.Normal));
        var alvo=CriarBotaoHUD(card.transform,"EscolherPrimeiroToque","Primeiro toque: automático",new Vector2(0,0),new Vector2(312,27),new Color32(44,54,44,255),out txtAlvo);
        alvo.onClick.AddListener(AbrirEscolhaAlvo);
        txtEncaçapadasMarcus=CriarTexto("EncaçapadasMarcus",card.transform,"MARCUS",9,new Color32(234,224,195,255),TextAlignmentOptions.Left);
        txtEncaçapadasZe=CriarTexto("EncaçapadasZe",card.transform,"ZÉ",9,new Color32(234,224,195,255),TextAlignmentOptions.Left);
        Posicionar(txtEncaçapadasMarcus.rectTransform,new Vector2(-80,-31),new Vector2(152,16));
        Posicionar(txtEncaçapadasZe.rectTransform,new Vector2(80,-31),new Vector2(152,16));
        for(int n=1;n<16;n++)if(n!=8){var chip=new GameObject("Encaçapada_"+n,typeof(RectTransform),typeof(Image));chip.transform.SetParent(card.transform,false);var im=chip.GetComponent<Image>();im.sprite=CriarSpriteCirculo(40,SinucaTableView.CorDaBola(n),new Color(.84f,.78f,.58f));im.raycastTarget=false;encaçapadasIcones[n]=im;var number=CriarTexto("Numero",chip.transform,n.ToString(),8.5f,(n==1||n==5||n==9||n==13)?new Color(.06f,.06f,.04f):Color.white,TextAlignmentOptions.Center);UIFactory.Stretch(number.rectTransform);chip.SetActive(false);}
        var nota=CriarTexto("NotaTrocaModo",card.transform,"Trocar de modo reinicia a série atual.",9,new Color32(173,184,172,255),TextAlignmentOptions.Left);
        Posicionar(nota.rectTransform,new Vector2(0,-86),new Vector2(312,18));
        btnAcaoPraticaOuDuelo=CriarBotaoHUD(card.transform,"BtnAcaoPraticaOuDuelo","Ir para o treino",new Vector2(0,-117),new Vector2(312,30),new Color32(58,66,43,255),out txtBtnAcaoPraticaOuDuelo);
        btnAcaoPraticaOuDuelo.onClick.AddListener(AoClicarBotaoPraticaOuDuelo);
        painelOpcoes.SetActive(false);
    }

    private void ConstruirEscolhaAlvo()
    {
        painelAlvo=CriarPainelHUD(raiz.transform,"PrimeiroToque",Vector2.zero,new Vector2(640,360));
        UIFactory.Stretch(painelAlvo.GetComponent<RectTransform>());
        painelAlvo.GetComponent<Image>().color=new Color(0,0,0,.5f);
        painelAlvo.AddComponent<Button>().onClick.AddListener(()=>painelAlvo.SetActive(false));
        var card=CriarPainelHUD(painelAlvo.transform,"EscolhaPrimeiroToque",Vector2.zero,new Vector2(320,182));
        card.AddComponent<Button>().navigation = new Navigation { mode = Navigation.Mode.None };
        var title=CriarTexto("ExplicacaoAlvo",card.transform,"PRIMEIRO TOQUE\nAutomático segue a mira. Escolha uma bola para jogar de tabela.",10,new Color32(232,219,183,255),TextAlignmentOptions.Center);
        Posicionar(title.rectTransform,new Vector2(0,57),new Vector2(294,48));
        for(int n=0;n<=15;n++)
        {
            int target=n;var button=CriarBotaoHUD(card.transform,"Alvo"+n,n==0?"Auto":n.ToString(),new Vector2(-126+(n%8)*36,n<8?8:-25),new Vector2(32,27),new Color32(43,47,37,255),out var label);
            if(n>0){button.GetComponent<Image>().sprite=CriarSpriteCirculo(40,SinucaTableView.CorDaBola(n),new Color(.72f,.65f,.44f));label.color=(n==8||n==2||n==4||n==7||n==10||n==12||n==15)?Color.white:new Color(.06f,.06f,.045f);}
            escolhasAlvo[n]=button;button.onClick.AddListener(()=>{cantadaManual=target;txtAlvo.text=target==0?"Primeiro toque: automático":"Primeiro toque: bola "+target;painelAlvo.SetActive(false);txtTurnoStatus.text=target==0?"Alvo automático: acompanhe a linha de mira.":"Acerte a bola "+target+" antes das demais.";});
        }
        var voltar=CriarBotaoHUD(card.transform,"VoltarAlvo","Voltar à mesa",new Vector2(0,-65),new Vector2(180,26),new Color32(44,64,54,255),out _);
        voltar.onClick.AddListener(()=>painelAlvo.SetActive(false));
        painelAlvo.SetActive(false);
    }

    private bool EntradaMesaBloqueada(bool permitirInspecao = false)
    {
        return (OlhandoDebaixo && !permitirInspecao) || !Application.isFocused || Time.timeScale==0f || MenuPausa.Aberto || (painelModal != null && painelModal.activeSelf)
            || (painelOpcoes != null && painelOpcoes.activeSelf) || (painelAlvo != null && painelAlvo.activeSelf);
    }

    private void AlternarOpcoesMesa()
    {
        if (estado != EstadoPartida.MirandoJogador) return;
        CancelarCargaAoSuspender();
        painelOpcoes.SetActive(!painelOpcoes.activeSelf);
    }

    private void AtualizarPainelContextual()
    {
        if (txtControlesRodape == null) return;
        if(escudoModal!=null)escudoModal.SetActive(painelModal!=null&&painelModal.activeSelf);
        bool carga = estado == EstadoPartida.CarregandoForca;
        grupoForca.SetActive(carga); btnCancelarTacada.gameObject.SetActive(carga);
        txtForcaPercentual.text = "Força " + Mathf.RoundToInt(forcaNormalizada * 100f) + "%";
        Posicionar(txtTurnoStatus.rectTransform,new Vector2(carga?-144:0,7),new Vector2(carga?308:596,12));
        Posicionar(txtControlesRodape.rectTransform,new Vector2(carga?-144:0,-6),new Vector2(carga?308:596,12));
        switch (estado)
        {
            case EstadoPartida.MirandoJogador: txtControlesRodape.text=EmPe?"WASD: caminhar · Mouse: olhar · Esc: cursor · Clique na branca próxima para se apoiar":"Mouse: mirar · Direito + arraste: efeito · Esquerdo: puxar/tacar · Q: levantar"; break;
            case EstadoPartida.CarregandoForca: txtControlesRodape.text="Mais recuo = mais força. Solte para tacar."; break;
            case EstadoPartida.PosicionandoBranca: txtControlesRodape.text="Clique na mesa para repor a branca · Setas: ajustar · E: confirmar"; break;
            case EstadoPartida.TurnoZePensando: txtControlesRodape.text="Zé está escolhendo a jogada. Observe a mesa."; break;
            case EstadoPartida.BolasEmMovimento: txtControlesRodape.text="Aguarde as bolas pararem para jogar de novo."; break;
            default: txtControlesRodape.text="Como jogar explica os controles e as regras da mesa."; break;
        }
        if(EmPe && estado==EstadoPartida.MirandoJogador)txtTurnoStatus.text=tableView3D.Room.CanReachWhite(bolas[0].pos)?"Sua vez · Clique na branca para preparar a tacada.":"Sua vez · Caminhe até o lado da branca.";
        else if(EmPe)txtControlesRodape.text="WASD: caminhar · Mouse: olhar · Esc: cursor · Aguarde sua vez";
        if(ajustandoEfeito)txtControlesRodape.text="Arraste para ajustar o contato · Solte o botão direito para manter o efeito";
        if(interfaceLivre)txtControlesRodape.text="Cursor livre · Clique na sala ou Esc para voltar · Pausa: menu do jogo";
        AtualizarBotaoVistaCamera();
        if (!turnoDeMarcus && !string.IsNullOrEmpty(txtPercepcaoSubtitulo.text))
            txtControlesRodape.text=txtPercepcaoSubtitulo.text;
        txtControlesRodape.overflowMode=TextOverflowModes.Ellipsis;
        bool podeAjustar = estado == EstadoPartida.MirandoJogador || estado == EstadoPartida.PosicionandoBranca;
        btnOpcoes.interactable=estado == EstadoPartida.MirandoJogador;
        btnAjuda.interactable=podeAjustar;
        btnAcaoPraticaOuDuelo.interactable=podeAjustar;
    }

    private void AbrirEscolhaAlvo()
    {
        if(estado!=EstadoPartida.MirandoJogador)return;
        CancelarCargaAoSuspender();painelOpcoes.SetActive(false);
        for(int n=1;n<=15;n++)escolhasAlvo[n].interactable=modoPratica?bolas[n].ativa:regrasBrasileiras!=null&&regrasBrasileiras.CanNominate(n);
        painelAlvo.SetActive(true);
    }

    private Button CriarBotaoHUD(Transform pai, string nome, string rotulo, Vector2 pos, Vector2 tam, Color32 corFundo, out TextMeshProUGUI txtOut)
    {
        GameObject obj = new GameObject(nome, typeof(RectTransform), typeof(Image), typeof(Button));
        obj.transform.SetParent(pai, false);
        Posicionar(obj.GetComponent<RectTransform>(), pos, tam);
        obj.GetComponent<Image>().color = corFundo;
        Outline o = obj.AddComponent<Outline>();
        o.effectColor = new Color32(146, 114, 66, 185);

        txtOut = CriarTexto("Txt_" + nome, obj.transform, rotulo, 11f, new Color32(242, 224, 182, 255), TextAlignmentOptions.Center);
        UIFactory.Stretch(txtOut.rectTransform);
        var button = obj.GetComponent<Button>();
        button.navigation = new Navigation { mode = Navigation.Mode.None };
        button.onClick.AddListener(()=>{if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);});
        var colors = button.colors;
        colors.highlightedColor = new Color(1.25f,1.25f,1.18f,1f);
        colors.pressedColor = new Color(.72f,.82f,.70f,1f);
        colors.disabledColor = new Color(.55f,.55f,.55f,.65f);
        colors.fadeDuration = .08f; button.colors = colors;
        txtOut.margin = new Vector4(4,1,4,1);
        return button;
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
        var escudo=CriarPainelHUD(pai,"EscudoModalSinuca",Vector2.zero,new Vector2(640,360));
        UIFactory.Stretch(escudo.GetComponent<RectTransform>());
        escudo.GetComponent<Image>().color=new Color(0,0,0,.62f);
        // Shield visibility follows the card in AtualizarPainelContextual.
        escudoModal=escudo;
        painelModal.transform.SetParent(escudo.transform, false);
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
        if(tableView3D!=null)tableView3D.Room?.CancelOpponentShot();
        LimparFalaZe();
        comentariosZe.ResetSeries();
        ReiniciarPercepcoesSinuca();
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
        DefinirVistaInicial();
        ajudaInicialPendente=true;
        MostrarModal("ZÉ · COMO VAMOS JOGAR?","Quinze bolas no triângulo. Você prefere identificar os grupos pelas cores ou pelos números? A preta 8 é sempre a última. A branca já está posicionada.","LISAS / LISTRADAS",()=>{gruposPorNumero=false;painelModal.SetActive(false);AbrirAjudaGuiada(0,true);},"1–7 / 9–15",()=>{gruposPorNumero=true;painelModal.SetActive(false);AbrirAjudaGuiada(0,true);});
    }

    private void ComecarModoPratica()
    {
        ReiniciarPercepcoesSinuca();
        StopAllCoroutines();
        if(tableView3D!=null)tableView3D.Room?.CancelOpponentShot();
        LimparFalaZe();
        CancelarCargaAtiva();
        raiz.SetActive(true);
        if (painelModal != null) painelModal.SetActive(false);

        modoPratica = true;
        ajudaInicialPendente=false;
        if(painelOpcoes!=null)painelOpcoes.SetActive(false);
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
            bolas[i].rolagem = Vector2.zero;
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
        DefinirVistaInicial();
        forcaNormalizada = 0f;
        estado = EstadoPartida.MirandoJogador;
        if (txtPercepcaoSubtitulo != null)
            txtPercepcaoSubtitulo.text = "";

        AtualizarTextosHUD();
        SincronizarTransformBolas();
        AtualizarVisualTacoEMira();
    }

    private void DefinirVistaInicial()
    {
        if(painelOpcoes!=null)painelOpcoes.SetActive(false);
        if(painelAlvo!=null)painelAlvo.SetActive(false);
        AtualizarVisualTacoEMira();
        levantarAposTacada=-1;interfaceLivre=false;
        tableView3D.DefinirCamera(SinucaCameraView.Caminhando3D);
        tableView3D.Room.ResetWalking();
        tableView3D.DefinirZoom(1f);
        tableView3D.ReiniciarReferenciaPonteiro();
        AtualizarBotaoVistaCamera();
    }

    private void AoClicarBotaoPraticaOuDuelo()
    {
        if(estado!=EstadoPartida.MirandoJogador && estado!=EstadoPartida.PosicionandoBranca)return;
        CancelarCargaAtiva();painelOpcoes.SetActive(false);
        bool irDuelo=modoPratica;
        MostrarModal(irDuelo?"JOGAR CONTRA O ZÉ?":"ENTRAR NO TREINO?",
            irDuelo?"O treino termina e uma nova série começa contra o Zé.":"A série atual será reiniciada. No treino você pode experimentar as tacadas sem penalidades.",
            irDuelo?"COMEÇAR DUELO":"IR PARA O TREINO",()=>{painelModal.SetActive(false);if(irDuelo)ComecarNovaSerie(onVictoryCallback);else ComecarModoPratica();},
            "CONTINUAR NA MESA",()=>painelModal.SetActive(false));
    }

    private void AlternarVistaCamera()
    {
        if(EntradaMesaBloqueada())return;
        if(!EmPe)LevantarDaMesa();
        else txtTurnoStatus.text="Caminhe até a branca e clique nela para preparar a tacada.";
    }

    private void LevantarDaMesa()
    {
        CancelarCargaAoSuspender();levantarAposTacada=-1;
        tableView3D.DefinirCamera(SinucaCameraView.Caminhando3D);
        tableView3D.ReiniciarReferenciaPonteiro();AtualizarVisualTacoEMira();
    }

    private void AtualizarBotaoVistaCamera()
    { if(txtBtnVistaCamera!=null)txtBtnVistaCamera.text=EmPe?"Sala · WASD":"Levantar · Q"; }

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
        AtualizarIndicadorEfeito();
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
                    txtTendenciaEfeito.text = "Avanço: a branca segue para frente após o contato.";
                    txtTendenciaEfeito.color = new Color32(156, 214, 248, 255);
                    break;
                case SinucaCueSpin.Recuar:
                    txtTendenciaEfeito.text = "Recuo: a branca tende a voltar após o contato.";
                    txtTendenciaEfeito.color = new Color32(248, 198, 142, 255);
                    break;
                default:
                    txtTendenciaEfeito.text = "Centro: tacada sem efeito.";
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

    private void AbrirTutorialInicial() { AbrirAjudaGuiada(0,true); }

    private void AbrirModalRegrasDuranteJogo()
    {
        if(estado!=EstadoPartida.MirandoJogador && estado!=EstadoPartida.PosicionandoBranca)return;
        CancelarCargaAoSuspender();painelOpcoes.SetActive(false);
        AbrirAjudaGuiada(0,false);
    }

    private void AbrirAjudaGuiada(int pagina,bool inicial)
    {
        CancelarCargaAoSuspender();
        paginaAjuda=Mathf.Clamp(pagina,0,2);ajudaInicialPendente=inicial;
        string titulo,corpo;
        if(paginaAjuda==0)
        {
            titulo="COMO JOGAR · 1 / 3 · A TACADA";
            corpo="<b>1. Caminhe</b> com WASD até o lado da branca. Mova o mouse para olhar. Esc libera o cursor para a interface.\n"+
                "<b>2. Clique na branca</b> para apoiar a mão e preparar o taco. Ajuste a mira com o mouse.\n"+
                "<b>3. Clique e puxe para baixo</b>. Quanto mais puxar, maior a força.\n"+
                "<b>4. Solte</b> para tacar. A direção fica travada durante a puxada.\n\n"+
                "<b>Shift</b>: ajustes bem pequenos na mira.\n<b>Botão direito ou C</b>: cancelar sem gastar a vez.";
        }
        else if(paginaAjuda==1)
        {
            titulo="COMO JOGAR · 2 / 3 · A MESA";
            corpo="Você entra <b>em pé na sala</b>, sem corpo visível. Após tacar, volta a caminhar.\n"+
                "<b>E</b>: examinar um ruído sob a mesa quando estiver perto. Segure E olhando uma bola para examiná-la.\n"+
                "<b>Q ou Levantar</b>: sair da posição de tacada. Caminhe ao redor da mesa para escolher outro ângulo.\n<b>← / →</b>: ajustar a mira. <b>Espaço</b>: segure para aumentar a força e solte para tacar.\n\n"+
                "Em <b>Opções</b> ficam o efeito na branca, o primeiro toque manual e o treino. São opcionais: você pode jogar sem mexer neles.";
        }
        else
        {
            titulo="COMO JOGAR · 3 / 3 · O OBJETIVO";
            corpo="A primeira bola válida encaçapada define seu grupo: <b>lisas 1–7</b> ou <b>listradas 9–15</b>.\n"+
                "Encaçape seu grupo e depois a <b>preta 8</b>. Preta antecipada perde a mão. Duas mãos vencem a série.\n"+
                "<b>Falta</b>: o adversário pode reposicionar a branca.\n\n"+
                "Da 2ª mão em diante, a bola com <b>X</b> deve ficar na mesa. Se cair, a tacada é anulada e a formação volta.";
        }
        MostrarModal(titulo,corpo,paginaAjuda<2?"PRÓXIMO":"VAMOS JOGAR",
            ()=>{if(paginaAjuda<2)AbrirAjudaGuiada(paginaAjuda+1,inicial);else FecharAjudaGuiada();},
            inicial?"PULAR AJUDA":"VOLTAR À MESA",FecharAjudaGuiada);
    }

    private void FecharAjudaGuiada()
    {
        painelModal.SetActive(false);
        if(ajudaInicialPendente){ajudaInicialPendente=false;IniciarTurnoAtual();}
    }

    private void ConfigurarMao(int numeroMao)
    {
        LimparFalaZe();
        comentariosZe.ResetHand();
        EnfileirarFalaZe(numeroMao==1 ? "Primeiro a partida, Marcus. Depois a gente conversa." :
            numeroMao==2 ? "Mais uma mão. E cuida com a bola marcada." :
            "Última mão. Vamos terminar isso.", true);
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
        bolas[0].rolagem = Vector2.zero;

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
            bolas[i].rolagem = Vector2.zero;
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
        if(txtAlvo!=null)txtAlvo.text="Primeiro toque: automático";
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
        agendaPercepcoes.TurnStarted();
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
                txtTurnoStatus.text = "Sua vez · Caminhe até a branca e clique nela.";
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
        bool suspenderAudio = Time.timeScale == 0f || MenuPausa.Aberto || !Application.isFocused;
        SuspenderAudioPercepcao(suspenderAudio || interfaceLivre || !IsShowing || (painelModal!=null && painelModal.activeSelf) || (painelOpcoes!=null && painelOpcoes.activeSelf) || (painelAlvo!=null && painelAlvo.activeSelf));
        if (suspenderAudio != audioSuspenso)
        {
            audioSuspenso = suspenderAudio;
            foreach (var voz in vozesSinuca)
                if (voz != null) { if (audioSuspenso) voz.Pause(); else voz.UnPause(); }
        }
        AtualizarFalaZe(suspenderAudio);
        if (!IsShowing || estado == EstadoPartida.Inativo) return;
        var tecladoCursor=Keyboard.current;
        if(tecladoCursor!=null && tecladoCursor.escapeKey.wasPressedThisFrame && !MenuPausa.Aberto && MenuPausa.ClosedFrame!=Time.frameCount)
        {
            if(OlhandoDebaixo){SairDeBaixoDaMesa();return;}
            CancelarCargaAoSuspender();
            if(painelOpcoes!=null&&painelOpcoes.activeSelf)painelOpcoes.SetActive(false);
            else if(painelAlvo!=null&&painelAlvo.activeSelf)painelAlvo.SetActive(false);
            else if(painelModal==null||!painelModal.activeSelf)interfaceLivre=!interfaceLivre;
        }
        AtualizarCapturaCursor();
        AtualizarPainelContextual();

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

        if(suspenderAudio)return;
        var kb = Keyboard.current;
        if (painelOpcoes != null && painelOpcoes.activeSelf)
        {
            CancelarCargaAoSuspender();
            return;
        }
        if(painelAlvo!=null&&painelAlvo.activeSelf)
        {
            if(kb!=null&&kb.escapeKey.wasPressedThisFrame)painelAlvo.SetActive(false);
            return;
        }

        if(AtualizarPercepcoesSinuca())return;
        AtualizarAjusteEfeito();
        LerMouseCapturado();
        if(levantarAposTacada>=0 && Time.time>=levantarAposTacada)LevantarDaMesa();
        if(!interfaceLivre && kb!=null && (kb.qKey.wasPressedThisFrame || kb.vKey.wasPressedThisFrame))AlternarVistaCamera();
        if(EmPe)
        {
            Vector2 move=Vector2.zero,look=Vector2.zero;
            if(!interfaceLivre && kb!=null){move.x=(kb.dKey.isPressed?1:0)-(kb.aKey.isPressed?1:0);move.y=(kb.wKey.isPressed?1:0)-(kb.sKey.isPressed?1:0);}
            var mouse=Mouse.current;
            if(mouse!=null && Cursor.lockState==CursorLockMode.Locked && Time.frameCount!=frameCaptura)look=mouse.delta.ReadValue();
            tableView3D.Room.MovePlayer(move,look,Time.deltaTime);
            AtualizarVisualTacoEMira();
        }
        else AtualizarVisualTacoEMira(); // also advances the approach/follow-through animation

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
                if(!interfaceLivre)AtualizarPosicionamentoBranca();
                break;

            case EstadoPartida.MirandoJogador:
                if(!interfaceLivre && !EmPe && tableView3D.Room.SettledAtCue)AtualizarMiraJogador();
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
        if (EntradaMesaBloqueada()) return;
        if(interfaceLivre){interfaceLivre=false;AtualizarCapturaCursor();return;}

        if (button == PointerEventData.InputButton.Right)
        {
            if (estado == EstadoPartida.CarregandoForca)
            {
                CancelarCargaAtiva();
                txtTurnoStatus.text = "Tacada cancelada. Mire, clique e puxe para trás. Solte para tacar.";
            }
            else IniciarAjusteEfeito();
            return;
        }

        if(ajustandoEfeito)return;
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

        if(EmPe)
        {
            if(estado!=EstadoPartida.MirandoJogador || !bolas[0].ativa)return;
            if((tablePos-bolas[0].pos).sqrMagnitude>BallRadius*BallRadius*2.25f)return;
            if(!tableView3D.Room.CanReachWhite(bolas[0].pos)) {txtTurnoStatus.text="Aproxime-se mais da branca.";return;}
            anguloMiraRad=eixoMiraApoiada=tableView3D.Room.ApproachAngle(bolas[0].pos);
            miraMouseAtiva=referenciaMiraMouse=false;
            tableView3D.DefinirCamera(SinucaCameraView.LinhaTaco);
            tableView3D.ReiniciarReferenciaPonteiro();frameConfirmacaoBranca=Time.frameCount;
            txtTurnoStatus.text="Ajuste a mira. Clique, puxe para baixo e solte. Q: levantar.";
            AtualizarVisualTacoEMira();return; // this click enters the stance, never charges
        }
        if(!tableView3D.Room.SettledAtCue)return;

        if (estado == EstadoPartida.MirandoJogador && Time.frameCount != frameConfirmacaoBranca)
        {
            // Preserve the aim, including fine keyboard adjustments, on mouse-down.
            IniciarCargaPorDispositivo(DispositivoCarga.MouseEsquerdo);
            Vector2 dir = new Vector2(Mathf.Cos(anguloMiraRad), Mathf.Sin(anguloMiraRad));
            Vector2 a = tableView3D.ProjetarPontoMesaParaLocalUI(new Vector3(bolas[0].pos.x, 0f, bolas[0].pos.y));
            Vector2 b = tableView3D.ProjetarPontoMesaParaLocalUI(new Vector3(bolas[0].pos.x + dir.x * 50f, 0f, bolas[0].pos.y + dir.y * 50f));
            direcaoPuxadaUI = Vector2.down; // first-person pullback stays stable as the camera follows aim
            puxadaValida = Mouse.current != null && tableView3D.ScreenToViewPoint(Mouse.current.position.ReadValue(), out inicioPuxadaUI)
                && (a - b).sqrMagnitude > 0.01f;
            if (!puxadaValida) CancelarCargaAtiva();
        }
    }

    private void OnMesaPointerMove(Vector2 tablePos)
    {
        if (EntradaMesaBloqueada() || interfaceLivre || EmPe || ajustandoEfeito) return;

        if (estado == EstadoPartida.PosicionandoBranca)
        {
            PosicionarBrancaEm(tablePos);
            return;
        }

        // Aim stays locked during pullback; power is sampled separately in viewport coordinates.
        if (estado == EstadoPartida.MirandoJogador)
        {
            AtualizarAnguloMiraParaPonto(tablePos);
            AtualizarVisualTacoEMira();
        }
    }

    private void OnMesaPointerUp(Vector2 tablePos, bool dentroDoViewport, PointerEventData.InputButton button)
    {
        if(button==PointerEventData.InputButton.Right){EncerrarAjusteEfeito();return;}
        if (estado != EstadoPartida.CarregandoForca) return;
        if (dispositivoCargaAtivo != DispositivoCarga.MouseEsquerdo || button != PointerEventData.InputButton.Left)
            return;

        bool sobreHUD=Mouse.current!=null && (RectTransformUtility.RectangleContainsScreenPoint(headerMesa,Mouse.current.position.ReadValue(),null)||RectTransformUtility.RectangleContainsScreenPoint(footerMesa,Mouse.current.position.ReadValue(),null));
        if (!dentroDoViewport || sobreHUD || EntradaMesaBloqueada())
        {
            CancelarCargaAtiva();
            txtTurnoStatus.text = "Tacada cancelada fora da mesa.";
            return;
        }

        AtualizarForcaPuxada();
        DispararTacadaCarregada();
    }

    private void OnMesaScrollZoom(float novoZoom)
    {
        CancelarCargaAoSuspender();
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
        txtTurnoStatus.text = "Branca posicionada. Mire, clique, puxe para trás e solte para tacar.";
        AtualizarVisualTacoEMira();
    }

    private void AtualizarAnguloMiraParaPonto(Vector2 tablePos)
    {
        Vector2 delta = tablePos - bolas[0].pos;
        if (delta.sqrMagnitude > BallRadius * BallRadius)
        {
            float raw = Mathf.Atan2(delta.y, delta.x);
            bool precisao = Keyboard.current != null && Keyboard.current.shiftKey.isPressed;
            if (!referenciaMiraMouse)
            {
                ultimoAnguloMouseRad = raw;
                offsetMiraMouseRad = precisao ? anguloMiraRad - raw : 0f;
                alvoMiraMouseRad = precisao ? anguloMiraRad : raw;
                referenciaMiraMouse = true;
            }
            if (precisao)
            {
                float passo = Mathf.DeltaAngle(ultimoAnguloMouseRad * Mathf.Rad2Deg, raw * Mathf.Rad2Deg) * Mathf.Deg2Rad;
                alvoMiraMouseRad += passo * 0.15f;
                // Preserve the fine adjustment when Shift is released: no snap back to the cursor.
                offsetMiraMouseRad = alvoMiraMouseRad - raw;
            }
            else alvoMiraMouseRad = raw + offsetMiraMouseRad;
            ultimoAnguloMouseRad = raw;
            miraMouseAtiva = true;
        }
    }

    public static float SuavizarMira(float atualRad, float alvoRad, float dt)
    {
        float peso = 1f - Mathf.Exp(-35f * Mathf.Max(0f, dt));
        return Mathf.LerpAngle(atualRad * Mathf.Rad2Deg, alvoRad * Mathf.Rad2Deg, peso) * Mathf.Deg2Rad;
    }

    private void AtualizarPosicionamentoBranca()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        Vector2 move = Vector2.zero;
        if (kb.upArrowKey.isPressed) move.y += 1f;
        if (kb.downArrowKey.isPressed) move.y -= 1f;
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

    private void LimitarMiraApoiada()
    {
        float offset=Mathf.DeltaAngle(eixoMiraApoiada*Mathf.Rad2Deg,anguloMiraRad*Mathf.Rad2Deg);
        anguloMiraRad=eixoMiraApoiada+Mathf.Clamp(offset,-55f,55f)*Mathf.Deg2Rad;
    }

    private void AtualizarMiraJogador()
    {
        if(ajustandoEfeito)return;
        if (miraMouseAtiva) anguloMiraRad = SuavizarMira(anguloMiraRad, alvoMiraMouseRad, Time.deltaTime);
        var kb = Keyboard.current;
        if (kb != null)
        {
            float rotSpeed = 0.75f;
            if (kb.shiftKey.isPressed) rotSpeed = 0.16f;

            if (kb.leftArrowKey.isPressed || kb.rightArrowKey.isPressed)
            {
                miraMouseAtiva = referenciaMiraMouse = false;
                tableView3D.ReiniciarReferenciaPonteiro();
            }
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

        LimitarMiraApoiada();

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
        miraMouseAtiva = referenciaMiraMouse = false;
        dispositivoCargaAtivo = disp;
        estado = EstadoPartida.CarregandoForca;
        forcaNormalizada = 0f;
        puxadaAcumulada=Vector2.zero;frameInicioCarga=Time.frameCount;framePuxada=-1;
        puxadaValida = false;
        AtualizarBarraForcaVisual(forcaNormalizada);
        txtTurnoStatus.text = disp == DispositivoCarga.MouseEsquerdo
            ? "Puxe para trás da mira. Mais recuo = mais força. Solte para tacar; Dir./C cancela."
            : "Espaço: segure para aumentar a força, solte para tacar. C cancela.";
        AtualizarVisualTacoEMira();
    }

    private void CancelarCargaAtiva()
    {
        miraMouseAtiva = referenciaMiraMouse = false;
        dispositivoCargaAtivo = DispositivoCarga.Nenhum;
        puxadaValida = false;
        if (tableView3D != null) tableView3D.ReiniciarReferenciaPonteiro();
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

    // Retained for existing QA callers; keyboard charge now rises once and stays at maximum.
    public static float OscilarCarga(float atual, float dt, ref bool subindo)
    {
        subindo = true;
        return Mathf.Clamp01(atual + Mathf.Max(0f, dt) * 0.65f);
    }

    public static float CalcularForcaPuxada(Vector2 deslocamento, Vector2 direcaoParaTras)
    {
        // Ignore lateral jitter and the first 3 canvas units. Returning the mouse reduces power.
        return Mathf.Clamp01((Vector2.Dot(deslocamento, direcaoParaTras.normalized) - 3f) / 97f);
    }

    private void AtualizarForcaPuxada()
    {
        var mouse = Mouse.current;
        if (!puxadaValida || mouse == null) return;
        if(Cursor.lockState==CursorLockMode.Locked)
        {
            if(framePuxada!=Time.frameCount && Time.frameCount!=frameInicioCarga)
            {puxadaAcumulada+=mouse.delta.ReadValue()/Mathf.Max(.01f,mesaRect.GetComponentInParent<Canvas>().scaleFactor);framePuxada=Time.frameCount;}
            forcaNormalizada=CalcularForcaPuxada(puxadaAcumulada,Vector2.down);return;
        }
        if (tableView3D.ScreenToViewPoint(mouse.position.ReadValue(), out var atual))
            forcaNormalizada = CalcularForcaPuxada(atual - inicioPuxadaUI, direcaoPuxadaUI);
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
            txtTurnoStatus.text = "Tacada cancelada. Mire, clique e puxe para trás. Solte para tacar.";
            return;
        }

        if (dispositivoCargaAtivo == DispositivoCarga.MouseEsquerdo)
        {
            AtualizarForcaPuxada();
            // A missing pointer-up must never leave the shot armed for a later click.
            if (mouse == null || (!mouse.leftButton.isPressed && !mouse.leftButton.wasReleasedThisFrame))
            { CancelarCargaAtiva(); return; }
        }
        else
        {
            bool subindo = true;
            forcaNormalizada = OscilarCarga(forcaNormalizada, Time.deltaTime, ref subindo);
        }
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
        if (forcaFinal < 0.015f)
        {
            CancelarCargaAtiva();
            txtTurnoStatus.text = "Sem tacada. Clique e puxe para trás antes de soltar.";
            return;
        }
        dispositivoCargaAtivo = DispositivoCarga.Nenhum;
        puxadaValida = false;
        if (tableView3D != null) tableView3D.ReiniciarReferenciaPonteiro();
        forcaNormalizada = 0f;
        AtualizarBarraForcaVisual(0f);

        Vector2 dir = new Vector2(Mathf.Cos(anguloMiraRad), Mathf.Sin(anguloMiraRad));
        float potencia = Mathf.Lerp(MinShotPower, MaxShotPower, Mathf.Pow(forcaFinal, 1.65f));
        tableView3D.Room.BeginStroke(forcaFinal);
        levantarAposTacada=Time.time+.26f;
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
        var room = tableView3D.Room;
        bool presented = room.PrepareOpponentShot(bolas[0].pos, direcaoZe);
        if (!presented)
        {
            // A missing art resource must not permanently strand the rules on Zé's turn.
            Debug.LogWarning("Sinuca: apresentação do Zé indisponível; executando a jogada escolhida.");
            while (ApresentacaoZeSuspensa()) yield return null;
            if (estado == EstadoPartida.TurnoZePensando)
                ExecutarTacadaComEfeito(direcaoZe, potenciaZe, SinucaCueSpin.Normal);
            yield break;
        }
        while (!room.OpponentAtShot)
        {
            if (estado != EstadoPartida.TurnoZePensando) { room.CancelOpponentShot(); yield break; }
            if (!ApresentacaoZeSuspensa()) room.TickOpponentWalk(Time.deltaTime);
            yield return null;
        }
        float elapsed = 0;
        bool struck = false;
        const float duration = 1.65f, contact = .82f;
        while (elapsed < duration)
        {
            if (!struck && estado != EstadoPartida.TurnoZePensando) { room.CancelOpponentShot(); yield break; }
            if (!ApresentacaoZeSuspensa())
            {
                elapsed = Mathf.Min(duration, elapsed + Mathf.Min(Time.deltaTime, .05f));
                float progress = elapsed / duration;
                room.PoseOpponentShot(progress);
                if (!struck && progress >= contact)
                {
                    struck = true;
                    ExecutarTacadaComEfeito(direcaoZe, potenciaZe, SinucaCueSpin.Normal);
                }
            }
            yield return null;
        }
        room.CancelOpponentShot();
    }

    private bool ApresentacaoZeSuspensa()
        => !IsShowing || MenuPausa.Aberto || Time.timeScale == 0 || !Application.isFocused ||
           (painelModal != null && painelModal.activeSelf) ||
           (painelOpcoes != null && painelOpcoes.activeSelf) ||
           (painelAlvo != null && painelAlvo.activeSelf);

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

                float potBase = Mathf.Clamp(165f + (distCueGhost + distBallPocket) * 0.58f / Mathf.Max(0.35f, cosCut), 155f, MaxShotPower);
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
        Vector2[] roll = new Vector2[16];
        bool[] a = new bool[16];
        for (int i = 0; i < 16; i++)
        {
            p[i] = bolas[i].pos;
            v[i] = Vector2.zero;
            a[i] = bolas[i].ativa;
        }

        v[0] = dir.normalized * Mathf.Clamp(potencia, MinShotPower, MaxShotPower);
        bolasLegaisCaidas = 0;
        caiuBranca = false;
        caiuMarcada = false;
        tocouObjeto = false;
        tocouTabelaOuCacapa = false;
        menorDistCacapa = 999f;

        float limX = TableWidth * 0.5f - BallRadius;
        float limY = TableHeight * 0.5f - BallRadius;
        const float dt = FixedStep;

        for (int step = 0; step < 2400; step++)
        {
            bool movendo = false;
            for (int i = 0; i < 16; i++)
            {
                if (!a[i]) continue;
                float spdSq = v[i].sqrMagnitude;
                if (spdSq <= StopSpeedSq && roll[i].sqrMagnitude <= StopSpeedSq)
                {
                    v[i] = Vector2.zero;
                    continue;
                }
                movendo = true;
                Vector2 anterior = p[i];
                p[i] += v[i] * dt;
                IntegrarPano(ref v[i], ref roll[i], dt);

                for (int c = 0; c < cacapas.Length; c++)
                {
                    float dCac = Vector2.Distance(p[i], cacapas[c]);
                    if (i > 0 && (modoPratica || regrasBrasileiras == null || regrasBrasileiras.CanNominate(i)) && dCac < menorDistCacapa)
                        menorDistCacapa = dCac;

                    if (CruzaCapturaCacapa(anterior, p[i], cacapas[c]))
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

                if (p[i].x < -limX && !PertoDeCacapa(p[i], PocketOpeningRadius)) { p[i].x = -limX; v[i].x = Mathf.Abs(v[i].x) * CushionRestitution; if (tocouObjeto) tocouTabelaOuCacapa = true; }
                else if (p[i].x > limX && !PertoDeCacapa(p[i], PocketOpeningRadius)) { p[i].x = limX; v[i].x = -Mathf.Abs(v[i].x) * CushionRestitution; if (tocouObjeto) tocouTabelaOuCacapa = true; }

                if (p[i].y < -limY && !PertoDeCacapa(p[i], PocketOpeningRadius)) { p[i].y = -limY; v[i].y = Mathf.Abs(v[i].y) * CushionRestitution; if (tocouObjeto) tocouTabelaOuCacapa = true; }
                else if (p[i].y > limY && !PertoDeCacapa(p[i], PocketOpeningRadius)) { p[i].y = limY; v[i].y = -Mathf.Abs(v[i].y) * CushionRestitution; if (tocouObjeto) tocouTabelaOuCacapa = true; }
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
                            Vector2 imp = n * (vn * (1f + BallRestitution) * 0.5f);
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
            bolas[i].rolagem = Vector2.zero;
            bolas[i].spinLateral = 0f;
        }

        Vector2 dirNorm = direcao.sqrMagnitude > 0.0001f ? direcao.normalized : Vector2.right;
        float potClamped = Mathf.Clamp(potencia, MinShotPower, MaxShotPower);
        bolas[0].vel = dirNorm * potClamped * Mathf.Sqrt(1f-.35f*contatoTaco.sqrMagnitude);
        bolas[0].spinLateral=turnoDeMarcus ? contatoTaco.x*potClamped*.3f : 0;

        // A centre strike starts sliding. Cloth develops natural forward roll;
        // follow/draw retain angular momentum at impact instead of adding an impulse.
        float spinRatio = efeito == SinucaCueSpin.Avancar ? 1.2f :
                          efeito == SinucaCueSpin.Recuar ? -0.8f : 0f;
        if (turnoDeMarcus && Mathf.Abs(contatoTaco.y) > .01f) spinRatio = contatoTaco.y * 1.2f;
        bolas[0].spinLongitudinal = spinRatio * bolas[0].vel.magnitude;
        bolas[0].rolagem = dirNorm * bolas[0].spinLongitudinal;
        TocarSomTacada(Mathf.InverseLerp(MinShotPower, MaxShotPower, potClamped));
        AtualizarVisualTacoEMira();
        txtTurnoStatus.text = turnoDeMarcus ? "Marcus executou a tacada..." : "Zé executou a tacada...";
    }

    // Coulomb sliding friction with exact slip cancellation (no numerical reversal).
    // Angular surface speed changes 5/2 times as fast as linear speed for a solid sphere.
    // Tangential cushion impulse also spends side-spin energy (I = 2/5 mr²).
    // Adding side spin directly to linear speed would create energy on glancing banks.
    private static void RebaterTabela(ref Vector2 velocidade, ref float spin, Vector2 normal)
    {
        float normalSpeed = Vector2.Dot(velocidade, normal);
        if (normalSpeed >= 0f) return;
        float normalImpulse = -(1f + CushionRestitution) * normalSpeed;
        velocidade += normal * normalImpulse;
        Vector2 tangent = new Vector2(-normal.y, normal.x);
        float slip = Vector2.Dot(velocidade, tangent) - spin;
        float tangentImpulse = Mathf.Clamp(slip / 3.5f, -normalImpulse * .12f, normalImpulse * .12f);
        velocidade -= tangent * tangentImpulse;
        spin += tangentImpulse * 2.5f;
    }

    private static void IntegrarPano(ref Vector2 velocidade, ref Vector2 rolagem, float dt)
    {
        Vector2 slip = velocidade - rolagem;
        float speed = slip.magnitude;
        if (speed > .0001f)
        {
            float slidingTime = Mathf.Min(dt, speed / (3.5f * SlidingFriction));
            Vector2 friction = slip / speed * (SlidingFriction * slidingTime);
            velocidade -= friction;
            rolagem += friction * 2.5f;
            dt -= slidingTime;
        }
        if (dt > 0f)
        {
            velocidade = Vector2.MoveTowards(velocidade, Vector2.zero, BallFriction * dt);
            rolagem = velocidade;
        }
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
                if (speedSq <= StopSpeedSq && b.rolagem.sqrMagnitude <= StopSpeedSq)
                {
                    b.vel = Vector2.zero;
                    b.spinLongitudinal = 0f;
                    b.rolagem = Vector2.zero;
                    continue;
                }

                Vector2 anterior = b.pos;
                Vector2 deslocamento = b.vel * subDt;
                b.pos += deslocamento;

                // Atualizar rotação 3D da esfera pelo deslocamento no pano + giro longitudinal
                float dist = b.rolagem.magnitude * subDt;
                if (dist > 0.0001f)
                {
                    Vector2 dirMov = b.rolagem.normalized;
                    Vector3 eixoRolagem = new Vector3(dirMov.y, 0f, -dirMov.x);
                    float anguloGraus = (dist / BallRadius) * Mathf.Rad2Deg;
                    b.rotacao = Quaternion.AngleAxis(anguloGraus, eixoRolagem) * b.rotacao;
                }

                IntegrarPano(ref b.vel, ref b.rolagem, subDt);
                b.spinLateral = Mathf.MoveTowards(b.spinLateral, 0f, BallFriction * .18f * subDt);

                // Verificar captura pelas 6 caçapas
                for (int p = 0; p < cacapas.Length; p++)
                {
                    // Sweep the centre through the actual clearance. No magnetic funnel;
                    // a fast valid shot cannot skip capture between physics steps.
                    if (CruzaCapturaCacapa(anterior, b.pos, cacapas[p]))
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
                            RebaterTabela(ref b.vel, ref b.spinLateral, nJaw);
                            RegistrarTabela();
                            TocarSomTabela(Mathf.Abs(vnJaw) / MaxShotPower);
                        }
                    }
                }

                // Rebote nas tabelas retas (fora da garganta das caçapas)
                bool naBocaCacapa = PertoDeCacapa(b.pos, PocketOpeningRadius);
                if (!naBocaCacapa)
                {
                    if (b.pos.x < -limX)
                    {
                        b.pos.x = -limX;
                        if (b.vel.x < 0f)
                        {
                            RebaterTabela(ref b.vel, ref b.spinLateral, Vector2.right);
                            RegistrarTabela();
                            TocarSomTabela(Mathf.Abs(b.vel.x) / MaxShotPower);
                        }
                    }
                    else if (b.pos.x > limX)
                    {
                        b.pos.x = limX;
                        if (b.vel.x > 0f)
                        {
                            RebaterTabela(ref b.vel, ref b.spinLateral, Vector2.left);
                            RegistrarTabela();
                            TocarSomTabela(Mathf.Abs(b.vel.x) / MaxShotPower);
                        }
                    }

                    if (b.pos.y < -limY)
                    {
                        b.pos.y = -limY;
                        if (b.vel.y < 0f)
                        {
                            RebaterTabela(ref b.vel, ref b.spinLateral, Vector2.up);
                            RegistrarTabela();
                            TocarSomTabela(Mathf.Abs(b.vel.y) / MaxShotPower);
                        }
                    }
                    else if (b.pos.y > limY)
                    {
                        b.pos.y = limY;
                        if (b.vel.y > 0f)
                        {
                            RebaterTabela(ref b.vel, ref b.spinLateral, Vector2.down);
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
                            Vector2 impulse = normal * (velAlongNormal * (1f + BallRestitution) * 0.5f);
                            bolas[i].vel -= impulse;
                            bolas[j].vel += impulse;

                            // Smooth sphere contact transfers only normal linear impulse.
                            // Cloth resolves the retained angular momentum on subsequent steps.

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
            if (bolas[i].ativa && (bolas[i].vel.sqrMagnitude > StopSpeedSq || bolas[i].rolagem.sqrMagnitude > StopSpeedSq))
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
                bolas[i].rolagem = Vector2.zero;
            bolas[i].spinLateral = 0f;
                bolas[i].caindoNaCacapa = false;
            }
            ResolverFimDaTacada();
        }
    }

    private static bool CruzaCapturaCacapa(Vector2 anterior, Vector2 atual, Vector2 centro)
    {
        Vector2 passo = atual - anterior;
        float t = passo.sqrMagnitude > .000001f
            ? Mathf.Clamp01(Vector2.Dot(centro - anterior, passo) / passo.sqrMagnitude) : 0f;
        return (anterior + passo * t - centro).sqrMagnitude <= PocketRadius * PocketRadius;
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
        b.rolagem = Vector2.zero;
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
        bool marcusTacou = regrasBrasileiras.MarcusTurn;
        var resultado = regrasBrasileiras.Resolve(bolaCantada,primeiroContatoNumero,convertidas,brancaCaiuNestaTacada,numeroBolaMarcada,!houveToqueTabelaOuCacapa);
        var comentario = comentariosZe.React(regrasBrasileiras, resultado, marcusTacou);
        EnfileirarFalaZe(comentario.Text, comentario.Urgent);
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
            txtTurnoStatus.text = "Treino: tente outra tacada. Para enfrentar o Zé, abra Opções.";
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
        EncerrarPercepcaoDaMao();
        LimparFalaZe();
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

        foreach (var jaw in bicosCacapas)
        {
            Vector2 delta = jaw.pos - origemBranca;
            float along = Vector2.Dot(delta, dir);
            float radius = BallRadius + jaw.raio;
            float perpendicularSq = delta.sqrMagnitude - along * along;
            if (along <= 0f || perpendicularSq >= radius * radius) continue;
            float hit = along - Mathf.Sqrt(radius * radius - perpendicularSq);
            if (hit >= 0f) tTabela = Mathf.Min(tTabela, hit);
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
        bool mostrarMira = !EmPe && (estado == EstadoPartida.MirandoJogador || estado == EstadoPartida.CarregandoForca) && bolas.Count > 0 && bolas[0].ativa;
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

        bool mostrarMira = !EmPe && (estado == EstadoPartida.MirandoJogador || estado == EstadoPartida.CarregandoForca) && bolas[0].ativa;
        SinucaAimGuideData guia = mostrarMira
            ? CalcularGuiaDeMira(bolas[0].pos, anguloMiraRad, efeitoEscolhido)
            : new SinucaAimGuideData { ativa = false };

        AplicarGuiaBolaFalsa(ref guia);
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

        AtualizarBolaFalsaVisual();
        tableView3D.Preaquecer();
        tableView3D.Room.SetCueContact(contatoTaco);
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
                ? "TREINO LIVRE"
                : $"MARCUS {maosVencidasMarcus}  :  {maosVencidasZe} ZÉ   ·   Mão {maoAtual}/3";
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
                txtRegraBolaMarcada.text = "Experimente as tacadas · sem penalidades";
            else if (numeroBolaMarcada > 0 && bolas[numeroBolaMarcada].ativa)
                txtRegraBolaMarcada.text = $"REGRA ATIVA — \"ESSA FICA NA MESA\": Bola [{numeroBolaMarcada}X] NÃO pode ser encaçapada!";
            else if (maoAtual >= 2)
                txtRegraBolaMarcada.text = "REGRA \"ESSA FICA NA MESA\": Desativada (menos de 2 bolas restantes).";
            else
                txtRegraBolaMarcada.text = "SINUQUINHA · 15 BOLAS · PRETA 8 POR ÚLTIMO";
        }

        if(!modoPratica&&regrasBrasileiras!=null&&txtRegraBolaMarcada!=null)
        {
            string grupo=regrasBrasileiras.MarcusGroup==0?"Mesa aberta":regrasBrasileiras.MarcusGroup==1?(gruposPorNumero?"Seu grupo: 1–7":"Seu grupo: lisas"):gruposPorNumero?"Seu grupo: 9–15":"Seu grupo: listradas";
            txtRegraBolaMarcada.text=grupo+(numeroBolaMarcada>0?" · Não encaçape "+numeroBolaMarcada+" [X]":" · Preta 8 por último");
        }

        if (txtBtnAcaoPraticaOuDuelo != null)
        {
            txtBtnAcaoPraticaOuDuelo.text = modoPratica ? "Jogar contra o Zé" : "Ir para o treino";
        }

        if(txtEncaçapadasMarcus!=null&&regrasBrasileiras!=null)
        {
            txtEncaçapadasMarcus.text="MARCUS · BOLAS"+(regrasBrasileiras.MarcusGroup==0?" —":"");
            txtEncaçapadasZe.text="ZÉ · BOLAS"+(regrasBrasileiras.MarcusGroup==0?" —":"");
            int left=0,right=0;for(int n=1;n<16;n++)if(n!=8){bool show=!modoPratica&&!regrasBrasileiras.Present[n]&&regrasBrasileiras.MarcusGroup!=0;encaçapadasIcones[n].gameObject.SetActive(show);if(show){bool marcus=SinuquinhaRules.Group(n)==regrasBrasileiras.MarcusGroup;int j=marcus?left++:right++;Posicionar(encaçapadasIcones[n].rectTransform,new Vector2((marcus?-150:10)+j*16,-53),new Vector2(13,13));}}
        }
        AtualizarBotaoVistaCamera();
    }

    // =========================================================================
    // Síntese e Disparo de Áudio por Evento Real (Tacada, Colisão, Tabela, Caçapa, Giz)
    // =========================================================================

    private void ConstruirLegendaZe()
    {
        var painel = CriarPainelHUD(raiz.transform,"LegendaZe",Vector2.zero,new Vector2(520,48));
        var rect = painel.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(.5f,0);
        rect.anchoredPosition = new Vector2(0,64);
        painel.GetComponent<Image>().color = new Color32(10,13,11,220);
        legendaZe = painel.AddComponent<CanvasGroup>();
        legendaZe.blocksRaycasts = legendaZe.interactable = false;
        legendaZe.alpha = 0;
        txtFalaZe = CriarTexto("FalaZe",painel.transform,"",16,new Color32(244,233,208,255),TextAlignmentOptions.Center);
        Posicionar(txtFalaZe.rectTransform,Vector2.zero,new Vector2(496,42));
        txtFalaZe.richText = false;
        txtFalaZe.raycastTarget = false;
        txtFalaZe.overflowMode = TextOverflowModes.Overflow;
    }

    private void EnfileirarFalaZe(string texto, bool urgente = false)
    {
        if(string.IsNullOrEmpty(texto) || agendaPercepcoes.Active!=SinucaPerceptionSchedule.Event.None || falasHistoria.Count>0 || OlhandoDebaixo || batidasRestantes>0 || (falaZePendenteUrgente && locutorPendenteMarcus))return;
        locutorPendenteMarcus = false;
        // Uma reação importante substitui a espera; comentários comuns não se acumulam.
        if(!urgente && (falaZePendenteUrgente || (txtFalaZe!=null && !string.IsNullOrEmpty(txtFalaZe.text))))return;
        falaZePendente = texto;
        falaZePendenteUrgente = urgente;
    }

    private void LimparFalaZe()
    {
        falaZePendente = null;falaZePendenteUrgente = false;
        tempoLetraZe = tempoLeituraZe = intervaloSilabaZe = intervaloFalaZe = 0;
        caractereZe = silabaZe = 0;
        if(txtFalaZe!=null)txtFalaZe.text = "";
        if(legendaZe!=null)legendaZe.alpha = 0;
        if(vozFalaZe!=null)vozFalaZe.Stop();
    }

    private void AtualizarFalaZe(bool suspenso)
    {
        if(txtFalaZe==null)return;
        bool bloqueado = suspenso || !IsShowing || modoPratica || estado==EstadoPartida.Inativo ||
            (painelModal!=null && painelModal.activeSelf) ||
            (painelOpcoes!=null && painelOpcoes.activeSelf) || (painelAlvo!=null && painelAlvo.activeSelf);
        if(bloqueado)
        {
            legendaZe.alpha = 0;
            if(vozFalaZe!=null)vozFalaZe.Pause();
            return;
        }
        if(vozFalaZe!=null)
        {
            vozFalaZe.volume = .20f * AudioSettingsController.VolumeFala;
            vozFalaZe.UnPause();
        }
        float dt = Mathf.Min(Time.unscaledDeltaTime,.1f);
        intervaloSilabaZe -= dt;
        if(string.IsNullOrEmpty(txtFalaZe.text))
        {
            intervaloFalaZe -= dt;
            if(string.IsNullOrEmpty(falaZePendente) || intervaloFalaZe>0)return;
            locutorAtualMarcus = locutorPendenteMarcus;
            string prefixo = locutorAtualMarcus ? "Marcus: " : "Zé: ";
            txtFalaZe.text = prefixo + falaZePendente;
            txtFalaZe.ForceMeshUpdate();
            txtFalaZe.maxVisibleCharacters = caractereZe = prefixo.Length;
            tempoLetraZe = 0;
            tempoLeituraZe = Mathf.Max(2.2f,falaZePendente.Length*.035f);
            falaZePendente = null;falaZePendenteUrgente = false;
        }
        legendaZe.alpha = Mathf.MoveTowards(legendaZe.alpha,1,dt*8);
        int total = txtFalaZe.textInfo.characterCount;
        if(caractereZe<total)
        {
            tempoLetraZe -= dt;
            if(tempoLetraZe<=0)
            {
                char letra = txtFalaZe.textInfo.characterInfo[caractereZe].character;
                txtFalaZe.maxVisibleCharacters = ++caractereZe;
                tempoLetraZe += char.IsPunctuation(letra) ? .16f : .032f;
                if(char.IsLetterOrDigit(letra) && intervaloSilabaZe<=0)
                {
                    TocarSilabaZe();
                    intervaloSilabaZe = .095f;
                }
            }
        }
        else
        {
            tempoLeituraZe -= dt;
            if(tempoLeituraZe<=.3f)legendaZe.alpha = Mathf.Max(0,tempoLeituraZe/.3f);
            if(tempoLeituraZe<=0)
            {
                txtFalaZe.text = "";legendaZe.alpha = 0;
                intervaloFalaZe = falaZePendenteUrgente ? .35f : 1.5f;
            }
        }
    }

    private void TocarSilabaZe()
    {
        float efeitos = AudioSettingsController.VolumeFala;
        if(efeitos<=.001f)return;
        if(silabasZe==null)
        {
            // Síntese original abstrata de quatro murmúrios "hm/bom"; sem voz gravada ou palavras.
            const int hz = 22050;
            silabasZe = new AudioClip[4];
            for(int v=0;v<silabasZe.Length;v++)
            {
                var data = new float[(int)(hz*.075f)];
                float fundamental = 112+v*9;
                for(int i=0;i<data.Length;i++)
                {
                    float t=i/(float)hz;
                    float env=Mathf.Pow(Mathf.Max(0,Mathf.Sin(Mathf.PI*i/(data.Length-1))),1.5f);
                    float onda=Mathf.Sin(2*Mathf.PI*fundamental*t)*.48f +
                        Mathf.Sin(2*Mathf.PI*fundamental*2*t)*.24f +
                        Mathf.Sin(2*Mathf.PI*fundamental*(v%2==0?5:6)*t)*.12f;
                    data[i]=onda*env*.65f;
                }
                silabasZe[v]=AudioClip.Create("SR_Ze_Murmurio_"+v,data.Length,1,hz,false);
                silabasZe[v].SetData(data,0);
            }
            vozFalaZe=gameObject.AddComponent<AudioSource>();
            vozFalaZe.playOnAwake=false;vozFalaZe.spatialBlend=0;
        }
        // Fonte separada: a fala nunca interrompe uma fonte de impacto das bolas.
        if(vozFalaZe.isPlaying)return;
        vozFalaZe.pitch=locutorAtualMarcus?.77f:1f;
        vozFalaZe.clip=silabasZe[(silabaZe++*3)%silabasZe.Length];
        vozFalaZe.volume=.20f*efeitos;
        vozFalaZe.Play();
    }

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
                float env = Mathf.Exp(-t * 135f) * Mathf.Min(1f, t * 8000f);
                float tone = Mathf.Sin(2f * Mathf.PI * 380f * t) * 0.35f + Mathf.Sin(2f * Mathf.PI * 1733f * t) * 0.12f;
                float noise = ((float)rng.NextDouble() * 2f - 1f) * 0.35f;
                data[i] = Mathf.Clamp((tone + noise) * env, -0.9f, 0.9f);
            }
            clipTacada = AudioClip.Create("SR_Sinuca_Tacada", data.Length, 1, hz, false);
            clipTacada.SetData(data, 0);
        }

        // 3. Batida abafada na tabela de borracha/madeira
        {
            var data = new float[(int)(hz * 0.11f)];
            var rng = new System.Random(2004);
            for (int i = 0; i < data.Length; i++)
            {
                float t = i / (float)hz;
                float env = Mathf.Exp(-t * 72f) * Mathf.Min(1f, t * 5000f);
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

    private static void GarantirColisoesGravadas()
    {
        if (clipsColisaoBola != null) return;
        clipsColisaoBola = new AudioClip[3];
        for (int i = 0; i < clipsColisaoBola.Length; i++)
        {
            string caminho = "Audio/Sinuca/Colisao_" + (i + 1).ToString("00");
            clipsColisaoBola[i] = Resources.Load<AudioClip>(caminho);
            if (clipsColisaoBola[i] == null)
                Debug.LogWarning("Sinuca: gravação de colisão ausente: " + caminho);
        }
    }

    private void TocarClipImpacto(AudioClip clip, float volumeBase, float pitch = 1f)
    {
        if (!Application.isPlaying || Time.timeScale == 0f || MenuPausa.Aberto || !Application.isFocused || clip == null) return;
        float volPref = AudioSettingsController.VolumeImpactos;
        if (volPref <= 0.001f || volumeBase <= 0.002f) return;
        if (ultimoImpacto.TryGetValue(clip, out float ultimo) && Time.unscaledTime - ultimo < 0.018f) return;
        ultimoImpacto[clip] = Time.unscaledTime;
        int indice = proximaVoz++ % vozesSinuca.Length;
        AudioSource src = vozesSinuca[indice];
        if (src == null)
        {
            src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 0f;
            vozesSinuca[indice] = src;
        }
        src.Stop();
        src.clip = clip;
        src.pitch = Mathf.Clamp(pitch, 0.94f, 1.06f);
        src.volume = Mathf.Clamp01(volumeBase * volPref);
        src.Play();
        EventosAudioDisparados++;
    }

    private void TocarSomTacada(float intensidade01)
    {
        GarantirClipsAudio();
        float v = Mathf.Lerp(0.28f, 0.66f, Mathf.Clamp01(intensidade01));
        TocarClipImpacto(clipTacada, v, Mathf.Lerp(0.95f, 1.08f, intensidade01));
    }

    private void TocarSomColisaoBola(float intensidade01)
    {
        float v = 0.70f * Mathf.Sqrt(Mathf.Clamp01(intensidade01));
        if (!Application.isPlaying || Time.timeScale == 0f || MenuPausa.Aberto || !Application.isFocused
            || v <= 0.002f || AudioSettingsController.VolumeImpactos <= 0.001f) return;
        // Um limite para toda a categoria evita empilhar as três variantes no mesmo contato.
        if (Time.unscaledTime - ultimoSomColisaoBola < 0.018f) return;
        GarantirColisoesGravadas();
        AudioClip clip = clipsColisaoBola[proximaColisaoBola++ % clipsColisaoBola.Length];
        if (clip == null) return;
        ultimoSomColisaoBola = Time.unscaledTime;
        // O impulso altera o volume; a gravação conserva o timbre e o pitch naturais.
        TocarClipImpacto(clip, v);
    }

    private void TocarSomTabela(float intensidade01)
    {
        GarantirClipsAudio();
        float v = 0.35f * Mathf.Sqrt(Mathf.Clamp01(intensidade01));
        TocarClipImpacto(clipTabela, v, Mathf.Lerp(0.92f, 1.04f, intensidade01));
    }

    private void TocarSomCacapa()
    {
        GarantirClipsAudio();
        TocarClipImpacto(clipCacapa, 0.40f, 0.98f);
    }

    private void TocarSomGiz(float volume = 0.16f)
    {
        GarantirClipsAudio();
        TocarClipImpacto(clipGiz, volume, 1.0f);
    }

    private void MostrarModal(string titulo, string corpo, string txtPrimario, UnityEngine.Events.UnityAction acaoPrimaria,
        string txtSecundario, UnityEngine.Events.UnityAction acaoSecundaria)
    {
        if(escudoModal!=null)escudoModal.SetActive(true);
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

// Reações transitórias da partida: não alteram flags, pistas, física ou saves.
internal sealed class SinucaZeComments
{
    internal readonly struct Reaction
    {
        public readonly string Text;
        public readonly bool Urgent;
        public Reaction(string text, bool urgent=false){Text=text;Urgent=urgent;}
    }
    private readonly int[] variation = new int[4];
    private readonly int[] milestone = new int[2];
    private int dryShots;
    private static readonly string[] MarcusPots = {
        "Essa foi bem. Vamos ver a próxima.",
        "Tá com a mão firme, hein?",
        "Não te apressa. Ainda tem mesa.",
        "Boa bola. Mas a partida não acabou."
    };
    private static readonly string[] ZePots = {
        "Essa entrou limpa.",
        "Ainda sei o caminho dessas caçapas.",
        "Pois é. Mais uma.",
        "Agora deixa eu ver a próxima."
    };
    private static readonly string[] MarcusFouls = {
        "Com pressa tu só me ajuda.",
        "Olha a branca, Marcus.",
        "Respira antes de bater."
    };
    private static readonly string[] ZeFouls = {
        "Errei a mão. Joga.",
        "Essa eu te entreguei.",
        "Peguei mal nessa."
    };
    public void ResetSeries(){Array.Clear(variation,0,variation.Length);ResetHand();}
    public void ResetHand(){Array.Clear(milestone,0,milestone.Length);dryShots=0;}
    private Reaction Next(int category,string[] lines)
        => new Reaction(lines[variation[category]++%lines.Length]);

    public Reaction React(SinuquinhaRules rules,SinuquinhaRules.Result result,bool marcusShot)
    {
        if(rules.Finished)return default;
        if(result.revertShot)return new Reaction("Eu avisei. A marcada fica na mesa.",true);
        // A falta tem precedência: não elogiar uma conversão anulada pela branca.
        if(result.foul)return Next(marcusShot?2:3,marcusShot?MarcusFouls:ZeFouls);
        if(result.credited>0 && rules.MarcusGroup!=0)
        {
            int side=marcusShot?0:1;
            int group=marcusShot?rules.MarcusGroup:3-rules.MarcusGroup;
            int remaining=0;
            for(int n=1;n<16;n++)if(rules.Present[n]&&SinuquinhaRules.Group(n)==group)remaining++;
            int stage=remaining==0?2:remaining<=2?1:0;
            if(stage>milestone[side])
            {
                milestone[side]=stage;
                return new Reaction(marcusShot ?
                    (stage==2?"Só a preta agora. Eu lembro do combinado.":"Tá chegando, Marcus. Não perde a mão agora.") :
                    (stage==2?"Falta a preta. Fica de olho.":"Tá ficando pouca bola pra mim. Cuida essa próxima."),true);
            }
            return Next(marcusShot?0:1,marcusShot?MarcusPots:ZePots);
        }
        // Espaço para silêncio: não comentar toda defesa ou tacada sem conversão.
        if(++dryShots%3!=0)return default;
        return new Reaction(marcusShot?"Não caiu. Agora é comigo.":"Ficou pra ti. Vamos ver.");
    }
}

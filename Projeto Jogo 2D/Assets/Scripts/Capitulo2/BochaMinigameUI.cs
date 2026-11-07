using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed partial class BochaMinigameUI : MonoBehaviour
{
    private static BochaMinigameUI instance;
    public static bool IsShowing => instance != null && instance.raiz != null && instance.raiz.activeSelf;
    public static BochaMinigameUI Instance
    {
        get
        {
            if (instance != null) return instance;
            var host = new GameObject("BochaMinigameUI"); DontDestroyOnLoad(host);
            instance = host.AddComponent<BochaMinigameUI>(); instance.Construir(); return instance;
        }
    }
    private enum Fase { Mira, Carregando, Rolagem, Tonando, Resultado }
    private Fase fase;
    private GameObject raiz, modal;
    private CanvasGroup hudEntrada;
    private RectTransform cancha;
    private BochaCourtView vista;
    private BochaMapGraphic minimapa;
    private TextMeshProUGUI placar, estado, estoque, medida, percepcao, numeroMao, forcaLabel, cameraLabel, gestoLabel, controls;
    private readonly List<Image> typeButtons = new List<Image>();
    private readonly List<TextMeshProUGUI> typeLabels = new List<TextMeshProUGUI>();
    private BochaMatch partida;
    private BochaSimulation sim;
    private readonly System.Random rng = new System.Random();
    private Action aoVencer;
    private bool pratica, aiPreparada, efeitoMao, arrastando, tecladoCarregando;
    private float xLancamento, forca = .04f, acumulador, quieto, esperaIA, ultimoImpacto, cargaTempo, distanciaMira = 12.65f;
    private int ponteiroCarga;
    private const float DuracaoGesto = .30f;
    private float recuoMouse, miraGesto, tempoSoltura = DuracaoGesto, anguloMao, tamanhoMao = 190;
    private Vector2 origemSoltura, impulsoVisual;
    private float anguloSoltura, tamanhoSoltura;
    private int frameCaptura=-1;
    private RawImage maoBocha;
    private Vector2 posMao;
    private bool cargaMouseAnterior;
    private Vector2 direcao = Vector2.up;
    private BochaThrowType tipo;
    private BochaShot shotIA;
    private AudioSource efeitos;
    private readonly AudioSource[] vozesBocha = new AudioSource[6];
    private readonly AudioClip[] batidasBocha = new AudioClip[3];
    private AudioSource rolagem;
    private AudioClip soltura, madeira, saibro;
    private int variacaoBatida;
    private float ultimoSomColisaoBola = -10f;
    private RectTransform powerFill;
    private Image powerColor;
    private int larguraTela, alturaTela;
    private static readonly Color Ink = new Color(.055f,.065f,.055f,.96f);
    private static readonly Color Gold = new Color(.83f,.68f,.38f);

    public void Abrir(Action vitoria)
    {
        PrepararSala(vitoria);
        MostrarEscolhaInicial();
    }

    internal void AbrirDaSociedade(Action vitoria)
    {
        if (IsShowing || SceneFader.IsTransitioning) return;
        var camera = Camera.main;
        if (camera == null)
        {
            Abrir(vitoria);
            return;
        }

        vista.Preaquecer();
        var luz = camera.WorldToScreenPoint(SantaRitaBochaAmbiente.PendenteDaEntrada);
        var foco = new Vector2(Mathf.Clamp01(luz.x / Screen.width), Mathf.Clamp01(luz.y / Screen.height));
        SceneFader.Instance.TransicionarParaMinigame(foco, Resources.Load<Texture2D>("Bocha/SaibroCancha"), () =>
        {
            PrepararSala(vitoria);
            hudEntrada.alpha = 0;
            hudEntrada.interactable = hudEntrada.blocksRaycasts = false;
        }, vista.OlharPendente, () =>
        {
            RestaurarHud();
            if (IsShowing) MostrarEscolhaInicial();
        });
    }

    private void PrepararSala(Action vitoria)
    {
        larguraTela = Screen.width; alturaTela = Screen.height; aoVencer = vitoria;
        sim = null; partida = null; raiz.SetActive(true); vista.ResetWalking(); vista.Room.ReiniciarTonao(); LimparFalaTonao(); vista.SetState(null, default, false); minimapa.Simulation = null; minimapa.Refresh();
    }

    private void MostrarEscolhaInicial()
    {
        MostrarModal("SOCIEDADE BEIRA-RIO · BOCHA",
            "A madeira fecha a cancha de saibro. Quatro bochas de cada lado; ganha quem chegar a seis pontos.\n\n" +
            "PONTO para encostar. RAFA para uma batida rasteira. VOLO para lançar por cima. Consulte as posições no minimapa ou nas outras vistas.\n\n" +
            "A prática não grava progresso.",
            ("PRATICAR", () => Comecar(true)), ("DUELAR ATÉ 6 PONTOS", () => Comecar(false)), ("VOLTAR AO SALÃO", Fechar));
    }

    public void IniciarPraticaOpcional()
    {
        Abrir(null);
        Comecar(true);
    }

    private void Construir()
    {
        Canvas canvas = UIFactory.CreateCanvas(transform,"BochaCanvas",8850);
        canvas.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        raiz = Painel(canvas.transform,"CanchaRaiz",Vector2.zero,new Vector2(640,360),Ink).gameObject;
        var background = Painel(raiz.transform,"Fundo",Vector2.zero,new Vector2(4000,4000),new Color(.025f,.03f,.025f,1));
        background.SetAsFirstSibling();
        var viewport = Painel(raiz.transform,"JanelaDaCancha",Vector2.zero,new Vector2(640,360),Color.clear);
        UIFactory.Stretch(viewport);
        var go = new GameObject("CanchaPerspectiva",typeof(RectTransform),typeof(CanvasRenderer),typeof(BochaCourtView));
        go.transform.SetParent(viewport,false); cancha = (RectTransform)go.transform; UIFactory.Stretch(cancha); vista = go.GetComponent<BochaCourtView>();
        vista.Pressed = IniciarArraste; vista.Dragged = Arrastar; vista.Released = SoltarArraste; vista.Hovered = MoverMira;
        vista.Scrolled = d => {};
        vista.WalkingClicked = () => {vista.CaptureMouse();frameCaptura=Time.frameCount;};

        var handGo=new GameObject("MaoBochaMarcus",typeof(RectTransform),typeof(CanvasRenderer),typeof(RawImage));
        handGo.transform.SetParent(viewport,false);maoBocha=handGo.GetComponent<RawImage>();
        maoBocha.texture=Resources.Load<Texture2D>("Bocha/MaoMarcusBocha");maoBocha.raycastTarget=false;
        maoBocha.rectTransform.localScale = new Vector3(-1, 1, 1);
        maoBocha.rectTransform.sizeDelta=Vector2.one*tamanhoMao;posMao=new Vector2(133,-99);maoBocha.rectTransform.anchoredPosition=posMao;
        maoBocha.gameObject.SetActive(false);
        int inicioHud = raiz.transform.childCount;
        Painel(raiz.transform,"Cabecalho",new Vector2(0,160),new Vector2(640,40),Ink);
        Botao(raiz.transform,"REGRAS",new Vector2(-285,160),new Vector2(52,23),()=>Regras(0));
        Label(raiz.transform,"Titulo","SOCIEDADE BEIRA-RIO",new Vector2(-173,160),new Vector2(160,25),12);
        placar = Label(raiz.transform,"Placar","MARCUS 0 × 0 TONÃO",new Vector2(51,160),new Vector2(255,28),17);
        numeroMao = Label(raiz.transform,"Mao","ATÉ 6",new Vector2(234,160),new Vector2(64,25),12);
        Botao(raiz.transform,"II",new Vector2(292,160),new Vector2(34,23),Pausar);
        Painel(raiz.transform,"FaixaTurno",new Vector2(0,122),new Vector2(300,28),new Color(.07f,.115f,.1f,.96f));
        estado = Label(raiz.transform,"Estado","",new Vector2(0,122),new Vector2(292,26),13);

        var info = Painel(raiz.transform,"Leitura",new Vector2(-263,56),new Vector2(96,85),Ink);
        Label(info,"Titulo","DISTÂNCIA AO BALIM",new Vector2(0,31),new Vector2(91,15),10);
        medida = Label(info,"Medida","M —\nT —",new Vector2(0,6),new Vector2(88,40),13);
        estoque = Label(info,"Estoque","",new Vector2(0,-27),new Vector2(90,16),10);
        Botao(raiz.transform,"VER ALVO",new Vector2(-263,-2),new Vector2(94,19),()=>MudarVista(BochaCameraView.Alvo));
        Botao(raiz.transform,"VISTA GERAL",new Vector2(-263,-27),new Vector2(94,19),()=>MudarVista(BochaCameraView.Superior));
        Botao(raiz.transform,"LANÇAMENTO",new Vector2(-263,-52),new Vector2(94,19),()=>MudarVista(BochaCameraView.Lancamento));

        var mapaPainel = Painel(raiz.transform,"CanchaSuperior",new Vector2(273,0),new Vector2(82,220),Ink);
        Label(mapaPainel,"MapaTitulo","CANCHA",new Vector2(0,99),new Vector2(74,15),10);
        var map = new GameObject("Minimapa",typeof(RectTransform),typeof(CanvasRenderer),typeof(BochaMapGraphic),typeof(Button));
        map.transform.SetParent(mapaPainel,false);var mapRect=(RectTransform)map.transform;mapRect.sizeDelta=new Vector2(64,178);
        minimapa=map.GetComponent<BochaMapGraphic>();minimapa.raycastTarget=true;
        var mapButton=map.GetComponent<Button>();mapButton.targetGraphic=minimapa;mapButton.onClick.AddListener(()=>MudarVista(BochaCameraView.Superior));
        Label(mapaPainel,"MapaLegenda","<color=#58B5C2>M</color> · <color=#DC8857>T</color> · BALIM",new Vector2(0,-99),new Vector2(76,15),9f);

        percepcao = Label(raiz.transform,"Percepcao","",new Vector2(0,-99),new Vector2(390,24),11.5f);
        percepcao.color = new Color(.89f,.82f,.66f);
        ConstruirPercepcoes();
        Painel(raiz.transform,"Comandos",new Vector2(0,-145),new Vector2(640,70),Ink);
        string[] nomes={"1 · PONTO\nAproximação","2 · RAFA\nBatida rasteira","3 · VOLO\nLançamento alto"};
        for(int i=0;i<3;i++)
        {int index=i;var button=Botao(raiz.transform,nomes[i],new Vector2((i-1)*106,-132),new Vector2(100,31),()=>EscolherTipo((BochaThrowType)index));typeButtons.Add(button.GetComponent<Image>());typeLabels.Add(button.GetComponentInChildren<TextMeshProUGUI>());}
        forcaLabel=Label(raiz.transform,"ForcaLbl","FORÇA 4%",new Vector2(-232,-127),new Vector2(144,18),11);
        var sliderRoot=Painel(raiz.transform,"Forca",new Vector2(-232,-146),new Vector2(126,11),new Color(.22f,.23f,.19f));
        powerFill=Painel(sliderRoot,"Preenchimento",Vector2.zero,Vector2.zero,Gold);UIFactory.Stretch(powerFill);powerColor=powerFill.GetComponent<Image>();
        gestoLabel=Label(raiz.transform,"Gesto","SEGURE O CLIQUE\nSOLTE PARA LANÇAR",new Vector2(238,-134),new Vector2(143,34),12);
        cameraLabel=Label(raiz.transform,"CameraAtual","",new Vector2(0,-111),new Vector2(360,14),9.5f);
        controls = Label(raiz.transform,"Controles","A/D: posição · Mouse / ← →: mira · Clique/Espaço: segure e solte · Direito/C: cancelar · V: vistas",new Vector2(0,-167),new Vector2(624,20),11);
        ConstruirFalaTonao();
        var filhosHud = new List<Transform>();
        for (int i = inicioHud; i < raiz.transform.childCount; i++) filhosHud.Add(raiz.transform.GetChild(i));
        var camadaHud = new GameObject("HudBocha", typeof(RectTransform), typeof(CanvasGroup));
        camadaHud.transform.SetParent(raiz.transform, false);
        UIFactory.Stretch((RectTransform)camadaHud.transform);
        hudEntrada = camadaHud.GetComponent<CanvasGroup>();
        foreach (var filho in filhosHud) filho.SetParent(camadaHud.transform, false);
        efeitos = CriarFonte();
        madeira = Resources.Load<AudioClip>("Audio/Foley/bocha") ?? Capitulo2Audio.CriarImpactoMadeira();
        saibro = Resources.Load<AudioClip>("Audio/Foley/BochaQueda") ?? madeira;
        soltura = Resources.Load<AudioClip>("Audio/Foley/BochaSoltura");
        rolagem = CriarFonte();
        rolagem.clip = Resources.Load<AudioClip>("Audio/Foley/BochaRolagem");
        rolagem.loop = true;
        rolagem.volume = 0;
        for (int i = 0; i < batidasBocha.Length; i++)
            batidasBocha[i] = Resources.Load<AudioClip>("Audio/Sinuca/Colisao_0" + (i + 1)) ?? madeira;
        for (int i = 0; i < vozesBocha.Length; i++) vozesBocha[i] = CriarFonte();
        raiz.SetActive(false);
        raiz.AddComponent<ChuvaMinigame>();
    }
    private void Comecar(bool treino)
    {
        ReiniciarPercepcoes();
        pratica=treino;partida=new BochaMatch();
        falasTonao=0;intervaloTonao=0;LimparFalaTonao();NovaMao();
        MostrarModal(treino?"PRÁTICA · SEM APOSTA":"PRIMEIRA MÃO",
            "Marcus joga com as bochas azul-esverdeadas; Tonão, as cor de cobre. O balim é o menor, claro e marcado por um anel.\n\n"+
            "WASD para andar e mouse para olhar. Clique na cancha para capturar o mouse; Tab libera.\n\nNa base, segure o botão esquerdo: Marcus se inclina. Puxe o mouse para trás para dar força; avance para reduzir. Solte para lançar. Troque 1 Ponto / 2 Rafa / 3 Volo sem soltar. Direito cancela.\n\n"+
            "V alterna caminhada e vista geral. O minimapa e as vistas de consulta mostram as mesmas bolas. A leitura não gasta lançamento.",
            ("LER A CANCHA",()=>modal.SetActive(false)),("REGRAS COMPLETAS",()=>Regras(0)));
    }
    private void NovaMao()
    {
        sim=partida.CriarMao();sim.Impacto=ImpactoReal;sim.Contato=ContatoReal;
        acumulador=quieto=0;xLancamento=0;direcao=Vector2.up;distanciaMira=sim.bolas[0].pos.y-.65f;efeitoMao=false;tipo=BochaThrowType.Ponto;
        CancelarCarga();vista.ResetWalking();vista.Room.ReiniciarTonao();LimparFalaTonao();intervaloTonao=0;percepcao.text="Piso uniforme de saibro. Observe o peso e a rolagem.";
        ProximoTurno();AtualizarTela();
    }
    private void ProximoTurno()
    {
        quieto=0;forca=.04f;aiPreparada=false;arrastando=tecladoCarregando=false;
        if(!sim.bolas[0].valida||partida.Proximo(sim)==BochaSide.Nenhum){ResolverMao();return;}
        fase=partida.Proximo(sim)==BochaSide.Marcus?Fase.Mira:Fase.Tonando;esperaIA=.8f;
        if (!pratica && fase == Fase.Mira) agendaBocha.TurnStarted();
        // Keep the player at their actual standing position between turns.
    }
    private bool PodeConsultar()=>IsShowing&&!MenuPausa.Aberto&&Time.timeScale>0&&!PayphoneCheckpoint.IsSaveUIOpen&&(modal==null||!modal.activeSelf)&&partida!=null;
    private bool PodeMirar()=>PodeConsultar()&&(fase==Fase.Mira||fase==Fase.Carregando);
    private void Update()
    {
        if(!IsShowing || SceneFader.IsTransitioning)return;
        if(MenuPausa.Aberto||PayphoneCheckpoint.IsSaveUIOpen||Time.timeScale==0)
        {CancelarCargaAoSuspender();return;}
        if(MenuPausa.ClosedFrame==Time.frameCount)return;
        if(larguraTela!=Screen.width||alturaTela!=Screen.height)
        {larguraTela=Screen.width;alturaTela=Screen.height;CancelarCarga();vista.ReleaseMouse();}
        var k=Keyboard.current;var mouse=Mouse.current;
        if(k!=null&&k.escapeKey.wasPressedThisFrame)
        {if(modal==null||!modal.activeSelf)Pausar();return;}
        if (modal != null && modal.activeSelf)
        {
            dialogoMarcus.Suspender();
            SuspenderFalaTonao();
            return;
        }
        if(!Application.isFocused){CancelarCargaAoSuspender();return;}
        if((mouse!=null&&mouse.rightButton.wasPressedThisFrame)||(k!=null&&k.cKey.wasPressedThisFrame))
        {CancelarCarga();vista.SetView(BochaCameraView.Caminhando);vista.CaptureMouse();}
        if(k!=null&&k.tabKey.wasPressedThisFrame)
        {CancelarCarga();if(Cursor.lockState==CursorLockMode.Locked)vista.ReleaseMouse();else {vista.CaptureMouse();frameCaptura=Time.frameCount;}}
        if(k!=null&&k.vKey.wasPressedThisFrame)MudarVista(vista.View==BochaCameraView.Superior?BochaCameraView.Caminhando:BochaCameraView.Superior);
        if(k!=null&&k.fKey.wasPressedThisFrame){MudarVista(BochaCameraView.Caminhando);vista.CaptureMouse();frameCaptura=Time.frameCount;}
        if(k!=null&&PodeMirar())EntradaTeclado(k);
        if(arrastando&&mouse!=null)
        {
            if(Cursor.lockState!=CursorLockMode.Locked){CancelarCarga();}
            else
            {
                if(Time.frameCount!=frameCaptura)MoverGesto(mouse.delta.ReadValue());
                if(mouse.leftButton.wasReleasedThisFrame)FinalizarGesto();
                else if(!mouse.leftButton.isPressed)CancelarCarga();
            }
        }
        else if(vista.View==BochaCameraView.Caminhando)
        {
            if(k!=null&&Cursor.lockState==CursorLockMode.Locked)
            {
                var axis=new Vector2((k.dKey.isPressed?1:0)-(k.aKey.isPressed?1:0),(k.wKey.isPressed?1:0)-(k.sKey.isPressed?1:0));
                var look=mouse!=null&&Time.frameCount!=frameCaptura?mouse.delta.ReadValue():Vector2.zero;
                vista.Walk(axis,look,Time.unscaledDeltaTime,k.leftShiftKey.isPressed);
            }
            // Locked mouse bypasses UI pointer events. Never consumes a ball just to capture the cursor.
            if(mouse!=null&&mouse.leftButton.wasPressedThisFrame&&Cursor.lockState==CursorLockMode.Locked&&Time.frameCount!=frameCaptura)
                IniciarGesto();
        }
        float dt = Time.deltaTime;
        AtualizarPercepcoes(dt);
        if (fase == Fase.Tonando && !aiPreparada)
        {
            esperaIA -= dt;
            if (esperaIA <= 0)
            {
                shotIA = BochaAI.Escolher(sim, 4 - partida.usadas[1], rng);
                aiPreparada = true;
                percepcao.text = shotIA.intencao;
                vista.Room.PrepararTonao(shotIA);
                ComentarJogadaTonao(shotIA);
            }
        }
        bool soltarTonao = vista.Room.AvancarTonao(dt);
        if (caindo) AtualizarQueda(dt);
        else if(fase==Fase.Rolagem)
        {
            acumulador+=Mathf.Min(Time.deltaTime,.2f);
            while (acumulador >= BochaSimulation.Passo)
            {
                sim.Step();
                vista.RecordContacts();
                acumulador -= BochaSimulation.Passo;
                if (TentarAbrirBuraco()) break;
            }
            if (!caindo)
            {
                quieto=sim.IsMoving?0:quieto+dt;
                if (quieto >= .45f)
                {
                    ReagirAoRepouso();
                    ProximoTurno();
                }
            }
        }
        else if (fase == Fase.Tonando && soltarTonao)
            EfetuarLancamento(BochaSide.Tonao, shotIA);

        dialogoMarcus.Atualizar(Mathf.Min(dt, .1f));
        if (dialogoMarcus.Ocupada || caindo) SuspenderFalaTonao();
        else AtualizarFalaTonao(Mathf.Min(dt, .1f));
        vista.Room.SetThrowPose(arrastando,forca,Time.unscaledDeltaTime);
        vista.Tick(Time.deltaTime);AtualizarMao();AtualizarTela();
        if (!caindo) AtualizarRolagem();
    }
    // Distance, never duration or velocity: returning the mouse forward reduces power.
    public static float ForcaPorRecuo(float recuo)=>Mathf.Clamp01(recuo);
    private void IniciarGesto()
    {
        if(!PodeMirar()||arrastando||!vista.Room.AtLaunch||vista.View==BochaCameraView.Superior)return;
        xLancamento=Mathf.Clamp(vista.Room.StandingPosition.x,-2.6f,2.6f);
        miraGesto=Mathf.Clamp(Mathf.DeltaAngle(0,vista.Room.Yaw)*Mathf.Deg2Rad,-.58f,.58f);
        direcao=new Vector2(Mathf.Sin(miraGesto),Mathf.Cos(miraGesto));
        vista.SetView(BochaCameraView.Lancamento);Cursor.lockState=CursorLockMode.Locked;Cursor.visible=false;
        frameCaptura=Time.frameCount;arrastando=true;tecladoCarregando=false;recuoMouse=0;forca=0;fase=Fase.Carregando;
    }
    private void MoverGesto(Vector2 delta)
    {
        if(!arrastando)return;
        recuoMouse=Mathf.Clamp01(recuoMouse-delta.y/Mathf.Max(140,Screen.height*.32f));
        forca=ForcaPorRecuo(recuoMouse);miraGesto=Mathf.Clamp(miraGesto+delta.x*.0011f,-.58f,.58f);
        direcao=new Vector2(Mathf.Sin(miraGesto),Mathf.Cos(miraGesto));
    }
    private void FinalizarGesto()
    {
        if(!arrastando)return;
        if(!PodeMirar()||forca<.025f){CancelarCarga();vista.SetView(BochaCameraView.Caminhando);return;}
        EfetuarLancamento(BochaSide.Marcus,Preparar(),animarMao:true);
    }
    public void CancelarCargaAoSuspender(){CancelarCarga();PararFoley();audioAlucinacao.Stop();dialogoMarcus.Suspender();SuspenderFalaTonao();vista.ReleaseMouse();}
    private void AtualizarMao()
    {
        float dt = Time.unscaledDeltaTime;
        bool soltando = tempoSoltura < DuracaoGesto && vista.View == BochaCameraView.Caminhando;
        bool segurando = PodeMirar() && vista.Room.AtLaunch &&
            (vista.View == BochaCameraView.Caminhando || vista.View == BochaCameraView.Lancamento);
        maoBocha.gameObject.SetActive(segurando || soltando);
        if (!maoBocha.gameObject.activeSelf) return;

        float opacidade = 1;
        int quadro = 0;
        if (soltando)
        {
            float t = tempoSoltura / DuracaoGesto;
            float avancar = Mathf.SmoothStep(0, 1, t / .40f);
            float recolher = Mathf.SmoothStep(0, 1, (t - .42f) / .58f);
            posMao = origemSoltura + impulsoVisual * avancar + new Vector2(32, -104) * recolher;
            anguloMao = anguloSoltura + 16 * avancar - 12 * recolher;
            tamanhoMao = tamanhoSoltura * (1 - .13f * avancar);
            opacidade = 1 - Mathf.SmoothStep(0, 1, (t - .64f) / .36f);
            quadro = t < .30f ? 1 : 2;
            tempoSoltura += dt;
        }
        else
        {
            float pull = arrastando ? forca : 0;
            float blend = 1 - Mathf.Exp(-dt * 19);
            posMao = Vector2.Lerp(posMao, new Vector2(133 + pull * 24, -99 - pull * 39), blend);
            anguloMao = Mathf.Lerp(anguloMao, -pull * 8, blend);
            tamanhoMao = Mathf.Lerp(tamanhoMao, 190 + pull * 19, blend);
        }
        maoBocha.uvRect = new Rect(quadro / 3f, 0, 1 / 3f, 1);
        var rect = maoBocha.rectTransform;
        rect.anchoredPosition = posMao + TremorMao();
        rect.localEulerAngles = new Vector3(0, 0, anguloMao);
        rect.sizeDelta = Vector2.one * tamanhoMao;
        Color luz = vista.Room.LuzDaMao(new Vector3(posMao.x / 440f, posMao.y / 420f, soltando ? .85f : .65f));
        luz.a = opacidade;
        maoBocha.color = luz;
    }
    private void EntradaTeclado(Keyboard k)
    {
        if(k.digit1Key.wasPressedThisFrame)EscolherTipo(BochaThrowType.Ponto);
        if(k.digit2Key.wasPressedThisFrame)EscolherTipo(BochaThrowType.Rafa);
        if(k.digit3Key.wasPressedThisFrame)EscolherTipo(BochaThrowType.Volo);
    }
    public static float ForcaNoTempo(float segundos)=>.04f+.96f*Mathf.PingPong(Mathf.Max(0,segundos)/1.5f,1);
    private BochaShot Preparar()
    {
        Vector2 mira = direcao;
        if (tremorMira != 0 && arrastando && !MenuPausa.ReduzirEfeitosIntensos)
        {
            float angulo = Mathf.Clamp(miraGesto + tremorMira, -.58f, .58f);
            mira = new Vector2(Mathf.Sin(angulo), Mathf.Cos(angulo));
        }
        return BochaLaunch.Preparar(tipo, xLancamento, mira, forca);
    }
    private void IniciarArraste(PointerEventData e)
    {
        if(e.button!=PointerEventData.InputButton.Left)return;
        if(Cursor.lockState!=CursorLockMode.Locked){vista.CaptureMouse();frameCaptura=Time.frameCount;return;}
        IniciarGesto();
    }
    private void Arrastar(PointerEventData e){}
    private void SoltarArraste(PointerEventData e){}
    private void MoverMira(PointerEventData e){}
    private void AtualizarMira(Vector2 screen,Camera camera)
    {
        if(!vista.CourtPoint(screen,camera,out var target))return;
        target=new Vector2(Mathf.Clamp(target.x,-2.8f,2.8f),Mathf.Clamp(target.y,1.2f,19.8f));
        var d=target-new Vector2(xLancamento,.65f);float a=Mathf.Clamp(Mathf.Atan2(d.x,d.y),-.58f,.58f);
        direcao=new Vector2(Mathf.Sin(a),Mathf.Cos(a));distanciaMira=Mathf.Clamp(d.magnitude,.8f,19.15f);
    }
    private void EscolherTipo(BochaThrowType value)
    {if(!PodeMirar())return;tipo=value;AtualizarTela();}
    private void MudarVista(BochaCameraView value)
    {if(!PodeConsultar())return;CancelarCarga();vista.SetView(value);AtualizarTela();}
    private void CancelarCarga()
    {
        bool wasPreparing=arrastando;arrastando=tecladoCarregando=false;recuoMouse=0;forca=0;
        tempoSoltura = DuracaoGesto;
        if (maoBocha != null) maoBocha.gameObject.SetActive(false);
        if(fase==Fase.Carregando)fase=Fase.Mira;
        if(wasPreparing&&vista!=null)vista.SetView(BochaCameraView.Caminhando);
    }
    // Entrada antiga preservada para verificações de integração já existentes.
    private void Lancar(BochaSide side,float x,Vector2 velocity)
    {EfetuarLancamento(side,new BochaShot{x=x,velocidade=velocity,giro=new Vector3(velocity.y,0,-velocity.x)/BochaSimulation.RaioBocha});}
    private void EfetuarLancamento(BochaSide side,BochaShot shot,bool animarMao=false)
    {
        if(modal!=null&&modal.activeSelf)return;
        if(!partida.Lancar(sim,side,shot))return;
        var bola = sim.bolas[sim.bolas.Count - 1];
        PrepararPercepcaoDoLancamento(side, bola);
        if (bola.valida)
        {
            float peso = Mathf.InverseLerp(1, BochaSimulation.VelocidadeMaxima, shot.velocidade.magnitude);
            TocarFoley(soltura, AudioSettingsController.GanhoTacada(peso), Mathf.Lerp(.95f, 1.06f, peso),
                PanBocha(bola.pos.x));
            if (animarMao)
            {
                tempoSoltura = 0;
                origemSoltura = posMao;
                anguloSoltura = anguloMao;
                tamanhoSoltura = tamanhoMao;
                impulsoVisual = new Vector2(-48 + shot.velocidade.normalized.x * 46,
                    shot.tipo == BochaThrowType.Volo ? 84 : Mathf.Lerp(43, 62, peso));
            }
        }
        arrastando=tecladoCarregando=false;fase=Fase.Rolagem;quieto=acumulador=0;
        if (side == BochaSide.Marcus)
        {
            vista.SetView(BochaCameraView.Caminhando);
            vista.CaptureMouse();
            frameCaptura = Time.frameCount;
        }
        AtualizarTela();
    }
    private void ResolverMao()
    {
        fase=Fase.Resultado;var r=partida.Resolver(sim);AtualizarTela();
        string detail=r.resumo+"\n\nMARCUS "+partida.pontos[0]+" × "+partida.pontos[1]+" TONÃO\nMedida entre superfícies; empate dentro de 1 cm.";
        if(pratica)MostrarModal("FIM DA PRÁTICA",detail+"\nO treino não salva nem concede ficha.",("COMEÇAR DUELO",()=>Comecar(false)),("VOLTAR AO SALÃO",Fechar));
        else if(partida.Vencedor==BochaSide.Marcus)MostrarModal("MARCUS VENCE A PARTIDA",detail+"\n\nTonão deixa a bocha no suporte. Pela primeira vez, não propõe outra tentativa.",("OUVIR TONÃO",()=>{var callback=aoVencer;aoVencer=null;Fechar();callback?.Invoke();}));
        else if(partida.Vencedor==BochaSide.Tonao)Derrota();
        else MostrarModal(r.anulada?"MÃO ANULADA":"CONTAGEM DA MÃO",detail,(r.anulada?"REPETIR A MÃO":"PRÓXIMA MÃO",()=>{modal.SetActive(false);NovaMao();}),("REVER REGRAS",()=>Regras(0,true)));
    }
    private void Derrota()
    {
        CancelarCarga();fase=Fase.Resultado;
        var opts=new List<(string,Action)>();
        if(CheckpointSystem.HasCheckpoint)opts.Add(("ESCOLHER SAVE",()=>{modal.SetActive(false);PayphoneCheckpoint.AbrirSelecaoCarregar(Derrota,Fechar);}));
        opts.Add(("MENU PRINCIPAL",()=>{Fechar();SceneFader.Instance.LoadScene("MainMenu");}));
        MostrarModal("TONÃO VENCE · A CONVERSA SE FECHA","A chuva volta a ocupar o silêncio. Marcus não consegue sustentar a pergunta.\n\n"+(CheckpointSystem.HasCheckpoint?"Escolha qual registro manual deseja carregar. A partida não criou checkpoint.":"Não existe save manual válido. Nenhum ponto de recuperação foi criado."),opts.ToArray());
    }
    private void Pausar()
    {
        if(!IsShowing||partida==null||MenuPausa.Aberto||MenuPausa.ClosedFrame==Time.frameCount)return;
        CancelarCargaAoSuspender();
        var pausa=FindAnyObjectByType<MenuPausa>();if(pausa==null)pausa=gameObject.AddComponent<MenuPausa>();pausa.Abrir();
    }
    private void OnApplicationFocus(bool focus)
    {if(!focus&&IsShowing)CancelarCargaAoSuspender();}
    private void Regras(int pagina,bool resultado=false)
    {
        CancelarCarga();
        string text=pagina==0?
            "VARIANTE DA SOCIEDADE\nQuatro bochas por lado, até seis pontos. Depois da primeira, joga quem está mais distante enquanto tiver estoque. Se as melhores empatam em até 1 cm, joga quem não lançou por último.\n\nAo repousar, cada bocha mais próxima que a melhor rival vale um ponto. Empate das melhores: zero. Prática não concede ficha nem salva.":
            "PONTO: aproximação. RAFA: batida rasteira. VOLO: arco, queda e quique; o anel indica a primeira queda em piso livre, antes de colisões.\n\nMire com o cursor: a linha cresce até a posição apontada. A força define o alcance. Segure o clique (ou Espaço): a barra sobe e desce. Solte para lançar. A/D: posição; ← →: mira; C/direito: cancelar. V: vistas; roda: zoom. Soltar fora da cancha cancela.\n\nSaibro uniforme. Guias e cabeceiras devolvem bochas e balim, com perda de velocidade. Rastros não alteram atrito.";
        MostrarModal(pagina==0?"REGRAS DA MÃO":"JOGADAS E CONTROLES",text,(pagina==0?"JOGADAS E CONTROLES":"PONTUAÇÃO",()=>Regras(1-pagina,resultado)),(resultado?"PRÓXIMA MÃO":"VOLTAR",()=>{modal.SetActive(false);if(resultado)NovaMao();}));
    }
    private void AtualizarTela()
    {
        if(sim==null||partida==null)return;
        placar.text="MARCUS "+partida.pontos[0]+" × "+partida.pontos[1]+" TONÃO";
        numeroMao.text=(pratica?"PRÁTICA":"MÃO "+(partida.Mao+(partida.Resolvida?0:1)));
        estado.text=fase==Fase.Rolagem?"BOLAS EM MOVIMENTO":fase==Fase.Tonando?"TONÃO PREPARA A JOGADA":fase==Fase.Resultado?"MÃO RESOLVIDA":fase==Fase.Carregando?"SOLTE O CLIQUE · "+tipo.ToString().ToUpperInvariant():"SUA VEZ · "+tipo.ToString().ToUpperInvariant();
        estoque.text="M: "+(4-partida.usadas[0])+"  T: "+(4-partida.usadas[1])+" BOCHAS";
        medida.text=sim.IsMoving?"AGUARDE\nO REPOUSO":"M  "+TextoDistancia(BochaMatch.MelhorDistancia(sim,BochaSide.Marcus))+"\nT  "+TextoDistancia(BochaMatch.MelhorDistancia(sim,BochaSide.Tonao));
        bool descendo=cargaTempo%3>1.5f;
        forcaLabel.text="RECUO "+Mathf.RoundToInt(forca*100)+"%";
        powerFill.anchorMax=new Vector2(forca,1);powerColor.color=Color.Lerp(new Color(.36f,.62f,.57f),new Color(.92f,.59f,.25f),forca);
        for(int i=0;i<typeButtons.Count;i++){typeButtons[i].GetComponent<Button>().interactable=PodeMirar();typeButtons[i].color=(int)tipo==i?new Color(.27f,.36f,.30f):new Color(.13f,.17f,.14f);typeLabels[i].color=(int)tipo==i?new Color(.98f,.87f,.61f):new Color(.74f,.76f,.67f);}
        bool walking=vista.View!=BochaCameraView.Superior;
        bool inspect=vista.View!=BochaCameraView.Lancamento;
        foreach(string name in new[]{"Leitura","VER ALVO","VISTA GERAL","LANÇAMENTO","FaixaTurno","Estado","Comandos"})
        {var element=controls.transform.parent.Find(name);if(element!=null)element.gameObject.SetActive(!walking);}
        for(int i=0;i<typeButtons.Count;i++)typeButtons[i].gameObject.SetActive(arrastando);
        forcaLabel.gameObject.SetActive(arrastando);powerFill.parent.gameObject.SetActive(arrastando);
        controls.text=walking?"WASD: andar · Mouse: olhar · Segure clique na base: preparar · Puxe: força · Solte: lançar · Tab: mouse · Esc: pausa":"A/D: posição · Mouse: mira · Clique/Espaço: força · F: caminhar · V: vista geral";
        bool legendaVisivel = SubtitleSettingsController.LegendasAtivas && (dialogoMarcus.Ocupada || dialogoTonao.Ocupada);
        percepcao.enabled = !walking && !legendaVisivel;
        gestoLabel.enabled = !walking && PodeMirar();
        gestoLabel.text=inspect?"LANÇAMENTO / V\nPARA VOLTAR À MIRA":fase==Fase.Carregando?"FORÇA SOBE E DESCE\nSOLTE PARA LANÇAR":"SEGURE O CLIQUE\nSOLTE PARA LANÇAR";
        cameraLabel.enabled = !legendaVisivel;
        cameraLabel.text=vista.View==BochaCameraView.Superior?"VISTA SUPERIOR · LANÇAMENTO ou V para voltar":vista.View==BochaCameraView.Alvo?"VISTA DO BALIM · roda ajusta o zoom":"MIRA "+distanciaMira.ToString("F1")+" m · força define alcance · roda: zoom";
        if(walking)
        {
            cameraLabel.text=arrastando?"PUXE O MOUSE PARA TRÁS · 1 PONTO / 2 RAFA / 3 VOLO · DIREITO CANCELA":
                !PodeMirar()?string.Empty:vista.Room.AtLaunch?"SEGURE O BOTÃO ESQUERDO PARA PREPARAR":"SUA VEZ · VOLTE À BASE PARA LANÇAR";
            cameraLabel.rectTransform.anchoredPosition=new Vector2(0,-105);
        }
        else cameraLabel.rectTransform.anchoredPosition=new Vector2(0,-111);
        vista.SetState(sim,Preparar(),arrastando,distanciaMira);minimapa.Simulation=sim;
        minimapa.BochaOculta = caindo ? bochaAlucinada.id : -1;
        minimapa.Refresh();
    }
    private static string TextoDistancia(float d)=>float.IsPositiveInfinity(d)?"—":d<1?(d*100).ToString("F0")+" cm":d.ToString("F2")+" m";
    private void RestaurarHud()
    {
        hudEntrada.alpha = 1;
        hudEntrada.interactable = hudEntrada.blocksRaycasts = true;
    }

    public void Fechar(){RestaurarHud();StopAllCoroutines();ReiniciarPercepcoes();PararFoley();LimparFalaTonao();if(efeitos!=null)efeitos.Stop();CancelarCarga();aoVencer=null;if(modal!=null)modal.SetActive(false);raiz.SetActive(false);}
    private void ContatoReal(BochaContact contact)
    {
        float pan = PanBocha(contact.ponto.x);
        float forcaBatida = Mathf.InverseLerp(.08f, 9, contact.velocidade);
        float intensidade = AudioSettingsController.GanhoColisaoBola(forcaBatida);
        intensidade *= Mathf.Lerp(1, .66f, Mathf.InverseLerp(2, 20, Mathf.Abs(contact.ponto.z - vista.Room.StandingPosition.z)));
        if (contact.tipo == BochaContactType.Madeira)
            TocarFoley(madeira, intensidade * .50f, .97f, pan);
        else if (contact.tipo == BochaContactType.Saibro)
        {
            if (contact.velocidade > .24f)
                TocarFoley(saibro, intensidade * (.40f / .70f), Mathf.Lerp(.93f, 1.08f, forcaBatida), pan);
        }
        else
        {
            if (Time.unscaledTime - ultimoSomColisaoBola < AudioSettingsController.IntervaloColisaoBola) return;
            if (intensidade <= .002f || AudioSettingsController.VolumeImpactos <= .001f) return;
            ultimoSomColisaoBola = Time.unscaledTime;
            int n = variacaoBatida++ % batidasBocha.Length;
            TocarFoley(batidasBocha[n], intensidade, 1f, pan);
        }
    }
    private AudioSource CriarFonte()
    {
        var source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0;
        return source;
    }
    private static float PanBocha(float x) => Mathf.Clamp(x / BochaSimulation.MeiaLargura, -1, 1) * .35f;
    private void TocarFoley(AudioClip clip, float volume, float pitch, float pan)
    {
        volume *= AudioSettingsController.VolumeImpactos;
        if (clip == null || volume < .001f || !PodeConsultar()) return;
        foreach (var voz in vozesBocha)
        {
            if (voz.isPlaying) continue;
            voz.clip = clip;
            voz.pitch = pitch;
            voz.panStereo = pan;
            voz.volume = volume;
            voz.Play();
            break;
        }
    }
    private void AtualizarRolagem()
    {
        if (rolagem.clip == null) return;
        float peso = 0, soma = 0, pan = 0, aspereza = 0;
        foreach (var b in sim.bolas)
        {
            if (!b.valida || b.altura > .0001f || Mathf.Abs(b.vertical) > .01f) continue;
            float ritmo = Mathf.InverseLerp(.12f, 8, b.vel.magnitude);
            if (ritmo <= 0) continue;
            Vector3 contato = b.Velocidade + Vector3.Cross(b.giro, Vector3.down * b.raio);
            float deslize = Mathf.InverseLerp(.03f, 4, contato.magnitude);
            float ganho = Mathf.Sqrt(ritmo) * Mathf.Lerp(1, 1.55f, deslize) *
                (b.lado == BochaSide.Nenhum ? .51f : 1);
            ganho *= Mathf.Lerp(1, .56f, Mathf.InverseLerp(2, 20, Mathf.Abs(b.pos.y - vista.Room.StandingPosition.z)));
            peso = Mathf.Max(peso, ganho);
            soma += ganho;
            pan += PanBocha(b.pos.x) * ganho;
            aspereza = Mathf.Max(aspereza, Mathf.Max(ritmo, deslize));
        }
        float alvo = Mathf.Min(.40f, (peso + (soma - peso) * .32f) * .27f) * AudioSettingsController.VolumeImpactos;
        float blend = 1 - Mathf.Exp(-Time.unscaledDeltaTime * (alvo > rolagem.volume ? 21 : 13));
        rolagem.volume = Mathf.Lerp(rolagem.volume, alvo, blend);
        rolagem.pitch = Mathf.Lerp(rolagem.pitch, Mathf.Lerp(.68f, 1.27f, aspereza), blend);
        if (soma > 0) rolagem.panStereo = Mathf.Lerp(rolagem.panStereo, pan / soma, blend);
        if (rolagem.isPlaying && rolagem.volume < .004f && alvo < .004f) rolagem.Stop();
        else if (!rolagem.isPlaying && rolagem.volume >= .004f) rolagem.Play();
    }
    private void PararFoley()
    {
        if (rolagem == null) return;
        if (rolagem.isPlaying) rolagem.Stop();
        rolagem.volume = 0;
        foreach (var voz in vozesBocha) if (voz != null && voz.isPlaying) voz.Stop();
    }
    private void ImpactoReal(float speed)
    {
        if(Time.unscaledTime-ultimoImpacto<.07f)return;ultimoImpacto=Time.unscaledTime;
        if(partida!=null&&partida.Mao>0&&!efeitoMao){efeitoMao=true;StartCoroutine(RespostaAbafada());}
    }
    private IEnumerator RespostaAbafada()
    {
        float elapsed=0;while(elapsed<.58f){if(PodeConsultar()&&Application.isFocused&&agendaBocha.Active==Alucinacao.Nenhuma&&!dialogoMarcus.Ocupada)elapsed+=Time.deltaTime;yield return null;}
        percepcao.text="Um sopro abafado responde do outro lado da parede.";efeitos.pitch=1;
        efeitos.PlayOneShot(Capitulo2Audio.RespostaAbafada(),.28f*AudioSettingsController.VolumeEfeitos);
    }
    private void MostrarModal(string title,string text,params(string texto,Action acao)[] options)
    {
        CancelarCargaAoSuspender();
        if(modal!=null){modal.SetActive(false);Destroy(modal);}
        // Camada inteira bloqueia clique na força/câmera atrás do painel.
        modal=Painel(raiz.transform,"ModalBocha",Vector2.zero,new Vector2(640,360),new Color(.02f,.025f,.02f,.76f)).gameObject;modal.GetComponent<Image>().raycastTarget=true;
        var frame=Painel(modal.transform,"Moldura",Vector2.zero,new Vector2(596,328),new Color(.39f,.33f,.21f));
        var paper=Painel(frame,"Painel",Vector2.zero,new Vector2(592,324),new Color(.075f,.09f,.075f,.995f));
        Label(paper,"Titulo",title,new Vector2(0,139),new Vector2(556,28),18);
        var body=Label(paper,"Texto",text,new Vector2(0,23),new Vector2(542,194),14.2f);body.alignment=TextAlignmentOptions.TopLeft;
        for(int i=0;i<options.Length;i++){var option=options[i];Botao(paper,option.texto,new Vector2(0,-81-i*27),new Vector2(436,23),()=>option.acao());}
    }
    private static RectTransform Painel(Transform parent,string name,Vector2 pos,Vector2 size,Color color)
    {
        var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);var r=(RectTransform)go.transform;r.anchoredPosition=pos;r.sizeDelta=size;var im=go.GetComponent<Image>();im.color=color;im.raycastTarget=false;return r;
    }
    private static TextMeshProUGUI Label(Transform parent,string name,string text,Vector2 pos,Vector2 size,float font)
    {var t=UIFactory.CreateLabel(parent,name,font);t.text=text;t.alignment=TextAlignmentOptions.Center;t.rectTransform.anchoredPosition=pos;t.rectTransform.sizeDelta=size;t.textWrappingMode=TextWrappingModes.Normal;return t;}
    private static GameObject Botao(Transform parent,string text,Vector2 pos,Vector2 size,Action click)
    {
        var r=Painel(parent,text,pos,size,new Color(.18f,.23f,.19f));var im=r.GetComponent<Image>();im.raycastTarget=true;var b=r.gameObject.AddComponent<Button>();b.targetGraphic=im;
        b.onClick.AddListener(()=>{EventSystem.current?.SetSelectedGameObject(null);click();});var t=Label(r,"Texto",text,Vector2.zero,size,12);t.raycastTarget=false;return r.gameObject;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class BochaMinigameUI : MonoBehaviour
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
    private RectTransform cancha;
    private BochaCourtView vista;
    private BochaMapGraphic minimapa;
    private TextMeshProUGUI placar, estado, estoque, medida, percepcao, numeroMao, forcaLabel, cameraLabel, gestoLabel;
    private readonly List<Image> typeButtons = new List<Image>();
    private readonly List<TextMeshProUGUI> typeLabels = new List<TextMeshProUGUI>();
    private BochaMatch partida;
    private BochaSimulation sim;
    private readonly System.Random rng = new System.Random();
    private Action aoVencer;
    private bool pratica, aiPreparada, efeitoMao, arrastando, tecladoCarregando;
    private float xLancamento, forca = .04f, acumulador, quieto, esperaIA, ultimoImpacto, ultimoContato, cargaTempo, distanciaMira = 12.65f;
    private int ponteiroCarga;
    private Vector2 direcao = Vector2.up;
    private BochaThrowType tipo;
    private BochaShot shotIA;
    private AudioSource efeitos;
    private AudioClip impacto, madeira, saibro;
    private RectTransform powerFill;
    private Image powerColor;
    private int larguraTela, alturaTela;
    private static readonly Color Ink = new Color(.055f,.065f,.055f,.96f);
    private static readonly Color Gold = new Color(.83f,.68f,.38f);

    public void Abrir(Action vitoria)
    {
        larguraTela = Screen.width; alturaTela = Screen.height; aoVencer = vitoria;
        sim = null; partida = null; raiz.SetActive(true); vista.SetState(null, default, false); minimapa.Simulation = null; minimapa.Refresh();
        MostrarModal("SOCIEDADE BEIRA-RIO · BOCHA",
            "A madeira fecha a cancha de saibro. Quatro bochas de cada lado; ganha quem chegar a seis pontos.\n\n" +
            "PONTO para encostar. RAFA para uma batida rasteira. VOLO para lançar por cima. Consulte as posições no minimapa ou nas outras vistas.\n\n" +
            "A prática não grava progresso.",
            ("PRATICAR", () => Comecar(true)), ("DUELAR ATÉ 6 PONTOS", () => Comecar(false)), ("VOLTAR AO SALÃO", Fechar));
    }
    private void Construir()
    {
        Canvas canvas = UIFactory.CreateCanvas(transform,"BochaCanvas",8850);
        canvas.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        raiz = Painel(canvas.transform,"CanchaRaiz",Vector2.zero,new Vector2(640,360),Ink).gameObject;
        var background = Painel(raiz.transform,"Fundo",Vector2.zero,new Vector2(4000,4000),new Color(.025f,.03f,.025f,1));
        background.SetAsFirstSibling();
        var viewport = Painel(raiz.transform,"JanelaDaCancha",new Vector2(0,10),new Vector2(640,286),Color.clear);
        viewport.gameObject.AddComponent<RectMask2D>();
        var go = new GameObject("CanchaPerspectiva",typeof(RectTransform),typeof(CanvasRenderer),typeof(BochaCourtView));
        go.transform.SetParent(viewport,false); cancha = (RectTransform)go.transform; UIFactory.Stretch(cancha); vista = go.GetComponent<BochaCourtView>();
        vista.Pressed = IniciarArraste; vista.Dragged = Arrastar; vista.Released = SoltarArraste; vista.Hovered = MoverMira;
        vista.Scrolled = d => { if (PodeConsultar()) vista.ChangeZoom(d); };

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

        Label(raiz.transform,"MapaTitulo","CANCHA",new Vector2(279,114),new Vector2(66,16),10);
        var map = new GameObject("Minimapa",typeof(RectTransform),typeof(CanvasRenderer),typeof(BochaMapGraphic),typeof(Button));
        map.transform.SetParent(raiz.transform,false);var mapRect=(RectTransform)map.transform;mapRect.anchoredPosition=new Vector2(279,32);mapRect.sizeDelta=new Vector2(66,140);
        minimapa=map.GetComponent<BochaMapGraphic>();minimapa.raycastTarget=true;
        var mapButton=map.GetComponent<Button>();mapButton.targetGraphic=minimapa;mapButton.onClick.AddListener(()=>MudarVista(BochaCameraView.Superior));
        Label(raiz.transform,"MapaLegenda","<color=#268796>M</color> · <color=#BA5224>T</color> · BALIM\nBASE EMBAIXO\nCLIQUE: AMPLIAR",new Vector2(279,-57),new Vector2(75,36),9.5f);

        percepcao = Label(raiz.transform,"Percepcao","",new Vector2(0,-99),new Vector2(390,24),11.5f);
        percepcao.color = new Color(.89f,.82f,.66f);
        Painel(raiz.transform,"Comandos",new Vector2(0,-145),new Vector2(640,70),Ink);
        string[] nomes={"1 · PONTO\nAproximação","2 · RAFA\nBatida rasteira","3 · VOLO\nLançamento alto"};
        for(int i=0;i<3;i++)
        {int index=i;var button=Botao(raiz.transform,nomes[i],new Vector2((i-1)*106,-132),new Vector2(100,31),()=>EscolherTipo((BochaThrowType)index));typeButtons.Add(button.GetComponent<Image>());typeLabels.Add(button.GetComponentInChildren<TextMeshProUGUI>());}
        forcaLabel=Label(raiz.transform,"ForcaLbl","FORÇA 4%",new Vector2(-232,-127),new Vector2(144,18),11);
        var sliderRoot=Painel(raiz.transform,"Forca",new Vector2(-232,-146),new Vector2(126,11),new Color(.22f,.23f,.19f));
        powerFill=Painel(sliderRoot,"Preenchimento",Vector2.zero,Vector2.zero,Gold);UIFactory.Stretch(powerFill);powerColor=powerFill.GetComponent<Image>();
        gestoLabel=Label(raiz.transform,"Gesto","SEGURE O CLIQUE\nSOLTE PARA LANÇAR",new Vector2(238,-134),new Vector2(143,34),12);
        cameraLabel=Label(raiz.transform,"CameraAtual","",new Vector2(0,-111),new Vector2(360,14),9.5f);
        Label(raiz.transform,"Controles","A/D: posição · Mouse / ← →: mira · Clique/Espaço: segure e solte · Direito/C: cancelar · V: vistas",new Vector2(0,-167),new Vector2(624,20),11);
        efeitos=gameObject.AddComponent<AudioSource>();efeitos.playOnAwake=false;
        impacto=Resources.Load<AudioClip>("Audio/Foley/bocha")??Capitulo2Audio.CriarImpactoMadeira();
        madeira=Resources.Load<AudioClip>("Audio/Foley/footstep_wood_001")??impacto;
        saibro=Resources.Load<AudioClip>("Audio/Foley/footstep_grass_001")??impacto;
        raiz.SetActive(false);
    }
    private void Comecar(bool treino)
    {
        pratica=treino;partida=new BochaMatch();NovaMao();
        MostrarModal(treino?"PRÁTICA · SEM APOSTA":"PRIMEIRA MÃO",
            "Marcus joga com as bochas azul-esverdeadas; Tonão, as cor de cobre. O balim é o menor, claro e marcado por um anel.\n\n"+
            "Mire com o cursor. Quanto mais ao fundo, maior a linha. Segure o clique para carregar e solte para lançar. A força sobe e desce continuamente. Escolha Ponto, Rafa ou Volo antes.\n\n"+
            "V troca a vista. O minimapa e as vistas de consulta mostram as mesmas bolas. A leitura não gasta lançamento.",
            ("LER A CANCHA",()=>modal.SetActive(false)),("REGRAS COMPLETAS",()=>Regras(0)));
    }
    private void NovaMao()
    {
        sim=partida.CriarMao();sim.Impacto=ImpactoReal;sim.Contato=ContatoReal;
        acumulador=quieto=0;xLancamento=0;direcao=Vector2.up;distanciaMira=sim.bolas[0].pos.y-.65f;efeitoMao=false;tipo=BochaThrowType.Ponto;
        CancelarCarga();vista.SetView(BochaCameraView.Lancamento);percepcao.text="Piso uniforme de saibro. Observe o peso e a rolagem.";
        ProximoTurno();AtualizarTela();
    }
    private void ProximoTurno()
    {
        quieto=0;forca=.04f;aiPreparada=false;arrastando=tecladoCarregando=false;
        if(!sim.bolas[0].valida||partida.Proximo(sim)==BochaSide.Nenhum){ResolverMao();return;}
        fase=partida.Proximo(sim)==BochaSide.Marcus?Fase.Mira:Fase.Tonando;esperaIA=.8f;
        if(fase==Fase.Mira)vista.SetView(BochaCameraView.Lancamento);
    }
    private bool PodeConsultar()=>IsShowing&&!PayphoneCheckpoint.IsSaveUIOpen&&(modal==null||!modal.activeSelf)&&partida!=null;
    private bool PodeMirar()=>PodeConsultar()&&(fase==Fase.Mira||fase==Fase.Carregando);
    private void Update()
    {
        if(!IsShowing||PayphoneCheckpoint.IsSaveUIOpen)return;
        if(larguraTela!=Screen.width||alturaTela!=Screen.height)
        {
            larguraTela=Screen.width;alturaTela=Screen.height;
            if(modal==null||!modal.activeSelf){CancelarCarga();Pausar();return;}
        }
        var k=Keyboard.current;
        if(k!=null&&k.escapeKey.wasPressedThisFrame){if(modal==null||!modal.activeSelf){CancelarCarga();Pausar();}return;}
        if(modal!=null&&modal.activeSelf)return;
        if(Mouse.current!=null&&Mouse.current.rightButton.wasPressedThisFrame){CancelarCarga();AtualizarTela();return;}
        if(k!=null&&k.vKey.wasPressedThisFrame)MudarVista((BochaCameraView)(((int)vista.View+1)%3));
        if(k!=null&&PodeMirar())EntradaTeclado(k);
        if(arrastando||tecladoCarregando)
        {
            cargaTempo+=Time.unscaledDeltaTime;forca=ForcaNoTempo(cargaTempo);
            // Atualiza mesmo com o mouse parado; não depende de eventos OnDrag.
            if(arrastando&&Mouse.current!=null)
            {
                if(!Mouse.current.leftButton.isPressed&&!Mouse.current.leftButton.wasReleasedThisFrame)CancelarCarga();
                else AtualizarMira(Mouse.current.position.ReadValue(),null);
            }
        }
        if(fase==Fase.Rolagem)
        {
            acumulador+=Mathf.Min(Time.deltaTime,.2f);
            while(acumulador>=BochaSimulation.Passo){sim.Step();vista.RecordContacts();acumulador-=BochaSimulation.Passo;}
            quieto=sim.IsMoving?0:quieto+Time.deltaTime;if(quieto>=.45f)ProximoTurno();
        }
        else if(fase==Fase.Tonando)
        {
            esperaIA-=Time.deltaTime;
            if(esperaIA<=0)
            {
                if(!aiPreparada){shotIA=BochaAI.Escolher(sim,4-partida.usadas[1],rng);aiPreparada=true;esperaIA=.7f;percepcao.text=shotIA.intencao;}
                else EfetuarLancamento(BochaSide.Tonao,shotIA);
            }
        }
        vista.Tick(Time.deltaTime);AtualizarTela();
    }
    private void EntradaTeclado(Keyboard k)
    {
        if(k.digit1Key.wasPressedThisFrame)EscolherTipo(BochaThrowType.Ponto);
        if(k.digit2Key.wasPressedThisFrame)EscolherTipo(BochaThrowType.Rafa);
        if(k.digit3Key.wasPressedThisFrame)EscolherTipo(BochaThrowType.Volo);
        if(k.cKey.wasPressedThisFrame){CancelarCarga();return;}
        if(vista.View!=BochaCameraView.Lancamento)
        {if(k.spaceKey.wasPressedThisFrame)MudarVista(BochaCameraView.Lancamento);return;}
        if(arrastando)return;
        xLancamento=Mathf.Clamp(xLancamento+((k.dKey.isPressed?1:0)-(k.aKey.isPressed?1:0))*Time.deltaTime*1.6f,-2.6f,2.6f);
        float angle=Mathf.Atan2(direcao.x,direcao.y)+((k.rightArrowKey.isPressed?1:0)-(k.leftArrowKey.isPressed?1:0))*Time.deltaTime*.26f;
        angle=Mathf.Clamp(angle,-.58f,.58f);direcao=new Vector2(Mathf.Sin(angle),Mathf.Cos(angle));
        if(k.spaceKey.wasPressedThisFrame){fase=Fase.Carregando;tecladoCarregando=true;cargaTempo=0;forca=.04f;}
        if(tecladoCarregando&&k.spaceKey.wasReleasedThisFrame)EfetuarLancamento(BochaSide.Marcus,Preparar());
    }
    public static float ForcaNoTempo(float segundos)=>.04f+.96f*Mathf.PingPong(Mathf.Max(0,segundos)/1.5f,1);
    private BochaShot Preparar()=>BochaLaunch.Preparar(tipo,xLancamento,direcao,forca);
    private void IniciarArraste(PointerEventData e)
    {
        if(e.button!=PointerEventData.InputButton.Left||!PodeMirar()||vista.View!=BochaCameraView.Lancamento)return;
        if(arrastando||tecladoCarregando)return;
        AtualizarMira(e.position,e.pressEventCamera);
        arrastando=true;tecladoCarregando=false;ponteiroCarga=e.pointerId;cargaTempo=0;forca=.04f;fase=Fase.Carregando;AtualizarTela();
    }
    private void Arrastar(PointerEventData e)
    {
        if(!arrastando||!PodeMirar()||e.pointerId!=ponteiroCarga)return;
        AtualizarMira(e.position,e.pressEventCamera);AtualizarTela();
    }
    private void SoltarArraste(PointerEventData e)
    {
        if(e.button!=PointerEventData.InputButton.Left||!arrastando||e.pointerId!=ponteiroCarga)return;
        var hit=e.pointerCurrentRaycast.gameObject;
        bool valid=PodeMirar()&&RectTransformUtility.RectangleContainsScreenPoint(cancha,e.position,e.pressEventCamera)&&
            (hit==null||hit==vista.gameObject);
        AtualizarMira(e.position,e.pressEventCamera);
        arrastando=false;
        if(valid)EfetuarLancamento(BochaSide.Marcus,Preparar());else CancelarCarga();
    }
    private void MoverMira(PointerEventData e)
    {
        if(!PodeMirar()||tecladoCarregando||vista.View!=BochaCameraView.Lancamento)return;
        AtualizarMira(e.position,e.enterEventCamera);AtualizarTela();
    }
    private void AtualizarMira(Vector2 screen,Camera camera)
    {
        if(!vista.CourtPoint(screen,camera,out var target))return;
        target=new Vector2(Mathf.Clamp(target.x,-2.8f,2.8f),Mathf.Clamp(target.y,1.2f,19.8f));
        var d=target-new Vector2(xLancamento,.65f);float a=Mathf.Clamp(Mathf.Atan2(d.x,d.y),-.58f,.58f);
        direcao=new Vector2(Mathf.Sin(a),Mathf.Cos(a));distanciaMira=Mathf.Clamp(d.magnitude,.8f,19.15f);
    }
    private void EscolherTipo(BochaThrowType value){if(!PodeMirar())return;CancelarCarga();tipo=value;AtualizarTela();}
    private void MudarVista(BochaCameraView value)
    {if(!PodeConsultar())return;CancelarCarga();vista.SetView(value);AtualizarTela();}
    private void CancelarCarga(){arrastando=tecladoCarregando=false;cargaTempo=0;forca=.04f;if(fase==Fase.Carregando)fase=Fase.Mira;}
    // Entrada antiga preservada para verificações de integração já existentes.
    private void Lancar(BochaSide side,float x,Vector2 velocity)
    {EfetuarLancamento(side,new BochaShot{x=x,velocidade=velocity,giro=new Vector3(velocity.y,0,-velocity.x)/BochaSimulation.RaioBocha});}
    private void EfetuarLancamento(BochaSide side,BochaShot shot)
    {
        if(modal!=null&&modal.activeSelf)return;
        if(!partida.Lancar(sim,side,shot))return;
        arrastando=tecladoCarregando=false;fase=Fase.Rolagem;quieto=acumulador=0;AtualizarTela();
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
        if(!IsShowing||partida==null)return;CancelarCarga();
        MostrarModal("BOCHA PAUSADA","Bolas, força e turno ficam parados. Consultar ou pausar não consome uma bocha.",("CONTINUAR",()=>modal.SetActive(false)),("REGRAS",()=>Regras(0)),(pratica?"SAIR DA PRÁTICA":"ENCERRAR DUELO",()=>{if(pratica)Fechar();else Derrota();}));
    }
    private void OnApplicationFocus(bool focus)
    {if(!focus&&IsShowing&&(modal==null||!modal.activeSelf)){CancelarCarga();Pausar();}}
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
        forcaLabel.text="FORÇA "+Mathf.RoundToInt(forca*100)+"%"+(fase==Fase.Carregando?(descendo?" −":" +"):"");
        powerFill.anchorMax=new Vector2(forca,1);powerColor.color=Color.Lerp(new Color(.36f,.62f,.57f),new Color(.92f,.59f,.25f),forca);
        for(int i=0;i<typeButtons.Count;i++){typeButtons[i].GetComponent<Button>().interactable=PodeMirar();typeButtons[i].color=(int)tipo==i?new Color(.27f,.36f,.30f):new Color(.13f,.17f,.14f);typeLabels[i].color=(int)tipo==i?new Color(.98f,.87f,.61f):new Color(.74f,.76f,.67f);}
        bool inspect=vista.View!=BochaCameraView.Lancamento;
        gestoLabel.text=fase==Fase.Rolagem?"AGUARDE O REPOUSO\nPODE CONSULTAR VISTAS":fase==Fase.Tonando?"VEZ DE TONÃO\nOBSERVE A JOGADA":inspect?"LANÇAMENTO / V\nPARA VOLTAR À MIRA":fase==Fase.Carregando?"FORÇA SOBE E DESCE\nSOLTE PARA LANÇAR":"SEGURE O CLIQUE\nSOLTE PARA LANÇAR";
        cameraLabel.text=vista.View==BochaCameraView.Superior?"VISTA SUPERIOR · LANÇAMENTO ou V para voltar":vista.View==BochaCameraView.Alvo?"VISTA DO BALIM · roda ajusta o zoom":"MIRA "+distanciaMira.ToString("F1")+" m · força define alcance · roda: zoom";
        vista.SetState(sim,Preparar(),PodeMirar()&&!inspect,distanciaMira);minimapa.Simulation=sim;minimapa.Refresh();
    }
    private static string TextoDistancia(float d)=>float.IsPositiveInfinity(d)?"—":d<1?(d*100).ToString("F0")+" cm":d.ToString("F2")+" m";
    public void Fechar(){StopAllCoroutines();if(efeitos!=null)efeitos.Stop();CancelarCarga();aoVencer=null;if(modal!=null)modal.SetActive(false);raiz.SetActive(false);}
    private void ContatoReal(BochaContact contact)
    {
        if(Time.unscaledTime-ultimoContato<.035f)return;ultimoContato=Time.unscaledTime;
        var clip=contact.tipo==BochaContactType.Madeira?madeira:contact.tipo==BochaContactType.Saibro?saibro:impacto;
        efeitos.pitch=contact.tipo==BochaContactType.Madeira?.82f:contact.tipo==BochaContactType.Saibro?.78f:1;
        efeitos.PlayOneShot(clip,Mathf.Clamp(contact.velocidade*.047f,.018f,.42f)*PlayerPrefs.GetFloat("audio_efeitos",1));
    }
    private void ImpactoReal(float speed)
    {
        if(Time.unscaledTime-ultimoImpacto<.07f)return;ultimoImpacto=Time.unscaledTime;
        if(partida!=null&&partida.Mao>0&&!efeitoMao){efeitoMao=true;StartCoroutine(RespostaAbafada());}
    }
    private IEnumerator RespostaAbafada()
    {
        float elapsed=0;while(elapsed<.58f){if(!PayphoneCheckpoint.IsSaveUIOpen&&(modal==null||!modal.activeSelf))elapsed+=Time.deltaTime;yield return null;}
        percepcao.text="Um sopro abafado responde do outro lado da parede.";efeitos.pitch=1;
        efeitos.PlayOneShot(Capitulo2Audio.RespostaAbafada(),.28f*PlayerPrefs.GetFloat("audio_efeitos",1));
    }
    private void MostrarModal(string title,string text,params(string texto,Action acao)[] options)
    {
        CancelarCarga();
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

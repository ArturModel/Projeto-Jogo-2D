using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Uma viagem contemplativa. O deslocamento lateral é limitado e não existe
// derrota, colisão, aceleração obrigatória ou interação que bloqueie a chegada.
public sealed class PrologoJogavel : MonoBehaviour
{
    public const float InicioManobra = 36f;
    public const float Parado = 44f;
    public const float Duracao = 51f;
    public const float DistanciaParada = 468f;
    internal const float EscalaLateralEntrada = 1.9f;
    internal const float EscalaLongitudinalEntrada = 4f;
    internal static float LateralEntrada(float norte) => (10f - norte) * EscalaLateralEntrada;
    internal static float DistanciaEntrada(float leste) =>
        DistanciaParada + (leste - Capitulo1Director.CarroEntrada.x) * EscalaLongitudinalEntrada;
    public float Tempo { get; private set; }
    public float Direcao { get; private set; }
    public float Lateral { get; private set; }
    public float Distancia { get; private set; }
    public Vector2 Olhar { get; private set; }
    public int Limpador { get; private set; } = 1;
    public bool FarolAlto { get; private set; }
    public bool RadioLigado { get; private set; }
    public bool PortaLuvasAberto { get; private set; }
    public bool Finalizado { get; private set; }
    public bool Suspenso => !foco || MenuPausa.Aberto || Time.timeScale <= 0f;
    public float Velocidade => Tempo < 34f ? 12f : Mathf.Lerp(12f, 0f, Mathf.SmoothStep(0f, 1f, (Tempo - 34f) / 10f));
    // Duas curvas contínuas no espaço; terreno e objetos usam a mesma trajetória.
    public static float CentroVia(float distancia) =>
        15f * Mathf.SmoothStep(0,1,Mathf.InverseLerp(105,215,distancia))
        - 22f * Mathf.SmoothStep(0,1,Mathf.InverseLerp(245,365,distancia));
    public static float TerraEm(float distancia) =>
        Mathf.SmoothStep(0, 1, Mathf.InverseLerp(140, 166, distancia)) *
        (1 - Mathf.SmoothStep(0, 1, Mathf.InverseLerp(348, 400, distancia)));

    private static float DistanciaEm(float tempo)
    {
        // Integral da desaceleração usada por Velocidade. A posição final é fixa.
        float t = Mathf.Clamp01((tempo - 34f) / 10f);
        return 12f * Mathf.Min(tempo, 34f) + 120f * (t - t * t * t * (1f - .5f * t));
    }
    public float Rumo => (CentroVia(Distancia+1)-CentroVia(Distancia-1))*.5f;
    public float Curva => Mathf.Clamp((CentroVia(Distancia+12)-2*CentroVia(Distancia)+CentroVia(Distancia-12))*.4f,-.65f,.65f);
    public float Terra => TerraEm(Distancia);
    public float Estacionamento => Mathf.SmoothStep(0,1,Mathf.InverseLerp(InicioManobra,Parado,Tempo));
    public float PosicaoLateral =>
        Lateral * (1 - Estacionamento) +
        Mathf.Lerp(-1.9f * Mathf.SmoothStep(0, 1, Mathf.InverseLerp(390, 428, Distancia)),
            LateralEntrada(Capitulo1Director.CarroEntrada.y), Estacionamento);
    public float FaseLimpador { get; private set; }
    public float Molhado { get; private set; } = .25f;

    private RectTransform quadro, cabine, volante, paisagem, conteudo, celular;
    private PrologoEstradaGraphic estrada, chuva;
    private TMP_Text dica, ajuda, mostrador, telefoneTexto;
    private Image escuro, luzPortaLuvas;
    private RadioCarro radio;
    private PortaLuvasCarro portaLuvas;
    private CelularCarro telefone;
    private Vector2 olharAlvo, pontoMouse;
    private bool arrastando, foco = true;
    private int fala;
    private float pular, cicloAnterior, proximaSeta;
    private bool motorDesligado;
    private readonly string[] pensamentos = {
        "\"Desde cedo sem atender.\"",
        "\"O vô Adílio nunca deixa o telefone tocar até cair.\"",
        "\"Seis anos sem voltar pra Santa Rita.\"",
        "\"Pelo menos a estrada eu ainda lembro.\"",
        "\"Vou encostar um pouco.\"",
        "\"Certo... vamos ver por que ele não atendeu hoje.\""
    };
    private readonly float[] tempos = { 4f, 10f, 19f, 28f, 36f, 46f };
    private readonly Rect areaVolante = new Rect(30, -60, 350, 335);
    private readonly Rect[] areas = {
        new Rect(389,166,176,71), new Rect(523,79,43,44),
        new Rect(393,77,52,45), new Rect(590,105,278,94),
        new Rect(543,231,61,71)
    };

    private void Awake()
    {
        // O protótipo antigo continua preservado na cena, mas nunca disputa input.
        foreach (var c in FindObjectsByType<OlharPrimeiraPessoa>(FindObjectsInactive.Include)) c.enabled = false;
        foreach (var c in FindObjectsByType<PonteiroCarro>(FindObjectsInactive.Include)) c.Ativo = false;
        foreach (var root in gameObject.scene.GetRootGameObjects())
            if (root.name == "Audio") root.SetActive(true);
        Construir();
        radio = Objeto<RadioCarro>("RadioInterativo");
        portaLuvas = Objeto<PortaLuvasCarro>("PortaLuvasInterativo");
        telefone = Objeto<CelularCarro>("TelefoneInterativo");
        if (FindAnyObjectByType<MenuPausa>() == null) gameObject.AddComponent<MenuPausa>();
    }

    private T Objeto<T>(string nome) where T : ObjetoCarro
    {
        var go = new GameObject(nome); go.transform.SetParent(transform, false);
        // ObjetoCarro exige Collider2D, que é abstrato. Fornecer o concreto antes.
        go.AddComponent<BoxCollider2D>().enabled = false;
        return go.AddComponent<T>();
    }

    private void Start()
    {
        AudioAmbiente.Instance?.ConfigurarLoop("motor",Resources.Load<AudioClip>("Audio/Prologo/MotorInterior"));
        AudioAmbiente.Instance?.ConfigurarLoop("pneusTerra",Resources.Load<AudioClip>("Audio/Prologo/RodagemTerra"));
        AudioAmbiente.Instance?.Definir("chuva", .72f, 1f);
        AudioAmbiente.Instance?.Definir("motor", .3f, 1f);
        AudioAmbiente.Instance?.Definir("pneus", .2f, 1f);
    }

    private void Construir()
    {
        var canvas = UIFactory.CreateCanvas(transform, "PrologoJogavelCanvas", 100);
        var preto = new GameObject("Letterbox").AddComponent<Image>();
        preto.transform.SetParent(canvas.transform, false); preto.color = Color.black; preto.raycastTarget = false;
        UIFactory.Stretch(preto.rectTransform);
        quadro = Rect("Enquadramento16x9", canvas.transform, new Vector2(960,540), Vector2.zero);
        quadro.gameObject.AddComponent<RectMask2D>();
        cabine = Rect("CamadasDoCarro", quadro, new Vector2(960,540), Vector2.zero);
        paisagem = Imagem("HorizonteDistante", cabine, "Horizonte", new Vector2(1160,350), new Vector2(480,425));
        paisagem.GetComponent<RawImage>().color = new Color(.49f, .55f, .64f);
        estrada = Graphic("AsfaltoReflexosAcostamento", cabine, false);
        chuva = Graphic("ChuvaPalhetas", cabine, true);
        Imagem("CabineDetalhada", cabine, "Cabine", new Vector2(1000,563), new Vector2(480,270));
        // Os elementos móveis são texturas independentes, nunca recortes do vídeo.
        volante = Imagem("VolanteEMaos", cabine, "Volante", new Vector2(400,400), new Vector2(215,45));
        // A textura do volante tem seu centro mecânico em 50%,43%.
        volante.pivot = new Vector2(.5f,.57f);
        volante.anchoredPosition = new Vector2(215-480, 84-270);
        celular = Imagem("Telefone", cabine, "Objetos", new Vector2(71,92), new Vector2(573,268), new Rect(0,.0f,.5f,1));
        celular.localRotation = Quaternion.Euler(0,0,13);
        conteudo = Imagem("PapeisEFotografia", cabine, "Objetos", new Vector2(187,123), new Vector2(733,141), new Rect(.5f,0,.5f,1));
        conteudo.gameObject.SetActive(false);
        luzPortaLuvas = Caixa(cabine, "LuzPortaLuvas", new Rect(594,104,270,91), new Color(.7f,.38f,.14f,.0f));
        luzPortaLuvas.transform.SetSiblingIndex(conteudo.GetSiblingIndex());
        mostrador = Texto(cabine, "DisplayRadio", new Rect(436,199,81,17), 12); mostrador.color = new Color(.88f,.57f,.25f);
        telefoneTexto = Texto(cabine, "LCDTelefone", new Rect(552,276,38,15), 7); telefoneTexto.color = new Color(.11f,.19f,.12f);
        dica = Texto(quadro, "AcaoSobCursor", new Rect(200,72,560,26), 17);
        ajuda = Texto(quadro, "Controles", new Rect(40,8,880,25), 17);
        ajuda.text = "A/D volante   •   botão direito: olhar   •   clique: interagir   •   R centraliza   •   ESC pausa";
        escuro = Caixa(quadro, "Abertura", new Rect(0,0,960,540), Color.black);
    }

    private PrologoEstradaGraphic Graphic(string nome, Transform pai, bool gotas)
    {
        var r = Rect(nome, pai, new Vector2(1040,600), Vector2.zero);
        r.gameObject.AddComponent<CanvasRenderer>();
        var g = r.gameObject.AddComponent<PrologoEstradaGraphic>(); g.Viagem = this; g.Chuva = gotas; g.raycastTarget = false;
        return g;
    }
    private static RectTransform Rect(string nome, Transform pai, Vector2 tamanho, Vector2 pos)
    {
        var r = new GameObject(nome, typeof(RectTransform)).GetComponent<RectTransform>();
        r.SetParent(pai,false); r.anchorMin = r.anchorMax = new Vector2(.5f,.5f);
        r.sizeDelta = tamanho; r.anchoredPosition = pos; return r;
    }
    private RectTransform Imagem(string nome, Transform pai, string recurso, Vector2 tamanho, Vector2 centro, Rect? uv = null)
    {
        var r = Rect(nome,pai,tamanho,centro-new Vector2(480,270));
        var im = r.gameObject.AddComponent<RawImage>(); im.texture = Resources.Load<Texture2D>("PrologoJogavel/"+recurso);
        im.raycastTarget = false; if(uv.HasValue) im.uvRect = uv.Value;
        if (im.texture == null) Debug.LogError("Asset do prólogo ausente: " + recurso);
        return r;
    }
    private static Image Caixa(Transform pai, string nome, Rect area, Color cor)
    {
        var r=Rect(nome,pai,area.size,area.center-new Vector2(480,270));
        var im=r.gameObject.AddComponent<Image>(); im.color=cor; im.raycastTarget=false; return im;
    }
    private static TMP_Text Texto(Transform pai, string nome, Rect area, float tamanho)
    {
        var t=UIFactory.CreateLabel(pai,nome,tamanho); t.alignment=TextAlignmentOptions.Center;
        t.maskable=false;
        t.textWrappingMode=TextWrappingModes.NoWrap;
        t.rectTransform.anchorMin=t.rectTransform.anchorMax=new Vector2(.5f,.5f);
        t.rectTransform.sizeDelta=area.size; t.rectTransform.anchoredPosition=area.center-new Vector2(480,270);
        return t;
    }

    private void Update()
    {
        var pai=(RectTransform)quadro.parent;
        quadro.localScale=Vector3.one*Mathf.Min(pai.rect.width/960f,pai.rect.height/540f);
        if (Finalizado || Suspenso) { arrastando=false; return; }
        float dt = Mathf.Min(Time.deltaTime,.1f);
        Tempo += dt;
        LerControles(dt);
        Distancia = DistanciaEm(Tempo);
        AtualizarAudio();
        if (Limpador > 0 && Tempo < Parado) FaseLimpador += dt/(Limpador==1?2.1f:1.15f);
        float fase=FaseLimpador%1f;
        if (Limpador>0 && fase < cicloAnterior) { Molhado=.08f; AudioAmbiente.Instance?.Disparar("limpador",.45f); }
        cicloAnterior=fase; Molhado=Mathf.MoveTowards(Molhado,.85f,dt*.065f);
        float trepidacao=1f+Terra*.8f;
        cabine.anchoredPosition = -Olhar + (MenuPausa.ReduzirEfeitosIntensos ? Vector2.zero : new Vector2(Mathf.Sin(Tempo*1.7f)*.6f,Mathf.Sin(Tempo*(2.1f+Terra*3f))*.7f)*Mathf.Clamp01(Velocidade/8f)*trepidacao);
        // O relevo distante quase não cresce, mas acompanha a orientação do carro.
        paisagem.anchoredPosition = new Vector2(-PosicaoLateral*2f-Rumo*210f-CentroVia(Distancia)*.8f,155f);
        float manobra=Mathf.Sin(Mathf.InverseLerp(InicioManobra,Parado,Tempo)*Mathf.PI*2)*21f;
        volante.localRotation=Quaternion.Euler(0,0,-Direcao*17f-Curva*12f+manobra);
        mostrador.text=RadioLigado?"FM  97.1":"- - -";
        telefoneTexto.text=GameState.Tem(Flags.PrologoTentouLigarAvo)?"SEM SINAL":"4 CHAM.";
        luzPortaLuvas.color=new Color(.8f,.47f,.2f,PortaLuvasAberto?.16f:0);
        escuro.color=new Color(0,0,0,1f-Mathf.SmoothStep(0,1,(Tempo-.8f)/2f));
        if (fala<tempos.Length && Tempo>=tempos[fala]) Legenda.Dizer(pensamentos[fala++],3.7f,Legenda.PrioridadeRoteiro);
        if(Tempo>=InicioManobra && Tempo<Parado && Tempo>=proximaSeta)
        { proximaSeta=Tempo+.7f; AudioAmbiente.Instance?.Disparar("seta",.24f); }
        if(Tempo>=Parado && !motorDesligado)
        {
            motorDesligado=true; Limpador=0; RadioLigado=false;
            AudioAmbiente.Instance?.Disparar("motorDesliga",.65f);
            AudioAmbiente.Instance?.Definir("motor",0,1.2f); AudioAmbiente.Instance?.Definir("pneus",0,1);
            AudioAmbiente.Instance?.Definir("pneusTerra",0,.35f);
            AudioAmbiente.Instance?.Definir("radio",0,1); AudioAmbiente.Instance?.Definir("chuva",.88f,2);
        }
        if(Tempo>=Duracao) Encerrar();
        estrada.SetVerticesDirty(); chuva.SetVerticesDirty();
    }

    private void AtualizarAudio()
    {
        if(motorDesligado)return;
        var audio=AudioAmbiente.Instance;
        if(audio==null)return;
        float movimento=Mathf.Clamp01(Velocidade/12f);
        float irregularidade=.93f+Mathf.PerlinNoise(Distancia*.13f,7.1f)*.14f;
        audio.Definir("motor",.19f+movimento*.13f,.4f);
        audio.Pitch("motor",.82f+movimento*.2f+Mathf.Sin(Tempo*.8f)*.006f);
        audio.Definir("pneus",.2f*movimento*(1-Terra),.3f);
        audio.Definir("pneusTerra",.58f*Terra*movimento*irregularidade,.3f);
        audio.Pitch("pneusTerra",.76f+movimento*.24f);
    }

    private void LerControles(float dt)
    {
        var k=Keyboard.current; var m=Mouse.current; var pad=Gamepad.current;
        float entrada=0;
        if(k!=null) entrada=((k.dKey.isPressed||k.rightArrowKey.isPressed)?1:0)-((k.aKey.isPressed||k.leftArrowKey.isPressed)?1:0);
        if(pad!=null && Mathf.Abs(pad.leftStick.x.ReadValue())>.1f) entrada=pad.leftStick.x.ReadValue();
        Vector2 local=Vector2.negativeInfinity;
        if(m!=null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(cabine,m.position.ReadValue(),null,out local);
            local+=new Vector2(480,270);
            if(m.leftButton.wasPressedThisFrame && areaVolante.Contains(local)) { arrastando=true; pontoMouse=m.position.ReadValue(); }
            if(!m.leftButton.isPressed) arrastando=false;
            if(arrastando) entrada=Mathf.Clamp((m.position.x.ReadValue()-pontoMouse.x)/(110f*quadro.localScale.x),-1,1);
            if(m.rightButton.isPressed) olharAlvo+=m.delta.ReadValue()*.16f/quadro.localScale.x;
            if(m.middleButton.wasPressedThisFrame) olharAlvo=Vector2.zero;
        }
        if(k!=null)
        {
            if(k.qKey.isPressed) olharAlvo.x-=dt*45f;
            if(k.eKey.isPressed) olharAlvo.x+=dt*45f;
            if(k.rKey.wasPressedThisFrame) olharAlvo=Vector2.zero;
            if(k.digit1Key.wasPressedThisFrame) Interagir(0);
            if(k.digit2Key.wasPressedThisFrame) Interagir(1);
            if(k.digit3Key.wasPressedThisFrame) Interagir(2);
            if(k.digit4Key.wasPressedThisFrame) Interagir(3);
            if(k.digit5Key.wasPressedThisFrame) Interagir(4);
            pular=k.spaceKey.isPressed?pular+dt:0;
            if(pular>1.2f) Encerrar();
        }
        if(pad!=null) olharAlvo+=pad.rightStick.ReadValue()*45f*dt;
        olharAlvo=new Vector2(Mathf.Clamp(olharAlvo.x,-26,26),Mathf.Clamp(olharAlvo.y,-12,12));
        Olhar=Vector2.Lerp(Olhar,olharAlvo,1-Mathf.Exp(-7*dt));
        Direcao=Mathf.MoveTowards(Direcao,Tempo<InicioManobra?entrada:0,dt*3f);
        // O retorno suave mantém a viagem segura mesmo quando se olha para os objetos.
        Lateral=Mathf.Clamp(Lateral+Direcao*dt*.42f-Lateral*dt*.3f,-.65f,.65f);
        int sob=-1;
        for(int i=0;i<areas.Length;i++) if(areas[i].Contains(local)) sob=i;
        dica.text=areaVolante.Contains(local)?"Segure e arraste  •  mover o volante":Rotulo(sob);
        if(sob>=0 && m!=null && m.leftButton.wasPressedThisFrame && !arrastando) Interagir(sob);
        if(pular>0) dica.text="Segure ESPAÇO para pular a viagem  " + Mathf.CeilToInt(Mathf.Clamp01(pular/1.2f)*100)+"%";
        ajuda.color=new Color(.78f,.78f,.75f,Tempo<12f? .85f : .32f);
    }
    private string Rotulo(int i)
    {
        switch(i) {
            case 0: return "1 / clique  •  "+(RadioLigado?"desligar rádio":"ligar rádio");
            case 1: return "2 / clique  •  limpadores: "+new[]{"desligados","intermitentes","rápidos"}[Limpador];
            case 2: return "3 / clique  •  "+(FarolAlto?"baixar faróis":"ligar farol alto");
            case 3: return "4 / clique  •  "+(PortaLuvasAberto?"examinar fotografia / fechar com 4":"abrir porta-luvas");
            case 4: return "5 / clique  •  examinar telefone / tentar ligação";
            default:return "";
        }
    }
    public void Interagir(int objeto)
    {
        if(Finalizado || Suspenso || Tempo<2.8f || Tempo>=Parado) return;
        SantaRitaSound.Play("inspect",.14f);
        switch(objeto) {
            case 0: RadioLigado=!RadioLigado; radio.Ativar(); break;
            case 1: Limpador=(Limpador+1)%3; break;
            case 2: FarolAlto=!FarolAlto; break;
            case 3:
                if(!PortaLuvasAberto) { PortaLuvasAberto=true; portaLuvas.Ativar(); }
                else if(!GameState.Tem(Flags.PrologoViuFotoAvo)) portaLuvas.Ativar();
                else PortaLuvasAberto=false;
                conteudo.gameObject.SetActive(PortaLuvasAberto); break;
            case 4: telefone.Ativar(); break;
        }
    }
    private void OnApplicationFocus(bool ativo) { foco=ativo; arrastando=false; }
    public void Encerrar()
    {
        if(Finalizado) return; Finalizado=true;
        Legenda.Limpar();
        GameState.Marcar(Flags.PrologoConcluido);
        GameState.RegistrarNotaDiario("prologo_chegada_2004","Chegada a Santa Rita (2004)","FATO",
            "Adílio parou de atender o telefone hoje cedo. Peguei meu carro na chuva e voltei a Santa Rita pela primeira vez desde 1998.");
        GameState.DefinirObjetivo("Ir até a casa de Adílio no Bairro das Casas, nº 40 (Leste).");
        SceneTransition.Ir("EntradaDaCidade","saida_carro");
    }
}

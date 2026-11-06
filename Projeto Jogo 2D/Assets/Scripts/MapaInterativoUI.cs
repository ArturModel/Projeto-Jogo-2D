using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Atlas georreferenciado com diário e fichas de C1/C2.
public class MapaInterativoUI : MonoBehaviour
{
    public static MapaInterativoUI Instance { get; private set; }
    public static bool IsShowing => Instance != null && Instance.painelRaiz != null && (Instance.painelRaiz.activeSelf || (Instance.notebook!=null&&Instance.notebook.IsOpen));

    private GameObject painelRaiz;
    private GameObject hudHintGo;
    private TextMeshProUGUI hudHintText;

    private RectTransform mapAreaRect;
    private RectTransform mapViewportRect;
    private Vector2 mapBaseSize = new Vector2(506.1f, 294f);
    private float mapZoom = 1f;
    private bool arrastandoMapa;
    private Vector2 posInicialClique;
    private bool arrastouNoCliqueAtual;
    private bool ocultarBlipsNoMapa;
    private TextMeshProUGUI textoZoomIndicador;
    private Image mapBackgroundImage;
    private SantaRitaAtlasView atlasView;
    private GameObject sidePanelObject;
    private GameObject atlasControls;
    private GameObject plantaButton;
    private GameObject mapFrame;
    private MarcusNotebookUI notebook;
    private float nextVisit;
    private string lastVisit;
    private RectTransform playerMarkerRect;
    private Image playerMarkerImage;

    private TextMeshProUGUI textoLocalAtual;
    private TextMeshProUGUI textoObjetivoAtual;
    private TextMeshProUGUI textoDiarioConteudo;
    private TextMeshProUGUI textoStatusSave;
    private ScrollRect diarioScroll;

    private int abaAtiva = 0; // 0 = Atlas, 1 = Diário, 2 = Fichas, 3 = Legenda legível
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
        if (IsShowing)
            Instance.Fechar();
    }

    private void InicializarPontos()
    {
        pontos.Clear();
        foreach(var l in SantaRitaAtlas.Survey.landmarks)
        {
            var r=SantaRitaAtlas.Region(l.region);
            var target=SantaRitaAtlas.Region(l.destination);
            pontos.Add(new PontoMapa{id=l.id,nome=target!=null?target.name:r.name,descricao="",uv=SantaRitaAtlas.UV(r.ToAtlas(l.local)),tipo=l.phone?"orelhao":"acesso",cor=Color.white});
        }
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
        mapFrame=moldura;
        moldura.transform.SetParent(painelRaiz.transform, false);
        var mRect = moldura.GetComponent<RectTransform>();
        Posicionar(mRect, Vector2.zero, new Vector2(624f, 348f));
        moldura.GetComponent<Image>().color = new Color(0.13f, 0.11f, 0.09f, 0.99f);

        var titulo = CriarTexto("TituloMapa", moldura.transform, "MAPA DA CIDADE", 17, new Color(0.96f, 0.86f, 0.54f), TextAlignmentOptions.Left);
        Posicionar(titulo.rectTransform, new Vector2(-172f, 160f), new Vector2(260f, 20f));

        var subFechar = CriarTexto("SubFechar", moldura.transform, "[Roda / + -] Zoom  •  [Segurar Mouse Esq. e Arrastar / WASD] Mover  •  [M / ESC] Fechar", 10.5f, new Color(0.84f, 0.80f, 0.70f), TextAlignmentOptions.Right);
        Posicionar(subFechar.rectTransform, new Vector2(118f, 160f), new Vector2(368f, 20f));

        // Esquerda: Área Gráfica do Mapa 2D em proporção 4:3 fiel à arte original (412 x 309)
        var viewport = new GameObject("MapViewport", typeof(RectTransform), typeof(Image), typeof(RectMask2D));
        viewport.transform.SetParent(moldura.transform, false);
        mapViewportRect = viewport.GetComponent<RectTransform>();
        Posicionar(mapViewportRect, new Vector2(0f, -6f), new Vector2(604,294));
        viewport.GetComponent<Image>().color = new Color(0.06f, 0.08f, 0.07f, 1f);

        var mapContainer = new GameObject("MapContainer", typeof(RectTransform));
        mapContainer.transform.SetParent(viewport.transform, false);
        mapAreaRect = mapContainer.GetComponent<RectTransform>();
        Posicionar(mapAreaRect, Vector2.zero, mapBaseSize);

        var mapArtGo = new GameObject("MapArt", typeof(RectTransform), typeof(Image));
        mapArtGo.transform.SetParent(mapContainer.transform, false);
        UIFactory.Stretch(mapArtGo.GetComponent<RectTransform>());
        mapBackgroundImage = mapArtGo.GetComponent<Image>();
        mapBackgroundImage.color = new Color(.13f,.17f,.13f);
        mapBackgroundImage.raycastTarget = false;
        var landscape=Resources.Load<Texture2D>("Atlas/Paisagem");
        if(landscape!=null){mapBackgroundImage.sprite=Sprite.Create(landscape,new Rect(0,0,landscape.width,landscape.height),new Vector2(.5f,.5f),16);mapBackgroundImage.color=Color.white;}
        atlasView = mapContainer.AddComponent<SantaRitaAtlasView>();
        atlasView.Initialize(mapAreaRect, SelecionarMarcoAtlas);

        // Marcador da Posição Atual de Marcus (tempo real)
        var playerGo = new GameObject("MarcadorMarcus", typeof(RectTransform), typeof(Image));
        playerGo.transform.SetParent(mapAreaRect, false);
        playerMarkerRect = playerGo.GetComponent<RectTransform>();
        playerMarkerRect.sizeDelta = new Vector2(16f, 16f);
        playerMarkerImage = playerGo.GetComponent<Image>();
        playerMarkerImage.sprite = GerarIconeJogador();
        playerMarkerImage.color = new Color(0.20f, 0.96f, 1f, 1f);
        playerMarkerImage.raycastTarget = false;

        var playerLbl = CriarTexto("LblVoce", playerMarkerRect, "VOCÊ", 10, new Color(.82f,.86f,.71f,1), TextAlignmentOptions.Center);
        Posicionar(playerLbl.rectTransform, new Vector2(0f, -11f), new Vector2(48f, 12f));
        playerLbl.raycastTarget = false;

        // Barra flutuante de controles de Zoom/Arraste na base do Viewport do Mapa
        var barraZoom = new GameObject("BarraControlesMapa", typeof(RectTransform), typeof(Image));
        atlasControls=barraZoom;
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

        // Direita: Painel Lateral (Atlas / Diário / Fichas)
        var sidePanel = new GameObject("SidePanel", typeof(RectTransform), typeof(Image));
        sidePanelObject=sidePanel;
        sidePanel.transform.SetParent(moldura.transform, false);
        var spRect = sidePanel.GetComponent<RectTransform>();
        Posicionar(spRect, new Vector2(208f, -6f), new Vector2(188f, 309f));
        sidePanel.GetComponent<Image>().color = new Color(0.09f, 0.08f, 0.07f, 0.98f);

        botoesAba[0] = CriarBotaoAba(sidePanel.transform, "MAPA", new Vector2(-66f, 138f), () => {pontoSelecionadoId="";SelecionarAba(0);});
        botoesAba[1] = CriarBotaoAba(sidePanel.transform, "CADERNO", new Vector2(-22f, 138f), () => SelecionarAba(1));
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

        // Abas ficam disponíveis sem encobrir a cidade.
        for(int i=0;i<botoesAba.Length;i++)
        {
            botoesAba[i].transform.SetParent(moldura.transform,false);
            Posicionar((RectTransform)botoesAba[i].transform,new Vector2(78+i*64,160),new Vector2(60,20));
        }
        subFechar.gameObject.SetActive(false);
        var help=CriarTexto("AjudaAtlas",moldura.transform,"M/Esc fechar · Roda +/- zoom · Arraste/Setas mover · C Marcus · Z cidade",10,new Color(.84f,.8f,.7f),TextAlignmentOptions.Center);
        Posicionar(help.rectTransform,new Vector2(0,-165),new Vector2(604,16));
        plantaButton=CriarBotaoControleMapa(barraZoom.transform,"Planta / Cidade",new Vector2(0,25),new Vector2(98,18),AlternarPlanta).gameObject;
        notebook=gameObject.AddComponent<MarcusNotebookUI>();
        notebook.Initialize(canvas.transform,Fechar);
        painelRaiz.SetActive(false);
    }

    private Button CriarBotaoControleMapa(Transform pai, string rotulo, Vector2 pos, Vector2 tam, UnityEngine.Events.UnityAction acao)
    {
        var go = new GameObject("BtnMapa_" + rotulo, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(pai, false);
        Posicionar(go.GetComponent<RectTransform>(), pos, tam);
        var img = go.GetComponent<Image>();
        img.color = new Color(0.22f, 0.19f, 0.15f, 0.98f);

        var txt = CriarTexto("Lbl", go.transform, rotulo, 11f, new Color(0.96f, 0.90f, 0.72f), TextAlignmentOptions.Center);
        UIFactory.Stretch(txt.rectTransform);

        var btn = go.GetComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(acao);
        return btn;
    }

    private Button CriarBotaoAba(Transform pai, string rotulo, Vector2 pos, UnityEngine.Events.UnityAction acao)
    {
        var go = new GameObject("Aba_" + rotulo, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(pai, false);
        Posicionar(go.GetComponent<RectTransform>(), pos, new Vector2(42f, 20f));
        var img = go.GetComponent<Image>();
        img.color = new Color(0.20f, 0.18f, 0.15f, 1f);

        var txt = CriarTexto("Lbl", go.transform, rotulo, 11f, new Color(0.94f, 0.90f, 0.80f), TextAlignmentOptions.Center);
        UIFactory.Stretch(txt.rectTransform);
        txt.raycastTarget = true;

        var btn = go.GetComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(acao);
        return btn;
    }

    void Update()
    {
        if(Time.unscaledTime>=nextVisit)
        {
            nextVisit=Time.unscaledTime+.5f;
            var region=SantaRitaAtlas.Current();
            if(region!=null && region.id!=lastVisit)
            {
                var previous=SantaRitaAtlas.Region(lastVisit);
                if(region.interior&&previous!=null)
                {
                    string entry=previous.interior?GameState.Texto("atlas.entry."+previous.id):previous.id;
                    if(!string.IsNullOrEmpty(entry)&&GameState.Texto("atlas.entry."+region.id)!=entry)GameState.DefinirTexto("atlas.entry."+region.id,entry);
                }
                lastVisit=region.id;if(!GameState.Tem("atlas.visit."+region.id))GameState.Marcar("atlas.visit."+region.id);
            }
        }
        if (Keyboard.current == null || SantaRitaInventory.IsOpen || SantaRitaTapeDeck.IsOpen) return;
        if (MenuPausa.Aberto) return;

        string sceneName = SceneManager.GetActiveScene().name;
        bool cenaValida = sceneName == "EntradaDaCidade" || sceneName == "CasaDoAvo" || sceneName == "BarDoZe" || Capitulo2Director.CenaDoCapitulo2(sceneName);
        if (!cenaValida) return;
        // Sem legenda fixa no canto: o M é ensinado uma vez pelo DicaDeTecla.
        if (hudHintGo != null && hudHintGo.activeSelf) hudHintGo.SetActive(false);

        if(Keyboard.current.jKey.wasPressedThisFrame)
        {
            if(GameMessageUI.IsShowing||SinucaMinigameUI.IsShowing||BochaMinigameUI.IsShowing)return;
            if(notebook.IsOpen)Fechar();else Abrir(1);
            return;
        }
        if (Keyboard.current.mKey.wasPressedThisFrame || Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (GameMessageUI.IsShowing || SinucaMinigameUI.IsShowing || BochaMinigameUI.IsShowing) return;

            if (IsShowing)
                Fechar();
            else
                Abrir(0);
        }

        if (IsShowing)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Fechar();
                return;
            }
            if(notebook.IsOpen)
            {
                if(Keyboard.current.leftArrowKey.wasPressedThisFrame)notebook.Turn(-1);
                if(Keyboard.current.rightArrowKey.wasPressedThisFrame)notebook.Turn(1);
                return;
            }
            if (Keyboard.current.digit1Key.wasPressedThisFrame){pontoSelecionadoId="";SelecionarAba(0);}
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SelecionarAba(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) SelecionarAba(2);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) SelecionarAba(3);

            AtualizarPosicaoMarcadoresDinamicos();
            AtualizarZoomMapa();
            atlasView.Detail(mapZoom,!ocultarBlipsNoMapa);
        }
    }

    private void AlternarVisibilidadeBlips()
    {
        ocultarBlipsNoMapa=!ocultarBlipsNoMapa;
        atlasView.Invalidate();atlasView.Detail(mapZoom,!ocultarBlipsNoMapa);
    }

    private void ResetarZoomEPan()
    {
        var region=SantaRitaAtlas.Region(atlasView.Interior);
        Vector2 extent=region!=null?region.local.size:SantaRitaAtlas.Extent.size;
        float fit=Mathf.Min(604/extent.x,294/extent.y);mapBaseSize=extent*fit;
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
        if(atlasView.Interior!=null && SantaRitaAtlas.Current()?.id!=atlasView.Interior)
        {atlasView.ShowInterior(null);ResetarZoomEPan();AtualizarPosicaoMarcadoresDinamicos();}
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
        float novoZoom = Mathf.Clamp(mapZoom * fator, 1f, 12f);
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
        Vector2 limite = Vector2.Max(Vector2.zero,(mapBaseSize*mapZoom-mapViewportRect.rect.size)*.5f);
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
        if(notebook.IsOpen || abaAtiva!=0 || !GameState.Tem(Flags.MapaDoCarroObtido))return;
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
        bool sobrePainel=sidePanelObject.activeInHierarchy&&RectTransformUtility.RectangleContainsScreenPoint((RectTransform)sidePanelObject.transform,mouse,null);
        bool dentroDoMapa = !sobrePainel&&RectTransformUtility.RectangleContainsScreenPoint(mapViewportRect, mouse, null);
        if(sobrePainel){arrastandoMapa=false;return;}

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

        bool sobreControles=atlasControls.activeInHierarchy&&RectTransformUtility.RectangleContainsScreenPoint((RectTransform)atlasControls.transform,mouse,null);
        sobreControles|=plantaButton.activeInHierarchy&&RectTransformUtility.RectangleContainsScreenPoint((RectTransform)plantaButton.transform,mouse,null);
        if (botaoIniciou && dentroDoMapa && !sobreControles)
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
        if (!GameState.Tem(Flags.MapaDoCarroObtido) && abaInicial<=0) abaInicial=1;
        if (abaInicial >= 0)
        {
            abaAtiva = Mathf.Clamp(abaInicial, 0, 3);
        }
        painelRaiz.SetActive(true);
        mapAreaRect.gameObject.SetActive(GameState.Tem(Flags.MapaDoCarroObtido));
        atlasControls.SetActive(GameState.Tem(Flags.MapaDoCarroObtido));
        AtualizarDisponibilidadePlanta();
        if (mapAreaRect != null)
        {
            mapAreaRect.localScale = Vector3.one;
            mapAreaRect.sizeDelta = mapBaseSize * mapZoom;
            LimitarPanDoMapa();
        }
        atlasView.Rebuild();atlasView.Detail(mapZoom,!ocultarBlipsNoMapa);
        playerMarkerRect.SetAsLastSibling();
        AtualizarIndicadorZoom();
        SelecionarAba(abaAtiva);
        AtualizarPosicaoMarcadoresDinamicos();
    }

    public void Fechar()
    {
        arrastandoMapa = false;
        if(notebook!=null)notebook.Hide();
        if (painelRaiz != null)
            painelRaiz.SetActive(false);
    }

    private void SelecionarAba(int indice)
    {
        if(indice==0&&!GameState.Tem(Flags.MapaDoCarroObtido))
        {Legenda.Dizer("Ainda preciso encontrar um mapa. Minhas anotações estão no diário.",2.5f);indice=1;}
        if (diarioScroll != null) diarioScroll.verticalNormalizedPosition = 1;
        abaAtiva = Mathf.Clamp(indice, 0, 3);
        arrastandoMapa=false;
        bool writing=abaAtiva==1;
        mapFrame.SetActive(!writing);
        if(writing){painelRaiz.SetActive(false);notebook.Show();return;}
        notebook.Hide();painelRaiz.SetActive(true);
        sidePanelObject.SetActive(abaAtiva!=0 || !string.IsNullOrEmpty(pontoSelecionadoId));
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

    private void AtualizarPosicaoMarcadoresDinamicos()
    {
        var player=FindAnyObjectByType<PlayerController>();
        string nomeLocal;
        Vector2 playerUV=SantaRitaAtlas.Player(player!=null?(Vector2)player.transform.position:Vector2.zero,out nomeLocal);
        if(atlasView.Interior!=null)
        {
            var current=SantaRitaAtlas.Current();
            playerMarkerRect.gameObject.SetActive(current!=null&&current.id==atlasView.Interior);
            if(player!=null)playerUV=atlasView.Project(player.transform.position);
        }
        else playerMarkerRect.gameObject.SetActive(true);
        playerMarkerRect.anchorMin=playerMarkerRect.anchorMax=playerUV;
        playerMarkerRect.anchoredPosition=Vector2.zero;
        playerMarkerImage.color=new Color(.77f,.87f,.79f);
        string objStr = GameState.ObterObjetivo();
        if (string.IsNullOrEmpty(objStr))
            objStr = "Investigar a casa do avô Adílio no Bairro das Casas, nº 40.";

        if (textoLocalAtual != null)
            textoLocalAtual.text = "LOCAL: " + nomeLocal.ToUpperInvariant();
        if (textoObjetivoAtual != null)
            textoObjetivoAtual.text = "OBJETIVO: " + objStr;
    }



    private void AtualizarPainelLateral()
    {
        if (textoStatusSave != null)
        {
            textoStatusSave.text = GameState.TemProgressoNaoSalvo
                ? "Progresso não salvo. Use um orelhão."
                : (CheckpointSystem.HasCheckpoint ? "Progresso registrado no orelhão." : "Nenhum registro no orelhão ainda.");
        }

        textoDiarioConteudo.fontSize = abaAtiva == 3 ? 10.5f : 11.5f;
        if (abaAtiva == 0)
        {
            var landmark=SantaRitaAtlas.Survey.landmarks.Find(l=>l.id==pontoSelecionadoId);
            string name=NomeMarco(pontoSelecionadoId);
            string building=landmark!=null?SantaRitaAtlas.Building(landmark):null;
            string note=SantaRitaAtlas.Annotation(building??pontoSelecionadoId);
            if(building=="mercadinho")note="Um único mercadinho: fachada na Encruzilhada e acesso de serviço pelo Centro.\n\n"+note;
            textoDiarioConteudo.text=landmark==null ? "Selecione um acesso para consultar o levantamento.\n\nA cidade permanece visível; as anotações dependem da investigação." :
                "<b>"+name+"</b>\n\n"+(landmark.phone?"Telefone público. Salvamento manual.":"Acesso identificado no levantamento.")+"\n\n"+note+"\n\nPlanta / Cidade: ambientes já visitados.";
        }
        else if (abaAtiva == 2)
        {
            var sb = new System.Text.StringBuilder("<b>FICHAS DO CLUBE</b>\n\n");
            if(GameState.Tem(Flags.RecebeuFichaClubeZe))sb.Append("<color=#D6BC78>Ficha nº 1 — Bar do Zé</color>\nLatão gasto. Zé me entregou depois da sinuca.\n\n");
            if(GameState.Tem(Capitulo2Director.Ficha))sb.Append("<color=#D6BC78>Ficha nº 2 — Sociedade Beira-Rio</color>\nA ficha que recebi de Tonão depois da Bocha.\n\n");
            if(GameState.Tem("ficha_clube_truco_03"))sb.Append("<color=#D6BC78>Ficha nº 3 — Galpão do Truco</color>\n\n");
            if(!GameState.Tem(Flags.RecebeuFichaClubeZe)&&!GameState.Tem(Capitulo2Director.Ficha)&&!GameState.Tem("ficha_clube_truco_03"))sb.Append("Ainda não guardei nenhuma ficha.");
            textoDiarioConteudo.text=sb.ToString();
        }

        else
        {
            textoDiarioConteudo.text = "<b>SÍMBOLOS DO MAPA</b>\n\nVOCÊ — Marcus\nQuadrado ocre — entrada/saída\nQuadrado verde — orelhão\nObjetivo conhecido — anotação atual\nAcessos ligam as regiões; não há rota GPS\n\n< b>ESCALAS</b>\n1x: cidade\n1,8x: acessos e mobiliário\n3,5x: detalhes e notas\n\nPlantas: só ambientes visitados.\nM/ESC: fechar • C: Marcus • Z: cidade".Replace("< b>","<b>");
        }
    }

    private string NomeMarco(string id)
    {
        var p=pontos.Find(x=>x.id==id);
        if(!string.IsNullOrEmpty(p.id))return p.nome.Split('—')[0].Trim();
        var l=SantaRitaAtlas.Survey.landmarks.Find(x=>x.id==id);
        return l!=null?(l.phone?"Orelhão":"Acesso • "+SantaRitaAtlas.Region(l.region).name):"";
    }
    private void SelecionarMarcoAtlas(string id)
    {
        if(arrastouNoCliqueAtual)return;
        pontoSelecionadoId=id;SelecionarAba(0);AtualizarDisponibilidadePlanta();
    }
    private void AtualizarDisponibilidadePlanta()
    {
        var current=SantaRitaAtlas.Current();
        var landmark=SantaRitaAtlas.Survey.landmarks.Find(l=>l.id==pontoSelecionadoId);
        bool visited=landmark!=null&&SantaRitaAtlas.Survey.regions.Exists(r=>r.interior&&SantaRitaAtlas.Known(r)&&(r.id==landmark.destination||r.scene==landmark.destination));
        plantaButton.SetActive(atlasView.Interior!=null || (current!=null&&current.interior) || visited);
    }
    private void AlternarPlanta()
    {
        if(atlasView.Interior!=null){atlasView.ShowInterior(null);ResetarZoomEPan();return;}
        var landmark=SantaRitaAtlas.Survey.landmarks.Find(l=>l.id==pontoSelecionadoId);
        var r=SantaRitaAtlas.Survey.regions.Find(x=>x.interior && (x.building==pontoSelecionadoId || (landmark!=null&&(x.id==landmark.destination||x.scene==landmark.destination))) && SantaRitaAtlas.Known(x));
        if(r==null){r=SantaRitaAtlas.Current();if(r==null||!r.interior||!SantaRitaAtlas.Known(r))return;}
        atlasView.ShowInterior(r.id);ResetarZoomEPan();
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

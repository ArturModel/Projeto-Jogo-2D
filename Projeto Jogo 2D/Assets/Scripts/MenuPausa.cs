using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// Menu de pausa, aberto com ESC durante o jogo.
//
// O VISUAL é próprio da pausa: uma caixa no centro, com a mesma moldura e a
// mesma fonte das caixas de diálogo, sobre o jogo escurecido. Nada do visual
// do menu principal (vídeo de fundo, coluna de botões, título grande).
//
// A LÓGICA é a do menu principal, sem cópia: os mesmos componentes cuidam de
// tudo o que o menu já sabia fazer —
//   MenuPanelTransition         troca pausa <-> configurações
//   OptionsPanelController      categorias, subpainéis e ESC para voltar
//   AudioSettingsController     volumes no mixer, mesmas chaves salvas
//   VideoSettingsController     tela cheia, resolução, vsync, brilho
//   SubtitleSettingsController  legendas ativas, tamanho, fundo
//   MenuItemSelector            indicador ">" e som de seleção
// Mudar uma opção na pausa é mudar a mesma opção do menu, e vice-versa.
public class MenuPausa : MonoBehaviour
{
    [Header("Recursos do menu principal")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioClip somSelecao;

    [Header("Comportamento")]
    [SerializeField] private string cenaMenu = "MainMenu";

    [Range(0f, 1f)]
    [SerializeField] private float escurecimento = 0.6f;

    [Header("Visual")]
    [SerializeField] private Color corTexto = new Color(0.88f, 0.86f, 0.80f, 1f);
    [SerializeField] private Color corDestaque = new Color(0.95f, 0.72f, 0.36f, 1f);
    [SerializeField] private Color corControle = new Color(0.14f, 0.13f, 0.12f, 0.95f);

    public static bool Aberto { get; private set; }

    public static bool ReduzirEfeitosIntensos
    {
        get => PlayerPrefs.GetInt("reduzir_efeitos_intensos", 0) == 1;
        set
        {
            PlayerPrefs.SetInt("reduzir_efeitos_intensos", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    private GameObject raiz;
    private CanvasGroup painelOpcoes;
    private GameObject primeiroBotao;
    private AudioSource fonteSom;
    private int quadroDoUltimoEsc = -1;
    private TMP_Text rotuloStatusSave;
    private bool aguardandoConfirmacaoSaida;
    private System.Action acaoConfirmada;

    private const float LarguraPausa = 276f;
    private const float LarguraOpcoes = 370f;

    private void Awake()
    {
        Aberto = false;
        Construir();
    }

    private void Update()
    {
        if (Keyboard.current == null ||
            !Keyboard.current.escapeKey.wasPressedThisFrame ||
            quadroDoUltimoEsc == Time.frameCount)
        {
            return;
        }

        // Se o mapa, a sinuca ou a seleção de slots do orelhão estiverem abertos, o ESC pertence a eles.
        if (!Aberto && (MapaInterativoUI.IsShowing || SinucaMinigameUI.IsShowing || PayphoneCheckpoint.IsSaveUIOpen))
        {
            return;
        }

        quadroDoUltimoEsc = Time.frameCount;

        if (!Aberto)
        {
            Abrir();
            return;
        }

        // Dentro das configurações o ESC é do OptionsPanelController (volta de
        // painel, e na raiz fecha as configurações). Só fecha a pausa a partir
        // da caixa de botões.
        if (!painelOpcoes.gameObject.activeInHierarchy)
        {
            Fechar();
        }
    }

    public void Abrir()
    {
        Aberto = true;
        aguardandoConfirmacaoSaida = false;
        AtualizarTextoStatusSave();
        raiz.SetActive(true);
        Time.timeScale = 0f;

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(primeiroBotao);
        }
    }

    public void Fechar()
    {
        Aberto = false;
        aguardandoConfirmacaoSaida = false;
        Time.timeScale = 1f;

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        raiz.SetActive(false);
    }

    public void VoltarAoMenu()
    {
        if (GameState.TemProgressoNaoSalvo && !aguardandoConfirmacaoSaida)
        {
            aguardandoConfirmacaoSaida = true;
            if (rotuloStatusSave != null)
            {
                rotuloStatusSave.text = "Há progresso não salvo! Clique novamente para sair sem salvar.";
                rotuloStatusSave.color = new Color(0.92f, 0.38f, 0.32f, 1f);
            }
            return;
        }

        Fechar();
        SceneFader.Instance.LoadScene(cenaMenu);
    }

    public void Sair()
    {
        if (GameState.TemProgressoNaoSalvo && !aguardandoConfirmacaoSaida)
        {
            aguardandoConfirmacaoSaida = true;
            if (rotuloStatusSave != null)
            {
                rotuloStatusSave.text = "Progresso não salvo! Clique em SAIR novamente para confirmar.";
                rotuloStatusSave.color = new Color(0.92f, 0.38f, 0.32f, 1f);
            }
            return;
        }

        Fechar();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void AtualizarTextoStatusSave()
    {
        if (rotuloStatusSave == null)
        {
            return;
        }

        if (GameState.TemProgressoNaoSalvo)
        {
            rotuloStatusSave.text = "Progresso não salvo (salve em um Orelhão)";
            rotuloStatusSave.color = new Color(0.86f, 0.66f, 0.34f, 0.95f);
        }
        else if (CheckpointSystem.HasCheckpoint)
        {
            rotuloStatusSave.text = "Salvo no último Orelhão";
            rotuloStatusSave.color = new Color(0.58f, 0.76f, 0.58f, 0.95f);
        }
        else
        {
            rotuloStatusSave.text = "Orelhões salvam a investigação";
            rotuloStatusSave.color = new Color(0.68f, 0.65f, 0.58f, 0.85f);
        }
    }

    private void OnDestroy()
    {
        // Trocar de cena com a pausa aberta não pode deixar o jogo congelado.
        if (Aberto)
        {
            Aberto = false;
            Time.timeScale = 1f;
        }
    }

    // =================================================================
    // CONSTRUÇÃO
    // =================================================================

    private void Construir()
    {
        GarantirEventSystem();

        // Tudo é montado com a raiz INATIVA: MenuPanelTransition e
        // OptionsPanelController leem as referências no Awake, que só roda
        // quando a pausa abre pela primeira vez — já com tudo vinculado.
        raiz = new GameObject("PausaUI");
        raiz.transform.SetParent(transform, false);
        raiz.SetActive(false);

        fonteSom = raiz.AddComponent<AudioSource>();
        fonteSom.playOnAwake = false;
        fonteSom.ignoreListenerPause = true;

        Canvas canvas = UIFactory.CreateCanvas(raiz.transform, "PausaCanvas", 9500);
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        Transform tela = canvas.transform;

        Image fundo = Imagem(tela, "Fundo", new Color(0f, 0f, 0f, escurecimento));
        UIFactory.Stretch(fundo.rectTransform);

        // escurece um pouco mais quando as configurações estão abertas
        Image foco = Imagem(tela, "Foco", new Color(0f, 0f, 0f, 0.5f));
        UIFactory.Stretch(foco.rectTransform);
        CanvasGroup overlay = foco.gameObject.AddComponent<CanvasGroup>();
        overlay.alpha = 0f;
        overlay.blocksRaycasts = false;

        // --- caixa da pausa ---------------------------------------------
        CanvasGroup pausa = Caixa(tela, "PainelPausa", new Vector2(LarguraPausa, 252f),
                                  out RectTransform pausaRect);
        Titulo(pausaRect, "PAUSADO", 92f, LarguraPausa);
        RectTransform indPausa = Indicador(pausaRect, -LarguraPausa / 2f + 22f);

        Button continuar = Botao(pausaRect, "CONTINUAR", 48f, indPausa);
        Button mapaBtn = Botao(pausaRect, "MAPA / DIÁRIO (M)", 18f, indPausa);
        Button opcoes = Botao(pausaRect, "CONFIGURAÇÕES", -12f, indPausa);
        Button menu = Botao(pausaRect, "MENU PRINCIPAL", -42f, indPausa);
        Button sair = Botao(pausaRect, "SAIR", -72f, indPausa);

        rotuloStatusSave = Rotulo(pausaRect, "", 13f, new Vector2(0f, -104f),
                                  new Vector2(LarguraPausa - 24f, 24f), TextAlignmentOptions.Center);
        rotuloStatusSave.textWrappingMode = TextWrappingModes.Normal;

        continuar.onClick.AddListener(Fechar);
        mapaBtn.onClick.AddListener(() =>
        {
            Fechar();
            MapaInterativoUI.EnsureInstance().Abrir(0);
        });
        menu.onClick.AddListener(VoltarAoMenu);
        sair.onClick.AddListener(Sair);
        primeiroBotao = continuar.gameObject;

        // --- caixa das configurações ------------------------------------
        painelOpcoes = Caixa(tela, "PainelOpcoes", new Vector2(LarguraOpcoes, 290f),
                             out RectTransform opcoesRect);
        TMP_Text tituloOpcoes = Titulo(opcoesRect, "CONFIGURAÇÕES", 114f, LarguraOpcoes);
        float xInd = -LarguraOpcoes / 2f + 22f;

        // categorias
        CanvasGroup categorias = SubPainel(opcoesRect, "Categorias", out RectTransform catRect);
        RectTransform indCat = Indicador(catRect, xInd);
        Button bAudio = Botao(catRect, "ÁUDIO", 44f, indCat);
        Button bVideo = Botao(catRect, "VÍDEO", 14f, indCat);
        Button bLegendas = Botao(catRect, "LEGENDAS", -16f, indCat);
        Button bVoltarCat = Botao(catRect, "VOLTAR", -76f, indCat);

        // áudio
        CanvasGroup audioGrupo = SubPainel(opcoesRect, "Audio", out RectTransform audioRect);
        RectTransform indAudio = Indicador(audioRect, xInd);
        Slider geral = LinhaSlider(audioRect, "VOLUME GERAL", 54f, 0f, 1f, false, indAudio, out _);
        Slider musica = LinhaSlider(audioRect, "MÚSICA", 24f, 0f, 1f, false, indAudio, out _);
        Slider efeitos = LinhaSlider(audioRect, "EFEITOS", -6f, 0f, 1f, false, indAudio, out _);
        Button bVoltarAudio = Botao(audioRect, "VOLTAR", -76f, indAudio);

        // vídeo
        CanvasGroup videoGrupo = SubPainel(opcoesRect, "Video", out RectTransform videoRect);
        RectTransform indVideo = Indicador(videoRect, xInd);
        Toggle telaCheia = LinhaToggle(videoRect, "TELA CHEIA", 70f, indVideo);
        TMP_Dropdown resolucao = LinhaDropdown(videoRect, "RESOLUÇÃO", 42f, indVideo);
        Toggle vsync = LinhaToggle(videoRect, "VSYNC", 14f, indVideo);
        Slider brilho = LinhaSlider(videoRect, "BRILHO", -14f, 0f, 1f, false, indVideo,
                                    out TMP_Text valorBrilho);
        Toggle efeitosSuaves = LinhaToggle(videoRect, "EFEITOS SUAVES", -42f, indVideo);
        efeitosSuaves.SetIsOnWithoutNotify(ReduzirEfeitosIntensos);
        efeitosSuaves.onValueChanged.AddListener(v => ReduzirEfeitosIntensos = v);
        Button bVoltarVideo = Botao(videoRect, "VOLTAR", -86f, indVideo);

        // legendas
        CanvasGroup legendasGrupo = SubPainel(opcoesRect, "Legendas", out RectTransform legRect);
        RectTransform indLeg = Indicador(legRect, xInd);
        Toggle ativas = LinhaToggle(legRect, "LEGENDAS ATIVAS", 60f, indLeg);
        Slider tamanho = LinhaSlider(legRect, "TAMANHO", 30f, 50f, 150f, true, indLeg,
                                     out TMP_Text valorTamanho);
        Toggle fundoLegenda = LinhaToggle(legRect, "FUNDO ESCURO", 0f, indLeg);

        Image fundoPreview = Imagem(legRect, "FundoPreview", new Color(0f, 0f, 0f, 0.65f));
        Posicionar(fundoPreview.rectTransform, new Vector2(0f, -36f), new Vector2(230f, 28f));
        TMP_Text preview = Rotulo(legRect, "Exemplo de legenda", 18f, new Vector2(0f, -36f),
                                  new Vector2(230f, 28f), TextAlignmentOptions.Center);
        Button bVoltarLeg = Botao(legRect, "VOLTAR", -80f, indLeg);

        // --- a lógica do menu principal ---------------------------------
        MenuPanelTransition transicao = raiz.AddComponent<MenuPanelTransition>();
        transicao.Vincular(pausa, painelOpcoes, overlay, pausaRect, opcoesRect,
                           continuar.gameObject, bAudio.gameObject);

        OptionsPanelController painel = painelOpcoes.gameObject.AddComponent<OptionsPanelController>();
        painel.Vincular(categorias, audioGrupo, videoGrupo, legendasGrupo, tituloOpcoes,
                        bAudio.gameObject, geral.gameObject, telaCheia.gameObject,
                        ativas.gameObject, transicao);

        opcoes.onClick.AddListener(transicao.AbrirOpcoes);
        bVoltarCat.onClick.AddListener(transicao.FecharOpcoes);
        bAudio.onClick.AddListener(painel.AbrirAudio);
        bVideo.onClick.AddListener(painel.AbrirVideo);
        bLegendas.onClick.AddListener(painel.AbrirLegendas);
        bVoltarAudio.onClick.AddListener(painel.MostrarCategorias);
        bVoltarVideo.onClick.AddListener(painel.MostrarCategorias);
        bVoltarLeg.onClick.AddListener(painel.MostrarCategorias);

        raiz.AddComponent<AudioSettingsController>().Vincular(mixer, geral, musica, efeitos);

        // brilho sem alvo: no menu ele escurece o vídeo de fundo, que aqui não existe
        raiz.AddComponent<VideoSettingsController>().Vincular(
            telaCheia, resolucao, vsync, brilho, valorBrilho, null);

        legendasGrupo.gameObject.AddComponent<SubtitleSettingsController>().Vincular(
            ativas, tamanho, fundoLegenda, valorTamanho, preview, fundoPreview);
    }

    private void GarantirEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject es = new GameObject("EventSystem",
            typeof(EventSystem), typeof(InputSystemUIInputModule));
        es.transform.SetParent(transform, false);
        es.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
    }

    // --- peças ---------------------------------------------------------

    private Image Imagem(Transform pai, string nome, Color cor)
    {
        Image img = new GameObject(nome, typeof(RectTransform)).AddComponent<Image>();
        img.transform.SetParent(pai, false);
        img.color = cor;
        return img;
    }

    private static void Posicionar(RectTransform r, Vector2 posicao, Vector2 tamanho)
    {
        r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = posicao;
        r.sizeDelta = tamanho;
    }

    // A moldura é a mesma das caixas de diálogo do jogo, com toque de papel envelhecido e flor.
    private CanvasGroup Caixa(Transform pai, string nome, Vector2 tamanho, out RectTransform rect)
    {
        Image caixa = UIFactory.CreateBox(pai, nome, "DialogueBox");
        caixa.raycastTarget = true;
        caixa.color = new Color(0.95f, 0.90f, 0.80f, 0.98f);
        rect = caixa.rectTransform;
        Posicionar(rect, Vector2.zero, tamanho);

        // Linha de margem vermelha discreta à esquerda (caderno gasto)
        Image margem = Imagem(rect, "LinhaMargemVermelha", new Color(0.62f, 0.22f, 0.20f, 0.28f));
        margem.raycastTarget = false;
        Posicionar(margem.rectTransform, new Vector2(-tamanho.x * 0.5f + 16f, 0f), new Vector2(1.5f, tamanho.y - 24f));

        // Pequena flor seca nos cantos superiores
        TMP_Text florEsq = Rotulo(rect, "*", 16f, new Vector2(-tamanho.x * 0.5f + 16f, tamanho.y * 0.5f - 14f),
                                  new Vector2(20f, 20f), TextAlignmentOptions.Center);
        florEsq.color = new Color(0.76f, 0.34f, 0.32f, 0.65f);
        florEsq.raycastTarget = false;

        TMP_Text florDir = Rotulo(rect, "*", 16f, new Vector2(tamanho.x * 0.5f - 16f, tamanho.y * 0.5f - 14f),
                                  new Vector2(20f, 20f), TextAlignmentOptions.Center);
        florDir.color = new Color(0.76f, 0.34f, 0.32f, 0.65f);
        florDir.raycastTarget = false;

        return caixa.gameObject.AddComponent<CanvasGroup>();
    }

    private static CanvasGroup SubPainel(RectTransform pai, string nome, out RectTransform rect)
    {
        GameObject go = new GameObject(nome, typeof(RectTransform));
        go.transform.SetParent(pai, false);
        rect = (RectTransform)go.transform;
        UIFactory.Stretch(rect);
        return go.AddComponent<CanvasGroup>();
    }

    private TMP_Text Rotulo(Transform pai, string texto, float tamanho, Vector2 posicao,
                            Vector2 dimensao, TextAlignmentOptions alinhamento)
    {
        TextMeshProUGUI rotulo = UIFactory.CreateLabel(pai, "Texto", tamanho);
        rotulo.text = texto;
        rotulo.color = corTexto;
        rotulo.alignment = alinhamento;
        rotulo.textWrappingMode = TextWrappingModes.NoWrap;
        Posicionar(rotulo.rectTransform, posicao, dimensao);
        return rotulo;
    }

    private TMP_Text Titulo(RectTransform pai, string texto, float y, float largura)
    {
        TMP_Text titulo = Rotulo(pai, texto, 30f, new Vector2(0f, y),
                                 new Vector2(largura - 20f, 36f), TextAlignmentOptions.Center);
        titulo.color = corDestaque;
        return titulo;
    }

    private RectTransform Indicador(RectTransform pai, float x)
    {
        TMP_Text seta = Rotulo(pai, ">", 22f, new Vector2(x, 0f), new Vector2(20f, 26f),
                               TextAlignmentOptions.Center);
        seta.color = corDestaque;
        return seta.rectTransform;
    }

    private void Selecionavel(Selectable s, Graphic alvo, RectTransform indicador)
    {
        s.targetGraphic = alvo;
        ColorBlock cores = s.colors;
        cores.normalColor = Color.white;
        cores.highlightedColor = corDestaque;
        cores.selectedColor = corDestaque;
        cores.pressedColor = corDestaque * 0.8f;
        cores.fadeDuration = 0.05f;
        s.colors = cores;

        s.gameObject.AddComponent<MenuItemSelector>().Vincular(indicador, fonteSom, somSelecao);
    }

    private Button Botao(RectTransform pai, string texto, float y, RectTransform indicador)
    {
        GameObject go = new GameObject(texto, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(pai, false);
        Posicionar((RectTransform)go.transform, new Vector2(0f, y), new Vector2(210f, 26f));
        go.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);   // só área de clique

        TMP_Text rotulo = Rotulo(go.transform, texto, 22f, Vector2.zero,
                                 new Vector2(210f, 26f), TextAlignmentOptions.Center);

        Button botao = go.GetComponent<Button>();
        Selecionavel(botao, rotulo, indicador);
        return botao;
    }

    private Slider LinhaSlider(RectTransform pai, string texto, float y, float min, float max,
                               bool inteiros, RectTransform indicador, out TMP_Text valor)
    {
        Rotulo(pai, texto, 18f, new Vector2(-72f, y), new Vector2(140f, 24f),
               TextAlignmentOptions.Left);

        GameObject go = DefaultControls.CreateSlider(new DefaultControls.Resources());
        go.transform.SetParent(pai, false);
        Posicionar((RectTransform)go.transform, new Vector2(60f, y), new Vector2(110f, 10f));

        go.transform.Find("Background").GetComponent<Image>().color = corControle;
        go.transform.Find("Fill Area/Fill").GetComponent<Image>().color = corDestaque * 0.8f;
        Image alca = go.transform.Find("Handle Slide Area/Handle").GetComponent<Image>();
        alca.rectTransform.sizeDelta = new Vector2(8f, 6f);
        alca.color = corTexto;

        Slider slider = go.GetComponent<Slider>();
        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = inteiros;
        Selecionavel(slider, alca, indicador);

        valor = Rotulo(pai, "", 18f, new Vector2(145f, y), new Vector2(48f, 24f),
                       TextAlignmentOptions.Right);
        return slider;
    }

    private Toggle LinhaToggle(RectTransform pai, string texto, float y, RectTransform indicador)
    {
        Rotulo(pai, texto, 18f, new Vector2(-72f, y), new Vector2(140f, 24f),
               TextAlignmentOptions.Left);

        GameObject go = DefaultControls.CreateToggle(new DefaultControls.Resources());
        go.transform.SetParent(pai, false);
        Posicionar((RectTransform)go.transform, new Vector2(20f, y), new Vector2(18f, 18f));

        // o rótulo padrão é Text legado; o texto da linha já está ao lado
        Destroy(go.transform.Find("Label").gameObject);

        Image caixa = go.transform.Find("Background").GetComponent<Image>();
        caixa.color = corControle;
        UIFactory.Stretch(caixa.rectTransform);
        Image marca = go.transform.Find("Background/Checkmark").GetComponent<Image>();
        marca.color = corDestaque;

        Toggle toggle = go.GetComponent<Toggle>();
        Selecionavel(toggle, caixa, indicador);
        return toggle;
    }

    private TMP_Dropdown LinhaDropdown(RectTransform pai, string texto, float y,
                                       RectTransform indicador)
    {
        Rotulo(pai, texto, 18f, new Vector2(-72f, y), new Vector2(140f, 24f),
               TextAlignmentOptions.Left);

        GameObject go = TMP_DefaultControls.CreateDropdown(new TMP_DefaultControls.Resources());
        go.transform.SetParent(pai, false);
        Posicionar((RectTransform)go.transform, new Vector2(80f, y), new Vector2(150f, 22f));

        TMP_Dropdown dropdown = go.GetComponent<TMP_Dropdown>();
        Image fundo = go.GetComponent<Image>();
        fundo.color = corControle;

        foreach (TMP_Text t in go.GetComponentsInChildren<TMP_Text>(true))
        {
            if (UIFactory.Font != null)
            {
                t.font = UIFactory.Font;
            }

            t.fontSize = 16f;
            t.color = corTexto;
        }

        dropdown.template.GetComponent<Image>().color = corControle;
        Transform item = dropdown.template.Find("Viewport/Content/Item");
        if (item != null)
        {
            item.Find("Item Background").GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.08f);
            item.Find("Item Checkmark").GetComponent<Image>().color = corDestaque;
        }

        Selecionavel(dropdown, fundo, indicador);
        return dropdown;
    }
}

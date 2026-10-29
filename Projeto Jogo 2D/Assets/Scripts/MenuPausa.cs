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

    private const float LarguraPausa = 246f;
    private const float AlturaPausa = 280f;
    private const float LarguraOpcoes = 246f;

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

        // Sinuca usa esta pausa global; mapa/Bocha/orelhão possuem fechamento próprio.
        if (!Aberto && (MapaInterativoUI.IsShowing || BochaMinigameUI.IsShowing || PayphoneCheckpoint.IsSaveUIOpen))
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
        if (SinucaMinigameUI.IsShowing) SinucaMinigameUI.Instance.CancelarCargaAoSuspender();
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

        if (SinucaMinigameUI.IsShowing) SinucaMinigameUI.Instance.Fechar();
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
            rotuloStatusSave.text = "Progresso não salvo";
            rotuloStatusSave.color = new Color(0.86f, 0.66f, 0.34f, 0.95f);
        }
        else if (CheckpointSystem.HasCheckpoint)
        {
            rotuloStatusSave.text = "Jogo salvo";
            rotuloStatusSave.color = new Color(0.58f, 0.76f, 0.58f, 0.95f);
        }
        else
        {
            rotuloStatusSave.text = "Progresso não salvo";
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

        // --- caixa da pausa (placa de ferro oxidada à esquerda) --------
        CanvasGroup pausa = CaixaPausa(tela, "PainelPausa", new Vector2(LarguraPausa, AlturaPausa),
                                       out RectTransform pausaRect);
        TituloPausa(pausaRect, "PAUSADO", 96f, LarguraPausa);
        RectTransform indPausa = IndicadorPausa(pausaRect);

        Button continuar = BotaoPausa(pausaRect, "Continuar", 44f, indPausa);
        Button opcoes = BotaoPausa(pausaRect, "Configurações", 12f, indPausa);
        Button menu = BotaoPausa(pausaRect, "Menu Principal", -20f, indPausa);
        Button sair = BotaoPausa(pausaRect, "Sair", -52f, indPausa);

        rotuloStatusSave = Rotulo(pausaRect, "", 11f, new Vector2(0f, -105f),
                                  new Vector2(LarguraPausa - 30f, 22f), TextAlignmentOptions.Center);
        rotuloStatusSave.color = new Color(0.65f, 0.62f, 0.56f, 0.75f);
        rotuloStatusSave.textWrappingMode = TextWrappingModes.Normal;

        continuar.onClick.AddListener(Fechar);
        menu.onClick.AddListener(VoltarAoMenu);
        sair.onClick.AddListener(Sair);
        primeiroBotao = continuar.gameObject;

        // --- caixa das configurações (mesma placa de ferro à esquerda) --
        painelOpcoes = CaixaPausa(tela, "PainelOpcoes", new Vector2(LarguraPausa, AlturaPausa),
                                  out RectTransform opcoesRect);
        TMP_Text tituloOpcoes = Titulo(opcoesRect, "CONFIGURAÇÕES", 92f, LarguraPausa);
        float xInd = -88f;

        // categorias (mesmo estilo de botões e papiro da pausa)
        CanvasGroup categorias = SubPainel(opcoesRect, "Categorias", out RectTransform catRect);
        RectTransform indCat = IndicadorPausa(catRect);
        Button bAudio = BotaoPausa(catRect, "Áudio", 44f, indCat);
        Button bVideo = BotaoPausa(catRect, "Vídeo", 12f, indCat);
        Button bLegendas = BotaoPausa(catRect, "Legendas", -20f, indCat);
        Button bVoltarCat = BotaoPausa(catRect, "Voltar", -52f, indCat);

        // áudio
        CanvasGroup audioGrupo = SubPainel(opcoesRect, "Audio", out RectTransform audioRect);
        RectTransform indAudio = Indicador(audioRect, xInd);
        Slider geral = LinhaSlider(audioRect, "VOLUME GERAL", 44f, 0f, 1f, false, indAudio, out _);
        Slider musica = LinhaSlider(audioRect, "MÚSICA", 12f, 0f, 1f, false, indAudio, out _);
        Slider efeitos = LinhaSlider(audioRect, "EFEITOS", -20f, 0f, 1f, false, indAudio, out _);
        Button bVoltarAudio = Botao(audioRect, "VOLTAR", -58f, indAudio);

        // vídeo
        CanvasGroup videoGrupo = SubPainel(opcoesRect, "Video", out RectTransform videoRect);
        RectTransform indVideo = Indicador(videoRect, xInd);
        Toggle telaCheia = LinhaToggle(videoRect, "TELA CHEIA", 48f, indVideo);
        TMP_Dropdown resolucao = LinhaStepperResolucao(videoRect, "RESOLUÇÃO", 24f, indVideo);
        Toggle vsync = LinhaToggle(videoRect, "VSYNC", 0f, indVideo);
        Slider brilho = LinhaSlider(videoRect, "BRILHO", -24f, 0f, 1f, false, indVideo,
                                    out TMP_Text valorBrilho);
        Toggle efeitosSuaves = LinhaToggle(videoRect, "EFEITOS SUAVES", -48f, indVideo);
        efeitosSuaves.SetIsOnWithoutNotify(ReduzirEfeitosIntensos);
        efeitosSuaves.onValueChanged.AddListener(v => ReduzirEfeitosIntensos = v);
        Button bVoltarVideo = Botao(videoRect, "VOLTAR", -74f, indVideo);

        // legendas
        CanvasGroup legendasGrupo = SubPainel(opcoesRect, "Legendas", out RectTransform legRect);
        RectTransform indLeg = Indicador(legRect, xInd);
        Toggle ativas = LinhaToggle(legRect, "LEGENDAS ATIVAS", 48f, indLeg);
        Slider tamanho = LinhaSlider(legRect, "TAMANHO", 22f, 50f, 150f, true, indLeg,
                                     out TMP_Text valorTamanho);
        Toggle fundoLegenda = LinhaToggle(legRect, "FUNDO ESCURO", -4f, indLeg);

        // Card refinado de preview de legenda
        Image molduraPreview = Imagem(legRect, "MolduraPreview", new Color(0.35f, 0.30f, 0.25f, 0.6f));
        Posicionar(molduraPreview.rectTransform, new Vector2(0f, -36f), new Vector2(144f, 20f));
        molduraPreview.raycastTarget = false;

        Image fundoPreview = Imagem(molduraPreview.transform, "FundoPreview", new Color(0.04f, 0.04f, 0.04f, 0.85f));
        Posicionar(fundoPreview.rectTransform, Vector2.zero, new Vector2(142f, 18f));
        fundoPreview.raycastTarget = false;

        TMP_Text preview = Rotulo(molduraPreview.transform, "Exemplo de legenda", 13f, Vector2.zero,
                                  new Vector2(140f, 18f), TextAlignmentOptions.Center);
        Button bVoltarLeg = Botao(legRect, "VOLTAR", -74f, indLeg);

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

    // A placa de ferro oxidada do menu de pausa com o lírio gravado no canto, ancorada à esquerda
    private CanvasGroup CaixaPausa(Transform pai, string nome, Vector2 tamanho, out RectTransform rect)
    {
        GameObject go = new GameObject(nome, typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        go.transform.SetParent(pai, false);
        Image img = go.GetComponent<Image>();
        Sprite spr = UIFactory.LoadSprite("PainelPausa_Fundo");
        if (spr != null)
        {
            img.sprite = spr;
            img.type = Image.Type.Simple;
            img.color = Color.white;
        }
        else
        {
            img.color = new Color(0.12f, 0.11f, 0.10f, 0.95f);
        }
        img.raycastTarget = true;
        rect = go.GetComponent<RectTransform>();
        Posicionar(rect, new Vector2(-160f, 0f), tamanho);
        return go.GetComponent<CanvasGroup>();
    }

    private TMP_Text TituloPausa(RectTransform pai, string texto, float y, float largura)
    {
        Sprite sprTitulo = UIFactory.LoadSprite("PainelPausa_Titulo");
        if (sprTitulo != null)
        {
            GameObject tituloGo = new GameObject("TituloGrafico", typeof(RectTransform), typeof(Image));
            tituloGo.transform.SetParent(pai, false);
            Image tituloImg = tituloGo.GetComponent<Image>();
            tituloImg.sprite = sprTitulo;
            tituloImg.color = Color.white;
            tituloImg.raycastTarget = false;
            // O sprite de título contém o carimbo estilizado 'PAUSADO' e a linha vermelha rústica original.
            // Proporções: 243x78 px -> em UI proporcional à placa 246x280 fica 132x42 px.
            Posicionar((RectTransform)tituloGo.transform, new Vector2(0f, 85f), new Vector2(132f, 42f));
            return null;
        }

        TMP_Text titulo = Rotulo(pai, texto, 28f, new Vector2(0f, y),
                                 new Vector2(largura - 40f, 34f), TextAlignmentOptions.Center);
        titulo.color = new Color(0.88f, 0.85f, 0.78f, 1f);

        Sprite sprLinha = UIFactory.LoadSprite("PainelPausa_Linha");
        if (sprLinha != null)
        {
            GameObject linhaGo = new GameObject("LinhaVermelha", typeof(RectTransform), typeof(Image));
            linhaGo.transform.SetParent(pai, false);
            Image linhaImg = linhaGo.GetComponent<Image>();
            linhaImg.sprite = sprLinha;
            linhaImg.color = Color.white;
            linhaImg.raycastTarget = false;
            Posicionar((RectTransform)linhaGo.transform, new Vector2(0f, y - 18f), new Vector2(110f, 3f));
        }

        return titulo;
    }

    private RectTransform IndicadorPausa(RectTransform pai)
    {
        GameObject go = new GameObject("IndicadorSelecao", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(pai, false);
        Image img = go.GetComponent<Image>();
        Sprite spr = UIFactory.LoadSprite("PainelPausa_Selecao");
        if (spr != null)
        {
            img.sprite = spr;
            img.type = Image.Type.Simple;
            img.color = Color.white;
        }
        else
        {
            img.color = new Color(0.88f, 0.78f, 0.62f, 0.9f);
        }
        img.raycastTarget = false;
        RectTransform rt = go.GetComponent<RectTransform>();
        Posicionar(rt, new Vector2(0f, 44f), new Vector2(196f, 30f));
        return rt;
    }

    private void SelecionavelPausa(Selectable s, Graphic alvo, RectTransform indicador)
    {
        s.targetGraphic = alvo;
        ColorBlock cores = s.colors;
        cores.normalColor = new Color(0.86f, 0.84f, 0.78f, 1f);
        cores.highlightedColor = new Color(0.12f, 0.10f, 0.08f, 1f);
        cores.selectedColor = new Color(0.12f, 0.10f, 0.08f, 1f);
        cores.pressedColor = new Color(0.25f, 0.18f, 0.14f, 1f);
        cores.fadeDuration = 0.05f;
        s.colors = cores;

        s.gameObject.AddComponent<MenuItemSelector>().Vincular(indicador, fonteSom, somSelecao);
    }

    private Button BotaoPausa(RectTransform pai, string texto, float y, RectTransform indicador)
    {
        GameObject go = new GameObject(texto, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(pai, false);
        Posicionar((RectTransform)go.transform, new Vector2(0f, y), new Vector2(196f, 30f));
        go.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);

        // Alinhado à esquerda com recuo de 37f para posicionar o texto ao lado da seta do papiro.
        // Assim 'Continuar', 'Configurações', 'Menu Principal' e 'Sair' formam uma coluna uniforme e fiel ao protótipo.
        TMP_Text rotulo = Rotulo(go.transform, texto, 20f, new Vector2(14f, 0f),
                                 new Vector2(150f, 26f), TextAlignmentOptions.MidlineLeft);

        Button botao = go.GetComponent<Button>();
        SelecionavelPausa(botao, rotulo, indicador);
        return botao;
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
        TMP_Text titulo = Rotulo(pai, texto, 24f, new Vector2(0f, y),
                                 new Vector2(largura - 30f, 32f), TextAlignmentOptions.Center);
        titulo.color = new Color(0.88f, 0.85f, 0.78f, 1f);

        Sprite sprLinha = UIFactory.LoadSprite("PainelPausa_Linha");
        if (sprLinha != null)
        {
            GameObject linhaGo = new GameObject("LinhaVermelha", typeof(RectTransform), typeof(Image));
            linhaGo.transform.SetParent(pai, false);
            Image linhaImg = linhaGo.GetComponent<Image>();
            linhaImg.sprite = sprLinha;
            linhaImg.color = Color.white;
            linhaImg.raycastTarget = false;
            Posicionar((RectTransform)linhaGo.transform, new Vector2(0f, y - 18f), new Vector2(110f, 3f));
        }

        return titulo;
    }

    private RectTransform Indicador(RectTransform pai, float x)
    {
        TMP_Text seta = Rotulo(pai, ">", 20f, new Vector2(x, 0f), new Vector2(18f, 24f),
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
        Posicionar((RectTransform)go.transform, new Vector2(0f, y), new Vector2(160f, 24f));
        go.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);   // só área de clique

        TMP_Text rotulo = Rotulo(go.transform, texto, 16f, Vector2.zero,
                                 new Vector2(160f, 24f), TextAlignmentOptions.Center);

        Button botao = go.GetComponent<Button>();
        Selecionavel(botao, rotulo, indicador);
        return botao;
    }

    private Slider LinhaSlider(RectTransform pai, string texto, float y, float min, float max,
                               bool inteiros, RectTransform indicador, out TMP_Text valor)
    {
        Rotulo(pai, texto, 13f, new Vector2(-36f, y), new Vector2(88f, 20f),
               TextAlignmentOptions.MidlineLeft);

        GameObject go = DefaultControls.CreateSlider(new DefaultControls.Resources());
        go.transform.SetParent(pai, false);
        Posicionar((RectTransform)go.transform, new Vector2(36f, y), new Vector2(52f, 6f));

        // Trilho com contraste nítido (visível de 0% a 100%)
        Image trilho = go.transform.Find("Background").GetComponent<Image>();
        trilho.color = new Color(0.22f, 0.20f, 0.18f, 1f);

        Image preenchimento = go.transform.Find("Fill Area/Fill").GetComponent<Image>();
        preenchimento.color = new Color(0.86f, 0.66f, 0.34f, 1f);

        Image alca = go.transform.Find("Handle Slide Area/Handle").GetComponent<Image>();
        alca.rectTransform.sizeDelta = new Vector2(5f, 10f);
        alca.color = new Color(0.88f, 0.85f, 0.78f, 1f);

        Slider slider = go.GetComponent<Slider>();
        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = inteiros;
        Selecionavel(slider, alca, indicador);

        valor = Rotulo(pai, "", 12f, new Vector2(76f, y), new Vector2(24f, 20f),
                       TextAlignmentOptions.MidlineRight);
        return slider;
    }

    private Toggle LinhaToggle(RectTransform pai, string texto, float y, RectTransform indicador)
    {
        Rotulo(pai, texto, 13f, new Vector2(-36f, y), new Vector2(88f, 20f),
               TextAlignmentOptions.MidlineLeft);

        GameObject go = DefaultControls.CreateToggle(new DefaultControls.Resources());
        go.transform.SetParent(pai, false);
        Posicionar((RectTransform)go.transform, new Vector2(45f, y), new Vector2(16f, 16f));

        // o rótulo padrão é Text legado; o texto da linha já está ao lado
        Destroy(go.transform.Find("Label").gameObject);

        // Moldura externa da caixa (bronze/ferro escovado com bom contraste)
        Image moldura = go.transform.Find("Background").GetComponent<Image>();
        moldura.color = new Color(0.42f, 0.38f, 0.32f, 1f);
        UIFactory.Stretch(moldura.rectTransform);

        // Fundo escavado interno (cavidade escura com 1px de respiro)
        Image cavidade = Imagem(moldura.transform, "Cavidade", new Color(0.08f, 0.07f, 0.06f, 1f));
        Posicionar(cavidade.rectTransform, Vector2.zero, new Vector2(14f, 14f));
        cavidade.raycastTarget = false;

        // Indicador de marcação centralizado proporcional
        Image marca = go.transform.Find("Background/Checkmark").GetComponent<Image>();
        marca.color = corDestaque;
        marca.transform.SetParent(cavidade.transform, false);
        Posicionar(marca.rectTransform, Vector2.zero, new Vector2(8f, 8f));

        Toggle toggle = go.GetComponent<Toggle>();
        Selecionavel(toggle, moldura, indicador);
        return toggle;
    }

    private TMP_Dropdown LinhaStepperResolucao(RectTransform pai, string texto, float y,
                                               RectTransform indicador)
    {
        Rotulo(pai, texto, 13f, new Vector2(-36f, y), new Vector2(88f, 20f),
               TextAlignmentOptions.MidlineLeft);

        // Container do seletor horizontal (stepper)
        GameObject container = new GameObject("StepperResolucao", typeof(RectTransform));
        container.transform.SetParent(pai, false);
        Posicionar((RectTransform)container.transform, new Vector2(48f, y), new Vector2(86f, 18f));

        // Cria dropdown base (invisível na tela, apenas gerencia opções e eventos com VideoSettingsController)
        GameObject goDropdown = TMP_DefaultControls.CreateDropdown(new TMP_DefaultControls.Resources());
        goDropdown.transform.SetParent(container.transform, false);
        Posicionar((RectTransform)goDropdown.transform, Vector2.zero, new Vector2(86f, 18f));
        goDropdown.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
        Transform arrow = goDropdown.transform.Find("Arrow");
        if (arrow != null) arrow.gameObject.SetActive(false);
        Transform label = goDropdown.transform.Find("Label");
        if (label != null) label.gameObject.SetActive(false);
        TMP_Dropdown dropdown = goDropdown.GetComponent<TMP_Dropdown>();
        dropdown.template.gameObject.SetActive(false);

        // Moldura do stepper
        Image moldura = Imagem(container.transform, "Moldura", new Color(0.35f, 0.30f, 0.25f, 0.8f));
        Posicionar(moldura.rectTransform, Vector2.zero, new Vector2(86f, 18f));
        moldura.raycastTarget = false;

        // Fundo interno escavado
        Image fundoInterno = Imagem(moldura.transform, "Fundo", new Color(0.08f, 0.07f, 0.06f, 0.95f));
        Posicionar(fundoInterno.rectTransform, Vector2.zero, new Vector2(84f, 16f));
        fundoInterno.raycastTarget = false;

        // Texto central da resolução
        TMP_Text textoResolucao = Rotulo(container.transform, "", 11f, Vector2.zero,
                                         new Vector2(56f, 16f), TextAlignmentOptions.Center);
        textoResolucao.color = corTexto;

        HorizontalStepperNavigation nav = goDropdown.AddComponent<HorizontalStepperNavigation>();
        nav.Configurar(dropdown, textoResolucao, fonteSom, somSelecao);

        // Botões laterais '<' e '>' para clique com mouse
        BotaoSeta(container.transform, "<", new Vector2(-34f, 0f), nav.Retroceder);
        BotaoSeta(container.transform, ">", new Vector2(34f, 0f), nav.Avancar);

        Selecionavel(dropdown, moldura, indicador);
        return dropdown;
    }

    private Button BotaoSeta(Transform pai, string seta, Vector2 pos, System.Action acao)
    {
        GameObject go = new GameObject("Btn_" + seta, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(pai, false);
        Posicionar((RectTransform)go.transform, pos, new Vector2(16f, 16f));
        go.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);

        TMP_Text rotulo = Rotulo(go.transform, seta, 13f, Vector2.zero, new Vector2(16f, 16f),
                                 TextAlignmentOptions.Center);
        rotulo.color = corTexto;

        Button btn = go.GetComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = corDestaque;
        cb.pressedColor = corDestaque * 0.8f;
        btn.colors = cb;
        btn.targetGraphic = rotulo;
        if (acao != null)
        {
            btn.onClick.AddListener(new UnityAction(acao));
        }
        return btn;
    }
}

public class HorizontalStepperNavigation : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private TMP_Dropdown dropdown;
    private TMP_Text texto;
    private AudioSource audioSource;
    private AudioClip somSelecao;
    private bool selecionado;

    public void Configurar(TMP_Dropdown d, TMP_Text t, AudioSource fonte, AudioClip som)
    {
        dropdown = d;
        texto = t;
        audioSource = fonte;
        somSelecao = som;
    }

    public void OnSelect(BaseEventData eventData)
    {
        selecionado = true;
        if (texto != null) texto.color = new Color(0.95f, 0.72f, 0.36f, 1f);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selecionado = false;
        if (texto != null) texto.color = new Color(0.88f, 0.86f, 0.80f, 1f);
    }

    private void Update()
    {
        if (dropdown == null || texto == null) return;

        if (dropdown.options != null && dropdown.options.Count > 0 &&
            dropdown.value >= 0 && dropdown.value < dropdown.options.Count)
        {
            string nomeOpcao = dropdown.options[dropdown.value].text;
            if (texto.text != nomeOpcao)
            {
                texto.text = nomeOpcao;
            }
        }

        if (!selecionado || Keyboard.current == null) return;

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame)
        {
            Retroceder();
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
        {
            Avancar();
        }
    }

    public void Retroceder()
    {
        if (dropdown != null && dropdown.value > 0)
        {
            dropdown.value--;
            TocarSom();
        }
    }

    public void Avancar()
    {
        if (dropdown != null && dropdown.options != null && dropdown.value < dropdown.options.Count - 1)
        {
            dropdown.value++;
            TocarSom();
        }
    }

    private void TocarSom()
    {
        if (audioSource != null && somSelecao != null)
        {
            audioSource.PlayOneShot(somSelecao);
        }
    }
}

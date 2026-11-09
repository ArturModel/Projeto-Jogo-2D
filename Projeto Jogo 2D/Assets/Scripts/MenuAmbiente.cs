using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

// Apresentação exclusiva do menu. Os painéis, callbacks e saves continuam nos
// controladores existentes; selecionar um botão nunca executa sua ação.
public sealed class MenuAmbiente : MonoBehaviour
{
    public string FocoAtual { get; private set; } = "Rua";
    public bool Pronto { get; private set; }
    public bool Saindo { get; private set; }

    RawImage fundo, telefone;
    RectTransform tela, quadro, telefoneRect;
    CanvasGroup principal, opcoes, identidade;
    TMP_Text legenda;
    MenuAmbienteGraphic atmosfera, vinheta;
    readonly Dictionary<Button, Image> placas = new Dictionary<Button, Image>();
    readonly Dictionary<Button, float> enfases = new Dictionary<Button, float>();
    readonly Dictionary<string, Transform> objetos = new Dictionary<string, Transform>();
    readonly List<Button> botoesPrincipais = new List<Button>();
    Vector2 ponto = new Vector2(.34f, .46f), mouseSuave;
    Vector2 tamanho;
    float zoom = 1.025f, luzTelefone, luzClube, saida, tempo, pulsoTelefone;
    bool temSave, focoJanela = true;
    bool menuMinigames;
    GameObject painelMinigames;
    string ultimaLegenda;

    void Start()
    {
        // Procurar só nesta cena: nenhuma alteração em pausa, prólogo ou gameplay.
        foreach (var root in gameObject.scene.GetRootGameObjects())
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
                if (!objetos.ContainsKey(t.name)) objetos.Add(t.name, t);
        fundo = Obter<RawImage>("VideoBackground");
        principal = Obter<CanvasGroup>("MainMenuPanel");
        opcoes = Obter<CanvasGroup>("OptionsPanel");
        var rua = Resources.Load<Texture2D>("MenuAmbiente/Rua");
        var orelhao = Resources.Load<Texture2D>("MenuAmbiente/Orelhao");
        if (fundo == null || principal == null || rua == null || orelhao == null)
        {
            Debug.LogError("MenuAmbiente: fundo, orelhão ou painel ausente; mantendo apresentação original.");
            var original = Obter<VideoPlayer>("VideoPlayer");
            if (original != null) { original.enabled = true; original.Play(); }
            enabled = false;
            return;
        }
        foreach (var vp in FindObjectsByType<VideoPlayer>(FindObjectsInactive.Include))
            if (vp.gameObject.scene == gameObject.scene) { vp.Stop(); vp.enabled = false; }
        // MenuChuva é uma fonte de áudio independente e permanece intacta.
        fundo.texture = rua;
        fundo.raycastTarget = false;
        quadro = fundo.rectTransform;
        tela = quadro.parent as RectTransform;
        quadro.anchorMin = quadro.anchorMax = new Vector2(.5f, .5f);
        quadro.pivot = new Vector2(.5f, .5f);

        atmosfera = NovoGrafico("ChuvaReflexosLuzes", quadro, MenuAmbienteGraphic.Camada.Ambiente);
        telefone = NovaImagem("OrelhaoPrimeiroPlano", quadro, orelhao);
        telefoneRect = telefone.rectTransform;
        telefoneRect.anchorMin = telefoneRect.anchorMax = new Vector2(.9f, .28f);
        telefoneRect.pivot = new Vector2(.5f, .5f);
        vinheta = NovoGrafico("SombraDeLeitura", tela, MenuAmbienteGraphic.Camada.Leitura);
        vinheta.transform.SetSiblingIndex(quadro.GetSiblingIndex() + 1);
        // A composição substitui o escurecimento uniforme antigo, sem ocultar o cenário.
        var overlay = Obter<Image>("FocusOverlay");
        if (overlay != null) overlay.enabled = false;

        foreach (var t in objetos.Values)
        {
            var b = t.GetComponent<Button>();
            if (b == null) continue;
            var cores = b.colors;
            cores.normalColor = new Color(.82f, .85f, .81f);
            cores.highlightedColor = cores.selectedColor = new Color(1f, .84f, .56f);
            cores.pressedColor = Color.white;
            cores.disabledColor = new Color(.38f, .43f, .43f);
            cores.fadeDuration = .15f;
            b.colors = cores;
            if (t.parent != principal.transform) continue;
            ConfigurarBotao(b, IndicePrincipal(t.name));
            botoesPrincipais.Add(b);
            CriarPlacaFoco(b);
        }
        var botaoMinigames = CriarBotao("btn_minigames", "MINIGAMES", principal.transform, 3,
            AbrirMenuMinigames);
        botoesPrincipais.Add(botaoMinigames);
        CriarPlacaFoco(botaoMinigames);
        CriarPainelMinigames(botaoMinigames.GetComponentInChildren<TMP_Text>().font);
        if (objetos.TryGetValue("OptionSelector", out var seletor)) seletor.gameObject.SetActive(false);
        foreach (var t in principal.GetComponentsInChildren<TMP_Text>(true))
            if (t.text.Trim() == ">") t.gameObject.SetActive(false);
        var font = Obter<Button>("btn_novojogo").GetComponentInChildren<TMP_Text>().font;
        var raiz = new GameObject("IdentidadeMenu", typeof(RectTransform), typeof(CanvasGroup));
        raiz.transform.SetParent(principal.transform, false);
        Esticar(raiz.GetComponent<RectTransform>());
        identidade = raiz.GetComponent<CanvasGroup>();
        identidade.blocksRaycasts = false;
        var filete = NovaImagem("FileteIdentidade", raiz.transform, null);
        filete.color = new Color(.88f, .65f, .36f, .68f);
        filete.rectTransform.anchorMin = filete.rectTransform.anchorMax = new Vector2(.078f, .602f);
        filete.rectTransform.pivot = new Vector2(0, .5f);
        filete.rectTransform.sizeDelta = new Vector2(104, 1.5f);
        Texto("SantaRita", raiz.transform, font, "SANTA RITA", 60, new Vector2(.075f, .75f), new Vector2(440, 76), new Color(.94f, .9f, .76f));
        Texto("Subtitulo", raiz.transform, font, "A NOITE DAS 03:17", 22, new Vector2(.078f, .665f), new Vector2(410, 36), new Color(.72f, .69f, .57f));
        legenda = Texto("Contexto", raiz.transform, font, "", 19, new Vector2(.078f, .105f), new Vector2(465, 45), new Color(.65f, .7f, .69f));
        temSave = CheckpointSystem.HasCheckpoint;
        Pronto = true;
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(Obter<Button>("btn_novojogo").gameObject);
    }

    T Obter<T>(string nome) where T : Component
    { return objetos.TryGetValue(nome, out var t) ? t.GetComponent<T>() : null; }

    static int IndicePrincipal(string nome)
    { return nome == "btn_novojogo" ? 0 : nome == "btn_continuar" ? 1 : nome == "btn_opcoes" ? 2 : nome == "btn_minigames" ? 3 : 4; }

    Button CriarBotao(string nome, string titulo, Transform pai, int indice, UnityEngine.Events.UnityAction acao)
    {
        var go = new GameObject(nome, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(pai, false);
        objetos[nome] = go.transform;
        var imagem = go.GetComponent<Image>(); imagem.color = new Color(.12f, .15f, .14f, .96f);
        var botao = go.GetComponent<Button>(); botao.targetGraphic = imagem;
        var cores = botao.colors; cores.normalColor = new Color(.82f, .85f, .81f);
        cores.highlightedColor = cores.selectedColor = new Color(1f, .84f, .56f);
        cores.pressedColor = Color.white; cores.fadeDuration = .15f; botao.colors = cores;
        var label = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        label.transform.SetParent(go.transform, false);
        var texto = label.GetComponent<TextMeshProUGUI>();
        texto.font = Obter<Button>("btn_novojogo").GetComponentInChildren<TMP_Text>().font;
        texto.text = titulo; texto.fontSize = 25; texto.color = new Color(.94f, .9f, .76f);
        texto.alignment = TextAlignmentOptions.MidlineLeft; texto.raycastTarget = false;
        Esticar(texto.rectTransform); texto.margin = Vector4.zero;
        ConfigurarBotao(botao, indice);
        texto.fontSize = 25;
        botao.onClick.AddListener(acao);
        return botao;
    }

    void CriarPainelMinigames(TMP_FontAsset font)
    {
        painelMinigames = new GameObject("MinigamesPanel", typeof(RectTransform), typeof(Image));
        painelMinigames.transform.SetParent(principal.transform, false);
        Esticar(painelMinigames.GetComponent<RectTransform>());
        var fundoPainel = painelMinigames.GetComponent<Image>();
        fundoPainel.color = new Color(.035f, .055f, .052f, .96f);
        var titulo = Texto("TituloMinigames", painelMinigames.transform, font,
            "JOGUE UMA PARTIDA", 30, new Vector2(.078f, .58f), new Vector2(400, 46), new Color(.94f, .9f, .76f));
        titulo.alignment = TextAlignmentOptions.MidlineLeft;
        var bocha = CriarBotao("btn_minigame_bocha", "BOCHA · PRÁTICA", painelMinigames.transform, 0,
            () => BochaMinigameUI.Instance.IniciarPraticaOpcional());
        ConfigurarBotaoPainel(bocha, .46f);
        var sinuca = CriarBotao("btn_minigame_sinuca", "SINUCA · PRÁTICA", painelMinigames.transform, 0,
            () => SinucaMinigameUI.Instance.IniciarPraticaOpcional());
        ConfigurarBotaoPainel(sinuca, .37f);
        var voltar = CriarBotao("btn_voltar_minigames", "VOLTAR", painelMinigames.transform, 0,
            FecharMenuMinigames);
        ConfigurarBotaoPainel(voltar, .28f);
        painelMinigames.SetActive(false);
    }

    static void ConfigurarBotaoPainel(Button botao, float y)
    {
        var r = botao.GetComponent<RectTransform>();
        r.anchorMin = r.anchorMax = new Vector2(.078f, y); r.pivot = new Vector2(0, .5f);
        r.anchoredPosition = Vector2.zero; r.sizeDelta = new Vector2(390, 44);
        var label = botao.GetComponentInChildren<TMP_Text>(); label.fontSize = 25;
    }

    void AbrirMenuMinigames()
    {
        menuMinigames = true;
        foreach (var botao in botoesPrincipais) botao.gameObject.SetActive(false);
        painelMinigames.SetActive(true);
        EventSystem.current?.SetSelectedGameObject(Obter<Button>("btn_minigame_bocha").gameObject);
    }

    void FecharMenuMinigames()
    {
        menuMinigames = false;
        painelMinigames.SetActive(false);
        foreach (var botao in botoesPrincipais) botao.gameObject.SetActive(true);
    }

    static void ConfigurarBotao(Button b, int i)
    {
        var r = b.GetComponent<RectTransform>();
        r.anchorMin = r.anchorMax = new Vector2(.078f, .50f - i * .077f);
        r.pivot = new Vector2(0, .5f);
        r.anchoredPosition = Vector2.zero;
        r.sizeDelta = new Vector2(246, 41);
        var label = b.GetComponentInChildren<TMP_Text>();
        label.fontSize = 27;
        label.alignment = TextAlignmentOptions.MidlineLeft;
        label.enableAutoSizing = false;
        label.text = label.text.Trim();
        Esticar(label.rectTransform);
        label.margin = new Vector4(12, 0, 6, 0);
        label.raycastTarget = false;
    }

    void Update()
    {
        if (!Pronto) return;
        float dt = Mathf.Min(Time.unscaledDeltaTime, .05f);
        if (focoJanela) tempo += dt;
        var selecionado = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        bool modal = PayphoneCheckpoint.IsSaveUIOpen;
        bool configurando = opcoes != null && opcoes.gameObject.activeInHierarchy;
        string nome = selecionado != null ? selecionado.name : "";
        if (menuMinigames && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            FecharMenuMinigames();
        string foco = modal ? "Orelhao" : FocoDoBotao(nome, configurando);
        if (!modal && configurando)
        {
            if (objetos["AudioPanel"].gameObject.activeInHierarchy) foco = "Poste";
            else if (objetos["VIdeoPanel"].gameObject.activeInHierarchy) foco = "Reflexos";
            else if (objetos["LegendasPanel"].gameObject.activeInHierarchy) foco = "Clube";
        }
        if (!Saindo) FocoAtual = foco;
        Vector2 alvo = PontoDoFoco(FocoAtual);
        float suavizar = 1 - Mathf.Exp(-dt * 4.2f);
        ponto = Vector2.Lerp(ponto, alvo, suavizar);
        float zoomAlvo = Saindo ? 1.15f : modal ? 1.075f : configurando ? 1.055f : FocoAtual == "Orelhao" ? 1.065f : 1.025f;
        zoom = Mathf.Lerp(zoom, zoomAlvo, suavizar);
        Vector2 mouse = Vector2.zero;
        if (focoJanela && !modal && !Saindo && Mouse.current != null)
        {
            var p = Mouse.current.position.ReadValue();
            mouse = new Vector2(Mathf.Clamp01(p.x / Mathf.Max(1, Screen.width)) - .5f,
                                Mathf.Clamp01(p.y / Mathf.Max(1, Screen.height)) - .5f);
        }
        mouseSuave = Vector2.Lerp(mouseSuave, mouse, suavizar);
        float proporcao = (float)fundo.texture.width / fundo.texture.height;
        float largura = Mathf.Max(tela.rect.width, tela.rect.height * proporcao);
        tamanho = new Vector2(largura, largura / proporcao);
        quadro.sizeDelta = tamanho;
        quadro.localScale = Vector3.one * zoom;
        Vector2 desvio = Vector2.Scale(new Vector2(.5f, .5f) - ponto, tamanho) * (zoom - 1f);
        desvio += mouseSuave * 4f;
        quadro.anchoredPosition = new Vector2(Mathf.Round(desvio.x), Mathf.Round(desvio.y));
        float larguraTelefone = Mathf.Min(tamanho.x * .23f, tela.rect.width * .27f);
        telefoneRect.sizeDelta = new Vector2(larguraTelefone, larguraTelefone * 1.5f);
        float areaVisivel = Mathf.Min(1, tela.rect.width / (tamanho.x * zoom));
        telefoneRect.anchorMin = telefoneRect.anchorMax = new Vector2(.5f + .40f * areaVisivel, .28f);
        telefoneRect.anchoredPosition = new Vector2(Mathf.Round(mouseSuave.x * 8), Mathf.Round(mouseSuave.y * 3));
        var ponteiro = PonteiroNoQuadro();
        bool sobreTelefone = ponteiro.x > .81f && ponteiro.x < .99f && ponteiro.y > .08f && ponteiro.y < .61f;
        float telefoneAlvo = FocoAtual == "Orelhao" || (!modal && !configurando && sobreTelefone) ? 1f : 0f;
        luzTelefone = Mathf.Lerp(luzTelefone, telefoneAlvo, suavizar);
        luzClube = Mathf.Lerp(luzClube, configurando || FocoAtual == "Clube" ? 1f : 0f, suavizar);
        pulsoTelefone = Mathf.Max(0, pulsoTelefone - dt);
        float brilho = fundo.color.r;
        float tom = Mathf.Lerp(.69f, .97f, luzTelefone) * brilho * (1 - saida * .8f);
        telefone.color = new Color(tom * .91f, tom * .98f, tom, 1);
        atmosfera.Tempo = tempo;
        atmosfera.LuzClube = luzClube;
        atmosfera.LuzTelefone = Mathf.Max(luzTelefone, pulsoTelefone);
        atmosfera.Brilho = brilho;
        atmosfera.Foco = ponto;
        vinheta.LuzClube = configurando ? 1 : 0;
        vinheta.Saida = saida;
        vinheta.SetVerticesDirty();
        if (focoJanela) atmosfera.SetVerticesDirty();
        AtualizarBotoes(selecionado, dt);
        string texto = modal ? "Escolha um registro do orelhão." : menuMinigames
            ? "Escolha uma partida rápida de bocha ou sinuca."
            : LegendaDoFoco(FocoAtual);
        if (ultimaLegenda != texto) { legenda.text = texto; ultimaLegenda = texto; }
        if (!modal && !configurando && focoJanela && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            InteragirCenario();
    }

    Vector2 PonteiroNoQuadro()
    {
        if (Mouse.current == null || !focoJanela) return new Vector2(-10, -10);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(quadro, Mouse.current.position.ReadValue(), null, out var p);
        return new Vector2(p.x / tamanho.x + .5f, p.y / tamanho.y + .5f);
    }

    void InteragirCenario()
    {
        // Não atravessar botões, controles ou o modal com um clique decorativo.
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        var p = PonteiroNoQuadro();
        if (p.x > .81f && p.x < .99f && p.y > .08f && p.y < .61f)
        {
            pulsoTelefone = 1.2f; atmosfera.Responder(p, tempo);
            EventSystem.current?.SetSelectedGameObject(Obter<Button>("btn_continuar").gameObject);
            // Atalho diegético para a mesma seleção explícita de slots.
            // Nunca salva, escolhe slot ou carrega uma partida por aproximação.
            GetComponent<MainMenuActions>().Continuar();
        }
        else if (p.y < .29f) atmosfera.Responder(p, tempo);
    }

    void CriarPlacaFoco(Button button)
    {
        var plate = new GameObject("PlacaDeSelecao", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        plate.transform.SetParent(button.transform, false);
        plate.transform.SetAsFirstSibling();
        Esticar(plate.rectTransform);
        UIFactory.ApplyWeatheredSurface(plate, 0);
        plate.raycastTarget = false;
        placas.Add(button, plate);
        enfases.Add(button, 0);
    }

    void AtualizarBotoes(GameObject selecionado, float dt)
    {
        foreach (var item in placas)
        {
            float target = selecionado == item.Key.gameObject ? 1f : 0f;
            float emphasis = Mathf.MoveTowards(enfases[item.Key], target, dt * 7f);
            enfases[item.Key] = emphasis;
            item.Value.color = new Color(1.22f, 1.12f, .86f, emphasis * .84f);
        }
    }

    static string FocoDoBotao(string nome, bool configurando)
    {
        if (nome == "btn_continuar") return "Orelhao";
        if (nome == "btn_sair") return "Despedida";
        if (nome == "btn_minigames" || nome.StartsWith("btn_minigame_")) return "Clube";
        if (nome == "btn_audio" || nome.Contains("volume")) return "Poste";
        if (nome == "btn_video" || nome.Contains("resolucao") || nome.Contains("brilho")) return "Reflexos";
        if (nome == "btn_legendas" || nome.Contains("Legenda")) return "Clube";
        if (nome == "btn_opcoes" || configurando) return "Clube";
        return "Rua";
    }

    static Vector2 PontoDoFoco(string foco)
    {
        switch (foco)
        {
            case "Orelhao": return new Vector2(.86f, .44f);
            case "Clube": return new Vector2(.78f, .48f);
            case "Poste": return new Vector2(.55f, .73f);
            case "Reflexos": return new Vector2(.56f, .2f);
            case "Despedida": return new Vector2(.26f, .5f);
            default: return new Vector2(.22f, .42f);
        }
    }

    string LegendaDoFoco(string foco)
    {
        switch (foco)
        {
            case "Orelhao": return temSave ? "Retomar de um orelhão registrado." : "Nenhum registro no orelhão. Comece uma nova viagem.";
            case "Clube": return "Ajuste a experiência antes de seguir.";
            case "Despedida": return "Deixar Santa Rita por enquanto.";
            default: return "Uma estrada de volta. Uma noite por resolver.";
        }
    }

    public bool PrepararSaida(Action aoConcluir, bool novoJogo)
    {
        if (!Pronto) return false;
        if (!Saindo) StartCoroutine(Sair(aoConcluir, novoJogo));
        return true;
    }

    IEnumerator Sair(Action aoConcluir, bool novoJogo)
    {
        Saindo = true;
        FocoAtual = novoJogo ? "Rua" : "Despedida";
        principal.interactable = principal.blocksRaycasts = false;
        float t = 0;
        while (t < .55f)
        {
            t += Time.unscaledDeltaTime;
            saida = Mathf.SmoothStep(0, 1, Mathf.Clamp01(t / .55f));
            principal.alpha = 1 - saida;
            yield return null;
        }
        aoConcluir();
    }

    void OnApplicationFocus(bool foco) { focoJanela = foco; }

    static MenuAmbienteGraphic NovoGrafico(string nome, Transform pai, MenuAmbienteGraphic.Camada camada)
    {
        var go = new GameObject(nome, typeof(RectTransform), typeof(CanvasRenderer));
        go.transform.SetParent(pai, false);
        var g = go.AddComponent<MenuAmbienteGraphic>();
        g.Tipo = camada; g.raycastTarget = false;
        Esticar(g.rectTransform);
        return g;
    }

    static RawImage NovaImagem(string nome, Transform pai, Texture tex)
    {
        var go = new GameObject(nome, typeof(RectTransform), typeof(RawImage));
        go.transform.SetParent(pai, false);
        var r = go.GetComponent<RawImage>(); r.texture = tex; r.raycastTarget = false;
        return r;
    }

    static TMP_Text Texto(string nome, Transform pai, TMP_FontAsset font, string texto, float size, Vector2 anchor, Vector2 dimensao, Color cor)
    {
        var go = new GameObject(nome, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(pai, false);
        var t = go.GetComponent<TextMeshProUGUI>();
        t.font = font; t.text = texto; t.fontSize = size; t.color = cor;
        t.alignment = TextAlignmentOptions.MidlineLeft; t.raycastTarget = false;
        t.rectTransform.anchorMin = t.rectTransform.anchorMax = anchor;
        t.rectTransform.pivot = new Vector2(0, .5f);
        t.rectTransform.anchoredPosition = Vector2.zero; t.rectTransform.sizeDelta = dimensao;
        return t;
    }

    static void Esticar(RectTransform r)
    { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = r.offsetMax = Vector2.zero; }
}

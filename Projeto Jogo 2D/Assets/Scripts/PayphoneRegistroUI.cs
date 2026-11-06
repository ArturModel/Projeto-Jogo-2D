using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Apresentação do mesmo seletor de slots: nenhuma gravação ou carga aqui.
// Os botões físicos 1–5 chamam o fluxo existente somente após confirmação.
public sealed class PayphoneRegistroUI : MonoBehaviour
{
    public bool Ocupado { get; private set; }
    public bool SomenteCarregar { get; private set; }
    public int RegistroSelecionado { get; private set; }
    public const float TempoAproximacao = .22f;

    RectTransform painel;
    CanvasGroup grupo;
    GameObject modal, anterior;
    Image escuro, led;
    TMP_Text modo, visor, instrucao, aviso;
    readonly Button[] teclas = new Button[5];
    readonly TMP_Text[] estados = new TMP_Text[5];
    readonly bool[] disponiveis = new bool[5];
    Button desligar, confirmar;
    Action<int> aoEscolher;
    Action aoCancelar;
    bool esperandoCarga;

    public static bool TemArte => Resources.Load<Texture2D>("MenuAmbiente/OrelhaoFrente") != null;

    public void Construir(Transform canvas, GameObject raiz, Action<int> escolher, Action cancelar,
                         Button[] botoesExistentes, TextMeshProUGUI[] textosExistentes)
    {
        modal = raiz; aoEscolher = escolher; aoCancelar = cancelar;
        var scaler = canvas.GetComponent<CanvasScaler>();
        scaler.referenceResolution = new Vector2(960, 540);
        scaler.matchWidthOrHeight = .5f;
        escuro = raiz.GetComponent<Image>();
        escuro.color = new Color(.003f, .009f, .017f, 0);
        painel = Rect("AparelhoFrontal", raiz.transform, Vector2.zero, new Vector2(960, 540));
        grupo = painel.gameObject.AddComponent<CanvasGroup>();
        var imagem = painel.gameObject.AddComponent<RawImage>();
        imagem.texture = Resources.Load<Texture2D>("MenuAmbiente/OrelhaoFrente");
        imagem.raycastTarget = false;

        modo = Texto("ModoOrelhao", painel, "", 21, new Vector2(-280, 120), new Vector2(200, 50), new Color(.86f, .8f, .64f));
        aviso = Texto("AvisoProgresso", painel, "", 12, new Vector2(-280, 72), new Vector2(200, 36), new Color(.93f, .72f, .4f));
        instrucao = Texto("InstrucoesOrelhao", painel, "", 14, new Vector2(-280, -5), new Vector2(190, 112), new Color(.64f, .71f, .69f));

        var lcd = Rect("VisorDoAparelho", painel, new Vector2(130.9f, 139.8f), new Vector2(194, 51));
        var vidro = lcd.gameObject.AddComponent<Image>(); vidro.color = new Color(.055f, .085f, .064f, .60f); vidro.raycastTarget = false;
        visor = Texto("DetalhesRegistro", lcd, "", 13.5f, Vector2.zero, new Vector2(184, 47), new Color(.67f, .83f, .47f));
        visor.overflowMode = TextOverflowModes.Ellipsis;
        visor.alignment = TextAlignmentOptions.MidlineLeft;
        led = Rect("LedDeLinha", painel, new Vector2(240, 140), new Vector2(4, 4)).gameObject.AddComponent<Image>();
        led.raycastTarget = false;

        string[] numeros = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "*", "0", "#" };
        for (int i = 0; i < 12; i++)
        {
            int coluna = i % 3, linha = i / 3;
            var r = Rect(i < 5 ? "BtnSlot_" + (i + 1) : i == 9 ? "BtnDesligarFone" : "Tecla_" + numeros[i], painel,
                         new Vector2(74.6f + coluna * 60f, 50.2f - linha * 53.6f), new Vector2(48, 42));
            var img = r.gameObject.AddComponent<Image>();
            img.color = new Color(.9f, .85f, .64f, .025f);
            var label = Texto("Numero", r, numeros[i], 23, new Vector2(0, 4), new Vector2(46, 28), new Color(.12f, .14f, .13f));
            label.alignment = TextAlignmentOptions.Center;
            if (i < 5)
            {
                int slot = i;
                var b = Botao(r, img);
                b.onClick.AddListener(() => Confirmar(slot));
                var foco = r.gameObject.AddComponent<PayphoneRegistroTecla>(); foco.Vincular(this, slot);
                teclas[i] = b; botoesExistentes[i] = b;
                estados[i] = Texto("Estado", r, "", 8, new Vector2(0, -12), new Vector2(46, 12), new Color(.18f, .23f, .18f));
                estados[i].alignment = TextAlignmentOptions.Center;
                textosExistentes[i] = label as TextMeshProUGUI;
            }
            else if (i == 9)
            {
                desligar = Botao(r, img);
                desligar.onClick.AddListener(Cancelar);
            }
            else if (i == 11)
            {
                confirmar = Botao(r, img);
                confirmar.onClick.AddListener(() => Confirmar(RegistroSelecionado));
            }
            else { img.raycastTarget = false; label.color = new Color(.16f, .17f, .15f, .6f); }
        }
        var etiqueta = Texto("LegendaTeclado", painel, "[1–5] REGISTROS   [* / ESC] DESLIGAR", 12,
            new Vector2(130, -211), new Vector2(340, 24), new Color(.69f, .7f, .62f));
        etiqueta.alignment = TextAlignmentOptions.Center;
        var fechar = Botao(Rect("DesligarTexto", painel, new Vector2(-280, -115), new Vector2(190, 34)), null);
        var txt = Texto("DesligarLabel", fechar.transform, "DESLIGAR [ESC]", 16, Vector2.zero, new Vector2(190, 34), new Color(.76f, .7f, .55f));
        txt.alignment = TextAlignmentOptions.Center;
        fechar.onClick.AddListener(Cancelar);
    }

    public void Abrir(bool apenasCarregar)
    {
        StopAllCoroutines();
        esperandoCarga = false;
        grupo.interactable = grupo.blocksRaycasts = true;
        anterior = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        SomenteCarregar = apenasCarregar; Ocupado = false;
        modal.SetActive(true);
        modo.text = apenasCarregar ? "CONTINUAR\nNO ORELHÃO" : "GRAVAR\nNO ORELHÃO";
        aviso.text = apenasCarregar && GameState.TemProgressoNaoSalvo ? "Carregar substitui o progresso não salvo." : "";
        instrucao.text = apenasCarregar
            ? "Passe o mouse para consultar.\n\n1–5 conecta ao registro.\n\n# confirma o registro em foco."
            : "Passe o mouse para consultar.\n\n1–5 grava no registro.\n\n# confirma o registro em foco.";
        Atualizar();
        int primeiro = 0;
        if (apenasCarregar) for (int i = 0; i < 5; i++) if (disponiveis[i]) { primeiro = i; break; }
        Selecionar(primeiro);
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(teclas[primeiro].interactable ? teclas[primeiro].gameObject : desligar.gameObject);
        StartCoroutine(Aproximar());
    }

    public void Atualizar()
    {
        for (int i = 0; i < 5; i++)
        {
            disponiveis[i] = CheckpointSystem.TryLoadSlot(i, out _);
            teclas[i].interactable = !Ocupado && (!SomenteCarregar || disponiveis[i]);
            estados[i].text = disponiveis[i] ? "GRAVADO" : "VAZIO";
        }
        desligar.interactable = confirmar.interactable = !Ocupado;
    }

    public void Selecionar(int slot)
    {
        if (Ocupado || slot < 0 || slot >= 5) return;
        RegistroSelecionado = slot;
        for (int i = 0; i < estados.Length; i++)
            estados[i].fontStyle = i == slot ? FontStyles.Bold : FontStyles.Normal;
        if (CheckpointSystem.TryLoadSlot(slot, out var d))
        {
            string local = string.IsNullOrEmpty(d.localDescricao) ? CheckpointSystem.ObterNomeAmigavelOrelhao(d.payphoneId, d.sceneName) : d.localDescricao;
            visor.text = "REGISTRO " + (slot + 1).ToString("00") + " / " + (SomenteCarregar ? "CARREGAR" : "SOBRESCREVER") + "\n" + local + "\n<size=85%>" + d.salvoEm + "</size>";
            led.color = new Color(.6f, .78f, .35f);
        }
        else
        {
            visor.text = "REGISTRO " + (slot + 1).ToString("00") + "\n" + CheckpointSystem.DescreverSlotIndisponivel(slot) + "\n<size=85%>" + (SomenteCarregar ? "Sem ligação registrada." : "Disponível para gravar.") + "</size>";
            led.color = new Color(.37f, .39f, .22f);
        }
    }

    public void Confirmar(int slot)
    {
        if (Ocupado || !PayphoneCheckpoint.IsSaveUIOpen || slot < 0 || slot >= 5) return;
        Selecionar(slot);
        if (SomenteCarregar && !disponiveis[slot]) return;
        Ocupado = true; Atualizar();
        PayphoneCheckpoint.TocarTeclaRegistro(slot);
        StartCoroutine(Discando(slot));
    }

    IEnumerator Aproximar()
    {
        float t = 0;
        while (t < TempoAproximacao)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.SmoothStep(0, 1, Mathf.Clamp01(t / TempoAproximacao));
            Apresentar(p);
            yield return null;
        }
        Apresentar(1);
    }

    void Apresentar(float p)
    {
        var area = modal.GetComponent<RectTransform>().rect.size;
        float ajuste = Mathf.Min(area.x / 960, area.y / 540);
        painel.localScale = Vector3.one * ajuste * Mathf.Lerp(.86f, 1, p);
        painel.anchoredPosition = new Vector2(Mathf.Lerp(70, 0, p), Mathf.Lerp(-12, 0, p));
        grupo.alpha = p;
        escuro.color = new Color(.003f, .009f, .017f, p * .69f);
    }

    IEnumerator Discando(int slot)
    {
        var tecla = teclas[slot].GetComponent<RectTransform>();
        Vector2 baseTecla = tecla.anchoredPosition;
        tecla.anchoredPosition = baseTecla + Vector2.down * 2;
        visor.text = (SomenteCarregar ? "DISCANDO " : "GRAVANDO ") + (slot + 1).ToString("00") + "\n<size=90%>" + (SomenteCarregar ? "Reconectando ao registro..." : "Registrando esta passagem...") + "</size>";
        float t = 0;
        float duracao = SomenteCarregar ? .30f : .42f;
        while (t < duracao)
        {
            t += Time.unscaledDeltaTime;
            led.color = Mathf.FloorToInt(t * 18) % 2 == 0 ? new Color(.83f, .85f, .4f) : new Color(.25f, .34f, .2f);
            yield return null;
        }
        tecla.anchoredPosition = baseTecla;
        aoEscolher(slot);
    }

    public void Fechar()
    {
        StopAllCoroutines();
        Ocupado = true;
        StartCoroutine(Recuar());
    }

    // Na carga o aparelho permanece enquadrado. O SceneFader cobre esta imagem
    // diretamente e só a troca de cena remove o seletor, já sob preto total.
    public void AguardarCarga()
    {
        StopAllCoroutines();
        esperandoCarga = true; Ocupado = true;
        Apresentar(1);
        grupo.interactable = grupo.blocksRaycasts = false;
    }

    public void FalhaAoCarregar()
    {
        esperandoCarga = false; Ocupado = false;
        grupo.interactable = grupo.blocksRaycasts = true;
        Atualizar();
        visor.text = "REGISTRO INDISPONÍVEL\nEscolha outro registro.";
    }

    void OnEnable() { SceneManager.sceneLoaded += DepoisDaCarga; }
    void OnDisable() { SceneManager.sceneLoaded -= DepoisDaCarga; }

    void DepoisDaCarga(Scene scene, LoadSceneMode mode)
    {
        if (!esperandoCarga) return;
        esperandoCarga = false; Ocupado = false;
        modal.SetActive(false);
    }

    IEnumerator Recuar()
    {
        float t = 0;
        while (t < .14f)
        {
            t += Time.unscaledDeltaTime;
            Apresentar(1 - Mathf.SmoothStep(0, 1, Mathf.Clamp01(t / .14f)));
            yield return null;
        }
        modal.SetActive(false); Ocupado = false;
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(anterior != null && anterior.activeInHierarchy ? anterior : null);
    }

    void Cancelar()
    {
        if (Ocupado) return;
        PayphoneCheckpoint.TocarSomConfirmarGancho();
        aoCancelar();
    }


    static Button Botao(RectTransform r, Image img)
    {
        if (img == null) { img = r.gameObject.AddComponent<Image>(); img.color = new Color(.4f, .48f, .4f, .08f); }
        var b = r.gameObject.AddComponent<Button>(); b.targetGraphic = img;
        var cores = b.colors;
        cores.normalColor = Color.white;
        cores.highlightedColor = cores.selectedColor = new Color(1f, .91f, .57f, 1);
        cores.pressedColor = new Color(.55f, .66f, .48f, 1);
        cores.disabledColor = new Color(.35f, .36f, .32f, .5f);
        cores.fadeDuration = .08f; b.colors = cores;
        return b;
    }

    static RectTransform Rect(string nome, Transform pai, Vector2 pos, Vector2 tamanho)
    {
        var go = new GameObject(nome, typeof(RectTransform)); go.transform.SetParent(pai, false);
        var r = go.GetComponent<RectTransform>(); r.anchorMin = r.anchorMax = r.pivot = new Vector2(.5f, .5f);
        r.anchoredPosition = pos; r.sizeDelta = tamanho; return r;
    }

    static TMP_Text Texto(string nome, Transform pai, string texto, float tamanho, Vector2 pos, Vector2 dimensao, Color cor)
    {
        var r = Rect(nome, pai, pos, dimensao); var t = r.gameObject.AddComponent<TextMeshProUGUI>();
        t.font = UIFactory.Font; t.text = texto; t.fontSize = tamanho; t.color = cor;
        t.raycastTarget = false; t.alignment = TextAlignmentOptions.MidlineLeft;
        return t;
    }
}

public sealed class PayphoneRegistroTecla : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    PayphoneRegistroUI ui; int slot;
    public void Vincular(PayphoneRegistroUI dono, int registro) { ui = dono; slot = registro; }
    public void OnPointerEnter(PointerEventData data) { ui.Selecionar(slot); }
    public void OnSelect(BaseEventData data) { ui.Selecionar(slot); }
}


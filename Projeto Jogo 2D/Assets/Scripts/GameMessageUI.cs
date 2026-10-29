using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[Serializable]
public struct DialogoLinha
{
    public string falante;
    [TextArea(2, 4)]
    public string texto;
    public string retratoId;
    public Sprite retratoCustom;

    public DialogoLinha(string falante, string texto, string retratoId = "", Sprite retratoCustom = null)
    {
        this.falante = falante;
        this.texto = texto;
        this.retratoId = retratoId;
        this.retratoCustom = retratoCustom;
    }
}

// Caixa de diálogo com suporte a Retratos (Portraits), Nome do Falante e Múltiplas Páginas (R1).
// Mantém compatibilidade total com GameMessageUI.Instance.Show(string) e Avancar().
public class GameMessageUI : MonoBehaviour
{
    private static GameMessageUI instance;

    public static GameMessageUI Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("GameMessageUI");
                instance = go.AddComponent<GameMessageUI>();
                DontDestroyOnLoad(go);
            }

            return instance;
        }
    }

    public static bool IsShowing => instance != null && instance.aberta;

    [Header("Ritmo da digitação")]
    [SerializeField] private float letrasPorSegundo = 17f;
    [SerializeField] private float pausaFinal = 8f;

    // Layout do texto (unidades do canvas 640x360). Padding = esquerda, baixo,
    // direita, cima; a direita deixa a coluna da tecla E livre.
    private const float TamanhoFonte = 18f;
    private static readonly Vector4 PaddingTexto = new Vector4(12f, 10f, 32f, 9f);
    private const float LarguraRetrato = 64f;
    private const float LarguraInspecao = 560f;
    private const float AlturaInspecao = 292f;
    private const float MargemInspecao = 18f;
    private const float LadoArte = 196f;

    private CanvasGroup group;
    private RectTransform boxRect;
    private GameObject dialogueBox;
    private GameObject portraitFrame;
    private Image portraitImage;
    private GameObject speakerBadge;
    private TextMeshProUGUI speakerLabel;
    private TextMeshProUGUI label;
    private RectTransform textRect;
    private Image dica;
    private GameObject painelInspecao;
    private Image imagemInspecao;
    private TextMeshProUGUI tituloInspecao;
    private TextMeshProUGUI conteudoInspecao;
    private ScrollRect rolagemInspecao;
    private TextMeshProUGUI etiquetaInspecao;
    private TextMeshProUGUI ajudaInspecao;
    private RectTransform arteFrameInspecao;
    private RectTransform leituraInspecao;
    private Coroutine rotina;

    private bool aberta;
    private bool fechando;
    private bool digitando;
    private DialogoLinha[] paginasAtuais;
    private int indicePagina;
    private Action aoFecharCallback;

    private static readonly Dictionary<string, Sprite> portraitCache = new Dictionary<string, Sprite>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        Build();
    }

    private void Build()
    {
        Canvas canvas = UIFactory.CreateCanvas(transform, "DialogueCanvas", 9000);
        canvas.gameObject.AddComponent<GraphicRaycaster>();

        group = canvas.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.blocksRaycasts = false;

        Image box = UIFactory.CreateBox(canvas.transform, "Box", "CaixaDialogo");
        dialogueBox = box.gameObject;
        boxRect = box.rectTransform;
        boxRect.anchorMin = new Vector2(0.5f, 0f);
        boxRect.anchorMax = new Vector2(0.5f, 0f);
        boxRect.pivot = new Vector2(0.5f, 0f);
        boxRect.anchoredPosition = new Vector2(0f, 16f);
        boxRect.sizeDelta = new Vector2(460f, 82f);

        // Aba preta com o nome do falante, colada no canto superior esquerdo da caixa
        speakerBadge = new GameObject("SpeakerBadge", typeof(RectTransform), typeof(Image));
        speakerBadge.transform.SetParent(box.transform, false);
        RectTransform badgeRect = speakerBadge.GetComponent<RectTransform>();
        badgeRect.anchorMin = new Vector2(0f, 1f);
        badgeRect.anchorMax = new Vector2(0f, 1f);
        badgeRect.pivot = new Vector2(0f, 0f);
        badgeRect.anchoredPosition = Vector2.zero;
        badgeRect.sizeDelta = new Vector2(128f, 20f);
        speakerBadge.GetComponent<Image>().color = new Color32(7, 7, 8, 255);

        speakerLabel = UIFactory.CreateLabel(speakerBadge.transform, "SpeakerName", 16f);
        speakerLabel.alignment = TextAlignmentOptions.MidlineLeft;
        // Vermelho de sangue seco: a única cor quente da caixa.
        speakerLabel.color = new Color32(170, 72, 64, 255);
        RectTransform spRect = speakerLabel.rectTransform;
        UIFactory.Stretch(spRect);
        spRect.offsetMin = new Vector2(8f, 1f);
        spRect.offsetMax = new Vector2(-6f, -1f);

        // Moldura de Retrato (Portrait) à esquerda
        portraitFrame = new GameObject("PortraitFrame", typeof(RectTransform), typeof(Image));
        portraitFrame.transform.SetParent(box.transform, false);
        RectTransform pfRect = portraitFrame.GetComponent<RectTransform>();
        pfRect.anchorMin = new Vector2(0f, 0.5f);
        pfRect.anchorMax = new Vector2(0f, 0.5f);
        pfRect.pivot = new Vector2(0f, 0.5f);
        pfRect.anchoredPosition = new Vector2(12f, 0f);
        pfRect.sizeDelta = new Vector2(58f, 58f);
        portraitFrame.GetComponent<Image>().color = new Color32(4, 4, 5, 245);

        GameObject pGo = new GameObject("PortraitImg", typeof(RectTransform), typeof(Image));
        pGo.transform.SetParent(portraitFrame.transform, false);
        RectTransform pRect = pGo.GetComponent<RectTransform>();
        UIFactory.Stretch(pRect);
        pRect.offsetMin = new Vector2(2f, 2f);
        pRect.offsetMax = new Vector2(-2f, -2f);
        portraitImage = pGo.GetComponent<Image>();
        portraitImage.preserveAspect = true;
        portraitImage.raycastTarget = false;

        // Texto quebra linha dentro da caixa e, se ainda não couber, vira
        // páginas do TMP que o jogador avança com E (nada vaza da caixa).
        label = UIFactory.CreateLabel(box.transform, "Texto", TamanhoFonte);
        label.alignment = TextAlignmentOptions.TopLeft;
        label.textWrappingMode = TextWrappingModes.Normal;
        label.overflowMode = TextOverflowModes.Page;
        label.lineSpacing = 0f;
        label.color = new Color32(204, 199, 186, 255);
        textRect = label.rectTransform;
        UIFactory.Stretch(textRect);
        textRect.offsetMin = new Vector2(PaddingTexto.x, PaddingTexto.y);
        textRect.offsetMax = new Vector2(-PaddingTexto.z, -PaddingTexto.w);

        // A mesma tecla E da interação, no canto inferior direito da caixa.
        GameObject dicaGo = new GameObject("Dica", typeof(RectTransform), typeof(Image));
        dicaGo.transform.SetParent(box.transform, false);
        dica = dicaGo.GetComponent<Image>();
        dica.sprite = UIFactory.LoadSprite("TeclaE");
        dica.raycastTarget = false;
        RectTransform dicaRect = dica.rectTransform;
        dicaRect.anchorMin = dicaRect.anchorMax = dicaRect.pivot = new Vector2(1f, 0f);
        dicaRect.sizeDelta = new Vector2(17f, 17f);
        dicaRect.anchoredPosition = new Vector2(-8f, 8f);

        // Tela de inspeção: escurece o jogo, usa a mesma moldura da caixa de
        // diálogo e separa o que está no objeto (texto claro) do que Marcus
        // pensa sobre ele (itálico dourado). Sem arte, o texto ocupa a largura toda.
        painelInspecao = new GameObject("InspecaoDetalhada", typeof(RectTransform), typeof(Image));
        painelInspecao.transform.SetParent(canvas.transform, false);
        RectTransform fundoRect = painelInspecao.GetComponent<RectTransform>();
        UIFactory.Stretch(fundoRect);
        painelInspecao.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.62f);
        painelInspecao.transform.SetAsLastSibling();

        Image moldura = UIFactory.CreateBox(painelInspecao.transform, "MolduraInspecao", "CaixaDialogo");
        RectTransform painelRect = moldura.rectTransform;
        painelRect.anchorMin = painelRect.anchorMax = painelRect.pivot = new Vector2(0.5f, 0.5f);
        painelRect.sizeDelta = new Vector2(LarguraInspecao, AlturaInspecao);
        painelRect.anchoredPosition = new Vector2(0f, 6f);

        etiquetaInspecao = UIFactory.CreateLabel(painelRect, "EtiquetaInspecao", 13f);
        etiquetaInspecao.alignment = TextAlignmentOptions.TopLeft;
        PosicionarNoTopo(etiquetaInspecao.rectTransform, new Vector2(MargemInspecao, -10f), new Vector2(LarguraInspecao - MargemInspecao * 2f, 14f));

        tituloInspecao = UIFactory.CreateLabel(painelRect, "TituloInspecao", 21f);
        tituloInspecao.alignment = TextAlignmentOptions.TopLeft;
        tituloInspecao.textWrappingMode = TextWrappingModes.NoWrap;
        tituloInspecao.overflowMode = TextOverflowModes.Ellipsis;
        tituloInspecao.color = new Color(0.97f, 0.87f, 0.62f);
        PosicionarNoTopo(tituloInspecao.rectTransform, new Vector2(MargemInspecao, -23f), new Vector2(LarguraInspecao - MargemInspecao * 2f, 24f));

        Image divisor = new GameObject("Divisor", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        divisor.transform.SetParent(painelRect, false);
        divisor.color = new Color(0.62f, 0.50f, 0.34f, 0.55f);
        divisor.raycastTarget = false;
        PosicionarNoTopo(divisor.rectTransform, new Vector2(MargemInspecao, -50f), new Vector2(LarguraInspecao - MargemInspecao * 2f, 1f));

        GameObject arteFrameGo = new GameObject("MolduraArte", typeof(RectTransform), typeof(Image));
        arteFrameGo.transform.SetParent(painelRect, false);
        arteFrameGo.GetComponent<Image>().color = new Color32(10, 9, 8, 235);
        arteFrameGo.GetComponent<Image>().raycastTarget = false;
        arteFrameInspecao = arteFrameGo.GetComponent<RectTransform>();
        PosicionarNoTopo(arteFrameInspecao, new Vector2(MargemInspecao, -58f), new Vector2(LadoArte, LadoArte));

        GameObject imagemGo = new GameObject("ArteInspecao", typeof(RectTransform), typeof(Image));
        imagemGo.transform.SetParent(arteFrameInspecao, false);
        RectTransform imagemRect = imagemGo.GetComponent<RectTransform>();
        UIFactory.Stretch(imagemRect);
        imagemRect.offsetMin = new Vector2(8f, 8f);
        imagemRect.offsetMax = new Vector2(-8f, -8f);
        imagemInspecao = imagemGo.GetComponent<Image>();
        imagemInspecao.preserveAspect = true;
        imagemInspecao.raycastTarget = false;

        var leitura = new GameObject("AreaLeitura", typeof(RectTransform), typeof(Image), typeof(RectMask2D), typeof(ScrollRect));
        leitura.transform.SetParent(painelRect, false);
        leituraInspecao = leitura.GetComponent<RectTransform>();
        leitura.GetComponent<Image>().color = new Color(0, 0, 0, .03f);
        rolagemInspecao = leitura.GetComponent<ScrollRect>();
        rolagemInspecao.horizontal = false;
        rolagemInspecao.movementType = ScrollRect.MovementType.Clamped;
        rolagemInspecao.scrollSensitivity = 24f;
        rolagemInspecao.viewport = leituraInspecao;
        conteudoInspecao = UIFactory.CreateLabel(leitura.transform, "Transcricao", 17f);
        conteudoInspecao.alignment = TextAlignmentOptions.TopLeft;
        conteudoInspecao.color = new Color(0.94f, 0.90f, 0.82f);
        conteudoInspecao.textWrappingMode = TextWrappingModes.Normal;
        conteudoInspecao.lineSpacing = 4f;
        RectTransform conteudoRect = conteudoInspecao.rectTransform;
        conteudoRect.anchorMin = new Vector2(0, 1);
        conteudoRect.anchorMax = new Vector2(1, 1);
        conteudoRect.pivot = new Vector2(.5f, 1);
        conteudoRect.sizeDelta = new Vector2(-6, 0);
        conteudoRect.anchoredPosition = Vector2.zero;
        var ajustarTexto = conteudoInspecao.gameObject.AddComponent<ContentSizeFitter>();
        ajustarTexto.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        rolagemInspecao.content = conteudoRect;

        ajudaInspecao = UIFactory.CreateLabel(painelRect, "AjudaInspecao", 13f);
        ajudaInspecao.alignment = TextAlignmentOptions.BottomRight;
        ajudaInspecao.color = new Color(0.88f, 0.75f, 0.52f);
        RectTransform ajudaRect = ajudaInspecao.rectTransform;
        ajudaRect.anchorMin = ajudaRect.anchorMax = ajudaRect.pivot = new Vector2(1f, 0f);
        ajudaRect.anchoredPosition = new Vector2(-MargemInspecao, 9f);
        ajudaRect.sizeDelta = new Vector2(LarguraInspecao - MargemInspecao * 2f, 14f);
        painelInspecao.SetActive(false);
    }

    private static void PosicionarNoTopo(RectTransform r, Vector2 pos, Vector2 tamanho)
    {
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0f, 1f);
        r.anchoredPosition = pos;
        r.sizeDelta = tamanho;
    }

    // pensamento: o que Marcus conclui ou sente (aparece separado, em itálico).
    // etiqueta: linha curta acima do título, ex. "PISTA · ANOTADA NO DIÁRIO".
    public void ShowInspection(string titulo, string nomeSprite, string transcricao, Action aoFechar = null, Sprite arteCustom = null,
        string pensamento = null, string etiqueta = null)
    {
        SantaRitaSound.Play("inspect", .12f);
        Sprite arte = arteCustom != null ? arteCustom : CarregarArteInspecao(nomeSprite);
        bool temArte = arte != null;

        imagemInspecao.sprite = arte;
        arteFrameInspecao.gameObject.SetActive(temArte);
        float xTexto = temArte ? MargemInspecao + LadoArte + 14f : MargemInspecao;
        PosicionarNoTopo(leituraInspecao, new Vector2(xTexto, -58f),
            new Vector2(LarguraInspecao - xTexto - MargemInspecao, LadoArte));

        bool anotada = !string.IsNullOrEmpty(etiqueta) && etiqueta.ToUpperInvariant().Contains("DIÁRIO");
        etiquetaInspecao.text = string.IsNullOrEmpty(etiqueta) ? "EXAMINAR" : etiqueta.ToUpperInvariant();
        etiquetaInspecao.color = anotada ? new Color32(152, 232, 152, 255) : new Color32(150, 138, 118, 255);
        tituloInspecao.text = titulo;

        string corpo = transcricao ?? string.Empty;
        if (!string.IsNullOrEmpty(pensamento))
            corpo += (corpo.Length > 0 ? "\n\n" : string.Empty) + "<color=#D2B27A><i>" + pensamento + "</i></color>";
        conteudoInspecao.text = corpo;

        painelInspecao.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(conteudoInspecao.rectTransform);
        Canvas.ForceUpdateCanvases();
        rolagemInspecao.verticalNormalizedPosition = 1;
        bool rola = conteudoInspecao.preferredHeight > leituraInspecao.rect.height + 1f;
        ajudaInspecao.text = rola ? "[CIMA/BAIXO] LER MAIS    [E] FECHAR" : "[E] FECHAR";

        Show(new[] { new DialogoLinha("", "", "") }, () =>
        {
            painelInspecao.SetActive(false);
            dialogueBox.SetActive(true);
            aoFechar?.Invoke();
        });
        dialogueBox.SetActive(false);
    }

    private static Sprite CarregarArteInspecao(string nomeSprite)
    {
        if (string.IsNullOrEmpty(nomeSprite)) return null;
        Sprite arte = Resources.Load<Sprite>("Cap1/" + nomeSprite);
        if (arte == null)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Cap1/" + nomeSprite);
            if (sprites.Length > 0) arte = sprites[0];
        }
        if (arte == null)
        {
            string caminho = System.IO.Path.Combine(Application.streamingAssetsPath, nomeSprite + ".png");
            if (System.IO.File.Exists(caminho))
            {
                Texture2D detalhe = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (ImageConversion.LoadImage(detalhe, System.IO.File.ReadAllBytes(caminho)))
                {
                    detalhe.filterMode = FilterMode.Point;
                    arte = Sprite.Create(detalhe, new Rect(0, 0, detalhe.width, detalhe.height),
                        new Vector2(0.5f, 0.5f), 32f);
                }
            }
        }
        return arte;
    }

    private void Update()
    {
        if (!aberta || painelInspecao == null || !painelInspecao.activeSelf || Keyboard.current == null || MenuPausa.Aberto) return;
        var k = Keyboard.current;
        float eixo = (k.upArrowKey.isPressed || k.pageUpKey.isPressed ? 1f : 0f) -
            (k.downArrowKey.isPressed || k.pageDownKey.isPressed ? 1f : 0f);
        if (eixo != 0) rolagemInspecao.verticalNormalizedPosition = Mathf.Clamp01(rolagemInspecao.verticalNormalizedPosition + eixo * Time.unscaledDeltaTime * .7f);
    }

    public void Show(string mensagem)
    {
        Show(mensagem, "", "", null);
    }

    public void Show(string mensagem, string falante, string retratoId, Action aoFechar = null)
    {
        DialogoLinha linha = ParseLinhaAutomatica(mensagem, falante, retratoId);
        Show(new[] { linha }, aoFechar);
    }

    public void ShowDialogue(List<DialogoLinha> linhas, Action aoFechar = null)
    {
        Show(linhas != null ? linhas.ToArray() : null, aoFechar);
    }

    public void Show(DialogoLinha[] linhas, Action aoFechar = null)
    {
        if (linhas == null || linhas.Length == 0)
        {
            aoFechar?.Invoke();
            return;
        }

        if (!aberta) SantaRitaSound.Play("dialogue", .07f);
        paginasAtuais = linhas;
        fechando = false;
        if (dialogueBox != null) dialogueBox.SetActive(true);
        indicePagina = 0;
        aoFecharCallback = aoFechar;

        if (rotina != null)
        {
            StopCoroutine(rotina);
        }

        rotina = StartCoroutine(RotinaPagina(primeiraAbertura: !aberta));
    }

    private static DialogoLinha ParseLinhaAutomatica(string raw, string falanteExplicito, string retratoExplicito)
    {
        if (!string.IsNullOrEmpty(falanteExplicito) || !string.IsNullOrEmpty(retratoExplicito))
            return new DialogoLinha(falanteExplicito, raw, retratoExplicito);

        if (string.IsNullOrEmpty(raw))
            return new DialogoLinha("", "", "");

        int colon = raw.IndexOf(':');
        if (colon > 0 && colon <= 18)
        {
            string possivelNome = raw.Substring(0, colon).Trim();
            string resto = raw.Substring(colon + 1).Trim();
            string lower = possivelNome.ToLowerInvariant();
            if (lower == "marcus" || lower == "zé" || lower == "ze" || lower == "frentista" || lower == "atendente")
            {
                return new DialogoLinha(possivelNome, resto, lower);
            }
        }

        return new DialogoLinha("", raw, "");
    }

    // Chamado pelo PlayerController quando o jogador aperta E com a caixa
    // aberta: primeiro pulo completa o texto, o segundo avança página ou fecha.
    public void Avancar()
    {
        if (!aberta || fechando)
        {
            return;
        }

        if (digitando)
        {
            digitando = false;
            return;
        }

        if (rotina != null)
        {
            StopCoroutine(rotina);
        }

        SantaRitaSound.Play("page", .065f);

        // Fala longa: mostra o resto dela antes de ir para a próxima.
        if (label.pageToDisplay < label.textInfo.pageCount)
        {
            rotina = StartCoroutine(RotinaContinuacao());
            return;
        }

        indicePagina++;
        if (paginasAtuais != null && indicePagina < paginasAtuais.Length)
        {
            rotina = StartCoroutine(RotinaPagina(primeiraAbertura: false));
        }
        else
        {
            rotina = StartCoroutine(Fechar());
        }
    }

    private IEnumerator RotinaPagina(bool primeiraAbertura)
    {
        aberta = true;
        DialogoLinha atual = paginasAtuais[indicePagina];

        ConfigurarRetratoEFalante(atual);
        label.text = string.Empty;
        dica.enabled = false;

        if (primeiraAbertura)
            yield return Fade(1f, 0.16f);
        else
            group.alpha = 1f;

        yield return Digitar(atual.texto ?? string.Empty);

        dica.enabled = true;

        // Só fecha sozinho se for uma única linha simples, que coube numa
        // página, sem callback de sequência
        if (paginasAtuais.Length == 1 && aoFecharCallback == null && label.textInfo.pageCount <= 1)
        {
            float espera = 0f;
            while (espera < pausaFinal && aberta)
            {
                espera += Time.deltaTime;
                yield return null;
            }

            rotina = StartCoroutine(Fechar());
        }
    }

    private void ConfigurarRetratoEFalante(DialogoLinha linha)
    {
        bool temFalante = !string.IsNullOrWhiteSpace(linha.falante);
        speakerBadge.SetActive(temFalante);
        if (temFalante)
            speakerLabel.text = linha.falante.ToUpperInvariant();

        Sprite retrato = linha.retratoCustom != null
            ? linha.retratoCustom
            : ResolverRetrato(linha.retratoId, linha.falante);

        bool temRetrato = retrato != null;
        portraitFrame.SetActive(temRetrato);
        if (temRetrato)
        {
            portraitImage.sprite = retrato;
            textRect.offsetMin = new Vector2(PaddingTexto.x + LarguraRetrato, PaddingTexto.y);
        }
        else
        {
            textRect.offsetMin = new Vector2(PaddingTexto.x, PaddingTexto.y);
        }
    }

    private IEnumerator RotinaContinuacao()
    {
        dica.enabled = false;
        label.pageToDisplay++;
        yield return DigitarPaginaAtual();
        dica.enabled = true;
    }

    private IEnumerator Digitar(string textoCompleto)
    {
        label.text = textoCompleto;
        label.pageToDisplay = 1;
        yield return DigitarPaginaAtual();
    }

    // Digita só os caracteres da página do TMP que está na tela.
    private IEnumerator DigitarPaginaAtual()
    {
        digitando = true;
        label.maxVisibleCharacters = 0;
        label.ForceMeshUpdate();

        TMP_TextInfo info = label.textInfo;
        int pagina = Mathf.Clamp(label.pageToDisplay - 1, 0, Mathf.Max(0, info.pageCount - 1));
        int visiveis = info.pageCount > 0 ? info.pageInfo[pagina].firstCharacterIndex : 0;
        int total = info.pageCount > 0 ? info.pageInfo[pagina].lastCharacterIndex + 1 : info.characterCount;
        label.maxVisibleCharacters = visiveis;

        float porLetra = 1f / Mathf.Max(1f, letrasPorSegundo);

        while (visiveis < total)
        {
            if (!digitando)
            {
                break;
            }

            visiveis++;
            label.maxVisibleCharacters = visiveis;
            yield return new WaitForSeconds(porLetra);
        }

        digitando = false;
        label.maxVisibleCharacters = int.MaxValue;
    }

    private IEnumerator Fechar()
    {
        fechando = true;
        yield return Fade(0f, 0.25f);
        aberta = false;
        fechando = false;
        digitando = false;
        rotina = null;

        var cb = aoFecharCallback;
        aoFecharCallback = null;
        cb?.Invoke();
    }

    private IEnumerator Fade(float alvo, float duracao)
    {
        float inicio = group.alpha;
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            group.alpha = Mathf.Lerp(inicio, alvo, tempo / duracao);
            yield return null;
        }

        group.alpha = alvo;
    }

    public static Sprite ResolverRetrato(string retratoId, string falante = "")
    {
        string key = !string.IsNullOrWhiteSpace(retratoId) ? retratoId.Trim().ToLowerInvariant() : "";
        if (string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(falante))
        {
            string f = falante.Trim().ToLowerInvariant();
            if (f.Contains("marcus")) key = "marcus";
            else if (f.Contains("frentista") || f.Contains("posto")) key = "frentista";
            else if (f.Contains("atendente") || f.Contains("mercadinho")) key = "atendente";
            else if (f == "zé" || f == "ze" || f.Contains("velho")) key = "ze";
            else if (f.Contains("adilio") || f.Contains("avô")) key = "adilio";
        }

        if (string.IsNullOrEmpty(key)) return null;

        if (portraitCache.TryGetValue(key, out Sprite cached) && cached != null)
            return cached;

        string portraitPath = System.IO.Path.Combine(Application.dataPath, "Resources/UI/Portraits/" + key + ".png");
        if (System.IO.File.Exists(portraitPath))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(portraitPath);
            Texture2D texDisk = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (ImageConversion.LoadImage(texDisk, bytes))
            {
                texDisk.filterMode = FilterMode.Point;
                texDisk.wrapMode = TextureWrapMode.Clamp;
                Sprite sprDisk = Sprite.Create(texDisk, new Rect(0, 0, texDisk.width, texDisk.height), new Vector2(0.5f, 0.5f), 32f);
                portraitCache[key] = sprDisk;
                return sprDisk;
            }
        }

        Sprite resourceSprite = Resources.Load<Sprite>("UI/Portraits/" + key);
        if (resourceSprite != null)
        {
            portraitCache[key] = resourceSprite;
            return resourceSprite;
        }

        Sprite generated = GerarRetratoPixelArt(key);
        if (generated != null)
            portraitCache[key] = generated;
        return generated;
    }

    private static Sprite GerarRetratoPixelArt(string key)
    {
        const int size = 32;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        Color bg = new Color32(18, 22, 30, 255);
        Color skin = new Color32(56, 34, 24, 255); // Marcus: negro retinto, 30 anos
        Color skinShadow = new Color32(38, 22, 15, 255);
        Color hair = new Color32(14, 12, 14, 255);
        Color shirt = new Color32(20, 20, 24, 255);
        Color accent = new Color32(42, 44, 52, 255);
        bool hasCap = false;
        bool hasMustache = false;
        bool hasGrayHair = false;

        if (key == "marcus")
        {
            bg = new Color32(16, 22, 32, 255);
            skin = new Color32(56, 34, 24, 255);
            skinShadow = new Color32(38, 22, 15, 255);
            hair = new Color32(14, 12, 14, 255);
            shirt = new Color32(20, 20, 24, 255);
            accent = new Color32(42, 44, 52, 255);
        }
        else if (key == "frentista")
        {
            bg = new Color32(20, 26, 24, 255);
            skin = new Color32(168, 122, 92, 255);
            skinShadow = new Color32(132, 92, 68, 255);
            hair = new Color32(35, 28, 22, 255);
            shirt = new Color32(34, 68, 52, 255);
            accent = new Color32(195, 145, 48, 255);
            hasCap = true;
        }
        else if (key == "atendente")
        {
            bg = new Color32(26, 22, 28, 255);
            skin = new Color32(188, 142, 112, 255);
            skinShadow = new Color32(148, 108, 84, 255);
            hair = new Color32(48, 28, 20, 255);
            shirt = new Color32(110, 38, 36, 255);
            accent = new Color32(210, 195, 165, 255);
        }
        else if (key == "ze" || key == "zé")
        {
            bg = new Color32(28, 20, 16, 255);
            skin = new Color32(154, 108, 78, 255);
            skinShadow = new Color32(116, 78, 54, 255);
            hair = new Color32(140, 140, 145, 255);
            shirt = new Color32(48, 34, 26, 255);
            accent = new Color32(185, 175, 155, 255);
            hasMustache = true;
            hasGrayHair = true;
        }
        else if (key == "adilio")
        {
            bg = new Color32(24, 20, 18, 255);
            skin = new Color32(64, 40, 28, 255);
            skinShadow = new Color32(44, 26, 18, 255);
            hair = new Color32(165, 165, 170, 255);
            shirt = new Color32(72, 58, 44, 255);
            accent = new Color32(130, 110, 85, 255);
            hasGrayHair = true;
        }
        else
        {
            return null;
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool border = x == 0 || y == 0 || x == size - 1 || y == size - 1;
                tex.SetPixel(x, y, border ? new Color32(42, 38, 34, 255) : bg);
            }
        }

        for (int y = 1; y <= 10; y++)
        {
            int halfWidth = 10 - (y - 1) / 3;
            for (int x = 16 - halfWidth; x <= 16 + halfWidth; x++)
            {
                bool gola = Mathf.Abs(x - 16) <= 2 && y >= 7;
                tex.SetPixel(x, y, gola ? accent : shirt);
            }
        }

        for (int y = 10; y <= 12; y++)
            for (int x = 13; x <= 19; x++)
                tex.SetPixel(x, y, skinShadow);

        for (int y = 12; y <= 24; y++)
        {
            for (int x = 10; x <= 22; x++)
            {
                if ((y == 12 || y == 24) && (x == 10 || x == 22)) continue;
                Color c = (x <= 12 || y <= 13) ? skinShadow : skin;
                tex.SetPixel(x, y, c);
            }
        }

        for (int y = 22; y <= 27; y++)
        {
            for (int x = 10; x <= 22; x++)
            {
                if (y == 27 && (x == 10 || x == 22)) continue;
                if (y >= 24 || x == 10 || x == 22)
                    tex.SetPixel(x, y, hasCap ? shirt : hair);
            }
        }

        if (hasCap)
        {
            for (int x = 10; x <= 24; x++)
                tex.SetPixel(x, 23, accent);
        }

        Color eyeWhite = new Color32(215, 210, 200, 255);
        Color pupil = new Color32(15, 12, 12, 255);
        tex.SetPixel(13, 19, eyeWhite);
        tex.SetPixel(14, 19, pupil);
        tex.SetPixel(18, 19, eyeWhite);
        tex.SetPixel(19, 19, pupil);

        Color brow = hasGrayHair ? hair : new Color32(18, 14, 14, 255);
        for (int x = 12; x <= 15; x++) tex.SetPixel(x, 21, brow);
        for (int x = 18; x <= 20; x++) tex.SetPixel(x, 21, brow);

        if (hasMustache)
        {
            for (int x = 13; x <= 19; x++)
                tex.SetPixel(x, 15, hair);
            tex.SetPixel(13, 14, hair);
            tex.SetPixel(19, 14, hair);
        }
        else
        {
            for (int x = 14; x <= 18; x++)
                tex.SetPixel(x, 14, skinShadow);
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f);
    }
}

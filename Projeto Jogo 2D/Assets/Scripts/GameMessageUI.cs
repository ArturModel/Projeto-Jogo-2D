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
    [SerializeField] private float letrasPorSegundo = 24f;
    [SerializeField] private float pausaFinal = 8f;

    private CanvasGroup group;
    private RectTransform boxRect;
    private GameObject dialogueBox;
    private GameObject portraitFrame;
    private Image portraitImage;
    private GameObject speakerBadge;
    private TextMeshProUGUI speakerLabel;
    private TextMeshProUGUI label;
    private RectTransform textRect;
    private TextMeshProUGUI dica;
    private GameObject painelInspecao;
    private Image imagemInspecao;
    private TextMeshProUGUI tituloInspecao;
    private TextMeshProUGUI conteudoInspecao;
    private ScrollRect rolagemInspecao;
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

        Image box = UIFactory.CreateBox(canvas.transform, "Box", "DialogueBox");
        dialogueBox = box.gameObject;
        boxRect = box.rectTransform;
        boxRect.anchorMin = new Vector2(0.5f, 0f);
        boxRect.anchorMax = new Vector2(0.5f, 0f);
        boxRect.pivot = new Vector2(0.5f, 0f);
        boxRect.anchoredPosition = new Vector2(0f, 16f);
        boxRect.sizeDelta = new Vector2(460f, 82f);

        // Crachá com o nome do falante acima da caixa
        speakerBadge = new GameObject("SpeakerBadge", typeof(RectTransform), typeof(Image));
        speakerBadge.transform.SetParent(box.transform, false);
        RectTransform badgeRect = speakerBadge.GetComponent<RectTransform>();
        badgeRect.anchorMin = new Vector2(0f, 1f);
        badgeRect.anchorMax = new Vector2(0f, 1f);
        badgeRect.pivot = new Vector2(0f, 0f);
        badgeRect.anchoredPosition = new Vector2(10f, -2f);
        badgeRect.sizeDelta = new Vector2(140f, 20f);
        speakerBadge.GetComponent<Image>().color = new Color(0.11f, 0.09f, 0.08f, 0.96f);

        speakerLabel = UIFactory.CreateLabel(speakerBadge.transform, "SpeakerName", 16f);
        speakerLabel.alignment = TextAlignmentOptions.MidlineLeft;
        speakerLabel.color = new Color(0.95f, 0.78f, 0.42f, 1f);
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
        portraitFrame.GetComponent<Image>().color = new Color(0.18f, 0.15f, 0.13f, 0.96f);

        GameObject pGo = new GameObject("PortraitImg", typeof(RectTransform), typeof(Image));
        pGo.transform.SetParent(portraitFrame.transform, false);
        RectTransform pRect = pGo.GetComponent<RectTransform>();
        UIFactory.Stretch(pRect);
        pRect.offsetMin = new Vector2(2f, 2f);
        pRect.offsetMax = new Vector2(-2f, -2f);
        portraitImage = pGo.GetComponent<Image>();
        portraitImage.preserveAspect = true;
        portraitImage.raycastTarget = false;

        label = UIFactory.CreateLabel(box.transform, "Texto", 19f);
        label.alignment = TextAlignmentOptions.TopLeft;
        textRect = label.rectTransform;
        UIFactory.Stretch(textRect);
        textRect.offsetMin = new Vector2(14f, 14f);
        textRect.offsetMax = new Vector2(-14f, -10f);

        dica = UIFactory.CreateLabel(box.transform, "Dica", 16f);
        dica.alignment = TextAlignmentOptions.BottomRight;
        dica.color = new Color(0.85f, 0.75f, 0.48f, 1f);
        dica.text = "[E]";
        RectTransform dicaRect = dica.rectTransform;
        UIFactory.Stretch(dicaRect);
        dicaRect.offsetMin = new Vector2(0f, 6f);
        dicaRect.offsetMax = new Vector2(-12f, 0f);

        painelInspecao = new GameObject("InspecaoDetalhada", typeof(RectTransform), typeof(Image));
        painelInspecao.transform.SetParent(canvas.transform, false);
        RectTransform painelRect = painelInspecao.GetComponent<RectTransform>();
        painelRect.anchorMin = painelRect.anchorMax = new Vector2(0.5f, 0.5f);
        painelRect.sizeDelta = new Vector2(600f, 330f);
        painelRect.anchoredPosition = Vector2.zero;
        painelInspecao.GetComponent<Image>().color = new Color(0.10f, 0.085f, 0.07f, 0.98f);
        painelInspecao.transform.SetAsLastSibling();

        tituloInspecao = UIFactory.CreateLabel(painelInspecao.transform, "TituloInspecao", 20f);
        tituloInspecao.alignment = TextAlignmentOptions.Center;
        tituloInspecao.color = new Color(0.97f, 0.87f, 0.62f);
        RectTransform tituloRect = tituloInspecao.rectTransform;
        tituloRect.anchorMin = tituloRect.anchorMax = new Vector2(0.5f, 1f);
        tituloRect.sizeDelta = new Vector2(560f, 28f);
        tituloRect.anchoredPosition = new Vector2(0f, -17f);

        GameObject imagemGo = new GameObject("ArteInspecao", typeof(RectTransform), typeof(Image));
        imagemGo.transform.SetParent(painelInspecao.transform, false);
        RectTransform imagemRect = imagemGo.GetComponent<RectTransform>();
        imagemRect.anchorMin = imagemRect.anchorMax = new Vector2(0.5f, 0.5f);
        imagemRect.sizeDelta = new Vector2(255f, 240f);
        imagemRect.anchoredPosition = new Vector2(-150f, -6f);
        imagemInspecao = imagemGo.GetComponent<Image>();
        imagemInspecao.preserveAspect = true;
        imagemInspecao.raycastTarget = false;

        var leitura = new GameObject("AreaLeitura", typeof(RectTransform), typeof(Image), typeof(RectMask2D), typeof(ScrollRect));
        leitura.transform.SetParent(painelInspecao.transform, false);
        var leituraRect = leitura.GetComponent<RectTransform>();
        leituraRect.sizeDelta = new Vector2(272f, 242f);
        leituraRect.anchoredPosition = new Vector2(142f, -10f);
        leitura.GetComponent<Image>().color = new Color(0,0,0,.03f);
        rolagemInspecao = leitura.GetComponent<ScrollRect>();
        rolagemInspecao.horizontal = false;
        rolagemInspecao.movementType = ScrollRect.MovementType.Clamped;
        rolagemInspecao.scrollSensitivity = 24f;
        rolagemInspecao.viewport = leituraRect;
        conteudoInspecao = UIFactory.CreateLabel(leitura.transform, "Transcricao", 18f);
        conteudoInspecao.alignment = TextAlignmentOptions.TopLeft;
        conteudoInspecao.color = new Color(0.96f, 0.91f, 0.81f);
        conteudoInspecao.textWrappingMode = TextWrappingModes.Normal;
        RectTransform conteudoRect = conteudoInspecao.rectTransform;
        conteudoRect.anchorMin = new Vector2(0,1);
        conteudoRect.anchorMax = new Vector2(1,1);
        conteudoRect.pivot = new Vector2(.5f,1);
        conteudoRect.sizeDelta = new Vector2(-4,0);
        conteudoRect.anchoredPosition = Vector2.zero;
        var ajustarTexto = conteudoInspecao.gameObject.AddComponent<ContentSizeFitter>();
        ajustarTexto.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        rolagemInspecao.content = conteudoRect;
        TextMeshProUGUI ajudaInspecao = UIFactory.CreateLabel(painelInspecao.transform, "AjudaInspecao", 13f);
        ajudaInspecao.alignment = TextAlignmentOptions.BottomRight;
        ajudaInspecao.color = new Color(0.88f, 0.75f, 0.52f);
        ajudaInspecao.text = "[RODA / CIMA / BAIXO] LER   •   [E] FECHAR";
        RectTransform ajudaRect = ajudaInspecao.rectTransform;
        ajudaRect.anchorMin = ajudaRect.anchorMax = new Vector2(1f, 0f);
        ajudaRect.pivot = new Vector2(1f, 0f);
        ajudaRect.anchoredPosition = new Vector2(-16f, 9f);
        ajudaRect.sizeDelta = new Vector2(460f, 22f);
        painelInspecao.SetActive(false);
    }

    public void ShowInspection(string titulo, string nomeSprite, string transcricao, Action aoFechar = null, Sprite arteCustom = null)
    {
        SantaRitaSound.Play("inspect", .12f);
        Sprite arte = arteCustom != null ? arteCustom : Resources.Load<Sprite>("Cap1/" + nomeSprite);
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
        imagemInspecao.sprite = arte;
        painelInspecao.SetActive(true);
        imagemInspecao.enabled = arte != null;
        tituloInspecao.text = titulo;
        conteudoInspecao.text = transcricao;
        LayoutRebuilder.ForceRebuildLayoutImmediate(conteudoInspecao.rectTransform);
        Canvas.ForceUpdateCanvases();
        rolagemInspecao.verticalNormalizedPosition = 1;
        Show(new[] { new DialogoLinha("", "", "") }, () =>
        {
            painelInspecao.SetActive(false);
            dialogueBox.SetActive(true);
            aoFechar?.Invoke();
        });
        dialogueBox.SetActive(false);
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
        dica.alpha = 0f;

        if (primeiraAbertura)
            yield return Fade(1f, 0.16f);
        else
            group.alpha = 1f;

        yield return Digitar(atual.texto ?? string.Empty);

        dica.alpha = 1f;

        // Só fecha sozinho se for uma única linha simples sem callback de sequência
        if (paginasAtuais.Length == 1 && aoFecharCallback == null)
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
            textRect.offsetMin = new Vector2(78f, 14f);
        }
        else
        {
            textRect.offsetMin = new Vector2(14f, 14f);
        }
    }

    private IEnumerator Digitar(string textoCompleto)
    {
        digitando = true;
        label.text = textoCompleto;
        label.maxVisibleCharacters = 0;

        label.ForceMeshUpdate();
        int total = label.textInfo.characterCount;

        float porLetra = 1f / Mathf.Max(1f, letrasPorSegundo);
        int visiveis = 0;

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

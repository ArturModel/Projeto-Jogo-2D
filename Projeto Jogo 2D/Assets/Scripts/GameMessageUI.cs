using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Caixa de diálogo: o texto aparece letra por letra, como quem lê devagar
// alguma coisa que não queria estar lendo. E avança / pula a digitação.
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
    [SerializeField] private float letrasPorSegundo = 14f;
    [SerializeField] private float pausaFinal = 6f;

    private CanvasGroup group;
    private TextMeshProUGUI label;
    private TextMeshProUGUI dica;
    private Coroutine rotina;

    private bool aberta;
    private bool digitando;
    private string textoCompleto = string.Empty;

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

        group = canvas.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.blocksRaycasts = false;

        Image box = UIFactory.CreateBox(canvas.transform, "Box", "DialogueBox");
        RectTransform boxRect = box.rectTransform;
        boxRect.anchorMin = new Vector2(0.5f, 0f);
        boxRect.anchorMax = new Vector2(0.5f, 0f);
        boxRect.pivot = new Vector2(0.5f, 0f);
        boxRect.anchoredPosition = new Vector2(0f, 16f);
        boxRect.sizeDelta = new Vector2(380f, 68f);

        // Dobrado junto com a resolução de referência: o tamanho físico
        // na tela continua o mesmo de antes.
        label = UIFactory.CreateLabel(box.transform, "Texto", 20f);
        label.alignment = TextAlignmentOptions.TopLeft;
        RectTransform textRect = label.rectTransform;
        UIFactory.Stretch(textRect);
        textRect.offsetMin = new Vector2(14f, 14f);
        textRect.offsetMax = new Vector2(-14f, -10f);

        dica = UIFactory.CreateLabel(box.transform, "Dica", 18f);
        dica.alignment = TextAlignmentOptions.BottomRight;
        dica.color = new Color(0.6f, 0.58f, 0.54f, 1f);
        dica.text = "E";
        RectTransform dicaRect = dica.rectTransform;
        UIFactory.Stretch(dicaRect);
        dicaRect.offsetMin = new Vector2(0f, 6f);
        dicaRect.offsetMax = new Vector2(-12f, 0f);
    }

    public void Show(string mensagem)
    {
        textoCompleto = mensagem;

        if (rotina != null)
        {
            StopCoroutine(rotina);
        }

        rotina = StartCoroutine(Rotina());
    }

    // Chamado pelo PlayerController quando o jogador aperta E com a caixa
    // aberta: primeiro pulo completa o texto, o segundo fecha.
    public void Avancar()
    {
        if (!aberta)
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

        rotina = StartCoroutine(Fechar());
    }

    private IEnumerator Rotina()
    {
        aberta = true;
        label.text = string.Empty;
        dica.alpha = 0f;

        yield return Fade(1f, 0.2f);
        yield return Digitar();

        dica.alpha = 1f;

        float espera = 0f;
        while (espera < pausaFinal && aberta)
        {
            espera += Time.deltaTime;
            yield return null;
        }

        rotina = StartCoroutine(Fechar());
    }

    private IEnumerator Digitar()
    {
        digitando = true;
        label.text = textoCompleto;
        label.maxVisibleCharacters = 0;

        // characterCount ignora as tags de rich text (<size=...>), então a
        // contagem bate com o que realmente aparece na tela.
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
        yield return Fade(0f, 0.4f);
        aberta = false;
        digitando = false;
        rotina = null;
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
}

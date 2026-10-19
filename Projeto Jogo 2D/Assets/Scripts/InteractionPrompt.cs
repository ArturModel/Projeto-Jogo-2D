using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Aviso discreto de "dá pra fazer alguma coisa aqui": uma tecla E e o nome
// da ação. Aparece só quando há algo ao alcance e some sozinho.
public class InteractionPrompt : MonoBehaviour
{
    private static InteractionPrompt instance;

    public static InteractionPrompt Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("InteractionPrompt");
                instance = go.AddComponent<InteractionPrompt>();
                DontDestroyOnLoad(go);
            }

            return instance;
        }
    }

    [SerializeField] private float velocidadeFade = 8f;

    private CanvasGroup group;
    private RectTransform boxRect;
    private TextMeshProUGUI tecla;
    private TextMeshProUGUI acao;

    private float alvoAlpha;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        Build();
        SceneManager.sceneLoaded += LimparAoTrocarCena;
    }

    private void OnDestroy()
    {
        if (instance == this) SceneManager.sceneLoaded -= LimparAoTrocarCena;
    }

    private void LimparAoTrocarCena(Scene cena, LoadSceneMode modo)
    {
        SetTarget(null);
        if (group != null) group.alpha = 0f;
    }

    private void Build()
    {
        Canvas canvas = UIFactory.CreateCanvas(transform, "PromptCanvas", 8900);

        group = canvas.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.blocksRaycasts = false;

        Image box = UIFactory.CreateBox(canvas.transform, "Box", "PromptBox");
        boxRect = box.rectTransform;
        boxRect.anchorMin = new Vector2(0.5f, 0f);
        boxRect.anchorMax = new Vector2(0.5f, 0f);
        boxRect.pivot = new Vector2(0.5f, 0f);
        boxRect.anchoredPosition = new Vector2(0f, 96f);
        boxRect.sizeDelta = new Vector2(116f, 26f);

        // A tecla, escura sobre o cinza da caixa, como um teclado de verdade.
        tecla = UIFactory.CreateLabel(box.transform, "Tecla", 18f);
        tecla.text = "E";
        tecla.color = new Color(0.12f, 0.12f, 0.13f, 1f);
        tecla.alignment = TextAlignmentOptions.Left;
        RectTransform teclaRect = tecla.rectTransform;
        UIFactory.Stretch(teclaRect);
        teclaRect.offsetMin = new Vector2(12f, 2f);
        teclaRect.offsetMax = new Vector2(0f, -2f);

        acao = UIFactory.CreateLabel(box.transform, "Acao", 18f);
        acao.color = new Color(0.18f, 0.18f, 0.19f, 1f);
        acao.alignment = TextAlignmentOptions.Right;
        RectTransform acaoRect = acao.rectTransform;
        UIFactory.Stretch(acaoRect);
        acaoRect.offsetMin = new Vector2(0f, 2f);
        acaoRect.offsetMax = new Vector2(-12f, -2f);
    }

    public void SetTarget(Interactable alvo)
    {
        bool mostrar = alvo != null && !GameMessageUI.IsShowing;

        if (mostrar)
        {
            acao.text = alvo.Prompt;

            // A caixa acompanha o tamanho do texto da ação.
            float largura = Mathf.Clamp(acao.preferredWidth + 44f, 88f, 300f);
            boxRect.sizeDelta = new Vector2(largura, 26f);
        }

        alvoAlpha = mostrar ? 1f : 0f;
    }

    private void Update()
    {
        group.alpha = Mathf.MoveTowards(
            group.alpha,
            alvoAlpha,
            velocidadeFade * Time.deltaTime
        );
    }
}

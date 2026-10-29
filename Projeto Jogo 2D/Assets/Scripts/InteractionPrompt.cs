using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Aviso discreto de "dá pra fazer alguma coisa aqui": só a tecla E, flutuando
// em cima do que está ao alcance. Sem texto: o que acontece o jogador descobre.
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

    // Folga entre o topo do objeto e a tecla, em unidades do canvas (640x360).
    private const float FolgaAcima = 6f;
    // Sobe e desce 1 px, em degraus, como a seta de um JRPG antigo.
    private const float PeriodoBalanço = 0.9f;

    private CanvasGroup group;
    private RectTransform canvasRect;
    private RectTransform teclaRect;
    private Interactable alvoAtual;

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
        canvasRect = canvas.GetComponent<RectTransform>();

        group = canvas.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.blocksRaycasts = false;

        GameObject go = new GameObject("TeclaE", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(canvas.transform, false);
        Image tecla = go.GetComponent<Image>();
        tecla.sprite = UIFactory.LoadSprite("TeclaE");
        tecla.raycastTarget = false;
        tecla.SetNativeSize();

        teclaRect = tecla.rectTransform;
        teclaRect.anchorMin = teclaRect.anchorMax = new Vector2(0.5f, 0.5f);
        teclaRect.pivot = new Vector2(0.5f, 0f);
    }

    public void SetTarget(Interactable alvo)
    {
        bool mostrar = alvo != null && !GameMessageUI.IsShowing;
        if (mostrar) alvoAtual = alvo;
        alvoAlpha = mostrar ? 1f : 0f;
    }

    private void LateUpdate()
    {
        group.alpha = Mathf.MoveTowards(group.alpha, alvoAlpha, velocidadeFade * Time.deltaTime);

        if (alvoAtual == null || group.alpha <= 0f) return;
        Posicionar(alvoAtual);
    }

    private void Posicionar(Interactable alvo)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        // Topo do sprite do objeto; sem sprite, um pouco acima do pivô.
        Vector3 topo = alvo.transform.position + Vector3.up;
        Renderer r = alvo.GetComponentInChildren<Renderer>();
        if (r != null) topo = new Vector3(r.bounds.center.x, r.bounds.max.y, 0f);

        Vector2 tela = cam.WorldToScreenPoint(topo);
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, tela, null, out Vector2 local)) return;

        float balanço = Mathf.Repeat(Time.unscaledTime, PeriodoBalanço) < PeriodoBalanço * 0.5f ? 0f : 1f;
        local.y += FolgaAcima + balanço;

        // Pixel inteiro, para a tecla não borrar entre dois pixels.
        teclaRect.anchoredPosition = new Vector2(Mathf.Round(local.x), Mathf.Round(local.y));
    }
}

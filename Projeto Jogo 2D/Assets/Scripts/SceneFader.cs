using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Transição entre cenas no clima do jogo: a tela pisca como uma lâmpada
// falhando, apaga por completo, e a cena seguinte nasce lentamente do escuro.
// O objeto se cria sozinho no boot — não precisa existir em nenhuma cena.
public class SceneFader : MonoBehaviour
{
    private static SceneFader instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null)
        {
            return;
        }

        GameObject go = new GameObject("SceneFader");
        instance = go.AddComponent<SceneFader>();
        DontDestroyOnLoad(go);
    }

    public static SceneFader Instance
    {
        get
        {
            if (instance == null)
            {
                Bootstrap();
            }

            return instance;
        }
    }

    private CanvasGroup group;
    private bool isTransitioning;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        BuildOverlay();

        // A primeira cena também nasce do escuro.
        group.alpha = 1f;
        StartCoroutine(FadeIn());

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void BuildOverlay()
    {
        GameObject canvasObject = new GameObject("FadeCanvas");
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        group = canvasObject.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;

        GameObject imageObject = new GameObject("Black");
        imageObject.transform.SetParent(canvasObject.transform, false);

        Image image = imageObject.AddComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = false;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    public void LoadScene(string sceneName)
    {
        if (isTransitioning)
        {
            return;
        }

        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        isTransitioning = true;
        group.blocksRaycasts = true;

        // Falha de energia: três piscadas curtas e irregulares.
        yield return Flicker(0.85f, 0.05f);
        yield return Flicker(0.1f, 0.09f);
        yield return Flicker(0.95f, 0.04f);
        yield return Flicker(0.15f, 0.13f);
        yield return Flicker(0.7f, 0.06f);

        yield return FadeTo(1f, 0.7f);
        yield return new WaitForSecondsRealtime(0.35f);

        isTransitioning = false;
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeIn()
    {
        group.blocksRaycasts = true;
        yield return new WaitForSecondsRealtime(0.15f);
        yield return FadeTo(0f, 1f);
        group.blocksRaycasts = false;
    }

    private IEnumerator Flicker(float alpha, float duration)
    {
        group.alpha = alpha;
        yield return new WaitForSecondsRealtime(duration);
    }

    private IEnumerator FadeTo(float target, float duration)
    {
        float start = group.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        group.alpha = target;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Transição entre cenas e entre zonas/interiores no clima do jogo:
// a tela pisca como uma lâmpada falhando, apaga por completo, e a cena/zona
// seguinte nasce lentamente do escuro com um letreiro discreto da região.
public class SceneFader : MonoBehaviour
{
    private static SceneFader instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatic()
    {
        instance = null;
        SincronizarCenasNoEditor();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        SincronizarCenasNoEditor();

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
    private Image fadeImage;
    private Material sinucaIris;
    private CanvasGroup bannerGroup;
    private TextMeshProUGUI bannerTitulo;
    private TextMeshProUGUI bannerSubtitulo;
    private bool isTransitioning;
    public static bool IsTransitioning => instance != null && instance.isTransitioning;
    private Coroutine bannerRoutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        BuildOverlay();

        // A primeira cena também nasce do escuro.
        group.alpha = 1f;
        StartCoroutine(FadeIn());

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (sinucaIris != null) Destroy(sinucaIris);
        if (instance == this)
        {
            instance = null;
        }
    }

    private void BuildOverlay()
    {
        GameObject canvasObject = new GameObject("FadeCanvas");
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;
        canvasObject.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.matchWidthOrHeight = 0.5f;

        group = canvasObject.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;

        GameObject imageObject = new GameObject("Black");
        imageObject.transform.SetParent(canvasObject.transform, false);

        fadeImage = imageObject.AddComponent<Image>();
        fadeImage.color = Color.black;
        fadeImage.raycastTarget = false;

        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // Letreiro cinematográfico de Zona / Interior (estilo OMORI / Darkwood / Pokémon)
        GameObject bannerCanvasObj = new GameObject("AreaBannerCanvas");
        bannerCanvasObj.transform.SetParent(transform, false);

        Canvas bannerCanvas = bannerCanvasObj.AddComponent<Canvas>();
        bannerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        bannerCanvas.sortingOrder = 9500;

        CanvasScaler bScaler = bannerCanvasObj.AddComponent<CanvasScaler>();
        bScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        bScaler.referenceResolution = new Vector2(1280f, 720f);
        bScaler.matchWidthOrHeight = 0.5f;

        bannerGroup = bannerCanvasObj.AddComponent<CanvasGroup>();
        bannerGroup.alpha = 0f;
        bannerGroup.blocksRaycasts = false;
        bannerGroup.interactable = false;

        GameObject card = new GameObject("AreaCard", typeof(RectTransform), typeof(Image));
        card.transform.SetParent(bannerCanvasObj.transform, false);
        RectTransform cardRt = (RectTransform)card.transform;
        cardRt.anchorMin = new Vector2(0.5f, 1f);
        cardRt.anchorMax = new Vector2(0.5f, 1f);
        cardRt.pivot = new Vector2(0.5f, 1f);
        cardRt.anchoredPosition = new Vector2(0f, -22f);
        cardRt.sizeDelta = new Vector2(520f, 64f);

        Image cardImg = card.GetComponent<Image>();
        cardImg.color = new Color(0.04f, 0.04f, 0.06f, 0.88f);
        cardImg.raycastTarget = false;

        GameObject lineTop = new GameObject("LineTop", typeof(RectTransform), typeof(Image));
        lineTop.transform.SetParent(cardRt, false);
        RectTransform ltRt = (RectTransform)lineTop.transform;
        ltRt.anchorMin = new Vector2(0.05f, 0f);
        ltRt.anchorMax = new Vector2(0.95f, 0f);
        ltRt.pivot = new Vector2(0.5f, 0f);
        ltRt.anchoredPosition = new Vector2(0f, 4f);
        ltRt.sizeDelta = new Vector2(0f, 2f);
        lineTop.GetComponent<Image>().color = new Color(0.76f, 0.62f, 0.36f, 0.75f);

        bannerTitulo = UIFactory.CreateLabel(cardRt, "BannerTitulo", 22f);
        bannerTitulo.alignment = TextAlignmentOptions.Center;
        bannerTitulo.color = new Color(0.96f, 0.91f, 0.78f, 1f);
        bannerTitulo.rectTransform.anchorMin = new Vector2(0f, 0.38f);
        bannerTitulo.rectTransform.anchorMax = new Vector2(1f, 0.95f);
        bannerTitulo.rectTransform.offsetMin = new Vector2(12f, 0f);
        bannerTitulo.rectTransform.offsetMax = new Vector2(-12f, 0f);

        bannerSubtitulo = UIFactory.CreateLabel(cardRt, "BannerSubtitulo", 14f);
        bannerSubtitulo.alignment = TextAlignmentOptions.Center;
        bannerSubtitulo.color = new Color(0.75f, 0.70f, 0.60f, 0.95f);
        bannerSubtitulo.rectTransform.anchorMin = new Vector2(0f, 0.06f);
        bannerSubtitulo.rectTransform.anchorMax = new Vector2(1f, 0.42f);
        bannerSubtitulo.rectTransform.offsetMin = new Vector2(12f, 0f);
        bannerSubtitulo.rectTransform.offsetMax = new Vector2(-12f, 0f);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isTransitioning = false;
        StopAllCoroutines();
        RestaurarImagemFade();
        StartCoroutine(FadeIn());
    }

    internal void TransicionarParaSinuca(Vector2 foco, Action abrirSala, Action<float> olharPendente, Action mostrarEscolha)
    {
        if (isTransitioning) return;
        StartCoroutine(EntradaSinuca(foco, abrirSala, olharPendente, mostrarEscolha));
    }

    private IEnumerator EntradaSinuca(Vector2 foco, Action abrirSala, Action<float> olharPendente, Action mostrarEscolha)
    {
        isTransitioning = true;
        group.blocksRaycasts = true;
        fadeImage.raycastTarget = true;

        if (!MenuPausa.ReduzirEfeitosIntensos && sinucaIris == null)
        {
            var shader = Resources.Load<Shader>("Art/Sinuca/EntradaSinuca");
            if (shader != null && shader.isSupported)
            {
                sinucaIris = new Material(shader);
                sinucaIris.SetTexture("_Grao", SantaRitaBarLayout.NativeSurface(false).texture);
            }
        }
        bool cinematica = !MenuPausa.ReduzirEfeitosIntensos && sinucaIris != null;
        Vector2 centro = Vector2.one * .5f;
        if (!cinematica)
            yield return FadeTo(1f, .45f);
        else
        {
            fadeImage.material = sinucaIris;
            fadeImage.color = Color.white;
            group.alpha = 1f;
            const float fechar = 1.35f;
            for (float t = 0f; t < fechar; t += Time.unscaledDeltaTime)
            {
                float alinhar = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(1.17f, fechar, t));
                AjustarIrisSinuca(Vector2.Lerp(foco, centro, alinhar), Mathf.SmoothStep(0f, 1f, t / fechar));
                yield return null;
            }
            AjustarIrisSinuca(centro, 1f);
        }

        bool abriu = false;
        try
        {
            abrirSala();
            if (cinematica) olharPendente(1f);
            abriu = true;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        // The lamp stays at the centre of the iris across the cut between cameras.
        yield return null;
        if (cinematica)
        {
            for (float t = 0f; t < 2f; t += Time.unscaledDeltaTime)
            {
                AjustarIrisSinuca(centro, 1f - Mathf.SmoothStep(0f, 1f, t / 1.15f));
                if (abriu) olharPendente(1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(.64f, 2f, t)));
                yield return null;
            }
            AjustarIrisSinuca(centro, 0f);
            if (abriu) olharPendente(0f);
            group.alpha = 0f;
        }
        else
            yield return FadeTo(0f, .56f);
        if (abriu) yield return new WaitForSecondsRealtime(.3f);
        RestaurarImagemFade();
        isTransitioning = false;
        if (abriu) mostrarEscolha();
    }

    private void AjustarIrisSinuca(Vector2 foco, float progresso)
    {
        float aspecto = (float)Screen.width / Mathf.Max(1, Screen.height);
        float alcance = new Vector2(Mathf.Max(foco.x, 1f - foco.x) * aspecto, Mathf.Max(foco.y, 1f - foco.y)).magnitude;
        sinucaIris.SetVector("_Foco", new Vector4(foco.x, foco.y, aspecto, alcance));
        sinucaIris.SetFloat("_Progresso", progresso);
    }

    private void RestaurarImagemFade()
    {
        fadeImage.material = null;
        fadeImage.color = Color.black;
        fadeImage.raycastTarget = false;
        group.blocksRaycasts = false;
    }

    public void LoadScene(string sceneName)
    {
        if (isTransitioning)
        {
            return;
        }

        StartCoroutine(TransitionRoutine(sceneName));
    }

    // Retomada pelo orelhão: imagem do aparelho -> preto -> cena salva,
    // sem piscar ou voltar ao enquadramento do menu antes de carregar.
    public bool LoadSceneSuave(string sceneName)
    {
        if (isTransitioning) return false;
        StartCoroutine(TransitionSuaveRoutine(sceneName));
        return true;
    }

    private IEnumerator TransitionSuaveRoutine(string sceneName)
    {
        isTransitioning = true;
        group.blocksRaycasts = true;
        yield return FadeTo(1, .28f);
        yield return null;
        isTransitioning = false;
        CarregarCenaSegura(sceneName);
    }

    /// <summary>
    /// Corta a cena com efeito de fade (estilo Pokémon FireRed / OMORI / Corpse Party)
    /// para transicionar entre diferentes regiões/quadrados da cidade ou entrar/sair de interiores.
    /// </summary>
    public void TransicionarParaZona(Action acaoNoEscuro, string tituloArea = null, string subtituloArea = null)
    {
        if (isTransitioning)
        {
            acaoNoEscuro?.Invoke();
            if (!string.IsNullOrEmpty(tituloArea))
            {
                MostrarBannerArea(tituloArea, subtituloArea);
            }
            return;
        }

        StartCoroutine(ZoneTransitionRoutine(acaoNoEscuro, tituloArea, subtituloArea));
    }

    public void MostrarBannerArea(string titulo, string subtitulo = "")
    {
        if (bannerGroup == null || bannerTitulo == null)
        {
            return;
        }

        bannerTitulo.text = titulo ?? "";
        bannerSubtitulo.text = subtitulo ?? "";

        if (bannerRoutine != null)
        {
            StopCoroutine(bannerRoutine);
        }
        bannerRoutine = StartCoroutine(BannerRoutine());
    }

    private IEnumerator BannerRoutine()
    {
        float t = 0f;
        while (t < 0.35f)
        {
            t += Time.unscaledDeltaTime;
            bannerGroup.alpha = Mathf.Clamp01(t / 0.35f);
            yield return null;
        }
        bannerGroup.alpha = 1f;

        yield return new WaitForSecondsRealtime(2.4f);

        t = 0f;
        while (t < 0.6f)
        {
            t += Time.unscaledDeltaTime;
            bannerGroup.alpha = 1f - Mathf.Clamp01(t / 0.6f);
            yield return null;
        }
        bannerGroup.alpha = 0f;
        bannerRoutine = null;
    }

    private IEnumerator ZoneTransitionRoutine(Action acaoNoEscuro, string tituloArea, string subtituloArea)
    {
        isTransitioning = true;
        group.blocksRaycasts = true;

        yield return Flicker(0.65f, 0.04f);
        yield return Flicker(0.20f, 0.05f);
        yield return FadeTo(1f, 0.28f);

        try
        {
            acaoNoEscuro?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.LogError("Erro durante transição de zona: " + ex);
        }

        yield return new WaitForSecondsRealtime(0.18f);
        yield return FadeTo(0f, 0.42f);

        group.blocksRaycasts = false;
        isTransitioning = false;

        if (!string.IsNullOrEmpty(tituloArea))
        {
            MostrarBannerArea(tituloArea, subtituloArea);
        }
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        isTransitioning = true;
        group.blocksRaycasts = true;

        // Falha de energia: piscadas curtas e irregulares.
        yield return Flicker(0.85f, 0.05f);
        yield return Flicker(0.1f, 0.09f);
        yield return Flicker(0.95f, 0.04f);
        yield return Flicker(0.15f, 0.13f);
        yield return Flicker(0.7f, 0.06f);

        yield return FadeTo(1f, 0.7f);
        yield return new WaitForSecondsRealtime(0.35f);

        isTransitioning = false;
        CarregarCenaSegura(sceneName);

        // Garantia contra tela preta caso a cena não dispare OnSceneLoaded no mesmo frame
        yield return null;
        yield return null;
        if (group != null && group.alpha > 0.01f && !isTransitioning)
        {
            StartCoroutine(FadeIn());
        }
    }

    private static string ResolverCaminhoCena(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            return null;
        }

        if (sceneName.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
        {
            return sceneName;
        }

        string[] candidatos =
        {
            "Assets/Scenes/Cidade/" + sceneName + ".unity",
            "Assets/Scenes/" + sceneName + ".unity"
        };

        string rootProjeto = Directory.GetParent(Application.dataPath)?.FullName ?? "";
        for (int i = 0; i < candidatos.Length; i++)
        {
            string full = Path.Combine(rootProjeto, candidatos[i]);
            if (File.Exists(full))
            {
                return candidatos[i];
            }
        }

        return null;
    }

    private static void SincronizarCenasNoEditor()
    {
#if UNITY_EDITOR
        if (!Application.isEditor)
        {
            return;
        }

        try
        {
            Type ebsType = Type.GetType("UnityEditor.EditorBuildSettings, UnityEditor.CoreModule")
                        ?? Type.GetType("UnityEditor.EditorBuildSettings, UnityEditor");
            Type sceneType = Type.GetType("UnityEditor.EditorBuildSettingsScene, UnityEditor.CoreModule")
                          ?? Type.GetType("UnityEditor.EditorBuildSettingsScene, UnityEditor");

            if (ebsType == null || sceneType == null)
            {
                return;
            }

            string[] paths =
            {
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/Prologo.unity",
                "Assets/Scenes/Cidade/EntradaDaCidade.unity",
                "Assets/Scenes/Cidade/CasaDoAvo.unity",
                "Assets/Scenes/Cidade/BarDoZe.unity",
                "Assets/Scenes/GameplayScene.unity",
                "Assets/Scenes/Cidade/CentroSantaRita.unity",
                "Assets/Scenes/Cidade/IgrejaMatriz.unity",
                "Assets/Scenes/Cidade/PrefeituraSantaRita.unity",
                "Assets/Scenes/Cidade/RodoviariaSantaRita.unity",
                "Assets/Scenes/Cidade/MercadinhoDeposito.unity",
                "Assets/Scenes/Cidade/OficinaRadio.unity",
                "Assets/Scenes/Cidade/CasaCostureira.unity",
                "Assets/Scenes/Cidade/SociedadeBeiraRio.unity",
                "Assets/Scenes/Cidade/AbrigoCemiterio.unity",
                "Assets/Scenes/Cidade/CemiterioSantaRita.unity"
            };

            string rootProjeto = Directory.GetParent(Application.dataPath)?.FullName ?? "";
            var validPaths = new List<string>();
            for (int i = 0; i < paths.Length; i++)
            {
                if (File.Exists(Path.Combine(rootProjeto, paths[i])))
                {
                    validPaths.Add(paths[i]);
                }
            }

            Array arr = Array.CreateInstance(sceneType, validPaths.Count);
            ConstructorInfo ctor = sceneType.GetConstructor(new[] { typeof(string), typeof(bool) });
            if (ctor == null)
            {
                return;
            }

            for (int i = 0; i < validPaths.Count; i++)
            {
                arr.SetValue(ctor.Invoke(new object[] { validPaths[i], true }), i);
            }

            PropertyInfo propScenes = ebsType.GetProperty("scenes", BindingFlags.Public | BindingFlags.Static);
            // A sincronização antiga reescrevia a lista inteira em cada Play e removia o Cemitério.
            // Acrescentar os destinos necessários preserva também cenas adicionais configuradas no Editor.
            var atual=propScenes?.GetValue(null,null) as Array;
            var merge=new List<object>();
            if(atual!=null)foreach(var item in atual)
            {
                string path=sceneType.GetProperty("path").GetValue(item,null) as string;
                if(!validPaths.Contains(path))merge.Add(item);
            }
            foreach(var item in arr)merge.Add(item);
            var final=Array.CreateInstance(sceneType,merge.Count);for(int i=0;i<merge.Count;i++)final.SetValue(merge[i],i);
            propScenes?.SetValue(null, final, null);
        }
        catch
        {
            // Ignora silenciosamente fora do Editor
        }
#endif
    }

    public static void CarregarCenaSegura(string sceneName)
    {
        SincronizarCenasNoEditor();

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        string caminhoCena = ResolverCaminhoCena(sceneName);
        if (!string.IsNullOrEmpty(caminhoCena) && Application.CanStreamedLevelBeLoaded(caminhoCena))
        {
            SceneManager.LoadScene(caminhoCena);
            return;
        }

        // Fallback infalível no Unity Editor: EditorSceneManager.LoadSceneInPlayMode carrega qualquer .unity
        // mesmo que o Build Profile ativo do Unity 6 não tenha recarregado a lista de cenas.
        if (Application.isEditor && !string.IsNullOrEmpty(caminhoCena))
        {
            try
            {
                Type esmType = Type.GetType("UnityEditor.SceneManagement.EditorSceneManager, UnityEditor.CoreModule")
                            ?? Type.GetType("UnityEditor.SceneManagement.EditorSceneManager, UnityEditor");
                if (esmType != null)
                {
                    MethodInfo loadInPlayMode = esmType.GetMethod(
                        "LoadSceneInPlayMode",
                        BindingFlags.Public | BindingFlags.Static,
                        null,
                        new[] { typeof(string), typeof(LoadSceneParameters) },
                        null
                    );

                    if (loadInPlayMode != null)
                    {
                        loadInPlayMode.Invoke(null, new object[]
                        {
                            caminhoCena,
                            new LoadSceneParameters(LoadSceneMode.Single)
                        });
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Fallback LoadSceneInPlayMode falhou: " + ex.Message);
            }
        }

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeIn()
    {
        group.blocksRaycasts = true;
        yield return new WaitForSecondsRealtime(0.15f);
        yield return FadeTo(0f, 0.85f);
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

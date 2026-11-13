using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public static class SantaRitaWorldLighting
{
    internal const string StockSpriteShader = "Universal Render Pipeline/2D/Sprite-Lit-Default";
    private const string StockMeshShader = "Universal Render Pipeline/2D/Mesh2D-Lit-Default";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Install()
    {
        SceneManager.sceneLoaded -= ApplyToScene;
        RenderPipelineManager.beginCameraRendering -= EnablePostProcessing;
        RenderPipelineManager.beginCameraRendering += EnablePostProcessing;
        SceneManager.sceneLoaded += ApplyToScene;
    }
    private static void EnablePostProcessing(ScriptableRenderContext context, Camera camera)
    {
        if (camera.cameraType != CameraType.Game || camera.targetTexture != null) return;

        if (camera.TryGetComponent<UniversalAdditionalCameraData>(out var data)
            && data.renderType == CameraRenderType.Base && !data.renderPostProcessing)
            data.renderPostProcessing = true;
    }


    private static void ApplyToScene(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CentroSantaRita")
        {
            DesativarSombraLegadaDaIgreja(scene);
            ApplyCentroCinematicVolume(scene);
        }

        var material = Resources.Load<Material>("Art/WorldLit");
        if (material == null || scene.name is "MainMenu" or "CentroSantaRita") return;

        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var sprite in root.GetComponentsInChildren<SpriteRenderer>(true))
                ReplaceStockMaterial(sprite, material);
            foreach (var tilemap in root.GetComponentsInChildren<TilemapRenderer>(true))
                ReplaceStockMaterial(tilemap, material);
        }
    }

    private static void ApplyCentroCinematicVolume(Scene scene)
    {
        var profile = Resources.Load<VolumeProfile>("Art/Producao/CentroSantaRitaCinematic");
        if (profile == null) return;
        ConfigurarPerfilDaMatriz(profile);

        Volume volume = null;
        Volume globalVolume = null;
        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var candidate in root.GetComponentsInChildren<Volume>(true))
            {
                if (candidate.isGlobal && globalVolume == null)
                    globalVolume = candidate;
                if (candidate.name == "Foco_Cinematico_Igreja")
                    volume = candidate;
            }
        }

        if (globalVolume != null && globalVolume.sharedProfile != null)
            ConfigurarPerfilGlobalCentro(globalVolume.sharedProfile);

        if (volume == null)
        {
            var go = new GameObject("Foco_Cinematico_Igreja");
            SceneManager.MoveGameObjectToScene(go, scene);
            go.transform.position = new Vector3(46f, 63.5f, 0f);
            var collider = go.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(20f, 16f, 10f);
            volume = go.AddComponent<Volume>();
            volume.isGlobal = false;
            volume.priority = 3f;
            volume.blendDistance = 6f;
            volume.colliders.Add(collider);
        }

        volume.sharedProfile = profile;
    }

    private static void ConfigurarPerfilGlobalCentro(VolumeProfile profile) =>
        ConfigurarPerfilCentroSuave(profile, -.10f, .22f, .16f);

    private static void ConfigurarPerfilDaMatriz(VolumeProfile profile) =>
        ConfigurarPerfilCentroSuave(profile, -.08f, .26f, .15f);

    private static void ConfigurarPerfilCentroSuave(VolumeProfile profile, float exposicao,
        float intensidadeBloom, float intensidadeVignette)
    {
        if (profile.TryGet<Tonemapping>(out var tonemapping)) tonemapping.active = false;
        if (profile.TryGet<SplitToning>(out var split)) split.active = false;
        if (profile.TryGet<ShadowsMidtonesHighlights>(out var smh)) smh.active = false;
        if (profile.TryGet<ChromaticAberration>(out var chroma)) chroma.active = false;
        if (profile.TryGet<FilmGrain>(out var grain)) grain.active = false;

        if (!profile.TryGet<ColorAdjustments>(out var color)) color = profile.Add<ColorAdjustments>(true);
        color.active = true;
        color.postExposure.Override(exposicao);
        color.contrast.Override(10f);
        color.colorFilter.Override(Color.white);
        color.saturation.Override(-8f);

        if (!profile.TryGet<Bloom>(out var bloom)) bloom = profile.Add<Bloom>(true);
        bloom.active = true;
        bloom.threshold.Override(.90f);
        bloom.intensity.Override(intensidadeBloom);
        bloom.scatter.Override(.52f);
        bloom.tint.Override(new Color(1f, .93f, .84f, 1f));

        if (!profile.TryGet<Vignette>(out var vignette)) vignette = profile.Add<Vignette>(true);
        vignette.active = true;
        vignette.color.Override(Color.black);
        vignette.center.Override(new Vector2(.5f, .5f));
        vignette.intensity.Override(intensidadeVignette);
        vignette.smoothness.Override(.56f);
        vignette.rounded.Override(false);
    }

    private static void DesativarSombraLegadaDaIgreja(Scene scene)
    {
        foreach (var root in scene.GetRootGameObjects())
        foreach (var child in root.GetComponentsInChildren<Transform>(true))
            if (child.name == "AO_Igreja") child.gameObject.SetActive(false);
    }

    private static void ReplaceStockMaterial(Renderer renderer, Material material)
    {
        if (renderer.name.Contains("Lanterna")) return;
        var shader = renderer.sharedMaterial?.shader;
        if (shader != null && (shader.name == StockSpriteShader || shader.name == StockMeshShader))
            renderer.sharedMaterial = material;
    }

    public static void Apply()
    {
        var material = Resources.Load<Material>("Art/WorldLit");
        if (material == null) return;

        foreach (var sprite in Object.FindObjectsByType<SpriteRenderer>())
        {
            if (sprite.name.Contains("Lanterna") || SantaRitaRuaNorte.ControlaRenderDaRua(sprite)) continue;
            sprite.sharedMaterial = material;
        }

        foreach (var tilemap in Object.FindObjectsByType<TilemapRenderer>())
            ReplaceStockMaterial(tilemap, material);

        var player = Object.FindAnyObjectByType<PlayerController>();
        if (player == null) return;
        if (player.GetComponent<SantaRitaRimLight>() == null) player.gameObject.AddComponent<SantaRitaRimLight>();
        if (player.GetComponent<SantaRitaAtmosphere>() == null) player.gameObject.AddComponent<SantaRitaAtmosphere>();
    }
}

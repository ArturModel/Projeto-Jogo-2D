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
        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var candidate in root.GetComponentsInChildren<Volume>(true))
            {
                if (candidate.name == "Foco_Cinematico_Igreja")
                {
                    volume = candidate;
                    break;
                }
            }
            if (volume != null) break;
        }

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

    private static void ConfigurarPerfilDaMatriz(VolumeProfile profile)
    {
        if (!profile.TryGet<Bloom>(out var bloom)) bloom = profile.Add<Bloom>(true);
        bloom.active = true;
        bloom.threshold.Override(1.05f);
        bloom.intensity.Override(.36f);
        bloom.scatter.Override(.65f);
        bloom.tint.Override(new Color(1f, .88f, .74f, 1f));

        // ACES was crushing the mids/shadows as the player entered this local volume.
        // Keep the church focus luminous without changing the plaza's overall exposure curve.
        if (profile.TryGet<Tonemapping>(out var tonemapping)) tonemapping.active = false;

        if (!profile.TryGet<ColorAdjustments>(out var color)) color = profile.Add<ColorAdjustments>(true);
        color.active = true;
        color.postExposure.Override(.20f);
        color.contrast.Override(-2f);
        color.saturation.Override(-4f);

        if (!profile.TryGet<Vignette>(out var vignette)) vignette = profile.Add<Vignette>(true);
        vignette.active = false;
        vignette.intensity.Override(0f);
        vignette.smoothness.Override(.62f);
        vignette.center.Override(new Vector2(.5f, .53f));

        if (!profile.TryGet<FilmGrain>(out var grain)) grain = profile.Add<FilmGrain>(true);
        grain.active = true;
        grain.type.Override(FilmGrainLookup.Medium1);
        grain.intensity.Override(.025f);
        grain.response.Override(.78f);

        if (!profile.TryGet<SplitToning>(out var split)) split = profile.Add<SplitToning>(true);
        split.active = false;
        split.shadows.Override(new Color(.46f, .49f, .54f, 1f));
        split.highlights.Override(new Color(1f, .84f, .68f, 1f));
        split.balance.Override(12f);

        if (!profile.TryGet<ChromaticAberration>(out var chroma)) chroma = profile.Add<ChromaticAberration>(true);
        chroma.active = true;
        chroma.intensity.Override(0f);
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

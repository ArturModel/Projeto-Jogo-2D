using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public static class SantaRitaWorldLighting
{
    private const string StockSpriteShader = "Universal Render Pipeline/2D/Sprite-Lit-Default";
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
        var material = Resources.Load<Material>("Art/WorldLit");
        if (material == null || scene.name == "MainMenu") return;

        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var sprite in root.GetComponentsInChildren<SpriteRenderer>(true))
                ReplaceStockMaterial(sprite, material);
            foreach (var tilemap in root.GetComponentsInChildren<TilemapRenderer>(true))
                ReplaceStockMaterial(tilemap, material);
        }
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

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public static class SinucaRef7Authoring
{
    [MenuItem("Santa Rita/Sinuca/Preparar arte e renderer Ref7")]
    public static void Prepare()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Encerrar Play antes da autoria.");
        foreach(string path in Directory.GetFiles("Assets/Resources/Art/BarRef7","*.png"))
        {
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);if(importer==null)continue;
            importer.textureType=TextureImporterType.Default;importer.isReadable=true;importer.mipmapEnabled=false;importer.filterMode=FilterMode.Point;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=4096;importer.alphaIsTransparency=true;importer.SaveAndReimport();
        }
        const string path3D="Assets/Settings/SinucaRenderer3D.asset";
        var renderer=AssetDatabase.LoadAssetAtPath<UniversalRendererData>(path3D);
        if(renderer==null){renderer=ScriptableObject.CreateInstance<UniversalRendererData>();AssetDatabase.CreateAsset(renderer,path3D);}
        foreach(string id in AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset"))
        {
            var pipeline=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(AssetDatabase.GUIDToAssetPath(id));var so=new SerializedObject(pipeline);var list=so.FindProperty("m_RendererDataList");if(list.arraySize<2)list.arraySize=2;list.GetArrayElementAtIndex(1).objectReferenceValue=renderer;so.ApplyModifiedPropertiesWithoutUndo();EditorUtility.SetDirty(pipeline);
        }
        foreach(string name in new[]{"Simple Lit","Unlit"})
        {
            string asset="Assets/Resources/Art/BarRef7/"+(name=="Unlit"?"BarUnlit":"BarLit")+".mat";
            var m=AssetDatabase.LoadAssetAtPath<Material>(asset);if(m==null){m=new Material(Shader.Find("Universal Render Pipeline/"+name));AssetDatabase.CreateAsset(m,asset);}m.SetFloat("_Smoothness",.15f);EditorUtility.SetDirty(m);
        }
        var pixel=AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Art/BarRef7/BarPixelCutout.mat");
        if(pixel==null){pixel=new Material(Shader.Find("Universal Render Pipeline/Unlit"));AssetDatabase.CreateAsset(pixel,"Assets/Resources/Art/BarRef7/BarPixelCutout.mat");}
        pixel.SetFloat("_AlphaClip",1);pixel.EnableKeyword("_ALPHATEST_ON");pixel.SetFloat("_Cutoff",.2f);pixel.SetFloat("_Cull",0);pixel.renderQueue=2450;EditorUtility.SetDirty(pixel);
        AssetDatabase.SaveAssets();Debug.Log("Sinuca Ref7: 3D em índice 1, Renderer2D original em índice 0.");
    }
}
// Explicit isolated preview; never attached to a campaign scene or built into Player.
public sealed class SinucaPixelPreviewSession : MonoBehaviour
{
    private IDisposable isolation;
    public static void Run(){var g=new GameObject("AutoriaRef7Explicita");DontDestroyOnLoad(g);g.AddComponent<SinucaPixelPreviewSession>();}
    private System.Collections.IEnumerator Start()
    {
        isolation=CheckpointSystem.BeginIsolatedTests();SceneFader.Instance.LoadScene("EntradaDaCidade");
        yield return new WaitForSecondsRealtime(3);
        while(GameMessageUI.IsShowing){GameMessageUI.Instance.Avancar();yield return null;}
        Capitulo1Director.Instance.TransicionarPara(Capitulo1Director.ZonaId.Interior_BarDoZe,new Vector2(509,-54.8f),1);
        yield return new WaitForSecondsRealtime(2);
        while(GameMessageUI.IsShowing){GameMessageUI.Instance.Avancar();yield return null;}
        SinucaMinigameUI.Instance.IniciarPraticaOpcional();yield return new WaitForSecondsRealtime(2);
        SinucaMinigameUI.Instance.Fechar();yield return null;
        ScreenCapture.CaptureScreenshot(Path.GetFullPath(Path.Combine(Application.dataPath,"../../Docs/Evidencias/BarIdentidade2D/bar-2d.png")));yield return new WaitForEndOfFrame();
        SinucaMinigameUI.Instance.IniciarPraticaOpcional();yield return new WaitForSecondsRealtime(1);
        ScreenCapture.CaptureScreenshot(Path.GetFullPath(Path.Combine(Application.dataPath,"../../Docs/Evidencias/BarIdentidade2D/sinuca-3d.png")));yield return new WaitForEndOfFrame();
        yield return null;
        SinucaMinigameUI.Instance.Fechar();isolation.Dispose();isolation=null;EditorApplication.isPlaying=false;
    }
    private void OnDestroy(){if(isolation!=null){isolation.Dispose();isolation=null;}}
}

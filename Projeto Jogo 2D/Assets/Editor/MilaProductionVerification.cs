using System;
using System.IO;
using UnityEngine;
using UnityEditor;
public static class MilaProductionVerification
{
    public static void EditorQA()
    {
        Prepare();SessionState.SetBool("MilaQA.EnterPlayOptions",EditorSettings.enterPlayModeOptionsEnabled);
        EditorSettings.enterPlayModeOptionsEnabled=false;
        EditorApplication.delayCall+=()=>{UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");EditorApplication.isPlaying=true;};
    }
    public static void BuildQA(){Prepare();Chapter2BuildVerification.ForcarDevelopment=true;Chapter2BuildVerification.Run();}
    public static void BuildNormal(){Prepare();Chapter2BuildVerification.ForcarDevelopment=false;Chapter2BuildVerification.Run();}
    private static void Prepare()
    {
        foreach(string id in new[]{"Cassette","Recorder","Letter"}){string p="Assets/Resources/Tapes/"+id+".png";AssetDatabase.ImportAsset(p);var t=(TextureImporter)AssetImporter.GetAtPath(p);t.filterMode=FilterMode.Point;t.mipmapEnabled=false;t.alphaIsTransparency=true;t.textureCompression=TextureImporterCompression.Uncompressed;t.SaveAndReimport();}
        AssetDatabase.SaveAssets();
    }
}

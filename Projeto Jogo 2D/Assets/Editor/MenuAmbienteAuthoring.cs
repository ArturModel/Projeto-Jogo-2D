using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public sealed class MenuAmbienteTextureImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith("Assets/Resources/MenuAmbiente/", StringComparison.Ordinal)) return;
        var t = (TextureImporter)assetImporter;
        t.textureType = TextureImporterType.Default;
        t.alphaIsTransparency = true;
        t.mipmapEnabled = false;
        t.filterMode = FilterMode.Point;
        t.wrapMode = TextureWrapMode.Clamp;
        t.textureCompression = TextureImporterCompression.Uncompressed;
        t.npotScale = TextureImporterNPOTScale.None;
        t.maxTextureSize = 4096;
    }
}

// Ferramenta opt-in: não abre nem salva cenas e não inicia build ao importar.
public static class MenuAmbienteAuthoring
{
    public static string Output => Environment.GetEnvironmentVariable("SANTA_RITA_MENU_QA_OUTPUT") ?? "C:/Temp/SantaRitaMenu";
    public static void BuildQA() { Build(true); }
    public static void BuildNormal() { Build(false); }

    static void Build(bool qa)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Pare o Play antes da build do menu.");
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            if (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).isDirty)
                throw new InvalidOperationException("Existe cena não salva; não gerar build.");
        foreach (var nome in new[] { "Rua", "Orelhao" })
            AssetDatabase.ImportAsset("Assets/Resources/MenuAmbiente/" + nome + ".png", ImportAssetOptions.ForceUpdate);
        Directory.CreateDirectory(Output);
        var options = new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
            locationPathName = Output + (qa ? "/QA" : "/Normal") + "/SantaRita.exe",
            target = BuildTarget.StandaloneWindows64,
            options = qa ? BuildOptions.Development : BuildOptions.None
        };
        var report = BuildPipeline.BuildPlayer(options);
        string status = report.summary.result + "; errors=" + report.summary.totalErrors + "; warnings=" + report.summary.totalWarnings;
        File.WriteAllText(Output + (qa ? "/build-qa.txt" : "/build-normal.txt"), status);
        if (report.summary.result != BuildResult.Succeeded) throw new Exception(status);
        Debug.Log("MENU BUILD " + status);
    }
}

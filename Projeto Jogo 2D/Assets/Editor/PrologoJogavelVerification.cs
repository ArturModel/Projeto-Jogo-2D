using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public sealed class PrologoTextureImporter : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        if(!assetPath.StartsWith("Assets/Resources/PrologoJogavel/",StringComparison.Ordinal))return;
        var t=(TextureImporter)assetImporter;
        t.textureType=TextureImporterType.Default; t.alphaIsTransparency=true;
        t.mipmapEnabled=false; t.filterMode=FilterMode.Point; t.wrapMode=TextureWrapMode.Clamp;
        t.textureCompression=TextureImporterCompression.Uncompressed;
        t.npotScale=TextureImporterNPOTScale.None; t.maxTextureSize=2048;
        if(assetPath.EndsWith("Asfalto.png",StringComparison.Ordinal) || assetPath.EndsWith("Terra.png",StringComparison.Ordinal) || assetPath.EndsWith("SoloVegetacao.png",StringComparison.Ordinal))
        { t.wrapMode=TextureWrapMode.Repeat; t.mipmapEnabled=true; t.filterMode=FilterMode.Bilinear; }
    }
}

public static class PrologoJogavelVerification
{
    public const string Output="C:/Temp/SantaRitaPrologo";
    public static void QueueBuildQA()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorApplication.playModeStateChanged-=DepoisDeParar;
            EditorApplication.playModeStateChanged+=DepoisDeParar;
            EditorApplication.isPlaying=false;
        }
        else EditorApplication.delayCall+=BuildQA;
    }
    private static void DepoisDeParar(PlayModeStateChange estado)
    {
        if(estado!=PlayModeStateChange.EnteredEditMode)return;
        EditorApplication.playModeStateChanged-=DepoisDeParar;
        EditorApplication.delayCall+=BuildQA;
    }
    public static void BuildQA() => Build(true);
    public static void BuildNormal() => Build(false);
    private static void Build(bool qa)
    {
        Directory.CreateDirectory(Output);
        foreach(string n in new[]{"Cabine","Volante","Horizonte","CenarioRural","Objetos","Asfalto","Terra","SoloVegetacao"})
        {
            string path="Assets/Resources/PrologoJogavel/"+n+".png";
            AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceUpdate);
            if(AssetDatabase.LoadAssetAtPath<Texture2D>(path)==null)throw new Exception("Asset ausente: "+path);
        }
        var scene=EditorSceneManager.OpenScene("Assets/Scenes/Prologo.unity",OpenSceneMode.Single);
        if(UnityEngine.Object.FindAnyObjectByType<PrologoJogavel>()==null)throw new Exception("Prólogo jogável não está ativo");
        if(UnityEngine.Object.FindAnyObjectByType<PrologoVideo>()!=null)throw new Exception("Vídeo legado ainda ativo");
        if(UnityEngine.Object.FindAnyObjectByType<PrologoDirector>()!=null)throw new Exception("Dois diretores ativos");
        Debug.Log("PROLOGO EDITOR PASS: oito texturas importadas; um diretor jogável; vídeo/diretor legados inativos.");
        string folder=Output+(qa?"/QA":"/Normal");
        var options=new BuildPlayerOptions {
            scenes=qa?new[]{"Assets/Scenes/MainMenu.unity","Assets/Scenes/Prologo.unity","Assets/Scenes/Cidade/EntradaDaCidade.unity"}:
                EditorBuildSettings.scenes.Where(s=>s.enabled).Select(s=>s.path).ToArray(),
            locationPathName=folder+"/SantaRita.exe",target=BuildTarget.StandaloneWindows64,
            options=qa?BuildOptions.Development:BuildOptions.None
        };
        var report=BuildPipeline.BuildPlayer(options);
        File.WriteAllText(Output+(qa?"/build-qa.txt":"/build-normal.txt"),report.summary.result+"; errors="+report.summary.totalErrors+"; warnings="+report.summary.totalWarnings);
        if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Build falhou: "+report.summary.result);
    }
}

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public static class SantaRitaProductionVerification
{
    [MenuItem("Santa Rita/Cidade/Compilar Player validado")]
    public static void BuildFinalLivingWorld()
    {
        if(EditorApplication.isPlaying||Enumerable.Range(0,SceneManager.sceneCount).Any(i=>SceneManager.GetSceneAt(i).isDirty))
            throw new Exception("Pare o Play e preserve as cenas não salvas antes de compilar.");
        var setup=EditorSceneManager.GetSceneManagerSetup();
        string oldDir=Environment.GetEnvironmentVariable("SR_BUILD_DIR"),oldEvidence=Environment.GetEnvironmentVariable("SR_EVIDENCE_DIR");
        bool oldDevelopment=Chapter2BuildVerification.ForcarDevelopment;
        try
        {
            Environment.SetEnvironmentVariable("SR_BUILD_DIR",Path.GetFullPath(Path.Combine(Application.dataPath,"../../Build/Capitulo2")));
            Environment.SetEnvironmentVariable("SR_EVIDENCE_DIR",Path.GetFullPath(Path.Combine(Application.dataPath,"../../Docs/Evidencias/CidadeViva/PlayerFinal")));
            Chapter2BuildVerification.ForcarDevelopment=false;Chapter2BuildVerification.Run();
        }
        finally
        {
            Chapter2BuildVerification.ForcarDevelopment=oldDevelopment;Environment.SetEnvironmentVariable("SR_BUILD_DIR",oldDir);
            Environment.SetEnvironmentVariable("SR_EVIDENCE_DIR",oldEvidence);EditorSceneManager.RestoreSceneManagerSetup(setup);
        }
    }
    [MenuItem("Santa Rita/Arte/Build QA de produção")]
    public static void BuildQA()
    {
        var setup=EditorSceneManager.GetSceneManagerSetup();
        string oldDir=Environment.GetEnvironmentVariable("SR_BUILD_DIR"),oldEvidence=Environment.GetEnvironmentVariable("SR_EVIDENCE_DIR");
        try
        {
            Environment.SetEnvironmentVariable("SR_BUILD_DIR",Path.GetFullPath(Path.Combine(Application.dataPath,"../../Build/ProducaoQA")));
            Environment.SetEnvironmentVariable("SR_EVIDENCE_DIR",Path.GetFullPath(Path.Combine(Application.dataPath,"../../Docs/Evidencias/Producao/PlayerQA")));
            Chapter2BuildVerification.ForcarDevelopment=true;Chapter2BuildVerification.Run();
        }
        finally
        {
            Chapter2BuildVerification.ForcarDevelopment=false;Environment.SetEnvironmentVariable("SR_BUILD_DIR",oldDir);
            Environment.SetEnvironmentVariable("SR_EVIDENCE_DIR",oldEvidence);EditorSceneManager.RestoreSceneManagerSetup(setup);
        }
    }
    [MenuItem("Santa Rita/Arte/Verificar produção e regressão (isolado)")]
    public static void Run()
    {
        if(EditorApplication.isPlaying)throw new Exception("Pare o Play para verificar as cenas salvas.");
        var setup=EditorSceneManager.GetSceneManagerSetup();var report=new List<string>();int count=0;
        void Check(bool value,string label){if(!value)throw new Exception(label);count++;}
        string folder=Path.GetFullPath(Path.Combine(Application.dataPath,"../../Docs/Evidencias/Producao"));Directory.CreateDirectory(folder);
        string oldEvidence=Environment.GetEnvironmentVariable("SR_EVIDENCE_DIR");
        try
        {
            foreach(string name in new[]{"tonao","bento","clarice","alcina","dario","mila","celina","nair"})
            {
                var sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Capitulo2/Npc_"+name+".png");
                Check(sprite!=null && sprite.rect.width==64 && sprite.rect.height==64,"NPC 64×64: "+name);
                Check(sprite.pixelsPerUnit==32 && Mathf.Abs(sprite.pivot.y-8)<.01f,"Escala/pés do NPC: "+name);
            }
            foreach(string name in new[]{"Asfalto","Grama","Terra"})
            {
                var tex=Resources.Load<Texture2D>("Art/Macros/"+name+"_Macro");
                Check(tex!=null&&tex.isReadable&&tex.width==128&&tex.height==128,"Macro legível na build: "+name);
            }
            foreach(string guid in AssetDatabase.FindAssets("t:AudioClip",new[]{"Assets/Resources/Audio/Foley"}))
            {
                var clip=AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(guid));
                Check(clip!=null&&clip.length>.015f&&clip.length<8,"Efeito importado: "+clip.name);
            }
            foreach(string name in SantaRitaChapter2Authoring.Scenes)
            {
                var scene=EditorSceneManager.OpenScene("Assets/Scenes/Cidade/"+name+".unity");
                var roots=scene.GetRootGameObjects();
                Check(!roots.SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).Any(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)>0),"Scripts de "+name);
                Check(!UnityEngine.Object.FindObjectsByType<TextMeshPro>().Any(t=>t.name.StartsWith("Letreiro_")),"Sem títulos flutuantes em "+name);
                Check(UnityEngine.Object.FindObjectsByType<TilemapRenderer>().All(r=>r.sharedMaterial!=null&&r.sharedMaterial.shader.name=="Universal Render Pipeline/2D/Sprite-Lit-Default"),"Piso recebe luz em "+name);
                foreach(var p in UnityEngine.Object.FindObjectsByType<Capitulo2Portal>())
                    Check(p.automatico||p.Prompt=="Abrir porta","Porta não revela destino: "+name);
                if(name=="CentroSantaRita")
                {
                    Check(GameObject.Find("Canteiro")==null,"Sem textura de cerâmica esticada em canteiros.");
                    Check(UnityEngine.Object.FindObjectsByType<ShadowCaster2D>().Length>=4,"Sombras projetadas nos edifícios.");
                    Check(GameObject.Find("VegetacaoDeBorda")!=null,"Sub-bosque integrado.");
                    var ground=GameObject.Find("Terreno").GetComponent<Tilemap>();
                    Check(ground.GetTile(new Vector3Int(31,45,0)).name.Contains("Asfalto"),"Avenida continua asfaltada.");
                    Check(GameObject.Find("Memorial_Praca")==null&&GameObject.Find("Examinar_placa_praca")==null,"Praça sem mesa/exposição dedicada a Júlia.");
                    Check(GameObject.Find("FontePublica")!=null&&GameObject.Find("QuadroAvisosDaPraca")!=null,"Praça com usos cívicos plausíveis.");
                    Check(UnityEngine.Object.FindObjectsByType<Capitulo2Portal>().Any(p=>p.cenaDestino=="CemiterioSantaRita"),"Cemitério conectado como cena independente.");
                    Check(UnityEngine.Object.FindObjectsByType<SantaRitaNpcRoutine>().Length>=3,"Moradores e jardineiro com atividades próprias.");
                }
                if(name=="CemiterioSantaRita")
                {
                    Check(!UnityEngine.Object.FindAnyObjectByType<Capitulo2Director>().interior,"Cemitério é exterior independente.");
                    Check(GameObject.Find("Porta_RuaNorte")==null,"Cemitério não herdou portal distante do Centro.");
                    Check(UnityEngine.Object.FindObjectsByType<Capitulo2Portal>().Any(p=>p.cenaDestino=="CentroSantaRita"),"Cemitério permite retorno ao Centro.");
                }
                if(name=="PrefeituraSantaRita")Check(GameObject.Find("Examinar_placa_praca")!=null,"Boletim antigo guardado no arquivo público.");
                if(name=="SociedadeBeiraRio")Check(GameObject.Find("CabeceiraDeMadeira")!=null,"Cancha possui cabeceiras visíveis no salão.");
                report.Add("PASS cena "+name);
            }
            Environment.SetEnvironmentVariable("SR_EVIDENCE_DIR",folder);
            Chapter2Verification.Run();
            report.Add("PASS "+count+" contratos de produção + regressão de regras/saves em arquivo separado.");
            report.Add("Não certifica aprovação artística, audição humana ou campanha completa sem fixture.");
            File.WriteAllLines(Path.Combine(folder,"verificacao-producao.txt"),report);Debug.Log(string.Join("\n",report));
        }
        finally { Environment.SetEnvironmentVariable("SR_EVIDENCE_DIR",oldEvidence);EditorSceneManager.RestoreSceneManagerSetup(setup); }
    }
}

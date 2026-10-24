using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;

// Comando explícito: nunca roda ao abrir o projeto ou modifica cenas aprovadas.
public static class Chapter2BuildVerification
{
    public static bool ForcarDevelopment { get; set; }
    public static string Evidence => Environment.GetEnvironmentVariable("SR_EVIDENCE_DIR") ?? Path.GetFullPath(Path.Combine(Application.dataPath,"../../Docs/Evidencias/Capitulo2"));

    // Cenas novas precisam existir também na lista persistida, além do Editor.
    // Registrar referências não reautora o conteúdo das cenas.
    public static void RegisterCampaignScenes()
    {
        var scenes = EditorBuildSettings.scenes.ToList();
        var required = new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/Prologo.unity",
            "Assets/Scenes/Cidade/EntradaDaCidade.unity", "Assets/Scenes/Cidade/CasaDoAvo.unity", "Assets/Scenes/Cidade/BarDoZe.unity" }
            .Concat(SantaRitaChapter2Authoring.Scenes.Select(n => "Assets/Scenes/Cidade/" + n + ".unity"));
        foreach (string path in required)
        {
            if (!File.Exists(path)) throw new Exception("Cena da campanha ausente: " + path);
            int index = scenes.FindIndex(s => s.path == path);
            if (index < 0) scenes.Add(new EditorBuildSettingsScene(path, true));
            else if (!scenes[index].enabled) scenes[index] = new EditorBuildSettingsScene(path, true);
        }
        EditorBuildSettings.scenes = scenes.ToArray(); AssetDatabase.SaveAssets();
    }

    private static void ApprovedScenes()
    {
        var report=new List<string>();
        foreach(string name in new[]{"EntradaDaCidade","CasaDoAvo","BarDoZe"})
        {
            string path="Assets/Scenes/Cidade/"+name+".unity";
            var scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Single);
            foreach(var tr in scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)))
            {
                if(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(tr.gameObject)>0)
                    throw new Exception("C1 possui componente sem script: "+name+"/"+tr.name);
                foreach(var component in tr.GetComponents<MonoBehaviour>())
                {
                    var script=MonoScript.FromMonoBehaviour(component);
                    if(script==null||string.IsNullOrEmpty(AssetDatabase.GetAssetPath(script))||script.GetClass()!=component.GetType())
                        throw new Exception("C1 possui MonoScript local/não persistente: "+name+"/"+tr.name);
                }
            }
            report.Add("PASS "+name+": componentes possuem MonoScript persistente e classe resolvida.");
        }
        // C1 lê os pixels destes terrenos para compor o piso em runtime.
        foreach(string name in new[]{"Grama_tiles","Terra","Terra_Rachada","Asfalto_Trinca","Asfalto_Poca","Asfalto_Buraco","Estrada_Asfalto","Concreto","Cruzamento_A","Asfalto_ParaTerra","FloorTile_Small","FloorTile_Beige","FloorTile_Patterned","WallTile_BlueBeige","WallPaper_Stained"})
        {
            var importer=AssetImporter.GetAtPath("Assets/Resources/Cap1/"+name+".png") as TextureImporter;
            if(importer==null||!importer.isReadable)throw new Exception("C1 precisa de leitura CPU no player: "+name);
        }
        report.Add("PASS quinze terrenos disponíveis para leitura CPU no player.");
        Directory.CreateDirectory(Evidence);File.WriteAllLines(Path.Combine(Evidence,"c1-referencias.txt"),report);
    }

    [MenuItem("Santa Rita/Capítulo 2/Verificar cenas e acessos")]
    public static void Scenes()
    {
        RequireSavedScenes();
        var report = new List<string>(); int errors=0;
        var spawns = new Dictionary<string,HashSet<string>>();
        foreach(var build in EditorBuildSettings.scenes.Where(s=>s.enabled&&s.path.EndsWith(".unity")))
        {
            var destination=EditorSceneManager.OpenScene(build.path,OpenSceneMode.Single);
            spawns[destination.name]=new HashSet<string>(destination.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<SpawnPoint>(true)).Select(s=>s.Id));
        }
        // Dois retornos são compostos pelo diretor C1 em runtime. Contrato explícito, também exercitado no Player.
        if(spawns.TryGetValue("EntradaDaCidade",out var dynamicC1))
        {dynamicC1.Add("retorno_deposito");dynamicC1.Add("saida_bar_ze");}
        foreach(string name in SantaRitaChapter2Authoring.Scenes)
        {
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/Cidade/"+name+".unity",OpenSceneMode.Single);
            var roots=scene.GetRootGameObjects();
            foreach(var g in roots.SelectMany(g=>g.GetComponentsInChildren<Transform>(true)))
            {
                if(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(g.gameObject)>0){errors++;report.Add("FAIL missing script "+name+"/"+g.name);}
                var sr=g.GetComponent<SpriteRenderer>();
                if(sr!=null&&sr.sprite==null){errors++;report.Add("FAIL sprite "+name+"/"+g.name);}
            }
            var d=UnityEngine.Object.FindAnyObjectByType<Capitulo2Director>();
            if(name=="CemiterioSantaRita"&&d.memorialPercepcao==null){errors++;report.Add("FAIL referência serializada da percepção do jazigo.");}
            if (Vector2.Distance(d.uvNoMapa, SantaRitaGeography.UV(SantaRitaGeography.IdDaCena(name))) > .0001f)
            { errors++; report.Add("FAIL geografia serializada " + name); }
            if(name=="CentroSantaRita")
            {
                var prefeitura = GameObject.Find("Prefeitura");
                var entry = UnityEngine.Object.FindObjectsByType<Capitulo2Portal>().Single(p => p.cenaDestino == "PrefeituraSantaRita");
                var back = UnityEngine.Object.FindObjectsByType<SpawnPoint>().Single(p => p.Id == "volta_PrefeituraSantaRita");
                if (Vector2.Distance(prefeitura.transform.position, SantaRitaGeography.Prefeitura) > .001f ||
                    Vector2.Distance(entry.transform.position, SantaRitaGeography.EntradaPrefeitura) > .001f ||
                    Vector2.Distance(back.transform.position, SantaRitaGeography.RetornoPrefeitura) > .001f)
                { errors++; report.Add("FAIL posição/acesso/retorno da Prefeitura R1."); }
                var map=GameObject.Find("Terreno").GetComponent<Tilemap>();
                foreach(var p in new[]{new Vector3Int(31,45,0),new Vector3Int(31,56,0),new Vector3Int(31,70,0),new Vector3Int(60,50,0),new Vector3Int(60,68,0),new Vector3Int(70,37,0)})
                    if(!AssetDatabase.GetAssetPath(map.GetTile(p)).Contains("Asfalto")){errors++;report.Add("FAIL rua perdeu asfalto em "+p);}
                report.Add("Eixos de rua: seis amostras verificadas após todos os acessos de terra/passeios.");
            }
            foreach(var tile in UnityEngine.Object.FindObjectsByType<TilemapCollider2D>())tile.ProcessTilemapChanges();
            Physics2D.SyncTransforms();
            // Portão de consulta público: valida também a sala depois do atendimento.
            foreach(var gate in UnityEngine.Object.FindObjectsByType<Capitulo2Gate>())foreach(var c in gate.GetComponents<Collider2D>())c.enabled=false;
            var player=UnityEngine.Object.FindAnyObjectByType<PlayerController>();
            foreach(var c in player.GetComponentsInChildren<Collider2D>())c.enabled=false;
            var reachable=Flood(d.limiteMin,d.limiteMax,player.transform.position);
            int items=0;
            foreach(var it in roots.SelectMany(g=>g.GetComponentsInChildren<Interactable>()))
            {
                if (it is Capitulo2Portal portal && !string.IsNullOrEmpty(portal.cenaDestino) &&
                    !EditorBuildSettings.scenes.Any(s => s.enabled && Path.GetFileNameWithoutExtension(s.path) == portal.cenaDestino))
                { errors++; report.Add("FAIL destino não incluído na build " + name + "/" + it.name + " → " + portal.cenaDestino); }
                if(it is Capitulo2Portal targetPortal && !string.IsNullOrEmpty(targetPortal.entradaDestino) &&
                    (!spawns.TryGetValue(targetPortal.cenaDestino,out var ids)||!ids.Contains(targetPortal.entradaDestino)))
                {errors++;report.Add("FAIL spawn de destino "+name+"/"+targetPortal.name+" → "+targetPortal.cenaDestino+"/"+targetPortal.entradaDestino);}
                items++;
                bool access=false;
                foreach(var p in reachable)if(Vector2.Distance(p,it.transform.position)<it.InteractionRadius-.15f){access=true;break;}
                if(!access){errors++;report.Add("FAIL acesso "+name+"/"+it.name+" @ "+it.transform.position);}
                if(it is Capitulo2Interactable ci && Capitulo2Director.Dados(ci.interactionId)==null){errors++;report.Add("FAIL conteúdo "+ci.interactionId);}
            }
            foreach(var spawn in UnityEngine.Object.FindObjectsByType<SpawnPoint>())
            {
                if(Blocked(spawn.transform.position)){errors++;report.Add("FAIL spawn em colisão "+name+"/"+spawn.Id);}
            }
            report.Add(name+": "+items+" interações; "+reachable.Count+" células acessíveis; limites "+d.limiteMin+" / "+d.limiteMax);
        }
        Directory.CreateDirectory(Evidence);File.WriteAllLines(Path.Combine(Evidence,"cenas-acessos.txt"),report);
        Debug.Log("C2 SCENES "+(errors==0?"PASS":"FAIL")+": "+errors+" problemas.");
        if(errors>0)throw new Exception("C2: acessos/referências precisam de correção. Ver cenas-acessos.txt");
    }

    private static bool Blocked(Vector2 p)
    {foreach(var c in Physics2D.OverlapCircleAll(p,.24f))if(c.enabled&&!c.isTrigger&&c.GetComponentInParent<PlayerController>()==null)return true;return false;}
    private static List<Vector2> Flood(Vector2 min,Vector2 max,Vector2 start)
    {
        const float step=.5f;int w=Mathf.CeilToInt((max.x-min.x)/step),h=Mathf.CeilToInt((max.y-min.y)/step);
        var seen=new bool[w*h];var queue=new Queue<Vector2Int>();var points=new List<Vector2>();
        var s=new Vector2Int(Mathf.Clamp(Mathf.RoundToInt((start.x-min.x)/step),0,w-1),Mathf.Clamp(Mathf.RoundToInt((start.y-min.y)/step),0,h-1));queue.Enqueue(s);seen[s.y*w+s.x]=true;
        Vector2Int[] dirs={Vector2Int.up,Vector2Int.down,Vector2Int.left,Vector2Int.right};
        while(queue.Count>0)
        {
            var p=queue.Dequeue();Vector2 world=min+new Vector2(p.x,p.y)*step;if(Blocked(world))continue;points.Add(world);
            foreach(var dir in dirs){var n=p+dir;if(n.x<0||n.y<0||n.x>=w||n.y>=h||seen[n.y*w+n.x])continue;seen[n.y*w+n.x]=true;queue.Enqueue(n);}
        }
        return points;
    }

    public static void Run()
    {
        RequireSavedScenes();
        RegisterCampaignScenes();
        var originalScenes=EditorBuildSettings.scenes;
        var originalApis=PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows64);
        bool originalAuto=PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64);
        var originalPreloaded=PlayerSettings.GetPreloadedAssets();
        try
        {
            ApprovedScenes();
            Scenes();
            // A cena de protótipo GameplayScene não é usada por Novo Jogo,
            // prólogo, portas aprovadas ou saves da campanha. Conservada no repo.
            EditorBuildSettings.scenes=originalScenes.Where(s=>!s.path.EndsWith("/GameplayScene.unity")).ToArray();
            // Evita que a análise URP aditiva encontre a luz da cena de inspeção.
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64,false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64,new[]{GraphicsDeviceType.Direct3D11});
            string output=Environment.GetEnvironmentVariable("SR_BUILD_DIR")??Path.GetFullPath(Path.Combine(Application.dataPath,"../../Build/Capitulo2"));
            Directory.CreateDirectory(output);
            bool qa=ForcarDevelopment || Environment.GetCommandLineArgs().Contains("-srDevelopment");
            var result=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=EditorBuildSettings.scenes.Where(s=>s.enabled).Select(s=>s.path).ToArray(),
                locationPathName=Path.Combine(output,"SantaRita.exe"),target=BuildTarget.StandaloneWindows64,options=(qa?BuildOptions.Development:BuildOptions.None)|(Environment.GetCommandLineArgs().Contains("-srCleanBuild")?BuildOptions.CleanBuildCache:BuildOptions.None)});
            string text="Unity "+Application.unityVersion+" Windows x64 "+(qa?"Development QA":"Player")+"\n"+result.summary.result+"\nErros: "+result.summary.totalErrors+"\nBytes: "+result.summary.totalSize+"\nDuração: "+result.summary.totalTime+"\nExecutável: "+output+"/SantaRita.exe\nUTC: "+DateTime.UtcNow.ToString("O")+
                "\nCommit base: " + Revision() + " (inclui alterações locais desta revisão)\nSHA256 C#/cenas/dados/texturas/materiais/metas: " + SourceHash() + "\nSO do Editor: " + SystemInfo.operatingSystem + "\nTarget: StandaloneWindows64 / D3D11";
            Directory.CreateDirectory(Evidence);File.WriteAllText(Path.Combine(Evidence,qa?"build-qa.txt":"build-windows.txt"),text);Debug.Log(text);
            if(result.summary.result!=BuildResult.Succeeded)throw new Exception("Build Windows falhou.");
            if(Application.isBatchMode)EditorApplication.Exit(0);
        }
        catch(Exception ex){Debug.LogException(ex);if(Application.isBatchMode)EditorApplication.Exit(1);else throw;}
        finally
        {
            EditorBuildSettings.scenes=originalScenes;
            PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64,originalApis);
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64,originalAuto);
            PlayerSettings.SetPreloadedAssets(originalPreloaded);
            AssetDatabase.SaveAssets();
        }
    }

    private static string Revision()
    {
        string dir = Path.GetFullPath(Path.Combine(Application.dataPath, "../../.git"));
        string head = File.ReadAllText(Path.Combine(dir, "HEAD")).Trim();
        if (!head.StartsWith("ref: ")) return head;
        string name = head.Substring(5), path = Path.Combine(dir, name);
        if (File.Exists(path)) return File.ReadAllText(path).Trim();
        string packed = Path.Combine(dir, "packed-refs");
        return File.Exists(packed) ? File.ReadLines(packed).FirstOrDefault(l => l.EndsWith(" " + name))?.Split(' ')[0] ?? head : head;
    }

    private static void RequireSavedScenes()
    {
        if (EditorApplication.isPlaying || Enumerable.Range(0, UnityEngine.SceneManagement.SceneManager.sceneCount)
            .Any(i => UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).isDirty))
            throw new Exception("Pare Play e preserve as cenas não salvas antes da verificação/build.");
    }

    private static string SourceHash()
    {
        using (var hash = System.Security.Cryptography.SHA256.Create())
        {
            var buffer=new byte[65536];
            foreach (var file in Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories)
                .Where(p => new[]{".cs",".unity",".json",".png",".mat",".shader",".asset",".meta"}.Contains(Path.GetExtension(p).ToLowerInvariant())).OrderBy(p => p, StringComparer.Ordinal))
            {
                byte[] name = System.Text.Encoding.UTF8.GetBytes(file.Substring(Application.dataPath.Length).Replace('\\', '/') + "\n");
                hash.TransformBlock(name,0,name.Length,null,0);
                using(var stream=File.OpenRead(file))
                {int count;while((count=stream.Read(buffer,0,buffer.Length))>0)hash.TransformBlock(buffer,0,count,null,0);}
            }
            hash.TransformFinalBlock(Array.Empty<byte>(),0,0);
            return BitConverter.ToString(hash.Hash).Replace("-", "").ToLowerInvariant();
        }
    }
}

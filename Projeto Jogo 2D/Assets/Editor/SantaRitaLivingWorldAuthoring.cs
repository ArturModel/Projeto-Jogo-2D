using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
using Object=UnityEngine.Object;

// Composição deliberada por função. Nunca executar automaticamente ao abrir o projeto.
public static class SantaRitaLivingWorldAuthoring
{
    const string Folder="Assets/Scenes/Cidade/";
    static Transform dressing;
    static Material material;
    static Sprite Art(string key)
    {
        var s=Resources.Load<Sprite>("Art/Props/"+key) ?? Resources.Load<Sprite>("Art/World/"+key) ?? Resources.Load<Sprite>("Art/Characters/"+key);
        if(s==null)s=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Capitulo2/"+key+".png");
        if(s==null)s=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Capitulo2/Legado/"+key+".png");
        if(s==null)
        {
            string source="Assets/Resources/Cap1/"+key+".png",copy="Assets/Art/Capitulo2/Legado/"+key+".png";
            if(File.Exists(source))
            {
                if(!File.Exists(copy))AssetDatabase.CopyAsset(source,copy);
                var imp=(TextureImporter)AssetImporter.GetAtPath(copy);imp.textureType=TextureImporterType.Sprite;imp.spriteImportMode=SpriteImportMode.Single;imp.spritePixelsPerUnit=32;imp.filterMode=FilterMode.Point;imp.mipmapEnabled=false;imp.textureCompression=TextureImporterCompression.Uncompressed;imp.SaveAndReimport();
                s=AssetDatabase.LoadAssetAtPath<Sprite>(copy);
            }
        }
        if(s==null)throw new Exception("Arte de composição ausente: "+key);return s;
    }
    static GameObject Prop(string name,string key,float x,float y,float width,bool solid=false)
    {
        var g=new GameObject(name);g.transform.SetParent(dressing,false);g.transform.position=new Vector3(x,y,0);
        var sr=g.AddComponent<SpriteRenderer>();sr.sprite=Art(key);sr.sharedMaterial=material;sr.sortingOrder=1000-Mathf.RoundToInt(y*10);
        g.transform.localScale=Vector3.one*(width/sr.sprite.bounds.size.x);
        if(solid){var b=g.AddComponent<BoxCollider2D>();float depth=key=="Fonte"?1.6f:key=="Sepultura"?1.25f:.35f;b.size=new Vector2(sr.sprite.bounds.size.x*.8f,depth/g.transform.localScale.x);b.offset=new Vector2(0,depth*.5f/g.transform.localScale.x);}
        return g;
    }
    static void Replace(string name,string key,float width)
    {
        var g=GameObject.Find(name);if(g==null)return;var sr=g.GetComponent<SpriteRenderer>();if(sr==null)return;
        var box=g.GetComponent<BoxCollider2D>();Vector2 size=box!=null?Vector2.Scale(box.size,g.transform.lossyScale):Vector2.zero;
        Vector2 off=box!=null?Vector2.Scale(box.offset,g.transform.lossyScale):Vector2.zero;
        sr.sprite=Art(key);g.transform.localScale=Vector3.one*(width/sr.sprite.bounds.size.x);
        if(box!=null){size.x=Mathf.Min(size.x,width*.85f);box.size=size/g.transform.localScale.x;box.offset=off/g.transform.localScale.x;}
    }
    static void Delete(string name){var g=GameObject.Find(name);if(g!=null)Object.DestroyImmediate(g);}
    static void Move(string name,float x,float y){var g=GameObject.Find(name);if(g!=null){g.transform.position=new Vector3(x,y,0);var s=g.GetComponent<SpriteRenderer>();if(s!=null)s.sortingOrder=1000-Mathf.RoundToInt(y*10);}}
    static void Interaction(string id,float x,float y)
    {
        var g=GameObject.Find("Examinar_"+id);if(g==null){g=new GameObject("Examinar_"+id);g.transform.SetParent(dressing,false);g.AddComponent<Capitulo2Interactable>().interactionId=id;}
        g.transform.position=new Vector3(x,y,0);
    }
    static void Spawn(string id,Vector2 pos){var g=new GameObject("Spawn_"+id);g.transform.SetParent(dressing,false);g.transform.position=pos;g.AddComponent<SpawnPoint>().Configurar(id,0);}
    static void Portal(string name,Vector2 pos,string target,string spawn)
    {
        var g=new GameObject(name);g.transform.SetParent(dressing,false);g.transform.position=pos;
        var p=g.AddComponent<Capitulo2Portal>();p.cenaDestino=target;p.entradaDestino=spawn;p.automatico=true;
        var b=g.AddComponent<BoxCollider2D>();b.isTrigger=true;b.size=new Vector2(3,1.1f);
    }
    static void Begin()
    {
        Delete("ComposicaoCotidiana");dressing=new GameObject("ComposicaoCotidiana").transform;
        material=Resources.Load<Material>("Art/WorldLit");
        if(Resources.Load<Material>("Art/PixelRim")==null)
        {var shader=Shader.Find("SantaRita/PixelRim");if(shader==null)throw new Exception("Shader de recorte ainda não importado.");AssetDatabase.CreateAsset(new Material(shader),"Assets/Resources/Art/PixelRim.mat");}
    }
    static void Routine(string name,string job,int phase)
    {
        var g=GameObject.Find(name);if(g==null)return;var r=g.GetComponent<SantaRitaNpcRoutine>()??g.AddComponent<SantaRitaNpcRoutine>();r.atividade=job;r.faseInicial=phase;
        // O alvo continua junto do trabalhador, em vez de ficar no lugar abandonado.
        var id=name.Replace("NPC_","");var proxy=GameObject.Find("Examinar_"+id);
        if(proxy!=null){proxy.transform.SetParent(g.transform,true);}
    }
    static void Lamp(float x,float y,float radius=4)
    {
        var g=new GameObject("LuminariaDeUso");g.transform.SetParent(dressing,false);g.transform.position=new Vector3(x,y,0);
        var l=g.AddComponent<Light2D>();l.lightType=Light2D.LightType.Point;l.color=new Color(1,.85f,.64f);l.intensity=.52f;l.pointLightOuterRadius=radius;l.shadowIntensity=.4f;
    }
    static void PaperOnTable(string name,float x,float y,int order)
    {var paper=Prop(name,"Caderno",x,y,.6f);paper.GetComponent<SpriteRenderer>().sortingOrder=order;}
    static void KeepOverTable(string item,string table)
    {
        var a=GameObject.Find(item);var b=GameObject.Find(table);if(a==null||b==null)return;
        a.GetComponent<SpriteRenderer>().sortingOrder=b.GetComponent<SpriteRenderer>().sortingOrder+2;
    }
    [MenuItem("Santa Rita/Cidade/Recompor Centro, Cemitério e interiores")]
    public static void Apply()
    {
        if(EditorApplication.isPlaying||Enumerable.Range(0,SceneManager.sceneCount).Any(i=>SceneManager.GetSceneAt(i).isDirty))throw new Exception("Pare Play e preserve as cenas não salvas antes da composição.");
        var setup=EditorSceneManager.GetSceneManagerSetup();
        try
        {
            CreateCemetery();
            foreach(var name in SantaRitaChapter2Authoring.Scenes)
            {
                var scene=EditorSceneManager.OpenScene(Folder+name+".unity");Begin();
                if(name=="CentroSantaRita")Center();else if(name=="CemiterioSantaRita")Cemetery();else Interior(name);
                foreach(var sr in Object.FindObjectsByType<SpriteRenderer>())
                {if(sr.name.StartsWith("AO_")||sr.sortingOrder< -2000)continue;sr.sharedMaterial=material;if(!sr.name.Contains("Parede")&&!sr.name.Contains("Rodape")&&!sr.name.Contains("CapaParede"))sr.sortingOrder=1000-Mathf.RoundToInt(sr.transform.position.y*10);if(sr.name.StartsWith("Arvore")&&sr.GetComponent<SantaRitaForeground>()==null)sr.gameObject.AddComponent<SantaRitaForeground>();}
                foreach(var sr in Object.FindObjectsByType<SpriteRenderer>().Where(s=>s.sprite!=null&&AssetDatabase.GetAssetPath(s.sprite).EndsWith("/Bench_Wood.png")).ToArray())Replace(sr.name,"BancoPraca",2.4f);
                // Depois da ordenação por Y: papéis estão apoiados na superfície,
                // portanto precisam aparecer acima do móvel que os sustenta.
                KeepOverTable("CadernoDeManutencao","CadernoAbrigo");KeepOverTable("RomaneioSobreMesa","RomaneioMesa");
                KeepOverTable("CadernoDeVisitas","CadernoConvivio");KeepOverTable("Protocolo","BalcaoConsulta");
                KeepOverTable("PilhaRequerimentos","BalcaoConsulta");KeepOverTable("FolhetosParoquiais","MesaFolhetos");
                KeepOverTable("BoletinsNoArquivo","ArquivoDeConsulta");
                EditorSceneManager.SaveScene(scene);
            }
            var builds=EditorBuildSettings.scenes.ToList();if(!builds.Any(s=>s.path==Folder+"CemiterioSantaRita.unity"))builds.Add(new EditorBuildSettingsScene(Folder+"CemiterioSantaRita.unity",true));EditorBuildSettings.scenes=builds.ToArray();
            AssetDatabase.SaveAssets();
        }
        finally{EditorSceneManager.RestoreSceneManagerSetup(setup);}
    }
    public static void CreateCemetery()
    {
        string path=Folder+"CemiterioSantaRita.unity";if(File.Exists(path))return;
        if(!AssetDatabase.CopyAsset(Folder+"CentroSantaRita.unity",path))throw new Exception("Não foi possível criar a cena do Cemitério.");
        var scene=EditorSceneManager.OpenScene(path);var d=Object.FindAnyObjectByType<Capitulo2Director>();
        foreach(Transform child in d.transform.Cast<Transform>().ToArray())
            if(child.name!="Piso" && child.position.y<82)Object.DestroyImmediate(child.gameObject);
        foreach(var map in Object.FindObjectsByType<Tilemap>())
            foreach(var cell in map.cellBounds.allPositionsWithin)
                if(cell.y<82||cell.y>110||cell.x<43||cell.x>87)map.SetTile(cell,null);
        var extra=GameObject.Find("ComposicaoCotidiana");if(extra!=null)Object.DestroyImmediate(extra);
        d.nomeLocal="Cemitério municipal";d.interior=false;d.assinaturaAudio=1;d.limiteMin=new Vector2(43,82);d.limiteMax=new Vector2(88,111);
        var player=Object.FindAnyObjectByType<PlayerController>();player.transform.position=new Vector2(64,87);
        var cam=Camera.main;if(cam!=null)cam.transform.position=new Vector3(64,88,-10);
        foreach(var sr in Object.FindObjectsByType<SpriteRenderer>().Where(s=>s.name!="Player"&&s.transform.position.y<82).ToArray())Object.DestroyImmediate(sr.gameObject);
        EditorSceneManager.SaveScene(scene);
    }
    static void Center()
    {
        var d=Object.FindAnyObjectByType<Capitulo2Director>();d.limiteMax=new Vector2(96,85);
        foreach(Transform child in d.transform.Cast<Transform>().ToArray())
            if(child.name!="Piso"&&child.position.y>=83&&child.position.x>43)Object.DestroyImmediate(child.gameObject);
        foreach(var map in Object.FindObjectsByType<Tilemap>())foreach(var cell in map.cellBounds.allPositionsWithin)if(cell.y>=83&&cell.x>43)map.SetTile(cell,null);
        var terrain=GameObject.Find("Terreno").GetComponent<Tilemap>();var dirt=AssetDatabase.LoadAssetAtPath<Tile>("Assets/Art/Capitulo2/Tiles/Terra.asset");var grass=AssetDatabase.LoadAssetAtPath<Tile>("Assets/Art/Capitulo2/Tiles/Grama.asset");
        if(grass!=null)for(int y=83;y<=91;y++)for(int x=-20;x<96;x++)terrain.SetTile(new Vector3Int(x,y,0),grass);
        Vector2 a=new Vector2(31,81),b=new Vector2(25,84),ab=b-a;
        if(dirt!=null)for(int y=81;y<=85;y++)for(int x=23;x<=33;x++)
        {Vector2 p=new Vector2(x+.5f,y+.5f);float t=Mathf.Clamp01(Vector2.Dot(p-a,ab)/ab.sqrMagnitude);if(Vector2.Distance(p,a+ab*t)<1.6f)terrain.SetTile(new Vector3Int(x,y,0),dirt);}
        SantaRitaRuralPaths.Apply(terrain,dressing,material);
        foreach(var range in new[]{new Vector2(43,62.5f),new Vector2(65.5f,88)})
        {
            for(float x=range.x;x<range.y;x+=2)Prop("MuroDoCemiterio","WoodFence",x,83,2.1f,true);
            var fence=new GameObject("ColisaoDoMuroVisivel");fence.transform.SetParent(dressing,false);fence.transform.position=new Vector2((range.x+range.y)/2,83.1f);
            fence.AddComponent<BoxCollider2D>().size=new Vector2(range.y-range.x,.35f);
        }
        Prop("EntradaDoCemiterio","PortaoCemiterio",64,82.5f,4);
        Lamp(64,83,4);
        foreach(float x in new[]{47f,54f,74f,82f})Prop("ArvoreFundoCemiterio","Tree_A",x,87,5);
        Delete("Memorial_Praca");Delete("Examinar_placa_praca");
        Prop("FontePublica","Fonte",45.5f,50,3.4f,true);Lamp(45.5f,51,5);
        Prop("QuadroAvisosDaPraca","QuadroAvisos",54.4f,51,2.4f,true);
        Interaction("avisos_praca",54.4f,49.9f);
        foreach(var p in new[]{new Vector2(41.1f,52.2f),new Vector2(49.5f,52.2f),new Vector2(41.1f,42),new Vector2(49.5f,42)})Prop("BancoPublico","BancoPraca",p.x,p.y,2.4f,true);
        foreach(var p in new[]{new Vector2(35.8f,40.5f),new Vector2(55.6f,60)})Prop("LixeiraDaPraca","Dumpster",p.x,p.y,.8f,true);
        foreach(var p in new[]{new Vector2(36,50),new Vector2(55,55),new Vector2(48,58),new Vector2(42,44)})Prop("JardimCuidado","Flowers",p.x,p.y,1.6f);
        Prop("JornalNoBanco","Bilhete",41,52.7f,.35f);
        Portal("Portao_Cemiterio",new Vector2(64,81.7f),"CemiterioSantaRita","centro");Spawn("volta_CemiterioSantaRita",new Vector2(64,79.5f));
        var north=Object.FindObjectsByType<Capitulo2Portal>().FirstOrDefault(p=>p.cenaDestino=="EntradaDaCidade");if(north!=null)north.flagNecessaria="";
        Routine("NPC_chico","varrer",3);
        // Passantes permanecem em seus afazeres, sem entregar uma pista central.
        var local=Prop("VizinhaNaPraca","Npc_Passageira",50.5f,54,2,true);local.AddComponent<SantaRitaNpcRoutine>().atividade="jornal";
        var vizinho=Prop("MoradorNoBanco","Npc_Motorista",40.5f,52.5f,2,true);vizinho.AddComponent<SantaRitaNpcRoutine>().atividade="chimarrao";
        Interaction("moradores_praca",49.6f,52.8f);
    }
    static void Cemetery()
    {
        var d=Object.FindAnyObjectByType<Capitulo2Director>();d.nomeLocal="Cemitério municipal";d.limiteMin=new Vector2(43,82);d.limiteMax=new Vector2(88,111);
        foreach(var it in Object.FindObjectsByType<Interactable>().Where(i=>i.transform.position.x<43||i.transform.position.x>88||i.transform.position.y<82).ToArray())Object.DestroyImmediate(it.gameObject);
        Delete("Memoria_Julia");Delete("Examinar_memorial");Delete("Sepultura_2_1");
        Prop("JazigoAntigoDaFamilia","Sepultura",60,99,2.5f,true);Interaction("memorial",60,97.7f);
        foreach(var sr in Object.FindObjectsByType<SpriteRenderer>().Where(s=>s.name.StartsWith("Sepultura_")).ToArray())Replace(sr.name,"Sepultura",2.1f);
        Portal("Saida_Cemiterio",new Vector2(64,83.2f),"CentroSantaRita","volta_CemiterioSantaRita");Spawn("centro",new Vector2(64,86));
        Routine("NPC_alcina","varrer",5);
        Prop("BancoDeEspera","BancoPraca",70.6f,87,2.3f,true);
        Prop("ApoioDeJardinagem","Caixotes",80.5f,97,.9f,true);
    }
    static void Interior(string name)
    {
        // Mesma família visual também nos móveis secundários; não deixar os
        // armários/mesas de preenchimento antigos destoarem das bancadas novas.
        foreach(var sr in Object.FindObjectsByType<SpriteRenderer>().Where(s=>s.sprite!=null).ToArray())
        {
            string key=Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(sr.sprite));
            switch(key)
            {
                case "WoodTable_Worn": Replace(sr.name,"MesaMadeira",2.5f); break;
                case "FoldingChair": Replace(sr.name,"CadeiraMadeira",.9f); break;
                case "Armchair": Replace(sr.name,"CadeiraMadeira",1.1f); break;
                case "Wardrobe_Medium": Replace(sr.name,"ArmarioMadeira",2); break;
                case "Cabinet_Medium": Replace(sr.name,"ArmarioMadeira",1.8f); break;
                case "Arquivo": Replace(sr.name,"ArquivoMetal",1.6f); break;
                case "Caixotes": Replace(sr.name,"CaixasManutencao",1.6f); break;
                case "BoxesShelf": Replace(sr.name,"CaixasManutencao",1.8f); break;
                case "Radio": Replace(sr.name,"RadioAntigo",.9f); break;
                case "Window_Curtain": Replace(sr.name,"JanelaCortina",1.5f); break;
                case "ArmarioVelas": Replace(sr.name,"ArmarioVelas",2.4f); break;
                case "BancoRodoviaria": Replace(sr.name,"BancoPraca",3.5f); break;
                case "BancadaManutencao": Replace(sr.name,"BancadaOficina",3.8f); break;
            }
        }
        if(name=="AbrigoCemiterio")
        {var p=Object.FindObjectsByType<Capitulo2Portal>().First(x=>x.name=="Porta_Saida");p.cenaDestino="CemiterioSantaRita";
            Prop("FerramentasDeUso","CaixasManutencao",2.4f,10.8f,1.4f,true);Prop("PanosDaRonda","ArmarioMadeira",13.7f,10.8f,1.6f,true);
            Prop("CafeDaRonda","Chimarrao",11.8f,4.2f,1.5f,true);Prop("CadeiraDaRonda","CadeiraMadeira",3,5,.9f,true);
            PaperOnTable("CadernoDeManutencao",9,9.1f,922);return;}
        if(name=="IgrejaMatriz")
        {
            Replace("MuralComunitario","QuadroAvisos",2.4f);Move("MuralComunitario",18.3f,22);Interaction("foto_igreja",18.3f,20.6f);
            Prop("ArmarioDeSacristia","ArmarioMadeira",3.1f,23,2.2f,true);Prop("FloresDoAltarEsq","Flowers",7.5f,21,1.4f);Prop("FloresDoAltarDir","Flowers",14.5f,21,1.4f);
            Prop("BancoAcolhimento","BancoPraca",4.8f,4.5f,2.5f,true);Prop("MesaFolhetos","MesaMadeira",18.2f,5,2,true);Prop("FolhetosParoquiais","Bilhete",18.2f,5.5f,.6f);
            Routine("NPC_bento","arrumar",2);
        }
        else if(name=="PrefeituraSantaRita")
        {
            Replace("BalcaoConsulta","MesaPublica",2.8f);
            Move("Protocolo",11.5f,9.8f);Interaction("protocolo",11.3f,7.9f);
            var paper=GameObject.Find("Protocolo");if(paper!=null)paper.GetComponent<SpriteRenderer>().sortingOrder=912;
            Prop("MesaExpediente","MesaPublica",19.5f,16.4f,3.1f,true);Prop("CadeiraExpediente","CadeiraMadeira",19.5f,18,.9f,true);
            Prop("ArquivoCorrespondencia","ArquivoMetal",20.5f,10.7f,1.5f,true);Prop("RelogioPlantao","WallClock",3,20.6f,.7f);
            Prop("PilhaRequerimentos","Bilhete",11.3f,10.1f,.5f);
            Interaction("placa_praca",4.5f,9.5f);Prop("BoletinsNoArquivo","Bilhete",4.5f,11.4f,.5f);
            Routine("NPC_clarice","papeis",1);
        }
        else if(name=="RodoviariaSantaRita")
        {
            Replace("Guiche","Caixa",3.8f);
            Replace("Horarios","QuadroAvisos",2.6f);
            Prop("CafeDaEspera","Cafe",4.5f,16.6f,3,true);Prop("LixeiraGuiche","Dumpster",23.6f,5,.7f,true);
            Prop("AguaDaEspera","Geladeira",2,11.8f,1.3f,true);Prop("DocumentosGuiche","Bilhete",18,13,.5f);
            Prop("MalaNoBanco","CardboardBox_Single",7.5f,6.2f,.7f);Routine("NPC_dario","papeis",0);
            Routine("Motorista_Escala","jornal",4);Routine("Passageira_Espera","espera",8);
        }
        else if(name=="MercadinhoDeposito")
        {
            foreach(var stock in Object.FindObjectsByType<SpriteRenderer>().Where(s=>s.name.StartsWith("Pallet_")).ToArray())Replace(stock.name,"CargasMercado",2.2f);
            Replace("EstoqueMesmaLoja","Prateleira",2.8f);Prop("EstoqueSecos","Prateleira",15.8f,14.5f,2.8f,true);
            Prop("EstoqueGarrafas","Geladeira",16,5.8f,2.1f,true);Prop("DevolucaoVasilhames","BottlesCrate",3.4f,14,1.5f,true);
            Prop("ConferenciaDeNotas","MesaMadeira",10,13.3f,2.4f,true);Prop("CadeiraConferencia","CadeiraMadeira",8,13.8f,.9f,true);
            PaperOnTable("RomaneioSobreMesa",10,11.1f,902);
            var portal=Object.FindObjectsByType<Capitulo2Portal>().First(p=>p.cenaDestino=="EntradaDaCidade");portal.flagNecessaria="";
        }
        else if(name=="OficinaRadio")
        {
            Replace("BancadaEletronica","BancadaOficina",4.8f);Prop("EstanteDePecas","CaixasManutencao",3.4f,13,2.3f,true);
            Prop("TelevisorEmReparo","Televisor",14.4f,4.9f,1.8f,true);Prop("MesaTeste","MesaMadeira",13.5f,8,2.2f,true);
            Prop("BancoDeTrabalho","CadeiraMadeira",6,10.8f,.9f,true);
            Move("NPC_mila",8.4f,8.2f);Interaction("mila",8.4f,7.5f);Routine("NPC_mila","reparo",3);
        }
        else if(name=="CasaCostureira")
        {
            Replace("MaquinaCelina","BancadaCostura",4);Prop("SofaDaSala","Sofa",12.8f,4.6f,3.1f,true);
            Move("PoltronaCelina",13.6f,6.4f);Prop("MesaDoCha","Chimarrao",11.2f,12.4f,2.2f,true);
            Prop("RoupasDeEntrega","ArmarioMadeira",15.5f,12.7f,2,true);Prop("LinhaERecortes","CaixasManutencao",3,11.1f,1.2f,true);
            Move("NPC_celina",6.4f,7.9f);Interaction("celina",6.4f,7.2f);Routine("NPC_celina","costura",2);
        }
        else if(name=="SociedadeBeiraRio")
        {
            Replace("MesaConvivio","Chimarrao",3.8f);Prop("MesaDoFundo","Chimarrao",19.6f,11,2.7f,true);
            PaperOnTable("CadernoDeVisitas",20.4f,6.7f,946);
            Prop("BalcaoDoSalao","BalcaoBar",5,10,4,true);Prop("TelevisaoDoSalao","Televisor",4.5f,12.5f,1.6f);
            Prop("FogaoDaCopa","Fogao",3.4f,4.5f,1.8f,true);Prop("GeladeiraDaCopa","Geladeira",20.8f,3.6f,1.5f,true);
            foreach(float y in new[]{19f,25.5f,32f})
            {Prop("BancoDaCancha","BancoPraca",18.6f,y,2.3f,true);Prop("PilarDoGalpao","PilarMadeira",7.2f,y,.3f,true);Lamp(11.8f,y+1,4.5f);}
            foreach(var wall in Object.FindObjectsByType<SpriteRenderer>().Where(s=>s.name=="Parede"&&Mathf.Abs(s.transform.position.y-26)<.1f&&(Mathf.Abs(s.transform.position.x-8.7f)<.1f||Mathf.Abs(s.transform.position.x-15.3f)<.1f)))wall.enabled=false;
            var railSprite=Art("CanchaGuia");
            foreach(float x in new[]{8.7f,15.3f})for(int y=17;y<36;y+=2)Prop("GuiaMadeiraCancha","CanchaGuia",x,y,2.1f).transform.rotation=Quaternion.Euler(0,0,90);
            foreach(float y in new[]{16f,36f})
            {var board=Prop("CabeceiraDeMadeira","CanchaGuia",12,y,6.5f,true);board.GetComponent<BoxCollider2D>().size=new Vector2(railSprite.bounds.size.x,.3f/board.transform.localScale.x);}
            Routine("NPC_tonao","bochas",5);Routine("NPC_nair","limpar",2);
            Prop("ParticipanteDoSalao","Npc_Frequentador",3.8f,7.3f,2,true).AddComponent<SantaRitaNpcRoutine>().atividade="chimarrao";
        }
        Lamp(3,3,3);
    }
}

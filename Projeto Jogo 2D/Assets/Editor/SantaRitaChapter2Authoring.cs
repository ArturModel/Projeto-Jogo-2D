using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

// Autoria explícita e limitada a C2. Sem InitializeOnLoad e sem reserializar C1.
// Após gerar, as cenas são conteúdo normal editável pelo artista no Editor.
public static class SantaRitaChapter2Authoring
{
    public static readonly string[] Scenes = { "CentroSantaRita", "IgrejaMatriz", "PrefeituraSantaRita",
        "RodoviariaSantaRita", "MercadinhoDeposito", "OficinaRadio", "CasaCostureira", "SociedadeBeiraRio", "AbrigoCemiterio", "CemiterioSantaRita" };
    private const string Root = "Assets/Art/Capitulo2";
    private static readonly Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();
    private static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    private static Scene source;
    private static Transform world;
    private static Capitulo2Director director;
    private static readonly List<string> connections = new List<string>();
    private static Tilemap ground;
    private static string currentSceneName;

    [MenuItem("Santa Rita/Capítulo 2/Criar cenas ausentes")]
    public static void Run()
    {
        bool regenerate = Environment.GetCommandLineArgs().Contains("-srRegenerateC2");
        try
        {
            ConfigureArt();
            source = EditorSceneManager.OpenScene("Assets/Scenes/Cidade/EntradaDaCidade.unity", OpenSceneMode.Single);
            connections.Clear(); connections.Add("cena,porta,destino,spawn,x,y,condicao");
            foreach (string name in Scenes)
            {
                string path = "Assets/Scenes/Cidade/" + name + ".unity";
                if (File.Exists(path) && !regenerate) continue;
                if (name=="CemiterioSantaRita")continue;
                if (name == "CentroSantaRita") Exterior(); else Interior(name);
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), path);
                EditorSceneManager.CloseScene(SceneManager.GetActiveScene(), true);
                SceneManager.SetActiveScene(source);
            }
            SantaRitaLivingWorldAuthoring.CreateCemetery();
            var build = EditorBuildSettings.scenes.ToList();
            foreach (string name in Scenes)
            {
                string path = "Assets/Scenes/Cidade/" + name + ".unity";
                if (!build.Any(s => s.path == path)) build.Add(new EditorBuildSettingsScene(path, true));
            }
            EditorBuildSettings.scenes = build.ToArray();
            AssetDatabase.SaveAssets();
            string docs = Environment.GetEnvironmentVariable("SR_EVIDENCE_DIR") ?? Path.GetFullPath(Path.Combine(Application.dataPath,"../../Docs/Evidencias/Capitulo2"));
            Directory.CreateDirectory(docs);
            File.WriteAllLines(Path.Combine(docs,"portas.csv"), connections);
            File.WriteAllText(Path.Combine(docs,"autoria.txt"), "Cenas C2 geradas por comando explícito em " + DateTime.UtcNow.ToString("O") +
                "\nSem salvar ou modificar a cena fonte de C1. Escola excluída por decisão do autor.\n" + string.Join("\n", Scenes));
            Debug.Log("C2 AUTHORING PASS: " + Scenes.Length + " cenas disponíveis.");
            if (Application.isBatchMode) EditorApplication.Exit(0);
        }
        catch(Exception ex) { Debug.LogException(ex); if(Application.isBatchMode) EditorApplication.Exit(1); else throw; }
    }

    private static void ConfigureArt()
    {
        Directory.CreateDirectory(Root + "/Tiles");
        AssetDatabase.Refresh();
        foreach(string guid in AssetDatabase.FindAssets("t:Texture2D", new[] {Root}))
        {
            var importer = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid)) as TextureImporter;
            if(importer == null) continue;
            importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = importer.assetPath.Contains("/Legado/") && !Path.GetFileName(importer.assetPath).StartsWith("Npc_") ? 16 : 32;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false; importer.textureCompression = TextureImporterCompression.Uncompressed;
            var settings = new TextureImporterSettings(); importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.Custom;
            settings.spritePivot = importer.assetPath.Contains("/Tiles/") ? new Vector2(.5f,.5f) :
                Path.GetFileName(importer.assetPath).StartsWith("Npc_") ? new Vector2(.5f,.125f) : new Vector2(.5f,0);
            importer.SetTextureSettings(settings); importer.SaveAndReimport();
        }
        foreach(string name in new[]{"tonao","bento","clarice","alcina","dario","mila","celina","nair"})
        {
            var importer=AssetImporter.GetAtPath("Assets/Resources/UI/Portraits/"+name+".png") as TextureImporter;
            if(importer==null) continue;
            importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
            importer.spritePixelsPerUnit=32;importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;
            importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();
        }
        foreach(string type in new[]{"Grama","Terra","Calcada","Asfalto","AsfaltoH","AsfaltoV","Pedra","Agua","Madeira","PisoVerde","PisoCreme","PisoCinza","Areia"}) Tile(type);
    }

    private static Scene Setup(string name, Vector2 min, Vector2 max, string local, bool inside, Vector2 map, int audio, Vector2 spawn)
    {
        currentSceneName = name;
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        SceneManager.SetActiveScene(scene);
        var originals = source.GetRootGameObjects();
        GameObject player = null, camera = null;
        foreach (string rootName in new[]{"Player","Main Camera","Global Light 2D","MenuPausa","Chuva"})
        {
            var original = originals.FirstOrDefault(g=>g.name==rootName);
            if(original==null) continue;
            var copy=Object.Instantiate(original);copy.name=rootName;SceneManager.MoveGameObjectToScene(copy,scene);copy.SetActive(true);
            if(rootName=="Player")player=copy;if(rootName=="Main Camera")camera=copy;
            if(rootName=="Global Light 2D") {var light=copy.GetComponent<Light2D>();if(light!=null){light.intensity=inside?.9f:.67f;light.color=inside?new Color(1,.94f,.82f):new Color(.77f,.85f,.91f);}}
        }
        if(player==null||camera==null)throw new Exception("C1 source rig missing.");
        player.transform.position=spawn;
        var camSO=new SerializedObject(camera.GetComponent<CameraFollow>());camSO.FindProperty("target").objectReferenceValue=player.transform;camSO.ApplyModifiedPropertiesWithoutUndo();
        camera.transform.position=new Vector3(spawn.x,spawn.y+1.1f,-10);
        var root=new GameObject("Capitulo2_"+name);world=root.transform;
        director=root.AddComponent<Capitulo2Director>();director.nomeLocal=local;director.interior=inside;
        director.limiteMin=min;director.limiteMax=max;director.uvNoMapa=map;director.assinaturaAudio=audio;
        var grid=new GameObject("Piso",typeof(Grid));grid.transform.SetParent(world,false);
        var mapGO=new GameObject("Terreno",typeof(Tilemap),typeof(TilemapRenderer),typeof(TilemapCollider2D));mapGO.transform.SetParent(grid.transform,false);
        ground=mapGO.GetComponent<Tilemap>();mapGO.GetComponent<TilemapRenderer>().sortingOrder=-3000;
        return scene;
    }

    private static void Exterior()
    {
        Setup("CentroSantaRita",new Vector2(-20,-8),new Vector2(96,112),"Centro de Santa Rita — Praça da Matriz",false,new Vector2(.5f,.55f),0,new Vector2(30,78));
        for(int y=-8;y<112;y++)for(int x=-20;x<96;x++)
            ground.SetTile(new Vector3Int(x,y,0),Tile(y < -3+Mathf.Sin(x*.09f)*1.3f || (x < -16+Mathf.Sin(y*.15f) && y<36) ? "Agua":"Grama"));
        Road(new Vector2(31,81),new Vector2(31,22),5);Road(new Vector2(31,37),new Vector2(78.5f,37),5);
        Road(new Vector2(60,20),new Vector2(60,79),5);Road(new Vector2(31,62),new Vector2(70,62),3,true);
        Road(new Vector2(64,77),new Vector2(64,86),3,true);
        Road(new Vector2(31,27),new Vector2(20,19),3,true);Road(new Vector2(20,19),new Vector2(5,13),3,true);
        Road(new Vector2(31,22),new Vector2(46,2),2,true);
        Road(new Vector2(31,35),new Vector2(12,35),3,true);Road(new Vector2(23,35),new Vector2(23,28),2,true);
        RectTiles(new RectInt(35,40,22,21),"Pedra");
        RectTiles(new RectInt(49,85,33,22),"Grama");
        Road(new Vector2(64,86),new Vector2(64,103),2,true);
        RectTiles(new RectInt(-2,2,16,12),"Terra");RectTiles(new RectInt(15,47,12,9),"Calcada");

        Building("Igreja",new Vector2(44.8f,64),"IGREJA DA MATRIZ","IgrejaMatriz",new Vector2(44.8f,61.5f));
        Building("Prefeitura",new Vector2(69,44),"PREFEITURA","PrefeituraSantaRita",new Vector2(69,41.5f));
        Building("Rodoviaria",new Vector2(68,22),"RODOVIÁRIA","RodoviariaSantaRita",new Vector2(68,19.5f));
        Building("Sociedade",new Vector2(5,5),"SOCIEDADE BEIRA-RIO","SociedadeBeiraRio",new Vector2(5,2.6f));
        Building("OficinaRadio",new Vector2(11.2f,33.6f),"RÁDIOS • CONSERTOS","OficinaRadio",new Vector2(11.2f,31.2f));
        Building("CasaCostura",new Vector2(21.8f,28.5f),"COSTURA • CELINA","CasaCostureira",new Vector2(21.8f,26));
        // Somente fundos do mercado: orientação/estoque preservados, sem segunda fachada frontal.
        Prop("Fundos_MesmoMercadinho","CasaSimples_B",new Vector2(20.2f,50),1.45f,new Vector2(5.2f,2.8f));
        LabelWorld("MERCADINHO • SERVIÇO",new Vector2(20.2f,52.7f),new Vector2(6,.5f));
        Portal("Deposito",new Vector2(20.2f,49.3f),"MercadinhoDeposito","centro","Entrar no depósito do Mercadinho");
        Spawn("volta_MercadinhoDeposito",new Vector2(20.2f,47.5f),0);
        Prop("Caixotes_Patio","Caixotes",new Vector2(24,49),.8f,new Vector2(1.2f,.7f));
        Prop("Lixeira_Patio","Dumpster",new Vector2(16,49),.9f,new Vector2(1.3f,.7f));
        // Portão norte com ida/volta direta à Rua Norte aprovada.
        Road(new Vector2(31,81),new Vector2(25,84),3,true);
        Portal("RuaNorte",new Vector2(25,84),"EntradaDaCidade","saida_bar_ze","Voltar à Rua Norte / Bar do Zé",true,Flags.ZeDerrotadoNaSinuca);
        Spawn("c1_portao_norte",new Vector2(30,78),0);
        Sign("RUA NORTE ←  •  CENTRO ↓",new Vector2(29,81));
        Sign("CEMITÉRIO ↑",new Vector2(63,78));Sign("BEIRA-RIO ← ↓",new Vector2(29,25));

        // Praça com passeios, canteiros, bancos e vida cotidiana.
        foreach(var bed in new[]{new RectInt(36,42,7,7),new RectInt(49,42,7,7),new RectInt(36,53,7,7),new RectInt(49,53,7,7)})RectTiles(bed,"Grama");
        foreach(var p in new[]{new Vector2(39,45),new Vector2(52,45),new Vector2(39,56),new Vector2(52,56)})Tree(p);
        for(int y=43;y<=58;y+=7)for(int x=37;x<=54;x+=17)
        { Garden(new Vector2(x,y),new Vector2(2.5f,3));Prop("BancoPraca_"+x+"_"+y,"Bench_Wood",new Vector2(x,y-2),1,new Vector2(1.5f,.55f)); }
        Prop("Memorial_Praca","Memorial",new Vector2(45.5f,50),1,new Vector2(3.2f,.7f));
        Interacao("placa_praca",new Vector2(45.5f,49.2f));
        NPC("chico","Npc_Frequentador",new Vector2(39,47),"orientacao",true);
        Phone("orelhao_centro_matriz",new Vector2(40,41.8f));
        Phone("orelhao_bocha_beira_rio",new Vector2(12.7f,4.6f));
        Interacao("epilogo",new Vector2(14.6f,7.7f));Prop("BancoMargem","Bench_Wood",new Vector2(14.6f,8.4f),1,new Vector2(1.5f,.6f));
        Interacao("rio",new Vector2(6,-.4f));
        // Cemitério no norte e a leste do Bar; a pequena construção é apoio local.
        Fence(new Vector2(49,85),new Vector2(60,85));Fence(new Vector2(68,85),new Vector2(82,85));
        Fence(new Vector2(49,107),new Vector2(82,107));Fence(new Vector2(49,85),new Vector2(49,107));Fence(new Vector2(82,85),new Vector2(82,107));
        Prop("PortaoCemiterio","PortaoCemiterio",new Vector2(64,84),.75f,Vector2.zero);
        for(int row=0;row<3;row++)for(int col=0;col<4;col++)
        {
            float x=53+col*7;float y=89+row*5; if(col==2)x+=2;
            RectTiles(new RectInt((int)x-1,(int)y-1,3,2),"Pedra");
            Prop("Sepultura_"+row+"_"+col,"Tumulo",new Vector2(x,y),row%2==0?.75f:.82f,new Vector2(.9f,.9f));
            if((row+col)%2==0)Prop("FloresDaFamilia","PlanterBox",new Vector2(x+1,y-.4f),.45f,Vector2.zero);
        }
        Road(new Vector2(64,102),new Vector2(76,102),2,true);
        foreach(var p in new[]{new Vector2(51,104),new Vector2(58,104),new Vector2(73,105),new Vector2(80,87)})Tree(p);
        Prop("Memoria_Julia","Memorial",new Vector2(64,100),.85f,new Vector2(3,.6f));
        Interacao("memorial",new Vector2(64,99));
        NPC("alcina","Npc_alcina",new Vector2(68,88),"alcina");
        Prop("Abrigo_Cemiterio","CasaSimples_C",new Vector2(78.5f,101),1.2f,new Vector2(4.4f,2.3f));
        LabelWorld("APOIO DO CEMITÉRIO",new Vector2(78.5f,103.3f),new Vector2(5,.5f));
        Portal("Abrigo",new Vector2(78.5f,100.2f),"AbrigoCemiterio","centro","Entrar no abrigo das velas");
        Spawn("volta_AbrigoCemiterio",new Vector2(78.5f,98),0);

        // Apenas a cabeceira e a continuação visual da ponte, sem produzir a outra margem.
        RectTiles(new RectInt(44,-8,4,8),"Madeira");
        Fence(new Vector2(43.7f,-8),new Vector2(43.7f,0));Fence(new Vector2(48.2f,-8),new Vector2(48.2f,0));
        Prop("CordaManutencaoPonte","WoodFence",new Vector2(46,-.3f),1.6f,new Vector2(2.4f,.5f));
        Interacao("ponte",new Vector2(46,1));
        // Aproximação entre Prefeitura e Clube; Clube/Escola não são construídos neste recorte.
        Road(new Vector2(74,48),new Vector2(78,48),3,true);
        var grade=Prop("Grade_Portaria_Futura","Portao_Centro",new Vector2(78,48),.65f,Vector2.zero);
        grade.transform.rotation=Quaternion.Euler(0,0,90);
        Box(grade,new Vector2(4/.65f,.5f/.65f),new Vector2(0,1.5f));
        Interacao("portaria_clube",new Vector2(76,48));
        BoundaryForest();
        foreach(var pos in new[]{new Vector2(28,42),new Vector2(57,40),new Vector2(61,65),new Vector2(41,62),new Vector2(67,83),new Vector2(27,22),new Vector2(12,13),new Vector2(72,32)}) Lamp(pos);
        Crosswalk(31,36,true);Crosswalk(60,36,true);
        RectTiles(new RectInt(28,34,7,7),"Asfalto");RectTiles(new RectInt(57,34,7,7),"Asfalto");
        Crosswalk(31,32,true);Crosswalk(60,32,true);
    }

    private static void Interior(string name)
    {
        var sizes=new Dictionary<string,Vector2> { {"IgrejaMatriz",new Vector2(22,26)}, {"PrefeituraSantaRita",new Vector2(24,22)},
            {"RodoviariaSantaRita",new Vector2(26,20)}, {"MercadinhoDeposito",new Vector2(20,18)}, {"OficinaRadio",new Vector2(18,16)},
            {"CasaCostureira",new Vector2(18,18)}, {"SociedadeBeiraRio",new Vector2(24,40)}, {"AbrigoCemiterio",new Vector2(16,14)} };
        var positions=new Dictionary<string,Vector2> { {"IgrejaMatriz",new Vector2(.503f,.712f)}, {"PrefeituraSantaRita",new Vector2(.614f,.592f)},
            {"RodoviariaSantaRita",new Vector2(.609f,.451f)}, {"MercadinhoDeposito",new Vector2(.39f,.6f)}, {"OficinaRadio",new Vector2(.348f,.503f)},
            {"CasaCostureira",new Vector2(.397f,.471f)}, {"SociedadeBeiraRio",new Vector2(.317f,.334f)}, {"AbrigoCemiterio",new Vector2(.659f,.917f)} };
        Vector2 size=sizes[name];float mid=size.x*.5f;
        var titles=new Dictionary<string,string>{{"SociedadeBeiraRio","Sociedade Beira-Rio — Salão e Cancha"},{"MercadinhoDeposito","Mercadinho Santa Rita — Depósito"},
            {"IgrejaMatriz","Igreja da Matriz"},{"PrefeituraSantaRita","Prefeitura de Santa Rita"},{"RodoviariaSantaRita","Rodoviária de Santa Rita"},
            {"OficinaRadio","Oficina de Rádio — Mila"},{"CasaCostureira","Casa de Costura — Celina"},{"AbrigoCemiterio","Abrigo das Velas — Cemitério"}};
        string title=titles[name];
        Setup(name,Vector2.zero,size,title,true,positions[name],name=="SociedadeBeiraRio"?2:name=="AbrigoCemiterio"?1:0,new Vector2(mid,3));
        string piso=name=="IgrejaMatriz"?"PisoCreme":name=="PrefeituraSantaRita"?"PisoVerde":name=="RodoviariaSantaRita"?"PisoCinza":name=="MercadinhoDeposito"?"PisoCinza":"Madeira";
        RectTiles(new RectInt(0,0,(int)size.x,(int)size.y),piso);
        Walls(size);
        Spawn("centro",new Vector2(mid,3),1);
        Portal("Saida",new Vector2(mid,1.2f),"CentroSantaRita","volta_"+name,"Sair para a rua");
        Prop("Porta","DoorFrame",new Vector2(mid,.35f),1.2f,Vector2.zero);
        Light(new Vector2(mid,5),7,.9f);Light(new Vector2(mid,size.y-6),8,.9f);
        if(name=="IgrejaMatriz")
        {
            Solid("PassadeiraNave",new Vector2(11,11),new Vector2(4,18),new Color(.43f,.16f,.12f),-2400);
            Solid("BordaPassadeira",new Vector2(8.95f,11),new Vector2(.08f,18),new Color(.67f,.49f,.22f),-2300);
            Solid("BordaPassadeira",new Vector2(13.05f,11),new Vector2(.08f,18),new Color(.67f,.49f,.22f),-2300);
            for(int y=8;y<=17;y+=4) { Prop("BancoEsq_"+y,"BancoIgreja",new Vector2(6,y),1,new Vector2(3.7f,.65f));Prop("BancoDir_"+y,"BancoIgreja",new Vector2(16,y),1,new Vector2(3.7f,.65f)); }
            Solid("DegrauAltar",new Vector2(11,20),new Vector2(12,2),new Color(.42f,.38f,.29f),-2300);
            Prop("Altar","Altar",new Vector2(11,21),1.35f,new Vector2(4.8f,1));
            NPC("bento","Npc_bento",new Vector2(11,18.8f),"padre");
            Prop("Velas","ArmarioVelas",new Vector2(3.4f,21),.9f,new Vector2(2.8f,.7f));Interacao("velas_igreja",new Vector2(3.4f,20));
            Prop("MuralComunitario","Memorial",new Vector2(18.7f,21.5f),.8f,new Vector2(3,.5f));Interacao("foto_igreja",new Vector2(18.7f,20.5f));
            LabelWorld("NAVE • ABERTA PARA A ESPERA",new Vector2(11,24),new Vector2(10,.7f));
        }
        else if(name=="PrefeituraSantaRita")
        {
            Prop("BalcaoConsulta","MesaPrefeitura",new Vector2(12,9),1.15f,new Vector2(4.9f,1));
            NPC("clarice","Npc_clarice",new Vector2(12,10.9f),"prefeitura",false,new Vector2(12,7.8f));
            Interacao("protocolo",new Vector2(9.6f,8));Prop("Protocolo","Bilhete",new Vector2(9.6f,9.7f),.7f,Vector2.zero);
            Wall(new Vector2(5.1f,13),new Vector2(10.2f,.6f));Wall(new Vector2(18.9f,13),new Vector2(10.2f,.6f));
            var porta=Solid("PortaConsultaPublica",new Vector2(12,13),new Vector2(3.6f,.55f),new Color(.22f,.32f,.3f),500);
            Box(porta,Vector2.one,Vector2.zero);porta.AddComponent<Capitulo2Gate>().flag="c2_prefeitura_atendimento";
            for(int x=3;x<=21;x+=6)Prop("Arquivo_"+x,"Arquivo",new Vector2(x,18.5f),1,new Vector2(1.5f,.9f));
            Prop("MesaLeitura","WoodTable_Worn",new Vector2(12,17),1.4f,new Vector2(2.8f,1));
            Interacao("manutencao",new Vector2(10.6f,15.6f));Interacao("extrato_publico",new Vector2(13.4f,15.6f));
            Prop("BancoEspera","BancoRodoviaria",new Vector2(4,5),.8f,new Vector2(3,.7f));
            Prop("BancoEsperaDireita","BancoRodoviaria",new Vector2(20,5),.8f,new Vector2(3,.7f));
            Prop("PlantaRecepcao","PlanterBox",new Vector2(3,10.5f),.8f,new Vector2(1.2f,.5f));
            Prop("PlantaRecepcao","PlanterBox",new Vector2(21,10.5f),.8f,new Vector2(1.2f,.5f));
            Prop("ArquivoDeConsulta","Arquivo",new Vector2(4.5f,10.8f),.8f,new Vector2(1.2f,.6f));
            LabelWorld("CONSULTA PÚBLICA • PLANTÃO",new Vector2(12,20.8f),new Vector2(10,.7f));
        }
        else if(name=="RodoviariaSantaRita")
        {
            foreach(int y in new[]{6,10,14})Prop("Espera_"+y,"BancoRodoviaria",new Vector2(6,y),1,new Vector2(4.1f,.7f));
            Prop("Guiche","Guiche",new Vector2(18,12),1.3f,new Vector2(5.2f,1));
            NPC("dario","Npc_dario",new Vector2(18,14),"dario",false,new Vector2(18,10.8f));
            Prop("Horarios","Memorial",new Vector2(22.8f,16),.65f,new Vector2(2.6f,.5f));Interacao("horarios",new Vector2(23.2f,15));
            Prop("TelefoneGuiche","TelefoneForaDoGancho",new Vector2(23.3f,9.7f),1,Vector2.zero);Interacao("telefone_rodo",new Vector2(23.3f,9));
            Prop("Bagagem","CardboardBox_Single",new Vector2(3,5),1,Vector2.zero);Interacao("bagagem",new Vector2(3,4.4f));
            LabelWorld("EMBARQUES • INFORMAÇÕES",new Vector2(13,18.5f),new Vector2(12,.7f));
        }
        else if(name=="MercadinhoDeposito")
        {
            for(int y=5;y<=12;y+=3)Prop("Pallet_"+y,"Caixotes",new Vector2(3.5f,y),1.1f,new Vector2(2.3f,1));
            Prop("EstoqueMesmaLoja","BoxesShelf",new Vector2(16,10),1.6f,new Vector2(2.4f,1));
            Prop("RomaneioMesa","WoodTable_Worn",new Vector2(10,10),1.2f,new Vector2(2.6f,.9f));Interacao("romaneio",new Vector2(10,8.8f));
            Interacao("atendente_fundos",new Vector2(10,13.7f));
            Portal("LojaOriginal",new Vector2(10,15.6f),"EntradaDaCidade","retorno_deposito","Entrar na mesma loja pela porta de serviço",false,Flags.ZeDerrotadoNaSinuca);
            Spawn("loja",new Vector2(10,13),0);
            LabelWorld("MERCADINHO SANTA RITA • ESTOQUE",new Vector2(10,16.6f),new Vector2(14,.6f));
        }
        else if(name=="OficinaRadio")
        {
            Prop("BancadaEletronica","BancadaRadio",new Vector2(9,9.6f),1,new Vector2(4.5f,1));Interacao("radio_test",new Vector2(9,8.4f));
            NPC("mila","Npc_mila",new Vector2(5,8.7f),"mila");
            Prop("DesenhoGuardado","Cabinet_Medium",new Vector2(14,11),1.2f,new Vector2(1.8f,.8f));Interacao("desenho_radio",new Vector2(14,10));
            Prop("RadiosEmReparo","Radio",new Vector2(14,6),1.3f,new Vector2(.8f,.5f));
            Prop("BancoOficina","FoldingChair",new Vector2(4,5),1,new Vector2(.6f,.6f));
            Prop("ComponentesSecos","BoxesShelf",new Vector2(3.2f,12.1f),1.2f,new Vector2(1.8f,.8f));
            Prop("AparelhoNaCaixa","CardboardBox_Single",new Vector2(14.5f,3.5f),1.2f,new Vector2(1,.7f));
            LabelWorld("TESTE DE RETORNO → FAIXA LOCAL",new Vector2(9,14.5f),new Vector2(14,.6f));
        }
        else if(name=="CasaCostureira")
        {
            Prop("MaquinaCelina","MaquinaCostura",new Vector2(6,9),1.1f,new Vector2(3.8f,1));
            NPC("celina","Npc_celina",new Vector2(12,9),"celina");Interacao("encomenda_faixa",new Vector2(6,7.7f));
            Prop("Tecidos","Wardrobe_Medium",new Vector2(4,13),1.4f,new Vector2(2,1));
            Prop("Comoda","Cabinet_Medium",new Vector2(13,13),1.3f,new Vector2(2,.8f));
            Prop("RetratoSala","FramedPhoto",new Vector2(9,16),1.3f,Vector2.zero);
            Prop("PoltronaCelina","Armchair",new Vector2(13,5),1.2f,new Vector2(1.2f,.8f));
            Prop("CadeiraDeProva","FoldingChair",new Vector2(4.5f,4.7f),1,new Vector2(.6f,.6f));
            Prop("CaixaDeRetalhos","CardboardBox_Single",new Vector2(3,7.1f),1,new Vector2(.8f,.6f));
            Solid("TapeteCostura",new Vector2(9,6),new Vector2(6,3),new Color(.36f,.18f,.18f),-2400);
            LabelWorld("COSTURA • ENCOMENDAS",new Vector2(9,16.7f),new Vector2(12,.6f));
        }
        else if(name=="SociedadeBeiraRio")
        {
            RectTiles(new RectInt(9,16,6,20),"Areia");
            Wall(new Vector2(8.7f,26),new Vector2(.4f,20));Wall(new Vector2(15.3f,26),new Vector2(.4f,20));
            Solid("LinhaDeLancamento",new Vector2(12,16.5f),new Vector2(6,.08f),new Color(.85f,.78f,.57f),-2200);
            foreach(float y in new[]{21f,26f,31f})
                Solid("ReferenciaGastaDaCancha",new Vector2(12,y),new Vector2(5.8f,.035f),new Color(.66f,.59f,.43f,.4f),-2200);
            Prop("Rack","RackBocha",new Vector2(19,17),1,new Vector2(3,.8f));Interacao("regra_bocha",new Vector2(19,15.8f));
            Prop("PlacarFisico","PlacarBocha",new Vector2(19,34),1,new Vector2(3,.6f));
            foreach(int y in new[]{20,27,33})Prop("BancoEspectadores_"+y,"Bench_Wood",new Vector2(4,y),1,new Vector2(2,.6f));
            NPC("tonao","Npc_tonao",new Vector2(19,24),"tonao");Interacao("bocha",new Vector2(12,14.8f));
            Prop("MesaConvivio","MesaSociedade",new Vector2(5,7),1,new Vector2(4.2f,1.1f));
            NPC("nair","Npc_nair",new Vector2(18,8),"nair");
            Prop("CadernoConvivio","WoodTable_Worn",new Vector2(20.4f,5.6f),.9f,new Vector2(1.9f,.7f));Interacao("visitas_sociedade",new Vector2(20.4f,4.6f));
            Prop("BancadaTonao","BancadaManutencao",new Vector2(4,13),1,new Vector2(3.7f,1));Interacao("cordas",new Vector2(4,11.8f));
            Light(new Vector2(12,27),9,1);Light(new Vector2(12,36),7,.9f);
            LabelWorld("CANCHA • SOCIEDADE BEIRA-RIO",new Vector2(12,38.5f),new Vector2(16,.7f));
        }
        else
        {
            Prop("VelasAbrigo","ArmarioVelas",new Vector2(4,9),1,new Vector2(3,.8f));Interacao("apoio_cemiterio",new Vector2(4,7.8f));
            Prop("CadernoAbrigo","WoodTable_Worn",new Vector2(9,8),1.2f,new Vector2(2.8f,.9f));Interacao("caderno_velas",new Vector2(9,6.7f));
            Prop("BancoAbrigo","Bench_Wood",new Vector2(12,4),1,new Vector2(1.7f,.6f));
            Prop("CaixaVelas","CardboardBox_Single",new Vector2(3,3.5f),1.2f,new Vector2(1,.7f));
            Prop("ArmarioDeApoio","Cabinet_Medium",new Vector2(12.5f,9.5f),1.1f,new Vector2(1.8f,.7f));
            LabelWorld("APOIO DO CEMITÉRIO",new Vector2(8,12.5f),new Vector2(12,.6f));
        }
        foreach(float x in new[]{2f,size.x-2})Prop("Janela_"+x,"Window_Curtain",new Vector2(x,size.y-1.2f),1.1f,Vector2.zero);
    }

    private static void Building(string art, Vector2 pos, string sign, string destination, Vector2 returnSpawn)
    {
        Sprite s=Sprite(art);float width=s!=null?s.bounds.size.x:8;float height=s!=null?s.bounds.size.y:7;
        Prop(art,art,pos,1,new Vector2(width*.86f,height*.5f));
        RectTiles(new RectInt(Mathf.FloorToInt(pos.x-width*.6f),Mathf.FloorToInt(pos.y-2),Mathf.CeilToInt(width*1.2f),2),"Calcada");
        LabelWorld(sign,pos+Vector2.up*(height*.3f),new Vector2(width*.82f,.55f));
        Portal("Porta_"+destination,pos+Vector2.down*.55f,destination,"centro","Entrar — "+sign);
        Spawn("volta_"+destination,returnSpawn,0);Light(pos+Vector2.up,6,.8f);
        Garden(pos+new Vector2(-width*.64f,.2f),new Vector2(1,1.4f));Garden(pos+new Vector2(width*.64f,.2f),new Vector2(1,1.4f));
    }

    private static void Walls(Vector2 size)
    {
        Wall(new Vector2(.35f,size.y*.5f),new Vector2(.7f,size.y));Wall(new Vector2(size.x-.35f,size.y*.5f),new Vector2(.7f,size.y));
        Wall(new Vector2(size.x*.5f,size.y-1),new Vector2(size.x,2));
        float part=(size.x-3.2f)*.5f;Wall(new Vector2(part*.5f,.3f),new Vector2(part,.6f));Wall(new Vector2(size.x-part*.5f,.3f),new Vector2(part,.6f));
    }
    private static void Wall(Vector2 pos,Vector2 size)
    {
        var g=Solid("Parede",pos,size,new Color(.59f,.57f,.49f),2000);Box(g,Vector2.one,Vector2.zero);
        if(size.x>size.y)
        {
            Solid("RodapeMadeira",pos+Vector2.down*(size.y*.5f-.06f),new Vector2(size.x,.12f),new Color(.23f,.18f,.13f),2001);
            Solid("CapaParede",pos+Vector2.up*(size.y*.5f-.05f),new Vector2(size.x,.1f),new Color(.3f,.31f,.27f),2002);
        }
    }

    private static GameObject Prop(string name,string art,Vector2 pos,float scale,Vector2 collision)
    {
        var go=new GameObject(name);go.transform.SetParent(world,false);go.transform.position=pos;go.transform.localScale=Vector3.one*scale;
        var sr=go.AddComponent<SpriteRenderer>();sr.sprite=Sprite(art);sr.sortingOrder=Depth(pos.y);
        if(sr.sprite==null)throw new Exception("Asset C2 ausente: "+art);
        if(collision.sqrMagnitude>0)Box(go,collision/scale,new Vector2(0,collision.y*.5f/scale));return go;
    }
    private static Sprite Sprite(string name)
    {
        if(sprites.TryGetValue(name,out var s))return s;
        s=AssetDatabase.LoadAssetAtPath<Sprite>(Root+"/"+name+".png");
        if(s==null)
        {
            string original="Assets/Resources/Cap1/"+name+".png";
            if(!File.Exists(original))original="Assets/Sprites/Capitulo1/"+name+".png";
            if(File.Exists(original))
            {
                // As texturas de C1 são fatiadas em múltiplos sprites; carregar o
                // primeiro subasset cortava a arte. Cópia integral com importador
                // próprio mantém C1 intacto e torna a vista de C2 reproduzível.
                Directory.CreateDirectory(Root+"/Legado");string copy=Root+"/Legado/"+name+".png";
                if(!File.Exists(copy))File.Copy(original,copy);
                AssetDatabase.ImportAsset(copy);
                var importer=(TextureImporter)AssetImporter.GetAtPath(copy);
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
                importer.spritePixelsPerUnit=name.StartsWith("Npc_")?32:16;
                importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;
                var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);settings.spriteAlignment=(int)SpriteAlignment.Custom;
                settings.spritePivot=name.StartsWith("Npc_")?new Vector2(.5f,.125f):new Vector2(.5f,0);importer.SetTextureSettings(settings);importer.SaveAndReimport();
                s=AssetDatabase.LoadAssetAtPath<Sprite>(copy);
            }
        }
        sprites[name]=s;return s;
    }
    private static int Depth(float y)=>1000-Mathf.RoundToInt(y*10);
    private static void NPC(string name,string art,Vector2 pos,string id,bool legacy=false,Vector2? interaction=null)
    {Prop("NPC_"+name,art,pos,1,new Vector2(.6f,.45f));Interacao(id,interaction??pos+Vector2.down*.55f);}
    private static void Interacao(string id,Vector2 pos)
    {
        var go=new GameObject("Examinar_"+id);go.transform.SetParent(world,false);go.transform.position=pos;
        var item=go.AddComponent<Capitulo2Interactable>();item.interactionId=id;var data=Capitulo2Director.Dados(id);
        if(data==null)throw new Exception("Conteúdo C2 ausente: "+id);
        if(!string.IsNullOrEmpty(data.art))item.arteExame=Sprite(data.art);
    }
    private static void Portal(string name,Vector2 pos,string dest,string spawn,string prompt,bool auto=false,string flag=null)
    {
        var go=new GameObject("Porta_"+name);go.transform.SetParent(world,false);go.transform.position=pos;
        var portal=go.AddComponent<Capitulo2Portal>();portal.cenaDestino=dest;portal.entradaDestino=spawn;portal.textoPrompt=prompt;portal.automatico=auto;
        portal.flagNecessaria=flag;portal.avisoBloqueio="A passagem da Rua Norte ainda depende da conversa com Zé.";
        if(auto){var col=go.AddComponent<BoxCollider2D>();col.isTrigger=true;col.size=new Vector2(2.5f,2.5f);}
        connections.Add(currentSceneName+","+name+","+dest+","+spawn+","+pos.x.ToString(System.Globalization.CultureInfo.InvariantCulture)+","+pos.y.ToString(System.Globalization.CultureInfo.InvariantCulture)+","+flag);
    }
    private static void Spawn(string id,Vector2 pos,int dir)
    {var go=new GameObject("Spawn_"+id);go.transform.SetParent(world,false);go.transform.position=pos;go.AddComponent<SpawnPoint>().Configurar(id,dir);}
    private static void Phone(string id,Vector2 pos)
    {
        var go=Prop(id,"PhoneBooth",pos,1,new Vector2(.8f,.55f));var phone=go.AddComponent<PayphoneCheckpoint>();phone.ConfigurarId(id);
        var so=new SerializedObject(phone);var falas=so.FindProperty("falas");falas.arraySize=3;
        falas.GetArrayElementAtIndex(0).stringValue="A ligação completa. Organizo as pistas de Adílio antes de desligar.";
        falas.GetArrayElementAtIndex(1).stringValue="A casa do vô continua sem resposta. A linha da cidade está funcionando.";
        falas.GetArrayElementAtIndex(2).stringValue="O plantão recebeu meu relato pela Rodoviária. Aqui posso registrar a investigação no orelhão.";
        so.ApplyModifiedPropertiesWithoutUndo();Light(pos+Vector2.up,4,.65f);
    }
    private static void Sign(string text,Vector2 pos)
    {Prop("Placa_"+text,"ArrowSign",pos,1,Vector2.zero);LabelWorld(text,pos+Vector2.up*1.6f,new Vector2(6,.5f));}
    private static void LabelWorld(string text,Vector2 pos,Vector2 size)
    {
        var go=new GameObject("Letreiro_"+text);go.transform.SetParent(world,false);go.transform.position=pos;
        var t=go.AddComponent<TextMeshPro>();t.text=text;t.font=UIFactory.Font;t.fontSize=3.5f;t.alignment=TextAlignmentOptions.Center;
        t.color=new Color(.95f,.89f,.69f);t.rectTransform.sizeDelta=size;t.textWrappingMode=TextWrappingModes.Normal;
        t.GetComponent<MeshRenderer>().sortingOrder=Depth(pos.y)+90;
    }
    private static void Lamp(Vector2 pos){Prop("Poste","StreetLamp",pos,1,new Vector2(.35f,.35f));Light(pos+Vector2.up*1.8f,6,.8f);}
    private static void Light(Vector2 pos,float radius,float intensity)
    {var go=new GameObject("LuzQuente");go.transform.SetParent(world,false);go.transform.position=pos;var l=go.AddComponent<Light2D>();l.lightType=Light2D.LightType.Point;l.pointLightOuterRadius=radius;l.intensity=intensity;l.color=new Color(1,.82f,.55f);}
    private static void Garden(Vector2 pos,Vector2 size)
    {Solid("Canteiro",pos,size,new Color(.25f,.31f,.17f),-2400);Prop("Flores","PlanterBox",pos,.9f,Vector2.zero);}
    private static void Fence(Vector2 a,Vector2 b)
    {
        float length=Vector2.Distance(a,b);int count=Mathf.CeilToInt(length/1.8f);
        for(int i=0;i<count;i++)
        {var pos=Vector2.Lerp(a,b,(i+.5f)/count);var go=Prop("Cerca","WoodFence",pos,1,new Vector2(1.9f,.35f));go.transform.rotation=Quaternion.Euler(0,0,Mathf.Atan2(b.y-a.y,b.x-a.x)*Mathf.Rad2Deg);}
    }
    private static void BoundaryForest()
    {
        // Cercas de propriedades/vegetação acompanham a margem de produção;
        // não deixam Marcus atravessar o terreno e sair da câmera para o vazio.
        // Noroeste volta ao Bar aprovado por sua conexão, sem duplicar a área C1.
        Vector2[] perimeter={new Vector2(-19,0),new Vector2(-19,75),new Vector2(22,75),new Vector2(22,85),new Vector2(40,85),new Vector2(40,110),
            new Vector2(94,110),new Vector2(94,94),new Vector2(88,75),new Vector2(78.5f,56),new Vector2(82,37),new Vector2(84.5f,18),new Vector2(80.5f,5),new Vector2(80,-7)};
        for(int i=1;i<perimeter.Length;i++)Fence(perimeter[i-1],perimeter[i]);
        for(int y=3;y<75;y+=4)Tree(new Vector2(-18,y));
        for(int x=42;x<94;x+=4)Tree(new Vector2(x,109));
        for(int i=6;i<perimeter.Length;i++)
            for(float t=0;t<1;t+=.2f)Tree(Vector2.Lerp(perimeter[i-1],perimeter[i],t)+Vector2.right*1.2f);
        for(int y=89;y<=105;y+=5)for(int x=-14;x<=34;x+=6)Tree(new Vector2(x+Mathf.Sin(x*.7f+y*.3f)*1.5f,y+Mathf.Sin(x*.31f)*1.3f));
        foreach(var p in new[]{new Vector2(37,74),new Vector2(22,66),new Vector2(43,82),new Vector2(85,77),new Vector2(81,65),new Vector2(75,15),new Vector2(30,9),new Vector2(20,4),new Vector2(-4,19),new Vector2(2,32),new Vector2(4,48),new Vector2(83,7)})Tree(p);
        // Matas já ilustradas a oeste e na margem: ocupam os terrenos entre
        // equipamentos, sem inventar outro bairro ou mudar os marcos do mapa.
        var rng=new System.Random(20040317);
        var occupied=world.GetComponentsInChildren<SpriteRenderer>().Select(s=>(Vector2)s.transform.position).ToList();
        foreach(var patch in new[]{new Rect(-14,20,19,45),new Rect(6,57,12,15),new Rect(25,3,11,14),new Rect(52,4,20,10),new Rect(44,74,10,9)})
            for(int i=0;i<180;i++)
            {
                var p=new Vector2(patch.x+(float)rng.NextDouble()*patch.width,patch.y+(float)rng.NextDouble()*patch.height);
                bool grass=true;
                for(int y=-1;y<=1;y++)for(int x=-1;x<=1;x++)
                    if(ground.GetTile(new Vector3Int(Mathf.FloorToInt(p.x)+x,Mathf.FloorToInt(p.y)+y,0))!=Tile("Grama"))grass=false;
                if(!grass||occupied.Any(q=>Vector2.Distance(q,p)<3.1f))continue;
                Tree(p);occupied.Add(p);
            }
    }
    private static void Tree(Vector2 p){Prop("Arvore","Oak_Tree",p,.9f,new Vector2(1,.8f));}
    private static void Road(Vector2 a,Vector2 b,int width,bool dirt=false)
    {
        Vector2 delta=b-a;float length=delta.magnitude;Vector2 direction=delta/Mathf.Max(.001f,length);
        Vector2 normal=new Vector2(-direction.y,direction.x);float radius=width*.5f;
        bool horizontal=Mathf.Abs(delta.x)>Mathf.Abs(delta.y);
        // Rasteriza cada célula uma única vez. Endcaps de carimbos sucessivos
        // não podem reescrever o asfalto anterior com calçada.
        int minX=Mathf.FloorToInt(Mathf.Min(a.x,b.x)-radius-1),maxX=Mathf.CeilToInt(Mathf.Max(a.x,b.x)+radius+1);
        int minY=Mathf.FloorToInt(Mathf.Min(a.y,b.y)-radius-1),maxY=Mathf.CeilToInt(Mathf.Max(a.y,b.y)+radius+1);
        for(int y=minY;y<=maxY;y++)for(int x=minX;x<=maxX;x++)
        {
            Vector2 p=new Vector2(x,y);float t=Mathf.Clamp(Vector2.Dot(p-a,direction),0,length);
            float distance=Vector2.Distance(p,a+direction*t);if(distance>radius+1)continue;
            var cell=new Vector3Int(x,y,0);var previous=ground.GetTile(cell);
            bool asphalt=previous==Tile("Asfalto")||previous==Tile("AsfaltoH")||previous==Tile("AsfaltoV");
            if(asphalt&&(dirt||distance>radius))continue;
            bool center=Mathf.Abs(Vector2.Dot(p-a,normal))<.5f;
            ground.SetTile(cell,Tile(distance>radius?"Calcada":dirt?"Terra":center?(horizontal?"AsfaltoH":"AsfaltoV"):"Asfalto"));
        }
    }
    private static void Crosswalk(int x,int y,bool vertical)
    {for(int i=0;i<5;i++)Solid("FaixaPedestre",new Vector2(x-2+i,y+.8f),new Vector2(.45f,3.4f),new Color(.78f,.78f,.66f),-2100);}
    private static void RectTiles(RectInt rect,string type)
    {for(int y=rect.yMin;y<rect.yMax;y++)for(int x=rect.xMin;x<rect.xMax;x++)ground.SetTile(new Vector3Int(x,y,0),Tile(type));}
    private static GameObject Solid(string name,Vector2 pos,Vector2 size,Color color,int order)
    {
        var go=new GameObject(name);go.transform.SetParent(world,false);go.transform.position=pos;go.transform.localScale=new Vector3(size.x,size.y,1);
        var sr=go.AddComponent<SpriteRenderer>();sr.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Producao/White.png")??Tile("PisoCreme").sprite;sr.color=color;sr.sortingOrder=order;return go;
    }
    private static void Box(GameObject go,Vector2 size,Vector2 offset)
    {var col=go.AddComponent<BoxCollider2D>();col.size=size;col.offset=offset;}
    private static Tile Tile(string type)
    {
        if(tiles.TryGetValue(type,out var cached))return cached;
        string path=Root+"/Tiles/"+type+".asset";var tile=AssetDatabase.LoadAssetAtPath<Tile>(path);
        if (tile != null && File.Exists(Root+"/Tiles/"+type+".aseprite"))
        { tiles[type]=tile; return tile; } // Fontes artísticas editáveis prevalecem sobre o protótipo procedural.
        // Mantém GUIDs dos Tile assets ao refazer as texturas originais de C2.
        {
            var colors=new Dictionary<string,Color32>{{"Grama",new Color32(50,67,43,255)},{"Terra",new Color32(100,78,53,255)},
                {"Calcada",new Color32(132,128,109,255)},{"Asfalto",new Color32(49,53,54,255)},{"AsfaltoH",new Color32(49,53,54,255)},{"AsfaltoV",new Color32(49,53,54,255)},
                {"Pedra",new Color32(108,111,102,255)},{"Agua",new Color32(23,49,61,255)},{"Madeira",new Color32(103,72,48,255)},
                {"PisoVerde",new Color32(98,116,105,255)},{"PisoCreme",new Color32(173,157,126,255)},{"PisoCinza",new Color32(106,111,105,255)},{"Areia",new Color32(142,116,77,255)}};
            var tex=new Texture2D(32,32,TextureFormat.RGBA32,false);var c=colors[type];
            int seed=17;foreach(char letter in type)seed=unchecked(seed*31+letter);var rng=new System.Random(seed);
            for(int y=0;y<32;y++)for(int x=0;x<32;x++)
            {
                int n=rng.Next(-2,3);
                if(type=="Madeira"&&(y%8==0||x==((y/8)%2)*16))n-=15;
                if(type=="Pedra"&&(y%8==0||(x+(y/8%2)*4)%12==0))n-=16;
                if(type.StartsWith("Piso")&&(x==0||y==0))n-=12;
                Color32 pixel=new Color32((byte)Mathf.Clamp(c.r+n,0,255),(byte)Mathf.Clamp(c.g+n,0,255),(byte)Mathf.Clamp(c.b+n,0,255),255);
                if(type=="AsfaltoH"&&y>=15&&y<=16&&x<21)pixel=new Color32(182,155,75,255);
                if(type=="AsfaltoV"&&x>=15&&x<=16&&y<21)pixel=new Color32(182,155,75,255);
                if(type=="Agua"&&((x+y*3)%29==0))pixel=new Color32(53,79,86,255);
                tex.SetPixel(x,y,pixel);
            }
            tex.Apply();string png=Root+"/Tiles/"+type+".png";File.WriteAllBytes(png,tex.EncodeToPNG());Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(png);var importer=(TextureImporter)AssetImporter.GetAtPath(png);
            importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.spritePixelsPerUnit=32;
            importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();
            bool novo=tile==null;if(novo)tile=ScriptableObject.CreateInstance<Tile>();tile.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(png);
            tile.colliderType=type=="Agua"?UnityEngine.Tilemaps.Tile.ColliderType.Grid:UnityEngine.Tilemaps.Tile.ColliderType.None;
            if(novo)AssetDatabase.CreateAsset(tile,path);else EditorUtility.SetDirty(tile);
        }
        tiles[type]=tile;return tile;
    }
}

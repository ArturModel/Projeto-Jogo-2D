using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Object=UnityEngine.Object;

// Autoria explícita e restrita aos interiores C2. Conserva cenas, GUIDs, rig, mapa e dados.
public static partial class SantaRitaC2Consolidation
{
    static Transform root;
    static Material lit;
    static Sprite white, shadow;
    static Tilemap ground;
    static readonly string[] rooms={"SociedadeBeiraRio","IgrejaMatriz","PrefeituraSantaRita","RodoviariaSantaRita","MercadinhoDeposito","OficinaRadio","CasaCostureira","AbrigoCemiterio"};
    [MenuItem("Santa Rita/C2 R2/1 Preparar arte e Sociedade")]
    public static void Pilot(){PrepareArt();Apply(new[]{rooms[0]});EncloseCourt();}

    [MenuItem("Santa Rita/Bocha/Integrar sala fechada à Sociedade")]
    public static void EncloseCourt()
    {
        Safe();var setup=EditorSceneManager.GetSceneManagerSetup();
        try
        {
            var scene=EditorSceneManager.OpenScene("Assets/Scenes/Cidade/SociedadeBeiraRio.unity");
            lit=Resources.Load<Material>("Art/WorldLit");
            white=AssetDatabase.FindAssets("White t:Texture2D").Select(AssetDatabase.GUIDToAssetPath).SelectMany(AssetDatabase.LoadAllAssetsAtPath).OfType<Sprite>().First();
            ground=GameObject.Find("Terreno")?.GetComponent<Tilemap>()??Object.FindAnyObjectByType<Tilemap>();
            // Atualização dirigida: conserva os objetos e GUIDs da composição R2.
            var old=GameObject.Find("SalaCanchaFechada");if(old!=null)Object.DestroyImmediate(old);
            root=new GameObject("SalaCanchaFechada").transform;root.SetParent(Object.FindAnyObjectByType<Capitulo2Director>().transform,false);
            Tile Surface(string key)
            {
                string path="Assets/Resources/Bocha/"+key+".png";var importer=(TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.spritePixelsPerUnit=32;
                importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();
                string asset="Assets/Art/Capitulo2/Tiles/"+key+".asset";var tile=AssetDatabase.LoadAssetAtPath<Tile>(asset);
                if(tile==null){tile=ScriptableObject.CreateInstance<Tile>();AssetDatabase.CreateAsset(tile,asset);}
                tile.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);tile.colliderType=Tile.ColliderType.None;tile.transform=Matrix4x4.Rotate(Quaternion.Euler(0,0,key=="MadeiraCancha"?90:0));EditorUtility.SetDirty(tile);return tile;
            }
            var sand=Surface("SaibroCancha");var wood=Surface("MadeiraCancha");
            var lampImport=(TextureImporter)AssetImporter.GetAtPath("Assets/Resources/Bocha/LuminariaCancha.png");lampImport.textureType=TextureImporterType.Sprite;lampImport.spriteImportMode=SpriteImportMode.Single;lampImport.spritePixelsPerUnit=32;
            lampImport.filterMode=FilterMode.Point;lampImport.mipmapEnabled=false;lampImport.textureCompression=TextureImporterCompression.Uncompressed;lampImport.SaveAndReimport();
            for(int x=3;x<30;x++)for(int y=8;y<17;y++)ground.SetTile(new Vector3Int(x,y,0),wood);
            CompactCourt(sand);
            // Abertura real de 1,8 m entre salão e cabeceira; restante é parede visível.
            void Partition(float from,float to)
            {
                Solid("ParedeDivisoriaCancha",new Vector2((from+to)/2,7.8f),new Vector2(to-from,.35f),new Color(.25f,.19f,.12f),922,true);
                var face=Empty("RebocoELambrilDivisoria",new Vector2((from+to)/2,7));var sr=face.AddComponent<SpriteRenderer>();
                sr.sprite=Art("RebocoLambril");sr.sharedMaterial=lit;sr.drawMode=SpriteDrawMode.Tiled;sr.size=new Vector2(to-from,1.5f);sr.sortingOrder=923;
                Solid("CapeamentoDivisoria",new Vector2((from+to)/2,7.8f),new Vector2(to-from,.12f),new Color(.48f,.34f,.19f),924);
            }
            Partition(3.2f,3.8f);Partition(5.6f,29.6f);
            Solid("ParedeCabeceiraSala",new Vector2(3.2f,12.55f),new Vector2(.38f,9.5f),new Color(.40f,.38f,.31f),780,true);
            Solid("LambrilCabeceira",new Vector2(3.42f,12.55f),new Vector2(.12f,9.5f),new Color(.28f,.18f,.10f),781);
            DrawCourtStair();
            var benches=Object.FindObjectsByType<SpriteRenderer>().Where(s=>s.name=="BancoDaCancha").OrderBy(s=>s.transform.position.x).ToArray();
            for(int i=0;i<benches.Length;i++){benches[i].transform.position=new Vector2(new[]{12f,20f,26f}[i%3],16.05f);benches[i].sortingOrder=839;}
            void Move(string name,float x,float y,float width=0)
            {
                var g=GameObject.Find(name);if(g==null)throw new Exception("Peça da Sociedade ausente: "+name);
                g.transform.position=new Vector2(x,y);var sr=g.GetComponent<SpriteRenderer>();if(sr!=null){if(width>0)g.transform.localScale=Vector3.one*(width/sr.sprite.bounds.size.x);sr.sortingOrder=1000-Mathf.RoundToInt(y*10);}
            }
            Move("SuporteDeBolas",4.0f,15.7f,1.7f);Move("BancadaTonao",4.0f,10.1f,1.6f);Move("Examinar_regra_bocha",4.5f,14.8f);Move("Examinar_cordas",4.65f,9.5f);
            Move("Examinar_bocha",5.1f,11.5f);Move("BancoTonao",2.0f,6.2f);Move("PlacarFisico",26.5f,16.6f);GameObject.Find("PlacarFisico").GetComponent<SpriteRenderer>().sortingOrder=710;
            // Parede ao fundo fica atrás de quem está à sua frente. Evita cortar
            // a cabeça de Marcus no corredor, sem mudar a colisão da parede.
            foreach(var sr in Object.FindObjectsByType<SpriteRenderer>().Where(s=>new[]{"ParedeNorte","RebocoELambril","LambrilNorte","JuntaLambril","FrisoNorte","JanelaOeste","JanelaLeste"}.Contains(s.name)))
                sr.sortingOrder=sr.name=="ParedeNorte"?700:sr.name=="LambrilNorte"?701:sr.name=="JuntaLambril"?702:sr.name=="FrisoNorte"?703:sr.name=="RebocoELambril"?704:708;
            var tonao=GameObject.Find("NPC_tonao");tonao.transform.position=new Vector2(4.9f,12.8f);
            var box=tonao.GetComponent<BoxCollider2D>();box.size=new Vector2(.5f,.35f)/tonao.transform.localScale.x;box.offset=new Vector2(0,.18f)/tonao.transform.localScale.x;
            var routine=tonao.GetComponent<SantaRitaNpcWork>();Vector2[] stops={new Vector2(4.9f,15.5f),new Vector2(4.9f,10.35f),new Vector2(5,12),new Vector2(5.9f,9.7f)};
            for(int i=0;i<routine.stations.Length;i++)routine.stations[i].anchor.position=stops[i%stops.Length];
            var nair=GameObject.Find("NPC_nair");var nb=nair.GetComponent<BoxCollider2D>();nb.size=new Vector2(.5f,.35f)/nair.transform.localScale.x;nb.offset=new Vector2(0,.18f)/nair.transform.localScale.x;
            var nr=nair.GetComponent<SantaRitaNpcWork>();if(nr.stations.Length>1)nr.stations[1].anchor.position=new Vector2(3.8f,5.2f);
            foreach(float x in new[]{10f,19f,26f})
            {
                Prop("PilarDaSalaCancha","PilarMadeira",x,16.65f,.28f);
                var lamp=Empty("ArandelaDaSalaCancha",new Vector2(x,17.2f));var sr=lamp.AddComponent<SpriteRenderer>();sr.sprite=Resources.Load<Sprite>("Bocha/LuminariaCancha");sr.sharedMaterial=lit;sr.sortingOrder=712;lamp.transform.localScale=Vector3.one*.7f;
            }
            Lamp(4.7f,8.8f,3,.43f);
            // Sala baixa com luz mais neutra e contida; o salão mantém seus pontos quentes.
            foreach(var lamp in Object.FindObjectsByType<Light2D>().Where(l=>l.lightType==Light2D.LightType.Point&&l.transform.position.y>9))
            {lamp.color=new Color(.92f,.89f,.73f);lamp.intensity=Mathf.Min(lamp.intensity,.58f);lamp.pointLightOuterRadius=Mathf.Min(lamp.pointLightOuterRadius,4.2f);}
            for(int x=3;x<30;x++)for(int y=9;y<17;y++)
            {var cell=new Vector3Int(x,y,0);ground.SetTileFlags(cell,TileFlags.None);ground.SetColor(cell,new Color(.77f,.81f,.82f));}
            Physics2D.SyncTransforms();EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }
        finally{EditorSceneManager.RestoreSceneManagerSetup(setup);}
    }
    [MenuItem("Santa Rita/C2 R2/2 Recompor demais interiores")]
    public static void OtherRooms(){Apply(rooms.Skip(1).ToArray());}
    [MenuItem("Santa Rita/C2 R2/3 Acabamento dos exteriores")]
    public static void Exteriors()
    {
        Safe();var setup=EditorSceneManager.GetSceneManagerSetup();
        try
        {
            lit=Resources.Load<Material>("Art/WorldLit");
            white=AssetDatabase.FindAssets("White t:Texture2D").Select(AssetDatabase.GUIDToAssetPath).SelectMany(AssetDatabase.LoadAllAssetsAtPath).OfType<Sprite>().First();
            foreach(string name in new[]{"CentroSantaRita","CemiterioSantaRita"})
            {
                var scene=EditorSceneManager.OpenScene("Assets/Scenes/Cidade/"+name+".unity");var d=Object.FindAnyObjectByType<Capitulo2Director>();
                var old=GameObject.Find("AcabamentoExteriorR2");if(old!=null)Object.DestroyImmediate(old);
                root=new GameObject("AcabamentoExteriorR2").transform;
                foreach(var global in Object.FindObjectsByType<Light2D>().Where(l=>l.lightType==Light2D.LightType.Global))global.intensity=name=="CentroSantaRita"?.4f:.31f;
                if(name=="CemiterioSantaRita")
                {
                    d.memorialPercepcao=GameObject.Find("JazigoAntigoDaFamilia").GetComponent<SpriteRenderer>();
                    int index=0;
                    foreach(var grave in Object.FindObjectsByType<SpriteRenderer>().Where(s=>s.name.StartsWith("Sepultura_")).OrderBy(s=>s.transform.position.x).ThenBy(s=>s.transform.position.y))
                    {
                        int variant=index++%4;grave.sprite=Art(variant==1?"SepulturaBaixa":variant==2?"SepulturaHorizontal":"Sepultura");
                        grave.color=Color.Lerp(Color.white,new Color(.69f,.75f,.72f),variant*.13f);grave.transform.localScale=Vector3.one*((index%3==0?.91f:1)*2.1f/grave.sprite.bounds.size.x);
                    }
                    var alcina=GameObject.Find("NPC_alcina");
                    if(alcina!=null)
                    {
                        var routine=alcina.GetComponent<SantaRitaNpcRoutine>();if(routine!=null)routine.enabled=false;
                        var r=alcina.GetComponent<SantaRitaNpcWork>()??alcina.AddComponent<SantaRitaNpcWork>();
                        r.poses=AssetDatabase.LoadAllAssetsAtPath("Assets/Art/Capitulo2/Trabalho/Trabalho_alcina.png").OfType<Sprite>().OrderBy(s=>s.name).ToArray();
                        r.stations=new[]{new SantaRitaNpcWork.Station{anchor=Empty("FolhasDoCaminho",new Vector2(70,90)).transform,duration=7,working=true},new SantaRitaNpcWork.Station{anchor=Empty("RondaDoPortao",new Vector2(70,87.5f)).transform,duration=4}};
                        alcina.transform.position=new Vector2(70,90);var it=GameObject.Find("Examinar_alcina");if(it!=null){it.transform.SetParent(alcina.transform,true);it.transform.localPosition=Vector3.down*.55f;}
                    }
                    Lamp(64,85.5f,5,.75f);Lamp(80.8f,101,5,.55f);
                    // Jazigo continua onde a leitura espera; a lâmpada do portão não ilumina toda a margem.
                    var b=d.memorialPercepcao.GetComponent<BoxCollider2D>();if(b!=null&&d.memorialPercepcao.GetComponent<ShadowCaster2D>()==null)Cast(d.memorialPercepcao.gameObject,b);
                }
                else
                {
                    Prop("GrelhaDoChico","GrelhaDrenagem",37.8f,44.7f,1.25f);
                    var chico=GameObject.Find("NPC_chico");if(chico!=null)
                    {
                        chico.transform.position=new Vector2(37.2f,45);var it=GameObject.Find("Examinar_orientacao");if(it!=null){it.transform.SetParent(chico.transform,true);it.transform.localPosition=Vector3.down*.55f;}
                    }
                    // Bancos da praça olham para o caminho/fonte. Não alterar eixos de rua nem lotes.
                    foreach(var bench in Object.FindObjectsByType<SpriteRenderer>().Where(s=>s.name=="BancoPublico"))
                    {if(bench.transform.position.y<47)bench.color=new Color(.91f,.91f,.87f);}
                    ground=GameObject.Find("Terreno").GetComponent<Tilemap>();var walk=AssetDatabase.LoadAssetAtPath<Tile>("Assets/Art/Capitulo2/Tiles/Calcada.asset");
                    foreach(var p in Object.FindObjectsByType<Capitulo2Portal>().Where(p=>p.cenaDestino!="EntradaDaCidade"&&p.transform.position.y<78&&p.transform.position.x>27))
                    {
                        var cell=ground.WorldToCell(p.transform.position);
                        for(int x=-1;x<=1;x++)for(int y=-1;y<=0;y++)
                        {var at=cell+new Vector3Int(x,y,0);var tile=ground.GetTile(at);if(tile!=null&&!tile.name.Contains("Asfalto")&&!tile.name.Contains("Agua"))ground.SetTile(at,walk);}
                    }
                }
                EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
            }
        }
        finally{EditorSceneManager.RestoreSceneManagerSetup(setup);}
    }
    static void Safe()
    {if(EditorApplication.isPlaying||Enumerable.Range(0,SceneManager.sceneCount).Any(i=>SceneManager.GetSceneAt(i).isDirty))throw new Exception("Pare Play e preserve cenas não salvas antes da autoria R2.");}
    static Sprite Art(string key)
    {
        var s=Resources.Load<Sprite>("Art/Props/"+key)??Resources.Load<Sprite>("Art/World/"+key)??Resources.Load<Sprite>("Art/Characters/"+key);
        if(s!=null)return s;
        foreach(string path in new[]{"Assets/Art/Capitulo2/"+key+".png","Assets/Art/Capitulo2/Legado/"+key+".png"})
        {s=AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().FirstOrDefault();if(s!=null)return s;}
        throw new Exception("Arte R2 ausente: "+key);
    }
    static void PrepareArt()
    {
        Safe();AssetDatabase.Refresh();
        foreach(string key in new[]{"ApoioAguaQuente","BancadaBochas","SuporteBochas","BancoCancha","GrelhaDrenagem","RebocoLambril","EstoqueDeposito","SepulturaBaixa","SepulturaHorizontal"})
        {
            var imp=(TextureImporter)AssetImporter.GetAtPath("Assets/Resources/Art/Props/"+key+".png");
            imp.textureType=TextureImporterType.Sprite;imp.spriteImportMode=SpriteImportMode.Single;imp.spritePixelsPerUnit=32;
            var settings=new TextureImporterSettings();imp.ReadTextureSettings(settings);settings.spriteAlignment=9;settings.spritePivot=new Vector2(.5f,key=="RebocoLambril"?.5f:.125f);imp.SetTextureSettings(settings);
            imp.filterMode=FilterMode.Point;imp.mipmapEnabled=false;imp.textureCompression=TextureImporterCompression.Uncompressed;imp.SaveAndReimport();
        }
        foreach(string guid in AssetDatabase.FindAssets("t:Texture2D",new[]{"Assets/Art/Capitulo2/Trabalho"}))
        {
            string path=AssetDatabase.GUIDToAssetPath(guid);var imp=(TextureImporter)AssetImporter.GetAtPath(path);
            imp.textureType=TextureImporterType.Sprite;imp.spriteImportMode=SpriteImportMode.Multiple;imp.spritePixelsPerUnit=32;imp.isReadable=true;
            imp.filterMode=FilterMode.Point;imp.mipmapEnabled=false;imp.textureCompression=TextureImporterCompression.Uncompressed;
#pragma warning disable CS0618
            imp.spritesheet=Enumerable.Range(0,8).Select(i=>new SpriteMetaData{name="pose_"+i,rect=new Rect(i*64,0,64,64),pivot=new Vector2(.5f,.125f),alignment=9}).ToArray();
#pragma warning restore CS0618
            imp.SaveAndReimport();
        }
    }
    static void Apply(string[] names)
    {
        Safe();var setup=EditorSceneManager.GetSceneManagerSetup();
        try
        {
            lit=Resources.Load<Material>("Art/WorldLit");
            white=AssetDatabase.FindAssets("White t:Texture2D").Select(AssetDatabase.GUIDToAssetPath).SelectMany(AssetDatabase.LoadAllAssetsAtPath).OfType<Sprite>().FirstOrDefault();
            shadow=AssetDatabase.FindAssets("ContactShadow t:Texture2D").Select(AssetDatabase.GUIDToAssetPath).SelectMany(AssetDatabase.LoadAllAssetsAtPath).OfType<Sprite>().FirstOrDefault();
            if(white==null)throw new Exception("Sprite neutro persistente ausente.");
            foreach(string name in names)
            {
                var scene=EditorSceneManager.OpenScene("Assets/Scenes/Cidade/"+name+".unity");
                var d=Object.FindAnyObjectByType<Capitulo2Director>();Vector2 old=d.limiteMax;
                var extra=GameObject.Find("ComposicaoCotidiana");if(extra!=null)Object.DestroyImmediate(extra);
                foreach(var legacy in scene.GetRootGameObjects().Where(g=>g.name=="Passageira_Espera"||g.name=="Motorista_Escala"))Object.DestroyImmediate(legacy);
                ground=Object.FindAnyObjectByType<Tilemap>();
                foreach(Transform child in d.transform.Cast<Transform>().ToArray())if(child.name!="Piso")Object.DestroyImmediate(child.gameObject);
                root=new GameObject("PlantaFuncionalR2").transform;root.SetParent(d.transform,false);
                Vector2 size=name=="SociedadeBeiraRio"?new Vector2(30,19):name=="IgrejaMatriz"?new Vector2(18,22):name=="PrefeituraSantaRita"?new Vector2(20,17):
                    name=="RodoviariaSantaRita"?new Vector2(22,15):name=="MercadinhoDeposito"?new Vector2(16,14):name=="OficinaRadio"?new Vector2(16,13):name=="CasaCostureira"?new Vector2(16,14):new Vector2(13,11);
                Vector2 entry=new Vector2(name=="SociedadeBeiraRio"?5:size.x/2,2.4f);
                d.limiteMin=Vector2.zero;d.limiteMax=size;
                var layout=d.GetComponent<SantaRitaRoomLayout>()??d.gameObject.AddComponent<SantaRitaRoomLayout>();
                // Valores originais estáveis: reexecutar autoria não redefine a origem de saves antigos.
                layout.oldSize=name=="SociedadeBeiraRio"?new Vector2(24,40):name=="IgrejaMatriz"?new Vector2(22,26):name=="PrefeituraSantaRita"?new Vector2(24,22):name=="RodoviariaSantaRita"?new Vector2(26,20):name=="MercadinhoDeposito"?new Vector2(20,18):name=="OficinaRadio"?new Vector2(18,16):name=="CasaCostureira"?new Vector2(18,18):new Vector2(16,14);
                layout.size=size;layout.arrival=entry;layout.versionFlag="layout_r2_"+name;
                ground.ClearAllTiles();var tile=AssetDatabase.LoadAssetAtPath<Tile>("Assets/Art/Capitulo2/Tiles/"+(name=="IgrejaMatriz"?"PisoCreme":name=="PrefeituraSantaRita"?"PisoVerde":name=="RodoviariaSantaRita"||name=="MercadinhoDeposito"?"PisoCinza":"Madeira")+".asset");
                foreach(var map in Object.FindObjectsByType<Tilemap>().Where(m=>m!=ground))map.ClearAllTiles();
                for(int x=0;x<size.x;x++)for(int y=0;y<size.y;y++)ground.SetTile(new Vector3Int(x,y,0),tile);
                ground.GetComponent<TilemapRenderer>().sharedMaterial=lit;
                Walls(size,entry.x);
                Spawn("centro",entry);Portal("Saida",new Vector2(entry.x,1),name=="AbrigoCemiterio"?"CemiterioSantaRita":"CentroSantaRita","volta_"+name);
                CreateExit(root,entry.x,.4f,name);
                Object.FindAnyObjectByType<PlayerController>().transform.position=entry;
                Camera.main.transform.position=new Vector3(entry.x,entry.y+1.1f,-10);
                foreach(var global in Object.FindObjectsByType<Light2D>().Where(l=>l.lightType==Light2D.LightType.Global)) {global.intensity=.48f;global.color=new Color(.89f,.91f,.9f);}
                if(name=="SociedadeBeiraRio")Sociedade();else if(name=="IgrejaMatriz")Igreja();else if(name=="PrefeituraSantaRita")Prefeitura();else if(name=="RodoviariaSantaRita")Rodoviaria();else if(name=="MercadinhoDeposito")Deposito();else if(name=="OficinaRadio")Oficina();else if(name=="CasaCostureira")Costura();else Abrigo();
                // O rig de câmera permanece com os mesmos limites de gameplay definidos pelo diretor.
                Physics2D.SyncTransforms();EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
            }
            AssetDatabase.SaveAssets();
        }
        finally{EditorSceneManager.RestoreSceneManagerSetup(setup);}
    }
    static GameObject Empty(string name,Vector2 p){var g=new GameObject(name);g.transform.SetParent(root,false);g.transform.position=p;return g;}
    static GameObject Solid(string name,Vector2 p,Vector2 size,Color color,int order,bool collider=false)
    {
        var g=Empty(name,p);var s=g.AddComponent<SpriteRenderer>();s.sprite=white;s.sharedMaterial=lit;s.color=color;s.sortingOrder=order;
        g.transform.localScale=new Vector3(size.x/white.bounds.size.x,size.y/white.bounds.size.y,1);
        if(collider){var b=g.AddComponent<BoxCollider2D>();b.size=white.bounds.size;Cast(g,b);}
        return g;
    }
    static void Cast(GameObject g,BoxCollider2D box)
    {
        var caster=g.AddComponent<ShadowCaster2D>();caster.selfShadows=false;caster.castsShadows=true;
        var so=new SerializedObject(caster);var path=so.FindProperty("m_ShapePath");path.arraySize=4;Vector2 h=box.size*.5f,c=box.offset;
        Vector3[] corners={c+new Vector2(-h.x,-h.y),c+new Vector2(-h.x,h.y),c+new Vector2(h.x,h.y),c+new Vector2(h.x,-h.y)};
        for(int i=0;i<4;i++)path.GetArrayElementAtIndex(i).vector3Value=corners[i];so.FindProperty("m_ShapePathHash").intValue=g.name.GetHashCode();so.ApplyModifiedPropertiesWithoutUndo();
    }
    static GameObject Prop(string name,string art,float x,float y,float width,bool collider=false)
    {
        var g=Empty(name,new Vector2(x,y));var s=g.AddComponent<SpriteRenderer>();s.sprite=Art(art);s.sharedMaterial=lit;s.sortingOrder=1000-Mathf.RoundToInt(y*10);
        g.transform.localScale=Vector3.one*(width/s.sprite.bounds.size.x);
        if(collider)
        {
            var b=g.AddComponent<BoxCollider2D>();b.size=new Vector2(width*.74f,.42f)/g.transform.localScale.x;b.offset=new Vector2(0,.2f)/g.transform.localScale.x;
            if(art.Contains("Armario")||art=="Geladeira"||art=="PilarMadeira")Cast(g,b);
            if(shadow!=null){var ao=new GameObject("AO_"+name);ao.transform.SetParent(g.transform,false);var sr=ao.AddComponent<SpriteRenderer>();sr.sprite=shadow;sr.color=new Color(.04f,.04f,.04f,.26f);sr.sortingOrder=-2500;ao.transform.localScale=new Vector3(s.sprite.bounds.size.x*.72f,.4f,1);}
        }
        return g;
    }
    static void Walls(Vector2 size,float door)
    {
        var plaster=new Color(.49f,.47f,.39f);var wood=new Color(.23f,.17f,.11f);
        Solid("ParedeNorte",new Vector2(size.x/2,size.y-1),new Vector2(size.x,2),plaster,2000,true);
        var finish=Empty("RebocoELambril",new Vector2(size.x/2,size.y-1));var fs=finish.AddComponent<SpriteRenderer>();fs.sprite=Art("RebocoLambril");fs.sharedMaterial=lit;fs.drawMode=SpriteDrawMode.Tiled;fs.size=new Vector2(size.x-.4f,2);fs.sortingOrder=2004;
        Solid("ParedeOeste",new Vector2(.22f,size.y/2),new Vector2(.44f,size.y),plaster,2000,true);
        Solid("ParedeLeste",new Vector2(size.x-.22f,size.y/2),new Vector2(.44f,size.y),plaster,2000,true);
        Solid("ParedeSulEsq",new Vector2((door-1)/2,.3f),new Vector2(door-1,.6f),wood,2000,true);
        Solid("ParedeSulDir",new Vector2((door+1+size.x)/2,.3f),new Vector2(size.x-door-1,.6f),wood,2000,true);
        Solid("LambrilNorte",new Vector2(size.x/2,size.y-1.7f),new Vector2(size.x,.6f),wood,2001);
        for(float x=.6f;x<size.x;x+=.28f)Solid("JuntaLambril",new Vector2(x,size.y-1.7f),new Vector2(.025f,.55f),new Color(.34f,.26f,.17f),2002);
        Solid("FrisoNorte",new Vector2(size.x/2,size.y-1.4f),new Vector2(size.x,.055f),new Color(.58f,.45f,.3f),2003);
        Prop("JanelaOeste","JanelaCortina",2.3f,size.y-1.4f,1.65f).GetComponent<SpriteRenderer>().sortingOrder=2008;
        Prop("JanelaLeste","JanelaCortina",size.x-2.3f,size.y-1.4f,1.65f).GetComponent<SpriteRenderer>().sortingOrder=2008;
    }
    static void Spawn(string id,Vector2 p){Empty("Spawn_"+id,p).AddComponent<SpawnPoint>().Configurar(id,1);}
    static void Portal(string name,Vector2 p,string dest,string entry){var g=Empty("Porta_"+name,p);var it=g.AddComponent<Capitulo2Portal>();it.cenaDestino=dest;it.entradaDestino=entry;it.saidaInterior=true;}
    static Capitulo2Interactable Interaction(string id,float x,float y)
    {
        var g=Empty("Examinar_"+id,new Vector2(x,y));var it=g.AddComponent<Capitulo2Interactable>();it.interactionId=id;
        var d=Capitulo2Director.Dados(id);if(d==null)throw new Exception(id);
        if(!string.IsNullOrEmpty(d.art))it.arteExame=Art(d.art);return it;
    }
    static void Lamp(float x,float y,float radius=5,float power=.7f)
    {var g=Empty("LuminariaDeUso",new Vector2(x,y));var l=g.AddComponent<Light2D>();l.lightType=Light2D.LightType.Point;l.pointLightOuterRadius=radius;l.pointLightInnerRadius=.5f;l.intensity=power;l.color=new Color(1,.83f,.58f);l.shadowIntensity=.65f;}
    static void Paper(string name,float x,float y,GameObject table)
    {var p=Prop(name,"Caderno",x,y,.45f);p.GetComponent<SpriteRenderer>().sortingOrder=table.GetComponent<SpriteRenderer>().sortingOrder+1;}
    static SantaRitaNpcWork NPC(string name,string id,Vector2 start,params (Vector2 p,float seconds,bool work,bool left)[] stations)
    {
        var g=Prop("NPC_"+name,"Npc_"+name,start.x,start.y,2,true);
        var talk=Interaction(id,start.x,start.y-.55f);talk.transform.SetParent(g.transform,true);
        var routine=g.AddComponent<SantaRitaNpcRoutine>();routine.enabled=false;
        var r=g.AddComponent<SantaRitaNpcWork>();r.poses=AssetDatabase.LoadAllAssetsAtPath("Assets/Art/Capitulo2/Trabalho/Trabalho_"+name+".png").OfType<Sprite>().OrderBy(s=>s.name).ToArray();
        r.stations=stations.Select((v,i)=>new SantaRitaNpcWork.Station{anchor=Empty("Posto_"+name+"_"+i,v.p).transform,duration=v.seconds,working=v.work,faceLeft=v.left}).ToArray();return r;
    }
    static void Sociedade()
    {
        var sand=AssetDatabase.LoadAssetAtPath<Tile>("Assets/Art/Capitulo2/Tiles/Areia.asset");
        for(int x=6;x<28;x++)for(int y=9;y<15;y++)ground.SetTile(new Vector3Int(x,y,0),sand);
        // Guias contínuas e baixas; cabeceiras reais nos extremos esquerda/direita.
        foreach(float y in new[]{8.85f,15.15f})
        {
            Solid("GuiaCancha",new Vector2(17,y),new Vector2(22.4f,.2f),new Color(.27f,.17f,.09f),840,true);
            for(float x=6;x<28;x+=2)Prop("MadeiraDaGuia","CanchaGuia",x+1,y,2.02f);
        }
        foreach(float x in new[]{5.85f,28.15f})
        {
            Solid("CabeceiraDeMadeira",new Vector2(x,12),new Vector2(.3f,6.5f),new Color(.32f,.21f,.12f),900,true);
            for(float y=9;y<15;y+=.4f)Solid("JuntaCabeceira",new Vector2(x,y),new Vector2(.31f,.035f),new Color(.16f,.11f,.07f),901);
        }
        Solid("LinhaDeLancamento",new Vector2(7,12),new Vector2(.065f,5.9f),new Color(.85f,.77f,.57f),-2200);
        foreach(float x in new[]{12.5f,18,23.5f})Solid("ReferenciaGastaDaCancha",new Vector2(x,12),new Vector2(.03f,5.8f),new Color(.64f,.55f,.39f,.4f),-2200);
        Prop("SuporteDeBolas","SuporteBochas",2.3f,13,2.3f,true);Interaction("regra_bocha",3.6f,12.8f);
        Prop("BancadaTonao","BancadaBochas",2.3f,10.1f,2.8f,true);Interaction("cordas",2.3f,9.4f);
        Prop("PlacarFisico","PlacarBocha",26,15.5f,2.1f).GetComponent<SpriteRenderer>().sortingOrder=2008;
        foreach(float x in new[]{12f,20f,26f})Prop("BancoDaCancha","BancoCancha",x,6.6f,2.5f,true);
        Prop("BancoTonao","BancoCancha",2.5f,7.1f,2.2f,true);
        Prop("CopaAguaQuente","ApoioAguaQuente",2.5f,4.8f,3,true);
        Prop("ConvivenciaChimarrao","Chimarrao",9,3.8f,3.2f,true);
        var caderno=Prop("MesaDeVisitas","MesaMadeira",14,3.2f,2.1f,true);Paper("CadernoDeVisitas",14,3.95f,caderno);Interaction("visitas_sociedade",14,2.7f);
        Prop("ArmarioDoMaterial","ArmarioMadeira",24,3.4f,2.2f,true);
        NPC("tonao","tonao",new Vector2(4,12.8f),(new Vector2(4,12.8f),3,true,true),(new Vector2(4,10.35f),7,true,true),(new Vector2(4.6f,12),3,false,false),(new Vector2(4,7.3f),5,false,false));
        NPC("nair","nair",new Vector2(4.4f,4.9f),(new Vector2(4.4f,4.9f),9,true,true),(new Vector2(5.5f,5.2f),3,false,false));
        Interaction("bocha",7,7.5f);
        Prop("FrequentadorSentado","Npc_Frequentador",9.3f,4.7f,2).AddComponent<SantaRitaNpcRoutine>().atividade="chimarrao";
        foreach(float x in new[]{10f,19f,26f})Lamp(x,12.5f,5.2f,.72f);Lamp(7,4.2f,6,.65f);Lamp(2.5f,11,4,.6f);
    }
    static void Igreja()
    {
        Solid("PassadeiraNave",new Vector2(9,10),new Vector2(3.1f,17),new Color(.34f,.13f,.1f),-2400);
        foreach(float y in new[]{6f,9.5f,13f})foreach(float x in new[]{4.5f,13.5f})Prop("BancoDaNave","BancoIgreja",x,y,4.1f,true);
        Solid("DegrauAltar",new Vector2(9,18),new Vector2(11,2),new Color(.37f,.34f,.27f),-2300);
        Prop("Altar","Altar",9,18.6f,5,true);
        Prop("ApoioDasVelas","ArmarioVelas",2.7f,17.6f,2,true);Interaction("velas_igreja",2.7f,16.8f);
        Prop("MuralComunitario","QuadroAvisos",14.8f,4,2.1f,true);Interaction("foto_igreja",14.8f,3.5f);
        Prop("MesaDaSacristia","MesaMadeira",14.9f,17.3f,2.2f,true);
        NPC("bento","padre",new Vector2(13.3f,17.55f),(new Vector2(13.3f,17.55f),8,true,false),(new Vector2(12.6f,16),5,false,false));
        foreach(float x in new[]{4f,14f}){Lamp(x,9,6,.55f);Lamp(x,16.5f,5,.65f);}Lamp(9,19,5,.65f);
    }
    static void Prefeitura()
    {
        var table=Prop("BalcaoConsulta","MesaPublica",10,6.6f,3.1f,true);Paper("Protocolo",9.5f,7.5f,table);Interaction("protocolo",9.5f,5.9f);
        NPC("clarice","prefeitura",new Vector2(10.6f,7.8f),(new Vector2(10.6f,7.8f),9,true,true));
        Prop("BancoEspera","BancoCancha",3.5f,3.6f,3,true);Prop("BancoEsperaDireita","BancoCancha",16.5f,3.6f,3,true);
        Prop("ArquivoDeConsulta","ArquivoMetal",3.1f,8.1f,1.7f,true);Interaction("placa_praca",3.1f,7.4f);
        Solid("DivisoriaConsultaEsq",new Vector2(3.8f,10),new Vector2(7.6f,.45f),new Color(.36f,.37f,.32f),900,true);
        Solid("DivisoriaConsultaDir",new Vector2(16.2f,10),new Vector2(7.6f,.45f),new Color(.36f,.37f,.32f),900,true);
        var gate=Solid("PortaConsultaPublica",new Vector2(10,10),new Vector2(4.8f,.4f),new Color(.26f,.34f,.3f),950,true);gate.AddComponent<Capitulo2Gate>().flag="c2_prefeitura_atendimento";
        var mesa=Prop("MesaLeitura","MesaPublica",10,13,3.6f,true);Paper("AnexoManutencao",9.1f,13.95f,mesa);Paper("ExtratoDeContas",10.9f,13.95f,mesa);
        Interaction("manutencao",9.1f,12.4f);Interaction("extrato_publico",10.9f,12.4f);
        foreach(float x in new[]{3f,17f})Prop("ArquivoExpediente","ArquivoMetal",x,13.3f,1.8f,true);
        Lamp(10,6.5f,6,.75f);Lamp(10,13,6,.85f);Lamp(3,8,3.5f,.6f);
    }
    static void Rodoviaria()
    {
        foreach(float y in new[]{4f,8f})Prop("BancoEspera","BancoCancha",5,y,3.6f,true);
        var guiche=Prop("Guiche","Caixa",16,9.3f,3.4f,true);
        NPC("dario","dario",new Vector2(16,10.6f),(new Vector2(16,10.6f),7,true,false));
        Interaction("telefone_rodo",19.3f,8.7f);Prop("TelefoneDoGuiche","TelefoneForaDoGancho",19.2f,9.7f,.8f);
        Prop("Horarios","QuadroAvisos",10.7f,11.3f,2.6f,true);Interaction("horarios",10.7f,10.6f);
        Prop("Bagagem","CaixasManutencao",3.3f,4.3f,1.2f,true);Interaction("bagagem",3.3f,3.6f);
        var passageira=Prop("Passageira_Espera","Npc_Passageira",5,5,2,true);passageira.AddComponent<SantaRitaNpcRoutine>().atividade="espera";var a=Interaction("rodo_passageira",5,4.4f);a.transform.SetParent(passageira.transform,true);
        var motorista=Prop("Motorista_Escala","Npc_Motorista",8.2f,8.3f,2,true);motorista.AddComponent<SantaRitaNpcRoutine>().atividade="jornal";var b=Interaction("rodo_motorista",8.2f,7.7f);b.transform.SetParent(motorista.transform,true);
        Lamp(5,6,6,.7f);Lamp(16,10.5f,6,.85f);Lamp(11,11.5f,4,.55f);
    }
    static void Deposito()
    {
        foreach(float y in new[]{3.5f,7f,10.5f})Prop("CargaOrganizada","EstoqueDeposito",2.5f,y,2.5f,true);
        Prop("EstoqueSecos","Prateleira",13.3f,9.6f,2.7f,true);Prop("VasilhamesRetornaveis","EstoqueDeposito",13.4f,4.1f,2.2f,true);
        var table=Prop("MesaConferencia","MesaMadeira",8,8.4f,2.6f,true);Paper("RomaneioSobreMesa",8,9.3f,table);Interaction("romaneio",8,7.7f);
        Portal("LojaOriginal",new Vector2(8,12),"EntradaDaCidade","retorno_deposito");Spawn("loja",new Vector2(8,10.7f));Interaction("atendente_fundos",8,11.2f);
        CreateExit(root,8,11.8f,"Servico");Lamp(8,8,6,.8f);Lamp(8,12,4,.6f);
    }
    static void Oficina()
    {
        Prop("BancadaEletronica","BancadaOficina",9.5f,7.9f,3.5f,true);
        NPC("mila","mila",new Vector2(7.3f,8.1f),(new Vector2(7.3f,8.1f),9,true,false));
        Interaction("radio_test",9.5f,7.2f);Prop("ArmarioSeco","ArmarioMadeira",13.5f,8.6f,1.9f,true);Interaction("desenho_radio",13.5f,7.9f);
        Prop("EstanteDePecas","CaixasManutencao",3,8.8f,2,true);Prop("BancoEspera","BancoCancha",3.4f,3.3f,2.4f,true);Prop("AparelhoRecebido","Televisor",12.6f,3.1f,1.7f,true);
        Lamp(9,8.5f,5,.9f);Lamp(5,3.5f,4,.55f);
    }
    static void Costura()
    {
        Prop("MaquinaCelina","BancadaCostura",7.8f,8.9f,3.5f,true);
        NPC("celina","celina",new Vector2(5.6f,9.05f),(new Vector2(5.6f,9.05f),8,true,false));
        Interaction("encomenda_faixa",7.7f,8.2f);Prop("TecidosGuardados","ArmarioMadeira",3,10.1f,1.9f,true);
        Prop("SofaDaSala","Sofa",12.8f,3.7f,2.9f,true);Prop("MesaDoCha","Chimarrao",11.9f,8.7f,2.4f,true);
        Solid("TapeteDaSala",new Vector2(10.8f,4.5f),new Vector2(5,2.7f),new Color(.3f,.18f,.16f),-2400);
        Prop("RetratoSala","FramedPhoto",8,12.2f,.8f).GetComponent<SpriteRenderer>().sortingOrder=2008;Lamp(7.5f,9.2f,5,.85f);Lamp(12.8f,4.5f,5,.6f);
    }
    static void Abrigo()
    {
        Prop("ArmarioVelas","ArmarioVelas",2.3f,6.6f,2,true);Interaction("apoio_cemiterio",2.3f,5.9f);
        var table=Prop("MesaDaRonda","MesaMadeira",9.2f,6.7f,2.2f,true);Paper("CadernoDeManutencao",9.2f,7.5f,table);Interaction("caderno_velas",9.2f,6);
        Prop("BancoDaRonda","BancoCancha",9.5f,2.8f,2.4f,true);Prop("FerramentasGuardadas","CaixasManutencao",2.3f,3.1f,1.4f,true);
        Lamp(8.5f,6.5f,5,.65f);Lamp(6.5f,2,3,.4f);
    }
}

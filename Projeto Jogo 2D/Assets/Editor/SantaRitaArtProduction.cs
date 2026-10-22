using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

public static class SantaRitaArtProduction
{
    private const string Root = "Assets/Art/Producao";
    [MenuItem("Santa Rita/Arte/Aplicar direção visual às cenas C2")]
    public static void Apply()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Pare o Play antes da autoria.");
        if (Enumerable.Range(0, SceneManager.sceneCount).Any(i => SceneManager.GetSceneAt(i).isDirty))
            throw new InvalidOperationException("Há trabalho não salvo. Salve as cenas antes da autoria.");
        var setup = EditorSceneManager.GetSceneManagerSetup();
        Directory.CreateDirectory(Root);
        var lit = AssetDatabase.LoadAssetAtPath<Material>(Root + "/WorldLit.mat");
        if (lit == null)
        {
            lit = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default"));
            AssetDatabase.CreateAsset(lit, Root + "/WorldLit.mat");
        }
        var shadow = ContactShadow();
        var white = NeutralSprite();
        Directory.CreateDirectory("Assets/Resources/Art");
        if(AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Art/WorldLit.mat")==null)
            AssetDatabase.CreateAsset(new Material(lit),"Assets/Resources/Art/WorldLit.mat");
        if(AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Art/Mist.mat")==null)
        {
            var mist=new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
            mist.SetFloat("_Surface",1);mist.SetFloat("_Blend",0);mist.SetFloat("_SrcBlend",5);mist.SetFloat("_DstBlend",10);mist.SetFloat("_ZWrite",0);
            mist.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");mist.renderQueue=3000;
            AssetDatabase.CreateAsset(mist,"Assets/Resources/Art/Mist.mat");
        }
        try
        {
            foreach (string name in SantaRitaChapter2Authoring.Scenes)
            {
                var scene = EditorSceneManager.OpenScene("Assets/Scenes/Cidade/" + name + ".unity");
                bool inside = name != "CentroSantaRita" && name != "CemiterioSantaRita";
                if(name=="RodoviariaSantaRita") BusStationRoutine(lit);
                foreach (var text in Object.FindObjectsByType<TextMeshPro>(FindObjectsSortMode.None))
                    if (text.gameObject.name.StartsWith("Letreiro_")) Object.DestroyImmediate(text.gameObject);
                foreach (var sprite in Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None))
                {
                    if (sprite.name.StartsWith("AO_")) continue;
                    if (sprite.name == "Canteiro") { Object.DestroyImmediate(sprite.gameObject); continue; }
                    if (sprite.sprite != null && AssetDatabase.GetAssetPath(sprite.sprite).Contains("/Tiles/PisoCreme.png")) sprite.sprite=white;
                    sprite.sharedMaterial = lit;
                    if(sprite.sprite!=null)
                    {
                        string file=Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(sprite.sprite));
                        var building=Resources.Load<Sprite>("Art/Buildings/"+file);if(building!=null)sprite.sprite=building;
                    }
                    if(sprite.name=="NPC_chico")
                    {var character=Resources.Load<Sprite>("Art/Characters/Npc_Chico");if(character!=null)sprite.sprite=character;}
                    if (sprite.name.StartsWith("Arvore"))
                    {
                        var tree=Resources.Load<Sprite>("Art/World/"+(Mathf.RoundToInt(sprite.transform.position.x)%2==0?"Tree_A":"Tree_B"));
                        if(tree!=null)sprite.sprite=tree;
                        sprite.color=Color.white;
                    }
                    if (sprite.GetComponent<Collider2D>() != null && sprite.name != "Player" &&
                        !sprite.name.Contains("Parede") && !sprite.name.Contains("Limite") && !sprite.name.Contains("Cerca"))
                    {
                        string id = "AO_" + sprite.name;
                        if (GameObject.Find(id) == null)
                        {
                            var go = new GameObject(id); go.transform.SetParent(sprite.transform, false);
                            var ao = go.AddComponent<SpriteRenderer>(); ao.sprite = shadow;
                            ao.color = new Color(.035f,.045f,.052f,.28f); ao.sortingOrder = -2500;
                            go.transform.localPosition = new Vector3(0,.05f,0);
                            float width = Mathf.Clamp(sprite.sprite.bounds.size.x * .73f, .6f, 9);
                            go.transform.localScale = new Vector3(width, Mathf.Clamp(width*.2f,.22f,1.1f),1);
                        }
                        if (!inside && (sprite.name == "Igreja" || sprite.name == "Prefeitura" || sprite.name == "Rodoviaria" || sprite.name == "Sociedade"))
                        {
                            var box=sprite.GetComponent<BoxCollider2D>();
                            if(box!=null && sprite.transform.Find("ShadowFootprint")==null)
                            {
                                var footprint=new GameObject("ShadowFootprint");footprint.transform.SetParent(sprite.transform,false);
                                var caster=footprint.AddComponent<ShadowCaster2D>();caster.selfShadows=false;caster.castsShadows=true;
                                var so=new SerializedObject(caster);var path=so.FindProperty("m_ShapePath");path.arraySize=4;
                                Vector2 c=box.offset,h=box.size*.5f;
                                Vector3[] corners={c+new Vector2(-h.x,-h.y),c+new Vector2(-h.x,h.y),c+new Vector2(h.x,h.y),c+new Vector2(h.x,-h.y)};
                                for(int i=0;i<4;i++)path.GetArrayElementAtIndex(i).vector3Value=corners[i];
                                so.FindProperty("m_ShapePathHash").intValue=731+sprite.name.Length;so.ApplyModifiedPropertiesWithoutUndo();
                            }
                        }
                    }
                }
                foreach (var map in Object.FindObjectsByType<TilemapRenderer>(FindObjectsSortMode.None)) map.sharedMaterial = lit;
                foreach (var light in Object.FindObjectsByType<Light2D>(FindObjectsSortMode.None))
                {
                    if (light.lightType == Light2D.LightType.Global)
                    { light.intensity = inside ? .78f : .66f; light.color = inside ? new Color(.92f,.91f,.82f) : new Color(.71f,.79f,.88f); }
                    else { light.intensity = Mathf.Min(light.intensity,.72f); light.shadowIntensity = .32f; }
                }
                var player = Object.FindAnyObjectByType<PlayerController>();
                if (player != null && player.GetComponent<SantaRitaFootsteps>() == null) player.gameObject.AddComponent<SantaRitaFootsteps>();
                if (!inside && player != null && player.GetComponent<SantaRitaAtmosphere>() == null) player.gameObject.AddComponent<SantaRitaAtmosphere>();
                var groundObject = GameObject.Find("Terreno");
                var ground = groundObject != null ? groundObject.GetComponent<Tilemap>() : null;
                if (!inside && ground != null) { ApplyMacroTiles(ground); DecorateTerrain(ground, lit); }
                var camera = Object.FindAnyObjectByType<Camera>();
                if (camera != null) camera.GetUniversalAdditionalCameraData().renderPostProcessing = true;
                if (GameObject.Find("Atmosfera_Producao") == null)
                {
                    var go = new GameObject("Atmosfera_Producao"); var volume = go.AddComponent<Volume>(); volume.isGlobal = true; volume.priority = 2;
                    string path = Root + "/" + (inside ? "Interior" : "Exterior") + ".asset";
                    var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);
                    if (profile == null)
                    {
                        profile = ScriptableObject.CreateInstance<VolumeProfile>(); AssetDatabase.CreateAsset(profile,path);
                        var vignette = profile.Add<Vignette>(true); vignette.intensity.Override(.15f); vignette.smoothness.Override(.55f);
                        var color = profile.Add<ColorAdjustments>(true); color.saturation.Override(-7); color.contrast.Override(4);
                        AssetDatabase.AddObjectToAsset(vignette,profile); AssetDatabase.AddObjectToAsset(color,profile);
                    }
                    volume.sharedProfile = profile;
                }
                EditorSceneManager.SaveScene(scene);
            }
            AssetDatabase.SaveAssets();
        }
        finally { EditorSceneManager.RestoreSceneManagerSetup(setup); }
        Debug.Log("Santa Rita arte: nove cenas C2 atualizadas; geografia e interações preservadas.");
    }

    private static Sprite ContactShadow()
    {
        string path = Root + "/ContactShadow.png";
        if (!File.Exists(path))
        {
            var image = new Texture2D(32,16,TextureFormat.RGBA32,false);
            for (int y=0;y<16;y++) for (int x=0;x<32;x++)
            { float r = new Vector2((x-15.5f)/16,(y-7.5f)/8).magnitude; image.SetPixel(x,y,new Color(1,1,1,Mathf.Clamp01((1-r)*3))); }
            File.WriteAllBytes(path,image.EncodeToPNG()); Object.DestroyImmediate(image); AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path); importer.textureType=TextureImporterType.Sprite;
            importer.spritePixelsPerUnit=32; importer.filterMode=FilterMode.Point; importer.mipmapEnabled=false; importer.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static void BusStationRoutine(Material lit)
    {
        var dispatcher=GameObject.Find("NPC_dario");
        if(dispatcher!=null)
        {
            dispatcher.transform.position=new Vector3(18,14,0);
            dispatcher.GetComponent<SpriteRenderer>().sortingOrder=860;
            var interaction=GameObject.Find("Examinar_dario");if(interaction!=null)interaction.transform.position=new Vector3(18,10.8f,0);
        }
        void Person(string name,string asset,Vector2 position,string interactionId)
        {
            if(GameObject.Find(name)!=null)return;var sprite=Resources.Load<Sprite>("Art/Characters/"+asset);if(sprite==null)return;
            var go=new GameObject(name);go.transform.position=position;
            var sr=go.AddComponent<SpriteRenderer>();sr.sprite=sprite;sr.sharedMaterial=lit;sr.sortingOrder=1000-Mathf.RoundToInt(position.y*10);
            var box=go.AddComponent<BoxCollider2D>();box.size=new Vector2(.6f,.4f);box.offset=new Vector2(0,.2f);
            var proxy=new GameObject("Examinar_"+interactionId);proxy.transform.SetParent(go.transform,false);proxy.transform.localPosition=Vector3.down*.6f;
            proxy.AddComponent<Capitulo2Interactable>().interactionId=interactionId;
        }
        Person("Passageira_Espera","Npc_Passageira",new Vector2(10.5f,7),"rodo_passageira");
        Person("Motorista_Escala","Npc_Motorista",new Vector2(20,5),"rodo_motorista");
    }

    private static Sprite NeutralSprite()
    {
        string path=Root+"/White.png";
        if(!File.Exists(path))
        {
            var tex=new Texture2D(32,32,TextureFormat.RGBA32,false);tex.SetPixels(Enumerable.Repeat(Color.white,1024).ToArray());tex.Apply();
            File.WriteAllBytes(path,tex.EncodeToPNG());Object.DestroyImmediate(tex);AssetDatabase.ImportAsset(path);
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Sprite;importer.spritePixelsPerUnit=32;
            importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static void DecorateTerrain(Tilemap ground, Material lit)
    {
        var previous = GameObject.Find("DetalhesTerreno"); if (previous != null) Object.DestroyImmediate(previous);
        var go = new GameObject("DetalhesTerreno",typeof(Tilemap),typeof(TilemapRenderer)); go.transform.SetParent(ground.transform.parent,false);
        var details=go.GetComponent<Tilemap>(); var renderer=go.GetComponent<TilemapRenderer>(); renderer.sortingOrder=-2900; renderer.sharedMaterial=lit;
        var edge = EdgeTile(); var bounds=ground.cellBounds;
        foreach(var cell in bounds.allPositionsWithin)
        {
            var tile=ground.GetTile(cell); if(tile==null || !tile.name.Contains("Grama"))continue;
            int mask=0;
            var directions=new[]{Vector3Int.up,Vector3Int.right,Vector3Int.down,Vector3Int.left};
            for(int i=0;i<4;i++) {var neighbor=ground.GetTile(cell+directions[i]);if(neighbor!=null&&!neighbor.name.Contains("Grama")&&!neighbor.name.Contains("Agua"))mask|=1<<i;}
            if(mask==0)continue;
            details.SetTile(cell,edge[mask]);
        }
        var garden=new GameObject("VegetacaoDeBorda");garden.transform.SetParent(go.transform,false);
        var trees=Object.FindObjectsByType<SpriteRenderer>().Where(s=>s.name.StartsWith("Arvore")).ToArray();
        foreach(var tree in trees)
            for(int side=-1;side<=1;side+=2)
            {
                var p=tree.transform.position+new Vector3(side*1.2f,-.4f,0);
                var tile=ground.GetTile(ground.WorldToCell(p));if(tile==null||!tile.name.Contains("Grama"))continue;
                var shrub=Resources.Load<Sprite>("Art/World/"+(side<0?"Fern":"Shrub"));if(shrub==null)continue;
                var detail=new GameObject("SubBosque");detail.transform.SetParent(garden.transform,false);detail.transform.position=p;
                var sr=detail.AddComponent<SpriteRenderer>();sr.sprite=shrub;sr.sharedMaterial=lit;sr.sortingOrder=1000-Mathf.RoundToInt(p.y*10);
            }
        foreach(var p in new[]{new Vector3(37,44,0),new Vector3(54,44,0),new Vector3(37,55,0),new Vector3(54,55,0)})
        {
            var groundTile=ground.GetTile(ground.WorldToCell(p));if(groundTile==null||!groundTile.name.Contains("Grama"))continue;
            var flower=Resources.Load<Sprite>("Art/World/Flowers");if(flower==null)continue;
            var detail=new GameObject("FloresDoCanteiro");detail.transform.SetParent(garden.transform,false);detail.transform.position=p;
            var sr=detail.AddComponent<SpriteRenderer>();sr.sprite=flower;sr.sharedMaterial=lit;sr.sortingOrder=1000-Mathf.RoundToInt(p.y*10);
        }
    }
    private static void ApplyMacroTiles(Tilemap map)
    {
        Directory.CreateDirectory(Root+"/Terrenos");var cache=new System.Collections.Generic.Dictionary<string,Tile>();
        foreach(var cell in map.cellBounds.allPositionsWithin)
        {
            var current=map.GetTile(cell);if(current==null)continue;
            string type=current.name.Split('_')[0];if(type!="Grama"&&type!="Terra"&&type!="Asfalto")continue;
            int x=((cell.x%4)+4)%4,y=((cell.y%4)+4)%4;string id=type+"_"+x+"_"+y;
            if(!cache.TryGetValue(id,out var tile))
            {
                string path=Root+"/Terrenos/"+id+".asset";tile=AssetDatabase.LoadAssetAtPath<Tile>(path);
                if(tile==null)
                {
                    var sprite=Resources.Load<Sprite>("Art/Macros/"+id);if(sprite==null)continue;
                    tile=ScriptableObject.CreateInstance<Tile>();tile.sprite=sprite;tile.colliderType=Tile.ColliderType.None;AssetDatabase.CreateAsset(tile,path);
                }
                cache[id]=tile;
            }
            map.SetTile(cell,tile);
        }
    }
    private static Tile[] EdgeTile()
    {
        var result=new Tile[16];Directory.CreateDirectory(Root+"/Bordas");
        for(int mask=1;mask<16;mask++)
        {
            string path=Root+"/Bordas/Grama_"+mask;var tile=AssetDatabase.LoadAssetAtPath<Tile>(path+".asset");
            if(tile==null)
            {
                var image=new Texture2D(32,32,TextureFormat.RGBA32,false);var pixels=new Color[1024];
                var random=new System.Random(617+mask);
                for(int y=0;y<32;y++)for(int x=0;x<32;x++)
                {
                    int depth=Mathf.Min((mask&1)!=0?31-y:99,(mask&2)!=0?31-x:99,(mask&4)!=0?y:99,(mask&8)!=0?x:99);
                    if(depth<2+random.Next(3))pixels[y*32+x]=new Color(.19f,.16f,.11f,.7f-depth*.12f);
                }
                image.SetPixels(pixels);image.Apply();File.WriteAllBytes(path+".png",image.EncodeToPNG());Object.DestroyImmediate(image);AssetDatabase.ImportAsset(path+".png");
                var importer=(TextureImporter)AssetImporter.GetAtPath(path+".png");importer.textureType=TextureImporterType.Sprite;importer.spritePixelsPerUnit=32;
                importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.SaveAndReimport();
                tile=ScriptableObject.CreateInstance<Tile>();tile.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path+".png");tile.colliderType=Tile.ColliderType.None;AssetDatabase.CreateAsset(tile,path+".asset");
            }
            result[mask]=tile;
        }
        return result;
    }
}

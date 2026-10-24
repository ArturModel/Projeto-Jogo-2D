using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object=UnityEngine.Object;

// Atualização localizada: não reconstrói mobiliário, progressão ou cenas C1.
public static partial class SantaRitaC2Consolidation
{
    [MenuItem("Santa Rita/Interiores/Atualizar soleiras e escada da cancha")]
    public static void UpdateAccesses()
    {
        Safe();ImportAccessArt();var setup=EditorSceneManager.GetSceneManagerSetup();
        try
        {
            foreach(string name in rooms)
            {
                var scene=EditorSceneManager.OpenScene("Assets/Scenes/Cidade/"+name+".unity");
                foreach(string obsolete in new[]{"PortaDeEntrada","PortaDeServico","SaidasIntegradas"})
                {var old=GameObject.Find(obsolete);if(old!=null)Object.DestroyImmediate(old);}
                var parent=new GameObject("SaidasIntegradas").transform;parent.SetParent(Object.FindAnyObjectByType<Capitulo2Director>().transform,false);
                float x=Object.FindAnyObjectByType<SantaRitaRoomLayout>().arrival.x;
                CreateExit(parent,x,.4f,name);
                foreach(var portal in Object.FindObjectsByType<Capitulo2Portal>())portal.saidaInterior=true;
                if(name=="MercadinhoDeposito")CreateExit(parent,8,11.8f,"Servico");
                EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
            }
            EncloseCourt();AssetDatabase.SaveAssets();
        }
        finally{EditorSceneManager.RestoreSceneManagerSetup(setup);}
    }
    static void ImportAccessArt()
    {
        foreach(string guid in AssetDatabase.FindAssets("t:Texture2D",new[]{"Assets/Resources/Art/Acessos"}))
        {
            var imp=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid));
            imp.textureType=TextureImporterType.Sprite;imp.spriteImportMode=SpriteImportMode.Single;imp.spritePixelsPerUnit=32;
            string key=System.IO.Path.GetFileNameWithoutExtension(imp.assetPath);
            var settings=new TextureImporterSettings();imp.ReadTextureSettings(settings);settings.spritePivot=new Vector2(.5f,key=="BancoCanchaSul"?.125f:key=="SombraContato"?.5f:0);settings.spriteAlignment=(int)SpriteAlignment.Custom;imp.SetTextureSettings(settings);
            imp.filterMode=FilterMode.Point;imp.mipmapEnabled=false;imp.textureCompression=TextureImporterCompression.Uncompressed;imp.SaveAndReimport();
        }
    }
    static void CreateExit(Transform parent,float x,float y,string room)
    {
        string style=room=="CasaCostureira"?"SaidaCasa":room=="SociedadeBeiraRio"?"SaidaSociedade":room=="MercadinhoDeposito"||room=="Servico"?"SaidaServico":
            room=="OficinaRadio"?"SaidaComercio":"SaidaPublica";
        SantaRitaExitThreshold.Create(parent,room=="Servico"?"SoleiraServico":"SoleiraSaida",new Vector2(x,y),style,1.8f,-2100);
    }
    static void DrawCourtStair()
    {
        var g=Empty("EscadaRasaDaCancha",new Vector2(4.7f,6.3f));var sr=g.AddComponent<SpriteRenderer>();
        sr.sprite=Resources.Load<Sprite>("Art/Acessos/EscadaCancha");sr.sharedMaterial=lit;sr.sortingOrder=-2080;
        g.transform.localScale=Vector3.one*(2.35f/sr.sprite.bounds.size.x);
        g.AddComponent<SantaRitaShallowStairs>();
        // Laterais visíveis limitam a passagem, sem mureta atravessando os degraus.
        Solid("ContencaoEscadaOeste",new Vector2(3.58f,7.65f),new Vector2(.24f,2.7f),new Color(.30f,.25f,.18f),924,true);
        Solid("ContencaoEscadaLeste",new Vector2(5.82f,7.65f),new Vector2(.24f,2.7f),new Color(.30f,.25f,.18f),924,true);
        var facade=Empty("FachadaInternaCancha",Vector2.zero);var cut=facade.AddComponent<SantaRitaRoomCutaway>();
        SpriteRenderer Face(float from,float to)
        {
            var go=Empty("ParedeRecortavelCancha",new Vector2((from+to)/2,6.3f));go.transform.SetParent(facade.transform,true);
            var face=go.AddComponent<SpriteRenderer>();face.sprite=Resources.Load<Sprite>("Art/Acessos/ParedeCancha");face.sharedMaterial=lit;
            face.drawMode=SpriteDrawMode.Tiled;face.size=new Vector2(to-from,2);face.sortingOrder=926;return face;
        }
        var roof=Empty("ForroRecortavelCancha",new Vector2(16.5f,12.925f));roof.transform.SetParent(facade.transform,true);
        var ceiling=roof.AddComponent<SpriteRenderer>();ceiling.sprite=Resources.Load<Sprite>("Bocha/MadeiraCancha");ceiling.sharedMaterial=lit;
        ceiling.drawMode=SpriteDrawMode.Tiled;ceiling.size=new Vector2(26.1f,8.35f);ceiling.color=new Color(.48f,.51f,.52f);ceiling.sortingOrder=1800;
        var pieces=new List<SpriteRenderer>{Face(3.2f,3.8f),Face(5.6f,29.6f),ceiling};
        foreach(var segment in new[]{new Vector2(3.2f,3.8f),new Vector2(5.6f,29.6f)})
        {
            var cap=Empty("EspessuraParedeCancha",new Vector2((segment.x+segment.y)/2,8.25f));cap.transform.SetParent(facade.transform,true);
            var face=cap.AddComponent<SpriteRenderer>();face.sprite=Resources.Load<Sprite>("Art/Acessos/CapeamentoParede");face.sharedMaterial=lit;face.drawMode=SpriteDrawMode.Tiled;
            face.size=new Vector2(segment.y-segment.x,.5f);face.sortingOrder=1801;pieces.Add(face);
            for(float x=segment.x+.12f;x<segment.y;x+=4)
            {
                var post=Solid("MontanteParedeCancha",new Vector2(x,7.3f),new Vector2(.16f,2.05f),new Color(.27f,.19f,.12f),927);
                post.transform.SetParent(facade.transform,true);pieces.Add(post.GetComponent<SpriteRenderer>());
                var edge=Solid("LuzMontante",new Vector2(x-.055f,7.3f),new Vector2(.028f,2.05f),new Color(.53f,.40f,.24f),928);
                edge.transform.SetParent(facade.transform,true);pieces.Add(edge.GetComponent<SpriteRenderer>());
            }
        }
        pieces.AddRange(Object.FindObjectsByType<SpriteRenderer>().Where(r=>r.name=="RebocoELambrilDivisoria"));cut.walls=pieces.ToArray();
        foreach(var actor in Object.FindObjectsByType<PlayerController>().Select(p=>p.gameObject).Concat(new[]{GameObject.Find("NPC_tonao")}))
            if(actor!=null&&actor.GetComponent<SantaRitaActorElevation>()==null)actor.AddComponent<SantaRitaActorElevation>();
        // Contenção do piso rebaixado: sombra contínua junto à divisória.
        for(int i=0;i<5;i++)Solid("SombraDesnivelCancha",new Vector2(17.5f,8.3f+i*.1f),new Vector2(23.8f,.1f),new Color(.035f,.04f,.038f,.26f-i*.045f),-2090);
    }
    static void CompactCourt(Tile sand)
    {
        // Apenas a cancha e suas bordas: atores, bancos e tamanho dos pixels não são escalados.
        string[] obsolete={"GuiaCancha","MadeiraDaGuia","CabeceiraDeMadeira","JuntaCabeceira","LinhaDeLancamento","ReferenciaGastaDaCancha","BancoDaCancha"};
        foreach(var t in Object.FindObjectsByType<Transform>().Where(t=>obsolete.Contains(t.name)).ToArray())Object.DestroyImmediate(t.gameObject);
        for(int x=7;x<26;x++)for(int y=10;y<15;y++)ground.SetTile(new Vector3Int(x,y,0),sand);
        var edge=Empty("SaibroMeiaCelula",new Vector2(16.5f,9.75f));var sr=edge.AddComponent<SpriteRenderer>();sr.sprite=sand.sprite;sr.sharedMaterial=lit;sr.drawMode=SpriteDrawMode.Tiled;sr.size=new Vector2(19,.5f);sr.sortingOrder=-2250;
        void Rail(string name,Vector2 p,Vector2 size)
        {
            var g=Empty(name,p);var r=g.AddComponent<SpriteRenderer>();r.sprite=Resources.Load<Sprite>("Bocha/MadeiraCancha");r.sharedMaterial=lit;
            r.drawMode=SpriteDrawMode.Tiled;r.size=size;r.color=new Color(.79f,.75f,.67f);r.sortingOrder=1000-Mathf.RoundToInt(p.y*10);
            var box=g.AddComponent<BoxCollider2D>();box.size=size;Cast(g,box);
            Solid("FrisoGuiaCompacta",p+Vector2.up*.06f,new Vector2(size.x,.035f),new Color(.62f,.45f,.26f),r.sortingOrder+1);
        }
        Rail("GuiaCancha",new Vector2(16.5f,9.38f),new Vector2(19.4f,.24f));Rail("GuiaCancha",new Vector2(16.5f,15.12f),new Vector2(19.4f,.24f));
        Rail("CabeceiraDeMadeira",new Vector2(6.86f,12.25f),new Vector2(.28f,6));Rail("CabeceiraDeMadeira",new Vector2(26.14f,12.25f),new Vector2(.28f,6));
        Solid("LinhaDeLancamento",new Vector2(8,12.25f),new Vector2(.06f,5.45f),new Color(.80f,.72f,.53f),-2200);
        foreach(float x in new[]{13f,18,23})Solid("ReferenciaGastaDaCancha",new Vector2(x,12.25f),new Vector2(.025f,5.4f),new Color(.64f,.55f,.39f,.3f),-2200);
        foreach(float y in new[]{8.15f,16.05f})foreach(float x in new[]{10f,14,18,22})
        {
            var bench=Empty(y<10?"BancoCanchaSul":"BancoCanchaNorte",new Vector2(x,y));var b=bench.AddComponent<SpriteRenderer>();
            b.sprite=Resources.Load<Sprite>(y<10?"Art/Acessos/BancoCanchaSul":"Art/Props/BancoCancha");b.sharedMaterial=lit;b.sortingOrder=1000-Mathf.RoundToInt(y*10);
            bench.transform.localScale=Vector3.one*(3.5f/b.sprite.bounds.size.x);
            var box=bench.AddComponent<BoxCollider2D>();box.size=new Vector2(2.65f,.36f)/bench.transform.localScale.x;box.offset=new Vector2(0,.2f)/bench.transform.localScale.x;
        }
    }
}

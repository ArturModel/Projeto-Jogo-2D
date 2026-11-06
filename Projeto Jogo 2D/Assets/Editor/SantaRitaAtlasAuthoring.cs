using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Object=UnityEngine.Object;

// Levantamento explícito da cena construída, inclusive objetos criados em Play.
// Não chama builders, não salva cenas e não escreve nenhum slot.
public static class SantaRitaAtlasAuthoring
{
    const string Root="Assets/Resources/Atlas";
    const string Stage="Library/SantaRitaAtlasStage";
    static readonly Dictionary<Texture2D,Color32[]> pixels=new Dictionary<Texture2D,Color32[]>();
    static readonly Dictionary<Texture2D,Texture2D> copies=new Dictionary<Texture2D,Texture2D>();
    public static AtlasSurvey CreateSurvey()
    {
        var s=new AtlasSurvey();
        var offsets=new[]{new Vector2(-136,40),new Vector2(-100,40),Vector2.zero,new Vector2(-62,40),Vector2.zero,new Vector2(-74,66),Vector2.zero,new Vector2(-30,100),Vector2.zero};
        string[] ids={"rodovia","posto_exterior","posto_interior","encruzilhada","mercado_interior","bairro","casa_interior","rua_norte","bar_interior"};
        string[] buildings={"","","posto","","mercadinho","","casa_avo","","bar_ze"};
        foreach(Capitulo1Director.ZonaId z in Enum.GetValues(typeof(Capitulo1Director.ZonaId)))
        {
            Capitulo1Director.TryObterInfoZona(z,out var info);int i=(int)z;
            var bounds=new Rect(info.min,info.max-info.min);
            s.regions.Add(new AtlasRegion{id=ids[i],scene="EntradaDaCidade",zone=i,name=info.titulo,interior=info.ehInterior,local=bounds,atlas=new Rect(offsets[i],bounds.size),building=buildings[i]});
        }
        foreach(string scene in SantaRitaChapter2Authoring.Scenes)
            s.regions.Add(new AtlasRegion{id=scene,scene=scene,name=scene,interior=scene!="CentroSantaRita"&&scene!="CemiterioSantaRita",building=SantaRitaGeography.IdDaCena(scene)});
        return s;
    }
    // Capturar é seguro na fixture do levantamento, nunca na sessão do autor.
    public static void Capture(AtlasSurvey survey, AtlasRegion region)
    {
        if(SceneManager.GetActiveScene().name!=region.scene)throw new Exception("Cena diferente do levantamento");
        if(region.zone<0)
        {
            var d=Object.FindAnyObjectByType<Capitulo2Director>();
            region.local=new Rect(d.limiteMin,d.limiteMax-d.limiteMin);
            region.atlas=region.local;region.name=d.nomeLocal;
        }
        Directory.CreateDirectory(Stage);
        var sprites=Object.FindObjectsByType<SpriteRenderer>()
            .Where(sr=>sr.enabled&&sr.sprite!=null&&sr.gameObject.scene==SceneManager.GetActiveScene()&&region.local.Overlaps(new Rect(sr.bounds.min,sr.bounds.size)))
            .Where(sr=>!Dynamic(sr)).OrderBy(sr=>SortingLayer.GetLayerValueFromID(sr.sortingLayerID)).ThenBy(sr=>sr.sortingOrder).ThenByDescending(sr=>sr.transform.position.y).ToArray();
        int w=Mathf.CeilToInt(region.local.width*32),h=Mathf.CeilToInt(region.local.height*32);
        var layers=new[]{new Color32[w*h],new Color32[w*h],new Color32[w*h]};
        // Tilemap: células/matrizes reais, não um asfalto reconstruído à mão.
        foreach(var tm in Object.FindObjectsByType<Tilemap>().OrderBy(t=>t.GetComponent<TilemapRenderer>().sortingOrder))
        {
            if(tm.gameObject.scene!=SceneManager.GetActiveScene())continue;
            foreach(var cell in tm.cellBounds.allPositionsWithin)
            {
                var sprite=tm.GetSprite(cell);if(sprite==null)continue;
                var center=tm.GetCellCenterWorld(cell);if(!region.local.Contains(center))continue;
                Matrix4x4 m=Matrix4x4.TRS(center,Quaternion.identity,Vector3.one)*tm.GetTransformMatrix(cell);
                Raster(layers[0],w,h,region.local,sprite,m,tm.GetColor(cell)*tm.color,false,false);
            }
        }
        foreach(var sr in sprites)
        {
            float size=Mathf.Max(sr.bounds.size.x,sr.bounds.size.y);
            int lod=size>=2?0:size>=.8f?1:2;
            // Terrenos de C1 são sprites gerados completos, não Tilemaps.
            Raster(layers[lod],w,h,region.local,sr.sprite,sr.transform.localToWorldMatrix,sr.color,sr.flipX,sr.flipY);
        }
        for(int lod=0;lod<3;lod++)
        {
            var t=new Texture2D(w,h,TextureFormat.RGBA32,false);t.SetPixels32(layers[lod]);t.Apply();
            File.WriteAllBytes(Stage+"/"+region.id+"_"+lod+".png",t.EncodeToPNG());Object.DestroyImmediate(t);
        }
        foreach(var p in Object.FindObjectsByType<Capitulo2Portal>(FindObjectsInactive.Include))
            if(region.local.Contains(p.transform.position))
            {
                Add(survey,region,p.name,p.transform.position,p.cenaDestino,p.flagNecessaria,false);
                survey.accesses.Add(new AtlasAccess{region=region.id,local=p.transform.position,destination=p.cenaDestino,entry=p.entradaDestino,condition=p.flagNecessaria});
            }
        foreach(var p in Object.FindObjectsByType<SpawnPoint>(FindObjectsInactive.Include))
            if(!survey.arrivals.Any(a=>a.scene==region.scene&&a.id==p.Id))survey.arrivals.Add(new AtlasArrival{scene=region.scene,id=p.Id,local=p.transform.position});
        foreach(var p in Object.FindObjectsByType<TransicaoDeZonaTrigger>(FindObjectsInactive.Include))
        {
            if(!region.local.Contains(p.transform.position))continue;
            var fields=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
            var dest=(Capitulo1Director.ZonaId)typeof(TransicaoDeZonaTrigger).GetField("zonaDestino",fields).GetValue(p);
            string id=dest==Capitulo1Director.ZonaId.Interior_Posto?"posto":dest==Capitulo1Director.ZonaId.Interior_Mercadinho?"mercadinho":dest==Capitulo1Director.ZonaId.Interior_CasaDoAvo?"casa_avo":dest==Capitulo1Director.ZonaId.Interior_BarDoZe?"bar_ze":p.name;
            Add(survey,region,id,p.transform.position,idsForZone(survey,(int)dest),"",false);
            survey.accesses.Add(new AtlasAccess{region=region.id,local=p.transform.position,destination=idsForZone(survey,(int)dest),arrival=(Vector2)typeof(TransicaoDeZonaTrigger).GetField("posicaoDestino",fields).GetValue(p),resolved=true});
        }
        foreach(var p in Object.FindObjectsByType<PayphoneCheckpoint>(FindObjectsInactive.Include))
            if(region.local.Contains(p.transform.position))Add(survey,region,"tel_"+region.id+"_"+p.name,p.transform.position,"","",true);
        if(region.id=="rua_norte")Gate(survey);
        // Aliases de orientação são a própria entrada real de cada estabelecimento.
        foreach(var l in survey.landmarks.Where(l=>l.region==region.id&&!region.interior).ToArray())
        {
            string alias=SantaRitaGeography.IdDaCena(l.destination);
            if(alias!=null&&!survey.landmarks.Any(x=>x.id==alias))Add(survey,region,alias,l.local,l.destination,l.condition,false);
        }
        File.WriteAllText(Stage+"/Survey.json",JsonUtility.ToJson(survey,true));
        File.AppendAllText(Path.GetFullPath("../Docs/Evidencias/Atlas/levantamento.csv"),$"{region.id};{region.scene};{region.zone};{region.local};{region.atlas};{sprites.Length}\n");
        foreach(var copy in copies.Values)Object.DestroyImmediate(copy);copies.Clear();pixels.Clear();
    }
    static string idsForZone(AtlasSurvey s,int zone)=>s.regions.Find(r=>r.zone==zone).id;
    static void Gate(AtlasSurvey s)
    {
        var gate=Object.FindObjectsByType<InteracaoCapitulo1>(FindObjectsInactive.Include).FirstOrDefault(p=>p.Tipo==InteracaoCapitulo1.TipoInteracao.LimiarCentro);
        if(gate==null)throw new Exception("Portão C1 não encontrado no mundo construído.");
        if(s.accesses.Any(a=>a.region=="rua_norte"&&a.destination=="CentroSantaRita"))return;
        s.accesses.Add(new AtlasAccess{region="rua_norte",local=gate.transform.position,destination="CentroSantaRita",entry="c1_portao_norte",condition=Flags.ZeDerrotadoNaSinuca});
    }
    public static void CompleteGate()
    {
        var s=JsonUtility.FromJson<AtlasSurvey>(File.ReadAllText(Stage+"/Survey.json"));Gate(s);
        foreach(var a in s.accesses.Where(a=>!a.resolved))
        {
            var spawn=s.arrivals.Find(p=>p.scene==a.destination&&p.id==a.entry);
            if(spawn==null)throw new Exception("Chegada de portão não encontrada");
            a.arrival=spawn.local;a.resolved=true;
        }
        File.WriteAllText(Stage+"/Survey.json",JsonUtility.ToJson(s,true));
    }
    static void Add(AtlasSurvey s,AtlasRegion r,string id,Vector2 p,string dest,string condition,bool phone)
    {
        if(s.landmarks.Any(l=>l.id==id))return;
        s.landmarks.Add(new AtlasLandmark{id=id,region=r.id,local=p,name=id.Replace('_',' '),destination=dest,condition=condition,phone=phone});
    }
    static bool Dynamic(SpriteRenderer sr)
    {
        if(sr.GetComponentInParent<PlayerController>()!=null||sr.GetComponentInParent<SantaRitaNpcRoutine>()!=null||sr.GetComponentInParent<SantaRitaNpcWork>()!=null)return true;
        string path=sr.name.ToLowerInvariant();
        for(var parent=sr.transform.parent;parent!=null;parent=parent.parent)path+="/"+parent.name.ToLowerInvariant();
        return path.Contains("npc")||path.Contains("sombra")||path.Contains("chuva")||path.Contains("percepcao")||path.Contains("lanterna")||sr.color.a<.05f;
    }
    static void Raster(Color32[] dst,int w,int h,Rect area,Sprite sprite,Matrix4x4 matrix,Color tint,bool fx,bool fy)
    {
        var source=sprite.texture;
        if(!pixels.TryGetValue(source,out var src))
        {
            var previous=RenderTexture.active;var rt=RenderTexture.GetTemporary(source.width,source.height,0,RenderTextureFormat.ARGB32);
            try{Graphics.Blit(source,rt);RenderTexture.active=rt;var copy=new Texture2D(source.width,source.height,TextureFormat.RGBA32,false);copy.ReadPixels(new Rect(0,0,source.width,source.height),0,0);copy.Apply();src=copy.GetPixels32();pixels[source]=src;copies[source]=copy;}
            finally{RenderTexture.active=previous;RenderTexture.ReleaseTemporary(rt);}
        }
        Rect tex=sprite.textureRect;var b=sprite.bounds;var inv=matrix.inverse;
        Vector3 a=matrix.MultiplyPoint3x4(b.min),z=matrix.MultiplyPoint3x4(b.max);
        int x0=Mathf.Clamp(Mathf.FloorToInt((Mathf.Min(a.x,z.x)-area.x)*32),0,w),x1=Mathf.Clamp(Mathf.CeilToInt((Mathf.Max(a.x,z.x)-area.x)*32),0,w);
        int y0=Mathf.Clamp(Mathf.FloorToInt((Mathf.Min(a.y,z.y)-area.y)*32),0,h),y1=Mathf.Clamp(Mathf.CeilToInt((Mathf.Max(a.y,z.y)-area.y)*32),0,h);
        for(int y=y0;y<y1;y++)for(int x=x0;x<x1;x++)
        {
            Vector3 p=inv.MultiplyPoint3x4(new Vector3(area.x+(x+.5f)/32,area.y+(y+.5f)/32));
            float u=(p.x-b.min.x)/b.size.x,v=(p.y-b.min.y)/b.size.y;if(u<0||u>=1||v<0||v>=1)continue;if(fx)u=1-u;if(fy)v=1-v;
            int tx=Mathf.Clamp((int)(tex.x+u*tex.width),0,source.width-1),ty=Mathf.Clamp((int)(tex.y+v*tex.height),0,source.height-1);
            Color c=src[ty*source.width+tx];c*=tint;if(c.a<.01f)continue;Color old=dst[y*w+x];float alpha=c.a+old.a*(1-c.a);
            dst[y*w+x]=new Color((c.r*c.a+old.r*old.a*(1-c.a))/alpha,(c.g*c.a+old.g*old.a*(1-c.a))/alpha,(c.b*c.a+old.b*old.a*(1-c.a))/alpha,alpha);
        }
    }
    public static void Import()
    {
        if(Application.isPlaying)throw new Exception("Publicar levantamento somente fora de Play.");
        Directory.CreateDirectory(Root);
        foreach(var texture in Resources.FindObjectsOfTypeAll<Texture2D>())
            if(AssetDatabase.GetAssetPath(texture).StartsWith(Root+"/"))Resources.UnloadAsset(texture);
        foreach(string staged in Directory.GetFiles(Stage).Where(f=>(f.EndsWith(".png")||f.EndsWith(".json"))&&!Path.GetFileName(f).StartsWith("previous-")&&!Path.GetFileName(f).StartsWith("publish-")))
        {
            string destination=Root+"/"+Path.GetFileName(staged);
            string temporary=Stage+"/publish-"+Path.GetFileName(staged);
            File.Copy(staged,temporary,true);
            if(File.Exists(destination))File.Replace(temporary,destination,Stage+"/previous-"+Path.GetFileName(staged));
            else File.Move(temporary,destination);
        }
        AssetDatabase.Refresh();
        foreach(string path in Directory.GetFiles(Root,"*.png"))
        {
            var i=(TextureImporter)AssetImporter.GetAtPath(path);i.textureType=TextureImporterType.Default;i.filterMode=FilterMode.Point;
            i.mipmapEnabled=false;i.textureCompression=TextureImporterCompression.Uncompressed;i.maxTextureSize=4096;i.wrapMode=TextureWrapMode.Clamp;i.SaveAndReimport();
        }
    }
    static AtlasSurvey pending;
    static int index;
    static double ready;
    static bool positioned;
    public static void BeginSurvey()
    {
        if(!Application.isPlaying)throw new Exception("Usar somente na fixture Play isolada do atlas.");
        pending=CreateSurvey();index=0;positioned=false;
        EditorApplication.isPaused=false;Application.runInBackground=true;
        GameState.Marcar(Flags.ZeDerrotadoNaSinuca);GameState.Marcar(Flags.MapaDoCarroObtido);GameState.Marcar(Flags.ChegouCasaAvo);GameState.Marcar(Flags.LanternaDoCarroObtida);
        Directory.CreateDirectory(Path.GetFullPath("../Docs/Evidencias/Atlas"));
        File.WriteAllText(Path.GetFullPath("../Docs/Evidencias/Atlas/levantamento.csv"),"regiao;cena;zona;limites_locais;limites_atlas;sprites\n");
        File.WriteAllText(Path.GetFullPath("../Docs/Evidencias/Atlas/bake.txt"),"RUNNING");
        SceneManager.LoadScene("EntradaDaCidade");ready=EditorApplication.timeSinceStartup+3;
        EditorApplication.update-=Tick;EditorApplication.update+=Tick;
    }
    static void Tick()
    {
        if(EditorApplication.timeSinceStartup<ready)return;
        try
        {
            if(!positioned&&pending.regions[index].zone>=0)
            {
                var r=pending.regions[index];Capitulo1Director.Instance.TransicionarPara((Capitulo1Director.ZonaId)r.zone,r.local.center,0);
                positioned=true;ready=EditorApplication.timeSinceStartup+2;return;
            }
            Capture(pending,pending.regions[index]);index++;
            positioned=false;
            if(index==pending.regions.Count)
            {
                EditorApplication.update-=Tick;
                foreach(var access in pending.accesses.Where(a=>!a.resolved))
                {
                    var spawn=pending.arrivals.Find(a=>a.scene==access.destination&&a.id==access.entry);
                    if(spawn==null)continue;
                    var target=pending.regions.Find(r=>r.scene==spawn.scene&&r.local.Contains(spawn.local));
                    if(target==null)continue;
                    access.destination=target.id;access.arrival=spawn.local;access.resolved=true;
                }
                File.WriteAllText(Stage+"/Survey.json",JsonUtility.ToJson(pending,true));
                File.WriteAllText(Path.GetFullPath("../Docs/Evidencias/Atlas/bake.txt"),"PASS "+index+" regiões capturadas, sem builders ou save.");return;
            }
            if(pending.regions[index].scene!=SceneManager.GetActiveScene().name)
                SceneManager.LoadScene(pending.regions[index].scene);
            ready=EditorApplication.timeSinceStartup+1;
        }
        catch(Exception e){EditorApplication.update-=Tick;Debug.LogException(e);File.WriteAllText(Path.GetFullPath("../Docs/Evidencias/Atlas/bake.txt"),e.ToString());}
    }
}

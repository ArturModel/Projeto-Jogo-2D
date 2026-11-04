using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Georreferenciamento do levantamento. Sem coordenadas da ilustração histórica.
[Serializable] public sealed class AtlasRegion
{
    public string id, scene, name, building;
    public int zone = -1;
    public bool interior;
    public Rect local, atlas;
    public Vector2 ToAtlas(Vector2 p) => atlas.min + Vector2.Scale(p-local.min, new Vector2(atlas.width/local.width, atlas.height/local.height));
    public Vector2 ToLocal(Vector2 p) => local.min + Vector2.Scale(p-atlas.min, new Vector2(local.width/atlas.width, local.height/atlas.height));
}
[Serializable] public sealed class AtlasLandmark
{
    public string id, region, name, destination, condition;
    public Vector2 local;
    public bool phone;
}
[Serializable] public sealed class AtlasSurvey
{
    public string city = "Santa Rita";
    public List<AtlasRegion> regions = new List<AtlasRegion>();
    public List<AtlasLandmark> landmarks = new List<AtlasLandmark>();
    public List<AtlasAccess> accesses = new List<AtlasAccess>();
    public List<AtlasArrival> arrivals = new List<AtlasArrival>();
}
[Serializable] public sealed class AtlasAccess
{
    public string region, destination, entry, condition;
    public Vector2 local, arrival;
    public bool resolved;
}
[Serializable] public sealed class AtlasArrival
{
    public string scene, id;
    public Vector2 local;
}

public static class SantaRitaAtlas
{
    public static readonly Rect Extent = new Rect(-138, -10, 241, 140);
    private static AtlasSurvey survey;
    public static AtlasSurvey Survey => survey ?? (survey = Load());
    private static AtlasSurvey Load()
    {
        var data = Resources.Load<TextAsset>("Atlas/Survey");
        return data != null ? JsonUtility.FromJson<AtlasSurvey>(data.text) : new AtlasSurvey();
    }
    public static Vector2 UV(Vector2 atlas) => new Vector2((atlas.x-Extent.x)/Extent.width,(atlas.y-Extent.y)/Extent.height);
    public static AtlasRegion Region(string id) => Survey.regions.Find(r=>r.id==id);
    public static AtlasRegion Current()
    {
        string scene=SceneManager.GetActiveScene().name;
        if(scene=="CasaDoAvo")return Region("casa_interior");
        if(scene=="BarDoZe")return Region("bar_interior");
        return Survey.regions.Find(r=>r.scene==scene && (r.zone<0 || r.zone==(int)Capitulo1Director.ZonaAtual));
    }
    public static Vector2 Player(Vector2 p, out string name)
    {
        var r=Current(); name=r!=null?r.name:"Santa Rita";
        if(r==null)return new Vector2(.5f,.5f);
        if(r.interior)
        {
            string entry=GameState.Texto("atlas.entry."+r.id);
            var entrance=Survey.landmarks.Find(l=>l.region==entry&&Building(l)==r.building);
            if(entrance!=null)return UV(Region(entrance.region).ToAtlas(entrance.local));
            if(Landmark(r.building,out Vector2 door))return UV(door);
        }
        return UV(r.ToAtlas(p));
    }
    public static string Building(AtlasLandmark l)
    {
        var target=Region(l.destination)??Survey.regions.Find(r=>r.scene==l.destination&&r.interior);
        return target!=null?target.building:null;
    }
    public static bool Landmark(string id, out Vector2 atlas)
    {
        var l=Survey.landmarks.Find(x=>x.id==id);
        var r=l!=null?Region(l.region):null;
        atlas=r!=null?r.ToAtlas(l.local):Vector2.zero;
        return r!=null;
    }
    public static bool Known(AtlasRegion r) => !r.interior || GameState.Tem("atlas.visit."+r.id) ||
        (r.building=="casa_avo" && GameState.Tem(Flags.ChegouCasaAvo));
    public static string ObjectiveId()
    {
        string text=(GameState.ObterObjetivo()??"").ToLowerInvariant();
        if(text.Contains("prefeitura"))return "prefeitura";
        if(text.Contains("igreja"))return "igreja";
        if(text.Contains("cemitério"))return "cemiterio";
        if(text.Contains("beira-rio")||text.Contains("tonão"))return "canto_se";
        if(text.Contains("rodoviária"))return "rodoviaria";
        if(text.Contains("mercadinho"))return "mercadinho";
        if(text.Contains("bar do zé"))return "bar_ze";
        if(text.Contains("casa")&&text.Contains("ad"))return "casa_avo";
        return null;
    }
    public static string Annotation(string id)
    {
        if(id=="mercadinho" && GameState.Tem(Flags.AchouReciboMercadinho))return "Tenho um recibo desta venda. Posso perguntar pela compra.";
        if(id=="bar_ze" && GameState.Tem(Flags.ZeDerrotadoNaSinuca))return "A passagem da Rua Norte está aberta.";
        if(id=="prefeitura" && GameState.Tem("c2_protocolo_adilio"))return "Consultei o protocolo de Adílio. Conferir com os demais registros.";
        if(id=="cemiterio" && GameState.Tem("c2_cemiterio_comparado"))return "Comparei os registros. Ainda preciso entender as diferenças.";
        if(id=="canto_se" && GameState.Tem(Capitulo2Director.Vitoria))return "Tonão entregou sua via do registro.";
        return "";
    }
}

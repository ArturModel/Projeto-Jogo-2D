using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Camadas autoradas uma vez; abrir o mapa não renderiza cenas nem cria texturas.
public sealed class SantaRitaAtlasView : MonoBehaviour
{
    public string Interior { get; private set; }
    private readonly List<GameObject> content=new List<GameObject>();
    private readonly List<KeyValuePair<GameObject,int>> levels=new List<KeyValuePair<GameObject,int>>();
    private Action<string> select;
    private RectTransform root;
    private float lastZoom=-1;
    private Vector2 lastSize;
    private readonly List<GameObject> annotations=new List<GameObject>();
    private readonly List<RectTransform> labels=new List<RectTransform>();
    public void Initialize(RectTransform container,Action<string> callback){root=container;select=callback;Rebuild();}
    public void ShowInterior(string id){Interior=id;Rebuild();}
    public Vector2 Project(Vector2 p)
    {
        var r=SantaRitaAtlas.Region(Interior);
        return r==null?SantaRitaAtlas.Player(p,out _):new Vector2((p.x-r.local.x)/r.local.width,(p.y-r.local.y)/r.local.height);
    }
    public void Rebuild()
    {
        foreach(var go in content){go.SetActive(false);Destroy(go);}content.Clear();levels.Clear();annotations.Clear();labels.Clear();
        foreach(var r in SantaRitaAtlas.Survey.regions)
        {
            if(Interior==null?r.interior:r.id!=Interior)continue;
            if(r.interior&&!SantaRitaAtlas.Known(r))continue;
            Vector2 min=Interior==null?SantaRitaAtlas.UV(r.atlas.min):Vector2.zero;
            Vector2 max=Interior==null?SantaRitaAtlas.UV(r.atlas.max):Vector2.one;
            for(int lod=0;lod<3;lod++)
            {
                var tex=Resources.Load<Texture2D>("Atlas/"+r.id+"_"+lod);if(tex==null)continue;
                var go=new GameObject("Levantamento_"+r.id+"_"+lod,typeof(RectTransform),typeof(RawImage));go.transform.SetParent(root,false);
                var rect=(RectTransform)go.transform;rect.anchorMin=min;rect.anchorMax=max;rect.offsetMin=rect.offsetMax=Vector2.zero;
                var image=go.GetComponent<RawImage>();image.texture=tex;image.raycastTarget=false;
                content.Add(go);levels.Add(new KeyValuePair<GameObject,int>(go,lod));
            }
            if(Interior==null)Label(RegionName(r),r.atlas.center+Vector2.up*(r.atlas.height*.46f),0);
        }
        var occupied=new HashSet<string>();
        if(Interior==null&&SantaRitaAtlas.Landmark(SantaRitaAtlas.ObjectiveId(),out Vector2 objective))Label("+ Objetivo conhecido",objective+Vector2.up*4,0);
        foreach(var l in SantaRitaAtlas.Survey.landmarks)
        {
            var r=SantaRitaAtlas.Region(l.region);if(r==null || (Interior==null?r.interior:r.id!=Interior))continue;
            string key=r.id+Mathf.Round(l.local.x*2)+":"+Mathf.Round(l.local.y*2);
            if(!occupied.Add(key))continue;
            Vector2 uv=Interior==null?SantaRitaAtlas.UV(r.ToAtlas(l.local)):new Vector2((l.local.x-r.local.x)/r.local.width,(l.local.y-r.local.y)/r.local.height);
            var go=new GameObject("Acesso_"+l.id,typeof(RectTransform),typeof(Image),typeof(Button));go.transform.SetParent(root,false);
            var rect=(RectTransform)go.transform;rect.anchorMin=rect.anchorMax=uv;rect.sizeDelta=new Vector2(8,8);rect.anchoredPosition=Vector2.zero;
            go.GetComponent<Image>().color=l.phone?new Color(.56f,.7f,.56f):new Color(.76f,.66f,.43f);
            string id=l.id;go.GetComponent<Button>().onClick.AddListener(()=>select(id));content.Add(go);levels.Add(new KeyValuePair<GameObject,int>(go,1));
            var destination=SantaRitaAtlas.Region(l.destination)??SantaRitaAtlas.Survey.regions.Find(x=>x.scene==l.destination);
            if(destination!=null&&!r.interior&&destination.interior)
                Label(destination.name.Split('—')[0].Trim(),r.ToAtlas(l.local)+Vector2.down*2,1);
            string note=SantaRitaAtlas.Annotation(SantaRitaAtlas.Building(l)??id);
            if(!string.IsNullOrEmpty(note))
            {
                var sketchTexture=Resources.Load<Texture2D>("Notebook/Sketch");
                var sketch=new GameObject("Rabisco_"+id,typeof(RectTransform),typeof(RawImage));sketch.transform.SetParent(root,false);
                var markRect=(RectTransform)sketch.transform;markRect.anchorMin=markRect.anchorMax=uv;markRect.sizeDelta=new Vector2(28,21);markRect.anchoredPosition=Vector2.zero;
                sketch.GetComponent<RawImage>().texture=sketchTexture;sketch.GetComponent<RawImage>().raycastTarget=false;content.Add(sketch);levels.Add(new KeyValuePair<GameObject,int>(sketch,2));
                Label("Anotação de Marcus",r.ToAtlas(l.local)+Vector2.up*2,2);
                annotations.Add(content[content.Count-1]);
            }
        }
        var player=root.Find("MarcadorMarcus");if(player!=null)player.SetAsLastSibling();
        lastZoom=-1;
    }
    private void Label(string text,Vector2 atlas,int lod)
    {
        var go=new GameObject("Nome_"+text,typeof(RectTransform),typeof(Image));go.transform.SetParent(root,false);
        var rect=(RectTransform)go.transform;rect.anchorMin=rect.anchorMax=SantaRitaAtlas.UV(atlas);rect.anchoredPosition=Vector2.zero;rect.sizeDelta=new Vector2(text.Length>22?132:100,16);
        go.GetComponent<Image>().color=new Color(.08f,.105f,.08f,.91f);go.GetComponent<Image>().raycastTarget=false;
        var label=new GameObject("Texto",typeof(RectTransform),typeof(TextMeshProUGUI));label.transform.SetParent(go.transform,false);UIFactory.Stretch((RectTransform)label.transform);
        var tmp=label.GetComponent<TextMeshProUGUI>();tmp.font=UIFactory.Font;tmp.fontSize=11;tmp.alignment=TextAlignmentOptions.Center;tmp.color=new Color(.86f,.83f,.68f);tmp.text=text;tmp.raycastTarget=false;tmp.overflowMode=TextOverflowModes.Ellipsis;
        content.Add(go);levels.Add(new KeyValuePair<GameObject,int>(go,lod));labels.Add(rect);
    }
    public void Detail(float zoom,bool icons)
    {
        if(Mathf.Approximately(lastZoom,zoom)&&lastSize==root.rect.size)return;lastZoom=zoom;lastSize=root.rect.size;
        foreach(var item in levels)item.Key.SetActive((item.Value==0 || zoom>=(item.Value==1?1.8f:3.5f)) && (icons || !item.Key.name.StartsWith("Acesso_")));
        var used=new List<Rect>();
        foreach(var rect in labels)
        {
            if(!rect.gameObject.activeSelf)continue;
            Vector2 p=Vector2.Scale(rect.anchorMin,root.rect.size);
            Rect bounds=new Rect(p-rect.sizeDelta*.5f,rect.sizeDelta+Vector2.one*3);
            if(used.Exists(r=>r.Overlaps(bounds)))rect.gameObject.SetActive(false);else used.Add(bounds);
        }
    }
    public void Invalidate()=>lastZoom=-1;
    private static string RegionName(AtlasRegion r)
    {
        switch(r.id){case "rodovia":return "ENTRADA";case "posto_exterior":return "POSTO";case "encruzilhada":return "ENCRUZILHADA";case "bairro":return "BAIRRO DAS CASAS";case "rua_norte":return "RUA NORTE";case "CentroSantaRita":return "CENTRO";case "CemiterioSantaRita":return "CEMITÉRIO";default:return r.name;}
    }
}

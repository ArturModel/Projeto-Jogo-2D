using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

// Bag and quick belt share one state. Possession comes from canonical flags; ordering stays in snapshot RAM.
public sealed class SantaRitaInventory : MonoBehaviour
{
    public static SantaRitaInventory Instance {get;private set;}
    public static int ClosedFrame {get;private set;} = -1;
    public static bool IsOpen=>Instance!=null&&Instance.bag!=null&&Instance.bag.activeSelf;
    public const int SlotCount=5;
    private Canvas canvas;private GameObject belt,bag;private Transform grid;
    private Image[] pictures=new Image[5];private Button[] buttons=new Button[5];
    private Sprite[] icons;private TextMeshProUGUI tooltip,description;private string selected="";private int hovered=-1;
    private string stamp="";private readonly Dictionary<string,Sprite> extras=new Dictionary<string,Sprite>();
    public static readonly string[] Catalog={"lanterna","mapa","recibo","tocador","fita01","fita02","fita03","fita_sociedade","bilhete"};
    public static bool Owns(string id)
    {
        switch(id){case "lanterna":return GameState.Tem(Flags.LanternaDoCarroObtida);case "mapa":return GameState.Tem(Flags.MapaDoCarroObtido);
        case "recibo":return GameState.Tem(Flags.AchouReciboMercadinho)&&!GameState.Tem("recibo_entregue")&&!GameState.Tem(Flags.ApresentouReciboAtendente);
        case "bilhete":return GameState.Tem(Flags.CasaAvo_LeuBilhete);default:return Catalog.Contains(id)&&GameState.Tem("item."+id);}
    }
    public static bool HasItem(int i)=>i>=0&&i<3&&Owns(Catalog[i]); // historical contract used by callers/tests
    public static string Name(string id){switch(id){case "lanterna":return "Lanterna";case "mapa":return "Mapa da cidade";case "recibo":return "Recibo do mercadinho";case "tocador":return "Toca-fitas de Mila";case "bilhete":return "Bilhete do vô";case "fita_sociedade":return "Fita: um domingo na Sociedade";default:return "Fita "+id.Replace("fita","")+" — Júlia";}}
    public static string Quick(int slot)
    {
        string id=GameState.Texto("bolsa.atalho."+slot,slot<3?Catalog[slot]:slot==3?"tocador":"fita01");return Owns(id)?id:"";
    }
    public static void Assign(int slot,string id)
    {
        if(slot<0||slot>=5||!Owns(id))return;
        string old=Quick(slot);
        for(int i=0;i<5;i++)if(i!=slot&&Quick(i)==id)GameState.DefinirTexto("bolsa.atalho."+i,old);
        GameState.DefinirTexto("bolsa.atalho."+slot,id);
    }
    public static string[] Contents()
    {
        var saved=GameState.Texto("bolsa.ordem").Split('|');return saved.Where(Owns).Concat(Catalog.Where(Owns)).Distinct().ToArray();
    }
    public static void Swap(string a,string b)
    {
        if(!Owns(a)||!Owns(b))return;var all=Contents();int x=Array.IndexOf(all,a),y=Array.IndexOf(all,b);all[x]=b;all[y]=a;GameState.DefinirTexto("bolsa.ordem",string.Join("|",all));
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void StartSession(){if(FindAnyObjectByType<SantaRitaInventory>()==null){var g=new GameObject("Inventario");DontDestroyOnLoad(g);g.AddComponent<SantaRitaInventory>();}}
    private void Awake()
    {
        Instance=this;canvas=UIFactory.CreateCanvas(transform,"Itens",8600);canvas.gameObject.AddComponent<GraphicRaycaster>();icons=LoadIcons();
        belt=Frame(canvas.transform,"CincoAtalhos",new Vector2(0,-159),new Vector2(150,34));
        for(int i=0;i<5;i++){int n=i;buttons[i]=Cell(belt.transform,"Atalho"+i,new Vector2((i-2)*28,0),25,()=>{if(IsOpen){if(selected!="")Assign(n,selected);stamp="";}else Use(n);});pictures[i]=Picture(buttons[i].transform);var h=buttons[i].gameObject.AddComponent<SantaRitaItemHover>();h.enter=()=>hovered=n;h.exit=()=>hovered=-1;}
        tooltip=Label(canvas.transform,"Dica",new Vector2(0,-132),new Vector2(300,16),11);
        var open=Cell(canvas.transform,"AbrirBolsa",new Vector2(91,-159),25,()=>Toggle());Label(open.transform,"I",Vector2.zero,new Vector2(24,24),14).text="I";
        bag=Frame(canvas.transform,"Bolsa",new Vector2(0,12),new Vector2(490,270));
        Label(bag.transform,"Titulo",new Vector2(0,112),new Vector2(460,25),20).text="PERTENCES";
        Label(bag.transform,"Orientacao",new Vector2(0,87),new Vector2(450,20),12).text="Escolha e use/examine. Clique em outro objeto para trocar ou num atalho de baixo.";
        var g=new GameObject("Grade",typeof(RectTransform));g.transform.SetParent(bag.transform,false);grid=g.transform;
        description=Label(bag.transform,"Descricao",new Vector2(0,-65),new Vector2(440,45),14);
        ActionButton(bag.transform,"Usar / examinar",new Vector2(-110,-110),()=>{if(selected!=""){string id=selected;Close();UseItem(id);}});
        ActionButton(bag.transform,"Fechar [I/Esc]",new Vector2(110,-110),Close);
        bag.SetActive(false);
    }
    public void Toggle(){if(IsOpen){Close();return;}if(Blocked())return;MapaInterativoUI.FecharSeAberto();bag.SetActive(true);stamp="";Rebuild();}
    public void Close(){if(IsOpen)ClosedFrame=Time.frameCount;bag.SetActive(false);selected="";}
    private bool Blocked()=>MenuPausa.Aberto||GameMessageUI.IsShowing||PayphoneCheckpoint.IsSaveUIOpen||SinucaMinigameUI.IsShowing||BochaMinigameUI.IsShowing||SantaRitaTapeDeck.IsOpen;
    public void Use(int i){if(i<0||i>=5||Blocked()||MapaInterativoUI.IsShowing)return;UseItem(Quick(i));}
    public void UseItem(string id)
    {
        if(!Owns(id))return;
        if(id=="lanterna"){GameState.AlternarLanterna();return;}if(id=="mapa"){MapaInterativoUI.EnsureInstance().Abrir(0);return;}
        if(id=="tocador"||id.StartsWith("fita")){SantaRitaTapeDeck.Ensure().Open(id);return;}
        string body=id=="recibo"?"Mercadinho Santa Rita, 19/10/2004, 18h10. Café, velas e fósforo. No verso, o aviso sobre o homem de capa. O original pode ser entregue no mercadinho.":"Papel guardado na casa do vô. A anotação original permanece no caderno; vou conservar este bilhete junto dos meus pertences.";
        GameMessageUI.Instance.ShowInspection(Name(id),null,body,arteCustom:Icon(id));
    }
    private void Update()
    {
        var k=Keyboard.current;if(k==null)return;
        if(k.iKey.wasPressedThisFrame)Toggle();
        else if(IsOpen&&k.escapeKey.wasPressedThisFrame)Close();
    }
    private void LateUpdate()
    {
        string scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        bool show=scene!="MainMenu"&&scene!="Prologo"&&!Blocked()&&!MapaInterativoUI.IsShowing;
        canvas.enabled=show;if(!show&&IsOpen)Close();
        for(int i=0;i<5;i++){var id=Quick(i);pictures[i].sprite=Icon(id);pictures[i].enabled=pictures[i].sprite!=null;}
        tooltip.text=IsOpen?"ACESSO RÁPIDO — clique para trocar o objeto":hovered>=0&&Quick(hovered)!=""?Name(Quick(hovered)):"";
        if(IsOpen)Rebuild();
    }
    private void Rebuild()
    {
        string key=string.Join("|",Contents())+":"+selected+":"+string.Join("|",Enumerable.Range(0,5).Select(Quick));if(key==stamp)return;stamp=key;
        foreach(Transform child in grid)Destroy(child.gameObject);
        var items=Contents();
        for(int i=0;i<20;i++){string id=i<items.Length?items[i]:"";var b=Cell(grid,"Objeto"+i,new Vector2((i%5-2)*43,49-(i/5)*30),27,()=>{if(selected!=""&&id!=""&&selected!=id){Swap(selected,id);selected="";}else selected=id;stamp="";});var picture=Picture(b.transform);picture.sprite=Icon(id);picture.enabled=picture.sprite!=null;b.GetComponent<Image>().color=id!=""&&selected==id?new Color(.32f,.28f,.19f):new Color(.11f,.115f,.102f);}
        description.text=selected==""?(items.Length==0?"A bolsa está vazia.":"Os espaços vazios ficam livres para os próximos achados."):Name(selected)+"\nUsar/examinar abre o objeto; os cinco atalhos podem ser reorganizados.";
    }
    public Sprite Icon(string id)
    {
        if(string.IsNullOrEmpty(id))return null;
        int index=Array.IndexOf(Catalog,id);if(index>=0&&index<3&&icons!=null)return icons[index];
        if(extras.TryGetValue(id,out var cached))return cached;
        var texture=Resources.Load<Texture2D>("Tapes/"+(id.StartsWith("fita")?"Cassette":id=="tocador"?"Recorder":"Letter"));
        if(texture==null)return null;var sprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(.5f,.5f),32);extras[id]=sprite;return sprite;
    }
    public static GameObject Frame(Transform parent,string name,Vector2 pos,Vector2 size)
    {
        var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);Place((RectTransform)go.transform,pos,size);go.GetComponent<Image>().color=new Color(.48f,.40f,.31f);
        var inner=new GameObject("Couro",typeof(RectTransform),typeof(Image));inner.transform.SetParent(go.transform,false);UIFactory.Stretch((RectTransform)inner.transform);((RectTransform)inner.transform).offsetMin=Vector2.one;((RectTransform)inner.transform).offsetMax=-Vector2.one;inner.GetComponent<Image>().color=new Color(.055f,.058f,.053f,.98f);inner.GetComponent<Image>().raycastTarget=false;return go;
    }
    public static Button Cell(Transform parent,string name,Vector2 pos,float size,UnityEngine.Events.UnityAction click){var go=Frame(parent,name,pos,Vector2.one*size);var b=go.AddComponent<Button>();b.targetGraphic=go.GetComponent<Image>();b.onClick.AddListener(click);return b;}
    public static Button ActionButton(Transform parent,string text,Vector2 pos,UnityEngine.Events.UnityAction click){var b=Cell(parent,text,pos,24,click);((RectTransform)b.transform).sizeDelta=new Vector2(180,24);Label(b.transform,"Texto",Vector2.zero,new Vector2(177,24),13).text=text;return b;}
    public static TextMeshProUGUI Label(Transform parent,string name,Vector2 pos,Vector2 size,float font){var l=UIFactory.CreateLabel(parent,name,font);Place(l.rectTransform,pos,size);l.alignment=TextAlignmentOptions.Center;return l;}
    private static Image Picture(Transform parent){var g=new GameObject("Icone",typeof(RectTransform),typeof(Image));g.transform.SetParent(parent,false);var i=g.GetComponent<Image>();i.preserveAspect=true;i.raycastTarget=false;UIFactory.Stretch(i.rectTransform);i.rectTransform.offsetMin=Vector2.one*3;i.rectTransform.offsetMax=-Vector2.one*3;return i;}
    public static void Place(RectTransform r,Vector2 pos,Vector2 size){r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=pos;r.sizeDelta=size;}
    private static Sprite[] LoadIcons()
    {
        var source=Resources.Load<Texture2D>("Art/BarRef7/ItensAtlas");var result=new Sprite[5];if(source==null)return result;
        var pixels=source.GetPixels32();var columns=new bool[source.width];for(int y=0;y<source.height;y++)for(int x=0;x<source.width;x++)if(pixels[y*source.width+x].a>32)columns[x]=true;
        var ranges=new List<Vector2Int>();int start=-1,gap=0;
        for(int x=0;x<columns.Length;x++){if(columns[x]){if(start<0)start=x;gap=0;}else if(start>=0&&++gap>4){ranges.Add(new Vector2Int(start,x-gap+1));start=-1;}}
        if(start>=0)ranges.Add(new Vector2Int(start,source.width));
        for(int i=0;i<5;i++){int left=ranges.Count==5?ranges[i].x:i*source.width/5,right=ranges.Count==5?ranges[i].y:(i+1)*source.width/5,minY=source.height,maxY=0;for(int y=0;y<source.height;y++)for(int x=left;x<right;x++)if(pixels[y*source.width+x].a>32){minY=Mathf.Min(minY,y);maxY=Mathf.Max(maxY,y);}if(maxY<minY)continue;result[i]=Sprite.Create(source,new Rect(left,minY,right-left,maxY-minY+1),new Vector2(.5f,.5f),32);}
        return result;
    }
}
public sealed class SantaRitaItemHover:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public Action enter,exit;public void OnPointerEnter(PointerEventData e)=>enter?.Invoke();public void OnPointerExit(PointerEventData e)=>exit?.Invoke();
}

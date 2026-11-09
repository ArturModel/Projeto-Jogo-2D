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
    private CanvasGroup bagGroup; private float bagEntrance;
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
        Instance = this;
        canvas = UIFactory.CreateCanvas(transform, "Itens", 8600);
        canvas.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        icons = LoadIcons();

        belt = Frame(canvas.transform, "CincoAtalhos", Vector2.zero, new Vector2(198, 30));
        StyleInventorySurface(belt, .78f);
        TopLeft((RectTransform)belt.transform, 14, 14);
        for (int i = 0; i < SlotCount; i++)
        {
            int slot = i;
            buttons[i] = Cell(belt.transform, "Atalho" + i, new Vector2(-84 + i * 25, 0), 22, () =>
            {
                if (!IsOpen) { Use(slot); return; }
                if (selected != "") Assign(slot, selected);
                stamp = "";
            });
            StyleInventorySurface(buttons[i].gameObject, .54f);
            pictures[i] = Picture(buttons[i].transform);
            var hover = buttons[i].gameObject.AddComponent<SantaRitaItemHover>();
            hover.enter = () => hovered = slot;
            hover.exit = () => hovered = -1;
        }

        var open = Cell(belt.transform, "AbrirBolsa", new Vector2(63, 0), 22, Toggle);
        StyleInventorySurface(open.gameObject, .91f);
        ((RectTransform)open.transform).sizeDelta = new Vector2(62, 22);
        Label(open.transform, "Acao", Vector2.zero, new Vector2(60, 20), 12).text = "BOLSA  [I]";
        tooltip = Label(canvas.transform, "Dica", Vector2.zero, new Vector2(220, 16), 11);
        TopLeft(tooltip.rectTransform, 15, 46);
        tooltip.alignment = TextAlignmentOptions.MidlineLeft;

        bag = Frame(canvas.transform, "Bolsa", Vector2.zero, new Vector2(238, 203));
        StyleInventorySurface(bag, .94f);
        TopLeft((RectTransform)bag.transform, 14, 56);
        bagGroup = bag.AddComponent<CanvasGroup>();
        var title = Label(bag.transform, "Titulo", new Vector2(0, 83), new Vector2(210, 23), 19);
        title.text = "Pertences";
        title.color = new Color(.95f, .80f, .58f);
        var divider = new GameObject("Costura", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        divider.transform.SetParent(bag.transform, false);
        Place(divider.rectTransform, new Vector2(0, 69), new Vector2(210, 1));
        divider.color = new Color(.75f, .55f, .30f, .44f);
        divider.raycastTarget = false;
        title.alignment = TextAlignmentOptions.MidlineLeft;
        var g = new GameObject("Grade", typeof(RectTransform));
        g.transform.SetParent(bag.transform, false);
        grid = g.transform;
        description = Label(bag.transform, "Descricao", new Vector2(0, -51), new Vector2(210, 27), 12);
        description.alignment = TextAlignmentOptions.MidlineLeft;
        var use = ActionButton(bag.transform, "Usar / examinar", new Vector2(-45, -79),
            () => { if (selected != "") { string id = selected; Close(); UseItem(id); } });
        StyleInventorySurface(use.gameObject, .92f);
        ((RectTransform)use.transform).sizeDelta = new Vector2(118, 23);
        var close = ActionButton(bag.transform, "Fechar [Esc]", new Vector2(63, -79), Close);
        StyleInventorySurface(close.gameObject, .74f);
        ((RectTransform)close.transform).sizeDelta = new Vector2(88, 23);
        bag.SetActive(false);
    }

    private static void TopLeft(RectTransform rect, float x, float y)
    {
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(x, -y);
    }

    public void Toggle(){if(IsOpen){Close();return;}if(Blocked())return;MapaInterativoUI.FecharSeAberto();bagEntrance = 0; bagGroup.alpha = 0; bag.SetActive(true);stamp="";Rebuild();}
    public void Close(){if(IsOpen)ClosedFrame=Time.frameCount;bag.SetActive(false);selected=""; hovered=-1;}
    private bool Blocked()=>SceneFader.IsTransitioning||MenuPausa.Aberto||GameMessageUI.IsShowing||PayphoneCheckpoint.IsSaveUIOpen||SinucaMinigameUI.IsShowing||BochaMinigameUI.IsShowing||SantaRitaTapeDeck.IsOpen;
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
        if(canvas.enabled && k.iKey.wasPressedThisFrame)Toggle();
        else if(IsOpen&&k.escapeKey.wasPressedThisFrame)Close();
    }
    private void LateUpdate()
    {
        string scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        bool show=scene!="MainMenu"&&scene!="Prologo"&&!Blocked()&&!MapaInterativoUI.IsShowing;
        canvas.enabled=show;
        if (!show) { if (IsOpen) Close(); hovered = -1; return; }
        for(int i=0;i<5;i++){var id=Quick(i);pictures[i].sprite=Icon(id);pictures[i].enabled=pictures[i].sprite!=null;}
        tooltip.text = IsOpen ? "Clique num atalho para equipar" :
            hovered >= 0 && Quick(hovered) != "" ? Name(Quick(hovered)) : "";
        if (IsOpen)
        {
            bagEntrance = Mathf.MoveTowards(bagEntrance, 1, Time.unscaledDeltaTime / .12f);
            bagGroup.alpha = bagEntrance;
            TopLeft((RectTransform)bag.transform, 14, Mathf.Lerp(60, 64, bagEntrance));
            Rebuild();
        }
    }
    private void Rebuild()
    {
        string key=string.Join("|",Contents())+":"+selected+":"+string.Join("|",Enumerable.Range(0,5).Select(Quick));if(key==stamp)return;stamp=key;
        foreach(Transform child in grid)Destroy(child.gameObject);
        var items=Contents();
        for (int i = 0; i < 20; i++)
        {
            string id = i < items.Length ? items[i] : "";
            var cell = Cell(grid, "Objeto" + i, new Vector2((i % 5 - 2) * 39, 52 - i / 5 * 24), 22, () =>
            {
                if (selected != "" && id != "" && selected != id) { Swap(selected, id); selected = ""; }
                else selected = id;
                stamp = "";
            });
            StyleInventorySurface(cell.gameObject, id == "" ? .48f : .92f);
            if (id != "" && selected == id) cell.image.color = new Color(1.65f, 1.24f, .72f);
            var picture = Picture(cell.transform);
            picture.sprite = Icon(id);
            picture.enabled = picture.sprite != null;
        }
        description.text = selected == "" ? (items.Length == 0 ? "A bolsa está vazia." : "Selecione um objeto para examinar.") :
            Name(selected) + "\nEscolha outro objeto para trocar de lugar.";
    }
    public Sprite Icon(string id)
    {
        if(string.IsNullOrEmpty(id))return null;
        int index=Array.IndexOf(Catalog,id);if(index>=0&&index<3&&icons!=null)return icons[index];
        if(extras.TryGetValue(id,out var cached))return cached;
        var texture=Resources.Load<Texture2D>("Tapes/"+(id.StartsWith("fita")?"Cassette":id=="tocador"?"Recorder":"Letter"));
        if(texture==null)return null;var sprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(.5f,.5f),32);extras[id]=sprite;return sprite;
    }
    private static void StyleInventorySurface(GameObject frame, float opacity)
    {
        frame.transform.Find("Couro").gameObject.SetActive(false);
        UIFactory.ApplyWeatheredSurface(frame.GetComponent<Image>(), opacity);
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

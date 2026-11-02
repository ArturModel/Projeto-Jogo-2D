using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// Five contextual slots derived from save-compatible session state. No independent disk storage.
public sealed class SantaRitaInventory : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] labels=new TextMeshProUGUI[5];
    private readonly string[] names={"LANTERNA","MAPA","RECIBO","FICHA 01","FICHA 02"};
    [SerializeField] private Canvas canvas;
    private Sprite[] icons;
    private Image[] pictures=new Image[5];
    private TextMeshProUGUI tooltip;
    private int hovered=-1;
    private float nextIconRetry;
    public const int SlotCount=5;
    public static bool HasItem(int i)
    {
        switch(i){case 0:return GameState.Tem(Flags.LanternaDoCarroObtida);case 1:return GameState.Tem(Flags.MapaDoCarroObtido);
        case 2:return GameState.Tem(Flags.AchouReciboMercadinho)&&!GameState.Tem("recibo_entregue")&&!GameState.Tem(Flags.ApresentouReciboAtendente);
        case 3:case 4:return false;default:return false;}
    }
    public void Use(int i)
    {
        if(!canvas.enabled||!HasItem(i)||MenuPausa.Aberto||GameMessageUI.IsShowing||PayphoneCheckpoint.IsSaveUIOpen)return;
        if(i==0){GameState.AlternarLanterna();return;}
        if(i==1){MapaInterativoUI.EnsureInstance().Abrir(0);return;}
        GameMessageUI.Instance.ShowInspection(i==2?"RECIBO DO MERCADINHO":i==3?"FICHA DO CLUBE Nº 01":"FICHA DO CLUBE Nº 02",null,
            i==2?"Mercadinho Santa Rita, 19/10/2004, 18h10. Café, velas e fósforo. No verso, o aviso para Adílio sobre o homem de capa. Posso entregar o original no mercadinho.":i==3?"Ficha recebida no Bar do Zé. O registro do confronto e do relato está no diário.":"Ficha recebida de Tonão na Sociedade Beira-Rio. O relato está no diário.", arteCustom:icons[i]);
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void StartSession(){if(FindAnyObjectByType<SantaRitaInventory>()==null){var g=new GameObject("InventarioCincoSlots");DontDestroyOnLoad(g);g.AddComponent<SantaRitaInventory>();}}
    private void Awake()
    {
        canvas=UIFactory.CreateCanvas(transform,"Itens",42);
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        icons=LoadIcons();
        var frame=UIFactory.CreateBox(canvas.transform,"MolduraItens",null);frame.color=new Color(.49f,.40f,.32f,.98f);
        var fr=frame.rectTransform;fr.anchorMin=fr.anchorMax=new Vector2(.5f,0);fr.anchoredPosition=new Vector2(0,21);fr.sizeDelta=new Vector2(150,34);frame.raycastTarget=false;
        frame.color=new Color(.025f,.025f,.022f,.96f);
        var rim=UIFactory.CreateBox(frame.transform,"ContornoLatão",null);rim.color=new Color(.48f,.40f,.31f);UIFactory.Stretch(rim.rectTransform);rim.rectTransform.offsetMin=new Vector2(1,1);rim.rectTransform.offsetMax=new Vector2(-1,-1);rim.raycastTarget=false;
        var inner=UIFactory.CreateBox(rim.transform,"CouroEscuro",null);inner.color=new Color(.055f,.058f,.053f,.98f);UIFactory.Stretch(inner.rectTransform);inner.rectTransform.offsetMin=new Vector2(1,1);inner.rectTransform.offsetMax=new Vector2(-1,-1);inner.raycastTarget=false;
        var highlight=UIFactory.CreateBox(inner.transform,"RelevoSuperior",null);highlight.color=new Color(.26f,.23f,.18f);var hr=highlight.rectTransform;hr.anchorMin=new Vector2(0,1);hr.anchorMax=new Vector2(1,1);hr.pivot=new Vector2(.5f,1);hr.sizeDelta=new Vector2(-2,1);hr.anchoredPosition=Vector2.zero;highlight.raycastTarget=false;
        tooltip=UIFactory.CreateLabel(canvas.transform,"NomeDoItem",9);tooltip.alignment=TextAlignmentOptions.Center;
        tooltip.rectTransform.anchorMin=tooltip.rectTransform.anchorMax=new Vector2(.5f,0);tooltip.rectTransform.anchoredPosition=new Vector2(0,46);tooltip.rectTransform.sizeDelta=new Vector2(200,14);tooltip.raycastTarget=false;
        for(int i=0;i<5;i++)
        {
            var box=UIFactory.CreateBox(canvas.transform,"Slot"+i,null);box.color=new Color(.11f,.115f,.102f,.95f);
            var r=box.rectTransform;r.anchorMin=r.anchorMax=new Vector2(.5f,0);r.anchoredPosition=new Vector2((i-2)*28,21);r.sizeDelta=new Vector2(25,25);
            int slot=i;box.raycastTarget=true;var button=box.gameObject.AddComponent<Button>();button.targetGraphic=box;button.onClick.AddListener(()=>Use(slot));
            var icon=new GameObject("Icone",typeof(RectTransform),typeof(Image));icon.transform.SetParent(box.transform,false);
            pictures[i]=icon.GetComponent<Image>();pictures[i].sprite=icons[i];pictures[i].preserveAspect=true;pictures[i].raycastTarget=false;UIFactory.Stretch(pictures[i].rectTransform);pictures[i].rectTransform.offsetMin=new Vector2(3,3);pictures[i].rectTransform.offsetMax=new Vector2(-3,-3);
            labels[i]=UIFactory.CreateLabel(box.transform,"Vazio",7);labels[i].alignment=TextAlignmentOptions.Center;UIFactory.Stretch(labels[i].rectTransform);
            var hover=box.gameObject.AddComponent<SantaRitaItemHover>();hover.enter=()=>hovered=slot;hover.exit=()=>hovered=-1;
        }
    }
    private void LateUpdate()
    {
        if(canvas==null||labels[0]==null)return;
        if(icons!=null&&icons[0]==null&&Time.unscaledTime>=nextIconRetry){nextIconRetry=Time.unscaledTime+2;icons=LoadIcons();for(int i=0;i<5;i++)pictures[i].sprite=icons[i];}
        string scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        canvas.enabled=!MenuPausa.Aberto&&!PayphoneCheckpoint.IsSaveUIOpen&&scene!="MainMenu"&&scene!="Prologo"&&!SinucaMinigameUI.IsShowing&&!BochaMinigameUI.IsShowing&&!MapaInterativoUI.IsShowing&&!GameMessageUI.IsShowing;
        for(int i=0;i<5;i++){bool owns=HasItem(i);labels[i].text="";labels[i].color=new Color(.36f,.34f,.27f);pictures[i].enabled=owns&&pictures[i].sprite!=null;}
        tooltip.text=hovered>=0&&HasItem(hovered)?names[hovered]+(hovered==0?"  [F]":hovered==1?"  [M]":""):"";
    }
    private static Sprite[] LoadIcons()
    {
        var source=Resources.Load<Texture2D>("Art/BarRef7/ItensAtlas");var result=new Sprite[5];if(source==null)return result;
        var pixels=source.GetPixels32();var columns=new bool[source.width];for(int y=0;y<source.height;y++)for(int x=0;x<source.width;x++)if(pixels[y*source.width+x].a>32)columns[x]=true;
        var ranges=new List<Vector2Int>();int start=-1,gap=0;
        for(int x=0;x<columns.Length;x++){if(columns[x]){if(start<0)start=x;gap=0;}else if(start>=0&&++gap>4){ranges.Add(new Vector2Int(start,x-gap+1));start=-1;}}
        if(start>=0)ranges.Add(new Vector2Int(start,source.width));
        for(int i=0;i<5;i++)
        {
            int left=ranges.Count==5?ranges[i].x:i*source.width/5,right=ranges.Count==5?ranges[i].y:(i+1)*source.width/5,minY=source.height,maxY=0;
            for(int y=0;y<source.height;y++)for(int x=left;x<right;x++)if(pixels[y*source.width+x].a>32){minY=Mathf.Min(minY,y);maxY=Mathf.Max(maxY,y);}
            if(maxY<minY)continue;result[i]=Sprite.Create(source,new Rect(left,minY,right-left,maxY-minY+1),new Vector2(.5f,.5f),32);result[i].name="Item"+i;
        }
        return result;
    }

}

public sealed class SantaRitaItemHover:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public System.Action enter,exit;
    public void OnPointerEnter(PointerEventData e)=>enter?.Invoke();
    public void OnPointerExit(PointerEventData e)=>exit?.Invoke();
}

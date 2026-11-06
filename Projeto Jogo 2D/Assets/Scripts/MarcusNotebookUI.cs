using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Caderno pessoal independente da carta da cidade; nunca grava um checkpoint.
public sealed class MarcusNotebookUI : MonoBehaviour
{
    public bool IsOpen=>book!=null&&book.activeSelf;
    public int PageCount=>pages.Count;
    public int Spread=>spread;
    public int UpdateCount {get;private set;}
    public bool HasPendingNotice=>pending;
    private GameObject book,toast;
    private TextMeshProUGUI left,right,leftNumber,rightNumber,emptyRight;
    private Button previous,next;
    private RectTransform leaf;
    private CanvasGroup paperGroup,noticeGroup;
    private readonly List<string> pages=new List<string>();
    private readonly Dictionary<string,string> known=new Dictionary<string,string>();
    private int spread;
    private readonly List<string> pageIds=new List<string>();
    private GameObject indexPanel,ribbon;private TextMeshProUGUI markLabel;
    private string filter="Todas";private int indexPage;
    public string MarkedEntry=>GameState.Texto("caderno.marcador.id");
    public void ToggleBookmark()
    {
        if(pages.Count==0)return;string id=pageIds[Mathf.Min(spread*2,pageIds.Count-1)];
        bool same=MarkedEntry==id;GameState.DefinirTexto("caderno.marcador.id",same?"":id);
        GameState.DefinirNumero("caderno.marcador.parte",same?0:spread*2-pageIds.IndexOf(id));Refresh();
    }
    private void RestoreBookmark()
    {
        int i=pageIds.IndexOf(MarkedEntry);if(i>=0)spread=(i+Mathf.Max(0,GameState.Numero("caderno.marcador.parte")))/2;
        spread=Mathf.Clamp(spread,0,Mathf.Max(0,(pages.Count-1)/2));
    }
    private bool Included(MarcusNotebookEntry e)
    {
        string id=e.id.ToLowerInvariant();
        if(filter=="Marcadas")return id==MarkedEntry;
        if(filter=="Fitas")return id.Contains("fita")||id.Contains("mila");
        if(filter=="Centro")return id.StartsWith("c2_");
        if(filter=="Bairro")return !id.StartsWith("c2_");
        if(filter=="Pessoas")return System.Text.RegularExpressions.Regex.IsMatch(id,"mila|ze_|tona|lurdes|ramiro|frentista|atendente|padre|alcina|celina|nair|morador|frequentador");
        return true;
    }
    public void OpenIndex(string category="Todas")
    {
        filter=category;indexPage=0;DrawIndex();
    }
    private void DrawIndex()
    {
        if(indexPanel!=null)Destroy(indexPanel);
        left.gameObject.SetActive(false);right.gameObject.SetActive(false);
        indexPanel=Panel("IndiceManuscrito",book.transform,Vector2.zero,new Vector2(578,300),new Color(.82f,.77f,.62f,0));
        string[] groups={"Todas","Bairro","Centro","Pessoas","Fitas","Marcadas"};
        for(int g=0;g<groups.Length;g++){string c=groups[g];var b=Button("Filtro_"+c,c,new Vector2(-237+g*95,129),()=>OpenIndex(c));b.transform.SetParent(indexPanel.transform,false);b.GetComponent<RectTransform>().sizeDelta=new Vector2(89,22);b.GetComponent<Image>().color=c==filter?new Color(.33f,.24f,.17f,.2f):Color.clear;var ink=b.GetComponentInChildren<TextMeshProUGUI>();ink.font=left.font;ink.fontSize=16;ink.color=new Color(.18f,.2f,.23f);}
        var entries=MarcusNotebookContent.Read().FindAll(Included);int start=indexPage*12;
        for(int i=0;i<12&&start+i<entries.Count;i++)
        {
            var e=entries[start+i];var b=Button("Nota_"+e.id,e.title,new Vector2(i%2==0?-141:141,94-(i/2)*31),()=>{int at=pageIds.IndexOf(e.id);if(at>=0){spread=at/2;Refresh();Destroy(indexPanel);left.gameObject.SetActive(true);right.gameObject.SetActive(true);}});
            b.transform.SetParent(indexPanel.transform,false);b.GetComponent<RectTransform>().sizeDelta=new Vector2(268,28);var t=b.GetComponentInChildren<TextMeshProUGUI>();b.GetComponent<Image>().color=new Color(.25f,.2f,.15f,.08f);t.font=left.font;t.fontSize=15;t.color=new Color(.18f,.2f,.23f);t.overflowMode=TextOverflowModes.Ellipsis;
        }
        if(entries.Count==0){var t=Hand("IndiceVazio",Vector2.zero,new Vector2(460,45),left.font);t.transform.SetParent(indexPanel.transform,false);t.text="Ainda não deixei anotações nesta parte.";}
        var prev=Button("IndiceAnterior","←",new Vector2(-185,-123),()=>{if(indexPage>0){indexPage--;DrawIndex();}});prev.transform.SetParent(indexPanel.transform,false);
        var done=Button("VoltarLeitura","Voltar à leitura",new Vector2(0,-123),()=>{Destroy(indexPanel);left.gameObject.SetActive(true);right.gameObject.SetActive(true);});done.transform.SetParent(indexPanel.transform,false);
        var nextIndex=Button("IndiceSeguinte","→",new Vector2(185,-123),()=>{if((indexPage+1)*12<entries.Count){indexPage++;DrawIndex();}});nextIndex.transform.SetParent(indexPanel.transform,false);
    }
    private bool pending,turning;
    private float noticeTime;
    private Action close;
    public void Initialize(Transform parent,Action closeAction)
    {
        close=closeAction;
        book=new GameObject("CadernoDeMarcus",typeof(RectTransform),typeof(RawImage));book.transform.SetParent(parent,false);Place((RectTransform)book.transform,Vector2.zero,new Vector2(624,348));
        var surface=Resources.Load<Texture2D>("Notebook/Book");
        var image=book.GetComponent<RawImage>();image.texture=surface;image.raycastTarget=true;
        var font=Resources.Load<TMP_FontAsset>("Fonts/Kalam/MarcusHand");
        if(font==null)throw new InvalidOperationException("Fonte manuscrita do caderno não foi importada.");
        left=Hand("PaginaEsquerda",new Vector2(-143,0),new Vector2(230,255),font);
        right=Hand("PaginaDireita",new Vector2(143,0),new Vector2(230,255),font);
        leftNumber=Hand("NumeroEsquerda",new Vector2(-143,-139),new Vector2(50,18),font);leftNumber.alignment=TextAlignmentOptions.Center;
        rightNumber=Hand("NumeroDireita",new Vector2(143,-139),new Vector2(50,18),font);rightNumber.alignment=TextAlignmentOptions.Center;
        emptyRight=Hand("PaginaEmBranco",new Vector2(143,-25),new Vector2(230,45),font);emptyRight.alignment=TextAlignmentOptions.Center;emptyRight.color=new Color(.3f,.26f,.22f,.45f);
        previous=Button("FolhaAnterior","← Anterior",new Vector2(-229,-164),()=>Turn(-1));
        next=Button("ProximaFolha","Próxima →",new Vector2(229,-164),()=>Turn(1));
        Button("UltimaAnotacao","Última anotação",new Vector2(0,-164),()=>GoToLast());
        Button("Indice","Índice",new Vector2(-235,163),()=>OpenIndex());
        var mark=Button("Marcador","Marcar página",new Vector2(-105,163),ToggleBookmark);markLabel=mark.GetComponentInChildren<TextMeshProUGUI>();
        ribbon=Panel("FitaDeTecido",book.transform,new Vector2(20,130),new Vector2(9,64),new Color(.31f,.15f,.12f));ribbon.GetComponent<Image>().raycastTarget=false;var seam=Panel("Costura",ribbon.transform,new Vector2(-2,0),new Vector2(1,57),new Color(.55f,.35f,.22f));seam.GetComponent<Image>().raycastTarget=false;
        Button("FecharCaderno","Fechar [J/Esc]",new Vector2(231,163),()=>close());
        var flip=new GameObject("FolhaVirando",typeof(RectTransform),typeof(RawImage));flip.transform.SetParent(book.transform,false);Place((RectTransform)flip.transform,Vector2.zero,new Vector2(278,288));
        leaf=(RectTransform)flip.transform;leaf.pivot=new Vector2(0,.5f);leaf.anchoredPosition=Vector2.zero;
        var texture=Resources.Load<Texture2D>("Notebook/Leaf");var raw=flip.GetComponent<RawImage>();raw.texture=texture;raw.raycastTarget=false;
        paperGroup=flip.AddComponent<CanvasGroup>();paperGroup.blocksRaycasts=false;flip.SetActive(false);
        toast=Panel("AvisoCaderno",parent,new Vector2(-213,-126),new Vector2(174,30),new Color(.09f,.08f,.065f,.97f));
        var icon=new GameObject("Caderninho",typeof(RectTransform),typeof(RawImage));icon.transform.SetParent(toast.transform,false);Place((RectTransform)icon.transform,new Vector2(-70,0),new Vector2(21,20));icon.GetComponent<RawImage>().texture=surface;icon.GetComponent<RawImage>().raycastTarget=false;
        var label=UIFactory.CreateLabel(toast.transform,"Aviso",14);Place(label.rectTransform,new Vector2(12,0),new Vector2(142,26));label.text="Caderno atualizado";label.alignment=TextAlignmentOptions.Center;label.color=new Color(.86f,.8f,.64f);
        noticeGroup=toast.AddComponent<CanvasGroup>();noticeGroup.blocksRaycasts=false;toast.SetActive(false);book.SetActive(false);
        GameState.OnDiarioAtualizado+=Changed;GameState.OnSnapshotRestaurado+=Restored;Restored();
    }
    private void OnDestroy(){GameState.OnDiarioAtualizado-=Changed;GameState.OnSnapshotRestaurado-=Restored;}
    private void Restored()
    {
        known.Clear();foreach(var entry in GameState.ObterEntradasDiario()){var p=MarcusNotebookContent.Project(entry);if(p!=null)known[entry.id]=p.body;}
        pending=false;noticeTime=0;if(toast!=null)toast.SetActive(false);
    }
    private void Changed(string id)
    {
        foreach(var entry in GameState.ObterEntradasDiario())
        {
            if(entry.id!=id)continue;var p=MarcusNotebookContent.Project(entry);if(p==null)return;
            if(known.TryGetValue(id,out string old)&&old==p.body)return;known[id]=p.body;
            UpdateCount++;pending=true;
            if(IsOpen){BuildPages();spread=Mathf.Clamp(spread,0,Mathf.Max(0,(pages.Count-1)/2));Refresh();}
            return;
        }
    }
    private void Update()
    {
        if(book==null)return;
        if(IsOpen&&indexPanel==null&&UnityEngine.InputSystem.Mouse.current!=null)
        {float wheel=UnityEngine.InputSystem.Mouse.current.scroll.ReadValue().y;if(Mathf.Abs(wheel)>.1f)Turn(wheel>0?-1:1);}
        if(pending&&!SantaRitaInventory.IsOpen&&!SantaRitaTapeDeck.IsOpen&&SantaRitaAtlas.Current()!=null&&!IsOpen&&!GameMessageUI.IsShowing&&!MenuPausa.Aberto&&!MapaInterativoUI.IsShowing&&!SinucaMinigameUI.IsShowing&&!BochaMinigameUI.IsShowing)
        {pending=false;noticeTime=4;toast.SetActive(true);}
        if(noticeTime>0)
        {if(SantaRitaInventory.IsOpen||SantaRitaTapeDeck.IsOpen){noticeGroup.alpha=0;return;}noticeTime-=Time.unscaledDeltaTime;noticeGroup.alpha=Mathf.Min(1,noticeTime*2);if(noticeTime<=0)toast.SetActive(false);}
    }
    public void Show()
    {
        book.SetActive(true);left.gameObject.SetActive(true);right.gameObject.SetActive(true);toast.SetActive(false);noticeTime=0;pending=false;
        Canvas.ForceUpdateCanvases();BuildPages();spread=Mathf.Clamp(GameState.Numero("caderno.folha",spread),0,Mathf.Max(0,(pages.Count-1)/2));RestoreBookmark();Refresh();
    }
    public void Hide(){if(indexPanel!=null)Destroy(indexPanel);book.SetActive(false);StopAllCoroutines();turning=false;leaf.gameObject.SetActive(false);}
    public void Turn(int direction)
    {
        int target=Mathf.Clamp(spread+direction,0,(pages.Count-1)/2);if(indexPanel!=null||turning||target==spread)return;
        StartCoroutine(Flip(target));
    }
    private void GoToLast(){if(!turning&&pages.Count>0)StartCoroutine(Flip((pages.Count-1)/2));}
    private IEnumerator Flip(int target)
    {
        turning=true;leaf.gameObject.SetActive(true);bool changed=false;
        for(float t=0;t<1;t+=Time.unscaledDeltaTime/.3f)
        {
            leaf.localScale=new Vector3(Mathf.Cos(t*Mathf.PI),1,1);paperGroup.alpha=.94f;
            if(t>=.5f&&!changed){spread=target;Refresh();changed=true;}
            yield return null;
        }
        spread=target;Refresh();leaf.gameObject.SetActive(false);turning=false;
        if(GameState.Numero("caderno.folha",-1)!=spread)GameState.DefinirNumero("caderno.folha",spread);
    }
    private void BuildPages()
    {
        pages.Clear();pageIds.Clear();
        foreach(var entry in MarcusNotebookContent.Read())
        {
            string heading="<size=23>"+entry.title+"</size>\n\n";
            var body=new StringBuilder();
            foreach(Match word in Regex.Matches(entry.body,@"\S+\s*"))
            {
                string candidate=heading+body+word.Value;
                if(body.Length>0&&left.GetPreferredValues(candidate,left.rectTransform.rect.width,10000).y>left.rectTransform.rect.height-4)
                {pages.Add(heading+body.ToString().TrimEnd());pageIds.Add(entry.id);heading="";body.Clear();}
                body.Append(word.Value);
            }
            if(body.Length>0){pages.Add(heading+body.ToString().TrimEnd());pageIds.Add(entry.id);}
        }
    }
    private void Refresh()
    {
        int index=spread*2;
        bool marked=pageIds.Count>index&&pageIds[index]==MarkedEntry;
        if(ribbon!=null)ribbon.SetActive(marked);if(markLabel!=null)markLabel.text=marked?"Retirar marcador":"Marcar página";
        left.text=index<pages.Count?pages[index]:"";right.text=index+1<pages.Count?pages[index+1]:"";
        leftNumber.text=(index+1).ToString();rightNumber.text=(index+2).ToString();
        emptyRight.text=index+1<pages.Count?"":"";previous.interactable=spread>0;next.interactable=index+2<pages.Count;
    }
    private TextMeshProUGUI Hand(string name,Vector2 position,Vector2 size,TMP_FontAsset font)
    {
        var label=UIFactory.CreateLabel(book.transform,name,17.5f);label.font=font;label.color=new Color(.16f,.19f,.23f);label.alignment=TextAlignmentOptions.TopLeft;
        label.textWrappingMode=TextWrappingModes.Normal;label.overflowMode=TextOverflowModes.Overflow;label.lineSpacing=2;label.paragraphSpacing=3;
        Place(label.rectTransform,position,size);return label;
    }
    private Button Button(string name,string text,Vector2 position,UnityEngine.Events.UnityAction action)
    {
        var go=Panel(name,book.transform,position,new Vector2(108,22),new Color(.14f,.12f,.095f,.96f));
        var button=go.AddComponent<Button>();button.targetGraphic=go.GetComponent<Image>();button.onClick.AddListener(action);
        var label=UIFactory.CreateLabel(go.transform,"Rotulo",12);UIFactory.Stretch(label.rectTransform);label.text=text;label.alignment=TextAlignmentOptions.Center;return button;
    }
    private static GameObject Panel(string name,Transform parent,Vector2 position,Vector2 size,Color color)
    {var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);Place((RectTransform)go.transform,position,size);go.GetComponent<Image>().color=color;return go;}
    private static void Place(RectTransform rect,Vector2 p,Vector2 size){rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(.5f,.5f);rect.anchoredPosition=p;rect.sizeDelta=size;}
}

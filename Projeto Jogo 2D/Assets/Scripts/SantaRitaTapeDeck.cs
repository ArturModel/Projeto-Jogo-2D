using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

[Serializable] public sealed class TapeLine {public string speaker,text;public float seconds;}
[Serializable] public sealed class TapeRecord {public string id,title,label,origin;public TapeLine[] lines;}
[Serializable] public sealed class TapeCatalog {public TapeRecord[] tapes;}
public sealed class SantaRitaTapeDeck:MonoBehaviour
{
    public static SantaRitaTapeDeck Instance {get;private set;}
    public static int ClosedFrame {get;private set;} = -1;
    public static bool IsOpen=>Instance!=null&&Instance.root!=null&&Instance.root.activeSelf;
    public static SantaRitaTapeDeck Ensure(){if(Instance==null){var go=new GameObject("TocaFitas");DontDestroyOnLoad(go);Instance=go.AddComponent<SantaRitaTapeDeck>();}return Instance;}
    private GameObject root,speedButton;private Button[] tapeButtons=new Button[4];private TextMeshProUGUI heading,caption,timeLabel,status;private Image reels;
    private Button playButton;
    private AudioSource voice;private TapeCatalog catalog;private TapeRecord tape;private float elapsed;private bool playing,repair;private int speed;
    public bool Playing=>playing;public float Position=>elapsed;
    public TapeRecord[] Records=>catalog.tapes;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Boot()=>Ensure();
    private void Awake()
    {
        Instance=this;catalog=JsonUtility.FromJson<TapeCatalog>(Resources.Load<TextAsset>("Tapes/Catalog").text);voice=gameObject.AddComponent<AudioSource>();voice.loop=false;
        var c=UIFactory.CreateCanvas(transform,"Gravador",8700);c.gameObject.AddComponent<GraphicRaycaster>();root=SantaRitaInventory.Frame(c.transform,"Estojo",Vector2.zero,new Vector2(580,328));
        heading=SantaRitaInventory.Label(root.transform,"Titulo",new Vector2(0,140),new Vector2(550,28),19);
        var pic=new GameObject("Aparelho",typeof(RectTransform),typeof(Image));pic.transform.SetParent(root.transform,false);SantaRitaInventory.Place((RectTransform)pic.transform,new Vector2(-175,25),new Vector2(160,150));reels=pic.GetComponent<Image>();var tex=Resources.Load<Texture2D>("Tapes/Recorder");if(tex!=null)reels.sprite=Sprite.Create(tex,new Rect(0,0,tex.width,tex.height),new Vector2(.5f,.5f));reels.preserveAspect=true;
        caption=SantaRitaInventory.Label(root.transform,"Legenda",new Vector2(81,20),new Vector2(327,118),17);
        status=SantaRitaInventory.Label(root.transform,"Estado",new Vector2(0,-58),new Vector2(530,35),12);
        timeLabel=SantaRitaInventory.Label(root.transform,"Contador",new Vector2(-175,-43),new Vector2(130,20),17);
        for(int i=0;i<4;i++){int n=i;var b=SantaRitaInventory.ActionButton(root.transform,catalog.tapes[i].title,new Vector2(-210+i*140,100),()=>Select(catalog.tapes[n].id));((RectTransform)b.transform).sizeDelta=new Vector2(134,24);b.GetComponentInChildren<TMP_Text>().rectTransform.sizeDelta=new Vector2(130,24);tapeButtons[i]=b;}
        var play=SantaRitaInventory.ActionButton(root.transform,"PLAY / PAUSA",new Vector2(-180,-105),()=>PlayPause());((RectTransform)play.transform).sizeDelta=new Vector2(145,25);playButton=play;
        var rewind=SantaRitaInventory.ActionButton(root.transform,"REBOBINAR",new Vector2(-20,-105),()=>Rewind());((RectTransform)rewind.transform).sizeDelta=new Vector2(145,25);
        var stop=SantaRitaInventory.ActionButton(root.transform,"STOP",new Vector2(140,-105),()=>Stop());((RectTransform)stop.transform).sizeDelta=new Vector2(145,25);
        SantaRitaInventory.ActionButton(root.transform,"Fechar [Esc]",new Vector2(150,-143),Close);
        speedButton=SantaRitaInventory.ActionButton(root.transform,"Velocidade de teste",new Vector2(-130,-143),()=>{if(repair){speed=(speed+1)%3;Capitulo2Audio.TocarRadio(speed);status.text=new[]{"LENTO: mecanismo arrastado","REGULAR: dois retornos iguais. Pressione PLAY para confirmar.","RÁPIDO: mecanismo acelerado"}[speed];}}).gameObject;
        root.SetActive(false);SceneManager.sceneLoaded+=SceneChanged;
    }
    private void OnDestroy(){SceneManager.sceneLoaded-=SceneChanged;}
    private void SceneChanged(Scene s,LoadSceneMode mode){Close();}
    public void Open(string id="tocador")
    {
        MapaInterativoUI.FecharSeAberto();root.SetActive(true);repair=false;speedButton.SetActive(false);playButton.interactable=GameState.Tem("item.tocador");
        for(int i=0;i<tapeButtons.Length;i++){bool own=SantaRitaInventory.Owns(catalog.tapes[i].id);tapeButtons[i].gameObject.SetActive(own);tapeButtons[i].interactable=own;}
        if(!GameState.Tem("item.tocador")){heading.text="Uma fita sem aparelho";caption.text="A etiqueta e a procedência podem ser examinadas. Preciso de um tocador para reproduzir.";status.text="Oficina de aparelhos — perguntar antes de mexer.";if(id.StartsWith("fita"))Select(id);status.text="Preciso de um toca-fitas. Por enquanto posso examinar a etiqueta e guardar o estojo.";return;}
        string first=id.StartsWith("fita")?id:catalog.tapes.FirstOrDefault(t=>SantaRitaInventory.Owns(t.id))?.id;Select(first);
    }
    public void Repair()
    {
        root.SetActive(true);repair=true;playButton.interactable=true;speedButton.SetActive(true);foreach(var b in tapeButtons)b.gameObject.SetActive(false);Stop();speed=0;heading.text="BANCADA DE MILA — TESTE DE VELOCIDADE";caption.text="Mila aponta a etiqueta do aparelho: REGULAR.\n\nCompare os três ajustes. Dois retornos iguais indicam a posição correta; PLAY confirma o teste.";status.text="Sem cronômetro e sem perda de objetos. O teste pode ser repetido.";
    }
    public void Select(string id)
    {
        if(repair)return;Stop();tape=catalog.tapes.FirstOrDefault(t=>t.id==id&&SantaRitaInventory.Owns(t.id));
        if(tape==null){heading.text="TOCA-FITAS PORTÁTIL";caption.text="O estojo ainda não tem esta fita. Só as gravações encontradas podem ser consultadas.";return;}
        heading.text=tape.label;caption.text=tape.origin;voice.clip=Resources.Load<AudioClip>("Tapes/Voice/"+tape.id);status.text=voice.clip==null?"Transcrição da gravação · PLAY para acompanhar as falas.":"PLAY para escutar. A gravação conserva o som original; a interpretação fica no caderno.";
    }
    public void PlayPause()
    {
        if(repair){if(speed!=1){status.text="O mecanismo ainda não está regular. Compare outro ajuste.";return;}GameState.Marcar("c2_radio_restaurado");GameState.Marcar("item.tocador");GameState.Marcar("item.fita02");GameState.RegistrarNotaDiario("mila_emprestimo","O aparelho de Mila","fato","Mila emprestou um tocador depois do teste de velocidade. A segunda fita veio do estojo de testes da oficina.");Close();GameMessageUI.Instance.Show("Ficou regular. Leva o tocador por enquanto; eu quase não uso mais. Essa fita estava com os testes da Júlia. Quando terminar, passa aqui de novo.","MILA","mila");return;}
        if(tape==null||!GameState.Tem("item.tocador"))return;
        if(elapsed>=Duration())Rewind();playing=!playing;if(voice.clip!=null){if(playing){voice.time=Mathf.Min(elapsed,voice.clip.length-.01f);voice.Play();}else voice.Pause();}
    }
    public void Stop(){playing=false;elapsed=0;if(voice!=null)voice.Stop();}
    public void Rewind(){Stop();if(tape!=null)caption.text=tape.origin;}
    public void Close(){if(IsOpen)ClosedFrame=Time.frameCount;Stop();repair=false;if(root!=null)root.SetActive(false);}
    private float Duration()=>tape==null?0:tape.lines.Sum(l=>l.seconds);
    private void Update()
    {
        if(!IsOpen)return;if(Keyboard.current!=null&&Keyboard.current.escapeKey.wasPressedThisFrame){Close();return;}
        voice.volume=AudioSettingsController.VolumeFala;
        if(MenuPausa.Aberto||!Application.isFocused){if(playing){playing=false;voice.Pause();}return;}
        if(!playing||tape==null)return;
        elapsed+=Time.unscaledDeltaTime;timeLabel.text=TimeSpan.FromSeconds(elapsed).ToString(@"mm\:ss");float t=0;
        foreach(var line in tape.lines){t+=line.seconds;if(elapsed<t){caption.text="<color=#D6BC78>"+line.speaker+"</color>\n"+line.text;break;}}
        if(elapsed>=Duration())
        {
            playing=false;voice.Stop();GameState.Marcar("fita.consultada."+tape.id);
            GameState.RegistrarNotaDiario("fita_"+tape.id,tape.title,"pista",voice.clip==null?"Consultei a transcrição desta gravação: "+tape.origin:"Escutei a gravação: "+tape.origin);
            caption.text="Fim da gravação.";status.text="Registro no caderno. Rebobinar permite consultar novamente.";
        }
    }
}
// Physical pickups are attached to already-authored shelves/records; no city reconstruction.
public sealed class SantaRitaTapePickup:Interactable
{
    public string tapeId;
    public override string Prompt=>"Examinar estojo de fita";
    public override void Interact()
    {
        var tape=SantaRitaTapeDeck.Ensure().Records.First(t=>t.id==tapeId);
        GameMessageUI.Instance.ShowInspection(tape.label,null,tape.origin+"\n\nA fita tem estojo e etiqueta próprios. Não há como saber o conteúdo só olhando.",arteCustom:SantaRitaInventory.Instance.Icon(tapeId));
        if(!SantaRitaInventory.Owns(tapeId))SantaRitaItemAction.Oferecer("GUARDAR FITA",()=>GameState.Marcar("item."+tapeId));
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){SceneManager.sceneLoaded-=Place;SceneManager.sceneLoaded+=Place;Place(SceneManager.GetActiveScene(),LoadSceneMode.Single);}
    private static void Place(Scene s,LoadSceneMode mode){var host=new GameObject("DistribuicaoDasFitas");host.AddComponent<SantaRitaTapePlacement>();}
}
public sealed class SantaRitaTapePlacement:MonoBehaviour
{
    private float next;
    private void Update()
    {
        if(Time.unscaledTime<next)return;next=Time.unscaledTime+2;
        foreach(var a in FindObjectsByType<InteracaoCapitulo1>())if(a.Tipo==InteracaoCapitulo1.TipoInteracao.RetratoFamiliaCasaAvo)Attach(a.transform,"fita01",new Vector2(.7f,-.2f));
        foreach(var a in FindObjectsByType<Capitulo2Interactable>())
        {if(a.interactionId=="foto_igreja")Attach(a.transform,"fita03",new Vector2(.8f,-.2f));if(a.interactionId=="cordas")Attach(a.transform,"fita_sociedade",new Vector2(.8f,-.2f));}
        enabled=false;
    }
    private void Attach(Transform anchor,string id,Vector2 offset)
    {
        if(anchor.Find("Estojo_"+id)!=null)return;var go=new GameObject("Estojo_"+id);go.transform.SetParent(anchor,false);go.transform.localPosition=offset;go.transform.localScale=Vector3.one*.25f;
        var sr=go.AddComponent<SpriteRenderer>();sr.sprite=SantaRitaInventory.Instance.Icon(id);sr.sortingOrder=anchor.GetComponent<SpriteRenderer>()!=null?anchor.GetComponent<SpriteRenderer>().sortingOrder+1:10;var p=go.AddComponent<SantaRitaTapePickup>();p.tapeId=id;
    }
}

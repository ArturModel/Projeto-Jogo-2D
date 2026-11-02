using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// The exploration room and the billiards environment consume this same physical plan.
public static class SantaRitaBarLayout
{
    public const float UnitsPerMetre=1f/1.15f;
    public const float PlayingLengthMetres=2.84f, PlayingWidthMetres=1.42f;
    public static readonly Vector2 TableCenter=new Vector2(9,6.5f);
    public static readonly Vector2 Origin=new Vector2(500,-60);
    public static readonly Rect WalkableRoom=new Rect(4.25f,0,9.5f,9.85f);
    public static void ConstrainArrival()
    {
        if(GameObject.Find("BarCoerente")==null)return;var player=UnityEngine.Object.FindAnyObjectByType<PlayerController>();if(player==null)return;
        Vector2 p=(Vector2)player.transform.position-Origin;
        if(p.x<WalkableRoom.xMin+.45f||p.x>WalkableRoom.xMax-.45f||p.y>WalkableRoom.yMax-.5f)
            player.TeleportarPara(Origin+new Vector2(Mathf.Clamp(p.x,WalkableRoom.xMin+.6f,WalkableRoom.xMax-.6f),Mathf.Clamp(p.y,.8f,WalkableRoom.yMax-1.1f)),1);
    }
    public enum Kind { Counter, Shelf, Table, Chair, Wall, Actor, Decor }
    public sealed class Prop
    {
        public string id,key; public Vector2 position;public float width;public float elevation,supportOffset;public Kind kind;
        public Prop(string id,string key,float x,float y,float width,Kind kind,float elevation=0,float supportOffset=0){this.id=id;this.key=key;position=new Vector2(x,y);this.width=width;this.kind=kind;this.elevation=elevation;this.supportOffset=supportOffset;}
    }
    private static readonly System.Collections.Generic.Dictionary<string,Transform> live=new System.Collections.Generic.Dictionary<string,Transform>();
    public static Vector2 LivePosition(Prop p)=>live.TryGetValue(p.id,out var t)&&t!=null?(Vector2)t.position-Origin:p.position;
    public static readonly Prop[] Props={
        new Prop("Balcao_Bar_Sprite","BalcaoBar",7.26087f,9.021739f,3f,Kind.Counter),
        new Prop("GeladeiraDoBar","Geladeira",13.1739f,8.93478f,1.0f,Kind.Shelf),
        new Prop("TelevisaoFutebol","Televisor",13.05f,11f,1.2f,Kind.Wall),
        new Prop("Calendario2004","CalendarioOutubro2004",10.9f,11f,0.55f,Kind.Wall),
        new Prop("CartazGaleto","CartazGaletoBar",8.52174f,11f,0.63f,Kind.Wall),
        new Prop("FiadoSoAmanha","FiadoBar",5.086957f,11f,.85f,Kind.Wall),
        new Prop("TimeDaComunidade","FotoTimeBar",6.365217f,11f,.80f,Kind.Wall),
        new Prop("Inter1994","Inter1994Bar",11.9f,11f,.9f,Kind.Wall),
        new Prop("RadioBalcao","RadioAntigo",8.217391f,10.05f,.65f,Kind.Decor,1.0f,1.115217f),
        new Prop("BanquetaBalcao0","BancoBar",6.391304f,8.326087f,.55f,Kind.Chair),
        new Prop("BanquetaBalcao1","BancoBar",7.130435f,8.326087f,.55f,Kind.Chair),
        new Prop("BanquetaBalcao2","BancoBar",7.869565f,8.326087f,.55f,Kind.Chair),
        new Prop("TaqueiraDoBar","TaqueiraBar",10.043478f,10.15f,1f,Kind.Wall),
        new Prop("EngradadosDoBar","EngradadoBar",11.608696f,9.308696f,.55f,Kind.Shelf),
        new Prop("MesaChimarrao","MesaPlasticaBar",6f,7.2f,1.35f,Kind.Table),
        new Prop("CadeiraChimaA","CadeiraPlasticaBar",5.05f,7.1f,.65f,Kind.Chair),
        new Prop("CadeiraChimaB","CadeiraPlasticaBar",6.95f,7.1f,.65f,Kind.Chair),

        new Prop("MesaConversa","MesaPlasticaBar",6f,3.7f,1.35f,Kind.Table),
        new Prop("CadeiraConversaA","CadeiraPlasticaBar",5.05f,3.6f,.65f,Kind.Chair),
        new Prop("CadeiraConversaB","CadeiraPlasticaBar",6.95f,3.6f,.65f,Kind.Chair),

        new Prop("MesaJanela","MesaPlasticaBar",12.1f,3.8f,1.35f,Kind.Table),
        new Prop("CadeiraJanelaA","CadeiraPlasticaBar",11.6f,3.7f,.65f,Kind.Chair),
        new Prop("CadeiraJanelaB","CadeiraPlasticaBar",12.6f,3.7f,.65f,Kind.Chair),

        new Prop("CopoConversa","CopoCervejaBar",6.15f,4.35f,.24f,Kind.Decor,.78f,.65f),
        new Prop("CopoJanela","CopoCervejaBar",12.25f,4.45f,.24f,Kind.Decor,.78f,.65f),
        new Prop("TermicaMesa","TermicaBar",5.85f,7.95f,.38f,Kind.Decor,.78f,.65f),
        new Prop("CuiaMesa","CuiaBar",5.55f,7.98f,.34f,Kind.Decor,.78f,.78f),
        new Prop("NPC_Ze_Interior","Npc_Ze",9.6f,7.95f,1,Kind.Actor),
        new Prop("NPC_Frequentador_Interior","Npc_Frequentador",5.05f,3.76f,1,Kind.Actor),
        new Prop("NPC_Conversa_Bar","Npc_Chico",6.95f,3.76f,1,Kind.Actor),
        new Prop("NPC_Chimarrao_Bar","Npc_Ramiro",5.05f,7.26f,1,Kind.Actor),
        new Prop("NPC_Cerveja_Bar","Npc_Motorista",12.6f,3.86f,1,Kind.Actor)
    };
    private static readonly System.Collections.Generic.Dictionary<string,Sprite[]> seatedFrames=new System.Collections.Generic.Dictionary<string,Sprite[]>();
    public static bool OccupiedChair(Prop p)=>p.id=="CadeiraChimaA"||p.id=="CadeiraConversaA"||p.id=="CadeiraConversaB"||p.id=="CadeiraJanelaB";
    public static Sprite PatronPose(string key,string id,float time)
    {
        if(!seatedFrames.TryGetValue(key,out var frames))
        {
            var sheet=Resources.Load<Texture2D>("Art/Characters/"+key+"_Rotina");if(sheet==null)return null;
            frames=new Sprite[8];for(int i=0;i<8;i++){frames[i]=Sprite.Create(sheet,new Rect(i*64,0,64,64),new Vector2(.5f,.125f),32,0,SpriteMeshType.FullRect);frames[i].name=key+"_Rotina_"+i;}seatedFrames[key]=frames;
        }
        return frames[PatronFrame(id,time)];
    }
    public static int PatronFrame(string id,float time)
    {
        float offset=0;foreach(char c in id)offset=(offset*31+c)%233;float t=(time+offset*.13f)%24;
        if(id=="NPC_Chimarrao_Bar")return t<7?0:t<7.6f?6:t<9?7:t<9.4f?6:t<10?0:t<10.4f?1:t<10.8f?2:t<12.2f?3:t<12.6f?4:t<13?5:0;
        return t<7?0:t<7.4f?1:t<7.8f?2:t<9.2f?3:t<9.6f?4:t<10?5:t<17?0:t<18.2f?6:t<19.4f?7:0;
    }
    public static bool TablePropVisible(string id,float time)
    {
        int f=PatronFrame("NPC_Chimarrao_Bar",time);
        return id=="CuiaMesa"?f==0:id=="TermicaMesa"?f!=6&&f!=7:true;
    }
    // The original 2D sprite is authoritative in exploration AND in the perspective room.
    public static Sprite SpriteFor(Prop p)
    {
        if(p.kind==Kind.Actor&&!p.key.Contains("Ze"))return PatronPose(p.key,p.id,0)??Resources.Load<Sprite>("Art/Characters/"+p.key+"_Sentado")??Resources.Load<Sprite>("Art/Characters/"+p.key);
        return Resources.Load<Sprite>((p.kind==Kind.Actor?"Art/Characters/":"Art/Props/")+p.key);
    }
    private static Sprite nativeFloor,nativeWall;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetArt(){nativeFloor=null;nativeWall=null;live.Clear();seatedFrames.Clear();}
    public static Sprite NativeSurface(bool wall)
    {
        if(wall&&nativeWall!=null)return nativeWall;if(!wall&&nativeFloor!=null)return nativeFloor;
        var room=GameObject.Find("Interior4_BarDoZe");var source=room==null?null:room.transform.Find("Piso_Interior_BarDoZe");
        var sheet=source==null?null:source.GetComponent<SpriteRenderer>().sprite.texture;
        int height=wall?64:32;var t=new Texture2D(32,height,TextureFormat.RGBA32,false){name=wall?"ParedeNativaBar":"PisoNativoBar",filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Repeat};
        if(sheet!=null)t.SetPixels(sheet.GetPixels(8*32,wall?12*32:6*32,32,height));
        else{var fallback=Resources.Load<Texture2D>("Art/Tiles/Madeira");for(int y=0;y<height;y++)for(int x=0;x<32;x++)t.SetPixel(x,y,fallback.GetPixel(x%fallback.width,y%fallback.height));}
        t.Apply();var sprite=Sprite.Create(t,new Rect(0,0,32,height),new Vector2(.5f,.5f),32,0,SpriteMeshType.FullRect);
        if(wall)nativeWall=sprite;else nativeFloor=sprite;return sprite;
    }
    public static Sprite LiveSprite(Prop p)=>live.TryGetValue(p.id,out var t)&&t!=null?t.GetComponent<SpriteRenderer>().sprite:SpriteFor(p);
    public static void Apply()
    {
        var room=GameObject.Find("Interior4_BarDoZe");if(room==null)return;
        var old=room.transform.Find("BarCoerente");if(old!=null)return;
        // Retire only furniture/actors from the old room. Floors, walls and transitions stay in place.
        foreach(Transform child in room.transform)
            if(child.GetComponent<SpriteRenderer>()!=null||child.GetComponent<InteracaoCapitulo1>()!=null)
                if(!child.name.StartsWith("Soleira")&&!child.name.Contains("Piso")&&!child.name.Contains("Parede"))child.gameObject.SetActive(false);
        foreach(var mat in room.GetComponentsInChildren<SantaRitaExitThreshold>(true))mat.GetComponent<SpriteRenderer>().sortingOrder=-2;
        var group=new GameObject("BarCoerente");group.transform.SetParent(room.transform,false);
        var lit=Resources.Load<Material>("Art/WorldLit");
        var floorSprite=NativeSurface(false);
        if(floorSprite!=null){var floor=new GameObject("PisoRef7");floor.transform.SetParent(group.transform,false);floor.transform.position=Origin+new Vector2(9,4.925f);var sr=floor.AddComponent<SpriteRenderer>();sr.sprite=floorSprite;sr.drawMode=SpriteDrawMode.Tiled;sr.size=new Vector2(9.5f,9.85f);sr.color=Color.white;sr.sortingOrder=-5;sr.sharedMaterial=lit;}
        var wallSprite=NativeSurface(true);
        if(wallSprite!=null)
        {
            var oldFloor=room.transform.Find("Piso_Interior_BarDoZe");if(oldFloor!=null)oldFloor.gameObject.SetActive(false);
            Surface(group.transform,"ParedeRef7Fundo",wallSprite,new Vector2(9,10.85f),new Vector2(9.65f,2f),-3,lit,Color.white);
            Surface(group.transform,"ParedeRef7Esq",wallSprite,new Vector2(4.15f,4.925f),new Vector2(.2f,9.85f),-3,lit,new Color(.34f,.28f,.20f));
            Surface(group.transform,"ParedeRef7Dir",wallSprite,new Vector2(13.85f,4.925f),new Vector2(.2f,9.85f),-3,lit,new Color(.34f,.28f,.20f));
            Boundary(group.transform,"LimiteRef7Fundo",new Vector2(9,9.96f),new Vector2(9.8f,.2f));
            Boundary(group.transform,"LimiteRef7Esq",new Vector2(4.15f,4.925f),new Vector2(.2f,10));
            Boundary(group.transform,"LimiteRef7Dir",new Vector2(13.85f,4.925f),new Vector2(.2f,10));
        }
        foreach(var def in Props)
        {
            if(OccupiedChair(def))continue;
            var sprite=SpriteFor(def);if(sprite==null){Debug.LogError("Arte do bar ausente: "+def.key);continue;}
            var g=new GameObject(def.id);g.transform.SetParent(group.transform,false);g.transform.position=Origin+def.position;
            var sr=g.AddComponent<SpriteRenderer>();sr.sprite=sprite;sr.sharedMaterial=lit;
            g.transform.localScale=Vector3.one*(def.kind==Kind.Actor?1f:def.width/sprite.bounds.size.x);sr.sortingOrder=def.id=="TapeteSinuca"?-4:1000-Mathf.RoundToInt(g.transform.position.y*10);
            if(def.supportOffset>0)sr.sortingOrder=1005-Mathf.RoundToInt((g.transform.position.y-def.supportOffset)*10);
            if(def.kind==Kind.Actor||def.kind==Kind.Table||def.kind==Kind.Chair||def.kind==Kind.Counter||def.kind==Kind.Shelf)
            {var col=g.AddComponent<BoxCollider2D>();col.size=new Vector2(sprite.bounds.size.x*.72f,.3f/g.transform.localScale.x);col.offset=new Vector2(0,col.size.y*.5f);}
            if(def.kind==Kind.Actor)live[def.id]=g.transform;
            if(def.id=="CuiaMesa"||def.id=="TermicaMesa"){var action=g.AddComponent<SantaRitaBarPose>();action.propId=def.id;}
            if(def.id=="NPC_Ze_Interior")
            {
                g.AddComponent<InteracaoCapitulo1>().Configurar(InteracaoCapitulo1.TipoInteracao.NpcZeBar);
                var a=new GameObject("TrabalhoZeBalcao");a.transform.SetParent(group.transform,false);a.transform.position=Origin+new Vector2(9.2f,9.1f);
                var b=new GameObject("TrabalhoZeMesa");b.transform.SetParent(group.transform,false);b.transform.position=Origin+def.position;
                var routine=g.AddComponent<SantaRitaNpcWork>();routine.stations=new[]{new SantaRitaNpcWork.Station{anchor=a.transform,duration=17.3f,working=true},new SantaRitaNpcWork.Station{anchor=b.transform,duration=11.7f,working=false}};
                g.AddComponent<SantaRitaBarHost>();
            }
            else if(def.id=="NPC_Frequentador_Interior")g.AddComponent<InteracaoCapitulo1>().Configurar(InteracaoCapitulo1.TipoInteracao.NpcFrequentadorBar);
            else if(def.kind==Kind.Actor){var patron=g.AddComponent<SantaRitaBarPatron>();patron.chimarrao=def.id.Contains("Chimarrao");}
            if(def.kind==Kind.Actor&&!def.key.Contains("Ze")){var pose=g.AddComponent<SantaRitaBarPose>();pose.key=def.key;pose.actorId=def.id;}
            if(def.kind==Kind.Wall){var inspect=g.AddComponent<SantaRitaBarProp>();inspect.calendar=def.id.Contains("Calendario");inspect.title=def.id=="TaqueiraDoBar"?"TAQUEIRA":def.id.Contains("Cartaz")?"ALMOÇO DA COMUNIDADE":def.id.Contains("Calendario")?"OUTUBRO DE 2004":def.id=="Inter1994"?"COLORADO":def.id=="TimeDaComunidade"?"TIME DA COMUNIDADE":def.id=="FiadoSoAmanha"?"FIADO SÓ AMANHÃ":"TELEVISOR";}
        }
        var tableSprite=Resources.Load<Sprite>("Art/Props/MesaSinuca");bool newTable=false;
        if(tableSprite!=null)
        {
            var t=new GameObject("Mesa_Sinuca_Central");t.transform.SetParent(group.transform,false);t.transform.position=Origin+TableCenter-(newTable?Vector2.zero:Vector2.up*(PlayingWidthMetres*UnitsPerMetre*.5f));
            var sr=t.AddComponent<SpriteRenderer>();sr.sprite=tableSprite;sr.sharedMaterial=lit;sr.sortingOrder=1535;
            t.transform.localScale=Vector3.one*(newTable?1:2.84f*UnitsPerMetre*1.105f/tableSprite.bounds.size.x);
            var col=t.AddComponent<BoxCollider2D>();col.size=new Vector2(tableSprite.bounds.size.x*.9f,1.42f*UnitsPerMetre/t.transform.localScale.x);
            col.offset=newTable?Vector2.zero:new Vector2(0,col.size.y*.5f);
        }
        var cue=new GameObject("Mesa_Sinuca_Trigger");cue.transform.SetParent(group.transform,false);cue.transform.position=Origin+TableCenter+Vector2.down*.75f;
        cue.AddComponent<InteracaoCapitulo1>().Configurar(InteracaoCapitulo1.TipoInteracao.MesaSinucaBar);
        var counter=new GameObject("Balcao_Bar_Interacao");counter.transform.SetParent(group.transform,false);counter.transform.position=Origin+new Vector2(8,9.7f);
        counter.AddComponent<InteracaoCapitulo1>().Configurar(InteracaoCapitulo1.TipoInteracao.BalcaoBarDoZe);
    }
    private static void Surface(Transform parent,string id,Sprite sprite,Vector2 position,Vector2 size,int order,Material material,Color tint)
    {var g=new GameObject(id);g.transform.SetParent(parent,false);g.transform.position=Origin+position;var sr=g.AddComponent<SpriteRenderer>();sr.sprite=sprite;sr.drawMode=SpriteDrawMode.Tiled;sr.size=size;sr.sortingOrder=order;sr.sharedMaterial=material;sr.color=tint;}
    private static void Boundary(Transform parent,string id,Vector2 position,Vector2 size)
    {var g=new GameObject(id);g.transform.SetParent(parent,false);g.transform.position=Origin+position;g.AddComponent<BoxCollider2D>().size=size;}
}
public sealed class SantaRitaBarPatron : Interactable
{
    public bool chimarrao;
    private Sprite resting,drinking;private float phase;private int conversations;
    private void Start(){resting=GetComponent<SpriteRenderer>().sprite;drinking=Resources.Load<Sprite>("Art/Characters/"+resting.name.Replace("_Sentado","_Bebendo"));phase=(transform.position.x*7.13f)%19f;}
    private void Update(){if(GetComponent<SantaRitaBarPose>()!=null)return;if(resting==null||drinking==null||MenuPausa.Aberto||GameMessageUI.IsShowing)return;float t=(Time.time+phase)%(chimarrao?23.7f:17.3f);GetComponent<SpriteRenderer>().sprite=t>3&&t<5.1f?drinking:resting;}
    public override string Prompt=>"Conversar";
    public override void Interact()
    {
        string[] lines=chimarrao?new[]{"A água já esfriou. Vou pedir outra térmica antes de passar a cuia.","O mate é amargo. Açúcar fica para o café; se quiser, pede ali no balcão.","Domingo o pessoal vem cedo para ajudar no galeto. Sempre falta alguém para lavar os pratos."}:
            name.Contains("Cerveja")?new[]{"Se a chuva continuar, espero mais um pouco. Não gosto de pegar a estrada com essa água toda.","Uma cerveja só. Depois vou pedir um café e esperar a chuva amansar.","Com esse tempo, até o rádio pega melhor do que a televisão."}:
            new[]{"A antena perdeu o sinal bem na hora do lance. Agora cada um aqui jura que viu uma coisa.","O Zé prometeu arrumar esse rádio. Disse a mesma coisa na semana passada.","Domingo tem almoço da comunidade. Se for, reserva no balcão; ajuda quem precisa calcular a comida."};
        GameMessageUI.Instance.Show(lines[conversations++%lines.Length],"FREQUENTADOR",null);
    }
}
public sealed class SantaRitaBarProp : Interactable
{
    public bool calendar;public string title;
    public override string Prompt=>"Examinar";
    public override void Interact(){var arte=title=="COLORADO"?Resources.Load<Sprite>("Art/Props/Inter1994Detalhe"):GetComponent<SpriteRenderer>().sprite;GameMessageUI.Instance.ShowInspection(title,null,calendar?"Calendário de outubro de 2004. Anotações de compras e do almoço da comunidade, presas com um recibo de entrega de bebidas. As entregas já recebidas estão riscadas a lápis; um círculo marca o almoço de domingo.":title=="TELEVISOR"?"Um jogo antigo passa com chuvisco. A antena perde o sinal quando o vento engrossa.":title=="TAQUEIRA"?"Tacos de comprimentos diferentes, descansando na taqueira. As pontas estão gastas pelo uso.":title=="COLORADO"?"Uma bandeira antiga do Internacional. O monograma e as quatro estrelas já eram usados em 1994. O tecido está desbotado perto da janela.":title=="FIADO SÓ AMANHÃ"?"Cerveja. Fiado só amanhã. A piada já ganhou outra camada de verniz.":title=="ALMOÇO DA COMUNIDADE"?"Galeto, massa e salada no almoço de domingo. Reservas no balcão; levar pratos e talheres.":"Retrato do time da comunidade, fotografado antes de um jogo de domingo. Alguns dos frequentadores do bar aparecem mais jovens.",arteCustom:arte);}
}

public sealed class SantaRitaBarHost : MonoBehaviour
{
    private void LateUpdate()
    {
        var routine=GetComponent<SantaRitaNpcWork>();
        if(SinucaMinigameUI.IsShowing){routine.enabled=false;transform.position=SantaRitaBarLayout.Origin+new Vector2(9.6f,7.95f);}
        else if(!routine.enabled)routine.enabled=true;
    }
}

// Independent of dialogue components; even the existing named patron keeps his own interaction.
public sealed class SantaRitaBarPose : MonoBehaviour
{
    public string key,actorId,propId;
    private SpriteRenderer sr;
    private void Start(){sr=GetComponent<SpriteRenderer>();}
    private void Update(){if(sr==null||MenuPausa.Aberto||GameMessageUI.IsShowing)return;if(!string.IsNullOrEmpty(propId)){sr.enabled=SantaRitaBarLayout.TablePropVisible(propId,Time.time);return;}var pose=SantaRitaBarLayout.PatronPose(key,actorId,Time.time);if(pose!=null)sr.sprite=pose;}
}

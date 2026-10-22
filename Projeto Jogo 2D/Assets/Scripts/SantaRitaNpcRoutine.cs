using UnityEngine;

// Trabalho local animado por relógio do mundo, sem gatilho de chegada do jogador.
[DefaultExecutionOrder(120)]
public sealed class SantaRitaNpcRoutine : MonoBehaviour
{
    public string atividade="papeis";
    public float faseInicial;
    SpriteRenderer body, tool;
    Transform player;
    static Sprite paper,broom,cloth,cup,ball;
    void Start()
    {
        body=GetComponent<SpriteRenderer>();var p=FindAnyObjectByType<PlayerController>();if(p!=null)player=p.transform;
        if(paper==null)PrepareTools();
        var g=new GameObject("ObjetoDeTrabalho");g.transform.SetParent(transform,false);tool=g.AddComponent<SpriteRenderer>();
        tool.sprite=atividade=="varrer"?broom:atividade=="limpar"||atividade=="arrumar"||atividade=="costura"?cloth:atividade=="bochas"?ball:atividade=="chimarrao"||atividade=="espera"?cup:paper;
        tool.sharedMaterial=body!=null?body.sharedMaterial:Resources.Load<Material>("Art/WorldLit");
    }
    void LateUpdate()
    {
        if(body==null||tool==null)return;
        bool talking=GameMessageUI.IsShowing&&player!=null&&Vector2.Distance(player.position,transform.position)<2.5f;
        float t=Time.time+faseInicial*1.7f;bool working=(t%11)<7.5f;
        if(talking)body.flipX=player.position.x<transform.position.x;
        else if(Time.timeScale>0)body.flipX=(t%17)>10;
        tool.enabled=!talking;
        tool.sortingOrder=body.sortingOrder+1;
        bool sweeping=atividade=="varrer";
        float motion=working&&!talking&&!MenuPausa.ReduzirEfeitosIntensos?Mathf.Sin(t*2.4f):0;
        tool.transform.localPosition=sweeping?new Vector3(.42f+motion*.06f,0,0):new Vector3(.3f,.56f+Mathf.Round(motion)/32f,0);
        tool.transform.localRotation=Quaternion.Euler(0,0,sweeping?motion*10:motion*4);
        body.sortingOrder=1000-Mathf.RoundToInt(transform.position.y*10);
    }
    static void PrepareTools()
    {
        Sprite Make(int kind)
        {
            var tex=new Texture2D(16,32,TextureFormat.RGBA32,false){filterMode=FilterMode.Point};var px=new Color[512];
            for(int y=0;y<32;y++)for(int x=0;x<16;x++)
            {
                Color c=Color.clear;
                if(kind==0&&x>1&&x<14&&y<10)c=y%3==1&&x>3&&x<11?new Color(.28f,.27f,.22f):new Color(.77f,.73f,.6f);
                if(kind==1&&x>=7&&x<=8&&y>=5&&y<31)c=new Color(.46f,.31f,.16f);
                if(kind==1&&y<6&&x>1&&x<14)c=new Color(.42f+(x%2)*.08f,.37f,.21f);
                if(kind==2&&x>2&&x<13&&y<7)c=new Color(.55f,.61f,.52f);
                if(kind==3&&y<9&&x>4&&x<12)c=new Color(.37f,.24f,.11f);
                if(kind==3&&x==11&&y>=8&&y<18)c=new Color(.56f,.53f,.4f);
                if(kind==4&&Vector2.Distance(new Vector2(x,y),new Vector2(8,5))<4)c=new Color(.49f,.29f,.14f);
                px[y*16+x]=c;
            }
            tex.SetPixels(px);tex.Apply();return Sprite.Create(tex,new Rect(0,0,16,32),new Vector2(.5f,0),32);
        }
        paper=Make(0);broom=Make(1);cloth=Make(2);cup=Make(3);ball=Make(4);
    }
}

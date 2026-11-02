using UnityEngine;

// Marcador integrado ao chão: preserva o gatilho/portal e não inicia transições.
// A aproximação apenas recupera discretamente a cor da trama, sem setas ou letreiros.
[RequireComponent(typeof(SpriteRenderer))]
public sealed class SantaRitaExitThreshold : MonoBehaviour
{
    private static readonly System.Collections.Generic.List<SantaRitaExitThreshold> all=new System.Collections.Generic.List<SantaRitaExitThreshold>();
    private void OnEnable(){all.Add(this);}
    private void OnDisable(){all.Remove(this);}
    public static bool OnMat(Vector2 player,Vector2 portal)
    {
        SantaRitaExitThreshold nearest=null;float best=3f;
        foreach(var mat in all){if(mat==null)continue;float d=Vector2.Distance(portal,mat.transform.position);if(d<best){best=d;nearest=mat;}}
        if(nearest==null)return Vector2.Distance(player,portal)<.45f;
        var bounds=nearest.GetComponent<SpriteRenderer>().bounds;
        return player.x>=bounds.min.x+.12f&&player.x<=bounds.max.x-.12f&&player.y>=bounds.min.y&&player.y<=bounds.max.y-.10f;
    }
    private SpriteRenderer surface;
    private PlayerController actor;
    private float visibility=.87f;

    public static GameObject Create(Transform parent,string name,Vector2 position,string style,float width,int order)
    {
        var sprite=Resources.Load<Sprite>("Art/Acessos/"+style);
        if(sprite==null)throw new System.InvalidOperationException("Soleira ausente: "+style);
        var g=new GameObject(name);g.transform.SetParent(parent,false);g.transform.position=position;
        g.transform.localScale=Vector3.one*(width/sprite.bounds.size.x);
        var sr=g.AddComponent<SpriteRenderer>();sr.sprite=sprite;sr.sharedMaterial=Resources.Load<Material>("Art/WorldLit");sr.sortingOrder=order;
        g.AddComponent<SantaRitaExitThreshold>();return g;
    }
    private void Awake(){surface=GetComponent<SpriteRenderer>();if(name=="SoleiraSaida"&&GetComponentInParent<Capitulo2Director>()!=null)transform.position+=Vector3.down*.25f;}
    private void LateUpdate()
    {
        if(actor==null)actor=FindAnyObjectByType<PlayerController>();
        float proximity=actor==null?0:1-Mathf.InverseLerp(.8f,2.4f,Vector2.Distance(actor.transform.position,transform.position));
        visibility=Mathf.MoveTowards(visibility,Mathf.Lerp(.87f,1,proximity),Time.unscaledDeltaTime*.4f);
        surface.color=new Color(visibility,visibility,visibility,1);
    }
}

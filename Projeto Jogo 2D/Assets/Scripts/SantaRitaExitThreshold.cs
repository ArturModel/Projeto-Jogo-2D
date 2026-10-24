using UnityEngine;

// Marcador integrado ao chão: preserva o gatilho/portal e não inicia transições.
// A aproximação apenas recupera discretamente a cor da trama, sem setas ou letreiros.
[RequireComponent(typeof(SpriteRenderer))]
public sealed class SantaRitaExitThreshold : MonoBehaviour
{
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
    private void Awake(){surface=GetComponent<SpriteRenderer>();}
    private void LateUpdate()
    {
        if(actor==null)actor=FindAnyObjectByType<PlayerController>();
        float proximity=actor==null?0:1-Mathf.InverseLerp(.8f,2.4f,Vector2.Distance(actor.transform.position,transform.position));
        visibility=Mathf.MoveTowards(visibility,Mathf.Lerp(.87f,1,proximity),Time.unscaledDeltaTime*.4f);
        surface.color=new Color(visibility,visibility,visibility,1);
    }
}

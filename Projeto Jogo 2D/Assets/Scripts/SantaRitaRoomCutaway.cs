using UnityEngine;

// Recorte localizado da fachada interna. Só os visuais desta lista desaparecem;
// paredes físicas, salão, luzes, bancos e pistas permanecem ativos.
public sealed class SantaRitaRoomCutaway : MonoBehaviour
{
    public SpriteRenderer[] walls;
    public Rect inside=new Rect(3.35f,8.1f,26.2f,9.1f);
    public Vector2 approach=new Vector2(4.7f,7.3f);
    public float approachRadius=3;
    private PlayerController actor;
    private float opacity=1;
    public float Opacity=>opacity;
    private void Start(){actor=FindAnyObjectByType<PlayerController>();opacity=ShouldReveal()?0:1;Apply();}
    private bool ShouldReveal()=>actor!=null&&(inside.Contains(actor.transform.position)||Vector2.Distance(actor.transform.position,approach)<approachRadius);
    private void LateUpdate()
    {
        if(actor==null)actor=FindAnyObjectByType<PlayerController>();
        opacity=Mathf.MoveTowards(opacity,ShouldReveal()?0:1,Time.unscaledDeltaTime*2.8f);Apply();
    }
    private void Apply()
    {if(walls!=null)foreach(var wall in walls)if(wall!=null){var c=wall.color;c.a=opacity;wall.color=c;}}
}

using UnityEngine;

// A copa oculta pela profundidade, mas abre leitura quando Marcus está atrás dela.
public sealed class SantaRitaForeground : MonoBehaviour
{
    static PlayerController cachedPlayer;
    SpriteRenderer canopy;Transform player;Color original;float lastAlpha=-1f;
    void Start(){canopy=GetComponent<SpriteRenderer>();if(canopy!=null){original=canopy.color;lastAlpha=original.a;}if(cachedPlayer==null)cachedPlayer=FindAnyObjectByType<PlayerController>();if(cachedPlayer!=null)player=cachedPlayer.transform;}
    void LateUpdate()
    {
        if(canopy==null||player==null)return;
        Vector3 p=player.position,t=transform.position;
        float target=original.a;
        if(p.y>t.y+.3f && p.y-t.y<8f && Mathf.Abs(p.x-t.x)<8f && canopy.bounds.Contains(p+Vector3.up*.8f))target=.48f;
        if(Mathf.Approximately(lastAlpha,target))return;
        lastAlpha=Mathf.MoveTowards(lastAlpha,target,Time.deltaTime*2);
        var color=original;color.a=lastAlpha;
        canopy.color=color;
    }
}

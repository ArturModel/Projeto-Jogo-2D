using UnityEngine;

// A copa oculta pela profundidade, mas abre leitura quando Marcus está atrás dela.
public sealed class SantaRitaForeground : MonoBehaviour
{
    SpriteRenderer canopy;Transform player;Color original;
    void Start(){canopy=GetComponent<SpriteRenderer>();if(canopy!=null)original=canopy.color;var p=FindAnyObjectByType<PlayerController>();if(p!=null)player=p.transform;}
    void LateUpdate()
    {
        if(canopy==null||player==null)return;
        bool covered=player.position.y>transform.position.y+.3f&&canopy.bounds.Contains(player.position+Vector3.up*.8f);
        var color=original;color.a=Mathf.MoveTowards(canopy.color.a,covered?.48f:original.a,Time.deltaTime*2);
        canopy.color=color;
    }
}

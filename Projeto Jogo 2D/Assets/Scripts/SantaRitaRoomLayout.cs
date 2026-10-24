using UnityEngine;
using System.Collections.Generic;

// Apenas a posição de um checkpoint antigo é adaptada; o arquivo original não é regravado.
public sealed class SantaRitaRoomLayout : MonoBehaviour
{
    public Vector2 oldSize, size, arrival;
    public string versionFlag;
    public Vector2 AdaptSavedPosition(Vector2 oldPosition)
    {
        if(GameState.Tem(versionFlag))return FindWalkable(oldPosition);
        Vector2 mapped=new Vector2(Mathf.Clamp01(oldPosition.x/oldSize.x)*size.x,Mathf.Clamp01(oldPosition.y/oldSize.y)*size.y);
        return FindWalkable(mapped);
    }
    private Vector2 FindWalkable(Vector2 p)
    {
        Physics2D.SyncTransforms();
        p=new Vector2(Mathf.Clamp(p.x,1,size.x-1),Mathf.Clamp(p.y,1,size.y-2));
        // Livre de colisão não basta: o interior fechado da cancha também é livre, mas não dá acesso à porta.
        var queue=new Queue<Vector2Int>();var seen=new HashSet<Vector2Int>();
        var start=Vector2Int.RoundToInt(arrival*2);queue.Enqueue(start);seen.Add(start);
        Vector2 best=arrival;float closest=float.PositiveInfinity;
        var dirs=new[]{Vector2Int.up,Vector2Int.down,Vector2Int.left,Vector2Int.right};
        while(queue.Count>0)
        {
            var cell=queue.Dequeue();Vector2 q=(Vector2)cell*.5f;
            if(q.x<.6f||q.x>size.x-.6f||q.y<.7f||q.y>size.y-2.1f||!Free(q))continue;
            float distance=(q-p).sqrMagnitude;if(distance<closest){closest=distance;best=q;}
            foreach(var dir in dirs){var next=cell+dir;if(seen.Add(next))queue.Enqueue(next);}
        }
        return closest<.13f&&Free(p)?p:best;
    }
    private bool Free(Vector2 p)
    {foreach(var c in Physics2D.OverlapCircleAll(p,.4f))if(c.enabled&&!c.isTrigger&&c.GetComponentInParent<PlayerController>()==null&&c.GetComponentInParent<SantaRitaNpcWork>()==null)return false;return true;}
}

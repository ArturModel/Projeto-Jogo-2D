using System.Collections.Generic;
using UnityEngine;

// Desnível raso, atravessável a pé nos dois sentidos; não é portal nem teleporte.
// A passada continua medida pela distância, inclusive ao reduzir a velocidade.
public sealed class SantaRitaShallowStairs : MonoBehaviour
{
    public Rect passage=new Rect(3.85f,6.3f,1.7f,2.8f);
    public Rect lowerRoom=new Rect(3.35f,8.1f,26.2f,9.1f);
    public float depth=.46875f; // Três espelhos de 15,6 cm, projetados no grid de 32 px.
    [Range(.5f,1)]public float pace=.78f;
    private static readonly List<SantaRitaShallowStairs> active=new List<SantaRitaShallowStairs>();
    private void OnEnable(){active.Add(this);}
    private void OnDisable(){active.Remove(this);}
    public static float SpeedAt(Vector2 position)
    {
        float speed=1;
        foreach(var stair in active)if(stair!=null&&stair.passage.Contains(position))speed=Mathf.Min(speed,stair.pace);
        return speed;
    }
    public static float ElevationAt(Vector2 position)
    {
        float amount=0;
        foreach(var stair in active)
        {
            if(stair==null)continue;
            if(!stair.passage.Contains(position))
            {if(stair.lowerRoom.Contains(position))amount=Mathf.Max(amount,stair.depth);continue;}
            float steps=Mathf.Clamp01((position.y-stair.passage.yMin)/stair.passage.height)*3;
            float complete=Mathf.Floor(steps);float rise=Mathf.SmoothStep(0,1,Mathf.InverseLerp(.3f,.85f,steps-complete));
            amount=Mathf.Max(amount,stair.depth*(complete+rise)/3);
        }
        return amount;
    }
}

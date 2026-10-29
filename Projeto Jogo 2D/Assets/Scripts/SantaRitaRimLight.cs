using UnityEngine;
using UnityEngine.Rendering.Universal;

[DefaultExecutionOrder(160)]
public sealed class SantaRitaRimLight : MonoBehaviour
{
    SpriteRenderer body,rim;MaterialPropertyBlock block;Light2D[] lights;float nextScan;
    void Start()
    {
        body=GetComponent<SpriteRenderer>();var mat=Resources.Load<Material>("Art/PixelRim");if(mat==null||body==null){enabled=false;return;}
        var go=new GameObject("RecorteDaLuz");go.transform.SetParent(transform,false);rim=go.AddComponent<SpriteRenderer>();rim.sharedMaterial=mat;block=new MaterialPropertyBlock();
    }
    void LateUpdate()
    {
        if(rim==null)return;
        if(Time.unscaledTime>nextScan){nextScan=Time.unscaledTime+2;lights=FindObjectsByType<Light2D>();}
        Light2D best=null;float strength=0;
        if(lights!=null)foreach(var l in lights)
        {
            if(l==null||!l.isActiveAndEnabled||l.lightType!=Light2D.LightType.Point||l.transform.IsChildOf(transform))continue;
            float distance=Vector2.Distance(l.transform.position,transform.position+Vector3.up*.8f);
            float weight=Mathf.Clamp01(1-distance/Mathf.Max(.1f,l.pointLightOuterRadius))*l.intensity;
            if(weight>strength){strength=weight;best=l;}
        }
        rim.enabled=best!=null&&!MenuPausa.ReduzirEfeitosIntensos;
        if(!rim.enabled)return;
        rim.sprite=body.sprite;rim.flipX=body.flipX;rim.sortingLayerID=body.sortingLayerID;rim.sortingOrder=body.sortingOrder+1;
        Vector2 direction=((Vector2)best.transform.position-(Vector2)transform.position).normalized;
        direction=transform.InverseTransformVector(direction).normalized;
        // Um pixel iluminado, só onde existe luz próxima. Não é contorno permanente.
        block.SetVector("_LightDir",new Vector4(direction.x,direction.y,0,0));block.SetColor("_RimColor",best.color);
        block.SetFloat("_Strength",Mathf.Clamp(strength*.32f,0,.24f));rim.SetPropertyBlock(block);
    }
}

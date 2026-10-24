using UnityEngine;

// Névoa leve de primeiro plano; sem esconder pistas nem mudar colisões.
public sealed class SantaRitaAtmosphere : MonoBehaviour
{
    private ParticleSystem mist;
    private Material material;
    private Texture2D texture;
    private void Start()
    {
        var go=new GameObject("NevoaBaixa");go.transform.SetParent(transform,false);
        mist=go.AddComponent<ParticleSystem>();mist.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        var main=mist.main;main.loop=true;main.maxParticles=18;main.startLifetime=12;main.startSpeed=.12f;
        main.startSize=new ParticleSystem.MinMaxCurve(3f,6f);main.startColor=new Color(.65f,.73f,.78f,.035f);
        main.simulationSpace=ParticleSystemSimulationSpace.World;
        var emission=mist.emission;emission.rateOverTime=1.3f;
        var shape=mist.shape;shape.shapeType=ParticleSystemShapeType.Box;shape.scale=new Vector3(24,14,.1f);
        texture=new Texture2D(16,16,TextureFormat.RGBA32,false);texture.filterMode=FilterMode.Bilinear;
        for(int y=0;y<16;y++)for(int x=0;x<16;x++)
        {float r=new Vector2((x-7.5f)/8,(y-7.5f)/8).magnitude;texture.SetPixel(x,y,new Color(1,1,1,Mathf.Pow(Mathf.Clamp01(1-r),2)));}
        texture.Apply();var template=Resources.Load<Material>("Art/Mist");
        if(template==null){Destroy(go);enabled=false;return;}
        material=new Material(template);material.mainTexture=texture;
        material.SetFloat("_Surface",1);material.SetFloat("_Blend",0);material.SetFloat("_SrcBlend",5);material.SetFloat("_DstBlend",10);material.SetFloat("_ZWrite",0);
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");material.renderQueue=3000;
        var renderer=mist.GetComponent<ParticleSystemRenderer>();renderer.sharedMaterial=material;renderer.sortingOrder=2100;
        if(!MenuPausa.ReduzirEfeitosIntensos && !Interior())mist.Play();
    }
    private void Update()
    {
        if(mist==null)return;
        bool disabled=MenuPausa.ReduzirEfeitosIntensos || Interior();
        if(disabled && mist.isPlaying)mist.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        else if(!disabled && !mist.isPlaying)mist.Play();
    }
    private void OnDestroy(){if(material!=null)Destroy(material);if(texture!=null)Destroy(texture);}
    private static bool Interior()=>Capitulo1Director.EstaEmInterior||(Capitulo2Director.Instance!=null&&Capitulo2Director.Instance.interior);
}

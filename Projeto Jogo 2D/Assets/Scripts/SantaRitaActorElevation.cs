using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Projeta o desnível nos pixels do ator e nos seus pés físicos, conservando
// coordenadas de navegação, saves, quadros originais e o evento de apoio.
[DefaultExecutionOrder(150)]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class SantaRitaActorElevation : MonoBehaviour
{
    private SpriteRenderer body;
    private Collider2D feet;
    private Vector2 baseOffset;
    private PlayerController player;
    private float depth;
    private bool initialized;
    private SpriteRenderer contact;
    private Color baseColor;
    private Vector2 previous;
    private readonly Dictionary<Sprite,Dictionary<int,Sprite>> shifted=new Dictionary<Sprite,Dictionary<int,Sprite>>();
    private readonly Dictionary<Sprite,Sprite> originals=new Dictionary<Sprite,Sprite>();
    public float VisualDepth=>depth;
    public float VisualScale=>Mathf.Lerp(1,.9f,Mathf.Clamp01(depth/.46875f));
    private void Awake()
    {
        body=GetComponent<SpriteRenderer>();feet=GetComponent<Collider2D>();player=GetComponent<PlayerController>();
        if(feet!=null)baseOffset=feet.offset;previous=transform.position;baseColor=body.color;
        var g=new GameObject("SombraDosPes");g.transform.SetParent(transform,false);contact=g.AddComponent<SpriteRenderer>();
        contact.sprite=Resources.Load<Sprite>("Art/Acessos/SombraContato");contact.sharedMaterial=Resources.Load<Material>("Art/WorldLit");
    }
    private void LateUpdate()
    {
        Vector2 position=transform.position;float target=SantaRitaShallowStairs.ElevationAt(position);
        depth=!initialized||Vector2.Distance(position,previous)>2?target:Mathf.MoveTowards(depth,target,Time.deltaTime*.95f);previous=position;initialized=true;
        float sy=Mathf.Max(.01f,Mathf.Abs(transform.lossyScale.y));
        int pixels=Mathf.RoundToInt(depth*32);float projected=pixels/32f;
        if(feet!=null)feet.offset=baseOffset+Vector2.down*(projected/sy);
        Sprite source=body.sprite;if(source==null)return;
        if(originals.TryGetValue(source,out var original))source=original;
        float renderScale=Mathf.Lerp(1,.9f,Mathf.Clamp01(projected/.46875f));
        float ppu=source.pixelsPerUnit/renderScale;
        float localPixels=projected*ppu/sy;
        if(pixels==0)body.sprite=source;
        else
        {
            if(!shifted.TryGetValue(source,out var poses)){poses=new Dictionary<int,Sprite>();shifted.Add(source,poses);}
            if(!poses.TryGetValue(pixels,out var pose))
            {
                Vector2 pivot=(source.pivot+Vector2.up*localPixels)/source.rect.size;
                pose=Sprite.Create(source.texture,source.rect,pivot,ppu,0,SpriteMeshType.FullRect);
                pose.name=source.name+"_desnivel_"+pixels;poses.Add(pixels,pose);originals.Add(pose,source);
            }
            body.sprite=pose;
        }
        body.sortingOrder=1000-Mathf.RoundToInt((position.y-projected)*10);
        float blend=Mathf.Clamp01(depth/.46875f);body.color=baseColor*Color.Lerp(Color.white,new Color(.84f,.87f,.89f),blend);
        contact.transform.localPosition=new Vector3(0,-projected/sy+.015f,0);contact.transform.localScale=new Vector3(.36f*renderScale/Mathf.Abs(transform.lossyScale.x),.36f*renderScale/sy,1);
        contact.sortingOrder=body.sortingOrder-1;
        if(player!=null&&player.LanternaVisivel)
            foreach(var lamp in GetComponentsInChildren<Light2D>())lamp.transform.position=transform.position+(lamp.transform.position-transform.position)*renderScale+Vector3.down*projected;
    }
    private void OnDestroy(){foreach(var pose in originals.Keys)if(pose!=null)Destroy(pose);}
}

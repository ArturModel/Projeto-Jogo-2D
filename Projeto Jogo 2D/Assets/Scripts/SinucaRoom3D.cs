using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Persistent GPU geometry, isolated from the world's Renderer2D and physics kernel.
public sealed partial class SinucaRoom3D : IDisposable
{
    public const float MetresPerUnit=2.84f/420f,TableY=.82f;
    public readonly GameObject root;
    public readonly Camera camera;
    public RenderTexture Target{get;private set;}
    public int StaticRendererCount{get;private set;}
    public int FrameGeometryBuilds=>0;
    public Texture2D FloorTile=>tiles[0];
    public Texture2D WallTile=>tiles[1];
    public readonly Dictionary<string,Sprite> PixelSources=new Dictionary<string,Sprite>();
    public readonly Dictionary<string,Renderer[]> ArtModels=new Dictionary<string,Renderer[]>();
    private readonly Transform scenery,dynamic;
    private readonly Dictionary<string,Material> materials=new Dictionary<string,Material>();
    private readonly List<UnityEngine.Object> owned=new List<UnityEngine.Object>();
    private readonly Transform[] balls=new Transform[16],nets=new Transform[6];
    private readonly float[] impactTime={-10,-10,-10,-10,-10,-10};
    private readonly bool[] wasFalling=new bool[16];
    private readonly List<Actor> actors=new List<Actor>();
    private readonly LineRenderer guide;
    private LineRenderer objectGuide,objectArrow,guideBorder;
    private readonly Transform[] markedSigns=new Transform[16];
    private Transform[] lampParts;
    private readonly Transform ghost,cue;
    private Renderer wallLeft,wallRight,wallBack,wallFront;
    private float yaw=8,topYaw,zoom=1,cueHeading;
    private SinucaCameraView mode;
    private Vector2 white;
    private readonly Texture2D[] tiles=new Texture2D[8];
    private sealed class Actor{public Transform root;public MeshFilter filter;public MeshRenderer renderer;public Sprite rest,drink;public float phase;public bool seated;public SantaRitaBarLayout.Prop definition;}
    private const int Layer=30;
    public SinucaRoom3D(int width,int height)
    {
        root=new GameObject("SalaSinuca3D_Persistente");UnityEngine.Object.DontDestroyOnLoad(root);root.transform.position=Vector3.zero;
        scenery=new GameObject("CenarioEstatico").transform;scenery.SetParent(root.transform,false);dynamic=new GameObject("ElementosMoveis").transform;dynamic.SetParent(root.transform,false);
        tiles[0]=SantaRitaBarLayout.NativeSurface(false).texture;tiles[1]=SantaRitaBarLayout.NativeSurface(true).texture;
        tiles[2]=Resources.Load<Texture2D>("Art/Tiles/Madeira");
        var felt=new Texture2D(8,8,TextureFormat.RGBA32,false){filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Repeat};
        var table=Resources.Load<Sprite>("Art/Props/MesaSinuca").texture;var sum=Color.black;int count=0;
        foreach(var c in table.GetPixels())if(c.a>.9f&&c.g>c.r*1.3f&&c.g>c.b*1.15f){sum+=c;count++;}
        Color cloth=count>0?sum/count:new Color(.12f,.32f,.20f);for(int y=0;y<8;y++)for(int x=0;x<8;x++)felt.SetPixel(x,y,cloth*(1+(((x*13+y*7)%5)-2)*.008f));felt.Apply();tiles[3]=felt;owned.Add(felt);
        camera=new GameObject("CameraSinuca3D").AddComponent<Camera>();camera.transform.SetParent(root.transform,false);camera.cullingMask=1<<Layer;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.065f,.047f,.03f);camera.nearClipPlane=.015f;camera.farClipPlane=35;camera.depth=-10;camera.allowHDR=false;
        var data=camera.gameObject.AddComponent<UniversalAdditionalCameraData>();data.SetRenderer(1);data.renderPostProcessing=false;data.renderShadows=true;data.volumeLayerMask=0;
        Target=new RenderTexture(width,height,24,RenderTextureFormat.ARGB32){name="Sinuca_Offscreen",antiAliasing=2,filterMode=FilterMode.Bilinear};Target.Create();camera.targetTexture=Target;owned.Add(Target);
        BuildRoom();CaptureArt("Mesa_Sinuca_Central",BuildTable);for(int n=0;n<16;n++){balls[n]=Sphere("Bola_"+n,new Vector3(0,TableY+SinucaTableView.BallRadius*MetresPerUnit,0),SinucaTableView.BallRadius*2*MetresPerUnit,BallMaterial(n),dynamic).transform;}
        cue=BuildCue(dynamic);guide=new GameObject("MiraFina").AddComponent<LineRenderer>();guide.transform.SetParent(dynamic,false);guide.useWorldSpace=false;guide.positionCount=2;guide.startWidth=guide.endWidth=.0045f;guide.numCapVertices=4;guide.sharedMaterial=Mat("mira",new Color(.82f,.88f,.78f),null,true);guide.shadowCastingMode=ShadowCastingMode.Off;guide.receiveShadows=false;
        objectGuide=new GameObject("DirecaoDaColorida").AddComponent<LineRenderer>();objectGuide.transform.SetParent(dynamic,false);objectGuide.useWorldSpace=false;objectGuide.positionCount=2;objectGuide.startWidth=.005f;objectGuide.endWidth=.0035f;objectGuide.sharedMaterial=Mat("direcaoObjeto",new Color(1f,.74f,.25f),null,true);objectGuide.shadowCastingMode=ShadowCastingMode.Off;
        guideBorder=NewGuide("ContrasteMira",.011f,Mat("sombraMira",new Color(.025f,.08f,.045f),null,true),2);
        objectArrow=NewGuide("SetaDaColorida",.005f,objectGuide.sharedMaterial,3);
        ghost=Ring("ContatoPrevisto",SinucaTableView.BallRadius*MetresPerUnit,.002f,new Color(.94f,.94f,.82f),dynamic,32).transform;
        for(int n=1;n<16;n++){var mark=new GameObject("MarcaGiz_"+n);mark.transform.SetParent(dynamic,false);var mat=Mat("gizDaMarcada",new Color(.9f,.92f,.65f),null,true);Tube("X1",new Vector3(-.016f,-.016f,0),new Vector3(.016f,.016f,0),.0025f,mat,mark.transform);Tube("X2",new Vector3(-.016f,.016f,0),new Vector3(.016f,-.016f,0),.0025f,mat,mark.transform);markedSigns[n]=mark.transform;mark.SetActive(false);}
        lampParts=scenery.GetComponentsInChildren<Transform>(true).Where(t=>t.name=="Cupula"||t.name=="FioLampada"||t.name=="Lampada").ToArray();
        foreach(var t in root.GetComponentsInChildren<Transform>(true))t.gameObject.layer=Layer;
        wallLeft=scenery.Find("LambrilEsq").GetComponent<Renderer>();wallRight=scenery.Find("LambrilDir").GetComponent<Renderer>();wallBack=scenery.Find("LambrilFundo").GetComponent<Renderer>();wallFront=scenery.Find("LambrilFrente").GetComponent<Renderer>();
        StaticRendererCount=scenery.GetComponentsInChildren<MeshRenderer>().Length;UpdateCamera();
    }
    private LineRenderer NewGuide(string name,float width,Material material,int count)
    {var g=new GameObject(name);g.transform.SetParent(dynamic,false);var line=g.AddComponent<LineRenderer>();line.useWorldSpace=false;line.positionCount=count;line.startWidth=line.endWidth=width;line.numCapVertices=4;line.sharedMaterial=material;line.shadowCastingMode=ShadowCastingMode.Off;line.receiveShadows=false;return line;}
    private void CombineNet(Transform parent)
    {
        var groups=new Dictionary<Material,List<CombineInstance>>();
        foreach(var f in parent.GetComponentsInChildren<MeshFilter>())
        {
            var r=f.GetComponent<MeshRenderer>();if(r==null)continue;var mat=r.sharedMaterial;
            if(!groups.TryGetValue(mat,out var list)){list=new List<CombineInstance>();groups[mat]=list;}
            list.Add(new CombineInstance{mesh=f.sharedMesh,transform=parent.worldToLocalMatrix*f.transform.localToWorldMatrix});r.enabled=false;
        }
        foreach(var pair in groups){var mesh=new Mesh{name="MalhaRedeCombinada",indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};mesh.CombineMeshes(pair.Value.ToArray(),true,true);owned.Add(mesh);var g=new GameObject("MalhaBarbante");g.transform.SetParent(parent,false);g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=pair.Key;}
    }
    private void CaptureArt(string id,Action build)
    {
        var before=new HashSet<Renderer>(root.GetComponentsInChildren<Renderer>(true));build();
        ArtModels[id]=root.GetComponentsInChildren<Renderer>(true).Where(r=>!before.Contains(r)).ToArray();
    }
    private Material Mat(string key,Color tint,Texture tex=null,bool unlit=false)
    {
        if(materials.TryGetValue(key,out var m))return m;
        m=new Material(Resources.Load<Material>("Art/BarRef7/"+(unlit?"BarUnlit":"BarLit"))){name=key};m.SetColor("_BaseColor",tint);m.color=tint;if(tex!=null){m.SetTexture("_BaseMap",tex);m.mainTexture=tex;}m.enableInstancing=true;m.SetFloat("_Smoothness",.15f);materials.Add(key,m);owned.Add(m);return m;
    }
    private Material Wood=>Mat("madeiraNativa",new Color(.64f,.54f,.41f),tiles[2]);
    private Material DarkWood=>Mat("madeiraEscuraNativa",new Color(.43f,.36f,.28f),tiles[2]);
    private GameObject Primitive(string name,PrimitiveType type,Vector3 at,Vector3 size,Material m,Transform parent)
    {var g=GameObject.CreatePrimitive(type);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=at;g.transform.localScale=size;UnityEngine.Object.Destroy(g.GetComponent<Collider>());g.GetComponent<MeshRenderer>().sharedMaterial=m;g.layer=Layer;return g;}
    private GameObject Box(string n,Vector3 p,Vector3 s,Material m,Transform t=null)=>Primitive(n,PrimitiveType.Cube,p,s,m,t??scenery);
    private GameObject Sphere(string n,Vector3 p,float s,Material m,Transform t)=>Primitive(n,PrimitiveType.Sphere,p,Vector3.one*s,m,t);
    private GameObject Cylinder(string n,Vector3 p,float r,float h,Material m,Transform t=null)=>Primitive(n,PrimitiveType.Cylinder,p,new Vector3(r*2,h*.5f,r*2),m,t??scenery);
    private GameObject Tube(string n,Vector3 a,Vector3 b,float r,Material m,Transform t=null){var g=Cylinder(n,(a+b)*.5f,r,Vector3.Distance(a,b),m,t);g.transform.localRotation=Quaternion.FromToRotation(Vector3.up,(b-a).normalized);return g;}
    private GameObject Ring(string n,float r,float wire,Color c,Transform t,int count=20)
    {var g=new GameObject(n);g.transform.SetParent(t,false);var m=Mat("corda_"+c.ToString(),c);for(int j=0;j<count;j++){float a=j*Mathf.PI*2/count,b=(j+1)*Mathf.PI*2/count;Tube("fio",new Vector3(Mathf.Cos(a)*r,0,Mathf.Sin(a)*r),new Vector3(Mathf.Cos(b)*r,0,Mathf.Sin(b)*r),wire,m,g.transform);}return g;}
    private void Panel(string n,Vector3 at,Vector2 size,Texture tex,Transform t=null,bool frame=true)
    {
        if(frame)Box(n+"_Moldura",at+new Vector3(0,0,.04f),new Vector3(size.x+.06f,size.y+.06f,.045f),DarkWood,t);
        var g=GameObject.CreatePrimitive(PrimitiveType.Quad);g.name=n;g.transform.SetParent(t??scenery,false);g.transform.localPosition=at;g.transform.localScale=new Vector3(size.x,size.y,1);UnityEngine.Object.Destroy(g.GetComponent<Collider>());g.GetComponent<MeshRenderer>().sharedMaterial=Mat(n+"_textura",Color.white,tex);g.GetComponent<MeshRenderer>().shadowCastingMode=ShadowCastingMode.Off;g.layer=Layer;
    }
    public static Vector3 Plan(Vector2 xy){Vector2 d=(xy-SantaRitaBarLayout.TableCenter)*1.15f;return new Vector3(d.x,0,d.y);}
    public void SetMode(SinucaCameraView value,float angle){mode=value;cueHeading=angle;UpdateCamera();}
    public void Orbit(float degrees){if(mode==SinucaCameraView.LinhaTaco)cueHeading-=degrees*Mathf.Deg2Rad;else if(mode==SinucaCameraView.VistaSuperior)topYaw+=degrees;else yaw+=degrees;UpdateCamera();}
    public void SetZoom(float value){zoom=Mathf.Clamp(value,.8f,1.35f);UpdateCamera();}
    public void Recenter(float angle){cueHeading=angle;UpdateCamera();}
    private void UpdateCamera()
    {
        camera.orthographic=mode==SinucaCameraView.VistaSuperior;
        if(camera.orthographic){camera.transform.localPosition=new Vector3(0,6,0);camera.transform.localRotation=Quaternion.Euler(90,topYaw,0);camera.orthographicSize=1.25f/zoom;}
        else if(mode==SinucaCameraView.Mesa3D){var target=new Vector3(0,1f,.5f);camera.transform.localPosition=target+Quaternion.Euler(0,yaw,0)*new Vector3(0,2f,-4.3f)/zoom;camera.transform.localRotation=Quaternion.LookRotation(target-camera.transform.localPosition);camera.fieldOfView=46;}
        else{var dir=new Vector3(Mathf.Cos(cueHeading),0,Mathf.Sin(cueHeading));var ball=new Vector3(white.x*MetresPerUnit,TableY,white.y*MetresPerUnit);camera.transform.localPosition=ball-dir*(.65f/zoom)+Vector3.up*.43f;camera.transform.localRotation=Quaternion.LookRotation(ball+dir*.85f+Vector3.up*.045f-camera.transform.localPosition);camera.fieldOfView=48;}
        // Cut only an intervening wall when inspecting from outside; inside cue views remain enclosed.
        if(lampParts!=null)foreach(var t in lampParts)t.gameObject.SetActive(mode!=SinucaCameraView.VistaSuperior);
        OrientPixelArt();
        if(wallLeft!=null){var p=camera.transform.localPosition;wallLeft.enabled=p.x>-5.3f;wallRight.enabled=p.x<5.3f;wallBack.enabled=p.z<3.7f;wallFront.enabled=p.z>-7.3f;}
    }
    public Vector2 Project(Vector3 sim){var p=root.transform.TransformPoint(new Vector3(sim.x*MetresPerUnit,TableY+sim.y*MetresPerUnit,sim.z*MetresPerUnit));return camera.WorldToViewportPoint(p);}
    public bool RayToTable(Vector2 viewport,out Vector2 sim)
    {var ray=camera.ViewportPointToRay(viewport);var plane=new Plane(Vector3.up,root.transform.position+Vector3.up*(TableY+SinucaTableView.BallRadius*.4f*MetresPerUnit));if(plane.Raycast(ray,out float d)&&d>0){var p=root.transform.InverseTransformPoint(ray.GetPoint(d));sim=new Vector2(p.x,p.z)/MetresPerUnit;return true;}sim=Vector2.zero;return false;}
    public void Sync(IList<SinucaVisualBall> values,Vector2 cuePos,float angle,float power,bool showCue,SinucaAimGuideData g)
    {
        white=cuePos;UpdateCamera();
        for(int i=0;i<values.Count;i++)
        {
            var b=values[i];int n=b.numero;if(n<0||n>15)continue;bool visible=b.ativa||b.caindo||b.quedaProgresso>=.99f;balls[n].gameObject.SetActive(visible);if(!visible)continue;var at=new Vector3(b.pos.x*MetresPerUnit,TableY+SinucaTableView.BallRadius*MetresPerUnit,b.pos.y*MetresPerUnit);
            if(b.caindo||!b.ativa){int pocket=0;float best=999;for(int j=0;j<6;j++){float d=Vector2.Distance(b.cacapaAlvo*MetresPerUnit,PocketPhysics(j));if(d<best){best=d;pocket=j;}}var p=PocketOuter(pocket);at=Vector3.Lerp(at,new Vector3(p.x,.56f,p.y),Mathf.SmoothStep(0,1,Mathf.Clamp01(b.quedaProgresso)));if(!wasFalling[n])impactTime[pocket]=Time.unscaledTime;}
            wasFalling[n]=b.caindo||!b.ativa;balls[n].localPosition=at;balls[n].localRotation=b.rotacao;
            if(n>0){markedSigns[n].gameObject.SetActive(b.marcada&&b.ativa);markedSigns[n].localPosition=at+Vector3.up*.065f;markedSigns[n].localRotation=camera.transform.localRotation;}
        }
        var dir=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle));cue.gameObject.SetActive(showCue);cue.localPosition=new Vector3(cuePos.x*MetresPerUnit,TableY+.029f,cuePos.y*MetresPerUnit)-dir*(power*.14f);cue.localRotation=Quaternion.LookRotation(dir);
        guide.gameObject.SetActive(g.ativa);ghost.gameObject.SetActive(g.ativa);if(g.ativa){guide.SetPosition(0,new Vector3(g.origem.x*MetresPerUnit,TableY+.005f,g.origem.y*MetresPerUnit));guide.SetPosition(1,new Vector3(g.pontoContato.x*MetresPerUnit,TableY+.005f,g.pontoContato.y*MetresPerUnit));ghost.localPosition=new Vector3(g.pontoContato.x*MetresPerUnit,TableY+.005f,g.pontoContato.y*MetresPerUnit);}
        guideBorder.gameObject.SetActive(g.ativa);if(g.ativa){guideBorder.SetPosition(0,guide.GetPosition(0)-Vector3.up*.001f);guideBorder.SetPosition(1,guide.GetPosition(1)-Vector3.up*.001f);}
        objectGuide.gameObject.SetActive(g.ativa&&g.atingeBola);objectArrow.gameObject.SetActive(g.ativa&&g.atingeBola);
        if(g.ativa&&g.atingeBola)
        {
            var a=g.posBolaAlvo;var d=g.direcaoBolaAlvo.normalized;float length=65;
            if(Mathf.Abs(d.x)>.001f)length=Mathf.Min(length,((d.x>0?204:-204)-a.x)/d.x);
            if(Mathf.Abs(d.y)>.001f)length=Mathf.Min(length,((d.y>0?99:-99)-a.y)/d.y);
            var b=a+d*Mathf.Max(0,length);var from=new Vector3(a.x*MetresPerUnit,TableY+.013f,a.y*MetresPerUnit);var tip=new Vector3(b.x*MetresPerUnit,TableY+.013f,b.y*MetresPerUnit);var objectDir=new Vector3(d.x,0,d.y);var side=new Vector3(-d.y,0,d.x);
            objectGuide.SetPosition(0,from);objectGuide.SetPosition(1,tip);objectArrow.SetPosition(0,tip-objectDir*.05f+side*.025f);objectArrow.SetPosition(1,tip);objectArrow.SetPosition(2,tip-objectDir*.05f-side*.025f);
        }
        for(int j=0;j<6;j++){float t=Time.unscaledTime-impactTime[j];float bounce=t<2?Mathf.Sin(t*22)*Mathf.Exp(-t*3)*.09f:0;nets[j].localScale=new Vector3(1,1+bounce,1);}
        foreach(var a in actors){if(a.definition.kind!=SantaRitaBarLayout.Kind.Actor){a.renderer.enabled=SantaRitaBarLayout.TablePropVisible(a.definition.id,Time.time);continue;}var sprite=a.seated?SantaRitaBarLayout.PatronPose(a.definition.key,a.definition.id,Time.time)??a.rest:a.rest;a.filter.sharedMesh=NativeMesh(sprite);a.renderer.sharedMaterial=PixelMaterial(sprite);}

    }
    public void SetActive(bool value){root.SetActive(value);}
    public void Dispose(){UnityEngine.Object.Destroy(root);foreach(var o in owned)if(o!=null)UnityEngine.Object.Destroy(o);}
}

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
    private Renderer wallLeft,wallRight,wallBack;
    private Renderer ceilingRenderer;
    private Renderer[] ceilingBeams;
    private float yaw=8,topYaw,zoom=1,cueHeading;
    private SinucaCameraView mode;
    private Vector2 white;
    // Independent standing position: never follows the cue ball after a shot.
    private Vector3 standingPosition = new Vector3(-2.1f,1.62f,-1.25f);
    private float standingYaw=58f, standingPitch=22f;
    private float entryLook;
    private float distanciaPasso;
    private int variantePasso;
    private bool cameraInitialized;
    private float poseBlend;
    private Transform bridgeHand,bridgeThumb,gripHand,bridgeAnchor;
    private static readonly Vector2 BridgeContactUV=new Vector2(.569f,.662f);
    private Camera firstPersonCamera;
    private const int FirstPersonLayer=29;
    private float strokeStarted=-10,strokePower;
    private Vector3 strokeOrigin;
    public static float BridgeRailLift(Vector2 ball,Vector2 direction)
    {
        var bridge=ball*MetresPerUnit-direction.normalized*.25f;
        return Mathf.Abs(bridge.x)>1.39f || Mathf.Abs(bridge.y)>.69f ? .045f : 0f;
    }
    public void BeginStroke(float power){strokeStarted=Time.time;strokePower=power;}
    private Transform HandSprite(string name,Transform parent,Vector2 pivot,float width,bool grip=false,float bridgeLayer=0)
    {
        const string resource="Art/Sinuca/MaosMarcusUnificadas";
        var texture=Resources.Load<Texture2D>(resource);if(texture==null){Debug.LogWarning("Sinuca: arte de mão ausente: "+resource);return null;}
        // Both poses share one palette/style atlas; the new top-down grip is already mirrored in the art.
        float cell=texture.width*.5f;
        var sprite=Sprite.Create(texture,new Rect(grip?cell:0,0,cell,texture.height),pivot,cell/width,0,SpriteMeshType.FullRect);
        owned.Add(sprite);var hand=PixelObject(name,sprite,Vector3.zero,1/1.15f,false);hand.SetParent(parent,false);hand.gameObject.layer=Layer;
        var shader=Resources.Load<Shader>("Art/Sinuca/MaoFlutuante");
        if(shader!=null)
        {
            var material=new Material(shader){name=name+"_PulsoPixel"};material.SetTexture("_BaseMap",texture);
            material.SetVector("_WristOrigin",grip?new Vector4(.56f,.06f,0,0):new Vector4(.055f,.025f,0,0));
            material.SetFloat("_BridgeLayer",bridgeLayer);
            // The closed grip must cover the shaft across its entire opaque silhouette.
            // A small depth offset alone fails where the long 3D shaft approaches the camera.
            if(grip){material.SetFloat("_ZTest",8f);material.renderQueue=2470;}
            material.SetVector("_BridgeSplit",new Vector4(BridgeContactUV.x*.5f,BridgeContactUV.y,.14f,0));
            material.SetVector("_WristDirection",grip?new Vector4(.4f,.6f,0,0):new Vector4(.325f,.76f,0,0));
            material.SetFloat("_WristFade",grip?.08f:.12f);
            hand.GetComponent<MeshRenderer>().sharedMaterial=material;owned.Add(material);
        }
        return hand;
    }
    private void BuildHands()
    {
        bridgeAnchor=new GameObject("ApoioNaMesa").transform;bridgeAnchor.SetParent(dynamic,false);
        bridgeHand=HandSprite("MaoApoioMarcus",bridgeAnchor,BridgeContactUV,.21f,false,1);
        bridgeThumb=HandSprite("PolegarApoioMarcus",bridgeAnchor,BridgeContactUV,.21f,false,2);
        // This transform is a child of the actual cue, not a screen overlay.
        gripHand=HandSprite("MaoEmpunhaduraMarcus",cue,new Vector2(.55f,.50f),.16f,true);
        if(bridgeHand!=null){bridgeHand.localPosition=new Vector3(0f,.047f,-.25f);bridgeHand.localRotation=Quaternion.identity;}
        if(gripHand!=null){gripHand.localPosition=new Vector3(0f,0f,-.72f);gripHand.localRotation=Quaternion.identity;}
    }
    public static float GripScreenRoll(Vector2 shaftInPixels,float spriteShaftAngle)
        => shaftInPixels.sqrMagnitude<.001f?0:Mathf.Atan2(shaftInPixels.y,shaftInPixels.x)*Mathf.Rad2Deg-spriteShaftAngle;
    private void AlignGripToShaft()
    {
        if(gripHand==null)return;
        var contact=cue.TransformPoint(new Vector3(0,0,-.72f));
        var rear=camera.WorldToViewportPoint(contact);
        var front=camera.WorldToViewportPoint(cue.TransformPoint(new Vector3(0,0,-.20f)));
        var pixels=new Vector2((front.x-rear.x)*Target.width,(front.y-rear.y)*Target.height);
        float roll=GripScreenRoll(pixels,90f); // the reference shaft is vertical in the new sprite
        // The new sprite already contains left-hand anatomy and the drawn top-down wrist angle.
        gripHand.rotation=camera.transform.rotation*Quaternion.AngleAxis(roll,Vector3.forward);
        PlaceHandDepth(gripHand,contact,-.035f);
    }
    // Scale along the camera ray to change depth without shifting the contact or the UV seam.
    private void PlaceHandDepth(Transform hand,Vector3 contact,float offset)
    {
        var ray=contact-camera.transform.position;
        float depth=Vector3.Dot(ray,camera.transform.forward);
        float scale=depth>.05f?Mathf.Max(.1f,(depth+offset)/depth):1f;
        hand.position=camera.transform.position+ray*scale;
        hand.localScale=Vector3.one*scale;
    }
    private void AlignBridgeToShaft()
    {
        if(bridgeHand==null || bridgeThumb==null)return;
        // The support follows the shaft at a fixed plane behind the ball.
        // Sliding the cue along itself during pullback leaves this point fixed.
        var support=bridgeAnchor.TransformPoint(new Vector3(0f,.047f,-.25f));
        float alignment=Vector3.Dot(cue.forward,bridgeAnchor.forward);
        if(Mathf.Abs(alignment)<.01f)return;
        var contact=cue.position+cue.forward*(Vector3.Dot(support-cue.position,bridgeAnchor.forward)/alignment);
        var rear=camera.WorldToViewportPoint(contact);
        var front=camera.WorldToViewportPoint(contact+cue.forward*.20f);
        var pixels=new Vector2((front.x-rear.x)*Target.width,(front.y-rear.y)*Target.height);
        var rotation=camera.transform.rotation*Quaternion.AngleAxis(GripScreenRoll(pixels,90f),Vector3.forward);
        bridgeHand.rotation=bridgeThumb.rotation=rotation;
        // Complementary masks of the SAME approved sprite: wood lies over dorsum, under thumb.
        PlaceHandDepth(bridgeHand,contact,.025f);
        PlaceHandDepth(bridgeThumb,contact,-.025f);
    }
    private void BuildFirstPersonLayer()
    {
        // A separate depth buffer is the usual FPS view-model treatment: the tabletop
        // cannot slice the wrists, while cue and hands still occlude each other in 3D.
        foreach(var model in new[]{cue,bridgeAnchor,maoMarcusPorta})
            if (model != null)
                foreach(var part in model.GetComponentsInChildren<Transform>(true))part.gameObject.layer=FirstPersonLayer;
        firstPersonCamera=new GameObject("CameraTacoEMaos").AddComponent<Camera>();
        firstPersonCamera.transform.SetParent(root.transform,false);
        var overlay=firstPersonCamera.gameObject.AddComponent<UniversalAdditionalCameraData>();
        overlay.SetRenderer(1);overlay.renderType=CameraRenderType.Overlay; // URP overlay defaults to clearing depth
        overlay.renderPostProcessing=false;overlay.renderShadows=false;
        camera.GetUniversalAdditionalCameraData().cameraStack.Add(firstPersonCamera);
        foreach(var light in root.GetComponentsInChildren<Light>())light.cullingMask|=1<<FirstPersonLayer;
        SyncFirstPersonCamera();
    }
    private void SyncFirstPersonCamera()
    {
        if(firstPersonCamera==null)return;
        firstPersonCamera.CopyFrom(camera);
        firstPersonCamera.transform.SetPositionAndRotation(camera.transform.position,camera.transform.rotation);
        firstPersonCamera.cullingMask=1<<FirstPersonLayer;
        firstPersonCamera.targetTexture=null; // inherits the base camera's RenderTexture through its URP stack
        firstPersonCamera.depth=camera.depth+1;
    }
    public bool Walking => mode == SinucaCameraView.Caminhando3D;
    public bool SettledAtCue => !Walking && poseBlend > .97f;
    public Vector2 StandingXZ => new Vector2(standingPosition.x,standingPosition.z);

    public void ResetWalking()
    {
        standingPosition=new Vector3(-2.1f,1.62f,-1.25f);standingYaw=58f;standingPitch=22f;
        entryLook=0;cameraInitialized=false;poseBlend=0;SetMode(SinucaCameraView.Caminhando3D,0);
    }
    internal void SetEntryLook(float weight)
    {
        entryLook = weight;
        if (weight > 0f) cue.gameObject.SetActive(false);
        UpdateCamera();
    }
    public bool CanReachWhite(Vector2 ball)
        => Vector2.Distance(StandingXZ,ball*MetresPerUnit)<=1.9f;
    public float ApproachAngle(Vector2 ball)
    { var d=ball*MetresPerUnit-StandingXZ;return Mathf.Atan2(d.y,d.x); }
    public static bool InsideTableFootprint(Vector2 p)
        => Mathf.Abs(p.x)<1.78f && Mathf.Abs(p.y)<1.06f;
    private bool Walkable(Vector2 p)
    {
        if(p.x < -5.12f || p.x > 5.12f || p.y < -7.16f || p.y > 3.5f || InsideTableFootprint(p))return false;
        foreach(var prop in SantaRitaBarLayout.Props)
        {
            if(prop.kind==SantaRitaBarLayout.Kind.Wall || prop.kind==SantaRitaBarLayout.Kind.Decor || prop.kind==SantaRitaBarLayout.Kind.Chair)continue;
            var at=prop.id=="NPC_Ze_Interior" && opponent!=null?opponent.root.localPosition:Plan(prop.position-Vector2.up*prop.supportOffset);
            float radius=prop.kind==SantaRitaBarLayout.Kind.Actor?.38f:prop.width*.5f+.20f;
            // The counter has depth; chairs/tables use circular clearance.
            if(prop.kind==SantaRitaBarLayout.Kind.Counter)
            { if(Mathf.Abs(p.x-at.x)<radius && Mathf.Abs(p.y-at.z)<.65f)return false; }
            else if(Vector2.Distance(p,new Vector2(at.x,at.z))<radius)return false;
        }
        return true;
    }
    public void MovePlayer(Vector2 input, Vector2 look, float dt)
    {
        MoverJogador(Vector2.ClampMagnitude(input, 1f) * 1.65f, look, dt);
    }

    internal void MoverJogador(Vector2 velocidade, Vector2 look, float dt)
    {
        if(!Walking)return;
        var anterior = standingPosition;
        standingYaw+=look.x*.13f;standingPitch=Mathf.Clamp(standingPitch-look.y*.10f,-42,68);
        float radians=standingYaw*Mathf.Deg2Rad;
        float seno = Mathf.Sin(radians);
        float cosseno = Mathf.Cos(radians);
        Vector3 delta = new Vector3(velocidade.x * cosseno + velocidade.y * seno, 0f,
            velocidade.y * cosseno - velocidade.x * seno) * Mathf.Clamp(dt, 0f, .05f);
        // Small axis-separated sweeps slide along furniture without tunnelling.
        int steps=Mathf.Max(1,Mathf.CeilToInt(delta.magnitude/.035f));delta/=steps;
        for(int i=0;i<steps;i++)
        {
            if(Walkable(new Vector2(standingPosition.x+delta.x,standingPosition.z)))standingPosition.x+=delta.x;
            if(Walkable(new Vector2(standingPosition.x,standingPosition.z+delta.z)))standingPosition.z+=delta.z;
        }
        // Passo discreto só depois de percorrer chão real: olhar/parar/colidir não dispara.
        float percorrido = Vector3.Distance(anterior,standingPosition);
        if(percorrido<=.0001f){distanciaPasso=0;return;}
        distanciaPasso+=percorrido;
        if(distanciaPasso>=.62f)
        {
            distanciaPasso-=.62f;
            variantePasso=(variantePasso+1)%3;
            if(Application.isFocused && !MenuPausa.Aberto)
                SantaRitaSound.Play("footstep_wood_00"+variantePasso,.065f,variantePasso==1?.97f:1.03f,.15f);
        }
    }
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
        cue=BuildCue(dynamic);BuildHands();BuildOpponent();guide=new GameObject("MiraFina").AddComponent<LineRenderer>();guide.transform.SetParent(dynamic,false);guide.useWorldSpace=false;guide.positionCount=2;guide.startWidth=.006f;guide.endWidth=.002f;guide.numCapVertices=4;guide.sharedMaterial=Mat("mira",new Color(.82f,.88f,.78f),null,true);guide.shadowCastingMode=ShadowCastingMode.Off;guide.receiveShadows=false;
        objectGuide=new GameObject("DirecaoDaColorida").AddComponent<LineRenderer>();objectGuide.transform.SetParent(dynamic,false);objectGuide.useWorldSpace=false;objectGuide.positionCount=2;objectGuide.startWidth=.005f;objectGuide.endWidth=.0035f;objectGuide.sharedMaterial=Mat("direcaoObjeto",new Color(1f,.74f,.25f),null,true);objectGuide.shadowCastingMode=ShadowCastingMode.Off;
        guideBorder=NewGuide("ContrasteMira",.011f,Mat("sombraMira",new Color(.025f,.08f,.045f),null,true),2);
        objectArrow=NewGuide("SetaDaColorida",.005f,objectGuide.sharedMaterial,3);
        ghost=Ring("ContatoPrevisto",SinucaTableView.BallRadius*MetresPerUnit,.002f,new Color(.94f,.94f,.82f),dynamic,32).transform;
        for(int n=1;n<16;n++){var mark=new GameObject("MarcaGiz_"+n);mark.transform.SetParent(dynamic,false);var mat=Mat("gizDaMarcada",new Color(.9f,.92f,.65f),null,true);Tube("X1",new Vector3(-.016f,-.016f,0),new Vector3(.016f,.016f,0),.0025f,mat,mark.transform);Tube("X2",new Vector3(-.016f,.016f,0),new Vector3(.016f,-.016f,0),.0025f,mat,mark.transform);markedSigns[n]=mark.transform;mark.SetActive(false);}
        lampParts=scenery.GetComponentsInChildren<Transform>(true).Where(t=>t.name=="Cupula"||t.name=="FioLampada"||t.name=="Lampada").ToArray();
        foreach(var t in root.GetComponentsInChildren<Transform>(true))t.gameObject.layer=Layer;
        BuildFirstPersonLayer();
        wallLeft=scenery.Find("LambrilEsq").GetComponent<Renderer>();wallRight=scenery.Find("LambrilDir").GetComponent<Renderer>();wallBack=scenery.Find("LambrilFundo").GetComponent<Renderer>();
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
        m=new Material(Resources.Load<Material>("Art/BarRef7/"+(unlit?"BarUnlit":"BarLit"))){name=key};m.SetColor("_BaseColor",tint);m.color=tint;if(tex!=null){m.SetTexture("_BaseMap",tex);m.mainTexture=tex;}if(!unlit)m.shader=Resources.Load<Shader>("Art/Sinuca/SalaLit");m.enableInstancing=true;m.SetFloat("_Smoothness",key.Contains("madeira")?.30f:key=="feltro"?.025f:.15f);materials.Add(key,m);owned.Add(m);return m;
    }
    private Material Wood=>Mat("madeiraNativa",new Color(.64f,.54f,.41f),tiles[2]);
    private Material DarkWood=>Mat("madeiraEscuraNativa",new Color(.43f,.36f,.28f),tiles[2]);
    private GameObject Primitive(string name,PrimitiveType type,Vector3 at,Vector3 size,Material m,Transform parent)
    {var g=GameObject.CreatePrimitive(type);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=at;g.transform.localScale=size;UnityEngine.Object.Destroy(g.GetComponent<Collider>());g.GetComponent<MeshRenderer>().sharedMaterial=m;g.layer=Layer;return g;}
    private GameObject Box(string n,Vector3 p,Vector3 s,Material m,Transform t=null)=>Primitive(n,PrimitiveType.Cube,p,s,m,t??scenery);
    private GameObject Sphere(string n,Vector3 p,float s,Material m,Transform t)=>Primitive(n,PrimitiveType.Sphere,p,Vector3.one*s,m,t);
    private readonly Dictionary<int,Mesh> cylinderMeshes=new Dictionary<int,Mesh>();
    private GameObject Cylinder(string n,Vector3 p,float r,float h,Material m,Transform t=null)
    {
        int sides=n=="fio"||n=="MalhaRede"?6:16;
        if(!cylinderMeshes.TryGetValue(sides,out var mesh))
        {
            var vertices=new List<Vector3>();var normals=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
            for(int k=0;k<=sides;k++)
            {
                float a=k*Mathf.PI*2/sides;var normal=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));
                vertices.Add(normal*.5f-Vector3.up);vertices.Add(normal*.5f+Vector3.up);
                normals.Add(normal);normals.Add(normal);uv.Add(new Vector2(k/(float)sides,0));uv.Add(new Vector2(k/(float)sides,1));
                if(k<sides){int v=k*2;triangles.AddRange(new[]{v,v+1,v+3,v,v+3,v+2});}
            }
            foreach(int sign in new[]{-1,1})
            {
                int center=vertices.Count;vertices.Add(Vector3.up*sign);normals.Add(Vector3.up*sign);uv.Add(new Vector2(.5f,.5f));
                for(int k=0;k<=sides;k++){float a=k*Mathf.PI*2/sides;var point=new Vector3(Mathf.Cos(a)*.5f,sign,Mathf.Sin(a)*.5f);vertices.Add(point);normals.Add(Vector3.up*sign);uv.Add(new Vector2(point.x+.5f,point.z+.5f));}
                for(int k=0;k<sides;k++)if(sign>0)triangles.AddRange(new[]{center,center+k+2,center+k+1});else triangles.AddRange(new[]{center,center+k+1,center+k+2});
            }
            mesh=new Mesh{name="CilindroCompartilhado_"+sides};mesh.SetVertices(vertices);mesh.SetNormals(normals);mesh.SetUVs(0,uv);mesh.SetTriangles(triangles,0);mesh.RecalculateBounds();owned.Add(mesh);cylinderMeshes.Add(sides,mesh);
        }
        var go=new GameObject(n);go.transform.SetParent(t??scenery,false);go.transform.localPosition=p;go.transform.localScale=new Vector3(r*2,h*.5f,r*2);go.layer=Layer;
        go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=m;return go;
    }
    private GameObject Tube(string n,Vector3 a,Vector3 b,float r,Material m,Transform t=null){var g=Cylinder(n,(a+b)*.5f,r,Vector3.Distance(a,b),m,t);g.transform.localRotation=Quaternion.FromToRotation(Vector3.up,(b-a).normalized);return g;}
    private GameObject Ring(string n,float r,float wire,Color c,Transform t,int count=20)
    {var g=new GameObject(n);g.transform.SetParent(t,false);var m=Mat("corda_"+c.ToString(),c);for(int j=0;j<count;j++){float a=j*Mathf.PI*2/count,b=(j+1)*Mathf.PI*2/count;Tube("fio",new Vector3(Mathf.Cos(a)*r,0,Mathf.Sin(a)*r),new Vector3(Mathf.Cos(b)*r,0,Mathf.Sin(b)*r),wire,m,g.transform);}return g;}
    private void Panel(string n,Vector3 at,Vector2 size,Texture tex,Transform t=null,bool frame=true)
    {
        if(frame)Box(n+"_Moldura",at+new Vector3(0,0,.04f),new Vector3(size.x+.06f,size.y+.06f,.045f),DarkWood,t);
        var g=GameObject.CreatePrimitive(PrimitiveType.Quad);g.name=n;g.transform.SetParent(t??scenery,false);g.transform.localPosition=at;g.transform.localScale=new Vector3(size.x,size.y,1);UnityEngine.Object.Destroy(g.GetComponent<Collider>());g.GetComponent<MeshRenderer>().sharedMaterial=Mat(n+"_textura",Color.white,tex);g.GetComponent<MeshRenderer>().shadowCastingMode=ShadowCastingMode.Off;g.layer=Layer;
    }
    public static Vector3 Plan(Vector2 xy){Vector2 d=(xy-SantaRitaBarLayout.TableCenter)*1.15f;return new Vector3(d.x,0,d.y);}
    private Vector3 transitionPosition;
    private Quaternion transitionRotation;
    private float transitionStarted=-10;
    public void SetMode(SinucaCameraView value,float angle)
    {
        if(mode!=value && cameraInitialized){transitionPosition=camera.transform.localPosition;transitionRotation=camera.transform.localRotation;transitionStarted=Time.unscaledTime;}
        mode=value;cueHeading=angle;UpdateCamera();
    }
    public void Orbit(float degrees){if(mode==SinucaCameraView.LinhaTaco)cueHeading-=degrees*Mathf.Deg2Rad;else if(mode==SinucaCameraView.VistaSuperior)topYaw+=degrees;else yaw+=degrees;UpdateCamera();}
    private Vector2 cueContact;
    private float spinFocusFrom,spinFocusTo,spinFocusStarted=-10;
    private float SpinFocus => Mathf.Lerp(spinFocusFrom,spinFocusTo,Mathf.SmoothStep(0,1,(Time.unscaledTime-spinFocusStarted)/.2f));
    public void SetSpinFocus(bool active){spinFocusFrom=SpinFocus;spinFocusTo=active?1:0;spinFocusStarted=Time.unscaledTime;}
    public void SetCueContact(Vector2 value){cueContact=Vector2.ClampMagnitude(value,.75f);}
    public void SetZoom(float value){zoom=Mathf.Clamp(value,.8f,1.35f);UpdateCamera();}
    public void SetViewportAspect(float aspect)
    {
        if(aspect<=0)return;
        int width=Mathf.Clamp(Mathf.RoundToInt(720*aspect),640,2560);
        if(Target.width!=width){Target.Release();Target.width=width;Target.Create();}
        camera.aspect=(float)Target.width/Target.height;
    }
    public void Recenter(float angle){cueHeading=angle;UpdateCamera();}
    private void UpdateCamera()
    {
        camera.orthographic=mode==SinucaCameraView.VistaSuperior;
        if(Walking)
        {
            var rotation = Quaternion.Euler(standingPitch + startleLook.y,standingYaw + startleLook.x,0);
            var towardTable = new Vector3(-standingPosition.x, 0, -standingPosition.z).normalized;
            var position = standingPosition + entryLook * (Vector3.up * .03f - towardTable * .18f);
            var towardLamp = Quaternion.LookRotation(PendantPosition - position);
            camera.transform.localPosition = position;
            camera.transform.localRotation = Quaternion.Slerp(rotation, towardLamp, entryLook);
            camera.fieldOfView = Mathf.Lerp(62f, 55f, entryLook);
        }
        else if(camera.orthographic){camera.transform.localPosition=new Vector3(0,6,0);camera.transform.localRotation=Quaternion.Euler(90,topYaw,0);camera.orthographicSize=1.25f/zoom;}
        else if(mode==SinucaCameraView.Mesa3D){var target=new Vector3(0,1f,.5f);camera.transform.localPosition=target+Quaternion.Euler(0,yaw,0)*new Vector3(0,2f,-4.3f)/zoom;camera.transform.localRotation=Quaternion.LookRotation(target-camera.transform.localPosition);camera.fieldOfView=46;}
        else{var dir=new Vector3(Mathf.Cos(cueHeading),0,Mathf.Sin(cueHeading));var ball=new Vector3(white.x*MetresPerUnit,TableY,white.y*MetresPerUnit);camera.transform.localPosition=ball-dir*(1.65f/zoom)+Vector3.up*.53f;camera.transform.localRotation=Quaternion.LookRotation(ball+dir*.13f+Vector3.up*.015f-camera.transform.localPosition);camera.fieldOfView=48;}
        if(mode==SinucaCameraView.LinhaTaco)
        {
            float focus=SpinFocus;
            var centre=new Vector3(white.x*MetresPerUnit,TableY+SinucaTableView.BallRadius*MetresPerUnit,white.y*MetresPerUnit);
            camera.transform.localPosition=Vector3.Lerp(camera.transform.localPosition,centre+(camera.transform.localPosition-centre)*.82f,focus);
            camera.transform.localRotation=Quaternion.Slerp(camera.transform.localRotation,Quaternion.LookRotation(centre-camera.transform.localPosition),focus*.35f);
        }
        float transition=Mathf.SmoothStep(0,1,(Time.unscaledTime-transitionStarted)/.42f);
        if(cameraInitialized && transition<1){camera.transform.localPosition=Vector3.Lerp(transitionPosition,camera.transform.localPosition,transition);camera.transform.localRotation=Quaternion.Slerp(transitionRotation,camera.transform.localRotation,transition);}
        AplicarCameraInvestigacao();
        cameraInitialized=true;poseBlend=Walking?0:transition;
        // Cut only an intervening wall when inspecting from outside; inside cue views remain enclosed.
        if(lampParts!=null)foreach(var t in lampParts)t.gameObject.SetActive(mode!=SinucaCameraView.VistaSuperior);
        bool insideCeiling=camera.transform.localPosition.y<2.85f;
        if(ceilingRenderer!=null)ceilingRenderer.enabled=insideCeiling;
        if(ceilingBeams!=null)foreach(var beam in ceilingBeams)beam.enabled=insideCeiling;
        foreach(var detail in overheadDetails)detail.enabled=insideCeiling;
        OrientPixelArt();
        UpdateOpponentView();
        if(wallLeft!=null){var p=camera.transform.localPosition;wallLeft.enabled=p.x>-5.3f;wallRight.enabled=p.x<5.3f;wallBack.enabled=p.z<3.7f;}
    }
    public Vector2 Project(Vector3 sim){var p=root.transform.TransformPoint(new Vector3(sim.x*MetresPerUnit,TableY+sim.y*MetresPerUnit,sim.z*MetresPerUnit));return camera.WorldToViewportPoint(p);}
    public bool RayHitsWhite(Vector2 viewport,Vector2 ball)
    {
        float radius=SinucaTableView.BallRadius*MetresPerUnit;
        var centre=root.transform.TransformPoint(new Vector3(ball.x*MetresPerUnit,TableY+radius,ball.y*MetresPerUnit));
        var ray=camera.ViewportPointToRay(viewport);var delta=centre-ray.origin;
        float depth=Vector3.Dot(delta,ray.direction);
        return depth>0 && (delta-ray.direction*depth).sqrMagnitude<radius*radius*1.32f;
    }
    public bool RayToTable(Vector2 viewport,out Vector2 sim)
    {var ray=camera.ViewportPointToRay(viewport);var plane=new Plane(Vector3.up,root.transform.position+Vector3.up*(TableY+SinucaTableView.BallRadius*.4f*MetresPerUnit));if(plane.Raycast(ray,out float d)&&d>0){var p=root.transform.InverseTransformPoint(ray.GetPoint(d));sim=new Vector2(p.x,p.z)/MetresPerUnit;return true;}sim=Vector2.zero;return false;}
    public void Sync(IList<SinucaVisualBall> values,Vector2 cuePos,float angle,float power,bool showCue,SinucaAimGuideData g)
    {
        TickSalon();
        if(Walking||showCue)white=cuePos;if(mode==SinucaCameraView.LinhaTaco)cueHeading=angle;UpdateCamera();
        for(int i=0;i<values.Count;i++)
        {
            var b=values[i];int n=b.numero;if(n<0||n>15)continue;bool visible=b.ativa||b.caindo||b.quedaProgresso>=.99f;balls[n].gameObject.SetActive(visible);if(!visible)continue;var at=new Vector3(b.pos.x*MetresPerUnit,TableY+SinucaTableView.BallRadius*MetresPerUnit,b.pos.y*MetresPerUnit);
            if(b.caindo||!b.ativa){int pocket=0;float best=999;for(int j=0;j<6;j++){float d=Vector2.Distance(b.cacapaAlvo*MetresPerUnit,PocketPhysics(j));if(d<best){best=d;pocket=j;}}var p=PocketOuter(pocket);at=Vector3.Lerp(at,new Vector3(p.x,.56f,p.y),Mathf.SmoothStep(0,1,Mathf.Clamp01(b.quedaProgresso)));if(!wasFalling[n])impactTime[pocket]=Time.unscaledTime;}
            wasFalling[n]=b.caindo||!b.ativa;balls[n].localPosition=at;balls[n].localRotation=b.rotacao;
            if(n>0){markedSigns[n].gameObject.SetActive(b.marcada&&b.ativa);markedSigns[n].localPosition=at+Vector3.up*.065f;markedSigns[n].localRotation=camera.transform.localRotation;}
        }
        var dir=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle));
        float strokeAge=Time.time-strokeStarted;
        bool stroke=strokeAge>=0 && strokeAge<.24f;
        bool handsVisible=!InspectingUnderTable && (showCue||stroke) && mode==SinucaCameraView.LinhaTaco;
        bool carry=Walking && !InspectingUnderTable;
        cue.gameObject.SetActive(handsVisible||carry);
        float pull=power*.22f;
        var bridgeBall=stroke?new Vector2(strokeOrigin.x,strokeOrigin.z)/MetresPerUnit:cuePos;
        float railLift=BridgeRailLift(bridgeBall,new Vector2(dir.x,dir.z));
        var side=new Vector3(-dir.z,0,dir.x);
        var cueAxis=(Quaternion.AngleAxis(cueContact.x*18f,Vector3.up)*dir-Vector3.up*Mathf.Tan(Mathf.Abs(cueContact.y)*14f*Mathf.Deg2Rad)-Vector3.up*(railLift/.25f)).normalized;
        if(stroke)pull=Mathf.Lerp(strokePower*.22f,-.055f,Mathf.SmoothStep(0,1,strokeAge/.09f));
        if (handsVisible && !stroke) pull += GripShake;
        // Freeze the stroke at the contact origin, even as the physical white rolls away.
        if(!stroke)
        {
            float radius=SinucaTableView.BallRadius*MetresPerUnit;
            var centre=new Vector3(cuePos.x*MetresPerUnit,TableY+radius,cuePos.y*MetresPerUnit);
            var contact=centre+(side*cueContact.x+Vector3.up*cueContact.y-dir*Mathf.Sqrt(1-cueContact.sqrMagnitude))*radius;
            // The cue model's tip is 8.7 cm behind its origin; leave 4 cm of approach gap.
            strokeOrigin=contact+cueAxis*.047f;
        }
        cue.localPosition=strokeOrigin-cueAxis*pull+Vector3.down*((1-poseBlend)*.16f);
        cue.localRotation=Quaternion.LookRotation(cueAxis);
        bridgeAnchor.gameObject.SetActive(handsVisible);
        bridgeAnchor.localPosition=new Vector3(strokeOrigin.x,TableY+railLift,strokeOrigin.z)+Vector3.up*((1-poseBlend)*.12f);
        bridgeAnchor.localRotation=Quaternion.LookRotation(dir);
        if(carry)
        {
            cue.localPosition=camera.transform.localPosition+camera.transform.localRotation*new Vector3(.22f,.60f,.95f);
            cue.localRotation=camera.transform.localRotation*Quaternion.Euler(-55,-8,3);
        }
        // Both sprites retain their physical contact pivots but always show their full face.
        // World rotation cancels the cue parent's tilt; grip translation still follows pullback.
        AlignBridgeToShaft();
        AlignGripToShaft();
        if(gripHand!=null)gripHand.gameObject.SetActive(handsVisible);
        SyncFirstPersonCamera();

        // Keep the support planted while the grip moves with the cue's actual transform.
        // Show only the first 90 cm, preserving the real first collision.
        g.ativa = g.ativa && handsVisible && !stroke && !InspectingUnderTable;
        const float previewLength = .90f / MetresPerUnit;
        float contactDistance = Vector2.Distance(g.origem, g.pontoContato);
        bool contactVisible = g.atingeBola && contactDistance <= previewLength;
        g.pontoContato = g.origem + (g.pontoContato - g.origem).normalized * Mathf.Min(contactDistance, previewLength);
        g.atingeBola = contactVisible;

        guide.gameObject.SetActive(g.ativa);ghost.gameObject.SetActive(g.ativa && g.atingeBola);if(g.ativa){guide.SetPosition(0,new Vector3(g.origem.x*MetresPerUnit,TableY+.005f,g.origem.y*MetresPerUnit));guide.SetPosition(1,new Vector3(g.pontoContato.x*MetresPerUnit,TableY+.005f,g.pontoContato.y*MetresPerUnit));ghost.localPosition=new Vector3(g.pontoContato.x*MetresPerUnit,TableY+.005f,g.pontoContato.y*MetresPerUnit);}
        guideBorder.gameObject.SetActive(g.ativa);if(g.ativa){guideBorder.SetPosition(0,guide.GetPosition(0)-Vector3.up*.001f);guideBorder.SetPosition(1,guide.GetPosition(1)-Vector3.up*.001f);}
        objectGuide.gameObject.SetActive(g.ativa&&g.atingeBola);objectArrow.gameObject.SetActive(g.ativa&&g.atingeBola);
        if(g.ativa&&g.atingeBola)
        {
            var a=g.posBolaAlvo;var d=g.direcaoBolaAlvo.normalized;float length=.32f/MetresPerUnit;
            foreach(var other in values)
            {
                if(!other.ativa || other.numero==g.numeroBolaAlvo || other.numero==0)continue;
                Vector2 offset=other.pos-a;float along=Vector2.Dot(offset,d);float r=SinucaTableView.BallRadius*2;
                float sideSq=offset.sqrMagnitude-along*along;
                if(along>0 && sideSq<r*r)length=Mathf.Min(length,Mathf.Max(0,along-Mathf.Sqrt(r*r-sideSq)));
            }
            if(Mathf.Abs(d.x)>.001f)length=Mathf.Min(length,((d.x>0?204:-204)-a.x)/d.x);
            if(Mathf.Abs(d.y)>.001f)length=Mathf.Min(length,((d.y>0?99:-99)-a.y)/d.y);
            var b=a+d*Mathf.Max(0,length);var from=new Vector3(a.x*MetresPerUnit,TableY+.013f,a.y*MetresPerUnit);var tip=new Vector3(b.x*MetresPerUnit,TableY+.013f,b.y*MetresPerUnit);var objectDir=new Vector3(d.x,0,d.y);var arrowSide=new Vector3(-d.y,0,d.x);
            objectGuide.SetPosition(0,from);objectGuide.SetPosition(1,tip);objectArrow.SetPosition(0,tip-objectDir*.05f+arrowSide*.025f);objectArrow.SetPosition(1,tip);objectArrow.SetPosition(2,tip-objectDir*.05f-arrowSide*.025f);
        }
        for(int j=0;j<6;j++){float t=Time.unscaledTime-impactTime[j];float bounce=t<2?Mathf.Sin(t*22)*Mathf.Exp(-t*3)*.09f:0;nets[j].localScale=new Vector3(1,1+bounce,1);}
        AtualizarAtoresDoSalao();
    }

    private void AtualizarAtoresDoSalao()
    {
        foreach (var a in actors)
        {
            if (a.definition.kind != SantaRitaBarLayout.Kind.Actor)
            {
                a.renderer.enabled = SalonPropVisible(a.definition.id);
                continue;
            }
            if (a == opponent || UpdateSalonPatron(a)) continue;
            var sprite = a.seated ? SantaRitaBarLayout.PatronPose(a.definition.key, a.definition.id, Time.time) ?? a.rest : a.rest;
            a.filter.sharedMesh = NativeMesh(sprite);
            a.renderer.sharedMaterial = PixelMaterial(sprite);
        }
    }
    public void SetActive(bool value){if(root!=null)root.SetActive(value);}
    public void Dispose(){UnityEngine.Object.Destroy(root);foreach(var o in owned)if(o!=null)UnityEngine.Object.Destroy(o);}
}

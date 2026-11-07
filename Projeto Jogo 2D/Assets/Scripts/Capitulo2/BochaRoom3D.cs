using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Presentation only. Rules, heights and ball rotation come from the existing kernel.
public sealed partial class BochaRoom3D : IDisposable
{
    private const int Layer = 28;
    public readonly GameObject root;
    public readonly Camera camera;
    public readonly RenderTexture Target;
    public Vector3 StandingPosition { get; private set; } = new Vector3(0,1.62f,-1.7f);
    public float Yaw { get; private set; }
    public float Pitch { get; private set; } = 12;
    private readonly List<UnityEngine.Object> owned = new List<UnityEngine.Object>();
    private readonly List<Bounds> obstacles = new List<Bounds>();
    private readonly List<Renderer> roof = new List<Renderer>();
    private readonly List<Light> luzesDaSala = new List<Light>();
    private readonly Dictionary<string,Material> materials = new Dictionary<string,Material>();
    private readonly Transform[] balls = new Transform[10], contacts = new Transform[10];
    private readonly Queue<LineRenderer> tracks = new Queue<LineRenderer>();
    private readonly Vector3[] previous = new Vector3[10];
    private readonly bool[] seen = new bool[10];
    private readonly LineRenderer guide;
    private readonly Transform ghost;
    private BochaCameraView mode;
    private float stepDistance,prepareBlend,pullBlend;
    public void SetThrowPose(bool preparing,float pull,float dt)
    {
        float blend=1-Mathf.Exp(-Mathf.Min(dt,.05f)*10);
        prepareBlend=Mathf.Lerp(prepareBlend,preparing?1:0,blend);
        pullBlend=Mathf.Lerp(pullBlend,preparing?pull:0,blend);
    }
    // The overlay is in camera space. Sample the actual room fixtures near
    // the projected hand so its exposure follows the same warm/cold pools.
    public Color LuzDaMao(Vector3 local)
    {
        Vector3 p = camera.transform.TransformPoint(local);
        Color cor = new Color(.44f, .40f, .37f);
        foreach (var luz in luzesDaSala)
        {
            if (!luz.isActiveAndEnabled || luz.type == LightType.Directional || luz.range <= 0) continue;
            Vector3 d = p - luz.transform.position;
            float dist = d.magnitude;
            if (dist >= luz.range) continue;
            float alcance = Mathf.Clamp01(1 - dist / luz.range);
            float peso = luz.intensity * alcance * alcance / (1 + .12f * d.sqrMagnitude);
            if (luz.type == LightType.Spot)
            {
                float limite = Mathf.Cos(luz.spotAngle * .5f * Mathf.Deg2Rad);
                float c = Vector3.Dot(luz.transform.forward, d / Mathf.Max(.001f, dist));
                peso *= Mathf.SmoothStep(0, 1, Mathf.InverseLerp(limite, Mathf.Lerp(limite, 1, .45f), c));
            }
            cor += luz.color * Mathf.Min(peso * .58f, .30f);
        }
        return new Color(Mathf.Min(cor.r, .90f), Mathf.Min(cor.g, .86f), Mathf.Min(cor.b, .80f), 1);
    }
    public int TrackCount => tracks.Count;
    public BochaRoom3D()
    {
        root = new GameObject("SalaBocha3D_Persistente");
        UnityEngine.Object.DontDestroyOnLoad(root);
        camera = new GameObject("CameraBocha3D").AddComponent<Camera>();
        camera.transform.SetParent(root.transform,false);camera.cullingMask=1<<Layer;
        camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.018f,.024f,.029f);
        camera.nearClipPlane=.04f;camera.farClipPlane=55;camera.depth=-11;camera.allowHDR=false;
        var data=camera.gameObject.AddComponent<UniversalAdditionalCameraData>();
        data.SetRenderer(1);data.renderPostProcessing=false;data.renderShadows=true;data.volumeLayerMask=0;
        Target=new RenderTexture(1280,720,24,RenderTextureFormat.ARGB32){name="Bocha_Offscreen",antiAliasing=2};
        Target.Create();camera.targetTexture=Target;
        BuildRoom();
        var shadow=new Material(Resources.Load<Shader>("Art/Sinuca/ContatoSala"));owned.Add(shadow);shadow.SetFloat("_Opacity",.40f);
        ConstruirTonao(shadow);
        for(int i=0;i<balls.Length;i++)
        {
            balls[i]=Primitive("Bocha_"+i,PrimitiveType.Sphere,Vector3.zero,Vector3.one,Mat("bola"+i,BochaCourtView.BallColor(i==0?BochaSide.Nenhum:i<5?BochaSide.Marcus:BochaSide.Tonao))).transform;
            contacts[i]=Primitive("Contato_"+i,PrimitiveType.Quad,Vector3.zero,Vector3.one,shadow).transform;
            contacts[i].localRotation=Quaternion.Euler(90,0,0);contacts[i].GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.Off;
            balls[i].gameObject.SetActive(false);contacts[i].gameObject.SetActive(false);
        }
        guide=Line("MiraNoSaibro",.018f,Mat("giz",new Color(.73f,.69f,.46f),null,true));
        ghost=Primitive("BochaNaBase",PrimitiveType.Sphere,new Vector3(0,.18f,.65f),Vector3.one*.36f,Mat("bochaMarcus",BochaCourtView.BallColor(BochaSide.Marcus))).transform;
        ResetWalking();
    }
    private Material Mat(string name,Color color,Texture texture=null,bool unlit=false)
    {
        if(materials.TryGetValue(name,out var found))return found;
        var m=new Material(Resources.Load<Material>("Art/BarRef7/"+(unlit?"BarUnlit":"BarLit"))){name="Bocha_"+name};
        if(!unlit)m.shader=Resources.Load<Shader>("Art/Sinuca/SalaLit");
        m.SetColor("_BaseColor",color);if(texture!=null)m.SetTexture("_BaseMap",texture);
        m.SetFloat("_Smoothness",name.StartsWith("bola")?.32f:.07f);m.enableInstancing=true;
        materials.Add(name,m);owned.Add(m);return m;
    }
    private GameObject Primitive(string name,PrimitiveType type,Vector3 at,Vector3 size,Material material)
    {
        var g=GameObject.CreatePrimitive(type);g.name=name;g.layer=Layer;g.transform.SetParent(root.transform,false);
        g.transform.localPosition=at;g.transform.localScale=size;
        UnityEngine.Object.Destroy(g.GetComponent<Collider>());g.GetComponent<Renderer>().sharedMaterial=material;return g;
    }
    private GameObject Box(string name,Vector3 at,Vector3 size,Material material,bool solid=false,bool ceiling=false)
    {
        var g=Primitive(name,PrimitiveType.Cube,at,size,material);
        if(solid)obstacles.Add(new Bounds(at,size+new Vector3(.48f,0,.48f)));
        if(ceiling)roof.Add(g.GetComponent<Renderer>());return g;
    }
    private void BuildRoom()
    {
        var wood=Mat("madeira",new Color(.53f,.43f,.31f),Resources.Load<Texture2D>("Bocha/MadeiraCancha"));
        var dark=Mat("estrutura",new Color(.21f,.24f,.21f));
        var plaster=Mat("reboco",new Color(.57f,.55f,.44f));
        plaster.SetFloat("_RoomSurface",1);plaster.SetTextureScale("_BaseMap",new Vector2(12,1));
        var floor=Mat("assoalho",new Color(.52f,.46f,.36f),Resources.Load<Texture2D>("Bocha/MadeiraCancha"));floor.SetTextureScale("_BaseMap",new Vector2(8,20));
        var sand=Mat("saibro",new Color(.78f,.64f,.43f),Resources.Load<Texture2D>("Bocha/SaibroCancha"));sand.SetTextureScale("_BaseMap",new Vector2(6,20));
        Box("PisoSala",new Vector3(0,-.09f,9.5f),new Vector3(11.6f,.16f,25.4f),floor);
        Box("Saibro",new Vector3(0,-.015f,10),new Vector3(6,.025f,20),sand);
        foreach(float x in new[]{-3.13f,3.13f})
        {
            Box("GuiaLateral",new Vector3(x,.18f,10),new Vector3(.26f,.36f,20),wood,true);
            Box("AcabamentoGuia",new Vector3(x,.368f,10),new Vector3(.29f,.025f,20),wood);
        }
        Box("CabeceiraBase",new Vector3(0,.13f,-.13f),new Vector3(6.5f,.26f,.26f),wood);
        Box("CabeceiraFundo",new Vector3(0,.24f,20.13f),new Vector3(6.5f,.48f,.26f),wood,true);
        Box("LinhaLancamento",new Vector3(0,.002f,1.1f),new Vector3(5.96f,.006f,.03f),Mat("linha",new Color(.58f,.51f,.34f)));
        foreach(float x in new[]{-5.8f,5.8f})
        {
            Box("ParedeLateral",new Vector3(x,2.15f,9.5f),new Vector3(.18f,4.3f,25.4f),plaster);
            Box("LambrilLateral",new Vector3(x*.982f,.58f,9.5f),new Vector3(.12f,1.16f,25.4f),wood);
            Box("Rodape",new Vector3(x*.967f,.075f,9.5f),new Vector3(.09f,.15f,25.4f),dark);
        }
        foreach(float z in new[]{-3.2f,22.2f})
        {
            Box("ParedeCabeceira",new Vector3(0,2.15f,z),new Vector3(11.6f,4.3f,.18f),plaster);
            Box("LambrilCabeceira",new Vector3(0,.58f,z>0?22.08f:-3.08f),new Vector3(11.6f,1.16f,.1f),wood);
        }
        Box("Forro",new Vector3(0,4.32f,9.5f),new Vector3(11.6f,.15f,25.4f),dark,false,true);
        foreach(float z in new[]{2f,10f,18f})
        {
            Box("Travessa",new Vector3(0,4.08f,z),new Vector3(11.6f,.26f,.22f),wood,false,true);
            foreach(float x in new[]{-5.55f,5.55f})Box("Pilar",new Vector3(x,2,z),new Vector3(.23f,4,.23f),wood,true);
        }
        // One visible warm fixture; three hidden bounces follow the same pool of light.
        Box("Suspensao",new Vector3(0,3.85f,10),new Vector3(.025f,.65f,.025f),dark,false,true);
        Box("LuminariaCentral",new Vector3(0,3.51f,10),new Vector3(.85f,.16f,1.8f),dark,false,true);
        Box("DifusorQuente",new Vector3(0,3.42f,10),new Vector3(.73f,.025f,1.62f),Mat("lampada",new Color(1,.76f,.40f),null,true),false,true);
        Light("AmbienteFrio",LightType.Directional,Vector3.zero,new Color(.45f,.53f,.66f),.075f,0).transform.localRotation=Quaternion.Euler(52,-30,0);
        var key=Light("LuzPrincipalCancha",LightType.Spot,new Vector3(0,3.32f,10),new Color(1,.80f,.55f),8.5f,24);
        key.transform.localRotation=Quaternion.Euler(90,0,0);key.spotAngle=158;key.innerSpotAngle=115;
        key.shadows=LightShadows.Hard;key.shadowBias=.012f;key.shadowNormalBias=.025f;key.shadowStrength=.85f;
        key.gameObject.AddComponent<UniversalAdditionalLightData>().additionalLightsShadowResolutionTier=1;
        foreach(float z in new[]{3f,10f,17f})Light("RebatimentoCancha",LightType.Point,new Vector3(0,2.2f,z),new Color(1,.74f,.46f),.65f,8);
        foreach(float x in new[]{-4.65f,4.65f})foreach(float z in new[]{5f,11f,17f})
        {
            Box("BancoAssento",new Vector3(x,.46f,z),new Vector3(.65f,.13f,2.65f),wood,true);
            Box("BancoEncosto",new Vector3(x+Mathf.Sign(x)*.27f,.83f,z),new Vector3(.1f,.66f,2.65f),wood);
            foreach(float dz in new[]{-.95f,.95f})Box("PeBanco",new Vector3(x,.22f,z+dz),new Vector3(.5f,.44f,.1f),dark);
        }
        foreach(float x in new[]{-2.6f,2.6f})
        {
            Box("JanelaMoldura",new Vector3(x,2.2f,22.02f),new Vector3(1.45f,1.3f,.15f),wood);
            Box("VidroNoite",new Vector3(x,2.2f,21.93f),new Vector3(1.24f,1.1f,.03f),Mat("vidro",new Color(.035f,.075f,.095f),null,true));
            Box("TravessaJanela",new Vector3(x,2.2f,21.90f),new Vector3(1.24f,.065f,.04f),wood);
            Box("MontanteJanela",new Vector3(x,2.2f,21.89f),new Vector3(.065f,1.1f,.04f),wood);
            Light("ReflexoNoite",LightType.Point,new Vector3(x,2,21),new Color(.30f,.43f,.61f),.24f,4);
        }
        Box("PortaSalao",new Vector3(-4.35f,1.1f,-3.03f),new Vector3(1.15f,2.2f,.12f),wood);
        Box("Macaneta",new Vector3(-3.94f,1.02f,-2.94f),new Vector3(.09f,.05f,.1f),Mat("metal",new Color(.50f,.45f,.27f)));
        Box("RackBochas",new Vector3(4.6f,.85f,-1.65f),new Vector3(1.25f,1.7f,.35f),dark,true);
        for(int row=0;row<3;row++)
        {
            Box("Prateleira",new Vector3(4.6f,.35f+row*.49f,-1.39f),new Vector3(1.25f,.07f,.48f),wood);
            for(int i=0;i<3;i++)Primitive("BochaReserva",PrimitiveType.Sphere,new Vector3(4.23f+i*.37f,.56f+row*.49f,-1.38f),Vector3.one*.32f,Mat("reserva"+row,BochaCourtView.BallColor(row%2==0?BochaSide.Marcus:BochaSide.Tonao)));
        }
        Box("QuadroPlacar",new Vector3(-5.66f,1.9f,1),new Vector3(.1f,1.15f,1.6f),wood);
        Box("Lousa",new Vector3(-5.59f,1.9f,1),new Vector3(.025f,.94f,1.38f),Mat("lousa",new Color(.06f,.12f,.10f)));
        BuildAtmosphere();
    }
    private Light Light(string name,LightType type,Vector3 at,Color color,float intensity,float range)
    {
        var l=new GameObject(name).AddComponent<Light>();l.transform.SetParent(root.transform,false);l.transform.localPosition=at;
        l.type=type;l.color=color;l.intensity=intensity;l.range=range;l.cullingMask=1<<Layer;l.shadows=LightShadows.None;luzesDaSala.Add(l);return l;
    }
    private LineRenderer Line(string name,float width,Material material)
    {
        var g=new GameObject(name);g.layer=Layer;g.transform.SetParent(root.transform,false);
        var l=g.AddComponent<LineRenderer>();l.useWorldSpace=false;l.positionCount=2;l.startWidth=l.endWidth=width;
        l.sharedMaterial=material;l.shadowCastingMode=ShadowCastingMode.Off;l.receiveShadows=false;return l;
    }
    public void ResetWalking(){StandingPosition=new Vector3(0,1.62f,-1.7f);Yaw=0;Pitch=12;stepDistance=0;}
    public bool CanStand(Vector2 p)
    {
        if(p.x< -5.32f||p.x>5.32f||p.y< -2.76f||p.y>21.7f)return false;
        foreach(var b in obstacles)if(p.x>b.min.x&&p.x<b.max.x&&p.y>b.min.z&&p.y<b.max.z)return false;
        return true;
    }
    public bool AtLaunch => StandingPosition.z<1.15f&&Mathf.Abs(StandingPosition.x)<2.7f;
    public void Walk(Vector2 axis,Vector2 look,float dt,bool fast)
    {
        Yaw=Mathf.Repeat(Yaw+look.x*.105f,360);Pitch=Mathf.Clamp(Pitch-look.y*.105f,-68,72);
        var turn=Quaternion.Euler(0,Yaw,0);var motion=turn*new Vector3(axis.x,0,axis.y);
        motion=Vector3.ClampMagnitude(motion,1)*(fast?3.3f:2.15f)*Mathf.Min(dt,.05f);
        // Substeps plus axis sliding prevent crossing a thin guide at low frame rate.
        int count=Mathf.Max(1,Mathf.CeilToInt(motion.magnitude/.07f));var start=StandingPosition;
        for(int i=0;i<count;i++)
        {
            var p=StandingPosition;float dx=motion.x/count,dz=motion.z/count;
            if(CanStand(new Vector2(p.x+dx,p.z)))p.x+=dx;
            if(CanStand(new Vector2(p.x,p.z+dz)))p.z+=dz;StandingPosition=p;
        }
        stepDistance+=Vector3.Distance(start,StandingPosition);
    }
    public void UpdateCamera(BochaCameraView view,float x,float zoom,BochaSimulation sim,float aspect)
    {
        mode=view;camera.aspect=aspect;camera.fieldOfView=62/zoom;
        foreach(var r in roof)r.enabled=view!=BochaCameraView.Superior;
        camera.orthographic=view==BochaCameraView.Superior;
        if(camera.orthographic)
        {camera.orthographicSize=Mathf.Max(7,13.3f/aspect);camera.transform.SetPositionAndRotation(new Vector3(0,27,10),Quaternion.LookRotation(Vector3.down,Vector3.left));}
        else if(view==BochaCameraView.Caminhando)
        {camera.transform.SetPositionAndRotation(StandingPosition+Vector3.up*(Mathf.Sin(stepDistance*8)*.012f-.28f*prepareBlend),Quaternion.Euler(Mathf.Lerp(Pitch,19,prepareBlend),Yaw,-2.5f*prepareBlend));}
        else if(view==BochaCameraView.Alvo)
        {
            var p=sim!=null&&sim.bolas.Count>0?sim.bolas[0].Centro:new Vector3(0,0,13);
            camera.transform.position=p+new Vector3(4.4f,4.5f,-5.4f);camera.transform.LookAt(p);
        }
        else
        {
            var lean=Quaternion.Euler(0,Yaw,0)*new Vector3(0,-.28f*prepareBlend,.18f*prepareBlend-.055f*pullBlend);
            camera.transform.position=StandingPosition+lean;
            camera.transform.rotation=Quaternion.Euler(Mathf.Lerp(Pitch,19,prepareBlend),Yaw,-2.5f*prepareBlend);
        }
        AtualizarSpriteTonao();
    }
    public void Sync(BochaSimulation sim,BochaShot shot,bool aim,float distance)
    {
        for(int i=0;i<balls.Length;i++)
        {
            var b=sim!=null&&i<sim.bolas.Count?sim.bolas[i]:null;bool active=b!=null&&b.valida;
            balls[i].gameObject.SetActive(active);contacts[i].gameObject.SetActive(active);if(!active)continue;
            balls[i].localPosition=b.Centro;balls[i].localRotation=b.rotacao;balls[i].localScale=Vector3.one*b.raio*2;
            balls[i].GetComponent<Renderer>().sharedMaterial=Mat("lado"+b.lado,BochaCourtView.BallColor(b.lado));
            contacts[i].localPosition=new Vector3(b.pos.x,.007f,b.pos.y);contacts[i].localScale=Vector3.one*b.raio*(2.8f+b.altura*.4f);
            AplicarQueda(b, balls[i], contacts[i]);
        }
        bool show=aim&&mode==BochaCameraView.Lancamento;guide.enabled=show;ghost.gameObject.SetActive(false);
        if(show)
        {
            var d=shot.velocidade.normalized;var start=new Vector3(shot.x,.018f,.65f);float len=distance;
            if(Mathf.Abs(d.x)>.001f)len=Mathf.Min(len,((d.x>0?2.8f:-2.8f)-shot.x)/d.x);
            guide.SetPosition(0,start);guide.SetPosition(1,start+new Vector3(d.x,0,d.y)*len);
            ghost.localPosition=new Vector3(shot.x,.18f+shot.altura,.65f);
        }
    }
    public void RecordContacts(BochaSimulation sim)
    {
        if(sim==null)return;
        foreach(var b in sim.bolas)
        {
            if(!b.valida||b.id<0||b.id>=seen.Length)continue;var p=b.Centro;
            if(seen[b.id]&&b.altura<.015f&&previous[b.id].y-b.raio<.015f&&Vector3.Distance(p,previous[b.id])>.055f)
            {
                var line=tracks.Count<480?Line("RastroSaibro",.018f,Mat("rastro",new Color(.39f,.30f,.19f))):tracks.Dequeue();
                line.SetPosition(0,new Vector3(previous[b.id].x,.006f,previous[b.id].z));line.SetPosition(1,new Vector3(p.x,.006f,p.z));tracks.Enqueue(line);previous[b.id]=p;
            }
            else if(!seen[b.id]||b.altura>=.015f||previous[b.id].y-b.raio>=.015f)previous[b.id]=p;
            seen[b.id]=true;
        }
    }
    public void ClearTracks(){Array.Clear(seen,0,seen.Length);while(tracks.Count>0)UnityEngine.Object.Destroy(tracks.Dequeue().gameObject);}
public void SetActive(bool active){if(root!=null)root.SetActive(active);}
    public void Dispose(){Target.Release();UnityEngine.Object.Destroy(Target);UnityEngine.Object.Destroy(root);foreach(var item in owned)UnityEngine.Object.Destroy(item);}


    private void BuildAtmosphere()
    {
        // Side bays: spectators, club notices, equipment and cleaning supplies.
        // Keep all furniture behind the existing benches and clear of the launch apron.
        var timber=materials["madeira"];
        var iron=materials["estrutura"];
        var plaster=materials["reboco"];
        var masonry=Mat("alvenaria",new Color(.38f,.39f,.34f),Resources.Load<Texture2D>("Art/Tiles/Pedra"));
        var brass=Mat("lataEnvelhecida",new Color(.42f,.32f,.16f));
        var paper=Mat("papelEnvelhecido",new Color(.67f,.61f,.44f));
        var green=Mat("armarioVerde",new Color(.18f,.24f,.20f));
        var bulb=Mat("arandelaAcesa",new Color(.84f,.52f,.23f),null,true);

        foreach(float x in new[]{-5.53f,5.53f})
        {
            float side=Mathf.Sign(x);
            Box("FrisoLambril",new Vector3(x,1.19f,9.5f),new Vector3(.17f,.09f,25.3f),timber);
            Box("Cornija",new Vector3(x,3.85f,9.5f),new Vector3(.32f,.18f,25.3f),timber,false,true);
            foreach(float z in new[]{-.65f,6.8f,13.8f,20.5f})
            {
                Box("PilastraAlvenaria",new Vector3(x,1.91f,z),new Vector3(.40f,3.82f,.46f),masonry,true);
                Box("BasePilastra",new Vector3(x,.20f,z),new Vector3(.53f,.40f,.61f),masonry);
                Box("Capitel",new Vector3(x,3.65f,z),new Vector3(.60f,.22f,.67f),timber,false,true);
                var brace=Box("MaoFrancesa",new Vector3(x-side*.59f,3.57f,z),new Vector3(1.65f,.15f,.20f),timber,false,true);
                brace.transform.localRotation=Quaternion.Euler(0,0,side*33);
            }
            // Individually visible bulbs explain the modest pools along the walls.
            foreach(float z in new[]{2.8f,10.1f,18.4f})
            {
                Box("ArandelaBase",new Vector3(x-side*.12f,2.47f,z),new Vector3(.10f,.42f,.23f),iron);
                Box("ArandelaBraco",new Vector3(x-side*.31f,2.41f,z),new Vector3(.43f,.055f,.055f),brass);
                Primitive("ArandelaBulbo",PrimitiveType.Sphere,new Vector3(x-side*.52f,2.53f,z),new Vector3(.12f,.24f,.12f),bulb);
                Box("ArandelaChapeu",new Vector3(x-side*.52f,2.72f,z),new Vector3(.38f,.065f,.38f),iron);
                Light("LuzArandela",LightType.Point,new Vector3(x-side*.62f,2.50f,z),new Color(1,.68f,.38f),1.65f,6.4f);
            }
            // Exposed conduit joins the fixtures, with short drops to each fitting.
            Box("EletrodutoLateral",new Vector3(x-side*.11f,3.15f,10.5f),new Vector3(.035f,.035f,22.6f),iron);
            foreach(float z in new[]{2.8f,10.1f,18.4f})
                Box("DescidaEletroduto",new Vector3(x-side*.14f,2.95f,z),new Vector3(.035f,.4f,.035f),iron);
        }
        foreach(float z in new[]{3f,17f})
        {
            Box("PendenteSecundarioCabo",new Vector3(0,3.92f,z),new Vector3(.025f,.54f,.025f),iron,false,true);
            Box("PendenteSecundario",new Vector3(0,3.65f,z),new Vector3(.58f,.13f,1.05f),iron,false,true);
            Box("PendenteSecundarioDifusor",new Vector3(0,3.57f,z),new Vector3(.48f,.025f,.91f),bulb,false,true);
            var pool=Light("LuzApoioCancha",LightType.Spot,new Vector3(0,3.48f,z),new Color(1,.78f,.51f),2.2f,10);
            pool.transform.localRotation=Quaternion.Euler(90,0,0);pool.spotAngle=112;pool.innerSpotAngle=75;
        }
        // Lower entrance ceiling and a framed equipment bay break the long box.
        Box("SancaEntrada",new Vector3(0,3.92f,-1.65f),new Vector3(11.2f,.48f,2.9f),plaster,false,true);
        Box("VigaEntrada",new Vector3(0,3.63f,-.23f),new Vector3(11.2f,.18f,.20f),timber,false,true);
        Box("AlcovaEquipamentos",new Vector3(5.1f,1.43f,-2.5f),new Vector3(.28f,2.86f,1.1f),masonry,true);
        Box("VergaEquipamentos",new Vector3(4.35f,2.88f,-2.5f),new Vector3(1.8f,.22f,1.1f),timber);
        Light("LuzEquipamentos",LightType.Point,new Vector3(4.2f,2.5f,-1.4f),new Color(.88f,.65f,.38f),.9f,4.3f);

        // Club noticeboard with overlapping, faded notices, away from the court.
        Box("MuralMoldura",new Vector3(5.62f,2.04f,8f),new Vector3(.10f,1.22f,1.85f),timber);
        Box("MuralCortica",new Vector3(5.55f,2.04f,8f),new Vector3(.035f,1.03f,1.65f),Mat("cortica",new Color(.35f,.24f,.13f)));
        for(int i=0;i<5;i++)
        {
            float z=7.43f+i*.27f,y=1.87f+(i%2)*.29f;
            var notice=Box("AvisoClube",new Vector3(5.52f,y,z),new Vector3(.016f,.36f,.24f),paper);
            notice.transform.localRotation=Quaternion.Euler((i-2)*4,0,0);
            for(int row=0;row<3;row++)Box("TintaAviso",new Vector3(5.508f,y+.07f-row*.065f,z),new Vector3(.005f,.012f,.15f-row*.02f),iron);
            Primitive("PregoAviso",PrimitiveType.Sphere,new Vector3(5.50f,y+.13f,z),Vector3.one*.024f,brass);
        }
        // Long-wall framed pennants and a clock; muted club colors, no new lore.
        foreach(float z in new[]{7.3f,14.2f})
        {
            Box("QuadroMemoriaClube",new Vector3(-5.61f,2.30f,z),new Vector3(.10f,.84f,1.05f),timber);
            Box("FundoQuadro",new Vector3(-5.54f,2.30f,z),new Vector3(.025f,.69f,.89f),paper);
            Box("FlamulaClube",new Vector3(-5.52f,2.35f,z),new Vector3(.018f,.43f,.28f),green);
            Box("FaixaFlamula",new Vector3(-5.505f,2.35f,z),new Vector3(.008f,.055f,.28f),brass);
        }
        var clock=Primitive("RelogioParede",PrimitiveType.Cylinder,new Vector3(0,2.72f,21.98f),new Vector3(.66f,.065f,.66f),iron);
        clock.transform.localRotation=Quaternion.Euler(90,0,0);
        var dial=Primitive("MostradorRelogio",PrimitiveType.Cylinder,new Vector3(0,2.72f,21.90f),new Vector3(.57f,.012f,.57f),paper);
        dial.transform.localRotation=Quaternion.Euler(90,0,0);
        Box("PonteiroRelogioMinuto",new Vector3(0,2.81f,21.88f),new Vector3(.025f,.19f,.015f),iron);
        var hand=Box("PonteiroRelogioHora",new Vector3(.065f,2.70f,21.86f),new Vector3(.15f,.028f,.015f),iron);
        hand.transform.localRotation=Quaternion.Euler(0,0,-20);

        // Storage at the far end, rather than random clutter in the walkways.
        Box("ArmarioManutencao",new Vector3(4.85f,.97f,21.47f),new Vector3(1.15f,1.94f,.68f),green,true);
        foreach(float x in new[]{4.57f,5.13f})
        {
            Box("PortaArmario",new Vector3(x,.98f,21.10f),new Vector3(.53f,1.80f,.045f),green);
            Box("PuxadorArmario",new Vector3(x+ (x<4.8f?.16f:-.16f),1.05f,21.06f),new Vector3(.035f,.17f,.035f),brass);
            for(int i=0;i<4;i++)Box("VentilacaoArmario",new Vector3(x,1.57f-i*.055f,21.065f),new Vector3(.30f,.018f,.012f),iron);
        }
        Box("CaixaManutencao",new Vector3(4.85f,2.10f,21.47f),new Vector3(.75f,.30f,.48f),timber);
        Box("AparadorFundo",new Vector3(-4.85f,.80f,21.1f),new Vector3(1.25f,.12f,.70f),timber,true);
        foreach(float x in new[]{-5.35f,-4.35f})Box("PeAparador",new Vector3(x,.39f,21.1f),new Vector3(.10f,.78f,.55f),iron);
        Primitive("GarrafaTermica",PrimitiveType.Cylinder,new Vector3(-4.90f,1.08f,21.1f),new Vector3(.17f,.22f,.17f),brass);
        Primitive("TampaTermica",PrimitiveType.Cylinder,new Vector3(-4.90f,1.32f,21.1f),new Vector3(.12f,.025f,.12f),iron);
        Primitive("Caneca",PrimitiveType.Cylinder,new Vector3(-4.53f,.96f,21.03f),new Vector3(.14f,.08f,.14f),paper);
        var broom=Box("CaboVassoura",new Vector3(-5.30f,.83f,19.7f),new Vector3(.045f,1.6f,.045f),timber);
        broom.transform.localRotation=Quaternion.Euler(-9,0,12);
        Box("EscovaSaibro",new Vector3(-5.14f,.10f,19.82f),new Vector3(.44f,.16f,.16f),Mat("cerdas",new Color(.33f,.26f,.13f)));
        // Cool light remains confined to the rear windows; warm court is dominant.
        Light("RebatimentoFundo",LightType.Point,new Vector3(0,2.4f,20.9f),new Color(.40f,.50f,.63f),.65f,5.5f);
    }
}

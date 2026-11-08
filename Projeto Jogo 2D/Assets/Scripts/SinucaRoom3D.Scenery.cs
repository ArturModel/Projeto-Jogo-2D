using UnityEngine;
public sealed partial class SinucaRoom3D
{
    private static readonly Vector3 PendantPosition = new Vector3(0,2.55f,0);

    private void BuildRoom()
    {
        var floor=Mat("assoalhoNativo",Color.white,tiles[0]);floor.mainTextureScale=new Vector2(9.5f,9.85f);
        Box("AssoalhoContinuo",new Vector3(0,-.025f,-1.8f),new Vector3(11,.05f,11.4f),floor);
        var wall=Mat("paredeNativa",Color.white,tiles[1]);wall.mainTextureScale=new Vector2(9.5f,1);
        Box("LambrilFundo",new Vector3(0,1.5f,3.86f),new Vector3(11,3f,.08f),wall);
        var side=Mat("paredeLateralNativa",Color.white,tiles[1]);side.mainTextureScale=new Vector2(9.85f,1);
        Box("LambrilEsq",new Vector3(-5.46f,1.5f,-1.8f),new Vector3(.08f,3f,11.4f),side);
        Box("LambrilDir",new Vector3(5.46f,1.5f,-1.8f),new Vector3(.08f,3f,11.4f),side);
        ConstruirParedeDaEntrada("LambrilFrente", 1.5f, 3f, 11f, -7.5f, .08f, wall);
        var rebocoDaRua = Mat("rebocoDaFachada", new Color(.52f,.42f,.32f));
        var barraDaRua = Mat("barraAzulDaFachada", new Color(.19f,.29f,.32f));
        ConstruirParedeDaEntrada("FachadaReboco", 1.91f, 2.18f, 11f, -7.55f, .03f, rebocoDaRua);
        ConstruirParedeDaEntrada("FachadaBarraAzul", .41f, .82f, 11f, -7.55f, .03f, barraDaRua);
        BuildRoomArchitecture();
        foreach(var p in SantaRitaBarLayout.Props)
        {
            // Occupied seats are already drawn in the seated character sprites.
            if(p.kind==SantaRitaBarLayout.Kind.Chair)continue;
            if(p.id=="Mesa_Sinuca_Central")continue;
            if(SantaRitaBarLayout.SpriteFor(p)==null)throw new System.InvalidOperationException("Sprite nativo ausente: "+p.key);
            CaptureArt(p.id,()=>
            {
                if(BuildSolidProp(p))return;
                var a=Character(p);
                if (p.id == "NPC_Frequentador_Interior") freguesDoBar = a.root;
                if(p.kind==SantaRitaBarLayout.Kind.Actor||p.id=="CuiaMesa"||p.id=="TermicaMesa")actors.Add(a);
                if(p.id=="TapeteSinuca")a.root.localRotation=Quaternion.Euler(90,0,0);
            });
        }
        BuildSalonDepth();
        var sun=new GameObject("LuzAmbiente3D").AddComponent<Light>();sun.transform.SetParent(root.transform,false);sun.type=LightType.Directional;sun.color=new Color(.57f,.65f,.77f);sun.intensity=.10f;sun.transform.localRotation=Quaternion.Euler(48,-32,0);sun.cullingMask=1<<Layer;sun.shadows=LightShadows.None;sun.shadowBias=.02f;
        Lamp(PendantPosition);
        // Low-cost indirect spill: no shadow cubemaps, fog, or screen-space effects.
        RoomFill("ReflexoQuenteMesa",new Vector3(0,1.75f,0),new Color(1f,.78f,.51f),.18f,3.1f);
        RoomFill("LuzDiscretaBalcao",new Vector3(-2.1f,2.4f,3.1f),new Color(1f,.74f,.47f),1.25f,4.3f);
        RoomFill("ReflexoFrioSala",new Vector3(3.8f,1.9f,-3.8f),new Color(.39f,.52f,.69f),.22f,4.5f);
        RoomFill("RebatimentoConversa",new Vector3(-3.3f,2.45f,-3.0f),new Color(1f,.80f,.58f),.68f,3.7f);
        RoomFill("RebatimentoChimarrao",new Vector3(-3.4f,2.4f,1.1f),new Color(1f,.80f,.58f),.58f,3.4f);
    }
    private void RoomFill(string name,Vector3 at,Color color,float intensity,float range)
    {
        var light=new GameObject(name).AddComponent<Light>();light.transform.SetParent(root.transform,false);
        light.transform.localPosition=at;light.type=LightType.Point;light.color=color;
        light.intensity=intensity;light.range=range;light.cullingMask=1<<Layer;light.shadows=LightShadows.None;
    }
    private void Lamp(Vector3 at)
    {
        var shade=Mat("cupulaVerde",new Color(.09f,.18f,.10f));Cylinder("Cupula",at,.23f,.055f,shade);Cylinder("FioLampada",at+Vector3.up*.3f,.009f,.6f,DarkWood);
        Cylinder("Lampada",at+Vector3.down*.035f,.06f,.05f,Mat("lampada",new Color(.94f,.80f,.47f),null,true));
        var light=new GameObject("Incandescente").AddComponent<Light>();light.transform.SetParent(root.transform,false);light.transform.localPosition=at+Vector3.down*.15f;light.type=LightType.Spot;light.transform.localRotation=Quaternion.Euler(90,0,0);
        light.color=new Color(1f,.88f,.70f);light.intensity=5.2f;light.range=4f;
        light.spotAngle=108f;light.innerSpotAngle=76f;light.cullingMask=1<<Layer;
        // One shadow map for the entire room, centered on the playable surface.
        light.shadows=LightShadows.Hard;
        UnityEngine.Rendering.Universal.UniversalAdditionalLightData lightData=light.gameObject.AddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalLightData>();
        if(Application.isPlaying)lightData.additionalLightsShadowResolutionTier=0;
        light.shadowBias=.015f;light.shadowNormalBias=.025f;light.shadowNearPlane=.08f;light.shadowStrength=.85f;
    }
}

using UnityEngine;
public sealed partial class SinucaRoom3D
{
    private void BuildRoom()
    {
        var floor=Mat("assoalhoNativo",Color.white,tiles[0]);floor.mainTextureScale=new Vector2(9.5f,9.85f);
        Box("AssoalhoContinuo",new Vector3(0,-.025f,-1.8f),new Vector3(11,.05f,11.4f),floor);
        var wall=Mat("paredeNativa",Color.white,tiles[1]);wall.mainTextureScale=new Vector2(9.5f,1);
        Box("LambrilFundo",new Vector3(0,1.5f,3.86f),new Vector3(11,3f,.08f),wall);
        var side=Mat("paredeLateralNativa",Color.white,tiles[1]);side.mainTextureScale=new Vector2(9.85f,1);
        Box("LambrilEsq",new Vector3(-5.46f,1.5f,-1.8f),new Vector3(.08f,3f,11.4f),side);
        Box("LambrilDir",new Vector3(5.46f,1.5f,-1.8f),new Vector3(.08f,3f,11.4f),side);
        Box("LambrilFrente",new Vector3(0,1.5f,-7.5f),new Vector3(11,3f,.08f),wall);
        Box("PortaSaida",new Vector3(0,1.03f,-7.43f),new Vector3(1.05f,2.06f,.10f),DarkWood);
        foreach(var p in SantaRitaBarLayout.Props)
        {
            if(SantaRitaBarLayout.OccupiedChair(p))continue;
            if(SantaRitaBarLayout.SpriteFor(p)==null)throw new System.InvalidOperationException("Sprite nativo ausente: "+p.key);
            CaptureArt(p.id,()=>
            {
                var a=Character(p);
                if(p.kind==SantaRitaBarLayout.Kind.Actor||p.id=="CuiaMesa"||p.id=="TermicaMesa")actors.Add(a);
                if(p.id=="TapeteSinuca")a.root.localRotation=Quaternion.Euler(90,0,0);
            });
        }
        var sun=new GameObject("LuzAmbiente3D").AddComponent<Light>();sun.transform.SetParent(root.transform,false);sun.type=LightType.Directional;sun.color=new Color(1,.96f,.87f);sun.intensity=.85f;sun.transform.localRotation=Quaternion.Euler(48,-32,0);sun.cullingMask=1<<Layer;sun.shadows=LightShadows.Soft;sun.shadowBias=.02f;
        Lamp(new Vector3(0,2.55f,0));
    }
    private void Lamp(Vector3 at)
    {
        var shade=Mat("cupulaVerde",new Color(.09f,.18f,.10f));Cylinder("Cupula",at,.23f,.055f,shade);Cylinder("FioLampada",at+Vector3.up*.3f,.009f,.6f,DarkWood);
        Cylinder("Lampada",at+Vector3.down*.035f,.06f,.05f,Mat("lampada",new Color(.94f,.80f,.47f),null,true));
        var light=new GameObject("Incandescente").AddComponent<Light>();light.transform.SetParent(root.transform,false);light.transform.localPosition=at+Vector3.down*.15f;light.type=LightType.Point;light.color=new Color(1,.86f,.66f);light.intensity=3;light.range=5.5f;light.cullingMask=1<<Layer;
    }
}

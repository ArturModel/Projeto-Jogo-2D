using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class SinucaRoom3D
{
    private Transform ceilingFan;
    private readonly List<Transform> contactActors = new List<Transform>();
    private readonly List<Transform> contactShadows = new List<Transform>();
    private readonly List<Renderer> overheadDetails = new List<Renderer>();

    // All additions stay above head height or within the existing wall clearance.
    // Entry, seating, service and play retain the shared physical plan.
    private void BuildSalonDepth()
    {
        BuildSalonDecoration();
        BuildSalonPatrons();
        var timber=Mat("estruturaCarvalho",new Color(.28f,.21f,.13f),tiles[2]);
        var stone=Mat("basePilares",new Color(.30f,.29f,.25f));
        foreach(float z in new[]{-6.8f,-1.7f,3.45f})
        {
            foreach(float x in new[]{-5.29f,5.29f})
            {
                Box("PilarEstrutural",new Vector3(x,1.43f,z),new Vector3(.22f,2.86f,.24f),timber);
                Box("SapataPilar",new Vector3(x,.17f,z),new Vector3(.25f,.34f,.28f),stone);
                Box("CapitelPilar",new Vector3(x,2.70f,z),new Vector3(.32f,.13f,.38f),timber);
                var brace=Box("MaoFrancesa",new Vector3(x-Mathf.Sign(x)*.26f,2.68f,z),new Vector3(.10f,.64f,.12f),timber);
                brace.transform.localRotation=Quaternion.Euler(0,0,Mathf.Sign(x)*-45);
                overheadDetails.Add(brace.GetComponent<Renderer>());
            }
            overheadDetails.Add(Box("TravessaSala",new Vector3(0,2.86f,z),new Vector3(10.7f,.20f,.25f),timber).GetComponent<Renderer>());
        }
        foreach(float x in new[]{-3.6f,0f,3.6f})
            overheadDetails.Add(Box("LongarinaForro",new Vector3(x,2.99f,-1.8f),new Vector3(.10f,.08f,11.2f),timber).GetComponent<Renderer>());

        BuildWindowBay("JanelaConversa",new Vector3(-5.31f,1.92f,-3.35f),90);
        BuildWindowBay("JanelaLateral",new Vector3(5.31f,1.92f,-3.35f),-90);
        // The service zone reads as built cabinetry rather than a flat back wall.
        foreach(float x in new[]{-3.73f,-.43f})
            Box("MontantePrateleira",new Vector3(x,1.95f,3.46f),new Vector3(.09f,1.58f,.20f),timber);
        Box("CoroamentoBalcao",new Vector3(-2.08f,2.77f,3.40f),new Vector3(3.55f,.16f,.34f),timber);
        Box("SoleiraMadeira",new Vector3(0,.012f,-7.12f),new Vector3(1.15f,.024f,.24f),timber);
        foreach(float x in new[]{-5.34f,5.34f})
            Box("CimalhaLateral",new Vector3(x,2.77f,-1.8f),new Vector3(.14f,.11f,11.2f),timber);

        var fan=new GameObject("VentiladorTeto").transform;fan.SetParent(scenery,false);
        fan.localPosition=new Vector3(0,2.70f,-4.8f);
        Cylinder("HasteVentilador",new Vector3(0,.15f,0),.018f,.30f,timber,fan);
        Cylinder("MotorVentilador",Vector3.zero,.10f,.10f,Mat("ferroPintado",new Color(.055f,.065f,.060f)),fan);
        ceilingFan=new GameObject("Helice").transform;ceilingFan.SetParent(fan,false);
        for(int i=0;i<3;i++)
        {
            var blade=Box("PaVentilador",new Vector3(0,-.04f,.32f),new Vector3(.15f,.018f,.62f),timber,ceilingFan);
            blade.transform.RotateAround(fan.position,Vector3.up,i*120);
        }
        overheadDetails.AddRange(fan.GetComponentsInChildren<Renderer>());

        // Bake contact footprints once. Moving actors only update transforms.
        foreach(var p in SantaRitaBarLayout.Props)
        {
            if(p.kind==SantaRitaBarLayout.Kind.Table)
                ContactPatch("ContatoMesa",Plan(p.position),new Vector2(p.width*.65f,p.width*.55f),.23f);
            else if(p.kind==SantaRitaBarLayout.Kind.Counter)
                ContactPatch("ContatoBalcao",Plan(p.position),new Vector2(3.6f,1.0f),.34f);
            else if(p.id=="GeladeiraDoBar"||p.id=="EngradadosDoBar")
                ContactPatch("ContatoMovel",Plan(p.position),new Vector2(.95f,.80f),.36f);
        }
        foreach(var a in actors)
        {
            if(a.definition.kind!=SantaRitaBarLayout.Kind.Actor)continue;
            contactActors.Add(a.root);
            contactShadows.Add(ContactPatch("Contato_"+a.definition.id,a.root.localPosition,new Vector2(.75f,.48f),.35f));
        }
        ContactPatch("OclusaoMesaSinuca",Vector3.zero,new Vector2(3.45f,1.78f),.24f);
    }

    private void BuildWindowBay(string name,Vector3 at,float yaw)
    {
        var bay=new GameObject(name).transform;bay.SetParent(scenery,false);bay.localPosition=at;bay.localRotation=Quaternion.Euler(0,yaw,0);
        var frame=Mat("estruturaCarvalho",new Color(.28f,.21f,.13f),tiles[2]);
        var dark=Mat("profundidadeJanela",new Color(.018f,.030f,.037f),null,true);
        var glass=Mat("vidroNoite",new Color(.065f,.105f,.13f),null,true);
        Box("RebaixoJanela",Vector3.zero,new Vector3(1.86f,1.25f,.06f),dark,bay);
        Box("VidroJanela",new Vector3(0,0,.035f),new Vector3(1.65f,1.06f,.02f),glass,bay);
        foreach(float x in new[]{-.91f,.91f})Box("OmbreiraJanela",new Vector3(x,0,.09f),new Vector3(.12f,1.35f,.20f),frame,bay);
        foreach(float y in new[]{-.62f,.62f})Box("VergaJanela",new Vector3(0,y,.09f),new Vector3(1.92f,.10f,.20f),frame,bay);
        Box("PeitorilJanela",new Vector3(0,-.66f,.18f),new Vector3(2.04f,.10f,.38f),frame,bay);
        foreach(float x in new[]{-.54f,0,.54f})Box("MontanteJanela",new Vector3(x,0,.12f),new Vector3(.035f,1.10f,.045f),frame,bay);
        Box("TravessaJanela",new Vector3(0,.05f,.12f),new Vector3(1.70f,.035f,.045f),frame,bay);
        // A partly open shutter has real thickness and catches the warm room light.
        var shutter=new GameObject("FolhaVeneziana").transform;shutter.SetParent(bay,false);shutter.localPosition=new Vector3(-.91f,0,.18f);shutter.localRotation=Quaternion.Euler(0,-32,0);
        for(int i=0;i<9;i++)Box("LaminaVeneziana",new Vector3(.22f,-.49f+i*.12f,0),new Vector3(.45f,.075f,.055f),frame,shutter);
        foreach(float x in new[]{0f,.44f})Box("BordaVeneziana",new Vector3(x,0,0),new Vector3(.045f,1.18f,.065f),frame,shutter);
        RoomFill(name+"_LuzExterior",at+bay.forward*.45f,new Color(.38f,.55f,.74f),.75f,4.6f);
        CombineNet(bay);
    }

    private Transform ContactPatch(string name,Vector3 at,Vector2 size,float opacity)
    {
        var g=GameObject.CreatePrimitive(PrimitiveType.Quad);g.name=name;g.layer=Layer;
        UnityEngine.Object.Destroy(g.GetComponent<Collider>());g.transform.SetParent(dynamic,false);
        g.transform.localPosition=new Vector3(at.x,.006f,at.z);g.transform.localRotation=Quaternion.Euler(90,0,0);
        g.transform.localScale=new Vector3(size.x,size.y,1);
        string key="contato_"+opacity;
        if(!materials.TryGetValue(key,out var mat))
        {
            mat=new Material(Resources.Load<Shader>("Art/Sinuca/ContatoSala"));mat.SetFloat("_Opacity",opacity);
            materials.Add(key,mat);owned.Add(mat);
        }
        var r=g.GetComponent<MeshRenderer>();r.sharedMaterial=mat;r.shadowCastingMode=ShadowCastingMode.Off;r.receiveShadows=false;
        return g.transform;
    }
    private void TickSalon()
    {
        if(ceilingFan!=null&&!MenuPausa.Aberto&&Application.isFocused)
            ceilingFan.Rotate(0,Time.deltaTime*32,0,Space.Self);
        for(int i=0;i<contactActors.Count;i++)
        {
            var p=contactActors[i].localPosition;contactShadows[i].localPosition=new Vector3(p.x,.007f,p.z);
        }
    }
}

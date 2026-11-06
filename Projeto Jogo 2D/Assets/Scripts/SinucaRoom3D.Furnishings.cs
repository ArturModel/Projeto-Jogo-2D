using UnityEngine;
using UnityEngine.Rendering;
public sealed partial class SinucaRoom3D
{
    // Volumes replace perspective-painted furniture only in this first-person room.
    // Shared layout anchors still govern circulation, exploration, and NPC positions.
    private bool BuildSolidProp(SantaRitaBarLayout.Prop p)
    {
        bool table=p.kind==SantaRitaBarLayout.Kind.Table;
        bool chair=p.kind==SantaRitaBarLayout.Kind.Chair;
        bool counter=p.kind==SantaRitaBarLayout.Kind.Counter;
        bool fridge=p.id=="GeladeiraDoBar",crates=p.id=="EngradadosDoBar";
        if(!table&&!chair&&!counter&&!fridge&&!crates&&p.id!="TelevisaoFutebol"&&p.id!="TaqueiraDoBar")return false;
        PixelSources[p.id]=SantaRitaBarLayout.SpriteFor(p);
        var g=new GameObject(p.id).transform;g.SetParent(scenery,false);
        g.localPosition=Plan(p.position-Vector2.up*p.supportOffset);
        var plastic=Mat("plasticoMarfim",new Color(.60f,.61f,.51f));
        var metal=Mat("ferroPintado",new Color(.055f,.065f,.060f));
        if(table)
        {
            float radius=p.width*.5f;
            Cylinder("TampoPlastico",new Vector3(0,.755f,0),radius,.065f,plastic,g);
            Cylinder("BordaTampo",new Vector3(0,.723f,0),radius*.98f,.025f,Mat("bordaPlastica",new Color(.40f,.42f,.34f)),g);
            foreach(float x in new[]{-1f,1f})foreach(float z in new[]{-1f,1f})
                Tube("PeMesaCafe",new Vector3(x*radius*.57f,.02f,z*radius*.57f),new Vector3(x*radius*.42f,.73f,z*radius*.42f),.031f,plastic,g);
        }
        else if(chair)
        {
            bool stool=p.key=="BancoBar";float y=stool?.68f:.44f;
            if(stool)Cylinder("AssentoBanqueta",new Vector3(0,y,0),.21f,.065f,Wood,g);
            else
            {
                // Face the associated table; no cardboard silhouettes when passing behind.
                g.localRotation=Quaternion.Euler(0,p.position.x<9?90:-90,0);
                Box("AssentoCadeira",new Vector3(0,y,0),new Vector3(.43f,.055f,.43f),plastic,g);
                for(int slat=-2;slat<=2;slat++)Box("LaminaEncosto",new Vector3(slat*.075f,.70f,-.20f),new Vector3(.038f,.44f,.038f),plastic,g);
                Box("TopoEncosto",new Vector3(0,.93f,-.20f),new Vector3(.45f,.055f,.055f),plastic,g);
            }
            foreach(float x in new[]{-1f,1f})foreach(float z in new[]{-1f,1f})
                Tube("PeAssento",new Vector3(x*.22f,.025f,z*.22f),new Vector3(x*.16f,y,z*.16f),.023f,stool?metal:plastic,g);
            if(stool){var ring=Ring("ApoioPes",.205f,.012f,new Color(.08f,.09f,.08f),g,16);ring.transform.localPosition=Vector3.up*.23f;}
        }
        else if(counter)
        {
            Box("CorpoBalcao",new Vector3(0,.48f,0),new Vector3(3.2f,.96f,.65f),DarkWood,g);
            Box("TampoBalcao",new Vector3(0,1.0f,0),new Vector3(3.35f,.10f,.80f),Wood,g);
            for(int i=-5;i<=5;i++)Box("FrisoBalcao",new Vector3(i*.28f,.47f,-.333f),new Vector3(.025f,.83f,.02f),Wood,g);
            Tube("BarraPesBalcao",new Vector3(-1.5f,.16f,-.49f),new Vector3(1.5f,.16f,-.49f),.024f,metal,g);
            // Upper half of the native sprite is a bottle shelf; the lower painted
            // counter is replaced by the solid carpentry above.
            var source=SantaRitaBarLayout.SpriteFor(p);var rect=source.rect;
            var crop=Sprite.Create(source.texture,new Rect(rect.x,rect.y+rect.height*.49f,rect.width,rect.height*.51f),new Vector2(.5f,0),source.pixelsPerUnit);
            owned.Add(crop);var shelf=PixelObject("PrateleiraGarrafas",crop,Vector3.zero,3.15f/crop.bounds.size.x/1.15f,false);
            shelf.SetParent(g,false);shelf.localPosition=new Vector3(0,1.28f,.52f);
            for(int i=0;i<3;i++)Box("PrateleiraSolida",new Vector3(0,1.35f+i*.36f,.43f),new Vector3(3.1f,.045f,.30f),Wood,g);
        }
        else if(fridge)
        {
            var enamel=Mat("esmalteGeladeira",new Color(.56f,.59f,.53f));
            Box("CorpoGeladeira",new Vector3(0,.88f,0),new Vector3(.78f,1.76f,.67f),enamel,g);
            Box("BorrachaPorta",new Vector3(0,.90f,-.345f),new Vector3(.73f,1.65f,.025f),metal,g);
            Box("PortaGeladeira",new Vector3(0,.90f,-.37f),new Vector3(.70f,1.60f,.035f),enamel,g);
            Box("DivisaoFreezer",new Vector3(0,1.36f,-.395f),new Vector3(.71f,.018f,.012f),metal,g);
            Box("PuxadorGeladeira",new Vector3(-.25f,.98f,-.42f),new Vector3(.04f,.29f,.05f),metal,g);
            for(int i=0;i<5;i++)Box("VentilacaoGeladeira",new Vector3(0,.12f+i*.022f,-.39f),new Vector3(.56f,.009f,.012f),metal,g);
        }
        else if(crates)
        {
            var crate=Mat("engradadoVerde",new Color(.09f,.17f,.10f));
            for(int level=0;level<2;level++)
            {
                float y=.16f+level*.31f;
                foreach(float z in new[]{-.22f,.22f})for(int i=0;i<3;i++)Box("RipaEngradado",new Vector3(0,y-.10f+i*.09f,z),new Vector3(.56f,.047f,.035f),crate,g);
                foreach(float x in new[]{-.27f,.27f})Box("LateralEngradado",new Vector3(x,y,0),new Vector3(.035f,.27f,.46f),crate,g);
                if(level==1)for(int i=0;i<6;i++)Cylinder("GarrafaRetornavel",new Vector3((i%3-1)*.15f,y+.12f,(i/3-.5f)*.18f),.038f,.30f,Mat("vidroGarrafa",new Color(.095f,.13f,.055f)),g);
            }
        }
        else if(p.id=="TelevisaoFutebol")
        {
            g.localPosition=new Vector3(4.55f,2.05f,3.50f);g.localRotation=Quaternion.Euler(0,-20,0);
            Box("CarcacaCRT",Vector3.zero,new Vector3(.70f,.53f,.48f),metal,g);
            Box("VidroCRT",new Vector3(-.035f,.02f,-.25f),new Vector3(.54f,.37f,.018f),Mat("telaCRT",new Color(.09f,.13f,.14f),null,true),g);
            Box("SuporteTV",new Vector3(0,-.32f,.10f),new Vector3(.55f,.05f,.65f),metal,g);
            for(int i=0;i<4;i++)Box("AltoFalanteTV",new Vector3(.29f,-.07f+i*.04f,-.253f),new Vector3(.04f,.009f,.008f),DarkWood,g);
            RoomFill("ReflexoTV",new Vector3(4.5f,1.95f,2.95f),new Color(.40f,.57f,.64f),.12f,2.2f);
        }
        else
        {
            g.localPosition=new Vector3(1.20f,1.3f,3.67f);
            Box("BaseTaqueira",new Vector3(0,-.20f,0),new Vector3(.85f,.07f,.13f),Wood,g);
            Box("TopoTaqueira",new Vector3(0,.74f,0),new Vector3(.85f,.07f,.13f),Wood,g);
            for(int i=0;i<5;i++)Tube("TacoReserva",new Vector3((i-2)*.15f,-.18f,-.08f),new Vector3((i-2)*.15f,.98f,-.10f),.008f,Wood,g);
        }
        CombineNet(g);
        return true;
    }
    private void BuildRoomArchitecture()
    {
        var plaster=Mat("rebocoEnvelhecido",new Color(.38f,.36f,.29f));
        var trim=Mat("guarnicaoEscura",new Color(.14f,.105f,.075f),tiles[2]);
        // Lambril remains below shoulder level; calm plaster above breaks up the
        // previous all-wood texture and makes the bar feel like a built interior.
        Box("RebocoFundo",new Vector3(0,2.18f,3.80f),new Vector3(10.9f,1.64f,.025f),plaster);
        Box("RebocoEsq",new Vector3(-5.40f,2.18f,-1.8f),new Vector3(.025f,1.64f,11.3f),plaster);
        Box("RebocoDir",new Vector3(5.40f,2.18f,-1.8f),new Vector3(.025f,1.64f,11.3f),plaster);
        Box("RebocoFrente",new Vector3(0,2.18f,-7.44f),new Vector3(10.9f,1.64f,.025f),plaster);
        foreach(float z in new[]{3.76f,-7.40f})
        {
            Box("RodapeHorizontal",new Vector3(0,.095f,z),new Vector3(10.9f,.19f,.055f),trim);
            Box("AcabamentoLambril",new Vector3(0,1.37f,z),new Vector3(10.9f,.055f,.065f),trim);
        }
        foreach(float x in new[]{-5.36f,5.36f})
        {
            Box("RodapeLateral",new Vector3(x,.095f,-1.8f),new Vector3(.055f,.19f,11.3f),trim);
            Box("AcabamentoLateral",new Vector3(x,1.37f,-1.8f),new Vector3(.065f,.055f,11.3f),trim);
        }
        // Ceiling strips are individually visible only from inside the room.
        ceilingRenderer=Box("Forro",new Vector3(0,3.06f,-1.8f),new Vector3(10.9f,.09f,11.3f),Mat("forroFosco",new Color(.22f,.21f,.17f))).GetComponent<Renderer>();
        ceilingBeams=new Renderer[4];int beamIndex=0;
        foreach(float z in new[]{-6f,-3f,0f,3f})ceilingBeams[beamIndex++]=Box("VigaForro",new Vector3(0,2.96f,z),new Vector3(10.9f,.15f,.14f),trim).GetComponent<Renderer>();
        // Door casing and panels establish scale at the otherwise empty entrance.
        foreach(float x in new[]{-.59f,.59f})Box("BatentePorta",new Vector3(x,1.08f,-7.34f),new Vector3(.09f,2.16f,.10f),trim);
        Box("VergaPorta",new Vector3(0,2.16f,-7.34f),new Vector3(1.27f,.10f,.10f),trim);
        foreach(float y in new[]{.58f,1.50f})Box("AlmofadaPorta",new Vector3(0,y,-7.35f),new Vector3(.78f,.70f,.035f),Wood);
        Sphere("MacanetaPorta",new Vector3(.39f,1.05f,-7.27f),.055f,Mat("lataoEnvelhecido",new Color(.38f,.28f,.12f)),scenery);
        WallSconce(new Vector3(-5.23f,2.05f,-.6f),90);
        WallSconce(new Vector3(5.23f,2.05f,-3.7f),-90);
        WallSconce(new Vector3(1.80f,2.12f,-7.24f),0);
        // A small service lamp motivates the warm spill behind the bar.
        var bulb=Mat("lenteArandela",new Color(.56f,.36f,.15f),null,true);
        Box("LuminariaBalcao",new Vector3(-2.1f,2.65f,3.43f),new Vector3(1.25f,.08f,.16f),trim);
        Box("DifusorBalcao",new Vector3(-2.1f,2.60f,3.40f),new Vector3(1.10f,.025f,.10f),bulb);
    }
    private void WallSconce(Vector3 at,float yaw)
    {
        var g=new GameObject("ArandelaParede").transform;g.SetParent(scenery,false);g.localPosition=at;g.localRotation=Quaternion.Euler(0,yaw,0);
        var iron=Mat("ferroPintado",new Color(.055f,.065f,.060f));
        Box("SuporteArandela",Vector3.zero,new Vector3(.15f,.32f,.06f),iron,g);
        Box("VidroFosco",new Vector3(0,0,.08f),new Vector3(.12f,.23f,.07f),Mat("lenteArandela",new Color(.56f,.36f,.15f),null,true),g);
        Box("ChapeuArandela",new Vector3(0,.14f,.08f),new Vector3(.23f,.045f,.18f),iron,g);
        RoomFill("LuzArandela",at+g.forward*.20f,new Color(1f,.71f,.42f),.34f,3.4f);
        CombineNet(g);
    }
}

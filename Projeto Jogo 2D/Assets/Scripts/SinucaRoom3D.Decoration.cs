using UnityEngine;
using UnityEngine.Rendering;

public sealed partial class SinucaRoom3D
{
    private void BuildSalonDecoration()
    {
        // Distribute the existing bar memorabilia by use, instead of lining every
        // item up on the service wall. No geography or narrative content changes.
        PlaceWallArt("TimeDaComunidade",new Vector3(-5.25f,1.98f,.70f),90,1.10f);
        PlaceWallArt("CartazGaleto",new Vector3(-5.25f,1.95f,-5.65f),90,.70f);
        PlaceWallArt("FiadoSoAmanha",new Vector3(-4.36f,1.98f,3.68f),180,.66f);
        PlaceWallArt("Calendario2004",new Vector3(3.7f,1.94f,3.68f),180,.53f);
        PlaceWallArt("Inter1994",new Vector3(2.50f,2.04f,3.68f),180,.94f,false);
        NoticeBoard(new Vector3(2.90f,1.83f,-7.27f));
        WallClock(new Vector3(5.24f,2.23f,.15f),-90);
        BilliardsMemorabilia(new Vector3(5.25f,1.87f,1.98f));

        var wood=Mat("estruturaCarvalho",new Color(.28f,.21f,.13f),tiles[2]);
        var iron=Mat("ferroPintado",new Color(.055f,.065f,.060f));
        // A rail at the entrance and an umbrella stand explain the threshold.
        var rail=new GameObject("ChapeleiraEntrada").transform;rail.SetParent(scenery,false);rail.localPosition=new Vector3(-2.65f,1.70f,-7.25f);
        Box("ReguaGanchos",Vector3.zero,new Vector3(1.65f,.12f,.08f),wood,rail);
        for(int i=0;i<5;i++)
        {
            float x=(i-2)*.30f;
            Tube("GanchoChapeu",new Vector3(x,0,.04f),new Vector3(x,-.07f,.15f),.012f,iron,rail);
            Sphere("PontaGancho",new Vector3(x,-.06f,.15f),.035f,iron,rail);
        }
        CombineNet(rail);
        // Small details are attached to furniture/walls; walking paths stay clear.
        var fridge=scenery.Find("GeladeiraDoBar");
        if(fridge!=null)
        {
            for(int i=0;i<3;i++)
            {
                var paper=Box("RecadoGeladeira",new Vector3(-.12f+i*.14f,1.12f+i*.08f,-.395f),new Vector3(.11f,.14f,.004f),Mat("papelRecado",new Color(.66f,.62f,.45f)),fridge);
                paper.transform.localRotation=Quaternion.Euler(0,0,(i-1)*7);
                Sphere("ImaGeladeira",new Vector3(-.12f+i*.14f,1.17f+i*.08f,-.404f),.026f,Mat("imaVinho",new Color(.28f,.06f,.035f)),fridge);
            }
        }
        // Service tray, glasses and bottles have physical thickness on the bar.
        var counter=scenery.Find("Balcao_Bar_Sprite");
        if(counter!=null)
        {
            Cylinder("BandejaBalcao",new Vector3(-.91f,1.065f,-.06f),.24f,.018f,iron,counter);
            for(int i=0;i<3;i++)
            {
                float x=-1.05f+i*.13f;
                Cylinder("CopoLimpo",new Vector3(x,1.14f,-.06f),.043f,.14f,Mat("vidroCopo",new Color(.36f,.40f,.34f)),counter);
                Cylinder("BocaCopo",new Vector3(x,1.211f,-.06f),.033f,.003f,iron,counter);
            }
        }
        // Exposed conduit follows the upper trim down to real switches.
        foreach(float x in new[]{-1.0f,1.0f})
        {
            Tube("ConduiteEntrada",new Vector3(x,2.73f,-7.30f),new Vector3(x,1.20f,-7.30f),.010f,iron);
            Box("EspelhoInterruptor",new Vector3(x,1.17f,-7.29f),new Vector3(.09f,.14f,.035f),Mat("baquelite",new Color(.35f,.30f,.19f)));
            Box("TeclaInterruptor",new Vector3(x,1.17f,-7.265f),new Vector3(.035f,.055f,.015f),iron);
        }
    }

    private void PlaceWallArt(string id,Vector3 centre,float yaw,float width,bool framed=true)
    {
        var art=dynamic.Find(id);if(art==null)return;
        FitWallArt(art,centre,yaw,width,framed);
    }
    private void NoticeBoard(Vector3 centre)
    {
        var board=new GameObject("AvisosDoBar").transform;board.SetParent(scenery,false);board.localPosition=centre;
        var wood=Mat("estruturaCarvalho",new Color(.28f,.21f,.13f),tiles[2]);
        Box("MolduraAvisos",Vector3.zero,new Vector3(1.28f,.88f,.06f),wood,board);
        Box("CorticaAvisos",new Vector3(0,0,.035f),new Vector3(1.18f,.78f,.025f),Mat("cortica",new Color(.35f,.25f,.13f)),board);
        for(int i=0;i<5;i++)
        {
            var note=new GameObject("AvisoPapel").transform;note.SetParent(board,false);
            note.localPosition=new Vector3(-.43f+(i%3)*.40f,.19f-(i/3)*.34f,.055f);
            note.localRotation=Quaternion.Euler(0,0,(i%3-1)*4);
            Box("FolhaAviso",Vector3.zero,new Vector3(.27f,.27f,.004f),Mat("papelRecado",new Color(.66f,.62f,.45f)),note);
            for(int line=0;line<4;line++)Box("ImpressaoAviso",new Vector3(0,.07f-line*.035f,.004f),new Vector3(line==0?.18f:.21f,.006f,.001f),Mat("tintaAviso",new Color(.22f,.20f,.15f)),note);
            Sphere("AlfineteAviso",new Vector3(0,.11f,.008f),.018f,Mat("imaVinho",new Color(.28f,.06f,.035f)),note);
        }
        CombineNet(board);
    }
    private void FitWallArt(Transform art,Vector3 centre,float yaw,float width,bool framed)
    {
        // Native sprite meshes face local -Z. The assembly's +Z points into room.
        var bounds=art.GetComponent<MeshFilter>().sharedMesh.bounds;
        float scale=width/Mathf.Max(.01f,bounds.size.x);
        art.localScale=Vector3.one*scale;
        art.localRotation=Quaternion.Euler(0,yaw+180,0);
        art.localPosition=centre-art.localRotation*(bounds.center*scale);
        if(!framed)return;
        var frame=new GameObject("Moldura_"+art.name).transform;frame.SetParent(scenery,false);
        frame.localPosition=centre;frame.localRotation=Quaternion.Euler(0,yaw,0);
        float h=bounds.size.y*scale;
        var timber=Mat("estruturaCarvalho",new Color(.28f,.21f,.13f),tiles[2]);
        Box("FundoQuadro",new Vector3(0,0,-.025f),new Vector3(width+.05f,h+.05f,.035f),timber,frame);
        foreach(float x in new[]{-width*.5f,width*.5f})Box("MolduraVertical",new Vector3(x,0,0),new Vector3(.035f,h+.07f,.065f),timber,frame);
        foreach(float y in new[]{-h*.5f,h*.5f})Box("MolduraHorizontal",new Vector3(0,y,0),new Vector3(width+.07f,.035f,.065f),timber,frame);
        CombineNet(frame);
    }
    private void WallClock(Vector3 centre,float yaw)
    {
        var clock=new GameObject("RelogioDoSalao").transform;clock.SetParent(scenery,false);clock.localPosition=centre;clock.localRotation=Quaternion.Euler(0,yaw,0);
        var cream=Mat("mostradorRelogio",new Color(.65f,.61f,.46f));
        var iron=Mat("ferroPintado",new Color(.055f,.065f,.060f));
        var rim=Cylinder("AroRelogio",Vector3.zero,.25f,.07f,iron,clock);rim.transform.localRotation=Quaternion.Euler(90,0,0);
        var dial=Cylinder("Mostrador",new Vector3(0,0,.04f),.223f,.009f,cream,clock);dial.transform.localRotation=Quaternion.Euler(90,0,0);
        for(int i=0;i<12;i++)
        {
            float angle=i*Mathf.PI/6;
            var tick=Box("IndiceRelogio",new Vector3(Mathf.Sin(angle)*.184f,Mathf.Cos(angle)*.184f,.048f),new Vector3(.012f,.030f,.005f),iron,clock);
            tick.transform.localRotation=Quaternion.Euler(0,0,-i*30);
        }
        Tube("PonteiroHora",new Vector3(0,0,.06f),new Vector3(-.10f,-.03f,.06f),.008f,iron,clock);
        Tube("PonteiroMinuto",new Vector3(0,0,.065f),new Vector3(.08f,.14f,.065f),.005f,iron,clock);
        CombineNet(clock);
    }
    private void BilliardsMemorabilia(Vector3 at)
    {
        var display=new GameObject("CantoTorneiosSinuca").transform;display.SetParent(scenery,false);
        display.localPosition=at;display.localRotation=Quaternion.Euler(0,-90,0);
        var timber=Mat("estruturaCarvalho",new Color(.28f,.21f,.13f),tiles[2]);
        var iron=Mat("ferroPintado",new Color(.055f,.065f,.060f));
        Box("BaseContador",Vector3.zero,new Vector3(1.45f,.42f,.06f),timber,display);
        foreach(float y in new[]{-.10f,.10f})
        {
            Tube("ArameContador",new Vector3(-.65f,y,.06f),new Vector3(.65f,y,.06f),.006f,iron,display);
            for(int i=0;i<10;i++)
            {
                var bead=Cylinder("ContaPlacar",new Vector3(-.58f+i*.061f,y,.06f),.041f,.040f,y>0?Wood:Mat("contaMarfim",new Color(.69f,.64f,.46f)),display);
                bead.transform.localRotation=Quaternion.Euler(0,0,90);
            }
        }
        Box("PrateleiraTrofeus",new Vector3(0,-.43f,.08f),new Vector3(1.52f,.045f,.18f),timber,display);
        var brass=Mat("lataoTrofeu",new Color(.47f,.32f,.09f));brass.SetFloat("_Smoothness",.55f);
        for(int i=0;i<3;i++)
        {
            float x=(i-1)*.42f,h=i==1?.24f:.18f;
            Box("PedestalTrofeu",new Vector3(x,-.38f,.08f),new Vector3(.14f,.07f,.12f),iron,display);
            Cylinder("HasteTrofeu",new Vector3(x,-.32f,.08f),.015f,.10f,brass,display);
            Cylinder("TacaTrofeu",new Vector3(x,-.25f+h*.15f,.08f),.068f,h*.42f,brass,display);
            Cylinder("BocaTaca",new Vector3(x,-.25f+h*.36f,.08f),.055f,.003f,iron,display);
        }
        CombineNet(display);
    }
}

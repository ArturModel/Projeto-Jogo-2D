using System.Collections.Generic;
using UnityEngine;
public sealed partial class SinucaRoom3D
{
    private void BuildTable()
    {
        var felt=Mat("feltro",new Color(.84f,.94f,.82f),tiles[3]);
        PocketSlab("FundoMesa",new Vector3(0,.72f,0),new Vector3(3,.12f,1.58f),DarkWood);
        PocketSlab("Pano",new Vector3(0,TableY-.002f,0),new Vector3(2.84f,.004f,1.42f),felt);
        foreach(float x in new[]{-1f,1f})foreach(float z in new[]{-1f,1f}){Box("PeMesa",new Vector3(x*1.20f,.35f,z*.53f),new Vector3(.16f,.7f,.16f),Wood);Box("SapatoPe",new Vector3(x*1.20f,.035f,z*.53f),new Vector3(.18f,.07f,.18f),DarkWood);}
        var rubber=Mat("borrachaVerde",new Color(.045f,.21f,.095f));
        foreach(float side in new[]{-1f,1f})
        {
            foreach(float half in new[]{-1f,1f}){PocketSlab("TabelaMadeiraLonga",new Vector3(half*.74f,.82f,side*.77f),new Vector3(1.30f,.105f,.13f),Wood);PocketSlab("SaiaMesa",new Vector3(half*.74f,.64f,side*.77f),new Vector3(1.30f,.27f,.09f),Wood);PocketSlab("BorrachaLonga",new Vector3(half*.74f,.839f,side*.704f),new Vector3(1.28f,.045f,.037f),rubber);}
            PocketSlab("TabelaCabeceira",new Vector3(side*1.47f,.82f,0),new Vector3(.13f,.105f,1.24f),Wood);PocketSlab("SaiaCabeceira",new Vector3(side*1.47f,.64f,0),new Vector3(.09f,.27f,1.24f),Wood);PocketSlab("BorrachaCabeceira",new Vector3(side*1.41f,.839f,0),new Vector3(.037f,.045f,1.22f),rubber);
        }
        var ivory=Mat("madreperola",new Color(.78f,.70f,.45f));for(int i=-2;i<=2;i++)foreach(float side in new[]{-1f,1f})if(i!=0)Sphere("Mosca",new Vector3(i*.47f,.877f,side*.77f),.012f,ivory,scenery);
        for(int j=0;j<6;j++)
        {
            Vector2 p=PocketOuter(j);
            BuildPocketLining(j);
            // Open leather lip: no solid cylinder or raised cap across the ball's path.
            Cylinder("BuracoEscuro_"+j,new Vector3(p.x,.75f,p.y),.077f,.004f,Mat("interiorCacapa",new Color(.004f,.005f,.003f),null,true));
            var lip=Ring("AroCouro_"+j,.078f,.006f,new Color(.10f,.065f,.035f),scenery,24);
            lip.transform.localPosition=new Vector3(p.x,TableY-.012f,p.y);CombineNet(lip.transform);
            var net=new GameObject("RedeExterna_"+j).transform;net.SetParent(dynamic,false);net.localPosition=new Vector3(p.x,.80f,p.y);nets[j]=net;var cord=Mat("barbante",new Color(.70f,.65f,.46f));
            for(int n=0;n<12;n++){float a=n*Mathf.PI*2/12;for(int k=0;k<5;k++){float y0=-k*.052f,y1=-(k+1)*.052f,r0=Mathf.Lerp(.061f,.024f,k/5f),r1=Mathf.Lerp(.061f,.024f,(k+1)/5f);Tube("MalhaRede",new Vector3(Mathf.Cos(a)*r0,y0,Mathf.Sin(a)*r0),new Vector3(Mathf.Cos(a+.18f)*r1,y1,Mathf.Sin(a+.18f)*r1),.0018f,cord,net);}}
            for(int k=0;k<6;k++)Ring("LacoRede",Mathf.Lerp(.061f,.024f,k/5f),.0018f,new Color(.70f,.65f,.46f),net,16).transform.localPosition=Vector3.down*k*.052f;
            foreach(var renderer in net.GetComponentsInChildren<MeshRenderer>())renderer.sharedMaterial=cord;
            CombineNet(net);
        }
    }

    private void BuildPocketLining(int pocket)
    {
        var from=PocketPhysics(pocket);var to=PocketCutExit(pocket);
        float angle=Mathf.Atan2(to.y-from.y,to.x-from.x);var polygon=new List<Vector2>();
        for(int k=0;k<=12;k++){float a=angle-Mathf.PI*.5f+k*Mathf.PI/12;polygon.Add(to+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*(SinucaMinigameUI.PocketOpeningRadius*MetresPerUnit));}
        for(int k=0;k<=12;k++){float a=angle+Mathf.PI*.5f+k*Mathf.PI/12;polygon.Add(from+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*(SinucaMinigameUI.PocketOpeningRadius*MetresPerUnit));}
        // The dark lining follows the existing opening and ends at the outer wood.
        // A rectangular backing projects black tabs beyond the corners of the table.
        var bounds=new[]{new Vector2(-1.535f,-.835f),new Vector2(1.535f,-.835f),new Vector2(1.535f,.835f),new Vector2(-1.535f,.835f)};
        for(int edge=0;edge<4;edge++)polygon=ClipPocketPolygon(polygon,bounds[edge],bounds[(edge+1)%4],true);
        var vertices=new List<Vector3>();var triangles=new List<int>();
        foreach(var point in polygon)vertices.Add(new Vector3(point.x,TableY-.065f,point.y));
        for(int i=1;i<polygon.Count-1;i++)triangles.AddRange(new[]{0,i+1,i});
        var mesh=new Mesh{name="ForroCacapa_"+pocket};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();owned.Add(mesh);
        var go=new GameObject("GargantaCacapa_"+pocket);go.transform.SetParent(scenery,false);go.layer=Layer;
        go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=Mat("interiorCacapa",new Color(.004f,.005f,.003f),null,true);
    }

    // Subtract six convex capsule profiles. Every remaining polygon is convex, so a fan
    // triangulates it exactly; no dense grid, stair-step holes, or per-frame rebuild.
    private GameObject PocketSlab(string name,Vector3 at,Vector3 size,Material material)
    {
        float x0=at.x-size.x*.5f,x1=at.x+size.x*.5f,z0=at.z-size.z*.5f,z1=at.z+size.z*.5f;
        var pieces=new List<List<Vector2>>{new List<Vector2>{new Vector2(x0,z0),new Vector2(x1,z0),new Vector2(x1,z1),new Vector2(x0,z1)}};
        for(int pocket=0;pocket<6;pocket++)
        {
            var from=PocketPhysics(pocket);var to=PocketCutExit(pocket);var axis=(to-from).normalized;
            float angle=Mathf.Atan2(axis.y,axis.x);var cut=new List<Vector2>();
            const float radius=(SinucaMinigameUI.PocketOpeningRadius*MetresPerUnit);
            for(int k=0;k<=12;k++){float a=angle-Mathf.PI*.5f+k*Mathf.PI/12;cut.Add(to+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*radius);}
            for(int k=0;k<=12;k++){float a=angle+Mathf.PI*.5f+k*Mathf.PI/12;cut.Add(from+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*radius);}
            var next=new List<List<Vector2>>();
            foreach(var piece in pieces)
            {
                var inside=piece;
                for(int edge=0;edge<cut.Count && inside.Count>=3;edge++)
                {
                    var outside=ClipPocketPolygon(inside,cut[edge],cut[(edge+1)%cut.Count],false);
                    if(outside.Count>=3)next.Add(outside);
                    inside=ClipPocketPolygon(inside,cut[edge],cut[(edge+1)%cut.Count],true);
                }
            }
            pieces=next;
        }
        var vertices=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
        float top=at.y+size.y*.5f,bottom=at.y-size.y*.5f;
        foreach(var polygon in pieces)
        {
            int start=vertices.Count;
            foreach(var point in polygon){vertices.Add(new Vector3(point.x,top,point.y));uv.Add(point*2);}
            for(int i=1;i<polygon.Count-1;i++)triangles.AddRange(new[]{start,start+i+1,start+i});
            // The thin cloth needs only its upper face. Solid pieces have closed edges.
            if(size.y<.005f)continue;
            for(int i=0;i<polygon.Count;i++)
            {
                var a=polygon[i];var b=polygon[(i+1)%polygon.Count];int v=vertices.Count;
                vertices.AddRange(new[]{new Vector3(a.x,top,a.y),new Vector3(a.x,bottom,a.y),new Vector3(b.x,bottom,b.y),new Vector3(b.x,top,b.y)});
                uv.AddRange(new[]{a*2,new Vector2(a.x*2,bottom*2),new Vector2(b.x*2,bottom*2),b*2});
                triangles.AddRange(new[]{v,v+2,v+1,v,v+3,v+2});
            }
        }
        var mesh=new Mesh{name=name+"_BocasRecortadas"};mesh.SetVertices(vertices);mesh.SetUVs(0,uv);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();owned.Add(mesh);
        var go=new GameObject(name);go.transform.SetParent(scenery,false);go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=material;go.layer=Layer;return go;
    }
    private static List<Vector2> ClipPocketPolygon(List<Vector2> polygon,Vector2 a,Vector2 b,bool keepInside)
    {
        var result=new List<Vector2>();var edge=b-a;
        for(int i=0;i<polygon.Count;i++)
        {
            var p=polygon[i];var q=polygon[(i+1)%polygon.Count];
            float dp=edge.x*(p.y-a.y)-edge.y*(p.x-a.x),dq=edge.x*(q.y-a.y)-edge.y*(q.x-a.x);
            bool pin=keepInside?dp>=0:dp<=0,qin=keepInside?dq>=0:dq<=0;
            if(pin)result.Add(p);
            if(pin!=qin)result.Add(Vector2.Lerp(p,q,dp/(dp-dq)));
        }
        return result;
    }
    private Transform BuildCue(Transform parent)
    {
        var g=new GameObject("TacoProporcional").transform;g.SetParent(parent,false);Tube("MadeiraTaco",new Vector3(0,0,-1.22f),new Vector3(0,0,-.16f),.008f,Mat("tacoClaro",new Color(.62f,.43f,.20f)),g);Tube("Empunhadura",new Vector3(0,0,-.92f),new Vector3(0,0,-.62f),.013f,DarkWood,g);Tube("Virola",new Vector3(0,0,-.16f),new Vector3(0,0,-.10f),.006f,Mat("virola",new Color(.78f,.77f,.60f)),g);Tube("SolaAzul",new Vector3(0,0,-.10f),new Vector3(0,0,-.087f),.006f,Mat("gizAzul",new Color(.16f,.39f,.47f)),g);return g;
    }
    public static Vector2 PocketPhysics(int j){float x=206*MetresPerUnit,z=101*MetresPerUnit;return j==0?new Vector2(-x,z):j==1?new Vector2(0,z+2*MetresPerUnit):j==2?new Vector2(x,z):j==3?new Vector2(-x,-z):j==4?new Vector2(0,-z-2*MetresPerUnit):new Vector2(x,-z);}
    // The visible mouth, net and falling ball share the actual opening centre.
    public static Vector2 PocketOuter(int j)=>PocketPhysics(j);
    // Preserve the already-correct cut through the wooden rail; this is its outer
    // clearance limit, not a second pocket centre behind the opening.
    private static Vector2 PocketCutExit(int j)=>j==0?new Vector2(-1.47f,.765f):j==1?new Vector2(0,.78f):j==2?new Vector2(1.47f,.765f):j==3?new Vector2(-1.47f,-.765f):j==4?new Vector2(0,-.78f):new Vector2(1.47f,-.765f);
}

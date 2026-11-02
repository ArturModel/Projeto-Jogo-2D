using System.Collections.Generic;
using UnityEngine;
public sealed partial class SinucaRoom3D
{
    private void BuildTable()
    {
        var felt=Mat("feltro",new Color(.84f,.94f,.82f),tiles[3]);Box("FundoMesa",new Vector3(0,.72f,0),new Vector3(3,.12f,1.58f),DarkWood);
        var verts=new List<Vector3>();var uv=new List<Vector2>();var tris=new List<int>();const int nx=114,nz=58;float dx=2.84f/nx,dz=1.42f/nz;
        for(int x=0;x<nx;x++)for(int z=0;z<nz;z++)
        {
            float px=-1.42f+x*dx,pz=-.71f+z*dz;var center=new Vector2(px+dx/2,pz+dz/2);bool hole=false;for(int j=0;j<6;j++)if(Vector2.Distance(center,PocketPhysics(j))<.068f){hole=true;break;}if(hole)continue;int v=verts.Count;
            verts.Add(new Vector3(px,TableY,pz));verts.Add(new Vector3(px,TableY,pz+dz));verts.Add(new Vector3(px+dx,TableY,pz+dz));verts.Add(new Vector3(px+dx,TableY,pz));uv.Add(new Vector2(px,pz)*2);uv.Add(new Vector2(px,pz+dz)*2);uv.Add(new Vector2(px+dx,pz+dz)*2);uv.Add(new Vector2(px+dx,pz)*2);tris.AddRange(new[]{v,v+1,v+2,v,v+2,v+3});
        }
        var mesh=new Mesh{name="PanoComBocas"};mesh.SetVertices(verts);mesh.SetUVs(0,uv);mesh.SetTriangles(tris,0);mesh.RecalculateNormals();owned.Add(mesh);var surface=new GameObject("Pano");surface.transform.SetParent(scenery,false);surface.AddComponent<MeshFilter>().sharedMesh=mesh;surface.AddComponent<MeshRenderer>().sharedMaterial=felt;
        foreach(float x in new[]{-1f,1f})foreach(float z in new[]{-1f,1f}){Box("PeMesa",new Vector3(x*1.20f,.35f,z*.53f),new Vector3(.16f,.7f,.16f),Wood);Box("SapatoPe",new Vector3(x*1.20f,.035f,z*.53f),new Vector3(.18f,.07f,.18f),DarkWood);}
        var rubber=Mat("borrachaVerde",new Color(.045f,.21f,.095f));
        foreach(float side in new[]{-1f,1f})
        {
            foreach(float half in new[]{-1f,1f}){Box("TabelaMadeiraLonga",new Vector3(half*.74f,.82f,side*.77f),new Vector3(1.30f,.105f,.13f),Wood);Box("SaiaMesa",new Vector3(half*.74f,.64f,side*.77f),new Vector3(1.30f,.27f,.09f),Wood);Box("BorrachaLonga",new Vector3(half*.74f,.839f,side*.704f),new Vector3(1.28f,.045f,.037f),rubber);}
            Box("TabelaCabeceira",new Vector3(side*1.47f,.82f,0),new Vector3(.13f,.105f,1.24f),Wood);Box("SaiaCabeceira",new Vector3(side*1.47f,.64f,0),new Vector3(.09f,.27f,1.24f),Wood);Box("BorrachaCabeceira",new Vector3(side*1.41f,.839f,0),new Vector3(.037f,.045f,1.22f),rubber);
        }
        var ivory=Mat("madreperola",new Color(.78f,.70f,.45f));for(int i=-2;i<=2;i++)foreach(float side in new[]{-1f,1f})if(i!=0)Sphere("Mosca",new Vector3(i*.47f,.877f,side*.77f),.012f,ivory,scenery);
        for(int j=0;j<6;j++)
        {
            Vector2 p=PocketOuter(j);Vector2 throat=PocketPhysics(j);Vector2 delta=p-throat;
            var chute=Box("GargantaCacapa",new Vector3((p.x+throat.x)*.5f,TableY-.004f,(p.y+throat.y)*.5f),new Vector3(.11f,.008f,delta.magnitude+.05f),Mat("interiorCacapa",new Color(.009f,.012f,.008f)));
            chute.transform.localRotation=Quaternion.Euler(0,Mathf.Atan2(delta.x,delta.y)*Mathf.Rad2Deg,0);
            Cylinder("BocaExterna_"+j,new Vector3(p.x,.855f,p.y),.075f,.035f,Mat("couroBoca",new Color(.055f,.045f,.025f)));Cylinder("BuracoEscuro",new Vector3(p.x,.879f,p.y),.058f,.004f,Mat("interiorCacapa",new Color(.009f,.012f,.008f)));Ring("AroCouro",.065f,.008f,new Color(.17f,.105f,.045f),scenery,28).transform.localPosition=new Vector3(p.x,.88f,p.y);
            var net=new GameObject("RedeExterna_"+j).transform;net.SetParent(dynamic,false);net.localPosition=new Vector3(p.x,.80f,p.y);nets[j]=net;var cord=Mat("barbante",new Color(.70f,.65f,.46f));
            for(int n=0;n<12;n++){float a=n*Mathf.PI*2/12;for(int k=0;k<5;k++){float y0=-k*.052f,y1=-(k+1)*.052f,r0=Mathf.Lerp(.061f,.024f,k/5f),r1=Mathf.Lerp(.061f,.024f,(k+1)/5f);Tube("MalhaRede",new Vector3(Mathf.Cos(a)*r0,y0,Mathf.Sin(a)*r0),new Vector3(Mathf.Cos(a+.18f)*r1,y1,Mathf.Sin(a+.18f)*r1),.0018f,cord,net);}}
            for(int k=0;k<6;k++)Ring("LacoRede",Mathf.Lerp(.061f,.024f,k/5f),.0018f,new Color(.70f,.65f,.46f),net,16).transform.localPosition=Vector3.down*k*.052f;
            CombineNet(net);
        }
    }
    private Transform BuildCue(Transform parent)
    {
        var g=new GameObject("TacoProporcional").transform;g.SetParent(parent,false);Tube("MadeiraTaco",new Vector3(0,0,-1.22f),new Vector3(0,0,-.16f),.008f,Mat("tacoClaro",new Color(.62f,.43f,.20f)),g);Tube("Empunhadura",new Vector3(0,0,-1.22f),new Vector3(0,0,-.89f),.013f,DarkWood,g);Tube("Virola",new Vector3(0,0,-.16f),new Vector3(0,0,-.10f),.006f,Mat("virola",new Color(.78f,.77f,.60f)),g);Tube("SolaAzul",new Vector3(0,0,-.10f),new Vector3(0,0,-.087f),.006f,Mat("gizAzul",new Color(.16f,.39f,.47f)),g);return g;
    }
    public static Vector2 PocketPhysics(int j){float x=206*MetresPerUnit,z=101*MetresPerUnit;return j==0?new Vector2(-x,z):j==1?new Vector2(0,z+2*MetresPerUnit):j==2?new Vector2(x,z):j==3?new Vector2(-x,-z):j==4?new Vector2(0,-z-2*MetresPerUnit):new Vector2(x,-z);}
    public static Vector2 PocketOuter(int j)=>j==0?new Vector2(-1.47f,.765f):j==1?new Vector2(0,.78f):j==2?new Vector2(1.47f,.765f):j==3?new Vector2(-1.47f,-.765f):j==4?new Vector2(0,-.78f):new Vector2(1.47f,-.765f);
}

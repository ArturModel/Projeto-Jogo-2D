using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public sealed partial class SinucaRoom3D
{
    private readonly Dictionary<Sprite,Mesh> pixelMeshes=new Dictionary<Sprite,Mesh>();
    private readonly List<Transform> pixelBillboards=new List<Transform>();
    private Actor Character(SantaRitaBarLayout.Prop p)
    {
        var sprite=SantaRitaBarLayout.SpriteFor(p);PixelSources[p.id]=sprite;var at=Plan(p.position-Vector2.up*p.supportOffset);at.y=p.elevation;if(p.kind==SantaRitaBarLayout.Kind.Wall){at.z=3.72f;at.y=(p.position.y-SantaRitaBarLayout.WalkableRoom.yMax)*1.15f;}var g=PixelObject(p.id,sprite,at,p.kind==SantaRitaBarLayout.Kind.Actor?1:p.width/sprite.bounds.size.x,p.kind!=SantaRitaBarLayout.Kind.Wall&&p.id!="TapeteSinuca");
        // Props apoiados compartilham o plano do sprite da mesa; não ficam atrás do tampo ao orbitar.
        string support=p.id=="CuiaMesa"||p.id=="TermicaMesa"?"MesaChimarrao":p.id=="CopoConversa"?"MesaConversa":p.id=="CopoJanela"?"MesaJanela":null;
        if(support!=null)
        {
            var definition=System.Array.Find(SantaRitaBarLayout.Props,x=>x.id==support);
            if(definition!=null)
            {
                var centre=Plan(definition.position);
                // Place cups/mate on the solid tabletop, independent of sprite perspective.
                g.localPosition=new Vector3(centre.x+(p.position.x-definition.position.x)*.8f,.79f,centre.z+.08f);
                g.localScale=Vector3.one*(p.width/sprite.bounds.size.x*.80f);
            }
        }

        var a=new Actor{definition=p,root=g,filter=g.GetComponent<MeshFilter>(),renderer=g.GetComponent<MeshRenderer>(),rest=sprite,phase=Mathf.Abs(p.id.GetHashCode()%197)*.1f,seated=!p.key.Contains("Ze")};
        if(p.kind==SantaRitaBarLayout.Kind.Actor&&a.seated){foreach(var t in new[]{0f,7.2f,7.6f,8.3f,9.4f,9.8f,17.5f,18.5f,22f}){var frame=SantaRitaBarLayout.PatronPose(p.key,p.id,t);if(frame!=null){NativeMesh(frame);PixelMaterial(frame);}}a.drink=Resources.Load<Sprite>("Art/Characters/"+p.key+"_Bebendo");if(a.drink!=null){NativeMesh(a.drink);PixelMaterial(a.drink);}}
        return a;
    }
    private Transform PixelObject(string id,Sprite sprite,Vector3 at,float scale,bool billboard)
    {
        var g=new GameObject(id);g.transform.SetParent(dynamic,false);g.transform.localPosition=at;g.transform.localScale=Vector3.one*(scale*1.15f);
        g.AddComponent<MeshFilter>().sharedMesh=NativeMesh(sprite);var renderer=g.AddComponent<MeshRenderer>();
        renderer.sharedMaterial=PixelMaterial(sprite);renderer.shadowCastingMode=ShadowCastingMode.Off;
        if(billboard)pixelBillboards.Add(g.transform);
        return g.transform;
    }
    private Material PixelMaterial(Sprite sprite)
    {
        string key="pixel_"+sprite.texture.name;
        if(materials.TryGetValue(key,out var mat))return mat;
        mat=new Material(Resources.Load<Material>("Art/BarRef7/BarPixelCutout")){name=key};mat.SetTexture("_BaseMap",sprite.texture);mat.SetColor("_BaseColor",new Color(.87f,.84f,.77f));mat.shader=Resources.Load<Shader>("Art/Sinuca/SalaLit");mat.SetFloat("_AlphaClip",1);mat.SetFloat("_Cull",0);mat.SetFloat("_Smoothness",.03f);mat.EnableKeyword("_ALPHATEST_ON");mat.renderQueue=2450;materials.Add(key,mat);owned.Add(mat);return mat;
    }
    private Mesh NativeMesh(Sprite sprite)
    {
        if(pixelMeshes.TryGetValue(sprite,out var mesh))return mesh;
        var points=sprite.vertices;var vertices=new Vector3[points.Length];for(int i=0;i<points.Length;i++)vertices[i]=points[i];
        var source=sprite.triangles;var triangles=new int[source.Length];for(int i=0;i<source.Length;i++)triangles[i]=source[i];
        mesh=new Mesh{name="PixelNativo_"+sprite.name,vertices=vertices,uv=sprite.uv,triangles=triangles};mesh.RecalculateNormals();mesh.RecalculateBounds();pixelMeshes.Add(sprite,mesh);owned.Add(mesh);return mesh;
    }
    private void OrientPixelArt()
    {
        foreach(var t in pixelBillboards)
        {
            if(mode==SinucaCameraView.VistaSuperior)t.localRotation=Quaternion.Euler(90,0,0);
            else{Vector3 away=t.localPosition-camera.transform.localPosition;away.y=0;if(away.sqrMagnitude>.01f)t.localRotation=Quaternion.LookRotation(away);}
        }
    }
    private Material BallMaterial(int n)
    {
        var color=n==0?new Color(.94f,.93f,.87f):SinucaTableView.CorDaBola(n);var m=Mat("bola_"+n,Color.white);m.SetFloat("_Smoothness",.65f);
        var tex=new Texture2D(256,128,TextureFormat.RGBA32,false){filterMode=FilterMode.Bilinear};
        for(int y=0;y<128;y++)for(int x=0;x<256;x++)
        {float u=x/256f,v=y/128f;bool patch=n>0&&((Mathf.Pow((u-.25f)*2,2)+Mathf.Pow(v-.5f,2)<.013f)||(Mathf.Pow((u-.75f)*2,2)+Mathf.Pow(v-.5f,2)<.013f));tex.SetPixel(x,y,patch||n>8&&Mathf.Abs(v-.5f)>.18f?new Color(.94f,.93f,.87f):color);}
        if(n>0){string[] glyph={"111101101101111","010110010010111","111001111100111","111001111001111","101101111001001","111100111001111","111100111101111","111001010010010","111101111101111","111101111001111"};string digits=n.ToString();foreach(int center in new[]{64,192})for(int k=0;k<digits.Length;k++)for(int row=0;row<5;row++)for(int col=0;col<3;col++)if(glyph[digits[k]-'0'][row*3+col]=='1')for(int dy=0;dy<3;dy++)for(int dx=0;dx<3;dx++)tex.SetPixel(center-digits.Length*6+k*12+col*3+dx,70-row*3+dy,new Color(.035f,.035f,.025f));}
        tex.Apply();owned.Add(tex);m.SetTexture("_BaseMap",tex);return m;
    }
}

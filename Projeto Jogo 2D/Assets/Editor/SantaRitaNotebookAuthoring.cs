using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using Object=UnityEngine.Object;

public static class SantaRitaNotebookAuthoring
{
    [MenuItem("Santa Rita/Mapa e caderno/Preparar fonte e paisagem")]
    public static void Run()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Autoria apenas fora de Play; preservar a sessão antes.");
        PrepareFont();Landscape();
        foreach(string path in new[]{"Assets/Resources/Notebook/Book.png","Assets/Resources/Notebook/Leaf.png","Assets/Resources/Notebook/Sketch.png","Assets/Resources/Atlas/Paisagem.png"})
        {
            AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceUpdate);var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType=TextureImporterType.Default;importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;
            importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=4096;importer.SaveAndReimport();
        }
        AssetDatabase.SaveAssets();Debug.Log("CADERNO AUTHORING PASS: fonte manuscrita, papel e paisagem prontos.");
    }
    private static void PrepareFont()
    {
        const string path="Assets/Resources/Fonts/Kalam/MarcusHand.asset";
        if(AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path)!=null)return;
        var source=AssetDatabase.LoadAssetAtPath<Font>("Assets/Resources/Fonts/Kalam/Kalam-Regular.ttf");
        var font=TMP_FontAsset.CreateFontAsset(source,48,6,GlyphRenderMode.SDFAA,1024,1024,AtlasPopulationMode.Dynamic,false);
        font.name="Marcus — manuscrito";var characters=new System.Text.StringBuilder();
        for(int c=32;c<=383;c++)characters.Append((char)c);characters.Append("‘’“”–—…");
        font.TryAddCharacters(characters.ToString(),out string missing);
        font.atlasPopulationMode=AtlasPopulationMode.Static;
        AssetDatabase.CreateAsset(font,path);AssetDatabase.AddObjectToAsset(font.material,font);
        foreach(var tex in font.atlasTextures)AssetDatabase.AddObjectToAsset(tex,font);
        EditorUtility.SetDirty(font);AssetDatabase.SaveAssets();
    }
    private static Texture2D Read(string resource)
    {
        var source=Resources.Load<Texture2D>(resource);if(source==null)throw new Exception("Arte ausente: "+resource);
        var previous=RenderTexture.active;var target=RenderTexture.GetTemporary(source.width,source.height,0,RenderTextureFormat.ARGB32);
        try{Graphics.Blit(source,target);RenderTexture.active=target;var copy=new Texture2D(source.width,source.height,TextureFormat.RGBA32,false);copy.ReadPixels(new Rect(0,0,source.width,source.height),0,0);copy.Apply();return copy;}
        finally{RenderTexture.active=previous;RenderTexture.ReleaseTemporary(target);}
    }
    private static void Landscape()
    {
        Rect extent=SantaRitaAtlas.Extent;const int ppu=16;
        int w=Mathf.RoundToInt(extent.width*ppu),h=Mathf.RoundToInt(extent.height*ppu);
        var grass=Read("Art/Macros/Grama_0_0");var water=Read("Art/Tiles/Agua");
        var gp=grass.GetPixels32();var wp=water.GetPixels32();var colors=new Color32[w*h];
        var hills=new[]{new Vector3(-114,103,29),new Vector3(-91,111,25),new Vector3(-130,78,23)};
        // Morros baixos, campos úmidos e arroio de entorno. Sem caminhos ou prédios novos.
        for(int y=0;y<h;y++)for(int x=0;x<w;x++)
        {
            float wx=extent.x+x/(float)ppu,wy=extent.y+y/(float)ppu;
            Color color=gp[(y%grass.height)*grass.width+x%grass.width];
            float roll=.9f+.1f*Mathf.Sin(wx*.073f+wy*.035f)+.07f*Mathf.PerlinNoise(wx*.07f+20,wy*.07f+20);
            color*=roll;color.a=1;
            foreach(var hill in hills)
            {
                float dx=(wx-hill.x)/hill.z,dy=(wy-hill.y)/(hill.z*.7f);
                float radius=Mathf.Sqrt(dx*dx+dy*dy);if(radius>=1)continue;
                float rise=Mathf.Pow(1-radius,1.3f);
                float ridge=Mathf.PerlinNoise(wx*.2f+12,wy*.2f+14);
                float light=1f+rise*.65f-dx*rise*.55f+dy*rise*.25f;
                color*=light;
                if(radius>.83f&&dx>0)color*=.84f;
                if(rise>.22f&&ridge>.48f)
                    color=Color.Lerp(color,new Color(.24f,.26f,.20f)*(1-dx*.18f),rise*.6f);

            }
            float river=12-(wx+138)*.1f+4*Mathf.Sin((wx+138)/26);
            float distance=Mathf.Abs(wy-river),width=2.3f+.7f*Mathf.Sin(wx*.08f);
            if(distance<width+1.1f)color*=.69f;
            if(distance<width)
            {
                // Alterna a origem das pequenas ondas por faixa; evita emendas quadriculadas.
                int sy=(y+(x/37)*7)%water.height,sx=(x+(y/13)*11)%water.width;
                Color sample=wp[sy*water.width+sx];
                color=Color.Lerp(new Color(.055f,.14f,.15f),sample,.45f);
                if(distance>width-.45f)color*=.72f;color.a=1;
            }
            color.a=1;colors[y*w+x]=color;
        }
        var trees=new[]{Read("Art/World/Tree_A"),Read("Art/World/Tree_B")};var random=new System.Random(317);
        for(int i=0;i<1900;i++)
        {
            float wx=Mathf.Lerp(extent.x,extent.xMax,(float)random.NextDouble()),wy=Mathf.Lerp(9,extent.yMax,(float)random.NextDouble());
            float grove=Mathf.PerlinNoise(wx*.055f+7,wy*.055f+11);
            if(grove<.54f&&random.NextDouble()<.94)continue;
            bool field=wx<-25&&wy<40;if(field&&random.NextDouble()<.84)continue;
            bool inside=SantaRitaAtlas.Survey.regions.Exists(r=>!r.interior&&r.atlas.Contains(new Vector2(wx,wy)));
            if(inside)continue;
            Stamp(colors,w,h,trees[i%2],Mathf.RoundToInt((wx-extent.x)*ppu),Mathf.RoundToInt((wy-extent.y)*ppu),Mathf.Lerp(2.1f,5.3f,(float)random.NextDouble())*ppu);
        }
        var output=new Texture2D(w,h,TextureFormat.RGBA32,false);output.SetPixels32(colors);output.Apply();
        const string file="Assets/Resources/Atlas/Paisagem.png";
        var temporary="Library/SantaRitaPaisagem.png";File.WriteAllBytes(temporary,output.EncodeToPNG());
        if(File.Exists(file))File.Replace(temporary,file,"Library/SantaRitaPaisagemAnterior.png");else File.Move(temporary,file);
        Object.DestroyImmediate(output);Object.DestroyImmediate(grass);Object.DestroyImmediate(water);foreach(var t in trees)Object.DestroyImmediate(t);
    }
    private static void Stamp(Color32[] target,int w,int h,Texture2D tree,int cx,int cy,float width)
    {
        var source=tree.GetPixels32();int sw=Mathf.RoundToInt(width),sh=Mathf.RoundToInt(width*tree.height/tree.width);
        for(int y=0;y<sh;y++)for(int x=0;x<sw;x++)
        {
            int dx=cx+x-sw/2,dy=cy+y;if(dx<0||dx>=w||dy<0||dy>=h)continue;
            Color c=source[Mathf.Min(tree.height-1,y*tree.height/sh)*tree.width+Mathf.Min(tree.width-1,x*tree.width/sw)];if(c.a<.1f)continue;
            Color old=target[dy*w+dx];target[dy*w+dx]=Color.Lerp(old,c,c.a);
        }
    }
}

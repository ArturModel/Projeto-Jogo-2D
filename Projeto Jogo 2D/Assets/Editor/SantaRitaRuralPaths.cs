using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

// Acabamento de autoria: mantém eixos/acessos; desenha bordas de terra em pixels,
// sem transformar diagonais de um metro em uma escada de lajes de concreto.
public static class SantaRitaRuralPaths
{
    struct Stretch { public Vector2 a,b; public float radius; public Stretch(float ax,float ay,float bx,float by,float r){a=new Vector2(ax,ay);b=new Vector2(bx,by);radius=r;} }
    public static void Apply(Tilemap ground,Transform parent,Material material)
    {
        Paint("CaminhosBeiraRio",ground,parent,material,new[]{new Stretch(31,27,20,19,3),new Stretch(20,19,5,13,3),new Stretch(31,22,46,2,2)});
        Paint("CaminhoRuaNorte",ground,parent,material,new[]{new Stretch(31,81,25,84,2)});
    }
    static float Edge(Vector2 p,Stretch[] route)
    {
        float nearest=999;
        foreach(var s in route){var ab=s.b-s.a;float t=Mathf.Clamp01(Vector2.Dot(p-s.a,ab)/ab.sqrMagnitude);nearest=Mathf.Min(nearest,Vector2.Distance(p,s.a+ab*t)-s.radius);}
        return nearest;
    }
    static void Paint(string name,Tilemap ground,Transform parent,Material material,Stretch[] route)
    {
        Vector2 low=new Vector2(999,999),high=new Vector2(-999,-999);
        foreach(var s in route){low=Vector2.Min(low,Vector2.Min(s.a,s.b)-Vector2.one*(s.radius+2));high=Vector2.Max(high,Vector2.Max(s.a,s.b)+Vector2.one*(s.radius+2));}
        var area=new RectInt(Mathf.FloorToInt(low.x),Mathf.FloorToInt(low.y),Mathf.CeilToInt(high.x-low.x),Mathf.CeilToInt(high.y-low.y));
        var grass=AssetDatabase.LoadAssetAtPath<Tile>("Assets/Art/Capitulo2/Tiles/Grama.asset");
        foreach(var cell in area.allPositionsWithin)
        {
            var c=new Vector3Int(cell.x,cell.y,0);var tile=ground.GetTile(c);if(tile==null||Edge((Vector2)cell+Vector2.one*.5f,route)>1.5f)continue;
            if(tile.name.StartsWith("Terra")||tile.name.StartsWith("Calcada"))ground.SetTile(c,grass);
        }
        var source=Resources.Load<Texture2D>("Art/Macros/Terra_Macro");var pixels=source.GetPixels32();int width=area.width*32,height=area.height*32;var image=new Texture2D(width,height,TextureFormat.RGBA32,false);var output=new Color32[width*height];
        for(int y=0;y<height;y++)for(int x=0;x<width;x++)
        {
            var p=new Vector2(area.x+(x+.5f)/32,area.y+(y+.5f)/32);float edge=Edge(p,route);
            // Deixa o pavimento urbano e os patamares dos edifícios intactos.
            var tile=ground.GetTile(new Vector3Int(Mathf.FloorToInt(p.x),Mathf.FloorToInt(p.y),0));
            if(tile==null||(!tile.name.StartsWith("Grama")&&!tile.name.StartsWith("Terra")))continue;
            float grain=Mathf.PerlinNoise(p.x*5.3f,p.y*5.3f)*.28f+Mathf.PerlinNoise(p.x*18,p.y*18)*.08f;
            float alpha=1-Mathf.Clamp01((edge+grain+.05f)/.26f);if(alpha<=0)continue;
            int sx=((area.x*32+x)%source.width+source.width)%source.width,sy=((area.y*32+y)%source.height+source.height)%source.height;
            var color=pixels[sy*source.width+sx];color.a=(byte)(alpha*255);output[y*width+x]=color;
        }
        image.SetPixels32(output);image.Apply();string folder="Assets/Art/Producao/Caminhos";Directory.CreateDirectory(folder);string path=folder+"/"+name+".png";File.WriteAllBytes(path,image.EncodeToPNG());UnityEngine.Object.DestroyImmediate(image);AssetDatabase.ImportAsset(path);
        var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.spritePixelsPerUnit=32;
        var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);settings.spriteAlignment=(int)SpriteAlignment.Custom;settings.spritePivot=Vector2.zero;importer.SetTextureSettings(settings);
        importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=4096;importer.SaveAndReimport();
        var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.position=new Vector3(area.x,area.y,0);var sr=go.AddComponent<SpriteRenderer>();sr.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);sr.sharedMaterial=material;sr.sortingOrder=-2950;
    }
}

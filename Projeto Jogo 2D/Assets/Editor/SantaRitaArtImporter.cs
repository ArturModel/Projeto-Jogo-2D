using UnityEditor;
using UnityEngine;

public sealed class SantaRitaArtImporter : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith("Assets/Resources/Art/") && !assetPath.StartsWith("Assets/Art/Capitulo2/Npc_")) return;
        // Ref7 has raw atlases and exported floor pivots; its explicit authoring importer owns those settings.
        if(assetPath.StartsWith("Assets/Resources/Art/BarRef7/"))return;
        // Rua Norte uses explicit 64 PPU and foot pivots from its texture metadata.
        if (assetPath.StartsWith("Assets/Resources/Art/RuaNorte/")) return;
        if (!(assetImporter is TextureImporter importer)) return;
        importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
        importer.spritePixelsPerUnit=32;importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;
        importer.textureCompression=TextureImporterCompression.Uncompressed;importer.isReadable=true;
        var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);settings.spriteAlignment=(int)SpriteAlignment.Custom;
        settings.spritePivot=(assetPath.Contains("/Tiles/")||assetPath.EndsWith("/TapeteBar.png")) ? new Vector2(.5f,.5f) : new Vector2(.5f,.125f);
        importer.SetTextureSettings(settings);
    }
}

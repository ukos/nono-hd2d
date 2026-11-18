using UnityEditor;
using UnityEngine;

// Assets/Art/Sprites 以下に置いた画像へ、ドット絵用の取り込み設定を自動適用する。
// （試作仕様 §5：Point フィルタ、圧縮なし、Pivot は下中央）
public class SpriteImportSettings : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (!assetPath.Replace('\\', '/').Contains("/Art/Sprites/")) return;
        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        // Pivot 下中央（0.5, 0）はスプライト単位の設定のため、複数スプライト時は
        // Sprite Editor で各フレームに設定すること（README 参照）。
    }
}

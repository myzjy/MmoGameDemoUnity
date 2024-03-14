using System.Linq;
using UnityEditor;
using UnityEngine.U2D;

namespace ZJYFrameWork.SpriteViewer
{
    internal class AtlasAsset
    {
        public bool Favorite;
        public string Path;
        public SpriteAtlas SpriteAtlasAsset;

        public static AtlasAsset Create(SpriteAtlas spriteAtlas)
        {
            return new AtlasAsset(spriteAtlas);
        }

        private AtlasAsset(SpriteAtlas spriteAtlas)
        {
            SpriteAtlasAsset = spriteAtlas;
            Path = AssetDatabase.GetAssetPath(spriteAtlas);
            Favorite = IsFavorite(Path);
        }

        public static bool IsFavorite(SpriteAtlas spriteAtlas)
        {
            var path = AssetDatabase.GetAssetPath(spriteAtlas);
            return SpriteAssetViewerSetting.GetSetting().LocalSetting.favoriteSpriteAtlasPath
                .Any(atlasPath => atlasPath == path);
        }

        public static bool IsFavorite(string path)
        {
            return SpriteAssetViewerSetting.GetSetting().LocalSetting.favoriteSpriteAtlasPath
                .Any(atlasPath => atlasPath == path);
        }
    }
}
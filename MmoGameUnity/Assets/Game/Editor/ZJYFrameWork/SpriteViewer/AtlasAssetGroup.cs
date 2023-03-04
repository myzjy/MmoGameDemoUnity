using System.Collections.Generic;
using UnityEngine.U2D;

namespace ZJYFrameWork.SpriteViewer
{
    internal class AtlasAssetGroup
    {
        /// <summary>
        /// Folder path
        /// </summary>
        public string Path;

        /// <summary>
        /// FolderName
        /// </summary>
        public string GroupName;

        /// <summary>
        /// Atlas in the folder
        /// </summary>
        public List<AtlasAsset> SpriteAtlasAssetsList;

        /// <summary>
        /// Sub folders
        /// </summary>
        public List<AtlasAssetGroup> SubAtlasAssetGroups;

        public static AtlasAssetGroup Create(string path, SpriteAtlas[] spriteAtlas, bool includeNotFavorite = true)
        {
            AtlasAssetGroup atlasGroup = new AtlasAssetGroup(path);

            atlasGroup.SpriteAtlasAssetsList = new List<AtlasAsset>(spriteAtlas.Length);

            for (int i = 0; i < spriteAtlas.Length; i++)
            {
                if (includeNotFavorite || AtlasAsset.IsFavorite(spriteAtlas[i]))
                {
                    atlasGroup.SpriteAtlasAssetsList.Add(AtlasAsset.Create(spriteAtlas[i]));
                }
            }

            return atlasGroup;
        }

        private AtlasAssetGroup(string path)
        {
            Path = path;
            GroupName = SpriteAssetViewerUtility.GetFolderName(path);
        }

        public void AddSubAtlasAssetGroup(AtlasAssetGroup targetAtlasAssetGroup)
        {
            if (SubAtlasAssetGroups == null)
            {
                SubAtlasAssetGroups = new List<AtlasAssetGroup>(4);
            }
            SubAtlasAssetGroups.Add(targetAtlasAssetGroup);
        }
    }
}
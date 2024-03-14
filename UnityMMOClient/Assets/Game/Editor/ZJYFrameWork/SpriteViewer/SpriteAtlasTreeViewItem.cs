using UnityEditor.IMGUI.Controls;

namespace ZJYFrameWork.SpriteViewer
{
    internal class SpriteAtlasTreeViewItem : TreeViewItem
    {
        internal AtlasAsset AtlasAsset;

        public SpriteAtlasTreeViewItem() : base(-1, -1)
        {
        }

        public SpriteAtlasTreeViewItem(string folderPath, string folderName, int depth) : base(
            $"{folderPath}{folderName}".GetHashCode(), depth)
        {
            displayName = folderName;
        }

        public SpriteAtlasTreeViewItem(AtlasAsset atlasAsset, int depth) : base(
            $"{atlasAsset.Path}{atlasAsset.SpriteAtlasAsset.name}".GetHashCode(), depth)
        {
            AtlasAsset = atlasAsset;
            displayName = atlasAsset.SpriteAtlasAsset.name;
        }
    }
}
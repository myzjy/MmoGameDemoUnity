using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace ZJYFrameWork.SpriteViewer
{
    internal class SpriteAssetViewerGlobalSetting : ScriptableObject
    {
        public bool Packer2Atlas_IsCheckoutTag = true;

        public bool Packer2Atlas_IsTightPack = false;

        public TextureImporterCompression Packer2Atlas_CompressionSetting = TextureImporterCompression.Uncompressed;

        [FormerlySerializedAs("Packer2Atlas_IsAddAtlasAbString")]
        public bool packer2AtlasIsAddAtlasAbString = true;

        [FormerlySerializedAs("Packer2Atlas_IsAddAtlasContentsAbString")]
        public bool packer2AtlasIsAddAtlasContentsAbString = true;

        [FormerlySerializedAs("Atlas_CheckTightPackMessageType")]
        public MessageType atlasCheckTightPackMessageType = MessageType.None;

        [FormerlySerializedAs("Atlas_IsTightPack")]
        public bool atlasIsTightPack = AtlasDefaultTightPack;

        [FormerlySerializedAs("Atlas_CheckCompressionSettingMessageType")]
        public MessageType atlasCheckCompressionSettingMessageType = MessageType.None;

        [FormerlySerializedAs("Atlas_CompressionSetting")]
        public TextureImporterCompression atlasCompressionSetting = AtlasDefaultCompressionSetting;

        [FormerlySerializedAs("Atlas_CheckAddAtlasAbStringMessageType")]
        public MessageType atlasCheckAddAtlasAbStringMessageType = MessageType.Error;

        [FormerlySerializedAs("Atlas_IsAddAtlasAbString")]
        public bool atlasIsAddAtlasAbString = true;

        [FormerlySerializedAs("Atlas_CheckAddAtlasContentsAbStringMessageType")]
        public MessageType atlasCheckAddAtlasContentsAbStringMessageType = MessageType.Error;

        [FormerlySerializedAs("Atlas_IsAddAtlasContentsAbString")]
        public bool atlasIsAddAtlasContentsAbString = true;

        public const bool AtlasDefaultTightPack = true;
        public const TextureImporterCompression AtlasDefaultCompressionSetting = TextureImporterCompression.Compressed;

        [FormerlySerializedAs("AssetBundleFileExtension")]
        public string assetBundleFileExtension = "ab";

        /// <summary>
        /// Save - Relative Path Without BaseFolderName[Assets/]
        /// </summary>
        [FormerlySerializedAs("SpriteAtlasSubSavePath")]
        public string spriteAtlasSubSavePath = @"SpriteAtlas";

        /// <summary>
        /// Save - Relative Full Path
        /// </summary>
        public string SpriteAtlasSavePath => $"{SpriteAssetViewerSetting.FolderBasePath}{spriteAtlasSubSavePath}";

        public static SpriteAssetViewerGlobalSetting GetGlobalSetting()
        {
            if (!IsSettingExist())
            {
                CreateGlobalSetting();
            }

            return LoadGlobalSetting();
        }

        private static bool IsSettingExist()
        {
            return AssetDatabase.FindAssets("t:SpriteAssetViewerGlobalSetting").Length > 0;
        }

        private static void CreateGlobalSetting()
        {
            AssetDatabase.CreateAsset(CreateInstance<SpriteAssetViewerGlobalSetting>(),
                $"{SpriteAssetViewerSetting.FolderBasePath}Game/GlobalSetting/SpriteAssetViewerGlobalSetting.asset");
            AssetDatabase.Refresh();
        }

        private static SpriteAssetViewerGlobalSetting LoadGlobalSetting()
        {
            return (SpriteAssetViewerGlobalSetting)AssetDatabase
                .FindAssets("t:SpriteAssetViewerGlobalSetting")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(SpriteAssetViewerGlobalSetting)))
                .FirstOrDefault(obj => obj != null);
        }

        public static void SaveSetting(SpriteAssetViewerGlobalSetting setting)
        {
            EditorUtility.SetDirty(setting);
            AssetDatabase.SaveAssets();
        }
    }
}
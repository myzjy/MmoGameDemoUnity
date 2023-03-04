namespace ZJYFrameWork.SpriteViewer
{
    internal partial class SpriteAssetViewerSetting
    {
        public enum FolderType
        {
            AssetBundle = 0,
            Local
        }

        public const string FolderBasePath = "Assets/";

        public const string FolderLocalRoot = "Local";
        public const string FolderAssetBundleRoot = "AssetBundle";
        public readonly string[] FolderRoot = new string[2] { FolderAssetBundleRoot, FolderLocalRoot };
        private SpriteAssetViewerGlobalSetting globalSetting;

        public SpriteAssetViewerGlobalSetting GlobalSetting
        {
            get
            {
                if (!globalSetting)
                {
                    globalSetting = SpriteAssetViewerGlobalSetting.GetGlobalSetting();
                }

                return globalSetting;
            }
        }

        private SpriteAssetViewerLocalSetting localSetting;

        public SpriteAssetViewerLocalSetting LocalSetting
        {
            get { return localSetting ??= SpriteAssetViewerLocalSetting.GetLocalSettings(); }
        }

        private static SpriteAssetViewerSetting instance;

        public static SpriteAssetViewerSetting GetSetting()
        {
            return instance ??= new SpriteAssetViewerSetting();
        }
        private SpriteAssetViewerSetting()
        {
        }
        public string GetFolderSavePath(FolderType type)
        {
            return $"{GlobalSetting.SpriteAtlasSavePath}/{FolderRoot[(int)type]}";
        }
        public void Save(bool saveLocal = true, bool saveGlobal = true)
        {
            if (saveLocal)
            {
                SpriteAssetViewerLocalSetting.SaveSetting(LocalSetting);
            }
            if (saveGlobal)
            {
                SpriteAssetViewerGlobalSetting.SaveSetting(GlobalSetting);
            }
        }
        public static void Release()
        {
            instance.globalSetting = null;
            instance.localSetting = null;
            instance = null;
        }
    }
}
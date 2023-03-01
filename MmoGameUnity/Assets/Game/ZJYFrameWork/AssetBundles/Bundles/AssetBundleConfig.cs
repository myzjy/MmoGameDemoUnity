using UnityEditor;

namespace ZJYFrameWork.AssetBundles.AssetBundleToolsConfig
{
    public static class AssetBundleConfig
    {
        //后缀名
        public const string AssetBundleSuffix = ".assetbundle";
        private static int mIsEditorMode = -1;
        private static int mIsEditorLogMode = -1;
        private const string kIsEditorMode = "IsEditorMode";
        private const string kIsEditorModeLog = "IsEditorModeLog";
        private static int mIsSimulateMode = -1;
        private const string kIsSimulateMode = "IsSimulateMode";
        public const string BundleRoot = "AssetBundles";
        public const string ManifestFilename = "manifest.dat";
        public const string SpriteAtlasSuffix = ".spriteatlas";
        public const string SpriteAtlasABSuffix = "_spriteatlas.assetbundle";

#if UNITY_EDITOR

        public static bool IsEditorLogMode
        {
            get
            {
                if (mIsEditorLogMode == -1)
                {
                    if (!EditorPrefs.HasKey(kIsEditorModeLog))
                    {
                        EditorPrefs.SetBool(kIsEditorModeLog, false);
                    }

                    mIsEditorLogMode = EditorPrefs.GetBool(kIsEditorModeLog, true) ? 1 : 0;
                }

                return mIsEditorLogMode != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != mIsEditorLogMode)
                {
                    mIsEditorLogMode = newValue;
                    EditorPrefs.SetBool(kIsEditorModeLog, value);
                }
            }
        }

        public static bool IsEditorMode
        {
            get
            {
                if (mIsEditorMode == -1)
                {
                    if (!EditorPrefs.HasKey(kIsEditorMode))
                    {
                        EditorPrefs.SetBool(kIsEditorMode, false);
                    }

                    mIsEditorMode = EditorPrefs.GetBool(kIsEditorMode, true) ? 1 : 0;
                }

                return mIsEditorMode != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != mIsEditorMode)
                {
                    mIsEditorMode = newValue;
                    EditorPrefs.SetBool(kIsEditorMode, value);
                    if (value)
                    {
                        IsSimulateMode = false;
                    }
                }
            }
        }

        public static bool IsSimulateMode
        {
            get
            {
                if (mIsSimulateMode == -1)
                {
                    if (!EditorPrefs.HasKey(kIsSimulateMode))
                    {
                        EditorPrefs.SetBool(kIsSimulateMode, true);
                    }

                    mIsSimulateMode = EditorPrefs.GetBool(kIsSimulateMode, true) ? 1 : 0;
                }

                return mIsSimulateMode != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != mIsSimulateMode)
                {
                    mIsSimulateMode = newValue;
                    EditorPrefs.SetBool(kIsSimulateMode, value);

                    if (value)
                    {
                        IsEditorMode = false;
                    }
                }
            }
        }
#endif
    }
}
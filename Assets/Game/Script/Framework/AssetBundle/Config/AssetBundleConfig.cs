using System.IO;
using Common.Utility;
using UnityEditor;
using  UnityEngine;

namespace Framework.AssetBundles.Config
{
    public static class AssetBundleConfig
    {
        public const string AssetsFolderName = "AddressablePaker";
        public const string AssetsFolderNameS = "Game/AddressablePaker";
        public const string AssetBundlesFolderName = "AssetBundles";
        public const string AssetsPathMapFileName = "AssetsMap";//"AssetsMap.bytes";
        public const string VariantsMapFileName = "VariantsMap.bytes";
        public const string ChannelFolderName = "Channel";
        public const string localSvrAppPath = "Editor/AssetBundle/LocalServer/AssetBundleServer.exe";

        //后缀名
        public const string AssetBundleSuffix = ".assetbundle";
        private static int mIsEditorMode = -1;
        private const string kIsEditorMode = "IsEditorMode";
        private static int mIsSimulateMode = -1;
        private const string kIsSimulateMode = "IsSimulateMode";
        public const string CommonMapPattren = ",";
        public const string AssetBundleServerUrlFileName = "AssetBundleServerUrl.txt";
#if UNITY_EDITOR
                
        public static string LocalSvrAppWorkPath
        {
            get
            {
                return AssetBundlesBuildOutputPath;
            }
        }
        
        public static string AssetBundlesBuildOutputPath
        {
            get
            {
                string outputPath = Path.Combine(System.Environment.CurrentDirectory, AssetBundlesFolderName);
                GameUtility.CheckDirAndCreateWhenNeeded(outputPath);
                return outputPath;
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
        public static string LocalSvrAppPath
        {
            get
            {
                return Path.Combine(Application.dataPath, localSvrAppPath);
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AssetBundles;
using Common.Utility;
using Framework.AssetBundles.Config;
using UnityEditor;
using UnityEngine;

namespace AssetBundleEditorTools.AssetBundleSet
{
    // unity editor启动和运行时调用静态构造函数
    [InitializeOnLoad]
    public class AssetBundleMenuItems
    {
        //%:ctrl,#:shift,&:alt
        const string kSimulateMode = "AssetBundles/Switch Model/Simulate Mode";
        const string kEditorMode = "AssetBundles/Switch Model/Editor Mode";
        const string kToolRunAllCheckers = "AssetBundles/Run All Checkers";
        const string kToolBuildForCurrentSetting = "AssetBundles/Build For Current Setting";
        const string kToolsCopyAssetbundles = "AssetBundles/Copy To StreamingAssets";
        const string kToolsOpenOutput = "AssetBundles/Open Current Output";
        const string kToolsOpenPerisitentData = "AssetBundles/Open PersistentData";
        const string kToolsClearOutput = "AssetBundles/Clear Current Output";
        const string kToolsClearStreamingAssets = "AssetBundles/Clear StreamingAssets";
        const string kToolsClearPersistentAssets = "AssetBundles/Clear PersistentData";

        const string kCreateAssetbundleForCurrent = "Assets/AssetBundles/Create Assetbundle For Current &#z";
        const string kCreateAssetbundleForFile = "Assets/AssetBundles/SetDefault without extension Name ";
        const string kCreateAssetbundleForChildren = "Assets/AssetBundles/Create Assetbundle For Children &#x";
        const string kAssetDependencis = "Assets/AssetBundles/Asset Dependencis &#h";
        const string kAssetbundleAllDependencis = "Assets/AssetBundles/Assetbundle All Dependencis &#j";
        const string kAssetbundleDirectDependencis = "Assets/AssetBundles/Assetbundle Direct Dependencis &#k";

        static AssetBundleMenuItems()
        {
            CheckSimulateModelEnv();
        }

        static void CheckSimulateModelEnv()
        {
            if (!AssetBundleConfig.IsSimulateMode)
            {
                return;
            }

            var buildTargetName = PackageUtils.GetCurPlatformName();
            var channelName = PackageUtils.GetCurSelectedChannel().ToString();
            var outputManifest = PackageUtils.GetCurBuildSettingAssetBundleManifestPath();
            bool hasBuildAssetBundles = false;
            if (!File.Exists(outputManifest))
            {
                bool checkBuild = EditorUtility.DisplayDialog("Build AssetBundles Warning",
                    $"Build AssetBundles for : \n\nplatform : {buildTargetName} \nchannel : {channelName} \n\nContinue ?",
                    "Confirm", "Cancel");
                if (!checkBuild)
                {
                    ToggleEditorMode();
                    return;
                }

                hasBuildAssetBundles = true;
                BuildPlayer.BuildAssetBundlesForCurSetting();
            }

            var streamingManifest = PackageUtils.GetCurBuildSettingStreamingManifestPath();
            if (hasBuildAssetBundles || !File.Exists(streamingManifest))
            {
                bool checkCopy = EditorUtility.DisplayDialog("Copy AssetBundles To StreamingAssets Warning",
                    $"Copy AssetBundles to streamingAssets folder for : \n\nplatform : {buildTargetName} \nchannel : {channelName} \n\nContinue ?",
                    "Confirm", "Cancel");
                if (!checkCopy)
                {
                    ToggleEditorMode();
                    return;
                }

                // 拷贝到StreamingAssets目录时，相当于执行大版本更新，那么沙盒目录下的数据就作废了
                // 真机上会对比这两个目录下的App版本号来删除，编辑器下暴力一点，直接删除
                ToolsClearPersistentAssets();
                PackageUtils.CopyCurSettingAssetBundlesToStreamingAssets();
            }

            LaunchAssetBundleServer.CheckAndDoRunning();
        }

        [MenuItem(kEditorMode, false)]
        public static void ToggleEditorMode()
        {
            if (!AssetBundleConfig.IsSimulateMode) return;
            AssetBundleConfig.IsEditorMode = true;
            LaunchAssetBundleServer.CheckAndDoRunning();
        }

        [MenuItem(kEditorMode, true)]
        public static bool ToggleEditorModeValidate()
        {
            Menu.SetChecked(kEditorMode, AssetBundleConfig.IsEditorMode);
            return true;
        }

        [MenuItem(kSimulateMode)]
        public static void ToggleSimulateMode()
        {
            if (!AssetBundleConfig.IsEditorMode) return;
            AssetBundleConfig.IsSimulateMode = true;
            CheckSimulateModelEnv();
            LaunchAssetBundleServer.CheckAndDoRunning();
        }

        [MenuItem(kSimulateMode, true)]
        public static bool ToggleSimulateModeValidate()
        {
            Menu.SetChecked(kSimulateMode, AssetBundleConfig.IsSimulateMode);
            return true;
        }

        [MenuItem(kToolRunAllCheckers)]
        public static void ToolRunAllCheckers()
        {
            var buildTargetName = PackageUtils.GetCurPlatformName();
            var channelName = PackageUtils.GetCurSelectedChannel().ToString();
            bool checkCopy = EditorUtility.DisplayDialog("Run Checkers Warning",
                $"Run Checkers for : \n\nplatform : {buildTargetName} \nchannel : {channelName} \n\nContinue ?",
                "Confirm", "Cancel");
            if (!checkCopy)
            {
                return;
            }

            bool checkChannel = PackageUtils.BuildAssetBundlesForPerChannel(EditorUserBuildSettings.activeBuildTarget);
            PackageUtils.CheckAndRunAllCheckers(checkChannel, true);
        }

        [MenuItem(kToolBuildForCurrentSetting, false, 1100)]
        public static void ToolBuildForCurrentSetting()
        {
            var buildTargetName = PackageUtils.GetCurPlatformName();
            var channelName = PackageUtils.GetCurSelectedChannel().ToString();
            bool checkCopy = EditorUtility.DisplayDialog("Build AssetBundles Warning",
                $"Build AssetBundles for : \n\nplatform : {buildTargetName} \nchannel : {channelName} \n\nContinue ?",
                "Confirm", "Cancel");
            if (!checkCopy)
            {
                return;
            }

            PackageTool.BuildAssetBundlesForCurrentChannel();
        }

        [MenuItem(kToolsCopyAssetbundles, false, 1101)]
        public static void ToolsCopyAssetbundles()
        {
            var buildTargetName = PackageUtils.GetCurPlatformName();
            var channelName = PackageUtils.GetCurSelectedChannel().ToString();
            bool checkCopy = EditorUtility.DisplayDialog("Copy AssetBundles To StreamingAssets Warning",
                $"Copy AssetBundles to streamingAssets folder for : \n\nplatform : {buildTargetName} \nchannel : {channelName} \n\nContinue ?",
                "Confirm", "Cancel");
            if (!checkCopy)
            {
                return;
            }

            // 拷贝到StreamingAssets目录时，相当于执行大版本更新，那么沙盒目录下的数据就作废了
            // 真机上会对比这两个目录下的App版本号来删除，编辑器下暴力一点，直接删除
            ToolsClearPersistentAssets();
            PackageUtils.CopyCurSettingAssetBundlesToStreamingAssets();
            LaunchAssetBundleServer.CheckAndDoRunning();
        }

        [MenuItem(kToolsOpenOutput, false, 1201)]
        public static void ToolsOpenOutput()
        {
            string outputPath = PackageUtils.GetCurBuildSettingAssetBundleOutputPath();
            EditorUtils.ExplorerFolder(outputPath);
        }

        [MenuItem(kToolsOpenPerisitentData, false, 1202)]
        public static void ToolsOpenPerisitentData()
        {
            EditorUtils.ExplorerFolder(Application.persistentDataPath);
        }

        [MenuItem(kToolsClearOutput, false, 1302)]
        public static void ToolsClearOutput()
        {
            var buildTargetName = PackageUtils.GetCurPlatformName();
            var channelName = PackageUtils.GetCurSelectedChannel().ToString();
            bool checkClear = EditorUtility.DisplayDialog("ClearOutput Warning",
                $"Clear output assetbundles will force to rebuild all : \n\nplatform : {buildTargetName} \nchannel : {channelName} \n\n continue ?",
                "Yes", "No");
            if (!checkClear)
            {
                return;
            }

            string outputPath = PackageUtils.GetCurBuildSettingAssetBundleOutputPath();
            GameUtility.SafeDeleteDir(outputPath);
            Debug.Log($"Clear done : {outputPath}");
        }

        [MenuItem(kToolsClearStreamingAssets, false, 1303)]
        public static void ToolsClearStreamingAssets()
        {
            bool checkClear = EditorUtility.DisplayDialog("ClearStreamingAssets Warning",
                "Clear streaming assets assetbundles will lost the latest player build info, continue ?",
                "Yes", "No");
            if (!checkClear)
            {
                return;
            }

            string outputPath = Path.Combine(Application.streamingAssetsPath, AssetBundleConfig.AssetBundlesFolderName);
            GameUtility.SafeClearDir(outputPath);
            AssetDatabase.Refresh();
            Debug.Log($"Clear {PackageUtils.GetCurPlatformName()} assetbundle streaming assets done!");
        }

        [MenuItem(kToolsClearPersistentAssets, false, 1301)]
        public static void ToolsClearPersistentAssets()
        {
            bool checkClear = EditorUtility.DisplayDialog("ClearPersistentAssets Warning",
                "Clear persistent assetbundles will force to update all assetbundles that difference with streaming assets assetbundles, continue ?",
                "Yes", "No");
            if (!checkClear)
            {
                return;
            }

            string outputPath = Path.Combine(Application.persistentDataPath, AssetBundleConfig.AssetBundlesFolderName);
            GameUtility.SafeDeleteDir(outputPath);
            Debug.Log($"Clear {PackageUtils.GetCurPlatformName()} assetbundle persistent assets done!");
        }

        [MenuItem(kCreateAssetbundleForFile)]
        public static void SetDefaultWithoutExtensionName()
        {
            //有没有选择
            if (!AssetBundleEditorHelper.HasValidSelection())
                return;
            var selectObjs = Selection.objects;
            AssetBundleEditorHelper.CreaeteAssetBundleForFile(selectObjs);
        }


        [MenuItem(kCreateAssetbundleForCurrent)]
        public static void CreateAssetbundleForCurrent()
        {
            if (!AssetBundleEditorHelper.HasValidSelection()) return;
            bool checkCreate = EditorUtility.DisplayDialog("CreateAssetbundleForCurrent Warning",
                "Create assetbundle for cur selected objects will reset assetbundles which contains this dir, continue ?",
                "Yes", "No");
            if (!checkCreate)
            {
                return;
            }

            var selObjs = Selection.objects;
            AssetBundleEditorHelper.CreateAssetbundleForCurrent(selObjs);
            var removeList = AssetBundleEditorHelper.RemoveAssetbundleInParents(selObjs);
            removeList.AddRange(AssetBundleEditorHelper.RemoveAssetbundleInChildren(selObjs));
            var removeStr = new StringBuilder(10);
            int i = 0;
            foreach (string str in removeList)
            {
                removeStr.Append($"[{++i}]{str}\n");
            }

            if (removeList.Count > 0)
            {
                Debug.Log("CreateAssetbundleForCurrent done!\nRemove list :" +
                          "\n-------------------------------------------\n" + $"{removeStr}" +
                          "\n-------------------------------------------\n");
            }
        }

        [MenuItem(kCreateAssetbundleForChildren)]
        public static void CreateAssetbundleForChildren()
        {
            if (!AssetBundleEditorHelper.HasValidSelection()) return;
            bool checkCreate = EditorUtility.DisplayDialog("CreateAssetbundleForChildren Warning",
                "Create assetbundle for all children of cur selected objects will reset assetbundles which contains this dir, continue ?",
                "Yes", "No");
            if (!checkCreate)
            {
                return;
            }

            var selObjs = Selection.objects;
            AssetBundleEditorHelper.CreateAssetbundleForChildren(selObjs);
            var removeList = AssetBundleEditorHelper.RemoveAssetbundleInParents(selObjs);
            removeList.AddRange(AssetBundleEditorHelper.RemoveAssetbundleInChildren(selObjs, true, false));
            var removeStr = new StringBuilder(10);
            int i = 0;
            foreach (string str in removeList)
            {
                removeStr.Append($"[{++i}]{str}\n");
            }

            if (removeList.Count > 0)
            {
                Debug.Log("CreateAssetbundleForChildren done!\nRemove list :" +
                          "\n-------------------------------------------\n" + $"{removeStr}" +
                          "\n-------------------------------------------\n");
            }
        }

        [MenuItem(kAssetDependencis)]
        public static void ListAssetDependencis()
        {
            if (!AssetBundleEditorHelper.HasValidSelection()) return;
            var selObjs = Selection.objects;
            string depsStr = AssetBundleEditorHelper.GetDependencyText(selObjs, false);
            var selStr = new StringBuilder(10);
            int i = 0;
            foreach (var obj in selObjs)
            {
                selStr.Append($"[{++i}]{AssetDatabase.GetAssetPath(obj)};");
            }

            Debug.Log($"Selection({selStr.ToString()}) depends on the following assets:" +
                      "\n-------------------------------------------\n" + $"{depsStr}" +
                      "\n-------------------------------------------\n");
            AssetBundleEditorHelper.SelectDependency(selObjs, false);
        }

        [MenuItem(kAssetbundleAllDependencis)]
        public static void ListAssetbundleAllDependencis()
        {
            ListAssetbundleDependencis(true);
        }

        [MenuItem(kAssetbundleDirectDependencis)]
        public static void ListAssetbundleDirectDependencis()
        {
            ListAssetbundleDependencis(false);
        }

        private static void ListAssetbundleDependencis(bool isAll)
        {
            if (!AssetBundleEditorHelper.HasValidSelection()) return;
            string localFilePath = PackageUtils.GetCurBuildSettingAssetBundleManifestPath();

            var selObjs = Selection.objects;
            var depsList = AssetBundleEditorHelper.GetDependancisFormBuildManifest(localFilePath, selObjs, isAll);
            if (depsList == null)
            {
                return;
            }

            depsList.Sort();
            string depsStr = string.Empty;
            int i = 0;
            depsStr = depsList.Aggregate(depsStr, (current, str) => current + $"[{++i}]{str}\n");

            string selStr = string.Empty;
            i = 0;
            selStr = selObjs.Aggregate(selStr,
                (current, obj) => current + $"[{++i}]{AssetDatabase.GetAssetPath(obj)};");

            Debug.Log($"Selection({selStr}) directly depends on the following assetbundles:" +
                      "\n-------------------------------------------\n" + $"{depsStr}" +
                      "\n-------------------------------------------\n");
        }
    }
}
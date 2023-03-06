using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AssetBundles;
using UnityEditor;
using UnityEngine;
using ZJYFrameWork.AssetBundles.AssetBundleToolsConfig;

namespace AssetBundleEditorTools.AssetBundleSet
{
    // unity editor启动和运行时调用静态构造函数
    [InitializeOnLoad]
    public class AssetBundleMenuItems
    {
        //%:ctrl,#:shift,&:alt
        const string kSimulateMode = "Tools/AssetBundles/Switch Model/Simulate Mode";
        const string kSimulateModeLog = "Tools/AssetBundles/Switch Model/Download Log Mod";

        const string kEditorMode = "Tools/AssetBundles/Switch Model/Editor Mode";
        // const string kToolRunAllCheckers = "AssetBundles/Run All Checkers";
        // const string kToolBuildForCurrentSetting = "AssetBundles/Build For Current Setting";
        // const string kToolsCopyAssetbundles = "AssetBundles/Copy To StreamingAssets";
        // const string kToolsOpenOutput = "AssetBundles/Open Current Output";
        // const string kToolsOpenPerisitentData = "AssetBundles/Open PersistentData";
        // const string kToolsClearOutput = "AssetBundles/Clear Current Output";
        // const string kToolsClearStreamingAssets = "AssetBundles/Clear StreamingAssets";
        // const string kToolsClearPersistentAssets = "AssetBundles/Clear PersistentData";

        const string kCreateAssetbundleForCurrent = "Assets/AssetBundles/Create Assetbundle For Current &#z";
        const string kCreateAssetbundleForFile = "Assets/AssetBundles/SetDefault without extension Name ";
        const string kCreateAssetbundleNoeForFile = "Assets/AssetBundles/SetDefault without None Name ";
        const string kAssetDependencis = "Assets/AssetBundles/Asset Dependencis &#h";
        const string kAssetbundleAllDependencis = "Assets/AssetBundles/Assetbundle All Dependencis &#j";
        const string kAssetbundleDirectDependencis = "Assets/AssetBundles/Assetbundle Direct Dependencis &#k";
        public const string OpenABNameAssetListWindows = "Tools/AssetBundles/ABName and Named asset list";
        static AssetBundleMenuItems()
        {
        }


        [MenuItem(kEditorMode, false)]
        public static void ToggleEditorMode()
        {
            if (!AssetBundleConfig.IsSimulateMode) return;
            AssetBundleConfig.IsEditorMode = true;
        }

        [MenuItem(kEditorMode, true)]
        public static bool ToggleEditorModeValidate()
        {
            Menu.SetChecked(kEditorMode, AssetBundleConfig.IsEditorMode);
            return true;
        }

        [MenuItem(kSimulateModeLog)]
        public static void ToggleSimulateLogMode()
        {
            if (!AssetBundleConfig.IsEditorLogMode) return;
            AssetBundleConfig.IsEditorLogMode = true;

            // LaunchAssetBundleServer.CheckAndDoRunning();
        }

        [MenuItem(kSimulateMode)]
        public static void ToggleSimulateMode()
        {
            if (!AssetBundleConfig.IsEditorMode) return;
            AssetBundleConfig.IsSimulateMode = true;
        }

        [MenuItem(kSimulateMode, true)]
        public static bool ToggleSimulateModeValidate()
        {
            Menu.SetChecked(kSimulateMode, AssetBundleConfig.IsSimulateMode);
            return true;
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

        [MenuItem(kCreateAssetbundleNoeForFile)]
        public static void SetDefaultWithoutNoneExtensionName()
        {
            //有没有选择
            if (!AssetBundleEditorHelper.HasValidSelection())
                return;
            var selectObjs = Selection.objects;
            AssetBundleEditorHelper.CreaeteAssetBundleForFile(selectObjs);
        }
        //
        // [MenuItem(kCreateAssetbundleForCurrent)]
        // public static void CreateAssetbundleForCurrent()
        // {
        //     if (!AssetBundleEditorHelper.HasValidSelection()) return;
        //     bool checkCreate = EditorUtility.DisplayDialog("CreateAssetbundleForCurrent Warning",
        //         "Create assetbundle for cur selected objects will reset assetbundles which contains this dir, continue ?",
        //         "Yes", "No");
        //     if (!checkCreate)
        //     {
        //         return;
        //     }
        //
        //     var selObjs = Selection.objects;
        //     AssetBundleEditorHelper.CreateAssetbundleForCurrent(selObjs);
        //     var removeList = AssetBundleEditorHelper.RemoveAssetbundleInParents(selObjs);
        //     removeList.AddRange(AssetBundleEditorHelper.RemoveAssetbundleInChildren(selObjs));
        //     var removeStr = new StringBuilder(10);
        //     int i = 0;
        //     foreach (string str in removeList)
        //     {
        //         removeStr.Append($"[{++i}]{str}\n");
        //     }
        //
        //     if (removeList.Count > 0)
        //     {
        //         UnityEngine.Debug.Log("CreateAssetbundleForCurrent done!\nRemove list :" +
        //                               "\n-------------------------------------------\n" + $"{removeStr}" +
        //                               "\n-------------------------------------------\n");
        //     }
        // }
        //
        // [MenuItem(kCreateAssetbundleForChildren)]
        // public static void CreateAssetbundleForChildren()
        // {
        //     if (!AssetBundleEditorHelper.HasValidSelection()) return;
        //     bool checkCreate = EditorUtility.DisplayDialog("CreateAssetbundleForChildren Warning",
        //         "Create assetbundle for all children of cur selected objects will reset assetbundles which contains this dir, continue ?",
        //         "Yes", "No");
        //     if (!checkCreate)
        //     {
        //         return;
        //     }
        //
        //     var selObjs = Selection.objects;
        //     AssetBundleEditorHelper.CreateAssetbundleForChildren(selObjs);
        //     var removeList = AssetBundleEditorHelper.RemoveAssetbundleInParents(selObjs);
        //     removeList.AddRange(AssetBundleEditorHelper.RemoveAssetbundleInChildren(selObjs, true, false));
        //     var removeStr = new StringBuilder(10);
        //     int i = 0;
        //     foreach (string str in removeList)
        //     {
        //         removeStr.Append($"[{++i}]{str}\n");
        //     }
        //
        //     if (removeList.Count > 0)
        //     {
        //         UnityEngine.Debug.Log("CreateAssetbundleForChildren done!\nRemove list :" +
        //                               "\n-------------------------------------------\n" + $"{removeStr}" +
        //                               "\n-------------------------------------------\n");
        //     }
        // }

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

            UnityEngine.Debug.Log($"Selection({selStr.ToString()}) depends on the following assets:" +
                                  "\n-------------------------------------------\n" + $"{depsStr}" +
                                  "\n-------------------------------------------\n");
            AssetBundleEditorHelper.SelectDependency(selObjs, false);
        }

        // [MenuItem(kAssetbundleAllDependencis)]
        // public static void ListAssetbundleAllDependencis()
        // {
        //     ListAssetbundleDependencis(true);
        // }
        //
        // [MenuItem(kAssetbundleDirectDependencis)]
        // public static void ListAssetbundleDirectDependencis()
        // {
        //     ListAssetbundleDependencis(false);
        // }

        // private static void ListAssetbundleDependencis(bool isAll)
        // {
        //     if (!AssetBundleEditorHelper.HasValidSelection()) return;
        //     string localFilePath = PackageUtils.GetCurBuildSettingAssetBundleManifestPath();
        //
        //     var selObjs = Selection.objects;
        //     var depsList = AssetBundleEditorHelper.GetDependancisFormBuildManifest(localFilePath, selObjs, isAll);
        //     if (depsList == null)
        //     {
        //         return;
        //     }
        //
        //     depsList.Sort();
        //     string depsStr = string.Empty;
        //     int i = 0;
        //     depsStr = depsList.Aggregate(depsStr, (current, str) => current + $"[{++i}]{str}\n");
        //
        //     string selStr = string.Empty;
        //     i = 0;
        //     selStr = selObjs.Aggregate(selStr,
        //         (current, obj) => current + $"[{++i}]{AssetDatabase.GetAssetPath(obj)};");
        //
        //     UnityEngine.Debug.Log($"Selection({selStr}) directly depends on the following assetbundles:" +
        //                           "\n-------------------------------------------\n" + $"{depsStr}" +
        //                           "\n-------------------------------------------\n");
        // }
    }
}
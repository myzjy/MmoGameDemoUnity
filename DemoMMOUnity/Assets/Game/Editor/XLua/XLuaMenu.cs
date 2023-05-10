using System.IO;
using UnityEditor;
using UnityEngine;
using ZJYFrameWork.AssetBundles.AssetBundleToolsConfig;

namespace ZJYFrameWorkEditor.XLuaEditor
{
    [InitializeOnLoad]
    public static class XLuaMenu
    {
        [MenuItem("XLua/Copy Lua Files To AssetsPackage", false, 51)]
        public static void CopyLuaFilesToAssetsPackage()
        {
            string destination = Path.Combine(Application.dataPath, AssetBundleConfig.AssetsFolderName);
            Debug.Log($"destination Path :{destination}");
            destination = Path.Combine(Application.dataPath, AssetBundleConfig.luaAssetbundleAssetName);
            Debug.Log($"最后：{destination}");
            string source = Path.Combine(Application.dataPath, $"../{AssetBundleConfig.luaAssetbundleAssetName}");
            Debug.Log($"source:{source}");
            DirectoryInfo dirInfo = new DirectoryInfo(source);
            Debug.Log($"dirInfo:{dirInfo.FullName}");
            source = dirInfo.FullName;
            Debug.Log($"source:{source}");
            Util.SafeDeleteDir(destination);
            FileUtil.CopyFileOrDirectoryFollowSymlinks(source, destination);
            Debug.Log("Copies the file or directory");
        }
    }
}
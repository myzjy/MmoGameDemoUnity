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
            string destination = Path.Combine(Application.dataPath,
                $"{AssetBundleConfig.AssetsFolderName}/{AssetBundleConfig.luaAssetbundleAssetName}/");
            Debug.Log($"destination Path :{destination}");
            DirectoryInfo destinationDir = new DirectoryInfo(destination);
            Debug.Log($"destinationDir Path :{destinationDir.FullName}");
            destination = destinationDir.FullName;
            string source = Path.Combine(Application.dataPath, $"../{AssetBundleConfig.luaAssetbundleAssetName}");
            Debug.Log($"source:{source}");
            DirectoryInfo dirInfo = new DirectoryInfo(source);
            Debug.Log($"dirInfo:{dirInfo.FullName}");
            source = dirInfo.FullName;
            Debug.Log($"source:{source}");
            Util.SafeDeleteDir(destination);
            FileUtil.CopyFileOrDirectoryFollowSymlinks(source, destination);
            Debug.Log("Copies the file or directory");
            var notLuaFiles = Util.GetSpecifyFilesInFolder(destination, new string[] { ".lua" }, true);
            if (notLuaFiles is { Length: > 0 })
            {
                foreach (var t in notLuaFiles)
                {
                    Debug.Log($"file：{t}");
                    Util.SafeDeleteFile(t);
                }
            }

            var luaFiles = Util.GetSpecifyFilesInFolder(destination, new string[] { ".lua" }, false);
            if (luaFiles is { Length: > 0 })
            {
                foreach (var t in luaFiles)
                {
                    Util.SafeRenameFile(t, t + ".bytes");
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("Copy lua files over");
        }
    }
}
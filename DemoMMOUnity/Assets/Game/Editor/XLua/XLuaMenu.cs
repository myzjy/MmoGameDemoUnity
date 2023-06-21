using System;
using System.IO;
using GameUtil;
using UnityEditor;
using UnityEngine;
using ZJYFrameWork.AssetBundles.AssetBundleToolsConfig;
using Object = UnityEngine.Object;

namespace ZJYFrameWorkEditor.XLuaEditor
{
    [InitializeOnLoad]
    public static class XLuaMenu
    {
        [MenuItem("Tools/FileDelete")]
        public static void DeleteMacErrorFile()
        {
            var notLuaFiles = Util.GetSpecifyFilesInFolder(Application.dataPath, true);
            if (notLuaFiles is { Length: > 0 })
            {
                foreach (var t in notLuaFiles)
                {
                    Debug.Log($"file：{t}");
                    Util.SafeDeleteFile(t);
                }
            }
            string source = Path.Combine(Application.dataPath, $"../{AssetBundleConfig.luaAssetbundleAssetName}");
            Debug.Log($"source:{source}");
            DirectoryInfo dirInfo = new DirectoryInfo(source);
            Debug.Log($"dirInfo:{dirInfo.FullName}");
            source = dirInfo.FullName;
            var notLuaFile = Util.GetSpecifyFilesInFolder(source, true);
            if (notLuaFile is { Length: > 0 })
            {
                foreach (var t in notLuaFile)
                {
                    Debug.Log($"file：{t}");
                    Util.SafeDeleteFile(t);
                }
            }
            XLuaAssetBundle();
        }


        [MenuItem("XLua/Copy Lua Files To AssetsPackage", false, 51)]
        public static void CopyLuaFilesToAssetsPackage()
        {
            DeleteMacErrorFile();
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

        [MenuItem("Tools/XLua/XLuaAssetBundle")]
        public static void XLuaAssetBundle()
        {
            // DeleteMacErrorFile();
            string destination = Path.Combine(Application.dataPath,
                $"{AssetBundleConfig.AssetsFolderName}/{AssetBundleConfig.luaAssetbundleAssetName}/");
            Debug.Log($"destination Path :{destination}");
            DirectoryInfo destinationDir = new DirectoryInfo(destination);
            Debug.Log($"destinationDir Path :{destinationDir.FullName}");
            destination = destinationDir.FullName;
            // var index = destination.IndexOf("Assets", StringComparison.Ordinal);
            // Debug.Log($"出现下标位置：{index},destination length:{destination.Length}");
            // Debug.Log($"出现下标位置：{index},截取字符：{destination.Substring(index)}");
            // destination = destination.Substring(index);
            Debug.Log($"截取字符 destinationDir Path :{destination}");
            var luaFiles = Util.GetSpecifyFilesInFolder(destination, new string[] { ".lua" }, true);
            foreach (var file in luaFiles)
            {
                if (file.EndsWith(".meta"))
                {
                    continue;
                }
                var index = file.IndexOf("Assets", StringComparison.Ordinal);
                Debug.Log($"出现下标位置：{index},destination length:{destination.Length}");
                Debug.Log($"出现下标位置：{index},截取字符：{file.Substring(index)}");
                var fileIndexString = file.Substring(index);
                var obj = AssetDatabase.LoadAssetAtPath<Object>(fileIndexString);
                Debug.Log($"{obj},{obj.name}");
                string assetPath = AssetDatabase.GetAssetPath(obj);
                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                Debug.Log($"importer.assetPath--->{importer.assetPath}");
                string assetBundleName = Path.GetFileName(assetPath);
                Debug.Log($"assetBundleName-->{assetBundleName}");
                importer.assetBundleName = "xlua.assetbundle";
                Debug.Log(assetPath);
                Debug.Log(importer.assetBundleName);
                EditorUtility.SetDirty(obj);
            }
            AssetDatabase.Refresh();
        }
    }
}
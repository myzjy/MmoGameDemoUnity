﻿using System;
using System.IO;
using System.Text;
using Common.Utility;
using Framework.AssetBundles.Config;
using UnityEngine;

namespace Framework.AssetBundles.Utilty
{
    public class ErrorInfo
    {
        public enum Type
        {
            ///暂停
            TIME_OUT = 0,

            ///WWW请求错误
            FAILED_WWW_REQUEST = 1,

            ///写入错误
            FAILED_WRITE_CACHEFILE = 2,

            ///读入错误
            FAILED_READ_CACHEFILE = 3,

            ///删除错误
            FAILED_DELETE_CACHEFILE = 4,

            ///文件夹创建错误
            FAILED_CREATE_DIRECTORY = 5,

            ///容量错误
            FAILED_DISK_FULL = 6,

            ///加载的捆绑的读入错误
            DOWNLOAD_BUNDLE_INVALID = 7,

            ///原因不明的错误
            UNKNOWN = 8,

            ///读取中断
            ABORT_LOADING_REQUSET = 9,

            ///加载的捆绑的读入错误
            CACHEFILE_BUNDLE_INVALID = 10,
        }

        public Type type;
        public string message;
        public string abError;
    }

    public enum Priority
    {
        /// <summary>
        /// 高
        /// </summary>
        High,

        /// <summary>
        /// 中
        /// </summary>
        Normal,

        /// <summary>
        /// 低
        /// </summary>
        Low,
    }

    public struct UnloadInfo
    {
        public UnityEngine.AssetBundle assetBundle;
        public uint key;
        public bool unloadAllLoadedObjects;
    }

    public class AssetBundleUtility
    {
        /// <summary>
        /// 附在URL后面的各平台前缀
        /// </summary>
        /// <returns></returns>
        public static string GetPlatformName()
        {
#if UNITY_ANDROID
            return "android/";
#elif UNITY_IOS
            return "ios/";
#else
            return "pc/";
#endif
        }

        /// <summary>
        /// 包路径到资产路径
        /// </summary>
        /// <param name="assetPath">资产路径</param>
        /// <returns></returns>
        public static string PackagePathToAssetsPath(string assetPath)
        {
            var sb = new StringBuilder();
            sb.Append("Assets/");
            sb.Append(AssetBundleConfig.AssetsFolderName);
            sb.Append($"/{assetPath}");
            return sb.ToString();
        }

        /// <summary>
        /// 资产包路径到资产包名称
        /// </summary>
        /// <param name="assetPath">资产包路径</param>
        /// <returns></returns>
        public static string AssetBundlePathToAssetBundleName(string assetPath)
        {
            var ab = new StringBuilder();
            if (string.IsNullOrEmpty(assetPath)) return null;
            if (assetPath.StartsWith("Assets/"))
            {
                assetPath = AssetsPathToPackagePath(assetPath);
            }

            //no " "
            assetPath = assetPath.Replace(" ", "");
            //there should not be any '.' in the assetbundle name
            //otherwise the variant handling in client may go wrong
            assetPath = assetPath.Replace(".", "_");
            //add after suffix ".assetbundle" to the end
            ab.Append(assetPath);
            ab.Append(AssetBundleConfig.AssetBundleSuffix);
            return ab.ToString().ToLower();
        }

        /// <summary>
        /// 资产路径到包路径
        /// </summary>
        /// <param name="assetPath">资产包路径</param>
        /// <returns></returns>
        public static string AssetsPathToPackagePath(string assetPath)
        {
            string path = $"{AssetBundleConfig.AssetsFolderName}/";
            if (assetPath.Contains(path))
            {
                int idnex = assetPath.IndexOf(path, StringComparison.Ordinal) + path.Length;
                return assetPath.Substring(idnex);
            }
            else
            {
                UnityEngine.Debug.LogError("Asset path is not a package path!");
                return assetPath;
            }
        }

        /// <summary>
        /// 获取到对应路径
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <returns></returns>
        public static string GetPersistentDataPath(string assetPath = null)
        {
            //根据参数返回上一级目录
            string outputPath = Path.Combine(Application.persistentDataPath, AssetBundleConfig.AssetBundlesFolderName);
            if (!string.IsNullOrEmpty(assetPath))
            {
                //根据参数返回上一级目录
                outputPath = Path.Combine(outputPath, assetPath);
            }
#if UNITY_EDITOR_WIN
            // 路径替换
            return GameUtility.FormatToSysFilePath(outputPath);
#else
            return outputPath;
#endif
        }

        /// <summary>
        /// 检查持久文件是否存在
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static bool CheckPersistentFileExsits(string filePath)
        {
            //获取到对应路径 检查对应文件
            var path = GetPersistentDataPath(filePath);
            return File.Exists(path);
        }

        public static string GetPersistentFilePath(string assetPath = null)
        {
            return "file://" + GetPersistentDataPath(assetPath);
        }

        public static string GetStreamingAssetsFilePath(string assetPath = null)
        {
#if UNITY_EDITOR
            string outputPath = Path.Combine("file://" + Application.streamingAssetsPath,
                AssetBundleConfig.AssetBundlesFolderName);
#else
#if UNITY_IPHONE || UNITY_IOS
            string outputPath =
 Path.Combine("file://" + Application.streamingAssetsPath, AssetBundleConfig.AssetBundlesFolderName);
#elif UNITY_ANDROID
            string outputPath = Path.Combine(Application.streamingAssetsPath, AssetBundleConfig.AssetBundlesFolderName);
#else
            Logger.LogError("Unsupported platform!!!");
#endif
#endif
            if (!string.IsNullOrEmpty(assetPath))
            {
                outputPath = Path.Combine(outputPath, assetPath);
            }

            return outputPath;
        }

        // 注意：这个路径是给WWW读文件使用的url，如果要直接磁盘写persistentDataPath，使用GetPlatformPersistentDataPath
        public static string GetAssetBundleFileUrl(string filePath)
        {
            if (CheckPersistentFileExsits(filePath))
            {
                return GetPersistentFilePath(filePath);
            }
            else
            {
                return GetStreamingAssetsFilePath(filePath);
            }
        }

        public static string GetStreamingAssetsDataPath(string assetPath = null)
        {
            string outputPath = Path.Combine(Application.streamingAssetsPath, AssetBundleConfig.AssetBundlesFolderName);
            if (!string.IsNullOrEmpty(assetPath))
            {
                outputPath = Path.Combine(outputPath, assetPath);
            }

            return outputPath;
        }

        public static bool IsPackagePath(string assetPath)
        {
            string path = "Assets/" + AssetBundleConfig.AssetsFolderName + "/";
            return assetPath.StartsWith(path);
        }
    }
}
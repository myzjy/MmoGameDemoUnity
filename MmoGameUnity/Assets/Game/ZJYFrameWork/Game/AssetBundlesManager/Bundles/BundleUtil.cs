using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using ZJYFrameWork.AssetBundles.AssetBundleToolsConfig;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.Execution;

#if UNITY_ANDROID && !UNITY_EDITOR
using Ionic.Zip;
using System.Text.RegularExpressions;
#endif

namespace ZJYFrameWork.AssetBundles.BundleUtils
{
    public class BundleUtil
    {
        public readonly static string streamingAssetsPath = Application.streamingAssetsPath;
        public readonly static string persistentDataPath = Application.persistentDataPath;
        public readonly static string temporaryCachePath = Application.temporaryCachePath;

        private static string root = string.Empty;
        private static string temporaryCacheDirectory;
        private static string storableDirectory;
        private static string readOnlyDirectory;

        static BundleUtil()
        {
            Root = AssetBundleConfig.BundleRoot;
#if UNITY_IOS
            UnityEngine.iOS.Device.SetNoBackupFlag(GetStorableDirectory());
#endif
        }
        /// <summary>
        /// 附在URL后面的各平台前缀
        /// </summary>
        /// <returns></returns>
        public static string GetPlatformName()
        {
#if UNITY_ANDROID
            return "Android/";
#elif UNITY_IOS
            return "Ios/";
#else
            return "Pc/";
#endif
        }
        /// <summary>
        /// The AssetBundle's root.
        /// </summary>
        private static string Root
        {
            get
            {
                if (!string.IsNullOrEmpty(root)) return root;
                root = AssetBundleConfig.BundleRoot;
  
                temporaryCacheDirectory =
#if UNITY_EDITOR
                    $"{temporaryCachePath}/{root}/{GetPlatformName()}";
#elif UNITY_IOS
                $"{temporaryCachePath}/{root}/{GetPlatformName()}";
#elif UNITY_STANDALONE_WIN
                $"{streamingAssetsPath}/{root}/{GetPlatformName()}";
#elif UNITY_ANDROID
                    $"{temporaryCachePath}/{root}/{GetPlatformName()}";
#endif

                storableDirectory =
#if UNITY_EDITOR
                    $"{streamingAssetsPath}/{root}/{GetPlatformName()}";// $"{persistentDataPath}/{root}/{GetPlatformName()}";
#elif UNITY_IOS
                   $"{streamingAssetsPath}/{root}/{GetPlatformName()}";// $"{persistentDataPath}/{root}/{GetPlatformName()}";
#elif UNITY_STANDALONE_WIN
                    $"{streamingAssetsPath}/{root}/{GetPlatformName()}";
#elif UNITY_ANDROID
                   $"{streamingAssetsPath}/{root}/{GetPlatformName()}";// $"{persistentDataPath}/{root}/{GetPlatformName()}";
#endif

                readOnlyDirectory =
                    $"{streamingAssetsPath}/{root}/{GetPlatformName()}";
                return root;
            }
            set
            {
                if (string.IsNullOrEmpty(root))
                {
                    root = AssetBundleConfig.BundleRoot;
                }
                root = value;
    
                temporaryCacheDirectory =
#if UNITY_EDITOR
                    $"{temporaryCachePath}/{root}/{GetPlatformName()}";
#elif UNITY_IOS
                $"{temporaryCachePath}/{root}/{GetPlatformName()}";
#elif UNITY_STANDALONE_WIN
                $"{streamingAssetsPath}/{root}/{GetPlatformName()}";
#elif UNITY_ANDROID
                    $"{temporaryCachePath}/{root}/{GetPlatformName()}";
#endif

                storableDirectory =
#if UNITY_EDITOR
                    $"{streamingAssetsPath}/{root}/{GetPlatformName()}";// $"{persistentDataPath}/{root}/{GetPlatformName()}";
#elif UNITY_IOS
                   $"{streamingAssetsPath}/{root}/{GetPlatformName()}";// $"{persistentDataPath}/{root}/{GetPlatformName()}";
#elif UNITY_STANDALONE_WIN
                    $"{streamingAssetsPath}/{root}/{GetPlatformName()}";
#elif UNITY_ANDROID
                   $"{streamingAssetsPath}/{root}/{GetPlatformName()}";// $"{persistentDataPath}/{root}/{GetPlatformName()}";
#endif


                readOnlyDirectory =
                    $"{streamingAssetsPath}/{root}/{GetPlatformName()}";
            }
        }

        public static string GetTemporaryCacheDirectory()
        {
            if (!Root.Equals(AssetBundleConfig.BundleRoot))
                Root = AssetBundleConfig.BundleRoot;

            return temporaryCacheDirectory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The storable directory.</returns>
        public static string GetStorableDirectory()
        {
            if (!Root.Equals(AssetBundleConfig.BundleRoot))
                Root = AssetBundleConfig.BundleRoot;

            return storableDirectory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The read only directory.</returns>
        public static string GetReadOnlyDirectory()
        {
            if (!Root.Equals(AssetBundleConfig.BundleRoot))
                Root = AssetBundleConfig.BundleRoot;

            return readOnlyDirectory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundleInfo"></param>
        /// <returns></returns>
        public static bool ExistsInCache(BundleInfo bundleInfo)
        {
            if (Caching.IsVersionCached(bundleInfo.Filename, bundleInfo.Hash))
                return true;
            return false;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private static Dictionary<string,ZipFile> zips = new Dictionary<string, ZipFile>();

        public static ZipFile GetAndroidAPK(string path)
        {
            ZipFile zip;
            if (zips.TryGetValue(path, out zip))
                return zip;

            zip = new ZipFile(path);
            zips[path] = zip;
            return zip;
        }

        public static string GetCompressedFileName(string url)
        {
            url = Regex.Replace(url, @"^jar:file:///", "");
            return url.Substring(0, url.LastIndexOf("!"));
        }
#endif

#if !UNITY_WEBGL || UNITY_EDITOR
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundleInfo"></param>
        /// <returns></returns>
        public static bool ExistsInReadOnlyDirectory(BundleInfo bundleInfo)
        {
            return ExistsInReadOnlyDirectory(bundleInfo.Filename);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static bool ExistsInReadOnlyDirectory(string relativePath)
        {
            string dir = GetReadOnlyDirectory();
            string fullName = System.IO.Path.Combine(dir, relativePath);
#if UNITY_ANDROID && !UNITY_EDITOR
            var zipFileName = GetCompressedFileName(fullName);
            var entryName = fullName.Substring(fullName.LastIndexOf("!") + 2);            
            if (GetAndroidAPK(zipFileName).ContainsEntry(entryName))
                return true;
#else
            if (File.Exists(fullName))
                return true;
#endif
            return false;
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundleInfo"></param>
        /// <returns></returns>
        public static bool ExistsInStorableDirectory(BundleInfo bundleInfo)
        {
            return ExistsInStorableDirectory(bundleInfo.Filename);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static bool ExistsInStorableDirectory(string relativePath)
        {
            string dir = GetStorableDirectory();
            string fullName = System.IO.Path.Combine(dir, relativePath);
            if (File.Exists(fullName))
                return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundleInfo"></param>
        /// <returns></returns>
        public static bool Exists(BundleInfo bundleInfo)
        {
            if (ExistsInCache(bundleInfo))
                return true;

#if !UNITY_WEBGL || UNITY_EDITOR
            if (ExistsInReadOnlyDirectory(bundleInfo))
                return true;
#endif

            if (ExistsInStorableDirectory(bundleInfo))
                return true;

            return false;
        }

        private static void DeleteEmptyDirectory(DirectoryInfo directory)
        {
            try
            {
                if (directory.GetFiles().Length > 0)
                    return;

                DirectoryInfo[] arr = directory.GetDirectories();
                foreach (DirectoryInfo dir in arr)
                {
                    DeleteEmptyDirectory(dir);
                }

                arr = directory.GetDirectories();
                if (arr.Length <= 0)
                {
                    directory.Delete();
                    return;
                }
            }
            catch (Exception)
            {
            }
        }

        public static void EvictExpiredInStorableDirectory(BundleManifest manifest)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Executors.RunAsyncNoReturn(() =>
            {
#endif
                try
                {
                    DirectoryInfo directory = new DirectoryInfo(GetStorableDirectory());
                    if (!directory.Exists)
                        return;

                    List<string> files = new List<string>();
                    FileInfo manifestFileInfo = new FileInfo(GetStorableDirectory() + AssetBundleConfig.ManifestFilename);
                    files.Add(manifestFileInfo.FullName);

                    BundleInfo[] bundleInfos = manifest.GetAll();
                    foreach (BundleInfo bundleInfo in bundleInfos)
                    {
                        string fullname = GetStorableDirectory() + bundleInfo.Filename;
                        FileInfo info = new FileInfo(fullname);
                        if (!info.Exists)
                            continue;

                        files.Add(info.FullName);
                    }

                    foreach (FileInfo info in directory.GetFiles("*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            if (files.Contains(info.FullName))
                                continue;

                            info.Delete();
                        }
                        catch (Exception e)
                        {
                            Debug.LogErrorFormat("Delete file {0}.Error:{1}", info.FullName, e);
                        }
                    }

                    DeleteEmptyDirectory(directory);

                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("DeleteExpiredInStorableDirectory exception.Error:{0}", ex);
                }
#if !UNITY_WEBGL || UNITY_EDITOR
            });
#endif
        }

        public static void ClearStorableDirectory()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Executors.RunAsyncNoReturn(() =>
            {
#endif
                try
                {
                    DirectoryInfo directory = new DirectoryInfo(GetStorableDirectory());
                    if (!directory.Exists)
                        return;

                    directory.Delete(true);
                }
                catch (Exception e)
                {
#if !UNITY_WEBGL || UNITY_EDITOR
                    Debug.LogErrorFormat("Clear {}.Error:{}", GetStorableDirectory(), e);
#endif
                }
#if !UNITY_WEBGL || UNITY_EDITOR
            });
#endif
        }
    }
}


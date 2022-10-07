using UnityEditor;
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
        public static string GetPlatformName(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.Android:
                    return "Android";
                case BuildTarget.iOS:
                    return "iOS";
                default:
                    Debug.LogError("Error buildTarget!!!");
                    return null;
            }
        }
        
     
    

       

      
    }
}
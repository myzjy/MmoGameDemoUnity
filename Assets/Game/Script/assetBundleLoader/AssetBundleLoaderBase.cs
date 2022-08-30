using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Framework.AssetBundles.Config;
using Framework.AssetBundles.Utilty;
using UnityEngine;
using UnityEngine.Networking;
using ZJYFrameWork.AssetBundleLoader;
using ZJYFrameWork.AssetBundleLoader.CustomManifest;

namespace ZJYFrameWork
{
    /// <summary>
    /// AssetBundle的加载和管理。
    /// </summary>
    public abstract partial class AssetBundleLoaderBase : Singleton<AssetBundleLoaderBase>
    {
        /// <summary>
        /// 加载完成时的事件
        /// AssetBundle加载完成时的处理
        /// </summary>
        public delegate void OnLoadedDelegate(AssetBundleHolder assetBundleHolder);

        /// <summary>
        /// 加载完成场景时的处理
        /// </summary>
        public delegate void OnSceneLoadedDelegate();

        /// <summary>
        /// MainAsset加载完成时的处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public delegate void OnMainAssetLoadedDelegate<in T>(T mainasset);

        /// <summary>
        /// 多个AssetBundle的加载完成时的处理
        /// </summary>
        public delegate void OnMultiObjectLoadDelegate(AssetBundleHolder[] assetBundleHolders);

        /// <summary>
        /// AssetBundle加载出错时的处理
        /// </summary>
        public delegate void OnErrorDelegate(ErrorInfo errorInfo);

        /// <summary>
        /// 优先顺序的数量
        /// </summary>
        protected const int PRIORITY_NUM = 3;

        /// <summary>
        /// 内部AssetBundle
        /// </summary>
        protected AssetBundleLoaderImpl impl;

        /// <summary>
        /// 返回AssetBundleLoaderImpl
        /// </summary>
        /// <returns></returns>
        public AssetBundleLoaderImpl GetAssetBundleLoaderImpl()
        {
            return impl;
        }

        /// <summary>
        /// Manifest
        /// </summary>
        protected CustomManifest<BundleData> CustomManifest;

        /// <summary>
        /// Manifest的hash值
        /// </summary>
        protected string CurrentManifestHash;

        /// <summary>
        /// 是否完成了初始化
        /// </summary>
        public virtual bool Initialized => impl.IsInitialized(this);

        public string GetAssetBundleHashString(string assetBundleName)
        {
            return CustomManifest.GetAssetBundleVersion(assetBundleName);
        }

        /// <summary>
        /// 填写Request Queue
        /// </summary>
        Queue<RequestInfo>[] requestQueues = new Queue<RequestInfo>[PRIORITY_NUM];

        protected internal virtual int GetMaxConcurrencyDownloadNum()
        {
            return 5;
        }

        protected internal virtual int GetMaxRetryNum()
        {
            return 5;
        }

        protected internal virtual float GetTimeoutTime()
        {
            return 30;
        }

        /// <summary>
        /// 基础Url
        /// </summary>
        /// <returns></returns>
        public abstract string GetAssetBaseUrl();

        /// <summary>
        /// 现在正在下载的资源
        /// </summary>
        RequestInfo[] currentDownloads;

        /// <summary>
        /// 自定义Dictionary保存AssetBundle
        /// </summary>
        private UIntKeyDictionary<AssetBundleHolder> loaderAssetBundles = new UIntKeyDictionary<AssetBundleHolder>(512);

        /// <summary>
        /// Time out时的错误信息
        /// </summary>
        internal const string TimeOutErrorMessage = "Time Out";

        /// <summary>
        /// 下载加载失败时的错误信息
        /// </summary>
        internal const string DownloadBundleInvalidErrorMessage =
#if UNITY_2020_2_OR_NEWER
            "Faild UnityWebRequest.assetBundle, AssetBundleName - {0}, Hash - {1}";
#else
            "Faild WWW.assetBundle, AssetBundleName - {0}, Hash - {1}";
#endif
        /// <summary>
        /// 本地加载失败时的错误消息
        /// </summary>
        internal const string CacheFileBundleInvalidErrorMessage =
            "Faild AssetBundle.LoadFromFileAsync, AssetBundleName - {0}, Hash - {1}";

        /// <summary>
        /// 中断处理时的错误消息
        /// </summary>
        internal const string AbortLoadingRequestErrorMessage = "Avort Loading Request";

        /// <summary>
        /// 缓存文件夹
        /// </summary>
        private string cacheDirectoryPath;

        private HashSet<string> cachedAssetHashSet;
        private List<UnloadInfo> unloadList = new List<UnloadInfo>(64);

        /// <summary>
        /// 打包时本地会包含的资源
        /// </summary>
        private const string ASSETS_MASTER_LOCAL = "assets_masters_local";

        private Dictionary<string, string> localStreamingAssetDic = new Dictionary<string, string>(); //拷贝进apk的ab包名,hash

        // private Dictionary<string, string> localStreamingAssetCPKDic = new Dictionary<string, string>(); //拷贝进apk的cpk,版本
        public virtual CustomManifest<BundleData> GetCustomManifest()
        {
            return CustomManifest;
        }

        public virtual string GetBundleExt()
        {
            return AssetBundleConfig.AssetBundleSuffix;
        }

        /// <summary>
        /// 附在URL后面的各平台前缀
        /// </summary>
        /// <returns></returns>
        public virtual string GetPlatformPrefix()
        {
            return AssetBundleUtility.GetPlatformName();
        }

        protected abstract AssetBundleLoaderImpl CreateImpl();

        /// <summary>
        /// 能读取assetBundle
        /// </summary>
        public bool CanLoadAssetBundle { get; private set; }

        /// <summary>
        /// assetbundle的hash值
        /// </summary>
        protected HashSet<string> AssetBundleNameHashSet = new HashSet<string>();

        /// <summary>
        /// 设置当新的Manifest被发行时进行的处理
        /// </summary>
        System.Action OnManifestUpdated;

        /// <summary>
        /// 设置当新的Manifest被发行时进行的处理
        /// </summary>
        /// <param name="OnManifestUpdated">设置当新的Manifest被发行时进行的处理</param>
        public void SetManifestUpdatedAction(System.Action OnManifestUpdated)
        {
            this.OnManifestUpdated = OnManifestUpdated;
        }

        /// <summary>
        /// 与本地保存的manifest的哈希进行比较，如果更新了就进行设定的处理
        /// </summary>
        /// <param name="hash">传递hash值</param>
        /// <returns><c>true</c>,有Manifest的更新, <c>false</c>没有Manifest的更新</returns>
        public bool CheckManifestUpdated(string hash)
        {
            if (!impl.RequireManifest())
            {
                return false;
            }

            if (CurrentManifestHash == hash)
            {
                return false;
            }

            if (OnManifestUpdated != null)
            {
                OnManifestUpdated();
            }

            return true;
        }

        public void Load(string assetBundleName, OnLoadedDelegate onLoad = null, OnErrorDelegate onError = null,
            Priority priority = Priority.Normal, Action onDownload = null, bool unloadAllLoadedObjects = false)
        {
            if (!assetBundleName.Contains(GetBundleExt()))
            {
                assetBundleName = $"{assetBundleName}{GetBundleExt()}";
            }

            if (impl.RequestEmptyHolder(assetBundleName, onLoad))
            {
                return;
            }

            if (CustomManifest == null)
            {
                Debug.LogError("自定义Manifest为空，但您正在请求新的AssetBundle: " + assetBundleName);
                return;
            }

            string hashString = GetAssetBundleHashString(assetBundleName);
            //Load结束的情况
            AssetBundleHolder holder;
            uint key = (uint)hashString.GetHashCode();
            //在删除预定列表中却被请求的情况下，从删除列表中排除
            int unloadInfoIndex = unloadList.FindIndex(info => info.key == key);
            if (unloadInfoIndex >= 0)
            {
                unloadList.RemoveAt(unloadInfoIndex);
            }

            if (loaderAssetBundles.TryGetValue(key, out holder))
            {
                if (onLoad != null)
                {
                    //Debug.LogWarning("LoadCache Ok:" + assetBundleName);
                    onLoad(new AssetBundleHolder(holder));
                    return;
                }
            }

            AssetBundleNameHashSet.Add(assetBundleName);
            //如果没有，就创建新的加载请求。
            RequestInfo request = new RequestInfo();
            request.assetUrlPath = Instance.GetAssetBundleLoaderImpl().CreateUrl(this, assetBundleName, hashString);
            request.assetBundleName = assetBundleName;

            request.onLoaded = onLoad;

            request.unloadAllLoadedObjects = unloadAllLoadedObjects;
            //如果在加载失败时设置了回调，则遵循回调
            //没有指定回调的情况下进行共同处理
            request.onError = onError ?? OnErrorGlobal;

            request.onDownload = onDownload;

            request.hashString = hashString;
            var dependencies = CustomManifest.GetAllDependencies(assetBundleName);
            request.dependencies.Capacity = dependencies.Length;
            request.dependenceHolders.Capacity = dependencies.Length;
            //生成解决依赖关系的请求信息
            foreach (var t in dependencies)
            {
                var dependenceHashString = GetAssetBundleHashString(t);

                var dependenceRequest = new RequestInfo
                {
                    assetBundleName = t,
                    assetUrlPath = Instance.GetAssetBundleLoaderImpl().CreateUrl(Instance, t, dependenceHashString),
                    hashString = dependenceHashString
                };
                dependenceRequest.onLoaded = ((assetHolder) =>
                {
                    dependenceRequest.loaded = true;
                    request.dependenceHolders.Add(assetHolder);
                });
                dependenceRequest.onError = ((errorInfo) =>
                {
                    //如果Join到另一个请求，就不会设置errorInfo。
                    dependenceRequest.errorInfo = errorInfo;
                });
                request.dependencies.Add(dependenceRequest);
                AssetBundleNameHashSet.Add(t);
            }

            requestQueues[(int)priority].Enqueue(request);
        }

        /// <summary>
        /// 加载多个AssetBundle
        /// </summary>
        /// <param name="assetBundleNames">Asset bundle name</param>
        /// <param name="onLoaded">加载委托</param>
        /// <param name="onError">错误</param>
        /// <param name="priority">priority</param>
        /// <param name="onDownload">只有在没有缓存进行下载时才被执行</param>
        public void Load(string[] assetBundleNames, OnMultiObjectLoadDelegate onLoaded = null,
            OnErrorDelegate onError = null, Priority priority = Priority.Normal, System.Action onDownload = null)
        {
            var hasError = false;
            var onDownloadActionCalled = false;
            var holders = new AssetBundleHolder[assetBundleNames.Length];
            for (var i = 0; i < assetBundleNames.Length; ++i)
            {
                var index = i;
                Load(assetBundleNames[index],
                    (assetHolder) =>
                    {
                        holders[index] = assetHolder;
                        if (hasError)
                        {
                            return;
                        }

                        for (var j = 0; j < assetBundleNames.Length; ++j)
                        {
                            if (holders[j] == null)
                            {
                                return;
                            }
                        }

                        onLoaded?.Invoke(holders);
                    },
                    (errorInfo) =>
                    {
                        if (onError == null) return;
                        hasError = true;
                        onError(errorInfo);
                    },
                    priority,
                    () =>
                    {
                        if (onDownloadActionCalled || onDownload == null) return;
                        onDownload();
                        onDownloadActionCalled = true;
                    });
            }
        }

        /// <summary>
        /// 加载AssetBundle，返回特定内容
        /// </summary>
        /// <param name="assetBundleName">Asset bundle name.</param>
        /// <param name="onLoaded">On loaded.</param>
        /// <param name="onError">On error.</param>
        /// <param name="onDownload">只有在没有缓存进行下载时才被执行</param>
        /// <param name="priority">Priority.</param>
        /// <param name="assetName">Asset name.</param>
        /// <param name="unloadAllLoadedObjects"></param>
        /// <typeparam name="T">第一个类型参数。</typeparam>
        public void Load<T>(string assetBundleName, OnMainAssetLoadedDelegate<T> onLoaded = null,
            OnErrorDelegate onError = null, Priority priority = Priority.Normal, string assetName = null,
            System.Action onDownload = null, bool unloadAllLoadedObjects = false) where T : class
        {
            if (string.IsNullOrEmpty(assetName))
            {
                assetName = Path.GetFileNameWithoutExtension(assetBundleName);
            }

            Load(assetBundleName, (assetHolder) =>
                {
                    if (onLoaded != null)
                    {
                        assetHolder.LoadAssetAsync(assetName, typeof(T), obj => { onLoaded(obj as T); });
                    }
                },
                // ReSharper disable once IdentifierTypo
                (errorinfo) => { onError?.Invoke(errorinfo); },
                priority,
                () => { onDownload?.Invoke(); }, unloadAllLoadedObjects);
        }

        /// <summary>
        /// 资产加载时的错误共同处理
        /// </summary>
        /// <param name="error"></param>
        protected virtual void OnErrorGlobal(ErrorInfo error)
        {
        }
    }
}
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using Framework.AssetBundles.Config;
// using Framework.AssetBundles.Utilty;
// using UnityEngine;
// using UnityEngine.Assertions;
// using UnityEngine.Networking;
// using UnityEngine.SceneManagement;
// using ZJYFrameWork.AssetBundleLoader;
// using ZJYFrameWork.AssetBundleLoader.CustomManifest;
//
// namespace ZJYFrameWork
// {
//     /// <summary>
//     /// AssetBundle的加载和管理。
//     /// </summary>
//     public abstract partial class AssetBundleLoaderBase : Singleton<AssetBundleLoaderBase>
//     {
//         /// <summary>
//         /// 加载完成时的事件
//         /// AssetBundle加载完成时的处理
//         /// </summary>
//         public delegate void OnLoadedDelegate(AssetBundleHolder assetBundleHolder);
//
//         /// <summary>
//         /// 加载完成场景时的处理
//         /// </summary>
//         public delegate void OnSceneLoadedDelegate();
//
//         /// <summary>
//         /// MainAsset加载完成时的处理
//         /// </summary>
//         /// <typeparam name="T"></typeparam>
//         public delegate void OnMainAssetLoadedDelegate<in T>(T mainasset);
//
//         /// <summary>
//         /// 多个AssetBundle的加载完成时的处理
//         /// </summary>
//         public delegate void OnMultiObjectLoadDelegate(AssetBundleHolder[] assetBundleHolders);
//
//         /// <summary>
//         /// AssetBundle加载出错时的处理
//         /// </summary>
//         public delegate void OnErrorDelegate(ErrorInfo errorInfo);
//
//         /// <summary>
//         /// 优先顺序的数量
//         /// </summary>
//         protected const int PriorityNum = 3;
//
//         /// <summary>
//         /// 内部AssetBundle
//         /// </summary>
//         protected AssetBundleLoaderImpl impl;
//
//         /// <summary>
//         /// 返回AssetBundleLoaderImpl
//         /// </summary>
//         /// <returns></returns>
//         public AssetBundleLoaderImpl GetAssetBundleLoaderImpl()
//         {
//             return impl;
//         }
//
//         /// <summary>
//         /// Manifest
//         /// </summary>
//         protected CustomManifest<BundleData> CustomManifest;
//
//         /// <summary>
//         /// Manifest的hash值
//         /// </summary>
//         protected string CurrentManifestHash;
//
//         /// <summary>
//         /// 是否完成了初始化
//         /// </summary>
//         public virtual bool Initialized => impl.IsInitialized(this);
//
//         public string GetAssetBundleHashString(string assetBundleName)
//         {
//             return CustomManifest.GetAssetBundleVersion(assetBundleName);
//         }
//
//         /// <summary>
//         /// 填写Request Queue
//         /// </summary>
//         Queue<RequestInfo>[] requestQueues = new Queue<RequestInfo>[PriorityNum];
//
//         protected internal virtual int GetMaxConcurrencyDownloadNum()
//         {
//             return 5;
//         }
//
//         protected internal virtual int GetMaxRetryNum()
//         {
//             return 5;
//         }
//
//         protected internal virtual float GetTimeoutTime()
//         {
//             return 30;
//         }
//
//         /// <summary>
//         /// 基础Url
//         /// </summary>
//         /// <returns></returns>
//         public abstract string GetAssetBaseUrl();
//
//         /// <summary>
//         /// 现在正在下载的资源
//         /// </summary>
//         RequestInfo[] currentDownloads;
//
//         /// <summary>
//         /// 自定义Dictionary保存AssetBundle
//         /// </summary>
//         private UIntKeyDictionary<AssetBundleHolder> loaderAssetBundles = new UIntKeyDictionary<AssetBundleHolder>(512);
//
//         /// <summary>
//         /// Time out时的错误信息
//         /// </summary>
//         internal const string TimeOutErrorMessage = "Time Out";
//
//         /// <summary>
//         /// 下载加载失败时的错误信息
//         /// </summary>
//         internal const string DownloadBundleInvalidErrorMessage =
// #if UNITY_2020_2_OR_NEWER
//             "Faild UnityWebRequest.assetBundle, AssetBundleName - {0}, Hash - {1}";
// #else
//             "Faild WWW.assetBundle, AssetBundleName - {0}, Hash - {1}";
// #endif
//         /// <summary>
//         /// 本地加载失败时的错误消息
//         /// </summary>
//         internal const string CacheFileBundleInvalidErrorMessage =
//             "Faild AssetBundle.LoadFromFileAsync, AssetBundleName - {0}, Hash - {1}";
//
//         /// <summary>
//         /// 中断处理时的错误消息
//         /// </summary>
//         internal const string AbortLoadingRequestErrorMessage = "Avort Loading Request";
//
//         /// <summary>
//         /// 缓存文件夹
//         /// </summary>
//         private string cacheDirectoryPath;
//
//         private HashSet<string> cachedAssetHashSet;
//         private List<UnloadInfo> unloadList = new List<UnloadInfo>(64);
//
//         /// <summary>
//         /// 打包时本地会包含的资源
//         /// </summary>
//         private const string ASSETS_MASTER_LOCAL = "assets_masters_local";
//
//         private Dictionary<string, string> localStreamingAssetDic = new Dictionary<string, string>(); //拷贝进apk的ab包名,hash
//
//         // private Dictionary<string, string> localStreamingAssetCPKDic = new Dictionary<string, string>(); //拷贝进apk的cpk,版本
//         public virtual CustomManifest<BundleData> GetCustomManifest()
//         {
//             return CustomManifest;
//         }
//
//         public virtual string GetBundleExt()
//         {
//             return AssetBundleConfig.AssetBundleSuffix;
//         }
//
//         /// <summary>
//         /// 附在URL后面的各平台前缀
//         /// </summary>
//         /// <returns></returns>
//         public virtual string GetPlatformPrefix()
//         {
//             return AssetBundleUtility.GetPlatformName();
//         }
//
//         protected abstract AssetBundleLoaderImpl CreateImpl();
//
//         /// <summary>
//         /// 能读取assetBundle
//         /// </summary>
//         public bool CanLoadAssetBundle { get; private set; }
//
//         /// <summary>
//         /// assetbundle的hash值
//         /// </summary>
//         protected HashSet<string> AssetBundleNameHashSet = new HashSet<string>();
//
//         /// <summary>
//         /// 设置当新的Manifest被发行时进行的处理
//         /// </summary>
//         System.Action OnManifestUpdated;
//
//         /// <summary>
//         /// 自定义清单AssetBundle
//         /// </summary>
//         private AssetBundle CustomManiFestAssetBundle = null;
//
//         /// <summary>
//         /// 自定义清单RequestInfo
//         /// </summary>
//         private RequestInfo customManifestRequestInfo = null;
//
//         /// <summary>
//         /// 是否卸载
//         /// </summary>
//         private bool NoUnload = true;
//
//         /// <summary>
//         /// 内存占用
//         /// </summary>
//         private long usedMemory = 0;
//
//         /// <summary>
//         /// 总内存
//         /// </summary>
//         private int totalMempry = 0;
//
//         /// <summary>
//         /// 缓存目录的路径生成和下载(加载)信息的管理类保持同时下载的数量
//         /// 初始化
//         /// </summary>
//         protected override void Init()
//         {
//             base.Init();
//             impl = CreateImpl();
//             //
//             //改变路径(temporary-> persistent)
//             cacheDirectoryPath = Path.Combine(Application.persistentDataPath, "AssetBundles");
//             if (!Directory.Exists(cacheDirectoryPath))
//             {
//                 Directory.CreateDirectory(cacheDirectoryPath);
//             }
//
//             InitLocalStreamingAsset();
//             for (var i = 0; i < requestQueues.Length; ++i)
//             {
//                 requestQueues[i] = new Queue<RequestInfo>(128);
//             }
//
//             //如果有可转移Asset，就转移
//             CopyCacheDirectory();
//
// #if UNITY_IOS
// 			// 备用标记
//                         UnityEngine.iOS.Device.SetNoBackupFlag(cacheDirectoryPath);
// #endif
//             //清除AssetBundle
//             var oldCacheDirectoryPath = Path.Combine(Application.temporaryCachePath, "AssetBundles");
//             if (Directory.Exists(oldCacheDirectoryPath))
//             {
//                 Directory.Delete(oldCacheDirectoryPath, true);
//             }
//
//             cachedAssetHashSet = new HashSet<string>();
//
//             // 根据同时下载数量制作存储请求的列表
//             currentDownloads = new RequestInfo[GetMaxConcurrencyDownloadNum()];
//
//             CanLoadAssetBundle = true;
//         }
//
//         /// <summary>
//         /// 监视下载和缓存加载
//         /// </summary>
//         protected void LateUpdate()
//         {
//             ProcessUnload();
//
//             Debug.Assert(currentDownloads.Length <= GetMaxConcurrencyDownloadNum());
//
//             if (!CanLoadAssetBundle)
//             {
//                 return;
//             }
//
//             ProcessDownloading();
//
//             // 在Manifest加载结束之前不处理新请求
//             if (CustomManifest == null) return;
//
//             TryProcessNewRequest();
//         }
//
//         /// <summary>
//         /// 后缀
//         /// </summary>
//         private char[] signs = new char[] { '\r', '\n' };
//
//         /// <summary>
//         /// 初始化StreamingAsset文件夹
//         /// </summary>
//         private void InitLocalStreamingAsset()
//         {
//             var text = Resources.Load<TextAsset>(ASSETS_MASTER_LOCAL);
//             var names = text.text.Split(signs);
//             foreach (var value in names)
//             {
//                 if (string.IsNullOrEmpty(value)) continue;
//                 var arr = value.Split(';');
//                 if (arr.Length != 2)
//                 {
//                     continue;
//                 }
//
//                 if (!localStreamingAssetDic.ContainsKey(arr[0]))
//                     localStreamingAssetDic.Add(arr[0], arr[1]);
//             }
//         }
//
//         /// <summary>
//         /// 设置当新的Manifest被发行时进行的处理
//         /// </summary>
//         /// <param name="OnManifestUpdated">设置当新的Manifest被发行时进行的处理</param>
//         public void SetManifestUpdatedAction(System.Action OnManifestUpdated)
//         {
//             this.OnManifestUpdated = OnManifestUpdated;
//         }
//
//         /// <summary>
//         /// 与本地保存的manifest的哈希进行比较，如果更新了就进行设定的处理
//         /// </summary>
//         /// <param name="hash">传递hash值</param>
//         /// <returns><c>true</c>,有Manifest的更新, <c>false</c>没有Manifest的更新</returns>
//         public bool CheckManifestUpdated(string hash)
//         {
//             if (!impl.RequireManifest())
//             {
//                 return false;
//             }
//
//             if (CurrentManifestHash == hash)
//             {
//                 return false;
//             }
//
//             if (OnManifestUpdated != null)
//             {
//                 OnManifestUpdated();
//             }
//
//             return true;
//         }
//
//         /// <summary>
//         /// 数据加载请求
//         /// </summary>
//         /// <param name="assetBundleName">assetBundle名</param>
//         /// <param name="onLoad">在加载完成时调用</param>
//         /// <param name="onError">发生错误时被调用</param>
//         /// <param name="priority">进行加载的优先顺序</param>
//         /// <param name="onDownload">只有在没有缓存进行下载时才被调用</param>
//         /// <param name="unloadAllLoadedObjects"></param>
//         public void Load(string assetBundleName, OnLoadedDelegate onLoad = null, OnErrorDelegate onError = null,
//             Priority priority = Priority.Normal, Action onDownload = null, bool unloadAllLoadedObjects = false)
//         {
//             if (!assetBundleName.Contains(GetBundleExt()))
//             {
//                 assetBundleName = $"{assetBundleName}{GetBundleExt()}";
//             }
//
//             if (impl.RequestEmptyHolder(assetBundleName, onLoad))
//             {
//                 return;
//             }
//
//             if (CustomManifest == null)
//             {
//                 Debug.LogError("自定义Manifest为空，但您正在请求新的AssetBundle: " + assetBundleName);
//                 return;
//             }
//
//             string hashString = GetAssetBundleHashString(assetBundleName);
//             //Load结束的情况
//             AssetBundleHolder holder;
//             uint key = (uint)hashString.GetHashCode();
//             //在删除预定列表中却被请求的情况下，从删除列表中排除
//             int unloadInfoIndex = unloadList.FindIndex(info => info.key == key);
//             if (unloadInfoIndex >= 0)
//             {
//                 unloadList.RemoveAt(unloadInfoIndex);
//             }
//
//             if (loaderAssetBundles.TryGetValue(key, out holder))
//             {
//                 if (onLoad != null)
//                 {
//                     //Debug.LogWarning("LoadCache Ok:" + assetBundleName);
//                     onLoad(new AssetBundleHolder(holder));
//                     return;
//                 }
//             }
//
//             AssetBundleNameHashSet.Add(assetBundleName);
//             //如果没有，就创建新的加载请求。
//             RequestInfo request = new RequestInfo();
//             request.assetUrlPath = Instance.GetAssetBundleLoaderImpl().CreateUrl(this, assetBundleName, hashString);
//             request.assetBundleName = assetBundleName;
//
//             request.onLoaded = onLoad;
//
//             request.unloadAllLoadedObjects = unloadAllLoadedObjects;
//             //如果在加载失败时设置了回调，则遵循回调
//             //没有指定回调的情况下进行共同处理
//             request.onError = onError ?? OnErrorGlobal;
//
//             request.onDownload = onDownload;
//
//             request.hashString = hashString;
//             var dependencies = CustomManifest.GetAllDependencies(assetBundleName);
//             request.dependencies.Capacity = dependencies.Length;
//             request.dependenceHolders.Capacity = dependencies.Length;
//             //生成解决依赖关系的请求信息
//             foreach (var t in dependencies)
//             {
//                 var dependenceHashString = GetAssetBundleHashString(t);
//
//                 var dependenceRequest = new RequestInfo
//                 {
//                     assetBundleName = t,
//                     assetUrlPath = Instance.GetAssetBundleLoaderImpl().CreateUrl(Instance, t, dependenceHashString),
//                     hashString = dependenceHashString
//                 };
//                 dependenceRequest.onLoaded = ((assetHolder) =>
//                 {
//                     dependenceRequest.loaded = true;
//                     request.dependenceHolders.Add(assetHolder);
//                 });
//                 dependenceRequest.onError = ((errorInfo) =>
//                 {
//                     //如果Join到另一个请求，就不会设置errorInfo。
//                     dependenceRequest.errorInfo = errorInfo;
//                 });
//                 request.dependencies.Add(dependenceRequest);
//                 AssetBundleNameHashSet.Add(t);
//             }
//
//             requestQueues[(int)priority].Enqueue(request);
//         }
//
//         /// <summary>
//         /// 加载多个AssetBundle
//         /// </summary>
//         /// <param name="assetBundleNames">Asset bundle name</param>
//         /// <param name="onLoaded">加载委托</param>
//         /// <param name="onError">错误</param>
//         /// <param name="priority">priority</param>
//         /// <param name="onDownload">只有在没有缓存进行下载时才被执行</param>
//         public void Load(string[] assetBundleNames, OnMultiObjectLoadDelegate onLoaded = null,
//             OnErrorDelegate onError = null, Priority priority = Priority.Normal, System.Action onDownload = null)
//         {
//             var hasError = false;
//             var onDownloadActionCalled = false;
//             var holders = new AssetBundleHolder[assetBundleNames.Length];
//             for (var i = 0; i < assetBundleNames.Length; ++i)
//             {
//                 var index = i;
//                 Load(assetBundleNames[index],
//                     (assetHolder) =>
//                     {
//                         holders[index] = assetHolder;
//                         if (hasError)
//                         {
//                             return;
//                         }
//
//                         for (var j = 0; j < assetBundleNames.Length; ++j)
//                         {
//                             if (holders[j] == null)
//                             {
//                                 return;
//                             }
//                         }
//
//                         onLoaded?.Invoke(holders);
//                     },
//                     (errorInfo) =>
//                     {
//                         if (onError == null) return;
//                         hasError = true;
//                         onError(errorInfo);
//                     },
//                     priority,
//                     () =>
//                     {
//                         if (onDownloadActionCalled || onDownload == null) return;
//                         onDownload();
//                         onDownloadActionCalled = true;
//                     });
//             }
//         }
//
//         /// <summary>
//         /// 加载AssetBundle，返回特定内容
//         /// </summary>
//         /// <param name="assetBundleName">Asset bundle name.</param>
//         /// <param name="onLoaded">On loaded.</param>
//         /// <param name="onError">On error.</param>
//         /// <param name="onDownload">只有在没有缓存进行下载时才被执行</param>
//         /// <param name="priority">Priority.</param>
//         /// <param name="assetName">Asset name.</param>
//         /// <param name="unloadAllLoadedObjects"></param>
//         /// <typeparam name="T">第一个类型参数。</typeparam>
//         public void Load<T>(string assetBundleName, OnMainAssetLoadedDelegate<T> onLoaded = null,
//             OnErrorDelegate onError = null, Priority priority = Priority.Normal, string assetName = null,
//             System.Action onDownload = null, bool unloadAllLoadedObjects = false) where T : class
//         {
//             if (string.IsNullOrEmpty(assetName))
//             {
//                 assetName = Path.GetFileNameWithoutExtension(assetBundleName);
//             }
//
//             Load(assetBundleName, (assetHolder) =>
//                 {
//                     if (onLoaded != null)
//                     {
//                         assetHolder.LoadAssetAsync(assetName, typeof(T), obj => { onLoaded(obj as T); });
//                     }
//                 },
//                 // ReSharper disable once IdentifierTypo
//                 (errorinfo) => { onError?.Invoke(errorinfo); },
//                 priority,
//                 () => { onDownload?.Invoke(); }, unloadAllLoadedObjects);
//         }
//
//         /// <summary>
//         /// 资产加载时的错误共同处理
//         /// </summary>
//         /// <param name="error"></param>
//         protected virtual void OnErrorGlobal(ErrorInfo error)
//         {
//         }
//
//         /// <summary>
//         /// AssetBundleをロードしシーンをロードする
//         /// </summary>
//         /// <param name="assetBundleName">Asset bundle name.</param>
//         /// <param name="onLoaded">On loaded.</param>
//         /// <param name="onError">On error.</param>
//         /// <param name="priority">Priority.</param>
//         /// <param name="sceneName">scene name.</param>
//         public void LoadScene(string assetBundleName, string sceneName, LoadSceneMode mode,
//             OnSceneLoadedDelegate onLoaded = null, OnErrorDelegate onError = null, Priority priority = Priority.Normal)
//         {
//             Load(assetBundleName, (assetHolder) =>
//                 {
//                     if (onLoaded != null)
//                     {
//                         assetHolder.LoadScene(sceneName, mode);
//                         onLoaded();
//                     }
//                 },
//                 (errorinfo) =>
//                 {
//                     if (onError != null)
//                     {
//                         onError(errorinfo);
//                     }
//                 }, priority);
//         }
//
//         /// <summary>
//         /// Hash128的字节大小
//         /// </summary>
//         internal const int Hash128ByteLength = 32;
//
//         /// <summary>
//         /// 创建写入本地存储的文件缓存路径
//         /// </summary>
//         /// <param name="assetBundleName">assetBundleName</param>
//         /// <returns></returns>
//         public string CreateCachePath(string assetBundleName)
//         {
//             return Path.Combine(cacheDirectoryPath, assetBundleName);
//         }
//
//         /// <summary>
//         /// 先从streamingasset加载，因为可能在打包时就已经打了ab资源进去
//         /// 根据不同平台文件路径会不一样？
//         /// </summary>
//         /// <param name="assetBundleName"></param>
//         /// <returns></returns>
//         public string CreatStreamingAssetsPath(string assetBundleName)
//         {
// #if UNITY_EDITOR || UNITY_STANDALONE
//             return Path.Combine("file://", Application.streamingAssetsPath, $"pc/{assetBundleName}");
// #elif UNITY_IOS
// 			return Path.Combine("file://", Application.streamingAssetsPath, $"ios/{assetBundleName}");
// #elif UNITY_ANDROID
// 			return Path.Combine(Application.streamingAssetsPath, $"android/{assetBundleName}");
// #endif
//         }
//
//         /// <summary>
//         /// 
//         /// </summary>
//         /// <param name="assetBundleName"></param>
//         public void DeleteCache(string assetBundleName)
//         {
//             // 从hashSet中删除
//             var hashString = (CustomManifest == null)
//                 ? CurrentManifestHash
//                 : GetAssetBundleHashString(assetBundleName);
//             var cachedAssetName = assetBundleName + hashString;
//             if (cachedAssetHashSet.Contains(cachedAssetName))
//             {
//                 cachedAssetHashSet.Remove(cachedAssetName);
//             }
//
//             var cachePath = CreateCachePath(assetBundleName);
//             // 如果已经存在就删除
//             if (File.Exists(cachePath))
//             {
//                 File.Delete(cachePath);
//             }
//         }
//
//         /// <summary>
//         /// 文件是否被缓存
//         /// 如果hashString是空的，就使用Manifest哈希
//         /// </summary>
//         /// <param name="assetBundleName"></param>
//         /// <param name="hashString">assetBundle的hash值</param>
//         /// <returns></returns>
//         protected internal bool IsCached(string assetBundleName, string hashString = null)
//         {
//             string cachePath = CreateCachePath(assetBundleName);
//             //路径是否存在
//             if (!File.Exists(cachePath))
//             {
// #if DEVELOP_BUILD
//                 Debug.LogError($"{cachePath} NOT EXIST");
// #endif
//                 return false;
//             }
//
//             //如果传递过来的hashString为空，使用assetBundle的hash值
//             if (string.IsNullOrEmpty(hashString))
//             {
//                 hashString = GetAssetBundleHashString(assetBundleName);
//             }
//
//             var cachedAssetName = $"{assetBundleName}{hashString}";
//             //缓存字典里面是否有 直接返回ture 代表进行过对比
//             if (cachedAssetHashSet.Contains(cachedAssetName))
//             {
//                 return true;
//             }
//
//             //读取本地
//             using (Stream stream = File.OpenRead(cachePath))
//             {
//                 using (BinaryReader reader = new BinaryReader(stream))
//                 {
//                     byte[] bytes = reader.ReadBytes(Hash128ByteLength);
//                     if (System.Text.Encoding.UTF8.GetString(bytes) != hashString)
//                     {
// #if UNITY_EDITOR || DEVELOP_BUILD
//                         Debug.LogError(
//                             $"{assetBundleName} hash 不一致。 本地hash:{System.Text.Encoding.UTF8.GetString(bytes)} 服务器 hash:{hashString}");
// #endif
//                         return false;
//                     }
//                 }
//             }
//
//             // 注册HashSet字典
//             cachedAssetHashSet.Add(cachedAssetName);
//
//             return true;
//         }
//
//         public static void RemoveDirectory(string directoryPath)
//         {
//             if (!Directory.Exists(directoryPath))
//             {
//                 return;
//             }
//
//             // 删除目录内的文件
//             string[] filePaths = Directory.GetFiles(directoryPath);
//             int fileCount = filePaths.Length;
//             for (int i = 0; i < fileCount; ++i)
//             {
//                 try
//                 {
//                     File.Delete(filePaths[i]);
//                 }
//                 catch (System.UnauthorizedAccessException ex)
//                 {
//                     Debug.LogError("[System.UnauthorizedAccessException] message = " + ex.Message);
//                 }
//             }
//
//             // 递归目录中的目录也删除
//             string[] directoryPaths = Directory.GetDirectories(directoryPath);
//             int directoryCount = directoryPaths.Length;
//             for (int i = 0; i < directoryCount; ++i)
//             {
//                 RemoveDirectory(directoryPaths[i]);
//             }
//         }
//
//         /// <summary>
//         /// 缓存的文件移动
//         /// </summary>
//         /// <returns></returns>
//         protected virtual string[] GetMoveDirectoryNames()
//         {
//             return Array.Empty<string>();
//         }
//
//         protected void CopyCacheDirectory()
//         {
//             var direcotries = GetMoveDirectoryNames();
//             string cacheDirectoryPath = Path.Combine(Application.persistentDataPath, "AssetBundles");
//             string oldCacheDirectoryPath = Path.Combine(Application.temporaryCachePath, "AssetBundles");
//             for (int i = 0; i < direcotries.Length; ++i)
//             {
//                 string sourceMovieCacheDirectoryPath = Path.Combine(oldCacheDirectoryPath, direcotries[i]);
//                 string destMovieCacheDirectoryPath = Path.Combine(cacheDirectoryPath, direcotries[i]);
//                 if (Directory.Exists(sourceMovieCacheDirectoryPath))
//                 {
//                     CopyDirectory(sourceMovieCacheDirectoryPath, destMovieCacheDirectoryPath);
//                 }
//             }
//         }
//
//         /// <summary>
//         /// 缓存文件的复制用
//         /// </summary>
//         /// <param name="sourceDirName">源目录名</param>
//         /// <param name="destDirName"></param>
//         private static void CopyDirectory(string sourceDirName, string destDirName)
//         {
//             //没有要复印的目录时制作
//             if (!Directory.Exists(destDirName))
//             {
//                 Directory.CreateDirectory(destDirName);
//                 //属性也复制
//                 File.SetAttributes(destDirName, File.GetAttributes(sourceDirName));
//             }
//
//             //在复制的目录名的末尾加上“\”
//             if (!destDirName.EndsWith(Path.DirectorySeparatorChar.ToString()))
//             {
//                 destDirName = destDirName + Path.DirectorySeparatorChar;
//             }
//
//             //复制原目录中的文件
//             var files = Directory.GetFiles(sourceDirName);
//             foreach (var file in files)
//             {
//                 try
//                 {
//                     Directory.Move(file, destDirName + Path.GetFileName(file));
//                 }
//                 catch (Exception e)
//                 {
//                     Debug.LogWarning(string.Format(e.Message));
//                 }
//             }
//
//             //对于复制源目录中的目录，递归调用
//             var dirs = Directory.GetDirectories(sourceDirName);
//             foreach (var dir in dirs)
//             {
//                 CopyDirectory(dir, destDirName + Path.GetFileName(dir));
//             }
//         }
//
//
//         void OnLoadedCustomManifest(AssetBundleHolder holder)
//         {
//             CustomManiFestAssetBundle = customManifestRequestInfo.assetBundle;
//
//             DefaultCustomManifestDataAsset customDataManifest =
//                 holder.LoadAsset("custom_manifest", typeof(DefaultCustomManifestDataAsset)) as
//                     DefaultCustomManifestDataAsset;
//             if (customDataManifest != null) CustomManifest = customDataManifest.CreateManifest(CurrentManifestHash);
//         }
//
//         //移动资源到目标路径的key，存储值为当前app版本号，当获取到版本号与存储版本号不同时，拷贝资源
//         private const string MoveAssetKey = "MoveAssetKey";
//
//         /// <summary>
//         /// 拷贝streamingasset资源目录到平时常用下载路径
//         /// </summary>
//         /// <returns></returns>
//         public IEnumerator CopyStreamToDataPath(Action<int, int> updateAct)
//         {
// #if UNITY_EDITOR
//             yield break;
// #endif
//
//             string appVersion = PlayerPrefs.GetString(MoveAssetKey, string.Empty);
//             if (appVersion == Application.version)
//             {
//                 yield break;
//             }
//
//             int loadCount = localStreamingAssetDic.Count;
//             int loadedCount = 0;
//
//             int maxCopyNum = 4; //最大同时拷贝数量
//             int copyNum = 0;
//             WaitUntil waitUntil = new WaitUntil(() => loadedCount >= loadCount);
//             WaitUntil waitCopyUntil = new WaitUntil(() => copyNum < maxCopyNum);
//             Action completeAct = () =>
//             {
//                 loadedCount++;
//                 copyNum--;
//                 updateAct(loadedCount, loadCount);
//             };
//
//             //拷贝AB资源
//             var entry = localStreamingAssetDic.GetEnumerator();
//             while (entry.MoveNext())
//             {
//                 string abName = entry.Current.Key;
//
//                 //Debug.Log(abName);
//
//                 string path = CreatStreamingAssetsPath(abName);
//
//                 string savePath = CreateCachePath(abName);
//
//
//                 if (copyNum >= 4)
//                 {
//                     yield return waitCopyUntil;
//                 }
//
//                 copyNum++;
//                 StartCoroutine(CopyFile(abName, path, savePath, completeAct, true));
//             }
//
//             entry.Dispose();
//
//
//             yield return waitUntil;
//
//             PlayerPrefs.SetString(MoveAssetKey, Application.version);
//             yield break;
//         }
//
//         /// <summary>
//         /// 拷贝文件，如果是assetbundle先读取文件读取版本号是否一致，如果是视频则对比视频本地存储的版本号
//         /// </summary>
//         /// <param name="sourcePath"></param>
//         /// <param name="targetPath"></param>
//         /// <param name="completeAct"></param>
//         /// <param name="isAssetbundle"></param>
//         /// <returns></returns>
//         private IEnumerator CopyFile(string assetbundleName, string sourcePath, string targetPath,
//             Action completeAct = null, bool isAssetbundle = false)
//         {
//             if (File.Exists(targetPath))
//             {
//                 bool isExist = false;
//                 try
//                 {
//                     if (isAssetbundle)
//                     {
//                         using (Stream stream = File.OpenRead(targetPath))
//                         {
//                             using (BinaryReader reader = new BinaryReader(stream))
//                             {
//                                 byte[] bytes = reader.ReadBytes(Hash128ByteLength);
//                                 string hash = System.Text.Encoding.UTF8.GetString(bytes);
//                                 if (hash == localStreamingAssetDic[assetbundleName]) //版本一致
//                                 {
//                                     isExist = true;
//                                 }
//                             }
//                         }
//                     }
//                 }
//                 catch (Exception e)
//                 {
//                     Debug.LogError(e.Message);
//                 }
//
//                 if (isExist)
//                 {
//                     yield return null;
//                     completeAct?.Invoke();
//                     yield break;
//                 }
//             }
// #if UNITY_EDITOR || UNITY_ANDROID
//             UnityWebRequest webRequest = UnityWebRequest.Get(sourcePath);
//             yield return webRequest.SendWebRequest();
//             try
//             {
//                 if (!string.IsNullOrEmpty(webRequest.error))
//                 {
//                     Debug.Log($"copy error:  {webRequest.error} {Path.GetFileName(sourcePath)}");
//                 }
//                 else
//                 {
//                     string dirPath = Path.GetDirectoryName(targetPath);
//                     if (!Directory.Exists(dirPath))
//                     {
//                         Directory.CreateDirectory(dirPath);
//                     }
//
//                     using (Stream stream = File.OpenWrite(targetPath))
//                     {
//                         using (BinaryWriter binary = new BinaryWriter(stream))
//                         {
//                             binary.Write(webRequest.downloadHandler.data);
//                         }
//                     }
//                 }
//             }
//             catch (Exception e)
//             {
//             }
//             finally
//             {
//                 completeAct?.Invoke();
//             }
// #else
// 			try
// 			{
// 				string dirPath = Path.GetDirectoryName(targetPath);
// 				if (!Directory.Exists(dirPath))
// 				{
// 					Directory.CreateDirectory(dirPath);
// 				}
// 				File.Copy(sourcePath, targetPath);
// 			}
// 			catch (Exception e)
// 			{
// 				Debug.LogError(e.Message);
// 			}
// 			finally
// 			{
//
// 			}
// 			yield return null;
// 			completeAct?.Invoke();
// #endif
//         }
//
//         /// <summary>
//         /// 请求加载清单
//         /// </summary>
//         /// <param name="hash">加载清单的hash值</param>
//         public void RequestLoadingManifest(string hash)
//         {
//             if (!impl.RequireManifest())
//             {
//                 return;
//             }
//
//             // 相同hash值的情况下不加载
//             if (CurrentManifestHash == hash)
//             {
//                 return;
//             }
//
//             // 把旧的CustomManifest、CurrentManifestHash设为空
//             CustomManifest = null;
//             CurrentManifestHash = hash;
//
//             if (CustomManiFestAssetBundle != null)
//             {
//                 CustomManiFestAssetBundle.Unload(true);
//             }
//
//
//             // 自定义customManifest的载入
//             customManifestRequestInfo = new RequestInfo();
//             var customManifestName = impl.GetCustomManifestName(this, hash);
//             customManifestRequestInfo.assetBundleName = customManifestName;
//             customManifestRequestInfo.hashString = hash;
//             customManifestRequestInfo.onLoaded = OnLoadedCustomManifest;
//             customManifestRequestInfo.onError = OnErrorGlobal;
//
//             customManifestRequestInfo.StartLoading();
//             currentDownloads[0] = customManifestRequestInfo;
//         }
//
//         /// <summary>
//         /// 检查内存状态
//         /// </summary>
//         /// <returns>是否需要卸载</returns>
//         public bool CheckMemState()
//         {
//             NoUnload = true;
//             //取出内存总量
//             usedMemory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1024 / 1024;
//             totalMempry = SystemInfo.systemMemorySize;
//             if (usedMemory <= ((totalMempry / 2) - 100)) return false;
//             NoUnload = false;
//             return true;
//         }
//
//         /// <summary>
//         /// 卸载过程
//         /// </summary>
//         private void ProcessUnload()
//         {
//             if (NoUnload) return;
//             NoUnload = true;
//             // AssetBundle的释放处理
//             var isNeededClearUnloadList = unloadList.Count > 0;
//             for (var i = 0; i < unloadList.Count; ++i)
//             {
//                 //移除缓存
//                 loaderAssetBundles.Remove(unloadList[i].key);
//                 if (unloadList[i].assetBundle != null)
//                 {
//                     //卸载对应 是否卸载
//                     unloadList[i].assetBundle.Unload(unloadList[i].unloadAllLoadedObjects);
//                 }
//             }
//
//             //需要卸载list数量大于0，清理过程结束，清空list
//             if (isNeededClearUnloadList)
//             {
//                 unloadList.Clear();
//             }
//         }
//
//         /// <summary>
//         /// 程序下载 过程
//         /// </summary>
//         private void ProcessDownloading()
//         {
//             // 当前请求下载的处理
//             for (var i = 0; i < GetMaxConcurrencyDownloadNum(); ++i)
//             {
//                 if (currentDownloads[i] == null)
//                 {
//                     continue;
//                 }
//
//                 if (currentDownloads[i].Update())
//                 {
//                     var loadedRequest = currentDownloads[i];
//                     currentDownloads[i] = null;
//                     RegisterAssetBundleAndCallOnLoadedDelegate(loadedRequest);
//                 }
//             }
//         }
//
//         /// <summary>
//         /// 从指定的Queue开始新的下载
//         /// </summary>
//         /// <param name="requestQueue">指定的queue</param>
//         /// <param name="j">自定下标</param>
//         void ProcessNewRequest(Queue<RequestInfo> requestQueue, int j)
//         {
//             // peek后，如果有依存关系的东西，就先要求那个。
//             var request = requestQueue.Peek();
//             var dependenceRequest = request.dependencies.Find(r => { return r.IsNotStartedLoading(); });
//             if (dependenceRequest != null)
//             {
//                 while (dependenceRequest != null)
//                 {
//                     // 如果没有加载，就发出请求
//                     if (!CheckAlreadyLoadedAndCallLoadedEvent(dependenceRequest))
//                     {
//                         // 如果现在正在请求的话就交给你处理
//                         if (!CheckAlreadyLoadingSameFileAndAddLoadedEvent(dependenceRequest))
//                         {
//                             currentDownloads[j] = dependenceRequest;
//                             break;
//                         }
//                     }
//
//                     // 寻找下一个需求
//                     dependenceRequest = request.dependencies.Find(r => { return r.IsNotStartedLoading(); });
//                 }
//             }
//             else
//             {
//                 //在依赖对象错误的情况下也视为加载完成的基础上，依赖包是否全部加载完成
//                 var isLoadedAllDependencies =
//                     request.dependencies.Find(r => { return !r.loaded && r.errorInfo == null; }) == null;
//                 if (!isLoadedAllDependencies)
//                 {
//                     return;
//                 }
//
//                 request = requestQueue.Dequeue();
//
//                 //如果依赖对象有错误，通过调用者的请求的错误处理进行处理
//                 var dependenciesError = GetDependenciesError(request);
//                 if (dependenciesError != null)
//                 {
//                     request.errorInfo = dependenciesError;
//                     if (request.onError != null)
//                     {
//                         request.onError(dependenciesError);
//                         ;
//                     }
//
//                     return;
//                 }
//
//                 // 没有加载就加载
//                 if (!CheckAlreadyLoadedAndCallLoadedEvent(request))
//                 {
//                     if (!CheckAlreadyLoadingSameFileAndAddLoadedEvent(request))
//                     {
//                         currentDownloads[j] = request;
//                     }
//                 }
//             }
//
//             if (currentDownloads[j] != null)
//             {
//                 currentDownloads[j].StartLoading();
//             }
//         }
//
//         /// <summary>
//         /// 对优先级为priority的queue尝试新下载
//         /// </summary>
//         /// <param name="priority">优先级</param>
//         private void TryProcessNewRequestForPriority(int priority)
//         {
//             var requestQueue = requestQueues[priority];
//             // 没有请求就不做是否请求的处理
//             if (requestQueue.Count == 0)
//             {
//                 return;
//             }
//
//             // 如果同时下载数量有空闲，就提出请求
//             for (var j = 0; j < GetMaxConcurrencyDownloadNum(); ++j)
//             {
//                 // 进行下载处理的结果，当队列为空时结束
//                 if (requestQueue.Count == 0)
//                 {
//                     break;
//                 }
//
//                 // 下载框填满后进入下一个
//                 if (currentDownloads[j] != null)
//                 {
//                     continue;
//                 }
//
//                 ProcessNewRequest(requestQueue, j);
//             }
//         }
//
//         /// <summary>
//         /// 发起新的请求
//         /// </summary>
//         private void TryProcessNewRequest()
//         {
//             // 按照优先顺序进行新请求处理
//             for (var i = 0; i < requestQueues.Length; ++i)
//             {
//                 TryProcessNewRequestForPriority(i);
//             }
//         }
//
//         /// <summary>
//         /// 设置启用AssetBundle加载
//         /// </summary>
//         /// <param name="enable">是否</param>
//         /// <param name="isResetRequestQueue">是重置请求队列</param>
//         public void SetEnableAssetBundleLoad(bool enable, bool isResetRequestQueue = false)
//         {
//             CanLoadAssetBundle = enable;
//             AssetBundleNameHashSet.Clear();
//
//             if (!isResetRequestQueue) return;
//             requestQueues[(int)Priority.Low].Clear();
//             requestQueues[(int)Priority.Normal].Clear();
//             requestQueues[(int)Priority.High].Clear();
//         }
//
//         /// <summary>
//         /// 是否有当前正在加载的资产包
//         /// </summary>
//         public bool HasLoading()
//         {
//             if (currentDownloads == null || currentDownloads.Length <= 0)
//             {
//                 return false;
//             }
//
//             return currentDownloads.Any(t => t != null);
//         }
//
//         /// <summary>
//         /// 是否有等待加载的请求
//         /// </summary>
//         public bool HasRequestQueue()
//         {
//             if (requestQueues == null || requestQueues.Length <= 0)
//             {
//                 return false;
//             }
//
//             return requestQueues.Any(t => t.Count > 0);
//         }
//
//         /// <summary>
//         /// 检查是否正在加载相同的文件，并将Loaded事件添加到正在加载的文件中
//         /// </summary>
//         /// <param name="request">请求单</param>
//         /// <returns></returns>
//         private bool CheckAlreadyLoadingSameFileAndAddLoadedEvent(RequestInfo request)
//         {
//             foreach (var t in currentDownloads)
//             {
//                 if (t == null || t.assetBundleName != request.assetBundleName) continue;
//                 t.Join(request);
//                 return true;
//             }
//
//             return false;
//         }
//
//         /// <summary>
//         /// 如果已经加载，返回true，调用onLoadedEvent
//         /// </summary>
//         /// <param name="request"></param>
//         /// <returns></returns>
//         private bool CheckAlreadyLoadedAndCallLoadedEvent(RequestInfo request)
//         {
//             //unloadAllLoadedObjects标记如果要求有发展完毕的AssetBundleHolder也没有旗帜站起来和意图行为也许不对应,所以思考
//             if (!loaderAssetBundles.TryGetValue((uint)request.hashString.GetHashCode(), out var holder)) return false;
//             request.MarkStarted();
//             request.onLoaded?.Invoke(new AssetBundleHolder(holder));
//
//             return true;
//         }
//
//         /// <summary>
//         /// 注册加载的AssetBundle，调用OnLoaded
//         /// 注册AssetBundle和调用OnLoaded委托
//         /// </summary>
//         /// <param name="request"></param>
//         private void RegisterAssetBundleAndCallOnLoadedDelegate(RequestInfo request)
//         {
//             Assert.IsNotNull(request.assetBundle, request.assetUrlPath);
//
//             if (request.assetBundle == null)
//             {
//                 return;
//             }
//
//             // onLoaded为空的时候没有参考，直接释放AssetBundle也没问题
//             if (request.onLoaded != null)
//             {
//                 var key = (uint)request.hashString.GetHashCode();
//                 var holder = new AssetBundleHolder(key, request.assetBundle, request.dependenceHolders,
//                     request.unloadAllLoadedObjects);
//                 loaderAssetBundles.Add(holder.key, holder);
//                 request.onLoaded(new AssetBundleHolder(holder));
//             }
//             else
//             {
//                 request.assetBundle.Unload(true);
//                 request.assetBundle = null;
//             }
//
//             request.loaded = true;
//         }
//
//         /// <summary>
//         /// 错误可以重试吗? 那些情况下可以
//         /// </summary>
//         /// <param name="errorInfo"></param>
//         /// <returns></returns>
//         public virtual bool IsRetryableErrorType(ErrorInfo errorInfo)
//         {
//             switch (errorInfo.type)
//             {
//                 case ErrorInfo.Type.TIME_OUT:
//                 case ErrorInfo.Type.FAILED_WWW_REQUEST:
//                     return true;
//                 default:
//                     return false;
//             }
//         }
//
//         /// <summary>
//         /// 依赖关系方是否存在错误
//         /// </summary>
//         /// <param name="request"></param>
//         /// <returns></returns>
//         private ErrorInfo GetDependenciesError(RequestInfo request)
//         {
//             ErrorInfo errorInfo = null;
//             var dependencies = request.dependencies;
//
//             if (dependencies == null) return null;
//
//             foreach (var t in dependencies.Where(t => t.errorInfo != null))
//             {
//                 //不可重试的错误基本是致命的，所以优先回复
//                 if (!IsRetryableErrorType(t.errorInfo)) return t.errorInfo;
//                 errorInfo = t.errorInfo;
//                 continue;
//             }
//
//             return errorInfo;
//         }
//
//         /// <summary>
//         /// 从Dictionary中删除AssetBundle
//         /// </summary>
//         /// <param name="key">assetBundle的key</param>
//         /// <param name="assetBundle">assetBundle</param>
//         /// <param name="unloadAllLoadedObjects">是否卸载</param>
//         public void Release(uint key, AssetBundle assetBundle, bool unloadAllLoadedObjects)
//         {
//             for (var i = 0; i < unloadList.Count; i++)
//             {
//                 if (unloadList[i].assetBundle.name == assetBundle.name)
//                 {
//                     return;
//                 }
//             }
//
//             var unloadInfo = new UnloadInfo
//             {
//                 assetBundle = assetBundle,
//                 key = key,
//                 unloadAllLoadedObjects = unloadAllLoadedObjects
//             };
//             unloadList.Add(unloadInfo);
//         }
//
//         // 完成等待执行的请求
//         public void ClearRequestQueues()
//         {
//             if (requestQueues == null) return;
//
//             var errorInfo = new ErrorInfo
//             {
//                 type = ErrorInfo.Type.ABORT_LOADING_REQUSET,
//                 message = AbortLoadingRequestErrorMessage
//             };
//             foreach (var requestQueue in requestQueues)
//             {
//                 while (requestQueue.Count > 0)
//                 {
//                     var requestInfo = requestQueue.Dequeue();
//                     requestInfo.onError?.Invoke(errorInfo);
//                 }
//             }
//         }
//
//         /// <summary>
//         /// 现在执行中的请求也中止
//         /// </summary>
//         /// <param name="onComplete">结束之后执行事件</param>
//         /// <returns></returns>
//         public IEnumerator AbortAllLoadingRequest(System.Action onComplete = null)
//         {
//             // 完成等待请求
//             ClearRequestQueues();
//             // 停止处理当前请求中的加载
//             var workingRequests = new Queue<RequestInfo>();
//             for (var i = 0; i < GetMaxConcurrencyDownloadNum(); ++i)
//             {
//                 if (currentDownloads[i] == null)
//                 {
//                     continue;
//                 }
//
//                 workingRequests.Enqueue(currentDownloads[i]);
//                 //停止OnLoad处理以免被在度请求
//                 currentDownloads[i] = null;
//             }
//
//             while (workingRequests.Count > 0)
//             {
//                 var request = workingRequests.Dequeue();
//                 yield return request.AbortLoading();
//             }
//
//             onComplete?.Invoke();
//         }
//     }
// }
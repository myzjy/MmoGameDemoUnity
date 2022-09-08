// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using Framework.AssetBundle.AsyncOperation;
// using Framework.AssetBundles.Config;
// using Framework.AssetBundles.Utilty;
// using UnityEditor;
// using UnityEngine;
// using ZJYFrameWork.AssetBundleLoader;
// using ZJYFrameWork.Log;
//
// namespace ZJYFrameWork.AssetBundles
// {
//     public class ApplicationNetworkAssetBundleLoaderImpl : NetworkAssetBundleLoaderImpl
//     {
//         /// <summary>
//         /// 回到标题等的重新启动中吗?
//         /// </summary>
//         // ReSharper disable once InconsistentNaming
// #pragma warning disable CS0169
//         private bool isRebooting;
// #pragma warning restore CS0169
//         public override bool IsInitialized(AssetBundleLoaderBase loader)
//         {
//             return !isRebooting && base.IsInitialized(loader);
//         }
//
//         public void StartReboot()
//         {
//             isRebooting = true;
//         }
//
//         public void EndReboot()
//         {
//             isRebooting = false;
//         }
//     }
// #if DEVELOP_BUILD
//     public class ApplicationNetworkAssetBundleForDevelopLoaderImpl : ApplicationNetworkAssetBundleLoaderImpl
//     {
//         public override string GetCustomManifestName(AssetBundleLoaderBase loader, string hash)
//         {
// #if UNITY_ANDROID
//             return $"{AssetBundleConfig.CustomManifestPrefix}android{AssetBundleConfig.CustomManifestSuffix}";
// #elif UNITY_IOS
// 			return  $"{AssetBundleConfig.CustomManifestPrefix}ios{AssetBundleConfig.CustomManifestSuffix}";
// #else
// 			return  $"{AssetBundleConfig.CustomManifestPrefix}pc{AssetBundleConfig.CustomManifestSuffix}";
// #endif
//
//             return base.GetCustomManifestName(loader, hash);
//         }
//     }
//
// #endif
//
//     public class AssetBundleLoader : AssetBundleLoaderBase
//     {
//         private static readonly ILog log = LogManager.GetLogger(typeof(AssetBundleLoader));
// #if DEVELOP_BUILD
//         /// <summary>
//         /// AB读取时是否添加了延迟(用于调试)
//         /// </summary>
//         public bool IsAdditionalLoadDelayEnabled { get; private set; }
//
//         private float additionalLoadDelayMin;
//         private float additionalLoadDelayMax;
//
//         /// <summary>
//         /// 启用在AB读取时追加延迟的设置(调试用)
//         /// </summary>
//         /// <param name="delayMin">延迟时间(最小値)</param>
//         /// <param name="delayMax">延迟时间(最大値)</param>
//         public void EnableAdditionalLoadDelay(float delayMin, float delayMax)
//         {
//             this.additionalLoadDelayMin = delayMin;
//             this.additionalLoadDelayMax = delayMax;
//             this.IsAdditionalLoadDelayEnabled = true;
//         }
//
//         /// <summary>
//         /// 禁用在AB读取时添加延迟的设置(调试用)
//         /// </summary>
//         public void DisableAdditionalLoadDelay()
//         {
//             this.IsAdditionalLoadDelayEnabled = false;
//         }
// #endif
//         /// <summary>
//         /// 获取
//         /// </summary>
//         /// <param name="assetBundleName"></param>
//         /// <param name="isIgnoreCachedBundleSize"></param>
//         /// <returns></returns>
//         public long GetAssetBundleSize(string assetBundleName, bool isIgnoreCachedBundleSize = true)
//         {
// #if UNITY_EDITOR
//             if (AssetBundleConfig.IsEditorMode)
//             {
//                 return 0;
//             }
// #endif
//             if (CustomManifest == null)
//             {
// #if DEVELOP_BUILD
//                 Debug.Log("CustomManifest is Null");
// #endif
//                 return 0;
//             }
//
//             assetBundleName = GetFixedAssetBundleName(assetBundleName);
//             if (isIgnoreCachedBundleSize && IsCached(assetBundleName))
//             {
//                 return 0;
//             }
//
//             return (long)CustomManifest.GetAssetBundleSize(assetBundleName);
//         }
//
//         /// <summary>
//         /// 计算AssetBundle大小
//         /// </summary>
//         /// <param name="assetBundleNames"></param>
//         /// <param name="isIgnoreCachedBundleSize"></param>
//         /// <returns></returns>
//         public long GetAssetBundleSize(IEnumerable<string> assetBundleNames, bool isIgnoreCachedBundleSize = true)
//         {
//             return assetBundleNames.Sum(t => GetAssetBundleSize(t, isIgnoreCachedBundleSize));
//         }
//
//         /// <summary>
//         /// 获取AssetBundle大小等待加载
//         /// 获取当前等待加载的资产包的大小
//         /// </summary>
//         /// <param name="isIgnoreCachedBundleSize">是否忽略缓存包大小</param>
//         /// <returns></returns>
//         public long GetAssetBundleSizeForWaitingLoad(bool isIgnoreCachedBundleSize = true)
//         {
//             string[] assetBundleNames = new string[AssetBundleNameHashSet.Count];
//             //将数组复制
//             AssetBundleNameHashSet.CopyTo(assetBundleNames);
//
//             //获取assetBundle数组的所有assetBundle大小
//             return GetAssetBundleSize(assetBundleNames, isIgnoreCachedBundleSize);
//         }
//
//         /// <summary>
//         /// 获取所有bundle name，用于更新全部资源
//         /// </summary>
//         /// <returns></returns>
//         public string[] GetAllAssetBundles()
//         {
//             return CustomManifest.GetAllAssetBundles();
//         }
//
//         /// <summary>
//         /// 获取所有依赖
//         /// </summary>
//         /// <param name="assetBundleName"></param>
//         /// <returns></returns>
//         public string[] GetAssetBundleDepend(string assetBundleName)
//         {
//             if (CustomManifest == null)
//             {
//                 return new string[] { };
//             }
//
//             return CustomManifest.GetAllDependencies(assetBundleName);
//         }
//
//
//         public new static AssetBundleLoader Instance => AssetBundleLoaderBase.Instance as AssetBundleLoader;
//
//
//         protected override AssetBundleLoaderImpl CreateImpl()
//         {
// #if UNITY_EDITOR
//             if (AssetBundleConfig.IsEditorMode)
//             {
//                 return new EditorAssetBundleAsyncLoader();
//             }
// #endif
// #if DEVELOP_BUILD
//             return new ApplicationNetworkAssetBundleForDevelopLoaderImpl();
// #else
//             return new ApplicationNetworkAssetBundleLoaderImpl();
// #endif
//         }
//
//         protected override void Init()
//         {
//             SetManifestUpdatedAction(() => { });
//             base.Init();
//         }
//
//         public void StartReboot()
//         {
//             if (GetAssetBundleLoaderImpl() is ApplicationNetworkAssetBundleLoaderImpl impl)
//             {
//                 impl.StartReboot();
//             }
//         }
//
//         public void EndReboot()
//         {
//             if (GetAssetBundleLoaderImpl() is ApplicationNetworkAssetBundleLoaderImpl impl)
//             {
//                 impl.EndReboot();
//             }
//         }
//
//         private string GetErrorDialogMessage(ErrorInfo error)
//         {
//             switch (error.type)
//             {
//                 case ErrorInfo.Type.TIME_OUT:
//                 case ErrorInfo.Type.FAILED_WWW_REQUEST:
//                     //可能是超时，也可能是切断Wifi
//                     return
//                         "网络连接错误。"; //MessageManager.I.GetMessage(Message.ASSET_BUNDLE_EROR_RETRY, Dpuzzle.Error.DetectAssetBundleError + (int)error.type);
//                 case ErrorInfo.Type.FAILED_DISK_FULL:
//                 case ErrorInfo.Type.FAILED_WRITE_CACHEFILE:
//                 case ErrorInfo.Type.FAILED_READ_CACHEFILE:
//                 case ErrorInfo.Type.FAILED_CREATE_DIRECTORY:
//                 case ErrorInfo.Type.FAILED_DELETE_CACHEFILE:
//                     //可能是由终端容量引起的错误
//                     return
//                         "无法进行下载。\n\n请确保足够的容量后再度进行尝试。\n\n※注意不要误删重要的应用程序。"; //MessageManager.I.GetMessage(Message.ASSET_BUNDLE_EROR_DISK_FULL);
//                 default:
//                     //标注错误类型
//                     // string message = MessageManager.I.GetMessage(Message.ASSET_BUNDLE_EROR_ETC, Dpuzzle.Error.DetectAssetBundleError+(int)error.type);
//                     // if (!string.IsNullOrEmpty(error.abError))
//                     // {
//                     //     message = $"{message}\n{error.abError}";
//                     // }
//                     // //出错
//                     // Debug.LogErrorFormat("Error:Type{0}:Message{1}",error.type.ToString(),message);
//                     // return message;
//                     break;
//             }
//
//             return "";
//         }
//
//         /// <summary>
//         /// 资产包的错误信息
//         /// </summary>
//         private class LoadErrorInfo
//         {
//             public ErrorInfo errorInfo;
//             public System.Action retryCallback;
//         }
//
//         /// <summary>
//         /// 资产包错误信息的列表
//         /// </summary>
//         private List<LoadErrorInfo> m_LoadErrors = new List<LoadErrorInfo>();
//
//         private void LoadFailedCallback(string assetBundleName, ErrorInfo errorInfo, System.Action retryCallback = null,
//             OnErrorDelegate onErrorCallback = null)
//         {
// #if DEVELOP_BUILD
//             Debug.LogErrorFormat("LoadFailedCallback:{0} ,{1} , {2}", assetBundleName, errorInfo.type,
//                 errorInfo.message);
// #endif
//             // 如果你有自己的错误处理方法，就交给你。
//             if (onErrorCallback != null)
//             {
//                 onErrorCallback(errorInfo);
//                 return;
//             }
//
//             // 在公共错误处理中忽略请求的中断
//             if (errorInfo.type == ErrorInfo.Type.ABORT_LOADING_REQUSET)
//             {
//                 return;
//             }
//
//             // 总结同种错误
//             var info = m_LoadErrors.Find(x => x.errorInfo.type == errorInfo.type);
//             if (info != null)
//             {
//                 if (AssetBundleLoader.Instance.IsRetryableErrorType(errorInfo))
//                 {
//                     //总结重试
//                     info.retryCallback += retryCallback;
//                 }
//
//                 return;
//             }
//
//             info = new LoadErrorInfo
//             {
//                 errorInfo = errorInfo,
//                 retryCallback = retryCallback
//             };
//             m_LoadErrors.Add(info);
//
//             bool isRetryable = AssetBundleLoader.Instance.IsRetryableErrorType(errorInfo);
//             string message = GetErrorDialogMessage(errorInfo);
//
//
//             // // 错误弹出显示
//             // CommonUIManager.I.OpenNetworkErrorDialog(isRetryable ? Dialog.ButtonType.YesNo : Dialog.ButtonType.OK, message, (result) =>
//             // {
//             //     //可以重试，只有回答YES时才进行重试
//             //     if (isRetryable && result == Dialog.Result.Yes)
//             //     {
//             //         if (SoundManager.IsValid())
//             //         {
//             //             SoundManager.I.PlaySE(SE_ID.TAP_CLOSE);
//             //         }
//             //         if (info != null)
//             //         {
//             //             if (info.retryCallback != null)
//             //             {
//             //                 info.retryCallback();
//             //             }
//             //             m_LoadErrors.Remove(info);
//             //         }
//             //         else
//             //         {
//             //             //没有info的情况是不可能的，所以在标题中
//             //             Debug.Assert(false);
//             //             LoadFailedReturenTitle();
//             //         }
//             //     }
//             //     else
//             //     {
//             //         if (SoundManager.IsValid())
//             //         {
//             //             SoundManager.I.PlaySE(SE_ID.APPEAR_MENU_WINDOW);
//             //         }
//             //         //不能重试的情况下返回标题
//             //         LoadFailedReturenTitle();
//             //     }
//             // });
//         }
//
//         private bool IsCacheByFixedAssetBundleName(string assetBundleName)
//         {
// #if UNITY_EDITOR
//             if (AssetBundleConfig.IsEditorMode)
//             {
//                 return true;
//             }
// #endif
// #if DEVELOP_BUILD
//             if (IsAdditionalLoadDelayEnabled)
//             {
//                 return false;
//             }
// #endif
//             return IsCached(GetFixedAssetBundleName(assetBundleName));
//         }
//
//         private string GetFixedAssetBundleName(string assetBundleName)
//         {
//             if (!assetBundleName.Contains(GetBundleExt()))
//             {
//                 assetBundleName = assetBundleName + GetBundleExt();
//             }
//
//             return assetBundleName;
//         }
//
//         private const string AssetBundleLogOutputText = "[AssetBundle] Download: {0}";
//
//         /// <summary>
//         /// 没有指定数据的加载请求错误处理的东西作为共同错误处理超时自动进行重试
//         /// </summary>
//         /// <param name="assetBundleName">AssetBundle名</param>
//         /// <param name="onLoaded">在加载完成时被调用</param>
//         /// <param name="onError">发生错误时被调用</param>
//         /// <param name="priority">进行加载的优先顺序</param>
//         /// <param name="isShowLoading">是否进行数据加载显示.</param>
//         /// <param name="unloadAllLoadedObjects"></param>
//         public void Load(string assetBundleName, OnLoadedDelegate onLoaded = null, OnErrorDelegate onError = null,
//             Priority priority = Priority.Normal, bool isShowLoading = true, bool unloadAllLoadedObjects = false)
//         {
//             bool isDownload = false;
//
// #if UNITY_EDITOR && DEVELOP_BUILD
//             AddLog(assetBundleName);
// #endif
//
//             if (!IsCacheByFixedAssetBundleName(assetBundleName))
//             {
// #if UNITY_EDITOR && DEVELOP_BUILD
//                 if (AssetBundleConfig.IsEditorMode)
//                 {
//                     Debug.Log(string.Format(AssetBundleLogOutputText, assetBundleName));
//                 }
// #endif
//                 if (isShowLoading)
//                 {
//                     // CommonUIManager.I.DataLoading.AddShowCount();
//                 }
//
//                 isDownload = true;
//             }
//
//             OnLoadedDelegate onLoadedAndSubShowCount = null;
//             if (onLoaded != null)
//             {
//                 onLoadedAndSubShowCount += onLoaded;
//             }
//
//             System.Action onErrorSubShowCount = null;
//             if (isShowLoading)
//             {
//                 onLoadedAndSubShowCount += (holder) =>
//                 {
//                     // if (isDownload && CommonUIManager.IsValid())
//                     // {
//                     //     CommonUIManager.I.DataLoading.SubShowCount();
//                     // }
//                 };
//                 onErrorSubShowCount = () =>
//                 {
//                     // if (isDownload && CommonUIManager.IsValid())
//                     // {
//                     //     CommonUIManager.I.DataLoading.SubShowCount();
//                     // }
//                 };
//             }
//
//             OnErrorDelegate errorWrapper = errorInfo =>
//             {
//                 if (onErrorSubShowCount != null)
//                 {
//                     onErrorSubShowCount();
//                 }
//
//                 System.Action retryCallback = null;
//                 if (IsRetryableErrorType(errorInfo))
//                 {
//                     retryCallback = () => { Load(assetBundleName, onLoaded, onError, priority, isShowLoading); };
//                 }
//
//                 LoadFailedCallback(assetBundleName, errorInfo, retryCallback, onError);
//             };
//
// #if DEVELOP_BUILD
//
//             base.Load(assetBundleName, onLoadedAndSubShowCount, errorWrapper, priority, null,
//                 unloadAllLoadedObjects);
//
// #else
// 		base.Load(assetBundleName, onLoadedAndSubShowCount, errorWrapper, priority, null, unloadAllLoadedObjects);
// #endif
//         }
//
//         /// <summary>
//         /// 加载AssetBundle，返回特定内容
//         /// 没有指定错误处理的东西作为共同错误处理超时自动进行重试
//         /// </summary>
//         /// <param name="assetBundleName">Asset bundle name.</param>
//         /// <param name="onLoaded">On loaded.</param>
//         /// <param name="onError">On error.</param>
//         /// <param name="priority">Priority.</param>
//         /// <param name="assetName">Asset name.</param>
//         /// <param name="isShowLoading">是否进行数据加载显示</param>
//         /// <typeparam name="T">第1类参数.</typeparam>
//         public void Load<T>(string assetBundleName, OnMainAssetLoadedDelegate<T> onLoaded = null,
//             OnErrorDelegate onError = null, Priority priority = Priority.Normal, string assetName = null,
//             bool isShowLoading = true, bool unloadAllLoadedObjects = false) where T : class
//         {
//             bool isDownload = false;
// #if UNITY_EDITOR && DEVELOP_BUILD
//             AddLog(assetBundleName);
// #endif
//
//             if (!IsCacheByFixedAssetBundleName(assetBundleName))
//             {
// #if UNITY_EDITOR && DEVELOP_BUILD
//                 if (AssetBundleConfig.IsEditorLogMode)
//                 {
//                     Debug.Log(string.Format(AssetBundleLogOutputText, assetBundleName));
//                 }
// #endif
//                 if (isShowLoading)
//                 {
//                     // CommonUIManager.I.DataLoading.AddShowCount();
//                 }
//
//                 isDownload = true;
//             }
//
//             OnMainAssetLoadedDelegate<T> onLoadedAndSubShowCount = null;
//             if (onLoaded != null)
//             {
//                 onLoadedAndSubShowCount += onLoaded;
//             }
//
//             System.Action onErrorSubShowCount = null;
//             if (isShowLoading)
//             {
//                 onLoadedAndSubShowCount += (asset) =>
//                 {
//                     // if (isDownload && CommonUIManager.IsValid())
//                     // {
//                     //     CommonUIManager.I.DataLoading.SubShowCount();
//                     // }
//                 };
//                 onErrorSubShowCount = () =>
//                 {
//                     // if (isDownload && CommonUIManager.IsValid())
//                     // {
//                     //     CommonUIManager.I.DataLoading.SubShowCount();
//                     // }
//                 };
//             }
//
//             OnErrorDelegate errorWrapper = errorInfo =>
//             {
//                 if (onErrorSubShowCount != null)
//                 {
//                     onErrorSubShowCount();
//                 }
//
//                 Action retryCallback = null;
//                 if (IsRetryableErrorType(errorInfo))
//                 {
//                     retryCallback = () =>
//                     {
//                         Load(assetBundleName, onLoaded, onError, priority, assetName, isShowLoading);
//                     };
//                 }
//
//                 LoadFailedCallback(assetBundleName, errorInfo, retryCallback, onError);
//             };
//
// #if UNITY_EDITOR && DEVELOP_BUILD
//             AddLog(assetBundleName);
// #endif
//
// #if DEVELOP_BUILD
//             base.Load(assetBundleName, onLoadedAndSubShowCount, errorWrapper, priority, assetName, null,
//                 unloadAllLoadedObjects);
// #else
// 		base.Load(assetBundleName, onLoadedAndSubShowCount, errorWrapper, priority, assetName, null,
// 			unloadAllLoadedObjects);
// #endif
//         }
// #if UNITY_EDITOR && DEVELOP_BUILD
//
//         private void AddLog(string name)
//         {
//             if (!name.Contains(GetBundleExt()))
//             {
//                 name += GetBundleExt();
//             }
//
//             var Dependencies = CustomManifest.GetAllDependencies(name);
//             StringBuilder sb = new StringBuilder();
//             foreach (var st in Dependencies)
//             {
//                 sb.Append($"{st},");
//             }
//
//             // var format = string.Format("{...}",CustomManifest.GetAllDependencies(name)) ?? throw new ArgumentNullException("string.Format(\"{...}\",CustomManifest.GetAllDependencies(name))");
//             log.Debug($"{name},{sb.ToString()}");
//         }
//
// #endif
//         /// <summary>
//         /// 加载多个AssetBundle没有指定错误处理的东西作为共同错误处理超时自动进行重试
//         /// </summary>
//         /// <param name="assetBundleNames">Asset bundle names</param>
//         /// <param name="onLoad">OnLoad</param>
//         /// <param name="onError">onError</param>
//         /// <param name="priority">priority</param>
//         /// <param name="isShowLoading">是否进行数据加载显示</param>
//         public void Load(string[] assetBundleNames, OnMultiObjectLoadDelegate onLoad = null,
//             OnErrorDelegate onError = null, Priority priority = Priority.Normal, bool isShowLoading = true)
//         {
//             bool isDownload = false;
//             for (int i = 0; i < assetBundleNames.Length; ++i)
//             {
// #if UNITY_EDITOR && DEVELOP_BUILD
//                 AddLog(assetBundleNames[i]);
// #endif
//                 if (!IsCacheByFixedAssetBundleName(assetBundleNames[i]))
//                 {
// #if UNITY_EDITOR && DEVELOP_BUILD
//                     if (AssetBundleConfig.IsEditorLogMode)
//                     {
//                         Debug.Log(string.Format(AssetBundleLogOutputText, assetBundleNames[i]));
//                     }
// #endif
//                     if (!isDownload && isShowLoading)
//                     {
//                         // CommonUIManager.I.DataLoading.AddShowCount();
//                     }
//
//                     isDownload = true;
//                 }
//             }
//
//             OnMultiObjectLoadDelegate onLoadedAndSubShowCount = null;
//             if (onLoad != null)
//             {
//                 onLoadedAndSubShowCount += onLoad;
//             }
//
//
//             System.Action onErrorSubShowCount = null;
//             if (isShowLoading)
//             {
//                 onLoadedAndSubShowCount += (holders) =>
//                 {
//                     // if (isDownload && CommonUIManager.IsValid())
//                     // {
//                     //     CommonUIManager.I.DataLoading.SubShowCount();
//                     // }
//                 };
//                 onErrorSubShowCount += () =>
//                 {
//                     // if (isDownload && CommonUIManager.IsValid())
//                     // {
//                     //     CommonUIManager.I.DataLoading.SubShowCount();
//                     // }
//                 };
//             }
//
//             //只发送一次重试
//             bool isSendRetryRequest = false;
//
//             base.Load(assetBundleNames, onLoadedAndSubShowCount, (errorInfo) =>
//             {
//                 if (onErrorSubShowCount != null)
//                 {
//                     onErrorSubShowCount();
//                 }
//
//                 System.Action retryCallback = null;
//                 if (IsRetryableErrorType(errorInfo) && !isSendRetryRequest)
//                 {
//                     isSendRetryRequest = true;
//                     retryCallback = () => { Load(assetBundleNames, onLoad, onError, priority, isShowLoading); };
//                 }
//
//                 LoadFailedCallback("", errorInfo, retryCallback, onError);
//             }, priority, null);
//         }
//
//         public IEnumerator LoadSync(string assetBundleName, OnLoadedDelegate onLoaded = null,
//             OnErrorDelegate onError = null, Priority priority = Priority.Normal, bool isShowLoading = true,
//             bool unloadAllLoadedObjects = false)
//         {
//             bool hasComplete = false;
//             onLoaded += (holder) => hasComplete = true;
//
//             if (onError != null)
//             {
//                 onError += (error) => hasComplete = true;
//             }
//
//             Load(assetBundleName, onLoaded, onError, priority, isShowLoading, unloadAllLoadedObjects);
//             yield return new WaitUntil(() => hasComplete);
//         }
//
//         public IEnumerator LoadSync<T>(string assetBundleName, OnMainAssetLoadedDelegate<T> onLoaded = null,
//             OnErrorDelegate onError = null, Priority priority = Priority.Normal, string assetName = null,
//             bool isShowLoading = true, bool unloadAllLoadedObjects = false) where T : class
//         {
//             bool hasComplete = false;
//             onLoaded += (holder) => hasComplete = true;
//
//             if (onError != null)
//             {
//                 onError += (error) => hasComplete = true;
//             }
//
//             Load<T>(assetBundleName, onLoaded, onError, priority, assetName, isShowLoading, unloadAllLoadedObjects);
//             yield return new WaitUntil(() => hasComplete);
//         }
//         protected override void OnErrorGlobal(ErrorInfo error)
//         {
//             Debug.LogFormat("ErrorType : {0} , Message : {1}", error.type.ToString(), error.message);
//             switch (error.type)
//             {
//                 case ErrorInfo.Type.ABORT_LOADING_REQUSET:
//                     //请求中断时，对错误什么都不做
//                     break;
//                 case ErrorInfo.Type.FAILED_WRITE_CACHEFILE:
//                 case ErrorInfo.Type.FAILED_READ_CACHEFILE:
//                 case ErrorInfo.Type.FAILED_CREATE_DIRECTORY:
//                 case ErrorInfo.Type.FAILED_DELETE_CACHEFILE:
//                 case ErrorInfo.Type.FAILED_DISK_FULL:
//                     // CommonUIManager.I.OpenDialog(Dialog.ButtonType.OK, string.Empty, GetErrorDialogMessage(error), (result) => {
//                     //     if (result == Dialog.Result.OK)
//                     //     {
//                     //         if (SoundManager.IsValid())
//                     //         {
//                     //             SoundManager.I.PlaySE(SE_ID.APPEAR_MENU_WINDOW);
//                     //         }
//                     //         SceneManager.I.BackToTitleForce();
//                     //     }
//                     // });
//                     break;
//                 case ErrorInfo.Type.DOWNLOAD_BUNDLE_INVALID:
//                 case ErrorInfo.Type.CACHEFILE_BUNDLE_INVALID:
//                 default:
//                     // CommonUIManager.I.OpenDialog(Dialog.ButtonType.OK, string.Empty, GetErrorDialogMessage(error), (result) => {
//                     //     if (result == Dialog.Result.OK)
//                     //     {
//                     //         if (SoundManager.IsValid())
//                     //         {
//                     //             SoundManager.I.PlaySE(SE_ID.APPEAR_MENU_WINDOW);
//                     //         }
//                     //         SceneManager.I.BackToTitleForce();
//                     //     }
//                     // });
//                     break;
//             }
//         }
//
//         private string assetBaseUri = "";
//         public override string GetAssetBaseUrl()
//         {
//             return "assetBaseUri";
//         }
//         public void SetAssetBaseUri(string uri)
//         {
//             assetBaseUri = $"{uri}/assets/{AssetBundleUtility.GetPlatformName()}";
//         }
//         public static void ClearCache()
//         {
//             // // BGM止める
//             //
//             // // スクリーンフェードをかけておく
//             // CommonUIManager.I.ShowFade(onComplete: () => {
//             //     // 何もない空のシーンに飛ばす
//             //     UnityEngine.SceneManagement.SceneManager.LoadScene("Game/Scene/Empty");
//             // });
//         }
//
//         private new void LateUpdate()
//         {
//             if (!CanLoadAssetBundle)
//             {
//                 return;
//             }
//             UnityEngine.Profiling.Profiler.BeginSample("1");
//             UnityEngine.Profiling.Profiler.EndSample();
//             UnityEngine.Profiling.Profiler.BeginSample("2");
//             UnityEngine.Profiling.Profiler.EndSample();
//             UnityEngine.Profiling.Profiler.BeginSample("3");
//             base.LateUpdate();
//             UnityEngine.Profiling.Profiler.EndSample();
//         }
//     }
// }
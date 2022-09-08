// using System.Collections;
// using System.IO;
// using System.IO.IsolatedStorage;
// using Framework.AssetBundles.Utilty;
// using UnityEngine;
// using UnityEngine.Networking;
//
// namespace ZJYFrameWork.AssetBundleLoader
// {
//     public class DownloadRequest : LoadingMethod
//     {
//         private UnityWebRequest www;
//
//         public override void StartLoading()
//         {
//             www = new UnityWebRequest(AssetBundleLoaderBase.Instance.GetAssetBundleLoaderImpl()
//                 .CreateUrl(AssetBundleLoaderBase.Instance, info.assetBundleName, info.hashString));
//             info.timeAtRequest = Time.time;
// #if DEVELOP_BUILD
//             Debug.Log(www.url);
// #endif
//             info.onDownload?.Invoke();
//             AssetBundleLoaderBase.Instance.StartCoroutine(StartUnityWebRequest());
//         }
//
//         public IEnumerator StartUnityWebRequest()
//         {
//             yield return www.SendWebRequest();
//         }
//
//         private const int ERROR_DISK_FULL = 0x70;
//         private const int ERROR_HANDLE_DISK_FULL = 0x27;
//
//         private bool IsDiskFull(System.Exception ex)
//         {
//             if (ex is IOException)
//             {
//                 int win32ErrorCode = System.Runtime.InteropServices.Marshal.GetHRForException(ex) & 0xFFFF;
//                 switch (win32ErrorCode)
//                 {
//                     case ERROR_DISK_FULL:
//                     case ERROR_HANDLE_DISK_FULL:
//                         return true;
//                 }
//             }
//
//             return false;
//         }
//
//         private void CatchException(System.Exception error, ErrorInfo.Type type = ErrorInfo.Type.UNKNOWN)
//         {
//             var errorInfo = new ErrorInfo
//             {
//                 type = type,
//                 message = error.Message
//             };
//             this.info.errorInfo = errorInfo;
//             info.onError?.Invoke(errorInfo);
//             www.Dispose();
//             www = null;
//         }
//
//         public override bool Update()
//         {
//             var loader = AssetBundleLoaderBase.Instance;
//             var isDone = www.isDone;
//             var isTimeOut = (Time.time - info.timeAtRequest) > loader.GetTimeoutTime();
//             bool hasErrorMessage = isDone && !string.IsNullOrEmpty(www.error);
//             if (hasErrorMessage || isTimeOut)
//             {
//                 ++info.retryNum;
//                 //如果超时了就要重新尝试
//                 if (info.retryNum < loader.GetMaxRetryNum())
//                 {
//                     www.Dispose();
//                     www = null;
//                     StartLoading();
//                 }
//                 else
//                 {
//                     //生成错误信息并传递给OnError
//                     ErrorInfo errorInfo = new ErrorInfo();
//                     if (hasErrorMessage)
//                     {
//                         //UnityWebRequest的错误大多是通信环境造成的
//                         errorInfo.type = ErrorInfo.Type.FAILED_WWW_REQUEST;
//                         errorInfo.message = www.error;
//                     }
//                     else
//                     {
//                         errorInfo.type = ErrorInfo.Type.TIME_OUT;
//                         errorInfo.message = AssetBundleLoaderBase.TimeOutErrorMessage;
//                     }
//
//                     this.info.errorInfo = errorInfo;
//                     if (info.onError != null)
//                     {
//                         Debug.LogError(
//                             $"url:{www.url} \n error:AssetBundleName :{info.assetBundleName},Message:{errorInfo.message}");
//                         Debug.LogErrorFormat("AssetBundleName:{0},Message:{1}", info.assetBundleName,
//                             errorInfo.message);
//                         info.onError(errorInfo);
//                     }
//
//                     www.Dispose();
//                     www = null;
//                     return true;
//                 }
//             }
//             else if (isDone)
//             {
//                 var cachePath = loader.CreateCachePath(info.assetBundleName);
//
//                 try
//                 {
//                     loader.DeleteCache(info.assetBundleName);
//                 }
//                 catch (System.Exception error)
//                 {
//                     CatchException(error, ErrorInfo.Type.FAILED_DELETE_CACHEFILE);
//                     return true;
//                 }
//
//                 try
//                 {
//                     // 需要目录的时候
//                     if (info.assetBundleName.Contains("/"))
//                     {
//                         var cacheDirectory = Path.GetDirectoryName(cachePath);
//                         if (!Directory.Exists(cacheDirectory))
//                         {
//                             if (cacheDirectory != null) Directory.CreateDirectory(cacheDirectory);
//                         }
//                     }
//                 }
//                 catch (System.Exception error)
//                 {
//                     CatchException(error,
//                         IsDiskFull(error) ? ErrorInfo.Type.FAILED_DISK_FULL : ErrorInfo.Type.FAILED_CREATE_DIRECTORY);
//                     return true;
//                 }
//
//                 try
//                 {
//                     // 写入存储空间
//                     var tempCachePath = cachePath + "__tmp";
//                     File.Delete(tempCachePath);
//                     using (Stream stream = File.OpenWrite(tempCachePath))
//                     {
//                         using (var writer = new BinaryWriter(stream))
//                         {
//                             if (string.IsNullOrEmpty(info.hashString))
//                             {
//                                 info.hashString = loader.GetAssetBundleHashString(info.assetBundleName);
//                             }
//
//                             var hashBytes = System.Text.Encoding.UTF8.GetBytes(info.hashString);
//                             writer.Write(hashBytes);
//                             writer.Write(www.downloadHandler.data);
//                         }
//                     }
//
//                     File.Delete(cachePath);
//                     File.Move(tempCachePath, cachePath);
//                 }
//                 catch (IOException error)
//                 {
//                     CatchException(error,
//                         IsDiskFull(error) ? ErrorInfo.Type.FAILED_DISK_FULL : ErrorInfo.Type.FAILED_WRITE_CACHEFILE);
//                     return true;
//                 }
//                 catch (IsolatedStorageException error)
//                 {
//                     CatchException(error, ErrorInfo.Type.FAILED_READ_CACHEFILE);
//                     return true;
//                 }
//                 catch (System.Exception error)
//                 {
//                     CatchException(error, ErrorInfo.Type.UNKNOWN);
//                     return true;
//                 }
//
//                 if (www.downloadHandler is DownloadHandlerAssetBundle downLoad)
//                 {
//                     info.assetBundle = downLoad.assetBundle;
//                 }
//
//                 www.Dispose();
//                 www = null;
//                 if (info.assetBundle != null) return true;
//                 {
//                     //考虑到下载数据有漏洞的情况，assetBundle为空的情况下删除
//                     try
//                     {
//                         loader.DeleteCache(info.assetBundleName);
//                         var errorInfo = new ErrorInfo
//                         {
//                             type = ErrorInfo.Type.DOWNLOAD_BUNDLE_INVALID,
//                             message = string.Format(AssetBundleLoaderBase.DownloadBundleInvalidErrorMessage,
//                                 info.assetBundleName, info.hashString),
//                             abError = $"{info.assetBundleName}下载失败"
//                         };
//                         this.info.errorInfo = errorInfo;
//                         info.onError?.Invoke(errorInfo);
//                     }
//                     catch (System.Exception error)
//                     {
//                         CatchException(error, ErrorInfo.Type.FAILED_DELETE_CACHEFILE);
//                         return true;
//                     }
//                 }
//                 return true;
//             }
//
//             return false;
//         }
//
//         public override IEnumerator AbortLoading()
//         {
//             if (www != null)
//             {
//                 www.Dispose();
//                 www = null;
//             }
//
//             if (info.onError == null) yield break;
//             var errorInfo = new ErrorInfo
//             {
//                 type = ErrorInfo.Type.ABORT_LOADING_REQUSET,
//                 message = AssetBundleLoaderBase.AbortLoadingRequestErrorMessage
//             };
//             info.onError(errorInfo);
//
//             yield break;
//         }
//     }
// }
using System.Collections;
using Framework.AssetBundles.Utilty;
using UnityEngine;
using ZJYFrameWork.AssetBundleLoader;

namespace ZJYFrameWork.assetBundleLoader
{
    internal class LoadFromLocalStorageRequest : LoadingMethod
    {
        /// <summary>
        /// 对正在请求的本地文件的文件信息
        /// </summary>
        private AssetBundleCreateRequest mLoadFromFileRequest;

        protected string localPath;
        /// <summary>
        /// 从本地加载，先从streamingAsset比较，如果版本匹配，则加载streamingAsset路径，否则加载其他路径
        /// </summary>
        public override void StartLoading()
        {
            localPath = AssetBundleLoaderBase.Instance.CreateCachePath(info.assetBundleName);
            mLoadFromFileRequest = AssetBundle.LoadFromFileAsync(localPath, 0, AssetBundleLoaderBase.Hash128ByteLength);
            info.timeAtRequest = Time.time;
        }

        public override bool Update()
        {
            if(!mLoadFromFileRequest.isDone)
            {
                return false;
            }
            info.assetBundle = mLoadFromFileRequest.assetBundle;
            mLoadFromFileRequest = null;
            if (info.assetBundle != null) return true;
            //考虑到写入错误等发生的情况assetbundle为空的情况删除
            try
            {
                AssetBundleLoaderBase.Instance.DeleteCache(info.assetBundleName);
                var errorInfo = new ErrorInfo
                {
                    type = ErrorInfo.Type.CACHEFILE_BUNDLE_INVALID,
                    message = string.Format(AssetBundleLoaderBase.CacheFileBundleInvalidErrorMessage, info.assetBundleName, info.hashString),
                    abError = $"{info.assetBundleName}加载失败"
                };
                this.info.errorInfo = errorInfo;
                info.onError?.Invoke(errorInfo);
            }
            catch (System.Exception error)
            {
                var errorInfo = new ErrorInfo
                {
                    type = ErrorInfo.Type.FAILED_DELETE_CACHEFILE,
                    message = error.Message
                };
                this.info.errorInfo = errorInfo;
                info.onError?.Invoke(errorInfo);
            }
            return true;
        }

        public override IEnumerator AbortLoading()
        {
            if(mLoadFromFileRequest != null)
            {
                yield return mLoadFromFileRequest;
                var assetBundle = mLoadFromFileRequest.assetBundle;
                if(assetBundle != null)
                {
                    assetBundle.Unload (true);
                    mLoadFromFileRequest = null;
                }
            }

            if (info.onError == null) yield break;
            var errorInfo = new ErrorInfo
            {
                type = ErrorInfo.Type.ABORT_LOADING_REQUSET,
                message = AssetBundleLoaderBase.AbortLoadingRequestErrorMessage
            };
            info.onError (errorInfo);
        }
    }
}
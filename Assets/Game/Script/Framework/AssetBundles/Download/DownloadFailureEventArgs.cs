﻿using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.Base.Model;
using ZJYFrameWork.Collection.Reference;

namespace ZJYFrameWork.AssetBundles.Download
{   /// <summary>
    /// 下载失败事件。
    /// </summary>
  public sealed class DownloadFailureEventArgs : GameFrameworkEventArgs
    {
        /// <summary>
        /// 初始化下载失败事件的新实例。
        /// </summary>
        public DownloadFailureEventArgs()
        {
            SerialId = null;
            DownloadPath = null;
            DownloadUri = null;
            ErrorMessage = null;
            UserData = null;
        }

        /// <summary>
        /// 获取下载任务的序列编号。
        /// </summary>
        public BundleInfo SerialId
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取下载后存放路径。
        /// </summary>
        public string DownloadPath
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取下载地址。
        /// </summary>
        public string DownloadUri
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取错误信息。
        /// </summary>
        public string ErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取用户自定义数据。
        /// </summary>
        public object UserData
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建下载失败事件。
        /// </summary>
        /// <param name="serialId">下载任务的序列编号。</param>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">下载地址。</param>
        /// <param name="errorMessage">错误信息。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>创建的下载失败事件。</returns>
        public static DownloadFailureEventArgs Create(BundleInfo serialId, string downloadPath, string downloadUri, string errorMessage, object userData)
        {
            DownloadFailureEventArgs downloadFailureEventArgs = ReferenceCache.Acquire<DownloadFailureEventArgs>();
            downloadFailureEventArgs.SerialId = serialId;
            downloadFailureEventArgs.DownloadPath = downloadPath;
            downloadFailureEventArgs.DownloadUri = downloadUri;
            downloadFailureEventArgs.ErrorMessage = errorMessage;
            downloadFailureEventArgs.UserData = userData;
            return downloadFailureEventArgs;
        }

        /// <summary>
        /// 清理下载失败事件。
        /// </summary>
        public override void Clear()
        {
            SerialId = null;
            DownloadPath = null;
            DownloadUri = null;
            ErrorMessage = null;
            UserData = null;
        }
    }
}
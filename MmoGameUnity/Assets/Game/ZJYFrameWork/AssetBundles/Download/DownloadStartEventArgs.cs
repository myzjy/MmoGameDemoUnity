using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.Base.Model;
using ZJYFrameWork.Collection.Reference;

namespace ZJYFrameWork.AssetBundles.DownLoader
{
    public sealed class DownloadStartEventArgs: GameFrameworkEventArgs
    {
        /// <summary>
        /// 初始化下载开始事件的新实例。
        /// </summary>
        public DownloadStartEventArgs()
        {
            SerialBundleInfo = null;
            DownloadPath = null;
            DownloadUri = null;
            CurrentLength = 0;
            UserData = null;
        }
        /// <summary>
        /// 获取下载任务的序列编号。
        /// </summary>
        public BundleInfo SerialBundleInfo
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
        /// 获取用户自定义数据。
        /// </summary>
        public object UserData
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取当前大小。
        /// </summary>
        public long CurrentLength
        {
            get;
            private set;
        }
        /// <summary>
        /// 创建下载开始事件。
        /// </summary>
        /// <param name="serialId">下载任务的序列编号。</param>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">下载地址。</param>
        /// <param name="currentLength">当前大小。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>创建的下载开始事件。</returns>
        public static DownloadStartEventArgs Create(BundleInfo serialId, string downloadPath, string downloadUri, long currentLength, object userData)
        {
            DownloadStartEventArgs downloadStartEventArgs = ReferenceCache.Acquire<DownloadStartEventArgs>();
            downloadStartEventArgs.SerialBundleInfo = serialId;
            downloadStartEventArgs.DownloadPath = downloadPath;
            downloadStartEventArgs.DownloadUri = downloadUri;
            downloadStartEventArgs.CurrentLength = currentLength;
            downloadStartEventArgs.UserData = userData;
            return downloadStartEventArgs;
        }

        /// <summary>
        /// 清理下载开始事件。
        /// </summary>
        public override void Clear()
        {
            SerialBundleInfo = null;
            DownloadPath = null;
            DownloadUri = null;
            CurrentLength = 0L;
            UserData = null;
        }
    }
}
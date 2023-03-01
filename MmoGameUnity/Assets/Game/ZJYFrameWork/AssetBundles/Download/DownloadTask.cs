using System;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.AssetBundles.IDownLoaderBundle;
using ZJYFrameWork.Collection.Reference;

namespace ZJYFrameWork.AssetBundles.DownLoader
{
    /// <summary>
    /// 下载任务
    /// </summary>
    public sealed class DownloadTask : TaskBase
    {
        private BundleInfo mBundleInfo = default;

        /// <summary>
        /// 下载任务状态
        /// </summary>
        private DownloadTaskStatus m_Status;

        /// <summary>
        /// 下载的路径
        /// </summary>
        private string m_DownloadPath;

        /// <summary>
        /// 下载路径url
        /// </summary>
        private Uri m_DownloadUri;

        /// <summary>
        /// 大小
        /// </summary>
        private int m_FlushSize;

        /// <summary>
        /// 超时时间
        /// </summary>
        private float m_Timeout;

        /// <summary>
        /// 自定义基础信息
        /// </summary>
        private object m_UserData;

        /// <summary>
        /// 初始化下载任务的新实例。
        /// </summary>
        public DownloadTask()
        {
            m_Status = DownloadTaskStatus.Todo;
            mBundleInfo = default;
            m_DownloadPath = null;
            m_DownloadUri = null;
            m_FlushSize = 0;
            m_Timeout = 0f;
            m_UserData = null;
        }

        public BundleInfo AssetBundleInfo
        {
            get { return mBundleInfo; }
        }

        /// <summary>
        /// 获取或设置下载任务的状态。
        /// </summary>
        public DownloadTaskStatus Status
        {
            get => m_Status;
            set => m_Status = value;
        }

        /// <summary>
        /// 获取下载后存放路径。
        /// </summary>
        public string DownloadPath => m_DownloadPath;

        /// <summary>
        /// 获取原始下载地址。
        /// </summary>
        public Uri DownloadUri
        {
            get { return m_DownloadUri; }
        }

        /// <summary>
        /// 获取将缓冲区写入磁盘的临界大小。
        /// </summary>
        public int FlushSize
        {
            get { return m_FlushSize; }
        }

        /// <summary>
        /// 获取下载超时时长，以秒为单位。
        /// </summary>
        public float Timeout
        {
            get { return m_Timeout; }
        }

        /// <summary>
        /// 获取用户自定义数据。
        /// </summary>
        public object UserData
        {
            get { return m_UserData; }
        }

        /// <summary>
        /// 获取下载任务的描述。
        /// </summary>
        public override string Description
        {
            get { return m_DownloadPath; }
        }

        public override IDownloader Downloader { get; set; }

        /// <summary>
        /// 创建下载任务。
        /// </summary>
        /// <param name="bundleInfo">需要下载</param>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="mUrl">原始下载地址。</param>
        /// <param name="priority">下载任务的优先级。</param>
        /// <param name="flushSize">将缓冲区写入磁盘的临界大小。</param>
        /// <param name="timeout">下载超时时长，以秒为单位。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="downloader"></param>
        /// <returns>创建的下载任务。</returns>
        public static DownloadTask Create(BundleInfo bundleInfo, Uri mUrl, string downloadPath, int priority,
            int flushSize,
            float timeout, object userData, IDownloader downloader)
        {
            DownloadTask downloadTask = ReferenceCache.Acquire<DownloadTask>();
            downloadTask.Initialize(bundleInfo, priority);
            downloadTask.m_DownloadPath = downloadPath;
            downloadTask.m_DownloadUri = mUrl;
            downloadTask.m_FlushSize = flushSize;
            downloadTask.m_Timeout = timeout;
            downloadTask.m_UserData = userData;
            downloadTask.Downloader = downloader;
            return downloadTask;
        }

        /// <summary>
        /// 清理下载任务。
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            m_Status = DownloadTaskStatus.Todo;
            mBundleInfo = default;
            m_DownloadPath = null;
            m_DownloadUri = null;
            m_FlushSize = 0;
            m_Timeout = 0f;
            m_UserData = null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.AssetBundles.DownLoader;
using ZJYFrameWork.AssetBundles.IAssetBundlesManagerInterface;
using ZJYFrameWork.AssetBundles.IAssetBundlesUpdaterInterface;
using ZJYFrameWork.AssetBundles.IDownLoaderBundle;
using ZJYFrameWork.AssetBundles.IDownLoadManagerInterface;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.AssetBundles.AssetBundlesUpdater
{
    /// <summary>
    /// 更新器
    /// </summary>
    [Bean]
    public sealed class AssetBundleUpdater : IAssetBundleUpdater
    {
        [Autowired] private readonly IAssetBundleManager resourceManager;


        // [Autowired] private IFileDownloader FileDownloader;
        /// <summary>
        /// 文件下载器
        /// </summary>
        [Autowired] private IDownloadManager downloadManager;

        /// <summary>
        /// 下载器
        /// </summary>
        [Autowired] private IDownloader Downloader;

        private readonly List<BundleInfo> updateWaitingInfos = new List<BundleInfo>();

        private readonly Dictionary<AssetPathInfo, BundleInfo> assetBundleInfosUpdate =
            new Dictionary<AssetPathInfo, BundleInfo>();

        public Action<BundleInfo, string, string, int> ResourceUpdateStart { get; set; }
        public Action<BundleInfo, string, string, int, long> ResourceUpdateChanged { get; set; }
        public Action<BundleInfo, string, string, int, long> ResourceUpdateSuccess { get; set; }
        public Action<BundleInfo, string, int, int, string> ResourceUpdateFailure { get; set; }

        [AfterPostConstruct]
        private void Init()
        {
            downloadManager.DownloadStart += OnDownloadStart;
            downloadManager.DownloadUpdate += OnDownloadUpdate;
            downloadManager.DownloadSuccess += OnDownloadSuccess;
            downloadManager.DownloadFailure += OnDownloadFailure;
        }

        private void OnDownloadStart(object sender, DownloadStartEventArgs e)
        {
            BundleInfo updateInfo = e.UserData as BundleInfo;
            if (updateInfo == null)
            {
                return;
            }

            if (ResourceUpdateStart != null)
            {
                ResourceUpdateStart(updateInfo, e.DownloadPath, e.DownloadUri, (int)e.CurrentLength);
            }
        }

        private void OnDownloadUpdate(object sender, DownloadUpdateEventArgs e)
        {
            BundleInfo updateInfo = e.UserData as BundleInfo;
            if (updateInfo == null)
            {
                return;
            }

            if (downloadManager == null)
            {
                throw new Exception("You must set download manager first.");
            }

            if (ResourceUpdateChanged != null)
            {
                ResourceUpdateChanged(updateInfo, e.DownloadPath, e.DownloadUri, (int)e.CurrentLength,
                    updateInfo.FileSize);
            }
        }

        private void OnDownloadSuccess(object sender, DownloadSuccessEventArgs e)
        {
            BundleInfo updateInfo = e.UserData as BundleInfo;
            if (updateInfo == null)
            {
                return;
            }

            if (ResourceUpdateSuccess != null)
            {
                ResourceUpdateSuccess(updateInfo, e.DownloadPath, e.DownloadUri, (int)e.CurrentLength,
                    updateInfo.FileSize);
            }
        }

        private void OnDownloadFailure(object sender, DownloadFailureEventArgs e)
        {
            BundleInfo updateInfo = e.UserData as BundleInfo;
            if (updateInfo == null)
            {
                return;
            }

            if (File.Exists(e.DownloadPath))
            {
                File.Delete(e.DownloadPath);
            }

            if (ResourceUpdateFailure != null)
            {
                ResourceUpdateFailure(updateInfo, e.DownloadUri, 1, 1, e.ErrorMessage);
            }
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}
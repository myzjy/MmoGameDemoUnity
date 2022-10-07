using System;
using System.Collections.Generic;
using ZJYFrameWork.AssetBundles.Bundle;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.AssetBundles.Download;
using ZJYFrameWork.Net.Http;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.AssetBundles
{
    /// <summary>
    /// 更新器
    /// </summary>
    public sealed class AssetBundleUpdater
    {
        [Autowired] private readonly AssetBundleManager resourceManager;

        /// <summary>
        /// 文件下载器
        /// </summary>
        [Autowired] private IFileDownloader FileDownloader;

        [Autowired] private IDownloadManager downloadManager;

        /// <summary>
        /// 下载器
        /// </summary>
        [Autowired] private IDownloader Downloader;

        private readonly List<BundleInfo> updateWaitingInfos = new List<BundleInfo>();

        private readonly Dictionary<AssetPathInfo, BundleInfo> assetBundleInfosUpdate =
            new Dictionary<AssetPathInfo, BundleInfo>();

        public Action<AssetPathInfo, string, string, int> ResourceUpdateStart;

        [AfterPostConstruct]
        private void Init()
        {
            downloadManager.DownloadStart += OnDownloadStart;
            // downloadManager.DownloadUpdate += OnDownloadUpdate;
            // downloadManager.DownloadSuccess += OnDownloadSuccess;
            // downloadManager.DownloadFailure += OnDownloadFailure;
        }

        private void OnDownloadStart(object sender, DownloadStartEventArgs e)
        {
            AssetPathInfo updateInfo = e.UserData as AssetPathInfo;
            if (updateInfo == null)
            {
                return;
            }

            if (ResourceUpdateStart != null)
            {
                ResourceUpdateStart(updateInfo, e.DownloadPath, e.DownloadUri, (int)e.CurrentLength);
            }
        }
 
    }
}
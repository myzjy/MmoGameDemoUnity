using ZJYFrameWork.AssetBundles.Bundle;
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

        /// <summary>
        /// 下载器
        /// </summary>
        [Autowired] private IDownloader Downloader;

        [AfterPostConstruct]
        private void Init()
        {
        }
    }
}
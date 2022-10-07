using ZJYFrameWork.AssetBundles.Bundles;

namespace ZJYFrameWork.AssetBundles
{
    /// <summary>
    /// 下载器管理类接口
    /// </summary>
    public interface IDownloadManager
    {
        /// <summary>
        /// 获取下载代理总数量。
        /// </summary>
        int TotalAgentCount { get; }

        /// <summary>
        /// 获取可用下载代理数量。
        /// </summary>
        int FreeAgentCount { get; }

        /// <summary>
        /// 获取工作中下载代理数量。
        /// </summary>
        int WorkingAgentCount { get; }

        /// <summary>
        /// 获取等待下载任务数量。
        /// </summary>
        int WaitingTaskCount { get; }

        /// <summary>
        /// 获取或设置将缓冲区写入磁盘的临界大小。
        /// </summary>
        int FlushSize { get; set; }

        /// <summary>
        /// 获取或设置下载超时时长，以秒为单位。
        /// </summary>
        float Timeout { get; set; }

        /// <summary>
        /// 获取当前下载速度。
        /// </summary>
        float CurrentSpeed { get; }

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="assetBundle">assetBundle 路径</param>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        void AddDownload(string assetBundle, string downloadPath, string downloadUri);

        /// <summary>
        /// 下载任务添加
        /// </summary>
        /// <param name="bundleInfo"></param>
        void AddDownload(BundleInfo bundleInfo);

        /// <summary>
        /// 移除下载任务。
        /// </summary>
        /// <param name="serialBundleInfo">要移除下载任务的bundleInfo。</param>
        /// <returns>是否移除下载任务成功。</returns>
        bool RemoveDownload(BundleInfo serialBundleInfo);

        /// <summary>
        /// 移除所有下载任务。
        /// </summary>
        void RemoveAllDownloads();
    }
}
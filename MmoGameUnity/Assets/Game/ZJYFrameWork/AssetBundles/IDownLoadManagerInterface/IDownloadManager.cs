using System;
using System.Collections;
using ZJYFrameWork.AssetBundles.Bundle;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.AssetBundles.DownLoader;
using ZJYFrameWork.Asynchronous;

namespace ZJYFrameWork.AssetBundles.IDownLoadManagerInterface
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
        /// 下载开始事件。
        /// </summary>
        event EventHandler<DownloadStartEventArgs> DownloadStart;

        /// <summary>
        /// 下载更新事件。
        /// </summary>
        event EventHandler<DownloadUpdateEventArgs> DownloadUpdate;

        /// <summary>
        /// 下载成功事件。
        /// </summary>
        event EventHandler<DownloadSuccessEventArgs> DownloadSuccess;

        /// <summary>
        /// 下载失败事件。
        /// </summary>
        event EventHandler<DownloadFailureEventArgs> DownloadFailure;

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

        IEnumerator StartFirstDownload();

        /// <summary>
        /// 下载开始事件。
        /// </summary>
        EventHandler<DownloadStartEventArgs> GetDownloadStart { get; }

        /// <summary>
        /// 下载更新事件。
        /// </summary>
        EventHandler<DownloadUpdateEventArgs> GetDownloadUpdate { get; }

        /// <summary>
        /// 下载成功事件。
        /// </summary>
        EventHandler<DownloadSuccessEventArgs> GetDownloadSuccess { get; }

        /// <summary>
        /// 下载失败事件。
        /// </summary>
        EventHandler<DownloadFailureEventArgs> GetDownloadFailure { get; }

        /// <summary>
        /// 当下在进度任务
        /// </summary>
        IProgressResult<Progress, bool> nowProgressResult { get; }
    }
}
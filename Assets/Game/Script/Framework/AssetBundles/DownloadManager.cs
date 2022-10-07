using System;
using Framework.AssetBundles.Config;
using UnityEngine;
using ZJYFrameWork.AssetBundles.Bundle;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.AssetBundles.Download;
using ZJYFrameWork.Base.Model;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.AssetBundles
{
    [Bean]
    public class DownloadManager : AbstractManager, IDownloadManager
    {
        public const int DefaultPriority = 0;

        private const int OneMegaBytes = 1024 * 1024;
        private readonly TaskPool<DownloadTask> taskPool = new TaskPool<DownloadTask>();

        /// <summary>
        /// 下载接口
        /// </summary>
        [Autowired] private IDownloader Downloader;

        public int TotalAgentCount
        {
            get => progress.TotalCount;
        }

        public int FreeAgentCount
        {
            get => progress.CompletedCount;
        }

        public int WorkingAgentCount
        {
            get => progress.CompletedCount;
        }

        public int WaitingTaskCount
        {
            get => progress.TotalCount - progress.CompletedCount;
        }

        private int flushSize = OneMegaBytes;

        /// <summary>
        /// 获取或设置将缓冲区写入磁盘的临界大小。
        /// </summary>
        public int FlushSize
        {
            get { return flushSize; }
            set { flushSize = value; }
        }

        private float timeout = 30f;

        /// <summary>
        /// 获取或设置下载超时时长，以秒为单位。
        /// </summary>
        public float Timeout
        {
            get => timeout;
            set => timeout = value;
        }

        /// <summary>
        /// 获取当前下载速度。
        /// </summary>
        public float CurrentSpeed
        {
            get { return progress.GetSpeed(); }
        }
        private EventHandler<DownloadStartEventArgs> _mDownloadStartEventHandler;
        private EventHandler<DownloadUpdateEventArgs> _mDownloadUpdateEventHandler;
        private EventHandler<DownloadSuccessEventArgs> _mDownloadSuccessEventHandler;
        private EventHandler<DownloadFailureEventArgs> _mDownloadFailureEventHandler;
        /// <summary>
        /// 下载开始事件。
        /// </summary>
        public event EventHandler<DownloadStartEventArgs> DownloadStart
        {
            add => _mDownloadStartEventHandler += value;
            remove => _mDownloadStartEventHandler -= value;
        }

        /// <summary>
        /// 下载更新事件。
        /// </summary>
        public event EventHandler<DownloadUpdateEventArgs> DownloadUpdate
        {
            add => _mDownloadUpdateEventHandler += value;
            remove => _mDownloadUpdateEventHandler -= value;
        }

        /// <summary>
        /// 下载成功事件。
        /// </summary>
        public event EventHandler<DownloadSuccessEventArgs> DownloadSuccess
        {
            add => _mDownloadSuccessEventHandler += value;
            remove => _mDownloadSuccessEventHandler -= value;
        }

        /// <summary>
        /// 下载失败事件。
        /// </summary>
        public event EventHandler<DownloadFailureEventArgs> DownloadFailure
        {
            add => _mDownloadFailureEventHandler += value;
            remove => _mDownloadFailureEventHandler -= value;
        }

        /// <summary>
        /// 进度条
        /// </summary>
        private Progress progress = new Progress();

        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        public override int Priority
        {
            get { return 80; }
        }

        /// <summary>
        /// 下载管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            taskPool.Update(elapseSeconds, realElapseSeconds);
        }

        public override void Shutdown()
        {
            taskPool.Shutdown();
        }


        public void AddDownload(string assetBundle, string downloadPath, string downloadUri)
        {
            throw new System.NotImplementedException();
        }

        public void AddDownload(BundleInfo bundleInfo)
        {
            if (bundleInfo == null)
            {
                throw new Exception("bundleInfo  is invalid.");
            }

            DownloadTask downloadTask = DownloadTask.Create(bundleInfo, downloadPath: bundleInfo.FullName,
                DefaultPriority, (int)bundleInfo.FileSize, Timeout, null);
            taskPool.AddTask(downloadTask);
        }

        public bool RemoveDownload(BundleInfo serialBundleInfo)
        {
            return taskPool.RemoveTask(serialBundleInfo);
        }

        public void RemoveAllDownloads()
        {
            taskPool.RemoveAllTasks();
        }

        [PostConstruct]
        public void Init()
        {
            // if (AssetBundleConfig.IsEditorMode)
            // {
            //     Debug.Log("编辑器模式");
// #if UNITY_EDITOR
            Uri baseUri = new Uri(BundleUtil.GetReadOnlyDirectory());
            Debug.Log($"{baseUri.AbsoluteUri}");
// #endif
            Downloader.BaseUri = baseUri;
            Downloader.MaxTaskCount = SystemInfo.processorCount * 2;
            // }
        }
    }
}
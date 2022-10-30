using System;
using System.Collections;
using System.Collections.Generic;
using Framework.AssetBundles.Config;
using UnityEngine;
using ZJYFrameWork.AssetBundles.Bundle;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.AssetBundles.Download;
using ZJYFrameWork.Asynchronous;
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

        [Autowired] private AssetBundleManager _assetBundleManager;

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
            if (_nowProgressResult != null)
            {
                if (_nowProgressResult.IsDone)
                {
                    _nowProgressResult = null;
                }
            }
        }

        public override void Shutdown()
        {
            taskPool.Shutdown();
        }


        public void AddDownload(string assetBundle, string downloadPath, string downloadUri)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 在没有第一次下载所有资源，遗漏的资源，进行添加任务池
        /// </summary>
        /// <param name="bundleInfo"></param>
        /// <exception cref="Exception"></exception>
        public void AddDownload(BundleInfo bundleInfo)
        {
            if (bundleInfo == null)
            {
                throw new Exception("bundleInfo  is invalid.");
            }

            Uri url = Downloader.GetAbsoluteUrl(bundleInfo.Filename);
            var fullname = $"{BundleUtil.GetStorableDirectory()}{bundleInfo.Filename}";
            DownloadTask downloadTask = DownloadTask.Create(bundleInfo, url,
                downloadPath: fullname,
                DefaultPriority, (int)bundleInfo.FileSize, Timeout, null, Downloader);
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

        public IEnumerable StartIDownAssetBundle()
        {
            throw new NotImplementedException();
        }

        public EventHandler<DownloadStartEventArgs> GetDownloadStart
        {
            get => _mDownloadStartEventHandler;
        }

        public EventHandler<DownloadUpdateEventArgs> GetDownloadUpdate
        {
            get => _mDownloadUpdateEventHandler;
        }

        public EventHandler<DownloadSuccessEventArgs> GetDownloadSuccess
        {
            get => _mDownloadSuccessEventHandler;
        }

        public EventHandler<DownloadFailureEventArgs> GetDownloadFailure
        {
            get => _mDownloadFailureEventHandler;
        }

        public IProgressResult<Progress, bool> nowProgressResult
        {
            get => _nowProgressResult;
        }

        private IProgressResult<Progress, bool> _nowProgressResult { get; set; }

        [PostConstruct]
        public void Init()
        {
            Uri baseUri = new Uri(BundleUtil.GetReadOnlyDirectory());
            Debug.Log($"{baseUri.AbsoluteUri}");
            Downloader.BaseUri = baseUri;
            Downloader.MaxTaskCount = SystemInfo.processorCount * 2;
            // }
        }

        /// <summary>
        /// 第一次下载，没有manifest的时候
        /// </summary>
        public IEnumerator StartFirstDownload()
        {
            IProgressResult<Progress, BundleManifest> manifestResult =
                this.Downloader.DownloadManifest(AssetBundleConfig.ManifestFilename);
            // DownloadStart()
            manifestResult.Callbackable().OnProgressCallback(res =>
            {
                Debug.Log("下载[{}]文件，当前已下载大小：{}KB,文件所需总下载：{}", AssetBundleConfig.ManifestFilename,
                    res.GetCompletedSize(UNIT.KB), res.GetTotalSize(UNIT.KB));
            });
            yield return manifestResult.WaitForDone();

            manifestResult.Callbackable().OnCallback(p =>
            {
                if (p.Result == null)
                {
#if DEVELOP_BUILD
                    Debug.Log("下载manifest出错");
#endif
                    return;
                }

                _assetBundleManager.SetBundleManifest(p.Result);
            });
            yield return manifestResult.WaitForDone();
            if (_assetBundleManager.BundleManifest == null)
            {
#if DEVELOP_BUILD
                Debug.Log($"BundleManifest文件：[{_assetBundleManager.BundleManifest}],下载错误，没有从资源服务器下载到文件，请检查");
#endif
            }

            yield return GetDownBundleLists(_assetBundleManager.BundleManifest);
        }

        /// <summary>
        /// 最新的所下载资源
        /// </summary>
        private List<BundleInfo> newDownBundleInfoList = new List<BundleInfo>();

        /// <summary>
        /// 获取manifest内部需要下载的
        /// </summary>
        /// <param name="manifest"></param>
        public IEnumerator GetDownBundleLists(BundleManifest manifest)
        {
            IProgressResult<float, List<BundleInfo>> bundlesResult = this.Downloader.GetDownloadList(manifest);

            yield return bundlesResult.WaitForDone();
            bundlesResult.Callbackable().OnCallback(p =>
            {
                if (p.Result == null)
                {
                    Debug.Log("bundle信息出错");
                    return;
                }

                var list = p.Result;
                if (list.Count <= 0)
                {
                    UnityEngine.Debug.Log("没有需要下载资源");
                    return;
                }

                newDownBundleInfoList = new List<BundleInfo>();
                newDownBundleInfoList.AddRange(list);
                // _assetBundleManager.SetBundleManifest(p.Result);
            });
            yield return bundlesResult.WaitForDone();
            DownloadAssetBundles(newDownBundleInfoList);
        }

        private void DownloadAssetBundles(List<BundleInfo> bundleInfos)
        {
            Downloader.SetDownManager(this);
            _nowProgressResult = Downloader.DownloadBundles(bundleInfos);

            _nowProgressResult.Callbackable().OnProgressCallback(res => { });
            _nowProgressResult.Callbackable().OnCallback(res => { });
            _nowProgressResult.WaitForDone();
        }
    }
}
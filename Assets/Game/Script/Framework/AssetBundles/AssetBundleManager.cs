using System;
using System.Collections;
using Framework.AssetBundles.Config;
using Framework.AssetBundles.Utilty;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.AssetBundles.Model.Callback;
using ZJYFrameWork.Base.Model;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.AssetBundles
{
    [Bean]
    public class AssetBundleManager : AbstractManager, IAssetBundleManager
    {
        /// <summary>
        /// 资源读取接口 需要在下载接口走完之后，查看有没有需要下载
        /// </summary>
        [Autowired] private IResources Resources;

        public string BundleRoot => AssetBundleConfig.BundleRoot;

        public string StorableDirectory
        {
            get => BundleUtil.GetStorableDirectory();
        }

        public string ReadOnlyDirectory
        {
            get => BundleUtil.GetReadOnlyDirectory();
        }

        public string TemporaryCacheDirectory
        {
            get => BundleUtil.GetTemporaryCacheDirectory();
        }


        [Autowired] public AssetBundleUpdater resourceUpdater;
        public string UpdatePrefixUri { get; set; }
        public string ApplicableGameVersion { get; }
        public float AssetAutoReleaseInterval { get; set; }
        public int AssetCapacity { get; set; }
        public float AssetExpireTime { get; set; }
        public float ResourceAutoReleaseInterval { get; set; }
        public int ResourceCapacity { get; set; }
        public float ResourceExpireTime { get; set; }
        public int ResourcePriority { get; set; }

        /// <summary>
        /// BundleManifest 读取器
        /// </summary>
        [Autowired] private IBundleManifestLoader BundleManifestLoader;

        /// <summary>
        /// 路径保存器
        /// </summary>
        [Autowired] private IPathInfoParser _pathInfoParser;

        //这个接口被多个类继承，所以不能在这里定义
        // /// <summary>
        // /// manifest升级器
        // /// </summary>
        // [Autowired] private IManifestUpdatable ManifestUpdatable;

        /// <summary>
        /// 构建管理器
        /// </summary>
        [Autowired] private ILoaderBuilder _loaderBuilder;

        /// <summary>
        /// 管理manifest
        /// </summary>
        public BundleManifest BundleManifest { get; set; }

        /// <summary>
        /// 下载接口
        /// </summary>
        // [Autowired] private IDownloader Downloader;
        [Autowired] private IBundleManager _bundleManager;

        [Autowired] private IDownloadManager DownloadManager;

        public void SetAssetBundle()
        {
            BundleManifestLoader = SpringContext.GetBean<BundleManifestLoader>();
            BundleManifest =
                BundleManifestLoader.Load(BundleUtil.GetReadOnlyDirectory() + AssetBundleConfig.ManifestFilename);

            //修正bundleManifest
            _pathInfoParser.BundleManifest = BundleManifest;
            _loaderBuilder.SetLoaderBuilder(new Uri(BundleUtil.GetReadOnlyDirectory()));
#if UNITY_EDITOR
            if (!AssetBundleConfig.IsEditorMode)
            {
                _bundleManager.SetManifestAndLoadBuilder(BundleManifest, _loaderBuilder);
            }
#else
  _bundleManager.SetManifestAndLoadBuilder(BundleManifest, _loaderBuilder);
#endif

            //设置bundle 当有更新的时候就需要从新设置
            Resources.SetIPathAndBundleResource(_pathInfoParser, _bundleManager);

            resourceUpdater.ResourceUpdateStart += OnUpdaterResourceUpdateStart;
            resourceUpdater.ResourceUpdateChanged += OnUpdaterResourceUpdateChanged;
            resourceUpdater.ResourceUpdateSuccess += OnUpdaterResourceUpdateSuccess;
            resourceUpdater.ResourceUpdateFailure += OnUpdaterResourceUpdateFailure;
            // resourceUpdater.ResourceUpdateComplete += OnUpdaterResourceUpdateComplete;
        }

        public void LoadAssetBundle(string assetBundle, LoadAssetCallbacks loadAssetCallbacks)
        {
            var obj = Resources.LoadAsset(assetBundle);
            if (obj == null)
            {
                // loadAssetCallbacks.LoadAssetFailureCallback();
            }
            else
            {
            }
        }

        public void LoadAsset(string assetBundle, LoadAssetCallbacks loadAssetCallbacks)
        {
            throw new NotImplementedException();
        }

        public void SetBundleManifest(BundleManifest bundleManifest)
        {
            BundleManifest = bundleManifest;
            _pathInfoParser.BundleManifest = BundleManifest;
            _bundleManager.SetManifestAndLoadBuilder(BundleManifest, _loaderBuilder);
            //设置bundle 当有更新的时候就需要从新设置
            Resources.SetIPathAndBundleResource(_pathInfoParser, _bundleManager);
        }

        public IEnumerable StartIDownAssetBundle()
        {
            throw new NotImplementedException();
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            //不需要进行轮询，下载，更新走另外一套
            return;
        }

        public override void Shutdown()
        {
        }

        private void OnUpdaterResourceUpdateStart(BundleInfo resourceName, string downloadPath, string downloadUri,
            int currentLength)
        {
            // EventBus.SyncSubmit(ResourceUpdateStartEvent.ValueOf(resourceName.FullName, downloadPath, downloadUri, currentLength, zipLength, retryCount));
        }

        private void OnUpdaterResourceUpdateChanged(BundleInfo resourceName, string downloadPath, string downloadUri,
            int currentLength, long zipLength)
        {
            // EventBus.SyncSubmit(ResourceUpdateChangedEvent.ValueOf(resourceName.FullName, downloadPath, downloadUri, currentLength, zipLength));
        }

        private void OnUpdaterResourceUpdateSuccess(BundleInfo resourceName, string downloadPath, string downloadUri,
            int length, long zipLength)
        {
            // EventBus.SyncSubmit(ResourceUpdateSuccessEvent.ValueOf(resourceName.FullName, downloadPath, downloadUri, length, zipLength));
        }

        private void OnUpdaterResourceUpdateFailure(BundleInfo resourceName, string downloadUri, int retryCount,
            int totalRetryCount, string errorMessage)
        {
            // EventBus.SyncSubmit(ResourceUpdateFailureEvent.ValueOf(resourceName.FullName, downloadUri, retryCount, totalRetryCount, errorMessage));
        }
    }
}
using System;
using Framework.AssetBundles.Config;
using UnityEngine;
using ZJYFrameWork.AssetBundles.Bundle;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.Base;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.AssetBundles
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Game/FrameWork/Asset Bundles Component")]
    public class AssetBundlesComponent : SpringComponent
    {
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
        private BundleManifest BundleManifest = null;

        /// <summary>
        /// 下载接口
        /// </summary>
        // [Autowired] private IDownloader Downloader;

        /// <summary>
        /// 资源读取接口 需要在下载接口走完之后，查看有没有需要下载
        /// </summary>
        [Autowired] private IResources Resources;

        [Autowired] private IBundleManager _bundleManager;
        [Autowired] private IDownloadManager DownloadManager;

        protected override void OnAwake()
        {
            base.OnAwake();
        }

        [PostConstruct]
        private void Init()
        {
//             if (AssetBundleConfig.IsEditorMode)
//             {
//                 Debug.Log("编辑器模式");
// #if UNITY_EDITOR
//                 Uri baseUri = new Uri(BundleUtil.GetReadOnlyDirectory());
//                 Debug.Log($"{baseUri.AbsoluteUri}");
// #endif
//                 Downloader.BaseUri = baseUri;
//                 Downloader.MaxTaskCount = SystemInfo.processorCount * 2;
//                 // _pathInfoParser = SpringContext.GetBean<SimulationAutoMappingPathInfoParser>();
//                 // _bundleManager = SpringContext.GetBean<SimulationBundleManager>();
//                 // Resources = SpringContext.GetBean<SimulationResources>();
//                 // Resources.SetIPathAndBundleResource(_pathInfoParser, _bundleManager);
//             }
        }
    }
}
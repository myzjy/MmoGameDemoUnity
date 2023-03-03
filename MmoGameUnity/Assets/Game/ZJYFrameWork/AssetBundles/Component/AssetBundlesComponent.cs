using System;
using UnityEngine;
using ZJYFrameWork.AssetBundles.IAssetBundlesManagerInterface;
using ZJYFrameWork.AssetBundles.IDownLoadManagerInterface;
using ZJYFrameWork.Base.Component;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.AssetBundles.Component
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Game/FrameWork/Asset Bundles Component")]
    public class AssetBundlesComponent : SpringComponent
    {
        [Autowired] private IAssetBundleManager AssetBundleManager;
        // [Autowired] private IDownloadManager DownloadManager;
        protected override void OnAwake()
        {
            base.OnAwake();
        }

        [PostConstruct]
        private void Init()
        {
            //设置bundle 当有更新的时候就需要从新设置
            AssetBundleManager.SetAssetBundle();
            // Resources.SetIPathAndBundleResource(_pathInfoParser,_bundleManager);
        }

        public void StartDownAssetBundle()
        {
            
        }
#if UNITY_EDITOR
        
#endif
    }
}
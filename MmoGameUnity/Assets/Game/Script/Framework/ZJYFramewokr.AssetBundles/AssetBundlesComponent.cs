using System;
using Framework.AssetBundles.Config;
using UnityEngine;
using ZJYFrameWork.AssetBundles.Bundle;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.Base;
using ZJYFrameWork.Base.Component;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.AssetBundles
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Game/FrameWork/Asset Bundles Component")]
    public class AssetBundlesComponent : SpringComponent
    {
        [Autowired] private IAssetBundleManager AssetBundleManager;
        [Autowired] private IDownloadManager DownloadManager;
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
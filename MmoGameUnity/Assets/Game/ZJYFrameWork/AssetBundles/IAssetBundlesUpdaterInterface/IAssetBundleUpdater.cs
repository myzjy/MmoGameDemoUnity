using System;
using ZJYFrameWork.AssetBundles.Bundles;

namespace ZJYFrameWork.AssetBundles.IAssetBundlesUpdaterInterface
{
    public interface IAssetBundleUpdater
    {
        Action<BundleInfo, string, string, int> ResourceUpdateStart { get; set; }
        Action<BundleInfo, string, string, int, long> ResourceUpdateChanged { get; set; }
        Action<BundleInfo, string, string, int, long> ResourceUpdateSuccess { get; set; }
        Action<BundleInfo, string, int, int, string> ResourceUpdateFailure { get; set; }
    }
}
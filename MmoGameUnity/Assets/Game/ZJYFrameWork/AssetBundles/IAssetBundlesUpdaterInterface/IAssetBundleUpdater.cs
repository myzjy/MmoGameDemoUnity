using System;
using ZJYFrameWork.AssetBundles.Bundles;

namespace ZJYFrameWork.AssetBundles.IAssetBundlesUpdaterInterface
{
    /// <summary>
    /// 会向服务器请求 每次从战斗场景退出都会请求一次
    /// </summary>
    public interface IAssetBundleUpdater
    {
        Action<BundleInfo, string, string, int> ResourceUpdateStart { get; set; }
        Action<BundleInfo, string, string, int, long> ResourceUpdateChanged { get; set; }
        Action<BundleInfo, string, string, int, long> ResourceUpdateSuccess { get; set; }
        Action<BundleInfo, string, int, int, string> ResourceUpdateFailure { get; set; }
    }
}
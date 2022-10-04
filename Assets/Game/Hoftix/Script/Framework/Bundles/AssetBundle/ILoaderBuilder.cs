using System;

namespace ZJYFrameWork.AssetBundles.Bundles
{
    public interface ILoaderBuilder
    {
        BundleLoader Create(BundleManager manager, BundleInfo bundleInfo);
    }
}

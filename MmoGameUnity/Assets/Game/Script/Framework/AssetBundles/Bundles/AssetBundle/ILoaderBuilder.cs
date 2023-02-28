using System;

namespace ZJYFrameWork.AssetBundles.Bundles
{
    public interface ILoaderBuilder
    {
        void SetLoaderBuilder(Uri baseUrl);
        BundleLoader Create(BundleManager manager, BundleInfo bundleInfo);
    }
}

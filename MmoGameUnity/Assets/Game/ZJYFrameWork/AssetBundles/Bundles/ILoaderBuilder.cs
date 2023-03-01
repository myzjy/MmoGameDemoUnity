using System;

namespace ZJYFrameWork.AssetBundles.Bundles.ILoaderBuilderInterface
{
    public interface ILoaderBuilder
    {
        void SetLoaderBuilder(Uri baseUrl);
        BundleLoader Create(BundleManager manager, BundleInfo bundleInfo);
    }
}

using System;

namespace ZJYFrameWork.AssetBundles.Bundles.ILoaderBuilderInterface
{
    public interface ILoaderBuilder
    {
        void SetUrl(Uri baseUrl);

        BundleLoader Create(BundleManager manager, BundleInfo bundleInfo);
    }
}

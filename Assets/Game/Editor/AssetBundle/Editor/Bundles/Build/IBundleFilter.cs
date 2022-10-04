using ZJYFrameWork.AssetBundles.Bundles;

namespace ZJYFrameWork.AssetBundles.EditorAssetBundle.Editors
{
    public interface IBundleFilter
    {
        bool IsValid(BundleInfo bundleInfo);
    }
}

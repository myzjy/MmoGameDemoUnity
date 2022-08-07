namespace ZJYFrameWork.AssetBundles
{
    public abstract class AssetBundleLoaderImpl
    {
        public virtual bool IsInitialized(AssetBundleManager loader)
        {
            return loader.GetCurManifest != null;
        }
    }
}
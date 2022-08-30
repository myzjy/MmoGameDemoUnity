namespace ZJYFrameWork.AssetBundles
{
    public interface BaseAssetsAsyncLoaderInterface
    {
        void Dispose();
    }
    public class BaseAssetsAsyncLoader:BaseAssetsAsyncLoaderInterface
    {
        public string assetbundleName
        {
            get;
            set;
        }

        public UnityEngine.AssetBundle assetbundle
        {
            get;
            set;
        }
        /// <summary>
        /// 当参考计数器为0时释放
        /// </summary>
        public AssetBundleLoaderBase.AssetBundleHolder.ReferenceCount refCount;
        
        public virtual void Dispose()
        {
        }
    }
}
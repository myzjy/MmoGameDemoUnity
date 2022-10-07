using Framework.AssetBundles.Config;
using Framework.AssetBundles.Utilty;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.Base.Model;

namespace ZJYFrameWork.AssetBundles
{
    public class AssetBundleManager : AbstractManager,IAssetBundleManager
    {
        public string BundleRoot => AssetBundleConfig.BundleRoot;

        public string StorableDirectory
        {
            get => BundleUtil.GetStorableDirectory();
        }

        public string ReadOnlyDirectory
        {
            get => BundleUtil.GetReadOnlyDirectory();
        }

        public string TemporaryCacheDirectory
        {
            get => BundleUtil.GetTemporaryCacheDirectory();
        }

        public string UpdatePrefixUri { get; set; }
        public string ApplicableGameVersion { get; }
        public float AssetAutoReleaseInterval { get; set; }
        public int AssetCapacity { get; set; }
        public float AssetExpireTime { get; set; }
        public float ResourceAutoReleaseInterval { get; set; }
        public int ResourceCapacity { get; set; }
        public float ResourceExpireTime { get; set; }
        public int ResourcePriority { get; set; }
        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            //不需要进行轮询，下载，更新走另外一套
            return;
            throw new System.NotImplementedException();
        }

        public override void Shutdown()
        {
            throw new System.NotImplementedException();
        }
    }
}
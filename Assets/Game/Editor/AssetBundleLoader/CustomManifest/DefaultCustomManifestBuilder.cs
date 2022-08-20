using UnityEngine;
using ZJYFrameWork.AssetBundleLoader.CustomManifest;


// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.AssetBundleLoader.Build
{
    /// <summary>
    /// 制作清单中使用的资产包数据。
    /// </summary>
    public class DefaultCustomManifestBuilder : CustomManifestBuilder<DefaultCustomManifestDataAsset, BundleData>
    {
        public DefaultCustomManifestBuilder(string dataAssetPath, string bundleOutputPath, AssetBundleManifest manifest)
            : base(dataAssetPath, bundleOutputPath, manifest)
        {
        }

        protected override BundleData CreateBundleData(string name)
        {
            var version = Manifest.GetAssetBundleHash(name).ToString();
            var crc = GetAssetBundleCrc(name);
            var size = GetFileSize(name);
            var dependenciesIndex = GetDependenciesIndex(name);
            Debug.Log($"name:{name},version:{version}");
            return new BundleData(name, version, crc, size, dependenciesIndex);
        }
    }
}
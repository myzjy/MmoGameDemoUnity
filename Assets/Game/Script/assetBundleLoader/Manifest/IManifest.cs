// ReSharper disable once CheckNamespace

namespace ZJYFrameWork.AssetBundleLoader.Manifest
{
    public interface IManifest
    {
        /// <summary>
        /// Manifest的版本返回。
        /// </summary>
        string GetManifestVersion();

        /// <summary>
        /// 返还所有的资产包名称。
        /// </summary>
        string[] GetAllAssetBundles();

        /// <summary>
        /// 从资产包名称返回所有有依赖关系的资产包名称。
        /// 在循环参照的情况下，自身可能包含在序列中。
        /// </summary>
        string[] GetAllDependencies(string assetBundleName);

        /// <summary>
        /// 从资产包名称返回版本。
        /// </summary>
        string GetAssetBundleVersion(string assetBundleName);

        /// <summary>
        /// 从资产包名称返回CRC。
        /// </summary>
        uint GetAssetBundleCrc(string assetBundleName);

        /// <summary>
        /// 从资产包名称返回文件大小。
        /// </summary>
        int GetAssetBundleSize(string assetBundleName);
    }
}
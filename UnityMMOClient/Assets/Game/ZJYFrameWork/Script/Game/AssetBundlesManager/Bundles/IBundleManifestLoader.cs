using ZJYFrameWork.Asynchronous;

namespace ZJYFrameWork.AssetBundles.Bundles.LoaderBuilders
{
    public interface IBundleManifestLoader
    {
        /// <summary>
        /// Synchronously loads a manifest.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        BundleManifest Load(string path);

        /// <summary>
        /// Asynchronously loads a manifest.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IAsyncResult<BundleManifest> LoadAsync(string path);
    }
}

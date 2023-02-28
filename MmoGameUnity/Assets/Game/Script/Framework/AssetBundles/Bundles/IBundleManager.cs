﻿using ZJYFrameWork.Asynchronous;

namespace ZJYFrameWork.AssetBundles.Bundles
{
    /// <summary>
    /// A common interface for bundle manager.
    /// </summary>
    public interface IBundleManager
    {
        /// <summary>
        /// Gets a bundle for the given bundle's name.If the Assetbundle isn't loaded, returns null.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        IBundle GetBundle(string bundleName);

        /// <summary>
        /// Asynchronously loads a bundle for the given bundle's name.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        IProgressResult<float, IBundle> LoadBundle(string bundleName);

        /// <summary>
        /// Asynchronously loads a bundle for the given bundle's name.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="priority">Positive or negative, the default value is 0.When multiple asynchronous operations are queued up, the operation with the higher priority will be executed first. Once an operation has been started on the background thread, changing the priority will have no effect anymore.</param>
        /// <returns></returns>
        IProgressResult<float, IBundle> LoadBundle(string bundleName, int priority);

        /// <summary>
        /// Asynchronously loads a group of bundles for the given bundle's names.
        /// </summary>
        /// <param name="bundleNames"></param>
        /// <returns></returns>
        IProgressResult<float, IBundle[]> LoadBundle(params string[] bundleNames);

        /// <summary>
        /// Asynchronously loads a group of bundles for the given bundle's names.
        /// </summary>
        /// <param name="bundleNames"></param>
        /// <param name="priority">Positive or negative, the default value is 0.When multiple asynchronous operations are queued up, the operation with the higher priority will be executed first. Once an operation has been started on the background thread, changing the priority will have no effect anymore.</param>
        /// <returns></returns>
        IProgressResult<float, IBundle[]> LoadBundle(string[] bundleNames, int priority);

        /// <summary>
        /// 进行设置 manifest和读取器
        /// </summary>
        /// <param name="manifest"></param>
        /// <param name="builder"></param>
        void SetManifestAndLoadBuilder(BundleManifest manifest, ILoaderBuilder builder);

    }
}

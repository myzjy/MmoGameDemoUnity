#if ASSET_BUNDLE_DEVELOP_EDITOR
using System;
using System.Collections.Generic;
using ZJYFrameWork.Asynchronous;
using ZJYFrameWork.AssetBundles.Bundles;

namespace ZJYFrameWork.AssetBundles.Bundle
{
    public interface IDownloader
    {
        Uri BaseUri { get; set; }

        int MaxTaskCount { get; set; }

        /// <summary>
        /// Get a list of files that need to be downloaded.
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        IProgressResult<float, List<BundleInfo>> GetDownloadList(BundleManifest manifest);

        /// <summary>
        /// Download the BundleManifest.Store address: BundleUtil.GetStorableDirectory() + bundles.dat,bak file: BundleUtil.GetStorableDirectory() + bundles.dat.bak
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        IProgressResult<Progress, BundleManifest> DownloadManifest(string relativePath);

        /// <summary>
        /// Download the Assetbundle.Store address:BundleUtil.GetStorableDirectory(),default:Application.persistentDataPath + "/bundles/"
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        IProgressResult<Progress, bool> DownloadBundles(List<BundleInfo> bundles);
        IProgressResult<Progress, bool> DownloadBundles(BundleInfo bundles);
    }
}
#endif
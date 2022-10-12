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
        /// 获取绝对路径的url
        /// </summary>
        /// <param name="relativePath">绝对路径的地址</param>
        /// <returns>绝对路径的url</returns>
        string GetAbsoluteUri(string relativePath);

        /// <summary>
        /// 传入一个绝对路径地址，返回一个对应的绝对路径地址
        /// </summary>
        /// <param name="relativePath">绝对路径地址</param>
        /// <returns></returns>
        string GetAbsolutePath(string relativePath);

        /// <summary>
        /// 获取url
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        Uri GetAbsoluteUrl(string relativePath);
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
        void SetDownManager(IDownloadManager _downloadManager);

    }
}
#endif
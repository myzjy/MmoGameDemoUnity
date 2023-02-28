﻿#if ASSET_BUNDLE_DEVELOP_EDITOR

#if UNITY_2017_1_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.Asynchronous;
#endif

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.AssetBundles.Bundle
{
#if UNITY_2017_1_OR_NEWER
    [Obsolete("Please use \"UnityWebRequestDownloader\" instead of this class.")]
#else
    [Bean]
#endif
    public class WWWDownloader : AbstractDownloader
    {
        protected bool useCache = false;

        public WWWDownloader()
        {
        }

        public WWWDownloader(Uri baseUri, bool useCache) : this(baseUri, SystemInfo.processorCount * 2, useCache)
        {
        }

        public WWWDownloader(Uri baseUri, int maxTaskCount, bool useCache) : base(baseUri, maxTaskCount)
        {
            this.useCache = useCache;
        }

#if UNITY_2017_1_OR_NEWER
        protected override IEnumerator DoDownloadBundles(IProgressPromise<Progress, bool> promise,
            List<BundleInfo> bundles)
        {
            long totalSize = 0;
            long downloadedSize = 0;
            Progress progress = new Progress();
            List<BundleInfo> list = new List<BundleInfo>();
            foreach (var info in bundles)
            {
                totalSize += info.FileSize;
                if (BundleUtil.Exists(info))
                {
                    downloadedSize += info.FileSize;
                    continue;
                }

                list.Add(info);
            }

            progress.TotalCount = bundles.Count;
            progress.CompletedCount = bundles.Count - list.Count;
            progress.TotalSize = totalSize;
            progress.CompletedSize = downloadedSize;
            yield return null;

            List<KeyValuePair<BundleInfo, UnityWebRequest>> tasks =
                new List<KeyValuePair<BundleInfo, UnityWebRequest>>();
            for (int i = 0; i < list.Count; i++)
            {
                BundleInfo bundleInfo = list[i];

                UnityWebRequest www;
                if (useCache && !bundleInfo.IsEncrypted)
                {
#if UNITY_2018_1_OR_NEWER
                    www = UnityWebRequestAssetBundle.GetAssetBundle(GetAbsoluteUri(bundleInfo.Filename),
                        bundleInfo.Hash, 0);
#else
                    www = UnityWebRequest.GetAssetBundle(GetAbsoluteUri(bundleInfo.Filename), bundleInfo.Hash, 0);
#endif
                }
                else
                {
                    www = new UnityWebRequest(GetAbsoluteUri(bundleInfo.Filename));
                    www.downloadHandler = new DownloadHandlerBuffer();
                }

#if UNITY_2018_1_OR_NEWER
                www.SendWebRequest();
#else
                www.Send();
#endif
                tasks.Add(new KeyValuePair<BundleInfo, UnityWebRequest>(bundleInfo, www));

                while (tasks.Count >= this.MaxTaskCount || (i == list.Count - 1 && tasks.Count > 0))
                {
                    long tmpSize = 0;
                    for (int j = tasks.Count - 1; j >= 0; j--)
                    {
                        var task = tasks[j];
                        BundleInfo _bundleInfo = task.Key;
                        UnityWebRequest _www = task.Value;

                        if (!_www.isDone)
                        {
                            tmpSize += (long)Math.Max(0,
                                _www.downloadedBytes); //the UnityWebRequest.downloadedProgress has a bug in android platform
                            continue;
                        }

                        progress.CompletedCount += 1;
                        tasks.RemoveAt(j);
                        downloadedSize += _bundleInfo.FileSize;
                        if (!string.IsNullOrEmpty(_www.error))
                        {
                            promise.SetException(new Exception(_www.error));
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.Log(
                                $"从地址[url:{GetAbsoluteUri(_bundleInfo.Filename)}]下载[AssetBundle:{_bundleInfo.FullName}]失败。[原因:{_www.error}]");
#endif
                            yield break;
                        }

                        try
                        {
                            if (useCache && !bundleInfo.IsEncrypted)
                            {
                                AssetBundle bundle = ((DownloadHandlerAssetBundle)_www.downloadHandler).assetBundle;
                                if (bundle != null)
                                {
                                    bundle.Unload(true);
                                }
                            }
                            else
                            {
                                string fullname = BundleUtil.GetStorableDirectory() + _bundleInfo.Filename;
                                FileInfo info = new FileInfo(fullname);
                                if (info.Exists)
                                {
                                    info.Delete();
                                }

                                if (info.Directory is { Exists: false })
                                {
                                    info.Directory.Create();
                                }

                                File.WriteAllBytes(info.FullName, _www.downloadHandler.data);
                            }
                        }
                        catch (Exception e)
                        {
                            promise.SetException(e);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.Log(
                                $"[Downloads] [AssetBundle:{_bundleInfo.FullName}] 下载失败 [Url:{GetAbsoluteUri(_bundleInfo.Filename)}].原因:{e}");
#endif
                            yield break;
                        }
                    }

                    progress.CompletedSize = downloadedSize + tmpSize;
                    promise.UpdateProgress(progress);

                    yield return null;
                }
            }

            promise.SetResult(true);
        }

        protected override IEnumerator DoDownloadBundles(IProgressPromise<Progress, bool> promise,
            BundleInfo bundleInfo)
        {
            throw new NotImplementedException();
        }
#else
        protected override IEnumerator DoDownloadBundles(IProgressPromise<Progress, bool> promise, List<BundleInfo> bundles)
        {
            long totalSize = 0;
            long downloadedSize = 0;
            Progress progress = new Progress();
            List<BundleInfo> list = new List<BundleInfo>();
            for (int i = 0; i < bundles.Count; i++)
            {
                var info = bundles[i];
                totalSize += info.FileSize;
                if (BundleUtil.Exists(info))
                {
                    downloadedSize += info.FileSize;
                    continue;
                }
                list.Add(info);
            }

            progress.TotalCount = bundles.Count;
            progress.CompletedCount = bundles.Count - list.Count;
            progress.TotalSize = totalSize;
            progress.CompletedSize = downloadedSize;
            yield return null;

            List<KeyValuePair<BundleInfo, WWW>> tasks = new List<KeyValuePair<BundleInfo, WWW>>();
            for (int i = 0; i < list.Count; i++)
            {
                BundleInfo bundleInfo = list[i];
                WWW www =
 (useCache && !bundleInfo.IsEncrypted) ? WWW.LoadFromCacheOrDownload(GetAbsoluteUri(bundleInfo.Filename), bundleInfo.Hash) : new WWW(GetAbsoluteUri(bundleInfo.Filename));
                tasks.Add(new KeyValuePair<BundleInfo, WWW>(bundleInfo, www));

                while (tasks.Count >= this.MaxTaskCount || (i == list.Count - 1 && tasks.Count > 0))
                {
                    long tmpSize = 0;
                    for (int j = tasks.Count - 1; j >= 0; j--)
                    {
                        var task = tasks[j];
                        BundleInfo _bundleInfo = task.Key;
                        WWW _www = task.Value;

                        if (!_www.isDone)
                        {                            
                            tmpSize += Math.Max(0, (long)(_www.progress * _bundleInfo.FileSize));
                            continue;
                        }

                        progress.CompletedCount += 1;
                        tasks.RemoveAt(j);
                        downloadedSize += _bundleInfo.FileSize;
                        if (!string.IsNullOrEmpty(_www.error))
                        {
                            promise.SetException(new Exception(_www.error));
                            if (log.IsErrorEnabled)
                                log.ErrorFormat("Downloads AssetBundle '{0}' failure from the address '{1}'.Reason:{2}", _bundleInfo.FullName, GetAbsoluteUri(_bundleInfo.Filename), _www.error);
                            yield break;
                        }

                        try
                        {
                            if (useCache && !_bundleInfo.IsEncrypted)
                            {
                                AssetBundle bundle = _www.assetBundle;
                                if (bundle != null)
                                    bundle.Unload(true);
                            }
                            else
                            {
                                string fullname = BundleUtil.GetStorableDirectory() + _bundleInfo.Filename;
                                FileInfo info = new FileInfo(fullname);
                                if (info.Exists)
                                    info.Delete();

                                if (!info.Directory.Exists)
                                    info.Directory.Create();

                                File.WriteAllBytes(info.FullName, _www.bytes);
                            }
                        }
                        catch (Exception e)
                        {
                            promise.SetException(e);
                            if (log.IsErrorEnabled)
                                log.ErrorFormat("Downloads AssetBundle '{0}' failure from the address '{1}'.Reason:{2}", _bundleInfo.FullName, GetAbsoluteUri(_bundleInfo.Filename), e);
                            yield break;
                        }
                    }

                    progress.CompletedSize = downloadedSize + tmpSize;
                    promise.UpdateProgress(progress);

                    yield return null;
                }
            }
            promise.SetResult(true);
        }
#endif
    }
}
#endif
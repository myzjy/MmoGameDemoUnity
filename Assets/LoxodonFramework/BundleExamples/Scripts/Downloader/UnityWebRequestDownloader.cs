#if ASSET_BUNDLE_DEVELOP_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using ZJYFrameWork.Asynchronous;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.Net.Http;
using UnityEngine;
using UnityEngine.Networking;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.AssetBundles.Bundle
{
#if UNITY_2017_1_OR_NEWER
    [Bean]
#endif
    public class UnityWebRequestDownloader : AbstractDownloader
    {
        public UnityWebRequestDownloader()
        {
        }

        public UnityWebRequestDownloader(Uri baseUri) : this(baseUri, SystemInfo.processorCount * 2)
        {
        }

        public UnityWebRequestDownloader(Uri baseUri, int maxTaskCount) : base(baseUri, maxTaskCount)
        {
        }

        public override IProgressResult<Progress, bool> DownloadBundles(BundleInfo bundles)
        {
            ProgressResult<Progress, bool> result = new ProgressResult<Progress, bool>();

            
            result.SetResult(true);
            
            
            return result;
        }

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
            for (var i = 0; i < list.Count; i++)
            {
                var bundleInfo = list[i];
                var fullname = BundleUtil.GetStorableDirectory() + bundleInfo.Filename;
                var www = new UnityWebRequest(GetAbsoluteUri(bundleInfo.Filename));
                www.downloadHandler = new DownloadFileHandler(fullname);
                www.timeout = 30;
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
#if UNITY_2018_1_OR_NEWER
                        if (_www.isNetworkError)
#else
                        if (_www.isError)
#endif
                        {
                            promise.SetException(new Exception(_www.error));
                            ZJYFrameWork.Debug.Log("从地址'[{}]'下载AssetBundle '[{}]'失败。原因:[{}]", _bundleInfo.FullName,
                                GetAbsoluteUri(_bundleInfo.Filename), _www.error);
                            _www.Dispose();

                            try
                            {
                                foreach (var kv in tasks)
                                {
                                    kv.Value.Dispose();
                                }
                            }
                            catch (Exception)
                            {
                            }

                            yield break;
                        }

                        _www.Dispose();
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
            long totalSize = 0;
            long downloadedSize = 0;
            Progress progress = new Progress();
            List<BundleInfo> list = new List<BundleInfo>();

            totalSize += bundleInfo.FileSize;
            if (BundleUtil.Exists(bundleInfo))
            {
                downloadedSize += bundleInfo.FileSize;
            }

            list.Add(bundleInfo);
            progress.TotalCount = 1;
            progress.CompletedCount = 0;
            progress.TotalSize = totalSize;
            progress.CompletedSize = downloadedSize;
            yield return null;

            List<KeyValuePair<BundleInfo, UnityWebRequest>> tasks =
                new List<KeyValuePair<BundleInfo, UnityWebRequest>>();
            for (var i = 0; i < list.Count; i++)
            {
                bundleInfo = list[i];
                var fullname = BundleUtil.GetStorableDirectory() + bundleInfo.Filename;
                var www = new UnityWebRequest(GetAbsoluteUri(bundleInfo.Filename));
                www.downloadHandler = new DownloadFileHandler(fullname);
                www.timeout = 30;
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
#if UNITY_2018_1_OR_NEWER
                        if (_www.result == UnityWebRequest.Result.ConnectionError)
//                      if (_www.isNetworkError)
#else
                        if (_www.isError)
#endif
                            {
                                promise.SetException(new Exception(_www.error));
                                ZJYFrameWork.Debug.Log("从地址'[{}]'下载AssetBundle '[{}]'失败。原因:[{}]", _bundleInfo.FullName,
                                    GetAbsoluteUri(_bundleInfo.Filename), _www.error);
                                _www.Dispose();

                                try
                                {
                                    foreach (var kv in tasks)
                                    {
                                        kv.Value.Dispose();
                                    }
                                }
                                catch (Exception)
                                {
                                }

                                yield break;
                            }

                        _www.Dispose();
                    }

                    progress.CompletedSize = downloadedSize + tmpSize;
                    promise.UpdateProgress(progress);

                    yield return null;
                }
            }

            promise.SetResult(true);
        }
    }
}
#endif
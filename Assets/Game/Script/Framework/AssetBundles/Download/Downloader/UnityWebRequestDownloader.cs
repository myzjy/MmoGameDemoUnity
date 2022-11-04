#if ASSET_BUNDLE_DEVELOP_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BestHTTP;
using Framework.AssetBundles.Config;
using Framework.AssetBundles.Utilty;
using ZJYFrameWork.Asynchronous;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.Net.Http;
using UnityEngine;
using UnityEngine.Networking;
using ZJYFrameWork.AssetBundles.Download;
using ZJYFrameWork.Collection.Reference;
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

        private const float TimeoutSec = 6f; //10 => 6 => 3 => 6
        private List<BundleInfo> errrorList = new List<BundleInfo>();

        protected IEnumerator DownloadHttpBundles(IProgressPromise<Progress, bool> promise,
            List<BundleInfo> bundles)
        {
            long totalSize = 0;
            long downloadedSize = 0;
            Progress eventProgressBar = new Progress();
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

            eventProgressBar.TotalCount = bundles.Count;
            eventProgressBar.CompletedCount = bundles.Count - list.Count;
            eventProgressBar.TotalSize = totalSize;
            eventProgressBar.CompletedSize = downloadedSize;
            yield return null;
            List<KeyValuePair<BundleInfo, HTTPRequest>> tasks = new List<KeyValuePair<BundleInfo, HTTPRequest>>();
            for (int i = 0; i < list.Count; i++)
            {
                BundleInfo bundleInfo = list[i];

                string fullname = $"{BundleUtil.GetStorableDirectory()}{bundleInfo.Filename}";
#if UNITY_EDITOR || DEVELOP_BUILD
                Debug.Log($"需要下载文件路径{fullname}");
#endif
                HTTPRequest request = new HTTPRequest(new Uri(GetAbsoluteUri(bundleInfo.Filename)));

                // 超时设定
                request.ConnectTimeout = TimeSpan.FromSeconds(TimeoutSec);
                request.Timeout = TimeSpan.FromSeconds(TimeoutSec);
                request.Callback += (HTTPRequest originalBhRequest, HTTPResponse bhResponse) =>
                {
                    LogResponse(bhResponse, originalBhRequest);

                    switch (originalBhRequest.State)
                    {
                        case HTTPRequestStates.Initial:
                            break;
                        case HTTPRequestStates.Queued:
                            break;
                        case HTTPRequestStates.Processing:
                            break;

                        case HTTPRequestStates.Finished:
                        {
                            downloadedSize += bundleInfo.FileSize;
                            // 请求完成后没有任何问题。
                            eventProgressBar.CompletedCount += 1;
                        }
                            break;
                        case HTTPRequestStates.Error:
                        {
                            // errrorList.Add(bundleInfo);
                            Debug.LogError($"下载错误：{bundleInfo.Filename},{bundleInfo.FullName}");
                        }
                            break;
                        case HTTPRequestStates.Aborted:
                            break;
                        case HTTPRequestStates.ConnectionTimedOut:
                            break;
                        case HTTPRequestStates.TimedOut:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                };
                long tmpSize = 0;

                request.OnDownloadProgress += (res, downloaded, length) =>
                {
                    float progressPercent = (downloaded / (float)length) * 100.0f;
                    Debug.Log("Downloaded: " + progressPercent.ToString("F2") + "%");
                };

                //第一个参数 请求：原始对象HTTPRequest
                //第二个参数 响应：数据所属的对象。通过此对象可以访问所有已接收的信息（状态代码、标头等）HTTPResponse
                //第三个参数 数据片段：实际下载的字节数。因为插件重用字节数组，它的长度可以大于下载的数据，所以必须使用参数代替！dataFragment.Length dataFragmentLength
                //第四个参数 dataFragmentLength：dataFragment 参数的实际下载字节数。使用此参数代替 ！dataFragment.Length
                request.OnStreamingData += (req, resp, dataFragment, dataFragmentLength) =>
                {
                    //下载读取留
                    if (resp.IsSuccess)
                    {
                        //请求成功
                        var fs = req.Tag as FileStream;
                        if (fs == null)
                        {
                            req.Tag = fs = new System.IO.FileStream(fullname, System.IO.FileMode.Create);
                        }

                        tmpSize += Math.Max(0,
                            dataFragment.Length);

                        fs.Write(dataFragment, 0, dataFragmentLength);
                        eventProgressBar.CompletedSize = downloadedSize + tmpSize;
                        promise.UpdateProgress(eventProgressBar);
                    }

                    //如果dataFragment被处理，插件可以回收它，则返回true
                    return true;
                };
                request.Send();
                while (request.State < HTTPRequestStates.Finished)
                {
                    yield return new WaitForSeconds(0.1f);

                    // tmpSize+=request.Response.CacheFileInfo.
                }
            }

            promise.SetResult(true);

            yield break;
        }

        private void LogResponse(HTTPResponse response, HTTPRequest request)
        {
#if DEVELOP_BUILD || UNITY_EDITOR
            StringBuilder sb = new StringBuilder();
            sb.Append(
                $"[ApiResponse] [Download] {request.Uri.AbsoluteUri}\n");
            sb.Append($"{response.StatusCode} {response.Message}  ");
            foreach (var item in response.Headers)
            {
                sb.Append(item.Key).Append(": ");
                var count = item.Value.Count;
                for (var i = 0; i < count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(",");
                    }

                    sb.Append(item.Value[i]);
                }

                sb.Append("\n");
            }

            Debug.Log($"{request.Uri.AbsoluteUri}");
            Debug.Log($"{sb.ToString()}");
#endif
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
                DownloadStartEventArgs downloadStartEventArgs = DownloadStartEventArgs.Create(bundleInfo,
                    downloadPath: fullname, GetAbsoluteUri(bundleInfo.Filename), bundleInfo.FileSize, bundleInfo);

                DownloadManager.GetDownloadStart(bundleInfo, downloadStartEventArgs);
                ReferenceCache.Release(downloadStartEventArgs);

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
                        var downFilenamePath = BundleUtil.GetStorableDirectory() + bundleInfo.Filename;
                        if (!_www.isDone)
                        {
                            tmpSize += (long)Math.Max(0,
                                _www.downloadedBytes); //the UnityWebRequest.downloadedProgress has a bug in android platform
                            DownloadUpdateEventArgs eventArgs = DownloadUpdateEventArgs.Create(serialId: _bundleInfo,
                                downloadPath: downFilenamePath, downloadUri: _www.url
                                , currentLength: tmpSize, _bundleInfo);
                            DownloadManager.GetDownloadUpdate(_bundleInfo, eventArgs);
                            ReferenceCache.Release(eventArgs);

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
                            string errorMessage =
                                $"从地址'[{GetAbsoluteUri(_bundleInfo.Filename)}]'下载AssetBundle '[{_bundleInfo.FullName}]'失败。原因:[{_www.error}]";
                            ZJYFrameWork.Debug.Log(errorMessage);
                            _www.Dispose();
                            DownloadFailureEventArgs failureEventArgs = DownloadFailureEventArgs.Create(_bundleInfo,
                                downFilenamePath, _www.url, errorMessage, _bundleInfo);
                            DownloadManager.GetDownloadFailure(_bundleInfo, failureEventArgs);
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
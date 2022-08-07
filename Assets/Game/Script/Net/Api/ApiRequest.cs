using System;
using BestHTTP;
using UnityEngine;

namespace ZJYFrameWork.Net
{
    public class ApiRequest
    {
    const float timeoutSec = 6f; //10 => 6 => 3 => 6

		internal HTTPRequest bhRequest;

		public Uri Uri { get; private set; }
		public HTTPMethods Method { get; private set; }

		private Action<ApiRequest> onBeforeSend;
		private Action<long, long> onProgress;
		private Action<ApiResponse> onComplete;
		private Action<ApiResponse> onSuccess;
		private Action<ApiResponse> onError;

		System.Diagnostics.Stopwatch watch;


		public ApiRequest(HTTPMethods method, Uri uri, byte[] data = null, Action<ApiRequest> onBeforeSend = null,
			Action<ApiResponse> onSuccess = null, Action<ApiResponse> onError = null,
			Action<ApiResponse> onComplete = null)
		{
			this.Uri = uri;
			this.Method = method;
			this.bhRequest = new HTTPRequest(uri, method);

			// タイムアウト設定
			bhRequest.ConnectTimeout = TimeSpan.FromSeconds(timeoutSec);
			bhRequest.Timeout = TimeSpan.FromSeconds(timeoutSec);

			// 自動リトライは危険なのでアプリ側で制御する
			// bhRequest.DisableRetry = true;

#if !BESTHTTP_DISABLE_CACHING && (!UNITY_WEBGL || UNITY_EDITOR)
			// キャッシュは使わない
			bhRequest.DisableCache = true;
#endif

			bhRequest.SetHeader("Accept-Encoding", "gzip");
			bhRequest.SetHeader("App-Version", Application.version);
			bhRequest.SetHeader("User-Agent", UserAgent.Value);


			bhRequest.RawData = data;

		


			bhRequest.Callback = HandleResponse;

			this.onBeforeSend = onBeforeSend;
			this.onSuccess = onSuccess;
			this.onError = onError;
			this.onComplete = onComplete;
		}

		public void Send(Action<long, long> onProgress = null)
		{
			if (this.onBeforeSend != null)
			{
				onBeforeSend(this);
			}

			this.onProgress = onProgress;

			if (this.onProgress != null)
			{
				// bhRequest.OnProgress = (req, loaded, total) => this.onProgress(loaded, total);
				this.onProgress(0, 0);
			}

			watch = new System.Diagnostics.Stopwatch();
			watch.Start();

			bhRequest.Send();
		}

		public void Abort()
		{
			bhRequest.Abort();
			onBeforeSend = null;
			onComplete = null;
			onSuccess = null;
			onError = null;
			watch = null;
		}

		public void SetHeader(string name, string value)
		{
			bhRequest.SetHeader(name, value);
		}


		public string DumpHeaders()
		{
			return bhRequest.DumpHeaders();
		}

		private void HandleResponse(HTTPRequest originalBhRequest, HTTPResponse bhResponse)
		{
			watch.Stop();
			long elapsedMsec = watch.ElapsedMilliseconds;
			ApiResponse response = new ApiResponse(this, bhResponse, elapsedMsec);

			if (onProgress != null && bhResponse != null)
			{
				onProgress(bhResponse.Data.Length, bhResponse.Data.Length);
			}

			if (response.IsSuccess)
			{
				if (this.onSuccess != null)
				{
					onSuccess(response);
				}
			}
			else
			{
				if (this.onError != null)
				{
					onError(response);
				}
			}

			if (this.onComplete != null)
			{
				this.onComplete(response);
			}
		}    
    }
}
using System;
using BestHTTP;
using UnityEngine;

namespace ZJYFrameWork.Net
{
    public class ApiRequest
    {
        private const float TimeoutSec = 6f; //10 => 6 => 3 => 6

        internal HTTPRequest _bhRequest;

        public Uri Uri { get; private set; }
        public HTTPMethods Method { get; private set; }

        private Action<ApiRequest> _onBeforeSend;
        private Action<long, long> _onProgress;
        private Action<ApiResponse> _onComplete;
        private Action<ApiResponse> _onSuccess;
        private Action<ApiResponse> _onError;

        private System.Diagnostics.Stopwatch _watch;


        public ApiRequest(HTTPMethods method, Uri uri, byte[] data = null, Action<ApiRequest> onBeforeSend = null,
            Action<ApiResponse> onSuccess = null, Action<ApiResponse> onError = null,
            Action<ApiResponse> onComplete = null)
        {
            this.Uri = uri;
            this.Method = method;
            this._bhRequest = new HTTPRequest(uri, method);

            // 超时设定
            _bhRequest.ConnectTimeout = TimeSpan.FromSeconds(TimeoutSec);
            _bhRequest.Timeout = TimeSpan.FromSeconds(TimeoutSec);


#if !BESTHTTP_DISABLE_CACHING && (!UNITY_WEBGL || UNITY_EDITOR)
            // 不使用现金
            _bhRequest.DisableCache = true;
#endif

            _bhRequest.SetHeader("Accept-Encoding", "gzip");
            _bhRequest.SetHeader("App-Version", Application.version);
            _bhRequest.SetHeader("User-Agent", UserAgent.Value);
            _bhRequest.SetHeader("Content-Type", "application/json");

            _bhRequest.RawData = data;

            _bhRequest.Callback = HandleResponse;

            this._onBeforeSend = onBeforeSend;
            this._onSuccess = onSuccess;
            this._onError = onError;
            this._onComplete = onComplete;
        }

        public void Send(Action<long, long> onProgress = null)
        {
            if (this._onBeforeSend != null)
            {
                _onBeforeSend(this);
            }

            this._onProgress = onProgress;

            if (this._onProgress != null)
            {
                // bhRequest.OnProgress = (req, loaded, total) => this.onProgress(loaded, total);
                this._onProgress(0, 0);
            }

            _watch = new System.Diagnostics.Stopwatch();
            _watch.Start();

            _bhRequest.Send();
        }

        public void Abort()
        {
            _bhRequest.Abort();
            _onBeforeSend = null;
            _onComplete = null;
            _onSuccess = null;
            _onError = null;
            _watch = null;
        }

        public void SetHeader(string name, string value)
        {
            _bhRequest.SetHeader(name, value);
        }


        public string DumpHeaders()
        {
            return _bhRequest.DumpHeaders();
        }

        private void HandleResponse(HTTPRequest originalBhRequest, HTTPResponse bhResponse)
        {
            _watch.Stop();
            long elapsedMsec = _watch.ElapsedMilliseconds;
            ApiResponse response = new ApiResponse(this, bhResponse, elapsedMsec);

            if (_onProgress != null && bhResponse != null)
            {
                _onProgress(bhResponse.Data.Length, bhResponse.Data.Length);
            }

            if (response.IsSuccess)
            {
                if (this._onSuccess != null)
                {
                    _onSuccess(response);
                }
            }
            else
            {
                if (this._onError != null)
                {
                    _onError(response);
                }
            }

            if (this._onComplete != null)
            {
                this._onComplete(response);
            }
        }
    }
}
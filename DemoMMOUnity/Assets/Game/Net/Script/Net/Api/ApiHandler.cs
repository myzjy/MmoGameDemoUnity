using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using ZJYFrameWork.Messaging;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Net
{
    public class ApiHandler : MonoBehaviour
    {
        public delegate void OnBeforeSendDelegate(ApiRequest request);

        public delegate void OnCompleteDelegate(ApiResponse response);

        public delegate void OnErrorDelegate(ApiResponse response, IError error);

        public delegate void OnResponseDelegate(ApiResponse response);

        // private ICertificateVerifyer certificateVerifyer;
        private Queue<ApiRequest> apiQueue = new Queue<ApiRequest>(32);

        /// <summary>
        /// token
        /// </summary>
        private string authToken;

        private Uri beasUrl;
        protected ILogger logger;

        public OnBeforeSendDelegate onBeforeSendGlobal = null;

        public OnCompleteDelegate onCompleteGlobal = null;

        public OnErrorDelegate onErrorGlobal = null;

        public OnResponseDelegate onResponseGlobal = null;

        public bool IsAuthenticated => !string.IsNullOrEmpty(authToken);

        public ApiHandler Setup(string baseUri, string authToken = null)
        {
            if (baseUri == "REMOVED_URL")
            {
                return this;
            }

            this.beasUrl = new Uri(baseUri);
            this.authToken = authToken;

            this.logger = UnityEngine.Debug.unityLogger;

            return this;
        }

        public void SetLogger(ILogger logger)
        {
            this.logger = logger;
        }

        public void Request<TRequestData, TResponseData, TError>(ApiHttp<TRequestData, TResponseData, TError> api)
            where TRequestData : Model, new()
            where TResponseData : Model, new()
            where TError : Model, IError, new()
        {
            string relativePath = api.Path;
            byte[] data = null;
            if (api.Method == BestHTTP.HttpMethods.Get || api.Method == BestHTTP.HttpMethods.Head)
            {
                relativePath += api.Param.BuildQuery();
            }
            else
            {
                data = api.Param.Pack();
            }

            ApiRequest request = new ApiRequest(
                method: api.Method,
                uri: new Uri(beasUrl, relativePath),
                data: data,
                onBeforeSend: req => OnBeforeSendCallback(request: req, api),
                onSuccess: res => StartCoroutine(OnSuccessRoutine(res, api)),
                onError: res => StartCoroutine(OnErrorRoutine(res, api)));
            if (!string.IsNullOrEmpty(authToken))
            {
                request.SetHeader("Authorization", authToken);
            }

            LogRequest(request, api.Param);

            apiQueue.Enqueue(request);

            if (apiQueue.Count == 1)
            {
                SendNext();
            }
        }

        public void OnBeforeSendCallback<TRequestData, TResponseData, TError>(ApiRequest request,
            ApiHttp<TRequestData, TResponseData, TError> api)
            where TRequestData : Model, new()
            where TResponseData : Model, new()
            where TError : Model, IError, new()
        {
            if (onBeforeSendGlobal != null)
            {
                onBeforeSendGlobal.Invoke(request: request);
            }

            if (api.onBeforeSend != null)
            {
                api.onBeforeSend();
            }
        }

        private IEnumerator OnSuccessRoutine<TRequestData, TResponseData, TError>(ApiResponse response,
            ApiHttp<TRequestData, TResponseData, TError> api)
            where TRequestData : Model, new()
            where TResponseData : Model, new()
            where TError : Model, IError, new()
        {
            TResponseData data = null;
            yield return UnpackData<TResponseData>(response.RawData, res => data = res);
            LogResponse(response, data);
            OnResponseCallbackHandler(response);

            if (api.onSuccess != null)
            {
                SpringContext.GetBean<Messenger>().Publish(data);
            }

            OnCompleteCallbackHandler(response, api.onComplete);
            apiQueue.Dequeue();
            SendNext();
        }

        public void Retry(ApiRequest request)
        {
            request.Send();
        }

        private void SendNext()
        {
            if (apiQueue.Count <= 0) return;
            var request = apiQueue.Peek();
            request.Send();
        }

        public void ResetQueue()
        {
            apiQueue.Clear();
        }

        private void OnCompleteCallbackHandler(ApiResponse response, Action callback)
        {
            if (onCompleteGlobal != null)
            {
                onCompleteGlobal.Invoke(response);
            }

            if (callback != null)
            {
                callback();
            }
        }

        private void OnResponseCallbackHandler(ApiResponse response)
        {
            if (onResponseGlobal != null)
            {
                onResponseGlobal.Invoke(response);
            }
        }

        public void SetAuthToken(string authToken)
        {
            this.authToken = authToken;
        }

        private IEnumerator UnpackData<TResponseData>(byte[] bytes, Action<TResponseData> callback)
            where TResponseData : Model, new()
        {
            TResponseData data = null;
            Exception exception = null;
            bool isDone = false;

            try
            {
                data = new TResponseData();
                data.Unpack(bytes);
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                isDone = true;
            }

            yield return new WaitUntil(() => isDone);

            if (exception != null && !(data is Error))
            {
                throw exception;
            }

            if (callback != null)
            {
                callback(data);
            }
        }

        private IEnumerator OnErrorRoutine<TRequestData, TResponseData, TError>(ApiResponse response,
            ApiHttp<TRequestData, TResponseData, TError> api)
            where TRequestData : Model, new()
            where TResponseData : Model, new()
            where TError : Model, IError, new()
        {
            TError error = null;
            yield return UnpackData<TError>(response.RawData, e => error = e);

            LogResponse(response, error);

            OnResponseCallbackHandler(response);

            if (api.ignoreError)
            {
                OnCompleteCallbackHandler(response, api.onComplete);

                apiQueue.Dequeue();
                SendNext();
            }
            else
            {
                if (onErrorGlobal != null)
                {
                    onErrorGlobal.Invoke(response, error);
                }

                if (api.onError != null)
                {
                    api.onError(error);
                }

                OnCompleteCallbackHandler(response, api.onComplete);
            }
        }


        private void LogRequest(ApiRequest request, Model data)
        {
#if DEVELOP_BUILD || UNITY_EDITOR
            Debug.Log($"[ApiRequest] {request.Method.ToString().ToUpper()} {request.Uri.AbsoluteUri}\n" +
                      $"{request.DumpHeaders()}\n\n{JsonConvert.SerializeObject(data)}\n");
#endif
        }

        private void LogResponse(ApiResponse response, Model data)
        {
#if DEVELOP_BUILD || UNITY_EDITOR
            StringBuilder sb = new StringBuilder();
            sb.Append(
                $"[ApiResponse] {response.Request.Method.ToString().ToUpper()} {response.Request.Uri.AbsoluteUri}\n");
            sb.Append($"{response.StatusCode} {response.StatusMessage}  ({response.ElapsedMilliseconds}ms)");
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

            Debug.Log($"{response.Request.Uri.AbsoluteUri}:{(Model)data}");
            Debug.Log($"{sb.ToString()}");
#endif
        }
    }
}
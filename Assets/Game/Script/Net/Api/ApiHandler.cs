using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace ZJYFrameWork.Net
{
    public class ApiHandler : MonoBehaviour
    {
        private Uri beasUrl;

        /// <summary>
        /// token
        /// </summary>
        private string authToken;

        // private ICertificateVerifyer certificateVerifyer;
        private Queue<ApiRequest> apiQueue = new Queue<ApiRequest>(32);
        protected ILogger logger;

        public delegate void OnBeforeSendDelegate(ApiRequest request);

        public OnBeforeSendDelegate onBeforeSendGlobal = null;

        public delegate void OnResponseDelegate(ApiResponse response);

        public OnResponseDelegate onResponseGlobal = null;

        public delegate void OnCompleteDelegate(ApiResponse response);

        public OnCompleteDelegate onCompleteGlobal = null;

        public delegate void OnErrorDelegate(ApiResponse response, Error error);

        public OnErrorDelegate onErrorGlobal = null;

        public ApiHandler Setup(string baseUri, string authToken = null)
        {
            if (baseUri == "REMOVED_URL")
            {
                return this;
            }

            this.beasUrl = new Uri(baseUri);
            this.authToken = authToken;
            ;
            this.logger = Debug.unityLogger;

            return this;
        }

        public bool IsAuthenticated
        {
            get { return !string.IsNullOrEmpty(authToken); }
        }

        public void SetLogger(ILogger logger)
        {
            this.logger = logger;
        }
        
        public void Request<TRequestData, TResponseData, TError>()
        

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

            if(exception != null && !(data is Error))
            {
                throw exception;
            }
            if(callback != null)
            {
                callback(data);
            }

        }

        private void LogRequest(ApiRequest request, Model data)
        {
#if DEVELOP_BUILD || UNITY_EDITOR
            ToolsDebug.Log($"[ApiRequest] {request.Method.ToString().ToUpper()} {request.Uri.AbsoluteUri}\n" +
                           $"{request.DumpHeaders()}\n\n{JsonConvert.SerializeObject(data)}\n");
#endif
        }

        private void LogResponse(ApiResponse response, Model data)
        {
        }
    }
}
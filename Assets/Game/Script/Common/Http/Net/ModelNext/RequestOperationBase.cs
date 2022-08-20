using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using GameSystem.ServiceDataPro;
using Newtonsoft.Json;
// using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace GameData.Net
{
    public static class WebApiRequestHelperHander
    {
        //默认请求web内容编码格式
        private const string DefaultContentType = "application/json";

        //请求数据
        public class ApiRequestData
        {
            public UnityWebRequest WebRequest { private set; get; }

            /// <summary>
            /// 请求数据 
            /// </summary>
            /// <param name="_uri">请求地址</param>
            /// <param name="method"> 请求类型 get post</param>
            /// <param name="timeOut"> 设置UnityWebRequest在超时时间过了几秒后尝试中止  </param>
            public ApiRequestData(Uri _uri, string method, int timeOut)
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;

                WebRequest = new UnityWebRequest(_uri.OriginalString, method);
                //持有一个DownloadHandler对象的引用，该对象管理由这个UnityWebRequest从远程服务器接收到的body数据  
                //downloadHandler-->卸载处理程序
                //DownloadHandlerBuffer()-->一个通用的DownloadHandler实现，它将接收到的数据存储在一个本地字节缓冲区中  
                WebRequest.downloadHandler = new DownloadHandlerBuffer();
                //设置UnityWebRequest在超时时间过了几秒后尝试中止  
                WebRequest.timeout = timeOut;
                WebRequest.certificateHandler = new BypassCertificate();
            }

            public ApiRequestData(Uri _uri, string method, int timeOut, byte[] post)
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;

                WebRequest = new UnityWebRequest(_uri.OriginalString, method);
                WebRequest.uploadHandler = (UploadHandler) new UploadHandlerRaw(post);
                WebRequest.uploadHandler.contentType = "application/x-www-form-urlencoded";
                //持有一个DownloadHandler对象的引用，该对象管理由这个UnityWebRequest从远程服务器接收到的body数据  
                //downloadHandler-->卸载处理程序
                //DownloadHandlerBuffer()-->一个通用的DownloadHandler实现，它将接收到的数据存储在一个本地字节缓冲区中  
                WebRequest.downloadHandler = new DownloadHandlerBuffer();
                //设置UnityWebRequest在超时时间过了几秒后尝试中止  
                WebRequest.timeout = timeOut;
                WebRequest.certificateHandler = new BypassCertificate();
            }

            /// <summary>
            /// 将HTTP请求头设置为自定义值
            /// </summary>
            /// <param name="key">要设置的头文件的键。 大小写敏感的。  </param>
            /// <param name="value">头文件的预期值  </param>
            public void AddHeader(string key, string value)
            {
                //
                WebRequest.SetRequestHeader(key, value);
                HeaderDict[key] = value;
            }

            Dictionary<string, string> HeaderDict = new Dictionary<string, string>();

            public Dictionary<string, string> GetHeader()
            {
                return HeaderDict;
            }

            /// <summary>
            /// 输入参数的内容被复制到本机缓冲区。  
            /// </summary>
            /// <param name="body">原始数据传输到远程服务器。</param>
            public void SetBody(byte[] body)
            {
                if (body == null) return;
                WebRequest.uploadHandler = new UploadHandlerRaw(body);
                //确定将与出站HTTP请求一起传输的默认Content-Type报头。  
                WebRequest.uploadHandler.contentType = DefaultContentType;
            }
        }

        //响应服务器请求数据
        public class ApiResponseData
        {
            //标识是否完成通信
            public bool IsDone { private set; get; }

            //错误消息
            public string ErrorStrMessage { private set; get; }

            //向服务器请求之后，服务器处理是否有错误
            public bool IsError { private set; get; }

            //错误的Code以便前端识别
            public long Code { private set; get; }

            //服务器传递过来数据的未转换字节数组
            public byte[] RawData { get; private set; }

            //头
            public Dictionary<string, string> Headers { get; private set; }

            public ApiResponseData(UnityWebRequest request)
            {
                IsDone = request.isDone;
                //200为正常
                IsError = request.isNetworkError || (request.responseCode != 200);
                ErrorStrMessage = request.error;
                Code = request.responseCode;
                Headers = request.GetResponseHeaders();
                RawData = request.downloadHandler.data;
                if (Code == 0)
                {
                    Code = -1;
                }

                if (Headers == null)
                {
                    Headers = new Dictionary<string, string>();
                }
            }
        }

        public static void SendRequests(ApiRequestData _apiRequest, Action<ApiResponseData> onFinish)
        {
            // Observable.FromCoroutine<ApiResponseData>(O => SendRequests(O, _apiRequest)).Subscribe(
            //     onFinish);
        }

        private static IEnumerator SendRequests(IObserver<ApiResponseData> observer, ApiRequestData requestData)
        {
            using (var res = requestData.WebRequest)
            {
                var operation = res.SendWebRequest();
                yield return operation;
                observer.OnNext(new ApiResponseData(res));
            }
        }
    }


    public class RequestOperationBase
    {
        public RequestOperationBase(HttpMethods methods, string requestPath)
        {
            Method = methods;
            RequestPath = requestPath;
            Payload = new Dictionary<string, object>();
            QueryParam = new Dictionary<string, string>();
            OptionalHeaders = new Dictionary<string, string>();
            NeedsToMiniJsonParser = true;
        }

        protected HttpMethods Method { get; set; }
        public string RequestPath { get; set; }
        public Dictionary<string, object> Payload { get; set; }

        public Dictionary<string, string> QueryParam { get; set; }

        public Dictionary<string, string> OptionalHeaders { get; set; }
        public Action<HttpResponse, Dictionary<string, object>> _SuccessCallBack { get; set; }
        public Action<HttpResponse, Dictionary<string, object>> _FailureCallBack { get; set; }

        public WebApiRequestHelperHander.ApiRequestData _ApiRequestData { get; set; }

        //要不要转换json
        public bool NeedsToMiniJsonParser { private get; set; }

        //转换请求
        public string PayloadStr { get; set; }
        private const string DefaultContentType = "application/json";

#if USE_BSET_HTTP
#else
        public virtual void Send()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
            // ToUpper-->返回此字符串转换为大写形式的副本
            //60
            var requestData =
                new WebApiRequestHelperHander.ApiRequestData(_uri: GetRequestUri(), method: Method.ToString().ToUpper(),
                    timeOut: 60);
            requestData.AddHeader("Content-Type", DefaultContentType);
            var heardstr = "";
            if (OptionalHeaders.Count != 0)
            {
                foreach (var item in OptionalHeaders)
                {
                    requestData.AddHeader(item.Key, item.Value);
                }
            }
#if OUTPUT_API_JSONS
            var hear = requestData.GetHeader();
            if (hear != null)
            {
                foreach (var item in hear)
                {
                    heardstr += $"{item.Key}： {item.Value}\n";
                }
            }
#endif
            string payloadDebugStr = "";
            if (Method == HttpMethods.Post || Method == HttpMethods.Put)
            {
                //add appirits
                if (!string.IsNullOrEmpty(PayloadStr))
                {
                    UnityEngine.Debug.Log("Payload: " + PayloadStr);
                    requestData.SetBody(Encoding.UTF8.GetBytes(PayloadStr));
                }
                else if (Payload.Count > 0)
                {
                    payloadDebugStr = JsonConvert.SerializeObject(Payload);

                    requestData.SetBody(Encoding.UTF8.GetBytes(payloadDebugStr));
                }
            }
#if OUTPUT_API_JSONS
            Debug.Log($"[HttpsRequest] :{GetRequestUri().ToString()}\n{heardstr}\n{payloadDebugStr}");
#endif
            WebApiRequestHelperHander.SendRequests(requestData, OnRequestFinished);
        }

        public virtual void Send(byte[] bytes, int timeout)
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
            // ToUpper-->返回此字符串转换为大写形式的副本
            //60
            _ApiRequestData =
                new WebApiRequestHelperHander.ApiRequestData(_uri: GetRequestUri(), method: Method.ToString().ToUpper(),
                    timeOut: timeout, bytes);
            _ApiRequestData.AddHeader("Content-Type", DefaultContentType);
            var heardstr = "";
            if (OptionalHeaders.Count != 0)
            {
                foreach (var item in OptionalHeaders)
                {
                    _ApiRequestData.AddHeader(item.Key, item.Value);
                }
            }
#if OUTPUT_API_JSONS
            var hear = requestData.GetHeader();
            if (hear != null)
            {
                foreach (var item in hear)
                {
                    heardstr += $"{item.Key}： {item.Value}\n";
                }
            }
#endif
            string payloadDebugStr = "";
            if (Method == HttpMethods.Post || Method == HttpMethods.Put)
            {
                //add appirits
                if (!string.IsNullOrEmpty(PayloadStr))
                {
                    UnityEngine.Debug.Log("Payload: " + PayloadStr);
                    _ApiRequestData.SetBody(Encoding.UTF8.GetBytes(PayloadStr));
                }
                else if (Payload.Count > 0)
                {
                    payloadDebugStr = JsonConvert.SerializeObject(Payload);

                    _ApiRequestData.SetBody(Encoding.UTF8.GetBytes(payloadDebugStr));
                }
            }
#if OUTPUT_API_JSONS
            Debug.Log($"[HttpsRequest] :{GetRequestUri().ToString()}\n{heardstr}\n{payloadDebugStr}");
#endif
            WebApiRequestHelperHander.SendRequests(_ApiRequestData, OnRequestFinished);
        }

        private void OnRequestFinished(WebApiRequestHelperHander.ApiResponseData data)
        {
            var response = HttpResponse.Create(data);
            Dictionary<string, object> json = null;
            if (NeedsToMiniJsonParser && !string.IsNullOrEmpty(response.DataAssetText))
            {
                json = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.DataAssetText);
            }

            if (data.IsError)
            {
                UnityEngine.Debug.LogError("HTTPRequestStates: Error：" + GetRequestUri());
                UnityEngine.Debug.LogWarning("Request Finished with Error! " + data.ErrorStrMessage);
                if (_FailureCallBack != null)
                {
                    _FailureCallBack(response, json);
                }
            }
            else if (!data.IsDone)
            {
                UnityEngine.Debug.Log("HTTPRequestStates: Aborted");
                if (_FailureCallBack != null)
                {
                    _FailureCallBack(response, json);
                }
            }
            else
            {
                //Debug.Log("HTTPRequestStates: Finished");
                _SuccessCallBack(response, json);
            }
        }
#endif
        protected Uri GetRequestUri()
        {
            if (Method != HttpMethods.Get || QueryParam.Count <= 0)
            {
                return HostManager.Instance.GetUri(RequestPath);
            }

            var paramStr = new StringBuilder(RequestPath);
            bool first = true;
            foreach (var p in QueryParam)
            {
                paramStr.Append(first ? "?" : "&");
                paramStr.Append(p.Key);
                paramStr.Append("=");
                paramStr.Append(p.Value);
                first = false;
            }

            return HostManager.Instance.GetUri(paramStr.ToString());
        }
        // public Action<
    }

    public enum HttpMethods : byte
    {
        Get,
        Put,
        Post,
    }

    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}
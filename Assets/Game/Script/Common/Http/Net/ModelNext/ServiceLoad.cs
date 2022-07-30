using System;
using System.Collections;
using System.IO;
using System.Text;
using GameSystem.ServiceDataPro;
using Newtonsoft.Json;
using Tools.Util;
using UnityEngine;

namespace GameData.Net
{
    //这个读取使用过程中数据
    public class ServiceLoad
    {
        //游戏进入创建
        public void SetURLs(string apiHost, int portNo)
        {
            HostManager.Instance.SetURLs(apiHost, portNo);
        }

        public void SetURLs(string apiHost)
        {
            HostManager.Instance.SetURLs(apiHost);
        }

        #region 不带api模板

        public void ServiceLoadJsonBgThread(string urlName, ServericeBase _SiBase, Action<HttpResponse> _succes,
            Action finish,
            Action<bool> isAbort, byte[] bytes, int timeOut)
        {
            Debug.Assert(_succes != null);
            var onComplte = new Action<HttpResponse>(json =>
            {
                try
                {
                    _succes.Execute(json);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log($"{urlName} not json");
                    throw;
                }
            });
            SendRequests(urlName, _SiBase, res =>
            {
                Util.PerformOnBgThread(() =>
                {
                    var json = res.DataAssetText;
                    onComplte.Execute(res);
                }, finish);
            }, isAbort, bytes, timeOut);
        }


        private void SendRequests<T>(string urlName, ServericeBase _SiBase, Action<HttpResponse> _succes,
            Action<bool> finish)
        {
            RequestOperationBase httpsRequest = new RequestOperationBase(_SiBase.Methods, urlName);
            httpsRequest._SuccessCallBack = (response, _) =>
            {
#if OUTPUT_API_JSONS
                var data = "";
                try
                {
                    data = Encoding.UTF8.GetString(response.RawData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"boby error-->{e}");
                }

#if UNITY_EDITOR
                Debug.Log($"[HttpsResponse] {urlName}\n{data}");
#endif
#endif
                if (_SiBase.NeedsRetry)
                {
                    _succes.Execute(response);
                }
                else
                {
                    _succes.Execute(response);
                }
            };
            httpsRequest._FailureCallBack = (respone, _) =>
            {
                //出现问题时
                InvokeFailureError(urlName, _SiBase, respone, _succes, finish);
            };
            _SiBase.SetupRequestOperation(httpsRequest);
            httpsRequest.Send();
        }

        private void SendRequests(string urlName, ServericeBase _SiBase, Action<HttpResponse> _succes,
            Action<bool> finish, byte[] bytes, int tims)
        {
            var httpsRequest = new RequestOperationBase(_SiBase.Methods, urlName)
            {
                _SuccessCallBack = (response, _) =>
                {
                    var data = "";
                    try
                    {
                        data = Encoding.UTF8.GetString(response.RawData);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"boby error-->{e}");
                    }
#if UNITY_EDITOR
                    Debug.Log($"[HttpsResponse] {urlName}\n{data}");
#endif
                    if (_SiBase.NeedsRetry)
                    {
                        _succes.Execute(response);
                    }
                    else
                    {
                        _succes.Execute(response);
                    }
                }
            };
            httpsRequest._FailureCallBack = (respone, _) =>
            {
                //出现问题时
                InvokeFailureError(urlName, _SiBase, respone, _succes, finish);
                httpsRequest._ApiRequestData.WebRequest.Dispose();
                _succes.Execute(respone);
            };
            _SiBase.SetupRequestOperation(httpsRequest);
            httpsRequest.Send(bytes, tims);
        }

        #endregion

        private static void InvokeFailureError(string urlpath, ServericeBase _SiBase, HttpResponse _https,
            Action<HttpResponse> _succes,
            Action<bool> finish)
        {
            //web api 出错code
            var errorcod = _https?.StatusCode ?? -1;
            //传递过来数据
            var text = _https?.DataAssetText ?? String.Empty;

            var errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(text) && text.StartsWith("{") && text.EndsWith("}") && text.Contains("error"))
            {
                //错误报告
                var errorc = JsonConvert.DeserializeObject<ErrorApi>(text);
                //解析出错误
                errorMessage = errorc.error;
            }

            if (_https != null)
            {
                var appErrorCode = _https.FindFirstHeaderValue("X-App-Error-Code");
                if (appErrorCode.StartsWith("900"))
                {
                }
            }
            //时间


            //客户端请求错误？
            finish.Execute(errorcod == 404);
        }

        [Serializable]
        public class ErrorApi
        {
            public string error = "";
        }
    }
}
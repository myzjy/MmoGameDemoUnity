using System;
using System.Collections;
using System.Collections.Generic;
using GameData.Net;
using GameSystem.ServiceDataPro;
using GameTools.Singletons;
using UnityEngine;
using UnityEngine.Networking;

namespace Common.Http
{
    /// <summary>
    /// http 
    /// </summary>
    public class SimpleHttp : MMOSingletonDontDestroy<SimpleHttp>
    {
        private HttpInfo m_info;

        private UnityWebRequest rootService = null;
        private float mduring = 0f;

        public override void OnAwake()
        {
        }

        void Update()
        {
            if (m_info == null || rootService == null) return;
            mduring += Time.deltaTime;
            if (!(mduring >= m_info.timeOut)) return;
            try
            {
                rootService.Dispose();
                if (m_info.callbackDel != null)
                {
                    m_info.callbackDel(null);
                    m_info.callbackDel = null;
                    m_info = null;
                }
            }
            catch (Exception ex)
            {
                ToolsDebug.LogError("http timeout callback got exception " + ex.Message + "\n" + ex.StackTrace);
            }

            DestroyImmediate(gameObject);
        }

        public static void HttpPost(string url, Dictionary<string, string> formData, byte[] byteData,
            Action<UnityWebRequest> callback, int timeOut = 10)
        {
            HttpInfo httpInfo = new HttpInfo
            {
                callbackDel = callback,
                url = url,
                formData = formData,
                byteData = byteData,
                type = HTTP_TYPE.POST,
                timeOut = timeOut
            };
            SimpleHttp.Instance.StartHttp(httpInfo);
            // rootService.ServicePro.SetGameLoad(url);
            // rootService.ServicePro.RequestSeverPost(url, formData, byteData, callback, timeOut);
        }

        public void StartHttp(HttpInfo info)
        {
            if (info == null) return;
            switch (info.type)
            {
                case HTTP_TYPE.GET:
                    StartCoroutine(DoHttpGet(info));
                    break;
                case HTTP_TYPE.POST:
                    StartCoroutine(DoHttpPost(info));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator DoHttpGet(HttpInfo info)
        {
            //TODO
            yield return null;
        }

        private IEnumerator DoHttpPost(HttpInfo info)
        {
            m_info = info;
            rootService = new UnityWebRequest(m_info.url);
            rootService.url = m_info.url;
            rootService.uploadHandler = (UploadHandler) new UploadHandlerRaw(m_info.byteData);
            if (m_info.formData != null)
            {
                foreach (var VARIABLE in m_info.formData)
                {
                    rootService.SetRequestHeader(VARIABLE.Key, VARIABLE.Value);
                }
            }

            yield return rootService;

            Complete();
        }

        private void Complete()
        {
            try
            {
                if (m_info != null && m_info.callbackDel != null)
                {
                    m_info.callbackDel(rootService);
                    m_info.callbackDel = null;
                }

                m_info = null;
                rootService.Dispose();
            }
            catch (Exception ex)
            {
                ToolsDebug.LogError("http complete callback got exception " + ex.Message + "\n" + ex.StackTrace);
                ToolsDebug.Log("http complete callback got exception " + ex.Message + "\n" + ex.StackTrace);
            }

            DestroyImmediate(gameObject);
        }
    }
}
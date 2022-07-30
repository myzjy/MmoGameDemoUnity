using System;
using System.Collections.Generic;
using GameData.Net;
using UnityEngine;
using Tools.Util;
using UnityEngine.Networking;

namespace GameSystem.ServiceDataPro
{
    public class BaseServiceData
    {
        private ServiceLoad _serviceLoad = null;

        protected ServericeBase CreateGet()
        {
            return ServericeBase.ServericeBaseGet();
        }

        protected ServericeBase CreatePos()
        {
            return ServericeBase.ServericeBasePost();
        }

        public void SetServiceLoad(ServiceLoad serviceLoad)
        {
            _serviceLoad = serviceLoad;
        }

        public void RequestSeverPost(string urlName, Dictionary<string, string> formData,
            byte[] byteData, Action<HttpResponse> call, int timsout)
        {
            Debug.Log($"{urlName}");
            ServericeBase _base = CreatePos();
            _base._querydict = formData;
            _serviceLoad.ServiceLoadJsonBgThread(urlName, _base, call.Execute, () =>
            {
#if UNITY_EDITOR
                ToolsDebug.Log("服务器链接 成功 数据返回成功");
#endif
            }, res => { }, byteData, timsout);
        }
    }
}
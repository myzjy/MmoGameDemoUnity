using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Common.Http
{
    public class HttpInfo
    {
        public string url;
        public HTTP_TYPE type = HTTP_TYPE.GET;
        public Dictionary<string, string> formData;
        public byte[] byteData;
        public float timeOut = 10f;
        public Action<UnityWebRequest> callbackDel;
    }

    public enum HTTP_TYPE
    {
        GET = 0,
        POST,
    }
}
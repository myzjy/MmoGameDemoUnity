using System;

namespace ZJYFrameWork.Net.Dispatcher
{
    public class RequestMapAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public HttpMethods[] method;

        public string[] Params;

        public RequestMapAttribute(HttpMethods[] method, string[] Params)
        {
        }
    }
}
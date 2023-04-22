using System;
using BestHTTP;
using ZJYFrameWork.AttributeCustom;

namespace ZJYFrameWork.WebRequest
{
    [AttributeUsage(AttributeTargets.All)]
    public class RequestMappingAttribute : Attribute
    {
        public RequestMappingAttribute()
        {
        }

        public RequestMappingAttribute(string[] path)
        {
            Path = path;
        }


        public RequestMappingAttribute(string[] path, HttpMethods[] methods)
        {
            Methods = methods;
            Path = path;
        }

        public RequestMappingAttribute(string[] path, HttpMethods[] methods, string[] consumes)
            : this(path, methods)
        {
            Consumes = consumes;
        }

        public RequestMappingAttribute(string[] path, HttpMethods[] methods, string[] consumes, string[] produces)
            : this(path, methods, consumes)
        {
            Produces = produces;
        }

        // [DefaultValue(Value = "")] public string name;
        public HttpMethods[] Methods;
        public string[] Path;
        public string[] Consumes;
        public string[] Produces;
    }
}
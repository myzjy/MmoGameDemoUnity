using System;

namespace ZJYFrameWork.Net
{
    public abstract class ApiHttp<TRequest, TResponse, TError>
        where TRequest : Model
        where TResponse : Model
        where TError : Model, IError
    {
        public bool authorize;
        public bool ignoreError;
        public bool ignoreVerify;

        public Action onBeforeSend;
        public Action onComplete;
        public Action<TError> onError;
        public Action<TResponse> onSuccess;
        public BestHTTP.HttpMethods Method { get; protected set; }
        public string Path { get; protected set; }
        public TRequest Param { get; protected set; }
    }
}
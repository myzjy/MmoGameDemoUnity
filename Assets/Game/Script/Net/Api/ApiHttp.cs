namespace ZJYFrameWork.Net
{
    public abstract class ApiHttp<TRequest, TResponse, TError>
        where TRequest : Model
        where TResponse : Model
        where TError : Model, IError
    {
        
    }
}
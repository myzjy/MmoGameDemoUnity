using ZJYFrameWork.Messaging;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Net
{
    /// <summary>
    /// 登录
    /// </summary>
    public class ApiLoginServerData : ApiHttp<ApiLoginServerDataRequest, ApiLoginServerDataResponse, Error>
    {
        public ApiLoginServerData()
        {
            this.Method = BestHTTP.HttpMethods.Post;
            this.Path = "/api/login";
            this.Param = new ApiLoginServerDataRequest();
            this.authorize = true;
            this.ignoreError = false;
            this.ignoreVerify = false;
            SpringContext.GetBean<Messenger>().Subscribe(onSuccess);
        }
    }
}
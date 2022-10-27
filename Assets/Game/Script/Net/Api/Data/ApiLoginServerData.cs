using System.Text;
using BestHTTP;
using Newtonsoft.Json;

namespace ZJYFrameWork.Net
{
    /// <summary>
    /// 登录
    /// </summary>
    public class ApiLoginServerData : ApiHttp<ApiLoginServerDataRequest, ApiLoginServerDataResponse, Error>
    {
        public ApiLoginServerData()
        {
            this.Method = HTTPMethods.Post;
            this.Path = "/api/login";
            this.Param = new ApiLoginServerDataRequest();
            this.authorize = true;
            this.ignoreError = false;
            this.ignoreVerify = false;
            
        }
    }
}
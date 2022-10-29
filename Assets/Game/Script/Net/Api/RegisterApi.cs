using BestHTTP;

namespace ZJYFrameWork.Net
{
    public class RegisterApi:ApiHttp<RegisterDataModel,RegisterServerCodeData,Error>
    {
        public RegisterApi()
        {
            Method = HTTPMethods.Post;
            Param = new RegisterDataModel();
            Path = "/api/register";
            authorize = true;
            ignoreError = false;
            ignoreVerify = false;
        }
    }
}
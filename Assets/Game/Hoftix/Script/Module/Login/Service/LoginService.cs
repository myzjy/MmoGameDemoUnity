using ZJYFrameWork.Net;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Module.Login.Service
{
    [Bean]
    public class LoginService:ILoginService
    {
        [Autowired]
        private INetManager netManager;
        public void ConnectToGateway()
        {
            throw new System.NotImplementedException();
        }

        public void LoginByToken()
        {
            throw new System.NotImplementedException();
        }

        public void LoginByAccount()
        {
            throw new System.NotImplementedException();
        }

        public void Logout()
        {
            throw new System.NotImplementedException();
        }
    }
}
using ZJYFrameWork.Hotfix.Common;
using ZJYFrameWork.Net;
using ZJYFrameWork.Net.CsProtocol.Protocol.Login;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Module.Register.Service
{
    [Bean]
    public class RegisterService : IRegisterService
    {
        [Autowired] private RegisterPartClientCacheData RegisterCacheData;
        [Autowired] private INetManager netManager;

        /// <summary>
        /// 注册
        /// </summary>
        public void RegisterAccount()
        {
            RegisterCacheData.RegisterError = false;
            netManager.Send(RegisterRequest.ValueOf(
                account: RegisterCacheData.Account,
                password: RegisterCacheData.Password,
                affirmPassword: RegisterCacheData.AffirmPassword));
        }
    }
}
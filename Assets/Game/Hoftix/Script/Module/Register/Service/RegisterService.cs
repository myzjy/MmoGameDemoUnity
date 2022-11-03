using DG.Tweening;
using ZJYFrameWork.Common;
using ZJYFrameWork.Net;
using ZJYFrameWork.Net.CsProtocol;
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
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                RegisterCacheData.RegisterError = false;
                netManager.Send(RegisterRequest.ValueOf(
                    account: RegisterCacheData.Account,
                    password: RegisterCacheData.Password,
                    affirmPassword: RegisterCacheData.AffirmPassword));
            });
            sequence.AppendInterval(3f);
            sequence.AppendCallback(() =>
            {
                if (RegisterCacheData.RegisterFlag)
                {
                    return;
                }

                if (RegisterCacheData.RegisterError)
                {
                    return;
                }

                RegisterAccount();
            });
        }
    }
}
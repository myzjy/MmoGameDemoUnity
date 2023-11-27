using GameUtil;
using ZJYFrameWork.Module.Register.Service;
using ZJYFrameWork.Setting;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.UISerializable.Manager;

namespace ZJYFrameWork.Hotfix.UISerializable
{
    public class UILoginView : UIBaseView<LoginPanelView>
    {
        private RegisterPartView RegisterPartView;
        public override void OnInit()
        {
            viewPanel.LoginPartView.Build();
            RegisterPartView = new RegisterPartView();
            RegisterPartView.Build(viewPanel.RegisterPart_UISerializableKeyObject);
            viewPanel.LoginTapToStartView.Build(this);
            viewPanel.LoginController.Build(loginPartView: viewPanel.LoginPartView,
                registerPartView: RegisterPartView, loginTapToStartView: viewPanel.LoginTapToStartView);
            RegisterPartView.okButton.SetListener(OnClickRegister);
            RegisterPartView.cancelButton.SetListener(RegisterPartView.OnClose);
            
            viewPanel.LoginController.OnInit();
        }

        public override void OnShow()
        {
            viewPanel.LoginController.OnInit();

        }
        private long clickLoginTime;

        private void OnClickRegister()
        {
            if (DateTimeUtil.CurrentTimeMillis() - clickLoginTime < DateTimeUtil.CLICK_INTERVAL)
            {
                return;
            }

            Debug.Log("账号密码注册[account:{}][password:{}][affirmPassword:{}]", RegisterPartView.registerAccountInputField.text,
                RegisterPartView.registerPasswordInputField.text, RegisterPartView.registerAffirmPasswordInputField.text);
            var accountString = RegisterPartView.registerAccountInputField.text;
            var passwordString = RegisterPartView.registerPasswordInputField.text;
            var affirmPasswordString = RegisterPartView.registerPasswordInputField.text;
            SpringContext.GetBean<ServerDataManager>()
                .SetCacheRegisterAccountAndPassword(accountString, passwordString, affirmPasswordString);
#if HTTP_SEND_OPEN
            SpringContext.GetBean<RegisterController>().AtRegisterRequest();
#else
            SpringContext.GetBean<IRegisterService>().RegisterAccount();
#endif
        }
    }
}
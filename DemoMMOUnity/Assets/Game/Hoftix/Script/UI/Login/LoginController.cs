using UnityEngine;
using UnityEngine.Serialization;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Hotfix.UISerializable
{
    public class LoginController : MonoBehaviour
    {
        [FormerlySerializedAs("LoginPartView")]
        public LoginPartView loginPartView = null;

        [FormerlySerializedAs("RegisterPart_RegisterPartView")]
        public RegisterPartView registerPartRegisterPartView = null;

        [FormerlySerializedAs("LoginTapToStartView")]
        public LoginTapToStartView loginTapToStartView = null;
        

        public void Build(LoginPartView loginPartView, RegisterPartView registerPartView,
            LoginTapToStartView loginTapToStartView)
        {
            this.loginPartView = loginPartView;
            this.registerPartRegisterPartView = registerPartView;
            this.loginTapToStartView = loginTapToStartView;
            SpringContext.RegisterBean(this);

        }

        public void OnInit()
        {
            OnHide();
            registerPartRegisterPartView.OnClose();
            loginTapToStartView.Hide();
            loginPartView.Show();
            
        }
        public void OnRegisterStartLogin(){}
        

        public void OnHide()
        {
            loginPartView.Hide();
            registerPartRegisterPartView.OnClose();
            loginTapToStartView.Hide();
        }
    }
}
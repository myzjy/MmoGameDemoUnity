using UnityEngine;
using UnityEngine.Serialization;

namespace ZJYFrameWork.UISerializable
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
        }

        public void OnInit()
        {
            OnHide();
            loginPartView.Show();
        }

        public void OnHide()
        {
            loginPartView.Hide();
            registerPartRegisterPartView.OnClose();
            loginTapToStartView.Hide();
        }
    }
}
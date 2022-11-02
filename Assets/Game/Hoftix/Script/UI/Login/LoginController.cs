﻿using UnityEngine;
using UnityEngine.Serialization;
using ZJYFrameWork.Spring.Core;

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

        public LoginView LoginView = null;

        public void Build(LoginPartView loginPartView, RegisterPartView registerPartView,
            LoginTapToStartView loginTapToStartView, LoginView loginView)
        {
            SpringContext.RegisterBean(this);
            this.loginPartView = loginPartView;
            this.registerPartRegisterPartView = registerPartView;
            this.loginTapToStartView = loginTapToStartView;
            this.LoginView = loginView;
        }

        public void OnInit()
        {
            OnHide();
            LoginView.OnShow();
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
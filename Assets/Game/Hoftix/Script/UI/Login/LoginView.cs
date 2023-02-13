﻿using System.Collections;
using UnityEngine;

namespace ZJYFrameWork.UISerializable
{
    public class LoginView : UIBaseView<LoginPanelView>
    {
        public override void OnInit()
        {
            viewPanel.LoginPartView.Build();
            viewPanel.RegisterPartView.Build();
            viewPanel.LoginTapToStartView.Build(this);
            //build
            viewPanel.LoginController.Build(viewPanel.LoginPartView, viewPanel.RegisterPartView,
                loginTapToStartView: viewPanel.LoginTapToStartView, this);
            viewPanel.LoginController.OnInit();
        }

        /// <summary>
        /// LoginUI tip
        /// </summary>
        public void LoginTip()
        {
            viewPanel.tips.SetActive(true);
            // viewPanel.UserNameText.text=
        }

        private IEnumerator TipActiveMove()
        {
            yield return new WaitForSeconds(2f);
            viewPanel.tips.SetActive(false);
        }

        public override void OnShow()
        {
            // viewPanel.LoginController.
        }
    }
}
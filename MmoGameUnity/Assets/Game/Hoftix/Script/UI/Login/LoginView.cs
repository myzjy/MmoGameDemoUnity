using System.Collections;
using UnityEngine;
using ZJYFrameWork.Execution;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Hotfix.UISerializable
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
            viewPanel.UserNameText.text = $"{viewPanel.LoginController.loginPartView.account.text}";
            Executors.RunOnCoroutine(TipActiveMove());
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
using System.Collections;
using UnityEngine;
using ZJYFrameWork.Execution;
using ZJYFrameWork.UI.UIModel;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.UISerializable.Common;

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
            viewPanel.Gonggao_Button.SetListener(() =>
            {
                CommonController.Instance.snackbar.OpenCommonUIPanel(Dialog.ButtonType.YesNo, "", "点击按钮了",
                    res => { }, "确定", "取消");
            });
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
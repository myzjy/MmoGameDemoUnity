using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Hotfix.UISerializable
{
    public class UILoginView : UIBaseView<LoginPanelView>
    {
        public override void OnInit()
        {
            viewPanel.LoginPartView.Build();
            viewPanel.RegisterPartView.Build();
            viewPanel.LoginTapToStartView.Build(this);
            viewPanel.LoginController.Build(loginPartView: viewPanel.LoginPartView,
                registerPartView: viewPanel.RegisterPartView, loginTapToStartView: viewPanel.LoginTapToStartView);
            viewPanel.LoginController.OnInit();
        }

        public override void OnShow()
        {
            
        }
    }
}
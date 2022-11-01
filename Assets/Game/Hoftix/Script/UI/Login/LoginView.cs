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
                loginTapToStartView: viewPanel.LoginTapToStartView,this);
        }

        public override void OnShow()
        {
            // viewPanel.LoginController.
        }
    }
}
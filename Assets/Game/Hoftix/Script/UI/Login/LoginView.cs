namespace ZJYFrameWork.UISerializable
{
    public class LoginView : UIBaseView<LoginPanelView>
    {
        public override void OnInit()
        {
            viewPanel.LoginPartView.Build();
        }

        public override void OnShow()
        {
        }
    }
}
namespace ZJYFrameWork.UISerializable
{
    public class LoginView : UIBaseView<LoginPanelView>
    {
        public override void OnInit()
        {
            viewPanel.LoginPartView.Build();
            viewPanel.RegisterPart_RegisterPartView.Build();
        }

        public override void OnShow()
        {
        }
    }
}
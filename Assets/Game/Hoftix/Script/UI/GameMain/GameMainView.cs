using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.UI.GameMain
{
    /// <summary>
    /// 主界面
    /// </summary>
    public class GameMainView : UIBaseView<GameMainUIPanelView>
    {
        public override void OnInit()
        {
            OnShow();
        }

        public override void OnShow()
        {
            GetSelfUIView.OnShow();
        }
    }
}
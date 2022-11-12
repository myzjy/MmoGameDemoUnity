using Tools.Util;
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
            viewPanel.pveButton.SetListener(() =>
            {
                //pve
                //打开地图界面
            });
        }

        public override void OnShow()
        {
        }
    }
}
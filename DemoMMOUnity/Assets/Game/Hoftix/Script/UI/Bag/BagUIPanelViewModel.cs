using ZJYFrameWork.Game.Framwork;
using ZJYFrameWork.Hotfix.UI.BagUI;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Hotfix.UISerializable
{
    public partial class UINotifEnum
    {
        /// <summary>
        /// 打开背包界面
        /// </summary>
        public const string OpenBagUiPanel = "OpenBagUiPanel";

        /// <summary>
        /// 关闭背包界面
        /// </summary>
        public const string CLOSE_BAGUI_PAENL = "CLOSE_BAGUI_PAENL";
    }

    /// <summary>
    /// 背包 model 
    /// </summary>
    public class BagUIPanelViewModel : UIBaseModule<BagUIPanelViewView, BagUIPanelView>
    {
        public override string PrefabName()
        {
            return "BagUIPanel";
        }

        public override UICanvasType GetCanvasType()
        {
            return UICanvasType.UI;
        }

        public override string[] Notification()
        {
            return base.Notification();
        }

        public override void NotificationHandler(UINotification _eventNotification)
        {
            base.NotificationHandler(_eventNotification);
        }
    }
}
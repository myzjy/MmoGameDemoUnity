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
            return new[]
            {
                UINotifEnum.OpenBagUiPanel,
                UINotifEnum.CLOSE_BAGUI_PAENL
            };
        }

        public override void NotificationHandler(UINotification _eventNotification)
        {
            switch (_eventNotification.GetEventName)
            {
                case UINotifEnum.OpenBagUiPanel:
                {
                    InstanceOrReuse();
                }
                    break;
                case UINotifEnum.CLOSE_BAGUI_PAENL:
                {
                    if (selfView != null && selfView.GetSelfUIView != null)
                    {
                        selfView.OnHide();
                    }
                }
                    break;
            }
        }
    }
}
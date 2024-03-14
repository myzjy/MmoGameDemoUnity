using ZJYFrameWork.Game.Framwork;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Hotfix.UISerializable
{
    public partial class UINotifEnum
    {
        /// <summary>
        /// 打开 选择关卡 界面
        /// </summary>
        public const string OPEN_OPERATIONAL_INTERFACE_VIEW="OPEN_OPERATIONAL_INTERFACE_VIEW";
        /// <summary>
        /// 关闭 选择关卡 界面
        /// </summary>
        public const string CLOSE_OPERATIONAL_INTERFACE_VIEW="CLOSE_OPERATIONAL_INTERFACE_VIEW";
    }
    [Bean]
    public class UIOperationalInterfacePanelModelView:UIBaseModule<UIOperationalInterfacePanelView,OperationalInterfacePanelView>
    {
        public override string PrefabName()
        {
            return "OperationalInterfacePanel";
        }

        public override UICanvasType GetCanvasType()
        {
            return UICanvasType.ActiviesUI;
        }

        public override UISortType GetSortType()
        {
            return UISortType.First;
        }
        public override string[] Notification()
        {
            return new string[]
            {
                UINotifEnum.OPEN_OPERATIONAL_INTERFACE_VIEW,
                UINotifEnum.CLOSE_OPERATIONAL_INTERFACE_VIEW
            };
        }
        public override void NotificationHandler(UINotification _eventNotification)
        {
            switch (_eventNotification.GetEventName)
            {
                case UINotifEnum.OPEN_OPERATIONAL_INTERFACE_VIEW:
                {
                    InstanceOrReuse();
                }
                    break;
                case UINotifEnum.CLOSE_OPERATIONAL_INTERFACE_VIEW:
                {
                    if (selfView.GetSelfUIView)
                    {
                        selfView.OnHide();
                    }
                }
                    break;
            }
        }

       
    }
}
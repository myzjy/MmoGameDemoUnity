using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Hotfix.UISerializable
{
    [Bean]
    public class UIOperationalInterfaceController
    {
        public UIOperationalInterfacePanelView UIView;

        public void SetUIView(UIOperationalInterfacePanelView UIView)
        {
            this.UIView = UIView;
        }

        public void Open()
        {
            if (UIView == null)
            {
                UIComponentManager.DispatchEvent(UINotifEnum.OPEN_OPERATIONAL_INTERFACE_VIEW);
            }
            else
            {
                UIView.OnShow();
            }
        }
    }
}
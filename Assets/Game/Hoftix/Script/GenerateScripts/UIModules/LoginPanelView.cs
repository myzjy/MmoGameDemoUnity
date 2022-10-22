using ZJYFrameWork.UISerializable.UIInitView;
using UnityEngine;
using UnityEngine.UI;

namespace ZJYFrameWork.UISerializable
{
    public class LoginPanelView:UIViewInterface
    {
        public UnityEngine.GameObject LoginPart=null;
		public ZJYFrameWork.UISerializable.LoginPartView LoginPartView=null;
		


        public void Init(UIView _view)
        {
            LoginPart=_view.GetObjType<UnityEngine.GameObject>("LoginPart");
			LoginPartView=_view.GetObjType<ZJYFrameWork.UISerializable.LoginPartView>("LoginPartView");
			
        }
    }
}
using ZJYFrameWork.UISerializable.UIInitView;
using UnityEngine;
using UnityEngine.UI;

namespace ZJYFrameWork.UISerializable
{
    public class LoginPanelView:UIViewInterface
    {
        public UnityEngine.GameObject LoginPart=null;
		public ZJYFrameWork.UISerializable.LoginPartView LoginPartView=null;
		public ZJYFrameWork.UISerializable.RegisterPartView RegisterPartView=null;
		public ZJYFrameWork.UISerializable.LoginTapToStartView LoginTapToStartView=null;
		public ZJYFrameWork.UISerializable.LoginController LoginController=null;
		


        public void Init(UIView _view)
        {
            LoginPart=_view.GetObjType<UnityEngine.GameObject>("LoginPart");
			LoginPartView=_view.GetObjType<ZJYFrameWork.UISerializable.LoginPartView>("LoginPartView");
			RegisterPartView=_view.GetObjType<ZJYFrameWork.UISerializable.RegisterPartView>("RegisterPartView");
			LoginTapToStartView=_view.GetObjType<ZJYFrameWork.UISerializable.LoginTapToStartView>("LoginTapToStartView");
			LoginController=_view.GetObjType<ZJYFrameWork.UISerializable.LoginController>("LoginController");
			
        }
    }
}
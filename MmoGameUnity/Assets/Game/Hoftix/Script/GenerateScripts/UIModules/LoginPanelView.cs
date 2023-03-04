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
		public ZJYFrameWork.Hotfix.UISerializable.LoginTapToStartView LoginTapToStartView=null;
		public ZJYFrameWork.Hotfix.UISerializable.LoginController LoginController=null;
		public UnityEngine.GameObject tips=null;
		public UnityEngine.UI.Text UserNameText=null;
		public UnityEngine.UI.Button Gonggao_Button=null;
		


        public void Init(UIView _view)
        {
            LoginPart=_view.GetObjType<UnityEngine.GameObject>("LoginPart");
			LoginPartView=_view.GetObjType<ZJYFrameWork.UISerializable.LoginPartView>("LoginPartView");
			RegisterPartView=_view.GetObjType<ZJYFrameWork.UISerializable.RegisterPartView>("RegisterPartView");
			LoginTapToStartView=_view.GetObjType<ZJYFrameWork.Hotfix.UISerializable.LoginTapToStartView>("LoginTapToStartView");
			LoginController=_view.GetObjType<ZJYFrameWork.Hotfix.UISerializable.LoginController>("LoginController");
			tips=_view.GetObjType<UnityEngine.GameObject>("tips");
			UserNameText=_view.GetObjType<UnityEngine.UI.Text>("UserNameText");
			Gonggao_Button=_view.GetObjType<UnityEngine.UI.Button>("Gonggao_Button");
			
        }
    }
}
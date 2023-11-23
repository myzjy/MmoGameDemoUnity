using ZJYFrameWork.UISerializable.UIInitView;
using UnityEngine;
using UnityEngine.UI;

namespace ZJYFrameWork.UISerializable
{
    public class LoginPanelView:UIViewInterface
    {
        public UnityEngine.UI.Image bg_Image=null;
		public UnityEngine.UI.Button Gonggao_Button=null;
		public UnityEngine.GameObject tips=null;
		public UnityEngine.UI.Text UserNameText=null;
		public UnityEngine.GameObject LoginPart=null;
		public ZJYFrameWork.UISerializable.LoginPartView LoginPartView=null;
		public ZJYFrameWork.UISerializable.UISerializableKeyObject RegisterPart_UISerializableKeyObject=null;
		public ZJYFrameWork.Hotfix.UISerializable.LoginTapToStartView LoginTapToStartView=null;
		public ZJYFrameWork.Hotfix.UISerializable.LoginUIController LoginController=null;
		public UnityEngine.AudioSource AudioSource=null;
		


        public void Init(UIView _view)
        {
            bg_Image=_view.GetObjType<UnityEngine.UI.Image>("bg_Image");
			Gonggao_Button=_view.GetObjType<UnityEngine.UI.Button>("Gonggao_Button");
			tips=_view.GetObjType<UnityEngine.GameObject>("tips");
			UserNameText=_view.GetObjType<UnityEngine.UI.Text>("UserNameText");
			LoginPart=_view.GetObjType<UnityEngine.GameObject>("LoginPart");
			LoginPartView=_view.GetObjType<ZJYFrameWork.UISerializable.LoginPartView>("LoginPartView");
			RegisterPart_UISerializableKeyObject=_view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>("RegisterPart_UISerializableKeyObject");
			LoginTapToStartView=_view.GetObjType<ZJYFrameWork.Hotfix.UISerializable.LoginTapToStartView>("LoginTapToStartView");
			LoginController=_view.GetObjType<ZJYFrameWork.Hotfix.UISerializable.LoginUIController>("LoginController");
			AudioSource=_view.GetObjType<UnityEngine.AudioSource>("AudioSource");
			
        }
    }
}
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
		public ZJYFrameWork.UISerializable.UISerializableKeyObject LoginPart_UISerializableKeyObject=null;
		public ZJYFrameWork.UISerializable.UISerializableKeyObject RegisterPart_UISerializableKeyObject=null;
		public UnityEngine.CanvasGroup LoginStart_CanvasGroup=null;
		public ZJYFrameWork.UISerializable.UISerializableKeyObject LoginStart_UISerializableKeyObject=null;
		public ZJYFrameWork.Hotfix.UISerializable.LoginUIController LoginController=null;
		public UnityEngine.AudioSource AudioSource=null;
		


        public void Init(UIView _view)
        {
            bg_Image=_view.GetObjType<UnityEngine.UI.Image>("bg_Image");
			Gonggao_Button=_view.GetObjType<UnityEngine.UI.Button>("Gonggao_Button");
			tips=_view.GetObjType<UnityEngine.GameObject>("tips");
			UserNameText=_view.GetObjType<UnityEngine.UI.Text>("UserNameText");
			LoginPart=_view.GetObjType<UnityEngine.GameObject>("LoginPart");
			LoginPart_UISerializableKeyObject=_view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>("LoginPart_UISerializableKeyObject");
			RegisterPart_UISerializableKeyObject=_view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>("RegisterPart_UISerializableKeyObject");
			LoginStart_CanvasGroup=_view.GetObjType<UnityEngine.CanvasGroup>("LoginStart_CanvasGroup");
			LoginStart_UISerializableKeyObject=_view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>("LoginStart_UISerializableKeyObject");
			LoginController=_view.GetObjType<ZJYFrameWork.Hotfix.UISerializable.LoginUIController>("LoginController");
			AudioSource=_view.GetObjType<UnityEngine.AudioSource>("AudioSource");
			
        }
    }
}
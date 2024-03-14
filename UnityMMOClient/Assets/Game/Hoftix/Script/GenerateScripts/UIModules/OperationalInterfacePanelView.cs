using ZJYFrameWork.UISerializable.UIInitView;
using UnityEngine;
using UnityEngine.UI;

namespace ZJYFrameWork.UISerializable
{
    public class OperationalInterfacePanelView:UIViewInterface
    {
        public UnityEngine.UI.Button ReturnButton=null;
		public UnityEngine.GameObject MapGuideObj=null;
		public ZJYFrameWork.Module.UICommon.AdaptiveReuseGrid Grid=null;
		


        public void Init(UIView _view)
        {
            ReturnButton=_view.GetObjType<UnityEngine.UI.Button>("ReturnButton");
			MapGuideObj=_view.GetObjType<UnityEngine.GameObject>("MapGuideObj");
			Grid=_view.GetObjType<ZJYFrameWork.Module.UICommon.AdaptiveReuseGrid>("Grid");
			
        }
    }
}
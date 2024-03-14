using ZJYFrameWork.UISerializable.UIInitView;
using UnityEngine;
using UnityEngine.UI;

namespace ZJYFrameWork.UISerializable
{
    public class BagUIPanelView:UIViewInterface
    {
        public UnityEngine.GameObject equipmentOtem=null;
		


        public void Init(UIView _view)
        {
            equipmentOtem=_view.GetObjType<UnityEngine.GameObject>("equipmentOtem");
			
        }
    }
}
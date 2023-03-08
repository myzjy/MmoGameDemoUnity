using ZJYFrameWork.UISerializable.UIInitView;
using UnityEngine;
using UnityEngine.UI;

namespace ZJYFrameWork.UISerializable
{
    public class BagUIPanelView:UIViewInterface
    {
        public UnityEngine.GameObject EquipAndItemPanel=null;
		


        public void Init(UIView _view)
        {
            EquipAndItemPanel=_view.GetObjType<UnityEngine.GameObject>("EquipAndItemPanel");
			
        }
    }
}
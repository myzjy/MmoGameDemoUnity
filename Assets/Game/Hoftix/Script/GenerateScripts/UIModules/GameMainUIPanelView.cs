using ZJYFrameWork.UISerializable.UIInitView;
using UnityEngine;
using UnityEngine.UI;

namespace ZJYFrameWork.UISerializable
{
    public class GameMainUIPanelView:UIViewInterface
    {
        public UnityEngine.UI.Button pveButton=null;
		


        public void Init(UIView _view)
        {
            pveButton=_view.GetObjType<UnityEngine.UI.Button>("pveButton");
			
        }
    }
}
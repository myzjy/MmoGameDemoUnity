using ZJYFrameWork.UISerializable.UIInitView;
using UnityEngine;
using UnityEngine.UI;

namespace ZJYFrameWork.UISerializable
{
    public class MapGuide11000View:UIViewInterface
    {
        public UnityEngine.UI.Button Grid01_Button=null;
		


        public void Init(UIView _view)
        {
            Grid01_Button=_view.GetObjType<UnityEngine.UI.Button>("Grid01_Button");
			
        }
    }
}
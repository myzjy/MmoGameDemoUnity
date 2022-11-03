using ZJYFrameWork.UISerializable.UIInitView;
using UnityEngine;
using UnityEngine.UI;

namespace ZJYFrameWork.UISerializable
{
    public class LoadingPanelView:UIViewInterface
    {
        public UnityEngine.UI.Slider leftSlider_Slider=null;
		public UnityEngine.UI.Slider rightSlider_Slider=null;
		


        public void Init(UIView _view)
        {
            leftSlider_Slider=_view.GetObjType<UnityEngine.UI.Slider>("leftSlider_Slider");
			rightSlider_Slider=_view.GetObjType<UnityEngine.UI.Slider>("rightSlider_Slider");
			
        }
    }
}
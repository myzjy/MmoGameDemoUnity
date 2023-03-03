﻿using ZJYFrameWork.UISerializable.UIInitView;
using UnityEngine;
using UnityEngine.UI;
using ZJYFrameWork.Hotfix.UISerializable;

namespace ZJYFrameWork.UISerializable
{
    public class LoadingPanelView:UIViewInterface
    {
        public UnityEngine.UI.Slider leftSlider_Slider=null;
		public UnityEngine.UI.Slider rightSlider_Slider=null;
		public UnityEngine.UI.Text progressNum_Text=null;
		public LoadUIController LoadingController=null;
		


        public void Init(UIView _view)
        {
            leftSlider_Slider=_view.GetObjType<UnityEngine.UI.Slider>("leftSlider_Slider");
			rightSlider_Slider=_view.GetObjType<UnityEngine.UI.Slider>("rightSlider_Slider");
			progressNum_Text=_view.GetObjType<UnityEngine.UI.Text>("progressNum_Text");
			LoadingController=_view.GetObjType<LoadUIController>("LoadingController");
			
        }
    }
}
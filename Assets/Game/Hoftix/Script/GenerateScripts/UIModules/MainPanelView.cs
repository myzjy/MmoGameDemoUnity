using ZJYFrameWork.UISerializable.UIInitView;
using UnityEngine;
using UnityEngine.UI;

namespace ZJYFrameWork.UISerializable
{
    public class MainPanelView:UIViewInterface
    {
        public ZJYFrameWork.UISerializable.UISerializableKeyObject GemsTim_UISerializableKeyObject=null;
		public ZJYFrameWork.UISerializable.UISerializableKeyObject Gem_UISerializableKeyObject=null;
		public ZJYFrameWork.UISerializable.UISerializableKeyObject head_UISerializableKeyObject=null;
		


        public void Init(UIView _view)
        {
            GemsTim_UISerializableKeyObject=_view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>("GemsTim_UISerializableKeyObject");
			Gem_UISerializableKeyObject=_view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>("Gem_UISerializableKeyObject");
			head_UISerializableKeyObject=_view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>("head_UISerializableKeyObject");
			
        }
    }
}
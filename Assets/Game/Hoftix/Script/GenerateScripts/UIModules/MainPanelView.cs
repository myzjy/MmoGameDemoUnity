﻿using ZJYFrameWork.UISerializable.UIInitView;

namespace ZJYFrameWork.UISerializable
{
    public class MainPanelView : UIViewInterface
    {
        public ZJYFrameWork.UISerializable.UISerializableKeyObject downRight_UISerializableKeyObject = null;
        public ZJYFrameWork.UISerializable.UISerializableKeyObject Gem_UISerializableKeyObject = null;
        public ZJYFrameWork.UISerializable.UISerializableKeyObject GemsTim_UISerializableKeyObject = null;
        public ZJYFrameWork.UISerializable.UISerializableKeyObject head_UISerializableKeyObject = null;


        public void Init(UIView _view)
        {
            GemsTim_UISerializableKeyObject =
                _view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>(
                    "GemsTim_UISerializableKeyObject");
            Gem_UISerializableKeyObject =
                _view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>("Gem_UISerializableKeyObject");
            head_UISerializableKeyObject =
                _view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>("head_UISerializableKeyObject");
            downRight_UISerializableKeyObject =
                _view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>(
                    "downRight_UISerializableKeyObject");
        }
    }
}
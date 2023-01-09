using ZJYFrameWork.UISerializable.UIInitView;

namespace ZJYFrameWork.UISerializable
{
    public class GameMainUIPanelView : UIViewInterface
    {
        public UnityEngine.UI.Image beadFrame = null;
        public ZJYFrameWork.UISerializable.UISerializableKeyObject Gem_UISerializableKeyObject = null;
        public ZJYFrameWork.UISerializable.UISerializableKeyObject GemsTim_UISerializableKeyObject = null;
        public UnityEngine.UI.Image headIcon = null;
        public UnityEngine.UI.Button headImgClick = null;
        public UnityEngine.GameObject middle = null;
        public ZJYFrameWork.UISerializable.UISerializableKeyObject middle_UISerializableKeyObject = null;
        public UnityEngine.GameObject top_head = null;
        public UnityEngine.UI.Text top_head_Name_Text = null;


        public void Init(UIView _view)
        {
            GemsTim_UISerializableKeyObject =
                _view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>(
                    "GemsTim_UISerializableKeyObject");
            Gem_UISerializableKeyObject =
                _view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>("Gem_UISerializableKeyObject");
            top_head = _view.GetObjType<UnityEngine.GameObject>("top_head");
            middle = _view.GetObjType<UnityEngine.GameObject>("middle");
            middle_UISerializableKeyObject =
                _view.GetObjType<ZJYFrameWork.UISerializable.UISerializableKeyObject>("middle_UISerializableKeyObject");
            headIcon = _view.GetObjType<UnityEngine.UI.Image>("headIcon");
            beadFrame = _view.GetObjType<UnityEngine.UI.Image>("beadFrame");
            headImgClick = _view.GetObjType<UnityEngine.UI.Button>("headImgClick");
            top_head_Name_Text = _view.GetObjType<UnityEngine.UI.Text>("top_head_Name_Text");
        }
    }
}
using ZJYFrameWork.UISerializable.UIInitView;

namespace ZJYFrameWork.UISerializable
{
    public class BagUIPanelView : UIViewInterface
    {
        public UnityEngine.GameObject EquipAndItemPanel = null;


        public void Init(UIView _view)
        {
            EquipAndItemPanel = _view.GetObjType<UnityEngine.GameObject>("EquipAndItemPanel");
        }
    }
}
using Tools.Util;
using UnityEngine.UI;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.UI.GameMain
{
    /// <summary>
    /// 主界面
    /// </summary>
    public class GameMainView : UIBaseView<GameMainUIPanelView>
    {
        public readonly GameMainMiddleUIDataPanel DataPanel = new GameMainMiddleUIDataPanel();
        public Button GemButton;
        public Text GemsText;
        public Button GemsTimButton;
        public Text GemText;

        public override void OnInit()
        {
            GemsTimButton = viewPanel.GemsTim_UISerializableKeyObject.GetObjType<Button>("click");
            GemsText = viewPanel.GemsTim_UISerializableKeyObject.GetObjType<Text>("numText");

            GemButton = viewPanel.Gem_UISerializableKeyObject.GetObjType<Button>("click");
            GemText = viewPanel.Gem_UISerializableKeyObject.GetObjType<Text>("numText");
            viewPanel.headImgClick.SetListener(() =>
            {
                //点击头像
            });
            DataPanel.Init(viewPanel.middle_UISerializableKeyObject);
            OnShow();
        }

        public override void OnShow()
        {
            GetSelfUIView.OnShow();
        }
    }
}
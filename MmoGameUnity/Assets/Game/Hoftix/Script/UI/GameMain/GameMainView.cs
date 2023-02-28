using UnityEngine.UI;
using ZJYFrameWork.Common;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.UI.GameMain
{
    /// <summary>
    /// 主界面
    /// </summary>
    public class GameMainView : UIBaseView<GameMainUIPanelView>
    {
        public Button GemButton;
        public Text GemsText;
        public Button GemsTimButton;
        public Text GemText;

        public override void OnInit()
        {
            //转换水晶
            GemsTimButton = viewPanel.GemsTim_UISerializableKeyObject.GetObjType<Button>("click");
            //普通水晶 数量显示
            GemsText = viewPanel.GemsTim_UISerializableKeyObject.GetObjType<Text>("numText");
            viewPanel.top_head_Name_Text.text = SpringContext.GetBean<PlayerUserCaCheData>().userName;
            GemButton = viewPanel.Gem_UISerializableKeyObject.GetObjType<Button>("click");
            //付费水晶 数量显示
            GemText = viewPanel.Gem_UISerializableKeyObject.GetObjType<Text>("numText");
            viewPanel.headImgClick.SetListener(() =>
            {
                //点击头像
            });
            //pve 主线
            viewPanel.PVEBtn_Button.SetListener(() => { });
            viewPanel.BagButton.SetListener(() =>
            {
                //背包界面
                // UIComponentManager.DispatchEvent(UINotifEnum.OPEN_GAMEMAIN_PANEL);
            });
            // SpringContext.GetBean<>()
            viewPanel.GMUIController.Build();
            OnShow();
            SpringContext.GetBean<GameMainUIController>().SetGameMainView(this);
        }

        public override void OnShow()
        {
            GetSelfUIView.OnShow();
        }
    }
}
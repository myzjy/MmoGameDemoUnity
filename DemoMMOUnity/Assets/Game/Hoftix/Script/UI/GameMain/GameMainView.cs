using UnityEngine.UI;
using ZJYFrameWork.Common;
using ZJYFrameWork.Hotfix.Common;
using ZJYFrameWork.Hotfix.UISerializable;
using ZJYFrameWork.Messaging;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Hotfix.UI.GameMain
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

        //显示金币
        public Text GoldCoinText;
        public Button GoldTimButton;

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
            GoldCoinText = viewPanel.glod_UISerializableKeyObject.GetObjType<Text>("numText");
            GoldTimButton = viewPanel.glod_UISerializableKeyObject.GetObjType<Button>("click");
            SpringContext.GetBean<Messenger>().Subscribe("ui.gold", (string res) =>
            {
                string st = res;
            });


            viewPanel.headImgClick.SetListener(() =>
            {
                //点击头像
            });
            //pve 主线
            viewPanel.middle_Right_PVEBtn_Button.SetListener(() => { });
            viewPanel.BagButton.SetListener(() =>
            {
                //背包界面
                UIComponentManager.DispatchEvent(UINotifEnum.OpenBagUiPanel);
            });
            GoldTimButton.SetListener(() =>
            {
                //兑换金币
            });
            // SpringContext.GetBean<>()
            viewPanel.GMUIController.Build();
            OnShow();
            SpringContext.GetBean<GameMainUIController>().SetGameMainView(this);
        }

        /// <summary>
        /// 设置普通水晶 数量
        /// </summary>
        /// <param name="num"></param>
        public void SetGemsTim(int num)
        {
            GemsText.text = num.ToString();
        }

        /// <summary>
        /// 设置付费水晶
        /// </summary>
        /// <param name="gemNum"></param>
        public void SetGemTextShow(int gemNum)
        {
            GemText.text = gemNum.ToString();
        }

        /// <summary>
        /// 设置金币数量显示
        /// </summary>
        /// <param name="goldCoinNum"></param>
        public void SetGoldCoinShow(int goldCoinNum)
        {
            GoldCoinText.text = goldCoinNum.ToString();
        }

        public override void OnShow()
        {
            GetSelfUIView.OnShow();
        }
    }
}
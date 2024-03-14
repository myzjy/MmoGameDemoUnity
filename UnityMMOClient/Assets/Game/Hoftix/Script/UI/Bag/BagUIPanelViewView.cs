using System;
using GameUtil;
using UnityEngine;
using UnityEngine.UI;
using ZJYFrameWork.Hotfix.Script.Module.Bag.Service;
using ZJYFrameWork.Hotfix.UISerializable;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Hotfix.UI.BagUI
{
    /// <summary>
    /// 背包打开的type 根据type处理对于功能代码
    /// </summary>
    public enum OpenBagType
    {
        /// <summary>
        /// 装备
        /// </summary>
        Equip,
    }

    /// <summary>
    /// 背包界面脚本
    /// </summary>
    public class BagUIPanelViewView : UIBaseView<BagUIPanelView>
    {
        /// <summary>
        /// 上一次打开的界面
        /// </summary>
        private OpenBagType lastOpenBagType = OpenBagType.Equip;

        public override void OnInit()
        {
            OnShow();
        }

        public override void OnShow()
        {
            CloseUIPanel();
            switch (lastOpenBagType)
            {
                case OpenBagType.Equip:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            SpringContext.GetBean<IBagServerService>().GetBagServerData(lastOpenBagType);
        }


        private void CloseUIPanel()
        {
            
        }

        public override void OnHide()
        {
            base.OnHide();
            CloseUIPanel();
        }

    

    }
}
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
            InitEquipPanel();
            OnShow();
        }

        public override void OnShow()
        {
            CloseUIPanel();
            switch (lastOpenBagType)
            {
                case OpenBagType.Equip:
                    EquipBagUIPanel();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            SpringContext.GetBean<IBagServerService>().GetBagServerData(lastOpenBagType);
        }


        private void CloseUIPanel()
        {
            EquipPanel.SetActive(false);
        }

        public override void OnHide()
        {
            base.OnHide();
            CloseUIPanel();
        }

        #region 背包界面 基础相关

        /// <summary>
        /// 背包关闭 按钮
        /// </summary>
        public Button bagCloseButton;

        public void InitBag()
        {
            var skObject = viewPanel.EquipAndItemPanel.GetComponent<UISerializableKeyObject>();
            bagCloseButton = skObject.GetObjType<Button>("CloseBtn");
            bagCloseButton.SetListener(OnHide);
        }

        #endregion

        #region 装备

        /// <summary>
        /// 装备界面
        /// </summary>
        private GameObject EquipPanel;

        /// <summary>
        /// 背包道具 item
        /// </summary>
        public UISerializableKeyObject ItemKeyObject;

        private void InitEquipPanel()
        {
            var skObject = viewPanel.EquipAndItemPanel.GetComponent<UISerializableKeyObject>();
            EquipPanel = skObject.GetObjType<GameObject>("EquipPanel");
            var ItemObj = skObject.GetObjType<GameObject>("ItemObjTr");
            ItemKeyObject = ItemObj.GetComponent<UISerializableKeyObject>();
        }

        /// <summary>
        /// 装备界面
        /// </summary>
        public void EquipBagUIPanel()
        {
            EquipPanel.SetActive(true);
            var skObject = viewPanel.EquipAndItemPanel.GetComponent<UISerializableKeyObject>();
            //生成装备 父物体
            var EquipContent = skObject.GetObjType<GameObject>("EquipContent");
        }

        #endregion
    }
}
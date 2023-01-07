﻿using UnityEngine;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.UI.GameMain
{
    public class GameMainUIController : MonoBehaviour
    {
        /// <summary>
        /// 构建
        /// </summary>
        public void Build()
        {
            //进行注册
            SpringContext.RegisterBean(this);
        }

        public void OnShow()
        {
            UIComponentManager.DispatchEvent(UINotifEnum.OPEN_GAMEMAIN_PANEL);
        }

        public void OnHide()
        {
            UIComponentManager.DispatchEvent(UINotifEnum.CLOSE_GAMEMAIN_PANEL);
        }
    }
}
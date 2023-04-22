using System;
using UnityEngine;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.Hotfix.UISerializable;

namespace ZJYFrameWork.Hotfix.UI.GameMain
{
    public class GameMainUIController : MonoBehaviour
    {
        /// <summary>
        /// GameView
        /// </summary>
        private GameMainView _view;

        /// <summary>
        /// 构建
        /// </summary>
        public void Build()
        {
            //进行注册
            SpringContext.RegisterBean(this);
        }

        public void SetGameMainView(GameMainView view)
        {
            //这样可以关联起来
            _view = view;
        }

        public void OnShow()
        {
            //UIComponentManager.DispatchEvent(UINotifEnum.OPEN_GAMEMAIN_PANEL);
        }

        public void OnHide()
        {
            //UIComponentManager.DispatchEvent(UINotifEnum.CLOSE_GAMEMAIN_PANEL);
        }

        public void OnDestroy()
        {
            //当物体被删除的时候，需要把自己移除
            SpringContext.UnBean(this);
        }
        
    }
}
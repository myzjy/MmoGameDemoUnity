using UnityEngine;
using ZJYFrameWork.Hotfix.UI.GameMain;
using ZJYFrameWork.Hotfix.UISerializable;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Hotfix.UI.BagUI
{
    /// <summary>
    /// BagUIView 在build 方法注册 bean
    /// </summary>
    public class BagUIViewController : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        private GameMainView _view;

        /// <summary>
        /// build
        /// </summary>
        public void Build()
        {
            SpringContext.RegisterBean(this);
        }

        public void SetGameView(GameMainView mainView)
        {
            this._view = mainView;
        }

        public void OnEnter()
        {
            //打开界面
            UIComponentManager.DispatchEvent(UINotifEnum.OPEN_GAMEMAIN_PANEL);
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void OnLeave()
        {
            UIComponentManager.DispatchEvent(UINotifEnum.CLOSE_GAMEMAIN_PANEL);
        }
    }
}
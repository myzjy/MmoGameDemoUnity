using System;
using UnityEngine;
using ZJYFrameWork.Hotfix.UI.Common;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.Hotfix.UISerializable;
using ZJYFrameWork.UISerializable.Manager;

namespace ZJYFrameWork.Hotfix.UI.GameMain
{
    [Bean]
    public class GameMainUIController : UIViewObject
    {
        /// <summary>
        /// GameView
        /// </summary>
        private GameMainView _view;


        public void SetGameMainView(GameMainView view)
        {
            //这样可以关联起来
            _view = view;
        }

        public void OnShow()
        {
            if (_view == null)
            {
                if (SpringContext.GetBean<GameMainModelView>().GetUIView() == null)
                {
                    SpringContext.GetBean<GameMainModelView>().InstanceOrReuse();
                }
            }
            else
            {
                _view.OnShow();
            }
        }

        public void ShowNowTime(long timeLong)
        {
            var time = DateTimeUtil.GetCurrEntTimeMilliseconds(timeLong);
            if (_view != null)
            {
                _view.ShowNowTime(time);
            }
        }

        public void OnHide()
        {
            _view.OnHide();
        }
    }
}
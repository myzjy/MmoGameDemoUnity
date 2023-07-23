using System;
using UnityEngine;
using ZJYFrameWork.Hotfix.Common;
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

        /// <summary>
        /// 设置体力 显示
        /// </summary>
        /// <param name="nowPhysicalPower">当前体力</param>
        /// <param name="maxNowPhysicalPower">最大体力</param>
        public void SetPhysicalPowerText(int nowPhysicalPower, int maxNowPhysicalPower)
        {
            if (_view == null)
            {
                return;
            }
            _view.SetPhysicalPowerText(nowPhysicalPower,maxNowPhysicalPower);
        }

        public void ShowGameMainUserInfoMessage(LoginClientCacheData cacheData)
        {
            if (_view != null)
            {
                /* *
                 * 设置 金币 砖石
                 */
                _view.SetGemsTim(SpringContext.GetBean<PlayerUserCaCheData>().DiamondNum);
                _view.SetGoldCoinShow(SpringContext.GetBean<PlayerUserCaCheData>().goldNum);
                _view.SetGemTextShow(SpringContext.GetBean<PlayerUserCaCheData>().PremiumDiamondNum);
                //刷新
                _view.SetLvAndExpShow(cacheData.GetLv(),cacheData.GetExp(),cacheData.GetMaxExp());
                _view.SetShowName(SpringContext.GetBean<PlayerUserCaCheData>().userName);
            }
        }

        public void OnHide()
        {
            _view.OnHide();
        }
    }
}
using System;
using ZJYFrameWork.Game.Framwork;
using ZJYFrameWork.Hotfix.UI.GameMain;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.Hotfix.UISerializable
{
    public partial class UINotifEnum
    {
        /// <summary>
        /// 打开主界面面板
        /// </summary>
        public const string OPEN_GAMEMAIN_PANEL = "OPEN_GAMEMAIN_PANEL";

        /// <summary>
        /// 关闭主界面面板
        /// </summary>
        public const string CLOSE_GAMEMAIN_PANEL = "CLOSE_GAMEMAIN_PANEL";

        /// <summary>
        /// Time时间
        /// </summary>
        public const string TIME_GAMEMAIN_PANEL = "TIME_GAMEMAIN_PANEL";
    }


    /// <summary>
    /// GameMain Model
    /// </summary>
    [Bean]
    public class GameMainModelView : UIBaseModule<GameMainView, GameMainUIPanelView>
    {
        /// <summary>
        /// UI预制体名字
        /// </summary>
        /// <returns></returns>
        public override string PrefabName()
        {
            return "GameMainUIPanel";
        }

        /// <summary>
        /// 创建在那个面板上面
        /// </summary>
        /// <returns></returns>
        public override UICanvasType GetCanvasType()
        {
            return UICanvasType.UI;
        }

        public override UISortType GetSortType()
        {
            return UISortType.Last;
        }

        public override string[] Notification()
        {
            return new[]
            {
                UINotifEnum.OPEN_GAMEMAIN_PANEL,
                UINotifEnum.CLOSE_GAMEMAIN_PANEL,
                UINotifEnum.TIME_GAMEMAIN_PANEL,
            };
        }

        public override void NotificationHandler(UINotification _eventNotification)
        {
            switch (_eventNotification.GetEventName)
            {
                case UINotifEnum.OPEN_GAMEMAIN_PANEL:
                {
                    InstanceOrReuse();
                }
                    break;
                case UINotifEnum.CLOSE_GAMEMAIN_PANEL:
                {
                    selfView.OnHide();
                }
                    break;
                case UINotifEnum.TIME_GAMEMAIN_PANEL:
                {
                    if (selfView != null)
                    {
                        if (selfView.GetSelfUIView != null)
                        {
                            var date = (DateTime)_eventNotification.GetEventBody;
                            selfView.ShowNowTime(date);
                        }
                    }
                }
                    break;
            }
        }
    }
}
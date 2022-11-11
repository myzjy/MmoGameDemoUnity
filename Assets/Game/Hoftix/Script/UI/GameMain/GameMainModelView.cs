using ZJYFrameWork.Game.Framwork;
using ZJYFrameWork.UI.GameMain;
using ZJYFrameWork.UISerializable;

namespace ZJYFrameWork.UISerializable
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
    }


    /// <summary>
    /// GameMain Model
    /// </summary>
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

        public override string[] Notification()
        {
            return new[]
            {
                UINotifEnum.OPEN_GAMEMAIN_PANEL,
                UINotifEnum.CLOSE_GAMEMAIN_PANEL
            };
        }

        public override void NotificationHandler(UINotification _eventNotification)
        {
            switch (_eventNotification.GetEventName)
            {
                case UINotifEnum.OPEN_GAMEMAIN_PANEL:
                {
                    
                } break;
                case UINotifEnum.CLOSE_GAMEMAIN_PANEL:
                {
                    
                }
                    break;
            }
        }
    }
}
namespace ZJYFrameWork.Game.Framwork
{
    /// <summary>
    /// UI层级
    /// </summary>
    public enum UICanvasType : int
    {
        None,
        BG,
        UI,
        LOADING,
        TOP,
        NOTICE,
        ActiviesUI,
        // ActiviesN,
    }

    public enum UISortType : int
    {
        /// <summary>
        /// 将变换移动到本地变换列表的开头
        /// </summary>
        First,

        /// <summary>
        ///将变换移动到本地变换列表的末尾
        /// </summary>
        Last
    }
}

namespace ZJYFrameWork.Hotfix.UISerializable
{
    public partial class UINotifEnum
    {
        #region GameView

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

        #endregion
    }
}
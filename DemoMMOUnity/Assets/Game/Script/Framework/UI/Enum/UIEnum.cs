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
    }
}
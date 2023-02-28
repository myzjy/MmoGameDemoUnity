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
        First,
        Last
    }
}

namespace ZJYFrameWork.UISerializable
{
    public partial class UINotifEnum
    {
    }
}
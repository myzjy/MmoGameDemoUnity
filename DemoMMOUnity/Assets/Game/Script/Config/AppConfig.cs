using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.UISerializable.Framwork.UIRootCS;

namespace ZJYFrameWork.Config
{
    public static class AppConfig
    {
        /// <summary>
        /// XLua的ab名
        /// </summary>
        public const string XLuaAssetBundleName = "xlua";

        public const string AssetsGameLuaPath = "Assets/Game/Lua/";
        public const string GameLuaPath = "Game/Lua/";
        public static UIRoot GetRoot => SpringContext.GetBean<UIComponent>().GetRoot;
    }
}
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.UISerializable.Framwork.UIRootCS;

namespace ZJYFrameWork.Config
{
    public struct ConfigurationDevice
    {
        /// <summary>
        /// 平台
        /// </summary>
        public string platform;

        /// <summary>
        /// 服务器宏 例如 API_DEMO
        /// </summary>
        public string serverMacro;
    }

    public static class AppConfig
    {
        /// <summary>
        /// XLua的ab名
        /// </summary>
        public const string XLuaAssetBundleName = "xlua";

        public const string AssetsGameLuaPath = "Assets/Game/Lua/";
        public const string GameLuaPath = "Game/Lua/";
        public static UIRoot GetRoot => SpringContext.GetBean<UIComponent>().GetRoot;

        public static ConfigurationDevice configurationDevice
        {
            get
            {
                var data = new ConfigurationDevice();
#if UNITY_STANDALONE_WIN
                //Windows pc平台
                data.platform = "Windows";
#elif UNITY_IOS
                //Ios 平台
                data.platform = "Ios";
#elif UNITY_ANDROID
                //安卓
                data.platform = "Android";
#else
                //编辑器平台
                data.platform = "Editor";
#endif
#if API_DEV
                data.serverMacro = "Dev";
#elif API_TEST
                data.serverMacro = "Test";
#elif API_ONLINE
                data.serverMacro = "Online";
#endif

                return data;
            }
        }
    }
}
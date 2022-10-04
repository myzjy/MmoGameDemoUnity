using System;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.WebRequest;

namespace ZJYFrameWork.Setting
{
    /// <summary>
    /// 游戏配置管理器
    /// </summary>
    [Bean]
    public sealed class SettingManager : ISettingManager
    {
        /// <summary>
        /// 服务器设置配置相关
        /// </summary>
        [Autowired] private IServerSettings Settings;

        /// <summary>
        /// 初始化
        /// </summary>
        [BeforePostConstruct]
        private void Init()
        {
            if (Settings == null)
            {
                throw new Exception("ServerSetting helper is invalid.");
            }

            Debug.Log(Settings);
            //读取
            Settings.Load();
        }

        public string GetHttpsBase()
        {
            if (Settings == null)
            {
                throw new Exception("ServerSetting helper is invalid.");
            }

            Debug.Log(Settings.ApiHttpsBaseUrl);
            return Settings.ApiHttpsBaseUrl;
        }

        public string GetWebSocketBase()
        {
            if (Settings == null)
            {
                throw new Exception("ServerSetting helper is invalid.");
            }

            return Settings.ApiWebSocketUrl;
        }

        public HostType GetSelectHostType()
        {
            if (Settings == null)
            {
                throw new Exception("ServerSetting helper is invalid.");
            }

            return Settings.Host;
        }
    }
}
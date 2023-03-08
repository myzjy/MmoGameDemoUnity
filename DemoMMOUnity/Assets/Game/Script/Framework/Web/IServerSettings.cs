using Net.Https;

namespace ZJYFrameWork.WebRequest
{
    /// <summary>
    /// 服务器URL配置相关
    /// </summary>
    public interface IServerSettings
    {
        /// <summary>
        /// 基础Http
        /// </summary>
        string ApiHttpsBaseUrl { get; set; }

        /// <summary>
        /// socketUrl
        /// </summary>
        string ApiWebSocketUrl { get; set; }
        string AssetBundleUrl { get; }

        /// <summary>
        /// 当前服务器
        /// </summary>
        HostType Host { get; }

        /// <summary>
        /// 设置当前服务器的一些配置相关
        /// </summary>
        /// <param name="type"></param>
        void SetHost(HostType type);

        /// <summary>
        /// 读取
        /// </summary>
        void Load();
    }
}
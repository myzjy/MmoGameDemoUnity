using ZJYFrameWork.WebRequest;

namespace ZJYFrameWork.Setting
{
    public interface ISettingManager
    {
        /// <summary>
        /// 基础https的url
        /// </summary>
        /// <returns></returns>
        string GetHttpsBase();

        /// <summary>
        /// 基础websocket请求url
        /// </summary>
        /// <returns></returns>
        string GetWebSocketBase();

        /// <summary>
        /// AssetBundle Url
        /// </summary>
        /// <returns></returns>
        string GetAssetBundleUrl();

        HostType GetSelectHostType();

    }
}
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.WebRequest
{
    public enum HostType
    {
        Develop,
        Test,
        Online,
        None = 99,
    }

    [Bean]
    public class ServerSettings : IServerSettings
    {
        public virtual string ApiHttpsBaseUrl { get; set; }
        public virtual string ApiWebSocketUrl { get; set; }

#pragma warning disable CS0649
        private HostType _hostType;
#pragma warning restore CS0649

        // ReSharper disable once ConvertToAutoProperty
        public string AssetBundleUrl
        {
            get => mAssetBundle;
        }

        private string mAssetBundle;
        public HostType Host => _hostType;

        public void SetHost(HostType type)
        {
            _hostType = type;
            switch (_hostType)
            {
                case HostType.Develop:
                    break;
                case HostType.Test:
                    ApiHttpsBaseUrl = "http://192.168.0.114:5000";
                    ApiWebSocketUrl = "ws://192.168.0.114:5000/websocket";
                    mAssetBundle = "http://192.168.0.114:5000/assetbundle/android";
                    break;
                case HostType.Online:
                    break;
            }
        }

        public void Load()
        {

        }

        private HostType GetHostBySysmol()
        {
            HostType type = HostType.Develop;
#if API_DEV
			type = HostType.Develop;
#elif API_TEST
			type = HostType.Test;
#elif API_ONLINE
			type = HostType.Online;
#endif
            return type;
        }
    }
}
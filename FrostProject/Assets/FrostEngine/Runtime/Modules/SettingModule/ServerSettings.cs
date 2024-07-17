namespace FrostEngine
{
    public enum HostType
    {
        Develop,
        Test,
        Online,
        None = 99,
    }

    internal sealed class ServerSettings : ModuleImp, IServerSettings
    {
#pragma warning disable CS0649
        private HostType _hostType;
#pragma warning restore CS0649

        private string mAssetBundle;
        public string ApiHttpsBaseUrl { get; set; }
        public string ApiWebSocketUrl { get; set; }

        // ReSharper disable once ConvertToAutoProperty
        public string AssetBundleUrl
        {
            get => mAssetBundle;
        }

        public HostType Host => _hostType;

        public void SetHost(HostType type)
        {
            _hostType = type;
            switch (_hostType)
            {
                case HostType.Develop:
                    break;
                case HostType.Test:
                    ApiHttpsBaseUrl = "http://127.0.0.1:8000";
                    // ApiWebSocketUrl = "ws://192.168.52.109:15000/websocket";
                    // ApiWebSocketUrl = "ws://192.168.0.113:15000/websocket";
                    // ApiWebSocketUrl = "ws://172.27.48.1:15000/websocket";
                    // ApiWebSocketUrl = "ws://172.17.208.1:15000/websocket";
                    ApiWebSocketUrl = "ws://172.20.10.5:15000/websocket";

                    // ApiWebSocketUrl = "ws://192.168.1.123:15000/websocket";
                    //  ApiWebSocketUrl = "ws://192.168.1.38:15000/websocket";
                    // ApiWebSocketUrl = "ws://127.0.0.1:10200/";

                    mAssetBundle = "http://192.168.0.114:5000/assetbundle/android";
                    break;
                case HostType.Online:
                    ApiHttpsBaseUrl = "http://127.0.0.1:443";
                    ApiWebSocketUrl = "ws://121.41.54.199:15000/websocket";

                    break;
            }
        }

        public void Load()
        {
#if UNITY_EDITOR
            HostType type = (HostType)UnityEditor.EditorPrefs.GetInt("FrostEngine/Tools/Skip Server Select/", (int)HostType.Test);
            if (type == HostType.None)
            {
                type = HostType.Test;
            }
#else
			HostType type = GetHostBySysmol();
#endif
            SetHost(type);
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

        internal override void Shutdown()
        {
        }
    }
}
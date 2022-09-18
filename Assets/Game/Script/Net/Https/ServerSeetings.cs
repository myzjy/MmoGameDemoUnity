namespace Net.Https
{
    public class ServerSettings
    {
        public enum HostType
        {
            Develop,
            Test,
            Online,
            None = 99,
        }


        public string ApiHttpsBaseUrl { get; private set; }
        public string ApiWebSocketUrl { get; private set; }

#pragma warning disable CS0649
        private HostType _hostType;
#pragma warning restore CS0649

        // ReSharper disable once ConvertToAutoProperty
        public HostType Host => _hostType;

        public ServerSettings()
        {
#if UNITY_EDITOR
            HostType type =
                (HostType)UnityEditor.EditorPrefs.GetInt("MmoGame/Tools/Skip Server Select/", (int)HostType.Develop);
            if (type == HostType.None)
            {
                type = HostType.Develop;
            }
#else
			HostType type = GetHostBySysmol();
#endif
            SetHost(type);
        }

        public void SetHost(HostType type)
        {
            _hostType = type;
            switch (_hostType)
            {
                case HostType.Develop:
                    ApiHttpsBaseUrl = "http://fz-game-ttl-dev.focusgames.cn";
                    break;
                case HostType.Test:
                    ApiHttpsBaseUrl = "http://192.168.0.105:5000";
                    break;
                case HostType.Online:
                    ApiHttpsBaseUrl = "http://fz-game-ttl-online.focusgames.cn";
                    break;
            }
#if UNITY_ANDROID
            //AssetBaseUri = AssetBaseUri.Replace("https", "http");
#endif
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
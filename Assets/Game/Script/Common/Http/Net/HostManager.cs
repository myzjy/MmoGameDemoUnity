using System;

namespace GameSystem.ServiceDataPro
{
    public class HostManager
    {
        private static HostManager _i;
        public Uri baseUri { get; private set; }

        private HostManager()
        {
            baseUri = new Uri("Https://localhost:443");
        }
        public static HostManager Instance {
            get {
                if (_i == null) {
                    _i = new HostManager();
                }
                return _i;
            }
        }
        public void SetURLs(string host, int port)
        {
            string protocol = port == 443 ? "https://" : "http://";
            baseUri = new Uri(protocol + host + ":" + port.ToString());
        }
        public void SetURLs(string urlString)
        {
            baseUri = new Uri(urlString);
        }
        public Uri GetUri(string relativePath)
        {
            return new Uri(baseUri, relativePath);
        }
    }
}
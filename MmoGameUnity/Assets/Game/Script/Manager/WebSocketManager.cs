using System;
using System.Text;
using BestHTTP.WebSocket;
using GameTools.Singletons;
using Newtonsoft.Json;

namespace ZJYFrameWork.UISerializable.Manager
{
    public class LoginDataServer
    {
        public int protocolId { get; set; }
        public LoginServerDataSend packet { get; set; }
    }

    public class LoginServerDataSend
    {
        public string account { get; set; }
        public string password { get; set; }
    }

    /// <summary>
    /// WebSocket管理类
    /// </summary>
    public class WebSocketManager : MMOSingletonDontDestroy<WebSocketManager>
    {
        WebSocket webSocket;

        public override void OnAwake()
        {
            base.OnAwake();
            Uri url = new Uri("http://192.168.0.105:5000/websocket");
            webSocket = new WebSocket(url);
#if !UNITY_WEBGL || UNITY_EDITOR
            this.webSocket.StartPingThread = true;

// #if !BESTHTTP_DISABLE_PROXY
//             if (HTTPManager.Proxy != null)
//                 this.webSocket.OnInternalRequestCreated = (ws, internalRequest) =>
//                     internalRequest.Proxy =
//                         new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);
// #endif
#endif

            // Subscribe to the WS events
            this.webSocket.OnOpen += OnOpen;
            this.webSocket.OnMessage += OnMessageReceived;
            this.webSocket.OnBinary += OnMessageReceived;
            this.webSocket.OnClosed += OnClosed;
            this.webSocket.OnError += OnError;
            this.webSocket.Open();
        }

        public void SendButton()
        {
            LoginDataServer data = new LoginDataServer();
            data.protocolId = 1000;
            data.packet = new LoginServerDataSend();
            data.packet.account = "1111";
            data.packet.password = "1111";
            webSocket.Send(JsonConvert.SerializeObject(data));
        }

        public void Send()
        {
        }

        #region WebSocket Event Handlers

        /// <summary>
        /// 是否打开链接了
        /// </summary>
#pragma warning disable CS0414
        private bool _isOpenSocket = false;
#pragma warning restore CS0414
        /// <summary>
        /// 链接是否关闭
        /// </summary>
#pragma warning disable CS0414
        private bool isCloseSocket = false;
#pragma warning restore CS0414
        /// <summary>
        /// 是否退出
        /// </summary>
#pragma warning disable CS0414
        private bool _isOutGame = false;
#pragma warning restore CS0414

        /// <summary>
        /// Called when the web socket is open, and we are ready to send and receive data
        /// </summary>
        void OnOpen(WebSocket ws)
        {
            _isOpenSocket = true;
        }

        /// <summary>
        /// Called when we received a text message from the server
        /// </summary>
        void OnMessageReceived(WebSocket ws, string message)
        {
            Debug.Log(message);
        }

        void OnMessageReceived(WebSocket ws, byte[] message)
        {
            var s = Encoding.UTF8.GetString(message);
            Debug.Log(s);
        }

        /// <summary>
        /// Called when the web socket closed
        /// </summary>
        void OnClosed(WebSocket ws, UInt16 code, string message)
        {
            _isOpenSocket = false;
            webSocket = null;
        }

        /// <summary>
        /// Called when an error occured on client side
        /// </summary>
        void OnError(WebSocket ws, string error)
        {
            Debug.Log($"An error occured: <color=red>{error}</color>");

            webSocket = null;
        }

        public void OnApplicationQuit()
        {
            _isOutGame = true;
            _isOpenSocket = false;
        }

        #endregion
    }
}
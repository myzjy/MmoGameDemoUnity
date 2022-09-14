using System;
using BestHTTP.WebSocket;
using GameTools.Singletons;

namespace ZJYFrameWork.UISerializable.Manager
{
    /// <summary>
    /// WebSocket管理类
    /// </summary>
    public class WebSocketManager : MMOSingletonDontDestroy<WebSocketManager>
    {
        WebSocket webSocket;

        public override void OnAwake()
        {
            base.OnAwake();
            Uri url = new Uri(NetworkManager.Instance.Settings.ApiWebSocketUrl);
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
            this.webSocket.OnClosed += OnClosed;
            this.webSocket.OnError += OnError;
            this.webSocket.Open();
        }

        public void Send()
        {
            // webSocket.Send();
        }

        #region WebSocket Event Handlers

        /// <summary>
        /// 是否打开链接了
        /// </summary>
        private bool isOpenSocket = false;
        /// <summary>
        /// 链接是否关闭
        /// </summary>
        private bool isCloseSocket = false;
        /// <summary>
        /// 是否退出
        /// </summary>
        private bool isOutGame = false;

        /// <summary>
        /// Called when the web socket is open, and we are ready to send and receive data
        /// </summary>
        void OnOpen(WebSocket ws)
        {
            
            isOpenSocket = true;
        }

        /// <summary>
        /// Called when we received a text message from the server
        /// </summary>
        void OnMessageReceived(WebSocket ws, string message)
        {
        }

        /// <summary>
        /// Called when the web socket closed
        /// </summary>
        void OnClosed(WebSocket ws, UInt16 code, string message)
        {
            isOpenSocket = false;
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
            isOutGame = true;
            isOpenSocket = false;
        }

        #endregion
    }
}
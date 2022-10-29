using System;
using AOT;
using BestHTTP.WebSocket;

namespace ZJYFrameWork.Net.Core.Websocket
{
    public class WebSocketBridge
    {
        /// <summary>
        /// webSocket
        /// </summary>
        private WebSocket _webSocket;

        public WebsocketClient websocketClient;

        public bool initialized = false;

        public void WebSocketClose()
        {
            if (_webSocket == null)
            {
                return;
            }

            if (_webSocket.State >= WebSocketStates.Closing)
            {
                Debug.Log($"当前Socket状态{_webSocket.State}");
                return;
            }
        }

        public void WebSocketSend(byte[] dataPtr)
        {
            _webSocket.Send(dataPtr);
        }

        /// <summary>
        /// 当socket打开成功的回调
        /// </summary>
        /// <param name="callback"></param>
        public void WebSocketSetOnOpen(OnWebSocketOpenDelegate callback)
        {
            _webSocket.OnOpen = callback;
        }

        /// <summary>
        /// 当服务器发送json或者text相关内容时回调
        /// </summary>
        /// <param name="callback"></param>
        public void WebSocketSetOnMessage(OnWebSocketMessageDelegate callback)
        {
            _webSocket.OnMessage = callback;
        }

        /// <summary>
        /// 当服务器发送消息为二进制的时候回调
        /// </summary>
        /// <param name="callback"></param>
        public void WebSocketSetOnBinary(OnWebSocketBinaryDelegate callback)
        {
            _webSocket.OnBinary = callback;
        }

        /// <summary>
        /// socket报错时回调
        /// </summary>
        /// <param name="callback"></param>
        public void WebSocketSetOnError(OnWebSocketErrorDelegate callback)
        {
            _webSocket.OnError = callback;
        }

        /// <summary>
        /// socket关闭时回调
        /// </summary>
        /// <param name="callback"></param>
        public void WebSocketSetOnClose(OnWebSocketClosedDelegate callback)
        {
            _webSocket.OnClosed = callback;
        }

        // /// <summary>
        // /// The current state of the transport.
        // /// </summary>
        // public TransportStates State
        // {
        //     get { return _state; }
        //     protected set
        //     {
        //         TransportStates old = _state;
        //         _state = value;
        //     }
        // }
        //
        // public TransportStates _state;

        public void Initialize()
        {
            WebSocketSetOnOpen(DelegateOnOpenEvent);
            WebSocketSetOnMessage(DelegateOnMessageEvent);
            WebSocketSetOnBinary(DelegateOnBinaryEvent);
            WebSocketSetOnError(DelegateOnErrorEvent);
            WebSocketSetOnClose(DelegateOnCloseEvent);
            initialized = true;
        }

        /// <summary>
        /// 开始链接
        /// </summary>
        /// <param name="url"></param>
        public void WebSocketConnect(string url)
        {
            Uri uri = new Uri(url);
            _webSocket = new WebSocket(uri);
            if (!initialized)
            {
                Initialize();
            }

            //打开
            _webSocket.Open();
        }

        public void DelegateOnOpenEvent(WebSocket webSocket)
        {
            websocketClient.HandleOnOpen(webSocket);
        }

        [MonoPInvokeCallback(typeof(OnWebSocketMessageDelegate))]
        public void DelegateOnMessageEvent(WebSocket webSocket, string message)
        {
            try
            {
                if (webSocket != _webSocket)
                {
                    return;
                }

                //
                if (webSocket.IsOpen)
                {
                    websocketClient.HandleOnMessage(message);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
        }

        [MonoPInvokeCallback(typeof(OnWebSocketBinaryDelegate))]
        public void DelegateOnBinaryEvent(WebSocket webSocket, byte[] data)
        {
            try
            {
                //
                if (webSocket.IsOpen)
                {
                    websocketClient.HandleOnMessage(data);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
        }

        [MonoPInvokeCallback(typeof(OnWebSocketErrorDelegate))]
        public void DelegateOnErrorEvent(WebSocket webSocket, string reason)
        {
            websocketClient.HandleOnError(reason);
        }

        [MonoPInvokeCallback(typeof(OnWebSocketClosedDelegate))]
        public void DelegateOnCloseEvent(WebSocket webSocket, ushort code, string message)
        {
            string reason = $"{code} : {message}";

            Debug.Log(reason);
            websocketClient.HandleOnClose(code, message);
        }
    }
}
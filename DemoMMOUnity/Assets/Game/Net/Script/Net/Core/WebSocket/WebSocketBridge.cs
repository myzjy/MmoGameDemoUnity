using System;
using System.Collections.Generic;
using System.Text;
using AOT;
using BestHTTP.WebSocket;
using Newtonsoft.Json;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.Net.CsProtocol.Buffer;
using ZJYFrameWork.Security.Cryptography;
using ZJYFrameWork.Spring.Utils;

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

        private System.Diagnostics.Stopwatch _watch;

        public void WebSocketSend(byte[] dataPtr)
        {
            _watch = new System.Diagnostics.Stopwatch();
            _watch.Start();
            _webSocket.Send(dataPtr);
            LogRequest(_webSocket, dataPtr);
        }

        private void LogRequest(WebSocket webSocket, byte[] bytes)
        {
#if DEVELOP_BUILD || UNITY_EDITOR
            var message = StringUtils.BytesToString(bytes);
            var byte1 = Encoding.Default.GetBytes(message);
            var message1 = Encoding.UTF8.GetString(byte1);
            Debug.Log(
                $"[ApiRequest] {webSocket.InternalRequest.MethodType.ToString().ToUpper()} {webSocket.InternalRequest.Uri.AbsoluteUri}\n" +
                $"{webSocket.InternalRequest.DumpHeaders()}\n\n{message1}\n");
#endif
        }

        private void LogResponse(WebSocket webSocket, string message)
        {
#if DEVELOP_BUILD || UNITY_EDITOR
            //接收到的部分会有Unicode字符,答应出来的时候,需要转换
            var itemStr = JsonConvert.DeserializeObject(message);

            string json = JsonConvert.SerializeObject(itemStr);

            StringBuilder sb = new StringBuilder();
            sb.Append(
                $"[ApiResponse] {webSocket.InternalRequest.MethodType.ToString().ToUpper()} {webSocket.InternalRequest.Uri.AbsoluteUri}\n");
            sb.Append(
                $"{webSocket.InternalRequest.Response.StatusCode} {webSocket.InternalRequest.Response.Message}  ({_watch.ElapsedMilliseconds}ms)"); // foreach (var item in webSocket.Cookies)
            foreach (var item in webSocket.InternalRequest.Response.Headers)
            {
                sb.Append(item.Key).Append(": ");
                var count = item.Value.Count;
                for (var i = 0; i < count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(",");
                    }

                    sb.Append(item.Value[i]);
                }

                sb.Append("\n");
            }

            // Debug.Log($"{webSocket.InternalRequest.Uri.AbsoluteUri}:{message}");
            Debug.Log($"{sb.ToString()}{json}");
#endif
        }

        private void LogResponse(WebSocket webSocket, byte[] data)
        {
#if DEVELOP_BUILD || UNITY_EDITOR
            var message = StringUtils.BytesToString(data);
            var itemStr = JsonConvert.DeserializeObject<Dictionary<object, object>>(message);
            bool isPing = false;
            foreach (var (key,value) in itemStr)
            {
                var keyString = key.ToString();
                if (keyString == "protocolId")
                {
                    var valueString = value.ToString();
                    if (valueString=="104")
                    {
                        isPing = true;
                        break;
                    }
                }
                else
                {
                    continue;
                }
                
            }

            if (isPing)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(
                $"[ApiResponse] {webSocket.InternalRequest.MethodType.ToString().ToUpper()} {webSocket.InternalRequest.Uri.AbsoluteUri}\n");
            sb.Append(
                $"{webSocket.InternalRequest.Response.StatusCode} {webSocket.InternalRequest.Response.Message}  ({_watch.ElapsedMilliseconds}ms)");
            foreach (var item in webSocket.InternalRequest.Response.Headers)
            {
                sb.Append(item.Key).Append(": ");
                var count = item.Value.Count;
                for (var i = 0; i < count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(",");
                    }

                    sb.Append(item.Value[i]);
                }

                sb.Append("\n");
            }

            // Debug.Log($"{webSocket.InternalRequest.Uri.AbsoluteUri}:{json}");
            Debug.Log($"{sb.ToString()}{message}");
#endif
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
            var bytes = ByteBuffer.ValueOf();
            decryptor = CryptographUtil.GetDecryptor(Algorithm.AES128_CTR_NONE, bytes.GetBytes("uslmcG1ep1gSsBcu"),
                bytes.GetBytes("AN9K3kQfITXr7P2Q"));
            _encryptor = CryptographUtil.GetEncryptor(Algorithm.AES128_CTR_NONE, bytes.GetBytes("uslmcG1ep1gSsBcu"),
                bytes.GetBytes("AN9K3kQfITXr7P2Q"));
            initialized = true;
        }

        //解密
        private IStreamDecryptor decryptor;

        //加密
        private IStreamEncryptor _encryptor;

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
                    _watch.Stop();
                    LogResponse(webSocket, message);
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
                    _watch.Stop();
                    LogResponse(webSocket, data);
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
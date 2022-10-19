using System;
using System.Collections.Generic;
using AOT;
using BestHTTP.JSON.LitJson;
using BestHTTP.WebSocket;
using BestHTTP.WebSocket.Frames;
using Newtonsoft.Json;
using UnityEngine;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.Net.Core.Websocket
{
    public class WebSocketBridge
    {
        /// <summary>
        /// webSocket
        /// </summary>
        private WebSocket _webSocket = new WebSocket(null)
        {
            OnOpen = null,
            OnMessage = null,
            OnError = null,
            OnClosed = null
        };

        public WebsocketClient websocketClient;

        public bool initialized = false;

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

        public void Initialize()
        {
            WebSocketSetOnOpen(DelegateOnOpenEvent);
            initialized = true;
        }

        [MonoPInvokeCallback(typeof(OnWebSocketOpenDelegate))]
        public void DelegateOnOpenEvent(WebSocket webSocket)
        {
            websocketClient.HandleOnOpen(webSocket);
        }

        [MonoPInvokeCallback(typeof(OnWebSocketMessageDelegate))]
        public static void DelegateOnMessageEvent(WebSocket webSocket, string message)
        {
            try
            {
                //
                if (webSocket.IsOpen)
                {
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
        }

        [MonoPInvokeCallback(typeof(OnWebSocketBinaryDelegate))]
        public static void DelegateOnBinaryEvent(WebSocket webSocket, byte[] data)
        {
            try
            {
                //
                if (webSocket.IsOpen)
                {
                   
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
        }
    }
}
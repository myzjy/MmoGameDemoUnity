using System;
using BestHTTP.WebSocket;

namespace ZJYFrameWork.Net.Core.Websocket
{
    internal static class WebSocketBridge
    {
        /* Delegates */
        /// <summary>
        /// 打开回调
        /// </summary>
        public delegate void OnOpenCallback();

        /// <summary>
        /// 发送消息回调
        /// </summary>
        public delegate void OnMessageCallback(IntPtr msgPtr, int msgSize);

        /// <summary>
        /// 当出错了的回调
        /// </summary>
        public delegate void OnErrorCallback();

        /// <summary>
        /// 当socket关闭了的回调
        /// </summary>
        public delegate void OnCloseCallback();

        /// <summary>
        /// webSocket
        /// </summary>
        private static WebSocket _webSocket;

        public static void DelegateOnOpenEvent()
        {
        }
    }
}
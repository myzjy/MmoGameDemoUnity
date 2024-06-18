#if !BESTHTTP_DISABLE_WEBSOCKET

#if !UNITY_WEBGL || UNITY_EDITOR
using System;
using System.Diagnostics;
using System.Text;
using BestHTTP.WebSocket.Frames;
#endif

namespace BestHTTP.WebSocket
{
    /// <summary>
    /// 底层实现的状态。
    /// </summary>
    public enum WebSocketStates : byte
    {
        Connecting = 0,
        Open = 1,
        Closing = 2,
        Closed = 3,
        Unknown
    };

    public delegate void OnWebSocketOpenDelegate(WebSocket webSocket);

    public delegate void OnWebSocketMessageDelegate(WebSocket webSocket, string message);

    public delegate void OnWebSocketBinaryDelegate(WebSocket webSocket, byte[] data);

    public delegate void OnWebSocketClosedDelegate(WebSocket webSocket, UInt16 code, string message);

    public delegate void OnWebSocketErrorDelegate(WebSocket webSocket, string reason);

#if !UNITY_WEBGL || UNITY_EDITOR
    public delegate void OnWebSocketIncompleteFrameDelegate(WebSocket webSocket, WebSocketFrameReader frame);
#endif

    public abstract class WebSocketBaseImplementation
    {
        protected WebSocketBaseImplementation(
            WebSocket parent,
            Uri uri,
            string origin,
            string protocol)
        {
#if (UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG) && ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                StringBuilder sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}]");
                sb.Append($"{sf.GetMethod().Name}");
                sb.Append($"Line:{sf.GetFileLineNumber()}");
                sb.Append($"[msg]初始化WebSocketBaseImplementation uri:  {uri.LocalPath}:{uri.Host}");
                sb.Append($"  origin:  {origin}");
                Debug.Log($"{sb}");
            }
#endif
            this.Parent = parent;
            this.Uri = uri;
            this.Origin = origin;
            this.Protocol = protocol;

#if !UNITY_WEBGL || UNITY_EDITOR

            // Set up some default values.
            this.Parent.PingFrequency = 1000;
            this.Parent.CloseAfterNoMessage = TimeSpan.FromSeconds(2);
#endif
        }

        public virtual WebSocketStates State { get; protected set; }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public virtual bool IsOpen { get; set; } = default;

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public virtual int BufferedAmount { get; set; } = default;

        protected WebSocket Parent { get; }
        public Uri Uri { get; protected set; }
        public string Origin { get; }
        public string Protocol { get; }

        public abstract void StartOpen();
        public abstract void StartClose(UInt16 code, string message);

        public abstract void Send(string message);
        public abstract void Send(byte[] buffer);
        public abstract void Send(byte[] buffer, ulong offset, ulong count);

#if !UNITY_WEBGL || UNITY_EDITOR
        public HttpRequest InternalRequest
        {
            get
            {
                if (this.SetInternalRequest == null)
                    CreateInternalRequest();

                return this.SetInternalRequest;
            }
        }

        protected HttpRequest SetInternalRequest;

        public virtual int Latency { get; protected set; }
        public virtual DateTime LastMessageReceived { get; protected set; } = DateTime.MinValue;
#endif

#if !UNITY_WEBGL || UNITY_EDITOR
        protected abstract void CreateInternalRequest();

        /// <summary>
        /// 它将把给定的帧发送到服务器。
        /// </summary>
        public abstract void Send(WebSocketFrame frame);
#endif
    }
}
#endif
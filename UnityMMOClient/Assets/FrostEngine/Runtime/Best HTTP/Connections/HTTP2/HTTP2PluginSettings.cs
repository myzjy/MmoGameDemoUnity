#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2
using System;

// ReSharper disable once CheckNamespace
namespace BestHTTP.Connections.HTTP2
{
    public sealed class WebSocketOverHttp2Settings
    {
        /// <summary>
        /// 将其设置为false以禁用Websocket Over HTTP/2 (RFC 8441)。默认情况下是正确的。
        /// </summary>
        public bool EnableWebSocketOverHttp2 { get; set; } = true;

        /// <summary>
        /// 将其设置为当连接失败时禁用从Websocket Over HTTP/2实现到“旧”HTTP/1实现的回退逻辑.
        /// </summary>
        public bool EnableImplementationFallback { get; set; } = true;
    }

    public sealed class Http2PluginSettings
    {
        /// <summary>
        /// 设置为true启用RFC 8441 "Bootstrapping WebSockets with HTTP/2" (https://tools.ietf.org/html/rfc8441).
        /// </summary>
        public readonly bool EnableConnectProtocol = false;

        /// <summary>
        /// http/2连接的全局窗口大小。它的默认值是31位的最大可能值。
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public readonly UInt32 InitialConnectionWindowSize = Http2Handler.MaxValueFor31Bits; // Spec default: 65535

        /// <summary>
        /// http2流的初始窗口大小。默认值为10mb(10 * 1024 * 1024)。
        /// </summary>
        public readonly UInt32 InitialStreamWindowSize = 10 * 1024 * 1024; // Spec default: 65535

        /// <summary>
        /// http2连接将允许最大并发http2流。默认值为128;
        /// </summary>
        public readonly UInt32 MaxConcurrentStreams = 128; // 规格默认值:未定义

        public readonly WebSocketOverHttp2Settings WebSocketOverHttp2Settings = new WebSocketOverHttp2Settings();

        /// <summary>
        /// HttpPACK报头表的最大大小。
        /// </summary>
        public UInt32 HeaderTableSize = 4096; // Spec default: 4096

        /// <summary>
        /// http2帧的最大大小。
        /// </summary>
        public UInt32 MaxFrameSize = 16384; // 16384 spec def.

        /// <summary>
        /// Not used.
        /// </summary>
        public UInt32 MaxHeaderListSize = UInt32.MaxValue; // Spec default: infinite

        /// <summary>
        /// 在HTTP/2中，只有一个连接是打开的，所以我们可以让它打开得更久，因为我们希望它能被更多地重用.
        /// </summary>
        public TimeSpan MaxIdleTime = TimeSpan.FromSeconds(120);

        /// <summary>
        /// 两次ping消息之间的时间。
        /// </summary>
        public TimeSpan PingFrequency = TimeSpan.FromSeconds(5);

        /// <summary>
        /// 从服务器接收ping确认超时。如果在此期间没有收到ack，连接将被视为断开。
        /// </summary>
        public TimeSpan Timeout = TimeSpan.FromSeconds(10);
    }
}
#endif
#if !BESTHTTP_DISABLE_SOCKETIO

using System.Collections.Generic;

namespace BestHTTP.SocketIO3
{
    /// <summary>
    /// Helper类来解析和保存握手信息。
    /// </summary>
    [PlatformSupport.IL2CPP.Preserve]
    public abstract  class HandshakeData
    {
        /// <summary>
        /// 该连接的会话ID。
        /// </summary>
        [PlatformSupport.IL2CPP.Preserve]
        public string Sid { get; private set; }

        /// <summary>
        /// 可能升级的列表。
        /// </summary>
        [PlatformSupport.IL2CPP.Preserve]
        // ReSharper disable once CollectionNeverUpdated.Global
        public List<string> Upgrades { get; private set; }

        /// <summary>
        /// 我们必须设置一个ping消息的时间间隔。
        /// </summary>
        [PlatformSupport.IL2CPP.Preserve]
        public int PingInterval { get; private set; }

        /// <summary>
        /// 当我们可以认为连接断开时，需要经过多长时间才能得到ping请求的应答。
        /// </summary>
        [PlatformSupport.IL2CPP.Preserve]
        public int PingTimeout { get; private set; }
    }
}

#endif

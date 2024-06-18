#if !BESTHTTP_DISABLE_SOCKETIO

using System.Collections.Generic;

namespace BestHTTP.SocketIO.Transports
{
    public enum TransportTypes
    {
        Polling,

#if !BESTHTTP_DISABLE_WEBSOCKET
        WebSocket
#endif
    }
    
    /// <summary>
    /// ITransport实现的可能状态。
    /// </summary>
    public enum TransportStates : int
    {
        /// <summary>
        /// 传输正在连接到服务器。
        /// </summary>
        Connecting = 0,

        /// <summary>
        /// Transport连接，并开始了开放进程。
        /// </summary>
        Opening = 1,

        /// <summary>
        /// 传输是开放的，可以发送和接收数据包。
        /// </summary>
        Open = 2,

        /// <summary>
        /// 传输通道关闭了。
        /// </summary>
        Closed = 3,

        /// <summary>
        /// 传输暂停。
        /// </summary>
        Paused = 4
    }
    
    /// <summary>
    /// 一个具有套接字的接口。IO传输必须实现。
    /// </summary>
    public interface ITransport
    {
        /// <summary>
        /// 传输的类型。
        /// </summary>
        TransportTypes Type { get; }

        /// <summary>
        /// 传输的当前状态
        /// </summary>
        TransportStates State { get; }

        /// <summary>
        /// 此传输绑定到的SocketManager实例。
        /// </summary>
        SocketManager Manager { get; }

        /// <summary>
        /// 如果传输忙于发送消息，则为true。
        /// </summary>
        bool IsRequestInProgress { get; }

        /// <summary>
        /// 如果传输忙于处理轮询请求，则为true。
        /// </summary>
        bool IsPollingInProgress { get; }

        /// <summary>
        /// 启动打开/升级传输。
        /// </summary>
        void Open();

        /// <summary>
        /// 对服务器上的可用消息进行轮询。
        /// </summary>
        void Poll();

        /// <summary>
        /// 向服务器发送单个数据包。
        /// </summary>
        void Send(Packet packet);

        /// <summary>
        /// 向服务器发送数据包列表。
        /// </summary>
        void Send(List<Packet> packets);

        /// <summary>
        /// 关闭这个传输。
        /// </summary>
        void Close();
    }
}

#endif
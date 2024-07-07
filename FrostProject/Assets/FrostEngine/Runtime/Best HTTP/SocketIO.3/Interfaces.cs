#if !BESTHTTP_DISABLE_SOCKETIO

namespace BestHTTP.SocketIO3
{
    using BestHTTP.Logger;
    using BestHTTP.PlatformSupport.Memory;
    using BestHTTP.SocketIO3.Transports;

    /// <summary>
    /// 接口，通过将其实现为显式接口来对用户隐藏内部函数。
    /// </summary>
    public interface IManager
    {
        LoggingContext Context { get; }
        void Remove(Socket socket);
        void Close(bool removeSockets = true);
        void TryToReconnect();
        bool OnTransportConnected(ITransport transport);
        void OnTransportError(ITransport trans, string err);
        void OnTransportProbed(ITransport trans);
        void SendPacket(OutgoingPacket packet);
        void OnPacket(IncomingPacket packet);
        void EmitEvent(string eventName, params object[] args);
        void EmitEvent(SocketIOEventTypes type, params object[] args);
        void EmitError(string msg);
        void EmitAll(string eventName, params object[] args);
    }

    /// <summary>
    /// 接口，通过将其实现为显式接口来对用户隐藏内部函数。
    /// </summary>
    public interface ISocket
    {
        LoggingContext Context { get; }
        void Open();
        void Disconnect(bool remove);
        void OnPacket(IncomingPacket packet);
        void EmitEvent(SocketIOEventTypes type, params object[] args);
        void EmitEvent(string eventName, params object[] args);
        void EmitError(string msg);
    }
}

#endif

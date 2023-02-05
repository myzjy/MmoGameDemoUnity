using System;
using System.Collections.Concurrent;
using BestHTTP.Connections;
using BestHTTP.Extensions;

// Required for ConcurrentQueue.Clear extension.

// ReSharper disable once CheckNamespace
namespace BestHTTP.Core
{
    public enum ConnectionEvents
    {
        StateChange,
        ProtocolSupport
    }

    public
#if CSHARP_7_OR_LATER
        readonly
#endif
        struct ConnectionEventInfo
    {
        public readonly ConnectionBase Source;

        public readonly ConnectionEvents Event;

        public readonly HttpConnectionStates State;

        public readonly HostProtocolSupport ProtocolSupport;

        public readonly HttpRequest Request;

        public ConnectionEventInfo(ConnectionBase sourceConn, ConnectionEvents @event)
        {
            this.Source = sourceConn;
            this.Event = @event;

            this.State = HttpConnectionStates.Initial;

            this.ProtocolSupport = HostProtocolSupport.Unknown;

            this.Request = null;
        }

        public ConnectionEventInfo(ConnectionBase sourceConn, HttpConnectionStates newState)
        {
            this.Source = sourceConn;

            this.Event = ConnectionEvents.StateChange;

            this.State = newState;

            this.ProtocolSupport = HostProtocolSupport.Unknown;

            this.Request = null;
        }

        public ConnectionEventInfo(ConnectionBase sourceConn, HostProtocolSupport protocolSupport)
        {
            this.Source = sourceConn;
            this.Event = ConnectionEvents.ProtocolSupport;

            this.State = HttpConnectionStates.Initial;

            this.ProtocolSupport = protocolSupport;

            this.Request = null;
        }

        public ConnectionEventInfo(ConnectionBase sourceConn, HttpRequest request)
        {
            this.Source = sourceConn;

            this.Event = ConnectionEvents.StateChange;

            this.State = HttpConnectionStates.ClosedResendRequest;

            this.ProtocolSupport = HostProtocolSupport.Unknown;

            this.Request = request;
        }

        public override string ToString()
        {
            return
                $"[ConnectionEventInfo SourceConnection: {this.Source}, Event: {this.Event}, State: {this.State}, ProtocolSupport: {this.ProtocolSupport}]";
        }
    }

    public static class ConnectionEventHelper
    {
        private static readonly ConcurrentQueue<ConnectionEventInfo> ConnectionEventQueue =
            new ConcurrentQueue<ConnectionEventInfo>();

#pragma warning disable 0649
        private static Action<ConnectionEventInfo> _onEvent;
#pragma warning restore

        public static void EnqueueConnectionEvent(ConnectionEventInfo @event)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[{nameof(ConnectionEventHelper)}] [method:EnqueueConnectionEvent] [msg] Enqueue connection event: {@event.ToString()}");
#endif
            ConnectionEventQueue.Enqueue(@event);
        }

        internal static void Clear()
        {
            ConnectionEventQueue.Clear();
        }

        internal static void ProcessQueue()
        {
            while (ConnectionEventQueue.TryDequeue(out var connectionEvent))
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[{nameof(ConnectionEventHelper)}] [method:ProcessQueue] [msg] Processing connection event:  {connectionEvent.ToString()}");
#endif
                if (_onEvent != null)
                {
                    try
                    {
                        _onEvent(connectionEvent);
                    }
                    catch (Exception ex)
                    {
                        HttpManager.Logger.Exception("ConnectionEventHelper", "ProcessQueue", ex,
                            connectionEvent.Source.Context);
                    }
                }

                if (connectionEvent.Source.LastProcessedUri == null)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.Log(
                        $"[{nameof(ConnectionEventHelper)}] [method:ProcessQueue] [msg] Ignoring ConnectionEventInfo({connectionEvent.ToString()}) because its LastProcessedUri is null!");
#endif
                    return;
                }

                switch (connectionEvent.Event)
                {
                    case ConnectionEvents.StateChange:
                        HandleConnectionStateChange(connectionEvent);
                        break;

                    case ConnectionEvents.ProtocolSupport:
                        HostManager.GetHost(connectionEvent.Source.LastProcessedUri.Host)
                            .GetHostDefinition(connectionEvent.Source.ServerAddress)
                            .AddProtocol(connectionEvent.ProtocolSupport);
                        break;
                }
            }
        }

        private static void HandleConnectionStateChange(ConnectionEventInfo @event)
        {
            var connection = @event.Source;

            switch (@event.State)
            {
                case HttpConnectionStates.Recycle:
                    HostManager.GetHost(connection.LastProcessedUri.Host)
                        .GetHostDefinition(connection.ServerAddress)
                        .RecycleConnection(connection)
                        .TryToSendQueuedRequests();

                    break;

                case HttpConnectionStates.WaitForProtocolShutdown:
                    HostManager.GetHost(connection.LastProcessedUri.Host)
                        .GetHostDefinition(connection.ServerAddress)
                        .RemoveConnection(connection, @event.State);
                    break;

                case HttpConnectionStates.Closed:
                case HttpConnectionStates.ClosedResendRequest:
                    // in case of ClosedResendRequest
                    if (@event.Request != null)
                        RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(@event.Request,
                            RequestEvents.Resend));

                    HostManager.GetHost(connection.LastProcessedUri.Host)
                        .GetHostDefinition(connection.ServerAddress)
                        .RemoveConnection(connection, @event.State)
                        .TryToSendQueuedRequests();
                    break;
            }
        }
    }
}
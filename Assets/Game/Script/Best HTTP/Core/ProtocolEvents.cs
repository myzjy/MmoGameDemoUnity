using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BestHTTP.Extensions;

// Required for ConcurrentQueue.Clear extension.

namespace BestHTTP.Core
{
    public
#if CSHARP_7_OR_LATER
        readonly
#endif
        struct ProtocolEventInfo
    {
        public readonly IProtocol Source;

        public ProtocolEventInfo(IProtocol source)
        {
            this.Source = source;
        }

        public override string ToString()
        {
            return string.Format("[ProtocolEventInfo Source: {0}]", Source);
        }
    }

    public static class ProtocolEventHelper
    {
        private static ConcurrentQueue<ProtocolEventInfo> protocolEvents = new ConcurrentQueue<ProtocolEventInfo>();
        private static List<IProtocol> ActiveProtocols = new List<IProtocol>(2);

#pragma warning disable 0649
        public static Action<ProtocolEventInfo> OnEvent;
#pragma warning restore

        public static void EnqueueProtocolEvent(ProtocolEventInfo @event)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[ProtocolEventHelper] [EnqueueProtocolEvent] [msg] Enqueue protocol event: {@event.ToString()}");
#endif
            protocolEvents.Enqueue(@event);
        }

        internal static void Clear()
        {
            protocolEvents.Clear();
        }

        internal static void ProcessQueue()
        {
            while (protocolEvents.TryDequeue(out var protocolEvent))
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[ProtocolEventHelper] [ProcessQueue] [msg] Processing protocol event: {protocolEvent.ToString()}");
#endif
                if (OnEvent != null)
                {
                    try
                    {
                        OnEvent(protocolEvent);
                    }
                    catch (Exception ex)
                    {
                        HttpManager.Logger.Exception("ProtocolEventHelper", "ProcessQueue", ex,
                            protocolEvent.Source.LoggingContext);
                    }
                }

                IProtocol protocol = protocolEvent.Source;

                protocol.HandleEvents();

                if (protocol.IsClosed)
                {
                    ActiveProtocols.Remove(protocol);

                    HostManager.GetHost(protocol.ConnectionKey.Host)
                        .GetHostDefinition(protocol.ConnectionKey.Connection)
                        .TryToSendQueuedRequests();

                    protocol.Dispose();
                }
            }
        }

        internal static void AddProtocol(IProtocol protocol)
        {
            ActiveProtocols.Add(protocol);
        }

        internal static void CancelActiveProtocols()
        {
            for (int i = 0; i < ActiveProtocols.Count; ++i)
            {
                var protocol = ActiveProtocols[i];

                protocol.CancellationRequested();
            }
        }
    }
}
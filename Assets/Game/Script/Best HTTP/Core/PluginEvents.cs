using System;
using System.Collections.Concurrent;
using BestHTTP.Extensions;

// Required for ConcurrentQueue.Clear extension.

// ReSharper disable once CheckNamespace
namespace BestHTTP.Core
{
    public enum PluginEvents
    {
#if !BESTHTTP_DISABLE_COOKIES
        SaveCookieLibrary,
#endif

        SaveCacheLibrary,

        AltSvcHeader,

        Http2ConnectProtocol
    }

    public
#if CSHARP_7_OR_LATER
        readonly
#endif
        struct PluginEventInfo
    {
        public readonly PluginEvents Event;
        public readonly object Payload;

        public PluginEventInfo(PluginEvents @event)
        {
            this.Event = @event;
            this.Payload = null;
        }

        public PluginEventInfo(PluginEvents @event, object payload)
        {
            this.Event = @event;
            this.Payload = payload;
        }

        public override string ToString()
        {
            return $"[PluginEventInfo Event: {this.Event}]";
        }
    }

    public static class PluginEventHelper
    {
        private static readonly ConcurrentQueue<PluginEventInfo> PluginEvents = new ConcurrentQueue<PluginEventInfo>();

#pragma warning disable 0649
        private static Action<PluginEventInfo> _onEvent;
#pragma warning restore

        public static void EnqueuePluginEvent(PluginEventInfo @event)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[PluginEventHelper] [EnqueuePluginEvent] [msg] Enqueue plugin event: {@event.ToString()}");
#endif
            PluginEvents.Enqueue(@event);
        }

        internal static void Clear()
        {
            PluginEvents.Clear();
        }

        internal static void ProcessQueue()
        {
#if !BESTHTTP_DISABLE_COOKIES
            var saveCookieLibrary = false;
#endif

#if !BESTHTTP_DISABLE_CACHING
            var saveCacheLibrary = false;
#endif

            while (PluginEvents.TryDequeue(out var pluginEvent))
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log($"[PluginEventHelper] [ProcessQueue] [msg] Processing plugin event: {pluginEvent}");
#endif
                if (_onEvent != null)
                {
                    try
                    {
                        _onEvent(pluginEvent);
                    }
                    catch (Exception ex)
                    {
                        HttpManager.Logger.Exception("PluginEventHelper", "ProcessQueue", ex);
                    }
                }

                switch (pluginEvent.Event)
                {
#if !BESTHTTP_DISABLE_COOKIES
                    case Core.PluginEvents.SaveCookieLibrary:
                        saveCookieLibrary = true;
                        break;
#endif

#if !BESTHTTP_DISABLE_CACHING
                    case Core.PluginEvents.SaveCacheLibrary:
                        saveCacheLibrary = true;
                        break;
#endif

                    case Core.PluginEvents.AltSvcHeader:
                    {
                        if (pluginEvent.Payload is AltSvcEventInfo altSvcEventInfo)
                        {
                            HostManager.GetHost(altSvcEventInfo.Host)
                                .HandleAltSvcHeader(altSvcEventInfo.Response);
                        }
                    }
                        break;

                    case Core.PluginEvents.Http2ConnectProtocol:
                    {
                        if (pluginEvent.Payload is Http2ConnectProtocolInfo info)
                        {
                            HostManager.GetHost(info.Host)
                                .HandleConnectProtocol(info);
                        }
                    }
                        break;
                }
            }

#if !BESTHTTP_DISABLE_COOKIES
            if (saveCookieLibrary)
                PlatformSupport.Threading.ThreadedRunner.RunShortLiving(Cookies.CookieJar.Persist);
#endif

#if !BESTHTTP_DISABLE_CACHING
            if (saveCacheLibrary)
                PlatformSupport.Threading.ThreadedRunner.RunShortLiving(Caching.HttpCacheService.SaveLibrary);
#endif
        }
    }

    public sealed class AltSvcEventInfo
    {
        public readonly string Host;
        public readonly HttpResponse Response;

        public AltSvcEventInfo(string host, HttpResponse resp)
        {
            this.Host = host;
            this.Response = resp;
        }
    }

    public sealed class Http2ConnectProtocolInfo
    {
        public readonly bool Enabled;
        public readonly string Host;

        public Http2ConnectProtocolInfo(string host, bool enabled)
        {
            this.Host = host;
            this.Enabled = enabled;
        }
    }
}
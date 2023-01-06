using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace BestHTTP.Core
{
    public sealed class HostDefinition
    {
        private static readonly System.Text.StringBuilder KeyBuilder = new System.Text.StringBuilder(11);

        // While a ReaderWriterLockSlim would be best with read and write locking and we use only WriteLock, it's still a lightweight locking mechanism instead of the lock statement.
        private static readonly System.Threading.ReaderWriterLockSlim KeyBuilderLock =
            new System.Threading.ReaderWriterLockSlim(System.Threading.LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// Requests to the same host can require different connections: http, https, http + proxy, https + proxy, http2, http2 + proxy
        /// </summary>
        public readonly Dictionary<string, HostConnection> HostConnectionVariant =
            new Dictionary<string, HostConnection>();

        // alt-svc support:
        //  1. When a request receives an alt-svc header send a plugin msg to the manager with all the details to route to the proper hostDefinition.
        //  2. HostDefinition parses the header value
        //  3. If there's at least one supported protocol found, start open a connection to that said alternate
        //  4. If the new connection is open, route new requests to that connection
        public List<HostConnection> Alternates;

        public HostDefinition(string host)
        {
            this.Host = host;
        }

        public string Host { get; private set; }

        public HostConnection HasBetterAlternate(HTTPRequest request)
        {
            return null;
        }

        private HostConnection GetHostDefinition(HTTPRequest request)
        {
            var key = GetKeyForRequest(request);

            return GetHostDefinition(key);
        }

        public HostConnection GetHostDefinition(string key)
        {
            if (!this.HostConnectionVariant.TryGetValue(key, out var host))
                this.HostConnectionVariant.Add(key, host = new HostConnection(this, key));

            return host;
        }

        public void Send(HTTPRequest request)
        {
            GetHostDefinition(request)
                .Send(request);
        }

        public void TryToSendQueuedRequests()
        {
            foreach (var kvp in HostConnectionVariant)
                kvp.Value.TryToSendQueuedRequests();
        }

        public void HandleAltSvcHeader(HttpResponse response)
        {
            var headerValues = response.GetHeaderValues("alt-svc");
            if (headerValues == null)
                HttpManager.Logger.Warning(nameof(HostDefinition),
                    "Received HandleAltSvcHeader message, but no Alt-Svc header found!", response.Context);
        }

        public void HandleConnectProtocol(Http2ConnectProtocolInfo info)
        {
            HttpManager.Logger.Information(nameof(HostDefinition),
                $"Received HandleConnectProtocol message. Connect protocol for host {info.Host}. Enabled: {info.Enabled}");
        }

        internal void Shutdown()
        {
            foreach (var kvp in this.HostConnectionVariant)
            {
                kvp.Value.Shutdown();
            }
        }

        internal void SaveTo(System.IO.BinaryWriter bw)
        {
            bw.Write(this.HostConnectionVariant.Count);

            foreach (var kvp in this.HostConnectionVariant)
            {
                bw.Write(kvp.Key);

                kvp.Value.SaveTo(bw);
            }
        }

        internal void LoadFrom(int version, System.IO.BinaryReader br)
        {
            int count = br.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                GetHostDefinition(br.ReadString())
                    .LoadFrom(version, br);
            }
        }

        public static string GetKeyForRequest(HTTPRequest request)
        {
            return GetKeyFor(request.CurrentUri
#if !BESTHTTP_DISABLE_PROXY
                , request.Proxy
#endif
            );
        }

        public static string GetKeyFor(Uri uri
#if !BESTHTTP_DISABLE_PROXY
            , Proxy proxy
#endif
        )
        {
            if (uri.IsFile)
                return uri.ToString();

            KeyBuilderLock.EnterWriteLock();

            try
            {
                KeyBuilder.Length = 0;

#if !BESTHTTP_DISABLE_PROXY
                if (proxy != null && proxy.UseProxyForAddress(uri))
                {
                    KeyBuilder.Append(proxy.Address.Scheme);
                    KeyBuilder.Append("://");
                    KeyBuilder.Append(proxy.Address.Host);
                    KeyBuilder.Append(":");
                    KeyBuilder.Append(proxy.Address.Port);
                    KeyBuilder.Append(" @ ");
                }
#endif

                KeyBuilder.Append(uri.Scheme);
                KeyBuilder.Append("://");
                KeyBuilder.Append(uri.Host);
                KeyBuilder.Append(":");
                KeyBuilder.Append(uri.Port);

                return KeyBuilder.ToString();
            }
            finally
            {
                KeyBuilderLock.ExitWriteLock();
            }
        }
    }
}
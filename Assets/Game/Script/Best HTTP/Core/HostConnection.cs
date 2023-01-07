using System;
using System.Collections.Generic;
using BestHTTP.Connections;
using BestHTTP.Extensions;
using BestHTTP.Logger;

// ReSharper disable once CheckNamespace
namespace BestHTTP.Core
{
    public enum HostProtocolSupport : byte
    {
        Unknown = 0x00,
        Http1 = 0x01,
        Http2 = 0x02
    }

    /// <summary>
    /// A HostConnection object manages the connections to a host and the request queue.
    /// </summary>
    public sealed class HostConnection
    {
        private readonly List<ConnectionBase> _connections = new List<ConnectionBase>();
        private readonly List<HttpRequest> _queue = new List<HttpRequest>();

        public HostConnection(HostDefinition host, string variantId)
        {
            this.Host = host;
            this.VariantId = variantId;

            this.Context = new LoggingContext(this);
            this.Context.Add("Host", this.Host.Host);
            this.Context.Add("VariantId", this.VariantId);
        }

        private HostDefinition Host { get; set; }

        private string VariantId { get; set; }

        private HostProtocolSupport ProtocolSupport { get; set; }
        private DateTime LastProtocolSupportUpdate { get; set; }

        public int QueuedRequests => this._queue.Count;

        private LoggingContext Context { get; set; }

        internal void AddProtocol(HostProtocolSupport protocolSupport)
        {
            this.LastProtocolSupportUpdate = DateTime.UtcNow;

            var oldProtocol = this.ProtocolSupport;

            if (oldProtocol != protocolSupport)
            {
                this.ProtocolSupport = protocolSupport;

                HttpManager.Logger.Information(nameof(HostConnection),
                    $"AddProtocol({this.VariantId}) - changing from {oldProtocol} to {protocolSupport}", this.Context);

                HostManager.Save();

                TryToSendQueuedRequests();
            }
        }

        internal HostConnection Send(HttpRequest request)
        {
            var conn = GetNextAvailable(request);

            if (conn != null)
            {
                request.State = HttpRequestStates.Processing;

                request.Prepare();

                // then start process the request
                conn.Process(request);
            }
            else
            {
                // If no free connection found and creation prohibited, we will put back to the queue
                this._queue.Add(request);
            }

            return this;
        }

        private ConnectionBase GetNextAvailable(HttpRequest request)
        {
            int activeConnections = 0;
            ConnectionBase conn;
            // Check the last created connection first. This way, if a higher level protocol is present that can handle more requests (== HTTP/2) that protocol will be chosen
            //  and others will be closed when their inactivity time is reached.
            for (int i = _connections.Count - 1; i >= 0; --i)
            {
                conn = _connections[i];

                if (conn.State == HttpConnectionStates.Initial || conn.State == HttpConnectionStates.Free ||
                    conn.CanProcessMultiple)
                {
                    if (!conn.TestConnection())
                    {
                        HttpManager.Logger.Verbose("HostConnection",
                            "GetNextAvailable - TestConnection returned false!", this.Context, request.Context,
                            conn.Context);

                        RemoveConnectionImpl(conn, HttpConnectionStates.Closed);
                        continue;
                    }

                    HttpManager.Logger.Verbose("HostConnection",
                        string.Format(
                            "GetNextAvailable - returning with connection. state: {0}, CanProcessMultiple: {1}",
                            conn.State, conn.CanProcessMultiple), this.Context, request.Context, conn.Context);
                    return conn;
                }

                activeConnections++;
            }

            if (activeConnections >= HttpManager.MaxConnectionPerServer)
            {
                HttpManager.Logger.Verbose("HostConnection",
                    string.Format(
                        "GetNextAvailable - activeConnections({0}) >= HTTPManager.MaxConnectionPerServer({1})",
                        activeConnections, HttpManager.MaxConnectionPerServer), this.Context, request.Context);
                return null;
            }

            string key = HostDefinition.GetKeyForRequest(request);

#if UNITY_WEBGL && !UNITY_EDITOR
            conn = new WebGLConnection(key);
#else
            if (request.CurrentUri.IsFile)
                conn = new FileConnection(key);
            else
            {
#if !BESTHTTP_DISABLE_ALTERNATE_SSL
                // Hold back the creation of a new connection until we know more about the remote host's features.
                // If we send out multiple requests at once it will execute the first and delay the others. 
                // While it will decrease performance initially, it will prevent the creation of TCP connections
                //  that will be unused after their first request processing if the server supports HTTP/2.
                if (activeConnections >= 1 && (this.ProtocolSupport == HostProtocolSupport.Unknown ||
                                               this.ProtocolSupport == HostProtocolSupport.Http2))
                {
                    HttpManager.Logger.Verbose("HostConnection",
                        string.Format(
                            "GetNextAvailable - waiting for protocol support message. activeConnections: {0}, ProtocolSupport: {1} ",
                            activeConnections, this.ProtocolSupport), this.Context, request.Context);
                    return null;
                }
#endif

                conn = new HTTPConnection(key);
                HttpManager.Logger.Verbose("HostConnection",
                    $"GetNextAvailable - creating new connection, key: {key} ", this.Context,
                    request.Context, conn.Context);
            }
#endif
            _connections.Add(conn);

            return conn;
        }

        internal HostConnection RecycleConnection(ConnectionBase conn)
        {
            conn.State = HttpConnectionStates.Free;

            Timer.Add(new TimerData(TimeSpan.FromSeconds(1), conn, CloseConnectionAfterInactivity));

            return this;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool RemoveConnectionImpl(ConnectionBase conn, HttpConnectionStates setState)
        {
            conn.State = setState;
            conn.Dispose();

            bool found = this._connections.Remove(conn);

            if (!found)
                HttpManager.Logger.Information(nameof(HostConnection),
                    $"RemoveConnection - Couldn't find connection! key: {conn.ServerAddress}", this.Context,
                    conn.Context);

            return found;
        }

        internal HostConnection RemoveConnection(ConnectionBase conn, HttpConnectionStates setState)
        {
            RemoveConnectionImpl(conn, setState);

            return this;
        }

        internal HostConnection TryToSendQueuedRequests()
        {
            while (this._queue.Count > 0 && GetNextAvailable(this._queue[0]) != null)
            {
                Send(this._queue[0]);
                this._queue.RemoveAt(0);
            }

            return this;
        }

        public ConnectionBase Find(Predicate<ConnectionBase> match)
        {
            return this._connections.Find(match);
        }

        private bool CloseConnectionAfterInactivity(DateTime now, object context)
        {
            var conn = context as ConnectionBase;

            bool closeConnection = conn is { State: HttpConnectionStates.Free } &&
                                   now - conn.LastProcessTime >= conn.KeepAliveTime;
            if (closeConnection)
            {
                HttpManager.Logger.Information(nameof(HostConnection),
                    $"CloseConnectionAfterInactivity - [{conn}] Closing! State: {conn.State}, Now: {now.ToString(System.Globalization.CultureInfo.InvariantCulture)}, LastProcessTime: {conn.LastProcessTime.ToString(System.Globalization.CultureInfo.InvariantCulture)}, KeepAliveTime: {conn.KeepAliveTime}",
                    this.Context, conn.Context);

                RemoveConnection(conn, HttpConnectionStates.Closed);
                return false;
            }

            // repeat until the connection's state is free
            return conn is { State: HttpConnectionStates.Free };
        }

        public void RemoveAllIdleConnections()
        {
            for (int i = 0; i < this._connections.Count; i++)
                if (this._connections[i].State == HttpConnectionStates.Free)
                {
                    int countBefore = this._connections.Count;
                    RemoveConnection(this._connections[i], HttpConnectionStates.Closed);

                    if (countBefore != this._connections.Count)
                        i--;
                }
        }

        internal void Shutdown()
        {
            this._queue.Clear();

            foreach (var conn in this._connections)
            {
                // Swallow any exceptions, we are quitting anyway.
                try
                {
                    conn.Shutdown(ShutdownTypes.Immediate);
                }
                catch
                {
                    // ignored
                }
            }
            //this.Connections.Clear();
        }

        internal void SaveTo(System.IO.BinaryWriter bw)
        {
            bw.Write(this.LastProtocolSupportUpdate.ToBinary());
            bw.Write((byte)this.ProtocolSupport);
        }

        internal void LoadFrom(int version, System.IO.BinaryReader br)
        {
            this.LastProtocolSupportUpdate = DateTime.FromBinary(br.ReadInt64());
            this.ProtocolSupport = (HostProtocolSupport)br.ReadByte();

            if (DateTime.UtcNow - this.LastProtocolSupportUpdate >= TimeSpan.FromDays(1))
            {
                HttpManager.Logger.Verbose("HostConnection",
                    string.Format("LoadFrom - Too Old! LastProtocolSupportUpdate: {0}, ProtocolSupport: {1}",
                        this.LastProtocolSupportUpdate.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        this.ProtocolSupport), this.Context);
                this.ProtocolSupport = HostProtocolSupport.Unknown;
            }
            else
                HttpManager.Logger.Verbose("HostConnection",
                    string.Format("LoadFrom - LastProtocolSupportUpdate: {0}, ProtocolSupport: {1}",
                        this.LastProtocolSupportUpdate.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        this.ProtocolSupport), this.Context);
        }
    }
}
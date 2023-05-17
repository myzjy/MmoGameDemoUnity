using System;
using System.Collections.Generic;
using System.Text;
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
    /// HostConnection对象管理到主机和请求队列的连接。
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

#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                Debug.Log(
                    $"[{nameof(HostConnection)}] [method:AddProtocol(HostProtocolSupport protocolSupport)] [msg|Exception] AddProtocol({this.VariantId}) - changing from {oldProtocol} to {protocolSupport}");
#endif
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

                // 然后开始处理请求
                conn.Process(request);
            }
            else
            {
                // 如果没有发现空闲连接且创建被禁止，则将其放回队列
                this._queue.Add(request);
            }

            return this;
        }

        private ConnectionBase GetNextAvailable(HttpRequest request)
        {
            int activeConnections = 0;
            ConnectionBase conn;
            //首先检查最后创建的连接。这样，如果存在一个更高级别的协议，可以处理更多的请求(== HTTP/2)，该协议将被选择，而其他协议将在达到不活动时间时关闭。
            for (int i = _connections.Count - 1; i >= 0; --i)
            {
                conn = _connections[i];

                if (conn.State == HttpConnectionStates.Initial || conn.State == HttpConnectionStates.Free ||
                    conn.CanProcessMultiple)
                {
                    if (!conn.TestConnection())
                    {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                        Debug.Log(
                            $"[HostConnection] [method: GetNextAvailable(HttpRequest request)] [msg|Exception] GetNextAvailable - TestConnection returned false!");
#endif
                        RemoveConnectionImpl(conn, HttpConnectionStates.Closed);
                        continue;
                    }
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                    StringBuilder sb = new StringBuilder();
                    sb.Append($" GetNextAvailable -返回连接。 ");
                    sb.Append($" state: {conn.State}, CanProcessMultiple: {conn.CanProcessMultiple}");
                    Debug.Log(
                        $"[HostConnection] [method: GetNextAvailable(HttpRequest request)] [msg|Exception] {sb.ToString()}");
#endif
                    return conn;
                }

                activeConnections++;
            }

            if (activeConnections >= HttpManager.MaxConnectionPerServer)
            {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                StringBuilder sb = new StringBuilder();
                sb.Append($"GetNextAvailable - activeConnections({activeConnections}) ");
                sb.Append($" >= HTTPManager.MaxConnectionPerServer({HttpManager.MaxConnectionPerServer})");
                Debug.Log(
                    $"[HostConnection] [method: GetNextAvailable(HttpRequest request)] [msg|Exception] {sb.ToString()}");
#endif
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
                //在我们了解更多远程主机的特性之前，暂停创建新连接。
                //如果我们一次发送多个请求，它将执行第一个请求，并延迟其他请求。
                //虽然会降低初始性能，但会阻止TCP连接的创建
                //如果服务器支持HTTP/2，在第一次请求处理后将不使用。
                if (activeConnections >= 1 && (this.ProtocolSupport == HostProtocolSupport.Unknown ||
                                               this.ProtocolSupport == HostProtocolSupport.Http2))
                {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"GetNextAvailable - 等待协议支持消息。如果连接: {activeConnections},");
                    sb.Append($"ProtocolSupport: {this.ProtocolSupport} ");
                    Debug.Log(
                        $"[HostConnection] [method: GetNextAvailable(HttpRequest request)] [msg|Exception] {sb.ToString()}");
#endif
                    return null;
                }
#endif

                conn = new HTTPConnection(key);
                {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"GetNextAvailable - creating new connection, key: {key} ");
                    Debug.Log(
                        $"[HostConnection] [method: GetNextAvailable(HttpRequest request)] [msg|Exception] {sb.ToString()}");
#endif
                }
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
            {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                StringBuilder sb = new StringBuilder();
                sb.Append($"RemoveConnection - 找不到联系! key: {conn.ServerAddress} ");
                Debug.Log(
                    $"[{nameof(HostConnection)}] [method: RemoveConnectionImpl(ConnectionBase conn, HttpConnectionStates setState)] [msg|Exception] {sb.ToString()}");
#endif
            }

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
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                StringBuilder sb = new StringBuilder();
                sb.Append($"CloseConnectionAfterInactivity - [{conn}] Closing!");
                sb.Append(
                    $" State: {conn.State}, Now: {now.ToString(System.Globalization.CultureInfo.InvariantCulture)},");
                sb.Append(
                    $" LastProcessTime: {conn.LastProcessTime.ToString(System.Globalization.CultureInfo.InvariantCulture)},");
                sb.Append($" KeepAliveTime: {conn.KeepAliveTime}");
                Debug.Log(
                    $"[{nameof(HostConnection)}] [method: CloseConnectionAfterInactivity(DateTime now, object context)] [msg|Exception] {sb.ToString()}");
#endif
                RemoveConnection(conn, HttpConnectionStates.Closed);
                return false;
            }

            // 重复该操作，直到连接状态为空闲
            return conn is { State: HttpConnectionStates.Free };
        }

        public void RemoveAllIdleConnections()
        {
            for (int i = 0; i < this._connections.Count; i++)
            {
                if (this._connections[i].State != HttpConnectionStates.Free) continue;
                int countBefore = this._connections.Count;
                RemoveConnection(this._connections[i], HttpConnectionStates.Closed);

                if (countBefore != this._connections.Count)
                {
                    i--;
                }
            }
        }

        internal void Shutdown()
        {
            this._queue.Clear();

            foreach (var conn in this._connections)
            {
                // 没有任何例外，我们无论如何都要退出。
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
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                StringBuilder sb = new StringBuilder();
                sb.Append($"LoadFrom - Too Old! LastProtocolSupportUpdate:");
                sb.Append(
                    $" {this.LastProtocolSupportUpdate.ToString(System.Globalization.CultureInfo.InvariantCulture)},");
                sb.Append(
                    $" ProtocolSupport: {this.ProtocolSupport}");
                Debug.Log(
                    $"[{nameof(HostConnection)}] [method: LoadFrom(int version, System.IO.BinaryReader br)] [msg|Exception] {sb.ToString()}");
#endif
                this.ProtocolSupport = HostProtocolSupport.Unknown;
            }
            else
            {
#if (UNITY_EDITOR || (DEVELOP_BUILD && ENABLE_LOG))&& ENABLE_LOG_NETWORK
                StringBuilder sb = new StringBuilder();
                sb.Append($"LoadFrom - LastProtocolSupportUpdate:");
                sb.Append(
                    $" {this.LastProtocolSupportUpdate.ToString(System.Globalization.CultureInfo.InvariantCulture)},");
                sb.Append(
                    $" ProtocolSupport: {this.ProtocolSupport}");
                Debug.Log(
                    $"[{nameof(HostConnection)}] [method: LoadFrom(int version, System.IO.BinaryReader br)] [msg|Exception] {sb.ToString()}");
#endif
            }
        }
    }
}
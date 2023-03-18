#if !BESTHTTP_DISABLE_SOCKETIO
#if !BESTHTTP_DISABLE_WEBSOCKET

using System;
using System.Collections.Generic;

namespace BestHTTP.SocketIO3.Transports
{
    using BestHTTP.Connections;
    using BestHTTP.PlatformSupport.Memory;
    using BestHTTP.WebSocket;
    using Extensions;

    /// <summary>
    /// 可以与SocketIO服务器通信的传输实现。
    /// </summary>
    internal sealed class WebSocketTransport : ITransport
    {
        public TransportTypes Type
        {
            get { return TransportTypes.WebSocket; }
        }

        public TransportStates State { get; private set; }
        public SocketManager Manager { get; private set; }

        public bool IsRequestInProgress
        {
            get { return false; }
        }

        public bool IsPollingInProgress
        {
            get { return false; }
        }

        public WebSocket Implementation { get; private set; }

        public WebSocketTransport(SocketManager manager)
        {
            State = TransportStates.Closed;
            Manager = manager;
        }


        public void Open()
        {
            if (State != TransportStates.Closed)
                return;

            Uri uri = null;
            var scheme = HttpProtocolFactory.IsSecureProtocol(Manager.Uri) ? "wss" : "ws";
            var uriBuilder = new UriBuilder(scheme: (scheme),
                host: Manager.Uri.Host,
                port: Manager.Uri.Port,
                pathValue: Manager.Uri.GetRequestPathAndQueryURL());
            string baseUrl = uriBuilder.Uri.ToString();
            string format = "{0}?EIO={1}&transport=websocket{3}";
            if (Manager.Handshake != null)
            {
                format += "&sid={2}";
            }

            bool sendAdditionalQueryParams = !Manager.Options.QueryParamsOnlyForHandshake ||
                                             (Manager.Options.QueryParamsOnlyForHandshake
                                              && Manager.Handshake == null);

            uri = new Uri(string.Format(format,
                baseUrl,
                Manager.ProtocolVersion,
                Manager.Handshake != null ? Manager.Handshake.Sid : string.Empty,
                sendAdditionalQueryParams ? Manager.Options.BuildQueryParams() : string.Empty));

            Implementation = new WebSocket(uri);

#if !UNITY_WEBGL || UNITY_EDITOR
            Implementation.StartPingThread = true;

            if (this.Manager.Options.httpRequestCustomizationCallback != null)
            {
                Implementation.OnInternalRequestCreated = (ws, internalRequest) =>
                    this.Manager.Options.httpRequestCustomizationCallback(this.Manager, internalRequest);
            }
#endif

            Implementation.OnOpen = OnOpen;
            Implementation.OnMessage = OnMessage;
            Implementation.OnBinary = OnBinary;
            Implementation.OnError = OnError;
            Implementation.OnClosed = OnClosed;

            Implementation.Open();

            State = TransportStates.Connecting;
        }

        /// <summary>
        /// 关闭传输并清理资源。
        /// </summary>
        public void Close()
        {
            if (State == TransportStates.Closed)
                return;

            State = TransportStates.Closed;

            if (Implementation != null)
            {
                Implementation.Close();
            }
            else
            {
                HttpManager.Logger.Warning("WebSocketTransport", "Close - WebSocket Implementation already null!",
                    this.Manager.Context);
            }

            Implementation = null;
        }

        /// <summary>
        /// 轮询的实现。而WebSocket只是一个骨架。
        /// </summary>
        public void Poll()
        {
        }


        /// <summary>
        /// WebSocket实现OnOpen事件处理程序。
        /// </summary>
        private void OnOpen(WebSocket ws)
        {
            if (ws != Implementation)
            {
                return;
            }

            HttpManager.Logger.Information("WebSocketTransport", "OnOpen", this.Manager.Context);

            State = TransportStates.Opening;

            // 发送一个探测包来测试传输。如果我们收到一架载荷相同的飞船，我们就可以升级
            if (Manager.UpgradingTransport != this) return;
            var packet = this.Manager.Parser.CreateOutgoing(
                transportEvent: TransportEventTypes.Ping,
                payload: "probe");
            Send(packet);
        }

        /// <summary>
        /// WebSocket实现OnMessage事件处理程序。
        /// </summary>
        private void OnMessage(WebSocket ws, string message)
        {
            if (ws != Implementation)
                return;

            if (HttpManager.Logger.Level <= BestHTTP.Logger.Loglevels.All)
            {
                HttpManager.Logger.Verbose("WebSocketTransport", "OnMessage: " + message, this.Manager.Context);
            }

            var packet = IncomingPacket.Empty;
            try
            {
                packet = this.Manager.Parser.Parse(this.Manager, message);

                if (packet.TransportEvent == TransportEventTypes.Open)
                {
                    packet.DecodedArg =
                        BestHTTP.JSON.LitJson.JsonMapper.ToObject<HandshakeData>(packet.DecodedArg as string);
                }
            }
            catch (Exception ex)
            {
                HttpManager.Logger.Exception("WebSocketTransport", "OnMessage Packet parsing", ex,
                    this.Manager.Context);
            }

            if (!packet.Equals(IncomingPacket.Empty))
            {
                try
                {
                    OnPacket(packet);
                }
                catch (Exception ex)
                {
                    HttpManager.Logger.Exception("WebSocketTransport", "OnMessage OnPacket", ex, this.Manager.Context);
                }
            }
        }

        /// <summary>
        /// WebSocket实现OnBinary事件处理程序。
        /// </summary>
        private void OnBinary(WebSocket ws, byte[] data)
        {
            if (ws != Implementation)
                return;

            if (HttpManager.Logger.Level <= BestHTTP.Logger.Loglevels.All)
            {
                HttpManager.Logger.Verbose("WebSocketTransport", "OnBinary", this.Manager.Context);
            }

            IncomingPacket packet = IncomingPacket.Empty;
            try
            {
                packet = this.Manager.Parser.Parse(this.Manager, new BufferSegment(data, 0, data.Length));
            }
            catch (Exception ex)
            {
                HttpManager.Logger.Exception("WebSocketTransport", "OnBinary Packet parsing", ex, this.Manager.Context);
            }

            if (!packet.Equals(IncomingPacket.Empty))
            {
                try
                {
                    OnPacket(packet);
                }
                catch (Exception ex)
                {
                    HttpManager.Logger.Exception("WebSocketTransport", "OnBinary OnPacket", ex, this.Manager.Context);
                }
            }
        }

        /// <summary>
        /// WebSocket实现OnError事件处理程序。
        /// </summary>
        private void OnError(WebSocket ws, string error)
        {
            if (ws != Implementation)
                return;

#if !UNITY_WEBGL || UNITY_EDITOR
            if (string.IsNullOrEmpty(error))
            {
                switch (ws.InternalRequest.State)
                {
                    // The request finished without any problem.
                    case HttpRequestStates.Finished:
                        if (ws.InternalRequest.Response.IsSuccess || ws.InternalRequest.Response.StatusCode == 101)
                        {
                            error =
                                $"请求完成。状态码: {ws.InternalRequest.Response.StatusCode.ToString()} Message: {ws.InternalRequest.Response.Message}";
                        }
                        else
                            error =
                                $"请求完成成功，但是服务器发送了一个错误。状态码: {ws.InternalRequest.Response.StatusCode}-{ws.InternalRequest.Response.Message} Message: {ws.InternalRequest.Response.DataAsText}";

                        break;

                    // 请求结束时出现意外错误。请求的Exception属性可能包含有关错误的更多信息。
                    case HttpRequestStates.Error:
                    {
                        var str = $"{ws.InternalRequest.Exception.Message} {ws.InternalRequest.Exception.StackTrace}";
                        error =
                            $"Request Finished with Error! : {(ws.InternalRequest.Exception != null ? str : string.Empty)}";
                    }
                        break;

                    // 由用户发起的请求中止。
                    case HttpRequestStates.Aborted:
                        error = "Request Aborted!";
                        break;

                    // 连接服务器超时。处理步骤
                    case HttpRequestStates.ConnectionTimedOut:
                        error = "Connection Timed Out!";
                        break;

                    // The request didn't finished in the given time.
                    case HttpRequestStates.TimedOut:
                        error = "Processing the request Timed Out!";
                        break;
                }
            }
#endif

            if (Manager.UpgradingTransport != this)
                (Manager as IManager).OnTransportError(this, error);
            else
                Manager.UpgradingTransport = null;
        }

        /// <summary>
        /// WebSocket implementation OnClosed event handler.
        /// </summary>
        private void OnClosed(WebSocket ws, ushort code, string message)
        {
            if (ws != Implementation)
                return;

            HttpManager.Logger.Information("WebSocketTransport", "OnClosed", this.Manager.Context);

            Close();

            if (Manager.UpgradingTransport != this)
                (Manager as IManager).TryToReconnect();
            else
                Manager.UpgradingTransport = null;
        }


        /// <summary>
        /// A WebSocket implementation of the packet sending.
        /// </summary>
        public void Send(OutgoingPacket packet)
        {
            if (State == TransportStates.Closed ||
                State == TransportStates.Paused)
            {
                HttpManager.Logger.Information("WebSocketTransport",
                    string.Format("Send - State == {0}, skipping packet sending!", State), this.Manager.Context);
                return;
            }

            if (packet.IsBinary)
                Implementation.Send(packet.PayloadData.Data, (ulong)packet.PayloadData.Offset,
                    (ulong)packet.PayloadData.Count);
            else
                Implementation.Send(packet.Payload);

            if (packet.Attachements != null)
                for (int i = 0; i < packet.Attachements.Count; ++i)
                    Implementation.Send(packet.Attachements[i]);
        }

        /// <summary>
        /// A WebSocket implementation of the packet sending.
        /// </summary>
        public void Send(List<OutgoingPacket> packets)
        {
            for (int i = 0; i < packets.Count; ++i)
                Send(packets[i]);

            packets.Clear();
        }

        /// <summary>
        /// Will only process packets that need to upgrade. All other packets are passed to the Manager.
        /// </summary>
        private void OnPacket(IncomingPacket packet)
        {
            switch (packet.TransportEvent)
            {
                case TransportEventTypes.Open:
                    if (this.State != TransportStates.Opening)
                        HttpManager.Logger.Warning("WebSocketTransport",
                            "Received 'Open' packet while state is '" + State.ToString() + "'", this.Manager.Context);
                    else
                        State = TransportStates.Open;
                    goto default;

                case TransportEventTypes.Pong:
                    // Answer for a Ping Probe.
                    if ("probe".Equals(packet.DecodedArg))
                    {
                        State = TransportStates.Open;
                        (Manager as IManager).OnTransportProbed(this);
                    }

                    goto default;

                default:
                    if (Manager.UpgradingTransport != this)
                        (Manager as IManager).OnPacket(packet);
                    break;
            }
        }

    }
}

#endif
#endif
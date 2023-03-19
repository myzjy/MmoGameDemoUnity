#if !BESTHTTP_DISABLE_SOCKETIO
#if !BESTHTTP_DISABLE_WEBSOCKET

using System;
using System.Collections.Generic;
using System.Text;

namespace BestHTTP.SocketIO.Transports
{
    using BestHTTP.Connections;
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

        private Packet PacketWithAttachment;
        private byte[] Buffer;

        public WebSocketTransport(SocketManager manager)
        {
            State = TransportStates.Closed;
            Manager = manager;
        }

        public void Open()
        {
            if (State != TransportStates.Closed)
            {
                return;
            }

            Uri uri = null;
            var isSecureProtocol = HttpProtocolFactory.IsSecureProtocol(Manager.Uri);
            var wsOrWssString = isSecureProtocol ? "wss" : "ws";
            var urlBuilder = new UriBuilder(
                scheme: wsOrWssString,
                host: Manager.Uri.Host,
                port: Manager.Uri.Port,
                pathValue: Manager.Uri.GetRequestPathAndQueryURL());
            string baseUrl = urlBuilder.Uri.ToString();
            var format = new StringBuilder(10);
            format.Append($"{baseUrl}?");
            format.Append($"EIO={Manager.ProtocolVersion}&");
            var sendAdditionalQueryParams = !Manager.Options.QueryParamsOnlyForHandshake ||
                                            (Manager.Options.QueryParamsOnlyForHandshake &&
                                             Manager.Handshake == null);

            var webString = sendAdditionalQueryParams ? Manager.Options.BuildQueryParams() : string.Empty;
            format.Append($"transport=websocket{webString}");
            if (Manager.Handshake != null)
            {
                var sid = Manager.Handshake != null ? Manager.Handshake.Sid : string.Empty;
                format.Append($"&sid={sid}");
            }

            uri = new Uri(format.ToString());

            Implementation = new WebSocket(uri);

#if !UNITY_WEBGL || UNITY_EDITOR
            Implementation.StartPingThread = true;

            if (this.Manager.Options.HTTPRequestCustomizationCallback != null)
            {
                void DelegateHttpRequestCustomization(WebSocket ws, HttpRequest internalRequest)
                {
                    this.Manager.Options.HTTPRequestCustomizationCallback(this.Manager, internalRequest);
                }

                Implementation.OnInternalRequestCreated = DelegateHttpRequestCustomization;
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
            {
                return;
            }

            State = TransportStates.Closed;

            if (Implementation != null)
            {
                Implementation.Close();
            }
            else
            {
                HttpManager.Logger.Warning("WebSocketTransport", "关闭- WebSocket实现已经为空!");
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
        /// WebSocket implementation OnOpen event handler.
        /// </summary>
        private void OnOpen(WebSocket ws)
        {
            if (ws != Implementation)
                return;

            HttpManager.Logger.Information("WebSocketTransport", "OnOpen");

            State = TransportStates.Opening;

            // Send a Probe packet to test the transport. If we receive back a pong with the same payload we can upgrade
            if (Manager.UpgradingTransport == this)
                Send(new Packet(TransportEventTypes.Ping, SocketIOEventTypes.Unknown, "/", "probe"));
        }

        /// <summary>
        /// WebSocket implementation OnMessage event handler.
        /// </summary>
        private void OnMessage(WebSocket ws, string message)
        {
            if (ws != Implementation)
                return;

            if (HttpManager.Logger.Level <= BestHTTP.Logger.Loglevels.All)
                HttpManager.Logger.Verbose("WebSocketTransport", "OnMessage: " + message);

            Packet packet = null;
            try
            {
                packet = new Packet(message);
            }
            catch (Exception ex)
            {
                HttpManager.Logger.Exception("WebSocketTransport", "OnMessage Packet parsing", ex);
            }

            if (packet == null)
            {
                HttpManager.Logger.Error("WebSocketTransport", "Message parsing failed. Message: " + message);
                return;
            }

            try
            {
                if (packet.AttachmentCount == 0)
                    OnPacket(packet);
                else
                    PacketWithAttachment = packet;
            }
            catch (Exception ex)
            {
                HttpManager.Logger.Exception("WebSocketTransport", "OnMessage OnPacket", ex);
            }
        }

        /// <summary>
        /// WebSocket implementation OnBinary event handler.
        /// </summary>
        private void OnBinary(WebSocket ws, byte[] data)
        {
            if (ws != Implementation)
                return;

            if (HttpManager.Logger.Level <= BestHTTP.Logger.Loglevels.All)
                HttpManager.Logger.Verbose("WebSocketTransport", "OnBinary");

            if (PacketWithAttachment != null)
            {
                switch (this.Manager.Options.ServerVersion)
                {
                    case SupportedSocketIOVersions.v2:
                        PacketWithAttachment.AddAttachmentFromServer(data, false);
                        break;
                    case SupportedSocketIOVersions.v3:
                        PacketWithAttachment.AddAttachmentFromServer(data, true);
                        break;
                    default:
                        HttpManager.Logger.Warning("WebSocketTransport",
                            "Binary packet received while the server's version is Unknown. Set SocketOption's ServerVersion to the correct value to avoid packet mishandling!");

                        // Fall back to V2 by default.
                        this.Manager.Options.ServerVersion = SupportedSocketIOVersions.v2;
                        goto case SupportedSocketIOVersions.v2;
                }

                if (PacketWithAttachment.HasAllAttachment)
                {
                    try
                    {
                        OnPacket(PacketWithAttachment);
                    }
                    catch (Exception ex)
                    {
                        HttpManager.Logger.Exception("WebSocketTransport", "OnBinary", ex);
                    }
                    finally
                    {
                        PacketWithAttachment = null;
                    }
                }
            }
            else
            {
                // Room for improvement: we received an unwanted binary message?
            }
        }

        /// <summary>
        /// WebSocket implementation OnError event handler.
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
                            error = string.Format("Request finished. Status Code: {0} Message: {1}",
                                ws.InternalRequest.Response.StatusCode.ToString(), ws.InternalRequest.Response.Message);
                        else
                            error = string.Format(
                                "Request Finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                ws.InternalRequest.Response.StatusCode,
                                ws.InternalRequest.Response.Message,
                                ws.InternalRequest.Response.DataAsText);
                        break;

                    // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
                    case HttpRequestStates.Error:
                        error = "Request Finished with Error! : " + ws.InternalRequest.Exception != null
                            ? (ws.InternalRequest.Exception.Message + " " + ws.InternalRequest.Exception.StackTrace)
                            : string.Empty;
                        break;

                    // The request aborted, initiated by the user.
                    case HttpRequestStates.Aborted:
                        error = "Request Aborted!";
                        break;

                    // Connecting to the server is timed out.
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

            HttpManager.Logger.Information("WebSocketTransport", "OnClosed");

            Close();

            if (Manager.UpgradingTransport != this)
                (Manager as IManager).TryToReconnect();
            else
                Manager.UpgradingTransport = null;
        }

        /// <summary>
        /// A WebSocket implementation of the packet sending.
        /// </summary>
        public void Send(Packet packet)
        {
            if (State == TransportStates.Closed ||
                State == TransportStates.Paused)
            {
                HttpManager.Logger.Information("WebSocketTransport",
                    string.Format("Send - State == {0}, skipping packet sending!", State));
                return;
            }

            string encoded = packet.Encode();

            if (HttpManager.Logger.Level <= BestHTTP.Logger.Loglevels.All)
                HttpManager.Logger.Verbose("WebSocketTransport", "Send: " + encoded);

            if (packet.AttachmentCount != 0 || (packet.Attachments != null && packet.Attachments.Count != 0))
            {
                if (packet.Attachments == null)
                    throw new ArgumentException("packet.Attachments are null!");

                if (packet.AttachmentCount != packet.Attachments.Count)
                    throw new ArgumentException(
                        "packet.AttachmentCount != packet.Attachments.Count. Use the packet.AddAttachment function to add data to a packet!");
            }

            Implementation.Send(encoded);

            if (packet.AttachmentCount != 0)
            {
                int maxLength = packet.Attachments[0].Length + 1;
                for (int cv = 1; cv < packet.Attachments.Count; ++cv)
                    if ((packet.Attachments[cv].Length + 1) > maxLength)
                        maxLength = packet.Attachments[cv].Length + 1;

                if (Buffer == null || Buffer.Length < maxLength)
                    Array.Resize(ref Buffer, maxLength);

                for (int i = 0; i < packet.AttachmentCount; i++)
                {
                    Buffer[0] = (byte)TransportEventTypes.Message;

                    Array.Copy(packet.Attachments[i], 0, Buffer, 1, packet.Attachments[i].Length);

                    Implementation.Send(Buffer, 0, (ulong)packet.Attachments[i].Length + 1UL);
                }
            }
        }

        /// <summary>
        /// A WebSocket implementation of the packet sending.
        /// </summary>
        public void Send(List<Packet> packets)
        {
            for (int i = 0; i < packets.Count; ++i)
                Send(packets[i]);

            packets.Clear();
        }


        /// <summary>
        /// 将只处理需要升级的数据包。所有其他数据包都传递给Manager。
        /// </summary>
        private void OnPacket(Packet packet)
        {
            switch (packet.TransportEvent)
            {
                case TransportEventTypes.Open:
                    if (this.State != TransportStates.Opening)
                    {
                        HttpManager.Logger.Warning("WebSocketTransport",
                            $"当状态为{State.ToString()}时，收到 打开 数据包");
                    }
                    else
                        State = TransportStates.Open;

                    goto default;

                case TransportEventTypes.Pong:
                {
                    // Answer for a Ping Probe.
                    if (packet.Payload == "probe")
                    {
                        State = TransportStates.Open;
                        (Manager as IManager).OnTransportProbed(this);
                    }

                    goto default;
                }

                case TransportEventTypes.Unknown:
                case TransportEventTypes.Close:
                case TransportEventTypes.Ping:
                case TransportEventTypes.Message:
                case TransportEventTypes.Upgrade:
                case TransportEventTypes.Noop:
                default:
                {
                    if (Manager.UpgradingTransport != this)
                    {
                        (Manager as IManager).OnPacket(packet);
                    }
                }
                    break;
            }
        }
    }
}

#endif
#endif
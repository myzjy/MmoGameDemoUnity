#if !BESTHTTP_DISABLE_SOCKETIO
#if !BESTHTTP_DISABLE_WEBSOCKET

using System;
using System.Collections.Generic;
using System.Text;

namespace BestHTTP.SocketIO.Transports
{
    using Connections;
    using WebSocket;
    using Extensions;

    /// <summary>
    /// 可以与SocketIO服务器通信的传输实现。
    /// </summary>
    internal sealed class WebSocketTransport : ITransport
    {
        public TransportTypes Type => TransportTypes.WebSocket;

        public TransportStates State { get; private set; }
        public SocketManager Manager { get; private set; }

        public bool IsRequestInProgress => false;

        public bool IsPollingInProgress => false;

        private WebSocket Implementation { get; set; }

        private Packet _packetWithAttachment;
        private byte[] _buffer;

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

            var uri = new Uri(format.ToString());

            Implementation = new WebSocket(uri);

#if !UNITY_WEBGL || UNITY_EDITOR
            Implementation.StartPingThread = true;

            if (this.Manager.Options.httpRequestCustomizationCallback != null)
            {
                void DelegateHttpRequestCustomization(WebSocket ws, HttpRequest internalRequest)
                {
                    this.Manager.Options.httpRequestCustomizationCallback(this.Manager, internalRequest);
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
        /// WebSocket实现OnOpen事件处理程序。
        /// </summary>
        private void OnOpen(WebSocket ws)
        {
            if (ws != Implementation)
                return;

            HttpManager.Logger.Information("WebSocketTransport", "OnOpen");

            State = TransportStates.Opening;

            // 发送一个探测包来测试传输。如果我们收到一架载荷相同的飞船，我们就可以升级
            if (Manager.UpgradingTransport != this) return;
            var packet = new Packet(
                transportEvent: TransportEventTypes.Ping,
                packetType: SocketIOEventTypes.Unknown,
                nsp: "/",
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

            if (HttpManager.Logger.Level <= Logger.Loglevels.All)
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
                    _packetWithAttachment = packet;
            }
            catch (Exception ex)
            {
                HttpManager.Logger.Exception("WebSocketTransport", "OnMessage OnPacket", ex);
            }
        }

        /// <summary>
        ///WebSocket实现OnBinary事件处理程序。
        /// </summary>
        private void OnBinary(WebSocket ws, byte[] data)
        {
            if (ws != Implementation)
                return;

            if (HttpManager.Logger.Level <= Logger.Loglevels.All)
                HttpManager.Logger.Verbose("WebSocketTransport", "OnBinary");

            if (_packetWithAttachment != null)
            {
                switch (this.Manager.Options.ServerVersion)
                {
                    case SupportedSocketIOVersions.v2:
                        _packetWithAttachment.AddAttachmentFromServer(data, false);
                        break;
                    case SupportedSocketIOVersions.v3:
                        _packetWithAttachment.AddAttachmentFromServer(data, true);
                        break;
                    default:
                        HttpManager.Logger.Warning("WebSocketTransport",
                            "服务器版本为Unknown时收到的二进制数据包。设置SocketOption的ServerVersion为正确的值，以避免包错误处理!");

                        // Fall back to V2 by default.
                        this.Manager.Options.ServerVersion = SupportedSocketIOVersions.v2;
                        goto case SupportedSocketIOVersions.v2;
                }

                if (_packetWithAttachment.HasAllAttachment)
                {
                    try
                    {
                        OnPacket(_packetWithAttachment);
                    }
                    catch (Exception ex)
                    {
                        HttpManager.Logger.Exception("WebSocketTransport", "OnBinary", ex);
                    }
                    finally
                    {
                        _packetWithAttachment = null;
                    }
                }
            }
            else
            {
                // 改进空间:我们收到了不想要的二进制消息?
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
                    // 请求顺利完成。
                    case HttpRequestStates.Finished:
                    {
                        if (ws.InternalRequest.Response.IsSuccess || ws.InternalRequest.Response.StatusCode == 101)
                        {
                            error =
                                $"Request finished. Status Code: {ws.InternalRequest.Response.StatusCode.ToString()} Message: {ws.InternalRequest.Response.Message}";
                        }
                        else
                        {
                            error =
                                $"请求完成成功，但是服务器发送了一个错误。状态码:{ws.InternalRequest.Response.StatusCode}-{ws.InternalRequest.Response.Message}报文:{ws.InternalRequest.Response.DataAsText}";
                        }
                    }

                        break;

                    // 请求结束时出现意外错误。请求的Exception属性可能包含有关错误的更多信息。
                    case HttpRequestStates.Error:
                    {
                        error = "Request Finished with Error! : " + (ws.InternalRequest.Exception != null
                            ? ($"{ws.InternalRequest.Exception.Message}  {ws.InternalRequest.Exception.StackTrace}")
                            : string.Empty);
                    }
                        break;

                    // 由用户发起的请求中止。
                    case HttpRequestStates.Aborted:
                    {
                        error = "Request Aborted!";
                    }
                        break;

                    // 连接服务器超时。处理步骤
                    case HttpRequestStates.ConnectionTimedOut:
                    {
                        error = "Connection Timed Out!";
                    }
                        break;

                    // 请求没有在规定的时间内完成。
                    case HttpRequestStates.TimedOut:
                    {
                        error = "Processing the request Timed Out!";
                    }
                        break;
                }
            }
#endif

            if (Manager.UpgradingTransport != this)
            {
                (Manager as IManager).OnTransportError(this, error);
            }
            else
            {
                Manager.UpgradingTransport = null;
            }
        }

        /// <summary>
        /// WebSocket实现OnClosed事件处理程序。
        /// </summary>
        private void OnClosed(WebSocket ws, ushort code, string message)
        {
            if (ws != Implementation)
            {
                return;
            }

            HttpManager.Logger.Information("WebSocketTransport", "OnClosed");

            Close();

            if (Manager.UpgradingTransport != this)
            {
                (Manager as IManager).TryToReconnect();
            }
            else
            {
                Manager.UpgradingTransport = null;
            }
        }

        /// <summary>
        /// WebSocket实现的数据包发送。
        /// </summary>
        public void Send(Packet packet)
        {
            if (State is TransportStates.Closed or TransportStates.Paused)
            {
                HttpManager.Logger.Information("WebSocketTransport",
                    $"Send - State == {State}, 跳过数据包发送!");
                return;
            }

            string encoded = packet.Encode();

            if (HttpManager.Logger.Level <= Logger.Loglevels.All)
            {
                HttpManager.Logger.Verbose("WebSocketTransport", "Send: " + encoded);
            }

            if (packet.AttachmentCount != 0 || (packet.Attachments != null && packet.Attachments.Count != 0))
            {
                if (packet.Attachments == null)
                    throw new ArgumentException("packet.Attachments are null!");

                if (packet.AttachmentCount != packet.Attachments.Count)
                {
                    throw new ArgumentException(
                        "packet.AttachmentCount != packet.Attachments.Count. 使用packet.AddAttachment 函数，添加数据到数据包!");
                }
            }

            Implementation.Send(encoded);

            if (packet.AttachmentCount != 0)
            {
                if (packet.Attachments != null)
                {
                    int maxLength = packet.Attachments[0].Length + 1;
                    for (int cv = 1; cv < packet.Attachments.Count; ++cv)
                    {
                        if ((packet.Attachments[cv].Length + 1) > maxLength)
                        {
                            maxLength = packet.Attachments[cv].Length + 1;
                        }
                    }

                    if (_buffer == null || _buffer.Length < maxLength)
                    {
                        Array.Resize(ref _buffer, maxLength);
                    }
                }

                for (int i = 0; i < packet.AttachmentCount; i++)
                {
                    _buffer[0] = (byte)TransportEventTypes.Message;

                    if (packet.Attachments == null) continue;
                    Array.Copy(packet.Attachments[i], 0, _buffer, 1, packet.Attachments[i].Length);

                    Implementation.Send(_buffer, 0, (ulong)packet.Attachments[i].Length + 1UL);
                }
            }
        }

        /// <summary>
        /// WebSocket实现的数据包发送。
        /// </summary>
        public void Send(List<Packet> packets)
        {
            foreach (var t in packets)
            {
                Send(t);
            }

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
                {
                    if (this.State != TransportStates.Opening)
                    {
                        HttpManager.Logger.Warning("WebSocketTransport",
                            $"当状态为{State.ToString()}时，收到 打开 数据包");
                    }
                    else
                        State = TransportStates.Open;

                    goto default;
                }

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
#if !BESTHTTP_DISABLE_SIGNALR_CORE && !BESTHTTP_DISABLE_WEBSOCKET
using System;
using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.Memory;

namespace BestHTTP.SignalRCore.Transports
{
    /// <summary>
    /// WebSockets transport implementation.
    /// https://github.com/dotnet/aspnetcore/blob/master/src/SignalR/docs/specs/TransportProtocols.md#websockets-full-duplex
    /// </summary>
    internal sealed class WebSocketTransport : TransportBase
    {
        private WebSocket.WebSocket webSocket;

        internal WebSocketTransport(HubConnection con)
            : base(con)
        {
        }

        public override TransportTypes TransportType
        {
            get { return TransportTypes.WebSocket; }
        }

        public override void StartConnect()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[WebSocketTransport] [method: WebSocketTransport.StartConnect] [msg|Exception] StartConnect");
#endif
            if (this.webSocket == null)
            {
                Uri uri = this.connection.Uri;
                string scheme = Connections.HttpProtocolFactory.IsSecureProtocol(uri) ? "wss" : "ws";
                int port = uri.Port != -1
                    ? uri.Port
                    : (scheme.Equals("wss", StringComparison.OrdinalIgnoreCase) ? 443 : 80);

                // 不知怎的，如果我使用UriBuilder，这是不一样的，如果uri是由字符串构造的
                uri = new Uri($"{scheme}://{uri.Host}:{port}{uri.GetRequestPathAndQueryURL()}");

                uri = BuildUri(uri);

                // 此外，如果有一个身份验证提供者，它可以进一步改变我们的uri。
                if (this.connection.AuthenticationProvider != null)
                {
                    uri = this.connection.AuthenticationProvider.PrepareUri(uri) ?? uri;
                }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[WebSocketTransport] [method: WebSocketTransport.StartConnect] [msg|Exception] StartConnect connecting to Uri: {uri.ToString()}");
#endif
                this.webSocket = new WebSocket.WebSocket(uri);
                this.webSocket.Context.Add("Transport", this.Context);
            }

#if !UNITY_WEBGL || UNITY_EDITOR
            this.webSocket.StartPingThread = true;

            // prepare the internal http request
            if (this.connection.AuthenticationProvider != null)
                webSocket.OnInternalRequestCreated = (ws, internalRequest) =>
                    this.connection.AuthenticationProvider.PrepareRequest(internalRequest);
#endif
            this.webSocket.OnOpen += OnOpen;
            this.webSocket.OnMessage += OnMessage;
            this.webSocket.OnBinary += OnBinary;
            this.webSocket.OnError += OnError;
            this.webSocket.OnClosed += OnClosed;

            this.webSocket.Open();

            this.State = TransportStates.Connecting;
        }

        public override void Send(BufferSegment msg)
        {
            if (this.webSocket == null || !this.webSocket.IsOpen)
            {
                BufferPool.Release(msg.Data);

                //this.OnError(this.webSocket, "Send called while the websocket is null or isn't open! Transport's State: " + this.State);
                return;
            }

            this.webSocket.Send(msg.Data, (ulong)msg.Offset, (ulong)msg.Count);

            BufferPool.Release(msg.Data);
        }

        // The websocket connection is open
        private void OnOpen(WebSocket.WebSocket webSocket)
        {
            HttpManager.Logger.Verbose("WebSocketTransport", "OnOpen", this.Context);

            // https://github.com/dotnet/aspnetcore/blob/master/src/SignalR/docs/specs/HubProtocol.md#overview
            // When our websocket connection is open, send the 'negotiation' message to the server.
            (this as ITransport).Send(JsonProtocol.WithSeparator(
                string.Format("{{\"protocol\":\"{0}\", \"version\": 1}}", this.connection.Protocol.Name)));
        }

        private void OnMessage(WebSocket.WebSocket webSocket, string data)
        {
            if (this.State == TransportStates.Closing)
                return;

            if (this.State == TransportStates.Connecting)
            {
                HandleHandshakeResponse(data);

                return;
            }

            this.messages.Clear();
            try
            {
                int len = System.Text.Encoding.UTF8.GetByteCount(data);

                byte[] buffer = BufferPool.Get(len, true);
                try
                {
                    Array.Clear(buffer, 0, buffer.Length);

                    System.Text.Encoding.UTF8.GetBytes(data, 0, data.Length, buffer, 0);

                    this.connection.Protocol.ParseMessages(new BufferSegment(buffer, 0, len), ref this.messages);
                }
                finally
                {
                    BufferPool.Release(buffer);
                }

                this.connection.OnMessages(this.messages);
            }
            catch (Exception ex)
            {
                HttpManager.Logger.Exception("WebSocketTransport", "OnMessage(string)", ex, this.Context);
            }
            finally
            {
                this.messages.Clear();
            }
        }

        private void OnBinary(WebSocket.WebSocket webSocket, byte[] data)
        {
            if (this.State == TransportStates.Closing)
                return;

            if (this.State == TransportStates.Connecting)
            {
                HandleHandshakeResponse(System.Text.Encoding.UTF8.GetString(data, 0, data.Length));

                return;
            }

            this.messages.Clear();
            try
            {
                this.connection.Protocol.ParseMessages(new BufferSegment(data, 0, data.Length), ref this.messages);

                this.connection.OnMessages(this.messages);
            }
            catch (Exception ex)
            {
                HttpManager.Logger.Exception("WebSocketTransport", "OnMessage(byte[])", ex, this.Context);
            }
            finally
            {
                this.messages.Clear();

                BufferPool.Release(data);
            }
        }

        private void OnError(WebSocket.WebSocket webSocket, string reason)
        {
            HttpManager.Logger.Verbose("WebSocketTransport", "OnError: " + reason, this.Context);

            if (this.State == TransportStates.Closing)
            {
                this.State = TransportStates.Closed;
            }
            else
            {
                this.ErrorReason = reason;
                this.State = TransportStates.Failed;
            }
        }

        private void OnClosed(WebSocket.WebSocket webSocket, ushort code, string message)
        {
            HttpManager.Logger.Verbose("WebSocketTransport", "OnClosed: " + code + " " + message, this.Context);

            this.webSocket = null;

            this.State = TransportStates.Closed;
        }

        public override void StartClose()
        {
            HttpManager.Logger.Verbose("WebSocketTransport", "StartClose", this.Context);

            if (this.webSocket != null && this.webSocket.IsOpen)
            {
                this.State = TransportStates.Closing;
                this.webSocket.Close();
            }
            else
                this.State = TransportStates.Closed;
        }
    }
}
#endif
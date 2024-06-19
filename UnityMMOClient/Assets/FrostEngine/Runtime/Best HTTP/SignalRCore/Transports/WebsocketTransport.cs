#if !BESTHTTP_DISABLE_SIGNALR_CORE && !BESTHTTP_DISABLE_WEBSOCKET
using System;
using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.Memory;
using FrostEngine;

namespace BestHTTP.SignalRCore.Transports
{
    /// <summary>
    /// WebSockets transport implementation.
    /// https://github.com/dotnet/aspnetcore/blob/master/src/SignalR/docs/specs/TransportProtocols.md#webSockets-full-duplex
    /// </summary>
    internal sealed class WebSocketTransport : TransportBase
    {
        private WebSocket.WebSocket _webSocket;

        internal WebSocketTransport(HubConnection con)
            : base(con)
        {
        }

        public override TransportTypes TransportType => TransportTypes.WebSocket;

        public override void StartConnect()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[WebSocketTransport] [method: WebSocketTransport.StartConnect] [msg|Exception] StartConnect");
#endif
            if (this._webSocket == null)
            {
                Uri uri = this.Connection.Uri;
                string scheme = Connections.HttpProtocolFactory.IsSecureProtocol(uri) ? "wss" : "ws";
                int port = uri.Port != -1
                    ? uri.Port
                    : (scheme.Equals("wss", StringComparison.OrdinalIgnoreCase) ? 443 : 80);

                // 不知怎的，如果我使用UriBuilder，这是不一样的，如果uri是由字符串构造的
                uri = new Uri($"{scheme}://{uri.Host}:{port}{uri.GetRequestPathAndQueryURL()}");

                uri = BuildUri(uri);

                // 此外，如果有一个身份验证提供者，它可以进一步改变我们的uri。
                if (this.Connection.AuthenticationProvider != null)
                {
                    uri = this.Connection.AuthenticationProvider.PrepareUri(uri) ?? uri;
                }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[WebSocketTransport] [method: WebSocketTransport.StartConnect] [msg|Exception] StartConnect connecting to Uri: {uri}");
#endif
                this._webSocket = new WebSocket.WebSocket(uri);
                this._webSocket.Context.Add("Transport", this.Context);
            }

#if !UNITY_WEBGL || UNITY_EDITOR
            this._webSocket.StartPingThread = true;

            // prepare the internal http request
            if (this.Connection.AuthenticationProvider != null)
                _webSocket.OnInternalRequestCreated = (_, internalRequest) =>
                    this.Connection.AuthenticationProvider.PrepareRequest(internalRequest);
#endif
            this._webSocket.OnOpen += OnOpen;
            this._webSocket.OnMessage += OnMessage;
            this._webSocket.OnBinary += OnBinary;
            this._webSocket.OnError += OnError;
            this._webSocket.OnClosed += OnClosed;

            this._webSocket.Open();

            this.State = TransportStates.Connecting;
        }

        public override void Send(BufferSegment msg)
        {
            if (this._webSocket == null || !this._webSocket.IsOpen)
            {
                BufferPool.Release(msg.Data);

                //this.OnError(this.webSocket, "Send called while the websocket is null or isn't open! Transport's State: " + this.State);
                return;
            }

            this._webSocket.Send(msg.Data, (ulong)msg.Offset, (ulong)msg.Count);

            BufferPool.Release(msg.Data);
        }

        // The websocket connection is open
        private void OnOpen(WebSocket.WebSocket webSocket)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[WebSocketTransport] [method:OnOpen] [msg] OnOpen");
#endif

            // https://github.com/dotnet/aspnetcore/blob/master/src/SignalR/docs/specs/HubProtocol.md#overview
            // When our websocket connection is open, send the 'negotiation' message to the server.
            this.Send(JsonProtocol.WithSeparator(
                $"{{\"protocol\":\"{this.Connection.Protocol.Name}\", \"version\": 1}}"));
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

            this.Messages.Clear();
            try
            {
                int len = System.Text.Encoding.UTF8.GetByteCount(data);

                byte[] buffer = BufferPool.Get(len, true);
                try
                {
                    Array.Clear(buffer, 0, buffer.Length);

                    System.Text.Encoding.UTF8.GetBytes(data, 0, data.Length, buffer, 0);

                    this.Connection.Protocol.ParseMessages(new BufferSegment(buffer, 0, len), ref this.Messages);
                }
                finally
                {
                    BufferPool.Release(buffer);
                }

                this.Connection.OnMessages(this.Messages);
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.LogError(
                    $"[WebSocketTransport] [method:OnMessage] [msg|Exception] OnMessage(string) [Exception] {ex}");
#endif
            }
            finally
            {
                this.Messages.Clear();
            }
        }

        private void OnBinary(WebSocket.WebSocket webSocket, byte[] data)
        {
            if (this.State == TransportStates.Closing)
            {
                return;
            }

            if (this.State == TransportStates.Connecting)
            {
                HandleHandshakeResponse(System.Text.Encoding.UTF8.GetString(data, 0, data.Length));

                return;
            }

            this.Messages.Clear();
            try
            {
                this.Connection.Protocol.ParseMessages(new BufferSegment(data, 0, data.Length), ref this.Messages);

                this.Connection.OnMessages(this.Messages);
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.LogError(
                    $"[WebSocketTransport] [method:OnMessage(byte[])] [msg|Exception] OnMessage(byte[])Exception:{ex}");
#endif
                throw new Exception(
                    $"[WebSocketTransport] [method:OnMessage(byte[])] [msg|Exception] OnMessage(byte[])Exception:{ex}");
            }
            finally
            {
                this.Messages.Clear();

                BufferPool.Release(data);
            }
        }

        private void OnError(WebSocket.WebSocket webSocket, string reason)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[WebSocketTransport] [method:OnError] [msg] OnError");
#endif
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
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[WebSocketTransport] [method:OnError] [msg] OnClosed:{code} {message}");
#endif

            this._webSocket = null;

            this.State = TransportStates.Closed;
        }

        public override void StartClose()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[WebSocketTransport] [method:StartClose] [msg] StartClose");
#endif

            if (this._webSocket is { IsOpen: true })
            {
                this.State = TransportStates.Closing;
                this._webSocket.Close();
            }
            else
                this.State = TransportStates.Closed;
        }
    }
}
#endif
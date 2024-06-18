#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_WEBSOCKET
using System;
using System.Text;
using BestHTTP.Connections;
using BestHTTP.Extensions;
using BestHTTP.WebSocket.Frames;

namespace BestHTTP.WebSocket
{
    internal sealed class OverHTTP1 : WebSocketBaseImplementation
    {
        /// <summary>
        /// Indicates whether we sent out the connection request to the server.
        /// </summary>
        private bool requestSent;

        /// <summary>
        /// The internal WebSocketResponse object
        /// </summary>
        private WebSocketResponse webSocket;

        public OverHTTP1(WebSocket parent, Uri uri, string origin, string protocol) : base(parent, uri, origin,
            protocol)
        {
            string scheme = HttpProtocolFactory.IsSecureProtocol(uri) ? "wss" : "ws";
            int port = uri.Port != -1
                ? uri.Port
                : (scheme.Equals("wss", StringComparison.OrdinalIgnoreCase) ? 443 : 80);

            // Somehow if i use the UriBuilder it's not the same as if the uri is constructed from a string...
            //uri = new UriBuilder(uri.Scheme, uri.Host, uri.Scheme.Equals("wss", StringComparison.OrdinalIgnoreCase) ? 443 : 80, uri.PathAndQuery).Uri;
            base.Uri = new Uri(scheme + "://" + uri.Host + ":" + port + uri.GetRequestPathAndQueryURL());
        }

        public override bool IsOpen => webSocket != null && !webSocket.IsClosed;

        public override int BufferedAmount => webSocket.BufferedAmount;

        public override int Latency => this.webSocket.Latency;
        public override DateTime LastMessageReceived => this.webSocket.LastMessage;

        protected override void CreateInternalRequest()
        {
            if (this.SetInternalRequest != null)
                return;

            this.SetInternalRequest = new HttpRequest(base.Uri, OnInternalRequestCallback);

            this.SetInternalRequest.Context.Add("WebSocket", this.Parent.Context);

            // Called when the regular GET request is successfully upgraded to WebSocket
            this.SetInternalRequest.OnUpgraded = OnInternalRequestUpgraded;

            //http://tools.ietf.org/html/rfc6455#section-4

            // The request MUST contain an |Upgrade| header field whose value MUST include the "websocket" keyword.
            this.SetInternalRequest.SetHeader("Upgrade", "websocket");

            // The request MUST contain a |Connection| header field whose value MUST include the "Upgrade" token.
            this.SetInternalRequest.SetHeader("Connection", "Upgrade");

            // The request MUST include a header field with the name |Sec-WebSocket-Key|.  The value of this header field MUST be a nonce consisting of a
            // randomly selected 16-byte value that has been base64-encoded (see Section 4 of [RFC4648]).  The nonce MUST be selected randomly for each connection.
            this.SetInternalRequest.SetHeader("Sec-WebSocket-Key",
                WebSocket.GetSecKey(new object[] { this, InternalRequest, base.Uri, new object() }));

            // The request MUST include a header field with the name |Origin| [RFC6454] if the request is coming from a browser client.
            // If the connection is from a non-browser client, the request MAY include this header field if the semantics of that client match the use-case described here for browser clients.
            // More on Origin Considerations: http://tools.ietf.org/html/rfc6455#section-10.2
            if (!string.IsNullOrEmpty(Origin))
                this.SetInternalRequest.SetHeader("Origin", Origin);

            // The request MUST include a header field with the name |Sec-WebSocket-Version|.  The value of this header field MUST be 13.
            this.SetInternalRequest.SetHeader("Sec-WebSocket-Version", "13");

            if (!string.IsNullOrEmpty(Protocol))
                this.SetInternalRequest.SetHeader("Sec-WebSocket-Protocol", Protocol);

            // Disable caching
            this.SetInternalRequest.SetHeader("Cache-Control", "no-cache");
            this.SetInternalRequest.SetHeader("Pragma", "no-cache");

#if !BESTHTTP_DISABLE_CACHING
            this.SetInternalRequest.DisableCache = true;
#endif

#if !BESTHTTP_DISABLE_PROXY
            this.SetInternalRequest.Proxy = this.Parent.GetProxy(this.Uri);
#endif

            if (this.Parent.OnInternalRequestCreated != null)
            {
                try
                {
                    this.Parent.OnInternalRequestCreated(this.Parent, this.SetInternalRequest);
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError(
                        $"[OverHTTP1] [method:CreateInternalRequest] [msg|Exception] CreateInternalRequest  Exception:{ex}");
#endif
                }
            }
        }

        public override void StartClose(UInt16 code, string message)
        {
            if (this.State == WebSocketStates.Connecting)
            {
                if (this.InternalRequest != null)
                    this.InternalRequest.Abort();

                this.State = WebSocketStates.Closed;
                if (this.Parent.OnClosed != null)
                    this.Parent.OnClosed(this.Parent, (ushort)WebSocketStausCodes.NoStatusCode, string.Empty);
            }
            else
            {
                this.State = WebSocketStates.Closing;
                webSocket.Close(code, message);
            }
        }

        public override void StartOpen()
        {
            if (requestSent)
                throw new InvalidOperationException("已经叫开了!你不能重用这个WebSocket实例!");

            if (this.Parent.Extensions != null)
            {
                try
                {
                    for (int i = 0; i < this.Parent.Extensions.Length; ++i)
                    {
                        var ext = this.Parent.Extensions[i];
                        if (ext != null)
                            ext.AddNegotiation(InternalRequest);
                    }
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError(
                        $"[OverHTTP1] [method:StartOpen] [msg|Exception] Open  Exception:{ex}");
#endif
                }
            }

            InternalRequest.Send();
            requestSent = true;
            this.State = WebSocketStates.Connecting;
        }

        private void OnInternalRequestCallback(HttpRequest req, HttpResponse resp)
        {
            string reason = string.Empty;

            switch (req.State)
            {
                case HttpRequestStates.Finished:
                {
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        var sb = new StringBuilder(3);
                        sb.Append("Request finished.");
                        sb.Append($" Status Code: {resp.StatusCode.ToString()}");
                        sb.Append($" Message: {resp.Message}");
                        Debug.Log($"[OverHTTP1] [method:OnInternalRequestCallback] [msg] {sb.ToString()}");
#endif
                    }

                    if (resp.StatusCode == 101)
                    {
                        // The request upgraded successfully.
                        return;
                    }
                    else
                    {
                        reason =
                            $"Request Finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText}";
                    }
                }
                    break;

                // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
                case HttpRequestStates.Error:
                {
                    reason = "Request Finished with Error! " + (req.Exception != null
                        ? ("Exception: " + req.Exception.Message + req.Exception.StackTrace)
                        : string.Empty);
                }
                    break;

                // The request aborted, initiated by the user.
                case HttpRequestStates.Aborted:
                    reason = "Request Aborted!";
                    break;

                // Connecting to the server is timed out.
                case HttpRequestStates.ConnectionTimedOut:
                    reason = "Connection Timed Out!";
                    break;

                // The request didn't finished in the given time.
                case HttpRequestStates.TimedOut:
                    reason = "Processing the request Timed Out!";
                    break;

                default:
                    return;
            }

            if (this.State != WebSocketStates.Connecting || !string.IsNullOrEmpty(reason))
            {
                if (this.Parent.OnError != null)
                {
                    this.Parent.OnError(this.Parent, reason);
                }
                else if (!HttpManager.IsQuitting)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError($"[OverHTTP1] [msg:{reason}]");
#endif
                }
            }
            else if (this.Parent.OnClosed != null)
            {
                this.Parent.OnClosed(this.Parent, (ushort)WebSocketStausCodes.NormalClosure, "Closed while opening");
            }

            this.State = WebSocketStates.Closed;

            if (!req.IsKeepAlive && resp is WebSocketResponse response)
            {
                response.CloseStream();
            }
        }

        private void OnInternalRequestUpgraded(HttpRequest req, HttpResponse resp)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[OverHTTP1] [msg:内部请求升级!]");
#endif
            // HttpManager.Logger.Information("OverHTTP1", "Internal request upgraded!", this.Parent.Context);

            webSocket = resp as WebSocketResponse;

            if (webSocket == null)
            {
                if (this.Parent.OnError != null)
                {
                    var reason = string.Empty;
                    if (req.Exception != null)
                    {
                        reason = $"{req.Exception.Message}  {req.Exception.StackTrace}";
                    }

                    this.Parent.OnError(this.Parent, reason);
                }

                this.State = WebSocketStates.Closed;
                return;
            }

            // If Close called while we connected
            if (this.State == WebSocketStates.Closed)
            {
                webSocket.CloseStream();
                return;
            }

            if (!resp.HasHeader("sec-websocket-accept"))
            {
                this.State = WebSocketStates.Closed;
                webSocket.CloseStream();

                if (this.Parent.OnError != null)
                    this.Parent.OnError(this.Parent, "No Sec-Websocket-Accept header is sent by the server!");
                return;
            }

            webSocket.WebSocket = this.Parent;

            if (this.Parent.Extensions != null)
            {
                for (int i = 0; i < this.Parent.Extensions.Length; ++i)
                {
                    var ext = this.Parent.Extensions[i];

                    try
                    {
                        if (ext != null && !ext.ParseNegotiation(webSocket))
                            this.Parent.Extensions[i] = null; // Keep extensions only that successfully negotiated
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.LogError(
                            $"[OverHTTP1] [method:OnInternalRequestUpgraded] [msg|Exception] ParseNegotiation  Exception:{ex}");
#endif
                        // Do not try to use a defective extension in the future
                        this.Parent.Extensions[i] = null;
                    }
                }
            }

            this.State = WebSocketStates.Open;
            if (this.Parent.OnOpen != null)
            {
                try
                {
                    this.Parent.OnOpen(this.Parent);
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError(
                        $"[OverHTTP1] [method:OnInternalRequestUpgraded] [msg|Exception] Open  Exception:{ex}");
#endif
                }
            }

            webSocket.OnText = (ws, msg) =>
            {
                if (this.Parent.OnMessage != null)
                    this.Parent.OnMessage(this.Parent, msg);
            };

            webSocket.OnBinary = (ws, bin) =>
            {
                if (this.Parent.OnBinary != null)
                    this.Parent.OnBinary(this.Parent, bin);
            };

            webSocket.OnClosed = (ws, code, msg) =>
            {
                this.State = WebSocketStates.Closed;

                if (this.Parent.OnClosed != null)
                    this.Parent.OnClosed(this.Parent, code, msg);
            };

            if (this.Parent.OnIncompleteFrame != null)
                webSocket.OnIncompleteFrame = (ws, frame) =>
                {
                    if (this.Parent.OnIncompleteFrame != null)
                        this.Parent.OnIncompleteFrame(this.Parent, frame);
                };

            if (this.Parent.StartPingThread)
                webSocket.StartPinging(Math.Max(this.Parent.PingFrequency, 100));

            webSocket.StartReceive();
        }

        public override void Send(string message)
        {
            webSocket.Send(message);
        }

        public override void Send(byte[] buffer)
        {
            webSocket.Send(buffer);
        }

        public override void Send(byte[] buffer, ulong offset, ulong count)
        {
            webSocket.Send(buffer, offset, count);
        }

        public override void Send(WebSocketFrame frame)
        {
            webSocket.Send(frame);
        }
    }
}
#endif
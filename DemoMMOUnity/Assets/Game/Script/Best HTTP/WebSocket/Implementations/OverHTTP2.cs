#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2 && !BESTHTTP_DISABLE_WEBSOCKET
using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP.Connections.HTTP2;
using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.Memory;
using BestHTTP.WebSocket.Frames;
using BestHTTP.WebSocket.Implementations.Utils;

namespace BestHTTP.WebSocket
{
    /// <summary>
    /// 实现RFC 8441 (https://tools.ietf.org/html/rfc8441)在HTTP/2上使用Websocket
    /// </summary>
    public sealed class OverHttp2 : WebSocketBaseImplementation, IHeartbeat
    {
        /// <summary>
        /// True if we sent out a Close message to the server
        /// </summary>
        private volatile bool _closeSent;

        private readonly Http2Handler _http2Handler;

        private PeekableIncomingSegmentStream incomingSegmentStream = new PeekableIncomingSegmentStream();

        private List<WebSocketFrameReader> IncompleteFrames = new List<WebSocketFrameReader>();

        /// <summary>
        /// When we sent out the last ping.
        /// </summary>
        private DateTime lastPing = DateTime.MinValue;

        /// <summary>
        /// A circular buffer to store the last N rtt times calculated by the pong messages.
        /// </summary>
        private CircularBuffer<int> rtts = new CircularBuffer<int>(WebSocketResponse.RTTBufferCapacity);

        private LockedBufferSegmenStream upStream;

        private bool waitingForPong = false;

        public OverHttp2(WebSocket parent, Http2Handler handler, Uri uri, string origin, string protocol) : base(parent,
            uri, origin, protocol)
        {
            this._http2Handler = handler;

            string scheme = "https";
            int port = uri.Port != -1 ? uri.Port : 443;

            base.Uri = new Uri(scheme + "://" + uri.Host + ":" + port + uri.GetRequestPathAndQueryURL());
        }

        public override int BufferedAmount
        {
            get => (int)this.upStream.Length;
        }

        public override bool IsOpen => this.State == WebSocketStates.Open;

        public override int Latency
        {
            get { return this.Parent.StartPingThread ? base.Latency : (int)this._http2Handler.Latency; }
        }

        public void OnHeartbeatUpdate(TimeSpan dif)
        {
            DateTime now = DateTime.Now;

            switch (this.State)
            {
                case WebSocketStates.Connecting:
                    if (now - this.InternalRequest.Timing.Start >= this.Parent.CloseAfterNoMessage)
                    {
                        if (HttpManager.Http2Settings.WebSocketOverHttp2Settings.EnableImplementationFallback)
                        {
                            this.State = WebSocketStates.Closed;
                            this.InternalRequest.OnHeadersReceived = null;
                            this.InternalRequest.Callback = null;
                            this.Parent.FallbackToHTTP1();
                        }
                        else
                        {
                            CloseWithError("WebSocket Over HTTP/2 Implementation failed to connect in the given time!");
                        }
                    }

                    break;

                case WebSocketStates.Open:
                    if (this.Parent.StartPingThread)
                    {
                        if (!waitingForPong && now - LastMessageReceived >=
                            TimeSpan.FromMilliseconds(this.Parent.PingFrequency))
                            SendPing();

                        if (waitingForPong && now - lastPing > this.Parent.CloseAfterNoMessage)
                        {
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                StringBuilder sb = new StringBuilder();
                                sb.Append("[OverHTTP2] ");
                                sb.Append("[method: OnHeartbeatUpdate] ");
                                sb.Append("[msg|Exception] ");
                                sb.Append($"No message received in the given time!");
                                sb.Append($" Closing WebSocket. LastPing: {this.lastPing}");
                                sb.Append($", PingFrequency: {TimeSpan.FromMilliseconds(this.Parent.PingFrequency)}");
                                sb.Append($", Close After: {this.Parent.CloseAfterNoMessage},");
                                sb.Append($" Now: {now}");
                                Debug.Log(sb.ToString());
#endif
                            }

                            CloseWithError("No message received in the given time!");
                        }
                    }

                    break;

                case WebSocketStates.Closed:
                    HttpManager.Heartbeats.Unsubscribe(this);
                    HttpUpdateDelegator.OnApplicationForegroundStateChanged -= OnApplicationForegroundStateChanged;
                    break;
            }
        }

        protected override void CreateInternalRequest()
        {
            base._internalRequest = new HttpRequest(base.Uri, HttpMethods.Connect, OnInternalRequestCallback);
            base._internalRequest.Context.Add("WebSocket", this.Parent.Context);

            base._internalRequest.SetHeader(":protocol", "websocket");

            // The request MUST include a header field with the name |Sec-WebSocket-Key|.  The value of this header field MUST be a nonce consisting of a
            // randomly selected 16-byte value that has been base64-encoded (see Section 4 of [RFC4648]).  The nonce MUST be selected randomly for each connection.
            base._internalRequest.SetHeader("sec-webSocket-key",
                WebSocket.GetSecKey(new object[] { this, InternalRequest, base.Uri, new object() }));

            // The request MUST include a header field with the name |Origin| [RFC6454] if the request is coming from a browser client.
            // If the connection is from a non-browser client, the request MAY include this header field if the semantics of that client match the use-case described here for browser clients.
            // More on Origin Considerations: http://tools.ietf.org/html/rfc6455#section-10.2
            if (!string.IsNullOrEmpty(base.Origin))
                base._internalRequest.SetHeader("origin", base.Origin);

            // The request MUST include a header field with the name |Sec-WebSocket-Version|.  The value of this header field MUST be 13.
            base._internalRequest.SetHeader("sec-webSocket-version", "13");

            if (!string.IsNullOrEmpty(base.Protocol))
                base._internalRequest.SetHeader("sec-webSocket-protocol", base.Protocol);

            // Disable caching
            base._internalRequest.SetHeader("cache-control", "no-cache");
            base._internalRequest.SetHeader("pragma", "no-cache");

#if !BESTHTTP_DISABLE_CACHING
            base._internalRequest.DisableCache = true;
#endif

            base._internalRequest.OnHeadersReceived += OnHeadersReceived;

            base._internalRequest.OnStreamingData += OnFrame;
            base._internalRequest.StreamChunksImmediately = true;

            base._internalRequest.UploadStream = this.upStream = new LockedBufferSegmenStream();
            base._internalRequest.UseUploadStreamLength = false;

            if (this.Parent.OnInternalRequestCreated != null)
            {
                try
                {
                    this.Parent.OnInternalRequestCreated(this.Parent, base._internalRequest);
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError(
                        $"[OverHTTP2] [method:CreateInternalRequest] [msg|Exception] CreateInternalRequest  Exception:{ex}");
#endif
                }
            }
        }

        private void OnHeadersReceived(HttpRequest req, HttpResponse resp, Dictionary<string, List<string>> newHeaders)
        {
            if (resp != null && resp.StatusCode == 200)
            {
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
                            $"[OverHTTP2] [method:OnHeadersReceived] [msg|Exception] OnOpen  Exception:{ex}");
#endif
                    }
                }

                if (this.Parent.StartPingThread)
                {
                    this.LastMessageReceived = DateTime.Now;
                    SendPing();
                }
            }
            else
                req.Abort();
        }

        private static bool CanReadFullFrame(PeekableIncomingSegmentStream stream)
        {
            if (stream.Length < 2)
                return false;

            stream.BeginPeek();

            if (stream.PeekByte() == -1)
                return false;

            int maskAndLength = stream.PeekByte();
            if (maskAndLength == -1)
                return false;

            // The second byte is the Mask Bit and the length of the payload data
            var HasMask = (maskAndLength & 0x80) != 0;

            // if 0-125, that is the payload length.
            var Length = (UInt64)(maskAndLength & 127);

            // If 126, the following 2 bytes interpreted as a 16-bit unsigned integer are the payload length.
            if (Length == 126)
            {
                byte[] rawLen = BufferPool.Get(2, true);

                for (int i = 0; i < 2; i++)
                {
                    int data = stream.PeekByte();
                    if (data < 0)
                        return false;

                    rawLen[i] = (byte)data;
                }

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(rawLen, 0, 2);

                Length = (UInt64)BitConverter.ToUInt16(rawLen, 0);

                BufferPool.Release(rawLen);
            }
            else if (Length == 127)
            {
                // If 127, the following 8 bytes interpreted as a 64-bit unsigned integer (the
                // most significant bit MUST be 0) are the payload length.

                byte[] rawLen = BufferPool.Get(8, true);

                for (int i = 0; i < 8; i++)
                {
                    int data = stream.PeekByte();
                    if (data < 0)
                        return false;

                    rawLen[i] = (byte)data;
                }

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(rawLen, 0, 8);

                Length = (UInt64)BitConverter.ToUInt64(rawLen, 0);

                BufferPool.Release(rawLen);
            }

            // Header + Mask&Length
            Length += 2;

            // 4 bytes for Mask if present
            if (HasMask)
                Length += 4;

            return stream.Length >= (long)Length;
        }

        private bool OnFrame(HttpRequest request, HttpResponse response, byte[] dataFragment, int dataFragmentLength)
        {
            base.LastMessageReceived = DateTime.Now;

            this.incomingSegmentStream.Write(dataFragment, 0, dataFragmentLength);

            while (CanReadFullFrame(this.incomingSegmentStream))
            {
                WebSocketFrameReader frame = new WebSocketFrameReader();
                frame.Read(this.incomingSegmentStream);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[OverHTTP2] [method:OnFrame] [msg] Frame received: {frame.Type}");
#endif
                if (!frame.IsFinal)
                {
                    if (this.Parent.OnIncompleteFrame == null)
                    {
                        IncompleteFrames.Add(frame);
                    }
                    else if (this.Parent.OnIncompleteFrame != null)
                    {
                        try
                        {
                            this.Parent.OnIncompleteFrame(this.Parent, frame);
                        }
                        catch (Exception ex)
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.LogError(
                                $"[OverHTTP2] [method:OnFrame] [msg|Exception] OnIncompleteFrame  Exception:{ex}");
#endif
                        }
                    }

                    return false;
                }

                switch (frame.Type)
                {
                    // For a complete documentation and rules on fragmentation see http://tools.ietf.org/html/rfc6455#section-5.4
                    // A fragmented Frame's last fragment's opcode is 0 (Continuation) and the FIN bit is set to 1.
                    case WebSocketFrameTypes.Continuation:
                        // Do an assemble pass only if OnFragment is not set. Otherwise put it in the CompletedFrames, we will handle it in the HandleEvent phase.
                        if (this.Parent.OnIncompleteFrame == null)
                        {
                            frame.Assemble(IncompleteFrames);

                            // Remove all incomplete frames
                            IncompleteFrames.Clear();

                            // Control frames themselves MUST NOT be fragmented. So, its a normal text or binary frame. Go, handle it as usual.
                            //goto case WebSocketFrameTypes.Binary;
                            if (frame.Type == WebSocketFrameTypes.Text)
                                goto case WebSocketFrameTypes.Text;
                            else if (frame.Type == WebSocketFrameTypes.Binary)
                                goto case WebSocketFrameTypes.Binary;
                        }
                        else
                        {
                            if (this.Parent.OnIncompleteFrame != null)
                            {
                                try
                                {
                                    this.Parent.OnIncompleteFrame(this.Parent, frame);
                                }
                                catch (Exception ex)
                                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                    Debug.LogError(
                                        $"[OverHTTP2] [method:OnFrame] [msg|Exception] OnIncompleteFrame  Exception:{ex}");
#endif
                                }
                            }
                        }

                        break;

                    case WebSocketFrameTypes.Text:
                        frame.DecodeWithExtensions(this.Parent);
                        if (this.Parent.OnMessage != null)
                        {
                            try
                            {
                                this.Parent.OnMessage(this.Parent, frame.DataAsText);
                            }
                            catch (Exception ex)
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                Debug.LogError(
                                    $"[OverHTTP2] [method:OnFrame] [msg|Exception] OnMessage  Exception:{ex}");
#endif
                            }
                        }

                        break;

                    case WebSocketFrameTypes.Binary:
                        frame.DecodeWithExtensions(this.Parent);
                        if (this.Parent.OnBinary != null)
                        {
                            try
                            {
                                this.Parent.OnBinary(this.Parent, frame.Data);
                            }
                            catch (Exception ex)
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                Debug.LogError(
                                    $"[OverHTTP2] [method:OnFrame] [msg|Exception] OnBinary  Exception:{ex}");
#endif
                            }
                        }

                        break;

                    // Upon receipt of a Ping frame, an endpoint MUST send a Pong frame in response, unless it already received a Close frame.
                    case WebSocketFrameTypes.Ping:
                        if (!_closeSent && !this.upStream.IsClosed)
                            Send(new WebSocketFrame(this.Parent, WebSocketFrameTypes.Pong, frame.Data));
                        break;

                    case WebSocketFrameTypes.Pong:
                        waitingForPong = false;

                        try
                        {
                            // Get the ticks from the frame's payload
                            long ticksSent = BitConverter.ToInt64(frame.Data, 0);

                            // the difference between the current time and the time when the ping message is sent
                            TimeSpan diff = TimeSpan.FromTicks(this.LastMessageReceived.Ticks - ticksSent);

                            // add it to the buffer
                            this.rtts.Add((int)diff.TotalMilliseconds);

                            // and calculate the new latency
                            base.Latency = CalculateLatency();
                        }
                        catch
                        {
                            // https://tools.ietf.org/html/rfc6455#section-5.5
                            // A Pong frame MAY be sent unsolicited.  This serves as a
                            // unidirectional heartbeat.  A response to an unsolicited Pong frame is
                            // not expected. 
                        }

                        break;

                    // If an endpoint receives a Close frame and did not previously send a Close frame, the endpoint MUST send a Close frame in response.
                    case WebSocketFrameTypes.ConnectionClose:

#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log($"[OverHTTP2] [method:OnFrame] [msg] ConnectionClose packet received!");
#endif
                        //CloseFrame = frame;
                        if (!_closeSent)
                            Send(new WebSocketFrame(this.Parent, WebSocketFrameTypes.ConnectionClose, null));
                        this.upStream.Close();

                        UInt16 statusCode = 0;
                        string msg = string.Empty;
                        try
                        {
                            // If we received any data, we will get the status code and the message from it
                            if (frame.Data != null && frame.Length >= 2)
                            {
                                if (BitConverter.IsLittleEndian)
                                    Array.Reverse(frame.Data, 0, 2);
                                statusCode = BitConverter.ToUInt16(frame.Data, 0);

                                if (frame.Data.Length > 2)
                                    msg = System.Text.Encoding.UTF8.GetString(frame.Data, 2, (int)frame.Length - 2);

                                frame.ReleaseData();
                            }
                        }
                        catch (Exception ex)
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.LogError(
                                $"[OverHTTP2] [method:OnFrame] [msg|Exception] OnFrame - parsing ConnectionClose data  Exception:{ex}");
#endif
                        }

                        if (this.Parent.OnClosed != null)
                        {
                            try
                            {
                                this.Parent.OnClosed(this.Parent, statusCode, msg);
                            }
                            catch (Exception ex)
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                Debug.LogError(
                                    $"[OverHTTP2] [method:OnFrame] [msg|Exception] OnClosed  Exception:{ex}");
#endif
                            }

                            this.Parent.OnClosed = null;
                        }

                        this.State = WebSocketStates.Closed;

                        break;
                }
            }

            return false;
        }

        private void OnInternalRequestCallback(HttpRequest req, HttpResponse resp)
        {
            // If it's already closed, all events are called too.
            if (this.State == WebSocketStates.Closed)
                return;

            if (this.State == WebSocketStates.Connecting &&
                HttpManager.Http2Settings.WebSocketOverHttp2Settings.EnableImplementationFallback)
            {
                this.Parent.FallbackToHTTP1();
                return;
            }

            string reason = string.Empty;

            switch (req.State)
            {
                case HttpRequestStates.Finished:
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    StringBuilder sb = new StringBuilder(3);
                    sb.Append($"Request finished. Status Code:");
                    sb.Append($" {resp.StatusCode.ToString()}");
                    sb.Append($" Message: {resp.Message}");
                    Debug.Log(
                        $"[OverHTTP2] [OnInternalRequestCallback] [msg]{sb.ToString()}");
#endif
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
                    reason = "Request Finished with Error! " + (req.Exception != null
                        ? ("Exception: " + req.Exception.Message + req.Exception.StackTrace)
                        : string.Empty);
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
                    try
                    {
                        this.Parent.OnError(this.Parent, reason);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.LogError(
                            $"[OverHTTP2] [method:OnInternalRequestCallback] [msg|Exception] OnError  Exception:{ex}");
#endif
                    }
                }
                else if (!HttpManager.IsQuitting)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    var sb = new StringBuilder(3);
                    sb.Append("[OverHTTP2] ");
                    sb.Append("[method:OnInternalRequestCallback] ");
                    sb.Append($"[msg] Unknown negotiated protocol: ");
                    sb.Append($"{reason}");
                    Debug.LogError(sb.ToString());
#endif
                }
            }
            else if (this.Parent.OnClosed != null)
            {
                try
                {
                    this.Parent.OnClosed(this.Parent, (ushort)WebSocketStausCodes.NormalClosure,
                        "Closed while opening");
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError(
                        $"[OverHTTP2] [method:OnInternalRequestCallback] [msg|Exception] OnClosed  Exception:{ex}");
#endif
                }
            }

            this.State = WebSocketStates.Closed;
        }

        public override void StartOpen()
        {
            if (this.Parent.Extensions != null)
            {
                try
                {
                    for (int i = 0; i < this.Parent.Extensions.Length; ++i)
                    {
                        var ext = this.Parent.Extensions[i];
                        if (ext != null)
                            ext.AddNegotiation(base.InternalRequest);
                    }
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError(
                        $"[OverHTTP2] [method:StartOpen] [msg|Exception] Open  Exception:{ex}");
#endif
                }
            }

            base.InternalRequest.Send();
            HttpManager.Heartbeats.Subscribe(this);
            HttpUpdateDelegator.OnApplicationForegroundStateChanged += OnApplicationForegroundStateChanged;

            this.State = WebSocketStates.Connecting;
        }

        public override void StartClose(ushort code, string message)
        {
            if (this.State == WebSocketStates.Connecting)
            {
                if (this.InternalRequest != null)
                    this.InternalRequest.Abort();

                this.State = WebSocketStates.Closed;
                if (this.Parent.OnClosed != null)
                    this.Parent.OnClosed(this.Parent, code, message);
            }
            else
            {
                Send(new WebSocketFrame(this.Parent, WebSocketFrameTypes.ConnectionClose,
                    WebSocket.EncodeCloseData(code, message)));
                this.State = WebSocketStates.Closing;
            }
        }

        public override void Send(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message must not be null!");

            int count = System.Text.Encoding.UTF8.GetByteCount(message);
            byte[] data = BufferPool.Get(count, true);
            System.Text.Encoding.UTF8.GetBytes(message, 0, message.Length, data, 0);

            var frame = new WebSocketFrame(this.Parent, WebSocketFrameTypes.Text, data, 0, (ulong)count, true, true);

            var maxFrameSize = this._http2Handler.Settings.RemoteSettings[Http2Settings.MaxFrameSize];
            if (frame.Data != null && frame.Data.Length > maxFrameSize)
            {
                WebSocketFrame[] additionalFrames = frame.Fragment(maxFrameSize);

                Send(frame);
                if (additionalFrames != null)
                    for (int i = 0; i < additionalFrames.Length; ++i)
                        Send(additionalFrames[i]);
            }
            else
                Send(frame);

            BufferPool.Release(data);
        }

        public override void Send(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("data must not be null!");

            WebSocketFrame frame = new WebSocketFrame(this.Parent, WebSocketFrameTypes.Binary, buffer);

            var maxFrameSize = this._http2Handler.Settings.RemoteSettings[Http2Settings.MaxFrameSize];
            if (frame.Data != null && frame.Data.Length > maxFrameSize)
            {
                WebSocketFrame[] additionalFrames = frame.Fragment(maxFrameSize);

                Send(frame);
                if (additionalFrames != null)
                    for (int i = 0; i < additionalFrames.Length; ++i)
                        Send(additionalFrames[i]);
            }
            else
                Send(frame);
        }

        public override void Send(byte[] data, ulong offset, ulong count)
        {
            if (data == null)
                throw new ArgumentNullException("data must not be null!");
            if (offset + count > (ulong)data.Length)
                throw new ArgumentOutOfRangeException("offset + count >= data.Length");

            WebSocketFrame frame =
                new WebSocketFrame(this.Parent, WebSocketFrameTypes.Binary, data, offset, count, true, true);

            var maxFrameSize = this._http2Handler.Settings.RemoteSettings[Http2Settings.MaxFrameSize];
            if (frame.Data != null && frame.Data.Length > maxFrameSize)
            {
                WebSocketFrame[] additionalFrames = frame.Fragment(maxFrameSize);

                Send(frame);

                if (additionalFrames != null)
                    for (int i = 0; i < additionalFrames.Length; ++i)
                        Send(additionalFrames[i]);
            }
            else
                Send(frame);
        }

        public override void Send(WebSocketFrame frame)
        {
            if (frame == null)
                throw new ArgumentNullException("frame is null!");

            if (this.upStream.IsClosed || _closeSent)
                return;

            var frameData = frame.Get();
            this.upStream.Write(new BufferSegment(frameData.Data, 0, frameData.Length));
            this._http2Handler.SignalRunnerThread();

            frameData.Data = null;

            if (frame.Type == WebSocketFrameTypes.ConnectionClose)
                this._closeSent = true;
        }

        private int CalculateLatency()
        {
            if (this.rtts.Count == 0)
                return 0;

            int sumLatency = 0;
            for (int i = 0; i < this.rtts.Count; ++i)
                sumLatency += this.rtts[i];

            return sumLatency / this.rtts.Count;
        }

        private void OnApplicationForegroundStateChanged(bool isPaused)
        {
            if (!isPaused)
                base.LastMessageReceived = DateTime.Now;
        }

        private void SendPing()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[OverHTTP2] [SendPing] [msg] Sending Ping frame, waiting for a pong...");
#endif
            lastPing = DateTime.Now;
            waitingForPong = true;

            long ticks = DateTime.Now.Ticks;
            var ticksBytes = BitConverter.GetBytes(ticks);

            var pingFrame = new WebSocketFrame(this.Parent, WebSocketFrameTypes.Ping, ticksBytes);

            Send(pingFrame);
        }

        private void CloseWithError(string message)
        {
            this.State = WebSocketStates.Closed;
            this.upStream.Close();

            if (this.Parent.OnError != null)
            {
                try
                {
                    this.Parent.OnError(this.Parent, message);
                }
                catch (Exception ex)
                {
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        var sb = new StringBuilder(3);
                        sb.Append("[WebSocketResponse] ");
                        sb.Append("[method:CloseWithError] ");
                        sb.Append($"[msg|Exception] OnError [Exception] {ex}");
                        Debug.LogError(sb.ToString());
#endif
                    }
                }
            }

            this.InternalRequest.Abort();
        }
    }
}
#endif
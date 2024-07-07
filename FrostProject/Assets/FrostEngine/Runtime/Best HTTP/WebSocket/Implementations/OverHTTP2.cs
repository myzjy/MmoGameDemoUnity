#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2 && !BESTHTTP_DISABLE_WEBSOCKET
using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP.Connections.HTTP2;
using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.Memory;
using BestHTTP.WebSocket.Frames;
using BestHTTP.WebSocket.Implementations.Utils;
using FrostEngine;

namespace BestHTTP.WebSocket
{
    /// <summary>
    /// 实现RFC 8441 (https://tools.ietf.org/html/rfc8441)在HTTP/2上使用Websocket
    /// </summary>
    public sealed class OverHttp2 : WebSocketBaseImplementation, IHeartbeat
    {
        /// <summary>
        /// 如果向服务器发送Close消息，则为True
        /// </summary>
        private volatile bool _closeSent;

        private readonly Http2Handler _http2Handler;

        private readonly PeekableIncomingSegmentStream _incomingSegmentStream = new PeekableIncomingSegmentStream();

        private readonly List<WebSocketFrameReader> _incompleteFrames = new List<WebSocketFrameReader>();

        /// <summary>
        /// 当我们发出最后一个信号时。
        /// </summary>
        private DateTime _lastPing = DateTime.MinValue;

        /// <summary>
        /// 一个循环缓冲区，用于存储pong消息计算的最后N个rtt时间。
        /// </summary>
        private readonly CircularBuffer<int> _rtt = new CircularBuffer<int>(WebSocketResponse.RTTBufferCapacity);

        private LockedBufferSegmentStream _upStream;

        private bool _waitingForPong;

        public OverHttp2(
            WebSocket parent,
            Http2Handler handler,
            Uri uri,
            string origin,
            string protocol)
            : base(
                parent: parent,
                uri: uri,
                origin: origin,
                protocol: protocol)
        {
            this._http2Handler = handler;

            var scheme = "https";
            var port = uri.Port != -1 ? uri.Port : 443;

            Uri = new Uri(scheme + "://" + uri.Host + ":" + port + uri.GetRequestPathAndQueryURL());
        }

        public override int BufferedAmount => (int)this._upStream.Length;

        public override bool IsOpen => this.State == WebSocketStates.Open;

        public override int Latency => this.Parent.StartPingThread ? base.Latency : (int)this._http2Handler.Latency;

        public void OnHeartbeatUpdate(TimeSpan dif)
        {
            DateTime now = DateTime.Now;

            switch (this.State)
            {
                case WebSocketStates.Connecting:
                {
                    if (now - this.InternalRequest.Timing.Start >= this.Parent.CloseAfterNoMessage)
                    {
                        if (HttpManager.Http2Settings.WebSocketOverHttp2Settings.EnableImplementationFallback)
                        {
                            this.State = WebSocketStates.Closed;
                            this.InternalRequest.OnHeadersReceived = null;
                            this.InternalRequest.Callback = null;
                            this.Parent.FallbackToHttp1();
                        }
                        else
                        {
                            CloseWithError("WebSocket Over HTTP/2实现在给定时间内连接失败!");
                        }
                    }
                }
                    break;

                case WebSocketStates.Open:
                {
                    if (this.Parent.StartPingThread)
                    {
                        if (!_waitingForPong
                            && now - LastMessageReceived >=
                            TimeSpan.FromMilliseconds(this.Parent.PingFrequency))
                        {
                            SendPing();
                        }

                        if (_waitingForPong && now - _lastPing > this.Parent.CloseAfterNoMessage)
                        {
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                StringBuilder sb = new StringBuilder();
                                sb.Append("[OverHTTP2] ");
                                sb.Append("[method: OnHeartbeatUpdate] ");
                                sb.Append("[msg|Exception] ");
                                sb.Append($"在给定的时间内没有收到任何消息!");
                                sb.Append($" Closing WebSocket. LastPing: {this._lastPing}");
                                sb.Append($", PingFrequency: {TimeSpan.FromMilliseconds(this.Parent.PingFrequency)}");
                                sb.Append($", Close After: {this.Parent.CloseAfterNoMessage},");
                                sb.Append($" Now: {now}");
                                Debug.Log(sb.ToString());
#endif
                            }

                            CloseWithError("在给定的时间内没有收到任何消息!");
                        }
                    }
                }
                    break;

                case WebSocketStates.Closed:
                {
                    HttpManager.Heartbeats.Unsubscribe(this);
                    HttpUpdateDelegator.OnApplicationForegroundStateChanged -= OnApplicationForegroundStateChanged;
                }
                    break;
            }
        }

        protected override void CreateInternalRequest()
        {
            SetInternalRequest = new HttpRequest(Uri, HttpMethods.Connect, OnInternalRequestCallback);
            SetInternalRequest.Context.Add("WebSocket", this.Parent.Context);

            SetInternalRequest.SetHeader(":protocol", "websocket");

            //请求必须包含一个头字段，名称为|Sec-WebSocket-Key|。这个报头字段的值必须是一个由
            //随机选择的base64编码的16字节值(参见[RFC4648]的第4节)。nonce必须为每个连接随机选择。
            SetInternalRequest.SetHeader("sec-webSocket-key",
                WebSocket.GetSecKey(new[] { this, InternalRequest, Uri, new object() }));

            //如果请求来自浏览器客户端，请求必须包含一个头字段|Origin| [RFC6454]
            //如果连接来自非浏览器客户端，如果该客户端语义与这里描述的浏览器客户端用例匹配，则请求可以包含此报头字段。
            //更多关于产地的考虑: http://tools.ietf.org/html/rfc6455#section-10.2
            if (!string.IsNullOrEmpty(Origin))
            {
                SetInternalRequest.SetHeader("origin", Origin);
            }

            // 请求必须包含一个名为|Sec-WebSocket-Version|的报头字段。这个报头字段的值必须是13。
            SetInternalRequest.SetHeader("sec-webSocket-version", "13");

            if (!string.IsNullOrEmpty(Protocol))
            {
                SetInternalRequest.SetHeader("sec-webSocket-protocol", Protocol);
            }

            // Disable caching
            SetInternalRequest.SetHeader("cache-control", "no-cache");
            SetInternalRequest.SetHeader("pragma", "no-cache");

#if !BESTHTTP_DISABLE_CACHING
            SetInternalRequest.DisableCache = true;
#endif

            SetInternalRequest.OnHeadersReceived += OnHeadersReceived;

            SetInternalRequest.OnStreamingData += OnFrame;
            SetInternalRequest.StreamChunksImmediately = true;

            SetInternalRequest.UploadStream = this._upStream = new LockedBufferSegmentStream();

            SetInternalRequest.UseUploadStreamLength = false;

            if (this.Parent.OnInternalRequestCreated == null) return;
            try
            {
                this.Parent.OnInternalRequestCreated(this.Parent, SetInternalRequest);
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.LogError(
                    $"[OverHTTP2] [method:CreateInternalRequest] [msg|Exception] CreateInternalRequest  Exception:{ex}");
#endif
            }
        }

        private void OnHeadersReceived(
            HttpRequest req,
            HttpResponse resp,
            Dictionary<string, List<string>> newHeaders)
        {
            if (resp is { StatusCode: 200 })
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

                if (!this.Parent.StartPingThread) return;
                this.LastMessageReceived = DateTime.Now;
                SendPing();
            }
            else
            {
                req.Abort();
            }
        }

        private static bool CanReadFullFrame(PeekableIncomingSegmentStream stream)
        {
            if (stream.Length < 2)
            {
                return false;
            }

            stream.BeginPeek();

            if (stream.PeekByte() == -1)
            {
                return false;
            }

            int maskAndLength = stream.PeekByte();
            if (maskAndLength == -1)
            {
                return false;
            }

            // 第二个字节是掩码位和有效负载数据的长度
            var hasMask = (maskAndLength & 0x80) != 0;

            // 如果0-125，这是有效载荷长度。
            var length = (UInt64)(maskAndLength & 127);

            switch (length)
            {
                //如果是126，下面2个被解释为16位无符号整数的字节就是有效负载长度。
                case 126:
                {
                    var rawLen = BufferPool.Get(2, true);

                    for (var i = 0; i < 2; i++)
                    {
                        var data = stream.PeekByte();
                        if (data < 0)
                        {
                            return false;
                        }

                        rawLen[i] = (byte)data;
                    }

                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(rawLen, 0, 2);

                    length = BitConverter.ToUInt16(rawLen, 0);

                    BufferPool.Release(rawLen);
                    break;
                }
                case 127:
                {
                    //如果127，下面的8个字节解释为一个64位无符号整数
                    //最有效位必须为0)是有效载荷长度。
                    byte[] rawLen = BufferPool.Get(8, true);

                    for (int i = 0; i < 8; i++)
                    {
                        int data = stream.PeekByte();
                        if (data < 0)
                        {
                            return false;
                        }

                        rawLen[i] = (byte)data;
                    }

                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(rawLen, 0, 8);

                    length = BitConverter.ToUInt64(rawLen, 0);

                    BufferPool.Release(rawLen);
                    break;
                }
            }

            // Header + Mask&Length
            length += 2;

            // 如果存在掩码，则为4字节
            if (hasMask)
            {
                length += 4;
            }

            return stream.Length >= (long)length;
        }

        private bool OnFrame(
            HttpRequest request,
            HttpResponse response,
            byte[] dataFragment,
            int dataFragmentLength)
        {
            LastMessageReceived = DateTime.Now;

            this._incomingSegmentStream.Write(
                buffer: dataFragment,
                offset: 0,
                count: dataFragmentLength);

            while (CanReadFullFrame(this._incomingSegmentStream))
            {
                var frame = new WebSocketFrameReader();
                frame.Read(this._incomingSegmentStream);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[OverHTTP2] [method:OnFrame] [msg] Frame received: {frame.Type}");
#endif
                if (!frame.IsFinal)
                {
                    if (this.Parent.OnIncompleteFrame == null)
                    {
                        _incompleteFrames.Add(frame);
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
                    // 有关碎片的完整文档和规则，请参阅 http://tools.ietf.org/html/rfc6455#section-5.4
                    // 一个碎片帧的最后一个片段的操作码是0(延续)，FIN位设置为1。
                    case WebSocketFrameTypes.Continuation:
                        //只有在OnFragment未设置时才进行组装传递。否则，把它放在CompletedFrames中，我们将在HandleEvent阶段处理它。
                        if (this.Parent.OnIncompleteFrame == null)
                        {
                            frame.Assemble(_incompleteFrames);

                            // 删除所有不完整的帧
                            _incompleteFrames.Clear();

                            // 控制帧本身一定不能碎片化。所以，它是一个正常的文本或二进制帧。去吧，像往常一样处理.
                            //goto case WebSocketFrameTypes.Binary;
                            if (frame.Type == WebSocketFrameTypes.Text)
                            {
                                goto case WebSocketFrameTypes.Text;
                            }
                            else if (frame.Type == WebSocketFrameTypes.Binary)
                            {
                                goto case WebSocketFrameTypes.Binary;
                            }
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
                        if (!_closeSent && !this._upStream.IsClosed)
                            Send(new WebSocketFrame(this.Parent, WebSocketFrameTypes.Pong, frame.Data));
                        break;

                    case WebSocketFrameTypes.Pong:
                        _waitingForPong = false;

                        try
                        {
                            // Get the ticks from the frame's payload
                            long ticksSent = BitConverter.ToInt64(frame.Data, 0);

                            // the difference between the current time and the time when the ping message is sent
                            TimeSpan diff = TimeSpan.FromTicks(this.LastMessageReceived.Ticks - ticksSent);

                            // add it to the buffer
                            this._rtt.Add((int)diff.TotalMilliseconds);

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
                        this._upStream.Close();

                        UInt16 statusCode = 0;
                        string msg = string.Empty;
                        try
                        {
                            // If we received any data, we will get the status code and the message from it
                            if (frame is { Data: not null, Length: >= 2 })
                            {
                                if (BitConverter.IsLittleEndian)
                                    Array.Reverse(frame.Data, 0, 2);
                                statusCode = BitConverter.ToUInt16(frame.Data, 0);

                                if (frame.Data.Length > 2)
                                    msg = Encoding.UTF8.GetString(frame.Data, 2, (int)frame.Length - 2);

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
                this.Parent.FallbackToHttp1();
                return;
            }

            string reason;

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
                        $"[OverHTTP2] [OnInternalRequestCallback] [msg]{sb}");
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
                    foreach (var ext in this.Parent.Extensions)
                    {
                        if (ext != null)
                            ext.AddNegotiation(InternalRequest);
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

            InternalRequest.Send();
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
                throw new ArgumentNullException(nameof(message));

            int count = Encoding.UTF8.GetByteCount(message);
            byte[] data = BufferPool.Get(count, true);
            Encoding.UTF8.GetBytes(message, 0, message.Length, data, 0);

            var frame = new WebSocketFrame(this.Parent, WebSocketFrameTypes.Text, data, 0, (ulong)count, true, true);

            var maxFrameSize = this._http2Handler.Settings.RemoteSettings[Http2Settings.MaxFrameSize];
            if (frame.Data != null && frame.Data.Length > maxFrameSize)
            {
                WebSocketFrame[] additionalFrames = frame.Fragment(maxFrameSize);

                Send(frame);
                if (additionalFrames != null)
                {
                    foreach (var t in additionalFrames)
                    {
                        Send(t);
                    }
                }
            }
            else
            {
                Send(frame);
            }

            BufferPool.Release(data);
        }

        public override void Send(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            WebSocketFrame frame = new WebSocketFrame(this.Parent, WebSocketFrameTypes.Binary, buffer);

            var maxFrameSize = this._http2Handler.Settings.RemoteSettings[Http2Settings.MaxFrameSize];
            if (frame.Data != null && frame.Data.Length > maxFrameSize)
            {
                WebSocketFrame[] additionalFrames = frame.Fragment(maxFrameSize);

                Send(frame);
                if (additionalFrames == null) return;
                foreach (var t in additionalFrames)
                {
                    Send(t);
                }
            }
            else
                Send(frame);
        }

        public override void Send(byte[] data, ulong offset, ulong count)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (offset + count > (ulong)data.Length)
                throw new ArgumentOutOfRangeException(nameof(data));

            WebSocketFrame frame =
                new WebSocketFrame(this.Parent, WebSocketFrameTypes.Binary, data, offset, count, true, true);

            var maxFrameSize = this._http2Handler.Settings.RemoteSettings[Http2Settings.MaxFrameSize];
            if (frame.Data != null && frame.Data.Length > maxFrameSize)
            {
                WebSocketFrame[] additionalFrames = frame.Fragment(maxFrameSize);

                Send(frame);

                if (additionalFrames == null) return;
                foreach (var t in additionalFrames)
                {
                    Send(t);
                }
            }
            else
                Send(frame);
        }

        public override void Send(WebSocketFrame frame)
        {
            if (frame == null)
                throw new ArgumentNullException(nameof(frame));

            if (this._upStream.IsClosed || _closeSent)
                return;

            var frameData = frame.Get();
            this._upStream.Write(new BufferSegment(frameData.Data, 0, frameData.Length));
            this._http2Handler.SignalRunnerThread();

            frameData.Data = null;

            if (frame.Type == WebSocketFrameTypes.ConnectionClose)
                this._closeSent = true;
        }

        private int CalculateLatency()
        {
            if (this._rtt.Count == 0)
                return 0;

            int sumLatency = 0;
            for (int i = 0; i < this._rtt.Count; ++i)
                sumLatency += this._rtt[i];

            return sumLatency / this._rtt.Count;
        }

        private void OnApplicationForegroundStateChanged(bool isPaused)
        {
            if (!isPaused)
                LastMessageReceived = DateTime.Now;
        }

        private void SendPing()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[OverHTTP2] [SendPing] [msg] Sending Ping frame, waiting for a pong...");
#endif
            _lastPing = DateTime.Now;
            _waitingForPong = true;

            long ticks = DateTime.Now.Ticks;
            var ticksBytes = BitConverter.GetBytes(ticks);

            var pingFrame = new WebSocketFrame(this.Parent, WebSocketFrameTypes.Ping, ticksBytes);

            Send(pingFrame);
        }

        private void CloseWithError(string message)
        {
            this.State = WebSocketStates.Closed;
            this._upStream.Close();

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
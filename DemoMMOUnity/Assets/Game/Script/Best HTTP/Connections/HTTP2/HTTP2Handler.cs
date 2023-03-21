#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BestHTTP.Core;
using BestHTTP.Extensions;
using BestHTTP.Logger;
using BestHTTP.PlatformSupport.Memory;

// ReSharper disable once CheckNamespace
namespace BestHTTP.Connections.HTTP2
{
    public sealed class Http2Handler : IHttpRequestHandler
    {
        public const UInt32 MaxValueFor31Bits = 0xFFFFFFFF >> 1;

        // Connection preface starts with the string PRI * HTTP/2.0\r\n\r\nSM\r\n\r\n).
        private static readonly byte[] Magic = new byte[]
        {
            0x50,
            0x52,
            0x49,
            0x20,
            0x2a,
            0x20,
            0x48,
            0x54,
            0x54,
            0x50,
            0x2f,
            0x32,
            0x2e,
            0x30,
            0x0d,
            0x0a,
            0x0d,
            0x0a,
            0x53,
            0x4d,
            0x0d,
            0x0a,
            0x0d,
            0x0a
        };

        private static readonly int RTTBufferCapacity = 5;

        private readonly List<HTTP2Stream> _clientInitiatedStreams = new List<HTTP2Stream>();

        private readonly HTTPConnection _conn;

        private
            readonly
            ConcurrentQueue<Http2FrameHeaderAndPayload> _newFrames =
                new ConcurrentQueue<Http2FrameHeaderAndPayload>();

        private
            readonly
            List<Http2FrameHeaderAndPayload> _outgoingFrames = new List<Http2FrameHeaderAndPayload>();

        private readonly ConcurrentQueue<HttpRequest> _requestQueue = new ConcurrentQueue<HttpRequest>();

        // ReSharper disable once IdentifierTypo
        private readonly CircularBuffer<double> _rtts = new CircularBuffer<double>(RTTBufferCapacity);

        public readonly Http2SettingsManager Settings;
        private DateTime _goAwaySentAt = DateTime.MaxValue;
        private HttpPackEncoder _httpPackEncoder;

        private volatile bool _isRunning;
        private DateTime _lastInteraction;

        private DateTime _lastPingSent = DateTime.MinValue;

        // https://httpwg.org/specs/rfc7540.html#StreamIdentifiers
        // 由客户端发起的流必须使用奇数的流标识符，初始值为-1，第一个客户端发起的流的id将为1。
        private long _lastStreamId = -1;

        private AutoResetEvent _newFrameSignal = new AutoResetEvent(false);

        // 将在RunHandler中被覆盖
        private TimeSpan _pingFrequency = TimeSpan.MaxValue;

        private UInt32 _remoteWindow;
        private int _threadExitCount;
        private int _waitingForPingAck;

        public Http2Handler(HTTPConnection conn)
        {
            this.Context = new LoggingContext(this);

            this._conn = conn;
            this._isRunning = true;

            this.Settings = new Http2SettingsManager(this);

            Process(this._conn.CurrentRequest);
        }

        public double Latency { get; private set; }

        private TimeSpan MaxGoAwayWaitTime =>
            this._goAwaySentAt == DateTime.MaxValue
                ? TimeSpan.MaxValue
                : TimeSpan.FromMilliseconds(Math.Max(this.Latency * 2.5, 1500));

        public bool HasCustomRequestProcessor => true;

        public KeepAliveHeader KeepAlive => null;

        public bool CanProcessMultiple => this._goAwaySentAt == DateTime.MaxValue && this._isRunning;

        public LoggingContext Context { get; private set; }

        public void Process(HttpRequest request)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.LogError($"[HTTP2Handler] 流程请求被调用");
#endif
            request.QueuedAt = DateTime.MinValue;
            request.ProcessingStarted = this._lastInteraction = DateTime.UtcNow;

            this._requestQueue.Enqueue(request);

            // Wee可能会将请求添加到死队列中，发送信号是毫无意义的。
            // 当ConnectionEventHelper处理关闭状态更改事件时，队列中的请求将被重新发送。
            // (我们现在应该避免重新发送请求，因为它仍然可能选择这个连接/处理程序，从而导致无限循环。)
            if (Volatile.Read(ref this._threadExitCount) == 0)
            {
                this._newFrameSignal.Set();
            }
        }

        public void RunHandler()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[HTTP2Handler] [method:RunHandler()] 处理线程启动并运行!");
#endif
            Thread.CurrentThread.Name = "BestHTTP.HTTP2 Process";

            PlatformSupport.Threading.ThreadedRunner.RunLongLiving(ReadThread);

            try
            {
                bool atLeastOneStreamHasAFrameToSend = true;

                this._httpPackEncoder = new HttpPackEncoder(this, this.Settings);

                // https://httpwg.org/specs/rfc7540.html#InitialWindowSize
                // The connection flow-control window is also 65,535 octets.
                this._remoteWindow = this.Settings.RemoteSettings[Http2Settings.InitialWindowSize];

                //我们希望在一个TCP段中包装尽可能多的数据，但是将缓冲区的大小设置得太高可能会使数据保持太长时间，并以爆发的方式发送数据，而不是以稳定的流发送数据。
                // 保持太低可能会导致一个完整的tcp段和一个非常低的有效负载。一个完整的tcp段大小的缓冲区可能是最好的，或它的多个。
                // 它将保持网络繁忙，没有任何碎片。以太网层最大1500字节，
                // 但是有两层20字节的报头，所以理论上最大是1500-20-20字节。
                // 另一方面，如果缓冲区很小(1-2)，这意味着对于较大的数据，我们必须进行大量的系统调用，在这种情况下，较大的缓冲区可能更好。不过，如果我们不受cpu限制，
                // 一个饱和的网络可能会更好地为我们服务。
                using var bufferedStream =
                    new WriteOnlyBufferedStream(
                        stream: this._conn.connector.Stream,
                        bufferSize: 1024 * 1024 /*1500 - 20 - 20*/);
                // 客户端连接序言以一个24字节的序列开始
                {
                    bufferedStream.Write(
                        bufferFrom: Magic,
                        offset: 0,
                        count: Magic.Length);
                }

                // 列后面必须有一个设置帧(Section 6.5)，设置帧可以是空的。
                // 客户端在收到101(交换协议)响应(表示升级成功)或作为TLS连接的第一个应用程序数据字节时立即发送客户端连接序言

                // 将流的初始窗口大小设置为最大
                this.Settings.InitiatedMySettings[Http2Settings.InitialWindowSize] =
                    HttpManager.Http2Settings.InitialStreamWindowSize;
                this.Settings.InitiatedMySettings[Http2Settings.MaxConcurrentStreams] =
                    HttpManager.Http2Settings.MaxConcurrentStreams;
                this.Settings.InitiatedMySettings[Http2Settings.EnableConnectProtocol] =
                    (uint)(HttpManager.Http2Settings.EnableConnectProtocol ? 1 : 0);
                this.Settings.InitiatedMySettings[Http2Settings.EnablePush] = 0;
                this.Settings.SendChanges(this._outgoingFrames);
                this.Settings.RemoteSettings.OnSettingChangedEvent += OnRemoteSettingChanged;

                // 整个连接的默认窗口大小是65535字节，
                // 但我们希望将其设置为可能的最大值。
                Int64 initialConnectionWindowSize = HttpManager.Http2Settings.InitialConnectionWindowSize;

                // 当插件试图将连接窗口设置为2^31 - 1时，yandex.ru返回一个FLOW_CONTROL_ERROR(3)错误，
                // 并且只能在2^31 - 10Mib(10 * 1024 * 1024)的最大值下工作。
                if (initialConnectionWindowSize == Http2Handler.MaxValueFor31Bits)
                {
                    initialConnectionWindowSize -= 10 * 1024 * 1024;
                }

                long diff = initialConnectionWindowSize - 65535;
                if (diff > 0)
                {
                    this._outgoingFrames.Add(Http2FrameHelper.CreateWindowUpdateFrame(0, (uint)diff));
                }

                this._pingFrequency = HttpManager.Http2Settings.PingFrequency;

                while (this._isRunning)
                {
                    DateTime now = DateTime.UtcNow;

                    if (!atLeastOneStreamHasAFrameToSend)
                    {
                        // 如果内部缓冲区已满，缓冲流将自动调用flush。
                        // But we have to make it sure that we flush remaining data before we go to sleep.
                        bufferedStream.Flush();

                        // 等待，直到我们必须发送下一个ping，或者在读线程上收到一个新的帧。
                        //                lastPingSent             Now           lastPingSent+frequency       lastPingSent+Ping timeout
                        //----|---------------------|---------------|----------------------|----------------------|------------|
                        // lastInteraction                                                                                    lastInteraction + MaxIdleTime

                        var sendPingAt = this._lastPingSent + this._pingFrequency;
                        var timeoutAt = this._lastPingSent + HttpManager.Http2Settings.Timeout;
                        var nextPingInteraction = sendPingAt < timeoutAt ? sendPingAt : timeoutAt;

                        var disconnectByIdleAt = this._lastInteraction + HttpManager.Http2Settings.MaxIdleTime;

                        var nextDueClientInteractionAt = nextPingInteraction < disconnectByIdleAt
                            ? nextPingInteraction
                            : disconnectByIdleAt;
                        int wait = (int)(nextDueClientInteractionAt - now).TotalMilliseconds;

                        wait = (int)Math.Min(wait, this.MaxGoAwayWaitTime.TotalMilliseconds);

                        if (wait >= 1)
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.Log($"[HTTP2Handler] [method:RunHandler()] 休眠{wait:N0}ms");
#endif
                            this._newFrameSignal.WaitOne(wait);

                            now = DateTime.UtcNow;
                        }
                    }

                    //  不要发送一个新的ping，直到没有收到一个pong的最后一个
                    if (now - this._lastPingSent >= this._pingFrequency &&
                        Interlocked.CompareExchange(ref this._waitingForPingAck, 1, 0) == 0)
                    {
                        this._lastPingSent = now;

                        var frame = Http2FrameHelper.CreatePingFrame();
                        BufferHelper.SetLong(frame.Payload, 0, now.Ticks);

                        this._outgoingFrames.Add(frame);
                    }

                    //  如果在(可配置的)合理时间内没有收到pong，则认为连接中断
                    if (this._waitingForPingAck != 0 &&
                        now - this._lastPingSent >= HttpManager.Http2Settings.Timeout)
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.LogError($"[HTTP2Handler] [method:RunHandler()] Ping ACK未及时收到!");
#endif
                        throw new TimeoutException("Ping ACK未及时收到!");
                    }

                    // 处理接收的帧
                    while (this._newFrames.TryDequeue(out var header))
                    {
                        if (header.StreamId > 0)
                        {
                            var http2Stream = FindStreamById(header.StreamId);

                            // 将帧添加到流中，以便在调用process函数时可以处理它
                            if (http2Stream != null)
                            {
                                http2Stream.AddFrame(header, this._outgoingFrames);
                            }
                            else
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                // 错误呢?当服务器正在发送帧时，我们可能关闭并删除了流
                                Debug.LogWarning(
                                    $"[HTTP2Handler] [method:RunHandler()] id: {header.StreamId}没有找到流!不能传递帧:{header}");
#endif
                            }
                        }
                        else
                        {
                            switch (header.Type)
                            {
                                case Http2FrameTypes.Settings:
                                {
                                    this.Settings.Process(header, this._outgoingFrames);
                                    var http2ConnectEnabled =
                                        this.Settings.MySettings[Http2Settings.EnableConnectProtocol] ==
                                        1 &&
                                        this.Settings.RemoteSettings[Http2Settings.EnableConnectProtocol] == 1;
                                    var http2Connect = new Http2ConnectProtocolInfo(
                                        host: this._conn.LastProcessedUri.Host,
                                        enabled: http2ConnectEnabled);
                                    var pluginEventInfo = new PluginEventInfo(
                                        @event: PluginEvents.Http2ConnectProtocol,
                                        payload: http2Connect);
                                    PluginEventHelper.EnqueuePluginEvent(pluginEventInfo);
                                }
                                    break;

                                case Http2FrameTypes.Ping:
                                {
                                    var pingFrame = Http2FrameHelper.ReadPingFrame(header);

                                    // https://httpwg.org/specs/rfc7540.html#PING
                                    // 如果它不是我们ping的ack，我们必须发送一个
                                    if ((pingFrame.Flags & Http2PingFlags.Ack) == 0)
                                    {
                                        var frame = Http2FrameHelper.CreatePingFrame(Http2PingFlags.Ack);
                                        Array.Copy(
                                            sourceArray: pingFrame.OpaqueData,
                                            sourceIndex: 0,
                                            destinationArray: frame.Payload,
                                            destinationIndex: 0,
                                            length: pingFrame.OpaqueDataLength);

                                        this._outgoingFrames.Add(frame);
                                    }
                                }
                                    break;
                                case Http2FrameTypes.WindowUpdate:
                                {
                                    var windowUpdateFrame = Http2FrameHelper.ReadWindowUpdateFrame(header);
                                    this._remoteWindow += windowUpdateFrame.WindowSizeIncrement;
                                }
                                    break;

                                case Http2FrameTypes.Goaway:
                                {
                                    //解析框架，这样我们就可以打印出详细的信息
                                    Http2GoAwayFrame goAwayFrame = Http2FrameHelper.ReadGoAwayFrame(header);

#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                    Debug.Log(
                                        $"[HTTP2Handler] [method:RunHandler()] Received GOAWAY frame: {goAwayFrame.ToString()}");
#endif
                                    var msg = $"服务器关闭连接!错误代码: {goAwayFrame.Error} ({goAwayFrame.ErrorCode})";
                                    foreach (var t in this._clientInitiatedStreams)
                                    {
                                        t.Abort(msg);
                                    }

                                    this._clientInitiatedStreams.Clear();

                                    // 将运行标志设置为false，这样线程就可以退出
                                    this._isRunning = false;

                                    this._conn.State = HttpConnectionStates.Closed;
                                }
                                    break;

                                case Http2FrameTypes.AltSvc:
                                    //HTTP2AltSVCFrame altSvcFrame = HTTP2FrameHelper.ReadAltSvcFrame(header);

                                    // Implement
                                    //HTTPManager.EnqueuePluginEvent(new PluginEventInfo(PluginEvents.AltSvcHeader, new AltSvcEventInfo(altSvcFrame.Origin, ))
                                    break;
                            }

                            if (header.Payload != null)
                            {
                                BufferPool.Release(header.Payload);
                            }
                        }
                    }

                    UInt32 maxConcurrentStreams = Math.Min(
                        HttpManager.Http2Settings.MaxConcurrentStreams,
                        this.Settings.RemoteSettings[Http2Settings.MaxConcurrentStreams]);

                    // 预测试流计数仅在真正需要时才锁定。
                    if (this._clientInitiatedStreams.Count < maxConcurrentStreams && this._isRunning)
                    {
                        // grab requests from queue
                        while (this._clientInitiatedStreams.Count < maxConcurrentStreams &&
                               this._requestQueue.TryDequeue(out var request))
                        {
                            // create a new stream
                            var newStream = new HTTP2Stream((UInt32)Interlocked.Add(ref _lastStreamId, 2), this,
                                this.Settings, this._httpPackEncoder);

                            // process the request
                            newStream.Assign(request);

                            this._clientInitiatedStreams.Add(newStream);
                        }
                    }

                    // send any settings changes
                    this.Settings.SendChanges(this._outgoingFrames);

                    atLeastOneStreamHasAFrameToSend = false;

                    // 处理其他流
                    // 流应该根据优先级来处理!
                    for (int i = 0; i < this._clientInitiatedStreams.Count; ++i)
                    {
                        var stream = this._clientInitiatedStreams[i];
                        stream.Process(this._outgoingFrames);

                        // 删除关闭的空流(不足以检查关闭标志，关闭的流仍然可以包含要发送的帧)
                        if (stream.State == Http2StreamStates.Closed && !stream.HasFrameToSend)
                        {
                            this._clientInitiatedStreams.RemoveAt(i--);
                            stream.Removed();
                        }

                        atLeastOneStreamHasAFrameToSend |= stream.HasFrameToSend;

                        this._lastInteraction = DateTime.UtcNow;
                    }

                    // 如果我们遇到一个对当前远程窗口来说太大的数据帧，我们必须停止发送所有的数据帧，因为我们可以在大的数据帧之前发送小的数据帧。
                    // 改进空间:这里的改进是停止每个流发送数据帧。
                    bool haltDataSending = false;

                    switch (this.ShutdownType)
                    {
                        case ShutdownTypes.Running
                            when now - this._lastInteraction >= HttpManager.Http2Settings.MaxIdleTime:
                        {
                            this._lastInteraction = DateTime.UtcNow;
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.Log($"[HTTP2Handler] [method:RunHandler()] 到达空闲时间，发送超时帧! ");
#endif
                            this._outgoingFrames.Add(Http2FrameHelper.CreateGoAwayFrame(0, Http2ErrorCodes.NoError));
                            this._goAwaySentAt = DateTime.UtcNow;
                        }

                            break;
                        // https://httpwg.org/specs/rfc7540.html#GOAWAY
                        // 端点在关闭连接之前应该总是发送一个超时帧，这样远端对等端就可以知道流是否被部分处理了。
                        case ShutdownTypes.Gentle:
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.Log($"[HTTP2Handler] [method:RunHandler()] 请求连接中断，发送超时帧! ");
#endif
                            this._outgoingFrames.Clear();
                            this._outgoingFrames.Add(Http2FrameHelper.CreateGoAwayFrame(0, Http2ErrorCodes.NoError));
                            this._goAwaySentAt = DateTime.UtcNow;
                        }
                            break;
                    }

                    if (this._isRunning && now - _goAwaySentAt >= this.MaxGoAwayWaitTime)
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log($"[HTTP2Handler] [method:RunHandler()] 没有接收到超时帧。真的要放弃了!");
#endif
                        this._isRunning = false;
                        _conn.State = HttpConnectionStates.Closed;
                    }

                    uint streamWindowUpdates = 0;

                    // 把所有收集到的帧都看一遍，然后发过去。
                    for (int i = 0; i < this._outgoingFrames.Count; ++i)
                    {
                        var frame = this._outgoingFrames[i];

                        if (frame.Type != Http2FrameTypes.Data /*&& frame.Type != HTTP2FrameTypes.PING*/)
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.Log($"[HTTP2Handler] [method:RunHandler()] Sending frame: {frame.ToString()}");
#endif
                        }

                        // post process frames
                        switch (frame.Type)
                        {
                            case Http2FrameTypes.Data:
                            {
                                if (haltDataSending)
                                {
                                    continue;
                                }

                                //如果跟踪的remoteWindow小于帧的有效负载，我们停止发送数据帧，直到我们收到窗口更新帧
                                if (frame.PayloadLength > this._remoteWindow)
                                {
                                    haltDataSending = true;
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                    Debug.Log(
                                        $"[HTTP2Handler] [method:RunHandler()] 这一轮停止了数据发送。远程窗口: {this._remoteWindow:N0}, frame: {frame.ToString()}");
#endif
                                    continue;
                                }
                            }
                                break;

                            case Http2FrameTypes.WindowUpdate:
                            {
                                if (frame.StreamId > 0)
                                {
                                    streamWindowUpdates += BufferHelper.ReadUInt31(frame.Payload, 0);
                                }
                            }
                                break;
                        }

                        this._outgoingFrames.RemoveAt(i--);

                        using (var buffer = Http2FrameHelper.HeaderAsBinary(frame))
                        {
                            bufferedStream.Write(buffer.Data, 0, buffer.Length);
                        }

                        if (frame.PayloadLength > 0)
                        {
                            bufferedStream.Write(frame.Payload, (int)frame.PayloadOffset, (int)frame.PayloadLength);

                            if (!frame.DontUseMemPool)
                                BufferPool.Release(frame.Payload);
                        }

                        if (frame.Type == Http2FrameTypes.Data)
                        {
                            this._remoteWindow -= frame.PayloadLength;
                        }
                    }

                    if (streamWindowUpdates > 0)
                    {
                        var frame = Http2FrameHelper.CreateWindowUpdateFrame(0, streamWindowUpdates);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log($"[HTTP2Handler] [method:RunHandler()] Sending frame: {frame.ToString()}");
#endif

                        using (var buffer = Http2FrameHelper.HeaderAsBinary(frame))
                        {
                            bufferedStream.Write(buffer.Data, 0, buffer.Length);
                        }

                        bufferedStream.Write(frame.Payload, (int)frame.PayloadOffset, (int)frame.PayloadLength);
                    }
                } // while (this.isRunning)

                bufferedStream.Flush();
            }
            catch (Exception ex)
            {
                // Log out the exception if it's a non-expected one.
                if (this.ShutdownType == ShutdownTypes.Running && this._goAwaySentAt == DateTime.MaxValue &&
                    HttpManager.IsQuitting)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError($"[HTTP2Handler] [method:RunHandler()] Sender thread [Exception] {ex}");
#endif
                }
            }
            finally
            {
                TryToCleanup();
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log($"[HTTP2Handler] [method:RunHandler()] 发送者线程关闭-清理剩余的请求…");
#endif
                foreach (var t in this._clientInitiatedStreams)
                {
                    t.Abort("连接异常关闭");
                }

                this._clientInitiatedStreams.Clear();
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log($"[HTTP2Handler] [method:RunHandler()] Sender thread closing");
#endif
            }

            try
            {
                if (this._conn is { connector: { } })
                {
                    // Works in the new runtime
                    if (this._conn.connector.TopmostStream != null)
                    {
                        using (this._conn.connector.TopmostStream)
                        {
                        }
                    }

                    // Works in the old runtime
                    if (this._conn.connector.Stream == null) return;
                    using (this._conn.connector.Stream)
                    {
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        public ShutdownTypes ShutdownType { get; private set; }

        public void Shutdown(ShutdownTypes type)
        {
            this.ShutdownType = type;

            switch (this.ShutdownType)
            {
                case ShutdownTypes.Gentle:
                    this._newFrameSignal.Set();
                    break;

                case ShutdownTypes.Immediate:
                    this._conn.connector.Stream.Dispose();
                    break;
            }
        }

        public void Dispose()
        {
            while (this._requestQueue.TryDequeue(out var request))
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[HTTP2Handler] [method:Dispose()] Dispose - Request '{request.CurrentUri}' IsCancellationRequested: {request.IsCancellationRequested.ToString()}");
#endif
                if (request.IsCancellationRequested)
                {
                    request.Response = null;
                    request.State = HttpRequestStates.Aborted;
                }
                else
                {
                    RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(request, RequestEvents.Resend));
                }
            }
        }

        public void SignalRunnerThread()
        {
            this._newFrameSignal.Set();
        }

        private void OnRemoteSettingChanged(Http2SettingsRegistry registry, Http2Settings setting, uint oldValue,
            uint newValue)
        {
            switch (setting)
            {
                case Http2Settings.InitialWindowSize:
                    this._remoteWindow = newValue - (oldValue - this._remoteWindow);
                    break;
            }
        }

        private void ReadThread()
        {
            try
            {
                Thread.CurrentThread.Name = "BestHTTP.HTTP2 Read";
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[HTTP2Handler] [method:ReadThread()] 阅读线程启动并运行!");
#endif

                while (this._isRunning)
                {
                    Http2FrameHeaderAndPayload header = Http2FrameHelper.ReadHeader(this._conn.connector.Stream);

                    if (header.Type != Http2FrameTypes.Data /*&& header.Type != HTTP2FrameTypes.PING*/)
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log(
                            $"[HTTP2Handler] [method:ReadThread()] New frame received: {header.ToString()}");
#endif
                    }

                    // 将新帧添加到队列中。
                    // 在写线程上处理它的好处是不需要处理太多的锁定。
                    this._newFrames.Enqueue(header);

                    // Ping写线程来处理新的帧
                    this._newFrameSignal.Set();

                    switch (header.Type)
                    {
                        // 在读线程上处理pongs，因此不会向rtt计算添加额外的延迟。
                        case Http2FrameTypes.Ping:
                            var pingFrame = Http2FrameHelper.ReadPingFrame(header);

                            if ((pingFrame.Flags & Http2PingFlags.Ack) != 0)
                            {
                                if (Interlocked.CompareExchange(ref this._waitingForPingAck, 0, 1) == 0)
                                {
                                    // waitingForPingAck was 0 ==不期待一个ping ack!
                                    break;
                                }

                                // 这是一个ack，有效载荷必须包含我们发送的内容
                                var ticks = BufferHelper.ReadLong(pingFrame.OpaqueData, 0);

                                // 当前时间与发送ping消息时的时间差
                                TimeSpan diff = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - ticks);

                                // 将其添加到缓冲区
                                this._rtts.Add(diff.TotalMilliseconds);

                                // 计算新的延迟
                                this.Latency = CalculateLatency();
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                Debug.Log(
                                    $"[HTTP2Handler] [method:ReadThread()] Latency: {this.Latency:F2}ms, RTT buffer: {this._rtts}");
#endif
                            }

                            break;

                        case Http2FrameTypes.Goaway:
                            // 退出这个线程。处理线程也会处理框架。
                            return;
                    }
                }
            }
            catch //(Exception ex)
            {
                //HTTPManager.Logger.Exception("HTTP2Handler", "", ex, this.Context);

                //this.isRunning = false;
            }
            finally
            {
                TryToCleanup();
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[HTTP2Handler] [method:ReadThread()] 读取线程关闭");
#endif
            }
        }

        private void TryToCleanup()
        {
            this._isRunning = false;

            // 第一个线程关闭通知ConnectionEventHelper
            int counter = Interlocked.Increment(ref this._threadExitCount);
            if (counter == 1)
                ConnectionEventHelper.EnqueueConnectionEvent(new ConnectionEventInfo(this._conn,
                    HttpConnectionStates.Closed));

            // Last thread closes the AutoResetEvent
            if (counter == 2)
            {
                if (this._newFrameSignal != null)
                    this._newFrameSignal.Close();
                this._newFrameSignal = null;
            }
        }

        private double CalculateLatency()
        {
            if (this._rtts.Count == 0)
                return 0;

            double sumLatency = 0;
            for (int i = 0; i < this._rtts.Count; ++i)
                sumLatency += this._rtts[i];

            return sumLatency / this._rtts.Count;
        }

        HTTP2Stream FindStreamById(uint streamId)
        {
            return this._clientInitiatedStreams.FirstOrDefault(stream => stream.Id == streamId);
        }
    }
}

#endif
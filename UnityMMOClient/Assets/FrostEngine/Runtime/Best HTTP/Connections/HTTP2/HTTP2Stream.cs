#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2


using FrostEngine;
#if !BESTHTTP_DISABLE_CACHING
using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP.Caching;
using BestHTTP.Core;
using BestHTTP.Logger;
using BestHTTP.PlatformSupport.Memory;
using BestHTTP.Timings;
#endif

namespace BestHTTP.Connections.HTTP2
{
    // https://httpwg.org/specs/rfc7540.html#StreamStates
    //
    //                                      Idle
    //                                       |
    //                                       V
    //                                      Open
    //                Receive END_STREAM  /  |   \  Send END_STREAM
    //                                   v   |R   V
    //                  Half Closed Remote   |S   Half Closed Locale
    //                                   \   |T  /
    //     Send END_STREAM | RST_STREAM   \  |  /    Receive END_STREAM | RST_STREAM
    //     Receive RST_STREAM              \ | /     Send RST_STREAM
    //                                       V
    //                                     Closed
    // 
    // IDLE -> send headers -> OPEN -> send data -> HALF CLOSED - LOCAL -> receive headers -> receive Data -> CLOSED
    //               |                                     ^                      |                             ^
    //               +-------------------------------------+                      +-----------------------------+
    //                      END_STREAM flag present?                                   END_STREAM flag present?
    //

    public enum Http2StreamStates
    {
        Idle,

        //ReservedLocale,
        //ReservedRemote,
        Open,
        HalfClosedLocal,
        HalfClosedRemote,
        Closed
    }

    public sealed class Http2Stream
    {
        private readonly HttpPackEncoder _encoder;

        private readonly Queue<Http2FrameHeaderAndPayload> _incomingFrames = new Queue<Http2FrameHeaderAndPayload>();
        private FramesAsStreamView _dataView;
        private uint _downloaded;

        private FramesAsStreamView _headerView;
        private bool _isEndStrReceived;

        private bool _isRstFrameSent;

        private bool _isStreamedDownload;
        private int _lastReadCount;

        // ReSharper disable once NotAccessedField.Local
        private DateTime _lastStateChangedAt;
        private Http2StreamStates _state;

        private uint _localWindow;

        //输出帧。流将为每个Process调用发送一帧，但是因为一个步骤可能能够
        //生成多个帧，我们使用一个列表。
        private readonly Queue<Http2FrameHeaderAndPayload> _outgoing = new Queue<Http2FrameHeaderAndPayload>();

        // ReSharper disable once NotAccessedField.Local
        private Http2Handler _parent;
        private long _remoteWindow;

        private Http2Response _response;

        private uint _sentData;

        private readonly Http2SettingsManager _settings;

        private HttpRequest.UploadStreamInfo _uploadStreamInfo;

        private readonly uint _windowUpdateThreshold;

        /// <summary>
        /// 构造函数来创建客户端流。
        /// </summary>
        public Http2Stream(
            uint id,
            Http2Handler parentHandler,
            Http2SettingsManager registry,
            HttpPackEncoder httpPackEncoder)
        {
            this.Id = id;
            this._parent = parentHandler;
            this._settings = registry;
            this._encoder = httpPackEncoder;

            this._remoteWindow = this._settings.RemoteSettings[Http2Settings.InitialWindowSize];
            this._settings.RemoteSettings.SettingChangedEvent += OnRemoteSettingChanged;

            // Room for improvement: If INITIAL_WINDOW_SIZE is small (what we can consider a 'small' value?), threshold must be higher
            this._windowUpdateThreshold = (uint)(this._remoteWindow / 2);

            this.Context = new LoggingContext(this);
            this.Context.Add("id", id);
        }

        public uint Id { get; private set; }

        public Http2StreamStates State
        {
            get { return this._state; }

            private set
            {
                var oldState = this._state;

                this._state = value;

                if (oldState == this._state) return;
                this._lastStateChangedAt = DateTime.Now;
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[HTTP2Stream] [method: this Http2StreamStates State set] [msg|Exception] [{this.Id}] State changed from {oldState} to {this._state}");
#endif
            }
        }
        //private TimeSpan TimeSpentInCurrentState { get { return DateTime.Now - this.lastStateChangedAt; } }

        /// <summary>
        /// 连接检查这个标志，以决定是否在休眠之前进行新的处理帧发送，直到新数据到达
        /// </summary>
        public bool HasFrameToSend =>
            // 不要让连接休眠直到
            this._outgoing.Count > 0 || // 我们至少提前订了一帧
            (this.State == Http2StreamStates.Open && this._remoteWindow > 0 &&
             this._lastReadCount > 0); //我们正在发送请求数据

        private HttpRequest AssignedRequest { get; set; }

        private LoggingContext Context { get; set; }

        public void Assign(HttpRequest request)
        {
            request.Timing.Add(request.IsRedirected
                ? TimingEventNames.QueuedForRedirection
                : TimingEventNames.Queued);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            StringBuilder sb = new StringBuilder(3);
            sb.Append($"[{this.Id}] Request assigned to stream.");
            sb.Append($"  Remote Window: {this._remoteWindow:N0}.");
            sb.Append($" Uri: {request.CurrentUri}");
            Debug.Log(
                $"[HTTP2Stream] [method: Assign] [msg|Exception] {sb}");
#endif
            this.AssignedRequest = request;
            this._isStreamedDownload = request.UseStreaming && request.OnStreamingData != null;
            this._downloaded = 0;
        }

        public void Process(List<Http2FrameHeaderAndPayload> outgoingFrames)
        {
            if (this.AssignedRequest.IsCancellationRequested && !this._isRstFrameSent)
            {
                // These two are already set in HTTPRequest's Abort().
                //this.AssignedRequest.Response = null;
                //this.AssignedRequest.State = this.AssignedRequest.IsTimedOut ? HTTPRequestStates.TimedOut : HTTPRequestStates.Aborted;

                this._outgoing.Clear();
                if (this.State != Http2StreamStates.Idle)
                    this._outgoing.Enqueue(Http2FrameHelper.CreateRstFrame(this.Id, Http2ErrorCodes.Cancel));

                // We can close the stream if already received headers, or not even sent one
                if (this.State == Http2StreamStates.HalfClosedRemote || this.State == Http2StreamStates.Idle)
                    this.State = Http2StreamStates.Closed;

                this._isRstFrameSent = true;
            }

            // 1.) Go through incoming frames
            ProcessIncomingFrames(outgoingFrames);

            // 2.) Create outgoing frames based on the stream's state and the request processing state.
            ProcessState(outgoingFrames);

            // 3.) Send one frame per Process call
            if (this._outgoing.Count > 0)
            {
                Http2FrameHeaderAndPayload frame = this._outgoing.Dequeue();

                outgoingFrames.Add(frame);

                // If END_Stream in header or data frame is present => half closed local
                if ((frame.Type == Http2FrameTypes.Headers &&
                     (frame.Flags & (byte)Http2HeadersFlags.EndStream) != 0) ||
                    (frame.Type == Http2FrameTypes.Data && (frame.Flags & (byte)Http2DataFlags.EndStream) != 0))
                {
                    this.State = Http2StreamStates.HalfClosedLocal;
                }
            }
        }

        public void AddFrame(Http2FrameHeaderAndPayload frame, List<Http2FrameHeaderAndPayload> outgoingFrames)
        {
            // Room for improvement: error check for forbidden frames (like settings) and stream state

            this._incomingFrames.Enqueue(frame);

            ProcessIncomingFrames(outgoingFrames);
        }

        public void Abort(string msg)
        {
            if (this.AssignedRequest.State != HttpRequestStates.Processing)
            {
                // do nothing, its state is already set.
            }
            else if (this.AssignedRequest.IsCancellationRequested)
            {
                // These two are already set in HTTPRequest's Abort().
                //this.AssignedRequest.Response = null;
                //this.AssignedRequest.State = this.AssignedRequest.IsTimedOut ? HTTPRequestStates.TimedOut : HTTPRequestStates.Aborted;

                this.State = Http2StreamStates.Closed;
            }
            else if (this.AssignedRequest.Retries >= this.AssignedRequest.MaxRetries)
            {
                this.AssignedRequest.Response = null;
                this.AssignedRequest.Exception = new Exception(msg);
                this.AssignedRequest.State = HttpRequestStates.Error;

                this.State = Http2StreamStates.Closed;
            }
            else
            {
                this.AssignedRequest.Retries++;
                RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.AssignedRequest,
                    RequestEvents.Resend));
            }

            this.Removed();
        }

        private void ProcessIncomingFrames(List<Http2FrameHeaderAndPayload> outgoingFrames)
        {
            UInt32 windowUpdate = 0;

            while (this._incomingFrames.Count > 0)
            {
                Http2FrameHeaderAndPayload frame = this._incomingFrames.Dequeue();

                if ((this._isRstFrameSent || this.AssignedRequest.IsCancellationRequested) &&
                    frame.Type != Http2FrameTypes.Headers && frame.Type != Http2FrameTypes.Continuation)
                {
                    BufferPool.Release(frame.Payload);
                    continue;
                }

                if (frame.Type != Http2FrameTypes.Data &&
                    frame.Type !=
                    Http2FrameTypes.WindowUpdate)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    var sb = new StringBuilder(3);
                    sb.Append($"[{this.Id}] Process - processing frame: {frame.ToString()}");
                    Debug.Log(
                        $"[HTTP2Stream] [method: ProcessIncomingFrames] [msg|Exception] {sb}");
#endif
                }

                switch (frame.Type)
                {
                    case Http2FrameTypes.Headers:
                    case Http2FrameTypes.Continuation:
                    {
                        if (this.State != Http2StreamStates.HalfClosedLocal &&
                            this.State != Http2StreamStates.Open &&
                            this.State != Http2StreamStates.Idle)
                        {
                            // ERROR!
                            continue;
                        }

                        // payload will be released by the view
                        frame.DontUseMemPool = true;

                        if (this._headerView == null)
                        {
                            this.AssignedRequest.Timing.Add(TimingEventNames.WaitingTTFB);
                            this._headerView = new FramesAsStreamView(new HeaderFrameView());
                        }

                        this._headerView.AddFrame(frame);

                        // END_STREAM may arrive sooner than an END_HEADERS, so we have to store that we already received it
                        if ((frame.Flags & (byte)Http2HeadersFlags.EndStream) != 0)
                        {
                            this._isEndStrReceived = true;
                        }

                        if ((frame.Flags & (byte)Http2HeadersFlags.EndHeaders) != 0)
                        {
                            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();

                            try
                            {
                                this._encoder.Decode(this, this._headerView, headers);
                            }
                            catch (Exception)
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                var sb = new StringBuilder(3);
                                sb.Append("[HTTP2Stream] ");
                                sb.Append("[method:ProcessIncomingFrames] ");
                                sb.Append($"[msg] [{this.Id}] ProcessIncomingFrames ");
                                sb.Append($"- Header Frames: {this._headerView},");
                                sb.Append($" Encoder: {this._encoder},");
                                Debug.LogError(sb.ToString());
#endif
                            }

                            this.AssignedRequest.Timing.Add(TimingEventNames.Headers);

                            if (this._isRstFrameSent)
                            {
                                this.State = Http2StreamStates.Closed;
                                break;
                            }

                            if (this._response == null)
                                this.AssignedRequest.Response =
                                    this._response = new Http2Response(this.AssignedRequest, false);

                            this._response.AddHeaders(headers);

                            this._headerView.Close();
                            this._headerView = null;

                            if (this._isEndStrReceived)
                            {
                                // If there's any trailing header, no data frame has an END_STREAM flag
                                if (this._isStreamedDownload)
                                    this._response.FinishProcessData();

                                PlatformSupport.Threading.ThreadedRunner
                                    .RunShortLiving(FinishRequest, this,
                                        this._dataView);

                                this._dataView = null;

                                this.State = this.State == Http2StreamStates.HalfClosedLocal
                                    ? Http2StreamStates.Closed
                                    : Http2StreamStates.HalfClosedRemote;
                            }
                        }
                    }
                        break;

                    case Http2FrameTypes.Data:
                    {
                        if (this.State != Http2StreamStates.HalfClosedLocal && this.State != Http2StreamStates.Open)
                        {
                            // ERROR!
                            continue;
                        }

                        this._downloaded += frame.PayloadLength;

                        if (this._isStreamedDownload && frame is { Payload: not null, PayloadLength: > 0 })
                            this._response.ProcessData(frame.Payload, (int)frame.PayloadLength);

                        // frame's buffer will be released by the frames view
                        frame.DontUseMemPool = !this._isStreamedDownload;

                        if (this._dataView == null && !this._isStreamedDownload)
                            this._dataView = new FramesAsStreamView(new DataFrameView());

                        if (!this._isStreamedDownload)
                            this._dataView.AddFrame(frame);

                        // Track received data, and if necessary(local window getting too low), send a window update frame
                        if (this._localWindow < frame.PayloadLength)
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            var sb = new StringBuilder(3);
                            sb.Append("[HTTP2Stream] ");
                            sb.Append("[method:ProcessIncomingFrames] ");
                            sb.Append($"[msg] [{this.Id}] Frame's PayloadLength");
                            sb.Append($" ({frame.PayloadLength:N0}) is larger then local window ");
                            sb.Append($" ({this._localWindow:N0}). Frame: {frame}");
                            Debug.LogError(sb.ToString());
#endif
                        }
                        else
                        {
                            this._localWindow -= frame.PayloadLength;
                        }

                        if ((frame.Flags & (byte)Http2DataFlags.EndStream) != 0)
                            this._isEndStrReceived = true;

                        // Window update logic.
                        //  1.) We could use a logic to only send window update(s) after a threshold is reached.
                        //      When the initial window size is high enough to contain the whole or most of the result,
                        //      sending back two window updates (connection and stream) after every data frame is pointless.
                        //  2.) On the other hand, window updates are cheap and works even when initial window size is low.
                        //          (
                        if (this._isEndStrReceived || this._localWindow <= this._windowUpdateThreshold)
                            windowUpdate += this._settings.MySettings[Http2Settings.InitialWindowSize] -
                                            this._localWindow - windowUpdate;

                        if (this._isEndStrReceived)
                        {
                            if (this._isStreamedDownload)
                            {
                                this._response.FinishProcessData();
                            }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            var sb = new StringBuilder(3);
                            sb.Append($"[{this.Id}] All data arrived,");
                            sb.Append($" data length: {this._downloaded:N0}");
                            Debug.Log(
                                $"[HTTP2Stream] [method: ProcessIncomingFrames] [msg|Exception] {sb}");
#endif
                            // create a short living thread to process the downloaded data:
                            PlatformSupport.Threading.ThreadedRunner.RunShortLiving(
                                FinishRequest, this, this._dataView);

                            this._dataView = null;

                            this.State = this.State == Http2StreamStates.HalfClosedLocal
                                ? Http2StreamStates.Closed
                                : Http2StreamStates.HalfClosedRemote;
                        }
                        else if (this.AssignedRequest.OnDownloadProgress != null)
                        {
                            RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.AssignedRequest,
                                RequestEvents.DownloadProgress,
                                _downloaded,
                                this._response.ExpectedContentLength));
                        }
                    }
                        break;

                    case Http2FrameTypes.WindowUpdate:
                    {
                        Http2WindowUpdateFrame windowUpdateFrame = Http2FrameHelper.ReadWindowUpdateFrame(frame);

#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        var sb = new StringBuilder(3);
                        var windowSizeIncrement = windowUpdateFrame.WindowSizeIncrement;
                        var remoteWindowSizeIncrement = this._remoteWindow + windowUpdateFrame.WindowSizeIncrement;
                        var remoteSettings = this._settings.RemoteSettings[Http2Settings.InitialWindowSize];
                        sb.Append($"[{this.Id}] Received Window Update: {windowSizeIncrement:N0},");
                        sb.Append($" new remoteWindow: {(remoteWindowSizeIncrement):N0}");
                        sb.Append($", initial remote window: {remoteSettings:N0},");
                        sb.Append($", total data sent: {this._sentData:N0}");
                        Debug.Log($"[HTTP2Stream] [method: ProcessIncomingFrames] [msg|Exception] {sb}");
#endif
                        this._remoteWindow += windowUpdateFrame.WindowSizeIncrement;
                    }
                        break;

                    case Http2FrameTypes.RstStream:
                    {
                        // https://httpwg.org/specs/rfc7540.html#RST_STREAM

                        // It's possible to receive an RST_STREAM on a closed stream. In this case, we have to ignore it.
                        if (this.State == Http2StreamStates.Closed)
                            break;

                        var rstStreamFrame = Http2FrameHelper.ReadRST_StreamFrame(frame);

                        //HTTPManager.Logger.Error("HTTP2Stream", string.Format("[{0}] RST Stream frame ({1}) received in state {2}!", this.Id, rstStreamFrame, this.State), this.Context, this.AssignedRequest.Context, this.parent.Context);

                        Abort(
                            $"RST_STREAM frame received! Error code: {rstStreamFrame.Error.ToString()}({rstStreamFrame.ErrorCode})");
                    }
                        break;

                    default:
                    {
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            StringBuilder sb = new StringBuilder();
                            sb.Append("[HTTP2Stream] ");
                            sb.Append("[method: ProcessIncomingFrames] ");
                            sb.Append("[msg|Exception] ");
                            sb.Append($"[{this.Id}] Unexpected frame");
                            sb.Append($" ({frame}, Payload: {frame.PayloadAsHex()})");
                            sb.Append($" in state {this.State}!");
                            Debug.Log(sb.ToString());
#endif
                        }
                    }
                        break;
                }

                if (!frame.DontUseMemPool)
                    BufferPool.Release(frame.Payload);
            }

            if (windowUpdate > 0)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                var sb = new StringBuilder(3);
                var windowUpdateLog = windowUpdate;
                var mySettings = this._settings.MySettings[Http2Settings.InitialWindowSize];
                sb.Append($"[{this.Id}] Sending window update: {windowUpdateLog:N0},");
                sb.Append($" current window: {this._localWindow:N0},");
                sb.Append($" initial window size: {mySettings:N0}");
                Debug.Log($"[HTTP2Stream] [method: ProcessIncomingFrames] [msg|Exception] {sb}");
#endif

                this._localWindow += windowUpdate;

                outgoingFrames.Add(Http2FrameHelper.CreateWindowUpdateFrame(this.Id, windowUpdate));
            }
        }

        // ReSharper disable once UnusedParameter.Local
        private void ProcessState(List<Http2FrameHeaderAndPayload> outgoingFrames)
        {
            switch (this.State)
            {
                case Http2StreamStates.Idle:
                {
                    UInt32 initiatedInitialWindowSize =
                        this._settings.InitiatedMySettings[Http2Settings.InitialWindowSize];
                    this._localWindow = initiatedInitialWindowSize;
                    // window update with a zero increment would be an error (https://httpwg.org/specs/rfc7540.html#WINDOW_UPDATE)
                    //if (HTTP2Connection.MaxValueFor31Bits > initiatedInitialWindowSize)
                    //    this.outgoing.Enqueue(HTTP2FrameHelper.CreateWindowUpdateFrame(this.Id, HTTP2Connection.MaxValueFor31Bits - initiatedInitialWindowSize));
                    //this.localWindow = HTTP2Connection.MaxValueFor31Bits;

#if !BESTHTTP_DISABLE_CACHING
                    // 在我们发送请求之前设置缓存控制头
                    if (!this.AssignedRequest.DisableCache)
                        HttpCacheService.SetHeaders(this.AssignedRequest);
#endif

                    // HttpPack对请求的头进行编码
                    this._encoder.Encode(this, this.AssignedRequest, this._outgoing, this.Id);

                    // HTTP/2使用数据帧来承载消息负载。
                    // HTTP/2中不能使用[RFC7230]章节4.1中定义的分块传输编码。
                    this._uploadStreamInfo = this.AssignedRequest.GetUpStream();

                    //this.State = HTTP2StreamStates.Open;

                    if (this._uploadStreamInfo.Stream == null)
                    {
                        this.State = Http2StreamStates.HalfClosedLocal;
                        this.AssignedRequest.Timing.Add(TimingEventNames.RequestSent);
                    }
                    else
                    {
                        this.State = Http2StreamStates.Open;
                        this._lastReadCount = 1;
                    }
                }
                    break;

                case Http2StreamStates.Open:
                {
                    // 远程窗口可以是负的!看到 https://httpwg.org/specs/rfc7540.html#InitialWindowSize
                    if (this._remoteWindow <= 0)
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        var sb = new StringBuilder(3);
                        sb.Append($"[{this.Id}] 跳过数据发送作为远程窗口 {this._remoteWindow}!");
                        Debug.Log($"[HTTP2Stream] [method: ProcessState] [msg|Exception] {sb}");
#endif
                        return;
                    }

                    // This step will send one frame per OpenState call.

                    Int64 maxFrameSize = Math.Min(this._remoteWindow,
                        this._settings.RemoteSettings[Http2Settings.MaxFrameSize]);

                    Http2FrameHeaderAndPayload frame = new Http2FrameHeaderAndPayload
                    {
                        Type = Http2FrameTypes.Data,
                        StreamId = this.Id,
                        Payload = BufferPool.Get(maxFrameSize, true)
                    };

                    // 如果它是流的结尾，则期望readCount为零。但是，为了使非阻塞场景能够等待数据，将负值视为没有数据。
                    this._lastReadCount =
                        this._uploadStreamInfo.Stream.Read(frame.Payload, 0, (int)Math.Min(maxFrameSize, int.MaxValue));
                    if (this._lastReadCount <= 0)
                    {
                        BufferPool.Release(frame.Payload);
                        frame.Payload = null;
                        frame.PayloadLength = 0;

                        if (this._lastReadCount < 0)
                        {
                            break;
                        }
                    }
                    else
                    {
                        frame.PayloadLength = (uint)this._lastReadCount;
                    }

                    frame.PayloadOffset = 0;
                    frame.DontUseMemPool = false;

                    if (this._lastReadCount <= 0)
                    {
                        this._uploadStreamInfo.Stream.Dispose();
                        this._uploadStreamInfo = new HttpRequest.UploadStreamInfo();

                        frame.Flags = (byte)(Http2DataFlags.EndStream);

                        this.State = Http2StreamStates.HalfClosedLocal;

                        this.AssignedRequest.Timing.Add(TimingEventNames.RequestSent);
                    }

                    this._outgoing.Enqueue(frame);

                    this._remoteWindow -= frame.PayloadLength;

                    this._sentData += frame.PayloadLength;

                    if (this.AssignedRequest.OnUploadProgress != null)
                    {
                        RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.AssignedRequest,
                            RequestEvents.UploadProgress, this._sentData, this._uploadStreamInfo.Length));
                    }
                }
                    //HTTPManager.Logger.Information("HTTP2Stream", string.Format("[{0}] New DATA frame created! remoteWindow: {1:N0}", this.Id, this.remoteWindow), this.Context, this.AssignedRequest.Context, this.parent.Context);
                    break;

                case Http2StreamStates.HalfClosedLocal:
                    break;

                case Http2StreamStates.HalfClosedRemote:
                    break;

                case Http2StreamStates.Closed:
                    break;
            }
        }

        private void OnRemoteSettingChanged(Http2SettingsRegistry registry, Http2Settings setting, uint oldValue,
            uint newValue)
        {
            switch (setting)
            {
                case Http2Settings.InitialWindowSize:
                {
                    // https://httpwg.org/specs/rfc7540.html#InitialWindowSize
                    // "在接收设置帧之前，设置SETTINGS_INITIAL_WINDOW_SIZE的值，
                    //终端发送流控制帧时只能使用默认的初始窗口大小。"
                    // "除了为尚未激活的流更改流量控制窗口外，
                    // a设置帧可以改变流的初始流量控制窗口大小
                    //(即流处于“打开”或“半封闭(远程)”状态)。当SETTINGS_INITIAL_WINDOW_SIZE的值改变时，
                    //接收端必须根据新值和旧值之间的差异来调整所有流流量控制窗口的大小。"
                    //所以，如果我们在远端对等体的初始设置帧被接收之前创建了一个流，我们
                    //将调整窗口大小。例如:初始窗口大小默认为65535，如果我们以后
                    //收到一个改变到1048576 (1 MB)，我们将增加当前的remoteWindow (1 048576 - 65 535 =) 983 041
                    //但由于初始窗口大小在设置帧可以小于默认的65535字节，
                    //差值可以是负的:
                    // " SETTINGS_INITIAL_WINDOW_SIZE的改变会导致流量控制窗口的可用空间变为负值。
                    //发送方必须跟踪负流量控制窗口，不能发送新的流量控制帧
                    //直到它接收到WINDOW_UPDATE帧，导致流控制窗口变成正的。

                    this._remoteWindow += newValue - oldValue;
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    var sb = new StringBuilder(3);
                    sb.Append($"[{this.Id}] Remote Setting's Initial Window Updated from {oldValue:N0}");
                    sb.Append($" to {newValue:N0}, diff: {newValue - oldValue:N0},");
                    sb.Append($" new remoteWindow: {this._remoteWindow:N0},");
                    sb.Append($" total data sent: {this._sentData:N0}");
                    Debug.Log($"[HTTP2Stream] [method: OnRemoteSettingChanged] [msg|Exception] {sb}");
#endif
                }
                    break;
            }
        }

        private static void FinishRequest(Http2Stream stream, FramesAsStreamView dataStream)
        {
            if (dataStream != null)
            {
                try
                {
                    stream._response.AddData(dataStream);
                }
                finally
                {
                    dataStream.Close();
                }
            }

            stream.AssignedRequest.Timing.Add(TimingEventNames.ResponseReceived);

            KeepAliveHeader keepAliveHeader = null; // ignored

            ConnectionHelper.HandleResponse("HTTP2Stream", stream.AssignedRequest, out var resendRequest,
                out _, ref keepAliveHeader, stream.Context, stream.AssignedRequest.Context);

            if (resendRequest && !stream.AssignedRequest.IsCancellationRequested)
            {
                RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(stream.AssignedRequest,
                    RequestEvents.Resend));
            }
            else if (stream.AssignedRequest.State == HttpRequestStates.Processing &&
                     !stream.AssignedRequest.IsCancellationRequested)
            {
                stream.AssignedRequest.State = HttpRequestStates.Finished;
            }
            else
            {
                // Already set in HTTPRequest's Abort().
                //if (stream.AssignedRequest.State == HTTPRequestStates.Processing && stream.AssignedRequest.IsCancellationRequested)
                //    stream.AssignedRequest.State = stream.AssignedRequest.IsTimedOut ? HTTPRequestStates.TimedOut : HTTPRequestStates.Aborted;
            }
        }

        public void Removed()
        {
            if (this._uploadStreamInfo.Stream != null)
            {
                this._uploadStreamInfo.Stream.Dispose();
                this._uploadStreamInfo = new HttpRequest.UploadStreamInfo();
            }

            // After receiving a RST_STREAM on a stream, the receiver MUST NOT send additional frames for that stream, with the exception of PRIORITY.
            this._outgoing.Clear();

            // https://github.com/Benedicht/BestHTTP-Issues/issues/77
            // Unsubscribe from OnSettingChangedEvent to remove reference to this instance.
            this._settings.RemoteSettings.SettingChangedEvent -= OnRemoteSettingChanged;
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[HTTP2Stream] [method: Removed] [msg|Exception] Stream removed: {this.Id.ToString()}");
#endif
        }
    }
}

#endif
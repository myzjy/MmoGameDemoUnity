#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2

#if !BESTHTTP_DISABLE_CACHING
using System;
using System.Collections.Generic;
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

    public sealed class HTTP2Stream
    {
        public readonly HttpPackEncoder Encoder;

        private readonly Queue<Http2FrameHeaderAndPayload> incomingFrames = new Queue<Http2FrameHeaderAndPayload>();
        private FramesAsStreamView _dataView;
        private uint _downloaded;

        private FramesAsStreamView _headerView;
        private bool _isEndStrReceived;

        private bool _isRstFrameSent;

        private bool _isStreamedDownload;
        private int _lastReadCount;

        private DateTime _lastStateChangedAt;
        private Http2StreamStates _state;

        private UInt32 localWindow;

        // Outgoing frames. The stream will send one frame per Process call, but because one step might be able to
        // generate more than one frames, we use a list.
        private Queue<Http2FrameHeaderAndPayload> outgoing = new Queue<Http2FrameHeaderAndPayload>();

        private Http2Handler parent;
        private Int64 remoteWindow;

        private Http2Response response;

        private UInt32 sentData;

        private Http2SettingsManager settings;

        private HttpRequest.UploadStreamInfo uploadStreamInfo;

        private uint windowUpdateThreshold;

        /// <summary>
        /// Constructor to create a client stream.
        /// </summary>
        public HTTP2Stream(UInt32 id, Http2Handler parentHandler, Http2SettingsManager registry,
            HttpPackEncoder httpPackEncoder)
        {
            this.Id = id;
            this.parent = parentHandler;
            this.settings = registry;
            this.Encoder = httpPackEncoder;

            this.remoteWindow = this.settings.RemoteSettings[Http2Settings.InitialWindowSize];
            this.settings.RemoteSettings.OnSettingChangedEvent += OnRemoteSettingChanged;

            // Room for improvement: If INITIAL_WINDOW_SIZE is small (what we can consider a 'small' value?), threshold must be higher
            this.windowUpdateThreshold = (uint)(this.remoteWindow / 2);

            this.Context = new LoggingContext(this);
            this.Context.Add("id", id);
        }

        public UInt32 Id { get; private set; }

        public Http2StreamStates State
        {
            get { return this._state; }

            private set
            {
                var oldState = this._state;

                this._state = value;

                if (oldState != this._state)
                {
                    this._lastStateChangedAt = DateTime.Now;

                    HttpManager.Logger.Information("HTTP2Stream",
                        string.Format("[{0}] State changed from {1} to {2}", this.Id, oldState, this._state),
                        this.Context, this.AssignedRequest.Context, this.parent.Context);
                }
            }
        }
        //private TimeSpan TimeSpentInCurrentState { get { return DateTime.Now - this.lastStateChangedAt; } }

        /// <summary>
        /// This flag is checked by the connection to decide whether to do a new processing-frame sending round before sleeping until new data arrives
        /// </summary>
        public bool HasFrameToSend
        {
            get
            {
                // Don't let the connection sleep until
                return this.outgoing.Count > 0 || // we already booked at least one frame in advance
                       (this.State == Http2StreamStates.Open && this.remoteWindow > 0 &&
                        this._lastReadCount > 0); // we are in the middle of sending request data
            }
        }

        public HttpRequest AssignedRequest { get; private set; }

        public LoggingContext Context { get; private set; }

        public void Assign(HttpRequest request)
        {
            if (request.IsRedirected)
                request.Timing.Add(TimingEventNames.Queued_For_Redirection);
            else
                request.Timing.Add(TimingEventNames.Queued);

            HttpManager.Logger.Information("HTTP2Stream",
                string.Format("[{0}] Request assigned to stream. Remote Window: {1:N0}. Uri: {2}", this.Id,
                    this.remoteWindow, request.CurrentUri.ToString()), this.Context, request.Context,
                this.parent.Context);
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

                this.outgoing.Clear();
                if (this.State != Http2StreamStates.Idle)
                    this.outgoing.Enqueue(Http2FrameHelper.CreateRSTFrame(this.Id, Http2ErrorCodes.Cancel));

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
            if (this.outgoing.Count > 0)
            {
                Http2FrameHeaderAndPayload frame = this.outgoing.Dequeue();

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

            this.incomingFrames.Enqueue(frame);

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

            while (this.incomingFrames.Count > 0)
            {
                Http2FrameHeaderAndPayload frame = this.incomingFrames.Dequeue();

                if ((this._isRstFrameSent || this.AssignedRequest.IsCancellationRequested) &&
                    frame.Type != Http2FrameTypes.Headers && frame.Type != Http2FrameTypes.Continuation)
                {
                    BufferPool.Release(frame.Payload);
                    continue;
                }

                if ( /*HTTPManager.Logger.Level == Logger.Loglevels.All && */frame.Type != Http2FrameTypes.Data &&
                                                                             frame.Type !=
                                                                             Http2FrameTypes.WindowUpdate)
                    HttpManager.Logger.Information("HTTP2Stream",
                        string.Format("[{0}] Process - processing frame: {1}", this.Id, frame.ToString()), this.Context,
                        this.AssignedRequest.Context, this.parent.Context);

                switch (frame.Type)
                {
                    case Http2FrameTypes.Headers:
                    case Http2FrameTypes.Continuation:
                        if (this.State != Http2StreamStates.HalfClosedLocal && this.State != Http2StreamStates.Open &&
                            this.State != Http2StreamStates.Idle)
                        {
                            // ERROR!
                            continue;
                        }

                        // payload will be released by the view
                        frame.DontUseMemPool = true;

                        if (this._headerView == null)
                        {
                            this.AssignedRequest.Timing.Add(TimingEventNames.Waiting_TTFB);
                            this._headerView = new FramesAsStreamView(new HeaderFrameView());
                        }

                        this._headerView.AddFrame(frame);

                        // END_STREAM may arrive sooner than an END_HEADERS, so we have to store that we already received it
                        if ((frame.Flags & (byte)Http2HeadersFlags.EndStream) != 0)
                            this._isEndStrReceived = true;

                        if ((frame.Flags & (byte)Http2HeadersFlags.EndHeaders) != 0)
                        {
                            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();

                            try
                            {
                                this.Encoder.Decode(this, this._headerView, headers);
                            }
                            catch (Exception ex)
                            {
                                HttpManager.Logger.Exception("HTTP2Stream",
                                    string.Format("[{0}] ProcessIncomingFrames - Header Frames: {1}, Encoder: {2}",
                                        this.Id, this._headerView.ToString(), this.Encoder.ToString()), ex,
                                    this.Context,
                                    this.AssignedRequest.Context, this.parent.Context);
                            }

                            this.AssignedRequest.Timing.Add(TimingEventNames.Headers);

                            if (this._isRstFrameSent)
                            {
                                this.State = Http2StreamStates.Closed;
                                break;
                            }

                            if (this.response == null)
                                this.AssignedRequest.Response =
                                    this.response = new Http2Response(this.AssignedRequest, false);

                            this.response.AddHeaders(headers);

                            this._headerView.Close();
                            this._headerView = null;

                            if (this._isEndStrReceived)
                            {
                                // If there's any trailing header, no data frame has an END_STREAM flag
                                if (this._isStreamedDownload)
                                    this.response.FinishProcessData();

                                PlatformSupport.Threading.ThreadedRunner
                                    .RunShortLiving<HTTP2Stream, FramesAsStreamView>(FinishRequest, this,
                                        this._dataView);

                                this._dataView = null;

                                if (this.State == Http2StreamStates.HalfClosedLocal)
                                    this.State = Http2StreamStates.Closed;
                                else
                                    this.State = Http2StreamStates.HalfClosedRemote;
                            }
                        }

                        break;

                    case Http2FrameTypes.Data:
                        if (this.State != Http2StreamStates.HalfClosedLocal && this.State != Http2StreamStates.Open)
                        {
                            // ERROR!
                            continue;
                        }

                        this._downloaded += frame.PayloadLength;

                        if (this._isStreamedDownload && frame.Payload != null && frame.PayloadLength > 0)
                            this.response.ProcessData(frame.Payload, (int)frame.PayloadLength);

                        // frame's buffer will be released by the frames view
                        frame.DontUseMemPool = !this._isStreamedDownload;

                        if (this._dataView == null && !this._isStreamedDownload)
                            this._dataView = new FramesAsStreamView(new DataFrameView());

                        if (!this._isStreamedDownload)
                            this._dataView.AddFrame(frame);

                        // Track received data, and if necessary(local window getting too low), send a window update frame
                        if (this.localWindow < frame.PayloadLength)
                        {
                            HttpManager.Logger.Error("HTTP2Stream",
                                string.Format(
                                    "[{0}] Frame's PayloadLength ({1:N0}) is larger then local window ({2:N0}). Frame: {3}",
                                    this.Id, frame.PayloadLength, this.localWindow, frame), this.Context,
                                this.AssignedRequest.Context, this.parent.Context);
                        }
                        else
                            this.localWindow -= frame.PayloadLength;

                        if ((frame.Flags & (byte)Http2DataFlags.EndStream) != 0)
                            this._isEndStrReceived = true;

                        // Window update logic.
                        //  1.) We could use a logic to only send window update(s) after a threshold is reached.
                        //      When the initial window size is high enough to contain the whole or most of the result,
                        //      sending back two window updates (connection and stream) after every data frame is pointless.
                        //  2.) On the other hand, window updates are cheap and works even when initial window size is low.
                        //          (
                        if (this._isEndStrReceived || this.localWindow <= this.windowUpdateThreshold)
                            windowUpdate += this.settings.MySettings[Http2Settings.InitialWindowSize] -
                                            this.localWindow - windowUpdate;

                        if (this._isEndStrReceived)
                        {
                            if (this._isStreamedDownload)
                                this.response.FinishProcessData();

                            HttpManager.Logger.Information("HTTP2Stream",
                                string.Format("[{0}] All data arrived, data length: {1:N0}", this.Id, this._downloaded),
                                this.Context, this.AssignedRequest.Context, this.parent.Context);

                            // create a short living thread to process the downloaded data:
                            PlatformSupport.Threading.ThreadedRunner.RunShortLiving<HTTP2Stream, FramesAsStreamView>(
                                FinishRequest, this, this._dataView);

                            this._dataView = null;

                            if (this.State == Http2StreamStates.HalfClosedLocal)
                                this.State = Http2StreamStates.Closed;
                            else
                                this.State = Http2StreamStates.HalfClosedRemote;
                        }
                        else if (this.AssignedRequest.OnDownloadProgress != null)
                            RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.AssignedRequest,
                                RequestEvents.DownloadProgress,
                                _downloaded,
                                this.response.ExpectedContentLength));

                        break;

                    case Http2FrameTypes.WindowUpdate:
                        Http2WindowUpdateFrame windowUpdateFrame = Http2FrameHelper.ReadWindowUpdateFrame(frame);

                        if (HttpManager.Logger.Level == Logger.Loglevels.All)
                            HttpManager.Logger.Information("HTTP2Stream",
                                string.Format(
                                    "[{0}] Received Window Update: {1:N0}, new remoteWindow: {2:N0}, initial remote window: {3:N0}, total data sent: {4:N0}",
                                    this.Id, windowUpdateFrame.WindowSizeIncrement,
                                    this.remoteWindow + windowUpdateFrame.WindowSizeIncrement,
                                    this.settings.RemoteSettings[Http2Settings.InitialWindowSize], this.sentData),
                                this.Context, this.AssignedRequest.Context, this.parent.Context);

                        this.remoteWindow += windowUpdateFrame.WindowSizeIncrement;
                        break;

                    case Http2FrameTypes.RstStream:
                        // https://httpwg.org/specs/rfc7540.html#RST_STREAM

                        // It's possible to receive an RST_STREAM on a closed stream. In this case, we have to ignore it.
                        if (this.State == Http2StreamStates.Closed)
                            break;

                        var rstStreamFrame = Http2FrameHelper.ReadRST_StreamFrame(frame);

                        //HTTPManager.Logger.Error("HTTP2Stream", string.Format("[{0}] RST Stream frame ({1}) received in state {2}!", this.Id, rstStreamFrame, this.State), this.Context, this.AssignedRequest.Context, this.parent.Context);

                        Abort(string.Format("RST_STREAM frame received! Error code: {0}({1})",
                            rstStreamFrame.Error.ToString(), rstStreamFrame.ErrorCode));
                        break;

                    default:
                        HttpManager.Logger.Warning("HTTP2Stream",
                            string.Format("[{0}] Unexpected frame ({1}, Payload: {2}) in state {3}!", this.Id, frame,
                                frame.PayloadAsHex(), this.State), this.Context, this.AssignedRequest.Context,
                            this.parent.Context);
                        break;
                }

                if (!frame.DontUseMemPool)
                    BufferPool.Release(frame.Payload);
            }

            if (windowUpdate > 0)
            {
                if (HttpManager.Logger.Level <= Logger.Loglevels.All)
                    HttpManager.Logger.Information("HTTP2Stream",
                        string.Format(
                            "[{0}] Sending window update: {1:N0}, current window: {2:N0}, initial window size: {3:N0}",
                            this.Id, windowUpdate, this.localWindow,
                            this.settings.MySettings[Http2Settings.InitialWindowSize]), this.Context,
                        this.AssignedRequest.Context, this.parent.Context);

                this.localWindow += windowUpdate;

                outgoingFrames.Add(Http2FrameHelper.CreateWindowUpdateFrame(this.Id, windowUpdate));
            }
        }

        private void ProcessState(List<Http2FrameHeaderAndPayload> outgoingFrames)
        {
            switch (this.State)
            {
                case Http2StreamStates.Idle:

                    UInt32 initiatedInitialWindowSize =
                        this.settings.InitiatedMySettings[Http2Settings.InitialWindowSize];
                    this.localWindow = initiatedInitialWindowSize;
                    // window update with a zero increment would be an error (https://httpwg.org/specs/rfc7540.html#WINDOW_UPDATE)
                    //if (HTTP2Connection.MaxValueFor31Bits > initiatedInitialWindowSize)
                    //    this.outgoing.Enqueue(HTTP2FrameHelper.CreateWindowUpdateFrame(this.Id, HTTP2Connection.MaxValueFor31Bits - initiatedInitialWindowSize));
                    //this.localWindow = HTTP2Connection.MaxValueFor31Bits;

#if !BESTHTTP_DISABLE_CACHING
                    // Setup cache control headers before we send out the request
                    if (!this.AssignedRequest.DisableCache)
                        HttpCacheService.SetHeaders(this.AssignedRequest);
#endif

                    // hpack encode the request's headers
                    this.Encoder.Encode(this, this.AssignedRequest, this.outgoing, this.Id);

                    // HTTP/2 uses DATA frames to carry message payloads.
                    // The chunked transfer encoding defined in Section 4.1 of [RFC7230] MUST NOT be used in HTTP/2.
                    this.uploadStreamInfo = this.AssignedRequest.GetUpStream();

                    //this.State = HTTP2StreamStates.Open;

                    if (this.uploadStreamInfo.Stream == null)
                    {
                        this.State = Http2StreamStates.HalfClosedLocal;
                        this.AssignedRequest.Timing.Add(TimingEventNames.Request_Sent);
                    }
                    else
                    {
                        this.State = Http2StreamStates.Open;
                        this._lastReadCount = 1;
                    }

                    break;

                case Http2StreamStates.Open:
                    // remote Window can be negative! See https://httpwg.org/specs/rfc7540.html#InitialWindowSize
                    if (this.remoteWindow <= 0)
                    {
                        HttpManager.Logger.Information("HTTP2Stream",
                            string.Format("[{0}] Skipping data sending as remote Window is {1}!", this.Id,
                                this.remoteWindow), this.Context, this.AssignedRequest.Context, this.parent.Context);
                        return;
                    }

                    // This step will send one frame per OpenState call.

                    Int64 maxFrameSize = Math.Min(this.remoteWindow,
                        this.settings.RemoteSettings[Http2Settings.MaxFrameSize]);

                    Http2FrameHeaderAndPayload frame = new Http2FrameHeaderAndPayload();
                    frame.Type = Http2FrameTypes.Data;
                    frame.StreamId = this.Id;

                    frame.Payload = BufferPool.Get(maxFrameSize, true);

                    // Expect a readCount of zero if it's end of the stream. But, to enable non-blocking scenario to wait for data, going to treat a negative value as no data.
                    this._lastReadCount =
                        this.uploadStreamInfo.Stream.Read(frame.Payload, 0, (int)Math.Min(maxFrameSize, int.MaxValue));
                    if (this._lastReadCount <= 0)
                    {
                        BufferPool.Release(frame.Payload);
                        frame.Payload = null;
                        frame.PayloadLength = 0;

                        if (this._lastReadCount < 0)
                            break;
                    }
                    else
                        frame.PayloadLength = (UInt32)this._lastReadCount;

                    frame.PayloadOffset = 0;
                    frame.DontUseMemPool = false;

                    if (this._lastReadCount <= 0)
                    {
                        this.uploadStreamInfo.Stream.Dispose();
                        this.uploadStreamInfo = new HttpRequest.UploadStreamInfo();

                        frame.Flags = (byte)(Http2DataFlags.EndStream);

                        this.State = Http2StreamStates.HalfClosedLocal;

                        this.AssignedRequest.Timing.Add(TimingEventNames.Request_Sent);
                    }

                    this.outgoing.Enqueue(frame);

                    this.remoteWindow -= frame.PayloadLength;

                    this.sentData += frame.PayloadLength;

                    if (this.AssignedRequest.OnUploadProgress != null)
                        RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.AssignedRequest,
                            RequestEvents.UploadProgress, this.sentData, this.uploadStreamInfo.Length));

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
                    // https://httpwg.org/specs/rfc7540.html#InitialWindowSize
                    // "Prior to receiving a SETTINGS frame that sets a value for SETTINGS_INITIAL_WINDOW_SIZE,
                    // an endpoint can only use the default initial window size when sending flow-controlled frames."
                    // "In addition to changing the flow-control window for streams that are not yet active,
                    // a SETTINGS frame can alter the initial flow-control window size for streams with active flow-control windows
                    // (that is, streams in the "open" or "half-closed (remote)" state). When the value of SETTINGS_INITIAL_WINDOW_SIZE changes,
                    // a receiver MUST adjust the size of all stream flow-control windows that it maintains by the difference between the new value and the old value."

                    // So, if we created a stream before the remote peer's initial settings frame is received, we
                    // will adjust the window size. For example: initial window size by default is 65535, if we later
                    // receive a change to 1048576 (1 MB) we will increase the current remoteWindow by (1 048 576 - 65 535 =) 983 041

                    // But because initial window size in a setting frame can be smaller then the default 65535 bytes,
                    // the difference can be negative:
                    // "A change to SETTINGS_INITIAL_WINDOW_SIZE can cause the available space in a flow-control window to become negative.
                    // A sender MUST track the negative flow-control window and MUST NOT send new flow-controlled frames
                    // until it receives WINDOW_UPDATE frames that cause the flow-control window to become positive.

                    // For example, if the client sends 60 KB immediately on connection establishment
                    // and the server sets the initial window size to be 16 KB, the client will recalculate
                    // the available flow - control window to be - 44 KB on receipt of the SETTINGS frame.
                    // The client retains a negative flow-control window until WINDOW_UPDATE frames restore the
                    // window to being positive, after which the client can resume sending."

                    this.remoteWindow += newValue - oldValue;

                    HttpManager.Logger.Information("HTTP2Stream",
                        string.Format(
                            "[{0}] Remote Setting's Initial Window Updated from {1:N0} to {2:N0}, diff: {3:N0}, new remoteWindow: {4:N0}, total data sent: {5:N0}",
                            this.Id, oldValue, newValue, newValue - oldValue, this.remoteWindow, this.sentData),
                        this.Context, this.AssignedRequest.Context, this.parent.Context);
                    break;
            }
        }

        private static void FinishRequest(HTTP2Stream stream, FramesAsStreamView dataStream)
        {
            if (dataStream != null)
            {
                try
                {
                    stream.response.AddData(dataStream);
                }
                finally
                {
                    dataStream.Close();
                }
            }

            stream.AssignedRequest.Timing.Add(TimingEventNames.Response_Received);

            bool resendRequest;
            HttpConnectionStates proposedConnectionStates; // ignored
            KeepAliveHeader keepAliveHeader = null; // ignored

            ConnectionHelper.HandleResponse("HTTP2Stream", stream.AssignedRequest, out resendRequest,
                out proposedConnectionStates, ref keepAliveHeader, stream.Context, stream.AssignedRequest.Context);

            if (resendRequest && !stream.AssignedRequest.IsCancellationRequested)
                RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(stream.AssignedRequest,
                    RequestEvents.Resend));
            else if (stream.AssignedRequest.State == HttpRequestStates.Processing &&
                     !stream.AssignedRequest.IsCancellationRequested)
                stream.AssignedRequest.State = HttpRequestStates.Finished;
            else
            {
                // Already set in HTTPRequest's Abort().
                //if (stream.AssignedRequest.State == HTTPRequestStates.Processing && stream.AssignedRequest.IsCancellationRequested)
                //    stream.AssignedRequest.State = stream.AssignedRequest.IsTimedOut ? HTTPRequestStates.TimedOut : HTTPRequestStates.Aborted;
            }
        }

        public void Removed()
        {
            if (this.uploadStreamInfo.Stream != null)
            {
                this.uploadStreamInfo.Stream.Dispose();
                this.uploadStreamInfo = new HttpRequest.UploadStreamInfo();
            }

            // After receiving a RST_STREAM on a stream, the receiver MUST NOT send additional frames for that stream, with the exception of PRIORITY.
            this.outgoing.Clear();

            // https://github.com/Benedicht/BestHTTP-Issues/issues/77
            // Unsubscribe from OnSettingChangedEvent to remove reference to this instance.
            this.settings.RemoteSettings.OnSettingChangedEvent -= OnRemoteSettingChanged;

            HttpManager.Logger.Information("HTTP2Stream", "Stream removed: " + this.Id.ToString(), this.Context,
                this.AssignedRequest.Context, this.parent.Context);
        }
    }
}

#endif
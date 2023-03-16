#if !BESTHTTP_DISABLE_WEBSOCKET && (!UNITY_WEBGL || UNITY_EDITOR)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using BestHTTP.Core;
using BestHTTP.Extensions;
using BestHTTP.Logger;
using BestHTTP.PlatformSupport.Memory;
using BestHTTP.WebSocket.Frames;

namespace BestHTTP.WebSocket
{
    public sealed class WebSocketResponse : HttpResponse, IProtocol
    {
        /// <summary>
        /// 保留延迟的RTT缓冲区的容量。
        /// </summary>
        public static int RTTBufferCapacity = 5;

        internal WebSocketResponse(
            HttpRequest request,
            Stream stream,
            bool isStreamed,
            bool isFromCache)
            : base(
                request,
                stream,
                isStreamed,
                isFromCache)
        {
            base.IsClosedManually = true;
            this.ConnectionKey = new HostConnectionKey(
                host: this.BaseRequest.CurrentUri.Host,
                connection: HostDefinition.GetKeyForRequest(this.BaseRequest));

            closed = false;
            MaxFragmentSize = WebSocket.MaxFragmentSize;
        }


        /// <summary>
        /// 内部功能，发送收到的消息。
        /// </summary>
        void IProtocol.HandleEvents()
        {
            while (CompletedFrames.TryDequeue(out var frame))
            {
                // 客户机中的错误不应该中断代码，因此我们需要尝试捕获并忽略此处发生的任何异常
                try
                {
                    switch (frame.Type)
                    {
                        case WebSocketFrameTypes.Continuation:
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            {
                                var st = new StackTrace(new StackFrame(true));
                                var sf = st.GetFrame(0);
                                var sb = new StringBuilder(6);
                                sb.Append($"[{sf.GetFileName()}]");
                                sb.Append($"[method:{sf.GetMethod().Name}] ");
                                sb.Append($"{sf.GetMethod().Name} ");
                                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                                sb.Append($"[msg{sf.GetMethod().Name}]");
                                sb.Append($"HandleEvents - OnIncompleteFrame");
                                Debug.Log($"{sb}");
                            }
#endif
                            OnIncompleteFrame?.Invoke(this, frame);
                        }
                            break;

                        case WebSocketFrameTypes.Text:
                        {
                            // 任何非Final帧都被作为片段处理
                            if (!frame.IsFinal)
                            {
                                goto case WebSocketFrameTypes.Continuation;
                            }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            {
                                var st = new StackTrace(new StackFrame(true));
                                var sf = st.GetFrame(0);
                                var sb = new StringBuilder(6);
                                sb.Append($"[{sf.GetFileName()}]");
                                sb.Append($"[method:{sf.GetMethod().Name}] ");
                                sb.Append($"{sf.GetMethod().Name} ");
                                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                                sb.Append($"[msg{sf.GetMethod().Name}]");
                                sb.Append($"HandleEvents - OnText");
                                Debug.Log($"{sb}");
                            }
#endif
                            OnText?.Invoke(this, frame.DataAsText);
                        }
                            break;

                        case WebSocketFrameTypes.Binary:
                        {
                            // 任何非Final帧都被作为片段处理
                            if (!frame.IsFinal)
                            {
                                goto case WebSocketFrameTypes.Continuation;
                            }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            {
                                var st = new StackTrace(new StackFrame(true));
                                var sf = st.GetFrame(0);
                                var sb = new StringBuilder(6);
                                sb.Append($"[{sf.GetFileName()}]");
                                sb.Append($"[method:{sf.GetMethod().Name}] ");
                                sb.Append($"{sf.GetMethod().Name} ");
                                sb.Append($"Line:{sf.GetFileLineNumber()} ");
                                sb.Append($"[msg{sf.GetMethod().Name}]");
                                sb.Append($"HandleEvents - OnBinary");
                                Debug.Log($"{sb}");
                            }
#endif
                            OnBinary?.Invoke(this, frame.Data);
                        }
                            break;
                        case WebSocketFrameTypes.ConnectionClose:
                            break;
                        case WebSocketFrameTypes.Ping:
                            break;
                        case WebSocketFrameTypes.Pong:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    {
                        var st = new StackTrace(new StackFrame(true));
                        var sf = st.GetFrame(0);
                        var sb = new StringBuilder(6);
                        sb.Append($"[{sf.GetFileName()}]");
                        sb.Append($"[method:{sf.GetMethod().Name}] ");
                        sb.Append($"{sf.GetMethod().Name} ");
                        sb.Append($"Line:{sf.GetFileLineNumber()} ");
                        sb.Append($"[msg{sf.GetMethod().Name}]");
                        sb.Append($"[Exception] {ex}");
                        Debug.LogError($"{sb}");
                    }
#endif
                }
            }

            // 2015.05.09
            //添加状态检查，因为如果有一个错误，OnClose首先被调用，然后OnError。
            //现在，当有错误时，只有OnError事件将被调用!
            if (!IsClosed || OnClosed == null || BaseRequest.State != HttpRequestStates.Processing) return;
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    var sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}] ");
                    sb.Append($"{sf.GetMethod().Name} ");
                    sb.Append($"Line:{sf.GetFileLineNumber()} ");
                    sb.Append($"[msg{sf.GetMethod().Name}]");
                    sb.Append($"HandleEvents - Calling OnClosed");
                    Debug.Log($"{sb}");
                }
#endif
                try
                {
                    UInt16 statusCode = 0;
                    string msg = string.Empty;

                    // 如果我们接收到任何数据，我们将从中获取状态码和消息
                    if ( /*CloseFrame != null && */CloseFrame.Data is { Length: >= 2 })
                    {
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(CloseFrame.Data, 0, 2);
                        }

                        statusCode = BitConverter.ToUInt16(CloseFrame.Data, 0);

                        if (CloseFrame.Data.Length > 2)
                        {
                            msg = Encoding.UTF8.GetString(CloseFrame.Data, 2, CloseFrame.Data.Length - 2);
                        }

                        CloseFrame.ReleaseData();
                    }

                    OnClosed(this, statusCode, msg);
                    OnClosed = null;
                }
                catch (Exception ex)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    {
                        var st = new StackTrace(new StackFrame(true));
                        var sf = st.GetFrame(0);
                        var sb = new StringBuilder(6);
                        sb.Append($"[{sf.GetFileName()}]");
                        sb.Append($"[method:{sf.GetMethod().Name}] ");
                        sb.Append($"{sf.GetMethod().Name} ");
                        sb.Append($"Line:{sf.GetFileLineNumber()} ");
                        sb.Append($"[msg{sf.GetMethod().Name}]");
                        sb.Append($"HandleEvents - OnClosed Exception--->>:{ex}");
                        Debug.LogError($"{sb}");
                    }
#endif
                }
            }
        }

        void IProtocol.CancellationRequested()
        {
            CloseWithError(
                state: HttpRequestStates.Aborted,
                message: null);
        }

        internal void StartReceive()
        {
            if (IsUpgraded)
            {
                BestHTTP.PlatformSupport.Threading.ThreadedRunner.RunLongLiving(ReceiveThreadFunc);
            }
        }

        internal void CloseStream()
        {
            if (base.Stream != null)
            {
                try
                {
                    base.Stream.Dispose();
                }
                catch
                {
                }
            }
        }

        private bool SendPing()
        {
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                var str = "Sending Ping frame, waiting for a pong...";
                Debug.Log($"[WebSocketResponse] [method:SendPing] [msg] {str}");
#endif
            }
            lastPing = DateTime.UtcNow;
            waitingForPong = true;

            try
            {
                var pingFrame = new WebSocketFrame(this.WebSocket, WebSocketFrameTypes.Ping, null);

                Send(pingFrame);
            }
            catch
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                var str = "Error while sending PING message! Closing WebSocket.";
                Debug.Log($"[WebSocketResponse] [method:SendPing] [msg] {str}");
#endif
                CloseWithError(HttpRequestStates.Error, "Error while sending PING message!");

                return false;
            }

            return true;
        }

        private void CloseWithError(HttpRequestStates state, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this.BaseRequest.Exception = new Exception(message);
            }

            this.BaseRequest.State = state;

            this.closed = true;

            CloseStream();
            ProtocolEventHelper.EnqueueProtocolEvent(new ProtocolEventInfo(this));
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

        private void TryToCleanup()
        {
            if (Interlocked.Increment(ref this.closedThreads) == 2)
            {
                ProtocolEventHelper.EnqueueProtocolEvent(new ProtocolEventInfo(this));
                (newFrameSignal as IDisposable).Dispose();
                newFrameSignal = null;

                CloseStream();
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log($"[WebSocketResponse] [method:TryToCleanup] [msg] TryToCleanup - finished!");
#endif
            }
        }

        public override string ToString()
        {
            return this.ConnectionKey.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            IncompleteFrames.Clear();
            CompletedFrames.Clear();
            unsentFrames.Clear();
        }

        /// <summary>
        /// 对原始WebSocket实例的引用。用于访问扩展。
        /// </summary>
        public WebSocket WebSocket { get; internal set; }

        /// <summary>
        /// 当收到文本消息时调用
        /// </summary>
        public Action<WebSocketResponse, string> OnText;

        /// <summary>
        /// 当接收到二进制消息时调用
        /// </summary>
        public Action<WebSocketResponse, byte[]> OnBinary;

        /// <summary>
        /// 当接收到不完整的帧时调用。我们不会试图重新组装这些碎片。
        /// </summary>
        public Action<WebSocketResponse, WebSocketFrameReader> OnIncompleteFrame;

        /// <summary>
        /// 当连接关闭时调用。
        /// </summary>
        public Action<WebSocketResponse, UInt16, string> OnClosed;

        /// <summary>
        /// IProtocol的ConnectionKey属性。
        /// </summary>
        public HostConnectionKey ConnectionKey { get; private set; }

        /// <summary>
        /// 指示到服务器的连接是否关闭。
        /// </summary>
        public bool IsClosed => closed;

        /// <summary>
        /// IProtocol。LoggingContext实现。
        /// </summary>
        LoggingContext IProtocol.LoggingContext
        {
            get => this.Context;
        }

        /// <summary>
        /// 我们需要用什么频率向服务器发送ping信号。
        /// </summary>
        private TimeSpan PingFrequency { get; set; }

        /// <summary>
        /// 片段有效载荷数据的最大大小。缺省值为WebSocket.MaxFragmentSize价值。
        /// </summary>
        private uint MaxFragmentSize { get; set; }

        /// <summary>
        /// 未发送、已缓冲数据的长度(以字节为单位)。
        /// </summary>
        public int BufferedAmount => this._bufferedAmount;

        private int _bufferedAmount;

        /// <summary>
        /// 我们存储在rtts字段中的往返时间计算的延迟。
        /// </summary>
        public int Latency { get; private set; }

        /// <summary>
        /// 当我们收到最后一帧。
        /// </summary>
        public DateTime LastMessage = DateTime.MinValue;


        #region Private Fields

        private List<WebSocketFrameReader> IncompleteFrames = new List<WebSocketFrameReader>();
        private ConcurrentQueue<WebSocketFrameReader> CompletedFrames = new ConcurrentQueue<WebSocketFrameReader>();
        private WebSocketFrameReader CloseFrame;

        private ConcurrentQueue<WebSocketFrame> unsentFrames = new ConcurrentQueue<WebSocketFrame>();
        private volatile AutoResetEvent newFrameSignal = new AutoResetEvent(false);
        private int sendThreadCreated = 0;
        private int closedThreads = 0;

        /// <summary>
        /// True if we sent out a Close message to the server
        /// </summary>
        private volatile bool closeSent;

        /// <summary>
        /// True if this WebSocket connection is closed
        /// </summary>
        private volatile bool closed;

        /// <summary>
        /// When we sent out the last ping.
        /// </summary>
        private DateTime lastPing = DateTime.MinValue;

        /// <summary>
        /// True if waiting for an answer to our ping request. Ping timeout is used only why waitingForPong is true.
        /// </summary>
        private volatile bool waitingForPong = false;

        /// <summary>
        /// A circular buffer to store the last N rtt times calculated by the pong messages.
        /// </summary>
        private CircularBuffer<int> rtts = new CircularBuffer<int>(WebSocketResponse.RTTBufferCapacity);

        #endregion

        #region Public interface for interacting with the server

        /// <summary>
        /// It will send the given message to the server in one frame.
        /// </summary>
        public void Send(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message must not be null!");

            int count = System.Text.Encoding.UTF8.GetByteCount(message);
            byte[] data = BufferPool.Get(count, true);
            System.Text.Encoding.UTF8.GetBytes(message, 0, message.Length, data, 0);

            var frame = new WebSocketFrame(this.WebSocket, WebSocketFrameTypes.Text, data, 0, (ulong)count, true, true);

            if (frame.Data != null && frame.Data.Length > this.MaxFragmentSize)
            {
                WebSocketFrame[] additionalFrames = frame.Fragment(this.MaxFragmentSize);

                Send(frame);
                if (additionalFrames != null)
                    for (int i = 0; i < additionalFrames.Length; ++i)
                        Send(additionalFrames[i]);
            }
            else
                Send(frame);

            BufferPool.Release(data);
        }

        /// <summary>
        /// It will send the given data to the server in one frame.
        /// </summary>
        public void Send(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data must not be null!");

            WebSocketFrame frame = new WebSocketFrame(this.WebSocket, WebSocketFrameTypes.Binary, data);

            if (frame.Data != null && frame.Data.Length > this.MaxFragmentSize)
            {
                WebSocketFrame[] additionalFrames = frame.Fragment(this.MaxFragmentSize);

                Send(frame);
                if (additionalFrames == null) return;
                foreach (var t in additionalFrames)
                    Send(t);
            }
            else
                Send(frame);
        }

        /// <summary>
        /// Will send count bytes from a byte array, starting from offset.
        /// </summary>
        public void Send(byte[] data, ulong offset, ulong count)
        {
            if (data == null)
                throw new ArgumentNullException("data must not be null!");
            if (offset + count > (ulong)data.Length)
                throw new ArgumentOutOfRangeException("offset + count >= data.Length");

            WebSocketFrame frame = new WebSocketFrame(this.WebSocket, WebSocketFrameTypes.Binary, data, offset, count,
                true, true);

            if (frame.Data != null && frame.Data.Length > this.MaxFragmentSize)
            {
                WebSocketFrame[] additionalFrames = frame.Fragment(this.MaxFragmentSize);

                Send(frame);

                if (additionalFrames != null)
                    for (int i = 0; i < additionalFrames.Length; ++i)
                        Send(additionalFrames[i]);
            }
            else
                Send(frame);
        }

        /// <summary>
        /// It will send the given frame to the server.
        /// </summary>
        public void Send(WebSocketFrame frame)
        {
            if (frame == null)
                throw new ArgumentNullException("frame is null!");

            if (closed || closeSent)
                return;

            this.unsentFrames.Enqueue(frame);

            if (Interlocked.CompareExchange(ref this.sendThreadCreated, 1, 0) == 0)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log($"[WebSocketResponse] [method:Send] [msg] Send - Creating thread");
#endif
                BestHTTP.PlatformSupport.Threading.ThreadedRunner.RunLongLiving(SendThreadFunc);
            }

            Interlocked.Add(ref this._bufferedAmount, frame.Data != null ? frame.DataLength : 0);

            //if (HTTPManager.Logger.Level <= Logger.Loglevels.All)
            //    HTTPManager.Logger.Information("WebSocketResponse", "Signaling SendThread!", this.Context);

            newFrameSignal.Set();
        }

        /// <summary>
        /// It will initiate the closing of the connection to the server.
        /// </summary>
        public void Close()
        {
            Close(1000, "Bye!");
        }

        /// <summary>
        /// It will initiate the closing of the connection to the server.
        /// </summary>
        public void Close(UInt16 code, string msg)
        {
            if (closed)
            {
                return;
            }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[WebSocketResponse] [method:Close] [msg] HandleEvents - Close({code}, \"{msg}\") ");
#endif
            while (this.unsentFrames.TryDequeue(out _))
                ;
            //this.unsentFrames.Clear();

            Interlocked.Exchange(ref this._bufferedAmount, 0);

            Send(new WebSocketFrame(this.WebSocket, WebSocketFrameTypes.ConnectionClose,
                WebSocket.EncodeCloseData(code, msg)));
        }

        public void StartPinging(int frequency)
        {
            if (frequency < 100)
                throw new ArgumentException("frequency must be at least 100 milliseconds!");

            PingFrequency = TimeSpan.FromMilliseconds(frequency);
            LastMessage = DateTime.UtcNow;

            SendPing();
        }

        #endregion

        #region Private Threading Functions

        private void SendThreadFunc()
        {
            try
            {
                using WriteOnlyBufferedStream bufferedStream = new WriteOnlyBufferedStream(this.Stream, 16 * 1024);
                while (!closed && !closeSent)
                {
                    //if (HTTPManager.Logger.Level <= Logger.Loglevels.All)
                    //    HTTPManager.Logger.Information("WebSocketResponse", "SendThread - Waiting...", this.Context);

                    TimeSpan waitTime = TimeSpan.FromMilliseconds(int.MaxValue);

                    if (this.PingFrequency != TimeSpan.Zero)
                    {
                        DateTime now = DateTime.UtcNow;
                        waitTime = LastMessage + PingFrequency - now;

                        if (waitTime <= TimeSpan.Zero)
                        {
                            if (!waitingForPong && now - LastMessage >= PingFrequency)
                            {
                                if (!SendPing())
                                    continue;
                            }

                            waitTime = PingFrequency;
                        }

                        if (waitingForPong && now - lastPing > this.WebSocket.CloseAfterNoMessage)
                        {
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                StringBuilder sb = new StringBuilder();
                                sb.Append("[WebSocketResponse] ");
                                sb.Append("[method: SendThreadFunc] ");
                                sb.Append("[msg|Exception] ");
                                sb.Append($"No message received in the given time!");
                                sb.Append($" Closing WebSocket. LastPing: {this.lastPing}");
                                sb.Append($" , PingFrequency: {this.PingFrequency},");
                                sb.Append($" Close After: {this.WebSocket.CloseAfterNoMessage},");
                                sb.Append($" Now: {now}");
                                Debug.Log(sb.ToString());
#endif
                            }
                            CloseWithError(HttpRequestStates.Error, "No message received in the given time!");
                            continue;
                        }
                    }

                    newFrameSignal.WaitOne(waitTime);

                    try
                    {
                        //if (HTTPManager.Logger.Level <= Logger.Loglevels.All)
                        //    HTTPManager.Logger.Information("WebSocketResponse", "SendThread - Wait is over, about " + this.unsentFrames.Count.ToString() + " new frames!", this.Context);

                        while (this.unsentFrames.TryDequeue(out var frame))
                        {
                            if (!closeSent)
                            {
                                using (var rawData = frame.Get())
                                    bufferedStream.Write(rawData.Data, 0, rawData.Length);

                                BufferPool.Release(frame.Data);

                                if (frame.Type == WebSocketFrameTypes.ConnectionClose)
                                    closeSent = true;
                            }

                            Interlocked.Add(ref this._bufferedAmount, -frame.DataLength);
                        }

                        bufferedStream.Flush();
                    }
                    catch (Exception ex)
                    {
                        if (HttpUpdateDelegator.IsCreated)
                        {
                            this.BaseRequest.Exception = ex;
                            this.BaseRequest.State = HttpRequestStates.Error;
                        }
                        else
                            this.BaseRequest.State = HttpRequestStates.Aborted;

                        closed = true;
                    }
                }

                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    StringBuilder sb = new StringBuilder(3);
                    sb.Append($"Ending Send thread. ");
                    sb.Append($" Closed: {closed},");
                    sb.Append($" closeSent: {closeSent}");
                    Debug.Log($"[WebSocketResponse] [method:SendThreadFunc] [msg] {sb.ToString()}");
#endif
                }
            }
            catch (Exception ex)
            {
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    var sb = new StringBuilder(3);
                    sb.Append("[WebSocketResponse] ");
                    sb.Append("[method:SendThreadFunc] ");
                    sb.Append($"[msg|Exception] SendThread [Exception] {ex}");
                    Debug.LogError(sb.ToString());
#endif
                }
            }
            finally
            {
                Interlocked.Exchange(ref sendThreadCreated, 0);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log($"[WebSocketResponse] [method:SendThreadFunc] [msg] SendThread - Closed!");
#endif
                TryToCleanup();
            }
        }

        private void ReceiveThreadFunc()
        {
            try
            {
                while (!closed)
                {
                    try
                    {
                        WebSocketFrameReader frame = new WebSocketFrameReader();
                        frame.Read(this.Stream);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log(
                            $"[WebSocketResponse] [method:ReceiveThreadFunc] [msg] Frame received: {frame.Type}");
#endif
                        LastMessage = DateTime.UtcNow;

                        // A server MUST NOT mask any frames that it sends to the client.  A client MUST close a connection if it detects a masked frame.
                        // In this case, it MAY use the status code 1002 (protocol error)
                        // (These rules might be relaxed in a future specification.)
                        if (frame.HasMask)
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.Log(
                                $"[WebSocketResponse] [method:ReceiveThreadFunc] [msg] Protocol Error: masked frame received from server!");
#endif
                            Close(1002, "Protocol Error: masked frame received from server!");
                            continue;
                        }

                        if (!frame.IsFinal)
                        {
                            if (OnIncompleteFrame == null)
                            {
                                IncompleteFrames.Add(frame);
                            }
                            else
                            {
                                CompletedFrames.Enqueue(frame);
                            }

                            continue;
                        }

                        switch (frame.Type)
                        {
                            // For a complete documentation and rules on fragmentation see http://tools.ietf.org/html/rfc6455#section-5.4
                            // A fragmented Frame's last fragment's opcode is 0 (Continuation) and the FIN bit is set to 1.
                            case WebSocketFrameTypes.Continuation:
                            {
                                // Do an assemble pass only if OnFragment is not set. Otherwise put it in the CompletedFrames, we will handle it in the HandleEvent phase.
                                if (OnIncompleteFrame == null)
                                {
                                    frame.Assemble(IncompleteFrames);

                                    // Remove all incomplete frames
                                    IncompleteFrames.Clear();

                                    // Control frames themselves MUST NOT be fragmented. So, its a normal text or binary frame. Go, handle it as usual.
                                    goto case WebSocketFrameTypes.Binary;
                                }
                                else
                                {
                                    CompletedFrames.Enqueue(frame);
                                    ProtocolEventHelper.EnqueueProtocolEvent(new ProtocolEventInfo(this));
                                }
                            }
                                break;

                            case WebSocketFrameTypes.Text:
                            case WebSocketFrameTypes.Binary:
                            {
                                frame.DecodeWithExtensions(WebSocket);
                                CompletedFrames.Enqueue(frame);
                                ProtocolEventHelper.EnqueueProtocolEvent(new ProtocolEventInfo(this));
                            }
                                break;

                            // Upon receipt of a Ping frame, an endpoint MUST send a Pong frame in response, unless it already received a Close frame.
                            case WebSocketFrameTypes.Ping:
                            {
                                if (!closeSent && !closed)
                                {
                                    Send(new WebSocketFrame(this.WebSocket, WebSocketFrameTypes.Pong, frame.Data));
                                }
                            }
                                break;

                            case WebSocketFrameTypes.Pong:
                            {
                                try
                                {
                                    // the difference between the current time and the time when the ping message is sent
                                    TimeSpan diff = TimeSpan.FromTicks(this.LastMessage.Ticks - this.lastPing.Ticks);

                                    // add it to the buffer
                                    this.rtts.Add((int)diff.TotalMilliseconds);

                                    // and calculate the new latency
                                    this.Latency = CalculateLatency();
                                }
                                catch
                                {
                                    // https://tools.ietf.org/html/rfc6455#section-5.5
                                    // A Pong frame MAY be sent unsolicited.  This serves as a
                                    // unidirectional heartbeat.  A response to an unsolicited Pong frame is
                                    // not expected. 
                                }
                                finally
                                {
                                    waitingForPong = false;
                                }
                            }
                                break;

                            // If an endpoint receives a Close frame and did not previously send a Close frame, the endpoint MUST send a Close frame in response.
                            case WebSocketFrameTypes.ConnectionClose:
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                Debug.Log(
                                    $"[WebSocketResponse] [method:ReceiveThreadFunc] [msg] ConnectionClose packet received!");
#endif
                                CloseFrame = frame;
                                if (!closeSent)
                                    Send(new WebSocketFrame(this.WebSocket, WebSocketFrameTypes.ConnectionClose, null));
                                closed = true;
                            }
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        if (HttpUpdateDelegator.IsCreated)
                        {
                            this.BaseRequest.Exception = e;
                            this.BaseRequest.State = HttpRequestStates.Error;
                        }
                        else
                            this.BaseRequest.State = HttpRequestStates.Aborted;

                        closed = true;
                        newFrameSignal.Set();
                    }
                }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[WebSocketResponse] [method:ReceiveThreadFunc] [msg] Ending Read thread! closed: {closed}");
#endif
            }
            finally
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[WebSocketResponse] [method:ReceiveThreadFunc] [msg] ReceiveThread - Closed!");
#endif
                TryToCleanup();
            }
        }

        #endregion
    }
}

#endif
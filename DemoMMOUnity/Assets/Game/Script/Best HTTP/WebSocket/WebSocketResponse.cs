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
        public static readonly int RTTBufferCapacity = 5;

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
            IsClosedManually = true;
            this.ConnectionKey = new HostConnectionKey(
                host: this.BaseRequest.CurrentUri.Host,
                connection: HostDefinition.GetKeyForRequest(this.BaseRequest));

            _closed = false;
            MaxFragmentSize = WebSocket.MaxFragmentSize;
        }


        /// <summary>
        /// 内部功能，发送收到的消息。
        /// </summary>
        void IProtocol.HandleEvents()
        {
            while (_completedFrames.TryDequeue(out var frame))
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
                    if ( /*CloseFrame != null && */_closeFrame.Data is { Length: >= 2 })
                    {
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(_closeFrame.Data, 0, 2);
                        }

                        statusCode = BitConverter.ToUInt16(_closeFrame.Data, 0);

                        if (_closeFrame.Data.Length > 2)
                        {
                            msg = Encoding.UTF8.GetString(_closeFrame.Data, 2, _closeFrame.Data.Length - 2);
                        }

                        _closeFrame.ReleaseData();
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
                PlatformSupport.Threading.ThreadedRunner.RunLongLiving(ReceiveThreadFunc);
            }
        }

        internal void CloseStream()
        {
            if (Stream == null) return;
            try
            {
                Stream.Dispose();
            }
            catch
            {
                // ignored
            }
        }

        private bool SendPing()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                StringBuilder sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}]");
                sb.Append($"{sf.GetMethod().Name}");
                sb.Append($"Line:{sf.GetFileLineNumber()}");
                var str = "Sending Ping frame, waiting for a pong...";

                sb.Append($"[msg] {str}");
                Debug.Log($"{sb}");
            }
#endif
            _lastPing = DateTime.UtcNow;
            _waitingForPong = true;

            try
            {
                var pingFrame = new WebSocketFrame(this.WebSocket, WebSocketFrameTypes.Ping, null);

                Send(pingFrame);
            }
            catch
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    StringBuilder sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}]");
                    sb.Append($"{sf.GetMethod().Name}");
                    sb.Append($"Line:{sf.GetFileLineNumber()}");
                    var str = "Error while sending PING message! Closing WebSocket.";
                    sb.Append($"[msg] {str}");
                    Debug.LogError($"{sb}");
                }
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

            this._closed = true;

            CloseStream();
            ProtocolEventHelper.EnqueueProtocolEvent(new ProtocolEventInfo(this));
        }

        private int CalculateLatency()
        {
            if (this._rtts.Count == 0)
                return 0;

            int sumLatency = 0;
            for (int i = 0; i < this._rtts.Count; ++i)
            {
                sumLatency += this._rtts[i];
            }

            return sumLatency / this._rtts.Count;
        }

        private void TryToCleanup()
        {
            if (Interlocked.Increment(ref this._closedThreads) == 2)
            {
                ProtocolEventHelper.EnqueueProtocolEvent(new ProtocolEventInfo(this));
                (_newFrameSignal as IDisposable).Dispose();
                _newFrameSignal = null;

                CloseStream();
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    StringBuilder sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}]");
                    sb.Append($"{sf.GetMethod().Name}");
                    sb.Append($"Line:{sf.GetFileLineNumber()}");
                    sb.Append($"[msg] TryToCleanup - finished!");
                    Debug.Log($"{sb}");
                }
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

            _incompleteFrames.Clear();
            _completedFrames.Clear();
            _unsentFrames.Clear();
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
        public bool IsClosed => _closed;

        /// <summary>
        /// IProtocol。LoggingContext实现。
        /// </summary>
        LoggingContext IProtocol.LoggingContext => this.Context;

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
        /// ReSharper disable once CommentTypo
        /// 我们存储在rtts字段中的往返时间计算的延迟。
        /// </summary>
        public int Latency { get; private set; }

        /// <summary>
        /// 当我们收到最后一帧。
        /// </summary>
        public DateTime LastMessage = DateTime.MinValue;

        private readonly List<WebSocketFrameReader> _incompleteFrames = new List<WebSocketFrameReader>();

        private readonly ConcurrentQueue<WebSocketFrameReader> _completedFrames =
            new ConcurrentQueue<WebSocketFrameReader>();

        private WebSocketFrameReader _closeFrame;

        private readonly ConcurrentQueue<WebSocketFrame> _unsentFrames = new ConcurrentQueue<WebSocketFrame>();
        private volatile AutoResetEvent _newFrameSignal = new AutoResetEvent(false);
        private int _sendThreadCreated;
        private int _closedThreads;

        /// <summary>
        /// 如果向服务器发送Close消息，则为True
        /// </summary>
        private volatile bool _closeSent;

        /// <summary>
        /// 如果WebSocket连接关闭，则为 true
        /// </summary>
        private volatile bool _closed;

        /// <summary>
        /// 当我们发出最后一个信号时。
        /// </summary>
        private DateTime _lastPing = DateTime.MinValue;

        /// <summary>
        /// 如果等待ping请求的应答，则为True。Ping超时仅用于waitingForPong为true的原因。
        /// </summary>
        private volatile bool _waitingForPong;

        /// <summary>
        /// 一个循环缓冲区，用于存储pong消息计算的最后N个rtt时间。
        /// </summary>
        // ReSharper disable once IdentifierTypo
        private readonly CircularBuffer<int> _rtts = new CircularBuffer<int>(WebSocketResponse.RTTBufferCapacity);


        /// <summary>
        ///它将在一帧内将给定的消息发送给服务器。
        /// </summary>
        public void Send(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            int count = Encoding.UTF8.GetByteCount(message);
            byte[] data = BufferPool.Get(count, true);
            Encoding.UTF8.GetBytes(message, 0, message.Length, data, 0);

            var frame = new WebSocketFrame(
                webSocket: this.WebSocket,
                type: WebSocketFrameTypes.Text,
                data: data,
                pos: 0,
                length: (ulong)count,
                isFinal: true,
                useExtensions: true);

            if (frame.Data != null && frame.Data.Length > this.MaxFragmentSize)
            {
                WebSocketFrame[] additionalFrames = frame.Fragment(this.MaxFragmentSize);

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

        /// <summary>
        /// 它将在一帧内将给定的数据发送给服务器。
        /// </summary>
        public void Send(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            WebSocketFrame frame = new WebSocketFrame(this.WebSocket, WebSocketFrameTypes.Binary, data);

            if (frame.Data != null && frame.Data.Length > this.MaxFragmentSize)
            {
                WebSocketFrame[] additionalFrames = frame.Fragment(this.MaxFragmentSize);

                Send(frame);
                if (additionalFrames == null) return;
                foreach (var t in additionalFrames)
                {
                    Send(t);
                }
            }
            else
            {
                Send(frame);
            }
        }

        /// <summary>
        /// 将从字节数组中发送count字节，从偏移量开始。
        /// </summary>
        public void Send(byte[] data, ulong offset, ulong count)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (offset + count > (ulong)data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            WebSocketFrame frame = new WebSocketFrame(
                webSocket: this.WebSocket,
                type: WebSocketFrameTypes.Binary,
                data: data,
                pos: offset,
                length: count,
                isFinal: true,
                useExtensions: true);

            if (frame.Data != null && frame.Data.Length > this.MaxFragmentSize)
            {
                var additionalFrames = frame.Fragment(this.MaxFragmentSize);

                Send(frame);

                if (additionalFrames == null) return;
                foreach (var t in additionalFrames)
                {
                    Send(t);
                }
            }
            else
            {
                Send(frame);
            }
        }

        /// <summary>
        /// 它将把给定的帧发送到服务器。
        /// </summary>
        public void Send(WebSocketFrame frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame));
            }

            if (_closed || _closeSent)
            {
                return;
            }

            this._unsentFrames.Enqueue(frame);

            if (Interlocked.CompareExchange(ref this._sendThreadCreated, 1, 0) == 0)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    StringBuilder sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}]");
                    sb.Append($"{sf.GetMethod().Name}");
                    sb.Append($"Line:{sf.GetFileLineNumber()}");
                    sb.Append($"[msg]Send - Creating thread");
                    Debug.Log($"{sb}");
                }
#endif
                PlatformSupport.Threading.ThreadedRunner.RunLongLiving(SendThreadFunc);
            }

            Interlocked.Add(ref this._bufferedAmount, frame.Data != null ? frame.DataLength : 0);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                StringBuilder sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}]");
                sb.Append($"{sf.GetMethod().Name}");
                sb.Append($"Line:{sf.GetFileLineNumber()}");
                sb.Append($"[msg]Signaling SendThread!");
                Debug.Log($"{sb}");
            }
#endif

            _newFrameSignal.Set();
        }

        /// <summary>
        /// 它将启动到服务器的连接的关闭。
        /// </summary>
        public void Close(UInt16 code = 1000, string msg = "Bye!")
        {
            if (_closed)
            {
                return;
            }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                StringBuilder sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}]");
                sb.Append($"{sf.GetMethod().Name}");
                sb.Append($"Line:{sf.GetFileLineNumber()}");
                sb.Append($"[msg]HandleEvents - Close({code}, \"{msg}\")");
                Debug.Log($"{sb}");
            }
#endif
            while (this._unsentFrames.TryDequeue(out _))
            {
            }
            //this.unsentFrames.Clear();

            Interlocked.Exchange(ref this._bufferedAmount, 0);
            var webSocketFrame = new WebSocketFrame(
                this.WebSocket,
                WebSocketFrameTypes.ConnectionClose,
                WebSocket.EncodeCloseData(code, msg));
            Send(webSocketFrame);
        }

        public void StartPinging(int frequency)
        {
            if (frequency < 100)
            {
                throw new ArgumentException("频率必须至少为100毫秒!");
            }

            PingFrequency = TimeSpan.FromMilliseconds(frequency);
            LastMessage = DateTime.UtcNow;

            SendPing();
        }

        private void SendThreadFunc()
        {
            try
            {
                using WriteOnlyBufferedStream bufferedStream = new WriteOnlyBufferedStream(this.Stream, 16 * 1024);
                while (!_closed && !_closeSent)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    {
                        var st = new StackTrace(new StackFrame(true));
                        var sf = st.GetFrame(0);
                        StringBuilder sb = new StringBuilder(6);
                        sb.Append($"[{sf.GetFileName()}]");
                        sb.Append($"[method:{sf.GetMethod().Name}]");
                        sb.Append($"{sf.GetMethod().Name}");
                        sb.Append($"Line:{sf.GetFileLineNumber()}");
                        sb.Append($"[msg]SendThread - Waiting...");
                        Debug.Log($"{sb}");
                    }
#endif

                    TimeSpan waitTime = TimeSpan.FromMilliseconds(int.MaxValue);

                    if (this.PingFrequency != TimeSpan.Zero)
                    {
                        DateTime now = DateTime.UtcNow;
                        waitTime = LastMessage + PingFrequency - now;

                        if (waitTime <= TimeSpan.Zero)
                        {
                            if (!_waitingForPong && now - LastMessage >= PingFrequency)
                            {
                                if (!SendPing())
                                    continue;
                            }

                            waitTime = PingFrequency;
                        }

                        if (_waitingForPong && now - _lastPing > this.WebSocket.CloseAfterNoMessage)
                        {
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                var st = new StackTrace(new StackFrame(true));
                                var sf = st.GetFrame(0);
                                StringBuilder sb = new StringBuilder(6);
                                sb.Append($"[{sf.GetFileName()}]");
                                sb.Append($"[method:{sf.GetMethod().Name}]");
                                sb.Append($"{sf.GetMethod().Name}");
                                sb.Append($"Line:{sf.GetFileLineNumber()}");
                                sb.Append("[msg|Exception] ");
                                sb.Append($"在给定的时间内没有收到任何消息!");
                                sb.Append($" Closing WebSocket. LastPing: {this._lastPing}");
                                sb.Append($" , PingFrequency: {this.PingFrequency},");
                                sb.Append($" Close After: {this.WebSocket.CloseAfterNoMessage},");
                                sb.Append($" Now: {now}");
                                Debug.Log(sb.ToString());
#endif
                            }
                            CloseWithError(HttpRequestStates.Error, "在给定的时间内没有收到任何消息!");
                            continue;
                        }
                    }

                    _newFrameSignal.WaitOne(waitTime);

                    try
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        {
                            var st = new StackTrace(new StackFrame(true));
                            var sf = st.GetFrame(0);
                            StringBuilder sb = new StringBuilder(6);
                            sb.Append($"[{sf.GetFileName()}]");
                            sb.Append($"[method:{sf.GetMethod().Name}]");
                            sb.Append($"{sf.GetMethod().Name}");
                            sb.Append($"Line:{sf.GetFileLineNumber()}");
                            sb.Append($"[msg]");
                            sb.Append($"SendThread - Wait is over, about ");
                            sb.Append($"{this._unsentFrames.Count.ToString()}");
                            sb.Append($" new frames!");
                            Debug.LogError($"{sb}");
                        }
#endif
                        while (this._unsentFrames.TryDequeue(out var frame))
                        {
                            if (!_closeSent)
                            {
                                using (var rawData = frame.Get())
                                    bufferedStream.Write(rawData.Data, 0, rawData.Length);

                                BufferPool.Release(frame.Data);

                                if (frame.Type == WebSocketFrameTypes.ConnectionClose)
                                {
                                    _closeSent = true;
                                }
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
                        {
                            this.BaseRequest.State = HttpRequestStates.Aborted;
                        }

                        _closed = true;
                    }
                }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    StringBuilder sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}]");
                    sb.Append($"{sf.GetMethod().Name}");
                    sb.Append($"Line:{sf.GetFileLineNumber()}");
                    sb.Append($"[msg]");
                    sb.Append($"Ending Send thread. ");
                    sb.Append($" Closed: {_closed},");
                    sb.Append($" closeSent: {_closeSent}");
                    Debug.LogError($"{sb}");
                }
#endif
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    StringBuilder sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}]");
                    sb.Append($"{sf.GetMethod().Name}");
                    sb.Append($"Line:{sf.GetFileLineNumber()}");
                    sb.Append($"[msg] SendThread [Exception] {ex}");
                    Debug.LogError($"{sb}");
                }
#endif
            }
            finally
            {
                Interlocked.Exchange(ref _sendThreadCreated, 0);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    StringBuilder sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}]");
                    sb.Append($"{sf.GetMethod().Name}");
                    sb.Append($"Line:{sf.GetFileLineNumber()}");
                    sb.Append($"[msg]SendThread - Closed!");
                    Debug.Log($"{sb}");
                }
#endif
                TryToCleanup();
            }
        }

        private void ReceiveThreadFunc()
        {
            try
            {
                while (!_closed)
                {
                    try
                    {
                        WebSocketFrameReader frame = new WebSocketFrameReader();
                        frame.Read(this.Stream);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        {
                            var st = new StackTrace(new StackFrame(true));
                            var sf = st.GetFrame(0);
                            StringBuilder sb = new StringBuilder(6);
                            sb.Append($"[{sf.GetFileName()}]");
                            sb.Append($"[method:{sf.GetMethod().Name}]");
                            sb.Append($"{sf.GetMethod().Name}");
                            sb.Append($"Line:{sf.GetFileLineNumber()}");
                            sb.Append($"[msg]Frame received: {frame.Type}");
                            Debug.Log($"{sb}");
                        }
#endif
                        LastMessage = DateTime.UtcNow;

                        //服务器不能屏蔽发送给客户端的任何帧。客户端在检测到屏蔽帧时必须关闭连接。
                        //在这种情况下，它可能使用状态码1002(协议错误)
                        //(这些规则在未来的规范中可能会被放宽。)
                        if (frame.HasMask)
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            {
                                var st = new StackTrace(new StackFrame(true));
                                var sf = st.GetFrame(0);
                                StringBuilder sb = new StringBuilder(6);
                                sb.Append($"[{sf.GetFileName()}]");
                                sb.Append($"[method:{sf.GetMethod().Name}]");
                                sb.Append($"{sf.GetMethod().Name}");
                                sb.Append($"Line:{sf.GetFileLineNumber()}");
                                sb.Append($"[msg]协议错误:从服务器收到屏蔽帧!");
                                Debug.Log($"{sb}");
                            }
#endif
                            Close(1002, "协议错误:从服务器收到屏蔽帧!");
                            continue;
                        }

                        if (!frame.IsFinal)
                        {
                            if (OnIncompleteFrame == null)
                            {
                                _incompleteFrames.Add(frame);
                            }
                            else
                            {
                                _completedFrames.Enqueue(frame);
                            }

                            continue;
                        }

                        switch (frame.Type)
                        {
                            // 有关碎片的完整文档和规则，请参阅 http://tools.ietf.org/html/rfc6455#section-5.4
                            // 一个碎片帧的最后一个片段的操作码是0(延续)，FIN位设置为1。
                            case WebSocketFrameTypes.Continuation:
                            {
                                // 只有在OnFragment未设置时才进行组装传递。否则，把它放在CompletedFrames中，我们将在HandleEvent阶段处理它。
                                if (OnIncompleteFrame == null)
                                {
                                    frame.Assemble(_incompleteFrames);

                                    //删除所有不完整的帧
                                    _incompleteFrames.Clear();

                                    // 控制帧本身一定不能碎片化。所以，它是一个正常的文本或二进制帧。去吧，像往常一样处理。
                                    goto case WebSocketFrameTypes.Binary;
                                }
                                else
                                {
                                    _completedFrames.Enqueue(frame);
                                    var protocolEventInfo = new ProtocolEventInfo(source: this);
                                    ProtocolEventHelper.EnqueueProtocolEvent(@event: protocolEventInfo);

                                }
                            }
                                break;

                            case WebSocketFrameTypes.Text:
                            case WebSocketFrameTypes.Binary:
                            {
                                frame.DecodeWithExtensions(webSocket: WebSocket);
                                _completedFrames.Enqueue(frame);
                                var protocolEventInfo = new ProtocolEventInfo(source: this);
                                ProtocolEventHelper.EnqueueProtocolEvent(@event: protocolEventInfo);
                            }
                                break;

                            // 在接收到Ping帧后，端点必须发送一个Pong帧作为响应，除非它已经收到了一个Close帧。
                            case WebSocketFrameTypes.Ping:
                            {
                                if (!_closeSent && !_closed)
                                {
                                    var webSocketFrame = new WebSocketFrame(
                                        webSocket: this.WebSocket,
                                        type: WebSocketFrameTypes.Pong,
                                        data: frame.Data);
                                    Send(webSocketFrame);
                                }
                            }
                                break;

                            case WebSocketFrameTypes.Pong:
                            {
                                try
                                {
                                    // 当前时间与发送ping消息时的时间差
                                    var diff = TimeSpan.FromTicks(
                                        value: this.LastMessage.Ticks - this._lastPing.Ticks);

                                    // 将其添加到缓冲区
                                    this._rtts.Add((int)diff.TotalMilliseconds);

                                    // 计算新的延迟
                                    this.Latency = CalculateLatency();
                                }
                                catch
                                {
                                    // https://tools.ietf.org/html/rfc6455#section-5.5
                                    //一个Pong帧可能被发送未经请求。这是一个
                                    //单向心跳。对一个不请自来的Pong框架的回应是
                                    //不期望。
                                }
                                finally
                                {
                                    _waitingForPong = false;
                                }
                            }
                                break;

                            // 如果一个端点接收到一个关闭帧，并且之前没有发送一个关闭帧，端点必须发送一个关闭帧作为响应。
                            case WebSocketFrameTypes.ConnectionClose:
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                {
                                    var st = new StackTrace(new StackFrame(true));
                                    var sf = st.GetFrame(0);
                                    StringBuilder sb = new StringBuilder(6);
                                    sb.Append($"[{sf.GetFileName()}]");
                                    sb.Append($"[method:{sf.GetMethod().Name}]");
                                    sb.Append($"{sf.GetMethod().Name}");
                                    sb.Append($"Line:{sf.GetFileLineNumber()}");
                                    sb.Append($"[msg]ConnectionClose packet received!");
                                    Debug.Log($"{sb}");
                                }
#endif
                                _closeFrame = frame;
                                if (!_closeSent)
                                {
                                    var webSocketFrame = new WebSocketFrame(
                                        webSocket: this.WebSocket,
                                        type: WebSocketFrameTypes.ConnectionClose,
                                        data: null);
                                    Send(webSocketFrame);
                                }

                                _closed = true;
                            }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
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
                        {
                            this.BaseRequest.State = HttpRequestStates.Aborted;
                        }

                        _closed = true;
                        _newFrameSignal.Set();
                    }
                }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    StringBuilder sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}]");
                    sb.Append($"{sf.GetMethod().Name}");
                    sb.Append($"Line:{sf.GetFileLineNumber()}");
                    sb.Append($"[msg] Ending Read thread! closed: {_closed}");
                    Debug.Log($"{sb}");
                }
#endif
            }
            finally
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                {
                    var st = new StackTrace(new StackFrame(true));
                    var sf = st.GetFrame(0);
                    StringBuilder sb = new StringBuilder(6);
                    sb.Append($"[{sf.GetFileName()}]");
                    sb.Append($"[method:{sf.GetMethod().Name}]");
                    sb.Append($"{sf.GetMethod().Name}");
                    sb.Append($"Line:{sf.GetFileLineNumber()}");
                    sb.Append($"[msg]ReceiveThread - Closed!");
                    Debug.Log($"{sb}");
                }
#endif
                TryToCleanup();
            }
        }
    }
}

#endif
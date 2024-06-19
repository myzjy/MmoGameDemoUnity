#if !BESTHTTP_DISABLE_SIGNALR_CORE
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using BestHTTP.Core;
using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.Memory;
using FrostEngine;

namespace BestHTTP.SignalRCore.Transports
{
    /// <summary>
    /// LongPolling transport implementation.
    /// https://github.com/aspnet/AspNetCore/blob/master/src/SignalR/docs/specs/TransportProtocols.md#http-post-client-to-server-only
    /// https://github.com/aspnet/AspNetCore/blob/master/src/SignalR/docs/specs/TransportProtocols.md#long-polling-server-to-client-only
    /// </summary>
    internal sealed class LongPollingTransport : TransportBase
    {
        /// <summary>
        /// 失败请求的最大重试次数
        /// </summary>
        const int MaxRetries = 6;

        /// <summary>
        /// 轮询传输在之前的发送消息请求未完成之前不能发送新的发送消息请求，因此它必须缓存新的发送消息请求。
        /// </summary>
        private ConcurrentQueue<BufferSegment> outgoingMessages = new ConcurrentQueue<BufferSegment>();

        /// <summary>
        /// 指示发送请求已经发出的标志。我们必须缓存消息(<see cref="outgoingMessages"/>) ，直到请求完成。
        /// </summary>
        private int sendingInProgress;

        /// <summary>
        /// 缓存流实例。使用<see cref="BufferSegmentStream"/>  ;我们可以避免为缓存消息分配byte[]，并将字节复制到新数组。
        /// </summary>
        private BufferSegmentStream stream = new BufferSegmentStream();

        internal LongPollingTransport(HubConnection con)
            : base(con)
        {
        }

        public override TransportTypes TransportType
        {
            get { return TransportTypes.LongPolling; }
        }

        public override void StartConnect()
        {
            if (this.State != TransportStates.Initial)
            {
                return;
            }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[LongPollingTransport] [method:StartConnect] [msg] StartConnect");
#endif
            this.State = TransportStates.Connecting;

            // https://github.com/dotnet/aspnetcore/blob/master/src/SignalR/docs/specs/HubProtocol.md#overview
            // When our connection is open, send the 'negotiation' message to the server.

            var request = new HttpRequest(BuildUri(this.Connection.Uri), HttpMethods.Post, OnHandshakeRequestFinished);
            request.Context.Add("Transport", this.Context);

            this.stream.Reset();
            this.stream.Write(JsonProtocol.WithSeparator(string.Format("{{\"protocol\":\"{0}\", \"version\": 1}}",
                this.Connection.Protocol.Name)));

            request.UploadStream = this.stream;

            if (this.Connection.AuthenticationProvider != null)
            {
                this.Connection.AuthenticationProvider.PrepareRequest(request);
            }

            request.Send();
        }

        public override void Send(BufferSegment msg)
        {
            if (this.State != TransportStates.Connected)
                return;

            outgoingMessages.Enqueue(msg);

            SendMessages();
        }

        public override void StartClose()
        {
            if (this.State != TransportStates.Connected)
            {
                return;
            }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[LongPollingTransport] [method:StartClose] [msg] StartClose");
#endif
            this.State = TransportStates.Closing;

            SendConnectionCloseRequest();
        }

        private void SendMessages()
        {
            if (this.State != TransportStates.Connected || this.outgoingMessages.Count == 0)
                return;

            if (Interlocked.CompareExchange(ref this.sendingInProgress, 1, 0) == 1)
                return;

            var request = new HttpRequest(BuildUri(this.Connection.Uri), HttpMethods.Post, OnSendMessagesFinished);
            request.Context.Add("Transport", this.Context);

            this.stream.Reset();

            BufferSegment buffer;
            while (this.outgoingMessages.TryDequeue(out buffer))
                this.stream.Write(buffer);

            request.UploadStream = this.stream;

            request.Tag = 0;

            if (this.Connection.AuthenticationProvider != null)
                this.Connection.AuthenticationProvider.PrepareRequest(request);

            request.Send();
        }

        private void DoPoll()
        {
            if (this.State != TransportStates.Connecting && this.State != TransportStates.Connected)
            {
                return;
            }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[LongPollingTransport] [method:DoPoll] [msg] Sending Poll request");
#endif
            var request = new HttpRequest(BuildUri(this.Connection.Uri), OnPollRequestFinished);
            request.Context.Add("Transport", this.Context);

            request.AddHeader("Accept", " application/octet-stream");
            request.Timeout = TimeSpan.FromMinutes(2);

            if (this.Connection.AuthenticationProvider != null)
            {
                this.Connection.AuthenticationProvider.PrepareRequest(request);
            }

            request.Send();
        }

        private void SendConnectionCloseRequest(int retryCount = 0)
        {
            if (this.State != TransportStates.Closing)
            {
                return;
            }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[LongPollingTransport] [method:SendConnectionCloseRequest] [msg] Sending DELETE request");
#endif
            var request = new HttpRequest(BuildUri(this.Connection.Uri), HttpMethods.Delete,
                OnConnectionCloseRequestFinished);
            request.Context.Add("Transport", this.Context);

            request.Tag = retryCount;

            if (this.Connection.AuthenticationProvider != null)
            {
                this.Connection.AuthenticationProvider.PrepareRequest(request);
            }

            request.Send();
        }

        private void OnHandshakeRequestFinished(HttpRequest req, HttpResponse resp)
        {
            switch (req.State)
            {
                // The request finished without any problem.
                case HttpRequestStates.Finished:
                    if (resp.IsSuccess)
                    {
                        // start listening for a handshake response
                        DoPoll();
                    }
                    else
                    {
                        this.ErrorReason = string.Format(
                            "Handshake Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                            resp.StatusCode,
                            resp.Message,
                            resp.DataAsText);
                    }

                    break;

                // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
                case HttpRequestStates.Error:
                    this.ErrorReason = "Handshake Request Finished with Error! " + (req.Exception != null
                        ? (req.Exception.Message + "\n" + req.Exception.StackTrace)
                        : "No Exception");
                    break;

                // The request aborted, initiated by the user.
                case HttpRequestStates.Aborted:
                    this.ErrorReason = "握手请求中止!";
                    break;

                // Connecting to the server is timed out.
                case HttpRequestStates.ConnectionTimedOut:
                    this.ErrorReason = "握手-连接超时!";
                    break;

                // The request didn't finished in the given time.
                case HttpRequestStates.TimedOut:
                    this.ErrorReason = "握手-处理请求超时!";
                    break;
            }

            if (!string.IsNullOrEmpty(this.ErrorReason))
                this.State = TransportStates.Failed;

            // 为了跳过处理流(因为我们重用它)，将请求的UploadStream设置为空
            req.UploadStream = null;
        }

        private void OnSendMessagesFinished(HttpRequest req, HttpResponse resp)
        {
            /*
             * HTTP POST请求发送到URL [endpoint-base]。返回id查询字符串值用于标识要发送到的连接。
             * 如果没有id查询字符串值，则返回400 Bad Request响应。在收到全部有效载荷后，
             * 服务器将处理有效负载，如果有效负载被成功处理，则响应200 OK。
             * 如果客户端发出另一个请求/而现有的请求是未完成的，新的请求将立即被服务器终止409冲突状态码。
             *
             * 如果客户端收到409冲突请求，连接保持打开状态。
             * 任何其他响应表示连接因错误而终止。
             *
             * 如果相关连接已被终止，则返回404 Not Found状态码。
             * 如果实例化端点或发送消息出错，则返回500 Server error状态码。
             * */
            switch (req.State)
            {
                // 请求顺利完成了。
                case HttpRequestStates.Finished:
                {
                    switch (resp.StatusCode)
                    {
                        // 在接收到整个有效负载后，服务器将处理有效负载，如果成功处理了有效负载，则响应200 OK。
                        case 200:
                        {
                            Interlocked.Exchange(ref this.sendingInProgress, 0);

                            // 连接是OK的，使用空列表调用OnMessages来更新HubConnection的lastMessageReceivedAt。
                            this.Messages.Clear();
                            try
                            {
                                this.Connection.OnMessages(this.Messages);
                            }
                            finally
                            {
                                this.Messages.Clear();
                            }

                            SendMessages();
                        }
                            break;

                        // 任何其他响应都表示连接由于错误而终止。
                        default:
                        {
                            this.ErrorReason =
                                $"发送请求成功完成，但服务器发送了一个错误。状态码: {resp.StatusCode}-{resp.Message} 消息: {resp.DataAsText}";
                        }
                            break;
                    }
                }

                    break;

                default:
                    int retryCount = (int)req.Tag;
                    if (retryCount < MaxRetries)
                    {
                        req.Tag = retryCount + 1;
                        RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(req, RequestEvents.Resend));
                    }
                    else
                    {
                        this.ErrorReason = $"发送消息达到最大重试次数({MaxRetries})!";
                    }

                    break;
            }

            if (!string.IsNullOrEmpty(this.ErrorReason))
            {
                this.State = TransportStates.Failed;
            }

            // 为了跳过处理流(因为我们重用它)，将请求的UploadStream设置为空
            req.UploadStream = null;
        }

        private void OnPollRequestFinished(HttpRequest req, HttpResponse resp)
        {
            /*
             * 当数据可用时，服务器用以下两种格式之一的正文响应(取决于Accept报头的值)。
             * 响应可能被分块，按照HTTP规范的分块编码部分。
             *
             * 如果缺少id参数，则返回400 Bad Request响应。
             * 如果与ID中指定的ID没有连接，则返回404 Not Found响应。
             *
             * 当客户端完成连接时，它可以向[endpoint-base]发出DELETE请求(带有查询字符串中的id)以优雅地终止连接。
             * 服务器将用204完成最近的轮询，表明它已经关闭。
             * */
            switch (req.State)
            {
                // The request finished without any problem.
                case HttpRequestStates.Finished:
                    switch (resp.StatusCode)
                    {
                        case 200:
                        {
                            int offset = 0;

                            // 仅当传输仍处于连接状态时才解析和分派消息
                            if (this.State == TransportStates.Connecting)
                            {
                                int idx = Array.IndexOf<byte>(resp.Data, (byte)JsonProtocol.Separator, 0);
                                if (idx > 0)
                                {
                                    base.HandleHandshakeResponse(
                                        System.Text.Encoding.UTF8.GetString(resp.Data, 0, idx));
                                    offset = idx + 1;

                                    if (this.State == TransportStates.Connected)
                                    {
                                        SendMessages();
                                    }
                                }
                                else
                                {
                                    DoPoll();
                                }
                            }

                            if (this.State == TransportStates.Connected)
                            {
                                this.Messages.Clear();
                                try
                                {
                                    if (resp.Data.Length - offset > 0)
                                    {
                                        this.Connection.Protocol.ParseMessages(
                                            new BufferSegment(resp.Data, offset, resp.Data.Length - offset),
                                            ref this.Messages);
                                    }
                                    else
                                    {
                                        this.Messages.Add(new Messages.Message
                                            { type = SignalRCore.Messages.MessageTypes.Ping });
                                    }

                                    this.Connection.OnMessages(this.Messages);
                                }
                                catch (Exception ex)
                                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                    Debug.LogError(
                                        $"[LongPollingTransport] [method:OnPollRequestFinished] [msg|Exception] OnMessage(byte[]) [Exception] {ex}");
#endif
                                }
                                finally
                                {
                                    this.Messages.Clear();

                                    DoPoll();
                                }
                            }
                            else if (this.State == TransportStates.Closing)
                            {
                                // 当我们收到投票结果时发出的DELETE消息。此时我们可以关闭传输，因为我们不想发送新的投票请求。
                                this.State = TransportStates.Closed;
                            }
                        }
                            break;

                        case 204:
                            this.State = TransportStates.Closed;
                            break;

                        case 400:
                        case 404:
                            if (this.State == TransportStates.Closing)
                            {
                                this.State = TransportStates.Closed;
                            }
                            else if (this.State != TransportStates.Closed)
                            {
                                this.ErrorReason = resp.DataAsText;
                            }

                            break;

                        default:
                            this.ErrorReason = string.Format(
                                "轮询请求成功完成，但服务器发送了一个错误。状态码: {0}-{1} 消息: {2}",
                                resp.StatusCode,
                                resp.Message,
                                resp.DataAsText);
                            break;
                    }

                    break;

                default:
                    if (this.State == TransportStates.Closing)
                        this.State = TransportStates.Closed;
                    else if (this.State != TransportStates.Closed)
                        DoPoll();
                    break;
            }

            if (!string.IsNullOrEmpty(this.ErrorReason))
                this.State = TransportStates.Failed;
        }

        private void OnConnectionCloseRequestFinished(HttpRequest req, HttpResponse resp)
        {
            switch (req.State)
            {
                // The request finished without any problem.
                case HttpRequestStates.Finished:
                    if (resp.IsSuccess)
                    {
                        return;
                    }
                    else
                    {
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            StringBuilder sb = new StringBuilder();
                            sb.Append("[LongPollingTransport] ");
                            sb.Append("[method:OnConnectionCloseRequestFinished] ");
                            sb.Append("[msg|Exception] ");
                            sb.Append($"连接关闭请求成功完成,");
                            sb.Append($" 但是服务器发送了一个错误。状态码: {resp.StatusCode}");
                            sb.Append($"-{resp.Message} Message: {resp.DataAsText}");
                            Debug.Log(sb.ToString());
#endif
                        }
                    }

                    break;

                // 请求以意外错误结束。请求的Exception属性可能包含有关错误的更多信息。
                case HttpRequestStates.Error:
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    StringBuilder sb = new StringBuilder();
                    sb.Append("[LongPollingTransport] ");
                    sb.Append("[method:OnPollRequestFinished] ");
                    sb.Append("[msg|Exception] ");
                    sb.Append($"连接关闭请求失败!!!! ");
                    sb.Append((req.Exception != null
                        ? ($"{req.Exception.Message}\n{req.Exception.StackTrace}")
                        : "No Exception"));
                    Debug.Log(sb.ToString());
#endif
                }
                    break;

                // 请求中止，由用户发起。
                case HttpRequestStates.Aborted:
                {
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        StringBuilder sb = new StringBuilder();
                        sb.Append("[LongPollingTransport] ");
                        sb.Append("[method:OnPollRequestFinished] ");
                        sb.Append("[msg|Exception] ");
                        sb.Append($"Connection Close Request Aborted!");
                        Debug.Log(sb.ToString());
#endif
                    }
                }
                    break;

                // 连接服务器超时。
                case HttpRequestStates.ConnectionTimedOut:
                {
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        StringBuilder sb = new StringBuilder();
                        sb.Append("[LongPollingTransport] ");
                        sb.Append("[method:OnPollRequestFinished] ");
                        sb.Append("[msg|Exception] ");
                        sb.Append($"Connection Close - Connection Timed Out!");
                        Debug.Log(sb.ToString());
#endif
                    }
                }
                    break;

                // 请求没有在规定的时间内完成。
                case HttpRequestStates.TimedOut:
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    StringBuilder sb = new StringBuilder();
                    sb.Append("[LongPollingTransport] ");
                    sb.Append("[method:OnPollRequestFinished] ");
                    sb.Append("[msg|Exception] ");
                    sb.Append($"Connection Close - Processing the request Timed Out!");
                    Debug.Log(sb.ToString());
#endif
                }
                    break;
            }

            int retryCount = (int)req.Tag;
            if (retryCount <= MaxRetries)
            {
                // 如果出现错误，请重试
                SendConnectionCloseRequest(retryCount + 1);
            }
            else
            {
                this.State = TransportStates.Closed;
            }
        }
    }
}
#endif
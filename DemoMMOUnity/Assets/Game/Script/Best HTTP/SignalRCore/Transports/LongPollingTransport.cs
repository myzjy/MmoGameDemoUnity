#if !BESTHTTP_DISABLE_SIGNALR_CORE
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using BestHTTP.Core;
using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.Memory;

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
                    this.ErrorReason = "Handshake Request Aborted!";
                    break;

                // Connecting to the server is timed out.
                case HttpRequestStates.ConnectionTimedOut:
                    this.ErrorReason = "Handshake - Connection Timed Out!";
                    break;

                // The request didn't finished in the given time.
                case HttpRequestStates.TimedOut:
                    this.ErrorReason = "Handshake - Processing the request Timed Out!";
                    break;
            }

            if (!string.IsNullOrEmpty(this.ErrorReason))
                this.State = TransportStates.Failed;

            // To skip disposing the stream (because we reuse it), set the request's UploadStream to null
            req.UploadStream = null;
        }

        private void OnSendMessagesFinished(HttpRequest req, HttpResponse resp)
        {
            /*
             * The HTTP POST request is made to the URL [endpoint-base]. The mandatory id query string value is used to identify the connection to send to.
             * If there is no id query string value, a 400 Bad Request response is returned. Upon receipt of the entire payload,
             * the server will process the payload and responds with 200 OK if the payload was successfully processed.
             * If a client makes another request to / while an existing request is outstanding, the new request is immediately terminated by the server with the 409 Conflict status code.
             * 
             * If a client receives a 409 Conflict request, the connection remains open.
             * Any other response indicates that the connection has been terminated due to an error.
             * 
             * If the relevant connection has been terminated, a 404 Not Found status code is returned.
             * If there is an error instantiating an EndPoint or dispatching the message, a 500 Server Error status code is returned.
             * */
            switch (req.State)
            {
                // The request finished without any problem.
                case HttpRequestStates.Finished:
                    switch (resp.StatusCode)
                    {
                        // Upon receipt of the entire payload, the server will process the payload and responds with 200 OK if the payload was successfully processed.
                        case 200:
                            Interlocked.Exchange(ref this.sendingInProgress, 0);

                            // The connections is OK, call OnMessages with an empty list to update HubConnection's lastMessageReceivedAt.
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

                            break;

                        // Any other response indicates that the connection has been terminated due to an error.
                        default:
                            this.ErrorReason = string.Format(
                                "Send Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                resp.StatusCode,
                                resp.Message,
                                resp.DataAsText);
                            break;
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
                        this.ErrorReason = string.Format("Send message reached max retry count ({0})!", MaxRetries);
                    }

                    break;
            }

            if (!string.IsNullOrEmpty(this.ErrorReason))
                this.State = TransportStates.Failed;

            // To skip disposing the stream (because we reuse it), set the request's UploadStream to null
            req.UploadStream = null;
        }

        private void OnPollRequestFinished(HttpRequest req, HttpResponse resp)
        {
            /*
             * When data is available, the server responds with a body in one of the two formats below (depending upon the value of the Accept header).
             * The response may be chunked, as per the chunked encoding part of the HTTP spec.
             * 
             * If the id parameter is missing, a 400 Bad Request response is returned.
             * If there is no connection with the ID specified in id, a 404 Not Found response is returned.
             *
             * When the client has finished with the connection, it can issue a DELETE request to [endpoint-base] (with the id in the query string) to gracefully terminate the connection.
             * The server will complete the latest poll with 204 to indicate that it has shut down.
             * */
            switch (req.State)
            {
                // The request finished without any problem.
                case HttpRequestStates.Finished:
                    switch (resp.StatusCode)
                    {
                        case 200:
                            int offset = 0;

                            // Parse and dispatch messages only if the transport is still in connected state
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
                                        this.Messages.Add(new Messages.Message { type = SignalRCore.Messages.MessageTypes.Ping });
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
                                // DELETE message sent out while we received the poll result. We can close the transport at this point as we don't want to send out a new poll request.
                                this.State = TransportStates.Closed;
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
                                "Poll Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
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
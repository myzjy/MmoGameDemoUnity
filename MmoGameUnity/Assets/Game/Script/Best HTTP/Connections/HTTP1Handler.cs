#if !UNITY_WEBGL || UNITY_EDITOR

#if !BESTHTTP_DISABLE_CACHING
using System;
using BestHTTP.Caching;
using BestHTTP.Core;
using BestHTTP.Logger;
using BestHTTP.Timings;
#endif

namespace BestHTTP.Connections
{
    public sealed class HTTP1Handler : IHttpRequestHandler
    {
        private readonly HTTPConnection conn;
        private KeepAliveHeader _keepAlive;

        public HTTP1Handler(HTTPConnection conn)
        {
            this.Context = new LoggingContext(this);
            this.conn = conn;
        }

        public bool HasCustomRequestProcessor
        {
            get { return false; }
        }

        public KeepAliveHeader KeepAlive
        {
            get { return this._keepAlive; }
        }

        public bool CanProcessMultiple
        {
            get { return false; }
        }

        public LoggingContext Context { get; private set; }

        public void Process(HttpRequest request)
        {
        }

        public void RunHandler()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[HTTP1Handler] [method:RunHandler] [msg|Exception] [{this}]开始处理请求 '{this.conn.CurrentRequest.CurrentUri.ToString()}'");
#endif
            System.Threading.Thread.CurrentThread.Name = "BestHTTP.HTTP1 R&W";

            HttpConnectionStates proposedConnectionState = HttpConnectionStates.Processing;

            bool resendRequest = false;

            try
            {
                if (this.conn.CurrentRequest.IsCancellationRequested)
                {
                    return;
                }

#if !BESTHTTP_DISABLE_CACHING
                // 在我们发送请求之前设置缓存控制头
                if (!this.conn.CurrentRequest.DisableCache)
                {
                    HttpCacheService.SetHeaders(this.conn.CurrentRequest);
                }
#endif

                // 将请求写入流
                this.conn.CurrentRequest.QueuedAt = DateTime.MinValue;
                this.conn.CurrentRequest.ProcessingStarted = DateTime.UtcNow;
                this.conn.CurrentRequest.SendOutTo(this.conn.connector.Stream);
                this.conn.CurrentRequest.Timing.Add(TimingEventNames.Request_Sent);

                if (this.conn.CurrentRequest.IsCancellationRequested)
                {
                    return;
                }

                this.conn.CurrentRequest.OnCancellationRequested += OnCancellationRequested;

                // 从服务器接收响应
                bool received = Receive(this.conn.CurrentRequest);

                this.conn.CurrentRequest.Timing.Add(TimingEventNames.Response_Received);

                if (this.conn.CurrentRequest.IsCancellationRequested)
                {
                    return;
                }

                if (!received && this.conn.CurrentRequest.Retries < this.conn.CurrentRequest.MaxRetries)
                {
                    proposedConnectionState = HttpConnectionStates.Closed;
                    this.conn.CurrentRequest.Retries++;
                    resendRequest = true;
                    return;
                }

                ConnectionHelper.HandleResponse(this.conn.ToString(), this.conn.CurrentRequest, out resendRequest,
                    out proposedConnectionState, ref this._keepAlive, this.conn.Context,
                    this.conn.CurrentRequest.Context);
            }
            catch (TimeoutException e)
            {
                this.conn.CurrentRequest.Response = null;

                // Do nothing here if Abort() got called on the request, its State is already set.
                if (!this.conn.CurrentRequest.IsTimedOut)
                {
                    // We will try again only once
                    if (this.conn.CurrentRequest.Retries < this.conn.CurrentRequest.MaxRetries)
                    {
                        this.conn.CurrentRequest.Retries++;
                        resendRequest = true;
                    }
                    else
                    {
                        this.conn.CurrentRequest.Exception = e;
                        this.conn.CurrentRequest.State = HttpRequestStates.ConnectionTimedOut;
                    }
                }

                proposedConnectionState = HttpConnectionStates.Closed;
            }
            catch (Exception e)
            {
                if (this.ShutdownType == ShutdownTypes.Immediate)
                    return;

                string exceptionMessage = string.Empty;
                if (e == null)
                    exceptionMessage = "null";
                else
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    Exception exception = e;
                    int counter = 1;
                    while (exception != null)
                    {
                        sb.AppendFormat("{0}: {1} {2}", counter++.ToString(), exception.Message, exception.StackTrace);

                        exception = exception.InnerException;

                        if (exception != null)
                            sb.AppendLine();
                    }

                    exceptionMessage = sb.ToString();
                }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log($"[HTTP1Handler] [method:RunHandler] [msg|Exception] {exceptionMessage}");
#endif

#if !BESTHTTP_DISABLE_CACHING
                if (this.conn.CurrentRequest.UseStreaming)
                    HttpCacheService.DeleteEntity(this.conn.CurrentRequest.CurrentUri);
#endif

                // Something gone bad, Response must be null!
                this.conn.CurrentRequest.Response = null;

                // Do nothing here if Abort() got called on the request, its State is already set.
                if (!this.conn.CurrentRequest.IsCancellationRequested)
                {
                    this.conn.CurrentRequest.Exception = e;
                    this.conn.CurrentRequest.State = HttpRequestStates.Error;
                }

                proposedConnectionState = HttpConnectionStates.Closed;
            }
            finally
            {
                this.conn.CurrentRequest.OnCancellationRequested -= OnCancellationRequested;

                // Exit ASAP
                if (this.ShutdownType != ShutdownTypes.Immediate)
                {
                    if (this.conn.CurrentRequest.IsCancellationRequested)
                    {
                        // we don't know what stage the request is canceled, we can't safely reuse the tcp channel.
                        proposedConnectionState = HttpConnectionStates.Closed;

                        this.conn.CurrentRequest.Response = null;

                        // The request's State already set, or going to be set soon in RequestEvents.cs.
                        //this.conn.CurrentRequest.State = this.conn.CurrentRequest.IsTimedOut ? HTTPRequestStates.TimedOut : HTTPRequestStates.Aborted;
                    }
                    else if (resendRequest)
                    {
                        // Here introducing a ClosedResendRequest connection state, where we have to process the connection's state change to Closed
                        // than we have to resend the request.
                        // If we would send the Resend request here, than a few lines below the Closed connection state change,
                        //  request events are processed before connection events (just switching the EnqueueRequestEvent and EnqueueConnectionEvent wouldn't work
                        //  see order of ProcessQueues in HTTPManager.OnUpdate!) and it would pick this very same closing/closed connection!

                        if (proposedConnectionState == HttpConnectionStates.Closed ||
                            proposedConnectionState == HttpConnectionStates.ClosedResendRequest)
                            ConnectionEventHelper.EnqueueConnectionEvent(
                                new ConnectionEventInfo(this.conn, this.conn.CurrentRequest));
                        else
                            RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.conn.CurrentRequest,
                                RequestEvents.Resend));
                    }
                    else if (this.conn.CurrentRequest.Response != null && this.conn.CurrentRequest.Response.IsUpgraded)
                    {
                        proposedConnectionState = HttpConnectionStates.WaitForProtocolShutdown;
                    }
                    else if (this.conn.CurrentRequest.State == HttpRequestStates.Processing)
                    {
                        if (this.conn.CurrentRequest.Response != null)
                            this.conn.CurrentRequest.State = HttpRequestStates.Finished;
                        else
                        {
                            this.conn.CurrentRequest.Exception = new Exception(string.Format(
                                "[{0}] Remote server closed the connection before sending response header! Previous request state: {1}. Connection state: {2}",
                                this.ToString(),
                                this.conn.CurrentRequest.State.ToString(),
                                this.conn.State.ToString()));
                            this.conn.CurrentRequest.State = HttpRequestStates.Error;

                            proposedConnectionState = HttpConnectionStates.Closed;
                        }
                    }

                    this.conn.CurrentRequest = null;

                    if (proposedConnectionState == HttpConnectionStates.Processing)
                        proposedConnectionState = HttpConnectionStates.Recycle;

                    if (proposedConnectionState != HttpConnectionStates.ClosedResendRequest)
                        ConnectionEventHelper.EnqueueConnectionEvent(new ConnectionEventInfo(this.conn,
                            proposedConnectionState));
                }
            }
        }

        public ShutdownTypes ShutdownType { get; private set; }

        public void Shutdown(ShutdownTypes type)
        {
            this.ShutdownType = type;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void OnCancellationRequested(HttpRequest obj)
        {
            if (this.conn != null && this.conn.connector != null)
                this.conn.connector.Dispose();
        }

        private bool Receive(HttpRequest request)
        {
            SupportedProtocols protocol = HttpProtocolFactory.GetProtocolFromUri(request.CurrentUri);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[HTTPConnection] [method:Receive(HttpRequest request)] [msg] [{this.ToString()}] - Receive - protocol: {protocol.ToString()}");
#endif
            request.Response = HttpProtocolFactory.Get(protocol, request, this.conn.connector.Stream,
                request.UseStreaming, false);

            if (!request.Response.Receive())
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[HTTP1Handler] [method:Receive(HttpRequest request)] [msg] [{this.ToString()}] - Receive - 失败了!响应将为空，返回false。");
#endif
                request.Response = null;
                return false;
            }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[HTTP1Handler] [method:Receive(HttpRequest request)] [msg] [{this.ToString()}] - Receive - 成功完成了!");
#endif
            return true;
        }

        private void Dispose(bool disposing)
        {
        }

        ~HTTP1Handler()
        {
            Dispose(false);
        }
    }
}

#endif
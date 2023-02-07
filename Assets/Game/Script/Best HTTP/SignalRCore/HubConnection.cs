#if !BESTHTTP_DISABLE_SIGNALR_CORE

#if CSHARP_7_OR_LATER
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BestHTTP.Futures;
using BestHTTP.Logger;
using BestHTTP.PlatformSupport.Threading;
using BestHTTP.SignalRCore.Authentication;
using BestHTTP.SignalRCore.Messages;
using ZJYFrameWork.UISerializable.Manager;
#endif

namespace BestHTTP.SignalRCore
{
    public sealed class HubConnection : BestHTTP.Extensions.IHeartbeat
    {
        public static readonly object[] EmptyArgs = new object[0];
        private volatile int _state;

        private DateTime connectionStartedAt;
        private RetryContext currentContext;

        bool defaultReconnect = true;

        List<Message> delayedMessages;

        /// <summary>
        ///  存储所有期望从服务器返回值的已发送消息的回调。所有发送的消息都有一个唯一的invocationId，该id将从服务器发送回来。
        /// </summary>
        private ConcurrentDictionary<long, InvocationDefinition> invocations =
            new ConcurrentDictionary<long, InvocationDefinition>();

        /// <summary>
        /// 这将为插件发送的每条消息添加一个唯一的id。
        /// </summary>
        private long lastInvocationId = 1;

        private DateTime lastMessageReceivedAt;

        /// <summary>
        /// 当我们向服务器发送最后一条消息时。
        /// </summary>
        private DateTime lastMessageSentAt;

        /// <summary>
        /// 最后一个流参数的Id。
        /// </summary>
        private int lastStreamId = 1;

        private bool pausedInLastFrame;
        private DateTime reconnectAt;
        private DateTime reconnectStartTime = DateTime.MinValue;

        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// 这是我们存储方法名的地方 => 回调函数的映射。
        /// </summary>
        private ConcurrentDictionary<string, Subscription> subscriptions =
            new ConcurrentDictionary<string, Subscription>(StringComparer.OrdinalIgnoreCase);

        private List<TransportTypes> triedoutTransports = new List<TransportTypes>();

        public HubConnection(Uri hubUri, IProtocol protocol)
            : this(hubUri, protocol, new HubOptions())
        {
        }

        public HubConnection(Uri hubUri, IProtocol protocol, HubOptions options)
        {
            this.Context = new LoggingContext(this);

            this.Uri = hubUri;
            this.State = ConnectionStates.Initial;
            this.Options = options;
            this.Protocol = protocol;
            this.Protocol.Connection = this;
            this.AuthenticationProvider = new DefaultAccessTokenAuthenticator(this);
        }

        /// <summary>
        /// Hub端点的Uri
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// 此连接的当前状态。
        /// </summary>
        public ConnectionStates State
        {
            get { return (ConnectionStates)this._state; }
            private set { Interlocked.Exchange(ref this._state, (int)value); }
        }

        /// <summary>
        /// 当前活动的transport实例。
        /// </summary>
        public ITransport Transport { get; private set; }

        /// <summary>
        /// 将解析、编码和解码消息的IProtocol实现。
        /// </summary>
        public IProtocol Protocol { get; private set; }

        /// <summary>
        /// 用于验证连接的IAuthenticationProvider实现。
        /// </summary>
        public IAuthenticationProvider AuthenticationProvider { get; set; }

        /// <summary>
        /// 服务器发送的协商响应。
        /// </summary>
        public NegotiationResult NegotiationResult { get; private set; }

        /// <summary>
        ///用于创建HubConnection的选项。
        /// </summary>
        public HubOptions Options { get; private set; }

        /// <summary>
        /// 这个连接被重定向了多少次.
        /// </summary>
        public int RedirectCount { get; private set; }

        /// <summary>
        /// 当底层连接丢失时将使用的重新连接策略。缺省值为空。
        /// </summary>
        public IRetryPolicy ReconnectPolicy { get; set; }

        /// <summary>
        /// 此HubConnection实例的日志记录上下文。
        /// </summary>
        public LoggingContext Context { get; private set; }

        void BestHTTP.Extensions.IHeartbeat.OnHeartbeatUpdate(TimeSpan dif)
        {
            switch (this.State)
            {
                case ConnectionStates.Negotiating:
                case ConnectionStates.Authenticating:
                case ConnectionStates.Redirected:
                {
                    var now = DateTimeUtil.GetCurrEntTimeMilliseconds(DateTimeUtil.Now());
                    if (now >= this.connectionStartedAt + this.Options.ConnectTimeout)
                    {
                        if (this.AuthenticationProvider != null)
                        {
                            this.AuthenticationProvider.OnAuthenticationSucceded -= OnAuthenticationSucceded;
                            this.AuthenticationProvider.OnAuthenticationFailed -= OnAuthenticationFailed;

                            try
                            {
                                this.AuthenticationProvider.Cancel();
                            }
                            catch (Exception ex)
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                Debug.LogError(
                                    $"[HubConnection] [method:BestHTTP.Extensions.IHeartbeat.OnHeartbeatUpdate] [msg|Exception]AuthenticationProvider中的异常。取消 !");
#endif
                            }
                        }

                        if (this.Transport != null)
                        {
                            this.Transport.OnStateChanged -= Transport_OnStateChanged;
                            this.Transport.StartClose();
                        }

                        SetState(ConnectionStates.Closed,
                            $"在给定的时间内无法连接({this.Options.ConnectTimeout})!",
                            this.defaultReconnect);
                    }
                }
                    break;

                case ConnectionStates.Connected:
                {
                    if (this.delayedMessages?.Count > 0)
                    {
                        pausedInLastFrame = false;
                        try
                        {
                            // 如果有任何关闭消息清除任何其他。
                            int idx = this.delayedMessages.FindLastIndex(dm => dm.type == MessageTypes.Close);
                            if (idx > 0)
                            {
                                this.delayedMessages.RemoveRange(0, idx);
                            }

                            OnMessages(this.delayedMessages);
                        }
                        finally
                        {
                            this.delayedMessages.Clear();
                        }
                    }

                    // Still connected? Check pinging.
                    if (this.State == ConnectionStates.Connected)
                    {
                        if (this.Options.PingInterval != TimeSpan.Zero && DateTime.Now - this.lastMessageReceivedAt >=
                            this.Options.PingTimeoutInterval)
                        {
                            // 传输本身可能处于失败状态，也可能处于完全有效的状态，因此当我们不想从它接收任何东西时，我们必须尝试关闭它
                            if (this.Transport != null)
                            {
                                this.Transport.OnStateChanged -= Transport_OnStateChanged;
                                this.Transport.StartClose();
                            }

                            SetState(ConnectionStates.Closed,
                                $"PingInterval set to '{this.Options.PingInterval}' and no message is received since '{this.lastMessageReceivedAt}'. PingTimeoutInterval: '{this.Options.PingTimeoutInterval}'",
                                this.defaultReconnect);
                        }
                        else if (this.Options.PingInterval != TimeSpan.Zero &&
                                 connectionStartedAt - this.lastMessageSentAt >= this.Options.PingInterval)
                        {
                            SendMessage(new Message() { type = MessageTypes.Ping });
                        }
                    }
                }
                    break;

                case ConnectionStates.Reconnecting:
                {
                    if (this.reconnectAt != DateTime.MinValue && DateTime.Now >= this.reconnectAt)
                    {
                        this.delayedMessages?.Clear();
                        var now = DateTimeUtil.GetCurrEntTimeMilliseconds(DateTimeUtil.Now());
                        this.connectionStartedAt = now;
                        this.reconnectAt = DateTime.MinValue;
                        this.triedoutTransports.Clear();
                        this.StartConnect();
                    }
                }
                    break;
            }
        }

        /// <summary>
        /// 当连接重定向到一个新的uri时，将调用此事件。
        /// </summary>
        public event Action<HubConnection, Uri, Uri> OnRedirected;

        /// <summary>
        /// 当成功连接到集线器时调用此事件。
        /// </summary>
        public event Action<HubConnection> OnConnected;

        /// <summary>
        /// 当发生意外错误并关闭连接时，将调用此事件。
        /// </summary>
        public event Action<HubConnection, string> OnError;

        /// <summary>
        /// 当连接正常终止时调用此事件。
        /// </summary>
        public event Action<HubConnection> OnClosed;

        /// <summary>
        /// 每个服务器发送的消息都会调用此事件。当返回false时，插件不会对消息做进一步的处理。
        /// </summary>
        public event Func<HubConnection, Message, bool> OnMessage;

        /// <summary>
        /// 当HubConnection在失去底层连接后启动其重连接过程时调用。
        /// </summary>
        public event Action<HubConnection, string> OnReconnecting;

        /// <summary>
        /// 重新连接成功后调用。
        /// </summary>
        public event Action<HubConnection> OnReconnected;

        /// <summary>
        /// 调用与运输相关的事件。
        /// </summary>
        public event Action<HubConnection, ITransport, TransportEvents> OnTransportEvent;

        public void StartConnect()
        {
            if (this.State != ConnectionStates.Initial &&
                this.State != ConnectionStates.Redirected &&
                this.State != ConnectionStates.Reconnecting)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.LogWarning(
                    $"[HubConnection] [method:StartConnect] [msg] StartConnect -预期的初始或重定向状态，已获得 {this.State.ToString()}");
#endif
                return;
            }

            if (this.State == ConnectionStates.Initial)
            {
                var now = DateTimeUtil.GetCurrEntTimeMilliseconds(DateTimeUtil.Now());
                this.connectionStartedAt = now;
                HttpManager.Heartbeats.Subscribe(this);
            }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            StringBuilder sb = new StringBuilder(3);
            sb.Append($"StartConnect State: {this.State},");
            var atToString = this.connectionStartedAt.ToString(System.Globalization.CultureInfo.InvariantCulture);
            sb.Append($"connectionStartedAt: {atToString}");
            Debug.Log(
                $"[HubConnection] [method:StartConnect] [msg]{sb.ToString()}");

#endif
            if (this.AuthenticationProvider != null && this.AuthenticationProvider.IsPreAuthRequired)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[HubConnection] [method:StartConnect] [msg] StartConnect - Authenticating");
#endif

                SetState(ConnectionStates.Authenticating, null, this.defaultReconnect);

                this.AuthenticationProvider.OnAuthenticationSucceded += OnAuthenticationSucceded;
                this.AuthenticationProvider.OnAuthenticationFailed += OnAuthenticationFailed;

                // 启动身份验证过程
                this.AuthenticationProvider.StartAuthentication();
            }
            else
            {
                StartNegotiation();
            }
        }

        private void OnAuthenticationSucceded(IAuthenticationProvider provider)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[HubConnection] [method:OnAuthenticationSucceded] [msg] OnAuthenticationSucceded");
#endif
            this.AuthenticationProvider.OnAuthenticationSucceded -= OnAuthenticationSucceded;
            this.AuthenticationProvider.OnAuthenticationFailed -= OnAuthenticationFailed;

            StartNegotiation();
        }

        private void OnAuthenticationFailed(IAuthenticationProvider provider, string reason)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.LogError($"[HubConnection] [method:OnAuthenticationFailed] [msg] OnAuthenticationFailed: {reason}");
#endif
            this.AuthenticationProvider.OnAuthenticationSucceded -= OnAuthenticationSucceded;
            this.AuthenticationProvider.OnAuthenticationFailed -= OnAuthenticationFailed;

            SetState(ConnectionStates.Closed, reason, this.defaultReconnect);
        }

        private void StartNegotiation()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[HubConnection] [method:StartNegotiation] [msg] StartNegotiation");
#endif

            if (this.State == ConnectionStates.CloseInitiated)
            {
                SetState(ConnectionStates.Closed, null, this.defaultReconnect);
                return;
            }

#if !BESTHTTP_DISABLE_WEBSOCKET
            if (this.Options.SkipNegotiation && this.Options.PreferedTransport == TransportTypes.WebSocket)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log($"[HubConnection] [method:StartNegotiation] [msg] 跳过谈判");
#endif
                ConnectImpl(this.Options.PreferedTransport);

                return;
            }
#endif

            SetState(ConnectionStates.Negotiating, null, this.defaultReconnect);

            // https://github.com/dotnet/aspnetcore/blob/master/src/SignalR/docs/specs/TransportProtocols.md#post-endpoint-basenegotiate-request
            // Send out a negotiation request. While we could skip it and connect right with the websocket transport
            //  it might return with additional information that could be useful.

            UriBuilder builder = new UriBuilder(this.Uri);
            if (builder.Path.EndsWith("/"))
            {
                builder.Path += "negotiate";
            }
            else
            {
                builder.Path += "/negotiate";
            }

            string query = builder.Query;
            if (string.IsNullOrEmpty(query))
            {
                query = "negotiateVersion=1";
            }
            else
            {
                query = query.Remove(0, 1) + "&negotiateVersion=1";
            }

            builder.Query = query;

            var request = new HttpRequest(builder.Uri, HttpMethods.Post, OnNegotiationRequestFinished);
            request.Context.Add("Hub", this.Context);

            if (this.AuthenticationProvider != null)
            {
                this.AuthenticationProvider.PrepareRequest(request);
            }

            request.Send();
        }

        private void ConnectImpl(TransportTypes transport)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[HubConnection] [method:ConnectImpl] [msg] ConnectImpl - {transport}");
#endif
            switch (transport)
            {
#if !BESTHTTP_DISABLE_WEBSOCKET
                case TransportTypes.WebSocket:
                {
                    if (this.NegotiationResult != null && !IsTransportSupported("WebSockets"))
                    {
                        SetState(ConnectionStates.Closed,
                            "不能使用首选传输，因为服务器不支持“WebSockets”传输!",
                            this.defaultReconnect);
                        return;
                    }

                    this.Transport = new Transports.WebSocketTransport(this);
                    this.Transport.OnStateChanged += Transport_OnStateChanged;
                }
                    break;
#endif

                case TransportTypes.LongPolling:
                {
                    if (this.NegotiationResult != null && !IsTransportSupported("LongPolling"))
                    {
                        SetState(ConnectionStates.Closed,
                            "不能使用首选传输，因为服务器不支持'LongPolling'传输!",
                            this.defaultReconnect);
                        return;
                    }

                    this.Transport = new Transports.LongPollingTransport(this);
                    this.Transport.OnStateChanged += Transport_OnStateChanged;
                }
                    break;

                default:
                    SetState(ConnectionStates.Closed, "Unsupported transport: " + transport, this.defaultReconnect);
                    break;
            }

            try
            {
                if (this.OnTransportEvent != null)
                {
                    this.OnTransportEvent(this, this.Transport, TransportEvents.SelectedToConnect);
                }
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.LogError(
                    $"[HubConnection] [method:ConnectImpl] [msg] 用户代码中的OnTransportEvent异常! Exception:{ex.Message}");
#endif
            }

            this.Transport.StartConnect();
        }

        private bool IsTransportSupported(string transportName)
        {
            // https://github.com/dotnet/aspnetcore/blob/master/src/SignalR/docs/specs/TransportProtocols.md#post-endpoint-basenegotiate-request
            // If the negotiation response contains only the url and accessToken, no 'availableTransports' list is sent
            if (this.NegotiationResult.SupportedTransports == null)
            {
                return true;
            }

            return this.NegotiationResult.SupportedTransports.Any(t =>
                t.Name.Equals(transportName, StringComparison.OrdinalIgnoreCase));
        }

        private void OnNegotiationRequestFinished(HttpRequest req, HttpResponse resp)
        {
            if (this.State == ConnectionStates.Closed)
                return;

            if (this.State == ConnectionStates.CloseInitiated)
            {
                SetState(ConnectionStates.Closed, null, this.defaultReconnect);
                return;
            }

            string errorReason = null;

            switch (req.State)
            {
                // The request finished without any problem.
                case HttpRequestStates.Finished:
                {
                    if (resp.IsSuccess)
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        var sb = new StringBuilder(3);
                        sb.Append($"Negotiation Request");
                        sb.Append($" Finished Successfully!");
                        sb.Append($" Response: {resp.DataAsText}");
                        Debug.Log($"[HubConnection] [method:SetState] [msg] {sb.ToString()}");
#endif

                        // Parse negotiation
                        this.NegotiationResult = NegotiationResult.Parse(resp, out errorReason, this);

                        // Room for improvement: check validity of the negotiation result:
                        //  If url and accessToken is present, the other two must be null.
                        //  https://github.com/dotnet/aspnetcore/blob/master/src/SignalR/docs/specs/TransportProtocols.md#post-endpoint-basenegotiate-request

                        if (string.IsNullOrEmpty(errorReason))
                        {
                            if (this.NegotiationResult.Url != null)
                            {
                                this.SetState(ConnectionStates.Redirected, null, this.defaultReconnect);

                                if (++this.RedirectCount >= this.Options.MaxRedirects)
                                {
                                    errorReason = $"MaxRedirects ({this.Options.MaxRedirects:N0}) reached!";
                                }
                                else
                                {
                                    var oldUri = this.Uri;
                                    this.Uri = this.NegotiationResult.Url;

                                    if (this.OnRedirected != null)
                                    {
                                        try
                                        {
                                            this.OnRedirected(this, oldUri, Uri);
                                        }
                                        catch (Exception ex)
                                        {
                                            HttpManager.Logger.Exception("HubConnection",
                                                "OnNegotiationRequestFinished - OnRedirected", ex, this.Context);
                                        }
                                    }

                                    StartConnect();
                                }
                            }
                            else
                            {
                                ConnectImpl(this.Options.PreferedTransport);
                            }
                        }
                    }
                    else // Internal server error?
                    {
                        errorReason =
                            $"Negotiation Request Finished Successfully, but the server sent an error. Status Code: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText}";
                    }
                }
                    break;

                // 请求结束时出现意外错误。请求的Exception属性可能包含有关错误的更多信息。
                case HttpRequestStates.Error:
                {
                    errorReason = "Negotiation Request Finished with Error! " + (req.Exception != null
                        ? (req.Exception.Message + "\n" + req.Exception.StackTrace)
                        : "No Exception");
                }
                    break;

                // 由用户发起的请求中止。
                case HttpRequestStates.Aborted:
                    errorReason = "Negotiation Request Aborted!";
                    break;

                // 连接服务器超时。处理步骤
                case HttpRequestStates.ConnectionTimedOut:
                    errorReason = "Negotiation Request - Connection Timed Out!";
                    break;

                // 请求没有在规定的时间内完成。
                case HttpRequestStates.TimedOut:
                    errorReason = "Negotiation Request - 处理请求超时!";
                    break;
            }

            if (errorReason != null)
            {
                this.NegotiationResult = new NegotiationResult();
                this.NegotiationResult.NegotiationResponse = resp;

                SetState(ConnectionStates.Closed, errorReason, this.defaultReconnect);
            }
        }

        public void StartClose()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[HubConnection] [method:StartClose] [msg] StartClose");
#endif
            this.defaultReconnect = false;

            switch (this.State)
            {
                case ConnectionStates.Initial:
                {
                    SetState(ConnectionStates.Closed, null, this.defaultReconnect);
                }
                    break;

                case ConnectionStates.Authenticating:
                {
                    this.AuthenticationProvider.OnAuthenticationSucceded -= OnAuthenticationSucceded;
                    this.AuthenticationProvider.OnAuthenticationFailed -= OnAuthenticationFailed;
                    this.AuthenticationProvider.Cancel();
                    SetState(ConnectionStates.Closed, null, this.defaultReconnect);
                }
                    break;

                case ConnectionStates.Reconnecting:
                {
                    SetState(ConnectionStates.Closed, null, this.defaultReconnect);
                }
                    break;

                case ConnectionStates.CloseInitiated:
                case ConnectionStates.Closed:
                    // Already initiated/closed
                    break;

                default:
                {
                    if (HttpManager.IsQuitting)
                    {
                        SetState(ConnectionStates.Closed, null, this.defaultReconnect);
                    }
                    else
                    {
                        SetState(ConnectionStates.CloseInitiated, null, this.defaultReconnect);

                        if (this.Transport != null)
                        {
                            this.Transport.StartClose();
                        }
                    }
                }
                    break;
            }
        }

        public IFuture<TResult> Invoke<TResult>(string target, params object[] args)
        {
            Future<TResult> future = new Future<TResult>();

            long id = InvokeImp(target,
                args,
                (message) =>
                {
                    bool isSuccess = string.IsNullOrEmpty(message.error);
                    if (isSuccess)
                    {
                        future.Assign((TResult)this.Protocol.ConvertTo(typeof(TResult), message.result));
                    }
                    else
                    {
                        future.Fail(new Exception(message.error));
                    }
                },
                typeof(TResult));

            if (id < 0)
            {
                future.Fail(new Exception("Not in Connected state! Current state: " + this.State));
            }

            return future;
        }

        public IFuture<object> Send(string target, params object[] args)
        {
            Future<object> future = new Future<object>();

            long id = InvokeImp(target,
                args,
                (message) =>
                {
                    bool isSuccess = string.IsNullOrEmpty(message.error);
                    if (isSuccess)
                        future.Assign(message.item);
                    else
                        future.Fail(new Exception(message.error));
                },
                typeof(object));

            if (id < 0)
                future.Fail(new Exception("Not in Connected state! Current state: " + this.State));

            return future;
        }

        private long InvokeImp(string target, object[] args, Action<Message> callback, Type itemType,
            bool isStreamingInvocation = false)
        {
            if (this.State != ConnectionStates.Connected)
                return -1;

            bool blockingInvocation = callback == null;

            long invocationId =
                blockingInvocation ? 0 : System.Threading.Interlocked.Increment(ref this.lastInvocationId);
            var message = new Message
            {
                type = isStreamingInvocation ? MessageTypes.StreamInvocation : MessageTypes.Invocation,
                invocationId = blockingInvocation ? null : invocationId.ToString(),
                target = target,
                arguments = args,
                nonblocking = callback == null,
            };

            SendMessage(message);

            if (!blockingInvocation)
            {
                if (!this.invocations.TryAdd(invocationId,
                        new InvocationDefinition { callback = callback, returnType = itemType }))
                {
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        StringBuilder sb = new StringBuilder();
                        sb.Append("[HubConnection] ");
                        sb.Append("[method:InvokeImp] ");
                        sb.Append("[msg|Exception] ");
                        sb.Append($"InvokeImp - invocations already contains id: ");
                        sb.Append($"{invocationId}");
                        Debug.Log(sb.ToString());
#endif
                    }
                }
            }

            return invocationId;
        }

        internal void SendMessage(Message message)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"[HubConnection] [method:StartClose] [msg] SendMessage: {message.ToString()}");
#endif
            try
            {
                using (new WriteLock(this.rwLock))
                {
                    var encoded = this.Protocol.EncodeMessage(message);
                    if (encoded.Data == null) return;
                    this.lastMessageSentAt = DateTime.Now;
                    this.Transport.Send(encoded);
                }
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.LogError($"[HubConnection] [method:StartClose] [msg] SendMessage  Exception:{ex.Message}");
#endif
            }
        }

        public DownStreamItemController<TDown> GetDownStreamController<TDown>(string target, params object[] args)
        {
            long invocationId = System.Threading.Interlocked.Increment(ref this.lastInvocationId);

            var future = new Future<TDown>();
            future.BeginProcess();

            var controller = new DownStreamItemController<TDown>(this, invocationId, future);

            Action<Message> callback = (Message msg) =>
            {
                switch (msg.type)
                {
                    // StreamItem message contains only one item.
                    case MessageTypes.StreamItem:
                    {
                        if (controller.IsCanceled)
                            break;

                        TDown item = (TDown)this.Protocol.ConvertTo(typeof(TDown), msg.item);

                        future.AssignItem(item);
                        break;
                    }

                    case MessageTypes.Completion:
                    {
                        bool isSuccess = string.IsNullOrEmpty(msg.error);
                        if (isSuccess)
                        {
                            // While completion message must not contain any result, this should be future-proof
                            if (!controller.IsCanceled && msg.result != null)
                            {
                                TDown result = (TDown)this.Protocol.ConvertTo(typeof(TDown), msg.result);

                                future.AssignItem(result);
                            }

                            future.Finish();
                        }
                        else
                            future.Fail(new Exception(msg.error));

                        break;
                    }
                }
            };

            var message = new Message
            {
                type = MessageTypes.StreamInvocation,
                invocationId = invocationId.ToString(),
                target = target,
                arguments = args,
                nonblocking = false,
            };

            SendMessage(message);

            if (callback != null)
            {
                if (!this.invocations.TryAdd(invocationId,
                        new InvocationDefinition { callback = callback, returnType = typeof(TDown) }))
                {
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        StringBuilder sb = new StringBuilder();
                        sb.Append("[HubConnection] ");
                        sb.Append("[method:GetDownStreamController] ");
                        sb.Append("[msg|Exception] ");
                        sb.Append($"GetUpStreamController - invocations already contains id: ");
                        sb.Append($"{invocationId}");
                        Debug.Log(sb.ToString());
#endif
                    }
                }
            }

            return controller;
        }

        public UpStreamItemController<TResult> GetUpStreamController<TResult>(string target, int paramCount,
            bool downStream, object[] args)
        {
            Future<TResult> future = new Future<TResult>();
            future.BeginProcess();

            long invocationId = System.Threading.Interlocked.Increment(ref this.lastInvocationId);

            string[] streamIds = new string[paramCount];
            for (int i = 0; i < paramCount; i++)
                streamIds[i] = System.Threading.Interlocked.Increment(ref this.lastStreamId).ToString();

            var controller = new UpStreamItemController<TResult>(this, invocationId, streamIds, future);

            Action<Message> callback = (Message msg) =>
            {
                switch (msg.type)
                {
                    // StreamItem message contains only one item.
                    case MessageTypes.StreamItem:
                    {
                        if (controller.IsCanceled)
                            break;

                        TResult item = (TResult)this.Protocol.ConvertTo(typeof(TResult), msg.item);

                        future.AssignItem(item);
                        break;
                    }

                    case MessageTypes.Completion:
                    {
                        bool isSuccess = string.IsNullOrEmpty(msg.error);
                        if (isSuccess)
                        {
                            // While completion message must not contain any result, this should be future-proof
                            if (!controller.IsCanceled && msg.result != null)
                            {
                                TResult result = (TResult)this.Protocol.ConvertTo(typeof(TResult), msg.result);

                                future.AssignItem(result);
                            }

                            future.Finish();
                        }
                        else
                        {
                            var ex = new Exception(msg.error);
                            future.Fail(ex);
                        }

                        break;
                    }
                }
            };

            var messageToSend = new Message
            {
                type = downStream ? MessageTypes.StreamInvocation : MessageTypes.Invocation,
                invocationId = invocationId.ToString(),
                target = target,
                arguments = args,
                streamIds = streamIds,
                nonblocking = false,
            };

            SendMessage(messageToSend);

            if (!this.invocations.TryAdd(invocationId,
                    new InvocationDefinition { callback = callback, returnType = typeof(TResult) }))
            {
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    StringBuilder sb = new StringBuilder();
                    sb.Append("[HubConnection] ");
                    sb.Append("[method:SetState] ");
                    sb.Append("[msg|Exception] ");
                    sb.Append($"GetUpStreamController - invocations already contains id: ");
                    sb.Append($"{invocationId}");
                    Debug.Log(sb.ToString());
#endif
                }
            }

            return controller;
        }

        public void On(string methodName, Action callback)
        {
            On(methodName, null, (args) => callback());
        }

        public void On<T1>(string methodName, Action<T1> callback)
        {
            On(methodName, new Type[] { typeof(T1) }, (args) => callback((T1)args[0]));
        }

        public void On<T1, T2>(string methodName, Action<T1, T2> callback)
        {
            On(methodName,
                new Type[] { typeof(T1), typeof(T2) },
                (args) => callback((T1)args[0], (T2)args[1]));
        }

        public void On<T1, T2, T3>(string methodName, Action<T1, T2, T3> callback)
        {
            On(methodName,
                new Type[] { typeof(T1), typeof(T2), typeof(T3) },
                (args) => callback((T1)args[0], (T2)args[1], (T3)args[2]));
        }

        public void On<T1, T2, T3, T4>(string methodName, Action<T1, T2, T3, T4> callback)
        {
            On(methodName,
                new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) },
                (args) => callback((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]));
        }

        private void On(string methodName, Type[] paramTypes, Action<object[]> callback)
        {
            if (this.State >= ConnectionStates.CloseInitiated)
                throw new Exception("Hub connection already closing or closed!");

            this.subscriptions.GetOrAdd(methodName, _ => new Subscription())
                .Add(paramTypes, callback);
        }

        /// <summary>
        /// Remove all event handlers for <paramref name="methodName"/> that subscribed with an On call.
        /// </summary>
        public void Remove(string methodName)
        {
            if (this.State >= ConnectionStates.CloseInitiated)
                throw new Exception("Hub connection already closing or closed!");

            Subscription _;
            this.subscriptions.TryRemove(methodName, out _);
        }

        internal Subscription GetSubscription(string methodName)
        {
            Subscription subscribtion = null;
            this.subscriptions.TryGetValue(methodName, out subscribtion);
            return subscribtion;
        }

        internal Type GetItemType(long invocationId)
        {
            InvocationDefinition def;
            this.invocations.TryGetValue(invocationId, out def);
            return def.returnType;
        }

        internal void OnMessages(List<Message> messages)
        {
            this.lastMessageReceivedAt = DateTime.Now;

            if (pausedInLastFrame)
            {
                if (this.delayedMessages == null)
                    this.delayedMessages = new List<Message>(messages.Count);
                foreach (var msg in messages)
                    delayedMessages.Add(msg);

                messages.Clear();
            }

            for (int messageIdx = 0; messageIdx < messages.Count; ++messageIdx)
            {
                var message = messages[messageIdx];

                if (this.OnMessage != null)
                {
                    try
                    {
                        if (!this.OnMessage(this, message))
                            continue;
                    }
                    catch (Exception ex)
                    {
                        HttpManager.Logger.Exception("HubConnection", "Exception in OnMessage user code!", ex,
                            this.Context);
                    }
                }

                switch (message.type)
                {
                    case MessageTypes.Invocation:
                    {
                        Subscription subscribtion = null;
                        if (this.subscriptions.TryGetValue(message.target, out subscribtion))
                        {
                            for (int i = 0; i < subscribtion.callbacks.Count; ++i)
                            {
                                var callbackDesc = subscribtion.callbacks[i];

                                object[] realArgs = null;
                                try
                                {
                                    realArgs = this.Protocol.GetRealArguments(callbackDesc.ParamTypes,
                                        message.arguments);
                                }
                                catch (Exception ex)
                                {
                                    HttpManager.Logger.Exception("HubConnection",
                                        "OnMessages - Invocation - GetRealArguments", ex, this.Context);
                                }

                                try
                                {
                                    callbackDesc.Callback.Invoke(realArgs);
                                }
                                catch (Exception ex)
                                {
                                    HttpManager.Logger.Exception("HubConnection", "OnMessages - Invocation - Invoke",
                                        ex, this.Context);
                                }
                            }
                        }

                        break;
                    }

                    case MessageTypes.StreamItem:
                    {
                        long invocationId;
                        if (long.TryParse(message.invocationId, out invocationId))
                        {
                            InvocationDefinition def;
                            if (this.invocations.TryGetValue(invocationId, out def) && def.callback != null)
                            {
                                try
                                {
                                    def.callback(message);
                                }
                                catch (Exception ex)
                                {
                                    HttpManager.Logger.Exception("HubConnection", "OnMessages - StreamItem - callback",
                                        ex, this.Context);
                                }
                            }
                        }

                        break;
                    }

                    case MessageTypes.Completion:
                    {
                        long invocationId;
                        if (long.TryParse(message.invocationId, out invocationId))
                        {
                            InvocationDefinition def;
                            if (this.invocations.TryRemove(invocationId, out def) && def.callback != null)
                            {
                                try
                                {
                                    def.callback(message);
                                }
                                catch (Exception ex)
                                {
                                    HttpManager.Logger.Exception("HubConnection", "OnMessages - Completion - callback",
                                        ex, this.Context);
                                }
                            }
                        }

                        break;
                    }

                    case MessageTypes.Ping:
                        // Send back an answer
                        SendMessage(new Message() { type = MessageTypes.Ping });
                        break;

                    case MessageTypes.Close:
                        SetState(ConnectionStates.Closed, message.error, message.allowReconnect);
                        if (this.Transport != null)
                            this.Transport.StartClose();
                        return;
                }
            }
        }

        private void Transport_OnStateChanged(TransportStates oldState, TransportStates newState)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            var sb = new StringBuilder(3);
            sb.Append($"Transport_OnStateChanged - oldState: {oldState.ToString()}");
            sb.Append($" newState: {newState.ToString()}");
            Debug.Log($"[HubConnection] [method:Transport_OnStateChanged] [msg] {sb.ToString()}");
#endif
            if (this.State == ConnectionStates.Closed)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[HubConnection] [method:Transport_OnStateChanged] [msg] Transport_OnStateChanged - already closed!");
#endif
                return;
            }

            switch (newState)
            {
                case TransportStates.Connected:
                {
                    try
                    {
                        this.OnTransportEvent?.Invoke(this, this.Transport, TransportEvents.Connected);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.LogError(
                            $"[HubConnection] [method:Transport_OnStateChanged] [msg] Exception in OnTransportEvent user code! Exception:{ex.Message}");
#endif
                    }

                    SetState(ConnectionStates.Connected, null, this.defaultReconnect);
                }
                    break;

                case TransportStates.Failed:
                {
                    if (this.State == ConnectionStates.Negotiating && !HttpManager.IsQuitting)
                    {
                        try
                        {
                            this.OnTransportEvent?.Invoke(this, this.Transport, TransportEvents.FailedToConnect);
                        }
                        catch (Exception ex)
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.LogError(
                                $"[HubConnection] [method:Transport_OnStateChanged] [msg] Exception in OnTransportEvent user code! Exception:{ex.Message}");
#endif
                        }

                        this.triedoutTransports.Add(this.Transport.TransportType);

                        var nextTransport = GetNextTransportToTry();
                        if (nextTransport == null)
                        {
                            var reason = this.Transport.ErrorReason;
                            this.Transport = null;

                            SetState(ConnectionStates.Closed, reason, this.defaultReconnect);
                        }
                        else
                            ConnectImpl(nextTransport.Value);
                    }
                    else
                    {
                        try
                        {
                            this.OnTransportEvent?.Invoke(this, this.Transport, TransportEvents.ClosedWithError);
                        }
                        catch (Exception ex)
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.LogError(
                                $"[HubConnection] [method:Transport_OnStateChanged] [msg] Exception in OnTransportEvent user code! Exception:{ex.Message}");
#endif
                        }

                        var reason = this.Transport.ErrorReason;
                        this.Transport = null;

                        SetState(ConnectionStates.Closed, HttpManager.IsQuitting ? null : reason,
                            this.defaultReconnect);
                    }
                }
                    break;

                case TransportStates.Closed:
                {
                    try
                    {
                        this.OnTransportEvent?.Invoke(this, this.Transport, TransportEvents.Closed);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.LogError(
                            $"[HubConnection] [method:Transport_OnStateChanged] [msg] Exception in OnTransportEvent user code! Exception:{ex.Message}");
#endif
                    }

                    // 检查我们是否有延迟消息和关闭消息。如果有，也延迟SetState(Close)。
                    if (this.delayedMessages == null ||
                        this.delayedMessages.FindLast(dm => dm.type == MessageTypes.Close).type != MessageTypes.Close)
                    {
                        SetState(ConnectionStates.Closed, null, this.defaultReconnect);
                    }
                }
                    break;
            }
        }

        private TransportTypes? GetNextTransportToTry()
        {
            foreach (TransportTypes val in Enum.GetValues(typeof(TransportTypes)))
                if (!this.triedoutTransports.Contains(val) && IsTransportSupported(val.ToString()))
                    return val;

            return null;
        }

        private void SetState(ConnectionStates state, string errorReason, bool allowReconnect)
        {
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG

                StringBuilder sb = new StringBuilder(5);
                sb.Append($"SetState - from State: '{this.State}'");
                sb.Append($" to State: '{state}',");
                sb.Append($" errorReason: '{errorReason}', ");
                sb.Append($"allowReconnect: {allowReconnect}, ");
                sb.Append($"isQuitting: {HttpManager.IsQuitting}");
                Debug.Log(
                    $"[HubConnection] [method:SetState] [msg]{sb.ToString()}");
#endif
            }
            if (this.State == state)
            {
                return;
            }

            var previousState = this.State;

            this.State = state;

            switch (state)
            {
                case ConnectionStates.Initial:
                case ConnectionStates.Authenticating:
                case ConnectionStates.Negotiating:
                case ConnectionStates.CloseInitiated:
                    break;

                case ConnectionStates.Reconnecting:
                    break;

                case ConnectionStates.Connected:
                {
                    // 如果reconnectStartTime不是它的默认值，我们重新连接
                    if (this.reconnectStartTime != DateTime.MinValue)
                    {
                        try
                        {
                            if (this.OnReconnected != null)
                                this.OnReconnected(this);
                        }
                        catch (Exception ex)
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.LogError($"[HubConnection] [method:SetState] [msg(OnReconnected)] {ex}");
#endif
                        }
                    }
                    else
                    {
                        try
                        {
                            if (this.OnConnected != null)
                            {
                                this.OnConnected(this);
                            }
                        }
                        catch (Exception ex)
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.LogError(
                                $"[HubConnection] [method:SetState] [msg|Exception] Exception in OnConnected user code!{ex}");
#endif
                        }
                    }

                    this.lastMessageSentAt = DateTime.Now;
                    this.lastMessageReceivedAt = DateTime.Now;

                    // 清理重新连接相关字段
                    this.currentContext = new RetryContext();
                    this.reconnectStartTime = DateTime.MinValue;
                    this.reconnectAt = DateTime.MinValue;

                    HttpUpdateDelegator.OnApplicationForegroundStateChanged -=
                        this.OnApplicationForegroundStateChanged;
                    HttpUpdateDelegator.OnApplicationForegroundStateChanged +=
                        this.OnApplicationForegroundStateChanged;
                }
                    break;

                case ConnectionStates.Closed:
                {
                    // 检查所有调用并取消它们。
                    var error = new Message
                    {
                        type = MessageTypes.Close,
                        error = errorReason
                    };

                    foreach (var kvp in this.invocations)
                    {
                        try
                        {
                            kvp.Value.callback(error);
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    this.invocations.Clear();

                    // No errorReason? It's an expected closure.
                    if (errorReason == null && (!allowReconnect || HttpManager.IsQuitting))
                    {
                        if (this.OnClosed != null)
                        {
                            try
                            {
                                this.OnClosed(this);
                            }
                            catch (Exception ex)
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                Debug.LogError(
                                    $"[HubConnection] [method:SetState] [msg|Exception] Exception in OnConnected user code!{ex}");
#endif
                            }
                        }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log($"[HubConnection] [method:SetState] [msg] Cleaning up");
#endif

                        this.delayedMessages?.Clear();
                        HttpManager.Heartbeats.Unsubscribe(this);

                        this.rwLock?.Dispose();
                        this.rwLock = null;

                        HttpUpdateDelegator.OnApplicationForegroundStateChanged -=
                            this.OnApplicationForegroundStateChanged;
                    }
                    else
                    {
                        // If possible, try to reconnect
                        if (allowReconnect && this.ReconnectPolicy != null &&
                            (previousState == ConnectionStates.Connected ||
                             this.reconnectStartTime != DateTime.MinValue))
                        {
                            // 这是成功连接后的第一次尝试
                            if (this.reconnectStartTime == DateTime.MinValue)
                            {
                                var now = DateTimeUtil.GetCurrEntTimeMilliseconds(DateTimeUtil.Now());

                                this.connectionStartedAt = this.reconnectStartTime = now;

                                try
                                {
                                    if (this.OnReconnecting != null)
                                    {
                                        this.OnReconnecting(this, errorReason);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    HttpManager.Logger.Exception("HubConnection",
                                        "SetState - ConnectionStates.Reconnecting", ex, this.Context);
                                }
                            }

                            RetryContext context = new RetryContext
                            {
                                ElapsedTime = DateTime.Now - this.reconnectStartTime,
                                PreviousRetryCount = this.currentContext.PreviousRetryCount,
                                RetryReason = errorReason
                            };

                            TimeSpan? nextAttempt = null;
                            try
                            {
                                nextAttempt = this.ReconnectPolicy.GetNextRetryDelay(context);
                            }
                            catch (Exception ex)
                            {
                                HttpManager.Logger.Exception("HubConnection", "ReconnectPolicy.GetNextRetryDelay",
                                    ex,
                                    this.Context);
                            }

                            // No more reconnect attempt, we are closing
                            if (nextAttempt == null)
                            {
                                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                    StringBuilder sb = new StringBuilder();
                                    sb.Append("[HubConnection] ");
                                    sb.Append("[method:SetState] ");
                                    sb.Append("[msg|Exception] ");
                                    sb.Append($"No more reconnect attempt!");
                                    Debug.Log(sb.ToString());
#endif
                                }

                                // Clean up everything
                                this.currentContext = new RetryContext();
                                this.reconnectStartTime = DateTime.MinValue;
                                this.reconnectAt = DateTime.MinValue;
                            }
                            else
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                var sb = new StringBuilder(3);
                                sb.Append($"Next reconnect attempt after");
                                sb.Append($" {nextAttempt.Value.ToString()}");
                                Debug.Log($"[HubConnection] [method:SetState] [msg] {sb.ToString()}");
#endif
                                this.currentContext = context;
                                this.currentContext.PreviousRetryCount += 1;

                                this.reconnectAt = DateTime.Now + nextAttempt.Value;

                                this.SetState(ConnectionStates.Reconnecting, null, this.defaultReconnect);

                                return;
                            }
                        }

                        if (this.OnError != null)
                        {
                            try
                            {
                                this.OnError(this, errorReason);
                            }
                            catch (Exception ex)
                            {
                                HttpManager.Logger.Exception("HubConnection", "Exception in OnError user code!", ex,
                                    this.Context);
                            }
                        }
                    }
                }
                    break;
                case ConnectionStates.Redirected:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void OnApplicationForegroundStateChanged(bool isPaused)
        {
            pausedInLastFrame = !isPaused;
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            var sb = new StringBuilder(3);
            sb.Append($"OnApplicationForegroundStateChanged");
            sb.Append($" isPaused: {isPaused}");
            sb.Append($" pausedInLastFrame: {pausedInLastFrame}");
            Debug.Log($"[HubConnection] [method:OnApplicationForegroundStateChanged] [msg] {sb.ToString()}");
#endif
        }

#if CSHARP_7_OR_LATER

        TaskCompletionSource<HubConnection> connectAsyncTaskCompletionSource;

        public Task<HubConnection> ConnectAsync()
        {
            if (this.State != ConnectionStates.Initial && this.State != ConnectionStates.Redirected &&
                this.State != ConnectionStates.Reconnecting)
                throw new Exception("HubConnection - ConnectAsync - Expected Initial or Redirected state, got " +
                                    this.State.ToString());

            if (this.connectAsyncTaskCompletionSource != null)
                throw new Exception("Connect process already started!");

            this.connectAsyncTaskCompletionSource = new TaskCompletionSource<HubConnection>();

            this.OnConnected += OnAsyncConnectedCallback;
            this.OnError += OnAsyncConnectFailedCallback;

            this.StartConnect();

            return connectAsyncTaskCompletionSource.Task;
        }

        private void OnAsyncConnectedCallback(HubConnection hub)
        {
            this.OnConnected -= OnAsyncConnectedCallback;
            this.OnError -= OnAsyncConnectFailedCallback;

            this.connectAsyncTaskCompletionSource.TrySetResult(this);
            this.connectAsyncTaskCompletionSource = null;
        }

        private void OnAsyncConnectFailedCallback(HubConnection hub, string error)
        {
            this.OnConnected -= OnAsyncConnectedCallback;
            this.OnError -= OnAsyncConnectFailedCallback;

            this.connectAsyncTaskCompletionSource.TrySetException(new Exception(error));
            this.connectAsyncTaskCompletionSource = null;
        }

#endif

#if CSHARP_7_OR_LATER

        TaskCompletionSource<HubConnection> closeAsyncTaskCompletionSource;

        public Task<HubConnection> CloseAsync()
        {
            if (this.closeAsyncTaskCompletionSource != null)
                throw new Exception("CloseAsync already called!");

            this.closeAsyncTaskCompletionSource = new TaskCompletionSource<HubConnection>();

            this.OnClosed += OnClosedAsyncCallback;
            this.OnError += OnClosedAsyncErrorCallback;

            // Avoid race condition by caching task prior to StartClose,
            // which asynchronously calls OnClosedAsyncCallback, which nulls
            // this.closeAsyncTaskCompletionSource immediately before we have
            // a chance to read from it.
            var task = this.closeAsyncTaskCompletionSource.Task;

            this.StartClose();

            return task;
        }

        void OnClosedAsyncCallback(HubConnection hub)
        {
            this.OnClosed -= OnClosedAsyncCallback;
            this.OnError -= OnClosedAsyncErrorCallback;

            this.closeAsyncTaskCompletionSource.TrySetResult(this);
            this.closeAsyncTaskCompletionSource = null;
        }

        void OnClosedAsyncErrorCallback(HubConnection hub, string error)
        {
            this.OnClosed -= OnClosedAsyncCallback;
            this.OnError -= OnClosedAsyncErrorCallback;

            this.closeAsyncTaskCompletionSource.TrySetException(new Exception(error));
            this.closeAsyncTaskCompletionSource = null;
        }

#endif

#if CSHARP_7_OR_LATER

        public Task<TResult> InvokeAsync<TResult>(string target, params object[] args)
        {
            return InvokeAsync<TResult>(target, default(CancellationToken), args);
        }

        public Task<TResult> InvokeAsync<TResult>(string target, CancellationToken cancellationToken = default,
            params object[] args)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            long id = InvokeImp(target,
                args,
                (message) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        tcs.TrySetCanceled(cancellationToken);
                        return;
                    }

                    bool isSuccess = string.IsNullOrEmpty(message.error);
                    if (isSuccess)
                        tcs.TrySetResult((TResult)this.Protocol.ConvertTo(typeof(TResult), message.result));
                    else
                        tcs.TrySetException(new Exception(message.error));
                },
                typeof(TResult));

            if (id < 0)
                tcs.TrySetException(new Exception("Not in Connected state! Current state: " + this.State));
            else
                cancellationToken.Register(() => tcs.TrySetCanceled());

            return tcs.Task;
        }

#endif

#if CSHARP_7_OR_LATER

        public Task<object> SendAsync(string target, params object[] args)
        {
            return SendAsync(target, default(CancellationToken), args);
        }

        public Task<object> SendAsync(string target, CancellationToken cancellationToken = default,
            params object[] args)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            long id = InvokeImp(target,
                args,
                (message) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        tcs.TrySetCanceled(cancellationToken);
                        return;
                    }

                    bool isSuccess = string.IsNullOrEmpty(message.error);
                    if (isSuccess)
                        tcs.TrySetResult(message.item);
                    else
                        tcs.TrySetException(new Exception(message.error));
                },
                typeof(object));

            if (id < 0)
                tcs.TrySetException(new Exception("Not in Connected state! Current state: " + this.State));
            else
                cancellationToken.Register(() => tcs.TrySetCanceled());

            return tcs.Task;
        }

#endif
    }
}

#endif
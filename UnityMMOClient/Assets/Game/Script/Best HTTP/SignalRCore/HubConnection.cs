#if !BESTHTTP_DISABLE_SIGNALR_CORE

#if CSHARP_7_OR_LATER
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BestHTTP.Extensions;
using BestHTTP.Futures;
using BestHTTP.Logger;
using BestHTTP.PlatformSupport.Threading;
using BestHTTP.SignalRCore.Authentication;
using BestHTTP.SignalRCore.Messages;
using BestHTTP.SignalRCore.Transports;
using ZJYFrameWork.UISerializable.Manager;
#endif

namespace BestHTTP.SignalRCore
{
    public sealed class HubConnection : IHeartbeat
    {
        public static readonly object[] EmptyArgs = Array.Empty<object>();
        private volatile int _state;

        private DateTime _connectionStartedAt;
        private RetryContext _currentContext;

        bool _defaultReconnect = true;

        List<Message> _delayedMessages;

        /// <summary>
        ///  存储所有期望从服务器返回值的已发送消息的回调。所有发送的消息都有一个唯一的invocationId，该id将从服务器发送回来。
        /// </summary>
        private
            readonly 
            ConcurrentDictionary<long, InvocationDefinition> _invocations =
            new ConcurrentDictionary<long, InvocationDefinition>();

        /// <summary>
        /// 这将为插件发送的每条消息添加一个唯一的id。
        /// </summary>
        private long _lastInvocationId = 1;

        private DateTime _lastMessageReceivedAt;

        /// <summary>
        /// 当我们向服务器发送最后一条消息时。
        /// </summary>
        private DateTime _lastMessageSentAt;

        /// <summary>
        /// 最后一个流参数的Id。
        /// </summary>
        private int _lastStreamId = 1;

        private bool _pausedInLastFrame;
        private DateTime _reconnectAt;
        private DateTime _reconnectStartTime = DateTime.MinValue;

        private ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        /// 这是我们存储方法名的地方 => 回调函数的映射。
        /// </summary>
        private
            readonly
            ConcurrentDictionary<string, Subscription> _subscriptions =
            new ConcurrentDictionary<string, Subscription>(StringComparer.OrdinalIgnoreCase);

        // ReSharper disable once IdentifierTypo
        private readonly List<TransportTypes> _triedoutTransports = new List<TransportTypes>();

        public HubConnection(Uri hubUri, IProtocol protocol)
            : this(hubUri, protocol, new HubOptions())
        {
        }

        private HubConnection(Uri hubUri, IProtocol protocol, HubOptions options)
        {
            Context = new LoggingContext(this);

            Uri = hubUri;
            State = ConnectionStates.Initial;
            Options = options;
            Protocol = protocol;
            Protocol.Connection = this;
            AuthenticationProvider = new DefaultAccessTokenAuthenticator(this);
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
            get => (ConnectionStates)_state;
            private set => Interlocked.Exchange(ref _state, (int)value);
        }

        /// <summary>
        /// 当前活动的transport实例。
        /// </summary>
        private ITransport Transport { get; set; }

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
        private HubOptions Options { get; set; }

        /// <summary>
        /// 这个连接被重定向了多少次.
        /// </summary>
        private int RedirectCount { get; set; }

        /// <summary>
        /// 当底层连接丢失时将使用的重新连接策略。缺省值为空。
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public IRetryPolicy ReconnectPolicy { get; set; }

        /// <summary>
        /// 此HubConnection实例的日志记录上下文。
        /// </summary>
        public LoggingContext Context { get; private set; }

        void IHeartbeat.OnHeartbeatUpdate(TimeSpan dif)
        {
            switch (State)
            {
                case ConnectionStates.Negotiating:
                case ConnectionStates.Authenticating:
                case ConnectionStates.Redirected:
                {
                    var now = DateTimeUtil.GetCurrEntTimeMilliseconds(DateTimeUtil.Now());
                    if (now >= _connectionStartedAt + Options.ConnectTimeout)
                    {
                        if (AuthenticationProvider != null)
                        {
                            AuthenticationProvider.OnAuthenticationSucceed -= OnAuthenticationSucceed;
                            AuthenticationProvider.OnAuthenticationFailed -= OnAuthenticationFailed;

                            try
                            {
                                AuthenticationProvider.Cancel();
                            }
#pragma warning disable CS0168
                            catch (Exception ex)
#pragma warning restore CS0168
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                Debug.LogError(
                                    "[HubConnection] [method:BestHTTP.Extensions.IHeartbeat.OnHeartbeatUpdate] [msg|Exception]AuthenticationProvider中的异常。取消 !");
#endif
                            }
                        }

                        if (Transport != null)
                        {
                            Transport.OnStateChanged -= Transport_OnStateChanged;
                            Transport.StartClose();
                        }

                        SetState(ConnectionStates.Closed,
                            $"在给定的时间内无法连接({Options.ConnectTimeout})!",
                            _defaultReconnect);
                    }
                }
                    break;

                case ConnectionStates.Connected:
                {
                    if (_delayedMessages?.Count > 0)
                    {
                        _pausedInLastFrame = false;
                        try
                        {
                            // 如果有任何关闭消息清除任何其他。
                            int idx = _delayedMessages.FindLastIndex(dm => dm.type == MessageTypes.Close);
                            if (idx > 0)
                            {
                                _delayedMessages.RemoveRange(0, idx);
                            }

                            OnMessages(_delayedMessages);
                        }
                        finally
                        {
                            _delayedMessages.Clear();
                        }
                    }

                    // Still connected? Check pinging.
                    if (State == ConnectionStates.Connected)
                    {
                        if (Options.PingInterval != TimeSpan.Zero && DateTime.Now - _lastMessageReceivedAt >=
                            Options.PingTimeoutInterval)
                        {
                            // 传输本身可能处于失败状态，也可能处于完全有效的状态，因此当我们不想从它接收任何东西时，我们必须尝试关闭它
                            if (Transport != null)
                            {
                                Transport.OnStateChanged -= Transport_OnStateChanged;
                                Transport.StartClose();
                            }

                            SetState(ConnectionStates.Closed,
                                $"PingInterval set to '{Options.PingInterval}' and no message is received since '{_lastMessageReceivedAt}'. PingTimeoutInterval: '{Options.PingTimeoutInterval}'",
                                _defaultReconnect);
                        }
                        else if (Options.PingInterval != TimeSpan.Zero &&
                                 _connectionStartedAt - _lastMessageSentAt >= Options.PingInterval)
                        {
                            SendMessage(new Message { type = MessageTypes.Ping });
                        }
                    }
                }
                    break;

                case ConnectionStates.Reconnecting:
                {
                    if (_reconnectAt != DateTime.MinValue && DateTime.Now >= _reconnectAt)
                    {
                        _delayedMessages?.Clear();
                        var now = DateTimeUtil.GetCurrEntTimeMilliseconds(DateTimeUtil.Now());
                        _connectionStartedAt = now;
                        _reconnectAt = DateTime.MinValue;
                        _triedoutTransports.Clear();
                        StartConnect();
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

        private void StartConnect()
        {
            if (State != ConnectionStates.Initial &&
                State != ConnectionStates.Redirected &&
                State != ConnectionStates.Reconnecting)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.LogWarning(
                    $"[HubConnection] [method:StartConnect] [msg] StartConnect -预期的初始或重定向状态，已获得 {State.ToString()}");
#endif
                return;
            }

            if (State == ConnectionStates.Initial)
            {
                var now = DateTimeUtil.GetCurrEntTimeMilliseconds(DateTimeUtil.Now());
                _connectionStartedAt = now;
                HttpManager.Heartbeats.Subscribe(this);
            }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            StringBuilder sb = new StringBuilder(3);
            sb.Append($"StartConnect State: {State},");
            var atToString = _connectionStartedAt.ToString(CultureInfo.InvariantCulture);
            sb.Append($"connectionStartedAt: {atToString}");
            Debug.Log(
                $"[HubConnection] [method:StartConnect] [msg]{sb}");

#endif
            if (AuthenticationProvider is { IsPreAuthRequired: true })
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    "[HubConnection] [method:StartConnect] [msg] StartConnect - Authenticating");
#endif

                SetState(ConnectionStates.Authenticating, null, _defaultReconnect);

                AuthenticationProvider.OnAuthenticationSucceed += OnAuthenticationSucceed;
                AuthenticationProvider.OnAuthenticationFailed += OnAuthenticationFailed;

                // 启动身份验证过程
                AuthenticationProvider.StartAuthentication();
            }
            else
            {
                StartNegotiation();
            }
        }

        private void OnAuthenticationSucceed(IAuthenticationProvider provider)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("[HubConnection] [method:OnAuthenticationSucceed] [msg] OnAuthenticationSucceed");
#endif
            AuthenticationProvider.OnAuthenticationSucceed -= OnAuthenticationSucceed;
            AuthenticationProvider.OnAuthenticationFailed -= OnAuthenticationFailed;

            StartNegotiation();
        }

        private void OnAuthenticationFailed(IAuthenticationProvider provider, string reason)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.LogError($"[HubConnection] [method:OnAuthenticationFailed] [msg] OnAuthenticationFailed: {reason}");
#endif
            AuthenticationProvider.OnAuthenticationSucceed -= OnAuthenticationSucceed;
            AuthenticationProvider.OnAuthenticationFailed -= OnAuthenticationFailed;

            SetState(ConnectionStates.Closed, reason, _defaultReconnect);
        }

        private void StartNegotiation()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("[HubConnection] [method:StartNegotiation] [msg] StartNegotiation");
#endif

            if (State == ConnectionStates.CloseInitiated)
            {
                SetState(ConnectionStates.Closed, null, _defaultReconnect);
                return;
            }

#if !BESTHTTP_DISABLE_WEBSOCKET
            if (Options.SkipNegotiation && Options.PreferTransport == TransportTypes.WebSocket)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log("[HubConnection] [method:StartNegotiation] [msg] 跳过谈判");
#endif
                ConnectImpl(Options.PreferTransport);

                return;
            }
#endif

            SetState(ConnectionStates.Negotiating, null, _defaultReconnect);

            // ReSharper disable once CommentTypo
            // https://github.com/dotnet/aspnetcore/blob/master/src/SignalR/docs/specs/TransportProtocols.md#post-endpoint-basenegotiate-request
            //发送协商请求。而我们可以跳过它，直接连接到websocket传输
            //它可能返回有用的额外信息。

            UriBuilder builder = new UriBuilder(Uri);
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
            request.Context.Add("Hub", Context);

            if (AuthenticationProvider != null)
            {
                AuthenticationProvider.PrepareRequest(request);
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
                    if (NegotiationResult != null && !IsTransportSupported("WebSockets"))
                    {
                        SetState(
                            state: ConnectionStates.Closed,
                            errorReason: "不能使用首选传输，因为服务器不支持“WebSockets”传输!",
                            allowReconnect: _defaultReconnect);
                        return;
                    }

                    Transport = new WebSocketTransport(this);
                    Transport.OnStateChanged += Transport_OnStateChanged;
                }
                    break;
#endif

                case TransportTypes.LongPolling:
                {
                    if (NegotiationResult != null && !IsTransportSupported("LongPolling"))
                    {
                        SetState(
                            state: ConnectionStates.Closed,
                            errorReason: "不能使用首选传输，因为服务器不支持'LongPolling'传输!",
                            allowReconnect: _defaultReconnect);
                        return;
                    }

                    Transport = new LongPollingTransport(this);
                    Transport.OnStateChanged += Transport_OnStateChanged;
                }
                    break;

                default:
                {
                    SetState(
                        state: ConnectionStates.Closed,
                        errorReason: $"Unsupported transport: {transport}",
                        allowReconnect: _defaultReconnect);
                }
                    break;
            }

            try
            {
                if (OnTransportEvent != null)
                {
                    OnTransportEvent(this, Transport, TransportEvents.SelectedToConnect);
                }
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.LogError(
                    $"[HubConnection] [method:ConnectImpl] [msg] 用户代码中的OnTransportEvent异常! Exception:{ex.Message}");
#endif
            }

            Transport.StartConnect();
        }

        private bool IsTransportSupported(string transportName)
        {
            // ReSharper disable once CommentTypo
            // https://github.com/dotnet/aspnetcore/blob/master/src/SignalR/docs/specs/TransportProtocols.md#post-endpoint-basenegotiate-request
            //如果协商响应只包含url和accessToken，则不发送' availableTransports '列表
            if (NegotiationResult.SupportedTransports == null)
            {
                return true;
            }

            bool IsAny(SupportedTransport transport)
            {
                var isEquals = transport.Name.Equals(transportName, StringComparison.OrdinalIgnoreCase);
                return isEquals;
            }

            return NegotiationResult.SupportedTransports.Any(IsAny);
        }

        private void OnNegotiationRequestFinished(HttpRequest req, HttpResponse resp)
        {
            switch (State)
            {
                case ConnectionStates.Closed:
                    return;
                case ConnectionStates.CloseInitiated:
                    SetState(ConnectionStates.Closed, null, _defaultReconnect);
                    return;
                case ConnectionStates.Initial:
                    break;
                case ConnectionStates.Authenticating:
                    break;
                case ConnectionStates.Negotiating:
                    break;
                case ConnectionStates.Redirected:
                    break;
                case ConnectionStates.Reconnecting:
                    break;
                case ConnectionStates.Connected:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            string errorReason = null;

            switch (req.State)
            {
                // 请求顺利完成。
                case HttpRequestStates.Finished:
                {
                    if (resp.IsSuccess)
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        var sb = new StringBuilder(3);
                        sb.Append("Negotiation Request");
                        sb.Append(" Finished Successfully!");
                        sb.Append($" Response: {resp.DataAsText}");
                        Debug.Log($"[HubConnection] [method:SetState] [msg] {sb}");
#endif

                        // Parse negotiation
                        NegotiationResult = NegotiationResult.Parse(resp, out errorReason, this);

                        //改进空间:检查谈判结果的有效性:
                        //如果url和accessToken存在，其他两个必须为空。
                        // ReSharper disable once CommentTypo
                        //  https://github.com/dotnet/aspnetcore/blob/master/src/SignalR/docs/specs/TransportProtocols.md#post-endpoint-basenegotiate-request

                        if (string.IsNullOrEmpty(errorReason))
                        {
                            if (NegotiationResult.Url != null)
                            {
                                SetState(ConnectionStates.Redirected, null, _defaultReconnect);

                                if (++RedirectCount >= Options.MaxRedirects)
                                {
                                    errorReason = $"MaxRedirects ({Options.MaxRedirects:N0}) reached!";
                                }
                                else
                                {
                                    var oldUri = Uri;
                                    Uri = NegotiationResult.Url;

                                    if (OnRedirected != null)
                                    {
                                        try
                                        {
                                            OnRedirected(this, oldUri, Uri);
                                        }
                                        catch (Exception ex)
                                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                            Debug.LogError(
                                                $"[HubConnection] [method:OnNegotiationRequestFinished] [msg|Exception] OnNegotiationRequestFinished - OnRedirected [Exception] {ex}");
#endif
                                        }
                                    }

                                    StartConnect();
                                }
                            }
                            else
                            {
                                ConnectImpl(Options.PreferTransport);
                            }
                        }
                    }
                    else // Internal server error?
                    {
                        errorReason =
                            $"协商请求完成成功，但是服务器发送了一个错误。状态码: {resp.StatusCode}-{resp.Message} Message: {resp.DataAsText}";
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
                NegotiationResult = new NegotiationResult
                {
                    NegotiationResponse = resp
                };

                SetState(ConnectionStates.Closed, errorReason, _defaultReconnect);
            }
        }

        private void StartClose()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log("[HubConnection] [method:StartClose] [msg] StartClose");
#endif
            _defaultReconnect = false;

            switch (State)
            {
                case ConnectionStates.Initial:
                {
                    SetState(ConnectionStates.Closed, null, _defaultReconnect);
                }
                    break;

                case ConnectionStates.Authenticating:
                {
                    AuthenticationProvider.OnAuthenticationSucceed -= OnAuthenticationSucceed;
                    AuthenticationProvider.OnAuthenticationFailed -= OnAuthenticationFailed;
                    AuthenticationProvider.Cancel();
                    SetState(
                        state: ConnectionStates.Closed,
                        errorReason: null,
                        allowReconnect: _defaultReconnect);
                }
                    break;

                case ConnectionStates.Reconnecting:
                {
                    SetState(
                        state: ConnectionStates.Closed,
                        errorReason: null,
                        allowReconnect: _defaultReconnect);
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
                        SetState(
                            state: ConnectionStates.Closed,
                            errorReason: null,
                            allowReconnect: _defaultReconnect);
                    }
                    else
                    {
                        SetState(
                            state: ConnectionStates.CloseInitiated,
                            errorReason: null,
                            allowReconnect: _defaultReconnect);

                        Transport?.StartClose();
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
                message =>
                {
                    bool isSuccess = string.IsNullOrEmpty(message.error);
                    if (isSuccess)
                    {
                        future.Assign((TResult)Protocol.ConvertTo(typeof(TResult), message.result));
                    }
                    else
                    {
                        future.Fail(new Exception(message.error));
                    }
                },
                typeof(TResult));

            if (id < 0)
            {
                future.Fail(new Exception($"未处于连接状态!当前状态: {State}"));
            }

            return future;
        }

        public IFuture<object> Send(string target, params object[] args)
        {
            Future<object> future = new Future<object>();

            long id = InvokeImp(target,
                args,
                message =>
                {
                    bool isSuccess = string.IsNullOrEmpty(message.error);
                    if (isSuccess)
                        future.Assign(message.item);
                    else
                        future.Fail(new Exception(message.error));
                },
                typeof(object));

            if (id < 0)
                future.Fail(new Exception($"未处于连接状态!当前状态: {State}"));

            return future;
        }

        private long InvokeImp(
            string target,
            object[] args,
            Action<Message> callback,
            Type itemType,
            bool isStreamingInvocation = false)
        {
            if (State != ConnectionStates.Connected)
                return -1;

            bool blockingInvocation = callback == null;

            long invocationId =
                blockingInvocation ? 0 : Interlocked.Increment(ref _lastInvocationId);
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
                if (!_invocations.TryAdd(invocationId,
                        new InvocationDefinition { Callback = callback, ReturnType = itemType }))
                {
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        StringBuilder sb = new StringBuilder();
                        sb.Append("[HubConnection] ");
                        sb.Append("[method:InvokeImp] ");
                        sb.Append("[msg|Exception] ");
                        sb.Append("InvokeImp - invocations already contains id: ");
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
                using (new WriteLock(_rwLock))
                {
                    var encoded = Protocol.EncodeMessage(message);
                    if (encoded.Data == null) return;
                    _lastMessageSentAt = DateTime.Now;
                    Transport.Send(encoded);
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
            long invocationId = Interlocked.Increment(ref _lastInvocationId);

            var future = new Future<TDown>();
            future.BeginProcess();

            var controller = new DownStreamItemController<TDown>(this, invocationId, future);

            void Callback(Message msg)
            {
                switch (msg.type)
                {
                    // StreamItem消息只包含一个项目。
                    case MessageTypes.StreamItem:
                    {
                        if (controller.IsCanceled) break;

                        TDown item = (TDown)Protocol.ConvertTo(typeof(TDown), msg.item);

                        future.AssignItem(item);
                        break;
                    }

                    case MessageTypes.Completion:
                    {
                        bool isSuccess = string.IsNullOrEmpty(msg.error);
                        if (isSuccess)
                        {
                            // 虽然完成消息必须不包含任何结果，但这应该是面向未来的
                            if (!controller.IsCanceled && msg.result != null)
                            {
                                TDown result = (TDown)Protocol.ConvertTo(typeof(TDown), msg.result);

                                future.AssignItem(result);
                            }

                            future.Finish();
                        }
                        else
                            future.Fail(new Exception(msg.error));

                        break;
                    }
                }
            }

            var message = new Message
            {
                type = MessageTypes.StreamInvocation,
                invocationId = invocationId.ToString(),
                target = target,
                arguments = args,
                nonblocking = false,
            };

            SendMessage(message);

            if (!_invocations.TryAdd(invocationId,
                    new InvocationDefinition { Callback = Callback, ReturnType = typeof(TDown) }))
            {
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    StringBuilder sb = new StringBuilder();
                    sb.Append("[HubConnection] ");
                    sb.Append("[method:GetDownStreamController] ");
                    sb.Append("[msg|Exception] ");
                    sb.Append("GetUpStreamController - invocations already contains id: ");
                    sb.Append($"{invocationId}");
                    Debug.Log(sb.ToString());
#endif
                }
            }

            return controller;
        }

        public
            UpStreamItemController<TResult> GetUpStreamController<TResult>(
                string target,
                int paramCount,
                bool downStream,
                object[] args)
        {
            Future<TResult> future = new Future<TResult>();
            future.BeginProcess();

            long invocationId = Interlocked.Increment(ref _lastInvocationId);

            string[] streamIds = new string[paramCount];
            for (int i = 0; i < paramCount; i++)
            {
                streamIds[i] = Interlocked.Increment(ref _lastStreamId).ToString();
            }

            var controller = new UpStreamItemController<TResult>(this, invocationId, streamIds, future);

            void Callback(Message msg)
            {
                switch (msg.type)
                {
                    //StreamItem消息只包含一个项目。
                    case MessageTypes.StreamItem:
                    {
                        if (controller.IsCanceled) break;

                        TResult item = (TResult)Protocol.ConvertTo(typeof(TResult), msg.item);

                        future.AssignItem(item);
                        break;
                    }

                    case MessageTypes.Completion:
                    {
                        bool isSuccess = string.IsNullOrEmpty(msg.error);
                        if (isSuccess)
                        {
                            // 虽然完成消息必须不包含任何结果，但这应该是面向未来的
                            if (!controller.IsCanceled && msg.result != null)
                            {
                                TResult result = (TResult)Protocol.ConvertTo(typeof(TResult), msg.result);

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
            }

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

            if (!_invocations.TryAdd(invocationId,
                    new InvocationDefinition { Callback = Callback, ReturnType = typeof(TResult) }))
            {
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    StringBuilder sb = new StringBuilder();
                    sb.Append("[HubConnection] ");
                    sb.Append("[method:SetState] ");
                    sb.Append("[msg|Exception] ");
                    sb.Append("GetUpStreamController - invocations already contains id: ");
                    sb.Append($"{invocationId}");
                    Debug.Log(sb.ToString());
#endif
                }
            }

            return controller;
        }

        public void On(string methodName, Action callback)
        {
            On(methodName, null, _ => callback());
        }

        public void On<T1>(string methodName, Action<T1> callback)
        {
            On(methodName, new[] { typeof(T1) }, args => callback((T1)args[0]));
        }

        public void On<T1, T2>(string methodName, Action<T1, T2> callback)
        {
            On(methodName,
                new[] { typeof(T1), typeof(T2) },
                args => callback((T1)args[0], (T2)args[1]));
        }

        public void On<T1, T2, T3>(string methodName, Action<T1, T2, T3> callback)
        {
            On(methodName,
                new[] { typeof(T1), typeof(T2), typeof(T3) },
                args => callback((T1)args[0], (T2)args[1], (T3)args[2]));
        }

        public void On<T1, T2, T3, T4>(string methodName, Action<T1, T2, T3, T4> callback)
        {
            On(methodName,
                new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) },
                args => callback((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]));
        }

        private void On(string methodName, Type[] paramTypes, Action<object[]> callback)
        {
            if (State >= ConnectionStates.CloseInitiated)
            {
                throw new Exception("集线器连接已经关闭或关闭!");
            }

            _subscriptions.GetOrAdd(methodName, _ => new Subscription())
                .Add(paramTypes, callback);
        }

        /// <summary>
        /// 删除的所有事件处理程序 <paramref name="methodName"/> 订阅了
        /// </summary>
        public void Remove(string methodName)
        {
            if (State >= ConnectionStates.CloseInitiated)
            {
                throw new Exception("集线器连接已经关闭或关闭!");
            }

            _subscriptions.TryRemove(methodName, out var _);
        }

        internal Subscription GetSubscription(string methodName)
        {
            // ReSharper disable once IdentifierTypo
            _subscriptions.TryGetValue(methodName, out var subscribtion);
            return subscribtion;
        }

        internal Type GetItemType(long invocationId)
        {
            _invocations.TryGetValue(invocationId, out var def);
            return def.ReturnType;
        }

        internal void OnMessages(List<Message> messages)
        {
            _lastMessageReceivedAt = DateTime.Now;

            if (_pausedInLastFrame)
            {
                _delayedMessages ??= new List<Message>(messages.Count);
                foreach (var msg in messages)
                    _delayedMessages.Add(msg);

                messages.Clear();
            }

            foreach (var message in messages)
            {
                if (OnMessage != null)
                {
                    try
                    {
                        if (!OnMessage(this, message))
                            continue;
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.LogError(
                            $"[HubConnection] [method:OnMessages] [msg|Exception] OnMessage用户代码中的异常![Exception] {ex}");
#endif
                    }
                }

                switch (message.type)
                {
                    case MessageTypes.Invocation:
                    {
                        if (_subscriptions.TryGetValue(message.target, out var subscription))
                        {
                            foreach (var callbackDesc in subscription.Callbacks)
                            {
                                object[] realArgs = null;
                                try
                                {
                                    realArgs = Protocol.GetRealArguments(callbackDesc.ParamTypes,
                                        message.arguments);
                                }
                                catch (Exception ex)
                                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                    Debug.LogError(
                                        $"[HubConnection] [method:OnMessages] [msg|Exception] OnMessages - Invocation - GetRealArguments[Exception] {ex}");
#endif
                                }

                                try
                                {
                                    callbackDesc.Callback.Invoke(realArgs);
                                }
                                catch (Exception ex)
                                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                    Debug.LogError(
                                        $"[HubConnection] [method:OnMessages] [msg|Exception] OnMessages - Invocation - Invoke [Exception] {ex}");
#endif
                                }
                            }
                        }

                        break;
                    }

                    case MessageTypes.StreamItem:
                    {
                        if (long.TryParse(message.invocationId, out var invocationId))
                        {
                            if (_invocations.TryGetValue(invocationId, out var def) && def.Callback != null)
                            {
                                try
                                {
                                    def.Callback(message);
                                }
                                catch (Exception ex)
                                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                    Debug.LogError(
                                        $"[HubConnection] [method:OnMessages] [msg|Exception] OnMessages - StreamItem - callback [Exception] {ex}");
#endif
                                }
                            }
                        }

                        break;
                    }

                    case MessageTypes.Completion:
                    {
                        if (long.TryParse(message.invocationId, out var invocationId))
                        {
                            if (_invocations.TryRemove(invocationId, out var def) && def.Callback != null)
                            {
                                try
                                {
                                    def.Callback(message);
                                }
                                catch (Exception ex)
                                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                    Debug.LogError(
                                        $"[HubConnection] [method:OnMessages] [msg|Exception] OnMessages - Completion - callback [Exception] {ex}");
#endif
                                }
                            }
                        }

                        break;
                    }

                    case MessageTypes.Ping:
                        // Send back an answer
                        SendMessage(new Message { type = MessageTypes.Ping });
                        break;

                    case MessageTypes.Close:
                        SetState(ConnectionStates.Closed, message.error, message.allowReconnect);
                        if (Transport != null)
                        {
                            Transport.StartClose();
                        }

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
            Debug.Log($"[HubConnection] [method:Transport_OnStateChanged] [msg] {sb}");
#endif
            if (State == ConnectionStates.Closed)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    "[HubConnection] [method:Transport_OnStateChanged] [msg] Transport_OnStateChanged - already closed!");
#endif
                return;
            }

            switch (newState)
            {
                case TransportStates.Connected:
                {
                    try
                    {
                        OnTransportEvent?.Invoke(this, Transport, TransportEvents.Connected);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.LogError(
                            $"[HubConnection] [method:Transport_OnStateChanged] [msg] Exception in OnTransportEvent user code! Exception:{ex.Message}");
#endif
                    }

                    SetState(ConnectionStates.Connected, null, _defaultReconnect);
                }
                    break;

                case TransportStates.Failed:
                {
                    if (State == ConnectionStates.Negotiating && !HttpManager.IsQuitting)
                    {
                        try
                        {
                            OnTransportEvent?.Invoke(this, Transport, TransportEvents.FailedToConnect);
                        }
                        catch (Exception ex)
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.LogError(
                                $"[HubConnection] [method:Transport_OnStateChanged] [msg] Exception in OnTransportEvent user code! Exception:{ex.Message}");
#endif
                        }

                        _triedoutTransports.Add(Transport.TransportType);

                        var nextTransport = GetNextTransportToTry();
                        if (nextTransport == null)
                        {
                            var reason = Transport.ErrorReason;
                            Transport = null;

                            SetState(ConnectionStates.Closed, reason, _defaultReconnect);
                        }
                        else
                            ConnectImpl(nextTransport.Value);
                    }
                    else
                    {
                        try
                        {
                            OnTransportEvent?.Invoke(this, Transport, TransportEvents.ClosedWithError);
                        }
                        catch (Exception ex)
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.LogError(
                                $"[HubConnection] [method:Transport_OnStateChanged] [msg] Exception in OnTransportEvent user code! Exception:{ex.Message}");
#endif
                        }

                        var reason = Transport.ErrorReason;
                        Transport = null;

                        SetState(ConnectionStates.Closed, HttpManager.IsQuitting ? null : reason,
                            _defaultReconnect);
                    }
                }
                    break;

                case TransportStates.Closed:
                {
                    try
                    {
                        OnTransportEvent?.Invoke(this, Transport, TransportEvents.Closed);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.LogError(
                            $"[HubConnection] [method:Transport_OnStateChanged] [msg] Exception in OnTransportEvent user code! Exception:{ex.Message}");
#endif
                    }

                    // 检查我们是否有延迟消息和关闭消息。如果有，也延迟SetState(Close)。
                    if (_delayedMessages == null ||
                        _delayedMessages.FindLast(dm => dm.type == MessageTypes.Close).type != MessageTypes.Close)
                    {
                        SetState(ConnectionStates.Closed, null, _defaultReconnect);
                    }
                }
                    break;
            }
        }

        private TransportTypes? GetNextTransportToTry()
        {
            foreach (TransportTypes val in Enum.GetValues(typeof(TransportTypes)))
                if (!_triedoutTransports.Contains(val) && IsTransportSupported(val.ToString()))
                    return val;

            return null;
        }

        private void SetState(ConnectionStates state, string errorReason, bool allowReconnect)
        {
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG

                StringBuilder sb = new StringBuilder(5);
                sb.Append($"SetState - from State: '{State}'");
                sb.Append($" to State: '{state}',");
                sb.Append($" errorReason: '{errorReason}', ");
                sb.Append($"allowReconnect: {allowReconnect}, ");
                sb.Append($"isQuitting: {HttpManager.IsQuitting}");
                Debug.Log(
                    $"[HubConnection] [method:SetState] [msg]{sb}");
#endif
            }
            if (State == state)
            {
                return;
            }

            var previousState = State;

            State = state;

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
                    if (_reconnectStartTime != DateTime.MinValue)
                    {
                        try
                        {
                            if (OnReconnected != null)
                                OnReconnected(this);
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
                            if (OnConnected != null)
                            {
                                OnConnected(this);
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

                    _lastMessageSentAt = DateTime.Now;
                    _lastMessageReceivedAt = DateTime.Now;

                    // 清理重新连接相关字段
                    _currentContext = new RetryContext();
                    _reconnectStartTime = DateTime.MinValue;
                    _reconnectAt = DateTime.MinValue;

                    HttpUpdateDelegator.OnApplicationForegroundStateChanged -=
                        OnApplicationForegroundStateChanged;
                    HttpUpdateDelegator.OnApplicationForegroundStateChanged +=
                        OnApplicationForegroundStateChanged;
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

                    foreach (var kvp in _invocations)
                    {
                        try
                        {
                            kvp.Value.Callback(error);
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    _invocations.Clear();

                    // No errorReason? It's an expected closure.
                    if (errorReason == null && (!allowReconnect || HttpManager.IsQuitting))
                    {
                        if (OnClosed != null)
                        {
                            try
                            {
                                OnClosed(this);
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
                        Debug.Log("[HubConnection] [method:SetState] [msg] Cleaning up");
#endif

                        _delayedMessages?.Clear();
                        HttpManager.Heartbeats.Unsubscribe(this);

                        _rwLock?.Dispose();
                        _rwLock = null;

                        HttpUpdateDelegator.OnApplicationForegroundStateChanged -=
                            OnApplicationForegroundStateChanged;
                    }
                    else
                    {
                        // If possible, try to reconnect
                        if (allowReconnect && ReconnectPolicy != null &&
                            (previousState == ConnectionStates.Connected ||
                             _reconnectStartTime != DateTime.MinValue))
                        {
                            // 这是成功连接后的第一次尝试
                            if (_reconnectStartTime == DateTime.MinValue)
                            {
                                var now = DateTimeUtil.GetCurrEntTimeMilliseconds(DateTimeUtil.Now());

                                _connectionStartedAt = _reconnectStartTime = now;

                                try
                                {
                                    if (OnReconnecting != null)
                                    {
                                        OnReconnecting(this, errorReason);
                                    }
                                }
                                catch (Exception ex)
                                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                    Debug.LogError(
                                        $"[HubConnection] [method:SetState] [msg|Exception] SetState - ConnectionStates.Reconnecting [Exception] {ex}");
#endif
                                }
                            }

                            RetryContext context = new RetryContext
                            {
                                ElapsedTime = DateTime.Now - _reconnectStartTime,
                                PreviousRetryCount = _currentContext.PreviousRetryCount,
                                RetryReason = errorReason
                            };

                            TimeSpan? nextAttempt = null;
                            try
                            {
                                nextAttempt = ReconnectPolicy.GetNextRetryDelay(context);
                            }
                            catch (Exception ex)
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                Debug.LogError(
                                    $"[HubConnection] [method:SetState] [msg|Exception] ReconnectPolicy.GetNextRetryDelay [Exception] {ex}");
#endif
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
                                    sb.Append("No more reconnect attempt!");
                                    Debug.Log(sb.ToString());
#endif
                                }

                                // Clean up everything
                                _currentContext = new RetryContext();
                                _reconnectStartTime = DateTime.MinValue;
                                _reconnectAt = DateTime.MinValue;
                            }
                            else
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                var sb = new StringBuilder(3);
                                sb.Append("Next reconnect attempt after");
                                sb.Append($" {nextAttempt.Value.ToString()}");
                                Debug.Log($"[HubConnection] [method:SetState] [msg] {sb}");
#endif
                                _currentContext = context;
                                _currentContext.PreviousRetryCount += 1;

                                _reconnectAt = DateTime.Now + nextAttempt.Value;

                                SetState(ConnectionStates.Reconnecting, null, _defaultReconnect);

                                return;
                            }
                        }

                        if (OnError != null)
                        {
                            try
                            {
                                OnError(this, errorReason);
                            }
                            catch (Exception ex)
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                Debug.LogError(
                                    $"[HubConnection] [method:SetState] [msg|Exception] Exception in OnError user code! [Exception] {ex}");
#endif
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
            _pausedInLastFrame = !isPaused;
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            var sb = new StringBuilder(3);
            sb.Append("OnApplicationForegroundStateChanged");
            sb.Append($" isPaused: {isPaused}");
            sb.Append($" pausedInLastFrame: {_pausedInLastFrame}");
            Debug.Log($"[HubConnection] [method:OnApplicationForegroundStateChanged] [msg] {sb}");
#endif
        }

#if CSHARP_7_OR_LATER

        TaskCompletionSource<HubConnection> _connectAsyncTaskCompletionSource;

        public Task<HubConnection> ConnectAsync()
        {
            if (State != ConnectionStates.Initial && State != ConnectionStates.Redirected &&
                State != ConnectionStates.Reconnecting)
                throw new Exception("HubConnection - ConnectAsync - Expected Initial or Redirected state, got " +
                                    State);

            if (_connectAsyncTaskCompletionSource != null)
                throw new Exception("Connect process already started!");

            _connectAsyncTaskCompletionSource = new TaskCompletionSource<HubConnection>();

            OnConnected += OnAsyncConnectedCallback;
            OnError += OnAsyncConnectFailedCallback;

            StartConnect();

            return _connectAsyncTaskCompletionSource.Task;
        }

        private void OnAsyncConnectedCallback(HubConnection hub)
        {
            OnConnected -= OnAsyncConnectedCallback;
            OnError -= OnAsyncConnectFailedCallback;

            _connectAsyncTaskCompletionSource.TrySetResult(this);
            _connectAsyncTaskCompletionSource = null;
        }

        private void OnAsyncConnectFailedCallback(HubConnection hub, string error)
        {
            OnConnected -= OnAsyncConnectedCallback;
            OnError -= OnAsyncConnectFailedCallback;

            _connectAsyncTaskCompletionSource.TrySetException(new Exception(error));
            _connectAsyncTaskCompletionSource = null;
        }

#endif

#if CSHARP_7_OR_LATER

        TaskCompletionSource<HubConnection> _closeAsyncTaskCompletionSource;

        public Task<HubConnection> CloseAsync()
        {
            if (_closeAsyncTaskCompletionSource != null)
                throw new Exception("CloseAsync already called!");

            _closeAsyncTaskCompletionSource = new TaskCompletionSource<HubConnection>();

            OnClosed += OnClosedAsyncCallback;
            OnError += OnClosedAsyncErrorCallback;

            //通过在StartClose之前缓存任务来避免竞争条件，
            //异步调用OnClosedAsyncCallback，该函数为空
            //这个。closeAsyncTaskCompletionSource
            //一个阅读的机会。
            var task = _closeAsyncTaskCompletionSource.Task;

            StartClose();

            return task;
        }

        void OnClosedAsyncCallback(HubConnection hub)
        {
            OnClosed -= OnClosedAsyncCallback;
            OnError -= OnClosedAsyncErrorCallback;

            _closeAsyncTaskCompletionSource.TrySetResult(this);
            _closeAsyncTaskCompletionSource = null;
        }

        void OnClosedAsyncErrorCallback(HubConnection hub, string error)
        {
            OnClosed -= OnClosedAsyncCallback;
            OnError -= OnClosedAsyncErrorCallback;

            _closeAsyncTaskCompletionSource.TrySetException(new Exception(error));
            _closeAsyncTaskCompletionSource = null;
        }

#endif

#if CSHARP_7_OR_LATER

        public Task<TResult> InvokeAsync<TResult>(string target, params object[] args)
        {
            return InvokeAsync<TResult>(target, default(CancellationToken), args);
        }

        private Task<TResult> InvokeAsync<TResult>(
            string target,
            CancellationToken cancellationToken = default,
            params object[] args)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            long id = InvokeImp(target,
                args,
                message =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        tcs.TrySetCanceled(cancellationToken);
                        return;
                    }

                    bool isSuccess = string.IsNullOrEmpty(message.error);
                    if (isSuccess)
                    {
                        tcs.TrySetResult((TResult)Protocol.ConvertTo(typeof(TResult), message.result));
                    }
                    else
                    {
                        tcs.TrySetException(new Exception(message.error));
                    }
                },
                typeof(TResult));

            if (id < 0)
            {
                tcs.TrySetException(new Exception($"未处于连接状态!当前状态:{State}"));
            }
            else
            {
                cancellationToken.Register(() => tcs.TrySetCanceled());
            }

            return tcs.Task;
        }

#endif

#if CSHARP_7_OR_LATER

        public Task<object> SendAsync(string target, params object[] args)
        {
            return SendAsync(target, default(CancellationToken), args);
        }

        private Task<object> SendAsync(string target, CancellationToken cancellationToken = default,
            params object[] args)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            long id = InvokeImp(target,
                args,
                message =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        tcs.TrySetCanceled(cancellationToken);
                        return;
                    }

                    bool isSuccess = string.IsNullOrEmpty(message.error);
                    if (isSuccess)
                    {
                        tcs.TrySetResult(message.item);
                    }
                    else
                    {
                        tcs.TrySetException(new Exception(message.error));
                    }
                },
                typeof(object));

            if (id < 0)
            {
                tcs.TrySetException(new Exception($"未处于连接状态!当前状态:{State}"));
            }
            else
            {
                cancellationToken.Register(() => tcs.TrySetCanceled());
            }

            return tcs.Task;
        }

#endif
    }
}

#endif
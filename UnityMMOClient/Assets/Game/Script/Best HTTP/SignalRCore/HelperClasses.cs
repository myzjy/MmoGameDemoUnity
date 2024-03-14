#if !BESTHTTP_DISABLE_SIGNALR_CORE
using System;
using System.Collections.Generic;
using BestHTTP.PlatformSupport.Memory;

namespace BestHTTP.SignalRCore
{
    public enum TransportTypes
    {
#if !BESTHTTP_DISABLE_WEBSOCKET
        WebSocket,
#endif
        LongPolling
    }

    public enum TransferModes
    {
        Binary,
        Text
    }

    public enum TransportStates
    {
        Initial,
        Connecting,
        Connected,
        Closing,
        Failed,
        Closed
    }

    /// <summary>
    /// HubConnection的可能状态
    /// </summary>
    public enum ConnectionStates
    {
        Initial,
        Authenticating,
        Negotiating,
        Redirected,
        Reconnecting,
        Connected,
        CloseInitiated,
        Closed
    }

    /// <summary>
    /// 说明从“外面”看，Transport可以走槽。
    /// </summary>
    public enum TransportEvents
    {
        /// <summary>
        /// 选择传输以尝试连接到服务器
        /// </summary>
        SelectedToConnect,

        /// <summary>
        ///传输连接到服务器失败。此事件可以发生在SelectedToConnect之后，当已经连接并且发生错误时，它将是一个ClosedWithError。
        /// </summary>
        FailedToConnect,

        /// <summary>
        /// 传输成功连接到服务器。
        /// </summary>
        Connected,

        /// <summary>
        /// 传输优雅地结束。
        /// </summary>
        Closed,

        /// <summary>
        /// 发生意外错误，传输无法恢复。
        /// </summary>
        ClosedWithError
    }

    public interface ITransport
    {
        TransferModes TransferMode { get; }
        TransportTypes TransportType { get; }
        TransportStates State { get; }

        string ErrorReason { get; }

        event Action<TransportStates, TransportStates> OnStateChanged;

        void StartConnect();
        void StartClose();

        void Send(BufferSegment bufferSegment);
    }

    public interface IEncoder
    {
        BufferSegment Encode<T>(T value);

        T DecodeAs<T>(BufferSegment buffer);

        object ConvertTo(Type toType, object obj);
    }

    public abstract class StreamItemContainer<T>
    {
        public readonly long ID;

        // ReSharper disable once CollectionNeverQueried.Local
        private List<T> Items { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public T LastAdded { get; private set; }

        public bool IsCanceled;

        protected StreamItemContainer(long id)
        {
            this.ID = id;
            this.Items = new List<T>();
        }

        public void AddItem(T item)
        {
            this.Items ??= new List<T>();

            this.Items.Add(item);
            this.LastAdded = item;
        }
    }

    struct CallbackDescriptor
    {
        public readonly Type[] ParamTypes;
        public readonly Action<object[]> Callback;

        public CallbackDescriptor(Type[] paramTypes, Action<object[]> callback)
        {
            this.ParamTypes = paramTypes;
            this.Callback = callback;
        }
    }

    internal struct InvocationDefinition
    {
        public Action<Messages.Message> Callback;
        public Type ReturnType;
    }

    internal sealed class Subscription
    {
        public readonly List<CallbackDescriptor> Callbacks = new List<CallbackDescriptor>(1);

        public void Add(Type[] paramTypes, Action<object[]> callback)
        {
            this.Callbacks.Add(new CallbackDescriptor(paramTypes, callback));
        }

        public void Remove(Action<object[]> callback)
        {
            int idx = -1;
            for (int i = 0; i < this.Callbacks.Count && idx == -1; ++i)
            {
                if (this.Callbacks[i].Callback == callback)
                {
                    idx = i;
                }
            }

            if (idx != -1)
                this.Callbacks.RemoveAt(idx);
        }
    }

    public sealed class HubOptions
    {
        /// <summary>
        ///当这个设置为true时，如果PreferTransport是WebSocket，插件将跳过协商请求。默认值为false。
        /// </summary>
        public bool SkipNegotiation { get; set; }

        /// <summary>
        /// 当有多个可用的传输方式时，选择首选的传输方式。它的默认值是TransportTypes.WebSocket。
        /// </summary>
        public TransportTypes PreferTransport { get; set; }

        /// <summary>
        /// ping消息只有在间隔时间过去后没有发送消息时才会发送。缺省值为15秒。
        /// </summary>
        public TimeSpan PingInterval { get; set; }

        /// <summary>
        /// 如果客户端在此间隔内没有看到任何消息，则认为连接已断开。缺省值为30秒。
        /// </summary>
        public TimeSpan PingTimeoutInterval { get; set; }

        /// <summary>
        /// 插件将遵循的重定向协商结果的最大计数。缺省值为100。
        /// </summary>
        public int MaxRedirects { get; set; }

        /// <summary>
        /// 插件允许花在尝试连接上的最大时间。默认值为1分钟。
        /// </summary>
        public TimeSpan ConnectTimeout { get; set; }

        public HubOptions()
        {
            this.SkipNegotiation = false;
#if !BESTHTTP_DISABLE_WEBSOCKET
            this.PreferTransport = TransportTypes.WebSocket;
#else
            this.PreferedTransport = TransportTypes.LongPolling;
#endif
            this.PingInterval = TimeSpan.FromSeconds(15);
            this.PingTimeoutInterval = TimeSpan.FromSeconds(30);
            this.MaxRedirects = 100;
            this.ConnectTimeout = TimeSpan.FromSeconds(60);
        }
    }

    public interface IRetryPolicy
    {
        /// <summary>
        ///此函数必须返回一个延迟时间以等待新的连接尝试，或者返回null以不执行另一个连接尝试。
        /// </summary>
        TimeSpan? GetNextRetryDelay(RetryContext context);
    }

    public struct RetryContext
    {
        /// <summary>
        /// 以前的重新连接尝试。一个成功的连接会将其置零。
        /// </summary>
        public uint PreviousRetryCount;

        /// <summary>
        /// 从最初的连接错误开始经过的时间。
        /// </summary>
        public TimeSpan ElapsedTime;

        /// <summary>
        /// 连接错误的字符串表示形式。
        /// </summary>
        public string RetryReason;
    }

    public sealed class DefaultRetryPolicy : IRetryPolicy
    {
        private static readonly TimeSpan?[] DefaultBackoffTimes = new TimeSpan?[]
        {
            TimeSpan.Zero,
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(30),
            null
        };

        private readonly TimeSpan?[] _backoffTimes;

        public DefaultRetryPolicy()
        {
            this._backoffTimes = DefaultBackoffTimes;
        }

        public DefaultRetryPolicy(TimeSpan?[] customBackoffTimes)
        {
            this._backoffTimes = customBackoffTimes;
        }

        public TimeSpan? GetNextRetryDelay(RetryContext context)
        {
            return context.PreviousRetryCount >= this._backoffTimes.Length
                ? null
                : this._backoffTimes[context.PreviousRetryCount];
        }
    }
}
#endif
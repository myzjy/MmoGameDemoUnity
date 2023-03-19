#if !BESTHttp_DISABLE_SOCKETIO
using System;
using System.Collections.Generic;

namespace BestHTTP.SocketIO3.Events
{
    [PlatformSupport.IL2CPP.Preserve]
    public abstract class ConnectResponse
    {
        // ReSharper disable once UnassignedField.Global
        [PlatformSupport.IL2CPP.Preserve] public string Sid;
    }

    public struct CallbackDescriptor
    {
        public readonly Type[] ParamTypes;
        public readonly Action<object[]> Callback;
        public readonly bool Once;

        public CallbackDescriptor(Type[] paramTypes, Action<object[]> callback, bool once)
        {
            this.ParamTypes = paramTypes;
            this.Callback = callback;
            this.Once = once;
        }
    }

    public sealed class Subscription
    {
        public readonly List<CallbackDescriptor> Callbacks = new List<CallbackDescriptor>(1);

        public void Add(Type[] paramTypes, Action<object[]> callback, bool once)
        {
            this.Callbacks.Add(new CallbackDescriptor(paramTypes, callback, once));
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
            {
                this.Callbacks.RemoveAt(idx);
            }
        }
    }

    public sealed class TypedEventTable
    {
        /// <summary>
        /// 事件表绑定到的套接字。
        /// </summary>
        private Socket Socket { get; set; }

        /// <summary>
        /// 这是我们存储methodName =>回调函数的映射。
        /// </summary>
        private readonly Dictionary<string, Subscription> _subscriptions =
            new Dictionary<string, Subscription>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 构造函数来创建实例并将其绑定到套接字。
        /// </summary>
        public TypedEventTable(Socket socket)
        {
            this.Socket = socket;
        }

        public Subscription GetSubscription(string name)
        {
            this._subscriptions.TryGetValue(name, out var subscription);
            return subscription;
        }

        public void Register(
            string methodName,
            Type[] paramTypes,
            Action<object[]> callback,
            bool once = false)
        {
            if (!this._subscriptions.TryGetValue(methodName, out var subscription))
            {
                this._subscriptions.Add(methodName, subscription = new Subscription());
            }

            subscription.Add(paramTypes, callback, once);
        }

        public void Call(string eventName, object[] args)
        {
            if (this._subscriptions.TryGetValue(eventName, out var subscription))
            {
                for (int i = 0; i < subscription.Callbacks.Count; ++i)
                {
                    var callbackDesc = subscription.Callbacks[i];

                    try
                    {
                        callbackDesc.Callback.Invoke(args);
                    }
                    catch (Exception ex)
                    {
                        HttpManager.Logger.Exception("TypedEventTable",
                            $"Call('{eventName}', {args?.Length ?? 0}) - Callback.Invoke", ex,
                            this.Socket.Context);
                    }

                    if (callbackDesc.Once)
                    {
                        subscription.Callbacks.RemoveAt(i--);
                    }
                }
            }
        }

        public void Call(IncomingPacket packet)
        {
            if (packet.Equals(IncomingPacket.Empty))
                return;

            string name = packet.EventName;
            object[] args = packet.DecodedArg != null ? new[] { packet.DecodedArg } : packet.DecodedArgs;

            Call(name, args);
        }

        public void Unregister(string name)
        {
            this._subscriptions.Remove(name);
        }

        public void Clear()
        {
            this._subscriptions.Clear();
        }
    }
}
#endif
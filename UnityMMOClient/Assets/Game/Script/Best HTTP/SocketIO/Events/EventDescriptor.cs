#if !BESTHTTP_DISABLE_SOCKETIO

using System;
using System.Collections.Generic;

namespace BestHTTP.SocketIO.Events
{
    public
        delegate
        void SocketIOCallback(Socket socket, Packet packet, params object[] args);

    public
        delegate
        void SocketIOAckCallback(Socket socket, Packet packet, params object[] args);

    /// <summary>
    /// 描述事件及其元数据的类。
    /// </summary>
    internal sealed class EventDescriptor
    {
        /// <summary>
        /// 回调委托的列表。
        /// </summary>
        public List<SocketIOCallback> Callbacks { get; private set; }

        /// <summary>
        /// 如果此属性为true，回调将在事件分派后自动移除。
        /// </summary>
        public bool OnlyOnce { get; private set; }

        /// <summary>
        /// 如果此属性为true，调度包的Payload将使用Manager's Encoder进行解码。
        /// </summary>
        public bool AutoDecodePayload { get; private set; }

        /// <summary>
        /// 在热路径上缓存一个数组。
        /// </summary>
        private SocketIOCallback[] _callbackArray;

        /// <summary>
        /// 构造函数来创建EventDescriptor实例并设置元数据。
        /// </summary>
        public EventDescriptor(bool onlyOnce, bool autoDecodePayload, SocketIOCallback callback)
        {
            this.OnlyOnce = onlyOnce;
            this.AutoDecodePayload = autoDecodePayload;
            this.Callbacks = new List<SocketIOCallback>(1);

            if (callback != null)
            {
                Callbacks.Add(callback);
            }
        }

        /// <summary>
        /// 如果此描述符标记为true OnlyOnce属性，将使用给定参数调用回调委托并删除回调。
        /// </summary>
        public void Call(Socket socket, Packet packet, params object[] args)
        {
            int callbackCount = Callbacks.Count;
            if (_callbackArray == null || _callbackArray.Length < callbackCount)
            {
                Array.Resize(ref _callbackArray, callbackCount);
            }

            //将回调委托复制到一个数组，因为在其中一个回调中我们可以修改列表(通过在事件处理程序中调用On/Once/Off)
            //这样我们可以防止一些奇怪的 bug
            Callbacks.CopyTo(_callbackArray);

            // Go through the delegates and call them
            //检查一下代表，给他们进行回调
            for (int i = 0; i < callbackCount; ++i)
            {
                try
                {
                    // 调用委托。
                    SocketIOCallback callback = _callbackArray[i];
                    callback?.Invoke(socket, packet, args);
                }
                catch (Exception ex)
                {
                    // 当我们已经尝试传递一个错误时，不要尝试发出一个新的错误，可能会导致堆栈溢出
                    if (args == null || args.Length == 0 || !(args[0] is Error))
                    {
                        (socket as ISocket).EmitError(SocketIOErrors.User, ex.Message + " " + ex.StackTrace);
                    }

                    HttpManager.Logger.Exception("EventDescriptor", "Call", ex);
                }

                // 如果这些回调函数只能被调用一次，请将它们从主列表中移除
                if (this.OnlyOnce)
                    Callbacks.Remove(_callbackArray[i]);

                // 不要保留任何引用以避免内存泄漏
                _callbackArray[i] = null;
            }
        }
    }
}

#endif
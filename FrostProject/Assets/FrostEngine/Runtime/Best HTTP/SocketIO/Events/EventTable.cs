#if !BESTHTTP_DISABLE_SOCKETIO

using System.Collections.Generic;
using System.Linq;

namespace BestHTTP.SocketIO.Events
{
    /// <summary>
    /// 这个类有助于跟踪和维护EventDescriptor实例，并将数据包分派给正确的委托。
    /// </summary>
    internal sealed class EventTable
    {
        /// <summary>
        /// 事件表绑定到的套接字。
        /// </summary>
        private Socket Socket { get; set; }

        /// <summary>
        /// EventName ->事件映射列表。
        /// </summary>
        private Dictionary<string, List<EventDescriptor>> Table = new Dictionary<string, List<EventDescriptor>>();

        /// <summary>
        /// 构造函数来创建实例并将其绑定到套接字。
        /// </summary>
        public EventTable(Socket socket)
        {
            this.Socket = socket;
        }

        /// <summary>
        /// 用给定的元数据注册对名称的回调。
        /// </summary>
        public void Register(
            string eventName,
            SocketIOCallback callback,
            bool onlyOnce,
            bool autoDecodePayload)
        {
            if (!Table.TryGetValue(eventName, out var events))
            {
                Table.Add(eventName, events = new List<EventDescriptor>(1));
            }

            bool FindEventDescriptor(EventDescriptor descriptor)
            {
                var isOnlyOnce = descriptor.OnlyOnce == onlyOnce;
                var isAutoDecodePayload = descriptor.AutoDecodePayload == autoDecodePayload;
                var isEventDescriptor = isOnlyOnce && isAutoDecodePayload;
                return isEventDescriptor;
            }


            // 查找匹配的描述符
            var desc = events.Find(FindEventDescriptor);

            // 如果没有找到，就创建一个
            if (desc == null)
            {
                var newEvent = new EventDescriptor(
                    onlyOnce: onlyOnce,
                    autoDecodePayload: autoDecodePayload,
                    callback: callback);
                events.Add(newEvent);
            }
            else
            {
                // 如果找到，添加新的回调
                desc.Callbacks.Add(callback);
            }
        }

        /// <summary>
        /// 删除为给定名称注册的所有事件。
        /// </summary>
        public void Unregister(string eventName)
        {
            Table.Remove(eventName);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Unregister(string eventName, SocketIOCallback callback)
        {
            if (!Table.TryGetValue(eventName, out var events)) return;
            foreach (var t in events)
            {
                t.Callbacks.Remove(callback);
            }
        }

        /// <summary>
        /// 将调用与给定eventName关联的委托。
        /// </summary>
        public void Call(string eventName, Packet packet, params object[] args)
        {
            if (Table.TryGetValue(eventName, out var events))
            {
                if (HttpManager.Logger.Level <= BestHTTP.Logger.Loglevels.All)
                {
                    HttpManager.Logger.Verbose("EventTable",
                        $"Call - {eventName} ({events.Count})");
                }

                foreach (var t in events)
                {
                    t.Call(Socket, packet, args);
                }
            }
            else
            {
                if (HttpManager.Logger.Level <= BestHTTP.Logger.Loglevels.All)
                {
                    HttpManager.Logger.Verbose("EventTable", $"Call - {eventName} (0)");
                }
            }
        }

        /// <summary>
        /// 这个函数将从数据包的Payload中获取eventName，并可选地从Json中解码它。
        /// </summary>
        public void Call(Packet packet)
        {
            string eventName = packet.DecodeEventName();
            string typeName = packet.SocketIOEvent != SocketIOEventTypes.Unknown
                ? EventNames.GetNameFor(packet.SocketIOEvent)
                : EventNames.GetNameFor(packet.TransportEvent);
            object[] args = null;

            if (!HasSubsciber(eventName) && !HasSubsciber(typeName))
                return;

            // 如果这是一个事件或BinaryEvent消息，或者我们有一个AutoDecodePayload订阅者，那么我们必须解码数据包的Payload。
            if (packet.TransportEvent == TransportEventTypes.Message &&
                packet.SocketIOEvent is SocketIOEventTypes.Event or SocketIOEventTypes.BinaryEvent &&
                ShouldDecodePayload(eventName))
            {
                args = packet.Decode(Socket.Manager.Encoder);
            }

            // 为'eventName'注册的调用事件回调
            if (!string.IsNullOrEmpty(eventName))
            {
                Call(eventName, packet, args);
            }

            if (!packet.IsDecoded && ShouldDecodePayload(typeName))
            {
                args = packet.Decode(Socket.Manager.Encoder);
            }

            // 为'typeName'注册的调用事件回调
            if (!string.IsNullOrEmpty(typeName))
            {
                Call(typeName, packet, args);
            }
        }

        /// <summary>
        /// 删除所有事件->代表协会。
        /// </summary>
        public void Clear()
        {
            Table.Clear();
        }

        /// <summary>
        /// 如果给定的事件名至少有一个事件需要解码，则返回true
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        private bool ShouldDecodePayload(string eventName)
        {
            bool AnyEventDescriptor(EventDescriptor descriptor)
            {
                var count = descriptor.Callbacks.Count;
                var isAny = descriptor.AutoDecodePayload
                            && count > 0;
                return isAny;
            }

            var isEventValue = Table.TryGetValue(eventName, out var events);
            // 如果我们找到至少一个带有AutoDecodePayload == true的EventDescriptor，我们必须解码整个有效负载
            return isEventValue && events.Any(AnyEventDescriptor);
        }

        // ReSharper disable once IdentifierTypo
        private bool HasSubsciber(string eventName)
        {
            return Table.ContainsKey(eventName);
        }
    }
}

#endif
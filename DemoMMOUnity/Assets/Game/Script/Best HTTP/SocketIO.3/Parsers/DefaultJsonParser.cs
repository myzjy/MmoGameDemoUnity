#if !BESTHTTP_DISABLE_SOCKETIO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BestHTTP.PlatformSupport.Memory;
using BestHTTP.SocketIO3.Events;

namespace BestHTTP.SocketIO3.Parsers
{
    public sealed class Placeholder
    {
        // ReSharper disable once InconsistentNaming
        public bool _placeholder;
        public int Num;
    }

    [PlatformSupport.IL2CPP.Il2CppEagerStaticClassConstructionAttribute]
    public sealed class DefaultJsonParser : IParser
    {
        static DefaultJsonParser()
        {
            JSON.LitJson.JsonMapper.RegisterImporter<string, byte[]>(Convert.FromBase64String);
        }

        private IncomingPacket _packetWithAttachment = IncomingPacket.Empty;

        private int ToInt(char ch)
        {
            int charValue = Convert.ToInt32(ch);
            int num = charValue - '0';
            if (num < 0 || num > 9)
                return -1;

            return num;
        }

        public IncomingPacket Parse(SocketManager manager, string from)
        {
            int idx = 0;
            var transportEvent = (TransportEventTypes)ToInt(from[idx++]);
            SocketIOEventTypes socketIOEvent;
            string nsp;
            var id = -1;
            int attachments = 0;

            if (from.Length > idx && ToInt(from[idx]) >= 0)
                socketIOEvent = (SocketIOEventTypes)ToInt(from[idx++]);
            else
                socketIOEvent = SocketIOEventTypes.Unknown;

            // Parse Attachment
            if (socketIOEvent == SocketIOEventTypes.BinaryEvent || socketIOEvent == SocketIOEventTypes.BinaryAck)
            {
                int endIdx = from.IndexOf('-', idx);
                if (endIdx == -1)
                    endIdx = from.Length;

                int.TryParse(from.Substring(idx, endIdx - idx), out attachments);

                idx = endIdx + 1;
            }

            // Parse Namespace
            if (from.Length > idx && from[idx] == '/')
            {
                int endIdx = from.IndexOf(',', idx);
                if (endIdx == -1)
                    endIdx = from.Length;

                nsp = from.Substring(idx, endIdx - idx);
                idx = endIdx + 1;
            }
            else
                nsp = "/";

            // Parse Id
            if (from.Length > idx && ToInt(from[idx]) >= 0)
            {
                int startIdx = idx++;
                while (from.Length > idx && ToInt(from[idx]) >= 0)
                    idx++;

                int.TryParse(from.Substring(startIdx, idx - startIdx), out id);
            }

            // What left is the payload data
            var payload = from.Length > idx ? from.Substring(idx) : string.Empty;

            var packet = new IncomingPacket(transportEvent, socketIOEvent, nsp, id)
            {
                AttachementCount = attachments
            };

            string eventName = packet.EventName;
            object[] args = null;

            switch (socketIOEvent)
            {
                case SocketIOEventTypes.Unknown:
                    packet.DecodedArg = payload;
                    break;

                case SocketIOEventTypes.Connect:
                    // No Data | Object
                    if (!string.IsNullOrEmpty(payload))
                        (eventName, args) = ReadData(manager, packet, payload);
                    break;

                case SocketIOEventTypes.Disconnect:
                    // No Data
                    break;

                case SocketIOEventTypes.Error:
                    // String | Object
                    (eventName, args) = ReadData(manager, packet, payload);
                    break;

                default:
                    // Array
                    (eventName, args) = ReadData(manager, packet, payload);
                    // Save payload until all attachments arrive
                    if (packet.AttachementCount > 0)
                        packet.DecodedArg = payload;
                    break;
            }

            packet.EventName = eventName;

            if (args != null)
            {
                if (args.Length == 1)
                    packet.DecodedArg = args[0];
                else
                    packet.DecodedArgs = args;
            }

            if (packet.AttachementCount > 0)
            {
                _packetWithAttachment = packet;
                return IncomingPacket.Empty;
            }

            return packet;
        }

        // ReSharper disable once IdentifierTypo
        public IncomingPacket MergeAttachements(SocketManager manager, IncomingPacket packet)
        {
            string payload = packet.DecodedArg as string;
            packet.DecodedArg = null;

            string placeholderFormat = "{{\"_placeholder\":true,\"num\":{0}}}";

            for (int i = 0; i < packet.Attachements.Count; ++i)
            {
                string placeholder = string.Format(placeholderFormat, i);
                BufferSegment data = packet.Attachements[i];

                if (payload != null)
                {
                    var baseString = Convert.ToBase64String(
                        inArray: data.Data,
                        offset: data.Offset,
                        length: data.Count);
                    var newString = $"\"{baseString}\"";
                    payload = payload.Replace(
                        oldValue: placeholder,
                        newValue: newString);
                }
            }

            var (eventName, args) = ReadData(manager, packet, payload);

            packet.EventName = eventName;

            if (args == null) return packet;
            if (args.Length == 1)
            {
                packet.DecodedArg = args[0];
            }
            else
            {
                packet.DecodedArgs = args;
            }

            return packet;
        }

        private (string, object[]) ReadData(
            SocketManager manager,
            IncomingPacket packet,
            string payload)
        {
            Socket socket = manager.GetSocket(packet.Namespace);

            string eventName = packet.EventName;
            Subscription subscription = socket.GetSubscription(eventName);

            object[] args = null;

            switch (packet.SocketIOEvent)
            {
                case SocketIOEventTypes.Unknown:
                    // TODO: Error?
                    break;

                case SocketIOEventTypes.Connect:
                    // No Data | Object
                    using (var strReader = new System.IO.StringReader(payload))
                        args = ReadParameters(socket, subscription, strReader);
                    break;

                case SocketIOEventTypes.Disconnect:
                    // No Data
                    break;

                case SocketIOEventTypes.Error:
                {
                    // String | Object
                    switch (payload[0])
                    {
                        case '{':
                            using (var strReader = new System.IO.StringReader(payload))
                            {
                                args = ReadParameters(socket, subscription, strReader);
                            }

                            break;

                        default:
                            args = new object[] { new Error(payload) };
                            break;
                    }
                }

                    break;

                case SocketIOEventTypes.Ack:
                {
                    eventName = IncomingPacket.GenerateAcknowledgementNameFromId(packet.Id);
                    subscription = socket.GetSubscription(eventName);
                    var jsonMap = JSON.LitJson.JsonMapper.ToObject<List<object>>(payload);
                    args = ReadParameters(
                        socket: socket,
                        subscription: subscription,
                        array: jsonMap,
                        startIdx: 0);
                }

                    break;

                default:
                {
                    // Array

                    List<object> array;
                    using (var reader = new System.IO.StringReader(payload))
                    {
                        var jsonReader = new JSON.LitJson.JsonReader(reader);
                        array = JSON.LitJson.JsonMapper.ToObject<List<object>>(jsonReader);
                    }

                    if (array.Count > 0)
                    {
                        eventName = array[0].ToString();
                        subscription = socket.GetSubscription(eventName);
                    }

                    if (packet.AttachementCount == 0 || packet.Attachements != null)
                    {
                        try
                        {
                            args = ReadParameters(
                                socket: socket,
                                subscription: subscription,
                                array: array,
                                startIdx: 1);
                        }
                        catch (Exception ex)
                        {
                            HttpManager.Logger.Exception("DefaultJsonParser",
                                $"带eventName的ReadParameters: {eventName}", ex);
                        }
                    }
                }

                    break;
            }

            return (eventName, args);
        }

        private object[] ReadParameters(Socket socket, Subscription subscription, List<object> array, int startIdx)
        {
            if (array.Count <= startIdx) return null;
            var desc = subscription != null ? subscription.Callbacks.FirstOrDefault() : default(CallbackDescriptor);
            int paramCount = desc.ParamTypes?.Length ?? 0;

            int arrayIdx = startIdx;
            if (paramCount <= 0) return null;
            var args = new object[paramCount];

            if (desc.ParamTypes == null) return args;
            for (var i = 0; i < desc.ParamTypes.Length; ++i)
            {
                var type = desc.ParamTypes[i];

                if (type == typeof(Socket))
                {
                    args[i] = socket;
                }
                else if (type == typeof(SocketManager))
                {
                    args[i] = socket.Manager;
                }
                else if (type == typeof(Placeholder))
                {
                    args[i] = new Placeholder();
                }
                else
                {
                    args[i] = ConvertTo(desc.ParamTypes[i], array[arrayIdx++]);
                }
            }

            return args;
        }

        private object ConvertTo(Type toType, object obj)
        {
            if (obj == null)
                return null;

#if NETFX_CORE
            TypeInfo objType = obj.GetType().GetTypeInfo();
#else
            Type objType = obj.GetType();
#endif

#if NETFX_CORE
            TypeInfo typeInfo = toType.GetTypeInfo();
#endif

#if NETFX_CORE
            if (typeInfo.IsEnum)
#else
            if (toType.IsEnum)
#endif
                return Enum.Parse(toType, obj.ToString(), true);

#if NETFX_CORE
            if (typeInfo.IsPrimitive)
#else
            if (toType.IsPrimitive)
#endif
                return Convert.ChangeType(obj, toType);

            if (toType == typeof(string))
                return obj.ToString();

#if NETFX_CORE
            if (typeInfo.IsGenericType && toType.Name == "Nullable`1")
                return Convert.ChangeType(obj, toType.GenericTypeArguments[0]);
#else
            if (toType.IsGenericType && toType.Name == "Nullable`1")
                return Convert.ChangeType(obj, toType.GetGenericArguments()[0]);
#endif

#if NETFX_CORE
            if (objType.Equals(typeInfo))
#else
            if (objType == toType)
#endif
                return obj;

            if (toType == typeof(byte[]) && objType == typeof(string))
                return Convert.FromBase64String(obj.ToString());

            return JSON.LitJson.JsonMapper.ToObject(toType, JSON.LitJson.JsonMapper.ToJson(obj));
        }

        private object[] ReadParameters(Socket socket, Subscription subscription, System.IO.TextReader reader)
        {
            var desc = subscription != null ? subscription.Callbacks.FirstOrDefault() : default(CallbackDescriptor);
            int paramCount = desc.ParamTypes?.Length ?? 0;
            object[] args = null;

            if (paramCount > 0)
            {
                args = new object[paramCount];

                if (desc.ParamTypes != null)
                {
                    for (var i = 0; i < desc.ParamTypes.Length; ++i)
                    {
                        Type type = desc.ParamTypes[i];

                        if (type == typeof(Socket))
                        {
                            args[i] = socket;
                        }
                        else if (type == typeof(SocketManager))
                        {
                            args[i] = socket.Manager;
                        }
                        else
                        {
                            var jr = new JSON.LitJson.JsonReader(reader);
                            args[i] = JSON.LitJson.JsonMapper.ToObject(desc.ParamTypes[i], jr);
                            reader.Read();
                        }
                    }
                }
            }

            return args;
        }

        public IncomingPacket Parse(SocketManager manager, BufferSegment data,
            TransportEventTypes transportEvent = TransportEventTypes.Unknown)
        {
            IncomingPacket packet = IncomingPacket.Empty;

            _packetWithAttachment.Attachements ??= new List<BufferSegment>(_packetWithAttachment.AttachementCount);
            _packetWithAttachment.Attachements.Add(data);

            if (_packetWithAttachment.Attachements.Count != _packetWithAttachment.AttachementCount) return packet;
            packet = manager.Parser.MergeAttachements(manager, _packetWithAttachment);
            _packetWithAttachment = IncomingPacket.Empty;

            return packet;
        }

        public OutgoingPacket CreateOutgoing(TransportEventTypes transportEvent, string payload)
        {
            return new OutgoingPacket { Payload = "" + (char)('0' + (byte)transportEvent) + payload };
        }

        private readonly StringBuilder _builder = new StringBuilder();

        public OutgoingPacket CreateOutgoing(
            Socket socket,
            SocketIOEventTypes socketIOEvent,
            int id,
            string name,
            object arg)
        {
            return CreateOutgoing(
                socket: socket,
                socketIOEvent: socketIOEvent,
                id: id,
                name: name,
                arg: arg != null ? new[] { arg } : null);
        }

        private static int GetBinaryCount(IReadOnlyCollection<object> args)
        {
            if (args == null || args.Count == 0)
                return 0;

            return args.OfType<byte[]>().Count();
        }

        public OutgoingPacket CreateOutgoing(
            Socket socket,
            SocketIOEventTypes socketIOEvent,
            int id,
            string name,
            object[] args)
        {
            _builder.Length = 0;
            // ReSharper disable once IdentifierTypo
            List<byte[]> attachements = null;

            switch (socketIOEvent)
            {
                case SocketIOEventTypes.Ack:
                {
                    if (GetBinaryCount(args) > 0)
                    {
                        attachements = CreatePlaceholders(args);
                        socketIOEvent = SocketIOEventTypes.BinaryAck;
                    }
                }

                    break;

                case SocketIOEventTypes.Event:
                {
                    if (GetBinaryCount(args) > 0)
                    {
                        attachements = CreatePlaceholders(args);
                        socketIOEvent = SocketIOEventTypes.BinaryEvent;
                    }
                }
                    break;
                case SocketIOEventTypes.Unknown:
                    break;
                case SocketIOEventTypes.Connect:
                    break;
                case SocketIOEventTypes.Disconnect:
                    break;
                case SocketIOEventTypes.Error:
                    break;
                case SocketIOEventTypes.BinaryEvent:
                    break;
                case SocketIOEventTypes.BinaryAck:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(socketIOEvent), socketIOEvent, null);
            }

            _builder.Append(((int)TransportEventTypes.Message).ToString());
            _builder.Append(((int)socketIOEvent).ToString());

            if (socketIOEvent is SocketIOEventTypes.BinaryEvent or SocketIOEventTypes.BinaryAck)
            {
                if (attachements != null)
                {
                    _builder.Append(attachements.Count.ToString());
                }

                _builder.Append('-');
            }

            /*
             * 添加命名空间如果有其他的，那么根nsp ("/")
             * 如果我们有更多的数据，那么我们必须在后面添加一个"，"。
             */
            bool nspAdded = false;
            if (socket.Namespace != "/")
            {
                _builder.Append(socket.Namespace);
                nspAdded = true;
            }

            // ack id, if any
            if (id >= 0)
            {
                if (nspAdded)
                {
                    _builder.Append(',');
                    nspAdded = false;
                }

                _builder.Append(id.ToString());
            }

            // payload
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (socketIOEvent)
            {
                case SocketIOEventTypes.Connect:
                {
                    // No Data | Object
                    if (args is { Length: > 0 })
                    {
                        if (nspAdded) _builder.Append(',');

                        _builder.Append(JSON.LitJson.JsonMapper.ToJson(args[0]));
                    }
                }
                    break;

                case SocketIOEventTypes.Disconnect:
                    // No Data
                    break;

                case SocketIOEventTypes.Error:
                {
                    // String | Object
                    if (args is { Length: > 0 })
                    {
                        if (nspAdded) _builder.Append(',');

                        _builder.Append(JSON.LitJson.JsonMapper.ToJson(args[0]));
                    }
                }
                    break;

                case SocketIOEventTypes.Ack:
                case SocketIOEventTypes.BinaryAck:
                {
                    if (nspAdded) _builder.Append(',');

                    if (args is { Length: > 0 })
                    {
                        var argsJson = JSON.LitJson.JsonMapper.ToJson(args);
                        _builder.Append(argsJson);
                    }
                    else
                    {
                        _builder.Append("[]");
                    }
                }
                    break;
                default:
                {
                    if (nspAdded) _builder.Append(',');

                    // Array
                    _builder.Append('[');
                    if (!string.IsNullOrEmpty(name))
                    {
                        _builder.Append('\"');
                        _builder.Append(name);
                        _builder.Append('\"');
                    }

                    if (args is { Length: > 0 })
                    {
                        _builder.Append(',');
                        var argsJson = JSON.LitJson.JsonMapper.ToJson(args);
                        _builder.Append(argsJson, 1, argsJson.Length - 2);
                    }

                    _builder.Append(']');
                }
                    break;
            }

            return new OutgoingPacket { Payload = _builder.ToString(), Attachements = attachements };
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private List<byte[]> CreatePlaceholders(IList<object> args)
        {
            // ReSharper disable once IdentifierTypo
            List<byte[]> attachements = null;

            for (var i = 0; i < args.Count; ++i)
            {
                if (args[i] is not byte[] binary) continue;
                attachements ??= new List<byte[]>();
                attachements.Add(binary);

                args[i] = new Placeholder { _placeholder = true, Num = attachements.Count - 1 };
            }

            return attachements;
        }
    }
}
#endif
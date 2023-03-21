#if !BESTHTTP_DISABLE_SIGNALR_CORE

using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP.PlatformSupport.Memory;
using BestHTTP.SignalRCore.Messages;
#if NETFX_CORE || NET_4_6
#endif

namespace BestHTTP.SignalRCore
{
    public interface IProtocol
    {
        string Name { get; }

        TransferModes Type { get; }

        IEncoder Encoder { get; }

        HubConnection Connection { get; set; }

        /// <summary>
        /// 此函数必须将消息的二进制表示解析为消息列表。
        /// </summary>
        void ParseMessages(BufferSegment segment, ref List<Message> messages);

        /// <summary>
        /// 此函数必须返回给定消息的编码表示。
        /// </summary>
        BufferSegment EncodeMessage(Message message);

        /// <summary>
        /// 该函数必须将arguments数组中的所有元素从argTypes数组转换为相应的类型。
        /// </summary>
        object[] GetRealArguments(Type[] argTypes, object[] arguments);

        /// <summary>
        /// 将值转换为给定类型。
        /// </summary>
        object ConvertTo(Type toType, object obj);
    }

    public abstract class JsonProtocol : IProtocol
    {
        public const char Separator = (char)0x1E;

        protected JsonProtocol(IEncoder encoder)
        {
            this.Encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
        }

        public string Name => "json";

        public TransferModes Type => TransferModes.Binary;

        public IEncoder Encoder { get; private set; }

        public HubConnection Connection { get; set; }

        public void ParseMessages(BufferSegment segment, ref List<Message> messages)
        {
            if (segment.Data == null || segment.Count == 0)
                return;

            int from = segment.Offset;
            int separatorIdx = Array.IndexOf(segment.Data, (byte)JsonProtocol.Separator, from);
            if (separatorIdx == -1)
            {
                throw new Exception($"数据中缺少分隔符!段: {segment.ToString()}");
            }

            while (separatorIdx != -1)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                var sb = new StringBuilder(3);
                var encodingString = Encoding.UTF8.GetString(segment.Data, from, separatorIdx - from);
                sb.Append($"ParseMessages - {encodingString}");
                Debug.Log(
                    $"[JsonProtocol] [method:ParseMessages] [msg]{sb}");
#endif
                var bufferSegment = new BufferSegment(
                    data: segment.Data,
                    from,
                    separatorIdx - from);
                var message = this.Encoder.DecodeAs<Message>(bufferSegment);

                messages.Add(message);

                from = separatorIdx + 1;
                separatorIdx = Array.IndexOf(segment.Data, (byte)JsonProtocol.Separator, from);
            }
        }

        public BufferSegment EncodeMessage(Message message)
        {
            BufferSegment result = BufferSegment.Empty;

            //虽然消息已经包含了所有的信息，规范规定消息中不允许有额外的字段，所以我们在这里创建“专门的”消息发送到服务器。
            switch (message.type)
            {
                case MessageTypes.StreamItem:
                {
                    var streamItem = new StreamItemMessage()
                    {
                        type = message.type,
                        invocationId = message.invocationId,
                        item = message.item
                    };
                    result = this.Encoder.Encode(streamItem);
                }
                    break;

                case MessageTypes.Completion:
                {
                    if (!string.IsNullOrEmpty(message.error))
                    {
                        var completionWith = new CompletionWithError()
                        {
                            type = MessageTypes.Completion,
                            invocationId = message.invocationId,
                            error = message.error
                        };
                        result = this.Encoder.Encode(completionWith);
                    }
                    else if (message.result != null)
                    {
                        var completionWith = new CompletionWithResult()
                        {
                            type = MessageTypes.Completion,
                            invocationId = message.invocationId,
                            result = message.result
                        };
                        result = this.Encoder.Encode(completionWith);
                    }
                    else
                    {
                        var completion = new Completion()
                        {
                            type = MessageTypes.Completion,
                            invocationId = message.invocationId
                        };
                        result = this.Encoder.Encode(completion);
                    }
                }
                    break;

                case MessageTypes.Invocation:
                case MessageTypes.StreamInvocation:
                {
                    if (message.streamIds != null)
                    {
                        var uploadMessage = new UploadInvocationMessage()
                        {
                            type = message.type,
                            invocationId = message.invocationId,
                            nonblocking = message.nonblocking,
                            target = message.target,
                            arguments = message.arguments,
                            streamIds = message.streamIds
                        };
                        result = this.Encoder.Encode(uploadMessage);
                    }
                    else
                    {
                        var invocationMessage = new InvocationMessage()
                        {
                            type = message.type,
                            invocationId = message.invocationId,
                            nonblocking = message.nonblocking,
                            target = message.target,
                            arguments = message.arguments
                        };
                        result = this.Encoder.Encode(invocationMessage);
                    }
                }
                    break;

                case MessageTypes.CancelInvocation:
                {
                    var cancelInvocation = new CancelInvocationMessage()
                    {
                        invocationId = message.invocationId
                    };
                    result = this.Encoder.Encode(cancelInvocation);
                }
                    break;

                case MessageTypes.Ping:
                {
                    result = this.Encoder.Encode(new PingMessage());
                }
                    break;

                case MessageTypes.Close:
                {
                    if (!string.IsNullOrEmpty(message.error))
                    {
                        var closeWithError = new CloseWithErrorMessage()
                        {
                            error = message.error
                        };
                        result = this.Encoder.Encode(closeWithError);
                    }
                    else
                    {
                        result = this.Encoder.Encode(new CloseMessage());
                    }
                }
                    break;
            }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            var sb = new StringBuilder(3);
            var encodingString = Encoding.UTF8.GetString(result.Data, 0, result.Count - 1);
            sb.Append($"EncodeMessage - json: {encodingString}");
            Debug.Log($"[JsonProtocol] [method:EncodeMessage] [msg]{sb}");
#endif

            return result;
        }

        public object[] GetRealArguments(Type[] argTypes, object[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
                return null;

            if (argTypes.Length > arguments.Length)
            {
                throw new Exception($"argType.Length({argTypes.Length}) < arguments.length({arguments.Length})");
            }

            object[] realArgs = new object[arguments.Length];

            for (int i = 0; i < arguments.Length; ++i)
            {
                realArgs[i] = ConvertTo(argTypes[i], arguments[i]);
            }

            return realArgs;
        }

        public object ConvertTo(Type toType, object obj)
        {
            if (obj == null)
            {
                return null;
            }

#if NETFX_CORE
            TypeInfo typeInfo = toType.GetTypeInfo();
#endif

#if NETFX_CORE
            if (typeInfo.IsEnum)
#else
            if (toType.IsEnum)
#endif
            {
                return Enum.Parse(toType, obj.ToString(), true);
            }

#if NETFX_CORE
            if (typeInfo.IsPrimitive)
#else
            if (toType.IsPrimitive)
#endif
            {
                return Convert.ChangeType(obj, toType);
            }

            if (toType == typeof(string))
            {
                return obj.ToString();
            }

#if NETFX_CORE
            if (typeInfo.IsGenericType && toType.Name == "Nullable`1")
                return Convert.ChangeType(obj, toType.GenericTypeArguments[0]);
#else
            if (toType.IsGenericType && toType.Name == "Nullable`1")
            {
                return Convert.ChangeType(obj, toType.GetGenericArguments()[0]);
            }
#endif

            return this.Encoder.ConvertTo(toType, obj);
        }

        /// <summary>
        /// 返回给定字符串参数的字节和添加的分隔符(0x1E)。
        /// </summary>
        public static BufferSegment WithSeparator(string str)
        {
            int len = Encoding.UTF8.GetByteCount(str);

            byte[] buffer = BufferPool.Get(len + 1, true);

            Encoding.UTF8.GetBytes(str, 0, str.Length, buffer, 0);

            buffer[len] = 0x1e;

            return new BufferSegment(buffer, 0, len + 1);
        }
    }
}
#endif
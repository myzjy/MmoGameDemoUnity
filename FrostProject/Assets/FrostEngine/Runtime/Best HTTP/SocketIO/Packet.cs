#if !BESTHTTP_DISABLE_SOCKETIO

using System.Text;

namespace BestHTTP.SocketIO
{
    using System;
    using System.Collections.Generic;
    using JSON;

    public sealed class Packet
    {
        private enum PayloadTypes : byte
        {
            Textual = 0,
            Binary = 1
        }

        public const string Placeholder = "_placeholder";

        /// <summary>
        /// 传输层报文的事件类型。
        /// </summary>
        public TransportEventTypes TransportEvent { get; private set; }

        /// <summary>
        /// 数据包在Socket中的类型。IO协议。
        /// </summary>
        public SocketIOEventTypes SocketIOEvent { get; private set; }

        /// <summary>
        /// 这个包需要多少附件?
        /// </summary>
        public int AttachmentCount { get; private set; }

        /// <summary>
        /// 报文内部的ack-id。
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// 广播pack的姓名
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        /// 作为Json字符串的有效负载。
        /// </summary>
        public string Payload { get; private set; }

        /// <summary>
        /// 从有效负载字符串解码的事件名称。
        /// </summary>
        private string EventName { get; set; }

        /// <summary>
        /// 所有二进制数据附加到此事件。
        /// </summary>
        public List<byte[]> Attachments
        {
            get => _attachments;
            set
            {
                _attachments = value;
                AttachmentCount = _attachments?.Count ?? 0;
            }
        }

        private List<byte[]> _attachments;

        /// <summary>
        /// 属性检查是否收到此包的所有附件。
        /// </summary>
        public bool HasAllAttachment => Attachments != null && Attachments.Count == AttachmentCount;

        /// <summary>
        /// 如果已经解码，则为。Decode调用之后，DecodedArgs仍然可以为空。
        /// </summary>
        public bool IsDecoded { get; private set; }

        /// <summary>
        /// 从Json字符串的结果解码的参数->c#对象转换。
        /// </summary>
        private object[] DecodedArgs { get; set; }

        /// <summary>
        /// 内部构造函数。不要直接使用它!
        /// </summary>
        internal Packet()
        {
            this.TransportEvent = TransportEventTypes.Unknown;
            this.SocketIOEvent = SocketIOEventTypes.Unknown;
            this.Payload = string.Empty;
        }

        /// <summary>
        /// 内部构造函数。不要直接使用它!
        /// </summary>
        internal Packet(string from)
        {
            this.Parse(from);
        }

        /// <summary>
        /// 内部构造函数。不要直接使用它!
        /// </summary>
        public Packet(TransportEventTypes transportEvent, SocketIOEventTypes packetType, string nsp, string payload,
            int attachment = 0, int id = 0)
        {
            this.TransportEvent = transportEvent;
            this.SocketIOEvent = packetType;
            this.Namespace = nsp;
            this.Payload = payload;
            this.AttachmentCount = attachment;
            this.Id = id;
        }

        public object[] Decode(JsonEncoders.IJsonEncoder encoder)
        {
            if (IsDecoded || encoder == null)
                return DecodedArgs;

            IsDecoded = true;

            if (string.IsNullOrEmpty(Payload))
                return DecodedArgs;

            List<object> decoded = encoder.Decode(Payload);

            if (decoded is { Count: > 0 })
            {
                if (this.SocketIOEvent == SocketIOEventTypes.Ack || this.SocketIOEvent == SocketIOEventTypes.BinaryAck)
                    DecodedArgs = decoded.ToArray();
                else
                {
                    decoded.RemoveAt(0);

                    DecodedArgs = decoded.ToArray();
                }
            }

            return DecodedArgs;
        }

        /// <summary>
        /// 将从数据包的有效负载字符串中设置并返回EventName。
        /// </summary>
        public string DecodeEventName()
        {
            // Already decoded
            if (!string.IsNullOrEmpty(EventName))
                return EventName;

            // No Payload to decode
            if (string.IsNullOrEmpty(Payload))
                return string.Empty;

            // 不是数组编码，我们无法解码
            if (Payload[0] != '[')
                return string.Empty;

            var idx = 1;

            // Search for the string-begin mark( ' or " chars)
            while (Payload.Length > idx &&
                   Payload[idx] != '"' &&
                   Payload[idx] != '\'')
            {
                idx++;
            }

            // 到达Payload数组的末端
            if (Payload.Length <= idx)
            {
                return string.Empty;
            }

            int startIdx = ++idx;

            // Search for the trailing mark of the string
            while (Payload.Length > idx &&
                   Payload[idx] != '"' &&
                   Payload[idx] != '\'')
            {
                idx++;
            }

            // 到达Payload数组的末端
            if (Payload.Length <= idx)
            {
                return string.Empty;
            }

            return EventName = Payload.Substring(startIdx, idx - startIdx);
        }

        public string RemoveEventName(bool removeArrayMarks)
        {
            // No Payload to decode
            if (string.IsNullOrEmpty(Payload))
            {
                return string.Empty;
            }

            // Not array encoded, we can't decode
            if (Payload[0] != '[')
            {
                return string.Empty;
            }

            int idx = 1;

            // Search for the string-begin mark( ' or " chars)
            while (Payload.Length > idx && Payload[idx] != '"' && Payload[idx] != '\'')
            {
                idx++;
            }

            // Reached the end of the string
            if (Payload.Length <= idx)
            {
                return string.Empty;
            }

            int startIdx = idx;

            // 搜索第一个元素的结束，或者数组标记的结束
            while (Payload.Length > idx && Payload[idx] != ',' && Payload[idx] != ']')
            {
                idx++;
            }

            // 到达Payload的末端
            if (Payload.Length <= ++idx)
            {
                return string.Empty;
            }

            string payload = Payload.Remove(startIdx, idx - startIdx);

            if (removeArrayMarks)
            {
                payload = payload.Substring(1, payload.Length - 2);
            }

            return payload;
        }

        /// <summary>
        /// Will switch the "{'_placeholder':true,'num':X}" to a the index num X.
        /// </summary>
        /// <returns>如果重构成功则为True，否则为false。</returns>
        public bool ReconstructAttachmentAsIndex()
        {
            //"452-["multiImage",{"image":true,"buffer1":{"_placeholder":true,"num":0},"buffer2":{"_placeholder":true,"num":1}}]"
            void Found(string json, Dictionary<string, object> dict)
            {
                var idx = Convert.ToInt32(dict["num"]);
                this.Payload = this.Payload.Replace(json, idx.ToString());
                this.IsDecoded = false;
            }

            return PlaceholderReplacer(Found);
        }

        /// <summary>
        /// Will switch the "{'_placeholder':true,'num':X}" to a the data as a base64 encoded string.
        /// </summary>
        /// <returns>如果重构成功则为True，否则为false。</returns>
        public bool ReconstructAttachmentAsBase64()
        {
            //"452-["multiImage",{"image":true,"buffer1":{"_placeholder":true,"num":0},"buffer2":{"_placeholder":true,"num":1}}]"

            if (!HasAllAttachment)
                return false;

            return PlaceholderReplacer((json, obj) =>
            {
                int idx = Convert.ToInt32(obj["num"]);
                this.Payload = this.Payload.Replace(
                    json,
                    $"\"{Convert.ToBase64String(this.Attachments[idx])}\"");
                this.IsDecoded = false;
            });
        }

        /// <summary>
        /// 解析来自服务器发送的文本数据的数据包。负载将是原始的json字符串。
        /// </summary>
        private void Parse(string from)
        {
            int idx = 0;
            this.TransportEvent = (TransportEventTypes)ToInt(from[idx++]);

            if (from.Length > idx && ToInt(from[idx]) >= 0)
                this.SocketIOEvent = (SocketIOEventTypes)ToInt(from[idx++]);
            else
                this.SocketIOEvent = SocketIOEventTypes.Unknown;

            // Parse Attachment
            if (this.SocketIOEvent is SocketIOEventTypes.BinaryEvent or SocketIOEventTypes.BinaryAck)
            {
                int endIdx = from.IndexOf('-', idx);
                if (endIdx == -1)
                {
                    endIdx = from.Length;
                }

                int.TryParse(from.Substring(idx, endIdx - idx), out var attachment);
                this.AttachmentCount = attachment;
                idx = endIdx + 1;
            }

            // Parse Namespace
            if (from.Length > idx && from[idx] == '/')
            {
                int endIdx = from.IndexOf(',', idx);
                if (endIdx == -1)
                    endIdx = from.Length;

                this.Namespace = from.Substring(idx, endIdx - idx);
                idx = endIdx + 1;
            }
            else
            {
                this.Namespace = "/";
            }

            // Parse Id
            if (from.Length > idx && ToInt(from[idx]) >= 0)
            {
                int startIdx = idx++;
                while (from.Length > idx && ToInt(from[idx]) >= 0)
                {
                    idx++;
                }

                int.TryParse(from.Substring(startIdx, idx - startIdx), out var id);
                this.Id = id;
            }

            // 剩下的是有效载荷数据
            this.Payload = from.Length > idx ? from[idx..] : string.Empty;
        }

        /// <summary>
        ///自定义函数代替char。GetNumericValue，因为它在WebGL下使用新的4抛出一个错误。x运行时。
        ///如果char是数值类型，则返回-1。
        /// </summary>
        private int ToInt(char ch)
        {
            int charValue = Convert.ToInt32(ch);
            int num = charValue - '0';
            if (num is < 0 or > 9)
            {
                return -1;
            }

            return num;
        }

        /// <summary>
        /// 将此包编码到套接字。IO格式化的字符串。
        /// </summary>
        internal string Encode()
        {
            StringBuilder builder = new StringBuilder();

            // 如果没有设置，则设置为消息，并且我们正在发送附件
            if (this.TransportEvent == TransportEventTypes.Unknown &&
                this.AttachmentCount > 0)
            {
                this.TransportEvent = TransportEventTypes.Message;
            }

            if (this.TransportEvent != TransportEventTypes.Unknown)
            {
                builder.Append(((int)this.TransportEvent).ToString());
            }

            // 如果没有设置，则设置为BinaryEvent，并且我们正在发送附件
            if (this.SocketIOEvent == SocketIOEventTypes.Unknown &&
                this.AttachmentCount > 0)
            {
                this.SocketIOEvent = SocketIOEventTypes.BinaryEvent;
            }

            if (this.SocketIOEvent != SocketIOEventTypes.Unknown)
            {
                builder.Append(((int)this.SocketIOEvent).ToString());
            }

            if (this.SocketIOEvent == SocketIOEventTypes.BinaryEvent ||
                this.SocketIOEvent == SocketIOEventTypes.BinaryAck)
            {
                builder.Append(this.AttachmentCount.ToString());
                builder.Append("-");
            }

            //添加命名空间如果有其他根nsp("/")，那么如果我们有更多的数据，我们必须添加一个后缀"，"。
            bool nspAdded = false;
            if (this.Namespace != "/")
            {
                builder.Append(this.Namespace);
                nspAdded = true;
            }

            // ack id, if any
            if (this.Id != 0)
            {
                if (nspAdded)
                {
                    builder.Append(",");
                    nspAdded = false;
                }

                builder.Append(this.Id.ToString());
            }

            // payload
            if (!string.IsNullOrEmpty(this.Payload))
            {
                if (nspAdded)
                {
                    builder.Append(",");
                    // ReSharper disable once RedundantAssignment
                    nspAdded = false;
                }

                builder.Append(this.Payload);
            }

            return builder.ToString();
        }

        /// <summary>
        /// 将此包编码到套接字。IO格式化的字节数组。
        /// </summary>
        internal byte[] EncodeBinary()
        {
            if (AttachmentCount != 0 || (Attachments != null && Attachments.Count != 0))
            {
                if (Attachments == null)
                {
                    throw new ArgumentException("packet.Attachments are null!");
                }

                if (AttachmentCount != Attachments.Count)
                {
                    throw new ArgumentException(
                        "packet.AttachmentCount != packet.Attachments.Count. 使用packet.AddAttachment函数，添加数据到数据包!");
                }
            }

            // 像往常一样编码
            string encoded = Encode();

            // 将其转换为byte[]
            byte[] payload = Encoding.UTF8.GetBytes(encoded);

            // 将其编码为消息
            byte[] buffer = EncodeData(payload, PayloadTypes.Textual, null);

            // 如果有附件，也转换它们，并将它们依次附加
            if (AttachmentCount != 0)
            {
                int idx = buffer.Length;

                // 列表临时保存转换后的附件
                List<byte[]> attachmentDataList = new List<byte[]>(AttachmentCount);

                // 转换后的附件的总和只能调整缓冲区的大小一次。这样我们可以避免GC垃圾
                int attachmentDataSize = 0;

                // 编码我们的附件，并将它们存储在列表中
                for (int i = 0; i < AttachmentCount; i++)
                {
                    if (Attachments == null) continue;
                    var tmpBuff = EncodeData(
                        data: Attachments[i],
                        type: PayloadTypes.Binary,
                        afterHeaderData: new byte[] { 4 });
                    attachmentDataList.Add(tmpBuff);

                    attachmentDataSize += tmpBuff.Length;
                }

                // 调整缓冲区的大小一次
                Array.Resize(ref buffer, buffer.Length + attachmentDataSize);

                // 把所有数据复制进去
                for (int i = 0; i < AttachmentCount; ++i)
                {
                    byte[] data = attachmentDataList[i];
                    Array.Copy(
                        sourceArray: data,
                        sourceIndex: 0,
                        destinationArray: buffer,
                        destinationIndex: idx,
                        length: data.Length);

                    idx += data.Length;
                }
            }

            // 返回缓冲区
            return buffer;
        }

        /// <summary>
        /// 将服务器发送的byte[]添加到附件列表。
        /// </summary>
        internal void AddAttachmentFromServer(byte[] data, bool copyFull)
        {
            if (data == null || data.Length == 0)
                return;

            this._attachments ??= new List<byte[]>(this.AttachmentCount);

            if (copyFull)
            {
                this.Attachments.Add(data);
            }
            else
            {
                byte[] buff = new byte[data.Length - 1];
                Array.Copy(
                    sourceArray: data,
                    sourceIndex: 1,
                    destinationArray: buff,
                    destinationIndex: 0,
                    length: data.Length - 1);

                this.Attachments.Add(buff);
            }
        }


        /// <summary>
        /// 将字节数组编码到套接字。输入输出二进制编码报文
        /// </summary>
        private
            byte[] EncodeData(
                byte[] data,
                PayloadTypes type,
                byte[] afterHeaderData)
        {
            // Packet binary encoding:
            // [          0|1         ][            length of data           ][    FF    ][data]
            // <1 = binary, 0 = string><number from 0-9><number from 0-9>[...]<number 255><data>

            //获取有效载荷的长度。套接字。IO使用一种浪费的编码来发送数据的长度。
            //如果数据是16字节，我们必须将长度作为两个字节发送:字符'1'的字节值和字符'6'的字节值。
            //不是只有一个字节:0xF。如果有效负载是123字节，我们不能作为0x7B发送…
            int afterHeaderLength = afterHeaderData?.Length ?? 0;
            string lenStr = (data.Length + afterHeaderLength).ToString();
            byte[] len = new byte[lenStr.Length];
            for (int cv = 0; cv < lenStr.Length; ++cv)
                len[cv] = (byte)char.GetNumericValue(lenStr[cv]);

            // 我们需要另一个缓冲区来存储最终数据
            byte[] buffer = new byte[data.Length + len.Length + 2 + afterHeaderLength];

            // The payload is textual -> 0
            buffer[0] = (byte)type;

            // 复制数据的长度
            for (int cv = 0; cv < len.Length; ++cv)
            {
                buffer[1 + cv] = len[cv];
            }

            int idx = 1 + len.Length;

            // 头数据结束
            buffer[idx++] = 0xFF;

            if (afterHeaderData is { Length: > 0 })
            {
                Array.Copy(
                    sourceArray: afterHeaderData,
                    sourceIndex: 0,
                    destinationArray: buffer,
                    destinationIndex: idx,
                    length: afterHeaderData.Length);
                idx += afterHeaderData.Length;
            }

            // 将有效负载数据复制到缓冲区
            Array.Copy(
                sourceArray: data,
                sourceIndex: 0,
                destinationArray: buffer,
                destinationIndex: idx,
                length: data.Length);

            return buffer;
        }

        /// <summary>
        /// 搜索"{'_placeholder':true，'num':X}"字符串，并调用给定的操作来修改PayLoad
        /// </summary>
        private
            bool PlaceholderReplacer(Action<string, Dictionary<string, object>> onFound)
        {
            if (string.IsNullOrEmpty(this.Payload))
            {
                return false;
            }

            // 找到"_placeholder" str的第一个索引
            var placeholderIdx = this.Payload.IndexOf(
                value: Placeholder,
                comparisonType: StringComparison.Ordinal);

            while (placeholderIdx >= 0)
            {
                // 找到对象启动令牌
                int startIdx = placeholderIdx;
                while (this.Payload[startIdx] != '{')
                    startIdx--;

                // 找到对象结束令牌
                int endIdx = placeholderIdx;
                while (this.Payload.Length > endIdx &&
                       this.Payload[endIdx] != '}')
                {
                    endIdx++;
                }

                //我们到达了终点
                if (this.Payload.Length <= endIdx)
                {
                    return false;
                }

                // 获取对象，并解码它
                string placeholderJson = this.Payload.Substring(startIdx, endIdx - startIdx + 1);
                bool success = false;
                var obj =
                    Json.Decode(placeholderJson, ref success)
                        as Dictionary<string, object>;
                if (!success)
                {
                    return false;
                }

                // 检查_placeholder的存在和值
                if (obj != null &&
                    (!obj.TryGetValue(Placeholder, out var value) ||
                     !(bool)value))
                {
                    return false;
                }

                // 检查是否存在num
                if (obj != null && !obj.TryGetValue("num", out value))
                {
                    return false;
                }

                // 让我们做我们该做的
                onFound(placeholderJson, obj);

                // 找到下一个附件，如果有的话
                placeholderIdx = this.Payload.IndexOf(Placeholder, StringComparison.Ordinal);
            }

            return true;
        }


        /// <summary>
        /// 返回此包的有效负载。
        /// </summary>
        public override string ToString()
        {
            return this.Payload;
        }

        /// <summary>
        /// 将这个包克隆到一个相同的包实例。
        /// </summary>
        internal Packet Clone()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            Packet packet = new Packet(
                transportEvent: this.TransportEvent,
                packetType: this.SocketIOEvent,
                nsp: this.Namespace,
                payload: this.Payload,
                attachment: 0,
                id: this.Id);
            packet.EventName = this.EventName;
            packet.AttachmentCount = this.AttachmentCount;
            packet._attachments = this._attachments;

            return packet;
        }
    }
}

#endif
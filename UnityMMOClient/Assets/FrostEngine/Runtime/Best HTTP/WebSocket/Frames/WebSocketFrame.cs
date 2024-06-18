#if !BESTHTTP_DISABLE_WEBSOCKET && (!UNITY_WEBGL || UNITY_EDITOR)

using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.Memory;
using System;

namespace BestHTTP.WebSocket.Frames
{
    public struct RawFrameData : IDisposable
    {
        public byte[] Data;
        public readonly int Length;

        public RawFrameData(byte[] data, int length)
        {
            Data = data;
            Length = length;
        }

        public void Dispose()
        {
            BufferPool.Release(Data);
            Data = null;
        }
    }

    /// <summary>
    ///表示二进制帧。“有效负载数据”是任意二进制数据，其解释完全取决于应用层。
    /// 这是所有其他帧写入器的基类，因为所有帧都可以表示为字节数组。
    /// </summary>
    [PlatformSupport.IL2CPP.Il2CppEagerStaticClassConstructionAttribute]
    public sealed class WebSocketFrame
    {
        public WebSocketFrameTypes Type { get; private set; }
        private bool IsFinal { get; set; }
        public byte Header { get; private set; }

        public byte[] Data { get; private set; }
        public int DataLength { get; private set; }
        private bool UseExtensions { get; set; }

        public override string ToString()
        {
            return
                $"[WebSocketFrame Type: {this.Type}, I" +
                $"sFinal: {this.IsFinal}, " +
                $"Header: {this.Header:X2}," +
                $" DataLength: {this.DataLength}," +
                $" UseExtensions: {this.UseExtensions}]";
        }


        public WebSocketFrame(WebSocket webSocket, WebSocketFrameTypes type, byte[] data, bool useExtensions = true)
            : this(
                webSocket: webSocket,
                type: type,
                data: data,
                pos: 0,
                length: data != null ? (ulong)data.Length : 0,
                isFinal: true,
                useExtensions: useExtensions)
        {
        }

        public WebSocketFrame(
            WebSocket webSocket,
            WebSocketFrameTypes type,
            byte[] data,
            bool isFinal,
            bool useExtensions)
            : this(
                webSocket: webSocket,
                type: type,
                data: data,
                pos: 0,
                length: data != null ? (ulong)data.Length : 0,
                isFinal: isFinal,
                useExtensions: useExtensions)
        {
        }

        public WebSocketFrame(
            WebSocket webSocket,
            WebSocketFrameTypes type,
            byte[] data,
            UInt64 pos,
            UInt64 length,
            bool isFinal,
            bool useExtensions)
        {
            this.Type = type;
            this.IsFinal = isFinal;
            this.UseExtensions = useExtensions;

            this.DataLength = (int)length;
            if (data != null)
            {
                this.Data = BufferPool.Get(this.DataLength, true);
                Array.Copy(data, (int)pos, this.Data, 0, this.DataLength);
            }
            else
            {
                // ReSharper disable once RedundantAssignment
                data = BufferPool.NoData;
            }

            // First byte: Final Bit + Rsv flags + OpCode
            byte finalBit = (byte)(IsFinal ? 0x80 : 0x0);
            this.Header = (byte)(finalBit | (byte)Type);

            if (!this.UseExtensions || webSocket?.Extensions == null) return;
            foreach (var ext in webSocket.Extensions)
            {
                if (ext == null) continue;
                this.Header |= ext.GetFrameHeader(this, this.Header);
                byte[] newData = ext.Encode(this);

                if (newData == this.Data) continue;
                BufferPool.Release(this.Data);

                this.Data = newData;
                this.DataLength = newData.Length;
            }
        }

        public unsafe RawFrameData Get()
        {
            Data ??= BufferPool.NoData;

            using var ms = new BufferPoolMemoryStream(this.DataLength + 9);
            // For the complete documentation for this section see:
            // http://tools.ietf.org/html/rfc6455#section-5.2

            // Write the header
            ms.WriteByte(this.Header);
            /*
             * “Payload data”的长度，以字节为单位:如果0-125，就是Payload长度。
             * 如果是126，下面2个被解释为16位无符号整数的字节就是有效负载长度。
             * 如果是127，则以下8个字节解释为64位无符号整数(最高位必须为0)是有效负载长度。
             * 多字节长度量以网络字节顺序表示。                 * 
             */
            switch (this.DataLength)
            {
                case < 126:
                {
                    ms.WriteByte((byte)(0x80 | (byte)this.DataLength));
                }
                    break;
                case < ushort.MaxValue:
                {
                    ms.WriteByte(0x80 | 126);
                    var len = BitConverter.GetBytes((UInt16)this.DataLength);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(len, 0, len.Length);

                    ms.Write(len, 0, len.Length);
                    break;
                }
                default:
                {
                    ms.WriteByte(0x80 | 127);
                    var len = BitConverter.GetBytes((ulong)this.DataLength);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(len, 0, len.Length);
                    }

                    ms.Write(len, 0, len.Length);
                    break;
                }
            }

            /*
             * 所有从客户端发送到服务器端的帧都被包含在帧中的32位值屏蔽。
             * 如果掩码位设置为1，则该字段存在;如果掩码位设置为0，则该字段不存在。
             * 如果数据是由客户端发送，帧必须被屏蔽。
             */
            byte[] mask = BufferPool.Get(4, true);

            int hash = this.GetHashCode();

            mask[0] = (byte)((hash >> 24) & 0xFF);
            mask[1] = (byte)((hash >> 16) & 0xFF);
            mask[2] = (byte)((hash >> 8) & 0xFF);
            mask[3] = (byte)(hash & 0xFF);

            ms.Write(mask, 0, 4);

            // Do the masking.
            fixed (byte* pData = Data, pMask = mask)
            {
                /*
                 * 在这里，我们不是一个字节一个字节地解释，而是将数据重新解释为单位，并应用屏蔽so。
                 * 这样，我们可以在一个周期中屏蔽4个字节，而不是1个
                 */
                int localLength = this.DataLength / 4;
                if (localLength > 0)
                {
                    uint* upData = (uint*)pData;
                    uint umask = *(uint*)pMask;

                    unchecked
                    {
                        for (var i = 0; i < localLength; ++i)
                        {
                            upData[i] = upData[i] ^ umask;
                        }
                    }
                }

                // Because data might not be exactly dividable by 4, we have to mask the remaining 0..3 too.
                int from = localLength * 4;
                localLength = from + this.DataLength % 4;
                for (int i = from; i < localLength; ++i)
                    pData[i] = (byte)(pData[i] ^ pMask[i % 4]);
            }

            BufferPool.Release(mask);

            ms.Write(Data, 0, DataLength);

            return new RawFrameData(ms.ToArray(true), (int)ms.Length);
        }


        public WebSocketFrame[] Fragment(uint maxFragmentSize)
        {
            if (this.Data == null)
                return null;

            // 所有控制帧的有效载荷长度必须小于等于125字节，并且不能被分割。
            if (this.Type != WebSocketFrameTypes.Binary && this.Type != WebSocketFrameTypes.Text)
                return null;

            if (this.DataLength <= maxFragmentSize)
                return null;

            this.IsFinal = false;

            // 清除标题标志中的最后一位
            this.Header &= 0x7F;

            // 一个块将保留在这个片段中，所以我们必须少分配一个
            int count = (int)((this.DataLength / maxFragmentSize) + (this.DataLength % maxFragmentSize == 0 ? -1 : 0));

            WebSocketFrame[] fragments = new WebSocketFrame[count];

            // 跳过当前的一部分
            UInt64 pos = maxFragmentSize;
            while (pos < (ulong)this.DataLength)
            {
                var chunkLength = Math.Min(maxFragmentSize, (ulong)this.DataLength - pos);

                fragments[^count--] = new WebSocketFrame(
                    webSocket: null,
                    type: WebSocketFrameTypes.Continuation,
                    data: this.Data,
                    pos: pos,
                    length: chunkLength,
                    isFinal: pos + chunkLength >= (ulong)this.DataLength,
                    useExtensions: false);

                pos += chunkLength;
            }

            //byte[] newData = VariableSizedBufferPool.Get(maxFragmentSize, true);
            //Array.Copy(this.Data, 0, newData, 0, maxFragmentSize);
            //VariableSizedBufferPool.Release(this.Data);

            //this.Data = newData;
            this.DataLength = (int)maxFragmentSize;

            return fragments;
        }
    }
}

#endif
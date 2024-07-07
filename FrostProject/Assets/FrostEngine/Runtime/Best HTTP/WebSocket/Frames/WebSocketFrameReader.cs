#if !BESTHTTP_DISABLE_WEBSOCKET && (!UNITY_WEBGL || UNITY_EDITOR)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.Memory;

namespace BestHTTP.WebSocket.Frames
{
    /// <summary>
    /// 表示一个传入的WebSocket帧。
    /// </summary>
    public struct WebSocketFrameReader
    {
        private byte Header { get; set; }

        /// <summary>
        /// 如果它是序列中的最后一个帧，或唯一一个，则为True。
        /// </summary>
        public bool IsFinal { get; private set; }

        /// <summary>
        /// 帧的类型。
        /// </summary>
        public WebSocketFrameTypes Type { get; private set; }

        /// <summary>
        /// 指示是否有任何掩码发送来解码数据。
        /// </summary>
        public bool HasMask { get; private set; }

        /// <summary>
        /// 数据的长度。
        /// </summary>
        public UInt64 Length { get; private set; }

        /// <summary>
        /// 解码后的字节数组。
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// 接收数据的文本表示。
        /// </summary>
        public string DataAsText { get; private set; }

        internal unsafe void Read(Stream stream)
        {
#if (UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG) && ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}]");
                sb.Append($"{sf.GetMethod().Name}");
                sb.Append($"Line:{sf.GetFileLineNumber()}");
                sb.Append($"[msg] Read");
                Debug.Log($"{sb}");
            }
#endif
            // For the complete documentation for this section see:
            // http://tools.ietf.org/html/rfc6455#section-5.2

            this.Header = ReadByte(stream);

            // 第一个字节是最终位和帧的类型
            IsFinal = (this.Header & 0x80) != 0;
            Type = (WebSocketFrameTypes)(this.Header & 0xF);

            byte maskAndLength = ReadByte(stream);
            /*
             * 第二个字节是掩码位和有效负载数据的长度
             */
            HasMask = (maskAndLength & 0x80) != 0;
            /*
             * 如果0-125，这是有效载荷长度。
             */
            Length = (UInt64)(maskAndLength & 127);

            /*
             * 如果是126，下面2个被解释为16位无符号整数的字节就是有效负载长度。
             */
            if (Length == 126)
            {
                byte[] rawLen = BufferPool.Get(2, true);

                stream.ReadBuffer(rawLen, 2);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(rawLen, 0, 2);

                Length = BitConverter.ToUInt16(rawLen, 0);

                BufferPool.Release(rawLen);
            }
            else if (Length == 127)
            {
                /*
                 *如果是127，则以下8个字节解释为64位无符号整数(最高位必须为0)是有效负载长度。
                 */
                byte[] rawLen = BufferPool.Get(8, true);

                stream.ReadBuffer(rawLen, 8);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(rawLen, 0, 8);

                Length = BitConverter.ToUInt64(rawLen, 0);

                BufferPool.Release(rawLen);
            }

            /*
             * 发送的字节数组作为掩码来解码数据。
             */
            byte[] mask = null;

            // Read the Mask, if has any
            if (HasMask)
            {
                mask = BufferPool.Get(4, true);
                if (stream.Read(mask, 0, 4) < mask.Length)
                {
                    throw ExceptionHelper.ServerClosedTCPStream();
                }
            }

            if (Type is WebSocketFrameTypes.Text or WebSocketFrameTypes.Continuation)
            {
                Data = BufferPool.Get((long)Length, true);
            }
            else if (Length == 0)
            {
                Data = BufferPool.NoData;
            }
            else
            {
                Data = new byte[Length];
            }
            //Data = Type == WebSocketFrameTypes.Text ? VariableSizedBufferPool.Get((long)Length, true) : new byte[Length];

            if (Length == 0L)
                return;

            uint readLength = 0;

            do
            {
                int read = stream.Read(Data, (int)readLength, (int)(Length - readLength));

                if (read <= 0)
                    throw ExceptionHelper.ServerClosedTCPStream();

                readLength += (uint)read;
            } while (readLength < Length);

            if (!HasMask) return;
            fixed (byte* pData = Data, pMask = mask)
            {
                /*
                 * 在这里，我们不是一个字节一个字节地解释，而是将数据重新解释为单位，并应用屏蔽so。
                 * 这样，我们可以在一个循环中屏蔽4个字节，而不是1个
                 */
                ulong localLength = this.Length / 4;
                if (localLength > 0)
                {
                    uint* upData = (uint*)pData;
                    uint umask = *(uint*)pMask;

                    unchecked
                    {
                        for (ulong i = 0; i < localLength; ++i)
                        {
                            upData[i] = upData[i] ^ umask;
                        }
                    }
                }

                /*
                 * 因为数据可能不能被4整除，我们必须屏蔽剩下的0..3。
                 */
                ulong from = localLength * 4;
                localLength = from + this.Length % 4;
                for (ulong i = from; i < localLength; ++i)
                {
                    if (pMask != null) pData[i] = (byte)(pData[i] ^ pMask[i % 4]);
                }
            }

            BufferPool.Release(mask);
#if (UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG) && ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}]");
                sb.Append($"{sf.GetMethod().Name}");
                sb.Append($"Line:{sf.GetFileLineNumber()}");
                sb.Append($"[msg] End Read");
                Debug.Log($"{sb}");
            }
#endif
        }

        private byte ReadByte(Stream stream)
        {
            int read = stream.ReadByte();

            if (read < 0)
            {
                throw ExceptionHelper.ServerClosedTCPStream();
            }

            return (byte)read;
        }


        /// <summary>
        /// 将所有片段组装成最终帧。在帧的最后一个片段上调用此函数。
        /// </summary>
        /// <param name="fragments">之前下载和解析过的帧片段的列表</param>
        public void Assemble(List<WebSocketFrameReader> fragments)
        {
#if (UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG) && ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}]");
                sb.Append($"{sf.GetMethod().Name}");
                sb.Append($"Line:{sf.GetFileLineNumber()}");
                sb.Append($"[msg] 将所有片段组装成最终帧。在帧的最后一个片段上调用此函数。");
                Debug.Log($"{sb}");
            }
#endif
            /*
             * 这样，下面的算法也会处理这个片段的数据
             */
            fragments.Add(this);

            UInt64 finalLength = fragments.Aggregate<WebSocketFrameReader, ulong>(seed: 0,
                func: (current, t) => current + t.Length);

            byte[] buffer = fragments[0].Type == WebSocketFrameTypes.Text
                ? BufferPool.Get((long)finalLength, true)
                : new byte[finalLength];
            ulong pos = 0;
            for (int i = 0; i < fragments.Count; ++i)
            {
                Array.Copy(fragments[i].Data, 0, buffer, (int)pos, (int)fragments[i].Length);
                fragments[i].ReleaseData();

                pos += fragments[i].Length;
            }

            /*
             * 消息的所有片段都具有相同的类型，由第一个片段的操作码设置。
             */
            this.Type = fragments[0].Type;

            /*
             * 保留标志可能只包含在第一个片段中
             */
            this.Header = fragments[0].Header;
            this.Length = finalLength;
            this.Data = buffer;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DecodeWithExtensions(WebSocket webSocket)
        {
#if (UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG) && ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}]");
                sb.Append($"{sf.GetMethod().Name}");
                sb.Append($"Line:{sf.GetFileLineNumber()}");
                sb.Append($"[msg] 将使用相关websocket的扩展对接收到的数据进行增量解码。");
                Debug.Log($"{sb}");
            }
#endif
            if (webSocket.Extensions != null)
            {
                foreach (var ext in webSocket.Extensions)
                {
                    if (ext == null) continue;
                    var newData = ext.Decode(this.Header, this.Data, (int)this.Length);
                    if (this.Data == newData) continue;
                    this.ReleaseData();
                    this.Data = newData;
                    this.Length = (ulong)newData.Length;
                }
            }

            if (this.Type != WebSocketFrameTypes.Text || this.Data == null) return;
            this.DataAsText = Encoding.UTF8.GetString(this.Data, 0, (int)this.Length);
            this.ReleaseData();
        }

        public void ReleaseData()
        {
#if (UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG) && ENABLE_LOG_NETWORK
            {
                var st = new StackTrace(new StackFrame(true));
                var sf = st.GetFrame(0);
                var sb = new StringBuilder(6);
                sb.Append($"[{sf.GetFileName()}]");
                sb.Append($"[method:{sf.GetMethod().Name}]");
                sb.Append($"{sf.GetMethod().Name}");
                sb.Append($"Line:{sf.GetFileLineNumber()}");
                sb.Append($"[msg]");
                Debug.Log($"{sb}");
            }
#endif
            if (this.Data == null) return;
            BufferPool.Release(this.Data);
            this.Data = null;
        }
    }
}

#endif
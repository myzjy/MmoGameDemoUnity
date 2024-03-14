#if !BESTHTTP_DISABLE_WEBSOCKET && (!UNITY_WEBGL || UNITY_EDITOR)

using System;
using System.Text;
using BestHTTP.Decompression.Zlib;
using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.Memory;
using BestHTTP.WebSocket.Frames;

namespace BestHTTP.WebSocket.Extensions
{
    /// <summary>
    /// WebSocket实现的压缩扩展。
    /// http://tools.ietf.org/html/rfc7692
    /// </summary>
    public sealed class PerMessageCompression : IExtension
    {
        public
            const
            int MinDataLengthToCompressDefault = 256;

        private
            static
            readonly
            byte[] Trailer = new byte[]
            {
                0x00,
                0x00,
                0xFF,
                0xFF
            };

        public PerMessageCompression()
            : this(
                level: CompressionLevel.Default,
                clientNoContextTakeover: false,
                serverNoContextTakeover: false,
                desiredClientMaxWindowBits: ZlibConstants.WindowBitsMax,
                desiredServerMaxWindowBits: ZlibConstants.WindowBitsMax,
                minDataLengthToCompress: MinDataLengthToCompressDefault)
        {
        }

        public PerMessageCompression(CompressionLevel level,
            bool clientNoContextTakeover,
            bool serverNoContextTakeover,
            int desiredClientMaxWindowBits,
            int desiredServerMaxWindowBits,
            int minDataLengthToCompress)
        {
            this.Level = level;
            this.ClientNoContextTakeover = clientNoContextTakeover;
            this.ServerNoContextTakeover = serverNoContextTakeover;
            this.ClientMaxWindowBits = desiredClientMaxWindowBits;
            this.ServerMaxWindowBits = desiredServerMaxWindowBits;
            this.MinimumDataLengthToCompress = minDataLengthToCompress;
        }

        /// <summary>
        /// <code>
        /// 通过在扩展协商提议中包含此扩展参数，客户端通知对端服务器
        /// 提示即使服务器不包含"client_no_context_takeover"扩展参数
        /// 对应扩展协商响应offer，客户端不打算使用上下文接管。
        /// </code>
        /// </summary>
        private bool ClientNoContextTakeover { get; set; }

        /// <summary>
        /// 通过在扩展协商要约中包含此扩展参数，客户端可以防止对端服务器使用上下文接管。
        /// </summary>
        private bool ServerNoContextTakeover { get; set; }

        /// <summary>
        /// 该参数表示客户端上下文的LZ77滑动窗口大小的以2为底的对数。
        /// </summary>
        private int ClientMaxWindowBits { get; set; }

        /// <summary>
        /// 该参数表示服务器上下文的LZ77滑动窗口大小的以2为底的对数。
        /// </summary>
        private int ServerMaxWindowBits { get; set; }

        /// <summary>
        /// 客户端将用于压缩帧的压缩级别。
        /// </summary>
        private CompressionLevel Level { get; set; }

        /// <summary>
        /// 触发压缩的最小数据长度。
        /// </summary>
        private int MinimumDataLengthToCompress { get; set; }

        /// <summary>
        /// 缓存对象以支持上下文接管。
        /// </summary>
        private BufferPoolMemoryStream _compressorOutputStream;

        private DeflateStream _compressorDeflateStream;

        /// <summary>
        ///缓存对象以支持上下文接管。
        /// </summary>
        private BufferPoolMemoryStream _decompressorInputStream;

        private BufferPoolMemoryStream _decompressorOutputStream;
        private DeflateStream _decompressorDeflateStream;

        /// <summary>
        /// ReSharper disable once CommentTypo
        /// 这将启动permessage-deflate协商过程。
        /// <seealso href="http://tools.ietf.org/html/rfc7692#section-5.1"/>
        /// </summary>
        public void AddNegotiation(HttpRequest request)
        {
            // The default header value that we will send out minimum.
            var sb = new StringBuilder();
            // ReSharper disable once StringLiteralTypo
            sb.Append("permessage-deflate");
            /*
             * http://tools.ietf.org/html/rfc7692#section-7.1.1.1
             * 客户端可以在扩展协商要约中包含"server_no_context_takeover"扩展参数。
             * 此扩展参数没有值。
             * 通过在扩展协商要约中包含此扩展参数，客户端可以防止对端服务器使用上下文接管。
             * 如果对等服务器不使用上下文接管，客户端不需要预留内存来保留消息之间的LZ77滑动窗口。
             */
            if (this.ServerNoContextTakeover)
            {
                sb.Append("; server_no_context_takeover");
            }


            // http://tools.ietf.org/html/rfc7692#section-7.1.1.2
            /*
             * 客户端可以在扩展协商要约中包含"client_no_context_takeover"扩展参数。
             * 此扩展参数没有值。
             * 通过在扩展协商提议中包含这个扩展参数，客户端通知对等服务器一个提示，
             * 即使服务器在对该提议的相应扩展协商响应中不包含“client_no_context_takeover”扩展参数，
             * 客户端也不会使用上下文接管。
             */
            if (this.ClientNoContextTakeover)
            {
                sb.Append("; client_no_context_takeover");
            }

            // http://tools.ietf.org/html/rfc7692#section-7.1.2.1
            //通过在扩展协商报价中包含此参数，客户端限制了服务器端所提供的LZ77滑动窗口大小
            //将用于压缩消息。如果对端服务器使用LZ77小滑动窗口压缩消息，客户端可以减少LZ77滑动窗口所需的内存。
            if (this.ServerMaxWindowBits != ZlibConstants.WindowBitsMax)
            {
                sb.Append($"; server_max_window_bits={this.ServerMaxWindowBits}");
            }
            else
            {
                /*
                 * 在扩展协商报价中没有此参数表示客户端可以接收使用LZ77滑动窗口压缩的最多32,768字节的消息。
                 */
                this.ServerMaxWindowBits = ZlibConstants.WindowBitsMax;
            }

            // http://tools.ietf.org/html/rfc7692#section-7.1.2.2
            //通过在offer中包含此参数，客户端通知对端服务器客户端支持"client_max_window_bits"
            //扩展协商响应中的扩展参数，可选地，通过将值附加到参数的提示。
            if (this.ClientMaxWindowBits != ZlibConstants.WindowBitsMax)
            {
                sb.Append($"; client_max_window_bits= {this.ClientMaxWindowBits}");
            }
            else
            {
                sb.Append($"; client_max_window_bits");

                /*
                 * 如果扩展协商offer中的"client_max_window_bits"扩展参数有值，该参数也会通知
                 * 对端服务器的一个提示，即使服务器不包含“client_max_window_bits”扩展参数在相应的
                 * 扩展协商响应的值大于扩展协商offer中的值，或者如果服务器不包含
                 * 扩展参数，客户端不会使用大于指定大小的LZ77滑动窗口
                 * 通过扩展协商提供压缩消息的值。
                 */
                this.ClientMaxWindowBits = ZlibConstants.WindowBitsMax;
            }

            // 将新标头添加到请求。
            request.AddHeader("Sec-WebSocket-Extensions", sb.ToString());
        }

        public bool ParseNegotiation(WebSocketResponse resp)
        {
            // 搜索任何退回的新报价
            var headerValues = resp.GetHeaderValues("Sec-WebSocket-Extensions");
            if (headerValues == null)
            {
                return false;
            }

            foreach (var t1 in headerValues)
            {
                // If found, tokenize it
                var parser = new HeaderParser(t1);

                foreach (var t in parser.Values)
                {
                    if (string.IsNullOrEmpty(t.Key) ||
                        // ReSharper disable once StringLiteralTypo
                        !t.Key.StartsWith("permessage-deflate", StringComparison.OrdinalIgnoreCase)) continue;
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    var sb = new StringBuilder(3);
                    sb.Append($"Enabled with header:{t1}");
                    sb.Append($" Message: {resp.Message}");
                    Debug.Log($"[PerMessageCompression] [method:ParseNegotiation] [msg] {sb}");
#endif
                    if (t.TryGetOption("client_no_context_takeover", out var option))
                    {
                        this.ClientNoContextTakeover = true;
                    }

                    if (t.TryGetOption("server_no_context_takeover", out option))
                    {
                        this.ServerNoContextTakeover = true;
                    }

                    if (t.TryGetOption("client_max_window_bits", out option))
                    {
                        if (option.HasValue)
                        {
                            if (int.TryParse(option.Value, out var windowBits))
                            {
                                this.ClientMaxWindowBits = windowBits;
                            }
                        }
                    }

                    if (!t.TryGetOption("server_max_window_bits", out option)) return true;
                    {
                        if (!option.HasValue) return true;
                        if (int.TryParse(option.Value, out var windowBits))
                        {
                            this.ServerMaxWindowBits = windowBits;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 如果我们想要压缩写入器中的数据，IExtension实现可以在头中设置Rsv1标志。
        /// </summary>
        public byte GetFrameHeader(WebSocketFrame writer, byte inFlag)
        {
            // http://tools.ietf.org/html/rfc7692#section-7.2.3.1
            //  RSV1位仅在第一帧上设置。
            if (writer.Type is WebSocketFrameTypes.Binary or WebSocketFrameTypes.Text &&
                writer.Data != null &&
                writer.DataLength >= this.MinimumDataLengthToCompress)
            {
                return (byte)(inFlag | 0x40);
            }
            else
            {
                return inFlag;
            }
        }

        /// <summary>
        /// IExtension实现能够压缩写入器中的数据。
        /// </summary>
        public byte[] Encode(WebSocketFrame writer)
        {
            if (writer.Data == null)
            {
                return BufferPool.NoData;
            }

            // 该帧是否启用压缩?如果是，压缩它。
            return (writer.Header & 0x40) != 0
                ? Compress(
                    data: writer.Data,
                    length: writer.DataLength)
                : writer.Data;
        }

        /// <summary>
        /// IExtension实现来可能地解压数据。
        /// </summary>
        public byte[] Decode(byte header, byte[] data, int length)
        {
            // 服务器是否对数据进行了压缩?如果存在，请解压。
            return (header & 0x40) != 0
                ? Decompress(
                    data: data,
                    length: length)
                : data;
        }

        /// <summary>
        /// 一个压缩并返回数据参数的函数，可能支持上下文接管(重用DeflateStream)。
        /// </summary>
        private
            byte[] Compress(byte[] data, int length)
        {
            _compressorOutputStream ??= new BufferPoolMemoryStream();
            _compressorOutputStream.SetLength(0);

            if (_compressorDeflateStream == null)
            {
                _compressorDeflateStream = new DeflateStream(
                    stream: _compressorOutputStream,
                    mode: CompressionMode.Compress,
                    level: this.Level,
                    leaveOpen: true,
                    windowBits: this.ClientMaxWindowBits);
                _compressorDeflateStream.FlushMode = FlushType.Sync;
            }

            byte[] result;
            try
            {
                _compressorDeflateStream.Write(data, 0, length);
                _compressorDeflateStream.Flush();

                _compressorOutputStream.Position = 0;

                // http://tools.ietf.org/html/rfc7692#section-7.2.1
                // 从尾部删除4个字节(即0x00 0x00 0xff 0xff)。在这一步之后，压缩数据的最后八位包含(可能是部分)DEFLATE头位，“byte”位设置为00。
                _compressorOutputStream.SetLength(_compressorOutputStream.Length - 4);

                result = _compressorOutputStream.ToArray();
            }
            finally
            {
                if (this.ClientNoContextTakeover)
                {
                    _compressorDeflateStream.Dispose();
                    _compressorDeflateStream = null;
                }
            }

            return result;
        }

        /// <summary>
        /// 一个解压缩并返回数据参数的函数，可能支持上下文接管(重用DeflateStream)。
        /// </summary>
        private byte[] Decompress(byte[] data, int length)
        {
            _decompressorInputStream ??= new BufferPoolMemoryStream(length + 4);

            _decompressorInputStream.Write(
                buffer: data,
                offset: 0,
                count: length);

            // http://tools.ietf.org/html/rfc7692#section-7.2.2
            // 在消息有效负载的尾部追加4个字节的0x00 0x00 0xff 0xff。
            _decompressorInputStream.Write(
                buffer: PerMessageCompression.Trailer,
                offset: 0,
                count: PerMessageCompression.Trailer.Length);

            _decompressorInputStream.Position = 0;

            if (_decompressorDeflateStream == null)
            {
                _decompressorDeflateStream = new DeflateStream(
                    stream: _decompressorInputStream,
                    mode: CompressionMode.Decompress,
                    level: CompressionLevel.Default,
                    leaveOpen: true,
                    windowBits: this.ServerMaxWindowBits);
                _decompressorDeflateStream.FlushMode = FlushType.Sync;
            }

            _decompressorOutputStream ??= new BufferPoolMemoryStream();

            _decompressorOutputStream.SetLength(0);

            byte[] copyBuffer = BufferPool.Get(1024, true);
            int readCount;
            while ((readCount =
                       _decompressorDeflateStream.Read(
                           buffer: copyBuffer,
                           offset: 0,
                           count: copyBuffer.Length)) != 0)
            {
                _decompressorOutputStream.Write(
                    buffer: copyBuffer,
                    offset: 0,
                    count: readCount);
            }

            BufferPool.Release(copyBuffer);

            _decompressorDeflateStream.SetLength(0);

            var result = _decompressorOutputStream.ToArray();

            if (!this.ServerNoContextTakeover) return result;
            _decompressorDeflateStream.Dispose();
            _decompressorDeflateStream = null;

            return result;
        }
    }
}

#endif
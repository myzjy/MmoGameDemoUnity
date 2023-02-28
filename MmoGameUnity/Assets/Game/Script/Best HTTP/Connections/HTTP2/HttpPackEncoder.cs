#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.Memory;

// ReSharper disable once CheckNamespace
namespace BestHTTP.Connections.HTTP2
{
    public sealed class HttpPackEncoder
    {
        private readonly Http2Handler _parent;

        // https://http2.github.io/http2-spec/compression.html#encoding.context
        // 当用于双向通信时，例如在HTTP中，端点维护的编码和解码动态表是完全独立的，也就是说，请求和响应动态表是分开的。
        private readonly HeaderTable _requestTable;
        private readonly HeaderTable _responseTable;
        private readonly Http2SettingsManager _settingsRegistry;

        public HttpPackEncoder(Http2Handler parentHandler, Http2SettingsManager registry)
        {
            this._parent = parentHandler;
            this._settingsRegistry = registry;

            // 我不确定我们应该为这两个表使用什么设置(本地或远程)!
            this._requestTable = new HeaderTable(this._settingsRegistry.MySettings);
            this._responseTable = new HeaderTable(this._settingsRegistry.RemoteSettings);
        }

        public void Encode(HTTP2Stream context, HttpRequest request, Queue<Http2FrameHeaderAndPayload> to,
            uint streamId)
        {
            // 添加SETTINGS_MAX_HEADER_LIST_SIZE的使用，以便能够创建一个头和一个或多个延续片段
            // (https://httpwg.org/specs/rfc7540.html#SettingValues)

            using var bufferStream = new BufferPoolMemoryStream();
            WriteHeader(bufferStream, ":method", HttpRequest.MethodNames[(int)request.MethodType]);
            // add path
            WriteHeader(bufferStream, ":path", request.CurrentUri.PathAndQuery);
            // add authority
            WriteHeader(bufferStream, ":authority", request.CurrentUri.Authority);
            // add scheme
            WriteHeader(bufferStream, ":scheme", "https");

            //bool hasBody = false;

            // add other, regular headers
            request.EnumerateHeaders((header, values) =>
            {
                if (header.Equals("connection", StringComparison.OrdinalIgnoreCase) ||
                    header.Equals("te", StringComparison.OrdinalIgnoreCase) ||
                    header.Equals("host", StringComparison.OrdinalIgnoreCase) ||
                    header.Equals("keep-alive", StringComparison.OrdinalIgnoreCase) ||
                    header.StartsWith("proxy-", StringComparison.OrdinalIgnoreCase))
                    return;

                //if (!hasBody)
                //    hasBody = header.Equals("content-length", StringComparison.OrdinalIgnoreCase) && int.Parse(values[0]) > 0;

                // https://httpwg.org/specs/rfc7540.html#HttpSequence
                // 在[RFC7230]Section 4.1中定义的分块传输编码绝对不能用于HTTP/2。
                if (header.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
                {
                    // error!
                    return;
                }

                // https://httpwg.org/specs/rfc7540.html#HttpHeaders
                // 就像在HTTP/1。x，报头字段名是ASCII字符字符串，以不区分大小写的方式进行比较。
                // 但是，在HTTP/2中，报头字段名必须在编码之前转换为小写。 
                //包含大写报头字段名的请求或响应必须被视为畸形
                if (header.Any(char.IsUpper))
                {
                    header = header.ToLower();
                }

                for (var i = 0; i < values.Count; ++i)
                {
                    // ReSharper disable once AccessToDisposedClosure
                    WriteHeader(bufferStream, header, values[i]);

#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    StringBuilder sb = new StringBuilder(3);
                    sb.Append($"[{context.Id}] ");
                    sb.Append("- Encode - Header");
                    sb.Append($"({i + 1}/{values.Count}):");
                    sb.Append($" '{header}': '{values[i]}'");
                    Debug.Log($"[HPACKEncoder] [method: Encode] [msg|Exception]{sb.ToString()}");
#endif
                }
            }, true);

            var upStreamInfo = request.GetUpStream();
            CreateHeaderFrames(to,
                streamId,
                bufferStream.ToArray(true),
                (uint)bufferStream.Length,
                upStreamInfo.Stream != null);
        }

        public void Decode(HTTP2Stream context, Stream stream, List<KeyValuePair<string, string>> to)
        {
            var headerType = stream.ReadByte();
            while (headerType != -1)
            {
                var firstDataByte = (byte)headerType;

                // https://http2.github.io/http2-spec/compression.html#indexed.header.representation
                if (BufferHelper.ReadBit(firstDataByte, 0) == 1)
                {
                    var header = ReadIndexedHeader(firstDataByte, stream);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    StringBuilder sb = new StringBuilder(3);
                    sb.Append($"[{context.Id}] ");
                    sb.Append(" Decode - IndexedHeader：");
                    sb.Append($" {header.ToString()}");
                    Debug.Log($"[HPACKEncoder] [method: Encode] [msg|Exception]{sb.ToString()}");
#endif
                    to.Add(header);
                }
                else if (BufferHelper.ReadValue(firstDataByte, 0, 1) == 1)
                {
                    // https://http2.github.io/http2-spec/compression.html#literal.header.with.incremental.indexing

                    if (BufferHelper.ReadValue(firstDataByte, 2, 7) == 0)
                    {
                        // 带增量索引的文字头字段-新名称
                        var header = ReadLiteralHeaderFieldWithIncrementalIndexing_NewName(stream);


#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        StringBuilder sb = new StringBuilder(3);
                        sb.Append($"[{context.Id}] ");
                        sb.Append("Decode - LiteralHeaderFieldWithIncrementalIndexing_NewName:");
                        sb.Append($"{header.ToString()}");
                        // ReSharper disable once StringLiteralTypo
                        Debug.Log($"[HPACKEncoder] [method:Decode] [msg] {sb.ToString()}");
#endif
                        this._responseTable.Add(header);
                        to.Add(header);
                    }
                    else
                    {
                        // 字面值报头字段与增量索引-索引名称
                        var header = ReadLiteralHeaderFieldWithIncrementalIndexing_IndexedName(firstDataByte, stream);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        // ReSharper disable once StringLiteralTypo
                        Debug.Log(
                            $"[HPACKEncoder] [method:Decode] [msg] [{context.Id}] Decode - LiteralHeaderFieldWithIncrementalIndexing_IndexedName: {header.ToString()}");
#endif
                        this._responseTable.Add(header);
                        to.Add(header);
                    }
                }
                else if (BufferHelper.ReadValue(firstDataByte, 0, 3) == 0)
                {
                    // https://http2.github.io/http2-spec/compression.html#literal.header.without.indexing

                    if (BufferHelper.ReadValue(firstDataByte, 4, 7) == 0)
                    {
                        // 没有索引的文字头字段-新名称
                        var header = ReadLiteralHeaderFieldwithoutIndexing_NewName(stream);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        // ReSharper disable once StringLiteralTypo
                        Debug.Log(
                            $"[HPACKEncoder] [method:Decode] [msg] [{context.Id}] Decode - LiteralHeaderFieldwithoutIndexing_NewName: {header.ToString()}");
#endif
                        to.Add(header);
                    }
                    else
                    {
                        // Literal Header Field without Indexing — Indexed Name
                        var header = ReadLiteralHeaderFieldwithoutIndexing_IndexedName(firstDataByte, stream);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        // ReSharper disable once StringLiteralTypo
                        Debug.Log(
                            $"[HPACKEncoder] [method:Decode] [msg] [{context.Id}] Decode - LiteralHeaderFieldwithoutIndexing_IndexedName: {header.ToString()}");
#endif
                        to.Add(header);
                    }
                }
                else if (BufferHelper.ReadValue(firstDataByte, 0, 3) == 1)
                {
                    // https://http2.github.io/http2-spec/compression.html#literal.header.never.indexed

                    if (BufferHelper.ReadValue(firstDataByte, 4, 7) == 0)
                    {
                        // Literal Header Field Never Indexed — New Name
                        var header = ReadLiteralHeaderFieldNeverIndexed_NewName(stream);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        // ReSharper disable once StringLiteralTypo
                        Debug.Log(
                            $"[HPACKEncoder] [method:Decode] [msg] [{context.Id}] Decode - LiteralHeaderFieldNeverIndexed_NewName: {header.ToString()}");
#endif
                        to.Add(header);
                    }
                    else
                    {
                        // 字面头字段从未被索引-被索引的名称
                        var header = ReadLiteralHeaderFieldNeverIndexed_IndexedName(firstDataByte, stream);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        // ReSharper disable once StringLiteralTypo
                        Debug.Log(
                            $"[HPACKEncoder] [method:Decode] [msg] [{context.Id}] Decode - LiteralHeaderFieldNeverIndexed_IndexedName: {header.ToString()}");
#endif
                        to.Add(header);
                    }
                }
                else if (BufferHelper.ReadValue(firstDataByte, 0, 2) == 1)
                {
                    // https://http2.github.io/http2-spec/compression.html#encoding.context.update

                    UInt32 newMaxSize = DecodeInteger(5, firstDataByte, stream);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    // ReSharper disable once StringLiteralTypo
                    Debug.Log(
                        $"[HPACKEncoder] [method:Decode] [msg] [{context.Id}] Decode - Dynamic Table Size Update: {newMaxSize}");
#endif
                    //this.settingsRegistry[HTTP2Settings.HEADER_TABLE_SIZE] = (UInt16)newMaxSize;
                    this._responseTable.MaxDynamicTableSize = (UInt16)newMaxSize;
                }
                else
                {
                    // ERROR
                }

                headerType = stream.ReadByte();
            }
        }

        private KeyValuePair<string, string> ReadIndexedHeader(byte firstByte, Stream stream)
        {
            // https://http2.github.io/http2-spec/compression.html#indexed.header.representation

            UInt32 index = DecodeInteger(7, firstByte, stream);
            return this._responseTable.GetHeader(index);
        }

        private KeyValuePair<string, string> ReadLiteralHeaderFieldWithIncrementalIndexing_IndexedName(byte firstByte,
            Stream stream)
        {
            // https://http2.github.io/http2-spec/compression.html#literal.header.with.incremental.indexing

            UInt32 keyIndex = DecodeInteger(6, firstByte, stream);

            string header = this._responseTable.GetKey(keyIndex);
            string value = DecodeString(stream);

            return new KeyValuePair<string, string>(header, value);
        }

        private KeyValuePair<string, string> ReadLiteralHeaderFieldWithIncrementalIndexing_NewName(Stream stream)
        {
            // https://http2.github.io/http2-spec/compression.html#literal.header.with.incremental.indexing

            string header = DecodeString(stream);
            string value = DecodeString(stream);

            return new KeyValuePair<string, string>(header, value);
        }

        // ReSharper disable once IdentifierTypo
        private KeyValuePair<string, string> ReadLiteralHeaderFieldwithoutIndexing_IndexedName(byte firstByte,
            Stream stream)
        {
            // https://http2.github.io/http2-spec/compression.html#literal.header.without.indexing

            UInt32 index = DecodeInteger(4, firstByte, stream);
            string header = this._responseTable.GetKey(index);
            string value = DecodeString(stream);

            return new KeyValuePair<string, string>(header, value);
        }

        // ReSharper disable once IdentifierTypo
        private KeyValuePair<string, string> ReadLiteralHeaderFieldwithoutIndexing_NewName(Stream stream)
        {
            // https://http2.github.io/http2-spec/compression.html#literal.header.without.indexing

            string header = DecodeString(stream);
            string value = DecodeString(stream);

            return new KeyValuePair<string, string>(header, value);
        }

        private KeyValuePair<string, string> ReadLiteralHeaderFieldNeverIndexed_IndexedName(byte firstByte,
            Stream stream)
        {
            // https://http2.github.io/http2-spec/compression.html#literal.header.never.indexed

            UInt32 index = DecodeInteger(4, firstByte, stream);
            string header = this._responseTable.GetKey(index);
            string value = DecodeString(stream);

            return new KeyValuePair<string, string>(header, value);
        }

        private KeyValuePair<string, string> ReadLiteralHeaderFieldNeverIndexed_NewName(Stream stream)
        {
            // https://http2.github.io/http2-spec/compression.html#literal.header.never.indexed

            string header = DecodeString(stream);
            string value = DecodeString(stream);

            return new KeyValuePair<string, string>(header, value);
        }

        private string DecodeString(Stream stream)
        {
            byte start = (byte)stream.ReadByte();
            bool rawString = BufferHelper.ReadBit(start, 0) == 0;
            UInt32 stringLength = DecodeInteger(7, start, stream);

            if (stringLength == 0)
                return string.Empty;

            if (rawString)
            {
                byte[] buffer = BufferPool.Get(stringLength, true);

                // ReSharper disable once MustUseReturnValue
                stream.Read(buffer, 0, (int)stringLength);

                BufferPool.Release(buffer);

                return System.Text.Encoding.UTF8.GetString(buffer, 0, (int)stringLength);
            }
            else
            {
                var node = HuffmanEncoder.GetRoot();
                byte currentByte = (byte)stream.ReadByte();
                byte bitIdx = 0; // 0..7

                using BufferPoolMemoryStream bufferStream = new BufferPoolMemoryStream();
                do
                {
                    byte bitValue = BufferHelper.ReadBit(currentByte, bitIdx);

                    if (++bitIdx > 7)
                    {
                        stringLength--;

                        if (stringLength > 0)
                        {
                            bitIdx = 0;
                            currentByte = (byte)stream.ReadByte();
                        }
                    }

                    node = HuffmanEncoder.GetNext(node, bitValue);

                    if (node.Value != 0)
                    {
                        if (node.Value != HuffmanEncoder.EOS)
                            bufferStream.WriteByte((byte)node.Value);

                        node = HuffmanEncoder.GetRoot();
                    }
                } while (stringLength > 0);

                byte[] buffer = bufferStream.ToArray(true);

                string result = System.Text.Encoding.UTF8.GetString(buffer, 0, (int)bufferStream.Length);

                BufferPool.Release(buffer);

                return result;
            }
        }

        private void CreateHeaderFrames(Queue<Http2FrameHeaderAndPayload> to, UInt32 streamId, byte[] dataToSend,
            UInt32 payloadLength, bool hasBody)
        {
            UInt32 maxFrameSize = this._settingsRegistry.RemoteSettings[Http2Settings.MaxFrameSize];

            // Only one headers frame
            if (payloadLength <= maxFrameSize)
            {
                var frameHeader = new Http2FrameHeaderAndPayload
                {
                    Type = Http2FrameTypes.Headers,
                    StreamId = streamId,
                    Flags = (byte)(Http2HeadersFlags.EndHeaders)
                };

                if (!hasBody)
                    frameHeader.Flags |= (byte)(Http2HeadersFlags.EndStream);

                frameHeader.PayloadLength = payloadLength;
                frameHeader.Payload = dataToSend;

                to.Enqueue(frameHeader);
            }
            else
            {
                var frameHeader = new Http2FrameHeaderAndPayload
                {
                    Type = Http2FrameTypes.Headers,
                    StreamId = streamId,
                    PayloadLength = maxFrameSize,
                    Payload = dataToSend,
                    DontUseMemPool = true,
                    PayloadOffset = 0
                };

                if (!hasBody)
                    frameHeader.Flags = (byte)(Http2HeadersFlags.EndStream);

                to.Enqueue(frameHeader);

                UInt32 offset = maxFrameSize;
                while (offset < payloadLength)
                {
                    frameHeader = new Http2FrameHeaderAndPayload
                    {
                        Type = Http2FrameTypes.Continuation,
                        StreamId = streamId,
                        PayloadLength = maxFrameSize,
                        Payload = dataToSend,
                        PayloadOffset = offset
                    };

                    offset += maxFrameSize;

                    if (offset >= payloadLength)
                    {
                        frameHeader.Flags = (byte)(Http2ContinuationFlags.EndHeaders);
                        // last sent continuation fragment will release back the payload buffer
                        frameHeader.DontUseMemPool = false;
                    }
                    else
                        frameHeader.DontUseMemPool = true;

                    to.Enqueue(frameHeader);
                }
            }
        }

        private void WriteHeader(Stream stream, string header, string value)
        {
            // https://http2.github.io/http2-spec/compression.html#header.representation

            KeyValuePair<UInt32, UInt32> index = this._requestTable.GetIndex(header, value);

            if (index.Key == 0 && index.Value == 0)
            {
                WriteLiteralHeaderFieldWithIncrementalIndexing_NewName(stream, header, value);
                this._requestTable.Add(new KeyValuePair<string, string>(header, value));
            }
            else if (index.Key != 0 && index.Value == 0)
            {
                WriteLiteralHeaderFieldWithIncrementalIndexing_IndexedName(stream, index.Key, value);
                this._requestTable.Add(new KeyValuePair<string, string>(header, value));
            }
            else
            {
                WriteIndexedHeaderField(stream, index.Key);
            }
        }

        private static void WriteIndexedHeaderField(Stream stream, UInt32 index)
        {
            byte requiredBytes = RequiredBytesToEncodeInteger(index, 7);
            byte[] buffer = BufferPool.Get(requiredBytes, true);
            UInt32 offset = 0;

            buffer[0] = 0x80;
            EncodeInteger(index, 7, buffer, ref offset);

            stream.Write(buffer, 0, (int)offset);

            BufferPool.Release(buffer);
        }

        private static void WriteLiteralHeaderFieldWithIncrementalIndexing_IndexedName(Stream stream, UInt32 index,
            string value)
        {
            // https://http2.github.io/http2-spec/compression.html#literal.header.with.incremental.indexing

            UInt32 requiredBytes = RequiredBytesToEncodeInteger(index, 6) +
                                   RequiredBytesToEncodeString(value);

            byte[] buffer = BufferPool.Get(requiredBytes, true);
            UInt32 offset = 0;

            buffer[0] = 0x40;
            EncodeInteger(index, 6, buffer, ref offset);
            EncodeString(value, buffer, ref offset);

            stream.Write(buffer, 0, (int)offset);

            BufferPool.Release(buffer);
        }

        private static void WriteLiteralHeaderFieldWithIncrementalIndexing_NewName(Stream stream, string header,
            string value)
        {
            // https://http2.github.io/http2-spec/compression.html#literal.header.with.incremental.indexing

            UInt32 requiredBytes = 1 + RequiredBytesToEncodeString(header) + RequiredBytesToEncodeString(value);

            byte[] buffer = BufferPool.Get(requiredBytes, true);
            UInt32 offset = 0;

            buffer[offset++] = 0x40;
            EncodeString(header, buffer, ref offset);
            EncodeString(value, buffer, ref offset);

            stream.Write(buffer, 0, (int)offset);

            BufferPool.Release(buffer);
        }

        // ReSharper disable once UnusedMember.Local
        private static void WriteLiteralHeaderFieldWithoutIndexing_IndexedName(Stream stream, UInt32 index,
            string value)
        {
            // https://http2.github.io/http2-spec/compression.html#literal.header.without.indexing

            UInt32 requiredBytes = RequiredBytesToEncodeInteger(index, 4) + RequiredBytesToEncodeString(value);

            byte[] buffer = BufferPool.Get(requiredBytes, true);
            UInt32 offset = 0;

            buffer[0] = 0;
            EncodeInteger(index, 4, buffer, ref offset);
            EncodeString(value, buffer, ref offset);

            stream.Write(buffer, 0, (int)offset);

            BufferPool.Release(buffer);
        }

        // ReSharper disable once UnusedMember.Local
        private static void WriteLiteralHeaderFieldWithoutIndexing_NewName(Stream stream, string header, string value)
        {
            // https://http2.github.io/http2-spec/compression.html#literal.header.without.indexing

            UInt32 requiredBytes = 1 + RequiredBytesToEncodeString(header) + RequiredBytesToEncodeString(value);

            byte[] buffer = BufferPool.Get(requiredBytes, true);
            UInt32 offset = 0;

            buffer[offset++] = 0;
            EncodeString(header, buffer, ref offset);
            EncodeString(value, buffer, ref offset);

            stream.Write(buffer, 0, (int)offset);

            BufferPool.Release(buffer);
        }

        // ReSharper disable once UnusedMember.Local
        private static void WriteLiteralHeaderFieldNeverIndexed_IndexedName(Stream stream, UInt32 index, string value)
        {
            // https://http2.github.io/http2-spec/compression.html#literal.header.never.indexed

            UInt32 requiredBytes = RequiredBytesToEncodeInteger(index, 4) + RequiredBytesToEncodeString(value);

            byte[] buffer = BufferPool.Get(requiredBytes, true);
            UInt32 offset = 0;

            buffer[0] = 0x10;
            EncodeInteger(index, 4, buffer, ref offset);
            EncodeString(value, buffer, ref offset);

            stream.Write(buffer, 0, (int)offset);

            BufferPool.Release(buffer);
        }

        // ReSharper disable once UnusedMember.Local
        private static void WriteLiteralHeaderFieldNeverIndexed_NewName(Stream stream, string header, string value)
        {
            // https://http2.github.io/http2-spec/compression.html#literal.header.never.indexed

            UInt32 requiredBytes = 1 + RequiredBytesToEncodeString(header) + RequiredBytesToEncodeString(value);

            byte[] buffer = BufferPool.Get(requiredBytes, true);
            UInt32 offset = 0;

            buffer[offset++] = 0x10;
            EncodeString(header, buffer, ref offset);
            EncodeString(value, buffer, ref offset);

            stream.Write(buffer, 0, (int)offset);

            BufferPool.Release(buffer);
        }

        // ReSharper disable once UnusedMember.Local
        private static void WriteDynamicTableSizeUpdate(Stream stream, UInt16 maxSize)
        {
            // https://http2.github.io/http2-spec/compression.html#encoding.context.update

            UInt32 requiredBytes = RequiredBytesToEncodeInteger(maxSize, 5);

            byte[] buffer = BufferPool.Get(requiredBytes, true);
            UInt32 offset = 0;

            buffer[offset] = 0x20;
            EncodeInteger(maxSize, 5, buffer, ref offset);

            stream.Write(buffer, 0, (int)offset);

            BufferPool.Release(buffer);
        }

        private static UInt32 RequiredBytesToEncodeString(string str)
        {
            uint requiredBytesForRawStr = RequiredBytesToEncodeRawString(str);
            uint requiredBytesForHuffman = RequiredBytesToEncodeStringWithHuffman(str);
            requiredBytesForHuffman += RequiredBytesToEncodeInteger(requiredBytesForHuffman, 7);

            return Math.Min(requiredBytesForRawStr, requiredBytesForHuffman);
        }

        private static void EncodeString(string str, byte[] buffer, ref UInt32 offset)
        {
            uint requiredBytesForRawStr = RequiredBytesToEncodeRawString(str);
            uint requiredBytesForHuffman = RequiredBytesToEncodeStringWithHuffman(str);

            // 如果使用霍夫曼编码可以产生相同的长度，我们选择原始编码，因为它需要更少的CPU周期
            if (requiredBytesForRawStr <=
                (requiredBytesForHuffman + RequiredBytesToEncodeInteger(requiredBytesForHuffman, 7)))
            {
                EncodeRawStringTo(str, buffer, ref offset);
            }
            else
            {
                EncodeStringWithHuffman(str, requiredBytesForHuffman, buffer, ref offset);
            }
        }

        // 这个函数只计算压缩字符串的长度，额外的头长度必须使用这个函数返回的值来计算
        private static uint RequiredBytesToEncodeStringWithHuffman(string str)
        {
            int requiredBytesForStr = System.Text.Encoding.UTF8.GetByteCount(str);
            byte[] strBytes = BufferPool.Get(requiredBytesForStr, true);

            System.Text.Encoding.UTF8.GetBytes(str, 0, str.Length, strBytes, 0);

            uint requiredBits = 0;

            for (int i = 0; i < requiredBytesForStr; ++i)
            {
                requiredBits += HuffmanEncoder.GetEntryForCodePoint(strBytes[i]).Bits;
            }

            BufferPool.Release(strBytes);

            return (uint)((requiredBits / 8) + ((requiredBits % 8) == 0 ? 0 : 1));
        }

        private static void EncodeStringWithHuffman(string str, UInt32 encodedLength, byte[] buffer, ref UInt32 offset)
        {
            int requiredBytesForStr = System.Text.Encoding.UTF8.GetByteCount(str);
            byte[] strBytes = BufferPool.Get(requiredBytesForStr, true);

            System.Text.Encoding.UTF8.GetBytes(str, 0, str.Length, strBytes, 0);

            // 0. bit: huffman flag
            buffer[offset] = 0x80;

            // 1..7+ bit: length
            EncodeInteger(encodedLength, 7, buffer, ref offset);

            byte bufferBitIdx = 0;

            for (int i = 0; i < requiredBytesForStr; ++i)
                AddCodePointToBuffer(HuffmanEncoder.GetEntryForCodePoint(strBytes[i]), buffer, ref offset,
                    ref bufferBitIdx);

            // https://http2.github.io/http2-spec/compression.html#string.literal.representation
            // As the Huffman-encoded data doesn't always end at an octet boundary, some padding is inserted after it,
            // up to the next octet boundary. To prevent this padding from being misinterpreted as part of the string literal,
            // the most significant bits of the code corresponding to the EOS (end-of-string) symbol are used.
            if (bufferBitIdx != 0)
                AddCodePointToBuffer(HuffmanEncoder.GetEntryForCodePoint(256), buffer, ref offset, ref bufferBitIdx,
                    true);

            BufferPool.Release(strBytes);
        }

        private static void AddCodePointToBuffer(HuffmanEncoder.TableEntry code, byte[] buffer, ref UInt32 offset,
            ref byte bufferBitIdx, bool finishOnBoundary = false)
        {
            for (byte codeBitIdx = 1; codeBitIdx <= code.Bits; ++codeBitIdx)
            {
                byte bit = code.GetBitAtIdx(codeBitIdx);
                buffer[offset] = BufferHelper.SetBit(buffer[offset], bufferBitIdx, bit);

                // octet boundary reached, proceed to the next octet
                if (++bufferBitIdx == 8)
                {
                    if (++offset < buffer.Length)
                        buffer[offset] = 0;

                    if (finishOnBoundary)
                        return;

                    bufferBitIdx = 0;
                }
            }
        }

        private static UInt32 RequiredBytesToEncodeRawString(string str)
        {
            int requiredBytesForStr = System.Text.Encoding.UTF8.GetByteCount(str);
            int requiredBytesForLengthPrefix = RequiredBytesToEncodeInteger((UInt32)requiredBytesForStr, 7);

            return (UInt32)(requiredBytesForStr + requiredBytesForLengthPrefix);
        }

        // This method encodes a string without huffman encoding
        private static void EncodeRawStringTo(string str, byte[] buffer, ref UInt32 offset)
        {
            uint requiredBytesForStr = (uint)System.Text.Encoding.UTF8.GetByteCount(str);
            int requiredBytesForLengthPrefix = RequiredBytesToEncodeInteger(requiredBytesForStr, 7);

            UInt32 originalOffset = offset;
            buffer[offset] = 0;
            EncodeInteger(requiredBytesForStr, 7, buffer, ref offset);

            // Zero out the huffman flag
            buffer[originalOffset] = BufferHelper.SetBit(buffer[originalOffset], 0, false);

            if (offset != originalOffset + requiredBytesForLengthPrefix)
                throw new Exception(string.Format(
                    "offset({0}) != originalOffset({1}) + requiredBytesForLengthPrefix({1})", offset, originalOffset));

            System.Text.Encoding.UTF8.GetBytes(str, 0, str.Length, buffer, (int)offset);
            offset += requiredBytesForStr;
        }

        private static byte RequiredBytesToEncodeInteger(UInt32 value, byte n)
        {
            UInt32 maxValue = (1u << n) - 1;
            byte count = 0;

            // If the integer value is small enough, i.e., strictly less than 2^N-1, it is encoded within the N-bit prefix.
            if (value < maxValue)
            {
                count++;
            }
            else
            {
                // Otherwise, all the bits of the prefix are set to 1, and the value, decreased by 2^N-1
                count++;
                value -= maxValue;

                while (value >= 0x80)
                {
                    // The most significant bit of each octet is used as a continuation flag: its value is set to 1 except for the last octet in the list.
                    count++;
                    value = value / 0x80;
                }

                count++;
            }

            return count;
        }

        // https://http2.github.io/http2-spec/compression.html#integer.representation
        private static void EncodeInteger(UInt32 value, byte n, byte[] buffer, ref UInt32 offset)
        {
            // 2^N - 1
            UInt32 maxValue = (1u << n) - 1;

            // If the integer value is small enough, i.e., strictly less than 2^N-1, it is encoded within the N-bit prefix.
            if (value < maxValue)
            {
                buffer[offset++] |= (byte)value;
            }
            else
            {
                // Otherwise, all the bits of the prefix are set to 1, and the value, decreased by 2^N-1
                buffer[offset++] |= (byte)(0xFF >> (8 - n));
                value -= maxValue;

                while (value >= 0x80)
                {
                    // The most significant bit of each octet is used as a continuation flag: its value is set to 1 except for the last octet in the list.
                    buffer[offset++] = (byte)(0x80 | (0x7F & value));
                    value = value / 0x80;
                }

                buffer[offset++] = (byte)value;
            }
        }

        // https://http2.github.io/http2-spec/compression.html#integer.representation
        // ReSharper disable once UnusedMember.Local
        private static UInt32 DecodeInteger(byte n, byte[] buffer, ref UInt32 offset)
        {
            // The starting value is the value behind the mask of the N bits
            UInt32 value = (UInt32)(buffer[offset++] & (byte)(0xFF >> (8 - n)));

            // All N bits are 1s ? If so, we have at least one another byte to decode
            if (value == (1u << n) - 1)
            {
                byte shift = 0;

                do
                {
                    // The most significant bit is a continuation flag, so we have to mask it out
                    value += (UInt32)((buffer[offset] & 0x7F) << shift);
                    shift += 7;
                } while ((buffer[offset++] & 0x80) == 0x80);
            }

            return value;
        }

        // https://http2.github.io/http2-spec/compression.html#integer.representation
        private static UInt32 DecodeInteger(byte n, byte data, Stream stream)
        {
            // The starting value is the value behind the mask of the N bits
            UInt32 value = (UInt32)(data & (byte)(0xFF >> (8 - n)));

            // All N bits are 1s ? If so, we have at least one another byte to decode
            if (value == (1u << n) - 1)
            {
                byte shift = 0;

                do
                {
                    data = (byte)stream.ReadByte();

                    // The most significant bit is a continuation flag, so we have to mask it out
                    value += (UInt32)((data & 0x7F) << shift);
                    shift += 7;
                } while ((data & 0x80) == 0x80);
            }

            return value;
        }

        public override string ToString()
        {
            return this._requestTable + this._responseTable.ToString();
        }
    }
}

#endif
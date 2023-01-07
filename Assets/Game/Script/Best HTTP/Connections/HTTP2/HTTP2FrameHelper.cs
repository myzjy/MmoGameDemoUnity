#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2

using System;
using System.Collections.Generic;
using System.IO;
using BestHTTP.PlatformSupport.Memory;

namespace BestHTTP.Connections.HTTP2
{
    // https://httpwg.org/specs/rfc7540.html#ErrorCodes
    public enum Http2ErrorCodes
    {
        NoError = 0x00,
        ProtocolError = 0x01,
        InternalError = 0x02,
        FlowControlError = 0x03,
        SettingsTimeout = 0x04,
        StreamClosed = 0x05,
        FrameSizeError = 0x06,
        RefusedStream = 0x07,
        Cancel = 0x08,
        CompressionError = 0x09,
        ConnectError = 0x0A,
        EnhanceYourCalm = 0x0B,
        InadequateSecurity = 0x0C,
        Http11Required = 0x0D
    }

    public static class Http2FrameHelper
    {
        public static Http2ContinuationFrame ReadContinuationFrame(Http2FrameHeaderAndPayload header)
        {
            // https://httpwg.org/specs/rfc7540.html#CONTINUATION

            var frame = new Http2ContinuationFrame(header)
            {
                HeaderBlockFragment = header.Payload
            };

            header.Payload = null;

            return frame;
        }

        public static Http2WindowUpdateFrame ReadWindowUpdateFrame(Http2FrameHeaderAndPayload header)
        {
            // https://httpwg.org/specs/rfc7540.html#WINDOW_UPDATE

            var frame = new Http2WindowUpdateFrame(header)
            {
                ReservedBit = BufferHelper.ReadBit(header.Payload[0], 0),
                WindowSizeIncrement = BufferHelper.ReadUInt31(header.Payload, 0)
            };

            return frame;
        }

        public static Http2GoAwayFrame ReadGoAwayFrame(Http2FrameHeaderAndPayload header)
        {
            // https://httpwg.org/specs/rfc7540.html#GOAWAY
            //      str id      error
            // | 0, 1, 2, 3 | 4, 5, 6, 7 | ...

            var frame = new Http2GoAwayFrame(header)
            {
                ReservedBit = BufferHelper.ReadBit(header.Payload[0], 0),
                LastStreamId = BufferHelper.ReadUInt31(header.Payload, 0),
                ErrorCode = BufferHelper.ReadUInt32(header.Payload, 4),
                AdditionalDebugDataLength = header.PayloadLength - 8
            };

            if (frame.AdditionalDebugDataLength <= 0) return frame;
            frame.AdditionalDebugData = BufferPool.Get(frame.AdditionalDebugDataLength, true);
            Array.Copy(header.Payload, 8, frame.AdditionalDebugData, 0, frame.AdditionalDebugDataLength);

            return frame;
        }

        public static Http2PingFrame ReadPingFrame(Http2FrameHeaderAndPayload header)
        {
            // https://httpwg.org/specs/rfc7540.html#PING

            var frame = new Http2PingFrame(header);

            Array.Copy(header.Payload, 0, frame.OpaqueData, 0, frame.OpaqueDataLength);

            return frame;
        }

        public static Http2PushPromiseFrame ReadPush_PromiseFrame(Http2FrameHeaderAndPayload header)
        {
            // https://httpwg.org/specs/rfc7540.html#PUSH_PROMISE

            Http2PushPromiseFrame frame = new Http2PushPromiseFrame(header);
            frame.HeaderBlockFragmentLength = header.PayloadLength - 4; // PromisedStreamId

            bool isPadded = (frame.Flags & Http2PushPromiseFlags.Padded) != 0;
            if (isPadded)
            {
                frame.PadLength = header.Payload[0];
                frame.HeaderBlockFragmentLength -= (uint)(1 + (frame.PadLength ?? 0));
            }

            frame.ReservedBit = BufferHelper.ReadBit(header.Payload[1], 0);
            frame.PromisedStreamId = BufferHelper.ReadUInt31(header.Payload, 1);

            frame.HeaderBlockFragmentIdx = (UInt32)(isPadded ? 5 : 4);
            frame.HeaderBlockFragment = header.Payload;
            header.Payload = null;

            return frame;
        }

        public static Http2RstStreamFrame ReadRST_StreamFrame(Http2FrameHeaderAndPayload header)
        {
            // https://httpwg.org/specs/rfc7540.html#RST_STREAM

            Http2RstStreamFrame frame = new Http2RstStreamFrame(header);
            frame.ErrorCode = BufferHelper.ReadUInt32(header.Payload, 0);

            return frame;
        }

        public static Http2PriorityFrame ReadPriorityFrame(Http2FrameHeaderAndPayload header)
        {
            // https://httpwg.org/specs/rfc7540.html#PRIORITY

            if (header.PayloadLength != 5)
            {
                //throw FRAME_SIZE_ERROR
            }

            Http2PriorityFrame frame = new Http2PriorityFrame(header);

            frame.IsExclusive = BufferHelper.ReadBit(header.Payload[0], 0);
            frame.StreamDependency = BufferHelper.ReadUInt31(header.Payload, 0);
            frame.Weight = header.Payload[4];

            return frame;
        }

        public static Http2HeadersFrame ReadHeadersFrame(Http2FrameHeaderAndPayload header)
        {
            // https://httpwg.org/specs/rfc7540.html#HEADERS

            Http2HeadersFrame frame = new Http2HeadersFrame(header);
            frame.HeaderBlockFragmentLength = header.PayloadLength;

            bool isPadded = (frame.Flags & Http2HeadersFlags.Padded) != 0;
            bool isPriority = (frame.Flags & Http2HeadersFlags.Priority) != 0;

            int payloadIdx = 0;

            if (isPadded)
            {
                frame.PadLength = header.Payload[payloadIdx++];

                uint subLength = (uint)(1 + (frame.PadLength ?? 0));
                if (subLength <= frame.HeaderBlockFragmentLength)
                    frame.HeaderBlockFragmentLength -= subLength;
                //else
                //    throw PROTOCOL_ERROR;
            }

            if (isPriority)
            {
                frame.IsExclusive = BufferHelper.ReadBit(header.Payload[payloadIdx], 0);
                frame.StreamDependency = BufferHelper.ReadUInt31(header.Payload, payloadIdx);
                payloadIdx += 4;
                frame.Weight = header.Payload[payloadIdx++];

                uint subLength = 5;
                if (subLength <= frame.HeaderBlockFragmentLength)
                    frame.HeaderBlockFragmentLength -= subLength;
                //else
                //    throw PROTOCOL_ERROR;
            }

            frame.HeaderBlockFragmentIdx = (UInt32)payloadIdx;
            frame.HeaderBlockFragment = header.Payload;

            return frame;
        }

        public static Http2DataFrame ReadDataFrame(Http2FrameHeaderAndPayload header)
        {
            // https://httpwg.org/specs/rfc7540.html#DATA

            Http2DataFrame frame = new Http2DataFrame(header);

            frame.DataLength = header.PayloadLength;

            bool isPadded = (frame.Flags & Http2DataFlags.Padded) != 0;
            if (isPadded)
            {
                frame.PadLength = header.Payload[0];

                uint subLength = (uint)(1 + (frame.PadLength ?? 0));
                if (subLength <= frame.DataLength)
                    frame.DataLength -= subLength;
                //else
                //    throw PROTOCOL_ERROR;
            }

            frame.DataIdx = (UInt32)(isPadded ? 1 : 0);
            frame.Data = header.Payload;
            header.Payload = null;

            return frame;
        }

        public static Http2AltSvcFrame ReadAltSvcFrame(Http2FrameHeaderAndPayload header)
        {
            Http2AltSvcFrame frame = new Http2AltSvcFrame(header);

            // Implement

            return frame;
        }

        public static void StreamRead(Stream stream, byte[] buffer, int offset, uint count)
        {
            if (count == 0)
                return;

            uint sumRead = 0;
            do
            {
                int readCount = (int)(count - sumRead);
                int streamReadCount = stream.Read(buffer, (int)(offset + sumRead), readCount);
                if (streamReadCount <= 0 && readCount > 0)
                    throw new Exception("TCP Stream closed!");
                sumRead += (uint)streamReadCount;
            } while (sumRead < count);
        }

        public static PooledBuffer HeaderAsBinary(Http2FrameHeaderAndPayload header)
        {
            // https://httpwg.org/specs/rfc7540.html#FrameHeader

            var buffer = BufferPool.Get(9, true);

            BufferHelper.SetUInt24(buffer, 0, header.PayloadLength);
            buffer[3] = (byte)header.Type;
            buffer[4] = header.Flags;
            BufferHelper.SetUInt31(buffer, 5, header.StreamId);

            return new PooledBuffer { Data = buffer, Length = 9 };
        }

        public static Http2FrameHeaderAndPayload ReadHeader(Stream stream)
        {
            byte[] buffer = BufferPool.Get(9, true);

            StreamRead(stream, buffer, 0, 9);

            Http2FrameHeaderAndPayload header = new Http2FrameHeaderAndPayload();

            header.PayloadLength = BufferHelper.ReadUInt24(buffer, 0);
            header.Type = (Http2FrameTypes)buffer[3];
            header.Flags = buffer[4];
            header.StreamId = BufferHelper.ReadUInt31(buffer, 5);

            BufferPool.Release(buffer);

            header.Payload = BufferPool.Get(header.PayloadLength, true);
            StreamRead(stream, header.Payload, 0, header.PayloadLength);

            return header;
        }

        public static Http2SettingsFrame ReadSettings(Http2FrameHeaderAndPayload header)
        {
            Http2SettingsFrame frame = new Http2SettingsFrame(header);

            if (header.PayloadLength > 0)
            {
                int kvpCount = (int)(header.PayloadLength / 6);

                frame.Settings = new List<KeyValuePair<Http2Settings, uint>>(kvpCount);
                for (int i = 0; i < kvpCount; ++i)
                {
                    Http2Settings key = (Http2Settings)BufferHelper.ReadUInt16(header.Payload, i * 6);
                    UInt32 value = BufferHelper.ReadUInt32(header.Payload, (i * 6) + 2);

                    frame.Settings.Add(new KeyValuePair<Http2Settings, uint>(key, value));
                }
            }

            return frame;
        }

        public static Http2FrameHeaderAndPayload CreateAckSettingsFrame()
        {
            var frame = new Http2FrameHeaderAndPayload
            {
                Type = Http2FrameTypes.Settings,
                Flags = (byte)Http2SettingsFlags.Ack
            };

            return frame;
        }

        public static Http2FrameHeaderAndPayload CreateSettingsFrame(List<KeyValuePair<Http2Settings, uint>> settings)
        {
            Http2FrameHeaderAndPayload frame = new Http2FrameHeaderAndPayload
            {
                Type = Http2FrameTypes.Settings,
                Flags = 0,
                PayloadLength = (uint)settings.Count * 6
            };

            frame.Payload = BufferPool.Get(frame.PayloadLength, true);

            for (var i = 0; i < settings.Count; ++i)
            {
                BufferHelper.SetUInt16(frame.Payload, i * 6, (ushort)settings[i].Key);
                BufferHelper.SetUInt32(frame.Payload, (i * 6) + 2, settings[i].Value);
            }

            return frame;
        }

        public static Http2FrameHeaderAndPayload CreatePingFrame(Http2PingFlags flags = Http2PingFlags.None)
        {
            // https://httpwg.org/specs/rfc7540.html#PING

            var frame = new Http2FrameHeaderAndPayload
            {
                Type = Http2FrameTypes.Ping,
                Flags = (byte)flags,
                StreamId = 0,
                Payload = BufferPool.Get(8, true),
                PayloadLength = 8
            };

            return frame;
        }

        public static Http2FrameHeaderAndPayload CreateWindowUpdateFrame(UInt32 streamId, UInt32 windowSizeIncrement)
        {
            // https://httpwg.org/specs/rfc7540.html#WINDOW_UPDATE

            Http2FrameHeaderAndPayload frame = new Http2FrameHeaderAndPayload
            {
                Type = Http2FrameTypes.WindowUpdate,
                Flags = 0,
                StreamId = streamId,
                Payload = BufferPool.Get(4, true),
                PayloadLength = 4
            };

            BufferHelper.SetBit(0, 0, 0);
            BufferHelper.SetUInt31(frame.Payload, 0, windowSizeIncrement);

            return frame;
        }

        public static Http2FrameHeaderAndPayload CreateGoAwayFrame(UInt32 lastStreamId, Http2ErrorCodes error)
        {
            // https://httpwg.org/specs/rfc7540.html#GOAWAY

            Http2FrameHeaderAndPayload frame = new Http2FrameHeaderAndPayload();
            frame.Type = Http2FrameTypes.Goaway;
            frame.Flags = 0;
            frame.StreamId = 0;
            frame.Payload = BufferPool.Get(8, true);
            frame.PayloadLength = 8;

            BufferHelper.SetUInt31(frame.Payload, 0, lastStreamId);
            BufferHelper.SetUInt31(frame.Payload, 4, (UInt32)error);

            return frame;
        }

        public static Http2FrameHeaderAndPayload CreateRSTFrame(UInt32 streamId, Http2ErrorCodes errorCode)
        {
            // https://httpwg.org/specs/rfc7540.html#RST_STREAM

            Http2FrameHeaderAndPayload frame = new Http2FrameHeaderAndPayload();
            frame.Type = Http2FrameTypes.RstStream;
            frame.Flags = 0;
            frame.StreamId = streamId;
            frame.Payload = BufferPool.Get(4, true);
            frame.PayloadLength = 4;

            BufferHelper.SetUInt32(frame.Payload, 0, (UInt32)errorCode);

            return frame;
        }
    }
}

#endif
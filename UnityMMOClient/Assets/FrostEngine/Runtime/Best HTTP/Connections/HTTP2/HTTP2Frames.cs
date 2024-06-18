#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2

using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.Memory;

// ReSharper disable once CheckNamespace
namespace BestHTTP.Connections.HTTP2
{
    // https://httpwg.org/specs/rfc7540.html#iana-frames
    public enum Http2FrameTypes : byte
    {
        Data = 0x00,
        Headers = 0x01,
        Priority = 0x02,
        RstStream = 0x03,
        Settings = 0x04,
        PushPromise = 0x05,
        Ping = 0x06,
        Goaway = 0x07,
        WindowUpdate = 0x08,
        Continuation = 0x09,

        // https://tools.ietf.org/html/rfc7838#section-4
        AltSvc = 0x0A
    }

    [Flags]
    public enum Http2DataFlags : byte
    {
        None = 0x00,
        EndStream = 0x01,
        Padded = 0x08,
    }

    [Flags]
    public enum Http2HeadersFlags : byte
    {
        None = 0x00,
        EndStream = 0x01,
        EndHeaders = 0x04,
        Padded = 0x08,
        Priority = 0x20,
    }

    [Flags]
    public enum Http2SettingsFlags : byte
    {
        None = 0x00,
        Ack = 0x01,
    }

    [Flags]
    public enum Http2PushPromiseFlags : byte
    {
        None = 0x00,
        EndHeaders = 0x04,
        Padded = 0x08,
    }

    [Flags]
    public enum Http2PingFlags : byte
    {
        None = 0x00,
        Ack = 0x01,
    }

    [Flags]
    public enum Http2ContinuationFlags : byte
    {
        None = 0x00,
        EndHeaders = 0x04,
    }

    public struct Http2FrameHeaderAndPayload
    {
        public UInt32 PayloadLength;
        public Http2FrameTypes Type;
        public byte Flags;
        public UInt32 StreamId;
        public byte[] Payload;

        public UInt32 PayloadOffset;
        public bool DontUseMemPool;

        public override string ToString()
        {
            var sb = new StringBuilder(10);
            sb.Append($"[HTTP2FrameHeaderAndPayload Length: {this.PayloadLength}, ");
            sb.Append($"Type: {this.Type}, ");
            sb.Append($"Flags: {this.Flags.ToBinaryStr()}, ");
            sb.Append($"StreamId: {this.StreamId}, ");
            sb.Append($"PayloadOffset: {this.PayloadOffset}, ");
            sb.Append($"DontUseMemPool: {this.DontUseMemPool}]");
            return $"{sb}";
        }

        public string PayloadAsHex()
        {
            var sb = new StringBuilder("[", (int)this.PayloadLength + 1);
            if (this.Payload != null && this.PayloadLength > 0)
            {
                uint idx = this.PayloadOffset;
                sb.Append(this.Payload[idx++]);
                for (int i = 1; i < this.PayloadLength; i++)
                {
                    sb.AppendFormat(", {0:X2}", this.Payload[idx++]);
                }
            }

            sb.Append("]");

            return sb.ToString();
        }
    }

    public struct Http2SettingsFrame
    {
        private readonly Http2FrameHeaderAndPayload _header;

        public Http2SettingsFlags Flags => (Http2SettingsFlags)this._header.Flags;

        public List<KeyValuePair<Http2Settings, UInt32>> Settings;

        public Http2SettingsFrame(Http2FrameHeaderAndPayload header)
        {
            this._header = header;
            this.Settings = null;
        }

        public override string ToString()
        {
            string settings = null;
            if (this.Settings != null)
            {
                var sb = new StringBuilder("[");
                foreach (var kvp in this.Settings)
                    sb.AppendFormat("[{0}: {1}]", kvp.Key, kvp.Value);
                sb.Append("]");

                settings = sb.ToString();
            }
            var stringBuilder = new StringBuilder(10);
            stringBuilder.Append($"[HTTP2SettingsFrame Header: {this._header.ToString()}, ");
            stringBuilder.Append($"Flags: {this.Flags}, ");
            stringBuilder.Append($"Settings: {settings ?? "Empty"}]");
            
            return $"{stringBuilder}";
        }
    }

    public struct Http2DataFrame
    {
        private readonly Http2FrameHeaderAndPayload _header;

        public Http2DataFlags Flags => (Http2DataFlags)this._header.Flags;

        public byte? PadLength;
        public UInt32 DataIdx;
        public byte[] Data;
        public uint DataLength;

        public Http2DataFrame(Http2FrameHeaderAndPayload header)
        {
            this._header = header;
            this.PadLength = null;
            this.DataIdx = 0;
            this.Data = null;
            this.DataLength = 0;
        }

        public override string ToString()
        {
            return
                $"[HTTP2DataFrame Header: {this._header.ToString()}, Flags: {this.Flags}, PadLength: {(this.PadLength == null ? ":Empty" : this.PadLength.Value.ToString())}, DataLength: {this.DataLength}]";
        }
    }

    public struct Http2HeadersFrame
    {
        private readonly Http2FrameHeaderAndPayload _header;

        public Http2HeadersFlags Flags => (Http2HeadersFlags)this._header.Flags;

        public byte? PadLength;
        public byte? IsExclusive;
        public UInt32? StreamDependency;
        public byte? Weight;
        public UInt32 HeaderBlockFragmentIdx;
        public byte[] HeaderBlockFragment;
        public UInt32 HeaderBlockFragmentLength;

        public Http2HeadersFrame(Http2FrameHeaderAndPayload header)
        {
            this._header = header;
            this.PadLength = null;
            this.IsExclusive = null;
            this.StreamDependency = null;
            this.Weight = null;
            this.HeaderBlockFragmentIdx = 0;
            this.HeaderBlockFragment = null;
            this.HeaderBlockFragmentLength = 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(10);
            sb.Append($"[HTTP2HeadersFrame Header: {this._header.ToString()},");
            sb.Append($" Flags: {this.Flags}, ");
            sb.Append($"PadLength: {(this.PadLength == null ? ":Empty" : this.PadLength.Value.ToString())}, ");
            sb.Append($"IsExclusive: {(this.IsExclusive == null ? "Empty" : this.IsExclusive.Value.ToString())}, ");
            sb.Append(
                $"StreamDependency: {(this.StreamDependency == null ? "Empty" : this.StreamDependency.Value.ToString())}, ");
            sb.Append($"Weight: {(this.Weight == null ? "Empty" : this.Weight.Value.ToString())}, ");
            sb.Append($"HeaderBlockFragmentLength: {this.HeaderBlockFragmentLength}]");
            return sb.ToString();
        }
    }

    public struct Http2PriorityFrame
    {
        private readonly Http2FrameHeaderAndPayload _header;

        public byte IsExclusive;
        public UInt32 StreamDependency;
        public byte Weight;

        public Http2PriorityFrame(Http2FrameHeaderAndPayload header)
        {
            this._header = header;
            this.IsExclusive = 0;
            this.StreamDependency = 0;
            this.Weight = 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(10);
            sb.Append($"[HTTP2PriorityFrame Header: {this._header.ToString()}, ");
            sb.Append($"IsExclusive: {this.IsExclusive}, ");
            sb.Append($"StreamDependency: {this.StreamDependency}, ");
            sb.Append($"Weight: {this.Weight}]");
            return sb.ToString();
        }
    }

    public struct Http2RstStreamFrame
    {
        private readonly Http2FrameHeaderAndPayload _header;

        public UInt32 ErrorCode;

        public Http2ErrorCodes Error => (Http2ErrorCodes)this.ErrorCode;

        public Http2RstStreamFrame(Http2FrameHeaderAndPayload header)
        {
            this._header = header;
            this.ErrorCode = 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(10);
            sb.Append($"[HTTP2RST_StreamFrame Header: {this._header.ToString()}, ");
            sb.Append($"Error: {this.Error}({this.ErrorCode})]");
            return $"{sb}";
        }
    }

    public struct Http2PushPromiseFrame
    {
        private readonly Http2FrameHeaderAndPayload _header;

        public Http2PushPromiseFlags Flags => (Http2PushPromiseFlags)this._header.Flags;

        public byte? PadLength;
        public byte ReservedBit;
        public UInt32 PromisedStreamId;
        public UInt32 HeaderBlockFragmentIdx;
        public byte[] HeaderBlockFragment;
        public UInt32 HeaderBlockFragmentLength;

        public Http2PushPromiseFrame(Http2FrameHeaderAndPayload header)
        {
            this._header = header;
            this.PadLength = null;
            this.ReservedBit = 0;
            this.PromisedStreamId = 0;
            this.HeaderBlockFragmentIdx = 0;
            this.HeaderBlockFragment = null;
            this.HeaderBlockFragmentLength = 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(10);
            sb.Append($"[HTTP2Push_PromiseFrame Header: {this._header.ToString()}, ");
            sb.Append($"Flags: {this.Flags}, ");
            sb.Append($"PadLength: {(this.PadLength == null ? "Empty" : this.PadLength.Value.ToString())}, ");
            sb.Append($"ReservedBit: {this.ReservedBit}, ");
            sb.Append($"PromisedStreamId: {this.PromisedStreamId}, ");
            sb.Append($"HeaderBlockFragmentLength: {this.HeaderBlockFragmentLength}]");

            return $"{sb}";
        }
    }

    public readonly struct Http2PingFrame
    {
        private readonly Http2FrameHeaderAndPayload _header;

        public Http2PingFlags Flags => (Http2PingFlags)this._header.Flags;

        public readonly byte[] OpaqueData;
        public readonly byte OpaqueDataLength;

        public Http2PingFrame(Http2FrameHeaderAndPayload header)
        {
            this._header = header;
            this.OpaqueData = BufferPool.Get(8, true);
            this.OpaqueDataLength = 8;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(10);
            sb.Append($"[HTTP2PingFrame Header: {this._header.ToString()}, ");
            sb.Append($"Flags: {this.Flags}, ");
            var hexString = SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(
                data: this.OpaqueData,
                off: 0,
                length: this.OpaqueDataLength);
            sb.Append($"OpaqueData: {hexString}]");

            return $"{sb}";
        }
    }

    public struct Http2GoAwayFrame
    {
        private readonly Http2FrameHeaderAndPayload _header;

        public Http2ErrorCodes Error => (Http2ErrorCodes)this.ErrorCode;

        public byte ReservedBit;
        public UInt32 LastStreamId;
        public UInt32 ErrorCode;
        public byte[] AdditionalDebugData;
        public UInt32 AdditionalDebugDataLength;

        public Http2GoAwayFrame(Http2FrameHeaderAndPayload header)
        {
            this._header = header;
            this.ReservedBit = 0;
            this.LastStreamId = 0;
            this.ErrorCode = 0;
            this.AdditionalDebugData = null;
            this.AdditionalDebugDataLength = 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(10);
            sb.Append($"[HTTP2GoAwayFrame Header: {this._header.ToString()}, ");
            sb.Append($"ReservedBit: {this.ReservedBit}, ");
            sb.Append($"LastStreamId: {this.LastStreamId}, ");
            sb.Append($"Error: {this.Error}({this.ErrorCode}), ");
            sb.Append($"AdditionalDebugData({this.AdditionalDebugDataLength}): ");
            var hexString = SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(
                data: this.AdditionalDebugData,
                off: 0,
                length: (int)this.AdditionalDebugDataLength);
            sb.Append($"{(this.AdditionalDebugData == null ? "Empty" : hexString)}]");

            return $"{sb}";
        }
    }

    public struct Http2WindowUpdateFrame
    {
        private readonly Http2FrameHeaderAndPayload _header;

        public byte ReservedBit;
        public UInt32 WindowSizeIncrement;

        public Http2WindowUpdateFrame(Http2FrameHeaderAndPayload header)
        {
            this._header = header;
            this.ReservedBit = 0;
            this.WindowSizeIncrement = 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(10);
            sb.Append($"[HTTP2WindowUpdateFrame Header: {this._header.ToString()}, ");
            sb.Append($"ReservedBit: {this.ReservedBit}, ");
            sb.Append($"WindowSizeIncrement: {this.WindowSizeIncrement}]");
            return $"{sb}";
        }
    }

    public struct Http2ContinuationFrame
    {
        private readonly Http2FrameHeaderAndPayload _header;

        private Http2ContinuationFlags Flags => (Http2ContinuationFlags)this._header.Flags;

        public byte[] HeaderBlockFragment;

        private UInt32 HeaderBlockFragmentLength => this._header.PayloadLength;

        public Http2ContinuationFrame(Http2FrameHeaderAndPayload header)
        {
            this._header = header;
            this.HeaderBlockFragment = null;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(10);
            sb.Append($"[HTTP2ContinuationFrame Header: {this._header.ToString()}, ");
            sb.Append($"Flags: {this.Flags}, ");
            sb.Append($"HeaderBlockFragmentLength: {this.HeaderBlockFragmentLength}]");

            return $"{sb}";
        }
    }

    /// <summary>
    /// https://tools.ietf.org/html/rfc7838#section-4
    /// </summary>
    public struct Http2AltSvcFrame
    {
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once NotAccessedField.Local
        private readonly Http2FrameHeaderAndPayload Header;

#pragma warning disable CS0414
        private string _origin;
#pragma warning restore CS0414
#pragma warning disable CS0414
        private string _altSvcFieldValue;
#pragma warning restore CS0414

        public Http2AltSvcFrame(Http2FrameHeaderAndPayload header)
        {
            this.Header = header;
            this._origin = null;
            this._altSvcFieldValue = null;
        }
    }
}

#endif
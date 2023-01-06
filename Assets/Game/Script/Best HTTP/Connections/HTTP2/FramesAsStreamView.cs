#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2

using System;
using System.Collections.Generic;
using System.IO;
using BestHTTP.PlatformSupport.Memory;

// ReSharper disable once CheckNamespace
namespace BestHTTP.Connections.HTTP2
{
    internal interface IFrameDataView : IDisposable
    {
        long Length { get; }
        long Position { get; }

        void AddFrame(HTTP2FrameHeaderAndPayload frame);
        int ReadByte();
        int Read(byte[] buffer, int offset, int count);
    }

    internal abstract class CommonFrameView : IFrameDataView
    {
        protected readonly List<HTTP2FrameHeaderAndPayload> Frames = new List<HTTP2FrameHeaderAndPayload>();
        protected int CurrentFrameIdx = -1;
        protected byte[] Data;
        protected UInt32 dataOffset;
        protected UInt32 maxOffset;
        public long Length { get; protected set; }
        public long Position { get; protected set; }

        public abstract void AddFrame(HTTP2FrameHeaderAndPayload frame);

        public virtual int Read(byte[] buffer, int offset, int count)
        {
            if (this.dataOffset >= this.maxOffset && !AdvanceFrame())
                return -1;

            int readCount = 0;

            while (count > 0)
            {
                long copyCount = Math.Min(count, this.maxOffset - this.dataOffset);

                Array.Copy(this.Data, this.dataOffset, buffer, offset + readCount, copyCount);

                count -= (int)copyCount;
                readCount += (int)copyCount;

                this.dataOffset += (UInt32)copyCount;
                this.Position += copyCount;

                if (this.dataOffset >= this.maxOffset && !AdvanceFrame())
                    break;
            }

            return readCount;
        }

        public virtual int ReadByte()
        {
            if (this.dataOffset >= this.maxOffset && !AdvanceFrame())
                return -1;

            byte data = this.Data[this.dataOffset];
            this.dataOffset++;
            this.Position++;

            return data;
        }

        public virtual void Dispose()
        {
            for (int i = 0; i < this.Frames.Count; ++i)
                if (this.Frames[i].Payload != null && !this.Frames[i].DontUseMemPool)
                    BufferPool.Release(this.Frames[i].Payload);
            this.Frames.Clear();
        }

        protected abstract long CalculateDataLengthForFrame(HTTP2FrameHeaderAndPayload frame);

        protected abstract bool AdvanceFrame();

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder("[CommonFrameView ");

            for (int i = 0; i < this.Frames.Count; ++i)
            {
                sb.AppendFormat("{0} Payload: {1}\n", this.Frames[i], this.Frames[i].PayloadAsHex());
            }

            sb.Append("]");

            return sb.ToString();
        }
    }

    sealed class HeaderFrameView : CommonFrameView
    {
        public override void AddFrame(HTTP2FrameHeaderAndPayload frame)
        {
            if (frame.Type != HTTP2FrameTypes.HEADERS && frame.Type != HTTP2FrameTypes.CONTINUATION)
                throw new ArgumentException("HeaderFrameView - Unexpected frame type: " + frame.Type);

            this.Frames.Add(frame);
            this.Length += CalculateDataLengthForFrame(frame);

            if (this.CurrentFrameIdx == -1)
                AdvanceFrame();
        }

        protected override long CalculateDataLengthForFrame(HTTP2FrameHeaderAndPayload frame)
        {
            switch (frame.Type)
            {
                case HTTP2FrameTypes.HEADERS:
                    return HTTP2FrameHelper.ReadHeadersFrame(frame).HeaderBlockFragmentLength;

                case HTTP2FrameTypes.CONTINUATION:
                    return frame.PayloadLength;
            }

            return 0;
        }

        protected override bool AdvanceFrame()
        {
            if (this.CurrentFrameIdx >= this.Frames.Count - 1)
                return false;

            this.CurrentFrameIdx++;
            HTTP2FrameHeaderAndPayload frame = this.Frames[this.CurrentFrameIdx];

            this.Data = frame.Payload;

            switch (frame.Type)
            {
                case HTTP2FrameTypes.HEADERS:
                    var header = HTTP2FrameHelper.ReadHeadersFrame(frame);
                    this.dataOffset = header.HeaderBlockFragmentIdx;
                    this.maxOffset = this.dataOffset + header.HeaderBlockFragmentLength;
                    break;

                case HTTP2FrameTypes.CONTINUATION:
                    this.dataOffset = 0;
                    this.maxOffset = frame.PayloadLength;
                    break;
            }

            return true;
        }
    }

    sealed class DataFrameView : CommonFrameView
    {
        public override void AddFrame(HTTP2FrameHeaderAndPayload frame)
        {
            if (frame.Type != HTTP2FrameTypes.DATA)
                throw new ArgumentException("HeaderFrameView - Unexpected frame type: " + frame.Type);

            this.Frames.Add(frame);
            this.Length += CalculateDataLengthForFrame(frame);
        }

        protected override long CalculateDataLengthForFrame(HTTP2FrameHeaderAndPayload frame)
        {
            return HTTP2FrameHelper.ReadDataFrame(frame).DataLength;
        }

        protected override bool AdvanceFrame()
        {
            if (this.CurrentFrameIdx >= this.Frames.Count - 1)
                return false;

            this.CurrentFrameIdx++;
            HTTP2FrameHeaderAndPayload frame = this.Frames[this.CurrentFrameIdx];
            HTTP2DataFrame dataFrame = HTTP2FrameHelper.ReadDataFrame(frame);

            this.Data = frame.Payload;
            this.dataOffset = dataFrame.DataIdx;
            this.maxOffset = dataFrame.DataIdx + dataFrame.DataLength;

            return true;
        }
    }

    sealed class FramesAsStreamView : Stream
    {
        private readonly IFrameDataView _view;

        public FramesAsStreamView(IFrameDataView view)
        {
            this._view = view;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => this._view.Length;

        public override long Position
        {
            get => this._view.Position;
            set => throw new NotSupportedException();
        }

        public void AddFrame(HTTP2FrameHeaderAndPayload frame)
        {
            this._view.AddFrame(frame);
        }

        public override int ReadByte()
        {
            return this._view.ReadByte();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this._view.Read(buffer, offset, count);
        }

        public override void Close()
        {
            base.Close();
            this._view.Dispose();
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return this._view.ToString();
        }
    }
}

#endif
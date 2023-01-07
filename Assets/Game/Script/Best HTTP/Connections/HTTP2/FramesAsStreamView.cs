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

        void AddFrame(Http2FrameHeaderAndPayload frame);
        int ReadByte();
        int Read(byte[] buffer, int offset, int count);
    }

    internal abstract class CommonFrameView : IFrameDataView
    {
        protected readonly List<Http2FrameHeaderAndPayload> Frames = new List<Http2FrameHeaderAndPayload>();
        protected int CurrentFrameIdx = -1;
        protected byte[] Data;
        protected UInt32 DataOffset;
        protected UInt32 MaxOffset;
        public long Length { get; protected set; }
        public long Position { get; private set; }

        public abstract void AddFrame(Http2FrameHeaderAndPayload frame);

        public virtual int Read(byte[] buffer, int offset, int count)
        {
            if (this.DataOffset >= this.MaxOffset && !AdvanceFrame())
                return -1;

            int readCount = 0;

            while (count > 0)
            {
                long copyCount = Math.Min(count, this.MaxOffset - this.DataOffset);

                Array.Copy(this.Data, this.DataOffset, buffer, offset + readCount, copyCount);

                count -= (int)copyCount;
                readCount += (int)copyCount;

                this.DataOffset += (UInt32)copyCount;
                this.Position += copyCount;

                if (this.DataOffset >= this.MaxOffset && !AdvanceFrame())
                    break;
            }

            return readCount;
        }

        public virtual int ReadByte()
        {
            if (this.DataOffset >= this.MaxOffset && !AdvanceFrame())
                return -1;

            byte data = this.Data[this.DataOffset];
            this.DataOffset++;
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

        protected abstract long CalculateDataLengthForFrame(Http2FrameHeaderAndPayload frame);

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
        public override void AddFrame(Http2FrameHeaderAndPayload frame)
        {
            if (frame.Type != Http2FrameTypes.Headers && frame.Type != Http2FrameTypes.Continuation)
                throw new ArgumentException("HeaderFrameView - Unexpected frame type: " + frame.Type);

            this.Frames.Add(frame);
            this.Length += CalculateDataLengthForFrame(frame);

            if (this.CurrentFrameIdx == -1)
                AdvanceFrame();
        }

        protected override long CalculateDataLengthForFrame(Http2FrameHeaderAndPayload frame)
        {
            switch (frame.Type)
            {
                case Http2FrameTypes.Headers:
                    return Http2FrameHelper.ReadHeadersFrame(frame).HeaderBlockFragmentLength;

                case Http2FrameTypes.Continuation:
                    return frame.PayloadLength;
            }

            return 0;
        }

        protected override bool AdvanceFrame()
        {
            if (this.CurrentFrameIdx >= this.Frames.Count - 1)
                return false;

            this.CurrentFrameIdx++;
            Http2FrameHeaderAndPayload frame = this.Frames[this.CurrentFrameIdx];

            this.Data = frame.Payload;

            switch (frame.Type)
            {
                case Http2FrameTypes.Headers:
                    var header = Http2FrameHelper.ReadHeadersFrame(frame);
                    this.DataOffset = header.HeaderBlockFragmentIdx;
                    this.MaxOffset = this.DataOffset + header.HeaderBlockFragmentLength;
                    break;

                case Http2FrameTypes.Continuation:
                    this.DataOffset = 0;
                    this.MaxOffset = frame.PayloadLength;
                    break;
            }

            return true;
        }
    }

    sealed class DataFrameView : CommonFrameView
    {
        public override void AddFrame(Http2FrameHeaderAndPayload frame)
        {
            if (frame.Type != Http2FrameTypes.Data)
                throw new ArgumentException("HeaderFrameView - Unexpected frame type: " + frame.Type);

            this.Frames.Add(frame);
            this.Length += CalculateDataLengthForFrame(frame);
        }

        protected override long CalculateDataLengthForFrame(Http2FrameHeaderAndPayload frame)
        {
            return Http2FrameHelper.ReadDataFrame(frame).DataLength;
        }

        protected override bool AdvanceFrame()
        {
            if (this.CurrentFrameIdx >= this.Frames.Count - 1)
                return false;

            this.CurrentFrameIdx++;
            Http2FrameHeaderAndPayload frame = this.Frames[this.CurrentFrameIdx];
            Http2DataFrame dataFrame = Http2FrameHelper.ReadDataFrame(frame);

            this.Data = frame.Payload;
            this.DataOffset = dataFrame.DataIdx;
            this.MaxOffset = dataFrame.DataIdx + dataFrame.DataLength;

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

        public void AddFrame(Http2FrameHeaderAndPayload frame)
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
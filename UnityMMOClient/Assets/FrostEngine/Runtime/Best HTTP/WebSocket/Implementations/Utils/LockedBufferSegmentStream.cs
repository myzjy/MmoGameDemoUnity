#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2 && !BESTHTTP_DISABLE_WEBSOCKET
using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.Memory;

namespace BestHTTP.WebSocket.Implementations.Utils
{
    public sealed class LockedBufferSegmentStream : BufferSegmentStream
    {
        public bool IsClosed { get; private set; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (bufferList)
            {
                if (this.IsClosed &&
                    bufferList.Count == 0)
                {
                    return 0;
                }

                var sumReadCount = base.Read(buffer, offset, count);

                return sumReadCount == 0 ? -1 : sumReadCount;
            }
        }

        public override void Write(BufferSegment bufferSegment)
        {
            lock (bufferList)
            {
                if (this.IsClosed)
                {
                    return;
                }

                base.Write(bufferSegment);
            }
        }

        public override void Reset()
        {
            lock (bufferList)
            {
                base.Reset();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Reset();
        }

        public override void Close()
        {
            this.IsClosed = true;
        }
    }
}
#endif
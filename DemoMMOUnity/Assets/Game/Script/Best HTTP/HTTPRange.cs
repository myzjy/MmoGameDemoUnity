namespace BestHTTP
{
    /// <summary>
    ///
    /// </summary>
    public sealed class HttpRange
    {
        internal HttpRange()
        {
            this.ContentLength = -1;
            this.IsValid = false;
        }

        internal HttpRange(int contentLength)
        {
            this.ContentLength = contentLength;
            this.IsValid = false;
        }

        internal HttpRange(long firstBytePosition, long lastBytePosition, long contentLength)
        {
            this.FirstBytePos  = firstBytePosition;
            this.LastBytePos   = lastBytePosition;
            this.ContentLength = contentLength;

            // 如果一个text -content-range-spec中的text -range- esp-spec的last-byte-pos值小于其first-byte-pos值，或者instance-length值小于或等于其last-byte-pos值，则该text -content-range-spec无效
            this.IsValid      = this.FirstBytePos <= this.LastBytePos && this.ContentLength > this.LastBytePos;
        }

        /// <summary>
        /// 服务器发送的第一个字节的位置。
        /// </summary>
        public long FirstBytePos { get; private set; }

        /// <summary>
        /// 服务器发送的最后一个字节的位置。
        /// </summary>
        public long LastBytePos { get; private set; }

        /// <summary>
        /// 表示服务器上完整实体的总长度，如果该长度未知或难以确定，则为-1。
        /// </summary>
        private long ContentLength { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsValid { get; private set; }

        public override string ToString()
        {
            return $"{FirstBytePos}-{LastBytePos}/{ContentLength} (valid: {IsValid})";
        }
    }
}
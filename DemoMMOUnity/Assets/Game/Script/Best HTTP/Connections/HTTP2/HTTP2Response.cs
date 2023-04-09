#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2

using System;
using System.Collections.Generic;
using System.IO;
using BestHTTP.Core;
using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.Memory;

// ReSharper disable once CheckNamespace
namespace BestHTTP.Connections.HTTP2
{
    using GZipStream = Decompression.Zlib.GZipStream;
    using CompressionMode = Decompression.Zlib.CompressionMode;
    using GZipDecompressor = Decompression.GZipDecompressor;

    public sealed class Http2Response : HttpResponse
    {
        private GZipDecompressor _decompressor;

        private bool _isPrepared;

        public Http2Response(HttpRequest request, bool isFromCache)
            : base(request, isFromCache)
        {
            this.VersionMajor = 2;
            this.VersionMinor = 0;
        }

        // For progress report
        public long ExpectedContentLength { get; private set; }
        private bool IsCompressed { get; set; }

        internal void AddHeaders(List<KeyValuePair<string, string>> headers)
        {
            this.ExpectedContentLength = -1;
            Dictionary<string, List<string>> newHeaders = this.BaseRequest.OnHeadersReceived != null
                ? new Dictionary<string, List<string>>()
                : null;

            foreach (var header in headers)
            {
                if (header.Key.Equals(":status", StringComparison.Ordinal))
                {
                    StatusCode = int.Parse(header.Value);
                    Message = string.Empty;
                }
                else
                {
                    if (!this.IsCompressed && header.Key.Equals("content-encoding", StringComparison.OrdinalIgnoreCase))
                    {
                        this.IsCompressed = true;
                    }
                    else if (BaseRequest.OnDownloadProgress != null &&
                             header.Key.Equals("content-length", StringComparison.OrdinalIgnoreCase))
                    {
                        if (long.TryParse(header.Value, out var contentLength))
                        {
                            this.ExpectedContentLength = contentLength;
                        }
                        else
                        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                            Debug.Log(
                                $"[HTTP2Response] [method:AddHeaders(List<KeyValuePair<string, string>> headers)] AddHeaders - Can't parse Content-Length as an int: '{header.Value}'");
#endif
                        }
                    }

                    AddHeader(header.Key, header.Value);
                }

                if (newHeaders == null) continue;
                if (!newHeaders.TryGetValue(header.Key, out var values))
                {
                    newHeaders.Add(header.Key, values = new List<string>(1));
                }

                values.Add(header.Value);
            }

            if (this.ExpectedContentLength == -1 && BaseRequest.OnDownloadProgress != null)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[HTTP2Response] [method:AddHeaders(List<KeyValuePair<string, string>> headers)] AddHeaders -没有发现内容长度的头!");
#endif
            }

            var requestEvent = new RequestEventInfo(
                request: this.BaseRequest,
                headers: newHeaders);

            RequestEventHelper.EnqueueRequestEvent(requestEvent);
        }

        internal void AddData(Stream stream)
        {
            if (this.IsCompressed)
            {
                using var decoderStream = new GZipStream(stream, CompressionMode.Decompress);
                using var ms = new BufferPoolMemoryStream((int)stream.Length);
                {
                    var buf = BufferPool.Get(8 * 1024, true);
                    int byteCount;

                    while ((byteCount = decoderStream.Read(buf, 0, buf.Length)) > 0)
                    {
                        ms.Write(buf, 0, byteCount);
                    }

                    BufferPool.Release(buf);

                    Data = ms.ToArray();
                }
            }
            else
            {
                Data = BufferPool.Get(stream.Length, false);
                // ReSharper disable once MustUseReturnValue
                stream.Read(Data, 0, (int)stream.Length);
            }
        }

        internal void ProcessData(byte[] payload, int payloadLength)
        {
            if (!this._isPrepared)
            {
                this._isPrepared = true;
                BeginReceiveStreamFragments();
            }

            if (this.IsCompressed)
            {
                this._decompressor ??= new GZipDecompressor(0);
                var result = this._decompressor.Decompress(payload, 0, payloadLength, true, true);

                FeedStreamFragment(result.Data, 0, result.Length);
            }
            else
            {
                FeedStreamFragment(payload, 0, payloadLength);
            }
        }

        internal void FinishProcessData()
        {
            FlushRemainingFragmentBuffer();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            if (this._decompressor == null) return;
            this._decompressor.Dispose();
            this._decompressor = null;
        }
    }
}

#endif
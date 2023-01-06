#if !BESTHTTP_DISABLE_CACHING

using System;
using System.Collections.Generic;
using System.IO;
using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.FileSystem;

namespace BestHTTP.Caching
{
    /// <summary>
    /// 保存所有需要高效缓存的元数据，因此我们不需要触摸磁盘来加载头文件。
    /// </summary>
    public class HttpCacheFileInfo : IComparable<HttpCacheFileInfo>
    {
        #region IComparable<HTTPCacheFileInfo>

        public int CompareTo(HttpCacheFileInfo other)
        {
            return this.LastAccess.CompareTo(other.LastAccess);
        }

        #endregion

        #region HttpCacheFileInfo 定义 属性

        /// <summary>
        /// 这个 HttpCacheFileInfo 所属的uri。
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// 最后一次访问此缓存实体的时间。日期以UTC为单位。
        /// </summary>
        public DateTime LastAccess { get; private set; }

        /// <summary>
        /// 缓存实体主体的长度。
        /// </summary>
        public int BodyLength { get; internal set; }

        /// <summary>
        /// 实体的ETag。
        /// </summary>
        public string ETag { get; private set; }

        /// <summary>
        /// 实体的LastModified日期。
        /// </summary>
        public string LastModified { get; private set; }

        /// <summary>
        /// 缓存何时过期。
        /// </summary>
        public DateTime Expires { get; private set; }

        /// <summary>
        /// 这个age带来了回应
        /// </summary>
        public long Age { get; private set; }

        /// <summary>
        ///在不重新验证的情况下，该条目应从缓存中使用的最长时间。
        /// </summary>
        public long MaxAge { get; private set; }

        /// <summary>
        /// 回复的日期。
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// 指示是否必须使用服务器重新验证实体，或者当内容被认为过时时，是否可以直接从缓存发送实体而不接触服务器。
        /// </summary>
        public bool MustRevalidate { get; private set; }

        /// <summary>
        /// 如果是真的，客户端总是必须在缓存内容过期时重新验证它。
        /// </summary>
        public bool NoCache { get; private set; }

        /// <summary>
        /// 这是一个宽限期，不需要重新验证就可以提供过时的内容。
        /// </summary>
        public long StaleWhileRevalidate { get; private set; }

        /// <summary>
        /// 如果服务器响应5xx错误，则允许客户端提供过时的内容。
        /// </summary>
        public long StaleIfError { get; private set; }

        /// <summary>
        ///接收HTTPResponse的日期和时间。
        /// </summary>
        public DateTime Received { get; private set; }

        /// <summary>
        /// Cached path.
        /// </summary>
        public string ConstructedPath { get; private set; }

        /// <summary>
        /// 这是实体的索引。文件名由该值生成。
        /// </summary>
        internal UInt64 MappedNameIDX { get; private set; }

        #endregion

        #region HttpCacheFileInfo定义的  构造函数

        internal HttpCacheFileInfo(Uri uri)
            : this(uri, DateTime.UtcNow, -1)
        {
        }

        internal HttpCacheFileInfo(Uri uri, DateTime lastAccess, int bodyLength)
        {
            this.Uri = uri;
            this.LastAccess = lastAccess;
            this.BodyLength = bodyLength;
            this.MaxAge = -1;

            this.MappedNameIDX = HTTPCacheService.GetNameIdx();
        }

        internal HttpCacheFileInfo(Uri uri, System.IO.BinaryReader reader, int version)
        {
            this.Uri = uri;
            this.LastAccess = DateTime.FromBinary(reader.ReadInt64());
            this.BodyLength = reader.ReadInt32();

            switch (version)
            {
                case 3:
                    this.NoCache = reader.ReadBoolean();
                    this.StaleWhileRevalidate = reader.ReadInt64();
                    this.StaleIfError = reader.ReadInt64();
                    goto case 2;

                case 2:
                    this.MappedNameIDX = reader.ReadUInt64();
                    goto case 1;

                case 1:
                {
                    this.ETag = reader.ReadString();
                    this.LastModified = reader.ReadString();
                    this.Expires = DateTime.FromBinary(reader.ReadInt64());
                    this.Age = reader.ReadInt64();
                    this.MaxAge = reader.ReadInt64();
                    this.Date = DateTime.FromBinary(reader.ReadInt64());
                    this.MustRevalidate = reader.ReadBoolean();
                    this.Received = DateTime.FromBinary(reader.ReadInt64());
                    break;
                }
            }
        }

        #endregion

        #region HttpCacheFileInfo 定义的 辅助函数

        internal void SaveTo(System.IO.BinaryWriter writer)
        {
            // base
            writer.Write(this.LastAccess.ToBinary());
            writer.Write(this.BodyLength);

            // version 3
            writer.Write(this.NoCache);
            writer.Write(this.StaleWhileRevalidate);
            writer.Write(this.StaleIfError);

            // version 2
            writer.Write(this.MappedNameIDX);

            // version 1
            writer.Write(this.ETag);
            writer.Write(this.LastModified);
            writer.Write(this.Expires.ToBinary());
            writer.Write(this.Age);
            writer.Write(this.MaxAge);
            writer.Write(this.Date.ToBinary());
            writer.Write(this.MustRevalidate);
            writer.Write(this.Received.ToBinary());
        }

        public string GetPath()
        {
            if (ConstructedPath != null)
                return ConstructedPath;

            return ConstructedPath = System.IO.Path.Combine(HTTPCacheService.CacheFolder, MappedNameIDX.ToString("X"));
        }

        public bool IsExists()
        {
            if (!HTTPCacheService.IsSupported)
                return false;

            return HttpManager.IOService.FileExists(GetPath());
        }

        internal void Delete()
        {
            if (!HTTPCacheService.IsSupported)
                return;

            string path = GetPath();
            try
            {
                HttpManager.IOService.FileDelete(path);
            }
            catch
            {
            }
            finally
            {
                Reset();
            }
        }

        private void Reset()
        {
            // MappedNameIDX will remain the same. When we re-save an entity, it will not reset the MappedNameIDX.
            this.BodyLength = -1;
            this.ETag = string.Empty;
            this.Expires = DateTime.FromBinary(0);
            this.LastModified = string.Empty;
            this.Age = 0;
            this.MaxAge = -1;
            this.Date = DateTime.FromBinary(0);
            this.MustRevalidate = false;
            this.Received = DateTime.FromBinary(0);
            this.NoCache = false;
            this.StaleWhileRevalidate = 0;
            this.StaleIfError = 0;
        }

        #endregion

        #region Caching

        internal void SetUpCachingValues(HttpResponse response)
        {
            response.CacheFileInfo = this;

            this.ETag = response.GetFirstHeaderValue("ETag").ToStr(this.ETag ?? string.Empty);
            this.Expires = response.GetFirstHeaderValue("Expires").ToDateTime(this.Expires);
            this.LastModified = response.GetFirstHeaderValue("Last-Modified").ToStr(this.LastModified ?? string.Empty);

            this.Age = response.GetFirstHeaderValue("Age").ToInt64(this.Age);

            this.Date = response.GetFirstHeaderValue("Date").ToDateTime(this.Date);

            List<string> cacheControls = response.GetHeaderValues("cache-control");
            if (cacheControls != null && cacheControls.Count > 0)
            {
                // Merge all Cache-Control header values into one
                string cacheControl = cacheControls[0];
                for (int i = 1; i < cacheControls.Count; ++i)
                    cacheControl += "," + cacheControls[i];

                if (!string.IsNullOrEmpty(cacheControl))
                {
                    HeaderParser parser = new HeaderParser(cacheControl);

                    if (parser.Values != null)
                    {
                        for (int i = 0; i < parser.Values.Count; ++i)
                        {
                            var kvp = parser.Values[i];

                            switch (kvp.Key.ToLowerInvariant())
                            {
                                case "max-age":
                                    if (kvp.HasValue)
                                    {
                                        // Some cache proxies will return float values
                                        double maxAge;
                                        if (double.TryParse(kvp.Value, out maxAge))
                                            this.MaxAge = (int)maxAge;
                                        else
                                            this.MaxAge = 0;
                                    }
                                    else
                                        this.MaxAge = 0;

                                    break;

                                case "stale-while-revalidate":
                                    this.StaleWhileRevalidate = kvp.HasValue ? kvp.Value.ToInt64(0) : 0;
                                    break;

                                case "stale-if-error":
                                    this.StaleIfError = kvp.HasValue ? kvp.Value.ToInt64(0) : 0;
                                    break;

                                case "must-revalidate":
                                    this.MustRevalidate = true;
                                    break;

                                case "no-cache":
                                    this.NoCache = true;
                                    break;
                            }
                        }
                    }

                    //string[] options = cacheControl.ToLowerInvariant().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    //
                    //string[] kvp = options.FindOption("max-age");
                    //if (kvp != null && kvp.Length > 1)
                    //{
                    //    // Some cache proxies will return float values
                    //    double maxAge;
                    //    if (double.TryParse(kvp[1], out maxAge))
                    //        this.MaxAge = (int)maxAge;
                    //    else
                    //        this.MaxAge = 0;
                    //}
                    //else
                    //    this.MaxAge = 0;
                    //
                    //kvp = options.FindOption("stale-while-revalidate");
                    //if (kvp != null && kvp.Length == 2 && !string.IsNullOrEmpty(kvp[1]))
                    //    this.StaleWhileRevalidate = kvp[1].ToInt64(0);
                    //
                    //kvp = options.FindOption("stale-if-error");
                    //if (kvp != null && kvp.Length == 2 && !string.IsNullOrEmpty(kvp[1]))
                    //    this.StaleIfError = kvp[1].ToInt64(0);
                    //
                    //this.MustRevalidate = cacheControl.Contains("must-revalidate");
                    //this.NoCache = cacheControl.Contains("no-cache");
                }
            }

            this.Received = DateTime.UtcNow;
        }

        /// <summary>
        /// 如果下载内容失败，isInError应该为真，在这种情况下，它可能会扩展内容的新鲜度
        /// </summary>
        public bool WillExpireInTheFuture(bool isInError)
        {
            if (!IsExists())
                return false;

            // https://csswizardry.com/2019/03/cache-control-for-civilians/#no-cache
            // No-cache总是会进入网络，因为在它释放浏览器的缓存副本之前，它必须与服务器重新验证(除非服务器响应一个新的响应)。
            // 但如果服务器响应良好，网络传输的只是文件的头文件:可以从缓存中抓取文件主体，而不是重新下载。
            if (this.NoCache)
                return false;

            // http://www.w3.org/Protocols/rfc2616/rfc2616-sec13.html#sec13.2.4 :
            //  max-age指令优先于Expires指令
            if (MaxAge > 0)
            {
                // Age calculation:
                // http://www.w3.org/Protocols/rfc2616/rfc2616-sec13.html#sec13.2.3

                long apparent_age = Math.Max(0, (long)(this.Received - this.Date).TotalSeconds);
                long corrected_received_age = Math.Max(apparent_age, this.Age);
                long resident_time = (long)(DateTime.UtcNow - this.Date).TotalSeconds;
                long current_age = corrected_received_age + resident_time;

                long maxAge = this.MaxAge + (this.NoCache ? 0 : this.StaleWhileRevalidate) +
                              (isInError ? this.StaleIfError : 0);

                return current_age < maxAge || this.Expires > DateTime.UtcNow;
            }

            return this.Expires > DateTime.UtcNow;
        }

        internal void SetUpRevalidationHeaders(HTTPRequest request)
        {
            if (!IsExists())
                return;

            // 如果源服务器已经提供了一个实体标签，必须在任何缓存条件请求中使用该实体标签(使用If-Match或If-None-Match)。
            // -如果源服务器只提供了一个Last-Modified值，应该在非子域缓存条件请求中使用该值(使用If-Modified-Since)。
            // -如果源服务器同时提供了实体标签和Last-Modified值，则应该在缓存条件请求中使用这两个验证器。这允许HTTP/1.0和HTTP/1.1缓存进行适当的响应。

            if (!string.IsNullOrEmpty(ETag))
                request.SetHeader("If-None-Match", ETag);

            if (!string.IsNullOrEmpty(LastModified))
                request.SetHeader("If-Modified-Since", LastModified);
        }

        public Stream GetBodyStream(out int length)
        {
            if (!IsExists())
            {
                length = 0;
                return null;
            }

            length = BodyLength;

            LastAccess = DateTime.UtcNow;

            var stream = HttpManager.IOService.CreateFileStream(GetPath(), FileStreamModes.OpenRead);
            stream.Seek(-length, System.IO.SeekOrigin.End);

            return stream;
        }

        internal HttpResponse ReadResponseTo(HTTPRequest request)
        {
            if (!IsExists())
                return null;

            LastAccess = DateTime.UtcNow;

            using var stream = HttpManager.IOService.CreateFileStream(GetPath(), FileStreamModes.OpenRead);
            {
                var response = new HttpResponse(request, stream, request.UseStreaming, true);
                response.CacheFileInfo = this;
                response.Receive(BodyLength);

                return response;
            }
        }


        internal void Store(HttpResponse response)
        {
            if (!HTTPCacheService.IsSupported)
                return;

            string path = GetPath();

            // 路径名太长，我们不想得到异常
            if (path.Length > HttpManager.MaxPathLength)
                return;

            if (HttpManager.IOService.FileExists(path))
                Delete();

            using (var writer = HttpManager.IOService.CreateFileStream(GetPath(), FileStreamModes.Create))
            {
                writer.WriteLine("HTTP/{0}.{1} {2} {3}", response.VersionMajor, response.VersionMinor,
                    response.StatusCode, response.Message);
                foreach (var kvp in response.Headers)
                {
                    foreach (var t in kvp.Value)
                    {
                        writer.WriteLine("{0}: {1}", kvp.Key, t);
                    }
                }

                writer.WriteLine();

                writer.Write(response.Data, 0, response.Data.Length);
            }

            BodyLength = response.Data.Length;

            LastAccess = DateTime.UtcNow;

            SetUpCachingValues(response);
        }

        internal Stream GetSaveStream(HttpResponse response)
        {
            if (!HTTPCacheService.IsSupported)
                return null;

            LastAccess = DateTime.UtcNow;

            var path = GetPath();

            if (HttpManager.IOService.FileExists(path))
            {
                Delete();
            }

            // 路径名太长，我们不想得到异常
            if (path.Length > HttpManager.MaxPathLength)
                return null;

            // 首先写出报头
            using (var writer = HttpManager.IOService.CreateFileStream(GetPath(), FileStreamModes.Create))
            {
                writer.WriteLine("HTTP/1.1 {0} {1}", response.StatusCode, response.Message);
                foreach (var kvp in response.Headers)
                {
                    for (int i = 0; i < kvp.Value.Count; ++i)
                        writer.WriteLine("{0}: {1}", kvp.Key, kvp.Value[i]);
                }

                writer.WriteLine();
            }

            // 如果启用了缓存，并且响应来自缓存，并且没有设置内容长度报头，那么我们将设置一个为响应。
            if (response.IsFromCache && !response.HasHeader("content-length"))
                response.AddHeader("content-length", BodyLength.ToString());

            SetUpCachingValues(response);

            // 然后使用Append FileMode创建流
            return HttpManager.IOService.CreateFileStream(GetPath(), FileStreamModes.Append);
        }

        #endregion
    }
}

#endif
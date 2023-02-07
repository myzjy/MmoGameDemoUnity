#if !BESTHTTP_DISABLE_COOKIES

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BestHTTP.Extensions;

// ReSharper disable once CheckNamespace
namespace BestHTTP.Cookies
{
    /// <summary>
    /// 基于RFC 6265的Cookie实现(http://tools.ietf.org/html/rfc6265).
    /// </summary>
    public sealed class Cookie : IComparable<Cookie>, IEquatable<Cookie>
    {
        private const int Version = 1;

        internal Cookie()
        {
            //如果一个cookie既没有Max-Age也没有Expires属性，用户代理将保留该cookie直到“当前会话结束”(由用户代理定义)。
            IsSession = true;
            MaxAge = -1;
            LastAccess = DateTime.UtcNow;
        }

        #region IComparable<Cookie> implementation

        public int CompareTo(Cookie other)
        {
            return this.LastAccess.CompareTo(other.LastAccess);
        }

        #endregion

        public bool WillExpireInTheFuture()
        {
            // 没有从服务器发送的Expires或Max-Age值，我们将伪造返回值，因此我们不会删除新来的Cookie
            if (IsSession)
                return true;

            // 如果一个cookie同时具有Max-Age和Expires属性，则Max-Age属性具有优先级，并控制cookie的过期日期。
            return MaxAge != -1
                ? Math.Max(0, (long)(DateTime.UtcNow - Date).TotalSeconds) < MaxAge
                : Expires > DateTime.UtcNow;
        }

        /// <summary>
        /// 猜猜cookie的存储大小。
        /// </summary>
        /// <returns></returns>
        public uint GuessSize()
        {
            return (uint)((this.Name != null ? this.Name.Length * sizeof(char) : 0) +
                          (this.Value != null ? this.Value.Length * sizeof(char) : 0) +
                          (this.Domain != null ? this.Domain.Length * sizeof(char) : 0) +
                          (this.Path != null ? this.Path.Length * sizeof(char) : 0) +
                          (this.SameSite != null ? this.SameSite.Length * sizeof(char) : 0) +
                          (sizeof(long) * 4) +
                          (sizeof(bool) * 3));
        }

        public static Cookie Parse(string header, Uri defaultDomain, Logger.LoggingContext context)
        {
            Cookie cookie = new Cookie();
            try
            {
                var kbps = ParseCookieHeader(header);

                foreach (var kvp in kbps)
                {
                    switch (kvp.Key.ToLowerInvariant())
                    {
                        case "path":
                            //如果属性值为空，或者属性值的第一个字符不是%x2F("/"):让cookie-path作为默认路径。
                            cookie.Path = string.IsNullOrEmpty(kvp.Value) || !kvp.Value.StartsWith("/")
                                ? "/"
                                : cookie.Path = kvp.Value;
                            break;

                        case "domain":
                            // 如果属性值为空，则行为未定义。然而，用户代理应该完全忽略cookie-av。
                            if (string.IsNullOrEmpty(kvp.Value))
                                return null;

                            //如果属性值字符串的第一个字符是%x2E ("."):
                            //让cookie-domain作为不带%x2E(".")字符的属性值。
                            cookie.Domain = kvp.Value.StartsWith(".") ? kvp.Value.Substring(1) : kvp.Value;
                            break;

                        case "expires":
                            cookie.Expires = kvp.Value.ToDateTime(DateTime.FromBinary(0));
                            cookie.IsSession = false;
                            break;

                        case "max-age":
                            cookie.MaxAge = kvp.Value.ToInt64(-1);
                            cookie.IsSession = false;
                            break;

                        case "secure":
                            cookie.IsSecure = true;
                            break;

                        case "httponly":
                            cookie.IsHttpOnly = true;
                            break;

                        // ReSharper disable once StringLiteralTypo
                        case "samesite":
                            cookie.SameSite = kvp.Value;
                            break;

                        default:
                            // 检查name是否已经设置，以避免使用未列出的设置覆盖它
                            if (string.IsNullOrEmpty(cookie.Name))
                            {
                                cookie.Name = kvp.Key;
                                cookie.Value = kvp.Value;
                            }

                            break;
                    }
                }

                // 一些用户代理为用户提供了防止跨会话持久存储cookie的选项。
                // 当这样配置时，用户代理必须处理所有接收到的cookie，就好像persistent-flag设置为false一样。
                if (HttpManager.EnablePrivateBrowsing)
                    cookie.IsSession = true;

                // http://tools.ietf.org/html/rfc6265#section-4.1.2.3
                // 警告:某些现有用户代理将不存在的Domain属性视为存在并包含当前主机名的Domain属性。
                // 例如，如果example.com返回一个没有Domain属性的Set-Cookie报头，这些用户代理也会错误地将cookie发送到www.example.com。
                if (string.IsNullOrEmpty(cookie.Domain))
                    cookie.Domain = defaultDomain.Host;

                // http://tools.ietf.org/html/rfc6265#section-5.3 section 7:
                // 如果cookie-attribute-list中包含一个attribute-name为Path的属性，
                // 将cookie的路径设置为attribute-name为path的cookie-attribute-list中最后一个属性的attribute-value。
                // __否则，将cookie的路径设置为request-uri.__的默认路径
                if (string.IsNullOrEmpty(cookie.Path))
                    cookie.Path = defaultDomain.AbsolutePath;

                cookie.Date = cookie.LastAccess = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    StringBuilder sb = new StringBuilder();
                    sb.Append("[Cookie] ");
                    sb.Append("[method: Parse] ");
                    sb.Append("[msg|Exception] ");
                    sb.Append($"Parse - Couldn't parse header:{header}");
                    sb.Append($" exception:{ex}  {ex.StackTrace}");
                    Debug.Log(sb.ToString());
#endif
                }
            }

            return cookie;
        }

        #region Public Properties

        /// <summary>
        /// cookie的名称。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// cookie的值。
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Cookie注册的日期。
        /// </summary>
        public DateTime Date { get; internal set; }

        /// <summary>
        /// 此Cookie最后一次在请求中使用的时间。
        /// </summary>
        public DateTime LastAccess { get; set; }

        /// <summary>
        /// Expires属性表示cookie的最大生命周期，表示为cookie过期的日期和时间。
        /// 用户代理在超过指定日期之前不需要保留cookie。
        /// 事实上，由于内存压力或隐私问题，用户代理经常会清除cookie。
        /// </summary>
        private DateTime Expires { get; set; }

        /// <summary>
        /// Max-Age属性表示cookie的最大生命周期，表示为cookie过期前的秒数。
        /// 用户代理在指定的时间内不需要保留cookie。
        /// 事实上，由于内存压力或隐私问题，用户代理经常会清除cookie。
        /// </summary>
        private long MaxAge { get; set; }

        /// <summary>
        ///如果一个cookie既没有Max-Age属性，也没有Expires属性，用户代理将保留该cookie，直到“当前会话结束”。
        /// </summary>
        public bool IsSession { get; private set; }

        /// <summary>
        /// Domain属性指定cookie将发送到的主机。
        /// 例如，如果Domain属性的值是"example.com"，用户代理在向example.com、www.example.com和www.corp.example.com发出HTTP请求时，cookie头中将包含cookie。
        /// 如果服务器忽略了Domain属性，用户代理将只将cookie返回给源服务器。
        /// </summary>
        public string Domain { get; private set; }

        /// <summary>
        /// 每个cookie的范围被限制为一组路径，由Path属性控制。
        /// 如果服务器忽略了Path属性，用户代理将使用request-uri的Path组件的“目录”作为默认值。
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Secure属性将cookie的范围限制为“安全”通道(其中“安全”由用户代理定义)。
        /// 当cookie具有Secure属性时，用户代理只会在HTTP请求中包含该cookie
        /// 通过安全通道传输(通常是基于传输层安全(TLS)的HTTP)。
        /// </summary>
        public bool IsSecure { get; private set; }

        /// <summary>
        /// HttpOnly属性将cookie的范围限制为HTTP请求。
        /// 该属性特别指示用户代理在提供访问时省略cookie
        /// cookie通过“非http”API(例如web浏览器API，将cookie暴露给脚本)。
        /// </summary>
        private bool IsHttpOnly { get; set; }

        /// <summary>
        /// SameSite阻止浏览器在跨站请求时发送此cookie
        /// 主要目的是降低跨源信息泄露的风险。
        /// 它还提供了一些保护，防止跨站请求伪造攻击。该标志可能的值是lax或strict。
        /// <seealso>
        ///     <cref>https://web.dev/samesite-cookies-explained/</cref>
        /// </seealso>
        /// </summary>
        private string SameSite { get; set; }

        #endregion

        #region Public Constructors

        public Cookie(string name, string value)
            : this(name, value, "/", string.Empty)
        {
        }

        public Cookie(string name, string value, string path)
            : this(name, value, path, string.Empty)
        {
        }

        private Cookie(string name, string value, string path, string domain)
            // 调用无参数构造函数来设置默认值
            : this()
        {
            this.Name = name;
            this.Value = value;
            this.Path = path;
            this.Domain = domain;
        }

        public Cookie(Uri uri, string name, string value, DateTime expires, bool isSession = true)
            : this(name, value, uri.AbsolutePath, uri.Host)
        {
            this.Expires = expires;
            this.IsSession = isSession;
            this.Date = DateTime.UtcNow;
        }

        public Cookie(Uri uri, string name, string value, long maxAge = -1, bool isSession = true)
            : this(name, value, uri.AbsolutePath, uri.Host)
        {
            this.MaxAge = maxAge;
            this.IsSession = isSession;
            this.Date = DateTime.UtcNow;
            this.SameSite = "none";
        }

        #endregion

        #region Save & Load

        internal void SaveTo(BinaryWriter stream)
        {
            stream.Write(Version);
            stream.Write(Name ?? string.Empty);
            stream.Write(Value ?? string.Empty);
            stream.Write(Date.ToBinary());
            stream.Write(LastAccess.ToBinary());
            stream.Write(Expires.ToBinary());
            stream.Write(MaxAge);
            stream.Write(IsSession);
            stream.Write(Domain ?? string.Empty);
            stream.Write(Path ?? string.Empty);
            stream.Write(IsSecure);
            stream.Write(IsHttpOnly);
        }

        internal void LoadFrom(BinaryReader stream)
        {
            /*int version = */
            stream.ReadInt32();
            this.Name = stream.ReadString();
            this.Value = stream.ReadString();
            this.Date = DateTime.FromBinary(stream.ReadInt64());
            this.LastAccess = DateTime.FromBinary(stream.ReadInt64());
            this.Expires = DateTime.FromBinary(stream.ReadInt64());
            this.MaxAge = stream.ReadInt64();
            this.IsSession = stream.ReadBoolean();
            this.Domain = stream.ReadString();
            this.Path = stream.ReadString();
            this.IsSecure = stream.ReadBoolean();
            this.IsHttpOnly = stream.ReadBoolean();
        }

        #endregion

        #region Overrides and new Equals function

        public override string ToString()
        {
            return string.Concat(this.Name, "=", this.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return this.Equals(obj as Cookie);
        }

        public bool Equals(Cookie cookie)
        {
            if (cookie == null)
                return false;

            if (Object.ReferenceEquals(this, cookie))
                return true;

            var domain = this.Domain;
            var path = this.Path;
            return path != null &&
                   domain != null &&
                   this.Name.Equals(cookie.Name, StringComparison.Ordinal) &&
                   (domain.Equals(cookie.Domain, StringComparison.Ordinal)) &&
                   (path.Equals(cookie.Path, StringComparison.Ordinal));
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        #endregion

        #region Private Helper Functions

        private static string ReadValue(string str, ref int pos)
        {
            string result = string.Empty;
            if (str == null)
                return result;

            return str.Read(ref pos, ';');
        }

        private static List<HeaderValue> ParseCookieHeader(string str)
        {
            List<HeaderValue> result = new List<HeaderValue>();

            if (str == null)
                return result;

            int idx = 0;

            // process the rest of the text
            while (idx < str.Length)
            {
                // Read key
                string key = str.Read(ref idx, (ch) => ch != '=' && ch != ';').Trim();
                HeaderValue qp = new HeaderValue(key);

                if (idx < str.Length && str[idx - 1] == '=')
                    qp.Value = ReadValue(str, ref idx);

                result.Add(qp);
            }

            return result;
        }

        #endregion
    }
}

#endif
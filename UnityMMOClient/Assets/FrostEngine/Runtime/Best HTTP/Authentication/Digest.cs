using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP.Extensions;
using BestHTTP.PlatformSupport.Memory;

namespace BestHTTP.Authentication
{
    /// <summary>
    /// 内部类，它将从服务器接收到的所有信息存储在WWW-Authenticate中，并且需要构造一个有效的Authorization标头。基于rfc 2617 (http://tools.ietf.org/html/rfc2617).
    /// 仅在插件内部使用。
    /// </summary>
    public sealed class Digest
    {
        internal Digest(Uri uri)
        {
            this.Uri = uri;
            this.Algorithm = "md5";
        }

        /// <summary>
        /// 解析WWW-Authenticate报头的值以检索所有信息。
        /// </summary>
        // ReSharper disable once IdentifierTypo
        public void ParseChallange(string header)
        {
            // Reset some values to its defaults.
            this.Type = AuthenticationTypes.Unknown;
            this.Stale = false;
            this.Opaque = null;
            this.Ha1Sess = null;
            this.NonceCount = 0;
            this.QualityOfProtections = null;

            if (this.ProtectedUris != null)
                this.ProtectedUris.Clear();

            // Parse the header
            WWWAuthenticateHeaderParser qpl = new WWWAuthenticateHeaderParser(header);

            // Then process
            foreach (var qp in qpl.Values)
            {
                switch (qp.Key)
                {
                    case "basic":
                        this.Type = AuthenticationTypes.Basic;
                        break;
                    case "digest":
                        this.Type = AuthenticationTypes.Digest;
                        break;
                    case "realm":
                        this.Realm = qp.Value;
                        break;
                    case "domain":
                    {
                        if (string.IsNullOrEmpty(qp.Value) || qp.Value.Length == 0)
                            break;

                        this.ProtectedUris ??= new List<string>();

                        int idx = 0;
                        string val = qp.Value.Read(ref idx, ' ');
                        do
                        {
                            this.ProtectedUris.Add(val);
                            val = qp.Value.Read(ref idx, ' ');
                        } while (idx < qp.Value.Length);

                        break;
                    }
                    case "nonce":
                        this.Nonce = qp.Value;
                        break;
                    case "qop":
                        this.QualityOfProtections = qp.Value;
                        break;
                    case "stale":
                        this.Stale = bool.Parse(qp.Value);
                        break;
                    case "opaque":
                        this.Opaque = qp.Value;
                        break;
                    case "algorithm":
                        this.Algorithm = qp.Value;
                        break;
                }
            }
        }

        /// <summary>
        /// 生成可设置为授权标头的字符串。
        /// </summary>
        public string GenerateResponseHeader(HttpRequest request, Credentials credentials, bool isProxy = false)
        {
            try
            {
                switch (Type)
                {
                    case AuthenticationTypes.Basic:
                    {
                        var bytes = Encoding.UTF8.GetBytes($"{credentials.UserName}:{credentials.Password}");
                        var base64 = Convert.ToBase64String(bytes);
                        return string.Concat(
                            str0: "Basic ",
                            str1: base64);
                    }
                    case AuthenticationTypes.Digest:
                    {
                        NonceCount++;

                        string ha1;

                        // Nonce -value是一个不透明的带引号的字符串值，由客户端提供，客户端和服务器都使用它来避免选择明文攻击，
                        // 提供相互身份验证，并提供一些消息完整性保护
                        var nonce = new Random(request.GetHashCode()).Next(int.MinValue, int.MaxValue)
                            .ToString("X8");

                        var value = NonceCount.ToString("X8");
                        switch (Algorithm.TrimAndLower())
                        {
                            case "md5":
                            {
                                var str = $"{credentials.UserName}:{Realm}:{credentials.Password}";
                                ha1 = str.CalculateMD5Hash();
                            }
                                break;

                            case "md5-sess":
                            {
                                if (string.IsNullOrEmpty(this.Ha1Sess))
                                {
                                    var str = $"{credentials.UserName}:{Realm}:{credentials.Password}:{Nonce}:{value}";
                                    this.Ha1Sess = str.CalculateMD5Hash();
                                }

                                ha1 = this.Ha1Sess;
                            }
                                break;

                            default:
                                //抛出新的NotSupportedException("在Web认证中发现的不支持的哈希算法:" +算法)
                                return string.Empty;
                        }

                        // 32个十六进制数字的字符串，证明用户知道密码。根据qop值进行设置。
                        // ReSharper disable once NotAccessedVariable
                        string response = string.Empty;

                        // 服务器发送的QoP-value可以是一个支持的方法列表(如果发送了，在本例中它是null)。
                        // rfc没有指定这是一个空格或逗号分隔的列表。所以它可以是auth auth-int或者auth auth-int。
                        // 我们将首先检查较长的值("auth-int")，然后检查较短的值("auth")。如果一个匹配，我们将qop重置为准确的值。
                        string qop = this.QualityOfProtections?.TrimAndLower();

                        //当我们使用代理进行身份验证并且想要隧道请求时，我们必须使用CONNECT方法而不是请求的方法，因为代理不会知道请求本身。
                        string method = isProxy ? "CONNECT" : request.MethodType.ToString().ToUpper();

                        //当我们使用代理进行身份验证并希望隧道请求时，uri必须与我们在CONNECT请求的Host头中发送的内容相匹配。
                        var uri = isProxy
                            ? request.CurrentUri.Host + ":" + request.CurrentUri.Port
                            : request.CurrentUri.GetRequestPathAndQueryURL();

                        if (qop == null)
                        {
                            string ha2 = string.Concat(
                                str0: request.MethodType.ToString().ToUpper(),
                                str1: ":",
                                str2: request.CurrentUri.GetRequestPathAndQueryURL()).CalculateMD5Hash();
                            // ReSharper disable once RedundantAssignment
                            response = $"{ha1}:{Nonce}:{ha2}".CalculateMD5Hash();
                        }
                        else if (qop.Contains("auth-int"))
                        {
                            qop = "auth-int";

                            byte[] entityBody = request.GetEntityBody() ?? BufferPool.NoData; //string.Empty.GetASCIIBytes();

                            string ha2 = $"{method}:{uri}:{entityBody.CalculateMD5Hash()}".CalculateMD5Hash();

                            // ReSharper disable once RedundantAssignment
                            response = $"{ha1}:{Nonce}:{value}:{nonce}:{qop}:{ha2}".CalculateMD5Hash();
                        }
                        else if (qop.Contains("auth"))
                        {
                            qop = "auth";
                            string ha2 = string.Concat(method, ":", uri).CalculateMD5Hash();

                            // ReSharper disable once RedundantAssignment
                            response = $"{ha1}:{Nonce}:{value}:{nonce}:{qop}:{ha2}".CalculateMD5Hash();
                        }
                        else
                        {
                            //抛出新的NotSupportedException("Unrecognized Quality of Protection value found: " + this.QualityOfProtections);
                            return string.Empty;
                        }

                        var sb = new StringBuilder(10);
                        sb.Append("Digest username=\"{credentials.UserName}\",");
                        sb.Append("realm=\"{Realm}\", nonce=\"{Nonce}\",");
                        // ReSharper disable once StringLiteralTypo
                        sb.Append(" uri=\"{uri}\", cnonce=\"{nonce}\",");
                        sb.Append("response=\"{response}\"");


                        if (qop != null)
                        {
                            var str = string.Concat(
                                str0: ", qop=\"",
                                str1: qop,
                                str2: "\", nc=",
                                str3: value);

                            sb.Append(str);
                        }

                        if (string.IsNullOrEmpty(Opaque)) return sb.ToString();
                        // sb.Clear();

                        var result = string.Concat(
                            str0: ", opaque=\"",
                            str1: Opaque,
                            str2: "\"");
                        sb.Append(result);

                        return sb.ToString();
                    } // case "digest"的结尾
                }
            }
            catch
            {
                // ignored
            }

            return string.Empty;
        }

        public bool IsUriProtected(Uri uri)
        {
            // http://tools.ietf.org/html/rfc2617#section-3.2.1
            // 这个列表中的absoluteURI可以引用
            // 与被访问的服务器不同的服务器。客户端可以使用
            // 这个列表来确定相同uri的集合
            // 可以发送身份验证信息:包含URI的任何URI
            // 这个列表作为一个前缀(在两者都被设置为绝对之后)可能是
            // 假设在相同的保护空间。如果这个指令是省略或其值为空时，客户端应假设
            // 保护空间由响应服务器上的所有uri组成。
            var isUriProtected = string.CompareOrdinal(
                strA: uri.Host,
                strB: this.Uri.Host) == 0;
            return isUriProtected;
        }

        /// <summary>
        /// 这个摘要所绑定的Uri。
        /// </summary>
        private Uri Uri { get; set; }

        private AuthenticationTypes Type { get; set; }

        /// <summary>
        /// 要显示给用户的字符串，以便他们知道要使用哪个用户名和密码.
        /// 该字符串应该至少包含执行身份验证的主机的名称，还可以额外指示可能具有访问权限的用户集合.
        /// </summary>
        private string Realm { get; set; }

        /// <summary>
        /// 一个标志，表明来自客户端的上一个请求因为nonce值过期而被拒绝。
        /// 如果stale为TRUE(不区分大小写)，客户端可能希望使用新的加密响应重试请求，而不需要用户输入新的用户名和密码。
        /// 只有当服务器接收到一个请求，该请求的nonce无效，但该nonce具有有效的摘要时，服务器才应该将stale设置为TRUE(表示客户端知道正确的用户名/密码)。
        /// 如果stale为FALSE，或者不是TRUE，或者stale指令不存在，则用户名和/或密码无效，必须获取新的值。
        /// </summary>
        public bool Stale { get; private set; }


        /// <summary>
        ///一个服务器指定的数据字符串，应该在每次401响应时惟一地生成。
        /// 具体来说，由于字符串是作为带引号的字符串在标题行中传递的，因此不允许使用双引号字符。
        /// </summary>
        private string Nonce { get; set; }

        /// <summary>
        /// 由服务器指定的一串数据，客户端应该在具有相同保护空间的uri的后续请求的授权报头中原封不动地返回该数据。
        /// 建议该字符串为base64或data。
        /// </summary>
        private string Opaque { get; set; }

        /// <summary>
        ///一个字符串，指示用于生成摘要和校验和的一对算法。如果没有显示，则认为是“MD5”.
        /// 如果不理解算法，则应忽略该挑战(如果存在多个挑战，则使用不同的挑战)。
        /// </summary>
        private string Algorithm { get; set; }

        /// <summary>
        /// ReSharper disable once CommentTypo
        /// RFC XURI 中定义保护空间的uri列表。如果一个URI是一个abs_path，它相对于被访问服务器的规范根URL(参见上面的1.2节)。
        ///这个列表中的absoluteURI可以指向与正在访问的服务器不同的服务器。
        ///客户端可以使用这个列表来确定可以发送相同身份验证信息的uri集:
        ///任何在这个列表中有一个URI作为前缀的URI(在两者都是绝对的之后)都可以被假定在相同的保护空间中。
        ///如果省略此指令或其值为空，客户端应假设保护空间由响应服务器上的所有uri组成。
        /// </summary>
        private List<string> ProtectedUris { get; set; }

        /// <summary>
        /// 如果存在，它是一个带引号的字符串，由一个或多个令牌组成，表示服务器支持的“保护质量”值。
        /// auth表示认证。auth-int表示完整性保护认证。
        /// </summary>
        private string QualityOfProtections { get; set; }

        /// <summary>
        /// 如果发送了qop指令(见上文)，必须指定，如果服务器没有在WWW-Authenticate报头字段中发送qop指令，则必须不指定。
        /// nc值是客户端在此请求中使用nonce值发送的请求数量(包括当前请求)的十六进制计数。
        /// </summary>
        private int NonceCount { get; set; }

        /// <summary>
        /// 当Algorithm设置为“md5-sess”时，用于存储下一次生成报头时可以使用的最后一个HA1。
        /// </summary>
        private string Ha1Sess { get; set; }
    }
}
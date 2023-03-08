#if !UNITY_WEBGL || UNITY_EDITOR

#if !NETFX_CORE || UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
#endif

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
using BestHTTP.SecureProtocol.Org.BouncyCastle.Tls;
using BestHTTP.Connections.TLS;
#endif

#if NETFX_CORE
    using System.Threading.Tasks;
    using Windows.Networking.Sockets;

    using TcpClient = BestHTTP.PlatformSupport.TcpClient.WinRT.TcpClient;

    //Disable CD4014: Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
#pragma warning disable 4014
#else
using TcpClient = BestHTTP.PlatformSupport.TcpClient.General.TcpClient;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BestHTTP.Timings;
#endif

namespace BestHTTP.Connections
{
    public sealed class TCPConnector : IDisposable
    {
        public bool IsConnected
        {
            get { return this.Client != null && this.Client.Connected; }
        }

        public string NegotiatedProtocol { get; private set; }

        public TcpClient Client { get; private set; }

        public Stream TopmostStream { get; private set; }

        public Stream Stream { get; private set; }

        public bool LeaveOpen { get; set; }

        public void Dispose()
        {
            Close();
        }

        public void Connect(HttpRequest request)
        {
            string negotiatedProtocol = HttpProtocolFactory.W3CHttp1;

            Uri uri =
#if !BESTHTTP_DISABLE_PROXY
                request.HasProxy
                    ? request.Proxy.Address
                    :
#endif
                    request.CurrentUri;

            #region TCP Connection

            Client ??= new TcpClient();

            if (!Client.Connected)
            {
                Client.ConnectTimeout = request.ConnectTimeout;

#if NETFX_CORE
                Client.UseHTTPSProtocol =
#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
                    !Request.UseAlternateSSL &&
#endif
                    HTTPProtocolFactory.IsSecureProtocol(uri);
#endif
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[TCPConnector] [method: Connect(HttpRequest request)] [msg] '{request.CurrentUri.ToString()}' - Connecting to {uri.Host}:{uri.Port.ToString()}");
#endif
#if !NETFX_CORE && (!UNITY_WEBGL || UNITY_EDITOR)
                bool changed = false;
                int? sendBufferSize = null, receiveBufferSize = null;

                if (HttpManager.SendBufferSize.HasValue)
                {
                    sendBufferSize = Client.SendBufferSize;
                    Client.SendBufferSize = HttpManager.SendBufferSize.Value;
                    changed = true;
                }

                if (HttpManager.ReceiveBufferSize.HasValue)
                {
                    receiveBufferSize = Client.ReceiveBufferSize;
                    Client.ReceiveBufferSize = HttpManager.ReceiveBufferSize.Value;
                    changed = true;
                }

                if (HttpManager.Logger.Level == Logger.Loglevels.All)
                {
                    if (changed)
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        StringBuilder sb = new StringBuilder();
                        sb.Append($"'{request.CurrentUri.ToString()}'");
                        sb.Append($"- Buffer sizes changed - Send from: {sendBufferSize}");
                        sb.Append($" to: {Client.SendBufferSize},");
                        sb.Append($" Receive from: {receiveBufferSize}");
                        sb.Append($" to: {Client.ReceiveBufferSize},");
                        sb.Append($" Blocking: {Client.Client.Blocking}");

                        Debug.Log(
                            $"[TCPConnector] [method: Connect(HttpRequest request)] [msg] {sb.ToString()}");
#endif
                    }
                    else
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG

                        StringBuilder sb = new StringBuilder();
                        sb.Append($"'{request.CurrentUri.ToString()}'");
                        sb.Append($"- Buffer sizes changed - Send: {Client.SendBufferSize},");
                        sb.Append($" Receive: {Client.ReceiveBufferSize},");
                        sb.Append($" Blocking: {Client.Client.Blocking}");
                        Debug.Log($"[TCPConnector] [method: Connect(HttpRequest request)] [msg] {sb.ToString()}");
#endif
                    }
                }
#endif

#if NETFX_CORE && !UNITY_EDITOR && !ENABLE_IL2CPP
                try
                {
                    Client.Connect(uri.Host, uri.Port);
                }
                finally
                {
                    request.Timing.Add(TimingEventNames.TCP_Connection);
                }
#else
                System.Net.IPAddress[] addresses = null;
                try
                {
                    if (Client.ConnectTimeout > TimeSpan.Zero)
                    {
                        // https://forum.unity3d.com/threads/best-http-released.200006/page-37#post-3150972
                        using System.Threading.ManualResetEvent mre = new System.Threading.ManualResetEvent(false);
                        var result = System.Net.Dns.BeginGetHostAddresses(uri.Host, (res) =>
                        {
                            try
                            {
                                mre.Set();
                            }
                            catch
                            {
                            }
                        }, null);
                        bool success = mre.WaitOne(Client.ConnectTimeout);
                        if (success)
                        {
                            addresses = System.Net.Dns.EndGetHostAddresses(result);
                        }
                        else
                        {
                            throw new TimeoutException("DNS resolve timed out!");
                        }
                    }
                    else
                    {
                        addresses = System.Net.Dns.GetHostAddresses(uri.Host);
                    }
                }
                finally
                {
                    request.Timing.Add(TimingEventNames.DNS_Lookup);
                }
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[TCPConnector] [method: Connect(HttpRequest request)] [msg] '{request.CurrentUri.ToString()}' - 返回带有地址的DNS查询: {addresses?.Length ?? -1}");
#endif

                if (request.IsCancellationRequested)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError(
                        $"[TCPConnector] [method: Connect(HttpRequest request)] [msg] IsCancellationRequested(是否要求取消)");
#endif
                    throw new Exception("IsCancellationRequested");
                }

                try
                {
                    Client.Connect(addresses, uri.Port, request);
                }
                finally
                {
                    request.Timing.Add(TimingEventNames.TCP_Connection);
                }

                if (request.IsCancellationRequested)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError(
                        $"[TCPConnector] [method: Connect(HttpRequest request)] [msg|Exception] IsCancellationRequested(是否要求取消)");
#endif
                    throw new Exception("IsCancellationRequested");
                }
#endif
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[TCPConnector] [method: Connect(HttpRequest request)] [msg] Connected to {uri.Host}:{uri.Port.ToString()}");
#endif
            }
            else
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log(
                    $"[TCPConnector] [method: Connect(HttpRequest request)] [msg] Already connected to {uri.Host}:{uri.Port.ToString()}");
#endif
            }

            #endregion

            if (Stream == null)
            {
                bool isSecure = HttpProtocolFactory.IsSecureProtocol(request.CurrentUri);

                // 将流设置为Client.GetStream()以便代理(如果有的话)可以直接使用它。
                this.Stream = this.TopmostStream = Client.GetStream();

                /*if (Stream.CanTimeout)
                    Stream.ReadTimeout = Stream.WriteTimeout = (int)Request.Timeout.TotalMilliseconds;*/

#if !BESTHTTP_DISABLE_PROXY
                if (request.HasProxy)
                {
                    try
                    {
                        request.Proxy.Connect(this.Stream, request);
                    }
                    finally
                    {
                        request.Timing.Add(TimingEventNames.Proxy_Negotiation);
                    }
                }

                if (request.IsCancellationRequested)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError(
                        $"[TCPConnector] [method: Connect(HttpRequest request)] [msg|Exception] IsCancellationRequested(是否要求取消)");
#endif
                    throw new Exception("IsCancellationRequested");
                }
#endif

                // 代理连接完成后，我们可以将流设置为缓冲流。HTTPProxy需要HTTPResponse的ReadUnknownSize的原始网络流!
                this.Stream = this.TopmostStream = new BufferedReadNetworkStream(Client.GetStream(),
                    Math.Max(8 * 1024, HttpManager.ReceiveBufferSize ?? Client.ReceiveBufferSize));

                // 我们必须使用Request。这里是CurrentUri，因为uri可以是不同协议的代理uri
                if (isSecure)
                {
                    DateTime tlsNegotiationStartedAt = DateTime.Now;

                    #region SSL Upgrade

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
                    if (HttpManager.UseAlternateSSLDefaultValue)
                    {
                        var handler = new TlsClientProtocol(this.Stream);

                        List<ProtocolName> protocols = new List<ProtocolName>();
#if !BESTHTTP_DISABLE_HTTP2
                        SupportedProtocols protocol = HttpProtocolFactory.GetProtocolFromUri(request.CurrentUri);
                        if (protocol == SupportedProtocols.Http && request.IsKeepAlive)
                        {
                            // http/2 over tls (https://www.iana.org/assignments/tls-extensiontype-values/tls-extensiontype-values.xhtml#alpn-protocol-ids)
                            protocols.Add(ProtocolName.AsUtf8Encoding(HttpProtocolFactory.W3CHttp2));
                        }
#endif

                        protocols.Add(ProtocolName.AsUtf8Encoding(HttpProtocolFactory.W3CHttp1));

                        AbstractTls13Client tlsClient = null;
                        if (HttpManager.TlsClientFactory == null)
                        {
                            tlsClient = HttpManager.DefaultTlsClientFactory(request, protocols);
                        }
                        else
                        {
                            try
                            {
                                tlsClient = HttpManager.TlsClientFactory(request, protocols);
                            }
                            catch (Exception ex)
                            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                                Debug.LogError(
                                    $"[{nameof(TCPConnector)}] [method: Connect(HttpRequest request)] [msg|Exception] {nameof(HttpManager.TlsClientFactory)} ex:{ex}");
#endif
                                //这里看后续是否需要拦截，如果需要就写，不需要就不写
                            }

                            tlsClient ??= HttpManager.DefaultTlsClientFactory(request, protocols);
                        }

                        //tlsClient.LoggingContext = request.Context;
                        handler.Connect(tlsClient);

                        var applicationProtocol = tlsClient.GetNegotiatedApplicationProtocol();
                        if (!string.IsNullOrEmpty(applicationProtocol))
                        {
                            negotiatedProtocol = applicationProtocol;
                        }

                        Stream = handler.Stream;
                    }
                    else
#endif
                    {
#if !NETFX_CORE
                        SslStream sslStream = null;

                        if (HttpManager.ClientCertificationProvider == null)
                        {
                            sslStream = new SslStream(Client.GetStream(), false,
                                (sender, cert, chain, errors) => HttpManager.DefaultCertificationValidator == null ||
                                                                 HttpManager.DefaultCertificationValidator(request,
                                                                     cert, chain, errors));
                        }
                        else
                        {
                            sslStream = new SslStream(Client.GetStream(), false,
                                (sender, cert, chain, errors) => HttpManager.DefaultCertificationValidator == null ||
                                                                 HttpManager.DefaultCertificationValidator(request,
                                                                     cert, chain, errors),
                                (object sender, string targetHost, X509CertificateCollection localCertificates,
                                        X509Certificate remoteCertificate, string[] acceptableIssuers) =>
                                    HttpManager.ClientCertificationProvider(request, targetHost,
                                        localCertificates, remoteCertificate, acceptableIssuers));
                        }

                        if (!sslStream.IsAuthenticated)
                        {
                            sslStream.AuthenticateAsClient(request.CurrentUri.Host);
                        }

                        Stream = sslStream;
#else
                        Stream = Client.GetStream();
#endif
                    }

                    #endregion

                    request.Timing.Add(TimingEventNames.TLS_Negotiation, DateTime.Now - tlsNegotiationStartedAt);
                }
            }

            this.NegotiatedProtocol = negotiatedProtocol;
        }

        public void Close()
        {
            if (Client != null && !this.LeaveOpen)
            {
                try
                {
                    if (Stream != null)
                    {
                        Stream.Close();
                    }
                }
                catch
                {
                    // ignored
                }
                finally
                {
                    Stream = null;
                }

                try
                {
                    Client.Close();
                }
                catch
                {
                }
                finally
                {
                    Client = null;
                }
            }
        }
    }
}
#endif
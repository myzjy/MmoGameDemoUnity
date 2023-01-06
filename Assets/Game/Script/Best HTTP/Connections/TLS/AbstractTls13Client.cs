#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.Logger;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Tls;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Tls.Crypto;

namespace BestHTTP.Connections.TLS
{
    public abstract class AbstractTls13Client : AbstractTlsClient, TlsAuthentication
    {
        protected static readonly int[] DefaultCipherSuites = new int[]
        {
            /*
             * TLS 1.3
             */
            CipherSuite.TLS_CHACHA20_POLY1305_SHA256,
            CipherSuite.TLS_AES_256_GCM_SHA384,
            CipherSuite.TLS_AES_128_GCM_SHA256,

            /*
             * pre-TLS 1.3
             */
            CipherSuite.TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256,
            CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
            CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256,
            CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA,
            CipherSuite.TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256,
            CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256,
            CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256,
            CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA,
            CipherSuite.TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256,
            CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256,
            CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256,
            CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA,
            CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256,
            CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256,
            CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA,
        };

        protected List<ProtocolName> _protocols;

        protected HTTPRequest _request;
        protected List<ServerName> _sniServerNames;

        protected AbstractTls13Client(HTTPRequest request, List<ServerName> sniServerNames,
            List<ProtocolName> protocols, TlsCrypto crypto)
            : base(crypto)
        {
            this._request = request;

            // get the request's logging context. The context has no reference to the request, so it won't keep it in memory.
            this.Context = this._request.Context;

            this._sniServerNames = sniServerNames;
            this._protocols = protocols;
        }

        protected LoggingContext Context { get; private set; }

        public virtual TlsCredentials GetClientCredentials(CertificateRequest certificateRequest)
        {
            HttpManager.Logger.Information(nameof(AbstractTls13Client), $"{nameof(GetClientCredentials)}",
                this.Context);
            return null;
        }

        public virtual void NotifyServerCertificate(TlsServerCertificate serverCertificate)
        {
            HttpManager.Logger.Information(nameof(AbstractTls13Client), $"{nameof(NotifyServerCertificate)}",
                this.Context);
        }

        /// <summary>
        /// TCPConnector has to know what protocol got negotiated
        /// </summary>
        public string GetNegotiatedApplicationProtocol() =>
            base.m_context.SecurityParameters.ApplicationProtocol?.GetUtf8Decoding();

        // (Abstract)TLSClient facing functions

        protected override ProtocolVersion[] GetSupportedVersions() =>
            ProtocolVersion.TLSv13.DownTo(ProtocolVersion.TLSv12);

        protected override IList GetProtocolNames() => this._protocols;
        protected override IList GetSniServerNames() => this._sniServerNames;

        protected override int[] GetSupportedCipherSuites()
        {
            HttpManager.Logger.Information(nameof(AbstractTls13Client), $"{nameof(GetSupportedCipherSuites)}",
                this.Context);
            return TlsUtilities.GetSupportedCipherSuites(Crypto, DefaultCipherSuites);
        }

        // TlsAuthentication implementation
        public override TlsAuthentication GetAuthentication()
        {
            HttpManager.Logger.Information(nameof(AbstractTls13Client), $"{nameof(GetAuthentication)}", this.Context);
            return this;
        }

        public override void NotifyAlertReceived(short alertLevel, short alertDescription)
        {
            base.NotifyAlertReceived(alertLevel, alertDescription);

            HttpManager.Logger.Information(nameof(AbstractTls13Client),
                $"{nameof(NotifyAlertReceived)}({alertLevel}, {alertDescription})", this.Context);
        }

        public override void NotifyAlertRaised(short alertLevel, short alertDescription, string message,
            Exception cause)
        {
            base.NotifyAlertRaised(alertLevel, alertDescription, message, cause);

            HttpManager.Logger.Information(nameof(AbstractTls13Client),
                $"{nameof(NotifyAlertRaised)}({alertLevel}, {alertDescription}, {message}, {cause?.StackTrace})",
                this.Context);
        }

        public override void NotifyHandshakeBeginning()
        {
            HttpManager.Logger.Information(nameof(AbstractTls13Client), $"{nameof(NotifyHandshakeBeginning)}",
                this.Context);
        }

        public override void NotifyHandshakeComplete()
        {
            HttpManager.Logger.Information(nameof(AbstractTls13Client), $"{nameof(NotifyHandshakeComplete)}",
                this.Context);
            this._request = null;
        }

        public override void NotifyNewSessionTicket(NewSessionTicket newSessionTicket)
        {
            HttpManager.Logger.Information(nameof(AbstractTls13Client), $"{nameof(NotifyNewSessionTicket)}",
                this.Context);

            base.NotifyNewSessionTicket(newSessionTicket);
        }

        public override void NotifySecureRenegotiation(bool secureRenegotiation)
        {
            HttpManager.Logger.Information(nameof(AbstractTls13Client), $"{nameof(NotifySecureRenegotiation)}",
                this.Context);

            base.NotifySecureRenegotiation(secureRenegotiation);
        }

        public override void NotifySelectedCipherSuite(int selectedCipherSuite)
        {
            HttpManager.Logger.Information(nameof(AbstractTls13Client),
                $"{nameof(NotifySelectedCipherSuite)}({selectedCipherSuite})", this.Context);

            base.NotifySelectedCipherSuite(selectedCipherSuite);
        }

        public override void NotifySelectedPsk(TlsPsk selectedPsk)
        {
            HttpManager.Logger.Information(nameof(AbstractTls13Client),
                $"{nameof(NotifySelectedPsk)}({selectedPsk?.PrfAlgorithm})", this.Context);

            base.NotifySelectedPsk(selectedPsk);
        }

        public override void NotifyServerVersion(ProtocolVersion serverVersion)
        {
            HttpManager.Logger.Information(nameof(AbstractTls13Client),
                $"{nameof(NotifyServerVersion)}({serverVersion})", this.Context);

            base.NotifyServerVersion(serverVersion);
        }

        public override void NotifySessionID(byte[] sessionID)
        {
            HttpManager.Logger.Information(nameof(AbstractTls13Client), $"{nameof(NotifySessionID)}", this.Context);

            base.NotifySessionID(sessionID);
        }

        public override void NotifySessionToResume(TlsSession session)
        {
            HttpManager.Logger.Information(nameof(AbstractTls13Client), $"{nameof(NotifySessionToResume)}",
                this.Context);

            base.NotifySessionToResume(session);
        }

        public override void ProcessServerExtensions(IDictionary serverExtensions)
        {
            HttpManager.Logger.Information(nameof(AbstractTls13Client), $"{nameof(ProcessServerExtensions)}",
                this.Context);

            base.ProcessServerExtensions(serverExtensions);
        }
    }
}
#endif
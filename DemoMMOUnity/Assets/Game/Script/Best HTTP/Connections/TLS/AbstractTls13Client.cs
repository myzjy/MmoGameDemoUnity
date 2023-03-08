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

        protected HttpRequest _request;
        protected List<ServerName> _sniServerNames;

        protected AbstractTls13Client(HttpRequest request, List<ServerName> sniServerNames,
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
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            // ReSharper disable once StringLiteralTypo
            Debug.Log(
                $"[{nameof(AbstractTls13Client)}] [method:GetClientCredentials] [msg] [{nameof(GetClientCredentials)}]");
#endif
            return null;
        }

        public virtual void NotifyServerCertificate(TlsServerCertificate serverCertificate)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            // ReSharper disable once StringLiteralTypo
            Debug.Log(
                $"[{nameof(AbstractTls13Client)}] [method:NotifyServerCertificate] [msg] [{nameof(NotifyServerCertificate)}]");
#endif
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
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            // ReSharper disable once StringLiteralTypo
            Debug.Log(
                $"[{nameof(AbstractTls13Client)}] [method:GetSupportedCipherSuites] [msg] [{nameof(GetSupportedCipherSuites)}]");
#endif
            return TlsUtilities.GetSupportedCipherSuites(Crypto, DefaultCipherSuites);
        }

        // TlsAuthentication implementation
        public override TlsAuthentication GetAuthentication()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            // ReSharper disable once StringLiteralTypo
            Debug.Log(
                $"[{nameof(AbstractTls13Client)}] [method:GetAuthentication] [msg] [{nameof(GetAuthentication)}]");
#endif
            return this;
        }

        public override void NotifyAlertReceived(short alertLevel, short alertDescription)
        {
            base.NotifyAlertReceived(alertLevel, alertDescription);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            // ReSharper disable once StringLiteralTypo
            Debug.Log(
                $"[{nameof(AbstractTls13Client)}] [method:NotifyAlertReceived] [msg] {nameof(NotifyAlertReceived)}({alertLevel}, {alertDescription})");
#endif
        }

        public override void NotifyAlertRaised(short alertLevel, short alertDescription, string message,
            Exception cause)
        {
            base.NotifyAlertRaised(alertLevel, alertDescription, message, cause);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[{nameof(AbstractTls13Client)}] [method:NotifyAlertRaised] [msg] {nameof(NotifyAlertRaised)}({alertLevel}, {alertDescription}, {message}, {cause?.StackTrace})");
#endif
        }

        public override void NotifyHandshakeBeginning()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[{nameof(AbstractTls13Client)}] [method:NotifyHandshakeBeginning] [msg] [{nameof(NotifyHandshakeBeginning)}]");
#endif
        }

        public override void NotifyHandshakeComplete()
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[{nameof(AbstractTls13Client)}] [method:NotifyHandshakeComplete] [msg] [{nameof(NotifyHandshakeComplete)}]");
#endif
            this._request = null;
        }

        public override void NotifyNewSessionTicket(NewSessionTicket newSessionTicket)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[{nameof(AbstractTls13Client)}] [method:NotifyNewSessionTicket] [msg] [{nameof(NotifyNewSessionTicket)}]");
#endif
            base.NotifyNewSessionTicket(newSessionTicket);
        }

        public override void NotifySecureRenegotiation(bool secureRenegotiation)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[{nameof(AbstractTls13Client)}] [method:NotifySecureRenegotiation] [msg] [{nameof(NotifySecureRenegotiation)}]");
#endif
            base.NotifySecureRenegotiation(secureRenegotiation);
        }

        public override void NotifySelectedCipherSuite(int selectedCipherSuite)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[{nameof(AbstractTls13Client)}] [method:NotifySelectedCipherSuite] [msg] [{nameof(NotifySelectedCipherSuite)}] ({selectedCipherSuite})");
#endif
            base.NotifySelectedCipherSuite(selectedCipherSuite);
        }

        public override void NotifySelectedPsk(TlsPsk selectedPsk)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[{nameof(AbstractTls13Client)}] [method:NotifySelectedPsk] [msg] [{nameof(NotifySelectedPsk)}] ({selectedPsk?.PrfAlgorithm})");
#endif
            base.NotifySelectedPsk(selectedPsk);
        }

        public override void NotifyServerVersion(ProtocolVersion serverVersion)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[{nameof(AbstractTls13Client)}] [method:NotifyServerVersion] [msg] [{nameof(NotifyServerVersion)}] ({serverVersion})");
#endif
            base.NotifyServerVersion(serverVersion);
        }

        public override void NotifySessionID(byte[] sessionID)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[{nameof(AbstractTls13Client)}] [method:NotifySessionID] [msg] [{nameof(NotifySessionID)}]");
#endif
            base.NotifySessionID(sessionID);
        }

        public override void NotifySessionToResume(TlsSession session)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[{nameof(AbstractTls13Client)}] [method:NotifySessionToResume] [msg] [{nameof(NotifySessionToResume)}]");
#endif
            base.NotifySessionToResume(session);
        }

        public override void ProcessServerExtensions(IDictionary serverExtensions)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log(
                $"[{nameof(AbstractTls13Client)}] [method:ProcessServerExtensions] [msg] [{nameof(ProcessServerExtensions)}]");
#endif
            base.ProcessServerExtensions(serverExtensions);
        }
    }
}
#endif
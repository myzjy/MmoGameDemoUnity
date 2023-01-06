#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Cmp
{
    public class CmpCertificate
        : Asn1Encodable, IAsn1Choice
    {
        private readonly AttributeCertificate _x509V2AttrCert;
        private readonly X509CertificateStructure _x509V3PkCert;

        /**
         * Note: the addition of attribute certificates is a BC extension.
         */
        public CmpCertificate(AttributeCertificate x509V2AttrCert)
        {
            this._x509V2AttrCert = x509V2AttrCert;
        }

        public CmpCertificate(X509CertificateStructure x509V3PkCert)
        {
            if (x509V3PkCert.Version != 3)
                throw new ArgumentException("only version 3 certificates allowed", "x509V3PkCert");

            this._x509V3PkCert = x509V3PkCert;
        }

        public virtual bool IsX509V3PkCert
        {
            get { return _x509V3PkCert != null; }
        }

        public virtual X509CertificateStructure X509V3PkCert
        {
            get { return _x509V3PkCert; }
        }

        public virtual AttributeCertificate X509V2AttrCert
        {
            get { return _x509V2AttrCert; }
        }

        public static CmpCertificate GetInstance(object obj)
        {
            if (obj is CmpCertificate)
                return (CmpCertificate)obj;

            if (obj is Asn1Sequence)
                return new CmpCertificate(X509CertificateStructure.GetInstance(obj));

            if (obj is Asn1TaggedObject)
                return new CmpCertificate(AttributeCertificate.GetInstance(((Asn1TaggedObject)obj).GetObject()));

            throw new ArgumentException(
                "Invalid object: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(obj),
                "obj");
        }

        /**
         * <pre>
         * CMPCertificate ::= CHOICE {
         *            x509v3PKCert        Certificate
         *            x509v2AttrCert      [1] AttributeCertificate
         *  }
         * </pre>
         * Note: the addition of attribute certificates is a BC extension.
         *
         * @return a basic ASN.1 object representation.
         */
        public override Asn1Object ToAsn1Object()
        {
            if (_x509V2AttrCert != null)
            {
                // explicit following CMP conventions
                return new DerTaggedObject(true, 1, _x509V2AttrCert);
            }

            return _x509V3PkCert.ToAsn1Object();
        }
    }
}
#pragma warning restore
#endif
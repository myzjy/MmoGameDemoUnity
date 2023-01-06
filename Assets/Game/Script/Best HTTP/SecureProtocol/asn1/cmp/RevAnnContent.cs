#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Crmf;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Cmp
{
    public class RevAnnContent
        : Asn1Encodable
    {
        private readonly DerGeneralizedTime _badSinceDate;
        private readonly CertId _certId;
        private readonly X509Extensions _crlDetails;
        private readonly PkiStatusEncodable _status;
        private readonly DerGeneralizedTime _willBeRevokedAt;

        private RevAnnContent(Asn1Sequence seq)
        {
            _status = PkiStatusEncodable.GetInstance(seq[0]);
            _certId = CertId.GetInstance(seq[1]);
            _willBeRevokedAt = DerGeneralizedTime.GetInstance(seq[2]);
            _badSinceDate = DerGeneralizedTime.GetInstance(seq[3]);

            if (seq.Count > 4)
            {
                _crlDetails = X509Extensions.GetInstance(seq[4]);
            }
        }

        public virtual PkiStatusEncodable Status
        {
            get { return _status; }
        }

        public virtual CertId CertID
        {
            get { return _certId; }
        }

        public virtual DerGeneralizedTime WillBeRevokedAt
        {
            get { return _willBeRevokedAt; }
        }

        public virtual DerGeneralizedTime BadSinceDate
        {
            get { return _badSinceDate; }
        }

        public virtual X509Extensions CrlDetails
        {
            get { return _crlDetails; }
        }

        public static RevAnnContent GetInstance(object obj)
        {
            if (obj is RevAnnContent)
                return (RevAnnContent)obj;

            if (obj is Asn1Sequence)
                return new RevAnnContent((Asn1Sequence)obj);

            throw new ArgumentException(
                "Invalid object: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(obj),
                "obj");
        }

        /**
		 * <pre>
		 * RevAnnContent ::= SEQUENCE {
		 *       status              PKIStatus,
		 *       certId              CertId,
		 *       willBeRevokedAt     GeneralizedTime,
		 *       badSinceDate        GeneralizedTime,
		 *       crlDetails          Extensions  OPTIONAL
		 *        -- extra CRL details (e.g., crl number, reason, location, etc.)
		 * }
		 * </pre>
		 * @return a basic ASN.1 object representation.
		 */
        public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector(_status, _certId, _willBeRevokedAt, _badSinceDate);
            v.AddOptional(_crlDetails);
            return new DerSequence(v);
        }
    }
}
#pragma warning restore
#endif
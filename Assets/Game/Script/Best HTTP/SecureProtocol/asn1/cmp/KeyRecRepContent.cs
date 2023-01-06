#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Cmp
{
    public class KeyRecRepContent
        : Asn1Encodable
    {
        private readonly Asn1Sequence _caCerts;
        private readonly Asn1Sequence _keyPairHist;
        private readonly CmpCertificate _newSigCert;
        private readonly PkiStatusInfo _status;

        private KeyRecRepContent(Asn1Sequence seq)
        {
            _status = PkiStatusInfo.GetInstance(seq[0]);

            for (int pos = 1; pos < seq.Count; ++pos)
            {
                Asn1TaggedObject tObj = Asn1TaggedObject.GetInstance(seq[pos]);

                switch (tObj.TagNo)
                {
                    case 0:
                        _newSigCert = CmpCertificate.GetInstance(tObj.GetObject());
                        break;
                    case 1:
                        _caCerts = Asn1Sequence.GetInstance(tObj.GetObject());
                        break;
                    case 2:
                        _keyPairHist = Asn1Sequence.GetInstance(tObj.GetObject());
                        break;
                    default:
                        throw new ArgumentException("unknown tag number: " + tObj.TagNo, "seq");
                }
            }
        }

        public virtual PkiStatusInfo Status
        {
            get { return _status; }
        }

        public virtual CmpCertificate NewSigCert
        {
            get { return _newSigCert; }
        }

        public static KeyRecRepContent GetInstance(object obj)
        {
            if (obj is KeyRecRepContent)
                return (KeyRecRepContent)obj;

            if (obj is Asn1Sequence)
                return new KeyRecRepContent((Asn1Sequence)obj);

            throw new ArgumentException(
                "Invalid object: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(obj),
                "obj");
        }

        public virtual CmpCertificate[] GetCaCerts()
        {
            if (_caCerts == null)
                return null;

            CmpCertificate[] results = new CmpCertificate[_caCerts.Count];
            for (int i = 0; i != results.Length; ++i)
            {
                results[i] = CmpCertificate.GetInstance(_caCerts[i]);
            }

            return results;
        }

        public virtual CertifiedKeyPair[] GetKeyPairHist()
        {
            if (_keyPairHist == null)
                return null;

            CertifiedKeyPair[] results = new CertifiedKeyPair[_keyPairHist.Count];
            for (int i = 0; i != results.Length; ++i)
            {
                results[i] = CertifiedKeyPair.GetInstance(_keyPairHist[i]);
            }

            return results;
        }

        /**
		 * <pre>
		 * KeyRecRepContent ::= SEQUENCE {
		 *                         status                  PKIStatusInfo,
		 *                         newSigCert          [0] CMPCertificate OPTIONAL,
		 *                         caCerts             [1] SEQUENCE SIZE (1..MAX) OF
		 *                                                           CMPCertificate OPTIONAL,
		 *                         keyPairHist         [2] SEQUENCE SIZE (1..MAX) OF
		 *                                                           CertifiedKeyPair OPTIONAL
		 *              }
		 * </pre> 
		 * @return a basic ASN.1 object representation.
		 */
        public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector(_status);
            v.AddOptionalTagged(true, 0, _newSigCert);
            v.AddOptionalTagged(true, 1, _caCerts);
            v.AddOptionalTagged(true, 2, _keyPairHist);
            return new DerSequence(v);
        }
    }
}
#pragma warning restore
#endif
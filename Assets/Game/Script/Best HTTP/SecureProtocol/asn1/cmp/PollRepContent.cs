#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Cmp
{
    public class PollRepContent
        : Asn1Encodable
    {
        private readonly DerInteger _certReqId;
        private readonly DerInteger _checkAfter;
        private readonly PkiFreeText _reason;

        private PollRepContent(Asn1Sequence seq)
        {
            _certReqId = DerInteger.GetInstance(seq[0]);
            _checkAfter = DerInteger.GetInstance(seq[1]);

            if (seq.Count > 2)
            {
                _reason = PkiFreeText.GetInstance(seq[2]);
            }
        }

        public PollRepContent(
            DerInteger certReqId,
            DerInteger checkAfter)
        {
            this._certReqId = certReqId;
            this._checkAfter = checkAfter;
            this._reason = null;
        }

        public PollRepContent(
            DerInteger certReqId,
            DerInteger checkAfter,
            PkiFreeText reason)
        {
            this._certReqId = certReqId;
            this._checkAfter = checkAfter;
            this._reason = reason;
        }

        public virtual DerInteger CertReqID
        {
            get { return _certReqId; }
        }

        public virtual DerInteger CheckAfter
        {
            get { return _checkAfter; }
        }

        public virtual PkiFreeText Reason
        {
            get { return _reason; }
        }

        public static PollRepContent GetInstance(object obj)
        {
            if (obj is PollRepContent)
                return (PollRepContent)obj;

            if (obj is Asn1Sequence)
                return new PollRepContent((Asn1Sequence)obj);

            throw new ArgumentException(
                "Invalid object: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(obj),
                "obj");
        }

        /**
		 * <pre>
		 * PollRepContent ::= SEQUENCE OF SEQUENCE {
		 *         certReqId              INTEGER,
		 *         checkAfter             INTEGER,  -- time in seconds
		 *         reason                 PKIFreeText OPTIONAL
		 *     }
		 * </pre>
		 * @return a basic ASN.1 object representation.
		 */
        public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector(_certReqId, _checkAfter);
            v.AddOptional(_reason);
            return new DerSequence(v);
        }
    }
}
#pragma warning restore
#endif
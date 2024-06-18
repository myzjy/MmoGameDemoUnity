#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Cmp
{
    public class CaKeyUpdAnnContent
        : Asn1Encodable
    {
        private readonly CmpCertificate _newWithNew;
        private readonly CmpCertificate _newWithOld;
        private readonly CmpCertificate _oldWithNew;

        private CaKeyUpdAnnContent(Asn1Sequence seq)
        {
            _oldWithNew = CmpCertificate.GetInstance(seq[0]);
            _newWithOld = CmpCertificate.GetInstance(seq[1]);
            _newWithNew = CmpCertificate.GetInstance(seq[2]);
        }

        public virtual CmpCertificate OldWithNew
        {
            get { return _oldWithNew; }
        }

        public virtual CmpCertificate NewWithOld
        {
            get { return _newWithOld; }
        }

        public virtual CmpCertificate NewWithNew
        {
            get { return _newWithNew; }
        }

        public static CaKeyUpdAnnContent GetInstance(object obj)
        {
            if (obj is CaKeyUpdAnnContent)
                return (CaKeyUpdAnnContent)obj;

            if (obj is Asn1Sequence)
                return new CaKeyUpdAnnContent((Asn1Sequence)obj);

            throw new ArgumentException(
                "Invalid object: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(obj),
                "obj");
        }

        /**
		 * <pre>
		 * CAKeyUpdAnnContent ::= SEQUENCE {
		 *                             oldWithNew   CmpCertificate, -- old pub signed with new priv
		 *                             newWithOld   CmpCertificate, -- new pub signed with old priv
		 *                             newWithNew   CmpCertificate  -- new pub signed with new priv
		 *  }
		 * </pre>
		 * @return a basic ASN.1 object representation.
		 */
        public override Asn1Object ToAsn1Object()
        {
            return new DerSequence(_oldWithNew, _newWithOld, _newWithNew);
        }
    }
}
#pragma warning restore
#endif
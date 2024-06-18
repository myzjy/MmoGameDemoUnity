#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Cmp
{
    public class CertRepMessage
        : Asn1Encodable
    {
        private readonly Asn1Sequence _caPubs;
        private readonly Asn1Sequence _response;

        private CertRepMessage(Asn1Sequence seq)
        {
            int index = 0;

            if (seq.Count > 1)
            {
                _caPubs = Asn1Sequence.GetInstance((Asn1TaggedObject)seq[index++], true);
            }

            _response = Asn1Sequence.GetInstance(seq[index]);
        }

        public CertRepMessage(CmpCertificate[] caPubs, CertResponse[] response)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            if (caPubs != null)
            {
                this._caPubs = new DerSequence(caPubs);
            }

            this._response = new DerSequence(response);
        }

        public static CertRepMessage GetInstance(object obj)
        {
            if (obj is CertRepMessage)
                return (CertRepMessage)obj;

            if (obj is Asn1Sequence)
                return new CertRepMessage((Asn1Sequence)obj);

            throw new ArgumentException(
                "Invalid object: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(obj),
                "obj");
        }

        public virtual CmpCertificate[] GetCaPubs()
        {
            if (_caPubs == null)
                return null;

            CmpCertificate[] results = new CmpCertificate[_caPubs.Count];
            for (int i = 0; i != results.Length; ++i)
            {
                results[i] = CmpCertificate.GetInstance(_caPubs[i]);
            }

            return results;
        }

        public virtual CertResponse[] GetResponse()
        {
            CertResponse[] results = new CertResponse[_response.Count];
            for (int i = 0; i != results.Length; ++i)
            {
                results[i] = CertResponse.GetInstance(_response[i]);
            }

            return results;
        }

        /**
		 * <pre>
		 * CertRepMessage ::= SEQUENCE {
		 *                          caPubs       [1] SEQUENCE SIZE (1..MAX) OF CMPCertificate
		 *                                                                             OPTIONAL,
		 *                          response         SEQUENCE OF CertResponse
		 * }
		 * </pre>
		 * @return a basic ASN.1 object representation.
		 */
        public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector();
            v.AddOptionalTagged(true, 1, _caPubs);
            v.Add(_response);
            return new DerSequence(v);
        }
    }
}
#pragma warning restore
#endif
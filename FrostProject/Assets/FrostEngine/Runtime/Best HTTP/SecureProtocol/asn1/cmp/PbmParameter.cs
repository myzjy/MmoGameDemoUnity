#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Cmp
{
    public class PbmParameter
        : Asn1Encodable
    {
        private DerInteger _iterationCount;
        private AlgorithmIdentifier _mac;
        private AlgorithmIdentifier _owf;
        private Asn1OctetString _salt;

        private PbmParameter(Asn1Sequence seq)
        {
            _salt = Asn1OctetString.GetInstance(seq[0]);
            _owf = AlgorithmIdentifier.GetInstance(seq[1]);
            _iterationCount = DerInteger.GetInstance(seq[2]);
            _mac = AlgorithmIdentifier.GetInstance(seq[3]);
        }

        public PbmParameter(
            byte[] salt,
            AlgorithmIdentifier owf,
            int iterationCount,
            AlgorithmIdentifier mac)
            : this(new DerOctetString(salt), owf, new DerInteger(iterationCount), mac)
        {
        }

        public PbmParameter(
            Asn1OctetString salt,
            AlgorithmIdentifier owf,
            DerInteger iterationCount,
            AlgorithmIdentifier mac)
        {
            this._salt = salt;
            this._owf = owf;
            this._iterationCount = iterationCount;
            this._mac = mac;
        }

        public virtual Asn1OctetString Salt
        {
            get { return _salt; }
        }

        public virtual AlgorithmIdentifier Owf
        {
            get { return _owf; }
        }

        public virtual DerInteger IterationCount
        {
            get { return _iterationCount; }
        }

        public virtual AlgorithmIdentifier Mac
        {
            get { return _mac; }
        }

        public static PbmParameter GetInstance(object obj)
        {
            if (obj is PbmParameter)
                return (PbmParameter)obj;

            if (obj is Asn1Sequence)
                return new PbmParameter((Asn1Sequence)obj);

            throw new ArgumentException(
                "Invalid object: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(obj),
                "obj");
        }

        /**
         * <pre>
         *  PbmParameter ::= SEQUENCE {
         *                        salt                OCTET STRING,
         *                        -- note:  implementations MAY wish to limit acceptable sizes
         *                        -- of this string to values appropriate for their environment
         *                        -- in order to reduce the risk of denial-of-service attacks
         *                        owf                 AlgorithmIdentifier,
         *                        -- AlgId for a One-Way Function (SHA-1 recommended)
         *                        iterationCount      INTEGER,
         *                        -- number of times the OWF is applied
         *                        -- note:  implementations MAY wish to limit acceptable sizes
         *                        -- of this integer to values appropriate for their environment
         *                        -- in order to reduce the risk of denial-of-service attacks
         *                        mac                 AlgorithmIdentifier
         *                        -- the MAC AlgId (e.g., DES-MAC, Triple-DES-MAC [PKCS11],
         *    }   -- or HMAC [RFC2104, RFC2202])
         * </pre>
         * @return a basic ASN.1 object representation.
         */
        public override Asn1Object ToAsn1Object()
        {
            return new DerSequence(_salt, _owf, _iterationCount, _mac);
        }
    }
}
#pragma warning restore
#endif
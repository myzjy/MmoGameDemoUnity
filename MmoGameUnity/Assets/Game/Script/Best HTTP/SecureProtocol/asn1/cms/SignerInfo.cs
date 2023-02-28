#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using System.Collections;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Cms
{
    public class SignerInfo
        : Asn1Encodable
    {
        private Asn1Set _authenticatedAttributes;
        private AlgorithmIdentifier _digAlgorithm;
        private AlgorithmIdentifier _digEncryptionAlgorithm;
        private Asn1OctetString _encryptedDigest;
        private SignerIdentifier _sid;
        private Asn1Set _unauthenticatedAttributes;
        private DerInteger _version;

        public SignerInfo(
            SignerIdentifier sid,
            AlgorithmIdentifier digAlgorithm,
            Asn1Set authenticatedAttributes,
            AlgorithmIdentifier digEncryptionAlgorithm,
            Asn1OctetString encryptedDigest,
            Asn1Set unauthenticatedAttributes)
        {
            this._version = new DerInteger(sid.IsTagged ? 3 : 1);
            this._sid = sid;
            this._digAlgorithm = digAlgorithm;
            this._authenticatedAttributes = authenticatedAttributes;
            this._digEncryptionAlgorithm = digEncryptionAlgorithm;
            this._encryptedDigest = encryptedDigest;
            this._unauthenticatedAttributes = unauthenticatedAttributes;
        }

        public SignerInfo(
            SignerIdentifier sid,
            AlgorithmIdentifier digAlgorithm,
            Attributes authenticatedAttributes,
            AlgorithmIdentifier digEncryptionAlgorithm,
            Asn1OctetString encryptedDigest,
            Attributes unauthenticatedAttributes)
        {
            this._version = new DerInteger(sid.IsTagged ? 3 : 1);
            this._sid = sid;
            this._digAlgorithm = digAlgorithm;
            this._authenticatedAttributes = Asn1Set.GetInstance(authenticatedAttributes);
            this._digEncryptionAlgorithm = digEncryptionAlgorithm;
            this._encryptedDigest = encryptedDigest;
            this._unauthenticatedAttributes = Asn1Set.GetInstance(unauthenticatedAttributes);
        }


        public SignerInfo(
            Asn1Sequence seq)
        {
            IEnumerator e = seq.GetEnumerator();

            e.MoveNext();
            _version = (DerInteger)e.Current;

            e.MoveNext();
            _sid = SignerIdentifier.GetInstance(e.Current);

            e.MoveNext();
            _digAlgorithm = AlgorithmIdentifier.GetInstance(e.Current);

            e.MoveNext();
            object obj = e.Current;

            if (obj is Asn1TaggedObject)
            {
                _authenticatedAttributes = Asn1Set.GetInstance((Asn1TaggedObject)obj, false);

                e.MoveNext();
                _digEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(e.Current);
            }
            else
            {
                _authenticatedAttributes = null;
                _digEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(obj);
            }

            e.MoveNext();
            _encryptedDigest = DerOctetString.GetInstance(e.Current);

            if (e.MoveNext())
            {
                _unauthenticatedAttributes = Asn1Set.GetInstance((Asn1TaggedObject)e.Current, false);
            }
            else
            {
                _unauthenticatedAttributes = null;
            }
        }

        public DerInteger Version
        {
            get { return _version; }
        }

        public SignerIdentifier SignerID
        {
            get { return _sid; }
        }

        public Asn1Set AuthenticatedAttributes
        {
            get { return _authenticatedAttributes; }
        }

        public AlgorithmIdentifier DigestAlgorithm
        {
            get { return _digAlgorithm; }
        }

        public Asn1OctetString EncryptedDigest
        {
            get { return _encryptedDigest; }
        }

        public AlgorithmIdentifier DigestEncryptionAlgorithm
        {
            get { return _digEncryptionAlgorithm; }
        }

        public Asn1Set UnauthenticatedAttributes
        {
            get { return _unauthenticatedAttributes; }
        }

        public static SignerInfo GetInstance(
            object obj)
        {
            if (obj == null || obj is SignerInfo)
                return (SignerInfo)obj;

            if (obj is Asn1Sequence)
                return new SignerInfo((Asn1Sequence)obj);

            throw new ArgumentException(
                "Unknown object in factory: " +
                BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(obj), "obj");
        }

        /**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         *  SignerInfo ::= Sequence {
         *      version Version,
         *      SignerIdentifier sid,
         *      digestAlgorithm DigestAlgorithmIdentifier,
         *      authenticatedAttributes [0] IMPLICIT Attributes OPTIONAL,
         *      digestEncryptionAlgorithm DigestEncryptionAlgorithmIdentifier,
         *      encryptedDigest EncryptedDigest,
         *      unauthenticatedAttributes [1] IMPLICIT Attributes OPTIONAL
         *  }
         *
         *  EncryptedDigest ::= OCTET STRING
         *
         *  DigestAlgorithmIdentifier ::= AlgorithmIdentifier
         *
         *  DigestEncryptionAlgorithmIdentifier ::= AlgorithmIdentifier
         * </pre>
         */
        public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector(_version, _sid, _digAlgorithm);
            v.AddOptionalTagged(false, 0, _authenticatedAttributes);
            v.Add(_digEncryptionAlgorithm, _encryptedDigest);
            v.AddOptionalTagged(false, 1, _unauthenticatedAttributes);
            return new DerSequence(v);
        }
    }
}
#pragma warning restore
#endif
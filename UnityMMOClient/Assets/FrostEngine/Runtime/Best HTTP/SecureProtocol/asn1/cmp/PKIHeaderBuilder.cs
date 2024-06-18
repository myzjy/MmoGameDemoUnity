#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Cmp
{
    public class PkiHeaderBuilder
    {
        private PkiFreeText _freeText;
        private Asn1Sequence _generalInfo;
        private DerGeneralizedTime _messageTime;
        private AlgorithmIdentifier _protectionAlg;
        private DerInteger _pvno;
        private GeneralName _recipient;
        private Asn1OctetString _recipKid; // KeyIdentifier
        private Asn1OctetString _recipNonce;
        private GeneralName _sender;
        private Asn1OctetString _senderKid; // KeyIdentifier
        private Asn1OctetString _senderNonce;
        private Asn1OctetString _transactionID;

        public PkiHeaderBuilder(
            int pvno,
            GeneralName sender,
            GeneralName recipient)
            : this(new DerInteger(pvno), sender, recipient)
        {
        }

        private PkiHeaderBuilder(
            DerInteger pvno,
            GeneralName sender,
            GeneralName recipient)
        {
            this._pvno = pvno;
            this._sender = sender;
            this._recipient = recipient;
        }

        public virtual PkiHeaderBuilder SetMessageTime(DerGeneralizedTime time)
        {
            _messageTime = time;
            return this;
        }

        public virtual PkiHeaderBuilder SetProtectionAlg(AlgorithmIdentifier aid)
        {
            _protectionAlg = aid;
            return this;
        }

        public virtual PkiHeaderBuilder SetSenderKid(byte[] kid)
        {
            return SetSenderKid(kid == null ? null : new DerOctetString(kid));
        }

        public virtual PkiHeaderBuilder SetSenderKid(Asn1OctetString kid)
        {
            _senderKid = kid;
            return this;
        }

        public virtual PkiHeaderBuilder SetRecipKid(byte[] kid)
        {
            return SetRecipKid(kid == null ? null : new DerOctetString(kid));
        }

        public virtual PkiHeaderBuilder SetRecipKid(Asn1OctetString kid)
        {
            _recipKid = kid;
            return this;
        }

        public virtual PkiHeaderBuilder SetTransactionID(byte[] tid)
        {
            return SetTransactionID(tid == null ? null : new DerOctetString(tid));
        }

        public virtual PkiHeaderBuilder SetTransactionID(Asn1OctetString tid)
        {
            _transactionID = tid;
            return this;
        }

        public virtual PkiHeaderBuilder SetSenderNonce(byte[] nonce)
        {
            return SetSenderNonce(nonce == null ? null : new DerOctetString(nonce));
        }

        public virtual PkiHeaderBuilder SetSenderNonce(Asn1OctetString nonce)
        {
            _senderNonce = nonce;
            return this;
        }

        public virtual PkiHeaderBuilder SetRecipNonce(byte[] nonce)
        {
            return SetRecipNonce(nonce == null ? null : new DerOctetString(nonce));
        }

        public virtual PkiHeaderBuilder SetRecipNonce(Asn1OctetString nonce)
        {
            _recipNonce = nonce;
            return this;
        }

        public virtual PkiHeaderBuilder SetFreeText(PkiFreeText text)
        {
            _freeText = text;
            return this;
        }

        public virtual PkiHeaderBuilder SetGeneralInfo(InfoTypeAndValue genInfo)
        {
            return SetGeneralInfo(MakeGeneralInfoSeq(genInfo));
        }

        public virtual PkiHeaderBuilder SetGeneralInfo(InfoTypeAndValue[] genInfos)
        {
            return SetGeneralInfo(MakeGeneralInfoSeq(genInfos));
        }

        public virtual PkiHeaderBuilder SetGeneralInfo(Asn1Sequence seqOfInfoTypeAndValue)
        {
            _generalInfo = seqOfInfoTypeAndValue;
            return this;
        }

        private static Asn1Sequence MakeGeneralInfoSeq(
            InfoTypeAndValue generalInfo)
        {
            return new DerSequence(generalInfo);
        }

        private static Asn1Sequence MakeGeneralInfoSeq(
            InfoTypeAndValue[] generalInfos)
        {
            Asn1Sequence genInfoSeq = null;
            if (generalInfos != null)
            {
                Asn1EncodableVector v = new Asn1EncodableVector();
                for (int i = 0; i < generalInfos.Length; ++i)
                {
                    v.Add(generalInfos[i]);
                }

                genInfoSeq = new DerSequence(v);
            }

            return genInfoSeq;
        }

        /**
		 * <pre>
		 *  PKIHeader ::= SEQUENCE {
		 *            pvno                INTEGER     { cmp1999(1), cmp2000(2) },
		 *            sender              GeneralName,
		 *            -- identifies the sender
		 *            recipient           GeneralName,
		 *            -- identifies the intended recipient
		 *            messageTime     [0] GeneralizedTime         OPTIONAL,
		 *            -- time of production of this message (used when sender
		 *            -- believes that the transport will be "suitable"; i.e.,
		 *            -- that the time will still be meaningful upon receipt)
		 *            protectionAlg   [1] AlgorithmIdentifier     OPTIONAL,
		 *            -- algorithm used for calculation of protection bits
		 *            senderKID       [2] KeyIdentifier           OPTIONAL,
		 *            recipKID        [3] KeyIdentifier           OPTIONAL,
		 *            -- to identify specific keys used for protection
		 *            transactionID   [4] OCTET STRING            OPTIONAL,
		 *            -- identifies the transaction; i.e., this will be the same in
		 *            -- corresponding request, response, certConf, and PKIConf
		 *            -- messages
		 *            senderNonce     [5] OCTET STRING            OPTIONAL,
		 *            recipNonce      [6] OCTET STRING            OPTIONAL,
		 *            -- nonces used to provide replay protection, senderNonce
		 *            -- is inserted by the creator of this message; recipNonce
		 *            -- is a nonce previously inserted in a related message by
		 *            -- the intended recipient of this message
		 *            freeText        [7] PKIFreeText             OPTIONAL,
		 *            -- this may be used to indicate context-specific instructions
		 *            -- (this field is intended for human consumption)
		 *            generalInfo     [8] SEQUENCE SIZE (1..MAX) OF
		 *                                 InfoTypeAndValue     OPTIONAL
		 *            -- this may be used to convey context-specific information
		 *            -- (this field not primarily intended for human consumption)
		 * }
		 * </pre>
		 * @return a basic ASN.1 object representation.
		 */
        public virtual PkiHeader Build()
        {
            Asn1EncodableVector v = new Asn1EncodableVector(_pvno, _sender, _recipient);
            AddOptional(v, 0, _messageTime);
            AddOptional(v, 1, _protectionAlg);
            AddOptional(v, 2, _senderKid);
            AddOptional(v, 3, _recipKid);
            AddOptional(v, 4, _transactionID);
            AddOptional(v, 5, _senderNonce);
            AddOptional(v, 6, _recipNonce);
            AddOptional(v, 7, _freeText);
            AddOptional(v, 8, _generalInfo);

            _messageTime = null;
            _protectionAlg = null;
            _senderKid = null;
            _recipKid = null;
            _transactionID = null;
            _senderNonce = null;
            _recipNonce = null;
            _freeText = null;
            _generalInfo = null;

            return PkiHeader.GetInstance(new DerSequence(v));
        }

        private void AddOptional(Asn1EncodableVector v, int tagNo, Asn1Encodable obj)
        {
            if (obj != null)
            {
                v.Add(new DerTaggedObject(true, tagNo, obj));
            }
        }
    }
}
#pragma warning restore
#endif
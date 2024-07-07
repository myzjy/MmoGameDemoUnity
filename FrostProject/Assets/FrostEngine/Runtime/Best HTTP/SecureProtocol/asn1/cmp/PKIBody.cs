#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Crmf;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Pkcs;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Cmp
{
    public class PkiBody
        : Asn1Encodable, IAsn1Choice
    {
        public const int TypeInitReq = 0;
        public const int TypeInitRep = 1;
        public const int TypeCertReq = 2;
        public const int TypeCertRep = 3;
        public const int TypeP10CertReq = 4;
        public const int TypePopoChall = 5;
        public const int TypePopoRep = 6;
        public const int TypeKeyUpdateReq = 7;
        public const int TypeKeyUpdateRep = 8;
        public const int TypeKeyRecoveryReq = 9;
        public const int TypeKeyRecoveryRep = 10;
        public const int TypeRevocationReq = 11;
        public const int TypeRevocationRep = 12;
        public const int TypeCrossCertReq = 13;
        public const int TypeCrossCertRep = 14;
        public const int TypeCaKeyUpdateAnn = 15;
        public const int TypeCertAnn = 16;
        public const int TypeRevocationAnn = 17;
        public const int TypeCrlAnn = 18;
        public const int TypeConfirm = 19;
        public const int TypeNested = 20;
        public const int TypeGenMsg = 21;
        public const int TypeGenRep = 22;
        public const int TypeError = 23;
        public const int TypeCertConfirm = 24;
        public const int TypePollReq = 25;
        public const int TypePollRep = 26;
        private Asn1Encodable _body;

        private int _tagNo;

        private PkiBody(Asn1TaggedObject tagged)
        {
            _tagNo = tagged.TagNo;
            _body = GetBodyForType(_tagNo, tagged.GetObject());
        }

        /**
         * Creates a new PkiBody.
         * @param type one of the TYPE_* constants
         * @param content message content
         */
        public PkiBody(
            int type,
            Asn1Encodable content)
        {
            _tagNo = type;
            _body = GetBodyForType(type, content);
        }

        public virtual int Type
        {
            get { return _tagNo; }
        }

        public virtual Asn1Encodable Content
        {
            get { return _body; }
        }

        public static PkiBody GetInstance(object obj)
        {
            if (obj is PkiBody)
                return (PkiBody)obj;

            if (obj is Asn1TaggedObject)
                return new PkiBody((Asn1TaggedObject)obj);

            throw new ArgumentException(
                "Invalid object: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(obj),
                "obj");
        }

        private static Asn1Encodable GetBodyForType(
            int type,
            Asn1Encodable o)
        {
            switch (type)
            {
                case TypeInitReq:
                    return CertReqMessages.GetInstance(o);
                case TypeInitRep:
                    return CertRepMessage.GetInstance(o);
                case TypeCertReq:
                    return CertReqMessages.GetInstance(o);
                case TypeCertRep:
                    return CertRepMessage.GetInstance(o);
                case TypeP10CertReq:
                    return CertificationRequest.GetInstance(o);
                case TypePopoChall:
                    return PopoDecKeyChallContent.GetInstance(o);
                case TypePopoRep:
                    return PopoDecKeyRespContent.GetInstance(o);
                case TypeKeyUpdateReq:
                    return CertReqMessages.GetInstance(o);
                case TypeKeyUpdateRep:
                    return CertRepMessage.GetInstance(o);
                case TypeKeyRecoveryReq:
                    return CertReqMessages.GetInstance(o);
                case TypeKeyRecoveryRep:
                    return KeyRecRepContent.GetInstance(o);
                case TypeRevocationReq:
                    return RevReqContent.GetInstance(o);
                case TypeRevocationRep:
                    return RevRepContent.GetInstance(o);
                case TypeCrossCertReq:
                    return CertReqMessages.GetInstance(o);
                case TypeCrossCertRep:
                    return CertRepMessage.GetInstance(o);
                case TypeCaKeyUpdateAnn:
                    return CaKeyUpdAnnContent.GetInstance(o);
                case TypeCertAnn:
                    return CmpCertificate.GetInstance(o);
                case TypeRevocationAnn:
                    return RevAnnContent.GetInstance(o);
                case TypeCrlAnn:
                    return CrlAnnContent.GetInstance(o);
                case TypeConfirm:
                    return PkiConfirmContent.GetInstance(o);
                case TypeNested:
                    return PkiMessages.GetInstance(o);
                case TypeGenMsg:
                    return GenMsgContent.GetInstance(o);
                case TypeGenRep:
                    return GenRepContent.GetInstance(o);
                case TypeError:
                    return ErrorMsgContent.GetInstance(o);
                case TypeCertConfirm:
                    return CertConfirmContent.GetInstance(o);
                case TypePollReq:
                    return PollReqContent.GetInstance(o);
                case TypePollRep:
                    return PollRepContent.GetInstance(o);
                default:
                    throw new ArgumentException("unknown tag number: " + type, "type");
            }
        }

        /**
         * <pre>
         * PkiBody ::= CHOICE {       -- message-specific body elements
         *        ir       [0]  CertReqMessages,        --Initialization Request
         *        ip       [1]  CertRepMessage,         --Initialization Response
         *        cr       [2]  CertReqMessages,        --Certification Request
         *        cp       [3]  CertRepMessage,         --Certification Response
         *        p10cr    [4]  CertificationRequest,   --imported from [PKCS10]
         *        popdecc  [5]  POPODecKeyChallContent, --pop Challenge
         *        popdecr  [6]  POPODecKeyRespContent,  --pop Response
         *        kur      [7]  CertReqMessages,        --Key Update Request
         *        kup      [8]  CertRepMessage,         --Key Update Response
         *        krr      [9]  CertReqMessages,        --Key Recovery Request
         *        krp      [10] KeyRecRepContent,       --Key Recovery Response
         *        rr       [11] RevReqContent,          --Revocation Request
         *        rp       [12] RevRepContent,          --Revocation Response
         *        ccr      [13] CertReqMessages,        --Cross-Cert. Request
         *        ccp      [14] CertRepMessage,         --Cross-Cert. Response
         *        ckuann   [15] CAKeyUpdAnnContent,     --CA Key Update Ann.
         *        cann     [16] CertAnnContent,         --Certificate Ann.
         *        rann     [17] RevAnnContent,          --Revocation Ann.
         *        crlann   [18] CRLAnnContent,          --CRL Announcement
         *        pkiconf  [19] PKIConfirmContent,      --Confirmation
         *        nested   [20] NestedMessageContent,   --Nested Message
         *        genm     [21] GenMsgContent,          --General Message
         *        genp     [22] GenRepContent,          --General Response
         *        error    [23] ErrorMsgContent,        --Error Message
         *        certConf [24] CertConfirmContent,     --Certificate confirm
         *        pollReq  [25] PollReqContent,         --Polling request
         *        pollRep  [26] PollRepContent          --Polling response
         * }
         * </pre>
         * @return a basic ASN.1 object representation.
         */
        public override Asn1Object ToAsn1Object()
        {
            return new DerTaggedObject(true, _tagNo, _body);
        }
    }
}
#pragma warning restore
#endif
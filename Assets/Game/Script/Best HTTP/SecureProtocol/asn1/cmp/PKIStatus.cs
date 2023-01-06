#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Cmp
{
    public enum PkiStatus
    {
        Granted = 0,
        GrantedWithMods = 1,
        Rejection = 2,
        Waiting = 3,
        RevocationWarning = 4,
        RevocationNotification = 5,
        KeyUpdateWarning = 6,
    }

    public class PkiStatusEncodable
        : Asn1Encodable
    {
        public static readonly PkiStatusEncodable Granted = new PkiStatusEncodable(PkiStatus.Granted);
        public static readonly PkiStatusEncodable GrantedWithMods = new PkiStatusEncodable(PkiStatus.GrantedWithMods);
        public static readonly PkiStatusEncodable Rejection = new PkiStatusEncodable(PkiStatus.Rejection);
        public static readonly PkiStatusEncodable Waiting = new PkiStatusEncodable(PkiStatus.Waiting);

        public static readonly PkiStatusEncodable RevocationWarning =
            new PkiStatusEncodable(PkiStatus.RevocationWarning);

        public static readonly PkiStatusEncodable RevocationNotification =
            new PkiStatusEncodable(PkiStatus.RevocationNotification);

        public static readonly PkiStatusEncodable KeyUpdateWaiting = new PkiStatusEncodable(PkiStatus.KeyUpdateWarning);

        private readonly DerInteger _status;

        private PkiStatusEncodable(PkiStatus status)
            : this(new DerInteger((int)status))
        {
        }

        private PkiStatusEncodable(DerInteger status)
        {
            this._status = status;
        }

        public virtual BigInteger Value
        {
            get { return _status.Value; }
        }

        public static PkiStatusEncodable GetInstance(object obj)
        {
            if (obj is PkiStatusEncodable)
                return (PkiStatusEncodable)obj;

            if (obj is DerInteger)
                return new PkiStatusEncodable((DerInteger)obj);

            throw new ArgumentException(
                "Invalid object: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(obj),
                "obj");
        }

        public override Asn1Object ToAsn1Object()
        {
            return _status;
        }
    }
}
#pragma warning restore
#endif
#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math.Raw;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Security;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Math.EC.Custom.Djb
{
    internal class Curve25519 : AbstractFpCurve
    {
        private const int Curve25519DefaultCoords = COORD_JACOBIAN_MODIFIED;
        private const int Curve25519FeInts = 8;
        public static readonly BigInteger q = Curve25519FieldElement.Q;

        private static readonly BigInteger CA = new BigInteger(1,
            Hex.DecodeStrict("2AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA984914A144"));

        private static readonly BigInteger CB = new BigInteger(1,
            Hex.DecodeStrict("7B425ED097B425ED097B425ED097B425ED097B425ED097B4260B5E9C7710C864"));

        private static readonly ECFieldElement[] Curve25519AffineZs = new ECFieldElement[]
        {
            new Curve25519FieldElement(BigInteger.One), new Curve25519FieldElement(CA)
        };

        protected readonly Curve25519Point MInfinity;

        public Curve25519()
            : base(q)
        {
            this.MInfinity = new Curve25519Point(this, null, null);

            this.m_a = FromBigInteger(CA);
            this.m_b = FromBigInteger(CB);
            this.m_order = new BigInteger(1,
                Hex.DecodeStrict("1000000000000000000000000000000014DEF9DEA2F79CD65812631A5CF5D3ED"));
            this.m_cofactor = BigInteger.ValueOf(8);
            this.m_coord = Curve25519DefaultCoords;
        }

        public virtual BigInteger Q
        {
            get { return q; }
        }

        public override ECPoint Infinity
        {
            get { return MInfinity; }
        }

        public override int FieldSize
        {
            get { return q.BitLength; }
        }

        protected override ECCurve CloneCurve()
        {
            return new Curve25519();
        }

        public override bool SupportsCoordinateSystem(int coord)
        {
            switch (coord)
            {
                case COORD_JACOBIAN_MODIFIED:
                    return true;
                default:
                    return false;
            }
        }

        public override ECFieldElement FromBigInteger(BigInteger x)
        {
            return new Curve25519FieldElement(x);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
        {
            return new Curve25519Point(this, x, y, withCompression);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs,
            bool withCompression)
        {
            return new Curve25519Point(this, x, y, zs, withCompression);
        }

        public override ECLookupTable CreateCacheSafeLookupTable(ECPoint[] points, int off, int len)
        {
            uint[] table = new uint[len * Curve25519FeInts * 2];
            {
                int pos = 0;
                for (int i = 0; i < len; ++i)
                {
                    ECPoint p = points[off + i];
                    Nat256.Copy(((Curve25519FieldElement)p.RawXCoord).X, 0, table, pos);
                    pos += Curve25519FeInts;
                    Nat256.Copy(((Curve25519FieldElement)p.RawYCoord).X, 0, table, pos);
                    pos += Curve25519FeInts;
                }
            }

            return new Curve25519LookupTable(this, table, len);
        }

        public override ECFieldElement RandomFieldElement(SecureRandom r)
        {
            uint[] x = Nat256.Create();
            Curve25519Field.Random(r, x);
            return new Curve25519FieldElement(x);
        }

        public override ECFieldElement RandomFieldElementMult(SecureRandom r)
        {
            uint[] x = Nat256.Create();
            Curve25519Field.RandomMult(r, x);
            return new Curve25519FieldElement(x);
        }

        private class Curve25519LookupTable
            : AbstractECLookupTable
        {
            private readonly Curve25519 _mOuter;
            private readonly int _mSize;
            private readonly uint[] _mTable;

            internal Curve25519LookupTable(Curve25519 outer, uint[] table, int size)
            {
                this._mOuter = outer;
                this._mTable = table;
                this._mSize = size;
            }

            public override int Size
            {
                get { return _mSize; }
            }

            public override ECPoint Lookup(int index)
            {
                uint[] x = Nat256.Create(), y = Nat256.Create();
                int pos = 0;

                for (int i = 0; i < _mSize; ++i)
                {
                    uint mask = (uint)(((i ^ index) - 1) >> 31);

                    for (int j = 0; j < Curve25519FeInts; ++j)
                    {
                        x[j] ^= _mTable[pos + j] & mask;
                        y[j] ^= _mTable[pos + Curve25519FeInts + j] & mask;
                    }

                    pos += (Curve25519FeInts * 2);
                }

                return CreatePoint(x, y);
            }

            public override ECPoint LookupVar(int index)
            {
                uint[] x = Nat256.Create(), y = Nat256.Create();
                int pos = index * Curve25519FeInts * 2;

                for (int j = 0; j < Curve25519FeInts; ++j)
                {
                    x[j] = _mTable[pos + j];
                    y[j] = _mTable[pos + Curve25519FeInts + j];
                }

                return CreatePoint(x, y);
            }

            private ECPoint CreatePoint(uint[] x, uint[] y)
            {
                return _mOuter.CreateRawPoint(new Curve25519FieldElement(x), new Curve25519FieldElement(y),
                    Curve25519AffineZs, false);
            }
        }
    }
}
#pragma warning restore
#endif
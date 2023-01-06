#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;

// ReSharper disable once CheckNamespace
namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Math.EC.Custom.Sec
{
    internal class SecT131R2Point
        : AbstractF2mPoint
    {
        /**
         * @deprecated per-point compression property will be removed, refer {@link #getEncoded(bool)}
         */
        public SecT131R2Point(ECCurve curve, ECFieldElement x, ECFieldElement y, bool withCompression = false)
            : base(curve, x, y, withCompression)
        {
            if ((x == null) != (y == null))
                throw new ArgumentException("Exactly one of the field elements is null");
        }

        internal SecT131R2Point(ECCurve curve, ECFieldElement x, ECFieldElement y, ECFieldElement[] zs,
            bool withCompression)
            : base(curve, x, y, zs, withCompression)
        {
        }

        public override ECFieldElement YCoord
        {
            get
            {
                ECFieldElement x = RawXCoord, l = RawYCoord;

                if (this.IsInfinity || x.IsZero)
                    return l;

                // Y is actually Lambda (X + Y/X) here; convert to affine value on the fly
                ECFieldElement y = l.Add(x).Multiply(x);

                ECFieldElement z = RawZCoords[0];
                if (!z.IsOne)
                {
                    y = y.Divide(z);
                }

                return y;
            }
        }

        protected internal override bool CompressionYTilde
        {
            get
            {
                var x = this.RawXCoord;
                if (x.IsZero)
                    return false;

                var y = this.RawYCoord;

                // Y is actually Lambda (X + Y/X) here
                return y.TestBitZero() != x.TestBitZero();
            }
        }

        protected override ECPoint Detach()
        {
            return new SecT131R2Point(null, AffineXCoord, AffineYCoord);
        }

        public override ECPoint Add(ECPoint b)
        {
            if (this.IsInfinity)
                return b;
            if (b.IsInfinity)
                return this;

            ECCurve curve = this.Curve;

            ECFieldElement x1 = this.RawXCoord;
            ECFieldElement x2 = b.RawXCoord;

            if (x1.IsZero)
            {
                if (x2.IsZero)
                    return curve.Infinity;

                return b.Add(this);
            }

            ECFieldElement l1 = this.RawYCoord, z1 = this.RawZCoords[0];
            ECFieldElement l2 = b.RawYCoord, z2 = b.RawZCoords[0];

            bool z1IsOne = z1.IsOne;
            ECFieldElement u2 = x2, s2 = l2;
            if (!z1IsOne)
            {
                u2 = u2.Multiply(z1);
                s2 = s2.Multiply(z1);
            }

            bool z2IsOne = z2.IsOne;
            ECFieldElement u1 = x1, s1 = l1;
            if (!z2IsOne)
            {
                u1 = u1.Multiply(z2);
                s1 = s1.Multiply(z2);
            }

            ECFieldElement a = s1.Add(s2);
            ECFieldElement ecFieldElement = u1.Add(u2);

            if (ecFieldElement.IsZero)
            {
                if (a.IsZero)
                    return Twice();

                return curve.Infinity;
            }

            ECFieldElement x3, l3, z3;
            if (x2.IsZero)
            {
                // TODO This can probably be optimized quite a bit
                ECPoint p = this.Normalize();
                x1 = p.XCoord;
                ECFieldElement y1 = p.YCoord;

                ECFieldElement l = y1.Add(l2).Divide(x1);

                x3 = l.Square().Add(l).Add(x1).Add(curve.A);
                if (x3.IsZero)
                {
                    return new SecT131R2Point(curve, x3, curve.B.Sqrt(), IsCompressed);
                }

                ECFieldElement y3 = l.Multiply(x1.Add(x3)).Add(x3).Add(y1);
                l3 = y3.Divide(x3).Add(x3);
                z3 = curve.FromBigInteger(BigInteger.One);
            }
            else
            {
                ecFieldElement = ecFieldElement.Square();

                ECFieldElement au1 = a.Multiply(u1);
                ECFieldElement au2 = a.Multiply(u2);

                x3 = au1.Multiply(au2);
                if (x3.IsZero)
                {
                    return new SecT131R2Point(curve, x3, curve.B.Sqrt(), IsCompressed);
                }

                ECFieldElement abz2 = a.Multiply(ecFieldElement);
                if (!z2IsOne)
                {
                    abz2 = abz2.Multiply(z2);
                }

                l3 = au2.Add(ecFieldElement).SquarePlusProduct(abz2, l1.Add(z1));

                z3 = abz2;
                if (!z1IsOne)
                {
                    z3 = z3.Multiply(z1);
                }
            }

            return new SecT131R2Point(curve, x3, l3, new[] { z3 }, IsCompressed);
        }

        public override ECPoint Twice()
        {
            if (this.IsInfinity)
            {
                return this;
            }

            ECCurve curve = this.Curve;

            ECFieldElement x1 = this.RawXCoord;
            if (x1.IsZero)
            {
                // A point with X == 0 is its own additive inverse
                return curve.Infinity;
            }

            ECFieldElement l1 = this.RawYCoord, z1 = this.RawZCoords[0];

            bool z1IsOne = z1.IsOne;
            ECFieldElement l1Z1 = z1IsOne ? l1 : l1.Multiply(z1);
            ECFieldElement z1Sq = z1IsOne ? z1 : z1.Square();
            ECFieldElement a = curve.A;
            ECFieldElement aZ1Sq = z1IsOne ? a : a.Multiply(z1Sq);
            ECFieldElement t = l1.Square().Add(l1Z1).Add(aZ1Sq);
            if (t.IsZero)
            {
                return new SecT131R2Point(curve, t, curve.B.Sqrt(), IsCompressed);
            }

            ECFieldElement x3 = t.Square();
            ECFieldElement z3 = z1IsOne ? t : t.Multiply(z1Sq);

            ECFieldElement x1Z1 = z1IsOne ? x1 : x1.Multiply(z1);
            ECFieldElement l3 = x1Z1.SquarePlusProduct(t, l1Z1).Add(x3).Add(z3);

            return new SecT131R2Point(curve, x3, l3, new[] { z3 }, IsCompressed);
        }

        public override ECPoint TwicePlus(ECPoint b)
        {
            if (this.IsInfinity)
                return b;
            if (b.IsInfinity)
                return Twice();

            ECCurve curve = this.Curve;

            ECFieldElement x1 = this.RawXCoord;
            if (x1.IsZero)
            {
                // A point with X == 0 is its own additive inverse
                return b;
            }

            ECFieldElement x2 = b.RawXCoord, z2 = b.RawZCoords[0];
            if (x2.IsZero || !z2.IsOne)
            {
                return Twice().Add(b);
            }

            ECFieldElement l1 = this.RawYCoord, z1 = this.RawZCoords[0];
            ECFieldElement l2 = b.RawYCoord;

            ECFieldElement x1Sq = x1.Square();
            ECFieldElement l1Sq = l1.Square();
            ECFieldElement z1Sq = z1.Square();
            ECFieldElement l1Z1 = l1.Multiply(z1);

            ECFieldElement t = curve.A.Multiply(z1Sq).Add(l1Sq).Add(l1Z1);
            ECFieldElement l2Plus1 = l2.AddOne();
            ECFieldElement a = curve.A.Add(l2Plus1).Multiply(z1Sq).Add(l1Sq).MultiplyPlusProduct(t, x1Sq, z1Sq);
            ECFieldElement x2Z1Sq = x2.Multiply(z1Sq);
            ECFieldElement b1 = x2Z1Sq.Add(t).Square();

            if (b1.IsZero)
            {
                if (a.IsZero)
                    return b.Twice();

                return curve.Infinity;
            }

            if (a.IsZero)
            {
                return new SecT131R2Point(curve, a, curve.B.Sqrt(), IsCompressed);
            }

            ECFieldElement x3 = a.Square().Multiply(x2Z1Sq);
            ECFieldElement z3 = a.Multiply(b1).Multiply(z1Sq);
            ECFieldElement l3 = a.Add(b1).Square().MultiplyPlusProduct(t, l2Plus1, z3);

            return new SecT131R2Point(curve, x3, l3, new[] { z3 }, IsCompressed);
        }

        public override ECPoint Negate()
        {
            if (IsInfinity)
                return this;

            ECFieldElement x = this.RawXCoord;
            if (x.IsZero)
                return this;

            // L is actually Lambda (X + Y/X) here
            ECFieldElement l = this.RawYCoord, z = this.RawZCoords[0];
            return new SecT131R2Point(Curve, x, l.Add(z), new[] { z }, IsCompressed);
        }
    }
}
#pragma warning restore
#endif
#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math.Raw;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Math.EC.Custom.Djb
{
    internal class Curve25519Point
        : AbstractFpPoint
    {
        /**
         * Create a point which encodes with point compression.
         * 
         * @param curve the curve to use
         * @param x affine x co-ordinate
         * @param y affine y co-ordinate
         * 
         * @deprecated Use ECCurve.CreatePoint to construct points
         */
        public Curve25519Point(ECCurve curve, ECFieldElement x, ECFieldElement y)
            : this(curve, x, y, false)
        {
        }

        /**
         * Create a point that encodes with or without point compresion.
         * 
         * @param curve the curve to use
         * @param x affine x co-ordinate
         * @param y affine y co-ordinate
         * @param withCompression if true encode with point compression
         * 
         * @deprecated per-point compression property will be removed, refer {@link #getEncoded(bool)}
         */
        public Curve25519Point(ECCurve curve, ECFieldElement x, ECFieldElement y, bool withCompression)
            : base(curve, x, y, withCompression)
        {
            if ((x == null) != (y == null))
                throw new ArgumentException("Exactly one of the field elements is null");
        }

        internal Curve25519Point(ECCurve curve, ECFieldElement x, ECFieldElement y, ECFieldElement[] zs,
            bool withCompression)
            : base(curve, x, y, zs, withCompression)
        {
        }

        protected override ECPoint Detach()
        {
            return new Curve25519Point(null, AffineXCoord, AffineYCoord);
        }

        public override ECFieldElement GetZCoord(int index)
        {
            if (index == 1)
            {
                return GetJacobianModifiedW();
            }

            return base.GetZCoord(index);
        }

        public override ECPoint Add(ECPoint b)
        {
            if (this.IsInfinity)
                return b;
            if (b.IsInfinity)
                return this;
            if (this == b)
                return Twice();

            ECCurve curve = this.Curve;

            Curve25519FieldElement x1 = (Curve25519FieldElement)this.RawXCoord,
                y1 = (Curve25519FieldElement)this.RawYCoord,
                z1 = (Curve25519FieldElement)this.RawZCoords[0];
            Curve25519FieldElement x2 = (Curve25519FieldElement)b.RawXCoord,
                y2 = (Curve25519FieldElement)b.RawYCoord,
                z2 = (Curve25519FieldElement)b.RawZCoords[0];

            uint c;
            uint[] tt1 = Nat256.CreateExt();
            uint[] t2 = Nat256.Create();
            uint[] t3 = Nat256.Create();
            uint[] t4 = Nat256.Create();

            bool z1IsOne = z1.IsOne;
            uint[] u2, s2;
            if (z1IsOne)
            {
                u2 = x2.X;
                s2 = y2.X;
            }
            else
            {
                s2 = t3;
                Curve25519Field.Square(z1.X, s2);

                u2 = t2;
                Curve25519Field.Multiply(s2, x2.X, u2);

                Curve25519Field.Multiply(s2, z1.X, s2);
                Curve25519Field.Multiply(s2, y2.X, s2);
            }

            bool z2IsOne = z2.IsOne;
            uint[] u1, s1;
            if (z2IsOne)
            {
                u1 = x1.X;
                s1 = y1.X;
            }
            else
            {
                s1 = t4;
                Curve25519Field.Square(z2.X, s1);

                u1 = tt1;
                Curve25519Field.Multiply(s1, x1.X, u1);

                Curve25519Field.Multiply(s1, z2.X, s1);
                Curve25519Field.Multiply(s1, y1.X, s1);
            }

            uint[] h = Nat256.Create();
            Curve25519Field.Subtract(u1, u2, h);

            uint[] r = t2;
            Curve25519Field.Subtract(s1, s2, r);

            // Check if b == this or b == -this
            if (Nat256.IsZero(h))
            {
                if (Nat256.IsZero(r))
                {
                    // this == b, i.e. this must be doubled
                    return this.Twice();
                }

                // this == -b, i.e. the result is the point at infinity
                return curve.Infinity;
            }

            uint[] hSquared = Nat256.Create();
            Curve25519Field.Square(h, hSquared);

            uint[] g = Nat256.Create();
            Curve25519Field.Multiply(hSquared, h, g);

            uint[] v = t3;
            Curve25519Field.Multiply(hSquared, u1, v);

            Curve25519Field.Negate(g, g);
            Nat256.Mul(s1, g, tt1);

            c = Nat256.AddBothTo(v, v, g);
            Curve25519Field.Reduce27(c, g);

            Curve25519FieldElement x3 = new Curve25519FieldElement(t4);
            Curve25519Field.Square(r, x3.X);
            Curve25519Field.Subtract(x3.X, g, x3.X);

            Curve25519FieldElement y3 = new Curve25519FieldElement(g);
            Curve25519Field.Subtract(v, x3.X, y3.X);
            Curve25519Field.MultiplyAddToExt(y3.X, r, tt1);
            Curve25519Field.Reduce(tt1, y3.X);

            Curve25519FieldElement z3 = new Curve25519FieldElement(h);
            if (!z1IsOne)
            {
                Curve25519Field.Multiply(z3.X, z1.X, z3.X);
            }

            if (!z2IsOne)
            {
                Curve25519Field.Multiply(z3.X, z2.X, z3.X);
            }

            uint[] z3Squared = (z1IsOne && z2IsOne) ? hSquared : null;

            // TODO If the result will only be used in a subsequent addition, we don't need W3
            Curve25519FieldElement w3 = CalculateJacobianModifiedW((Curve25519FieldElement)z3, z3Squared);

            ECFieldElement[] zs = new ECFieldElement[] { z3, w3 };

            return new Curve25519Point(curve, x3, y3, zs, IsCompressed);
        }

        public override ECPoint Twice()
        {
            if (this.IsInfinity)
                return this;

            ECCurve curve = this.Curve;

            ECFieldElement y1 = this.RawYCoord;
            if (y1.IsZero)
                return curve.Infinity;

            return TwiceJacobianModified(true);
        }

        public override ECPoint TwicePlus(ECPoint b)
        {
            if (this == b)
                return ThreeTimes();
            if (this.IsInfinity)
                return b;
            if (b.IsInfinity)
                return Twice();

            ECFieldElement y1 = this.RawYCoord;
            if (y1.IsZero)
                return b;

            return TwiceJacobianModified(false).Add(b);
        }

        public override ECPoint ThreeTimes()
        {
            if (this.IsInfinity || this.RawYCoord.IsZero)
                return this;

            return TwiceJacobianModified(false).Add(this);
        }

        public override ECPoint Negate()
        {
            if (IsInfinity)
                return this;

            return new Curve25519Point(Curve, RawXCoord, RawYCoord.Negate(), RawZCoords, IsCompressed);
        }

        protected virtual Curve25519FieldElement CalculateJacobianModifiedW(Curve25519FieldElement z, uint[] zSquared)
        {
            Curve25519FieldElement a4 = (Curve25519FieldElement)this.Curve.A;
            if (z.IsOne)
                return a4;

            Curve25519FieldElement w = new Curve25519FieldElement();
            if (zSquared == null)
            {
                zSquared = w.X;
                Curve25519Field.Square(z.X, zSquared);
            }

            Curve25519Field.Square(zSquared, w.X);
            Curve25519Field.Multiply(w.X, a4.X, w.X);
            return w;
        }

        protected virtual Curve25519FieldElement GetJacobianModifiedW()
        {
            ECFieldElement[] zz = this.RawZCoords;
            Curve25519FieldElement w = (Curve25519FieldElement)zz[1];
            if (w == null)
            {
                // NOTE: Rarely, TwicePlus will result in the need for a lazy W1 calculation here
                zz[1] = w = CalculateJacobianModifiedW((Curve25519FieldElement)zz[0], null);
            }

            return w;
        }

        protected virtual Curve25519Point TwiceJacobianModified(bool calculateW)
        {
            Curve25519FieldElement x1 = (Curve25519FieldElement)this.RawXCoord,
                y1 = (Curve25519FieldElement)this.RawYCoord,
                z1 = (Curve25519FieldElement)this.RawZCoords[0],
                w1 = GetJacobianModifiedW();

            uint c;

            uint[] m = Nat256.Create();
            Curve25519Field.Square(x1.X, m);
            c = Nat256.AddBothTo(m, m, m);
            c += Nat256.AddTo(w1.X, m);
            Curve25519Field.Reduce27(c, m);

            uint[] _2Y1 = Nat256.Create();
            Curve25519Field.Twice(y1.X, _2Y1);

            uint[] _2Y1Squared = Nat256.Create();
            Curve25519Field.Multiply(_2Y1, y1.X, _2Y1Squared);

            uint[] s = Nat256.Create();
            Curve25519Field.Multiply(_2Y1Squared, x1.X, s);
            Curve25519Field.Twice(s, s);

            uint[] _8T = Nat256.Create();
            Curve25519Field.Square(_2Y1Squared, _8T);
            Curve25519Field.Twice(_8T, _8T);

            Curve25519FieldElement x3 = new Curve25519FieldElement(_2Y1Squared);
            Curve25519Field.Square(m, x3.X);
            Curve25519Field.Subtract(x3.X, s, x3.X);
            Curve25519Field.Subtract(x3.X, s, x3.X);

            Curve25519FieldElement y3 = new Curve25519FieldElement(s);
            Curve25519Field.Subtract(s, x3.X, y3.X);
            Curve25519Field.Multiply(y3.X, m, y3.X);
            Curve25519Field.Subtract(y3.X, _8T, y3.X);

            Curve25519FieldElement z3 = new Curve25519FieldElement(_2Y1);
            if (!Nat256.IsOne(z1.X))
            {
                Curve25519Field.Multiply(z3.X, z1.X, z3.X);
            }

            Curve25519FieldElement w3 = null;
            if (calculateW)
            {
                w3 = new Curve25519FieldElement(_8T);
                Curve25519Field.Multiply(w3.X, w1.X, w3.X);
                Curve25519Field.Twice(w3.X, w3.X);
            }

            return new Curve25519Point(this.Curve, x3, y3, new ECFieldElement[] { z3, w3 }, IsCompressed);
        }
    }
}
#pragma warning restore
#endif
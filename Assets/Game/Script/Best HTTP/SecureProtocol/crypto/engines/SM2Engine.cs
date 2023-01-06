#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Digests;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math.EC;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math.EC.Multiplier;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Security;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /// <summary>
    /// SM2 public key encryption engine - based on https://tools.ietf.org/html/draft-shen-sm2-ecdsa-02.
    /// </summary>
    public class Sm2Engine
    {
        private readonly IDigest _mDigest;
        private int _mCurveLength;
        private ECKeyParameters _mEcKey;
        private ECDomainParameters _mEcParams;

        private bool _mForEncryption;
        private SecureRandom _mRandom;

        public Sm2Engine()
            : this(new SM3Digest())
        {
        }

        public Sm2Engine(IDigest digest)
        {
            this._mDigest = digest;
        }

        public virtual void Init(bool forEncryption, ICipherParameters param)
        {
            this._mForEncryption = forEncryption;

            if (forEncryption)
            {
                ParametersWithRandom rParam = (ParametersWithRandom)param;

                _mEcKey = (ECKeyParameters)rParam.Parameters;
                _mEcParams = _mEcKey.Parameters;

                ECPoint s = ((ECPublicKeyParameters)_mEcKey).Q.Multiply(_mEcParams.H);
                if (s.IsInfinity)
                    throw new ArgumentException("invalid key: [h]Q at infinity");

                _mRandom = rParam.Random;
            }
            else
            {
                _mEcKey = (ECKeyParameters)param;
                _mEcParams = _mEcKey.Parameters;
            }

            _mCurveLength = (_mEcParams.Curve.FieldSize + 7) / 8;
        }

        public virtual byte[] ProcessBlock(byte[] input, int inOff, int inLen)
        {
            if (_mForEncryption)
            {
                return Encrypt(input, inOff, inLen);
            }
            else
            {
                return Decrypt(input, inOff, inLen);
            }
        }

        protected virtual ECMultiplier CreateBasePointMultiplier()
        {
            return new FixedPointCombMultiplier();
        }

        private byte[] Encrypt(byte[] input, int inOff, int inLen)
        {
            byte[] c2 = new byte[inLen];

            Array.Copy(input, inOff, c2, 0, c2.Length);

            ECMultiplier multiplier = CreateBasePointMultiplier();

            byte[] c1;
            ECPoint kPb;
            do
            {
                BigInteger k = NextK();

                ECPoint c1P = multiplier.Multiply(_mEcParams.G, k).Normalize();

                c1 = c1P.GetEncoded(false);

                kPb = ((ECPublicKeyParameters)_mEcKey).Q.Multiply(k).Normalize();

                Kdf(_mDigest, kPb, c2);
            } while (NotEncrypted(c2, input, inOff));

            AddFieldElement(_mDigest, kPb.AffineXCoord);
            _mDigest.BlockUpdate(input, inOff, inLen);
            AddFieldElement(_mDigest, kPb.AffineYCoord);

            byte[] c3 = DigestUtilities.DoFinal(_mDigest);

            return Arrays.ConcatenateAll(c1, c2, c3);
        }

        private byte[] Decrypt(byte[] input, int inOff, int inLen)
        {
            byte[] c1 = new byte[_mCurveLength * 2 + 1];

            Array.Copy(input, inOff, c1, 0, c1.Length);

            ECPoint c1P = _mEcParams.Curve.DecodePoint(c1);

            ECPoint s = c1P.Multiply(_mEcParams.H);
            if (s.IsInfinity)
                throw new InvalidCipherTextException("[h]C1 at infinity");

            c1P = c1P.Multiply(((ECPrivateKeyParameters)_mEcKey).D).Normalize();

            byte[] c2 = new byte[inLen - c1.Length - _mDigest.GetDigestSize()];

            Array.Copy(input, inOff + c1.Length, c2, 0, c2.Length);

            Kdf(_mDigest, c1P, c2);

            AddFieldElement(_mDigest, c1P.AffineXCoord);
            _mDigest.BlockUpdate(c2, 0, c2.Length);
            AddFieldElement(_mDigest, c1P.AffineYCoord);

            byte[] c3 = DigestUtilities.DoFinal(_mDigest);

            int check = 0;
            for (int i = 0; i != c3.Length; i++)
            {
                check |= c3[i] ^ input[inOff + c1.Length + c2.Length + i];
            }

            Arrays.Fill(c1, 0);
            Arrays.Fill(c3, 0);

            if (check != 0)
            {
                Arrays.Fill(c2, 0);
                throw new InvalidCipherTextException("invalid cipher text");
            }

            return c2;
        }

        private bool NotEncrypted(byte[] encData, byte[] input, int inOff)
        {
            for (int i = 0; i != encData.Length; i++)
            {
                if (encData[i] != input[inOff + i])
                {
                    return false;
                }
            }

            return true;
        }

        private void Kdf(IDigest digest, ECPoint c1, byte[] encData)
        {
            int digestSize = digest.GetDigestSize();
            byte[] buf = new byte[System.Math.Max(4, digestSize)];
            int off = 0;

            IMemoable memo = digest as IMemoable;
            IMemoable copy = null;

            if (memo != null)
            {
                AddFieldElement(digest, c1.AffineXCoord);
                AddFieldElement(digest, c1.AffineYCoord);
                copy = memo.Copy();
            }

            uint ct = 0;

            while (off < encData.Length)
            {
                if (memo != null)
                {
                    memo.Reset(copy);
                }
                else
                {
                    AddFieldElement(digest, c1.AffineXCoord);
                    AddFieldElement(digest, c1.AffineYCoord);
                }

                Pack.UInt32_To_BE(++ct, buf, 0);
                digest.BlockUpdate(buf, 0, 4);
                digest.DoFinal(buf, 0);

                int xorLen = System.Math.Min(digestSize, encData.Length - off);
                Xor(encData, buf, off, xorLen);
                off += xorLen;
            }
        }

        private void Xor(byte[] data, byte[] kdfOut, int dOff, int dRemaining)
        {
            for (int i = 0; i != dRemaining; i++)
            {
                data[dOff + i] ^= kdfOut[i];
            }
        }

        private BigInteger NextK()
        {
            int qBitLength = _mEcParams.N.BitLength;

            BigInteger k;
            do
            {
                k = new BigInteger(qBitLength, _mRandom);
            } while (k.SignValue == 0 || k.CompareTo(_mEcParams.N) >= 0);

            return k;
        }

        private void AddFieldElement(IDigest digest, ECFieldElement v)
        {
            byte[] p = v.GetEncoded();
            digest.BlockUpdate(p, 0, p.Length);
        }
    }
}
#pragma warning restore
#endif
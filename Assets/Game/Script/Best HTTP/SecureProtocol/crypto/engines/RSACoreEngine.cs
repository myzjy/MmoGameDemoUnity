#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Security;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
	* this does your basic RSA algorithm.
	*/
    public class RsaCoreEngine
        : IRsa
    {
        private int _bitSize;
        private bool _forEncryption;
        private RsaKeyParameters _key;

        /**
		* initialise the RSA engine.
		*
		* @param forEncryption true if we are encrypting, false otherwise.
		* @param param the necessary RSA key parameters.
		*/
        public virtual void Init(
            bool forEncryption,
            ICipherParameters parameters)
        {
            if (parameters is ParametersWithRandom)
            {
                parameters = ((ParametersWithRandom)parameters).Parameters;
            }

            if (!(parameters is RsaKeyParameters))
                throw new InvalidKeyException("Not an RSA key");

            this._key = (RsaKeyParameters)parameters;
            this._forEncryption = forEncryption;
            this._bitSize = _key.Modulus.BitLength;
        }

        /**
		* Return the maximum size for an input block to this engine.
		* For RSA this is always one byte less than the key size on
		* encryption, and the same length as the key size on decryption.
		*
		* @return maximum size for an input block.
		*/
        public virtual int GetInputBlockSize()
        {
            CheckInitialised();

            if (_forEncryption)
            {
                return (_bitSize - 1) / 8;
            }

            return (_bitSize + 7) / 8;
        }

        /**
		* Return the maximum size for an output block to this engine.
		* For RSA this is always one byte less than the key size on
		* decryption, and the same length as the key size on encryption.
		*
		* @return maximum size for an output block.
		*/
        public virtual int GetOutputBlockSize()
        {
            CheckInitialised();

            if (_forEncryption)
            {
                return (_bitSize + 7) / 8;
            }

            return (_bitSize - 1) / 8;
        }

        public virtual BigInteger ConvertInput(
            byte[] inBuf,
            int inOff,
            int inLen)
        {
            CheckInitialised();

            int maxLength = (_bitSize + 7) / 8;

            if (inLen > maxLength)
                throw new DataLengthException("input too large for RSA cipher.");

            BigInteger input = new BigInteger(1, inBuf, inOff, inLen);

            if (input.CompareTo(_key.Modulus) >= 0)
                throw new DataLengthException("input too large for RSA cipher.");

            return input;
        }

        public virtual byte[] ConvertOutput(
            BigInteger result)
        {
            CheckInitialised();

            byte[] output = result.ToByteArrayUnsigned();

            if (_forEncryption)
            {
                int outSize = GetOutputBlockSize();

                // TODO To avoid this, create version of BigInteger.ToByteArray that
                // writes to an existing array
                if (output.Length < outSize) // have ended up with less bytes than normal, lengthen
                {
                    byte[] tmp = new byte[outSize];
                    output.CopyTo(tmp, tmp.Length - output.Length);
                    output = tmp;
                }
            }

            return output;
        }

        public virtual BigInteger ProcessBlock(
            BigInteger input)
        {
            CheckInitialised();

            if (_key is RsaPrivateCrtKeyParameters)
            {
                //
                // we have the extra factors, use the Chinese Remainder Theorem - the author
                // wishes to express his thanks to Dirk Bonekaemper at rtsffm.com for
                // advice regarding the expression of this.
                //
                RsaPrivateCrtKeyParameters crtKey = (RsaPrivateCrtKeyParameters)_key;

                BigInteger p = crtKey.P;
                BigInteger q = crtKey.Q;
                BigInteger dP = crtKey.DP;
                BigInteger dQ = crtKey.DQ;
                BigInteger qInv = crtKey.QInv;

                BigInteger mP, mQ, h, m;

                // mP = ((input Mod p) ^ dP)) Mod p
                mP = (input.Remainder(p)).ModPow(dP, p);

                // mQ = ((input Mod q) ^ dQ)) Mod q
                mQ = (input.Remainder(q)).ModPow(dQ, q);

                // h = qInv * (mP - mQ) Mod p
                h = mP.Subtract(mQ);
                h = h.Multiply(qInv);
                h = h.Mod(p); // Mod (in Java) returns the positive residual

                // m = h * q + mQ
                m = h.Multiply(q);
                m = m.Add(mQ);

                return m;
            }

            return input.ModPow(_key.Exponent, _key.Modulus);
        }

        private void CheckInitialised()
        {
            if (_key == null)
                throw new InvalidOperationException("RSA engine not initialised");
        }
    }
}
#pragma warning restore
#endif
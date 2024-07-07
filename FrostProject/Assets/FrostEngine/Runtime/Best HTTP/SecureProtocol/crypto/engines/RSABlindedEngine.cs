#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Security;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
     * this does your basic RSA algorithm with blinding
     */
    public class RsaBlindedEngine
        : IAsymmetricBlockCipher
    {
        private readonly IRsa _core;

        private RsaKeyParameters _key;
        private SecureRandom _random;

        public RsaBlindedEngine()
            : this(new RsaCoreEngine())
        {
        }

        public RsaBlindedEngine(IRsa rsa)
        {
            this._core = rsa;
        }

        public virtual string AlgorithmName
        {
            get { return "RSA"; }
        }

        /**
         * initialise the RSA engine.
         *
         * @param forEncryption true if we are encrypting, false otherwise.
         * @param param the necessary RSA key parameters.
         */
        public virtual void Init(
            bool forEncryption,
            ICipherParameters param)
        {
            _core.Init(forEncryption, param);

            if (param is ParametersWithRandom)
            {
                ParametersWithRandom rParam = (ParametersWithRandom)param;

                this._key = (RsaKeyParameters)rParam.Parameters;

                if (_key is RsaPrivateCrtKeyParameters)
                {
                    this._random = rParam.Random;
                }
                else
                {
                    this._random = null;
                }
            }
            else
            {
                this._key = (RsaKeyParameters)param;

                if (_key is RsaPrivateCrtKeyParameters)
                {
                    this._random = new SecureRandom();
                }
                else
                {
                    this._random = null;
                }
            }
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
            return _core.GetInputBlockSize();
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
            return _core.GetOutputBlockSize();
        }

        /**
         * Process a single block using the basic RSA algorithm.
         *
         * @param inBuf the input array.
         * @param inOff the offset into the input buffer where the data starts.
         * @param inLen the length of the data to be processed.
         * @return the result of the RSA process.
         * @exception DataLengthException the input block is too large.
         */
        public virtual byte[] ProcessBlock(
            byte[] inBuf,
            int inOff,
            int inLen)
        {
            if (_key == null)
                throw new InvalidOperationException("RSA engine not initialised");

            BigInteger input = _core.ConvertInput(inBuf, inOff, inLen);

            BigInteger result;
            if (_key is RsaPrivateCrtKeyParameters)
            {
                RsaPrivateCrtKeyParameters k = (RsaPrivateCrtKeyParameters)_key;
                BigInteger e = k.PublicExponent;
                if (e != null) // can't do blinding without a public exponent
                {
                    BigInteger m = k.Modulus;
                    BigInteger r = BigIntegers.CreateRandomInRange(
                        BigInteger.One, m.Subtract(BigInteger.One), _random);

                    BigInteger blindedInput = r.ModPow(e, m).Multiply(input).Mod(m);
                    BigInteger blindedResult = _core.ProcessBlock(blindedInput);

                    BigInteger rInv = BigIntegers.ModOddInverse(m, r);
                    result = blindedResult.Multiply(rInv).Mod(m);

                    // defence against Arjen Lenstra�s CRT attack
                    if (!input.Equals(result.ModPow(e, m)))
                        throw new InvalidOperationException("RSA engine faulty decryption/signing detected");
                }
                else
                {
                    result = _core.ProcessBlock(input);
                }
            }
            else
            {
                result = _core.ProcessBlock(input);
            }

            return _core.ConvertOutput(result);
        }
    }
}
#pragma warning restore
#endif
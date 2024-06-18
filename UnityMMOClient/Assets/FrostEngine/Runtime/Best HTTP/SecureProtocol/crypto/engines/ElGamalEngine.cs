#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Security;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
	* this does your basic ElGamal algorithm.
	*/
    public class ElGamalEngine
        : IAsymmetricBlockCipher
    {
        private int _bitSize;
        private bool _forEncryption;
        private ElGamalKeyParameters _key;
        private SecureRandom _random;

        public virtual string AlgorithmName
        {
            get { return "ElGamal"; }
        }

        /**
		* initialise the ElGamal engine.
		*
		* @param forEncryption true if we are encrypting, false otherwise.
		* @param param the necessary ElGamal key parameters.
		*/
        public virtual void Init(
            bool forEncryption,
            ICipherParameters parameters)
        {
            if (parameters is ParametersWithRandom)
            {
                ParametersWithRandom p = (ParametersWithRandom)parameters;

                this._key = (ElGamalKeyParameters)p.Parameters;
                this._random = p.Random;
            }
            else
            {
                this._key = (ElGamalKeyParameters)parameters;
                this._random = new SecureRandom();
            }

            this._forEncryption = forEncryption;
            this._bitSize = _key.Parameters.P.BitLength;

            if (forEncryption)
            {
                if (!(_key is ElGamalPublicKeyParameters))
                {
                    throw new ArgumentException("ElGamalPublicKeyParameters are required for encryption.");
                }
            }
            else
            {
                if (!(_key is ElGamalPrivateKeyParameters))
                {
                    throw new ArgumentException("ElGamalPrivateKeyParameters are required for decryption.");
                }
            }
        }

        /**
		* Return the maximum size for an input block to this engine.
		* For ElGamal this is always one byte less than the size of P on
		* encryption, and twice the length as the size of P on decryption.
		*
		* @return maximum size for an input block.
		*/
        public virtual int GetInputBlockSize()
        {
            if (_forEncryption)
            {
                return (_bitSize - 1) / 8;
            }

            return 2 * ((_bitSize + 7) / 8);
        }

        /**
		* Return the maximum size for an output block to this engine.
		* For ElGamal this is always one byte less than the size of P on
		* decryption, and twice the length as the size of P on encryption.
		*
		* @return maximum size for an output block.
		*/
        public virtual int GetOutputBlockSize()
        {
            if (_forEncryption)
            {
                return 2 * ((_bitSize + 7) / 8);
            }

            return (_bitSize - 1) / 8;
        }

        /**
		* Process a single block using the basic ElGamal algorithm.
		*
		* @param in the input array.
		* @param inOff the offset into the input buffer where the data starts.
		* @param length the length of the data to be processed.
		* @return the result of the ElGamal process.
		* @exception DataLengthException the input block is too large.
		*/
        public virtual byte[] ProcessBlock(
            byte[] input,
            int inOff,
            int length)
        {
            if (_key == null)
                throw new InvalidOperationException("ElGamal engine not initialised");

            int maxLength = _forEncryption
                ? (_bitSize - 1 + 7) / 8
                : GetInputBlockSize();

            if (length > maxLength)
                throw new DataLengthException("input too large for ElGamal cipher.\n");

            BigInteger p = _key.Parameters.P;

            byte[] output;
            if (_key is ElGamalPrivateKeyParameters) // decryption
            {
                int halfLength = length / 2;
                BigInteger gamma = new BigInteger(1, input, inOff, halfLength);
                BigInteger phi = new BigInteger(1, input, inOff + halfLength, halfLength);

                ElGamalPrivateKeyParameters priv = (ElGamalPrivateKeyParameters)_key;

                // a shortcut, which generally relies on p being prime amongst other things.
                // if a problem with this shows up, check the p and g values!
                BigInteger m = gamma.ModPow(p.Subtract(BigInteger.One).Subtract(priv.X), p).Multiply(phi).Mod(p);

                output = m.ToByteArrayUnsigned();
            }
            else // encryption
            {
                BigInteger tmp = new BigInteger(1, input, inOff, length);

                if (tmp.BitLength >= p.BitLength)
                    throw new DataLengthException("input too large for ElGamal cipher.\n");


                ElGamalPublicKeyParameters pub = (ElGamalPublicKeyParameters)_key;

                BigInteger pSub2 = p.Subtract(BigInteger.Two);

                // TODO In theory, a series of 'k', 'g.ModPow(k, p)' and 'y.ModPow(k, p)' can be pre-calculated
                BigInteger k;
                do
                {
                    k = new BigInteger(p.BitLength, _random);
                } while (k.SignValue == 0 || k.CompareTo(pSub2) > 0);

                BigInteger g = _key.Parameters.G;
                BigInteger gamma = g.ModPow(k, p);
                BigInteger phi = tmp.Multiply(pub.Y.ModPow(k, p)).Mod(p);

                output = new byte[this.GetOutputBlockSize()];

                // TODO Add methods to allow writing BigInteger to existing byte array?
                byte[] out1 = gamma.ToByteArrayUnsigned();
                byte[] out2 = phi.ToByteArrayUnsigned();
                out1.CopyTo(output, output.Length / 2 - out1.Length);
                out2.CopyTo(output, output.Length - out2.Length);
            }

            return output;
        }
    }
}
#pragma warning restore
#endif
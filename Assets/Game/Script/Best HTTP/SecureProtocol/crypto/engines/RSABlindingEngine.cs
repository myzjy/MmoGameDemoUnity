#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
	* This does your basic RSA Chaum's blinding and unblinding as outlined in
	* "Handbook of Applied Cryptography", page 475. You need to use this if you are
	* trying to get another party to generate signatures without them being aware
	* of the message they are signing.
	*/
    public class RsaBlindingEngine
        : IAsymmetricBlockCipher
    {
        private readonly IRsa _core;
        private BigInteger _blindingFactor;

        private bool _forEncryption;

        private RsaKeyParameters _key;

        public RsaBlindingEngine()
            : this(new RsaCoreEngine())
        {
        }

        public RsaBlindingEngine(IRsa rsa)
        {
            this._core = rsa;
        }

        public virtual string AlgorithmName
        {
            get { return "RSA"; }
        }

        /**
		* Initialise the blinding engine.
		*
		* @param forEncryption true if we are encrypting (blinding), false otherwise.
		* @param param         the necessary RSA key parameters.
		*/
        public virtual void Init(
            bool forEncryption,
            ICipherParameters param)
        {
            RsaBlindingParameters p;

            if (param is ParametersWithRandom)
            {
                ParametersWithRandom rParam = (ParametersWithRandom)param;

                p = (RsaBlindingParameters)rParam.Parameters;
            }
            else
            {
                p = (RsaBlindingParameters)param;
            }

            _core.Init(forEncryption, p.PublicKey);

            this._forEncryption = forEncryption;
            this._key = p.PublicKey;
            this._blindingFactor = p.BlindingFactor;
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
		* Process a single block using the RSA blinding algorithm.
		*
		* @param in    the input array.
		* @param inOff the offset into the input buffer where the data starts.
		* @param inLen the length of the data to be processed.
		* @return the result of the RSA process.
		* @throws DataLengthException the input block is too large.
		*/
        public virtual byte[] ProcessBlock(
            byte[] inBuf,
            int inOff,
            int inLen)
        {
            BigInteger msg = _core.ConvertInput(inBuf, inOff, inLen);

            if (_forEncryption)
            {
                msg = BlindMessage(msg);
            }
            else
            {
                msg = UnblindMessage(msg);
            }

            return _core.ConvertOutput(msg);
        }

        /*
        * Blind message with the blind factor.
        */
        private BigInteger BlindMessage(
            BigInteger msg)
        {
            BigInteger blindMsg = _blindingFactor;
            blindMsg = msg.Multiply(blindMsg.ModPow(_key.Exponent, _key.Modulus));
            blindMsg = blindMsg.Mod(_key.Modulus);

            return blindMsg;
        }

        /*
        * Unblind the message blinded with the blind factor.
        */
        private BigInteger UnblindMessage(
            BigInteger blindedMsg)
        {
            BigInteger m = _key.Modulus;
            BigInteger msg = blindedMsg;
            BigInteger blindFactorInverse = BigIntegers.ModOddInverse(m, _blindingFactor);
            msg = msg.Multiply(blindFactorInverse);
            msg = msg.Mod(m);

            return msg;
        }
    }
}
#pragma warning restore
#endif
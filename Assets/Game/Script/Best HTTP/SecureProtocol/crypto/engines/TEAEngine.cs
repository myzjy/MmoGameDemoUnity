#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
	* An TEA engine.
	*/
    public class TeaEngine
        : IBlockCipher
    {
        private const int
            Rounds = 32,
            BlockSize = 8;
//			key_size	= 16,

        private const uint
            Delta = 0x9E3779B9,
            DSum = 0xC6EF3720; // sum on decrypt

        /*
        * the expanded key array of 4 subkeys
        */
        private uint _a, _b, _c, _d;
        private bool _forEncryption;
        private bool _initialised;

        /**
		* Create an instance of the TEA encryption algorithm
		* and set some defaults
		*/
        public TeaEngine()
        {
            _initialised = false;
        }

        public virtual string AlgorithmName
        {
            get { return "TEA"; }
        }

        public virtual bool IsPartialBlockOkay
        {
            get { return false; }
        }

        public virtual int GetBlockSize()
        {
            return BlockSize;
        }

        /**
		* initialise
		*
		* @param forEncryption whether or not we are for encryption.
		* @param params the parameters required to set up the cipher.
		* @exception ArgumentException if the params argument is
		* inappropriate.
		*/
        public virtual void Init(
            bool forEncryption,
            ICipherParameters parameters)
        {
            if (!(parameters is KeyParameter))
            {
                throw new ArgumentException("invalid parameter passed to TEA init - "
                                            + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(
                                                parameters));
            }

            _forEncryption = forEncryption;
            _initialised = true;

            KeyParameter p = (KeyParameter)parameters;

            SetKey(p.GetKey());
        }

        public virtual int ProcessBlock(
            byte[] inBytes,
            int inOff,
            byte[] outBytes,
            int outOff)
        {
            if (!_initialised)
                throw new InvalidOperationException(AlgorithmName + " not initialised");

            Check.DataLength(inBytes, inOff, BlockSize, "input buffer too short");
            Check.OutputLength(outBytes, outOff, BlockSize, "output buffer too short");

            return _forEncryption
                ? EncryptBlock(inBytes, inOff, outBytes, outOff)
                : DecryptBlock(inBytes, inOff, outBytes, outOff);
        }

        public virtual void Reset()
        {
        }

        /**
		* Re-key the cipher.
		*
		* @param  key  the key to be used
		*/
        private void SetKey(
            byte[] key)
        {
            _a = Pack.BE_To_UInt32(key, 0);
            _b = Pack.BE_To_UInt32(key, 4);
            _c = Pack.BE_To_UInt32(key, 8);
            _d = Pack.BE_To_UInt32(key, 12);
        }

        private int EncryptBlock(
            byte[] inBytes,
            int inOff,
            byte[] outBytes,
            int outOff)
        {
            // Pack bytes into integers
            uint v0 = Pack.BE_To_UInt32(inBytes, inOff);
            uint v1 = Pack.BE_To_UInt32(inBytes, inOff + 4);

            uint sum = 0;

            for (int i = 0; i != Rounds; i++)
            {
                sum += Delta;
                v0 += ((v1 << 4) + _a) ^ (v1 + sum) ^ ((v1 >> 5) + _b);
                v1 += ((v0 << 4) + _c) ^ (v0 + sum) ^ ((v0 >> 5) + _d);
            }

            Pack.UInt32_To_BE(v0, outBytes, outOff);
            Pack.UInt32_To_BE(v1, outBytes, outOff + 4);

            return BlockSize;
        }

        private int DecryptBlock(
            byte[] inBytes,
            int inOff,
            byte[] outBytes,
            int outOff)
        {
            // Pack bytes into integers
            uint v0 = Pack.BE_To_UInt32(inBytes, inOff);
            uint v1 = Pack.BE_To_UInt32(inBytes, inOff + 4);

            uint sum = DSum;

            for (int i = 0; i != Rounds; i++)
            {
                v1 -= ((v0 << 4) + _c) ^ (v0 + sum) ^ ((v0 >> 5) + _d);
                v0 -= ((v1 << 4) + _a) ^ (v1 + sum) ^ ((v1 >> 5) + _b);
                sum -= Delta;
            }

            Pack.UInt32_To_BE(v0, outBytes, outOff);
            Pack.UInt32_To_BE(v1, outBytes, outOff + 4);

            return BlockSize;
        }
    }
}
#pragma warning restore
#endif
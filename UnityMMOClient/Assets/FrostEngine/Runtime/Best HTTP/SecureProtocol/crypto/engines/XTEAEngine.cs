#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
	* An XTEA engine.
	*/
    public class XteaEngine
        : IBlockCipher
    {
        private const int
            Rounds = 32,
            BlockSize = 8,
//			key_size	= 16,
            Delta = unchecked((int)0x9E3779B9);

        private bool _initialised, _forEncryption;

        /*
        * the expanded key array of 4 subkeys
        */
        private uint[] _s = new uint[4],
            _sum0 = new uint[32],
            _sum1 = new uint[32];

        /**
		* Create an instance of the TEA encryption algorithm
		* and set some defaults
		*/
        public XteaEngine()
        {
            _initialised = false;
        }

        public virtual string AlgorithmName
        {
            get { return "XTEA"; }
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
            int i, j;
            for (i = j = 0; i < 4; i++, j += 4)
            {
                _s[i] = Pack.BE_To_UInt32(key, j);
            }

            for (i = j = 0; i < Rounds; i++)
            {
                _sum0[i] = ((uint)j + _s[j & 3]);
                j += Delta;
                _sum1[i] = ((uint)j + _s[j >> 11 & 3]);
            }
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

            for (int i = 0; i < Rounds; i++)
            {
                v0 += ((v1 << 4 ^ v1 >> 5) + v1) ^ _sum0[i];
                v1 += ((v0 << 4 ^ v0 >> 5) + v0) ^ _sum1[i];
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

            for (int i = Rounds - 1; i >= 0; i--)
            {
                v1 -= ((v0 << 4 ^ v0 >> 5) + v0) ^ _sum1[i];
                v0 -= ((v1 << 4 ^ v1 >> 5) + v1) ^ _sum0[i];
            }

            Pack.UInt32_To_BE(v0, outBytes, outOff);
            Pack.UInt32_To_BE(v1, outBytes, outOff + 4);

            return BlockSize;
        }
    }
}
#pragma warning restore
#endif
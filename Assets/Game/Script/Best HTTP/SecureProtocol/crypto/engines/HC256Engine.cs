#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
	* HC-256 is a software-efficient stream cipher created by Hongjun Wu. It 
	* generates keystream from a 256-bit secret key and a 256-bit initialization 
	* vector.
	* <p>
	* http://www.ecrypt.eu.org/stream/p3ciphers/hc/hc256_p3.pdf
	* </p><p>
	* Its brother, HC-128, is a third phase candidate in the eStream contest.
	* The algorithm is patent-free. No attacks are known as of today (April 2007). 
	* See
	* 
	* http://www.ecrypt.eu.org/stream/hcp3.html
	* </p>
	*/
    public class Hc256Engine
        : IStreamCipher
    {
        private byte[] _buf = new byte[4];
        private uint _cnt = 0;
        private int _idx = 0;
        private bool _initialised;

        private byte[] _key, _iv;
        private uint[] _p = new uint[1024];
        private uint[] _q = new uint[1024];

        public virtual string AlgorithmName
        {
            get { return "HC-256"; }
        }

        /**
		* Initialise a HC-256 cipher.
		*
		* @param forEncryption whether or not we are for encryption. Irrelevant, as
		*                      encryption and decryption are the same.
		* @param params        the parameters required to set up the cipher.
		* @throws ArgumentException if the params argument is
		*                                  inappropriate (ie. the key is not 256 bit long).
		*/
        public virtual void Init(
            bool forEncryption,
            ICipherParameters parameters)
        {
            ICipherParameters keyParam = parameters;

            if (parameters is ParametersWithIV)
            {
                _iv = ((ParametersWithIV)parameters).GetIV();
                keyParam = ((ParametersWithIV)parameters).Parameters;
            }
            else
            {
                _iv = new byte[0];
            }

            if (keyParam is KeyParameter)
            {
                _key = ((KeyParameter)keyParam).GetKey();
                Init();
            }
            else
            {
                throw new ArgumentException(
                    "Invalid parameter passed to HC256 init - " +
                    BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(parameters),
                    "parameters");
            }

            _initialised = true;
        }

        public virtual void ProcessBytes(
            byte[] input,
            int inOff,
            int len,
            byte[] output,
            int outOff)
        {
            if (!_initialised)
                throw new InvalidOperationException(AlgorithmName + " not initialised");

            Check.DataLength(input, inOff, len, "input buffer too short");
            Check.OutputLength(output, outOff, len, "output buffer too short");

            for (int i = 0; i < len; i++)
            {
                output[outOff + i] = (byte)(input[inOff + i] ^ GetByte());
            }
        }

        public virtual void Reset()
        {
            Init();
        }

        public virtual byte ReturnByte(byte input)
        {
            return (byte)(input ^ GetByte());
        }

        private uint Step()
        {
            uint j = _cnt & 0x3FF;
            uint ret;
            if (_cnt < 1024)
            {
                uint x = _p[(j - 3 & 0x3FF)];
                uint y = _p[(j - 1023 & 0x3FF)];
                _p[j] += _p[(j - 10 & 0x3FF)]
                         + (RotateRight(x, 10) ^ RotateRight(y, 23))
                         + _q[((x ^ y) & 0x3FF)];

                x = _p[(j - 12 & 0x3FF)];
                ret = (_q[x & 0xFF] + _q[((x >> 8) & 0xFF) + 256]
                                    + _q[((x >> 16) & 0xFF) + 512] + _q[((x >> 24) & 0xFF) + 768])
                      ^ _p[j];
            }
            else
            {
                uint x = _q[(j - 3 & 0x3FF)];
                uint y = _q[(j - 1023 & 0x3FF)];
                _q[j] += _q[(j - 10 & 0x3FF)]
                         + (RotateRight(x, 10) ^ RotateRight(y, 23))
                         + _p[((x ^ y) & 0x3FF)];

                x = _q[(j - 12 & 0x3FF)];
                ret = (_p[x & 0xFF] + _p[((x >> 8) & 0xFF) + 256]
                                    + _p[((x >> 16) & 0xFF) + 512] + _p[((x >> 24) & 0xFF) + 768])
                      ^ _q[j];
            }

            _cnt = _cnt + 1 & 0x7FF;
            return ret;
        }

        private void Init()
        {
            if (_key.Length != 32 && _key.Length != 16)
                throw new ArgumentException("The key must be 128/256 bits long");

            if (_iv.Length < 16)
                throw new ArgumentException("The IV must be at least 128 bits long");

            if (_key.Length != 32)
            {
                byte[] k = new byte[32];

                Array.Copy(_key, 0, k, 0, _key.Length);
                Array.Copy(_key, 0, k, 16, _key.Length);

                _key = k;
            }

            if (_iv.Length < 32)
            {
                byte[] newIv = new byte[32];

                Array.Copy(_iv, 0, newIv, 0, _iv.Length);
                Array.Copy(_iv, 0, newIv, _iv.Length, newIv.Length - _iv.Length);

                _iv = newIv;
            }

            _idx = 0;
            _cnt = 0;

            uint[] w = new uint[2560];

            for (int i = 0; i < 32; i++)
            {
                w[i >> 2] |= ((uint)_key[i] << (8 * (i & 0x3)));
            }

            for (int i = 0; i < 32; i++)
            {
                w[(i >> 2) + 8] |= ((uint)_iv[i] << (8 * (i & 0x3)));
            }

            for (uint i = 16; i < 2560; i++)
            {
                uint x = w[i - 2];
                uint y = w[i - 15];
                w[i] = (RotateRight(x, 17) ^ RotateRight(x, 19) ^ (x >> 10))
                       + w[i - 7]
                       + (RotateRight(y, 7) ^ RotateRight(y, 18) ^ (y >> 3))
                       + w[i - 16] + i;
            }

            Array.Copy(w, 512, _p, 0, 1024);
            Array.Copy(w, 1536, _q, 0, 1024);

            for (int i = 0; i < 4096; i++)
            {
                Step();
            }

            _cnt = 0;
        }

        private byte GetByte()
        {
            if (_idx == 0)
            {
                Pack.UInt32_To_LE(Step(), _buf);
            }

            byte ret = _buf[_idx];
            _idx = _idx + 1 & 0x3;
            return ret;
        }

        private static uint RotateRight(uint x, int bits)
        {
            return (x >> bits) | (x << -bits);
        }
    }
}
#pragma warning restore
#endif
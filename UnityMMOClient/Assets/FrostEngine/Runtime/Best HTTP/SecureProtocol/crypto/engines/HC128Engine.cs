#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
	* HC-128 is a software-efficient stream cipher created by Hongjun Wu. It
	* generates keystream from a 128-bit secret key and a 128-bit initialization
	* vector.
	* <p>
	* http://www.ecrypt.eu.org/stream/p3ciphers/hc/hc128_p3.pdf
	* </p><p>
	* It is a third phase candidate in the eStream contest, and is patent-free.
	* No attacks are known as of today (April 2007). See
	*
	* http://www.ecrypt.eu.org/stream/hcp3.html
	* </p>
	*/
    public class Hc128Engine
        : IStreamCipher
    {
        private byte[] _buf = new byte[4];
        private uint _cnt = 0;
        private int _idx = 0;
        private bool _initialised;

        private byte[] _key, _iv;
        private uint[] _p = new uint[512];
        private uint[] _q = new uint[512];

        public virtual string AlgorithmName
        {
            get { return "HC-128"; }
        }

        /**
		* Initialise a HC-128 cipher.
		*
		* @param forEncryption whether or not we are for encryption. Irrelevant, as
		*                      encryption and decryption are the same.
		* @param params        the parameters required to set up the cipher.
		* @throws ArgumentException if the params argument is
		*                                  inappropriate (ie. the key is not 128 bit long).
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
                    "Invalid parameter passed to HC128 init - " +
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

        private static uint F1(uint x)
        {
            return RotateRight(x, 7) ^ RotateRight(x, 18) ^ (x >> 3);
        }

        private static uint F2(uint x)
        {
            return RotateRight(x, 17) ^ RotateRight(x, 19) ^ (x >> 10);
        }

        private uint G1(uint x, uint y, uint z)
        {
            return (RotateRight(x, 10) ^ RotateRight(z, 23)) + RotateRight(y, 8);
        }

        private uint G2(uint x, uint y, uint z)
        {
            return (RotateLeft(x, 10) ^ RotateLeft(z, 23)) + RotateLeft(y, 8);
        }

        private static uint RotateLeft(uint x, int bits)
        {
            return (x << bits) | (x >> -bits);
        }

        private static uint RotateRight(uint x, int bits)
        {
            return (x >> bits) | (x << -bits);
        }

        private uint H1(uint x)
        {
            return _q[x & 0xFF] + _q[((x >> 16) & 0xFF) + 256];
        }

        private uint H2(uint x)
        {
            return _p[x & 0xFF] + _p[((x >> 16) & 0xFF) + 256];
        }

        private static uint Mod1024(uint x)
        {
            return x & 0x3FF;
        }

        private static uint Mod512(uint x)
        {
            return x & 0x1FF;
        }

        private static uint Dim(uint x, uint y)
        {
            return Mod512(x - y);
        }

        private uint Step()
        {
            uint j = Mod512(_cnt);
            uint ret;
            if (_cnt < 512)
            {
                _p[j] += G1(_p[Dim(j, 3)], _p[Dim(j, 10)], _p[Dim(j, 511)]);
                ret = H1(_p[Dim(j, 12)]) ^ _p[j];
            }
            else
            {
                _q[j] += G2(_q[Dim(j, 3)], _q[Dim(j, 10)], _q[Dim(j, 511)]);
                ret = H2(_q[Dim(j, 12)]) ^ _q[j];
            }

            _cnt = Mod1024(_cnt + 1);
            return ret;
        }

        private void Init()
        {
            if (_key.Length != 16)
                throw new ArgumentException("The key must be 128 bits long");

            _idx = 0;
            _cnt = 0;

            uint[] w = new uint[1280];

            for (int i = 0; i < 16; i++)
            {
                w[i >> 2] |= ((uint)_key[i] << (8 * (i & 0x3)));
            }

            Array.Copy(w, 0, w, 4, 4);

            for (int i = 0; i < _iv.Length && i < 16; i++)
            {
                w[(i >> 2) + 8] |= ((uint)_iv[i] << (8 * (i & 0x3)));
            }

            Array.Copy(w, 8, w, 12, 4);

            for (uint i = 16; i < 1280; i++)
            {
                w[i] = F2(w[i - 2]) + w[i - 7] + F1(w[i - 15]) + w[i - 16] + i;
            }

            Array.Copy(w, 256, _p, 0, 512);
            Array.Copy(w, 768, _q, 0, 512);

            for (int i = 0; i < 512; i++)
            {
                _p[i] = Step();
            }

            for (int i = 0; i < 512; i++)
            {
                _q[i] = Step();
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
    }
}
#pragma warning restore
#endif
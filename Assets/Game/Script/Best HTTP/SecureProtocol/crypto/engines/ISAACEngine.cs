#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
    * Implementation of Bob Jenkin's ISAAC (Indirection Shift Accumulate Add and Count).
    * see: http://www.burtleburtle.net/bob/rand/isaacafa.html
    */
    public class IsaacEngine
        : IStreamCipher
    {
        // Constants
        private static readonly int SizeL = 8,
            StateArraySize = SizeL << 5; // 256

        private uint _a = 0, _b = 0, _c = 0;

        // Cipher's internal state
        private uint[] _engineState = null, // mm                
            _results = null; // randrsl

        // Engine state
        private int _index = 0;
        private bool _initialised = false;

        private byte[] _keyStream = new byte[StateArraySize << 2], // results expanded into bytes
            _workingKey = null;

        /**
        * initialise an ISAAC cipher.
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
                throw new ArgumentException(
                    "invalid parameter passed to ISAAC Init - " +
                    BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(parameters),
                    "parameters");

            /* 
            * ISAAC encryption and decryption is completely
            * symmetrical, so the 'forEncryption' is 
            * irrelevant.
            */
            KeyParameter p = (KeyParameter)parameters;
            SetKey(p.GetKey());
        }

        public virtual byte ReturnByte(
            byte input)
        {
            if (_index == 0)
            {
                Isaac();
                _keyStream = Pack.UInt32_To_BE(_results);
            }

            byte output = (byte)(_keyStream[_index] ^ input);
            _index = (_index + 1) & 1023;

            return output;
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
                if (_index == 0)
                {
                    Isaac();
                    _keyStream = Pack.UInt32_To_BE(_results);
                }

                output[i + outOff] = (byte)(_keyStream[_index] ^ input[i + inOff]);
                _index = (_index + 1) & 1023;
            }
        }

        public virtual string AlgorithmName
        {
            get { return "ISAAC"; }
        }

        public virtual void Reset()
        {
            SetKey(_workingKey);
        }

        // Private implementation
        private void SetKey(
            byte[] keyBytes)
        {
            _workingKey = keyBytes;

            if (_engineState == null)
            {
                _engineState = new uint[StateArraySize];
            }

            if (_results == null)
            {
                _results = new uint[StateArraySize];
            }

            int i, j, k;

            // Reset state
            for (i = 0; i < StateArraySize; i++)
            {
                _engineState[i] = _results[i] = 0;
            }

            _a = _b = _c = 0;

            // Reset index counter for output
            _index = 0;

            // Convert the key bytes to ints and put them into results[] for initialization
            byte[] t = new byte[keyBytes.Length + (keyBytes.Length & 3)];
            Array.Copy(keyBytes, 0, t, 0, keyBytes.Length);
            for (i = 0; i < t.Length; i += 4)
            {
                _results[i >> 2] = Pack.LE_To_UInt32(t, i);
            }

            // It has begun?
            uint[] abcdefgh = new uint[SizeL];

            for (i = 0; i < SizeL; i++)
            {
                abcdefgh[i] = 0x9e3779b9; // Phi (golden ratio)
            }

            for (i = 0; i < 4; i++)
            {
                Mix(abcdefgh);
            }

            for (i = 0; i < 2; i++)
            {
                for (j = 0; j < StateArraySize; j += SizeL)
                {
                    for (k = 0; k < SizeL; k++)
                    {
                        abcdefgh[k] += (i < 1) ? _results[j + k] : _engineState[j + k];
                    }

                    Mix(abcdefgh);

                    for (k = 0; k < SizeL; k++)
                    {
                        _engineState[j + k] = abcdefgh[k];
                    }
                }
            }

            Isaac();

            _initialised = true;
        }

        private void Isaac()
        {
            uint x, y;

            _b += ++_c;
            for (int i = 0; i < StateArraySize; i++)
            {
                x = _engineState[i];
                switch (i & 3)
                {
                    case 0:
                        _a ^= (_a << 13);
                        break;
                    case 1:
                        _a ^= (_a >> 6);
                        break;
                    case 2:
                        _a ^= (_a << 2);
                        break;
                    case 3:
                        _a ^= (_a >> 16);
                        break;
                }

                _a += _engineState[(i + 128) & 0xFF];
                _engineState[i] = y = _engineState[(int)((uint)x >> 2) & 0xFF] + _a + _b;
                _results[i] = _b = _engineState[(int)((uint)y >> 10) & 0xFF] + x;
            }
        }

        private void Mix(uint[] x)
        {
            x[0] ^= x[1] << 11;
            x[3] += x[0];
            x[1] += x[2];
            x[1] ^= x[2] >> 2;
            x[4] += x[1];
            x[2] += x[3];
            x[2] ^= x[3] << 8;
            x[5] += x[2];
            x[3] += x[4];
            x[3] ^= x[4] >> 16;
            x[6] += x[3];
            x[4] += x[5];
            x[4] ^= x[5] << 10;
            x[7] += x[4];
            x[5] += x[6];
            x[5] ^= x[6] >> 4;
            x[0] += x[5];
            x[6] += x[7];
            x[6] ^= x[7] << 8;
            x[1] += x[6];
            x[7] += x[0];
            x[7] ^= x[0] >> 9;
            x[2] += x[7];
            x[0] += x[1];
        }
    }
}
#pragma warning restore
#endif
#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    public class Rc4Engine
        : IStreamCipher
    {
        private readonly static int StateLength = 256;

        /*
        * variables to hold the state of the RC4 engine
        * during encryption and decryption
        */

        private byte[] _engineState;
        private byte[] _workingKey;
        private int _x;
        private int _y;

        /**
        * initialise a RC4 cipher.
        *
        * @param forEncryption whether or not we are for encryption.
        * @param parameters the parameters required to set up the cipher.
        * @exception ArgumentException if the parameters argument is
        * inappropriate.
        */
        public virtual void Init(
            bool forEncryption,
            ICipherParameters parameters)
        {
            if (parameters is KeyParameter)
            {
                /*
                * RC4 encryption and decryption is completely
                * symmetrical, so the 'forEncryption' is
                * irrelevant.
                */
                _workingKey = ((KeyParameter)parameters).GetKey();
                SetKey(_workingKey);

                return;
            }

            throw new ArgumentException("invalid parameter passed to RC4 init - " +
                                        BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(
                                            parameters));
        }

        public virtual string AlgorithmName
        {
            get { return "RC4"; }
        }

        public virtual byte ReturnByte(
            byte input)
        {
            _x = (_x + 1) & 0xff;
            _y = (_engineState[_x] + _y) & 0xff;

            // swap
            byte tmp = _engineState[_x];
            _engineState[_x] = _engineState[_y];
            _engineState[_y] = tmp;

            // xor
            return (byte)(input ^ _engineState[(_engineState[_x] + _engineState[_y]) & 0xff]);
        }

        public virtual void ProcessBytes(
            byte[] input,
            int inOff,
            int length,
            byte[] output,
            int outOff)
        {
            Check.DataLength(input, inOff, length, "input buffer too short");
            Check.OutputLength(output, outOff, length, "output buffer too short");

            for (int i = 0; i < length; i++)
            {
                _x = (_x + 1) & 0xff;
                _y = (_engineState[_x] + _y) & 0xff;

                // swap
                byte tmp = _engineState[_x];
                _engineState[_x] = _engineState[_y];
                _engineState[_y] = tmp;

                // xor
                output[i + outOff] = (byte)(input[i + inOff]
                                            ^ _engineState[(_engineState[_x] + _engineState[_y]) & 0xff]);
            }
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

            // System.out.println("the key length is ; "+ workingKey.Length);

            _x = 0;
            _y = 0;

            if (_engineState == null)
            {
                _engineState = new byte[StateLength];
            }

            // reset the state of the engine
            for (int i = 0; i < StateLength; i++)
            {
                _engineState[i] = (byte)i;
            }

            int i1 = 0;
            int i2 = 0;

            for (int i = 0; i < StateLength; i++)
            {
                i2 = ((keyBytes[i1] & 0xff) + _engineState[i] + i2) & 0xff;
                // do the byte-swap inline
                byte tmp = _engineState[i];
                _engineState[i] = _engineState[i2];
                _engineState[i2] = tmp;
                i1 = (i1 + 1) % keyBytes.Length;
            }
        }
    }
}
#pragma warning restore
#endif
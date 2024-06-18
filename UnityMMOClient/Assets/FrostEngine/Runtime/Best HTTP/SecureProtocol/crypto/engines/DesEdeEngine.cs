#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /// <remarks>A class that provides a basic DESede (or Triple DES) engine.</remarks>
    public class DesEdeEngine
        : DesEngine
    {
        private bool _forEncryption;
        private int[] _workingKey1, _workingKey2, _workingKey3;

        public override string AlgorithmName
        {
            get { return "DESede"; }
        }

        /**
        * initialise a DESede cipher.
        *
        * @param forEncryption whether or not we are for encryption.
        * @param parameters the parameters required to set up the cipher.
        * @exception ArgumentException if the parameters argument is
        * inappropriate.
        */
        public override void Init(
            bool forEncryption,
            ICipherParameters parameters)
        {
            if (!(parameters is KeyParameter))
                throw new ArgumentException("invalid parameter passed to DESede init - " +
                                            BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(
                                                parameters));

            byte[] keyMaster = ((KeyParameter)parameters).GetKey();
            if (keyMaster.Length != 24 && keyMaster.Length != 16)
                throw new ArgumentException("key size must be 16 or 24 bytes.");

            this._forEncryption = forEncryption;

            byte[] key1 = new byte[8];
            Array.Copy(keyMaster, 0, key1, 0, key1.Length);
            _workingKey1 = GenerateWorkingKey(forEncryption, key1);

            byte[] key2 = new byte[8];
            Array.Copy(keyMaster, 8, key2, 0, key2.Length);
            _workingKey2 = GenerateWorkingKey(!forEncryption, key2);

            if (keyMaster.Length == 24)
            {
                byte[] key3 = new byte[8];
                Array.Copy(keyMaster, 16, key3, 0, key3.Length);
                _workingKey3 = GenerateWorkingKey(forEncryption, key3);
            }
            else // 16 byte key
            {
                _workingKey3 = _workingKey1;
            }
        }

        public override int GetBlockSize()
        {
            return BlockSize;
        }

        public override int ProcessBlock(
            byte[] input,
            int inOff,
            byte[] output,
            int outOff)
        {
            if (_workingKey1 == null)
                throw new InvalidOperationException("DESede engine not initialised");

            Check.DataLength(input, inOff, BlockSize, "input buffer too short");
            Check.OutputLength(output, outOff, BlockSize, "output buffer too short");

            byte[] temp = new byte[BlockSize];

            if (_forEncryption)
            {
                DesFunc(_workingKey1, input, inOff, temp, 0);
                DesFunc(_workingKey2, temp, 0, temp, 0);
                DesFunc(_workingKey3, temp, 0, output, outOff);
            }
            else
            {
                DesFunc(_workingKey3, input, inOff, temp, 0);
                DesFunc(_workingKey2, temp, 0, temp, 0);
                DesFunc(_workingKey1, temp, 0, output, outOff);
            }

            return BlockSize;
        }

        public override void Reset()
        {
        }
    }
}
#pragma warning restore
#endif
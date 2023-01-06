#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using System.Collections;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    public class Dstu7624WrapEngine
        : IWrapper
    {
        private int _blockSize;
        private Dstu7624Engine _engine;
        private bool _forWrapping;
        private KeyParameter _param;

        public Dstu7624WrapEngine(int blockSizeBits)
        {
            _engine = new Dstu7624Engine(blockSizeBits);
            _param = null;

            _blockSize = blockSizeBits / 8;
        }

        public string AlgorithmName
        {
            get { return "Dstu7624WrapEngine"; }
        }

        public void Init(bool forWrapping, ICipherParameters parameters)
        {
            this._forWrapping = forWrapping;

            if (parameters is KeyParameter)
            {
                this._param = (KeyParameter)parameters;

                _engine.Init(forWrapping, _param);
            }
            else
            {
                throw new ArgumentException("Bad parameters passed to Dstu7624WrapEngine");
            }
        }

        public byte[] Wrap(byte[] input, int inOff, int length)
        {
            if (!_forWrapping)
                throw new InvalidOperationException("Not set for wrapping");

            if (length % _blockSize != 0)
                throw new ArgumentException("Padding not supported");

            int n = 2 * (1 + length / _blockSize);
            int v = (n - 1) * 6;

            byte[] buffer = new byte[length + _blockSize];
            Array.Copy(input, inOff, buffer, 0, length);
            //Console.WriteLine(BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(buffer));

            byte[] b = new byte[_blockSize / 2];
            Array.Copy(buffer, 0, b, 0, _blockSize / 2);
            //Console.WriteLine("B0: "+ BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(B));

            IList bTemp = BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.CreateArrayList();
            int bHalfBlocksLen = buffer.Length - _blockSize / 2;
            int bufOff = _blockSize / 2;
            while (bHalfBlocksLen != 0)
            {
                byte[] temp = new byte[_blockSize / 2];
                Array.Copy(buffer, bufOff, temp, 0, _blockSize / 2);
                //Console.WriteLine(BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(buffer));
                //Console.WriteLine(buffer.Length);
                //Console.WriteLine("b: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(temp));

                bTemp.Add(temp);

                bHalfBlocksLen -= _blockSize / 2;
                bufOff += _blockSize / 2;
            }

            for (int j = 0; j < v; j++)
            {
                Array.Copy(b, 0, buffer, 0, _blockSize / 2);
                Array.Copy((byte[])bTemp[0], 0, buffer, _blockSize / 2, _blockSize / 2);

                _engine.ProcessBlock(buffer, 0, buffer, 0);

                byte[] intArray = Pack.UInt32_To_LE((uint)(j + 1));
                for (int byteNum = 0; byteNum < intArray.Length; byteNum++)
                {
                    buffer[byteNum + _blockSize / 2] ^= intArray[byteNum];
                }

                Array.Copy(buffer, _blockSize / 2, b, 0, _blockSize / 2);

                for (int i = 2; i < n; i++)
                {
                    Array.Copy((byte[])bTemp[i - 1], 0, (byte[])bTemp[i - 2], 0, _blockSize / 2);
                }

                Array.Copy(buffer, 0, (byte[])bTemp[n - 2], 0, _blockSize / 2);

                //Console.WriteLine("B" + j.ToString() + ": " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(B));
                //Console.WriteLine("b: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(bTemp[0]));
                //Console.WriteLine("b: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(bTemp[1]));
                //Console.WriteLine("b: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(bTemp[2]));

                //Console.WriteLine(BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(buffer));
            }

            Array.Copy(b, 0, buffer, 0, _blockSize / 2);
            bufOff = _blockSize / 2;

            for (int i = 0; i < n - 1; i++)
            {
                Array.Copy((byte[])bTemp[i], 0, buffer, bufOff, _blockSize / 2);
                bufOff += _blockSize / 2;
            }

            return buffer;
        }

        public byte[] Unwrap(byte[] input, int inOff, int length)
        {
            if (_forWrapping)
                throw new InvalidOperationException("not set for unwrapping");

            if (length % _blockSize != 0)
                throw new ArgumentException("Padding not supported");

            int n = 2 * length / _blockSize;
            int v = (n - 1) * 6;

            byte[] buffer = new byte[length];
            Array.Copy(input, inOff, buffer, 0, length);

            byte[] b = new byte[_blockSize / 2];
            Array.Copy(buffer, 0, b, 0, _blockSize / 2);
            //Console.WriteLine("B18: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(B));

            IList bTemp = BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.CreateArrayList();

            int bHalfBlocksLen = buffer.Length - _blockSize / 2;
            int bufOff = _blockSize / 2;
            while (bHalfBlocksLen != 0)
            {
                byte[] temp = new byte[_blockSize / 2];
                Array.Copy(buffer, bufOff, temp, 0, _blockSize / 2);
                //Console.WriteLine(BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(buffer));
                //Console.WriteLine(buffer.Length);
                //Console.WriteLine("b: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(temp));

                bTemp.Add(temp);

                bHalfBlocksLen -= _blockSize / 2;
                bufOff += _blockSize / 2;
            }

            for (int j = 0; j < v; j++)
            {
                Array.Copy((byte[])bTemp[n - 2], 0, buffer, 0, _blockSize / 2);
                Array.Copy(b, 0, buffer, _blockSize / 2, _blockSize / 2);

                byte[] intArray = Pack.UInt32_To_LE((uint)(v - j));
                for (int byteNum = 0; byteNum < intArray.Length; byteNum++)
                {
                    buffer[byteNum + _blockSize / 2] ^= intArray[byteNum];
                }

                //Console.WriteLine(BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(buffer));

                _engine.ProcessBlock(buffer, 0, buffer, 0);

                //Console.WriteLine(BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(buffer));

                Array.Copy(buffer, 0, b, 0, _blockSize / 2);

                for (int i = 2; i < n; i++)
                {
                    Array.Copy((byte[])bTemp[n - i - 1], 0, (byte[])bTemp[n - i], 0, _blockSize / 2);
                }

                Array.Copy(buffer, _blockSize / 2, (byte[])bTemp[0], 0, _blockSize / 2);

                //Console.WriteLine("B" + (V - j - 1).ToString() + ": " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(B));
                //Console.WriteLine("b: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(bTemp[0]));
                //Console.WriteLine("b: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(bTemp[1]));
                //Console.WriteLine("b: " + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(bTemp[2]));

                //Console.WriteLine(BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders.Hex.ToHexString(buffer));
            }

            Array.Copy(b, 0, buffer, 0, _blockSize / 2);
            bufOff = _blockSize / 2;

            for (int i = 0; i < n - 1; i++)
            {
                Array.Copy((byte[])bTemp[i], 0, buffer, bufOff, _blockSize / 2);
                bufOff += _blockSize / 2;
            }

            byte diff = 0;
            for (int i = buffer.Length - _blockSize; i < buffer.Length; ++i)
            {
                diff |= buffer[i];
            }

            if (diff != 0)
                throw new InvalidCipherTextException("checksum failed");

            return Arrays.CopyOfRange(buffer, 0, buffer.Length - _blockSize);
        }
    }
}
#pragma warning restore
#endif
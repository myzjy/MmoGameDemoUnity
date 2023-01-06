#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /// <remarks>
    /// An implementation of the AES Key Wrapper from the NIST Key Wrap
    /// Specification as described in RFC 3394.
    /// <p/>
    /// For further details see: <a href="http://www.ietf.org/rfc/rfc3394.txt">http://www.ietf.org/rfc/rfc3394.txt</a>
    /// and  <a href="http://csrc.nist.gov/encryption/kms/key-wrap.pdf">http://csrc.nist.gov/encryption/kms/key-wrap.pdf</a>.
    /// </remarks>
    public class Rfc3394WrapEngine
        : IWrapper
    {
        private readonly IBlockCipher _engine;
        private bool _forWrapping;

        private byte[] _iv =
        {
            0xa6, 0xa6, 0xa6, 0xa6,
            0xa6, 0xa6, 0xa6, 0xa6
        };

        private KeyParameter _param;

        public Rfc3394WrapEngine(
            IBlockCipher engine)
        {
            this._engine = engine;
        }

        public virtual void Init(
            bool forWrapping,
            ICipherParameters parameters)
        {
            this._forWrapping = forWrapping;

            if (parameters is ParametersWithRandom)
            {
                parameters = ((ParametersWithRandom)parameters).Parameters;
            }

            if (parameters is KeyParameter)
            {
                this._param = (KeyParameter)parameters;
            }
            else if (parameters is ParametersWithIV)
            {
                ParametersWithIV pIv = (ParametersWithIV)parameters;
                byte[] iv = pIv.GetIV();

                if (iv.Length != 8)
                    throw new ArgumentException("IV length not equal to 8", "parameters");

                this._iv = iv;
                this._param = (KeyParameter)pIv.Parameters;
            }
            else
            {
                // TODO Throw an exception for bad parameters?
            }
        }

        public virtual string AlgorithmName
        {
            get { return _engine.AlgorithmName; }
        }

        public virtual byte[] Wrap(
            byte[] input,
            int inOff,
            int inLen)
        {
            if (!_forWrapping)
            {
                throw new InvalidOperationException("not set for wrapping");
            }

            int n = inLen / 8;

            if ((n * 8) != inLen)
            {
                throw new DataLengthException("wrap data must be a multiple of 8 bytes");
            }

            byte[] block = new byte[inLen + _iv.Length];
            byte[] buf = new byte[8 + _iv.Length];

            Array.Copy(_iv, 0, block, 0, _iv.Length);
            Array.Copy(input, inOff, block, _iv.Length, inLen);

            _engine.Init(true, _param);

            for (int j = 0; j != 6; j++)
            {
                for (int i = 1; i <= n; i++)
                {
                    Array.Copy(block, 0, buf, 0, _iv.Length);
                    Array.Copy(block, 8 * i, buf, _iv.Length, 8);
                    _engine.ProcessBlock(buf, 0, buf, 0);

                    int t = n * j + i;
                    for (int k = 1; t != 0; k++)
                    {
                        byte v = (byte)t;

                        buf[_iv.Length - k] ^= v;
                        t = (int)((uint)t >> 8);
                    }

                    Array.Copy(buf, 0, block, 0, 8);
                    Array.Copy(buf, 8, block, 8 * i, 8);
                }
            }

            return block;
        }

        public virtual byte[] Unwrap(
            byte[] input,
            int inOff,
            int inLen)
        {
            if (_forWrapping)
            {
                throw new InvalidOperationException("not set for unwrapping");
            }

            int n = inLen / 8;

            if ((n * 8) != inLen)
            {
                throw new InvalidCipherTextException("unwrap data must be a multiple of 8 bytes");
            }

            byte[] block = new byte[inLen - _iv.Length];
            byte[] a = new byte[_iv.Length];
            byte[] buf = new byte[8 + _iv.Length];

            Array.Copy(input, inOff, a, 0, _iv.Length);
            Array.Copy(input, inOff + _iv.Length, block, 0, inLen - _iv.Length);

            _engine.Init(false, _param);

            n = n - 1;

            for (int j = 5; j >= 0; j--)
            {
                for (int i = n; i >= 1; i--)
                {
                    Array.Copy(a, 0, buf, 0, _iv.Length);
                    Array.Copy(block, 8 * (i - 1), buf, _iv.Length, 8);

                    int t = n * j + i;
                    for (int k = 1; t != 0; k++)
                    {
                        byte v = (byte)t;

                        buf[_iv.Length - k] ^= v;
                        t = (int)((uint)t >> 8);
                    }

                    _engine.ProcessBlock(buf, 0, buf, 0);
                    Array.Copy(buf, 0, a, 0, 8);
                    Array.Copy(buf, 8, block, 8 * (i - 1), 8);
                }
            }

            if (!Arrays.ConstantTimeAreEqual(a, _iv))
                throw new InvalidCipherTextException("checksum failed");

            return block;
        }
    }
}
#pragma warning restore
#endif
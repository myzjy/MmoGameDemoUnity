#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.PlatformSupport.IL2CPP;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;

namespace BestHTTP.Connections.TLS.Crypto.Impl
{
    /// <summary>
    /// Poly1305 message authentication code, designed by D. J. Bernstein.
    /// </summary>
    /// <remarks>
    /// Poly1305使用128位nonce和256位key计算128位(16字节)验证器
    /// 由应用于底层密码的128位密钥和用于验证者的128位密钥(有106个有效密钥位)组成。
    /// 
    /// 本实现中的多项式计算采用了公共领域的多项式计算方法
    /// <a href="https://github.com/floodyberry/poly1305-donna">poly1305-donna-unrolled</a> C implementation
    /// by Andrew M (@floodYBerry).
    /// </remarks>
    /// <seealso cref="BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Generators.Poly1305KeyGenerator"/>
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [Il2CppEagerStaticClassConstruction]
    public sealed class FastPoly1305 : IMac
    {
        private const int BlockSize = 16;

        private readonly IBlockCipher _cipher;

        private readonly byte[] _singleByte = new byte[1];

        // Initialised state

        /** Polynomial key */
        private uint _r0, _r1, _r2, _r3, _r4;

        /** Precomputed 5 * r[1..4] */
        private uint _s1, _s2, _s3, _s4;

        /** Encrypted nonce */
        private uint _k0, _k1, _k2, _k3;

        // Accumulating state

        /** 缓冲输入的当前块 */
        private readonly byte[] _currentBlock = new byte[BlockSize];

        /** 输入缓冲区中的电流偏移量 */
        private int _currentBlockOffset;

        /**多项式蓄电池 */
        private uint _h0, _h1, _h2, _h3, _h4;

        /**
         * 构造一个Poly1305 MAC，其中传递给init()的键将直接使用。
         */
        public FastPoly1305()
        {
            _cipher = null;
        }

        /**
         * Constructs a Poly1305 MAC, using a 128 bit block cipher.
         */
        public FastPoly1305(IBlockCipher cipher)
        {
            if (cipher.GetBlockSize() != BlockSize)
            {
                throw new ArgumentException("Poly1305 requires a 128 bit block cipher.");
            }

            this._cipher = cipher;
        }

        /// <summary>
        /// Initialises the Poly1305 MAC.
        /// </summary>
        /// <param name="parameters">
        /// a {@link ParametersWithIV} containing a 128 bit nonce and a {@link KeyParameter} with
        ///          a 256 bit key complying to the {@link Poly1305KeyGenerator Poly1305 key format}.</param>
        public void Init(ICipherParameters parameters)
        {
            byte[] nonce = null;

            if (_cipher != null)
            {
                if (parameters is not FastParametersWithIV ivParams)
                {
                    throw new ArgumentException("Poly1305在与分组密码一起使用时需要IV。", nameof(parameters));
                }

                nonce = ivParams.GetIV();
                parameters = ivParams.Parameters;
            }

            if (parameters is not NoCopyKeyParameter keyParams)
            {
                throw new ArgumentException("Poly1305需要一个密钥.");
            }

            SetKey(keyParams.GetKey(), nonce);

            Reset();
        }

        private void SetKey(byte[] key, byte[] nonce)
        {
            if (key.Length != 32)
                throw new ArgumentException("Poly1305 key must be 256 bits.");

            if (_cipher != null && (nonce == null || nonce.Length != BlockSize))
            {
                throw new ArgumentException("Poly1305 requires a 128 bit IV.");
            }

            // Extract r portion of key (and "clamp" the values)
            var t0 = Pack.LE_To_UInt32(key, 0);
            uint t1 = Pack.LE_To_UInt32(key, 4);
            uint t2 = Pack.LE_To_UInt32(key, 8);
            uint t3 = Pack.LE_To_UInt32(key, 12);

            // NOTE: The masks perform the key "clamping" implicitly
            _r0 = t0 & 0x03FFFFFFU;
            _r1 = ((t0 >> 26) | (t1 << 6)) & 0x03FFFF03U;
            _r2 = ((t1 >> 20) | (t2 << 12)) & 0x03FFC0FFU;
            _r3 = ((t2 >> 14) | (t3 << 18)) & 0x03F03FFFU;
            _r4 = (t3 >> 8) & 0x000FFFFFU;

            // Precompute multipliers
            _s1 = _r1 * 5;
            _s2 = _r2 * 5;
            _s3 = _r3 * 5;
            _s4 = _r4 * 5;

            byte[] kBytes;
            int kOff;

            if (_cipher == null)
            {
                kBytes = key;
                kOff = BlockSize;
            }
            else
            {
                // Compute encrypted nonce
                kBytes = new byte[BlockSize];
                kOff = 0;

                _cipher.Init(true, new KeyParameter(key, BlockSize, BlockSize));
                _cipher.ProcessBlock(nonce, 0, kBytes, 0);
            }

            _k0 = Pack.LE_To_UInt32(kBytes, kOff + 0);
            _k1 = Pack.LE_To_UInt32(kBytes, kOff + 4);
            _k2 = Pack.LE_To_UInt32(kBytes, kOff + 8);
            _k3 = Pack.LE_To_UInt32(kBytes, kOff + 12);
        }

        public string AlgorithmName => _cipher == null ? "Poly1305" : $"Poly1305- {_cipher.AlgorithmName}";

        public int GetMacSize()
        {
            return BlockSize;
        }

        public void Update(byte input)
        {
            _singleByte[0] = input;
            BlockUpdate(_singleByte, 0, 1);
        }

        public void BlockUpdate(byte[] input, int inOff, int len)
        {
            int copied = 0;
            while (len > copied)
            {
                if (_currentBlockOffset == BlockSize)
                {
                    ProcessBlock();
                    _currentBlockOffset = 0;
                }

                int toCopy = Math.Min((len - copied), BlockSize - _currentBlockOffset);
                Array.Copy(input, copied + inOff, _currentBlock, _currentBlockOffset, toCopy);
                copied += toCopy;
                _currentBlockOffset += toCopy;
            }
        }

        private void ProcessBlock()
        {
            if (_currentBlockOffset < BlockSize)
            {
                _currentBlock[_currentBlockOffset] = 1;
                for (int i = _currentBlockOffset + 1; i < BlockSize; i++)
                {
                    _currentBlock[i] = 0;
                }
            }

            ulong t0 = Pack.LE_To_UInt32(_currentBlock, 0);
            ulong t1 = Pack.LE_To_UInt32(_currentBlock, 4);
            ulong t2 = Pack.LE_To_UInt32(_currentBlock, 8);
            ulong t3 = Pack.LE_To_UInt32(_currentBlock, 12);

            _h0 += (uint)(t0 & 0x3ffffffU);
            _h1 += (uint)((((t1 << 32) | t0) >> 26) & 0x3ffffff);
            _h2 += (uint)((((t2 << 32) | t1) >> 20) & 0x3ffffff);
            _h3 += (uint)((((t3 << 32) | t2) >> 14) & 0x3ffffff);
            _h4 += (uint)(t3 >> 8);

            if (_currentBlockOffset == BlockSize)
            {
                _h4 += (1 << 24);
            }

            var tp0 = mul32x32_64(_h0, _r0) + mul32x32_64(_h1, _s4) + mul32x32_64(_h2, _s3) + mul32x32_64(_h3, _s2) +
                      mul32x32_64(_h4, _s1);
            var tp1 = mul32x32_64(_h0, _r1) + mul32x32_64(_h1, _r0) + mul32x32_64(_h2, _s4) + mul32x32_64(_h3, _s3) +
                      mul32x32_64(_h4, _s2);
            var tp2 = mul32x32_64(_h0, _r2) + mul32x32_64(_h1, _r1) + mul32x32_64(_h2, _r0) + mul32x32_64(_h3, _s4) +
                      mul32x32_64(_h4, _s3);
            var tp3 = mul32x32_64(_h0, _r3) + mul32x32_64(_h1, _r2) + mul32x32_64(_h2, _r1) + mul32x32_64(_h3, _r0) +
                      mul32x32_64(_h4, _s4);
            var tp4 = mul32x32_64(_h0, _r4) + mul32x32_64(_h1, _r3) + mul32x32_64(_h2, _r2) + mul32x32_64(_h3, _r1) +
                      mul32x32_64(_h4, _r0);

            _h0 = (uint)tp0 & 0x3ffffff;
            tp1 += (tp0 >> 26);
            _h1 = (uint)tp1 & 0x3ffffff;
            tp2 += (tp1 >> 26);
            _h2 = (uint)tp2 & 0x3ffffff;
            tp3 += (tp2 >> 26);
            _h3 = (uint)tp3 & 0x3ffffff;
            tp4 += (tp3 >> 26);
            _h4 = (uint)tp4 & 0x3ffffff;
            _h0 += (uint)(tp4 >> 26) * 5;
            _h1 += (_h0 >> 26);
            _h0 &= 0x3ffffff;
        }

        public int DoFinal(byte[] output, int outOff)
        {
            Check.DataLength(output, outOff, BlockSize, "输出缓冲区太短.");

            if (_currentBlockOffset > 0)
            {
                // Process padded block
                ProcessBlock();
            }

            _h1 += (_h0 >> 26);
            _h0 &= 0x3ffffff;
            _h2 += (_h1 >> 26);
            _h1 &= 0x3ffffff;
            _h3 += (_h2 >> 26);
            _h2 &= 0x3ffffff;
            _h4 += (_h3 >> 26);
            _h3 &= 0x3ffffff;
            _h0 += (_h4 >> 26) * 5;
            _h4 &= 0x3ffffff;
            _h1 += (_h0 >> 26);
            _h0 &= 0x3ffffff;

            var g0 = _h0 + 5;
            var b = g0 >> 26;
            g0 &= 0x3ffffff;
            var g1 = _h1 + b;
            b = g1 >> 26;
            g1 &= 0x3ffffff;
            var g2 = _h2 + b;
            b = g2 >> 26;
            g2 &= 0x3ffffff;
            var g3 = _h3 + b;
            b = g3 >> 26;
            g3 &= 0x3ffffff;
            var g4 = _h4 + b - (1 << 26);

            b = (g4 >> 31) - 1;
            uint nb = ~b;
            _h0 = (_h0 & nb) | (g0 & b);
            _h1 = (_h1 & nb) | (g1 & b);
            _h2 = (_h2 & nb) | (g2 & b);
            _h3 = (_h3 & nb) | (g3 & b);
            _h4 = (_h4 & nb) | (g4 & b);

            var f0 = ((_h0) | (_h1 << 26)) + (ulong)_k0;
            var f1 = ((_h1 >> 6) | (_h2 << 20)) + (ulong)_k1;
            var f2 = ((_h2 >> 12) | (_h3 << 14)) + (ulong)_k2;
            var f3 = ((_h3 >> 18) | (_h4 << 8)) + (ulong)_k3;

            Pack.UInt32_To_LE((uint)f0, output, outOff);
            f1 += (f0 >> 32);
            Pack.UInt32_To_LE((uint)f1, output, outOff + 4);
            f2 += (f1 >> 32);
            Pack.UInt32_To_LE((uint)f2, output, outOff + 8);
            f3 += (f2 >> 32);
            Pack.UInt32_To_LE((uint)f3, output, outOff + 12);

            Reset();
            return BlockSize;
        }

        public void Reset()
        {
            _currentBlockOffset = 0;

            _h0 = _h1 = _h2 = _h3 = _h4 = 0;
        }

        private static ulong mul32x32_64(uint i1, uint i2)
        {
            return ((ulong)i1) * i2;
        }
    }
}
#pragma warning restore
#endif
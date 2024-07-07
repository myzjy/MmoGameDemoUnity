#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.PlatformSupport.IL2CPP;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

namespace BestHTTP.Connections.TLS.Crypto.Impl
{
    /// <summary>
    /// Implementation of Daniel J. Bernstein's Salsa20 stream cipher, Snuffle 2005
    /// </summary>
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [Il2CppEagerStaticClassConstruction]
    public class FastSalsa20Engine
        : IStreamCipher
    {
        private const int DefaultRounds = 20;

        /** Constants */
        private const int StateSize = 16; // 16, 32 bit ints = 64 bytes

        private static readonly uint[] TauSigma =
            Pack.LE_To_UInt32(Strings.ToAsciiByteArray("expand 16-byte k expand 32-byte k"), 0, 8);

        internal static void PackTauOrSigma(int keyLength, uint[] state, int stateOffset)
        {
            int tsOff = (keyLength - 16) / 4;
            state[stateOffset] = TauSigma[tsOff];
            state[stateOffset + 1] = TauSigma[tsOff + 1];
            state[stateOffset + 2] = TauSigma[tsOff + 2];
            state[stateOffset + 3] = TauSigma[tsOff + 3];
        }

        [Obsolete] protected static readonly byte[]
            Sigma = Strings.ToAsciiByteArray("expand 32-byte k"),
            Tau = Strings.ToAsciiByteArray("expand 16-byte k");

        protected readonly int Rounds;

        /*
		 * 变量来保存引擎的状态
		 * 在加密和解密期间
		 */
        private int _index;
        internal readonly uint[] EngineState = new uint[StateSize]; // state
        internal readonly uint[] X = new uint[StateSize]; // internal buffer
        private readonly byte[] _keyStream = new byte[StateSize * 4]; // expanded state, 64 bytes
        private bool _initialised;

        /*
		 * internal counter
		 */
        private uint _cW0, _cW1, _cW2;

        /// <summary>
        /// 创建一个20轮Salsa20引擎。
        /// </summary>
        protected FastSalsa20Engine()
            : this(DefaultRounds)
        {
        }

        /// <summary>
        /// 创建一个具有特定轮数的Salsa20引擎。
        /// </summary>
        /// <param name="rounds">回合数 (must be an even number).</param>
        protected FastSalsa20Engine(int rounds)
        {
            if (rounds <= 0 || (rounds & 1) != 0)
            {
                throw new ArgumentException("'rounds'必须是正数，偶数");
            }

            this.Rounds = rounds;
        }

        public virtual void Init(
            bool forEncryption,
            ICipherParameters parameters)
        {
            /* 
             * Salsa20加密和解密是完全的
             *对称，所以'forEncryption'是
             *无关紧要。(像90%的流密码)
			 */

            FastParametersWithIV ivParams = parameters as FastParametersWithIV;
            if (ivParams == null)
                throw new ArgumentException($"{AlgorithmName} 初始化需要一个 IV", nameof(parameters));

            byte[] iv = ivParams.GetIV();
            if (iv == null || iv.Length != NonceSize)
            {
                throw new ArgumentException($"{AlgorithmName} 需要准确 {NonceSize} bytes of IV");
            }

            ICipherParameters keyParam = ivParams.Parameters;
            if (keyParam == null)
            {
                if (!_initialised)
                    throw new InvalidOperationException($"{AlgorithmName} 第一次初始化时，KeyParameter不能为空");

                SetKey(null, iv);
            }
            else if (keyParam is NoCopyKeyParameter parameter)
            {
                SetKey(parameter.GetKey(), iv);
            }
            else
            {
                throw new ArgumentException($"{AlgorithmName} 初始化参数必须包含一个KeyParameter(重新初始化时为空)");
            }

            Reset();
            _initialised = true;
        }

        protected virtual int NonceSize => 8;

        public virtual string AlgorithmName
        {
            get
            {
                string name = "Salsa20";
                if (Rounds != DefaultRounds)
                {
                    name += "/" + Rounds;
                }

                return name;
            }
        }

        public virtual byte ReturnByte(
            byte input)
        {
            if (LimitExceeded())
            {
                throw new MaxBytesExceededException("2^70字节限制每IV;改变IV");
            }

            if (_index == 0)
            {
                GenerateKeyStream(_keyStream);
                AdvanceCounter();
            }

            byte output = (byte)(_keyStream[_index] ^ input);
            _index = (_index + 1) & 63;

            return output;
        }

        protected virtual void AdvanceCounter()
        {
            if (++EngineState[8] == 0)
            {
                ++EngineState[9];
            }
        }

        public virtual void ProcessBytes(
            byte[] inBytes,
            int inOff,
            int len,
            byte[] outBytes,
            int outOff)
        {
            if (!_initialised)
                throw new InvalidOperationException($"{AlgorithmName} 没有初始化");

            Check.DataLength(inBytes, inOff, len, "输入缓冲区过短");
            Check.OutputLength(outBytes, outOff, len, "输出缓冲区过短");

            if (LimitExceeded((uint)len))
                throw new MaxBytesExceededException("每个IV将超过2^70字节限制;改变IV");

            for (int i = 0; i < len; i++)
            {
                if (_index == 0)
                {
                    GenerateKeyStream(_keyStream);
                    AdvanceCounter();
                }

                outBytes[i + outOff] = (byte)(_keyStream[_index] ^ inBytes[i + inOff]);
                _index = (_index + 1) & 63;
            }
        }

        public virtual void Reset()
        {
            _index = 0;
            ResetLimitCounter();
            ResetCounter();
        }

        protected virtual void ResetCounter()
        {
            EngineState[8] = EngineState[9] = 0;
        }

        protected virtual void SetKey(byte[] keyBytes, byte[] ivBytes)
        {
            if (keyBytes != null)
            {
                if ((keyBytes.Length != 16) && (keyBytes.Length != 32))
                    throw new ArgumentException( $"{AlgorithmName} 需要128位或256位密钥");

                int tsOff = (keyBytes.Length - 16) / 4;
                EngineState[0] = TauSigma[tsOff];
                EngineState[5] = TauSigma[tsOff + 1];
                EngineState[10] = TauSigma[tsOff + 2];
                EngineState[15] = TauSigma[tsOff + 3];

                // Key
                Pack.LE_To_UInt32(keyBytes, 0, EngineState, 1, 4);
                Pack.LE_To_UInt32(keyBytes, keyBytes.Length - 16, EngineState, 11, 4);
            }

            // IV
            Pack.LE_To_UInt32(ivBytes, 0, EngineState, 6, 2);
        }

        protected virtual void GenerateKeyStream(byte[] output)
        {
            SalsaCore(Rounds, EngineState, X);
            Pack.UInt32_To_LE(X, output, 0);
        }

        private static void SalsaCore(int rounds, uint[] input, uint[] x)
        {
            if (input.Length != 16)
                throw new ArgumentException();
            if (x.Length != 16)
                throw new ArgumentException();
            if (rounds % 2 != 0)
                throw new ArgumentException("回合数必须为偶数");

            uint x00 = input[0];
            uint x01 = input[1];
            uint x02 = input[2];
            uint x03 = input[3];
            uint x04 = input[4];
            uint x05 = input[5];
            uint x06 = input[6];
            uint x07 = input[7];
            uint x08 = input[8];
            uint x09 = input[9];
            uint x10 = input[10];
            uint x11 = input[11];
            uint x12 = input[12];
            uint x13 = input[13];
            uint x14 = input[14];
            uint x15 = input[15];

            for (int i = rounds; i > 0; i -= 2)
            {
                x04 ^= Integers.RotateLeft((x00 + x12), 7);
                x08 ^= Integers.RotateLeft((x04 + x00), 9);
                x12 ^= Integers.RotateLeft((x08 + x04), 13);
                x00 ^= Integers.RotateLeft((x12 + x08), 18);
                x09 ^= Integers.RotateLeft((x05 + x01), 7);
                x13 ^= Integers.RotateLeft((x09 + x05), 9);
                x01 ^= Integers.RotateLeft((x13 + x09), 13);
                x05 ^= Integers.RotateLeft((x01 + x13), 18);
                x14 ^= Integers.RotateLeft((x10 + x06), 7);
                x02 ^= Integers.RotateLeft((x14 + x10), 9);
                x06 ^= Integers.RotateLeft((x02 + x14), 13);
                x10 ^= Integers.RotateLeft((x06 + x02), 18);
                x03 ^= Integers.RotateLeft((x15 + x11), 7);
                x07 ^= Integers.RotateLeft((x03 + x15), 9);
                x11 ^= Integers.RotateLeft((x07 + x03), 13);
                x15 ^= Integers.RotateLeft((x11 + x07), 18);

                x01 ^= Integers.RotateLeft((x00 + x03), 7);
                x02 ^= Integers.RotateLeft((x01 + x00), 9);
                x03 ^= Integers.RotateLeft((x02 + x01), 13);
                x00 ^= Integers.RotateLeft((x03 + x02), 18);
                x06 ^= Integers.RotateLeft((x05 + x04), 7);
                x07 ^= Integers.RotateLeft((x06 + x05), 9);
                x04 ^= Integers.RotateLeft((x07 + x06), 13);
                x05 ^= Integers.RotateLeft((x04 + x07), 18);
                x11 ^= Integers.RotateLeft((x10 + x09), 7);
                x08 ^= Integers.RotateLeft((x11 + x10), 9);
                x09 ^= Integers.RotateLeft((x08 + x11), 13);
                x10 ^= Integers.RotateLeft((x09 + x08), 18);
                x12 ^= Integers.RotateLeft((x15 + x14), 7);
                x13 ^= Integers.RotateLeft((x12 + x15), 9);
                x14 ^= Integers.RotateLeft((x13 + x12), 13);
                x15 ^= Integers.RotateLeft((x14 + x13), 18);
            }

            x[0] = x00 + input[0];
            x[1] = x01 + input[1];
            x[2] = x02 + input[2];
            x[3] = x03 + input[3];
            x[4] = x04 + input[4];
            x[5] = x05 + input[5];
            x[6] = x06 + input[6];
            x[7] = x07 + input[7];
            x[8] = x08 + input[8];
            x[9] = x09 + input[9];
            x[10] = x10 + input[10];
            x[11] = x11 + input[11];
            x[12] = x12 + input[12];
            x[13] = x13 + input[13];
            x[14] = x14 + input[14];
            x[15] = x15 + input[15];
        }

        private void ResetLimitCounter()
        {
            _cW0 = 0;
            _cW1 = 0;
            _cW2 = 0;
        }

        private bool LimitExceeded()
        {
            if (++_cW0 == 0)
            {
                if (++_cW1 == 0)
                {
                    return (++_cW2 & 0x20) != 0; // 2^(32 + 32 + 6)
                }
            }

            return false;
        }

        /*
		 * 这依赖于len总是正的事实。
		 */
        private bool LimitExceeded(
            uint len)
        {
            uint old = _cW0;
            _cW0 += len;
            if (_cW0 < old)
            {
                if (++_cW1 == 0)
                {
                    return (++_cW2 & 0x20) != 0; // 2^(32 + 32 + 6)
                }
            }

            return false;
        }
    }
}
#pragma warning restore
#endif
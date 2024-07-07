#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.PlatformSupport.IL2CPP;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

namespace BestHTTP.Connections.TLS.Crypto.Impl
{
    /// <summary>
    /// 实现 of Daniel J. Bernstein's ChaCha stream cipher.
    /// </summary>
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [Il2CppEagerStaticClassConstruction]
    public abstract class FastChaChaEngine
        : FastSalsa20Engine
    {
        /// <summary>
        /// 创建一个20轮的ChaCha引擎。
        /// </summary>
        public FastChaChaEngine()
        {
        }

        /// <summary>
        /// 创建一个具有特定回合数的ChaCha引擎。
        /// </summary>
        /// <param name="rounds">回合数(必须是偶数)。</param>
        public FastChaChaEngine(int rounds)
            : base(rounds)
        {
        }

        public override string AlgorithmName
        {
            get { return $"ChaCha{Rounds}"; }
        }

        protected override void AdvanceCounter()
        {
            if (++EngineState[12] == 0)
            {
                ++EngineState[13];
            }
        }

        protected override void ResetCounter()
        {
            EngineState[12] = EngineState[13] = 0;
        }

        protected override void SetKey(byte[] keyBytes, byte[] ivBytes)
        {
            if (keyBytes != null)
            {
                if ((keyBytes.Length != 16) && (keyBytes.Length != 32))
                {
                    throw new ArgumentException($"{AlgorithmName} 需要128位或256位密钥");
                }

                PackTauOrSigma(keyBytes.Length, EngineState, 0);

                // Key
                Pack.LE_To_UInt32(keyBytes, 0, EngineState, 4, 4);
                Pack.LE_To_UInt32(keyBytes, keyBytes.Length - 16, EngineState, 8, 4);
            }

            // IV
            Pack.LE_To_UInt32(ivBytes, 0, EngineState, 14, 2);
        }

        protected override void GenerateKeyStream(byte[] output)
        {
            ChaChaCore(Rounds, EngineState, X);
            Pack.UInt32_To_LE(X, output, 0);
        }

        /// <summary>
        /// ChaCha function.
        /// </summary>
        /// <param name="rounds">要执行的ChaCha回合数</param>
        /// <param name="input">输入单词.</param>
        /// <param name="x">要修改的ChaCha状态.</param>
        internal static void ChaChaCore(int rounds, uint[] input, uint[] x)
        {
            if (input.Length != 16)
                throw new ArgumentException();
            if (x.Length != 16)
                throw new ArgumentException();
            if (rounds % 2 != 0)
            {
                throw new ArgumentException("回合数必须为偶数");
            }

            var x00 = input[0];
            var x01 = input[1];
            var x02 = input[2];
            var x03 = input[3];
            var x04 = input[4];
            var x05 = input[5];
            var x06 = input[6];
            var x07 = input[7];
            var x08 = input[8];
            var x09 = input[9];
            var x10 = input[10];
            var x11 = input[11];
            var x12 = input[12];
            var x13 = input[13];
            var x14 = input[14];
            var x15 = input[15];

            for (int i = rounds; i > 0; i -= 2)
            {
                x00 += x04;
                x12 = Integers.RotateLeft(x12 ^ x00, 16);
                x08 += x12;
                x04 = Integers.RotateLeft(x04 ^ x08, 12);
                x00 += x04;
                x12 = Integers.RotateLeft(x12 ^ x00, 8);
                x08 += x12;
                x04 = Integers.RotateLeft(x04 ^ x08, 7);
                x01 += x05;
                x13 = Integers.RotateLeft(x13 ^ x01, 16);
                x09 += x13;
                x05 = Integers.RotateLeft(x05 ^ x09, 12);
                x01 += x05;
                x13 = Integers.RotateLeft(x13 ^ x01, 8);
                x09 += x13;
                x05 = Integers.RotateLeft(x05 ^ x09, 7);
                x02 += x06;
                x14 = Integers.RotateLeft(x14 ^ x02, 16);
                x10 += x14;
                x06 = Integers.RotateLeft(x06 ^ x10, 12);
                x02 += x06;
                x14 = Integers.RotateLeft(x14 ^ x02, 8);
                x10 += x14;
                x06 = Integers.RotateLeft(x06 ^ x10, 7);
                x03 += x07;
                x15 = Integers.RotateLeft(x15 ^ x03, 16);
                x11 += x15;
                x07 = Integers.RotateLeft(x07 ^ x11, 12);
                x03 += x07;
                x15 = Integers.RotateLeft(x15 ^ x03, 8);
                x11 += x15;
                x07 = Integers.RotateLeft(x07 ^ x11, 7);
                x00 += x05;
                x15 = Integers.RotateLeft(x15 ^ x00, 16);
                x10 += x15;
                x05 = Integers.RotateLeft(x05 ^ x10, 12);
                x00 += x05;
                x15 = Integers.RotateLeft(x15 ^ x00, 8);
                x10 += x15;
                x05 = Integers.RotateLeft(x05 ^ x10, 7);
                x01 += x06;
                x12 = Integers.RotateLeft(x12 ^ x01, 16);
                x11 += x12;
                x06 = Integers.RotateLeft(x06 ^ x11, 12);
                x01 += x06;
                x12 = Integers.RotateLeft(x12 ^ x01, 8);
                x11 += x12;
                x06 = Integers.RotateLeft(x06 ^ x11, 7);
                x02 += x07;
                x13 = Integers.RotateLeft(x13 ^ x02, 16);
                x08 += x13;
                x07 = Integers.RotateLeft(x07 ^ x08, 12);
                x02 += x07;
                x13 = Integers.RotateLeft(x13 ^ x02, 8);
                x08 += x13;
                x07 = Integers.RotateLeft(x07 ^ x08, 7);
                x03 += x04;
                x14 = Integers.RotateLeft(x14 ^ x03, 16);
                x09 += x14;
                x04 = Integers.RotateLeft(x04 ^ x09, 12);
                x03 += x04;
                x14 = Integers.RotateLeft(x14 ^ x03, 8);
                x09 += x14;
                x04 = Integers.RotateLeft(x04 ^ x09, 7);
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
    }
}
#pragma warning restore
#endif
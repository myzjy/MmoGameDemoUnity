#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.PlatformSupport.IL2CPP;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;

namespace BestHTTP.Connections.TLS.Crypto.Impl
{
    /// <summary>
    /// Implementation of Daniel J. Bernstein's ChaCha stream cipher.
    /// </summary>
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [Il2CppEagerStaticClassConstruction]
    public sealed class FastChaCha7539Engine
        : FastSalsa20Engine
    {
        /// <summary>
        /// Creates a 20 rounds ChaCha engine.
        /// </summary>
        public FastChaCha7539Engine()
        {
        }

        public override string AlgorithmName => "ChaCha7539";

        protected override int NonceSize => 12;

        protected override void AdvanceCounter()
        {
            if (++EngineState[12] == 0)
            {
                throw new InvalidOperationException("尝试增加计数器过去 2^32.");
            }
        }

        protected override void ResetCounter()
        {
            EngineState[12] = 0;
        }

        protected override void SetKey(byte[] keyBytes, byte[] ivBytes)
        {
            if (keyBytes != null)
            {
                if (keyBytes.Length != 32)
                {
                    throw new ArgumentException($"{AlgorithmName} 需要 256 bit key");
                }

                PackTauOrSigma(keyBytes.Length, EngineState, 0);

                // Key
                Pack.LE_To_UInt32(keyBytes, 0, EngineState, 4, 8);
            }

            // IV
            Pack.LE_To_UInt32(ivBytes, 0, EngineState, 13, 3);
        }

        protected override void GenerateKeyStream(byte[] output)
        {
            FastChaChaEngine.ChaChaCore(Rounds, EngineState, X);
            Pack.UInt32_To_LE(X, output, 0);
        }
    }
}

#pragma warning restore
#endif
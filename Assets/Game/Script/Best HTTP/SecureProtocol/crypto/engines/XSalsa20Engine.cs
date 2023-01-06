#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /// <summary>
    /// Implementation of Daniel J. Bernstein's XSalsa20 stream cipher - Salsa20 with an extended nonce.
    /// </summary>
    /// <remarks>
    /// XSalsa20 requires a 256 bit key, and a 192 bit nonce.
    /// </remarks>
    public class XSalsa20Engine
        : Salsa20Engine
    {
        public override string AlgorithmName
        {
            get { return "XSalsa20"; }
        }

        protected override int NonceSize
        {
            get { return 24; }
        }

        /// <summary>
        /// XSalsa20 key generation: process 256 bit input key and 128 bits of the input nonce
        /// using a core Salsa20 function without input addition to produce 256 bit working key
        /// and use that with the remaining 64 bits of nonce to initialize a standard Salsa20 engine state.
        /// </summary>
        protected override void SetKey(byte[] keyBytes, byte[] ivBytes)
        {
            if (keyBytes == null)
                throw new ArgumentException(AlgorithmName + " doesn't support re-init with null key");

            if (keyBytes.Length != 32)
                throw new ArgumentException(AlgorithmName + " requires a 256 bit key");

            // Set key for HSalsa20
            base.SetKey(keyBytes, ivBytes);

            // Pack next 64 bits of IV into engine state instead of counter
            Pack.LE_To_UInt32(ivBytes, 8, EngineState, 8, 2);

            // Process engine state to generate Salsa20 key
            uint[] hsalsa20Out = new uint[EngineState.Length];
            SalsaCore(20, EngineState, hsalsa20Out);

            // Set new key, removing addition in last round of salsaCore
            EngineState[1] = hsalsa20Out[0] - EngineState[0];
            EngineState[2] = hsalsa20Out[5] - EngineState[5];
            EngineState[3] = hsalsa20Out[10] - EngineState[10];
            EngineState[4] = hsalsa20Out[15] - EngineState[15];

            EngineState[11] = hsalsa20Out[6] - EngineState[6];
            EngineState[12] = hsalsa20Out[7] - EngineState[7];
            EngineState[13] = hsalsa20Out[8] - EngineState[8];
            EngineState[14] = hsalsa20Out[9] - EngineState[9];

            // Last 64 bits of input IV
            Pack.LE_To_UInt32(ivBytes, 16, EngineState, 6, 2);
        }
    }
}
#pragma warning restore
#endif
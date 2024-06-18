#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
using System;
using BestHTTP.PlatformSupport.Memory;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Tls;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Tls.Crypto.Impl;

namespace BestHTTP.Connections.TLS.Crypto.Impl
{
    public sealed class FastBcChaCha20Poly1305
        : TlsAeadCipherImpl
    {
        private static readonly byte[] Zeroes = new byte[15];

        private readonly FastChaCha7539Engine _mCipher = new FastChaCha7539Engine();
        private readonly FastPoly1305 _mMac = new FastPoly1305();

        private readonly bool _mIsEncrypting;

        private int _mAdditionalDataLength;

        public FastBcChaCha20Poly1305(bool isEncrypting)
        {
            _mIsEncrypting = isEncrypting;
        }

        public int DoFinal(byte[] input, int inputOffset, int inputLength, byte[] output, int outputOffset)
        {
            if (_mIsEncrypting)
            {
                int ciphertextLength = inputLength;

                _mCipher.ProcessBytes(input, inputOffset, inputLength, output, outputOffset);
                int outputLength = inputLength;

                if (ciphertextLength != outputLength)
                    throw new InvalidOperationException();

                UpdateMac(output, outputOffset, ciphertextLength);

                byte[] lengths = BufferPool.Get(16, true);
                Pack.UInt64_To_LE((ulong)_mAdditionalDataLength, lengths, 0);
                Pack.UInt64_To_LE((ulong)ciphertextLength, lengths, 8);
                _mMac.BlockUpdate(lengths, 0, 16);

                BufferPool.Release(lengths);

                _mMac.DoFinal(output, outputOffset + ciphertextLength);

                return ciphertextLength + 16;
            }
            else
            {
                int ciphertextLength = inputLength - 16;

                UpdateMac(input, inputOffset, ciphertextLength);

                byte[] expectedMac = BufferPool.Get(16, true);
                Pack.UInt64_To_LE((ulong)_mAdditionalDataLength, expectedMac, 0);
                Pack.UInt64_To_LE((ulong)ciphertextLength, expectedMac, 8);
                _mMac.BlockUpdate(expectedMac, 0, 16);
                _mMac.DoFinal(expectedMac, 0);

                bool badMac =
                    !TlsUtilities.ConstantTimeAreEqual(16, expectedMac, 0, input, inputOffset + ciphertextLength);
                BufferPool.Release(expectedMac);
                if (badMac)
                    throw new TlsFatalAlert(AlertDescription.bad_record_mac);

                _mCipher.ProcessBytes(input, inputOffset, ciphertextLength, output, outputOffset);
                int outputLength = ciphertextLength;

                if (ciphertextLength != outputLength)
                    throw new InvalidOperationException();

                return ciphertextLength;
            }
        }

        public int GetOutputSize(int inputLength)
        {
            return _mIsEncrypting ? inputLength + 16 : inputLength - 16;
        }

        public void Init(byte[] nonce, int macSize, byte[] additionalData)
        {
            if (nonce is not { Length: 12 } || macSize != 16)
                throw new TlsFatalAlert(AlertDescription.internal_error);

            _mCipher.Init(_mIsEncrypting, new FastParametersWithIV(null, nonce));
            InitMac();
            if (additionalData == null)
            {
                _mAdditionalDataLength = 0;
            }
            else
            {
                _mAdditionalDataLength = additionalData.Length;
                UpdateMac(additionalData, 0, additionalData.Length);
            }
        }

        public void SetKey(byte[] key, int keyOff, int keyLen)
        {
            NoCopyKeyParameter cipherKey = new NoCopyKeyParameter(key, keyOff, keyLen);
            _mCipher.Init(_mIsEncrypting, new FastParametersWithIV(cipherKey, Zeroes, 0, 12));
        }

        private void InitMac()
        {
            byte[] firstBlock = new byte[64];
            _mCipher.ProcessBytes(firstBlock, 0, 64, firstBlock, 0);
            _mMac.Init(new NoCopyKeyParameter(firstBlock, 0, 32));
            Array.Clear(firstBlock, 0, firstBlock.Length);
        }

        private void UpdateMac(byte[] buf, int off, int len)
        {
            _mMac.BlockUpdate(buf, off, len);

            int partial = len % 16;
            if (partial != 0)
            {
                _mMac.BlockUpdate(Zeroes, 0, 16 - partial);
            }
        }
    }
}
#endif
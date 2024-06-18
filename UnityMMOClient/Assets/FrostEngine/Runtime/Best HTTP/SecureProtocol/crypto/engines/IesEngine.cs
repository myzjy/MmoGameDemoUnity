#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
    * support class for constructing intergrated encryption ciphers
    * for doing basic message exchanges on top of key agreement ciphers
    */
    public class IesEngine
    {
        private readonly IBasicAgreement _agree;
        private readonly BufferedBlockCipher _cipher;
        private readonly IDerivationFunction _kdf;
        private readonly IMac _mac;
        private readonly byte[] _macBuf;

        private bool _forEncryption;
        private IesParameters _param;
        private ICipherParameters _privParam, _pubParam;

        /**
        * set up for use with stream mode, where the key derivation function
        * is used to provide a stream of bytes to xor with the message.
        *
        * @param agree the key agreement used as the basis for the encryption
        * @param kdf the key derivation function used for byte generation
        * @param mac the message authentication code generator for the message
        */
        public IesEngine(
            IBasicAgreement agree,
            IDerivationFunction kdf,
            IMac mac)
        {
            this._agree = agree;
            this._kdf = kdf;
            this._mac = mac;
            this._macBuf = new byte[mac.GetMacSize()];
//            this.cipher = null;
        }

        /**
        * set up for use in conjunction with a block cipher to handle the
        * message.
        *
        * @param agree the key agreement used as the basis for the encryption
        * @param kdf the key derivation function used for byte generation
        * @param mac the message authentication code generator for the message
        * @param cipher the cipher to used for encrypting the message
        */
        public IesEngine(
            IBasicAgreement agree,
            IDerivationFunction kdf,
            IMac mac,
            BufferedBlockCipher cipher)
        {
            this._agree = agree;
            this._kdf = kdf;
            this._mac = mac;
            this._macBuf = new byte[mac.GetMacSize()];
            this._cipher = cipher;
        }

        /**
        * Initialise the encryptor.
        *
        * @param forEncryption whether or not this is encryption/decryption.
        * @param privParam our private key parameters
        * @param pubParam the recipient's/sender's public key parameters
        * @param param encoding and derivation parameters.
        */
        public virtual void Init(
            bool forEncryption,
            ICipherParameters privParameters,
            ICipherParameters pubParameters,
            ICipherParameters iesParameters)
        {
            this._forEncryption = forEncryption;
            this._privParam = privParameters;
            this._pubParam = pubParameters;
            this._param = (IesParameters)iesParameters;
        }

        private byte[] DecryptBlock(
            byte[] inEnc,
            int inOff,
            int inLen,
            byte[] z)
        {
            byte[] m = null;
            KeyParameter macKey = null;
            KdfParameters kParam = new KdfParameters(z, _param.GetDerivationV());
            int macKeySize = _param.MacKeySize;

            _kdf.Init(kParam);

            // Ensure that the length of the input is greater than the MAC in bytes
            if (inLen < _mac.GetMacSize())
                throw new InvalidCipherTextException("Length of input must be greater than the MAC");

            inLen -= _mac.GetMacSize();

            if (_cipher == null) // stream mode
            {
                byte[] buffer = GenerateKdfBytes(kParam, inLen + (macKeySize / 8));

                m = new byte[inLen];

                for (int i = 0; i != inLen; i++)
                {
                    m[i] = (byte)(inEnc[inOff + i] ^ buffer[i]);
                }

                macKey = new KeyParameter(buffer, inLen, (macKeySize / 8));
            }
            else
            {
                int cipherKeySize = ((IesWithCipherParameters)_param).CipherKeySize;
                byte[] buffer = GenerateKdfBytes(kParam, (cipherKeySize / 8) + (macKeySize / 8));

                _cipher.Init(false, new KeyParameter(buffer, 0, (cipherKeySize / 8)));

                m = _cipher.DoFinal(inEnc, inOff, inLen);

                macKey = new KeyParameter(buffer, (cipherKeySize / 8), (macKeySize / 8));
            }

            byte[] macIv = _param.GetEncodingV();

            _mac.Init(macKey);
            _mac.BlockUpdate(inEnc, inOff, inLen);
            _mac.BlockUpdate(macIv, 0, macIv.Length);
            _mac.DoFinal(_macBuf, 0);

            inOff += inLen;

            byte[] t1 = Arrays.CopyOfRange(inEnc, inOff, inOff + _macBuf.Length);

            if (!Arrays.ConstantTimeAreEqual(t1, _macBuf))
                throw (new InvalidCipherTextException("Invalid MAC."));

            return m;
        }

        private byte[] EncryptBlock(
            byte[] input,
            int inOff,
            int inLen,
            byte[] z)
        {
            byte[] c = null;
            KeyParameter macKey = null;
            KdfParameters kParam = new KdfParameters(z, _param.GetDerivationV());
            int cTextLength = 0;
            int macKeySize = _param.MacKeySize;

            if (_cipher == null) // stream mode
            {
                byte[] buffer = GenerateKdfBytes(kParam, inLen + (macKeySize / 8));

                c = new byte[inLen + _mac.GetMacSize()];
                cTextLength = inLen;

                for (int i = 0; i != inLen; i++)
                {
                    c[i] = (byte)(input[inOff + i] ^ buffer[i]);
                }

                macKey = new KeyParameter(buffer, inLen, (macKeySize / 8));
            }
            else
            {
                int cipherKeySize = ((IesWithCipherParameters)_param).CipherKeySize;
                byte[] buffer = GenerateKdfBytes(kParam, (cipherKeySize / 8) + (macKeySize / 8));

                _cipher.Init(true, new KeyParameter(buffer, 0, (cipherKeySize / 8)));

                cTextLength = _cipher.GetOutputSize(inLen);
                byte[] tmp = new byte[cTextLength];

                int len = _cipher.ProcessBytes(input, inOff, inLen, tmp, 0);
                len += _cipher.DoFinal(tmp, len);

                c = new byte[len + _mac.GetMacSize()];
                cTextLength = len;

                Array.Copy(tmp, 0, c, 0, len);

                macKey = new KeyParameter(buffer, (cipherKeySize / 8), (macKeySize / 8));
            }

            byte[] macIv = _param.GetEncodingV();

            _mac.Init(macKey);
            _mac.BlockUpdate(c, 0, cTextLength);
            _mac.BlockUpdate(macIv, 0, macIv.Length);
            //
            // return the message and it's MAC
            //
            _mac.DoFinal(c, cTextLength);
            return c;
        }

        private byte[] GenerateKdfBytes(
            KdfParameters kParam,
            int length)
        {
            byte[] buf = new byte[length];

            _kdf.Init(kParam);

            _kdf.GenerateBytes(buf, 0, buf.Length);

            return buf;
        }

        public virtual byte[] ProcessBlock(
            byte[] input,
            int inOff,
            int inLen)
        {
            _agree.Init(_privParam);

            BigInteger z = _agree.CalculateAgreement(_pubParam);

            byte[] zBytes = BigIntegers.AsUnsignedByteArray(_agree.GetFieldSize(), z);

            try
            {
                return _forEncryption
                    ? EncryptBlock(input, inOff, inLen, zBytes)
                    : DecryptBlock(input, inOff, inLen, zBytes);
            }
            finally
            {
                Array.Clear(zBytes, 0, zBytes.Length);
            }
        }
    }
}
#pragma warning restore
#endif
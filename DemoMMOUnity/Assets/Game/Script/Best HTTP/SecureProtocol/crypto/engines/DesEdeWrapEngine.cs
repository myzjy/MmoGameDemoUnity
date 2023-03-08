#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Digests;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Security;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
    * Wrap keys according to
    * <a href="http://www.ietf.org/internet-drafts/draft-ietf-smime-key-wrap-01.txt">
    * draft-ietf-smime-key-wrap-01.txt</a>.
    * <p>
    * Note:
    * <ul>
    * <li>this is based on a draft, and as such is subject to change - don't use this class for anything requiring long term storage.</li>
    * <li>if you are using this to wrap triple-des keys you need to set the
    * parity bits on the key and, if it's a two-key triple-des key, pad it
    * yourself.</li>
    * </ul>
	* </p>
    */
    public class DesEdeWrapEngine
        : IWrapper
    {
        /** Field IV2           */
        private static readonly byte[] Iv2 =
        {
            (byte)0x4a, (byte)0xdd, (byte)0xa2,
            (byte)0x2c, (byte)0x79, (byte)0xe8,
            (byte)0x21, (byte)0x05
        };

        private readonly byte[] _digest = new byte[20];

        //
        // checksum digest
        //
        private readonly IDigest _sha1 = new Sha1Digest();

        /** Field engine */
        private CbcBlockCipher _engine;

        /** Field forWrapping */
        private bool _forWrapping;

        /** Field iv */
        private byte[] _iv;

        /** Field param */
        private KeyParameter _param;

        /** Field paramPlusIV */
        private ParametersWithIV _paramPlusIv;

        /**
        * Method init
        *
        * @param forWrapping
        * @param param
        */
        public virtual void Init(
            bool forWrapping,
            ICipherParameters parameters)
        {
            this._forWrapping = forWrapping;
            this._engine = new CbcBlockCipher(new DesEdeEngine());

            SecureRandom sr;
            if (parameters is ParametersWithRandom)
            {
                ParametersWithRandom pr = (ParametersWithRandom)parameters;
                parameters = pr.Parameters;
                sr = pr.Random;
            }
            else
            {
                sr = new SecureRandom();
            }

            if (parameters is KeyParameter)
            {
                this._param = (KeyParameter)parameters;
                if (this._forWrapping)
                {
                    // Hm, we have no IV but we want to wrap ?!?
                    // well, then we have to create our own IV.
                    this._iv = new byte[8];
                    sr.NextBytes(_iv);

                    this._paramPlusIv = new ParametersWithIV(this._param, this._iv);
                }
            }
            else if (parameters is ParametersWithIV)
            {
                if (!forWrapping)
                    throw new ArgumentException("You should not supply an IV for unwrapping");

                this._paramPlusIv = (ParametersWithIV)parameters;
                this._iv = this._paramPlusIv.GetIV();
                this._param = (KeyParameter)this._paramPlusIv.Parameters;

                if (this._iv.Length != 8)
                    throw new ArgumentException("IV is not 8 octets", "parameters");
            }
        }

        /**
        * Method GetAlgorithmName
        *
        * @return
        */
        public virtual string AlgorithmName
        {
            get { return "DESede"; }
        }

        /**
        * Method wrap
        *
        * @param in
        * @param inOff
        * @param inLen
        * @return
        */
        public virtual byte[] Wrap(
            byte[] input,
            int inOff,
            int length)
        {
            if (!_forWrapping)
            {
                throw new InvalidOperationException("Not initialized for wrapping");
            }

            byte[] keyToBeWrapped = new byte[length];
            Array.Copy(input, inOff, keyToBeWrapped, 0, length);

            // Compute the CMS Key Checksum, (section 5.6.1), call this CKS.
            byte[] cks = CalculateCmsKeyChecksum(keyToBeWrapped);

            // Let WKCKS = WK || CKS where || is concatenation.
            byte[] wkcks = new byte[keyToBeWrapped.Length + cks.Length];
            Array.Copy(keyToBeWrapped, 0, wkcks, 0, keyToBeWrapped.Length);
            Array.Copy(cks, 0, wkcks, keyToBeWrapped.Length, cks.Length);

            // Encrypt WKCKS in CBC mode using KEK as the key and IV as the
            // initialization vector. Call the results TEMP1.

            int blockSize = _engine.GetBlockSize();

            if (wkcks.Length % blockSize != 0)
                throw new InvalidOperationException("Not multiple of block length");

            _engine.Init(true, _paramPlusIv);

            byte[] temp1 = new byte[wkcks.Length];

            for (int currentBytePos = 0; currentBytePos != wkcks.Length; currentBytePos += blockSize)
            {
                _engine.ProcessBlock(wkcks, currentBytePos, temp1, currentBytePos);
            }

            // Let TEMP2 = IV || TEMP1.
            byte[] temp2 = new byte[this._iv.Length + temp1.Length];
            Array.Copy(this._iv, 0, temp2, 0, this._iv.Length);
            Array.Copy(temp1, 0, temp2, this._iv.Length, temp1.Length);

            // Reverse the order of the octets in TEMP2 and call the result TEMP3.
            byte[] temp3 = Reverse(temp2);

            // Encrypt TEMP3 in CBC mode using the KEK and an initialization vector
            // of 0x 4a dd a2 2c 79 e8 21 05. The resulting cipher text is the desired
            // result. It is 40 octets long if a 168 bit key is being wrapped.
            ParametersWithIV param2 = new ParametersWithIV(this._param, Iv2);
            this._engine.Init(true, param2);

            for (int currentBytePos = 0; currentBytePos != temp3.Length; currentBytePos += blockSize)
            {
                _engine.ProcessBlock(temp3, currentBytePos, temp3, currentBytePos);
            }

            return temp3;
        }

        /**
        * Method unwrap
        *
        * @param in
        * @param inOff
        * @param inLen
        * @return
        * @throws InvalidCipherTextException
        */
        public virtual byte[] Unwrap(
            byte[] input,
            int inOff,
            int length)
        {
            if (_forWrapping)
            {
                throw new InvalidOperationException("Not set for unwrapping");
            }

            if (input == null)
            {
                throw new InvalidCipherTextException("Null pointer as ciphertext");
            }

            int blockSize = _engine.GetBlockSize();

            if (length % blockSize != 0)
            {
                throw new InvalidCipherTextException("Ciphertext not multiple of " + blockSize);
            }

            /*
            // Check if the length of the cipher text is reasonable given the key
            // type. It must be 40 bytes for a 168 bit key and either 32, 40, or
            // 48 bytes for a 128, 192, or 256 bit key. If the length is not supported
            // or inconsistent with the algorithm for which the key is intended,
            // return error.
            //
            // we do not accept 168 bit keys. it has to be 192 bit.
            int lengthA = (estimatedKeyLengthInBit / 8) + 16;
            int lengthB = estimatedKeyLengthInBit % 8;
            if ((lengthA != keyToBeUnwrapped.Length) || (lengthB != 0)) {
                throw new XMLSecurityException("empty");
            }
            */

            // Decrypt the cipher text with TRIPLedeS in CBC mode using the KEK
            // and an initialization vector (IV) of 0x4adda22c79e82105. Call the output TEMP3.
            ParametersWithIV param2 = new ParametersWithIV(this._param, Iv2);
            this._engine.Init(false, param2);

            byte[] temp3 = new byte[length];

            for (int currentBytePos = 0; currentBytePos != temp3.Length; currentBytePos += blockSize)
            {
                _engine.ProcessBlock(input, inOff + currentBytePos, temp3, currentBytePos);
            }

            // Reverse the order of the octets in TEMP3 and call the result TEMP2.
            byte[] temp2 = Reverse(temp3);

            // Decompose TEMP2 into IV, the first 8 octets, and TEMP1, the remaining octets.
            this._iv = new byte[8];
            byte[] temp1 = new byte[temp2.Length - 8];
            Array.Copy(temp2, 0, this._iv, 0, 8);
            Array.Copy(temp2, 8, temp1, 0, temp2.Length - 8);

            // Decrypt TEMP1 using TRIPLedeS in CBC mode using the KEK and the IV
            // found in the previous step. Call the result WKCKS.
            this._paramPlusIv = new ParametersWithIV(this._param, this._iv);
            this._engine.Init(false, this._paramPlusIv);

            byte[] wkcks = new byte[temp1.Length];

            for (int currentBytePos = 0; currentBytePos != wkcks.Length; currentBytePos += blockSize)
            {
                _engine.ProcessBlock(temp1, currentBytePos, wkcks, currentBytePos);
            }

            // Decompose WKCKS. CKS is the last 8 octets and WK, the wrapped key, are
            // those octets before the CKS.
            byte[] result = new byte[wkcks.Length - 8];
            byte[] ckStoBeVerified = new byte[8];
            Array.Copy(wkcks, 0, result, 0, wkcks.Length - 8);
            Array.Copy(wkcks, wkcks.Length - 8, ckStoBeVerified, 0, 8);

            // Calculate a CMS Key Checksum, (section 5.6.1), over the WK and compare
            // with the CKS extracted in the above step. If they are not equal, return error.
            if (!CheckCmsKeyChecksum(result, ckStoBeVerified))
            {
                throw new InvalidCipherTextException(
                    "Checksum inside ciphertext is corrupted");
            }

            // WK is the wrapped key, now extracted for use in data decryption.
            return result;
        }

        /**
        * Some key wrap algorithms make use of the Key Checksum defined
        * in CMS [CMS-Algorithms]. This is used to provide an integrity
        * check value for the key being wrapped. The algorithm is
        *
        * - Compute the 20 octet SHA-1 hash on the key being wrapped.
        * - Use the first 8 octets of this hash as the checksum value.
        *
        * @param key
        * @return
        * @throws Exception
        * @see http://www.w3.org/TR/xmlenc-core/#sec-CMSKeyChecksum
        */
        private byte[] CalculateCmsKeyChecksum(
            byte[] key)
        {
            _sha1.BlockUpdate(key, 0, key.Length);
            _sha1.DoFinal(_digest, 0);

            byte[] result = new byte[8];
            Array.Copy(_digest, 0, result, 0, 8);
            return result;
        }

        /**
        * @param key
        * @param checksum
        * @return
        * @see http://www.w3.org/TR/xmlenc-core/#sec-CMSKeyChecksum
        */
        private bool CheckCmsKeyChecksum(
            byte[] key,
            byte[] checksum)
        {
            return Arrays.ConstantTimeAreEqual(CalculateCmsKeyChecksum(key), checksum);
        }

        private static byte[] Reverse(byte[] bs)
        {
            byte[] result = new byte[bs.Length];
            for (int i = 0; i < bs.Length; i++)
            {
                result[i] = bs[bs.Length - (i + 1)];
            }

            return result;
        }
    }
}
#pragma warning restore
#endif
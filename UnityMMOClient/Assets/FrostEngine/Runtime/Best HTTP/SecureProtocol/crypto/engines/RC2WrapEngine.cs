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
	 * Wrap keys according to RFC 3217 - RC2 mechanism
	 */
    public class Rc2WrapEngine
        : IWrapper
    {
        /** Field IV2           */
        private static readonly byte[] Iv2 =
        {
            (byte)0x4a, (byte)0xdd, (byte)0xa2,
            (byte)0x2c, (byte)0x79, (byte)0xe8,
            (byte)0x21, (byte)0x05
        };

        byte[] _digest = new byte[20];

        /** Field engine */
        private CbcBlockCipher _engine;

        /** Field forWrapping */
        private bool _forWrapping;

        /** Field iv */
        private byte[] _iv;

        /** Field param */
        private ICipherParameters _parameters;

        /** Field paramPlusIV */
        private ParametersWithIV _paramPlusIv;

        //
        // checksum digest
        //
        IDigest _sha1 = new Sha1Digest();

        private SecureRandom _sr;

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
            this._engine = new CbcBlockCipher(new Rc2Engine());

            if (parameters is ParametersWithRandom)
            {
                ParametersWithRandom pWithR = (ParametersWithRandom)parameters;
                _sr = pWithR.Random;
                parameters = pWithR.Parameters;
            }
            else
            {
                _sr = new SecureRandom();
            }

            if (parameters is ParametersWithIV)
            {
                if (!forWrapping)
                    throw new ArgumentException("You should not supply an IV for unwrapping");

                this._paramPlusIv = (ParametersWithIV)parameters;
                this._iv = this._paramPlusIv.GetIV();
                this._parameters = this._paramPlusIv.Parameters;

                if (this._iv.Length != 8)
                    throw new ArgumentException("IV is not 8 octets");
            }
            else
            {
                this._parameters = parameters;

                if (this._forWrapping)
                {
                    // Hm, we have no IV but we want to wrap ?!?
                    // well, then we have to create our own IV.
                    this._iv = new byte[8];
                    _sr.NextBytes(_iv);
                    this._paramPlusIv = new ParametersWithIV(this._parameters, this._iv);
                }
            }
        }

        /**
		* Method GetAlgorithmName
		*
		* @return
		*/
        public virtual string AlgorithmName
        {
            get { return "RC2"; }
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

            int len = length + 1;
            if ((len % 8) != 0)
            {
                len += 8 - (len % 8);
            }

            byte[] keyToBeWrapped = new byte[len];

            keyToBeWrapped[0] = (byte)length;
            Array.Copy(input, inOff, keyToBeWrapped, 1, length);

            byte[] pad = new byte[keyToBeWrapped.Length - length - 1];

            if (pad.Length > 0)
            {
                _sr.NextBytes(pad);
                Array.Copy(pad, 0, keyToBeWrapped, length + 1, pad.Length);
            }

            // Compute the CMS Key Checksum, (section 5.6.1), call this CKS.
            byte[] cks = CalculateCmsKeyChecksum(keyToBeWrapped);

            // Let WKCKS = WK || CKS where || is concatenation.
            byte[] wkcks = new byte[keyToBeWrapped.Length + cks.Length];

            Array.Copy(keyToBeWrapped, 0, wkcks, 0, keyToBeWrapped.Length);
            Array.Copy(cks, 0, wkcks, keyToBeWrapped.Length, cks.Length);

            // Encrypt WKCKS in CBC mode using KEK as the key and IV as the
            // initialization vector. Call the results TEMP1.
            byte[] temp1 = new byte[wkcks.Length];

            Array.Copy(wkcks, 0, temp1, 0, wkcks.Length);

            int noOfBlocks = wkcks.Length / _engine.GetBlockSize();
            int extraBytes = wkcks.Length % _engine.GetBlockSize();

            if (extraBytes != 0)
            {
                throw new InvalidOperationException("Not multiple of block length");
            }

            _engine.Init(true, _paramPlusIv);

            for (int i = 0; i < noOfBlocks; i++)
            {
                int currentBytePos = i * _engine.GetBlockSize();

                _engine.ProcessBlock(temp1, currentBytePos, temp1, currentBytePos);
            }

            // Left TEMP2 = IV || TEMP1.
            byte[] temp2 = new byte[this._iv.Length + temp1.Length];

            Array.Copy(this._iv, 0, temp2, 0, this._iv.Length);
            Array.Copy(temp1, 0, temp2, this._iv.Length, temp1.Length);

            // Reverse the order of the octets in TEMP2 and call the result TEMP3.
            byte[] temp3 = new byte[temp2.Length];

            for (int i = 0; i < temp2.Length; i++)
            {
                temp3[i] = temp2[temp2.Length - (i + 1)];
            }

            // Encrypt TEMP3 in CBC mode using the KEK and an initialization vector
            // of 0x 4a dd a2 2c 79 e8 21 05. The resulting cipher text is the desired
            // result. It is 40 octets long if a 168 bit key is being wrapped.
            ParametersWithIV param2 = new ParametersWithIV(this._parameters, Iv2);

            this._engine.Init(true, param2);

            for (int i = 0; i < noOfBlocks + 1; i++)
            {
                int currentBytePos = i * _engine.GetBlockSize();

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

            if (length % _engine.GetBlockSize() != 0)
            {
                throw new InvalidCipherTextException("Ciphertext not multiple of "
                                                     + _engine.GetBlockSize());
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
            ParametersWithIV param2 = new ParametersWithIV(this._parameters, Iv2);

            this._engine.Init(false, param2);

            byte[] temp3 = new byte[length];

            Array.Copy(input, inOff, temp3, 0, length);

            for (int i = 0; i < (temp3.Length / _engine.GetBlockSize()); i++)
            {
                int currentBytePos = i * _engine.GetBlockSize();

                _engine.ProcessBlock(temp3, currentBytePos, temp3, currentBytePos);
            }

            // Reverse the order of the octets in TEMP3 and call the result TEMP2.
            byte[] temp2 = new byte[temp3.Length];

            for (int i = 0; i < temp3.Length; i++)
            {
                temp2[i] = temp3[temp3.Length - (i + 1)];
            }

            // Decompose TEMP2 into IV, the first 8 octets, and TEMP1, the remaining octets.
            this._iv = new byte[8];

            byte[] temp1 = new byte[temp2.Length - 8];

            Array.Copy(temp2, 0, this._iv, 0, 8);
            Array.Copy(temp2, 8, temp1, 0, temp2.Length - 8);

            // Decrypt TEMP1 using TRIPLedeS in CBC mode using the KEK and the IV
            // found in the previous step. Call the result WKCKS.
            this._paramPlusIv = new ParametersWithIV(this._parameters, this._iv);

            this._engine.Init(false, this._paramPlusIv);

            byte[] lcekpadicv = new byte[temp1.Length];

            Array.Copy(temp1, 0, lcekpadicv, 0, temp1.Length);

            for (int i = 0; i < (lcekpadicv.Length / _engine.GetBlockSize()); i++)
            {
                int currentBytePos = i * _engine.GetBlockSize();

                _engine.ProcessBlock(lcekpadicv, currentBytePos, lcekpadicv, currentBytePos);
            }

            // Decompose LCEKPADICV. CKS is the last 8 octets and WK, the wrapped key, are
            // those octets before the CKS.
            byte[] result = new byte[lcekpadicv.Length - 8];
            byte[] ckStoBeVerified = new byte[8];

            Array.Copy(lcekpadicv, 0, result, 0, lcekpadicv.Length - 8);
            Array.Copy(lcekpadicv, lcekpadicv.Length - 8, ckStoBeVerified, 0, 8);

            // Calculate a CMS Key Checksum, (section 5.6.1), over the WK and compare
            // with the CKS extracted in the above step. If they are not equal, return error.
            if (!CheckCmsKeyChecksum(result, ckStoBeVerified))
            {
                throw new InvalidCipherTextException(
                    "Checksum inside ciphertext is corrupted");
            }

            if ((result.Length - ((result[0] & 0xff) + 1)) > 7)
            {
                throw new InvalidCipherTextException(
                    "too many pad bytes (" + (result.Length - ((result[0] & 0xff) + 1)) + ")");
            }

            // CEK is the wrapped key, now extracted for use in data decryption.
            byte[] cek = new byte[result[0]];
            Array.Copy(result, 1, cek, 0, cek.Length);
            return cek;
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
    }
}
#pragma warning restore
#endif
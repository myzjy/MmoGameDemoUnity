#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
     * A class that provides CAST6 key encryption operations,
     * such as encoding data and generating keys.
     *
     * All the algorithms herein are from the Internet RFC
     *
     * RFC2612 - CAST6 (128bit block, 128-256bit key)
     *
     * and implement a simplified cryptography interface.
     */
    public sealed class Cast6Engine
        : Cast5Engine
    {
        //====================================
        // Useful constants
        //====================================
        private const int Rounds = 12;
        private const int BlockSize = 16; // bytes = 128 bits
        private uint[] _km = new uint[Rounds * 4]; // the masking round key(s)

        /*
        * Put the round and mask keys into an array.
        * Kr0[i] => _Kr[i*4 + 0]
        */
        private int[] _kr = new int[Rounds * 4]; // the rotating round key(s)
        private uint[] _tm = new uint[24 * 8];

        /*
        * Key setup
        */
        private int[] _tr = new int[24 * 8];
        private uint[] _workingKey = new uint[8];

        public Cast6Engine()
        {
        }

        public override string AlgorithmName
        {
            get { return "CAST6"; }
        }

        public override void Reset()
        {
        }

        public override int GetBlockSize()
        {
            return BlockSize;
        }

        //==================================
        // Private Implementation
        //==================================
        /*
        * Creates the subkeys using the same nomenclature
        * as described in RFC2612.
        *
        * See section 2.4
        */
        internal override void SetKey(
            byte[] key)
        {
            uint cm = 0x5a827999;
            uint mm = 0x6ed9eba1;
            int cr = 19;
            int mr = 17;
            /*
            * Determine the key size here, if required
            *
            * if keysize < 256 bytes, pad with 0
            *
            * Typical key sizes => 128, 160, 192, 224, 256
            */
            for (int i = 0; i < 24; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    _tm[i * 8 + j] = cm;
                    cm += mm; //mod 2^32;
                    _tr[i * 8 + j] = cr;
                    cr = (cr + mr) & 0x1f; // mod 32
                }
            }

            byte[] tmpKey = new byte[64];
            key.CopyTo(tmpKey, 0);

            // now create ABCDEFGH
            for (int i = 0; i < 8; i++)
            {
                _workingKey[i] = Pack.BE_To_UInt32(tmpKey, i * 4);
            }

            // Generate the key schedule
            for (int i = 0; i < 12; i++)
            {
                // KAPPA <- W2i(KAPPA)
                int i2 = i * 2 * 8;
                _workingKey[6] ^= F1(_workingKey[7], _tm[i2], _tr[i2]);
                _workingKey[5] ^= F2(_workingKey[6], _tm[i2 + 1], _tr[i2 + 1]);
                _workingKey[4] ^= F3(_workingKey[5], _tm[i2 + 2], _tr[i2 + 2]);
                _workingKey[3] ^= F1(_workingKey[4], _tm[i2 + 3], _tr[i2 + 3]);
                _workingKey[2] ^= F2(_workingKey[3], _tm[i2 + 4], _tr[i2 + 4]);
                _workingKey[1] ^= F3(_workingKey[2], _tm[i2 + 5], _tr[i2 + 5]);
                _workingKey[0] ^= F1(_workingKey[1], _tm[i2 + 6], _tr[i2 + 6]);
                _workingKey[7] ^= F2(_workingKey[0], _tm[i2 + 7], _tr[i2 + 7]);
                // KAPPA <- W2i+1(KAPPA)
                i2 = (i * 2 + 1) * 8;
                _workingKey[6] ^= F1(_workingKey[7], _tm[i2], _tr[i2]);
                _workingKey[5] ^= F2(_workingKey[6], _tm[i2 + 1], _tr[i2 + 1]);
                _workingKey[4] ^= F3(_workingKey[5], _tm[i2 + 2], _tr[i2 + 2]);
                _workingKey[3] ^= F1(_workingKey[4], _tm[i2 + 3], _tr[i2 + 3]);
                _workingKey[2] ^= F2(_workingKey[3], _tm[i2 + 4], _tr[i2 + 4]);
                _workingKey[1] ^= F3(_workingKey[2], _tm[i2 + 5], _tr[i2 + 5]);
                _workingKey[0] ^= F1(_workingKey[1], _tm[i2 + 6], _tr[i2 + 6]);
                _workingKey[7] ^= F2(_workingKey[0], _tm[i2 + 7], _tr[i2 + 7]);
                // Kr_(i) <- KAPPA
                _kr[i * 4] = (int)(_workingKey[0] & 0x1f);
                _kr[i * 4 + 1] = (int)(_workingKey[2] & 0x1f);
                _kr[i * 4 + 2] = (int)(_workingKey[4] & 0x1f);
                _kr[i * 4 + 3] = (int)(_workingKey[6] & 0x1f);
                // Km_(i) <- KAPPA
                _km[i * 4] = _workingKey[7];
                _km[i * 4 + 1] = _workingKey[5];
                _km[i * 4 + 2] = _workingKey[3];
                _km[i * 4 + 3] = _workingKey[1];
            }
        }

        /**
        * Encrypt the given input starting at the given offset and place
        * the result in the provided buffer starting at the given offset.
        *
        * @param src        The plaintext buffer
        * @param srcIndex    An offset into src
        * @param dst        The ciphertext buffer
        * @param dstIndex    An offset into dst
        */
        internal override int EncryptBlock(
            byte[] src,
            int srcIndex,
            byte[] dst,
            int dstIndex)
        {
            // process the input block
            // batch the units up into 4x32 bit chunks and go for it
            uint a = Pack.BE_To_UInt32(src, srcIndex);
            uint b = Pack.BE_To_UInt32(src, srcIndex + 4);
            uint c = Pack.BE_To_UInt32(src, srcIndex + 8);
            uint d = Pack.BE_To_UInt32(src, srcIndex + 12);
            uint[] result = new uint[4];
            CAST_Encipher(a, b, c, d, result);
            // now stuff them into the destination block
            Pack.UInt32_To_BE(result[0], dst, dstIndex);
            Pack.UInt32_To_BE(result[1], dst, dstIndex + 4);
            Pack.UInt32_To_BE(result[2], dst, dstIndex + 8);
            Pack.UInt32_To_BE(result[3], dst, dstIndex + 12);
            return BlockSize;
        }

        /**
        * Decrypt the given input starting at the given offset and place
        * the result in the provided buffer starting at the given offset.
        *
        * @param src        The plaintext buffer
        * @param srcIndex    An offset into src
        * @param dst        The ciphertext buffer
        * @param dstIndex    An offset into dst
        */
        internal override int DecryptBlock(
            byte[] src,
            int srcIndex,
            byte[] dst,
            int dstIndex)
        {
            // process the input block
            // batch the units up into 4x32 bit chunks and go for it
            uint a = Pack.BE_To_UInt32(src, srcIndex);
            uint b = Pack.BE_To_UInt32(src, srcIndex + 4);
            uint c = Pack.BE_To_UInt32(src, srcIndex + 8);
            uint d = Pack.BE_To_UInt32(src, srcIndex + 12);
            uint[] result = new uint[4];
            CAST_Decipher(a, b, c, d, result);
            // now stuff them into the destination block
            Pack.UInt32_To_BE(result[0], dst, dstIndex);
            Pack.UInt32_To_BE(result[1], dst, dstIndex + 4);
            Pack.UInt32_To_BE(result[2], dst, dstIndex + 8);
            Pack.UInt32_To_BE(result[3], dst, dstIndex + 12);
            return BlockSize;
        }

        /**
        * Does the 12 quad rounds rounds to encrypt the block.
        *
        * @param A    the 00-31  bits of the plaintext block
        * @param B    the 32-63  bits of the plaintext block
        * @param C    the 64-95  bits of the plaintext block
        * @param D    the 96-127 bits of the plaintext block
        * @param result the resulting ciphertext
        */
        private void CAST_Encipher(
            uint a,
            uint b,
            uint c,
            uint d,
            uint[] result)
        {
            for (int i = 0; i < 6; i++)
            {
                int x = i * 4;
                // BETA <- Qi(BETA)
                c ^= F1(d, _km[x], _kr[x]);
                b ^= F2(c, _km[x + 1], _kr[x + 1]);
                a ^= F3(b, _km[x + 2], _kr[x + 2]);
                d ^= F1(a, _km[x + 3], _kr[x + 3]);
            }

            for (int i = 6; i < 12; i++)
            {
                int x = i * 4;
                // BETA <- QBARi(BETA)
                d ^= F1(a, _km[x + 3], _kr[x + 3]);
                a ^= F3(b, _km[x + 2], _kr[x + 2]);
                b ^= F2(c, _km[x + 1], _kr[x + 1]);
                c ^= F1(d, _km[x], _kr[x]);
            }

            result[0] = a;
            result[1] = b;
            result[2] = c;
            result[3] = d;
        }

        /**
        * Does the 12 quad rounds rounds to decrypt the block.
        *
        * @param A    the 00-31  bits of the ciphertext block
        * @param B    the 32-63  bits of the ciphertext block
        * @param C    the 64-95  bits of the ciphertext block
        * @param D    the 96-127 bits of the ciphertext block
        * @param result the resulting plaintext
        */
        private void CAST_Decipher(
            uint a,
            uint b,
            uint c,
            uint d,
            uint[] result)
        {
            for (int i = 0; i < 6; i++)
            {
                int x = (11 - i) * 4;
                // BETA <- Qi(BETA)
                c ^= F1(d, _km[x], _kr[x]);
                b ^= F2(c, _km[x + 1], _kr[x + 1]);
                a ^= F3(b, _km[x + 2], _kr[x + 2]);
                d ^= F1(a, _km[x + 3], _kr[x + 3]);
            }

            for (int i = 6; i < 12; i++)
            {
                int x = (11 - i) * 4;
                // BETA <- QBARi(BETA)
                d ^= F1(a, _km[x + 3], _kr[x + 3]);
                a ^= F3(b, _km[x + 2], _kr[x + 2]);
                b ^= F2(c, _km[x + 1], _kr[x + 1]);
                c ^= F1(d, _km[x], _kr[x]);
            }

            result[0] = a;
            result[1] = b;
            result[2] = c;
            result[3] = d;
        }
    }
}
#pragma warning restore
#endif
#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
     * Tnepres is a 128-bit 32-round block cipher with variable key lengths,
     * including 128, 192 and 256 bit keys conjectured to be at least as
     * secure as three-key triple-DES.
     * <p>
     * Tnepres is based on Serpent which was designed by Ross Anderson, Eli Biham and Lars Knudsen as a
     * candidate algorithm for the NIST AES Quest. Unfortunately there was an endianness issue
     * with test vectors in the AES submission and the resulting confusion lead to the Tnepres cipher
     * as well, which is a byte swapped version of Serpent.
     * </p>
     * <p>
     * For full details see <a href="http://www.cl.cam.ac.uk/~rja14/serpent.html">The Serpent home page</a>
     * </p>
     */
    public sealed class TnepresEngine
        : SerpentEngineBase
    {
        public override string AlgorithmName
        {
            get { return "Tnepres"; }
        }

        /**
        * Expand a user-supplied key material into a session key.
        *
        * @param key  The user-key bytes (multiples of 4) to use.
        * @exception ArgumentException
        */
        protected override int[] MakeWorkingKey(byte[] key)
        {
            //
            // pad key to 256 bits
            //
            int[] kPad = new int[16];
            int off = 0;
            int length = 0;

            for (off = key.Length - 4; off > 0; off -= 4)
            {
                kPad[length++] = (int)Pack.BE_To_UInt32(key, off);
            }

            if (off == 0)
            {
                kPad[length++] = (int)Pack.BE_To_UInt32(key, 0);
                if (length < 8)
                {
                    kPad[length] = 1;
                }
            }
            else
            {
                throw new ArgumentException("key must be a multiple of 4 bytes");
            }

            //
            // expand the padded key up to 33 x 128 bits of key material
            //
            int amount = (Rounds + 1) * 4;
            int[] w = new int[amount];

            //
            // compute w0 to w7 from w-8 to w-1
            //
            for (int i = 8; i < 16; i++)
            {
                kPad[i] = RotateLeft(kPad[i - 8] ^ kPad[i - 5] ^ kPad[i - 3] ^ kPad[i - 1] ^ Phi ^ (i - 8), 11);
            }

            Array.Copy(kPad, 8, w, 0, 8);

            //
            // compute w8 to w136
            //
            for (int i = 8; i < amount; i++)
            {
                w[i] = RotateLeft(w[i - 8] ^ w[i - 5] ^ w[i - 3] ^ w[i - 1] ^ Phi ^ i, 11);
            }

            //
            // create the working keys by processing w with the Sbox and IP
            //
            Sb3(w[0], w[1], w[2], w[3]);
            w[0] = X0;
            w[1] = X1;
            w[2] = X2;
            w[3] = X3;
            Sb2(w[4], w[5], w[6], w[7]);
            w[4] = X0;
            w[5] = X1;
            w[6] = X2;
            w[7] = X3;
            Sb1(w[8], w[9], w[10], w[11]);
            w[8] = X0;
            w[9] = X1;
            w[10] = X2;
            w[11] = X3;
            Sb0(w[12], w[13], w[14], w[15]);
            w[12] = X0;
            w[13] = X1;
            w[14] = X2;
            w[15] = X3;
            Sb7(w[16], w[17], w[18], w[19]);
            w[16] = X0;
            w[17] = X1;
            w[18] = X2;
            w[19] = X3;
            Sb6(w[20], w[21], w[22], w[23]);
            w[20] = X0;
            w[21] = X1;
            w[22] = X2;
            w[23] = X3;
            Sb5(w[24], w[25], w[26], w[27]);
            w[24] = X0;
            w[25] = X1;
            w[26] = X2;
            w[27] = X3;
            Sb4(w[28], w[29], w[30], w[31]);
            w[28] = X0;
            w[29] = X1;
            w[30] = X2;
            w[31] = X3;
            Sb3(w[32], w[33], w[34], w[35]);
            w[32] = X0;
            w[33] = X1;
            w[34] = X2;
            w[35] = X3;
            Sb2(w[36], w[37], w[38], w[39]);
            w[36] = X0;
            w[37] = X1;
            w[38] = X2;
            w[39] = X3;
            Sb1(w[40], w[41], w[42], w[43]);
            w[40] = X0;
            w[41] = X1;
            w[42] = X2;
            w[43] = X3;
            Sb0(w[44], w[45], w[46], w[47]);
            w[44] = X0;
            w[45] = X1;
            w[46] = X2;
            w[47] = X3;
            Sb7(w[48], w[49], w[50], w[51]);
            w[48] = X0;
            w[49] = X1;
            w[50] = X2;
            w[51] = X3;
            Sb6(w[52], w[53], w[54], w[55]);
            w[52] = X0;
            w[53] = X1;
            w[54] = X2;
            w[55] = X3;
            Sb5(w[56], w[57], w[58], w[59]);
            w[56] = X0;
            w[57] = X1;
            w[58] = X2;
            w[59] = X3;
            Sb4(w[60], w[61], w[62], w[63]);
            w[60] = X0;
            w[61] = X1;
            w[62] = X2;
            w[63] = X3;
            Sb3(w[64], w[65], w[66], w[67]);
            w[64] = X0;
            w[65] = X1;
            w[66] = X2;
            w[67] = X3;
            Sb2(w[68], w[69], w[70], w[71]);
            w[68] = X0;
            w[69] = X1;
            w[70] = X2;
            w[71] = X3;
            Sb1(w[72], w[73], w[74], w[75]);
            w[72] = X0;
            w[73] = X1;
            w[74] = X2;
            w[75] = X3;
            Sb0(w[76], w[77], w[78], w[79]);
            w[76] = X0;
            w[77] = X1;
            w[78] = X2;
            w[79] = X3;
            Sb7(w[80], w[81], w[82], w[83]);
            w[80] = X0;
            w[81] = X1;
            w[82] = X2;
            w[83] = X3;
            Sb6(w[84], w[85], w[86], w[87]);
            w[84] = X0;
            w[85] = X1;
            w[86] = X2;
            w[87] = X3;
            Sb5(w[88], w[89], w[90], w[91]);
            w[88] = X0;
            w[89] = X1;
            w[90] = X2;
            w[91] = X3;
            Sb4(w[92], w[93], w[94], w[95]);
            w[92] = X0;
            w[93] = X1;
            w[94] = X2;
            w[95] = X3;
            Sb3(w[96], w[97], w[98], w[99]);
            w[96] = X0;
            w[97] = X1;
            w[98] = X2;
            w[99] = X3;
            Sb2(w[100], w[101], w[102], w[103]);
            w[100] = X0;
            w[101] = X1;
            w[102] = X2;
            w[103] = X3;
            Sb1(w[104], w[105], w[106], w[107]);
            w[104] = X0;
            w[105] = X1;
            w[106] = X2;
            w[107] = X3;
            Sb0(w[108], w[109], w[110], w[111]);
            w[108] = X0;
            w[109] = X1;
            w[110] = X2;
            w[111] = X3;
            Sb7(w[112], w[113], w[114], w[115]);
            w[112] = X0;
            w[113] = X1;
            w[114] = X2;
            w[115] = X3;
            Sb6(w[116], w[117], w[118], w[119]);
            w[116] = X0;
            w[117] = X1;
            w[118] = X2;
            w[119] = X3;
            Sb5(w[120], w[121], w[122], w[123]);
            w[120] = X0;
            w[121] = X1;
            w[122] = X2;
            w[123] = X3;
            Sb4(w[124], w[125], w[126], w[127]);
            w[124] = X0;
            w[125] = X1;
            w[126] = X2;
            w[127] = X3;
            Sb3(w[128], w[129], w[130], w[131]);
            w[128] = X0;
            w[129] = X1;
            w[130] = X2;
            w[131] = X3;

            return w;
        }

        /**
        * Encrypt one block of plaintext.
        *
        * @param input the array containing the input data.
        * @param inOff offset into the in array the data starts at.
        * @param output the array the output data will be copied into.
        * @param outOff the offset into the out array the output will start at.
        */
        protected override void EncryptBlock(byte[] input, int inOff, byte[] output, int outOff)
        {
            X3 = (int)Pack.BE_To_UInt32(input, inOff);
            X2 = (int)Pack.BE_To_UInt32(input, inOff + 4);
            X1 = (int)Pack.BE_To_UInt32(input, inOff + 8);
            X0 = (int)Pack.BE_To_UInt32(input, inOff + 12);

            Sb0(WKey[0] ^ X0, WKey[1] ^ X1, WKey[2] ^ X2, WKey[3] ^ X3);
            LT();
            Sb1(WKey[4] ^ X0, WKey[5] ^ X1, WKey[6] ^ X2, WKey[7] ^ X3);
            LT();
            Sb2(WKey[8] ^ X0, WKey[9] ^ X1, WKey[10] ^ X2, WKey[11] ^ X3);
            LT();
            Sb3(WKey[12] ^ X0, WKey[13] ^ X1, WKey[14] ^ X2, WKey[15] ^ X3);
            LT();
            Sb4(WKey[16] ^ X0, WKey[17] ^ X1, WKey[18] ^ X2, WKey[19] ^ X3);
            LT();
            Sb5(WKey[20] ^ X0, WKey[21] ^ X1, WKey[22] ^ X2, WKey[23] ^ X3);
            LT();
            Sb6(WKey[24] ^ X0, WKey[25] ^ X1, WKey[26] ^ X2, WKey[27] ^ X3);
            LT();
            Sb7(WKey[28] ^ X0, WKey[29] ^ X1, WKey[30] ^ X2, WKey[31] ^ X3);
            LT();
            Sb0(WKey[32] ^ X0, WKey[33] ^ X1, WKey[34] ^ X2, WKey[35] ^ X3);
            LT();
            Sb1(WKey[36] ^ X0, WKey[37] ^ X1, WKey[38] ^ X2, WKey[39] ^ X3);
            LT();
            Sb2(WKey[40] ^ X0, WKey[41] ^ X1, WKey[42] ^ X2, WKey[43] ^ X3);
            LT();
            Sb3(WKey[44] ^ X0, WKey[45] ^ X1, WKey[46] ^ X2, WKey[47] ^ X3);
            LT();
            Sb4(WKey[48] ^ X0, WKey[49] ^ X1, WKey[50] ^ X2, WKey[51] ^ X3);
            LT();
            Sb5(WKey[52] ^ X0, WKey[53] ^ X1, WKey[54] ^ X2, WKey[55] ^ X3);
            LT();
            Sb6(WKey[56] ^ X0, WKey[57] ^ X1, WKey[58] ^ X2, WKey[59] ^ X3);
            LT();
            Sb7(WKey[60] ^ X0, WKey[61] ^ X1, WKey[62] ^ X2, WKey[63] ^ X3);
            LT();
            Sb0(WKey[64] ^ X0, WKey[65] ^ X1, WKey[66] ^ X2, WKey[67] ^ X3);
            LT();
            Sb1(WKey[68] ^ X0, WKey[69] ^ X1, WKey[70] ^ X2, WKey[71] ^ X3);
            LT();
            Sb2(WKey[72] ^ X0, WKey[73] ^ X1, WKey[74] ^ X2, WKey[75] ^ X3);
            LT();
            Sb3(WKey[76] ^ X0, WKey[77] ^ X1, WKey[78] ^ X2, WKey[79] ^ X3);
            LT();
            Sb4(WKey[80] ^ X0, WKey[81] ^ X1, WKey[82] ^ X2, WKey[83] ^ X3);
            LT();
            Sb5(WKey[84] ^ X0, WKey[85] ^ X1, WKey[86] ^ X2, WKey[87] ^ X3);
            LT();
            Sb6(WKey[88] ^ X0, WKey[89] ^ X1, WKey[90] ^ X2, WKey[91] ^ X3);
            LT();
            Sb7(WKey[92] ^ X0, WKey[93] ^ X1, WKey[94] ^ X2, WKey[95] ^ X3);
            LT();
            Sb0(WKey[96] ^ X0, WKey[97] ^ X1, WKey[98] ^ X2, WKey[99] ^ X3);
            LT();
            Sb1(WKey[100] ^ X0, WKey[101] ^ X1, WKey[102] ^ X2, WKey[103] ^ X3);
            LT();
            Sb2(WKey[104] ^ X0, WKey[105] ^ X1, WKey[106] ^ X2, WKey[107] ^ X3);
            LT();
            Sb3(WKey[108] ^ X0, WKey[109] ^ X1, WKey[110] ^ X2, WKey[111] ^ X3);
            LT();
            Sb4(WKey[112] ^ X0, WKey[113] ^ X1, WKey[114] ^ X2, WKey[115] ^ X3);
            LT();
            Sb5(WKey[116] ^ X0, WKey[117] ^ X1, WKey[118] ^ X2, WKey[119] ^ X3);
            LT();
            Sb6(WKey[120] ^ X0, WKey[121] ^ X1, WKey[122] ^ X2, WKey[123] ^ X3);
            LT();
            Sb7(WKey[124] ^ X0, WKey[125] ^ X1, WKey[126] ^ X2, WKey[127] ^ X3);

            Pack.UInt32_To_BE((uint)(WKey[131] ^ X3), output, outOff);
            Pack.UInt32_To_BE((uint)(WKey[130] ^ X2), output, outOff + 4);
            Pack.UInt32_To_BE((uint)(WKey[129] ^ X1), output, outOff + 8);
            Pack.UInt32_To_BE((uint)(WKey[128] ^ X0), output, outOff + 12);
        }

        /**
        * Decrypt one block of ciphertext.
        *
        * @param input the array containing the input data.
        * @param inOff offset into the in array the data starts at.
        * @param output the array the output data will be copied into.
        * @param outOff the offset into the out array the output will start at.
        */
        protected override void DecryptBlock(byte[] input, int inOff, byte[] output, int outOff)
        {
            X3 = WKey[131] ^ (int)Pack.BE_To_UInt32(input, inOff);
            X2 = WKey[130] ^ (int)Pack.BE_To_UInt32(input, inOff + 4);
            X1 = WKey[129] ^ (int)Pack.BE_To_UInt32(input, inOff + 8);
            X0 = WKey[128] ^ (int)Pack.BE_To_UInt32(input, inOff + 12);

            Ib7(X0, X1, X2, X3);
            X0 ^= WKey[124];
            X1 ^= WKey[125];
            X2 ^= WKey[126];
            X3 ^= WKey[127];
            InverseLT();
            Ib6(X0, X1, X2, X3);
            X0 ^= WKey[120];
            X1 ^= WKey[121];
            X2 ^= WKey[122];
            X3 ^= WKey[123];
            InverseLT();
            Ib5(X0, X1, X2, X3);
            X0 ^= WKey[116];
            X1 ^= WKey[117];
            X2 ^= WKey[118];
            X3 ^= WKey[119];
            InverseLT();
            Ib4(X0, X1, X2, X3);
            X0 ^= WKey[112];
            X1 ^= WKey[113];
            X2 ^= WKey[114];
            X3 ^= WKey[115];
            InverseLT();
            Ib3(X0, X1, X2, X3);
            X0 ^= WKey[108];
            X1 ^= WKey[109];
            X2 ^= WKey[110];
            X3 ^= WKey[111];
            InverseLT();
            Ib2(X0, X1, X2, X3);
            X0 ^= WKey[104];
            X1 ^= WKey[105];
            X2 ^= WKey[106];
            X3 ^= WKey[107];
            InverseLT();
            Ib1(X0, X1, X2, X3);
            X0 ^= WKey[100];
            X1 ^= WKey[101];
            X2 ^= WKey[102];
            X3 ^= WKey[103];
            InverseLT();
            Ib0(X0, X1, X2, X3);
            X0 ^= WKey[96];
            X1 ^= WKey[97];
            X2 ^= WKey[98];
            X3 ^= WKey[99];
            InverseLT();
            Ib7(X0, X1, X2, X3);
            X0 ^= WKey[92];
            X1 ^= WKey[93];
            X2 ^= WKey[94];
            X3 ^= WKey[95];
            InverseLT();
            Ib6(X0, X1, X2, X3);
            X0 ^= WKey[88];
            X1 ^= WKey[89];
            X2 ^= WKey[90];
            X3 ^= WKey[91];
            InverseLT();
            Ib5(X0, X1, X2, X3);
            X0 ^= WKey[84];
            X1 ^= WKey[85];
            X2 ^= WKey[86];
            X3 ^= WKey[87];
            InverseLT();
            Ib4(X0, X1, X2, X3);
            X0 ^= WKey[80];
            X1 ^= WKey[81];
            X2 ^= WKey[82];
            X3 ^= WKey[83];
            InverseLT();
            Ib3(X0, X1, X2, X3);
            X0 ^= WKey[76];
            X1 ^= WKey[77];
            X2 ^= WKey[78];
            X3 ^= WKey[79];
            InverseLT();
            Ib2(X0, X1, X2, X3);
            X0 ^= WKey[72];
            X1 ^= WKey[73];
            X2 ^= WKey[74];
            X3 ^= WKey[75];
            InverseLT();
            Ib1(X0, X1, X2, X3);
            X0 ^= WKey[68];
            X1 ^= WKey[69];
            X2 ^= WKey[70];
            X3 ^= WKey[71];
            InverseLT();
            Ib0(X0, X1, X2, X3);
            X0 ^= WKey[64];
            X1 ^= WKey[65];
            X2 ^= WKey[66];
            X3 ^= WKey[67];
            InverseLT();
            Ib7(X0, X1, X2, X3);
            X0 ^= WKey[60];
            X1 ^= WKey[61];
            X2 ^= WKey[62];
            X3 ^= WKey[63];
            InverseLT();
            Ib6(X0, X1, X2, X3);
            X0 ^= WKey[56];
            X1 ^= WKey[57];
            X2 ^= WKey[58];
            X3 ^= WKey[59];
            InverseLT();
            Ib5(X0, X1, X2, X3);
            X0 ^= WKey[52];
            X1 ^= WKey[53];
            X2 ^= WKey[54];
            X3 ^= WKey[55];
            InverseLT();
            Ib4(X0, X1, X2, X3);
            X0 ^= WKey[48];
            X1 ^= WKey[49];
            X2 ^= WKey[50];
            X3 ^= WKey[51];
            InverseLT();
            Ib3(X0, X1, X2, X3);
            X0 ^= WKey[44];
            X1 ^= WKey[45];
            X2 ^= WKey[46];
            X3 ^= WKey[47];
            InverseLT();
            Ib2(X0, X1, X2, X3);
            X0 ^= WKey[40];
            X1 ^= WKey[41];
            X2 ^= WKey[42];
            X3 ^= WKey[43];
            InverseLT();
            Ib1(X0, X1, X2, X3);
            X0 ^= WKey[36];
            X1 ^= WKey[37];
            X2 ^= WKey[38];
            X3 ^= WKey[39];
            InverseLT();
            Ib0(X0, X1, X2, X3);
            X0 ^= WKey[32];
            X1 ^= WKey[33];
            X2 ^= WKey[34];
            X3 ^= WKey[35];
            InverseLT();
            Ib7(X0, X1, X2, X3);
            X0 ^= WKey[28];
            X1 ^= WKey[29];
            X2 ^= WKey[30];
            X3 ^= WKey[31];
            InverseLT();
            Ib6(X0, X1, X2, X3);
            X0 ^= WKey[24];
            X1 ^= WKey[25];
            X2 ^= WKey[26];
            X3 ^= WKey[27];
            InverseLT();
            Ib5(X0, X1, X2, X3);
            X0 ^= WKey[20];
            X1 ^= WKey[21];
            X2 ^= WKey[22];
            X3 ^= WKey[23];
            InverseLT();
            Ib4(X0, X1, X2, X3);
            X0 ^= WKey[16];
            X1 ^= WKey[17];
            X2 ^= WKey[18];
            X3 ^= WKey[19];
            InverseLT();
            Ib3(X0, X1, X2, X3);
            X0 ^= WKey[12];
            X1 ^= WKey[13];
            X2 ^= WKey[14];
            X3 ^= WKey[15];
            InverseLT();
            Ib2(X0, X1, X2, X3);
            X0 ^= WKey[8];
            X1 ^= WKey[9];
            X2 ^= WKey[10];
            X3 ^= WKey[11];
            InverseLT();
            Ib1(X0, X1, X2, X3);
            X0 ^= WKey[4];
            X1 ^= WKey[5];
            X2 ^= WKey[6];
            X3 ^= WKey[7];
            InverseLT();
            Ib0(X0, X1, X2, X3);

            Pack.UInt32_To_BE((uint)(X3 ^ WKey[3]), output, outOff);
            Pack.UInt32_To_BE((uint)(X2 ^ WKey[2]), output, outOff + 4);
            Pack.UInt32_To_BE((uint)(X1 ^ WKey[1]), output, outOff + 8);
            Pack.UInt32_To_BE((uint)(X0 ^ WKey[0]), output, outOff + 12);
        }
    }
}
#pragma warning restore
#endif
#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
    * A class that provides Twofish encryption operations.
    *
    * This Java implementation is based on the Java reference
    * implementation provided by Bruce Schneier and developed
    * by Raif S. Naffah.
    */
    public sealed class TwofishEngine
        : IBlockCipher
    {
        /**
        * Define the fixed p0/p1 permutations used in keyed S-box lookup.
        * By changing the following constant definitions, the S-boxes will
        * automatically Get changed in the Twofish engine.
        */
        private const int P00 = 1;

        private const int P01 = 0;
        private const int P02 = 0;
        private const int P03 = P01 ^ 1;
        private const int P04 = 1;

        private const int P10 = 0;
        private const int P11 = 0;
        private const int P12 = 1;
        private const int P13 = P11 ^ 1;
        private const int P14 = 0;

        private const int P20 = 1;
        private const int P21 = 1;
        private const int P22 = 0;
        private const int P23 = P21 ^ 1;
        private const int P24 = 0;

        private const int P30 = 0;
        private const int P31 = 1;
        private const int P32 = 1;
        private const int P33 = P31 ^ 1;
        private const int P34 = 1;

        /* Primitive polynomial for GF(256) */
        private const int Gf256Fdbk = 0x169;
        private const int Gf256Fdbk2 = Gf256Fdbk / 2;
        private const int Gf256Fdbk4 = Gf256Fdbk / 4;

        private const int RsGfFdbk = 0x14D; // field generator

        //====================================
        // Useful constants
        //====================================

        private const int Rounds = 16;
        private const int MaxRounds = 16; // bytes = 128 bits
        private const int BlockSize = 16; // bytes = 128 bits
        private const int MaxKeyBits = 256;

        private const int InputWhiten = 0;
        private const int OutputWhiten = InputWhiten + BlockSize / 4; // 4
        private const int RoundSubkeys = OutputWhiten + BlockSize / 4; // 8

        private const int TotalSubkeys = RoundSubkeys + 2 * MaxRounds; // 40

        private const int SkStep = 0x02020202;
        private const int SkBump = 0x01010101;
        private const int SkRotl = 9;

        private static readonly byte[,] P =
        {
            {
                // p0
                (byte)0xA9, (byte)0x67, (byte)0xB3, (byte)0xE8,
                (byte)0x04, (byte)0xFD, (byte)0xA3, (byte)0x76,
                (byte)0x9A, (byte)0x92, (byte)0x80, (byte)0x78,
                (byte)0xE4, (byte)0xDD, (byte)0xD1, (byte)0x38,
                (byte)0x0D, (byte)0xC6, (byte)0x35, (byte)0x98,
                (byte)0x18, (byte)0xF7, (byte)0xEC, (byte)0x6C,
                (byte)0x43, (byte)0x75, (byte)0x37, (byte)0x26,
                (byte)0xFA, (byte)0x13, (byte)0x94, (byte)0x48,
                (byte)0xF2, (byte)0xD0, (byte)0x8B, (byte)0x30,
                (byte)0x84, (byte)0x54, (byte)0xDF, (byte)0x23,
                (byte)0x19, (byte)0x5B, (byte)0x3D, (byte)0x59,
                (byte)0xF3, (byte)0xAE, (byte)0xA2, (byte)0x82,
                (byte)0x63, (byte)0x01, (byte)0x83, (byte)0x2E,
                (byte)0xD9, (byte)0x51, (byte)0x9B, (byte)0x7C,
                (byte)0xA6, (byte)0xEB, (byte)0xA5, (byte)0xBE,
                (byte)0x16, (byte)0x0C, (byte)0xE3, (byte)0x61,
                (byte)0xC0, (byte)0x8C, (byte)0x3A, (byte)0xF5,
                (byte)0x73, (byte)0x2C, (byte)0x25, (byte)0x0B,
                (byte)0xBB, (byte)0x4E, (byte)0x89, (byte)0x6B,
                (byte)0x53, (byte)0x6A, (byte)0xB4, (byte)0xF1,
                (byte)0xE1, (byte)0xE6, (byte)0xBD, (byte)0x45,
                (byte)0xE2, (byte)0xF4, (byte)0xB6, (byte)0x66,
                (byte)0xCC, (byte)0x95, (byte)0x03, (byte)0x56,
                (byte)0xD4, (byte)0x1C, (byte)0x1E, (byte)0xD7,
                (byte)0xFB, (byte)0xC3, (byte)0x8E, (byte)0xB5,
                (byte)0xE9, (byte)0xCF, (byte)0xBF, (byte)0xBA,
                (byte)0xEA, (byte)0x77, (byte)0x39, (byte)0xAF,
                (byte)0x33, (byte)0xC9, (byte)0x62, (byte)0x71,
                (byte)0x81, (byte)0x79, (byte)0x09, (byte)0xAD,
                (byte)0x24, (byte)0xCD, (byte)0xF9, (byte)0xD8,
                (byte)0xE5, (byte)0xC5, (byte)0xB9, (byte)0x4D,
                (byte)0x44, (byte)0x08, (byte)0x86, (byte)0xE7,
                (byte)0xA1, (byte)0x1D, (byte)0xAA, (byte)0xED,
                (byte)0x06, (byte)0x70, (byte)0xB2, (byte)0xD2,
                (byte)0x41, (byte)0x7B, (byte)0xA0, (byte)0x11,
                (byte)0x31, (byte)0xC2, (byte)0x27, (byte)0x90,
                (byte)0x20, (byte)0xF6, (byte)0x60, (byte)0xFF,
                (byte)0x96, (byte)0x5C, (byte)0xB1, (byte)0xAB,
                (byte)0x9E, (byte)0x9C, (byte)0x52, (byte)0x1B,
                (byte)0x5F, (byte)0x93, (byte)0x0A, (byte)0xEF,
                (byte)0x91, (byte)0x85, (byte)0x49, (byte)0xEE,
                (byte)0x2D, (byte)0x4F, (byte)0x8F, (byte)0x3B,
                (byte)0x47, (byte)0x87, (byte)0x6D, (byte)0x46,
                (byte)0xD6, (byte)0x3E, (byte)0x69, (byte)0x64,
                (byte)0x2A, (byte)0xCE, (byte)0xCB, (byte)0x2F,
                (byte)0xFC, (byte)0x97, (byte)0x05, (byte)0x7A,
                (byte)0xAC, (byte)0x7F, (byte)0xD5, (byte)0x1A,
                (byte)0x4B, (byte)0x0E, (byte)0xA7, (byte)0x5A,
                (byte)0x28, (byte)0x14, (byte)0x3F, (byte)0x29,
                (byte)0x88, (byte)0x3C, (byte)0x4C, (byte)0x02,
                (byte)0xB8, (byte)0xDA, (byte)0xB0, (byte)0x17,
                (byte)0x55, (byte)0x1F, (byte)0x8A, (byte)0x7D,
                (byte)0x57, (byte)0xC7, (byte)0x8D, (byte)0x74,
                (byte)0xB7, (byte)0xC4, (byte)0x9F, (byte)0x72,
                (byte)0x7E, (byte)0x15, (byte)0x22, (byte)0x12,
                (byte)0x58, (byte)0x07, (byte)0x99, (byte)0x34,
                (byte)0x6E, (byte)0x50, (byte)0xDE, (byte)0x68,
                (byte)0x65, (byte)0xBC, (byte)0xDB, (byte)0xF8,
                (byte)0xC8, (byte)0xA8, (byte)0x2B, (byte)0x40,
                (byte)0xDC, (byte)0xFE, (byte)0x32, (byte)0xA4,
                (byte)0xCA, (byte)0x10, (byte)0x21, (byte)0xF0,
                (byte)0xD3, (byte)0x5D, (byte)0x0F, (byte)0x00,
                (byte)0x6F, (byte)0x9D, (byte)0x36, (byte)0x42,
                (byte)0x4A, (byte)0x5E, (byte)0xC1, (byte)0xE0
            },
            {
                // p1
                (byte)0x75, (byte)0xF3, (byte)0xC6, (byte)0xF4,
                (byte)0xDB, (byte)0x7B, (byte)0xFB, (byte)0xC8,
                (byte)0x4A, (byte)0xD3, (byte)0xE6, (byte)0x6B,
                (byte)0x45, (byte)0x7D, (byte)0xE8, (byte)0x4B,
                (byte)0xD6, (byte)0x32, (byte)0xD8, (byte)0xFD,
                (byte)0x37, (byte)0x71, (byte)0xF1, (byte)0xE1,
                (byte)0x30, (byte)0x0F, (byte)0xF8, (byte)0x1B,
                (byte)0x87, (byte)0xFA, (byte)0x06, (byte)0x3F,
                (byte)0x5E, (byte)0xBA, (byte)0xAE, (byte)0x5B,
                (byte)0x8A, (byte)0x00, (byte)0xBC, (byte)0x9D,
                (byte)0x6D, (byte)0xC1, (byte)0xB1, (byte)0x0E,
                (byte)0x80, (byte)0x5D, (byte)0xD2, (byte)0xD5,
                (byte)0xA0, (byte)0x84, (byte)0x07, (byte)0x14,
                (byte)0xB5, (byte)0x90, (byte)0x2C, (byte)0xA3,
                (byte)0xB2, (byte)0x73, (byte)0x4C, (byte)0x54,
                (byte)0x92, (byte)0x74, (byte)0x36, (byte)0x51,
                (byte)0x38, (byte)0xB0, (byte)0xBD, (byte)0x5A,
                (byte)0xFC, (byte)0x60, (byte)0x62, (byte)0x96,
                (byte)0x6C, (byte)0x42, (byte)0xF7, (byte)0x10,
                (byte)0x7C, (byte)0x28, (byte)0x27, (byte)0x8C,
                (byte)0x13, (byte)0x95, (byte)0x9C, (byte)0xC7,
                (byte)0x24, (byte)0x46, (byte)0x3B, (byte)0x70,
                (byte)0xCA, (byte)0xE3, (byte)0x85, (byte)0xCB,
                (byte)0x11, (byte)0xD0, (byte)0x93, (byte)0xB8,
                (byte)0xA6, (byte)0x83, (byte)0x20, (byte)0xFF,
                (byte)0x9F, (byte)0x77, (byte)0xC3, (byte)0xCC,
                (byte)0x03, (byte)0x6F, (byte)0x08, (byte)0xBF,
                (byte)0x40, (byte)0xE7, (byte)0x2B, (byte)0xE2,
                (byte)0x79, (byte)0x0C, (byte)0xAA, (byte)0x82,
                (byte)0x41, (byte)0x3A, (byte)0xEA, (byte)0xB9,
                (byte)0xE4, (byte)0x9A, (byte)0xA4, (byte)0x97,
                (byte)0x7E, (byte)0xDA, (byte)0x7A, (byte)0x17,
                (byte)0x66, (byte)0x94, (byte)0xA1, (byte)0x1D,
                (byte)0x3D, (byte)0xF0, (byte)0xDE, (byte)0xB3,
                (byte)0x0B, (byte)0x72, (byte)0xA7, (byte)0x1C,
                (byte)0xEF, (byte)0xD1, (byte)0x53, (byte)0x3E,
                (byte)0x8F, (byte)0x33, (byte)0x26, (byte)0x5F,
                (byte)0xEC, (byte)0x76, (byte)0x2A, (byte)0x49,
                (byte)0x81, (byte)0x88, (byte)0xEE, (byte)0x21,
                (byte)0xC4, (byte)0x1A, (byte)0xEB, (byte)0xD9,
                (byte)0xC5, (byte)0x39, (byte)0x99, (byte)0xCD,
                (byte)0xAD, (byte)0x31, (byte)0x8B, (byte)0x01,
                (byte)0x18, (byte)0x23, (byte)0xDD, (byte)0x1F,
                (byte)0x4E, (byte)0x2D, (byte)0xF9, (byte)0x48,
                (byte)0x4F, (byte)0xF2, (byte)0x65, (byte)0x8E,
                (byte)0x78, (byte)0x5C, (byte)0x58, (byte)0x19,
                (byte)0x8D, (byte)0xE5, (byte)0x98, (byte)0x57,
                (byte)0x67, (byte)0x7F, (byte)0x05, (byte)0x64,
                (byte)0xAF, (byte)0x63, (byte)0xB6, (byte)0xFE,
                (byte)0xF5, (byte)0xB7, (byte)0x3C, (byte)0xA5,
                (byte)0xCE, (byte)0xE9, (byte)0x68, (byte)0x44,
                (byte)0xE0, (byte)0x4D, (byte)0x43, (byte)0x69,
                (byte)0x29, (byte)0x2E, (byte)0xAC, (byte)0x15,
                (byte)0x59, (byte)0xA8, (byte)0x0A, (byte)0x9E,
                (byte)0x6E, (byte)0x47, (byte)0xDF, (byte)0x34,
                (byte)0x35, (byte)0x6A, (byte)0xCF, (byte)0xDC,
                (byte)0x22, (byte)0xC9, (byte)0xC0, (byte)0x9B,
                (byte)0x89, (byte)0xD4, (byte)0xED, (byte)0xAB,
                (byte)0x12, (byte)0xA2, (byte)0x0D, (byte)0x52,
                (byte)0xBB, (byte)0x02, (byte)0x2F, (byte)0xA9,
                (byte)0xD7, (byte)0x61, (byte)0x1E, (byte)0xB4,
                (byte)0x50, (byte)0x04, (byte)0xF6, (byte)0xC2,
                (byte)0x16, (byte)0x25, (byte)0x86, (byte)0x56,
                (byte)0x55, (byte)0x09, (byte)0xBE, (byte)0x91
            }
        };

        private bool _encrypting;

        private int[] _gMds0 = new int[MaxKeyBits];
        private int[] _gMds1 = new int[MaxKeyBits];
        private int[] _gMds2 = new int[MaxKeyBits];
        private int[] _gMds3 = new int[MaxKeyBits];
        private int[] _gSBox;

        /**
        * gSubKeys[] and gSBox[] are eventually used in the
        * encryption and decryption methods.
        */
        private int[] _gSubKeys;

        private int _k64Cnt;

        private byte[] _workingKey;

        public TwofishEngine()
        {
            // calculate the MDS matrix
            int[] m1 = new int[2];
            int[] mX = new int[2];
            int[] mY = new int[2];
            int j;

            for (int i = 0; i < MaxKeyBits; i++)
            {
                j = P[0, i] & 0xff;
                m1[0] = j;
                mX[0] = Mx_X(j) & 0xff;
                mY[0] = Mx_Y(j) & 0xff;

                j = P[1, i] & 0xff;
                m1[1] = j;
                mX[1] = Mx_X(j) & 0xff;
                mY[1] = Mx_Y(j) & 0xff;

                _gMds0[i] = m1[P00] | mX[P00] << 8 |
                            mY[P00] << 16 | mY[P00] << 24;

                _gMds1[i] = mY[P10] | mY[P10] << 8 |
                            mX[P10] << 16 | m1[P10] << 24;

                _gMds2[i] = mX[P20] | mY[P20] << 8 |
                            m1[P20] << 16 | mY[P20] << 24;

                _gMds3[i] = mX[P30] | m1[P30] << 8 |
                            mY[P30] << 16 | mX[P30] << 24;
            }
        }

        /**
        * initialise a Twofish cipher.
        *
        * @param forEncryption whether or not we are for encryption.
        * @param parameters the parameters required to set up the cipher.
        * @exception ArgumentException if the parameters argument is
        * inappropriate.
        */
        public void Init(
            bool forEncryption,
            ICipherParameters parameters)
        {
            if (!(parameters is KeyParameter))
                throw new ArgumentException("invalid parameter passed to Twofish init - " +
                                            BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(
                                                parameters));

            this._encrypting = forEncryption;
            this._workingKey = ((KeyParameter)parameters).GetKey();

            int keyBits = this._workingKey.Length * 8;
            switch (keyBits)
            {
                case 128:
                case 192:
                case 256:
                    break;
                default:
                    throw new ArgumentException("Key length not 128/192/256 bits.");
            }

            this._k64Cnt = this._workingKey.Length / 8;
            SetKey(this._workingKey);
        }

        public string AlgorithmName
        {
            get { return "Twofish"; }
        }

        public bool IsPartialBlockOkay
        {
            get { return false; }
        }

        public int ProcessBlock(
            byte[] input,
            int inOff,
            byte[] output,
            int outOff)
        {
            if (_workingKey == null)
                throw new InvalidOperationException("Twofish not initialised");

            Check.DataLength(input, inOff, BlockSize, "input buffer too short");
            Check.OutputLength(output, outOff, BlockSize, "output buffer too short");

            if (_encrypting)
            {
                EncryptBlock(input, inOff, output, outOff);
            }
            else
            {
                DecryptBlock(input, inOff, output, outOff);
            }

            return BlockSize;
        }

        public void Reset()
        {
            if (this._workingKey != null)
            {
                SetKey(this._workingKey);
            }
        }

        public int GetBlockSize()
        {
            return BlockSize;
        }

        //==================================
        // Private Implementation
        //==================================

        private void SetKey(byte[] key)
        {
            int[] k32E = new int[MaxKeyBits / 64]; // 4
            int[] k32O = new int[MaxKeyBits / 64]; // 4

            int[] sBoxKeys = new int[MaxKeyBits / 64]; // 4
            _gSubKeys = new int[TotalSubkeys];

            /*
             * k64Cnt is the number of 8 byte blocks (64 chunks) that are in the input key.
             * The input key is 16, 24 or 32 bytes, so the range for k64Cnt is 2..4
            */
            for (int i = 0; i < _k64Cnt; i++)
            {
                int p = i * 8;

                k32E[i] = (int)Pack.LE_To_UInt32(key, p);
                k32O[i] = (int)Pack.LE_To_UInt32(key, p + 4);

                sBoxKeys[_k64Cnt - 1 - i] = RS_MDS_Encode(k32E[i], k32O[i]);
            }

            int q, a, b;
            for (int i = 0; i < TotalSubkeys / 2; i++)
            {
                q = i * SkStep;
                a = F32(q, k32E);
                b = F32(q + SkBump, k32O);
                b = Integers.RotateLeft(b, 8);
                a += b;
                _gSubKeys[i * 2] = a;
                a += b;
                _gSubKeys[i * 2 + 1] = Integers.RotateLeft(a, SkRotl);
            }

            /*
            * fully expand the table for speed
            */
            int k0 = sBoxKeys[0];
            int k1 = sBoxKeys[1];
            int k2 = sBoxKeys[2];
            int k3 = sBoxKeys[3];
            int b0, b1, b2, b3;
            _gSBox = new int[4 * MaxKeyBits];
            for (int i = 0; i < MaxKeyBits; i++)
            {
                b0 = b1 = b2 = b3 = i;
                switch (_k64Cnt & 3)
                {
                    case 1:
                        _gSBox[i * 2] = _gMds0[(P[P01, b0] & 0xff) ^ M_b0(k0)];
                        _gSBox[i * 2 + 1] = _gMds1[(P[P11, b1] & 0xff) ^ M_b1(k0)];
                        _gSBox[i * 2 + 0x200] = _gMds2[(P[P21, b2] & 0xff) ^ M_b2(k0)];
                        _gSBox[i * 2 + 0x201] = _gMds3[(P[P31, b3] & 0xff) ^ M_b3(k0)];
                        break;
                    case 0: // 256 bits of key
                        b0 = (P[P04, b0] & 0xff) ^ M_b0(k3);
                        b1 = (P[P14, b1] & 0xff) ^ M_b1(k3);
                        b2 = (P[P24, b2] & 0xff) ^ M_b2(k3);
                        b3 = (P[P34, b3] & 0xff) ^ M_b3(k3);
                        // fall through, having pre-processed b[0]..b[3] with k32[3]
                        goto case 3;
                    case 3: // 192 bits of key
                        b0 = (P[P03, b0] & 0xff) ^ M_b0(k2);
                        b1 = (P[P13, b1] & 0xff) ^ M_b1(k2);
                        b2 = (P[P23, b2] & 0xff) ^ M_b2(k2);
                        b3 = (P[P33, b3] & 0xff) ^ M_b3(k2);
                        // fall through, having pre-processed b[0]..b[3] with k32[2]
                        goto case 2;
                    case 2: // 128 bits of key
                        _gSBox[i * 2] = _gMds0[(P[P01, (P[P02, b0] & 0xff) ^ M_b0(k1)] & 0xff) ^ M_b0(k0)];
                        _gSBox[i * 2 + 1] = _gMds1[(P[P11, (P[P12, b1] & 0xff) ^ M_b1(k1)] & 0xff) ^ M_b1(k0)];
                        _gSBox[i * 2 + 0x200] = _gMds2[(P[P21, (P[P22, b2] & 0xff) ^ M_b2(k1)] & 0xff) ^ M_b2(k0)];
                        _gSBox[i * 2 + 0x201] = _gMds3[(P[P31, (P[P32, b3] & 0xff) ^ M_b3(k1)] & 0xff) ^ M_b3(k0)];
                        break;
                }
            }

            /*
            * the function exits having setup the gSBox with the
            * input key material.
            */
        }

        /**
        * Encrypt the given input starting at the given offset and place
        * the result in the provided buffer starting at the given offset.
        * The input will be an exact multiple of our blocksize.
        *
        * encryptBlock uses the pre-calculated gSBox[] and subKey[]
        * arrays.
        */
        private void EncryptBlock(
            byte[] src,
            int srcIndex,
            byte[] dst,
            int dstIndex)
        {
            int x0 = (int)Pack.LE_To_UInt32(src, srcIndex) ^ _gSubKeys[InputWhiten];
            int x1 = (int)Pack.LE_To_UInt32(src, srcIndex + 4) ^ _gSubKeys[InputWhiten + 1];
            int x2 = (int)Pack.LE_To_UInt32(src, srcIndex + 8) ^ _gSubKeys[InputWhiten + 2];
            int x3 = (int)Pack.LE_To_UInt32(src, srcIndex + 12) ^ _gSubKeys[InputWhiten + 3];

            int k = RoundSubkeys;
            int t0, t1;
            for (int r = 0; r < Rounds; r += 2)
            {
                t0 = Fe32_0(x0);
                t1 = Fe32_3(x1);
                x2 ^= t0 + t1 + _gSubKeys[k++];
                x2 = Integers.RotateRight(x2, 1);
                x3 = Integers.RotateLeft(x3, 1) ^ (t0 + 2 * t1 + _gSubKeys[k++]);

                t0 = Fe32_0(x2);
                t1 = Fe32_3(x3);
                x0 ^= t0 + t1 + _gSubKeys[k++];
                x0 = Integers.RotateRight(x0, 1);
                x1 = Integers.RotateLeft(x1, 1) ^ (t0 + 2 * t1 + _gSubKeys[k++]);
            }

            Pack.UInt32_To_LE((uint)(x2 ^ _gSubKeys[OutputWhiten]), dst, dstIndex);
            Pack.UInt32_To_LE((uint)(x3 ^ _gSubKeys[OutputWhiten + 1]), dst, dstIndex + 4);
            Pack.UInt32_To_LE((uint)(x0 ^ _gSubKeys[OutputWhiten + 2]), dst, dstIndex + 8);
            Pack.UInt32_To_LE((uint)(x1 ^ _gSubKeys[OutputWhiten + 3]), dst, dstIndex + 12);
        }

        /**
        * Decrypt the given input starting at the given offset and place
        * the result in the provided buffer starting at the given offset.
        * The input will be an exact multiple of our blocksize.
        */
        private void DecryptBlock(
            byte[] src,
            int srcIndex,
            byte[] dst,
            int dstIndex)
        {
            int x2 = (int)Pack.LE_To_UInt32(src, srcIndex) ^ _gSubKeys[OutputWhiten];
            int x3 = (int)Pack.LE_To_UInt32(src, srcIndex + 4) ^ _gSubKeys[OutputWhiten + 1];
            int x0 = (int)Pack.LE_To_UInt32(src, srcIndex + 8) ^ _gSubKeys[OutputWhiten + 2];
            int x1 = (int)Pack.LE_To_UInt32(src, srcIndex + 12) ^ _gSubKeys[OutputWhiten + 3];

            int k = RoundSubkeys + 2 * Rounds - 1;
            int t0, t1;
            for (int r = 0; r < Rounds; r += 2)
            {
                t0 = Fe32_0(x2);
                t1 = Fe32_3(x3);
                x1 ^= t0 + 2 * t1 + _gSubKeys[k--];
                x0 = Integers.RotateLeft(x0, 1) ^ (t0 + t1 + _gSubKeys[k--]);
                x1 = Integers.RotateRight(x1, 1);

                t0 = Fe32_0(x0);
                t1 = Fe32_3(x1);
                x3 ^= t0 + 2 * t1 + _gSubKeys[k--];
                x2 = Integers.RotateLeft(x2, 1) ^ (t0 + t1 + _gSubKeys[k--]);
                x3 = Integers.RotateRight(x3, 1);
            }

            Pack.UInt32_To_LE((uint)(x0 ^ _gSubKeys[InputWhiten]), dst, dstIndex);
            Pack.UInt32_To_LE((uint)(x1 ^ _gSubKeys[InputWhiten + 1]), dst, dstIndex + 4);
            Pack.UInt32_To_LE((uint)(x2 ^ _gSubKeys[InputWhiten + 2]), dst, dstIndex + 8);
            Pack.UInt32_To_LE((uint)(x3 ^ _gSubKeys[InputWhiten + 3]), dst, dstIndex + 12);
        }

        /*
        * TODO:  This can be optimised and made cleaner by combining
        * the functionality in this function and applying it appropriately
        * to the creation of the subkeys during key setup.
        */
        private int F32(int x, int[] k32)
        {
            int b0 = M_b0(x);
            int b1 = M_b1(x);
            int b2 = M_b2(x);
            int b3 = M_b3(x);
            int k0 = k32[0];
            int k1 = k32[1];
            int k2 = k32[2];
            int k3 = k32[3];

            int result = 0;
            switch (_k64Cnt & 3)
            {
                case 1:
                    result = _gMds0[(P[P01, b0] & 0xff) ^ M_b0(k0)] ^
                             _gMds1[(P[P11, b1] & 0xff) ^ M_b1(k0)] ^
                             _gMds2[(P[P21, b2] & 0xff) ^ M_b2(k0)] ^
                             _gMds3[(P[P31, b3] & 0xff) ^ M_b3(k0)];
                    break;
                case 0: /* 256 bits of key */
                    b0 = (P[P04, b0] & 0xff) ^ M_b0(k3);
                    b1 = (P[P14, b1] & 0xff) ^ M_b1(k3);
                    b2 = (P[P24, b2] & 0xff) ^ M_b2(k3);
                    b3 = (P[P34, b3] & 0xff) ^ M_b3(k3);
                    goto case 3;
                case 3:
                    b0 = (P[P03, b0] & 0xff) ^ M_b0(k2);
                    b1 = (P[P13, b1] & 0xff) ^ M_b1(k2);
                    b2 = (P[P23, b2] & 0xff) ^ M_b2(k2);
                    b3 = (P[P33, b3] & 0xff) ^ M_b3(k2);
                    goto case 2;
                case 2:
                    result =
                        _gMds0[(P[P01, (P[P02, b0] & 0xff) ^ M_b0(k1)] & 0xff) ^ M_b0(k0)] ^
                        _gMds1[(P[P11, (P[P12, b1] & 0xff) ^ M_b1(k1)] & 0xff) ^ M_b1(k0)] ^
                        _gMds2[(P[P21, (P[P22, b2] & 0xff) ^ M_b2(k1)] & 0xff) ^ M_b2(k0)] ^
                        _gMds3[(P[P31, (P[P32, b3] & 0xff) ^ M_b3(k1)] & 0xff) ^ M_b3(k0)];
                    break;
            }

            return result;
        }

        /**
        * Use (12, 8) Reed-Solomon code over GF(256) to produce
        * a key S-box 32-bit entity from 2 key material 32-bit
        * entities.
        *
        * @param    k0 first 32-bit entity
        * @param    k1 second 32-bit entity
        * @return     Remainder polynomial Generated using RS code
        */
        private int RS_MDS_Encode(int k0, int k1)
        {
            int r = k1;
            for (int i = 0; i < 4; i++) // shift 1 byte at a time
            {
                r = RS_rem(r);
            }

            r ^= k0;
            for (int i = 0; i < 4; i++)
            {
                r = RS_rem(r);
            }

            return r;
        }

        /**
        * Reed-Solomon code parameters: (12,8) reversible code:
		* <p>
        * <pre>
        * G(x) = x^4 + (a+1/a)x^3 + ax^2 + (a+1/a)x + 1
        * </pre>
        * where a = primitive root of field generator 0x14D
		* </p>
        */
        private int RS_rem(int x)
        {
            int b = (int)(((uint)x >> 24) & 0xff);
            int g2 = ((b << 1) ^
                      ((b & 0x80) != 0 ? RsGfFdbk : 0)) & 0xff;
            int g3 = ((int)((uint)b >> 1) ^
                      ((b & 0x01) != 0 ? (int)((uint)RsGfFdbk >> 1) : 0)) ^ g2;
            return ((x << 8) ^ (g3 << 24) ^ (g2 << 16) ^ (g3 << 8) ^ b);
        }

        private int Lfsr1(int x)
        {
            return (x >> 1) ^
                   (((x & 0x01) != 0) ? Gf256Fdbk2 : 0);
        }

        private int Lfsr2(int x)
        {
            return (x >> 2) ^
                   (((x & 0x02) != 0) ? Gf256Fdbk2 : 0) ^
                   (((x & 0x01) != 0) ? Gf256Fdbk4 : 0);
        }

        private int Mx_X(int x)
        {
            return x ^ Lfsr2(x);
        } // 5B

        private int Mx_Y(int x)
        {
            return x ^ Lfsr1(x) ^ Lfsr2(x);
        } // EF

        private int M_b0(int x)
        {
            return x & 0xff;
        }

        private int M_b1(int x)
        {
            return (int)((uint)x >> 8) & 0xff;
        }

        private int M_b2(int x)
        {
            return (int)((uint)x >> 16) & 0xff;
        }

        private int M_b3(int x)
        {
            return (int)((uint)x >> 24) & 0xff;
        }

        private int Fe32_0(int x)
        {
            return _gSBox[0x000 + 2 * (x & 0xff)] ^
                   _gSBox[0x001 + 2 * ((int)((uint)x >> 8) & 0xff)] ^
                   _gSBox[0x200 + 2 * ((int)((uint)x >> 16) & 0xff)] ^
                   _gSBox[0x201 + 2 * ((int)((uint)x >> 24) & 0xff)];
        }

        private int Fe32_3(int x)
        {
            return _gSBox[0x000 + 2 * ((int)((uint)x >> 24) & 0xff)] ^
                   _gSBox[0x001 + 2 * (x & 0xff)] ^
                   _gSBox[0x200 + 2 * ((int)((uint)x >> 8) & 0xff)] ^
                   _gSBox[0x201 + 2 * ((int)((uint)x >> 16) & 0xff)];
        }
    }
}
#pragma warning restore
#endif
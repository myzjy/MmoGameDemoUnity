#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
     * RFC 5794.
     * 
     * ARIA is a 128-bit block cipher with 128-, 192-, and 256-bit keys.
     */
    public class AriaEngine
        : IBlockCipher
    {
        protected const int BlockSize = 16;

        private static readonly byte[][] C =
        {
            Hex.DecodeStrict("517cc1b727220a94fe13abe8fa9a6ee0"),
            Hex.DecodeStrict("6db14acc9e21c820ff28b1d5ef5de2b0"), Hex.DecodeStrict("db92371d2126e9700324977504e8c90e")
        };

        private static readonly byte[] Sb1Sbox =
        {
            (byte)0x63, (byte)0x7c, (byte)0x77, (byte)0x7b, (byte)0xf2, (byte)0x6b,
            (byte)0x6f, (byte)0xc5, (byte)0x30, (byte)0x01, (byte)0x67, (byte)0x2b, (byte)0xfe, (byte)0xd7, (byte)0xab,
            (byte)0x76, (byte)0xca, (byte)0x82, (byte)0xc9, (byte)0x7d, (byte)0xfa, (byte)0x59, (byte)0x47, (byte)0xf0,
            (byte)0xad, (byte)0xd4, (byte)0xa2, (byte)0xaf, (byte)0x9c, (byte)0xa4, (byte)0x72, (byte)0xc0, (byte)0xb7,
            (byte)0xfd, (byte)0x93, (byte)0x26, (byte)0x36, (byte)0x3f, (byte)0xf7, (byte)0xcc, (byte)0x34, (byte)0xa5,
            (byte)0xe5, (byte)0xf1, (byte)0x71, (byte)0xd8, (byte)0x31, (byte)0x15, (byte)0x04, (byte)0xc7, (byte)0x23,
            (byte)0xc3, (byte)0x18, (byte)0x96, (byte)0x05, (byte)0x9a, (byte)0x07, (byte)0x12, (byte)0x80, (byte)0xe2,
            (byte)0xeb, (byte)0x27, (byte)0xb2, (byte)0x75, (byte)0x09, (byte)0x83, (byte)0x2c, (byte)0x1a, (byte)0x1b,
            (byte)0x6e, (byte)0x5a, (byte)0xa0, (byte)0x52, (byte)0x3b, (byte)0xd6, (byte)0xb3, (byte)0x29, (byte)0xe3,
            (byte)0x2f, (byte)0x84, (byte)0x53, (byte)0xd1, (byte)0x00, (byte)0xed, (byte)0x20, (byte)0xfc, (byte)0xb1,
            (byte)0x5b, (byte)0x6a, (byte)0xcb, (byte)0xbe, (byte)0x39, (byte)0x4a, (byte)0x4c, (byte)0x58, (byte)0xcf,
            (byte)0xd0, (byte)0xef, (byte)0xaa, (byte)0xfb, (byte)0x43, (byte)0x4d, (byte)0x33, (byte)0x85, (byte)0x45,
            (byte)0xf9, (byte)0x02, (byte)0x7f, (byte)0x50, (byte)0x3c, (byte)0x9f, (byte)0xa8, (byte)0x51, (byte)0xa3,
            (byte)0x40, (byte)0x8f, (byte)0x92, (byte)0x9d, (byte)0x38, (byte)0xf5, (byte)0xbc, (byte)0xb6, (byte)0xda,
            (byte)0x21, (byte)0x10, (byte)0xff, (byte)0xf3, (byte)0xd2, (byte)0xcd, (byte)0x0c, (byte)0x13, (byte)0xec,
            (byte)0x5f, (byte)0x97, (byte)0x44, (byte)0x17, (byte)0xc4, (byte)0xa7, (byte)0x7e, (byte)0x3d, (byte)0x64,
            (byte)0x5d, (byte)0x19, (byte)0x73, (byte)0x60, (byte)0x81, (byte)0x4f, (byte)0xdc, (byte)0x22, (byte)0x2a,
            (byte)0x90, (byte)0x88, (byte)0x46, (byte)0xee, (byte)0xb8, (byte)0x14, (byte)0xde, (byte)0x5e, (byte)0x0b,
            (byte)0xdb, (byte)0xe0, (byte)0x32, (byte)0x3a, (byte)0x0a, (byte)0x49, (byte)0x06, (byte)0x24, (byte)0x5c,
            (byte)0xc2, (byte)0xd3, (byte)0xac, (byte)0x62, (byte)0x91, (byte)0x95, (byte)0xe4, (byte)0x79, (byte)0xe7,
            (byte)0xc8, (byte)0x37, (byte)0x6d, (byte)0x8d, (byte)0xd5, (byte)0x4e, (byte)0xa9, (byte)0x6c, (byte)0x56,
            (byte)0xf4, (byte)0xea, (byte)0x65, (byte)0x7a, (byte)0xae, (byte)0x08, (byte)0xba, (byte)0x78, (byte)0x25,
            (byte)0x2e, (byte)0x1c, (byte)0xa6, (byte)0xb4, (byte)0xc6, (byte)0xe8, (byte)0xdd, (byte)0x74, (byte)0x1f,
            (byte)0x4b, (byte)0xbd, (byte)0x8b, (byte)0x8a, (byte)0x70, (byte)0x3e, (byte)0xb5, (byte)0x66, (byte)0x48,
            (byte)0x03, (byte)0xf6, (byte)0x0e, (byte)0x61, (byte)0x35, (byte)0x57, (byte)0xb9, (byte)0x86, (byte)0xc1,
            (byte)0x1d, (byte)0x9e, (byte)0xe1, (byte)0xf8, (byte)0x98, (byte)0x11, (byte)0x69, (byte)0xd9, (byte)0x8e,
            (byte)0x94, (byte)0x9b, (byte)0x1e, (byte)0x87, (byte)0xe9, (byte)0xce, (byte)0x55, (byte)0x28, (byte)0xdf,
            (byte)0x8c, (byte)0xa1, (byte)0x89, (byte)0x0d, (byte)0xbf, (byte)0xe6, (byte)0x42, (byte)0x68, (byte)0x41,
            (byte)0x99, (byte)0x2d, (byte)0x0f, (byte)0xb0, (byte)0x54, (byte)0xbb, (byte)0x16
        };

        private static readonly byte[] Sb2Sbox =
        {
            (byte)0xe2, (byte)0x4e, (byte)0x54, (byte)0xfc, (byte)0x94, (byte)0xc2,
            (byte)0x4a, (byte)0xcc, (byte)0x62, (byte)0x0d, (byte)0x6a, (byte)0x46, (byte)0x3c, (byte)0x4d, (byte)0x8b,
            (byte)0xd1, (byte)0x5e, (byte)0xfa, (byte)0x64, (byte)0xcb, (byte)0xb4, (byte)0x97, (byte)0xbe, (byte)0x2b,
            (byte)0xbc, (byte)0x77, (byte)0x2e, (byte)0x03, (byte)0xd3, (byte)0x19, (byte)0x59, (byte)0xc1, (byte)0x1d,
            (byte)0x06, (byte)0x41, (byte)0x6b, (byte)0x55, (byte)0xf0, (byte)0x99, (byte)0x69, (byte)0xea, (byte)0x9c,
            (byte)0x18, (byte)0xae, (byte)0x63, (byte)0xdf, (byte)0xe7, (byte)0xbb, (byte)0x00, (byte)0x73, (byte)0x66,
            (byte)0xfb, (byte)0x96, (byte)0x4c, (byte)0x85, (byte)0xe4, (byte)0x3a, (byte)0x09, (byte)0x45, (byte)0xaa,
            (byte)0x0f, (byte)0xee, (byte)0x10, (byte)0xeb, (byte)0x2d, (byte)0x7f, (byte)0xf4, (byte)0x29, (byte)0xac,
            (byte)0xcf, (byte)0xad, (byte)0x91, (byte)0x8d, (byte)0x78, (byte)0xc8, (byte)0x95, (byte)0xf9, (byte)0x2f,
            (byte)0xce, (byte)0xcd, (byte)0x08, (byte)0x7a, (byte)0x88, (byte)0x38, (byte)0x5c, (byte)0x83, (byte)0x2a,
            (byte)0x28, (byte)0x47, (byte)0xdb, (byte)0xb8, (byte)0xc7, (byte)0x93, (byte)0xa4, (byte)0x12, (byte)0x53,
            (byte)0xff, (byte)0x87, (byte)0x0e, (byte)0x31, (byte)0x36, (byte)0x21, (byte)0x58, (byte)0x48, (byte)0x01,
            (byte)0x8e, (byte)0x37, (byte)0x74, (byte)0x32, (byte)0xca, (byte)0xe9, (byte)0xb1, (byte)0xb7, (byte)0xab,
            (byte)0x0c, (byte)0xd7, (byte)0xc4, (byte)0x56, (byte)0x42, (byte)0x26, (byte)0x07, (byte)0x98, (byte)0x60,
            (byte)0xd9, (byte)0xb6, (byte)0xb9, (byte)0x11, (byte)0x40, (byte)0xec, (byte)0x20, (byte)0x8c, (byte)0xbd,
            (byte)0xa0, (byte)0xc9, (byte)0x84, (byte)0x04, (byte)0x49, (byte)0x23, (byte)0xf1, (byte)0x4f, (byte)0x50,
            (byte)0x1f, (byte)0x13, (byte)0xdc, (byte)0xd8, (byte)0xc0, (byte)0x9e, (byte)0x57, (byte)0xe3, (byte)0xc3,
            (byte)0x7b, (byte)0x65, (byte)0x3b, (byte)0x02, (byte)0x8f, (byte)0x3e, (byte)0xe8, (byte)0x25, (byte)0x92,
            (byte)0xe5, (byte)0x15, (byte)0xdd, (byte)0xfd, (byte)0x17, (byte)0xa9, (byte)0xbf, (byte)0xd4, (byte)0x9a,
            (byte)0x7e, (byte)0xc5, (byte)0x39, (byte)0x67, (byte)0xfe, (byte)0x76, (byte)0x9d, (byte)0x43, (byte)0xa7,
            (byte)0xe1, (byte)0xd0, (byte)0xf5, (byte)0x68, (byte)0xf2, (byte)0x1b, (byte)0x34, (byte)0x70, (byte)0x05,
            (byte)0xa3, (byte)0x8a, (byte)0xd5, (byte)0x79, (byte)0x86, (byte)0xa8, (byte)0x30, (byte)0xc6, (byte)0x51,
            (byte)0x4b, (byte)0x1e, (byte)0xa6, (byte)0x27, (byte)0xf6, (byte)0x35, (byte)0xd2, (byte)0x6e, (byte)0x24,
            (byte)0x16, (byte)0x82, (byte)0x5f, (byte)0xda, (byte)0xe6, (byte)0x75, (byte)0xa2, (byte)0xef, (byte)0x2c,
            (byte)0xb2, (byte)0x1c, (byte)0x9f, (byte)0x5d, (byte)0x6f, (byte)0x80, (byte)0x0a, (byte)0x72, (byte)0x44,
            (byte)0x9b, (byte)0x6c, (byte)0x90, (byte)0x0b, (byte)0x5b, (byte)0x33, (byte)0x7d, (byte)0x5a, (byte)0x52,
            (byte)0xf3, (byte)0x61, (byte)0xa1, (byte)0xf7, (byte)0xb0, (byte)0xd6, (byte)0x3f, (byte)0x7c, (byte)0x6d,
            (byte)0xed, (byte)0x14, (byte)0xe0, (byte)0xa5, (byte)0x3d, (byte)0x22, (byte)0xb3, (byte)0xf8, (byte)0x89,
            (byte)0xde, (byte)0x71, (byte)0x1a, (byte)0xaf, (byte)0xba, (byte)0xb5, (byte)0x81
        };

        private static readonly byte[] Sb3Sbox =
        {
            (byte)0x52, (byte)0x09, (byte)0x6a, (byte)0xd5, (byte)0x30, (byte)0x36,
            (byte)0xa5, (byte)0x38, (byte)0xbf, (byte)0x40, (byte)0xa3, (byte)0x9e, (byte)0x81, (byte)0xf3, (byte)0xd7,
            (byte)0xfb, (byte)0x7c, (byte)0xe3, (byte)0x39, (byte)0x82, (byte)0x9b, (byte)0x2f, (byte)0xff, (byte)0x87,
            (byte)0x34, (byte)0x8e, (byte)0x43, (byte)0x44, (byte)0xc4, (byte)0xde, (byte)0xe9, (byte)0xcb, (byte)0x54,
            (byte)0x7b, (byte)0x94, (byte)0x32, (byte)0xa6, (byte)0xc2, (byte)0x23, (byte)0x3d, (byte)0xee, (byte)0x4c,
            (byte)0x95, (byte)0x0b, (byte)0x42, (byte)0xfa, (byte)0xc3, (byte)0x4e, (byte)0x08, (byte)0x2e, (byte)0xa1,
            (byte)0x66, (byte)0x28, (byte)0xd9, (byte)0x24, (byte)0xb2, (byte)0x76, (byte)0x5b, (byte)0xa2, (byte)0x49,
            (byte)0x6d, (byte)0x8b, (byte)0xd1, (byte)0x25, (byte)0x72, (byte)0xf8, (byte)0xf6, (byte)0x64, (byte)0x86,
            (byte)0x68, (byte)0x98, (byte)0x16, (byte)0xd4, (byte)0xa4, (byte)0x5c, (byte)0xcc, (byte)0x5d, (byte)0x65,
            (byte)0xb6, (byte)0x92, (byte)0x6c, (byte)0x70, (byte)0x48, (byte)0x50, (byte)0xfd, (byte)0xed, (byte)0xb9,
            (byte)0xda, (byte)0x5e, (byte)0x15, (byte)0x46, (byte)0x57, (byte)0xa7, (byte)0x8d, (byte)0x9d, (byte)0x84,
            (byte)0x90, (byte)0xd8, (byte)0xab, (byte)0x00, (byte)0x8c, (byte)0xbc, (byte)0xd3, (byte)0x0a, (byte)0xf7,
            (byte)0xe4, (byte)0x58, (byte)0x05, (byte)0xb8, (byte)0xb3, (byte)0x45, (byte)0x06, (byte)0xd0, (byte)0x2c,
            (byte)0x1e, (byte)0x8f, (byte)0xca, (byte)0x3f, (byte)0x0f, (byte)0x02, (byte)0xc1, (byte)0xaf, (byte)0xbd,
            (byte)0x03, (byte)0x01, (byte)0x13, (byte)0x8a, (byte)0x6b, (byte)0x3a, (byte)0x91, (byte)0x11, (byte)0x41,
            (byte)0x4f, (byte)0x67, (byte)0xdc, (byte)0xea, (byte)0x97, (byte)0xf2, (byte)0xcf, (byte)0xce, (byte)0xf0,
            (byte)0xb4, (byte)0xe6, (byte)0x73, (byte)0x96, (byte)0xac, (byte)0x74, (byte)0x22, (byte)0xe7, (byte)0xad,
            (byte)0x35, (byte)0x85, (byte)0xe2, (byte)0xf9, (byte)0x37, (byte)0xe8, (byte)0x1c, (byte)0x75, (byte)0xdf,
            (byte)0x6e, (byte)0x47, (byte)0xf1, (byte)0x1a, (byte)0x71, (byte)0x1d, (byte)0x29, (byte)0xc5, (byte)0x89,
            (byte)0x6f, (byte)0xb7, (byte)0x62, (byte)0x0e, (byte)0xaa, (byte)0x18, (byte)0xbe, (byte)0x1b, (byte)0xfc,
            (byte)0x56, (byte)0x3e, (byte)0x4b, (byte)0xc6, (byte)0xd2, (byte)0x79, (byte)0x20, (byte)0x9a, (byte)0xdb,
            (byte)0xc0, (byte)0xfe, (byte)0x78, (byte)0xcd, (byte)0x5a, (byte)0xf4, (byte)0x1f, (byte)0xdd, (byte)0xa8,
            (byte)0x33, (byte)0x88, (byte)0x07, (byte)0xc7, (byte)0x31, (byte)0xb1, (byte)0x12, (byte)0x10, (byte)0x59,
            (byte)0x27, (byte)0x80, (byte)0xec, (byte)0x5f, (byte)0x60, (byte)0x51, (byte)0x7f, (byte)0xa9, (byte)0x19,
            (byte)0xb5, (byte)0x4a, (byte)0x0d, (byte)0x2d, (byte)0xe5, (byte)0x7a, (byte)0x9f, (byte)0x93, (byte)0xc9,
            (byte)0x9c, (byte)0xef, (byte)0xa0, (byte)0xe0, (byte)0x3b, (byte)0x4d, (byte)0xae, (byte)0x2a, (byte)0xf5,
            (byte)0xb0, (byte)0xc8, (byte)0xeb, (byte)0xbb, (byte)0x3c, (byte)0x83, (byte)0x53, (byte)0x99, (byte)0x61,
            (byte)0x17, (byte)0x2b, (byte)0x04, (byte)0x7e, (byte)0xba, (byte)0x77, (byte)0xd6, (byte)0x26, (byte)0xe1,
            (byte)0x69, (byte)0x14, (byte)0x63, (byte)0x55, (byte)0x21, (byte)0x0c, (byte)0x7d
        };

        private static readonly byte[] Sb4Sbox =
        {
            (byte)0x30, (byte)0x68, (byte)0x99, (byte)0x1b, (byte)0x87, (byte)0xb9,
            (byte)0x21, (byte)0x78, (byte)0x50, (byte)0x39, (byte)0xdb, (byte)0xe1, (byte)0x72, (byte)0x9, (byte)0x62,
            (byte)0x3c, (byte)0x3e, (byte)0x7e, (byte)0x5e, (byte)0x8e, (byte)0xf1, (byte)0xa0, (byte)0xcc, (byte)0xa3,
            (byte)0x2a, (byte)0x1d, (byte)0xfb, (byte)0xb6, (byte)0xd6, (byte)0x20, (byte)0xc4, (byte)0x8d, (byte)0x81,
            (byte)0x65, (byte)0xf5, (byte)0x89, (byte)0xcb, (byte)0x9d, (byte)0x77, (byte)0xc6, (byte)0x57, (byte)0x43,
            (byte)0x56, (byte)0x17, (byte)0xd4, (byte)0x40, (byte)0x1a, (byte)0x4d, (byte)0xc0, (byte)0x63, (byte)0x6c,
            (byte)0xe3, (byte)0xb7, (byte)0xc8, (byte)0x64, (byte)0x6a, (byte)0x53, (byte)0xaa, (byte)0x38, (byte)0x98,
            (byte)0x0c, (byte)0xf4, (byte)0x9b, (byte)0xed, (byte)0x7f, (byte)0x22, (byte)0x76, (byte)0xaf, (byte)0xdd,
            (byte)0x3a, (byte)0x0b, (byte)0x58, (byte)0x67, (byte)0x88, (byte)0x06, (byte)0xc3, (byte)0x35, (byte)0x0d,
            (byte)0x01, (byte)0x8b, (byte)0x8c, (byte)0xc2, (byte)0xe6, (byte)0x5f, (byte)0x02, (byte)0x24, (byte)0x75,
            (byte)0x93, (byte)0x66, (byte)0x1e, (byte)0xe5, (byte)0xe2, (byte)0x54, (byte)0xd8, (byte)0x10, (byte)0xce,
            (byte)0x7a, (byte)0xe8, (byte)0x08, (byte)0x2c, (byte)0x12, (byte)0x97, (byte)0x32, (byte)0xab, (byte)0xb4,
            (byte)0x27, (byte)0x0a, (byte)0x23, (byte)0xdf, (byte)0xef, (byte)0xca, (byte)0xd9, (byte)0xb8, (byte)0xfa,
            (byte)0xdc, (byte)0x31, (byte)0x6b, (byte)0xd1, (byte)0xad, (byte)0x19, (byte)0x49, (byte)0xbd, (byte)0x51,
            (byte)0x96, (byte)0xee, (byte)0xe4, (byte)0xa8, (byte)0x41, (byte)0xda, (byte)0xff, (byte)0xcd, (byte)0x55,
            (byte)0x86, (byte)0x36, (byte)0xbe, (byte)0x61, (byte)0x52, (byte)0xf8, (byte)0xbb, (byte)0x0e, (byte)0x82,
            (byte)0x48, (byte)0x69, (byte)0x9a, (byte)0xe0, (byte)0x47, (byte)0x9e, (byte)0x5c, (byte)0x04, (byte)0x4b,
            (byte)0x34, (byte)0x15, (byte)0x79, (byte)0x26, (byte)0xa7, (byte)0xde, (byte)0x29, (byte)0xae, (byte)0x92,
            (byte)0xd7, (byte)0x84, (byte)0xe9, (byte)0xd2, (byte)0xba, (byte)0x5d, (byte)0xf3, (byte)0xc5, (byte)0xb0,
            (byte)0xbf, (byte)0xa4, (byte)0x3b, (byte)0x71, (byte)0x44, (byte)0x46, (byte)0x2b, (byte)0xfc, (byte)0xeb,
            (byte)0x6f, (byte)0xd5, (byte)0xf6, (byte)0x14, (byte)0xfe, (byte)0x7c, (byte)0x70, (byte)0x5a, (byte)0x7d,
            (byte)0xfd, (byte)0x2f, (byte)0x18, (byte)0x83, (byte)0x16, (byte)0xa5, (byte)0x91, (byte)0x1f, (byte)0x05,
            (byte)0x95, (byte)0x74, (byte)0xa9, (byte)0xc1, (byte)0x5b, (byte)0x4a, (byte)0x85, (byte)0x6d, (byte)0x13,
            (byte)0x07, (byte)0x4f, (byte)0x4e, (byte)0x45, (byte)0xb2, (byte)0x0f, (byte)0xc9, (byte)0x1c, (byte)0xa6,
            (byte)0xbc, (byte)0xec, (byte)0x73, (byte)0x90, (byte)0x7b, (byte)0xcf, (byte)0x59, (byte)0x8f, (byte)0xa1,
            (byte)0xf9, (byte)0x2d, (byte)0xf2, (byte)0xb1, (byte)0x00, (byte)0x94, (byte)0x37, (byte)0x9f, (byte)0xd0,
            (byte)0x2e, (byte)0x9c, (byte)0x6e, (byte)0x28, (byte)0x3f, (byte)0x80, (byte)0xf0, (byte)0x3d, (byte)0xd3,
            (byte)0x25, (byte)0x8a, (byte)0xb5, (byte)0xe7, (byte)0x42, (byte)0xb3, (byte)0xc7, (byte)0xea, (byte)0xf7,
            (byte)0x4c, (byte)0x11, (byte)0x33, (byte)0x03, (byte)0xa2, (byte)0xac, (byte)0x60
        };

        private byte[][] _mRoundKeys;

        public virtual void Init(bool forEncryption, ICipherParameters parameters)
        {
            KeyParameter keyParameter = parameters as KeyParameter;

            if (keyParameter == null)
                throw new ArgumentException("invalid parameter passed to ARIA init - "
                                            + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(
                                                parameters));

            this._mRoundKeys = KeySchedule(forEncryption, keyParameter.GetKey());
        }

        public virtual string AlgorithmName
        {
            get { return "ARIA"; }
        }

        public virtual bool IsPartialBlockOkay
        {
            get { return false; }
        }

        public virtual int GetBlockSize()
        {
            return BlockSize;
        }

        public virtual int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
        {
            if (_mRoundKeys == null)
                throw new InvalidOperationException("ARIA engine not initialised");

            Check.DataLength(input, inOff, BlockSize, "input buffer too short");
            Check.OutputLength(output, outOff, BlockSize, "output buffer too short");

            byte[] z = new byte[BlockSize];
            Array.Copy(input, inOff, z, 0, BlockSize);

            int i = 0, rounds = _mRoundKeys.Length - 3;
            while (i < rounds)
            {
                Fo(z, _mRoundKeys[i++]);
                Fe(z, _mRoundKeys[i++]);
            }

            Fo(z, _mRoundKeys[i++]);
            Xor(z, _mRoundKeys[i++]);
            Sl2(z);
            Xor(z, _mRoundKeys[i]);

            Array.Copy(z, 0, output, outOff, BlockSize);

            return BlockSize;
        }

        public virtual void Reset()
        {
            // Empty
        }

        protected static void A(byte[] z)
        {
            byte x0 = z[0],
                x1 = z[1],
                x2 = z[2],
                x3 = z[3],
                x4 = z[4],
                x5 = z[5],
                x6 = z[6],
                x7 = z[7],
                x8 = z[8],
                x9 = z[9],
                x10 = z[10],
                x11 = z[11],
                x12 = z[12],
                x13 = z[13],
                x14 = z[14],
                x15 = z[15];

            z[0] = (byte)(x3 ^ x4 ^ x6 ^ x8 ^ x9 ^ x13 ^ x14);
            z[1] = (byte)(x2 ^ x5 ^ x7 ^ x8 ^ x9 ^ x12 ^ x15);
            z[2] = (byte)(x1 ^ x4 ^ x6 ^ x10 ^ x11 ^ x12 ^ x15);
            z[3] = (byte)(x0 ^ x5 ^ x7 ^ x10 ^ x11 ^ x13 ^ x14);
            z[4] = (byte)(x0 ^ x2 ^ x5 ^ x8 ^ x11 ^ x14 ^ x15);
            z[5] = (byte)(x1 ^ x3 ^ x4 ^ x9 ^ x10 ^ x14 ^ x15);
            z[6] = (byte)(x0 ^ x2 ^ x7 ^ x9 ^ x10 ^ x12 ^ x13);
            z[7] = (byte)(x1 ^ x3 ^ x6 ^ x8 ^ x11 ^ x12 ^ x13);
            z[8] = (byte)(x0 ^ x1 ^ x4 ^ x7 ^ x10 ^ x13 ^ x15);
            z[9] = (byte)(x0 ^ x1 ^ x5 ^ x6 ^ x11 ^ x12 ^ x14);
            z[10] = (byte)(x2 ^ x3 ^ x5 ^ x6 ^ x8 ^ x13 ^ x15);
            z[11] = (byte)(x2 ^ x3 ^ x4 ^ x7 ^ x9 ^ x12 ^ x14);
            z[12] = (byte)(x1 ^ x2 ^ x6 ^ x7 ^ x9 ^ x11 ^ x12);
            z[13] = (byte)(x0 ^ x3 ^ x6 ^ x7 ^ x8 ^ x10 ^ x13);
            z[14] = (byte)(x0 ^ x3 ^ x4 ^ x5 ^ x9 ^ x11 ^ x14);
            z[15] = (byte)(x1 ^ x2 ^ x4 ^ x5 ^ x8 ^ x10 ^ x15);
        }

        protected static void Fe(byte[] d, byte[] rk)
        {
            Xor(d, rk);
            Sl2(d);
            A(d);
        }

        protected static void Fo(byte[] d, byte[] rk)
        {
            Xor(d, rk);
            Sl1(d);
            A(d);
        }

        protected static byte[][] KeySchedule(bool forEncryption, byte[] k)
        {
            int keyLen = k.Length;
            if (keyLen < 16 || keyLen > 32 || (keyLen & 7) != 0)
                throw new ArgumentException("Key length not 128/192/256 bits.");

            int keyLenIdx = (keyLen >> 3) - 2;

            byte[] ck1 = C[keyLenIdx];
            byte[] ck2 = C[(keyLenIdx + 1) % 3];
            byte[] ck3 = C[(keyLenIdx + 2) % 3];

            byte[] kl = new byte[16], kr = new byte[16];
            Array.Copy(k, 0, kl, 0, 16);
            Array.Copy(k, 16, kr, 0, keyLen - 16);

            byte[] w0 = new byte[16];
            byte[] w1 = new byte[16];
            byte[] w2 = new byte[16];
            byte[] w3 = new byte[16];

            Array.Copy(kl, 0, w0, 0, 16);

            Array.Copy(w0, 0, w1, 0, 16);
            Fo(w1, ck1);
            Xor(w1, kr);

            Array.Copy(w1, 0, w2, 0, 16);
            Fe(w2, ck2);
            Xor(w2, w0);

            Array.Copy(w2, 0, w3, 0, 16);
            Fo(w3, ck3);
            Xor(w3, w1);

            int numRounds = 12 + (keyLenIdx * 2);
            byte[][] rks = new byte[numRounds + 1][];

            rks[0] = KeyScheduleRound(w0, w1, 19);
            rks[1] = KeyScheduleRound(w1, w2, 19);
            rks[2] = KeyScheduleRound(w2, w3, 19);
            rks[3] = KeyScheduleRound(w3, w0, 19);

            rks[4] = KeyScheduleRound(w0, w1, 31);
            rks[5] = KeyScheduleRound(w1, w2, 31);
            rks[6] = KeyScheduleRound(w2, w3, 31);
            rks[7] = KeyScheduleRound(w3, w0, 31);

            rks[8] = KeyScheduleRound(w0, w1, 67);
            rks[9] = KeyScheduleRound(w1, w2, 67);
            rks[10] = KeyScheduleRound(w2, w3, 67);
            rks[11] = KeyScheduleRound(w3, w0, 67);

            rks[12] = KeyScheduleRound(w0, w1, 97);
            if (numRounds > 12)
            {
                rks[13] = KeyScheduleRound(w1, w2, 97);
                rks[14] = KeyScheduleRound(w2, w3, 97);
                if (numRounds > 14)
                {
                    rks[15] = KeyScheduleRound(w3, w0, 97);

                    rks[16] = KeyScheduleRound(w0, w1, 109);
                }
            }

            if (!forEncryption)
            {
                ReverseKeys(rks);

                for (int i = 1; i < numRounds; ++i)
                {
                    A(rks[i]);
                }
            }

            return rks;
        }

        protected static byte[] KeyScheduleRound(byte[] w, byte[] wr, int n)
        {
            byte[] rk = new byte[16];

            int off = n >> 3, right = n & 7, left = 8 - right;

            int hi = wr[15 - off] & 0xFF;

            for (int to = 0; to < 16; ++to)
            {
                int lo = wr[(to - off) & 0xF] & 0xFF;

                int b = (hi << left) | (lo >> right);
                b ^= (w[to] & 0xFF);

                rk[to] = (byte)b;

                hi = lo;
            }

            return rk;
        }

        protected static void ReverseKeys(byte[][] keys)
        {
            int length = keys.Length, limit = length / 2, last = length - 1;
            for (int i = 0; i < limit; ++i)
            {
                byte[] t = keys[i];
                keys[i] = keys[last - i];
                keys[last - i] = t;
            }
        }

        protected static byte Sb1(byte x)
        {
            return Sb1Sbox[x & 0xFF];
        }

        protected static byte Sb2(byte x)
        {
            return Sb2Sbox[x & 0xFF];
        }

        protected static byte Sb3(byte x)
        {
            return Sb3Sbox[x & 0xFF];
        }

        protected static byte Sb4(byte x)
        {
            return Sb4Sbox[x & 0xFF];
        }

        protected static void Sl1(byte[] z)
        {
            z[0] = Sb1(z[0]);
            z[1] = Sb2(z[1]);
            z[2] = Sb3(z[2]);
            z[3] = Sb4(z[3]);
            z[4] = Sb1(z[4]);
            z[5] = Sb2(z[5]);
            z[6] = Sb3(z[6]);
            z[7] = Sb4(z[7]);
            z[8] = Sb1(z[8]);
            z[9] = Sb2(z[9]);
            z[10] = Sb3(z[10]);
            z[11] = Sb4(z[11]);
            z[12] = Sb1(z[12]);
            z[13] = Sb2(z[13]);
            z[14] = Sb3(z[14]);
            z[15] = Sb4(z[15]);
        }

        protected static void Sl2(byte[] z)
        {
            z[0] = Sb3(z[0]);
            z[1] = Sb4(z[1]);
            z[2] = Sb1(z[2]);
            z[3] = Sb2(z[3]);
            z[4] = Sb3(z[4]);
            z[5] = Sb4(z[5]);
            z[6] = Sb1(z[6]);
            z[7] = Sb2(z[7]);
            z[8] = Sb3(z[8]);
            z[9] = Sb4(z[9]);
            z[10] = Sb1(z[10]);
            z[11] = Sb2(z[11]);
            z[12] = Sb3(z[12]);
            z[13] = Sb4(z[13]);
            z[14] = Sb1(z[14]);
            z[15] = Sb2(z[15]);
        }

        protected static void Xor(byte[] z, byte[] x)
        {
            for (int i = 0; i < 16; ++i)
            {
                z[i] ^= x[i];
            }
        }
    }
}
#pragma warning restore
#endif
#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
	* Camellia - based on RFC 3713, smaller implementation, about half the size of CamelliaEngine.
	*/
    public class CamelliaLightEngine
        : IBlockCipher
    {
        private const int BlockSize = 16;

        private static readonly uint[] Sigma =
        {
            0xa09e667f, 0x3bcc908b,
            0xb67ae858, 0x4caa73b2,
            0xc6ef372f, 0xe94f82be,
            0x54ff53a5, 0xf1d36f1c,
            0x10e527fa, 0xde682d1d,
            0xb05688c2, 0xb3e6c1fd
        };

        /*
        *
        * S-box data
        *
        */
        private static readonly byte[] Sbox1 =
        {
            (byte)112, (byte)130, (byte)44, (byte)236,
            (byte)179, (byte)39, (byte)192, (byte)229,
            (byte)228, (byte)133, (byte)87, (byte)53,
            (byte)234, (byte)12, (byte)174, (byte)65,
            (byte)35, (byte)239, (byte)107, (byte)147,
            (byte)69, (byte)25, (byte)165, (byte)33,
            (byte)237, (byte)14, (byte)79, (byte)78,
            (byte)29, (byte)101, (byte)146, (byte)189,
            (byte)134, (byte)184, (byte)175, (byte)143,
            (byte)124, (byte)235, (byte)31, (byte)206,
            (byte)62, (byte)48, (byte)220, (byte)95,
            (byte)94, (byte)197, (byte)11, (byte)26,
            (byte)166, (byte)225, (byte)57, (byte)202,
            (byte)213, (byte)71, (byte)93, (byte)61,
            (byte)217, (byte)1, (byte)90, (byte)214,
            (byte)81, (byte)86, (byte)108, (byte)77,
            (byte)139, (byte)13, (byte)154, (byte)102,
            (byte)251, (byte)204, (byte)176, (byte)45,
            (byte)116, (byte)18, (byte)43, (byte)32,
            (byte)240, (byte)177, (byte)132, (byte)153,
            (byte)223, (byte)76, (byte)203, (byte)194,
            (byte)52, (byte)126, (byte)118, (byte)5,
            (byte)109, (byte)183, (byte)169, (byte)49,
            (byte)209, (byte)23, (byte)4, (byte)215,
            (byte)20, (byte)88, (byte)58, (byte)97,
            (byte)222, (byte)27, (byte)17, (byte)28,
            (byte)50, (byte)15, (byte)156, (byte)22,
            (byte)83, (byte)24, (byte)242, (byte)34,
            (byte)254, (byte)68, (byte)207, (byte)178,
            (byte)195, (byte)181, (byte)122, (byte)145,
            (byte)36, (byte)8, (byte)232, (byte)168,
            (byte)96, (byte)252, (byte)105, (byte)80,
            (byte)170, (byte)208, (byte)160, (byte)125,
            (byte)161, (byte)137, (byte)98, (byte)151,
            (byte)84, (byte)91, (byte)30, (byte)149,
            (byte)224, (byte)255, (byte)100, (byte)210,
            (byte)16, (byte)196, (byte)0, (byte)72,
            (byte)163, (byte)247, (byte)117, (byte)219,
            (byte)138, (byte)3, (byte)230, (byte)218,
            (byte)9, (byte)63, (byte)221, (byte)148,
            (byte)135, (byte)92, (byte)131, (byte)2,
            (byte)205, (byte)74, (byte)144, (byte)51,
            (byte)115, (byte)103, (byte)246, (byte)243,
            (byte)157, (byte)127, (byte)191, (byte)226,
            (byte)82, (byte)155, (byte)216, (byte)38,
            (byte)200, (byte)55, (byte)198, (byte)59,
            (byte)129, (byte)150, (byte)111, (byte)75,
            (byte)19, (byte)190, (byte)99, (byte)46,
            (byte)233, (byte)121, (byte)167, (byte)140,
            (byte)159, (byte)110, (byte)188, (byte)142,
            (byte)41, (byte)245, (byte)249, (byte)182,
            (byte)47, (byte)253, (byte)180, (byte)89,
            (byte)120, (byte)152, (byte)6, (byte)106,
            (byte)231, (byte)70, (byte)113, (byte)186,
            (byte)212, (byte)37, (byte)171, (byte)66,
            (byte)136, (byte)162, (byte)141, (byte)250,
            (byte)114, (byte)7, (byte)185, (byte)85,
            (byte)248, (byte)238, (byte)172, (byte)10,
            (byte)54, (byte)73, (byte)42, (byte)104,
            (byte)60, (byte)56, (byte)241, (byte)164,
            (byte)64, (byte)40, (byte)211, (byte)123,
            (byte)187, (byte)201, (byte)67, (byte)193,
            (byte)21, (byte)227, (byte)173, (byte)244,
            (byte)119, (byte)199, (byte)128, (byte)158
        };

//		private const int MASK8 = 0xff;
        private bool _initialised;
        private uint[] _ke = new uint[6 * 2]; // for FL and FL^(-1)
        private bool _keyis128;
        private uint[] _kw = new uint[4 * 2]; // for whitening
        private uint[] _state = new uint[4]; // for encryption and decryption

        private uint[] _subkey = new uint[24 * 4];

        public CamelliaLightEngine()
        {
            _initialised = false;
        }

        public virtual string AlgorithmName
        {
            get { return "Camellia"; }
        }

        public virtual bool IsPartialBlockOkay
        {
            get { return false; }
        }

        public virtual int GetBlockSize()
        {
            return BlockSize;
        }

        public virtual void Init(
            bool forEncryption,
            ICipherParameters parameters)
        {
            if (!(parameters is KeyParameter))
                throw new ArgumentException("only simple KeyParameter expected.");

            SetKey(forEncryption, ((KeyParameter)parameters).GetKey());

            _initialised = true;
        }

        public virtual int ProcessBlock(
            byte[] input,
            int inOff,
            byte[] output,
            int outOff)
        {
            if (!_initialised)
                throw new InvalidOperationException("Camellia engine not initialised");

            Check.DataLength(input, inOff, BlockSize, "input buffer too short");
            Check.OutputLength(output, outOff, BlockSize, "output buffer too short");

            if (_keyis128)
            {
                return ProcessBlock128(input, inOff, output, outOff);
            }
            else
            {
                return ProcessBlock192Or256(input, inOff, output, outOff);
            }
        }

        public virtual void Reset()
        {
        }

        private static uint RightRotate(uint x, int s)
        {
            return ((x >> s) + (x << (32 - s)));
        }

        private static uint LeftRotate(uint x, int s)
        {
            return (x << s) + (x >> (32 - s));
        }

        private static void Roldq(int rot, uint[] ki, int ioff, uint[] ko, int ooff)
        {
            ko[0 + ooff] = (ki[0 + ioff] << rot) | (ki[1 + ioff] >> (32 - rot));
            ko[1 + ooff] = (ki[1 + ioff] << rot) | (ki[2 + ioff] >> (32 - rot));
            ko[2 + ooff] = (ki[2 + ioff] << rot) | (ki[3 + ioff] >> (32 - rot));
            ko[3 + ooff] = (ki[3 + ioff] << rot) | (ki[0 + ioff] >> (32 - rot));
            ki[0 + ioff] = ko[0 + ooff];
            ki[1 + ioff] = ko[1 + ooff];
            ki[2 + ioff] = ko[2 + ooff];
            ki[3 + ioff] = ko[3 + ooff];
        }

        private static void Decroldq(int rot, uint[] ki, int ioff, uint[] ko, int ooff)
        {
            ko[2 + ooff] = (ki[0 + ioff] << rot) | (ki[1 + ioff] >> (32 - rot));
            ko[3 + ooff] = (ki[1 + ioff] << rot) | (ki[2 + ioff] >> (32 - rot));
            ko[0 + ooff] = (ki[2 + ioff] << rot) | (ki[3 + ioff] >> (32 - rot));
            ko[1 + ooff] = (ki[3 + ioff] << rot) | (ki[0 + ioff] >> (32 - rot));
            ki[0 + ioff] = ko[2 + ooff];
            ki[1 + ioff] = ko[3 + ooff];
            ki[2 + ioff] = ko[0 + ooff];
            ki[3 + ioff] = ko[1 + ooff];
        }

        private static void Roldqo32(int rot, uint[] ki, int ioff, uint[] ko, int ooff)
        {
            ko[0 + ooff] = (ki[1 + ioff] << (rot - 32)) | (ki[2 + ioff] >> (64 - rot));
            ko[1 + ooff] = (ki[2 + ioff] << (rot - 32)) | (ki[3 + ioff] >> (64 - rot));
            ko[2 + ooff] = (ki[3 + ioff] << (rot - 32)) | (ki[0 + ioff] >> (64 - rot));
            ko[3 + ooff] = (ki[0 + ioff] << (rot - 32)) | (ki[1 + ioff] >> (64 - rot));
            ki[0 + ioff] = ko[0 + ooff];
            ki[1 + ioff] = ko[1 + ooff];
            ki[2 + ioff] = ko[2 + ooff];
            ki[3 + ioff] = ko[3 + ooff];
        }

        private static void Decroldqo32(int rot, uint[] ki, int ioff, uint[] ko, int ooff)
        {
            ko[2 + ooff] = (ki[1 + ioff] << (rot - 32)) | (ki[2 + ioff] >> (64 - rot));
            ko[3 + ooff] = (ki[2 + ioff] << (rot - 32)) | (ki[3 + ioff] >> (64 - rot));
            ko[0 + ooff] = (ki[3 + ioff] << (rot - 32)) | (ki[0 + ioff] >> (64 - rot));
            ko[1 + ooff] = (ki[0 + ioff] << (rot - 32)) | (ki[1 + ioff] >> (64 - rot));
            ki[0 + ioff] = ko[2 + ooff];
            ki[1 + ioff] = ko[3 + ooff];
            ki[2 + ioff] = ko[0 + ooff];
            ki[3 + ioff] = ko[1 + ooff];
        }

        private static uint Bytes2Uint(byte[] src, int offset)
        {
            uint word = 0;
            for (int i = 0; i < 4; i++)
            {
                word = (word << 8) + (uint)src[i + offset];
            }

            return word;
        }

        private static void Uint2Bytes(uint word, byte[] dst, int offset)
        {
            for (int i = 0; i < 4; i++)
            {
                dst[(3 - i) + offset] = (byte)word;
                word >>= 8;
            }
        }

        private byte LRot8(byte v, int rot)
        {
            return (byte)(((uint)v << rot) | ((uint)v >> (8 - rot)));
        }

        private uint Sbox2(int x)
        {
            return (uint)LRot8(Sbox1[x], 1);
        }

        private uint Sbox3(int x)
        {
            return (uint)LRot8(Sbox1[x], 7);
        }

        private uint Sbox4(int x)
        {
            return (uint)Sbox1[LRot8((byte)x, 1)];
        }

        private void CamelliaF2(uint[] s, uint[] skey, int keyoff)
        {
            uint t1, t2, u, v;

            t1 = s[0] ^ skey[0 + keyoff];
            u = Sbox4((byte)t1);
            u |= (Sbox3((byte)(t1 >> 8)) << 8);
            u |= (Sbox2((byte)(t1 >> 16)) << 16);
            u |= ((uint)(Sbox1[(byte)(t1 >> 24)]) << 24);

            t2 = s[1] ^ skey[1 + keyoff];
            v = (uint)Sbox1[(byte)t2];
            v |= (Sbox4((byte)(t2 >> 8)) << 8);
            v |= (Sbox3((byte)(t2 >> 16)) << 16);
            v |= (Sbox2((byte)(t2 >> 24)) << 24);

            v = LeftRotate(v, 8);
            u ^= v;
            v = LeftRotate(v, 8) ^ u;
            u = RightRotate(u, 8) ^ v;
            s[2] ^= LeftRotate(v, 16) ^ u;
            s[3] ^= LeftRotate(u, 8);

            t1 = s[2] ^ skey[2 + keyoff];
            u = Sbox4((byte)t1);
            u |= Sbox3((byte)(t1 >> 8)) << 8;
            u |= Sbox2((byte)(t1 >> 16)) << 16;
            u |= ((uint)Sbox1[(byte)(t1 >> 24)]) << 24;

            t2 = s[3] ^ skey[3 + keyoff];
            v = (uint)Sbox1[(byte)t2];
            v |= Sbox4((byte)(t2 >> 8)) << 8;
            v |= Sbox3((byte)(t2 >> 16)) << 16;
            v |= Sbox2((byte)(t2 >> 24)) << 24;

            v = LeftRotate(v, 8);
            u ^= v;
            v = LeftRotate(v, 8) ^ u;
            u = RightRotate(u, 8) ^ v;
            s[0] ^= LeftRotate(v, 16) ^ u;
            s[1] ^= LeftRotate(u, 8);
        }

        private void CamelliaFLs(uint[] s, uint[] fkey, int keyoff)
        {
            s[1] ^= LeftRotate(s[0] & fkey[0 + keyoff], 1);
            s[0] ^= fkey[1 + keyoff] | s[1];

            s[2] ^= fkey[3 + keyoff] | s[3];
            s[3] ^= LeftRotate(fkey[2 + keyoff] & s[2], 1);
        }

        private void SetKey(bool forEncryption, byte[] key)
        {
            uint[] k = new uint[8];
            uint[] ka = new uint[4];
            uint[] kb = new uint[4];
            uint[] t = new uint[4];

            switch (key.Length)
            {
                case 16:
                    _keyis128 = true;
                    k[0] = Bytes2Uint(key, 0);
                    k[1] = Bytes2Uint(key, 4);
                    k[2] = Bytes2Uint(key, 8);
                    k[3] = Bytes2Uint(key, 12);
                    k[4] = k[5] = k[6] = k[7] = 0;
                    break;
                case 24:
                    k[0] = Bytes2Uint(key, 0);
                    k[1] = Bytes2Uint(key, 4);
                    k[2] = Bytes2Uint(key, 8);
                    k[3] = Bytes2Uint(key, 12);
                    k[4] = Bytes2Uint(key, 16);
                    k[5] = Bytes2Uint(key, 20);
                    k[6] = ~k[4];
                    k[7] = ~k[5];
                    _keyis128 = false;
                    break;
                case 32:
                    k[0] = Bytes2Uint(key, 0);
                    k[1] = Bytes2Uint(key, 4);
                    k[2] = Bytes2Uint(key, 8);
                    k[3] = Bytes2Uint(key, 12);
                    k[4] = Bytes2Uint(key, 16);
                    k[5] = Bytes2Uint(key, 20);
                    k[6] = Bytes2Uint(key, 24);
                    k[7] = Bytes2Uint(key, 28);
                    _keyis128 = false;
                    break;
                default:
                    throw new ArgumentException("key sizes are only 16/24/32 bytes.");
            }

            for (int i = 0; i < 4; i++)
            {
                ka[i] = k[i] ^ k[i + 4];
            }

            /* compute KA */
            CamelliaF2(ka, Sigma, 0);
            for (int i = 0; i < 4; i++)
            {
                ka[i] ^= k[i];
            }

            CamelliaF2(ka, Sigma, 4);

            if (_keyis128)
            {
                if (forEncryption)
                {
                    /* KL dependant keys */
                    _kw[0] = k[0];
                    _kw[1] = k[1];
                    _kw[2] = k[2];
                    _kw[3] = k[3];
                    Roldq(15, k, 0, _subkey, 4);
                    Roldq(30, k, 0, _subkey, 12);
                    Roldq(15, k, 0, t, 0);
                    _subkey[18] = t[2];
                    _subkey[19] = t[3];
                    Roldq(17, k, 0, _ke, 4);
                    Roldq(17, k, 0, _subkey, 24);
                    Roldq(17, k, 0, _subkey, 32);
                    /* KA dependant keys */
                    _subkey[0] = ka[0];
                    _subkey[1] = ka[1];
                    _subkey[2] = ka[2];
                    _subkey[3] = ka[3];
                    Roldq(15, ka, 0, _subkey, 8);
                    Roldq(15, ka, 0, _ke, 0);
                    Roldq(15, ka, 0, t, 0);
                    _subkey[16] = t[0];
                    _subkey[17] = t[1];
                    Roldq(15, ka, 0, _subkey, 20);
                    Roldqo32(34, ka, 0, _subkey, 28);
                    Roldq(17, ka, 0, _kw, 4);
                }
                else
                {
                    // decryption
                    /* KL dependant keys */
                    _kw[4] = k[0];
                    _kw[5] = k[1];
                    _kw[6] = k[2];
                    _kw[7] = k[3];
                    Decroldq(15, k, 0, _subkey, 28);
                    Decroldq(30, k, 0, _subkey, 20);
                    Decroldq(15, k, 0, t, 0);
                    _subkey[16] = t[0];
                    _subkey[17] = t[1];
                    Decroldq(17, k, 0, _ke, 0);
                    Decroldq(17, k, 0, _subkey, 8);
                    Decroldq(17, k, 0, _subkey, 0);
                    /* KA dependant keys */
                    _subkey[34] = ka[0];
                    _subkey[35] = ka[1];
                    _subkey[32] = ka[2];
                    _subkey[33] = ka[3];
                    Decroldq(15, ka, 0, _subkey, 24);
                    Decroldq(15, ka, 0, _ke, 4);
                    Decroldq(15, ka, 0, t, 0);
                    _subkey[18] = t[2];
                    _subkey[19] = t[3];
                    Decroldq(15, ka, 0, _subkey, 12);
                    Decroldqo32(34, ka, 0, _subkey, 4);
                    Roldq(17, ka, 0, _kw, 0);
                }
            }
            else
            {
                // 192bit or 256bit
                /* compute KB */
                for (int i = 0; i < 4; i++)
                {
                    kb[i] = ka[i] ^ k[i + 4];
                }

                CamelliaF2(kb, Sigma, 8);

                if (forEncryption)
                {
                    /* KL dependant keys */
                    _kw[0] = k[0];
                    _kw[1] = k[1];
                    _kw[2] = k[2];
                    _kw[3] = k[3];
                    Roldqo32(45, k, 0, _subkey, 16);
                    Roldq(15, k, 0, _ke, 4);
                    Roldq(17, k, 0, _subkey, 32);
                    Roldqo32(34, k, 0, _subkey, 44);
                    /* KR dependant keys */
                    Roldq(15, k, 4, _subkey, 4);
                    Roldq(15, k, 4, _ke, 0);
                    Roldq(30, k, 4, _subkey, 24);
                    Roldqo32(34, k, 4, _subkey, 36);
                    /* KA dependant keys */
                    Roldq(15, ka, 0, _subkey, 8);
                    Roldq(30, ka, 0, _subkey, 20);
                    /* 32bit rotation */
                    _ke[8] = ka[1];
                    _ke[9] = ka[2];
                    _ke[10] = ka[3];
                    _ke[11] = ka[0];
                    Roldqo32(49, ka, 0, _subkey, 40);

                    /* KB dependant keys */
                    _subkey[0] = kb[0];
                    _subkey[1] = kb[1];
                    _subkey[2] = kb[2];
                    _subkey[3] = kb[3];
                    Roldq(30, kb, 0, _subkey, 12);
                    Roldq(30, kb, 0, _subkey, 28);
                    Roldqo32(51, kb, 0, _kw, 4);
                }
                else
                {
                    // decryption
                    /* KL dependant keys */
                    _kw[4] = k[0];
                    _kw[5] = k[1];
                    _kw[6] = k[2];
                    _kw[7] = k[3];
                    Decroldqo32(45, k, 0, _subkey, 28);
                    Decroldq(15, k, 0, _ke, 4);
                    Decroldq(17, k, 0, _subkey, 12);
                    Decroldqo32(34, k, 0, _subkey, 0);
                    /* KR dependant keys */
                    Decroldq(15, k, 4, _subkey, 40);
                    Decroldq(15, k, 4, _ke, 8);
                    Decroldq(30, k, 4, _subkey, 20);
                    Decroldqo32(34, k, 4, _subkey, 8);
                    /* KA dependant keys */
                    Decroldq(15, ka, 0, _subkey, 36);
                    Decroldq(30, ka, 0, _subkey, 24);
                    /* 32bit rotation */
                    _ke[2] = ka[1];
                    _ke[3] = ka[2];
                    _ke[0] = ka[3];
                    _ke[1] = ka[0];
                    Decroldqo32(49, ka, 0, _subkey, 4);

                    /* KB dependant keys */
                    _subkey[46] = kb[0];
                    _subkey[47] = kb[1];
                    _subkey[44] = kb[2];
                    _subkey[45] = kb[3];
                    Decroldq(30, kb, 0, _subkey, 32);
                    Decroldq(30, kb, 0, _subkey, 16);
                    Roldqo32(51, kb, 0, _kw, 0);
                }
            }
        }

        private int ProcessBlock128(byte[] input, int inOff, byte[] output, int outOff)
        {
            for (int i = 0; i < 4; i++)
            {
                _state[i] = Bytes2Uint(input, inOff + (i * 4));
                _state[i] ^= _kw[i];
            }

            CamelliaF2(_state, _subkey, 0);
            CamelliaF2(_state, _subkey, 4);
            CamelliaF2(_state, _subkey, 8);
            CamelliaFLs(_state, _ke, 0);
            CamelliaF2(_state, _subkey, 12);
            CamelliaF2(_state, _subkey, 16);
            CamelliaF2(_state, _subkey, 20);
            CamelliaFLs(_state, _ke, 4);
            CamelliaF2(_state, _subkey, 24);
            CamelliaF2(_state, _subkey, 28);
            CamelliaF2(_state, _subkey, 32);

            _state[2] ^= _kw[4];
            _state[3] ^= _kw[5];
            _state[0] ^= _kw[6];
            _state[1] ^= _kw[7];

            Uint2Bytes(_state[2], output, outOff);
            Uint2Bytes(_state[3], output, outOff + 4);
            Uint2Bytes(_state[0], output, outOff + 8);
            Uint2Bytes(_state[1], output, outOff + 12);

            return BlockSize;
        }

        private int ProcessBlock192Or256(byte[] input, int inOff, byte[] output, int outOff)
        {
            for (int i = 0; i < 4; i++)
            {
                _state[i] = Bytes2Uint(input, inOff + (i * 4));
                _state[i] ^= _kw[i];
            }

            CamelliaF2(_state, _subkey, 0);
            CamelliaF2(_state, _subkey, 4);
            CamelliaF2(_state, _subkey, 8);
            CamelliaFLs(_state, _ke, 0);
            CamelliaF2(_state, _subkey, 12);
            CamelliaF2(_state, _subkey, 16);
            CamelliaF2(_state, _subkey, 20);
            CamelliaFLs(_state, _ke, 4);
            CamelliaF2(_state, _subkey, 24);
            CamelliaF2(_state, _subkey, 28);
            CamelliaF2(_state, _subkey, 32);
            CamelliaFLs(_state, _ke, 8);
            CamelliaF2(_state, _subkey, 36);
            CamelliaF2(_state, _subkey, 40);
            CamelliaF2(_state, _subkey, 44);

            _state[2] ^= _kw[4];
            _state[3] ^= _kw[5];
            _state[0] ^= _kw[6];
            _state[1] ^= _kw[7];

            Uint2Bytes(_state[2], output, outOff);
            Uint2Bytes(_state[3], output, outOff + 4);
            Uint2Bytes(_state[0], output, outOff + 8);
            Uint2Bytes(_state[1], output, outOff + 12);
            return BlockSize;
        }
    }
}
#pragma warning restore
#endif
#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /// <summary>
    /// Implementation of the Threefish tweakable large block cipher in 256, 512 and 1024 bit block
    /// sizes.
    /// </summary>
    /// <remarks>
    /// This is the 1.3 version of Threefish defined in the Skein hash function submission to the NIST
    /// SHA-3 competition in October 2010.
    /// <p/>
    /// Threefish was designed by Niels Ferguson - Stefan Lucks - Bruce Schneier - Doug Whiting - Mihir
    /// Bellare - Tadayoshi Kohno - Jon Callas - Jesse Walker.
    /// <p/>
    /// This implementation inlines all round functions, unrolls 8 rounds, and uses 1.2k of static tables
    /// to speed up key schedule injection. <br/>
    /// 2 x block size state is retained by each cipher instance.
    /// </remarks>
    public class ThreefishEngine
        : IBlockCipher
    {
        /// <summary>
        /// 256 bit block size - Threefish-256
        /// </summary>
        public const int Blocksize256 = 256;

        /// <summary>
        /// 512 bit block size - Threefish-512
        /// </summary>
        public const int Blocksize512 = 512;

        /// <summary>
        /// 1024 bit block size - Threefish-1024
        /// </summary>
        public const int Blocksize1024 = 1024;

        /**
	     * Size of the tweak in bytes (always 128 bit/16 bytes)
	     */
        private const int TweakSizeBytes = 16;

        private const int TweakSizeWords = TweakSizeBytes / 8;

        /**
	     * Rounds in Threefish-256
	     */
        private const int Rounds256 = 72;

        /**
	     * Rounds in Threefish-512
	     */
        private const int Rounds512 = 72;

        /**
	     * Rounds in Threefish-1024
	     */
        private const int Rounds1024 = 80;

        /**
	     * Max rounds of any of the variants
	     */
        private const int MaxRounds = Rounds1024;

        /**
	     * Key schedule parity constant
	     */
        private const ulong C240 = 0x1BD11BDAA9FC1A22L;

        /* Pre-calculated modulo arithmetic tables for key schedule lookups */
        private static readonly int[] Mod9 = new int[MaxRounds];
        private static readonly int[] Mod17 = new int[Mod9.Length];
        private static readonly int[] Mod5 = new int[Mod9.Length];
        private static readonly int[] Mod3 = new int[Mod9.Length];

        /**
	     * Block size in bytes
	     */
        private readonly int _blocksizeBytes;

        /**
	     * Block size in 64 bit words
	     */
        private readonly int _blocksizeWords;

        /**
	     * The internal cipher implementation (varies by blocksize)
	     */
        private readonly ThreefishCipher _cipher;

        /**
	     * Buffer for byte oriented processBytes to call internal word API
	     */
        private readonly ulong[] _currentBlock;

        /**
	     * Key schedule words
	     */
        private readonly ulong[] _kw;

        /**
	     * Tweak bytes (2 byte t1,t2, calculated t3 and repeat of t1,t2 for modulo free lookup
	     */
        private readonly ulong[] _t = new ulong[5];

        private bool _forEncryption;

        static ThreefishEngine()
        {
            for (int i = 0; i < Mod9.Length; i++)
            {
                Mod17[i] = i % 17;
                Mod9[i] = i % 9;
                Mod5[i] = i % 5;
                Mod3[i] = i % 3;
            }
        }

        /// <summary>
        /// Constructs a new Threefish cipher, with a specified block size.
        /// </summary>
        /// <param name="blocksizeBits">the block size in bits, one of <see cref="Blocksize256"/>, <see cref="Blocksize512"/>,
        ///                      <see cref="Blocksize1024"/> .</param>
        public ThreefishEngine(int blocksizeBits)
        {
            this._blocksizeBytes = (blocksizeBits / 8);
            this._blocksizeWords = (this._blocksizeBytes / 8);
            this._currentBlock = new ulong[_blocksizeWords];

            /*
             * Provide room for original key words, extended key word and repeat of key words for modulo
             * free lookup of key schedule words.
             */
            this._kw = new ulong[2 * _blocksizeWords + 1];

            switch (blocksizeBits)
            {
                case Blocksize256:
                    _cipher = new Threefish256Cipher(_kw, _t);
                    break;
                case Blocksize512:
                    _cipher = new Threefish512Cipher(_kw, _t);
                    break;
                case Blocksize1024:
                    _cipher = new Threefish1024Cipher(_kw, _t);
                    break;
                default:
                    throw new ArgumentException(
                        "Invalid blocksize - Threefish is defined with block size of 256, 512, or 1024 bits");
            }
        }

        /// <summary>
        /// Initialise the engine.
        /// </summary>
        /// <param name="forEncryption">Initialise for encryption if true, for decryption if false.</param>
        /// <param name="parameters">an instance of <see cref="TweakableBlockCipherParameters"/> or <see cref="KeyParameter"/> (to
        ///               use a 0 tweak)</param>
        public virtual void Init(bool forEncryption, ICipherParameters parameters)
        {
            byte[] keyBytes;
            byte[] tweakBytes;

            if (parameters is TweakableBlockCipherParameters)
            {
                TweakableBlockCipherParameters tParams = (TweakableBlockCipherParameters)parameters;
                keyBytes = tParams.Key.GetKey();
                tweakBytes = tParams.Tweak;
            }
            else if (parameters is KeyParameter)
            {
                keyBytes = ((KeyParameter)parameters).GetKey();
                tweakBytes = null;
            }
            else
            {
                throw new ArgumentException("Invalid parameter passed to Threefish init - "
                                            + BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(
                                                parameters));
            }

            ulong[] keyWords = null;
            ulong[] tweakWords = null;

            if (keyBytes != null)
            {
                if (keyBytes.Length != this._blocksizeBytes)
                {
                    throw new ArgumentException("Threefish key must be same size as block (" + _blocksizeBytes
                        + " bytes)");
                }

                keyWords = new ulong[_blocksizeWords];
                for (int i = 0; i < keyWords.Length; i++)
                {
                    keyWords[i] = BytesToWord(keyBytes, i * 8);
                }
            }

            if (tweakBytes != null)
            {
                if (tweakBytes.Length != TweakSizeBytes)
                {
                    throw new ArgumentException("Threefish tweak must be " + TweakSizeBytes + " bytes");
                }

                tweakWords = new ulong[] { BytesToWord(tweakBytes, 0), BytesToWord(tweakBytes, 8) };
            }

            Init(forEncryption, keyWords, tweakWords);
        }

        public virtual string AlgorithmName
        {
            get { return "Threefish-" + (_blocksizeBytes * 8); }
        }

        public virtual bool IsPartialBlockOkay
        {
            get { return false; }
        }

        public virtual int GetBlockSize()
        {
            return _blocksizeBytes;
        }

        public virtual void Reset()
        {
        }

        public virtual int ProcessBlock(byte[] inBytes, int inOff, byte[] outBytes, int outOff)
        {
            if ((outOff + _blocksizeBytes) > outBytes.Length)
            {
                throw new DataLengthException("Output buffer too short");
            }

            if ((inOff + _blocksizeBytes) > inBytes.Length)
            {
                throw new DataLengthException("Input buffer too short");
            }

            for (int i = 0; i < _blocksizeBytes; i += 8)
            {
                _currentBlock[i >> 3] = BytesToWord(inBytes, inOff + i);
            }

            ProcessBlock(this._currentBlock, this._currentBlock);
            for (int i = 0; i < _blocksizeBytes; i += 8)
            {
                WordToBytes(this._currentBlock[i >> 3], outBytes, outOff + i);
            }

            return _blocksizeBytes;
        }

        /// <summary>
        /// Initialise the engine, specifying the key and tweak directly.
        /// </summary>
        /// <param name="forEncryption">the cipher mode.</param>
        /// <param name="key">the words of the key, or <code>null</code> to use the current key.</param>
        /// <param name="tweak">the 2 word (128 bit) tweak, or <code>null</code> to use the current tweak.</param>
        internal void Init(bool forEncryption, ulong[] key, ulong[] tweak)
        {
            this._forEncryption = forEncryption;
            if (key != null)
            {
                SetKey(key);
            }

            if (tweak != null)
            {
                SetTweak(tweak);
            }
        }

        private void SetKey(ulong[] key)
        {
            if (key.Length != this._blocksizeWords)
            {
                throw new ArgumentException("Threefish key must be same size as block (" + _blocksizeWords
                    + " words)");
            }

            /*
             * Full subkey schedule is deferred to execution to avoid per cipher overhead (10k for 512,
             * 20k for 1024).
             * 
             * Key and tweak word sequences are repeated, and static MOD17/MOD9/MOD5/MOD3 calculations
             * used, to avoid expensive mod computations during cipher operation.
             */

            ulong knw = C240;
            for (int i = 0; i < _blocksizeWords; i++)
            {
                _kw[i] = key[i];
                knw = knw ^ _kw[i];
            }

            _kw[_blocksizeWords] = knw;
            Array.Copy(_kw, 0, _kw, _blocksizeWords + 1, _blocksizeWords);
        }

        private void SetTweak(ulong[] tweak)
        {
            if (tweak.Length != TweakSizeWords)
            {
                throw new ArgumentException("Tweak must be " + TweakSizeWords + " words.");
            }

            /*
             * Tweak schedule partially repeated to avoid mod computations during cipher operation
             */
            _t[0] = tweak[0];
            _t[1] = tweak[1];
            _t[2] = _t[0] ^ _t[1];
            _t[3] = _t[0];
            _t[4] = _t[1];
        }

        /// <summary>
        /// Process a block of data represented as 64 bit words.
        /// </summary>
        /// <returns>the number of 8 byte words processed (which will be the same as the block size).</returns>
        /// <param name="inWords">a block sized buffer of words to process.</param>
        /// <param name="outWords">a block sized buffer of words to receive the output of the operation.</param>
        /// <exception cref="DataLengthException">if either the input or output is not block sized</exception>
        /// <exception cref="InvalidOperationException">if this engine is not initialised</exception>
        internal int ProcessBlock(ulong[] inWords, ulong[] outWords)
        {
            if (_kw[_blocksizeWords] == 0)
            {
                throw new InvalidOperationException("Threefish engine not initialised");
            }

            if (inWords.Length != _blocksizeWords)
            {
                throw new DataLengthException("Input buffer too short");
            }

            if (outWords.Length != _blocksizeWords)
            {
                throw new DataLengthException("Output buffer too short");
            }

            if (_forEncryption)
            {
                _cipher.EncryptBlock(inWords, outWords);
            }
            else
            {
                _cipher.DecryptBlock(inWords, outWords);
            }

            return _blocksizeWords;
        }

        /// <summary>
        /// Read a single 64 bit word from input in LSB first order.
        /// </summary>
        internal static ulong BytesToWord(byte[] bytes, int off)
        {
            if ((off + 8) > bytes.Length)
            {
                // Help the JIT avoid index checks
                throw new ArgumentException();
            }

            ulong word = 0;
            int index = off;

            word = (bytes[index++] & 0xffUL);
            word |= (bytes[index++] & 0xffUL) << 8;
            word |= (bytes[index++] & 0xffUL) << 16;
            word |= (bytes[index++] & 0xffUL) << 24;
            word |= (bytes[index++] & 0xffUL) << 32;
            word |= (bytes[index++] & 0xffUL) << 40;
            word |= (bytes[index++] & 0xffUL) << 48;
            word |= (bytes[index++] & 0xffUL) << 56;

            return word;
        }

        /// <summary>
        /// Write a 64 bit word to output in LSB first order.
        /// </summary>
        internal static void WordToBytes(ulong word, byte[] bytes, int off)
        {
            if ((off + 8) > bytes.Length)
            {
                // Help the JIT avoid index checks
                throw new ArgumentException();
            }

            int index = off;

            bytes[index++] = (byte)word;
            bytes[index++] = (byte)(word >> 8);
            bytes[index++] = (byte)(word >> 16);
            bytes[index++] = (byte)(word >> 24);
            bytes[index++] = (byte)(word >> 32);
            bytes[index++] = (byte)(word >> 40);
            bytes[index++] = (byte)(word >> 48);
            bytes[index++] = (byte)(word >> 56);
        }

        /**
	     * Rotate left + xor part of the mix operation.
	     */
        private static ulong RotlXor(ulong x, int n, ulong xor)
        {
            return ((x << n) | (x >> (64 - n))) ^ xor;
        }

        /**
	     * Rotate xor + rotate right part of the unmix operation.
	     */
        private static ulong XorRotr(ulong x, int n, ulong xor)
        {
            ulong xored = x ^ xor;
            return (xored >> n) | (xored << (64 - n));
        }

        private abstract class ThreefishCipher
        {
            /**
	         * The extended + repeated key words
	         */
            protected readonly ulong[] Kw;

            /**
	         * The extended + repeated tweak words
	         */
            protected readonly ulong[] T;

            protected ThreefishCipher(ulong[] kw, ulong[] t)
            {
                this.Kw = kw;
                this.T = t;
            }

            internal abstract void EncryptBlock(ulong[] block, ulong[] outWords);

            internal abstract void DecryptBlock(ulong[] block, ulong[] outWords);
        }

        private sealed class Threefish256Cipher
            : ThreefishCipher
        {
            /**
	         * Mix rotation constants defined in Skein 1.3 specification
	         */
            private const int Rotation00 = 14, Rotation01 = 16;

            private const int Rotation10 = 52, Rotation11 = 57;
            private const int Rotation20 = 23, Rotation21 = 40;
            private const int Rotation30 = 5, Rotation31 = 37;

            private const int Rotation40 = 25, Rotation41 = 33;
            private const int Rotation50 = 46, Rotation51 = 12;
            private const int Rotation60 = 58, Rotation61 = 22;
            private const int Rotation70 = 32, Rotation71 = 32;

            public Threefish256Cipher(ulong[] kw, ulong[] t)
                : base(kw, t)
            {
            }

            internal override void EncryptBlock(ulong[] block, ulong[] outWords)
            {
                ulong[] kw = this.Kw;
                ulong[] t = this.T;
                int[] mod5 = Mod5;
                int[] mod3 = Mod3;

                /* Help the JIT avoid index bounds checks */
                if (kw.Length != 9)
                {
                    throw new ArgumentException();
                }

                if (t.Length != 5)
                {
                    throw new ArgumentException();
                }

                /*
                 * Read 4 words of plaintext data, not using arrays for cipher state
                 */
                ulong b0 = block[0];
                ulong b1 = block[1];
                ulong b2 = block[2];
                ulong b3 = block[3];

                /*
                 * First subkey injection.
                 */
                b0 += kw[0];
                b1 += kw[1] + t[0];
                b2 += kw[2] + t[1];
                b3 += kw[3];

                /*
                 * Rounds loop, unrolled to 8 rounds per iteration.
                 * 
                 * Unrolling to multiples of 4 avoids the mod 4 check for key injection, and allows
                 * inlining of the permutations, which cycle every of 2 rounds (avoiding array
                 * index/lookup).
                 * 
                 * Unrolling to multiples of 8 avoids the mod 8 rotation constant lookup, and allows
                 * inlining constant rotation values (avoiding array index/lookup).
                 */

                for (int d = 1; d < (Rounds256 / 4); d += 2)
                {
                    int dm5 = mod5[d];
                    int dm3 = mod3[d];

                    /*
                     * 4 rounds of mix and permute.
                     * 
                     * Permute schedule has a 2 round cycle, so permutes are inlined in the mix
                     * operations in each 4 round block.
                     */
                    b1 = RotlXor(b1, Rotation00, b0 += b1);
                    b3 = RotlXor(b3, Rotation01, b2 += b3);

                    b3 = RotlXor(b3, Rotation10, b0 += b3);
                    b1 = RotlXor(b1, Rotation11, b2 += b1);

                    b1 = RotlXor(b1, Rotation20, b0 += b1);
                    b3 = RotlXor(b3, Rotation21, b2 += b3);

                    b3 = RotlXor(b3, Rotation30, b0 += b3);
                    b1 = RotlXor(b1, Rotation31, b2 += b1);

                    /*
                     * Subkey injection for first 4 rounds.
                     */
                    b0 += kw[dm5];
                    b1 += kw[dm5 + 1] + t[dm3];
                    b2 += kw[dm5 + 2] + t[dm3 + 1];
                    b3 += kw[dm5 + 3] + (uint)d;

                    /*
                     * 4 more rounds of mix/permute
                     */
                    b1 = RotlXor(b1, Rotation40, b0 += b1);
                    b3 = RotlXor(b3, Rotation41, b2 += b3);

                    b3 = RotlXor(b3, Rotation50, b0 += b3);
                    b1 = RotlXor(b1, Rotation51, b2 += b1);

                    b1 = RotlXor(b1, Rotation60, b0 += b1);
                    b3 = RotlXor(b3, Rotation61, b2 += b3);

                    b3 = RotlXor(b3, Rotation70, b0 += b3);
                    b1 = RotlXor(b1, Rotation71, b2 += b1);

                    /*
                     * Subkey injection for next 4 rounds.
                     */
                    b0 += kw[dm5 + 1];
                    b1 += kw[dm5 + 2] + t[dm3 + 1];
                    b2 += kw[dm5 + 3] + t[dm3 + 2];
                    b3 += kw[dm5 + 4] + (uint)d + 1;
                }

                /*
                 * Output cipher state.
                 */
                outWords[0] = b0;
                outWords[1] = b1;
                outWords[2] = b2;
                outWords[3] = b3;
            }

            internal override void DecryptBlock(ulong[] block, ulong[] state)
            {
                ulong[] kw = this.Kw;
                ulong[] t = this.T;
                int[] mod5 = Mod5;
                int[] mod3 = Mod3;

                /* Help the JIT avoid index bounds checks */
                if (kw.Length != 9)
                {
                    throw new ArgumentException();
                }

                if (t.Length != 5)
                {
                    throw new ArgumentException();
                }

                ulong b0 = block[0];
                ulong b1 = block[1];
                ulong b2 = block[2];
                ulong b3 = block[3];

                for (int d = (Rounds256 / 4) - 1; d >= 1; d -= 2)
                {
                    int dm5 = mod5[d];
                    int dm3 = mod3[d];

                    /* Reverse key injection for second 4 rounds */
                    b0 -= kw[dm5 + 1];
                    b1 -= kw[dm5 + 2] + t[dm3 + 1];
                    b2 -= kw[dm5 + 3] + t[dm3 + 2];
                    b3 -= kw[dm5 + 4] + (uint)d + 1;

                    /* Reverse second 4 mix/permute rounds */

                    b3 = XorRotr(b3, Rotation70, b0);
                    b0 -= b3;
                    b1 = XorRotr(b1, Rotation71, b2);
                    b2 -= b1;

                    b1 = XorRotr(b1, Rotation60, b0);
                    b0 -= b1;
                    b3 = XorRotr(b3, Rotation61, b2);
                    b2 -= b3;

                    b3 = XorRotr(b3, Rotation50, b0);
                    b0 -= b3;
                    b1 = XorRotr(b1, Rotation51, b2);
                    b2 -= b1;

                    b1 = XorRotr(b1, Rotation40, b0);
                    b0 -= b1;
                    b3 = XorRotr(b3, Rotation41, b2);
                    b2 -= b3;

                    /* Reverse key injection for first 4 rounds */
                    b0 -= kw[dm5];
                    b1 -= kw[dm5 + 1] + t[dm3];
                    b2 -= kw[dm5 + 2] + t[dm3 + 1];
                    b3 -= kw[dm5 + 3] + (uint)d;

                    /* Reverse first 4 mix/permute rounds */
                    b3 = XorRotr(b3, Rotation30, b0);
                    b0 -= b3;
                    b1 = XorRotr(b1, Rotation31, b2);
                    b2 -= b1;

                    b1 = XorRotr(b1, Rotation20, b0);
                    b0 -= b1;
                    b3 = XorRotr(b3, Rotation21, b2);
                    b2 -= b3;

                    b3 = XorRotr(b3, Rotation10, b0);
                    b0 -= b3;
                    b1 = XorRotr(b1, Rotation11, b2);
                    b2 -= b1;

                    b1 = XorRotr(b1, Rotation00, b0);
                    b0 -= b1;
                    b3 = XorRotr(b3, Rotation01, b2);
                    b2 -= b3;
                }

                /*
                 * First subkey uninjection.
                 */
                b0 -= kw[0];
                b1 -= kw[1] + t[0];
                b2 -= kw[2] + t[1];
                b3 -= kw[3];

                /*
                 * Output cipher state.
                 */
                state[0] = b0;
                state[1] = b1;
                state[2] = b2;
                state[3] = b3;
            }
        }

        private sealed class Threefish512Cipher
            : ThreefishCipher
        {
            /**
	         * Mix rotation constants defined in Skein 1.3 specification
	         */
            private const int Rotation00 = 46, Rotation01 = 36, Rotation02 = 19, Rotation03 = 37;

            private const int Rotation10 = 33, Rotation11 = 27, Rotation12 = 14, Rotation13 = 42;
            private const int Rotation20 = 17, Rotation21 = 49, Rotation22 = 36, Rotation23 = 39;
            private const int Rotation30 = 44, Rotation31 = 9, Rotation32 = 54, Rotation33 = 56;

            private const int Rotation40 = 39, Rotation41 = 30, Rotation42 = 34, Rotation43 = 24;
            private const int Rotation50 = 13, Rotation51 = 50, Rotation52 = 10, Rotation53 = 17;
            private const int Rotation60 = 25, Rotation61 = 29, Rotation62 = 39, Rotation63 = 43;
            private const int Rotation70 = 8, Rotation71 = 35, Rotation72 = 56, Rotation73 = 22;

            internal Threefish512Cipher(ulong[] kw, ulong[] t)
                : base(kw, t)
            {
            }

            internal override void EncryptBlock(ulong[] block, ulong[] outWords)
            {
                ulong[] kw = this.Kw;
                ulong[] t = this.T;
                int[] mod9 = Mod9;
                int[] mod3 = Mod3;

                /* Help the JIT avoid index bounds checks */
                if (kw.Length != 17)
                {
                    throw new ArgumentException();
                }

                if (t.Length != 5)
                {
                    throw new ArgumentException();
                }

                /*
                 * Read 8 words of plaintext data, not using arrays for cipher state
                 */
                ulong b0 = block[0];
                ulong b1 = block[1];
                ulong b2 = block[2];
                ulong b3 = block[3];
                ulong b4 = block[4];
                ulong b5 = block[5];
                ulong b6 = block[6];
                ulong b7 = block[7];

                /*
                 * First subkey injection.
                 */
                b0 += kw[0];
                b1 += kw[1];
                b2 += kw[2];
                b3 += kw[3];
                b4 += kw[4];
                b5 += kw[5] + t[0];
                b6 += kw[6] + t[1];
                b7 += kw[7];

                /*
                 * Rounds loop, unrolled to 8 rounds per iteration.
                 * 
                 * Unrolling to multiples of 4 avoids the mod 4 check for key injection, and allows
                 * inlining of the permutations, which cycle every of 4 rounds (avoiding array
                 * index/lookup).
                 * 
                 * Unrolling to multiples of 8 avoids the mod 8 rotation constant lookup, and allows
                 * inlining constant rotation values (avoiding array index/lookup).
                 */

                for (int d = 1; d < (Rounds512 / 4); d += 2)
                {
                    int dm9 = mod9[d];
                    int dm3 = mod3[d];

                    /*
                     * 4 rounds of mix and permute.
                     * 
                     * Permute schedule has a 4 round cycle, so permutes are inlined in the mix
                     * operations in each 4 round block.
                     */
                    b1 = RotlXor(b1, Rotation00, b0 += b1);
                    b3 = RotlXor(b3, Rotation01, b2 += b3);
                    b5 = RotlXor(b5, Rotation02, b4 += b5);
                    b7 = RotlXor(b7, Rotation03, b6 += b7);

                    b1 = RotlXor(b1, Rotation10, b2 += b1);
                    b7 = RotlXor(b7, Rotation11, b4 += b7);
                    b5 = RotlXor(b5, Rotation12, b6 += b5);
                    b3 = RotlXor(b3, Rotation13, b0 += b3);

                    b1 = RotlXor(b1, Rotation20, b4 += b1);
                    b3 = RotlXor(b3, Rotation21, b6 += b3);
                    b5 = RotlXor(b5, Rotation22, b0 += b5);
                    b7 = RotlXor(b7, Rotation23, b2 += b7);

                    b1 = RotlXor(b1, Rotation30, b6 += b1);
                    b7 = RotlXor(b7, Rotation31, b0 += b7);
                    b5 = RotlXor(b5, Rotation32, b2 += b5);
                    b3 = RotlXor(b3, Rotation33, b4 += b3);

                    /*
                     * Subkey injection for first 4 rounds.
                     */
                    b0 += kw[dm9];
                    b1 += kw[dm9 + 1];
                    b2 += kw[dm9 + 2];
                    b3 += kw[dm9 + 3];
                    b4 += kw[dm9 + 4];
                    b5 += kw[dm9 + 5] + t[dm3];
                    b6 += kw[dm9 + 6] + t[dm3 + 1];
                    b7 += kw[dm9 + 7] + (uint)d;

                    /*
                     * 4 more rounds of mix/permute
                     */
                    b1 = RotlXor(b1, Rotation40, b0 += b1);
                    b3 = RotlXor(b3, Rotation41, b2 += b3);
                    b5 = RotlXor(b5, Rotation42, b4 += b5);
                    b7 = RotlXor(b7, Rotation43, b6 += b7);

                    b1 = RotlXor(b1, Rotation50, b2 += b1);
                    b7 = RotlXor(b7, Rotation51, b4 += b7);
                    b5 = RotlXor(b5, Rotation52, b6 += b5);
                    b3 = RotlXor(b3, Rotation53, b0 += b3);

                    b1 = RotlXor(b1, Rotation60, b4 += b1);
                    b3 = RotlXor(b3, Rotation61, b6 += b3);
                    b5 = RotlXor(b5, Rotation62, b0 += b5);
                    b7 = RotlXor(b7, Rotation63, b2 += b7);

                    b1 = RotlXor(b1, Rotation70, b6 += b1);
                    b7 = RotlXor(b7, Rotation71, b0 += b7);
                    b5 = RotlXor(b5, Rotation72, b2 += b5);
                    b3 = RotlXor(b3, Rotation73, b4 += b3);

                    /*
                     * Subkey injection for next 4 rounds.
                     */
                    b0 += kw[dm9 + 1];
                    b1 += kw[dm9 + 2];
                    b2 += kw[dm9 + 3];
                    b3 += kw[dm9 + 4];
                    b4 += kw[dm9 + 5];
                    b5 += kw[dm9 + 6] + t[dm3 + 1];
                    b6 += kw[dm9 + 7] + t[dm3 + 2];
                    b7 += kw[dm9 + 8] + (uint)d + 1;
                }

                /*
                 * Output cipher state.
                 */
                outWords[0] = b0;
                outWords[1] = b1;
                outWords[2] = b2;
                outWords[3] = b3;
                outWords[4] = b4;
                outWords[5] = b5;
                outWords[6] = b6;
                outWords[7] = b7;
            }

            internal override void DecryptBlock(ulong[] block, ulong[] state)
            {
                ulong[] kw = this.Kw;
                ulong[] t = this.T;
                int[] mod9 = Mod9;
                int[] mod3 = Mod3;

                /* Help the JIT avoid index bounds checks */
                if (kw.Length != 17)
                {
                    throw new ArgumentException();
                }

                if (t.Length != 5)
                {
                    throw new ArgumentException();
                }

                ulong b0 = block[0];
                ulong b1 = block[1];
                ulong b2 = block[2];
                ulong b3 = block[3];
                ulong b4 = block[4];
                ulong b5 = block[5];
                ulong b6 = block[6];
                ulong b7 = block[7];

                for (int d = (Rounds512 / 4) - 1; d >= 1; d -= 2)
                {
                    int dm9 = mod9[d];
                    int dm3 = mod3[d];

                    /* Reverse key injection for second 4 rounds */
                    b0 -= kw[dm9 + 1];
                    b1 -= kw[dm9 + 2];
                    b2 -= kw[dm9 + 3];
                    b3 -= kw[dm9 + 4];
                    b4 -= kw[dm9 + 5];
                    b5 -= kw[dm9 + 6] + t[dm3 + 1];
                    b6 -= kw[dm9 + 7] + t[dm3 + 2];
                    b7 -= kw[dm9 + 8] + (uint)d + 1;

                    /* Reverse second 4 mix/permute rounds */

                    b1 = XorRotr(b1, Rotation70, b6);
                    b6 -= b1;
                    b7 = XorRotr(b7, Rotation71, b0);
                    b0 -= b7;
                    b5 = XorRotr(b5, Rotation72, b2);
                    b2 -= b5;
                    b3 = XorRotr(b3, Rotation73, b4);
                    b4 -= b3;

                    b1 = XorRotr(b1, Rotation60, b4);
                    b4 -= b1;
                    b3 = XorRotr(b3, Rotation61, b6);
                    b6 -= b3;
                    b5 = XorRotr(b5, Rotation62, b0);
                    b0 -= b5;
                    b7 = XorRotr(b7, Rotation63, b2);
                    b2 -= b7;

                    b1 = XorRotr(b1, Rotation50, b2);
                    b2 -= b1;
                    b7 = XorRotr(b7, Rotation51, b4);
                    b4 -= b7;
                    b5 = XorRotr(b5, Rotation52, b6);
                    b6 -= b5;
                    b3 = XorRotr(b3, Rotation53, b0);
                    b0 -= b3;

                    b1 = XorRotr(b1, Rotation40, b0);
                    b0 -= b1;
                    b3 = XorRotr(b3, Rotation41, b2);
                    b2 -= b3;
                    b5 = XorRotr(b5, Rotation42, b4);
                    b4 -= b5;
                    b7 = XorRotr(b7, Rotation43, b6);
                    b6 -= b7;

                    /* Reverse key injection for first 4 rounds */
                    b0 -= kw[dm9];
                    b1 -= kw[dm9 + 1];
                    b2 -= kw[dm9 + 2];
                    b3 -= kw[dm9 + 3];
                    b4 -= kw[dm9 + 4];
                    b5 -= kw[dm9 + 5] + t[dm3];
                    b6 -= kw[dm9 + 6] + t[dm3 + 1];
                    b7 -= kw[dm9 + 7] + (uint)d;

                    /* Reverse first 4 mix/permute rounds */
                    b1 = XorRotr(b1, Rotation30, b6);
                    b6 -= b1;
                    b7 = XorRotr(b7, Rotation31, b0);
                    b0 -= b7;
                    b5 = XorRotr(b5, Rotation32, b2);
                    b2 -= b5;
                    b3 = XorRotr(b3, Rotation33, b4);
                    b4 -= b3;

                    b1 = XorRotr(b1, Rotation20, b4);
                    b4 -= b1;
                    b3 = XorRotr(b3, Rotation21, b6);
                    b6 -= b3;
                    b5 = XorRotr(b5, Rotation22, b0);
                    b0 -= b5;
                    b7 = XorRotr(b7, Rotation23, b2);
                    b2 -= b7;

                    b1 = XorRotr(b1, Rotation10, b2);
                    b2 -= b1;
                    b7 = XorRotr(b7, Rotation11, b4);
                    b4 -= b7;
                    b5 = XorRotr(b5, Rotation12, b6);
                    b6 -= b5;
                    b3 = XorRotr(b3, Rotation13, b0);
                    b0 -= b3;

                    b1 = XorRotr(b1, Rotation00, b0);
                    b0 -= b1;
                    b3 = XorRotr(b3, Rotation01, b2);
                    b2 -= b3;
                    b5 = XorRotr(b5, Rotation02, b4);
                    b4 -= b5;
                    b7 = XorRotr(b7, Rotation03, b6);
                    b6 -= b7;
                }

                /*
                 * First subkey uninjection.
                 */
                b0 -= kw[0];
                b1 -= kw[1];
                b2 -= kw[2];
                b3 -= kw[3];
                b4 -= kw[4];
                b5 -= kw[5] + t[0];
                b6 -= kw[6] + t[1];
                b7 -= kw[7];

                /*
                 * Output cipher state.
                 */
                state[0] = b0;
                state[1] = b1;
                state[2] = b2;
                state[3] = b3;
                state[4] = b4;
                state[5] = b5;
                state[6] = b6;
                state[7] = b7;
            }
        }

        private sealed class Threefish1024Cipher
            : ThreefishCipher
        {
            /**
	         * Mix rotation constants defined in Skein 1.3 specification
	         */
            private const int Rotation00 = 24, Rotation01 = 13, Rotation02 = 8, Rotation03 = 47;

            private const int Rotation04 = 8, Rotation05 = 17, Rotation06 = 22, Rotation07 = 37;
            private const int Rotation10 = 38, Rotation11 = 19, Rotation12 = 10, Rotation13 = 55;
            private const int Rotation14 = 49, Rotation15 = 18, Rotation16 = 23, Rotation17 = 52;
            private const int Rotation20 = 33, Rotation21 = 4, Rotation22 = 51, Rotation23 = 13;
            private const int Rotation24 = 34, Rotation25 = 41, Rotation26 = 59, Rotation27 = 17;
            private const int Rotation30 = 5, Rotation31 = 20, Rotation32 = 48, Rotation33 = 41;
            private const int Rotation34 = 47, Rotation35 = 28, Rotation36 = 16, Rotation37 = 25;

            private const int Rotation40 = 41, Rotation41 = 9, Rotation42 = 37, Rotation43 = 31;
            private const int Rotation44 = 12, Rotation45 = 47, Rotation46 = 44, Rotation47 = 30;
            private const int Rotation50 = 16, Rotation51 = 34, Rotation52 = 56, Rotation53 = 51;
            private const int Rotation54 = 4, Rotation55 = 53, Rotation56 = 42, Rotation57 = 41;
            private const int Rotation60 = 31, Rotation61 = 44, Rotation62 = 47, Rotation63 = 46;
            private const int Rotation64 = 19, Rotation65 = 42, Rotation66 = 44, Rotation67 = 25;
            private const int Rotation70 = 9, Rotation71 = 48, Rotation72 = 35, Rotation73 = 52;
            private const int Rotation74 = 23, Rotation75 = 31, Rotation76 = 37, Rotation77 = 20;

            public Threefish1024Cipher(ulong[] kw, ulong[] t)
                : base(kw, t)
            {
            }

            internal override void EncryptBlock(ulong[] block, ulong[] outWords)
            {
                ulong[] kw = this.Kw;
                ulong[] t = this.T;
                int[] mod17 = Mod17;
                int[] mod3 = Mod3;

                /* Help the JIT avoid index bounds checks */
                if (kw.Length != 33)
                {
                    throw new ArgumentException();
                }

                if (t.Length != 5)
                {
                    throw new ArgumentException();
                }

                /*
                 * Read 16 words of plaintext data, not using arrays for cipher state
                 */
                ulong b0 = block[0];
                ulong b1 = block[1];
                ulong b2 = block[2];
                ulong b3 = block[3];
                ulong b4 = block[4];
                ulong b5 = block[5];
                ulong b6 = block[6];
                ulong b7 = block[7];
                ulong b8 = block[8];
                ulong b9 = block[9];
                ulong b10 = block[10];
                ulong b11 = block[11];
                ulong b12 = block[12];
                ulong b13 = block[13];
                ulong b14 = block[14];
                ulong b15 = block[15];

                /*
                 * First subkey injection.
                 */
                b0 += kw[0];
                b1 += kw[1];
                b2 += kw[2];
                b3 += kw[3];
                b4 += kw[4];
                b5 += kw[5];
                b6 += kw[6];
                b7 += kw[7];
                b8 += kw[8];
                b9 += kw[9];
                b10 += kw[10];
                b11 += kw[11];
                b12 += kw[12];
                b13 += kw[13] + t[0];
                b14 += kw[14] + t[1];
                b15 += kw[15];

                /*
                 * Rounds loop, unrolled to 8 rounds per iteration.
                 * 
                 * Unrolling to multiples of 4 avoids the mod 4 check for key injection, and allows
                 * inlining of the permutations, which cycle every of 4 rounds (avoiding array
                 * index/lookup).
                 * 
                 * Unrolling to multiples of 8 avoids the mod 8 rotation constant lookup, and allows
                 * inlining constant rotation values (avoiding array index/lookup).
                 */

                for (int d = 1; d < (Rounds1024 / 4); d += 2)
                {
                    int dm17 = mod17[d];
                    int dm3 = mod3[d];

                    /*
                     * 4 rounds of mix and permute.
                     * 
                     * Permute schedule has a 4 round cycle, so permutes are inlined in the mix
                     * operations in each 4 round block.
                     */
                    b1 = RotlXor(b1, Rotation00, b0 += b1);
                    b3 = RotlXor(b3, Rotation01, b2 += b3);
                    b5 = RotlXor(b5, Rotation02, b4 += b5);
                    b7 = RotlXor(b7, Rotation03, b6 += b7);
                    b9 = RotlXor(b9, Rotation04, b8 += b9);
                    b11 = RotlXor(b11, Rotation05, b10 += b11);
                    b13 = RotlXor(b13, Rotation06, b12 += b13);
                    b15 = RotlXor(b15, Rotation07, b14 += b15);

                    b9 = RotlXor(b9, Rotation10, b0 += b9);
                    b13 = RotlXor(b13, Rotation11, b2 += b13);
                    b11 = RotlXor(b11, Rotation12, b6 += b11);
                    b15 = RotlXor(b15, Rotation13, b4 += b15);
                    b7 = RotlXor(b7, Rotation14, b10 += b7);
                    b3 = RotlXor(b3, Rotation15, b12 += b3);
                    b5 = RotlXor(b5, Rotation16, b14 += b5);
                    b1 = RotlXor(b1, Rotation17, b8 += b1);

                    b7 = RotlXor(b7, Rotation20, b0 += b7);
                    b5 = RotlXor(b5, Rotation21, b2 += b5);
                    b3 = RotlXor(b3, Rotation22, b4 += b3);
                    b1 = RotlXor(b1, Rotation23, b6 += b1);
                    b15 = RotlXor(b15, Rotation24, b12 += b15);
                    b13 = RotlXor(b13, Rotation25, b14 += b13);
                    b11 = RotlXor(b11, Rotation26, b8 += b11);
                    b9 = RotlXor(b9, Rotation27, b10 += b9);

                    b15 = RotlXor(b15, Rotation30, b0 += b15);
                    b11 = RotlXor(b11, Rotation31, b2 += b11);
                    b13 = RotlXor(b13, Rotation32, b6 += b13);
                    b9 = RotlXor(b9, Rotation33, b4 += b9);
                    b1 = RotlXor(b1, Rotation34, b14 += b1);
                    b5 = RotlXor(b5, Rotation35, b8 += b5);
                    b3 = RotlXor(b3, Rotation36, b10 += b3);
                    b7 = RotlXor(b7, Rotation37, b12 += b7);

                    /*
                     * Subkey injection for first 4 rounds.
                     */
                    b0 += kw[dm17];
                    b1 += kw[dm17 + 1];
                    b2 += kw[dm17 + 2];
                    b3 += kw[dm17 + 3];
                    b4 += kw[dm17 + 4];
                    b5 += kw[dm17 + 5];
                    b6 += kw[dm17 + 6];
                    b7 += kw[dm17 + 7];
                    b8 += kw[dm17 + 8];
                    b9 += kw[dm17 + 9];
                    b10 += kw[dm17 + 10];
                    b11 += kw[dm17 + 11];
                    b12 += kw[dm17 + 12];
                    b13 += kw[dm17 + 13] + t[dm3];
                    b14 += kw[dm17 + 14] + t[dm3 + 1];
                    b15 += kw[dm17 + 15] + (uint)d;

                    /*
                     * 4 more rounds of mix/permute
                     */
                    b1 = RotlXor(b1, Rotation40, b0 += b1);
                    b3 = RotlXor(b3, Rotation41, b2 += b3);
                    b5 = RotlXor(b5, Rotation42, b4 += b5);
                    b7 = RotlXor(b7, Rotation43, b6 += b7);
                    b9 = RotlXor(b9, Rotation44, b8 += b9);
                    b11 = RotlXor(b11, Rotation45, b10 += b11);
                    b13 = RotlXor(b13, Rotation46, b12 += b13);
                    b15 = RotlXor(b15, Rotation47, b14 += b15);

                    b9 = RotlXor(b9, Rotation50, b0 += b9);
                    b13 = RotlXor(b13, Rotation51, b2 += b13);
                    b11 = RotlXor(b11, Rotation52, b6 += b11);
                    b15 = RotlXor(b15, Rotation53, b4 += b15);
                    b7 = RotlXor(b7, Rotation54, b10 += b7);
                    b3 = RotlXor(b3, Rotation55, b12 += b3);
                    b5 = RotlXor(b5, Rotation56, b14 += b5);
                    b1 = RotlXor(b1, Rotation57, b8 += b1);

                    b7 = RotlXor(b7, Rotation60, b0 += b7);
                    b5 = RotlXor(b5, Rotation61, b2 += b5);
                    b3 = RotlXor(b3, Rotation62, b4 += b3);
                    b1 = RotlXor(b1, Rotation63, b6 += b1);
                    b15 = RotlXor(b15, Rotation64, b12 += b15);
                    b13 = RotlXor(b13, Rotation65, b14 += b13);
                    b11 = RotlXor(b11, Rotation66, b8 += b11);
                    b9 = RotlXor(b9, Rotation67, b10 += b9);

                    b15 = RotlXor(b15, Rotation70, b0 += b15);
                    b11 = RotlXor(b11, Rotation71, b2 += b11);
                    b13 = RotlXor(b13, Rotation72, b6 += b13);
                    b9 = RotlXor(b9, Rotation73, b4 += b9);
                    b1 = RotlXor(b1, Rotation74, b14 += b1);
                    b5 = RotlXor(b5, Rotation75, b8 += b5);
                    b3 = RotlXor(b3, Rotation76, b10 += b3);
                    b7 = RotlXor(b7, Rotation77, b12 += b7);

                    /*
                     * Subkey injection for next 4 rounds.
                     */
                    b0 += kw[dm17 + 1];
                    b1 += kw[dm17 + 2];
                    b2 += kw[dm17 + 3];
                    b3 += kw[dm17 + 4];
                    b4 += kw[dm17 + 5];
                    b5 += kw[dm17 + 6];
                    b6 += kw[dm17 + 7];
                    b7 += kw[dm17 + 8];
                    b8 += kw[dm17 + 9];
                    b9 += kw[dm17 + 10];
                    b10 += kw[dm17 + 11];
                    b11 += kw[dm17 + 12];
                    b12 += kw[dm17 + 13];
                    b13 += kw[dm17 + 14] + t[dm3 + 1];
                    b14 += kw[dm17 + 15] + t[dm3 + 2];
                    b15 += kw[dm17 + 16] + (uint)d + 1;
                }

                /*
                 * Output cipher state.
                 */
                outWords[0] = b0;
                outWords[1] = b1;
                outWords[2] = b2;
                outWords[3] = b3;
                outWords[4] = b4;
                outWords[5] = b5;
                outWords[6] = b6;
                outWords[7] = b7;
                outWords[8] = b8;
                outWords[9] = b9;
                outWords[10] = b10;
                outWords[11] = b11;
                outWords[12] = b12;
                outWords[13] = b13;
                outWords[14] = b14;
                outWords[15] = b15;
            }

            internal override void DecryptBlock(ulong[] block, ulong[] state)
            {
                ulong[] kw = this.Kw;
                ulong[] t = this.T;
                int[] mod17 = Mod17;
                int[] mod3 = Mod3;

                /* Help the JIT avoid index bounds checks */
                if (kw.Length != 33)
                {
                    throw new ArgumentException();
                }

                if (t.Length != 5)
                {
                    throw new ArgumentException();
                }

                ulong b0 = block[0];
                ulong b1 = block[1];
                ulong b2 = block[2];
                ulong b3 = block[3];
                ulong b4 = block[4];
                ulong b5 = block[5];
                ulong b6 = block[6];
                ulong b7 = block[7];
                ulong b8 = block[8];
                ulong b9 = block[9];
                ulong b10 = block[10];
                ulong b11 = block[11];
                ulong b12 = block[12];
                ulong b13 = block[13];
                ulong b14 = block[14];
                ulong b15 = block[15];

                for (int d = (Rounds1024 / 4) - 1; d >= 1; d -= 2)
                {
                    int dm17 = mod17[d];
                    int dm3 = mod3[d];

                    /* Reverse key injection for second 4 rounds */
                    b0 -= kw[dm17 + 1];
                    b1 -= kw[dm17 + 2];
                    b2 -= kw[dm17 + 3];
                    b3 -= kw[dm17 + 4];
                    b4 -= kw[dm17 + 5];
                    b5 -= kw[dm17 + 6];
                    b6 -= kw[dm17 + 7];
                    b7 -= kw[dm17 + 8];
                    b8 -= kw[dm17 + 9];
                    b9 -= kw[dm17 + 10];
                    b10 -= kw[dm17 + 11];
                    b11 -= kw[dm17 + 12];
                    b12 -= kw[dm17 + 13];
                    b13 -= kw[dm17 + 14] + t[dm3 + 1];
                    b14 -= kw[dm17 + 15] + t[dm3 + 2];
                    b15 -= kw[dm17 + 16] + (uint)d + 1;

                    /* Reverse second 4 mix/permute rounds */
                    b15 = XorRotr(b15, Rotation70, b0);
                    b0 -= b15;
                    b11 = XorRotr(b11, Rotation71, b2);
                    b2 -= b11;
                    b13 = XorRotr(b13, Rotation72, b6);
                    b6 -= b13;
                    b9 = XorRotr(b9, Rotation73, b4);
                    b4 -= b9;
                    b1 = XorRotr(b1, Rotation74, b14);
                    b14 -= b1;
                    b5 = XorRotr(b5, Rotation75, b8);
                    b8 -= b5;
                    b3 = XorRotr(b3, Rotation76, b10);
                    b10 -= b3;
                    b7 = XorRotr(b7, Rotation77, b12);
                    b12 -= b7;

                    b7 = XorRotr(b7, Rotation60, b0);
                    b0 -= b7;
                    b5 = XorRotr(b5, Rotation61, b2);
                    b2 -= b5;
                    b3 = XorRotr(b3, Rotation62, b4);
                    b4 -= b3;
                    b1 = XorRotr(b1, Rotation63, b6);
                    b6 -= b1;
                    b15 = XorRotr(b15, Rotation64, b12);
                    b12 -= b15;
                    b13 = XorRotr(b13, Rotation65, b14);
                    b14 -= b13;
                    b11 = XorRotr(b11, Rotation66, b8);
                    b8 -= b11;
                    b9 = XorRotr(b9, Rotation67, b10);
                    b10 -= b9;

                    b9 = XorRotr(b9, Rotation50, b0);
                    b0 -= b9;
                    b13 = XorRotr(b13, Rotation51, b2);
                    b2 -= b13;
                    b11 = XorRotr(b11, Rotation52, b6);
                    b6 -= b11;
                    b15 = XorRotr(b15, Rotation53, b4);
                    b4 -= b15;
                    b7 = XorRotr(b7, Rotation54, b10);
                    b10 -= b7;
                    b3 = XorRotr(b3, Rotation55, b12);
                    b12 -= b3;
                    b5 = XorRotr(b5, Rotation56, b14);
                    b14 -= b5;
                    b1 = XorRotr(b1, Rotation57, b8);
                    b8 -= b1;

                    b1 = XorRotr(b1, Rotation40, b0);
                    b0 -= b1;
                    b3 = XorRotr(b3, Rotation41, b2);
                    b2 -= b3;
                    b5 = XorRotr(b5, Rotation42, b4);
                    b4 -= b5;
                    b7 = XorRotr(b7, Rotation43, b6);
                    b6 -= b7;
                    b9 = XorRotr(b9, Rotation44, b8);
                    b8 -= b9;
                    b11 = XorRotr(b11, Rotation45, b10);
                    b10 -= b11;
                    b13 = XorRotr(b13, Rotation46, b12);
                    b12 -= b13;
                    b15 = XorRotr(b15, Rotation47, b14);
                    b14 -= b15;

                    /* Reverse key injection for first 4 rounds */
                    b0 -= kw[dm17];
                    b1 -= kw[dm17 + 1];
                    b2 -= kw[dm17 + 2];
                    b3 -= kw[dm17 + 3];
                    b4 -= kw[dm17 + 4];
                    b5 -= kw[dm17 + 5];
                    b6 -= kw[dm17 + 6];
                    b7 -= kw[dm17 + 7];
                    b8 -= kw[dm17 + 8];
                    b9 -= kw[dm17 + 9];
                    b10 -= kw[dm17 + 10];
                    b11 -= kw[dm17 + 11];
                    b12 -= kw[dm17 + 12];
                    b13 -= kw[dm17 + 13] + t[dm3];
                    b14 -= kw[dm17 + 14] + t[dm3 + 1];
                    b15 -= kw[dm17 + 15] + (uint)d;

                    /* Reverse first 4 mix/permute rounds */
                    b15 = XorRotr(b15, Rotation30, b0);
                    b0 -= b15;
                    b11 = XorRotr(b11, Rotation31, b2);
                    b2 -= b11;
                    b13 = XorRotr(b13, Rotation32, b6);
                    b6 -= b13;
                    b9 = XorRotr(b9, Rotation33, b4);
                    b4 -= b9;
                    b1 = XorRotr(b1, Rotation34, b14);
                    b14 -= b1;
                    b5 = XorRotr(b5, Rotation35, b8);
                    b8 -= b5;
                    b3 = XorRotr(b3, Rotation36, b10);
                    b10 -= b3;
                    b7 = XorRotr(b7, Rotation37, b12);
                    b12 -= b7;

                    b7 = XorRotr(b7, Rotation20, b0);
                    b0 -= b7;
                    b5 = XorRotr(b5, Rotation21, b2);
                    b2 -= b5;
                    b3 = XorRotr(b3, Rotation22, b4);
                    b4 -= b3;
                    b1 = XorRotr(b1, Rotation23, b6);
                    b6 -= b1;
                    b15 = XorRotr(b15, Rotation24, b12);
                    b12 -= b15;
                    b13 = XorRotr(b13, Rotation25, b14);
                    b14 -= b13;
                    b11 = XorRotr(b11, Rotation26, b8);
                    b8 -= b11;
                    b9 = XorRotr(b9, Rotation27, b10);
                    b10 -= b9;

                    b9 = XorRotr(b9, Rotation10, b0);
                    b0 -= b9;
                    b13 = XorRotr(b13, Rotation11, b2);
                    b2 -= b13;
                    b11 = XorRotr(b11, Rotation12, b6);
                    b6 -= b11;
                    b15 = XorRotr(b15, Rotation13, b4);
                    b4 -= b15;
                    b7 = XorRotr(b7, Rotation14, b10);
                    b10 -= b7;
                    b3 = XorRotr(b3, Rotation15, b12);
                    b12 -= b3;
                    b5 = XorRotr(b5, Rotation16, b14);
                    b14 -= b5;
                    b1 = XorRotr(b1, Rotation17, b8);
                    b8 -= b1;

                    b1 = XorRotr(b1, Rotation00, b0);
                    b0 -= b1;
                    b3 = XorRotr(b3, Rotation01, b2);
                    b2 -= b3;
                    b5 = XorRotr(b5, Rotation02, b4);
                    b4 -= b5;
                    b7 = XorRotr(b7, Rotation03, b6);
                    b6 -= b7;
                    b9 = XorRotr(b9, Rotation04, b8);
                    b8 -= b9;
                    b11 = XorRotr(b11, Rotation05, b10);
                    b10 -= b11;
                    b13 = XorRotr(b13, Rotation06, b12);
                    b12 -= b13;
                    b15 = XorRotr(b15, Rotation07, b14);
                    b14 -= b15;
                }

                /*
                 * First subkey uninjection.
                 */
                b0 -= kw[0];
                b1 -= kw[1];
                b2 -= kw[2];
                b3 -= kw[3];
                b4 -= kw[4];
                b5 -= kw[5];
                b6 -= kw[6];
                b7 -= kw[7];
                b8 -= kw[8];
                b9 -= kw[9];
                b10 -= kw[10];
                b11 -= kw[11];
                b12 -= kw[12];
                b13 -= kw[13] + t[0];
                b14 -= kw[14] + t[1];
                b15 -= kw[15];

                /*
                 * Output cipher state.
                 */
                state[0] = b0;
                state[1] = b1;
                state[2] = b2;
                state[3] = b3;
                state[4] = b4;
                state[5] = b5;
                state[6] = b6;
                state[7] = b7;
                state[8] = b8;
                state[9] = b9;
                state[10] = b10;
                state[11] = b11;
                state[12] = b12;
                state[13] = b13;
                state[14] = b14;
                state[15] = b15;
            }
        }
    }
}
#pragma warning restore
#endif
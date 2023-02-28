#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable

using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
#if UNITY_WSA && !UNITY_EDITOR && !ENABLE_IL2CPP
using System.TypeFix;
#endif

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
    * The specification for RC5 came from the <code>RC5 Encryption Algorithm</code>
    * publication in RSA CryptoBytes, Spring of 1995.
    * <em>http://www.rsasecurity.com/rsalabs/cryptobytes</em>.
    * <p>
    * This implementation is set to work with a 64 bit word size.</p>
    */
    public class Rc564Engine
        : IBlockCipher
    {
        private static readonly int WordSize = 64;
        private static readonly int BytesPerWord = WordSize / 8;

        /*
        * our "magic constants" for wordSize 62
        *
        * Pw = Odd((e-2) * 2^wordsize)
        * Qw = Odd((o-2) * 2^wordsize)
        *
        * where e is the base of natural logarithms (2.718281828...)
        * and o is the golden ratio (1.61803398...)
        */
        private static readonly long P64 = unchecked((long)0xb7e151628aed2a6bL);
        private static readonly long Q64 = unchecked((long)0x9e3779b97f4a7c15L);

        private bool _forEncryption;

        /*
        * the number of rounds to perform
        */
        private int _noRounds;

        /*
        * the expanded key array of size 2*(rounds + 1)
        */
        private long[] _s;

        /**
        * Create an instance of the RC5 encryption algorithm
        * and set some defaults
        */
        public Rc564Engine()
        {
            _noRounds = 12;
//            _S            = null;
        }

        public virtual string AlgorithmName
        {
            get { return "RC5-64"; }
        }

        public virtual bool IsPartialBlockOkay
        {
            get { return false; }
        }

        public virtual int GetBlockSize()
        {
            return 2 * BytesPerWord;
        }

        /**
        * initialise a RC5-64 cipher.
        *
        * @param forEncryption whether or not we are for encryption.
        * @param parameters the parameters required to set up the cipher.
        * @exception ArgumentException if the parameters argument is
        * inappropriate.
        */
        public virtual void Init(
            bool forEncryption,
            ICipherParameters parameters)
        {
            if (!(typeof(RC5Parameters).IsInstanceOfType(parameters)))
            {
                throw new ArgumentException("invalid parameter passed to RC564 init - " +
                                            BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(
                                                parameters));
            }

            RC5Parameters p = (RC5Parameters)parameters;

            this._forEncryption = forEncryption;

            _noRounds = p.Rounds;

            SetKey(p.GetKey());
        }

        public virtual int ProcessBlock(
            byte[] input,
            int inOff,
            byte[] output,
            int outOff)
        {
            return (_forEncryption)
                ? EncryptBlock(input, inOff, output, outOff)
                : DecryptBlock(input, inOff, output, outOff);
        }

        public virtual void Reset()
        {
        }

        /**
        * Re-key the cipher.
        *
        * @param  key  the key to be used
        */
        private void SetKey(
            byte[] key)
        {
            //
            // KEY EXPANSION:
            //
            // There are 3 phases to the key expansion.
            //
            // Phase 1:
            //   Copy the secret key K[0...b-1] into an array L[0..c-1] of
            //   c = ceil(b/u), where u = wordSize/8 in little-endian order.
            //   In other words, we fill up L using u consecutive key bytes
            //   of K. Any unfilled byte positions in L are zeroed. In the
            //   case that b = c = 0, set c = 1 and L[0] = 0.
            //
            long[] l = new long[(key.Length + (BytesPerWord - 1)) / BytesPerWord];

            for (int i = 0; i != key.Length; i++)
            {
                l[i / BytesPerWord] += (long)(key[i] & 0xff) << (8 * (i % BytesPerWord));
            }

            //
            // Phase 2:
            //   Initialize S to a particular fixed pseudo-random bit pattern
            //   using an arithmetic progression modulo 2^wordsize determined
            //   by the magic numbers, Pw & Qw.
            //
            _s = new long[2 * (_noRounds + 1)];

            _s[0] = P64;
            for (int i = 1; i < _s.Length; i++)
            {
                _s[i] = (_s[i - 1] + Q64);
            }

            //
            // Phase 3:
            //   Mix in the user's secret key in 3 passes over the arrays S & L.
            //   The max of the arrays sizes is used as the loop control
            //
            int iter;

            if (l.Length > _s.Length)
            {
                iter = 3 * l.Length;
            }
            else
            {
                iter = 3 * _s.Length;
            }

            long a = 0, b = 0;
            int ii = 0, jj = 0;

            for (int k = 0; k < iter; k++)
            {
                a = _s[ii] = RotateLeft(_s[ii] + a + b, 3);
                b = l[jj] = RotateLeft(l[jj] + a + b, a + b);
                ii = (ii + 1) % _s.Length;
                jj = (jj + 1) % l.Length;
            }
        }

        /**
        * Encrypt the given block starting at the given offset and place
        * the result in the provided buffer starting at the given offset.
        *
        * @param  in      in byte buffer containing data to encrypt
        * @param  inOff   offset into src buffer
        * @param  out     out buffer where encrypted data is written
        * @param  outOff  offset into out buffer
        */
        private int EncryptBlock(
            byte[] input,
            int inOff,
            byte[] outBytes,
            int outOff)
        {
            long a = BytesToWord(input, inOff) + _s[0];
            long b = BytesToWord(input, inOff + BytesPerWord) + _s[1];

            for (int i = 1; i <= _noRounds; i++)
            {
                a = RotateLeft(a ^ b, b) + _s[2 * i];
                b = RotateLeft(b ^ a, a) + _s[2 * i + 1];
            }

            WordToBytes(a, outBytes, outOff);
            WordToBytes(b, outBytes, outOff + BytesPerWord);

            return 2 * BytesPerWord;
        }

        private int DecryptBlock(
            byte[] input,
            int inOff,
            byte[] outBytes,
            int outOff)
        {
            long a = BytesToWord(input, inOff);
            long b = BytesToWord(input, inOff + BytesPerWord);

            for (int i = _noRounds; i >= 1; i--)
            {
                b = RotateRight(b - _s[2 * i + 1], a) ^ a;
                a = RotateRight(a - _s[2 * i], b) ^ b;
            }

            WordToBytes(a - _s[0], outBytes, outOff);
            WordToBytes(b - _s[1], outBytes, outOff + BytesPerWord);

            return 2 * BytesPerWord;
        }


        //////////////////////////////////////////////////////////////
        //
        // PRIVATE Helper Methods
        //
        //////////////////////////////////////////////////////////////

        /**
        * Perform a left "spin" of the word. The rotation of the given
        * word <em>x</em> is rotated left by <em>y</em> bits.
        * Only the <em>lg(wordSize)</em> low-order bits of <em>y</em>
        * are used to determine the rotation amount. Here it is
        * assumed that the wordsize used is a power of 2.
        *
        * @param  x  word to rotate
        * @param  y    number of bits to rotate % wordSize
        */
        private long RotateLeft(long x, long y)
        {
            return ((long)((ulong)(x << (int)(y & (WordSize - 1))) |
                           ((ulong)x >> (int)(WordSize - (y & (WordSize - 1)))))
                );
        }

        /**
        * Perform a right "spin" of the word. The rotation of the given
        * word <em>x</em> is rotated left by <em>y</em> bits.
        * Only the <em>lg(wordSize)</em> low-order bits of <em>y</em>
        * are used to determine the rotation amount. Here it is
        * assumed that the wordsize used is a power of 2.
        *
        * @param x word to rotate
        * @param y number of bits to rotate % wordSize
        */
        private long RotateRight(long x, long y)
        {
            return ((long)(((ulong)x >> (int)(y & (WordSize - 1))) |
                           (ulong)(x << (int)(WordSize - (y & (WordSize - 1)))))
                );
        }

        private long BytesToWord(
            byte[] src,
            int srcOff)
        {
            long word = 0;

            for (int i = BytesPerWord - 1; i >= 0; i--)
            {
                word = (word << 8) + (src[i + srcOff] & 0xff);
            }

            return word;
        }

        private void WordToBytes(
            long word,
            byte[] dst,
            int dstOff)
        {
            for (int i = 0; i < BytesPerWord; i++)
            {
                dst[i + dstOff] = (byte)word;
                word = (long)((ulong)word >> 8);
            }
        }
    }
}
#pragma warning restore
#endif
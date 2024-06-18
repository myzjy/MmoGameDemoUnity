#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines
{
    /**
    * An RC6 engine.
    */
    public class Rc6Engine
        : IBlockCipher
    {
        private static readonly int WordSize = 32;
        private static readonly int BytesPerWord = WordSize / 8;

        /*
        * the number of rounds to perform
        */
        private static readonly int NoRounds = 20;

        /*
        * our "magic constants" for wordSize 32
        *
        * Pw = Odd((e-2) * 2^wordsize)
        * Qw = Odd((o-2) * 2^wordsize)
        *
        * where e is the base of natural logarithms (2.718281828...)
        * and o is the golden ratio (1.61803398...)
        */
        private static readonly int P32 = unchecked((int)0xb7e15163);
        private static readonly int Q32 = unchecked((int)0x9e3779b9);

        private static readonly int Lgw = 5; // log2(32)

        private bool _forEncryption;

        /*
        * the expanded key array of size 2*(rounds + 1)
        */
        private int[] _s;

        /**
        * Create an instance of the RC6 encryption algorithm
        * and set some defaults
        */
        public Rc6Engine()
        {
//            _S            = null;
        }

        public virtual string AlgorithmName
        {
            get { return "RC6"; }
        }

        public virtual bool IsPartialBlockOkay
        {
            get { return false; }
        }

        public virtual int GetBlockSize()
        {
            return 4 * BytesPerWord;
        }

        /**
        * initialise a RC5-32 cipher.
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
            if (!(parameters is KeyParameter))
                throw new ArgumentException("invalid parameter passed to RC6 init - " +
                                            BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.GetTypeName(
                                                parameters));

            this._forEncryption = forEncryption;

            KeyParameter p = (KeyParameter)parameters;
            SetKey(p.GetKey());
        }

        public virtual int ProcessBlock(
            byte[] input,
            int inOff,
            byte[] output,
            int outOff)
        {
            int blockSize = GetBlockSize();
            if (_s == null)
                throw new InvalidOperationException("RC6 engine not initialised");

            Check.DataLength(input, inOff, blockSize, "input buffer too short");
            Check.OutputLength(output, outOff, blockSize, "output buffer too short");

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
        * @param inKey the key to be used
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
            // compute number of dwords
            int c = (key.Length + (BytesPerWord - 1)) / BytesPerWord;
            if (c == 0)
            {
                c = 1;
            }

            int[] l = new int[(key.Length + BytesPerWord - 1) / BytesPerWord];

            // load all key bytes into array of key dwords
            for (int i = key.Length - 1; i >= 0; i--)
            {
                l[i / BytesPerWord] = (l[i / BytesPerWord] << 8) + (key[i] & 0xff);
            }

            //
            // Phase 2:
            //   Key schedule is placed in a array of 2+2*ROUNDS+2 = 44 dwords.
            //   Initialize S to a particular fixed pseudo-random bit pattern
            //   using an arithmetic progression modulo 2^wordsize determined
            //   by the magic numbers, Pw & Qw.
            //
            _s = new int[2 + 2 * NoRounds + 2];

            _s[0] = P32;
            for (int i = 1; i < _s.Length; i++)
            {
                _s[i] = (_s[i - 1] + Q32);
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

            int a = 0;
            int b = 0;
            int ii = 0, jj = 0;

            for (int k = 0; k < iter; k++)
            {
                a = _s[ii] = RotateLeft(_s[ii] + a + b, 3);
                b = l[jj] = RotateLeft(l[jj] + a + b, a + b);
                ii = (ii + 1) % _s.Length;
                jj = (jj + 1) % l.Length;
            }
        }

        private int EncryptBlock(
            byte[] input,
            int inOff,
            byte[] outBytes,
            int outOff)
        {
            // load A,B,C and D registers from in.
            int a = BytesToWord(input, inOff);
            int b = BytesToWord(input, inOff + BytesPerWord);
            int c = BytesToWord(input, inOff + BytesPerWord * 2);
            int d = BytesToWord(input, inOff + BytesPerWord * 3);

            // Do pseudo-round #0: pre-whitening of B and D
            b += _s[0];
            d += _s[1];

            // perform round #1,#2 ... #ROUNDS of encryption
            for (int i = 1; i <= NoRounds; i++)
            {
                int t = 0, u = 0;

                t = b * (2 * b + 1);
                t = RotateLeft(t, 5);

                u = d * (2 * d + 1);
                u = RotateLeft(u, 5);

                a ^= t;
                a = RotateLeft(a, u);
                a += _s[2 * i];

                c ^= u;
                c = RotateLeft(c, t);
                c += _s[2 * i + 1];

                int temp = a;
                a = b;
                b = c;
                c = d;
                d = temp;
            }

            // do pseudo-round #(ROUNDS+1) : post-whitening of A and C
            a += _s[2 * NoRounds + 2];
            c += _s[2 * NoRounds + 3];

            // store A, B, C and D registers to out
            WordToBytes(a, outBytes, outOff);
            WordToBytes(b, outBytes, outOff + BytesPerWord);
            WordToBytes(c, outBytes, outOff + BytesPerWord * 2);
            WordToBytes(d, outBytes, outOff + BytesPerWord * 3);

            return 4 * BytesPerWord;
        }

        private int DecryptBlock(
            byte[] input,
            int inOff,
            byte[] outBytes,
            int outOff)
        {
            // load A,B,C and D registers from out.
            int a = BytesToWord(input, inOff);
            int b = BytesToWord(input, inOff + BytesPerWord);
            int c = BytesToWord(input, inOff + BytesPerWord * 2);
            int d = BytesToWord(input, inOff + BytesPerWord * 3);

            // Undo pseudo-round #(ROUNDS+1) : post whitening of A and C
            c -= _s[2 * NoRounds + 3];
            a -= _s[2 * NoRounds + 2];

            // Undo round #ROUNDS, .., #2,#1 of encryption
            for (int i = NoRounds; i >= 1; i--)
            {
                int t = 0, u = 0;

                int temp = d;
                d = c;
                c = b;
                b = a;
                a = temp;

                t = b * (2 * b + 1);
                t = RotateLeft(t, Lgw);

                u = d * (2 * d + 1);
                u = RotateLeft(u, Lgw);

                c -= _s[2 * i + 1];
                c = RotateRight(c, t);
                c ^= u;

                a -= _s[2 * i];
                a = RotateRight(a, u);
                a ^= t;
            }

            // Undo pseudo-round #0: pre-whitening of B and D
            d -= _s[1];
            b -= _s[0];

            WordToBytes(a, outBytes, outOff);
            WordToBytes(b, outBytes, outOff + BytesPerWord);
            WordToBytes(c, outBytes, outOff + BytesPerWord * 2);
            WordToBytes(d, outBytes, outOff + BytesPerWord * 3);

            return 4 * BytesPerWord;
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
        * @param x word to rotate
        * @param y number of bits to rotate % wordSize
        */
        private int RotateLeft(int x, int y)
        {
            return ((int)((uint)(x << (y & (WordSize - 1)))
                          | ((uint)x >> (WordSize - (y & (WordSize - 1))))));
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
        private int RotateRight(int x, int y)
        {
            return ((int)(((uint)x >> (y & (WordSize - 1)))
                          | (uint)(x << (WordSize - (y & (WordSize - 1))))));
        }

        private int BytesToWord(
            byte[] src,
            int srcOff)
        {
            int word = 0;

            for (int i = BytesPerWord - 1; i >= 0; i--)
            {
                word = (word << 8) + (src[i + srcOff] & 0xff);
            }

            return word;
        }

        private void WordToBytes(
            int word,
            byte[] dst,
            int dstOff)
        {
            for (int i = 0; i < BytesPerWord; i++)
            {
                dst[i + dstOff] = (byte)word;
                word = (int)((uint)word >> 8);
            }
        }
    }
}
#pragma warning restore
#endif
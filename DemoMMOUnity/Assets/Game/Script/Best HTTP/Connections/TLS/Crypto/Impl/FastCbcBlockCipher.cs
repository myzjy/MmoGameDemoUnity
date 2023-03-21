#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.Connections.TLS.Crypto.Impl;
using BestHTTP.PlatformSupport.IL2CPP;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto;

namespace BestHTTP.Connections.TLS.Crypto
{
    /**
    * implements Cipher-Block-Chaining (CBC) mode on top of a simple cipher.
    */
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [Il2CppEagerStaticClassConstruction]
    public class FastCbcBlockCipher
        : IBlockCipher
    {
        private readonly byte[] _iv;
        private byte[] _cbcV, _cbcNextV;
        private readonly int _blockSize;
        private readonly IBlockCipher _cipher;
        private bool _encrypting;

        /**
        * Basic constructor.
        *
        * @param cipher the block cipher to be used as the basis of chaining.
        */
        public FastCbcBlockCipher(
            IBlockCipher cipher)
        {
            _cipher = cipher;
            _blockSize = cipher.GetBlockSize();

            _iv = new byte[_blockSize];
            _cbcV = new byte[_blockSize];
            _cbcNextV = new byte[_blockSize];
        }

        /**
        * return the underlying block cipher that we are wrapping.
        *
        * @return the underlying block cipher that we are wrapping.
        */
        public IBlockCipher GetUnderlyingCipher()
        {
            return _cipher;
        }

        /**
        * Initialise the cipher and, possibly, the initialisation vector (IV).
        * If an IV isn't passed as part of the parameter, the IV will be all zeros.
        *
        * @param forEncryption if true the cipher is initialised for
        *  encryption, if false for decryption.
        * @param param the key and other data required by the cipher.
        * @exception ArgumentException if the parameters argument is
        * inappropriate.
        */
        public void Init(
            bool forEncryption,
            ICipherParameters parameters)
        {
            bool oldEncrypting = _encrypting;

            _encrypting = forEncryption;

            if (parameters is FastParametersWithIV ivParam)
            {
                byte[] iv = ivParam.GetIV();

                if (iv.Length != _blockSize)
                {
                    throw new ArgumentException("初始化向量的长度必须与块大小相同");
                }

                Array.Copy(iv, 0, _iv, 0, iv.Length);

                parameters = ivParam.Parameters;
            }

            Reset();

            // 如果为空，它只是静脉改变。
            if (parameters != null)
            {
                _cipher.Init(_encrypting, parameters);
            }
            else if (oldEncrypting != _encrypting)
            {
                throw new ArgumentException("在不提供密钥的情况下无法更改加密状态.");
            }
        }

        /**
        * 返回算法名称和模式。
        *
        * @return 基础算法的名称，后跟 "/CBC".
        */
        public string AlgorithmName => _cipher.AlgorithmName + "/CBC";

        public bool IsPartialBlockOkay => false;

        /**
        * 返回底层密码的块大小。
        *
        * @return 底层密码的块大小。
        */
        public int GetBlockSize()
        {
            return _cipher.GetBlockSize();
        }

        /**
        * 处理来自in数组的一个输入块，并将其写入out数组。
        *
        * @param 在包含输入数据的数组中。
        * @param inOff 偏移到数据起始位置的in数组中。
        * @param out the array the output data will be copied into.
        * @param outOff the offset into the out array the output will start at.
        * @exception DataLengthException 如果里面没有足够的数据，或者里面没有足够的空间。
        * @exception InvalidOperationException 如果密码没有初始化。
        * @return 处理和产生的字节数。
        */
        public int ProcessBlock(
            byte[] input,
            int inOff,
            byte[] output,
            int outOff)
        {
            return (_encrypting)
                ? EncryptBlock(input, inOff, output, outOff)
                : DecryptBlock(input, inOff, output, outOff);
        }

        /**
        *将链向量重置回IV并重置底层密码。
        */
        public void Reset()
        {
            Array.Copy(_iv, 0, _cbcV, 0, _iv.Length);
            Array.Clear(_cbcNextV, 0, _cbcNextV.Length);

            _cipher.Reset();
        }

        /**
        * 为CBC模式加密执行适当的链接步骤。
        *
        * <param name="input">in the array containing the data to be encrypted.</param>
        * <param name="inOff">
          偏移到数据起始位置的in数组中。
          将加密的数据复制到数组之外。
          </param>
        * <param name="outOff"> 输出开始位置的out数组的偏移量.</param>
        * <exception cref="DataLengthException">  如果里面没有足够的数据，或者里面没有足够的空间.</exception>
        * <returns>返回处理和产生的字节数。</returns>
        */
        private unsafe int EncryptBlock(
            byte[] input,
            int inOff,
            byte[] outBytes,
            int outOff)
        {
            if ((inOff + _blockSize) > input.Length)
            {
                throw new DataLengthException("输入缓冲区过短");
            }

            /*
            * XOR the cbcV and the input,
            * then encrypt the cbcV
            */
            //for (int i = 0; i < blockSize; i++)
            //{
            //    cbcV[i] ^= input[inOff + i];
            //}
            fixed (byte* pInput = input, pCbcV = _cbcV)
            {
                ulong* puLongInput = (ulong*)&pInput[inOff], puLongCbcV = (ulong*)pCbcV;

                for (int i = 0; i < _blockSize / 8; i++)
                {
                    puLongCbcV[i] ^= puLongInput[i];
                }
            }

            int length = _cipher.ProcessBlock(_cbcV, 0, outBytes, outOff);

            /*
            * copy ciphertext to cbcV
            */
            Array.Copy(outBytes, outOff, _cbcV, 0, _cbcV.Length);

            return length;
        }

        /**
         * 为CBC模式解密执行适当的链接步骤。
         *
         * <param name="input"> 在包含待解密数据的数组中。</param>
         * <param name="inOff">  偏移到数据起始位置的in数组中.</param>
         * <param name="outBytes">将解密的数据复制到数组之外.</param>
         * <param name="outOff">输出到输出数组的偏移量。</param>
         * <exception cref="DataLengthException">
         * 如果里面没有足够的数据，或者里面没有足够的空间。
         * </exception>
         * <exception cref="InvalidOperationException">
         *  如果密码没有初始化。
         * </exception>
         *  <returns>返回处理和产生的字节数.</returns>
         */
        private unsafe int DecryptBlock(
            byte[] input,
            int inOff,
            byte[] outBytes,
            int outOff)
        {
            if ((inOff + _blockSize) > input.Length)
            {
                throw new DataLengthException("输入缓冲区过短");
            }

            Array.Copy(input, inOff, _cbcNextV, 0, _blockSize);

            int length = _cipher.ProcessBlock(input, inOff, outBytes, outOff);

            /*
            * XOR the cbcV and the output
            */
            //for (int i = 0; i < blockSize; i++)
            //{
            //    outBytes[outOff + i] ^= cbcV[i];
            //}
            fixed (byte* poutBytes = outBytes, pCbcV = _cbcV)
            {
                ulong* puLongBytes = (ulong*)&poutBytes[outOff], puLongCbcV = (ulong*)pCbcV;

                for (int i = 0; i < _blockSize / 8; i++)
                {
                    puLongBytes[i] ^= puLongCbcV[i];
                }
            }

            /*
            * 将备份缓冲区交换到下一个位置
            */

            (_cbcV, _cbcNextV) = (_cbcNextV, _cbcV);

            return length;
        }
    }
}
#pragma warning restore
#endif
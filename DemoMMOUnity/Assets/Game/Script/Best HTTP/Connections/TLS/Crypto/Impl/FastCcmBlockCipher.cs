#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using System.IO;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Macs;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

namespace BestHTTP.Connections.TLS.Crypto.Impl
{
    public sealed class FastSicBlockCipher
        : IBlockCipher
    {
        private readonly IBlockCipher _cipher;
        private readonly int _blockSize;
        private readonly byte[] _counter;
        private readonly byte[] _counterOut;
        private byte[] _iv;

        /**
        * Basic constructor.
        *
        * @param c the block cipher to be used.
        */
        public FastSicBlockCipher(IBlockCipher cipher)
        {
            this._cipher = cipher;
            this._blockSize = cipher.GetBlockSize();
            this._counter = new byte[_blockSize];
            this._counterOut = new byte[_blockSize];
            this._iv = new byte[_blockSize];
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

        public void Init(
            bool forEncryption, //ignored by this CTR mode
            ICipherParameters parameters)
        {
            if (parameters is not FastParametersWithIV ivParam)
            {
                throw new ArgumentException("CTR/SIC模式需要ParametersWithIV", nameof(parameters));
            }

            this._iv = ivParam.GetIV();

            if (_blockSize < _iv.Length)
            {
                throw new ArgumentException($"CTR/SIC模式要求IV不大于: {_blockSize} bytes.");
            }

            int maxCounterSize = Math.Min(8, _blockSize / 2);
            if (_blockSize - _iv.Length > maxCounterSize)
            {
                throw new ArgumentException($"CTR/SIC模式要求IV至少:{(_blockSize - maxCounterSize)}bytes.");
            }

            // if null it's an IV changed only.
            if (ivParam.Parameters != null)
            {
                _cipher.Init(true, ivParam.Parameters);
            }

            Reset();
        }

        public string AlgorithmName => _cipher.AlgorithmName + "/SIC";

        public bool IsPartialBlockOkay => true;

        public int GetBlockSize()
        {
            return _cipher.GetBlockSize();
        }

        public int ProcessBlock(
            byte[] input,
            int inOff,
            byte[] output,
            int outOff)
        {
            _cipher.ProcessBlock(_counter, 0, _counterOut, 0);

            //
            // 用明文异或反输出产生密文
            //
            for (int i = 0; i < _counterOut.Length; i++)
            {
                output[outOff + i] = (byte)(_counterOut[i] ^ input[inOff + i]);
            }

            // Increment the counter
            int j = _counter.Length;
            while (--j >= 0 && ++_counter[j] == 0)
            {
            }

            return _counter.Length;
        }

        public void Reset()
        {
            Arrays.Fill(_counter, 0);
            Array.Copy(_iv, 0, _counter, 0, _iv.Length);
            _cipher.Reset();
        }
    }

    /**
    * 实现了密码块链模式(CCM)的计数器
    * NIST Special Publication 800-38C.
    * <p>
    * <b>Note</b>: 这种模式是一种分组模式——它需要预先准备所有的数据。
    * </p>
    */
    public sealed class FastCcmBlockCipher
        : IAeadBlockCipher
    {
        private static readonly int BlockSize = 16;

        private readonly IBlockCipher _cipher;
        private readonly byte[] _macBlock;
        private bool _forEncryption;
        private byte[] _nonce;
        private byte[] _initialAssociatedText;
        private int _macSize;
        private ICipherParameters _keyParam;
        private readonly MemoryStream _associatedText = new MemoryStream();
        private readonly MemoryStream _data = new MemoryStream();

        /**
        * 基本的构造函数。
        * <param name="cipher">对要使用的分组密码进行加密。</param>
        */
        public FastCcmBlockCipher(
            IBlockCipher cipher)
        {
            this._cipher = cipher;
            this._macBlock = new byte[BlockSize];

            if (cipher.GetBlockSize() != BlockSize)
            {
                throw new ArgumentException($"{BlockSize}块大小为的密码.");
            }
        }

        /**
        * 返回正在换行的底层分组密码。
        * <returns>返回正在换行的底层分组密码。</returns>
        */
        public IBlockCipher GetUnderlyingCipher()
        {
            return _cipher;
        }

        public void Init(
            bool forEncryption,
            ICipherParameters parameters)
        {
            this._forEncryption = forEncryption;

            ICipherParameters cipherParameters;
            if (parameters is FastAeadParameters)
            {
                // ReSharper disable once ExpressionIsAlwaysNull
                AeadParameters param = parameters as AeadParameters;

                // ReSharper disable once PossibleNullReferenceException
                _nonce = param.GetNonce();
                _initialAssociatedText = param.GetAssociatedText();
                _macSize = GetMacSize(forEncryption, param.MacSize);
                cipherParameters = param.Key;
            }
            else if (parameters is FastParametersWithIV iv)
            {
                _nonce = iv.GetIV();
                _initialAssociatedText = null;
                _macSize = GetMacSize(forEncryption, 64);
                cipherParameters = iv.Parameters;
            }
            else
            {
                throw new ArgumentException("传递给CCM的参数无效");
            }

            // NOTE: 非常基本的键重用支持，但没有性能提升
            if (cipherParameters != null)
            {
                _keyParam = cipherParameters;
            }

            if (_nonce == null || _nonce.Length < 7 || _nonce.Length > 13)
            {
                throw new ArgumentException("Nonce的长度必须在7到13个字节之间");
            }

            Reset();
        }

        public string AlgorithmName => _cipher.AlgorithmName + "/CCM";

        public int GetBlockSize()
        {
            return _cipher.GetBlockSize();
        }

        public void ProcessAadByte(byte input)
        {
            _associatedText.WriteByte(input);
        }

        public void ProcessAadBytes(byte[] inBytes, int inOff, int len)
        {
            // TODO: Process AAD online
            _associatedText.Write(inBytes, inOff, len);
        }

        public int ProcessByte(
            byte input,
            byte[] outBytes,
            int outOff)
        {
            _data.WriteByte(input);

            return 0;
        }

        public int ProcessBytes(
            byte[] inBytes,
            int inOff,
            int inLen,
            byte[] outBytes,
            int outOff)
        {
            Check.DataLength(inBytes, inOff, inLen, "Input buffer too short");

            _data.Write(inBytes, inOff, inLen);

            return 0;
        }

        public int DoFinal(
            byte[] outBytes,
            int outOff)
        {
#if PORTABLE || NETFX_CORE
            byte[] input = data.ToArray();
            int inLen = input.Length;
#else
            byte[] input = _data.GetBuffer();
            int inLen = (int)_data.Position;
#endif

            int len = ProcessPacket(input, 0, inLen, outBytes, outOff);

            Reset();

            return len;
        }

        public void Reset()
        {
            _cipher.Reset();
            _associatedText.SetLength(0);
            _data.SetLength(0);
        }

        /**
        * 返回一个字节数组，其中包含作为最后一次加密或解密操作的一部分计算的mac。
        * <returns>返回最后计算的MAC。</returns>
        * 
        */
        public byte[] GetMac()
        {
            return Arrays.CopyOfRange(_macBlock, 0, _macSize);
        }

        public int GetUpdateOutputSize(
            int len)
        {
            return 0;
        }

        public int GetOutputSize(
            int len)
        {
            int totalData = (int)_data.Length + len;

            if (_forEncryption)
            {
                return totalData + _macSize;
            }

            return totalData < _macSize ? 0 : totalData - _macSize;
        }

        /**
         * 处理一个数据包以进行CCM解密或加密。
         *
         * <param name="input">在数据中进行处理。</param>
         * <param name="inOff">输入数组中数据开始的inOff偏移量。</param>
         * <param name="inLen">inLen输入数组中数据的长度。</param>
         * <returns>包含已处理输入的字节数组。</returns>
         * <exception cref="IllegalStateException">如果密码设置不当。</exception>
         * <exception cref="InvalidCipherTextException">如果输入数据被截断或MAC检查失败。</exception>
         */
        public byte[] ProcessPacket(byte[] input, int inOff, int inLen)
        {
            byte[] output;

            if (_forEncryption)
            {
                output = new byte[inLen + _macSize];
            }
            else
            {
                if (inLen < _macSize)
                    throw new InvalidCipherTextException("data too short");

                output = new byte[inLen - _macSize];
            }

            ProcessPacket(input, inOff, inLen, output, 0);

            return output;
        }

        /**
         * 处理一个数据包以进行CCM解密或加密。
         *
         * <param name="input">在数据中进行处理</param>
         * <param name="inOff">输入数组中数据开始的inOff偏移量</param>
         * <param name="inLen">输入数组中数据的长度</param>
         * <param name="output">输出数组</param>
         * <param name="outOff">偏移量到输出数组以开始放置处理过的字节</param>
         * <returns>添加到输出中的字节数</returns>
         * <exception cref="IllegalStateException">如果密码设置不当</exception>
         * <exception cref="InvalidCipherTextException">如果输入数据被截断或MAC检查失败</exception>
         * <exception cref="DataLengthException">如果输出缓冲区过短</exception>
         */
        private int ProcessPacket(byte[] input, int inOff, int inLen, byte[] output, int outOff)
        {
            // TODO: handle null keyParam (e.g. via RepeatedKeySpec)
            // 需要保持CTR和CBC Mac部件周围和重置
            if (_keyParam == null)
            {
                throw new InvalidOperationException("CCM cipher initialized.");
            }

            int n = _nonce.Length;
            int q = 15 - n;
            if (q < 4)
            {
                int limitLen = 1 << (8 * q);
                if (inLen >= limitLen)
                {
                    throw new InvalidOperationException("CCM包太大，不能选择q.");
                }
            }

            byte[] iv = new byte[BlockSize];
            iv[0] = (byte)((q - 1) & 0x7);
            _nonce.CopyTo(iv, 1);

            IBlockCipher ctrCipher = new FastSicBlockCipher(_cipher);
            ctrCipher.Init(_forEncryption, new FastParametersWithIV(_keyParam, iv));

            int outputLen;
            int inIndex = inOff;
            int outIndex = outOff;

            if (_forEncryption)
            {
                outputLen = inLen + _macSize;
                Check.OutputLength(output, outOff, outputLen, "输出缓冲区过短.");

                CalculateMac(input, inOff, inLen, _macBlock);

                byte[] encMac = new byte[BlockSize];
                ctrCipher.ProcessBlock(_macBlock, 0, encMac, 0); // S0

                while (inIndex < (inOff + inLen - BlockSize)) // S1...
                {
                    ctrCipher.ProcessBlock(input, inIndex, output, outIndex);
                    outIndex += BlockSize;
                    inIndex += BlockSize;
                }

                byte[] block = new byte[BlockSize];

                Array.Copy(input, inIndex, block, 0, inLen + inOff - inIndex);

                ctrCipher.ProcessBlock(block, 0, block, 0);

                Array.Copy(block, 0, output, outIndex, inLen + inOff - inIndex);

                Array.Copy(encMac, 0, output, outOff + inLen, _macSize);
            }
            else
            {
                if (inLen < _macSize)
                    throw new InvalidCipherTextException("数据太短");

                outputLen = inLen - _macSize;
                Check.OutputLength(output, outOff, outputLen, "输出缓冲区过短.");

                Array.Copy(input, inOff + outputLen, _macBlock, 0, _macSize);

                ctrCipher.ProcessBlock(_macBlock, 0, _macBlock, 0);

                for (int i = _macSize; i != _macBlock.Length; i++)
                {
                    _macBlock[i] = 0;
                }

                while (inIndex < (inOff + outputLen - BlockSize))
                {
                    ctrCipher.ProcessBlock(input, inIndex, output, outIndex);
                    outIndex += BlockSize;
                    inIndex += BlockSize;
                }

                byte[] block = new byte[BlockSize];

                Array.Copy(input, inIndex, block, 0, outputLen - (inIndex - inOff));

                ctrCipher.ProcessBlock(block, 0, block, 0);

                Array.Copy(block, 0, output, outIndex, outputLen - (inIndex - inOff));

                byte[] calculatedMacBlock = new byte[BlockSize];

                CalculateMac(output, outOff, outputLen, calculatedMacBlock);

                if (!Arrays.ConstantTimeAreEqual(_macBlock, calculatedMacBlock))
                    throw new InvalidCipherTextException("mac check in CCM失败");
            }

            return outputLen;
        }

        private void CalculateMac(byte[] data, int dataOff, int dataLen, byte[] macBlock)
        {
            IMac cMac = new CbcBlockCipherMac(_cipher, _macSize * 8);

            cMac.Init(_keyParam);

            //
            // build b0
            //
            byte[] b0 = new byte[16];

            if (HasAssociatedText())
            {
                b0[0] |= 0x40;
            }

            b0[0] |= (byte)((((cMac.GetMacSize() - 2) / 2) & 0x7) << 3);

            b0[0] |= (byte)(((15 - _nonce.Length) - 1) & 0x7);

            Array.Copy(_nonce, 0, b0, 1, _nonce.Length);

            int q = dataLen;
            int count = 1;
            while (q > 0)
            {
                b0[^count] = (byte)(q & 0xff);
                q >>= 8;
                count++;
            }

            cMac.BlockUpdate(b0, 0, b0.Length);

            //
            // process associated text
            //
            if (HasAssociatedText())
            {
                int extra;

                int textLength = GetAssociatedTextLength();
                if (textLength < ((1 << 16) - (1 << 8)))
                {
                    cMac.Update((byte)(textLength >> 8));
                    cMac.Update((byte)textLength);

                    extra = 2;
                }
                else // can't go any higher than 2^32
                {
                    cMac.Update(0xff);
                    cMac.Update(0xfe);
                    cMac.Update((byte)(textLength >> 24));
                    cMac.Update((byte)(textLength >> 16));
                    cMac.Update((byte)(textLength >> 8));
                    // ReSharper disable once IntVariableOverflowInUncheckedContext
                    cMac.Update(input: (byte)textLength);

                    extra = 6;
                }

                if (_initialAssociatedText != null)
                {
                    cMac.BlockUpdate(_initialAssociatedText, 0, _initialAssociatedText.Length);
                }

                if (_associatedText.Position > 0)
                {
#if PORTABLE || NETFX_CORE
                    byte[] input = associatedText.ToArray();
                    int len = input.Length;
#else
                    byte[] input = _associatedText.GetBuffer();
                    int len = (int)_associatedText.Position;
#endif

                    cMac.BlockUpdate(input, 0, len);
                }

                extra = (extra + textLength) % 16;
                if (extra != 0)
                {
                    for (int i = extra; i < 16; ++i)
                    {
                        cMac.Update(0x00);
                    }
                }
            }

            //
            // add the text
            //
            cMac.BlockUpdate(data, dataOff, dataLen);

            cMac.DoFinal(macBlock, 0);
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private int GetMacSize(bool forEncryption, int requestedMacBits)
        {
            if (forEncryption && (requestedMacBits < 32 || requestedMacBits > 128 || 0 != (requestedMacBits & 15)))
            {
                throw new ArgumentException("tAg的八字节长度必须为{4,6,8,10,12,14,16}之一");
            }

            return requestedMacBits >> 3;
        }

        private int GetAssociatedTextLength()
        {
            return (int)_associatedText.Length + (_initialAssociatedText?.Length ?? 0);
        }

        private bool HasAssociatedText()
        {
            return GetAssociatedTextLength() > 0;
        }
    }
}
#pragma warning restore
#endif
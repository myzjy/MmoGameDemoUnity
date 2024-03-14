#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using BestHTTP.PlatformSupport.IL2CPP;
using BestHTTP.PlatformSupport.Memory;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes.Gcm;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

namespace BestHTTP.Connections.TLS.Crypto.Impl
{
    /// <summary>
    /// 实现Galois/Counter模式(GCM)
    /// NIST特别出版物800-38D。
    /// </summary>
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [Il2CppEagerStaticClassConstruction]
    public sealed class FastGcmBlockCipher
        : IAeadBlockCipher
    {
        private const int BlockSize = 16;
        private readonly byte[] _ctrBlock = new byte[BlockSize];

        private readonly IBlockCipher _cipher;
        private IGcmExponentiator _exp;

        // 这些字段由Init设置，不被processing修改
        private bool _forEncryption;
        private bool _initialised;
        private int _macSize;
        private byte[] _lastKey;
        private byte[] _nonce;
        private byte[] _initialAssociatedText;
        private byte[] _h;
        private byte[] _j0;

        // 这些字段在处理过程中被修改
        private int _bufLength;
        private byte[] _bufBlock;
        private byte[] _macBlock;
        private byte[] _s, _sAt, _sAtPre;
        private byte[] _counter;
        private uint _blocksRemaining;
        private int _bufOff;
        private ulong _totalLength;
        private byte[] _atBlock;
        private int _atBlockPos;
        private ulong _atLength;
        private ulong _atLengthPre;

        public FastGcmBlockCipher(
            IBlockCipher c,
            IGcmMultiplier m = null)
        {
            if (c.GetBlockSize() != BlockSize)
            {
                throw new ArgumentException($"{BlockSize} 块大小为的密码.");
            }

            if (m != null)
            {
                throw new NotImplementedException("IGcmMultiplier");
            }

            _cipher = c;
        }

        public string AlgorithmName => $"{_cipher.AlgorithmName}/GCM";

        public IBlockCipher GetUnderlyingCipher()
        {
            return _cipher;
        }

        public int GetBlockSize()
        {
            return BlockSize;
        }

        /// <remarks>
        /// MAC支持32位到128位(必须是8的倍数)缺省值是128位。
        ///不建议小于96的大小，但特殊应用程序支持。
        /// </remarks>
        public void Init(
            bool forEncryption,
            ICipherParameters parameters)
        {
            _forEncryption = forEncryption;
            //this.macBlock = null;
            if (_macBlock != null)
                Array.Clear(_macBlock, 0, _macBlock.Length);
            _initialised = true;

            NoCopyKeyParameter keyParam;
            byte[] newNonce;

            if (parameters is FastAeadParameters fastAeAdParameters)
            {
                newNonce = fastAeAdParameters.GetNonce();
                _initialAssociatedText = fastAeAdParameters.GetAssociatedText();

                int macSizeBits = fastAeAdParameters.MacSize;
                if (macSizeBits < 32 || macSizeBits > 128 || macSizeBits % 8 != 0)
                {
                    throw new ArgumentException($"MAC大小无效值: {macSizeBits}");
                }

                _macSize = macSizeBits / 8;
                keyParam = fastAeAdParameters.Key;
            }
            else if (parameters is FastParametersWithIV iv)
            {
                newNonce = iv.GetIV();
                _initialAssociatedText = null;
                _macSize = 16;
                keyParam = (NoCopyKeyParameter)iv.Parameters;
            }
            else
            {
                throw new ArgumentException("无效参数传递给GCM");
            }

            _bufLength = forEncryption ? BlockSize : (BlockSize + _macSize);
            if (_bufBlock == null || _bufLength < _bufBlock.Length)
                BufferPool.Resize(ref _bufBlock, _bufLength, true, true);

            if (newNonce == null || newNonce.Length < 1)
            {
                throw new ArgumentException("IV必须至少为1个字节");
            }

            if (forEncryption)
            {
                if (_nonce != null && Arrays.AreEqual(_nonce, newNonce))
                {
                    if (keyParam == null)
                    {
                        throw new ArgumentException("不能重用nonce进行GCM加密");
                    }

                    if (_lastKey != null && Arrays.AreEqual(_lastKey, keyParam.GetKey()))
                    {
                        throw new ArgumentException("不能重用nonce进行GCM加密");
                    }
                }
            }

            _nonce = newNonce;
            if (keyParam != null)
            {
                _lastKey = keyParam.GetKey();
            }

            // TODO Restrict macSize to 16 if nonce length not 12?

            //密码总是在转发模式中使用
            //如果keyParam为空，我们将重用最后一个键。
            if (keyParam != null)
            {
                _cipher.Init(true, keyParam);

                if (_h == null)
                    _h = new byte[BlockSize];
                else
                    Array.Clear(_h, 0, BlockSize);
                _cipher.ProcessBlock(_h, 0, _h, 0);

                // 如果keyParam为null，我们将重用最后一个键，乘数不需要重新初始化
                Tables8kGcmMultiplier_Init(_h);
                _exp = null;
            }
            else if (_h == null)
            {
                throw new ArgumentException("Key必须在initial init中指定");
            }

            if (_j0 == null)
                _j0 = new byte[BlockSize];
            else
                Array.Clear(_j0, 0, BlockSize);

            if (_nonce.Length == 12)
            {
                Array.Copy(_nonce, 0, _j0, 0, _nonce.Length);
                _j0[BlockSize - 1] = 0x01;
            }
            else
            {
                GHash(_j0, _nonce, _nonce.Length);
                byte[] x = BufferPool.Get(BlockSize, false);
                Pack.UInt64_To_BE((ulong)_nonce.Length * 8UL, x, 8);
                GHashBlock(_j0, x);
                BufferPool.Release(x);
            }

            //BufferPool.Resize(ref this.S, BlockSize, false, true);
            //BufferPool.Resize(ref this.S_at, BlockSize, false, true);
            //BufferPool.Resize(ref this.S_atPre, BlockSize, false, true);
            //BufferPool.Resize(ref this.atBlock, BlockSize, false, true);
            if (_s == null)
                _s = new byte[BlockSize];
            else
                Array.Clear(_s, 0, _s.Length);

            if (_sAt == null)
                _sAt = new byte[BlockSize];
            else
                Array.Clear(_sAt, 0, _sAt.Length);

            if (_sAtPre == null)
                _sAtPre = new byte[BlockSize];
            else
                Array.Clear(_sAtPre, 0, _sAtPre.Length);

            if (_atBlock == null)
                _atBlock = new byte[BlockSize];
            else
                Array.Clear(_atBlock, 0, _atBlock.Length);

            _atBlockPos = 0;
            _atLength = 0;
            _atLengthPre = 0;

            //this.counter = Arrays.Clone(J0);
            //BufferPool.Resize(ref this.counter, BlockSize, false, true);
            if (_counter == null)
                _counter = new byte[BlockSize];
            else
                Array.Clear(_counter, 0, _counter.Length);

            Array.Copy(_j0, 0, _counter, 0, BlockSize);

            _blocksRemaining = uint.MaxValue - 1; // page 8, len(P) <= 2^39 - 256, 1 block used by tag
            _bufOff = 0;
            _totalLength = 0;

            if (_initialAssociatedText != null)
            {
                ProcessAadBytes(_initialAssociatedText, 0, _initialAssociatedText.Length);
            }
        }

        public byte[] GetMac()
        {
            return _macBlock == null
                ? new byte[_macSize]
                : Arrays.Clone(_macBlock);
        }

        public int GetOutputSize(
            int len)
        {
            int totalData = len + _bufOff;

            if (_forEncryption)
            {
                return totalData + _macSize;
            }

            return totalData < _macSize ? 0 : totalData - _macSize;
        }

        public int GetUpdateOutputSize(
            int len)
        {
            int totalData = len + _bufOff;
            if (!_forEncryption)
            {
                if (totalData < _macSize)
                {
                    return 0;
                }

                totalData -= _macSize;
            }

            return totalData - totalData % BlockSize;
        }

        public void ProcessAadByte(byte input)
        {
            CheckStatus();

            _atBlock[_atBlockPos] = input;
            if (++_atBlockPos == BlockSize)
            {
                // Hash each block as it fills
                GHashBlock(_sAt, _atBlock);
                _atBlockPos = 0;
                _atLength += BlockSize;
            }
        }

        public void ProcessAadBytes(byte[] inBytes, int inOff, int len)
        {
            CheckStatus();

            for (int i = 0; i < len; ++i)
            {
                _atBlock[_atBlockPos] = inBytes[inOff + i];
                if (++_atBlockPos == BlockSize)
                {
                    // 当每个块填满时散列
                    GHashBlock(_sAt, _atBlock);
                    _atBlockPos = 0;
                    _atLength += BlockSize;
                }
            }
        }

        private void InitCipher()
        {
            if (_atLength > 0)
            {
                Array.Copy(_sAt, 0, _sAtPre, 0, BlockSize);
                _atLengthPre = _atLength;
            }

            // 完成部分AAD块的散列
            if (_atBlockPos > 0)
            {
                GHashPartial(_sAtPre, _atBlock, 0, _atBlockPos);
                _atLengthPre += (uint)_atBlockPos;
            }

            if (_atLengthPre > 0)
            {
                Array.Copy(_sAtPre, 0, _s, 0, BlockSize);
            }
        }

        public int ProcessByte(
            byte input,
            byte[] output,
            int outOff)
        {
            CheckStatus();

            _bufBlock[_bufOff] = input;
            if (++_bufOff == _bufLength)
            {
                ProcessBlock(_bufBlock, 0, output, outOff);
                if (_forEncryption)
                {
                    _bufOff = 0;
                }
                else
                {
                    Array.Copy(
                        sourceArray: _bufBlock,
                        sourceIndex: BlockSize,
                        destinationArray: _bufBlock,
                        destinationIndex: 0,
                        length: _macSize);
                    _bufOff = _macSize;
                }

                return BlockSize;
            }

            return 0;
        }

        public unsafe int ProcessBytes(
            byte[] input,
            int inOff,
            int len,
            byte[] output,
            int outOff)
        {
            CheckStatus();

            Check.DataLength(input, inOff, len, "输入缓冲区过短");

            var resultLen = 0;

            if (_forEncryption)
            {
                if (_bufOff != 0)
                {
                    while (len > 0)
                    {
                        --len;
                        _bufBlock[_bufOff] = input[inOff++];
                        if (++_bufOff != BlockSize) continue;
                        ProcessBlock(_bufBlock, 0, output, outOff);
                        _bufOff = 0;
                        resultLen += BlockSize;
                        break;
                    }
                }

                fixed (byte* pCtrBlock = _ctrBlock, pBuf = input, pS = _s, poutPut = output)
                {
                    while (len >= BlockSize)
                    {
                        // ProcessBlock(byte[] buf, int bufOff, byte[] output, int outOff)

                        #region ProcessBlock(buf: input, bufOff: inOff, output: output, outOff: outOff + resultLen);

                        if (_totalLength == 0)
                            InitCipher();

                        #region GetNextCtrBlock(ctrBlock);

                        _blocksRemaining--;

                        uint c = 1;
                        c += _counter[15];
                        _counter[15] = (byte)c;
                        c >>= 8;
                        c += _counter[14];
                        _counter[14] = (byte)c;
                        c >>= 8;
                        c += _counter[13];
                        _counter[13] = (byte)c;
                        c >>= 8;
                        c += _counter[12];
                        _counter[12] = (byte)c;

                        _cipher.ProcessBlock(_counter, 0, _ctrBlock, 0);

                        #endregion

                        ulong* pUlongBuf = (ulong*)&pBuf[inOff];
                        ulong* pUlongCtrBlock = (ulong*)pCtrBlock;
                        pUlongCtrBlock[0] ^= pUlongBuf[0];
                        pUlongCtrBlock[1] ^= pUlongBuf[1];

                        ulong* pUlongS = (ulong*)pS;
                        pUlongS[0] ^= pUlongCtrBlock[0];
                        pUlongS[1] ^= pUlongCtrBlock[1];

                        Tables8kGcmMultiplier_MultiplyH(_s);

                        ulong* pUlongOutput = (ulong*)&poutPut[outOff + resultLen];
                        pUlongOutput[0] = pUlongCtrBlock[0];
                        pUlongOutput[1] = pUlongCtrBlock[1];

                        _totalLength += BlockSize;

                        #endregion

                        inOff += BlockSize;
                        len -= BlockSize;
                        resultLen += BlockSize;
                    }
                }

                if (len > 0)
                {
                    Array.Copy(input, inOff, _bufBlock, 0, len);
                    _bufOff = len;
                }
            }
            else
            {
                fixed (byte* pInput = input, pBufBlock = _bufBlock, pCtrBlock = _ctrBlock, pS = _s, pOutput = output)
                {
                    ulong* pUlongBufBlock = (ulong*)pBufBlock;

                    // adjust bufOff to be on a 8 byte boundary
                    int adjustCount = 0;
                    for (int i = 0; i < len && (_bufOff % 8) != 0; ++i)
                    {
                        pBufBlock[_bufOff++] = pInput[inOff++ + i];
                        adjustCount++;

                        if (_bufOff != _bufLength) continue;
                        ProcessBlock(_bufBlock, 0, output, outOff + resultLen);

                        pUlongBufBlock[0] = pUlongBufBlock[2];
                        pUlongBufBlock[1] = pUlongBufBlock[3];

                        _bufOff = _macSize;
                        resultLen += BlockSize;
                    }

                    int longLen = (len - adjustCount) / 8;
                    if (longLen > 0)
                    {
                        ulong* pUlongInput = (ulong*)&pInput[inOff];

                        int bufLongOff = _bufOff / 8;

                        // copy 8 bytes per cycle instead of just 1
                        for (var i = 0; i < longLen; ++i)
                        {
                            pUlongBufBlock[bufLongOff++] = pUlongInput[i];
                            _bufOff += 8;

                            if (_bufOff != _bufLength) continue;

                            #region ProcessBlock(buf: bufBlock, bufOff: 0, output: output, outOff: outOff + resultLen);

                            if (_totalLength == 0)
                                InitCipher();

                            #region GetNextCtrBlock(ctrBlock);

                            _blocksRemaining--;

                            uint c = 1;
                            c += _counter[15];
                            _counter[15] = (byte)c;
                            c >>= 8;
                            c += _counter[14];
                            _counter[14] = (byte)c;
                            c >>= 8;
                            c += _counter[13];
                            _counter[13] = (byte)c;
                            c >>= 8;
                            c += _counter[12];
                            _counter[12] = (byte)c;

                            _cipher.ProcessBlock(_counter, 0, _ctrBlock, 0);

                            #endregion

                            ulong* pUlongS = (ulong*)pS;

                            pUlongS[0] ^= pUlongBufBlock[0];
                            pUlongS[1] ^= pUlongBufBlock[1];

                            Tables8kGcmMultiplier_MultiplyH(_s);

                            ulong* pUlongOutput = (ulong*)&pOutput[outOff + resultLen];
                            ulong* pUlongCtrBlock = (ulong*)pCtrBlock;

                            pUlongOutput[0] = pUlongCtrBlock[0] ^ pUlongBufBlock[0];
                            pUlongOutput[1] = pUlongCtrBlock[1] ^ pUlongBufBlock[1];

                            _totalLength += BlockSize;

                            #endregion

                            pUlongBufBlock[0] = pUlongBufBlock[2];
                            pUlongBufBlock[1] = pUlongBufBlock[3];

                            _bufOff = _macSize;
                            resultLen += BlockSize;

                            bufLongOff = _bufOff / 8;
                        }
                    }

                    for (int i = longLen * 8; i < len; i++)
                    {
                        pBufBlock[_bufOff++] = pInput[inOff + i];

                        if (_bufOff != _bufLength) continue;
                        ProcessBlock(_bufBlock, 0, output, outOff + resultLen);

                        pUlongBufBlock[0] = pUlongBufBlock[2];
                        pUlongBufBlock[1] = pUlongBufBlock[3];

                        _bufOff = _macSize;
                        resultLen += BlockSize;
                    }
                }
            }

            return resultLen;
        }

        private unsafe void ProcessBlock(byte[] buf, int bufferOff, byte[] output, int outOff)
        {
            if (_totalLength == 0)
                InitCipher();

            GetNextCtrBlock(_ctrBlock);

            if (_forEncryption)
            {
                fixed (byte* pCtrBlock = _ctrBlock, pBuf = buf, pS = _s)
                {
                    ulong* pUlongBuf = (ulong*)&pBuf[bufferOff];
                    ulong* pUlongCtrBlock = (ulong*)pCtrBlock;
                    pUlongCtrBlock[0] ^= pUlongBuf[0];
                    pUlongCtrBlock[1] ^= pUlongBuf[1];

                    ulong* pUlongS = (ulong*)pS;
                    pUlongS[0] ^= pUlongCtrBlock[0];
                    pUlongS[1] ^= pUlongCtrBlock[1];

                    Tables8kGcmMultiplier_MultiplyH(_s);

                    fixed (byte* pOutput = output)
                    {
                        ulong* pUlongOutput = (ulong*)&pOutput[outOff];
                        pUlongOutput[0] = pUlongCtrBlock[0];
                        pUlongOutput[1] = pUlongCtrBlock[1];
                    }
                }
            }
            else
            {
                //将此部分移动到Process Bytes是主要部分
                fixed (byte* pCtrBlock = _ctrBlock, pBuf = buf, pS = _s, pOutput = output)
                {
                    ulong* pUlongS = (ulong*)pS;
                    ulong* pUlongBuf = (ulong*)&pBuf[bufferOff];

                    pUlongS[0] ^= pUlongBuf[0];
                    pUlongS[1] ^= pUlongBuf[1];

                    Tables8kGcmMultiplier_MultiplyH(_s);

                    ulong* pUlongOutput = (ulong*)&pOutput[outOff];
                    ulong* pUlongCtrBlock = (ulong*)pCtrBlock;

                    pUlongOutput[0] = pUlongCtrBlock[0] ^ pUlongBuf[0];
                    pUlongOutput[1] = pUlongCtrBlock[1] ^ pUlongBuf[1];
                }
            }

            _totalLength += BlockSize;
        }

        public int DoFinal(byte[] output, int outOff)
        {
            CheckStatus();

            if (_totalLength == 0)
            {
                InitCipher();
            }

            int extra = _bufOff;

            if (_forEncryption)
            {
                Check.OutputLength(output, outOff, extra + _macSize, "输出缓冲区过短");
            }
            else
            {
                if (extra < _macSize)
                {
                    throw new InvalidCipherTextException("数据太短");
                }

                extra -= _macSize;

                Check.OutputLength(output, outOff, extra, "输出缓冲区过短");
            }

            if (extra > 0)
            {
                ProcessPartial(_bufBlock, 0, extra, output, outOff);
            }

            _atLength += (uint)_atBlockPos;

            if (_atLength > _atLengthPre)
            {
                /*
                 * 在密码开始后发送了一些AAD。我们确定b/w哈希值的差值
                 * 我们实际使用的密码开始时(S_atPre)和最终哈希值计算(S_at)。
                 * 然后我们通过乘以H^c向前携带这个差值，其中c是产生的(全部或部分)密文块的数量，并调整当前哈希
                 */

                // Finish hash for partial AAD block
                if (_atBlockPos > 0)
                {
                    GHashPartial(_sAt, _atBlock, 0, _atBlockPos);
                }

                // 找出AAD哈希值之间的差异
                if (_atLengthPre > 0)
                {
                    GcmUtilities.Xor(_sAt, _sAtPre);
                }

                // 产生的密文块数
                long c = (long)(((_totalLength * 8) + 127) >> 7);

                // 计算调整因子
                byte[] hC = BufferPool.Get(16, true);
                if (_exp == null)
                {
                    _exp = new Tables1kGcmExponentiator();
                    _exp.Init(_h);
                }

                _exp.ExponentiateX(c, hC);

                // 把差异发扬光大
                GcmUtilities.Multiply(_sAt, hC);

                // 调整当前哈希
                GcmUtilities.Xor(_s, _sAt);

                BufferPool.Release(hC);
            }

            // Final gHASH
            byte[] x = BufferPool.Get(BlockSize, false);
            Pack.UInt64_To_BE(_atLength * 8UL, x, 0);
            Pack.UInt64_To_BE(_totalLength * 8UL, x, 8);

            GHashBlock(_s, x);

            BufferPool.Release(x);

            // T = MSBt(GCTRk(J0,S))
            byte[] tag = BufferPool.Get(BlockSize, false);
            _cipher.ProcessBlock(_j0, 0, tag, 0);
            GcmUtilities.Xor(tag, _s);

            int resultLen = extra;

            // 我们将T的计算值放入macBlock中

            if (_macBlock == null || _macBlock.Length < _macSize)
            {
                _macBlock = BufferPool.Resize(ref _macBlock, _macSize, false, false);
            }

            Array.Copy(tag, 0, _macBlock, 0, _macSize);

            BufferPool.Release(tag);

            if (_forEncryption)
            {
                // Append T to the message
                Array.Copy(_macBlock, 0, output, outOff + _bufOff, _macSize);
                resultLen += _macSize;
            }
            else
            {
                // 从消息中检索T值并与计算的T值进行比较
                byte[] msgMac = BufferPool.Get(_macSize, false);
                Array.Copy(_bufBlock, extra, msgMac, 0, _macSize);
                if (!Arrays.ConstantTimeAreEqual(_macBlock, msgMac))
                    throw new InvalidCipherTextException("mac check in GCM failed");
                BufferPool.Release(msgMac);
            }

            Reset(false);

            return resultLen;
        }

        public void Reset()
        {
            Reset(true);
        }

        private unsafe void Reset(
            bool clearMac)
        {
            _cipher.Reset();

            // note: we do not reset the nonce.

            //BufferPool.Resize(ref this.S, BlockSize, false, true);
            //BufferPool.Resize(ref this.S_at, BlockSize, false, true);
            //BufferPool.Resize(ref this.S_atPre, BlockSize, false, true);
            //BufferPool.Resize(ref this.atBlock, BlockSize, false, true);
            fixed (byte* pS = _s, pSAt = _sAt, pSAtPre = _sAtPre, patBlock = _atBlock)
            {
                for (int i = 0; i < BlockSize; ++i)
                {
                    pS[i] = pSAt[i] = pSAtPre[i] = patBlock[i] = 0;
                }
            }

            _atBlockPos = 0;
            _atLength = 0;
            _atLengthPre = 0;

            //BufferPool.Resize(ref this.counter, BlockSize, false, false);
            Array.Copy(_j0, 0, _counter, 0, BlockSize);

            _blocksRemaining = uint.MaxValue - 1;
            _bufOff = 0;
            _totalLength = 0;

            if (_bufBlock != null)
            {
                //Arrays.Fill(bufBlock, 0);
            }

            if (clearMac)
            {
                //macBlock = null;
                Array.Clear(_macBlock, 0, _macSize);
            }

            if (_forEncryption)
            {
                _initialised = false;
            }
            else
            {
                if (_initialAssociatedText != null)
                {
                    ProcessAadBytes(_initialAssociatedText, 0, _initialAssociatedText.Length);
                }
            }
        }

        private void ProcessPartial(byte[] buf, int off, int len, byte[] output, int outOff)
        {
            //byte[] ctrBlock = new byte[BlockSize];
            GetNextCtrBlock(_ctrBlock);

            if (_forEncryption)
            {
                GcmUtilities.Xor(buf, off, _ctrBlock, 0, len);
                GHashPartial(_s, buf, off, len);
            }
            else
            {
                GHashPartial(_s, buf, off, len);
                GcmUtilities.Xor(buf, off, _ctrBlock, 0, len);
            }

            Array.Copy(buf, off, output, outOff, len);
            _totalLength += (uint)len;
        }

        private void GHash(byte[] y, byte[] b, int len)
        {
            for (int pos = 0; pos < len; pos += BlockSize)
            {
                int num = Math.Min(len - pos, BlockSize);
                GHashPartial(y, b, pos, num);
            }
        }

        private void GHashBlock(byte[] y, byte[] b)
        {
            GcmUtilities.Xor(y, b);
            Tables8kGcmMultiplier_MultiplyH(y);
        }

        private void GHashBlock(byte[] y, byte[] b, int off)
        {
            GcmUtilities.Xor(y, b, off);
            Tables8kGcmMultiplier_MultiplyH(y);
        }

        private void GHashPartial(byte[] y, byte[] b, int off, int len)
        {
            GcmUtilities.Xor(y, b, off, len);
            Tables8kGcmMultiplier_MultiplyH(y);
        }

        private void GetNextCtrBlock(byte[] block)
        {
            if (_blocksRemaining == 0)
                throw new InvalidOperationException("尝试处理过多的块");

            _blocksRemaining--;

            uint c = 1;
            c += _counter[15];
            _counter[15] = (byte)c;
            c >>= 8;
            c += _counter[14];
            _counter[14] = (byte)c;
            c >>= 8;
            c += _counter[13];
            _counter[13] = (byte)c;
            c >>= 8;
            c += _counter[12];
            _counter[12] = (byte)c;

            _cipher.ProcessBlock(_counter, 0, block, 0);
        }

        private void CheckStatus()
        {
            if (!_initialised)
            {
                if (_forEncryption)
                {
                    throw new InvalidOperationException("GCM密码不能用于加密");
                }

                throw new InvalidOperationException("GCM密码需要初始化");
            }
        }

        #region Tables8kGcmMultiplier

        private byte[] _tables8KGcmMultiplierH;
        private uint[][][] _tables8KGcmMultiplierM;

        private void Tables8kGcmMultiplier_Init(byte[] h)
        {
            if (_tables8KGcmMultiplierM == null)
            {
                _tables8KGcmMultiplierM = new uint[32][][];
            }
            else if (Arrays.AreEqual(_tables8KGcmMultiplierH, h))
            {
                return;
            }

            _tables8KGcmMultiplierH = Arrays.Clone(h);

            _tables8KGcmMultiplierM[0] = new uint[16][];
            _tables8KGcmMultiplierM[1] = new uint[16][];
            _tables8KGcmMultiplierM[0][0] = new uint[4];
            _tables8KGcmMultiplierM[1][0] = new uint[4];
            _tables8KGcmMultiplierM[1][8] = GcmUtilities.AsUints(h);

            for (int j = 4; j >= 1; j >>= 1)
            {
                uint[] tmp = (uint[])_tables8KGcmMultiplierM[1][j + j].Clone();
                GcmUtilities.MultiplyP(tmp);
                _tables8KGcmMultiplierM[1][j] = tmp;
            }

            {
                uint[] tmp = (uint[])_tables8KGcmMultiplierM[1][1].Clone();
                GcmUtilities.MultiplyP(tmp);
                _tables8KGcmMultiplierM[0][8] = tmp;
            }

            for (int j = 4; j >= 1; j >>= 1)
            {
                uint[] tmp = (uint[])_tables8KGcmMultiplierM[0][j + j].Clone();
                GcmUtilities.MultiplyP(tmp);
                _tables8KGcmMultiplierM[0][j] = tmp;
            }

            for (int i = 0;;)
            {
                for (int j = 2; j < 16; j += j)
                {
                    for (int k = 1; k < j; ++k)
                    {
                        uint[] tmp = (uint[])_tables8KGcmMultiplierM[i][j].Clone();
                        GcmUtilities.Xor(tmp, _tables8KGcmMultiplierM[i][k]);
                        _tables8KGcmMultiplierM[i][j + k] = tmp;
                    }
                }

                if (++i == 32) return;

                if (i > 1)
                {
                    _tables8KGcmMultiplierM[i] = new uint[16][];
                    _tables8KGcmMultiplierM[i][0] = new uint[4];
                    for (int j = 8; j > 0; j >>= 1)
                    {
                        uint[] tmp = (uint[])_tables8KGcmMultiplierM[i - 2][j].Clone();
                        GcmUtilities.MultiplyP8(tmp);
                        _tables8KGcmMultiplierM[i][j] = tmp;
                    }
                }
            }
        }

        private readonly uint[] _tables8KGcmMultiplierZ = new uint[4];

        private unsafe void Tables8kGcmMultiplier_MultiplyH(byte[] x)
        {
            fixed (byte* px = x)
            fixed (uint* pz = _tables8KGcmMultiplierZ)
            {
                ulong* pUlongZ = (ulong*)pz;
                pUlongZ[0] = 0;
                pUlongZ[1] = 0;

                for (int i = 15; i >= 0; --i)
                {
                    uint[] m = _tables8KGcmMultiplierM[i + i][px[i] & 0x0f];
                    fixed (uint* pm = m)
                    {
                        ulong* pUlongM = (ulong*)pm;

                        pUlongZ[0] ^= pUlongM[0];
                        pUlongZ[1] ^= pUlongM[1];
                    }

                    m = _tables8KGcmMultiplierM[i + i + 1][(px[i] & 0xf0) >> 4];
                    fixed (uint* pm = m)
                    {
                        ulong* pUlongM = (ulong*)pm;

                        pUlongZ[0] ^= pUlongM[0];
                        pUlongZ[1] ^= pUlongM[1];
                    }
                }

                byte* pByteZ = (byte*)pz;
                px[0] = pByteZ[3];
                px[1] = pByteZ[2];
                px[2] = pByteZ[1];
                px[3] = pByteZ[0];

                px[4] = pByteZ[7];
                px[5] = pByteZ[6];
                px[6] = pByteZ[5];
                px[7] = pByteZ[4];

                px[8] = pByteZ[11];
                px[9] = pByteZ[10];
                px[10] = pByteZ[9];
                px[11] = pByteZ[8];

                px[12] = pByteZ[15];
                px[13] = pByteZ[14];
                px[14] = pByteZ[13];
                px[15] = pByteZ[12];
            }
        }

        #endregion
    }
}
#pragma warning restore
#endif
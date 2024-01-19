﻿using System.Collections.Generic;
using ZJYFrameWork.Net.CsProtocol.Buffer;

namespace ZJYFrameWork.Net.CsProtocol
{
    /**
     * 协议包类注册
     *
     */
    public interface IProtocolRegistration
    {
        /**
         * 协议id
         *
         */
        short ProtocolId();

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="buffer">字节器</param>
        /// <param name="packet">包</param>
        void Write(ByteBuffer buffer, IPacket packet);

        /// <summary>
        /// 将传递的数据读取出来 通过字节器读取
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="json">解析的json 字典</param>
        /// <returns></returns>
        IPacket Read(ByteBuffer buffer);
    }
}
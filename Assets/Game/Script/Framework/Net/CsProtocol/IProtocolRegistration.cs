namespace ZJYFrameWork.Net.CsProtocol.Buffer
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
        /// <returns></returns>
        IPacket Read(ByteBuffer buffer);
    }
}
namespace ZJYFrameWork.Net.CsProtocol
{
    /// <summary>
    /// 网络包
    /// </summary>
    public interface IPacket
    {
        /// <summary>
        /// 协议号
        /// </summary>
        /// <returns></returns>
        short ProtocolId();
    }
}
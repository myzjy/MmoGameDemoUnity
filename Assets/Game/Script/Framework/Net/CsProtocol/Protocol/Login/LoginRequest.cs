namespace ZJYFrameWork.Net.CsProtocol.Buffer
{
    public class LoginRequest:IPacket
    {
        public short ProtocolId()
        {
            return 1000;
        }
    }
}
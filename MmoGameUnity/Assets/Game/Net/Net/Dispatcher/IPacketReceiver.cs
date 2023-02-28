using ZJYFrameWork.Net.CsProtocol.Buffer;

namespace ZJYFrameWork.Net.Dispatcher
{
    public interface IPacketReceiver
    {
        void Invoke(IPacket packet);
    }
}
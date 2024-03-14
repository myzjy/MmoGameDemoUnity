using ZJYFrameWork.Net.CsProtocol;

namespace ZJYFrameWork.Net.Dispatcher
{
    public interface IPacketReceiver
    {
        void Invoke(IPacket packet);
    }
}
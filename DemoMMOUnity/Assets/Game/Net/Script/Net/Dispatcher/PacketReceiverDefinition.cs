using System.Reflection;
using ZJYFrameWork.Net.CsProtocol.Buffer;

namespace ZJYFrameWork.Net.Dispatcher
{
    public class PacketReceiverDefinition : IPacketReceiver
    {
        private object bean;

        // 被PacketReceiver注解的方法
        private MethodInfo method;

        public void Invoke(IPacket packet)
        {
            method.Invoke(bean, new object[] { packet });
        }
        public static PacketReceiverDefinition ValueOf(object bean, MethodInfo method)
        {
            var definition = new PacketReceiverDefinition
            {
                bean = bean,
                method = method
            };
            return definition;
        }
    }
}
using ZJYFrameWork.Event;

namespace ZJYFrameWork.Net.Core.Model
{
    public class NetOpenEvent : IEvent
    {
        public static NetOpenEvent ValueOf()
        {
            return new NetOpenEvent();
        }
    }
}
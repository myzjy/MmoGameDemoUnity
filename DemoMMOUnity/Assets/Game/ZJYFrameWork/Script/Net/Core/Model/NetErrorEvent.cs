using ZJYFrameWork.Event;

namespace ZJYFrameWork.Net.Core.Model
{
    public class NetErrorEvent:IEvent
    {
        public static NetErrorEvent ValueOf()
        {
            var eve = new NetErrorEvent();
            return eve;
        }
    }
}
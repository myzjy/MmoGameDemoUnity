using ZJYFrameWork.Event;

namespace ZJYFrameWork.Net.Core.Model
{
    public class ResourceInitCompleteEvent: IEvent
    {
        public static ResourceInitCompleteEvent ValueOf()
        {
            return new ResourceInitCompleteEvent();
        }
    }
}
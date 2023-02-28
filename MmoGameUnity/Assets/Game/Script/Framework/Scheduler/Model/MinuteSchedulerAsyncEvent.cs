using ZJYFrameWork.Event;

namespace ZJYFrameWork.Scheduler.Model
{
    public class MinuteSchedulerAsyncEvent : IEvent
    {
        public static MinuteSchedulerAsyncEvent ValueOf()
        {
            return new MinuteSchedulerAsyncEvent();
        }
    }
}
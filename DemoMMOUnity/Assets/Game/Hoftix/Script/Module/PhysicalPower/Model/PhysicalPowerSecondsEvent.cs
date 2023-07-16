using ZJYFrameWork.Event;

namespace ZJYFrameWork.Module.PhysicalPower.Model
{
    /// <summary>
    /// 每秒 恢复体力
    /// </summary>
    public class PhysicalPowerSecondsEvent : IEvent
    {
        public long nowTime { get; set; }

        public static PhysicalPowerSecondsEvent ValueOf(long nowTime)
        {
            var eventData = new PhysicalPowerSecondsEvent
            {
                nowTime = nowTime
            };
            return eventData;
        }
    }
}
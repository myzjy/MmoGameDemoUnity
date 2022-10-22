using ZJYFrameWork.Base;
using ZJYFrameWork.Base.Model;
using ZJYFrameWork.Event;
using ZJYFrameWork.Scheduler.Model;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable.Manager;

namespace ZJYFrameWork.Scheduler
{
    /// <summary>
    /// 时间调度更新服务器最新时间
    /// </summary>
    [Bean]
    public class SchedulerManager : AbstractManager, ISchedulerManager
    {
        /// <summary>
        /// 最基础
        /// </summary>
        [Autowired] private BaseComponent baseComponent;

        private byte count;
        private long minuteSchedulerTimestamp = DateTimeUtil.Now();

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            //会有补位
            if (++count < baseComponent.frameRate)
            {
                return;
            }

            var now = DateTimeUtil.Now() + DateTimeUtil.MILLIS_PER_SECOND;
            DateTimeUtil.SetNow(now);
            count = 0;
            if (now - minuteSchedulerTimestamp > DateTimeUtil.MILLIS_PER_MINUTE)
            {
                minuteSchedulerTimestamp = now;
                //异步请求最新
                EventBus.AsyncSubmit(MinuteSchedulerAsyncEvent.ValueOf());

            }
        }

        public override void Shutdown()
        {
        }
    }
}
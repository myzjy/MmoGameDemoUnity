using System;
using System.Collections.Generic;
using ZJYFrameWork.Base;
using ZJYFrameWork.Base.Component;
using ZJYFrameWork.Base.Model;
using ZJYFrameWork.Event;
using ZJYFrameWork.Hotfix.Common;
using ZJYFrameWork.Hotfix.UI.GameMain;
using ZJYFrameWork.Module.PhysicalPower.Model;
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

        [Autowired] private GameMainUIController _gameMainUIController;

        public long serverTime = 0;

        private byte count;
        private long minuteSchedulerTimestamp = DateTimeUtil.Now();

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            //会有补位
            if (++count < baseComponent.frameRate)
            {
                return;
            }

            if (DateTimeUtil.Now() <= 0)
            {
                EventBus.AsyncSubmit(MinuteSchedulerAsyncEvent.ValueOf());
                // if (SpringContext.GetBean<PlayerUserCaCheData>().Uid > 0)
                // {
                //     EventBus.AsyncSubmit(PhysicalPowerSecondsEvent.ValueOf());
                // }

                return;
            }

            var nowCha = DateTimeUtil.Now() - serverTime;

            if (nowCha > DateTimeUtil.MILLIS_PER_SECOND * 3)
            {
                return;
            }

            var now = DateTimeUtil.Now() + DateTimeUtil.MILLIS_PER_SECOND;
            count = 0;
            DateTimeUtil.SetNow(now);
            _gameMainUIController.ShowNowTime(now);
            //定时
            if (now - minuteSchedulerTimestamp >= DateTimeUtil.MILLIS_PER_SECOND)
            {
                minuteSchedulerTimestamp = DateTimeUtil.Now();
                // if (SpringContext.GetBean<PlayerUserCaCheData>().Uid > 0)
                // {
                //     EventBus.AsyncSubmit(PhysicalPowerSecondsEvent.ValueOf(now));
                // }

                //异步请求最新
                EventBus.AsyncSubmit(MinuteSchedulerAsyncEvent.ValueOf());
            }
        }

        public override void Shutdown()
        {
        }
    }
}
using System;
using System.Collections.Generic;
using GameTools.Singletons;
// using ZJYFrameWork;

namespace ZJYFrameWork.Framwork.Times
{
    //定时器
    public class TimeTools : MMOSingletonDontDestroy<TimeTools>
    {
        private readonly int MaxExcuteCount = 3;
        private int count = 0;
        protected List<DelayMethod> lastDelayMethods = new List<DelayMethod>();
        protected List<DelayMethod> listDelayMethods = new List<DelayMethod>();

        private void Update()
        {
            for (int i = 0; i < listDelayMethods.Count; i++)
            {
                DelayMethod method = listDelayMethods[i];
                method.EndTime = DateTime.Now;
                if (method.StartTime.CompareTo(method.EndTime) <= 0)
                {
                    try
                    {
                        if (method.nowParms == null)
                        {
                            method.Methd();
                        }
                        else
                        {
                            method.Methds?.Invoke(method.nowParms);
                        }
                    }
                    finally
                    {
                        if (!this.lastDelayMethods.Contains(method))
                        {
                            lastDelayMethods.Add(method);
                        }
                    }

                    count++;
                    if (count >= MaxExcuteCount)
                    {
                        count = 0;
                        break;
                    }
                }
            }

            for (int i = 0; i < lastDelayMethods.Count; i++)
            {
                listDelayMethods.Remove(lastDelayMethods[i]);
            }

            lastDelayMethods.Clear();
            if (updateEvent != null)
            {
                updateEvent.Invoke();
            }
        }

        private event Action updateEvent;

        public event Action GetUpdateEvent
        {
            add => updateEvent += value;
            remove => updateEvent -= value;
        }

#pragma warning disable CS0067
        private event Action LateUpdateEvent;
#pragma warning restore CS0067

        public event Action GetLastUpdateEvent
        {
            add => LateUpdateEvent += value;
            remove => LateUpdateEvent -= value;
        }

        #region Public Method

        /// <summary>
        /// 延迟多少秒后触发 无回调 无参数
        /// </summary>
        /// <param name="action">执行方法</param>
        /// <param name="second">延迟时间</param>
        /// <returns>返回延迟方法</returns>
        public DelayMethod TimeInvoke(Action action, float second)
        {
            return TimeInvoke(action, null, null, second);
        }

        /// <summary>
        /// 延迟多少秒后触发 无回调 带参数
        /// </summary>
        /// <param name="action">执行方法</param>
        /// <param name="inParams">参数</param>
        /// <param name="second">延迟时间</param>
        /// <returns>返回延迟方法</returns>
        public DelayMethod TimeInvoke(Action<object> action, object inParams, float second)
        {
            return TimeInvoke(action: action, inParams: inParams, second: second, paramsComplete: null,
                objParams: null);
        }

        /// <summary>
        /// 延迟多少 秒后触发
        /// </summary>
        /// <param name="paramsComplete">执行方法</param>
        /// <param name="action">延迟回调</param>
        /// <param name="inParams">参数</param>
        /// <param name="second">延迟时间</param>
        /// <returns></returns>
        public DelayMethod TimeInvoke(Action paramsComplete, Action<object> action, object inParams, float second)
        {
            DelayMethod delayMethod = new DelayMethod()
            {
                Methd = paramsComplete,
                nowParms = inParams,
                CompleteHandler = action,
                StartTime = DateTime.Now.AddSeconds(second),
                Second = second
            };
            listDelayMethods.Add(delayMethod);
            return delayMethod;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramsComplete"></param>
        /// <param name="action"></param>
        /// <param name="inParams"></param>
        /// <param name="objParams"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public DelayMethod TimeInvoke(Action paramsComplete, Action<object> action, object inParams, object objParams,
            float second)
        {
            DelayMethod delayMethod = new DelayMethod()
            {
                Methd = paramsComplete,
                nowParms = inParams,
                CallbackParms = objParams,
                CompleteHandler = action,
                StartTime = DateTime.Now.AddSeconds(second),
                Second = second
            };
            listDelayMethods.Add(delayMethod);
            return delayMethod;
        }

        #endregion
    }
}
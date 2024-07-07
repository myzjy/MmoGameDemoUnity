using System;
using System.Collections.Generic;
using System.Linq;
using BestHTTP.Core;

namespace BestHTTP.Timings
{
    public sealed class TimingCollector
    {
        public TimingCollector(HttpRequest parentRequest)
        {
            this.ParentRequest = parentRequest;
            this.Start = DateTime.Now;
        }

        private HttpRequest ParentRequest { get; }

        /// <summary>
        /// 创建TimingCollector实例时。
        /// </summary>
        public DateTime Start { get; private set; }

        /// <summary>
        /// 已添加事件的列表。
        /// </summary>
        private List<TimingEvent> Events { get; set; }

        internal void AddEvent(string name, DateTime when, TimeSpan duration)
        {
            this.Events ??= new List<TimingEvent>();

            if (duration == TimeSpan.Zero)
            {
                DateTime prevEventAt = this.Start;
                if (this.Events.Count > 0)
                {
                    prevEventAt = this.Events[^1].When;
                }

                duration = when - prevEventAt;
            }

            this.Events.Add(new TimingEvent(name, when, duration));
        }

        /// <summary>
        /// 添加事件。持续时间是从收集器的前一个事件或开始计算的。
        /// </summary>
        public void Add(string name)
        {
            RequestEventHelper.EnqueueRequestEvent(new RequestEventInfo(this.ParentRequest, name, DateTime.Now));
        }

        /// <summary>
        /// 添加一个已知持续时间的事件。
        /// </summary>
        public void Add(string name, TimeSpan duration)
        {
            var eventInfo = new RequestEventInfo(
                request: this.ParentRequest,
                name: name,
                duration: duration);
            RequestEventHelper.EnqueueRequestEvent(@event: eventInfo);
        }

        public TimingEvent FindFirst(string name)
        {
            if (this.Events == null)
                return TimingEvent.Empty;

            for (int i = 0; i < this.Events.Count; ++i)
            {
                if (this.Events[i].Name == name)
                {
                    return this.Events[i];
                }
            }

            return TimingEvent.Empty;
        }

        public TimingEvent FindLast(string name)
        {
            if (this.Events == null)
            {
                return TimingEvent.Empty;
            }

            for (var i = this.Events.Count - 1; i >= 0; --i)
            {
                if (this.Events[i].Name == name)
                {
                    return this.Events[i];
                }
            }

            return TimingEvent.Empty;
        }

        public override string ToString()
        {
            string result = $"[TimingCollector Start: '{this.Start.ToLongTimeString()}' ";

            if (this.Events != null)
            {
                result = this.Events.Aggregate(result, (current, @event) => $"{current}\n{@event.ToString()}");
            }

            result += "]";

            return result;
        }
    }
}
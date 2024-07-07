using System;

namespace BestHTTP.Timings
{
    public readonly struct TimingEvent : IEquatable<TimingEvent>
    {
        public static readonly TimingEvent Empty = new TimingEvent(null, TimeSpan.Zero);

        /// <summary>
        /// 事件名称
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// 事件的持续时间。
        /// </summary>
        private readonly TimeSpan _duration;

        /// <summary>
        /// 事件发生的时间。
        /// </summary>
        public readonly DateTime When;

        private TimingEvent(string name, TimeSpan duration)
        {
            this.Name = name;
            this._duration = duration;
            this.When = DateTime.Now;
        }

        public TimingEvent(string name, DateTime when, TimeSpan duration)
        {
            this.Name = name;
            this.When = when;
            this._duration = duration;
        }

        public TimeSpan CalculateDuration(TimingEvent @event)
        {
            if (this.When < @event.When)
            {
                return @event.When - this.When;
            }

            return this.When - @event.When;
        }

        public bool Equals(TimingEvent other)
        {
            return this.Name == other.Name &&
                   this._duration == other._duration &&
                   this.When == other.When;
        }

        public override bool Equals(object obj)
        {
            if (obj is TimingEvent @event)
            {
                return this.Equals(@event);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (this.Name != null ? this.Name.GetHashCode() : 0) ^
                   this._duration.GetHashCode() ^
                   this.When.GetHashCode();
        }

        public static bool operator ==(TimingEvent lhs, TimingEvent rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(TimingEvent lhs, TimingEvent rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override string ToString()
        {
            return $"['{this.Name}': {this._duration}]";
        }
    }
}
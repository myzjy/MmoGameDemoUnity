using System;

namespace ZJYFrameWork.UISerializable.Manager
{
    public class TimesManager
    {
        private DateTime receiveSeverTimes = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private DateTime lastSeverTimes = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime LOCAL_EPOCH = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);

        public DateTime CurrentTime
        {
            get
            {
                if (receiveSeverTimes < UNIX_EPOCH)
                {
                    return default(DateTime);
                }

                var diff = (DateTime.UtcNow - lastSeverTimes).TotalSeconds;
                return diff < 0 ? default(DateTime) : receiveSeverTimes.AddSeconds(diff);
            }
        }

        public DateTime GetCurrEntTime(long timesLong)
        {
            return LOCAL_EPOCH.AddSeconds(timesLong).ToUniversalTime();
        }

        public DateTime GetCurrEntTimeMilliseconds(long timesLong)
        {
            return LOCAL_EPOCH.AddMilliseconds(timesLong);
        }

        public void SetServerTime(DateTime time)
        {
            receiveSeverTimes = time;
            lastSeverTimes = DateTime.UtcNow;
        }
    }

    public static class DateTimeUtil
    {
        public static DateTime NotStarted = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        public static DateTime FromUnixTimeStamp(double timestamp)
        {
            return epoch.AddSeconds(timestamp);
        }
        public static double ToUnixTimeStamp(this DateTime time)
        {
            return (time - epoch).TotalSeconds;
        }

        public static bool IsValid(this DateTime time)
        {
            return time != NotStarted && time != DateTime.MinValue;
        }
    }
}
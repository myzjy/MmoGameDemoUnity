using System;

namespace ZJYFrameWork.Framwork.Times
{
    public class DelayMethod
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public float Second { get; set; }
        public  Action Methd { get;set; }
        public Action<object> Methds { get; set; }
        public object nowParms { get; set; }
        public Action<object> CompleteHandler { get; set; }
        public object CallbackParms { get; set; }
    }
}
using System;

namespace ZJYFrameWork.AssetBundles.Bundles
{
    public class LoopingReferenceException : Exception
    {
        public LoopingReferenceException()
        {
        }

        public LoopingReferenceException(string message) : base(message)
        {
        }

        public LoopingReferenceException(Exception exception) : base("", exception)
        {
        }

        public LoopingReferenceException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}

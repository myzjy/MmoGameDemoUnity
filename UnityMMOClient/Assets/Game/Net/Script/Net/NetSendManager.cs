using System;
using System.Collections.Concurrent;
using BestHTTP.Extensions;
using ZJYFrameWork.Base.Model;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.Net
{
    [Bean]
    public class NetSendManager : AbstractManager,INetSendManager
    {
        protected ConcurrentQueue<Action> receiveQueue = new ConcurrentQueue<Action>();

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            Action action;
            while (receiveQueue.TryDequeue(out action))
            {
                action.Invoke();
            }
        }

        public override void Shutdown()
        {
            // receiveQueue
        }

        public void Add(Action action)
        {
            receiveQueue.Enqueue(action);
        }
    }
}
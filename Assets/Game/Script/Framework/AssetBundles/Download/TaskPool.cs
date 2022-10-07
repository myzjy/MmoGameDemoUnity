using System;
using System.Collections.Generic;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.Collection;
using ZJYFrameWork.Collection.Reference;

namespace ZJYFrameWork.AssetBundles.Download
{
    sealed class TaskPool<T> where T : TaskBase
    {
        private readonly Stack<T> freeAgents = new Stack<T>();
        private readonly CachedLinkedList<T> workingAgents = new CachedLinkedList<T>();

        private readonly CachedLinkedList<T> waitingTasks = new CachedLinkedList<T>();

        /// <summary>
        /// 获取可用任务代理数量。
        /// </summary>
        public int FreeAgentCount
        {
            get { return freeAgents.Count; }
        }

        /// <summary>
        /// 任务池轮询。主要用于时间
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        /// <summary>
        /// 关闭并清理任务池。
        /// </summary>
        public void Shutdown()
        {
            RemoveAllTasks();

            while (FreeAgentCount > 0)
            {
                freeAgents.Pop().Clear();
            }
        }

        /// <summary>
        /// 移除所有任务。
        /// </summary>
        public void RemoveAllTasks()
        {
            foreach (T task in waitingTasks)
            {
                ReferenceCache.Release(task);
            }

            waitingTasks.Clear();

            foreach (T workingAgent in workingAgents)
            {
                T task = workingAgent;
                // workingAgent.Reset();
                freeAgents.Push(workingAgent);
                ReferenceCache.Release(task);
            }

            workingAgents.Clear();
        }

        /// <summary>
        /// 增加任务代理。
        /// </summary>
        /// <param name="agent">要增加的任务代理。</param>
        /// <param name="bundleInfo"></param>
        public void AddAgent(T agent, BundleInfo bundleInfo)
        {
            if (agent == null)
            {
                throw new Exception("Task agent is invalid.");
            }

            agent.Initialize(bundleInfo, 0);
            freeAgents.Push(agent);
        }

        /// <summary>
        /// 增加任务。
        /// </summary>
        /// <param name="task">要增加的任务。</param>
        public void AddTask(T task)
        {
            LinkedListNode<T> current = waitingTasks.Last;
            while (current != null)
            {
                if (task.Priority <= current.Value.Priority)
                {
                    break;
                }

                current = current.Previous;
            }

            if (current != null)
            {
                waitingTasks.AddAfter(current, task);
            }
            else
            {
                waitingTasks.AddFirst(task);
            }
        }
        // public void 
    }
}
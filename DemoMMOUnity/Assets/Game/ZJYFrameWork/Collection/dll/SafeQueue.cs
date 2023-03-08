﻿// Net 4.X 有ConcurrentQueue，但是ConcurrentQueue没有TryDequeueAll方法，
// 这使得SafeQueue发送线程的速度提高了一倍。
//
// uMMORPG 450 CCU
//   SafeQueue:       900-1440ms latency
//   ConcurrentQueue:     2000ms latency
//
// 这在LoadTest项目中也很明显，它很难用ConcurrentQueue处理300个CCU !

using System.Collections.Generic;

namespace ZJYFrameWork.Collection
{
    /// <summary>
    /// 内部有着线程操作，必须注意防止出错
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SafeQueue<T>
    {
        readonly Queue<T> queue = new Queue<T>();

        public int Count
        {
            get
            {
                //线程访问操作需要对资源进行锁操作避免多线程操作出现访问下标问题
                lock (queue)
                {
                    return queue.Count;
                }
            }
        }

        public void Enqueue(T item)
        {
            //线程访问操作需要对资源进行锁操作避免多线程操作出现访问下标问题
            lock (queue)
            {
                queue.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            //线程访问操作需要对资源进行锁操作避免多线程操作出现访问下标问题
            lock (queue)
            {
                if (queue.Count == 0)
                {
                    return default(T);
                }

                return queue.Dequeue();
            }
        }

        // 当我们想要退出队列并一次删除所有的时候
        // 锁定每一个TryDequeue。
        public bool TryDequeueAll(out T[] result)
        {
            //线程访问操作需要对资源进行锁操作避免多线程操作出现访问下标问题
            lock (queue)
            {
                result = queue.ToArray();
                queue.Clear();
                return result.Length > 0;
            }
        }

        public void Clear()
        {
            //线程访问操作需要对资源进行锁操作避免多线程操作出现访问下标问题
            lock (queue)
            {
                queue.Clear();
            }
        }
    }
}
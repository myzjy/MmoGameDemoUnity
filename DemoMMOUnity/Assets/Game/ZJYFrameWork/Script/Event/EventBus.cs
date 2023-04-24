using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using ZJYFrameWork.Net;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.Spring.Utils;
using ZJYFrameWork.WebRequest;

namespace ZJYFrameWork.Event
{
    public abstract class EventBus
    {
        private static readonly Dictionary<Type, ICollection<IEventReceiver>> receiverMap =
            new Dictionary<Type, ICollection<IEventReceiver>>();

        static EventBus()
        {
            ThreadPool.SetMinThreads(2, 2);
            ThreadPool.SetMaxThreads(8, 8);
        }

        private EventBus()
        {
        }

        /// <summary>
        /// 扫描 所有bean注解
        /// </summary>
        public static void Scan()
        {
            var allBeans = SpringContext.GetAllBeans();
            foreach (var bean in allBeans)
            {
                RegisterEventReceiver(bean);
            }
        }

        /// <summary>
        /// 扫描并注册 EventReceiver注解
        /// </summary>
        /// <param name="bean"></param>
        private static void RegisterEventReceiver(object bean)
        {
            var classType = bean.GetType();
            var methods = AssemblyUtils.GetMethodsByAnnoInPOJOClass(classType, typeof(EventReceiverAttribute));
            foreach (var item in methods)
            {
                var parameters = item.GetParameters();
                if (parameters.Length != 1)
                {
                    throw new Exception(StringUtils.Format("[class:{}] [method:{}] must have one parameter!",
                        bean.GetType().Name, item.Name));
                }

                if (!typeof(IEvent).IsAssignableFrom(parameters[0].ParameterType))
                {
                    throw new Exception(StringUtils.Format(
                        "[class:{}] [method:{}] must have one [IEvent] type parameter!",
                        bean.GetType().Name, item.Name));
                }

                var paramType = item.GetParameters()[0].ParameterType;
                var expectedMethodName = StringUtils.Format("On{}", paramType.Name);
                if (!item.Name.Equals(expectedMethodName))
                {
                    throw new Exception(StringUtils.Format(
                        "[class:{}] [method:{}] [event:{}] expects '{}' as method name!", bean.GetType().FullName,
                        item.Name, paramType.Name, expectedMethodName));
                }

                var receiverDefinition = EventReceiverDefinition.ValueOf(bean, item);
                if (!receiverMap.ContainsKey(paramType))
                {
                    receiverMap.Add(paramType, new LinkedList<IEventReceiver>());
                }

                receiverMap[paramType].Add(receiverDefinition);
            }
        }

        public static void SyncSubmit(IEvent eve)
        {
            receiverMap.TryGetValue(eve.GetType(), out var list);

            if (CollectionUtils.IsEmpty(list))
            {
                return;
            }

            DoSubmit(eve, list);
        }

        public static void AsyncSubmit(IEvent eve)
        {
            receiverMap.TryGetValue(eve.GetType(), out var list);

            if (CollectionUtils.IsEmpty(list))
            {
                return;
            }

            AsyncExecute(() => DoSubmit(eve, list));
        }

        public static void AsyncExecute(Action action)
        {
            if (action == null)
            {
                return;
            }

#if UNITY_WEBGL
            // 如果是webgl则直接执行，因为webgl不支持线程操作
            action();
#else
            SpringContext.GetApplicationContext().GetMainLoopExcutor().RunOnMainThread(action);
            //SpringContext.GetBean<NetSendManager>().Add(action);
            // ThreadPool.QueueUserWorkItem((param) =>
            // {
            //     try
            //     {
            //         action();
            //     }
            //     catch (Exception e)
            //     {
            //         Debug.LogError(e);
            //     }
            // });
#endif
        }

        private static void DoSubmit(IEvent eve, ICollection<IEventReceiver> listReceiver)
        {
            foreach (var receiver in listReceiver)
            {
                try
                {
                    receiver.Invoke(eve);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
    }
}
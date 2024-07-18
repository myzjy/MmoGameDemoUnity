﻿using System;
using System.Collections.Generic;

namespace FrostEngine
{
    /// <summary>
    /// 封装消息的底层分发和注册。
    /// </summary>
    public class EventDispatcher
    {
        /// <summary>
        /// 事件Table。
        /// </summary>
        private static readonly Dictionary<object, EventDelegateData> _eventTable =
            new Dictionary<object, EventDelegateData>();

        #region 事件管理接口

        /// <summary>
        /// 增加事件监听。
        /// </summary>
        /// <param name="eventType">事件类型。</param>
        /// <param name="handler">事件处理委托。</param>
        /// <returns>是否添加成功。</returns>
        public bool AddEventListener(object eventType, object ownerPlayer, Delegate handler)
        {
            if (!_eventTable.TryGetValue(eventType, out var data))
            {
                data = new EventDelegateData(eventType, ownerPlayer);
                _eventTable.Add(eventType, data);
            }

            return data.AddHandler(handler);
        }

        /// <summary>
        /// 移除事件监听。
        /// </summary>
        /// <param name="eventType">事件类型。</param>
        /// <param name="handler">事件处理委托。</param>
        public void RemoveEventListener(object eventType, Delegate handler)
        {
            if (_eventTable.TryGetValue(eventType, out var data))
            {
                data.RmvHandler(handler);
            }
        }

        #endregion

        #region 事件分发接口

        /// <summary>
        /// 发送事件。
        /// </summary>
        /// <param name="eventType">事件类型。</param>
        public void Send(object eventType)
        {
            if (_eventTable.TryGetValue(eventType, out var d))
            {
                if (d.GetOwner() != null)
                {
                    d.Callback(d.GetOwner());
                }
                else
                {
                    d.Callback();
                }
            }
        }

        /// <summary>
        /// 发送事件。
        /// </summary>
        /// <param name="eventType">事件类型。</param>
        /// <param name="arg1">事件参数1。</param>
        /// <typeparam name="TArg1">事件参数1类型。</typeparam>
        public void Send<TArg1>(object eventType, TArg1 arg1)
        {
            if (_eventTable.TryGetValue(eventType, out var d))
            {
                if (d.GetOwner() != null)
                {
                    d.Callback(d.GetOwner(), arg1);
                }
                else
                {
                    d.Callback(arg1);
                }
            }
        }

        /// <summary>
        /// 发送事件。
        /// </summary>
        /// <param name="eventType">事件类型。</param>
        /// <param name="arg1">事件参数1。</param>
        /// <param name="arg2">事件参数2。</param>
        /// <typeparam name="TArg1">事件参数1类型。</typeparam>
        /// <typeparam name="TArg2">事件参数2类型。</typeparam>
        public void Send<TArg1, TArg2>(object eventType, TArg1 arg1, TArg2 arg2)
        {
            if (_eventTable.TryGetValue(eventType, out var d))
            {
                if (d.GetOwner() != null)
                {
                    d.Callback(d.GetOwner(), arg1, arg2);
                }
                else
                {
                    d.Callback(arg1, arg2);
                }
            }
        }

        /// <summary>
        /// 发送事件。
        /// </summary>
        /// <param name="eventType">事件类型。</param>
        /// <param name="arg1">事件参数1。</param>
        /// <param name="arg2">事件参数2。</param>
        /// <param name="arg3">事件参数3。</param>
        /// <typeparam name="TArg1">事件参数1类型。</typeparam>
        /// <typeparam name="TArg2">事件参数2类型。</typeparam>
        /// <typeparam name="TArg3">事件参数3类型。</typeparam>
        public void Send<TArg1, TArg2, TArg3>(object eventType, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            if (_eventTable.TryGetValue(eventType, out var d))
            {
                if (d.GetOwner() != null)
                {
                    d.Callback(d.GetOwner(), arg1, arg2, arg3);
                }
                else
                {
                    d.Callback(arg1, arg2, arg3);
                }
            }
        }

        /// <summary>
        /// 发送事件。
        /// </summary>
        /// <param name="eventType">事件类型。</param>
        /// <param name="arg1">事件参数1。</param>
        /// <param name="arg2">事件参数2。</param>
        /// <param name="arg3">事件参数3。</param>
        /// <param name="arg4">事件参数4。</param>
        /// <typeparam name="TArg1">事件参数1类型。</typeparam>
        /// <typeparam name="TArg2">事件参数2类型。</typeparam>
        /// <typeparam name="TArg3">事件参数3类型。</typeparam>
        /// <typeparam name="TArg4">事件参数4类型。</typeparam>
        public void Send<TArg1, TArg2, TArg3, TArg4>(object eventType, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
        {
            if (_eventTable.TryGetValue(eventType, out var d))
            {
                if (d.GetOwner() != null)
                {
                    d.Callback(d.GetOwner(), arg1, arg2, arg3, arg4);
                }
                else
                {
                    d.Callback(arg1, arg2, arg3, arg4);
                }
            }
        }

        /// <summary>
        /// 发送事件。
        /// </summary>
        /// <param name="eventType">事件类型。</param>
        /// <param name="arg1">事件参数1。</param>
        /// <param name="arg2">事件参数2。</param>
        /// <param name="arg3">事件参数3。</param>
        /// <param name="arg4">事件参数4。</param>
        /// <param name="arg5">事件参数5。</param>
        /// <typeparam name="TArg1">事件参数1类型。</typeparam>
        /// <typeparam name="TArg2">事件参数2类型。</typeparam>
        /// <typeparam name="TArg3">事件参数3类型。</typeparam>
        /// <typeparam name="TArg4">事件参数4类型。</typeparam>
        /// <typeparam name="TArg5">事件参数5类型。</typeparam>
        public void Send<TArg1, TArg2, TArg3, TArg4, TArg5>(object eventType, TArg1 arg1, TArg2 arg2, TArg3 arg3,
            TArg4 arg4, TArg5 arg5)
        {
            if (_eventTable.TryGetValue(eventType, out var d))
            {
                if (d.GetOwner() != null)
                {
                    d.Callback(d.GetOwner(), arg1, arg2, arg3, arg4, arg5);
                }
                else
                {
                    d.Callback(arg1, arg2, arg3, arg4, arg5);
                }
            }
        }

        /// <summary>
        /// 发送事件。
        /// </summary>
        /// <param name="eventType">事件类型。</param>
        /// <param name="arg1">事件参数1。</param>
        /// <param name="arg2">事件参数2。</param>
        /// <param name="arg3">事件参数3。</param>
        /// <param name="arg4">事件参数4。</param>
        /// <param name="arg5">事件参数5。</param>
        /// <param name="arg6">事件参数6。</param>
        /// <typeparam name="TArg1">事件参数1类型。</typeparam>
        /// <typeparam name="TArg2">事件参数2类型。</typeparam>
        /// <typeparam name="TArg3">事件参数3类型。</typeparam>
        /// <typeparam name="TArg4">事件参数4类型。</typeparam>
        /// <typeparam name="TArg5">事件参数5类型。</typeparam>
        /// <typeparam name="TArg6">事件参数6类型。</typeparam>
        public void Send<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(object eventType, TArg1 arg1, TArg2 arg2, TArg3 arg3,
            TArg4 arg4, TArg5 arg5, TArg6 arg6)
        {
            if (_eventTable.TryGetValue(eventType, out var d))
            {
                if (d.GetOwner() != null)
                {
                    d.Callback(d.GetOwner(), arg1, arg2, arg3, arg4, arg5, arg6);
                }
                else
                {
                    d.Callback(arg1, arg2, arg3, arg4, arg5, arg6);
                }
            }
        }

        #endregion
    }
}
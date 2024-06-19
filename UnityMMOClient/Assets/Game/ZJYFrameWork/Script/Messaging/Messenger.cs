/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using FrostEngine;
using ZJYFrameWork.Spring.Core;

#if NETFX_CORE
using System.Reflection;
#endif

namespace ZJYFrameWork.Messaging
{
    /// <summary>
    /// Messenger是一个允许对象交换消息的类。
    /// </summary>
    [Bean]
    public class Messenger : IMessenger
    {
        public static readonly Messenger Default = new Messenger();

        private readonly Dictionary<Type, SubjectBase> notifiers = new Dictionary<Type, SubjectBase>();

        private readonly Dictionary<string, Dictionary<Type, SubjectBase>> channelNotifiers =
            new Dictionary<string, Dictionary<Type, SubjectBase>>();

        /// <summary>
        /// 订阅消息。
        /// </summary>
        /// <param name="type">收件人订阅的消息类型.</param>
        /// <param name="action">当发送类型为T的消息时将执行的操作.</param>
        /// <returns>
        ///一次性对象，可用于从信使取消订阅消息。
        ///如果处置对象被处置，消息将自动取消订阅。
        /// </returns>
        public virtual IDisposable Subscribe(Type type, Action<object> action)
        {
            SubjectBase notifier;
            lock (notifiers)
            {
                if (!notifiers.TryGetValue(type, out notifier))
                {
                    notifier = new Subject<object>();
                    notifiers.Add(type, notifier);
                }
            }

            return (notifier as Subject<object>)?.Subscribe(action);
        }

        /// <summary>
        /// 订阅消息。
        /// </summary>
        /// <typeparam name="T">收件人订阅的消息类型.</typeparam>
        /// <param name="action">当发送类型为T的消息时将执行的操作.</param>
        /// <returns>
        ///一次性对象，可用于从信使取消订阅消息。
        ///如果处置对象被处置，消息将自动取消订阅。
        /// </returns>
        public virtual IDisposable Subscribe<T>(Action<T> action)
        {
            SubjectBase notifier;
            lock (notifiers)
            {
                if (notifiers.TryGetValue(typeof(T), out notifier))
                {
                    return (notifier as Subject<T>)?.Subscribe(action);
                }

                notifier = new Subject<T>();
                notifiers.Add(typeof(T), notifier);
            }

            return ((Subject<T>)notifier).Subscribe(action);
        }

        /// <summary>
        /// 订阅消息。
        /// </summary>
        /// <param name="channel">
        ///消息通道的名称。如果收件人订阅
        ///使用一个通道，并且发送方使用相同的通道发送消息，则this
        ///消息将被传递给接收者。其他没有这样做的接受者
        ///订阅时使用频道(或使用不同频道的用户)不会使用频道
        ///获取消息。
        /// </param>
        /// <param name="type">
        /// 收件人订阅的消息类型。
        /// </param>
        /// <param name="action">
        ///发送类型为T的消息时执行的动作。
        /// </param>
        /// <returns>
        ///一次性对象，可用于从信使取消订阅消息。
        ///如果处置对象被处置，消息将自动取消订阅。
        /// </returns>
        public virtual IDisposable Subscribe(string channel, Type type, Action<object> action)
        {
            Dictionary<Type, SubjectBase> dict = null;
            SubjectBase notifier = null;
            lock (channelNotifiers)
            {
                if (!channelNotifiers.TryGetValue(channel, out dict))
                {
                    dict = new Dictionary<Type, SubjectBase>();
                    channelNotifiers.Add(channel, dict);
                }

                if (dict.TryGetValue(type, out notifier)) return (notifier as Subject<object>)?.Subscribe(action);
                notifier = new Subject<object>();
                dict.Add(type, notifier);
            }

            return ((Subject<object>)notifier)?.Subscribe(action);
        }

        /// <summary>
        /// 订阅消息。
        /// </summary>
        /// <typeparam name="T">
        /// 接收方订阅的消息类型。
        /// </typeparam>
        /// <param name="channel">
        /// 消息通道的名称。如果收件人订阅
        /// 使用一个通道，并且发送方使用相同的通道发送消息，则this
        /// 消息将被传递给接收者。其他没有这样做的接受者
        /// 订阅时使用频道(或使用不同频道的用户)不会使用频道
        /// 获取消息。
        /// </param>
        /// <param name="action">
        /// 发送类型为T的消息时执行的动作。
        /// </param>
        /// <returns>
        /// 一次性对象，可用于从信使取消订阅消息。
        /// 如果处置对象被处置，消息将自动取消订阅。
        /// </returns>
        public virtual IDisposable Subscribe<T>(string channel, Action<T> action)
        {
            SubjectBase notifier = null;
            lock (channelNotifiers)
            {
                if (!channelNotifiers.TryGetValue(channel, out var dict))
                {
                    dict = new Dictionary<Type, SubjectBase>();
                    channelNotifiers.Add(channel, dict);
                }

                if (!dict.TryGetValue(typeof(T), out notifier))
                {
                    notifier = new Subject<T>();
                    dict.Add(typeof(T), notifier);
                }
            }

            return (notifier as Subject<T>)?.Subscribe(action);
        }

        /// <summary>
        /// 向已订阅的收件人发布消息。
        /// </summary>
        /// <param name="message"></param>
        public virtual void Publish(object message)
        {
            this.Publish<object>(message);
        }

        /// <summary>
        /// 向已订阅的收件人发布消息。
        /// </summary>
        /// <typeparam name="T">将要发送的消息类型.</typeparam>
        /// <param name="message">要发送给已订阅收件人的消息.</param>
        public virtual void Publish<T>(T message)
        {
            if (message == null)
                return;

            var messageType = message.GetType();

            List<KeyValuePair<Type, SubjectBase>> list;

            lock (notifiers)
            {
                if (notifiers.Count <= 0)
                    return;

                list = new List<KeyValuePair<Type, SubjectBase>>(this.notifiers);
            }

            foreach (var kv in list)
            {
                try
                {
                    if (kv.Key.IsAssignableFrom(messageType))
                        kv.Value.Publish(message);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        /// <summary>
        /// 向已订阅的收件人发布消息。
        /// </summary>
        /// <param name="channel">
        /// 消息通道的名称。如果收件人订阅
        /// 使用一个通道，并且发送方使用相同的通道发送消息，则this
        /// 消息将被传递给接收者。其他没有这样做的接受者
        /// 订阅时使用频道(或使用不同频道的用户)不会使用频道
        /// 获取消息。
        /// </param>
        /// <param name="message">要发送给已订阅收件人的消息.</param>
        public virtual void Publish(string channel, object message)
        {
            this.Publish<object>(channel, message);
        }

        /// <summary>
        /// 向已订阅的收件人发布消息。
        /// </summary>
        /// <typeparam name="T">
        /// 将要发送的消息类型。
        /// </typeparam>
        /// <param name="channel">
        /// 消息通道的名称。如果收件人订阅
        /// 使用一个通道，并且发送方使用相同的通道发送消息，则this
        /// 消息将被传递给接收者。其他没有这样做的接受者
        /// 订阅时使用频道(或使用不同频道的用户)不会使用频道
        /// 获取消息。
        /// </param>
        /// <param name="message">
        /// 要发送给已订阅收件人的消息。
        /// </param>
        public virtual void Publish<T>(string channel, T message)
        {
            if (string.IsNullOrEmpty(channel) || message == null)
                return;

            Type messageType = message.GetType();
            List<KeyValuePair<Type, SubjectBase>> list = null;

            lock (this.channelNotifiers)
            {
                if (!channelNotifiers.TryGetValue(channel, out var dict) || dict.Count <= 0)
                {
                    return;
                }

                list = new List<KeyValuePair<Type, SubjectBase>>(dict);
            }

            foreach (var kv in list)
            {
                try
                {
                    if (kv.Key.IsAssignableFrom(messageType))
                        kv.Value.Publish(message);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
    }
}
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZJYFrameWork.Execution
{
    /// <summary>
    /// Interceptable enumerator
    /// </summary>
    public class InterceptableEnumerator : IEnumerator
    {
        private object current;
        private Stack<IEnumerator> stack = new Stack<IEnumerator>();

        private Action<Exception> onException;
        private Action onFinally;
        private Func<bool> hasNext;

        public InterceptableEnumerator(IEnumerator routine)
        {
            this.stack.Push(routine);
        }

        public object Current
        {
            get { return this.current; }
        }

        public bool MoveNext()
        {
            try
            {
                if (!this.HasNext())
                {
                    this.OnFinally();
                    return false;
                }

                if (stack.Count <= 0)
                {
                    this.OnFinally();
                    return false;
                }

                IEnumerator ie = stack.Peek();
                bool hasNext = ie.MoveNext();
                if (!hasNext)
                {
                    this.stack.Pop();
                    return MoveNext();
                }

                this.current = ie.Current;
                if (this.current is IEnumerator enumerator)
                {
                    stack.Push(enumerator);
                    return MoveNext();
                }

                if (this.current is Coroutine)
                    ZJYFrameWork.Debug.Log(
                        "枚举器的结果包含'UnityEngine。协程的类型，如果发生异常，它不能被捕获。建议使用“yield return例程”，而不是“yield return StartCoroutine(例程)”。.");

                return true;
            }
            catch (Exception e)
            {
                this.OnException(e);
                this.OnFinally();
                return false;
            }
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        private void OnException(Exception e)
        {
            try
            {
                if (this.onException == null)
                    return;

                foreach (Action<Exception> action in this.onException.GetInvocationList())
                {
                    try
                    {
                        action(e);
                    }
                    catch (Exception ex)
                    {
                        ZJYFrameWork.Debug.Log("{}", ex);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void OnFinally()
        {
            try
            {
                if (this.onFinally == null)
                    return;

                foreach (var @delegate in this.onFinally.GetInvocationList())
                {
                    var action = (Action)@delegate;
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        ZJYFrameWork.Debug.LogError("{}", ex);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private bool HasNext()
        {
            return hasNext == null || hasNext();
        }

        /// <summary>
        /// Register a condition code block.
        /// </summary>
        /// <param name="hasNext"></param>
        public virtual void RegisterConditionBlock(Func<bool> hasNext)
        {
            this.hasNext = hasNext;
        }

        /// <summary>
        /// Register a code block, when an exception occurs it will be executed.
        /// </summary>
        /// <param name="onException"></param>
        public virtual void RegisterCatchBlock(Action<Exception> onException)
        {
            this.onException += onException;
        }

        /// <summary>
        /// Register a code block, when the end of the operation is executed.
        /// </summary>
        /// <param name="onFinally"></param>
        public virtual void RegisterFinallyBlock(Action onFinally)
        {
            this.onFinally += onFinally;
        }
    }
}
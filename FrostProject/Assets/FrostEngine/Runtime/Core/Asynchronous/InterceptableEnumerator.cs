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


// ReSharper disable once CheckNamespace
namespace FrostEngine
{
    /// <summary>
    /// Interceptable enumerator
    /// </summary>
    public class InterceptableEnumerator : IEnumerator
    {
        private readonly Stack<IEnumerator> _stack = new Stack<IEnumerator>();
        private object _current;
        private Func<bool> _hasNext;

        private Action<Exception> _onException;
        private Action _onFinally;

        public InterceptableEnumerator(IEnumerator routine)
        {
            this._stack.Push(routine);
        }

        public object Current => this._current;

        public bool MoveNext()
        {
            try
            {
                if (!this.HasNext())
                {
                    this.OnFinally();
                    return false;
                }

                if (_stack.Count <= 0)
                {
                    this.OnFinally();
                    return false;
                }

                IEnumerator ie = _stack.Peek();
                bool hasNext = ie.MoveNext();
                if (!hasNext)
                {
                    this._stack.Pop();
                    return MoveNext();
                }

                this._current = ie.Current;
                switch (this._current)
                {
                    case IEnumerator enumerator:
                        _stack.Push(enumerator);
                        return MoveNext();
                    case Coroutine _:
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        Debug.Log(
                            "枚举器的结果包含'UnityEngine。协程的类型，如果发生异常，它不能被捕获。建议使用“yield return例程”，而不是“yield return StartCoroutine(例程)”。.");
#endif
                        break;
                }

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
                if (this._onException == null)
                    return;

                foreach (var @delegate in this._onException.GetInvocationList())
                {
                    var action = (Action<Exception>)@delegate;
                    try
                    {
                        action(e);
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        // ReSharper disable once StringLiteralTypo
                        Debug.Log($"[InterceptableEnumerator] [msg:{ex}]");
#endif
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void OnFinally()
        {
            try
            {
                if (this._onFinally == null)
                    return;

                foreach (var @delegate in this._onFinally.GetInvocationList())
                {
                    var action = (Action)@delegate;
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                        // ReSharper disable once StringLiteralTypo
                        Debug.Log($"[InterceptableEnumerator] [msg:{ex}]");
#endif
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
            return _hasNext == null || _hasNext();
        }

        /// <summary>
        /// Register a condition code block.
        /// </summary>
        /// <param name="haseHasNext"></param>
        public virtual void RegisterConditionBlock(Func<bool> haseHasNext)
        {
            this._hasNext = haseHasNext;
        }

        /// <summary>
        /// Register a code block, when an exception occurs it will be executed.
        /// </summary>
        /// <param name="OnException"></param>
        // ReSharper disable once InconsistentNaming
        public virtual void RegisterCatchBlock(Action<Exception> OnException)
        {
            this._onException += OnException;
        }

        /// <summary>
        /// Register a code block, when the end of the operation is executed.
        /// </summary>
        /// <param name="onFinally"></param>
        public virtual void RegisterFinallyBlock(Action onFinally)
        {
            this._onFinally += onFinally;
        }
    }
}
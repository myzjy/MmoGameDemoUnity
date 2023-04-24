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
 * FITNESS FOR A PARTICULAR PURPOSE AND NON INFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;
using System.Collections;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.Utilities
{
    public class FilterEnumerator : IEnumerator
    {
        private readonly IEnumerator _enumerator;
        private readonly Predicate<object> _match;
        public FilterEnumerator(IEnumerator enumerator, Predicate<object> match)
        {
            this._enumerator = enumerator;
            this._match = match;
        }

        public object Current { get; private set; }

        public bool MoveNext()
        {
            while (this._enumerator.MoveNext())
            {
                var current = this._enumerator.Current;
                if (!_match(current))
                    continue;

                this.Current = current;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            this._enumerator.Reset();
        }
    }

    public class FilterEnumerator<T> : IEnumerator<T>
    {
        private IEnumerator<T> _enumerator;
        private Predicate<T> _match;
        public FilterEnumerator(IEnumerator<T> enumerator, Predicate<T> match)
        {
            this.Current = default(T);
            this._enumerator = enumerator;
            this._match = match;
        }

        public T Current { get; private set; }

        object IEnumerator.Current => this.Current;

        public bool MoveNext()
        {
            while (this._enumerator.MoveNext())
            {
                var current = this._enumerator.Current;
                if (!_match(current))
                    continue;

                this.Current = current;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            this._enumerator.Reset();
        }

        #region IDisposable Support
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                this.Reset();
                this._enumerator = null;
                this._match = null;
                this._disposedValue = true;
            }
        }

        ~FilterEnumerator()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

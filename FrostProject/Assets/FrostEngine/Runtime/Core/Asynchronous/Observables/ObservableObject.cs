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
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;
using FrostEngine;
using Debug = FrostEngine.Debug;


// ReSharper disable once CheckNamespace
namespace FrostEngine
{
    [Serializable]
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        private readonly object _lock = new object();
        private PropertyChangedEventHandler _propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (_lock)
                {
                    _propertyChanged += value;
                }
            }
            remove
            {
                lock (_lock)
                {
                    _propertyChanged -= value;
                }
            }
        }

        [Conditional("DEBUG")]
        protected void VerifyPropertyName(string propertyName)
        {
            var type = GetType();
            if (!string.IsNullOrEmpty(propertyName) && type.GetProperty(propertyName) == null)
                throw new ArgumentException("Property not found", propertyName);
        }

        /// <summary>
        /// Raises the PropertyChanging event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected virtual void RaisePropertyChanged(string propertyName = null)
        {
            RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the PropertyChanging event.
        /// </summary>
        /// <param name="eventArgs">Property changed event.</param>
        protected virtual void RaisePropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            try
            {
                VerifyPropertyName(eventArgs.PropertyName);

                if (_propertyChanged != null)
                    _propertyChanged(this, eventArgs);
            }
            catch (Exception e)
            {
                Debug.Log("设置属性“{}”，引发PropertyChanged失败。例外:{}", eventArgs.PropertyName, e);
            }
        }

        /// <summary>
        /// Raises the PropertyChanging event.
        /// </summary>
        /// <param name="eventArgs"></param>
        protected virtual void RaisePropertyChanged(params PropertyChangedEventArgs[] eventArgs)
        {
            foreach (var args in eventArgs)
            {
                try
                {
                    VerifyPropertyName(args.PropertyName);

                    if (_propertyChanged != null)
                        _propertyChanged(this, args);
                }
                catch (Exception e)
                {
                    Debug.Log("设置属性“{}”，引发PropertyChanged失败。例外:{}", args.PropertyName, e);
                }
            }
        }

        protected virtual string ParserPropertyName(LambdaExpression propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            if (!(propertyExpression.Body is MemberExpression body))
                throw new ArgumentException("Invalid argument", nameof(propertyExpression));

            var property = body.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("Argument is not a property", nameof(propertyExpression));

            return property.Name;
        }

        /// <summary>
        /// 设置指定的propertyExpression, field和newValue。
        /// </summary>
        /// <param name="propertyExpression">Property expression.</param>
        /// <param name="field">Field.</param>
        /// <param name="newValue">New value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected bool Set<T>(ref T field, T newValue, Expression<Func<T>> propertyExpression)
        {
            if (Equals(field, newValue))
                return false;

            field = newValue;
            var propertyName = ParserPropertyName(propertyExpression);
            RaisePropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        ///  设置指定的propertyName, field, newValue。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T field, T newValue, string propertyName)
        {
            if (Equals(field, newValue))
                return false;

            field = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        ///  设置指定的propertyName, field, newValue。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="newValue"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T field, T newValue, PropertyChangedEventArgs eventArgs)
        {
            if (Equals(field, newValue))
                return false;

            field = newValue;
            RaisePropertyChanged(eventArgs);
            return true;
        }
    }
}
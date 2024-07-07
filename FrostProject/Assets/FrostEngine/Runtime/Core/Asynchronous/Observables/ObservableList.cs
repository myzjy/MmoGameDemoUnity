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
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if NETFX_CORE
using System.Reflection;
#endif

using NotifyCollectionChangedEventHandler = System.Collections.Specialized.NotifyCollectionChangedEventHandler;
using INotifyCollectionChanged = System.Collections.Specialized.INotifyCollectionChanged;
using NotifyCollectionChangedAction = System.Collections.Specialized.NotifyCollectionChangedAction;
using NotifyCollectionChangedEventArgs = System.Collections.Specialized.NotifyCollectionChangedEventArgs;
using INotifyPropertyChanged = System.ComponentModel.INotifyPropertyChanged;
using PropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;
using PropertyChangedEventHandler = System.ComponentModel.PropertyChangedEventHandler;

namespace FrostEngine
{
    [Serializable]
    public class ObservableList<T> : IList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly PropertyChangedEventArgs CountEventArgs = new PropertyChangedEventArgs("Count");

        // ReSharper disable once StaticMemberInGenericType
        private static readonly PropertyChangedEventArgs IndexerEventArgs = new PropertyChangedEventArgs("Item[]");

        // ReSharper disable once InconsistentNaming
        private readonly object propertyChangedLock = new object();

        // ReSharper disable once InconsistentNaming
        private readonly object collectionChangedLock = new object();
        private PropertyChangedEventHandler _propertyChanged;
        private NotifyCollectionChangedEventHandler _collectionChanged;

        private SimpleMonitor _monitor = new SimpleMonitor();
        private IList<T> _items;

        [NonSerialized] private object _syncRoot;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (propertyChangedLock)
                {
                    _propertyChanged += value;
                }
            }
            remove
            {
                lock (propertyChangedLock)
                {
                    _propertyChanged -= value;
                }
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                lock (collectionChangedLock)
                {
                    _collectionChanged += value;
                }
            }
            remove
            {
                lock (collectionChangedLock)
                {
                    _collectionChanged -= value;
                }
            }
        }

        public ObservableList()
        {
            _items = new List<T>();
        }

        public ObservableList(List<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            _items = new List<T>();
            using IEnumerator<T> enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                _items.Add(enumerator.Current);
            }
        }

        public int Count => _items.Count;

        protected IList<T> Items => _items;

        public T this[int index]
        {
            get => _items[index];
            set
            {
                if (_items.IsReadOnly)
                    throw new NotSupportedException("ReadOnlyCollection");

                if (index < 0 || index >= _items.Count)
                    throw new ArgumentOutOfRangeException(StringUtils.Format("ArgumentOutOfRangeException:[{}]", index));

                SetItem(index, value);
            }
        }

        public void Add(T item)
        {
            if (_items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            int index = _items.Count;
            InsertItem(index, item);
        }

        public void Clear()
        {
            if (_items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            ClearItems();
        }

        public void CopyTo(T[] array, int index)
        {
            _items.CopyTo(array, index);
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (_items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            if (index < 0 || index > _items.Count)
                throw new ArgumentOutOfRangeException(string.Format("ArgumentOutOfRangeException:{0}", index));

            InsertItem(index, item);
        }

        public bool Remove(T item)
        {
            if (_items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            int index = _items.IndexOf(item);
            if (index < 0)
                return false;
            RemoveItem(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            if (_items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            if (index < 0 || index >= _items.Count)
                throw new ArgumentOutOfRangeException(string.Format("ArgumentOutOfRangeException:{0}", index));

            RemoveItem(index);
        }

        public void Move(int oldIndex, int newIndex)
        {
            MoveItem(oldIndex, newIndex);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            if (_items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            int index = _items.Count;
            InsertItem(index, collection);
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (_items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            if (index < 0 || index > _items.Count)
                throw new ArgumentOutOfRangeException(string.Format("ArgumentOutOfRangeException:{0}", index));

            InsertItem(index, collection);
        }

        public void RemoveRange(int index, int count)
        {
            if (_items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            if (index < 0 || index >= _items.Count)
                throw new ArgumentOutOfRangeException($"ArgumentOutOfRangeException:{index}");

            RemoveItem(index, count);
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return _items.IsReadOnly; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot != null) return _syncRoot;
                if (_items is ICollection c)
                {
                    _syncRoot = c.SyncRoot;
                }
                else
                {
                    Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
                }

                return _syncRoot;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (array.Rank != 1)
                throw new ArgumentException("RankMultiDimNotSupported");

            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException("NonZeroLowerBound");

            if (index < 0)
                throw new ArgumentOutOfRangeException($"ArgumentOutOfRangeException:{index}");

            if (array.Length - index < Count)
                throw new ArgumentException("ArrayPlusOffTooSmall");

            if (array is T[] tArray)
            {
                _items.CopyTo(tArray, index);
            }
            else
            {
                Type targetType = array.GetType().GetElementType();
                Type sourceType = typeof(T);
                if (targetType != null && !(targetType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(targetType)))
                    throw new ArgumentException("InvalidArrayType");

                object[] objects = array as object[];
                if (objects == null)
                    throw new ArgumentException("InvalidArrayType");

                int count = _items.Count;
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        objects[index++] = _items[i];
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException("InvalidArrayType");
                }
            }
        }

        object IList.this[int index]
        {
            get { return _items[index]; }
            set
            {
                if (value == null && !(default(T) == null))
                    throw new ArgumentNullException("value");

                try
                {
                    this[index] = (T)value;
                }
                catch (InvalidCastException e)
                {
                    throw new ArgumentException("", e);
                }
            }
        }

        bool IList.IsReadOnly
        {
            get { return _items.IsReadOnly; }
        }

        bool IList.IsFixedSize
        {
            get
            {
                if (_items is IList list)
                {
                    return list.IsFixedSize;
                }

                return _items.IsReadOnly;
            }
        }

        int IList.Add(object value)
        {
            if (_items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            if (value == null && default(T) != null)
                throw new ArgumentNullException(nameof(value));

            try
            {
                Add((T)value);
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException("", e);
            }

            return Count - 1;
        }

        bool IList.Contains(object value)
        {
            if (IsCompatibleObject(value))
            {
                return Contains((T)value);
            }

            return false;
        }

        int IList.IndexOf(object value)
        {
            if (IsCompatibleObject(value))
            {
                return IndexOf((T)value);
            }

            return -1;
        }

        void IList.Insert(int index, object value)
        {
            if (_items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            if (value == null && default(T) != null)
                throw new ArgumentNullException(nameof(value));

            try
            {
                Insert(index, (T)value);
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException("", e);
            }
        }

        void IList.Remove(object value)
        {
            if (_items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            if (IsCompatibleObject(value))
            {
                Remove((T)value);
            }
        }

        protected virtual void ClearItems()
        {
            CheckReentrancy();
            _items.Clear();
            OnPropertyChanged(CountEventArgs);
            OnPropertyChanged(IndexerEventArgs);
            OnCollectionReset();
        }

        protected virtual void RemoveItem(int index)
        {
            CheckReentrancy();
            T removedItem = this[index];

            _items.RemoveAt(index);

            OnPropertyChanged(CountEventArgs);
            OnPropertyChanged(IndexerEventArgs);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem, index);
        }

        protected virtual void RemoveItem(int index, int count)
        {
            CheckReentrancy();

            if (_items is List<T> list)
            {
                List<T> changedItems = list.GetRange(index, count);
                list.RemoveRange(index, count);

                OnPropertyChanged(CountEventArgs);
                OnPropertyChanged(IndexerEventArgs);
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, changedItems, index);
            }
        }

        protected virtual void InsertItem(int index, T item)
        {
            CheckReentrancy();

            _items.Insert(index, item);

            OnPropertyChanged(CountEventArgs);
            OnPropertyChanged(IndexerEventArgs);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        protected virtual void InsertItem(int index, IEnumerable<T> collection)
        {
            CheckReentrancy();

            var enumerable = collection as T[] ?? collection.ToArray();
            (_items as List<T>)?.InsertRange(index, enumerable);

            OnPropertyChanged(CountEventArgs);
            OnPropertyChanged(IndexerEventArgs);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, ToList(enumerable), index);
        }

        protected virtual void SetItem(int index, T item)
        {
            CheckReentrancy();
            T originalItem = this[index];

            _items[index] = item;

            OnPropertyChanged(IndexerEventArgs);
            OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, item, index);
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            CheckReentrancy();

            T removedItem = this[oldIndex];

            _items.RemoveAt(oldIndex);
            _items.Insert(newIndex, removedItem);

            OnPropertyChanged(IndexerEventArgs);
            OnCollectionChanged(NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex);
        }


        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (_propertyChanged != null)
            {
                _propertyChanged(this, e);
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_collectionChanged != null)
            {
                using (BlockReentrancy())
                {
                    _collectionChanged(this, e);
                }
            }
        }

        protected IDisposable BlockReentrancy()
        {
            _monitor.Enter();
            return _monitor;
        }

        protected void CheckReentrancy()
        {
            if (_monitor.Busy)
            {
                if ((_collectionChanged != null) && (_collectionChanged.GetInvocationList().Length > 1))
                    throw new InvalidOperationException();
            }
        }

        private IList ToList(IEnumerable<T> collection)
        {
            if (collection is IList list1)
                return list1;

            List<T> list = new List<T>();
            list.AddRange(collection);
            return list;
        }

        private static bool IsCompatibleObject(object value)
        {
            return ((value is T) || (value == null && default(T) == null));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            if (_collectionChanged != null)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, IList changedItems, int index)
        {
            if (_collectionChanged != null)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItems, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            if (_collectionChanged != null)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem,
            int index)
        {
            if (_collectionChanged != null)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        private void OnCollectionReset()
        {
            if (_collectionChanged != null)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        [Serializable()]
        private class SimpleMonitor : IDisposable
        {
            private int _busyCount;

            public bool Busy => _busyCount > 0;

            public void Enter()
            {
                ++_busyCount;
            }

            public void Dispose()
            {
                --_busyCount;
            }
        }
    }
}
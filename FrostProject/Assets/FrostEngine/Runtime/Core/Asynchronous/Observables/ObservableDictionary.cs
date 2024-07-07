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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NotifyCollectionChangedEventHandler = System.Collections.Specialized.NotifyCollectionChangedEventHandler;
using INotifyCollectionChanged = System.Collections.Specialized.INotifyCollectionChanged;
using NotifyCollectionChangedAction = System.Collections.Specialized.NotifyCollectionChangedAction;
using NotifyCollectionChangedEventArgs = System.Collections.Specialized.NotifyCollectionChangedEventArgs;
using INotifyPropertyChanged = System.ComponentModel.INotifyPropertyChanged;
using PropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;
using PropertyChangedEventHandler = System.ComponentModel.PropertyChangedEventHandler;

// ReSharper disable once CheckNamespace
namespace FrostEngine
{
    [Serializable]
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly PropertyChangedEventArgs CountEventArgs = new PropertyChangedEventArgs("Count");

        // ReSharper disable once StaticMemberInGenericType
        private static readonly PropertyChangedEventArgs IndexerEventArgs = new PropertyChangedEventArgs("Item[]");

        // ReSharper disable once StaticMemberInGenericType
        private static readonly PropertyChangedEventArgs KeysEventArgs = new PropertyChangedEventArgs("Keys");

        // ReSharper disable once StaticMemberInGenericType
        private static readonly PropertyChangedEventArgs ValuesEventArgs = new PropertyChangedEventArgs("Values");

        private readonly object _propertyChangedLock = new object();
        private readonly object _collectionChangedLock = new object();
        private PropertyChangedEventHandler _propertyChanged;
        private NotifyCollectionChangedEventHandler _collectionChanged;

        protected Dictionary<TKey, TValue> Dictionary;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (_propertyChangedLock)
                {
                    _propertyChanged += value;
                }
            }
            remove
            {
                lock (_propertyChangedLock)
                {
                    _propertyChanged -= value;
                }
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                lock (_collectionChangedLock)
                {
                    _collectionChanged += value;
                }
            }
            remove
            {
                lock (_collectionChangedLock)
                {
                    _collectionChanged -= value;
                }
            }
        }

        public ObservableDictionary()
        {
            Dictionary = new Dictionary<TKey, TValue>();
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            Dictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            Dictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public ObservableDictionary(int capacity)
        {
            Dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            Dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            Dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        public TValue this[TKey key]
        {
            get => !Dictionary.ContainsKey(key) ? default(TValue) : Dictionary[key];
            set => Insert(key, value, false);
        }

        public ICollection<TKey> Keys => Dictionary.Keys;

        public ICollection<TValue> Values => Dictionary.Values;

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            Dictionary.TryGetValue(key, out var value);
            var removed = Dictionary.Remove(key);
            if (!removed) return false;
            OnPropertyChanged(NotifyCollectionChangedAction.Remove);
            if (_collectionChanged != null)
                OnCollectionChanged(NotifyCollectionChangedAction.Remove,
                    new KeyValuePair<TKey, TValue>(key, value));

            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Insert(item.Key, item.Value, true);
        }

        public void Clear()
        {
            if (Dictionary.Count > 0)
            {
                Dictionary.Clear();
                OnPropertyChanged(NotifyCollectionChangedAction.Reset);
                if (_collectionChanged != null)
                    OnCollectionChanged();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary)Dictionary).CopyTo(array, arrayIndex);
        }

        public int Count => Dictionary.Count;

        public bool IsReadOnly => ((IDictionary)Dictionary).IsReadOnly;

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // ReSharper disable once NotDisposedResourceIsReturned
            return ((IEnumerable)Dictionary).GetEnumerator();
        }

        public void AddRange(IDictionary<TKey, TValue> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (items.Count <= 0) return;
            if (Dictionary.Count > 0)
            {
                if (items.Keys.Any((k) => Dictionary.ContainsKey(k)))
                    throw new ArgumentException("An item with the same key has already been added.");
                else
                {
                    foreach (var item in items)
                        ((IDictionary<TKey, TValue>)Dictionary).Add(item);
                }
            }
            else
            {
                Dictionary = new Dictionary<TKey, TValue>(items);
            }

            OnPropertyChanged(NotifyCollectionChangedAction.Add);
            if (_collectionChanged != null)
                OnCollectionChanged(NotifyCollectionChangedAction.Add, items.ToArray());
        }

        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (Dictionary.TryGetValue(key, out var item))
            {
                if (add)
                    throw new ArgumentException("An item with the same key has already been added.");

                if (Equals(item, value))
                    return;

                Dictionary[key] = value;
                OnPropertyChanged(NotifyCollectionChangedAction.Replace);
                if (_collectionChanged != null)
                    OnCollectionChanged(NotifyCollectionChangedAction.Replace,
                        new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, item));
            }
            else
            {
                Dictionary[key] = value;
                OnPropertyChanged(NotifyCollectionChangedAction.Add);
                if (_collectionChanged != null)
                    OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
            }
        }

        private void OnPropertyChanged(NotifyCollectionChangedAction action)
        {
            switch (action)
            {
                case NotifyCollectionChangedAction.Reset:
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                {
                    OnPropertyChanged(CountEventArgs);
                    OnPropertyChanged(IndexerEventArgs);
                    OnPropertyChanged(KeysEventArgs);
                    OnPropertyChanged(ValuesEventArgs);
                    break;
                }
                case NotifyCollectionChangedAction.Replace:
                {
                    OnPropertyChanged(IndexerEventArgs);
                    OnPropertyChanged(ValuesEventArgs);
                    break;
                }
                case NotifyCollectionChangedAction.Move:
                default:
                {
                    OnPropertyChanged(CountEventArgs);
                    OnPropertyChanged(IndexerEventArgs);
                    OnPropertyChanged(KeysEventArgs);
                    OnPropertyChanged(ValuesEventArgs);
                    break;
                }
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, eventArgs);
        }

        private void OnCollectionChanged()
        {
            if (_collectionChanged != null)
                _collectionChanged(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
        {
            if (_collectionChanged != null)
                _collectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem,
            KeyValuePair<TKey, TValue> oldItem)
        {
            if (_collectionChanged != null)
                _collectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
        {
            if (_collectionChanged != null)
                _collectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItems));
        }

        object IDictionary.this[object key]
        {
            get => ((IDictionary)Dictionary)[key];
            set => Insert((TKey)key, (TValue)value, false);
        }

        ICollection IDictionary.Keys => ((IDictionary)Dictionary).Keys;

        ICollection IDictionary.Values => ((IDictionary)Dictionary).Values;

        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)Dictionary).Contains(key);
        }

        void IDictionary.Add(object key, object value)
        {
            Add((TKey)key, (TValue)value);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)Dictionary).GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            Remove((TKey)key);
        }

        bool IDictionary.IsFixedSize => ((IDictionary)Dictionary).IsFixedSize;

        void ICollection.CopyTo(Array array, int index)
        {
            ((IDictionary)Dictionary).CopyTo(array, index);
        }

        object ICollection.SyncRoot => ((IDictionary)Dictionary).SyncRoot;

        bool ICollection.IsSynchronized => ((IDictionary)Dictionary).IsSynchronized;
    }
}
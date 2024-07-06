using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FrostEngine
{
    public class GameFrameworkMultiDictionary<K, V> : IEnumerable<KeyValuePair<K, GameFrameworkLinkedListRange<V>>>, IEnumerable
    {
        private readonly GameFrameworkLinkedList<V> linkedList;
        private readonly Dictionary<K, GameFrameworkLinkedListRange<V>> dictionary;

        /// <summary>
        /// 初始化游戏框架多值字典类的新实例。
        /// </summary>
        public GameFrameworkMultiDictionary()
        {
            linkedList = new GameFrameworkLinkedList<V>();
            dictionary = new Dictionary<K, GameFrameworkLinkedListRange<V>>();
        }

        /// <summary>
        /// 获取多值字典中实际包含的主键数量。
        /// </summary>
        public int Count
        {
            get { return dictionary.Count; }
        }

        /// <summary>
        /// 获取多值字典中指定主键的范围。
        /// </summary>
        /// <param name="key">指定的主键。</param>
        /// <returns>指定主键的范围。</returns>
        public GameFrameworkLinkedListRange<V> this[K key]
        {
            get
            {
                GameFrameworkLinkedListRange<V> range = default(GameFrameworkLinkedListRange<V>);
                dictionary.TryGetValue(key, out range);
                return range;
            }
        }

        /// <summary>
        /// 清理多值字典。
        /// </summary>
        public void Clear()
        {
            dictionary.Clear();
            linkedList.Clear();
        }

        /// <summary>
        /// 检查多值字典中是否包含指定主键。
        /// </summary>
        /// <param name="key">要检查的主键。</param>
        /// <returns>多值字典中是否包含指定主键。</returns>
        public bool Contains(K key)
        {
            return dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 检查多值字典中是否包含指定值。
        /// </summary>
        /// <param name="key">要检查的主键。</param>
        /// <param name="value">要检查的值。</param>
        /// <returns>多值字典中是否包含指定值。</returns>
        public bool Contains(K key, V value)
        {
            GameFrameworkLinkedListRange<V> range = default(GameFrameworkLinkedListRange<V>);
            if (dictionary.TryGetValue(key, out range))
            {
                return range.Contains(value);
            }

            return false;
        }

        /// <summary>
        /// 尝试获取多值字典中指定主键的范围。
        /// </summary>
        /// <param name="key">指定的主键。</param>
        /// <param name="range">指定主键的范围。</param>
        /// <returns>是否获取成功。</returns>
        public bool TryGetValue(K key, out GameFrameworkLinkedListRange<V> range)
        {
            return dictionary.TryGetValue(key, out range);
        }

        /// <summary>
        /// 向指定的主键增加指定的值。
        /// </summary>
        /// <param name="key">指定的主键。</param>
        /// <param name="value">指定的值。</param>
        public void Add(K key, V value)
        {
            GameFrameworkLinkedListRange<V> range;
            if (dictionary.TryGetValue(key, out range))
            {
                linkedList.AddBefore(range.Terminal, value);
            }
            else
            {
                LinkedListNode<V> first = linkedList.AddLast(value);
                LinkedListNode<V> terminal = linkedList.AddLast(default(V));
                dictionary.Add(key, new GameFrameworkLinkedListRange<V>(first, terminal));
            }
        }

        /// <summary>
        /// 从指定的主键中移除指定的值。
        /// </summary>
        /// <param name="key">指定的主键。</param>
        /// <param name="value">指定的值。</param>
        /// <returns>是否移除成功。</returns>
        public bool Remove(K key, V value)
        {
            GameFrameworkLinkedListRange<V> range = default(GameFrameworkLinkedListRange<V>);
            if (dictionary.TryGetValue(key, out range))
            {
                for (LinkedListNode<V> current = range.First;
                    current != null && current != range.Terminal;
                    current = current.Next)
                {
                    if (current.Value.Equals(value))
                    {
                        if (current == range.First)
                        {
                            LinkedListNode<V> next = current.Next;
                            if (next == range.Terminal)
                            {
                                linkedList.Remove(next);
                                dictionary.Remove(key);
                            }
                            else
                            {
                                dictionary[key] = new GameFrameworkLinkedListRange<V>(next, range.Terminal);
                            }
                        }

                        linkedList.Remove(current);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 从指定的主键中移除所有的值。
        /// </summary>
        /// <param name="key">指定的主键。</param>
        /// <returns>是否移除成功。</returns>
        public bool RemoveAll(K key)
        {
            GameFrameworkLinkedListRange<V> range = default(GameFrameworkLinkedListRange<V>);
            if (dictionary.TryGetValue(key, out range))
            {
                dictionary.Remove(key);

                LinkedListNode<V> current = range.First;
                while (current != null)
                {
                    LinkedListNode<V> next = current != range.Terminal ? current.Next : null;
                    linkedList.Remove(current);
                    current = next;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(dictionary);
        }

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        IEnumerator<KeyValuePair<K, GameFrameworkLinkedListRange<V>>> IEnumerable<KeyValuePair<K, GameFrameworkLinkedListRange<V>>>.
            GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>循环访问集合的枚举数。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 循环访问集合的枚举数。
        /// </summary>
        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<KeyValuePair<K, GameFrameworkLinkedListRange<V>>>, IEnumerator
        {
            private Dictionary<K, GameFrameworkLinkedListRange<V>>.Enumerator enumerator;

            public Enumerator(Dictionary<K, GameFrameworkLinkedListRange<V>> dictionary)
            {
                if (dictionary == null)
                {
                    throw new Exception("Dictionary is invalid.");
                }

                enumerator = dictionary.GetEnumerator();
            }

            /// <summary>
            /// 获取当前结点。
            /// </summary>
            public KeyValuePair<K, GameFrameworkLinkedListRange<V>> Current
            {
                get { return enumerator.Current; }
            }

            /// <summary>
            /// 获取当前的枚举数。
            /// </summary>
            object IEnumerator.Current
            {
                get { return enumerator.Current; }
            }

            /// <summary>
            /// 清理枚举数。
            /// </summary>
            public void Dispose()
            {
                enumerator.Dispose();
            }

            /// <summary>
            /// 获取下一个结点。
            /// </summary>
            /// <returns>返回下一个结点。</returns>
            public bool MoveNext()
            {
                return enumerator.MoveNext();
            }

            /// <summary>
            /// 重置枚举数。
            /// </summary>
            void IEnumerator.Reset()
            {
                ((IEnumerator<KeyValuePair<K, GameFrameworkLinkedListRange<V>>>) enumerator).Reset();
            }
        }
    }
}
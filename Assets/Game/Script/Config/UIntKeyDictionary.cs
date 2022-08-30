using UnityEngine;

namespace ZJYFrameWork
{
    public class UIntKeyDictionary<T> where T : class
    {
        private struct Entry
        {
            public int next;
            public uint key;
            public T value;
        }

        private int[] buckets; // 
        private Entry[] entries; // 
        private int freeList; // 
        private int count; // 
        private int freeCount; // 

        public UIntKeyDictionary(int capacity = 0)
        {
            Initalize(capacity);
        }

        // buckets和entries的capacity分法
        private void Initalize(int capacity)
        {
            if (capacity <= 0)
            {
                capacity = 1;
            }

            buckets = new int[capacity];
            for (int i = 0; i < capacity; ++i)
            {
                buckets[i] = -1;
            }

            entries = new Entry[capacity];
            freeList = -1;
        }

        // 返回当前目录中塞满的数字 
        public int Count
        {
            get { return count - freeCount; }
        }

        // 是否包含钥匙
        public bool ContainsKey(uint key)
        {
            return FindEntry(key) >= 0;
        }

        // 内容的删除
        public void Clear()
        {
            if (count <= 0) return;
            for (int i = 0; i < buckets.Length; ++i)
            {
                buckets[i] = -1;
            }

            System.Array.Clear(entries, 0, count);
            freeList = -1;
            count = 0;
            freeCount = 0;
        }

        // 删除
        public bool Remove(uint key)
        {
            uint bucket = (uint)(key % buckets.Length);
            int last = -1;
            for (int i = buckets[bucket]; i >= 0; last = i, i = entries[i].next)
            {
                if (entries[i].key == key)
                {
                    if (last < 0)
                    {
                        buckets[bucket] = entries[i].next;
                    }
                    else
                    {
                        entries[last].next = entries[i].next;
                    }

                    entries[i].next = freeList;
                    entries[i].key = 0;
                    entries[i].value = null;
                    freeList = i;
                    ++freeCount;
                    return true;
                }
            }

            return false;
        }

        public T this[uint key]
        {
            get
            {
                int i = FindEntry(key);
                if (i >= 0)
                {
                    return entries[i].value;
                }

                Debug.Assert(i >= 0, "Key is not included : " + key);
                return null;
            }
            set { Insert(key, value, false); }
        }

        public void Add(uint key, T value)
        {
            Insert(key, value, true);
        }

        public bool TryGetValue(uint key, out T value)
        {
            int i = FindEntry(key);
            if (i >= 0)
            {
                value = entries[i].value;
                return true;
            }

            value = null;
            return false;
        }

        // 返回与key对应的Entry的索引
        private int FindEntry(uint key)
        {
            if (buckets != null)
            {
                for (int i = buckets[key % buckets.Length]; i >= 0; i = entries[i].next)
                {
                    if (entries[i].key == key)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        // 在entries中插入key, value
        private void Insert(uint key, T value, bool add)
        {
            if (buckets == null)
            {
                Initalize(1);
            }

            uint targetBucket = (uint)(key % buckets.Length);

            // 已经拥有key时的处理
            for (int i = buckets[targetBucket]; i >= 0; i = entries[i].next)
            {
                if (entries[i].key == key)
                {
                    Debug.Assert(!add, "Already contains key : " + key);
                    entries[i].value = value;
                    return;
                }
            }

            // 没有key的情况下注册key并设置value
            int index;
            // 有空位的情况
            if (freeCount > 0)
            {
                index = freeList;
                freeList = entries[index].next;
                --freeCount;
            }
            else
            {
                // 没有空闲的情况下调整大小，将原来的大小作为索引
                if (count == entries.Length)
                {
                    Resize();
                    targetBucket = (uint)(key % buckets.Length);
                }

                index = count;
                ++count;
            }

            entries[index].key = key;
            entries[index].next = buckets[targetBucket];
            entries[index].value = value;
            buckets[targetBucket] = index;
        }

        /// <summary>
        /// 获得新的大小
        /// </summary>
        /// <param name="currentSize"></param>
        /// <returns></returns>
        int GetNewSize(int currentSize)
        {
            for (int i = currentSize * 2 + 1; i < System.Int32.MaxValue; i += 2)
            {
                if (IsPrime(i))
                {
                    return i;
                }
            }

            return System.Int32.MaxValue;
        }

        bool IsPrime(int candidate)
        {
            if ((candidate & 1) != 0)
            {
                int limit = (int)Mathf.Sqrt(candidate);
                for (int divisor = 3; divisor <= limit; divisor += 2)
                {
                    if ((candidate % divisor) == 0)
                        return false;
                }

                return true;
            }

            return (candidate == 2);
        }

        /// <summary>
        /// 调整大小
        /// </summary>
        private void Resize()
        {
            int newSize = GetNewSize(entries.Length);
            int[] newBuckets = new int[newSize];
            for (int i = 0; i < newBuckets.Length; ++i)
            {
                newBuckets[i] = -1;
            }

            Entry[] newEntries = new Entry[newSize];
            System.Array.Copy(entries, 0, newEntries, 0, count);

            for (int i = 0; i < count; ++i)
            {
                uint bucket = (uint)(newEntries[i].key % newSize);
                newEntries[i].next = newBuckets[bucket];
                newBuckets[bucket] = i;
            }

            buckets = newBuckets;
            entries = newEntries;
        }

        /// <summary>
        /// 内部すべての内容を貰う
        /// </summary>
        /// <returns></returns>
        public T[] GetAllElements()
        {
            System.Collections.Generic.List<T> tempList = new System.Collections.Generic.List<T>(Count);
            for (int i = 0; i < buckets.Length; i++)
            {
                for (int j = buckets[i]; j >= 0; j = entries[j].next)
                {
                    if (entries[j].value != null)
                    {
                        tempList.Add(entries[j].value);
                    }
                }
            }

            return tempList.ToArray();
        }
    }
}
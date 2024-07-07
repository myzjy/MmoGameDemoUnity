#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_ALTERNATE_SSL && !BESTHTTP_DISABLE_HTTP2

using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace BestHTTP.Connections.HTTP2
{
    sealed class HeaderTable
    {
        // https://http2.github.io/http2-spec/compression.html#static.table.definition
        // 有效索引从1开始，所以有一个空条目。
        private static readonly string[] StaticTableValues = new string[]
        {
            string.Empty,
            string.Empty,
            "GET",
            "POST", 
            "/",
            "/index.html", 
            "http",
            "https", 
            "200", 
            "204",
            "206",
            "304",
            "400", 
            "404",
            "500",
            string.Empty,
            "gzip, deflate"
        };

        // https://http2.github.io/http2-spec/compression.html#static.table.definition
        // 有效索引从1开始，所以有一个空条目。
        private static readonly string[] StaticTable = new string[]
        {
            string.Empty,
            ":authority",
            ":method", // GET
            ":method", // POST
            ":path", // /
            ":path", // index.html
            ":scheme", // http
            ":scheme", // https
            ":status", // 200
            ":status", // 204
            ":status", // 206
            ":status", // 304
            ":status", // 400
            ":status", // 404
            ":status", // 500
            "accept-charset",
            "accept-encoding", // gzip, deflate
            "accept-language",
            "accept-ranges",
            "accept",
            "access-control-allow-origin",
            "age",
            "allow",
            "authorization",
            "cache-control",
            "content-disposition",
            "content-encoding",
            "content-language",
            "content-length",
            "content-location",
            "content-range",
            "content-type",
            "cookie",
            "date",
            "etag",
            "expect",
            "expires",
            "from",
            "host",
            "if-match",
            "if-modified-since",
            "if-none-match",
            "if-range",
            "if-unmodified-since",
            "last-modified",
            "link",
            "location",
            "max-forwards",
            "proxy-authenticate",
            "proxy-authorization",
            "range",
            "referer",
            "refresh",
            "retry-after",
            "server",
            "set-cookie",
            "strict-transport-security",
            "transfer-encoding",
            "user-agent",
            "vary",
            "via",
            "www-authenticate",
        };

        private readonly List<KeyValuePair<string, string>> _dynamicTable = new List<KeyValuePair<string, string>>();

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Http2SettingsRegistry _settingsRegistry;

        private uint _maxDynamicTableSize;

        public HeaderTable(Http2SettingsRegistry registry)
        {
            this._settingsRegistry = registry;
            this.MaxDynamicTableSize = this._settingsRegistry[Http2Settings.HeaderTableSize];
        }

        private uint DynamicTableSize { get; set; }

        public uint MaxDynamicTableSize
        {
            get => this._maxDynamicTableSize;
            set
            {
                this._maxDynamicTableSize = value;
                EvictEntries(0);
            }
        }

        public KeyValuePair<uint, uint> GetIndex(string key, string value)
        {
            for (var i = 0; i < _dynamicTable.Count; ++i)
            {
                var kvp = _dynamicTable[i];

                // 键和值的精确匹配
                if (kvp.Key.Equals(key, StringComparison.OrdinalIgnoreCase) &&
                    kvp.Value.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return new KeyValuePair<uint, uint>((uint)(StaticTable.Length + i),
                        (uint)(StaticTable.Length + i));
                }
            }

            var bestMatch = new KeyValuePair<uint, uint>(0, 0);
            for (var i = 0; i < StaticTable.Length; ++i)
            {
                if (!StaticTable[i].Equals(key, StringComparison.OrdinalIgnoreCase)) continue;
                if (i < StaticTableValues.Length && !string.IsNullOrEmpty(StaticTableValues[i]) &&
                    StaticTableValues[i].Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return new KeyValuePair<uint, uint>((uint)i, (uint)i);
                }
                else
                {
                    bestMatch = new KeyValuePair<uint, uint>((uint)i, 0);
                }
            }

            return bestMatch;
        }

        public string GetKey(uint index)
        {
            return index < StaticTable.Length
                ? StaticTable[index]
                : this._dynamicTable[(int)(index - StaticTable.Length)].Key;
        }

        public KeyValuePair<string, string> GetHeader(UInt32 index)
        {
            if (index < StaticTable.Length)
            {
                return new KeyValuePair<string, string>(StaticTable[index],
                    index < StaticTableValues.Length ? StaticTableValues[index] : null);
            }

            return this._dynamicTable[(int)(index - StaticTable.Length)];
        }

        public void Add(KeyValuePair<string, string> header)
        {
            // https://http2.github.io/http2-spec/compression.html#calculating.table.size
            // 一个条目的大小是它的名称长度的和，以字节为单位(定义在Section5.2)。,
            // 它的值的长度(以八字节为单位)和32.
            var newHeaderSize = CalculateEntrySize(header);

            EvictEntries(newHeaderSize);

            // 如果新条目的大小小于或等于最大大小，则将该条目添加到表中。
            // 尝试添加大于最大大小的条目不会出错;
            // 尝试添加大于最大大小的条目会导致清空表中的所有现有条目并导致空表。
            if (this.DynamicTableSize + newHeaderSize > this.MaxDynamicTableSize) return;
            this._dynamicTable.Insert(0, header);
            this.DynamicTableSize += newHeaderSize;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private uint CalculateEntrySize(KeyValuePair<string, string> entry)
        {
            return 32 + (uint)System.Text.Encoding.UTF8.GetByteCount(entry.Key) +
                   (uint)System.Text.Encoding.UTF8.GetByteCount(entry.Value);
        }

        private void EvictEntries(uint newHeaderSize)
        {
            // https://http2.github.io/http2-spec/compression.html#entry.addition
            // 在向动态表添加新条目之前，将从动态表的末尾删除条目，直到动态表的大小小于或等于(最大大小-新条目大小)或直到表为空
            while (this.DynamicTableSize + newHeaderSize > this.MaxDynamicTableSize && this._dynamicTable.Count > 0)
            {
                var entry = this._dynamicTable[^1];
                this._dynamicTable.RemoveAt(this._dynamicTable.Count - 1);
                this.DynamicTableSize -= CalculateEntrySize(entry);
            }
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder("[HeaderTable ");
            sb.AppendFormat("DynamicTable count: {0}, DynamicTableSize: {1}, MaxDynamicTableSize: {2}, ",
                this._dynamicTable.Count, this.DynamicTableSize, this.MaxDynamicTableSize);

            foreach (var kvp in this._dynamicTable)
            {
                sb.AppendFormat("\"{0}\": \"{1}\", ", kvp.Key, kvp.Value);
            }

            sb.Append("]");
            return sb.ToString();
        }
    }
}

#endif
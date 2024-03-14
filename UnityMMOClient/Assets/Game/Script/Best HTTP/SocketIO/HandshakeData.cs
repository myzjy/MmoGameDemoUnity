#if !BESTHTTP_DISABLE_SOCKETIO

using System;
using System.Collections.Generic;

namespace BestHTTP.SocketIO
{
    using JSON;

    /// <summary>
    /// Helper类来解析和保存握手信息。
    /// </summary>
    public sealed class HandshakeData
    {
        /// <summary>
        /// 该连接的会话ID。
        /// </summary>
        public string Sid { get; private set; }

        /// <summary>
        /// 可能升级的列表。
        /// </summary>
        public List<string> Upgrades { get; private set; }

        /// <summary>
        /// 我们必须设置一个ping消息的时间间隔。
        /// </summary>
        public TimeSpan PingInterval { get; private set; }

        /// <summary>
        /// 当我们可以认为连接断开时，需要经过多长时间才能得到ping请求的应答。
        /// </summary>
        public TimeSpan PingTimeout { get; private set; }


        public bool Parse(string str)
        {
            bool success = false;
            Dictionary<string, object> dict = Json.Decode(str, ref success) as Dictionary<string, object>;
            if (!success)
                return false;

            try
            {
                this.Sid = GetString(dict, "sid");
                this.Upgrades = GetStringList(dict, "upgrades");
                this.PingInterval = TimeSpan.FromMilliseconds(GetInt(dict, "pingInterval"));
                this.PingTimeout = TimeSpan.FromMilliseconds(GetInt(dict, "pingTimeout"));
            }
            catch (Exception ex)
            {
                HttpManager.Logger.Exception("HandshakeData", "Parse", ex);
                return false;
            }

            return true;
        }

        private static object Get(Dictionary<string, object> from, string key)
        {
            if (!from.TryGetValue(key, out var value))
            {
                throw new Exception($"Can't get {key} from Handshake data!");
            }

            return value;
        }

        private static string GetString(Dictionary<string, object> from, string key)
        {
            return Get(from, key) as string;
        }

        private static List<string> GetStringList(Dictionary<string, object> from, string key)
        {
            if (Get(from, key) is List<object> value)
            {
                List<string> result = new List<string>(value.Count);
                for (int i = 0; i < value.Count; ++i)
                {
                    if (value[i] is string str)
                    {
                        result.Add(str);
                    }
                }

                return result;
            }

            return null;
        }

        private static int GetInt(Dictionary<string, object> from, string key)
        {
            return (int)(double)Get(from, key);
        }

    }
}

#endif
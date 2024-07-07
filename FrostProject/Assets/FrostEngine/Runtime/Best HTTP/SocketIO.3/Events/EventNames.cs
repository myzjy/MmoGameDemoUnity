#if !BESTHTTP_DISABLE_SOCKETIO

using System;
using System.Linq;

namespace BestHTTP.SocketIO3.Events
{
    /// <summary>
    /// Helper类，为传输和SocketIO事件类型的简单Enum->string会话提供函数。
    /// </summary>
    public static class EventNames
    {
        public const string Connect = "connect";
        public const string Disconnect = "disconnect";
        public const string Event = "event";
        public const string Ack = "ack";
        public const string Error = "error";
        public const string BinaryEvent = "binaryevent";
        public const string BinaryAck = "binaryack";

        private static readonly string[] SocketIONames = new string[]
        {
            "unknown",
            "connect",
            "disconnect",
            "event",
            "ack",
            "error",
            // ReSharper disable once StringLiteralTypo
            "binaryevent",
            // ReSharper disable once StringLiteralTypo
            "binaryack"
        };

        private static readonly string[] TransportNames = new string[]
        {
            "unknown",
            "open",
            "close",
            "ping",
            "pong",
            "message",
            "upgrade",
            "noop"
        };

        private static readonly string[] BlacklistedEvents = new string[]
        {
            "connect",
            "connect_error",
            "connect_timeout",
            "disconnect",
            "error",
            "reconnect",
            "reconnect_attempt",
            "reconnect_failed",
            "reconnect_error",
            "reconnecting"
        };

        public static string GetNameFor(SocketIOEventTypes type)
        {
            return SocketIONames[(int)type + 1];
        }

        public static string GetNameFor(TransportEventTypes transEvent)
        {
            return TransportNames[(int)transEvent + 1];
        }

        /// <summary>
        ///检查事件名称是否列入黑名单。
        /// </summary>
        public static bool IsBlacklisted(string eventName)
        {
            return BlacklistedEvents.Any(t => string.Compare(t, eventName, StringComparison.OrdinalIgnoreCase) == 0);
        }
    }
}

#endif
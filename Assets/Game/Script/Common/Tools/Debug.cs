using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Debugger.Widows.Model.VO;
using ZJYFrameWork.Log;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork
{
    public class Debug
    {
        private static ILogFactory log = new LogManager();
        public static readonly Queue<LogNode> logNodes = new Queue<LogNode>();
        private static int maxLine = 100;

        public static void Initialize()
        {
            Application.logMessageReceived += OnLogMessageReceived;
        }

        private static void OnLogMessageReceived(string logMessage, string stackTrace, LogType logType)
        {
            if (logType == LogType.Assert)
            {
                logType = LogType.Error;
            }

            logNodes.Enqueue(LogNode.Create(logType, logMessage, stackTrace));
            while (logNodes.Count > maxLine)
            {
                ReferenceCache.Release(logNodes.Dequeue());
            }
        }

        public static void Shutdown()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
            Clear();
        }

        private static void Clear()
        {
            logNodes.Clear();
        }

        /// <summary>
        /// 设置游戏框架日志辅助器。
        /// </summary>
        /// <param name="logHelper">要设置的游戏框架日志辅助器。</param>
        public static void SetLogHelper(ILogFactory logHelper)
        {
            Debug.log = logHelper;
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
        {
            UnityEngine.Debug.DrawLine(start, end, color, duration);
        }

        /// <summary>
        /// 打印信息级别日志，用于记录程序正常运行日志信息。
        /// </summary>
        /// <param name="format">日志格式。</param>
        /// <param name="arg0">日志参数 0。</param>
        /// <param name="arg1">日志参数 1。</param>
        /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG 或 ENABLE_INFO_AND_ABOVE_LOG 预编译选项时生效。</remarks>
        [Conditional("ENABLE_LOG")]
        [Conditional("ENABLE_INFO_LOG")]
        [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
        [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
        public static void Info(string format, object arg0, object arg1)
        {
            log.Log(Level.DEBUG, StringUtils.Format(format, arg0, arg1));
        }

        /// <summary>
        /// 打印调试级别日志，用于记录调试类日志信息。
        /// </summary>
        /// <param name="message">日志内容。</param>
        /// <remarks>仅在带有 DEBUG 预编译选项且带有 ENABLE_LOG、ENABLE_DEBUG_LOG 或 ENABLE_DEBUG_AND_ABOVE_LOG 预编译选项时生效。</remarks>
        // [Conditional("ENABLE_LOG")]
        // [Conditional("ENABLE_DEBUG_LOG")]
        // [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
        public static void Log(object message)
        {
            if (log == null)
            {
                return;
            }

            log.Log(Level.DEBUG, message);
        }

        public static void Log(string message, params object[] args)
        {
            if (log == null)
            {
                return;
            }

            log.Log(Level.INFO, StringUtils.Format(message, args));
        }

        /// <summary>
        ///   <para>Logs a message to the Unity Console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional("ENABLE_LOG")]
        [Conditional("ENABLE_DEBUG_LOG")]
        [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
        public static void Log(object message, Object context) =>
            UnityEngine.Debug.unityLogger.Log(LogType.Log, message, context);

        /// <summary>
        ///   <para>Logs a formatted message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="logType">Type of message e.g. warn or error etc.</param>
        /// <param name="logOptions">Option flags to treat the log message special.</param>
        /// <footer><a href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/30_search.html?q=Debug.LogFormat">`Debug.LogFormat` on docs.unity3d.com</a></footer>
        public static void LogFormat(string format, params object[] args) =>
            log.Log(Level.DEBUG, StringUtils.Format(format, args));

        /// <summary>
        ///   <para>Logs a formatted message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="logType">Type of message e.g. warn or error etc.</param>
        /// <param name="logOptions">Option flags to treat the log message special.</param>
        /// <footer><a href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/30_search.html?q=Debug.LogFormat">`Debug.LogFormat` on docs.unity3d.com</a></footer>
        [Conditional("ENABLE_LOG")]
        [Conditional("ENABLE_DEBUG_LOG")]
        [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
        public static void LogFormat(Object context, string format, params object[] args) =>
            UnityEngine.Debug.unityLogger.LogFormat(LogType.Log, context, format, args);

        /// <summary>
        ///   <para>Logs a formatted message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="logType">Type of message e.g. warn or error etc.</param>
        /// <param name="logOptions">Option flags to treat the log message special.</param>
        [Conditional("ENABLE_LOG")]
        [Conditional("ENABLE_DEBUG_LOG")]
        [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
        public static void LogFormat(
            LogType logType,
            LogOption logOptions,
            Object context,
            string format,
            params object[] args)
        {
            UnityEngine.Debug.LogFormat(logType, logOptions, context, format, args);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <footer><a href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/30_search.html?q=Debug.LogError">`Debug.LogError` on docs.unity3d.com</a></footer>
        public static void LogError(object message)
        {
            if (log == null)
            {
                return;
            }

            log.Log(Level.ERROR, message);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs an error message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <footer><a href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/30_search.html?q=Debug.LogError">`Debug.LogError` on docs.unity3d.com</a></footer>
        [Conditional("ENABLE_LOG")]
        [Conditional("ENABLE_DEBUG_LOG")]
        [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
        public static void LogError(object message, Object context)
        {
            if (log == null)
            {
                return;
            }

            log.Log(Level.ERROR, StringUtils.Format((string)message, context));
        }

        /// <summary>
        ///   <para>Logs a formatted error message to the Unity console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <footer><a href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/30_search.html?q=Debug.LogErrorFormat">`Debug.LogErrorFormat` on docs.unity3d.com</a></footer>
        [Conditional("ENABLE_LOG")]
        [Conditional("ENABLE_DEBUG_LOG")]
        [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
        public static void LogErrorFormat(string format, params object[] args)
        {
            if (log == null)
            {
                return;
            }

            log.Log(Level.ERROR, StringUtils.Format(format, args));
        }


        public static void ClearDeveloperConsole()
        {
            UnityEngine.Debug.ClearDeveloperConsole();
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <footer><a href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/30_search.html?q=Debug.LogWarning">`Debug.LogWarning` on docs.unity3d.com</a></footer>
        public static void LogWarning(object message)
        {
            if (log == null)
            {
                return;
            }

            log.Log(Level.WARN, message);
        }

        /// <summary>
        ///   <para>A variant of Debug.Log that logs a warning message to the console.</para>
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <footer><a href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/30_search.html?q=Debug.LogWarning">`Debug.LogWarning` on docs.unity3d.com</a></footer>
        public static void LogWarning(object message, params object[] context)
        {
            if (log == null)
            {
                return;
            }

            log.Log(Level.WARN, StringUtils.Format((string)message, context));
        }

        /// <summary>
        ///   <para>Logs a formatted warning message to the Unity Console.</para>
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <footer><a href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/30_search.html?q=Debug.LogWarningFormat">`Debug.LogWarningFormat` on docs.unity3d.com</a></footer>
        public static void LogWarningFormat(string format, params object[] args)
        {
            if (log == null)
            {
                return;
            }

            log.Log(Level.WARN, StringUtils.Format(format, args));
        }

        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <footer><a href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/30_search.html?q=Debug.Assert">`Debug.Assert` on docs.unity3d.com</a></footer>
        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition)
        {
            if (condition)
                return;
            UnityEngine.Debug.unityLogger.Log(LogType.Assert, (object)"Assertion failed");
        }

        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <footer><a href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/30_search.html?q=Debug.Assert">`Debug.Assert` on docs.unity3d.com</a></footer>
        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, Object context)
        {
            if (condition)
                return;
            UnityEngine.Debug.unityLogger.Log(LogType.Assert, (object)"Assertion failed", context);
        }

        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <footer><a href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/30_search.html?q=Debug.Assert">`Debug.Assert` on docs.unity3d.com</a></footer>
        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, object message)
        {
            if (condition)
                return;
            UnityEngine.Debug.unityLogger.Log(LogType.Assert, message);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, string message)
        {
            if (condition)
                return;
            UnityEngine.Debug.unityLogger.Log(LogType.Assert, (object)message);
        }

        /// <summary>
        ///   <para>Assert a condition and logs an error message to the Unity console on failure.</para>
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="context">Object to which the message applies.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <footer><a href="https://docs.unity3d.com/2019.4/Documentation/ScriptReference/30_search.html?q=Debug.Assert">`Debug.Assert` on docs.unity3d.com</a></footer>
        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, object message, Object context)
        {
            if (condition)
                return;
            UnityEngine.Debug.unityLogger.Log(LogType.Assert, message, context);
        }

        [Conditional("UNITY_ASSERTIONS")]
        public static void Assert(bool condition, string message, Object context)
        {
            if (condition)
                return;
            UnityEngine.Debug.unityLogger.Log(LogType.Assert, (object)message, context);
        }

        public static void LogError(string s, params object[] p)
        {
            if (log == null)
            {
                return;
            }

            log.Log(Level.ERROR, StringUtils.Format(s, p));
        }
    }
}
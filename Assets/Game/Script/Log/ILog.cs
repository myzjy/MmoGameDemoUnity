﻿using System;

namespace ZJYFrameWork.Log
{
    /// <summary>
    /// 应用程序使用ILog接口将消息记录到 ZJYFrameWork.Log框架中。
    /// </summary>
    /// <example>Simple example of logging messages
    ///  <code lang="C#">
    /// ILog log = LogManager.GetLogger("application-log");
    /// 
    /// log.Info("Application Start");
    /// log.Debug("This is a debug message");
    /// 
    /// if (log.IsDebugEnabled)
    /// {
    ///		log.Debug("This is another debug message");
    /// }
    /// </code>
    /// </example>
    /// <author>Clark</author>
    public interface ILog
    {
        void Debug(object message);

        void Debug(object message, Exception exception);

        void DebugFormat(string format, params object[] args);
        void Info(string format, object arg0, object arg1);
        void Info(object message);

        void Info(object message, Exception exception);

        void InfoFormat(string format, params object[] args);

        void Warn(object message);

        void Warn(object message, Exception exception);

        void WarnFormat(string format, params object[] args);

        void Error(object message);

        void Error(object message, Exception exception);

        void ErrorFormat(string format, params object[] args);

        void Fatal(object message);

        void Fatal(object message, Exception exception);

        void FatalFormat(string format, params object[] args);

        bool IsDebugEnabled { get; }

        bool IsInfoEnabled { get; }

        bool IsWarnEnabled { get; }

        bool IsErrorEnabled { get; }

        bool IsFatalEnabled { get; }
    }
}
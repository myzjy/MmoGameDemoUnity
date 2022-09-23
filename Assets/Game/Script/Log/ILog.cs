using System;

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

        void Info(string format,params object[] arg0);
    }
}
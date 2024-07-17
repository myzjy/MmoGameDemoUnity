using System;
using System.Diagnostics;
using System.Text;

namespace FrostEngine
{
    /// <summary>
    /// 默认游戏框架日志辅助。
    /// </summary>
    public class DefaultLogHelper : GameFrameworkLog.ILogHelper
    {
        private enum ELogLevel
        {
            Info,
            Debug,
            Assert,
            Warning,
            Error,
            Exception,
        }

        private const ELogLevel FilterLevel = ELogLevel.Info;
        private static readonly StringBuilder StringBuilder = new(1024);

        /// <summary>
        /// 打印游戏日志。
        /// </summary>
        /// <param name="level">游戏框架日志等级。</param>
        /// <param name="message">日志信息。</param>
        /// <exception>游戏框架异常类。
        ///     <cref>GameFrameworkException</cref>
        /// </exception>
        public void Log(GameFrameworkLogLevel level, object message)
        {
            switch (level)
            {
                case GameFrameworkLogLevel.Debug:
                    string str = StringUtils.Format("<color=#888888>{}</color>", message);
                    LogImp(ELogLevel.Debug,str);
                    break;

                case GameFrameworkLogLevel.Info:
                    LogImp(ELogLevel.Info, message.ToString());
                    break;

                case GameFrameworkLogLevel.Warning:
                    LogImp(ELogLevel.Warning, message.ToString());
                    break;

                case GameFrameworkLogLevel.Error:
                    LogImp(ELogLevel.Error, message.ToString());
                    break;

                case GameFrameworkLogLevel.Fatal:
                    LogImp(ELogLevel.Exception, message.ToString());
                    break;

                default:
                    throw new Exception(message.ToString());
            }
        }

        /// <summary>
        /// 获取日志格式。
        /// </summary>
        /// <param name="eLogLevel">日志级别。</param>
        /// <param name="logString">日志字符。</param>
        /// <param name="bColor">是否使用颜色。</param>
        /// <returns>StringBuilder。</returns>
        private static StringBuilder GetFormatString(ELogLevel eLogLevel, string logString, bool bColor)
        {
            StringBuilder.Clear();
            switch (eLogLevel)
            {
                case ELogLevel.Debug:
                    StringBuilder.AppendFormat(
                        bColor
                            ? "<color=gray><b>[Debug] ► </b></color> - <color=#00FF18>{0}</color>"
                            : "<color=#00FF18><b>[Debug] ► </b></color> - {0}",
                        logString);
                    break;
                case ELogLevel.Info:
                    StringBuilder.AppendFormat(
                        bColor
                            ? "<color=gray><b>[INFO] ► </b></color> - <color=gray>{0}</color>"
                            : "<color=gray><b>[INFO] ► </b></color> - {0}",
                        logString);
                    break;
                case ELogLevel.Assert:
                    StringBuilder.AppendFormat(
                        bColor
                            ? "<color=#FF00BD><b>[ASSERT] ► </b></color> - <color=green>{0}</color>"
                            : "<color=#FF00BD><b>[ASSERT] ► </b></color> - {0}",
                        logString);
                    break;
                case ELogLevel.Warning:
                    StringBuilder.AppendFormat(
                        bColor
                            ? "<color=#FF9400><b>[WARNING] ► </b></color> - <color=yellow>{0}</color>"
                            : "<color=#FF9400><b>[WARNING] ► </b></color> - {0}",
                        logString);
                    break;
                case ELogLevel.Error:
                    StringBuilder.AppendFormat(
                        bColor
                            ? "<color=red><b>[ERROR] ► </b></color> - <color=red>{0}</color>"
                            : "<color=red><b>[ERROR] ► </b></color>- {0}",
                        logString);
                    break;
                case ELogLevel.Exception:
                    StringBuilder.AppendFormat(
                        bColor
                            ? "<color=red><b>[EXCEPTION] ► </b></color> - <color=red>{0}</color>"
                            : "<color=red><b>[EXCEPTION] ► </b></color> - {0}",
                        logString);
                    break;
            }

            return StringBuilder;
        }

        private static void LogImp(ELogLevel type, string logString)
        {
            if (type < FilterLevel)
            {
                return;
            }

            StringBuilder infoBuilder = GetFormatString(type, logString, true);
            string logStr = infoBuilder.ToString();

            //获取C#堆栈,Warning以上级别日志才获取堆栈
            if (type == ELogLevel.Error || type == ELogLevel.Warning || type == ELogLevel.Exception)
            {
                StackFrame[] stackFrames = new StackTrace().GetFrames();
                // ReSharper disable once PossibleNullReferenceException
                foreach (var t in stackFrames)
                {
                    StackFrame frame = t;
                    // ReSharper disable once PossibleNullReferenceException
                    string declaringTypeName = frame.GetMethod().DeclaringType.FullName;
                    string methodName = t.GetMethod().Name;

                    infoBuilder.AppendFormat("[{0}::{1}\n", declaringTypeName, methodName);
                }
            }

            switch (type)
            {
                case ELogLevel.Info:
                case ELogLevel.Debug:
                    UnityEngine.Debug.Log(logStr);
                    break;
                case ELogLevel.Warning:
                    UnityEngine.Debug.LogWarning(logStr);
                    break;
                case ELogLevel.Assert:
                    UnityEngine.Debug.LogAssertion(logStr);
                    break;
                case ELogLevel.Error:
                    UnityEngine.Debug.LogError(logStr);
                    break;
                case ELogLevel.Exception:
                    throw new Exception(logStr);
            }
        }
    }
}
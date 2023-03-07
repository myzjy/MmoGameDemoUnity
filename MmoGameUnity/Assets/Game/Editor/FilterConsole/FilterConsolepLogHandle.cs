using System;
using UnityEngine;

namespace FilterConsole
{
   	/// <summary>
	/// 日志关联
	/// </summary>
	public partial class FilterConsoleWindow
	{

		private void AddReceiver()
		{
#if UNITY_5_3_OR_NEWER
			Application.logMessageReceived += HandleLog;
#else
			Application.RegisterLogCallback(HandleLog);
#endif
		}

		private void SubReceiver()
		{
#if UNITY_5_3_OR_NEWER
			Application.logMessageReceived -= HandleLog;
#else
			Application.RegisterLogCallback(null);
#endif
		}

		// ログの受け口
		void HandleLog(string logString, string stackTrace, LogType type)
		{
			LogEntry logEntry = new LogEntry(logString, stackTrace, type);

			// 从上面插入的类型
			{
				// if (logEntryList.Count >= LOG_ENTRY_COUNT)
				// {
				// 	logEntryList.RemoveAt(logEntryList.Count - 1);
				// }
				// logEntryList.Insert(0, logEntry);
			}

			// 从下往上推的类型
			{
				if (logEntryList.Count >= LogEntryCount)
				{
					logEntryList.RemoveAt(0);
				}
				logEntryList.Add(logEntry);

				// 随着滚动幅度的变动，追到最下端
				if (Prefs.GetBool(PrefsKey.AutoScroll))
				{
					logScrollPosition.y = float.MaxValue;
				}
			}

			Repaint();
		}

		public static string TrimLongMessage(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				return "";
			}
			return message.Length > 10000 ? message.Substring(0, 10000) : message;
		}

		/// <summary>
		/// Log信息的一行
		/// </summary>
		public class LogEntry
		{
			private string message;
			public string Message
			{
				get { return message; }
			}

			// 绘图用:控制在GUI允许绘图程度的长度的信息
			private string messageShort;
			public string MessageShort
			{
				get { return messageShort; }
			}
			// Filter比較用
			private string messageLower;
			public string MessageLower
			{
				get { return messageLower; }
			}

			private string stackTrace;
			public string StackTrace
			{
				get { return stackTrace; }
			}
			private string stackTraceLower;
			public string StackTraceLower
			{
				get { return stackTraceLower; }
			}

			private LogType logType;
			public LogType LogType
			{
				get { return logType; }
			}

			private string logTypeText;
			public string LogTypeText
			{
				get { return logTypeText; }
			}

			private DateTime time;
			private string timeText;
			public string TimeText
			{
				get { return timeText; }
			}

			private Color bgColor;
			public Color BgColor
			{
				get { return bgColor; }
			}

			private string detailMessage;
			public string DetailMessage
			{
				get { return detailMessage; }
			}

			// LogEntry 作成
			public LogEntry(string logString, string stackTrace, LogType type)
			{
				message = logString;
				this.stackTrace = stackTrace;

				logType = type;

				logTypeText = $"[{logType}]";

				time = DateTime.Now;
				timeText = time.ToString("HH:mm:ss.fff");

				switch (logType)
				{
					case LogType.Log:
					default:
						bgColor = Color.white;
						break;
					case LogType.Warning:
						// bgColor = Color.yellow;
						bgColor = new Color(1f, 1f, 0.5f, 1f);
						break;
					case LogType.Error:
					case LogType.Exception:
						// bgColor = Color.red;
						bgColor = new Color(1f, 0.5f, 0.5f, 1f);
						break;
				}

				// 使用时只需取得的负荷，生成时全部缓存
				messageShort = TrimLongMessage(message);
				messageLower = !string.IsNullOrEmpty(message) ? message.ToLower() : "";
				stackTraceLower = (!string.IsNullOrEmpty(this.stackTrace) ? this.stackTrace.ToLower() : "");
				detailMessage = $"{LogTypeText} {TimeText}\n\n{Message}\n\n{StackTrace}";
			}

			public bool isDisplay(string filterText)
			{
				if (string.IsNullOrEmpty(filterText))
				{
					return true;
				}
				if (messageLower.Contains(filterText.ToLower()))
				{
					return true;
				}

				// 烦恼了
				// 但是不包含在正文里的东西就会混淆，所以可以选择
				if (Prefs.GetBool(PrefsKey.IncludingStacktrace))
				{
					if (!string.IsNullOrEmpty(stackTraceLower) && stackTraceLower.Contains(filterText.ToLower()))
					{
						return true;
					}
				}
				return false;
			}
		}

	}
}
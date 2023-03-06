using System;
using UnityEngine;

namespace FilterConsole
{
   	/// <summary>
	/// ログ関連
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

			// 上から差し込むタイプ
			{
				// if (logEntryList.Count >= LOG_ENTRY_COUNT)
				// {
				// 	logEntryList.RemoveAt(logEntryList.Count - 1);
				// }
				// logEntryList.Insert(0, logEntry);
			}

			// 下から押し上げるタイプ
			{
				if (logEntryList.Count >= LogEntryCount)
				{
					logEntryList.RemoveAt(0);
				}
				logEntryList.Add(logEntry);

				// スクロール幅の変動タイミングに合わせて下端に追いやる
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
		/// Log情報の1行
		/// </summary>
		public class LogEntry
		{
			private string message;
			public string Message
			{
				get { return message; }
			}

			// 描画用：GUIで描画が許される程度の長さに抑えたメッセージ
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

				// 使用時には取得だけの負荷で済ませるため、生成時に全部キャッシュ
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

				// 悩んだ
				// けど 本文に含まれない物が並ぶと紛らわしくなるのでオプションで
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
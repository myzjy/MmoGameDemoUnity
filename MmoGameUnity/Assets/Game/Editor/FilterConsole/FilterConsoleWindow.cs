using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZJYFrameWork;

namespace FilterConsole
{
    public partial class FilterConsoleWindow : EditorWindow, IHasCustomMenu
    {
        public static void ShowWindow()
        {
            GetWindow<FilterConsoleWindow>("Filter Console", true);
        }
        	public void OnGUI()
		{
			if (splitter == null)
			{
				float y = Prefs.GetFloat(PrefsKey.ResizerRectY);
				splitter = new Splitter(y, DrawTop, DrawBottom, Splitter.SplitMode.Horizonal, 100f);

				rectAfterBottom.y = y;

				drawSplitterRect.x = 0f;
				drawSplitterRect.y = 0f;
			}

			// 监视窗口大小的变化
			drawSplitterRect.width = position.width;
			drawSplitterRect.height = position.height;

			if (splitter.DoSplitter(drawSplitterRect))
			{
				if (Prefs.GetBool(PrefsKey.AutoScroll))
				{
					logScrollPosition.y = float.MaxValue;
				}

				Repaint();

				Prefs.SetFloat(PrefsKey.ResizerRectY, rectAfterBottom.y - ResizerHeight);
			}
		}

		private void DrawTop(Rect rect)
		{
			using(new GUILayout.AreaScope(rect))
			{
				// 调试按钮
				DrawDebug();
				EditorGUILayout.Space();

				// 工具栏
				DrawHeader();
				EditorGUILayout.Space();

				// 滚动浏览日志
				DrawLogList(Rect.zero);
			}
		}

		private void DrawBottom(Rect rect)
		{
			using(new GUILayout.AreaScope(rect))
			{
				// 所选择的记录显示
				DrawDetail(rect);
			}

			rectAfterBottom = rect;
		}

		public void AddItemsToMenu(GenericMenu menu)
		{
			string optionPrefix = "选项";
			menu.AddItem(new GUIContent($"{optionPrefix}/重置设定"), false, () => Prefs.Delete());

			foreach (var key in PrefsKey.BoolKeys)
			{
				string keyName = key.KeyName;
				if (keyName.Contains("DebugHeader"))
				{
					continue;
				}
				menu.AddItem(new GUIContent($"{optionPrefix}/{key.MenuName}"), Prefs.GetBool(keyName), () =>
				{
					Prefs.SetBool(keyName, !Prefs.GetBool(keyName));
				});
			}
		}

		// 不知道可以接受的范围
		private const int LogEntryCount = 10000;
		private static List<LogEntry> logEntryList = new List<LogEntry>(LogEntryCount);

		private Splitter splitter;
		private Rect drawSplitterRect = Rect.zero;
		private const float ResizerHeight = 16f;
		private Rect rectAfterBottom;

		public FilterConsoleWindow()
		{

		}

		void OnEnable()
		{
			AddReceiver();
		}

		void OnDisable()
		{
			SubReceiver();
		}

		private string filterText = "";
		private LogEntry selectLog = null;

		private bool isDisplayLog = true;
		private bool isDisplayError = true;
		private bool isDisplayWarning = true;

		private Vector2 logScrollPosition;
		private Vector2 selectableLabelScrollPosition;

		private System.Text.RegularExpressions.Regex r = null;

		// 剥离UI的焦点
		private void ResetGUIFocus()
		{
			GUI.FocusControl("");
			GUIUtility.keyboardControl = 0;
			Repaint();
		}

		#region 主要功能和绘图

		private void DrawDebug()
		{
			if (!Prefs.GetBool(PrefsKey.DebugHeader))
			{
				return;
			}

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Debug.Log"))
			{
				Guid guidValue = Guid.NewGuid();
				Debug.Log(guidValue.ToString());
			}
			if (GUILayout.Button("Debug.Warning"))
			{
				Guid guidValue = Guid.NewGuid();
				Debug.LogWarning(guidValue.ToString());
			}
			if (GUILayout.Button("Debug.Error"))
			{
				Guid guidValue = Guid.NewGuid();
				Debug.LogError(guidValue.ToString());
			}
			if (GUILayout.Button("Debug.Exception"))
			{
				throw new Exception("Exception!!!");
			}

			EditorGUILayout.EndHorizontal();
		}

		private void DrawHeader()
		{
			Event e = Event.current;

			if (toolbarButtonStyle == null)
			{
				toolbarButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
				toolbarButtonStyle.font = GUI.skin.label.font;
			}

			EditorGUILayout.BeginHorizontal(toolbarButtonStyle, GUILayout.Height(80f));

			if (GUILayout.Button("Clear", toolbarButtonStyle, GUILayout.Width(40)))
			{
				logEntryList.Clear();
				selectLog = null;
				ResetGUIFocus();
			}
			// @Filter - Label
			EditorGUILayout.LabelField("Filter", GUILayout.Width(30));

			const string filterFieldControlName = "FilterTextField";
			GUI.SetNextControlName(filterFieldControlName);
			// @Filter - Field
			filterText = EditorGUILayout.TextField(filterText, FindTextFieldStyle, GUILayout.Width(150f));

			if (string.IsNullOrEmpty(filterText))
			{
				GUILayout.Button("", FindTextCancelEmptyStyle);
			}
			else if (GUILayout.Button("", FindTextCancelStyle))
			{
				filterText = "";
				ResetGUIFocus();
			}

			if (e.command)
			{
				if (e.type == EventType.KeyDown && e.keyCode == KeyCode.F)
				{
					if (!GUI.GetNameOfFocusedControl().Equals(filterFieldControlName))
					{
						GUI.FocusControl(filterFieldControlName);
						Repaint();
					}
				}
			}
			else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
			{
				if (!string.IsNullOrEmpty(GUI.GetNameOfFocusedControl()))
				{
					ResetGUIFocus();
				}
			}

			GUILayout.FlexibleSpace();

			// 从这里往右

			// @Date
			TogglePrefs(PrefsKey.DisplayDate, TimeIcon, toolbarButtonStyle, GUILayout.Width(40));
			// @AutoScroll
			{
				bool prev = Prefs.GetBool(PrefsKey.AutoScroll);
				TogglePrefs(PrefsKey.AutoScroll, ArrowIcon, toolbarButtonStyle, GUILayout.Width(40));
				if (!prev && prev != Prefs.GetBool(PrefsKey.AutoScroll))
				{
					logScrollPosition.y = float.MaxValue;
				}
			}
			// @Fold
			TogglePrefs(PrefsKey.FoldDetail, FoldIcon, toolbarButtonStyle, GUILayout.Width(40));

			GUILayout.Space(20f);

			// @LogTypeButton
			isDisplayLog = TogglePrefs(PrefsKey.DisplayLog, GetLogTypeIcon(LogType.Log), toolbarButtonStyle, GUILayout.Width(40));
			isDisplayWarning = TogglePrefs(PrefsKey.DisplayWarning, GetLogTypeIcon(LogType.Warning), toolbarButtonStyle, GUILayout.Width(40));
			isDisplayError = TogglePrefs(PrefsKey.DisplayError, GetLogTypeIcon(LogType.Error), toolbarButtonStyle, GUILayout.Width(40));

			EditorGUILayout.EndHorizontal();
		}

		private bool TogglePrefs(string key, GUIContent content, GUIStyle style, params GUILayoutOption[] option)
		{
			bool getBool, retToggle;

			getBool = Prefs.GetBool(key);
			retToggle = GUILayout.Toggle(getBool, content, style, option);
			if (getBool != retToggle)
			{
				Prefs.SetBool(key, retToggle);
			}
			return retToggle;
		}

		private void DrawLogList(Rect rect)
		{
			GUI.color = Color.white;

			bool displayDate = Prefs.GetBool(PrefsKey.DisplayDate);
			System.Action<LogEntry> drawTimeText = null;
			if (displayDate)
			{
				drawTimeText = (logEntry) => GUILayout.Label(logEntry.TimeText, GUILayout.Width(80));
			}

			float logTypeIconWidth = 40f;
			float messageWidth = position.width - logTypeIconWidth - 40f;

			using(new EditorGUILayout.HorizontalScope())
			{
				logScrollPosition = EditorGUILayout.BeginScrollView(logScrollPosition);
				int count = logEntryList != null ? logEntryList.Count : 0;
				bool isEven = true;
				for (int i = 0; i < count; ++i)
				{
					LogEntry logEntry = logEntryList[i];

					// LogTypeとFilterのチェック
					if (!AllowDisplay(logEntry))
					{
						continue;
					}

					// GUI.contentColor = Color.gray;
					GUI.backgroundColor = logEntry.BgColor;

					BgStyle.normal.background = isEven ? BoxBgEven : BoxBgOdd;

					GUIStyle lineStyle = BgStyle;
					if (logEntry == selectLog)
					{
						lineStyle = GUI.skin.button;
						GUI.backgroundColor = SelectionBgColor;
					}
					using(new EditorGUILayout.HorizontalScope(lineStyle))
					{
						// @LogType Icon
						Color contentColor = GUI.contentColor;
						if (logEntry != selectLog)
						{
							GUI.contentColor = UnselectIconContentColor;
						}
						GUILayout.Label(GetLogTypeIcon(logEntry.LogType), GUILayout.Width(logTypeIconWidth));
						GUI.contentColor = contentColor;

						// @Date Label
						if (drawTimeText != null)
						{
							drawTimeText.Invoke(logEntry);
						}

						// @Message
						// TODO: heightの算出はLogEntryのコンストラクタに移す
						int lineCount = !string.IsNullOrEmpty(logEntry.MessageShort) ? logEntry.MessageShort.Split('\n').Length : 0;
						float height = Mathf.Clamp(lineCount * 20f, 40f, 120f);
						if (GUILayout.Button(logEntry.MessageShort, LabelStyle, GUILayout.Width(messageWidth), GUILayout.Height(height)))
						{
							if (selectLog != logEntry)
							{
								selectLog = logEntry;
							}
							// 選択済みのものはOpen試す
							else if (Event.current.command || Event.current.control)
							{
								ChoiceScript(selectLog, Event.current.command, Event.current.control);
							}
						}
					}
					isEven = !isEven;
				}
				EditorGUILayout.EndScrollView();
			}
			GUI.backgroundColor = Color.white;
		}

		// 以记录为单位，确认是否显示
		private bool AllowDisplay(LogEntry logEntry)
		{
			if (!isDisplayLog)
			{
				if (logEntry.LogType == LogType.Log)
				{
					return false;
				}
			}
			if (!isDisplayError)
			{
				if (logEntry.LogType == LogType.Error)
				{
					return false;
				}
			}
			if (!isDisplayWarning)
			{
				if (logEntry.LogType == LogType.Warning)
				{
					return false;
				}
			}

			if (!logEntry.isDisplay(filterText))
			{
				return false;
			}

			return true;
		}

		const float DetailAreaHeight = 200f;

		// 所选择的日志的显示
		private void DrawDetail(Rect rect)
		{
			GUI.color = Color.white;

			using(new EditorGUILayout.HorizontalScope(GUI.skin.box, GUILayout.MinHeight(Mathf.Min(DetailAreaHeight, position.height - rect.y)), GUILayout.MaxHeight(position.height - rect.y)))
			{
				string msg = selectLog != null ? selectLog.DetailMessage : "";

				selectableLabelScrollPosition = EditorGUILayout.BeginScrollView(selectableLabelScrollPosition);

				string[] lines = msg.Split('\n');
				if (lines.Length > 10)
				{
					List<string> msgs = LinesToMultilineGroup(lines);

					EditorGUILayout.BeginVertical();
					for (int i = 0; i < msgs.Count; ++i)
					{
						EditorGUILayout.LabelField(msgs[i], LabelStyle, GUILayout.Height(148f));
						// Debug.Log(msgs[i]);
						if (Prefs.GetBool(PrefsKey.FoldDetail))
						{
							break;
						}
					}
					EditorGUILayout.EndVertical();
				}
				// else if (msg.Length > 10000)
				// {
				// 	lines = msg.Split('\n');

				// 	int count = msg.Length / 10000;
				// 	for (int i = 0; i < count; ++i)
				// 	{
				// 		int st = i * 10000;
				// 		string m = msg.Substring(st, Mathf.Min(msg.Length - st, 10000));
				// 		EditorGUILayout.LabelField(m, LabelStyle);
				// 	}

				// }
				else
				{
					EditorGUILayout.LabelField(msg, LabelStyle, GUILayout.Height(rect.height - 10f));
				}

				EditorGUILayout.EndScrollView();

				Event e = Event.current;
				if (e.type == EventType.ContextClick || (e.type == EventType.MouseUp && e.button == 1))
				{
					if (GUILayoutUtility.GetLastRect().Contains(e.mousePosition))
					{
						ShowSubMenu();
					}
				}
			}
		}

		// 把几行总结成一个string
		private List<string> LinesToMultilineGroup(string[] lines)
		{
			List<string> msgs = new List<string>();

			string tmp = "";
			for (int i = 0; i < lines.Length; ++i)
			{
				if (!string.IsNullOrEmpty(tmp))
				{
					tmp += "\n";
				}

				tmp += lines[i];
				if (i > 0 && i % 10 == 0)
				{
					msgs.Add(tmp);
					tmp = "";
				}
			}
			if (!string.IsNullOrEmpty(tmp))
			{
				msgs.Add(tmp);
				tmp = "";
			}

			return msgs;
		}

		private void ShowSubMenu()
		{
			if (selectLog == null)
			{
				return;
			}

			var menu = new GenericMenu();

			AddJumpItem(menu);
			AddCopyItem(menu);

			menu.ShowAsContext();
		}

		// 剪贴板
		private void AddCopyItem(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Copy/All"), false, () =>
			{
				if (!string.IsNullOrEmpty(selectLog.DetailMessage))
				{
					Clipboard = selectLog.DetailMessage;
				}
			});
			if (!string.IsNullOrEmpty(selectLog.Message))
			{
				menu.AddItem(new GUIContent("Copy/Message"), false, () =>
				{
					if (!string.IsNullOrEmpty(selectLog.Message))
					{
						Clipboard = selectLog.Message;
					}
				});
			}
			else
			{
				menu.AddDisabledItem(new GUIContent("Copy/Message"));
			}
			menu.AddItem(new GUIContent("Copy/Stacktrace"), false, () =>
			{
				if (!string.IsNullOrEmpty(selectLog.StackTrace))
				{
					Clipboard = selectLog.StackTrace;
				}
			});
		}

		// 跳跃者 
		private void AddJumpItem(GenericMenu menu)
		{
			int addScriptCount = 0;
			if (!string.IsNullOrEmpty(selectLog.StackTrace))
			{
				string[] lines = selectLog.StackTrace.Split('\n');
				string prefix = "(at ";
				foreach (string line in lines)
				{
					int atIndex = line.LastIndexOf(prefix);
					if (atIndex < 0 || !line.EndsWith(")"))
					{
						continue;
					}

					string file = line.Substring(atIndex + prefix.Length, line.Length - (atIndex + prefix.Length + 1));

					menu.AddItem(new GUIContent("Jump/" + file.Replace("/", "__")), false, () =>
					{
						string path;
						int lineNum;
						MessageToScriptPath(file, out path, out lineNum);
						if (string.IsNullOrEmpty(path))
						{
							return;
						}
						OpenScript(path, lineNum);
					});
					++addScriptCount;
				}
			}

			// Assets/Script.cs(144,3): error CS0000: 
			// 这样的错误时的信息对应
			if (!string.IsNullOrEmpty(selectLog.Message))
			{
				string path;
				int lineNum;
				MessageToScriptPathRegex(selectLog.Message, out path, out lineNum);
				if (!string.IsNullOrEmpty(path))
				{
					string contentText = "Jump/" + path.Replace("/", "__");
					if (lineNum > 0)
					{
						contentText += string.Format(" ({0})", lineNum);
					}
					menu.AddItem(new GUIContent(contentText), false, () =>
					{
						OpenScript(path, lineNum);
					});
					++addScriptCount;
				}
			}
			if (addScriptCount == 0)
			{
				menu.AddDisabledItem(new GUIContent("Jump"));
			}
		}

		// 从StackTrace解释路径
		private void MessageToScriptPath(string source, out string path, out int lineNum)
		{
			string[] s = source.Split(':');
			if (s.Length < 2)
			{
				path = "";
				lineNum = 0;
				return;
			}

			path = s[0];
			if (!int.TryParse(s[1], out lineNum))
			{
				lineNum = 0;
			}
		}

		// Message从正文解释路径
		private void MessageToScriptPathRegex(string source, out string path, out int lineNum)
		{
			if (r == null)
			{
				r = new System.Text.RegularExpressions.Regex(@"Assets(/\w+)+\.\w+\(\d+,\d+\):");
			}

			path = "";
			lineNum = 0;

			if (r.IsMatch(source))
			{
				string[] elem = source.Split(':');
				string[] file = elem[0].Split('(');
				path = file[0];
				string lineNumStr = file[1].Split(',') [0];

				if (!int.TryParse(lineNumStr, out lineNum))
				{
					lineNum = 0;
				}
			}
		}

		// 用您的编辑器打开。
		private void OpenScript(string path, int lineNum)
		{
			var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
			if (script == null)
			{
				return;
			}
			AssetDatabase.OpenAsset(script, lineNum);
		}

		// 是打开脚本，还是聚焦在Project视图中
		private void ChoiceScript(LogEntry logEntry, bool openScript, bool pingObject)
		{
			string path;
			int lineNum;

			if (!string.IsNullOrEmpty(selectLog.StackTrace))
			{
				string[] lines = selectLog.StackTrace.Split('\n');
				string prefix = "(at ";
				foreach (string line in lines)
				{
					int atIndex = line.LastIndexOf(prefix);
					if (atIndex < 0 || !line.EndsWith(")"))
					{
						continue;
					}

					string file = line.Substring(atIndex + prefix.Length, line.Length - (atIndex + prefix.Length + 1));

					MessageToScriptPath(file, out path, out lineNum);
					if (string.IsNullOrEmpty(path))
					{
						return;
					}

					if (openScript)
					{
						OpenScript(path, lineNum);
					}
					else if (Event.current.control)
					{
						var s = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
						if (s != null)
						{
							EditorGUIUtility.PingObject(s);
						}
					}
					// 识别路径就结束了
					return;
				}
			}
			else if (!string.IsNullOrEmpty(selectLog.Message))
			{
				MessageToScriptPathRegex(selectLog.Message, out path, out lineNum);
				if (string.IsNullOrEmpty(path))
				{
					return;
				}
				if (openScript)
				{
					OpenScript(path, lineNum);
				}
				else if (pingObject)
				{
					var s = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
					if (s != null)
					{
						EditorGUIUtility.PingObject(s);
					}
				}
			}
		}

		#endregion

		protected string Clipboard
		{
			get { return EditorGUIUtility.systemCopyBuffer; }
			set
			{
				if (EditorGUIUtility.systemCopyBuffer != value)
				{
					ShowNotification(new GUIContent("复印了一份"));
				}
				EditorGUIUtility.systemCopyBuffer = value;
			}
		}

		private GUIStyle toolbarButtonStyle = null;
		private UIntKeyDictionary<GUIContent> logTypeIcon = null;

		private GUIContent GetLogTypeIcon(LogType logType)
		{
			if (logTypeIcon == null)
			{
				logTypeIcon = new UIntKeyDictionary<GUIContent>(5);
			}

			GUIContent content;
			bool result = logTypeIcon.TryGetValue((uint) logType, out content);
			if (!result || (result && content == null))
			{
				string[] icon = {
					"icons/console.erroricon.png",
					"icons/console.erroricon.png",
					"icons/console.warnicon.png",
					"icons/console.infoicon.png",
					"icons/console.erroricon.png",
				};
				string iconName = "icons/console.erroricon.png";
				if ((int) logType < icon.Length)
				{
					iconName = icon[(int) logType];
				}

				content = EditorGUIUtility.IconContent(iconName);
				if (result)
				{
					logTypeIcon.Remove((uint) logType);
				}
				logTypeIcon.Add((uint) logType, content);
			}

			return content;
		}

		GUIStyle labelStyle;
		GUIStyle LabelStyle
		{
			get
			{
				if (labelStyle == null)
				{
					labelStyle = new GUIStyle(EditorStyles.label);
					labelStyle.richText = true;

				}
				return labelStyle;
			}
		}

		GUIStyle bgStyle;
		GUIStyle BgStyle
		{
			get
			{
				if (bgStyle == null)
				{
					bgStyle = new GUIStyle();
				}
				return bgStyle;
			}
		}
		GUIStyle findTextFieldStyle;
		GUIStyle FindTextFieldStyle
		{
			get
			{
				if (findTextFieldStyle == null)
				{
					findTextFieldStyle = GetStyle("ToolbarSeachTextField");
				}
				return findTextFieldStyle;
			}
		}

		GUIStyle findTextCancelStyle;
		GUIStyle FindTextCancelStyle
		{
			get
			{
				if (findTextCancelStyle == null)
				{
					findTextCancelStyle = GetStyle("ToolbarSeachCancelButton");
				}
				return findTextCancelStyle;
			}
		}

		GUIStyle findTextCancelEmptyStyle;
		GUIStyle FindTextCancelEmptyStyle
		{
			get
			{
				if (findTextCancelEmptyStyle == null)
				{
					findTextCancelEmptyStyle = GetStyle("ToolbarSeachCancelButtonEmpty");
				}
				return findTextCancelEmptyStyle;
			}
		}

		private GUIStyle GetStyle(string styleName)
		{
			GUIStyle guiStyle = GUI.skin.FindStyle(styleName);
			if (guiStyle == null)
			{
				guiStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
			}
			if (guiStyle == null)
			{
				Debug.LogError("Missing built-in guistyle " + styleName);
				guiStyle = new GUIStyle();
			}
			return guiStyle;
		}

		private GUIContent timeIcon;
		private GUIContent TimeIcon
		{
			get
			{
				if (timeIcon == null)
				{
					timeIcon = new GUIContent(EditorGUIUtility.Load("icons/d_UnityEditor.AnimationWindow.png") as Texture2D);
				}
				return timeIcon;
			}
		}

		private GUIContent arrowIcon;
		private GUIContent ArrowIcon
		{
			get
			{
				if (arrowIcon == null)
				{
					arrowIcon = new GUIContent(EditorGUIUtility.Load("icons/d_endButton.png") as Texture2D);
				}
				return arrowIcon;
			}
		}

		private GUIContent foldIcon;
		private GUIContent FoldIcon
		{
			get
			{
				if (foldIcon == null)
				{
					foldIcon = new GUIContent(EditorGUIUtility.Load("icons/d_LookDevMirrorViewsActive@2x.png") as Texture2D);
				}
				return foldIcon;
			}
		}

		private Texture2D boxBgOdd, boxBgEven;
		private Texture2D BoxBgOdd
		{
			get
			{
				if (boxBgOdd == null)
				{
					boxBgOdd = EditorGUIUtility.Load("builtin skins/darkskin/images/cn entrybackodd.png") as Texture2D;
				}
				return boxBgOdd;
			}
		}
		private Texture2D BoxBgEven
		{
			get
			{
				if (boxBgEven == null)
				{
					boxBgEven = EditorGUIUtility.Load("builtin skins/darkskin/images/cnentrybackeven.png") as Texture2D;
				}
				return boxBgEven;
			}
		}

		private readonly Color SelectionBgColor = new Color(0.6f, 0.8f, 1f, 1f);

		private readonly Color UnselectIconContentColor = new Color(0.65f, 0.65f, 0.65f, 1f);

    }
}
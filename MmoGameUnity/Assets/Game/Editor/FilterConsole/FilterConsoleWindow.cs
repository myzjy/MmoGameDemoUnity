using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using ZJYFrameWork;

// ReSharper disable once CheckNamespace
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
            if (_splitter == null)
            {
                float y = Prefs.GetFloat(PrefsKey.ResizerRectY);
                _splitter = new Splitter(y, DrawTop, DrawBottom, Splitter.SplitMode.Horizonal, 100f);

                _rectAfterBottom.y = y;

                _drawSplitterRect.x = 0f;
                _drawSplitterRect.y = 0f;
            }

            // 监视窗口大小的变化
            _drawSplitterRect.width = position.width;
            _drawSplitterRect.height = position.height;

            if (_splitter.DoSplitter(_drawSplitterRect))
            {
                if (Prefs.GetBool(PrefsKey.AutoScroll))
                {
                    _logScrollPosition.y = float.MaxValue;
                }

                Repaint();

                Prefs.SetFloat(PrefsKey.ResizerRectY, _rectAfterBottom.y - ResizerHeight);
            }
        }

        private void DrawTop(Rect rect)
        {
            using (new GUILayout.AreaScope(rect))
            {
                // 调试按钮
                DrawDebug();
                EditorGUILayout.Space();

                // 工具栏
                DrawHeader();
                EditorGUILayout.Space();

                // 滚动浏览日志
                DrawLogList();
            }
        }

        private void DrawBottom(Rect rect)
        {
            using (new GUILayout.AreaScope(rect))
            {
                // 所选择的记录显示
                DrawDetail(rect);
            }

            _rectAfterBottom = rect;
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            string optionPrefix = "选项";
            menu.AddItem(new GUIContent($"{optionPrefix}/重置设定"), false, Prefs.Delete);

            foreach (var key in PrefsKey.BoolKeys)
            {
                string keyName = key.KeyName;
                if (keyName.Contains("DebugHeader"))
                {
                    continue;
                }

                menu.AddItem(new GUIContent($"{optionPrefix}/{key.MenuName}"), Prefs.GetBool(keyName),
                    () => { Prefs.SetBool(keyName, !Prefs.GetBool(keyName)); });
            }
        }

        // 不知道可以接受的范围
        private const int LogEntryCount = 10000;
        private static readonly List<LogEntry> LOGEntryList = new List<LogEntry>(LogEntryCount);

        private Splitter _splitter;
        private Rect _drawSplitterRect = Rect.zero;
        private const float ResizerHeight = 16f;
        private Rect _rectAfterBottom;

        void OnEnable()
        {
            AddReceiver();
        }

        void OnDisable()
        {
            SubReceiver();
        }

        private string _filterText = "";
        private LogEntry _selectLog;

        private bool _isDisplayLog = true;
        private bool _isDisplayError = true;
        private bool _isDisplayWarning = true;

        private Vector2 _logScrollPosition;
        private Vector2 _selectableLabelScrollPosition;

        private System.Text.RegularExpressions.Regex _r;

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

            _toolbarButtonStyle ??= new GUIStyle(EditorStyles.toolbarButton)
            {
                font = GUI.skin.label.font
            };

            EditorGUILayout.BeginHorizontal(_toolbarButtonStyle, GUILayout.Height(80f));

            if (GUILayout.Button("Clear", _toolbarButtonStyle, GUILayout.Width(40)))
            {
                LOGEntryList.Clear();
                _selectLog = null;
                ResetGUIFocus();
            }

            // @Filter - Label
            EditorGUILayout.LabelField("Filter", GUILayout.Width(30));

            const string filterFieldControlName = "FilterTextField";
            GUI.SetNextControlName(filterFieldControlName);
            // @Filter - Field
            _filterText = EditorGUILayout.TextField(_filterText, FindTextFieldStyle, GUILayout.Width(150f));

            if (string.IsNullOrEmpty(_filterText))
            {
                GUILayout.Button("", FindTextCancelEmptyStyle);
            }
            else if (GUILayout.Button("", FindTextCancelStyle))
            {
                _filterText = "";
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
            TogglePrefs(PrefsKey.DisplayDate, TimeIcon, _toolbarButtonStyle, GUILayout.Width(40));
            // @AutoScroll
            {
                bool prev = Prefs.GetBool(PrefsKey.AutoScroll);
                TogglePrefs(PrefsKey.AutoScroll, ArrowIcon, _toolbarButtonStyle, GUILayout.Width(40));
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (!prev && prev != Prefs.GetBool(PrefsKey.AutoScroll))
                {
                    _logScrollPosition.y = float.MaxValue;
                }
            }
            // @Fold
            TogglePrefs(PrefsKey.FoldDetail, FoldIcon, _toolbarButtonStyle, GUILayout.Width(40));

            GUILayout.Space(20f);

            // @LogTypeButton
            _isDisplayLog = TogglePrefs(PrefsKey.DisplayLog, GetLogTypeIcon(LogType.Log), _toolbarButtonStyle,
                GUILayout.Width(40));
            _isDisplayWarning = TogglePrefs(PrefsKey.DisplayWarning, GetLogTypeIcon(LogType.Warning),
                _toolbarButtonStyle, GUILayout.Width(40));
            _isDisplayError = TogglePrefs(PrefsKey.DisplayError, GetLogTypeIcon(LogType.Error), _toolbarButtonStyle,
                GUILayout.Width(40));

            EditorGUILayout.EndHorizontal();
        }

        private bool TogglePrefs(string key, GUIContent content, GUIStyle style, params GUILayoutOption[] option)
        {
            var getBool = Prefs.GetBool(key);
            var retToggle = GUILayout.Toggle(getBool, content, style, option);
            if (getBool != retToggle)
            {
                Prefs.SetBool(key, retToggle);
            }

            return retToggle;
        }

        private void DrawLogList()
        {
            GUI.color = Color.white;

            bool displayDate = Prefs.GetBool(PrefsKey.DisplayDate);
            Action<LogEntry> drawTimeText = null;
            if (displayDate)
            {
                drawTimeText = (logEntry) => GUILayout.Label(logEntry.TimeText, GUILayout.Width(80));
            }

            float logTypeIconWidth = 40f;
            float messageWidth = position.width - logTypeIconWidth - 40f;

            using (new EditorGUILayout.HorizontalScope())
            {
                _logScrollPosition = EditorGUILayout.BeginScrollView(_logScrollPosition);
                int count = LOGEntryList?.Count ?? 0;
                bool isEven = true;
                for (int i = 0; i < count; ++i)
                {
                    if (LOGEntryList != null)
                    {
                        LogEntry logEntry = LOGEntryList[i];

                        // LogType和Filter的检查
                        if (!AllowDisplay(logEntry))
                        {
                            continue;
                        }

                        // GUI.contentColor = Color.gray;
                        GUI.backgroundColor = logEntry.BgColor;

                        BgStyle.normal.background = isEven ? BoxBgEven : BoxBgOdd;

                        GUIStyle lineStyle = BgStyle;
                        if (logEntry == _selectLog)
                        {
                            lineStyle = GUI.skin.button;
                            GUI.backgroundColor = _selectionBgColor;
                        }

                        using (new EditorGUILayout.HorizontalScope(lineStyle))
                        {
                            // @LogType Icon
                            Color contentColor = GUI.contentColor;
                            if (logEntry != _selectLog)
                            {
                                GUI.contentColor = _unselectIconContentColor;
                            }

                            GUILayout.Label(GetLogTypeIcon(logEntry.LogType), GUILayout.Width(logTypeIconWidth));
                            GUI.contentColor = contentColor;

                            // @Date Label
                            if (drawTimeText != null)
                            {
                                drawTimeText.Invoke(logEntry);
                            }

                            // @Message
                            // TODO: height的计算转移到LogEntry的构造函数
                            int lineCount = !string.IsNullOrEmpty(logEntry.MessageShort)
                                ? logEntry.MessageShort.Split('\n').Length
                                : 0;
                            float height = Mathf.Clamp(lineCount * 30f, 40f, 120f);
                            if (GUILayout.Button(logEntry.MessageShort, LabelStyle, GUILayout.Width(messageWidth),
                                    GUILayout.Height(height)))
                            {
                                if (_selectLog != logEntry)
                                {
                                    _selectLog = logEntry;
                                }
                                // 已经选择的东西进行Open尝试
                                else if (Event.current.command || Event.current.control)
                                {
                                    ChoiceScript(_selectLog, Event.current.command, Event.current.control);
                                }
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
            if (!_isDisplayLog)
            {
                if (logEntry.LogType == LogType.Log)
                {
                    return false;
                }
            }

            if (!_isDisplayError)
            {
                if (logEntry.LogType == LogType.Error)
                {
                    return false;
                }
            }

            if (!_isDisplayWarning)
            {
                if (logEntry.LogType == LogType.Warning)
                {
                    return false;
                }
            }

            if (!logEntry.isDisplay(_filterText))
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

            using (new EditorGUILayout.HorizontalScope(GUI.skin.box,
                       GUILayout.MinHeight(Mathf.Min(DetailAreaHeight, position.height - rect.y)),
                       GUILayout.MaxHeight(position.height - rect.y)))
            {
                string msg = _selectLog != null ? _selectLog.DetailMessage : "";

                _selectableLabelScrollPosition = EditorGUILayout.BeginScrollView(_selectableLabelScrollPosition);

                string[] lines = msg.Split('\n');
                if (lines.Length > 10)
                {
                    List<string> msgList = LinesToMultilineGroup(lines);

                    EditorGUILayout.BeginVertical();
                    foreach (var t in msgList)
                    {
                        EditorGUILayout.LabelField(t, LabelStyle, GUILayout.Height(148f));
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
            List<string> msgList = new List<string>();

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
                    msgList.Add(tmp);
                    tmp = "";
                }
            }

            if (!string.IsNullOrEmpty(tmp))
            {
                msgList.Add(tmp);
            }

            return msgList;
        }

        private void ShowSubMenu()
        {
            if (_selectLog == null)
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
                if (!string.IsNullOrEmpty(_selectLog.DetailMessage))
                {
                    Clipboard = _selectLog.DetailMessage;
                }
            });
            if (!string.IsNullOrEmpty(_selectLog.Message))
            {
                menu.AddItem(new GUIContent("Copy/Message"), false, () =>
                {
                    if (!string.IsNullOrEmpty(_selectLog.Message))
                    {
                        Clipboard = _selectLog.Message;
                    }
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Copy/Message"));
            }

            menu.AddItem(new GUIContent("Copy/Stacktrace"), false, () =>
            {
                if (!string.IsNullOrEmpty(_selectLog.StackTrace))
                {
                    Clipboard = _selectLog.StackTrace;
                }
            });
        }

        // 跳跃者 
        private void AddJumpItem(GenericMenu menu)
        {
            int addScriptCount = 0;
            if (!string.IsNullOrEmpty(_selectLog.StackTrace))
            {
                string[] lines = _selectLog.StackTrace.Split('\n');
                string prefix = "(at ";
                foreach (string line in lines)
                {
                    int atIndex = line.LastIndexOf(prefix, StringComparison.Ordinal);
                    if (atIndex < 0 || !line.EndsWith(")"))
                    {
                        continue;
                    }

                    string file = line.Substring(atIndex + prefix.Length, line.Length - (atIndex + prefix.Length + 1));

                    menu.AddItem(new GUIContent("Jump/" + file.Replace("/", "__")), false, () =>
                    {
                        MessageToScriptPath(file, out var path, out var lineNum);
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
            if (!string.IsNullOrEmpty(_selectLog.Message))
            {
                MessageToScriptPathRegex(_selectLog.Message, out var path, out var lineNum);
                if (!string.IsNullOrEmpty(path))
                {
                    string contentText = "Jump/" + path.Replace("/", "__");
                    if (lineNum > 0)
                    {
                        contentText += $" ({lineNum})";
                    }

                    menu.AddItem(new GUIContent(contentText), false, () => { OpenScript(path, lineNum); });
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
            _r ??= new System.Text.RegularExpressions.Regex(@"Assets(/\w+)+\.\w+\(\d+,\d+\):");

            path = "";
            lineNum = 0;

            if (_r.IsMatch(source))
            {
                string[] elem = source.Split(':');
                string[] file = elem[0].Split('(');
                path = file[0];
                string lineNumStr = file[1].Split(',')[0];

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
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void ChoiceScript([NotNull] LogEntry logEntry, bool openScript, bool pingObject)
        {
            if (logEntry == null) throw new ArgumentNullException(nameof(logEntry));
            string path;
            int lineNum;

            if (!string.IsNullOrEmpty(_selectLog.StackTrace))
            {
                string[] lines = _selectLog.StackTrace.Split('\n');
                string prefix = "(at ";
                foreach (string line in lines)
                {
                    int atIndex = line.LastIndexOf(prefix, StringComparison.Ordinal);
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
            else if (!string.IsNullOrEmpty(_selectLog.Message))
            {
                MessageToScriptPathRegex(_selectLog.Message, out path, out lineNum);
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

        private string Clipboard
        {
            // ReSharper disable once UnusedMember.Local
            get => EditorGUIUtility.systemCopyBuffer;
            set
            {
                if (EditorGUIUtility.systemCopyBuffer != value)
                {
                    ShowNotification(new GUIContent("复印了一份"));
                }

                EditorGUIUtility.systemCopyBuffer = value;
            }
        }

        private GUIStyle _toolbarButtonStyle;
        private UIntKeyDictionary<GUIContent> _logTypeIcon;

        private GUIContent GetLogTypeIcon(LogType logType)
        {
            _logTypeIcon ??= new UIntKeyDictionary<GUIContent>(5);

            bool result = _logTypeIcon.TryGetValue((uint)logType, out var content);
            if (!result || (content == null))
            {
                string[] icon =
                {
                    "icons/console.erroricon.png",
                    "icons/console.erroricon.png",
                    "icons/console.warnicon.png",
                    "icons/console.infoicon.png",
                    "icons/console.erroricon.png",
                };
                string iconName = "icons/console.erroricon.png";
                if ((int)logType < icon.Length)
                {
                    iconName = icon[(int)logType];
                }

                content = EditorGUIUtility.IconContent(iconName);
                if (result)
                {
                    _logTypeIcon.Remove((uint)logType);
                }

                _logTypeIcon.Add((uint)logType, content);
            }

            return content;
        }

        GUIStyle _labelStyle;

        GUIStyle LabelStyle =>
            _labelStyle ??= new GUIStyle(EditorStyles.label)
            {
                richText = true
            };

        GUIStyle _bgStyle;
        GUIStyle BgStyle => _bgStyle ??= new GUIStyle();
        GUIStyle _findTextFieldStyle;
        GUIStyle FindTextFieldStyle => _findTextFieldStyle ?? (_findTextFieldStyle = GetStyle("ToolbarSeachTextField"));

        GUIStyle _findTextCancelStyle;

        GUIStyle FindTextCancelStyle =>
            _findTextCancelStyle ?? (_findTextCancelStyle = GetStyle("ToolbarSeachCancelButton"));

        GUIStyle _findTextCancelEmptyStyle;

        GUIStyle FindTextCancelEmptyStyle =>
            _findTextCancelEmptyStyle ??
            (_findTextCancelEmptyStyle = GetStyle("ToolbarSeachCancelButtonEmpty"));

        private GUIStyle GetStyle(string styleName)
        {
            GUIStyle guiStyle = GUI.skin.FindStyle(styleName) ??
                                EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
            if (guiStyle == null)
            {
                Debug.LogError("Missing built-in guistyle " + styleName);
                guiStyle = new GUIStyle();
            }

            return guiStyle;
        }

        private GUIContent _timeIcon;

        private GUIContent TimeIcon =>
            _timeIcon ??= new GUIContent(EditorGUIUtility.Load("icons/d_UnityEditor.AnimationWindow.png") as Texture2D);

        private GUIContent _arrowIcon;

        private GUIContent ArrowIcon =>
            _arrowIcon ??= new GUIContent(EditorGUIUtility.Load("icons/d_endButton.png") as Texture2D);

        private GUIContent _foldIcon;

        private GUIContent FoldIcon =>
            _foldIcon ??= new GUIContent(EditorGUIUtility.Load("icons/d_LookDevMirrorViewsActive@2x.png") as Texture2D);

        private Texture2D _boxBgOdd, _boxBgEven;

        private Texture2D BoxBgOdd
        {
            get
            {
                if (_boxBgOdd == null)
                {
                    _boxBgOdd = EditorGUIUtility.Load("builtin skins/darkskin/images/cn entrybackodd.png") as Texture2D;
                }

                return _boxBgOdd;
            }
        }

        private Texture2D BoxBgEven
        {
            get
            {
                if (_boxBgEven == null)
                {
                    _boxBgEven =
                        EditorGUIUtility.Load("builtin skins/darkskin/images/cnentrybackeven.png") as Texture2D;
                }

                return _boxBgEven;
            }
        }

        private readonly Color _selectionBgColor = new Color(0.6f, 0.8f, 1f, 1f);

        private readonly Color _unselectIconContentColor = new Color(0.65f, 0.65f, 0.65f, 1f);
    }
}
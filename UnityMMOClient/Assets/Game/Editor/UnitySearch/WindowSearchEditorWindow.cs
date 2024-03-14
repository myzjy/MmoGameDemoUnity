using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace UnitySearch
{
    internal class WindowSearchEditorWindow : EditorWindow
    {
        private static WindowSearchEditorWindow _editorWindow;
        private static Type _mainWindowType;
        private static FieldInfo _fieldInfo;
        private static PropertyInfo _propertyInfo;

        internal static void CloseWindow()
        {
            if (_editorWindow != null)
                _editorWindow.Close();
        }

        internal static void ShowWindow()
        {
            if (_editorWindow != null)
                _editorWindow.Close();

            var window = CreateInstance<WindowSearchEditorWindow>();

            window.position = new Rect(window.position.x, window.position.y, UnitySearchConstant.WIDTH,
                UnitySearchConstant.HEIGHT);

            var main = CenterPosition();
            var pos = window.position;
            var w = (main.width - pos.width) * 0.5f;
            var h = (main.height - pos.height) * 0.5f;
            pos.x = main.x + w;
            pos.y = main.y + h;
            window.position = pos;

            window.ShowPopup();
            window.Focus();
        }

        private static Rect CenterPosition()
        {
            try
            {
                _mainWindowType ??= AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .First(t => t.IsSubclassOf(typeof(ScriptableObject)) && t.Name == "ContainerWindow");

                if (_fieldInfo == null)
                {
                    _fieldInfo = _mainWindowType.GetField("m_ShowMode", BindingFlags.NonPublic | BindingFlags.Instance);
                }

                if (_propertyInfo == null)
                {
                    _propertyInfo =
                        _mainWindowType.GetProperty("position", BindingFlags.Public | BindingFlags.Instance);
                }

                foreach (var window in Resources.FindObjectsOfTypeAll(_mainWindowType))
                {
                    if (_fieldInfo == null || (int)_fieldInfo.GetValue(window) != 4) continue;
                    if (_propertyInfo != null)
                    {
                        return (Rect)_propertyInfo.GetValue(window, null);
                    }
                }
            }
            catch
            {
                // ignored
            }

            return Rect.zero;
        }

        private Rect _titleRect;

        private Rect _inputRect;
        private Rect _resultRect;
        private bool isFocus;

        private UnitySearchTreeView _view;
        [SerializeField] private TreeViewState _state;
        private UnitySearchField _search;
        private Texture2D _inputTexture;
        private int _countTimer;
        private string _searchString;

        /// <summary>
        /// 0.5sec
        /// </summary>
        private const int SearchCount = 3;

        private void OnEnable()
        {
            _editorWindow = this;

            isFocus = true;
            _inputTexture = new Texture2D(1, 1);
            _inputTexture.SetPixel(0, 0,
                EditorGUIUtility.isProSkin ? new Color(0.218f, 0.218f, 0.218f) : new Color(0.818f, 0.818f, 0.818f));

            _inputTexture.Apply();
        }

        private void OnDisable()
        {
            if (_editorWindow == this)
                _editorWindow = null;

            if (_inputTexture != null)
                DestroyImmediate(_inputTexture, true);
        }

        private void OnGUI()
        {
            ProcessKeyboardEvents();
            DrawGUI();
            if (!isFocus) return;
            _search.SetFocus();
            isFocus = false;
        }

        private void ProcessKeyboardEvents()
        {
            var ev = Event.current;
            var keyCode = ev.keyCode;

            switch (keyCode)
            {
                case KeyCode.Escape:
                    Close();
                    break;
                case KeyCode.Return:
                    _view.ActionCurrentSelection();
                    Close();
                    break;
            }
        }

        private void Init()
        {
            _state ??= new TreeViewState();
            _view ??= new UnitySearchTreeView(_state);
            if (_search != null) return;
            _search = new UnitySearchField();
            _search.downOrUpArrowKeyPressed += _view.DownOrUpArrowKeyPressed;
            _search.SetFocus();
        }

        /// <summary>
        /// 以每秒10帧的速度调用，给检查器更新的机会。
        /// </summary>
        private void OnInspectorUpdate()
        {
            if (focusedWindow != this)
                Close();

            if (_countTimer <= 0)
                return;

            _countTimer -= 1;

            if (_countTimer <= 0)
            {
                _view.searchString = _searchString;
                _view.Reload();
            }
        }

        private void DrawGUI()
        {
            if (this == null)
                return;

            Init();

            DrawInput();

            DrawSearchResult();
        }

        /// <summary>
        /// 输入搜索字段
        /// </summary>
        private void DrawInput()
        {
            _inputRect = new Rect
            {
                x = 1,
                y = 1,
                width = position.width - 2,
                height = UnitySearchConstant.INPUT_HEIGHT - 2
            };

            GUI.DrawTexture(_inputRect, _inputTexture);

            EditorGUI.BeginChangeCheck();
            _searchString = _search.OnGUI(_inputRect, _searchString, UnitySearchConstant.SearchStyle,
                UnitySearchConstant.ClearButtonStyle);
            if (EditorGUI.EndChangeCheck())
            {
                if (string.IsNullOrEmpty(_searchString))
                {
                    _view.searchString = _searchString;
                    _view.Reload();
                }
                else
                    _countTimer = SearchCount;
            }

            var iconRect = new Rect(_inputRect)
            {
                x = 10,
                width = 20,
            };
            GUI.DrawTexture(iconRect, UnitySearchConstant.SearchTexture, ScaleMode.ScaleToFit);
        }

        /// <summary>
        /// SearchResult
        /// </summary>
        private void DrawSearchResult()
        {
            _resultRect = new Rect(position)
            {
                x = 0,
                y = UnitySearchConstant.INPUT_HEIGHT,
                height = UnitySearchConstant.RESULT_HEIGHT,
            };

            _view.OnGUI(_resultRect);
        }

        /// <summary>
        /// 检索语句的替换
        /// </summary>
        internal void SetSearchString(string str)
        {
            _view.searchString = _searchString = str;
            _view.Reload();
        }
    }
}
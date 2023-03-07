#if UNITY_EDITOR && DEVELOP_BUILD

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Tools.BundleLog
{
    public class BundleLogWindow : EditorWindow
    {
        [MenuItem("Tools/Window/BundleLog")]
        private static void ShowWindow()
        {
            var window = GetWindow<BundleLogWindow>();
            window.titleContent = new GUIContent("BundleLog");
        }

        private BundleLogTreeView _treeView;
        private SearchField searchField;

        [SerializeField] private bool _isAutoReload;

        [SerializeField] private TreeViewState treeViewState;

        private bool _clearOnPlay;
        private bool _isDependencySearch;
        private string _filter;

        private void OnEnable()
        {
            Init();
            EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
            if (_isAutoReload)
            {
                BundleLogData.UpdateLogEvent += BundleLogDataOnUpdateLogEvent;
            }
        }

        private void EditorApplicationOnplayModeStateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.EnteredPlayMode)
            {
                Clear();
            }
        }

        private void OnDisable()
        {
            BundleLogData.UpdateLogEvent -= BundleLogDataOnUpdateLogEvent;
            EditorApplication.playModeStateChanged -= EditorApplicationOnplayModeStateChanged;
        }

        private void Init()
        {
            treeViewState ??= new TreeViewState();

            _treeView ??= new BundleLogTreeView(treeViewState);

            if (searchField == null)
            {
                searchField = new SearchField();
                searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;
            }

            _treeView?.Reload(_filter, _isDependencySearch);
        }

        private void OnGUI()
        {
            Init();

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Space(10);

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    _filter = searchField.OnToolbarGUI(_filter);
                    if (check.changed)
                    {
                        _treeView.Reload(_filter, _isDependencySearch);
                    }
                }

                GUILayout.Space(5);

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    _isDependencySearch = GUILayout.Toggle(_isDependencySearch, "Dependency Search",
                        EditorStyles.toolbarButton);
                    if (check.changed)
                    {
                        _treeView.Reload(_filter, _isDependencySearch);
                    }
                }

                GUILayout.FlexibleSpace();

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    _isAutoReload = GUILayout.Toggle(_isAutoReload, "AutoReload", EditorStyles.toolbarButton);
                    if (check.changed)
                    {
                        BundleLogData.UpdateLogEvent -= BundleLogDataOnUpdateLogEvent;
                        if (_isAutoReload)
                        {
                            BundleLogData.UpdateLogEvent += BundleLogDataOnUpdateLogEvent;
                        }
                    }
                }

                _clearOnPlay = GUILayout.Toggle(_clearOnPlay, "Clear On Play", EditorStyles.toolbarButton);

                GUILayout.Space(10);

                if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
                {
                    Clear();
                }

                if (GUILayout.Button("Reload", EditorStyles.toolbarButton))
                {
                    _treeView?.Reload(_filter, _isDependencySearch);
                }
            }

            var rect = GUILayoutUtility.GetRect(0, float.MaxValue, 0, float.MaxValue);
            _treeView.OnGUI(rect);
        }

        private void Clear()
        {
            BundleLogData.Clear();
            _treeView?.CollapseAll();
            _treeView?.Reload(_filter, _isDependencySearch);
        }

        private void BundleLogDataOnUpdateLogEvent(IEnumerable<BundleData> datas)
        {
            _treeView?.Reload(_filter, _isDependencySearch);
        }
    }
}

#endif
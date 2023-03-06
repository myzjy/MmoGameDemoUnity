using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnitySearch
{
    public class WindowProvider : Provider
    {
        private class WindowInfo
        {
            public readonly string DisplayName;
            public readonly Type Type;

            public WindowInfo(string name, Type type)
            {
                DisplayName = name;
                Type = type;
            }
        }

        // UnityのBuild In Windowは全取得でもいいが名前が違うのでこちらで定義
        private static List<WindowInfo> _editorWindowTypes = new List<WindowInfo>()
        {
            new WindowInfo("Hierarchy", typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow")),
            new WindowInfo("Game", typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView")),
            new WindowInfo("Scene", typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneView")),
            new WindowInfo("Project", typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser")),
            new WindowInfo("Console", typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow")),
            new WindowInfo("Inspector", typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow")),
            new WindowInfo("Animation", typeof(EditorWindow).Assembly.GetType("UnityEditor.AnimationWindow")),
            new WindowInfo("Profiler", typeof(EditorWindow).Assembly.GetType("UnityEditor.ProfilerWindow")),
            new WindowInfo("AssetStore", typeof(EditorWindow).Assembly.GetType("UnityEditor.AssetStoreWindow")),
            new WindowInfo("FrameDebugger", typeof(EditorWindow).Assembly.GetType("UnityEditor.FrameDebuggerWindow")),
            new WindowInfo("SpritePacker",
                typeof(UnityEditor.Sprites.Packer).Assembly.GetType("UnityEditor.Sprites.PackerWindow")),
        };

        public WindowProvider()
        {
            _editorWindowTypes.AddRange(
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t =>
                        t.IsSubclassOf(typeof(EditorWindow)) &&
                        !t.IsSubclassOf(typeof(PopupWindow)) &&
                        t != typeof(WindowSearchEditorWindow)
                    )
                    .Select(t => new WindowInfo(t.Name, t))
                    .ToList());
        }

        private static readonly string Tag = "win";

        internal override string GetTag()
        {
            return Tag;
        }

        internal override IEnumerable<UnitySearchTreeViewItem> SearchItems(string searchText)
        {
            return _editorWindowTypes
                .Where(i => i.DisplayName.StartsWith(searchText, StringComparison.OrdinalIgnoreCase))
                .Select(i => GetItem(i.DisplayName + ":" + i.Type.FullName));
        }

        internal override UnitySearchTreeViewItem GetItem(string key)
        {
            var splits = key.Split(':');
            return new UnitySearchTreeViewItem(this, key, splits[0])
            {
                displayName = splits[0],
                SubDisplayName = splits[1],
                icon = UnitySearchConstant.WindowTexture,
            };
        }

        internal override void Action(string name)
        {
            var info = _editorWindowTypes.FirstOrDefault(t => t.DisplayName == name);
            if (info == null)
            {
                return;
            }

            var window = EditorWindow.GetWindow(info.Type);
            window.Focus();
        }
    }
}
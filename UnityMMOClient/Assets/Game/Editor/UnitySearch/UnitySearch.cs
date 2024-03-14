using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace UnitySearch
{
    public class UnitySearch
    {
        [MenuItem("Window/UnitySearch %#w")]
        private static void ShowWindow()
        {
            WindowSearchEditorWindow.ShowWindow();
        }

        private static readonly Provider[] _providers;

        static UnitySearch()
        {
            _providers = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(Provider)))
                .Select(t => Activator.CreateInstance(t) as Provider)
                .OrderBy(t => t.GetPriority())
                .ToArray();
        }

        public static readonly string TAG_KEYWORD = ":";
        private static readonly List<TreeViewItem> _empty = new List<TreeViewItem>();

        internal static IList<TreeViewItem> SearchResult(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return _empty;
            }

            string tag = string.Empty;
            int tagIndex = searchText.IndexOf(TAG_KEYWORD, StringComparison.Ordinal);
            switch (tagIndex)
            {
                case > 0:
                    tag = searchText[..tagIndex];
                    searchText = searchText[(tagIndex + 1)..];
                    break;
            }

            return _providers
                .Where(p =>
                    (string.IsNullOrEmpty(tag) && !p.TagSearchOnly()) ||
                    p.GetTag() == tag ||
                    (string.IsNullOrEmpty(searchText)
                     && p.AllowEmptySearch()))
                .SelectMany(p => p.SearchItems(searchText).Take(p.SearchMaxCount()))
                .Select((p, i) =>
                {
                    p.id = i;
                    return p as TreeViewItem;
                })
                .ToList();
        }

        internal static Provider GetProvider(string type)
        {
            return _providers?.FirstOrDefault(p => p.GetType().Name == type);
        }
    }
}
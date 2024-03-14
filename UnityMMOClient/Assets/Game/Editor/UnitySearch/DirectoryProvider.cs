using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

// ReSharper disable once CheckNamespace
namespace UnitySearch
{
    public class DirectoryProvider : AssetProvider
    {
        private static readonly string Tag = "dir";

        internal override string GetTag()
        {
            return Tag;
        }

        internal override IEnumerable<UnitySearchTreeViewItem> SearchItems(string searchText)
        {
            int index = searchText.IndexOf("/", StringComparison.Ordinal);
            if (index < 0)
                return new List<UnitySearchTreeViewItem>();

            string dirName = searchText[..index];
            string fileName = searchText[(index + 1)..];

            return AssetDatabase.FindAssets($"t:folder {dirName}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .SelectMany(p => AssetDatabase.FindAssets(fileName, new[] { p }))
                .Select(GetItem);
        }

        public override bool TagSearchOnly()
        {
            return true;
        }
    }
}
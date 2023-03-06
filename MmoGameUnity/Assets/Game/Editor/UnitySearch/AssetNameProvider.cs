using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

// ReSharper disable once CheckNamespace
namespace UnitySearch
{
    public class AssetNameProvider : Provider
    {
        internal override string GetTag()
        {
            return "abn";
        }

        internal override IEnumerable<UnitySearchTreeViewItem> SearchItems(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return Array.Empty<UnitySearchTreeViewItem>();

            var assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            return assetBundleNames
                .Where(abn => abn.StartsWith(searchText, StringComparison.OrdinalIgnoreCase))
                .SelectMany(AssetDatabase.GetAssetPathsFromAssetBundle)
                .Distinct()
                .Select(GetItem);
        }

        internal override void Action(string path)
        {
            Selection.objects = new[] {AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path)};
        }

        internal override UnitySearchTreeViewItem GetItem(string path)
        {
            var importer = AssetImporter.GetAtPath(path);
            return new UnitySearchTreeViewItem(this, path)
            {
                displayName = importer.assetBundleName,
                SubDisplayName = path,
                CanDrag = true,
            };

        }

        public override bool TagSearchOnly()
        {
            return true;
        }
    }
}
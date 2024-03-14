using System.Collections.Generic;
using UnityEditor;

// ReSharper disable once CheckNamespace
namespace UnitySearch
{
    public class GuidProvider : AssetProvider
    {
        private static readonly string Tag = "guid";

        internal override string GetTag()
        {
            return Tag;
        }

        internal override IEnumerable<UnitySearchTreeViewItem> SearchItems(string searchText)
        {
            var path = AssetDatabase.GUIDToAssetPath(searchText);
            if (string.IsNullOrEmpty(path))
            {
                return new List<UnitySearchTreeViewItem>();
            }

            return new List<UnitySearchTreeViewItem>()
            {
                new(this, searchText)
                {
                    displayName = System.IO.Path.GetFileName(path),
                    SubDisplayName = path,
                }
            };
        }

        public override bool TagSearchOnly()
        {
            return true;
        }
    }
}
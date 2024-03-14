using System.Collections.Generic;
using System.Linq;
using UnityEditor;

// ReSharper disable once CheckNamespace
namespace UnitySearch
{
    /// <summary>
    ///捆绑名称搜索
    /// </summary>
    public class BundleNameProvider : AssetProvider
    {
        private static readonly string Tag = "bn";

        internal override string GetTag()
        {
            return Tag;
        }

        internal override IEnumerable<UnitySearchTreeViewItem> SearchItems(string searchText)
        {
            var paths = AssetDatabase.GetAssetPathsFromAssetBundle(searchText + ".assetbundle");
            if (paths.Length <= 0)
                return new List<UnitySearchTreeViewItem>();


            return paths.Select(p => new UnitySearchTreeViewItem(this, AssetDatabase.AssetPathToGUID(p))
            {
                displayName = System.IO.Path.GetFileName(p),
                SubDisplayName = p,
            });
        }
    }
}
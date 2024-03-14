using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace UnitySearch
{
    public class HierarchyLogProvider : HierarchyProvider
    {
        private const string Tag = "hl";
        internal override string GetTag()
        {
            return Tag;
        }

        internal override IEnumerable<UnitySearchTreeViewItem> SearchItems(string searchText)
        {
            return UnitySearchHierarchyLog.Logs.Select(GetItem);
        }
		
        public override bool TagSearchOnly()
        {
            return true;
        }
    }
}
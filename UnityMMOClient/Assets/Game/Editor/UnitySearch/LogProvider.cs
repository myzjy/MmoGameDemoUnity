using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace UnitySearch
{
    public class LogProvider : Provider
    {
        private static readonly string Tag = "log";
        private Provider _providerImplementation;

        internal override string GetTag()
        {
            return Tag;
        }

        internal override IEnumerable<UnitySearchTreeViewItem> SearchItems(string searchText)
        {
            return UnitySearchLog.Logs.Select(i =>
            {
                var provider = UnitySearch.GetProvider(i.TypeName);
                if (provider == null)
                    return null;
				
                return provider.GetItem(i.Key);
            });
        }

        internal override void Action(string id)
        {
        }

        internal override UnitySearchTreeViewItem GetItem(string key)
        {
            return null;
        }

        public override bool TagSearchOnly()
        {
            return true;
        }
    }
}
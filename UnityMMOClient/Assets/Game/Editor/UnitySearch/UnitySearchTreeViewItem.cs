using UnityEditor.IMGUI.Controls;

namespace UnitySearch
{
    public class UnitySearchTreeViewItem : TreeViewItem
    {
        private readonly Provider _owner;
        private readonly string _key;

        internal string TypeName;
        internal string FullKey;
        internal string SubDisplayName;
        internal bool CanDrag;

        public UnitySearchTreeViewItem(Provider owner, string fullKey, string key = "")
        {
            _owner = owner;
            FullKey = fullKey;
            _key = key;
            if (string.IsNullOrEmpty(key))
                _key = fullKey;

            TypeName = owner.GetType().Name;
        }

        public void Action()
        {
            _owner.Action(_key);
        }

        public void DragAndDrop()
        {
            _owner.DragAndDrop(_key);
        }
    }
}
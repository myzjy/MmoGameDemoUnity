#if UNITY_EDITOR && DEVELOP_BUILD

using UnityEditor.IMGUI.Controls;

namespace Tools.BundleLog
{
	public class BundleLogTreeViewItem : TreeViewItem
	{
		public BundleLogTreeViewItem(int id, int depth, string name)
		{
			this.id = id;
			this.depth = depth;
			displayName = name;
		}
	}
}

#endif
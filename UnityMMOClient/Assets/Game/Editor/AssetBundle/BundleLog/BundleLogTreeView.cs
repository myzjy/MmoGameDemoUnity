#if UNITY_EDITOR && DEVELOP_BUILD

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Tools.BundleLog
{
	internal class BundleLogTreeView : TreeView
	{
		private readonly TreeViewItem _root;

		internal BundleLogTreeView(TreeViewState state) : base(state)
		{
			showAlternatingRowBackgrounds = true;
			showBorder = true;
			_root = new TreeViewItem {
				id = 0,
				depth = -1,
				children = new List<TreeViewItem>(),
			};

			Reload();
		}

		internal void Reload(string filter, bool isDependencySearch)
		{
			_root.children.Clear();

			foreach (var item in BundleLogData.Logs)
			{
				if (IsHidden(filter, isDependencySearch, item))
					continue;

				var id = (_root.children.Count + 1) * Data.LogMax;
				var parent = new TreeViewItem(id, 0, item.Name) {
					parent = _root,
					children = new List<TreeViewItem>(item.Dependencies
						.Select((i, index) => new BundleLogTreeViewItem(index + id + 1, 1, i))
						.ToList()),
				};
				_root.children.Add(parent);
			}

			if (_root.children.Count > 0)
			{
				SetupDepthsFromParentsAndChildren(_root);
			}

			Reload();
		}

		/// <summary>
		/// 是否显示要素
		/// </summary>
		private bool IsHidden(string filter, bool isDependencySearch, BundleData data)
		{
			if (string.IsNullOrEmpty(filter))
				return false;

			if (!isDependencySearch)
				return data.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0;

			if (data.Dependencies.Length <= 0)
				return true;

			foreach (string dependency in data.Dependencies)
			{
				if (dependency.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
					return false;
			}

			return true;
		}

		protected override TreeViewItem BuildRoot()
		{
			return _root;
		}

		protected override void DoubleClickedItem(int id)
		{
			var item = GetRows().First(r => r.id == id);
			if (item == null)
				return;

			var paths = AssetDatabase.GetAssetPathsFromAssetBundle(item.displayName);
			if (paths.Length <= 0)
			{
				Debug.LogError("Bundle Not Found");
				return;
			}

			var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(paths.First());
			Selection.objects = new[] {obj};
		}
	}
}

#endif
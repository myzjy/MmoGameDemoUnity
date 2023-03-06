using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnitySearch
{
	public class UnitySearchTreeView : TreeView
	{
		public UnitySearchTreeView(TreeViewState state) : base(state)
		{
			Reload();
		}

		protected override TreeViewItem BuildRoot()
		{
			return new TreeViewItem {id = -1, depth = -1};
		}

		protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			var items = UnitySearch.SearchResult(searchString);
			SetupParentsAndChildrenFromDepths(root, items.ToArray());
			SetSelection(new List<int>(0));
			return items; 
		}

		protected override bool CanMultiSelect(TreeViewItem item)
		{
			return false;
		}

		protected override bool CanStartDrag(CanStartDragArgs args)
		{
			var item = args.draggedItem as UnitySearchTreeViewItem;
			return item.CanDrag;
		}

		protected override float GetCustomRowHeight(int row, TreeViewItem item)
		{
			return UnitySearchConstant.ROW_HEIGHT;
		}
		
		public void DownOrUpArrowKeyPressed(bool isUp)
		{
			var rows = GetRows();
			if (rows.Count <= 0)
				return;

			int index = 0;
			var selectId = new List<int>();
			if (HasSelection())
			{
				var id = GetSelection()[0];
				for (int i = 0; i < rows.Count; i++)
					if (rows[i].id == id)
						index = i;
			
				index += isUp ? -1 : 1;
				if (index < 0)
					index = rows.Count - 1;
				if (index >= rows.Count)
					index = 0;
			}
			selectId.Add(rows[index].id);
			SetSelection(selectId, TreeViewSelectionOptions.RevealAndFrame);
		}

		protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
		{
			if (args.draggedItemIDs.Count <= 0)
				return;
			
			var rows = GetRows();
			if (rows.Count <= 0)
				return;

			var item = rows[args.draggedItemIDs[0]] as UnitySearchTreeViewItem;
			UnitySearchLog.AddLog(item);
			item.DragAndDrop();
			WindowSearchEditorWindow.CloseWindow();
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			var item = (UnitySearchTreeViewItem)args.item;
			var position = args.rowRect;
			
			// backGround
			if (Event.current.type == EventType.Repaint)
				UnitySearchConstant.DrawBackground(position, args.row % 2 != 0, args.selected);

			var iconRect = new Rect(position)
			{
				x = 4,
				y = position.y + 2,
				height = UnitySearchConstant.ROW_HEIGHT - 4,
				width = UnitySearchConstant.ROW_HEIGHT - 4,
			};
			if (item.icon != null)
				GUI.DrawTexture(iconRect, item.icon, ScaleMode.ScaleToFit);

			EditorGUI.LabelField(position, item.displayName, UnitySearchConstant.RowLabelStyle);
			
			EditorGUI.LabelField(position, item.SubDisplayName, UnitySearchConstant.RowSubLabelStyle);
		}

		public void ActionCurrentSelection()
		{
			var rows = GetRows();
			if (rows.Count <= 0)
				return;
			
			int index = HasSelection() ? GetSelection()[0] : rows[0].id;
			if (index >= rows.Count)
				return;
			
			var item = rows[index] as UnitySearchTreeViewItem;
			UnitySearchLog.AddLog(item);
			item.Action();
		}
	}
}
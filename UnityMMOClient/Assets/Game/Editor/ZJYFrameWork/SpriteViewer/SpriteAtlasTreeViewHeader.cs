using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ZJYFrameWork.SpriteViewer
{
    internal class SpriteAtlasTreeViewHeader : MultiColumnHeader
    {
        public bool IsFavoriteMode = false;

        private Action<bool> onFavoriteModeChanged;

        public event Action<bool> OnFavoriteModeChanged
        {
            add { onFavoriteModeChanged += value; }
            remove { onFavoriteModeChanged -= value; }
        }

        private Action onFavoriteClearClicked;

        public event Action OnFavoriteClearClicked
        {
            add { onFavoriteClearClicked += value; }
            remove { onFavoriteClearClicked -= value; }
        }

        private Action onExpandAllClicked;

        public event Action OnExpandAllClicked
        {
            add { onExpandAllClicked += value; }
            remove { onExpandAllClicked -= value; }
        }

        private Action onCollapseAllClicked;

        public event Action OnCollapseAllClicked
        {
            add { onCollapseAllClicked += value; }
            remove { onCollapseAllClicked -= value; }
        }

        public SpriteAtlasTreeViewHeader(MultiColumnHeaderState state) : base(state)
        {
        }

        protected override void AddColumnHeaderContextMenuItems(GenericMenu menu)
        {
            base.AddColumnHeaderContextMenuItems(menu);
            menu.AddItem(new GUIContent("Show Only Favorite"), IsFavoriteMode, () =>
            {
                IsFavoriteMode = !IsFavoriteMode;
                if (onFavoriteModeChanged != null)
                {
                    onFavoriteModeChanged.Invoke(IsFavoriteMode);
                }
            });

            menu.AddItem(new GUIContent("Clear All Favorite"), false, () =>
            {
                if (onFavoriteClearClicked != null)
                {
                    onFavoriteClearClicked.Invoke();
                }
            });

            menu.AddSeparator(string.Empty);

            menu.AddItem(new GUIContent("Expand All"), false, () =>
            {
                if (onExpandAllClicked != null)
                {
                    onExpandAllClicked.Invoke();
                }
            });

            menu.AddItem(new GUIContent("Collapse All"), false, () =>
            {
                if (onCollapseAllClicked != null)
                {
                    onCollapseAllClicked.Invoke();
                }
            });
        }
    }
}
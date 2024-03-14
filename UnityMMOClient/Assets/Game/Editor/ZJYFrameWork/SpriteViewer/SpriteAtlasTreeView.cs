using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace ZJYFrameWork.SpriteViewer
{
    internal class SpriteAtlasTreeView : TreeView
    {
        #region Param

        internal enum SortOption
        {
            Name,
            Favorite,
        }

        internal SortOption[] sortOptions =
        {
            SortOption.Name,
            SortOption.Favorite,
        };

        private SpriteAssetViewerUtility.Styles styles
        {
            get { return SpriteAssetViewerUtility.SpriteAssetViewerStyles; }
        }

        private AtlasAssetGroup atlasAssetGroup;

        private List<AtlasAsset> searchResult;

        private Action<AtlasAsset> onClickContentsAction;

        private SpriteAtlasTreeViewHeader header
        {
            get { return multiColumnHeader as SpriteAtlasTreeViewHeader; }
        }

        #endregion

        #region LifeCycle

        internal SpriteAtlasTreeView(TreeViewState state, string extension,
            Action<AtlasAsset> onClickContentsAction = null, Action onClickFavoriteClearAction = null)
            : base(state, new SpriteAtlasTreeViewHeader(CreateDefaultMultiColumnHeaderState()) { height = 20f })
        {
            BuildSpriteAtlasList();
            showBorder = true;
            showAlternatingRowBackgrounds = true;
            header.sortingChanged += OnSortingChanged;
            header.OnFavoriteModeChanged += OnFavoriteModeChanged;
            header.OnFavoriteClearClicked += onClickFavoriteClearAction;
            header.OnExpandAllClicked += ExpandAll;
            header.OnCollapseAllClicked += CollapseAll;
            this.onClickContentsAction = onClickContentsAction;
        }

        public void BuildSpriteAtlasList()
        {
            BuildSpriteAtlasList(SpriteAssetViewerSetting.GetSetting().GlobalSetting.SpriteAtlasSavePath,
                SpriteAssetViewerUtility.SpriteAtlasExtension, !header.IsFavoriteMode);
        }

        private void BuildSpriteAtlasList(string savePath, string extension, bool isShowNotFavorite = true)
        {
            List<SpriteAtlas> spriteAtlasList = SpriteAssetViewerUtility.GetAssetList<SpriteAtlas>(savePath);
            atlasAssetGroup = AtlasAssetGroup.Create(savePath, spriteAtlasList.ToArray(), isShowNotFavorite);
            BuildSpriteAtlasList(atlasAssetGroup, extension, isShowNotFavorite);
        }

        private void BuildSpriteAtlasList(AtlasAssetGroup rootGroup, string extension, bool isShowNotFavorite = true)
        {
            string[] subPaths = SpriteAssetViewerUtility.GetSubFolder(rootGroup.Path);

            foreach (var path in subPaths)
            {
                List<SpriteAtlas> spriteAtlasList = SpriteAssetViewerUtility.GetAssetList<SpriteAtlas>(path);
                AtlasAssetGroup group = AtlasAssetGroup.Create(path, spriteAtlasList.ToArray(), isShowNotFavorite);
                rootGroup.AddSubAtlasAssetGroup(group);
                BuildSpriteAtlasList(group, extension, isShowNotFavorite);
            }
        }

        protected override TreeViewItem BuildRoot()
        {
            var mainRoot = new SpriteAtlasTreeViewItem();

            if (searchResult == null)
            {
                BuildViewItems(mainRoot, atlasAssetGroup);
            }
            else
            {
                BuildSearchItems(mainRoot, searchResult);
            }

            return mainRoot;
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            return base.BuildRows(root);
        }

        public override void OnGUI(Rect rect)
        {
            base.OnGUI(rect);
            if (UnityEngine.Event.current.type == EventType.MouseDown && UnityEngine.Event.current.button == 0 &&
                rect.Contains(UnityEngine.Event.current.mousePosition))
            {
                SetSelection(Array.Empty<int>(), TreeViewSelectionOptions.FireSelectionChanged);
            }
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), args.item as SpriteAtlasTreeViewItem, args.GetColumn(i), ref args);
            }
        }

        private void CellGUI(Rect cellRect, SpriteAtlasTreeViewItem item, int column, ref RowGUIArgs args)
        {
            if (column == 0)
            {
                float num = this.GetContentIndent(item) + this.extraSpaceBeforeIconAndLabel;
                cellRect.xMin += num;
                DefaultGUI.Label(cellRect, item.displayName, args.selected, args.focused);
            }
            else if (column == 1)
            {
                if (item.AtlasAsset != null)
                {
                    GUI.DrawTexture(cellRect,
                        item.AtlasAsset.Favorite ? styles.FavoriteTexture : styles.NotFavoriteTexture,
                        ScaleMode.StretchToFill);
                    if (GUI.Button(cellRect, GUIContent.none, GUIStyle.none))
                    {
                        item.AtlasAsset.Favorite = !item.AtlasAsset.Favorite;
                        if (item.AtlasAsset.Favorite)
                        {
                            SpriteAssetViewerSetting.GetSetting().LocalSetting
                                .AddFavoriteSpriteAtlas(item.AtlasAsset.Path);
                        }
                        else
                        {
                            SpriteAssetViewerSetting.GetSetting().LocalSetting
                                .RemoveFavoriteSpriteAtlas(item.AtlasAsset.Path);
                        }
                    }
                }
            }
        }

        private void BuildViewItems(TreeViewItem treeViewRoot, AtlasAssetGroup atlasAssetGroupRoot)
        {
            if (atlasAssetGroupRoot == null || treeViewRoot == null)
            {
                return;
            }

            int depth = treeViewRoot.depth + 1;

            // add Folder
            if (atlasAssetGroupRoot.SubAtlasAssetGroups != null && atlasAssetGroupRoot.SubAtlasAssetGroups.Count != 0)
            {
                for (int i = 0; i < atlasAssetGroupRoot.SubAtlasAssetGroups.Count; i++)
                {
                    SpriteAtlasTreeViewItem subItem = new SpriteAtlasTreeViewItem(
                        atlasAssetGroupRoot.SubAtlasAssetGroups[i].Path,
                        atlasAssetGroupRoot.SubAtlasAssetGroups[i].GroupName, depth);
                    treeViewRoot.AddChild(subItem);
                    BuildViewItems(subItem, atlasAssetGroupRoot.SubAtlasAssetGroups[i]);
                }
            }

            // add File
            if (atlasAssetGroupRoot.SpriteAtlasAssetsList != null &&
                atlasAssetGroupRoot.SpriteAtlasAssetsList.Count != 0)
            {
                for (int i = 0; i < atlasAssetGroupRoot.SpriteAtlasAssetsList.Count; i++)
                {
                    treeViewRoot.AddChild(new SpriteAtlasTreeViewItem(atlasAssetGroupRoot.SpriteAtlasAssetsList[i],
                        depth));
                }
            }
        }

        private void BuildSearchItems(TreeViewItem treeViewRoot, List<AtlasAsset> assetAssetList)
        {
            int depth = treeViewRoot.depth + 1;

            for (int i = 0; i < assetAssetList.Count; i++)
            {
                treeViewRoot.AddChild(new SpriteAtlasTreeViewItem(assetAssetList[i], depth));
            }
        }

        #endregion

        #region Public Method

        public void Search(string searchStr)
        {
            if (string.IsNullOrEmpty(searchStr))
            {
                ClearSearchResult();
                Reload();
                return;
            }

            if (searchResult == null)
            {
                searchResult = new List<AtlasAsset>();
            }

            searchResult.Clear();

            string searchLowerString = searchStr.ToLower();

            LoopAssetBundleGroups(atlasAssetGroup, null, (atlasAsset) =>
            {
                SpriteAtlasTreeViewFilterPopup.SearchType searchType =
                    SpriteAtlasTreeViewFilterPopup.GetSearchType(searchLowerString);

                switch (searchType)
                {
                    case SpriteAtlasTreeViewFilterPopup.SearchType.SpriteAtlas_Name:
                        if (atlasAsset.SpriteAtlasAsset.name.ToLower().Contains(searchLowerString))
                        {
                            searchResult.Add(atlasAsset);
                        }

                        break;
                    case SpriteAtlasTreeViewFilterPopup.SearchType.AtlasContents_Name:
                    case SpriteAtlasTreeViewFilterPopup.SearchType.AtlasContents_GUID:

                        string searchContentString = searchLowerString.Substring(2, searchLowerString.Length - 2);
                        if (string.IsNullOrEmpty(searchContentString))
                        {
                            break;
                        }

                        // find as name
                        UnityEngine.Object[] packedAssets = atlasAsset.SpriteAtlasAsset.GetPackables();
                        for (int j = 0; j < packedAssets.Length; j++)
                        {
                            if (searchType == SpriteAtlasTreeViewFilterPopup.SearchType.AtlasContents_Name)
                            {
                                if (packedAssets[j].name.ToLower().Contains(searchContentString))
                                {
                                    searchResult.Add(atlasAsset);
                                    break;
                                }
                            }
                            else
                            {
                                if (AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(packedAssets[j])) ==
                                    searchContentString)
                                {
                                    searchResult.Add(atlasAsset);
                                    break;
                                }
                            }
                        }

                        break;
                }
            });
            Reload();
        }

        public void ClearSearchResult()
        {
            if (searchResult == null)
            {
                return;
            }

            searchResult.Clear();
            searchResult = null;
        }

        #endregion

        #region Inner Method

        private static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
        {
            return new MultiColumnHeaderState(GetColumns());
        }

        private static MultiColumnHeaderState.Column[] GetColumns()
        {
            var retVal = new MultiColumnHeaderState.Column[]
            {
                new MultiColumnHeaderState.Column(),
                new MultiColumnHeaderState.Column(),
            };

            retVal[0].headerContent = new GUIContent("Name", "Folder or file name");
            retVal[0].minWidth = 100;
            retVal[0].width = 200;
            retVal[0].headerTextAlignment = TextAlignment.Left;
            retVal[0].canSort = true;
            retVal[0].autoResize = true;
            retVal[0].contextMenuText = string.Empty;

            retVal[1].headerContent = SpriteAssetViewerUtility.SpriteAssetViewerStyles.FavoriteIcon;
            retVal[1].minWidth = 20;
            retVal[1].width = 20;
            retVal[1].maxWidth = 20;
            retVal[1].headerTextAlignment = TextAlignment.Center;
            retVal[1].canSort = true;
            retVal[1].autoResize = true;
            retVal[1].contextMenuText = "Favorite";

            return retVal;
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (selectedIds != null && selectedIds.Count > 0)
            {
                var item = FindItem(selectedIds[0], rootItem) as SpriteAtlasTreeViewItem;
                if (item != null && item.AtlasAsset != null)
                {
                    onClickContentsAction(item.AtlasAsset);
                }
            }

            base.SelectionChanged(selectedIds);
        }

        protected void LoopAssetBundleGroups(AtlasAssetGroup rootGroup, Action<AtlasAssetGroup> doSomethingGroup,
            Action<AtlasAsset> doSomethingInfo)
        {
            if (rootGroup == null)
            {
                return;
            }

            if (doSomethingGroup != null)
            {
                doSomethingGroup.Invoke(rootGroup);
            }

            for (int i = 0; i < rootGroup.SpriteAtlasAssetsList.Count; i++)
            {
                if (doSomethingInfo != null)
                {
                    doSomethingInfo.Invoke(rootGroup.SpriteAtlasAssetsList[i]);
                }
            }

            if (rootGroup.SubAtlasAssetGroups != null && rootGroup.SubAtlasAssetGroups.Count != 0)
            {
                for (int i = 0; i < rootGroup.SubAtlasAssetGroups.Count; i++)
                {
                    LoopAssetBundleGroups(rootGroup.SubAtlasAssetGroups[i], doSomethingGroup, doSomethingInfo);
                }
            }
        }

        protected void OnFavoriteModeChanged(bool result)
        {
            BuildSpriteAtlasList();
            Reload();
            Repaint();
        }

        #region Sort

        protected void OnSortingChanged(MultiColumnHeader multiColumnHeader)
        {
            Sort();
        }

        protected void Sort()
        {
            var sortedColumns = multiColumnHeader.state.sortedColumns;

            if (sortedColumns.Length == 0)
            {
                return;
            }

            SortByColumn(sortedColumns, rootItem as SpriteAtlasTreeViewItem);

            BuildRows(rootItem);

            Repaint();
        }

        protected void SortByColumn(int[] sortedColumns, SpriteAtlasTreeViewItem root)
        {
            List<SpriteAtlasTreeViewItem> atlasAssetList = new List<SpriteAtlasTreeViewItem>();
            foreach (var item in root.children)
            {
                atlasAssetList.Add(item as SpriteAtlasTreeViewItem);
                if (item.hasChildren)
                {
                    SortByColumn(sortedColumns, item as SpriteAtlasTreeViewItem);
                }
            }

            var orderedItems = InitialOrder(atlasAssetList, sortedColumns);
            root.children = orderedItems.Cast<TreeViewItem>().ToList();
        }

        protected List<SpriteAtlasTreeViewItem> InitialOrder(List<SpriteAtlasTreeViewItem> myTypes, int[] columnList)
        {
            SortOption sortOption = sortOptions[columnList[0]];
            bool ascending = multiColumnHeader.IsSortedAscending(columnList[0]);
            switch (sortOption)
            {
                case SortOption.Name:
                {
                    myTypes.Sort((a, b) =>
                    {
                        if (ascending)
                        {
                            string aWeight = a.AtlasAsset == null
                                ? string.Format("B{0}", a.displayName)
                                : string.Format("A{0}", a.displayName);
                            string bWeight = b.AtlasAsset == null
                                ? string.Format("B{0}", b.displayName)
                                : string.Format("A{0}", b.displayName);
                            return bWeight.CompareTo(aWeight);
                        }
                        else
                        {
                            string aWeight = a.AtlasAsset == null
                                ? string.Format("A{0}", a.displayName)
                                : string.Format("B{0}", a.displayName);
                            string bWeight = b.AtlasAsset == null
                                ? string.Format("A{0}", b.displayName)
                                : string.Format("B{0}", b.displayName);
                            return aWeight.CompareTo(bWeight);
                        }
                    });
                    break;
                }
                case SortOption.Favorite:
                {
                    myTypes.Sort((a, b) =>
                    {
                        if ((a.AtlasAsset == null && b.AtlasAsset == null) ||
                            (a.AtlasAsset != null && a.AtlasAsset != null &&
                             b.AtlasAsset != null && b.AtlasAsset != null &&
                             a.AtlasAsset.Favorite == b.AtlasAsset.Favorite)
                           )
                        {
                            return a.displayName.CompareTo(b.displayName);
                        }

                        if (a.AtlasAsset == null)
                        {
                            return -1;
                        }

                        if (b.AtlasAsset == null)
                        {
                            return 1;
                        }

                        if (ascending)
                        {
                            return a.AtlasAsset.Favorite ? 1 : -1;
                        }
                        else
                        {
                            return a.AtlasAsset.Favorite ? -1 : 1;
                        }
                    });
                    break;
                }
            }

            return myTypes;
        }

        #endregion

        #endregion
    }
}
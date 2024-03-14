using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.U2D;

namespace ZJYFrameWork.SpriteViewer
{
    internal partial class SpriteAssetViewerWindow
    {
        internal class SpriteAssetViewerSpriteAtlasMode : SpriteAssetViewerModeBase
        {
            #region Param

            public SpriteAssetViewerSpriteAtlasMode(SpriteAssetViewerWindow owner) : base(owner) { }

            private AtlasAssetGroup cachedSpriteAtlasGroup { get { return owner.cachedSpriteAtlasGroup; } set { owner.cachedSpriteAtlasGroup = value; } }

            private Vector2 leftScorllViewPosition;

            private Vector2 rightScorllViewPosition;


            private AtlasAsset currentSelectAtlas;
            internal AtlasAsset CurrentSelectAtlas
            {
                get
                {
                    if (currentSelectAtlas != null && currentSelectAtlas.SpriteAtlasAsset == null)
                    {
                        currentSelectAtlas = null;
                        currentSelectAtlasEditor = null;
                        OnClickRefresh();
                        owner.Repaint();
                    }
                    return currentSelectAtlas;
                }
                set
                {
                    currentSelectAtlas = value;
                    if (currentSelectAtlas == null)
                    {
                        currentSelectAtlasEditor = null;
                    }
                    else
                    {
                        currentSelectAtlasEditor = Editor.CreateEditor(currentSelectAtlas.SpriteAtlasAsset);
                    }
                }
            }

            private Editor currentSelectAtlasEditor;
            public Editor CurrentSelectAtlasEditor
            {
                get
                {
                    if (CurrentSelectAtlas == null)
                    {
                    }
                    return currentSelectAtlasEditor;
                }

            }

            private bool isInited;

            private bool isShowPackingPreview;

            public string CacheSearchString = string.Empty;

            private List<AtlasAsset> cacheShowSpriteAtlas;

            private SearchField searchField;

            private SpriteAtlasTreeView spriteAtlasTreeView;
            private SpriteAtlasTreeView SpriteAtlasTreeView
            {
                get
                {
                    if (spriteAtlasTreeView == null)
                    {
                        spriteAtlasTreeView = new SpriteAtlasTreeView(
                            treeViewState ?? (treeViewState = new TreeViewState()),
                            SpriteAssetViewerUtility.SpriteAtlasExtension,
                            OnClickSpriteAtlas,
                            OnClickClearAllFavorite);
                        spriteAtlasTreeView.Reload();
                    }
                    return spriteAtlasTreeView;
                }
            }

            [SerializeField]
            private float leftWidth = 242f;

            [SerializeField]
            private TreeViewState treeViewState;

            private bool resizingHorizon = false;

            #endregion

            #region LifeCycle

            public override void DrawMode(Rect rect)
            {
                if (isInited)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        Rect leftRect = new Rect(rect);
                        leftRect.width = leftWidth;
                        DrawLeft(leftRect);

                        float resizeRectWidth = 8f;
                        Rect resizeRect = new Rect(rect.x + leftWidth, rect.y, resizeRectWidth, rect.height);
                        owner.HandleHorizonResize(resizeRect, leftRect.position, ref resizingHorizon, ref leftWidth, new UnityEngine.Vector2(10f, rect.width));

                        Rect rightRect = new Rect(rect);
                        rightRect.x = leftRect.x + leftRect.width + resizeRect.width;
                        rightRect.width = rect.width - leftRect.width - resizeRect.width;
                        DrawRight(rightRect);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginArea(rect);
                    {
                        EditorGUILayout.HelpBox("The default spriteAtlas folder is not exit ! \n Click 「Refresh」button and create it first !", MessageType.Info);
                    }
                    GUILayout.EndArea();
                }
            }

            private void DrawLeft(Rect rect)
            {
                GUILayout.BeginArea(rect);
                {
                    leftScorllViewPosition = EditorGUILayout.BeginScrollView(leftScorllViewPosition, styles.BoxStyle, GUILayout.Width(leftWidth));
                    {
                        float topSpace = styles.BoxStyle.margin.left;
                        float lrMargin = styles.BoxStyle.margin.left;
                        Rect searchFieldRect = new Rect(lrMargin, lrMargin + topSpace, leftWidth - 2 * lrMargin, EditorGUIUtility.singleLineHeight);
                        DrawSearchBar(searchFieldRect);

                        float spaceY = 10f;
                        float spaceBottom = styles.BoxStyle.margin.bottom;
                        float fileListHeight = rect.height - topSpace - EditorGUIUtility.singleLineHeight - spaceY - spaceBottom;
                        Rect fileListRect = new Rect(searchFieldRect.x, searchFieldRect.y + searchFieldRect.height + spaceY, searchFieldRect.width, fileListHeight);

                        SpriteAtlasTreeView.OnGUI(fileListRect);
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndScrollView();
                }
                GUILayout.EndArea();
            }

            private void DrawSearchBar(Rect rect)
            {
                if (searchField == null)
                {
                    searchField = new SearchField();
                }

                GUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginChangeCheck();
                    {
                        float padding = 5f;
                        Rect popupButtonRect = GUILayoutUtility.GetRect(styles.FilterByTypeIcon, EditorStyles.popup, GUILayout.ExpandWidth(false));
                        Rect searchBarRect = new Rect(rect.x, rect.y, rect.width - popupButtonRect.width - padding, rect.height);
                        popupButtonRect.x = searchBarRect.x + searchBarRect.width + padding;
                        popupButtonRect.y = searchBarRect.y;
                        popupButtonRect.height = searchBarRect.height;
                        CacheSearchString = searchField.OnGUI(searchBarRect, CacheSearchString);
                        if (EditorGUI.DropdownButton(popupButtonRect, styles.FilterByTypeIcon, FocusType.Passive, EditorStyles.popup))
                        {
                            PopupWindow.Show(popupButtonRect, new SpriteAtlasTreeViewFilterPopup(owner));
                        }
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        SpriteAtlasTreeView.Search(CacheSearchString);
                    }
                }
                GUILayout.EndHorizontal();
            }

            private void DrawRight(Rect rect)
            {
                if (CurrentSelectAtlas == null || CurrentSelectAtlas.SpriteAtlasAsset == null)
                {
                    GUILayout.BeginArea(rect);
                    {
                        rightScorllViewPosition = EditorGUILayout.BeginScrollView(rightScorllViewPosition, "box", GUILayout.ExpandWidth(true));
                        {
                            GUILayout.FlexibleSpace();
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    GUILayout.EndArea();
                }
                else
                {
                    GUILayout.BeginArea(rect);
                    {
                        rightScorllViewPosition = EditorGUILayout.BeginScrollView(rightScorllViewPosition, "box", GUILayout.ExpandWidth(true));
                        {
                            if (!isShowPackingPreview)
                            {
                                if (CurrentSelectAtlas != null)
                                {
                                    DrawAtlas();
                                }
                            }
                            else
                            {
                                if (CurrentSelectAtlasEditor != null && CurrentSelectAtlasEditor.HasPreviewGUI())
                                {
                                    var previewRect = new Rect(0f, 0f, rect.width, rect.height);
                                    CurrentSelectAtlasEditor.DrawPreview(previewRect);
                                }
                                else
                                {
                                    isShowPackingPreview = false;
                                }
                            }

                            GUILayout.FlexibleSpace();
                            DrawFooterTools();
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    GUILayout.EndArea();
                }
            }

            private void DrawAtlas()
            {
                if (!CurrentSelectAtlasEditor)
                {
                    return;
                }

                CurrentSelectAtlasEditor.OnInspectorGUI();
            }

            private void DrawFooterTools()
            {
                EditorGUILayout.BeginVertical();
                {
                    if (EditorSettings.spritePackerMode != SpritePackerMode.AlwaysOnAtlas && EditorSettings.spritePackerMode != SpritePackerMode.BuildTimeOnlyAtlas)
                    {
                        EditorGUILayout.HelpBox("Sprite Atlas packing is disabled. Enable it in Edit > Project Setting > Editor. Or change from the option below.", MessageType.Info);
                        EditorSettings.spritePackerMode = (SpritePackerMode)EditorGUILayout.EnumPopup("Sprite Packer Mode : ", EditorSettings.spritePackerMode);
                    }
                    else
                    {
                        if (CurrentSelectAtlasEditor && CurrentSelectAtlasEditor.HasPreviewGUI())
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                GUILayout.FlexibleSpace();
                                if (GUILayout.Button(isShowPackingPreview ? styles.PropertyIcon : styles.TextureIcon, GUILayout.Width(40f), GUILayout.Height(40f)))
                                {
                                    isShowPackingPreview = !isShowPackingPreview;
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }

            public override void DrawExToolbar()
            {
                if (GUILayout.Button(styles.RefreshIcon, styles.LabelStyle, GUILayout.Width(20f), GUILayout.Height(20f)))
                {
                    OnClickRefresh();
                }
                base.DrawExToolbar();
            }

            public override void DrawSetting(GenericMenu menu)
            {
                if (owner.Setting.GlobalSetting.atlasIsAddAtlasAbString)
                {
                    menu.AddItem(new GUIContent("Set All SpriteAtlas as Assetbundle"), false, () => OnClickSetAllSpriteAtlasAbString());
                    if (owner.Setting.GlobalSetting.atlasIsAddAtlasContentsAbString)
                    {
                        menu.AddItem(new GUIContent("Set All SpriteAtlas Contents in Assetbundle"), false, () => OnClickSetAllAssetContentsAbString());
                    }
                }

                base.DrawSetting(menu);
            }

            public override EditorMode GetMode()
            {
                return EditorMode.SpriteAtlas;
            }

            public override void OnModeIn()
            {
                isInited = CheckFolderExist();
                if (isInited)
                {
                    OnClickRefresh();
                }
                if (searchField == null)
                {
                    searchField = new SearchField();
                }
            }

            public override void OnModeOut()
            {
            }

            public override void Reload()
            {
                SpriteAtlasTreeView.BuildSpriteAtlasList();
                SpriteAtlasTreeView.Reload();
                SpriteAtlasTreeView.Repaint();
            }

            #endregion

            #region 外部Api

            public void Search(string searchString)
            {
                CacheSearchString = searchString;
                SpriteAtlasTreeView.Search(CacheSearchString);
                owner.Repaint();
            }

            public void SearchSpriteAtlas(string targetGUID)
            {
                CacheSearchString = string.Format("g:{0}", targetGUID);
                SpriteAtlasTreeView.Search(CacheSearchString);
                owner.Repaint();
            }

            #endregion

            #region 内部メソッド

            #region ClickEvent

            private void OnClickRefresh()
            {
                CheckAndCreateFolderExist();
                if (!isInited)
                {
                    return;
                }

                List<SpriteAtlas> spriteAtlasList = SpriteAssetViewerUtility.GetAssetList<SpriteAtlas>(owner.Setting.GlobalSetting.SpriteAtlasSavePath);
                AtlasAssetGroup atlasAssetGroup = AtlasAssetGroup.Create(owner.Setting.GlobalSetting.SpriteAtlasSavePath, spriteAtlasList.ToArray());
                GetSubAtlasGroup(ref atlasAssetGroup);
                cachedSpriteAtlasGroup = atlasAssetGroup;
                Reload();
            }

            private void OnClickSpriteAtlas(AtlasAsset atlasAsset)
            {
                CurrentSelectAtlas = atlasAsset;
            }

            private void OnClickClearAllFavorite()
            {
                if (EditorUtility.DisplayDialog("Warring!", "Clear All Favorite.\nContinue?", "Yes", "No"))
                {
                    owner.Setting.LocalSetting.ClearFavoriteSpriteAtlas();
                }

                SetFavoriteOff(cachedSpriteAtlasGroup);
                OnClickRefresh();
            }

            private void OnClickSetAllSpriteAtlasAbString()
            {
                if (!owner.Setting.GlobalSetting.atlasIsAddAtlasAbString)
                {
                    EditorUtility.DisplayDialog("Warring!", "Does not match the settings! Change the setting first.", "Ok");
                    return;
                }
                if (!EditorUtility.DisplayDialog("Warring!", "Are you sure you want to set SpriteAtlas as AssetBundle?", "Yes", "No"))
                {
                    return;
                }

                SetAllSpriteAtlasAssetBundleString();
            }

            private void OnClickSetAllAssetContentsAbString()
            {
                if (!owner.Setting.GlobalSetting.atlasIsAddAtlasContentsAbString || !owner.Setting.GlobalSetting.atlasIsAddAtlasAbString)
                {
                    EditorUtility.DisplayDialog("Warring!", "Does not match the settings! Change the setting first.", "Ok");
                    return;
                }

                if (!EditorUtility.DisplayDialog("Warring!", "Are you sure you want to add all contents to the spriteAtlas AssetBundle?", "Yes", "No"))
                {
                    return;
                }

                SetAllAssetContentsAssetBundleString();
            }

            #endregion

            private void GetSubAtlasGroup(ref AtlasAssetGroup rootAtlasAssetGroup)
            {
                string[] subPaths = SpriteAssetViewerUtility.GetSubFolder(rootAtlasAssetGroup.Path);
                foreach (var path in subPaths)
                {
                    List<SpriteAtlas> spriteAtlasList = SpriteAssetViewerUtility.GetAssetList<SpriteAtlas>(path);
                    AtlasAssetGroup atlasAssetGroup = AtlasAssetGroup.Create(path, spriteAtlasList.ToArray());
                    rootAtlasAssetGroup.AddSubAtlasAssetGroup(atlasAssetGroup);
                    GetSubAtlasGroup(ref atlasAssetGroup);
                }
            }

            private void SetFavoriteOff(AtlasAssetGroup atlasAssetGroup)
            {
                LoopAtlasAssetGroupAndDoSomething(atlasAssetGroup, (atlasAsset) => atlasAsset.Favorite = false);
            }

            private void SetAllSpriteAtlasAssetBundleString()
            {
                OnClickRefresh();

                var assetBundleGroup = cachedSpriteAtlasGroup.SubAtlasAssetGroups.Find(subGroup => subGroup.Path == owner.Setting.GetFolderSavePath(SpriteAssetViewerSetting.FolderType.AssetBundle));

                LoopAtlasAssetGroupAndDoSomething(assetBundleGroup, (atlasAsset) =>
                {
                    SpriteAssetViewerUtility.SetAssetBundleWithExtension(atlasAsset.SpriteAtlasAsset);
                });
            }

            private void SetAllAssetContentsAssetBundleString()
            {
                SetAllSpriteAtlasAssetBundleString();

                OnClickRefresh();

                var assetBundleGroup = cachedSpriteAtlasGroup.SubAtlasAssetGroups.Find(subGroup => subGroup.Path == owner.Setting.GetFolderSavePath(SpriteAssetViewerSetting.FolderType.AssetBundle));

                LoopAtlasAssetGroupAndDoSomething(assetBundleGroup, (atlasAsset) =>
                {
                    SpriteAssetViewerUtility.SetAllContentsAssetBundle(atlasAsset.SpriteAtlasAsset);
                });
            }

            private void LoopAtlasAssetGroupAndDoSomething(AtlasAssetGroup atlasAssetGroup, Action<AtlasAsset> doSomething)
            {
                for (int i = 0; i < atlasAssetGroup.SpriteAtlasAssetsList.Count; i++)
                {
                    doSomething.Invoke(atlasAssetGroup.SpriteAtlasAssetsList[i]);
                }

                if (atlasAssetGroup.SubAtlasAssetGroups != null && atlasAssetGroup.SubAtlasAssetGroups.Count > 0)
                {
                    for (int j = 0; j < atlasAssetGroup.SubAtlasAssetGroups.Count; j++)
                    {
                        LoopAtlasAssetGroupAndDoSomething(atlasAssetGroup.SubAtlasAssetGroups[j], doSomething);
                    }
                }
            }

            private bool CheckFolderExist()
            {
                return SpriteAssetViewerUtility.IsExistFolder(owner.Setting.GlobalSetting.SpriteAtlasSavePath) &&
                    SpriteAssetViewerUtility.IsExistFolder(owner.Setting.GetFolderSavePath(SpriteAssetViewerSetting.FolderType.Local)) &&
                    SpriteAssetViewerUtility.IsExistFolder(owner.Setting.GetFolderSavePath(SpriteAssetViewerSetting.FolderType.AssetBundle));
            }

            private void CheckAndCreateFolderExist()
            {
                bool isCreateFolder = false;

                if (!CheckFolderExist())
                {
                    if (EditorUtility.DisplayDialog("Warring!", "The default spriteAtlas folder is not exit! Create it ?", "Yes", "No"))
                    {
                        isCreateFolder = true;
                    }
                }
                else
                {
                    isInited = true;
                }

                if (isCreateFolder)
                {
                    SpriteAssetViewerUtility.CreateBaseFolder(owner.Setting.GlobalSetting.SpriteAtlasSavePath);
                    SpriteAssetViewerUtility.CreateBaseFolder(owner.Setting.GetFolderSavePath(SpriteAssetViewerSetting.FolderType.AssetBundle));
                    SpriteAssetViewerUtility.CreateBaseFolder(owner.Setting.GetFolderSavePath(SpriteAssetViewerSetting.FolderType.Local));
                    isInited = true;
                }
            }

            #endregion
        }
    }
}
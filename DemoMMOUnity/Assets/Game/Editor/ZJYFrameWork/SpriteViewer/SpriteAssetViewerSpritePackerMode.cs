using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ZJYFrameWork.SpriteViewer
{
    internal partial class SpriteAssetViewerWindow
    {
        internal class SpriteAssetViewerSpritePackerMode : SpriteAssetViewerModeBase
        {
            #region Param

            public SpriteAssetViewerSpritePackerMode(SpriteAssetViewerWindow owner) : base(owner)
            {
            }

            private bool isCacheOver
            {
                get
                {
                    return owner.cachedSpriteGuids != null && owner.cachedSpriteGuids.Length != 0 &&
                           owner.cachedPackingAssets != null && owner.cachedPackingAssets.Count != 0;
                }
            }

            private string[] cachedTextureGuids
            {
                get { return owner.cachedSpriteGuids; }
                set { owner.cachedSpriteGuids = value; }
            }

            private List<PackingAsset> cachedPackingAssets
            {
                get { return owner.cachedPackingAssets; }
                set { owner.cachedPackingAssets = value; }
            }

            private PackingAsset[] cacheShowPackingAssets;

            private PackingAsset[] showPackingAssets
            {
                get
                {
                    if (cacheShowPackingAssets == null)
                    {
                        RefreshShowPackingAssets();
                    }

                    return cacheShowPackingAssets;
                }
                set { cacheShowPackingAssets = value; }
            }

            private PackingAsset currentSelectPackingAsset;

            private bool isEditTagName = false;

            private Vector2 leftScorllViewPosition;

            private Vector2 rightScorllViewPosition;

            private string cacheSearchString = string.Empty;

            private HashSet<string> cachedDelayImportAssetPaths;

            private SearchField searchField;

            #endregion

            #region LifeCycle

            public override void DrawMode(Rect rect)
            {
                GUILayout.BeginArea(rect);
                {
                    DrawTop();

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    {
                        DrawLeft();
                        DrawRight();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }

            private void DrawTop()
            {
                EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
                {
                    if (isCacheOver)
                    {
                        if (GUILayout.Button("Clear Cache", GUILayout.ExpandWidth(true)))
                        {
                            OnClickClearCache();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Search All Packing Tag", GUILayout.ExpandWidth(true)))
                        {
                            OnClickSetupCache();
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }

            private void DrawLeft()
            {
                if (!isCacheOver)
                {
                    return;
                }

                EditorGUILayout.BeginVertical("box", GUILayout.MinWidth(240), GUILayout.ExpandHeight(true));
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUI.BeginChangeCheck();
                        {
                            Rect searchFiledRect = EditorGUILayout.GetControlRect();
                            cacheSearchString = searchField.OnGUI(searchFiledRect, string.Empty);
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            RefreshShowPackingAssets();
                        }

                        EditorGUILayout.LabelField(styles.SearchIcon, GUILayout.Width(16f));
                    }
                    EditorGUILayout.EndHorizontal();

                    leftScorllViewPosition = EditorGUILayout.BeginScrollView(leftScorllViewPosition);
                    {
                        foreach (var packingAsset in showPackingAssets)
                        {
                            if (currentSelectPackingAsset != null && currentSelectPackingAsset.PackingTagName ==
                                packingAsset.PackingTagName)
                            {
                                Color guicolorBefore = GUI.color;
                                GUI.color = Color.green;

                                if (GUILayout.Button(packingAsset.PackingTagName))
                                {
                                    OnClickPackingTagButton(packingAsset.PackingTagName);
                                }

                                GUI.color = guicolorBefore;
                            }
                            else
                            {
                                if (GUILayout.Button(packingAsset.PackingTagName))
                                {
                                    OnClickPackingTagButton(packingAsset.PackingTagName);
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }

            private void DrawRight()
            {
                if (!isCacheOver)
                {
                    return;
                }

                DrawTextureList();

                GUILayout.FlexibleSpace();

                DrawFooterTools();
            }

            private void DrawTextureList()
            {
                if (currentSelectPackingAsset != null && currentSelectPackingAsset.PackingSprites != null)
                {
                    EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Texture List : ");
                            GUILayout.FlexibleSpace();

                            bool guiEnableBefore = GUI.enabled;
                            GUI.enabled = isEditTagName;
                            EditorGUILayout.TextField(currentSelectPackingAsset.PackingTagName);
                            GUI.enabled = guiEnableBefore;

                            isEditTagName = GUILayout.Toggle(isEditTagName, styles.EditIcon, styles.SingleButton);
                        }
                        EditorGUILayout.EndHorizontal();

                        GUILayout.Space(5f);

                        rightScorllViewPosition = EditorGUILayout.BeginScrollView(rightScorllViewPosition);
                        {
                            for (int i = 0; i < currentSelectPackingAsset.PackingSprites.Count; i++)
                            {
                                EditorGUILayout.ObjectField(currentSelectPackingAsset.PackingSprites[i].texture,
                                    typeof(Texture), false);
                            }
                        }
                        EditorGUILayout.EndScrollView();
                    }
                }
            }

            private void DrawFooterTools()
            {
                if (currentSelectPackingAsset == null || currentSelectPackingAsset.PackingSprites.Count == 0)
                {
                    return;
                }

#pragma warning disable CS0618
                if (EditorSettings.spritePackerMode != SpritePackerMode.AlwaysOn &&
                    EditorSettings.spritePackerMode != SpritePackerMode.BuildTimeOnly)
#pragma warning restore CS0618
                {
                    EditorGUILayout.HelpBox(
                        "Sprite Atlas packing is disabled. Enable it in Edit > Project Setting > Editor. Or change from the option below.",
                        MessageType.Info);
                    EditorSettings.spritePackerMode =
                        (SpritePackerMode)EditorGUILayout.EnumPopup("Sprite Packer Mode : ",
                            EditorSettings.spritePackerMode);
                }

                if (GUILayout.Button("Packing Preview"))
                {
                    OnClickTestPackingButton();
                }

                if (GUILayout.Button("Change To SpriteAtlas"))
                {
                    OnClickChange2AtlasButton();
                }

                EditorGUILayout.EndVertical();
            }

            public override void DrawSetting(GenericMenu menu)
            {
                base.DrawSetting(menu);
                if (isCacheOver && cachedPackingAssets.Count != 0)
                {
                    menu.AddItem(new GUIContent("Change All To SpriteAtlas"), false,
                        () => OnClickChange2AtlasAllButton());
                }
            }

            public override void OnModeIn()
            {
                if (searchField == null)
                {
                    searchField = new SearchField();
                }

                return;
            }

            public override void OnModeOut()
            {
                searchField = null;
                return;
            }

            #endregion

            #region 外部Api

            public override EditorMode GetMode()
            {
                return EditorMode.SpritePacker;
            }

            #endregion

            #region 内部メソッド

            #region ClickEvent

            private void OnClickSetupCache()
            {
                if (isCacheOver)
                {
                    return;
                }

                cachedPackingAssets = new List<PackingAsset>(128);
                cachedTextureGuids = AssetDatabase.FindAssets("t:texture");

                int textureCount = cachedTextureGuids.Length;

                if (textureCount == 0)
                {
                    EditorUtility.DisplayDialog("Info", "Can't find any textures.", "ok");
                }

                for (int i = 0; i < cachedTextureGuids.Length; i++)
                {
                    float rate = (float)i / cachedTextureGuids.Length;
                    EditorUtility.DisplayProgressBar("Loading...",
                        string.Format("Total progress {0}% .", (rate * 100).ToString("F2")), rate);

                    var targetPath = AssetDatabase.GUIDToAssetPath(cachedTextureGuids[i]);

                    if (string.IsNullOrEmpty(targetPath))
                    {
                        continue;
                    }

                    var targetSprite = AssetDatabase.LoadAssetAtPath<Sprite>(targetPath);

                    if (!targetSprite)
                    {
                        continue;
                    }

                    var targetimporter = AssetImporter.GetAtPath(targetPath) as TextureImporter;
                    if (!targetimporter)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(targetimporter.spritePackingTag))
                    {
                        continue;
                    }

                    var key = targetimporter.spritePackingTag;

                    PackingAsset targetPackingAsset =
                        cachedPackingAssets.Find((packingAsset) => packingAsset.PackingTagName == key);
                    if (targetPackingAsset == null)
                    {
                        cachedPackingAssets.Add(new PackingAsset(key, targetSprite));
                    }
                    else
                    {
                        targetPackingAsset.PackingSprites.Add(targetSprite);
                    }
                }

                EditorUtility.ClearProgressBar();

                RefreshShowPackingAssets();
            }

            private void OnClickClearCache()
            {
                if (EditorUtility.DisplayDialog("Warring ! ",
                        "Do you really want to clear it ? Recache will take a long time.", "Yes", "No"))
                {
                    cachedTextureGuids = null;

                    if (cachedPackingAssets != null)
                    {
                        cachedPackingAssets.Clear();
                    }

                    cachedPackingAssets = null;

                    if (showPackingAssets != null)
                    {
                        showPackingAssets = new PackingAsset[0];
                    }
                }

                currentSelectPackingAsset = null;
                owner.Repaint();
            }

            private void OnClickPackingTagButton(string packingTagName)
            {
                if (!isCacheOver)
                {
                    return;
                }

                currentSelectPackingAsset =
                    cachedPackingAssets.Find((packingAsset) => packingAsset.PackingTagName == packingTagName);
            }

            private void OnClickChange2AtlasButton()
            {
                if (!EditorUtility.DisplayDialog(
                        "Warring!",
                        string.Format("{0}\n{1}\n{2}",
                            "Create SpriteAtlas in the following path.",
                            owner.Setting.GlobalSetting.SpriteAtlasSavePath,
                            "Continue ? "
                        ), "Yes", "No"))
                {
                    return;
                }

                ResetDelayImportPaths();
                ChangePackingTag2Atlas(currentSelectPackingAsset);
                ApplyDelayImportPaths();
            }

            private void OnClickTestPackingButton()
            {
                SpritePackingPreviewWindow.ShowWindow(owner, currentSelectPackingAsset.PackingSprites.ToArray());
            }

            private void OnClickChange2AtlasAllButton()
            {
                if (cachedPackingAssets == null || cachedPackingAssets.Count == 0)
                {
                    return;
                }

                if (!EditorUtility.DisplayDialog(
                        "Warring!",
                        string.Format("{0}\n{1}\n{2}\n{3}",
                            "Create SpriteAtlas in the following path.",
                            owner.Setting.GlobalSetting.SpriteAtlasSavePath,
                            owner.Setting.GlobalSetting.Packer2Atlas_IsCheckoutTag
                                ? "And delete all sprite PackingTag.\n"
                                : string.Empty,
                            "Continue ? "
                        ), "Yes", "No"))
                {
                    return;
                }

                ResetDelayImportPaths();

                for (int i = 0; i < cachedPackingAssets.Count; i++)
                {
                    ChangePackingTag2Atlas(cachedPackingAssets[i]);
                    SpriteAssetViewerUtility.ShowProgressbar(i + 1, cachedPackingAssets.Count);
                }

                ApplyDelayImportPaths();
            }

            #endregion

            private void RefreshShowPackingAssets()
            {
                if (string.IsNullOrEmpty(cacheSearchString))
                {
                    showPackingAssets = cachedPackingAssets.ToArray();
                }
                else
                {
                    string searchString = cacheSearchString.ToLower();
                    showPackingAssets = cachedPackingAssets
                        .FindAll(packingAsset => packingAsset.PackingTagName.ToLower().Contains(searchString))
                        .ToArray();
                }
            }

            private void ChangePackingTag2Atlas(PackingAsset targetAsset)
            {
                if (targetAsset == null)
                {
                    return;
                }

                string createPath = string.Empty;
                string existFilePath = string.Empty;

                if (owner.Setting.GlobalSetting.packer2AtlasIsAddAtlasAbString)
                {
                    createPath = owner.Setting.GetFolderSavePath(SpriteAssetViewerSetting.FolderType.AssetBundle);
                    existFilePath = SpriteAssetViewerUtility.FindSpriteAtlas(targetAsset.PackingTagName,
                        SpriteAssetViewerSetting.FolderType.AssetBundle);
                }
                else
                {
                    createPath = owner.Setting.GetFolderSavePath(SpriteAssetViewerSetting.FolderType.Local);
                    existFilePath = SpriteAssetViewerUtility.FindSpriteAtlas(targetAsset.PackingTagName,
                        SpriteAssetViewerSetting.FolderType.Local);
                }

                if (!string.IsNullOrEmpty(existFilePath))
                {
                    if (EditorUtility.DisplayDialog("Warring",
                            string.Format(
                                "The spriteAtlas is exist ! \n (path : {0})\nAdd these textures into this exist atlas ? ",
                                existFilePath), "Ok", "No"))
                    {
                        var targetAtlas = SpriteAssetViewerUtility.CreateSpriteAtlasAndSetAb(targetAsset.PackingTagName,
                            SpriteAssetViewerUtility.GetFolderPath(existFilePath),
                            System.Array.ConvertAll(targetAsset.PackingSprites.ToArray(), sprite => sprite.texture));
                        EditorGUIUtility.PingObject(targetAtlas);
                    }

                    return;
                }

                string[] delayImportPath = SpriteAssetViewerUtility.Change2SpriteAtlasAndSetAb(
                    targetAsset.PackingTagName, createPath,
                    System.Array.ConvertAll(targetAsset.PackingSprites.ToArray(), sprite => sprite.texture), false);

                CacheDelayImportPath(delayImportPath);
            }

            private void ResetDelayImportPaths()
            {
                cachedDelayImportAssetPaths = new HashSet<string>();
            }

            private void CacheDelayImportPath(string targetPath)
            {
                if (string.IsNullOrEmpty(targetPath))
                {
                    return;
                }

                if (cachedDelayImportAssetPaths == null)
                {
                    ResetDelayImportPaths();
                }

                cachedDelayImportAssetPaths.Add(targetPath);
            }

            private void CacheDelayImportPath(string[] targetPaths)
            {
                if (targetPaths == null || targetPaths.Length == 0)
                {
                    return;
                }

                cachedDelayImportAssetPaths.UnionWith(targetPaths);
            }

            private void ApplyDelayImportPaths()
            {
                if (cachedDelayImportAssetPaths == null)
                {
                    return;
                }

                string[] delayImportPathArray = new string[cachedDelayImportAssetPaths.Count];

                cachedDelayImportAssetPaths.CopyTo(delayImportPathArray);

                SpriteAssetViewerUtility.ReimportAssets(delayImportPathArray);

                ResetDelayImportPaths();
            }

            #endregion
        }
    }
}
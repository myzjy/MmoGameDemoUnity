using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEngine;

namespace ZJYFrameWork.SpriteViewer
{
    internal class SpritePackingPreviewWindow : SpriteAssetViewerUtilityWindow
    {
        #region Param;

        internal static SpritePackingPreviewWindow Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GetWindow<SpritePackingPreviewWindow>();
                }

                return instance;
            }
        }

        private static SpritePackingPreviewWindow instance;

        public SpriteAssetViewerUtility.Styles styles
        {
            get { return SpriteAssetViewerUtility.SpriteAssetViewerStyles; }
        }

        private HashSet<Sprite> spriteHashSet;

        public HashSet<Sprite> SpriteHashSet
        {
            get
            {
                if (spriteHashSet == null)
                {
                    if (cacheSprite == null)
                    {
                        spriteHashSet = new HashSet<Sprite>();
                    }
                    else
                    {
                        spriteHashSet = new HashSet<Sprite>(cacheSprite);
                    }
                }

                return spriteHashSet;
            }
            set { spriteHashSet = value; }
        }

        private Sprite[] cacheSprite;

        public int[] TextureImporterInstanceIDs { get; set; }

        private static string[] s_AtlasNamesEmpty = { "Sprite atlas cache is empty" };
        private string[] m_AtlasNames = s_AtlasNamesEmpty;
        private int m_SelectedAtlas = 0;

        private static string[] s_PageNamesEmpty = new string[0];
        private string[] m_PageNames = s_PageNamesEmpty;
        private int m_SelectedPage;

        private Sprite m_SelectedSprite = null;

        private MethodInfo getTextureFormatStringMethodInfo;

        protected MethodInfo GetTextureFormatStringMethodInfo
        {
            get
            {
                if (getTextureFormatStringMethodInfo == null)
                {
                    Type TextureUtilType = UnityEditorAssembly.GetType("UnityEditor.TextureUtil");
                    getTextureFormatStringMethodInfo = TextureUtilType.GetMethod("GetTextureFormatString",
                        BindingFlags.Public | BindingFlags.Static);
                }

                return getTextureFormatStringMethodInfo;
            }
        }

        private string DefaultPolice
        {
            get { return typeof(SpriteAssetViewerDefaultPackerPolicy).Name; }
        }

        private Vector2 leftScorllViewPosition;

        private Vector2 rightScorllViewPosition;

        private class PackerWindowStyle
        {
            public static readonly GUIContent packLabel = EditorGUIUtility.TrTextContent("Pack", null, (Texture)null);

            public static readonly GUIContent repackLabel =
                EditorGUIUtility.TrTextContent("Repack", null, (Texture)null);

            public static readonly GUIContent viewAtlasLabel =
                EditorGUIUtility.TrTextContent("View Atlas:", null, (Texture)null);

            public static readonly GUIContent windowTitle =
                EditorGUIUtility.TrTextContent("Sprite Packer", null, (Texture)null);

            public static readonly GUIContent pageContentLabel =
                EditorGUIUtility.TrTextContent("Page {0}", null, (Texture)null);

            public static readonly GUIContent packingDisabledLabel =
                EditorGUIUtility.TrTextContent(
                    "Legacy sprite packing is disabled. Enable it in Edit > Project Settings > Editor.", null,
                    (Texture)null);

            public static readonly GUIContent openProjectSettingButton =
                EditorGUIUtility.TrTextContent("Open Project Editor Settings", null, (Texture)null);
        }

        #endregion

        #region LifeCycle

        void OnEnable()
        {
            Packer.SelectedPolicy = DefaultPolice;
            Reset();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                DrawLeft();

                DrawRight();
            }
            EditorGUILayout.EndHorizontal();
        }

        #region Left

        private void DrawLeft()
        {
            Sprite tempDeleteSprite = null;
            EditorGUILayout.BeginVertical("Box", GUILayout.Width(200f), GUILayout.ExpandWidth(true));
            {
                leftScorllViewPosition = EditorGUILayout.BeginScrollView(leftScorllViewPosition);
                {
                    foreach (var sprite in SpriteHashSet)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.ObjectField(sprite.texture, typeof(Texture), true,
                                GUILayout.ExpandWidth(true));

                            if (GUILayout.Button("ー", GUILayout.Width(15f), GUILayout.Height(15f)))
                            {
                                tempDeleteSprite = sprite;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.EndScrollView();

                Texture newTexture = null;
                EditorGUI.BeginChangeCheck();
                {
                    if (SpriteHashSet.Count > 0)
                    {
                        EditorGUILayout.Space();
                    }

                    EditorGUILayout.PrefixLabel("Add : ");
                    newTexture =
                        EditorGUILayout.ObjectField(newTexture, typeof(Texture), true, GUILayout.ExpandWidth(true)) as
                            Texture;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    if (newTexture != null)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(newTexture);
                        SpriteHashSet.Add(AssetDatabase.LoadAssetAtPath<Sprite>(assetPath));
                        cacheSprite = SpriteHashSet.ToArray();
                    }
                }

                EditorGUILayout.PrefixLabel("Add from folder : ");

                EditorGUILayout.BeginHorizontal();
                {
                    DefaultAsset targetFolder = null;
                    EditorGUI.BeginChangeCheck();
                    {
                        targetFolder = EditorGUILayout.ObjectField(null, typeof(DefaultAsset), true) as DefaultAsset;
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (targetFolder != null)
                        {
                            LoadAssetsAtPath(AssetDatabase.GetAssetPath(targetFolder));
                        }
                    }

                    if (GUILayout.Button("Select"))
                    {
                        OnClickLoadFolder();
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (tempDeleteSprite != null)
                {
                    SpriteHashSet.Remove(tempDeleteSprite);
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Clear All"))
                {
                    OnClickClearButton();
                }
            }
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Right

        private void DrawRight()
        {
            rightScorllViewPosition = EditorGUILayout.BeginScrollView(rightScorllViewPosition,
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                if (ValidateIsPackingEnabled())
                {
                    Matrix4x4 matrix = Handles.matrix;
                    InitStyles();
                    RefreshState();
                    Rect rect = DrawToolbarGUI();
                    if (m_Texture != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            m_TextureViewRect = new Rect(0.0f, rect.yMax, position.width - 16f - 206f,
                                position.height - 16f - rect.height);
                            GUILayout.FlexibleSpace();
                            DoTextureGUI();

                            string textureFormatString =
                                (string)GetTextureFormatStringMethodInfo.Invoke(null,
                                    new object[] { m_Texture.format });
                            EditorGUI.DropShadowLabel(
                                new Rect(m_TextureViewRect.x, m_TextureViewRect.y + 10f, m_TextureViewRect.width, 20f),
                                string.Format("{1}x{2}, {0}", textureFormatString, m_Texture.width, m_Texture.height));
                        }
                        EditorGUILayout.EndHorizontal();
                        Handles.matrix = matrix;
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private Rect DrawToolbarGUI()
        {
            Rect rect = new Rect(0.0f, 0.0f, this.position.width - 206f, 17f);
            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                EditorStyles.toolbar.Draw(rect, false, false, false, false);
            }

            // Pack Button / Repack Button
            bool enabled = GUI.enabled;
            GUI.enabled = m_AtlasNames.Length > 0;
            Rect toolbarRect = DoAlphaZoomToolbarGUI(rect);

            GUI.enabled = enabled;
            Rect drawRect = new Rect(5f, 0.0f, 0.0f, 17f);
            toolbarRect.width -= drawRect.x;
            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                drawRect.width = EditorStyles.toolbarButton.CalcSize(PackerWindowStyle.packLabel).x;

                DrawToolBarWidget(ref drawRect, ref toolbarRect, adjustedDrawRect =>
                {
                    if (!GUI.Button(adjustedDrawRect, PackerWindowStyle.packLabel, EditorStyles.toolbarButton)) return;
                    OnClickPackPreview();
                    m_SelectedSprite = null;
                    RefreshAtlasPageList();
                    RefreshState();
                });

                using (new EditorGUI.DisabledScope(Packer.SelectedPolicy == DefaultPolice))
                {
                    drawRect.x += drawRect.width;
                    drawRect.width = EditorStyles.toolbarButton.CalcSize(PackerWindowStyle.repackLabel).x;
                    DrawToolBarWidget(ref drawRect, ref toolbarRect, adjustedDrawRect =>
                    {
                        if (GUI.Button(adjustedDrawRect, PackerWindowStyle.repackLabel, EditorStyles.toolbarButton))
                        {
                            OnClickPackPreview();
                            m_SelectedSprite = null;
                            RefreshAtlasPageList();
                            RefreshState();
                        }
                    });
                }
            }

            // ViewAtlas / Page
            float x = GUI.skin.label.CalcSize(PackerWindowStyle.viewAtlasLabel).x;
            float y = 32f;
            float num = (float)(x + 100.0 + 70.0 + 100.0);
            drawRect.x += 5f;
            toolbarRect.width -= 5f;
            float width = toolbarRect.width - y;
            using (new EditorGUI.DisabledScope(m_AtlasNames.Length == 0))
            {
                drawRect.x += drawRect.width;
                drawRect.width = x / num * width;
                DrawToolBarWidget(ref drawRect, ref toolbarRect,
                    adjustedDrawArea => GUI.Label(adjustedDrawArea, PackerWindowStyle.viewAtlasLabel));

                // ViewAtlas
                EditorGUI.BeginChangeCheck();
                {
                    drawRect.x += drawRect.width;
                    drawRect.width = 100f / num * width;

                    DrawToolBarWidget(ref drawRect, ref toolbarRect,
                        adjustedDrawArea => m_SelectedAtlas = EditorGUI.Popup(adjustedDrawArea, m_SelectedAtlas,
                            m_AtlasNames, EditorStyles.toolbarPopup));
                }
                if (EditorGUI.EndChangeCheck())
                {
                    RefreshAtlasPageList();
                    m_SelectedSprite = null;
                }

                // Page
                EditorGUI.BeginChangeCheck();
                {
                    drawRect.x += drawRect.width;
                    drawRect.width = 70f / num * width;
                    DrawToolBarWidget(ref drawRect, ref toolbarRect,
                        adjustedDrawArea => m_SelectedPage = EditorGUI.Popup(adjustedDrawArea, m_SelectedPage,
                            m_PageNames, EditorStyles.toolbarPopup));
                }
                if (EditorGUI.EndChangeCheck())
                {
                    m_SelectedSprite = null;
                }
            }

            // Policy Select
            string[] policies = Packer.Policies
                .Where(policie => policie.StartsWith("SpriteAssetViewer") && !policie.EndsWith("Base")).ToArray();
            int selectedPolicy = Array.IndexOf(policies, Packer.SelectedPolicy);
            EditorGUI.BeginChangeCheck();
            {
                drawRect.x += drawRect.width;
                drawRect.width = 100f / num * width;
                DrawToolBarWidget(ref drawRect, ref toolbarRect,
                    adjustedDrawArea => selectedPolicy = EditorGUI.Popup(adjustedDrawArea, selectedPolicy, policies,
                        EditorStyles.toolbarPopup));
            }
            if (EditorGUI.EndChangeCheck())
            {
                Packer.SelectedPolicy = policies[selectedPolicy];
            }

            drawRect.x += drawRect.width;
            drawRect.width = y;
            DrawToolBarWidget(ref drawRect, ref toolbarRect, adjustedDrawRect =>
            {
                if (GUI.Button(adjustedDrawRect, "＋", EditorStyles.toolbarButton))
                {
                    OnClickPlusPackerPolicyButton();
                }
            });

            return toolbarRect;
        }

        #endregion

        #endregion

        #region 外部Api

        [MenuItem("Tools/SpriteViewer/SpritePackerPreview")]
        public static void ShowWindow()
        {
            if (instance != null)
            {
                instance.Close();
            }

            instance = EditorWindow.GetWindow<SpritePackingPreviewWindow>();
            instance.SpriteHashSet = new HashSet<Sprite>();
            instance.minSize = new Vector2(600f, 400f);

            instance.Show();
            instance.Focus();
        }

        public static void ShowWindow(EditorWindow mainWindow, Sprite[] sprites)
        {
            if (instance != null)
            {
                instance.Close();
            }

            instance = EditorWindow.GetWindow<SpritePackingPreviewWindow>();
            instance.SpriteHashSet = new HashSet<Sprite>(sprites);
            instance.minSize = new Vector2(600f, 400f);
            Vector2 targetPosition = new Vector2(mainWindow.position.position.x + mainWindow.position.size.x,
                mainWindow.position.position.y);
            Vector2 targetSize = new Vector2(instance.minSize.x, mainWindow.position.size.y);
            instance.position = new Rect(targetPosition, targetSize);

            instance.Show();
            instance.Focus();
        }

        #endregion

        #region 内部メソッド

        #region ClickEvent

        private void OnClickClearButton()
        {
            SpriteHashSet.Clear();
            Repaint();
        }

        private void OnClickLoadFolder()
        {
            LoadAssetsAtPath(EditorUtility.OpenFolderPanel("Load png Textures", "Assets", ""));
        }

        private void OnClickPackPreview()
        {
            if (SpriteHashSet == null || SpriteHashSet.Count == 0)
            {
                return;
            }

            TextureImporterInstanceIDs = new int[SpriteHashSet.Count];
            var index = 0;
            foreach (var texturePath in SpriteHashSet.Select(sprite => AssetDatabase.GetAssetPath(sprite)))
            {
                if (AssetImporter.GetAtPath(texturePath) is TextureImporter { } targetimporter)
                    TextureImporterInstanceIDs[index] = targetimporter.GetInstanceID();

                index++;
            }

            if (TextureImporterInstanceIDs == null || TextureImporterInstanceIDs.Length == 0)
            {
                return;
            }

            typeof(Packer).GetMethod("RegenerateList", BindingFlags.Static | BindingFlags.NonPublic)
                ?.Invoke(null, new object[0]);

            var selectedPolicyBefore = Packer.SelectedPolicy;

            Packer.SelectedPolicy = "SpriteAssetViewerDefaultPackerPolicy";

#pragma warning disable 618
            Packer.RebuildAtlasCacheIfNeeded(EditorUserBuildSettings.activeBuildTarget, true,
                Packer.Execution.ForceRegroup);
#pragma warning restore 618

            Packer.SelectedPolicy = selectedPolicyBefore;
        }

        private void OnClickPlusPackerPolicyButton()
        {
            Application.OpenURL(SpriteAssetViewerUtility.PlusPolicyHelpUrl);
        }

        #endregion

        private void Reset()
        {
            RefreshAtlasNameList();
            RefreshAtlasPageList();

            m_SelectedAtlas = 0;
            m_SelectedPage = 0;
            m_SelectedSprite = null;
        }

        private void RefreshState()
        {
            string[] atlasNames = Packer.atlasNames;

            if (!atlasNames.SequenceEqual(m_AtlasNames))
            {
                if (atlasNames.Length == 0)
                {
                    Reset();
                    return;
                }

                OnAtlasNameListChanged();
            }

            if (m_AtlasNames.Length == 0)
            {
                SetNewTexture(null);
            }
            else
            {
                if (m_SelectedAtlas >= m_AtlasNames.Length)
                {
                    m_SelectedAtlas = 0;
                }

                string atlasName = m_AtlasNames[m_SelectedAtlas];
                Texture2D[] texturesForAtlas1 = Packer.GetTexturesForAtlas(atlasName);

                if (m_SelectedPage >= texturesForAtlas1.Length)
                {
                    m_SelectedPage = 0;
                }

                SetNewTexture(texturesForAtlas1[m_SelectedPage]);
                Texture2D[] texturesForAtlas2 = Packer.GetAlphaTexturesForAtlas(atlasName);
                SetAlphaTextureOverride(m_SelectedPage >= texturesForAtlas2.Length
                    ? null
                    : texturesForAtlas2[m_SelectedPage]);
            }
        }

        private void RefreshAtlasNameList()
        {
            m_AtlasNames = Packer.atlasNames;

            if (m_SelectedAtlas < m_AtlasNames.Length)
            {
                return;
            }

            m_SelectedAtlas = 0;
        }

        private void RefreshAtlasPageList()
        {
            if (m_AtlasNames.Length > 0)
            {
                Texture2D[] texturesForAtlas = Packer.GetTexturesForAtlas(m_AtlasNames[m_SelectedAtlas]);
                m_PageNames = new string[texturesForAtlas.Length];

                for (int index = 0; index < texturesForAtlas.Length; ++index)
                {
                    m_PageNames[index] = string.Format(PackerWindowStyle.pageContentLabel.text, index + 1);
                }
            }
            else
            {
                m_PageNames = s_PageNamesEmpty;
            }

            if (m_SelectedPage < m_PageNames.Length)
            {
                return;
            }

            m_SelectedPage = 0;
        }

        private bool ValidateIsPackingEnabled()
        {
#pragma warning disable 618
            if (EditorSettings.spritePackerMode == SpritePackerMode.BuildTimeOnly ||
#pragma warning restore 618
#pragma warning disable 618
                EditorSettings.spritePackerMode == SpritePackerMode.AlwaysOn)
#pragma warning restore 618
            {
                return true;
            }

            EditorGUILayout.BeginVertical();
            {
                GUILayout.Label(PackerWindowStyle.packingDisabledLabel);
                if (GUILayout.Button(PackerWindowStyle.openProjectSettingButton))
                {
                    EditorApplication.ExecuteMenuItem("Edit/Project Settings/Editor");
                }
            }
            EditorGUILayout.EndVertical();

            return false;
        }

        private void OnAtlasNameListChanged()
        {
            if (m_AtlasNames.Length > 0)
            {
                string[] atlasNames = Packer.atlasNames;
                string curAtlasName = m_AtlasNames[m_SelectedAtlas];
                string newAtlasName = (atlasNames.Length <= m_SelectedAtlas) ? null : atlasNames[m_SelectedAtlas];

                if (curAtlasName.Equals(newAtlasName))
                {
                    RefreshAtlasNameList();
                    RefreshAtlasPageList();
                    m_SelectedSprite = null;
                    return;
                }
            }

            Reset();
        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject == null)
            {
                return;
            }

            Sprite activeObject = Selection.activeObject as Sprite;

            if (!(activeObject != m_SelectedSprite))
            {
                return;
            }

            if (activeObject != null)
            {
                Packer.GetAtlasDataForSprite(activeObject, out var selAtlasName, out var selAtlasTexture);
                var index1 = ((IEnumerable<string>)m_AtlasNames).ToList().FindIndex(s => selAtlasName == s);

                if (index1 == -1)
                {
                    return;
                }

                var index2 = Packer.GetTexturesForAtlas(selAtlasName).ToList().FindIndex(t => selAtlasTexture == t);
                if (index2 == -1)
                {
                    return;
                }

                m_SelectedAtlas = index1;
                m_SelectedPage = index2;
                RefreshAtlasPageList();
            }

            m_SelectedSprite = activeObject;
            Repaint();
        }

        private void LoadAssetsAtPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            List<Sprite> targetSprites = SpriteAssetViewerUtility.GetAssetList<Sprite>(path);
            if (targetSprites != null)
            {
                for (int i = 0; i < targetSprites.Count; i++)
                {
                    SpriteHashSet.Add(targetSprites[i]);
                }

                cacheSprite = SpriteHashSet.ToArray();
            }
            else
            {
                EditorUtility.DisplayDialog("Info", "Can't find any textures.", "ok");
            }
        }

        #endregion
    }
}
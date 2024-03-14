using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Event = UnityEngine.Event;

namespace ZJYFrameWork.SpriteViewer
{
    internal partial class SpriteAssetViewerWindow : EditorWindow
    {
        #region Param

        public enum EditorMode
        {
            SpritePacker,
            SpriteAtlas,
        }

        [System.Serializable]
        public class PackingAsset
        {
            public string PackingTagName;
            public List<Sprite> PackingSprites;

            public PackingAsset(string packingTagName, Sprite packingSprite)
            {
                PackingTagName = packingTagName;
                PackingSprites = new List<Sprite>();
                PackingSprites.Add(packingSprite);
            }
        }

        private static SpriteAssetViewerWindow instance;

        private string[] cachedSpriteGuids = null;

        [SerializeField] private List<PackingAsset> cachedPackingAssets;

        [SerializeField] private AtlasAssetGroup cachedSpriteAtlasGroup;

        private EditorMode currentMode = EditorMode.SpritePacker;

        private SpriteAssetViewerModeBase currentEditorMode;

        public SpriteAssetViewerModeBase CurrentEditorMode
        {
            get
            {
                if (currentEditorMode == null)
                {
                    currentEditorMode = editorModeBuilder[currentMode].Invoke();
                    currentEditorMode.OnModeIn();
                }
                else if (currentEditorMode.GetMode() != currentMode)
                {
                    currentEditorMode.OnModeOut();
                    currentEditorMode = editorModeBuilder[currentMode].Invoke();
                    currentEditorMode.OnModeIn();
                }

                return currentEditorMode;
            }
        }

        private readonly Dictionary<EditorMode, Func<SpriteAssetViewerModeBase>> editorModeBuilder =
            new Dictionary<EditorMode, Func<SpriteAssetViewerModeBase>>()
            {
                {
                    EditorMode.SpritePacker,
                    () => new SpriteAssetViewerSpritePackerMode(SpriteAssetViewerWindow
                        .GetWindow<SpriteAssetViewerWindow>())
                },
                {
                    EditorMode.SpriteAtlas,
                    () => new SpriteAssetViewerSpriteAtlasMode(SpriteAssetViewerWindow
                        .GetWindow<SpriteAssetViewerWindow>())
                }
            };

        public SpriteAssetViewerUtility.Styles styles
        {
            get { return SpriteAssetViewerUtility.SpriteAssetViewerStyles; }
        }

        public SpriteAssetViewerSetting Setting
        {
            get { return SpriteAssetViewerSetting.GetSetting(); }
        }

        #endregion

        #region LifeCycle

        private void OnGUI()
        {
            GUILayout.Space(8f);

            float topHeight = 10f;
            Rect contentsRect = new Rect(Vector2.zero, position.size);
            contentsRect = contentsRect.MoveV(topHeight);

            EditorGUILayout.BeginHorizontal();
            {
                float toolBarHeight = 20f;

                Rect toolBarRect = new Rect(contentsRect);
                toolBarRect.height = toolBarHeight;

                contentsRect = contentsRect.MoveV(toolBarHeight);

                DrawEditorMode(toolBarRect);

                DrawOptions(toolBarRect);
            }
            EditorGUILayout.EndHorizontal();

            float spaceHeight = 8f;
            GUILayout.Space(spaceHeight);

            contentsRect = contentsRect.MoveV(spaceHeight);

            if (CurrentEditorMode != null)
            {
                CurrentEditorMode.DrawMode(contentsRect);
            }

            EditorGUILayout.Space();
        }

        private void DrawEditorMode(Rect rect)
        {
            GUILayout.BeginArea(rect);
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    var newMode = (EditorMode)GUILayout.Toolbar((int)currentMode, styles.EditorModeToggles,
                        styles.TabButtonStyle, styles.TabButtonSize, GUILayout.Height(20f));
                    if (newMode != currentMode)
                    {
                        ChangeEditorMode(newMode);
                    }

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }

        private void DrawOptions(Rect rect)
        {
            GUILayout.BeginArea(rect);
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    CurrentEditorMode.DrawExToolbar();

                    if (GUILayout.Button(styles.HelpIcon, styles.LabelStyle, GUILayout.Width(20f),
                            GUILayout.Height(20f)))
                    {
                        OnClickHelp();
                    }

                    if (GUILayout.Button(styles.SettingIcon, styles.LabelStyle, GUILayout.Width(20f),
                            GUILayout.Height(20f)))
                    {
                        OnClickSetting();
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }

        private void OnDestroy()
        {
            instance = null;
        }

        #endregion

        #region 外部Api

        [MenuItem("Tools/SpriteViewer/SpriteAssetViewer")]
        public static void ShowWindow()
        {
            if (instance == null)
            {
                instance = GetWindow<SpriteAssetViewerWindow>();
                instance.minSize = new Vector2(300f, 400f);
                switch (EditorSettings.spritePackerMode)
                {
                    case SpritePackerMode.Disabled:
                    // case SpritePackerMode.BuildTimeOnly:
                    // case SpritePackerMode.AlwaysOn:
                        instance.currentMode = EditorMode.SpritePacker;
                        break;
                    case SpritePackerMode.BuildTimeOnlyAtlas:
                    case SpritePackerMode.AlwaysOnAtlas:
                        instance.currentMode = EditorMode.SpriteAtlas;
                        break;
                    default:
                        instance.currentMode = EditorMode.SpritePacker;
                        break;
                }
            }
            else
            {
                instance.Focus();
            }
        }

        public static void ShowWindowAndSearchAtlas(UnityEngine.Object targetObject)
        {
            if (targetObject == null)
            {
                return;
            }

            ShowWindow();
            SpriteAssetViewerSpriteAtlasMode spriteAtlasMode =
                instance.CurrentEditorMode as SpriteAssetViewerSpriteAtlasMode;
            if (spriteAtlasMode == null)
            {
                return;
            }

            string targetGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(targetObject));

            spriteAtlasMode.SearchSpriteAtlas(targetGUID);
        }

        public static bool IsOpened()
        {
            return instance != null;
        }

        public void Reload()
        {
            if (CurrentEditorMode != null)
            {
                CurrentEditorMode.Reload();
            }

            Repaint();
        }

        public void HandleHorizonResize(Rect resizeRect, Vector2 basePosition, ref bool isResize, ref float resizeWidth,
            Vector2 resizeRange)
        {
            EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeHorizontal);
            if (UnityEngine.Event.current.type == EventType.MouseDown &&
                resizeRect.Contains(UnityEngine.Event.current.mousePosition))
            {
                isResize = true;
            }

            if (isResize)
            {
                resizeWidth = Mathf.Clamp(
                    UnityEngine.Event.current.mousePosition.x - basePosition.x - resizeRect.width * 0.5f,
                    resizeRange.x, resizeRange.y);
                Repaint();
            }

            if (UnityEngine.Event.current.type == EventType.MouseUp && isResize)
            {
                isResize = false;
            }
        }

        #endregion


        private void OnClickSetting()
        {
            GenericMenu menu = new GenericMenu();
            Vector3 position = UnityEngine.Event.current.mousePosition;
            CurrentEditorMode.DrawSetting(menu);
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Tool Setting"), false, () => OpenPreferencesSetting());
            menu.DropDown(new Rect(position, Vector2.zero));
        }

        private void OnClickHelp()
        {
            Application.OpenURL(SpriteAssetViewerUtility.MainHelpUrl);
        }


        private void ChangeEditorMode(EditorMode targetMode)
        {
            if (targetMode == currentMode)
            {
                return;
            }

            currentMode = targetMode;
        }

        private void OpenPreferencesSetting()
        {
            try
            {
#if UNITY_2018_3_OR_NEWER
                SettingsService.OpenUserPreferences(
                    $"Preferences/{SpriteAssetViewerPreferences.PreferenceCategoryKey}");
#else
                //Open preferences window
                Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.EditorWindow));
                Type type = assembly.GetType("UnityEditor.PreferencesWindow");
                type.GetMethod("ShowPreferencesWindow", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);

                //Get the window
                EditorWindow window = EditorWindow.GetWindow(type);

                //Make sure custom sections are added (because waiting for it to happen automatically is too slow)
                FieldInfo refreshField =
 type.GetField("m_RefreshCustomPreferences", BindingFlags.NonPublic | BindingFlags.Instance);
                if ((bool)refreshField.GetValue(window))
                {
                    type.GetMethod("AddCustomSections", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(window, null);
                    refreshField.SetValue(window, false);
                }

                //Get sections
                FieldInfo sectionsField = type.GetField("m_Sections", BindingFlags.Instance | BindingFlags.NonPublic);
                IList sections = sectionsField.GetValue(window) as IList;

                //Iterate through sections and check contents
                Type sectionType = sectionsField.FieldType.GetGenericArguments()[0];
                FieldInfo sectionContentField =
 sectionType.GetField("content", BindingFlags.Instance | BindingFlags.Public);
                for (int i = 0; i < sections.Count; i++)
                {
                    GUIContent sectionContent = sectionContentField.GetValue(sections[i]) as GUIContent;
                    if (sectionContent.text == SpriteAssetViewerPreferences.PreferenceCategoryKey)
                    {
                        //Found contents - Set index
                        FieldInfo sectionIndexField =
 type.GetField("m_SelectedSectionIndex", BindingFlags.Instance | BindingFlags.NonPublic);
                        sectionIndexField.SetValue(window, i);
                        return;
                    }
                }
#endif
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogWarning("Unity has changed around internally. Can't open properties through reflection.");
            }
        }
    }
}
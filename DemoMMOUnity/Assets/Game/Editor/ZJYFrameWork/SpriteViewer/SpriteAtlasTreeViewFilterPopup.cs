using System;
using UnityEditor;
using UnityEngine;
using Event=UnityEngine.Event;

namespace ZJYFrameWork.SpriteViewer
{
    internal class SpriteAtlasTreeViewFilterPopup : PopupWindowContent
    {
        private readonly SpriteAssetViewerWindow _window;

        private SpriteAssetViewerWindow.SpriteAssetViewerSpriteAtlasMode spriteAtlasMode
        {
            get
            {
                if (_window == null)
                {
                    return null;
                }

                return _window.CurrentEditorMode as SpriteAssetViewerWindow.SpriteAssetViewerSpriteAtlasMode;
            }
        }

        internal enum SearchType
        {
            SpriteAtlas_Name = -1,
            AtlasContents_Name = 0,
            AtlasContents_GUID = 1,
            Max,
        }

        internal static readonly string[] SearchTypeTag = new string[2]
        {
            "t:",
            "g:"
        };

        public static SearchType GetSearchType(string searchLowerStr)
        {
            if (!string.IsNullOrEmpty(searchLowerStr))
            {
                for (int i = 0; i < SearchTypeTag.Length; i++)
                {
                    if (searchLowerStr.StartsWith(SearchTypeTag[i]))
                    {
                        return (SearchType)i;
                    }
                }
            }

            return SearchType.SpriteAtlas_Name;
        }

        internal SpriteAtlasTreeViewFilterPopup(SpriteAssetViewerWindow window)
        {
            _window = window;
        }

        public override void OnGUI(Rect rect)
        {
            if (UnityEngine.Event.current.type == EventType.Layout)
            {
                return;
            }

            // Draw
            Draw(rect);

            if (UnityEngine.Event.current.type == EventType.MouseMove)
            {
                UnityEngine.Event.current.Use();
            }

            if (UnityEngine.Event.current.type == EventType.KeyDown && UnityEngine.Event.current.keyCode == KeyCode.Escape)
            {
                editorWindow.Close();
                GUIUtility.ExitGUI();
            }
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(
                180f,
                (int)SearchType.Max * SpriteAssetViewerUtility.Styles.POPUP_ITEM_HEIGHT +
                SpriteAssetViewerUtility.Styles.POPUP_PADDING * 2
            );
        }

        private void Draw(Rect rect)
        {
            rect.y += SpriteAssetViewerUtility.Styles.POPUP_PADDING;
            rect.xMin += 5;
            rect.height = SpriteAssetViewerUtility.Styles.POPUP_ITEM_HEIGHT;

            for (int i = 0; i < (int)SearchType.Max; i++)
            {
                bool match = false;
                if (spriteAtlasMode != null)
                {
                    match = spriteAtlasMode.CacheSearchString.ToLower().StartsWith(SearchTypeTag[i]);
                }

                DrawElement(rect, ((SearchType)i).ToString(), match);
                rect.y += SpriteAssetViewerUtility.Styles.POPUP_ITEM_HEIGHT;
            }
        }

        private void DrawElement(Rect rect, string toggleName, bool value)
        {
            EditorGUI.BeginChangeCheck();
            value = GUI.Toggle(rect, value, toggleName, SpriteAssetViewerUtility.SpriteAssetViewerStyles.MenuItem);
            if (EditorGUI.EndChangeCheck())
            {
                if (spriteAtlasMode != null)
                {
                    if (value)
                    {
                        spriteAtlasMode.Search(SearchTypeTag[(int)Enum.Parse(typeof(SearchType), toggleName)]);
                    }
                    else
                    {
                        spriteAtlasMode.Search(string.Empty);
                    }
                }
            }
        }
    }
}
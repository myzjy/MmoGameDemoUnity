using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ZJYFrameWork.SpriteViewer
{
    internal class SpriteAssetViewerUtilityWindow : EditorWindow
    {
        protected bool m_ShowAlpha = false;
        protected float m_Zoom = -1f;
        protected float m_MipLevel = 0.0f;
        protected Vector2 m_ScrollPosition = new Vector2();
        protected Styles m_Styles;
        protected const float k_BorderMargin = 10f;
        protected const float k_ScrollbarMargin = 16f;
        protected const float k_InspectorWindowMargin = 8f;
        protected const float k_InspectorWidth = 330f;
        protected const float k_MinZoomPercentage = 0.9f;
        protected const float k_MaxZoom = 50f;
        protected const float k_WheelZoomSpeed = 0.03f;
        protected const float k_MouseZoomSpeed = 0.005f;
        protected const float k_ToolbarHeight = 17f;
        protected Texture2D m_Texture;
        protected Texture2D m_TextureAlphaOverride;
        protected Rect m_TextureViewRect;
        protected Rect m_TextureRect;

        private Assembly unityEngineAssembly;

        protected Assembly UnityEngineAssembly
        {
            get
            {
                if (unityEngineAssembly == null)
                {
                    unityEngineAssembly = Assembly.GetAssembly(typeof(GUIStyle));
                }

                return unityEngineAssembly;
            }
        }

        private Assembly unityEditorAssembly;

        protected Assembly UnityEditorAssembly
        {
            get
            {
                if (unityEditorAssembly == null)
                {
                    unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
                }

                return unityEditorAssembly;
            }
        }

        private Type guiClipTYpe;

        private Type GUIClipType
        {
            get
            {
                if (guiClipTYpe == null)
                {
                    guiClipTYpe = UnityEngineAssembly.GetType("UnityEngine.GUIClip");
                }

                return guiClipTYpe;
            }
        }

        private MethodInfo guiClipPopMethodInfo;

        private MethodInfo GUIClipPopMethodInfo
        {
            get
            {
                if (guiClipPopMethodInfo == null)
                {
                    guiClipPopMethodInfo = GUIClipType.GetMethod("Pop", BindingFlags.NonPublic | BindingFlags.Static);
                }

                return guiClipPopMethodInfo;
            }
        }

        private MethodInfo guiClipPushMethodInfo;

        private MethodInfo GUIClipPushMethodInfo
        {
            get
            {
                if (guiClipPushMethodInfo == null)
                {
                    guiClipPushMethodInfo = GUIClipType.GetMethod("Push", BindingFlags.NonPublic | BindingFlags.Static);
                }

                return guiClipPushMethodInfo;
            }
        }

        private MethodInfo getMipmapCountMethodInfo;

        protected MethodInfo GetMipmapCountMethodInfo
        {
            get
            {
                if (getMipmapCountMethodInfo == null)
                {
                    Type TextureUtilType = UnityEditorAssembly.GetType("UnityEditor.TextureUtil");
                    getMipmapCountMethodInfo =
                        TextureUtilType.GetMethod("GetMipmapCount", BindingFlags.Public | BindingFlags.Static);
                }

                return getMipmapCountMethodInfo;
            }
        }

        private MethodInfo setFilterModeNoDirtyMethodInfo;

        protected MethodInfo SetFilterModeNoDirtyMethodInfo
        {
            get
            {
                if (setFilterModeNoDirtyMethodInfo == null)
                {
                    Type TextureUtilType = UnityEditorAssembly.GetType("UnityEditor.TextureUtil");
                    setFilterModeNoDirtyMethodInfo = TextureUtilType.GetMethod("SetFilterModeNoDirty",
                        BindingFlags.Public | BindingFlags.Static);
                }

                return setFilterModeNoDirtyMethodInfo;
            }
        }

        protected void InitStyles()
        {
            if (m_Styles != null)
            {
                return;
            }

            m_Styles = new Styles();
        }

        protected float GetMinZoom()
        {
            return m_Texture == null
                ? 1f
                : Mathf.Min(m_TextureViewRect.width / m_Texture.width, m_TextureViewRect.height / m_Texture.height,
                    50f) * 0.9f;
        }

        protected void HandleZoom()
        {
            bool flag = UnityEngine.Event.current.alt && UnityEngine.Event.current.button == 1;
            if (flag)
            {
                EditorGUIUtility.AddCursorRect(m_TextureViewRect, MouseCursor.Zoom);
            }

            if ((UnityEngine.Event.current.type is EventType.MouseUp or EventType.MouseDown && flag) ||
                (UnityEngine.Event.current.type is EventType.KeyUp or EventType.KeyDown &&
                 UnityEngine.Event.current.keyCode == KeyCode.LeftAlt))
            {
                Repaint();
            }

            if (UnityEngine.Event.current.type != EventType.ScrollWheel &&
                (UnityEngine.Event.current.type != EventType.MouseDrag ||
                 !UnityEngine.Event.current.alt || UnityEngine.Event.current.button != 1))
            {
                return;
            }

            var num1 = (float)(1.0 - (UnityEngine.Event.current.delta.y *
                                      (UnityEngine.Event.current.type != EventType.ScrollWheel
                                          ? -0.00499999988824129
                                          : 0.0299999993294477)));
            float num2 = m_Zoom * num1;
            float num3 = Mathf.Clamp(num2, GetMinZoom(), 50f);
            if (num3 != (double)m_Zoom)
            {
                m_Zoom = num3;
                if (num2 != (double)num3)
                {
                    num1 /= num2 / num3;
                }

                m_ScrollPosition *= num1;
                float num4 =
                    (float)((UnityEngine.Event.current.mousePosition.x / (double)m_TextureViewRect.width) - 0.5);
                float num5 = (float)((UnityEngine.Event.current.mousePosition.y / (double)m_TextureViewRect.height) -
                                     0.5);
                float num6 = num4 * (num1 - 1f);
                float num7 = num5 * (num1 - 1f);
                Rect maxScrollRect = this.maxScrollRect;
                m_ScrollPosition.x += num6 * (maxScrollRect.width / 2f);
                m_ScrollPosition.y += num7 * (maxScrollRect.height / 2f);
                UnityEngine.Event.current.Use();
            }
        }

        protected void HandlePanning()
        {
            bool flag = (!UnityEngine.Event.current.alt && UnityEngine.Event.current.button > 0) ||
                        (UnityEngine.Event.current.alt && UnityEngine.Event.current.button <= 0);
            if (flag && GUIUtility.hotControl == 0)
            {
                EditorGUIUtility.AddCursorRect(m_TextureViewRect, MouseCursor.Pan);
                if (UnityEngine.Event.current.type == EventType.MouseDrag)
                {
                    m_ScrollPosition -= UnityEngine.Event.current.delta;
                    UnityEngine.Event.current.Use();
                }
            }

            if (((UnityEngine.Event.current.type != EventType.MouseUp &&
                  UnityEngine.Event.current.type != EventType.MouseDown) || !flag) &&
                ((UnityEngine.Event.current.type != EventType.KeyUp &&
                  UnityEngine.Event.current.type != EventType.KeyDown) ||
                 UnityEngine.Event.current.keyCode != KeyCode.LeftAlt))
            {
                return;
            }

            Repaint();
        }

        protected Rect maxScrollRect
        {
            get
            {
                float num1 = m_Texture.width * 0.5f * m_Zoom;
                float num2 = m_Texture.height * 0.5f * m_Zoom;
                return new Rect(-num1, -num2, m_TextureViewRect.width + (num1 * 2f),
                    m_TextureViewRect.height + (num2 * 2f));
            }
        }

        protected Rect maxRect
        {
            get
            {
                float num1 = m_TextureViewRect.width * 0.5f / GetMinZoom();
                float num2 = m_TextureViewRect.height * 0.5f / GetMinZoom();
                return new Rect(-num1, -num2, m_Texture.width + (num1 * 2f), m_Texture.height + (num2 * 2f));
            }
        }

        private float Log2(float x)
        {
            return (float)(Math.Log(x) / Math.Log(2.0));
        }


        protected void DrawTexture()
        {
            float mipmapCount = (int)GetMipmapCountMethodInfo.Invoke(null, new object[] { m_Texture });
            float mipLevel = Mathf.Min(m_MipLevel, mipmapCount - 1);
            FilterMode filterMode = m_Texture.filterMode;
            SetFilterModeNoDirtyMethodInfo.Invoke(null, new object[] { m_Texture, FilterMode.Point });
            if (m_ShowAlpha)
            {
                if (m_TextureAlphaOverride != null)
                {
                    EditorGUI.DrawTextureTransparent(m_TextureRect, m_TextureAlphaOverride, ScaleMode.StretchToFill,
                        0.0f, mipLevel);
                }
                else
                {
                    EditorGUI.DrawTextureAlpha(m_TextureRect, m_Texture, ScaleMode.StretchToFill, 0.0f, mipLevel);
                }
            }
            else
            {
                EditorGUI.DrawTextureTransparent(m_TextureRect, m_Texture, ScaleMode.StretchToFill, 0.0f, mipLevel);
            }

            SetFilterModeNoDirtyMethodInfo.Invoke(null, new object[] { m_Texture, filterMode });
        }

        protected void DrawScreenspaceBackground()
        {
            if (UnityEngine.Event.current.type != EventType.Repaint)
            {
                return;
            }

            m_Styles.preBackground.Draw(m_TextureViewRect, false, false, false, false);
        }

        protected void HandleScrollbars()
        {
            m_ScrollPosition.x =
                GUI.HorizontalScrollbar(
                    new Rect(m_TextureViewRect.xMin, m_TextureViewRect.yMax, m_TextureViewRect.width, 16f),
                    m_ScrollPosition.x, m_TextureViewRect.width, maxScrollRect.xMin, maxScrollRect.xMax);
            m_ScrollPosition.y =
                GUI.VerticalScrollbar(
                    new Rect(m_TextureViewRect.xMax, m_TextureViewRect.yMin, 16f, m_TextureViewRect.height),
                    m_ScrollPosition.y, m_TextureViewRect.height, maxScrollRect.yMin, maxScrollRect.yMax);
        }

        protected void SetupHandlesMatrix()
        {
            Handles.matrix = Matrix4x4.TRS(new Vector3(m_TextureRect.x, m_TextureRect.yMax, 0.0f), Quaternion.identity,
                new Vector3(m_Zoom, -m_Zoom, 1f));
        }

        protected Rect DoAlphaZoomToolbarGUI(Rect area)
        {
            int a = 1;

            if (m_Texture != null)
            {
                a = Mathf.Max(a, (int)GetMipmapCountMethodInfo.Invoke(null, new object[] { m_Texture }));
            }

            Rect position = new Rect(area.width, 0.0f, 0.0f, area.height);

            using (new EditorGUI.DisabledScope(a == 1))
            {
                position.width = m_Styles.largeMip.image.width;
                position.x -= position.width;
                GUI.Box(position, m_Styles.largeMip, m_Styles.preLabel);
                position.width = 60f;
                position.x -= position.width;
                m_MipLevel = Mathf.Round(GUI.HorizontalSlider(position, m_MipLevel, a - 1, 0.0f, m_Styles.preSlider,
                    m_Styles.preSliderThumb));
                position.width = m_Styles.smallMip.image.width;
                position.x -= position.width;
                GUI.Box(position, m_Styles.smallMip, m_Styles.preLabel);
            }

            position.width = 60f;
            position.x -= position.width;
            m_Zoom = GUI.HorizontalSlider(position, m_Zoom, GetMinZoom(), 50f, m_Styles.preSlider,
                m_Styles.preSliderThumb);
            position.width = 32f;
            position.x -= position.width + 5f;
            m_ShowAlpha = GUI.Toggle(position, m_ShowAlpha, !m_ShowAlpha ? m_Styles.RGBIcon : m_Styles.alphaIcon,
                "toolbarButton");

            return new Rect(area.x, area.y, position.x, area.height);
        }

        protected void DoTextureGUI()
        {
            if (m_Texture == null)
            {
                return;
            }

            if (m_Zoom < 0.0)
            {
                m_Zoom = GetMinZoom();
            }

            m_TextureRect = new Rect(
                (float)((m_TextureViewRect.width / 2.0) - (m_Texture.width * (double)m_Zoom / 2.0)),
                (float)((m_TextureViewRect.height / 2.0) - (m_Texture.height * (double)m_Zoom / 2.0)),
                m_Texture.width * m_Zoom,
                m_Texture.height * m_Zoom);

            HandleScrollbars();
            SetupHandlesMatrix();
            DrawScreenspaceBackground();

            GUIClipPushMethodInfo.Invoke(null,
                new object[] { m_TextureViewRect, -m_ScrollPosition, Vector2.zero, false });
            if (UnityEngine.Event.current.type == EventType.Repaint)
            {
                DrawTexture();
                DrawGizmos();
            }

            DoTextureGUIExtras();
            GUIClipPopMethodInfo.Invoke(null, new object[] { });
            HandleZoom();
            HandlePanning();
        }

        protected virtual void DoTextureGUIExtras()
        {
        }

        protected virtual void DrawGizmos()
        {
        }

        protected void SetNewTexture(UnityEngine.Texture2D texture)
        {
            if (!(texture != m_Texture))
            {
                return;
            }

            m_Texture = texture;
            m_Zoom = -1f;
            m_TextureAlphaOverride = null;
        }

        protected void SetAlphaTextureOverride(Texture2D alphaTexture)
        {
            if (!(alphaTexture != m_TextureAlphaOverride))
                return;
            m_TextureAlphaOverride = alphaTexture;
            m_Zoom = -1f;
        }

        internal void OnResized()
        {
            if (!(m_Texture != null) || UnityEngine.Event.current == null)
            {
                return;
            }

            HandleZoom();
        }

        internal static void DrawToolBarWidget(ref Rect drawRect, ref Rect toolbarRect, Action<Rect> drawAction)
        {
            toolbarRect.width -= drawRect.width;

            if (toolbarRect.width < 0.0)
            {
                drawRect.width += toolbarRect.width;
            }

            if (drawRect.width <= 0.0)
            {
                return;
            }

            drawAction(drawRect);
        }

        protected class Styles
        {
            public readonly GUIStyle dragdot = "U2D.dragDot";
            public readonly GUIStyle dragdotDimmed = "U2D.dragDotDimmed";
            public readonly GUIStyle dragdotactive = "U2D.dragDotActive";
            public readonly GUIStyle createRect = "U2D.createRect";
            public readonly GUIStyle preToolbar = "preToolbar";
            public readonly GUIStyle preButton = "preButton";
            public readonly GUIStyle preLabel = "preLabel";
            public readonly GUIStyle preSlider = "preSlider";
            public readonly GUIStyle preSliderThumb = "preSliderThumb";
            public readonly GUIStyle preBackground = "preBackground";
            public readonly GUIStyle pivotdotactive = "U2D.pivotDotActive";
            public readonly GUIStyle pivotdot = "U2D.pivotDot";
            public readonly GUIStyle dragBorderdot = new GUIStyle();
            public readonly GUIStyle dragBorderDotActive = new GUIStyle();
            public readonly GUIStyle toolbar;
            public readonly GUIContent alphaIcon;
            public readonly GUIContent RGBIcon;
            public readonly GUIStyle notice;
            public readonly GUIContent smallMip;
            public readonly GUIContent largeMip;

            public Styles()
            {
                toolbar = new GUIStyle(GetStyle("In BigTitle"))
                {
                    margin =
                    {
                        top = 0,
                        bottom = 0
                    }
                };
                alphaIcon = EditorGUIUtility.IconContent("PreTextureAlpha");
                RGBIcon = EditorGUIUtility.IconContent("PreTextureRGB");
                preToolbar.border.top = 0;
                createRect.border = new RectOffset(3, 3, 3, 3);
                notice = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal =
                    {
                        textColor = Color.yellow
                    }
                };
                dragBorderdot.fixedHeight = 5f;
                dragBorderdot.fixedWidth = 5f;
                dragBorderdot.normal.background = EditorGUIUtility.whiteTexture;
                dragBorderDotActive.fixedHeight = this.dragBorderdot.fixedHeight;
                dragBorderDotActive.fixedWidth = this.dragBorderdot.fixedWidth;
                dragBorderDotActive.normal.background = EditorGUIUtility.whiteTexture;
                smallMip = EditorGUIUtility.IconContent("PreTextureMipMapLow");
                largeMip = EditorGUIUtility.IconContent("PreTextureMipMapHigh");
            }

            private GUIStyle GetStyle(string styleName)
            {
                GUIStyle guiStyle = GUI.skin.FindStyle(styleName) ??
                                    EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
                if (guiStyle == null)
                {
                    UnityEngine.Debug.LogError("Missing built-in guistyle " + styleName);
                }

                return guiStyle;
            }
        }
    }
}
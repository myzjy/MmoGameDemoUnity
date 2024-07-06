using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace FrostEngine
{
    [Serializable]
    public class FrameworkGlobalSettings
    {
        [SerializeField] [Tooltip("脚本作者名")] private string m_ScriptAuthor = "Default";

        public string ScriptAuthor => m_ScriptAuthor;

        [SerializeField] [Tooltip("版本")] private string m_ScriptVersion = "0.1";

        public string ScriptVersion => m_ScriptVersion;

        [FormerlySerializedAs("mEAppStage")] [FormerlySerializedAs("m_AppStage")] [SerializeField]
        private AppStageEnum mAppStageEnum = AppStageEnum.Debug;

        public AppStageEnum AppStageEnum => mAppStageEnum;

        [Header("Font")] [SerializeField] private string m_DefaultFont = "Arial";
        public string DefaultFont => m_DefaultFont;

        [Header("Resources")] [Tooltip("资源存放地")] [SerializeField]
        private ResourcesArea m_ResourcesArea;

        public ResourcesArea ResourcesArea => m_ResourcesArea;

        [Header("SpriteCollection")] [SerializeField]
        private string m_AtlasFolder = "Assets/AssetRaw/Atlas";

        public string AtlasFolder => m_AtlasFolder;

        [Header("Hotfix")] public string HostServerURL = "http://127.0.0.1:8081";

        public string FallbackHostServerURL = "http://127.0.0.1:8081";

        public bool EnableUpdateData = false;

        public string WindowsUpdateDataUrl = "http://127.0.0.1";
        public string MacOSUpdateDataUrl = "http://127.0.0.1";
        public string IOSUpdateDataUrl = "http://127.0.0.1";
        public string AndroidUpdateDataUrl = "http://127.0.0.1";
        public string WebGLUpdateDataUrl = "http://127.0.0.1";
        [Header("Server")] [SerializeField] private string m_CurUseServerChannel;

        public string CurUseServerChannel => m_CurUseServerChannel;

        [SerializeField] private List<ServerChannelInfo> m_ServerChannelInfos;

        public List<ServerChannelInfo> ServerChannelInfos => m_ServerChannelInfos;

        [SerializeField] private string @namespace = "GameLogic";

        public string NameSpace => @namespace;

        [SerializeField] private List<ScriptGenerateRuler> scriptGenerateRule = new List<ScriptGenerateRuler>()
        {
            new ScriptGenerateRuler("m_go", "GameObject"),
            new ScriptGenerateRuler("m_item", "GameObject"),
            new ScriptGenerateRuler("m_tf", "Transform"),
            new ScriptGenerateRuler("m_rect", "RectTransform"),
            new ScriptGenerateRuler("m_text", "Text"),
            new ScriptGenerateRuler("m_richText", "RichTextItem"),
            new ScriptGenerateRuler("m_btn", "Button"),
            new ScriptGenerateRuler("m_img", "Image"),
            new ScriptGenerateRuler("m_rimg", "RawImage"),
            new ScriptGenerateRuler("m_scrollBar", "Scrollbar"),
            new ScriptGenerateRuler("m_scroll", "ScrollRect"),
            new ScriptGenerateRuler("m_input", "InputField"),
            new ScriptGenerateRuler("m_grid", "GridLayoutGroup"),
            new ScriptGenerateRuler("m_hlay", "HorizontalLayoutGroup"),
            new ScriptGenerateRuler("m_vlay", "VerticalLayoutGroup"),
            new ScriptGenerateRuler("m_red", "RedNoteBehaviour"),
            new ScriptGenerateRuler("m_slider", "Slider"),
            new ScriptGenerateRuler("m_group", "ToggleGroup"),
            new ScriptGenerateRuler("m_curve", "AnimationCurve"),
            new ScriptGenerateRuler("m_canvasGroup", "CanvasGroup"),
#if ENABLE_TEXTMESHPRO
        new ScriptGenerateRuler("m_tmp","TextMeshProUGUI"),
#endif
        };

        public List<ScriptGenerateRuler> ScriptGenerateRule => scriptGenerateRule;
    }
}
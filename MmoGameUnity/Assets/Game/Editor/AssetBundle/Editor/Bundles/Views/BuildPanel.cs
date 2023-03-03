using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.Attributes;

namespace ZJYFrameWork.AssetBundles.EditorAssetBundle.Editors
{
    public class BuildPanel : Panel
    {
        private BuildVM buildVM;

        private Vector2 scrollPosition;
        private Vector2 textScrollPosition;

        private List<CheckBoxData<BuildAssetBundleOptions>> buildOptionList;
        private GUIContent buildTargetContent;
        private GUIContent compressionContent;
        private GUIContent dataVersionContent;
        private GUIContent outputPathContent;
        private GUIContent usePlaySettingVersionContent;
        private GUIContent copyToStreamingContent;
        private GUIContent useHashFilenameContent;
        private GUIContent encryptionContent;
        private GUIContent algorithmContent;
        private GUIContent ivContent;
        private GUIContent keyContent;
        private GUIContent filterTypeContent;

        public BuildPanel(EditorWindow parent, BuildVM buildVM) : base(parent)
        {
            this.buildVM = buildVM;
        }


        public override void OnEnable()
        {
            buildOptionList = new List<CheckBoxData<BuildAssetBundleOptions>>();
            buildOptionList.Add(new CheckBoxData<BuildAssetBundleOptions>(
                "添加hash名",
                "assetBundle名称进行hash模糊化.",
                (this.buildVM.BuildAssetBundleOptions & BuildAssetBundleOptions.AppendHashToAssetBundleName) > 0,
                BuildAssetBundleOptions.AppendHashToAssetBundleName));
            buildOptionList.Add(new CheckBoxData<BuildAssetBundleOptions>(
                "排除类型信息",
                "不要在资产包中包含类型信息(不要编写类型树)。",
                (this.buildVM.BuildAssetBundleOptions & BuildAssetBundleOptions.DisableWriteTypeTree) > 0,
                BuildAssetBundleOptions.DisableWriteTypeTree));
            buildOptionList.Add(new CheckBoxData<BuildAssetBundleOptions>(
                "忽略类型树的更改",
                "在执行增量构建检查时，忽略类型树更改.",
                (this.buildVM.BuildAssetBundleOptions & BuildAssetBundleOptions.IgnoreTypeTreeChanges) > 0,
                BuildAssetBundleOptions.IgnoreTypeTreeChanges));
#if UNITY_5_6_OR_NEWER
            buildOptionList.Add(new CheckBoxData<BuildAssetBundleOptions>(
                "严格遵循标准模式",
                "如果在构建过程中报告了任何错误，是否不允许构建成功.",
                (this.buildVM.BuildAssetBundleOptions & BuildAssetBundleOptions.StrictMode) > 0,
                BuildAssetBundleOptions.StrictMode));
            buildOptionList.Add(new CheckBoxData<BuildAssetBundleOptions>(
                "保持运行构建",
                "做一个演练构建.",
                (this.buildVM.BuildAssetBundleOptions & BuildAssetBundleOptions.DryRunBuild) > 0,
                BuildAssetBundleOptions.DryRunBuild));
#endif

            this.outputPathContent = new GUIContent("输出路径", "");
            this.dataVersionContent = new GUIContent("AssetBundle版本", "AssetBundle的数据版本。");
            this.usePlaySettingVersionContent = new GUIContent("使用PlayerSetting版本", "使用PlayerSetting版本.");
            this.copyToStreamingContent = new GUIContent("C复制到StreamingAssets",
                "构建完成后，将复制所有构建内容到Assets/StreamingAssets/bundle，以在独立的播放器中使用。");
            this.useHashFilenameContent = new GUIContent("使用hash文件名", "");
            this.encryptionContent = new GUIContent("加密", "加密AssetBundle的数据。");
            this.algorithmContent =
                new GUIContent("加密算法", "Choose AES128_CBC_PKCS7, AES192_CBC_PKCS7 or AES256_CBC_PKCS7");
            this.filterTypeContent = new GUIContent("Bundles", "");

            this.buildTargetContent = new GUIContent("构建平台", "选择目标平台进行构建.");
            this.compressionContent = new GUIContent("压缩格式", "选择不压缩、标准(LZMA)或基于标准(LZ4)");
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginArea(rect);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.Space();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            //this.buildVM.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup(buildTargetContent, this.buildVM.BuildTarget);

            this.buildVM.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
            EditorGUILayout.LabelField(this.buildTargetContent, new GUIContent(buildVM.BuildTarget.ToString()),
                GUILayout.Width(300f), GUILayout.Height(20));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("切换平台", GUILayout.Width(200f)))
            {
                EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                GUIUtility.ExitGUI();
            }

            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            if (this.buildVM.UsePlayerSettingVersion)
            {
                EditorGUILayout.LabelField(this.dataVersionContent, new GUIContent(buildVM.AppDataVersion),
                    GUILayout.MinWidth(230f), GUILayout.MaxWidth(250f), GUILayout.Height(20));
            }
            else
            {
                buildVM.AppDataVersion = EditorGUILayout.TextField(this.dataVersionContent, buildVM.AppDataVersion,
                    GUILayout.MinWidth(230f), GUILayout.MaxWidth(250f), GUILayout.Height(20));
            }

            this.buildVM.UsePlayerSettingVersion = EditorGUILayout.ToggleLeft(this.usePlaySettingVersionContent,
                this.buildVM.UsePlayerSettingVersion);

            GUILayout.FlexibleSpace();

            if (this.buildVM.UsePlayerSettingVersion)
            {
                if (GUILayout.Button("Player Setting", GUILayout.Width(200f)))
                {
                    Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
                    GUIUtility.ExitGUI();
                }
            }

            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            this.buildVM.OutputPath =
                EditorGUILayout.TextField(this.outputPathContent, this.buildVM.OutputPath, GUILayout.Height(20));

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Browse", GUILayout.MaxWidth(100f), GUILayout.MinHeight(25f)))
            {
                this.buildVM.BrowseOutputFolder();
                this.Repaint();
                GUIUtility.ExitGUI();
            }

            if (GUILayout.Button("Reset", GUILayout.MaxWidth(100f), GUILayout.MinHeight(25f)))
            {
                this.buildVM.ResetOutputFolder();
                this.Repaint();
                GUIUtility.ExitGUI();
            }

            if (GUILayout.Button("Open", GUILayout.MaxWidth(100f), GUILayout.MinHeight(25f)))
            {
                this.buildVM.OpenOutputFolder();
                this.Repaint();
                GUIUtility.ExitGUI();
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            this.buildVM.CopyToStreaming =
                EditorGUILayout.ToggleLeft(this.copyToStreamingContent, this.buildVM.CopyToStreaming);
            this.buildVM.UseHashFilename =
                EditorGUILayout.ToggleLeft(this.useHashFilenameContent, this.buildVM.UseHashFilename);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // advanced options
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            this.buildVM.AdvancedSettings = EditorGUILayout.Foldout(this.buildVM.AdvancedSettings, "Advanced Settings");

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (this.buildVM.AdvancedSettings)
            {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel += 1;

                GUILayout.BeginHorizontal();
                this.buildVM.Compression = (CompressOptions)EditorGUILayout.IntPopup(compressionContent,
                    (int)this.buildVM.Compression,
                    new GUIContent[]
                    {
                        new GUIContent(CompressOptions.Uncompressed.GetRemark()),
                        new GUIContent(CompressOptions.StandardCompression.GetRemark()),
                        new GUIContent(CompressOptions.ChunkBasedCompression.GetRemark())
                    }, new int[] { 0, 1, 2 }, GUILayout.Width(400), GUILayout.Height(20));

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                for (int i = 0; i < buildOptionList.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    var buildOption = buildOptionList[i];

                    var newValue = EditorGUILayout.ToggleLeft(buildOption.Content, buildOption.Value);
                    if (newValue != buildOption.Value)
                    {
                        buildOption.Value = newValue;
                        if (buildOption.Value && buildOption.Tag is BuildAssetBundleOptions.IgnoreTypeTreeChanges
                                or BuildAssetBundleOptions.DisableWriteTypeTree)
                        {
                            foreach (var checkBox2 in buildOptionList.Where(checkBox2 => buildOption != checkBox2 &&
                                         checkBox2.Tag is BuildAssetBundleOptions.IgnoreTypeTreeChanges
                                             or BuildAssetBundleOptions.DisableWriteTypeTree &&
                                         checkBox2.Value))
                            {
                                checkBox2.Value = false;
                                break;
                            }
                        }

                        BuildAssetBundleOptions options = buildOptionList.Where(t => t.Value)
                            .Aggregate(BuildAssetBundleOptions.None, (current, t) => current | t.Tag);

                        this.buildVM.BuildAssetBundleOptions = options;
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                this.buildVM.Encryption = EditorGUILayout.ToggleLeft(this.encryptionContent, this.buildVM.Encryption);

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (this.buildVM.Encryption)
                {
                    int indent2 = EditorGUI.indentLevel;
                    EditorGUI.indentLevel += 1;
                    GUILayout.BeginHorizontal();
                    this.buildVM.Algorithm = (Algorithm)EditorGUILayout.EnumPopup(this.algorithmContent,
                        this.buildVM.Algorithm, GUILayout.Height(20));
#if UNITY_2018_1_OR_NEWER && !NETFX_CORE && !UNITY_WSA && !UNITY_WSA_10_0
                    Color oldColor = GUI.color;
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("推荐(AES128_CTR_NONE) 支持字节流，有更好的性能");
                    GUI.color = oldColor;
#endif
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal();
                    this.buildVM.IV = EditorGUILayout.TextField("IV", this.buildVM.IV, GUILayout.MinWidth(230f),
                        GUILayout.Height(20));

                    if (GUILayout.Button("Generate IV", GUILayout.Width(100f)))
                    {
                        this.buildVM.IV = this.buildVM.GenerateIV();
                    }

                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal();
                    this.buildVM.KEY = EditorGUILayout.TextField("KEY", this.buildVM.KEY, GUILayout.MinWidth(230f),
                        GUILayout.Height(20));
                    if (GUILayout.Button("Generate KEY", GUILayout.Width(100f)))
                    {
                        this.buildVM.KEY = this.buildVM.GenerateKey();
                    }

                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    //EditorGUILayout.LabelField(new GUIContent("Bundles", "All the names of Assetbundle that need to be encrypted."));
                    this.buildVM.FilterType = (EncryptionFilterType)EditorGUILayout.EnumPopup(this.filterTypeContent,
                        this.buildVM.FilterType, GUILayout.Height(20));
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    if (this.buildVM.FilterType == EncryptionFilterType.BundleNameList)
                    {
                        this.textScrollPosition = EditorGUILayout.BeginScrollView(textScrollPosition);
                        this.buildVM.BundleNames = EditorGUILayout.TextArea(this.buildVM.BundleNames,
                            GUILayout.Height(rect.height - 80));
                        EditorGUILayout.EndScrollView();
                    }
                    else if (this.buildVM.FilterType == EncryptionFilterType.RegularExpression)
                    {
                        this.buildVM.FilterExpression = EditorGUILayout.TextField("", this.buildVM.FilterExpression,
                            GUILayout.MinWidth(230f), GUILayout.Height(20));
                    }

                    EditorGUI.indentLevel = indent2;
                }

                EditorGUI.indentLevel = indent;
            }

            EditorGUILayout.Space();


            // build.
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Rebuild", GUILayout.Width(250f), GUILayout.MinHeight(40f)))
            {
                if (!this.buildVM.VersionExists() || EditorUtility.DisplayDialog("Version already exist",
                        "The version already exist!Are you sure you want to replace this version?", "Yes", "No"))
                {
                    EditorApplication.delayCall += () => this.buildVM.Build(true);
                }
            }

            if (GUILayout.Button("Build", GUILayout.Width(250f), GUILayout.MinHeight(40f)))
            {
                if (!this.buildVM.VersionExists() || EditorUtility.DisplayDialog("Version already exist",
                        "The version already exist!Are you sure you want to replace this version?", "Yes", "No"))
                {
                    EditorApplication.delayCall += () => this.buildVM.Build(false);
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear from StreamingAssets", GUILayout.Width(250f), GUILayout.MinHeight(40f)))
            {
                EditorApplication.delayCall += () => this.buildVM.ClearFromStreamingAssets();
            }

            if (GUILayout.Button("Copy to StreamingAssets", GUILayout.Width(250f), GUILayout.MinHeight(40f)))
            {
                EditorApplication.delayCall += () => this.buildVM.CopyToStreamingAssets();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public class PropertyData<T>
        {
            protected string prefsKey;
            protected GUIContent content;
            protected T value;
            protected GUILayoutOption[] options;

            public PropertyData(string text, string tooltip, T value) : this(new GUIContent(text, tooltip), value, null)
            {
            }

            public PropertyData(GUIContent content, T value) : this(content, value, null)
            {
            }

            public PropertyData(GUIContent content, T value, GUILayoutOption[] options)
            {
                this.content = content;
                this.value = value;
                this.options = options;
                this.prefsKey = this.content.text;
                if (this.options == null)
                    this.options = new GUILayoutOption[] { };
            }

            public string PrefsKey
            {
                get { return this.prefsKey; }
            }

            public GUIContent Content
            {
                get { return this.content; }
            }

            public GUILayoutOption[] Options
            {
                get { return this.options; }
            }

            public T Value
            {
                get { return this.value; }
                set { this.value = value; }
            }
        }

        public class CheckBoxData : PropertyData<bool>
        {
            public CheckBoxData(string text, string tooltip, bool value) : this(new GUIContent(text, tooltip), value,
                null)
            {
            }

            public CheckBoxData(GUIContent content, bool value) : this(content, value, null)
            {
            }

            public CheckBoxData(GUIContent content, bool value, GUILayoutOption[] options) : base(content, value,
                options)
            {
            }
        }

        public class CheckBoxData<T> : PropertyData<bool>
        {
            protected T tag;

            public CheckBoxData(string text, string tooltip, bool value, T tag) : this(new GUIContent(text, tooltip),
                value, tag, null)
            {
            }

            public CheckBoxData(GUIContent content, bool value, T tag) : this(content, value, tag, null)
            {
            }

            public CheckBoxData(GUIContent content, bool value, T tag, GUILayoutOption[] options) : base(content, value,
                options)
            {
                this.tag = tag;
            }

            public virtual T Tag
            {
                get { return this.tag; }
                set { this.tag = value; }
            }
        }
    }
}
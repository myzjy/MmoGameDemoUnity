using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace GameDataEditor.Common
{
    public abstract class ImgComPressEditor
    {
        [MenuItem("Tools/修改图片图集压缩方式/图片")]
        private static void ShowPngWindow()
        {
            PngCompressWindow window = EditorWindow.GetWindow<PngCompressWindow>(title: "图片压缩");
            window.Show();
        }

        [MenuItem("Tools/修改图片图集压缩方式/图集")]
        private static void ShowSpriteAtlasWindow()
        {
            SpriteAtlasCompressWindow window = EditorWindow.GetWindow<SpriteAtlasCompressWindow>(title: "图集压缩");
            window.Show();
        }
    }


    public class CompressWindowBase : EditorWindow
    {
        protected List<string> allFiles;

        protected TextureImporterPlatformSettings AndroidETC2Setting = new TextureImporterPlatformSettings()
        {
            name = "Android",
            overridden = true,
            compressionQuality = 50,
            androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings,
            format = TextureImporterFormat.ETC2_RGBA8,
        };

        protected TextureImporterPlatformSettings AndroidASTC4Setting = new TextureImporterPlatformSettings()
        {
            name = "Android",
            overridden = true,
            compressionQuality = 50,
            androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings,
            format = TextureImporterFormat.ASTC_4x4,
        };

        protected TextureImporterPlatformSettings AndroidNoSetting = new TextureImporterPlatformSettings()
        {
            name = "Android",
            overridden = false,
            //compressionQuality = 50,
            //androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings,
            //format = TextureImporterFormat.RGBA32,
        };

        protected TextureImporterPlatformSettings IOSNoSetting = new TextureImporterPlatformSettings()
        {
            name = "iPhone",
            overridden = false,
            //compressionQuality = 50,
            //androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings,
            //format = TextureImporterFormat.RGBA32,
        };

        protected TextureImporterPlatformSettings IOSASTC4Setting = new TextureImporterPlatformSettings()
        {
            name = "iPhone",
            overridden = true,
            compressionQuality = 50,
            format = TextureImporterFormat.ASTC_4x4,
        };

        public GUIStyle titleStyle;

        public GUIStyle descStyle;

        public GUIStyle boxCenterStyle;

        protected float windowWidth;

        private Vector2 scrollViewPos = new Vector2();

        protected int androidEct2CompressQuality = 1;

        protected int androidAstc4CompressQuality = 1;

        protected int iosAstc4CompressQuality = 1;

        protected string[] CompressQualityDesc = new string[]
        {
            "0", "50", "100"
        };

        protected int[] CompressQualityOpetion = new int[]
        {
            0, 50, 100
        };

        protected int SetCount;

        private string setCountString;

        private string[] IgnoreDirArr;

        private string IgnoreDirString = "Editor;Plugins;Android;IOS;Ios";

        protected bool android = false;

        protected bool ios = false;

        protected bool AtlasSpriteCom = false;

        protected int settedCount = 0;

        protected string startPath = "";

        protected float MaxWidth
        {
            get { return windowWidth * 0.95f; }
        }

        protected GUILayoutOption MaxWidthLayout
        {
            get { return GUILayout.Width(MaxWidth); }
        }

        private void OnEnable()
        {
            titleStyle = new GUIStyle()
            {
                fixedHeight = 40,
                fontSize = 30,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleCenter,
                normal = new GUIStyleState() { textColor = Color.white },
                padding = new RectOffset(10, 10, 0, 0),
            };

            descStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(10, 10, 0, 0),
                wordWrap = true,
                normal = new GUIStyleState() { textColor = Color.white }
            };

            boxCenterStyle = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
            };
        }


        protected virtual void OnGUI()
        {
            windowWidth = this.position.size.x;
            scrollViewPos =
                GUILayout.BeginScrollView(scrollViewPos, alwaysShowHorizontal: false, alwaysShowVertical: false);
            DrawAndroidTitle();
            Space(10);
            DrawAndroidContent();
            Space(10);
            DrawIOSTitle();
            Space(10);
            DrawIOSContent();
            Space(10);
            DrawCommonContent();
            GUILayout.EndScrollView();
        }

        protected void Space(float pixels)
        {
            GUILayout.Space(pixels);
        }


        protected void DrawAndroidTitle()
        {
            GUILayout.BeginVertical(GUI.skin.textArea, MaxWidthLayout);
            GUILayout.Label("Android设置", titleStyle, MaxWidthLayout);
            Space(5);
            GUILayout.Label("android压缩方式有两种,Etc2与Astc4,前者兼容性好，后者压缩后展示效果好，图片长宽不是4的倍数时，只能选用后者压缩方式", descStyle,
                MaxWidthLayout);
            GUILayout.EndVertical();
        }

        protected virtual void DrawAndroidContent()
        {
        }

        protected void DrawIOSTitle()
        {
            GUILayout.BeginVertical(GUI.skin.textArea, MaxWidthLayout);
            GUILayout.Label("IOS设置", titleStyle, MaxWidthLayout);
            Space(5);
            GUILayout.Label("ios压缩方式用Astc4", descStyle, MaxWidthLayout);
            GUILayout.EndVertical();
        }

        protected virtual void DrawIOSContent()
        {
            GUILayout.BeginVertical(GUI.skin.box, MaxWidthLayout);
            using (new CustomScope())
            {
                GUILayout.BeginVertical();
                GUILayout.Label("IOS设置(只显示可能更改的设置):");

                GUILayout.BeginHorizontal();
                GUILayout.Label("CompressQuality:");

                IOSASTC4Setting.compressionQuality = EditorGUILayout.IntPopup(IOSASTC4Setting.compressionQuality,
                    CompressQualityDesc, CompressQualityOpetion, GUILayout.Width(200));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("CompressFormat: ");
                IOSASTC4Setting.format =
                    (TextureImporterFormat)EditorGUILayout.EnumPopup(IOSASTC4Setting.format, GUILayout.Width(200));
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }

        protected void DrawCommonContent()
        {
            GUILayout.BeginVertical(GUI.skin.textArea, MaxWidthLayout);
            GUILayout.Label("通用设置", titleStyle, MaxWidthLayout);
            Space(5);

            GUILayout.BeginHorizontal(GUI.skin.box, MaxWidthLayout);
            GUILayout.Label("本次修改的最大数量，修改到该数量时则停止，设置为0表示无限制");
            setCountString = GUILayout.TextField(setCountString);
            int.TryParse(setCountString, out SetCount);
            GUILayout.EndHorizontal();

            Space(5);

            GUILayout.BeginHorizontal(GUI.skin.box, MaxWidthLayout);
            GUILayout.Label("过滤的文件夹，含以下名称的文件夹过滤掉，以;隔开");
            IgnoreDirString = GUILayout.TextField(IgnoreDirString);
            GUILayout.EndHorizontal();

            Space(5);

            GUILayout.BeginHorizontal(GUI.skin.box, MaxWidthLayout);
            GUILayout.Label("目标文件夹，只处理此文件夹的资源,正斜/");
            startPath = GUILayout.TextField(startPath);
            GUILayout.EndHorizontal();

            Space(5);

            GUILayout.BeginHorizontal(GUI.skin.box, MaxWidthLayout);
            GUILayout.Label("选择要操作的平台");
            android = GUILayout.Toggle(android, "Android");
            Space(30);
            ios = GUILayout.Toggle(ios, "IOS");
            GUILayout.EndHorizontal();

            Space(5);

            GUILayout.BeginHorizontal(GUI.skin.box, MaxWidthLayout);
            GUILayout.Label("打进图集的图片二次压缩(默认不选择，多次压缩失真严重)");
            AtlasSpriteCom = GUILayout.Toggle(AtlasSpriteCom, "二次压缩");
            GUILayout.EndHorizontal();

            Space(5);
            if (GUILayout.Button("开始压缩", MaxWidthLayout, GUILayout.Height(50)))
            {
                GetIgnoreString();
                if (!android && !ios)
                {
                    EditorUtility.DisplayDialog("错误", "没有选择要操作的平台!!!", "确定");
                }
                else if (EditorUtility.DisplayDialog("提示", $"最后的检查\n" +
                                                           $"Android4*4(ETC2)选择压缩格式为  {AndroidETC2Setting.format}" +
                                                           $",压缩质量为  {AndroidETC2Setting.compressionQuality}\n" +
                                                           $"Android非4*4(ASTC4)选择压缩格式为  {AndroidASTC4Setting.format}" +
                                                           $",压缩质量为  {AndroidASTC4Setting.compressionQuality}\n" +
                                                           $"IOS压缩格式为  {IOSASTC4Setting.format}" +
                                                           $",压缩质量为  {IOSASTC4Setting.compressionQuality}\n" +
                                                           $"图集图片二次压缩  {(AtlasSpriteCom ? "是" : "否")}\n" +
                                                           $"过滤以下文件夹  {GetIgnoreString()}\n" +
                                                           $"目标文件夹   {(string.IsNullOrEmpty(startPath) ? "全局" : startPath)}"
                             , "确定", "取消"))
                {
                    Opetion();
                }
            }

            GUILayout.EndVertical();
        }

        private string GetIgnoreString()
        {
            string tmp = string.Empty;
            IgnoreDirArr = IgnoreDirString.Split(';');
            for (int i = 0; i < IgnoreDirArr.Length; i++)
            {
                tmp = $"{tmp}{(tmp.Length == 0 ? "" : ",")}{IgnoreDirArr[i]}";
            }

            return tmp;
        }

        protected virtual void Opetion()
        {
            settedCount = 0;
            EditorUtility.ClearProgressBar();
            Debug.Log(DateTime.Now);
        }


        protected void GetFiles(string path, string ext, List<string> values)
        {
            string[] guids = AssetDatabase.FindAssets(ext);
            int length = guids.Length;

            for (int i = 0; i < length; i++)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(guids[i]); //Assets/***/****/***.*
                if (!string.IsNullOrEmpty(path) && !filePath.StartsWith(path))
                {
                    continue;
                }

                bool ignore = false;
                for (int j = 0; j < IgnoreDirArr.Length; j++)
                {
                    if (filePath.Contains(IgnoreDirArr[j]))
                    {
                        ignore = true;
                        break;
                    }
                }

                if (!ignore)
                {
                    values.Add(filePath);
                }
            }
        }
    }

    public class CustomScope : GUI.Scope
    {
        public CustomScope() : base()
        {
            //GUI.backgroundColor = Color.gray;
        }

        protected override void CloseScope()
        {
        }
    }

    public class PngCompressWindow : CompressWindowBase
    {
        private List<string> AllFiles = new List<string>();

        protected override void OnGUI()
        {
            base.OnGUI();
        }

        protected override void Opetion()
        {
            base.Opetion();
            AllFiles = new List<string>();
            GetFiles(startPath, "t:Texture", AllFiles);
            bool jump = false;
            for (int i = 0; i < AllFiles.Count && (SetCount == 0 ? true : settedCount < SetCount); i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar("压缩中", $"第{settedCount}张，进度显示没用", 0.5f))
                {
                    jump = true;
                }

                string filePath = AllFiles[i];
                bool change = false;
                try
                {
                    var obj = AssetDatabase.LoadAssetAtPath<Texture>(filePath);
                    AssetImporter assetImporter = AssetImporter.GetAtPath(filePath);
                    //在图集中，且不二次压缩,直接跳过
                    if (!AtlasSpriteCom && !string.IsNullOrEmpty(assetImporter.assetBundleName) &&
                        assetImporter.assetBundleName.EndsWith("_spriteatlas.ab"))
                    {
                        if (android)
                        {
                            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(filePath);
                            TextureImporterPlatformSettings AndroidSettings = AndroidNoSetting;
                            AndroidSettings.maxTextureSize = textureImporter.maxTextureSize;
                            var setting = textureImporter.GetPlatformTextureSettings("Android");

                            if (setting.overridden == AndroidSettings.overridden)
                            {
                            }
                            else
                            {
                                textureImporter.SetPlatformTextureSettings(AndroidSettings);
                                textureImporter.SaveAndReimport();
                                change = true;
                            }
                        }

                        if (ios)
                        {
                            TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(filePath);
                            TextureImporterPlatformSettings IOSSetting = IOSNoSetting;
                            IOSSetting.maxTextureSize = textureImporter.maxTextureSize;

                            var setting = textureImporter.GetPlatformTextureSettings("IOS");

                            if (setting.overridden == IOSSetting.overridden)
                            {
                            }
                            else
                            {
                                textureImporter.SetPlatformTextureSettings(IOSSetting);
                                textureImporter.SaveAndReimport();
                                change = true;
                            }
                        }

                        if (change)
                        {
                            settedCount++;
                            Debug.Log($"filePath:{filePath}");
                        }

                        continue;
                    }

                    if (android)
                    {
                        TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(filePath);
                        TextureImporterPlatformSettings AndroidSettings = null;
                        if (obj.width % 4 != 0 || obj.height % 4 != 0)
                        {
                            AndroidSettings = AndroidASTC4Setting;
                        }
                        else
                        {
                            AndroidSettings = AndroidETC2Setting;
                        }

                        AndroidSettings.maxTextureSize = textureImporter.maxTextureSize;
                        var setting = textureImporter.GetPlatformTextureSettings("Android");

                        if (setting.overridden == AndroidSettings.overridden &&
                            setting.format == AndroidSettings.format &&
                            setting.compressionQuality == AndroidSettings.compressionQuality)
                        {
                        }
                        else
                        {
                            textureImporter.SetPlatformTextureSettings(AndroidSettings);
                            textureImporter.SaveAndReimport();
                            change = true;
                        }
                    }

                    if (ios)
                    {
                        TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(filePath);
                        TextureImporterPlatformSettings IOSSetting = IOSASTC4Setting;
                        IOSSetting.maxTextureSize = textureImporter.maxTextureSize;

                        var setting = textureImporter.GetPlatformTextureSettings("IOS");

                        if (setting.overridden == IOSSetting.overridden && setting.format == IOSSetting.format &&
                            setting.compressionQuality == IOSSetting.compressionQuality)
                        {
                        }
                        else
                        {
                            textureImporter.SetPlatformTextureSettings(IOSSetting);
                            textureImporter.SaveAndReimport();
                            change = true;
                        }
                    }

                    if (change)
                    {
                        settedCount++;
                        Debug.Log($"filePath:{filePath}");
                    }

                    if (jump)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"{filePath}:{e.Message}");
                }
            }

            EditorUtility.ClearProgressBar();
            Debug.Log(DateTime.Now);
            Debug.Log($"图片操作完成：{settedCount}");
            AssetDatabase.Refresh();
        }

        protected override void DrawAndroidContent()
        {
            base.DrawAndroidContent();
            GUILayout.BeginVertical(GUI.skin.box, MaxWidthLayout);
            using (new CustomScope())
            {
                GUILayout.BeginVertical();
                GUILayout.Label("4*4尺寸贴图压缩模式(Etc2)设置(只显示可能更改的设置):");

                GUILayout.BeginHorizontal();
                GUILayout.Label("CompressQuality:");

                AndroidETC2Setting.compressionQuality = EditorGUILayout.IntPopup(AndroidETC2Setting.compressionQuality,
                    CompressQualityDesc, CompressQualityOpetion, GUILayout.Width(200));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("CompressFormat: ");
                AndroidETC2Setting.format =
                    (TextureImporterFormat)EditorGUILayout.EnumPopup(AndroidETC2Setting.format, GUILayout.Width(200));
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            Space(10);
            GUILayout.Label("=============分割线=============", descStyle, MaxWidthLayout);
            Space(10);

            using (new CustomScope())
            {
                GUILayout.BeginVertical();
                GUILayout.Label("非4*4尺寸贴图压缩模式(ASTC4)设置(只显示可能更改的设置):");

                GUILayout.BeginHorizontal();
                GUILayout.Label("CompressQuality:");

                AndroidASTC4Setting.compressionQuality = EditorGUILayout.IntPopup(
                    AndroidASTC4Setting.compressionQuality, CompressQualityDesc, CompressQualityOpetion,
                    GUILayout.Width(200));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("CompressFormat: ");
                AndroidASTC4Setting.format =
                    (TextureImporterFormat)EditorGUILayout.EnumPopup(AndroidASTC4Setting.format, GUILayout.Width(200));
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }
    }

    public class SpriteAtlasCompressWindow : CompressWindowBase
    {
        private List<string> AllFiles = new List<string>();

        protected override void OnGUI()
        {
            base.OnGUI();
        }

        protected override void Opetion()
        {
            base.Opetion();
            AllFiles = new List<string>();
            GetFiles(startPath, "t:Spriteatlas", AllFiles);

            bool jump = false;
            for (int i = 0; i < AllFiles.Count && (SetCount == 0 ? true : settedCount < SetCount); i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar("压缩中", $"第{settedCount}张，进度显示没用", 0.5f))
                {
                    jump = true;
                }

                string filePath = AllFiles[i];
                try
                {
                    var obj = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(filePath);

                    if (obj == null)
                    {
                        Debug.LogError($"{filePath} data is null");
                        continue;
                    }

                    int maxSize = obj.GetPlatformSettings("Android").maxTextureSize;

                    if (android)
                    {
                        TextureImporterPlatformSettings settings = AndroidETC2Setting;
                        settings.maxTextureSize = maxSize;
                        obj.SetPlatformSettings(settings);
                    }

                    if (ios)
                    {
                        TextureImporterPlatformSettings settings = IOSASTC4Setting;
                        settings.maxTextureSize = maxSize;
                        obj.SetPlatformSettings(settings);
                    }

                    settedCount++;
                    Debug.Log(filePath);

                    if (jump)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"{filePath}:{e.Message}");
                }
            }

            EditorUtility.ClearProgressBar();
            Debug.Log(DateTime.Now);
            Debug.Log("图集操作完成");
            AssetDatabase.Refresh();
        }


        protected override void DrawAndroidContent()
        {
            base.DrawAndroidContent();
            GUILayout.BeginVertical(GUI.skin.box, MaxWidthLayout);
            using (new CustomScope())
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Etc2设置(只显示可能更改的设置):");

                GUILayout.BeginHorizontal();
                GUILayout.Label("CompressQuality:");

                AndroidETC2Setting.compressionQuality = EditorGUILayout.IntPopup(AndroidETC2Setting.compressionQuality,
                    CompressQualityDesc, CompressQualityOpetion, GUILayout.Width(200));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("CompressFormat: ");
                AndroidETC2Setting.format =
                    (TextureImporterFormat)EditorGUILayout.EnumPopup(AndroidETC2Setting.format, GUILayout.Width(200));
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }
    }

    public class SpriteUtilEditor : Editor
    {
        private static readonly string AssetUIDataPath = $"{Application.dataPath}/Game/UI/";
        private static string startWith = "Assets/Game/AssetBundles/UI/Sprites";

        private const string IgnoreDirString = "Editor;Plugins;Android;IOS;Ios";

        //特定文件夹
        private const string SpriteDirString = "";
        private static string extString = "t:Texture";
        private const string SPRITE_SETTINGS_EDITOR = "Tools/GameEditor/SpriteDefault";
        private static int SettedCount = 0;

        private TextureImporterSettings SpriteTextureImporterSettings = new TextureImporterSettings()
        {
            textureType = TextureImporterType.Sprite
        };

        #region 安卓sprite

        //安卓sprite图片设置压缩格式
        private static readonly TextureImporterPlatformSettings AndroidASTC4Setting =
            new TextureImporterPlatformSettings()
            {
                name = "Android",

                overridden = true,
                //0-->fast
                //50--> normal
                //100-->best
                compressionQuality = 100,
                androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings,
                format = TextureImporterFormat.ASTC_4x4,
            };

        private static readonly TextureImporterPlatformSettings AndroidETC2Setting =
            new TextureImporterPlatformSettings()
            {
                name = "Android",
                overridden = true,
                //0-->fast
                //50--> normal
                //100-->best
                compressionQuality = 100,
                androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings,
                format = TextureImporterFormat.ETC2_RGBA8,
            };

        protected static TextureImporterPlatformSettings AndroidNoSetting = new TextureImporterPlatformSettings()
        {
            name = "Android",
            overridden = false,
            //compressionQuality = 50,
            //androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings,
            //format = TextureImporterFormat.RGBA32,
        };

        #endregion

        #region IOS Sprite

        protected static TextureImporterPlatformSettings IOSNoSetting = new TextureImporterPlatformSettings()
        {
            name = "iPhone",
            overridden = false,
            //compressionQuality = 50,
            //androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings,
            //format = TextureImporterFormat.RGBA32,
        };

        private static readonly TextureImporterPlatformSettings IOSASTC4Setting = new TextureImporterPlatformSettings()
        {
            name = "iPhone",
            overridden = true,
            //0-->fast
            //50--> normal
            //100-->best
            compressionQuality = 100,
            format = TextureImporterFormat.ASTC_4x4,
        };

        #endregion

        [MenuItem(SPRITE_SETTINGS_EDITOR)]
        public static void SpriteEditor()
        {
            var pathList = new List<string>();
            var strs = IgnoreDirString.Split(';');
            GetFiles(strs, ref pathList);
            bool jump = false;
            for (int i = 0; i < pathList.Count; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar("设置图片", $"第{SettedCount}张，进度显示没用",
                        ((i * 1.0f) / pathList.Count * 1.0f)))
                {
                    jump = true;
                }

                var file = pathList[i];
                bool isChange = false;
                var obj = AssetDatabase.LoadAssetAtPath<Texture>(file);
                try
                {
                    #region Android

                    {
                        //获取
                        TextureImporter _testureImp = (TextureImporter)AssetImporter.GetAtPath(file);


                        TextureImporterPlatformSettings androidSettings = null;
                        if (obj.width % 4 != 0 || obj.height % 4 != 0)
                        {
                            androidSettings = AndroidASTC4Setting;
                        }
                        else
                        {
                            androidSettings = AndroidETC2Setting;
                        }

                        _testureImp.textureType = TextureImporterType.Sprite;
                        _testureImp.spriteImportMode = SpriteImportMode.Single;
                        androidSettings.maxTextureSize = _testureImp.maxTextureSize;
                        var setting = _testureImp.GetPlatformTextureSettings("Android");
                        //进行设置
                        if (setting.overridden == androidSettings.overridden &&
                            setting.format == androidSettings.format &&
                            setting.compressionQuality == androidSettings.compressionQuality)
                        {
                        }
                        else
                        {
                            
                            _testureImp.SetPlatformTextureSettings(androidSettings);

                            _testureImp.SaveAndReimport();
                            isChange = true;
                        }
                    }

                    #endregion

                    #region IOS

                    {
                        TextureImporter _testureImp = (TextureImporter)AssetImporter.GetAtPath(file);
                        TextureImporterPlatformSettings IOSSetting = IOSASTC4Setting;
                        _testureImp.textureType = TextureImporterType.Sprite;
                        _testureImp.spriteImportMode = SpriteImportMode.Single;
                        IOSSetting.maxTextureSize = _testureImp.maxTextureSize;

                        var setting = _testureImp.GetPlatformTextureSettings("IOS");

                        if (setting.overridden == IOSSetting.overridden && setting.format == IOSSetting.format &&
                            setting.compressionQuality == IOSSetting.compressionQuality)
                        {
                        }
                        else
                        {
                            _testureImp.SetPlatformTextureSettings(IOSSetting);
                            _testureImp.SaveAndReimport();
                            isChange = true;
                        }
                    }

                    #endregion

                    if (isChange)
                    {
                        SettedCount++;
                        UnityEngine.Debug.Log(file, obj);
                    }

                    if (jump)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log($"{file}:{e.Message}");
                }
            }

            EditorUtility.ClearProgressBar();
            UnityEngine.Debug.Log(DateTime.Now);
            UnityEngine.Debug.Log($"图片操作完成：{SettedCount}");
            AssetDatabase.Refresh();
        }

        public static void GetFiles(string[] ignores, ref List<string> paths)
        {
            //拿到所有
            string[] guids = AssetDatabase.FindAssets(extString);
            int length = guids.Length;
            for (int i = 0; i < length; i++)
            {
                string filePath = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (!string.IsNullOrEmpty(filePath) && !filePath.StartsWith(startWith))
                {
                    continue;
                }

                bool ignore = ignores.Any(t => filePath != null && filePath.Contains(t));

                if (!ignore)
                {
                    paths.Add(filePath);
                }
            }
        }


        [MenuItem("Assets/SpriteSlicer/SpriteSlice")]
        public static void SpliceSprite()
        {
            Texture2D inputImage = Selection.activeObject as Texture2D;
            Texture2D outputImage = GetChildTexture(inputImage);
            Debug.Log(outputImage);
            Sprite outputImage2 = GetChildSprite(inputImage);
            Debug.Log(outputImage2);
        }

        private static Sprite GetChildSprite(Object image)
        {
            string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(image)); //获取路径名称  
            string path = $"{rootPath}/{image.name}.png"; //图片路径名称

            UnityEngine.Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path);
            //Object[] sprites = Resources.LoadAll<Sprite>(path);


            Sprite outputSprite = (Sprite)sprites[^1];

            Debug.Log(outputSprite.name);

            return outputSprite;
        }

        private static Texture2D GetChildTexture(Texture2D image)
        {
            /*
             * 存放所有转化的png 路径
             */
            var paths = new List<string>();

            /*
             * 获取路径
             */
            string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(image));
            /*
             * 图片路径名称
             */
            string path = $"{rootPath}/{image.name}.png";

            /*
             * 获取图片入口
             */
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            AssetDatabase.CreateFolder(rootPath, image.name);
            if (textureImporter != null)
            {
                foreach (SpriteMetaData metaData in textureImporter.spritesheet) //遍历小图集  
                {
                    Texture2D myImage = new Texture2D((int)metaData.rect.width, (int)metaData.rect.height);


                    //abc_0:(x:2.00, y:400.00, width:103.00, height:112.00)  
                    for (int y = (int)metaData.rect.y; y < metaData.rect.y + metaData.rect.height; y++) //Y轴像素  
                    {
                        for (int x = (int)metaData.rect.x; x < metaData.rect.x + metaData.rect.width; x++)
                        {
                            myImage.SetPixel(x - (int)metaData.rect.x, y - (int)metaData.rect.y, image.GetPixel(x, y));
                        }
                    }


                    //转换纹理到EncodeToPNG兼容格式  
                    if (myImage.format != TextureFormat.ARGB32 && myImage.format != TextureFormat.RGB24)
                    {
                        Texture2D newTexture = new Texture2D(myImage.width, myImage.height);
                        newTexture.SetPixels(myImage.GetPixels(0), 0);
                        myImage = newTexture;
                    }

                    var pngData = myImage.EncodeToPNG();
                    string outputPath = $"{rootPath}/{image.name}/{metaData.name}.png"; //子图片输出路径
                    File.WriteAllBytes(outputPath, pngData); //输出子PNG图片

                    paths.Add(outputPath);

                    // 刷新资源窗口界面  
                    AssetDatabase.Refresh();
                }
            }

            Texture2D outputImage = AssetDatabase.LoadAssetAtPath<Texture2D>(paths[^1]);
            Debug.Log(outputImage.name);

            return outputImage;
        }
    }
}
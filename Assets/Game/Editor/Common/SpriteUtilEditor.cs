using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameDataEditor.Common
{
    public class SpriteUtilEditor : Editor
    {
        private static readonly string AssetUIDataPath = $"{Application.dataPath}/Game/UI/";
        private static string startWith = "Assets/Game/UI";

        private const string IgnoreDirString = "Editor;Plugins;Android;IOS;Ios";

        //特定文件夹
        private const string SpriteDirString = "";
        private static string extString = "t:Texture";
        private const string SPRITE_SETTINGS_EDITOR = "Tool/GameEditor/SpriteDefault";
        private static int SettedCount = 0;

        private TextureImporterSettings SpriteTextureImporterSettings = new TextureImporterSettings()
        {
            textureType = TextureImporterType.Sprite
        };

        #region 安卓sprite

        //安卓sprite图片设置压缩格式
        private static readonly TextureImporterPlatformSettings AndroidASTC4Setting = new TextureImporterPlatformSettings()
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

        private static readonly TextureImporterPlatformSettings AndroidETC2Setting = new TextureImporterPlatformSettings()
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
                if (EditorUtility.DisplayCancelableProgressBar("设置图片", $"第{SettedCount}张，进度显示没用", ((i*1.0f)/pathList.Count*1.0f)))
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
                        TextureImporter _testureImp = (TextureImporter) AssetImporter.GetAtPath(file);

                        
                        TextureImporterPlatformSettings androidSettings = null;
                        if (obj.width % 4 != 0 || obj.height % 4 != 0)
                        {
                            androidSettings = AndroidASTC4Setting;
                        }
                        else
                        {
                            androidSettings = AndroidETC2Setting;
                        }

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
                        TextureImporter _testureImp = (TextureImporter) AssetImporter.GetAtPath(file);
                        TextureImporterPlatformSettings IOSSetting = IOSASTC4Setting;
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
                        Debug.Log(file, obj);
                    }

                    if (jump)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"{file}:{e.Message}");
                }
            }

            EditorUtility.ClearProgressBar();
            Debug.Log(DateTime.Now);
            Debug.Log($"图片操作完成：{SettedCount}");
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

                bool isgnore = ignores.Any(t => filePath != null && filePath.Contains(t));

                if (!isgnore)
                {
                    paths.Add(filePath);
                }
            }
        }
    }
}
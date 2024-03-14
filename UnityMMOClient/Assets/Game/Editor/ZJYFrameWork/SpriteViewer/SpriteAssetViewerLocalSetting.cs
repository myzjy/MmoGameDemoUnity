using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZJYFrameWork.SpriteViewer
{
    internal partial class SpriteAssetViewerSetting
    {
        public class SpriteAssetViewerLocalSetting : ISerializationCallbackReceiver
        {
            public const string LocalSettingSaveKey = "[SpritesViewer]";

            public string[] favoriteSpriteAtlasPath = new string[0];

            public static SpriteAssetViewerLocalSetting GetLocalSettings()
            {
                SpriteAssetViewerLocalSetting targetSettings = LoadPrefs();
                if (targetSettings == null)
                {
                    targetSettings = new SpriteAssetViewerLocalSetting();
                    SaveSetting(targetSettings);
                }

                return targetSettings;
            }

            public void OnAfterDeserialize()
            {
                return;
            }

            public void OnBeforeSerialize()
            {
                return;
            }

            /// <summary>
            /// Save preferences in EditorPrefs
            /// </summary>
            public static void SaveSetting(SpriteAssetViewerLocalSetting settings)
            {
                EditorPrefs.SetString(LocalSettingSaveKey, JsonUtility.ToJson(settings));
            }

            /// <summary> 
            /// Load prefs if they exist. Create if they don't 
            /// </summary>
            private static SpriteAssetViewerLocalSetting LoadPrefs()
            {
                // Create settings if it doesn't exist
                if (!EditorPrefs.HasKey(LocalSettingSaveKey))
                {
                    EditorPrefs.SetString(LocalSettingSaveKey, JsonUtility.ToJson(new SpriteAssetViewerLocalSetting()));
                }

                return JsonUtility.FromJson<SpriteAssetViewerLocalSetting>(EditorPrefs.GetString(LocalSettingSaveKey));
            }

            /// <summary>
            /// Reset prefs
            /// </summary>
            private static void ResetPrefs()
            {
                if (EditorPrefs.HasKey(LocalSettingSaveKey))
                {
                    EditorPrefs.DeleteKey(LocalSettingSaveKey);
                }
            }

            public void AddFavoriteSpriteAtlas(string path)
            {
                if (favoriteSpriteAtlasPath == null)
                {
                    favoriteSpriteAtlasPath = new string[1];
                    favoriteSpriteAtlasPath[0] = path;
                }
                else
                {
                    HashSet<string> newPathSet = new HashSet<string>(favoriteSpriteAtlasPath) { path };

                    string[] stringArray = new string[newPathSet.Count];
                    newPathSet.CopyTo(stringArray);
                    favoriteSpriteAtlasPath = stringArray;
                }

                SaveSetting(this);
            }

            public void RemoveFavoriteSpriteAtlas(string path)
            {
                if (favoriteSpriteAtlasPath == null)
                {
                    return;
                }

                HashSet<string> newPathSet = new HashSet<string>(favoriteSpriteAtlasPath);
                newPathSet.Remove(path);

                string[] stringArray = new string[newPathSet.Count];
                newPathSet.CopyTo(stringArray);

                favoriteSpriteAtlasPath = stringArray;
                SaveSetting(this);
            }

            public void ClearFavoriteSpriteAtlas()
            {
                if (favoriteSpriteAtlasPath != null)
                {
                    favoriteSpriteAtlasPath = Array.Empty<string>();
                }

                SaveSetting(this);
            }
        }
    }
}
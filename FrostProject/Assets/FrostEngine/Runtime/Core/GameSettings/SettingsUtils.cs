using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrostEngine
{
    public class SettingsUtils
    {
        private static readonly string GlobalSettingsPath = $"FrostEngineGlobalSettings";
        private static FrostEngineSettings _globalSettings;

        public static FrostEngineSettings GlobalSettings
        {
            get
            {
                if (_globalSettings == null)
                {
                    _globalSettings = GetSingletonAssetsByResources<FrostEngineSettings>(GlobalSettingsPath);
                }

                return _globalSettings;
            }
        }
        public static HybridCLRCustomGlobalSettings HybridCLRCustomGlobalSettings => GlobalSettings.BybridCLRCustomGlobalSettings;
        public static FrameworkGlobalSettings FrameworkGlobalSettings => GlobalSettings.FrameworkGlobalSettings;


        public static void SetHybridCLRHotUpdateAssemblies(List<string> hotUpdateAssemblies)
        {
            HybridCLRCustomGlobalSettings.HotUpdateAssemblies.Clear();
            HybridCLRCustomGlobalSettings.HotUpdateAssemblies.AddRange(hotUpdateAssemblies);
        }
        
        public static void SetHybridCLRAOTMetaAssemblies(List<string> aOTMetaAssemblies)
        {
            HybridCLRCustomGlobalSettings.AOTMetaAssemblies.Clear();
            HybridCLRCustomGlobalSettings.AOTMetaAssemblies.AddRange(aOTMetaAssemblies);
        }
        
        public static bool EnableUpdateData()
        {
            return FrameworkGlobalSettings.EnableUpdateData;
        }
        
        public static string GetUpdateDataUrl()
        {
            string url = null;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            url = FrameworkGlobalSettings.WindowsUpdateDataUrl;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            url = FrameworkGlobalSettings.MacOSUpdateDataUrl;
#elif UNITY_IOS
            url = FrameworkGlobalSettings.IOSUpdateDataUrl;
#elif UNITY_ANDROID
            url = FrameworkGlobalSettings.AndroidUpdateDataUrl;
#elif UNITY_WEBGL
            url = FrameworkGlobalSettings.WebGLUpdateDataUrl;
#endif
            return url;
        }
        
        private static T GetSingletonAssetsByResources<T>(string assetsPath) where T : ScriptableObject, new()
        {
            string assetType = typeof(T).Name;
#if UNITY_EDITOR
            string[] globalAssetPaths = UnityEditor.AssetDatabase.FindAssets($"t:{assetType}");
            if (globalAssetPaths.Length > 1)
            {
                foreach (var assetPath in globalAssetPaths)
                {
                    Debug.LogError($"Could not had Multiple {assetType}. Repeated Path: {UnityEditor.AssetDatabase.GUIDToAssetPath(assetPath)}");
                }

                throw new Exception($"Could not had Multiple {assetType}");
            }
#endif
            T customGlobalSettings = Resources.Load<T>(assetsPath);
            if (customGlobalSettings == null)
            {
                Debug.LogError($"Could not found {assetType} asset，so auto create:{assetsPath}.");
                return null;
            }

            return customGlobalSettings;
        }
        public static List<ScriptGenerateRuler> GetScriptGenerateRule()
        {
            return FrameworkGlobalSettings.ScriptGenerateRule;
        }

        public static string GetUINameSpace()
        {
            return FrameworkGlobalSettings.NameSpace;
        }
    }
}
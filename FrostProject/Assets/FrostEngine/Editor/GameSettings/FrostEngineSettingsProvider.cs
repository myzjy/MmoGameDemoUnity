using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostEngine.Editor.GameSettings
{
    public class FrostEngineSettingsProvider : SettingsProvider
    {
        const string k_SettingsPath = "Assets/FrostEngine/ResRaw/Resources/FrostEngineGlobalSettings.asset";
        private const string headerName = "FrostEngine/FrostEngineSettings";
        private SerializedObject m_CustomSettings;

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(SettingsUtils.GlobalSettings);
        }

        public static bool IsSettingsAvailable()
        {
            return File.Exists(k_SettingsPath);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            m_CustomSettings = GetSerializedSettings();
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            SaveAssetData(k_SettingsPath);
        }

        void SaveAssetData(string path)
        {
            FrostEngineSettings old = AssetDatabase.LoadAssetAtPath<FrostEngineSettings>(k_SettingsPath);
            FrostEngineSettings data = ScriptableObject.CreateInstance<FrostEngineSettings>();
            data.Set(old.FrameworkGlobalSettings, old.BybridCLRCustomGlobalSettings);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);
            using var changeCheckScope = new EditorGUI.ChangeCheckScope();
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_FrameworkGlobalSettings"));

            if (GUILayout.Button("Refresh HotUpdateAssemblies"))
            {
                SyncAssemblyContent.RefreshAssembly();
                m_CustomSettings.ApplyModifiedPropertiesWithoutUndo();
                m_CustomSettings = null;
                m_CustomSettings = GetSerializedSettings();
            }

            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_HybridCLRCustomGlobalSettings"));
            EditorGUILayout.Space(20);
            if (!changeCheckScope.changed)
            {
                return;
            }

            m_CustomSettings.ApplyModifiedPropertiesWithoutUndo();
        }

        public FrostEngineSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(
            path, scopes, keywords)
        {
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingProvider()
        {
            if (IsSettingsAvailable())
            {
                var provider = new FrostEngineSettingsProvider(headerName, SettingsScope.Project);
                provider.keywords = GetSearchKeywordsFromGUIContentProperties<FrostEngineSettings>();
                return provider;
            }
            else
            {
                Debug.LogError(
                    $"Open FrostEngine Settings error,Please Create FrostEngine FrostEngineSettings.assets File in Path FrostEngine/ResRaw/Resources/");
            }

            return null;
        }
    }
}
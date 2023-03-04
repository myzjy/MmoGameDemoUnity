using UnityEditor;
using UnityEngine;

namespace ZJYFrameWork.SpriteViewer
{
    internal static class SpriteAssetViewerPreferences
    {
        public const string PreferenceSaveKey = "SpritesViewer";

        public const string PreferenceCategoryKey = "Tools/SpriteAssetViewer";

        private static SpriteAssetViewerSetting settings => SpriteAssetViewerSetting.GetSetting();


        [MenuItem(PreferenceCategoryKey)]

        private static void PreferencesGUI()
        {
            settings.Save(false);
            EditorGUI.BeginChangeCheck();
            {
                DrawSpritePackerSetting(settings);
                DrawChangeAssetSetting(settings);
                DrawSpriteAtlasSetting(settings);
                DrawCommonSetting(settings);
            }
            if (EditorGUI.EndChangeCheck())
            {
              
            }
        }

        private static void DrawSpritePackerSetting(SpriteAssetViewerSetting settings)
        {
            EditorGUILayout.LabelField("SpritePackerSetting : ");
            EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(EditorGUIUtility.singleLineHeight),
                GUILayout.ExpandWidth(true));
            {
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
        }

        private static void DrawSpriteAtlasSetting(SpriteAssetViewerSetting settings)
        {
            EditorGUILayout.LabelField("SpriteAtlasSetting : ");
            EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(EditorGUIUtility.singleLineHeight),
                GUILayout.ExpandWidth(true));
            {
                // Tight
                settings.GlobalSetting.atlasCheckTightPackMessageType =
                    (MessageType)EditorGUILayout.EnumPopup("Check Tight Setting",
                        settings.GlobalSetting.atlasCheckTightPackMessageType);
                if (settings.GlobalSetting.atlasCheckTightPackMessageType != MessageType.None)
                {
                    settings.GlobalSetting.atlasIsTightPack = EditorGUILayout.Toggle("Default Pack use Tight",
                        settings.GlobalSetting.atlasIsTightPack);
                }
                else
                {
                    if (settings.GlobalSetting.atlasCheckTightPackMessageType == MessageType.None &&
                        settings.GlobalSetting.atlasIsTightPack !=
                        SpriteAssetViewerGlobalSetting.AtlasDefaultTightPack)
                    {
                        settings.GlobalSetting.atlasIsTightPack =
                            SpriteAssetViewerGlobalSetting.AtlasDefaultTightPack;
                    }
                }

                GUILayout.Space(10f);

                // Compression
                settings.GlobalSetting.atlasCheckCompressionSettingMessageType =
                    (MessageType)EditorGUILayout.EnumPopup("Check Compression Setting",
                        settings.GlobalSetting.atlasCheckCompressionSettingMessageType);
                if (settings.GlobalSetting.atlasCheckCompressionSettingMessageType != MessageType.None)
                {
                    settings.GlobalSetting.atlasCompressionSetting =
                        (TextureImporterCompression)EditorGUILayout.EnumPopup("Default Compression",
                            settings.GlobalSetting.atlasCompressionSetting);
                }
                else
                {
                    if (settings.GlobalSetting.atlasCheckCompressionSettingMessageType == MessageType.None &&
                        settings.GlobalSetting.atlasCompressionSetting !=
                        SpriteAssetViewerGlobalSetting.AtlasDefaultCompressionSetting)
                    {
                        settings.GlobalSetting.atlasCompressionSetting =
                            SpriteAssetViewerGlobalSetting.AtlasDefaultCompressionSetting;
                    }
                }

                GUILayout.Space(10f);

                // Atlas AssetBundle
                settings.GlobalSetting.atlasCheckAddAtlasAbStringMessageType = (MessageType)EditorGUILayout.EnumPopup(
                    "Check SpriteAtlas AssetBundle Setting",
                    settings.GlobalSetting.atlasCheckAddAtlasAbStringMessageType);
                if (settings.GlobalSetting.atlasCheckAddAtlasAbStringMessageType != MessageType.None)
                {
                    bool beforeSetting = settings.GlobalSetting.atlasIsAddAtlasAbString;
                    settings.GlobalSetting.atlasIsAddAtlasAbString = EditorGUILayout.Toggle(
                        "Set SpriteAtlas as assetBundle ", settings.GlobalSetting.atlasIsAddAtlasAbString);
                    if (beforeSetting && !settings.GlobalSetting.atlasIsAddAtlasAbString)
                    {
                        settings.GlobalSetting.atlasIsAddAtlasContentsAbString = false;
                    }
                }

                GUILayout.Space(10f);

                // AtlasContents AssetBundle
                if (settings.GlobalSetting.atlasIsAddAtlasAbString)
                {
                    settings.GlobalSetting.atlasCheckAddAtlasContentsAbStringMessageType =
                        (MessageType)EditorGUILayout.EnumPopup("Check Contents AssetBundle Setting",
                            settings.GlobalSetting.atlasCheckAddAtlasContentsAbStringMessageType);
                    if (settings.GlobalSetting.atlasCheckAddAtlasContentsAbStringMessageType != MessageType.None)
                    {
                        settings.GlobalSetting.atlasIsAddAtlasContentsAbString = EditorGUILayout.Toggle(
                            "Set all contents into SpriteAtlasAssetBundle",
                            settings.GlobalSetting.atlasIsAddAtlasContentsAbString);
                    }
                }

                GUILayout.Space(10f);

                // SpriteAtlasSavePath
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.PrefixLabel("SpriteAtlasSavePath : ");
                    EditorGUILayout.LabelField(SpriteAssetViewerSetting.FolderBasePath, GUILayout.Width(48f));
                    string tempPath = EditorGUILayout.DelayedTextField(settings.GlobalSetting.spriteAtlasSubSavePath);
                    if (tempPath.EndsWith("/") || tempPath.EndsWith("\\"))
                    {
                        tempPath = tempPath.Substring(0, tempPath.Length - 1);
                    }

                    settings.GlobalSetting.spriteAtlasSubSavePath = tempPath;
                }
                EditorGUILayout.EndHorizontal();

                bool guiEnableBefore = GUI.enabled;
                GUI.enabled = false;
                {
                    EditorGUILayout.LabelField("LocalSpriteAtlasSavePath : ",
                        settings.GetFolderSavePath(SpriteAssetViewerSetting.FolderType.Local));
                    EditorGUILayout.LabelField("AbSpriteAtlasSavePath : ",
                        settings.GetFolderSavePath(SpriteAssetViewerSetting.FolderType.AssetBundle));
                }
                GUI.enabled = true;
                GUI.enabled = guiEnableBefore;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
        }

        private static void DrawChangeAssetSetting(SpriteAssetViewerSetting settings)
        {
            EditorGUILayout.LabelField("SpritePacker -> SpriteAtlas Setting : ");
            EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(EditorGUIUtility.singleLineHeight),
                GUILayout.ExpandWidth(true));
            {
                settings.GlobalSetting.Packer2Atlas_IsCheckoutTag =
                    EditorGUILayout.Toggle("Remove PackingTag after Change",
                        settings.GlobalSetting.Packer2Atlas_IsCheckoutTag);
                settings.GlobalSetting.Packer2Atlas_IsTightPack = EditorGUILayout.Toggle("Default Pack use Tight",
                    settings.GlobalSetting.Packer2Atlas_IsTightPack);
                settings.GlobalSetting.Packer2Atlas_CompressionSetting =
                    (TextureImporterCompression)EditorGUILayout.EnumPopup("Default Compression",
                        settings.GlobalSetting.Packer2Atlas_CompressionSetting);
                settings.GlobalSetting.packer2AtlasIsAddAtlasAbString = EditorGUILayout.Toggle(
                    "Set SpriteAtlas as assetBundle ", settings.GlobalSetting.packer2AtlasIsAddAtlasAbString);

                if (settings.GlobalSetting.packer2AtlasIsAddAtlasAbString)
                {
                    settings.GlobalSetting.packer2AtlasIsAddAtlasContentsAbString = EditorGUILayout.Toggle(
                        "Set all contents into SpriteAtlasAssetBundle",
                        settings.GlobalSetting.packer2AtlasIsAddAtlasContentsAbString);
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
        }

        private static void DrawCommonSetting(SpriteAssetViewerSetting settings)
        {
            EditorGUILayout.LabelField("CommonSetting : ");
            EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(EditorGUIUtility.singleLineHeight),
                GUILayout.ExpandWidth(true));
            {
                settings.GlobalSetting.assetBundleFileExtension =
                    EditorGUILayout.DelayedTextField("The assetbundle file extension : ",
                        settings.GlobalSetting.assetBundleFileExtension);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
        }
    }
}
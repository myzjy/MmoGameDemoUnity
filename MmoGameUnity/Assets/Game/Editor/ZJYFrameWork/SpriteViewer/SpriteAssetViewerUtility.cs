using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace ZJYFrameWork.SpriteViewer
{
    public class SpriteAssetViewerUtility
    {
        public const string MainHelpUrl = "REMOVED_URL";

        public const string PlusPolicyHelpUrl = "www.google.com";

        public const string DefaultTextureCompressionPlatformName = "DefaultTexturePlatform";

        public const string SpriteAtlasExtension = "spriteatlas";

        private static Styles styles;

        public class Styles
        {
            // Icon
            public readonly GUIContent HelpIcon = EditorGUIUtility.IconContent("_Help", "Help");
            public readonly GUIContent SettingIcon = EditorGUIUtility.IconContent("_Popup", "Setting");
            public readonly GUIContent EditIcon = EditorGUIUtility.IconContent("editicon.sml", "Edit Name");

            public readonly GUIContent SubWindowIcon =
                EditorGUIUtility.IconContent("d_LookDevSideBySide@2x", "Sub Window");

            public readonly GUIContent FilePlusIcon = EditorGUIUtility.IconContent("Collab.FileAdded", "Load File");

            public readonly GUIContent FolderPlusIcon =
                EditorGUIUtility.IconContent("Collab.FolderAdded", "Load From Folder");

            public readonly GUIContent SearchIcon = EditorGUIUtility.IconContent("ViewToolZoom", "Search");
            public readonly GUIContent TextureIcon = EditorGUIUtility.IconContent("RenderTexture Icon", "Preview");
            public readonly GUIContent PropertyIcon = EditorGUIUtility.IconContent("d_Preset.Context");

            public readonly GUIContent ListIcon =
                EditorGUIUtility.IconContent("d_SelectionListTemplate Icon", "SpriteAtlas List");

            public readonly GUIContent RefreshIcon = EditorGUIUtility.IconContent("d_Refresh", "Refresh List");
            public readonly GUIContent NotFavoriteIcon = EditorGUIUtility.IconContent("d_Favorite", "Not Favorite");
            public readonly GUIContent FavoriteIcon = EditorGUIUtility.IconContent("Favorite Icon", "Favorite");

            public readonly GUIContent FilterByTypeIcon =
                EditorGUIUtility.IconContent("FilterByType", "Search by Type");

            public readonly Texture FavoriteTexture = EditorGUIUtility.Load("Favorite Icon") as Texture;
            public readonly Texture NotFavoriteTexture = EditorGUIUtility.Load("d_Favorite") as Texture;

            public const float POPUP_ITEM_HEIGHT = 16f;
            public const float POPUP_PADDING = 10f;

            private GUIContent[] editorModeToggles = null;

            public GUIContent[] EditorModeToggles
            {
                get
                {
                    if (editorModeToggles == null)
                    {
                        editorModeToggles = System.Enum.GetNames(typeof(SpriteAssetViewerWindow.EditorMode))
                            .Select(x => new GUIContent(x)).ToArray();
                    }

                    return editorModeToggles;
                }
            }

            public readonly GUIStyle LabelStyle = "Label";
            public readonly GUIStyle ButtonStyle = "Button";
            public readonly GUIStyle ButtonPressStyle = "Button";
            public readonly GUIStyle TabButtonStyle = "LargeButton";
            public readonly GUIStyle BoxStyle = "Box";
            public readonly GUIStyle MenuItem = "MenuItem";

            public readonly GUI.ToolbarButtonSize TabButtonSize = GUI.ToolbarButtonSize.FitToContents;

            public readonly GUIStyle SingleButton = new GUIStyle("Button")
            {
                fixedWidth = 15f,
                fixedHeight = 15f,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(),
                margin = new RectOffset()
            };

            public readonly GUIStyle SingleButtonPressed = new GUIStyle("Button")
            {
                fixedWidth = 15f,
                fixedHeight = 15f,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(),
                margin = new RectOffset()
            };
        }

        public static Styles SpriteAssetViewerStyles => styles ??= new Styles();

        public static bool IsExistFolder(string path)
        {
            return AssetDatabase.IsValidFolder(path);
        }

        public static bool IsExistFile<T>(string path) where T : UnityEngine.Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(path) != null;
        }

        public static SpriteAtlas CreateSpriteAtlasAndSetAb(string atlasName, string createFolderPath,
            Texture[] textures = null)
        {
            var Setting = SpriteAssetViewerSetting.GetSetting().GlobalSetting;

            List<string> assetChangedPathList = new List<string>(16);

            bool isExistsSpriteAtlas = IsExistsSpriteAtlas(atlasName, createFolderPath);
            // Create SpriteAtlas
            SpriteAtlas targetSpriteAtlas = CreateSpriteAtlas(
                atlasName,
                createFolderPath,
                Setting.atlasIsTightPack,
                Setting.atlasCompressionSetting,
                textures,
                false);


            string assetBundleName = string.Empty;

            if (!isExistsSpriteAtlas)
            {
                // SpriteAtlas set as Ab
                string spriteAtlasPath = AssetDatabase.GetAssetPath(targetSpriteAtlas);

                if (Setting.atlasIsAddAtlasAbString)
                {
                    assetBundleName = GetAssetBundleNameWithExtension(spriteAtlasPath);
                    SetAssetBundleName(spriteAtlasPath, assetBundleName);
                    assetChangedPathList.Add(spriteAtlasPath);
                }
            }
            else
            {
                assetBundleName = GetAssetBundleName(targetSpriteAtlas);
            }

            // Create SpriteAtals Over
            // Sprite(In SpriteAtlas) set as Ab (keep name is same as atlas)
            if (textures != null && textures.Length != 0 && !string.IsNullOrEmpty(assetBundleName))
            {
                assetChangedPathList.AddRange(KeepSpriteAtlasAbNameInnerTextures(textures, assetBundleName,
                    Setting.atlasIsAddAtlasContentsAbString, false));
            }

            var assetChangedPaths = assetChangedPathList.ToArray();

            // ReimportAssets to apply ab name
            ReimportAssets(assetChangedPaths);

            return targetSpriteAtlas;
        }

        public static string[] Change2SpriteAtlasAndSetAb(string atlasName, string createFolderPath,
            Texture[] textures = null, bool isReimportAssets = true)
        {
            var Setting = SpriteAssetViewerSetting.GetSetting().GlobalSetting;

            List<string> assetChangedPathList = new List<string>(16);

            bool isExistsSpriteAtlas = IsExistsSpriteAtlas(atlasName, createFolderPath);
            // Create SpriteAtlas
            SpriteAtlas targetSpriteAtlas = CreateSpriteAtlas(
                atlasName,
                createFolderPath,
                Setting.Packer2Atlas_IsTightPack,
                Setting.Packer2Atlas_CompressionSetting,
                textures,
                false);

            string assetBundleName = string.Empty;

            if (!isExistsSpriteAtlas)
            {
                // SpriteAtlas set as Ab
                string spriteAtlasPath = AssetDatabase.GetAssetPath(targetSpriteAtlas);

                if (Setting.atlasIsAddAtlasAbString)
                {
                    assetBundleName = GetAssetBundleNameWithExtension(spriteAtlasPath);
                    SetAssetBundleName(spriteAtlasPath, assetBundleName);
                    assetChangedPathList.Add(spriteAtlasPath);
                }
            }
            else
            {
                assetBundleName = GetAssetBundleName(targetSpriteAtlas);
            }

            // Create SpriteAtals Over
            // Sprite(In SpriteAtlas) set as Ab (keep name is same as atlas)
            if (textures != null && textures.Length != 0 && !string.IsNullOrEmpty(assetBundleName))
            {
                assetChangedPathList.AddRange(KeepSpriteAtlasAbNameInnerTextures(textures, assetBundleName,
                    Setting.packer2AtlasIsAddAtlasContentsAbString, false));
            }

            var assetChangedPaths = assetChangedPathList.ToArray();

            // ReimportAssets to apply ab name
            if (isReimportAssets)
            {
                ReimportAssets(assetChangedPaths);
            }

            return assetChangedPaths;
        }

        public static bool IsExistsSpriteAtlas(string atlasName, string folderPath)
        {
            return GetSpriteAtlas(atlasName, folderPath) != null;
        }

        public static SpriteAtlas GetSpriteAtlas(string atlasName, string folderPath)
        {
            string fileName = atlasName.Replace('/', '_').Replace('\\', '_');
            string fullPath = string.Format("{0}/{1}.{2}", folderPath, fileName,
                SpriteAssetViewerUtility.SpriteAtlasExtension);
            return AssetDatabase.LoadAssetAtPath<SpriteAtlas>(fullPath);
        }

        public static SpriteAtlas CreateSpriteAtlas(
            string atlasName,
            string createFolderPath,
            bool isTight,
            TextureImporterCompression compression,
            Texture[] textures = null,
            bool isReimport = true)
        {
            var Setting = SpriteAssetViewerSetting.GetSetting().GlobalSetting;

            // Create folder
            CreateBaseFolder(createFolderPath);

            // SpriteAtlas Name
            string fileName = atlasName.Replace('/', '_').Replace('\\', '_');

            // SpriteAtlas Path
            string atlasCreatePath = string.Format("{0}/{1}.{2}", createFolderPath, fileName,
                SpriteAssetViewerUtility.SpriteAtlasExtension);

            bool isNeedCreate = false;

            SpriteAtlas targetSpriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasCreatePath);

            if (targetSpriteAtlas == null)
            {
                targetSpriteAtlas = new SpriteAtlas();
                isNeedCreate = true;
            }

            if (textures != null && textures.Length != 0)
            {
                Object[] packedObjects = targetSpriteAtlas.GetAtlasPacked();
                List<Texture> exsitsTextures = new List<Texture>(packedObjects.Length);

                for (int i = 0; i < packedObjects.Length; i++)
                {
                    if (packedObjects[i] is Sprite)
                    {
                        exsitsTextures.Add((packedObjects[i] as Sprite).texture);
                    }
                    else if (packedObjects[i] is Texture)
                    {
                        exsitsTextures.Add(packedObjects[i] as Texture);
                    }
                }

                textures = textures.Except(exsitsTextures.ToArray()).ToArray();

                targetSpriteAtlas.Add(textures);
            }

            if (!isTight)
            {
                var packingSetting = targetSpriteAtlas.GetPackingSettings();
                packingSetting.enableTightPacking = false;
                targetSpriteAtlas.SetPackingSettings(packingSetting);
            }

            if (compression != TextureImporterCompression.Compressed)
            {
                var platformSetting = targetSpriteAtlas.GetPlatformSettings(DefaultTextureCompressionPlatformName);
                platformSetting.textureCompression = compression;
                targetSpriteAtlas.SetPlatformSettings(platformSetting);
            }

            if (isNeedCreate)
            {
                AssetDatabase.CreateAsset(targetSpriteAtlas,
                    string.Format("{0}/{1}.spriteatlas", createFolderPath, fileName));
            }
            else
            {
                EditorUtility.SetDirty(targetSpriteAtlas);
            }

            if (isReimport)
            {
                string spriteAtlasPath = AssetDatabase.GetAssetPath(targetSpriteAtlas);
                ReimportAsset(spriteAtlasPath);
            }

            return targetSpriteAtlas;
        }

        private static string[] KeepSpriteAtlasAbNameInnerTextures(Sprite[] targetSprites, string assetBundleName,
            bool isAddAtlasContentsAbString, bool isReimport = true)
        {
            var Setting = SpriteAssetViewerSetting.GetSetting().GlobalSetting;

            List<string> spritePaths = new List<string>(targetSprites.Length);

            // SpritePacker data change start
            if (isAddAtlasContentsAbString || Setting.Packer2Atlas_IsCheckoutTag)
            {
                for (int i = 0; i < targetSprites.Length; i++)
                {
                    string assetPath = AssetDatabase.GetAssetPath(targetSprites[i]);

                    // Setting -> Set assetbundle name as same as spriteatlas
                    // Setting -> Checkout PackingTag
                    if (isAddAtlasContentsAbString && Setting.Packer2Atlas_IsCheckoutTag)
                    {
                        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                        SetAssetBundleName(textureImporter, assetBundleName);
                        RemoveSpritePackingTag(textureImporter);
                    }
                    else
                    {
                        // Setting -> Only Set assetbundle name as same as spriteatlas
                        if (isAddAtlasContentsAbString)
                        {
                            SetAssetBundleName(assetPath, assetBundleName);
                        }

                        // Setting -> Only Checkout PackingTag
                        else if (Setting.Packer2Atlas_IsCheckoutTag)
                        {
                            RemoveSpritePackingTag(assetPath);
                        }
                    }

                    spritePaths.Add(assetPath);
                }
            }

            if (isReimport)
            {
                ReimportAssets(spritePaths.ToArray());
            }

            return spritePaths.ToArray();
        }

        private static string[] KeepSpriteAtlasAbNameInnerTextures(Texture[] targetTextures, string assetBundleName,
            bool isAddAtlasContentsAbString, bool isReimport = true)
        {
            var Setting = SpriteAssetViewerSetting.GetSetting().GlobalSetting;

            List<string> targetTexturesPaths = new List<string>(targetTextures.Length);

            // SpritePacker data change start
            if (isAddAtlasContentsAbString || Setting.Packer2Atlas_IsCheckoutTag)
            {
                for (int i = 0; i < targetTextures.Length; i++)
                {
                    string assetPath = AssetDatabase.GetAssetPath(targetTextures[i]);

                    // Setting -> Set assetbundle name as same as spriteatlas
                    // Setting -> Checkout PackingTag
                    if (isAddAtlasContentsAbString && Setting.Packer2Atlas_IsCheckoutTag)
                    {
                        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                        SetAssetBundleName(textureImporter, assetBundleName);
                        RemoveSpritePackingTag(textureImporter);
                    }
                    else
                    {
                        // Setting -> Only Set assetbundle name as same as spriteatlas
                        if (isAddAtlasContentsAbString)
                        {
                            SetAssetBundleName(assetPath, assetBundleName);
                        }

                        // Setting -> Only Checkout PackingTag
                        else if (Setting.Packer2Atlas_IsCheckoutTag)
                        {
                            RemoveSpritePackingTag(assetPath);
                        }
                    }

                    targetTexturesPaths.Add(assetPath);
                }
            }

            if (isReimport)
            {
                ReimportAssets(targetTexturesPaths.ToArray());
            }

            return targetTexturesPaths.ToArray();
        }

        /// <summary>
        /// 該当リソースが存在すべきフォルダPathが存在していない場合、必要なフォルダを全部作ります
        /// </summary>
        public static void CreateBaseFolder(string path)
        {
            Queue<string> pathSplitQueue = new Queue<string>(path.Split('/'));
            string baseFolderPath = pathSplitQueue.Dequeue();

            while (pathSplitQueue.Count != 0)
            {
                string newFolderName = pathSplitQueue.Dequeue();

                if (string.IsNullOrEmpty(newFolderName))
                {
                    break;
                }

                string folderNext = string.Format("{0}/{1}", baseFolderPath, newFolderName);

                if (!AssetDatabase.IsValidFolder(folderNext))
                {
                    try
                    {
                        AssetDatabase.CreateFolder(baseFolderPath, newFolderName);
                    }
                    catch
                    {
                        UnityEngine.Debug.LogErrorFormat(
                            "Create folder failed ! BaseFolderPath:{0} TargetFolderPath:{1}", baseFolderPath,
                            newFolderName);
                    }
                }

                baseFolderPath = folderNext;
            }
        }

        internal static string FindSpriteAtlas(string fileName, SpriteAssetViewerSetting.FolderType folderType)
        {
            string[] searchResultGUIDs = AssetDatabase.FindAssets(fileName,
                new string[1] { SpriteAssetViewerSetting.GetSetting().GetFolderSavePath(folderType) });
            List<string> targetPathList = new List<string>(4);
            foreach (var guid in searchResultGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Substring(path.LastIndexOf(fileName) + fileName.Length).StartsWith("."))
                {
                    targetPathList.Add(path);
                }
            }

            if (targetPathList.Count == 0)
            {
                return string.Empty;
            }
            else if (targetPathList.Count > 1)
            {
                EditorUtility.DisplayDialog("Error",
                    string.Format("Find more than one sprite atlas(name:{0}", fileName), "Ok");
            }

            return targetPathList[0];
        }

        public static List<T> GetAssetList<T>(string path) where T : class
        {
            if (!Directory.Exists(path))
            {
                return new List<T>(0);
            }

            string[] fileEntries = Directory.GetFiles(path);

            return fileEntries.Select(fileName =>
            {
                fileName = fileName.Replace("\\", "/");
                string assetPath = fileName.Substring(fileName.IndexOf("Assets"));
                assetPath = Path.ChangeExtension(assetPath, null);
                return AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
            }).OfType<T>().ToList();
        }

        public static string[] GetSubFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                return Array.Empty<string>();
            }

            string[] directoriesPath = Directory.GetDirectories(path);
            for (int i = 0; i < directoriesPath.Length; i++)
            {
                directoriesPath[i] = directoriesPath[i].Replace("\\", "/");
            }

            return directoriesPath;
        }

        public static string GetFolderName(string path)
        {
            string[] splitPath = path.Split('/');
            return splitPath.Last();
        }

        public static string GetFolderPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }

            int lastSlashIndex = filePath.LastIndexOf('/');
            if (lastSlashIndex < 0)
            {
                return string.Empty;
            }

            return filePath.Substring(0, filePath.LastIndexOf('/'));
        }

        public static string[] GetFolderFileNames(string path, string extension = "")
        {
            if (!IsExistFolder(path))
            {
                return new string[0];
            }

            string[] fileEntries = Directory.GetFiles(path);
            string[] fileNames = new string[fileEntries.Length];
            if (string.IsNullOrEmpty(extension))
            {
                fileNames = fileEntries;
            }
            else
            {
                fileNames = fileEntries.Where(fileName => fileName.EndsWith(extension)).ToArray();
            }


            for (int i = 0; i < fileNames.Length; i++)
            {
                fileNames[i] = fileNames[i].Replace("\\", "/");
                fileNames[i] = fileNames[i].Substring(fileNames[i].LastIndexOf("/") + 1);
            }

            return fileNames;
        }

        public static void DrawLine(Vector3 p1, Vector3 p2)
        {
            GL.Vertex(p1);
            GL.Vertex(p2);
        }

        public static void BeginLines(Color color)
        {
            GL.PushMatrix();
            GL.MultMatrix(Handles.matrix);
            GL.Begin(1);
            GL.Color(color);
        }

        public static void EndLines()
        {
            GL.End();
            GL.PopMatrix();
        }

        public static void ShowProgressbar(int currentFileIndex, int totalFileCount)
        {
            float progress = (float)currentFileIndex / totalFileCount;
            EditorUtility.DisplayProgressBar(
                "Creating...",
                string.Format("Total progress {0}% .", (progress * 100).ToString("F2")),
                progress
            );

            if (currentFileIndex == totalFileCount)
            {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        ///  Get the resource's assetBundleName has set
        /// </summary>
        public static string GetAssetBundleName(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                return string.Empty;
            }

            AssetImporter importer = AssetImporter.GetAtPath(resourcePath);
            return importer.assetBundleName;
        }

        /// <summary>
        ///  Get the resource's assetBundleName has set
        /// </summary>
        public static string GetAssetBundleName(Object resource)
        {
            if (resource == null)
            {
                return string.Empty;
            }

            string assetFilePath = AssetDatabase.GetAssetPath(resource);
            return GetAssetBundleName(assetFilePath);
        }

        /// <summary>
        /// Get the resource assetBundleName from fileName.
        /// </summary>
        public static string GetAssetBundleNameWithExtension(Object resource)
        {
            if (resource == null)
            {
                EditorUtility.DisplayDialog("Error", "Get asseetBundle failed. The target resource is null ! ", "Ok");
                return string.Empty;
            }

            return GetAssetBundleNameWithExtension(AssetDatabase.GetAssetPath(resource));
        }

        /// <summary>
        /// Get the resource assetBundleName from fileName.
        /// </summary>
        public static string GetAssetBundleNameWithExtension(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                EditorUtility.DisplayDialog("Error", "Get asseetBundle failed. The target resourcePath is empty ! ",
                    "Ok");
                return string.Empty;
            }

            var abName =
                $"{Path.GetFileName(resourcePath).Replace('.', '_')}." +
                $"{SpriteAssetViewerSetting.GetSetting().GlobalSetting.assetBundleFileExtension}";
            return abName;
        }

        /// <summary>
        /// 対象Resourceにアセットバンドル名を設定（拡張子なし
        /// </summary>
        /// <param name="resource">対象リソース</param>
        /// <returns>設定成功かどうか</returns>
        public static bool SetAssetBundleWithExtension(string resourcePath, bool reimport = false)
        {
            string assetBundleName = GetAssetBundleNameWithExtension(resourcePath);
            return SetAssetBundleName(resourcePath, assetBundleName, reimport);
        }

        public static bool SetAssetBundleWithExtension(Object resource, bool reimport = false)
        {
            string assetPath = AssetDatabase.GetAssetPath(resource);
            return SetAssetBundleWithExtension(assetPath, reimport);
        }

        public static bool SetAssetBundleName(AssetImporter assetImporter, string assetBundleName,
            bool reimport = false)
        {
            if (assetImporter == null)
            {
                return false;
            }

            if (assetImporter.assetBundleName == assetBundleName)
            {
                return true;
            }

            assetImporter.assetBundleName = assetBundleName;
            if (reimport)
            {
                assetImporter.SaveAndReimport();
            }

            return true;
        }

        public static bool SetAssetBundleName(string resourcePath, string assetBundleName, bool reimport = false)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                return false;
            }

            AssetImporter importer = AssetImporter.GetAtPath(resourcePath);
            return SetAssetBundleName(importer, assetBundleName, reimport);
        }

        public static bool RemoveSpritePackingTag(TextureImporter textureImporter, bool reimport = false)
        {
            if (textureImporter == null)
            {
                return false;
            }

            textureImporter.spritePackingTag = string.Empty;

            if (reimport)
            {
                textureImporter.SaveAndReimport();
            }

            return true;
        }

        public static bool RemoveSpritePackingTag(string texturePath, bool reimport = false)
        {
            if (string.IsNullOrEmpty(texturePath))
            {
                return false;
            }

            var targetimporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;

            return RemoveSpritePackingTag(targetimporter, reimport);
        }

        public static void RemoveSpritesPackingTag(string[] texturePaths, bool reimport = false)
        {
            foreach (var texturePath in texturePaths)
            {
                RemoveSpritePackingTag(texturePath);
            }

            if (reimport)
            {
                ReimportAssets(texturePaths);
            }
        }

        public static void ReimportAsset(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                return;
            }

            AssetDatabase.ImportAsset(resourcePath);
        }

        public static void ReimportAssets(string[] resourcePaths)
        {
            try
            {
                AssetDatabase.StartAssetEditing();

                for (int i = 0; i < resourcePaths.Length; i++)
                {
                    AssetDatabase.ImportAsset(resourcePaths[i]);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        public static int[] CheckAssetBundleStringMatch(SpriteAtlas spriteAtlas)
        {
            if (spriteAtlas == null)
            {
                return null;
            }

            string assetBundleName = GetAssetBundleName(spriteAtlas);
            Object[] targetPackedAssets = spriteAtlas.GetPackables();

            List<int> notMachedIndexArray = new List<int>(targetPackedAssets.Length);

            for (int i = 0; i < targetPackedAssets.Length; i++)
            {
                string packedAssetAbName = GetAssetBundleName(targetPackedAssets[i]);

                if (packedAssetAbName != assetBundleName)
                {
                    notMachedIndexArray.Add(i);
                }
            }

            return notMachedIndexArray.ToArray();
        }

        public static void SetAllContentsAssetBundle(SpriteAtlas spriteAtlas)
        {
            if (spriteAtlas == null)
            {
                return;
            }

            string assetBundleName = SpriteAssetViewerUtility.GetAssetBundleName(spriteAtlas);

            var targetPackedAssets = spriteAtlas.GetPackables();

            for (int i = 0; i < targetPackedAssets.Length; i++)
            {
                SetAssetBundleName(AssetDatabase.GetAssetPath(targetPackedAssets[i]), assetBundleName);
            }
        }

        public static void ClearAllContentsAssetBundle(SpriteAtlas spriteAtlas)
        {
            if (spriteAtlas == null)
            {
                return;
            }

            var targetPackedAssets = spriteAtlas.GetPackables();
            for (int i = 0; i < targetPackedAssets.Length; i++)
            {
                SetAssetBundleName(AssetDatabase.GetAssetPath(targetPackedAssets[i]), string.Empty);
            }
        }

        public static Color GetMessageTypeColor(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Warning:
                    return Color.yellow;
                case MessageType.Error:
                    return Color.red;
                case MessageType.None:
                case MessageType.Info:
                default:
                    return Color.white;
            }
        }

        public static SpriteAtlas FindAtlasFromTexture(Texture targetTexture)
        {
            if (targetTexture)
            {
                string path = AssetDatabase.GetAssetPath(targetTexture);
                string guid = AssetDatabase.AssetPathToGUID(path);
                var setting = SpriteAssetViewerSetting.GetSetting().GlobalSetting;

                string[] findTargetPath = new string[1] { setting.SpriteAtlasSavePath };
                return FindSpriteAtlasByPath(guid, findTargetPath);
            }

            return null;
        }

        private static SpriteAtlas FindSpriteAtlasByPath(string targetGUID, string[] paths,
            bool includeSubFolder = true)
        {
            foreach (var t2 in paths)
            {
                var subSpriteAtlas = GetAssetList<SpriteAtlas>(t2);
                foreach (var t1 in from t1 in subSpriteAtlas
                         let packedAssets = t1.GetPackables()
                         where packedAssets.Any(
                             t => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(t)) == targetGUID)
                         select t1)
                {
                    return t1;
                }
            }

            if (!includeSubFolder) return null;
            return paths.Select(t => FindSpriteAtlasByPath(targetGUID, GetSubFolder(t)))
                .FirstOrDefault(targetAtals => targetAtals);
        }

        [MenuItem("Assets/Find Target Atlas", false, 20)]
        private static void SearchSpriteAtlas()
        {
            if (Selection.objects.Length > 0)
            {
                SpriteAssetViewerWindow.ShowWindowAndSearchAtlas(Selection.objects[0]);
            }
        }

        [MenuItem("Assets/Find Target Atlas", true)]
        private static bool ValidateSearchSpriteAtlas()
        {
            bool isShowMenu = false;

            if (Selection.objects.Length <= 0)
            {
                return false;
            }

            if (EditorSettings.spritePackerMode != SpritePackerMode.AlwaysOnAtlas &&
                EditorSettings.spritePackerMode != SpritePackerMode.BuildTimeOnlyAtlas)
            {
                return false;
            }

            var selectionObjectType = Selection.objects[0].GetType();

            isShowMenu = selectionObjectType == typeof(Texture) ||
                         selectionObjectType.IsSubclassOf(typeof(Texture)) ||
                         selectionObjectType == typeof(Sprite) ||
                         selectionObjectType.IsSubclassOf(typeof(Sprite));

            return isShowMenu;
        }

        [MenuItem("Assets/Create Sprite Atlas", false, 21)]
        private static void CreateSpriteAtlas()
        {
            if (Selection.objects.Length == 0)
            {
                return;
            }

            HashSet<Texture> targetSprites = new HashSet<Texture>();

            foreach (var selectionObject in Selection.objects)
            {
                var selectionObjectType = selectionObject.GetType();

                if (selectionObjectType == typeof(Texture) ||
                    selectionObjectType.IsSubclassOf(typeof(Texture)))
                {
                    targetSprites.Add((Texture)selectionObject);
                }
                else if (selectionObjectType == typeof(Sprite) ||
                         selectionObjectType.IsSubclassOf(typeof(Sprite)))
                {
                    targetSprites.Add(((Sprite)selectionObject).texture);
                }
            }

            SpriteAtlasMakerWindow.ShowWindow(targetSprites.ToArray());
        }

        [MenuItem("Assets/Create Sprite Atlas", true)]
        private static bool ValidateCreateSpriteAtlas()
        {
            if (Selection.objects.Length <= 0)
            {
                return false;
            }

            if (EditorSettings.spritePackerMode != SpritePackerMode.AlwaysOnAtlas &&
                EditorSettings.spritePackerMode != SpritePackerMode.BuildTimeOnlyAtlas)
            {
                return false;
            }

            return Selection.objects.Select(t => t.GetType()).All(selectionObjectType =>
                selectionObjectType == typeof(Texture) || selectionObjectType.IsSubclassOf(typeof(Texture)) ||
                selectionObjectType == typeof(Sprite) || selectionObjectType.IsSubclassOf(typeof(Sprite)));
        }
    }
}
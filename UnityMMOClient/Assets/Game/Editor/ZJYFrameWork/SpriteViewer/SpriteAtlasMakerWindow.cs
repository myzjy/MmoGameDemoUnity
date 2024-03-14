using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ZJYFrameWork.SpriteViewer
{
    public partial class SpriteAtlasMakerWindow : EditorWindow
    {
        private static SpriteAtlasMakerWindow instance;

        private HashSet<Texture> textureHashSet;

        private Vector2 scorllViewPosition;

        private SpriteAssetViewerSetting.FolderType saveType = SpriteAssetViewerSetting.FolderType.AssetBundle;

        private string targetSpriteAtlasName = string.Empty;

        public static void ShowWindow(Texture[] textures)
        {
            if (instance != null)
            {
                instance.Close();
            }

            switch (EditorSettings.spritePackerMode)
            {
                case SpritePackerMode.Disabled:
#pragma warning disable CS0618
                case SpritePackerMode.BuildTimeOnly:
#pragma warning restore CS0618
#pragma warning disable CS0618
                case SpritePackerMode.AlwaysOn:
#pragma warning restore CS0618
                    return;
                case SpritePackerMode.BuildTimeOnlyAtlas:
                case SpritePackerMode.AlwaysOnAtlas:
                    if (instance != null)
                    {
                        instance.Close();
                    }

                    instance = EditorWindow.GetWindow<SpriteAtlasMakerWindow>();
                    instance.textureHashSet = new HashSet<Texture>(textures);
                    instance.Show();
                    instance.Focus();
                    break;
                case SpritePackerMode.SpriteAtlasV2:
                default:
                    break;
            }
        }

        private void OnEnable()
        {
            textureHashSet = new HashSet<Texture>();
        }

        private void OnGUI()
        {
            Texture tempDeleteTexture = null;

            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
            {
                scorllViewPosition = EditorGUILayout.BeginScrollView(scorllViewPosition);
                {
                    foreach (var texture in textureHashSet)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.ObjectField(texture, typeof(Texture), true);

                            if (GUILayout.Button("ー", GUILayout.Width(15f), GUILayout.Height(15f)))
                            {
                                tempDeleteTexture = texture;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndScrollView();

                GUILayout.FlexibleSpace();

                // フォルダーことの選択
                Texture newTexture = null;
                EditorGUI.BeginChangeCheck();
                {
                    if (textureHashSet.Count > 0)
                    {
                        EditorGUILayout.Space();
                    }

                    EditorGUILayout.PrefixLabel("Add : ");
                    newTexture =
                        EditorGUILayout.ObjectField(newTexture, typeof(Texture), true, GUILayout.ExpandWidth(true)) as
                            Texture;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    if (newTexture != null)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(newTexture);
                        textureHashSet.Add(AssetDatabase.LoadAssetAtPath<Texture>(assetPath));
                    }
                }

                EditorGUILayout.PrefixLabel("Add from folder : ");

                // 単一ファイル選択
                EditorGUILayout.BeginHorizontal();
                {
                    DefaultAsset targetFolder = null;
                    EditorGUI.BeginChangeCheck();
                    {
                        targetFolder = EditorGUILayout.ObjectField(null, typeof(DefaultAsset), true) as DefaultAsset;
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (targetFolder != null)
                        {
                            LoadAssetsAtPath(AssetDatabase.GetAssetPath(targetFolder));
                        }
                    }

                    if (GUILayout.Button("Select"))
                    {
                        OnClickLoadFolder();
                    }
                }
                EditorGUILayout.EndHorizontal();

                // Delete処理
                if (tempDeleteTexture != null)
                {
                    textureHashSet.Remove(tempDeleteTexture);
                }

                // ボタンたち
                if (GUILayout.Button("Clear All"))
                {
                    OnClickClearButton();
                }

                EditorGUILayout.Space();

                targetSpriteAtlasName = EditorGUILayout.TextField("Atlas Name : ", targetSpriteAtlasName);
                saveType = (SpriteAssetViewerSetting.FolderType)EditorGUILayout.EnumPopup("Save As :", saveType);

                if (GUILayout.Button("Create"))
                {
                    OnClickCreate();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void LoadAssetsAtPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            List<Texture> targetTextures = SpriteAssetViewerUtility.GetAssetList<Texture>(path);
            if (targetTextures != null || targetTextures.Count != 0)
            {
                for (int i = 0; i < targetTextures.Count; i++)
                {
                    textureHashSet.Add(targetTextures[i]);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Info", "Can't find any textures.", "ok");
            }
        }

        private void OnClickClearButton()
        {
            textureHashSet.Clear();
            Repaint();
        }

        private void OnClickLoadFolder()
        {
            LoadAssetsAtPath(EditorUtility.OpenFolderPanel("Load png Textures", "Assets", ""));
        }

        private void OnClickCreate()
        {
            if (textureHashSet.Count == 0)
            {
                EditorUtility.DisplayDialog("Error", "The sprite resource has not set ! ", "Ok");
                return;
            }

            if (string.IsNullOrEmpty(targetSpriteAtlasName))
            {
                EditorUtility.DisplayDialog("Error", "The sprite atlas name is empty", "Ok");
                return;
            }

            string savePath = SpriteAssetViewerSetting.GetSetting().GetFolderSavePath(saveType);

            string fileName = targetSpriteAtlasName.Replace('/', '_').Replace('\\', '_');

            StringBuilder builder = null;

            List<Object> needRemoveObjectList = null;

            foreach (var texture in textureHashSet)
            {
                if (!string.IsNullOrEmpty(SpriteAssetViewerUtility.GetAssetBundleName(texture)))
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder("The texture already is set as a assetbundle .\nTexture name : ");
                        needRemoveObjectList = new List<Object>();
                    }

                    needRemoveObjectList.Add(texture);
                    builder.AppendFormat("\n{0}", texture.name);
                }
            }

            if (builder != null)
            {
                EditorUtility.DisplayDialog("Error", builder.ToString(), "Ok");
                return;
            }

            var existFilePath = SpriteAssetViewerUtility.FindSpriteAtlas(fileName, saveType);
            if (!string.IsNullOrEmpty(existFilePath))
            {
                if (!EditorUtility.DisplayDialog("Warring",
                        $"存在spriteAtlas ! \n (path : {existFilePath})\n将这些纹理添加到这个图集中? ",
                        "Ok", "No")) return;
                var targetAtlas = SpriteAssetViewerUtility.CreateSpriteAtlasAndSetAb(targetSpriteAtlasName,
                    SpriteAssetViewerUtility.GetFolderPath(existFilePath), textureHashSet.ToArray());
                EditorGUIUtility.PingObject(targetAtlas);

                return;
            }

            if (!EditorUtility.DisplayDialog("Confirm",
                    $"Create the spriteAtlas ? \n fileName : {fileName} \n filePath : {savePath}",
                    "Yes", "No")) return;
            {
                var targetAtlas =
                    SpriteAssetViewerUtility.CreateSpriteAtlasAndSetAb(targetSpriteAtlasName, savePath,
                        textureHashSet.ToArray());
                EditorGUIUtility.PingObject(targetAtlas);
            }
        }
    }
}
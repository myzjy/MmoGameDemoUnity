using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace ZJYFrameWork.SpriteViewer
{
    [CustomEditor(typeof(SpriteAtlas), true)]
    [CanEditMultipleObjects]
    internal class SpriteAtlasEditor : Editor
    {
        #region Param

        private string spriteAtlasAbName;

        private int[] abErrorIndexArray;

        private SpriteAtlas targetSpriteAtlas;

        private bool shouldSetAsAssetBundle = false;

        private UnityEngine.Object[] cachedPackedObjects;

        private Editor baseEditor;

        public Editor BaseEditor
        {
            get
            {
                if (baseEditor == null)
                {
                    Assembly unityEditorU2D = NewMethod().Assembly;
                    Type gameobjectInspectorType = unityEditorU2D.GetType("UnityEditor.U2D.SpriteAtlasInspector");
                    CreateCachedEditor(targets, gameobjectInspectorType, ref baseEditor);
                }

                return baseEditor;
            }
        }

        private static Type NewMethod()
        {
            return typeof(UnityEditor.U2D.SpriteAtlasUtility);
        }

        private bool HasError
        {
            get { return HasAbError || HasSettingError; }
        }

        private bool HasAbError
        {
            get { return abErrorIndexArray != null && abErrorIndexArray.Length != 0; }
        }

        private bool HasSettingError
        {
            get
            {
                return (CheckTightSetting != MessageType.None && !MatchTightSetting) ||
                       (CheckCompressionSetting != MessageType.None && !MatchCompressionSetting);
            }
        }

        private MessageType CheckContentsAssetBundleSetting
        {
            get
            {
                return SpriteAssetViewerSetting.GetSetting().GlobalSetting
                    .atlasCheckAddAtlasContentsAbStringMessageType;
            }
        }

        private MessageType CheckTightSetting
        {
            get { return SpriteAssetViewerSetting.GetSetting().GlobalSetting.atlasCheckTightPackMessageType; }
        }

        private bool MatchTightSetting
        {
            get
            {
                return targetSpriteAtlas != null &&
                       SpriteAssetViewerSetting.GetSetting().GlobalSetting.atlasIsTightPack ==
                       targetSpriteAtlas.GetPackingSettings().enableTightPacking;
            }
        }

        private MessageType CheckCompressionSetting
        {
            get { return SpriteAssetViewerSetting.GetSetting().GlobalSetting.atlasCheckCompressionSettingMessageType; }
        }

        private bool MatchCompressionSetting
        {
            get
            {
                return targetSpriteAtlas != null &&
                       SpriteAssetViewerSetting.GetSetting().GlobalSetting.atlasCompressionSetting == targetSpriteAtlas
                           .GetPlatformSettings(SpriteAssetViewerUtility.DefaultTextureCompressionPlatformName)
                           .textureCompression;
            }
        }

        private MessageType CheckAddAtlasAbString
        {
            get { return SpriteAssetViewerSetting.GetSetting().GlobalSetting.atlasCheckAddAtlasAbStringMessageType; }
        }

        #endregion

        #region LifeCycle

        private void OnEnable()
        {
            targetSpriteAtlas = target as SpriteAtlas;

            shouldSetAsAssetBundle = ShouldSetAsAssetBundle();

            if (CheckContentsAssetBundleSetting != MessageType.None)
            {
                CheckContentsAssetBundle();
            }

            if (targetSpriteAtlas != null)
            {
                cachedPackedObjects = targetSpriteAtlas.GetPackables();
            }
        }

        public sealed override void OnInspectorGUI()
        {
            if (targetSpriteAtlas == null)
            {
                return;
            }

            EditorGUILayout.ObjectField("SpriteAtlas", target, typeof(SpriteAtlas), true);

            if (targets.Length == 1 && shouldSetAsAssetBundle)
            {
                Color guiBeforeColor = GUI.color;

                if (HasError)
                {
                    EditorGUILayout.BeginVertical("Box", GUILayout.ExpandWidth(true));
                    {
                        if (CheckTightSetting != MessageType.None && !MatchTightSetting)
                        {
                            GUI.color = SpriteAssetViewerUtility.GetMessageTypeColor(CheckTightSetting);
                            EditorGUILayout.HelpBox(
                                $"The default tight setting is {SpriteAssetViewerSetting.GetSetting().GlobalSetting.atlasIsTightPack.ToString()}.\nBut this atlas set {targetSpriteAtlas.GetPackingSettings().enableTightPacking.ToString()}",
                                CheckTightSetting
                            );

                            EditorGUILayout.Space();
                        }

                        if (CheckCompressionSetting != MessageType.None && !MatchCompressionSetting)
                        {
                            GUI.color = SpriteAssetViewerUtility.GetMessageTypeColor(CheckCompressionSetting);
                            EditorGUILayout.HelpBox(
                                $"默认压缩设置为{SpriteAssetViewerSetting.GetSetting().GlobalSetting.atlasCompressionSetting.ToString()}.\nBut this altas set {targetSpriteAtlas.GetPlatformSettings(SpriteAssetViewerUtility.DefaultTextureCompressionPlatformName).textureCompression.ToString()}",
                                CheckCompressionSetting
                            );

                            EditorGUILayout.Space();
                        }

                        if (HasAbError)
                        {
                            GUI.color = SpriteAssetViewerUtility.GetMessageTypeColor(CheckContentsAssetBundleSetting);
                            EditorGUILayout.LabelField("SpriteAtlas AssetBundle名称为 : ");
                            EditorGUILayout.TextField(SpriteAssetViewerUtility.GetAssetBundleName(targetSpriteAtlas));

                            EditorGUILayout.Space();

                            EditorGUILayout.LabelField("Different with : ");

                            var contents = targetSpriteAtlas.GetPackables();

                            for (int i = 0; i < abErrorIndexArray.Length; i++)
                            {
                                if (abErrorIndexArray[i] >= contents.Length)
                                {
                                    CheckContentsAssetBundle();
                                    break;
                                }

                                string errorAbName =
                                    SpriteAssetViewerUtility.GetAssetBundleName(contents[abErrorIndexArray[i]]);
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.ObjectField(contents[abErrorIndexArray[i]],
                                        typeof(UnityEngine.Object), false);
                                    EditorGUILayout.LabelField(string.IsNullOrEmpty(errorAbName)
                                        ? "None"
                                        : errorAbName);
                                }
                                EditorGUILayout.EndHorizontal();
                            }

                            if (GUILayout.Button("Update"))
                            {
                                CheckContentsAssetBundle();
                                Repaint();
                            }

                            if (GUILayout.Button("将所有内容设置为AssetBundle"))
                            {
                                SetContentsAssetBundle();
                                CheckContentsAssetBundle();
                                Repaint();
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                }

                GUI.color = guiBeforeColor;

                BaseEditor.OnInspectorGUI();

                var targetSpriteAtlasAbName = SpriteAssetViewerUtility.GetAssetBundleName(targetSpriteAtlas);
                var targetSpriteAtlasWithName = SpriteAssetViewerUtility
                    .GetAssetBundleNameWithExtension(targetSpriteAtlas).ToLower();
                if (CheckAddAtlasAbString != MessageType.None &&
                    targetSpriteAtlasAbName !=targetSpriteAtlasWithName)
                {
                    Color guiColorBefore = GUI.color;
                    {
                        GUI.color = SpriteAssetViewerUtility.GetMessageTypeColor(CheckAddAtlasAbString);
                        EditorGUILayout.HelpBox("资产包名称设置错误.", CheckAddAtlasAbString);
                        if (GUILayout.Button("Set As AssetBundle"))
                        {
                            SetAsAssetBundle();
                        }
                    }
                    if (GUI.color != guiColorBefore)
                    {
                        GUI.color = guiColorBefore;
                    }
                }
            }
            else
            {
                BaseEditor.OnInspectorGUI();
            }

            GUILayout.Space(20f);

            if (GUILayout.Button("Release SpriteAtlas"))
            {
                if (EditorUtility.DisplayDialog("Warring!",
                        "删除spriteAtlas并将精灵设置为默认值。\n 此操作无法撤消!\n Continue ? ",
                        "Yes", "No"))
                {
                    SetContentDefault();
                    ReleaseSpriteAtlas();
                }
            }

            CheckSpriteAtlasChanged();
        }

        private void OnDestroy()
        {
            if (baseEditor != null)
            {
                DestroyImmediate(baseEditor);
                baseEditor = null;
            }
        }


        public override bool RequiresConstantRepaint()
        {
            return BaseEditor.RequiresConstantRepaint();
        }

        public override bool UseDefaultMargins()
        {
            return BaseEditor.UseDefaultMargins();
        }

        public override bool HasPreviewGUI()
        {
            return BaseEditor.HasPreviewGUI();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            BaseEditor.OnPreviewGUI(r, background);
        }

        public override void DrawPreview(Rect previewArea)
        {
            BaseEditor.DrawPreview(previewArea);
        }

        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width,
            int height)
        {
            return BaseEditor.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        public override GUIContent GetPreviewTitle()
        {
            return BaseEditor.GetPreviewTitle();
        }

        #endregion

        #region 内部メソッド

        private void SetAsAssetBundle()
        {
            if (targets == null || targets.Length == 0)
            {
                return;
            }

            for (int i = 0; i < targets.Length; i++)
            {
                SpriteAssetViewerUtility.SetAssetBundleWithExtension(targets[i]);
            }
        }

        private void SetContentsAssetBundle()
        {
            if (targetSpriteAtlas == null)
            {
                return;
            }

            SpriteAssetViewerUtility.SetAllContentsAssetBundle(targetSpriteAtlas);
        }

        private void CheckContentsAssetBundle()
        {
            if (targets.Length != 1)
            {
                return;
            }

            if (target == null)
            {
                return;
            }

            abErrorIndexArray = SpriteAssetViewerUtility.CheckAssetBundleStringMatch(targetSpriteAtlas);
        }

        private bool ShouldSetAsAssetBundle()
        {
            if (targetSpriteAtlas == null)
            {
                return false;
            }

            string path = AssetDatabase.GetAssetPath(target).Replace('\\', '/');
            string abFolderPath = SpriteAssetViewerSetting.GetSetting()
                .GetFolderSavePath(SpriteAssetViewerSetting.FolderType.AssetBundle).Replace('\\', '/');

            if (path.Contains(abFolderPath))
            {
                return true;
            }

            return false;
        }

        private void CheckSpriteAtlasChanged()
        {
            if (targetSpriteAtlas == null)
            {
                return;
            }

            if (!SpriteAssetViewerSetting.GetSetting().GlobalSetting.atlasIsAddAtlasContentsAbString)
            {
                return;
            }

            var currentPackedObjects = targetSpriteAtlas.GetPackables();
            if (currentPackedObjects.Length != cachedPackedObjects.Length)
            {
                // 有什么变化(内容的增减)
                if (currentPackedObjects.Length > cachedPackedObjects.Length)
                {
                    // 内容的增加
                    var addObjects = currentPackedObjects.Except(cachedPackedObjects).ToArray();

                    StringBuilder builder = null;
                    List<UnityEngine.Object> needRemoveObjectList = null;
                    foreach (var addObject in addObjects)
                    {
                        string addObjectAssetBundleName = SpriteAssetViewerUtility.GetAssetBundleName(addObject);
                        if (!string.IsNullOrEmpty(addObjectAssetBundleName) &&
                            SpriteAssetViewerUtility.GetAssetBundleName(targetSpriteAtlas) != addObjectAssetBundleName)
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(
                                    "纹理已经被设置为一个资产包 .\n 纹理的名字 : ");
                                needRemoveObjectList = new List<UnityEngine.Object>();
                            }

                            needRemoveObjectList.Add(addObject);
                            builder.AppendFormat("\n{0}", addObject.name);
                        }
                    }

                    if (builder != null)
                    {
                        targetSpriteAtlas.Remove(needRemoveObjectList.ToArray());
                        EditorUtility.DisplayDialog("Error", builder.ToString(), "Ok");
                        return;
                    }

                    SetContentsAssetBundle();
                }
                else
                {
                    // 内容の減
                    var removeObjects = cachedPackedObjects.Except(currentPackedObjects).ToArray();
                    for (int i = 0; i < removeObjects.Length; i++)
                    {
                        SpriteAssetViewerUtility.SetAssetBundleName(AssetDatabase.GetAssetPath(removeObjects[i]),
                            string.Empty, true);
                    }
                }

                cachedPackedObjects = currentPackedObjects;
            }
        }

        #region Release

        private void ReleaseSpriteAtlas()
        {
            if (targetSpriteAtlas == null)
            {
                return;
            }

            string path = AssetDatabase.GetAssetPath(targetSpriteAtlas);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.Refresh();

            this.Repaint();
        }

        private void SetContentDefault()
        {
            if (targetSpriteAtlas == null)
            {
                return;
            }

            if (SpriteAssetViewerSetting.GetSetting().GlobalSetting.atlasIsAddAtlasContentsAbString)
            {
                SpriteAssetViewerUtility.ClearAllContentsAssetBundle(targetSpriteAtlas);
            }
        }

        #endregion

        #endregion
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetBundleEditorTools.AssetBundleSet
{
    /// <summary>
    /// AssetBundleName的一揽，并标记设定的资产
    /// </summary>
    public class AssetBundleNameListWindow : EditorWindow
    {
        [MenuItem(AssetBundleMenuItems.OpenABNameAssetListWindows, false, 900)]
        public static void OpenWindow()
        {
            window = GetWindow<AssetBundleNameListWindow>();
            window.minSize = new Vector2(400f, 300f);
        }

        private static AssetBundleNameListWindow window;
        Vector2 scroll, ecludeScroll;
        private static Dictionary<string, List<Object>> abNameDict;
        private bool[] fold;
        private string filter = "", excludeFilter = "";
        private string hideCountText = "";
        private int hideCount;

        private void OnGUI()
        {
            GUILayout.Space(20f);
            HeaderButtons();
            GUILayout.Space(20f);
            FilterField();
            GUILayout.Space(20f);
        }

        private void HeaderButtons()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("项目内的所有asset检查", GUILayout.Width(150f)))
            {
                LoadAbName();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("删除不使用的ABName", GUILayout.Width(150f)))
            {
                AssetDatabase.RemoveUnusedAssetBundleNames();
            }

            EditorGUILayout.EndHorizontal();
            DrawList();
        }

        /// <summary>
        /// AssetBundle Name 的获取和设定 其资产的提取
        /// </summary>
        private void LoadAbName()
        {
            var abNames = AssetDatabase.GetAllAssetBundleNames();
            if (abNameDict == null)
            {
                abNameDict = new Dictionary<string, List<Object>>();
            }

            abNameDict.Clear();

            foreach (var abName in abNames)
            {
                abNameDict.Add(abName, new List<Object>());
            }

            fold = new bool[abNameDict.Keys.Count];
            var allAssets = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < allAssets.Length; ++i)
            {
                if (EditorUtility.DisplayCancelableProgressBar("", "", (float)i / (float)allAssets.Length))
                {
                    break;
                }

                var importer = AssetImporter.GetAtPath(allAssets[i]);
                if (string.IsNullOrEmpty(importer.assetBundleName))
                {
                    continue;
                }

                if (!abNameDict.ContainsKey(importer.assetBundleName))
                {
                    continue;
                }

                abNameDict[importer.assetBundleName].Add(AssetDatabase.LoadAssetAtPath<Object>(importer.assetPath));
            }

            EditorUtility.ClearProgressBar();
        }

        private void FilterField()
        {
            // 筛选对象的输入
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("filter", GUILayout.Width(80f));
            filter = EditorGUILayout.TextField(filter);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // 排除对象的输入
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("excludeFilter", GUILayout.Width(80f));
            excludeFilter = EditorGUILayout.TextField(excludeFilter);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();


            // hit件数的限定
            EditorGUILayout.BeginHorizontal();
            hideCountText = EditorGUILayout.TextField(hideCountText, GUILayout.Width(20f));
            string text;
            if (int.TryParse(hideCountText, out hideCount))
            {
                text = "不显示未满件的name";
            }
            else
            {
                text = "没有指定为表示的件数";
                hideCount = 0;
            }

            EditorGUILayout.LabelField(text);
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 結果を折りたたみリストで
        /// </summary>
        private void DrawList()
        {
            if (abNameDict == null || abNameDict.Count == 0 || fold == null)
            {
                // データ取得前はごめん
                EditorGUILayout.LabelField("nothing abnames.");
                return;
            }

            int count = abNameDict.Keys.Count;
            string[] keys = new string[count];
            abNameDict.Keys.CopyTo(keys, 0);

            bool enableFilter = !string.IsNullOrEmpty(filter);

            List<string> excludedUnuseKeys = new List<string>();
            string[] excludeKeys = excludeFilter.Split(',');


            EditorGUILayout.LabelField("assetBundleName (ref count)");
            EditorGUILayout.BeginVertical("box");
            scroll = EditorGUILayout.BeginScrollView(scroll);

            for (int i = 0; i < count; ++i)
            {
                var key = keys[i];

                if (enableFilter && !key.Contains(filter))
                {
                    continue;
                }

                // 手动输入的排除key
                if (!string.IsNullOrEmpty(excludeFilter))
                {
                    var exclude = excludeKeys.Where(t => !string.IsNullOrEmpty(t)).Any(t => key.Contains(t));

                    if (exclude)
                    {
                        continue;
                    }
                }

                // 未被引用到的key排除
                var refCount = abNameDict[key].Count;
                if (refCount < hideCount)
                {
                    excludedUnuseKeys.Add(key);
                    continue;
                }

                // abName (资产数量)
                fold[i] = EditorGUILayout.Foldout(fold[i], $"{key} ({refCount})");

                if (fold[i])
                {
                    EditorGUI.indentLevel++;
                    var objs = abNameDict[key];
                    foreach (var obj in objs)
                    {
                        EditorGUILayout.ObjectField(obj, typeof(Object), false);
                    }

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            // 实际上，如果有被排除的abName，只要在另外一个列表中排列就行了
            if (excludedUnuseKeys.Count > 0)
            {
                EditorGUILayout.BeginVertical("box", GUILayout.MinHeight(80f));
                EditorGUILayout.LabelField($"除外({excludedUnuseKeys.Count})");
                ecludeScroll = EditorGUILayout.BeginScrollView(ecludeScroll);

                EditorGUI.indentLevel++;
                excludedUnuseKeys.ForEach(k => { EditorGUILayout.LabelField($"{k}"); }
                );
                EditorGUI.indentLevel--;

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }
        }
    }
}
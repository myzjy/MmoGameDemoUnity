﻿using System.Collections.Generic;
using Framework.AssetBundles.Config;
using UnityEditor;
using UnityEngine;
using ZJYFrameWork.AssetBundles;

// ReSharper disable once InvalidXmlDocComment
/// <summary>
/// added by wsh @ 2017.12.29
/// 功能：Assetbundle编辑器支持，方便调试用
/// </summary>

// ReSharper disable once CheckNamespace
namespace AssetBundles
{
    [CustomEditor(typeof(AssetBundleManager), true)]
    public class AssetBundleManagerEditor : Editor
    {
        static protected string[] displayTypes = new string[] {
        "Resident", "AssetBundles Caching", "Assets Caching",
        "Web Requesting", "Web Requester Queue", "Prosessing Web Requester",
        "Prosessing AssetBundle AsyncLoader", "Prosessing Asset AsyncLoader",
    };

        private static int _selectedTypeIndex = 6;
        private static readonly Dictionary<string, bool> AbItemSate = new Dictionary<string, bool>();
        private static readonly Dictionary<string, bool> RefrenceSate = new Dictionary<string, bool>();
        // ReSharper disable once InconsistentNaming
        private static readonly Dictionary<string, bool> _dependenciesSate = new Dictionary<string, bool>();
        // ReSharper disable once InconsistentNaming
        private static readonly Dictionary<string, bool> abRefrenceSate = new Dictionary<string, bool>();
        // ReSharper disable once InconsistentNaming
        private static readonly Dictionary<string, bool> webRequestRefrenceSate = new Dictionary<string, bool>();
        // ReSharper disable once InconsistentNaming
        private static readonly Dictionary<string, bool> AbLoaderSate = new Dictionary<string, bool>();

        private static void ClearStates()
        {
            AbItemSate.Clear();
            RefrenceSate.Clear();
            _dependenciesSate.Clear();
            abRefrenceSate.Clear();
            webRequestRefrenceSate.Clear();
            AbLoaderSate.Clear();
        }
        
        public void OnEnable()
        {
            EditorApplication.update += Update;
        }

        public void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        public void Update()
        {
            Repaint();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("DisplayType:", GUILayout.MaxWidth(80f));
            var newSelectedTypeIndex = EditorGUILayout.Popup(_selectedTypeIndex, displayTypes);
            EditorGUILayout.EndHorizontal();

            if (newSelectedTypeIndex != _selectedTypeIndex)
            {
                ClearStates();
            }
            _selectedTypeIndex = newSelectedTypeIndex;
            OnRefresh(_selectedTypeIndex);
        }

        public void OnRefresh(int selectedTypeIndex)
        {
            if (!AssetBundleConfig.IsSimulateMode)
            {
                return;
            }

            switch (selectedTypeIndex)
            {
                case 0:
                    {
                        OnDrawAssetBundleResident();
                        break;
                    }
                case 1:
                    {
                        OnDrawAssetBundleCaching();
                        break;
                    }
                case 2:
                    {
                        OnDrawAssetCaching();
                        break;
                    }
                case 3:
                    {
                        OnDrawWebRequesting();
                        break;
                    }
                case 4:
                    {
                        OnDrawWebRequesterQueue();
                        break;
                    }
                case 5:
                    {
                        OnDrawProcessingWebRequester();
                        break;
                    }
                case 6:
                    {
                        OnDrawProcessingAssetBundleAsyncLoader();
                        break;
                    }
                case 7:
                    {
                        OnDrawProcessingAssetAsyncLoader();
                        break;
                    }
            }
        }

        protected void DrawAssetbundleRefrences(string assetbundleName, string key, int level = 0)
        {
            var instance = AssetBundleManager.Instance;
            var abRefrences = instance.GetAssetBundleRefraction(assetbundleName);
            var webRequestRefrences = instance.GetWebRequesterRefraction(assetbundleName);
            var abLoaderRefrences = instance.GetAssetBundleLoaderRefraction(assetbundleName);
            var expanded = false;

            expanded = GUILayoutUtils.DrawSubHeader(level + 1, "ABRefrence:", abRefrenceSate, key, abRefrences.Count.ToString());
            if (expanded && abRefrences.Count > 0)
            {
                GUILayoutUtils.DrawTextListContent(abRefrences);
            }

            expanded = GUILayoutUtils.DrawSubHeader(level + 1, "WebRequester:", webRequestRefrenceSate, key, webRequestRefrences.Count.ToString());
            if (expanded && webRequestRefrences.Count > 0)
            {
                GUILayoutUtils.DrawTextListContent(webRequestRefrences, "Sequence : ");
            }

            expanded = GUILayoutUtils.DrawSubHeader(level + 1, "ABLoader:", AbLoaderSate, key, abLoaderRefrences.Count.ToString());
            if (expanded && abLoaderRefrences.Count > 0)
            {
                GUILayoutUtils.DrawTextListContent(abLoaderRefrences, "Sequence : ");
            }
        }

        protected void DrawAssetbundleContent(string assetbundleName, string key, int level)
        {
            var instance = AssetBundleManager.Instance;
            var expanded = false;
            GUILayoutUtils.BeginContents(false);

            var loaded = instance.GetAssetBundleCache(assetbundleName);
            EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(250f));
            EditorGUILayout.LabelField("", GUILayout.MinWidth(20 * level));
            GUILayoutUtils.DrawProperty("Has Loaded:", loaded ? "true" : "false");
            EditorGUILayout.EndHorizontal();

            var referencesCount = instance.GetAssetBundleRefractionCount(assetbundleName);
            expanded = GUILayoutUtils.DrawSubHeader(level, "References Count:", RefrenceSate, key, referencesCount.ToString());
            if (expanded)
            {
                DrawAssetbundleRefrences(assetbundleName, key, level);
            }

            var dependencies = instance.GetCurManifest.GetAllDependencies(assetbundleName);
            var dependenciesCount = instance.GetAssetbundleDependenciesCount(assetbundleName);
            expanded = GUILayoutUtils.DrawSubHeader(level, "Dependencies Count:", _dependenciesSate, key, dependenciesCount.ToString());
            if (expanded && dependenciesCount > 0)
            {
                for (int i = 0; i < dependencies.Length; i++)
                {
                    var dependence = dependencies[i];
                    if (!string.IsNullOrEmpty(dependence) && dependence != assetbundleName)
                    {
                        DrawAssetbundleItem(dependence, dependence, AbItemSate, assetbundleName + dependence, level + 1);
                    }
                }
            }

            GUILayoutUtils.EndContents(false);
        }

        protected void DrawAssetbundleItem(string title, string assetbundleName, Dictionary<string, bool> states, string key, int level = 0)
        {
            var instance = AssetBundleManager.Instance;
            if (instance.IsAssetBundleLoaded(assetbundleName))
            {
                title += "[loaded]";
            }

            if (level == 0)
            {
                if (GUILayoutUtils.DrawHeader(title, states, key, false, false))
                {
                    DrawAssetbundleContent(assetbundleName, key, level);
                }
            }
            else
            {
                if (GUILayoutUtils.DrawSubHeader(level, title, states, key, ""))
                {
                    DrawAssetbundleContent(assetbundleName, key, level + 1);
                }
            }
        }

        protected void OnDrawAssetBundleResident()
        {
            var instance = AssetBundleManager.Instance;
            var resident = instance.GetAssetBundleResident();
            using var iter = resident.GetEnumerator();
            EditorGUILayout.BeginVertical();
            while (iter.MoveNext())
            {
                var assetbundleName = iter.Current;
                DrawAssetbundleItem(assetbundleName, assetbundleName, AbItemSate, assetbundleName);
            }
            EditorGUILayout.EndVertical();
        }

        private void OnDrawAssetBundleCaching()
        {
            var instance = AssetBundleManager.Instance;
            var assetBundleCaching = instance.GetAssetBundleCaching();
            using var iter = assetBundleCaching.GetEnumerator();
            EditorGUILayout.BeginVertical();
            while (iter.MoveNext())
            {
                var assetBundleName = iter.Current;
                DrawAssetbundleItem(assetBundleName, assetBundleName, AbItemSate, assetBundleName);
            }
            EditorGUILayout.EndVertical();
        }

        protected static void OnDrawAssetCaching()
        {
            var instance = AssetBundleManager.Instance;
            var assetCaching = instance.GetAssetCaching();
            var totalCount = instance.GetAssetCachingCount();
            using var iter = assetCaching.GetEnumerator();
            EditorGUILayout.BeginVertical();
            GUILayoutUtils.DrawProperty("Total loaded assets count : ", totalCount.ToString());
            while (iter.MoveNext())
            {
                var assetbundleName = iter.Current.Key;
                var assetNameList = iter.Current.Value;
                string title = string.Format("{0}[{1}]", assetbundleName, assetNameList.Count);
                if (GUILayoutUtils.DrawHeader(title, AbItemSate, assetbundleName, false, false))
                {
                    GUILayoutUtils.DrawTextListContent(assetNameList);
                }
            }
            EditorGUILayout.EndVertical();
        }

        protected void OnDrawWebRequesting()
        {
            var instance = AssetBundleManager.Instance;
            var webRequesting = instance.GetWebRequesting();
            using var iter = webRequesting.GetEnumerator();
            EditorGUILayout.BeginVertical();
            while (iter.MoveNext())
            {
                var assetbundleName = iter.Current.Key;
                var webRequester = iter.Current.Value;
                string title = string.Format("Sequence : {0} --- {1}", webRequester.Sequence, assetbundleName);
                DrawAssetbundleItem(title, assetbundleName, AbItemSate, assetbundleName);
            }
            EditorGUILayout.EndVertical();
        }

        protected void OnDrawWebRequesterQueue()
        {
            var instance = AssetBundleManager.Instance;
            var requesterQueue = instance.GetWebRequestQueue();
            using var iter = requesterQueue.GetEnumerator();
            EditorGUILayout.BeginVertical();
            while (iter.MoveNext())
            {
                if (iter.Current != null)
                {
                    var assetBundleName = iter.Current.assetbundleName;
                    var webRequester = iter.Current;
                    string title = $"Sequence : {webRequester.Sequence} --- {assetBundleName}";
                    DrawAssetbundleItem(title, assetBundleName, AbItemSate, assetBundleName);
                }
            }
            EditorGUILayout.EndVertical();
        }

        protected void OnDrawProcessingWebRequester()
        {
            var instance = AssetBundleManager.Instance;
            var processing = instance.GetProsesWebRequester();
            using var iter = processing.GetEnumerator();
            EditorGUILayout.BeginVertical();
            while (iter.MoveNext())
            {
                if (iter.Current != null)
                {
                    var assetBundleName = iter.Current.assetbundleName;
                    var webRequester = iter.Current;
                    var title = $"Sequence : {webRequester.Sequence} --- {assetBundleName}";
                    DrawAssetbundleItem(title, assetBundleName, AbItemSate, assetBundleName);
                }
            }
            EditorGUILayout.EndVertical();
        }

        protected void OnDrawProcessingAssetBundleAsyncLoader()
        {
            var instance = AssetBundleManager.Instance;
            var processing = instance.GetProsesAssetBundleAsyncLoader();
            using var iter = processing.GetEnumerator();
            EditorGUILayout.BeginVertical();
            while (iter.MoveNext())
            {
                if (iter.Current == null) continue;
                var assetBundleName = iter.Current.assetbundleName;
                var loader = iter.Current;
                var title = $"Sequence : {loader.Sequence} --- {assetBundleName}";
                DrawAssetbundleItem(title, assetBundleName, AbItemSate, assetBundleName);
            }
            EditorGUILayout.EndVertical();
        }

        private void OnDrawProcessingAssetAsyncLoader()
        {
            var instance = AssetBundleManager.Instance;
            var processing = instance.GetProsesAssetAsyncLoader();
            using var iter = processing.GetEnumerator();
            EditorGUILayout.BeginVertical();
            while (iter.MoveNext())
            {
                if (iter.Current == null) continue;
                var assetName = iter.Current.AssetName;
                var loader = iter.Current;
                var title = $"Sequence : {loader.Sequence} --- {assetName}";
                var assetBundleName = instance.GetAssetBundleName(assetName);
                DrawAssetbundleItem(title, assetBundleName, AbItemSate, assetBundleName);
            }
            EditorGUILayout.EndVertical();
        }
    }
}

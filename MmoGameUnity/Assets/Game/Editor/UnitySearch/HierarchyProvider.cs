using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace UnitySearch
{
    internal static class TransformExtension
    {
        internal static List<Transform> SearchChild(this Transform self, string search)
        {
            var finds = new List<Transform>();
            FindDataRecursive(ref finds, search, self);

            return finds;
        }
		
        private static void FindDataRecursive(ref List<Transform> finds, string search, Transform parent)
        {
            if (parent.name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                finds.Add(parent);
				
            foreach (Transform child in parent.transform)
                FindDataRecursive(ref finds, search, child);
        }
		
        internal static string Path(this Transform self)
        {
            var builder = new System.Text.StringBuilder();
            var tran = self;
            while (tran.parent != null)
            {
                var parent = tran.parent;
                builder.Insert(0, parent.name + "/");
                tran = parent;
            }
            builder.Append(self.name);
            builder.Insert(0, self.gameObject.scene.name + ":");
            return builder.ToString();
        }
    }
    public class HierarchyProvider : Provider
    {
        private static readonly string Tag = "hi";

        internal override string GetTag()
        {
            return Tag;
        }

        /// <summary>
        /// 全検索だから重い
        /// </summary>
        internal override IEnumerable<UnitySearchTreeViewItem> SearchItems(string searchText)
        {
            var finds = new List<Transform>();
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                foreach (var gameObject in UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects())
                    finds.AddRange(gameObject.transform.SearchChild(searchText));
            }

            if (finds.Count < 0)
                return new List<UnitySearchTreeViewItem>();

            return finds.Select(t => GetItem(t.Path()));
        }

        internal override UnitySearchTreeViewItem GetItem(string path)
        {
            var split = path.Split(UnitySearchHierarchyLog.Separator);
            var scene = split[0];
            var paths = split[1].Split('/');

            return new UnitySearchTreeViewItem(this, path, split[1])
            {
                displayName = paths.Last(),
                SubDisplayName = string.IsNullOrEmpty(scene)
                    ? split[1]
                    : $"[{scene}] {split[1]}",
                icon = UnitySearchConstant.HierarchyTexture,
            };
        }

        internal override void Action(string path)
        {
            var obj = GameObject.Find(path);
            if (obj != null)
                Selection.activeTransform = obj.transform;
        }

        public override bool TagSearchOnly()
        {
            return true;
        }
    }
}
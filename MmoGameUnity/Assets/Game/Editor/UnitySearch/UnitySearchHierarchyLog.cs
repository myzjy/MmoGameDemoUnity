using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnitySearch
{
    public static class UnitySearchHierarchyLog
    {
        internal static IEnumerable<string> Logs
        {
            get { return UnitySearchHierarchyLogData.instance.Logs; }
        }

        internal static readonly char Separator = ':'; 

        [InitializeOnLoadMethod]
        private static void Init()
        {
            Selection.selectionChanged += SelectionChanged;
        }

        private static void SelectionChanged()
        {
            if (Selection.transforms == null)
                return;
			
            UnitySearchHierarchyLogData.instance.Add(Selection.transforms);
        }

        private class UnitySearchHierarchyLogData : ScriptableSingleton<UnitySearchHierarchyLogData>
        {
            [SerializeField]
            internal List<string> Logs = new List<string>();
			
            private const int MAX = 50;
			
            internal void Add(params Transform[] transforms)
            {
                foreach (var transform in transforms)
                {
                    var scene = transform.gameObject.scene.name;
                    var path = GetPath(transform);

                    var key = scene + Separator + path;

                    if (Logs.Contains(key))
                        Logs.Remove(key);
						
                    Logs.Insert(0, key);
                }
				
                while (Logs.Count > MAX)
                    Logs.RemoveAt(MAX);
            }
			
            private string GetPath(Transform transform)
            {
                var builder = new StringBuilder();
                var tran = transform;
                while (tran.parent != null)
                {
                    builder.Insert(0, tran.parent.name + "/");
                    tran = tran.parent;
                }

                builder.Append(transform.name);
                return builder.ToString();
            }
        }
    }
}
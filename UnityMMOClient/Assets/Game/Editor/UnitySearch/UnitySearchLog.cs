using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnitySearch
{
    public static class UnitySearchLog
    {
        internal static IEnumerable<UnitySearchLogItem> Logs
        {
            get { return UnitySearchLogData.instance.Commands; }
        }

        internal static void AddLog(UnitySearchTreeViewItem item)
        {
            UnitySearchLogData.instance.Add(new UnitySearchLogItem(item));
        }
		
        private class UnitySearchLogData : ScriptableSingleton<UnitySearchLogData>
        {
            [SerializeField]
            internal List<UnitySearchLogItem> Commands = new List<UnitySearchLogItem>();
			
            private const int MAX = 50;

            internal void Add(UnitySearchLogItem item)
            {
                if (Commands.Contains(item))
                    Commands.Remove(item);
				
                Commands.Insert(0, item);
				
                while (Commands.Count > MAX)
                    Commands.RemoveAt(MAX);
            }
        }
    }

    [Serializable]
    internal class UnitySearchLogItem
    {
        public string TypeName;
        public string Key;

        public UnitySearchLogItem(UnitySearchTreeViewItem item)
        {
            TypeName = item.TypeName;
            Key = item.FullKey;
        }
    }
}
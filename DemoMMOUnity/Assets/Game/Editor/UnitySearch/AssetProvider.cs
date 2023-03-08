using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace UnitySearch
{
    public class AssetProvider : Provider
    {
        private static readonly string Tag = "as";

        internal override string GetTag()
        {
            return Tag;
        }

        internal override IEnumerable<UnitySearchTreeViewItem> SearchItems(string searchText)
        {
            var tagIndex = searchText.IndexOf(UnitySearch.TAG_KEYWORD, StringComparison.Ordinal);
            if (tagIndex > 0)
            {
                var type = searchText.Substring(0, tagIndex);
                searchText = searchText.Substring(tagIndex + 1);
                searchText = $"t:{type} {searchText}";
            }

            return AssetDatabase.FindAssets(searchText).Take(SearchMaxCount()).Select(GetItem);
        }

        internal override UnitySearchTreeViewItem GetItem(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            return new UnitySearchTreeViewItem(this, guid)
            {
                displayName = System.IO.Path.GetFileNameWithoutExtension(path),
                SubDisplayName = path,
                icon = AssetDatabase.GetCachedIcon(path) as Texture2D,
                CanDrag = true,
            };
        }

        internal override void Action(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            Selection.objects = new[] {AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path)};
        }
		
        public override void DragAndDrop(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);

            var dragObjects = new[] {AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path)};
            UnityEditor.DragAndDrop.PrepareStartDrag();
            UnityEditor.DragAndDrop.paths = null;
            UnityEditor.DragAndDrop.objectReferences = dragObjects.ToArray();
            UnityEditor.DragAndDrop.SetGenericData("AssetProvider", dragObjects);
            UnityEditor.DragAndDrop.StartDrag(dragObjects.Length == 1 ? dragObjects[0].name : "<Multiple>");
        }
    }
}
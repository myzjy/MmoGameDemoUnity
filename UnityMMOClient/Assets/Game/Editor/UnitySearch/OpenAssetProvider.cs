using UnityEditor;
using UnityEngine;

namespace UnitySearch
{
    public class OpenAssetProvider : AssetProvider
    {
        private static readonly string Tag = "aso";

        internal override string GetTag()
        {
            return Tag;
        }
		
        public override bool TagSearchOnly()
        {
            return true;
        }

        internal override void Action(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);

            if (AssetDatabase.IsValidFolder(path))
                EditorUtility.RevealInFinder(path);
            else
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<Object>(path));
        }
    }
}
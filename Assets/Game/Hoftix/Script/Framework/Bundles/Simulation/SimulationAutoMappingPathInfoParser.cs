#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ZJYFrameWork.AssetBundles.Bundles
{
    public class SimulationAutoMappingPathInfoParser : IPathInfoParser
    {
        private Dictionary<string, string> dict = new Dictionary<string, string>();

        public SimulationAutoMappingPathInfoParser()
        {
            this.Initialize();
        }

        protected virtual void Initialize()
        {
            Regex regex = new Regex("^assets/", RegexOptions.IgnoreCase);
            foreach (string bundleName in AssetDatabaseHelper.GetUsedAssetBundleNames())
            {
                var assets = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
                for (int i = 0; i < assets.Length; i++)
                {
                    var assetPath = assets[i].ToLower();
                    var key = regex.Replace(assetPath, "");
                    dict[key] = Path.GetFilePathWithoutExtension(bundleName).ToLower();
                }
            }
        }

        public virtual AssetPathInfo Parse(string path)
        {
            string bundleName;
            if (!this.dict.TryGetValue(path.ToLower(), out bundleName))
            {
                Debug.Log("Not found the AssetBundle,please check the configuration of the asset '{0}'.", path);
                return null;
            }

            return new AssetPathInfo(bundleName, path);
        }
    }
}
#endif
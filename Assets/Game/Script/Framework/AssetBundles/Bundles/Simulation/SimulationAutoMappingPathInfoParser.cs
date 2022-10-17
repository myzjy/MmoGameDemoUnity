#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.AssetBundles.Bundles
{
    public sealed class SimulationAutoMappingPathInfoParser : IPathInfoParser
    {
        private Dictionary<string, string> dict = new Dictionary<string, string>();

        public SimulationAutoMappingPathInfoParser()
        {
            this.Initialize();
        }

        public void Initialize()
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

        public AssetPathInfo Parse(string path)
        {
            if (this.dict.TryGetValue(path.ToLower(), out var bundleName)) return new AssetPathInfo(bundleName, path);
            Debug.Log("Not found the AssetBundle,please check the configuration of the asset '{0}'.", path);
            return null;

        }

        public BundleManifest BundleManifest { get; set; }
    }
}
#endif
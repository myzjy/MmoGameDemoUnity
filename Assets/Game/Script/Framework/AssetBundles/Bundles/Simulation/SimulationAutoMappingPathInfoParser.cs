#if UNITY_EDITOR
using System;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Framework.AssetBundles.Config;
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
            foreach (var bundleName in AssetDatabaseHelper.GetUsedAssetBundleNames())
            {
                var assets = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
                foreach (var t in assets)
                {
                    var assetPath = t.ToLower();
                    var key = regex.Replace(assetPath, "");
                    var path = bundleName; //Path.GetFilePathWithoutExtension(bundleName).ToLower();
                    //path --> playerhuanzhuang_spriteatlas.assetbundle
                    //key --> game/assetbundle/ui/uispriteatlas/playerhuanzhuang.spriteatlas
                    dict[key] = path;
                }
            }
        }

        public AssetPathInfo Parse(string path)
        {
            string bundleName = string.Empty;
            try
            {
                var firstOrDefault = dict.Where(a => a.Value == path.ToLower());
                if (path.EndsWith(AssetBundleConfig.SpriteAtlasABSuffix))
                {
                    string newPath = path.Replace(AssetBundleConfig.SpriteAtlasABSuffix,
                        AssetBundleConfig.SpriteAtlasSuffix);
                    var dictDefault = firstOrDefault.FirstOrDefault(a => a.Key.Contains(newPath));
                    bundleName = dictDefault.Key;
                }
                else
                {
                    var first = firstOrDefault.FirstOrDefault();

                    bundleName = first.Key;
                }
            }
            catch (Exception e)
            {
                //代表没找到
              
                    Debug.Log("没有找到资产包，请检查资产的配置 '{0}'.", path);
                return null;
            }
            // if (!this.dict.TryGetValue(path.ToLower(), out var bundleName))
            // {
            //     if (log.IsWarnEnabled)
            //         log.WarnFormat("没有找到资产包，请检查资产的配置 '{0}'.", path);
            //     return null;
            // }

            return new AssetPathInfo(path, bundleName);

        }

        public BundleManifest BundleManifest { get; set; }
    }
}
#endif
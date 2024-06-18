using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FrostEngine;
using ZJYFrameWork.AssetBundles.AssetBundleToolsConfig;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.AssetBundles.Bundles
{
    public sealed class AutoMappingPathInfoParser : IPathInfoParser, IManifestUpdatable
    {
        private BundleManifest bundleManifest;
        private Dictionary<string, string> dict = new Dictionary<string, string>();
        public AutoMappingPathInfoParser()
        {
        }
        public AutoMappingPathInfoParser(BundleManifest manifest)
        {
            this.BundleManifest = manifest;
        }

        public BundleManifest BundleManifest
        {
            get { return this.bundleManifest; }
            set
            {
                // if (this.bundleManifest == value)
                //     return;

                this.bundleManifest = value;
                this.Initialize();
            }
        }

        public void Initialize()
        {
            if (this.dict != null)
                this.dict.Clear();

            if (this.dict == null)
                this.dict = new Dictionary<string, string>();
            if (this.bundleManifest == null)
            {
                return;
            }
            Regex regex = new Regex("^assets/", RegexOptions.IgnoreCase);
            BundleInfo[] bundleInfos = this.bundleManifest.GetAll();
            foreach (BundleInfo info in bundleInfos)
            {
                if (!info.Published)
                    continue;

                var assets = info.Assets;
                foreach (var t in assets)
                {
                    // var assetPath = t.ToLower();
                    // var key = regex.Replace(assetPath, "");
                    // dict[key] = info.Name;
                    var assetPath = t;
                    if (!info.IsStreamedScene)
                    {
                        assetPath = t.ToLower();
                    }

                    var assetName = regex.Replace(assetPath, "");
                    var bundleName = $"{info.Name}.{info.Variant}";
                    //bunlename --> playerhuanzhuang_spriteatlas.assetbundle
                    //assetName --> game/assetbundle/ui/uispriteatlas/playerhuanzhuang.spriteatlas
                    dict[assetName] = bundleName;
                }
            }
        }

        public AssetPathInfo Parse(string path)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"开始查找 容器中 [{path}] Bundle");
#endif

            string assetName;
            try
            {
                var firstOrDefault = dict.Where(a => a.Value == path.ToLower());
                if (path.EndsWith(AssetBundleConfig.SpriteAtlasABSuffix))
                {
                    //去除后缀
                    var newPath = path.Replace(AssetBundleConfig.SpriteAtlasABSuffix,
                        AssetBundleConfig.SpriteAtlasSuffix);
                    var dictDefault = firstOrDefault.FirstOrDefault(a => a.Key.Contains(newPath));
                    assetName = dictDefault.Key;
                }
                else
                {
                    var first = firstOrDefault.FirstOrDefault();

                    assetName = first.Key;
                }
            }
            catch (Exception)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                //代表没找到
                Debug.LogError($"没有找到资产包，请检查资产的配置 '{path}'.");
#endif
                return null;
            }

            return new AssetPathInfo(path, assetName);
        }
    }
}

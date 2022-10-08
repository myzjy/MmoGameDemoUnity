using System.Collections.Generic;
using System.Text.RegularExpressions;
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
                if (this.bundleManifest == value)
                    return;

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
                    var assetPath = t.ToLower();
                    var key = regex.Replace(assetPath, "");
                    dict[key] = info.Name;
                }
            }
        }

        public AssetPathInfo Parse(string path)
        {
            if (!this.dict.TryGetValue(path.ToLower(), out var bundleName))
                return null;

            return new AssetPathInfo(bundleName, path);
        }
    }
}

#if UNITY_EDITOR
using ZJYFrameWork.AssetBundles.EditorAssetBundle.Archives;
using System.Collections.Generic;

namespace ZJYFrameWork.AssetBundles.EditorAssetBundle.Objects
{
    public class AssetBundleManifest : UnityDynamicObject
    {
        public AssetBundleManifest(TypeTree tree) : base(tree)
        {
        }

        public virtual string[] GetAllAssetBundles()
        {
            Map map = (Map)this["AssetBundleNames"];
            List<string> bundleNames = new List<string>();
            foreach (var kv in map)
            {
                bundleNames.Add((string)kv.Value);
            }
            return bundleNames.ToArray();
        }
    }
}
#endif
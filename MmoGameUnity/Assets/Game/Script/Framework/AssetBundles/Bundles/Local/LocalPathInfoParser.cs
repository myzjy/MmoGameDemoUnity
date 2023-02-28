namespace ZJYFrameWork.AssetBundles.Bundles
{
    public class LocalPathInfoParser : IPathInfoParser
    {
        public virtual AssetPathInfo Parse(string path)
        {
            return new AssetPathInfo("", path);
        }

        public BundleManifest BundleManifest { get; set; }
        public void Initialize()
        {
            throw new System.NotImplementedException();
        }
    }
}

#if UNITY_EDITOR
using ZJYFrameWork.AssetBundles.EditorAssetBundle.Archives;
using System.Collections.Generic;
using System.Text;

namespace ZJYFrameWork.AssetBundles.EditorAssetBundle.Objects
{
    public class PreloadData
    {
        public string Name { get; set; }
        public readonly List<PPtr> Preloads = new List<PPtr>();
        public readonly List<string> Dependencies = new List<string>();

        public ObjectArchive Archive { get; private set; }

        public PreloadData(ObjectArchive archive)
        {
            this.Archive = archive;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("PreloadData { ");
            buf.AppendFormat("Name : {0}, ", this.Name);
            buf.AppendFormat("Archive : {0} ", this.Archive);
            buf.Append("}");
            return buf.ToString();
        }
    }
}
#endif
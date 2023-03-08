using ZJYFrameWork.Asynchronous;

#if UNITY_EDITOR
namespace ZJYFrameWork.AssetBundles.EditorAssetBundle.Redundancy
{
    public interface IRedundancyAnalyzer
    {
        IProgressResult<float, RedundancyReport> AnalyzeRedundancy();
    }
}
#endif
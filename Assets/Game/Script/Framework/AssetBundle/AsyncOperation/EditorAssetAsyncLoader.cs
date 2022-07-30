// ReSharper disable once InvalidXmlDocComment
/// <summary>
/// added by wsh @ 2017.12.22
/// 2022.5.28
/// 功能：assetbundle在simulate模式下的Asset模拟异步加载器 
/// </summary>
namespace Framework.AssetBundle.AsyncOperation
{
    public class EditorAssetAsyncLoader : BaseAssetAsyncLoader
    {
        public EditorAssetAsyncLoader(UnityEngine.Object obj)
        {
            asset = obj;
        }

        public override void Update()
        {
        }

        public override bool IsDone()
        {
            return true;
        }

        public override float Progress()
        {
            return 1.0f;
        }
    }
}
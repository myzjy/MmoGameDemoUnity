//
// using System.IO;
// using Framework.AssetBundle.AsyncOperation;
//
// namespace ZJYFrameWork.AssetBundles
// {
//     public class NetworkAssetBundleLoaderImpl:BundleFileAssetBundleLoaderImpl
//     {
//         public override string CreateUrl(AssetBundleLoaderBase loader, string path, string hash)
//         {
//             var url = Path.Combine(loader.GetAssetBaseUrl(), path);
//             if(string.IsNullOrEmpty(hash) && loader.Initialized)
//             {
//                 //找到hash值
//                 hash = loader.GetCustomManifest().GetAssetBundleVersion(path);
//             }
//             //组成请求url
//             return $"{url}?v={hash}";
//         }
//     }
// }
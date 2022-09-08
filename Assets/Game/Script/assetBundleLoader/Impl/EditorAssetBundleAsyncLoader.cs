// using System;
// using System.Collections;
// using System.IO;
// using Framework.AssetBundles.Utilty;
// using UnityEditor;
// using UnityEngine.SceneManagement;
// using ZJYFrameWork;
// using ZJYFrameWork.AssetBundleLoader;
// using Object = UnityEngine.Object;
//
// namespace Framework.AssetBundle.AsyncOperation
// {
//     /// <summary>
//     /// zjy
//     /// 2022.5.28
//     /// 功能：assetBundle在simulate模式下的Asset模拟加载器 
//     /// </summary>
//     public class EditorAssetBundleAsyncLoader : AssetBundleLoaderImpl
//     {
//         public override bool IsInitialized(AssetBundleLoaderBase loader)
//         {
//             return true;
//         }
//
//         public override bool RequestEmptyHolder(string name, AssetBundleLoaderBase.OnLoadedDelegate onLoaded)
//         {
//             if (onLoaded != null)
//             {
//                 onLoaded(new AssetBundleLoaderBase.AssetBundleHolder(name));
//             }
//
//             return true;
//         }
//
//         public override bool RequireManifest()
//         {
//             return false;
//         }
//
//         public override Object LoadAsset(UnityEngine.AssetBundle assetBundle, string assetBundleName, string name,
//             Type type)
//         {
//             var paths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
//
//             for (int i = 0; i < paths.Length; i++)
//             {
//                 UnityEngine.Debug.Log(Path.GetFileNameWithoutExtension(paths[i]));
//                 if (string.Equals(Path.GetFileNameWithoutExtension(paths[i]), assetBundleName,
//                         StringComparison.OrdinalIgnoreCase))
//                 {
// #if UNITY_EDITOR && DEVELOP_BUILD
//
// #endif
//                     UnityEngine.Object target = AssetDatabase.LoadAssetAtPath(paths[i], type);
//                     return target;
//                 }
//             }
//
//             Debug.LogError($"不存在下一个资产包.\n {assetBundleName} 内の {name}");
//
//             return null;
//         }
//
//         public override string CreateUrl(AssetBundleLoaderBase loader, string path, string hash)
//         {
//             return path;
//         }
//
//         public override void LoadScene(UnityEngine.AssetBundle assetBundle, string assetBundleName, string name,
//             LoadSceneMode mode)
//         {
//             base.LoadScene(assetBundle, assetBundleName, name, mode);
//         }
//
//         public override void LoadAssetAsync(AssetBundleLoaderBase loader, UnityEngine.AssetBundle assetBundle,
//             string assetBundleName, string name,
//             Type type, Action<Object> callback)
//         {
//             var obj = LoadAsset(assetBundle, assetBundleName, name, type);
//             loader.StartCoroutine(EmulateLoadAssetAsync(obj, callback));
//         }
//         IEnumerator EmulateLoadAssetAsync(Object asset, Action<Object> callback)
//         {
//             yield return null;
//             callback(asset);
//         }
//     }
// }
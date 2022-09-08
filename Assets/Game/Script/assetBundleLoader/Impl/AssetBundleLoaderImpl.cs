// using System;
// using System.Collections;
// using Framework.AssetBundles.Config;
// using UnityEngine;
// using UnityEngine.Assertions;
// using UnityEngine.SceneManagement;
// using Object = UnityEngine.Object;
//
// namespace ZJYFrameWork.AssetBundleLoader
// {
//     public abstract class AssetBundleLoaderImpl : IAssetBundleLoaderImpl
//     {
//         // public 
//         public virtual bool IsInitialized(AssetBundleLoaderBase loader)
//         {
//             throw new NotImplementedException();
//         }
//
//         public virtual bool RequestEmptyHolder(string name, AssetBundleLoaderBase.OnLoadedDelegate onLoaded)
//         {
//             return false;
//         }
//
//         public virtual bool RequireManifest()
//         {
//             return true;
//         }
//
//         public virtual string GetCustomManifestName(AssetBundleLoaderBase loader, string hash)
//         {
// #if UNITY_ANDROID
//             return $"{AssetBundleConfig.CustomManifestPrefix}/android_{AssetBundleConfig.AssetBundleSuffix}";
// #elif UNITY_IOS
//             return $"{AssetBundleConfig.CustomManifestPrefix}/ios_{AssetBundleConfig.AssetBundleSuffix}";
// #else
//             return $"{AssetBundleConfig.CustomManifestPrefix}/pc_{AssetBundleConfig.AssetBundleSuffix}{loader.GetBundleExt()}";
// #endif
//         }
//
//         public virtual Object LoadAsset(AssetBundle assetBundle, string assetBundleName, string name, Type type)
//         {
//             Debug.Assert(assetBundle != null);
//             if (assetBundle == null)
//             {
//                 return null;
//             }
//
//             var asset = type == typeof(Object)
//                 ? assetBundle.LoadAsset(assetBundleName)
//                 : assetBundle.LoadAsset(name, type);
//             Assert.IsNull(asset, $"不存在下一个资产包。{assetBundleName}内的{name}不存在");
//             OnLoadAsset(asset);
//             return asset;
//         }
//
//         public abstract string CreateUrl(AssetBundleLoaderBase loader, string path, string hash);
//
//
//         public virtual void OnLoadAsset(Object asset)
//         {
//         }
//
//         public virtual void LoadScene(AssetBundle assetBundle, string assetBundleName, string name, LoadSceneMode mode)
//         {
//             throw new NotImplementedException();
//         }
//
//         public virtual void LoadAssetAsync(AssetBundleLoaderBase loader, AssetBundle assetBundle,
//             string assetBundleName, string name,
//             Type type, Action<Object> callback)
//         {
//             Debug.Assert(assetBundle != null);
//             if (assetBundle == null)
//             {
//                 callback(null);
//                 return;
//             }
//
//             loader.StartCoroutine(_LoadAssetAsync(assetBundle, name, type, callback));
//         }
//
//         IEnumerator _LoadAssetAsync(AssetBundle bundle, string name, Type type, Action<Object> callback)
//         {
//             var op = bundle.LoadAssetAsync(name, type);
//             yield return op;
//             var asset = op.asset;
//             Debug.Assert(asset != null);
//             OnLoadAsset(asset);
//             callback(asset);
//         }
//     }
// }
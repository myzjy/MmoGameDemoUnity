using UnityEngine;
using UnityEngine.SceneManagement;
using ZJYFrameWork.Asynchronous;
using System.Collections.Generic;

namespace ZJYFrameWork.AssetBundles.Bundles
{
    /// <summary>
    /// A common interface for the asset loader.
    /// </summary>
    public interface IResources:IBundleManager
    {
        void SetIPathAndBundleResource(IPathInfoParser pathInfo, IBundleManager manager);

        /// <summary>
        /// Synchronously loads binary data.The related AssetBundle must already be loaded, otherwise returns null.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        byte[] LoadData(string path);

        /// <summary>
        /// Synchronously loads text data.The related AssetBundle must already be loaded, otherwise returns null.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string LoadText(string path);

        /// <summary>
        /// Synchronously loads an asset.The related AssetBundle must already be loaded, otherwise returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        T LoadAsset<T>(string path) where T : Object;

        /// <summary>
        /// Synchronously loads an asset.The related AssetBundle must already be loaded, otherwise returns null.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Object LoadAsset(string path, System.Type type);

        /// <summary>
        /// 同步加载资源。相关的AssetBundle必须已经被加载，否则返回null。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Object LoadAsset(string path);

        /// <summary>
        ///  Asynchronously loads an asset.If the Assetbundle isn't loaded, automatic loading the Assetbundle.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        IProgressResult<float, T> LoadAssetAsync<T>(string path) where T : Object;

        /// <summary>
        /// Asynchronously loads an asset.If the Assetbundle isn't loaded, automatic loading the Assetbundle.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IProgressResult<float, Object> LoadAssetAsync(string path, System.Type type);

        /// <summary>
        /// Asynchronously loads an asset.If the Assetbundle isn't loaded, automatic loading the Assetbundle.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IProgressResult<float, Object> LoadAssetAsync(string path);

        /// <summary>
        /// Synchronously loads a group of assets.The related AssetBundle must already be loaded, otherwise returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paths"></param>
        /// <returns></returns>
        T[] LoadAssets<T>(params string[] paths) where T : Object;

        /// <summary>
        /// Synchronously loads a group of assets.The related AssetBundle must already be loaded, otherwise returns null.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        Object[] LoadAssets(System.Type type, params string[] paths);

        /// <summary>
        /// Synchronously loads a group of assets.The related AssetBundle must already be loaded, otherwise returns null.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        Object[] LoadAssets(params string[] paths);

        /// <summary>
        /// Synchronously loads a group of assets.The related AssetBundle must already be loaded, otherwise returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paths"></param>
        /// <returns></returns>
        Dictionary<string, T> LoadAssetsToMap<T>(params string[] paths) where T : Object;

        /// <summary>
        /// Synchronously loads a group of assets.The related AssetBundle must already be loaded, otherwise returns null.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        Dictionary<string, Object> LoadAssetsToMap(System.Type type, params string[] paths);

        /// <summary>
        /// Synchronously loads a group of assets.The related AssetBundle must already be loaded, otherwise returns null.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        Dictionary<string, Object> LoadAssetsToMap(params string[] paths);

        /// <summary>
        /// Asynchronously loads a group of assets.If the Assetbundle isn't loaded, automatic loading the Assetbundle.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paths"></param>
        /// <returns></returns>
        IProgressResult<float, T[]> LoadAssetsAsync<T>(params string[] paths) where T : Object;

        /// <summary>
        /// Asynchronously loads a group of assets.If the Assetbundle isn't loaded, automatic loading the Assetbundle.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        IProgressResult<float, Object[]> LoadAssetsAsync(System.Type type, params string[] paths);

        /// <summary>
        /// Asynchronously loads a group of assets.If the Assetbundle isn't loaded, automatic loading the Assetbundle.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        IProgressResult<float, Object[]> LoadAssetsAsync(params string[] paths);

        /// <summary>
        /// Asynchronously loads a group of assets.If the Assetbundle isn't loaded, automatic loading the Assetbundle.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paths"></param>
        /// <returns></returns>
        IProgressResult<float, Dictionary<string, T>> LoadAssetsToMapAsync<T>(params string[] paths) where T : Object;

        /// <summary>
        /// Asynchronously loads a group of assets.If the Assetbundle isn't loaded, automatic loading the Assetbundle.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        IProgressResult<float, Dictionary<string, Object>>
            LoadAssetsToMapAsync(System.Type type, params string[] paths);

        /// <summary>
        /// Asynchronously loads a group of assets.If the Assetbundle isn't loaded, automatic loading the Assetbundle.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        IProgressResult<float, Dictionary<string, Object>> LoadAssetsToMapAsync(params string[] paths);

#if SUPPORT_LOADALL
        /// <summary>
        /// Synchronously loads all of the assets for the given bundle's name.The related AssetBundle must already be loaded, otherwise returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        T[] LoadAllAssets<T>(string bundleName) where T : Object;

        /// <summary>
        /// Synchronously loads all of the assets for the given bundle's name.The related AssetBundle must already be loaded, otherwise returns null.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Object[] LoadAllAssets(string bundleName, System.Type type);

        /// <summary>
        /// Synchronously loads all of the assets for the given bundle's name.The related AssetBundle must already be loaded, otherwise returns null.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        Object[] LoadAllAssets(string bundleName);

        /// <summary>
        /// Asynchronously loads all of the assets for the given bundle's name.If the Assetbundle isn't loaded, automatic loading the Assetbundle.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        IProgressResult<float, T[]> LoadAllAssetsAsync<T>(string bundleName) where T : Object;

        /// <summary>
        /// Asynchronously loads all of the assets for the given bundle's name.If the Assetbundle isn't loaded, automatic loading the Assetbundle.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IProgressResult<float, Object[]> LoadAllAssetsAsync(string bundleName, System.Type type);

        /// <summary>
        /// Asynchronously loads all of the assets for the given bundle's name.If the Assetbundle isn't loaded, automatic loading the Assetbundle.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        IProgressResult<float, Object[]> LoadAllAssetsAsync(string bundleName);
#endif
        /// <summary>
        /// Asynchronously loads a scene for the given scene's name.If the Assetbundle isn't loaded, automatic loading the Assetbundle.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        ISceneLoadingResult<Scene> LoadSceneAsync(string path, LoadSceneMode mode = LoadSceneMode.Single);

        /// <summary>
        /// 异步读取本地场景
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        ISceneLoadingResult<UnityEngine.SceneManagement.Scene> LoadLocalSceneAsync(string path,
            LoadSceneMode mode = LoadSceneMode.Single);
        
        /// <summary>
        /// Gets a bundle for the given bundle's name.If the Assetbundle isn't loaded, returns null.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        new IBundle GetBundle(string bundleName);

        /// <summary>
        /// Asynchronously loads a bundle for the given bundle's name.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        new IProgressResult<float, IBundle> LoadBundle(string bundleName);

        /// <summary>
        /// Asynchronously loads a bundle for the given bundle's name.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="priority">Positive or negative, the default value is 0.When multiple asynchronous operations are queued up, the operation with the higher priority will be executed first. Once an operation has been started on the background thread, changing the priority will have no effect anymore.</param>
        /// <returns></returns>
        new IProgressResult<float, IBundle> LoadBundle(string bundleName, int priority);

        /// <summary>
        /// Asynchronously loads a group of bundles for the given bundle's names.
        /// </summary>
        /// <param name="bundleNames"></param>
        /// <returns></returns>
        new IProgressResult<float, IBundle[]> LoadBundle(params string[] bundleNames);

        /// <summary>
        /// Asynchronously loads a group of bundles for the given bundle's names.
        /// </summary>
        /// <param name="bundleNames"></param>
        /// <param name="priority">Positive or negative, the default value is 0.When multiple asynchronous operations are queued up, the operation with the higher priority will be executed first. Once an operation has been started on the background thread, changing the priority will have no effect anymore.</param>
        /// <returns></returns>
        new IProgressResult<float, IBundle[]> LoadBundle(string[] bundleNames, int priority);
        
    }
}
// using System;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using Object = UnityEngine.Object;
//
// namespace ZJYFrameWork.AssetBundleLoader
// {
//     public interface IAssetBundleLoaderImpl
//     {
//         /// <summary>
//         /// 初始化
//         /// </summary>
//         /// <param name="loader">基础类</param>
//         /// <returns></returns>
//         bool IsInitialized(AssetBundleLoaderBase loader);
//
//         /// <summary>
//         /// 请求空架包
//         /// </summary>
//         /// <param name="name"></param>
//         /// <param name="onloaded"></param>
//         /// <returns></returns>
//         // ReSharper disable once IdentifierTypo
//         bool RequestEmptyHolder(string name, AssetBundleLoaderBase.OnLoadedDelegate onloaded);
//
//         /// <summary>
//         /// AssetBundle要求清单
//         /// </summary>
//         /// <returns></returns>
//         bool RequireManifest();
//
//         /// <summary>
//         /// 获取自定义清单名称
//         /// </summary>
//         /// <param name="loader">数据</param>
//         /// <param name="hash">hash值</param>
//         /// <returns></returns>
//         string GetCustomManifestName(AssetBundleLoaderBase loader, string hash);
//
//         /// <summary>
//         /// 读取资源
//         /// </summary>
//         /// <param name="assetBundle">资源</param>
//         /// <param name="assetBundleName">资源名</param>
//         /// <param name="name">名字</param>
//         /// <param name="type">类型</param>
//         /// <returns>返回解析到资源</returns>
//         Object LoadAsset(AssetBundle assetBundle, string assetBundleName, string name, Type type);
//
//         /// <summary>
//         /// 创建assetBundle的请求url
//         /// </summary>
//         /// <param name="loader"></param>
//         /// <param name="path">路径</param>
//         /// <param name="hash">hash值</param>
//         /// <returns>url</returns>
//         abstract string CreateUrl(AssetBundleLoaderBase loader, string path, string hash);
//
//         /// <summary>
//         /// 当读取资源时
//         /// </summary>
//         /// <param name="asset"></param>
//         void OnLoadAsset(UnityEngine.Object asset);
//
//         /// <summary>
//         /// 加载场景
//         /// </summary>
//         /// <param name="assetBundle"></param>
//         /// <param name="assetBundleName"></param>
//         /// <param name="name">场景名 </param>
//         /// <param name="mode">
//         /// <summary>
//         ///   <para>关闭所有当前加载的场景，并加载一个场景</para>
//         /// </summary>
//         /// Single,
//         /// <summary>
//         ///   <para>将场景添加到当前加载的场景</para>
//         /// </summary>
//         ///   Additive,
//         /// </param>
//         void LoadScene(AssetBundle assetBundle, string assetBundleName, string name, LoadSceneMode mode);
//
//         /// <summary>
//         /// 异步加载资源
//         /// </summary>
//         /// <param name="loader"></param>
//         /// <param name="assetBundle"></param>
//         /// <param name="assetBundleName"></param>
//         /// <param name="name"></param>
//         /// <param name="type">类型</param>
//         /// <param name="callback">事件回调</param>
//         void LoadAssetAsync(AssetBundleLoaderBase loader, AssetBundle assetBundle, string assetBundleName, string name,
//             Type type, Action<Object> callback);
//     }
// }
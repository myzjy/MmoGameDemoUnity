// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using ZJYFrameWork.AssetBundleLoader;
// using ZJYFrameWork.AssetBundles;
// using Object = UnityEngine.Object;
//
// namespace ZJYFrameWork
// {
//     public abstract partial class AssetBundleLoaderBase
//     {
//         /// <summary>
//         /// 外部AssetBundle的Holder.引用计数为0时Unload(假)
//         /// </summary>
//         public class AssetBundleHolder : IBundle
//         {
//             /// <summary>
//             /// 涵盖资源名，资产包
//             /// </summary>
//             public BaseAssetsAsyncLoader BaseAssetsAsyncLoader = new BaseAssetsAsyncLoader();
//
//             /// <summary>
//             /// 参考计数器
//             /// </summary>
//             public class ReferenceCount
//             {
//                 /// <summary>
//                 /// 锁
//                 /// </summary>
//                 private object guard = new object();
//
//                 public void AddRef()
//                 {
//                     lock (guard)
//                     {
//                         count++;
//                     }
//                 }
//
//                 public bool Release()
//                 {
//                     lock (guard)
//                     {
//                         --count;
//                         return count == 0;
//                     }
//                 }
//
//                 /// <summary>
//                 /// 引用基数
//                 /// </summary>
//                 private int count;
//             }
//
//             /// <summary>
//             /// 当参考计数器为0时释放
//             /// </summary>
//             ReferenceCount refCount;
//
//             /// <summary>
//             /// 依赖对象的资产包
//             /// </summary>
//             public List<AssetBundleHolder> dependecies =
//                 new List<AssetBundleHolder>();
//
//             /// <summary>
//             /// 卸载时丢弃吗? true
//             /// </summary>
//             public bool unloadAllLoadedObjects;
//
//             /// <summary>
//             /// 资产包的钥匙
//             /// </summary>
//             public uint key { get; private set; }
//
//             /// <summary>
//             /// 初始化
//             /// </summary>
//             /// <param name="ket">资产包的秘钥</param>
//             /// <param name="assetBundle">资源</param>
//             /// <param name="holders">list assetbundle</param>
//             /// <param name="unloadAllLoadedObjects">卸载时丢弃吗</param>
//             public AssetBundleHolder(uint ket, AssetBundle assetBundle, List<AssetBundleHolder> holders,
//                 bool unloadAllLoadedObjects)
//             {
//                 this.key = ket;
//                 Name = assetBundle.name;
//                 BaseAssetsAsyncLoader.assetbundle = assetBundle;
//                 BaseAssetsAsyncLoader.assetbundleName = assetBundle.name;
//                 this.unloadAllLoadedObjects = unloadAllLoadedObjects;
//                 dependecies = holders;
//                 refCount = new ReferenceCount();
//                 // m_AssetBundleConfigs = new AssetBundleConfigs(assetBundle, holders, unloadAllLoadedObjects);
//             }
//
//             /// <summary>
//             /// 
//             /// </summary>
//             /// <param name="holderSource"></param>
//             public AssetBundleHolder(AssetBundleHolder holderSource)
//             {
//                 key = holderSource.key;
//                 BaseAssetsAsyncLoader.assetbundleName = holderSource.BaseAssetsAsyncLoader.assetbundleName;
//                 BaseAssetsAsyncLoader.assetbundle = holderSource.BaseAssetsAsyncLoader.assetbundle;
//                 Name = holderSource.BaseAssetsAsyncLoader.assetbundleName;
//
//                 unloadAllLoadedObjects = holderSource.unloadAllLoadedObjects;
//                 dependecies = holderSource.dependecies;
//                 refCount = holderSource.refCount;
//                 refCount.AddRef();
//             }
//
//             public AssetBundleHolder(string assetName)
//             {
//                 BaseAssetsAsyncLoader.assetbundleName = assetName;
//                 Name = assetName;
//             }
//
//             /// <summary>
//             /// 析构 释放资源
//             /// </summary>
//             ~AssetBundleHolder()
//             {
//                 //当资源释放的时候 引用基数减1 直到归零
//                 if (refCount.Release())
//                 {
//                 }
//             }
//
//             /// <summary>
//             /// AssetBundle加载资源
//             /// AssetBundle的资源负载
//             /// </summary>
//             /// <param name="name">需要加载的AssetBundle资源名</param>
//             /// <returns>返回万物最基础的object 在外部进行 类型转换</returns>
//             public Object LoadAsset(string name)
//             {
//                 return LoadAsset(name, typeof(Object));
//             }
//
//             public string Name { get; }
//
//             public T LoadAsset<T>(string name) where T : Object
//             {
//                 throw new NotImplementedException();
//             }
//
//             public Object LoadAsset(string assetName, Type type)
//             {
//                 return Instance.GetAssetBundleLoaderImpl().LoadAsset(BaseAssetsAsyncLoader.assetbundle,
//                     BaseAssetsAsyncLoader.assetbundleName, assetName, type);
//             }
//
//             public void LoadScene(string name, LoadSceneMode mode)
//             {
//                 Instance.GetAssetBundleLoaderImpl().LoadScene(BaseAssetsAsyncLoader.assetbundle, BaseAssetsAsyncLoader.assetbundleName, name, mode);
//             }
//
//             /// <summary>
//             /// AssetBundle的异步加载模式
//             /// </summary>
//             /// <param name="assetName">资源名</param>
//             /// <param name="type">资源类型</param>
//             /// <param name="callObjBack">返回时的事件</param>
//             public void LoadAssetAsync(string assetName, Type type, Action<Object> callObjBack)
//             {
//                 var loader = Instance;
//                 loader.GetAssetBundleLoaderImpl().LoadAssetAsync(loader, BaseAssetsAsyncLoader.assetbundle,
//                     BaseAssetsAsyncLoader.assetbundleName, assetName, type, callObjBack);
//             }
//         }
//     }
// }
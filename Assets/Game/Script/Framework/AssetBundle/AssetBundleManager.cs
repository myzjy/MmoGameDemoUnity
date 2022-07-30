using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssetBundles.Config;
using Common.Utility;
using Framework.AssetBundle.AsyncOperation;
using Framework.AssetBundle.Config;
using Framework.AssetBundles.Config;
using Framework.AssetBundles.Utilty;
using GameTools.Singletons;
using Script.Config;
using UnityEditor;
using UnityEngine;

namespace Script.Framework.AssetBundle
{
    public class AssetBundleManager : MMOSingletonDontDestroy<AssetBundleManager>
    {
        /// <summary>
        /// 最大同时进行的ab创建数量
        /// </summary>
        private const int MAX_ASSETBUNDLE_CREATE_NUM = 5;

        /// <summary>
        /// manifest 用于提供依赖关系连
        /// </summary>
        private Manifest manifest = null;

        /// <summary>
        ///资源路径映射表
        /// </summary>
        private AssetsPathMapping assetsPathMapping = null;

        // 常驻ab包：需要手动添加公共ab包进来，常驻包不会自动卸载（即使引用计数为0），引用计数为0时可以手动卸载
        HashSet<string> assetbundleResident = new HashSet<string>();

        // ab缓存包：所有目前已经加载的ab包，包括临时ab包与公共ab包
        Dictionary<string, UnityEngine.AssetBundle> assetbundlesCaching =
            new Dictionary<string, UnityEngine.AssetBundle>();

        // ab缓存包引用计数：卸载ab包时只有引用计数为0时才会真正执行卸载
        Dictionary<string, int> assetbundleRefCount = new Dictionary<string, int>();

        // asset缓存：给非公共ab包的asset提供逻辑层的复用
        Dictionary<string, UnityEngine.Object> assetsCaching = new Dictionary<string, UnityEngine.Object>();

        // 加载数据请求：正在prosessing或者等待prosessing的资源请求
        Dictionary<string, ResourceWebRequester> webRequesting = new Dictionary<string, ResourceWebRequester>();

        // 等待处理的资源请求
        Queue<ResourceWebRequester> webRequesterQueue = new Queue<ResourceWebRequester>();

        // 正在处理的资源请求
        List<ResourceWebRequester> prosessingWebRequester = new List<ResourceWebRequester>();

        // 逻辑层正在等待的ab加载异步句柄
        List<AssetBundleAsyncLoader> prosessingAssetBundleAsyncLoader = new List<AssetBundleAsyncLoader>();

        // 逻辑层正在等待的asset加载异步句柄
        List<AssetAsyncLoader> prosessingAssetAsyncLoader = new List<AssetAsyncLoader>();

        // 为了消除GC
        List<string> tmpStringList = new List<string>(8);

        /// <summary>
        /// 资产包名称
        /// </summary>
        public static string ManifestBundleName
        {
            get { return BuildUtils.ManifestBundleName; }
        }

        public override void OnAwake()
        {
        }

        void Update()
        {
            OnProsessingWebRequester();
            OnProsessingAssetBundleAsyncLoader();
            OnProsessingAssetAsyncLoader();
        }

        public IEnumerator Initialize()
        {
#if UNITY_EDITOR
            if (AssetBundleConfig.IsEditorMode)
            {
                yield break;
            }
#endif

            manifest = new Manifest();
            //创建依赖路径 map
            assetsPathMapping = new AssetsPathMapping();
            // 说明：同时请求资源可以提高加载速度
            var manifestRequest = RequestAssetBundleAsync(manifest.AssetbundleName);
            var pathMapRequest = RequestAssetBundleAsync(assetsPathMapping.AssetbundleName);
            Debug.Log(assetsPathMapping.AssetbundleName);

            yield return manifestRequest;
            var assetbundle = manifestRequest.assetbundle;
            manifest.LoadFromAssetBundle(assetbundle);
            assetbundle.Unload(false);
            manifestRequest.Dispose();

            yield return pathMapRequest;
            assetbundle = pathMapRequest.assetbundle;
            var s = assetbundle.LoadAllAssets();
            var mapContent = s[0] as TextAsset;// assetbundle.LoadAsset<TextAsset>(assetsPathMapping.AssetName);
            if (mapContent != null)
            {
                assetsPathMapping.Initialize(mapContent.text);
            }

            assetbundle.Unload(true);
            pathMapRequest.Dispose();

            // 设置所有公共包为常驻包
            var start = DateTime.Now;
            var allAssetbundleNames = manifest.GetAllAssetBundleNames();
            foreach (var curAssetbundleName in allAssetbundleNames)
            {
                if (string.IsNullOrEmpty(curAssetbundleName))
                {
                    continue;
                }

                int count = 0;
                foreach (var checkAssetbundle in allAssetbundleNames)
                {
                    if (checkAssetbundle == curAssetbundleName || string.IsNullOrEmpty(checkAssetbundle))
                    {
                        continue;
                    }

                    var allDependencies = manifest.GetAllDependencies(checkAssetbundle);
                    if (Array.IndexOf(allDependencies, curAssetbundleName) < 0) continue;
                    count++;
                    if (count >= 2)
                    {
                        break;
                    }
                }

                // 说明：设置被依赖数量为1的AB包为常驻包的理由详细情况见AssetBundleAsyncLoader.cs那一大堆注释
                // TODO：1）目前已知Unity5.3版本和Unity5.5版本没问题，其它试过的几个版本都有问题，如果你使用的版本也有问题，需要修改这里的宏
                //       2）整套AB包括压缩格式可能都要重新设计，这个以后有时间再去尝试
#if UNITY_5_5_OR_NEWER
                if (count >= 1)
#else
                if (count >= 2)
#endif
                {
                    SetAssetBundleResident(curAssetbundleName, true);
                }
            }

            ToolsDebug.Log($"AssetBundleResident Initialize use {(DateTime.Now - start).Milliseconds}ms");
            yield break;
        }

        public IEnumerator Cleanup()
        {
#if UNITY_EDITOR
            if (AssetBundleConfig.IsEditorMode)
            {
                yield break;
            }
#endif

            // 等待所有请求完成
            // 要是不等待Unity很多版本都有各种Bug
            yield return new WaitUntil(() => !IsProsessRunning);

            ClearAssetsCache();
            foreach (var assetbunle in assetbundlesCaching.Values.Where(assetbunle => assetbunle != null))
            {
                assetbunle.Unload(false);
            }

            assetbundlesCaching.Clear();
            assetbundleRefCount.Clear();
            assetbundleResident.Clear();
            yield break;
        }

        public Manifest curManifest
        {
            get { return manifest; }
        }

        public void ClearAssetsCache()
        {
            assetsCaching.Clear();
        }

        public override void Disable()
        {
        }

        // 本地异步请求Assetbundle资源，不计引用计数、不缓存，Creater使用后记得回收
        public ResourceWebRequester RequestAssetBundleAsync(string assetbundleName)
        {
            var creater = ResourceWebRequester.Get();
            var url = AssetBundleUtility.GetAssetBundleFileUrl(assetbundleName);
            creater.Init(assetbundleName, url, true);
            webRequesting.Add(assetbundleName, creater);
            webRequesterQueue.Enqueue(creater);
            return creater;
        }

        public void SetAssetBundleResident(string assetbundleName, bool resident)
        {
            ToolsDebug.Log("SetAssetBundleResident : " + assetbundleName + ", " + resident.ToString());
            bool exist = assetbundleResident.Contains(assetbundleName);
            if (resident && !exist)
            {
                assetbundleResident.Add(assetbundleName);
            }
            else if (!resident && exist)
            {
                assetbundleResident.Remove(assetbundleName);
            }
        }

        public ResourceWebRequester GetAssetBundleAsyncCreater(string assetbundleName)
        {
            webRequesting.TryGetValue(assetbundleName, out var creater);
            return creater;
        }

        /// <summary>
        /// 获取到资源包缓存
        /// </summary>
        /// <param name="assetbundleName">资产包</param>
        /// <returns>资源包缓存</returns>
        public UnityEngine.AssetBundle GetAssetBundleCache(string assetbundleName)
        {
            //out 参数，获取到对应数据
            assetbundlesCaching.TryGetValue(assetbundleName, out var target);
            return target;
        }

        /// <summary>
        /// 添加资源缓存
        /// </summary>
        /// <param name="assetName">资产包</param>
        /// <param name="asset">资产</param>
        public void AddAssetCache(string assetName, UnityEngine.Object asset)
        {
            assetsCaching[assetName] = asset;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public UnityEngine.Object GetAssetCache(string assetName)
        {
            assetsCaching.TryGetValue(assetName, out var target);
            return target;
        }

        /// <summary>
        /// 资产加载
        /// </summary>
        /// <param name="assetName">资产</param>
        /// <returns>返回是否加载<para>true<c>---已加载</c></para><para>false<c>---未加载</c></para></returns>
        public bool IsAssetLoaded(string assetName)
        {
            return assetsCaching.ContainsKey(assetName);
        }

        /// <summary>
        /// 是否加载了资产包
        /// </summary>
        /// <param name="assetbundleName">资产包</param>
        /// <returns>返回是否加载<para>true<c>---已加载</c></para><para>false<c>---未加载</c></para></returns>
        public bool IsAssetBundleLoaded(string assetbundleName)
        {
            return assetbundlesCaching.ContainsKey(assetbundleName);
        }

        /// <summary>
        /// 添加资产包资产缓存
        /// </summary>
        /// <param name="assetbundleName">资产包的名字</param>
        /// <param name="postfix">后缀</param>
        public void AddAssetbundleAssetsCache(string assetbundleName, string postfix = null)
        {
#if UNITY_EDITOR
            //是否编辑器模式
            if (AssetBundleConfig.IsEditorMode)
            {
                return;
            }
#endif
            if (!IsAssetBundleLoaded(assetbundleName))
            {
                ToolsDebug.LogError($"Try to add assets cache from unloaded assetbundle :  {assetbundleName}");
                return;
            }

            //获取对应资源包的缓存
            var curAssetbundle = GetAssetBundleCache(assetbundleName);
            //获取到资源包的各项依赖
            var allAssetNames = assetsPathMapping.GetAllAssetNames(assetbundleName);
            //循环添加
            foreach (var assetName in allAssetNames)
            {
                //判断
                if (IsAssetLoaded(assetName))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(postfix) && !assetName.EndsWith(postfix))
                {
                    continue;
                }

                //获取到包的路径到资源路径
                var assetPath = AssetBundleUtility.PackagePathToAssetsPath(assetName);
                //读取到资源
                var asset = curAssetbundle == null ? null : curAssetbundle.LoadAsset(assetPath);
                //添加缓存
                AddAssetCache(assetName, asset);

#if UNITY_EDITOR
                // 说明：在Editor模拟时，Shader要重新指定
                var go = asset as GameObject;
                if (go == null) continue;
                var renderers = go.GetComponentsInChildren<Renderer>();
                foreach (var t in renderers)
                {
                    var mat = t.sharedMaterial;
                    if (mat == null)
                    {
                        continue;
                    }

                    var shader = mat.shader;
                    if (shader == null) continue;
                    var shaderName = shader.name;
                    mat.shader = Shader.Find(shaderName);
                }
#endif
            }
        }

        public bool IsProsessRunning
        {
            get
            {
                return prosessingWebRequester.Count != 0 || prosessingAssetBundleAsyncLoader.Count != 0 ||
                       prosessingAssetAsyncLoader.Count != 0;
            }
        }

        // 从资源服务器下载Assetbundle资源，非AB（不计引用计数、不缓存），Creater使用后记得回收
        public ResourceWebRequester DownloadAssetBundleAsync(string filePath)
        {
            // 如果ResourceWebRequester升级到使用UnityWebRequester，那么下载AB和下载普通资源需要两个不同的DownLoadHandler
            // 兼容升级的可能性，这里也做一下区分
            return DownloadAssetFileAsync(filePath);
        }

        // 从资源服务器下载非Assetbundle资源，非AB（不计引用计数、不缓存），Creater使用后记得回收
        public ResourceWebRequester DownloadAssetFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(DownloadUrl))
            {
                ToolsDebug.LogError("You should set download url first!!!");
                return null;
            }

            var creater = ResourceWebRequester.Get();
            var url = DownloadUrl + filePath;
            creater.Init(filePath, url, true);
            webRequesting.Add(filePath, creater);
            webRequesterQueue.Enqueue(creater);
            return creater;
        }

        /// <summary>
        /// 下载url路径
        /// </summary>
        public string DownloadUrl
        {
            get { return URLSetting.SERVER_RESOURCE_URL; }
        }

        // 异步请求Assetbundle资源，AB是否缓存取决于是否设置为常驻包，Assets一律缓存，处理依赖
        public BaseAssetBundleAsyncLoader LoadAssetBundleAsync(string assetbundleName)
        {
#if UNITY_EDITOR
            if (AssetBundleConfig.IsEditorMode)
            {
                return new EditorAssetBundleAsyncLoader(assetbundleName);
            }
#endif

            var loader = AssetBundleAsyncLoader.Get();
            prosessingAssetBundleAsyncLoader.Add(loader);
            if (manifest != null)
            {
                string[] dependancies = manifest.GetAllDependencies(assetbundleName);
                for (int i = 0; i < dependancies.Length; i++)
                {
                    var dependance = dependancies[i];
                    if (!string.IsNullOrEmpty(dependance) && dependance != assetbundleName)
                    {
                        CreateAssetBundleAsync(dependance);
                        // A依赖于B，A对B持有引用
                        IncreaseReferenceCount(dependance);
                    }
                }

                loader.Init(assetbundleName, dependancies);
            }
            else
            {
                loader.Init(assetbundleName, null);
            }

            CreateAssetBundleAsync(assetbundleName);
            // 加载器持有的引用：同一个ab能同时存在多个加载器，等待ab创建器完成
            IncreaseReferenceCount(assetbundleName);
            return loader;
        }

        protected bool CreateAssetBundleAsync(string assetbundleName)
        {
            if (IsAssetBundleLoaded(assetbundleName) || webRequesting.ContainsKey(assetbundleName))
            {
                return false;
            }

            var creater = ResourceWebRequester.Get();
            var url = AssetBundleUtility.GetAssetBundleFileUrl(assetbundleName);
            creater.Init(assetbundleName, url);
            webRequesting.Add(assetbundleName, creater);
            webRequesterQueue.Enqueue(creater);
            // 创建器持有的引用：创建器对每个ab来说是全局唯一的
            IncreaseReferenceCount(assetbundleName);
            return true;
        }

        protected int IncreaseReferenceCount(string assetbundleName)
        {
            assetbundleRefCount.TryGetValue(assetbundleName, out var count);
            count++;
            assetbundleRefCount[assetbundleName] = count;
            return count;
        }

        protected int DecreaseReferenceCount(string assetbundleName)
        {
            assetbundleRefCount.TryGetValue(assetbundleName, out var count);
            count--;
            if (count <= 0)
            {
                assetbundleRefCount.Remove(assetbundleName);
            }
            else
            {
                assetbundleRefCount[assetbundleName] = count;
            }

            return count;
        }

        protected int GetReferenceCount(string assetbundleName)
        {
            assetbundleRefCount.TryGetValue(assetbundleName, out var count);
            return count;
        }

        private void RemoveAssetBundleCache(string assetbundleName)
        {
            assetbundlesCaching.Remove(assetbundleName);
        }

        protected void AddAssetBundleCache(string assetbundleName, UnityEngine.AssetBundle assetbundle)
        {
            assetbundlesCaching[assetbundleName] = assetbundle;
        }

        public bool IsAssetBundleResident(string assebundleName)
        {
            return assetbundleResident.Contains(assebundleName);
        }

        public bool MapAssetPath(string assetPath,string assetBundleName, out string assetbundleName, out string assetName)
        {
            return assetsPathMapping.MapAssetPath(assetPath,assetBundleName, out assetbundleName, out assetName);
        }

        public BaseAssetAsyncLoader LoadAssetAsync(string assetPath, System.Type assetType)
        {
#if UNITY_EDITOR
            if (AssetBundleConfig.IsEditorMode)
            {
                string path = AssetBundleUtility.PackagePathToAssetsPath(assetPath);
                UnityEngine.Object target = AssetDatabase.LoadAssetAtPath(path, assetType);
                return new EditorAssetAsyncLoader(target);
            }
#endif
            var newAssetPath = $"{assetPath}{AssetBundleConfig.AssetBundleSuffix}";
            string assetbundleName = null;
            string assetName = null;
            bool status = MapAssetPath(newAssetPath, "",out assetbundleName, out assetName);
            if (!status)
            {
                ToolsDebug.LogError("No assetbundle at asset path :" + newAssetPath);
                return null;
            }

            var loader = AssetAsyncLoader.Get();
            prosessingAssetAsyncLoader.Add(loader);
            if (IsAssetLoaded(assetName))
            {
                loader.Init(assetName, GetAssetCache(assetName));
                return loader;
            }
            else
            {
                var assetbundleLoader = LoadAssetBundleAsync(assetbundleName);
                loader.Init(assetName, assetbundleLoader);
                return loader;
            }
        }

        protected bool UnloadAssetBundle(string assetbundleName, bool unloadResident = false,
            bool unloadAllLoadedObjects = false, bool unloadDependencies = true)
        {
            int count = GetReferenceCount(assetbundleName);
            if (count > 0)
            {
                // 存在引用，还是被需要的，不能卸载
                return false;
            }

            var assetbundle = GetAssetBundleCache(assetbundleName);
            var isResident = IsAssetBundleResident(assetbundleName);
            if (!isResident || (isResident && unloadResident))
            {
                if (assetbundle != null)
                {
                    assetbundle.Unload(unloadAllLoadedObjects);
                }

                RemoveAssetBundleCache(assetbundleName);
                if (!unloadDependencies || manifest == null) return true;
                string[] dependancies = manifest.GetAllDependencies(assetbundleName);
                foreach (var dependance in dependancies)
                {
                    if (string.IsNullOrEmpty(dependance) || dependance == assetbundleName) continue;
                    // 解除对依赖项持有的引用
                    int dependanceCount = DecreaseReferenceCount(dependance);
                    if (dependanceCount <= 0)
                    {
                        UnloadAssetBundle(dependance, unloadResident, unloadAllLoadedObjects, false);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnProsessingWebRequester()
        {
            for (int i = prosessingWebRequester.Count - 1; i >= 0; i--)
            {
                var creater = prosessingWebRequester[i];
                creater.Update();
                if (!creater.IsDone()) continue;
                prosessingWebRequester.RemoveAt(i);
                webRequesting.Remove(creater.assetbundleName);
                if (creater.noCache)
                {
                    // 无缓存，不计引用计数、Creater使用后由上层回收，所以这里不需要做任何处理
                }
                else
                {
                    // AB缓存
                    // 说明：有错误也缓存下来，只不过资源为空
                    // 1、避免再次错误加载
                    // 2、如果不存下来加载器将无法判断什么时候结束
                    AddAssetBundleCache(creater.assetbundleName, creater.assetbundle);

                    // 解除创建器对AB持有的引用，一般创建器存在，则一定至少有一个加载器在等待并对该AB持有引用
                    int count = DecreaseReferenceCount(creater.assetbundleName);
                    ToolsDebug.Assert(count > 0, "AssetBundle creater done but no one need it!!!");
                    if (count <= 0)
                    {
                        UnloadAssetBundle(creater.assetbundleName);
                    }

                    creater.Dispose();
                }
            }

            int slotCount = prosessingWebRequester.Count;
            while (slotCount < MAX_ASSETBUNDLE_CREATE_NUM && webRequesterQueue.Count > 0)
            {
                var creater = webRequesterQueue.Dequeue();
                creater.Start();
                prosessingWebRequester.Add(creater);
                slotCount++;
            }
        }

        private void OnProsessingAssetBundleAsyncLoader()
        {
            for (int i = prosessingAssetBundleAsyncLoader.Count - 1; i >= 0; i--)
            {
                var loader = prosessingAssetBundleAsyncLoader[i];
                loader.Update();
                if (!loader.IsDone()) continue;
                // 解除加载器对AB持有的引用
                int count = DecreaseReferenceCount(loader.assetbundleName);
                if (count <= 0)
                {
                    UnloadAssetBundle(loader.assetbundleName);
                }

                prosessingAssetBundleAsyncLoader.RemoveAt(i);
            }
        }

        private void OnProsessingAssetAsyncLoader()
        {
            for (int i = prosessingAssetAsyncLoader.Count - 1; i >= 0; i--)
            {
                var loader = prosessingAssetAsyncLoader[i];
                loader.Update();
                if (loader.IsDone())
                {
                    prosessingAssetAsyncLoader.RemoveAt(i);
                }
            }
        }

        // 本地异步请求非Assetbundle资源，非AB（不计引用计数、不缓存），Creater使用后记得回收
        public ResourceWebRequester RequestAssetFileAsync(string filePath, bool streamingAssetsOnly = true)
        {
            var creater = ResourceWebRequester.Get();
            string url = null;
            url = streamingAssetsOnly
                ? AssetBundleUtility.GetStreamingAssetsFilePath(filePath)
                : AssetBundleUtility.GetAssetBundleFileUrl(filePath);
            // ReSharper disable once InvalidXmlDocComment
            /**
             * @ name : 文件路径
             * @ url : 下载url
             * @ noCache : 缓存
             */
            creater.Init(filePath, url, true);
            webRequesting.Add(filePath, creater);
            webRequesterQueue.Enqueue(creater);
            return creater;
        }

        public void TestHotfix()
        {
#if UNITY_EDITOR || CLIENT_DEBUG
            ToolsDebug.Log("********** AssetBundleManager : Call TestHotfix in cs...");
#endif
        }

        public ResourceWebRequester DownloadWebResourceAsync(string url)
        {
            var creater = ResourceWebRequester.Get();
            creater.Init(url, url, true);
            webRequesting.Add(url, creater);
            webRequesterQueue.Enqueue(creater);
            return creater;
        }

 
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZJYFrameWork.AssetBundles.Bundles.ILoaderBuilderInterface;
using ZJYFrameWork.Asynchronous;
using ZJYFrameWork.Execution;
using ZJYFrameWork.Utilities;
using Object = UnityEngine.Object;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.AssetBundles.Bundles
{
#pragma warning disable 0414, 0219
    public abstract class AbstractResources : IResources, IDisposable
    {
        protected const float DefaultWeight = 0.8f;

        private readonly WeakValueDictionary<string, Object> _assetCache = new WeakValueDictionary<string, Object>();

        // ReSharper disable once InconsistentNaming
        protected IPathInfoParser _pathInfoParser;

        // ReSharper disable once InconsistentNaming
        protected IBundleManager bundleManager;
        private bool _useWeakCache;

        protected AbstractResources()
        {
        }

        protected AbstractResources(IPathInfoParser pathInfoParser, IBundleManager manager, bool useWeakCache)
        {
            this._pathInfoParser = pathInfoParser;
            this.bundleManager = manager;
            this._useWeakCache = useWeakCache;
        }

        protected virtual void AddCache<T>(string key, T obj) where T : Object
        {
            if (!_useWeakCache)
                return;

            this._assetCache[key] = obj;
        }

        protected virtual T GetCache<T>(string key) where T : Object
        {
            try
            {
                if (!_useWeakCache)
                    return null;

                if (this._assetCache.TryGetValue(key, out var value) && value != null && value is T o)
                {
                    //检查对象是否有效，因为它可能已被销毁。
                    //非托管对象，弱缓存不能准确跟踪对象的有效性。
                    return o;
                }

                return null;
            }
            catch (Exception)
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log($"事件解释缓存无效，对象[{key}]已销毁");
#endif
                return null;
            }
        }

        private WaitForSecondsRealtime _waitForSeconds;

        protected WaitForSecondsRealtime WaitForSeconds()
        {
#if UNITY_2018_4_OR_NEWER
            return _waitForSeconds ??= new WaitForSecondsRealtime(0.1f);
#else
            return new WaitForSecondsRealtime(0.1f);
#endif
        }

        public virtual IBundleManager BundleManager => this.bundleManager;

        public virtual IPathInfoParser PathInfoParser => this._pathInfoParser;

        #region IBundleManager支持

        public virtual ISceneLoadingResult<Scene> LoadLocalSceneAsync(string path,
            LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneLoadingResult<Scene> result = new SceneLoadingResult<Scene>();
            InterceptableEnumerator enumerator = new InterceptableEnumerator(DoLoadLocalSceneAsync(result, path, mode));
            enumerator.RegisterCatchBlock(e =>
            {
                result.SetException(e);
                Debug.LogError(e);
            });
            enumerator.RegisterFinallyBlock(() =>
            {
                if (!result.IsDone)
                {
                    result.SetException(new Exception("没有给出结果的值"));
                }
            });
            Executors.RunOnCoroutineNoReturn(enumerator);

            return result;
        }

        protected abstract IEnumerator DoLoadLocalSceneAsync(ISceneLoadingPromise<Scene> promise, string path,
            LoadSceneMode mode = LoadSceneMode.Single);

        public virtual IBundle GetBundle(string bundleName)
        {
            return this.bundleManager.GetBundle(bundleName);
        }

        public virtual IProgressResult<float, IBundle> LoadBundle(string bundleName)
        {
            return this.bundleManager.LoadBundle(bundleName);
        }

        public virtual IProgressResult<float, IBundle> LoadBundle(string bundleName, int priority)
        {
            return this.bundleManager.LoadBundle(bundleName, priority);
        }

        public virtual IProgressResult<float, IBundle[]> LoadBundle(params string[] bundleNames)
        {
            return this.bundleManager.LoadBundle(bundleNames);
        }

        public virtual IProgressResult<float, IBundle[]> LoadBundle(string[] bundleNames, int priority)
        {
            return this.bundleManager.LoadBundle(bundleNames, priority);
        }

        public void SetManifestAndLoadBuilder(BundleManifest manifest, ILoaderBuilder builder)
        {
            bundleManager.SetManifestAndLoadBuilder(manifest, builder);
        }

        #endregion

        #region IResource Support

        public void SetIPathAndBundleResource(IPathInfoParser pathInfo, IBundleManager manager)
        {
            this._pathInfoParser = pathInfo;
            this.bundleManager = manager;
            _useWeakCache = true;
        }

        public virtual byte[] LoadData(string path)
        {
            TextAsset text = this.LoadAsset<TextAsset>(path);
            if (text != null)
                return text.bytes;
            return null;
        }

        public virtual string LoadText(string path)
        {
            TextAsset text = this.LoadAsset<TextAsset>(path);
            if (text != null)
                return text.text;
            return null;
        }

        public virtual Object LoadAsset(string path)
        {
            return this.LoadAsset<Object>(path);
        }

        public virtual Object LoadAsset(string path, Type type)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path), "The path is null or empty!");
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            AssetPathInfo pathInfo = this._pathInfoParser.Parse(path);
            if (pathInfo == null)
            {
                throw new Exception($"没有找到AssetBundle或解析路径信息“{path}”失败.");
            }

            Object asset = this.GetCache<Object>(path);
            if (asset != null)
            {
                return asset;
            }

            using IBundle bundle = this.GetBundle(pathInfo.BundleName);
            if (bundle == null)
            {
                Debug.LogError("当前资产(路径:{})的资产包未加载，请先加载资产包.", path);
                return null;
            }

            asset = bundle.LoadAsset(pathInfo.AssetName, type);
            if (asset != null)
            {
                this.AddCache(path, asset);
            }

            return asset;
        }

        public virtual T LoadAsset<T>(string path) where T : Object
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path), "The path is null or empty!");
            }

            AssetPathInfo pathInfo = this._pathInfoParser.Parse(path);
            if (pathInfo == null)
            {
                throw new Exception($"未找到资源包或解析路径信息“{path}”失败。");
            }

            T asset = this.GetCache<T>(path);
            if (asset != null)
            {
                return asset;
            }

            using var bundle = this.GetBundle(pathInfo.BundleName);
            if (bundle == null)
            {
                Debug.LogError("当前资产(路径:{})的资产包未加载，请先加载资产包", path);
                return null;
            }

            asset = bundle.LoadAsset<T>(pathInfo.AssetName);
            if (asset != null)
            {
                this.AddCache(path, asset);
            }

            return asset;
        }

        public virtual IProgressResult<float, Object> LoadAssetAsync(string path)
        {
            return this.LoadAssetAsync<Object>(path);
        }

        public virtual IProgressResult<float, Object> LoadAssetAsync(string path, Type type)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    throw new ArgumentNullException(nameof(path), "路径为空或空!");
                }

                if (type == null)
                {
                    throw new ArgumentNullException(nameof(type));
                }

                var result = new ProgressResult<float, Object>();
                var pathInfo = this._pathInfoParser.Parse(path);
                if (pathInfo == null)
                {
                    throw new Exception($"没有找到AssetBundle或者解析路径信息 '{path}' 失败.");
                }

                Object asset = this.GetCache<Object>(path);
                if (asset != null)
                {
                    result.UpdateProgress(1f);
                    result.SetResult(asset);
                    return result;
                }

                var bundleResult = this.LoadBundle(pathInfo.BundleName);
                var weight = bundleResult.IsDone ? 0f : DefaultWeight;
                bundleResult.Callbackable().OnProgressCallback(p => result.UpdateProgress(p * weight));
                bundleResult.Callbackable().OnCallback((r) =>
                {
                    if (r.Exception != null)
                    {
                        result.SetException(r.Exception);
                        return;
                    }

                    using var bundle = r.Result;
                    IProgressResult<float, Object> assetResult = bundle.LoadAssetAsync(pathInfo.AssetName, type);
                    assetResult.Callbackable()
                        .OnProgressCallback(p => result.UpdateProgress(weight + (1f - weight) * p));
                    assetResult.Callbackable().OnCallback((ar) =>
                    {
                        if (ar.Exception != null)
                        {
                            result.SetException(ar.Exception);
                        }
                        else
                        {
                            result.SetResult(ar.Result);
                            this.AddCache(path, ar.Result);
                        }
                    });
                });
                return result;
            }
            catch (Exception e)
            {
                return new ImmutableProgressResult<float, Object>(e, 0f);
            }
        }

        public virtual IProgressResult<float, T> LoadAssetAsync<T>(string path) where T : Object
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    throw new ArgumentNullException(nameof(path), "路径为空或空!");
                }

                var result = new ProgressResult<float, T>();
                var pathInfo = this._pathInfoParser.Parse(path);
                if (pathInfo == null)
                {
                    throw new Exception($"没有找到AssetBundle或解析路径信息“{path}”失败.");
                }

                var asset = this.GetCache<T>(path);
                if (asset != null)
                {
                    result.UpdateProgress(1f);
                    result.SetResult(asset);
                    return result;
                }

                var bundleResult = this.LoadBundle(pathInfo.BundleName);
                var weight = bundleResult.IsDone ? 0f : DefaultWeight;
                bundleResult.Callbackable().OnProgressCallback(p => result.UpdateProgress(p * weight));
                bundleResult.Callbackable().OnCallback((r) =>
                {
                    if (r.Exception != null)
                    {
                        result.SetException(r.Exception);
                        return;
                    }

                    using var bundle = r.Result;
                    var assetResult = bundle.LoadAssetAsync<T>(pathInfo.AssetName);
                    assetResult.Callbackable()
                        .OnProgressCallback(p => result.UpdateProgress(weight + (1f - weight) * p));
                    assetResult.Callbackable().OnCallback((ar) =>
                    {
                        if (ar.Exception != null)
                            result.SetException(ar.Exception);
                        else
                        {
                            result.SetResult(ar.Result);
                            this.AddCache(path, ar.Result);
                        }
                    });
                });
                return result;
            }
            catch (Exception e)
            {
                return new ImmutableProgressResult<float, T>(e, 0f);
            }
        }

        public virtual Object[] LoadAssets(params string[] paths)
        {
            return this.LoadAssets<Object>(paths);
        }

        public virtual Object[] LoadAssets(Type type, params string[] paths)
        {
            if (paths == null || paths.Length <= 0)
            {
                throw new ArgumentNullException(nameof(paths), "路径为空或空!");
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return paths.Select(path => this.LoadAsset(path, type)).Where(r => r != null).ToArray();
        }

        public virtual T[] LoadAssets<T>(params string[] paths) where T : Object
        {
            if (paths == null || paths.Length <= 0)
            {
                throw new ArgumentNullException(nameof(paths), "路径为空或空!");
            }

            return paths.Select(this.LoadAsset<T>).Where(r => r != null).ToArray();
        }

        public virtual Dictionary<string, Object> LoadAssetsToMap(params string[] paths)
        {
            return this.LoadAssetsToMap(typeof(Object), paths);
        }

        public virtual Dictionary<string, Object> LoadAssetsToMap(Type type, params string[] paths)
        {
            if (paths == null || paths.Length <= 0)
            {
                throw new ArgumentNullException(nameof(paths), "路径为空或空!");
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var dict = new Dictionary<string, Object>();
            foreach (var path in paths)
            {
                if (dict.ContainsKey(path))
                {
                    continue;
                }

                var asset = this.LoadAsset(path, type);
                dict.Add(path, asset);
            }

            return dict;
        }

        public virtual Dictionary<string, T> LoadAssetsToMap<T>(params string[] paths) where T : Object
        {
            if (paths == null || paths.Length <= 0)
            {
                throw new ArgumentNullException(nameof(paths), "路径为空或空!");
            }

            Dictionary<string, T> dict = new Dictionary<string, T>();
            foreach (var path in paths)
            {
                if (dict.ContainsKey(path))
                {
                    continue;
                }

                T asset = this.LoadAsset<T>(path);
                dict.Add(path, asset);
            }

            return dict;
        }

        public virtual IProgressResult<float, Object[]> LoadAssetsAsync(params string[] paths)
        {
            return this.LoadAssetsAsync<Object>(paths);
        }

        public virtual IProgressResult<float, Object[]> LoadAssetsAsync(Type type, params string[] paths)
        {
            try
            {
                if (paths == null || paths.Length <= 0)
                {
                    throw new ArgumentNullException(nameof(paths), "路径为空或空 !");
                }

                if (type == null)
                {
                    throw new ArgumentNullException(nameof(type));
                }

                return Executors.RunOnCoroutine<float, Object[]>((promise) => DoLoadAssetsAsync(promise, type, paths));
            }
            catch (Exception e)
            {
                return new ImmutableProgressResult<float, Object[]>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAssetsAsync(IProgressPromise<float, Object[]> promise, Type type,
            params string[] paths)
        {
            List<Object> results = new List<Object>();
            Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>();
            List<string> bundleNames = new List<string>();
            foreach (string path in paths)
            {
                var pathInfo = this._pathInfoParser.Parse(path);
                if (pathInfo?.BundleName == null)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError("未找到资源包或解析路径信息[{}]失败。", path);
#endif
                    continue;
                }

                var asset = this.GetCache<Object>(path);
                if (asset != null)
                {
                    results.Add(asset);
                    continue;
                }

                if (!groups.TryGetValue(pathInfo.BundleName, out var list))
                {
                    list = new List<string>();
                    groups.Add(pathInfo.BundleName, list);
                    bundleNames.Add(pathInfo.BundleName);
                }

                if (!list.Contains(pathInfo.AssetName))
                    list.Add(pathInfo.AssetName);
            }

            if (bundleNames.Count <= 0)
            {
                promise.UpdateProgress(1f);
                promise.SetResult(results.ToArray());
                yield break;
            }

            IProgressResult<float, IBundle[]> bundleResult = this.LoadBundle(bundleNames.ToArray(), 0);
            var weight = bundleResult.IsDone ? 0f : DefaultWeight;
            bundleResult.Callbackable().OnProgressCallback(p => promise.UpdateProgress(weight * p));

            yield return bundleResult.WaitForDone();

            if (bundleResult.Exception != null)
            {
                promise.SetException(bundleResult.Exception);
                yield break;
            }

            Dictionary<string, IProgressResult<float, Object[]>> assetResults =
                new Dictionary<string, IProgressResult<float, Object[]>>();
            IBundle[] bundles = bundleResult.Result;
            for (int i = 0; i < bundles.Length; i++)
            {
                using IBundle bundle = bundles[i];
                if (!groups.ContainsKey(bundle.Name))
                {
                    continue;
                }

                List<string> assetNames = groups[bundle.Name];
                if (assetNames == null || assetNames.Count < 0)
                {
                    continue;
                }

                IProgressResult<float, Object[]> assetResult = bundle.LoadAssetsAsync(type, assetNames.ToArray());
                assetResult.Callbackable().OnCallback(ar =>
                {
                    if (ar.Exception != null)
                    {
                        return;
                    }

                    results.AddRange(ar.Result);
                });
                assetResults.Add(bundle.Name, assetResult);
            }

            if (assetResults.Count < 0)
            {
                promise.UpdateProgress(1f);
                promise.SetResult(results.ToArray());
                yield break;
            }

            bool finished;
            int assetCount = assetResults.Count;
            do
            {
                yield return WaitForSeconds();

                finished = true;
                var progress = 0f;

                using var assetEnumerator = assetResults.GetEnumerator();
                while (assetEnumerator.MoveNext())
                {
                    var kv = assetEnumerator.Current;
                    var assetResult = kv.Value;
                    if (!assetResult.IsDone)
                    {
                        finished = false;
                    }

                    progress += (1f - weight) * assetResult.Progress / assetCount;
                }

                promise.UpdateProgress(weight + progress);
            } while (!finished);

            promise.UpdateProgress(1f);
            promise.SetResult(results.ToArray());
        }

        public virtual IProgressResult<float, T[]> LoadAssetsAsync<T>(params string[] paths) where T : Object
        {
            try
            {
                if (paths == null || paths.Length <= 0)
                {
                    throw new ArgumentNullException(nameof(paths), "The paths is null or empty!");
                }

                return Executors.RunOnCoroutine<float, T[]>((promise) => DoLoadAssetsAsync(promise, paths));
            }
            catch (Exception e)
            {
                return new ImmutableProgressResult<float, T[]>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAssetsAsync<T>(IProgressPromise<float, T[]> promise, params string[] paths)
            where T : Object
        {
            List<T> results = new List<T>();
            Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>();
            List<string> bundleNames = new List<string>();
            foreach (string path in paths)
            {
                AssetPathInfo pathInfo = this._pathInfoParser.Parse(path);
                if (pathInfo?.BundleName == null)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError("没有找到AssetBundle或解析路径信息“{}”失败.", path);
#endif
                    continue;
                }

                var asset = this.GetCache<T>(path);
                if (asset != null)
                {
                    results.Add(asset);
                    continue;
                }

                if (!groups.TryGetValue(pathInfo.BundleName, out var list))
                {
                    list = new List<string>();
                    groups.Add(pathInfo.BundleName, list);
                    bundleNames.Add(pathInfo.BundleName);
                }

                if (!list.Contains(pathInfo.AssetName))
                    list.Add(pathInfo.AssetName);
            }

            if (bundleNames.Count <= 0)
            {
                promise.UpdateProgress(1f);
                promise.SetResult(results.ToArray());
                yield break;
            }

            IProgressResult<float, IBundle[]> bundleResult = this.LoadBundle(bundleNames.ToArray(), 0);
            float weight = bundleResult.IsDone ? 0f : DefaultWeight;
            bundleResult.Callbackable().OnProgressCallback(p => promise.UpdateProgress(weight * p));

            yield return bundleResult.WaitForDone();

            if (bundleResult.Exception != null)
            {
                promise.SetException(bundleResult.Exception);
                yield break;
            }

            Dictionary<string, IProgressResult<float, T[]>> assetResults =
                new Dictionary<string, IProgressResult<float, T[]>>();
            IBundle[] bundles = bundleResult.Result;
            foreach (var t in bundles)
            {
                using IBundle bundle = t;
                if (!groups.ContainsKey(bundle.Name))
                    continue;

                List<string> assetNames = groups[bundle.Name];
                if (assetNames == null || assetNames.Count < 0)
                    continue;

                var assetResult = bundle.LoadAssetsAsync<T>(assetNames.ToArray());
                assetResult.Callbackable().OnCallback(ar =>
                {
                    if (ar.Exception != null)
                    {
                        return;
                    }

                    results.AddRange(ar.Result);
                });
                assetResults.Add(bundle.Name, assetResult);
            }

            if (assetResults.Count < 0)
            {
                promise.UpdateProgress(1f);
                promise.SetResult(results.ToArray());
                yield break;
            }

            bool finished;
            int assetCount = assetResults.Count;
            do
            {
                yield return WaitForSeconds();

                finished = true;
                var progress = 0f;

                using var assetEnumerator = assetResults.GetEnumerator();
                while (assetEnumerator.MoveNext())
                {
                    var kv = assetEnumerator.Current;
                    var assetResult = kv.Value;
                    if (!assetResult.IsDone)
                    {
                        finished = false;
                    }

                    progress += (1f - weight) * assetResult.Progress / assetCount;
                }

                promise.UpdateProgress(weight + progress);
            } while (!finished);

            promise.UpdateProgress(1f);
            promise.SetResult(results.ToArray());
        }

        public virtual IProgressResult<float, Dictionary<string, Object>> LoadAssetsToMapAsync(params string[] paths)
        {
            return this.LoadAssetsToMapAsync(typeof(Object), paths);
        }

        public virtual IProgressResult<float, Dictionary<string, Object>> LoadAssetsToMapAsync(Type type,
            params string[] paths)
        {
            try
            {
                if (paths == null || paths.Length <= 0)
                {
                    throw new ArgumentNullException(nameof(paths), "The paths is null or empty!");
                }

                if (type == null)
                {
                    throw new ArgumentNullException(nameof(type));
                }

                return Executors.RunOnCoroutine<float, Dictionary<string, Object>>((promise) =>
                    DoLoadAssetsToMapAsync(promise, type, paths));
            }
            catch (Exception e)
            {
                return new ImmutableProgressResult<float, Dictionary<string, Object>>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAssetsToMapAsync(
            IProgressPromise<float, Dictionary<string, Object>> promise, Type type, params string[] paths)
        {
            Dictionary<string, Object> results = new Dictionary<string, Object>();
            Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>();
            Dictionary<string, string> assetNameAndPathMapping = new Dictionary<string, string>();
            List<string> bundleNames = new List<string>();
            foreach (var path in paths)
            {
                AssetPathInfo pathInfo = this._pathInfoParser.Parse(path);
                if (pathInfo?.BundleName == null)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError("没有找到AssetBundle或解析路径信息“{}”失败.", path);
#endif
                    continue;
                }

                var asset = this.GetCache<Object>(path);
                if (asset != null)
                {
                    if (!results.ContainsKey(path))
                    {
                        results.Add(path, asset);
                    }

                    continue;
                }

                if (!groups.TryGetValue(pathInfo.BundleName, out var list))
                {
                    list = new List<string>();
                    groups.Add(pathInfo.BundleName, list);
                    bundleNames.Add(pathInfo.BundleName);
                }

                if (!list.Contains(pathInfo.AssetName))
                {
                    list.Add(pathInfo.AssetName);
                    assetNameAndPathMapping[pathInfo.AssetName] = path;
                }
            }

            if (bundleNames.Count <= 0)
            {
                promise.UpdateProgress(1f);
                promise.SetResult(results);
                yield break;
            }

            IProgressResult<float, IBundle[]> bundleResult = this.LoadBundle(bundleNames.ToArray(), 0);
            var weight = bundleResult.IsDone ? 0f : DefaultWeight;
            bundleResult.Callbackable().OnProgressCallback(p => promise.UpdateProgress(weight * p));

            yield return bundleResult.WaitForDone();

            if (bundleResult.Exception != null)
            {
                promise.SetException(bundleResult.Exception);
                yield break;
            }

            Dictionary<string, IProgressResult<float, Dictionary<string, Object>>> assetResults =
                new Dictionary<string, IProgressResult<float, Dictionary<string, Object>>>();
            IBundle[] bundles = bundleResult.Result;
            for (int i = 0; i < bundles.Length; i++)
            {
                using var bundle = bundles[i];
                if (!groups.ContainsKey(bundle.Name))
                {
                    continue;
                }

                List<string> assetNames = groups[bundle.Name];
                if (assetNames == null || assetNames.Count < 0)
                {
                    continue;
                }

                IProgressResult<float, Dictionary<string, Object>> assetResult =
                    bundle.LoadAssetsToMapAsync(type, assetNames.ToArray());
                assetResult.Callbackable().OnCallback(ar =>
                {
                    if (ar.Exception != null)
                        return;

                    foreach (var kv in ar.Result)
                    {
                        var key = assetNameAndPathMapping[kv.Key];
                        var value = kv.Value;
                        if (!results.ContainsKey(key))
                        {
                            results.Add(key, value);
                        }
                    }
                });
                assetResults.Add(bundle.Name, assetResult);
            }

            if (assetResults.Count < 0)
            {
                promise.UpdateProgress(1f);
                promise.SetResult(results);
                yield break;
            }

            bool finished;
            int assetCount = assetResults.Count;
            do
            {
                yield return WaitForSeconds();

                finished = true;
                var progress = 0f;

                using var assetEnumerator = assetResults.GetEnumerator();
                while (assetEnumerator.MoveNext())
                {
                    var kv = assetEnumerator.Current;
                    var assetResult = kv.Value;
                    if (!assetResult.IsDone)
                    {
                        finished = false;
                    }

                    progress += (1f - weight) * assetResult.Progress / assetCount;
                }

                promise.UpdateProgress(weight + progress);
            } while (!finished);

            promise.UpdateProgress(1f);
            promise.SetResult(results);
        }

        public virtual IProgressResult<float, Dictionary<string, T>> LoadAssetsToMapAsync<T>(params string[] paths)
            where T : Object
        {
            try
            {
                if (paths == null || paths.Length <= 0)
                {
                    throw new ArgumentNullException(nameof(paths), "The paths is null or empty!");
                }

                return Executors.RunOnCoroutine<float, Dictionary<string, T>>((promise) =>
                    DoLoadAssetsToMapAsync(promise, paths));
            }
            catch (Exception e)
            {
                return new ImmutableProgressResult<float, Dictionary<string, T>>(e, 0f);
            }
        }

        protected virtual IEnumerator DoLoadAssetsToMapAsync<T>(IProgressPromise<float, Dictionary<string, T>> promise,
            params string[] paths) where T : Object
        {
            Dictionary<string, T> results = new Dictionary<string, T>();
            Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>();
            Dictionary<string, string> assetNameAndPathMapping = new Dictionary<string, string>();
            List<string> bundleNames = new List<string>();
            foreach (var path in paths)
            {
                AssetPathInfo pathInfo = this._pathInfoParser.Parse(path);
                if (pathInfo?.BundleName == null)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError("没有找到AssetBundle或解析路径信息“{}”失败.", path);
#endif
                    continue;
                }

                var asset = this.GetCache<T>(path);
                if (asset != null)
                {
                    if (!results.ContainsKey(path))
                    {
                        results.Add(path, asset);
                    }

                    continue;
                }

                if (!groups.TryGetValue(pathInfo.BundleName, out var list))
                {
                    list = new List<string>();
                    groups.Add(pathInfo.BundleName, list);
                    bundleNames.Add(pathInfo.BundleName);
                }

                if (list.Contains(pathInfo.AssetName)) continue;
                list.Add(pathInfo.AssetName);
                assetNameAndPathMapping[pathInfo.AssetName] = path;
            }

            if (bundleNames.Count <= 0)
            {
                promise.UpdateProgress(1f);
                promise.SetResult(results);
                yield break;
            }

            IProgressResult<float, IBundle[]> bundleResult = this.LoadBundle(bundleNames.ToArray(), 0);
            float weight = bundleResult.IsDone ? 0f : DefaultWeight;
            bundleResult.Callbackable().OnProgressCallback(p => promise.UpdateProgress(weight * p));

            yield return bundleResult.WaitForDone();

            if (bundleResult.Exception != null)
            {
                promise.SetException(bundleResult.Exception);
                yield break;
            }

            Dictionary<string, IProgressResult<float, Dictionary<string, T>>> assetResults =
                new Dictionary<string, IProgressResult<float, Dictionary<string, T>>>();
            IBundle[] bundles = bundleResult.Result;
            foreach (var t in bundles)
            {
                using IBundle bundle = t;
                if (!groups.ContainsKey(bundle.Name))
                    continue;

                List<string> assetNames = groups[bundle.Name];
                if (assetNames == null || assetNames.Count < 0)
                    continue;

                IProgressResult<float, Dictionary<string, T>> assetResult =
                    bundle.LoadAssetsToMapAsync<T>(assetNames.ToArray());
                assetResult.Callbackable().OnCallback(ar =>
                {
                    if (ar.Exception != null)
                        return;

                    foreach (var kv in ar.Result)
                    {
                        string key = assetNameAndPathMapping[kv.Key];
                        var value = kv.Value;
                        if (!results.ContainsKey(key))
                            results.Add(key, value);
                    }
                });
                assetResults.Add(bundle.Name, assetResult);
            }

            if (assetResults.Count < 0)
            {
                promise.UpdateProgress(1f);
                promise.SetResult(results);
                yield break;
            }

            bool finished;
            int assetCount = assetResults.Count;
            do
            {
                yield return WaitForSeconds();

                finished = true;
                var progress = 0f;

                using var assetEnumerator = assetResults.GetEnumerator();
                while (assetEnumerator.MoveNext())
                {
                    var kv = assetEnumerator.Current;
                    var assetResult = kv.Value;
                    if (!assetResult.IsDone)
                        finished = false;

                    progress += (1f - weight) * assetResult.Progress / assetCount;
                }

                promise.UpdateProgress(weight + progress);
            } while (!finished);

            promise.UpdateProgress(1f);
            promise.SetResult(results);
        }

#if SUPPORT_LOADALL
        public virtual Object[] LoadAllAssets(string bundleName)
        {
            return this.LoadAllAssets<Object>(bundleName);
        }

        public virtual Object[] LoadAllAssets(string bundleName, System.Type type)
        {
            if (bundleName == null)
                throw new System.ArgumentNullException("bundleName");

            if (type == null)
                throw new System.ArgumentNullException("type");

            using (IBundle bundle = this.GetBundle(bundleName))
            {
                if (bundle == null)
                    return null;
                return bundle.LoadAllAssets(type);
            }
        }

        public virtual T[] LoadAllAssets<T>(string bundleName) where T : Object
        {
            if (bundleName == null)
                throw new System.ArgumentNullException("bundleName");

            using (IBundle bundle = this.GetBundle(bundleName))
            {
                if (bundle == null)
                    return null;
                return bundle.LoadAllAssets<T>();
            }
        }

        public virtual IProgressResult<float, Object[]> LoadAllAssetsAsync(string bundleName)
        {
            return this.LoadAllAssetsAsync<Object>(bundleName);
        }

        public virtual IProgressResult<float, Object[]> LoadAllAssetsAsync(string bundleName, System.Type type)
        {
            try
            {
                if (bundleName == null)
                    throw new System.ArgumentNullException("bundleName");

                if (type == null)
                    throw new System.ArgumentNullException("type");

                ProgressResult<float, Object[]> result = new ProgressResult<float, Object[]>();
                IProgressResult<float, IBundle> bundleResult = this.LoadBundle(bundleName);
                float weight = bundleResult.IsDone ? 0f : DEFAULT_WEIGHT;
                bundleResult.Callbackable().OnProgressCallback(p => result.UpdateProgress(p * weight));
                bundleResult.Callbackable().OnCallback((r) =>
                {
                    if (r.Exception != null)
                    {
                        result.SetException(r.Exception);
                        return;
                    }

                    using (IBundle bundle = r.Result)
                    {
                        IProgressResult<float, Object[]> assetResult = bundle.LoadAllAssetsAsync(type);
                        assetResult.Callbackable().OnProgressCallback(p => result.UpdateProgress(weight + (1f - weight) * p));
                        assetResult.Callbackable().OnCallback((ar) =>
                        {
                            if (ar.Exception != null)
                                result.SetException(ar.Exception);
                            else {
                                result.SetResult(ar.Result);
                            }
                        });
                    }
                });
                return result;
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, Object[]>(e, 0f);
            }
        }

        public virtual IProgressResult<float, T[]> LoadAllAssetsAsync<T>(string bundleName) where T : Object
        {
            try
            {
                if (bundleName == null)
                    throw new System.ArgumentNullException("bundleName");

                ProgressResult<float, T[]> result = new ProgressResult<float, T[]>();
                IProgressResult<float, IBundle> bundleResult = this.LoadBundle(bundleName);
                float weight = bundleResult.IsDone ? 0f : DEFAULT_WEIGHT;
                bundleResult.Callbackable().OnProgressCallback(p => result.UpdateProgress(p * weight));
                bundleResult.Callbackable().OnCallback(r =>
                {
                    if (r.Exception != null)
                    {
                        result.SetException(r.Exception);
                        return;
                    }

                    using (IBundle bundle = r.Result)
                    {
                        IProgressResult<float, T[]> assetResult = bundle.LoadAllAssetsAsync<T>();
                        assetResult.Callbackable().OnProgressCallback(p => result.UpdateProgress(weight + (1f - weight) * p));
                        assetResult.Callbackable().OnCallback((ar) =>
                        {
                            if (ar.Exception != null)
                                result.SetException(ar.Exception);
                            else {
                                result.SetResult(ar.Result);
                            }
                        });
                    }
                });
                return result;
            }
            catch (System.Exception e)
            {
                return new ImmutableProgressResult<float, T[]>(e, 0f);
            }
        }
#endif
        public virtual ISceneLoadingResult<Scene> LoadSceneAsync(string path, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneLoadingResult<Scene> result = new SceneLoadingResult<Scene>();
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    throw new ArgumentNullException(nameof(path), "The path is null or empty!");
                }

                InterceptableEnumerator enumerator = new InterceptableEnumerator(DoLoadSceneAsync(result, path, mode));
                enumerator.RegisterCatchBlock(e =>
                {
                    result.SetException(e);
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError(e);
#endif
                });
                enumerator.RegisterFinallyBlock(() =>
                {
                    if (!result.IsDone)
                    {
                        result.SetException(new Exception("没有给出结果值"));
                    }
                });
                Executors.RunOnCoroutineNoReturn(enumerator);
            }
            catch (Exception e)
            {
                result.Progress = 0f;
                result.SetException(e);
            }

            return result;
        }

        protected abstract IEnumerator DoLoadSceneAsync(ISceneLoadingPromise<Scene> promise, string path,
            LoadSceneMode mode = LoadSceneMode.Single);

        #endregion

        #region IDisposable Support

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    this._assetCache.Clear();
                    if (bundleManager is IDisposable disposable)
                        disposable.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
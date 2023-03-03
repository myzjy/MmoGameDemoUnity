using System;
using System.Collections;
using System.Collections.Generic;
using Framework.AssetBundles.Utilty;
using HybridCLR;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZJYFrameWork.AssetBundles.AssetBundleToolsConfig;
using ZJYFrameWork.AssetBundles.Bundle.LoaderBuilders;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.AssetBundles.Bundles.ILoaderBuilderInterface;
using ZJYFrameWork.AssetBundles.Bundles.LoaderBuilders;
using ZJYFrameWork.AssetBundles.BundleUtils;
using ZJYFrameWork.AssetBundles.IAssetBundlesManagerInterface;
// using ZJYFrameWork.AssetBundles.IDownLoadManagerInterface;
using ZJYFrameWork.AssetBundles.Model;
using ZJYFrameWork.AssetBundles.Model.Callback;
using ZJYFrameWork.Execution;
using ZJYFrameWork.Module.Scenes.Callbacks;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable.Common;
using Object = UnityEngine.Object;

namespace ZJYFrameWork.AssetBundles.AssetBundlesManager
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Game/Framework/Asset Bundle Manager")]
    public sealed  class AssetBundleManager : MonoBehaviour, IAssetBundleManager
    {
        private IBundleManager _bundleManager;

        /// <summary>
        /// 构建管理器
        /// </summary>
        private ILoaderBuilder _loaderBuilder;

        /// <summary>
        /// 路径保存器
        /// </summary>
        private IPathInfoParser _pathInfoParser;

        /// <summary>
        /// BundleManifest 读取器
        /// </summary>
        private IBundleManifestLoader BundleManifestLoader;

     //   private IDownloadManager DownloadManager;

        /// <summary>
        /// 资源读取接口 需要在下载接口走完之后，查看有没有需要下载
        /// </summary>
        private IResources Resources;
        

        /// <summary>
        /// 部分本地场景加载
        /// </summary>
        private List<string> SceneLoacelList = new List<string>()
        {
            "LoginBase",
            "LoginBase".ToLower(),
            "GameMain",
            "GameMain".ToLower(),
        };

        public string BundleRoot => AssetBundleConfig.BundleRoot;

        private string BundleManifestPath =>
            $"{StorableDirectory}{AssetBundleConfig.ManifestFilename}";

        public string StorableDirectory
        {
            get => BundleUtil.GetStorableDirectory();
        }

        public string ReadOnlyDirectory
        {
            get => BundleUtil.GetReadOnlyDirectory();
        }

        public string TemporaryCacheDirectory
        {
            get => BundleUtil.GetTemporaryCacheDirectory();
        }

        public string UpdatePrefixUri { get; set; }
        public string ApplicableGameVersion { get; }
        public float AssetAutoReleaseInterval { get; set; }
        public int AssetCapacity { get; set; }
        public float AssetExpireTime { get; set; }
        public float ResourceAutoReleaseInterval { get; set; }
        public int ResourceCapacity { get; set; }
        public float ResourceExpireTime { get; set; }
        public int ResourcePriority { get; set; }

        /// <summary>
        /// 管理manifest
        /// </summary>
        public BundleManifest BundleManifest { get; set; }
        public List<string> AOTMetaAssemblyNames { get; } = new List<string>()
        {
            "mscorlib_bytes",
            "System_bytes",
            "System_Core_bytes",
        };

        private void Awake()
        {
            Debug.Log("[AssetBundleManager]");
            Executors.RunOnCoroutine(InitBase());
        }

        /// <summary>
        /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
        /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
        /// </summary>
        private IEnumerator LoadMetadataForAOTAssemblies()
        {
            // 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            // 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
            // 
            HomologousImageMode mode = HomologousImageMode.SuperSet;
            bool isLoad = false;
            foreach (var aotDllName in AOTMetaAssemblyNames)
            {
                isLoad = false;
                LoadAsset(aotDllName, res =>
                {
                    byte[] dllBytes = res;
                    // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                    LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
                    isLoad = true;
                });
                yield return new WaitUntil(() => isLoad);
            }
        }
        public IEnumerator InitBase()
        {
            SetAssetBundle();
            yield return LoadMetadataForAOTAssemblies();
            bool isLoad = false;
#if !UNITY_EDITOR
            LoadAsset("Assembly-CSharp_bytes", res =>
            {
                byte[] dllBytes = res;
                // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                System.Reflection.Assembly.Load(dllBytes);
                isLoad = true;
            });
            yield return new WaitUntil(() => isLoad);
#endif
            isLoad = false;
            Object obj = null;
            LoadAsset("Base", res =>
            {
                //生成
                obj = res;
                isLoad = true;
            });
            yield return new WaitUntil(() => isLoad);
            var baseObj = Instantiate(obj, this.transform) as GameObject;
        }
        public void SetAssetBundle()
        {
            if (BundleManifestLoader == null)
            {
                BundleManifestLoader = new BundleManifestLoader();
            }
#if UNITY_EDITOR
            if (AssetBundleConfig.IsEditorMode)
            {
                //编辑器
                if (_pathInfoParser == null)
                {
                    _pathInfoParser = new SimulationAutoMappingPathInfoParser();
                }

                if (_bundleManager == null)
                {
                    _bundleManager = new SimulationBundleManager();
                }

                if (Resources == null)
                {
                    Resources = new SimulationResources();
                }

                Resources.SetIPathAndBundleResource(_pathInfoParser, _bundleManager);
            }
            else
#endif
            {
                //读取Manifest文件
                BundleManifest = BundleManifestLoader.Load(BundleManifestPath);
                if (_pathInfoParser == null)
                {
                    _pathInfoParser = new AutoMappingPathInfoParser();
                }

                if (BundleManifest != null && _pathInfoParser != null)
                {
                    BundleManifest.ActiveVariants = new string[] { "", "hd" };
                    _pathInfoParser.BundleManifest = BundleManifest;
                }

                if (_loaderBuilder == null)
                {
                    Uri baseUrl = new Uri(StorableDirectory);
                    _loaderBuilder = new CustomBundleLoaderBuilder(baseUrl, false);
                }

                if (BundleManifest != null && _bundleManager == null)
                {
                    //先new出来，在
                    _bundleManager = new BundleManager();
                }

                if (BundleManifest != null && Resources == null)
                {
                    Resources = new BundleResources();
                    //下面的设置必须有先后顺序
                    Resources.SetIPathAndBundleResource(_pathInfoParser, _bundleManager);
                    Resources.SetManifestAndLoadBuilder(BundleManifest, _loaderBuilder);
                }

                if (BundleManifest != null
                    && Resources != null
                    && _bundleManager != null
                    && _loaderBuilder != null)
                {
                    Uri baseUrl = new Uri(StorableDirectory);
                    _loaderBuilder.SetUrl(baseUrl);
                    Resources.SetIPathAndBundleResource(_pathInfoParser, _bundleManager);
                    Resources.SetManifestAndLoadBuilder(BundleManifest, _loaderBuilder);
                }
                
                // resourceUpdater.ResourceUpdateComplete += OnUpdaterResourceUpdateComplete;
            }
        }

        public void LoadAssetBundle(string assetBundle, LoadAssetCallbacks loadAssetCallbacks)
        {
            var obj = Resources.LoadAsset(assetBundle);
            if (obj == null)
            {
                // loadAssetCallbacks.LoadAssetFailureCallback();
            }
            else
            {
            }
        }

        public void LoadAsset(string assetBundle, System.Action<Object> loadAssetCallbacks)
        {
            var abName = $"{assetBundle.ToLower()}{AssetBundleConfig.AssetBundleSuffix}";
            var obj = Resources.LoadAssetAsync(abName);
            obj.WaitForDone();
            obj.Callbackable().OnProgressCallback(res =>
            {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                Debug.Log("[{}]加载进度：[{}]%", abName, res * 100.0f);
#endif
                CommonController.Instance.snackbar.OpenUIDataLoadingPanel("", res, 1, "正在加载资源,请稍等...");
            });
            obj.Callbackable().OnCallback(res =>
            {
                if (res.Exception != null) return;
                if (!res.IsDone) return;
                if (res.Result != null)
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.Log(res.Result);
#endif
                    loadAssetCallbacks(res.Result);
                }
            });
        }
        public void LoadAsset(string assetBundle, LoadAssetCallbacks loadAssetCallbacks)
        {
            var abName = $"{assetBundle.ToLower()}{AssetBundleConfig.AssetBundleSuffix}";
            var obj = Resources.LoadAssetAsync(abName);
            obj.WaitForDone();
            obj.Callbackable().OnProgressCallback(res => { Debug.Log("[{}]加载进度：[{}]%", abName, res * 100.0f); });
            obj.Callbackable().OnCallback(res =>
            {
                if (res.Exception == null)
                {
                    if (res.IsDone)
                    {
                        if (res.IsCancelled)
                        {
                            loadAssetCallbacks.LoadAssetFailureCallback(abName, LoadResourceStatus.AssetError,
                                "加载完成前，被取消了", null);
                            // loadAssetCallbacks.LoadAssetUpdateCallback
                        }
                        else
                        {
                            if (res.Result != null)
                            {
                                loadAssetCallbacks.LoadAssetSuccessCallback(abName, res.Result, 1, null);
                            }

                            else
                            {
                                loadAssetCallbacks.LoadAssetFailureCallback(abName, LoadResourceStatus.AssetError,
                                    "加载完成前,资源为空", null);
                            }
                        }
                    }
                }
                else
                {
                    loadAssetCallbacks.LoadAssetFailureCallback(abName, LoadResourceStatus.AssetError,
                        res.Exception.Message, null);
                }
            });
            // loadAssetCallbacks.LoadAssetSuccessCallback(assetBundle, obj, 0, null);
        }

        public void SetBundleManifest(BundleManifest bundleManifest)
        {
            BundleManifest = bundleManifest;
            SetAssetBundle();
        }

        public void LoadScene(string sceneAssetName, int priority, LoadSceneCallbacks loadSceneCallbacks,
            object userData)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                throw new Exception("Scene asset name is invalid.");
            }

            if (loadSceneCallbacks == null)
            {
                throw new Exception("Load scene callbacks is invalid.");
            }

            var abName = $"{sceneAssetName}{AssetBundleConfig.AssetBundleSuffix}";

            //代表我是加载本地场景，有部分是直接加载的本地场景
            if (SceneLoacelList.Contains(sceneAssetName))
            {
                loadSceneCallbacks.LoadSceneDependencyAssetCallback(sceneAssetName, "", 0, 0, null);
                Executors.RunOnCoroutine(LoadSceneAsyncIe(sceneAssetName, loadSceneCallbacks));
                return;
            }


            var sceneLoading = Resources.LoadSceneAsync(abName);
            sceneLoading.OnStateChangedCallback(res =>
            {
                switch (res)
                {
                    case LoadState.None:
                        break;
                    case LoadState.AssetBundleLoaded:
                    {
                        //
                        loadSceneCallbacks.LoadSceneUpdateCallback(sceneAssetName, 0, null);
                    }
                        break;
                    case LoadState.SceneActivationReady:
                    {
                        Debug.Log("Ready to activate the scene.");
                        sceneLoading.AllowSceneActivation = true;
                        // CommonUIManager.Instance.Snackbar.CloseUINetLoading();
                    }
                        break;
                    case LoadState.Failed:
                        Debug.Log($"Loads scene '{abName}' failure.Error:{sceneLoading.Exception}");
                        loadSceneCallbacks.LoadSceneFailureCallback(abName, LoadResourceStatus.AssetError,
                            sceneLoading.Exception.ToString(), null);
                        break;
                    case LoadState.Completed:
                        loadSceneCallbacks.LoadSceneSuccessCallback(sceneAssetName, 1, null);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(res), res, null);
                }
            });
            sceneLoading.OnProgressCallback(res =>
            {
#if UNITY_EDITOR || DEVELOP_BUILD
                Debug.Log($"加载场景进度：{res * 100}");
                loadSceneCallbacks.LoadSceneUpdateCallback(sceneAssetName, res, null);
#else
                loadSceneCallbacks.LoadSceneUpdateCallback(sceneAssetName, res, null);
#endif
            });
            sceneLoading.WaitForDone();
            if (sceneLoading.Exception != null)
            {
                //先关闭场景加载动画
                // CommonUIManager.Instance.Snackbar.OpenUIDataScenePanel(1, 1);
                var buildInfo = BundleManifest.GetBundleInfo(sceneAssetName);
                //没有真确读取到
                //可能需要去下载
//                 DownloadManager.OpenUIDataLoading(() =>
//                 {
//                     //重新设置
//                     SetBundleManifest(BundleManifest);
// #if UNITY_EDITOR || DEVELOP_BUILD
//                     Debug.Log("重新加载场景，场景已经下载完成");
// #endif
//                     LoadScene(sceneAssetName, priority, loadSceneCallbacks, userData);
//                 }, buildInfo);
            }
        }

        public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks)
        {
            throw new NotImplementedException();
        }

        public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData)
        {
            throw new NotImplementedException();
        }

        public IEnumerator LoadSceneAsyncIe(string sceneName, LoadSceneCallbacks loadSceneCallbacks)
        {
            //关闭
            // UIManager.Instance.GetSystem<IUISystemModule>().CloseUIEvent();
            // LandscapeLeftOrPortrait(false);

            //场景加载进度条
            var loadSceneAsync = SceneManager.LoadSceneAsync(sceneName);
            //如果为true，那么加载结束后直接就会跳转，我们根本看不见加载的过程
            loadSceneAsync.allowSceneActivation = false;
            while (!loadSceneAsync.isDone)
            {
                Debug.Log($"场景进度:{loadSceneAsync.progress * 100}%");
#if UNITY_EDITOR || DEVELOP_BUILD
                Debug.Log($"加载场景进度：{loadSceneAsync.progress * 100}");
                loadSceneCallbacks.LoadSceneUpdateCallback(sceneName, loadSceneAsync.progress, null);
#else
                loadSceneCallbacks.LoadSceneUpdateCallback(sceneName,  loadSceneAsync.progress, null);
#endif
                if (loadSceneAsync.progress >= 0.9f)
                {
                    // LandscapeLeftOrPortrait(true);
                    //直接转换场景
                    loadSceneAsync.allowSceneActivation = true;
                }

                yield return new WaitForEndOfFrame();

                yield return null;
            }

            loadSceneCallbacks.LoadSceneSuccessCallback(sceneName, 1, null);
        }


        private void OnUpdaterResourceUpdateStart(BundleInfo resourceName, string downloadPath, string downloadUri,
            int currentLength)
        {
            // EventBus.SyncSubmit(ResourceUpdateStartEvent.ValueOf(resourceName.FullName, downloadPath, downloadUri, currentLength, zipLength, retryCount));
        }

        private void OnUpdaterResourceUpdateChanged(BundleInfo resourceName, string downloadPath, string downloadUri,
            int currentLength, long zipLength)
        {
            // EventBus.SyncSubmit(ResourceUpdateChangedEvent.ValueOf(resourceName.FullName, downloadPath, downloadUri, currentLength, zipLength));
        }

        private void OnUpdaterResourceUpdateSuccess(BundleInfo resourceName, string downloadPath, string downloadUri,
            int length, long zipLength)
        {
            // EventBus.SyncSubmit(ResourceUpdateSuccessEvent.ValueOf(resourceName.FullName, downloadPath, downloadUri, length, zipLength));
        }

        private void OnUpdaterResourceUpdateFailure(BundleInfo resourceName, string downloadUri, int retryCount,
            int totalRetryCount, string errorMessage)
        {
            // EventBus.SyncSubmit(ResourceUpdateFailureEvent.ValueOf(resourceName.FullName, downloadUri, retryCount, totalRetryCount, errorMessage));
        }

        public void ReginBean()
        {
            SpringContext.RegisterBean(this);
        }
        public void LoadAsset(string assetBundle, Action<byte[]> loadAssetCallbacks)
        {
            var abName = $"{assetBundle.ToLower()}{AssetBundleConfig.AssetBundleSuffix}";

            var obj = Resources.LoadAssetAsync<TextAsset>(abName);
            obj.Callbackable().OnCallback(res =>
            {
                if (res.Result == null)
                {
                    var iBundle = Resources.LoadBundle(abName);
                    iBundle.Callbackable().OnCallback(res =>
                    {
                        Debug.Log($"{res.Result}");
                        var data = res.Result.LoadAsset<TextAsset>(abName);
                        loadAssetCallbacks.Invoke(data.bytes);
                    });
                }
                else
                {
                    Debug.Log(obj.Result);
                    loadAssetCallbacks.Invoke(obj.Result.bytes);
                }
            });
        }
    }
}
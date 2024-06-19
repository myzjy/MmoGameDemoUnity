using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZJYFrameWork.AssetBundles.IAssetBundlesManagerInterface;
using ZJYFrameWork.AssetBundles.Model;
using ZJYFrameWork.Base.Model;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Event;
using ZJYFrameWork.Module.Scenes.Callbacks;
using ZJYFrameWork.Net.Core.Model;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable.Common;
using Debug = FrostEngine.Debug;

namespace ZJYFrameWork.Scenes
{
    /// <summary>
    /// 场景管理器。
    /// </summary>
    [Bean]
    public sealed class SceneManager : AbstractManager, ISceneManager
    {
        public readonly List<string> loadedSceneAssetNames = new List<string>();
        private readonly LoadSceneCallbacks loadSceneCallbacks;

        public readonly List<string> unloadingSceneAssetNames = new List<string>();
        private readonly UnloadSceneCallbacks unloadSceneCallbacks;
        [Autowired] private IAssetBundleManager resourceManager;

        /// <summary>
        /// 初始化场景管理器的新实例。
        /// </summary>
        public SceneManager()
        {
            loadSceneCallbacks = new LoadSceneCallbacks(LoadSceneSuccessCallback, LoadSceneFailureCallback,
                LoadSceneUpdateCallback, LoadSceneDependencyAssetCallback);
            unloadSceneCallbacks = new UnloadSceneCallbacks(UnloadSceneSuccessCallback, UnloadSceneFailureCallback);
        }

        public void LoadScene(string sceneAssetName)
        {
            // throw new System.NotImplementedException();
            resourceManager.LoadScene(sceneAssetName, priority: 0, loadSceneCallbacks, null);
        }

        public void LoadScene(string sceneAssetName, int priority, object userData)
        {
            resourceManager.LoadScene(sceneAssetName, priority: priority, loadSceneCallbacks, userData);
        }

        public void UnloadScene(string sceneAssetName, object userData)
        {
            throw new System.NotImplementedException();
        }

        public void UnloadAllScenes()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator UnloadScene(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                yield break;
            }

            var unloadAsync = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneAssetName);
            if (unloadAsync == null)
            {
                unloadSceneCallbacks.UnloadSceneFailureCallback(sceneAssetName, null);
                yield break;
            }

            while (!unloadAsync.isDone)
            {
                if (unloadAsync.progress > 0.9f)
                {
                    //卸载成功
                }

                yield return new WaitForEndOfFrame();
                yield return null;
            }

            yield return UnloadSceneNew();
            unloadSceneCallbacks.UnloadSceneSuccessCallback(sceneAssetName: sceneAssetName, null);
            yield return null;
        }


        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            return;
        }

        public override void Shutdown()
        {
            // throw new System.NotImplementedException();
        }

        private void LoadSceneSuccessCallback(string sceneAssetName, float duration, object userData)
        {
            // CommonController.Instance.snackbar.OpenUIDataScenePanel(0, 1);

            CommonController.Instance.snackbar.OpenUIDataScenePanel(duration, 1);

            Debug.Log("Load assetSceneName:[{}]", sceneAssetName);
            loadedSceneAssetNames.Add(sceneAssetName);
            unloadingSceneAssetNames.Add(sceneAssetName);
            var eve = LoadSceneSuccessEvent.ValueOf(sceneAssetName, duration, userData);
            EventBus.SyncSubmit(eve);
            ReferenceCache.Release(eve);
        }

        private void LoadSceneFailureCallback(string sceneAssetName, LoadResourceStatus status, string errorMessage,
            object userData)
        {
            Debug.Log("场景加载失败");
            unloadingSceneAssetNames.Remove(sceneAssetName);
            loadedSceneAssetNames.Remove(sceneAssetName);
            Action action = (Action)userData;
            action?.Invoke();
            Debug.LogError("Load scene failure, scene asset name '{}', status '{}', error message '{}'.",
                sceneAssetName, status.ToString(), errorMessage);
        }

        private void LoadSceneUpdateCallback(string sceneAssetName, float progress, object userData)
        {
            CommonController.Instance.snackbar.OpenUIDataScenePanel(progress, 1);

            // SpringContext.GetBean<LoadUIController>().SetNowProgressNum(progress);
        }

        private void LoadSceneDependencyAssetCallback(string sceneAssetName, string dependencyAssetName,
            int loadedCount, int totalCount, object userData)
        {
            CommonController.Instance.snackbar.OpenUIDataScenePanel(0, 1);

            // var eve = LoadSceneDependencyAssetEvent.ValueOf(sceneAssetName, dependencyAssetName, loadedCount, totalCount, userData);
            // EventBus.SyncSubmit(eve);
            // ReferenceCache.Release(eve);
        }

        private void UnloadSceneSuccessCallback(string sceneAssetName, object userData)
        {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
            Debug.Log($"卸载场景成功{sceneAssetName}");
#endif
            unloadingSceneAssetNames.Remove(sceneAssetName);
            loadedSceneAssetNames.Remove(sceneAssetName);
        }

        private void UnloadSceneFailureCallback(string sceneAssetName, object userData)
        {
            unloadingSceneAssetNames.Remove(sceneAssetName);
        }

        public IEnumerator FadeAndLoadSceneAsyncNew(string sceneAssetName)
        {
            CommonController.Instance.snackbar.OpenUIDataScenePanel(0, 1);

            // SpringContext.GetBean<LoadUIController>().SetNowProgressNum(0);
            for (var i = unloadingSceneAssetNames.Count - 1; i >= 0; i--)
            {
                yield return UnloadScene(unloadingSceneAssetNames[i]);
            }

            LoadScene(sceneAssetName);
        }

        private IEnumerator UnloadSceneNew()
        {
            yield return Resources.UnloadUnusedAssets();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
    }
}
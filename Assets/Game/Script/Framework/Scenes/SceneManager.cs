using ZJYFrameWork.AssetBundles;
using ZJYFrameWork.AssetBundles.Model;
using ZJYFrameWork.Base.Model;
using ZJYFrameWork.Collection.Reference;
using ZJYFrameWork.Event;
using ZJYFrameWork.Module.Scenes.Callbacks;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable;
using ZJYFrameWork.UISerializable.Manager;

namespace ZJYFrameWork.Scenes
{
      /// <summary>
    /// 场景管理器。
    /// </summary>
    [Bean]
    public sealed class SceneManager : AbstractManager, ISceneManager
    {
        [Autowired] private IAssetBundleManager resourceManager;
        private readonly LoadSceneCallbacks loadSceneCallbacks;
        private readonly UnloadSceneCallbacks unloadSceneCallbacks;

        /// <summary>
        /// 初始化场景管理器的新实例。
        /// </summary>
        public SceneManager()
        {
            loadSceneCallbacks = new LoadSceneCallbacks(LoadSceneSuccessCallback, LoadSceneFailureCallback,
                LoadSceneUpdateCallback, LoadSceneDependencyAssetCallback);
            unloadSceneCallbacks = new UnloadSceneCallbacks(UnloadSceneSuccessCallback, UnloadSceneFailureCallback);
        }


        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            // throw new System.NotImplementedException();
        }

        public override void Shutdown()
        {
            // throw new System.NotImplementedException();
        }

        public void LoadScene(string sceneAssetName)
        {
            // throw new System.NotImplementedException();
            resourceManager.LoadScene(sceneAssetName,priority:0,loadSceneCallbacks,null);
        }

        public void LoadScene(string sceneAssetName, int priority, object userData)
        {
            resourceManager.LoadScene(sceneAssetName,priority:priority,loadSceneCallbacks,userData);

        }

        public void UnloadScene(string sceneAssetName)
        {
            throw new System.NotImplementedException();
        }

        public void UnloadScene(string sceneAssetName, object userData)
        {
            throw new System.NotImplementedException();
        }

        public void UnloadAllScenes()
        {
            throw new System.NotImplementedException();
        }

        private void LoadSceneSuccessCallback(string sceneAssetName, float duration, object userData)
        {
            CommonController.Instance.snackbar.OpenUIDataScenePanel(1,1);


            // UIComponentManager.DispatchEvent(UINotifEnum.OPEN_ACTITVIESUIMANAGER_PANEL);
            // loadingSceneAssetNames.Remove(sceneAssetName);
            // loadedSceneAssetNames.Add(sceneAssetName);
            // var eve = LoadSceneSuccessEvent.ValueOf(sceneAssetName, duration, userData);
            // EventBus.SyncSubmit(eve);
            // ReferenceCache.Release(eve);
        }

        private void LoadSceneFailureCallback(string sceneAssetName, LoadResourceStatus status, string errorMessage,
            object userData)
        {
            Debug.Log("场景加载失败");
            // loadingSceneAssetNames.Remove(sceneAssetName);
            //
            // var eve = LoadSceneFailureEvent.ValueOf(sceneAssetName, userData);
            // EventBus.SyncSubmit(eve);
            // ReferenceCache.Release(eve);
            //
            // Log.Error("Load scene failure, scene asset name '{}', status '{}', error message '{}'.", sceneAssetName, status.ToString(), errorMessage);
        }

        private void LoadSceneUpdateCallback(string sceneAssetName, float progress, object userData)
        {
            Debug.Log("加载");
            CommonController.Instance.snackbar.OpenUIDataScenePanel(progress,1);
            // var eve = LoadSceneUpdateEvent.ValueOf(sceneAssetName, progress, userData);
            // EventBus.SyncSubmit(eve);
            // ReferenceCache.Release(eve);
        }

        private void LoadSceneDependencyAssetCallback(string sceneAssetName, string dependencyAssetName,
            int loadedCount, int totalCount, object userData)
        {
            // var eve = LoadSceneDependencyAssetEvent.ValueOf(sceneAssetName, dependencyAssetName, loadedCount, totalCount, userData);
            // EventBus.SyncSubmit(eve);
            // ReferenceCache.Release(eve);
        }

        private void UnloadSceneSuccessCallback(string sceneAssetName, object userData)
        {
            // unloadingSceneAssetNames.Remove(sceneAssetName);
            // loadedSceneAssetNames.Remove(sceneAssetName);
            //
            // var eve = UnloadSceneSuccessEvent.ValueOf(sceneAssetName, userData);
            // EventBus.SyncSubmit(eve);
            // ReferenceCache.Release(eve);
        }

        private void UnloadSceneFailureCallback(string sceneAssetName, object userData)
        {
            // unloadingSceneAssetNames.Remove(sceneAssetName);
            //
            // var eve = UnloadSceneFailureEvent.ValueOf(sceneAssetName, userData);
            // EventBus.SyncSubmit(eve);
            // ReferenceCache.Release(eve);
            //
            // Log.Error(StringUtils.Format("Unload scene failure, scene asset name '{}'.", sceneAssetName));
        }
    }
}
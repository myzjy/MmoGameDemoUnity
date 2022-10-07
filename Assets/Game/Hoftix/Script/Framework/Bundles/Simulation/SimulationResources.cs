#if UNITY_EDITOR
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using ZJYFrameWork.Spring.Core;
#if UNITY_2018_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif

namespace ZJYFrameWork.AssetBundles.Bundles
{
    public class SimulationResources : AbstractResources
    {
        protected const string ASSETS = "Assets/";

        public SimulationResources() : base()
        {
            
        }
        // public SimulationResources() : this(new SimulationAutoMappingPathInfoParser(), new SimulationBundleManager())
        // {
        // }

        public SimulationResources(IPathInfoParser pathInfoParser) : base(pathInfoParser, new SimulationBundleManager(), false)
        {
        }

        public SimulationResources(IPathInfoParser pathInfoParser, IBundleManager manager) : base(pathInfoParser, manager, false)
        {
        }

        protected override IEnumerator DoLoadSceneAsync(ISceneLoadingPromise<Scene> promise, string path, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AssetPathInfo pathInfo = pathInfoParser.Parse(path);
            if (pathInfo == null)
            {
                promise.Progress = 0f;
                promise.SetException($"Parses the path info '{path}' failure.");
                yield break;
            }

            var name = $"{ASSETS}{pathInfo.AssetName}";
#if UNITY_2018_3_OR_NEWER
            AsyncOperation operation = EditorSceneManager.LoadSceneAsyncInPlayMode(name, new LoadSceneParameters(mode));
#else
            AsyncOperation operation = LoadSceneMode.Additive.Equals(mode) ? EditorApplication.LoadLevelAdditiveAsyncInPlayMode(name) : EditorApplication.LoadLevelAsyncInPlayMode(name);
#endif
            if (operation == null)
            {
                promise.SetException($"Not found the scene '{path}'.");
                yield break;
            }

            operation.allowSceneActivation = false;
            while (operation.progress < 0.9f)
            {
                if (operation.progress == 0f)
                    operation.priority = promise.Priority;

                promise.Progress = operation.progress;
                yield return WaitForSeconds();
            }
            promise.Progress = operation.progress;
            promise.State = LoadState.SceneActivationReady;

            while (!operation.isDone)
            {
                if (promise.AllowSceneActivation && !operation.allowSceneActivation)
                    operation.allowSceneActivation = promise.AllowSceneActivation;

                promise.Progress = operation.progress;
                yield return WaitForSeconds();
            }

            Scene scene = SceneManager.GetSceneByName(Path.GetFileNameWithoutExtension(pathInfo.AssetName));
            if (!scene.IsValid())
            {
                promise.SetException($"Not found the scene '{path}'.");
                yield break;
            }

            promise.Progress = 1f;
            promise.SetResult(scene);
        }
    }
}
#endif
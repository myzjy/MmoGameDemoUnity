using System.Collections;

namespace ZJYFrameWork.Scenes
{
    public interface ISceneManager
    {
        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        void LoadScene(string sceneAssetName);

        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="priority">加载场景资源的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        void LoadScene(string sceneAssetName, int priority, object userData);

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        IEnumerator UnloadScene(string sceneAssetName);

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="userData">用户自定义数据。</param>
        void UnloadScene(string sceneAssetName, object userData);

        void UnloadAllScenes();
    }
}
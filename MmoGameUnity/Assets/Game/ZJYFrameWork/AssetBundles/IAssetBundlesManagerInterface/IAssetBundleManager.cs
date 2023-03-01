using System.Collections;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.AssetBundles.Model.Callback;
using ZJYFrameWork.Module.Scenes.Callbacks;

namespace ZJYFrameWork.AssetBundles.IAssetBundlesManagerInterface
{
    /// <summary>
    /// AssetBundle
    /// </summary>
    public interface IAssetBundleManager
    {
        /// <summary>
        /// 路径之后接 指向下一个路径 ?/AssetBundles
        /// </summary>
        string BundleRoot { get; }

        /// <summary>
        /// 获取资源可读写路径，一般指向persistentDataPath
        /// </summary>
        string StorableDirectory { get; }

        /// <summary>
        /// 获取资源只读区路径，一般指向streamingAssetsPath
        /// </summary>
        string ReadOnlyDirectory { get; }

        /// <summary>
        /// 缓存路径一般指向temporaryCachePath
        /// </summary>
        string TemporaryCacheDirectory { get; }

        /// <summary>
        /// 获取或设置资源更新下载地址。
        /// </summary>
        string UpdatePrefixUri { get; set; }

        /// <summary>
        /// 获取当前资源适用的游戏版本号。
        /// </summary>
        string ApplicableGameVersion { get; }

        /// <summary>
        /// 获取或设置资源对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        float AssetAutoReleaseInterval { get; set; }

        /// <summary>
        /// 获取或设置资源对象池的容量。
        /// </summary>
        int AssetCapacity { get; set; }

        /// <summary>
        /// 获取或设置资源对象池对象过期秒数。
        /// </summary>
        float AssetExpireTime { get; set; }

        /// <summary>
        /// 获取或设置资源对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        float ResourceAutoReleaseInterval { get; set; }

        /// <summary>
        /// 获取或设置资源对象池的容量。
        /// </summary>
        int ResourceCapacity { get; set; }

        /// <summary>
        /// 获取或设置资源对象池对象过期秒数。
        /// </summary>
        float ResourceExpireTime { get; set; }

        /// <summary>
        /// 获取或设置资源对象池的优先级。
        /// </summary>
        int ResourcePriority { get; set; }

        BundleManifest BundleManifest { get; set; }
        void SetAssetBundle();

        /// <summary>
        /// 加载assetBundle IBundle
        /// </summary>
        /// <param name="assetBundle"></param>
        void LoadAssetBundle(string assetBundle, LoadAssetCallbacks loadAssetCallbacks);

        /// <summary>
        /// 加载出具体的asset资源
        /// </summary>
        /// <param name="assetBundle"></param>
        void LoadAsset(string assetBundle, LoadAssetCallbacks loadAssetCallbacks);

        /// <summary>
        /// 设置bundleManifest
        /// </summary>
        /// <param name="bundleManifest"></param>
        void SetBundleManifest(BundleManifest bundleManifest);

        IEnumerable StartIDownAssetBundle();
        
             
        /// <summary>
        /// 异步加载场景。
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称。</param>
        /// <param name="priority">加载场景资源的优先级。</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        void LoadScene(string sceneAssetName, int priority, LoadSceneCallbacks loadSceneCallbacks, object userData);

        /// <summary>
        /// 异步卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">要卸载场景资源的名称。</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
        void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks);

        /// <summary>
        /// 异步卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">要卸载场景资源的名称。</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData);
        
    }
}
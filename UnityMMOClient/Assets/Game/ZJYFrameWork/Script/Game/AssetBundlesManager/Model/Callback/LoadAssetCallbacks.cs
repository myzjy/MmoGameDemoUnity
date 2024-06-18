using System;
using FrostEngine;
using ZJYFrameWork.Spring.Utils;

namespace ZJYFrameWork.AssetBundles.Model.Callback
{
    public delegate void LoadAssetSuccessCallback(string assetName, UnityEngine.Object asset, float duration,
        object userData);

    public delegate void LoadAssetFailureCallback(string assetName, LoadResourceStatus status, string errorMessage,
        object userData);

    public delegate void LoadAssetUpdateCallback(string assetName, float progress, object userData);

    public delegate void LoadAssetDependencyAssetCallback(string assetName, string dependencyAssetName, int loadedCount,
        int totalCount, object userData);

    public sealed class LoadAssetCallbacks
    {
        private readonly LoadAssetFailureCallback loadAssetFailureCallback;
        private readonly LoadAssetUpdateCallback loadAssetUpdateCallback;
        private readonly LoadAssetDependencyAssetCallback loadAssetDependencyAssetCallback;

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        public LoadAssetCallbacks(LoadAssetSuccessCallback loadAssetSuccessCallback)
            : this(loadAssetSuccessCallback, null, null, null)
        {
        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        /// <param name="loadAssetFailureCallback">加载资源失败回调函数。</param>
        public LoadAssetCallbacks(LoadAssetSuccessCallback loadAssetSuccessCallback,
            LoadAssetFailureCallback loadAssetFailureCallback)
            : this(loadAssetSuccessCallback, loadAssetFailureCallback, null, null)
        {
        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        /// <param name="loadAssetUpdateCallback">加载资源更新回调函数。</param>
        public LoadAssetCallbacks(LoadAssetSuccessCallback loadAssetSuccessCallback,
            LoadAssetUpdateCallback loadAssetUpdateCallback)
            : this(loadAssetSuccessCallback, null, loadAssetUpdateCallback, null)
        {
        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        /// <param name="loadAssetDependencyAssetCallback">加载资源时加载依赖资源回调函数。</param>
        public LoadAssetCallbacks(LoadAssetSuccessCallback loadAssetSuccessCallback,
            LoadAssetDependencyAssetCallback loadAssetDependencyAssetCallback)
            : this(loadAssetSuccessCallback, null, null, loadAssetDependencyAssetCallback)
        {
        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        /// <param name="loadAssetFailureCallback">加载资源失败回调函数。</param>
        /// <param name="loadAssetUpdateCallback">加载资源更新回调函数。</param>
        public LoadAssetCallbacks(LoadAssetSuccessCallback loadAssetSuccessCallback,
            LoadAssetFailureCallback loadAssetFailureCallback, LoadAssetUpdateCallback loadAssetUpdateCallback)
            : this(loadAssetSuccessCallback, loadAssetFailureCallback, loadAssetUpdateCallback, null)
        {
        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        /// <param name="loadAssetFailureCallback">加载资源失败回调函数。</param>
        /// <param name="loadAssetDependencyAssetCallback">加载资源时加载依赖资源回调函数。</param>
        public LoadAssetCallbacks(LoadAssetSuccessCallback loadAssetSuccessCallback,
            LoadAssetFailureCallback loadAssetFailureCallback,
            LoadAssetDependencyAssetCallback loadAssetDependencyAssetCallback)
            : this(loadAssetSuccessCallback, loadAssetFailureCallback, null, loadAssetDependencyAssetCallback)
        {
        }

        /// <summary>
        /// 初始化加载资源回调函数集的新实例。
        /// </summary>
        /// <param name="loadAssetSuccessCallback">加载资源成功回调函数。</param>
        /// <param name="loadAssetFailureCallback">加载资源失败回调函数。</param>
        /// <param name="loadAssetUpdateCallback">加载资源更新回调函数。</param>
        /// <param name="loadAssetDependencyAssetCallback">加载资源时加载依赖资源回调函数。</param>
        public LoadAssetCallbacks(LoadAssetSuccessCallback loadAssetSuccessCallback,
            LoadAssetFailureCallback loadAssetFailureCallback, LoadAssetUpdateCallback loadAssetUpdateCallback,
            LoadAssetDependencyAssetCallback loadAssetDependencyAssetCallback)
        {
    
            this.LoadAssetSuccessCallback = loadAssetSuccessCallback;
            this.loadAssetFailureCallback = loadAssetFailureCallback;
            this.loadAssetUpdateCallback = loadAssetUpdateCallback;
            this.loadAssetDependencyAssetCallback = loadAssetDependencyAssetCallback;

            this.loadAssetUpdateCallback ??= DefaultOnLoadAssetUpdateCallback;

            this.loadAssetFailureCallback ??= DefaultOnLoadAssetFailureCallback;

            this.loadAssetDependencyAssetCallback ??= DefaultOnLoadAssetDependencyAssetCallback;
        }

        /// <summary>
        /// 获取加载资源成功回调函数。
        /// </summary>
        public LoadAssetSuccessCallback LoadAssetSuccessCallback { get; }

        /// <summary>
        /// 获取加载资源失败回调函数。
        /// </summary>
        public LoadAssetFailureCallback LoadAssetFailureCallback
        {
            get { return loadAssetFailureCallback; }
        }

        /// <summary>
        /// 获取加载资源更新回调函数。
        /// </summary>
        public LoadAssetUpdateCallback LoadAssetUpdateCallback
        {
            get { return loadAssetUpdateCallback; }
        }

        /// <summary>
        /// 获取加载资源时加载依赖资源回调函数。
        /// </summary>
        public LoadAssetDependencyAssetCallback LoadAssetDependencyAssetCallback
        {
            get { return loadAssetDependencyAssetCallback; }
        }

        private void DefaultOnLoadAssetUpdateCallback(string assetName, float progress, object userData)
        {
        }

        private void DefaultOnLoadAssetFailureCallback(string assetName, LoadResourceStatus status, string errorMessage,
            object userData)
        {
            throw new Exception(StringUtils.Format(
                "Asset [asset:{}] is invalid, [status:{}] [errorMessage:{}] [userData:{}]"
                , assetName, status, errorMessage, userData));
        }

        public void DefaultOnLoadAssetDependencyAssetCallback(string assetName, string dependencyAssetName,
            int loadedCount, int totalCount, object userData)
        {
            Debug.Log(
                "DefaultOnLoadAssetDependencyAssetCallback [asset:{}], [dependencyAssetName:{}] [loadedCount:{}] [totalCount:{}] [userData:{}]",
                assetName, dependencyAssetName, loadedCount, totalCount, userData);
        }
    }
}
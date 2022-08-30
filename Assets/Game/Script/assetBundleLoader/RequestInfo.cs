using System.Collections.Generic;
using Framework.AssetBundles.Utilty;
using UnityEngine;

namespace ZJYFrameWork.AssetBundleLoader
{
    //限制使用，只在这个命名空间下
    /// <summary>
    /// 请求的信息 我们想服务器请求的相关
    /// </summary>
    internal class RequestInfo
    {
        /// <summary>
        /// AssetBundle名
        /// </summary>
        public string assetBundleName { get; set; }

        /// <summary>
        /// 加入请求URL或缓存文件路径
        /// </summary>
        public string assetUrlPath { get; set; }

        /// <summary>
        /// 开始请求(WWW开始下载)的时间 可用于判断是否下载结束
        /// </summary>
        public float timeAtRequest { get; set; }

        /// <summary>
        /// 重试的次数 请求这个assetBundle下载错误时，可以像服务器在度请求的次数
        /// 必须限制住，不能无限请求，保证服务器通信
        /// </summary>
        public int retryNum { get; set; }

        /// <summary>
        /// 有依赖关系的请求信息
        /// 这个assetBundle 的依赖的请求信息 list
        /// </summary>
        public List<RequestInfo> dependencies = new List<RequestInfo>();

        /// <summary>
        /// 有依存关系的负载结果
        /// 这个assetBundle 负载结构
        /// </summary>
        public List<AssetBundleLoaderBase.AssetBundleHolder> dependenceHolders =
            new List<AssetBundleLoaderBase.AssetBundleHolder>();

        /// <summary>
        /// 加载的资产包 url 加载的资源包
        /// </summary>
        public AssetBundle assetBundle { get; set; }

        /// <summary>
        /// 请求资产包时的hash值 用于进行对比
        /// </summary>
        public string hashString { get; set; }

        /// <summary>
        /// 卸载时资源也会被丢弃吗?
        /// </summary>
        public bool unloadAllLoadedObjects { get; set; }

        /// <summary>
        /// 加载完成的委派
        /// </summary>
        public AssetBundleLoaderBase.OnLoadedDelegate onLoaded;

        /// <summary>
        /// 发生错误时的委派
        /// </summary>
        public AssetBundleLoaderBase.OnErrorDelegate onError;

        /// <summary>
        /// 发生下载时的委派
        /// </summary>
        public System.Action onDownload;

        /// <summary>
        /// 加载是否完成
        /// </summary>
        public bool loaded;

        /// <summary>
        /// 主要是为了接收依赖请求的错误
        /// </summary>
        public ErrorInfo errorInfo;

        LoadingMethod method;
    }
}
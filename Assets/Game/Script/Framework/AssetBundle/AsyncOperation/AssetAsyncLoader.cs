/// <summary>
/// added by wsh @ 2017.12.22
///  v1 2022.5.27
/// 功能：Asset异步加载器，自动追踪依赖的ab加载进度
/// 说明：一定要所有ab都加载完毕以后再加载asset，所以这里分成两个加载步骤
/// </summary>

using System.Collections.Generic;
using Script.Framework.AssetBundle;

namespace Framework.AssetBundle.AsyncOperation
{
    public class AssetAsyncLoader : BaseAssetAsyncLoader
    {
        private static Queue<AssetAsyncLoader> pool = new Queue<AssetAsyncLoader>();
        private  static int sequence = 0;
        private bool isOver = false;
        private BaseAssetBundleAsyncLoader assetbundleLoader = null;

        public static AssetAsyncLoader Get()
        {
            return pool.Count > 0 ? pool.Dequeue() : new AssetAsyncLoader(++sequence);
        }

        public static void Recycle(AssetAsyncLoader creater)
        {
            pool.Enqueue(creater);
        }
        
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="sequence">计数</param>
        public AssetAsyncLoader(int sequence)
        {
            Sequence = sequence;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="assetName">资源包名</param>
        /// <param name="asset">资源</param>
        public void Init(string assetName, UnityEngine.Object asset)
        {
            AssetName = assetName;
            this.asset = asset;
            assetbundleLoader = null;
            isOver = true;
        }
        
        /// <summary>
        /// 计数引用
        /// </summary>
        public int Sequence
        {
            get;
            protected set;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="assetName">资源包名</param>
        /// <param name="loader">异步基础资产包加载器</param>
        public void Init(string assetName, BaseAssetBundleAsyncLoader loader)
        {
            AssetName = assetName;
            this.asset = null;
            isOver = false;
            assetbundleLoader = loader;
        }
        
        /// <summary>
        /// 资源名
        /// </summary>
        public string AssetName
        {
            get;
            protected set;
        }
        
        /// <summary>
        /// 是否完成
        /// </summary>
        /// <returns></returns>
        public override bool IsDone()
        {
            return isOver;
        }
        
        /// <summary>
        /// 进度
        /// </summary>
        /// <returns></returns>
        public override float Progress()
        {
            if (isDone)
            {
                return 1.0f;
            }

            return assetbundleLoader.progress;
        }

        public override void Update()
        {
            if (isDone)
            {
                return;
            }

            isOver = assetbundleLoader.isDone;
            if (!isOver)
            {
                return;
            }

            asset = AssetBundleManager.Instance.GetAssetCache(AssetName);
            assetbundleLoader.Dispose();
        }

        public override void Dispose()
        {
            isOver = true;
            AssetName = null;
            asset = null;
            Recycle(this);
        }
    }
}
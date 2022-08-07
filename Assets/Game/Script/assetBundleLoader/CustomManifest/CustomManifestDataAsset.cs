using UnityEngine;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.AssetBundleLoader.CustomManifest
{
    public abstract class CustomManifestDataAsset<T> : ScriptableObject where T : BundleData
    {
        [SerializeField] private T[] mData;

        public T[] Data => mData;

        [SerializeField] private BundleDataDependencies[] mDependencies;

        public BundleDataDependencies[] Dependencies => mDependencies;


        /// <summary>
        /// 返回自定义的Manifest。
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public virtual CustomManifest<T> CreateManifest(string version)
        {
            return new CustomManifest<T>(version, Data, Dependencies);
        }
        public void SetData(T[] data, BundleDataDependencies[] dependencies)
        {
            mData = data;
            mDependencies = dependencies;
        }

    }
}
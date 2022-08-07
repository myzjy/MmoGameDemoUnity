using System;
using System.Collections.Generic;
using ZJYFrameWork.AssetBundleLoader.Manifest;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.AssetBundleLoader.CustomManifest
{
    public class CustomManifest<T> : IManifest where T : BundleData
    {
        private readonly string _mVersion;
        private readonly T[] _mData;
        private readonly BundleDataDependencies[] _mDependencies;
        private readonly Dictionary<string, T> _mDic;
        private string[] _mNames;
        private readonly string[] _mEmptyStringArray = Array.Empty<string>();
        private readonly List<string> _mTempNameList = new List<string>();

        public CustomManifest(string version, T[] data, BundleDataDependencies[] dependencies)
        {
            _mVersion = version;
            _mData = data;
            _mDependencies = dependencies;
            _mDic = new Dictionary<string, T>(_mData.Length);
            foreach (var bundleData in _mData)
            {
                _mDic.Add(bundleData.Name, bundleData);
            }
        }

        /// <summary>
        /// 获取指定的捆绑数据。
        /// 如果没有捆绑数据，则返回false。
        /// </summary>
        protected bool TryGetData(string assetBundleName, out T data)
        {
            return _mDic.TryGetValue(assetBundleName, out data);
        }

        /// <summary>
        /// 把Manifest的版本返回。
        /// 必要时请用派生类重写。
        /// </summary>
        public string GetManifestVersion()
        {
            return _mVersion;
        }

        /// <summary>
        /// 返还所有的资产包名称。
        /// 必要时请用派生类重写。
        /// </summary>
        public virtual string[] GetAllAssetBundles()
        {
            if (_mNames != null) return _mNames;
            _mNames = new string[_mDic.Count];
            _mDic.Keys.CopyTo(_mNames, 0);
            return _mNames;
        }

        /// <summary>
        /// 递归地获取所有的依赖关系
        /// </summary>
        /// <param name="dep">BundleData的依赖信息数据</param>
        /// <param name="depNames">各种依赖名字</param>
        private void GetAllDependenciesImpl(BundleDataDependencies dep, List<string> depNames)
        {
            if (dep == null)
            {
                return;
            }

            //再帰的に依存先の名前を取得
            var indices = dep.NameIndices;
            foreach (var nameIndex in indices)
            {
                var data = _mData[nameIndex];
                //すでに挿入済みであれば再帰処理は行わない
                if (depNames.Contains(data.Name)) continue;
                depNames.Add(data.Name);
                GetAllDependenciesImpl(_mDependencies[data.DependenciesIndex], depNames);
            }
        }

        /// <summary>
        /// 获取与依赖数据相关的所有资产包名称。
        /// </summary>
        protected string[] GetAllDependencies(BundleDataDependencies dep)
        {
            if (dep.AllDependenciesCache != null)
            {
                return dep.AllDependenciesCache;
            }

            _mTempNameList.Clear();
            GetAllDependenciesImpl(dep, _mTempNameList);
            var allDependencies = _mTempNameList.ToArray();
            dep.SetAllDependenciesCache(allDependencies);
            return allDependencies;
        }

        /// <summary>
        /// 从资产包名称中获取所有有依赖关系的资产包名称。
        /// 在循环参照的情况下，自身可能包含在序列中。
        /// 必要时请用派生类重写。
        /// </summary>
        public virtual string[] GetAllDependencies(string assetBundleName)
        {
            if (!TryGetData(assetBundleName, out var data))
            {
                return _mEmptyStringArray;
            }

            if (data.DependenciesIndex < 0 || data.DependenciesIndex >= _mDependencies.Length)
            {
                return _mEmptyStringArray;
            }

            return GetAllDependencies(_mDependencies[data.DependenciesIndex]);
        }

        /// <summary>
        /// 返还资产包的版本。
        /// 必要时请用派生类重写。
        /// </summary>
        public virtual string GetAssetBundleVersion(string assetBundleName)
        {
            return !TryGetData(assetBundleName, out var data) ? assetBundleName.GetHashCode().ToString() : data.Version;
        }

        /// <summary>
        /// 返回资产包的CRC。
        /// 必要时请用派生类重写。
        /// </summary>
        public virtual uint GetAssetBundleCrc(string assetBundleName)
        {
            return !TryGetData(assetBundleName, out var data) ? 0 : data.Crc;
        }

        /// <summary>
        /// 从资产包名称中获取文件大小。
        /// 必要时请用派生类重写。
        /// </summary>
        public virtual int GetAssetBundleSize(string assetBundleName)
        {
            return !TryGetData(assetBundleName, out var data) ? 0 : data.Size;
        }
    }
}
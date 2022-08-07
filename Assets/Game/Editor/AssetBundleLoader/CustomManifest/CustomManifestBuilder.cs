using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ZJYFrameWork.AssetBundleLoader.CustomManifest;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.AssetBundleLoader.Build
{
    /// <summary>
    /// 独立Manifest的建设者。请在单份声明的构建处理之后执行。
    /// </summary>
    /// <typeparam name="TManifest"></typeparam>
    /// <typeparam name="TUBundleData"></typeparam>
    public abstract class CustomManifestBuilder<TManifest, TUBundleData> where
        TManifest : CustomManifestDataAsset<TUBundleData>
        where TUBundleData : BundleData
    {
        /// <summary>
        /// 构建资产的文件路径
        /// </summary>
        protected string DataAssetPath { get; set; }

        /// <summary>
        /// 资产包的输出文件夹
        /// </summary>
        protected string BundleOutputPath { get; set; }

        /// <summary>
        /// build后的AssetBundleManifest
        /// </summary>
        protected AssetBundleManifest Manifest { get; set; }

        /// <summary>
        /// 从每个单表中获得的全部资产包名称
        /// </summary>
        protected string[] AllNames { get; set; }

        /// <summary>
        /// 每个资产包的直接依赖信息
        /// </summary>
        protected BundleDataDependencies[] Dependencies { get; set; }

        /// <summary>
        /// 每个资产包的直接依赖信息
        /// Key是对依赖的包名称的列表进行排序，并以区间划分的东西。
        /// </summary>
        protected Dictionary<string, BundleDataDependencies> DependenciesDic { get; set; }

        protected CustomManifestBuilder(string dataAssetPath, string bundleOutputPath, AssetBundleManifest manifest)
        {
            DataAssetPath = dataAssetPath;
            BundleOutputPath = bundleOutputPath;
            Manifest = manifest;
            AllNames = manifest.GetAllAssetBundles();
            CreateDependenciesData();
        }

        private void CreateDependenciesData()
        {
            var dic = new Dictionary<string, BundleDataDependencies>();
            var list = new List<BundleDataDependencies>();
            foreach (var name in AllNames)
            {
                var deps = Manifest.GetDirectDependencies(name);
                System.Array.Sort(deps);
                var key = string.Join(",", deps);
                if (dic.ContainsKey(key)) continue;
                var depData = new BundleDataDependencies(ConvertNameIndices(deps));
                list.Add(depData);
                dic[key] = depData;
            }

            Dependencies = list.ToArray();
            DependenciesDic = dic;
        }

        private int[] ConvertNameIndices(IReadOnlyList<string> names)
        {
            var nameIndices = new int[names.Count];
            for (var i = 0; i < nameIndices.Length; i++)
            {
                nameIndices[i] = System.Array.IndexOf(AllNames, names[i]);
            }

            return nameIndices;
        }

        /// <summary>
        /// 在Manifest有变更的情况下更新资产
        /// 发生改变时返回true
        /// </summary>
        protected virtual bool TryUpdateData(bool forceUpdate = false)
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            var oldAsset = AssetDatabase.LoadAssetAtPath<TManifest>(DataAssetPath);
            var newAsset = CreateDataAsset();
            var update = forceUpdate;

            if (!update)
            {
                update = CheckUpdateManifest(oldAsset, newAsset);
            }

            if (!update) return false;
            UpdateDataAsset(newAsset);
            Debug.Log("update manifest asset.");

            return true;
        }

        /// <summary>
        /// 制作清单中使用的资产包数据。
        /// 请用派生类实现。
        /// </summary>
        protected abstract TUBundleData CreateBundleData(string name);

        /// <summary>
        /// 制作具有Manifest用数据的资产。
        /// 如果需要的话请用派生类重写。
        /// </summary>
        protected virtual TManifest CreateDataAsset()
        {
            var dataList = new TUBundleData[AllNames.Length];
            for (var i = 0; i < AllNames.Length; i++)
            {
                dataList[i] = CreateBundleData(AllNames[i]);
            }

            var dataAsset = ScriptableObject.CreateInstance<TManifest>();
            dataAsset.SetData(dataList, Dependencies);
            return dataAsset;
        }

        /// <summary>
        /// 用DataAssetPath指定的路径更新资产。
        /// </summary>
        private void UpdateDataAsset(ScriptableObject asset)
        {
            var dirPath = Path.GetDirectoryName(DataAssetPath);
            if (!Directory.Exists(dirPath))
            {
                if (dirPath != null) Directory.CreateDirectory(dirPath);
                AssetDatabase.Refresh();
            }

            if (File.Exists(DataAssetPath))
            {
                AssetDatabase.DeleteAsset(DataAssetPath);
            }

            AssetDatabase.CreateAsset(asset, DataAssetPath);
        }

        /// <summary>
        /// 返回资产包名称使用的依赖信息的索引。
        /// </summary>
        protected int GetDependenciesIndex(string assetBundleName)
        {
            var deps = Manifest.GetDirectDependencies(assetBundleName);
            System.Array.Sort(deps);
            var key = string.Join(",", deps);
            return System.Array.FindIndex(Dependencies, x => x == DependenciesDic[key]);
        }

        /// <summary>
        /// 返回已构建资产包的文件路径。
        /// </summary>
        private string GetFilePath(string assetBundleName)
        {
            return Path.Combine(BundleOutputPath, assetBundleName);
        }

        /// <summary>
        /// 返回已构建资产包的文件大小
        /// </summary>
        protected int GetFileSize(string assetBundleName, double divisor = 1)
        {
            FileInfo info = new FileInfo(GetFilePath(assetBundleName));
            return (int)(info.Length / divisor);
        }

        /// <summary>
        /// 返回已构建资产包的CRC。
        /// </summary>
        protected uint GetAssetBundleCrc(string assetBundleName)
        {
            var path = GetFilePath(assetBundleName);
            if (BuildPipeline.GetCRCForAssetBundle(path, out var crc))
            {
                return crc;
            }
            else
            {
                throw new IOException($"Error GetCRCForAssetBundle : {path}");
            }
        }

        /// <summary>
        /// 返回以包名称为Key的包数据的词典。
        /// </summary>
        private Dictionary<string, TUBundleData> GetBundleDataDic(TManifest manifestAsset)
        {
            var dic = new Dictionary<string, TUBundleData>(manifestAsset.Data.Length);
            foreach (var data in manifestAsset.Data)
            {
                dic.Add(data.Name, data);
            }

            return dic;
        }

        /// <summary>
        /// 如果资产包的信息相同，则返回true。
        /// 请用派生班重写。
        /// </summary>
        protected virtual bool IsEqualBundleData(TUBundleData x, TUBundleData y)
        {
            if (x.Crc != y.Crc || x.Version != y.Version || x.Size != y.Size)
            {
                return false;
            }

            return true;
        }

        protected virtual bool CheckUpdateManifest(TManifest oldAsset, TManifest newAsset)
        {
            if (oldAsset == null ||
                oldAsset.Data == null ||
                oldAsset.Dependencies == null ||
                oldAsset.Data.Length != newAsset.Data.Length ||
                oldAsset.Dependencies.Length != newAsset.Dependencies.Length)
            {
                return true;
            }

            var oldManifest = oldAsset.CreateManifest("old");
            var newManifest = newAsset.CreateManifest("new");
            var oldDic = GetBundleDataDic(oldAsset);
            var newDic = GetBundleDataDic(newAsset);
            foreach (var kvp in newDic)
            {
                if (!oldDic.TryGetValue(kvp.Key, out var oldData))
                {
                    //有一把不存在的钥匙。
                    return true;
                }

                if (!IsEqualBundleData(oldData, kvp.Value))
                {
                    //内容变了
                    return true;
                }

                var oldDeps = oldManifest.GetAllDependencies(oldData.Name);
                var newDeps = newManifest.GetAllDependencies(kvp.Key);
                if (oldDeps.Length != newDeps.Length)
                {
                    //依存情報の長さが違う
                    return true;
                }

                System.Array.Sort(oldDeps);
                System.Array.Sort(newDeps);
                if (oldDeps.Where((t, i) => t != newDeps[i]).Any())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 当manifest的数据被更新时，进行独自manifest的构建处理。
        /// </summary>
        public bool TryBuild(string dataAssetBundleName, string manifestName, BuildAssetBundleOptions option,
            BuildTarget target, bool forceUpdate = false)
        {
            if (TryUpdateData(forceUpdate))
            {
                Build(dataAssetBundleName, manifestName, option, target);
            }

            return false;
        }

        /// <summary>
        /// 建立自己Manifest
        /// 不更新Manifest的数据等。
        /// </summary>
        private void Build(string dataAssetBundleName, string manifestName, BuildAssetBundleOptions option,
            BuildTarget target)
        {
            var manifestPaths = new[]
            {
                $"{BundleOutputPath}/{manifestName}",
                $"{BundleOutputPath}/{manifestName}.manifest",
            };
            var tmpManifestPaths = new[]
            {
                $"{BundleOutputPath}/{manifestName}_tmp",
                $"{BundleOutputPath}/{manifestName}_tmp.manifest",
            };

            CopyFiles(manifestPaths, tmpManifestPaths);
            AssetDatabase.Refresh();

            var build = new AssetBundleBuild
            {
                assetBundleName = dataAssetBundleName,
                assetNames = new[] { DataAssetPath }
            };
            var assetBundleManifest =
                BuildPipeline.BuildAssetBundles(BundleOutputPath, new[] { build }, option, target);
            Debug.Log($"hash:{assetBundleManifest.GetAssetBundleHash(dataAssetBundleName)}");
            MoveFiles(tmpManifestPaths, manifestPaths);
            AssetDatabase.Refresh();
        }

        private static void CopyFiles(IReadOnlyList<string> src, IReadOnlyList<string> dst)
        {
            for (var i = 0; i < src.Count; ++i)
            {
                if (!File.Exists(src[i]))
                {
                    continue;
                }

                File.Copy(src[i], dst[i], true);
            }
        }

        private static void MoveFiles(IReadOnlyList<string> src, IReadOnlyList<string> dst)
        {
            for (var i = 0; i < src.Count; ++i)
            {
                if (!File.Exists(src[i]))
                {
                    continue;
                }

                if (File.Exists(dst[i]))
                {
                    File.Delete(dst[i]);
                }

                File.Move(src[i], dst[i]);
            }
        }
    }
}
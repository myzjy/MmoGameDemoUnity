using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.AssetBundles.Bundles
{
    /// <summary>
    /// The manifest about the AssetBundle.
    /// </summary>
    [Serializable]
    public sealed class BundleManifest : ISerializationCallbackReceiver
    {
        [SerializeField] private BundleInfo[] bundleInfos;
        [SerializeField] private string defaultVariant;
        [SerializeField] private string version;

        private readonly object _lock = new object();

        [NonSerialized] private string[] _activeVariants;

        private Action _activeVariantsChanged;
        [NonSerialized] private Dictionary<string, BundleInfo> _bundles = new Dictionary<string, BundleInfo>();

        /// <summary>
        /// BundleManifest
        /// </summary>
        public BundleManifest() : this(null, "1.0.0")
        {
        }

        /// <summary>
        /// BundleManifest
        /// </summary>
        /// <param name="bundleInfos">All of the BundleInfos.</param>
        /// <param name="version">The version of the AssetBundle data.</param>
        /// <param name="defaultVariant">The default variant's name.</param>
        public BundleManifest(List<BundleInfo> bundleInfos, string version, string defaultVariant) : this(bundleInfos,
            version, defaultVariant, null)
        {
        }

        /// <summary>
        /// BundleManifest
        /// </summary>
        /// <param name="bundleInfos">All of the BundleInfos.</param>
        /// <param name="version">The version of the AssetBundle data.</param>
        /// <param name="defaultVariant">The default variant's name.</param>
        /// <param name="variants">All of the variants has been activated.According to the priority ascending.</param>
        public BundleManifest(List<BundleInfo> bundleInfos, string version, string defaultVariant = null,
            string[] variants = null)
        {
            this.bundleInfos = bundleInfos != null ? bundleInfos.ToArray() : Array.Empty<BundleInfo>();

            this.defaultVariant = defaultVariant ?? this.AnalyzeDefaultVariant(bundleInfos);

            this.version = version;
            this.ActiveVariants = variants ?? new[] { this.defaultVariant };
        }

        /// <summary>
        ///  Gets the version of the AssetBundle data.
        /// </summary>
        public string Version => this.version;

        /// <summary>
        ///  Gets or sets all of the variants,they have been activated.
        /// </summary>
        public string[] ActiveVariants
        {
            get => this._activeVariants;
            set
            {
                List<string> variants = new List<string>() { "" };
                if (!variants.Contains(this.defaultVariant))
                    variants.Add(this.defaultVariant);

                if (value != null && value.Length > 0)
                {
                    foreach (string variant in value)
                    {
                        if (string.IsNullOrEmpty(variant))
                            continue;

                        if (variants.Contains(variant))
                            continue;

                        variants.Add(variant);
                    }
                }

                this._activeVariants = variants.ToArray();
                try
                {
                    if (this._activeVariantsChanged != null)
                        this._activeVariantsChanged();
                }
                catch (Exception)
                {
                    // ignored
                }

                Initialize();
            }
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            this.ActiveVariants = this.defaultVariant != null ? new[] { this.defaultVariant } : new[] { "" };
        }

        /// <summary>
        /// Convert from JSON string to BundleManifest object.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static BundleManifest Parse(string json)
        {
            return JsonUtility.FromJson<BundleManifest>(json);
        }

        public event Action ActiveVariantsChanged
        {
            add
            {
                lock (_lock)
                {
                    this._activeVariantsChanged += value;
                }
            }
            remove
            {
                lock (_lock)
                {
                    this._activeVariantsChanged -= value;
                }
            }
        }

        /// <summary>
        /// Analysis of the default variants.
        /// </summary>
        /// <param name="bundleInfoList"></param>
        /// <returns></returns>
        private string AnalyzeDefaultVariant(List<BundleInfo> bundleInfoList)
        {
            if (bundleInfoList == null || bundleInfoList.Count <= 0)
                return "";

            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (var info in bundleInfoList)
            {
                if (string.IsNullOrEmpty(info.Variant))
                {
                    return "";
                }

                if (!dict.ContainsKey(info.Variant))
                {
                    dict[info.Variant] = 1;
                }
                else
                {
                    dict[info.Variant] = dict[info.Variant] + 1;
                }
            }

            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
            using var it = dict.GetEnumerator();
            while (it.MoveNext())
            {
                list.Add(it.Current);
            }

            list.Sort((x, y) => y.Value.CompareTo(x.Value));

            return list[0].Key;
        }

        private BundleInfo Compare(BundleInfo info1, BundleInfo info2)
        {
            if (this._activeVariants != null && this._activeVariants.Length > 0)
            {
                int index1 = Array.IndexOf(this._activeVariants, info1.Variant);
                int index2 = Array.IndexOf(this._activeVariants, info2.Variant);
                return index1 > index2 ? info1 : info2;
            }

            return info2;
        }

        private void Initialize()
        {
            _bundles.Clear();

            if (this.bundleInfos == null || this.bundleInfos.Length <= 0)
                return;

            foreach (var info in this.bundleInfos)
            {
                if (_bundles.TryGetValue(info.Name, out var old) && old != null)
                    _bundles[info.Name] = Compare(old, info);
                else
                    _bundles[info.Name] = info;
            }
        }

        public bool Contains(string bundleName)
        {
            if (this.bundleInfos == null || this.bundleInfos.Length <= 0)
                return false;

            var name = Path.GetFilePathWithoutExtension(bundleName);
            var variant = Path.GetExtension(bundleName);
            return Array.Exists(this.bundleInfos, b => b.Name == name && b.Variant == variant);
        }

        /// <summary>
        /// Gets the BundleInfo for the given name.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public BundleInfo GetBundleInfo(string bundleName)
        {
            if (this._bundles.TryGetValue(Path.GetFilePathWithoutExtension(bundleName), out var info))
                return info;
            return null;
        }

        /// <summary>
        /// Gets the BundleInfos for the given name.
        /// </summary>
        /// <param name="bundleNames"></param>
        /// <returns></returns>
        public BundleInfo[] GetBundleInfos(params string[] bundleNames)
        {
            if (bundleNames == null || bundleNames.Length <= 0)
            {
                return Array.Empty<BundleInfo>();
            }

            List<BundleInfo> list = new List<BundleInfo>();
            foreach (var item in bundleNames)
            {
                var name = Path.GetFilePathWithoutExtension(item);
                if (_bundles.TryGetValue(name, out var info))
                {
                    if (info != null && !list.Contains(info))
                    {
                        list.Add(info);
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 获取给定名称的所有依赖项。
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public BundleInfo[] GetDependencies(string bundleName, bool recursive)
        {
            BundleInfo info = this.GetBundleInfo(bundleName);
            if (info == null)
            {
                return Array.Empty<BundleInfo>();
            }

            List<BundleInfo> list = new List<BundleInfo>();
            this.GetDependencies(info, info, recursive, list);
            return list.ToArray();
        }

        private void GetDependencies(BundleInfo root, BundleInfo info, bool recursive, List<BundleInfo> list)
        {
            string[] dependencyNames = info.Dependencies;
            if (dependencyNames == null || dependencyNames.Length <= 0)
                return;

            BundleInfo[] dependencies = this.GetBundleInfos(dependencyNames);
            foreach (var dependency in dependencies)
            {
                if (dependency.Equals(root))
                {
#if UNITY_EDITOR || DEVELOP_BUILD && ENABLE_LOG
                    Debug.LogError($"它在'[{root.Name}]'和'[{info.Name}]'之间有一个循环引用，建议重新分配它们的资产。");
#endif
                    continue;
                    //throw new LoopingReferenceException(string.Format("There is a error occurred.It has an unresolvable loop reference between '{0}' and '{1}'.", root.Name, info.Name));
                }

                if (list.Contains(dependency))
                {
                    continue;
                }

                list.Add(dependency);

                if (recursive)
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    this.GetDependencies(root, dependency, recursive, list);
                }
            }
        }

        /// <summary>
        /// 获取所有的 BundleInfo。
        /// </summary>
        /// <returns></returns>
        public BundleInfo[] GetAll()
        {
            return this.bundleInfos;
        }

        /// <summary>
        /// 获取所有已激活的BundleInfos。
        /// </summary>
        /// <returns></returns>
        public BundleInfo[] GetAllActivated()
        {
            BundleInfo[] getAllActivated = new BundleInfo[this._bundles.Count];
            using var it = this._bundles.GetEnumerator();
            int i = 0;
            while (it.MoveNext())
            {
                getAllActivated[i++] = it.Current.Value;
            }

            return getAllActivated;
        }

        /// <summary>
        /// 从BundleManifest转换为JSON字符串
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        /// <summary>
        /// 从BundleManifest转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToJson();
        }
    }
}
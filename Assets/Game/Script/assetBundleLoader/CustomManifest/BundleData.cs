using UnityEngine;
using UnityEngine.Serialization;

namespace ZJYFrameWork.AssetBundleLoader.CustomManifest
{
    [System.Serializable]
    public class BundleData
    {
        [SerializeField] private string m_Name;

        /// <summary>
        /// ReSharper disable once CommentTypo
        /// 资产包名称   含.assetbundle后缀
        /// </summary>
        public string Name
        {
            get { return m_Name; }
        }

        [FormerlySerializedAs("m_Version")] [SerializeField]
        private string mVersion;

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version => mVersion;

        [FormerlySerializedAs("m_Crc")] [SerializeField]
        private uint mCrc;

        /// <summary>
        /// CRC 校验
        /// </summary>
        public uint Crc => mCrc;

        [FormerlySerializedAs("m_Size")] [SerializeField]
        private int mSize;

        /// <summary>
        /// 文件大小
        /// </summary>
        public int Size => mSize;

        [FormerlySerializedAs("m_DependenciesIndex")] [SerializeField]
        private int mDependenciesIndex;

        /// <summary>
        /// 拥有现有数据BundleDataDependencies的Index
        /// </summary>
        public int DependenciesIndex => mDependenciesIndex;

        public BundleData(string name, string version, uint crc, int size, int dependenciesIndex)
        {
            m_Name = name;
            mVersion = version;
            mCrc = crc;
            mSize = size;
            mDependenciesIndex = dependenciesIndex;
        }
    }

    /// <summary>
    /// BundleData的依赖信息数据
    /// </summary>
    public class BundleDataDependencies
    {
        [SerializeField] private  int[] _mNameIndices;

        /// <summary>
        /// BundleDataのIndexの配列
        /// </summary>
        public int[] NameIndices => _mNameIndices;

        public string[] AllDependenciesCache { get; private set; }

        public BundleDataDependencies(int[] indices)
        {
            _mNameIndices = indices;
        }

        public void SetAllDependenciesCache(string[] names)
        {
            AllDependenciesCache = names;
        }
    }
}
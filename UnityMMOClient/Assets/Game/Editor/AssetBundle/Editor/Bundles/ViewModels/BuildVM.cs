using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using AssetBundles;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using UnityEngine.Serialization;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.AssetBundles.BundleUtils;
using ZJYFrameWork.Attributes;
using ZJYFrameWork.Bundles.Editors;
using ZJYFrameWork.Security.Cryptography;
using Path = System.IO.Path;

namespace ZJYFrameWork.AssetBundles.EditorAssetBundle.Editors
{
    [Serializable]
    public class BuildVM : AbstractViewModel
    {
        private const string PREFIX = "ZJYFrameWork::Bundle::";
        public const string PATH = "Assets/Game/Editor/AssetBundle/Editor/BuildSetting.json";
        public const string DEFAULT_OUTPUT = "AssetBundles";

        [SerializeField] private CompressOptions compression;
        [SerializeField] private string outputPath = DEFAULT_OUTPUT;
        [SerializeField] private bool usePlayerSettingVersion;

        /// <summary>
        /// app版本号
        /// </summary>
        [SerializeField] private string appDataVersion;

        /// <summary>
        /// 资源版本号
        /// </summary>
        [SerializeField] private string resDataVersion;

        /// <summary>
        /// 客户端脚本版本号
        /// </summary>
        [SerializeField] private string silenceDataVersion;

        [SerializeField] private BuildAssetBundleOptions buildAssetBundleOptions;
        [SerializeField] private bool copyToStreaming;
        [SerializeField] private bool useHashFilename;
        [SerializeField] private bool encryption;

        /// <summary>
        /// 加密格式
        /// </summary>
        [SerializeField] private Algorithm algorithm = Algorithm.AES128_CBC_PKCS7;

        #region 秘钥

        [SerializeField] private string iv;
        [SerializeField] private string key;

        #endregion

        [SerializeField] private EncryptionFilterType filterType;
        [SerializeField] private string filterExpression;
        [SerializeField] private string bundleNames;

        [NonSerialized] private BuildTarget buildTarget;
        [NonSerialized] private bool advancedSettings = false;

        public BuildVM()
        {
            // this.appDataVersion = "1000001";
            this.copyToStreaming = true;
            this.useHashFilename = true;
            this.encryption = false;
        }

        public BuildTarget BuildTarget
        {
            get { return this.buildTarget; }
            set
            {
                this.Set<BuildTarget>(ref this.buildTarget, value, () => BuildTarget);
                // EditorPrefs.SetInt(PREFIX + "BuildTarget", (int)value);
            }
        }

        public CompressOptions Compression
        {
            get { return this.compression; }
            set
            {
                if (this.Set<CompressOptions>(ref this.compression, value, () => Compression))
                    this.Save();
            }
        }

        public string OutputPath
        {
            get { return this.outputPath; }
            set
            {
                if (this.Set<string>(ref this.outputPath, value, () => OutputPath))
                    this.Save();
            }
        }

        public bool UsePlayerSettingVersion
        {
            get { return this.usePlayerSettingVersion; }
            set
            {
                if (this.Set<bool>(ref this.usePlayerSettingVersion, value, () => UsePlayerSettingVersion))
                {
                    this.RaisePropertyChanged("DataVersion");
                    this.Save();
                }
            }
        }

        public string AppDataVersion
        {
            get
            {
                if (this.usePlayerSettingVersion)
                    return PlayerSettings.bundleVersion;

                return this.appDataVersion;
            }
            set
            {
                if (this.usePlayerSettingVersion)
                {
                    if (string.IsNullOrEmpty(value) || value.Equals(PlayerSettings.bundleVersion))
                        return;

                    PlayerSettings.bundleVersion = value;
                    this.RaisePropertyChanged("DataVersion");
                    return;
                }

                if (this.Set<string>(ref this.appDataVersion, value, () => AppDataVersion))
                    this.Save();
            }
        }

        public bool AdvancedSettings
        {
            get { return this.advancedSettings; }
            set
            {
                if (this.Set<bool>(ref this.advancedSettings, value, () => AdvancedSettings))
                    EditorPrefs.SetBool(PREFIX + "AdvancedSettings", value);
            }
        }

        public BuildAssetBundleOptions BuildAssetBundleOptions
        {
            get { return this.buildAssetBundleOptions; }
            set
            {
                if (this.Set<BuildAssetBundleOptions>(ref this.buildAssetBundleOptions, value,
                        () => BuildAssetBundleOptions))
                    this.Save();
            }
        }

        public bool CopyToStreaming
        {
            get => this.copyToStreaming;
            set
            {
                if (this.Set<bool>(ref this.copyToStreaming, value, () => CopyToStreaming))
                    this.Save();
            }
        }

        public bool UseHashFilename
        {
            get { return this.useHashFilename; }
            set
            {
                if (this.Set<bool>(ref this.useHashFilename, value, () => UseHashFilename))
                    this.Save();
            }
        }

        public bool Encryption
        {
            get => this.encryption;
            set
            {
                if (this.Set<bool>(ref this.encryption, value, () => Encryption))
                    this.Save();
            }
        }

        public Algorithm Algorithm
        {
            get { return this.algorithm; }
            set
            {
                if (this.Set<Algorithm>(ref this.algorithm, value, () => Algorithm))
                    this.Save();
            }
        }

        public string IV
        {
            get { return this.iv; }
            set
            {
                if (this.Set<string>(ref this.iv, value, () => IV))
                    this.Save();
            }
        }

        public string KEY
        {
            get { return this.key; }
            set
            {
                if (this.Set<string>(ref this.key, value, () => KEY))
                    this.Save();
            }
        }

        public EncryptionFilterType FilterType
        {
            get { return this.filterType; }
            set
            {
                if (this.Set<EncryptionFilterType>(ref this.filterType, value, () => FilterType))
                    this.Save();
            }
        }

        public string FilterExpression
        {
            get { return this.filterExpression; }
            set
            {
                if (this.Set<string>(ref this.filterExpression, value, () => FilterExpression))
                    this.Save();
            }
        }

        public string BundleNames
        {
            get { return this.bundleNames; }
            set
            {
                if (this.Set<string>(ref this.bundleNames, value, () => BundleNames))
                    this.Save();
            }
        }

        public override void OnEnable()
        {
            this.Load();

            this.buildTarget = (BuildTarget)EditorPrefs.GetInt(PREFIX + "BuildTarget",
                (int)EditorUserBuildSettings.activeBuildTarget);
            this.advancedSettings = EditorPrefs.GetBool(PREFIX + "AdvancedSettings", this.advancedSettings);
        }

        public override void OnDisable()
        {
        }

        protected virtual void Load()
        {
            try
            {
                if (!File.Exists(PATH))
                    return;

                string json = File.ReadAllText(PATH);
                JsonUtility.FromJsonOverwrite(json, this);
            }
            catch (Exception e)
            {
                Debug.LogWarningFormat("Loads {0} failure. Error:{1}", PATH, e);
            }
        }

        /// <summary>
        /// 用json保存
        /// </summary>
        protected virtual void Save()
        {
            try
            {
                if (!File.Exists(PATH))
                {
                    FileInfo info = new FileInfo(PATH);
                    if (info.Directory is { Exists: false })
                        info.Directory.Create();
                }

                string json = JsonUtility.ToJson(this);
                File.WriteAllText(PATH, json);
            }
            catch (System.Exception e)
            {
                Debug.LogWarningFormat("Write {0} failure. Error:{1}", PATH, e);
            }
        }

        public virtual bool VersionExists()
        {
            BundleBuilder builder = new BundleBuilder();
            string versionOutput = builder.GetVersionOutput(this.OutputPath, this.BuildTarget, this.AppDataVersion);
            return Directory.Exists(versionOutput);
        }

        static Stopwatch sw = new Stopwatch();
        static Stopwatch swCopy = new Stopwatch();
        static Stopwatch swCopyDll = new Stopwatch();

        public static List<string> AOTMetaAssemblyNames { get; } = new List<string>()
        {
            "mscorlib.dll",
            "System.dll",
            "System.Core.dll",
        };

        /// <summary>
        /// copy dll
        /// </summary>
        public static void CopyDllAb()
        {
            swCopy = new Stopwatch();
            var target = EditorUserBuildSettings.activeBuildTarget;
            CompileDllCommand.CompileDll(target);

            string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
            string aotAssembliesDstDir = $"{Application.dataPath}/Game/AssetBundles/dllAb/";
            sw = new Stopwatch();
            if (Directory.Exists(aotAssembliesDstDir))
            {
                sw.Start();
                Debug.Log("╔======================================╗");
                Debug.Log($"┃     删除目录{aotAssembliesDstDir}                  ┃");
                Debug.Log("╚======================================╝");
                string[] filenames = Directory.GetFileSystemEntries(aotAssembliesDstDir);
                if (filenames.Length > 0)
                {
                    Debug.Log($"当前目录下有文件,开始删除文件");
                    foreach (var pathString in filenames)
                    {
                        Debug.Log($"删除文件路径：{pathString}");
                        File.Delete(pathString);
                    }
                }

                //移除 目录
                Directory.Delete(aotAssembliesDstDir);
                sw.Stop();
                Debug.Log($"删除目录{aotAssembliesDstDir}成功 耗时:{sw.ElapsedMilliseconds}ms");
                sw = new Stopwatch();
                sw.Start();
                Debug.Log($"开始创建目录{aotAssembliesDstDir}");
                //移除之后
                Directory.CreateDirectory(aotAssembliesDstDir);
                sw.Stop();
                Debug.Log($"创建目录{aotAssembliesDstDir}成功 耗时:{sw.ElapsedMilliseconds}ms");
            }
            else
            {
                //创建对应目录
                sw = new Stopwatch();
                sw.Start();
                Debug.Log($"开始创建目录{aotAssembliesDstDir}");
                Directory.CreateDirectory(aotAssembliesDstDir);
                sw.Stop();
                Debug.Log($"创建目录{aotAssembliesDstDir}成功 耗时:{sw.ElapsedMilliseconds}ms");
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            Debug.Log($"复制aot {aotAssembliesDstDir} 下相关dll");
            sw = new Stopwatch();
            sw.Start();
            foreach (var dll in AOTMetaAssemblyNames)
            {
                //dll aot 具体文件夹位置
                string srcDllPath = $"{aotAssembliesSrcDir}/{dll}";
                if (!File.Exists(srcDllPath))
                {
                    Debug.LogError(
                        $"ab中添加AOT补充元数据dll:{srcDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                    continue;
                }

                swCopyDll = new Stopwatch();
                var s = dll.Replace(".dll", "");
                swCopyDll.Start();
                string dllBytesPath = $"{aotAssembliesDstDir}/{s}.bytes";
                Debug.Log($"复制{srcDllPath}--->{dllBytesPath}");
                File.Copy(srcDllPath, dllBytesPath, true);
                swCopyDll.Stop();
                Debug.Log($"复制{srcDllPath}--->{dllBytesPath}   复制成功 耗时:{swCopyDll.ElapsedMilliseconds}ms");
                Debug.Log($"[CopyAOTAssembliesToPath] copy AOT dll {srcDllPath} -> {dllBytesPath}");
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            sw.Stop();
            Debug.Log($"复制{aotAssembliesDstDir}成功 耗时:{sw.ElapsedMilliseconds}ms");
            //复制需要热更的dll
            string hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
            aotAssembliesDstDir = $"{Application.dataPath}/Game/AssetBundles/dllAb/";

            foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
            {
                swCopyDll = new Stopwatch();
                string dllPath = $"{hotfixDllSrcDir}/{dll}";
                swCopyDll.Start();
                var s = dll.Replace(".dll", "");

                Debug.Log($"[HotUpdateAssemblyFilesExcludePreserved] dllPath:{dllPath}");
                string dllBytesPath = $"{aotAssembliesDstDir}{s}.bytes";
                Debug.Log($"[HotUpdateAssemblyFilesExcludePreserved] dllBytesPath:{dllBytesPath}");
                File.Copy(dllPath, dllBytesPath, true);
                swCopyDll.Stop();
                Debug.Log($"复制{dllPath}--->{dllBytesPath}成功 耗时:{swCopyDll.ElapsedMilliseconds}ms");
                Debug.Log($"[CopyHotUpdateAssembliesToDllBytesPath] copy hotfix dll {dllPath} -> {dllBytesPath}");
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            //为自己复制的dll 打ab标签
            foreach (var dll in AOTMetaAssemblyNames)
            {
                sw = new Stopwatch();
                sw.Start();
                var s = dll.Replace(".dll", "");
                string dllBytesPath = $"{aotAssembliesDstDir}{s}.bytes";
                var index = dllBytesPath.IndexOf("Assets", StringComparison.Ordinal);
                var path_last = dllBytesPath.Substring(index);
                var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path_last);
                Debug.Log($"[AOTMetaAssemblyNames]last:{path_last} AB Label dllBytesPath:{dllBytesPath} {obj}");
                string assetPath = AssetDatabase.GetAssetPath(obj);
                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                Debug.Log($"[AOTMetaAssemblyNames] AB Label AssetImporter:{importer}");

                string assetBundleName = Path.GetFileName(dllBytesPath);
                assetBundleName = assetBundleName.Replace(".", "_");
                importer.assetBundleName =
                    $"{assetBundleName}{AssetBundleEditorHelper.ASSETBUNDLE_EXT}";
                Debug.Log(
                    $"{assetBundleName},assetBundleName--->None---->{assetBundleName}{AssetBundleEditorHelper.ASSETBUNDLE_EXT}");
                sw.Stop();
                Debug.Log($"标签{dllBytesPath} 耗时:{swCopyDll.ElapsedMilliseconds}ms ");
                EditorUtility.SetDirty(obj);
            }

            foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
            {
                sw = new Stopwatch();
                sw.Start();
                var s = dll.Replace(".dll", "");
                string dllBytesPath = $"{aotAssembliesDstDir}{s}.bytes";
                var index = dllBytesPath.IndexOf("Assets", StringComparison.Ordinal);
                var path_last = dllBytesPath.Substring(index);
                var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path_last);
                Debug.Log($"[AOTMetaAssemblyNames]last:{path_last} AB Label dllBytesPath:{dllBytesPath} {obj}");
                string assetPath = AssetDatabase.GetAssetPath(obj);
                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                Debug.Log($"[AOTMetaAssemblyNames] AB Label AssetImporter:{importer}");

                string assetBundleName = Path.GetFileName(dllBytesPath);
                assetBundleName = assetBundleName.Replace(".", "_");
                importer.assetBundleName =
                    $"{assetBundleName}{AssetBundleEditorHelper.ASSETBUNDLE_EXT}";
                Debug.Log(
                    $"{assetBundleName},assetBundleName--->None---->{assetBundleName}{AssetBundleEditorHelper.ASSETBUNDLE_EXT}");
                sw.Stop();
                Debug.Log($"标签{dllBytesPath} 耗时:{swCopyDll.ElapsedMilliseconds}ms ");
                EditorUtility.SetDirty(obj);
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        public virtual void Build(bool forceRebuild)
        {
            Debug.Log("----------------BuildAssetBundleWithCommandLineArgs------------------");
            Debug.Log("开始执行 ab出包逻辑");
            string path = this.OutputPath;
            PrebuildCommand.GenerateAll();
            CopyDllAb();
            if (string.IsNullOrEmpty(path))
                BrowseOutputFolder();

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("AssetBundle Build: No valid output path for build.");
                return;
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var options = BuildAssetBundleOptions.DeterministicAssetBundle;
            switch (this.Compression)
            {
                case CompressOptions.Uncompressed:
                    options |= BuildAssetBundleOptions.UncompressedAssetBundle;
                    break;
                case CompressOptions.ChunkBasedCompression:
                    options |= BuildAssetBundleOptions.ChunkBasedCompression;
                    break;
                case CompressOptions.StandardCompression:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (forceRebuild)
                options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;

            options |= this.BuildAssetBundleOptions;

            List<IBundleModifier> bundleModifierChain = this.CreateBundleModifierChain();
            BundleBuilder builder = new BundleBuilder();
            builder.Build(path, this.BuildTarget, options, this.AppDataVersion, bundleModifierChain);

#if UNITY_5_6_OR_NEWER
            if ((options & BuildAssetBundleOptions.DryRunBuild) > 0)
            {
                Debug.LogFormat("Dry Build OK.");
                return;
            }
#endif

            DirectoryInfo dir = new DirectoryInfo(builder.GetPlatformOutput(path, this.BuildTarget));
            try
            {
                //open the folder 
                EditorUtility.OpenWithDefaultApp(dir.FullName);
            }
            catch (Exception)
            {
            }

            Debug.Log($"Build OK.Please check the folder:{dir.FullName}" );

            if (!this.CopyToStreaming)
                return;

            this.CopyToStreamingAssets();
            Debug.Log(" ab出包 完成 结束");
        }

        protected virtual List<IBundleModifier> CreateBundleModifierChain()
        {
            List<IBundleModifier> bundleModifierChain = new List<IBundleModifier>();

            bundleModifierChain.Add(new PublishBundleModifier(bundleInfo => true));

            if (this.Encryption)
            {
                Func<BundleInfo, bool> filter = null;
                switch (this.FilterType)
                {
                    case EncryptionFilterType.All:
                        filter = bundle => true;
                        break;
                    case EncryptionFilterType.RegularExpression:
                        filter = bundle => Regex.IsMatch(bundle.FullName, this.filterExpression);
                        break;
                    case EncryptionFilterType.BundleNameList:
                        var bundles = Regex.Split(this.bundleNames, @"(\s)", RegexOptions.IgnorePatternWhitespace);
                        filter = bundle => Array.IndexOf(bundles, bundle.FullName) >= 0;
                        break;
                }

                var encryptor = this.GetEncryptor();
                bundleModifierChain.Add(new CryptographBundleModifier(encryptor, filter));
            }

            if (this.useHashFilename)
                bundleModifierChain.Add(new HashFilenameBundleModifier());

            return bundleModifierChain;
        }

        public virtual IStreamEncryptor GetEncryptor()
        {
            return CryptographUtil.GetEncryptor(this.Algorithm, Encoding.ASCII.GetBytes(this.KEY),
                Encoding.ASCII.GetBytes(this.IV));
        }

        public virtual IStreamDecryptor GetDecryptor()
        {
            return CryptographUtil.GetDecryptor(this.Algorithm, Encoding.ASCII.GetBytes(this.KEY),
                Encoding.ASCII.GetBytes(this.IV));
        }

        public virtual void ClearFromStreamingAssets()
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                DirectoryInfo dir = new DirectoryInfo(BundleUtil.GetReadOnlyDirectory());
                if (dir.Exists)
                    dir.Delete(true);
                AssetDatabase.Refresh();
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        public virtual void CopyToStreamingAssets()
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                BundleBuilder builder = new BundleBuilder();

                DirectoryInfo src =
                    new DirectoryInfo(builder.GetVersionOutput(this.OutputPath, this.BuildTarget, this.AppDataVersion));
                DirectoryInfo dest = new DirectoryInfo(BundleUtil.GetReadOnlyDirectory());

                if (dest.Exists)
                    dest.Delete(true);
                if (!dest.Exists)
                    dest.Create();

                BundleManifest manifest = builder.CopyAssetBundleAndManifest(src, dest);
                if (manifest != null)
                    Debug.LogFormat("Copy AssetBundles success.");

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogFormat("Copy AssetBundles failure. Error:{0}", e);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        public virtual string GenerateKey()
        {
            if (this.Encryption)
            {
                int keySize = 16;
                switch (this.Algorithm)
                {
                    case Algorithm.AES128_CBC_PKCS7:
                        keySize = 16;
                        break;
                    case Algorithm.AES192_CBC_PKCS7:
                        keySize = 24;
                        break;
                    case Algorithm.AES256_CBC_PKCS7:
                        keySize = 32;
                        break;
#if UNITY_2018_1_OR_NEWER && !NETFX_CORE && !UNITY_WSA && !UNITY_WSA_10_0
                    case Algorithm.AES128_CTR_NONE:
                        keySize = 16;
                        break;
#endif
                }

                return Security.Cryptography.RijndaelCryptograph.GenerateKey(keySize);
            }

            return string.Empty;
        }

        public virtual string GenerateIV()
        {
            if (this.Encryption)
                return Security.Cryptography.RijndaelCryptograph.GenerateIv();
            return string.Empty;
        }

        public virtual void BrowseOutputFolder()
        {
            var path = EditorUtility.OpenFolderPanel("AssetBundle Folder", this.OutputPath, string.Empty);
            if (string.IsNullOrEmpty(path))
                return;

            var projectPath = System.IO.Path.GetFullPath(".");
            projectPath = projectPath.Replace("\\", "/");
            if (path.StartsWith(projectPath))
                path = path.Remove(0, projectPath.Length + 1);
            this.OutputPath = path;
        }

        public virtual void ResetOutputFolder()
        {
            this.OutputPath = DEFAULT_OUTPUT;
        }

        public virtual void OpenOutputFolder()
        {
            if (!Directory.Exists(this.OutputPath))
                Directory.CreateDirectory(this.OutputPath);

            EditorUtility.OpenWithDefaultApp(this.OutputPath);
        }
    }

    public enum CompressOptions
    {
        [Remark("No Compression")] Uncompressed = 0,

        [Remark("Standard Compression (LZMA)")]
        StandardCompression, /*LZMA*/

        [Remark("Chunk Based Compression (LZ4)")]
        ChunkBasedCompression /*LZ4*/
    }

    public enum EncryptionFilterType
    {
        All,
        RegularExpression,
        BundleNameList
    }
}
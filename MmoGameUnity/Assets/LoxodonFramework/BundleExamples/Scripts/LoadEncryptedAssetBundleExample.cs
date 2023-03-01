﻿// #if ASSET_BUNDLE_DEVELOP_EDITOR

using System;
using System.Text;
using UnityEngine;
using ZJYFrameWork.AssetBundles.AssetBundleToolsConfig;
using ZJYFrameWork.AssetBundles.Bundle;
using ZJYFrameWork.AssetBundles.Bundle.LoaderBuilders;
using ZJYFrameWork.AssetBundles.Bundles;
using ZJYFrameWork.AssetBundles.Bundles.ILoaderBuilderInterface;
using ZJYFrameWork.AssetBundles.Bundles.LoaderBuilders;
using ZJYFrameWork.AssetBundles.BundleUtils;
using ZJYFrameWork.Asynchronous;
using ZJYFrameWork.Security.Cryptography;

namespace ZJYFrameWork.Examples.Bundle
{
    /// <summary>
    /// 实例代码
    /// </summary>
    public class LoadEncryptedAssetBundleExample : MonoBehaviour
    {
        private IResources resources;

        private string iv = "5Hh2390dQlVh0AqC";
        private string key = "E4YZgiGQ0aqe5LEJ";

        void Awake()
        {
            /* Create a BundleManifestLoader. */
            IBundleManifestLoader manifestLoader = new BundleManifestLoader();

            /* Loads BundleManifest. */
            BundleManifest manifest =
                manifestLoader.Load(BundleUtil.GetReadOnlyDirectory() + AssetBundleConfig.ManifestFilename);

            /* Create a PathInfoParser. */
            IPathInfoParser pathInfoParser = new AutoMappingPathInfoParser(manifest);

            IStreamDecryptor decryptor = CryptographUtil.GetDecryptor(Algorithm.AES128_CBC_PKCS7,
                Encoding.ASCII.GetBytes(this.key), Encoding.ASCII.GetBytes(this.iv));

            /* Use a BundleLoaderBuilder */
            ILoaderBuilder builder =
                new CustomBundleLoaderBuilder(new Uri(BundleUtil.GetReadOnlyDirectory()), false, decryptor);

            /* Create a BundleManager */
            IBundleManager manager = new BundleManager(manifest, builder);

            /* Create a BundleResources */
            resources = new BundleResources(pathInfoParser, manager);
        }

        void Start()
        {
            this.Load(new string[]
            {
                "LoxodonFramework/BundleExamples/Encrypted/Tanks/Prefabs/Terrain.prefab",
                "LoxodonFramework/BundleExamples/Encrypted/Tanks/Prefabs/Tank.prefab"
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        void Load(string[] names)
        {
            IProgressResult<float, GameObject[]> result = resources.LoadAssetsAsync<GameObject>(names);
            result.Callbackable().OnCallback((r) =>
            {
                try
                {
                    if (r.Exception != null)
                        throw r.Exception;

                    foreach (GameObject template in r.Result)
                    {
                        GameObject.Instantiate(template);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("Load failure.Error:{0}", e);
                }
            });
        }
    }
}
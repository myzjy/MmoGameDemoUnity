﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using ZJYFrameWork.AssetBundles.Bundles.ILoaderBuilderInterface;
using ZJYFrameWork.Asynchronous;
using ZJYFrameWork.Spring.Core;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.AssetBundles.Bundles
{
    public sealed class SimulationBundleManager : IBundleManager
    {
        private Dictionary<string, SimulationBundle> bundles;
        private List<string> bundleNames;

        public SimulationBundleManager()
        {
            this.bundles = new Dictionary<string, SimulationBundle>();
            this.bundleNames = AssetDatabaseHelper.GetUsedAssetBundleNames();
        }

        public void AddBundle(SimulationBundle bundle)
        {
            if (this.bundles == null)
                return;

            this.bundles.Add(bundle.Name, bundle);
        }

        public void RemoveBundle(SimulationBundle bundle)
        {
            if (this.bundles == null)
                return;

            this.bundles.Remove(bundle.Name);
        }

        private SimulationBundle GetOrCreateBundle(string bundleName)
        {
            var bundleNameWhitoutExtension = Path.GetFilePathWithoutExtension(bundleName).ToLower();
            var extension = Path.GetExtension(bundleName).ToLower();
            var bundleNameWhitExtension = string.IsNullOrEmpty(extension)
                ? bundleNameWhitoutExtension
                : $"{bundleNameWhitoutExtension}.{extension}";

            if (bundleNames.IndexOf(bundleNameWhitExtension) < 0)
                throw new Exception($"Not found the AssetBundle '{bundleNameWhitExtension}'.");

            if (this.bundles.TryGetValue(bundleNameWhitoutExtension, out var bundle))
                return bundle;

            return new SimulationBundle(bundleNameWhitoutExtension, extension, this);
        }

        public IBundle GetBundle(string bundleName)
        {
            if (this.bundles == null)
                return null;

            SimulationBundle bundle;
            if (this.bundles.TryGetValue(bundleName, out bundle))
                return new SimulationInternalBundleWrapper(bundle);
            return null;
        }

        public IProgressResult<float, IBundle> LoadBundle(string bundleName)
        {
            return this.LoadBundle(bundleName, 0);
        }

        public IProgressResult<float, IBundle> LoadBundle(string bundleName, int priority)
        {
            try
            {
                if (string.IsNullOrEmpty(bundleName))
                    throw new ArgumentNullException("bundleName", "The bundleName is null or empty!");

                SimulationBundle bundle = this.GetOrCreateBundle(bundleName);
                return new ImmutableProgressResult<float, IBundle>(new SimulationInternalBundleWrapper(bundle), 1f);
            }
            catch (Exception e)
            {
                return new ImmutableProgressResult<float, IBundle>(e, 0f);
            }
        }

        public IProgressResult<float, IBundle[]> LoadBundle(params string[] bundleNames)
        {
            return this.LoadBundle(bundleNames, 0);
        }

        public IProgressResult<float, IBundle[]> LoadBundle(string[] bundleNames, int priority)
        {
            try
            {
                if (bundleNames == null || bundleNames.Length <= 0)
                    throw new ArgumentNullException(nameof(bundleNames), "The bundleNames is null or empty!");

                List<IBundle> list = new List<IBundle>(0);
                foreach (string bundleName in bundleNames)
                {
                    try
                    {
                        list.Add(new SimulationInternalBundleWrapper(this.GetOrCreateBundle(bundleName)));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("加载Bundle '{}'失败!错误:{}", bundleName, e);
                    }
                }

                return new ImmutableProgressResult<float, IBundle[]>(list.ToArray(), 1f);
            }
            catch (Exception e)
            {
                return new ImmutableProgressResult<float, IBundle[]>(e, 0f);
            }
        }

        public void SetManifestAndLoadBuilder(BundleManifest manifest, ILoaderBuilder builder)
        {
        }
    }
}
#endif
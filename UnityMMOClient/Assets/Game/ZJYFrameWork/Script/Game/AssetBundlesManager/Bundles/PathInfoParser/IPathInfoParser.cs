﻿namespace ZJYFrameWork.AssetBundles.Bundles
{
    public interface IPathInfoParser
    {
        /// <summary>
        /// Parses the path information of the asset.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        AssetPathInfo Parse(string path);

        BundleManifest BundleManifest { get; set; }
        void Initialize();
    }
}
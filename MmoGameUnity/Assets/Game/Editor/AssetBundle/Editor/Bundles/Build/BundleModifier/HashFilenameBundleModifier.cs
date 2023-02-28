﻿using System;
using System.Text;
using System.Security.Cryptography;
using ZJYFrameWork.AssetBundles.Bundles;

namespace ZJYFrameWork.AssetBundles.EditorAssetBundle.Editors
{
    public class HashFilenameBundleModifier : IBundleModifier
    {
        [ThreadStatic]
        private static MD5 md5;

        private static MD5 MD5
        {
            get
            {
                if (md5 == null)
                    md5 = new MD5CryptoServiceProvider();
                return md5;
            }
        }

        public virtual void Modify(BundleData bundleData)
        {
            BundleInfo bundleInfo = bundleData.BundleInfo;
            string name =
                $"{bundleInfo.FullName}-{bundleInfo.Hash.ToString()}-{bundleInfo.CRC}-{(bundleInfo.IsEncrypted ? bundleInfo.Encoding : "")}";
            bundleInfo.Filename = BitConverter.ToString(MD5.ComputeHash(Encoding.ASCII.GetBytes(name))).Replace("-", "").ToLower();
        }
    }
}

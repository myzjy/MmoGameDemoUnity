using System.Collections.Generic;
using Common.Utility;
using Framework.AssetBundles.Utilty;
using Script.Framework.AssetBundle;
using UnityEngine;

namespace AssetBundles.Config
{
    public class Manifest
    {
        /// <summary>
        /// 基础资源名字
        /// </summary>
        private const string assetName = "AssetBundleManifest";

        /// <summary>
        /// 资源财产包
        /// </summary>
        private AssetBundleManifest manifest = null;

        public AssetBundleManifest assetbundleManifest
        {
            get { return manifest; }
        }

        /// <summary>
        /// 下载资源财产包字节包
        /// </summary>
        private byte[] manifestBytes = null;

        /// <summary>
        /// 数组？
        /// </summary>
        private string[] emptyStringArray;

        /// <summary>
        /// 资源包名
        /// </summary>
        public string AssetbundleName { get; protected set; }

        /// <summary>
        /// 资源财产包数量
        /// </summary>
        public int Length
        {
            get { return manifest == null ? 0 : manifest.GetAllAssetBundles().Length; }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public Manifest()
        {
            //资源包名
            AssetbundleName = AssetBundleManager.ManifestBundleName;
            //判断是否设置了资源包财产名
            if (string.IsNullOrEmpty(AssetbundleName))
            {
                ToolsDebug.LogError("请设置ManifestBundleName！！！");
            }
        }

        #region 读取资源

        /// <summary>
        /// 读取的AssetBundle
        /// </summary>
        /// <param name="assetBundle"></param>
        public void LoadFromAssetBundle(AssetBundle assetBundle)
        {
            if (assetBundle == null)
            {
                ToolsDebug.LogError("manifest LoadFromAssetBundle assetBundle is null");
                return;
            }

            //读取到 manifest 文件
            manifest = assetBundle.LoadAsset<AssetBundleManifest>(assetName);
        }

        #endregion

        /// <summary>
        /// 设置字节数组
        /// </summary>
        /// <param name="bytes"></param>
        public void SaveBytes(byte[] bytes)
        {
            manifestBytes = bytes;
        }

        /// <summary>
        /// 保存到磁盘缓存
        /// </summary>
        public void SaveToDiskCahce()
        {
            if (manifestBytes == null || manifestBytes.Length <= 0) return;
            //根据资源文件名 获取持久数据路径
            string path = AssetBundleUtility.GetPersistentDataPath(AssetbundleName);
            //根据路径和manifest字节数组 将字节数组写入对应路径
            GameUtility.SafeWriteAllBytes(path, manifestBytes);
        }

        /// <summary>
        /// 获取AssetBundle的hash值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Hash128 GetAssetBundleHash(string name)
        {
            //获取给定的AssetBundle的hash值
            return manifest == null ? default(Hash128) : manifest.GetAssetBundleHash(name);
        }

        /// <summary>
        /// 获取清单中的所有资产包。
        /// </summary>
        /// <returns></returns>
        public string[] GetAllAssetBundleNames()
        {
            return manifest == null ? emptyStringArray : manifest.GetAllAssetBundles();
        }
        /// <summary>
        /// 在清单中获得所有带有变体的资产包  
        /// </summary>
        /// <returns></returns>
        public string[] GetAllAssetBundlesWithVariant()
        {
            return manifest == null ? emptyStringArray : manifest.GetAllAssetBundlesWithVariant();
        }

        /// <summary>
        /// 为给定的AssetBundle设置所有依赖的AssetBundles。  
        /// </summary>
        /// <param name="assetbundleName">资产包的名称。</param>
        /// <returns></returns>
        public string[] GetAllDependencies(string assetbundleName)
        {
            return manifest == null ? emptyStringArray : manifest.GetAllDependencies(assetbundleName);
        }

        public string[] GetDirectDependencies(string assetbundleName)
        {
            return manifest == null ? emptyStringArray : manifest.GetDirectDependencies(assetbundleName);
        }

        /// <summary>
        /// 进行对比
        /// </summary>
        /// <param name="otherManifest"></param>
        /// <returns></returns>
        public List<string> CompareTo(Manifest otherManifest)
        {
            var ret_list = new List<string>();
            if (otherManifest.assetbundleManifest == null)
            {
                return ret_list;
            }

            //对比manifest (列表) 不为空
            if (otherManifest == null)
            {
                ret_list.AddRange(otherManifest.GetAllAssetBundleNames());
                return ret_list;
            }
            //获取其他清单的资产列表
            string[] other_name_list = otherManifest.GetAllAssetBundleNames();
            //活动当前
            string[] self_name_list = GetAllAssetBundleNames();
            //循环判断
            foreach (string name in other_name_list)
            {
                int idx = System.Array.FindIndex(self_name_list, element => element.Equals(name));
                if (idx == -1)
                {
                    //对方有、自己无
                    ret_list.Add(name);
                }
                else if (!GetAssetBundleHash(self_name_list[idx]).Equals(otherManifest.GetAssetBundleHash(name)))
                {
                    //对方有，自己有，但是hash不同
                    ret_list.Add(name);
                }
                else
                {
                    //对方有，自己有，且hash相同：什么也不做
                }
            }

            return ret_list;
        }
    }
}
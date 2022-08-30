using UnityEngine;

namespace ZJYFrameWork.AssetBundleLoader
{
    public interface IBundle
    {
        /// <summary>
        /// 获取捆绑包的名称。
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 同步加载资产。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T LoadAsset<T>(string name) where T : Object;
        /// <summary>
        /// 同步加载资产。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Object LoadAsset(string name, System.Type type);
    }
}
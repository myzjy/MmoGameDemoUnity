using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ZJYFrameWork
{
    public class HttpInstructionsImporter
    {
        private const string FullPathEndsWith = ".json.txt";
        /// <summary>
        /// 刷新文件
        /// </summary>
        /// <param name="fullPath">路径</param>
        /// <param name="assetRefresh">是否刷新资源 默认必须刷</param>
        public static void ProcessFile(string fullPath, bool assetRefresh = true)
        {
            if (!fullPath.EndsWith(FullPathEndsWith))
            {
                //传递路径不对
                Debug.Log($"路径错误：{fullPath},解析出错了！！");
                return;
            }

            //将文本完全读到
            var text = File.ReadAllText(fullPath);
            var pack = JsonConvert.DeserializeObject<Instructions>(text);
            
        }
    }
}
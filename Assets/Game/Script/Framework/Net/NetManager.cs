using UnityEngine;
using ZJYFrameWork.Base.Model;
using ZJYFrameWork.Spring.Core;

// ReSharper disable once CheckNamespace
namespace ZJYFrameWork.Net
{
    [Bean]
    public class NetManager : AbstractManager
    {
        /// <summary>
        /// 轮询
        /// </summary>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSeconds"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            throw new System.NotImplementedException();
        }

        public override void Shutdown()
        {
            Close();
        }

        /// <summary>
        /// 链接
        /// </summary>
        /// <param name="url"></param>
        public void Connect(string url)
        {
            Debug.Info("开始链接服务器[url:{}][Platform:{}]", url, Application.platform);
        }

        /// <summary>
        /// 关闭socket
        /// </summary>
        public void Close()
        {
        }
    }
}
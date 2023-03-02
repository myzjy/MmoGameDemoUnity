// using System;
// using System.IO;
// using ZJYFrameWork.AssetBundles.Bundle;
// using ZJYFrameWork.AssetBundles.IDownLoaderBundle;
//
// namespace ZJYFrameWork.AssetBundles.DownLoader
// {
//     public sealed class DownloadAgent : ITaskAgent<DownloadTask>, IDisposable
//     {
//         private readonly IDownloader m_Helper;
//         public Action<DownloadAgent, string> DownloadAgentFailure;
//
//         public Action DownloadAgentStart;
//         public Action<DownloadAgent, long> DownloadAgentSuccess;
//         public Action<DownloadAgent, int> DownloadAgentUpdate;
//         private bool m_Disposed;
//         private long m_DownloadedLength;
//         private FileStream m_FileStream;
//         private long m_SavedLength;
//         private long m_StartLength;
//         private DownloadTask m_Task;
// #pragma warning disable CS0414
//         private int m_WaitFlushSize;
// #pragma warning restore CS0414
//         private float m_WaitTime;
//
//         /// <summary>
//         /// 初始化下载代理的新实例。
//         /// </summary>
//         /// <param name="downloadAgentHelper">下载代理辅助器。</param>
//         public DownloadAgent(IDownloader downloadAgentHelper)
//         {
//             if (downloadAgentHelper == null)
//             {
//                 throw new Exception("Download agent helper is invalid.");
//             }
//
//             m_Helper = downloadAgentHelper;
//             m_Task = null;
//             m_FileStream = null;
//             m_WaitFlushSize = 0;
//             m_WaitTime = 0f;
//             m_StartLength = 0L;
//             m_DownloadedLength = 0L;
//             m_SavedLength = 0L;
//             m_Disposed = false;
//
//             DownloadAgentStart = null;
//             DownloadAgentUpdate = null;
//             DownloadAgentSuccess = null;
//             DownloadAgentFailure = null;
//         }
//
//         /// <summary>
//         /// 获取已经等待时间。
//         /// </summary>
//         public float WaitTime
//         {
//             get { return m_WaitTime; }
//         }
//
//         /// <summary>
//         /// 获取开始下载时已经存在的大小。
//         /// </summary>
//         public long StartLength
//         {
//             get { return m_StartLength; }
//         }
//
//         /// <summary>
//         /// 获取本次已经下载的大小。
//         /// </summary>
//         public long DownloadedLength
//         {
//             get { return m_DownloadedLength; }
//         }
//
//         /// <summary>
//         /// 获取当前的大小。
//         /// </summary>
//         public long CurrentLength
//         {
//             get { return m_StartLength + m_DownloadedLength; }
//         }
//
//         /// <summary>
//         /// 获取已经存盘的大小。
//         /// </summary>
//         public long SavedLength
//         {
//             get { return m_SavedLength; }
//         }
//
//         public void Dispose()
//         {
//             Dispose(true);
//             GC.SuppressFinalize(this);
//         }
//
//         /// <summary>
//         /// 获取下载任务。
//         /// </summary>
//         public DownloadTask Task
//         {
//             get { return m_Task; }
//         }
//
//         public void Initialize()
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Update(float elapseSeconds, float realElapseSeconds)
//         {
//             if (m_Task.Status == DownloadTaskStatus.Doing)
//             {
//                 m_WaitTime += realElapseSeconds;
//                 if (m_WaitTime >= m_Task.Timeout)
//                 {
//                     Debug.Log($"下载{m_Task.DownloadPath}，{m_Task.AssetBundleInfo.Filename}文件下载超时");
//                 }
//             }
//         }
//
//         public void Shutdown()
//         {
//             Dispose();
//         }
//
//         public DownloadTaskStatus Start(DownloadTask task)
//         {
//             if (task == null)
//             {
//                 throw new Exception("Task is invalid.");
//             }
//
//             m_Task = task;
//
//             return m_Task.Status;
//         }
//
//         public void Reset()
//         {
//             if (m_Task != null)
//                 m_Task.Clear();
//             m_Task = null;
//             m_WaitFlushSize = 0;
//             m_WaitTime = 0f;
//             m_StartLength = 0L;
//             m_DownloadedLength = 0L;
//             m_SavedLength = 0L;
//         }
//
//         /// <summary>
//         /// 释放资源。
//         /// </summary>
//         /// <param name="disposing">释放资源标记。</param>
//         private void Dispose(bool disposing)
//         {
//             if (m_Disposed)
//             {
//                 return;
//             }
//
//             if (disposing)
//             {
//                 if (m_FileStream != null)
//                 {
//                     m_FileStream.Dispose();
//                     m_FileStream = null;
//                 }
//             }
//
//             m_Disposed = true;
//         }
//     }
// }
// using ZJYFrameWork.AssetBundles.Bundles;
// using ZJYFrameWork.AssetBundles.IDownLoaderBundle;
// using ZJYFrameWork.Collection.Reference;
//
// namespace ZJYFrameWork.AssetBundles.DownLoader
// {
//     /// <summary>
//     /// AssetBundle 任务基类
//     /// </summary>
//     public abstract class TaskBase : IReference
//     {
//         /// <summary>
//         /// 任务默认优先级。
//         /// </summary>
//         public const int DefaultPriority = 0;
//
//         private BundleInfo _bundleInfo;
//         private int m_Priority;
//         private bool m_Done;
//
//         /// <summary>
//         /// 获取AssetBundle的Bundle信息。
//         /// </summary>
//         public BundleInfo SerialBundleInfo
//         {
//             get { return _bundleInfo; }
//         }
//
//         /// <summary>
//         /// 任务是否开始了，或者正在进行中
//         /// </summary>
//         public bool IsTaskDone
//         {
//             get;
//             set;
//         }
//
//         /// <summary>
//         /// 当前任务是否已经结束了
//         /// </summary>
//         public bool IsEndTaskDone
//         {
//             get;
//             set;
//         }
//         /// <summary>
//         /// 获取任务的优先级。
//         /// </summary>
//         public int Priority
//         {
//             get { return m_Priority; }
//         }
//
//         /// <summary>
//         /// 获取或设置任务是否完成。
//         /// </summary>
//         public bool Done
//         {
//             get { return m_Done; }
//             set { m_Done = value; }
//         }
//         public virtual IDownloader Downloader { get; set; }
//         /// <summary>
//         /// 获取任务描述。
//         /// </summary>
//         public virtual string Description
//         {
//             get { return null; }
//         }
//
//         /// <summary>
//         /// 初始化任务基类的新实例。
//         /// </summary>
//         public TaskBase()
//         {
//             _bundleInfo = default;
//             m_Priority = DefaultPriority;
//             m_Done = false;
//         }
//
//         public void Initialize(BundleInfo serialBundleInfo, int priority)
//         {
//             _bundleInfo = serialBundleInfo;
//             m_Priority = priority;
//             m_Done = false;
//         }
//
//         public void StartUpdate()
//         {
//             
//         }
//
//         public virtual void Clear()
//         {
//             _bundleInfo = default;
//             m_Priority = DefaultPriority;
//             m_Done = false;
//         }
//     }
// }
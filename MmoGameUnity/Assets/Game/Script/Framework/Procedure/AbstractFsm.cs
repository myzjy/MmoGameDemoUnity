using System;

namespace ZJYFrameWork.Procedure
{
    /// <summary>
    /// 状态机 基类
    /// </summary>
    public abstract class AbstractFsm
    {
        /// <summary>
        /// 状态机的名字
        /// </summary>
        public string Name => OwnerType.FullName;

        /// <summary>
        /// 这个状态机持有者的状态
        /// </summary>
        public abstract Type OwnerType { get; }

        /// <summary>
        /// 这个状态机是否已经被销毁了
        /// </summary>
        public abstract bool IsDestroy { get; }

        /// <summary>
        /// 有限状态机轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">当前已流逝时间，以秒为单位。</param>
        public abstract void Update(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 关闭并清理有限状态机。
        /// </summary>
        public abstract void Shutdown();
    }
}
using System;
using System.Collections.Generic;

namespace FrostEngine
{
    /// <summary>
    /// 游戏框架模块实现类管理系统。
    /// </summary>
    public static class ModuleImpSystem
    {
        /// <summary>
        /// 默认设计的模块数量。
        /// <remarks>有增删可以自行修改减少内存分配与GCAlloc。</remarks>
        /// </summary>
        internal const int DesignModuleCount = 16;
        private const string ModuleRootNameSpace = "FrostEngine.";
        
        private static readonly Dictionary<Type, ModuleImp> _moduleMaps = new(DesignModuleCount);
        private static readonly GameFrameworkLinkedList<ModuleImp> _modules = new();
        private static readonly GameFrameworkLinkedList<ModuleImp> _updateModules = new();
        private static readonly List<ModuleImp> _updateExecuteList = new(DesignModuleCount);
        private static bool _isExecuteListDirty;
        
        /// <summary>
        /// 所有游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (_isExecuteListDirty)
            {
                _isExecuteListDirty = false;
                BuildExecuteList();
            }
            
            int executeCount = _updateExecuteList.Count;
            for (int i = 0; i < executeCount; i++)
            {
                _updateExecuteList[i].Update(elapseSeconds, realElapseSeconds);
            }
        }
        
        /// <summary>
        /// 关闭并清理所有游戏框架模块。
        /// </summary>
        public static void Shutdown()
        {
            for (LinkedListNode<ModuleImp> current = _modules.Last; current != null; current = current.Previous)
            {
                current.Value.Shutdown();
            }

            _modules.Clear();
            _moduleMaps.Clear();
            _updateModules.Clear();
            _updateExecuteList.Clear();
            MemoryPool.ClearAll();
            Marshal.FreeCachedHGlobal();
        }
        
        /// <summary>
        /// 构造执行队列。
        /// </summary>
        private static void BuildExecuteList()
        {
            _updateExecuteList.Clear();
            _updateExecuteList.AddRange(_updateModules);
        }
    }
}
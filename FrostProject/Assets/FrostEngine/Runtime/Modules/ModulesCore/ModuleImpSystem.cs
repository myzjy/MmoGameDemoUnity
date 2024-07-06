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
        /// 获取游戏框架模块。
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架模块类型。</typeparam>
        /// <returns>要获取的游戏框架模块。</returns>
        /// <remarks>如果要获取的游戏框架模块不存在，则自动创建该游戏框架模块。</remarks>
        public static T GetModule<T>() where T : class
        {
            Type module = typeof(T);

            if (module.FullName != null && !module.FullName.StartsWith(ModuleRootNameSpace, StringComparison.Ordinal))
            {
                throw new GameFrameworkException(StringUtils.Format("You must get a Framework module, but '{}' is not.",
                    module.FullName));
            }

            string moduleName = StringUtils.Format("{}.{}", module.Namespace, module.Name.Substring(1));
            Type moduleType = Type.GetType(moduleName);
            if (moduleType == null)
            {
                moduleName = StringUtils.Format("{}.{}", module.Namespace, module.Name);
                moduleType = Type.GetType(moduleName);
                if (moduleType == null)
                {
                    throw new GameFrameworkException(StringUtils.Format("Can not find Game Framework module type '{}'.",
                        moduleName));
                }
            }

            return GetModule(moduleType) as T;
        }

        /// <summary>
        /// 获取游戏框架模块。
        /// </summary>
        /// <param name="moduleType">要获取的游戏框架模块类型。</param>
        /// <returns>要获取的游戏框架模块。</returns>
        /// <remarks>如果要获取的游戏框架模块不存在，则自动创建该游戏框架模块。</remarks>
        private static ModuleImp GetModule(Type moduleType)
        {
            return _moduleMaps.TryGetValue(moduleType, out ModuleImp module) ? module : CreateModule(moduleType);
        }

        /// <summary>
        /// 创建游戏框架模块。
        /// </summary>
        /// <param name="moduleType">要创建的游戏框架模块类型。</param>
        /// <returns>要创建的游戏框架模块。</returns>
        private static ModuleImp CreateModule(Type moduleType)
        {
            ModuleImp moduleImp = (ModuleImp)Activator.CreateInstance(moduleType);
            if (moduleImp == null)
            {
                throw new GameFrameworkException(StringUtils.Format("Can not create module '{}'.", moduleType.FullName));
            }

            _moduleMaps[moduleType] = moduleImp;

            LinkedListNode<ModuleImp> current = _modules.First;
            while (current != null)
            {
                if (moduleImp.Priority > current.Value.Priority)
                {
                    break;
                }

                current = current.Next;
            }

            if (current != null)
            {
                _modules.AddBefore(current, moduleImp);
            }
            else
            {
                _modules.AddLast(moduleImp);
            }

            if (Attribute.GetCustomAttribute(moduleType, typeof(UpdateModuleAttribute)) is UpdateModuleAttribute updateModuleAttribute)
            {
                LinkedListNode<ModuleImp> currentUpdate = _updateModules.First;
                while (currentUpdate != null)
                {
                    if (moduleImp.Priority > currentUpdate.Value.Priority)
                    {
                        break;
                    }

                    currentUpdate = currentUpdate.Next;
                }

                if (currentUpdate != null)
                {
                    _updateModules.AddBefore(currentUpdate, moduleImp);
                }
                else
                {
                    _updateModules.AddLast(moduleImp);
                }
                
                _isExecuteListDirty = true;
            }

            return moduleImp;
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
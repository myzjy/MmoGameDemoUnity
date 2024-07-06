using System;
using System.Collections.Generic;

namespace FrostEngine
{
    public static class ModuleSystem
    {
        /**
         * Module 链表
         */
        private static readonly GameFrameworkLinkedList<Module> _modules = new();

        /// <summary>
        /// 游戏框架所在的场景编号。
        /// </summary>
        internal const int GameFrameworkSceneId = 0;

        /// <summary>
        /// 获取游戏框架模块。
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架模块类型。</typeparam>
        /// <returns>要获取的游戏框架模块。</returns>
        public static T GetModule<T>() where T : Module
        {
            return (T)GetModule(typeof(T));
        }

        /// <summary>
        /// 获取游戏框架模块。
        /// </summary>
        /// <param name="type">要获取的游戏框架模块类型。</param>
        /// <returns>要获取的游戏框架模块。</returns>
        public static Module GetModule(Type type)
        {
            LinkedListNode<Module> current = _modules.First;
            while (current != null)
            {
                if (current.Value.GetType() == type)
                {
                    return current.Value;
                }

                current = current.Next;
            }

            return null;
        }

        /// <summary>
        /// 获取游戏框架模块。
        /// </summary>
        /// <param name="typeName">要获取的游戏框架模块类型名称。</param>
        /// <returns>要获取的游戏框架模块。</returns>
        public static Module GetModule(string typeName)
        {
            LinkedListNode<Module> current = _modules.First;
            while (current != null)
            {
                Type type = current.Value.GetType();
                if (type.FullName == typeName || type.Name == typeName)
                {
                    return current.Value;
                }

                current = current.Next;
            }

            return null;
        }

        /// <summary>
        /// 关闭游戏框架。
        /// </summary>
        /// <param name="shutdownType">关闭游戏框架类型。</param>
        public static void Shutdown(ShutdownType shutdownType)
        {
            Debug.Info("Shutdown Game Framework ({})...", shutdownType);
            RootModules rootModule = GetModule<RootModules>();
            if (rootModule != null)
            {
                rootModule.Shutdown();
                rootModule = null;
            }

            _modules.Clear();
            
        }
    }
}
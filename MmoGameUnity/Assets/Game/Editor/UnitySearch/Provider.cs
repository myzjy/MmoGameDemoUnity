using System.Collections.Generic;

namespace UnitySearch
{
    public abstract class Provider
    {
        /// <summary>
        /// 获取Tag
        /// </summary>
        /// <returns></returns>
        internal abstract string GetTag();
        internal abstract IEnumerable<UnitySearchTreeViewItem> SearchItems(string searchText);
        /// <summary>
        /// 根据字符 在方法内部执行
        /// </summary>
        /// <param name="id"></param>
        internal abstract void Action(string id);
        internal abstract UnitySearchTreeViewItem GetItem(string key);
        /// <summary>
        /// 一次搜索获得的最大数量
        /// </summary>
        /// <returns></returns>
        public virtual int SearchMaxCount()
        {
            return 20;
        }
        /// <summary>
        /// 只有在有标签的时候检索才有效
        /// </summary>
        /// <returns></returns>
        public virtual bool TagSearchOnly()
        {
            return false;
        }
        public virtual void DragAndDrop(string param)
        {
        }

        public virtual bool AllowEmptySearch()
        {
            return false;
        }

        /// <summary>
        /// 表示優先度
        /// </summary>
        public virtual int GetPriority()
        {
            return 100;
        }
    }
}
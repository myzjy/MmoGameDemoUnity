using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FrostEngine
{
    [DisallowMultipleComponent]
    public class ViewSignSerializableUI:MonoBehaviour
    {
        [SerializeField] private List<UIKeyObjectData> kodComs = new List<UIKeyObjectData>();
        public List<UIKeyObjectData> KodComs => kodComs;
#if UNITY_EDITOR

        private Dictionary<string, Object> ObjectsDict = new Dictionary<string, Object>();

        [SerializeField] private int maskIndex;

        public int MaskIndex
        {
            get => maskIndex;
            set => maskIndex = value;
        }

        /// <summary>
        /// 刷新根节点
        /// </summary>
        public void ViewRootFlush()
        {
            Transform parent = this.transform.parent;
            UISerializableKeyObject m_UISko = null;
            //内部循环判断这个物体根节点是否有 UISerializableKeyObject
            while (parent != null)
            {
                //获取
                m_UISko = parent.GetComponent<UISerializableKeyObject>();
                if (m_UISko != null)
                {
                    break;
                }

                parent = parent.parent;
            }

            //获取到组件不为空
            if (m_UISko != null)
            {
                //刷新根节点
                // m_UISko.FlushData();
                UnityEngine.Debug.Log("刷新节点");
            }
            else
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogError("UISerializableKeyObject 找不到根节点");
#endif
            }
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="thisObj"></param>
        /// <returns></returns>
        public bool Contains(Object thisObj)
        {
            //循环判断是否存在
            return kodComs.Any(item =>
                item.UI_Serializable_Obj == thisObj && item.UI_Serializable_Obj.name == thisObj.name);
        }

        public bool IsContains(Object thisObj) => kodComs.Any(a =>
            a.UI_Serializable_Obj.name == thisObj.name);

        public void Delete(Object DeleteObj)
        {
            var count = kodComs.Count;
            for (var i = 0; i < count; i++)
            {
                var itemData = kodComs[i];
                //我要删除的是否存在
                if (itemData.UI_Serializable_Obj == DeleteObj)
                {
                    //删除之后就退出循环
                    kodComs.Remove(itemData);
                    break;
                }
            }
        }

        public static string GetObjPath(Component component)
        {
            StringBuilder sb = new StringBuilder(14);
            sb.Append(GetObjPath(component.gameObject));
            return sb.ToString();
        }

        public static string GetPath(Component component)
        {
            StringBuilder sb = new StringBuilder(14);
            sb.Append(component.gameObject.name);
            // sb.Append("_");
            // sb.Append(component.GetType().Name);

            return sb.ToString();
        }

        /// <summary>
        /// 路径获取
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetPath(GameObject obj)
        {
            StringBuilder sb = new StringBuilder(15);
            var UISko = obj.GetComponent<UISerializableKeyObject>();
            //父物体
            var parent = obj.transform.parent;
            Stack<string> paths = new Stack<string>();
            paths.Push(obj.name);
            while (UISko == null && parent != null)
            {
                UISko = parent.GetComponent<UISerializableKeyObject>();
                if (UISko == null)
                {
                    //获取到路径，循环父物体
                    paths.Push($"{parent.name}_");
                    parent = parent.parent;
                }
                else
                {
                    break;
                }
            }

            foreach (var str in paths)
            {
                sb.Append(str);
            }
#if UNITY_EDITOR
            UnityEngine.Debug.Log(sb.ToString());
#endif
            return sb.ToString();
        }

        public static string GetObjPath(GameObject obj)
        {
            StringBuilder sb = new StringBuilder(15);
            var UISko = obj.GetComponent<UISerializableKeyObject>();
            //父物体
            var parent = obj.transform.parent;
            Stack<string> paths = new Stack<string>();
            paths.Push(obj.name);
            while (UISko == null && parent != null)
            {
                UISko = parent.GetComponent<UISerializableKeyObject>();
                //获取到路径，循环父物体
                paths.Push($"{parent.name}/");
                parent = parent.parent;
            }

            foreach (var str in paths)
            {
                sb.Append(str);
            }
#if UNITY_EDITOR
            UnityEngine.Debug.Log(sb.ToString());
#endif
            return sb.ToString();
        }

#endif
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZJYFrameWork.UISerializable
{
    [Serializable]
    public class UIKeyObjectData
    {
        /// <summary>
        /// 自定义组件名
        /// </summary>
        public string UI_Serializable_Key;

        /// <summary>
        /// 保存组件 Obj
        /// </summary>
        public Object UI_Serializable_Obj;

        /// <summary>
        /// UI组件在预制体中位置
        /// </summary>
        public string Path;
    }


    public class UISerializableKeyObject : MonoBehaviour, ISerializationCallbackReceiver
    {
        //ui组件保存
        public List<UIKeyObjectData> dataList => dataViewList.SelectMany(a => a.KodComs).ToList();

        [SerializeField] private List<ViewSignSerializableUI> dataViewList = new List<ViewSignSerializableUI>();

        //UI组件更具我们定义的名字去保存Key
        private Dictionary<string, Object> ObjectsDict =>
            dataList.ToDictionary(a => a.UI_Serializable_Key, a => a.UI_Serializable_Obj);

        /// <summary>
        /// 反序列化后触发
        /// </summary>
        public void OnAfterDeserialize()
        {
        }

        /// <summary>
        /// 反序列化前触发
        /// </summary>
        public void OnBeforeSerialize()
        {
        }

        /// <summary>
        /// 根据组件名 获取
        /// </summary>
        /// <param name="objKey"></param>
        /// <returns></returns>
        public T GetObjType<T>(string objKey) where T : class
        {
            ObjectsDict.TryGetValue(key: objKey, out var data);
            return data != null ? data as T : null;
        }

        public Object GetObjType(string objKey)
        {
            ObjectsDict.TryGetValue(key: objKey, out var data);
            return data != null ? data : null;
        }
#if UNITY_EDITOR
        public void FlushData()
        {
            List<ViewSignSerializableUI> list = new List<ViewSignSerializableUI>();
            var ssu = GetComponent<ViewSignSerializableUI>();
            if (ssu != null && ssu.KodComs.Count > 0)
            {
                list.Add(ssu);
            }

            Transform tf = transform;

            FindAllChild(ref list, tf);

            dataViewList.Clear();
            foreach (ViewSignSerializableUI viewSignSerializableUI in list)
            {
                dataViewList.Add(viewSignSerializableUI);
            }
        }


        void FindAllChild(ref List<ViewSignSerializableUI> list, Transform selfTf)
        {
            Transform tf = selfTf;
            int childCount = tf.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform childTf = tf.GetChild(i);
                if (childTf.GetComponent<UISerializableKeyObject>() == null)
                {
                    var child = childTf.GetComponent<ViewSignSerializableUI>();
                    if (child != null && child.KodComs.Count > 0)
                    {
                        list.Add(child);
                    }

                    FindAllChild(ref list, childTf);
                }

                if (childTf.GetComponent<UISerializableKeyObject>() != null)
                {
                    var child = childTf.GetComponent<ViewSignSerializableUI>();
                    if (child != null && child.KodComs.Count > 0)
                    {
                        list.Add(child);
                    }
                }
            }
        }
#endif
    }
}
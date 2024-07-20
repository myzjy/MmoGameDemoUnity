using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace FrostEngine
{
    public class UISerializableKeyObject : MonoBehaviour, ISerializationCallbackReceiver
    {
        //ui组件保存
        public List<UIKeyObjectData> dataList => dataViewList.SelectMany(a => a.KodComs).ToList();

        [SerializeField] public List<ViewSignSerializableUI> dataViewList = new List<ViewSignSerializableUI>();

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

        public Object GetObjTypeStr(string objKey)
        {
            ObjectsDict.TryGetValue(key: objKey, out var data);
            return data != null ? data : null;
        }
#if UNITY_EDITOR
        [ButtonGroup("list\an")]
        public void OnFindAllChild()
        {
            FlushData(this);
        }
        public void FlushData(UISerializableKeyObject data)
        {
            List<ViewSignSerializableUI> list = new List<ViewSignSerializableUI>();
            var ssu = data.GetComponent<ViewSignSerializableUI>();
            if (ssu != null && ssu.KodComs.Count > 0)
            {
                list.Add(ssu);
            }
        
            Transform tf = data.transform;
        
            FindAllChild(ref list, tf);
        
            data.dataViewList.Clear();
            foreach (ViewSignSerializableUI viewSignSerializableUI in list)
            {
                data.dataViewList.Add(viewSignSerializableUI);
            }
        
            AssetDatabase.Refresh();
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
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
//             ObjectsDict.Clear();
//             foreach (var item in dataList)
//             {
//                 if (!ObjectsDict.ContainsKey(item.UI_Serializable_Key))
//                 {
//                     ObjectsDict.Add(item.UI_Serializable_Key, item.UI_Serializable_Obj);
//                 }
//                 else
//                 {
// #if UNITY_EDITOR
//                     Debug.LogError($"{item.UI_Serializable_Obj} 组件有重复的，{item.UI_Serializable_Key} 组件path：{item.Path}");
// #endif
//                 }
//             }
        }

        /// <summary>
        /// 反序列化前触发
        /// </summary>
        public void OnBeforeSerialize()
        {
            // //循环赋值
            // foreach (var item in dataList.Where(item =>
            //     item.UI_Serializable_Obj != null && string.IsNullOrEmpty(item.UI_Serializable_Key)))
            // {
            //     item.UI_Serializable_Key = item.UI_Serializable_Obj.name;
            // }
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
    }
}
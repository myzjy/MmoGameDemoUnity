using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ZJYFrameWork.Module.UICommon
{
    public class UIViewObjectPickerGroup : MonoBehaviour
    {
        [FormerlySerializedAs("_MinGameObject")] public GameObject minGameObject;
        [FormerlySerializedAs("_MaxGameObject")] public GameObject maxGameObject;
        [FormerlySerializedAs("_SelectGameObject")] public GameObject selectGameObject;
        /// <summary>
        /// 物体选择列表
        /// </summary>
        private List<GameObject> _gameObjectPackerList;
    }
}
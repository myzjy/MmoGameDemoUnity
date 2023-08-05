using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using NotImplementedException = System.NotImplementedException;

namespace ZJYFrameWork.Module.UICommon
{
    public class UIScrollerViewPicker : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /**
         * 子节点数量（奇数）
         */
        public int _itemNum = 5;

        /**
         * 更新选择的目标值
         */
        [HideInInspector] public float _updateLength;

        /**
         * 子节点的预制体
         */
        public GameObject _ItemObj;

        /// <summary>
        /// 子节点容器对象
        /// </summary>
        public Transform _itemParent;

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}
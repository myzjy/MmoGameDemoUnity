using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using NotImplementedException = System.NotImplementedException;

namespace ZJYFrameWork.Module.UICommon
{
    public class UIScrollerViewPicker : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
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
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FrostEngine
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(NonDrawingGrphic))]
    public class UUIButtonWidget : UUIWidget
    {
        public Button thisButton;
        public Button.ButtonClickedEvent onClick
        {
            get { return thisButton.onClick; }
            set { thisButton.onClick = value; }
        }
#if UNITY_EDITOR

        public override void Reset()
        {
            base.Reset();
            thisButton = GetComponent<Button>();
            var noDrawing = GetComponent<NonDrawingGrphic>();
            if (thisButton != null && noDrawing != null)
            {
                thisButton.targetGraphic = noDrawing;
            }
        }
#endif
        public void AddListener(UnityAction call)
        {
            thisButton.onClick.AddListener(call);
        }

        public void RemoveListener(UnityAction call)
        {
            thisButton.onClick.RemoveListener(call);
        }

        public void RemoveAllListeners()
        {
            thisButton.onClick.RemoveAllListeners();
        }
    }
}
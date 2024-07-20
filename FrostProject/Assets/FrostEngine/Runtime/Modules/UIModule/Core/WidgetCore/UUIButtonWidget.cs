using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FrostEngine
{
    [RequireComponent(typeof(Button))]
    public class UUIButtonWidget:UUIWidget
    {
        public Button thisButton;
        public Button.ButtonClickedEvent onClick
        {
            get { return thisButton.onClick; }
            set { thisButton.onClick = value; }
        }

        public void OnEnable()
        {
            thisButton = GetComponent<Button>();
        }

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
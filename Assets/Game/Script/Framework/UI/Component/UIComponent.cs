using System;
using UnityEngine;
using ZJYFrameWork.Base;
using ZJYFrameWork.UISerializable.Framwork.UIRootCS;

namespace ZJYFrameWork.UISerializable
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Game/FrameWork/UI")]
    public class UIComponent:SpringComponent
    {
        /// <summary>
        /// 当前打开UI事件
        /// </summary>
        private string OpenEventString=string.Empty;
        [SerializeField] private UIRoot Root = null;
        public UIRoot GetRoot => Root;
        public void OnOpenUIEvent()
        {
            
        }
    }
}
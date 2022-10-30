using System;
using UnityEngine;
using ZJYFrameWork.Base;

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

        public void OnOpenUIEvent()
        {
            
        }
    }
}
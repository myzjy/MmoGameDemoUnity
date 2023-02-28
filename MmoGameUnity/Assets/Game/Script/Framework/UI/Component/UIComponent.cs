using UnityEngine;
using ZJYFrameWork.Base;
using ZJYFrameWork.Spring.Core;
using ZJYFrameWork.UISerializable.Framwork.UIRootCS;

namespace ZJYFrameWork.UISerializable
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Game/FrameWork/UI")]
    public class UIComponent : SpringComponent
    {
        [SerializeField] private UIRoot Root = null;

        /// <summary>
        /// 当前打开UI事件
        /// </summary>
        private string OpenEventString = string.Empty;

        public UIRoot GetRoot => Root;

        public void OnOpenUIEvent()
        {
        }

        [AfterPostConstruct]
        public void Init()
        {
            GetRoot.SortOrder();
        }
    }
}
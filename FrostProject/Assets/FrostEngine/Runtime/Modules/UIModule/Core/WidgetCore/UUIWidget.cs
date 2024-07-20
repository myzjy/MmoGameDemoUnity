using UnityEngine;

namespace FrostEngine
{
    /// <summary>
    /// UIWidget 区别框架侧的UIWidget 这个是挂载在UI上面
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class UUIWidget : MonoBehaviour
    {
        /// <summary>
        /// 标识 当前组件会不会被调度到
        /// </summary>
        [SerializeField] private bool IsVariable = false;

        public bool GetIsVariable => IsVariable;

        /// <summary>
        ///  显示组件
        /// </summary>
        [SerializeField] private CanvasGroup mCanvasGroup;

        public CanvasGroup CanvasGroup => mCanvasGroup;

        public virtual void OnCanvasGroupOpen()
        {
            CanvasGroup.alpha = 1;
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.interactable = true;
        }

        public virtual void OnCanvasGroupClose()
        {
            CanvasGroup.alpha = 0;
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.interactable = false;
        }
    }
}
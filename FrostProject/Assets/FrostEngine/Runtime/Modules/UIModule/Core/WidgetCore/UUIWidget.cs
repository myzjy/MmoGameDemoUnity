using UnityEngine;

namespace FrostEngine.Core.WidgetCore
{
    /// <summary>
    /// UIWidget 区别框架侧的UIWidget 这个是挂载在UI上面
    /// </summary>
    public class UUIWidget : MonoBehaviour
    {
        /// <summary>
        /// 标识 当前组件会不会被调度到
        /// </summary>
        private bool IsVariable = false;

        public bool GetIsVariable => IsVariable;
    }
}
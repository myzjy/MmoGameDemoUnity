using UnityEngine.UI;

namespace ZJYFrameWork.Hotfix.UI.Common
{
    public class UIViewObject
    {
        /// <summary>
        /// 设置文本
        /// </summary>
        /// <param name="textUI">Text 组件</param>
        /// <param name="text">文本</param>
        public void SetText(Text textUI, string text)
        {
            textUI.text = text;
        }
    }
}
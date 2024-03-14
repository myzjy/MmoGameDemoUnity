using UnityEngine.Events;
using UnityEngine.UI;
using XLua;

namespace ZJYFrameWork.Common
{
    [LuaCallCSharp]
    public static class GameCommonUtil
    {
        
        public static void SetListener(Button button, UnityAction action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }
    }
}
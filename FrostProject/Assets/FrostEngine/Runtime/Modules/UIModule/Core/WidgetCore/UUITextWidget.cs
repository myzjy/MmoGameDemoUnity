using UnityEngine;
using UnityEngine.UI;

namespace FrostEngine
{
    [RequireComponent(typeof(Text))]
    [DisallowMultipleComponent]
    public class UUITextWidget:UUIWidget
    {
        public Text text = null;
#if UNITY_EDITOR
        public override void Reset()
        {
            base.Reset();
            text = GetComponent<Text>();
        }
#endif
    }
}
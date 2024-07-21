using UnityEngine;
using UnityEngine.UI;

namespace FrostEngine
{
    [RequireComponent(typeof(Image))]
    public class UUIImageWidget:UUIWidget
    {
        public Image _image;

#if UNITY_EDITOR
        public override void Reset()
        {
            base.Reset();
            _image = GetComponent<Image>();
        }
#endif
    }
}
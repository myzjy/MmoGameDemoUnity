using UnityEngine;

namespace ZJYFrameWork.UISerializable
{
    public class LoadingRotate:MonoBehaviour
    {
        public void OnShow()
        {
            gameObject.SetActive(true);
        }

        public void OnClose()
        {
            gameObject.SetActive(false);
        }
    }
}
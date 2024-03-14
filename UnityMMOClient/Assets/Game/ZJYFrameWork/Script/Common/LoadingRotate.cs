using System;
using UnityEngine;

namespace ZJYFrameWork.UISerializable.Common
{
    public class LoadingRotate : MonoBehaviour
    {
        public Animation loadingDot;

        public void OnShow()
        {
            gameObject.SetActive(true);
            loadingDot.enabled = true;
        }

        public void OnClose()
        {
            gameObject.SetActive(false);
            loadingDot.enabled = false;
        }

        private void OnDisable()
        {
            loadingDot.enabled = false;
        }
    }
}
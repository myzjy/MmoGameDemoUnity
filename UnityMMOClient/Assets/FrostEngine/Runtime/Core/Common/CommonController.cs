using System;
using UnityEngine;
using ZJYFrameWork.Spring.Core;
using Object = UnityEngine.Object;

namespace ZJYFrameWork.UISerializable.Common
{
    public class CommonController : MonoBehaviour
    {
        private static CommonController instance = null;

        public static CommonController Instance
        {
            get { return instance; }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            Init();
        }

        public LoadingRotate loadingRotate;
        public Snackbar snackbar;
        public bool isLuaInit = false;

        protected void Init()
        {
        }
    }
}
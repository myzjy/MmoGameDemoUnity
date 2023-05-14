using UnityEngine;
using ZJYFrameWork.Spring.Core;

namespace ZJYFrameWork.UISerializable.Common
{
    public class CommonController : Singleton<CommonController>
    {
        public LoadingRotate loadingRotate;
        public Snackbar snackbar;

        protected override void Init()
        {
            base.Init();
        }


    }
}
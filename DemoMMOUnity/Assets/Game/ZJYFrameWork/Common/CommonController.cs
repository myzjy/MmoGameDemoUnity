using UnityEngine;

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
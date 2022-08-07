using System.Collections;

namespace ZJYFrameWork.AssetBundleLoader
{
    public abstract class LoadingMethod
    {
        internal RequestInfo info;

        internal void SetRequestInfo(RequestInfo info)
        {
            this.info = info;
        }
		
        public abstract void StartLoading();
        public abstract bool Update();
        public abstract IEnumerator AbortLoading();
    }
}
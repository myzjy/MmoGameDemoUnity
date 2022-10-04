using ZJYFrameWork.AssetBundles.Observables;

namespace ZJYFrameWork.AssetBundles.EditorAssetBundle.Editors
{
    [System.Serializable]
    public abstract class AbstractViewModel : ObservableObject
    {
        public virtual void OnEnable()
        {
        }

        public virtual void OnDisable()
        {
        }
    }
}
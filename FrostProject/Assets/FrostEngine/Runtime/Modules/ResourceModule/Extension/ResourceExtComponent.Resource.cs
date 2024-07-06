using Cysharp.Threading.Tasks;

namespace FrostEngine
{
    public partial class ResourceExtComponent
    {
        /// <summary>
        /// 资源组件。
        /// </summary>
        private ResourceModule m_ResourceModule;

        private LoadAssetCallbacks m_LoadAssetCallbacks;

        private void InitializedResources()
        {
            m_ResourceModule = GameModule.Get<ResourceModule>();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(OnLoadAssetSuccess, OnLoadAssetFailure);
        }

        private void OnLoadAssetFailure(string assetName, LoadResourceStatus status, string errormessage,
            object userdata)
        {
            _assetLoadingList.Remove(assetName);
            Debug.LogError("Can not load asset from '{}' with error message '{}'.", assetName, errormessage);
        }

        private void OnLoadAssetSuccess(string assetName, object asset, float duration, object userdata)
        {
            _assetLoadingList.Remove(assetName);
            ISetAssetObject setAssetObject = (ISetAssetObject)userdata;
            UnityEngine.Object assetObject = asset as UnityEngine.Object;
            if (assetObject != null)
            {
                m_AssetItemPool.Register(AssetItemObject.Create(setAssetObject.Location, assetObject), true);
                SetAsset(setAssetObject, assetObject);
            }
            else
            {
                Debug.LogError($"Load failure asset type is {asset.GetType()}.");
            }
        }

        /// <summary>
        /// 通过资源系统设置资源。
        /// </summary>
        /// <param name="setAssetObject">需要设置的对象。</param>
        public async UniTaskVoid SetAssetByResources<T>(ISetAssetObject setAssetObject) where T : UnityEngine.Object
        {
            await TryWaitingLoading(setAssetObject.Location);

            if (m_AssetItemPool.CanSpawn(setAssetObject.Location))
            {
                var assetObject = (T)m_AssetItemPool.Spawn(setAssetObject.Location).Target;
                SetAsset(setAssetObject, assetObject);
            }
            else
            {
                _assetLoadingList.Add(setAssetObject.Location);
                m_ResourceModule.LoadAssetAsync(setAssetObject.Location, typeof(T), m_LoadAssetCallbacks,
                    setAssetObject);
            }
        }
    }
}
---@class ZJYFrameWork.AssetBundles.AssetBundlesManager.AssetBundleManager
---@field public BundleRoot string
---@field public StorableDirectory string
---@field public ReadOnlyDirectory string
---@field public TemporaryCacheDirectory string
---@field public UpdatePrefixUri string
---@field public ApplicableGameVersion string
---@field public AssetAutoReleaseInterval number
---@field public AssetCapacity integer
---@field public AssetExpireTime number
---@field public ResourceAutoReleaseInterval number
---@field public ResourceCapacity integer
---@field public ResourceExpireTime number
---@field public ResourcePriority integer
---@field public BundleManifest ZJYFrameWork.AssetBundles.Bundles.BundleManifest
---@field public AOTMetaAssemblyNames table<integer,string>

---@type ZJYFrameWork.AssetBundles.AssetBundlesManager.AssetBundleManager
ZJYFrameWork.AssetBundles.AssetBundlesManager.AssetBundleManager = {}


---@param assetBundle string
---@param loadAssetCallbacks fun(t:UnityEngine.Object)
function ZJYFrameWork.AssetBundles.AssetBundlesManager.AssetBundleManager:LoadAssetAction(assetBundle,loadAssetCallbacks) end

---@return UnityEngine.Object
---@param assetBundle string
function ZJYFrameWork.AssetBundles.AssetBundlesManager.AssetBundleManager:LoadAsset(assetBundle) end

---@return UnityEngine.GameObject
---@param assetBundle string
function ZJYFrameWork.AssetBundles.AssetBundlesManager.AssetBundleManager:LoadAssetGameObject(assetBundle) end
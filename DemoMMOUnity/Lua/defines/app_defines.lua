--默认东八区时间
global.UNIVERSAL_TIMEDELTA = 8 * 3600
UNIVERSAL_TIMEDELTA = 8 * 3600

UnityEngine = CS.UnityEngine
ZJYFrameWork = CS.ZJYFrameWork
System = CS.System
DG = CS.DG


function GetAssetBundleManager()
    local AssetBundleManager = ZJYFrameWork.Spring.Core.Springtext.GetBean("AssetBundleManager") or
        ZJYFrameWork.AssetBundles.AssetBundlesManager.AssetBundleManager
    return AssetBundleManager
end

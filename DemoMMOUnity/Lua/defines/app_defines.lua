--默认东八区时间
global.UNIVERSAL_TIMEDELTA = 8 * 3600
UNIVERSAL_TIMEDELTA = 8 * 3600

UnityEngine = CS.UnityEngine
ZJYFrameWork = CS.ZJYFrameWork
System = CS.System
DG = CS.DG

local assetBundleManagerBeanStr = "ZJYFrameWork.AssetBundles.AssetBundlesManager.AssetBundleManager"
local SchedulerManagerBeanStr = "ZJYFrameWork.Scheduler.SchedulerManager"

function GetAssetBundleManager()
    local AssetBundleManager = ZJYFrameWork.Spring.Core.SpringContext.GetBean(assetBundleManagerBeanStr) or
        ZJYFrameWork.AssetBundles.AssetBundlesManager.AssetBundleManager
    return AssetBundleManager
end
function GetSchedulerManager()
    local SchedulerManager = ZJYFrameWork.Spring.Core.SpringContext.GetBean(SchedulerManagerBeanStr) or
    ZJYFrameWork.Scheduler.SchedulerManager
    return SchedulerManager
end
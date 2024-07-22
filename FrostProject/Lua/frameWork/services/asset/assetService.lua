--[[
    author : zhangjingyi 
    DESCRIPTION : 资源
--]]

---@class AssetService:ServiceBase
local AssetService = Class("AssetService",ClassLibraryMap.ServiceBase)

function AssetService:ctor()
    -- 资源
    self._resourceModules = GameModule.Resource
    self._preLoadAssetCallbacks = CS.FrostEngine.LoadAssetCallbacks()
end

function AssetService:LoadAssetAsync(inLocation, inCallBack, inAssetType, inAssetName)
    self._preLoadAssetCallbacks = CS.FrostEngine.LoadAssetCallbacks(function (assetName, asset, duration, userdata)
        -- OnPreLoadAssetSuccess
        inCallBack:OnLoadAssetCompleted(0,inAssetType,assetName)
    end,function (assetName, status, errormessage, userdata)
        
    end)
    self._resourceModules:LoadAssetAsync(inAssetName, inAssetType,  self._preLoadAssetCallbacks)
end

function AssetService.OnPreLoadAssetSuccess(assetName, asset, duration, userdata)
    
end

return AssetService
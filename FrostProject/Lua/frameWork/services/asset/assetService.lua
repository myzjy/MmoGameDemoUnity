--[[
    author : zhangjingyi 
    DESCRIPTION : 资源
--]]

---@class AssetService:ServiceBase
local AssetService = Class("AssetService",ClassLibraryMap.ServiceBase)

function AssetService:ctor()
    -- 资源
    self._resourceModules = GameModule.Resource
    self._preLoadAssetCallbacks = {}
end

function AssetService:vGetConfig()
    return {
        name = "AssetService",
    }
end

function AssetService:LoadAssetAsync(inLocation, inCallBack, inAssetType, inAssetName)
    self._preLoadAssetCallbacks = CS.FrostEngine.LoadAssetCallbacks(function (assetName, asset, duration, userdata)
        -- OnPreLoadAssetSuccess
        inCallBack:OnLoadAssetCompleted(0,inAssetType,assetName)
    end,function (assetName, status, errormessage, userdata)
        
    end)
    self._resourceModules:LoadAssetAsync(inAssetName, inAssetType,  self._preLoadAssetCallbacks)
end

---@param gameState any
---@param UiState any
---@param completedEventHandler any
---@param assetCountPerFrame any
---@param sceneID any
function AssetService:Load(gameState, UiState, completedEventHandler, assetCountPerFrame, sceneID)
    local strs = string.split(UiState,'/')
    if strs and #strs > 0 then
        self._preLoadAssetCallbacks = CS.FrostEngine.LoadAssetCallbacks(function (assetName, asset, duration, userdata)
            -- OnPreLoadAssetSuccess
            completedEventHandler(assetName, asset)
        end,function (assetName, status, errormessage, userdata)
            
        end)
        self._resourceModules:LoadAssetAsync(strs[1], typeof(CS.UnityEngine.GameObject),  self._preLoadAssetCallbacks)
        return true
    end
    return false
end

function AssetService:ClearCache(...)
    
end

function AssetService:ClearAllCache(inClearSceneUI)
end

return AssetService

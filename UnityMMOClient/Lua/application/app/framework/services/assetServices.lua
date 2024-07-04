---@class AssetServices
local AssetServices = class("AssetServices")

function AssetServices:ctor()
    self.assetLoadServices = GetAssetBundleManager()
end

function AssetServices:OnLoadAssetAction(assetBundle, loadAssetCallbacks)
    if not self.assetLoadServices then
        PrintError("错误AssetBundleManager Bean 没有注册")
        return
    end
    if self.assetLoadServices.LoadAssetAction then
        PrintError("AssetBundleManager 类 不存在 LoadAssetAction 方法")
    end
    self.assetLoadServices:LoadAssetAction(assetBundle, loadAssetCallbacks)
end

----------------------------------------------------------------------
--- public
--- @param inAssetBundle string 资源名
--- @param tLoadAssetCallbacks function
----------------------------------------------------------------------
function AssetServices:OnLoadAssetAsync(inAssetBundle, tLoadAssetCallbacks)
    if not self.assetLoadServices then
        PrintError("AssetServices:OnLoadAssetAsync 错误AssetBundleManager Bean 没有注册")
        return
    end
    if self.assetLoadServices.LoadAssetAsync then
        PrintError("AssetServices:OnLoadAssetAsync AssetBundleManager 类 不存在 LoadAssetAsync 方法")
    end
    self.assetLoadServices:LoadAssetAsync(inAssetBundle, function(obj)
        if not obj then
            return
        end
        tLoadAssetCallbacks(inAssetBundle, obj)
    end)
end

----------------------------------------------------------------------
--- public
--- @param inAssetBundle string 资源名
--- @return UnityEngine.Object
----------------------------------------------------------------------
function AssetServices:OnLoadAsset(inAssetBundle)
    if not self.assetLoadServices then
        PrintError("AssetServices:LoadAsset 错误AssetBundleManager Bean 没有注册")
        return
    end
    if self.assetLoadServices.LoadAsset then
        PrintError("AssetServices:LoadAsset AssetBundleManager 类 不存在 LoadAsset 方法")
    end
    local tObject = self.assetLoadServices:LoadAsset(inAssetBundle)
    if not tObject then
        PrintError("AssetServices:LoadAsset  %s.assetbundle not ", inAssetBundle)
    end
    return tObject
end
function AssetServices:OnLoadUIAsset(inAssetBundle, uiGlobal)
    -- 获取 UI 全局变量
    local tUIValue = LuaUtils.GetGlobalVariable(uiGlobal)
    tUIValue()
end

--------------------------------------------------------------------
--- public
---@param inAssetBundle string 资源名
---@return UnityEngine.GameObject
---------------------------------------------------------------------
function AssetServices:OnLoadAssetGameObject(inAssetBundle)
    if not self.assetLoadServices then
        PrintError("AssetServices:LoadAsset 错误AssetBundleManager Bean 没有注册")
        return
    end
    if self.assetLoadServices.LoadAsset then
        PrintError("AssetServices:OnLoadAssetGameObject AssetBundleManager 类 不存在 LoadAsset 方法")
    end
    local tObject = self.assetLoadServices:LoadAssetGameObject(inAssetBundle)
    if not tObject then
        PrintError("AssetServices:OnLoadAssetGameObject  %s.assetbundle not ", inAssetBundle)
    end
    return tObject
end
return AssetServices

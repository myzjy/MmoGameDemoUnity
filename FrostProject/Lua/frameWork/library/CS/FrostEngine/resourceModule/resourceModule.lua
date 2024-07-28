---@class CS.FrostEngine.ResourceModule
CS.FrostEngine.ResourceModule = {}

--- 检查资源是否存在
---@param location string 要检查资源的名称
---@param customPackageName string 指定资源包的名称。不传使用默认资源包
---@return integer 检查资源是否存在的结果
function CS.FrostEngine.ResourceModule:HasAsset(location, customPackageName)
    return 0
end

---检查资源定位地址是否有效。
---@param location string 资源的定位地址
---@param customPackageName string 指定资源包的名称。不传使用默认资源包
---@return boolean
function CS.FrostEngine.ResourceModule:CheckLocationValid(location, customPackageName)
    return false
end

---获取资源信息列表
---@param resTag string 资源标签
---@param customPackageName string 指定资源包的名称。不传使用默认资源包
---@return table<number,CS.YooAsset.AssetInfo>
function CS.FrostEngine.ResourceModule:GetAssetInfos(resTag, customPackageName)
    return {}
end

--- 异步加载资源
---@param location string
---@param assetType CS.System.Type
---@param priority integer
---@param loadAssetCallbacks CS.FrostEngine.LoadAssetCallbacks
---@param userData CS.System.Object
---@param packageName string
---@overload fun(location:string, assetType:CS.System.Type, loadAssetCallbacks:CS.FrostEngine.LoadAssetCallbacks, userData:CS.System.Object, packageName:string)
function CS.FrostEngine.ResourceModule:LoadAssetAsync(location, assetType, priority, loadAssetCallbacks, userData, packageName)
end

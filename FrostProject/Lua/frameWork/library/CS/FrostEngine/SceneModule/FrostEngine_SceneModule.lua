---@class FrostEngine.SceneModule
CS.FrostEngine.SceneModule = {
    ---@type string 当前主场景名称。
    CurrentMainSceneName = "",
}

--- 加载场景
---@param location string 场景的定位地址
---@param sceneMode UnityEngine.SceneManagement.LoadSceneMode 场景加载模式
---@param suspendLoad boolean 加载完毕时是否主动挂起
---@param priority number 优先级
---@param callBack fun(parame: YooAsset.SceneHandle) 加载回调
---@param gcCollect boolean 加载主场景是否回收垃圾
---@param progressCallBack fun(parame: number) 加载进度回调
---@return YooAsset.SceneHandle
function CS.FrostEngine.SceneModule:LoadScene(location, sceneMode, suspendLoad, priority, callBack, gcCollect, progressCallBack)
    return {}
end

--- 加载子场景
---@param location string 场景的定位地址
---@param suspendLoad boolean 加载完毕时是否主动挂起
---@param priority number 优先级
---@param callBack fun(parame: YooAsset.SceneHandle) 加载回调
---@param gcCollect boolean 加载主场景是否回收垃圾
---@param progressCallBack fun(parame: number) 加载进度回调
---@return YooAsset.SceneHandle
function CS.FrostEngine.SceneModule:LoadSubScene(location,suspendLoad,priority,callBack,gcCollect,progressCallBack)
    return {}
end

--- 激活场景（当同时存在多个场景时用于切换激活场景）。
---@param location string 场景资源定位地址
---@return boolean 是否操作成功
function CS.FrostEngine.SceneModule:ActivateScene(location)
    return false
end


---解除场景加载挂起操作。
---@param location string 场景资源定位地址。
---@return boolean 是否操作成功。
function CS.FrostEngine.SceneModule:UnSuspend(location)
    return false
end

--- 是否为主场景。
---@param location any
---@return boolean
function CS.FrostEngine.SceneModule:IsMainScene(location)
    return false
end

--- 异步卸载子场景。
---@param location string
---@return CS.YooAsset.UnloadSceneOperation
function CS.FrostEngine.SceneModule:UnloadAsync(location)
    return {}
end

--- 是否包含场景。
---@param location string 场景资源定位地址。
---@return boolean 是否包含场景。
function CS.FrostEngine.SceneModule:IsContainScene(location)
    return false
end
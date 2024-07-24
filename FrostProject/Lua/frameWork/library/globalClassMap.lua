---@class global
---@field GameTimeService GameTimeService
---@field NetMessageService NetMessageService
---@field ScheduleService ScheduleService
---@field GameStageService GameStageService
---@field EventService EventService
---@field MVCService MVCService
---@field UPool UPoolClass
---@field DataCentreService DataCentreService
---@field AssetService AssetService

---@class CS.FrostEngine.SceneModule
---@field CurrentMainSceneName string 当前主场景名称。
---@field LoadScene fun(location:string,sceneMode:CS.UnityEngine.SceneManagement.LoadSceneMode,suspendLoad:boolean,priority:number,callBack:fun(parame:CS.YooAsset.SceneHandle),gcCollect:boolean,progressCallBack:fun(parame:number)):CS.YooAsset.SceneHandle
---@field LoadSubScene fun(location:string,suspendLoad:boolean,priority:number,callBack:fun(parame:CS.YooAsset.SceneHandle),gcCollect:boolean,progressCallBack:fun(parame:number)):CS.YooAsset.SceneHandle
---@field ActivateScene fun(location:boolean):boolean 激活场景（当同时存在多个场景时用于切换激活场景）。
---@field UnSuspend fun(location:boolean):boolean 解除场景加载挂起操作。
---@field IsMainScene fun(location:string):boolean 是否为主场景。
---@field UnloadAsync fun(location:string):CS.UnityEngine.SceneManagement.UnloadSceneOperation 异步卸载子场景。
---@field IsContainScene fun(location:string):boolean 是否包含场景
GameModule.Scene = {}

---@class CS.FrostEngine.GameModule
---@field Scene CS.FrostEngine.SceneModule

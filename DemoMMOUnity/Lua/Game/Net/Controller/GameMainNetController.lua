---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/6/9 18:25
---

---@class GameMainNetController
local GameMainNetController = class("GameMainNetController")
GameMainConst = {
    Status = {},
    Event = {
        ServerData = 1010
    }
}

--- GameMainNetController 初始化
function GameMainNetController:Init()
    --- 初始化事件
    GameMainNetController:InitEvent()
end

--- GameMainNetController 事件初始化
function GameMainNetController:InitEvent()
    GlobalEventSystem:Bind(GameMainConst.Event.ServerData, function(response)
        --- 请求服务器 部分配置表时 在打开游戏登录界面之前 必须请求完成
        GameMainNetController:AtServerConfigResponse(response)
    end)
end

function GameMainNetController:AtServerConfigResponse(response)
    ServerDataManager:GetInstance():SetItemBaseDataList(response.bagItemEntityList)
    CommonController.Instance.snackbar:OpenUIDataScenePanel(1, 1)
    --- 关闭 登陆 注册界面
    DispatchEvent(LoginConfig.eventNotification.CLOSE_LOGIN_INIT_PANEL)
    --- 打开游戏主界面
    GameMainViewController:GetInstance():OnOpen()
end

function GameMainNetController:AtPhysicalPowerUserPropsResponse(response)
    
end

return GameMainNetController


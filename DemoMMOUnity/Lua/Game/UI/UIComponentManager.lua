--- UIModelView 总管理类
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/5/17 15:53
---

local UIModelInterfaces = {}
local UIComponentManager = {}

UIComponentManager.UIEventNotificationDict = {}

function UIComponentManager:InitUIModelComponent()
    --- 这里不能使用 baseClass 的new函数 去读取函数 会报错
    --- 创建一个新的table ，不会读取到正确 元地址
    UIModelInterfaces["loginUI"] = require("Game.UI.Login.LoginUIModelView")
    UIModelInterfaces["gameMainUI"] = require("Game.UI.GameMain.ModelView.GameMainModelView")
    ---循环遍历
    for i, v in pairs(UIModelInterfaces) do
        for _i, _v in pairs(v.Notification()) do
            local uiAction = UIComponentManager.UIEventNotificationDict[_v]
            if uiAction == nil then
                local data = v
                --- 当前 UI事件没有存储
                UIComponentManager.UIEventNotificationDict[_v] = function(_eventNotification)
                    data:NotificationHandler(_eventNotification)
                end
            else
                error("[class:" .. i .. "]" .. "[Notification:" .. _v .. "] 有重复的事件id")
            end
        end
    end

end

--- 调用事件
--- 当前函数为全局函数 放置与全局表中
function DispatchEvent(name, body)
    local eventUI = require("Game.Common.UINotification"):new(name, body)
    local eventAction = UIComponentManager.UIEventNotificationDict[name]
    if eventAction ~= nil then
        eventAction(eventUI)
    end
end

return UIComponentManager
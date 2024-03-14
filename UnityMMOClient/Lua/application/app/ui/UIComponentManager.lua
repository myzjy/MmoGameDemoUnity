--- UIModelView 总管理类
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/5/17 15:53


printDebug("require ui framework lua scripts start")
require("application.app.ui.framework.luaUIObject")
require("application.app.ui.framework.luaObject")
require("application.app.ui.framework.UIBaseView")
require("application.app.ui.framework.UIView")

printDebug("require ui framework lua scripts end")


---
printDebug("require UI commonUI lua scripts start")

require("application.app.ui.commonUI.UICommonView")
UICommonViewController = require("application.app.ui.commonUI.UICommonViewController"):GetInstance()
require("application.app.ui.commonUI.dialog")
DialogConfig = require("application.app.ui.commonUI.dialogConfig")
require("application.app.ui.commonUI.loadingRotate")

printDebug("require UI commonUI lua scripts end")

printDebug("require UI commonUI lua scripts config start")

UIConfigEnum = require("application.app.ui.config.UIConfig")
LoginConfig = require("application.app.ui.login.loginConfig")

printDebug("require UI commonUI lua scripts config end")

printDebug("require UI View Lua Scirpt start")
require("application.app.ui.generateScripts.UIModules.LoginPanelView")
require("application.app.ui.generateScripts.UIModules.GameMainUIPanelView")
require("application.app.ui.bag.modelView.bagModelUIView")
require("application.app.ui.generateScripts.UIModules.BagUIPanelView")

printDebug("require UI View Lua Scirpt end")

printDebug("start require UI controller lua scripts ing ...")

LoginUIController = require("application.app.ui.login.LoginUIController"):GetInstance()
GameMainUIViewController = require("application.app.ui.gameMain.gameMainUIViewController"):GetInstance()
BagUIController = require("application.app.ui.bag.bagUIController").GetInstance()

printDebug("end require UI controller lua scripts ing ...")




local UIComponentManager = {}

UIComponentManager.UIEventNotificationDict = {}

function UIComponentManager:InitUIModelComponent()
    --- 创建一个新的table ，不会读取到正确 元地址
    require("application.app.ui.login.LoginView")
    require("application.app.ui.gameMain.gameMainView")
    local bagUIView = require("application.app.ui.bag.bagUIView")
    for _i, _v in pairs(LoginView:Notification()) do
        local uiAction = UIComponentManager.UIEventNotificationDict[_v]
        if uiAction == nil then
            --local data = v
            --- 当前 UI事件没有存储
            UIComponentManager.UIEventNotificationDict[_v] = function(_eventNotification)
                LoginView:NotificationHandler(_eventNotification)
            end
        else
            printError("[class:" .. _i .. "]" .. "[Notification:" .. _v .. "] 有重复的事件id")
        end
    end
    for _i, _v in pairs(GameMainView:Notification()) do
        local uiAction = UIComponentManager.UIEventNotificationDict[_v]
        if uiAction == nil then
            --local data = v
            --- 当前 UI事件没有存储
            UIComponentManager.UIEventNotificationDict[_v] = function(_eventNotification)
                GameMainView:NotificationHandler(_eventNotification)
            end
        else
            printError("[class:" .. _i .. "]" .. "[Notification:" .. _v .. "] 有重复的事件id")
        end
    end
    for _i, _v in pairs(bagUIView:Notification()) do
        local uiAction = UIComponentManager.UIEventNotificationDict[_v]
        if uiAction == nil then
            --local data = v
            --- 当前 UI事件没有存储
            UIComponentManager.UIEventNotificationDict[_v] = function(_eventNotification)
                bagUIView:NotificationHandler(_eventNotification)
            end
        else
            printError("[class:" .. _i .. "]" .. "[Notification:" .. _v .. "] 有重复的事件id")
        end
    end
end

--- 调用事件
--- 当前函数为全局函数 放置与全局表中
function DispatchEvent(name, body)
    local eventUI = require("application.app.ui.commonUI.UINotification"):new(name, body)
    local eventAction = UIComponentManager.UIEventNotificationDict[name]
    if eventAction ~= nil then
        eventAction(eventUI)
    end
end

return UIComponentManager

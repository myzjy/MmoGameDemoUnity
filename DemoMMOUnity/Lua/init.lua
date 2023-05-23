--- 初始化相关函数
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/5/16 19:33
---
Debug = 0
configurationDevice = {}

function init()
    Debug = CS.ZJYFrameWork.Common.CommonManager.Instance:DebugConfig()
    configurationDevice = CS.ZJYFrameWork.Common.AppConfig.configurationDevice
end
---试一试直接运行
---
init()
require("utils.functions")

printDebug("# DEBUG                        = " .. Debug)
printDebug("Game Main working...")
printDebug("Game Main Init...")
printDebug("Init Global init start")
-- export global variable

local __g = _G
--- 定义全局表
global = {}
setmetatable(global, {
    __newindex = function(_, name, value)
        rawset(__g, name, value)
    end,

    __index = function(_, name)
        return rawget(__g, name)
    end
})
global.class = require("Common.class")

local lastLuaExceptionMsg

function __G__TRACKBACK__(exceptionMsg)
    local msg = string.format("[%d] %s\n", handleLuaExceptionIdx, tostring(exceptionMsg))
    local str = "LUA ERROR: " .. msg
    print("----------------------------------------")
    print(str)
    print(debug.traceback())
    print("----------------------------------------")

    -- 若报错信息与上一条相同，忽略显示
    --if lastLuaExceptionMsg == exceptionMsg then
    --    return
    --end
    lastLuaExceptionMsg = exceptionMsg
    --handleLuaException(msg)
    if Debug < 1 then
        -- 正式线上不显示详细信息
        str = str .. debug.traceback() .. "\n"
    end

end
--GlobalEventValue = {}
GlobalEventSystem={}
local function main()
    require("BaseRequire")
    --local GlobalEvents = require("Game.Common.GlobalEvents")
    --GlobalEventValue = GlobalEvents
    printDebug("main() line 63")
    require("Game.Manager.ProtocolManager")
    require("Game.Net.PacketDispatcher")

    --setmetatable(GlobalEventSystem, EventSystem)
    GlobalEventSystem = require("utils.EventSystem").New()
    --global.GlobalEventSystem:Constructor()
    if Debug > 0 then
        printDebug("开启Debug Log")
    else
        log.disable()
    end

    --- UI 初始化
    UIComponentManager.InitUIModelComponent()
    ProtocolManager.initProtocolManager()
    PacketDispatcher:Init()
end


--main()
local status, msg = pcall(main, __G__TRACKBACK__)
if not status then
    print('xpcall robot main error', status, msg)
end
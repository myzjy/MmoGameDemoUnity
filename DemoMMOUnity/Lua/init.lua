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
GlobalEventSystem = {}
local function main()
    require("BaseRequire")
    printDebug("main() line 63")
    require("Game.Net.main")

    GlobalEventSystem = require("utils.EventSystem")
    if Debug > 0 then
        printDebug("开启Debug Log")
    else
        log.disable()
    end

    ----- UI 初始化
    UIComponentManager.InitUIModelComponent()
    ProtocolManager.initProtocolManager()
    PacketDispatcher:Init()
    --local client = cs_generator(function()
    --    printDebug("执行 init 最外 86")
    --    coroutine.yield(CS.UnityEngine.WaitForSeconds(1))
    --    printDebug("1S结束 line 89")
    --end)
    --Executors.RunOnCoroutineNoReturn(client)
    --gameObject:SetActive(false)
    -- printError("tr:" .. type(gameObject:transform()))

    --require("Game.CS.ZJYFrameWork.Hotfix.Common.PlayerUserCaCheData")
end

main()
--local status, msg = pcall(main, __G__TRACKBACK__)
--if not status then
--    print('xpcall robot main error', status, msg)
--end


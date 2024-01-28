Debug = 0
ConfigurationDevice = {}

global = {}
local __g = _G

local function Init()
    Debug = CS.ZJYFrameWork.Common.CommonManager.Instance:DebugConfig()
    ConfigurationDevice = CS.ZJYFrameWork.Common.AppConfig.configurationDevice
end
Init()

require("Common.util.functions")
_G.null = nil
_G.NULL = nil

printDebug("# DEBUG                        = " .. Debug)
printDebug("Game Main working...")
printDebug("Game Main Init...")
printDebug("Init Global init start")
require("BaseRequire")

if UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android then
    printDebug("you iphone is panel android")
elseif UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer then
    printDebug("you iphone is panel ios")
elseif UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WindowsPlayer then
    printDebug("you iphone is panel pc")
else
    printDebug("Current platform is unknown.")
end


function SyncSubmit(eventString)
    if Debug > 0 then
        printDebug("执行事件, eventName:" .. eventString)
    end
    GlobalEventSystem:Fire(eventString)
end

--读取脚本

setmetatable(global, {
    __newindex = function(_, name, value)
        rawset(__g, name, value)
    end,

    __index = function(_, name)
        return rawget(__g, name)
    end
})
local lastLuaExceptionMsg
local handleLuaExceptionIdx = 1

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
    handleLuaExceptionIdx = handleLuaExceptionIdx + 1

    --handleLuaException(msg)
    if Debug < 1 then
        -- 正式线上不显示详细信息
        str = str .. debug.traceback() .. "\n"
    end
end
local function UpdateEvent()
    
end
local function main()
    printDebug("init active main function")
    if Debug > 0 then
        printDebug("open debug log ing....")
    end
    UIComponentManager:InitUIModelComponent()
    UICommonViewController:GetInstance():OnInit()
    ProtocolManager.initProtocolManager()
    PacketDispatcher:Init()
    UpdateBeat:AddListener(UpdateEvent)
    CS.ZJYFrameWork.UISerializable.Common.CommonController.Instance.isLuaInit = true
end

local status, msg = pcall(main, __G__TRACKBACK__)
if not status then
    printError('xpcall robot main error' .. type(status) .. "," .. msg)
end

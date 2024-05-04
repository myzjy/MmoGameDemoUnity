Debug = 0
ConfigurationDevice = {}

Global = {}
local __g = _G

local function Init()
    Debug = CS.ZJYFrameWork.Common.CommonManager.Instance:DebugConfig()
    ConfigurationDevice = CS.ZJYFrameWork.Common.AppConfig.configurationDevice
end
Init()

require("common.util.functions")
_G.null = nil
_G.NULL = nil

PrintDebug("# DEBUG                        = " .. Debug)
PrintDebug("Game Main working...")
PrintDebug("Game Main Init...")
PrintDebug("Init Global init start")
require("BaseRequire")

if UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android then
    PrintDebug("you iphone is panel android")
elseif UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer then
    PrintDebug("you iphone is panel ios")
elseif UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WindowsPlayer then
    PrintDebug("you iphone is panel pc")
else
    PrintDebug("Current platform is unknown.")
end
local inspect = require "common/inspect"

function dump( t )
	print( inspect(t) );
end
function SyncSubmit(eventString)
    if Debug > 0 then
        PrintDebug("执行事件, eventName:" .. eventString)
    end
    GlobalEventSystem:Fire(eventString)
end

--读取脚本

setmetatable(Global, {
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
    PrintDebug("init active main function")
    if Debug > 0 then
        PrintDebug("open debug log ing....")
    end

    UIComponentManager:InitUIModelComponent()
    UICommonViewController:GetInstance():OnInit()
    ProtocolManager.initProtocolManager()
    PacketDispatcher:Init()
    I18nManager:OnInit()
    ProcedureGame:OnInit()
    -- UpdateBeat:AddListener(UpdateEvent)
    CS.ZJYFrameWork.UISerializable.Common.CommonController.Instance.isLuaInit = true
end

local status, msg = pcall(main, __G__TRACKBACK__)
if not status then
    PrintError('xpcall robot main error' .. type(status) .. "," .. msg)
end

--- 错误消息机制
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/5/19 18:55
---
---@class Error
local Error = {}
local messageData = require("Game.Net.serverMessageWrite")
local json = require("Common.json")

function Error:new(errorCode, errorMessage, module)
    local obj = {
        errorCode = errorCode, -- int
        errorMessage = errorMessage, -- java.lang.String
        module = module -- int
    }
    setmetatable(obj, self)
    self.__index = self
    return obj
end

function Error:protocolId()
    return 101
end

function Error:write(buffer, packet)
    --printError(type(packet))
    if packet == nil then
        --log.log(type(packet))
        return
    end
    local data = packet or Error
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = json.encode(message)
    buffer:writeString(jsonStr)
    printDebug(jsonStr)
end

function Error:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    printDebug("Error:read(buffer) line 44" .. jsonString)
    local data = json.decode(jsonString)
    return Error:new(data.packet.errorCode, data.packet.errorMessage, data.packet.module)
end
return Error

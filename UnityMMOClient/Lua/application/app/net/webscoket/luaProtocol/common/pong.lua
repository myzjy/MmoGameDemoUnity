--- ping response
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/5/24 10:35
---
---@class Pong pong
local Pong = class("Pong")

function Pong:ctor()
    ---@type number
    self.time = 0
end

function Pong:new(time)
    self.time = time -- long
    return self
end

function Pong:protocolId()
    return 104
end

function Pong:write(buffer, packet)
    --PrintError(type(packet))
    if packet == nil then
        --log.log(type(packet))
        return
    end
    local data = packet or Pong
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function Pong:read(data)
    return Pong:new(data.packet.time)
end

return Pong

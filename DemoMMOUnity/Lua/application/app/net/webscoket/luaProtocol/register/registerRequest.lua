--- 注册
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Administrator.
--- DateTime: 2023/5/24 0:16

---@class RegisterRequest
local RegisterRequest = {}

function RegisterRequest:new(account, password, affirmPassword)
    local obj = {
        account = account,               -- java.lang.String
        password = password,             -- java.lang.String
        affirmPassword = affirmPassword, -- java.lang.String
    }
    setmetatable(obj, self)
    self.__index = self
    return obj
end

function RegisterRequest:protocolId()
    return 1005
end

function RegisterRequest:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or RegisterRequest
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function RegisterRequest:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    local data = JSON.decode(jsonString)
    return RegisterRequest:new(data.packet.account, data.packet.password, data.packet.affirmPassword)
end

return RegisterRequest

---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/5/22 14:40
---
---@class LoginResponse
local LoginResponse = {}

---@param userName string
function LoginResponse:new(token, uid, userName, goldNum, premiumDiamondNum, diamondNum)
    --local obj = LoginResponse.New()
    local obj = {
        token = token, ---java.lang.String
        uid = uid, -- long
        userName = userName, ---java.lang.String
        goldNum = goldNum, -- long
        premiumDiamondNum = premiumDiamondNum, -- long
        diamondNum = diamondNum -- long
    }
    setmetatable(obj, self)
    self.__index = self
    return obj
end

function LoginResponse:protocolId()
    return 1001
end

function LoginResponse:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or LoginResponse
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end
function LoginResponse:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    local data = JSON.decode(jsonString)
    local jsonData = LoginResponse:new(data.packet.token,
            data.packet.uid,
            data.packet.userName,
            data.packet.goldNum,
            data.packet.premiumDiamondNum,
            data.packet.diamondNum)
    return jsonData
end
return LoginResponse
--- 刷新主界面显示的金币 免费钻石和付费钻石
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/6/13 19:48
---

---@class RefreshingResourcesMainRequest
local RefreshingResourcesMainRequest = {}

local json = require("Common.json")
function RefreshingResourcesMainRequest:new()
    --local obj = LoginResponse.New()
    local obj = {
    }
    setmetatable(obj, self)
    self.__index = self
    return obj
end

function RefreshingResourcesMainRequest:protocolId()
    return 1027
end

function RefreshingResourcesMainRequest:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or RefreshingResourcesMainRequest
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = json.encode(message)
    buffer:writeString(jsonStr)
end

function RefreshingResourcesMainRequest:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    --local data = json.decode(jsonString)
    local jsonData = RefreshingResourcesMainRequest:new()
    return jsonData
end

return ServerConfigResponse
--- 获取体力
--- @class PhysicalPowerRequest
local PhysicalPowerRequest = class("PhysicalPowerRequest")

--- 初始化
---@param uid number
function PhysicalPowerRequest:ctor(uid)
    ---@type number 玩家UID
    self.uid = uid
end

--- 协议号
--- @return integer
function PhysicalPowerRequest:protocolId()
    return 1023
end

--- 将类的数据 转换成服务器需要结构 json
--- @return string 调用协议
function PhysicalPowerRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = { uid = self.uid }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

return PhysicalPowerRequest

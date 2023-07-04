--- 体力恢复
--- @author zjy
--- @since 2023/7/2 21:54

---@class PhysicalPowerSecondsRequest
local PhysicalPowerSecondsRequest = {}

--- 创建最新体力恢复 request 结构
---@param nowTime number 请求时间
function PhysicalPowerSecondsRequest:new(nowTime)
    local obj = {
        nowTime = nowTime
    }
    setmetatable(obj, self)
    self.__index = self
    return obj
end


-- 协议id
---@return number
function PhysicalPowerSecondsRequest:protocolId()
    return 1029
end

---@param buffer ByteBuffer
function PhysicalPowerSecondsRequest:write(buffer, packer)
    if packer == nil then
        -- body
        return
    end
    --- 传递过来得 包数据可能 类型不一定是需要 需要转换一下
    local data = packer or PhysicalPowerSecondsRequest
    --- 数据结合
    local message = {
        protocolId = data:protocolId(),
        packet = data
    }
    local jsonString = JSON.encode(message)
    buffer:writeString(jsonString)
end

---@param buffer ByteBuffer
function PhysicalPowerSecondsRequest:read(buffer)
    local jsonStr = buffer:readString()
    ---@type {protocolId:number,packet:{nowTime:number}}
    local data = JSON.decode(jsonStr)
    local jsonData = PhysicalPowerSecondsRequest:new(data.packet.nowTime)
    return jsonData
end

return PhysicalPowerSecondsRequest
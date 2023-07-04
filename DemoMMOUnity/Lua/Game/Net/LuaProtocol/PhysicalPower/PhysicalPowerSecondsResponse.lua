---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by zhangjingyi.
--- DateTime: 2023/7/4 19:36
---

---@class PhysicalPowerSecondsResponse
local PhysicalPowerSecondsResponse = {}

---@param nowPhysicalPower number 返回 使用体力 所扣除 当前体力
---@param residueTime number 一点体力增长剩余时间
---@param residueNowTime number 当前体力实时时间 会跟着剩余时间一起变化
---@param maximumStrength number 最大体力 用于限制 这个值会随着 等级增长
---@param maximusResidueEndTime number 我恢复到最大体力的结束时间
function PhysicalPowerSecondsResponse:new(nowPhysicalPower, residueTime, residueNowTime, maximumStrength, maximusResidueEndTime)
    local object = {
        nowPhysicalPower = nowPhysicalPower,
        residueTime = residueTime,
        maximumStrength = maximumStrength,
        maximusResidueEndTime = maximusResidueEndTime,
        residueNowTime = residueNowTime
    }
    setmetatable(object, self)
    self.__index = self
    return object
end

function PhysicalPowerSecondsResponse:protocolId()
    return 1030
end

---
---@param buffer ByteBuffer
function PhysicalPowerSecondsResponse:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or PhysicalPowerSecondsResponse
    local message = {
        protocolId = data:protocolId(),
        packet = data
    }
    --- 数据转换成 json string
    local jsonStr = JSON.encode(message)
    --- 写入json string
    buffer:writeString(jsonStr)
end

--- 读取字节流 转换成对应数据结构
--- @param buffer ByteBuffer
function PhysicalPowerSecondsResponse:read(buffer)
    --- 读字符
    local jsonStr = buffer:readString()
    ---@type {protocolId:number,packet:{nowPhysicalPower:number, residueTime:number, residueNowTime:number, maximumStrength:number, maximusResidueEndTime:number}}
    local data = JSON.decode(jsonStr)
    local jsonData = PhysicalPowerSecondsResponse:new(data.packet.nowPhysicalPower,
            data.packet.residueTime,
            data.packet.residueNowTime,
            data.packet.maximumStrength,
            data.packet.maximusResidueEndTime)
    return jsonData
end

return PhysicalPowerSecondsResponse

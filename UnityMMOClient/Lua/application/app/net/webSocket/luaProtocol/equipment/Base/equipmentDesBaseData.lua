---@class EquipmentDesBaseData
local EquipmentDesBaseData = class("EquipmentDesBaseData")
function EquipmentDesBaseData:ctor()
    --- 介绍id
    ---@type number
    self.desId = 0
    --- 介绍
    ---@type string
    self.desStr = string.empty
    --- 这个介绍圣遗物的名字
    ---@type string
    self.name = string.empty
    --- 故事
    ---@type string
    self.storyDesStr = string.empty
end

---@param desId number 介绍id
---@param desStr string 介绍
---@param name string 这个介绍圣遗物的名字
---@param storyDesStr string 故事
---@return EquipmentDesBaseData
function EquipmentDesBaseData:new(desId, desStr, name, storyDesStr)
    self.desId = desId --- int
    self.desStr = desStr --- java.lang.String
    self.name = name --- java.lang.String
    self.storyDesStr = storyDesStr --- java.lang.String
    return self
end

---@return number
function EquipmentDesBaseData:protocolId()
    return 212
end

---@return string
function EquipmentDesBaseData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            desId = self.desId,
            desStr = self.desStr,
            name = self.name,
            storyDesStr = self.storyDesStr
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return EquipmentDesBaseData
function EquipmentDesBaseData:read(data)

    local packet = self:new(
            data.desId,
            data.desStr,
            data.name,
            data.storyDesStr)
    return packet
end

--- 介绍id
---@return number 介绍id
function EquipmentDesBaseData:getDesId()
    return self.desId
end
--- 介绍
---@type  string 介绍
function EquipmentDesBaseData:getDesStr()
    return self.desStr
end

--- 这个介绍圣遗物的名字
---@type  string 这个介绍圣遗物的名字
function EquipmentDesBaseData:getName()
    return self.name
end

--- 故事
---@type  string 故事
function EquipmentDesBaseData:getStoryDesStr()
    return self.storyDesStr
end



return EquipmentDesBaseData

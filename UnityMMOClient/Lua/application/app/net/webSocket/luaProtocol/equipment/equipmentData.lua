---@class EquipmentData
local EquipmentData = class("EquipmentData")
function EquipmentData:ctor()
    --- 副词条多个词条
    ---@type  table<number,EquipmentGlossaryData>
    self.AdverbStripEquipmentDataList = {}
    --- 主属性词条
    ---@type EquipmentGlossaryData
    self.SubjectClauseEquipmentData = {}
    --- 当前圣遗物装备id
    ---@type number
    self.equipmentId = 0
    --- 等级
    ---@type number
    self.equipmentLv = 0
    --- 当前圣遗物最大等级
    ---@type number
    self.equipmentMaxLv = 0
    --- 圣遗物升级的最大经验
    ---@type number
    self.maxEquipmentExp = 0
    --- 圣遗物升级的当前经验
    ---@type number
    self.nowEquipmentExp = 0
    --- list包含当前等级之前圣遗物转换经验值
    ---@type  table<number,number>
    self.rankSwitchingExperienceList = {}
    --- 当前装备所属角色id
    ---@type number
    self.useTheRole = 0
end

---@param AdverbStripEquipmentDataList table<number,EquipmentGlossaryData> 副词条多个词条
---@param SubjectClauseEquipmentData EquipmentGlossaryData 主属性词条
---@param equipmentId number 当前圣遗物装备id
---@param equipmentLv number 等级
---@param equipmentMaxLv number 当前圣遗物最大等级
---@param maxEquipmentExp number 圣遗物升级的最大经验
---@param nowEquipmentExp number 圣遗物升级的当前经验
---@param rankSwitchingExperienceList table<number,number> list包含当前等级之前圣遗物转换经验值
---@param useTheRole number 当前装备所属角色id
---@return EquipmentData
function EquipmentData:new(AdverbStripEquipmentDataList, SubjectClauseEquipmentData, equipmentId, equipmentLv, equipmentMaxLv, maxEquipmentExp, nowEquipmentExp, rankSwitchingExperienceList, useTheRole)
    self.AdverbStripEquipmentDataList = AdverbStripEquipmentDataList --- java.util.List<com.gameServer.common.protocol.equipment.EquipmentGlossaryData>
    self.SubjectClauseEquipmentData = SubjectClauseEquipmentData --- com.gameServer.common.protocol.equipment.EquipmentGlossaryData
    self.equipmentId = equipmentId --- int
    self.equipmentLv = equipmentLv --- int
    self.equipmentMaxLv = equipmentMaxLv --- int
    self.maxEquipmentExp = maxEquipmentExp --- int
    self.nowEquipmentExp = nowEquipmentExp --- int
    self.rankSwitchingExperienceList = rankSwitchingExperienceList --- java.util.List<java.lang.Integer>
    self.useTheRole = useTheRole --- int
    return self
end

---@return number
function EquipmentData:protocolId()
    return 206
end

---@return string
function EquipmentData:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            AdverbStripEquipmentDataList = self.AdverbStripEquipmentDataList,
            SubjectClauseEquipmentData = self.SubjectClauseEquipmentData,
            equipmentId = self.equipmentId,
            equipmentLv = self.equipmentLv,
            equipmentMaxLv = self.equipmentMaxLv,
            maxEquipmentExp = self.maxEquipmentExp,
            nowEquipmentExp = self.nowEquipmentExp,
            rankSwitchingExperienceList = self.rankSwitchingExperienceList,
            useTheRole = self.useTheRole
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return EquipmentData
function EquipmentData:read(data)
    local AdverbStripEquipmentDataList = {}
    for index, value in ipairs(data.AdverbStripEquipmentDataList) do
        local AdverbStripEquipmentDataListPacket = EquipmentGlossaryData()
        local packetData = AdverbStripEquipmentDataListPacket:read(value)
        table.insert(AdverbStripEquipmentDataList,packetData)
    end
    local SubjectClauseEquipmentDataPacket = EquipmentGlossaryData()
    local SubjectClauseEquipmentData = SubjectClauseEquipmentDataPacket:read(data.SubjectClauseEquipmentData)
    local rankSwitchingExperienceList = {}
    for index, value in ipairs(data.rankSwitchingExperienceList) do
        local packetData = value
        table.insert(rankSwitchingExperienceList,packetData)
    end

    local packet = self:new(
            AdverbStripEquipmentDataList,
            SubjectClauseEquipmentData,
            data.equipmentId,
            data.equipmentLv,
            data.equipmentMaxLv,
            data.maxEquipmentExp,
            data.nowEquipmentExp,
            rankSwitchingExperienceList,
            data.useTheRole)
    return packet
end

--- 副词条多个词条
---@type  table<number,EquipmentGlossaryData> 副词条多个词条
function EquipmentData:getAdverbStripEquipmentDataList()
    return self.AdverbStripEquipmentDataList
end
--- 主属性词条
---@return EquipmentGlossaryData 主属性词条
function EquipmentData:getEquipmentGlossaryData()
    return self.SubjectClauseEquipmentData
end
--- 当前圣遗物装备id
---@return number 当前圣遗物装备id
function EquipmentData:getEquipmentId()
    return self.equipmentId
end
--- 等级
---@return number 等级
function EquipmentData:getEquipmentLv()
    return self.equipmentLv
end
--- 当前圣遗物最大等级
---@return number 当前圣遗物最大等级
function EquipmentData:getEquipmentMaxLv()
    return self.equipmentMaxLv
end
--- 圣遗物升级的最大经验
---@return number 圣遗物升级的最大经验
function EquipmentData:getMaxEquipmentExp()
    return self.maxEquipmentExp
end
--- 圣遗物升级的当前经验
---@return number 圣遗物升级的当前经验
function EquipmentData:getNowEquipmentExp()
    return self.nowEquipmentExp
end
--- list包含当前等级之前圣遗物转换经验值
---@type  table<number,number> list包含当前等级之前圣遗物转换经验值
function EquipmentData:getRankSwitchingExperienceList()
    return self.rankSwitchingExperienceList
end
--- 当前装备所属角色id
---@return number 当前装备所属角色id
function EquipmentData:getUseTheRole()
    return self.useTheRole
end


return EquipmentData

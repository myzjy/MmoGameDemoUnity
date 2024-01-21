---@class WeaponPlayerUserDataStruct
local WeaponPlayerUserDataStruct = {}


---@param id number 武器id
---@param weaponName string 武器名字
---@param weaponType number 武器 type 武器所属类型
---@param nowSkills number 当前武器所属技能
---@param weaponMainValue number 武器主词条的数值
---@param weaponMainValueType number 武器主词条的所属type
---@param haveTimeAt string 武器获取到的第一时间
---@param nowLv number 当前武器强化到的等级
---@param nowMaxLv number 当前武器能强化到的最大等级 强化到特定等级会有突破 突破之后这个最大等级就会变化  直到没有可以突破等级
---@param nowLvExp number 当前 等级 已经强化 到的经验
---@param nowLvMaxExp number 当前等级已知的可以强化的最大经验
---@param weaponIcons string 武器Icon
---@param weaponModelNameIcons string 武器模型所属资源名
function WeaponPlayerUserDataStruct:new(id, weaponName, weaponType, nowSkills,
                                        weaponMainValue, weaponMainValueType, haveTimeAt, nowLv, nowMaxLv, nowLvExp,
                                        nowLvMaxExp, weaponIcons, weaponModelNameIcons)
    local object = {
        id = id,
        weaponName = weaponName,
        weaponType = weaponType,
        nowSkills = nowSkills,
        weaponMainValue = weaponMainValue,
        weaponMainValueType = weaponMainValueType,
        haveTimeAt = haveTimeAt,
        nowLv = nowLv,
        nowMaxLv = nowMaxLv,
        nowLvExp = nowLvExp,
        nowLvMaxExp = nowLvMaxExp,
        weaponIcons = weaponIcons,
        weaponModelNameIcons = weaponModelNameIcons,
    }
    setmetatable(object, self)
    -- 索引指向 自己
    self.__index = self
end

function WeaponPlayerUserDataStruct:protocolId()
    return 1040
end

---comment
---@param buffer ByteBuffer
---@param packet
function WeaponPlayerUserDataStruct:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or WeaponPlayerUserDataStruct
    local message = {
        protocolId = data:protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function WeaponPlayerUserDataStruct:read(buffer)
    local jsonStr = buffer:readString()
    ---@type { id :number,weaponName :string,weaponType:number,nowSkills:number ,weaponMainValue :number,weaponMainValueType :number,haveTimeAt :string,nowLv :number ,nowMaxLv: number , nowLvExp: number,nowLvMaxExp: number ,weaponIcons :string,weaponModelNameIcons :string}
    local data = JSON.decode(jsonStr)
    local jsonData = WeaponPlayerUserDataStruct:new(
        data.id,
        data.weaponName,
        data.weaponType,
        data.nowSkills,
        data.weaponMainValue,
        data.weaponMainValueType,
        data.haveTimeAt,
        data.nowLv,
        data.nowMaxLv,
        data.nowLvExp,
        data.nowLvMaxExp,
        data.weaponIcons,
        data.weaponModelNameIcons)
    return jsonData
end

return WeaponPlayerUserDataStruct

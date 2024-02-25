---武器
---@class WeaponPlayerUserDataStruct
local WeaponPlayerUserDataStruct = class("WeaponPlayerUserDataStruct")
function WeaponPlayerUserDataStruct:ctor()
    printDebug("init WeaponPlayerUserDataStruct ctor")
    ---武器id
    ---@type number
    self.id = -1 ---java.lang.long

    ---武器名字
    ---@type string
    self.weaponName = string.empty ---java.lang.string

    ---武器 type 武器所属类型
    ---@type number
    self.weaponType = -1 ---java.lang.integer

    ---当前武器所属技能
    ---@type number
    self.nowSkills = -1 ---java.lang.integer

    ---武器主词条的数值
    ---@type number
    self.weaponMainValue = -1 ---java.lang.integer

    ---武器主词条的所属type
    ---@type number
    self.weaponMainValueType = -1 ---java.lang.integer

    ---武器获取到的第一时间
    ---@type string
    self.haveTimeAt = string.empty ---java.lang.string

    ---当前武器强化到的等级
    ---@type number
    self.nowLv = -1 ---java.lang.integer

    ---当前武器能强化到的最大等级
    ---@type number
    self.nowMaxLv = -1 ---java.lang.integer

    ---当前 等级 已经强化 到的经验
    ---@type number
    self.nowLvExp = -1 ---java.lang.integer

    ---前等级已知的可以强化的最大经验
    ---@type number
    self.nowLvMaxExp = -1 ---java.lang.integer

    ---武器Icon
    ---@type string
    self.weaponIcons = string.empty ---java.lang.string

    ---武器模型所属资源名
    ---@type string
    self.weaponModelNameIcons = string.empty ---java.lang.string
end

---@param id number 武器id
---@param weaponName string 武器名字
---@param weaponType integer 武器 type 武器所属类型
---@param nowSkills integer 当前武器所属技能
---@param weaponMainValue integer 武器主词条的数值
---@param weaponMainValueType integer 武器主词条的所属type
---@param haveTimeAt string 武器获取到的第一时间
---@param nowLv integer 当前武器强化到的等级
---@param nowMaxLv integer 当前武器能强化到的最大等级
---@param nowLvExp integer 当前 等级 已经强化 到的经验
---@param nowLvMaxExp integer 前等级已知的可以强化的最大经验
---@param weaponIcons string 武器Icon
---@param weaponModelNameIcons string 武器模型所属资源名
function WeaponPlayerUserDataStruct:new(
    id,
    weaponName,
    weaponType,
    nowSkills,
    weaponMainValue,
    weaponMainValueType,
    haveTimeAt,
    nowLv,
    nowMaxLv,
    nowLvExp,
    nowLvMaxExp,
    weaponIcons,
    weaponModelNameIcons)
    self.id = id                                          ---java.lang.long
    self.weaponName = weaponName                          ---java.lang.string
    self.weaponType = weaponType                          ---java.lang.integer
    self.nowSkills = nowSkills                            ---java.lang.integer
    self.weaponMainValue = weaponMainValue                ---java.lang.integer
    self.weaponMainValueType = weaponMainValueType        ---java.lang.integer
    self.haveTimeAt = haveTimeAt                          ---java.lang.string
    self.nowLv = nowLv                                    ---java.lang.integer
    self.nowMaxLv = nowMaxLv                              ---java.lang.integer
    self.nowLvExp = nowLvExp                              ---java.lang.integer
    self.nowLvMaxExp = nowLvMaxExp                        ---java.lang.integer
    self.weaponIcons = weaponIcons                        ---java.lang.string
    self.weaponModelNameIcons = weaponModelNameIcons      ---java.lang.string


    return self
end

function WeaponPlayerUserDataStruct:protocolId()
    return 213
end

---@param buffer ByteBuffer
---@param packet any|nil
function WeaponPlayerUserDataStruct:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or WeaponPlayerUserDataStruct
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function WeaponPlayerUserDataStruct:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    ---@type {protocolId:number,packet:{id:number, weaponName:string, weaponType:integer, nowSkills:integer, weaponMainValue:integer, weaponMainValueType:integer, haveTimeAt:string, nowLv:integer, nowMaxLv:integer, nowLvExp:integer, nowLvMaxExp:integer, weaponIcons:string, weaponModelNameIcons:string}}
    local data = JSON.decode(jsonString)
    return WeaponPlayerUserDataStruct:new(
        data.packet.id,
        data.packet.weaponName,
        data.packet.weaponType,
        data.packet.nowSkills,
        data.packet.weaponMainValue,
        data.packet.weaponMainValueType,
        data.packet.haveTimeAt,
        data.packet.nowLv,
        data.packet.nowMaxLv,
        data.packet.nowLvExp,
        data.packet.nowLvMaxExp,
        data.packet.weaponIcons,
        data.packet.weaponModelNameIcons
    )
end

function WeaponPlayerUserDataStruct:readData(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    ---@type {id:number, weaponName:string, weaponType:integer, nowSkills:integer, weaponMainValue:integer, weaponMainValueType:integer, haveTimeAt:string, nowLv:integer, nowMaxLv:integer, nowLvExp:integer, nowLvMaxExp:integer, weaponIcons:string, weaponModelNameIcons:string}
    local data = JSON.decode(jsonString)
    return WeaponPlayerUserDataStruct:new(
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
        data.weaponModelNameIcons
    )
end

return WeaponPlayerUserDataStruct

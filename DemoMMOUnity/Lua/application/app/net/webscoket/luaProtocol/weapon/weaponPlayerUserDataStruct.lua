---@class WeaponPlayerUserDataStruct
local WeaponPlayerUserDataStruct = {}


---@param id number
---@param weaponName string
---@param weaponType integer
---@param nowSkills integer
---@param weaponMainValue integer
---@param weaponMainValueType integer
---@param haveTimeAt string
---@param nowLv integer
---@param nowMaxLv integer
---@param nowLvExp integer
---@param nowLvMaxExp integer
---@param weaponIcons string
---@param weaponModelNameIcons string
function WeaponPlayerUserDataStruct:new(id, weaponName,
                                        weaponType, nowSkills, weaponMainValue, weaponMainValueType, haveTimeAt,
                                        nowLv, nowMaxLv, nowLvExp, nowLvMaxExp, weaponIcons, weaponModelNameIcons)
    local obj = {
        id = id,                                    ---java.lang.long
        weaponName = weaponName,                    ---java.lang.string
        weaponType = weaponType,                    ---java.lang.integer
        nowSkills = nowSkills,                      ---java.lang.integer
        weaponMainValue = weaponMainValue,          ---java.lang.integer
        weaponMainValueType = weaponMainValueType,  ---java.lang.integer
        haveTimeAt = haveTimeAt,                    ---java.lang.string
        nowLv = nowLv,                              ---java.lang.integer
        nowMaxLv = nowMaxLv,                        ---java.lang.integer
        nowLvExp = nowLvExp,                        ---java.lang.integer
        nowLvMaxExp = nowLvMaxExp,                  ---java.lang.integer
        weaponIcons = weaponIcons,                  ---java.lang.string
        weaponModelNameIcons = weaponModelNameIcons ---java.lang.string
    }
    setmetatable(obj, self)
    self.__index = self
    return obj
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
    return WeaponPlayerUserDataStruct:new(data.packet.id, data.packet.weaponName,
        data.packet.weaponType, data.packet.nowSkills, data.packet.weaponMainValue,
        data.packet.weaponMainValueType, data.packet.haveTimeAt,
        data.packet.nowLv, data.packet.nowMaxLv,
        data.packet.nowLvExp, data.packet.nowLvMaxExp, data.packet.weaponIcons,
        data.packet.weaponModelNameIcons)
end

function WeaponPlayerUserDataStruct:readData(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    ---@type {id:number, weaponName:string, weaponType:integer, nowSkills:integer, weaponMainValue:integer, weaponMainValueType:integer, haveTimeAt:string, nowLv:integer, nowMaxLv:integer, nowLvExp:integer, nowLvMaxExp:integer, weaponIcons:string, weaponModelNameIcons:string}
    local data = JSON.decode(jsonString)
    return WeaponPlayerUserDataStruct:new(data.id, data.weaponName,
        data.weaponType, data.nowSkills, data.weaponMainValue,
        data.weaponMainValueType, data.haveTimeAt,
        data.nowLv, data.nowMaxLv,
        data.nowLvExp, data.nowLvMaxExp, data.weaponIcons,
        data.weaponModelNameIcons)
end

return WeaponPlayerUserDataStruct

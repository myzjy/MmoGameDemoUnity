---@class WeaponsConfigData
local WeaponsConfigData = class("WeaponsConfigData")

function WeaponsConfigData:ctor()
    self.id = -1;

    self.weaponNam = string.empty;

    self.weaponType = 1;

    self.weaponSkills = 1;

    self.weaponInitValue = string.empty;

    self.weaponInitProgress = string.empty;

    self.iconResource = string.empty;

    self.weaponBreakthrough = string.empty;

    self.maxLv = 1;
end

---@param id integer
---@param weaponName string
---@param weaponType integer
---@param weaponSkills integer
---@param weaponInitValue string
---@param weaponInitProgress string
---@param iconResource string
---@param weaponBreakthrough string
---@param maxLv integer
function WeaponsConfigData:new(id, weaponName, weaponType, weaponSkills, weaponInitValue, weaponInitProgress,
                               iconResource,
                               weaponBreakthrough, maxLv)
    self.id = id;
    self.weaponName = weaponName
    self.weaponType = weaponType
    self.weaponSkills = weaponSkills
    self.weaponInitValue = weaponInitValue
    self.iconResource = iconResource
    self.weaponInitProgress = weaponInitProgress
    self.weaponBreakthrough = weaponBreakthrough
    self.maxLv = maxLv
    return self;
end
function WeaponsConfigData:protocolId()
    return 214
end

---@param buffer ByteBuffer
---@param packet any|nil
function WeaponsConfigData:write(buffer, packet)
    if packet == nil then
        return
    end
    local data = packet or WeaponsConfigData
    local message = {
        protocolId = data.protocolId(),
        packet = data
    }
    local jsonStr = JSON.encode(message)
    buffer:writeString(jsonStr)
end

function WeaponsConfigData:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    ---@type {protocolId:number,packet:{}}
    local data = JSON.decode(jsonString)
    return WeaponsConfigData:new(
        data.packet.id,
        data.packet.weaponName,
        data.packet.weaponType,
        data.packet.nowSkills,
        data.packet.weaponMainValue,
        data.packet.weaponMainValueType,
        data.packet.haveTimeAt,
        data.packet.nowLv,
        data.packet.nowMaxLv
    )
end

function WeaponsConfigData:readData(data)

    return WeaponsConfigData:new(
        data.id,
        data.weaponName,
        data.weaponType,
        data.nowSkills,
        data.weaponMainValue,
        data.weaponMainValueType,
        data.haveTimeAt,
        data.nowLv,
        data.nowMaxLv
    )
end
return WeaponsConfigData

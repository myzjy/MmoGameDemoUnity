---@class BagUserItemMsgData
local BagUserItemMsgData = class("BagUserItemMsgData")
--- init data
---@param id number
---@param masterUserId number
---@param userPlayerId number
---@param nowItemNum number
---@param itemId number
---@param quality number
---@param resourcePath string
---@param icon string
---@param itemNew boolean
function BagUserItemMsgData:ctor(id, masterUserId, userPlayerId, nowItemNum, itemId, quality, resourcePath, icon, itemNew)
    self.id = id;
    self.masterUserId = masterUserId;
    self.userPlayerId = userPlayerId;
    self.nowItemNum = nowItemNum;
    self.itemId = itemId;
    self.quality = quality;
    self.resourcePath = resourcePath;
    self.icon = icon;
    self.itemNew = itemNew;
end

--- return self protocolId
---@return integer
function BagUserItemMsgData:protocolId()
    return 200
end

function BagUserItemMsgData:read(buffer)
    local jsonString = buffer:readString()
    ---字节读取器中存放字符
    local data = JSON.decode(jsonString)
    return BagUserItemMsgData(data._id,
        data.masterUserId, data.userPlayerId,
        data.nowItemNum, data.itemId,
        data.quality, data.resourcePath,
        data.icon, data.itemNew)
end

return BagUserItemMsgData

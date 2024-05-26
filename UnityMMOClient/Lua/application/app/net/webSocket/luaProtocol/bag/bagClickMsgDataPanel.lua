---@class BagClickMsgDataPanel
local BagClickMsgDataPanel = class("BagClickMsgDataPanel")
function BagClickMsgDataPanel:ctor()
    --- id
    ---@type number
    self.id = 0
end

---@param id number id
---@return BagClickMsgDataPanel
function BagClickMsgDataPanel:new(id)
    self.id = id --- int
    return self
end

---@return number
function BagClickMsgDataPanel:protocolId()
    return 215
end

---@return string
function BagClickMsgDataPanel:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            id = self.id
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return BagClickMsgDataPanel
function BagClickMsgDataPanel:read(data)

    local packet = self:new(
            data.id)
    return packet
end

--- id
---@return number id
function BagClickMsgDataPanel:getId()
    return self.id
end


return BagClickMsgDataPanel

---@class RefreshingResourcesMainRequest
local RefreshingResourcesMainRequest = class("RefreshingResourcesMainRequest")
function RefreshingResourcesMainRequest:ctor()
    
end


---@return RefreshingResourcesMainRequest
function RefreshingResourcesMainRequest:new()
    
    return self
end

---@return number
function RefreshingResourcesMainRequest:protocolId()
    return 1027
end

---@return string
function RefreshingResourcesMainRequest:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return RefreshingResourcesMainRequest
function RefreshingResourcesMainRequest:read(data)

    local packet = self:new(
            )
    return packet
end



return RefreshingResourcesMainRequest

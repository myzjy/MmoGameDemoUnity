---@class LoginCreateCharacterAnswer
local LoginCreateCharacterAnswer = class("LoginCreateCharacterAnswer")
function LoginCreateCharacterAnswer:ctor()
    
end


---@return LoginCreateCharacterAnswer
function LoginCreateCharacterAnswer:new()
    
    return self
end

---@return number
function LoginCreateCharacterAnswer:protocolId()
    return 6003
end

---@return string
function LoginCreateCharacterAnswer:write()
    local message = {
        protocolId = self:protocolId(),
        packet = {
            
        }
    }
    local jsonStr = JSON.encode(message)
    return jsonStr
end

---@return LoginCreateCharacterAnswer
function LoginCreateCharacterAnswer:read(data)

    local packet = self:new(
            )
    return packet
end



return LoginCreateCharacterAnswer

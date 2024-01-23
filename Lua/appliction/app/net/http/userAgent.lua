UserAgent = class("UserAgent")
local cached = string.empty

local appVersion = string.empty

function UserAgent:Value()
    if string.IsNullOrEmtty(cached) then
        local appName = CS.UnityEngine.Application.identifier
        local version = CS.UnityEngine.Application.version
        local operatingSystem = CS.UnityEngine.SystemInfo.operatingSystem
        local device = CS.UnityEngine.SystemInfo.deviceModel
        cached = appName .. "/" .. version .. "(" .. operatingSystem .. "; " .. device .. ")"
    end
    return cached
end

---@return string
function UserAgent:Version()
    if string.IsNullOrEmtty(appVersion) then
        appVersion = CS.UnityEngine.Application.version
    end
    return appVersion
end
